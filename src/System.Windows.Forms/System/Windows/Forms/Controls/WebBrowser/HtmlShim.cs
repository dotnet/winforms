// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.ExceptionServices;
using Windows.Win32.Web.MsHtml;

namespace System.Windows.Forms;

/// <summary>
///  Owns the native event connections and managed delegates for an HTML wrapper.
/// </summary>
/// <remarks>
///  <para>
///   Connection-point events and individually attached dispatch proxies have independent lifetimes.
///   Removing the last standard handler must not detach proxies registered through AttachEventHandler.
///  </para>
/// </remarks>
internal abstract class HtmlShim : IDisposable
{
    private EventHandlerList? _events;
    private int _eventCount;
    private List<(EventHandler Handler, HtmlToClrEventProxy Proxy)>? _attachedEventList;

    internal bool IsDisposed { get; private set; }

    protected HtmlShim()
    {
    }

    private EventHandlerList Events =>
        _events ??= new EventHandlerList();

    public void AttachEventHandler(string eventName, EventHandler eventHandler)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        ArgumentNullException.ThrowIfNull(eventHandler);

        HtmlToClrEventProxy proxy = new(eventName, eventHandler);
        if (!AttachEventProxy(proxy))
        {
            // MSHTML rejects some names (including null) with S_OK/false. Preserve the existing
            // no-op behavior, but do not retain a proxy for a registration that never happened.
            Debug.WriteLine("The native HTML object did not attach the event.");
            return;
        }

        // A native call can reenter disposal. Do not leave a new connection outside the disposed owner.
        if (IsDisposed)
        {
            DetachEventProxy(proxy);
            throw new ObjectDisposedException(GetType().Name);
        }

        // Each attach creates a distinct native dispatch identity, even for the same name and delegate.
        // Retaining only one proxy loses the information needed to detach the earlier registrations.
        (_attachedEventList ??= []).Add((eventHandler, proxy));
    }

    protected abstract bool AttachEventProxy(HtmlToClrEventProxy proxy);

    protected abstract void DetachEventProxy(HtmlToClrEventProxy proxy);

    public void AddHandler(object key, Delegate? value)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        if (value is null)
        {
            return;
        }

        _eventCount += value.GetInvocationList().Length;
        Events.AddHandler(key, value);
        OnEventHandlerAdded();
    }

    public void DetachEventHandler(string eventName, EventHandler eventHandler)
    {
        if (_attachedEventList is not { } registrations)
        {
            return;
        }

        for (int index = registrations.Count - 1; index >= 0; index--)
        {
            var registration = registrations[index];
            if (registration.Handler != eventHandler || registration.Proxy.EventName != eventName)
            {
                continue;
            }

            // Remove before the COM call so a reentrant detach cannot select the same proxy.
            registrations.RemoveAt(index);
            try
            {
                DetachEventProxy(registration.Proxy);
            }
            catch (Exception exception) when (!exception.IsCriticalException())
            {
                if (!IsDisposed)
                {
                    // A failed detach must remain owned and retryable, not become an untracked native sink.
                    (_attachedEventList ??= []).Insert(Math.Min(index, _attachedEventList.Count), registration);
                }

                throw;
            }

            return;
        }
    }

    public abstract IHTMLWindow2.Interface? AssociatedWindow { get; }

    /// create connectionpoint cookie
    public abstract void ConnectToEvents();

    public abstract void DisconnectFromEvents();

    /// return the sender for events, usually the HtmlWindow, HtmlElement, HtmlDocument
    protected abstract object GetEventSender();

    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            var registrations = _attachedEventList;
            _attachedEventList = null;
            _events?.Dispose();
            _events = null;
            _eventCount = 0;

            List<Exception>? exceptions = null;
            try
            {
                DisconnectFromEvents();
            }
            catch (Exception exception) when (!exception.IsCriticalException())
            {
                (exceptions ??= []).Add(exception);
            }

            if (registrations is not null)
            {
                foreach (var registration in registrations)
                {
                    try
                    {
                        DetachEventProxy(registration.Proxy);
                    }
                    catch (Exception exception) when (!exception.IsCriticalException())
                    {
                        (exceptions ??= []).Add(exception);
                    }
                }
            }

            // Attempt every independent release before reporting failures; one failed native detach
            // must not leave unrelated event sources and their subscribers connected.
            ThrowCleanupExceptions(exceptions);
        }
    }

    public void FireEvent(object key, EventArgs e)
    {
        Delegate? delegateToInvoke = _events?[key];

        if (delegateToInvoke is not null)
        {
            try
            {
                delegateToInvoke.DynamicInvoke(GetEventSender(), e);
            }
            catch (Exception ex)
            {
                // Note: this check is for the debugger, so we can catch exceptions in the debugger instead of
                // throwing a thread exception.
                if (NativeWindow.WndProcShouldBeDebuggable)
                {
                    throw;
                }
                else
                {
                    Application.OnThreadException(ex);
                }
            }
        }
    }

    protected virtual void OnEventHandlerAdded()
    {
        ConnectToEvents();
    }

    protected virtual void OnEventHandlerRemoved()
    {
        if (_eventCount <= 0)
        {
            DisconnectFromEvents();
            _eventCount = 0;
        }
    }

    public void RemoveHandler(object key, Delegate? value)
    {
        if (_events is null || value is null)
        {
            return;
        }

        int before = _events[key]?.GetInvocationList().Length ?? 0;
        _events.RemoveHandler(key, value);
        int removed = before - (_events[key]?.GetInvocationList().Length ?? 0);
        if (removed > 0)
        {
            // Unmatched -= is a no-op. Counting it (or counting a multicast delegate as one)
            // disconnects the native source while valid handlers are still stored in EventHandlerList.
            _eventCount -= removed;
            OnEventHandlerRemoved();
        }
    }

    internal static void DisposeAll(IEnumerable<HtmlShim> shims)
    {
        List<Exception>? exceptions = null;
        foreach (HtmlShim shim in shims)
        {
            try
            {
                shim.Dispose();
            }
            catch (Exception exception) when (!exception.IsCriticalException())
            {
                (exceptions ??= []).Add(exception);
            }
        }

        ThrowCleanupExceptions(exceptions);
    }

    private static void ThrowCleanupExceptions(List<Exception>? exceptions)
    {
        if (exceptions is { Count: 1 })
        {
            ExceptionDispatchInfo.Throw(exceptions[0]);
        }

        if (exceptions is { Count: > 1 })
        {
            throw new AggregateException(exceptions);
        }
    }
}

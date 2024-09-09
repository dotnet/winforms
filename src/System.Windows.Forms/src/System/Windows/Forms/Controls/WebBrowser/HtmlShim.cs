// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Windows.Win32.Web.MsHtml;

namespace System.Windows.Forms;

///  This is essentially a proxy object between the native
///  html objects and our managed ones. We want the managed
///  HtmlDocument, HtmlWindow and HtmlElement to be super-lightweight,
///  which means that we shouldn't have things that tie up their lifetimes
///  contained within them. The "Shim" is essentially the object that
///  manages events coming out of the HtmlDocument, HtmlElement and HtmlWindow
///  and serves them back up to the user.

internal abstract class HtmlShim : IDisposable
{
    private EventHandlerList? _events;
    private int _eventCount;
    private Dictionary<EventHandler, HtmlToClrEventProxy>? _attachedEventList;

    protected HtmlShim()
    {
    }

    private EventHandlerList Events =>
        _events ??= new EventHandlerList();

    ///  Support IHtml*3.AttachHandler
    public abstract void AttachEventHandler(string eventName, EventHandler eventHandler);

    public void AddHandler(object key, Delegate? value)
    {
        _eventCount++;
        Events.AddHandler(key, value);
        OnEventHandlerAdded();
    }

    protected HtmlToClrEventProxy AddEventProxy(string eventName, EventHandler eventHandler)
    {
        _attachedEventList ??= [];

        HtmlToClrEventProxy proxy = new(eventName, eventHandler);
        _attachedEventList[eventHandler] = proxy;
        return proxy;
    }

    public abstract IHTMLWindow2.Interface? AssociatedWindow { get; }

    ///  create connectionpoint cookie
    public abstract void ConnectToEvents();

    ///  Support IHtml*3.DetachEventHandler
    public abstract void DetachEventHandler(string eventName, EventHandler eventHandler);

    ///  disconnect from connectionpoint cookie
    ///  inheriting classes should override to disconnect from ConnectionPoint and call base.
    public virtual void DisconnectFromEvents()
    {
        if (_attachedEventList is not null)
        {
            EventHandler[] events = new EventHandler[_attachedEventList.Count];
            _attachedEventList.Keys.CopyTo(events, 0);

            foreach (EventHandler eh in events)
            {
                HtmlToClrEventProxy proxy = _attachedEventList[eh];
                DetachEventHandler(proxy.EventName, eh);
            }
        }
    }

    ///  return the sender for events, usually the HtmlWindow, HtmlElement, HtmlDocument
    protected abstract object GetEventSender();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DisconnectFromEvents();
            _events?.Dispose();
            _events = null;
        }
    }

    public void FireEvent(object key, EventArgs e)
    {
        Delegate? delegateToInvoke = Events[key];

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
        _eventCount--;
        Events.RemoveHandler(key, value);
        OnEventHandlerRemoved();
    }

    protected HtmlToClrEventProxy? RemoveEventProxy(EventHandler eventHandler)
    {
        if (_attachedEventList is null)
        {
            return null;
        }

        if (_attachedEventList.Remove(eventHandler, out HtmlToClrEventProxy? proxy))
        {
            return proxy;
        }

        return null;
    }
}

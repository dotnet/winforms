// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.Web.MsHtml;

namespace System.Windows.Forms;

/// <summary>
///  HtmlShimManager - this class manages the shims for HtmlWindows, HtmlDocuments, and HtmlElements.
///  essentially we need a long-lasting object to call back on events from the web browser, and the
///  manager is the one in charge of making sure this list stays around as long as needed.
///
///  When a HtmlWindow unloads we prune our list of corresponding document, window, and element shims.
/// </summary>
internal sealed class HtmlShimManager : IDisposable
{
    private Dictionary<HtmlWindow, HtmlWindow.HtmlWindowShim>? _htmlWindowShims;
    private Dictionary<HtmlElement, HtmlElement.HtmlElementShim>? _htmlElementShims;
    private Dictionary<HtmlDocument, HtmlDocument.HtmlDocumentShim>? _htmlDocumentShims;
    private bool _disposed;

    internal HtmlShimManager()
    {
    }

    /// <summary>
    ///  Adds a <see cref="HtmlDocument.HtmlDocumentShim"/> to list of shims to manage.
    ///  Can create a WindowShim as a side effect so it knows when to self prune from the list.
    /// </summary>
    public void AddDocumentShim(HtmlDocument doc)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ObjectDisposedException.ThrowIf(doc.NativeHtmlDocument2.IsDisposed, doc);
        HtmlDocument.HtmlDocumentShim? shim = null;

        if (_htmlDocumentShims is null)
        {
            _htmlDocumentShims = [];
            shim = new HtmlDocument.HtmlDocumentShim(doc);
            _htmlDocumentShims[doc] = shim;
        }
        else if (!_htmlDocumentShims.ContainsKey(doc))
        {
            shim = new HtmlDocument.HtmlDocumentShim(doc);
            _htmlDocumentShims[doc] = shim;
        }

        if (shim is not null)
        {
            OnShimAdded(shim);
        }
    }

    /// <summary>
    ///  Adds a <see cref="HtmlWindow.HtmlWindowShim"/> to list of shims to manage.
    /// </summary>
    public void AddWindowShim(HtmlWindow window)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ObjectDisposedException.ThrowIf(window.NativeHtmlWindow.IsDisposed, window);
        HtmlWindow.HtmlWindowShim? shim = null;
        if (_htmlWindowShims is null)
        {
            _htmlWindowShims = [];
            shim = new HtmlWindow.HtmlWindowShim(window);
            _htmlWindowShims[window] = shim;
        }
        else if (!_htmlWindowShims.ContainsKey(window))
        {
            shim = new HtmlWindow.HtmlWindowShim(window);
            _htmlWindowShims[window] = shim;
        }

        // Reuse an existing shim as well: application subscriptions may have created it first.
        (shim ?? _htmlWindowShims[window]).EnsureWindowObservation();
    }

    /// <summary> AddElementShim - adds a HtmlDocumentShim to list of shims to manage
    ///  Can create a WindowShim as a side effect so it knows when to self prune from the list.
    /// </summary>
    public void AddElementShim(HtmlElement element)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ObjectDisposedException.ThrowIf(element.NativeHtmlElement.IsDisposed, element);
        HtmlElement.HtmlElementShim? shim = null;

        if (_htmlElementShims is null)
        {
            _htmlElementShims = [];
            shim = new HtmlElement.HtmlElementShim(element);
            _htmlElementShims[element] = shim;
        }
        else if (!_htmlElementShims.ContainsKey(element))
        {
            shim = new HtmlElement.HtmlElementShim(element);
            _htmlElementShims[element] = shim;
        }

        if (shim is not null)
        {
            OnShimAdded(shim);
        }
    }

    internal HtmlDocument.HtmlDocumentShim? GetDocumentShim(HtmlDocument document)
    {
        // Dictionary equality queries COM identity. A revoked wrapper can share a cached hash with
        // a live replacement after navigation, so reject it before the comparer queries its dead registration.
        if (_htmlDocumentShims is null || document.NativeHtmlDocument2.IsDisposed)
        {
            return null;
        }

        if (_htmlDocumentShims.TryGetValue(document, out HtmlDocument.HtmlDocumentShim? value))
        {
            return value;
        }

        return null;
    }

    internal HtmlElement.HtmlElementShim? GetElementShim(HtmlElement element)
    {
        if (_htmlElementShims is null || element.NativeHtmlElement.IsDisposed)
        {
            return null;
        }

        if (_htmlElementShims.TryGetValue(element, out HtmlElement.HtmlElementShim? elementShim))
        {
            return elementShim;
        }

        return null;
    }

    internal HtmlWindow.HtmlWindowShim? GetWindowShim(HtmlWindow window)
    {
        if (_htmlWindowShims is null || window.NativeHtmlWindow.IsDisposed)
        {
            return null;
        }

        if (_htmlWindowShims.TryGetValue(window, out HtmlWindow.HtmlWindowShim? windowShim))
        {
            return windowShim;
        }

        return null;
    }

    private unsafe void OnShimAdded(HtmlShim addedShim)
    {
        IHTMLWindow2.Interface? associatedWindow = addedShim.AssociatedWindow;
        if (associatedWindow is null)
        {
            return;
        }

        using var nativeWindow = ComHelpers.GetComScope<IHTMLWindow2>(associatedWindow);
        if (_htmlWindowShims is not null)
        {
            foreach (var (window, shim) in _htmlWindowShims)
            {
                if (window.NativeHtmlWindow.IsSameNativeObject(nativeWindow.Value))
                {
                    shim.EnsureWindowObservation();
                    return;
                }
            }
        }

        // Only create a wrapper when the manager needs a new owner. A discarded duplicate wrapper
        // would leave another GIT registration waiting for finalization on every shim addition.
        AddWindowShim(new HtmlWindow(this, ComHelpers.GetComPointer<IHTMLWindow2>(associatedWindow)));
    }

    /// <summary>
    ///  HtmlWindowShim calls back on us when it has unloaded the page. At this point we need to
    ///  walk through our lists and make sure we've cleaned up
    /// </summary>
    internal void OnWindowUnloaded(HtmlWindow unloadedWindow)
    {
        if (_disposed)
        {
            return;
        }

        List<HtmlShim> unloadedShims = [];
        if (_htmlDocumentShims is not null)
        {
            foreach (HtmlDocument.HtmlDocumentShim shim in _htmlDocumentShims.Values.ToArray())
            {
                if (IsAssociatedWindow(shim, unloadedWindow))
                {
                    _htmlDocumentShims.Remove(shim.Document);
                    unloadedShims.Add(shim);
                }
            }
        }

        if (_htmlElementShims is not null)
        {
            foreach (HtmlElement.HtmlElementShim shim in _htmlElementShims.Values.ToArray())
            {
                if (IsAssociatedWindow(shim, unloadedWindow))
                {
                    _htmlElementShims.Remove(shim.Element);
                    unloadedShims.Add(shim);
                }
            }
        }

        if (_htmlWindowShims is not null
            && _htmlWindowShims.Remove(unloadedWindow, out HtmlWindow.HtmlWindowShim? windowShim))
        {
            unloadedShims.Add(windowShim);
        }

        // Remove all affected owners before native releases can reenter the manager. Attempt every
        // disposal even if one detach fails, without disturbing another frame's live subscriptions.
        HtmlShim.DisposeAll(unloadedShims);
    }

    private static unsafe bool IsAssociatedWindow(HtmlShim shim, HtmlWindow window)
    {
        IHTMLWindow2.Interface? associatedWindow = shim.AssociatedWindow;
        if (associatedWindow is null)
        {
            return false;
        }

        // RCWs and AgileComPointer wrappers are not comparable; COM identity must be queried
        // in the same apartment or the unloaded page's shims will never be removed.
        using var nativeWindow = ComHelpers.GetComScope<IHTMLWindow2>(associatedWindow);
        return window.NativeHtmlWindow.IsSameNativeObject(nativeWindow.Value);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        List<HtmlShim> shims = [];
        if (_htmlElementShims is not null)
        {
            shims.AddRange(_htmlElementShims.Values);
        }

        if (_htmlDocumentShims is not null)
        {
            shims.AddRange(_htmlDocumentShims.Values);
        }

        if (_htmlWindowShims is not null)
        {
            shims.AddRange(_htmlWindowShims.Values);
        }

        // A retained browser/wrapper must not retain disposed element shims through the manager.
        // Clear before releasing COM objects, which may call back into this manager.
        _htmlElementShims = null;
        _htmlDocumentShims = null;
        _htmlWindowShims = null;
        HtmlShim.DisposeAll(shims);
    }
}

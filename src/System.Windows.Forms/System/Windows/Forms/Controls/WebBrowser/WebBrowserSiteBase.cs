// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms;

/// <summary>
///  This class implements the necessary interfaces required for an ActiveX site.
///
///  This class is public, but has an internal constructor so that external
///  users can only reference the Type (cannot instantiate it directly).
///  Other classes have to inherit this class and expose it to the outside world.
///
///  This class does not have any public property/method/event by itself.
///  All implementations of the site interface methods are private, which
///  means that inheritors who want to override even a single method of one
///  of these interfaces will have to implement the whole interface.
/// </summary>
public unsafe class WebBrowserSiteBase :
    IOleControlSite.Interface,
    IOleInPlaceSite.Interface,
    IOleWindow.Interface,
    IOleClientSite.Interface,
    ISimpleFrameSite.Interface,
    IPropertyNotifySink.Interface,
    IDisposable
{
    private readonly WebBrowserBase _host;
    private AxHost.ConnectionPointCookie? _connectionPoint;

    //
    // The constructor takes an WebBrowserBase as a parameter, so unfortunately,
    // this cannot be used as a standalone site. It has to be used in conjunction
    // with WebBrowserBase. Perhaps we can change it in future.
    //
    internal WebBrowserSiteBase(WebBrowserBase h) => _host = h.OrThrowIfNull();

    /// <summary>
    ///  Dispose(release the cookie)
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    ///  Release the cookie if we're disposing
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopEvents();
        }
    }

    /// <summary>
    ///  Retrieves the WebBrowserBase object set in the constructor.
    /// </summary>
    internal WebBrowserBase Host => _host;

    // IOleControlSite methods:
    HRESULT IOleControlSite.Interface.OnControlInfoChanged() => HRESULT.S_OK;

    HRESULT IOleControlSite.Interface.LockInPlaceActive(BOOL fLock) => HRESULT.E_NOTIMPL;

    HRESULT IOleControlSite.Interface.GetExtendedControl(IDispatch** ppDisp)
    {
        if (ppDisp is null)
        {
            return HRESULT.E_POINTER;
        }

        *ppDisp = null;
        return HRESULT.E_NOTIMPL;
    }

    HRESULT IOleControlSite.Interface.TransformCoords(POINTL* pPtlHimetric, PointF* pPtfContainer, uint dwFlags)
    {
        if (pPtlHimetric is null || pPtfContainer is null)
        {
            return HRESULT.E_POINTER;
        }

        XFORMCOORDS coordinates = (XFORMCOORDS)dwFlags;
        if (coordinates.HasFlag(XFORMCOORDS.XFORMCOORDS_HIMETRICTOCONTAINER))
        {
            if (coordinates.HasFlag(XFORMCOORDS.XFORMCOORDS_SIZE))
            {
                pPtfContainer->X = WebBrowserHelper.HM2Pix(pPtlHimetric->x, WebBrowserHelper.LogPixelsX);
                pPtfContainer->Y = WebBrowserHelper.HM2Pix(pPtlHimetric->y, WebBrowserHelper.LogPixelsY);
            }
            else if (coordinates.HasFlag(XFORMCOORDS.XFORMCOORDS_POSITION))
            {
                pPtfContainer->X = WebBrowserHelper.HM2Pix(pPtlHimetric->x, WebBrowserHelper.LogPixelsX);
                pPtfContainer->Y = WebBrowserHelper.HM2Pix(pPtlHimetric->y, WebBrowserHelper.LogPixelsY);
            }
            else
            {
                return HRESULT.E_INVALIDARG;
            }
        }
        else if (coordinates.HasFlag(XFORMCOORDS.XFORMCOORDS_CONTAINERTOHIMETRIC))
        {
            if (coordinates.HasFlag(XFORMCOORDS.XFORMCOORDS_SIZE))
            {
                pPtlHimetric->x = WebBrowserHelper.Pix2HM((int)pPtfContainer->X, WebBrowserHelper.LogPixelsX);
                pPtlHimetric->y = WebBrowserHelper.Pix2HM((int)pPtfContainer->Y, WebBrowserHelper.LogPixelsY);
            }
            else if (coordinates.HasFlag(XFORMCOORDS.XFORMCOORDS_POSITION))
            {
                pPtlHimetric->x = WebBrowserHelper.Pix2HM((int)pPtfContainer->X, WebBrowserHelper.LogPixelsX);
                pPtlHimetric->y = WebBrowserHelper.Pix2HM((int)pPtfContainer->Y, WebBrowserHelper.LogPixelsY);
            }
            else
            {
                return HRESULT.E_INVALIDARG;
            }
        }
        else
        {
            return HRESULT.E_INVALIDARG;
        }

        return HRESULT.S_OK;
    }

    HRESULT IOleControlSite.Interface.TranslateAccelerator(MSG* pMsg, KEYMODIFIERS grfModifiers)
    {
        if (pMsg is null)
        {
            return HRESULT.E_POINTER;
        }

        Debug.Assert(!Host.GetAXHostState(WebBrowserHelper.s_siteProcessedInputKey), "Re-entering IOleControlSite.TranslateAccelerator!!!");
        Host.SetAXHostState(WebBrowserHelper.s_siteProcessedInputKey, true);

        Message msg = Message.Create(pMsg);
        try
        {
            bool f = Host.PreProcessControlMessage(ref msg) == PreProcessControlState.MessageProcessed;
            return f ? HRESULT.S_OK : HRESULT.S_FALSE;
        }
        finally
        {
            Host.SetAXHostState(WebBrowserHelper.s_siteProcessedInputKey, false);
        }
    }

    HRESULT IOleControlSite.Interface.OnFocus(BOOL fGotFocus) => HRESULT.S_OK;

    HRESULT IOleControlSite.Interface.ShowPropertyFrame() => HRESULT.E_NOTIMPL;

    // IOleClientSite methods:
    HRESULT IOleClientSite.Interface.SaveObject() => HRESULT.E_NOTIMPL;

    HRESULT IOleClientSite.Interface.GetMoniker(uint dwAssign, uint dwWhichMoniker, IMoniker** ppmk)
    {
        if (ppmk is null)
        {
            return HRESULT.E_POINTER;
        }

        *ppmk = null;
        return HRESULT.E_NOTIMPL;
    }

    HRESULT IOleClientSite.Interface.GetContainer(IOleContainer** ppContainer)
    {
        if (ppContainer is null)
        {
            return HRESULT.E_POINTER;
        }

        *ppContainer = ComHelpers.GetComPointer<IOleContainer>(Host.GetParentContainer());
        return HRESULT.S_OK;
    }

    HRESULT IOleClientSite.Interface.ShowObject()
    {
        if (Host.ActiveXState >= WebBrowserHelper.AXState.InPlaceActive)
        {
            HWND hwnd = HWND.Null;
            if (Host.AXInPlaceObject!.GetWindow(&hwnd).Succeeded)
            {
                if (Host.GetHandleNoCreate() != hwnd)
                {
                    if (!hwnd.IsNull)
                    {
                        Host.AttachWindow(hwnd);
                        RECT posRect = Host.Bounds;
                        OnActiveXRectChange(&posRect);
                    }
                }
            }
            else if (Host.AXInPlaceObject is IOleInPlaceObjectWindowless.Interface)
            {
                throw new InvalidOperationException(SR.AXWindowlessControl);
            }
        }

        return HRESULT.S_OK;
    }

    HRESULT IOleClientSite.Interface.OnShowWindow(BOOL fShow) => HRESULT.S_OK;

    HRESULT IOleClientSite.Interface.RequestNewObjectLayout() => HRESULT.E_NOTIMPL;

    // IOleInPlaceSite methods:
    unsafe HRESULT IOleInPlaceSite.Interface.GetWindow(HWND* phwnd)
    {
        if (phwnd is null)
        {
            return HRESULT.E_POINTER;
        }

        *phwnd = PInvoke.GetParent(Host);
        return HRESULT.S_OK;
    }

    HRESULT IOleInPlaceSite.Interface.ContextSensitiveHelp(BOOL fEnterMode) => HRESULT.E_NOTIMPL;

    HRESULT IOleInPlaceSite.Interface.CanInPlaceActivate() => HRESULT.S_OK;

    HRESULT IOleInPlaceSite.Interface.OnInPlaceActivate()
    {
        Host.ActiveXState = WebBrowserHelper.AXState.InPlaceActive;
        RECT posRect = Host.Bounds;
        OnActiveXRectChange(&posRect);
        return HRESULT.S_OK;
    }

    HRESULT IOleInPlaceSite.Interface.OnUIActivate()
    {
        Host.ActiveXState = WebBrowserHelper.AXState.UIActive;
        Host.GetParentContainer().OnUIActivate(Host);
        return HRESULT.S_OK;
    }

    HRESULT IOleInPlaceSite.Interface.GetWindowContext(
        IOleInPlaceFrame** ppFrame,
        IOleInPlaceUIWindow** ppDoc,
        RECT* lprcPosRect,
        RECT* lprcClipRect,
        OLEINPLACEFRAMEINFO* lpFrameInfo)
    {
        if (ppFrame is null || lprcPosRect is null || lprcClipRect is null)
        {
            return HRESULT.E_POINTER;
        }

        if (ppDoc is not null)
        {
            *ppDoc = null;
        }

        *ppFrame = ComHelpers.GetComPointer<IOleInPlaceFrame>(Host.GetParentContainer());

        *lprcPosRect = Host.Bounds;
        *lprcClipRect = WebBrowserHelper.GetClipRect();
        if (lpFrameInfo is not null)
        {
            lpFrameInfo->cb = (uint)Marshal.SizeOf<OLEINPLACEFRAMEINFO>();
            lpFrameInfo->fMDIApp = false;
            lpFrameInfo->haccel = HACCEL.Null;
            lpFrameInfo->cAccelEntries = 0;
            lpFrameInfo->hwndFrame = Host.ParentInternal is { } parent ? parent.HWND : HWND.Null;
        }

        return HRESULT.S_OK;
    }

    HRESULT IOleInPlaceSite.Interface.Scroll(SIZE scrollExtant) => HRESULT.S_FALSE;

    HRESULT IOleInPlaceSite.Interface.OnUIDeactivate(BOOL fUndoable)
    {
        Host.GetParentContainer().OnUIDeactivate(Host);
        if (Host.ActiveXState > WebBrowserHelper.AXState.InPlaceActive)
        {
            Host.ActiveXState = WebBrowserHelper.AXState.InPlaceActive;
        }

        return HRESULT.S_OK;
    }

    HRESULT IOleInPlaceSite.Interface.OnInPlaceDeactivate()
    {
        if (Host.ActiveXState == WebBrowserHelper.AXState.UIActive)
        {
            ((IOleInPlaceSite.Interface)this).OnUIDeactivate(false);
        }

        Host.GetParentContainer().OnInPlaceDeactivate(Host);
        Host.ActiveXState = WebBrowserHelper.AXState.Running;
        return HRESULT.S_OK;
    }

    HRESULT IOleInPlaceSite.Interface.DiscardUndoState() => HRESULT.S_OK;

    HRESULT IOleInPlaceSite.Interface.DeactivateAndUndo() => Host.AXInPlaceObject!.UIDeactivate();

    HRESULT IOleInPlaceSite.Interface.OnPosRectChange(RECT* lprcPosRect) => OnActiveXRectChange(lprcPosRect);

    // ISimpleFrameSite methods:
    HRESULT ISimpleFrameSite.Interface.PreMessageFilter(
        HWND hWnd,
        uint msg,
        WPARAM wp,
        LPARAM lp,
        LRESULT* plResult,
        uint* pdwCookie) => HRESULT.S_OK;

    HRESULT ISimpleFrameSite.Interface.PostMessageFilter(
        HWND hWnd,
        uint msg,
        WPARAM wp,
        LPARAM lp,
        LRESULT* plResult,
        uint dwCookie) => HRESULT.S_FALSE;

    // IPropertyNotifySink methods:
    HRESULT IPropertyNotifySink.Interface.OnChanged(int dispid)
    {
        // Some controls fire OnChanged() notifications when getting values of some properties.
        // To prevent this kind of recursion, we check to see if we are already inside a OnChanged() call.
        if (Host.NoComponentChangeEvents != 0)
        {
            return HRESULT.S_OK;
        }

        Host.NoComponentChangeEvents++;
        try
        {
            OnPropertyChanged(dispid);
        }
        catch (Exception t)
        {
            Debug.Fail(t.ToString());
            throw;
        }
        finally
        {
            Host.NoComponentChangeEvents--;
        }

        return HRESULT.S_OK;
    }

    HRESULT IPropertyNotifySink.Interface.OnRequestEdit(int dispid) => HRESULT.S_OK;

    internal virtual void OnPropertyChanged(int dispid)
    {
        if (Host.Site.TryGetService(out IComponentChangeService? changeService))
        {
            try
            {
                changeService.OnComponentChanging(Host);
                changeService.OnComponentChanged(Host);
            }
            catch (CheckoutException e) when (e == CheckoutException.Canceled)
            {
                return;
            }
        }
    }

    internal void StartEvents()
    {
        if (_connectionPoint is not null)
        {
            return;
        }

        object? nativeObject = Host._activeXInstance;
        if (nativeObject is not null)
        {
            try
            {
                _connectionPoint = new AxHost.ConnectionPointCookie(nativeObject, this, typeof(IPropertyNotifySink.Interface));
            }
#if DEBUG
            catch (Exception)
            {
                throw;
            }
#else
            catch (Exception ex) when (!ex.IsCriticalException())
            {
            }
#endif
        }
    }

    internal void StopEvents()
    {
        if (_connectionPoint is not null)
        {
            _connectionPoint.Disconnect();
            _connectionPoint = null;
        }
    }

    private unsafe HRESULT OnActiveXRectChange(RECT* lprcPosRect)
    {
        if (lprcPosRect is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        RECT posRect = new(0, 0, lprcPosRect->right - lprcPosRect->left, lprcPosRect->bottom - lprcPosRect->top);
        RECT clipRect = WebBrowserHelper.GetClipRect();
        Host.AXInPlaceObject!.SetObjectRects(&posRect, &clipRect);
        Host.MakeDirty();
        return HRESULT.S_OK;
    }

    HRESULT IOleWindow.Interface.GetWindow(HWND* phwnd)
        => ((IOleInPlaceSite.Interface)this).GetWindow(phwnd);

    HRESULT IOleWindow.Interface.ContextSensitiveHelp(BOOL fEnterMode)
        => ((IOleInPlaceSite.Interface)this).ContextSensitiveHelp(fEnterMode);
}

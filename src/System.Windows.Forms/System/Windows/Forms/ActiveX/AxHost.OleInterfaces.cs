// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms;

public abstract partial class AxHost
{
    /// <summary>
    ///  This private class encapsulates all of the ole interfaces so that users cannot access and call them directly.
    /// </summary>
    private sealed unsafe class OleInterfaces :
        UnknownDispatch,
        IOleControlSite.Interface,
        IOleClientSite.Interface,
        IOleInPlaceSite.Interface,
        IOleWindow.Interface,
        ISimpleFrameSite.Interface,
        IVBGetControl.Interface,
        IGetVBAObject.Interface,
        IPropertyNotifySink.Interface,
        IDisposable,
        IManagedWrapper<IDispatch, IDispatchEx, IOleControlSite, IOleClientSite, IOleWindow, IOleInPlaceSite, ISimpleFrameSite, IVBGetControl, IGetVBAObject, IPropertyNotifySink>
    {
        private readonly AxHost _host;
        private ConnectionPointCookie? _connectionPoint;

        internal OleInterfaces(AxHost host) => _host = host.OrThrowIfNull();

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                StopEvents();
            }
        }

        internal AxHost GetAxHost() => _host;

        internal void OnOcxCreate() => StartEvents();

        internal void StartEvents()
        {
            if (_connectionPoint is not null)
            {
                return;
            }

            object? nativeObject = _host.GetOcx();
            _connectionPoint = new ConnectionPointCookie(nativeObject, this, typeof(IPropertyNotifySink.Interface), throwException: false);
        }

        internal void StopEvents()
        {
            _connectionPoint?.Disconnect();
            _connectionPoint = null;
        }

        // IGetVBAObject methods:
        HRESULT IGetVBAObject.Interface.GetObject(Guid* riid, void** ppvObj, uint dwReserved)
        {
            if (ppvObj is null || riid is null)
            {
                return HRESULT.E_INVALIDARG;
            }

            if (!riid->Equals(s_ivbformat_Guid))
            {
                *ppvObj = null;
                return HRESULT.E_NOINTERFACE;
            }

            *ppvObj = ComHelpers.GetComPointer<IVBFormat>(new VBFormat());
            return HRESULT.S_OK;
        }

        // IVBGetControl methods:

        HRESULT IVBGetControl.Interface.EnumControls(
            uint dwOleContF,
            ENUM_CONTROLS_WHICH_FLAGS dwWhich,
            IEnumUnknown** ppenum)
        {
            *ppenum = ComHelpers.GetComPointer<IEnumUnknown>(
                _host.GetParentContainer().EnumControls(_host, dwOleContF, dwWhich));
            return HRESULT.S_OK;
        }

        // ISimpleFrameSite methods:
        HRESULT ISimpleFrameSite.Interface.PreMessageFilter(
            HWND hwnd,
            uint msg,
            WPARAM wp,
            LPARAM lp,
            LRESULT* plResult,
            uint* pdwCookie) => HRESULT.S_OK;

        HRESULT ISimpleFrameSite.Interface.PostMessageFilter(
            HWND hwnd,
            uint msg,
            WPARAM wp,
            LPARAM lp,
            LRESULT* plResult,
            uint dwCookie) => HRESULT.S_FALSE;

        protected override unsafe HRESULT Invoke(
            int dispId,
            uint lcid,
            DISPATCH_FLAGS flags,
            DISPPARAMS* parameters,
            VARIANT* result,
            EXCEPINFO* exceptionInfo,
            uint* argumentError)
        {
            if (result is null)
            {
                return HRESULT.E_POINTER;
            }

            object? ambient = _host.GetAmbientProperty(dispId);
            if (ambient is not null)
            {
                Marshal.GetNativeVariantForObject(ambient, (nint)result);
                return HRESULT.S_OK;
            }

            return HRESULT.DISP_E_MEMBERNOTFOUND;
        }

        // IOleControlSite methods:
        HRESULT IOleControlSite.Interface.OnControlInfoChanged() => HRESULT.S_OK;

        HRESULT IOleControlSite.Interface.LockInPlaceActive(BOOL fLock) => HRESULT.E_NOTIMPL;

        HRESULT IOleControlSite.Interface.GetExtendedControl(IDispatch** ppDisp)
        {
            if (ppDisp is null)
            {
                return HRESULT.E_POINTER;
            }

            AxContainer.ExtenderProxy? proxy = _host.GetParentContainer().GetExtenderProxyForControl(_host);
            if (proxy is null)
            {
                return HRESULT.E_NOTIMPL;
            }

            *ppDisp = ComHelpers.GetComPointer<IDispatch>(proxy);
            return HRESULT.S_OK;
        }

        HRESULT IOleControlSite.Interface.TransformCoords(POINTL* pPtlHimetric, PointF* pPtfContainer, uint dwFlags)
        {
            if (pPtlHimetric is null || pPtfContainer is null)
            {
                return HRESULT.E_INVALIDARG;
            }

            HRESULT hr = SetupLogPixels(force: false);
            if (hr.Failed)
            {
                return hr;
            }

            XFORMCOORDS coordinates = (XFORMCOORDS)dwFlags;
            if (coordinates.HasFlag(XFORMCOORDS.XFORMCOORDS_HIMETRICTOCONTAINER))
            {
                if (coordinates.HasFlag(XFORMCOORDS.XFORMCOORDS_SIZE))
                {
                    pPtfContainer->X = HM2Pix(pPtlHimetric->x, s_logPixelsX);
                    pPtfContainer->Y = HM2Pix(pPtlHimetric->y, s_logPixelsY);
                }
                else if (coordinates.HasFlag(XFORMCOORDS.XFORMCOORDS_POSITION))
                {
                    pPtfContainer->X = HM2Pix(pPtlHimetric->x, s_logPixelsX);
                    pPtfContainer->Y = HM2Pix(pPtlHimetric->y, s_logPixelsY);
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
                    pPtlHimetric->x = Pix2HM((int)pPtfContainer->X, s_logPixelsX);
                    pPtlHimetric->y = Pix2HM((int)pPtfContainer->Y, s_logPixelsY);
                }
                else if (coordinates.HasFlag(XFORMCOORDS.XFORMCOORDS_POSITION))
                {
                    pPtlHimetric->x = Pix2HM((int)pPtfContainer->X, s_logPixelsX);
                    pPtlHimetric->y = Pix2HM((int)pPtfContainer->Y, s_logPixelsY);
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

            Debug.Assert(!_host.GetAxState(s_siteProcessedInputKey), "Re-entering IOleControlSite.TranslateAccelerator!!!");
            _host.SetAxState(s_siteProcessedInputKey, true);

            Message msg = Message.Create(pMsg);
            try
            {
                bool f = _host.PreProcessMessage(ref msg);
                return f ? HRESULT.S_OK : HRESULT.S_FALSE;
            }
            finally
            {
                _host.SetAxState(s_siteProcessedInputKey, false);
            }
        }

        HRESULT IOleControlSite.Interface.OnFocus(BOOL fGotFocus) => HRESULT.S_OK;

        HRESULT IOleControlSite.Interface.ShowPropertyFrame()
        {
            if (_host.CanShowPropertyPages())
            {
                _host.ShowPropertyPages();
                return HRESULT.S_OK;
            }

            return HRESULT.E_NOTIMPL;
        }

        // IOleClientSite methods:
        HRESULT IOleClientSite.Interface.SaveObject() => HRESULT.E_NOTIMPL;

        unsafe HRESULT IOleClientSite.Interface.GetMoniker(uint dwAssign, uint dwWhichMoniker, IMoniker** ppmk)
        {
            if (ppmk is null)
            {
                return HRESULT.E_POINTER;
            }

            *ppmk = null;
            return HRESULT.E_NOTIMPL;
        }

        unsafe HRESULT IOleClientSite.Interface.GetContainer(IOleContainer** ppContainer)
        {
            if (ppContainer is null)
            {
                return HRESULT.E_POINTER;
            }

            *ppContainer = ComHelpers.GetComPointer<IOleContainer>(_host.GetParentContainer());
            return HRESULT.S_OK;
        }

        unsafe HRESULT IOleClientSite.Interface.ShowObject()
        {
            if (_host.GetAxState(s_fOwnWindow))
            {
                Debug.Fail("we can't be in showobject if we own our window...");
                return HRESULT.S_OK;
            }

            if (_host.GetAxState(s_fFakingWindow))
            {
                // we really should not be here...
                // this means that the ctl inplace deactivated and didn't call on inplace activate before calling showobject
                // so we need to destroy our fake window first...
                _host.DestroyFakeWindow();

                // The fact that we have a fake window means that the OCX inplace deactivated when we hid it. It means
                // that we have to bring it back from RUNNING to INPLACE so that it can re-create its handle properly.
                _host.TransitionDownTo(OC_LOADED);
                _host.TransitionUpTo(OC_INPLACE);
            }

            if (_host.GetOcState() < OC_INPLACE)
            {
                return HRESULT.S_OK;
            }

            HWND hwnd = HWND.Null;
            using var inPlaceObject = _host.GetComScope<IOleInPlaceObject>();
            if (inPlaceObject.Value->GetWindow(&hwnd).Succeeded)
            {
                if (_host.GetHandleNoCreate() != hwnd)
                {
                    _host.DetachWindow();
                    if (!hwnd.IsNull)
                    {
                        _host.AttachWindow(hwnd);
                    }
                }
            }
            else if (inPlaceObject.SupportsInterface<IOleInPlaceObjectWindowless>())
            {
                throw new InvalidOperationException(SR.AXWindowlessControl);
            }

            return HRESULT.S_OK;
        }

        HRESULT IOleClientSite.Interface.OnShowWindow(BOOL fShow) => HRESULT.S_OK;

        HRESULT IOleClientSite.Interface.RequestNewObjectLayout() => HRESULT.E_NOTIMPL;

        // IOleInPlaceSite methods:

        HRESULT IOleWindow.Interface.GetWindow(HWND* phwnd)
            => ((IOleInPlaceSite.Interface)this).GetWindow(phwnd);

        HRESULT IOleWindow.Interface.ContextSensitiveHelp(BOOL fEnterMode)
            => ((IOleInPlaceSite.Interface)this).ContextSensitiveHelp(fEnterMode);

        HRESULT IOleInPlaceSite.Interface.GetWindow(HWND* phwnd)
        {
            if (phwnd is null)
            {
                return HRESULT.E_POINTER;
            }

            *phwnd = _host.ParentInternal?.HWND ?? HWND.Null;
            return HRESULT.S_OK;
        }

        HRESULT IOleInPlaceSite.Interface.ContextSensitiveHelp(BOOL fEnterMode) => HRESULT.E_NOTIMPL;

        HRESULT IOleInPlaceSite.Interface.CanInPlaceActivate() => HRESULT.S_OK;

        HRESULT IOleInPlaceSite.Interface.OnInPlaceActivate()
        {
            _host.SetAxState(s_ownDisposing, false);
            _host.SetAxState(s_rejectSelection, false);
            _host.SetOcState(OC_INPLACE);
            return HRESULT.S_OK;
        }

        HRESULT IOleInPlaceSite.Interface.OnUIActivate()
        {
            _host.SetOcState(OC_UIACTIVE);
            _host.GetParentContainer().OnUIActivate(_host);
            return HRESULT.S_OK;
        }

        HRESULT IOleInPlaceSite.Interface.GetWindowContext(
            IOleInPlaceFrame** ppFrame,
            IOleInPlaceUIWindow** ppDoc,
            RECT* lprcPosRect,
            RECT* lprcClipRect,
            OLEINPLACEFRAMEINFO* lpFrameInfo)
        {
            // Following MFC CAxHostWindow::GetWindowContext handling

            if (ppFrame is not null)
            {
                *ppFrame = null;
            }

            if (ppDoc is not null)
            {
                *ppDoc = null;
            }

            if (ppDoc is null || ppFrame is null || lprcPosRect is null || lprcClipRect is null)
            {
                return HRESULT.E_POINTER;
            }

            *ppFrame = ComHelpers.GetComPointer<IOleInPlaceFrame>(_host.GetParentContainer());
            *lprcPosRect = _host.Bounds;
            *lprcClipRect = WebBrowserHelper.GetClipRect();

            if (lpFrameInfo is not null)
            {
                lpFrameInfo->cb = (uint)sizeof(OLEINPLACEFRAMEINFO);
                lpFrameInfo->fMDIApp = false;
                lpFrameInfo->haccel = HACCEL.Null;
                lpFrameInfo->cAccelEntries = 0;
                lpFrameInfo->hwndFrame = _host.ParentInternal?.HWND ?? HWND.Null;
            }

            return HRESULT.S_OK;
        }

        HRESULT IOleInPlaceSite.Interface.Scroll(SIZE scrollExtant) => HRESULT.S_FALSE;

        HRESULT IOleInPlaceSite.Interface.OnUIDeactivate(BOOL fUndoable)
        {
            _host.GetParentContainer().OnUIDeactivate(_host);
            if (_host.GetOcState() > OC_INPLACE)
            {
                _host.SetOcState(OC_INPLACE);
            }

            return HRESULT.S_OK;
        }

        HRESULT IOleInPlaceSite.Interface.OnInPlaceDeactivate()
        {
            if (_host.GetOcState() == OC_UIACTIVE)
            {
                ((IOleInPlaceSite.Interface)this).OnUIDeactivate(fUndoable: false).AssertSuccess();
            }

            _host.GetParentContainer().OnInPlaceDeactivate(_host);
            _host.DetachWindow();
            _host.SetOcState(OC_RUNNING);
            return HRESULT.S_OK;
        }

        HRESULT IOleInPlaceSite.Interface.DiscardUndoState() => HRESULT.S_OK;

        HRESULT IOleInPlaceSite.Interface.DeactivateAndUndo()
        {
            using var inPlaceObject = _host.GetComScope<IOleInPlaceObject>();
            return inPlaceObject.Value->UIDeactivate();
        }

        HRESULT IOleInPlaceSite.Interface.OnPosRectChange(RECT* lprcPosRect)
        {
            if (lprcPosRect is null)
            {
                return HRESULT.E_INVALIDARG;
            }

            // The MediaPlayer control has a AllowChangeDisplaySize property that users can set to control size changes
            // at runtime, but the control itself ignores that and sets the new size. We prevent this by not allowing
            // controls to call OnPosRectChange(), unless we instantiated the resize. Visual Basic 6 does the same.
            bool useRect = true;
            if (s_windowsMediaPlayer_Clsid.Equals(_host._clsid))
            {
                useRect = _host.GetAxState(s_handlePosRectChanged);
            }

            if (useRect)
            {
                RECT clipRect = WebBrowserHelper.GetClipRect();
                using var inPlaceObject = _host.GetComScope<IOleInPlaceObject>();
                inPlaceObject.Value->SetObjectRects(lprcPosRect, &clipRect).ThrowOnFailure();
                _host.MakeDirty();
            }

            return HRESULT.S_OK;
        }

        // IPropertyNotifySink methods

        HRESULT IPropertyNotifySink.Interface.OnChanged(int dispid)
        {
            // Some controls fire OnChanged() notifications when getting values of some properties.
            // To prevent this kind of recursion, we check to see if we are already inside a OnChanged() call.
            if (_host.NoComponentChangeEvents != 0)
            {
                return HRESULT.S_OK;
            }

            _host.NoComponentChangeEvents++;
            try
            {
                AxPropertyDescriptor? prop = null;

                if (dispid != PInvokeCore.DISPID_UNKNOWN)
                {
                    prop = _host.GetPropertyDescriptorFromDispid(dispid);
                    if (prop is not null)
                    {
                        prop.OnValueChanged(_host);
                        if (!prop.SettingValue)
                        {
                            prop.UpdateTypeConverterAndTypeEditor(true);
                        }
                    }
                }
                else
                {
                    // Update them all for DISPID_UNKNOWN.
                    PropertyDescriptorCollection props = ((ICustomTypeDescriptor)_host).GetProperties();
                    foreach (PropertyDescriptor p in props)
                    {
                        prop = p as AxPropertyDescriptor;
                        if (prop is not null && !prop.SettingValue)
                        {
                            prop.UpdateTypeConverterAndTypeEditor(true);
                        }
                    }
                }

                if (_host.Site.TryGetService(out IComponentChangeService? changeService))
                {
                    try
                    {
                        changeService.OnComponentChanging(_host, prop);
                    }
                    catch (CheckoutException e) when (e == CheckoutException.Canceled)
                    {
                        return HRESULT.S_OK;
                    }

                    // Now notify the change service that the change was successful.
                    changeService.OnComponentChanged(_host, prop, oldValue: null, prop?.GetValue(_host));
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                throw;
            }
            finally
            {
                _host.NoComponentChangeEvents--;
            }

            return HRESULT.S_OK;
        }

        HRESULT IPropertyNotifySink.Interface.OnRequestEdit(int dispid) => HRESULT.S_OK;
    }
}

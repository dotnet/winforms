// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using static Interop.Ole32;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        /// <summary>
        ///  This private class encapsulates all of the ole interfaces so that users cannot access
        ///  and call them directly.
        /// </summary>
        private class OleInterfaces :
            IOleControlSite,
            IOleClientSite,
            IOleInPlaceSite,
            ISimpleFrameSite,
            IVBGetControl,
            IGetVBAObject,
            IPropertyNotifySink,
            IReflect,
            IDisposable
        {
            private readonly AxHost _host;
            private ConnectionPointCookie? _connectionPoint;

            internal OleInterfaces(AxHost host)
            {
                _host = host.OrThrowIfNull();
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (!AppDomain.CurrentDomain.IsFinalizingForUnload())
                    {
                        SynchronizationContext? context = SynchronizationContext.Current;
                        if (context is null)
                        {
                            Debug.Fail("Attempted to disconnect ConnectionPointCookie from the finalizer with no SynchronizationContext.");
                        }
                        else
                        {
                            context.Post(new SendOrPostCallback(AttemptStopEvents), null);
                        }
                    }
                }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            internal AxHost GetAxHost()
            {
                return _host;
            }

            internal void OnOcxCreate()
            {
                StartEvents();
            }

            internal void StartEvents()
            {
                if (_connectionPoint is not null)
                {
                    return;
                }

                object nativeObject = _host.GetOcx();

                try
                {
                    _connectionPoint = new ConnectionPointCookie(nativeObject, this, typeof(IPropertyNotifySink));
                }
                catch
                {
                }
            }

            void AttemptStopEvents(object? trash)
            {
                if (_connectionPoint is null)
                {
                    return;
                }

                if (_connectionPoint._threadId == Environment.CurrentManagedThreadId)
                {
                    StopEvents();
                }
                else
                {
                    Debug.Fail("Attempted to disconnect ConnectionPointCookie from the wrong thread (finalizer).");
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

            // IGetVBAObject methods:
            unsafe HRESULT IGetVBAObject.GetObject(Guid* riid, IVBFormat?[] rval, uint dwReserved)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in GetObject");

                if (rval is null || riid is null)
                {
                    return HRESULT.Values.E_INVALIDARG;
                }

                if (!riid->Equals(s_ivbformat_Guid))
                {
                    rval[0] = null;
                    return HRESULT.Values.E_NOINTERFACE;
                }

                rval[0] = new VBFormat();
                return HRESULT.Values.S_OK;
            }

            // IVBGetControl methods:

            unsafe HRESULT IVBGetControl.EnumControls(OLECONTF dwOleContF, GC_WCH dwWhich, out IEnumUnknown ppenum)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in EnumControls");
                ppenum = _host.GetParentContainer().EnumControls(_host, dwOleContF, dwWhich);
                return HRESULT.Values.S_OK;
            }

            // ISimpleFrameSite methods:
            unsafe HRESULT ISimpleFrameSite.PreMessageFilter(IntPtr hwnd, uint msg, IntPtr wp, IntPtr lp, IntPtr* plResult, uint* pdwCookie)
            {
                return HRESULT.Values.S_OK;
            }

            unsafe HRESULT ISimpleFrameSite.PostMessageFilter(IntPtr hwnd, uint msg, IntPtr wp, IntPtr lp, IntPtr* plResult, uint dwCookie)
            {
                return HRESULT.Values.S_FALSE;
            }

            // IReflect methods:

            MethodInfo? IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder? binder, Type[] types, ParameterModifier[]? modifiers)
            {
                return null;
            }

            MethodInfo? IReflect.GetMethod(string name, BindingFlags bindingAttr)
            {
                return null;
            }

            MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr)
            {
                return Array.Empty<MethodInfo>();
            }

            FieldInfo? IReflect.GetField(string name, BindingFlags bindingAttr)
            {
                return null;
            }

            FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr)
            {
                return Array.Empty<FieldInfo>();
            }

            PropertyInfo? IReflect.GetProperty(string name, BindingFlags bindingAttr)
            {
                return null;
            }

            PropertyInfo? IReflect.GetProperty(
                string name,
                BindingFlags bindingAttr,
                Binder? binder,
                Type? returnType,
                Type[] types,
                ParameterModifier[]? modifiers)
            {
                return null;
            }

            PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
            {
                return Array.Empty<PropertyInfo>();
            }

            MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr)
            {
                return Array.Empty<MemberInfo>();
            }

            MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
            {
                return Array.Empty<MemberInfo>();
            }

            object? IReflect.InvokeMember(
                string name,
                BindingFlags invokeAttr,
                Binder? binder,
                object? target,
                object?[]? args,
                ParameterModifier[]? modifiers,
                CultureInfo? culture,
                string[]? namedParameters)
            {
                if (name.StartsWith("[DISPID="))
                {
                    int endIndex = name.IndexOf(']');
                    DispatchID dispid = (DispatchID)int.Parse(name.AsSpan(8, endIndex - 8), CultureInfo.InvariantCulture);
                    object ambient = _host.GetAmbientProperty(dispid);
                    if (ambient is not null)
                    {
                        return ambient;
                    }
                }

                throw s_unknownErrorException;
            }

            Type IReflect.UnderlyingSystemType
            {
                get
                {
                    return null!;
                }
            }

            // IOleControlSite methods:
            HRESULT IOleControlSite.OnControlInfoChanged()
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in OnControlInfoChanged");
                return HRESULT.Values.S_OK;
            }

            HRESULT IOleControlSite.LockInPlaceActive(BOOL fLock)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in LockInPlaceActive");
                return HRESULT.Values.E_NOTIMPL;
            }

            unsafe HRESULT IOleControlSite.GetExtendedControl(IntPtr* ppDisp)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in GetExtendedControl {_host}");
                if (ppDisp is null)
                {
                    return HRESULT.Values.E_POINTER;
                }

                object proxy = _host.GetParentContainer().GetProxyForControl(_host);
                if (proxy is null)
                {
                    return HRESULT.Values.E_NOTIMPL;
                }

                *ppDisp = Marshal.GetIDispatchForObject(proxy);
                return HRESULT.Values.S_OK;
            }

            unsafe HRESULT IOleControlSite.TransformCoords(Point* pPtlHimetric, PointF* pPtfContainer, XFORMCOORDS dwFlags)
            {
                if (pPtlHimetric is null || pPtfContainer is null)
                {
                    return HRESULT.Values.E_INVALIDARG;
                }

                HRESULT hr = SetupLogPixels(false);
                if (hr < 0)
                {
                    return hr;
                }

                if ((dwFlags & XFORMCOORDS.HIMETRICTOCONTAINER) != 0)
                {
                    if ((dwFlags & XFORMCOORDS.SIZE) != 0)
                    {
                        pPtfContainer->X = HM2Pix(pPtlHimetric->X, s_logPixelsX);
                        pPtfContainer->Y = HM2Pix(pPtlHimetric->Y, s_logPixelsY);
                    }
                    else if ((dwFlags & XFORMCOORDS.POSITION) != 0)
                    {
                        pPtfContainer->X = HM2Pix(pPtlHimetric->X, s_logPixelsX);
                        pPtfContainer->Y = HM2Pix(pPtlHimetric->Y, s_logPixelsY);
                    }
                    else
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"\t dwFlags not supported: {dwFlags}");
                        return HRESULT.Values.E_INVALIDARG;
                    }
                }
                else if ((dwFlags & XFORMCOORDS.CONTAINERTOHIMETRIC) != 0)
                {
                    if ((dwFlags & XFORMCOORDS.SIZE) != 0)
                    {
                        pPtlHimetric->X = Pix2HM((int)pPtfContainer->X, s_logPixelsX);
                        pPtlHimetric->Y = Pix2HM((int)pPtfContainer->Y, s_logPixelsY);
                    }
                    else if ((dwFlags & XFORMCOORDS.POSITION) != 0)
                    {
                        pPtlHimetric->X = Pix2HM((int)pPtfContainer->X, s_logPixelsX);
                        pPtlHimetric->Y = Pix2HM((int)pPtfContainer->Y, s_logPixelsY);
                    }
                    else
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"\t dwFlags not supported: {dwFlags}");
                        return HRESULT.Values.E_INVALIDARG;
                    }
                }
                else
                {
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"\t dwFlags not supported: {dwFlags}");
                    return HRESULT.Values.E_INVALIDARG;
                }

                return HRESULT.Values.S_OK;
            }

            unsafe HRESULT IOleControlSite.TranslateAccelerator(MSG* pMsg, KEYMODIFIERS grfModifiers)
            {
                if (pMsg is null)
                {
                    return HRESULT.Values.E_POINTER;
                }

                Debug.Assert(!_host.GetAxState(s_siteProcessedInputKey), "Re-entering IOleControlSite.TranslateAccelerator!!!");
                _host.SetAxState(s_siteProcessedInputKey, true);

                Message msg = Message.Create(pMsg);
                try
                {
                    bool f = _host.PreProcessMessage(ref msg);
                    return f ? HRESULT.Values.S_OK : HRESULT.Values.S_FALSE;
                }
                finally
                {
                    _host.SetAxState(s_siteProcessedInputKey, false);
                }
            }

            HRESULT IOleControlSite.OnFocus(BOOL fGotFocus) => HRESULT.Values.S_OK;

            HRESULT IOleControlSite.ShowPropertyFrame()
            {
                if (_host.CanShowPropertyPages())
                {
                    _host.ShowPropertyPages();
                    return HRESULT.Values.S_OK;
                }

                return HRESULT.Values.E_NOTIMPL;
            }

            // IOleClientSite methods:
            HRESULT IOleClientSite.SaveObject()
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in SaveObject");
                return HRESULT.Values.E_NOTIMPL;
            }

            unsafe HRESULT IOleClientSite.GetMoniker(OLEGETMONIKER dwAssign, OLEWHICHMK dwWhichMoniker, IntPtr* ppmk)
            {
                if (ppmk is null)
                {
                    return HRESULT.Values.E_POINTER;
                }

                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetMoniker");
                *ppmk = IntPtr.Zero;
                return HRESULT.Values.E_NOTIMPL;
            }

            IOleContainer IOleClientSite.GetContainer()
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in getContainer");
                return _host.GetParentContainer();
            }

            unsafe HRESULT IOleClientSite.ShowObject()
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in ShowObject");
                if (_host.GetAxState(s_fOwnWindow))
                {
                    Debug.Fail("we can't be in showobject if we own our window...");
                    return HRESULT.Values.S_OK;
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
                    return HRESULT.Values.S_OK;
                }

                HWND hwnd = HWND.Null;
                if (_host.GetInPlaceObject().GetWindow(&hwnd).Succeeded)
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
                else if (_host.GetInPlaceObject() is IOleInPlaceObjectWindowless)
                {
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "Windowless control.");
                    throw new InvalidOperationException(SR.AXWindowlessControl);
                }

                return HRESULT.Values.S_OK;
            }

            HRESULT IOleClientSite.OnShowWindow(BOOL fShow)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in OnShowWindow");
                return HRESULT.Values.S_OK;
            }

            HRESULT IOleClientSite.RequestNewObjectLayout()
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in RequestNewObjectLayout");
                return HRESULT.Values.E_NOTIMPL;
            }

            // IOleInPlaceSite methods:

            unsafe HRESULT IOleInPlaceSite.GetWindow(IntPtr* phwnd)
            {
                if (phwnd is null)
                {
                    return HRESULT.Values.E_POINTER;
                }

                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in GetWindow");
                *phwnd = _host.ParentInternal?.Handle ?? IntPtr.Zero;
                return HRESULT.Values.S_OK;
            }

            HRESULT IOleInPlaceSite.ContextSensitiveHelp(BOOL fEnterMode)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in ContextSensitiveHelp");
                return HRESULT.Values.E_NOTIMPL;
            }

            HRESULT IOleInPlaceSite.CanInPlaceActivate()
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in CanInPlaceActivate");
                return HRESULT.Values.S_OK;
            }

            HRESULT IOleInPlaceSite.OnInPlaceActivate()
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in OnInPlaceActivate");
                _host.SetAxState(s_ownDisposing, false);
                _host.SetAxState(s_rejectSelection, false);
                _host.SetOcState(OC_INPLACE);
                return HRESULT.Values.S_OK;
            }

            HRESULT IOleInPlaceSite.OnUIActivate()
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in OnUIActivate for {_host}");
                _host.SetOcState(OC_UIACTIVE);
                _host.GetParentContainer().OnUIActivate(_host);
                return HRESULT.Values.S_OK;
            }

            unsafe HRESULT IOleInPlaceSite.GetWindowContext(
                out IOleInPlaceFrame ppFrame,
                out IOleInPlaceUIWindow? ppDoc,
                RECT* lprcPosRect,
                RECT* lprcClipRect,
                OLEINPLACEFRAMEINFO* lpFrameInfo)
            {
                ppDoc = null;
                ppFrame = _host.GetParentContainer();

                if (lprcPosRect is null || lprcClipRect is null)
                {
                    return HRESULT.Values.E_POINTER;
                }

                *lprcPosRect = _host.Bounds;
                *lprcClipRect = WebBrowserHelper.GetClipRect();
                if (lpFrameInfo is not null)
                {
                    lpFrameInfo->cb = (uint)Marshal.SizeOf<OLEINPLACEFRAMEINFO>();
                    lpFrameInfo->fMDIApp = false;
                    lpFrameInfo->hAccel = IntPtr.Zero;
                    lpFrameInfo->cAccelEntries = 0;
                    lpFrameInfo->hwndFrame = _host.ParentInternal?.Handle ?? IntPtr.Zero;
                }

                return HRESULT.Values.S_OK;
            }

            HRESULT IOleInPlaceSite.Scroll(Size scrollExtant) => HRESULT.Values.S_FALSE;

            HRESULT IOleInPlaceSite.OnUIDeactivate(BOOL fUndoable)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in OnUIDeactivate for {_host}");
                _host.GetParentContainer().OnUIDeactivate(_host);
                if (_host.GetOcState() > OC_INPLACE)
                {
                    _host.SetOcState(OC_INPLACE);
                }

                return HRESULT.Values.S_OK;
            }

            HRESULT IOleInPlaceSite.OnInPlaceDeactivate()
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in OnInPlaceDeactivate");
                if (_host.GetOcState() == OC_UIACTIVE)
                {
                    ((IOleInPlaceSite)this).OnUIDeactivate(false);
                }

                _host.GetParentContainer().OnInPlaceDeactivate(_host);
                _host.DetachWindow();
                _host.SetOcState(OC_RUNNING);
                return HRESULT.Values.S_OK;
            }

            HRESULT IOleInPlaceSite.DiscardUndoState()
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in DiscardUndoState");
                return HRESULT.Values.S_OK;
            }

            HRESULT IOleInPlaceSite.DeactivateAndUndo()
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in DeactivateAndUndo for {_host}");
                return _host.GetInPlaceObject().UIDeactivate();
            }

            unsafe HRESULT IOleInPlaceSite.OnPosRectChange(RECT* lprcPosRect)
            {
                if (lprcPosRect is null)
                {
                    return HRESULT.Values.E_INVALIDARG;
                }

                // The MediaPlayer control has a AllowChangeDisplaySize property that users
                // can set to control size changes at runtime, but the control itself ignores that and sets the new size.
                // We prevent this by not allowing controls to call OnPosRectChange(), unless we instantiated the resize.
                // visual basic6 does the same.
                bool useRect = true;
                if (s_windowsMediaPlayer_Clsid.Equals(_host._clsid))
                {
                    useRect = _host.GetAxState(s_handlePosRectChanged);
                }

                if (useRect)
                {
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in OnPosRectChange{lprcPosRect->ToString()}");
                    RECT clipRect = WebBrowserHelper.GetClipRect();
                    _host.GetInPlaceObject().SetObjectRects(lprcPosRect, &clipRect);
                    _host.MakeDirty();
                }
                else
                {
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "Control directly called OnPosRectChange... ignoring the new size");
                }

                return HRESULT.Values.S_OK;
            }

            // IPropertyNotifySink methods

            HRESULT IPropertyNotifySink.OnChanged(DispatchID dispid)
            {
                // Some controls fire OnChanged() notifications when getting values of some properties.
                // To prevent this kind of recursion, we check to see if we are already inside a OnChanged() call.
                if (_host.NoComponentChangeEvents != 0)
                {
                    return HRESULT.Values.S_OK;
                }

                _host.NoComponentChangeEvents++;
                try
                {
                    AxPropertyDescriptor? prop = null;

                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in OnChanged");

                    if (dispid != DispatchID.UNKNOWN)
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
                        // update them all for DISPID_UNKNOWN.
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
                            return HRESULT.Values.S_OK;
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

                return HRESULT.Values.S_OK;
            }

            HRESULT IPropertyNotifySink.OnRequestEdit(DispatchID dispid)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in OnRequestEdit for {_host}");
                return HRESULT.Values.S_OK;
            }
        }
    }
}

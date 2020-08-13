// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using static Interop;
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
            private readonly AxHost host;
            private ConnectionPointCookie connectionPoint;

            internal OleInterfaces(AxHost host)
            {
                this.host = host ?? throw new ArgumentNullException(nameof(host));
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (!AppDomain.CurrentDomain.IsFinalizingForUnload())
                    {
                        SynchronizationContext context = SynchronizationContext.Current;
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
                return host;
            }

            internal void OnOcxCreate()
            {
                StartEvents();
            }

            internal void StartEvents()
            {
                if (connectionPoint != null)
                {
                    return;
                }

                object nativeObject = host.GetOcx();

                try
                {
                    connectionPoint = new ConnectionPointCookie(nativeObject, this, typeof(IPropertyNotifySink));
                }
                catch
                {
                }
            }

            void AttemptStopEvents(object trash)
            {
                if (connectionPoint is null)
                {
                    return;
                }

                if (connectionPoint.threadId == Thread.CurrentThread.ManagedThreadId)
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
                if (connectionPoint != null)
                {
                    connectionPoint.Disconnect();
                    connectionPoint = null;
                }
            }

            // IGetVBAObject methods:
            unsafe HRESULT IGetVBAObject.GetObject(Guid* riid, IVBFormat[] rval, uint dwReserved)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in GetObject");

                if (rval is null || riid is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                if (!riid->Equals(ivbformat_Guid))
                {
                    rval[0] = null;
                    return HRESULT.E_NOINTERFACE;
                }

                rval[0] = new VBFormat();
                return HRESULT.S_OK;
            }

            // IVBGetControl methods:

            unsafe HRESULT IVBGetControl.EnumControls(OLECONTF dwOleContF, GC_WCH dwWhich, out IEnumUnknown ppenum)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in EnumControls");
                ppenum = host.GetParentContainer().EnumControls(host, dwOleContF, dwWhich);
                return HRESULT.S_OK;
            }

            // ISimpleFrameSite methods:
            unsafe HRESULT ISimpleFrameSite.PreMessageFilter(IntPtr hwnd, uint msg, IntPtr wp, IntPtr lp, IntPtr* plResult, uint* pdwCookie)
            {
                return HRESULT.S_OK;
            }

            unsafe HRESULT ISimpleFrameSite.PostMessageFilter(IntPtr hwnd, uint msg, IntPtr wp, IntPtr lp, IntPtr* plResult, uint dwCookie)
            {
                return HRESULT.S_FALSE;
            }

            // IReflect methods:

            MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
            {
                return null;
            }

            MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr)
            {
                return null;
            }

            MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr)
            {
                return Array.Empty<MethodInfo>();
            }

            FieldInfo IReflect.GetField(string name, BindingFlags bindingAttr)
            {
                return null;
            }

            FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr)
            {
                return Array.Empty<FieldInfo>();
            }

            PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr)
            {
                return null;
            }

            PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
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

            object IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
            {
                if (name.StartsWith("[DISPID="))
                {
                    int endIndex = name.IndexOf(']');
                    DispatchID dispid = (DispatchID)int.Parse(name.Substring(8, endIndex - 8), CultureInfo.InvariantCulture);
                    object ambient = host.GetAmbientProperty(dispid);
                    if (ambient != null)
                    {
                        return ambient;
                    }
                }

                throw E_FAIL;
            }

            Type IReflect.UnderlyingSystemType
            {
                get
                {
                    return null;
                }
            }

            // IOleControlSite methods:
            HRESULT IOleControlSite.OnControlInfoChanged()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnControlInfoChanged");
                return HRESULT.S_OK;
            }

            HRESULT IOleControlSite.LockInPlaceActive(BOOL fLock)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in LockInPlaceActive");
                return HRESULT.E_NOTIMPL;
            }

            unsafe HRESULT IOleControlSite.GetExtendedControl(IntPtr* ppDisp)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in GetExtendedControl " + host.ToString());
                if (ppDisp is null)
                {
                    return HRESULT.E_POINTER;
                }

                object proxy = host.GetParentContainer().GetProxyForControl(host);
                if (proxy is null)
                {
                    return HRESULT.E_NOTIMPL;
                }

                *ppDisp = Marshal.GetIDispatchForObject(proxy);
                return HRESULT.S_OK;
            }

            unsafe HRESULT IOleControlSite.TransformCoords(Point *pPtlHimetric, PointF *pPtfContainer, XFORMCOORDS dwFlags)
            {
                if (pPtlHimetric is null || pPtfContainer is null)
                {
                    return HRESULT.E_INVALIDARG;
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
                        pPtfContainer->X = (float)host.HM2Pix(pPtlHimetric->X, logPixelsX);
                        pPtfContainer->Y = (float)host.HM2Pix(pPtlHimetric->Y, logPixelsY);
                    }
                    else if ((dwFlags & XFORMCOORDS.POSITION) != 0)
                    {
                        pPtfContainer->X = (float)host.HM2Pix(pPtlHimetric->X, logPixelsX);
                        pPtfContainer->Y = (float)host.HM2Pix(pPtlHimetric->Y, logPixelsY);
                    }
                    else
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "\t dwFlags not supported: " + dwFlags);
                        return HRESULT.E_INVALIDARG;
                    }
                }
                else if ((dwFlags & XFORMCOORDS.CONTAINERTOHIMETRIC) != 0)
                {
                    if ((dwFlags & XFORMCOORDS.SIZE) != 0)
                    {
                        pPtlHimetric->X = host.Pix2HM((int)pPtfContainer->X, logPixelsX);
                        pPtlHimetric->Y = host.Pix2HM((int)pPtfContainer->Y, logPixelsY);
                    }
                    else if ((dwFlags & XFORMCOORDS.POSITION) != 0)
                    {
                        pPtlHimetric->X = host.Pix2HM((int)pPtfContainer->X, logPixelsX);
                        pPtlHimetric->Y = host.Pix2HM((int)pPtfContainer->Y, logPixelsY);
                    }
                    else
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "\t dwFlags not supported: " + dwFlags);
                        return HRESULT.E_INVALIDARG;
                    }
                }
                else
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "\t dwFlags not supported: " + dwFlags);
                    return HRESULT.E_INVALIDARG;
                }

                return HRESULT.S_OK;
            }

            unsafe HRESULT IOleControlSite.TranslateAccelerator(User32.MSG* pMsg, KEYMODIFIERS grfModifiers)
            {
                if (pMsg is null)
                {
                    return HRESULT.E_POINTER;
                }

                Debug.Assert(!host.GetAxState(AxHost.siteProcessedInputKey), "Re-entering IOleControlSite.TranslateAccelerator!!!");
                host.SetAxState(AxHost.siteProcessedInputKey, true);

                Message msg = *pMsg;
                try
                {
                    bool f = ((Control)host).PreProcessMessage(ref msg);
                    return f ? HRESULT.S_OK : HRESULT.S_FALSE;
                }
                finally
                {
                    host.SetAxState(AxHost.siteProcessedInputKey, false);
                }
            }

            HRESULT IOleControlSite.OnFocus(BOOL fGotFocus) => HRESULT.S_OK;

            HRESULT IOleControlSite.ShowPropertyFrame()
            {
                if (host.CanShowPropertyPages())
                {
                    host.ShowPropertyPages();
                    return HRESULT.S_OK;
                }

                return HRESULT.E_NOTIMPL;
            }

            // IOleClientSite methods:
            HRESULT IOleClientSite.SaveObject()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in SaveObject");
                return HRESULT.E_NOTIMPL;
            }

            unsafe HRESULT IOleClientSite.GetMoniker(OLEGETMONIKER dwAssign, OLEWHICHMK dwWhichMoniker, IntPtr* ppmk)
            {
                if (ppmk is null)
                {
                    return HRESULT.E_POINTER;
                }

                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetMoniker");
                *ppmk = IntPtr.Zero;
                return HRESULT.E_NOTIMPL;
            }

            IOleContainer IOleClientSite.GetContainer()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getContainer");
                return host.GetParentContainer();
            }

            unsafe HRESULT IOleClientSite.ShowObject()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in ShowObject");
                if (host.GetAxState(AxHost.fOwnWindow))
                {
                    Debug.Fail("we can't be in showobject if we own our window...");
                    return HRESULT.S_OK;
                }
                if (host.GetAxState(AxHost.fFakingWindow))
                {
                    // we really should not be here...
                    // this means that the ctl inplace deactivated and didn't call on inplace activate before calling showobject
                    // so we need to destroy our fake window first...
                    host.DestroyFakeWindow();

                    // The fact that we have a fake window means that the OCX inplace deactivated when we hid it. It means
                    // that we have to bring it back from RUNNING to INPLACE so that it can re-create its handle properly.
                    //
                    host.TransitionDownTo(OC_LOADED);
                    host.TransitionUpTo(OC_INPLACE);
                }
                if (host.GetOcState() < OC_INPLACE)
                {
                    return HRESULT.S_OK;
                }

                IntPtr hwnd = IntPtr.Zero;
                if (host.GetInPlaceObject().GetWindow(&hwnd).Succeeded())
                {
                    if (host.GetHandleNoCreate() != hwnd)
                    {
                        host.DetachWindow();
                        if (hwnd != IntPtr.Zero)
                        {
                            host.AttachWindow(hwnd);
                        }
                    }
                }
                else if (host.GetInPlaceObject() is IOleInPlaceObjectWindowless)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Windowless control.");
                    throw new InvalidOperationException(SR.AXWindowlessControl);
                }

                return HRESULT.S_OK;
            }

            HRESULT IOleClientSite.OnShowWindow(BOOL fShow)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnShowWindow");
                return HRESULT.S_OK;
            }

            HRESULT IOleClientSite.RequestNewObjectLayout()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in RequestNewObjectLayout");
                return HRESULT.E_NOTIMPL;
            }

            // IOleInPlaceSite methods:

            unsafe HRESULT IOleInPlaceSite.GetWindow(IntPtr* phwnd)
            {
                if (phwnd is null)
                {
                    return HRESULT.E_POINTER;
                }

                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in GetWindow");
                Control parent = host.ParentInternal;
                *phwnd = parent != null ? parent.Handle : IntPtr.Zero;
                return HRESULT.S_OK;
            }

            HRESULT IOleInPlaceSite.ContextSensitiveHelp(BOOL fEnterMode)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in ContextSensitiveHelp");
                return HRESULT.E_NOTIMPL;
            }

            HRESULT IOleInPlaceSite.CanInPlaceActivate()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in CanInPlaceActivate");
                return HRESULT.S_OK;
            }

            HRESULT IOleInPlaceSite.OnInPlaceActivate()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnInPlaceActivate");
                host.SetAxState(AxHost.ownDisposing, false);
                host.SetAxState(AxHost.rejectSelection, false);
                host.SetOcState(OC_INPLACE);
                return HRESULT.S_OK;
            }

            HRESULT IOleInPlaceSite.OnUIActivate()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnUIActivate for " + host.ToString());
                host.SetOcState(OC_UIACTIVE);
                host.GetParentContainer().OnUIActivate(host);
                return HRESULT.S_OK;
            }

            unsafe HRESULT IOleInPlaceSite.GetWindowContext(
                out IOleInPlaceFrame ppFrame,
                out IOleInPlaceUIWindow ppDoc,
                RECT* lprcPosRect,
                RECT* lprcClipRect,
                OLEINPLACEFRAMEINFO* lpFrameInfo)
            {
                ppDoc = null;
                ppFrame = host.GetParentContainer();

                if (lprcPosRect is null || lprcClipRect is null)
                {
                    return HRESULT.E_POINTER;
                }

                *lprcPosRect = host.Bounds;
                *lprcClipRect = WebBrowserHelper.GetClipRect();
                if (lpFrameInfo != null)
                {
                    lpFrameInfo->cb = (uint)Marshal.SizeOf<OLEINPLACEFRAMEINFO>();
                    lpFrameInfo->fMDIApp = BOOL.FALSE;
                    lpFrameInfo->hAccel = IntPtr.Zero;
                    lpFrameInfo->cAccelEntries = 0;
                    lpFrameInfo->hwndFrame = host.ParentInternal.Handle;
                }

                return HRESULT.S_OK;
            }

            HRESULT IOleInPlaceSite.Scroll(Size scrollExtant) => HRESULT.S_FALSE;

            HRESULT IOleInPlaceSite.OnUIDeactivate(BOOL fUndoable)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnUIDeactivate for " + host.ToString());
                host.GetParentContainer().OnUIDeactivate(host);
                if (host.GetOcState() > OC_INPLACE)
                {
                    host.SetOcState(OC_INPLACE);
                }

                return HRESULT.S_OK;
            }

            HRESULT IOleInPlaceSite.OnInPlaceDeactivate()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnInPlaceDeactivate");
                if (host.GetOcState() == OC_UIACTIVE)
                {
                    ((IOleInPlaceSite)this).OnUIDeactivate(0);
                }

                host.GetParentContainer().OnInPlaceDeactivate(host);
                host.DetachWindow();
                host.SetOcState(OC_RUNNING);
                return HRESULT.S_OK;
            }

            HRESULT IOleInPlaceSite.DiscardUndoState()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in DiscardUndoState");
                return HRESULT.S_OK;
            }

            HRESULT IOleInPlaceSite.DeactivateAndUndo()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in DeactivateAndUndo for " + host.ToString());
                return host.GetInPlaceObject().UIDeactivate();
            }

            unsafe HRESULT IOleInPlaceSite.OnPosRectChange(RECT* lprcPosRect)
            {
                if (lprcPosRect is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                // The MediaPlayer control has a AllowChangeDisplaySize property that users
                // can set to control size changes at runtime, but the control itself ignores that and sets the new size.
                // We prevent this by not allowing controls to call OnPosRectChange(), unless we instantiated the resize.
                // visual basic6 does the same.
                //
                bool useRect = true;
                if (AxHost.windowsMediaPlayer_Clsid.Equals(host.clsid))
                {
                    useRect = host.GetAxState(AxHost.handlePosRectChanged);
                }

                if (useRect)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnPosRectChange" + lprcPosRect->ToString());
                    RECT clipRect = WebBrowserHelper.GetClipRect();
                    host.GetInPlaceObject().SetObjectRects(lprcPosRect, &clipRect);
                    host.MakeDirty();
                }
                else
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Control directly called OnPosRectChange... ignoring the new size");
                }

                return HRESULT.S_OK;
            }

            // IPropertyNotifySink methods

            HRESULT IPropertyNotifySink.OnChanged(DispatchID dispid)
            {
                // Some controls fire OnChanged() notifications when getting values of some properties.
                // To prevent this kind of recursion, we check to see if we are already inside a OnChanged() call.
                if (host.NoComponentChangeEvents != 0)
                {
                    return HRESULT.S_OK;
                }

                host.NoComponentChangeEvents++;
                try
                {
                    AxPropertyDescriptor prop = null;

                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnChanged");

                    if (dispid != DispatchID.UNKNOWN)
                    {
                        prop = host.GetPropertyDescriptorFromDispid(dispid);
                        if (prop != null)
                        {
                            prop.OnValueChanged(host);
                            if (!prop.SettingValue)
                            {
                                prop.UpdateTypeConverterAndTypeEditor(true);
                            }
                        }
                    }
                    else
                    {
                        // update them all for DISPID_UNKNOWN.
                        PropertyDescriptorCollection props = ((ICustomTypeDescriptor)host).GetProperties();
                        foreach (PropertyDescriptor p in props)
                        {
                            prop = p as AxPropertyDescriptor;
                            if (prop != null && !prop.SettingValue)
                            {
                                prop.UpdateTypeConverterAndTypeEditor(true);
                            }
                        }
                    }

                    ISite site = host.Site;
                    if (site != null)
                    {
                        IComponentChangeService changeService = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));

                        if (changeService != null)
                        {
                            try
                            {
                                changeService.OnComponentChanging(host, prop);
                            }
                            catch (CheckoutException coEx)
                            {
                                if (coEx == CheckoutException.Canceled)
                                {
                                    return HRESULT.S_OK;
                                }
                                throw;
                            }

                            // Now notify the change service that the change was successful.
                            //
                            changeService.OnComponentChanged(host, prop, null, (prop?.GetValue(host)));
                        }
                    }
                }
                catch (Exception t)
                {
                    Debug.Fail(t.ToString());
                    throw;
                }
                finally
                {
                    host.NoComponentChangeEvents--;
                }

                return HRESULT.S_OK;
            }

            HRESULT IPropertyNotifySink.OnRequestEdit(DispatchID dispid)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnRequestEdit for " + host.ToString());
                return HRESULT.S_OK;
            }
        }
    }
}

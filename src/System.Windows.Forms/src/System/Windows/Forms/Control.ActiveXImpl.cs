// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms
{
    public partial class Control
    {
        /// <summary>
        ///  This class holds all of the state data for an ActiveX control and
        ///  supplies the implementation for many of the non-trivial methods.
        /// </summary>
        private class ActiveXImpl : MarshalByRefObject, IWindowTarget
        {
            private const int HiMetricPerInch = 2540;
            private static readonly int s_viewAdviseOnlyOnce = BitVector32.CreateMask();
            private static readonly int s_viewAdvisePrimeFirst = BitVector32.CreateMask(s_viewAdviseOnlyOnce);
            private static readonly int s_eventsFrozen = BitVector32.CreateMask(s_viewAdvisePrimeFirst);
            private static readonly int s_changingExtents = BitVector32.CreateMask(s_eventsFrozen);
            private static readonly int s_saving = BitVector32.CreateMask(s_changingExtents);
            private static readonly int s_isDirty = BitVector32.CreateMask(s_saving);
            private static readonly int s_inPlaceActive = BitVector32.CreateMask(s_isDirty);
            private static readonly int s_inPlaceVisible = BitVector32.CreateMask(s_inPlaceActive);
            private static readonly int s_uiActive = BitVector32.CreateMask(s_inPlaceVisible);
            private static readonly int s_uiDead = BitVector32.CreateMask(s_uiActive);
            private static readonly int s_adjustingRect = BitVector32.CreateMask(s_uiDead);

            private static Point s_logPixels = Point.Empty;
            private static NativeMethods.tagOLEVERB[] s_axVerbs;

            private static int s_globalActiveXCount = 0;
            private static bool s_checkedIE;
            private static bool s_isIE;

            private readonly Control _control;
            private readonly IWindowTarget _controlWindowTarget;
            private IntPtr _clipRegion;
            private UnsafeNativeMethods.IOleClientSite _clientSite;
            private UnsafeNativeMethods.IOleInPlaceUIWindow _inPlaceUiWindow;
            private UnsafeNativeMethods.IOleInPlaceFrame _inPlaceFrame;
            private readonly ArrayList _adviseList;
            private IAdviseSink _viewAdviseSink;
            private BitVector32 _activeXState;
            private readonly AmbientProperty[] _ambientProperties;
            private IntPtr _accelTable;
            private short _accelCount = -1;
            private NativeMethods.COMRECT _adjustRect; // temporary rect used during OnPosRectChange && SetObjectRects

            /// <summary>
            ///  Creates a new ActiveXImpl.
            /// </summary>
            internal ActiveXImpl(Control control)
            {
                _control = control;

                // We replace the control's window target with our own.  We
                // do this so we can handle the UI Dead ambient property.
                _controlWindowTarget = control.WindowTarget;
                control.WindowTarget = this;

                _adviseList = new ArrayList();
                _activeXState = new BitVector32();
                _ambientProperties = new AmbientProperty[] {
                    new AmbientProperty("Font", NativeMethods.ActiveX.DISPID_AMBIENT_FONT),
                    new AmbientProperty("BackColor", NativeMethods.ActiveX.DISPID_AMBIENT_BACKCOLOR),
                    new AmbientProperty("ForeColor", NativeMethods.ActiveX.DISPID_AMBIENT_FORECOLOR)
                };
            }

            /// <summary>
            ///  Retrieves the ambient back color for the control.
            /// </summary>
            [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            internal Color AmbientBackColor
            {
                get
                {
                    AmbientProperty prop = LookupAmbient(NativeMethods.ActiveX.DISPID_AMBIENT_BACKCOLOR);

                    if (prop.Empty)
                    {
                        object obj = null;
                        if (GetAmbientProperty(NativeMethods.ActiveX.DISPID_AMBIENT_BACKCOLOR, ref obj))
                        {
                            if (obj != null)
                            {
                                try
                                {
                                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Object color type=" + obj.GetType().FullName);
                                    prop.Value = ColorTranslator.FromOle(Convert.ToInt32(obj, CultureInfo.InvariantCulture));
                                }
                                catch (Exception e)
                                {
                                    Debug.Fail("Failed to massage ambient back color to a Color", e.ToString());

                                    if (ClientUtils.IsSecurityOrCriticalException(e))
                                    {
                                        throw;
                                    }
                                }
                            }
                        }
                    }

                    if (prop.Value == null)
                    {
                        return Color.Empty;
                    }
                    else
                    {
                        return (Color)prop.Value;
                    }
                }
            }

            /// <summary>
            ///  Retrieves the ambient font for the control.
            /// </summary>
            [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            internal Font AmbientFont
            {
                get
                {
                    AmbientProperty prop = LookupAmbient(NativeMethods.ActiveX.DISPID_AMBIENT_FONT);

                    if (prop.Empty)
                    {
                        object obj = null;
                        if (GetAmbientProperty(NativeMethods.ActiveX.DISPID_AMBIENT_FONT, ref obj))
                        {
                            try
                            {
                                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Object font type=" + obj.GetType().FullName);
                                Debug.Assert(obj != null, "GetAmbientProperty failed");
                                IntPtr hfont = IntPtr.Zero;

                                UnsafeNativeMethods.IFont ifont = (UnsafeNativeMethods.IFont)obj;
                                Font font = null;
                                hfont = ifont.GetHFont();
                                font = Font.FromHfont(hfont);
                                prop.Value = font;
                            }
                            catch (Exception e)
                            {
                                if (ClientUtils.IsSecurityOrCriticalException(e))
                                {
                                    throw;
                                }

                                // Do NULL, so we just defer to the default font
                                prop.Value = null;
                            }
                        }
                    }

                    return (Font)prop.Value;
                }
            }

            /// <summary>
            ///  Retrieves the ambient back color for the control.
            /// </summary>
            [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            internal Color AmbientForeColor
            {
                get
                {
                    AmbientProperty prop = LookupAmbient(NativeMethods.ActiveX.DISPID_AMBIENT_FORECOLOR);

                    if (prop.Empty)
                    {
                        object obj = null;
                        if (GetAmbientProperty(NativeMethods.ActiveX.DISPID_AMBIENT_FORECOLOR, ref obj))
                        {
                            if (obj != null)
                            {
                                try
                                {
                                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Object color type=" + obj.GetType().FullName);
                                    prop.Value = ColorTranslator.FromOle(Convert.ToInt32(obj, CultureInfo.InvariantCulture));
                                }
                                catch (Exception e)
                                {
                                    Debug.Fail("Failed to massage ambient fore color to a Color", e.ToString());

                                    if (ClientUtils.IsSecurityOrCriticalException(e))
                                    {
                                        throw;
                                    }
                                }
                            }
                        }
                    }

                    if (prop.Value == null)
                    {
                        return Color.Empty;
                    }
                    else
                    {
                        return (Color)prop.Value;
                    }
                }
            }

            /// <summary>
            ///  Determines if events should be frozen.
            /// </summary>
            [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            internal bool EventsFrozen
            {
                get
                {
                    return _activeXState[s_eventsFrozen];
                }
                set
                {
                    _activeXState[s_eventsFrozen] = value;
                }
            }

            /// <summary>
            ///  Provides access to the parent window handle
            ///  when we are UI active
            /// </summary>
            internal IntPtr HWNDParent { get; private set; }

            /// <summary>
            ///  Returns true if this app domain is running inside of IE.  The
            ///  control must be sited for this to succeed (it will assert and
            ///  return false if the control is not sited).
            /// </summary>
            internal bool IsIE
            {
                get
                {
                    if (!s_checkedIE)
                    {
                        if (_clientSite == null)
                        {
                            Debug.Fail("Do not reference IsIE property unless control is sited.");
                            return false;
                        }

                        // First, is this a managed EXE?  If so, it will correctly shut down
                        // the runtime.
                        if (Assembly.GetEntryAssembly() == null)
                        {
                            // Now check for IHTMLDocument2

                            if (NativeMethods.Succeeded(_clientSite.GetContainer(out UnsafeNativeMethods.IOleContainer container)) && container is Mshtml.IHTMLDocument)
                            {
                                s_isIE = true;
                                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "AxSource:IsIE running under IE");
                            }

                            if (container != null && Marshal.IsComObject(container))
                            {
                                Marshal.ReleaseComObject(container);
                            }
                        }

                        s_checkedIE = true;
                    }

                    return s_isIE;
                }
            }

            /// <summary>
            ///  Retrieves the number of logical pixels per inch on the
            ///  primary monitor.
            /// </summary>
            private Point LogPixels
            {
                get
                {
                    if (s_logPixels.IsEmpty)
                    {
                        s_logPixels = new Point();
                        using ScreenDC dc = ScreenDC.Create();
                        s_logPixels.X = Gdi32.GetDeviceCaps(dc, Gdi32.DeviceCapability.LOGPIXELSX);
                        s_logPixels.Y = Gdi32.GetDeviceCaps(dc, Gdi32.DeviceCapability.LOGPIXELSY);
                    }
                    return s_logPixels;
                }
            }

            /// <summary>
            ///  Implements IOleObject::Advise
            /// </summary>
            internal int Advise(IAdviseSink pAdvSink)
            {
                _adviseList.Add(pAdvSink);
                return _adviseList.Count;
            }

            /// <summary>
            ///  Implements IOleObject::Close
            /// </summary>
            internal void Close(int dwSaveOption)
            {
                if (_activeXState[s_inPlaceActive])
                {
                    InPlaceDeactivate();
                }

                if ((dwSaveOption == NativeMethods.OLECLOSE_SAVEIFDIRTY ||
                     dwSaveOption == NativeMethods.OLECLOSE_PROMPTSAVE) &&
                    _activeXState[s_isDirty])
                {

                    if (_clientSite != null)
                    {
                        _clientSite.SaveObject();
                    }
                    SendOnSave();
                }
            }

            /// <summary>
            ///  Implements IOleObject::DoVerb
            /// </summary>
            internal void DoVerb(int iVerb, IntPtr lpmsg, UnsafeNativeMethods.IOleClientSite pActiveSite, int lindex, IntPtr hwndParent, NativeMethods.COMRECT lprcPosRect)
            {
                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "AxSource:ActiveXImpl:DoVerb(" + iVerb + ")");
                switch (iVerb)
                {
                    case NativeMethods.OLEIVERB_SHOW:
                    case NativeMethods.OLEIVERB_INPLACEACTIVATE:
                    case NativeMethods.OLEIVERB_UIACTIVATE:
                    case NativeMethods.OLEIVERB_PRIMARY:
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "DoVerb:Show, InPlaceActivate, UIActivate");
                        InPlaceActivate(iVerb);

                        // Now that we're active, send the lpmsg to the control if it
                        // is valid.
                        if (lpmsg != IntPtr.Zero)
                        {
                            NativeMethods.MSG msg = Marshal.PtrToStructure<NativeMethods.MSG>(lpmsg);
                            Control target = _control;

                            if (msg.hwnd != _control.Handle && msg.message >= WindowMessages.WM_MOUSEFIRST && msg.message <= WindowMessages.WM_MOUSELAST)
                            {
                                // Must translate message coordniates over to our HWND.  We first try
                                IntPtr hwndMap = msg.hwnd == IntPtr.Zero ? hwndParent : msg.hwnd;
                                var pt = new Point
                                {
                                    X = NativeMethods.Util.LOWORD(msg.lParam),
                                    Y = NativeMethods.Util.HIWORD(msg.lParam)
                                };
                                UnsafeNativeMethods.MapWindowPoints(new HandleRef(null, hwndMap), new HandleRef(_control, _control.Handle), ref pt, 1);

                                // check to see if this message should really go to a child
                                //  control, and if so, map the point into that child's window
                                //  coordinates
                                Control realTarget = target.GetChildAtPoint(pt);
                                if (realTarget != null && realTarget != target)
                                {
                                    UnsafeNativeMethods.MapWindowPoints(new HandleRef(target, target.Handle), new HandleRef(realTarget, realTarget.Handle), ref pt, 1);
                                    target = realTarget;
                                }

                                msg.lParam = NativeMethods.Util.MAKELPARAM(pt.X, pt.Y);
                            }

#if DEBUG
                            if (CompModSwitches.ActiveX.TraceVerbose)
                            {
                                Message m = Message.Create(msg.hwnd, msg.message, msg.wParam, msg.lParam);
                                Debug.WriteLine("Valid message pointer passed, sending to control: " + m.ToString());
                            }
#endif

                            if (msg.message == WindowMessages.WM_KEYDOWN && msg.wParam == (IntPtr)NativeMethods.VK_TAB)
                            {
                                target.SelectNextControl(null, Control.ModifierKeys != Keys.Shift, true, true, true);
                            }
                            else
                            {
                                target.SendMessage(msg.message, msg.wParam, msg.lParam);
                            }
                        }
                        break;

                    // These affect our visibility
                    case NativeMethods.OLEIVERB_HIDE:
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "DoVerb:Hide");
                        UIDeactivate();
                        InPlaceDeactivate();
                        if (_activeXState[s_inPlaceVisible])
                        {
                            SetInPlaceVisible(false);
                        }
                        break;

                    // All other verbs are notimpl.
                    default:
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "DoVerb:Other");
                        ThrowHr(NativeMethods.E_NOTIMPL);
                        break;
                }
            }

            /// <summary>
            ///  Implements IViewObject2::Draw.
            /// </summary>
            internal unsafe void Draw(int dwDrawAspect, int lindex, IntPtr pvAspect, NativeMethods.tagDVTARGETDEVICE ptd,
                             IntPtr hdcTargetDev, IntPtr hdcDraw, NativeMethods.COMRECT prcBounds, NativeMethods.COMRECT lprcWBounds,
                             IntPtr pfnContinue, int dwContinue)
            {

                // support the aspects required for multi-pass drawing
                //
                switch (dwDrawAspect)
                {
                    case NativeMethods.DVASPECT_CONTENT:
                    case NativeMethods.DVASPECT_OPAQUE:
                    case NativeMethods.DVASPECT_TRANSPARENT:
                        break;
                    default:
                        ThrowHr(NativeMethods.DV_E_DVASPECT);
                        break;
                }

                // We can paint to an enhanced metafile, but not all GDI / GDI+ is
                // supported on classic metafiles.  We throw VIEW_E_DRAW in the hope that
                // the caller figures it out and sends us a different DC.
                //
                Gdi32.ObjectType hdcType = Gdi32.GetObjectType(hdcDraw);
                if (hdcType == Gdi32.ObjectType.OBJ_METADC)
                {
                    ThrowHr(NativeMethods.VIEW_E_DRAW);
                }

                RECT rc;
                var pVp = new Point();
                var pW = new Point();
                var sWindowExt = new Size();
                var sViewportExt = new Size();
                int iMode = NativeMethods.MM_TEXT;

                if (!_control.IsHandleCreated)
                {
                    _control.CreateHandle();
                }

                // if they didn't give us a rectangle, just copy over ours
                //
                if (prcBounds != null)
                {
                    rc = new RECT(prcBounds.left, prcBounds.top, prcBounds.right, prcBounds.bottom);

                    // To draw to a given rect, we scale the DC in such a way as to
                    // make the values it takes match our own happy MM_TEXT.  Then,
                    // we back-convert prcBounds so that we convert it to this coordinate
                    // system.  This puts us in the most similar coordinates as we currently
                    // use.
                    SafeNativeMethods.LPtoDP(new HandleRef(null, hdcDraw), ref rc, 2);

                    iMode = SafeNativeMethods.SetMapMode(new HandleRef(null, hdcDraw), NativeMethods.MM_ANISOTROPIC);
                    SafeNativeMethods.SetWindowOrgEx(hdcDraw, 0, 0, &pW);
                    SafeNativeMethods.SetWindowExtEx(hdcDraw, _control.Width, _control.Height, &sWindowExt);
                    SafeNativeMethods.SetViewportOrgEx(hdcDraw, rc.left, rc.top, &pVp);
                    SafeNativeMethods.SetViewportExtEx(hdcDraw, rc.right - rc.left, rc.bottom - rc.top, &sViewportExt);
                }

                // Now do the actual drawing.  We must ask all of our children to draw as well.
                try
                {
                    IntPtr flags = (IntPtr)(NativeMethods.PRF_CHILDREN
                                    | NativeMethods.PRF_CLIENT
                                    | NativeMethods.PRF_ERASEBKGND
                                    | NativeMethods.PRF_NONCLIENT);

                    if (hdcType != Gdi32.ObjectType.OBJ_ENHMETADC)
                    {
                        _control.SendMessage(WindowMessages.WM_PRINT, hdcDraw, flags);
                    }
                    else
                    {
                        _control.PrintToMetaFile(new HandleRef(null, hdcDraw), flags);
                    }
                }
                finally
                {
                    // And clean up the DC
                    if (prcBounds != null)
                    {
                        SafeNativeMethods.SetWindowOrgEx(hdcDraw, pW.X, pW.Y, null);
                        SafeNativeMethods.SetWindowExtEx(hdcDraw, sWindowExt.Width, sWindowExt.Height, null);
                        SafeNativeMethods.SetViewportOrgEx(hdcDraw, pVp.X, pVp.Y, null);
                        SafeNativeMethods.SetViewportExtEx(hdcDraw, sViewportExt.Width, sViewportExt.Height, null);
                        SafeNativeMethods.SetMapMode(new HandleRef(null, hdcDraw), iMode);
                    }
                }
            }

            /// <summary>
            ///  Returns a new verb enumerator.
            /// </summary>
            internal static int EnumVerbs(out UnsafeNativeMethods.IEnumOLEVERB e)
            {
                if (s_axVerbs == null)
                {
                    NativeMethods.tagOLEVERB verbShow = new NativeMethods.tagOLEVERB();
                    NativeMethods.tagOLEVERB verbInplaceActivate = new NativeMethods.tagOLEVERB();
                    NativeMethods.tagOLEVERB verbUIActivate = new NativeMethods.tagOLEVERB();
                    NativeMethods.tagOLEVERB verbHide = new NativeMethods.tagOLEVERB();
                    NativeMethods.tagOLEVERB verbPrimary = new NativeMethods.tagOLEVERB();
                    NativeMethods.tagOLEVERB verbProperties = new NativeMethods.tagOLEVERB();

                    verbShow.lVerb = NativeMethods.OLEIVERB_SHOW;
                    verbInplaceActivate.lVerb = NativeMethods.OLEIVERB_INPLACEACTIVATE;
                    verbUIActivate.lVerb = NativeMethods.OLEIVERB_UIACTIVATE;
                    verbHide.lVerb = NativeMethods.OLEIVERB_HIDE;
                    verbPrimary.lVerb = NativeMethods.OLEIVERB_PRIMARY;
                    verbProperties.lVerb = NativeMethods.OLEIVERB_PROPERTIES;
                    verbProperties.lpszVerbName = SR.AXProperties;
                    verbProperties.grfAttribs = NativeMethods.ActiveX.OLEVERBATTRIB_ONCONTAINERMENU;

                    s_axVerbs = new NativeMethods.tagOLEVERB[] {
                        verbShow,
                        verbInplaceActivate,
                        verbUIActivate,
                        verbHide,
                        verbPrimary
                    };
                }

                e = new ActiveXVerbEnum(s_axVerbs);
                return NativeMethods.S_OK;
            }

            /// <summary>
            ///  Converts the given string to a byte array.
            /// </summary>
            private static byte[] FromBase64WrappedString(string text)
            {
                if (text.IndexOfAny(new char[] { ' ', '\r', '\n' }) != -1)
                {
                    StringBuilder sb = new StringBuilder(text.Length);
                    for (int i = 0; i < text.Length; i++)
                    {
                        switch (text[i])
                        {
                            case ' ':
                            case '\r':
                            case '\n':
                                break;
                            default:
                                sb.Append(text[i]);
                                break;
                        }
                    }
                    return Convert.FromBase64String(sb.ToString());
                }
                else
                {
                    return Convert.FromBase64String(text);
                }
            }

            /// <summary>
            ///  Implements IViewObject2::GetAdvise.
            /// </summary>
            internal void GetAdvise(int[] paspects, int[] padvf, IAdviseSink[] pAdvSink)
            {
                // if they want it, give it to them
                if (paspects != null)
                {
                    paspects[0] = NativeMethods.DVASPECT_CONTENT;
                }

                if (padvf != null)
                {
                    padvf[0] = 0;

                    if (_activeXState[s_viewAdviseOnlyOnce])
                    {
                        padvf[0] |= NativeMethods.ADVF_ONLYONCE;
                    }

                    if (_activeXState[s_viewAdvisePrimeFirst])
                    {
                        padvf[0] |= NativeMethods.ADVF_PRIMEFIRST;
                    }
                }

                if (pAdvSink != null)
                {
                    pAdvSink[0] = _viewAdviseSink;
                }
            }

            /// <summary>
            ///  Helper function to retrieve an ambient property.  Returns false if the
            ///  property wasn't found.
            /// </summary>
            private bool GetAmbientProperty(int dispid, ref object obj)
            {
                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetAmbientProperty");
                Debug.Indent();

                if (_clientSite is UnsafeNativeMethods.IDispatch)
                {
                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "clientSite implements IDispatch");

                    UnsafeNativeMethods.IDispatch disp = (UnsafeNativeMethods.IDispatch)_clientSite;
                    object[] pvt = new object[1];
                    Guid g = Guid.Empty;
                    int hr = disp.Invoke(dispid, ref g, NativeMethods.LOCALE_USER_DEFAULT,
                                        NativeMethods.DISPATCH_PROPERTYGET, new NativeMethods.tagDISPPARAMS(),
                                        pvt, null, null);
                    if (NativeMethods.Succeeded(hr))
                    {
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "IDispatch::Invoke succeeded. VT=" + pvt[0].GetType().FullName);
                        obj = pvt[0];
                        Debug.Unindent();
                        return true;
                    }
                    else
                    {
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "IDispatch::Invoke failed. HR: 0x" + string.Format(CultureInfo.CurrentCulture, "{0:X}", hr));
                    }
                }

                Debug.Unindent();
                return false;
            }

            /// <summary>
            ///  Implements IOleObject::GetClientSite.
            /// </summary>
            internal UnsafeNativeMethods.IOleClientSite GetClientSite()
            {
                return _clientSite;
            }

            internal int GetControlInfo(NativeMethods.tagCONTROLINFO pCI)
            {
                if (_accelCount == -1)
                {
                    ArrayList mnemonicList = new ArrayList();
                    GetMnemonicList(_control, mnemonicList);

                    _accelCount = (short)mnemonicList.Count;

                    if (_accelCount > 0)
                    {
                        int accelSize = Marshal.SizeOf<NativeMethods.ACCEL>();

                        // In the worst case we may have two accelerators per mnemonic:  one lower case and
                        // one upper case, hence the * 2 below.
                        //
                        IntPtr accelBlob = Marshal.AllocHGlobal(accelSize * _accelCount * 2);

                        try
                        {
                            NativeMethods.ACCEL accel = new NativeMethods.ACCEL
                            {
                                cmd = 0
                            };

                            Debug.Indent();

                            _accelCount = 0;

                            foreach (char ch in mnemonicList)
                            {
                                IntPtr structAddr = (IntPtr)((long)accelBlob + _accelCount * accelSize);

                                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Mnemonic: " + ch.ToString());

                                if (ch >= 'A' && ch <= 'Z')
                                {
                                    // Lower case letter
                                    accel.fVirt = NativeMethods.FALT | NativeMethods.FVIRTKEY;
                                    accel.key = (short)(UnsafeNativeMethods.VkKeyScan(ch) & 0x00FF);
                                    Marshal.StructureToPtr(accel, structAddr, false);

                                    // Upper case letter
                                    _accelCount++;
                                    structAddr = (IntPtr)((long)structAddr + accelSize);
                                    accel.fVirt = NativeMethods.FALT | NativeMethods.FVIRTKEY | NativeMethods.FSHIFT;
                                    Marshal.StructureToPtr(accel, structAddr, false);
                                }
                                else
                                {
                                    // Some non-printable character.
                                    accel.fVirt = NativeMethods.FALT | NativeMethods.FVIRTKEY;
                                    short scan = (short)(UnsafeNativeMethods.VkKeyScan(ch));
                                    if ((scan & 0x0100) != 0)
                                    {
                                        accel.fVirt |= NativeMethods.FSHIFT;
                                    }
                                    accel.key = (short)(scan & 0x00FF);
                                    Marshal.StructureToPtr(accel, structAddr, false);
                                }

                                accel.cmd++;
                                _accelCount++;
                            }

                            Debug.Unindent();

                            // Now create an accelerator table and then free our memory.

                            if (_accelTable != IntPtr.Zero)
                            {
                                UnsafeNativeMethods.DestroyAcceleratorTable(new HandleRef(this, _accelTable));
                                _accelTable = IntPtr.Zero;
                            }

                            _accelTable = UnsafeNativeMethods.CreateAcceleratorTable(new HandleRef(null, accelBlob), _accelCount);
                        }
                        finally
                        {
                            if (accelBlob != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(accelBlob);
                            }
                        }
                    }
                }

                pCI.cAccel = _accelCount;
                pCI.hAccel = _accelTable;
                return NativeMethods.S_OK;
            }

            /// <summary>
            ///  Implements IOleObject::GetExtent.
            /// </summary>
            internal unsafe void GetExtent(uint dwDrawAspect, Size* pSizel)
            {
                if ((dwDrawAspect & NativeMethods.DVASPECT_CONTENT) != 0)
                {
                    Size size = _control.Size;

                    Point pt = PixelToHiMetric(size.Width, size.Height);
                    pSizel->Width = pt.X;
                    pSizel->Height = pt.Y;
                }
                else
                {
                    ThrowHr(NativeMethods.DV_E_DVASPECT);
                }
            }

            /// <summary>
            ///  Searches the control hierarchy of the given control and adds
            ///  the mnemonics for each control to mnemonicList.  Each mnemonic
            ///  is added as a char to the list.
            /// </summary>
            private void GetMnemonicList(Control control, ArrayList mnemonicList)
            {
                // Get the mnemonic for our control
                char mnemonic = WindowsFormsUtils.GetMnemonic(control.Text, true);
                if (mnemonic != 0)
                {
                    mnemonicList.Add(mnemonic);
                }

                // And recurse for our children.
                foreach (Control c in control.Controls)
                {
                    if (c != null)
                    {
                        GetMnemonicList(c, mnemonicList);
                    }
                }
            }

            /// <summary>
            ///  Name to use for a stream: use the control's type name (max 31 chars, use the end chars
            ///  if it's longer than that)
            /// </summary>
            private string GetStreamName()
            {
                string streamName = _control.GetType().FullName;
                int len = streamName.Length;
                if (len > 31)
                {
                    // The max allowed length of the stream name is 31.
                    streamName = streamName.Substring(len - 31);
                }
                return streamName;
            }

            /// <summary>
            ///  Implements IOleWindow::GetWindow
            /// </summary>
            internal int GetWindow(out IntPtr hwnd)
            {
                if (!_activeXState[s_inPlaceActive])
                {
                    hwnd = IntPtr.Zero;
                    return NativeMethods.E_FAIL;
                }
                hwnd = _control.Handle;
                return NativeMethods.S_OK;
            }

            /// <summary>
            ///  Converts coordinates in HiMetric to pixels.  Used for ActiveX sourcing.
            /// </summary>
            private Point HiMetricToPixel(int x, int y)
            {
                Point pt = new Point
                {
                    X = (LogPixels.X * x + HiMetricPerInch / 2) / HiMetricPerInch,
                    Y = (LogPixels.Y * y + HiMetricPerInch / 2) / HiMetricPerInch
                };
                return pt;
            }

            /// <summary>
            ///  In place activates this Object.
            /// </summary>
            internal void InPlaceActivate(int verb)
            {
                // If we don't have a client site, then there's not much to do.
                // We also punt if this isn't an in-place site, since we can't
                // go active then.
                if (!(_clientSite is UnsafeNativeMethods.IOleInPlaceSite inPlaceSite))
                {
                    return;
                }

                // If we're not already active, go and do it.
                if (!_activeXState[s_inPlaceActive])
                {
                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "\tActiveXImpl:InPlaceActivate --> inplaceactive");

                    int hr = inPlaceSite.CanInPlaceActivate();

                    if (hr != NativeMethods.S_OK)
                    {
                        if (NativeMethods.Succeeded(hr))
                        {
                            hr = NativeMethods.E_FAIL;
                        }
                        ThrowHr(hr);
                    }

                    inPlaceSite.OnInPlaceActivate();

                    _activeXState[s_inPlaceActive] = true;
                }

                // And if we're not visible, do that too.
                if (!_activeXState[s_inPlaceVisible])
                {
                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "\tActiveXImpl:InPlaceActivate --> inplacevisible");
                    NativeMethods.tagOIFI inPlaceFrameInfo = new NativeMethods.tagOIFI
                    {
                        cb = Marshal.SizeOf<NativeMethods.tagOIFI>()
                    };
                    IntPtr hwndParent = IntPtr.Zero;

                    // We are entering a secure context here.
                    hwndParent = inPlaceSite.GetWindow();

                    NativeMethods.COMRECT posRect = new NativeMethods.COMRECT();
                    NativeMethods.COMRECT clipRect = new NativeMethods.COMRECT();

                    if (_inPlaceUiWindow != null && Marshal.IsComObject(_inPlaceUiWindow))
                    {
                        Marshal.ReleaseComObject(_inPlaceUiWindow);
                        _inPlaceUiWindow = null;
                    }

                    if (_inPlaceFrame != null && Marshal.IsComObject(_inPlaceFrame))
                    {
                        Marshal.ReleaseComObject(_inPlaceFrame);
                        _inPlaceFrame = null;
                    }

                    inPlaceSite.GetWindowContext(out UnsafeNativeMethods.IOleInPlaceFrame pFrame, out UnsafeNativeMethods.IOleInPlaceUIWindow pWindow, posRect, clipRect, inPlaceFrameInfo);

                    SetObjectRects(posRect, clipRect);

                    _inPlaceFrame = pFrame;
                    _inPlaceUiWindow = pWindow;

                    // We are parenting ourselves
                    // directly to the host window.  The host must
                    // implement the ambient property
                    // DISPID_AMBIENT_MESSAGEREFLECT.
                    // If it doesn't, that means that the host
                    // won't reflect messages back to us.
                    HWNDParent = hwndParent;
                    UnsafeNativeMethods.SetParent(new HandleRef(_control, _control.Handle), new HandleRef(null, hwndParent));

                    // Now create our handle if it hasn't already been done.
                    _control.CreateControl();

                    _clientSite.ShowObject();

                    SetInPlaceVisible(true);
                    Debug.Assert(_activeXState[s_inPlaceVisible], "Failed to set inplacevisible");
                }

                // if we weren't asked to UIActivate, then we're done.
                if (verb != NativeMethods.OLEIVERB_PRIMARY && verb != NativeMethods.OLEIVERB_UIACTIVATE)
                {
                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "\tActiveXImpl:InPlaceActivate --> not becoming UIActive");
                    return;
                }

                // if we're not already UI active, do sow now.
                if (!_activeXState[s_uiActive])
                {
                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "\tActiveXImpl:InPlaceActivate --> uiactive");
                    _activeXState[s_uiActive] = true;

                    // inform the container of our intent
                    inPlaceSite.OnUIActivate();

                    // take the focus  [which is what UI Activation is all about !]
                    if (!_control.ContainsFocus)
                    {
                        _control.Focus();
                    }

                    // set ourselves up in the host.
                    Debug.Assert(_inPlaceFrame != null, "Setting us to visible should have created the in place frame");
                    _inPlaceFrame.SetActiveObject(_control, null);
                    if (_inPlaceUiWindow != null)
                    {
                        _inPlaceUiWindow.SetActiveObject(_control, null);
                    }

                    // we have to explicitly say we don't wany any border space.
                    int hr = _inPlaceFrame.SetBorderSpace(null);
                    if (NativeMethods.Failed(hr) && hr != NativeMethods.OLE_E_INVALIDRECT &&
                        hr != NativeMethods.INPLACE_E_NOTOOLSPACE && hr != NativeMethods.E_NOTIMPL)
                    {
                        Marshal.ThrowExceptionForHR(hr);
                    }

                    if (_inPlaceUiWindow != null)
                    {
                        hr = _inPlaceFrame.SetBorderSpace(null);
                        if (NativeMethods.Failed(hr) && hr != NativeMethods.OLE_E_INVALIDRECT &&
                            hr != NativeMethods.INPLACE_E_NOTOOLSPACE && hr != NativeMethods.E_NOTIMPL)
                        {
                            Marshal.ThrowExceptionForHR(hr);
                        }
                    }
                }
                else
                {
                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "\tActiveXImpl:InPlaceActivate --> already uiactive");
                }
            }

            /// <summary>
            ///  Implements IOleInPlaceObject::InPlaceDeactivate.
            /// </summary>
            internal void InPlaceDeactivate()
            {
                // Only do this if we're already in place active.
                if (!_activeXState[s_inPlaceActive])
                {
                    return;
                }

                // Deactivate us if we're UI active
                if (_activeXState[s_uiActive])
                {
                    UIDeactivate();
                }

                // Some containers may call into us to save, and if we're still
                // active we will try to deactivate and recurse back into the container.
                // So, set the state bits here first.
                _activeXState[s_inPlaceActive] = false;
                _activeXState[s_inPlaceVisible] = false;

                // Notify our site of our deactivation.
                if (_clientSite is UnsafeNativeMethods.IOleInPlaceSite oleClientSite)
                {
                    oleClientSite.OnInPlaceDeactivate();
                }

                _control.Visible = false;
                HWNDParent = IntPtr.Zero;

                if (_inPlaceUiWindow != null && Marshal.IsComObject(_inPlaceUiWindow))
                {
                    Marshal.ReleaseComObject(_inPlaceUiWindow);
                    _inPlaceUiWindow = null;
                }

                if (_inPlaceFrame != null && Marshal.IsComObject(_inPlaceFrame))
                {
                    Marshal.ReleaseComObject(_inPlaceFrame);
                    _inPlaceFrame = null;
                }
            }

            /// <summary>
            ///  Implements IPersistStreamInit::IsDirty.
            /// </summary>
            internal HRESULT IsDirty()
            {
                if (_activeXState[s_isDirty])
                {
                    return HRESULT.S_OK;
                }
                
                return HRESULT.S_FALSE;
            }

            /// <summary>
            ///  Looks at the property to see if it should be loaded / saved as a resource or
            ///  through a type converter.
            /// </summary>
            private bool IsResourceProp(PropertyDescriptor prop)
            {
                TypeConverter converter = prop.Converter;
                Type[] convertTypes = new Type[] {
                    typeof(string),
                    typeof(byte[])
                    };

                foreach (Type t in convertTypes)
                {
                    if (converter.CanConvertTo(t) && converter.CanConvertFrom(t))
                    {
                        return false;
                    }
                }

                // Finally, if the property can be serialized, it is a resource property.
                return (prop.GetValue(_control) is ISerializable);
            }

            /// <summary>
            ///  Implements IPersistStorage::Load
            /// </summary>
            internal void Load(Ole32.IStorage stg)
            {
                Ole32.IStream stream;
                try
                {
                    stream = stg.OpenStream(
                        GetStreamName(),
                        IntPtr.Zero,
                        Ole32.STGM.STGM_READ | Ole32.STGM.STGM_SHARE_EXCLUSIVE,
                        0);
                }
                catch (COMException e) when (e.ErrorCode == (int)HRESULT.STG_E_FILENOTFOUND)
                {
                    // For backward compatibility: We were earlier using GetType().FullName
                    // as the stream name in v1. Lets see if a stream by that name exists.
                    stream = stg.OpenStream(
                        GetType().FullName,
                        IntPtr.Zero,
                        Ole32.STGM.STGM_READ | Ole32.STGM.STGM_SHARE_EXCLUSIVE,
                        0);
                }

                Load(stream);
                if (Marshal.IsComObject(stg))
                {
                    Marshal.ReleaseComObject(stg);
                }
            }

            /// <summary>
            ///  Implements IPersistStreamInit::Load
            /// </summary>
            internal void Load(Ole32.IStream stream)
            {
                // We do everything through property bags because we support full fidelity
                // in them.  So, load through that method.
                PropertyBagStream bag = new PropertyBagStream();
                bag.Read(stream);
                Load(bag, null);

                if (Marshal.IsComObject(stream))
                {
                    Marshal.ReleaseComObject(stream);
                }
            }

            /// <summary>
            ///  Implements IPersistPropertyBag::Load
            /// </summary>
            internal void Load(UnsafeNativeMethods.IPropertyBag pPropBag, UnsafeNativeMethods.IErrorLog pErrorLog)
            {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(_control,
                    new Attribute[] { DesignerSerializationVisibilityAttribute.Visible });

                for (int i = 0; i < props.Count; i++)
                {
                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Loading property " + props[i].Name);

                    try
                    {
                        object obj = null;
                        int hr = pPropBag.Read(props[i].Name, ref obj, pErrorLog);

                        if (NativeMethods.Succeeded(hr) && obj != null)
                        {
                            Debug.Indent();
                            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Property was in bag");

                            string errorString = null;
                            int errorCode = 0;

                            try
                            {
                                if (obj.GetType() != typeof(string))
                                {
                                    Debug.Fail("Expected property " + props[i].Name + " to be stored in IPropertyBag as a string.  Attempting to coerce");
                                    obj = Convert.ToString(obj, CultureInfo.InvariantCulture);
                                }

                                // Determine if this is a resource property or was persisted via a type converter.
                                if (IsResourceProp(props[i]))
                                {
                                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "It's a resource property");

                                    // Resource property.  We encode these as base 64 strings.  To load them, we convert
                                    // to a binary blob and then de-serialize.
                                    byte[] bytes = Convert.FromBase64String(obj.ToString());
                                    MemoryStream stream = new MemoryStream(bytes);
                                    BinaryFormatter formatter = new BinaryFormatter();
                                    props[i].SetValue(_control, formatter.Deserialize(stream));
                                }
                                else
                                {
                                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "It's a standard property");

                                    // Not a resource property.  Use TypeConverters to convert the string back to the data type.  We do
                                    // not check for CanConvertFrom here -- we the conversion fails the type converter will throw,
                                    // and we will log it into the COM error log.
                                    TypeConverter converter = props[i].Converter;
                                    Debug.Assert(converter != null, "No type converter for property '" + props[i].Name + "' on class " + _control.GetType().FullName);

                                    // Check to see if the type converter can convert from a string.  If it can,.
                                    // use that as it is the best format for IPropertyBag.  Otherwise, check to see
                                    // if it can convert from a byte array.  If it can, get the string, decode it
                                    // to a byte array, and then set the value.
                                    object value = null;

                                    if (converter.CanConvertFrom(typeof(string)))
                                    {
                                        value = converter.ConvertFromInvariantString(obj.ToString());
                                    }
                                    else if (converter.CanConvertFrom(typeof(byte[])))
                                    {
                                        string objString = obj.ToString();
                                        value = converter.ConvertFrom(null, CultureInfo.InvariantCulture, FromBase64WrappedString(objString));
                                    }
                                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Converter returned " + value);
                                    props[i].SetValue(_control, value);
                                }
                            }
                            catch (Exception e)
                            {
                                errorString = e.ToString();
                                if (e is ExternalException)
                                {
                                    errorCode = ((ExternalException)e).ErrorCode;
                                }
                                else
                                {
                                    errorCode = NativeMethods.E_FAIL;
                                }
                            }
                            if (errorString != null)
                            {
                                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Exception converting property: " + errorString);
                                if (pErrorLog != null)
                                {
                                    NativeMethods.tagEXCEPINFO err = new NativeMethods.tagEXCEPINFO
                                    {
                                        bstrSource = _control.GetType().FullName,
                                        bstrDescription = errorString,
                                        scode = errorCode
                                    };
                                    pErrorLog.AddError(props[i].Name, err);
                                }
                            }
                            Debug.Unindent();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Fail("Unexpected failure reading property", ex.ToString());

                        if (ClientUtils.IsSecurityOrCriticalException(ex))
                        {
                            throw;
                        }
                    }
                }
                if (Marshal.IsComObject(pPropBag))
                {
                    Marshal.ReleaseComObject(pPropBag);
                }
            }

            /// <summary>
            ///  Simple lookup to find the AmbientProperty corresponding to the given
            ///  dispid.
            /// </summary>
            private AmbientProperty LookupAmbient(int dispid)
            {
                for (int i = 0; i < _ambientProperties.Length; i++)
                {
                    if (_ambientProperties[i].DispID == dispid)
                    {
                        return _ambientProperties[i];
                    }
                }
                Debug.Fail("No ambient property for dispid " + dispid.ToString(CultureInfo.InvariantCulture));
                return _ambientProperties[0];
            }

            /// <summary>
            ///  Merges the input region with the current clipping region.
            ///  The output is always a region that can be fed directly
            ///  to SetWindowRgn.  The region does not have to be destroyed.
            ///  The original region is destroyed if a new region is returned.
            /// </summary>
            internal IntPtr MergeRegion(IntPtr region)
            {
                if (_clipRegion == IntPtr.Zero)
                {
                    return region;
                }

                if (region == IntPtr.Zero)
                {
                    return _clipRegion;
                }

                IntPtr newRegion = Gdi32.CreateRectRgn(0, 0, 0, 0);
                Gdi32.CombineRgn(newRegion, region, _clipRegion, Gdi32.CombineMode.RGN_DIFF);
                Gdi32.DeleteObject(region);
                return newRegion;
            }

            private void CallParentPropertyChanged(Control control, string propName)
            {
                switch (propName)
                {
                    case "BackColor":
                        control.OnParentBackColorChanged(EventArgs.Empty);
                        break;
                    case "BackgroundImage":
                        control.OnParentBackgroundImageChanged(EventArgs.Empty);
                        break;
                    case "BindingContext":
                        control.OnParentBindingContextChanged(EventArgs.Empty);
                        break;
                    case "Enabled":
                        control.OnParentEnabledChanged(EventArgs.Empty);
                        break;
                    case "Font":
                        control.OnParentFontChanged(EventArgs.Empty);
                        break;
                    case "ForeColor":
                        control.OnParentForeColorChanged(EventArgs.Empty);
                        break;
                    case "RightToLeft":
                        control.OnParentRightToLeftChanged(EventArgs.Empty);
                        break;
                    case "Visible":
                        control.OnParentVisibleChanged(EventArgs.Empty);
                        break;
                    default:
                        Debug.Fail("There is no property change notification for: " + propName + " on Control.");
                        break;
                }
            }

            /// <summary>
            ///  Implements IOleControl::OnAmbientPropertyChanged
            /// </summary>
            internal void OnAmbientPropertyChange(int dispID)
            {
                if (dispID != NativeMethods.ActiveX.DISPID_UNKNOWN)
                {
                    // Look for a specific property that has changed.
                    for (int i = 0; i < _ambientProperties.Length; i++)
                    {
                        if (_ambientProperties[i].DispID == dispID)
                        {
                            _ambientProperties[i].ResetValue();
                            CallParentPropertyChanged(_control, _ambientProperties[i].Name);
                            return;
                        }
                    }

                    // Special properties that we care about
                    object obj = new object();

                    switch (dispID)
                    {
                        case NativeMethods.ActiveX.DISPID_AMBIENT_UIDEAD:
                            if (GetAmbientProperty(NativeMethods.ActiveX.DISPID_AMBIENT_UIDEAD, ref obj))
                            {
                                _activeXState[s_uiDead] = (bool)obj;
                            }
                            break;

                        case NativeMethods.ActiveX.DISPID_AMBIENT_DISPLAYASDEFAULT:
                            if (_control is IButtonControl ibuttonControl && GetAmbientProperty(NativeMethods.ActiveX.DISPID_AMBIENT_DISPLAYASDEFAULT, ref obj))
                            {
                                ibuttonControl.NotifyDefault((bool)obj);
                            }
                            break;
                    }
                }
                else
                {
                    // Invalidate all properties.  Ideally we should be checking each one, but
                    // that's pretty expensive too.
                    for (int i = 0; i < _ambientProperties.Length; i++)
                    {
                        _ambientProperties[i].ResetValue();
                        CallParentPropertyChanged(_control, _ambientProperties[i].Name);
                    }
                }
            }

            /// <summary>
            ///  Implements IOleInPlaceActiveObject::OnDocWindowActivate.
            /// </summary>
            internal void OnDocWindowActivate(int fActivate)
            {
                if (_activeXState[s_uiActive] && fActivate != 0 && _inPlaceFrame != null)
                {
                    // we have to explicitly say we don't wany any border space.
                    int hr = _inPlaceFrame.SetBorderSpace(null);

                    if (NativeMethods.Failed(hr) && hr != NativeMethods.INPLACE_E_NOTOOLSPACE && hr != NativeMethods.E_NOTIMPL)
                    {
                        Marshal.ThrowExceptionForHR(hr);
                    }
                }
            }

            /// <summary>
            ///  Called by Control when it gets the focus.
            /// </summary>
            internal void OnFocus(bool focus)
            {
                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AXSource: SetFocus:  " + focus.ToString());
                if (_activeXState[s_inPlaceActive] && _clientSite is UnsafeNativeMethods.IOleControlSite)
                {
                    ((UnsafeNativeMethods.IOleControlSite)_clientSite).OnFocus(focus ? 1 : 0);
                }

                if (focus && _activeXState[s_inPlaceActive] && !_activeXState[s_uiActive])
                {
                    InPlaceActivate(NativeMethods.OLEIVERB_UIACTIVATE);
                }
            }

            /// <summary>
            ///  Converts coordinates in pixels to HiMetric.
            /// </summary>
            private Point PixelToHiMetric(int x, int y)
            {
                Point pt = new Point
                {
                    X = (HiMetricPerInch * x + (LogPixels.X >> 1)) / LogPixels.X,
                    Y = (HiMetricPerInch * y + (LogPixels.Y >> 1)) / LogPixels.Y
                };
                return pt;
            }

            /// <summary>
            ///  Our implementation of IQuickActivate::QuickActivate
            /// </summary>
            internal void QuickActivate(UnsafeNativeMethods.tagQACONTAINER pQaContainer, UnsafeNativeMethods.tagQACONTROL pQaControl)
            {
                // Hookup our ambient colors
                AmbientProperty prop = LookupAmbient(NativeMethods.ActiveX.DISPID_AMBIENT_BACKCOLOR);
                prop.Value = ColorTranslator.FromOle(unchecked((int)pQaContainer.colorBack));

                prop = LookupAmbient(NativeMethods.ActiveX.DISPID_AMBIENT_FORECOLOR);
                prop.Value = ColorTranslator.FromOle(unchecked((int)pQaContainer.colorFore));

                // And our ambient font
                if (pQaContainer.pFont != null)
                {
                    prop = LookupAmbient(NativeMethods.ActiveX.DISPID_AMBIENT_FONT);

                    try
                    {
                        IntPtr hfont = IntPtr.Zero;
                        object objfont = pQaContainer.pFont;
                        UnsafeNativeMethods.IFont ifont = (UnsafeNativeMethods.IFont)objfont;
                        hfont = ifont.GetHFont();
                        Font font = Font.FromHfont(hfont);
                        prop.Value = font;
                    }
                    catch (Exception e)
                    {
                        if (ClientUtils.IsSecurityOrCriticalException(e))
                        {
                            throw;
                        }

                        // Do NULL, so we just defer to the default font
                        prop.Value = null;
                    }
                }

                // Now use the rest of the goo that we got passed in.

                pQaControl.cbSize = Marshal.SizeOf<UnsafeNativeMethods.tagQACONTROL>();

                SetClientSite(pQaContainer.pClientSite);

                if (pQaContainer.pAdviseSink != null)
                {
                    SetAdvise(NativeMethods.DVASPECT_CONTENT, 0, (IAdviseSink)pQaContainer.pAdviseSink);
                }

                ((UnsafeNativeMethods.IOleObject)_control).GetMiscStatus(NativeMethods.DVASPECT_CONTENT, out int status);
                pQaControl.dwMiscStatus = status;

                // Advise the event sink so VB6 can catch events raised from UserControls.
                // VB6 expects the control to do this during IQuickActivate, otherwise it will not hook events at runtime.
                // We will do this if all of the following are true:
                //  1. The container (e.g., vb6) has supplied an event sink
                //  2. The control is a UserControl (this is only to limit the scope of the changed behavior)
                //  3. The UserControl has indicated it wants to expose events to COM via the ComSourceInterfacesAttribute
                // Note that the AdviseHelper handles some non-standard COM interop that is required in order to access
                // the events on the CLR-supplied CCW (COM-callable Wrapper.

                if ((pQaContainer.pUnkEventSink != null) && (_control is UserControl))
                {
                    // Check if this control exposes events to COM.
                    Type eventInterface = GetDefaultEventsInterface(_control.GetType());

                    if (eventInterface != null)
                    {
                        try
                        {
                            // For the default source interface, call IConnectionPoint.Advise with the supplied event sink.
                            // This is easier said than done. See notes in AdviseHelper.AdviseConnectionPoint.
                            AdviseHelper.AdviseConnectionPoint(_control, pQaContainer.pUnkEventSink, eventInterface, out pQaControl.dwEventCookie);
                        }
                        catch (Exception e)
                        {
                            if (ClientUtils.IsSecurityOrCriticalException(e))
                            {
                                throw;
                            }
                        }
                    }
                }

                if (pQaContainer.pPropertyNotifySink != null && Marshal.IsComObject(pQaContainer.pPropertyNotifySink))
                {
                    Marshal.ReleaseComObject(pQaContainer.pPropertyNotifySink);
                }

                if (pQaContainer.pUnkEventSink != null && Marshal.IsComObject(pQaContainer.pUnkEventSink))
                {
                    Marshal.ReleaseComObject(pQaContainer.pUnkEventSink);
                }
            }

            /// <summary>
            ///  Helper class. Calls IConnectionPoint.Advise to hook up a native COM event sink
            ///  to a manage .NET event interface.
            ///  The events are exposed to COM by the CLR-supplied COM-callable Wrapper (CCW).
            /// </summary>
            internal static class AdviseHelper
            {
                /// <summary>
                ///  Get the COM connection point container from the CLR's CCW and advise for the given event id.
                /// </summary>
                public static bool AdviseConnectionPoint(object connectionPoint, object sink, Type eventInterface, out int cookie)
                {
                    // Note that we cannot simply cast the connectionPoint object to
                    // System.Runtime.InteropServices.ComTypes.IConnectionPointContainer because the .NET
                    // object doesn't implement it directly. When the object is exposed to COM, the CLR
                    // implements IConnectionPointContainer on the proxy object called the CCW or COM-callable wrapper.
                    // We use the helper class ComConnectionPointContainer to get to the CCW directly
                    // to to call the interface.
                    // It is critical to call Dispose to ensure that the IUnknown is released.

                    using (ComConnectionPointContainer cpc = new ComConnectionPointContainer(connectionPoint, true))
                    {
                        return AdviseConnectionPoint(cpc, sink, eventInterface, out cookie);
                    }
                }

                /// <summary>
                ///  Find the COM connection point and call Advise for the given event id.
                /// </summary>
                internal static bool AdviseConnectionPoint(ComConnectionPointContainer cpc, object sink, Type eventInterface, out int cookie)
                {
                    // Note that we cannot simply cast the returned IConnectionPoint to
                    // System.Runtime.InteropServices.ComTypes.IConnectionPoint because the .NET
                    // object doesn't implement it directly. When the object is exposed to COM, the CLR
                    // implements IConnectionPoint for the proxy object via the CCW or COM-callable wrapper.
                    // We use the helper class ComConnectionPoint to get to the CCW directly to to call the interface.
                    // It is critical to call Dispose to ensure that the IUnknown is released.
                    using (ComConnectionPoint cp = cpc.FindConnectionPoint(eventInterface))
                    {
                        using (SafeIUnknown punkEventsSink = new SafeIUnknown(sink, true))
                        {
                            // Finally...we can call IConnectionPoint.Advise to hook up a native COM event sink
                            // to a managed .NET event interface.
                            return cp.Advise(punkEventsSink.DangerousGetHandle(), out cookie);
                        }
                    }
                }

                /// <summary>
                ///  Wraps a native IUnknown in a SafeHandle.
                ///  See similar implementaton in the <see cref='Transactions.SafeIUnknown'/> class.
                /// </summary>
                internal class SafeIUnknown : SafeHandle
                {
                    /// <summary>
                    ///  Wrap an incomoing unknown or get the unknown for the CCW (COM-callable wrapper).
                    /// </summary>
                    public SafeIUnknown(object obj, bool addRefIntPtr)
                        : this(obj, addRefIntPtr, Guid.Empty)
                    {
                    }

                    /// <summary>
                    ///  Wrap an incomoing unknown or get the unknown for the CCW (COM-callable wrapper).
                    ///  If an iid is supplied, QI for the interface and wrap that unknonwn instead.
                    /// </summary>
                    public SafeIUnknown(object obj, bool addRefIntPtr, Guid iid)
                        : base(IntPtr.Zero, true)
                    {
                        RuntimeHelpers.PrepareConstrainedRegions();
                        try
                        {
                            // Set this.handle in a finally block to ensure the com ptr is set in the SafeHandle
                            // even if the runtime throws a exception (such as ThreadAbortException) during the call.
                            // This ensures that the finalizer will clean up the COM reference.
                        }
                        finally
                        {
                            // Get a raw IUnknown for this object.
                            // We are responsible for releasing the IUnknown ourselves.
                            IntPtr unknown;

                            if (obj is IntPtr)
                            {
                                unknown = (IntPtr)obj;

                                // The incoming IntPtr may already be reference counted or not, depending on
                                // where it came from. The caller needs to tell us whether to add-ref or not.
                                if (addRefIntPtr)
                                {
                                    Marshal.AddRef(unknown);
                                }
                            }
                            else
                            {
                                // GetIUnknownForObject will return a reference-counted object
                                unknown = Marshal.GetIUnknownForObject(obj);
                            }

                            // Attempt QueryInterface if an iid is specified.
                            if (iid != Guid.Empty)
                            {
                                IntPtr oldUnknown = unknown;
                                try
                                {
                                    unknown = InternalQueryInterface(unknown, ref iid);
                                }
                                finally
                                {
                                    // It is critical to release the original unknown if
                                    // InternalQueryInterface throws out so we don't leak ref counts.
                                    Marshal.Release(oldUnknown);
                                }
                            }

                            // Preserve the com ptr in the SafeHandle.
                            handle = unknown;
                        }
                    }

                    /// <summary>
                    ///  Helper function for QueryInterface.
                    /// </summary>
                    private static IntPtr InternalQueryInterface(IntPtr pUnk, ref Guid iid)
                    {
                        int hresult = Marshal.QueryInterface(pUnk, ref iid, out IntPtr ppv);
                        if (hresult != 0 || ppv == IntPtr.Zero)
                        {
                            throw new InvalidCastException(SR.AxInterfaceNotSupported);
                        }
                        return ppv;
                    }

                    /// <summary>
                    ///  Return whether the handle is invalid.
                    /// </summary>
                    public sealed override bool IsInvalid
                    {
                        get
                        {
                            if (!IsClosed)
                            {
                                return (IntPtr.Zero == handle);
                            }
                            return true;
                        }
                    }

                    /// <summary>
                    ///  Release the IUnknown.
                    /// </summary>
                    protected sealed override bool ReleaseHandle()
                    {
                        IntPtr ptr1 = handle;
                        handle = IntPtr.Zero;
                        if (IntPtr.Zero != ptr1)
                        {
                            Marshal.Release(ptr1);
                        }
                        return true;
                    }

                    /// <summary>
                    ///  Helper function to load a COM v-table from a com object pointer.
                    /// </summary>
                    protected V LoadVtable<V>()
                    {
                        IntPtr vtblptr = Marshal.ReadIntPtr(handle, 0);
                        return Marshal.PtrToStructure<V>(vtblptr);
                    }
                }

                /// <summary>
                ///  Helper class to access IConnectionPointContainer from a .NET COM-callable wrapper.
                ///  The IConnectionPointContainer COM pointer is wrapped in a SafeHandle.
                /// </summary>
                internal sealed class ComConnectionPointContainer
                    : SafeIUnknown
                {
                    public ComConnectionPointContainer(object obj, bool addRefIntPtr)
                        : base(obj, addRefIntPtr, typeof(IConnectionPointContainer).GUID)
                    {
                        _vtbl = LoadVtable<VTABLE>();
                    }

                    private readonly VTABLE _vtbl;

                    [StructLayout(LayoutKind.Sequential)]
                    private class VTABLE
                    {
                        public IntPtr QueryInterfacePtr;
                        public IntPtr AddRefPtr;
                        public IntPtr ReleasePtr;
                        public IntPtr EnumConnectionPointsPtr;
                        public IntPtr FindConnectionPointPtr;
                    }

                    /// <summary>
                    ///  Call IConnectionPointContainer.FindConnectionPoint using Delegate.Invoke on the v-table slot.
                    /// </summary>
                    public ComConnectionPoint FindConnectionPoint(Type eventInterface)
                    {
                        FindConnectionPointD findConnectionPoint = (FindConnectionPointD)Marshal.GetDelegateForFunctionPointer(_vtbl.FindConnectionPointPtr, typeof(FindConnectionPointD));

                        Guid iid = eventInterface.GUID;
                        int hresult = findConnectionPoint.Invoke(handle, ref iid, out IntPtr result);
                        if (hresult != 0 || result == IntPtr.Zero)
                        {
                            throw new ArgumentException(string.Format(SR.AXNoConnectionPoint, eventInterface.Name));
                        }

                        return new ComConnectionPoint(result, false);   // result is already ref-counted as an out-param so pass in false
                    }

                    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                    private delegate int FindConnectionPointD(IntPtr This, ref Guid iid, out IntPtr ppv);
                }

                /// <summary>
                ///  Helper class to access IConnectionPoint from a .NET COM-callable wrapper.
                ///  The IConnectionPoint COM pointer is wrapped in a SafeHandle.
                /// </summary>
                internal sealed class ComConnectionPoint
                    : SafeIUnknown
                {
                    public ComConnectionPoint(object obj, bool addRefIntPtr)
                        : base(obj, addRefIntPtr, typeof(IConnectionPoint).GUID)
                    {
                        _vtbl = LoadVtable<VTABLE>();
                    }

                    [StructLayout(LayoutKind.Sequential)]
                    private class VTABLE
                    {
                        public IntPtr QueryInterfacePtr;
                        public IntPtr AddRefPtr;
                        public IntPtr ReleasePtr;
                        public IntPtr GetConnectionInterfacePtr;
                        public IntPtr GetConnectionPointContainterPtr;
                        public IntPtr AdvisePtr;
                        public IntPtr UnadvisePtr;
                        public IntPtr EnumConnectionsPtr;
                    }

                    private readonly VTABLE _vtbl;

                    /// <summary>
                    ///  Call IConnectioinPoint.Advise using Delegate.Invoke on the v-table slot.
                    /// </summary>
                    public bool Advise(IntPtr punkEventSink, out int cookie)
                    {
                        AdviseD advise = (AdviseD)Marshal.GetDelegateForFunctionPointer(_vtbl.AdvisePtr, typeof(AdviseD));
                        if (advise.Invoke(handle, punkEventSink, out cookie) == 0)
                        {
                            return true;
                        }
                        return false;
                    }

                    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                    private delegate int AdviseD(IntPtr This, IntPtr punkEventSink, out int cookie);
                }

            }

            /// <summary>
            ///  Return the default COM events interface declared on a .NET class.
            ///  This looks for the ComSourceInterfacesAttribute and returns the .NET
            ///  interface type of the first interface declared.
            /// </summary>
            private static Type GetDefaultEventsInterface(Type controlType)
            {
                Type eventInterface = null;
                object[] custom = controlType.GetCustomAttributes(typeof(ComSourceInterfacesAttribute), false);

                if (custom.Length > 0)
                {
                    ComSourceInterfacesAttribute coms = (ComSourceInterfacesAttribute)custom[0];
                    string eventName = coms.Value.Split(new char[] { '\0' })[0];
                    eventInterface = controlType.Module.Assembly.GetType(eventName, false);
                    if (eventInterface == null)
                    {
                        eventInterface = Type.GetType(eventName, false);
                    }
                }

                return eventInterface;
            }

            /// <summary>
            ///  Implements IPersistStorage::Save
            /// </summary>
            internal void Save(Ole32.IStorage stg, BOOL fSameAsLoad)
            {
                Ole32.IStream stream = stg.CreateStream(
                    GetStreamName(),
                    Ole32.STGM.STGM_WRITE | Ole32.STGM.STGM_SHARE_EXCLUSIVE | Ole32.STGM.STGM_CREATE,
                    0,
                    0);
                Debug.Assert(stream != null, "Stream should be non-null, or an exception should have been thrown.");

                Save(stream, BOOL.TRUE);
                Marshal.ReleaseComObject(stream);
            }

            /// <summary>
            ///  Implements IPersistStreamInit::Save
            /// </summary>
            internal void Save(Ole32.IStream stream, BOOL fClearDirty)
            {
                // We do everything through property bags because we support full fidelity
                // in them.  So, save through that method.
                PropertyBagStream bag = new PropertyBagStream();
                Save(bag, fClearDirty, BOOL.FALSE);
                bag.Write(stream);

                if (Marshal.IsComObject(stream))
                {
                    Marshal.ReleaseComObject(stream);
                }
            }

            /// <summary>
            ///  Implements IPersistPropertyBag::Save
            /// </summary>
            internal void Save(UnsafeNativeMethods.IPropertyBag pPropBag, BOOL fClearDirty, BOOL fSaveAllProperties)
            {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(_control,
                    new Attribute[] { DesignerSerializationVisibilityAttribute.Visible });

                for (int i = 0; i < props.Count; i++)
                {
                    if (fSaveAllProperties != BOOL.FALSE || props[i].ShouldSerializeValue(_control))
                    {
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Saving property " + props[i].Name);

                        object propValue;

                        if (IsResourceProp(props[i]))
                        {
                            // Resource property.  Save this to the bag as a 64bit encoded string.
                            MemoryStream stream = new MemoryStream();
                            BinaryFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(stream, props[i].GetValue(_control));
                            byte[] bytes = new byte[(int)stream.Length];
                            stream.Position = 0;
                            stream.Read(bytes, 0, bytes.Length);
                            propValue = Convert.ToBase64String(bytes);
                            pPropBag.Write(props[i].Name, ref propValue);
                        }
                        else
                        {
                            // Not a resource property.  Persist this using standard type converters.
                            TypeConverter converter = props[i].Converter;
                            Debug.Assert(converter != null, "No type converter for property '" + props[i].Name + "' on class " + _control.GetType().FullName);

                            if (converter.CanConvertFrom(typeof(string)))
                            {
                                propValue = converter.ConvertToInvariantString(props[i].GetValue(_control));
                                pPropBag.Write(props[i].Name, ref propValue);
                            }
                            else if (converter.CanConvertFrom(typeof(byte[])))
                            {
                                byte[] data = (byte[])converter.ConvertTo(null, CultureInfo.InvariantCulture, props[i].GetValue(_control), typeof(byte[]));
                                propValue = Convert.ToBase64String(data);
                                pPropBag.Write(props[i].Name, ref propValue);
                            }
                        }
                    }
                }

                if (Marshal.IsComObject(pPropBag))
                {
                    Marshal.ReleaseComObject(pPropBag);
                }

                if (fClearDirty != BOOL.FALSE)
                {
                    _activeXState[s_isDirty] = false;
                }
            }

            /// <summary>
            ///  Fires the OnSave event to all of our IAdviseSink
            ///  listeners.  Used for ActiveXSourcing.
            /// </summary>
            private void SendOnSave()
            {
                int cnt = _adviseList.Count;
                for (int i = 0; i < cnt; i++)
                {
                    IAdviseSink s = (IAdviseSink)_adviseList[i];
                    Debug.Assert(s != null, "NULL in our advise list");
                    s.OnSave();
                }
            }

            /// <summary>
            ///  Implements IViewObject2::SetAdvise.
            /// </summary>
            internal void SetAdvise(int aspects, int advf, IAdviseSink pAdvSink)
            {
                // if it's not a content aspect, we don't support it.
                if ((aspects & NativeMethods.DVASPECT_CONTENT) == 0)
                {
                    ThrowHr(NativeMethods.DV_E_DVASPECT);
                }

                // set up some flags  [we gotta stash for GetAdvise ...]
                _activeXState[s_viewAdvisePrimeFirst] = (advf & NativeMethods.ADVF_PRIMEFIRST) != 0 ? true : false;
                _activeXState[s_viewAdviseOnlyOnce] = (advf & NativeMethods.ADVF_ONLYONCE) != 0 ? true : false;

                if (_viewAdviseSink != null && Marshal.IsComObject(_viewAdviseSink))
                {
                    Marshal.ReleaseComObject(_viewAdviseSink);
                }

                _viewAdviseSink = pAdvSink;

                // prime them if they want it [we need to store this so they can get flags later]
                if (_activeXState[s_viewAdvisePrimeFirst])
                {
                    ViewChanged();
                }
            }

            /// <summary>
            ///  Implements IOleObject::SetClientSite.
            /// </summary>
            internal void SetClientSite(UnsafeNativeMethods.IOleClientSite value)
            {
                if (_clientSite != null)
                {
                    if (value == null)
                    {
                        s_globalActiveXCount--;

                        if (s_globalActiveXCount == 0 && IsIE)
                        {
                            // This the last ActiveX control and we are
                            // being hosted in IE.  Use private reflection
                            // to ask SystemEvents to shutdown.  This is to
                            // prevent a crash.

                            MethodInfo method = typeof(SystemEvents).GetMethod("Shutdown",
                                                                                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
                                                                                null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
                            Debug.Assert(method != null, "No Shutdown method on SystemEvents");
                            if (method != null)
                            {
                                method.Invoke(null, null);
                            }
                        }
                    }

                    if (Marshal.IsComObject(_clientSite))
                    {
                        Marshal.FinalReleaseComObject(_clientSite);
                    }
                }

                _clientSite = value;

                if (_clientSite != null)
                {
                    _control.Site = new AxSourcingSite(_control, _clientSite, "ControlAxSourcingSite");
                }
                else
                {
                    _control.Site = null;
                }

                // Get the ambient properties that effect us...
                object obj = new object();
                if (GetAmbientProperty(NativeMethods.ActiveX.DISPID_AMBIENT_UIDEAD, ref obj))
                {
                    _activeXState[s_uiDead] = (bool)obj;
                }

                if (_control is IButtonControl && GetAmbientProperty(NativeMethods.ActiveX.DISPID_AMBIENT_UIDEAD, ref obj))
                {
                    ((IButtonControl)_control).NotifyDefault((bool)obj);
                }

                if (_clientSite == null)
                {
                    if (_accelTable != IntPtr.Zero)
                    {
                        UnsafeNativeMethods.DestroyAcceleratorTable(new HandleRef(this, _accelTable));
                        _accelTable = IntPtr.Zero;
                        _accelCount = -1;
                    }

                    if (IsIE)
                    {
                        _control.Dispose();
                    }
                }
                else
                {
                    s_globalActiveXCount++;

                    if (s_globalActiveXCount == 1 && IsIE)
                    {
                        // This the first ActiveX control and we are
                        // being hosted in IE.  Use private reflection
                        // to ask SystemEvents to start.  Startup will only
                        // restart system events if we previously shut it down.
                        // This is to prevent a crash.
                        MethodInfo method = typeof(SystemEvents).GetMethod("Startup",
                                                                            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
                                                                            null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
                        Debug.Assert(method != null, "No Startup method on SystemEvents");
                        if (method != null)
                        {
                            method.Invoke(null, null);
                        }
                    }
                }
                _control.OnTopMostActiveXParentChanged(EventArgs.Empty);
            }

            /// <summary>
            ///  Implements IOleObject::SetExtent
            /// </summary>
            internal unsafe void SetExtent(uint dwDrawAspect, Size* pSizel)
            {
                if ((dwDrawAspect & NativeMethods.DVASPECT_CONTENT) != 0)
                {
                    if (_activeXState[s_changingExtents])
                    {
                        return;
                    }

                    _activeXState[s_changingExtents] = true;

                    try
                    {
                        Size size = new Size(HiMetricToPixel(pSizel->Width, pSizel->Height));
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "SetExtent : new size:" + size.ToString());

                        // If we're in place active, let the in place site set our bounds.
                        // Otherwise, just set it on our control directly.
                        if (_activeXState[s_inPlaceActive])
                        {
                            if (_clientSite is UnsafeNativeMethods.IOleInPlaceSite ioleClientSite)
                            {
                                Rectangle bounds = _control.Bounds;
                                bounds.Location = new Point(bounds.X, bounds.Y);
                                Size adjusted = new Size(size.Width, size.Height);
                                bounds.Width = adjusted.Width;
                                bounds.Height = adjusted.Height;
                                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "SetExtent : Announcing to in place site that our rect has changed.");
                                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "            Announcing rect = " + bounds);
                                Debug.Assert(_clientSite != null, "How can we setextent before we are sited??");

                                ioleClientSite.OnPosRectChange(NativeMethods.COMRECT.FromXYWH(bounds.X, bounds.Y, bounds.Width, bounds.Height));
                            }
                        }

                        _control.Size = size;

                        // Check to see if the control overwrote our size with
                        // its own values.
                        if (!_control.Size.Equals(size))
                        {
                            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "SetExtent : Control has changed size.  Setting dirty bit");
                            _activeXState[s_isDirty] = true;

                            // If we're not inplace active, then anounce that the view changed.
                            if (!_activeXState[s_inPlaceActive])
                            {
                                ViewChanged();
                            }

                            // We need to call RequestNewObjectLayout
                            // here so we visually display our new extents.
                            if (!_activeXState[s_inPlaceActive] && _clientSite != null)
                            {
                                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "SetExtent : Requesting new Object layout.");
                                _clientSite.RequestNewObjectLayout();
                            }
                        }
                    }
                    finally
                    {
                        _activeXState[s_changingExtents] = false;
                    }
                }
                else
                {
                    // We don't support any other aspects
                    ThrowHr(NativeMethods.DV_E_DVASPECT);
                }
            }

            /// <summary>
            ///  Marks our state as in place visible.
            /// </summary>
            private void SetInPlaceVisible(bool visible)
            {
                _activeXState[s_inPlaceVisible] = visible;
                _control.Visible = visible;
            }

            /// <summary>
            ///  Implements IOleInPlaceObject::SetObjectRects.
            /// </summary>
            internal void SetObjectRects(NativeMethods.COMRECT lprcPosRect, NativeMethods.COMRECT lprcClipRect)
            {
#if DEBUG
                if (CompModSwitches.ActiveX.TraceInfo)
                {
                    Debug.WriteLine("SetObjectRects:");
                    Debug.Indent();

                    if (lprcPosRect != null)
                    {
                        Debug.WriteLine("PosLeft:    " + lprcPosRect.left.ToString(CultureInfo.InvariantCulture));
                        Debug.WriteLine("PosTop:     " + lprcPosRect.top.ToString(CultureInfo.InvariantCulture));
                        Debug.WriteLine("PosRight:   " + lprcPosRect.right.ToString(CultureInfo.InvariantCulture));
                        Debug.WriteLine("PosBottom:  " + lprcPosRect.bottom.ToString(CultureInfo.InvariantCulture));
                    }

                    if (lprcClipRect != null)
                    {
                        Debug.WriteLine("ClipLeft:   " + lprcClipRect.left.ToString(CultureInfo.InvariantCulture));
                        Debug.WriteLine("ClipTop:    " + lprcClipRect.top.ToString(CultureInfo.InvariantCulture));
                        Debug.WriteLine("ClipRight:  " + lprcClipRect.right.ToString(CultureInfo.InvariantCulture));
                        Debug.WriteLine("ClipBottom: " + lprcClipRect.bottom.ToString(CultureInfo.InvariantCulture));
                    }
                    Debug.Unindent();
                }
#endif

                Rectangle posRect = Rectangle.FromLTRB(lprcPosRect.left, lprcPosRect.top, lprcPosRect.right, lprcPosRect.bottom);

                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Set control bounds: " + posRect.ToString());

                // ActiveX expects to be notified when a control's bounds change, and also
                // intends to notify us through SetObjectRects when we report that the
                // bounds are about to change.  We implement this all on a control's Bounds
                // property, which doesn't use this callback mechanism.  The adjustRect
                // member handles this. If it is non-null, then we are being called in
                // response to an OnPosRectChange call.  In this case we do not
                // set the control bounds but set the bounds on the adjustRect.  When
                // this returns from the container and comes back to our OnPosRectChange
                // implementation, these new bounds will be handed back to the control
                // for the actual window change.
                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Old Control Bounds: " + _control.Bounds);
                if (_activeXState[s_adjustingRect])
                {
                    _adjustRect.left = posRect.X;
                    _adjustRect.top = posRect.Y;
                    _adjustRect.right = posRect.Width + posRect.X;
                    _adjustRect.bottom = posRect.Height + posRect.Y;
                }
                else
                {
                    _activeXState[s_adjustingRect] = true;
                    try
                    {
                        _control.Bounds = posRect;
                    }
                    finally
                    {
                        _activeXState[s_adjustingRect] = false;
                    }
                }

                bool setRegion = false;

                if (_clipRegion != IntPtr.Zero)
                {
                    // Bad -- after calling SetWindowReg, windows owns the region.
                    //SafeNativeMethods.DeleteObject(clipRegion);
                    _clipRegion = IntPtr.Zero;
                    setRegion = true;
                }

                if (lprcClipRect != null)
                {
                    // The container wants us to clip, so figure out if we really
                    // need to.
                    Rectangle clipRect = Rectangle.FromLTRB(lprcClipRect.left, lprcClipRect.top, lprcClipRect.right, lprcClipRect.bottom);

                    Rectangle intersect;

                    // Trident always sends an empty ClipRect... and so, we check for that and not do an
                    // intersect in that case.
                    if (!clipRect.IsEmpty)
                    {
                        intersect = Rectangle.Intersect(posRect, clipRect);
                    }
                    else
                    {
                        intersect = posRect;
                    }

                    if (!intersect.Equals(posRect))
                    {
                        // Offset the rectangle back to client coordinates
                        RECT rcIntersect = intersect;
                        IntPtr hWndParent = UnsafeNativeMethods.GetParent(new HandleRef(_control, _control.Handle));

                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Old Intersect: " + new Rectangle(rcIntersect.left, rcIntersect.top, rcIntersect.right - rcIntersect.left, rcIntersect.bottom - rcIntersect.top));
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "New Control Bounds: " + posRect);

                        UnsafeNativeMethods.MapWindowPoints(new HandleRef(null, hWndParent), new HandleRef(_control, _control.Handle), ref rcIntersect, 2);

                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "New Intersect: " + new Rectangle(rcIntersect.left, rcIntersect.top, rcIntersect.right - rcIntersect.left, rcIntersect.bottom - rcIntersect.top));

                        // Create a Win32 region for it
                        _clipRegion = Gdi32.CreateRectRgn(rcIntersect.left, rcIntersect.top,
                                                                 rcIntersect.right, rcIntersect.bottom);
                        setRegion = true;
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Created clipping region");
                    }
                }

                // If our region has changed, set the new value.  We only do this if
                // the handle has been created, since otherwise the control will
                // merge our region automatically.
                if (setRegion && _control.IsHandleCreated)
                {
                    IntPtr finalClipRegion = _clipRegion;

                    Region controlRegion = _control.Region;
                    if (controlRegion != null)
                    {
                        IntPtr rgn = _control.GetHRgn(controlRegion);
                        finalClipRegion = MergeRegion(rgn);
                    }

                    UnsafeNativeMethods.SetWindowRgn(new HandleRef(_control, _control.Handle), new HandleRef(this, finalClipRegion), SafeNativeMethods.IsWindowVisible(new HandleRef(_control, _control.Handle)));
                }

                // Yuck.  Forms^3 uses transparent overlay windows that appear to cause
                // painting artifacts.  Flicker like a banshee.
                _control.Invalidate();
            }

            /// <summary>
            ///  Throws the given hresult.  This is used by ActiveX sourcing.
            /// </summary>
            internal static void ThrowHr(int hr)
            {
                ExternalException e = new ExternalException(SR.ExternalException, hr);
                throw e;
            }

            /// <summary>
            ///  Handles IOleControl::TranslateAccelerator
            /// </summary>
            internal int TranslateAccelerator(ref NativeMethods.MSG lpmsg)
            {
#if DEBUG
                if (CompModSwitches.ActiveX.TraceInfo)
                {
                    if (!_control.IsHandleCreated)
                    {
                        Debug.WriteLine("AxSource: TranslateAccelerator before handle creation");
                    }
                    else
                    {
                        Message m = Message.Create(lpmsg.hwnd, lpmsg.message, lpmsg.wParam, lpmsg.lParam);
                        Debug.WriteLine("AxSource: TranslateAccelerator : " + m.ToString());
                    }
                }
#endif // DEBUG

                bool needPreProcess = false;

                switch (lpmsg.message)
                {
                    case WindowMessages.WM_KEYDOWN:
                    case WindowMessages.WM_SYSKEYDOWN:
                    case WindowMessages.WM_CHAR:
                    case WindowMessages.WM_SYSCHAR:
                        needPreProcess = true;
                        break;
                }

                Message msg = Message.Create(lpmsg.hwnd, lpmsg.message, lpmsg.wParam, lpmsg.lParam);

                if (needPreProcess)
                {
                    Control target = FromChildHandle(lpmsg.hwnd);
                    if (target != null && (_control == target || _control.Contains(target)))
                    {
                        PreProcessControlState messageState = PreProcessControlMessageInternal(target, ref msg);
                        switch (messageState)
                        {
                            case PreProcessControlState.MessageProcessed:
                                // someone returned true from PreProcessMessage
                                // no need to dispatch the message, its already been coped with.
                                lpmsg.message = msg.Msg;
                                lpmsg.wParam = msg.WParam;
                                lpmsg.lParam = msg.LParam;
                                return NativeMethods.S_OK;
                            case PreProcessControlState.MessageNeeded:
                                // Here we need to dispatch the message ourselves
                                // otherwise the host may never send the key to our wndproc.

                                // Someone returned true from IsInputKey or IsInputChar
                                UnsafeNativeMethods.TranslateMessage(ref lpmsg);
                                if (SafeNativeMethods.IsWindowUnicode(new HandleRef(null, lpmsg.hwnd)))
                                {
                                    UnsafeNativeMethods.DispatchMessageW(ref lpmsg);
                                }
                                else
                                {
                                    UnsafeNativeMethods.DispatchMessageA(ref lpmsg);
                                }
                                return NativeMethods.S_OK;
                            case PreProcessControlState.MessageNotNeeded:
                                // in this case we'll check the site to see if it wants the message.
                                break;
                        }
                    }
                }

                // SITE processing.  We're not interested in the message, but the site may be.

                int hr = NativeMethods.S_FALSE;

                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource: Control did not process accelerator, handing to site");

                if (_clientSite is UnsafeNativeMethods.IOleControlSite ioleClientSite)
                {
                    int keyState = 0;

                    if (UnsafeNativeMethods.GetKeyState(NativeMethods.VK_SHIFT) < 0)
                    {
                        keyState |= 1;
                    }

                    if (UnsafeNativeMethods.GetKeyState(NativeMethods.VK_CONTROL) < 0)
                    {
                        keyState |= 2;
                    }

                    if (UnsafeNativeMethods.GetKeyState(NativeMethods.VK_MENU) < 0)
                    {
                        keyState |= 4;
                    }

                    hr = ioleClientSite.TranslateAccelerator(ref lpmsg, keyState);
                }

                return hr;
            }

            /// <summary>
            ///  Implements IOleInPlaceObject::UIDeactivate.
            /// </summary>
            internal int UIDeactivate()
            {
                // Only do this if we're UI active
                if (!_activeXState[s_uiActive])
                {
                    return NativeMethods.S_OK;
                }

                _activeXState[s_uiActive] = false;

                // Notify frame windows, if appropriate, that we're no longer ui-active.
                if (_inPlaceUiWindow != null)
                {
                    _inPlaceUiWindow.SetActiveObject(null, null);
                }

                // May need this for SetActiveObject & OnUIDeactivate, so leave until function return
                Debug.Assert(_inPlaceFrame != null, "No inplace frame -- how dod we go UI active?");
                _inPlaceFrame.SetActiveObject(null, null);

                if (_clientSite is UnsafeNativeMethods.IOleInPlaceSite ioleClientSite)
                {
                    ioleClientSite.OnUIDeactivate(0);
                }

                return NativeMethods.S_OK;
            }

            /// <summary>
            ///  Implements IOleObject::Unadvise
            /// </summary>
            internal void Unadvise(int dwConnection)
            {
                if (dwConnection > _adviseList.Count || _adviseList[dwConnection - 1] == null)
                {
                    ThrowHr(NativeMethods.OLE_E_NOCONNECTION);
                }

                IAdviseSink sink = (IAdviseSink)_adviseList[dwConnection - 1];
                _adviseList.RemoveAt(dwConnection - 1);
                if (sink != null && Marshal.IsComObject(sink))
                {
                    Marshal.ReleaseComObject(sink);
                }
            }

            /// <summary>
            ///  Notifies our site that we have changed our size and location.
            /// </summary>
            internal void UpdateBounds(ref int x, ref int y, ref int width, ref int height, int flags)
            {
                if (!_activeXState[s_adjustingRect] && _activeXState[s_inPlaceVisible])
                {
                    if (_clientSite is UnsafeNativeMethods.IOleInPlaceSite ioleClientSite)
                    {
                        NativeMethods.COMRECT rc = new NativeMethods.COMRECT();

                        if ((flags & NativeMethods.SWP_NOMOVE) != 0)
                        {
                            rc.left = _control.Left;
                            rc.top = _control.Top;
                        }
                        else
                        {
                            rc.left = x;
                            rc.top = y;
                        }

                        if ((flags & NativeMethods.SWP_NOSIZE) != 0)
                        {
                            rc.right = rc.left + _control.Width;
                            rc.bottom = rc.top + _control.Height;
                        }
                        else
                        {
                            rc.right = rc.left + width;
                            rc.bottom = rc.top + height;
                        }

                        // This member variable may be modified by SetObjectRects by the container.
                        _adjustRect = rc;
                        _activeXState[s_adjustingRect] = true;

                        try
                        {
                            ioleClientSite.OnPosRectChange(rc);
                        }
                        finally
                        {
                            _adjustRect = null;
                            _activeXState[s_adjustingRect] = false;
                        }

                        // On output, the new bounds will be reflected in  rc
                        if ((flags & NativeMethods.SWP_NOMOVE) == 0)
                        {
                            x = rc.left;
                            y = rc.top;
                        }
                        if ((flags & NativeMethods.SWP_NOSIZE) == 0)
                        {
                            width = rc.right - rc.left;
                            height = rc.bottom - rc.top;
                        }
                    }
                }
            }

            /// <summary>
            ///  Notifies that the accelerator table needs to be updated due to a change in a control mnemonic.
            /// </summary>
            internal void UpdateAccelTable()
            {
                // Setting the count to -1 will recreate the table on demand (when GetControlInfo is called).
                _accelCount = -1;

                if (_clientSite is UnsafeNativeMethods.IOleControlSite ioleClientSite)
                {
                    ioleClientSite.OnControlInfoChanged();
                }
            }

            // Since this method is used by Reflection .. dont change the "signature"
            internal void ViewChangedInternal()
            {
                ViewChanged();
            }

            /// <summary>
            ///  Notifies our view advise sink (if it exists) that the view has
            ///  changed.
            /// </summary>
            private void ViewChanged()
            {
                // send the view change notification to anybody listening.
                //
                // Note: Word2000 won't resize components correctly if an OnViewChange notification
                //       is sent while the component is persisting it's state.  The !m_fSaving check
                //       is to make sure we don't call OnViewChange in this case.
                if (_viewAdviseSink != null && !_activeXState[s_saving])
                {
                    _viewAdviseSink.OnViewChange(NativeMethods.DVASPECT_CONTENT, -1);

                    if (_activeXState[s_viewAdviseOnlyOnce])
                    {
                        if (Marshal.IsComObject(_viewAdviseSink))
                        {
                            Marshal.ReleaseComObject(_viewAdviseSink);
                        }
                        _viewAdviseSink = null;
                    }
                }
            }

            /// <summary>
            ///  Called when the window handle of the control has changed.
            /// </summary>
            void IWindowTarget.OnHandleChange(IntPtr newHandle)
            {
                _controlWindowTarget.OnHandleChange(newHandle);
            }

            /// <summary>
            ///  Called to do control-specific processing for this window.
            /// </summary>
            void IWindowTarget.OnMessage(ref Message m)
            {
                if (_activeXState[s_uiDead])
                {
                    if (m.Msg >= WindowMessages.WM_MOUSEFIRST && m.Msg <= WindowMessages.WM_MOUSELAST)
                    {
                        return;
                    }
                    if (m.Msg >= WindowMessages.WM_NCLBUTTONDOWN && m.Msg <= WindowMessages.WM_NCMBUTTONDBLCLK)
                    {
                        return;
                    }
                    if (m.Msg >= WindowMessages.WM_KEYFIRST && m.Msg <= WindowMessages.WM_KEYLAST)
                    {
                        return;
                    }
                }

                _controlWindowTarget.OnMessage(ref m);
            }

            /// <summary>
            ///  This is a property bag implementation that sits on a stream.  It can
            ///  read and write the bag to the stream.
            /// </summary>
            private class PropertyBagStream : UnsafeNativeMethods.IPropertyBag
            {
                private Hashtable _bag = new Hashtable();

                internal void Read(Ole32.IStream istream)
                {
                    // visual basic's memory streams don't support seeking, so we have to
                    // work around this limitation here.  We do this by copying
                    // the contents of the stream into a MemoryStream object.
                    Stream stream = new DataStreamFromComStream(istream);
                    const int PAGE_SIZE = 0x1000; // one page (4096b)
                    byte[] streamData = new byte[PAGE_SIZE];
                    int offset = 0;

                    int count = stream.Read(streamData, offset, PAGE_SIZE);
                    int totalCount = count;

                    while (count == PAGE_SIZE)
                    {
                        byte[] newChunk = new byte[streamData.Length + PAGE_SIZE];
                        Array.Copy(streamData, newChunk, streamData.Length);
                        streamData = newChunk;

                        offset += PAGE_SIZE;
                        count = stream.Read(streamData, offset, PAGE_SIZE);
                        totalCount += count;
                    }

                    stream = new MemoryStream(streamData);

                    BinaryFormatter formatter = new BinaryFormatter();
                    try
                    {
                        _bag = (Hashtable)formatter.Deserialize(stream);
                    }
                    catch (Exception e)
                    {
                        if (ClientUtils.IsSecurityOrCriticalException(e))
                        {
                            throw;
                        }

                        // Error reading.  Just init an empty hashtable.
                        _bag = new Hashtable();
                    }
                }

                int UnsafeNativeMethods.IPropertyBag.Read(string pszPropName, ref object pVar, UnsafeNativeMethods.IErrorLog pErrorLog)
                {
                    if (!_bag.Contains(pszPropName))
                    {
                        return NativeMethods.E_INVALIDARG;
                    }

                    pVar = _bag[pszPropName];
                    return NativeMethods.S_OK;
                }

                int UnsafeNativeMethods.IPropertyBag.Write(string pszPropName, ref object pVar)
                {
                    _bag[pszPropName] = pVar;
                    return NativeMethods.S_OK;
                }

                internal void Write(Ole32.IStream istream)
                {
                    Stream stream = new DataStreamFromComStream(istream);
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, _bag);
                }
            }
        }
    }
}

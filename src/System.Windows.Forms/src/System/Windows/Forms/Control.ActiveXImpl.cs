// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using static Interop;
using static Interop.Ole32;
using IAdviseSink = System.Runtime.InteropServices.ComTypes.IAdviseSink;

namespace System.Windows.Forms
{
    public partial class Control
    {
        /// <summary>
        ///  This class holds all of the state data for an ActiveX control and
        ///  supplies the implementation for many of the non-trivial methods.
        /// </summary>
        private unsafe class ActiveXImpl : MarshalByRefObject, IWindowTarget
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
            private static OLEVERB[] s_axVerbs;

            private readonly Control _control;
            private readonly IWindowTarget _controlWindowTarget;
            private Rectangle? _lastClipRect;

            private IOleClientSite _clientSite;
            private IOleInPlaceUIWindow _inPlaceUiWindow;
            private IOleInPlaceFrame _inPlaceFrame;
            private readonly ArrayList _adviseList;
            private IAdviseSink _viewAdviseSink;
            private BitVector32 _activeXState;
            private readonly AmbientProperty[] _ambientProperties;
            private IntPtr _accelTable;
            private short _accelCount = -1;
            private RECT* _adjustRect; // temporary rect used during OnPosRectChange && SetObjectRects

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
                    new AmbientProperty("Font", DispatchID.AMBIENT_FONT),
                    new AmbientProperty("BackColor", DispatchID.AMBIENT_BACKCOLOR),
                    new AmbientProperty("ForeColor", DispatchID.AMBIENT_FORECOLOR)
                };
            }

            /// <summary>
            ///  Retrieves the ambient back color for the control.
            /// </summary>
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Advanced)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            internal Color AmbientBackColor
            {
                get
                {
                    AmbientProperty prop = LookupAmbient(DispatchID.AMBIENT_BACKCOLOR);

                    if (prop.Empty)
                    {
                        object obj = null;
                        if (GetAmbientProperty(DispatchID.AMBIENT_BACKCOLOR, ref obj))
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

                                    if (ClientUtils.IsCriticalException(e))
                                    {
                                        throw;
                                    }
                                }
                            }
                        }
                    }

                    if (prop.Value is null)
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
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Advanced)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            internal Font AmbientFont
            {
                get
                {
                    AmbientProperty prop = LookupAmbient(DispatchID.AMBIENT_FONT);

                    if (prop.Empty)
                    {
                        object obj = null;
                        if (GetAmbientProperty(DispatchID.AMBIENT_FONT, ref obj))
                        {
                            try
                            {
                                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Object font type=" + obj.GetType().FullName);
                                Debug.Assert(obj != null, "GetAmbientProperty failed");
                                IFont ifont = (IFont)obj;
                                prop.Value = Font.FromHfont(ifont.hFont);
                            }
                            catch (Exception e) when (!ClientUtils.IsCriticalException(e))
                            {
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
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Advanced)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            internal Color AmbientForeColor
            {
                get
                {
                    AmbientProperty prop = LookupAmbient(DispatchID.AMBIENT_FORECOLOR);

                    if (prop.Empty)
                    {
                        object obj = null;
                        if (GetAmbientProperty(DispatchID.AMBIENT_FORECOLOR, ref obj))
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

                                    if (ClientUtils.IsCriticalException(e))
                                    {
                                        throw;
                                    }
                                }
                            }
                        }
                    }

                    if (prop.Value is null)
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
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Advanced)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
                        using var dc = User32.GetDcScope.ScreenDC;
                        s_logPixels.X = Gdi32.GetDeviceCaps(dc, Gdi32.DeviceCapability.LOGPIXELSX);
                        s_logPixels.Y = Gdi32.GetDeviceCaps(dc, Gdi32.DeviceCapability.LOGPIXELSY);
                    }
                    return s_logPixels;
                }
            }

            /// <summary>
            ///  Implements IOleObject::Advise
            /// </summary>
            internal uint Advise(IAdviseSink pAdvSink)
            {
                _adviseList.Add(pAdvSink);
                return (uint)_adviseList.Count;
            }

            /// <summary>
            ///  Implements IOleObject::Close
            /// </summary>
            internal void Close(OLECLOSE dwSaveOption)
            {
                if (_activeXState[s_inPlaceActive])
                {
                    InPlaceDeactivate();
                }

                if ((dwSaveOption == OLECLOSE.SAVEIFDIRTY ||
                     dwSaveOption == OLECLOSE.PROMPTSAVE) &&
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
            internal unsafe HRESULT DoVerb(
                OLEIVERB iVerb,
                User32.MSG* lpmsg,
                IOleClientSite pActiveSite,
                int lindex,
                IntPtr hwndParent,
                RECT* lprcPosRect)
            {
                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "AxSource:ActiveXImpl:DoVerb(" + iVerb + ")");
                switch (iVerb)
                {
                    case OLEIVERB.SHOW:
                    case OLEIVERB.INPLACEACTIVATE:
                    case OLEIVERB.UIACTIVATE:
                    case OLEIVERB.PRIMARY:
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "DoVerb:Show, InPlaceActivate, UIActivate");
                        InPlaceActivate(iVerb);

                        // Now that we're active, send the lpmsg to the control if it
                        // is valid.
                        if (lpmsg != null)
                        {
                            Control target = _control;

                            if (lpmsg->hwnd != _control.Handle && lpmsg->IsMouseMessage())
                            {
                                // Must translate message coordniates over to our HWND.  We first try
                                IntPtr hwndMap = lpmsg->hwnd == IntPtr.Zero ? hwndParent : lpmsg->hwnd;
                                var pt = new Point
                                {
                                    X = PARAM.LOWORD(lpmsg->lParam),
                                    Y = PARAM.HIWORD(lpmsg->lParam)
                                };
                                User32.MapWindowPoints(hwndMap, new HandleRef(_control, _control.Handle), ref pt, 1);

                                // check to see if this message should really go to a child
                                //  control, and if so, map the point into that child's window
                                //  coordinates
                                Control realTarget = target.GetChildAtPoint(pt);
                                if (realTarget != null && realTarget != target)
                                {
                                    User32.MapWindowPoints(new HandleRef(target, target.Handle), new HandleRef(realTarget, realTarget.Handle), ref pt, 1);
                                    target = realTarget;
                                }

                                lpmsg->lParam = PARAM.FromLowHigh(pt.X, pt.Y);
                            }

#if DEBUG
                            if (CompModSwitches.ActiveX.TraceVerbose)
                            {
                                Message m = Message.Create(lpmsg->hwnd, lpmsg->message, lpmsg->wParam, lpmsg->lParam);
                                Debug.WriteLine("Valid message pointer passed, sending to control: " + m.ToString());
                            }
#endif

                            if (lpmsg->message == User32.WM.KEYDOWN && lpmsg->wParam == (IntPtr)User32.VK.TAB)
                            {
                                target.SelectNextControl(null, Control.ModifierKeys != Keys.Shift, true, true, true);
                            }
                            else
                            {
                                User32.SendMessageW(target, (User32.WM)lpmsg->message, lpmsg->wParam, lpmsg->lParam);
                            }
                        }
                        break;

                    // These affect our visibility
                    case OLEIVERB.HIDE:
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
                        ThrowHr(HRESULT.E_NOTIMPL);
                        break;
                }

                return HRESULT.S_OK;
            }

            /// <summary>
            ///  Implements IViewObject2::Draw.
            /// </summary>
            internal unsafe HRESULT Draw(
                DVASPECT dwDrawAspect,
                int lindex,
                IntPtr pvAspect,
                DVTARGETDEVICE* ptd,
                IntPtr hdcTargetDev,
                IntPtr hdcDraw,
                RECT* prcBounds,
                RECT* lprcWBounds,
                IntPtr pfnContinue,
                uint dwContinue)
            {
                // support the aspects required for multi-pass drawing
                switch (dwDrawAspect)
                {
                    case DVASPECT.CONTENT:
                    case DVASPECT.OPAQUE:
                    case DVASPECT.TRANSPARENT:
                        break;
                    default:
                        return HRESULT.DV_E_DVASPECT;
                }

                // We can paint to an enhanced metafile, but not all GDI / GDI+ is
                // supported on classic metafiles.  We throw VIEW_E_DRAW in the hope that
                // the caller figures it out and sends us a different DC.

                Gdi32.HDC hdc = (Gdi32.HDC)hdcDraw;
                Gdi32.OBJ hdcType = Gdi32.GetObjectType(hdc);
                if (hdcType == Gdi32.OBJ.METADC)
                {
                    return HRESULT.VIEW_E_DRAW;
                }

                var pVp = new Point();
                var pW = new Point();
                var sWindowExt = new Size();
                var sViewportExt = new Size();
                Gdi32.MM iMode = Gdi32.MM.TEXT;

                if (!_control.IsHandleCreated)
                {
                    _control.CreateHandle();
                }

                // if they didn't give us a rectangle, just copy over ours
                if (prcBounds != null)
                {
                    RECT rc = *prcBounds;

                    // To draw to a given rect, we scale the DC in such a way as to
                    // make the values it takes match our own happy MM_TEXT.  Then,
                    // we back-convert prcBounds so that we convert it to this coordinate
                    // system. This puts us in the most similar coordinates as we currently
                    // use.
                    Gdi32.LPtoDP(hdc, ref rc, 2);

                    iMode = Gdi32.SetMapMode(hdc, Gdi32.MM.ANISOTROPIC);
                    Gdi32.SetWindowOrgEx(hdc, 0, 0, &pW);
                    Gdi32.SetWindowExtEx(hdc, _control.Width, _control.Height, &sWindowExt);
                    Gdi32.SetViewportOrgEx(hdc, rc.left, rc.top, &pVp);
                    Gdi32.SetViewportExtEx(hdc, rc.right - rc.left, rc.bottom - rc.top, &sViewportExt);
                }

                // Now do the actual drawing.  We must ask all of our children to draw as well.
                try
                {
                    IntPtr flags = (IntPtr)(User32.PRF.CHILDREN | User32.PRF.CLIENT | User32.PRF.ERASEBKGND | User32.PRF.NONCLIENT);
                    if (hdcType != Gdi32.OBJ.ENHMETADC)
                    {
                        User32.SendMessageW(_control, User32.WM.PRINT, hdcDraw, flags);
                    }
                    else
                    {
                        _control.PrintToMetaFile(hdc, flags);
                    }
                }
                finally
                {
                    // And clean up the DC
                    if (prcBounds != null)
                    {
                        Gdi32.SetWindowOrgEx(hdc, pW.X, pW.Y, null);
                        Gdi32.SetWindowExtEx(hdc, sWindowExt.Width, sWindowExt.Height, null);
                        Gdi32.SetViewportOrgEx(hdc, pVp.X, pVp.Y, null);
                        Gdi32.SetViewportExtEx(hdc, sViewportExt.Width, sViewportExt.Height, null);
                        Gdi32.SetMapMode(hdc, iMode);
                    }
                }

                return HRESULT.S_OK;
            }

            /// <summary>
            ///  Returns a new verb enumerator.
            /// </summary>
            internal static HRESULT EnumVerbs(out IEnumOLEVERB ppEnumOleVerb)
            {
                if (s_axVerbs is null)
                {
                    var verbShow = new OLEVERB();
                    var verbInplaceActivate = new OLEVERB();
                    var verbUIActivate = new OLEVERB();
                    var verbHide = new OLEVERB();
                    var verbPrimary = new OLEVERB();
                    var verbProperties = new OLEVERB();

                    verbShow.lVerb = OLEIVERB.SHOW;
                    verbInplaceActivate.lVerb = OLEIVERB.INPLACEACTIVATE;
                    verbUIActivate.lVerb = OLEIVERB.UIACTIVATE;
                    verbHide.lVerb = OLEIVERB.HIDE;
                    verbPrimary.lVerb = OLEIVERB.PRIMARY;
                    verbProperties.lVerb = OLEIVERB.PROPERTIES;
                    verbProperties.lpszVerbName = SR.AXProperties;
                    verbProperties.grfAttribs = OLEVERBATTRIB.ONCONTAINERMENU;

                    s_axVerbs = new OLEVERB[]
                    {
                        verbShow,
                        verbInplaceActivate,
                        verbUIActivate,
                        verbHide,
                        verbPrimary
                    };
                }

                ppEnumOleVerb = new ActiveXVerbEnum(s_axVerbs);
                return HRESULT.S_OK;
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
            internal unsafe HRESULT GetAdvise(DVASPECT* pAspects, ADVF* pAdvf, IAdviseSink[] ppAdvSink)
            {
                if (pAspects != null)
                {
                    *pAspects = DVASPECT.CONTENT;
                }

                if (pAdvf != null)
                {
                    *pAdvf = 0;

                    if (_activeXState[s_viewAdviseOnlyOnce])
                    {
                        *pAdvf |= ADVF.ONLYONCE;
                    }
                    if (_activeXState[s_viewAdvisePrimeFirst])
                    {
                        *pAdvf |= ADVF.PRIMEFIRST;
                    }
                }

                if (ppAdvSink != null)
                {
                    ppAdvSink[0] = _viewAdviseSink;
                }

                return HRESULT.S_OK;
            }

            /// <summary>
            ///  Helper function to retrieve an ambient property.  Returns false if the
            ///  property wasn't found.
            /// </summary>
            private bool GetAmbientProperty(DispatchID dispid, ref object obj)
            {
                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetAmbientProperty");
                Debug.Indent();

                if (_clientSite is Oleaut32.IDispatch disp)
                {
                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "clientSite implements IDispatch");

                    var dispParams = new Oleaut32.DISPPARAMS();
                    object[] pvt = new object[1];
                    Guid g = Guid.Empty;
                    HRESULT hr = disp.Invoke(
                        dispid,
                        &g,
                        Kernel32.LCID.USER_DEFAULT,
                        Oleaut32.DISPATCH.PROPERTYGET,
                        &dispParams,
                        pvt,
                        null,
                        null);
                    if (hr.Succeeded())
                    {
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "IDispatch::Invoke succeeded. VT=" + pvt[0].GetType().FullName);
                        obj = pvt[0];
                        Debug.Unindent();
                        return true;
                    }

                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "IDispatch::Invoke failed. HR: 0x" + string.Format(CultureInfo.CurrentCulture, "{0:X}", hr));
                }

                Debug.Unindent();
                return false;
            }

            /// <summary>
            ///  Implements IOleObject::GetClientSite.
            /// </summary>
            internal IOleClientSite GetClientSite() => _clientSite;

            internal unsafe HRESULT GetControlInfo(CONTROLINFO* pCI)
            {
                if (_accelCount == -1)
                {
                    ArrayList mnemonicList = new ArrayList();
                    GetMnemonicList(_control, mnemonicList);

                    _accelCount = (short)mnemonicList.Count;

                    if (_accelCount > 0)
                    {
                        // In the worst case we may have two accelerators per mnemonic:  one lower case and
                        // one upper case, hence the * 2 below.
                        var accelerators = new User32.ACCEL[_accelCount * 2];
                        Debug.Indent();

                        ushort cmd = 0;
                        _accelCount = 0;

                        foreach (char ch in mnemonicList)
                        {
                            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Mnemonic: " + ch.ToString());

                            short scan = User32.VkKeyScanW(ch);
                            ushort key = (ushort)(scan & 0x00FF);
                            if (ch >= 'A' && ch <= 'Z')
                            {
                                // Lower case letter
                                accelerators[_accelCount++] = new User32.ACCEL
                                {
                                    fVirt = User32.AcceleratorFlags.FALT | User32.AcceleratorFlags.FVIRTKEY,
                                    key = key,
                                    cmd = cmd
                                };

                                // Upper case letter
                                accelerators[_accelCount++] = new User32.ACCEL
                                {
                                    fVirt = User32.AcceleratorFlags.FALT | User32.AcceleratorFlags.FVIRTKEY | User32.AcceleratorFlags.FSHIFT,
                                    key = key,
                                    cmd = cmd
                                };
                            }
                            else
                            {
                                // Some non-printable character.
                                User32.AcceleratorFlags virt = User32.AcceleratorFlags.FALT | User32.AcceleratorFlags.FVIRTKEY;
                                if ((scan & 0x0100) != 0)
                                {
                                    virt |= User32.AcceleratorFlags.FSHIFT;
                                }
                                accelerators[_accelCount++] = new User32.ACCEL
                                {
                                    fVirt = virt,
                                    key = key,
                                    cmd = cmd
                                };
                            }

                            cmd++;
                        }

                        Debug.Unindent();

                        // Now create an accelerator table and then free our memory.

                        if (_accelTable != IntPtr.Zero)
                        {
                            User32.DestroyAcceleratorTable(new HandleRef(this, _accelTable));
                            _accelTable = IntPtr.Zero;
                        }

                        fixed (User32.ACCEL* pAccelerators = accelerators)
                        {
                            _accelTable = User32.CreateAcceleratorTableW(pAccelerators, _accelCount);
                        }
                    }
                }

                pCI->cAccel = (ushort)_accelCount;
                pCI->hAccel = _accelTable;
                return HRESULT.S_OK;
            }

            /// <summary>
            ///  Implements IOleObject::GetExtent.
            /// </summary>
            internal unsafe void GetExtent(DVASPECT dwDrawAspect, Size* pSizel)
            {
                if ((dwDrawAspect & DVASPECT.CONTENT) != 0)
                {
                    Size size = _control.Size;

                    Point pt = PixelToHiMetric(size.Width, size.Height);
                    pSizel->Width = pt.X;
                    pSizel->Height = pt.Y;
                }
                else
                {
                    ThrowHr(HRESULT.DV_E_DVASPECT);
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
            internal unsafe HRESULT GetWindow(IntPtr* phwnd)
            {
                if (phwnd is null)
                {
                    return HRESULT.E_POINTER;
                }

                if (!_activeXState[s_inPlaceActive])
                {
                    *phwnd = IntPtr.Zero;
                    return HRESULT.E_FAIL;
                }

                *phwnd = _control.Handle;
                return HRESULT.S_OK;
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
            internal unsafe void InPlaceActivate(OLEIVERB verb)
            {
                // If we don't have a client site, then there's not much to do.
                // We also punt if this isn't an in-place site, since we can't
                // go active then.
                if (!(_clientSite is IOleInPlaceSite inPlaceSite))
                {
                    return;
                }

                // If we're not already active, go and do it.
                if (!_activeXState[s_inPlaceActive])
                {
                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "\tActiveXImpl:InPlaceActivate --> inplaceactive");

                    HRESULT hr = inPlaceSite.CanInPlaceActivate();
                    if (hr != HRESULT.S_OK)
                    {
                        if (hr.Succeeded())
                        {
                            hr = HRESULT.E_FAIL;
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
                    var inPlaceFrameInfo = new OLEINPLACEFRAMEINFO
                    {
                        cb = (uint)Marshal.SizeOf<OLEINPLACEFRAMEINFO>()
                    };

                    // We are entering a secure context here.
                    IntPtr hwndParent = IntPtr.Zero;
                    HRESULT hr = _inPlaceUiWindow.GetWindow(&hwndParent);
                    if (!hr.Succeeded())
                    {
                        ThrowHr(hr);
                    }

                    var posRect = new RECT();
                    var clipRect = new RECT();

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

                    inPlaceSite.GetWindowContext(
                        out IOleInPlaceFrame pFrame,
                        out IOleInPlaceUIWindow pWindow,
                        &posRect,
                        &clipRect,
                        &inPlaceFrameInfo);

                    SetObjectRects(&posRect, &clipRect);

                    _inPlaceFrame = pFrame;
                    _inPlaceUiWindow = pWindow;

                    // We are parenting ourselves
                    // directly to the host window.  The host must
                    // implement the ambient property
                    // DISPID_AMBIENT_MESSAGEREFLECT.
                    // If it doesn't, that means that the host
                    // won't reflect messages back to us.
                    HWNDParent = hwndParent;
                    if (User32.SetParent(new HandleRef(_control, _control.Handle), hwndParent) == IntPtr.Zero)
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error(), SR.Win32SetParentFailed);
                    }

                    // Now create our handle if it hasn't already been done.
                    _control.CreateControl();

                    _clientSite.ShowObject();

                    SetInPlaceVisible(true);
                    Debug.Assert(_activeXState[s_inPlaceVisible], "Failed to set inplacevisible");
                }

                // if we weren't asked to UIActivate, then we're done.
                if (verb != OLEIVERB.PRIMARY && verb != OLEIVERB.UIACTIVATE)
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
                    HRESULT hr = _inPlaceFrame.SetBorderSpace(null);
                    if (!hr.Succeeded() && hr != HRESULT.OLE_E_INVALIDRECT &&
                        hr != HRESULT.INPLACE_E_NOTOOLSPACE && hr != HRESULT.E_NOTIMPL)
                    {
                        Marshal.ThrowExceptionForHR((int)hr);
                    }

                    if (_inPlaceUiWindow != null)
                    {
                        hr = _inPlaceFrame.SetBorderSpace(null);
                        if (!hr.Succeeded() && hr != HRESULT.OLE_E_INVALIDRECT &&
                            hr != HRESULT.INPLACE_E_NOTOOLSPACE && hr != HRESULT.E_NOTIMPL)
                        {
                            Marshal.ThrowExceptionForHR((int)hr);
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
            internal HRESULT InPlaceDeactivate()
            {
                // Only do this if we're already in place active.
                if (!_activeXState[s_inPlaceActive])
                {
                    return HRESULT.S_OK;
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
                if (_clientSite is IOleInPlaceSite oleClientSite)
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

                return HRESULT.S_OK;
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
            internal void Load(IStorage stg)
            {
                IStream stream;
                try
                {
                    stream = stg.OpenStream(
                        GetStreamName(),
                        IntPtr.Zero,
                        STGM.READ | STGM.SHARE_EXCLUSIVE,
                        0);
                }
                catch (COMException e) when (e.ErrorCode == (int)HRESULT.STG_E_FILENOTFOUND)
                {
                    // For backward compatibility: We were earlier using GetType().FullName
                    // as the stream name in v1. Lets see if a stream by that name exists.
                    stream = stg.OpenStream(
                        GetType().FullName,
                        IntPtr.Zero,
                        STGM.READ | STGM.SHARE_EXCLUSIVE,
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
            internal void Load(IStream stream)
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
            internal unsafe void Load(Oleaut32.IPropertyBag pPropBag, Oleaut32.IErrorLog pErrorLog)
            {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(_control,
                    new Attribute[] { DesignerSerializationVisibilityAttribute.Visible });

                for (int i = 0; i < props.Count; i++)
                {
                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Loading property " + props[i].Name);

                    try
                    {
                        object obj = null;
                        HRESULT hr = pPropBag.Read(props[i].Name, ref obj, pErrorLog);
                        if (hr.Succeeded() && obj != null)
                        {
                            Debug.Indent();
                            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Property was in bag");

                            string errorString = null;
                            HRESULT errorCode = HRESULT.S_OK;

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
#pragma warning disable SYSLIB0011
                                    props[i].SetValue(_control, formatter.Deserialize(stream));
#pragma warning restore SYSLIB0011
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
                                if (e is ExternalException ee)
                                {
                                    errorCode = (HRESULT)ee.ErrorCode;
                                }
                                else
                                {
                                    errorCode = HRESULT.E_FAIL;
                                }
                            }
                            if (errorString != null)
                            {
                                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Exception converting property: " + errorString);
                                if (pErrorLog != null)
                                {
                                    IntPtr bstrSource = Marshal.StringToBSTR(_control.GetType().FullName);
                                    IntPtr bstrDescription = Marshal.StringToBSTR(errorString);
                                    try
                                    {
                                        var err = new Oleaut32.EXCEPINFO
                                        {
                                            bstrSource = bstrSource,
                                            bstrDescription = bstrDescription,
                                            scode = errorCode
                                        };
                                        pErrorLog.AddError(props[i].Name, &err);
                                    }
                                    finally
                                    {
                                        Marshal.FreeBSTR(bstrSource);
                                        Marshal.FreeBSTR(bstrDescription);
                                    }
                                }
                            }
                            Debug.Unindent();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Fail("Unexpected failure reading property", ex.ToString());

                        if (ClientUtils.IsCriticalException(ex))
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
            private AmbientProperty LookupAmbient(DispatchID dispid)
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
            ///  Merges the input region with the current clipping region, if any.
            /// </summary>
            internal Region MergeRegion(Region region)
            {
                if (_lastClipRect.HasValue)
                {
                    region.Exclude(_lastClipRect.Value);
                }

                return region;
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
            internal void OnAmbientPropertyChange(DispatchID dispID)
            {
                if (dispID != DispatchID.UNKNOWN)
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
                        case DispatchID.AMBIENT_UIDEAD:
                            if (GetAmbientProperty(DispatchID.AMBIENT_UIDEAD, ref obj))
                            {
                                _activeXState[s_uiDead] = (bool)obj;
                            }
                            break;

                        case DispatchID.AMBIENT_DISPLAYASDEFAULT:
                            if (_control is IButtonControl ibuttonControl && GetAmbientProperty(DispatchID.AMBIENT_DISPLAYASDEFAULT, ref obj))
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
            internal void OnDocWindowActivate(BOOL fActivate)
            {
                if (_activeXState[s_uiActive] && fActivate.IsTrue() && _inPlaceFrame != null)
                {
                    // we have to explicitly say we don't wany any border space.
                    HRESULT hr = _inPlaceFrame.SetBorderSpace(null);
                    if (!hr.Succeeded() && hr != HRESULT.INPLACE_E_NOTOOLSPACE && hr != HRESULT.E_NOTIMPL)
                    {
                        Marshal.ThrowExceptionForHR((int)hr);
                    }
                }
            }

            /// <summary>
            ///  Called by Control when it gets the focus.
            /// </summary>
            internal void OnFocus(bool focus)
            {
                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AXSource: SetFocus:  " + focus.ToString());
                if (_activeXState[s_inPlaceActive] && _clientSite is IOleControlSite)
                {
                    ((IOleControlSite)_clientSite).OnFocus(focus ? BOOL.TRUE : BOOL.FALSE);
                }

                if (focus && _activeXState[s_inPlaceActive] && !_activeXState[s_uiActive])
                {
                    InPlaceActivate(OLEIVERB.UIACTIVATE);
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
            internal unsafe HRESULT QuickActivate(QACONTAINER pQaContainer, QACONTROL* pQaControl)
            {
                if (pQaControl is null)
                {
                    return HRESULT.E_FAIL;
                }

                // Hookup our ambient colors
                AmbientProperty prop = LookupAmbient(DispatchID.AMBIENT_BACKCOLOR);
                prop.Value = ColorTranslator.FromOle(unchecked((int)pQaContainer.colorBack));

                prop = LookupAmbient(DispatchID.AMBIENT_FORECOLOR);
                prop.Value = ColorTranslator.FromOle(unchecked((int)pQaContainer.colorFore));

                // And our ambient font
                if (pQaContainer.pFont != null)
                {
                    prop = LookupAmbient(DispatchID.AMBIENT_FONT);

                    try
                    {
                        prop.Value = Font.FromHfont(pQaContainer.pFont.hFont);
                    }
                    catch (Exception e) when (!ClientUtils.IsCriticalException(e))
                    {
                        // Do NULL, so we just defer to the default font
                        prop.Value = null;
                    }
                }

                // Now use the rest of the goo that we got passed in.
                pQaControl->cbSize = (uint)Marshal.SizeOf<QACONTROL>();

                SetClientSite(pQaContainer.pClientSite);

                if (pQaContainer.pAdviseSink != null)
                {
                    SetAdvise(DVASPECT.CONTENT, 0, pQaContainer.pAdviseSink);
                }

                OLEMISC status = 0;
                ((IOleObject)_control).GetMiscStatus(DVASPECT.CONTENT, &status);
                pQaControl->dwMiscStatus = status;

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
                            AdviseHelper.AdviseConnectionPoint(_control, pQaContainer.pUnkEventSink, eventInterface, out pQaControl->dwEventCookie);
                        }
                        catch (Exception e) when (!ClientUtils.IsCriticalException(e))
                        {
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

                return HRESULT.S_OK;
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
                public static bool AdviseConnectionPoint(object connectionPoint, object sink, Type eventInterface, out uint pdwCookie)
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
                        return AdviseConnectionPoint(cpc, sink, eventInterface, out pdwCookie);
                    }
                }

                /// <summary>
                ///  Find the COM connection point and call Advise for the given event id.
                /// </summary>
                internal static bool AdviseConnectionPoint(ComConnectionPointContainer cpc, object sink, Type eventInterface, out uint pdwCookie)
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
                            return cp.Advise(punkEventsSink.DangerousGetHandle(), out pdwCookie);
                        }
                    }
                }

                /// <summary>
                ///  Wraps a native IUnknown in a SafeHandle.
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
#pragma warning disable SYSLIB0004 // Type or member is obsolete
                        RuntimeHelpers.PrepareConstrainedRegions();
#pragma warning restore SYSLIB0004 // Type or member is obsolete
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
                    public bool Advise(IntPtr punkEventSink, out uint pdwCookie)
                    {
                        AdviseD advise = (AdviseD)Marshal.GetDelegateForFunctionPointer(_vtbl.AdvisePtr, typeof(AdviseD));
                        if (advise.Invoke(handle, punkEventSink, out pdwCookie) == 0)
                        {
                            return true;
                        }
                        return false;
                    }

                    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                    private delegate int AdviseD(IntPtr This, IntPtr punkEventSink, out uint pdwCookie);
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
                    if (eventInterface is null)
                    {
                        eventInterface = Type.GetType(eventName, false);
                    }
                }

                return eventInterface;
            }

            /// <summary>
            ///  Implements IPersistStorage::Save
            /// </summary>
            internal void Save(IStorage stg, BOOL fSameAsLoad)
            {
                IStream stream = stg.CreateStream(
                    GetStreamName(),
                    STGM.WRITE | STGM.SHARE_EXCLUSIVE | STGM.CREATE,
                    0,
                    0);
                Debug.Assert(stream != null, "Stream should be non-null, or an exception should have been thrown.");

                Save(stream, BOOL.TRUE);
                Marshal.ReleaseComObject(stream);
            }

            /// <summary>
            ///  Implements IPersistStreamInit::Save
            /// </summary>
            internal void Save(IStream stream, BOOL fClearDirty)
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
            internal void Save(Oleaut32.IPropertyBag pPropBag, BOOL fClearDirty, BOOL fSaveAllProperties)
            {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(_control,
                    new Attribute[] { DesignerSerializationVisibilityAttribute.Visible });

                for (int i = 0; i < props.Count; i++)
                {
                    if (fSaveAllProperties.IsTrue() || props[i].ShouldSerializeValue(_control))
                    {
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Saving property " + props[i].Name);

                        object propValue;

                        if (IsResourceProp(props[i]))
                        {
                            // Resource property.  Save this to the bag as a 64bit encoded string.
                            MemoryStream stream = new MemoryStream();
                            BinaryFormatter formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011
                            formatter.Serialize(stream, props[i].GetValue(_control));
#pragma warning restore SYSLIB0011
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

                if (fClearDirty.IsTrue())
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
            internal HRESULT SetAdvise(DVASPECT aspects, ADVF advf, IAdviseSink pAdvSink)
            {
                // if it's not a content aspect, we don't support it.
                if ((aspects & DVASPECT.CONTENT) == 0)
                {
                    return HRESULT.DV_E_DVASPECT;
                }

                // Set up some flags to return from GetAdvise.
                _activeXState[s_viewAdvisePrimeFirst] = (advf & ADVF.PRIMEFIRST) != 0;
                _activeXState[s_viewAdviseOnlyOnce] = (advf & ADVF.ONLYONCE) != 0;

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

                return HRESULT.S_OK;
            }

            /// <summary>
            ///  Implements IOleObject::SetClientSite.
            /// </summary>
            internal void SetClientSite(IOleClientSite value)
            {
                if (_clientSite != null)
                {
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

                // Get the ambient properties that effect us.
                object obj = new object();
                if (GetAmbientProperty(DispatchID.AMBIENT_UIDEAD, ref obj))
                {
                    _activeXState[s_uiDead] = (bool)obj;
                }

                if (_control is IButtonControl buttonControl && GetAmbientProperty(Ole32.DispatchID.AMBIENT_UIDEAD, ref obj))
                {
                    buttonControl.NotifyDefault((bool)obj);
                }

                if (_clientSite is null && _accelTable != IntPtr.Zero)
                {
                    User32.DestroyAcceleratorTable(new HandleRef(this, _accelTable));
                    _accelTable = IntPtr.Zero;
                    _accelCount = -1;
                }

                _control.OnTopMostActiveXParentChanged(EventArgs.Empty);
            }

            /// <summary>
            ///  Implements IOleObject::SetExtent
            /// </summary>
            internal unsafe void SetExtent(DVASPECT dwDrawAspect, Size* pSizel)
            {
                if ((dwDrawAspect & DVASPECT.CONTENT) != 0)
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
                            if (_clientSite is IOleInPlaceSite ioleClientSite)
                            {
                                Rectangle bounds = _control.Bounds;
                                bounds.Location = new Point(bounds.X, bounds.Y);
                                Size adjusted = new Size(size.Width, size.Height);
                                bounds.Width = adjusted.Width;
                                bounds.Height = adjusted.Height;
                                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "SetExtent : Announcing to in place site that our rect has changed.");
                                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "            Announcing rect = " + bounds);
                                Debug.Assert(_clientSite != null, "How can we setextent before we are sited??");

                                RECT posRect = bounds;
                                ioleClientSite.OnPosRectChange(&posRect);
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
                    ThrowHr(HRESULT.DV_E_DVASPECT);
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
            internal unsafe HRESULT SetObjectRects(RECT* lprcPosRect, RECT* lprcClipRect)
            {
                if (lprcPosRect is null || lprcClipRect is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

#if DEBUG
                if (CompModSwitches.ActiveX.TraceInfo)
                {
                    Debug.WriteLine("SetObjectRects:");
                    Debug.Indent();

                    Debug.WriteLine("PosLeft:    " + lprcPosRect->left.ToString(CultureInfo.InvariantCulture));
                    Debug.WriteLine("PosTop:     " + lprcPosRect->top.ToString(CultureInfo.InvariantCulture));
                    Debug.WriteLine("PosRight:   " + lprcPosRect->right.ToString(CultureInfo.InvariantCulture));
                    Debug.WriteLine("PosBottom:  " + lprcPosRect->bottom.ToString(CultureInfo.InvariantCulture));

                    Debug.WriteLine("ClipLeft:   " + lprcClipRect->left.ToString(CultureInfo.InvariantCulture));
                    Debug.WriteLine("ClipTop:    " + lprcClipRect->top.ToString(CultureInfo.InvariantCulture));
                    Debug.WriteLine("ClipRight:  " + lprcClipRect->right.ToString(CultureInfo.InvariantCulture));
                    Debug.WriteLine("ClipBottom: " + lprcClipRect->bottom.ToString(CultureInfo.InvariantCulture));

                    Debug.Unindent();
                }
#endif

                Rectangle posRect = Rectangle.FromLTRB(lprcPosRect->left, lprcPosRect->top, lprcPosRect->right, lprcPosRect->bottom);

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
                    _adjustRect->left = posRect.X;
                    _adjustRect->top = posRect.Y;
                    _adjustRect->right = posRect.Right;
                    _adjustRect->bottom = posRect.Bottom;
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

                if (_lastClipRect.HasValue)
                {
                    // We had a clipping rectangle, we need to set the Control's Region even if we don't have a new
                    // lprcClipRect to ensure it remove it in said case.
                    _lastClipRect = null;
                    setRegion = true;
                }

                if (lprcClipRect != null)
                {
                    // The container wants us to clip, so figure out if we really need to.
                    Rectangle clipRect = *lprcClipRect;
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
                        IntPtr hWndParent = User32.GetParent(_control);

                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, $"Old Intersect: {(Rectangle)rcIntersect}");
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, $"New Control Bounds: {posRect}");

                        User32.MapWindowPoints(hWndParent, new HandleRef(_control, _control.Handle), ref rcIntersect, 2);

                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, $"New Intersect: {(Rectangle)rcIntersect}");

                        _lastClipRect = rcIntersect;
                        setRegion = true;

                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Created clipping region");
                    }
                }

                // If our region has changed, set the new value.  We only do this if
                // the handle has been created, since otherwise the control will
                // merge our region automatically.
                if (setRegion)
                {
                    _control.SetRegion(_control.Region);
                }

                // Yuck.  Forms^3 uses transparent overlay windows that appear to cause
                // painting artifacts.  Flicker like a banshee.
                _control.Invalidate();

                return HRESULT.S_OK;
            }

            /// <summary>
            ///  Throws the given hresult. This is used by ActiveX sourcing.
            /// </summary>
            internal static void ThrowHr(HRESULT hr)
            {
                throw new ExternalException(SR.ExternalException, (int)hr);
            }

            /// <summary>
            ///  Handles IOleControl::TranslateAccelerator
            /// </summary>
            internal unsafe HRESULT TranslateAccelerator(User32.MSG* lpmsg)
            {
                if (lpmsg is null)
                {
                    return HRESULT.E_POINTER;
                }

#if DEBUG
                if (CompModSwitches.ActiveX.TraceInfo)
                {
                    if (!_control.IsHandleCreated)
                    {
                        Debug.WriteLine("AxSource: TranslateAccelerator before handle creation");
                    }
                    else
                    {
                        Message m = Message.Create(lpmsg->hwnd, lpmsg->message, lpmsg->wParam, lpmsg->lParam);
                        Debug.WriteLine("AxSource: TranslateAccelerator : " + m.ToString());
                    }
                }
#endif // DEBUG

                bool needPreProcess = false;
                switch (lpmsg->message)
                {
                    case User32.WM.KEYDOWN:
                    case User32.WM.SYSKEYDOWN:
                    case User32.WM.CHAR:
                    case User32.WM.SYSCHAR:
                        needPreProcess = true;
                        break;
                }

                Message msg = Message.Create(lpmsg->hwnd, lpmsg->message, lpmsg->wParam, lpmsg->lParam);
                if (needPreProcess)
                {
                    Control target = FromChildHandle(lpmsg->hwnd);
                    if (target != null && (_control == target || _control.Contains(target)))
                    {
                        PreProcessControlState messageState = PreProcessControlMessageInternal(target, ref msg);
                        switch (messageState)
                        {
                            case PreProcessControlState.MessageProcessed:
                                // someone returned true from PreProcessMessage
                                // no need to dispatch the message, its already been coped with.
                                lpmsg->message = (User32.WM)msg.Msg;
                                lpmsg->wParam = msg.WParam;
                                lpmsg->lParam = msg.LParam;
                                return HRESULT.S_OK;
                            case PreProcessControlState.MessageNeeded:
                                // Here we need to dispatch the message ourselves
                                // otherwise the host may never send the key to our wndproc.

                                // Someone returned true from IsInputKey or IsInputChar
                                User32.TranslateMessage(ref *lpmsg);
                                if (User32.IsWindowUnicode(lpmsg->hwnd).IsTrue())
                                {
                                    User32.DispatchMessageW(ref *lpmsg);
                                }
                                else
                                {
                                    User32.DispatchMessageA(ref *lpmsg);
                                }
                                return HRESULT.S_OK;
                            case PreProcessControlState.MessageNotNeeded:
                                // in this case we'll check the site to see if it wants the message.
                                break;
                        }
                    }
                }

                // SITE processing.  We're not interested in the message, but the site may be.
                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource: Control did not process accelerator, handing to site");
                if (_clientSite is IOleControlSite ioleClientSite)
                {
                    KEYMODIFIERS keyState = 0;
                    if (User32.GetKeyState(User32.VK.SHIFT) < 0)
                    {
                        keyState |= KEYMODIFIERS.SHIFT;
                    }

                    if (User32.GetKeyState(User32.VK.CONTROL) < 0)
                    {
                        keyState |= KEYMODIFIERS.CONTROL;
                    }

                    if (User32.GetKeyState(User32.VK.MENU) < 0)
                    {
                        keyState |= KEYMODIFIERS.ALT;
                    }

                    return ioleClientSite.TranslateAccelerator(lpmsg, keyState);
                }

                return HRESULT.S_FALSE;
            }

            /// <summary>
            ///  Implements IOleInPlaceObject::UIDeactivate.
            /// </summary>
            internal HRESULT UIDeactivate()
            {
                // Only do this if we're UI active
                if (!_activeXState[s_uiActive])
                {
                    return HRESULT.S_OK;
                }

                _activeXState[s_uiActive] = false;

                // Notify frame windows, if appropriate, that we're no longer ui-active.
                _inPlaceUiWindow?.SetActiveObject(null, null);

                // May need this for SetActiveObject & OnUIDeactivate, so leave until function return
                Debug.Assert(_inPlaceFrame != null, "No inplace frame -- how dod we go UI active?");
                _inPlaceFrame.SetActiveObject(null, null);

                if (_clientSite is IOleInPlaceSite ioleClientSite)
                {
                    ioleClientSite.OnUIDeactivate(0);
                }

                return HRESULT.S_OK;
            }

            /// <summary>
            ///  Implements IOleObject::Unadvise
            /// </summary>
            internal HRESULT Unadvise(uint dwConnection)
            {
                if (dwConnection > _adviseList.Count || _adviseList[(int)dwConnection - 1] is null)
                {
                    return HRESULT.OLE_E_NOCONNECTION;
                }

                IAdviseSink sink = (IAdviseSink)_adviseList[(int)dwConnection - 1];
                _adviseList.RemoveAt((int)dwConnection - 1);
                if (sink != null && Marshal.IsComObject(sink))
                {
                    Marshal.ReleaseComObject(sink);
                }

                return HRESULT.S_OK;
            }

            /// <summary>
            ///  Notifies our site that we have changed our size and location.
            /// </summary>
            internal unsafe void UpdateBounds(ref int x, ref int y, ref int width, ref int height, User32.SWP flags)
            {
                if (!_activeXState[s_adjustingRect] && _activeXState[s_inPlaceVisible])
                {
                    if (_clientSite is IOleInPlaceSite ioleClientSite)
                    {
                        var rc = new RECT();
                        if ((flags & User32.SWP.NOMOVE) != 0)
                        {
                            rc.left = _control.Left;
                            rc.top = _control.Top;
                        }
                        else
                        {
                            rc.left = x;
                            rc.top = y;
                        }

                        if ((flags & User32.SWP.NOSIZE) != 0)
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
                        _adjustRect = &rc;
                        _activeXState[s_adjustingRect] = true;

                        try
                        {
                            ioleClientSite.OnPosRectChange(&rc);
                        }
                        finally
                        {
                            _adjustRect = null;
                            _activeXState[s_adjustingRect] = false;
                        }

                        // On output, the new bounds will be reflected in  rc
                        if ((flags & User32.SWP.NOMOVE) == 0)
                        {
                            x = rc.left;
                            y = rc.top;
                        }
                        if ((flags & User32.SWP.NOSIZE) == 0)
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

                if (_clientSite is IOleControlSite ioleClientSite)
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
                    _viewAdviseSink.OnViewChange((int)DVASPECT.CONTENT, -1);

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
                    if (m.IsMouseMessage())
                    {
                        return;
                    }
                    if (m.Msg >= (int)User32.WM.NCLBUTTONDOWN && m.Msg <= (int)User32.WM.NCMBUTTONDBLCLK)
                    {
                        return;
                    }
                    if (m.IsKeyMessage())
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
            private class PropertyBagStream : Oleaut32.IPropertyBag
            {
                private Hashtable _bag = new Hashtable();

                internal void Read(IStream istream)
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
#pragma warning disable SYSLIB0011
                        _bag = (Hashtable)formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011
                    }
                    catch (Exception e)
                    {
                        if (ClientUtils.IsCriticalException(e))
                        {
                            throw;
                        }

                        // Error reading.  Just init an empty hashtable.
                        _bag = new Hashtable();
                    }
                }

                HRESULT Oleaut32.IPropertyBag.Read(string pszPropName, ref object pVar, Oleaut32.IErrorLog pErrorLog)
                {
                    if (!_bag.Contains(pszPropName))
                    {
                        return HRESULT.E_INVALIDARG;
                    }

                    pVar = _bag[pszPropName];
                    return HRESULT.S_OK;
                }

                HRESULT Oleaut32.IPropertyBag.Write(string pszPropName, ref object pVar)
                {
                    _bag[pszPropName] = pVar;
                    return HRESULT.S_OK;
                }

                internal void Write(IStream istream)
                {
                    Stream stream = new DataStreamFromComStream(istream);
                    BinaryFormatter formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011
                    formatter.Serialize(stream, _bag);
#pragma warning restore SYSLIB0011
                }
            }
        }
    }
}

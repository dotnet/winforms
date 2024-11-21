// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Formats.Nrbf;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Windows.Win32.System.Com;
using Windows.Win32.System.Com.StructuredStorage;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using System.Windows.Forms.Nrbf;
using RECTL = Windows.Win32.Foundation.RECTL;
using System.Windows.Forms.BinaryFormat;

namespace System.Windows.Forms;

public partial class Control
{
    // Helper methods for retrieving ActiveX properties.
    // We abstract these through another method so we do not force JIT the ActiveX codebase.

    private Color ActiveXAmbientBackColor
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        get => ActiveXInstance.AmbientBackColor;
    }

    private Color ActiveXAmbientForeColor
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        get => ActiveXInstance.AmbientForeColor;
    }

    private Font? ActiveXAmbientFont
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        get => ActiveXInstance.AmbientFont;
    }

    private bool ActiveXEventsFrozen
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        get => ActiveXInstance.EventsFrozen;
    }

    private IntPtr ActiveXHWNDParent
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        get => ActiveXInstance.HWNDParent;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private Region ActiveXMergeRegion(Region region) => ActiveXInstance.MergeRegion(region);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ActiveXOnFocus(bool focus) => ActiveXInstance.OnFocus(focus);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ActiveXViewChanged() => ActiveXInstance.ViewChangedInternal();

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ActiveXUpdateBounds(ref int x, ref int y, ref int width, ref int height, SET_WINDOW_POS_FLAGS flags)
        => ActiveXInstance.UpdateBounds(ref x, ref y, ref width, ref height, flags);

    /// <summary>
    ///  Retrieves the ActiveX control implementation for this control.
    ///  This will demand create the implementation if it does not already exist.
    /// </summary>
    private ActiveXImpl ActiveXInstance
    {
        get
        {
            if (!Properties.TryGetValue(s_activeXImplProperty, out ActiveXImpl? activeXImpl))
            {
                // Don't allow top level objects to be hosted as activeX controls.
                if (GetState(States.TopLevel))
                {
                    throw new NotSupportedException(SR.AXTopLevelSource);
                }

                // PERF: IsActiveX is called quite a bit - checked everywhere from sizing to event raising. Using a
                // state bit to track instead of fetching from the property store.
                SetExtendedState(ExtendedStates.IsActiveX, true);
                activeXImpl = Properties.AddValue(s_activeXImplProperty, new ActiveXImpl(this));
            }

            return activeXImpl;
        }
    }

    /// <summary>
    ///  This class holds all of the state data for an ActiveX control and supplies the implementation for many of
    ///  the non-trivial methods.
    /// </summary>
    private sealed unsafe partial class ActiveXImpl : MarshalByRefObject, IWindowTarget, IDisposable
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
        private static readonly SearchValues<char> s_whitespace = SearchValues.Create(" \r\n");

        private static Point s_logPixels = Point.Empty;
        private static OLEVERB[]? s_axVerbs;

        private readonly Control _control;
        private readonly IWindowTarget _controlWindowTarget;
        private Rectangle? _lastClipRect;

        private AgileComPointer<IOleClientSite>? _clientSite;
        private AgileComPointer<IOleInPlaceUIWindow>? _inPlaceUiWindow;
        private AgileComPointer<IOleInPlaceFrame>? _inPlaceFrame;
        private AgileComPointer<IOleAdviseHolder>? _adviseHolder;
        private IAdviseSink* _viewAdviseSink;
        private BitVector32 _activeXState;
        private readonly AmbientProperty[] _ambientProperties;
        private HACCEL _accelTable;
        private short _accelCount = -1;
        private RECT* _adjustRect; // temporary rect used during OnPosRectChange && SetObjectRects

        // Feature switch, when set to false, ActiveX is not supported in trimmed applications.
        [FeatureSwitchDefinition("System.Windows.Forms.ActiveXImpl.IsSupported")]
#pragma warning disable IDE0075 // Simplify conditional expression - the simpler expression is hard to read
        private static bool IsSupported { get; } =
            AppContext.TryGetSwitch("System.Windows.Forms.ActiveXImpl.IsSupported", out bool isSupported)
                ? isSupported
                : true;
#pragma warning restore IDE0075

        /// <summary>
        ///  Creates a new ActiveXImpl.
        /// </summary>
        internal ActiveXImpl(Control control)
        {
            _control = control;

            // We replace the control's window target with our own. We
            // do this so we can handle the UI Dead ambient property.
            _controlWindowTarget = control.WindowTarget;
            control.WindowTarget = this;

            _activeXState = default;
            _ambientProperties =
            [
                new("Font", PInvokeCore.DISPID_AMBIENT_FONT),
                new("BackColor", PInvokeCore.DISPID_AMBIENT_BACKCOLOR),
                new("ForeColor", PInvokeCore.DISPID_AMBIENT_FORECOLOR)
            ];
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
                AmbientProperty property = LookupAmbient(PInvokeCore.DISPID_AMBIENT_BACKCOLOR);

                if (property.Empty)
                {
                    using VARIANT value = GetAmbientProperty(PInvokeCore.DISPID_AMBIENT_BACKCOLOR);
                    if (value.vt is VARENUM.VT_I4 or VARENUM.VT_INT)
                    {
                        property.Value = ColorTranslator.FromOle(value.data.intVal);
                    }
                }

                return property.Value is null ? Color.Empty : (Color)property.Value;
            }
        }

        /// <summary>
        ///  Retrieves the ambient font for the control.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal Font? AmbientFont
        {
            get
            {
                AmbientProperty property = LookupAmbient(PInvokeCore.DISPID_AMBIENT_FONT);

                if (property.Empty)
                {
                    using VARIANT value = GetAmbientProperty(PInvokeCore.DISPID_AMBIENT_FONT);
                    if (value.vt == VARENUM.VT_UNKNOWN)
                    {
                        using var font = ComScope<IFont>.TryQueryFrom(value.data.punkVal, out HRESULT hr);
                        if (hr.Succeeded)
                        {
                            try
                            {
                                property.Value = Font.FromHfont(font.Value->hFont);
                            }
                            catch (ArgumentException)
                            {
                                // Not a TrueType font.
                            }
                        }
                    }
                }

                return (Font?)property.Value;
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
                AmbientProperty property = LookupAmbient(PInvokeCore.DISPID_AMBIENT_FORECOLOR);

                if (property.Empty)
                {
                    using VARIANT value = GetAmbientProperty(PInvokeCore.DISPID_AMBIENT_FORECOLOR);
                    if (value.vt is VARENUM.VT_I4 or VARENUM.VT_INT)
                    {
                        property.Value = ColorTranslator.FromOle(value.data.intVal);
                    }
                }

                return property.Value is null ? Color.Empty : (Color)property.Value;
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
            get => _activeXState[s_eventsFrozen];
            set => _activeXState[s_eventsFrozen] = value;
        }

        /// <summary>
        ///  Provides access to the parent window handle when we are UI active.
        /// </summary>
        internal HWND HWNDParent { get; private set; }

        /// <summary>
        ///  Retrieves the number of logical pixels per inch on the primary monitor.
        /// </summary>
        private static Point LogPixels
        {
            get
            {
                if (s_logPixels.IsEmpty)
                {
                    s_logPixels = default;
                    using var dc = GetDcScope.ScreenDC;
                    s_logPixels.X = PInvokeCore.GetDeviceCaps(dc, GET_DEVICE_CAPS_INDEX.LOGPIXELSX);
                    s_logPixels.Y = PInvokeCore.GetDeviceCaps(dc, GET_DEVICE_CAPS_INDEX.LOGPIXELSY);
                }

                return s_logPixels;
            }
        }

        /// <inheritdoc cref="IOleObject.Advise(IAdviseSink*, uint*)"/>
        internal HRESULT Advise(IAdviseSink* pAdvSink, uint* token)
        {
            if (_adviseHolder is null)
            {
                IOleAdviseHolder* holder = null;
                HRESULT hr = PInvoke.CreateOleAdviseHolder(&holder);
                if (hr.Failed)
                {
                    return hr;
                }

                _adviseHolder = new(holder, takeOwnership: true);
            }

            using var adviseHolder = _adviseHolder.GetInterface();
            return adviseHolder.Value->Advise(pAdvSink, token);
        }

        /// <inheritdoc cref="IOleObject.Close(uint)"/>
        internal void Close(OLECLOSE dwSaveOption)
        {
            if (_activeXState[s_inPlaceActive])
            {
                InPlaceDeactivate();
            }

            if ((dwSaveOption == OLECLOSE.OLECLOSE_SAVEIFDIRTY || dwSaveOption == OLECLOSE.OLECLOSE_PROMPTSAVE)
                && _activeXState[s_isDirty])
            {
                if (_clientSite is not null)
                {
                    using var clientSite = _clientSite.GetInterface();
                    clientSite.Value->SaveObject();
                }

                SendOnSave();
            }
        }

        /// <inheritdoc cref="IOleObject.DoVerb(int, MSG*, IOleClientSite*, int, HWND, RECT*)"/>
        internal unsafe HRESULT DoVerb(
            OLEIVERB iVerb,
            MSG* lpmsg,
            IOleClientSite* pActiveSite,
            int lindex,
            HWND hwndParent,
            RECT* lprcPosRect)
        {
            switch (iVerb)
            {
                case OLEIVERB.OLEIVERB_SHOW:
                case OLEIVERB.OLEIVERB_INPLACEACTIVATE:
                case OLEIVERB.OLEIVERB_UIACTIVATE:
                case OLEIVERB.OLEIVERB_PRIMARY:
                    InPlaceActivate(iVerb);

                    // Now that we're active, send the lpmsg to the control if it is valid.
                    if (lpmsg is null)
                    {
                        break;
                    }

                    Control target = _control;

                    HWND hwnd = lpmsg->hwnd;
                    if (hwnd != _control.HWND && lpmsg->IsMouseMessage())
                    {
                        // Must translate message coordinates over to our HWND.
                        HWND hwndMap = hwnd.IsNull ? hwndParent : hwnd;
                        Point pt = new(PARAM.LOWORD(lpmsg->lParam), PARAM.HIWORD(lpmsg->lParam));

                        PInvokeCore.MapWindowPoints(hwndMap, _control, ref pt);

                        // Check to see if this message should really go to a child control, and if so, map the
                        // point into that child's window coordinates.
                        Control? realTarget = target.GetChildAtPoint(pt);
                        if (realTarget is not null && realTarget != target)
                        {
                            pt = WindowsFormsUtils.TranslatePoint(pt, target, realTarget);
                            target = realTarget;
                        }

                        lpmsg->lParam = PARAM.FromPoint(pt);
                    }

                    if (lpmsg->message == PInvokeCore.WM_KEYDOWN && lpmsg->wParam == (WPARAM)(nuint)VIRTUAL_KEY.VK_TAB)
                    {
                        target.SelectNextControl(null, ModifierKeys != Keys.Shift, tabStopOnly: true, nested: true, wrap: true);
                    }
                    else
                    {
                        PInvokeCore.SendMessage(target, lpmsg->message, lpmsg->wParam, lpmsg->lParam);
                    }

                    break;

                case OLEIVERB.OLEIVERB_HIDE:
                    UIDeactivate();
                    InPlaceDeactivate();
                    if (_activeXState[s_inPlaceVisible])
                    {
                        SetInPlaceVisible(false);
                    }

                    break;

                // All other verbs are not implemented.
                default:
                    return HRESULT.E_NOTIMPL;
            }

            return HRESULT.S_OK;
        }

        /// <inheritdoc cref="IViewObject.Interface.Draw(DVASPECT, int, void*, DVTARGETDEVICE*, HDC, HDC, RECTL*, RECTL*, nint, nuint)"/>
        internal HRESULT Draw(
            DVASPECT dwDrawAspect,
            int lindex,
            void* pvAspect,
            DVTARGETDEVICE* ptd,
            HDC hdcTargetDev,
            HDC hdcDraw,
            RECT* prcBounds,
            RECT* lprcWBounds,
            nint pfnContinue,
            nuint dwContinue)
        {
            // Support the aspects required for multi-pass drawing.
            switch (dwDrawAspect)
            {
                case DVASPECT.DVASPECT_CONTENT:
                case DVASPECT.DVASPECT_OPAQUE:
                case DVASPECT.DVASPECT_TRANSPARENT:
                    break;
                default:
                    return HRESULT.DV_E_DVASPECT;
            }

            // We can paint to an enhanced metafile, but not all GDI / GDI+ is
            // supported on classic metafiles. We throw VIEW_E_DRAW in the hope that
            // the caller figures it out and sends us a different DC.

            OBJ_TYPE hdcType = (OBJ_TYPE)PInvokeCore.GetObjectType(hdcDraw);
            if (hdcType == OBJ_TYPE.OBJ_METADC)
            {
                return HRESULT.VIEW_E_DRAW;
            }

            Point pVp = default;
            Point pW = default;
            Size sWindowExt = default;
            Size sViewportExt = default;
            HDC_MAP_MODE iMode = HDC_MAP_MODE.MM_TEXT;

            if (!_control.IsHandleCreated)
            {
                _control.CreateHandle();
            }

            // If they didn't give us a rectangle, just copy over ours.
            if (prcBounds is not null)
            {
                RECT rc = *prcBounds;

                // To draw to a given rect, we scale the DC in such a way as to make the values it takes match our
                // own happy MM_TEXT. Then, we back-convert prcBounds so that we convert it to this coordinate
                // system. This puts us in the most similar coordinates as we currently use.
                Point p1 = new(rc.left, rc.top);
                Point p2 = new(rc.right - rc.left, rc.bottom - rc.top);
                PInvoke.LPtoDP(hdcDraw, [p1, p2]);

                iMode = (HDC_MAP_MODE)PInvokeCore.SetMapMode(hdcDraw, HDC_MAP_MODE.MM_ANISOTROPIC);
                PInvoke.SetWindowOrgEx(hdcDraw, 0, 0, &pW);
                PInvoke.SetWindowExtEx(hdcDraw, _control.Width, _control.Height, (SIZE*)&sWindowExt);
                PInvoke.SetViewportOrgEx(hdcDraw, p1.X, p1.Y, &pVp);
                PInvoke.SetViewportExtEx(hdcDraw, p2.X, p2.Y, (SIZE*)&sViewportExt);
            }

            // Now do the actual drawing. We must ask all of our children to draw as well.
            try
            {
                nint flags = PInvoke.PRF_CHILDREN | PInvoke.PRF_CLIENT | PInvoke.PRF_ERASEBKGND | PInvoke.PRF_NONCLIENT;
                if (hdcType != OBJ_TYPE.OBJ_ENHMETADC)
                {
                    PInvokeCore.SendMessage(_control, PInvokeCore.WM_PRINT, (WPARAM)hdcDraw, (LPARAM)flags);
                }
                else
                {
                    _control.PrintToMetaFile(hdcDraw, flags);
                }
            }
            finally
            {
                // And clean up the DC
                if (prcBounds is not null)
                {
                    PInvoke.SetWindowOrgEx(hdcDraw, pW.X, pW.Y, lppt: null);
                    PInvoke.SetWindowExtEx(hdcDraw, sWindowExt.Width, sWindowExt.Height, lpsz: null);
                    PInvoke.SetViewportOrgEx(hdcDraw, pVp.X, pVp.Y, lppt: null);
                    PInvoke.SetViewportExtEx(hdcDraw, sViewportExt.Width, sViewportExt.Height, lpsz: null);
                    PInvokeCore.SetMapMode(hdcDraw, iMode);
                }
            }

            return HRESULT.S_OK;
        }

        /// <inheritdoc cref="IOleObject.EnumVerbs(IEnumOLEVERB**)"/>
        internal static IEnumOLEVERB* EnumVerbs()
        {
            s_axVerbs ??=
            [
                new() { lVerb = OLEIVERB.OLEIVERB_SHOW },
                new() { lVerb = OLEIVERB.OLEIVERB_INPLACEACTIVATE },
                new() { lVerb = OLEIVERB.OLEIVERB_UIACTIVATE },
                new() { lVerb = OLEIVERB.OLEIVERB_HIDE },
                new() { lVerb = OLEIVERB.OLEIVERB_PRIMARY },
            ];

            return ComHelpers.GetComPointer<IEnumOLEVERB>(new ActiveXVerbEnum(s_axVerbs));
        }

        /// <summary>
        ///  Converts the given string to a byte array.
        /// </summary>
        private static byte[] FromBase64WrappedString(string text)
        {
            if (text.AsSpan().ContainsAny(s_whitespace))
            {
                StringBuilder sb = new(text.Length);
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

        /// <inheritdoc cref="IViewObject.GetAdvise(uint*, uint*, IAdviseSink**)"/>
        internal unsafe HRESULT GetAdvise(DVASPECT* pAspects, ADVF* pAdvf, IAdviseSink** ppAdvSink)
        {
            if (pAspects is not null)
            {
                *pAspects = DVASPECT.DVASPECT_CONTENT;
            }

            if (pAdvf is not null)
            {
                *pAdvf = 0;

                if (_activeXState[s_viewAdviseOnlyOnce])
                {
                    *pAdvf |= ADVF.ADVF_ONLYONCE;
                }

                if (_activeXState[s_viewAdvisePrimeFirst])
                {
                    *pAdvf |= ADVF.ADVF_PRIMEFIRST;
                }
            }

            if (ppAdvSink is not null)
            {
                *ppAdvSink = _viewAdviseSink;

                if (_viewAdviseSink is not null)
                {
                    _viewAdviseSink->AddRef();
                }
            }

            return HRESULT.S_OK;
        }

        /// <summary>
        ///  Helper function to retrieve an ambient property. Returns empty if not found.
        /// </summary>
        private VARIANT GetAmbientProperty(int dispid)
        {
            VARIANT property = default;

            if (_clientSite is null)
            {
                return property;
            }

            using var dispatch = _clientSite.TryGetInterface<IDispatch>(out HRESULT hr);
            if (hr.Failed)
            {
                return property;
            }

            hr = dispatch.Value->TryGetProperty(dispid, &property, PInvoke.LCID.USER_DEFAULT.RawValue);

            if (hr.Failed)
            {
                Debug.WriteLine($"Failed to get property for dispid {dispid}: {hr}");
            }

            return property;
        }

        /// <inheritdoc cref="IOleObject.GetClientSite(IOleClientSite**)"/>
        internal ComScope<IOleClientSite> GetClientSite() => _clientSite is null ? default : _clientSite.GetInterface();

        /// <inheritdoc cref="IOleControl.GetControlInfo(CONTROLINFO*)"/>
        internal unsafe HRESULT GetControlInfo(CONTROLINFO* pControlInfo)
        {
            if (_accelCount == -1)
            {
                List<char> mnemonicList = [];
                GetMnemonicList(_control, mnemonicList);

                _accelCount = (short)mnemonicList.Count;

                if (_accelCount > 0)
                {
                    // In the worst case we may have two accelerators per mnemonic: one lower case and
                    // one upper case, hence the * 2 below.
                    var accelerators = new ACCEL[_accelCount * 2];
                    ushort cmd = 0;
                    _accelCount = 0;

                    foreach (char ch in mnemonicList)
                    {
                        short scan = PInvoke.VkKeyScan(ch);
                        ushort key = (ushort)(scan & 0x00FF);
                        if (ch is >= 'A' and <= 'Z')
                        {
                            // Lowercase letter.
                            accelerators[_accelCount++] = new ACCEL
                            {
                                fVirt = ACCEL_VIRT_FLAGS.FALT | ACCEL_VIRT_FLAGS.FVIRTKEY,
                                key = key,
                                cmd = cmd
                            };

                            // Uppercase letter.
                            accelerators[_accelCount++] = new ACCEL
                            {
                                fVirt = ACCEL_VIRT_FLAGS.FALT | ACCEL_VIRT_FLAGS.FVIRTKEY | ACCEL_VIRT_FLAGS.FSHIFT,
                                key = key,
                                cmd = cmd
                            };
                        }
                        else
                        {
                            // Some non-printable character.
                            ACCEL_VIRT_FLAGS virt = ACCEL_VIRT_FLAGS.FALT | ACCEL_VIRT_FLAGS.FVIRTKEY;
                            if ((scan & 0x0100) != 0)
                            {
                                virt |= ACCEL_VIRT_FLAGS.FSHIFT;
                            }

                            accelerators[_accelCount++] = new ACCEL
                            {
                                fVirt = virt,
                                key = key,
                                cmd = cmd
                            };
                        }

                        cmd++;
                    }

                    // Now create an accelerator table and then free our memory.

                    if (!_accelTable.IsNull)
                    {
                        PInvoke.DestroyAcceleratorTable(new HandleRef<HACCEL>(_control, _accelTable));
                        _accelTable = HACCEL.Null;
                    }

                    fixed (ACCEL* pAccelerators = accelerators)
                    {
                        _accelTable = PInvoke.CreateAcceleratorTable(pAccelerators, _accelCount);
                    }
                }
            }

            pControlInfo->cAccel = (ushort)_accelCount;
            pControlInfo->hAccel = _accelTable;
            return HRESULT.S_OK;
        }

        /// <inheritdoc cref="IOleObject.GetExtent(DVASPECT, SIZE*)"/>
        internal unsafe void GetExtent(DVASPECT dwDrawAspect, Size* pSizel)
        {
            if (!dwDrawAspect.HasFlag(DVASPECT.DVASPECT_CONTENT))
            {
                ThrowHr(HRESULT.DV_E_DVASPECT);
            }

            Size size = _control.Size;
            Point pt = PixelToHiMetric(size.Width, size.Height);
            pSizel->Width = pt.X;
            pSizel->Height = pt.Y;
        }

        /// <summary>
        ///  Searches the control hierarchy of the given control and adds
        ///  the mnemonics for each control to mnemonicList. Each mnemonic
        ///  is added as a char to the list.
        /// </summary>
        private static void GetMnemonicList(Control control, List<char> mnemonicList)
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
                if (c is not null)
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
            string streamName = _control.GetType().FullName!;
            int len = streamName.Length;
            if (len > 31)
            {
                // The max allowed length of the stream name is 31.
                streamName = streamName[(len - 31)..];
            }

            return streamName;
        }

        /// <inheritdoc cref="IOleWindow.GetWindow(HWND*)"/>
        internal unsafe HRESULT GetWindow(HWND* phwnd)
        {
            if (phwnd is null)
            {
                return HRESULT.E_POINTER;
            }

            if (!_activeXState[s_inPlaceActive])
            {
                *phwnd = HWND.Null;
                return HRESULT.E_FAIL;
            }

            *phwnd = (HWND)_control.Handle;
            return HRESULT.S_OK;
        }

        /// <summary>
        ///  Converts coordinates in HiMetric to pixels. Used for ActiveX sourcing.
        /// </summary>
        private static Point HiMetricToPixel(int x, int y)
        {
            Point pt = new Point
            {
                X = (LogPixels.X * x + HiMetricPerInch / 2) / HiMetricPerInch,
                Y = (LogPixels.Y * y + HiMetricPerInch / 2) / HiMetricPerInch
            };
            return pt;
        }

        /// <summary>
        ///  In place activates this object.
        /// </summary>
        internal unsafe void InPlaceActivate(OLEIVERB verb)
        {
            // If we don't have a client site, then there's not much to do.
            // We also punt if this isn't an in-place site, since we can't go active then.

            if (_clientSite is null)
            {
                return;
            }

            using var inPlaceSite = _clientSite.TryGetInterface<IOleInPlaceSite>(out HRESULT hr);
            if (hr.Failed)
            {
                return;
            }

            // If we're not already active, go and do it.
            if (!_activeXState[s_inPlaceActive])
            {
                hr = inPlaceSite.Value->CanInPlaceActivate();
                if (hr != HRESULT.S_OK)
                {
                    if (hr.Succeeded)
                    {
                        hr = HRESULT.E_FAIL;
                    }

                    ThrowHr(hr);
                }

                inPlaceSite.Value->OnInPlaceActivate();

                _activeXState[s_inPlaceActive] = true;
            }

            // And if we're not visible, do that too.
            if (!_activeXState[s_inPlaceVisible])
            {
                OLEINPLACEFRAMEINFO inPlaceFrameInfo = new()
                {
                    cb = (uint)sizeof(OLEINPLACEFRAMEINFO)
                };

                // We are entering a secure context here.
                HWND hwndParent = default;
                hr = inPlaceSite.Value->GetWindow(&hwndParent);
                if (!hr.Succeeded)
                {
                    ThrowHr(hr);
                }

                RECT posRect = default;
                RECT clipRect = default;

                DisposeHelper.NullAndDispose(ref _inPlaceUiWindow);
                DisposeHelper.NullAndDispose(ref _inPlaceFrame);

                IOleInPlaceFrame* pFrame;
                IOleInPlaceUIWindow* pWindow;
                inPlaceSite.Value->GetWindowContext(
                    &pFrame,
                    &pWindow,
                    &posRect,
                    &clipRect,
                    &inPlaceFrameInfo).AssertSuccess();

                SetObjectRects(&posRect, &clipRect);

                _inPlaceFrame = pFrame is null ? null : new(pFrame, takeOwnership: true);
                _inPlaceUiWindow = pWindow is null ? null : new(pWindow, takeOwnership: true);

                // We are parenting ourselves directly to the host window. The host must implement the ambient property
                // DISPID_AMBIENT_MESSAGEREFLECT. If it doesn't, that means that the host won't reflect messages back
                // to us.
                HWNDParent = hwndParent;
                if (PInvoke.SetParent(_control, hwndParent).IsNull)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), SR.Win32SetParentFailed);
                }

                // Now create our handle if it hasn't already been done.
                _control.CreateControl();

                using var clientSite = _clientSite.GetInterface();
                clientSite.Value->ShowObject();

                SetInPlaceVisible(true);
                Debug.Assert(_activeXState[s_inPlaceVisible], "Failed to set inplacevisible");
            }

            // If we weren't asked to UIActivate, then we're done.
            if (verb is not OLEIVERB.OLEIVERB_PRIMARY and not OLEIVERB.OLEIVERB_UIACTIVATE)
            {
                return;
            }

            // If we're not already UI active, do sow now.
            if (_activeXState[s_uiActive])
            {
                return;
            }

            _activeXState[s_uiActive] = true;

            // Inform the container of our intent.
            inPlaceSite.Value->OnUIActivate();

            // Take the focus [which is what UI Activation is all about]
            if (!_control.ContainsFocus)
            {
                _control.Focus();
            }

            // Set ourselves up in the host.
            Debug.Assert(_inPlaceFrame is not null, "Setting us to visible should have created the in place frame");

            using var activeObject = ComHelpers.GetComScope<IOleInPlaceActiveObject>(_control);

            using var inPlaceFrame = _inPlaceFrame.GetInterface();
            inPlaceFrame.Value->SetActiveObject(activeObject, (PCWSTR)null);
            using var inPlaceUiWindow = _inPlaceUiWindow is null ? default : _inPlaceUiWindow.GetInterface();
            if (_inPlaceUiWindow is not null)
            {
                inPlaceUiWindow.Value->SetActiveObject(activeObject, (PCWSTR)null);
            }

            // We have to explicitly say we don't wany any border space.
            hr = inPlaceFrame.Value->SetBorderSpace(null);
            if (hr.Failed
                && hr != HRESULT.OLE_E_INVALIDRECT
                && hr != HRESULT.INPLACE_E_NOTOOLSPACE
                && hr != HRESULT.E_NOTIMPL)
            {
                hr.ThrowOnFailure();
            }

            if (_inPlaceUiWindow is not null)
            {
                hr = inPlaceUiWindow.Value->SetBorderSpace(null);
                if (hr.Failed
                    && hr != HRESULT.OLE_E_INVALIDRECT
                    && hr != HRESULT.INPLACE_E_NOTOOLSPACE
                    && hr != HRESULT.E_NOTIMPL)
                {
                    hr.ThrowOnFailure();
                }
            }
        }

        /// <inheritdoc cref="IOleInPlaceObject.InPlaceDeactivate"/>
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

            // Some containers may call into us to save, and if we're still active we will try to deactivate and
            // recurse back into the container. So, set the state bits here first.
            _activeXState[s_inPlaceActive] = false;
            _activeXState[s_inPlaceVisible] = false;

            // Notify our site of our deactivation.
            if (_clientSite is not null)
            {
                using var inPlaceSite = _clientSite.TryGetInterface<IOleInPlaceSite>(out HRESULT hr);
                if (hr.Succeeded)
                {
                    inPlaceSite.Value->OnInPlaceDeactivate();
                }
            }

            _control.Visible = false;
            HWNDParent = default;

            DisposeHelper.NullAndDispose(ref _inPlaceUiWindow);
            DisposeHelper.NullAndDispose(ref _inPlaceFrame);

            return HRESULT.S_OK;
        }

        /// <inheritdoc cref="IPersistStorage.IsDirty"/>
        internal HRESULT IsDirty() => _activeXState[s_isDirty] ? HRESULT.S_OK : HRESULT.S_FALSE;

        /// <summary>
        ///  Looks at the property to see if it should be loaded / saved as a resource or through a type converter.
        /// </summary>
        private bool IsResourceProperty(PropertyDescriptor property)
        {
            TypeConverter converter = property.Converter;

            // If we have a converter round trip through string or byte[], it should be used
            if ((converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)))
                || (converter.CanConvertTo(typeof(byte[])) && converter.CanConvertFrom(typeof(byte))))
            {
                return false;
            }

            // Otherwise we require the type explicitly implements ISerializable. Strangely, in the past this only
            // worked off of the current value. If the current value was null checking it for ISerializable would always
            // fail. This means properties would never load into a reference type property if it's current value was null.
            //
            // While we could always just check the property type for serializable this would break derived class scenarios
            // where it adds ISerializable but the property type doesn't have it. In this scenario it would still not work
            // if the value is null on load. Not enabling that scenario for now as it would require more refactoring.
            return property.PropertyType.IsAssignableTo(typeof(ISerializable))
                || property.GetValue(_control) is ISerializable;
        }

        /// <inheritdoc cref="IPersistStorage.Load(IStorage*)"/>
        internal HRESULT Load(IStorage* stg)
        {
            using ComScope<IStream> stream = new(null);
            HRESULT hr = stg->OpenStream(
                GetStreamName(),
                STGM.STGM_READ | STGM.STGM_SHARE_EXCLUSIVE,
                0,
                stream);

            if (hr == HRESULT.STG_E_FILENOTFOUND)
            {
                // For backward compatibility: We were earlier using GetType().FullName
                // as the stream name in v1. Lets see if a stream by that name exists.
                hr = stg->OpenStream(
                    GetType().FullName!,
                    STGM.STGM_READ | STGM.STGM_SHARE_EXCLUSIVE,
                    0,
                    stream);
            }

            if (hr.Succeeded)
            {
                Load(stream);
            }

            return hr;
        }

        /// <inheritdoc cref="IPersistStreamInit.Load(IStream*)"/>
        internal void Load(IStream* stream)
        {
            // We do everything through property bags because we support full fidelity
            // in them. So, load through that method.
            PropertyBagStream bagStream = new();
            bagStream.Read(stream);
            using var propertyBag = ComHelpers.GetComScope<IPropertyBag>(bagStream);
            Load(propertyBag, errorLog: null);
        }

        /// <inheritdoc cref="IPersistPropertyBag.Load(IPropertyBag*, IErrorLog*)"/>
        internal unsafe void Load(IPropertyBag* propertyBag, IErrorLog* errorLog)
        {
            if (!IsSupported)
            {
                throw new NotSupportedException(string.Format(SR.ControlNotSupportedInTrimming, nameof(ActiveXImpl)));
            }

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(
                _control,
                [DesignerSerializationVisibilityAttribute.Visible]);

            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor currentProperty = props[i];

                try
                {
                    object? value = null;
                    using (VARIANT variant = default)
                    {
                        fixed (char* n = currentProperty.Name)
                        {
                            if (propertyBag->Read(n, &variant, errorLog).Failed)
                            {
                                continue;
                            }
                        }

                        value = variant.ToObject();
                    }

                    if (value is null)
                    {
                        continue;
                    }

                    string? errorString = null;
                    HRESULT errorCode = HRESULT.S_OK;

                    try
                    {
                        if (!SetValue(currentProperty, value))
                        {
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        errorString = e.ToString();
                        errorCode = e is ExternalException ee ? (HRESULT)ee.ErrorCode : HRESULT.E_FAIL;
                    }

                    if (errorString is not null && errorLog is not null)
                    {
                        using BSTR bstrSource = new(_control.GetType().FullName!);
                        using BSTR bstrDescription = new(errorString);
                        EXCEPINFO err = new()
                        {
                            bstrSource = bstrSource,
                            bstrDescription = bstrDescription,
                            scode = errorCode
                        };

                        errorLog->AddError(currentProperty.Name, in err);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Fail("Unexpected failure reading property", ex.ToString());

                    if (ex.IsCriticalException())
                    {
                        throw;
                    }
                }
            }

            bool SetValue(PropertyDescriptor currentProperty, object data)
            {
                string? value = data as string ?? Convert.ToString(data, CultureInfo.InvariantCulture);

                if (value is null)
                {
                    Debug.Fail($"Couldn't convert {currentProperty.Name} to string.");
                    return false;
                }

                if (IsResourceProperty(currentProperty))
                {
                    // Resource property. We encode these as base 64 strings. To load them, we convert
                    // to a binary blob and then de-serialize.
                    using MemoryStream stream = new(Convert.FromBase64String(value), writable: false);
                    bool success = false;
                    object? deserialized = null;
                    try
                    {
                        SerializationRecord rootRecord = stream.Decode();
                        success = rootRecord.TryGetResXObject(out deserialized);
                    }
                    catch (Exception ex) when (!ex.IsCriticalException())
                    {
                    }

                    if (!success)
                    {
                        if (!DataObject.Composition.EnableUnsafeBinaryFormatterInNativeObjectSerialization)
                        {
                            throw new NotSupportedException(SR.BinaryFormatterNotSupported);
                        }

                        stream.Position = 0;

#pragma warning disable SYSLIB0011 // Type or member is obsolete
#pragma warning disable CA2300 // Do not use insecure deserializer BinaryFormatter
#pragma warning disable CA2301 // Do not call BinaryFormatter.Deserialize without first setting BinaryFormatter.Binder
                        deserialized = new BinaryFormatter().Deserialize(stream); // CodeQL[SM03722, SM04191] : BinaryFormatter is intended to be used as a fallback for unsupported types. Users must explicitly opt into this behavior
#pragma warning restore CA2301
#pragma warning restore CA2300
#pragma warning restore SYSLIB0011
                    }

                    currentProperty.SetValue(_control, deserialized);
                    return true;
                }

                // Not a resource property. Use TypeConverters to convert the string back to the data type. We do
                // not check for CanConvertFrom here -- we the conversion fails the type converter will throw,
                // and we will log it into the COM error log.
                TypeConverter converter = currentProperty.Converter;
                Debug.Assert(
                    converter is not null,
                    $"No type converter for property '{currentProperty.Name}' on class {_control.GetType().FullName}");

                // Check to see if the type converter can convert from a string. If it can,.
                // use that as it is the best format for IPropertyBag. Otherwise, check to see
                // if it can convert from a byte array. If it can, get the string, decode it
                // to a byte array, and then set the value.
                object? newValue = null;

                if (converter.CanConvertFrom(typeof(string)))
                {
                    newValue = converter.ConvertFromInvariantString(value);
                }
                else if (converter.CanConvertFrom(typeof(byte[])))
                {
                    newValue = converter.ConvertFrom(null, CultureInfo.InvariantCulture, FromBase64WrappedString(value));
                }

                currentProperty.SetValue(_control, newValue);
                return true;
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

            Debug.Fail($"No ambient property for dispid {dispid}");
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

        private static void CallParentPropertyChanged(Control control, string propName)
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
                    Debug.Fail($"There is no property change notification for: {propName} on Control.");
                    break;
            }
        }

        /// <inheritdoc cref="IOleControl.OnAmbientPropertyChange(int)"/>
        internal void OnAmbientPropertyChange(int dispID)
        {
            if (dispID == PInvokeCore.DISPID_UNKNOWN)
            {
                // Invalidate all properties. Ideally we should be checking each one, but
                // that's pretty expensive too.
                for (int i = 0; i < _ambientProperties.Length; i++)
                {
                    _ambientProperties[i].ResetValue();
                    CallParentPropertyChanged(_control, _ambientProperties[i].Name);
                }

                return;
            }

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
            switch (dispID)
            {
                case PInvokeCore.DISPID_AMBIENT_UIDEAD:
                    using (VARIANT value = GetAmbientProperty(PInvokeCore.DISPID_AMBIENT_UIDEAD))
                    {
                        if (value.vt == VARENUM.VT_BOOL)
                        {
                            _activeXState[s_uiDead] = value.data.boolVal == VARIANT_BOOL.VARIANT_TRUE;
                        }
                    }

                    break;

                case PInvokeCore.DISPID_AMBIENT_DISPLAYASDEFAULT:
                    if (_control is IButtonControl ibuttonControl)
                    {
                        using VARIANT value = GetAmbientProperty(PInvokeCore.DISPID_AMBIENT_DISPLAYASDEFAULT);
                        if (value.vt == VARENUM.VT_BOOL)
                        {
                            ibuttonControl.NotifyDefault(value.data.boolVal == VARIANT_BOOL.VARIANT_TRUE);
                        }
                    }

                    break;
            }
        }

        /// <inheritdoc cref="IOleInPlaceActiveObject.OnDocWindowActivate(BOOL)"/>
        internal void OnDocWindowActivate(BOOL fActivate)
        {
            if (_activeXState[s_uiActive] && fActivate && _inPlaceFrame is not null)
            {
                // We have to explicitly say we don't wany any border space.
                using var inPlaceFrame = _inPlaceFrame.GetInterface();
                HRESULT hr = inPlaceFrame.Value->SetBorderSpace(null);
                if (!hr.Succeeded && hr != HRESULT.INPLACE_E_NOTOOLSPACE && hr != HRESULT.E_NOTIMPL)
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
            if (_activeXState[s_inPlaceActive] && _clientSite is not null)
            {
                using var controlSite = _clientSite.TryGetInterface<IOleControlSite>(out HRESULT hr);
                if (hr.Succeeded)
                {
                    controlSite.Value->OnFocus(focus);
                }
            }

            if (focus && _activeXState[s_inPlaceActive] && !_activeXState[s_uiActive])
            {
                InPlaceActivate(OLEIVERB.OLEIVERB_UIACTIVATE);
            }
        }

        /// <summary>
        ///  Converts coordinates in pixels to HiMetric.
        /// </summary>
        private static Point PixelToHiMetric(int x, int y) => new()
        {
            X = (HiMetricPerInch * x + (LogPixels.X >> 1)) / LogPixels.X,
            Y = (HiMetricPerInch * y + (LogPixels.Y >> 1)) / LogPixels.Y
        };

        /// <inheritdoc cref="IQuickActivate.QuickActivate(QACONTAINER*, QACONTROL*)"/>
        internal unsafe HRESULT QuickActivate(QACONTAINER* pQaContainer, QACONTROL* pQaControl)
        {
            if (pQaControl is null)
            {
                return HRESULT.E_FAIL;
            }

            // Hookup our ambient colors
            AmbientProperty prop = LookupAmbient(PInvokeCore.DISPID_AMBIENT_BACKCOLOR);
            prop.Value = ColorTranslator.FromOle((int)pQaContainer->colorBack);

            prop = LookupAmbient(PInvokeCore.DISPID_AMBIENT_FORECOLOR);
            prop.Value = ColorTranslator.FromOle((int)pQaContainer->colorFore);

            // And our ambient font
            if (pQaContainer->pFont is not null)
            {
                prop = LookupAmbient(PInvokeCore.DISPID_AMBIENT_FONT);

                try
                {
                    prop.Value = Font.FromHfont(pQaContainer->pFont->hFont);
                }
                catch (Exception e) when (!e.IsCriticalException())
                {
                    // Do NULL, so we just defer to the default font
                    prop.Value = null;
                }
            }

            // Now use the rest of the goo that we got passed in.
            pQaControl->cbSize = (uint)sizeof(QACONTROL);

            if (pQaContainer->pClientSite is not null)
            {
                SetClientSite(pQaContainer->pClientSite);
            }

            if (pQaContainer->pAdviseSink is not null)
            {
                SetAdvise(DVASPECT.DVASPECT_CONTENT, 0, (IAdviseSink*)pQaContainer->pAdviseSink);
            }

            _control.GetMiscStatus(DVASPECT.DVASPECT_CONTENT, out OLEMISC status);
            pQaControl->dwMiscStatus = status;

            // Advise the event sink so Visual Basic 6 can catch events raised from UserControls. VB6 expects the control
            // to do this during IQuickActivate, otherwise it will not hook events at runtime. We will do this if all of
            // the following are true:
            //
            //  1. The container (e.g., VB6) has supplied an event sink
            //  2. The control is a UserControl (this is only to limit the scope of the changed behavior)
            //  3. The UserControl has indicated it wants to expose events to COM via the ComSourceInterfacesAttribute

            if ((pQaContainer->pUnkEventSink is not null) && (_control is UserControl))
            {
                // Check if this control exposes events to COM.
                if (GetDefaultEventsInterface(_control.GetType()) is { } eventInterface)
                {
                    // Control doesn't explicitly implement IConnectionPointContainer, but it is generated with a CCW by
                    // COM interop.

                    using var container = ComHelpers.TryGetComScope<IConnectionPointContainer>(_control, out HRESULT hr);

                    hr.AssertSuccess();

                    using ComScope<IConnectionPoint> connectionPoint = new(null);
                    if (hr.Failed || container.Value->FindConnectionPoint(eventInterface.GUID, connectionPoint).Failed)
                    {
                        throw new ArgumentException(string.Format(SR.AXNoConnectionPoint, eventInterface.Name));
                    }

                    hr = connectionPoint.Value->Advise(pQaContainer->pUnkEventSink, out pQaControl->dwEventCookie);
                    Debug.WriteLineIf(hr.Failed, $"Failed to advise with {eventInterface.Name}.");
                }
            }

            return HRESULT.S_OK;

            // Get the default COM events interface declared on a .NET class.
            static Type? GetDefaultEventsInterface(Type controlType)
            {
                Type? eventInterface = null;

                if (!IsSupported)
                {
                    throw new NotSupportedException(string.Format(SR.ControlNotSupportedInTrimming, nameof(ActiveXImpl)));
                }

                // Get the first declared interface, if any.
                if (controlType.GetCustomAttributes<ComSourceInterfacesAttribute>(inherit: false).FirstOrDefault()
                    is { } comSourceInterfaces)
                {
                    string eventName = comSourceInterfaces.Value.AsSpan().SliceAtFirstNull().ToString();
                    eventInterface = controlType.Module.Assembly.GetType(eventName, throwOnError: false);
                    eventInterface ??= Type.GetType(eventName, throwOnError: false);
                }

                return eventInterface;
            }
        }

        /// <inheritdoc cref="IPersistStorage.Save(IStorage*, BOOL)"/>
        internal HRESULT Save(IStorage* storage, BOOL fSameAsLoad)
        {
            using ComScope<IStream> stream = new(null);
            HRESULT hr = storage->CreateStream(
                GetStreamName(),
                STGM.STGM_WRITE | STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_CREATE,
                0,
                0,
                stream);

            Debug.Assert(hr.Succeeded, "Stream should be non-null.");

            if (hr.Succeeded)
            {
                Save(stream, fClearDirty: true);
            }

            return hr;
        }

        /// <inheritdoc cref="IPersistStreamInit.Save(IStream*, BOOL)"/>
        internal void Save(IStream* stream, BOOL fClearDirty)
        {
            // We do everything through property bags because we support full fidelity in them.
            // So, save through that method.
            PropertyBagStream bagStream = new();

            using var propertyBag = ComHelpers.GetComScope<IPropertyBag>(bagStream);
            Save(propertyBag, fClearDirty, saveAllProperties: false);
            bagStream.Write(stream);
        }

        /// <inheritdoc cref="IPersistPropertyBag.Save(IPropertyBag*, BOOL, BOOL)"/>
        internal void Save(IPropertyBag* propertyBag, BOOL clearDirty, BOOL saveAllProperties)
        {
            if (!IsSupported)
            {
                throw new NotSupportedException(string.Format(SR.ControlNotSupportedInTrimming, nameof(ActiveXImpl)));
            }

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(
                _control,
                [DesignerSerializationVisibilityAttribute.Visible]);

            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor currentProperty = props[i];
                if (!saveAllProperties && !currentProperty.ShouldSerializeValue(_control))
                {
                    continue;
                }

                string? value = null;

                if (IsResourceProperty(currentProperty))
                {
                    // Resource property. Save this to the bag as a 64bit encoded string.
                    using MemoryStream stream = new();
                    object sourceValue = currentProperty.GetValue(_control)!;
                    bool success = false;

                    try
                    {
                        success = WinFormsBinaryFormatWriter.TryWriteObject(stream, sourceValue);
                    }
                    catch (Exception ex) when (!ex.IsCriticalException())
                    {
                        Debug.Fail($"Failed to write with BinaryFormatWriter: {ex.Message}");
                    }

                    if (!success)
                    {
                        stream.SetLength(0);

                        if (!DataObject.Composition.EnableUnsafeBinaryFormatterInNativeObjectSerialization)
                        {
                            throw new NotSupportedException(SR.BinaryFormatterNotSupported);
                        }

#pragma warning disable SYSLIB0011 // Type or member is obsolete
                        new BinaryFormatter().Serialize(stream, sourceValue);
#pragma warning restore
                    }

                    using VARIANT data = (VARIANT)new BSTR(Convert.ToBase64String(
                        new ReadOnlySpan<byte>(stream.GetBuffer(), 0, (int)stream.Length)));
                    propertyBag->Write(currentProperty.Name, data);
                    continue;
                }

                // Not a resource property. Persist this using standard type converters.
                TypeConverter converter = currentProperty.Converter;
                Debug.Assert(
                    converter is not null,
                    $"No type converter for property '{currentProperty.Name}' on class {_control.GetType().FullName}");

                if (converter.CanConvertFrom(typeof(string)))
                {
                    value = converter.ConvertToInvariantString(currentProperty.GetValue(_control));
                }
                else if (converter.CanConvertFrom(typeof(byte[])))
                {
                    byte[] data = (byte[])converter.ConvertTo(
                        context: null,
                        CultureInfo.InvariantCulture,
                        currentProperty.GetValue(_control),
                        typeof(byte[]))!;

                    value = Convert.ToBase64String(data);
                }

                if (value is not null)
                {
                    using VARIANT variant = (VARIANT)(new BSTR(value));
                    fixed (char* pszPropName = currentProperty.Name)
                    {
                        propertyBag->Write(pszPropName, &variant);
                    }
                }
            }

            if (clearDirty)
            {
                _activeXState[s_isDirty] = false;
            }
        }

        /// <summary>
        ///  Fires the OnSave event to all of our IAdviseSink listeners. Used for ActiveXSourcing.
        /// </summary>
        private void SendOnSave()
        {
            if (_adviseHolder is null)
            {
                return;
            }

            using var holder = _adviseHolder.GetInterface();
            holder.Value->SendOnSave();
        }

        /// <inheritdoc cref="IViewObject.SetAdvise(DVASPECT, uint, IAdviseSink*)"/>
        internal HRESULT SetAdvise(DVASPECT aspects, ADVF advf, IAdviseSink* pAdvSink)
        {
            // If it's not a content aspect, we don't support it.
            if (!aspects.HasFlag(DVASPECT.DVASPECT_CONTENT))
            {
                return HRESULT.DV_E_DVASPECT;
            }

            // Set up some flags to return from GetAdvise.
            _activeXState[s_viewAdvisePrimeFirst] = advf.HasFlag(ADVF.ADVF_PRIMEFIRST);
            _activeXState[s_viewAdviseOnlyOnce] = advf.HasFlag(ADVF.ADVF_ONLYONCE);

            DisposeHelper.NullAndRelease(ref _viewAdviseSink);

            if (pAdvSink is not null)
            {
                pAdvSink->AddRef();
            }

            _viewAdviseSink = pAdvSink;

            // Prime them if they want it [we need to store this so they can get flags later]
            if (_activeXState[s_viewAdvisePrimeFirst])
            {
                ViewChanged();
            }

            return HRESULT.S_OK;
        }

        /// <inheritdoc cref="IOleObject.SetClientSite(IOleClientSite*)"/>
        internal void SetClientSite(IOleClientSite* value)
        {
            DisposeHelper.NullAndDispose(ref _clientSite);

            if (value is not null)
            {
                // Callers don't increment the ref count when they pass IOleClientSite, it is up to us to do so as we're
                // maintaining a reference to the pointer. Validated this behavior with the ATL/MFC sources.
                //
                // https://learn.microsoft.com/windows/win32/api/oleidl/nf-oleidl-ioleobject-setclientsite#notes-to-implementers

                value->AddRef();
                _clientSite = new(value, takeOwnership: true);
                _control.Site = new AxSourcingSite(_control, _clientSite, "ControlAxSourcingSite");
            }
            else
            {
                _control.Site = null;
            }

            // Get the ambient properties that effect us.
            using VARIANT property = GetAmbientProperty(PInvokeCore.DISPID_AMBIENT_UIDEAD);
            if (property.vt == VARENUM.VT_BOOL)
            {
                bool uiDead = property.data.boolVal == VARIANT_BOOL.VARIANT_TRUE;
                _activeXState[s_uiDead] = uiDead;

                if (_control is IButtonControl buttonControl)
                {
                    buttonControl.NotifyDefault(uiDead);
                }
            }

            if (_clientSite is null && !_accelTable.IsNull)
            {
                PInvoke.DestroyAcceleratorTable(new HandleRef<HACCEL>(_control, _accelTable));
                _accelTable = HACCEL.Null;
                _accelCount = -1;
            }

            _control.OnTopMostActiveXParentChanged(EventArgs.Empty);
        }

        /// <inheritdoc cref="IOleObject.SetExtent(DVASPECT, SIZE*)"/>
        internal unsafe void SetExtent(DVASPECT dwDrawAspect, Size* pSizel)
        {
            if (!dwDrawAspect.HasFlag(DVASPECT.DVASPECT_CONTENT))
            {
                // We don't support any other aspects
                ThrowHr(HRESULT.DV_E_DVASPECT);
            }

            if (_activeXState[s_changingExtents])
            {
                return;
            }

            _activeXState[s_changingExtents] = true;

            try
            {
                Size size = new(HiMetricToPixel(pSizel->Width, pSizel->Height));

                // If we're in place active, let the in place site set our bounds.
                // Otherwise, just set it on our control directly.
                if (_activeXState[s_inPlaceActive] && _clientSite is not null)
                {
                    using var clientSite = _clientSite.TryGetInterface<IOleInPlaceSite>(out HRESULT hr);
                    if (hr.Succeeded)
                    {
                        Rectangle bounds = _control.Bounds;
                        bounds.Location = new Point(bounds.X, bounds.Y);
                        Size adjusted = new(size.Width, size.Height);
                        bounds.Width = adjusted.Width;
                        bounds.Height = adjusted.Height;
                        Debug.Assert(_clientSite is not null, "How can we setextent before we are sited??");

                        RECT posRect = bounds;
                        clientSite.Value->OnPosRectChange(&posRect);
                    }
                }

                _control.Size = size;

                // Check to see if the control overwrote our size with its own values.
                if (_control.Size.Equals(size))
                {
                    return;
                }

                _activeXState[s_isDirty] = true;

                // If we're not inplace active, then announce that the view changed.
                if (!_activeXState[s_inPlaceActive])
                {
                    ViewChanged();
                }

                // We need to call RequestNewObjectLayout here so we visually display our new extents.
                if (!_activeXState[s_inPlaceActive] && _clientSite is not null)
                {
                    using var clientSite = _clientSite.GetInterface();
                    clientSite.Value->RequestNewObjectLayout();
                }
            }
            finally
            {
                _activeXState[s_changingExtents] = false;
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

        /// <inheritdoc cref="IOleInPlaceObject.SetObjectRects(RECT*, RECT*)"/>
        internal unsafe HRESULT SetObjectRects(RECT* lprcPosRect, RECT* lprcClipRect)
        {
            if (lprcPosRect is null || lprcClipRect is null)
            {
                return HRESULT.E_INVALIDARG;
            }

            Rectangle posRect = *lprcPosRect;

            // ActiveX expects to be notified when a control's bounds change, and also
            // intends to notify us through SetObjectRects when we report that the
            // bounds are about to change. We implement this all on a control's Bounds
            // property, which doesn't use this callback mechanism. The adjustRect
            // member handles this. If it is non-null, then we are being called in
            // response to an OnPosRectChange call. In this case we do not
            // set the control bounds but set the bounds on the adjustRect. When
            // this returns from the container and comes back to our OnPosRectChange
            // implementation, these new bounds will be handed back to the control
            // for the actual window change.
            if (_activeXState[s_adjustingRect])
            {
                *_adjustRect = posRect;
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

            if (lprcClipRect is not null)
            {
                // The container wants us to clip, so figure out if we really need to.
                Rectangle clipRect = *lprcClipRect;

                // Trident always sends an empty ClipRect, don't do an intersect in that case.
                Rectangle intersect = !clipRect.IsEmpty ? Rectangle.Intersect(posRect, clipRect) : posRect;

                if (!intersect.Equals(posRect))
                {
                    // Offset the rectangle back to client coordinates
                    RECT rcIntersect = intersect;
                    HWND hWndParent = PInvoke.GetParent(_control);

                    PInvokeCore.MapWindowPoints(hWndParent, _control, ref rcIntersect);

                    _lastClipRect = rcIntersect;
                    setRegion = true;
                }
            }

            // If our region has changed, set the new value. We only do this if
            // the handle has been created, since otherwise the control will
            // merge our region automatically.
            if (setRegion)
            {
                _control.SetRegionInternal(_control.Region);
            }

            // Forms^3 uses transparent overlay windows that appear to cause painting artifacts.
            _control.Invalidate();

            return HRESULT.S_OK;
        }

        /// <summary>
        ///  Throws the given hresult. This is used by ActiveX sourcing.
        /// </summary>
        [DoesNotReturn]
        internal static void ThrowHr(HRESULT hr) => throw new ExternalException(SR.ExternalException, (int)hr);

        /// <inheritdoc cref="IOleInPlaceActiveObject.TranslateAccelerator(MSG*)"/>
        internal unsafe HRESULT TranslateAccelerator(MSG* lpmsg)
        {
            if (lpmsg is null)
            {
                return HRESULT.E_POINTER;
            }

            bool needPreProcess = false;
            switch (lpmsg->message)
            {
                case PInvokeCore.WM_KEYDOWN:
                case PInvokeCore.WM_SYSKEYDOWN:
                case PInvokeCore.WM_CHAR:
                case PInvokeCore.WM_SYSCHAR:
                    needPreProcess = true;
                    break;
            }

            Message msg = Message.Create(lpmsg->hwnd, lpmsg->message, lpmsg->wParam, lpmsg->lParam);
            if (needPreProcess)
            {
                Control? target = FromChildHandle(lpmsg->hwnd);
                if (_control == target || _control.Contains(target))
                {
                    PreProcessControlState messageState = PreProcessControlMessageInternal(target, ref msg);
                    switch (messageState)
                    {
                        case PreProcessControlState.MessageProcessed:
                            // someone returned true from PreProcessMessage
                            // no need to dispatch the message, its already been coped with.
                            lpmsg->message = (uint)msg.MsgInternal;
                            lpmsg->wParam = msg.WParamInternal;
                            lpmsg->lParam = msg.LParamInternal;
                            return HRESULT.S_OK;
                        case PreProcessControlState.MessageNeeded:
                            // Here we need to dispatch the message ourselves
                            // otherwise the host may never send the key to our wndproc.

                            // Someone returned true from IsInputKey or IsInputChar
                            PInvoke.TranslateMessage(lpmsg);
                            if (PInvoke.IsWindowUnicode(lpmsg->hwnd))
                            {
                                PInvoke.DispatchMessage(lpmsg);
                            }
                            else
                            {
                                PInvoke.DispatchMessageA(lpmsg);
                            }

                            return HRESULT.S_OK;
                        case PreProcessControlState.MessageNotNeeded:
                            // in this case we'll check the site to see if it wants the message.
                            break;
                    }
                }
            }

            // SITE processing. We're not interested in the message, but the site may be.
            if (_clientSite is null)
            {
                return HRESULT.S_FALSE;
            }

            using var controlSite = _clientSite.TryGetInterface<IOleControlSite>(out HRESULT hr);
            if (hr.Failed)
            {
                return HRESULT.S_FALSE;
            }

            KEYMODIFIERS keyState = 0;
            if (PInvoke.GetKeyState((int)VIRTUAL_KEY.VK_SHIFT) < 0)
            {
                keyState |= KEYMODIFIERS.KEYMOD_SHIFT;
            }

            if (PInvoke.GetKeyState((int)VIRTUAL_KEY.VK_CONTROL) < 0)
            {
                keyState |= KEYMODIFIERS.KEYMOD_CONTROL;
            }

            if (PInvoke.GetKeyState((int)VIRTUAL_KEY.VK_MENU) < 0)
            {
                keyState |= KEYMODIFIERS.KEYMOD_ALT;
            }

            return controlSite.Value->TranslateAccelerator(lpmsg, keyState);
        }

        /// <inheritdoc cref="IOleInPlaceObject.UIDeactivate"/>
        internal HRESULT UIDeactivate()
        {
            // Only do this if we're UI active
            if (!_activeXState[s_uiActive])
            {
                return HRESULT.S_OK;
            }

            _activeXState[s_uiActive] = false;

            // Notify frame windows, if appropriate, that we're no longer ui-active.
            if (_inPlaceUiWindow is not null)
            {
                using var window = _inPlaceUiWindow.GetInterface();
                window.Value->SetActiveObject(null, (PCWSTR)null);
            }

            // May need this for SetActiveObject & OnUIDeactivate, so leave until function return.
            Debug.Assert(_inPlaceFrame is not null, "No inplace frame -- how did we go UI active?");

            if (_inPlaceFrame is not null)
            {
                using var frame = _inPlaceFrame.GetInterface();
                frame.Value->SetActiveObject(null, (PCWSTR)null);
            }

            if (_clientSite is not null)
            {
                using var inPlaceSite = _clientSite.TryGetInterface<IOleInPlaceSite>(out HRESULT hr);
                if (hr.Succeeded)
                {
                    inPlaceSite.Value->OnUIDeactivate(fUndoable: BOOL.FALSE);
                }
            }

            return HRESULT.S_OK;
        }

        /// <inheritdoc cref="IOleObject.Unadvise(uint)"/>
        internal HRESULT Unadvise(uint dwConnection)
        {
            if (_adviseHolder is null)
            {
                return HRESULT.E_FAIL;
            }

            using var holder = _adviseHolder.GetInterface();
            return holder.Value->Unadvise(dwConnection);
        }

        /// <summary>
        ///  Notifies our site that we have changed our size and location.
        /// </summary>
        internal unsafe void UpdateBounds(ref int x, ref int y, ref int width, ref int height, SET_WINDOW_POS_FLAGS flags)
        {
            if (_activeXState[s_adjustingRect] || !_activeXState[s_inPlaceVisible] || _clientSite is null)
            {
                return;
            }

            using var inPlaceSite = _clientSite.TryGetInterface<IOleInPlaceSite>(out HRESULT hr);
            if (hr.Failed)
            {
                return;
            }

            RECT rect = default;
            if (flags.HasFlag(SET_WINDOW_POS_FLAGS.SWP_NOMOVE))
            {
                rect.left = _control.Left;
                rect.top = _control.Top;
            }
            else
            {
                rect.left = x;
                rect.top = y;
            }

            if (flags.HasFlag(SET_WINDOW_POS_FLAGS.SWP_NOSIZE))
            {
                rect.right = rect.left + _control.Width;
                rect.bottom = rect.top + _control.Height;
            }
            else
            {
                rect.right = rect.left + width;
                rect.bottom = rect.top + height;
            }

            // This member variable may be modified by SetObjectRects by the container.
            _adjustRect = &rect;
            _activeXState[s_adjustingRect] = true;

            try
            {
                inPlaceSite.Value->OnPosRectChange(&rect);
            }
            finally
            {
                _adjustRect = null;
                _activeXState[s_adjustingRect] = false;
            }

            // On output, the new bounds will be reflected in  rc
            if (!flags.HasFlag(SET_WINDOW_POS_FLAGS.SWP_NOMOVE))
            {
                x = rect.left;
                y = rect.top;
            }

            if (!flags.HasFlag(SET_WINDOW_POS_FLAGS.SWP_NOSIZE))
            {
                width = rect.right - rect.left;
                height = rect.bottom - rect.top;
            }
        }

        /// <summary>
        ///  Notifies that the accelerator table needs to be updated due to a change in a control mnemonic.
        /// </summary>
        internal void UpdateAccelTable()
        {
            // Setting the count to -1 will recreate the table on demand (when GetControlInfo is called).
            _accelCount = -1;

            if (_clientSite is null)
            {
                return;
            }

            using var controlSite = _clientSite.TryGetInterface<IOleControlSite>(out HRESULT hr);
            if (hr.Failed)
            {
                return;
            }

            controlSite.Value->OnControlInfoChanged();
        }

        // This method is used by Reflection- don't change the signature.
        internal void ViewChangedInternal() => ViewChanged();

        /// <summary>
        ///  Notifies our view advise sink (if it exists) that the view has changed.
        /// </summary>
        private void ViewChanged()
        {
            // Send the view change notification to anybody listening.
            //
            // Note: Word2000 won't resize components correctly if an OnViewChange notification is sent while the
            // component is persisting it's state. The !s_saving check is to make sure we don't call OnViewChange
            // in this case.

            if (_viewAdviseSink is null || _activeXState[s_saving])
            {
                return;
            }

            _viewAdviseSink->OnViewChange((int)DVASPECT.DVASPECT_CONTENT, -1);

            if (_activeXState[s_viewAdviseOnlyOnce])
            {
                DisposeHelper.NullAndRelease(ref _viewAdviseSink);
            }
        }

        /// <summary>
        ///  Called when the window handle of the control has changed.
        /// </summary>
        void IWindowTarget.OnHandleChange(IntPtr newHandle) => _controlWindowTarget.OnHandleChange(newHandle);

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

                if (m.Msg is >= ((int)PInvokeCore.WM_NCLBUTTONDOWN) and <= ((int)PInvokeCore.WM_NCMBUTTONDBLCLK))
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

        public void Dispose()
        {
            // Disposing the client site handle can get us called back with SetClientSite(null). We need to
            // make sure that we clear the field before disposing it. To avoid similar problems, we do the same
            // pattern for every COM pointer.

            DisposeHelper.NullAndDispose(ref _inPlaceFrame);
            DisposeHelper.NullAndDispose(ref _inPlaceUiWindow);
            DisposeHelper.NullAndDispose(ref _clientSite);
            DisposeHelper.NullAndDispose(ref _adviseHolder);
        }
    }
}

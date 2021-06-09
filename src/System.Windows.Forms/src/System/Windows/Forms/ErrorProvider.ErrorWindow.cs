// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    partial class ErrorProvider
    {
        /// <summary>
        ///  There is one ErrorWindow for each control parent. It is parented to the
        ///  control parent. The window's region is made up of the regions from icons
        ///  of all child icons. The window's size is the enclosing rectangle for all
        ///  the regions. A tooltip window is created as a child of this window. The
        ///  rectangle associated with each error icon being displayed is added as a
        ///  tool to the tooltip window.
        /// </summary>
        internal partial class ErrorWindow : NativeWindow
        {
            private static readonly int s_accessibilityProperty = PropertyStore.CreateKey();

            private readonly List<ControlItem> _items = new List<ControlItem>();
            private readonly Control _parent;
            private readonly ErrorProvider _provider;
            private Rectangle _windowBounds;
            private Timer _timer;
            private NativeWindow _tipWindow;

            /// <summary>
            ///  Construct an error window for this provider and control parent.
            /// </summary>
            public ErrorWindow(ErrorProvider provider, Control parent)
            {
                _provider = provider;
                _parent = parent;
                Properties = new PropertyStore();
            }

            /// <summary>
            ///  The Accessibility Object for this ErrorProvider
            /// </summary>
            internal AccessibleObject AccessibilityObject
            {
                get
                {
                    AccessibleObject accessibleObject = (AccessibleObject)Properties.GetObject(s_accessibilityProperty);

                    if (accessibleObject is null)
                    {
                        accessibleObject = CreateAccessibilityInstance();
                        Properties.SetObject(s_accessibilityProperty, accessibleObject);
                    }

                    return accessibleObject;
                }
            }

            /// <summary>
            ///  This is called when a control would like to show an error icon.
            /// </summary>
            public void Add(ControlItem item)
            {
                _items.Add(item);
                if (!EnsureCreated())
                {
                    return;
                }

                var toolInfo = new ComCtl32.ToolInfoWrapper<ErrorWindow>(this, item.Id, ComCtl32.TTF.SUBCLASS, item.Error);
                toolInfo.SendMessage(_tipWindow, (User32.WM)ComCtl32.TTM.ADDTOOLW);

                Update(timerCaused: false);
            }

            internal List<ControlItem> ControlItems => _items;

            /// <summary>
            ///  Constructs the new instance of the accessibility object for this ErrorProvider. Subclasses
            ///  should not call base.CreateAccessibilityObject.
            /// </summary>
            private AccessibleObject CreateAccessibilityInstance()
            {
                return new ErrorWindowAccessibleObject(this);
            }

            /// <summary>
            ///  Called to get rid of any resources the Object may have.
            /// </summary>
            public void Dispose() => EnsureDestroyed();

            /// <summary>
            ///  Make sure the error window is created, and the tooltip window is created.
            /// </summary>
            bool EnsureCreated()
            {
                if (Handle == IntPtr.Zero)
                {
                    if (!_parent.IsHandleCreated)
                    {
                        return false;
                    }

                    CreateParams cparams = new CreateParams
                    {
                        Caption = string.Empty,
                        Style = (int)(User32.WS.VISIBLE | User32.WS.CHILD),
                        ClassStyle = (int)User32.CS.DBLCLKS,
                        X = 0,
                        Y = 0,
                        Width = 0,
                        Height = 0,
                        Parent = _parent.Handle
                    };

                    CreateHandle(cparams);

                    var icc = new ComCtl32.INITCOMMONCONTROLSEX
                    {
                        dwICC = ComCtl32.ICC.TAB_CLASSES
                    };
                    ComCtl32.InitCommonControlsEx(ref icc);

                    cparams = new CreateParams
                    {
                        Parent = Handle,
                        ClassName = ComCtl32.WindowClasses.TOOLTIPS_CLASS,
                        Style = (int)ComCtl32.TTS.ALWAYSTIP
                    };
                    _tipWindow = new NativeWindow();
                    _tipWindow.CreateHandle(cparams);

                    User32.SendMessageW(_tipWindow, (User32.WM)ComCtl32.TTM.SETMAXTIPWIDTH, IntPtr.Zero, (IntPtr)SystemInformation.MaxWindowTrackSize.Width);
                    User32.SetWindowPos(
                        new HandleRef(_tipWindow, _tipWindow.Handle),
                        User32.HWND_TOP,
                        flags: User32.SWP.NOSIZE | User32.SWP.NOMOVE | User32.SWP.NOACTIVATE);
                    User32.SendMessageW(_tipWindow, (User32.WM)ComCtl32.TTM.SETDELAYTIME, (IntPtr)ComCtl32.TTDT.INITIAL, (IntPtr)0);
                }

                return true;
            }

            /// <summary>
            ///  Destroy the timer, toolwindow, and the error window itself.
            /// </summary>
            private void EnsureDestroyed()
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }

                if (_tipWindow != null)
                {
                    _tipWindow.DestroyHandle();
                    _tipWindow = null;
                }

                // Hide the window and invalidate the parent to ensure
                // that we leave no visual artifacts. given that we
                // have a bizarre region window, this is needed.
                User32.SetWindowPos(
                    new HandleRef(this, Handle),
                    User32.HWND_TOP,
                    _windowBounds.X,
                    _windowBounds.Y,
                    _windowBounds.Width,
                    _windowBounds.Height,
                    User32.SWP.HIDEWINDOW | User32.SWP.NOSIZE | User32.SWP.NOMOVE);
                _parent?.Invalidate(true);
                DestroyHandle();
            }

            private unsafe void MirrorDcIfNeeded(Gdi32.HDC hdc)
            {
                if (_parent.IsMirrored)
                {
                    // Mirror the DC
                    Gdi32.SetMapMode(hdc, Gdi32.MM.ANISOTROPIC);
                    Gdi32.GetViewportExtEx(hdc, out Size originalExtents);
                    Gdi32.SetViewportExtEx(hdc, -originalExtents.Width, originalExtents.Height, null);
                    Gdi32.GetViewportOrgEx(hdc, out Point originalOrigin);
                    Gdi32.SetViewportOrgEx(hdc, originalOrigin.X + _windowBounds.Width - 1, originalOrigin.Y, null);
                }
            }

            /// <summary>
            ///  This is called when the error window needs to paint. We paint each icon at its correct location.
            /// </summary>
            private unsafe void OnPaint()
            {
                using var hdc = new User32.BeginPaintScope(Handle);
                using var save = new Gdi32.SaveDcScope(hdc);

                MirrorDcIfNeeded(hdc);

                for (int i = 0; i < _items.Count; i++)
                {
                    ControlItem item = _items[i];
                    Rectangle bounds = item.GetIconBounds(_provider.Region.Size);
                    User32.DrawIconEx(
                        hdc,
                        bounds.X - _windowBounds.X,
                        bounds.Y - _windowBounds.Y,
                        _provider.Region,
                        bounds.Width, bounds.Height);
                }
            }

            protected override void OnThreadException(Exception e)
            {
                Application.OnThreadException(e);
            }

            /// <summary>
            ///  This is called when an error icon is flashing, and the view needs to be updated.
            /// </summary>
            private void OnTimer(object sender, EventArgs e)
            {
                int blinkPhase = 0;
                for (int i = 0; i < _items.Count; i++)
                {
                    blinkPhase += _items[i].BlinkPhase;
                }

                if (blinkPhase == 0 && _provider.BlinkStyle != ErrorBlinkStyle.AlwaysBlink)
                {
                    Debug.Assert(_timer != null);
                    _timer.Stop();
                }

                Update(timerCaused: true);
            }

            private void OnToolTipVisibilityChanging(IntPtr id, bool toolTipShown)
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    if (_items[i].Id == id)
                    {
                        _items[i].ToolTipShown = toolTipShown;
                    }
                }
#if DEBUG
                int shownTooltips = 0;
                for (int j = 0; j < _items.Count; j++)
                {
                    if (_items[j].ToolTipShown)
                    {
                        shownTooltips++;
                    }
                }

                Debug.Assert(shownTooltips <= 1);
#endif
            }

            /// <summary>
            ///  Retrieves our internal property storage object. If you have a property
            ///  whose value is not always set, you should store it in here to save
            ///  space.
            /// </summary>
            internal PropertyStore Properties { get; }

            /// <summary>
            ///  This is called when a control no longer needs to display an error icon.
            /// </summary>
            public void Remove(ControlItem item)
            {
                _items.Remove(item);

                if (_tipWindow != null)
                {
                    var info = new ComCtl32.ToolInfoWrapper<ErrorWindow>(this, item.Id);
                    info.SendMessage(_tipWindow, (User32.WM)ComCtl32.TTM.DELTOOLW);
                }

                if (_items.Count == 0)
                {
                    EnsureDestroyed();
                }
                else
                {
                    Update(timerCaused: false);
                }
            }

            /// <summary>
            ///  Start the blinking process. The timer will fire until there are no more
            ///  icons that need to blink.
            /// </summary>
            public void StartBlinking()
            {
                if (_timer is null)
                {
                    _timer = new Timer();
                    _timer.Tick += new EventHandler(OnTimer);
                }

                _timer.Interval = _provider.BlinkRate;
                _timer.Start();
                Update(timerCaused: false);
            }

            public void StopBlinking()
            {
                _timer?.Stop();
                Update(timerCaused: false);
            }

            /// <summary>
            ///  Move and size the error window, compute and set the window region, set the tooltip
            ///  rectangles and descriptions. This basically brings the error window up to date with
            ///  the internal data structures.
            /// </summary>
            public unsafe void Update(bool timerCaused)
            {
                IconRegion iconRegion = _provider.Region;
                Size size = iconRegion.Size;
                _windowBounds = Rectangle.Empty;
                for (int i = 0; i < _items.Count; i++)
                {
                    ControlItem item = _items[i];
                    Rectangle iconBounds = item.GetIconBounds(size);
                    if (_windowBounds.IsEmpty)
                    {
                        _windowBounds = iconBounds;
                    }
                    else
                    {
                        _windowBounds = Rectangle.Union(_windowBounds, iconBounds);
                    }
                }

                using var windowRegion = new Region(new Rectangle(0, 0, 0, 0));

                for (int i = 0; i < _items.Count; i++)
                {
                    ControlItem item = _items[i];
                    Rectangle iconBounds = item.GetIconBounds(size);
                    iconBounds.X -= _windowBounds.X;
                    iconBounds.Y -= _windowBounds.Y;

                    bool showIcon = true;
                    if (!item.ToolTipShown)
                    {
                        switch (_provider.BlinkStyle)
                        {
                            case ErrorBlinkStyle.NeverBlink:
                                // always show icon
                                break;
                            case ErrorBlinkStyle.BlinkIfDifferentError:
                                showIcon = (item.BlinkPhase == 0) || (item.BlinkPhase > 0 && (item.BlinkPhase & 1) == (i & 1));
                                break;
                            case ErrorBlinkStyle.AlwaysBlink:
                                showIcon = ((i & 1) == 0) == _provider._showIcon;
                                break;
                        }
                    }

                    if (showIcon)
                    {
                        iconRegion.Region.Translate(iconBounds.X, iconBounds.Y);
                        windowRegion.Union(iconRegion.Region);
                        iconRegion.Region.Translate(-iconBounds.X, -iconBounds.Y);
                    }

                    if (_tipWindow != null)
                    {
                        ComCtl32.TTF flags = ComCtl32.TTF.SUBCLASS;
                        if (_provider.RightToLeft)
                        {
                            flags |= ComCtl32.TTF.RTLREADING;
                        }

                        var toolInfo = new ComCtl32.ToolInfoWrapper<ErrorWindow>(this, item.Id, flags, item.Error, iconBounds);
                        toolInfo.SendMessage(_tipWindow, (User32.WM)ComCtl32.TTM.SETTOOLINFOW);
                    }

                    if (timerCaused && item.BlinkPhase > 0)
                    {
                        item.BlinkPhase--;
                    }
                }

                if (timerCaused)
                {
                    _provider._showIcon = !_provider._showIcon;
                }

                using var hdc = new User32.GetDcScope(Handle);
                using var save = new Gdi32.SaveDcScope(hdc);
                MirrorDcIfNeeded(hdc);

                using Graphics g = hdc.CreateGraphics();
                using var windowRegionHandle = new Gdi32.RegionScope(windowRegion, g);
                if (User32.SetWindowRgn(this, windowRegionHandle, BOOL.TRUE) != 0)
                {
                    // The HWnd owns the region.
                    windowRegionHandle.RelinquishOwnership();
                }

                User32.SetWindowPos(
                    new HandleRef(this, Handle),
                    User32.HWND_TOP,
                    _windowBounds.X,
                    _windowBounds.Y,
                    _windowBounds.Width,
                    _windowBounds.Height,
                    User32.SWP.NOACTIVATE);
                User32.InvalidateRect(new HandleRef(this, Handle), null, BOOL.FALSE);
            }

            /// <summary>
            ///  Handles the WM_GETOBJECT message. Used for accessibility.
            /// </summary>
            private void WmGetObject(ref Message m)
            {
                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "In WmGetObject, this = " + GetType().FullName + ", lParam = " + m.LParam.ToString());

                if (m.Msg == (int)User32.WM.GETOBJECT && m.LParam == (IntPtr)NativeMethods.UiaRootObjectId)
                {
                    // If the requested object identifier is UiaRootObjectId,
                    // we should return an UI Automation provider using the UiaReturnRawElementProvider function.
                    InternalAccessibleObject intAccessibleObject = new InternalAccessibleObject(AccessibilityObject);
                    m.Result = UiaCore.UiaReturnRawElementProvider(
                        new HandleRef(this, Handle),
                        m.WParam,
                        m.LParam,
                        intAccessibleObject);

                    return;
                }

                // some accessible object requested that we don't care about, so do default message processing
                DefWndProc(ref m);
            }

            /// <summary>
            ///  Called when the error window gets a windows message.
            /// </summary>
            protected unsafe override void WndProc(ref Message m)
            {
                switch ((User32.WM)m.Msg)
                {
                    case User32.WM.GETOBJECT:
                        WmGetObject(ref m);
                        break;
                    case User32.WM.NOTIFY:
                        User32.NMHDR* nmhdr = (User32.NMHDR*)m.LParam;
                        if (nmhdr->code == (int)ComCtl32.TTN.SHOW || nmhdr->code == (int)ComCtl32.TTN.POP)
                        {
                            OnToolTipVisibilityChanging(nmhdr->idFrom, nmhdr->code == (int)ComCtl32.TTN.SHOW);
                        }

                        break;
                    case User32.WM.ERASEBKGND:
                        break;
                    case User32.WM.PAINT:
                        OnPaint();
                        break;
                    default:
                        base.WndProc(ref m);
                        break;
                }
            }
        }
    }
}

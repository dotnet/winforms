// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.Graphics.GdiPlus;

namespace System.Windows.Forms;

public partial class ErrorProvider
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
        private AccessibleObject? _accessibleObject;
        private readonly List<ControlItem> _items = [];
        private readonly Control _parent;
        private readonly ErrorProvider _provider;
        private Rectangle _windowBounds;
        private Timer? _timer;
        private NativeWindow? _tipWindow;

        /// <summary>
        ///  Construct an error window for this provider and control parent.
        /// </summary>
        public ErrorWindow(ErrorProvider provider, Control parent)
        {
            _provider = provider;
            _parent = parent;
        }

        /// <summary>
        ///  The Accessibility Object for this ErrorProvider
        /// </summary>
        internal AccessibleObject AccessibilityObject => _accessibleObject ??= CreateAccessibilityInstance();

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

            if (_tipWindow is not null)
            {
                ToolInfoWrapper<ErrorWindow> toolInfo = new(this, item.Id, TOOLTIP_FLAGS.TTF_SUBCLASS, item.Error);
                toolInfo.SendMessage(_tipWindow, PInvoke.TTM_ADDTOOLW);
            }

            Update(timerCaused: false);
        }

        internal List<ControlItem> ControlItems => _items;

        /// <summary>
        ///  Constructs the new instance of the accessibility object for this ErrorProvider. Subclasses
        ///  should not call base.CreateAccessibilityObject.
        /// </summary>
        private ErrorWindowAccessibleObject CreateAccessibilityInstance() => new(this);

        /// <summary>
        ///  Called to get rid of any resources the Object may have.
        /// </summary>
        public void Dispose() => EnsureDestroyed();

        /// <summary>
        ///  Make sure the error window is created, and the tooltip window is created.
        /// </summary>
        private unsafe bool EnsureCreated()
        {
            if (Handle != 0)
            {
                return true;
            }

            if (!_parent.IsHandleCreated)
            {
                return false;
            }

            CreateParams cparams = new()
            {
                Caption = string.Empty,
                Style = (int)(WINDOW_STYLE.WS_VISIBLE | WINDOW_STYLE.WS_CHILD),
                ClassStyle = (int)WNDCLASS_STYLES.CS_DBLCLKS,
                X = 0,
                Y = 0,
                Width = 0,
                Height = 0,
                Parent = _parent.Handle
            };

            CreateHandle(cparams);

            PInvoke.InitCommonControlsEx(new INITCOMMONCONTROLSEX
            {
                dwSize = (uint)sizeof(INITCOMMONCONTROLSEX),
                dwICC = INITCOMMONCONTROLSEX_ICC.ICC_TAB_CLASSES
            });

            cparams = new()
            {
                Parent = Handle,
                ClassName = PInvoke.TOOLTIPS_CLASS,
                Style = (int)PInvoke.TTS_ALWAYSTIP
            };

            _tipWindow = new NativeWindow();
            _tipWindow.CreateHandle(cparams);

            PInvokeCore.SendMessage(
                _tipWindow,
                PInvoke.TTM_SETMAXTIPWIDTH,
                (WPARAM)0,
                (LPARAM)SystemInformation.MaxWindowTrackSize.Width);
            PInvoke.SetWindowPos(
                _tipWindow,
                HWND.HWND_TOP,
                0, 0, 0, 0,
                SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);
            PInvokeCore.SendMessage(_tipWindow, PInvoke.TTM_SETDELAYTIME, (WPARAM)PInvoke.TTDT_INITIAL);

            return true;
        }

        /// <summary>
        ///  Destroy the timer, toolwindow, and the error window itself.
        /// </summary>
        private void EnsureDestroyed()
        {
            if (_timer is not null)
            {
                _timer.Dispose();
                _timer = null;
            }

            if (_tipWindow is not null)
            {
                _tipWindow.DestroyHandle();
                _tipWindow = null;
            }

            // Hide the window and invalidate the parent to ensure that we leave no visual artifacts.
            // Given that we have an unusual region window, this is needed.
            PInvoke.SetWindowPos(
                this,
                HWND.HWND_TOP,
                _windowBounds.X,
                _windowBounds.Y,
                _windowBounds.Width,
                _windowBounds.Height,
                SET_WINDOW_POS_FLAGS.SWP_HIDEWINDOW | SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOMOVE);
            _parent?.Invalidate(true);
            DestroyHandle();
        }

        private unsafe void MirrorDcIfNeeded(HDC hdc)
        {
            if (_parent.IsMirrored)
            {
                // Mirror the DC
                PInvokeCore.SetMapMode(hdc, HDC_MAP_MODE.MM_ANISOTROPIC);
                SIZE originalExtents = default;
                PInvoke.GetViewportExtEx(hdc, &originalExtents);
                PInvoke.SetViewportExtEx(hdc, -originalExtents.Width, originalExtents.Height, lpsz: null);
                Point originalOrigin = default;
                PInvokeCore.GetViewportOrgEx(hdc, &originalOrigin);
                PInvoke.SetViewportOrgEx(hdc, originalOrigin.X + _windowBounds.Width - 1, originalOrigin.Y, lppt: null);
            }
        }

        /// <summary>
        ///  This is called when the error window needs to paint. We paint each icon at its correct location.
        /// </summary>
        private unsafe void OnPaint()
        {
            using BeginPaintScope hdc = new(HWND);
            using SaveDcScope save = new(hdc);

            MirrorDcIfNeeded(hdc);

            for (int i = 0; i < _items.Count; i++)
            {
                ControlItem item = _items[i];
                Rectangle bounds = item.GetIconBounds(_provider.Region.Size);
                PInvokeCore.DrawIconEx(
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
        private void OnTimer(object? sender, EventArgs e)
        {
            int blinkPhase = 0;
            for (int i = 0; i < _items.Count; i++)
            {
                blinkPhase += _items[i].BlinkPhase;
            }

            if (blinkPhase == 0 && _provider.BlinkStyle != ErrorBlinkStyle.AlwaysBlink)
            {
                Debug.Assert(_timer is not null);
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
        ///  This is called when a control no longer needs to display an error icon.
        /// </summary>
        public void Remove(ControlItem item)
        {
            _items.Remove(item);

            if (_tipWindow is not null)
            {
                ToolInfoWrapper<ErrorWindow> info = new(this, item.Id);
                info.SendMessage(_tipWindow, PInvoke.TTM_DELTOOLW);
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
                _timer.Tick += OnTimer;
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
                _windowBounds = _windowBounds.IsEmpty ? iconBounds : Rectangle.Union(_windowBounds, iconBounds);
            }

            using Region windowRegion = new(new Rectangle(0, 0, 0, 0));

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
                            showIcon = ((i & 1) == 0) == _provider.ShowIcon;
                            break;
                    }
                }

                if (showIcon)
                {
                    iconRegion.Region.Translate(iconBounds.X, iconBounds.Y);
                    windowRegion.Union(iconRegion.Region);
                    iconRegion.Region.Translate(-iconBounds.X, -iconBounds.Y);
                }

                if (_tipWindow is not null)
                {
                    TOOLTIP_FLAGS flags = TOOLTIP_FLAGS.TTF_SUBCLASS;
                    if (_provider.RightToLeft)
                    {
                        flags |= TOOLTIP_FLAGS.TTF_RTLREADING;
                    }

                    ToolInfoWrapper<ErrorWindow> toolInfo = new(this, item.Id, flags, item.Error, iconBounds);
                    toolInfo.SendMessage(_tipWindow, PInvoke.TTM_SETTOOLINFOW);
                }

                if (timerCaused && item.BlinkPhase > 0)
                {
                    item.BlinkPhase--;
                }
            }

            if (timerCaused)
            {
                _provider.ShowIcon = !_provider.ShowIcon;
            }

            using GetDcScope hdc = new(HWND);
            using SaveDcScope save = new(hdc);
            MirrorDcIfNeeded(hdc);

            using Graphics g = hdc.CreateGraphics();
            using RegionScope windowRegionHandle = windowRegion.GetRegionScope(g);
            if (PInvoke.SetWindowRgn(this, windowRegionHandle, fRedraw: true) != 0)
            {
                // The HWnd owns the region.
                windowRegionHandle.RelinquishOwnership();
            }

            PInvoke.SetWindowPos(
                this,
                HWND.HWND_TOP,
                _windowBounds.X,
                _windowBounds.Y,
                _windowBounds.Width,
                _windowBounds.Height,
                SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);

            PInvoke.InvalidateRect(this, lpRect: null, bErase: false);
        }

        /// <summary>
        ///  Handles the WM_GETOBJECT message. Used for accessibility.
        /// </summary>
        private void WmGetObject(ref Message m)
        {
            if (m.Msg == (int)PInvokeCore.WM_GETOBJECT && m.LParamInternal == PInvoke.UiaRootObjectId)
            {
                // If the requested object identifier is UiaRootObjectId,
                // we should return an UI Automation provider using the UiaReturnRawElementProvider function.
                m.ResultInternal = PInvoke.UiaReturnRawElementProvider(
                    this,
                    m.WParamInternal,
                    m.LParamInternal,
                    AccessibilityObject);

                return;
            }

            // Some accessible object requested that we don't care about, so do default message processing.
            DefWndProc(ref m);
        }

        /// <summary>
        ///  Called when the error window gets a windows message.
        /// </summary>
        protected override unsafe void WndProc(ref Message m)
        {
            switch (m.MsgInternal)
            {
                case PInvokeCore.WM_GETOBJECT:
                    WmGetObject(ref m);
                    break;
                case PInvokeCore.WM_NOTIFY:
                    NMHDR* nmhdr = (NMHDR*)(nint)m.LParamInternal;
                    if (nmhdr->code is PInvoke.TTN_SHOW or PInvoke.TTN_POP)
                    {
                        OnToolTipVisibilityChanging((nint)nmhdr->idFrom, nmhdr->code == PInvoke.TTN_SHOW);
                    }

                    break;
                case PInvokeCore.WM_ERASEBKGND:
                    break;
                case PInvokeCore.WM_PAINT:
                    OnPaint();
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        protected override void WmDpiChangedBeforeParent(ref Message m)
        {
            base.WmDpiChangedBeforeParent(ref m);

            int currentDpi = (int)PInvoke.GetDpiForWindow(this);
            if (currentDpi == _parent._deviceDpi)
            {
                return;
            }

            double factor = ((double)currentDpi) / _parent._deviceDpi;
            using Icon icon = _provider.Icon;
            _provider.Icon = new Icon(icon, (int)(icon.Width * factor), (int)(icon.Height * factor));
        }
    }
}

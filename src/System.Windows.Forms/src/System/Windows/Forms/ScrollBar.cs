// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Implements the basic functionality of a scroll bar control.
    /// </summary>
    [DefaultProperty(nameof(Value))]
    [DefaultEvent(nameof(Scroll))]
    public abstract partial class ScrollBar : Control
    {
        private static readonly object s_scrollEvent = new object();
        private static readonly object s_valueChangedEvent = new object();

        private int _minimum;
        private int _maximum = 100;
        private int _smallChange = 1;
        private int _largeChange = 10;
        private int _value;
        private readonly ScrollOrientation _scrollOrientation;
        private int _wheelDelta;
        private bool _scaleScrollBarForDpiChange = true;

        /// <summary>
        ///  Initializes a new instance of the <see cref='ScrollBar'/> class.
        /// </summary>
        public ScrollBar() : base()
        {
            SetStyle(ControlStyles.UserPaint, false);
            SetStyle(ControlStyles.StandardClick, false);
            SetStyle(ControlStyles.UseTextForAccessibility, false);

            TabStop = false;

            if ((CreateParams.Style & (int)User32.SBS.VERT) != 0)
            {
                _scrollOrientation = ScrollOrientation.VerticalScroll;
            }
            else
            {
                _scrollOrientation = ScrollOrientation.HorizontalScroll;
            }
        }

        /// <summary>
        ///  Hide AutoSize: it doesn't make sense for this control
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoSize
        {
            get => base.AutoSize;
            set => base.AutoSize = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler? AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler? BackColorChanged
        {
            add => base.BackColorChanged += value;
            remove => base.BackColorChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Image? BackgroundImage
        {
            get => base.BackgroundImage;
            set => base.BackgroundImage = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler? BackgroundImageChanged
        {
            add => base.BackgroundImageChanged += value;
            remove => base.BackgroundImageChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout
        {
            get => base.BackgroundImageLayout;
            set => base.BackgroundImageLayout = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler? BackgroundImageLayoutChanged
        {
            add => base.BackgroundImageLayoutChanged += value;
            remove => base.BackgroundImageLayoutChanged -= value;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassName = ComCtl32.WindowClasses.WC_SCROLLBAR;
                cp.Style &= ~(int)User32.WS.BORDER;
                return cp;
            }
        }

        protected override ImeMode DefaultImeMode => ImeMode.Disable;

        protected override Padding DefaultMargin => Padding.Empty;

        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
        {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            Scale((float)deviceDpiNew / deviceDpiOld);
        }

        /// <summary>
        ///  Gets or sets the foreground color of the scroll bar control.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Color ForeColor
        {
            get => base.ForeColor;
            set => base.ForeColor = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler? ForeColorChanged
        {
            add => base.ForeColorChanged += value;
            remove => base.ForeColorChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Font Font
        {
            get => base.Font;
            set => base.Font = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler? FontChanged
        {
            add => base.FontChanged += value;
            remove => base.FontChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ImeMode ImeMode
        {
            get => base.ImeMode;
            set => base.ImeMode = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler? ImeModeChanged
        {
            add => base.ImeModeChanged += value;
            remove => base.ImeModeChanged -= value;
        }

        /// <summary>
        ///  Gets or sets a value to be added or subtracted to the <see cref='Value'/>
        ///  property when the scroll box is moved a large distance.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(10)]
        [SRDescription(nameof(SR.ScrollBarLargeChangeDescr))]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int LargeChange
        {
            get
            {
                // We preserve the actual large change value that has been set, but when we come to
                // get the value of this property, make sure it's within the maximum allowable value.
                // This way we ensure that we don't depend on the order of property sets when
                // code is generated at design-time.
                return Math.Min(_largeChange, _maximum - _minimum + 1);
            }
            set
            {
                if (_largeChange != value)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidLowBoundArgumentEx, nameof(LargeChange), value, 0));
                    }

                    _largeChange = value;
                    UpdateScrollInfo();
                }
            }
        }

        /// <summary>
        ///  Gets or sets the upper limit of values of the scrollable range.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(100)]
        [SRDescription(nameof(SR.ScrollBarMaximumDescr))]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int Maximum
        {
            get => _maximum;
            set
            {
                if (_maximum != value)
                {
                    if (_minimum > value)
                    {
                        _minimum = value;
                    }
                    if (value < _value)
                    {
                        Value = value;
                    }

                    _maximum = value;
                    UpdateScrollInfo();
                }
            }
        }

        /// <summary>
        ///  Gets or sets the lower limit of values of the scrollable range.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(0)]
        [SRDescription(nameof(SR.ScrollBarMinimumDescr))]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int Minimum
        {
            get => _minimum;
            set
            {
                if (_minimum != value)
                {
                    if (_maximum < value)
                    {
                        _maximum = value;
                    }
                    if (value > _value)
                    {
                        _value = value;
                    }

                    _minimum = value;
                    UpdateScrollInfo();
                }
            }
        }

        /// <summary>
        ///  Gets or sets the value to be added or subtracted to the <see cref='Value'/>
        ///  property when the scroll box is moved a small distance.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(1)]
        [SRDescription(nameof(SR.ScrollBarSmallChangeDescr))]
        public int SmallChange
        {
            get
            {
                // We can't have SmallChange > LargeChange, but we shouldn't manipulate
                // the set values for these properties, so we just return the smaller
                // value here.
                return Math.Min(_smallChange, LargeChange);
            }
            set
            {
                if (_smallChange != value)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidLowBoundArgumentEx, nameof(SmallChange), value, 0));
                    }

                    _smallChange = value;
                    UpdateScrollInfo();
                }
            }
        }

        [DefaultValue(false)]
        public new bool TabStop
        {
            get => base.TabStop;
            set => base.TabStop = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler? TextChanged
        {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }

        /// <summary>
        ///  Gets or sets a numeric value that represents the current position of the scroll box
        ///  on the scroll bar control.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(0)]
        [Bindable(true)]
        [SRDescription(nameof(SR.ScrollBarValueDescr))]
        public int Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    if (value < _minimum || value > _maximum)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidBoundArgument, nameof(Value), value, $"'{nameof(Minimum)}'", $"'{nameof(Maximum)}'"));
                    }

                    _value = value;
                    UpdateScrollInfo();
                    OnValueChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///  Get/Set flag to let scrollbar scale according to the DPI of the window.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [SRDescription(nameof(SR.ControlDpiChangeScale))]
        public bool ScaleScrollBarForDpiChange
        {
            get => _scaleScrollBarForDpiChange;
            set => _scaleScrollBarForDpiChange = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler? Click
        {
            add => base.Click += value;
            remove => base.Click -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event PaintEventHandler? Paint
        {
            add => base.Paint += value;
            remove => base.Paint -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler? DoubleClick
        {
            add => base.DoubleClick += value;
            remove => base.DoubleClick -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler? MouseClick
        {
            add => base.MouseClick += value;
            remove => base.MouseClick -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler? MouseDoubleClick
        {
            add => base.MouseDoubleClick += value;
            remove => base.MouseDoubleClick -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler? MouseDown
        {
            add => base.MouseDown += value;
            remove => base.MouseDown -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler? MouseUp
        {
            add => base.MouseUp += value;
            remove => base.MouseUp -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler? MouseMove
        {
            add => base.MouseMove += value;
            remove => base.MouseMove -= value;
        }

        /// <summary>
        ///  Occurs when the scroll box has been moved by either a mouse or keyboard action.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.ScrollBarOnScrollDescr))]
        public event ScrollEventHandler? Scroll
        {
            add => Events.AddHandler(s_scrollEvent, value);
            remove => Events.RemoveHandler(s_scrollEvent, value);
        }

        /// <summary>
        ///  Occurs when the <see cref='Value'/> property has
        ///  changed, either by a <see cref='OnScroll'/> event
        ///  or programatically.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.valueChangedEventDescr))]
        public event EventHandler? ValueChanged
        {
            add => Events.AddHandler(s_valueChangedEvent, value);
            remove => Events.RemoveHandler(s_valueChangedEvent, value);
        }

        /// <summary>
        ///  This is a helper method that is called by ScaleControl to retrieve the bounds
        ///  that the control should be scaled by. You may override this method if you
        ///  wish to reuse ScaleControl's scaling logic but you need to supply your own
        ///  bounds. The default implementation returns scaled bounds that take into
        ///  account the BoundsSpecified, whether the control is top level, and whether
        ///  the control is fixed width or auto size, and any adornments the control may have.
        /// </summary>
        protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified)
        {
            // Adjust Specified for vertical or horizontal scaling
            if (_scrollOrientation == ScrollOrientation.VerticalScroll)
            {
                specified &= ~BoundsSpecified.Width;
            }
            else
            {
                specified &= ~BoundsSpecified.Height;
            }

            return base.GetScaledBounds(bounds, factor, specified);
        }

        internal override Gdi32.HBRUSH InitializeDCForWmCtlColor(Gdi32.HDC dc, User32.WM msg) => default;

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (Enabled)
            {
                UpdateScrollInfo();
            }

            base.OnEnabledChanged(e);
        }

        /// <summary>
        ///  Creates the handle. overridden to help set up scrollbar information.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateScrollInfo();
        }

        /// <summary>
        ///  Raises the <see cref='ValueChanged'/> event.
        /// </summary>
        protected virtual void OnScroll(ScrollEventArgs se)
            => ((ScrollEventHandler?)Events[s_scrollEvent])?.Invoke(this, se);

        /// <summary>
        ///  Converts mouse wheel movements into scrolling, when scrollbar has the focus.
        ///  Typically one wheel step will cause one small scroll increment, in either
        ///  direction. A single wheel message could represent either one wheel step, multiple
        ///  wheel steps (fast wheeling), or even a fraction of a step (smooth-wheeled mice).
        ///  So we accumulate the total wheel delta, and consume it in whole numbers of steps.
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e != null)
            {
                _wheelDelta += e.Delta;

                bool scrolled = false;

                while (Math.Abs(_wheelDelta) >= NativeMethods.WHEEL_DELTA)
                {
                    if (_wheelDelta > 0)
                    {
                        _wheelDelta -= NativeMethods.WHEEL_DELTA;
                        DoScroll(ScrollEventType.SmallDecrement);
                        scrolled = true;
                    }
                    else
                    {
                        _wheelDelta += NativeMethods.WHEEL_DELTA;
                        DoScroll(ScrollEventType.SmallIncrement);
                        scrolled = true;
                    }
                }

                if (scrolled)
                {
                    DoScroll(ScrollEventType.EndScroll);
                }

                if (e is HandledMouseEventArgs mouseEventArgs)
                {
                    mouseEventArgs.Handled = true;
                }
            }

            // The base implementation should be called before the implementation above,
            // but changing the order in Whidbey would be too much of a breaking change
            // for this particular class.
            base.OnMouseWheel(e);
        }

        /// <summary>
        ///  Raises the <see cref='ValueChanged'/> event.
        /// </summary>
        protected virtual void OnValueChanged(EventArgs e)
            => ((EventHandler?)Events[s_valueChangedEvent])?.Invoke(this, e);

        private int ReflectPosition(int position)
        {
            if (this is HScrollBar)
            {
                return _minimum + (_maximum - LargeChange + 1) - position;
            }

            return position;
        }

        public override string ToString()
        {
            string s = base.ToString();
            return s + ", Minimum: " + Minimum + ", Maximum: " + Maximum + ", Value: " + Value;
        }

        protected void UpdateScrollInfo()
        {
            if (IsHandleCreated && Enabled)
            {
                var si = new User32.SCROLLINFO
                {
                    cbSize = (uint)Marshal.SizeOf<User32.SCROLLINFO>(),
                    fMask = User32.SIF.ALL,
                    nMin = _minimum,
                    nMax = _maximum,
                    nPage = (uint)LargeChange
                };

                if (RightToLeft == RightToLeft.Yes)
                {
                    // Reflect the scrollbar position horizontally on an Rtl system
                    si.nPos = ReflectPosition(_value);
                }
                else
                {
                    si.nPos = _value;
                }

                si.nTrackPos = 0;

                User32.SetScrollInfo(this, User32.SB.CTL, ref si, BOOL.TRUE);
            }
        }

        private void WmReflectScroll(ref Message m)
        {
            ScrollEventType type = (ScrollEventType)PARAM.LOWORD(m.WParam);
            DoScroll(type);
        }

        private void DoScroll(ScrollEventType type)
        {
            // For Rtl systems we need to swap increment and decrement
            if (RightToLeft == RightToLeft.Yes)
            {
                switch (type)
                {
                    case ScrollEventType.First:
                        type = ScrollEventType.Last;
                        break;

                    case ScrollEventType.Last:
                        type = ScrollEventType.First;
                        break;

                    case ScrollEventType.SmallDecrement:
                        type = ScrollEventType.SmallIncrement;
                        break;

                    case ScrollEventType.SmallIncrement:
                        type = ScrollEventType.SmallDecrement;
                        break;

                    case ScrollEventType.LargeDecrement:
                        type = ScrollEventType.LargeIncrement;
                        break;

                    case ScrollEventType.LargeIncrement:
                        type = ScrollEventType.LargeDecrement;
                        break;
                }
            }

            int newValue = _value;
            int oldValue = _value;

            // The ScrollEventArgs constants are defined in terms of the windows
            // messages. This eliminates confusion between the VSCROLL and
            // HSCROLL constants, which are identical.
            switch (type)
            {
                case ScrollEventType.First:
                    newValue = _minimum;
                    break;

                case ScrollEventType.Last:
                    newValue = _maximum - LargeChange + 1;
                    break;

                case ScrollEventType.SmallDecrement:
                    newValue = Math.Max(_value - SmallChange, _minimum);
                    break;

                case ScrollEventType.SmallIncrement:
                    newValue = Math.Min(_value + SmallChange, _maximum - LargeChange + 1);
                    break;

                case ScrollEventType.LargeDecrement:
                    newValue = Math.Max(_value - LargeChange, _minimum);
                    break;

                case ScrollEventType.LargeIncrement:
                    newValue = Math.Min(_value + LargeChange, _maximum - LargeChange + 1);
                    break;

                case ScrollEventType.ThumbPosition:
                case ScrollEventType.ThumbTrack:
                    var si = new User32.SCROLLINFO
                    {
                        cbSize = (uint)Marshal.SizeOf<User32.SCROLLINFO>(),
                        fMask = User32.SIF.TRACKPOS
                    };
                    User32.GetScrollInfo(this, User32.SB.CTL, ref si);

                    if (RightToLeft == RightToLeft.Yes)
                    {
                        newValue = ReflectPosition(si.nTrackPos);
                    }
                    else
                    {
                        newValue = si.nTrackPos;
                    }

                    break;
            }

            var se = new ScrollEventArgs(type, oldValue, newValue, _scrollOrientation);
            OnScroll(se);
            Value = se.NewValue;
        }

        protected override void WndProc(ref Message m)
        {
            switch ((User32.WM)m.Msg)
            {
                case User32.WM.REFLECT_HSCROLL:
                case User32.WM.REFLECT_VSCROLL:
                    WmReflectScroll(ref m);
                    break;

                case User32.WM.ERASEBKGND:
                    break;

                case User32.WM.SIZE:
                    // Fixes the scrollbar focus rect
                    if (User32.GetFocus() == Handle)
                    {
                        DefWndProc(ref m);
                        User32.SendMessageW(this, User32.WM.KILLFOCUS);
                        User32.SendMessageW(this, User32.WM.SETFOCUS);
                    }
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <summary>
        ///  Creates a new AccessibleObject for this <see cref='ScrollBar'/> instance.
        ///  The AccessibleObject instance returned by this method supports ControlType UIA property.
        /// </summary>
        /// <returns>
        ///  AccessibleObject for this <see cref='ScrollBar'/> instance.
        /// </returns>
        protected override AccessibleObject CreateAccessibilityInstance()
            => new ScrollBarAccessibleObject(this);
    }
}

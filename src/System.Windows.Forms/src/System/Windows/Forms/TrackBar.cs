﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.InteropServices;

    using System.Diagnostics;

    using System;
    using Microsoft.Win32;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using System.ComponentModel.Design;
    using System.Windows.Forms.Layout;
    using System.Globalization;
    
    /// <devdoc>
    ///     The TrackBar is a scrollable control similar to the ScrollBar, but
    ///     has a different UI.  You can configure ranges through which it should
    ///     scroll, and also define increments for off-button clicks.  It can be
    ///     aligned horizontally or vertically.  You can also configure how many
    ///     'ticks' are shown for the total range of values
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultProperty(nameof(Value)),
    DefaultEvent(nameof(Scroll)),
    DefaultBindingProperty(nameof(Value)),
    Designer("System.Windows.Forms.Design.TrackBarDesigner, " + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionTrackBar))
    ]
    public class TrackBar : Control, ISupportInitialize {

        private static readonly object EVENT_SCROLL = new object();
        private static readonly object EVENT_VALUECHANGED = new object();
        private static readonly object EVENT_RIGHTTOLEFTLAYOUTCHANGED = new object();        

        private bool autoSize = true;
        private int largeChange = 5;
        private int maximum = 10;
        private int minimum = 0;
        private Orientation orientation = System.Windows.Forms.Orientation.Horizontal;
        private int value = 0;
        private int smallChange = 1;
        private int tickFrequency = 1;
        private TickStyle tickStyle = System.Windows.Forms.TickStyle.BottomRight;

        private int requestedDim;

        // Mouse wheel movement
        private int cumulativeWheelData = 0;

        // Disable value range checking while initializing the control
        private bool initializing = false;

        private bool rightToLeftLayout = false;        

        /// <devdoc>
        ///     Creates a new TrackBar control with a default range of 0..10 and
        ///     ticks shown every value.
        /// </devdoc>
        public TrackBar()
        : base() {
            SetStyle(ControlStyles.UserPaint, false);
            SetStyle(ControlStyles.UseTextForAccessibility, false);
            requestedDim = PreferredDimension;
        }
        
        /// <devdoc>
        ///     Indicates if the control is being auto-sized.  If true, the
        ///     TrackBar will adjust either its height or width [depending on
        ///     orientation] to make sure that only the required amount of
        ///     space is used.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.TrackBarAutoSizeDescr)),
        Browsable(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        ]
        public override bool AutoSize {
            get {
                return autoSize;
            }

            set {
                // Note that we intentionally do not call base.  Labels size themselves by
                // overriding SetBoundsCore (old RTM code).  We let CommonProperties.GetAutoSize
                // continue to return false to keep our LayoutEngines from messing with TextBoxes.
                // This is done for backwards compatibility since the new AutoSize behavior differs.                
                if (autoSize != value) {
                    autoSize = value;
                    if (orientation == Orientation.Horizontal) {
                        SetStyle(ControlStyles.FixedHeight, autoSize);
                        SetStyle(ControlStyles.FixedWidth, false);
                    }
                    else {
                        SetStyle(ControlStyles.FixedWidth, autoSize);
                        SetStyle(ControlStyles.FixedHeight, false);
                    }
                    AdjustSize();
                    OnAutoSizeChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        new public event EventHandler AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }


        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage {
            get {
                return base.BackgroundImage;
            }
            set {
                base.BackgroundImage = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged {
            add => base.BackgroundImageChanged += value;
            remove => base.BackgroundImageChanged -= value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout {
            get {
                return base.BackgroundImageLayout;
            }
            set {
                base.BackgroundImageLayout = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged {
            add => base.BackgroundImageLayoutChanged += value;
            remove => base.BackgroundImageLayoutChanged -= value;
        }

        /// <devdoc>
        ///     This is called when creating a window.  Inheriting classes can override
        ///     this to add extra functionality, but should not forget to first call
        ///     base.getCreateParams() to make sure the control continues to work
        ///     correctly.
        /// </devdoc>
        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ClassName = NativeMethods.WC_TRACKBAR;

                switch (tickStyle) {
                    case TickStyle.None:
                        cp.Style |= NativeMethods.TBS_NOTICKS;
                        break;
                    case TickStyle.TopLeft:
                        cp.Style |= (NativeMethods.TBS_AUTOTICKS | NativeMethods.TBS_TOP);
                        break;
                    case TickStyle.BottomRight:
                        cp.Style |= (NativeMethods.TBS_AUTOTICKS | NativeMethods.TBS_BOTTOM);
                        break;
                    case TickStyle.Both:
                        cp.Style |= (NativeMethods.TBS_AUTOTICKS | NativeMethods.TBS_BOTH);
                        break;
                }

                if (orientation == Orientation.Vertical) {
                    cp.Style |= NativeMethods.TBS_VERT; // HORIZ == 0
                }

                if (RightToLeft == RightToLeft.Yes && RightToLeftLayout == true) {
                    //We want to turn on mirroring for Trackbar explicitly.
                    cp.ExStyle |= NativeMethods.WS_EX_LAYOUTRTL | NativeMethods.WS_EX_NOINHERITLAYOUT;
                    //Don't need these styles when mirroring is turned on.
                    cp.ExStyle &= ~(NativeMethods.WS_EX_RTLREADING | NativeMethods.WS_EX_RIGHT | NativeMethods.WS_EX_LEFTSCROLLBAR);
                }
                return cp;
            }
        }
        
        protected override ImeMode DefaultImeMode {
            get {
                return ImeMode.Disable;
            }
        }

        /// <devdoc>
        ///     Deriving classes can override this to configure a default size for their control.
        ///     This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Size DefaultSize {
            get {
                return new Size(104, PreferredDimension);
            }
        }

        /// <devdoc>
        ///     This property is overridden and hidden from statement completion
        ///     on controls that are based on Win32 Native Controls.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override bool DoubleBuffered {
            get {
                return base.DoubleBuffered;
            }
            set {
                base.DoubleBuffered = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Font Font {
            get {
                return base.Font;
            }
            set {
                base.Font = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler FontChanged {
            add => base.FontChanged += value;
            remove => base.FontChanged -= value;
        }

        /// <devdoc>
        ///     The current foreground color of the TrackBar.  Note that users
        ///     are unable to change this.  It is always Color.WINDOWTEXT
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color ForeColor {
            get {
                return SystemColors.WindowText;
            }
            set {
            }
        }
        
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler ForeColorChanged {
            add => base.ForeColorChanged += value;
            remove => base.ForeColorChanged -= value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public ImeMode ImeMode {
            get {
                return base.ImeMode;
            }
            set {
                base.ImeMode = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler ImeModeChanged {
            add => base.ImeModeChanged += value;
            remove => base.ImeModeChanged -= value;
        }

        /// <devdoc>
        ///     The number of ticks by which the TrackBar will change when an
        ///     event considered a "large change" occurs.  These include, Clicking the
        ///     mouse to the side of the button, or using the PgUp/PgDn keys on the
        ///     keyboard.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(5),
        SRDescription(nameof(SR.TrackBarLargeChangeDescr))
        ]
        public int LargeChange {
            get {
                return largeChange;
            }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(LargeChange), string.Format(SR.TrackBarLargeChangeError, value));
                }

                if (largeChange != value) {
                    largeChange = value;
                    if (IsHandleCreated)
                    {
                        SendMessage(NativeMethods.TBM_SETPAGESIZE, 0, value);
                    }
                }
            }
        }

        /// <devdoc>
        ///     The upper limit of the range this TrackBar is working with.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(10),
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.TrackBarMaximumDescr))
        ]
        public int Maximum {
            get {
                return maximum;
            }
            set {
                if (maximum != value) {
                    if (value < minimum) {
                        minimum = value;
                    }
                    SetRange(minimum, value);
                }
            }
        }

        /// <devdoc>
        ///     The lower limit of the range this TrackBar is working with.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(0),
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.TrackBarMinimumDescr))
        ]
        public int Minimum {
            get {
                return minimum;
            }
            set {
                if (minimum != value) {
                    if (value > maximum) {
                        maximum = value;
                    }
                    SetRange(value, maximum);
                }
            }
        }

        /// <devdoc>
        ///    <para>The orientation for this TrackBar. Valid values are from
        ///       the Orientation enumeration. The control currently supports being
        ///       oriented horizontally and vertically.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(Orientation.Horizontal),
        Localizable(true),
        SRDescription(nameof(SR.TrackBarOrientationDescr))
        ]
        public Orientation Orientation {
            get {
                return orientation;
            }
            set {
                //valid values are 0x0 to 0x1
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)Orientation.Horizontal, (int)Orientation.Vertical))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Orientation));
                }

                if (orientation != value) {
                    orientation = value;

                    if (orientation == Orientation.Horizontal) {
                        SetStyle(ControlStyles.FixedHeight, autoSize);
                        SetStyle(ControlStyles.FixedWidth, false);
                        Width = requestedDim;
                    }
                    else {
                        SetStyle(ControlStyles.FixedHeight, false);
                        SetStyle(ControlStyles.FixedWidth, autoSize);
                        Height = requestedDim;
                    }

                    if (IsHandleCreated) {
                        Rectangle r = Bounds;
                        RecreateHandle();
                        SetBounds(r.X, r.Y, r.Height, r.Width, BoundsSpecified.All);
                        AdjustSize();
                    }
                }
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Padding Padding {
            get { return base.Padding; }
            set { base.Padding = value;}
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new event EventHandler PaddingChanged {
            add => base.PaddingChanged += value; 
            remove => base.PaddingChanged -= value; }

        /// <devdoc>
        ///     Little private routine that helps with auto-sizing.
        /// </devdoc>
        private int PreferredDimension {
            get {
                int cyhscroll = UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYHSCROLL);

                // this is our preferred size
                //
                return((cyhscroll * 8) / 3);
            }
        }

        /// <devdoc>
        ///     Redraw control, if the handle's created
        /// </devdoc>
        private void RedrawControl()
        {
            if (IsHandleCreated)
            {
                //The '1' in the call to SendMessage below indicates that the 
                //trackbar should be redrawn (see TBM_SETRANGEMAX in MSDN)
                SendMessage(NativeMethods.TBM_SETRANGEMAX, 1, maximum);
                Invalidate();
            }
        }

        /// <devdoc>
        ///     This is used for international applications where the language
        ///     is written from RightToLeft. When this property is true,
        //      and the RightToLeft property is true, mirroring will be turned on on the trackbar.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        DefaultValue(false),
        SRDescription(nameof(SR.ControlRightToLeftLayoutDescr))
        ]
        public virtual bool RightToLeftLayout {
            get {

                return rightToLeftLayout;
            }

            set {
                if (value != rightToLeftLayout) {
                    rightToLeftLayout = value;
                    using(new LayoutTransaction(this, this, PropertyNames.RightToLeftLayout)) {
                        OnRightToLeftLayoutChanged(EventArgs.Empty);
                    }
                }
            }
        }

        /// <devdoc>
        ///     The number of ticks by which the TrackBar will change when an
        ///     event considered a "small change" occurs.  These are most commonly
        ///     seen by using the arrow keys to move the TrackBar thumb around.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(1),
        SRDescription(nameof(SR.TrackBarSmallChangeDescr))
        ]
        public int SmallChange {
            get {
                return smallChange;
            }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(SmallChange), string.Format(SR.TrackBarSmallChangeError, value));
                }
                if (smallChange != value) {
                    smallChange = value;
                    if (IsHandleCreated)
                    {
                        SendMessage(NativeMethods.TBM_SETLINESIZE, 0, value);
                    }
                }
            }
        }
        
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Bindable(false)]        
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }
        
        /// <devdoc>
        ///     Indicates how the TrackBar control will draw itself.  This affects
        ///     both where the ticks will be drawn in relation to the moveable thumb,
        ///     and how the thumb itself will be drawn.  values are taken from the
        ///     TickStyle enumeration.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(TickStyle.BottomRight),
        SRDescription(nameof(SR.TrackBarTickStyleDescr))
        ]
        public TickStyle TickStyle {
            get {
                return tickStyle;
            }
            set {
                // Confirm that value is a valid enum
                //valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)TickStyle.None, (int)TickStyle.Both))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(TickStyle));
                }

                if (tickStyle != value) {
                    tickStyle = value;
                    RecreateHandle();
                }
            }
        }

        /// <devdoc>
        ///     Indicates just how many ticks will be drawn.  For a TrackBar with a
        ///     range of 0..100, it might be impractical to draw all 100 ticks for a
        ///     very small control.  Passing in a value of 5 here would only draw
        ///     20 ticks -- i.e. Each tick would represent 5 units in the TrackBars
        ///     range of values.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(1),
        SRDescription(nameof(SR.TrackBarTickFrequencyDescr))
        ]
        public int TickFrequency {
            get {
                return tickFrequency;
            }
            set {
                if (tickFrequency != value) {
                    tickFrequency = value;
                    if (IsHandleCreated) {
                        SendMessage(NativeMethods.TBM_SETTICFREQ, value, 0);
                        Invalidate();
                    }
                }
            }
        }

        /// <devdoc>
        ///     The current location of the TrackBar thumb.  This value must
        ///     be between the lower and upper limits of the TrackBar range, of course.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(0),
        Bindable(true),
        SRDescription(nameof(SR.TrackBarValueDescr))
        ]
        public int Value {
            get {
                GetTrackBarValue();
                return value;
            }
            set {
                if (this.value != value) {
                    if (!initializing && ((value < minimum) || (value > maximum)))
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidBoundArgument, nameof(Value), value, $"'{nameof(Minimum)}'", $"'${nameof(Maximum)}'"));
                    }

                    this.value = value;
                    SetTrackBarPosition();
                    OnValueChanged(EventArgs.Empty);
                }
            }
        }    

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler Click {
            add => base.Click += value;
            remove => base.Click -= value;
        }

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler DoubleClick {
            add => base.DoubleClick += value;
            remove => base.DoubleClick -= value;
        }

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler MouseClick {
            add => base.MouseClick += value;
            remove => base.MouseClick -= value;
        }

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler MouseDoubleClick {
            add => base.MouseDoubleClick += value;
            remove => base.MouseDoubleClick -= value;
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
        public event EventHandler RightToLeftLayoutChanged {
            add => Events.AddHandler(EVENT_RIGHTTOLEFTLAYOUTCHANGED, value);
            remove => Events.RemoveHandler(EVENT_RIGHTTOLEFTLAYOUTCHANGED, value);
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.TrackBarOnScrollDescr))]
        public event EventHandler Scroll {
            add => Events.AddHandler(EVENT_SCROLL, value);
            remove => Events.RemoveHandler(EVENT_SCROLL, value);
        }

        /// <devdoc>
        ///     TrackBar Onpaint.
        /// </devdoc>
        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event PaintEventHandler Paint {
            add => base.Paint += value;
            remove => base.Paint -= value;
        }

        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.valueChangedEventDescr))]
        public event EventHandler ValueChanged {
            add => Events.AddHandler(EVENT_VALUECHANGED, value);
            remove => Events.RemoveHandler(EVENT_VALUECHANGED, value);
        }

        /// <devdoc>
        ///     Enforces autoSizing
        /// </devdoc>
        private void AdjustSize() {
            if (IsHandleCreated) {
                int saveDim = requestedDim;
                try {
                    if (orientation == Orientation.Horizontal)
                        Height = autoSize ? PreferredDimension : saveDim;
                    else
                        Width = autoSize ? PreferredDimension : saveDim;
                }
                finally {
                    requestedDim = saveDim;
                }
            }
        }

        /// <devdoc>
        ///      Handles tasks required when the control is being initialized.
        /// </devdoc>
        public void BeginInit() {
            initializing = true;
        }

        // Constrain the current value of the control to be within
        // the minimum and maximum.
        //
        private void ConstrainValue() {

            // Don't constrain the value while we're initializing the control
            if (initializing) {
                return;
            }

            Debug.Assert(minimum <= maximum, "Minimum should be <= Maximum");

            // Keep the current value within the minimum and maximum
            if (Value < minimum) {
                Value = minimum;
            }
            if (Value > maximum) {
                Value = maximum;
            }
        }

        /// <devdoc>
        /// </devdoc>
        protected override void CreateHandle() {
            if (!RecreatingHandle) {
                IntPtr userCookie = UnsafeNativeMethods.ThemingScope.Activate();
                try {
                    NativeMethods.INITCOMMONCONTROLSEX icc = new NativeMethods.INITCOMMONCONTROLSEX();
                    icc.dwICC = NativeMethods.ICC_BAR_CLASSES;
                    SafeNativeMethods.InitCommonControlsEx(icc);
                } finally {
                    UnsafeNativeMethods.ThemingScope.Deactivate(userCookie);
                }
            }
            base.CreateHandle();
        }
        
        /// <devdoc>
        ///      Called when initialization of the control is complete.
        /// </devdoc>
        public void EndInit() {
            initializing = false;

            // Make sure the value is constrained by the minimum and maximum
            ConstrainValue();
        }

        private void GetTrackBarValue() {
            if (IsHandleCreated) {
                value = unchecked( (int) (long)SendMessage(NativeMethods.TBM_GETPOS, 0, 0));
                
                // See SetTrackBarValue() for a description of why we sometimes reflect the trackbar value
                //                   
                   
                if (orientation == Orientation.Vertical) {
                    // Reflect value
                    value = Minimum + Maximum - value;
                }
                
                // Reflect for a RightToLeft horizontal trackbar
                //
                if (orientation == Orientation.Horizontal && RightToLeft == RightToLeft.Yes && !IsMirrored) {
                    value = Minimum + Maximum - value;
                }
            }
        }                                               

        /// <devdoc>
        ///      Handling special input keys, such as pgup, pgdown, home, end, etc...
        /// </devdoc>
        protected override bool IsInputKey(Keys keyData) {
            if ((keyData & Keys.Alt) == Keys.Alt) return false;
            switch (keyData & Keys.KeyCode) {
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Home:
                case Keys.End:
                    return true;
            }
            return base.IsInputKey(keyData);
        }
        
        /// <devdoc>
        /// </devdoc>
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            SendMessage(NativeMethods.TBM_SETRANGEMIN, 0, minimum);
            SendMessage(NativeMethods.TBM_SETRANGEMAX, 0, maximum);
            SendMessage(NativeMethods.TBM_SETTICFREQ, tickFrequency, 0);
            SendMessage(NativeMethods.TBM_SETPAGESIZE, 0, largeChange);
            SendMessage(NativeMethods.TBM_SETLINESIZE, 0, smallChange);
            SetTrackBarPosition();
            AdjustSize();
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnRightToLeftLayoutChanged(EventArgs e) {
            if (GetAnyDisposingInHierarchy()) {
                return;
            }

            if (RightToLeft == RightToLeft.Yes) {
                RecreateHandle();
            }

            EventHandler eh = Events[EVENT_RIGHTTOLEFTLAYOUTCHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <devdoc>
        ///     Actually fires the "scroll" event.  Inheriting classes should override
        ///     this method in favor of actually adding an EventHandler for this
        ///     event.  Inheriting classes should not forget to call
        ///     base.onScroll(e)
        /// </devdoc>
        protected virtual void OnScroll(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EVENT_SCROLL];
            if (handler != null) handler(this,e);
        }

        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.MouseWheel'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel( e );

            HandledMouseEventArgs hme = e as HandledMouseEventArgs;
            if (hme != null) {
               if (hme.Handled) {
                   return;
               }
               hme.Handled = true;
            }

            if ((ModifierKeys & (Keys.Shift | Keys.Alt)) != 0 || MouseButtons != MouseButtons.None) {
                return; // Do not scroll when Shift or Alt key is down, or when a mouse button is down.
            }

            int wheelScrollLines = SystemInformation.MouseWheelScrollLines;
            if (wheelScrollLines == 0) {
                return; // Do not scroll when the user system setting is 0 lines per notch
            }

            Debug.Assert(this.cumulativeWheelData > -NativeMethods.WHEEL_DELTA, "cumulativeWheelData is too small");
            Debug.Assert(this.cumulativeWheelData < NativeMethods.WHEEL_DELTA, "cumulativeWheelData is too big");
            this.cumulativeWheelData += e.Delta;

            float partialNotches;
            partialNotches = (float)this.cumulativeWheelData / (float)NativeMethods.WHEEL_DELTA;

            if (wheelScrollLines == -1) {
                wheelScrollLines = TickFrequency;
            }

            // Evaluate number of bands to scroll
            int scrollBands = (int)((float)wheelScrollLines * partialNotches);

            if (scrollBands != 0) {
               int absScrollBands;
               if (scrollBands > 0) {
                  absScrollBands = scrollBands;
                  Value = Math.Min(absScrollBands+Value, Maximum);
                  this.cumulativeWheelData -= (int)((float)scrollBands * ((float)NativeMethods.WHEEL_DELTA / (float)wheelScrollLines));
               }
               else {
                  absScrollBands = -scrollBands;
                  Value = Math.Max(Value-absScrollBands, Minimum);
                  this.cumulativeWheelData -= (int)((float)scrollBands * ((float)NativeMethods.WHEEL_DELTA / (float)wheelScrollLines));
               }
            }

            if (e.Delta != Value) {
                OnScroll(EventArgs.Empty);
                OnValueChanged(EventArgs.Empty);
            }
        }

        /// <devdoc>
        ///     Actually fires the "valueChanged" event.
        /// </devdoc>
        protected virtual void OnValueChanged(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EVENT_VALUECHANGED];
            if (handler != null) handler(this,e);
        }

        /// <devdoc>
        ///     This method is called by the control when any property changes. Inheriting
        ///     controls can overide this method to get property change notification on
        ///     basic properties. Inherting controls must call base.propertyChanged.
        /// </devdoc>
        protected override void OnBackColorChanged(EventArgs e) {
            base.OnBackColorChanged(e);
            RedrawControl();
        }

        protected override void OnSystemColorsChanged (EventArgs e) {
            base.OnSystemColorsChanged (e);
            RedrawControl();
        }

        /// <devdoc>
        ///     Overrides Control.setBoundsCore to enforce autoSize.
        /// </devdoc>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
            //SetBoundsCore .. sets the height for a control in designer .. we should obey the requested 
            //height is Autosize is false..
            //if (IsHandleCreated) {
            requestedDim = (orientation == Orientation.Horizontal)
                            ? height
                            : width;

            if (autoSize) {
                if (orientation == Orientation.Horizontal) {
                    if ((specified & BoundsSpecified.Height) != BoundsSpecified.None)
                        height = PreferredDimension;
                }
                else {
                    if ((specified & BoundsSpecified.Width) != BoundsSpecified.None)
                        width = PreferredDimension;
                }
            }
            //}
            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <devdoc>
        ///     Lets you set the the entire range for the TrackBar control at once.
        ///     The values passed are both the lower and upper limits to the range
        ///     with which the control will work.
        /// </devdoc>
        public void SetRange(int minValue, int maxValue) {
            if (minimum != minValue || maximum != maxValue) {
            
                // The Minimum and Maximum properties contain the logic for
                // ensuring that minValue <= maxValue. It is possible, however,
                // that this function will be called somewhere other than from
                // these two properties, so we'll check that here anyway.
                if (minValue > maxValue) {
                    // We'll just adjust maxValue to match minValue
                    maxValue = minValue;
                }

                minimum = minValue;
                maximum = maxValue;

                if (IsHandleCreated) {
                    SendMessage(NativeMethods.TBM_SETRANGEMIN, 0, minimum);

                    // We must repaint the trackbar after changing
                    // the range. The '1' in the call to
                    // SendMessage below indicates that the trackbar
                    // should be redrawn (see TBM_SETRANGEMAX in MSDN)
                    SendMessage(NativeMethods.TBM_SETRANGEMAX, 1, maximum);

                    Invalidate();
                }
                
                // When we change the range, the comctl32 trackbar's internal position can change 
                // (because of the reflection that occurs with vertical trackbars)
                // so we make sure to explicitly set the trackbar position.
                //
                if (value < minimum) {
                    value = minimum;
                }
                if (value > maximum) {
                    value = maximum;
                }
                SetTrackBarPosition();
            }
        }
        
        private void SetTrackBarPosition() {
            if (IsHandleCreated) {
            
                // There are two situations where we want to reflect the track bar position:
                //
                // 1. For a vertical trackbar, it seems to make more sense for the trackbar to increase in value
                //    as the slider moves up the trackbar (this is opposite what the underlying winctl control does)
                //
                // 2. For a RightToLeft horizontal trackbar, we want to reflect the position.
                //
                int reflectedValue = value;
                
                // 1. Reflect for a vertical trackbar
                //
                if (orientation == Orientation.Vertical) {
                    reflectedValue = Minimum + Maximum - value;
                }
                
                // 2. Reflect for a RightToLeft horizontal trackbar
                //
                if (orientation == Orientation.Horizontal && RightToLeft == RightToLeft.Yes && !IsMirrored) {
                    reflectedValue = Minimum + Maximum - value;
                }
                
                SendMessage(NativeMethods.TBM_SETPOS, 1, reflectedValue);
            }
        }

        /// <devdoc>
        ///     Returns a string representation for this control.
        /// </devdoc>
        public override string ToString() {

            string s = base.ToString();
            return s + ", Minimum: " + Minimum.ToString(CultureInfo.CurrentCulture) + ", Maximum: " + Maximum.ToString(CultureInfo.CurrentCulture) + ", Value: " + value;
        }

        /// <devdoc>
        ///     The button's window procedure.  Inheriting classes can override this
        ///     to add extra functionality, but should not forget to call
        ///     base.wndProc(m); to ensure the button continues to function properly.
        /// </devdoc>
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case Interop.WindowMessages.WM_REFLECT+Interop.WindowMessages.WM_HSCROLL:
                case Interop.WindowMessages.WM_REFLECT+Interop.WindowMessages.WM_VSCROLL:
                    switch (NativeMethods.Util.LOWORD(m.WParam)) {
                        case NativeMethods.TB_LINEUP:
                        case NativeMethods.TB_LINEDOWN:
                        case NativeMethods.TB_PAGEUP:
                        case NativeMethods.TB_PAGEDOWN:
                            //case NativeMethods.TB_THUMBPOSITION:
                        case NativeMethods.TB_THUMBTRACK:
                        case NativeMethods.TB_TOP:
                        case NativeMethods.TB_BOTTOM:
                        case NativeMethods.TB_ENDTRACK:
                            if (value != Value) {
                                OnScroll(EventArgs.Empty);
                                OnValueChanged(EventArgs.Empty);
                            }
                            break;
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Diagnostics;

    using System;
    using System.Security.Permissions;
    
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Globalization;


    /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Implements the basic functionality of a scroll bar control.
    ///    </para>
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultProperty(nameof(Value)),
    DefaultEvent(nameof(Scroll)),
    System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors") // Shipped in Everett
    ]
    public abstract class ScrollBar : Control {

        private static readonly object EVENT_SCROLL = new object();
        private static readonly object EVENT_VALUECHANGED = new object();

        private int minimum = 0;
        private int maximum = 100;
        private int smallChange = 1;
        private int largeChange = 10;
        private int value = 0;
        private ScrollOrientation scrollOrientation;
        private int wheelDelta = 0;
        private bool scaleScrollBarForDpiChange = true;

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.ScrollBar"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.ScrollBar'/>
        ///       class.
        ///       
        ///    </para>
        /// </devdoc>
        public ScrollBar()
        : base() {
            SetStyle(ControlStyles.UserPaint, false);
            SetStyle(ControlStyles.StandardClick, false);
            SetStyle(ControlStyles.UseTextForAccessibility, false);

            TabStop = false;

            if ((this.CreateParams.Style & NativeMethods.SBS_VERT) != 0)
            {
                scrollOrientation = ScrollOrientation.VerticalScroll;
            }
            else
            {
                scrollOrientation = ScrollOrientation.HorizontalScroll;
            }
        }

        /// <devdoc>
        ///     <para>Hide AutoSize: it doesn't make sense for this control</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.AutoSizeChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler AutoSizeChanged {
            add {
                base.AutoSizeChanged += value;
            }
            remove {
                base.AutoSizeChanged -= value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.BackColor"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color BackColor {
            get {
                return base.BackColor;
            }
            set {
                base.BackColor = value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.BackColorChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackColorChanged {
            add {
                base.BackColorChanged += value;
            }
            remove {
                base.BackColorChanged -= value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.BackgroundImage"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage {
            get {
                return base.BackgroundImage;
            }
            set {
                base.BackgroundImage = value;
            }
        }
        
        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.BackgroundImageChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged {
            add {
                base.BackgroundImageChanged += value;
            }
            remove {
                base.BackgroundImageChanged -= value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.BackgroundImageLayout"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout {
            get {
                return base.BackgroundImageLayout;
            }
            set {
                base.BackgroundImageLayout = value;
            }
        }
        
        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.BackgroundImageLayoutChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged {
            add {
                base.BackgroundImageLayoutChanged += value;
            }
            remove {
                base.BackgroundImageLayoutChanged -= value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.CreateParams"]/*' />
        /// <devdoc>
        ///     Retrieves the parameters needed to create the handle.  Inheriting classes
        ///     can override this to provide extra functionality.  They should not,
        ///     however, forget to call base.getCreateParams() first to get the struct
        ///     filled up with the basic info.
        /// </devdoc>
        /// <internalonly/>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;
                cp.ClassName = "SCROLLBAR";
                cp.Style &= (~NativeMethods.WS_BORDER);
                return cp;
            }
        }
        
        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.DefaultImeMode"]/*' />
        protected override ImeMode DefaultImeMode {
            get {
                return ImeMode.Disable;
            }
        }

        protected override Padding DefaultMargin {
            get {
                return Padding.Empty;
            }
        }

        /// <summary>
        /// rescale constants for the DPI change
        /// </summary>
        /// <param name="deviceDpiOld">ols dpi</param>
        /// <param name="deviceDpiNew">new dpi</param>
        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew) { 
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            Scale((float)deviceDpiNew / deviceDpiOld);
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.ForeColor"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the foreground color of the scroll bar control.
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color ForeColor {
            get {
                return base.ForeColor;
            }
            set {
                base.ForeColor = value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.ForeColorChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler ForeColorChanged {
            add {
                base.ForeColorChanged += value;
            }
            remove {
                base.ForeColorChanged -= value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.Font"]/*' />
        /// <internalonly/><hideinheritance/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Font Font {
            get {
                return base.Font;
            }
            set {
                base.Font = value;
            }
        }
        
        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.FontChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler FontChanged {
            add {
                base.FontChanged += value;
            }
            remove {
                base.FontChanged -= value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.ImeMode"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public ImeMode ImeMode {
            get {
                return base.ImeMode;
            }
            set {
                base.ImeMode = value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.ImeModeChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler ImeModeChanged {
            add {
                base.ImeModeChanged += value;
            }
            remove {
                base.ImeModeChanged -= value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.LargeChange"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value to be added or subtracted to the <see cref='System.Windows.Forms.ScrollBar.Value'/>
        ///       property when the scroll box is moved a large distance.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(10),
        SRDescription(nameof(SR.ScrollBarLargeChangeDescr)),
        RefreshProperties(RefreshProperties.Repaint)
        ]
        public int LargeChange {
            get {
                // We preserve the actual large change value that has been set, but when we come to
                // get the value of this property, make sure it's within the maximum allowable value.
                // This way we ensure that we don't depend on the order of property sets when
                // code is generated at design-time.
                //
                return Math.Min(largeChange, maximum - minimum + 1);                
            }
            set {
                if (largeChange != value) {
                
                    if (value < 0) {                
                        throw new ArgumentOutOfRangeException(nameof(LargeChange), string.Format(SR.InvalidLowBoundArgumentEx, "LargeChange", (value).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                    }
                
                    largeChange = value;
                    UpdateScrollInfo();
                }
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.Maximum"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the upper limit of values of the scrollable range.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(100),
        SRDescription(nameof(SR.ScrollBarMaximumDescr)),
        RefreshProperties(RefreshProperties.Repaint)
        ]
        public int Maximum {
            get {
                return maximum;
            }
            set {
                if (maximum != value) {
                    if (minimum > value)
                        minimum = value;
                    // bring this.value in line.
                    if (value < this.value)
                        Value = value;
                    maximum = value;
                    UpdateScrollInfo();
                }
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.Minimum"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the lower limit of values of the scrollable range.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(0),
        SRDescription(nameof(SR.ScrollBarMinimumDescr)),
        RefreshProperties(RefreshProperties.Repaint)
        ]
        public int Minimum {
            get {
                return minimum;
            }
            set {
                if (minimum != value) {
                    if (maximum < value)
                        maximum = value;
                    // bring this.value in line.
                    if (value > this.value)
                        this.value = value;
                    minimum = value;
                    UpdateScrollInfo();
                }
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.SmallChange"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the value to be added or subtracted to the
        ///    <see cref='System.Windows.Forms.ScrollBar.Value'/> 
        ///    property when the scroll box is
        ///    moved a small distance.
        /// </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(1),
        SRDescription(nameof(SR.ScrollBarSmallChangeDescr))
        ]
        public int SmallChange {
            get {
                // We can't have SmallChange > LargeChange, but we shouldn't manipulate
                // the set values for these properties, so we just return the smaller 
                // value here. 
                //
                return Math.Min(smallChange, LargeChange);
            }
            set {
                if (smallChange != value) {
                
                    if (value < 0) {                
                        throw new ArgumentOutOfRangeException(nameof(SmallChange), string.Format(SR.InvalidLowBoundArgumentEx, "SmallChange", (value).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                    }
                
                    smallChange = value;
                    UpdateScrollInfo();
                }
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.TabStop"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [DefaultValue(false)]
        new public bool TabStop {
            get {
                return base.TabStop;
            }
            set {
                base.TabStop = value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.Text"]/*' />
        /// <internalonly/>        
        /// <devdoc>
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
        Bindable(false), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]                
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.TextChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged {
            add {
                base.TextChanged += value;
            }
            remove {
                base.TextChanged -= value;
            }
        }
        
        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.Value"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a numeric value that represents the current
        ///       position of the scroll box
        ///       on
        ///       the scroll bar control.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(0),
        Bindable(true),
        SRDescription(nameof(SR.ScrollBarValueDescr))
        ]
        public int Value {
            get {
                return value;
            }
            set {
                if (this.value != value) {
                    if (value < minimum || value > maximum) {
                        throw new ArgumentOutOfRangeException(nameof(Value), string.Format(SR.InvalidBoundArgument, "Value", (value).ToString(CultureInfo.CurrentCulture), "'minimum'", "'maximum'"));
                    }
                    this.value = value;
                    UpdateScrollInfo();
                    OnValueChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get/Set flag to let scrollbar scale according to the DPI of the window.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        Browsable(true), 
        EditorBrowsable(EditorBrowsableState.Always),
        SRDescription(nameof(SR.ControlDpiChangeScale))]
        public bool ScaleScrollBarForDpiChange {
            get {
                return scaleScrollBarForDpiChange;
            }
            set {
                scaleScrollBarForDpiChange = value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.Click"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler Click {
            add {
                base.Click += value;
            }
            remove {
                base.Click -= value;
            }
        }
         
        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.OnPaint"]/*' />
        /// <devdoc>
        ///     ScrollBar Onpaint.
        /// </devdoc>
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event PaintEventHandler Paint {
            add {
                base.Paint += value;
            }
            remove {
                base.Paint -= value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.DoubleClick"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler DoubleClick {
            add {
                base.DoubleClick += value;
            }
            remove {
                base.DoubleClick -= value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.MouseClick"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler MouseClick {
            add {
                base.MouseClick += value;
            }
            remove {
                base.MouseClick -= value;
            }
        }
        
        
        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.MouseDoubleClick"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler MouseDoubleClick {
            add {
                base.MouseDoubleClick += value;
            }
            remove {
                base.MouseDoubleClick -= value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.MouseDown"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler MouseDown {
            add {
                base.MouseDown += value;
            }
            remove {
                base.MouseDown -= value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.MouseUp"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler MouseUp {
            add {
                base.MouseUp += value;
            }
            remove {
                base.MouseUp -= value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.MouseMove"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler MouseMove {
            add {
                base.MouseMove += value;
            }
            remove {
                base.MouseMove -= value;
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.Scroll"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs when the scroll box has
        ///       been
        ///       moved by either a mouse or keyboard action.
        ///    </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ScrollBarOnScrollDescr))]
        public event ScrollEventHandler Scroll {
            add {
                Events.AddHandler(EVENT_SCROLL, value);
            }
            remove {
                Events.RemoveHandler(EVENT_SCROLL, value);
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.ValueChanged"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs when the <see cref='System.Windows.Forms.ScrollBar.Value'/> property has changed, either by a
        ///    <see cref='System.Windows.Forms.ScrollBar.OnScroll'/> event or programatically.
        ///    </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.valueChangedEventDescr))]
        public event EventHandler ValueChanged {
            add {
                Events.AddHandler(EVENT_VALUECHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_VALUECHANGED, value);
            }
        }

        
        /// <devdoc>
        ///     This is a helper method that is called by ScaleControl to retrieve the bounds
        ///     that the control should be scaled by.  You may override this method if you
        ///     wish to reuse ScaleControl's scaling logic but you need to supply your own 
        ///     bounds.  The default implementation returns scaled bounds that take into 
        ///     account the BoundsSpecified, whether the control is top level, and whether
        ///     the control is fixed width or auto size, and any adornments the control may have.
        /// </devdoc>
        protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified) {
            
            // Adjust Specified for vertical or horizontal scaling
            if (scrollOrientation == ScrollOrientation.VerticalScroll) {
                specified &= ~BoundsSpecified.Width;
            }
            else {
                specified &= ~BoundsSpecified.Height;
            }

            return base.GetScaledBounds(bounds, factor, specified);
        }

        internal override IntPtr InitializeDCForWmCtlColor (IntPtr dc, int msg)
        {
            return IntPtr.Zero;
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.OnEnabledChanged"]/*' />
        protected override void OnEnabledChanged(EventArgs e) {
            if (Enabled) {
                UpdateScrollInfo();
            }
            base.OnEnabledChanged(e);
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.OnHandleCreated"]/*' />
        /// <devdoc>
        ///     Creates the handle.  overridden to help set up scrollbar information.
        /// </devdoc>
        /// <internalonly/>
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            UpdateScrollInfo();
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.OnScroll"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.ScrollBar.ValueChanged'/> event.
        ///    </para>
        /// </devdoc>
        protected virtual void OnScroll(ScrollEventArgs se) {
            ScrollEventHandler handler = (ScrollEventHandler)Events[EVENT_SCROLL];
            if (handler != null) handler(this,se);
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.OnMouseWheel"]/*' />
        /// <devdoc>
        ///     Converts mouse wheel movements into scrolling, when scrollbar has the focus.
        ///     Typically one wheel step will cause one small scroll increment, in either
        ///     direction. A single wheel message could represent either one wheel step, multiple
        ///     wheel steps (fast wheeling), or even a fraction of a step (smooth-wheeled mice).
        ///     So we accumulate the total wheel delta, and consume it in whole numbers of steps.
        /// </devdoc>
        protected override void OnMouseWheel(MouseEventArgs e) {
            wheelDelta += e.Delta;

            bool scrolled = false;

            while (Math.Abs(wheelDelta) >= NativeMethods.WHEEL_DELTA) {
                if (wheelDelta > 0) {
                    wheelDelta -= NativeMethods.WHEEL_DELTA;
                    DoScroll(ScrollEventType.SmallDecrement);
                    scrolled = true;
                }
                else {
                    wheelDelta += NativeMethods.WHEEL_DELTA;
                    DoScroll(ScrollEventType.SmallIncrement);
                    scrolled = true;
                }
            }

            if (scrolled) {
                DoScroll(ScrollEventType.EndScroll);
            }

            if (e is HandledMouseEventArgs) {
                ((HandledMouseEventArgs) e).Handled = true;
            }

            // The base implementation should be called before the implementation above,
            // but changing the order in Whidbey would be too much of a breaking change
            // for this particular class.
            base.OnMouseWheel(e);
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.OnValueChanged"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.ScrollBar.ValueChanged'/> event.
        ///    </para>
        /// </devdoc>
        protected virtual void OnValueChanged(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EVENT_VALUECHANGED];
            if (handler != null) handler(this,e);
        }

        // Reflects the position of the scrollbar
        private int ReflectPosition(int position) {
            if (this is HScrollBar) {
                return minimum + (maximum - LargeChange + 1) - position;
            }
            return position;
        }
        
        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.ToString"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        public override string ToString() {
            string s = base.ToString();
            return s + ", Minimum: " + Minimum.ToString(CultureInfo.CurrentCulture) + ", Maximum: " + Maximum.ToString(CultureInfo.CurrentCulture) + ", Value: " + Value.ToString(CultureInfo.CurrentCulture);
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.UpdateScrollInfo"]/*' />
        /// <devdoc>
        ///     Internal helper method
        /// </devdoc>
        /// <internalonly/>
        protected void UpdateScrollInfo() {
            if (IsHandleCreated && Enabled) {
            
                NativeMethods.SCROLLINFO si = new NativeMethods.SCROLLINFO();
                si.cbSize = Marshal.SizeOf(typeof(NativeMethods.SCROLLINFO));
                si.fMask = NativeMethods.SIF_ALL;
                si.nMin = minimum;
                si.nMax = maximum;
                si.nPage = LargeChange;
                
                if (RightToLeft == RightToLeft.Yes) {
                    // Reflect the scrollbar position horizontally on an Rtl system
                    si.nPos = ReflectPosition(value);
                }
                else {
                    si.nPos = value;
                }
                
                si.nTrackPos = 0;

                UnsafeNativeMethods.SetScrollInfo(new HandleRef(this, Handle), NativeMethods.SB_CTL, si, true);
            }
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.WmReflectScroll"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void WmReflectScroll(ref Message m) {
            ScrollEventType type = (ScrollEventType)NativeMethods.Util.LOWORD(m.WParam);
            DoScroll(type);
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.DoScroll"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void DoScroll(ScrollEventType type) {

            // For Rtl systems we need to swap increment and decrement
            //
            if (RightToLeft == RightToLeft.Yes) {
                switch (type) {
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
            
            int newValue = value;
            int oldValue = value;

            // The ScrollEventArgs constants are defined in terms of the windows
            // messages..  this eliminates confusion between the VSCROLL and
            // HSCROLL constants, which are identical.
            //
            switch (type) {
                case ScrollEventType.First:
                    newValue = minimum;
                    break;

                case ScrollEventType.Last:
                    newValue = maximum - LargeChange + 1; // si.nMax - si.nPage + 1;
                    break;

                case ScrollEventType.SmallDecrement:
                    newValue = Math.Max(value - SmallChange, minimum);
                    break;

                case ScrollEventType.SmallIncrement:
                    newValue = Math.Min(value + SmallChange, maximum - LargeChange + 1); // max - lChange + 1);
                    break;

                case ScrollEventType.LargeDecrement:
                    newValue = Math.Max(value - LargeChange, minimum);
                    break;

                case ScrollEventType.LargeIncrement:
                    newValue = Math.Min(value + LargeChange, maximum - LargeChange + 1); // si.nPos + si.nPage,si.nMax - si.nPage + 1);
                    break;

                case ScrollEventType.ThumbPosition:
                case ScrollEventType.ThumbTrack:
                    NativeMethods.SCROLLINFO si = new NativeMethods.SCROLLINFO();
                    si.fMask = NativeMethods.SIF_TRACKPOS;
                    SafeNativeMethods.GetScrollInfo(new HandleRef(this, Handle), NativeMethods.SB_CTL, si);
                    
                    if (RightToLeft == RightToLeft.Yes) {
                        newValue = ReflectPosition(si.nTrackPos);
                    }
                    else {
                        newValue = si.nTrackPos;
                    }
                    
                    break;
            }

            ScrollEventArgs se = new ScrollEventArgs(type, oldValue, newValue, this.scrollOrientation);
            OnScroll(se);
            Value = se.NewValue;
        }

        /// <include file='doc\ScrollBar.uex' path='docs/doc[@for="ScrollBar.WndProc"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_REFLECT + NativeMethods.WM_HSCROLL:
                case NativeMethods.WM_REFLECT + NativeMethods.WM_VSCROLL:
                    WmReflectScroll(ref m);
                    break;
                case NativeMethods.WM_ERASEBKGND:
                    break;

                case NativeMethods.WM_SIZE:
                    //Fixes the scrollbar focus rect
                    if (UnsafeNativeMethods.GetFocus() == this.Handle) {
                        DefWndProc(ref m);
                        SendMessage(NativeMethods.WM_KILLFOCUS, 0, 0);
                        SendMessage(NativeMethods.WM_SETFOCUS, 0, 0);
                    }
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}

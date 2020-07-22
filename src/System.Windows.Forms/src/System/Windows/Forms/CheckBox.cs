// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms.ButtonInternal;
using System.Windows.Forms.Layout;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a Windows
    ///  check box.
    /// </summary>
    [DefaultProperty(nameof(Checked))]
    [DefaultEvent(nameof(CheckedChanged))]
    [DefaultBindingProperty(nameof(CheckState))]
    [ToolboxItem("System.Windows.Forms.Design.AutoSizeToolboxItem," + AssemblyRef.SystemDesign)]
    [SRDescription(nameof(SR.DescriptionCheckBox))]
    public partial class CheckBox : ButtonBase
    {
        private static readonly object EVENT_CHECKEDCHANGED = new object();
        private static readonly object EVENT_CHECKSTATECHANGED = new object();
        private static readonly object EVENT_APPEARANCECHANGED = new object();
        private const ContentAlignment AnyRight = ContentAlignment.TopRight | ContentAlignment.MiddleRight | ContentAlignment.BottomRight;

        private ContentAlignment _checkAlign = ContentAlignment.MiddleLeft;
        private CheckState _checkState;
        private Appearance _appearance;

        private const int FlatSystemStylePaddingWidth = 25;
        private const int FlatSystemStyleMinimumHeight = 13;

        internal int _flatSystemStylePaddingWidth = FlatSystemStylePaddingWidth;
        internal int _flatSystemStyleMinimumHeight = FlatSystemStyleMinimumHeight;

        /// <summary>
        ///  Initializes a new instance of the <see cref='CheckBox'/> class.
        /// </summary>
        public CheckBox() : base()
        {
            if (DpiHelper.IsScalingRequirementMet)
            {
                _flatSystemStylePaddingWidth = LogicalToDeviceUnits(FlatSystemStylePaddingWidth);
                _flatSystemStyleMinimumHeight = LogicalToDeviceUnits(FlatSystemStyleMinimumHeight);
            }

            // Checkboxes shouldn't respond to right clicks, so we need to do all our own click logic
            SetStyle(ControlStyles.StandardClick |
                     ControlStyles.StandardDoubleClick, false);

            SetAutoSizeMode(AutoSizeMode.GrowAndShrink);

            AutoCheck = true;
            TextAlign = ContentAlignment.MiddleLeft;
        }

        private bool AccObjDoDefaultAction { get; set; }

        /// <summary>
        ///  Gets
        ///  or sets the value that determines the appearance of a
        ///  check box control.
        /// </summary>
        [DefaultValue(Appearance.Normal)]
        [Localizable(true)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.CheckBoxAppearanceDescr))]
        public Appearance Appearance
        {
            get
            {
                return _appearance;
            }

            set
            {
                //valid values are 0x0 to 0x1
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)Appearance.Normal, (int)Appearance.Button))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Appearance));
                }

                if (_appearance != value)
                {
                    using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.Appearance))
                    {
                        _appearance = value;
                        if (OwnerDraw)
                        {
                            Refresh();
                        }
                        else
                        {
                            UpdateStyles();
                        }
                        OnAppearanceChanged(EventArgs.Empty);
                    }
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.CheckBoxOnAppearanceChangedDescr))]
        public event EventHandler AppearanceChanged
        {
            add => Events.AddHandler(EVENT_APPEARANCECHANGED, value);
            remove => Events.RemoveHandler(EVENT_APPEARANCECHANGED, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the <see cref='Checked'/> or <see cref='CheckState'/>
        ///  value and the check box's appearance are automatically
        ///  changed when it is clicked.
        /// </summary>
        [DefaultValue(true)]
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.CheckBoxAutoCheckDescr))]
        public bool AutoCheck { get; set; }

        /// <summary>
        ///  Gets or sets
        ///  the horizontal and vertical alignment of a check box on a check box
        ///  control.
        /// </summary>
        [Bindable(true)]
        [Localizable(true)]
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        [SRDescription(nameof(SR.CheckBoxCheckAlignDescr))]
        public ContentAlignment CheckAlign
        {
            get
            {
                return _checkAlign;
            }
            set
            {
                if (!WindowsFormsUtils.EnumValidator.IsValidContentAlignment(value))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ContentAlignment));
                }

                if (_checkAlign != value)
                {
                    _checkAlign = value;
                    LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.CheckAlign);
                    if (OwnerDraw)
                    {
                        Invalidate();
                    }
                    else
                    {
                        UpdateStyles();
                    }
                }
            }
        }

        /// <summary>
        ///  Gets
        ///  or sets a value indicating whether the
        ///  check box
        ///  is checked.
        /// </summary>
        [Bindable(true),
        SettingsBindable(true)]
        [DefaultValue(false)]
        [SRCategory(nameof(SR.CatAppearance))]
        [RefreshProperties(RefreshProperties.All)]
        [SRDescription(nameof(SR.CheckBoxCheckedDescr))]
        public bool Checked
        {
            get
            {
                return _checkState != CheckState.Unchecked;
            }

            set
            {
                if (value != Checked)
                {
                    CheckState = value ? CheckState.Checked : CheckState.Unchecked;
                }
            }
        }

        /// <summary>
        ///  Gets
        ///  or sets a value indicating whether the check box is checked.
        /// </summary>
        [Bindable(true)]
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(CheckState.Unchecked)]
        [RefreshProperties(RefreshProperties.All)]
        [SRDescription(nameof(SR.CheckBoxCheckStateDescr))]
        public CheckState CheckState
        {
            get
            {
                return _checkState;
            }

            set
            {
                // valid values are 0-2 inclusive.
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)CheckState.Unchecked, (int)CheckState.Indeterminate))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(CheckState));
                }

                if (_checkState != value)
                {
                    bool oldChecked = Checked;

                    _checkState = value;

                    if (IsHandleCreated)
                    {
                        User32.SendMessageW(this, (User32.WM)User32.BM.SETCHECK, (IntPtr)_checkState);
                    }

                    if (oldChecked != Checked)
                    {
                        OnCheckedChanged(EventArgs.Empty);
                    }
                    OnCheckStateChanged(EventArgs.Empty);
                }
            }
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler DoubleClick
        {
            add => base.DoubleClick += value;
            remove => base.DoubleClick -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler MouseDoubleClick
        {
            add => base.MouseDoubleClick += value;
            remove => base.MouseDoubleClick -= value;
        }

        /// <summary>
        ///  Gets the information used to create the handle for the
        ///  <see cref='CheckBox'/>
        ///  control.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassName = ComCtl32.WindowClasses.WC_BUTTON;
                if (OwnerDraw)
                {
                    cp.Style |= (int)User32.BS.OWNERDRAW;
                }
                else
                {
                    cp.Style |= (int)User32.BS.THREE_STATE;
                    if (Appearance == Appearance.Button)
                    {
                        cp.Style |= (int)User32.BS.PUSHLIKE;
                    }

                    // Determine the alignment of the check box
                    //
                    ContentAlignment align = RtlTranslateContent(CheckAlign);
                    if ((int)(align & AnyRight) != 0)
                    {
                        cp.Style |= (int)User32.BS.RIGHTBUTTON;
                    }
                }

                return cp;
            }
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(104, 24);
            }
        }

        /// <summary>
        ///  When overridden in a derived class, handles rescaling of any magic numbers used in control painting.
        ///  For CheckBox controls, scale the width of the system-style padding, and height of the box.
        ///  Must call the base class method to get the current DPI values. This method is invoked only when
        ///  Application opts-in into the Per-monitor V2 support, targets .NETFX 4.7 and has
        ///  EnableDpiChangedMessageHandling and EnableDpiChangedHighDpiImprovements config switches turned on.
        /// </summary>
        /// <param name="deviceDpiOld">Old DPI value</param>
        /// <param name="deviceDpiNew">New DPI value</param>
        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
        {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);

            _flatSystemStylePaddingWidth = LogicalToDeviceUnits(FlatSystemStylePaddingWidth);
            _flatSystemStyleMinimumHeight = LogicalToDeviceUnits(FlatSystemStyleMinimumHeight);
        }

        internal override Size GetPreferredSizeCore(Size proposedConstraints)
        {
            if (Appearance == Appearance.Button)
            {
                ButtonStandardAdapter adapter = new ButtonStandardAdapter(this);
                return adapter.GetPreferredSizeCore(proposedConstraints);
            }

            if (FlatStyle != FlatStyle.System)
            {
                return base.GetPreferredSizeCore(proposedConstraints);
            }

            Size textSize = TextRenderer.MeasureText(Text, Font);
            Size size = SizeFromClientSize(textSize);
            size.Width += _flatSystemStylePaddingWidth;
            size.Height = Math.Max(size.Height + 5, _flatSystemStyleMinimumHeight); // ensure minimum height to avoid truncation of check-box or text
            return size + Padding.Size;
        }

        internal override Rectangle OverChangeRectangle
        {
            get
            {
                if (Appearance == Appearance.Button)
                {
                    return base.OverChangeRectangle;
                }
                else
                {
                    if (FlatStyle == FlatStyle.Standard)
                    {
                        // this Rectangle will cause no Invalidation
                        // can't use Rectangle.Empty because it will cause Invalidate(ClientRectangle)
                        return new Rectangle(-1, -1, 1, 1);
                    }
                    else
                    {
                        // Popup mouseover rectangle is actually bigger than GetCheckmarkRectangle
                        return Adapter.CommonLayout().Layout().checkBounds;
                    }
                }
            }
        }

        internal override Rectangle DownChangeRectangle
        {
            get
            {
                if (Appearance == Appearance.Button || FlatStyle == FlatStyle.System)
                {
                    return base.DownChangeRectangle;
                }
                else
                {
                    // Popup mouseover rectangle is actually bigger than GetCheckmarkRectangle()
                    return Adapter.CommonLayout().Layout().checkBounds;
                }
            }
        }

        internal override bool SupportsUiaProviders => true;

        /// <summary>
        ///  Gets or sets a value indicating the alignment of the
        ///  text on the checkbox control.
        /// </summary>
        [Localizable(true)]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        public override ContentAlignment TextAlign
        {
            get => base.TextAlign;
            set => base.TextAlign = value;
        }

        /// <summary>
        ///  Gets or sets a value indicating
        ///  whether the check box will allow three check states rather than two.
        /// </summary>
        [DefaultValue(false)]
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.CheckBoxThreeStateDescr))]
        public bool ThreeState { get; set; }

        /// <summary>
        ///  Occurs when the
        ///  value of the <see cref='Checked'/>
        ///  property changes.
        /// </summary>
        [SRDescription(nameof(SR.CheckBoxOnCheckedChangedDescr))]
        public event EventHandler CheckedChanged
        {
            add => Events.AddHandler(EVENT_CHECKEDCHANGED, value);
            remove => Events.RemoveHandler(EVENT_CHECKEDCHANGED, value);
        }

        /// <summary>
        ///  Occurs when the
        ///  value of the <see cref='CheckState'/>
        ///  property changes.
        /// </summary>
        [SRDescription(nameof(SR.CheckBoxOnCheckStateChangedDescr))]
        public event EventHandler CheckStateChanged
        {
            add => Events.AddHandler(EVENT_CHECKSTATECHANGED, value);
            remove => Events.RemoveHandler(EVENT_CHECKSTATECHANGED, value);
        }

        /// <summary>
        ///  Constructs the new instance of the accessibility object for this control. Subclasses
        ///  should not call base.CreateAccessibilityObject.
        /// </summary>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new CheckBoxAccessibleObject(this);
        }

        protected virtual void OnAppearanceChanged(EventArgs e)
        {
            if (Events[EVENT_APPEARANCECHANGED] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        /// <summary>
        ///  Raises the <see cref='CheckedChanged'/>
        ///  event.
        /// </summary>
        protected virtual void OnCheckedChanged(EventArgs e)
        {
            // accessibility stuff
            if (FlatStyle == FlatStyle.System)
            {
                AccessibilityNotifyClients(AccessibleEvents.SystemCaptureStart, -1);
            }

            // MSAA events:
            AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
            AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);

            // UIA events:
            AccessibilityObject.RaiseAutomationPropertyChangedEvent(UiaCore.UIA.NamePropertyId, Name, Name);
            AccessibilityObject.RaiseAutomationEvent(UiaCore.UIA.AutomationPropertyChangedEventId);

            if (FlatStyle == FlatStyle.System)
            {
                AccessibilityNotifyClients(AccessibleEvents.SystemCaptureEnd, -1);
            }

            ((EventHandler)Events[EVENT_CHECKEDCHANGED])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='CheckStateChanged'/> event.
        /// </summary>
        protected virtual void OnCheckStateChanged(EventArgs e)
        {
            if (OwnerDraw)
            {
                Refresh();
            }

            ((EventHandler)Events[EVENT_CHECKSTATECHANGED])?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the event indicating that the control has been clicked.
        ///  Inheriting controls should use this in favour of actually listening to
        ///  the event, but should not forget to call base.onClicked() to
        ///  ensure that the event is still fired for external listeners.
        /// </summary>
        protected override void OnClick(EventArgs e)
        {
            if (AutoCheck)
            {
                switch (CheckState)
                {
                    case CheckState.Unchecked:
                        CheckState = CheckState.Checked;
                        break;
                    case CheckState.Checked:
                        if (ThreeState)
                        {
                            CheckState = CheckState.Indeterminate;

                            // If the check box is clicked as a result of AccObj::DoDefaultAction
                            // then the native check box does not fire OBJ_STATE_CHANGE event when going to Indeterminate state.
                            // So the WinForms layer fires the OBJ_STATE_CHANGE event.
                            if (AccObjDoDefaultAction)
                            {
                                AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
                            }
                        }
                        else
                        {
                            CheckState = CheckState.Unchecked;
                        }
                        break;
                    default:
                        CheckState = CheckState.Unchecked;
                        break;
                }
            }
            base.OnClick(e);
        }

        /// <summary>
        ///  We override this to ensure that the control's click values are set up
        ///  correctly.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (IsHandleCreated)
            {
                User32.SendMessageW(this, (User32.WM)User32.BM.SETCHECK, (IntPtr)_checkState);
            }
        }

        /// <summary>
        ///  Raises the <see cref='ButtonBase.OnMouseUp'/> event.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left && MouseIsPressed)
            {
                // It's best not to have the mouse captured while running Click events
                if (base.MouseIsDown)
                {
                    Point pt = PointToScreen(new Point(mevent.X, mevent.Y));
                    if (User32.WindowFromPoint(pt) == Handle)
                    {
                        //Paint in raised state...
                        ResetFlagsandPaint();
                        if (!ValidationCancelled)
                        {
                            if (Capture)
                            {
                                OnClick(mevent);
                            }
                            OnMouseClick(mevent);
                        }
                    }
                }
            }
            base.OnMouseUp(mevent);
        }

        internal override ButtonBaseAdapter CreateFlatAdapter()
        {
            return new CheckBoxFlatAdapter(this);
        }

        internal override ButtonBaseAdapter CreatePopupAdapter()
        {
            return new CheckBoxPopupAdapter(this);
        }

        internal override ButtonBaseAdapter CreateStandardAdapter()
        {
            return new CheckBoxStandardAdapter(this);
        }

        /// <summary>
        ///  Overridden to handle mnemonics properly.
        /// </summary>
        protected internal override bool ProcessMnemonic(char charCode)
        {
            if (UseMnemonic && IsMnemonic(charCode, Text) && CanSelect)
            {
                if (Focus())
                {
                    //Paint in raised state...
                    //
                    ResetFlagsandPaint();
                    if (!ValidationCancelled)
                    {
                        OnClick(EventArgs.Empty);
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        ///  Provides some interesting information for the CheckBox control in
        ///  String form.
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();
            // We shouldn't need to convert the enum to int
            int checkState = (int)CheckState;
            return s + ", CheckState: " + checkState.ToString(CultureInfo.InvariantCulture);
        }
    }
}

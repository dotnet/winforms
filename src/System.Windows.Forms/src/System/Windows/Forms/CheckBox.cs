// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;

    using System.Diagnostics;

    using System;
    using System.Security.Permissions;
    using System.Windows.Forms.ButtonInternal;

    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Windows.Forms.Layout;

    using System.Drawing;
    using System.Windows.Forms.Internal;
    using System.Drawing.Drawing2D;
    using Microsoft.Win32;
    using System.Globalization;

    /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox"]/*' />
    /// <devdoc>
    ///    <para> Represents a Windows
    ///       check box.</para>
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultProperty(nameof(Checked)),
    DefaultEvent(nameof(CheckedChanged)),
    DefaultBindingProperty(nameof(CheckState)),
    ToolboxItem("System.Windows.Forms.Design.AutoSizeToolboxItem," + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionCheckBox))
    ]
    public class CheckBox : ButtonBase {
        private static readonly object EVENT_CHECKEDCHANGED = new object();
        private static readonly object EVENT_CHECKSTATECHANGED = new object();
        private static readonly object EVENT_APPEARANCECHANGED = new object();
        static readonly ContentAlignment anyRight  = ContentAlignment.TopRight | ContentAlignment.MiddleRight | ContentAlignment.BottomRight;

        private bool autoCheck;
        private bool threeState;
        private bool accObjDoDefaultAction = false;

        private ContentAlignment checkAlign = ContentAlignment.MiddleLeft;
        private CheckState checkState;
        private Appearance appearance;

        private const int FlatSystemStylePaddingWidth = 25;
        private const int FlatSystemStyleMinimumHeight = 13;

        internal int flatSystemStylePaddingWidth = FlatSystemStylePaddingWidth;
        internal int flatSystemStyleMinimumHeight = FlatSystemStyleMinimumHeight;

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.CheckBox"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.CheckBox'/> class.
        ///    </para>
        /// </devdoc>
        public CheckBox()
        : base() {

            if (DpiHelper.IsScalingRequirementMet) {
                flatSystemStylePaddingWidth = LogicalToDeviceUnits(FlatSystemStylePaddingWidth);
                flatSystemStyleMinimumHeight = LogicalToDeviceUnits(FlatSystemStyleMinimumHeight);
            }

            // Checkboxes shouldn't respond to right clicks, so we need to do all our own click logic
            SetStyle(ControlStyles.StandardClick |
                     ControlStyles.StandardDoubleClick, false);

            SetAutoSizeMode(AutoSizeMode.GrowAndShrink);
        
            autoCheck = true;
            TextAlign = ContentAlignment.MiddleLeft;        
            
        }
        
        private bool AccObjDoDefaultAction {
            get {
                return this.accObjDoDefaultAction;
            }
            set {
                this.accObjDoDefaultAction = value;
            }
        }

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.Appearance"]/*' />
        /// <devdoc>
        ///    <para>Gets
        ///       or sets the value that determines the appearance of a
        ///       check box control.</para>
        /// </devdoc>
        [
        DefaultValue(Appearance.Normal),
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.CheckBoxAppearanceDescr))
        ]
        public Appearance Appearance {
            get {
                return appearance;
            }

            set {
                //valid values are 0x0 to 0x1
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)Appearance.Normal, (int)Appearance.Button)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Appearance));
                }

                if (appearance != value) {
                    using (LayoutTransaction.CreateTransactionIf(AutoSize, this.ParentInternal, this, PropertyNames.Appearance)) {
                        appearance = value;
                        if (OwnerDraw) {
                            Refresh();
                        }
                        else {
                            UpdateStyles();
                        }
                        OnAppearanceChanged(EventArgs.Empty);
                    }
                }
            }
        }

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.AppearanceChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.CheckBoxOnAppearanceChangedDescr))]
        public event EventHandler AppearanceChanged {
            add {
                Events.AddHandler(EVENT_APPEARANCECHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_APPEARANCECHANGED, value);
            }
        }

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.AutoCheck"]/*' />
        /// <devdoc>
        /// <para>Gets or sets a value indicating whether the <see cref='System.Windows.Forms.CheckBox.Checked'/> or <see cref='System.Windows.Forms.CheckBox.CheckState'/>
        /// value and the check box's appearance are automatically
        /// changed when it is clicked.</para>
        /// </devdoc>
        [
        DefaultValue(true),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.CheckBoxAutoCheckDescr))
        ]
        public bool AutoCheck {
            get {
                return autoCheck;
            }

            set {
                autoCheck = value;
            }
        }

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.CheckAlign"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the horizontal and vertical alignment of a check box on a check box
        ///       control.
        ///       
        ///    </para>
        /// </devdoc>
        [
        Bindable(true),
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(ContentAlignment.MiddleLeft),
        SRDescription(nameof(SR.CheckBoxCheckAlignDescr))
        ]
        public ContentAlignment CheckAlign {
            get {
                return checkAlign;
            }
            set {
                if (!WindowsFormsUtils.EnumValidator.IsValidContentAlignment(value)) {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ContentAlignment));
                }

                if (checkAlign != value) {
                    checkAlign = value;
                    LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.CheckAlign);
                    if (OwnerDraw) {
                        Invalidate();
                    }
                    else {
                        UpdateStyles();
                    }
                }
            }
        }

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.Checked"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       or sets a value indicating whether the
        ///       check box
        ///       is checked.
        ///    </para>
        /// </devdoc>
        [
        Bindable(true),
        SettingsBindable(true),
        DefaultValue(false),
        SRCategory(nameof(SR.CatAppearance)),
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.CheckBoxCheckedDescr))
        ]
        public bool Checked {
            get {
                return checkState != CheckState.Unchecked;
            }

            set {
                if (value != Checked) {
                    CheckState = value ? CheckState.Checked : CheckState.Unchecked;
                }
            }
        }

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.CheckState"]/*' />
        /// <devdoc>
        ///    <para>Gets
        ///       or sets a value indicating whether the check box is checked.</para>
        /// </devdoc>
        [
        Bindable(true),
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(CheckState.Unchecked),
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.CheckBoxCheckStateDescr))
        ]
        public CheckState CheckState {
            get {
                return checkState;
            }

            set {
                // valid values are 0-2 inclusive.
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)CheckState.Unchecked, (int)CheckState.Indeterminate)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(CheckState));
                }

                if (checkState != value) {
                
                    bool oldChecked = Checked;
                
                    checkState = value;

                    if (IsHandleCreated) {
                        SendMessage(NativeMethods.BM_SETCHECK, (int)checkState, 0);
                    }
                    
                    if (oldChecked != Checked) {
                        OnCheckedChanged(EventArgs.Empty);
                    }
                    OnCheckStateChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.DoubleClick"]/*' />
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

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.MouseDoubleClick"]/*' />
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

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.CreateParams"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Gets the information used to create the handle for the
        ///    <see cref='System.Windows.Forms.CheckBox'/>
        ///    control.
        /// </para>
        /// </devdoc>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;
                cp.ClassName = "BUTTON";
                if (OwnerDraw) {
                    cp.Style |= NativeMethods.BS_OWNERDRAW;
                }
                else {
                    cp.Style |= NativeMethods.BS_3STATE;
                    if (Appearance == Appearance.Button) {
                        cp.Style |= NativeMethods.BS_PUSHLIKE;
                    }
                    
                    // Determine the alignment of the check box
                    //
                    ContentAlignment align = RtlTranslateContent(CheckAlign);                              
                    if ((int)(align & anyRight) != 0) {
                        cp.Style |= NativeMethods.BS_RIGHTBUTTON;
                    }

                }

                return cp;
            }
        }
        
        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.DefaultSize"]/*' />
        /// <devdoc>
        ///     Deriving classes can override this to configure a default size for their control.
        ///     This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Size DefaultSize {
            get {
                return new Size(104, 24);
            }
        }

        /// <summary>
        /// When overridden in a derived class, handles rescaling of any magic numbers used in control painting.
        /// For CheckBox controls, scale the width of the system-style padding, and height of the box.
        /// Must call the base class method to get the current DPI values. This method is invoked only when 
        /// Application opts-in into the Per-monitor V2 support, targets .NETFX 4.7 and has 
        /// EnableDpiChangedMessageHandling and EnableDpiChangedHighDpiImprovements config switches turned on.
        /// </summary>
        /// <param name="deviceDpiOld">Old DPI value</param>
        /// <param name="deviceDpiNew">New DPI value</param>
        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew) {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);

            flatSystemStylePaddingWidth = LogicalToDeviceUnits(FlatSystemStylePaddingWidth);
            flatSystemStyleMinimumHeight = LogicalToDeviceUnits(FlatSystemStyleMinimumHeight);
        }

        internal override Size GetPreferredSizeCore(Size proposedConstraints) {
            if (Appearance == Appearance.Button) {
                ButtonStandardAdapter adapter = new ButtonStandardAdapter(this);
                return adapter.GetPreferredSizeCore(proposedConstraints);
            } 

            if(FlatStyle != FlatStyle.System) {
                return base.GetPreferredSizeCore(proposedConstraints);
            }

            Size textSize = TextRenderer.MeasureText(this.Text, this.Font);
            Size size = SizeFromClientSize(textSize);
            size.Width += flatSystemStylePaddingWidth;
            size.Height = Math.Max(size.Height + 5, flatSystemStyleMinimumHeight); // ensure minimum height to avoid truncation of check-box or text
            return size + Padding.Size;
        }

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.OverChangeRectangle"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        internal override Rectangle OverChangeRectangle {
            get {
                if (Appearance == Appearance.Button) {
                    return base.OverChangeRectangle;
                }
                else {
                    if (FlatStyle == FlatStyle.Standard) {
                        // this Rectangle will cause no Invalidation
                        // can't use Rectangle.Empty because it will cause Invalidate(ClientRectangle)
                        return new Rectangle(-1, -1, 1, 1);
                    }
                    else {
                        // Popup mouseover rectangle is actually bigger than GetCheckmarkRectangle
                        return Adapter.CommonLayout().Layout().checkBounds;
                    }
                }
            }
        }
        
        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.DownChangeRectangle"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        internal override Rectangle DownChangeRectangle {
            get {
                if (Appearance == Appearance.Button || FlatStyle == FlatStyle.System) {
                    return base.DownChangeRectangle;
                }
                else {
                    // Popup mouseover rectangle is actually bigger than GetCheckmarkRectangle()
                    return Adapter.CommonLayout().Layout().checkBounds;
                }
            }
        }
        
        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.TextAlign"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating the alignment of the
        ///       text on the checkbox control.
        ///       
        ///    </para>
        /// </devdoc>
        [
        Localizable(true),
        DefaultValue(ContentAlignment.MiddleLeft)
        ]
        public override ContentAlignment TextAlign {
            get {
                return base.TextAlign;
            }
            set {
                base.TextAlign = value;
            }
        }

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.ThreeState"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets a value indicating
        ///       whether the check box will allow three check states rather than two.</para>
        /// </devdoc>
        [
        DefaultValue(false),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.CheckBoxThreeStateDescr))
        ]
        public bool ThreeState {
            get {
                return threeState;
            }
            set {
                threeState = value;
            }
        }

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.CheckedChanged"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the
        ///       value of the <see cref='System.Windows.Forms.CheckBox.Checked'/>
        ///       property changes.</para>
        /// </devdoc>
        [SRDescription(nameof(SR.CheckBoxOnCheckedChangedDescr))]
        public event EventHandler CheckedChanged {
            add {
                Events.AddHandler(EVENT_CHECKEDCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_CHECKEDCHANGED, value);
            }
        }
        
        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.CheckStateChanged"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the
        ///       value of the <see cref='System.Windows.Forms.CheckBox.CheckState'/>
        ///       property changes.</para>
        /// </devdoc>
        [SRDescription(nameof(SR.CheckBoxOnCheckStateChangedDescr))]
        public event EventHandler CheckStateChanged {
            add {
                Events.AddHandler(EVENT_CHECKSTATECHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_CHECKSTATECHANGED, value);
            }
        }

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.CreateAccessibilityInstance"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Constructs the new instance of the accessibility object for this control. Subclasses
        ///       should not call base.CreateAccessibilityObject.
        ///    </para>
        /// </devdoc>
        protected override AccessibleObject CreateAccessibilityInstance() {
            return new CheckBoxAccessibleObject(this);
        }
        
        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.OnAppearanceChanged"]/*' />
        protected virtual void OnAppearanceChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_APPEARANCECHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.OnCheckedChanged"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.CheckBox.CheckedChanged'/>
        /// event.</para>
        /// </devdoc>
        protected virtual void OnCheckedChanged(EventArgs e) {
            // accessibility stuff
            if (this.FlatStyle == FlatStyle.System) {
                AccessibilityNotifyClients(AccessibleEvents.SystemCaptureStart, -1);
            }

            AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
            AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);

            if (this.FlatStyle == FlatStyle.System) {
                AccessibilityNotifyClients(AccessibleEvents.SystemCaptureEnd, -1);
            }

            EventHandler handler = (EventHandler)Events[EVENT_CHECKEDCHANGED];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.OnCheckStateChanged"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.CheckBox.CheckStateChanged'/> event.</para>
        /// </devdoc>
        protected virtual void OnCheckStateChanged(EventArgs e) {
            if (OwnerDraw) {
                Refresh();
            }
            
            EventHandler handler = (EventHandler)Events[EVENT_CHECKSTATECHANGED];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.OnClick"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Fires the event indicating that the control has been clicked.
        ///       Inheriting controls should use this in favour of actually listening to
        ///       the event, but should not forget to call base.onClicked() to
        ///       ensure that the event is still fired for external listeners.
        ///       
        ///    </para>
        /// </devdoc>
        protected override void OnClick(EventArgs e) {
            if (autoCheck) {
                switch (CheckState) {
                    case CheckState.Unchecked:
                        CheckState = CheckState.Checked;
                        break;
                    case CheckState.Checked:
                        if (threeState) {
                            CheckState = CheckState.Indeterminate;

                            // If the check box is clicked as a result of AccObj::DoDefaultAction
                            // then the native check box does not fire OBJ_STATE_CHANGE event when going to Indeterminate state.
                            // So the WinForms layer fires the OBJ_STATE_CHANGE event.
                            if (this.AccObjDoDefaultAction) {
                                AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
                            }
                        }
                        else {
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

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.OnHandleCreated"]/*' />
        /// <devdoc>
        ///     We override this to ensure that the control's click values are set up
        ///     correctly.
        /// </devdoc>
        /// <internalonly/>
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            
            // Since this is a protected override...
            // this can be directly called in by a overriden class..
            // and the Handle need not be created... 
            // So Check for the handle
            if (IsHandleCreated) {
                SendMessage(NativeMethods.BM_SETCHECK, (int)checkState, 0);
            }
        }

        /// <devdoc>
        ///     We override this to ensure that press '+' or '=' checks the box,
        ///     while pressing '-' unchecks the box
        /// </devdoc>
        /// <internalonly/>
        protected override void OnKeyDown(KeyEventArgs e) {
            /*
            if (Enabled) {
                if (e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.Add) {
                    CheckState = CheckState.Checked;
                }
                if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract) {
                    CheckState = CheckState.Unchecked;
                }
            }
            */
            base.OnKeyDown(e);
        }
        
        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.OnMouseUp"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.ButtonBase.OnMouseUp'/> event.
        ///       
        ///    </para>
        /// </devdoc>
        protected override void OnMouseUp(MouseEventArgs mevent) {
            if (mevent.Button == MouseButtons.Left && MouseIsPressed) {
                // It's best not to have the mouse captured while running Click events
                if (base.MouseIsDown) {
                    Point pt = PointToScreen(new Point(mevent.X, mevent.Y));
                    if (UnsafeNativeMethods.WindowFromPoint(pt.X, pt.Y) == Handle) {
                        //Paint in raised state...
                        ResetFlagsandPaint();
                        if (!ValidationCancelled) {
                            if (this.Capture) {
                                OnClick(mevent);
                            }
                            OnMouseClick(mevent);
                        }
                    }
                }
            }
            base.OnMouseUp(mevent);
        }

        internal override ButtonBaseAdapter CreateFlatAdapter() {
            return new CheckBoxFlatAdapter(this);
        }

        internal override ButtonBaseAdapter CreatePopupAdapter() {
            return new CheckBoxPopupAdapter(this);
        }
            
        internal override ButtonBaseAdapter CreateStandardAdapter() {
            return new CheckBoxStandardAdapter(this);
        }

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.ProcessMnemonic"]/*' />
        /// <devdoc>
        ///     Overridden to handle mnemonics properly.
        /// </devdoc>
        /// <internalonly/>        
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected internal override bool ProcessMnemonic(char charCode) {
            if (UseMnemonic && IsMnemonic(charCode, Text) && CanSelect) {
                if (FocusInternal()) {
                    //Paint in raised state...
                    //
                    ResetFlagsandPaint();
                    if (!ValidationCancelled) {
                        OnClick(EventArgs.Empty);
                    }
                    
                }
                return true;
            }
            return false;
        }

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.ToString"]/*' />
        /// <devdoc>
        ///     Provides some interesting information for the CheckBox control in
        ///     String form.
        /// </devdoc>
        /// <internalonly/>
        public override string ToString() {

            string s = base.ToString();
            // We shouldn't need to convert the enum to int
            int checkState = (int)CheckState;
            return s + ", CheckState: " + checkState.ToString(CultureInfo.InvariantCulture);
        }

        /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.CheckBoxAccessibleObject"]/*' />
        /// <internalonly/>        
        /// <devdoc>
        /// </devdoc>
        [System.Runtime.InteropServices.ComVisible(true)]        
        public class CheckBoxAccessibleObject : ButtonBaseAccessibleObject {

            /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.CheckBoxAccessibleObject.CheckBoxAccessibleObject"]/*' />
            public CheckBoxAccessibleObject(Control owner) : base(owner) {
            }

            /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.CheckBoxAccessibleObject.DefaultAction"]/*' />
            public override string DefaultAction {
                get {
                    string defaultAction = Owner.AccessibleDefaultActionDescription;
                    if (defaultAction != null) {
                        return defaultAction;
                    }

                    if (((CheckBox)Owner).Checked) {
                        return SR.AccessibleActionUncheck;
                    }
                    else {
                        return SR.AccessibleActionCheck;
                    }
                }
            }

            /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.CheckBoxAccessibleObject.Role"]/*' />
            public override AccessibleRole Role {
                get {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default) {
                        return role;
                    }
                    return AccessibleRole.CheckButton;
                }
            }
            
            /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.CheckBoxAccessibleObject.State"]/*' />
            public override AccessibleStates State {
                get {
                    switch (((CheckBox)Owner).CheckState) {
                        case CheckState.Checked:
                            return AccessibleStates.Checked | base.State;
                        case CheckState.Indeterminate:
                            return AccessibleStates.Indeterminate | base.State;
                    }

                    return base.State;
                }
            }                        

            /// <include file='doc\CheckBox.uex' path='docs/doc[@for="CheckBox.CheckBoxAccessibleObject.DoDefaultAction"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void DoDefaultAction() {
                CheckBox cb = this.Owner as CheckBox;

                if (cb != null) {
                    cb.AccObjDoDefaultAction = true;
                }

                try {
                    base.DoDefaultAction();
                } finally {
                    if (cb != null) {
                        cb.AccObjDoDefaultAction = false;
                    }
                }

            }
        }
    }
}


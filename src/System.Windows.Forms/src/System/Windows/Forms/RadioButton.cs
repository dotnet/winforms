// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.InteropServices;

    using System.Diagnostics;

    using System;
    using System.Security.Permissions;
    using System.Windows.Forms.ButtonInternal;

    using System.ComponentModel;
    using System.ComponentModel.Design;

    using System.Drawing;
    using System.Windows.Forms.Internal;

    using System.Drawing.Drawing2D;
    using System.Windows.Forms.Layout;
    using Microsoft.Win32;

    /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Encapsulates a
    ///       standard
    ///       Windows radio button (option button).
    ///    </para>
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultProperty(nameof(Checked)),
    DefaultEvent(nameof(CheckedChanged)),
    DefaultBindingProperty(nameof(Checked)),
    ToolboxItem("System.Windows.Forms.Design.AutoSizeToolboxItem," + AssemblyRef.SystemDesign),
    Designer("System.Windows.Forms.Design.RadioButtonDesigner, " + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionRadioButton))
    ]
    public class RadioButton : ButtonBase {

        private static readonly object EVENT_CHECKEDCHANGED = new object();
        private static readonly ContentAlignment anyRight  = ContentAlignment.TopRight | ContentAlignment.MiddleRight | ContentAlignment.BottomRight;

        // Used to see if we need to iterate through the autochecked items and modify their tabstops.
        private bool firstfocus = true;
        private bool isChecked;
        private bool autoCheck = true;
        private ContentAlignment checkAlign = ContentAlignment.MiddleLeft;
        private Appearance appearance        = System.Windows.Forms.Appearance.Normal;

        private const int FlatSystemStylePaddingWidth = 24;
        private const int FlatSystemStyleMinimumHeight = 13;

        internal int flatSystemStylePaddingWidth = FlatSystemStylePaddingWidth;
        internal int flatSystemStyleMinimumHeight = FlatSystemStyleMinimumHeight;

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.RadioButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.RadioButton'/>
        ///       class.
        ///    </para>
        /// </devdoc>
        public RadioButton() : base() {
            if (DpiHelper.IsScalingRequirementMet) {
                flatSystemStylePaddingWidth = LogicalToDeviceUnits(FlatSystemStylePaddingWidth);
                flatSystemStyleMinimumHeight = LogicalToDeviceUnits(FlatSystemStyleMinimumHeight);
            }

            // Radiobuttons shouldn't respond to right clicks, so we need to do all our own click logic
            SetStyle(ControlStyles.StandardClick, false);

            TextAlign = ContentAlignment.MiddleLeft;
            TabStop = false;
            SetAutoSizeMode(AutoSizeMode.GrowAndShrink);
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.AutoCheck"]/*' />
        /// <devdoc>
        /// <para>Gets or sets a value indicating whether the <see cref='System.Windows.Forms.RadioButton.Checked'/>
        /// value and the appearance of
        /// the control automatically change when the control is clicked.</para>
        /// </devdoc>
        [
        DefaultValue(true),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.RadioButtonAutoCheckDescr))
        ]
        public bool AutoCheck {
            get {
                return autoCheck;
            }

            set {
                if (autoCheck != value) {
                    autoCheck = value;
                    PerformAutoUpdates(false);
                }
            }
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.Appearance"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the appearance of the radio
        ///       button
        ///       control is drawn.
        ///    </para>
        /// </devdoc>
        [
        DefaultValue(Appearance.Normal),
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        SRDescription(nameof(SR.RadioButtonAppearanceDescr))
        ]
        public Appearance Appearance {
            get {
                return appearance;
            }

            set {
                if (appearance != value) {
                    //valid values are 0x0 to 0x1
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)Appearance.Normal, (int)Appearance.Button)){
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Appearance));
                    }

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

        private static readonly object EVENT_APPEARANCECHANGED = new object();

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.AppearanceChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.RadioButtonOnAppearanceChangedDescr))]
        public event EventHandler AppearanceChanged {
            add {
                Events.AddHandler(EVENT_APPEARANCECHANGED, value);
            }

            remove {
                Events.RemoveHandler(EVENT_APPEARANCECHANGED, value);
            }
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.CheckAlign"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or
        ///       sets the location of the check box portion of the
        ///       radio button control.
        ///       
        ///    </para>
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(ContentAlignment.MiddleLeft),
        SRDescription(nameof(SR.RadioButtonCheckAlignDescr))
        ]
        public ContentAlignment CheckAlign {
            get {
                return checkAlign;
            }
            set {
                if (!WindowsFormsUtils.EnumValidator.IsValidContentAlignment(value)) {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ContentAlignment));
                }

                checkAlign = value;
                if (OwnerDraw) {
                    Invalidate();
                }
                else {
                    UpdateStyles();
                }
            }
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.Checked"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the
        ///       control is checked or not.
        ///       
        ///    </para>
        /// </devdoc>
        [
        Bindable(true),
        SettingsBindable(true),
        DefaultValue(false),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.RadioButtonCheckedDescr))
        ]
        public bool Checked {
            get {
                return isChecked;
            }

            set {
                if (isChecked != value) {
                    isChecked = value;

                    if (IsHandleCreated) SendMessage(NativeMethods.BM_SETCHECK, value? 1: 0, 0);
                    Invalidate();
                    Update();
                    PerformAutoUpdates(false);
                    OnCheckedChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.DoubleClick"]/*' />
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

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.MouseDoubleClick"]/*' />
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

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.CreateParams"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;
                cp.ClassName = "BUTTON";
                if (OwnerDraw) {
                    cp.Style |= NativeMethods.BS_OWNERDRAW;
                }
                else {
                    cp.Style |= NativeMethods.BS_RADIOBUTTON;
                    if (Appearance == Appearance.Button) {
                        cp.Style |= NativeMethods.BS_PUSHLIKE;
                    }
                    
                    // Determine the alignment of the radio button
                    //
                    ContentAlignment align = RtlTranslateContent(CheckAlign);                              
                    if ((int)(align & anyRight) != 0) {
                        cp.Style |= NativeMethods.BS_RIGHTBUTTON;
                    }
                }
                return cp;
            }
        }
        
        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.DefaultSize"]/*' />
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
        /// For RadioButton controls, scale the width of the system-style padding and height of the radio button image.
        /// Must call the base class method to get the current DPI values. This method is invoked only when 
        /// Application opts-in into the Per-monitor V2 support, targets .NETFX 4.7 and has 
        /// EnableDpiChangedMessageHandling and EnableDpiChangedHighDpiImprovements config switches turned on.
        /// </summary>
        /// <param name="deviceDpiOld">Old DPI value</param>
        /// <param name="deviceDpiNew">New DPI value</param>
        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew) {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);

            if (DpiHelper.IsScalingRequirementMet) {
                flatSystemStylePaddingWidth = LogicalToDeviceUnits(FlatSystemStylePaddingWidth);
                flatSystemStyleMinimumHeight = LogicalToDeviceUnits(FlatSystemStyleMinimumHeight);
            }
        }

        internal override Size GetPreferredSizeCore(Size proposedConstraints) {
            if(FlatStyle != FlatStyle.System) {
                return base.GetPreferredSizeCore(proposedConstraints);
            }

            Size textSize = TextRenderer.MeasureText(this.Text, this.Font);
            Size size = SizeFromClientSize(textSize);
            size.Width += flatSystemStylePaddingWidth;
            size.Height = DpiHelper.IsScalingRequirementMet ? Math.Max(size.Height + 5, flatSystemStyleMinimumHeight) : size.Height + 5; // ensure minimum height to avoid truncation of RadioButton circle or text
            return size;                
        }

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
                        return Adapter.CommonLayout().Layout().checkBounds;
                    }
                }
            }
        }

        internal override Rectangle DownChangeRectangle {
            get {
                if (Appearance == Appearance.Button || FlatStyle == FlatStyle.System) {
                    return base.DownChangeRectangle;
                }
                else {
                    return Adapter.CommonLayout().Layout().checkBounds;
                }
            }
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.TabStop"]/*' />
        [DefaultValue(false)]
        new public bool TabStop {
            get {
                return base.TabStop;
            }
            set {
                base.TabStop = value;
            }
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.TextAlign"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the value indicating whether the user can give the focus to this
        ///       control using the TAB key.
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

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.CheckedChanged"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs when the
        ///       value of the <see cref='System.Windows.Forms.RadioButton.Checked'/>
        ///       property changes.
        ///    </para>
        /// </devdoc>
        [SRDescription(nameof(SR.RadioButtonOnCheckedChangedDescr))]
        public event EventHandler CheckedChanged {
            add {
                Events.AddHandler(EVENT_CHECKEDCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_CHECKEDCHANGED, value);
            }
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.CreateAccessibilityInstance"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Constructs the new instance of the accessibility object for this control. Subclasses
        ///       should not call base.CreateAccessibilityObject.
        ///    </para>
        /// </devdoc>
        protected override AccessibleObject CreateAccessibilityInstance() {
            return new RadioButtonAccessibleObject(this);
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.OnHandleCreated"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            //Since this is protected override, this can be called directly in a overriden class
            //and the handle doesn't need to be created.
            //So check for the handle to improve performance
            if (IsHandleCreated) {
                SendMessage(NativeMethods.BM_SETCHECK, isChecked? 1: 0, 0);
            }
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.OnCheckedChanged"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.CheckBox.CheckedChanged'/>
        ///       event.
        ///    </para>
        /// </devdoc>
        protected virtual void OnCheckedChanged(EventArgs e) {
            AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
            AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
            EventHandler handler = (EventHandler)Events[EVENT_CHECKEDCHANGED];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.OnClick"]/*' />
        /// <devdoc>
        ///     We override this to implement the autoCheck functionality.
        /// </devdoc>
        /// <internalonly/>
        protected override void OnClick(EventArgs e) {
            if (autoCheck) {
                Checked = true;
            }
            base.OnClick(e);
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.OnEnter"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        protected override void OnEnter(EventArgs e) {
            // Just like the Win32 RadioButton, fire a click if the
            // user arrows onto the control..
            //
            if (MouseButtons == MouseButtons.None) {
                if (UnsafeNativeMethods.GetKeyState((int)Keys.Tab) >= 0) {
                    //We enter the radioButton by using arrow keys
                    //Paint in raised state...
                    //
                    ResetFlagsandPaint();
                    if(!ValidationCancelled){ 
                        OnClick(e);
                    }
                }
                else {
                    //we enter the radioButton by pressing Tab
                    PerformAutoUpdates(true);
                    //reset the TabStop so we can come back later
                    //notice that PerformAutoUpdates will set the 
                    //TabStop of this button to false
                    TabStop = true;
                }
            }
            base.OnEnter(e);
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.PerformAutoUpdates"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void PerformAutoUpdates(bool tabbedInto) {
            if (autoCheck) {
                if (firstfocus) {
                    WipeTabStops(tabbedInto);
                }
                TabStop = isChecked;
                if (isChecked) {
                    Control parent = ParentInternal;
                    if (parent != null) {
                        Control.ControlCollection children = parent.Controls;
                        for (int i = 0; i < children.Count; i++) {
                            Control ctl = children[i];
                            if (ctl != this && ctl is RadioButton) {
                                RadioButton button = (RadioButton)ctl;
                                if (button.autoCheck && button.Checked) {
                                    PropertyDescriptor propDesc = TypeDescriptor.GetProperties(this)["Checked"];
                                    propDesc.SetValue(button, false);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.WipeTabStops"]/*' />
        /// <devdoc>
        ///     Removes tabstops from all radio buttons, other than the one that currently has the focus.
        /// </devdoc>
        /// <internalonly/>
        private void WipeTabStops(bool tabbedInto) {
		    Control parent = ParentInternal;
            if (parent != null) {
                Control.ControlCollection children = parent.Controls;
                for (int i = 0; i < children.Count; i++) {                  
                    Control ctl = children[i];
                    if (ctl is RadioButton) {
                        RadioButton button = (RadioButton) ctl;
                        if (!tabbedInto) {
                            button.firstfocus = false;
                        }
                        if (button.autoCheck) {
                            button.TabStop = false;
                        }
                    }
                }
            }
        }

        internal override ButtonBaseAdapter CreateFlatAdapter() {
            return new RadioButtonFlatAdapter(this);
        }

        internal override ButtonBaseAdapter CreatePopupAdapter() {
            return new RadioButtonPopupAdapter(this);
        }
            
        internal override ButtonBaseAdapter CreateStandardAdapter() {
            return new RadioButtonStandardAdapter(this);
        }

        private void OnAppearanceChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_APPEARANCECHANGED] as EventHandler;
            if (eh != null) {
                eh(this, e);
            }
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.OnMouseUp"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.ButtonBase.OnMouseUp'/> event.
        ///       
        ///    </para>
        /// </devdoc>
        protected override void OnMouseUp(MouseEventArgs mevent) {
            if (mevent.Button == MouseButtons.Left && GetStyle(ControlStyles.UserPaint)) {
                if (base.MouseIsDown) {
                    Point pt = PointToScreen(new Point(mevent.X, mevent.Y));
                    if (UnsafeNativeMethods.WindowFromPoint(pt.X, pt.Y) == Handle) {
                        //Paint in raised state...
                        //
                        ResetFlagsandPaint();
                        if (!ValidationCancelled) {
                            OnClick(mevent);
                            OnMouseClick(mevent);
                        }
                        
                    }
                }
            }
            base.OnMouseUp(mevent);
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.PerformClick"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Generates a <see cref='System.Windows.Forms.Control.Click'/> event for the
        ///       button, simulating a click by a user.
        ///    </para>
        /// </devdoc>
        public void PerformClick() {
            if (CanSelect) {
                //Paint in raised state...
                //
                ResetFlagsandPaint();
                if (!ValidationCancelled) {
                    OnClick(EventArgs.Empty);
                }
            }
                
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.ProcessMnemonic"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>        
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected internal override bool ProcessMnemonic(char charCode) {
            if (UseMnemonic && IsMnemonic(charCode, Text) && CanSelect) {
                if (!Focused) {
                    FocusInternal();    // This will cause an OnEnter event, which in turn will fire the click event
                }
                else {
                    PerformClick();     // Generate a click if already focused
                }
                return true;
            }
            return false;
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.ToString"]/*' />
        /// <devdoc>
        ///     Returns a string representation for this control.
        /// </devdoc>
        /// <internalonly/>
        public override string ToString() {

            string s = base.ToString();
            return s + ", Checked: " + Checked.ToString();
        }

        /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.RadioButtonAccessibleObject"]/*' />
        /// <internalonly/>        
        /// <devdoc>
        /// </devdoc>
        [System.Runtime.InteropServices.ComVisible(true)]        
        public class RadioButtonAccessibleObject : ButtonBaseAccessibleObject {

            /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.RadioButtonAccessibleObject.RadioButtonAccessibleObject"]/*' />
            public RadioButtonAccessibleObject(RadioButton owner) : base(owner) {
            }

            /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.RadioButtonAccessibleObject.DefaultAction"]/*' />
            public override string DefaultAction {
                get {
                    string defaultAction = Owner.AccessibleDefaultActionDescription;
                    if (defaultAction != null) {
                        return defaultAction;
                    }

                    return SR.AccessibleActionCheck;
                }
            }

            /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.RadioButtonAccessibleObject.Role"]/*' />
            public override AccessibleRole Role {
                get {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default) {
                        return role;
                    }
                    return AccessibleRole.RadioButton;
                }
            }

            /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.RadioButtonAccessibleObject.State"]/*' />
            public override AccessibleStates State {
                get {
                    if (((RadioButton)Owner).Checked) {
                        return AccessibleStates.Checked | base.State;
                    }
                    return base.State;
                }
            }

            /// <include file='doc\RadioButton.uex' path='docs/doc[@for="RadioButton.RadioButtonAccessibleObject.DoDefaultAction"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void DoDefaultAction() {
                ((RadioButton)Owner).PerformClick();
            }
        }

    }
}

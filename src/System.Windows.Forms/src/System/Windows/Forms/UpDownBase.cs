// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Runtime.Serialization.Formatters;
    using System.Threading;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Security.Permissions;
    using System.Windows.Forms.VisualStyles;
    using System.Drawing;
    using Microsoft.Win32;

    /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase"]/*' />
    /// <devdoc>
    ///    <para>Implements the basic
    ///       functionality required by an up-down control.</para>
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    Designer("System.Windows.Forms.Design.UpDownBaseDesigner, " + AssemblyRef.SystemDesign),
    System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors") // Shipped in Everett
    ]
    public abstract class UpDownBase : ContainerControl {

        private const int                       DefaultWheelScrollLinesPerPage = 1;
        private const int                       DefaultButtonsWidth = 16;
        private const int                       DefaultControlWidth = 120;
        private const int                       ThemedBorderWidth = 1; // width of custom border we draw when themed
        private const BorderStyle               DefaultBorderStyle = BorderStyle.Fixed3D;
        private static readonly bool            DefaultInterceptArrowKeys = true;
        private const LeftRightAlignment        DefaultUpDownAlign = LeftRightAlignment.Right;
        private const int                       DefaultTimerInterval = 500;

        ////////////////////////////////////////////////////////////////////////
        // Member variables
        //
        ////////////////////////////////////////////////////////////////////////

        // Child controls
        internal UpDownEdit upDownEdit; // See nested class at end of this file
        internal UpDownButtons upDownButtons; // See nested class at end of this file

        // Intercept arrow keys?
        private bool interceptArrowKeys = DefaultInterceptArrowKeys;

        // If true, the updown buttons will be drawn on the left-hand side of the control.
        private LeftRightAlignment upDownAlign = DefaultUpDownAlign;

        // userEdit is true when the text of the control has been changed,
        // and the internal value of the control has not yet been updated.
        // We do not always want to keep the internal value up-to-date,
        // hence this variable.
        private bool userEdit = false;

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.borderStyle"]/*' />
        /// <devdoc>
        ///     The current border for this edit control.
        /// </devdoc>
        private BorderStyle borderStyle = DefaultBorderStyle;

        // Mouse wheel movement
        private int wheelDelta = 0;

        // Indicates if the edit text is being changed
        private bool changingText = false;

        // Indicates whether we have doubleClicked
        private bool doubleClickFired = false;

        internal int defaultButtonsWidth = DefaultButtonsWidth;

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.UpDownBase"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.UpDownBase'/>
        ///       class.
        ///    </para>
        /// </devdoc>
        public UpDownBase() {
            if (DpiHelper.IsScalingRequired) {
                defaultButtonsWidth = LogicalToDeviceUnits(DefaultButtonsWidth);
            }

            upDownButtons = new UpDownButtons(this);
            upDownEdit = new UpDownEdit(this);
            upDownEdit.BorderStyle = BorderStyle.None;
            upDownEdit.AutoSize = false;
            upDownEdit.KeyDown += new KeyEventHandler(this.OnTextBoxKeyDown);
            upDownEdit.KeyPress += new KeyPressEventHandler(this.OnTextBoxKeyPress);
            upDownEdit.TextChanged += new EventHandler(this.OnTextBoxTextChanged);
            upDownEdit.LostFocus += new EventHandler(this.OnTextBoxLostFocus);
            upDownEdit.Resize += new EventHandler(this.OnTextBoxResize);
            upDownButtons.TabStop = false;
            upDownButtons.Size = new Size(defaultButtonsWidth, PreferredHeight);
            upDownButtons.UpDown += new UpDownEventHandler(this.OnUpDown);

            Controls.AddRange(new Control[] { upDownButtons, upDownEdit} );

            SetStyle(ControlStyles.Opaque | ControlStyles.FixedHeight | ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.StandardClick, false);
            SetStyle(ControlStyles.UseTextForAccessibility, false);
        }

        ////////////////////////////////////////////////////////////////////////
        // Properties
        //
        ////////////////////////////////////////////////////////////////////////

        // AutoScroll is not relevant to an UpDownBase
        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.AutoScroll"]/*' />
        /// <hideinheritance/>
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoScroll {
            get {
                return false;
            }
            set {
                // Don't allow AutoScroll to be set to anything
            }
        }

        // AutoScrollMargin is not relevant to an UpDownBase
        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.AutoScrollMargin"]/*' />
        /// <internalonly/>
        /// <hideinheritance/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        new public Size AutoScrollMargin {
            get {
                return base.AutoScrollMargin;
            }
            set {
                base.AutoScrollMargin = value;
            }
        }

        // AutoScrollMinSize is not relevant to an UpDownBase
        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.AutoScrollMinSize"]/*' />
        /// <internalonly/>
        /// <hideinheritance/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        new public Size AutoScrollMinSize {
            get {
                return base.AutoScrollMinSize;
            }
            set {
                base.AutoScrollMinSize = value;
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.AutoSize"]/*' />
        /// <devdoc>
        ///    <para> Override to re-expose AutoSize.</para>
        /// </devdoc>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.AutoSizeChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        new public event EventHandler AutoSizeChanged
        {
            add
            {
                base.AutoSizeChanged += value;
            }
            remove
            {
                base.AutoSizeChanged -= value;
            }
        }
        
        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.BackColor"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the background color for the
        ///       text box portion of the up-down control.
        ///    </para>
        /// </devdoc>
        public override Color BackColor {
            get {
                return upDownEdit.BackColor;
            }
            set {
                base.BackColor = value; // Don't remove this or you will break serialization.
                upDownEdit.BackColor = value;
                Invalidate();
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.BackgroundImage"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image BackgroundImage {
            get {
                return base.BackgroundImage;
            }
            set {
                base.BackgroundImage = value;
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.BackgroundImageChanged"]/*' />
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

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.BackgroundImageLayout"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout {
            get {
                return base.BackgroundImageLayout;
            }
            set {
                base.BackgroundImageLayout = value;
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.BackgroundImageLayoutChanged"]/*' />
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

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.BorderStyle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the border style for
        ///       the up-down control.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(BorderStyle.Fixed3D),
        DispId(NativeMethods.ActiveX.DISPID_BORDERSTYLE),
        SRDescription(nameof(SR.UpDownBaseBorderStyleDescr))
        ]
        public BorderStyle BorderStyle {
            get {
                return borderStyle;
            }

            set {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(BorderStyle));
                }

                if (borderStyle != value) {
                    borderStyle = value;
                    RecreateHandle();
                }
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.ChangingText"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the text
        ///       property is being changed internally by its parent class.
        ///    </para>
        /// </devdoc>
        protected bool ChangingText {
            get {
                return changingText;
            }

            set {
                changingText = value;
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.ContextMenu"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        public override ContextMenu ContextMenu {
            get {
                return base.ContextMenu;
            }
            set {
                base.ContextMenu = value;
                this.upDownEdit.ContextMenu = value;
            }
        }

        public override ContextMenuStrip ContextMenuStrip {
            get {
                return base.ContextMenuStrip;
            }
            set {
                base.ContextMenuStrip = value;
                this.upDownEdit.ContextMenuStrip = value;
            }
        }


        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.CreateParams"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Returns the parameters needed to create the handle. Inheriting classes
        ///       can override this to provide extra functionality. They should not,
        ///       however, forget to call base.getCreateParams() first to get the struct
        ///       filled up with the basic info.
        ///    </para>
        /// </devdoc>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;

                cp.Style &= (~NativeMethods.WS_BORDER);
                if (!Application.RenderWithVisualStyles) {
                    switch (borderStyle) {
                        case BorderStyle.Fixed3D:
                            cp.ExStyle |= NativeMethods.WS_EX_CLIENTEDGE;
                            break;
                        case BorderStyle.FixedSingle:
                            cp.Style |= NativeMethods.WS_BORDER;
                            break;
                    }
                }
                return cp;
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.DefaultSize"]/*' />
        /// <devdoc>
        ///     Deriving classes can override this to configure a default size for their control.
        ///     This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Size DefaultSize {
            get {
                return new Size(DefaultControlWidth, PreferredHeight);
            }
        }

        // DockPadding is not relevant to UpDownBase
        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.DockPadding"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        new public DockPaddingEdges DockPadding {
            get {
                return base.DockPadding;
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.Focused"]/*' />
        /// <devdoc>
        ///     Returns true if this control has focus.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlFocusedDescr))
        ]
        public override bool Focused {
            get {
                return upDownEdit.Focused;
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.ForeColor"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Indicates the foreground color for the control.
        ///    </para>
        /// </devdoc>
        public override Color ForeColor {
            get {
                return upDownEdit.ForeColor;
            }
            set {
                base.ForeColor = value;
                upDownEdit.ForeColor = value;
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.InterceptArrowKeys"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether
        ///       the user can use the UP
        ///       ARROW and DOWN ARROW keys to select values.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.UpDownBaseInterceptArrowKeysDescr))
        ]
        public bool InterceptArrowKeys {

            get {
                return interceptArrowKeys;
            }

            set {
                interceptArrowKeys = value;
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.MaximumSize"]/*' />
        public override Size MaximumSize {
            get { return base.MaximumSize; }
            set {
                base.MaximumSize = new Size(value.Width, 0);
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.MinimumSize"]/*' />
        public override Size MinimumSize {
            get { return base.MinimumSize; }
            set {
                base.MinimumSize = new Size(value.Width, 0);
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.MouseEnter"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler MouseEnter {
            add {
                base.MouseEnter += value;
            }
            remove {
                base.MouseEnter -= value;
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.MouseLeave"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler MouseLeave {
            add {
                base.MouseLeave += value;
            }
            remove {
                base.MouseLeave -= value;
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.MouseHover"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler MouseHover {
            add {
                base.MouseHover += value;
            }
            remove {
                base.MouseHover -= value;
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.MouseMove"]/*' />
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

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.PreferredHeight"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the height of
        ///       the up-down control.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.UpDownBasePreferredHeightDescr))
        ]
        public int PreferredHeight {
            get {

                int height = FontHeight;

                // Adjust for the border style
                if (borderStyle != BorderStyle.None) {
                    height += SystemInformation.BorderSize.Height * 4 + 3;
                }
                else {
                    height += 3;
                }

                return height;
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.ReadOnly"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       a
        ///       value
        ///       indicating whether the text may only be changed by the
        ///       use
        ///       of the up or down buttons.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.UpDownBaseReadOnlyDescr))
        ]
        public bool ReadOnly {

            get {
                return upDownEdit.ReadOnly;
            }

            set {
                upDownEdit.ReadOnly = value;
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.Text"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the text
        ///       displayed in the up-down control.
        ///    </para>
        /// </devdoc>
        [
        Localizable(true)
        ]
        public override string Text {
            get {
                return upDownEdit.Text;
            }

            set {
                upDownEdit.Text = value;
                // The text changed event will at this point be triggered.
                // After returning, the value of UserEdit will reflect
                // whether or not the current upDownEditbox text is in sync
                // with any internally stored values. If UserEdit is true,
                // we must validate the text the user typed or set.

                ChangingText = false;
                // Details: Usually, the code in the Text changed event handler
                // sets ChangingText back to false.
                // If the text hasn't actually changed though, the event handler
                // never fires. ChangingText should always be false on exit from
                // this property.

                if (UserEdit) {
                    ValidateEditText();
                }
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.TextAlign"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or
        ///       sets the alignment of the text in the up-down
        ///       control.
        ///    </para>
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(HorizontalAlignment.Left),
        SRDescription(nameof(SR.UpDownBaseTextAlignDescr))
        ]
        public HorizontalAlignment TextAlign {
            get {
                return upDownEdit.TextAlign;
            }
            set {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)HorizontalAlignment.Left, (int)HorizontalAlignment.Center))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(HorizontalAlignment));
                }
                upDownEdit.TextAlign = value;
            }
        }

        internal TextBox TextBox {
            get {
                return upDownEdit;
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.UpDownAlign"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       or sets the
        ///       alignment
        ///       of the up and down buttons on the up-down control.
        ///    </para>
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(LeftRightAlignment.Right),
        SRDescription(nameof(SR.UpDownBaseAlignmentDescr))
        ]
        public LeftRightAlignment UpDownAlign {

            get {
                return upDownAlign;
            }

            set {
                //valid values are 0x0 to 0x1
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)LeftRightAlignment.Left, (int)LeftRightAlignment.Right))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(LeftRightAlignment));
                }

                if (upDownAlign != value) {

                    upDownAlign = value;
                    PositionControls();
                    Invalidate();
                }
            }
        }

        internal UpDownButtons UpDownButtonsInternal {
            get {
                return upDownButtons;
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.UserEdit"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       or sets a value indicating whether a value has been entered by the
        ///       user.
        ///    </para>
        /// </devdoc>
        protected bool UserEdit {
            get {
                return userEdit;
            }

            set {
                userEdit = value;
            }
        }


        ////////////////////////////////////////////////////////////////////////
        // Methods
        //
        ////////////////////////////////////////////////////////////////////////
       

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.DownButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When overridden in a derived class, handles the pressing of the down button
        ///       on the up-down control.
        ///    </para>
        /// </devdoc>
        public abstract void DownButton();

        // GetPreferredSize and SetBoundsCore call this method to allow controls to self impose
        // constraints on their size.
        internal override Rectangle ApplyBoundsConstraints(int suggestedX, int suggestedY, int proposedWidth, int proposedHeight) {
            return base.ApplyBoundsConstraints(suggestedX,suggestedY, proposedWidth, PreferredHeight);
        }

        /// <summary>
        /// Gets an accessible name.
        /// </summary>
        /// <param name="baseName">The base name.</param>
        /// <returns>The accessible name.</returns>
        internal string GetAccessibleName(string baseName) {
            if (baseName == null) {
                if (AccessibilityImprovements.Level3) {
                    return SR.SpinnerAccessibleName;
                }
                else if (AccessibilityImprovements.Level1) {
                    return this.GetType().Name;
                }
            }

            return baseName;
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.RescaleConstantsForDpi"]/*' />
        /// <devdoc>
        ///       When overridden in a derived class, handles rescaling of any magic numbers used in control painting.
        ///       For UpDown controls, scale the width of the up/down buttons.
        ///       Must call the base class method to get the current DPI values. This method is invoked only when 
        ///       Application opts-in into the Per-monitor V2 support, targets .NETFX 4.7 and has 
        ///       EnableDpiChangedMessageHandling config switch turned on.
        /// </devdoc>
        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew) {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            defaultButtonsWidth = LogicalToDeviceUnits(DefaultButtonsWidth);
            upDownButtons.Width = defaultButtonsWidth;
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.OnChanged"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>When overridden in a derived class, raises the Changed event.
        /// event.</para>
        /// </devdoc>
        protected virtual void OnChanged(object source, EventArgs e) {
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.OnHandleCreated"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Initialize the updown. Adds the upDownEdit and updown buttons.
        ///    </para>
        /// </devdoc>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            PositionControls();
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(this.UserPreferenceChanged);
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.OnHandleCreated"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Tear down the updown.
        ///    </para>
        /// </devdoc>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(this.UserPreferenceChanged);
            base.OnHandleDestroyed(e);
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.OnPaint"]/*' />
        /// <devdoc>
        ///     Handles painting the buttons on the control.
        ///
        /// </devdoc>
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            
            Rectangle editBounds = upDownEdit.Bounds;
            if (Application.RenderWithVisualStyles) {
                if (borderStyle != BorderStyle.None) {
                    Rectangle bounds = ClientRectangle;
                    Rectangle clipBounds = e.ClipRectangle;

                    //Draw a themed textbox-like border, which is what the spin control does
                    VisualStyleRenderer vsr = new VisualStyleRenderer(VisualStyleElement.TextBox.TextEdit.Normal);
                    int border = ThemedBorderWidth;
                    Rectangle clipLeft = new Rectangle(bounds.Left, bounds.Top, border, bounds.Height);
                    Rectangle clipTop = new Rectangle(bounds.Left, bounds.Top, bounds.Width, border);
                    Rectangle clipRight = new Rectangle(bounds.Right - border, bounds.Top, border, bounds.Height);
                    Rectangle clipBottom = new Rectangle(bounds.Left, bounds.Bottom - border, bounds.Width, border);
                    clipLeft.Intersect(clipBounds);
                    clipTop.Intersect(clipBounds);
                    clipRight.Intersect(clipBounds);
                    clipBottom.Intersect(clipBounds);
                    vsr.DrawBackground(e.Graphics, bounds, clipLeft, HandleInternal);
                    vsr.DrawBackground(e.Graphics, bounds, clipTop, HandleInternal);
                    vsr.DrawBackground(e.Graphics, bounds, clipRight, HandleInternal);
                    vsr.DrawBackground(e.Graphics, bounds, clipBottom, HandleInternal);
                    // Draw rectangle around edit control with background color
                    using (Pen pen = new Pen(BackColor)) {
                        Rectangle backRect = editBounds;
                        backRect.X--;
                        backRect.Y--;
                        backRect.Width++;
                        backRect.Height++;
                        e.Graphics.DrawRectangle(pen, backRect);
                    }
                }
            }
            else {
                // Draw rectangle around edit control with background color
                using (Pen pen = new Pen(BackColor, Enabled ? 2 : 1))
                {
                    Rectangle backRect = editBounds;
                    backRect.Inflate(1, 1);
                    if (!Enabled)
                    {
                        backRect.X--;
                        backRect.Y--;
                        backRect.Width++;
                        backRect.Height++;
                    }
                    e.Graphics.DrawRectangle(pen, backRect);
                }
            }
            if (!Enabled && BorderStyle != BorderStyle.None && !upDownEdit.ShouldSerializeBackColor()) {
                //draws a grayed rectangled around the upDownEdit, since otherwise we will have a white
                //border around the upDownEdit, which is inconsistent with Windows' behavior
                //we only want to do this when BackColor is not serialized, since otherwise
                //we should display the backcolor instead of the usual grayed textbox.
                editBounds.Inflate(1, 1);
                ControlPaint.DrawBorder(e.Graphics, editBounds, SystemColors.Control, ButtonBorderStyle.Solid);
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.OnTextBoxKeyDown"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.KeyDown'/>
        /// event.</para>
        /// </devdoc>
        protected virtual void OnTextBoxKeyDown(object source, KeyEventArgs e) {
            this.OnKeyDown(e);
            if (interceptArrowKeys) {

                // Intercept up arrow
                if (e.KeyData == Keys.Up) {
                    UpButton();
                    e.Handled = true;
                }

                // Intercept down arrow
                else if (e.KeyData == Keys.Down) {
                    DownButton();
                    e.Handled = true;
                }
            }

            // Perform text validation if ENTER is pressed
            //
            if (e.KeyCode == Keys.Return && UserEdit) {
                ValidateEditText();
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.OnTextBoxKeyPress"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.KeyPress'/>
        /// event.</para>
        /// </devdoc>
        protected virtual void OnTextBoxKeyPress(object source, KeyPressEventArgs e) {
            this.OnKeyPress(e);

        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.OnTextBoxLostFocus"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.LostFocus'/> event.</para>
        /// </devdoc>
        protected virtual void OnTextBoxLostFocus(object source, EventArgs e) {
            if (UserEdit) {
                ValidateEditText();
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.OnTextBoxResize"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.Resize'/> event.</para>
        /// </devdoc>
        protected virtual void OnTextBoxResize(object source, EventArgs e) {
            this.Height = PreferredHeight;
            PositionControls();
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.OnTextBoxTextChanged"]/*' />
        /// <devdoc>
        /// <para>Raises the TextBoxTextChanged event.
        /// event.</para>
        /// </devdoc>
        protected virtual void OnTextBoxTextChanged(object source, EventArgs e) {
            if (changingText) {
                Debug.Assert(UserEdit == false, "OnTextBoxTextChanged() - UserEdit == true");
                ChangingText = false;
            }
            else {
                UserEdit = true;
            }

            this.OnTextChanged(e);
            OnChanged(source, new EventArgs());
        }

        /// <devdoc>
        ///     Called from the UpDownButtons member. Provided for derived controls to have a finer way to handle the event.
        /// </devdoc>
        internal virtual void OnStartTimer() {
        }

        internal virtual void OnStopTimer() {
        }

        /// <devdoc>
        ///       Raises the <see cref='System.Windows.Forms.Control.OnMouseDown'/> event.
        /// </devdoc>
        protected override void OnMouseDown(MouseEventArgs e) {
            if (e.Clicks == 2 && e.Button == MouseButtons.Left) {
                doubleClickFired = true;
            }

            base.OnMouseDown(e);
        }

        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.Control.OnMouseUp'/> event.
        ///
        ///    </para>
        /// </devdoc>
        protected override void OnMouseUp(MouseEventArgs mevent) {
            if (mevent.Button == MouseButtons.Left) {
                Point pt = PointToScreen(new Point(mevent.X, mevent.Y));
                if (UnsafeNativeMethods.WindowFromPoint(pt.X, pt.Y) == Handle && !ValidationCancelled) {
                    if (!doubleClickFired) {
                        OnClick(mevent);
                        OnMouseClick(mevent);
                    }
                    else {
                        doubleClickFired = false;
                        OnDoubleClick(mevent);
                        OnMouseDoubleClick(mevent);
                    }
                }
                doubleClickFired = false;
            }
            base.OnMouseUp(mevent);
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.OnMouseWheel"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.OnMouseWheel'/> event.</para>
        /// </devdoc>
        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);
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

            Debug.Assert(this.wheelDelta > -NativeMethods.WHEEL_DELTA, "wheelDelta is too smal");
            Debug.Assert(this.wheelDelta < NativeMethods.WHEEL_DELTA, "wheelDelta is too big");
            this.wheelDelta += e.Delta;

            float partialNotches;
            partialNotches = (float)this.wheelDelta / (float)NativeMethods.WHEEL_DELTA;

            if (wheelScrollLines == -1) {
               wheelScrollLines = DefaultWheelScrollLinesPerPage;
            }

            // Evaluate number of bands to scroll
            int scrollBands = (int)((float)wheelScrollLines * partialNotches);
            if (scrollBands != 0) {
               int absScrollBands;
               if (scrollBands > 0) {
                  absScrollBands = scrollBands;
                  while (absScrollBands > 0) {
                     UpButton();
                     absScrollBands--;
                  }
                  this.wheelDelta -= (int)((float)scrollBands * ((float)NativeMethods.WHEEL_DELTA / (float)wheelScrollLines));
               }
               else {
                  absScrollBands = -scrollBands;
                  while (absScrollBands > 0) {
                     DownButton();
                     absScrollBands--;
                  }
                  this.wheelDelta -= (int)((float)scrollBands * ((float)NativeMethods.WHEEL_DELTA / (float)wheelScrollLines));
               }
            }
        }


        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.OnLayout"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Handle the layout event. The size of the upDownEdit control, and the
        ///    position of the UpDown control must be modified.
        /// </devdoc>
        protected override void OnLayout(LayoutEventArgs e) {

            PositionControls();
            base.OnLayout(e);
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.OnFontChanged"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Raises the FontChanged event.
        ///    </para>
        /// </devdoc>
        protected override void OnFontChanged(EventArgs e) {
            // Clear the font height cache
            FontHeight = -1;

            Height = PreferredHeight;
            PositionControls();

            base.OnFontChanged(e);
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.OnUpDown"]/*' />
        /// <devdoc>
        ///
        ///     Handles UpDown events, which are generated by clicking on
        ///     the updown buttons in the child updown control.
        ///
        /// </devdoc>
        private void OnUpDown(object source, UpDownEventArgs e) {
            // Modify the value
            if (e.ButtonID == (int)ButtonID.Up)
                UpButton();
            else if (e.ButtonID == (int)ButtonID.Down)
                DownButton();
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.PositionControls"]/*' />
        /// <devdoc>
        ///     Calculates the size and position of the upDownEdit control and
        ///     the updown buttons.
        /// </devdoc>
        private void PositionControls() {
            Rectangle upDownEditBounds    = Rectangle.Empty,
                      upDownButtonsBounds = Rectangle.Empty;

            Rectangle clientArea     = new Rectangle(Point.Empty, ClientSize);
            int totalClientWidth     = clientArea.Width;
            bool themed              = Application.RenderWithVisualStyles;
            BorderStyle borderStyle  = BorderStyle;


            // determine how much to squish in - Fixed3d and FixedSingle have 2PX border
            int borderWidth = (borderStyle == BorderStyle.None) ? 0 : 2;
            clientArea.Inflate(-borderWidth, -borderWidth);

            // Reposition and resize the upDownEdit control
            //
            if (upDownEdit != null) {
                upDownEditBounds = clientArea;
                upDownEditBounds.Size = new Size(clientArea.Width - defaultButtonsWidth, clientArea.Height);
            }

            // Reposition and resize the updown buttons
            //
            if (upDownButtons != null) {
                int borderFixup = (themed) ? 1: 2;
                if (borderStyle == BorderStyle.None) {
                    borderFixup = 0;
                }
                upDownButtonsBounds = new Rectangle(/*x*/clientArea.Right - defaultButtonsWidth+borderFixup,
                                                    /*y*/clientArea.Top-borderFixup,
                                                    /*w*/defaultButtonsWidth,
                                                    /*h*/clientArea.Height+(borderFixup*2));
            }

            // Right to left translation
            LeftRightAlignment updownAlign = UpDownAlign;
            updownAlign = RtlTranslateLeftRight(updownAlign);

            // left/right updown align translation
            if (updownAlign == LeftRightAlignment.Left) {
                // if the buttons are aligned to the left, swap position of text box/buttons
                upDownButtonsBounds.X = totalClientWidth - upDownButtonsBounds.Right;
                upDownEditBounds.X = totalClientWidth - upDownEditBounds.Right;
            }

            // apply locations
            if (upDownEdit != null) {
                 upDownEdit.Bounds = upDownEditBounds;
            }
            if (upDownButtons != null) {
                upDownButtons.Bounds = upDownButtonsBounds;
                upDownButtons.Invalidate();
            }

       }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.Select"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Selects a range of
        ///       text in the up-down control.
        ///    </para>
        /// </devdoc>
        public void Select(int start, int length) {
            upDownEdit.Select(start, length);
        }


        /// <devdoc>
        ///   Child controls run their
        /// </devdoc>
        private MouseEventArgs TranslateMouseEvent(Control child, MouseEventArgs e) {
            if (child != null && IsHandleCreated) {
                // same control as PointToClient or PointToScreen, just
                // with two specific controls in mind.
                NativeMethods.POINT point = new NativeMethods.POINT(e.X, e.Y);
                UnsafeNativeMethods.MapWindowPoints(new HandleRef(child, child.Handle), new HandleRef(this, Handle), point, 1);
                return new MouseEventArgs(e.Button, e.Clicks, point.x, point.y , e.Delta);
            }
            return e;
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.UpButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When overridden in a derived class, handles the pressing of the up button on the up-down control.
        ///    </para>
        /// </devdoc>
        public abstract void UpButton();

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.UpdateEditText"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When overridden
        ///       in a derived class, updates the text displayed in the up-down control.
        ///    </para>
        /// </devdoc>
        protected abstract void UpdateEditText();
        
        private void UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs pref) {
            if (pref.Category == UserPreferenceCategory.Locale) {
                UpdateEditText();
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.ValidateEditText"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When overridden in a
        ///       derived class, validates the text displayed in the up-down control.
        ///    </para>
        /// </devdoc>
        protected virtual void ValidateEditText() {
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.WndProc"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_SETFOCUS:
                    if (!HostedInWin32DialogManager) {
                        if (ActiveControl == null) {
                            SetActiveControlInternal(TextBox);
                        }
                        else {
                            FocusActiveControlInternal();
                        }
                    }
                    else {
                        if (TextBox.CanFocus){
                            UnsafeNativeMethods.SetFocus(new HandleRef(TextBox, TextBox.Handle));
                        }
                        base.WndProc(ref m);
                    }
                    break;
                case NativeMethods.WM_KILLFOCUS:
                    DefWndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.SetToolTip"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    This Function sets the ToolTip for this composite control.
        /// </devdoc>
        internal void SetToolTip(ToolTip toolTip, string caption) {
            toolTip.SetToolTip(this.upDownEdit , caption);
            toolTip.SetToolTip(this.upDownButtons , caption);
        }

        internal class UpDownEdit : TextBox{
            /////////////////////////////////////////////////////////////////////
            // Member variables
            //
            /////////////////////////////////////////////////////////////////////

            // Parent control
            private UpDownBase parent;
            private bool doubleClickFired = false;
            /////////////////////////////////////////////////////////////////////
            // Constructors
            //
            /////////////////////////////////////////////////////////////////////

            internal UpDownEdit(UpDownBase parent)
            : base() {

                SetStyle(ControlStyles.FixedHeight |
                         ControlStyles.FixedWidth, true);

                SetStyle(ControlStyles.Selectable, false);

                this.parent = parent;
            }

            public override string Text {
                get {
                    return base.Text;
                }
                set {
                    bool valueChanged = (value != base.Text);
                    base.Text = value;      
                    if (valueChanged && AccessibilityImprovements.Level1) {
                            AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
                    }
                }
            }

            protected override AccessibleObject CreateAccessibilityInstance() {
                return new UpDownEditAccessibleObject(this, parent);
            }

            protected override void OnMouseDown(MouseEventArgs e) {
                if (e.Clicks == 2 && e.Button == MouseButtons.Left) {
                    doubleClickFired = true;
                }
                parent.OnMouseDown(parent.TranslateMouseEvent(this, e));
            }

            /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.UpDownEdit.OnMouseUp"]/*' />
            /// <devdoc>
            ///
            ///     Handles detecting when the mouse button is released.
            ///
            /// </devdoc>
            protected override void OnMouseUp(MouseEventArgs e) {

                Point pt = new Point(e.X,e.Y);
                pt = PointToScreen(pt);

                MouseEventArgs me = parent.TranslateMouseEvent(this, e);
                if (e.Button == MouseButtons.Left) {
                    if (!parent.ValidationCancelled && UnsafeNativeMethods.WindowFromPoint(pt.X, pt.Y) == Handle) {
                        if (!doubleClickFired) {
                            parent.OnClick(me);
                            parent.OnMouseClick(me);
                        }
                        else {
                            doubleClickFired = false;
                            parent.OnDoubleClick(me);
                            parent.OnMouseDoubleClick(me);
                        }
                    }
                    doubleClickFired = false;
                }

                parent.OnMouseUp(me);
            }

            internal override void WmContextMenu(ref Message m) {
                // Want to make the SourceControl to be the UpDownBase, not the Edit.
                if (ContextMenu == null && ContextMenuStrip != null) {
                    WmContextMenu(ref m, parent);
                }
                else {
                    WmContextMenu(ref m, this);
                }
            }


            /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.UpDownEdit.OnKeyUp"]/*' />
            /// <devdoc>
            /// <para>Raises the <see cref='System.Windows.Forms.Control.KeyUp'/>
            /// event.</para>
            /// </devdoc>
            protected override void OnKeyUp(KeyEventArgs e) {
                parent.OnKeyUp(e);
            }

            protected override void OnGotFocus(EventArgs e) {
                parent.SetActiveControlInternal(this);
                parent.InvokeGotFocus(parent, e);
            }

            protected override void OnLostFocus(EventArgs e) {
                parent.InvokeLostFocus(parent, e);
            }

            // Microsoft: Focus fixes. The XXXUpDown control will
            //         also fire a Leave event. We don't want
            //         to fire two of them.
            // protected override void OnLeave(EventArgs e) {
            //     parent.OnLeave(e);
            // }

            // Create our own accessibility object to map the accessible name
            // back to our parent.  They should track.
            internal class UpDownEditAccessibleObject : ControlAccessibleObject {
                UpDownBase parent;

                public UpDownEditAccessibleObject(UpDownEdit owner, UpDownBase parent) : base(owner) {
                    this.parent = parent;
                }

                public override string Name {
                    get {
                        return parent.AccessibilityObject.Name;
                    }
                    set {
                        parent.AccessibilityObject.Name = value;
                    }
                }

                public override string KeyboardShortcut {
                    get {
                        return parent.AccessibilityObject.KeyboardShortcut;
                    }
                }
            }
        }

        /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.UpDownButtons"]/*' />
        /// <devdoc>
        ///
        ///     Nested class UpDownButtons
        ///
        ///     A control representing the pair of buttons on the end of the upDownEdit control.
        ///     This class handles drawing the updown buttons, and detecting mouse actions
        ///     on these buttons. Acceleration on the buttons is handled. The control
        ///     sends UpDownEventArgss to the parent UpDownBase class when a button is pressed,
        ///     or when the acceleration determines that another event should be generated.
        /// </devdoc>
        internal class UpDownButtons : Control {
            // 

            /////////////////////////////////////////////////////////////////////
            // Member variables
            //
            /////////////////////////////////////////////////////////////////////

            // Parent control
            private UpDownBase parent;

            // Button state
            private ButtonID pushed = ButtonID.None;
            private ButtonID captured = ButtonID.None;
            private ButtonID mouseOver = ButtonID.None;

            // UpDown event handler
            private UpDownEventHandler upDownEventHandler;

            // Timer
            private Timer timer;                    // generates UpDown events
            private int timerInterval;              // milliseconds between events

            private bool doubleClickFired = false;

            /////////////////////////////////////////////////////////////////////
            // Constructors
            //
            /////////////////////////////////////////////////////////////////////

            internal UpDownButtons(UpDownBase parent)

            : base() {

                SetStyle(ControlStyles.Opaque | ControlStyles.FixedHeight |
                         ControlStyles.FixedWidth, true);

                SetStyle(ControlStyles.Selectable, false);

                this.parent = parent;
            }


            /////////////////////////////////////////////////////////////////////
            // Methods
            //
            /////////////////////////////////////////////////////////////////////

            /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.UpDownButtons.UpDown"]/*' />
            /// <devdoc>
            ///
            ///     Adds a handler for the updown button event.
            /// </devdoc>
            public event UpDownEventHandler UpDown {
                add {
                    upDownEventHandler += value;
                }
                remove {
                    upDownEventHandler -= value;
                }
            }

            // Called when the mouse button is pressed - we need to start
            // spinning the value of the updown.
            //
            private void BeginButtonPress(MouseEventArgs e) {

                int half_height = Size.Height / 2;

                if (e.Y < half_height) {

                    // Up button
                    //
                    pushed = captured = ButtonID.Up;
                    Invalidate();

                }
                else {

                    // Down button
                    //
                    pushed = captured = ButtonID.Down;
                    Invalidate();
                }

                // Capture the mouse
                //
                CaptureInternal = true;

                // Generate UpDown event
                //
                OnUpDown(new UpDownEventArgs((int)pushed));

                // Start the timer for new updown events
                //
                StartTimer();
            }

            protected override AccessibleObject CreateAccessibilityInstance() {
                return new UpDownButtonsAccessibleObject(this);
            }

            // Called when the mouse button is released - we need to stop
            // spinning the value of the updown.
            //
            private void EndButtonPress() {

                pushed = ButtonID.None;
                captured = ButtonID.None;

                // Stop the timer
                StopTimer();

                // Release the mouse
                CaptureInternal = false;

                // Redraw the buttons
                Invalidate();
            }

            /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.UpDownButtons.OnMouseDown"]/*' />
            /// <devdoc>
            ///
            ///     Handles detecting mouse hits on the buttons. This method
            ///     detects which button was hit (up or down), fires a
            ///     updown event, captures the mouse, and starts a timer
            ///     for repeated updown events.
            ///
            /// </devdoc>
            protected override void OnMouseDown(MouseEventArgs e) {
                // Begin spinning the value
                //

                // Focus the parent
                //
                this.parent.FocusInternal();

                if (!parent.ValidationCancelled && e.Button == MouseButtons.Left) {
                    BeginButtonPress(e);
                }
                if (e.Clicks == 2 && e.Button == MouseButtons.Left) {
                    doubleClickFired = true;
                }
                // At no stage should a button be pushed, and the mouse
                // not captured.
                //
                Debug.Assert(!(pushed != ButtonID.None && captured == ButtonID.None),
                             "Invalid button pushed/captured combination");

                parent.OnMouseDown(parent.TranslateMouseEvent(this, e));
            }

            /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.UpDownButtons.OnMouseMove"]/*' />
            /// <devdoc>
            ///
            ///     Handles detecting mouse movement.
            ///
            /// </devdoc>
            protected override void OnMouseMove(MouseEventArgs e) {

                // If the mouse is captured by the buttons (i.e. an updown button
                // was pushed, and the mouse button has not yet been released),
                // determine the new state of the buttons depending on where
                // the mouse pointer has moved.

                if (Capture) {

                    // Determine button area

                    Rectangle rect = ClientRectangle;
                    rect.Height /= 2;

                    if (captured == ButtonID.Down) {
                        rect.Y += rect.Height;
                    }

                    // Test if the mouse has moved outside the button area

                    if (rect.Contains(e.X, e.Y)) {

                        // Inside button
                        // Repush the button if necessary

                        if (pushed != captured) {

                            // Restart the timer
                            StartTimer();

                            pushed = captured;
                            Invalidate();
                        }

                    }
                    else {

                        // Outside button
                        // Retain the capture, but pop the button up whilst
                        // the mouse remains outside the button and the
                        // mouse button remains pressed.

                        if (pushed != ButtonID.None) {

                            // Stop the timer for updown events
                            StopTimer();

                            pushed = ButtonID.None;
                            Invalidate();
                        }
                    }
                }

                //Logic for seeing which button is Hot if any
                Rectangle rectUp = ClientRectangle, rectDown = ClientRectangle;
                rectUp.Height /= 2;
                rectDown.Y += rectDown.Height / 2;

                //Check if the mouse is on the upper or lower button. Note that it could be in neither.
                if (rectUp.Contains(e.X, e.Y)) {
                    mouseOver = ButtonID.Up;
                    Invalidate();
                }
                else if (rectDown.Contains(e.X, e.Y)) {
                    mouseOver = ButtonID.Down;
                    Invalidate();
                }

                // At no stage should a button be pushed, and the mouse
                // not captured.
                Debug.Assert(!(pushed != ButtonID.None && captured == ButtonID.None),
                             "Invalid button pushed/captured combination");

                parent.OnMouseMove(parent.TranslateMouseEvent(this, e));
            }

            /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.UpDownButtons.OnMouseUp"]/*' />
            /// <devdoc>
            ///
            ///     Handles detecting when the mouse button is released.
            ///
            /// </devdoc>
            protected override void OnMouseUp(MouseEventArgs e) {

                if (!parent.ValidationCancelled && e.Button == MouseButtons.Left) {
                    EndButtonPress();
                }

                // At no stage should a button be pushed, and the mouse
                // not captured.
                Debug.Assert(!(pushed != ButtonID.None && captured == ButtonID.None),
                             "Invalid button pushed/captured combination");

                Point pt = new Point(e.X,e.Y);
                pt = PointToScreen(pt);

                MouseEventArgs me = parent.TranslateMouseEvent(this, e);
                if (e.Button == MouseButtons.Left) {
                    if (!parent.ValidationCancelled && UnsafeNativeMethods.WindowFromPoint(pt.X, pt.Y) == Handle) {
                        if (!doubleClickFired) {
                            this.parent.OnClick(me);
                        }
                        else {
                            doubleClickFired = false;
                            this.parent.OnDoubleClick(me);
                            this.parent.OnMouseDoubleClick(me);
                        }
                    }
                    doubleClickFired = false;
                }

                parent.OnMouseUp(me);
            }

            /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.UpDownButtons.OnMouseLeave"]/*' />
            /// <devdoc>
            ///
            ///     Handles detecting when the mouse leaves.
            ///
            /// </devdoc>
            protected override void OnMouseLeave(EventArgs e) {
                mouseOver = ButtonID.None;
                Invalidate();

                parent.OnMouseLeave(e);
            }

            /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.UpDownButtons.OnPaint"]/*' />
            /// <devdoc>
            ///     Handles painting the buttons on the control.
            ///
            /// </devdoc>
            protected override void OnPaint(PaintEventArgs e) {
                int half_height = ClientSize.Height / 2;

                /* Draw the up and down buttons */

                if (Application.RenderWithVisualStyles) {
                    VisualStyleRenderer vsr = new VisualStyleRenderer(mouseOver == ButtonID.Up ? VisualStyleElement.Spin.Up.Hot : VisualStyleElement.Spin.Up.Normal);

                    if (!Enabled) {
                        vsr.SetParameters(VisualStyleElement.Spin.Up.Disabled);
                    }
                    else if (pushed == ButtonID.Up) {
                        vsr.SetParameters(VisualStyleElement.Spin.Up.Pressed);
                    }

                    vsr.DrawBackground(e.Graphics, new Rectangle(0, 0, parent.defaultButtonsWidth, half_height), HandleInternal);

                    if (!Enabled) {
                        vsr.SetParameters(VisualStyleElement.Spin.Down.Disabled);
                    }
                    else if (pushed == ButtonID.Down) {
                        vsr.SetParameters(VisualStyleElement.Spin.Down.Pressed);
                    }
                    else {
                        vsr.SetParameters(mouseOver == ButtonID.Down ? VisualStyleElement.Spin.Down.Hot : VisualStyleElement.Spin.Down.Normal);
                    }

                    vsr.DrawBackground(e.Graphics, new Rectangle(0, half_height, parent.defaultButtonsWidth, half_height), HandleInternal);
                }
                else {
                    ControlPaint.DrawScrollButton(e.Graphics,
                                                  new Rectangle(0, 0, parent.defaultButtonsWidth, half_height),
                                                  ScrollButton.Up,
                                                  pushed == ButtonID.Up ? ButtonState.Pushed : (Enabled ? ButtonState.Normal : ButtonState.Inactive));

                    ControlPaint.DrawScrollButton(e.Graphics,
                                                  new Rectangle(0, half_height, parent.defaultButtonsWidth, half_height),
                                                  ScrollButton.Down,
                                                  pushed == ButtonID.Down ? ButtonState.Pushed : (Enabled ? ButtonState.Normal : ButtonState.Inactive));
                }

                if (half_height != (ClientSize.Height + 1) / 2) {
                    // When control has odd height, a line needs to be drawn below the buttons with the backcolor.
                    using (Pen pen = new Pen(this.parent.BackColor)) {
                        Rectangle clientRect = ClientRectangle;
                        e.Graphics.DrawLine(pen, clientRect.Left, clientRect.Bottom - 1, clientRect.Right - 1, clientRect.Bottom - 1);
                    }
                }

                base.OnPaint(e); // raise paint event, just in case this inner class goes public some day
            }

            /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.UpDownButtons.OnUpDown"]/*' />
            /// <devdoc>
            ///     Occurs when the UpDown buttons are pressed and when the acceleration timer tick event is raised.
            /// </devdoc>
            protected virtual void OnUpDown(UpDownEventArgs upevent) {
                if (upDownEventHandler != null)
                    upDownEventHandler(this, upevent);
            }

            /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.UpDownButtons.StartTimer"]/*' />
            /// <devdoc>
            ///     Starts the timer for generating updown events
            /// </devdoc>
            protected void StartTimer() {
                parent.OnStartTimer();
                if (timer == null) {
                    timer = new Timer();      // generates UpDown events
                    // Add the timer handler
                    timer.Tick += new EventHandler(TimerHandler);
                }

                this.timerInterval = DefaultTimerInterval;

                timer.Interval = this.timerInterval;
                timer.Start();
            }

            /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.UpDownButtons.StopTimer"]/*' />
            /// <devdoc>
            ///     Stops the timer for generating updown events
            /// </devdoc>
            protected void StopTimer() {
                if (timer != null) {
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                }
                parent.OnStopTimer();
            }

            /// <include file='doc\UpDownBase.uex' path='docs/doc[@for="UpDownBase.UpDownButtons.TimerHandler"]/*' />
            /// <devdoc>
            ///     Generates updown events when the timer calls this function.
            /// </devdoc>
            private void TimerHandler(object source, EventArgs args) {

                // Make sure we've got mouse capture
                if (!Capture) {
                    EndButtonPress();
                    return;
                }

                // onUpDown method calls customer's ValueCHanged event handler which might enter the message loop and 
                // process the mouse button up event, which results in timer being disposed 
                OnUpDown(new UpDownEventArgs((int)pushed));

                if (timer != null) {
                    // Accelerate timer.
                    this.timerInterval *= 7;
                    this.timerInterval /= 10;

                    if (this.timerInterval < 1) {
                        this.timerInterval = 1;
                    }

                    timer.Interval = this.timerInterval;
                } 
            }

            internal class UpDownButtonsAccessibleObject : ControlAccessibleObject {

                private DirectionButtonAccessibleObject upButton;
                private DirectionButtonAccessibleObject downButton;

                public UpDownButtonsAccessibleObject(UpDownButtons owner) : base(owner) {
                }

                public override string Name {
                    get {
                        string baseName = base.Name;
                        if (baseName == null || baseName.Length == 0) {
                            if (AccessibilityImprovements.Level3) {
                                // For AI.Level3 spinner is already announced so use type name.
                                return Owner.ParentInternal.GetType().Name;
                            }
                            return SR.SpinnerAccessibleName;
                        }
                        return baseName;
                    }
                    set {
                        base.Name = value;
                    }
                }

                /// <include file='doc\DomainUpDown.uex' path='docs/doc[@for="DomainUpDown.DomainUpDownAccessibleObject.Role"]/*' />
                public override AccessibleRole Role {
                    get {
                        AccessibleRole role = Owner.AccessibleRole;
                        if (role != AccessibleRole.Default) {
                            return role;
                        }
                        return AccessibleRole.SpinButton;
                    }
                }

                private DirectionButtonAccessibleObject UpButton {
                    get {
                        if (upButton == null) {
                            upButton = new DirectionButtonAccessibleObject(this, true);
                        }
                        return upButton;
                    }
                }

                private DirectionButtonAccessibleObject DownButton {
                    get {
                        if (downButton == null) {
                            downButton = new DirectionButtonAccessibleObject(this, false);
                        }
                        return downButton;
                    }
                }



                /// <include file='doc\DomainUpDown.uex' path='docs/doc[@for="DomainUpDown.DomainUpDownAccessibleObject.GetChild"]/*' />
                /// <devdoc>
                /// </devdoc>
                public override AccessibleObject GetChild(int index) {

                    // Up button
                    //
                    if (index == 0) {
                        return UpButton;
                    }

                    // Down button
                    //
                    if (index == 1) {
                        return DownButton;
                    }

                    return null;
                }

                /// <include file='doc\DomainUpDown.uex' path='docs/doc[@for="DomainUpDown.DomainUpDownAccessibleObject.GetChildCount"]/*' />
                /// <devdoc>
                /// </devdoc>
                public override int GetChildCount() {
                    return 2;
                }

                internal class DirectionButtonAccessibleObject : AccessibleObject {
                    private bool up;
                    private UpDownButtonsAccessibleObject parent;

                    public DirectionButtonAccessibleObject(UpDownButtonsAccessibleObject parent, bool up) {
                        this.parent = parent;
                        this.up = up;
                    }

                    public override Rectangle Bounds {
                        get {
                            // Get button bounds
                            //
                            Rectangle bounds = ((UpDownButtons)parent.Owner).Bounds;
                            bounds.Height /= 2;
                            if (!up) {
                                bounds.Y += bounds.Height;
                            }

                            // Convert to screen co-ords
                            //
                            return (((UpDownButtons)parent.Owner).ParentInternal).RectangleToScreen(bounds);
                        }
                    }

                    public override string Name {
                        get {
                            if (up) {
                                return SR.UpDownBaseUpButtonAccName;
                            }
                            return SR.UpDownBaseDownButtonAccName;
                        }
                        set {
                        }
                    }

                    public override AccessibleObject Parent {
                        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                        get {
                            return parent;
                        }
                    }

                    public override AccessibleRole Role {
                        get {
                            return AccessibleRole.PushButton;
                        }
                    }
                }
            }

        } // end class UpDownButtons

        // Button identifiers

        internal enum ButtonID {
            None = 0,
            Up = 1,
            Down = 2,
        }
    } // end class UpDownBase
}


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
    using System.ComponentModel.Design;
    using System.ComponentModel;
    using System.Windows.Forms.Layout;

    using System.Drawing;
    using System.Windows.Forms.Internal;
    using Microsoft.Win32;

    /// <include file='doc\Button.uex' path='docs/doc[@for="Button"]/*' />
    /// <devdoc>
    ///    <para>Represents a
    ///       Windows button.</para>
    /// </devdoc>
    [ComVisible(true),
     ClassInterface(ClassInterfaceType.AutoDispatch),
     SRDescription(nameof(SR.DescriptionButton)),
     Designer("System.Windows.Forms.Design.ButtonBaseDesigner, " + AssemblyRef.SystemDesign)
    ]
    public class Button : ButtonBase, IButtonControl {

        /// <include file='doc\Button.uex' path='docs/doc[@for="Button.dialogResult"]/*' />
        /// <devdoc>
        ///     The dialog result that will be sent to the parent dialog form when
        ///     we are clicked.
        /// </devdoc>
        private DialogResult dialogResult;

        private const int InvalidDimensionValue = Int32.MinValue;

        /// <include file='doc\Button.uex' path='docs/doc[@for="Button.dialogResult"]/*' />
        /// <devdoc>
        ///     For buttons whose FaltStyle = FlatStyle.Flat, this property specifies the size, in pixels  
        ///     of the border around the button.
        /// </devdoc>
        private Size systemSize = new Size(InvalidDimensionValue, InvalidDimensionValue);

        /// <include file='doc\Button.uex' path='docs/doc[@for="Button.Button"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.Button'/>
        ///       class.
        ///    </para>
        /// </devdoc>
        public Button() : base() {
            // Buttons shouldn't respond to right clicks, so we need to do all our own click logic
            SetStyle(ControlStyles.StandardClick |
                     ControlStyles.StandardDoubleClick,
                     false);
        }

        /// <devdoc>
        ///     Allows the control to optionally shrink when AutoSize is true.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(true),
        DefaultValue(AutoSizeMode.GrowOnly),
        Localizable(true),
        SRDescription(nameof(SR.ControlAutoSizeModeDescr))
        ]
        public AutoSizeMode AutoSizeMode {
            get {
                return GetAutoSizeMode();
            }
            set {

                if (!ClientUtils.IsEnumValid(value, (int)value, (int)AutoSizeMode.GrowAndShrink, (int)AutoSizeMode.GrowOnly)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AutoSizeMode));
                }
                
                if (GetAutoSizeMode() != value) {                    
                    SetAutoSizeMode(value);
                    if(ParentInternal != null) {
                        // DefaultLayout does not keep anchor information until it needs to.  When
                        // AutoSize became a common property, we could no longer blindly call into
                        // DefaultLayout, so now we do a special InitLayout just for DefaultLayout.
                        if(ParentInternal.LayoutEngine == DefaultLayout.Instance) {
                            ParentInternal.LayoutEngine.InitLayout(this, BoundsSpecified.Size);
                        }
                        LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.AutoSize);
                    }
                }
            }
        }

        internal override ButtonBaseAdapter CreateFlatAdapter() {
            return new ButtonFlatAdapter(this);
        }

        internal override ButtonBaseAdapter CreatePopupAdapter() {
            return new ButtonPopupAdapter(this);
        }
            
        internal override ButtonBaseAdapter CreateStandardAdapter() {
            return new ButtonStandardAdapter(this);
        }

        internal override Size GetPreferredSizeCore(Size proposedConstraints) {
            if(FlatStyle != FlatStyle.System) {
                Size prefSize = base.GetPreferredSizeCore(proposedConstraints);
                return AutoSizeMode == AutoSizeMode.GrowAndShrink ? prefSize : LayoutUtils.UnionSizes(prefSize, Size);                
            }

            if (systemSize.Width == InvalidDimensionValue) {
                Size requiredSize;
                // Note: The result from the BCM_GETIDEALSIZE message isn't accurate if the font has been
                // changed, because this method is called before the font is set into the device context.

                requiredSize = TextRenderer.MeasureText(this.Text, this.Font);
                requiredSize = SizeFromClientSize(requiredSize);

                // This padding makes FlatStyle.System about the same size as FlatStyle.Standard
                // with an 8px font.
                requiredSize.Width += 14;
                requiredSize.Height += 9;
                systemSize = requiredSize;
            }
            Size paddedSize = systemSize + Padding.Size;            
            return AutoSizeMode == AutoSizeMode.GrowAndShrink ? paddedSize : LayoutUtils.UnionSizes(paddedSize, Size);
        }

        /// <include file='doc\Button.uex' path='docs/doc[@for="Button.CreateParams"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       This is called when creating a window. Inheriting classes can overide
        ///       this to add extra functionality, but should not forget to first call
        ///       base.CreateParams() to make sure the control continues to work
        ///       correctly.
        ///
        ///    </para>
        /// </devdoc>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;
                cp.ClassName = "BUTTON";                
                if (GetStyle(ControlStyles.UserPaint)) {
                    cp.Style |= NativeMethods.BS_OWNERDRAW;
                }
                else {
                    cp.Style |= NativeMethods.BS_PUSHBUTTON;
                    if (IsDefault) {
                        cp.Style |= NativeMethods.BS_DEFPUSHBUTTON;
                    }
                }
                return cp;
            }
        }

        /// <include file='doc\Button.uex' path='docs/doc[@for="Button.DialogResult"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value that is returned to the
        ///       parent form when the button
        ///       is clicked.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(DialogResult.None),
        SRDescription(nameof(SR.ButtonDialogResultDescr))
        ]
        public virtual DialogResult DialogResult {
            get {
                return dialogResult;
            }

            set {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DialogResult.None, (int) DialogResult.No)) {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DialogResult));
                }
                dialogResult = value;
            }
        }

        /// <include file='doc\Button.uex' path='docs/doc[@for="Button.OnMouseEnter"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.Control.OnMouseEnter'/> event.
        ///    </para>
        /// </devdoc>
        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
        }

        /// <include file='doc\Button.uex' path='docs/doc[@for="Button.OnMouseLeave"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.Control.OnMouseLeave'/> event.
        ///    </para>
        /// </devdoc>
        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
        }

        /// <include file='doc\Button.uex' path='docs/doc[@for="Button.DoubleClick"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event EventHandler DoubleClick {
            add {
                base.DoubleClick += value;
            }
            remove {
                base.DoubleClick -= value;
            }
        }

        /// <include file='doc\Button.uex' path='docs/doc[@for="Button.MouseDoubleClick"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event MouseEventHandler MouseDoubleClick {
            add {
                base.MouseDoubleClick += value;
            }
            remove {
                base.MouseDoubleClick -= value;
            }
        }

        /// <include file='doc\Button.uex' path='docs/doc[@for="Button.NotifyDefault"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Notifies the <see cref='System.Windows.Forms.Button'/>
        ///       whether it is the default button so that it can adjust its appearance
        ///       accordingly.
        ///       
        ///    </para>
        /// </devdoc>
        public virtual void NotifyDefault(bool value) {
            if (IsDefault != value) {
                IsDefault = value;                
            }
        }

        /// <include file='doc\Button.uex' path='docs/doc[@for="Button.OnClick"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       This method actually raises the Click event. Inheriting classes should
        ///       override this if they wish to be notified of a Click event. (This is far
        ///       preferable to actually adding an event handler.) They should not,
        ///       however, forget to call base.onClick(e); before exiting, to ensure that
        ///       other recipients do actually get the event.
        ///
        ///    </para>
        /// </devdoc>
        protected override void OnClick(EventArgs e) {
            Form form = FindFormInternal();
            if (form != null) form.DialogResult = dialogResult;


            // accessibility stuff
            //
            AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
            AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);

            base.OnClick(e);
        }

        protected override void OnFontChanged(EventArgs e) {
            systemSize = new Size(InvalidDimensionValue, InvalidDimensionValue);
            base.OnFontChanged(e);
        }

        /// <include file='doc\Button.uex' path='docs/doc[@for="Button.OnMouseUp"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.ButtonBase.OnMouseUp'/> event.
        ///       
        ///    </para>
        /// </devdoc>
        protected override void OnMouseUp(MouseEventArgs mevent) {
            if (mevent.Button == MouseButtons.Left && MouseIsPressed) {
                bool isMouseDown = base.MouseIsDown;

                if (GetStyle(ControlStyles.UserPaint)) {
                    //Paint in raised state...
                    ResetFlagsandPaint();
                }
                if (isMouseDown) {
                    Point pt = PointToScreen(new Point(mevent.X, mevent.Y));
                    if (UnsafeNativeMethods.WindowFromPoint(pt.X, pt.Y) == Handle && !ValidationCancelled) {
                        if (GetStyle(ControlStyles.UserPaint)) {
                            OnClick(mevent);
                        }
                        OnMouseClick(mevent);
                    }
                }
            }
            base.OnMouseUp(mevent);
        }

        protected override void OnTextChanged(EventArgs e) {
            systemSize = new Size(InvalidDimensionValue, InvalidDimensionValue);
            base.OnTextChanged(e);
        }

        /// <summary>
        /// When overridden in a derived class, handles rescaling of any magic numbers used in control painting.
        /// Must call the base class method to get the current DPI values. This method is invoked only when 
        /// Application opts-in into the Per-monitor V2 support, targets .NETFX 4.7 and has 
        /// EnableDpiChangedMessageHandling and EnableDpiChangedHighDpiImprovements config switches turned on.
        /// </summary>
        /// <param name="deviceDpiOld">Old DPI value</param>
        /// <param name="deviceDpiNew">New DPI value</param>
        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew) {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);

            if (DpiHelper.IsScalingRequirementMet) {
                // reset cached boundary size - it needs to be recalculated for new DPI
                systemSize = new Size(InvalidDimensionValue, InvalidDimensionValue);
            }
        }

        /// <include file='doc\Button.uex' path='docs/doc[@for="Button.PerformClick"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Generates a <see cref='System.Windows.Forms.Control.Click'/> event for a
        ///       button.
        ///    </para>
        /// </devdoc>
        public void PerformClick() {
            if (CanSelect) {
                bool validatedControlAllowsFocusChange;
                bool validate = ValidateActiveControl(out validatedControlAllowsFocusChange);
                if (!ValidationCancelled && (validate || validatedControlAllowsFocusChange))
                {
                    //Paint in raised state...
                    //
                    ResetFlagsandPaint();
                    OnClick(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\Button.uex' path='docs/doc[@for="Button.ProcessMnemonic"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Lets a control process mnmemonic characters. Inheriting classes can
        ///       override this to add extra functionality, but should not forget to call
        ///       base.ProcessMnemonic(charCode); to ensure basic functionality
        ///       remains unchanged.
        ///
        ///    </para>
        /// </devdoc>
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected internal override bool ProcessMnemonic(char charCode) {
            if (UseMnemonic && CanProcessMnemonic() && IsMnemonic(charCode, Text)) {
                PerformClick();
                return true;
            }
            return base.ProcessMnemonic(charCode);
        }

        /// <include file='doc\Button.uex' path='docs/doc[@for="Button.ToString"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Provides some interesting information for the Button control in
        ///       String form.
        ///    </para>
        /// </devdoc>
        public override string ToString() {

            string s = base.ToString();
            return s + ", Text: " + Text;
        }

        /// <include file='doc\Button.uex' path='docs/doc[@for="Button.WndProc"]/*' />
        /// <devdoc>
        ///     The button's window procedure.  Inheriting classes can override this
        ///     to add extra functionality, but should not forget to call
        ///     base.wndProc(m); to ensure the button continues to function properly.
        /// </devdoc>
        /// <internalonly/>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_REFLECT + NativeMethods.WM_COMMAND:
                    if (NativeMethods.Util.HIWORD(m.WParam) == NativeMethods.BN_CLICKED) {
                        Debug.Assert(!GetStyle(ControlStyles.UserPaint), "Shouldn't get BN_CLICKED when UserPaint");
                        if (!ValidationCancelled) {
                            OnClick(EventArgs.Empty);
                        }                        
                    }
                    break;
                case NativeMethods.WM_ERASEBKGND:
                    DefWndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}



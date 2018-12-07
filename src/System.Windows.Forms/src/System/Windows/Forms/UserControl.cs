// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using Microsoft.Win32;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Drawing;    
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Security.Permissions;
    using System.Windows.Forms.Design;
    using System.Windows.Forms.Layout;

    /// <include file='doc\UserControl.uex' path='docs/doc[@for="UserControl"]/*' />
    /// <devdoc>
    ///     Represents an empty control that can be used in the Forms Designer to create other  controls.   By extending form, UserControl inherits all of
    ///     the standard positioning and mnemonic handling code that is necessary
    ///     in a user control.
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    Designer("System.Windows.Forms.Design.UserControlDocumentDesigner, " + AssemblyRef.SystemDesign, typeof(IRootDesigner)),
    Designer("System.Windows.Forms.Design.ControlDesigner, " + AssemblyRef.SystemDesign),
    DesignerCategory("UserControl"),
    DefaultEvent(nameof(Load))
    ]
    public class UserControl : ContainerControl {
        private static readonly object EVENT_LOAD = new object();
        private BorderStyle borderStyle = System.Windows.Forms.BorderStyle.None;

        /// <include file='doc\UserControl.uex' path='docs/doc[@for="UserControl.UserControl"]/*' />
        /// <devdoc>
        ///    Creates a new UserControl object. A vast majority of people
        ///    will not want to instantiate this class directly, but will be a
        ///    sub-class of it.
        /// </devdoc>
        public UserControl() {
            SetScrollState(ScrollStateAutoScrolling, false);
            SetState(STATE_VISIBLE, true);
            SetState(STATE_TOPLEVEL, false);
	    SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }
        
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

        /// <devdoc>
        ///    <para> Re-expose AutoSizeChanged.</para>
        /// </devdoc>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public new event EventHandler AutoSizeChanged {
            add {
                base.AutoSizeChanged += value;
            }
            remove {
                base.AutoSizeChanged -= value;
            }
        }

        /// <devdoc>
        ///     Allows the control to optionally shrink when AutoSize is true.
        /// </devdoc>
        [
        SRDescription(nameof(SR.ControlAutoSizeModeDescr)),
        SRCategory(nameof(SR.CatLayout)),
        Browsable(true),
        DefaultValue(AutoSizeMode.GrowOnly),
        Localizable(true)
        ]
        public AutoSizeMode AutoSizeMode {
            get {
                return GetAutoSizeMode();
            }
            set {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)AutoSizeMode.GrowAndShrink, (int)AutoSizeMode.GrowOnly))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AutoSizeMode));
                }
                
                if (GetAutoSizeMode() != value) {                    
                    SetAutoSizeMode(value);
                    Control toLayout = DesignMode || ParentInternal == null ? this : ParentInternal;
                    
                    if(toLayout != null) {
                        // DefaultLayout does not keep anchor information until it needs to.  When
                        // AutoSize became a common property, we could no longer blindly call into
                        // DefaultLayout, so now we do a special InitLayout just for DefaultLayout.
                        if(toLayout.LayoutEngine == DefaultLayout.Instance) {
                            toLayout.LayoutEngine.InitLayout(this, BoundsSpecified.Size);
                        }
                        LayoutTransaction.DoLayout(toLayout, this, PropertyNames.AutoSize);
                    }
                }
            }
        }

        /// <include file='doc\UserControl.uex' path='docs/doc[@for="UserControl.AutoValidate"]/*' />
        /// <devdoc>
        ///     Indicates whether controls in this container will be automatically validated when the focus changes.
        /// </devdoc>
        [
        Browsable(true),
        EditorBrowsable(EditorBrowsableState.Always),
        ]
        public override AutoValidate AutoValidate {
            get {
                return base.AutoValidate;
            }
            set {
                base.AutoValidate = value;
            }
        }

        /// <include file='doc\UserControl.uex' path='docs/doc[@for="UserControl.AutoValidateChanged"]/*' />
        [
        Browsable(true),
        EditorBrowsable(EditorBrowsableState.Always),
        ]
        public new event EventHandler AutoValidateChanged {
            add {
                base.AutoValidateChanged += value;
            }
            remove {
                base.AutoValidateChanged -= value;
            }
        }

		/// <include file='doc\UserControl.uex' path='docs/doc[@for="UserControl.BorderStyle"]/*' />
        /// <devdoc>
        ///    <para> 
        ///       Indicates the borderstyle for the UserControl.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(BorderStyle.None),
        SRDescription(nameof(SR.UserControlBorderStyleDescr)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public BorderStyle BorderStyle {
            get {
                return borderStyle;
            }

            set {
                if (borderStyle != value) {
                    //valid values are 0x0 to 0x2
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(BorderStyle));
                    }

                    borderStyle = value;
                    UpdateStyles();
                }
            }
        }

        /// <include file='doc\UserControl.uex' path='docs/doc[@for="UserControl.CreateParams"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Returns the parameters needed to create the handle.  Inheriting classes
        ///    can override this to provide extra functionality.  They should not,
        ///    however, forget to call base.getCreateParams() first to get the struct
        ///    filled up with the basic info.This is required as we now need to pass the 
        ///    styles for appropriate BorderStyle that is set by the user.
        /// </devdoc>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= NativeMethods.WS_EX_CONTROLPARENT;

                cp.ExStyle &= (~NativeMethods.WS_EX_CLIENTEDGE);
                cp.Style &= (~NativeMethods.WS_BORDER);

                switch (borderStyle) {
                    case BorderStyle.Fixed3D:
                        cp.ExStyle |= NativeMethods.WS_EX_CLIENTEDGE;
                        break;
                    case BorderStyle.FixedSingle:
                        cp.Style |= NativeMethods.WS_BORDER;
                        break;
                }
                return cp;
            }
        }
        
        /// <include file='doc\UserControl.uex' path='docs/doc[@for="UserControl.DefaultSize"]/*' />
        /// <devdoc>
        ///     The default size for this user control.
        /// </devdoc>
        protected override Size DefaultSize {
            get {
                return new Size(150, 150);
            }
        }

        /// <include file='doc\UserControl.uex' path='docs/doc[@for="UserControl.Load"]/*' />
        /// <devdoc>
        ///    <para>Occurs before the control becomes visible.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.UserControlOnLoadDescr))]
        public event EventHandler Load {
            add {
                Events.AddHandler(EVENT_LOAD, value);
            }
            remove {
                Events.RemoveHandler(EVENT_LOAD, value);
            }
        }

        /// <include file='doc\UserControl.uex' path='docs/doc[@for="UserControl.Text"]/*' />
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

        /// <include file='doc\UserControl.uex' path='docs/doc[@for="UserControl.TextChanged"]/*' />
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
        
        /// <include file='doc\UserControl.uex' path='docs/doc[@for="UserControl.ValidateChildren"]/*' />
        /// <devdoc>
        ///     Validates all selectable child controls in the container, including descendants. This is
        ///     equivalent to calling ValidateChildren(ValidationConstraints.Selectable). See <see cref='ValidationConstraints.Selectable'/>
        ///     for details of exactly which child controls will be validated.
        /// </devdoc>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public override bool ValidateChildren() {
            return base.ValidateChildren();
        }

        /// <include file='doc\UserControl.uex' path='docs/doc[@for="UserControl.ValidateChildren1"]/*' />
        /// <devdoc>
        ///     Validates all the child controls in the container. Exactly which controls are
        ///     validated and which controls are skipped is determined by <paramref name="flags"/>.
        /// </devdoc>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public override bool ValidateChildren(ValidationConstraints validationConstraints) {
            return base.ValidateChildren(validationConstraints);
        }

        private bool FocusInside() {
            if (!IsHandleCreated) return false;

            IntPtr hwndFocus = UnsafeNativeMethods.GetFocus();
            if (hwndFocus == IntPtr.Zero) return false;

            IntPtr hwnd = Handle;
            if (hwnd == hwndFocus || SafeNativeMethods.IsChild(new HandleRef(this, hwnd), new HandleRef(null, hwndFocus)))
                return true;

            return false;
        }

        /// <include file='doc\UserControl.uex' path='docs/doc[@for="UserControl.OnCreateControl"]/*' />
        /// <devdoc>
        ///    <para> Raises the CreateControl event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnCreateControl() {
            base.OnCreateControl();
            
            OnLoad(EventArgs.Empty);
        }

        /// <include file='doc\UserControl.uex' path='docs/doc[@for="UserControl.OnLoad"]/*' />
        /// <devdoc>
        ///    <para>The Load event is fired before the control becomes visible for the first time.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnLoad(EventArgs e) {
            // There is no good way to explain this event except to say
            // that it's just another name for OnControlCreated.
            EventHandler handler = (EventHandler)Events[EVENT_LOAD];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\UserControl.uex' path='docs/doc[@for="UserControl.OnResize"]/*' />
        /// <devdoc>
        ///     OnResize override to invalidate entire control in Stetch mode
        /// </devdoc>
        /// <internalonly/>
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            if (BackgroundImage != null) {
                Invalidate();
            }
        }

        /// <include file='doc\UserControl.uex' path='docs/doc[@for="UserControl.OnMouseDown"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnMouseDown(MouseEventArgs e) {
            if (!FocusInside())
                FocusInternal();
            base.OnMouseDown(e);
        }

        private void WmSetFocus(ref Message m) {
            if (!HostedInWin32DialogManager) {
                IntSecurity.ModifyFocus.Assert();
                try {
                    if (ActiveControl == null)
                        SelectNextControl(null, true, true, true, false);
                }
                finally {
                    System.Security.CodeAccessPermission.RevertAssert();
                }
            }
            if (!ValidationCancelled) {
                base.WndProc(ref m);
            }
            
        }

        /// <include file='doc\UserControl.uex' path='docs/doc[@for="UserControl.WndProc"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_SETFOCUS:
                    WmSetFocus(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}

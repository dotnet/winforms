// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;
using static Interop;

namespace System.Windows.Forms
{

    /// <summary>
    ///  Represents an empty control that can be used in the Forms Designer to create other  controls.   By extending form, UserControl inherits all of
    ///  the standard positioning and mnemonic handling code that is necessary
    ///  in a user control.
    /// </summary>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    Designer("System.Windows.Forms.Design.UserControlDocumentDesigner, " + AssemblyRef.SystemDesign, typeof(IRootDesigner)),
    Designer("System.Windows.Forms.Design.ControlDesigner, " + AssemblyRef.SystemDesign),
    DesignerCategory("UserControl"),
    DefaultEvent(nameof(Load))
    ]
    public class UserControl : ContainerControl
    {
        private static readonly object EVENT_LOAD = new object();
        private BorderStyle borderStyle = System.Windows.Forms.BorderStyle.None;

        /// <summary>
        ///  Creates a new UserControl object. A vast majority of people
        ///  will not want to instantiate this class directly, but will be a
        ///  sub-class of it.
        /// </summary>
        public UserControl()
        {
            SetScrollState(ScrollStateAutoScrolling, false);
            SetState(STATE_VISIBLE, true);
            SetState(STATE_TOPLEVEL, false);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        /// <summary>
        ///  Override to re-expose AutoSize.
        /// </summary>
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

        /// <summary>
        ///  Re-expose AutoSizeChanged.
        /// </summary>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public new event EventHandler AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        /// <summary>
        ///  Allows the control to optionally shrink when AutoSize is true.
        /// </summary>
        [
        SRDescription(nameof(SR.ControlAutoSizeModeDescr)),
        SRCategory(nameof(SR.CatLayout)),
        Browsable(true),
        DefaultValue(AutoSizeMode.GrowOnly),
        Localizable(true)
        ]
        public AutoSizeMode AutoSizeMode
        {
            get
            {
                return GetAutoSizeMode();
            }
            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)AutoSizeMode.GrowAndShrink, (int)AutoSizeMode.GrowOnly))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AutoSizeMode));
                }

                if (GetAutoSizeMode() != value)
                {
                    SetAutoSizeMode(value);
                    Control toLayout = DesignMode || ParentInternal == null ? this : ParentInternal;

                    if (toLayout != null)
                    {
                        // DefaultLayout does not keep anchor information until it needs to.  When
                        // AutoSize became a common property, we could no longer blindly call into
                        // DefaultLayout, so now we do a special InitLayout just for DefaultLayout.
                        if (toLayout.LayoutEngine == DefaultLayout.Instance)
                        {
                            toLayout.LayoutEngine.InitLayout(this, BoundsSpecified.Size);
                        }
                        LayoutTransaction.DoLayout(toLayout, this, PropertyNames.AutoSize);
                    }
                }
            }
        }

        /// <summary>
        ///  Indicates whether controls in this container will be automatically validated when the focus changes.
        /// </summary>
        [
        Browsable(true),
        EditorBrowsable(EditorBrowsableState.Always),
        ]
        public override AutoValidate AutoValidate
        {
            get
            {
                return base.AutoValidate;
            }
            set
            {
                base.AutoValidate = value;
            }
        }

        [
        Browsable(true),
        EditorBrowsable(EditorBrowsableState.Always),
        ]
        public new event EventHandler AutoValidateChanged
        {
            add => base.AutoValidateChanged += value;
            remove => base.AutoValidateChanged -= value;
        }

        /// <summary>
        ///
        ///  Indicates the borderstyle for the UserControl.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(BorderStyle.None),
        SRDescription(nameof(SR.UserControlBorderStyleDescr)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public BorderStyle BorderStyle
        {
            get
            {
                return borderStyle;
            }

            set
            {
                if (borderStyle != value)
                {
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

        /// <summary>
        ///  Returns the parameters needed to create the handle.  Inheriting classes
        ///  can override this to provide extra functionality.  They should not,
        ///  however, forget to call base.getCreateParams() first to get the struct
        ///  filled up with the basic info.This is required as we now need to pass the
        ///  styles for appropriate BorderStyle that is set by the user.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= NativeMethods.WS_EX_CONTROLPARENT;

                cp.ExStyle &= (~NativeMethods.WS_EX_CLIENTEDGE);
                cp.Style &= (~NativeMethods.WS_BORDER);

                switch (borderStyle)
                {
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

        /// <summary>
        ///  The default size for this user control.
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(150, 150);
            }
        }

        /// <summary>
        ///  Occurs before the control becomes visible.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.UserControlOnLoadDescr))]
        public event EventHandler Load
        {
            add => Events.AddHandler(EVENT_LOAD, value);
            remove => Events.RemoveHandler(EVENT_LOAD, value);
        }

        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        Bindable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged
        {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }

        /// <summary>
        ///  Validates all selectable child controls in the container, including descendants. This is
        ///  equivalent to calling ValidateChildren(ValidationConstraints.Selectable). See <see cref='ValidationConstraints.Selectable'/>
        ///  for details of exactly which child controls will be validated.
        /// </summary>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public override bool ValidateChildren()
        {
            return base.ValidateChildren();
        }

        /// <summary>
        ///  Validates all the child controls in the container. Exactly which controls are
        ///  validated and which controls are skipped is determined by <paramref name="flags"/>.
        /// </summary>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public override bool ValidateChildren(ValidationConstraints validationConstraints)
        {
            return base.ValidateChildren(validationConstraints);
        }

        private bool FocusInside()
        {
            if (!IsHandleCreated)
            {
                return false;
            }

            IntPtr hwndFocus = UnsafeNativeMethods.GetFocus();
            if (hwndFocus == IntPtr.Zero)
            {
                return false;
            }

            IntPtr hwnd = Handle;
            if (hwnd == hwndFocus || SafeNativeMethods.IsChild(new HandleRef(this, hwnd), new HandleRef(null, hwndFocus)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///  Raises the CreateControl event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            OnLoad(EventArgs.Empty);
        }

        /// <summary>
        ///  The Load event is fired before the control becomes visible for the first time.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnLoad(EventArgs e)
        {
            // There is no good way to explain this event except to say
            // that it's just another name for OnControlCreated.
            ((EventHandler)Events[EVENT_LOAD])?.Invoke(this, e);
        }

        /// <summary>
        ///  OnResize override to invalidate entire control in Stetch mode
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (BackgroundImage != null)
            {
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!FocusInside())
            {
                Focus();
            }

            base.OnMouseDown(e);
        }

        private void WmSetFocus(ref Message m)
        {
            if (!HostedInWin32DialogManager)
            {
                if (ActiveControl == null)
                {
                    SelectNextControl(null, true, true, true, false);
                }
            }
            if (!ValidationCancelled)
            {
                base.WndProc(ref m);
            }

        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WindowMessages.WM_SETFOCUS:
                    WmSetFocus(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}

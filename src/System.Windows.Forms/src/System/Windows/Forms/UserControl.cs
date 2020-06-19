// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

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
    [Designer("System.Windows.Forms.Design.UserControlDocumentDesigner, " + AssemblyRef.SystemDesign, typeof(IRootDesigner))]
    [Designer("System.Windows.Forms.Design.ControlDesigner, " + AssemblyRef.SystemDesign)]
    [DesignerCategory("UserControl")]
    [DefaultEvent(nameof(Load))]
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
            SetState(States.Visible, true);
            SetState(States.TopLevel, false);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        /// <summary>
        ///  Override to re-expose AutoSize.
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override bool AutoSize
        {
            get => base.AutoSize;
            set => base.AutoSize = value;
        }

        /// <summary>
        ///  Re-expose AutoSizeChanged.
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public new event EventHandler AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        /// <summary>
        ///  Allows the control to optionally shrink when AutoSize is true.
        /// </summary>
        [SRDescription(nameof(SR.ControlAutoSizeModeDescr))]
        [SRCategory(nameof(SR.CatLayout))]
        [Browsable(true)]
        [DefaultValue(AutoSizeMode.GrowOnly)]
        [Localizable(true)]
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
                    Control toLayout = DesignMode || ParentInternal is null ? this : ParentInternal;

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
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public override AutoValidate AutoValidate
        {
            get => base.AutoValidate;
            set => base.AutoValidate = value;
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public new event EventHandler AutoValidateChanged
        {
            add => base.AutoValidateChanged += value;
            remove => base.AutoValidateChanged -= value;
        }

        /// <summary>
        ///
        ///  Indicates the borderstyle for the UserControl.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(BorderStyle.None)]
        [SRDescription(nameof(SR.UserControlBorderStyleDescr))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
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
                cp.Style &= ~(int)User32.WS.BORDER;
                cp.ExStyle |= (int)User32.WS_EX.CONTROLPARENT;
                cp.ExStyle &= ~(int)User32.WS_EX.CLIENTEDGE;

                switch (borderStyle)
                {
                    case BorderStyle.Fixed3D:
                        cp.ExStyle |= (int)User32.WS_EX.CLIENTEDGE;
                        break;
                    case BorderStyle.FixedSingle:
                        cp.Style |= (int)User32.WS.BORDER;
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
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.UserControlOnLoadDescr))]
        public event EventHandler Load
        {
            add => Events.AddHandler(EVENT_LOAD, value);
            remove => Events.RemoveHandler(EVENT_LOAD, value);
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
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public override bool ValidateChildren()
        {
            return base.ValidateChildren();
        }

        /// <summary>
        ///  Validates all the child controls in the container. Exactly which controls are
        ///  validated and which controls are skipped is determined by <paramref name="validationConstraints"/>.
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
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

            IntPtr hwndFocus = User32.GetFocus();
            if (hwndFocus == IntPtr.Zero)
            {
                return false;
            }

            IntPtr hwnd = Handle;
            return hwnd == hwndFocus || User32.IsChild(new HandleRef(this, hwnd), hwndFocus).IsTrue();
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
                if (ActiveControl is null)
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
            switch ((User32.WM)m.Msg)
            {
                case User32.WM.SETFOCUS:
                    WmSetFocus(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents the container for multiple-document interface (MDI) child forms. 
    /// This class cannot be inherited.
    /// </summary>
    /// <remarks>
    ///  Don't create an <see cref="MdiClient"/> control.
    ///  A form creates and uses the <see cref="MdiClient"/> when you set the <see cref="Form.IsMdiContainer"/> property to <see langword="true"/>.  
    /// </remarks>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    ToolboxItem(false),
    DesignTimeVisible(false)
    ]
    public sealed class MdiClient : Control
    {
        // kept in add order, not ZOrder. Need to return the correct
        // array of items...
        //
        private readonly ArrayList children = new ArrayList();

        /// <summary>
        ///  Creates a new MdiClient.
        /// </summary>
        public MdiClient() : base()
        {
            SetStyle(ControlStyles.Selectable, false);
            BackColor = SystemColors.AppWorkspace;
            Dock = DockStyle.Fill;
        }

        /// <summary>
        ///  Gets or sets the background image displayed in the <see cref="MdiClient" /> control.
        /// </summary>
        /// <value>The image to display in the background of the control.</value>
        [
        Localizable(true)
        ]
        public override Image BackgroundImage
        {
            get
            {
                Image result = base.BackgroundImage;
                if (result == null && ParentInternal != null)
                {
                    result = ParentInternal.BackgroundImage;
                }

                return result;
            }

            set
            {
                base.BackgroundImage = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                Image backgroundImage = BackgroundImage;
                if (backgroundImage != null && ParentInternal != null)
                {
                    ImageLayout imageLayout = base.BackgroundImageLayout;
                    if (imageLayout != ParentInternal.BackgroundImageLayout)
                    {
                        // if the Layout is set on the parent use that.
                        return ParentInternal.BackgroundImageLayout;
                    }
                }
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        /// <summary>
        ///  Gets the required creation parameters when the control handle is created.
        /// </summary>
        /// <value>The required creation parameters when the control handle is created.</value>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                cp.ClassName = "MDICLIENT";

                // Note: Don't set the MDIS_ALLCHILDSTYLES CreatParams.Style bit, it prevents an MDI child form from getting activated
                // when made visible (no WM_MDIACTIVATE sent to it), and forcing activation on it changes the activation event sequence
                // (MdiChildActivate/Enter/Focus/Activate/etc.).
                // Comment for removed code:
                // Add the style MDIS_ALLCHILDSTYLES
                // so that MDI Client windows can have the WS_VISIBLE style removed from the window style
                // to make them not visible but still present.
                cp.Style |= NativeMethods.WS_VSCROLL | NativeMethods.WS_HSCROLL;
                cp.ExStyle |= NativeMethods.WS_EX_CLIENTEDGE;
                cp.Param = new NativeMethods.CLIENTCREATESTRUCT(IntPtr.Zero, 1);
                ISite site = ParentInternal?.Site;
                if (site != null && site.DesignMode)
                {
                    cp.Style |= NativeMethods.WS_DISABLED;
                    SetState(STATE_ENABLED, false);
                }

                if (RightToLeft == RightToLeft.Yes && ParentInternal != null && ParentInternal.IsMirrored)
                {
                    //We want to turn on mirroring for MdiClient explicitly.
                    cp.ExStyle |= NativeMethods.WS_EX_LAYOUTRTL | NativeMethods.WS_EX_NOINHERITLAYOUT;
                    //Don't need these styles when mirroring is turned on.
                    cp.ExStyle &= ~(NativeMethods.WS_EX_RTLREADING | NativeMethods.WS_EX_RIGHT | NativeMethods.WS_EX_LEFTSCROLLBAR);
                }

                return cp;
            }
        }

        /// <summary>
        ///  The list of MDI children contained. This list
        ///  will be sorted by the order in which the children were
        ///  added to the form, not the current ZOrder.
        /// </summary>
        public Form[] MdiChildren
        {
            get
            {
                Form[] temp = new Form[children.Count];
                children.CopyTo(temp, 0);
                return temp;
            }
        }

        protected override Control.ControlCollection CreateControlsInstance()
        {
            return new ControlCollection(this);
        }

        /// <summary>
        ///  Arranges the multiple-document interface (MDI) child forms within the MDI parent form.
        /// </summary>
        /// <param name="value">One of the enumeration values that defines the layout of MDI child forms.</param>
        public void LayoutMdi(MdiLayout value)
        {
            if (Handle == IntPtr.Zero)
            {
                return;
            }

            switch (value)
            {
                case MdiLayout.Cascade:
                    SendMessage(WindowMessages.WM_MDICASCADE, 0, 0);
                    break;
                case MdiLayout.TileVertical:
                    SendMessage(WindowMessages.WM_MDITILE, NativeMethods.MDITILE_VERTICAL, 0);
                    break;
                case MdiLayout.TileHorizontal:
                    SendMessage(WindowMessages.WM_MDITILE, NativeMethods.MDITILE_HORIZONTAL, 0);
                    break;
                case MdiLayout.ArrangeIcons:
                    SendMessage(WindowMessages.WM_MDIICONARRANGE, 0, 0);
                    break;
            }
        }

        /// <summary>
        ///  Raises the <see cref="Control.Resize" /> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnResize(EventArgs e)
        {
            ISite site = ParentInternal?.Site;
            if (site != null && site.DesignMode && Handle != IntPtr.Zero)
            {
                SetWindowRgn();
            }
            base.OnResize(e);
        }

        /// <summary>
        ///  Scales the entire control and any child controls.
        /// </summary>
        /// <param name="dx">The scaling factor for the x-axis</param>
        /// <param name="dy">The scaling factor for the y-axis.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void ScaleCore(float dx, float dy)
        {
            // Don't scale child forms...
            //

            SuspendLayout();
            try
            {
                Rectangle bounds = Bounds;
                int sx = (int)Math.Round(bounds.X * dx);
                int sy = (int)Math.Round(bounds.Y * dy);
                int sw = (int)Math.Round((bounds.X + bounds.Width) * dx - sx);
                int sh = (int)Math.Round((bounds.Y + bounds.Height) * dy - sy);
                SetBounds(sx, sy, sw, sh, BoundsSpecified.All);
            }
            finally
            {
                ResumeLayout();
            }
        }

        /// <summary>
        ///  Scales this form's location, size, padding, and margin. The <see cref="MdiClient" /> overrides 
        ///  <see cref="ScaleControl(SizeF,BoundsSpecified)" /> to enforce a minimum and maximum size.
        /// </summary>
        /// <param name="factor">The factor by which the height and width of the control will be scaled.</param>
        /// <param name="specified">The bounds of the control to use when defining its size and position.</param>
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            // never scale X and Y of an MDI client form
            specified &= ~BoundsSpecified.Location;
            base.ScaleControl(factor, specified);
        }

        /// <summary>
        ///  Sets the specified bounds of the control.
        /// </summary>
        /// <param name="x">The new <see cref="Control.Left" /> property value of the control.</param>
        /// <param name="y">The new <see cref="Control.Top" /> property value of the control.</param>
        /// <param name="width">The new <see cref="Control.Width" /> property value of the control.</param>
        /// <param name="height">The new <see cref="Control.Height" /> property value of the control.</param>
        /// <param name="specified">A bitwise combination of the enumeration values that specifies the bounds of the control to use.</param>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            ISite site = ParentInternal?.Site;
            if (IsHandleCreated && (site == null || !site.DesignMode))
            {
                Rectangle oldBounds = Bounds;
                base.SetBoundsCore(x, y, width, height, specified);
                Rectangle newBounds = Bounds;

                int yDelta = oldBounds.Height - newBounds.Height;
                if (yDelta != 0)
                {
                    // NOTE: This logic is to keep minimized MDI children anchored to
                    // the bottom left of the client area, normally they are anchored
                    // to the top right which just looks wierd!
                    //
                    NativeMethods.WINDOWPLACEMENT wp = new NativeMethods.WINDOWPLACEMENT
                    {
                        length = Marshal.SizeOf<NativeMethods.WINDOWPLACEMENT>()
                    };

                    for (int i = 0; i < Controls.Count; i++)
                    {
                        Control ctl = Controls[i];
                        if (ctl != null && ctl is Form)
                        {
                            Form child = (Form)ctl;
                            // Only adjust the window position for visible MDI Child windows to prevent
                            // them from being re-displayed.
                            if (child.CanRecreateHandle() && child.WindowState == FormWindowState.Minimized)
                            {
                                UnsafeNativeMethods.GetWindowPlacement(new HandleRef(child, child.Handle), ref wp);
                                wp.ptMinPosition.Y -= yDelta;
                                if (wp.ptMinPosition.Y == -1)
                                {
                                    if (yDelta < 0)
                                    {
                                        wp.ptMinPosition.Y = 0;
                                    }
                                    else
                                    {
                                        wp.ptMinPosition.Y = -2;
                                    }
                                }
                                wp.flags = NativeMethods.WPF_SETMINPOSITION;
                                UnsafeNativeMethods.SetWindowPlacement(new HandleRef(child, child.Handle), ref wp);
                                wp.flags = 0;
                            }
                        }
                    }
                }
            }
            else
            {
                base.SetBoundsCore(x, y, width, height, specified);
            }
        }

        /// <summary>
        ///  This code is required to set the correct window region during the resize of the Form at design time.
        ///  There is case when the form contains a MainMenu and also has IsMdiContainer property set, in which, the MdiClient fails to
        ///  resize and hence draw the correct backcolor.
        /// </summary>
        private void SetWindowRgn()
        {
            IntPtr rgn1 = IntPtr.Zero;
            IntPtr rgn2 = IntPtr.Zero;
            RECT rect = new RECT();
            CreateParams cp = CreateParams;

            AdjustWindowRectEx(ref rect, cp.Style, false, cp.ExStyle);

            Rectangle bounds = Bounds;
            rgn1 = Gdi32.CreateRectRgn(0, 0, bounds.Width, bounds.Height);
            try
            {
                rgn2 = Gdi32.CreateRectRgn(
                    -rect.left,
                    -rect.top,
                    bounds.Width - rect.right,
                    bounds.Height - rect.bottom);

                try
                {
                    if (rgn1 == IntPtr.Zero || rgn2 == IntPtr.Zero)
                    {
                        throw new InvalidOperationException(SR.ErrorSettingWindowRegion);
                    }

                    if (Gdi32.CombineRgn(rgn1, rgn1, rgn2, Gdi32.CombineMode.RGN_DIFF) == 0)
                    {
                        throw new InvalidOperationException(SR.ErrorSettingWindowRegion);
                    }

                    if (UnsafeNativeMethods.SetWindowRgn(new HandleRef(this, Handle), new HandleRef(null, rgn1), true) == 0)
                    {
                        throw new InvalidOperationException(SR.ErrorSettingWindowRegion);
                    }
                    else
                    {
                        // The hwnd now owns the region.
                        rgn1 = IntPtr.Zero;
                    }
                }
                finally
                {
                    if (rgn2 != IntPtr.Zero)
                    {
                        Gdi32.DeleteObject(rgn2);
                    }
                }
            }
            finally
            {
                if (rgn1 != IntPtr.Zero)
                {
                    Gdi32.DeleteObject(rgn1);
                }
            }
        }

        internal override bool ShouldSerializeBackColor()
        {
            return BackColor != SystemColors.AppWorkspace;
        }

        private bool ShouldSerializeLocation()
        {
            return false;
        }

        internal override bool ShouldSerializeSize()
        {
            return false;
        }

        /// <summary>
        ///  Processes Windows messages.
        /// </summary>
        /// <param name="m">The Windows <see cref="Message" /> to process.</param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {

                case WindowMessages.WM_CREATE:
                    if (ParentInternal != null && ParentInternal.Site != null && ParentInternal.Site.DesignMode && Handle != IntPtr.Zero)
                    {
                        SetWindowRgn();
                    }
                    break;

                case WindowMessages.WM_SETFOCUS:
                    InvokeGotFocus(ParentInternal, EventArgs.Empty);
                    Form childForm = null;
                    if (ParentInternal is Form)
                    {
                        childForm = ((Form)ParentInternal).ActiveMdiChildInternal;
                    }
                    if (childForm == null && MdiChildren.Length > 0 && MdiChildren[0].IsMdiChildFocusable)
                    {
                        childForm = MdiChildren[0];
                    }
                    if (childForm != null && childForm.Visible)
                    {
                        childForm.Active = true;
                    }

                    // Do not use control's implementation of WmSetFocus
                    // as it will improperly activate this control.
                    WmImeSetFocus();
                    DefWndProc(ref m);
                    InvokeGotFocus(this, EventArgs.Empty);
                    return;
                case WindowMessages.WM_KILLFOCUS:
                    InvokeLostFocus(ParentInternal, EventArgs.Empty);
                    break;
            }
            base.WndProc(ref m);
        }

        internal override void OnInvokedSetScrollPosition(object sender, EventArgs e)
        {
            Application.Idle += new EventHandler(OnIdle); //do this on idle (it must be mega-delayed).
        }

        private void OnIdle(object sender, EventArgs e)
        {
            Application.Idle -= new EventHandler(OnIdle);
            base.OnInvokedSetScrollPosition(sender, e);
        }

        /// <summary>
        ///  Collection of controls...
        /// </summary>
        [ComVisible(false)]
        new public class ControlCollection : Control.ControlCollection
        {
            private readonly MdiClient owner;

            /*C#r: protected*/

            public ControlCollection(MdiClient owner)
            : base(owner)
            {
                this.owner = owner;
            }

            /// <summary>
            ///  Adds a control to the MDI Container. This child must be
            ///  a Form that is marked as an MDI Child to be added to the
            ///  container. You should not call this directly, but rather
            ///  set the child form's (ctl) MDIParent property:
            /// <code>
            ///  //     wrong
            ///  Form child = new ChildForm();
            ///  this.getMdiClient().add(child);
            ///  //     right
            ///  Form child = new ChildForm();
            ///  child.setMdiParent(this);
            /// </code>
            /// </summary>
            public override void Add(Control value)
            {
                if (value == null)
                {
                    return;
                }
                if (!(value is Form) || !((Form)value).IsMdiChild)
                {
                    throw new ArgumentException(SR.MDIChildAddToNonMDIParent, "value");
                }
                if (owner.CreateThreadId != value.CreateThreadId)
                {
                    throw new ArgumentException(SR.AddDifferentThreads, "value");
                }
                owner.children.Add((Form)value);
                base.Add(value);
            }

            /// <summary>
            ///  Removes a child control.
            /// </summary>
            public override void Remove(Control value)
            {
                owner.children.Remove(value);
                base.Remove(value);
            }
        }
    }
}

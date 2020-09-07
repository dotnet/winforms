// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        internal partial class DropDownHolder : Form, IMouseHookClient
        {
            private Control currentControl; // the control that is hosted in the holder
            private readonly PropertyGridView gridView; // the owner gridview
            private readonly MouseHook mouseHook; // we use this to hook mouse downs, etc. to know when to close the dropdown.

            private LinkLabel createNewLink;

            // all the resizing goo...
            //
            private bool resizable = true; // true if we're showing the resize widget.
            private bool resizing; // true if we're in the middle of a resize operation.
            private bool resizeUp; // true if the dropdown is above the grid row, which means the resize widget is at the top.
            private Point dragStart = Point.Empty; // the point at which the drag started to compute the delta
            private Rectangle dragBaseRect = Rectangle.Empty; // the original bounds of our control.
            private int currentMoveType = MoveTypeNone; // what type of move are we processing? left, bottom, or both?

            private readonly static int ResizeBarSize = ResizeGripSize + 1; // the thickness of the resize bar
            private readonly static int ResizeBorderSize = ResizeBarSize / 2; // the thickness of the 2-way resize area along the outer edge of the resize bar
            private readonly static int ResizeGripSize = SystemInformation.HorizontalScrollBarHeight; // the size of the 4-way resize grip at outermost corner of the resize bar
            private readonly static Size MinDropDownSize =
                new Size(SystemInformation.VerticalScrollBarWidth * 4, SystemInformation.HorizontalScrollBarHeight * 4); // the minimum size for the control.
            private Bitmap sizeGripGlyph; // our cached size grip glyph.  Control paint only does right bottom glyphs, so we cache a mirrored one.  See GetSizeGripGlyph

            private const int DropDownHolderBorder = 1;
            private const int MoveTypeNone = 0x0;
            private const int MoveTypeBottom = 0x1;
            private const int MoveTypeLeft = 0x2;
            private const int MoveTypeTop = 0x4;

            internal DropDownHolder(PropertyGridView psheet)
                : base()
            {
                ShowInTaskbar = false;
                ControlBox = false;
                MinimizeBox = false;
                MaximizeBox = false;
                Text = string.Empty;
                FormBorderStyle = FormBorderStyle.None;
                AutoScaleMode = AutoScaleMode.None; // children may scale, but we won't interfere.
                mouseHook = new MouseHook(this, this, psheet);
                Visible = false;
                gridView = psheet;
                BackColor = gridView.BackColor;
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.Style |= unchecked((int)(User32.WS.POPUP | User32.WS.BORDER));
                    cp.ExStyle |= (int)User32.WS_EX.TOOLWINDOW;
                    cp.ClassStyle |= (int)User32.CS.DROPSHADOW;
                    if (gridView != null)
                    {
                        cp.Parent = gridView.ParentInternal.Handle;
                    }
                    return cp;
                }
            }

            private LinkLabel CreateNewLink
            {
                get
                {
                    if (createNewLink is null)
                    {
                        createNewLink = new LinkLabel();
                        createNewLink.LinkClicked += new LinkLabelLinkClickedEventHandler(OnNewLinkClicked);
                    }
                    return createNewLink;
                }
            }

            public virtual bool HookMouseDown
            {
                get
                {
                    return mouseHook.HookMouseDown;
                }
                set
                {
                    mouseHook.HookMouseDown = value;
                }
            }

            /// <summary>
            ///  This gets set to true if there isn't enough space below the currently selected
            ///  row for the drop down, so it appears above the row.  In this case, we make the resize
            ///  grip appear at the top left.
            /// </summary>
            public bool ResizeUp
            {
                set
                {
                    if (resizeUp != value)
                    {
                        // clear the glyph so we regenerate it.
                        //
                        sizeGripGlyph = null;
                        resizeUp = value;

                        if (resizable)
                        {
                            DockPadding.Bottom = 0;
                            DockPadding.Top = 0;
                            if (value)
                            {
                                DockPadding.Top = ResizeBarSize;
                            }
                            else
                            {
                                DockPadding.Bottom = ResizeBarSize;
                            }
                        }
                    }
                }
            }

            internal override bool SupportsUiaProviders => true;

            protected override AccessibleObject CreateAccessibilityInstance()
                => new DropDownHolderAccessibleObject(this);

            protected override void DestroyHandle()
            {
                mouseHook.HookMouseDown = false;
                base.DestroyHandle();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing && createNewLink != null)
                {
                    createNewLink.Dispose();
                    createNewLink = null;
                }
                base.Dispose(disposing);
            }

            public void DoModalLoop()
            {
                // Push a modal loop.  This seems expensive, but I think it is a
                // better user model than returning from DropDownControl immediately.
                while (Visible)
                {
                    Application.DoEventsModal();
                    User32.MsgWaitForMultipleObjectsEx(0, IntPtr.Zero, 250, User32.QS.ALLINPUT, User32.MWMO.INPUTAVAILABLE);
                }
            }

            public virtual Control Component
            {
                get
                {
                    return currentControl;
                }
            }

            /// <summary>
            ///  Get an InstanceCreationEditor for this entry.  First, we look on the property type, and if we
            ///  don't find that we'll go up to the editor type itself.  That way people can associate the InstanceCreationEditor with
            ///  the type of DropDown UIType Editor.
            ///
            /// </summary>
            private InstanceCreationEditor GetInstanceCreationEditor(PropertyDescriptorGridEntry entry)
            {
                if (entry is null)
                {
                    return null;
                }

                InstanceCreationEditor editor = null;

                // check the property type itself.  this is the default path.
                //
                PropertyDescriptor pd = entry.PropertyDescriptor;
                if (pd != null)
                {
                    editor = pd.GetEditor(typeof(InstanceCreationEditor)) as InstanceCreationEditor;
                }

                // now check if there is a dropdown UI type editor.  If so, use that.
                //
                if (editor is null)
                {
                    UITypeEditor ute = entry.UITypeEditor;
                    if (ute != null && ute.GetEditStyle() == UITypeEditorEditStyle.DropDown)
                    {
                        editor = (InstanceCreationEditor)TypeDescriptor.GetEditor(ute, typeof(InstanceCreationEditor));
                    }
                }
                return editor;
            }

            /// <summary>
            ///  Get a glyph for sizing the lower left hand grip.  The code in ControlPaint only does lower-right glyphs
            ///  so we do some GDI+ magic to take that glyph and mirror it.  That way we can still share the code (in case it changes for theming, etc),
            ///  not have any special cases, and possibly solve world hunger.
            /// </summary>
            private Bitmap GetSizeGripGlyph(Graphics g)
            {
                if (sizeGripGlyph != null)
                {
                    return sizeGripGlyph;
                }

                // create our drawing surface based on the current graphics context.
                //
                sizeGripGlyph = new Bitmap(ResizeGripSize, ResizeGripSize, g);

                using (Graphics glyphGraphics = Graphics.FromImage(sizeGripGlyph))
                {
                    // mirror the image around the x-axis to get a gripper handle that works
                    // for the lower left.
                    Matrix m = new Matrix();

                    // basically, mirroring is just scaling by -1 on the X-axis.  So any point that's like (10, 10) goes to (-10, 10).
                    // that mirrors it, but also moves everything to the negative axis, so we just bump the whole thing over by it's width.
                    //
                    // the +1 is because things at (0,0) stay at (0,0) since [0 * -1 = 0] so we want to get them over to the other side too.
                    //
                    // resizeUp causes the image to also be mirrored vertically so the grip can be used as a top-left grip instead of bottom-left.
                    //
                    m.Translate(ResizeGripSize + 1, (resizeUp ? ResizeGripSize + 1 : 0));
                    m.Scale(-1, (resizeUp ? -1 : 1));
                    glyphGraphics.Transform = m;
                    ControlPaint.DrawSizeGrip(glyphGraphics, BackColor, 0, 0, ResizeGripSize, ResizeGripSize);
                    glyphGraphics.ResetTransform();
                }
                sizeGripGlyph.MakeTransparent(BackColor);
                return sizeGripGlyph;
            }

            public virtual bool GetUsed()
            {
                return (currentControl != null);
            }

            public virtual void FocusComponent()
            {
                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "DropDownHolder:FocusComponent()");
                if (currentControl != null && Visible)
                {
                    currentControl.Focus();
                }
            }

            /// <summary>
            ///  General purpose method, based on Control.Contains()...
            ///
            ///  Determines whether a given window (specified using native window handle)
            ///  is a descendant of this control. This catches both contained descendants
            ///  and 'owned' windows such as modal dialogs. Using window handles rather
            ///  than Control objects allows it to catch un-managed windows as well.
            /// </summary>
            private bool OwnsWindow(IntPtr hWnd)
            {
                while (hWnd != IntPtr.Zero)
                {
                    hWnd = User32.GetWindowLong(hWnd, User32.GWL.HWNDPARENT);
                    if (hWnd == IntPtr.Zero)
                    {
                        return false;
                    }
                    if (hWnd == Handle)
                    {
                        return true;
                    }
                }
                return false;
            }

            public bool OnClickHooked()
            {
                gridView.CloseDropDownInternal(false);
                return false;
            }

            private void OnCurrentControlResize(object o, EventArgs e)
            {
                if (currentControl != null && !resizing)
                {
                    int oldWidth = Width;
                    Size newSize = new Size(2 * DropDownHolderBorder + currentControl.Width, 2 * DropDownHolderBorder + currentControl.Height);
                    if (resizable)
                    {
                        newSize.Height += ResizeBarSize;
                    }
                    try
                    {
                        resizing = true;
                        SuspendLayout();
                        Size = newSize;
                    }
                    finally
                    {
                        resizing = false;
                        ResumeLayout(false);
                    }
                    Left -= (Width - oldWidth);
                }
            }

            protected override void OnLayout(LayoutEventArgs levent)
            {
                try
                {
                    resizing = true;
                    base.OnLayout(levent);
                }
                finally
                {
                    resizing = false;
                }
            }

            private void OnNewLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            {
                InstanceCreationEditor ice = e.Link.LinkData as InstanceCreationEditor;

                Debug.Assert(ice != null, "How do we have a link without the InstanceCreationEditor?");
                if (ice != null && gridView?.SelectedGridEntry != null)
                {
                    Type createType = gridView.SelectedGridEntry.PropertyType;
                    if (createType != null)
                    {
                        gridView.CloseDropDown();

                        object newValue = ice.CreateInstance(gridView.SelectedGridEntry, createType);

                        if (newValue != null)
                        {
                            // make sure we got what we asked for.
                            //
                            if (!createType.IsInstanceOfType(newValue))
                            {
                                throw new InvalidCastException(string.Format(SR.PropertyGridViewEditorCreatedInvalidObject, createType));
                            }

                            gridView.CommitValue(newValue);
                        }
                    }
                }
            }

            /// <summary>
            ///  Just figure out what kind of sizing we would do at a given drag location.
            /// </summary>
            private int MoveTypeFromPoint(int x, int y)
            {
                Rectangle bGripRect = new Rectangle(0, Height - ResizeGripSize, ResizeGripSize, ResizeGripSize);
                Rectangle tGripRect = new Rectangle(0, 0, ResizeGripSize, ResizeGripSize);

                if (!resizeUp && bGripRect.Contains(x, y))
                {
                    return MoveTypeLeft | MoveTypeBottom;
                }
                else if (resizeUp && tGripRect.Contains(x, y))
                {
                    return MoveTypeLeft | MoveTypeTop;
                }
                else if (!resizeUp && Math.Abs(Height - y) < ResizeBorderSize)
                {
                    return MoveTypeBottom;
                }
                else if (resizeUp && Math.Abs(y) < ResizeBorderSize)
                {
                    return MoveTypeTop;
                }
                return MoveTypeNone;
            }

            /// <summary>
            ///  Decide if we're going to be sizing at the given point, and if so, Capture and safe our current state.
            /// </summary>
            protected override void OnMouseDown(MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    currentMoveType = MoveTypeFromPoint(e.X, e.Y);
                    if (currentMoveType != MoveTypeNone)
                    {
                        dragStart = PointToScreen(new Point(e.X, e.Y));
                        dragBaseRect = Bounds;
                        Capture = true;
                    }
                    else
                    {
                        gridView.CloseDropDown();
                    }
                }
                base.OnMouseDown(e);
            }

            /// <summary>
            ///  Either set the cursor or do a move, depending on what our currentMoveType is/
            /// </summary>
            protected override void OnMouseMove(MouseEventArgs e)
            {
                // not moving so just set the cursor.
                //
                if (currentMoveType == MoveTypeNone)
                {
                    int cursorMoveType = MoveTypeFromPoint(e.X, e.Y);
                    switch (cursorMoveType)
                    {
                        case (MoveTypeLeft | MoveTypeBottom):
                            Cursor = Cursors.SizeNESW;
                            break;
                        case MoveTypeBottom:
                        case MoveTypeTop:
                            Cursor = Cursors.SizeNS;
                            break;
                        case MoveTypeTop | MoveTypeLeft:
                            Cursor = Cursors.SizeNWSE;
                            break;
                        default:
                            Cursor = null;
                            break;
                    }
                }
                else
                {
                    Point dragPoint = PointToScreen(new Point(e.X, e.Y));
                    Rectangle newBounds = Bounds;

                    // we're in a move operation, so do the resize.
                    //
                    if ((currentMoveType & MoveTypeBottom) == MoveTypeBottom)
                    {
                        newBounds.Height = Math.Max(MinDropDownSize.Height, dragBaseRect.Height + (dragPoint.Y - dragStart.Y));
                    }

                    // for left and top moves, we actually have to resize and move the form simultaneously.
                    // do to that, we compute the xdelta, and apply that to the base rectangle if it's not going to
                    // make the form smaller than the minimum.
                    //
                    if ((currentMoveType & MoveTypeTop) == MoveTypeTop)
                    {
                        int delta = dragPoint.Y - dragStart.Y;

                        if ((dragBaseRect.Height - delta) > MinDropDownSize.Height)
                        {
                            newBounds.Y = dragBaseRect.Top + delta;
                            newBounds.Height = dragBaseRect.Height - delta;
                        }
                    }

                    if ((currentMoveType & MoveTypeLeft) == MoveTypeLeft)
                    {
                        int delta = dragPoint.X - dragStart.X;

                        if ((dragBaseRect.Width - delta) > MinDropDownSize.Width)
                        {
                            newBounds.X = dragBaseRect.Left + delta;
                            newBounds.Width = dragBaseRect.Width - delta;
                        }
                    }

                    if (newBounds != Bounds)
                    {
                        try
                        {
                            resizing = true;
                            Bounds = newBounds;
                        }
                        finally
                        {
                            resizing = false;
                        }
                    }

                    // Redraw!
                    //
                    Invalidate();
                }
                base.OnMouseMove(e);
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                // just clear the cursor back to the default.
                //
                Cursor = null;
                base.OnMouseLeave(e);
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);

                if (e.Button == MouseButtons.Left)
                {
                    // reset the world.
                    //
                    currentMoveType = MoveTypeNone;
                    dragStart = Point.Empty;
                    dragBaseRect = Rectangle.Empty;
                    Capture = false;
                }
            }

            /// <summary>
            ///  Just paint and draw our glyph.
            /// </summary>
            protected override void OnPaint(PaintEventArgs pe)
            {
                base.OnPaint(pe);
                if (resizable)
                {
                    // Draw the grip
                    Rectangle lRect = new Rectangle(0, resizeUp ? 0 : Height - ResizeGripSize, ResizeGripSize, ResizeGripSize);
                    pe.Graphics.DrawImage(GetSizeGripGlyph(pe.Graphics), lRect);

                    // Draw the divider
                    int y = resizeUp ? (ResizeBarSize - 1) : (Height - ResizeBarSize);
                    Pen pen = new Pen(SystemColors.ControlDark, 1)
                    {
                        DashStyle = DashStyle.Solid
                    };
                    pe.Graphics.DrawLine(pen, 0, y, Width, y);
                    pen.Dispose();
                }
            }

            protected override bool ProcessDialogKey(Keys keyData)
            {
                if ((keyData & (Keys.Shift | Keys.Control | Keys.Alt)) == 0)
                {
                    switch (keyData & Keys.KeyCode)
                    {
                        case Keys.Escape:
                            gridView.OnEscape(this);
                            return true;
                        case Keys.F4:
                            gridView.F4Selection(true);
                            return true;
                        case Keys.Return:
                            // make sure the return gets forwarded to the control that
                            // is being displayed
                            if (gridView.UnfocusSelection() && gridView.SelectedGridEntry != null)
                            {
                                gridView.SelectedGridEntry.OnValueReturnKey();
                            }
                            return true;
                    }
                }

                return base.ProcessDialogKey(keyData);
            }

            public void SetComponent(Control ctl, bool resizable)
            {
                this.resizable = resizable;
                Font = gridView.Font;

                // check to see if we're going to be adding an InstanceCreationEditor
                //
                InstanceCreationEditor editor = (ctl is null ? null : GetInstanceCreationEditor(gridView.SelectedGridEntry as PropertyDescriptorGridEntry));

                // clear any existing control we have
                //
                if (currentControl != null)
                {
                    currentControl.Resize -= new EventHandler(OnCurrentControlResize);
                    Controls.Remove(currentControl);
                    currentControl = null;
                }

                // remove the InstanceCreationEditor link
                //
                if (createNewLink != null && createNewLink.Parent == this)
                {
                    Controls.Remove(createNewLink);
                }

                // now set up the new control, top to bottom
                //
                if (ctl != null)
                {
                    currentControl = ctl;
                    Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "DropDownHolder:SetComponent(" + (ctl.GetType().Name) + ")");

                    DockPadding.All = 0;

                    // first handle the control.  If it's a listbox, make sure it's got some height
                    // to it.
                    //
                    if (currentControl is GridViewListBox)
                    {
                        ListBox lb = (ListBox)currentControl;

                        if (lb.Items.Count == 0)
                        {
                            lb.Height = Math.Max(lb.Height, lb.ItemHeight);
                        }
                    }

                    // Parent the control now.  That way it can inherit our
                    // font and scale itself if it wants to.
                    try
                    {
                        SuspendLayout();
                        Controls.Add(ctl);

                        Size sz = new Size(2 * DropDownHolderBorder + ctl.Width, 2 * DropDownHolderBorder + ctl.Height);

                        // now check for an editor, and show the link if there is one.
                        //
                        if (editor != null)
                        {
                            // set up the link.
                            //
                            CreateNewLink.Text = editor.Text;
                            CreateNewLink.Links.Clear();
                            CreateNewLink.Links.Add(0, editor.Text.Length, editor);

                            // size it as close to the size of the text as possible.
                            //
                            int linkHeight = CreateNewLink.Height;
                            using (Graphics g = gridView.CreateGraphics())
                            {
                                SizeF sizef = PropertyGrid.MeasureTextHelper.MeasureText(gridView.OwnerGrid, g, editor.Text, gridView.GetBaseFont());
                                linkHeight = (int)sizef.Height;
                            }

                            CreateNewLink.Height = linkHeight + DropDownHolderBorder;

                            // add the total height plus some border
                            sz.Height += (linkHeight + (DropDownHolderBorder * 2));
                        }

                        // finally, if we're resizable, add the space for the widget.
                        //
                        if (resizable)
                        {
                            sz.Height += ResizeBarSize;

                            // we use dockpadding to save space to draw the widget.
                            //
                            if (resizeUp)
                            {
                                DockPadding.Top = ResizeBarSize;
                            }
                            else
                            {
                                DockPadding.Bottom = ResizeBarSize;
                            }
                        }

                        // set the size stuff.
                        //
                        Size = sz;
                        ctl.Dock = DockStyle.Fill;
                        ctl.Visible = true;

                        if (editor != null)
                        {
                            CreateNewLink.Dock = DockStyle.Bottom;
                            Controls.Add(CreateNewLink);
                        }
                    }
                    finally
                    {
                        ResumeLayout(true);
                    }

                    // hook the resize event.
                    //
                    currentControl.Resize += new EventHandler(OnCurrentControlResize);
                }
                Enabled = currentControl != null;
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == (int)User32.WM.ACTIVATE)
                {
                    SetState(States.Modal, true);
                    Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "DropDownHolder:WM_ACTIVATE()");
                    IntPtr activatedWindow = (IntPtr)m.LParam;
                    if (Visible && PARAM.LOWORD(m.WParam) == (int)User32.WA.INACTIVE && !OwnsWindow(activatedWindow))
                    {
                        gridView.CloseDropDownInternal(false);
                        return;
                    }
                }
                else if (m.Msg == (int)User32.WM.CLOSE)
                {
                    // don't let an ALT-F4 get you down
                    //
                    if (Visible)
                    {
                        gridView.CloseDropDown();
                    }
                    return;
                }
                else if (m.Msg == (int)User32.WM.DPICHANGED)
                {
                    // Dropdownholder in PropertyGridView is already scaled based on parent font and other properties that were already set for new DPI
                    // This case is to avoid rescaling(double scaling) of this form
                    m.Result = IntPtr.Zero;
                    return;
                }

                base.WndProc(ref m);
            }
        }
    }
}

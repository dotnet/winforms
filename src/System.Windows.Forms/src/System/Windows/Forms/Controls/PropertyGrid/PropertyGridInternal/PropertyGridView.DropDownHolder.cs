// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class PropertyGridView
{
    internal sealed partial class DropDownHolder : Form, IMouseHookClient
    {
        private Control? _currentControl;            // the control that is hosted in the holder
        private readonly PropertyGridView _gridView; // the owner gridview
        private readonly MouseHook _mouseHook;       // we use this to hook mouse downs, etc. to know when to close the dropdown.

        private LinkLabel? _createNewLinkLabel;

        // Resizing

        private bool _resizable = true;                         // true if we're showing the resize widget.
        private bool _resizing;                                 // true if we're in the middle of a resize operation.
        private bool _resizeUp;                                 // true if the dropdown is above the grid row, which means the resize widget is at the top.
        private Point _dragStart = Point.Empty;                 // the point at which the drag started to compute the delta
        private Rectangle _dragBaseRect = Rectangle.Empty;      // the original bounds of our control.
        private int _currentMoveType = MoveTypeNone;            // what type of move are we processing? left, bottom, or both?

        // The size of the 4-way resize grip at outermost corner of the resize bar
        private static readonly int s_resizeGripSize = SystemInformation.HorizontalScrollBarHeight;

        // The thickness of the resize bar
        private static readonly int s_resizeBarSize = s_resizeGripSize + 1;

        // The thickness of the 2-way resize area along the outer edge of the resize bar
        private static readonly int s_resizeBorderSize = s_resizeBarSize / 2;

        // The minimum size for the control.
        private static readonly Size s_minDropDownSize =
            new(SystemInformation.VerticalScrollBarWidth * 4, SystemInformation.HorizontalScrollBarHeight * 4);

        // Our cached size grip glyph. Control paint only does right bottom glyphs, so we cache a mirrored one.
        private Bitmap? _sizeGripGlyph;

        private const int DropDownHolderBorder = 1;
        private const int MoveTypeNone = 0x0;
        private const int MoveTypeBottom = 0x1;
        private const int MoveTypeLeft = 0x2;
        private const int MoveTypeTop = 0x4;

        internal DropDownHolder(PropertyGridView gridView) : base()
        {
            ShowInTaskbar = false;
            ControlBox = false;
            MinimizeBox = false;
            MaximizeBox = false;
            Text = string.Empty;
            FormBorderStyle = FormBorderStyle.None;
            AutoScaleMode = AutoScaleMode.None; // children may scale, but we won't interfere.
            _mouseHook = new(this, this, gridView);
            Visible = false;
            _gridView = gridView;
            BackColor = _gridView.BackColor;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= unchecked((int)(WINDOW_STYLE.WS_POPUP | WINDOW_STYLE.WS_BORDER));
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_TOOLWINDOW;
                cp.ClassStyle |= (int)WNDCLASS_STYLES.CS_DROPSHADOW;
                if (_gridView is not null && _gridView.ParentInternal is not null)
                {
                    cp.Parent = _gridView.ParentInternal.Handle;
                }

                return cp;
            }
        }

        private LinkLabel CreateNewLink
        {
            get
            {
                if (_createNewLinkLabel is null)
                {
                    _createNewLinkLabel = new LinkLabel();
                    _createNewLinkLabel.LinkClicked += OnNewLinkClicked;
                }

                return _createNewLinkLabel;
            }
        }

        public bool HookMouseDown
        {
            get => _mouseHook.HookMouseDown;
            set => _mouseHook.HookMouseDown = value;
        }

        /// <summary>
        ///  This gets set to true if there isn't enough space below the currently selected
        ///  row for the drop down, so it appears above the row. In this case, we make the resize
        ///  grip appear at the top left.
        /// </summary>
        public bool ResizeUp
        {
            set
            {
                if (_resizeUp == value)
                {
                    return;
                }

                // Clear the glyph so we regenerate it.
                _sizeGripGlyph = null;
                _resizeUp = value;

                if (_resizable)
                {
                    DockPadding.Bottom = 0;
                    DockPadding.Top = 0;
                    if (value)
                    {
                        DockPadding.Top = s_resizeBarSize;
                    }
                    else
                    {
                        DockPadding.Bottom = s_resizeBarSize;
                    }
                }
            }
        }

        internal override bool SupportsUiaProviders => true;

        protected override AccessibleObject CreateAccessibilityInstance()
            => new DropDownHolderAccessibleObject(this);

        protected override void DestroyHandle()
        {
            _mouseHook.HookMouseDown = false;
            base.DestroyHandle();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _createNewLinkLabel is not null)
            {
                _createNewLinkLabel.Dispose();
                _createNewLinkLabel = null;
            }

            base.Dispose(disposing);
        }

        public unsafe void DoModalLoop()
        {
            // Push a modal loop. This seems expensive, but it is a better user model than
            // returning from DropDownControl immediately.
            while (Visible)
            {
                Application.DoEventsModal();
                PInvoke.MsgWaitForMultipleObjectsEx(
                    0,
                    null,
                    250,
                    QUEUE_STATUS_FLAGS.QS_ALLINPUT,
                    MSG_WAIT_FOR_MULTIPLE_OBJECTS_EX_FLAGS.MWMO_INPUTAVAILABLE);
            }
        }

        public Control? Component => _currentControl;

        private static InstanceCreationEditor? GetInstanceCreationEditor(PropertyDescriptorGridEntry? entry)
        {
            // First we look on the property type, and if we don't find that we'll go up to the editor type
            // itself. That way people can associate the InstanceCreationEditor with the type of DropDown
            // UIType Editor.

            if (entry is null)
            {
                return null;
            }

            // Check the property type itself. This is the default path.
            var editor = entry.PropertyDescriptor.GetEditor(typeof(InstanceCreationEditor)) as InstanceCreationEditor;

            // Now check if there is a dropdown UI type editor. If so, use that.
            if (editor is null)
            {
                UITypeEditor? ute = entry.UITypeEditor;
                if (ute is not null && ute.GetEditStyle() == UITypeEditorEditStyle.DropDown)
                {
                    editor = (InstanceCreationEditor?)TypeDescriptor.GetEditor(ute, typeof(InstanceCreationEditor));
                }
            }

            return editor;
        }

        /// <summary>
        ///  Get a glyph for sizing the lower left hand grip.
        /// </summary>
        private Bitmap GetSizeGripGlyph(Graphics g)
        {
            // The code in ControlPaint only does lower-right glyphs so we do some GDI+ magic to take that glyph
            // and mirror it. That way we can still share the code (in case it changes for theming, etc) and not
            // have any special cases.

            if (_sizeGripGlyph is not null)
            {
                return _sizeGripGlyph;
            }

            // Create our drawing surface based on the current graphics context.
            _sizeGripGlyph = new Bitmap(s_resizeGripSize, s_resizeGripSize, g);

            using (Graphics glyphGraphics = Graphics.FromImage(_sizeGripGlyph))
            {
                // Mirror the image around the x-axis to get a gripper handle that works for the lower left.
                Matrix m = new();

                // Mirroring is just scaling by -1 on the X-axis. So any point that's like (10, 10) goes to (-10, 10).
                // That mirrors it, but also moves everything to the negative axis, so we just bump the whole thing
                // over by it's width.
                //
                // The +1 is because things at (0,0) stay at (0,0) since [0 * -1 = 0] and we want to get them over
                // to the other side too.
                //
                // _resizeUp causes the image to also be mirrored vertically so the grip can be used as a top-left
                // grip instead of bottom-left.

                m.Translate(s_resizeGripSize + 1, (_resizeUp ? s_resizeGripSize + 1 : 0));
                m.Scale(-1, (_resizeUp ? -1 : 1));
                glyphGraphics.Transform = m;
                ControlPaint.DrawSizeGrip(glyphGraphics, BackColor, 0, 0, s_resizeGripSize, s_resizeGripSize);
                glyphGraphics.ResetTransform();
            }

            _sizeGripGlyph.MakeTransparent(BackColor);
            return _sizeGripGlyph;
        }

        public bool GetUsed() => _currentControl is not null;

        public void FocusComponent()
        {
            if (_currentControl is not null && Visible)
            {
                _currentControl.Focus();
            }
        }

        /// <summary>
        ///  Determines whether a given window (specified using native window handle)
        ///  is a descendant of this control. This catches both contained descendants
        ///  and 'owned' windows such as modal dialogs. Using window handles rather
        ///  than Control objects allows it to catch un-managed windows as well.
        /// </summary>
        private bool OwnsWindow(HWND hWnd)
        {
            while (!hWnd.IsNull)
            {
                hWnd = (HWND)PInvokeCore.GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT);
                if (hWnd.IsNull)
                {
                    return false;
                }

                if (hWnd == HWND)
                {
                    return true;
                }
            }

            return false;
        }

        public bool OnClickHooked()
        {
            _gridView.CloseDropDownInternal(resetFocus: false);
            return false;
        }

        private void OnCurrentControlResize(object? o, EventArgs e)
        {
            if (_currentControl is null || _resizing)
            {
                return;
            }

            int oldWidth = Width;
            Size newSize = new(2 * DropDownHolderBorder + _currentControl.Width, 2 * DropDownHolderBorder + _currentControl.Height);
            if (_resizable)
            {
                newSize.Height += s_resizeBarSize;
            }

            try
            {
                _resizing = true;
                SuspendLayout();
                Size = newSize;
            }
            finally
            {
                _resizing = false;
                ResumeLayout(false);
            }

            Left -= (Width - oldWidth);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            try
            {
                _resizing = true;
                base.OnLayout(levent);
            }
            finally
            {
                _resizing = false;
            }
        }

        private void OnNewLinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            InstanceCreationEditor? editor = e.Link?.LinkData as InstanceCreationEditor;

            Debug.Assert(editor is not null, "How do we have a link without the InstanceCreationEditor?");
            if (editor is not null && _gridView?.SelectedGridEntry is not null)
            {
                Type? createType = _gridView.SelectedGridEntry.PropertyType;
                if (createType is not null)
                {
                    _gridView.CloseDropDown();

                    object? newValue = editor.CreateInstance(_gridView.SelectedGridEntry, createType);

                    if (newValue is not null)
                    {
                        // Make sure we got what we asked for.
                        if (!createType.IsInstanceOfType(newValue))
                        {
                            throw new InvalidCastException(string.Format(SR.PropertyGridViewEditorCreatedInvalidObject, createType));
                        }

                        _gridView.CommitValue(newValue);
                    }
                }
            }
        }

        /// <summary>
        ///  Figure out what kind of sizing we would do at a given drag location.
        /// </summary>
        private int MoveTypeFromPoint(int x, int y)
        {
            Rectangle bottomGrip = new(0, Height - s_resizeGripSize, s_resizeGripSize, s_resizeGripSize);
            Rectangle topGrip = new(0, 0, s_resizeGripSize, s_resizeGripSize);

            if (!_resizeUp && bottomGrip.Contains(x, y))
            {
                return MoveTypeLeft | MoveTypeBottom;
            }
            else if (_resizeUp && topGrip.Contains(x, y))
            {
                return MoveTypeLeft | MoveTypeTop;
            }
            else if (!_resizeUp && Math.Abs(Height - y) < s_resizeBorderSize)
            {
                return MoveTypeBottom;
            }
            else if (_resizeUp && Math.Abs(y) < s_resizeBorderSize)
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
                _currentMoveType = MoveTypeFromPoint(e.X, e.Y);
                if (_currentMoveType != MoveTypeNone)
                {
                    _dragStart = PointToScreen(new Point(e.X, e.Y));
                    _dragBaseRect = Bounds;
                    Capture = true;
                }
                else
                {
                    _gridView.CloseDropDown();
                }
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        ///  Either set the cursor or do a move, depending on what our current move type is.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_currentMoveType == MoveTypeNone)
            {
                // Not moving so just set the cursor.
                int cursorMoveType = MoveTypeFromPoint(e.X, e.Y);
                Cursor = cursorMoveType switch
                {
                    (MoveTypeLeft | MoveTypeBottom) => Cursors.SizeNESW,
                    MoveTypeBottom or MoveTypeTop => Cursors.SizeNS,
                    MoveTypeTop | MoveTypeLeft => Cursors.SizeNWSE,
                    _ => null,
                };
            }
            else
            {
                Point dragPoint = PointToScreen(new Point(e.X, e.Y));
                Rectangle newBounds = Bounds;

                // We're in a move operation, so do the resize.
                if ((_currentMoveType & MoveTypeBottom) == MoveTypeBottom)
                {
                    newBounds.Height = Math.Max(s_minDropDownSize.Height, _dragBaseRect.Height + (dragPoint.Y - _dragStart.Y));
                }

                // For left and top moves, we actually have to resize and move the form simultaneously.
                // Due to that, we compute the x delta, and apply that to the base rectangle if it's not going to
                // make the form smaller than the minimum.
                if ((_currentMoveType & MoveTypeTop) == MoveTypeTop)
                {
                    int delta = dragPoint.Y - _dragStart.Y;

                    if ((_dragBaseRect.Height - delta) > s_minDropDownSize.Height)
                    {
                        newBounds.Y = _dragBaseRect.Top + delta;
                        newBounds.Height = _dragBaseRect.Height - delta;
                    }
                }

                if ((_currentMoveType & MoveTypeLeft) == MoveTypeLeft)
                {
                    int delta = dragPoint.X - _dragStart.X;

                    if ((_dragBaseRect.Width - delta) > s_minDropDownSize.Width)
                    {
                        newBounds.X = _dragBaseRect.Left + delta;
                        newBounds.Width = _dragBaseRect.Width - delta;
                    }
                }

                if (newBounds != Bounds)
                {
                    try
                    {
                        _resizing = true;
                        Bounds = newBounds;
                    }
                    finally
                    {
                        _resizing = false;
                    }
                }

                // Redraw.
                Invalidate();
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            // Just clear the cursor back to the default.
            Cursor = null;
            base.OnMouseLeave(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                // Reset the world.
                _currentMoveType = MoveTypeNone;
                _dragStart = Point.Empty;
                _dragBaseRect = Rectangle.Empty;
                Capture = false;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_resizable)
            {
                // Draw the grip.
                Rectangle lRect = new(0, _resizeUp ? 0 : Height - s_resizeGripSize, s_resizeGripSize, s_resizeGripSize);
                e.Graphics.DrawImage(GetSizeGripGlyph(e.Graphics), lRect);

                // Draw the divider.
                int y = _resizeUp ? (s_resizeBarSize - 1) : (Height - s_resizeBarSize);
                using Pen pen = new(SystemColors.ControlDark, 1)
                {
                    DashStyle = DashStyle.Solid
                };
                e.Graphics.DrawLine(pen, 0, y, Width, y);
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if ((keyData & (Keys.Shift | Keys.Control | Keys.Alt)) == 0)
            {
                switch (keyData & Keys.KeyCode)
                {
                    case Keys.Escape:
                        _gridView.OnEscape(this);
                        return true;
                    case Keys.F4:
                        _gridView.F4Selection(true);
                        return true;
                    case Keys.Return:
                        // make sure the return gets forwarded to the control that
                        // is being displayed
                        if (_gridView.UnfocusSelection() && _gridView.SelectedGridEntry is not null)
                        {
                            _gridView.SelectedGridEntry.OnValueReturnKey();
                        }

                        return true;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        ///  Set the control to host in this <see cref="DropDownHolder"/>.
        /// </summary>
        public void SetDropDownControl(Control? control, bool resizable)
        {
            _resizable = resizable;
            Font = _gridView.Font;

            // Check to see if we're going to be adding an InstanceCreationEditor.
            InstanceCreationEditor? editor = control is not null
                ? GetInstanceCreationEditor(_gridView.SelectedGridEntry as PropertyDescriptorGridEntry)
                : null;

            // Clear any existing control we have.
            if (_currentControl is not null)
            {
                _currentControl.Resize -= OnCurrentControlResize;
                Controls.Remove(_currentControl);
                _currentControl = null;
            }

            // Remove the InstanceCreationEditor link.
            if (_createNewLinkLabel is not null && _createNewLinkLabel.Parent == this)
            {
                Controls.Remove(_createNewLinkLabel);
            }

            if (control is null)
            {
                Enabled = false;
                return;
            }

            _currentControl = control;
            DockPadding.All = 0;

            // First handle the control. If it's a listbox, make sure it's got some height to it.
            if (_currentControl is GridViewListBox listBox)
            {
                if (listBox.Items.Count == 0)
                {
                    listBox.Height = Math.Max(listBox.Height, listBox.ItemHeight);
                }
            }

            // Parent the control now. That way it can inherit our font and scale itself if it wants to.
            using (SuspendLayoutScope scope = new(this, performLayout: true))
            {
                Controls.Add(control);

                Size size = new(2 * DropDownHolderBorder + control.Width, 2 * DropDownHolderBorder + control.Height);

                // Now check for an editor, and show the link if there is one.
                if (editor is not null)
                {
                    // Set up the link.
                    CreateNewLink.Text = editor.Text;
                    CreateNewLink.Links.Clear();
                    CreateNewLink.Links.Add(0, editor.Text.Length, editor);

                    // Size it as close to the size of the text as possible.
                    int linkHeight = CreateNewLink.Height;
                    using (Graphics g = _gridView.CreateGraphics())
                    {
                        SizeF sizef = PropertyGrid.MeasureTextHelper.MeasureText(
                            _gridView.OwnerGrid,
                            g,
                            editor.Text,
                            _gridView.GetBaseFont());
                        linkHeight = (int)sizef.Height;
                    }

                    CreateNewLink.Height = linkHeight + DropDownHolderBorder;

                    // Add the total height plus some border.
                    size.Height += (linkHeight + (DropDownHolderBorder * 2));
                }

                // Finally, if we're resizable, add the space for the widget.
                if (resizable)
                {
                    size.Height += s_resizeBarSize;

                    // We use DockPadding to save space to draw the widget.
                    if (_resizeUp)
                    {
                        DockPadding.Top = s_resizeBarSize;
                    }
                    else
                    {
                        DockPadding.Bottom = s_resizeBarSize;
                    }
                }

                // Set the size.
                Size = size;
                control.Dock = DockStyle.Fill;
                control.Visible = true;

                if (editor is not null)
                {
                    CreateNewLink.Dock = DockStyle.Bottom;
                    Controls.Add(CreateNewLink);
                }
            }

            // Hook the resize event.
            _currentControl.Resize += OnCurrentControlResize;

            Enabled = _currentControl is not null;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.MsgInternal == PInvokeCore.WM_ACTIVATE)
            {
                SetState(States.Modal, true);
                HWND activatedWindow = (HWND)m.LParamInternal;
                if (Visible && m.WParamInternal.LOWORD == PInvoke.WA_INACTIVE && !OwnsWindow(activatedWindow))
                {
                    _gridView.CloseDropDownInternal(false);
                    return;
                }
            }
            else if (m.MsgInternal == PInvokeCore.WM_CLOSE)
            {
                // Don't let an ALT-F4 get you down.
                if (Visible)
                {
                    _gridView.CloseDropDown();
                }

                return;
            }
            else if (m.MsgInternal == PInvokeCore.WM_DPICHANGED)
            {
                // Dropdownholder in PropertyGridView is already scaled based on the parent font and other
                // properties that were already set for the new DPI. This case is to avoid rescaling
                // (double scaling) of this form.
                m.ResultInternal = (LRESULT)0;
                return;
            }

            base.WndProc(ref m);
        }
    }
}

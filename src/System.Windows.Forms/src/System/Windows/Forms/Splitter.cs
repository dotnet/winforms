// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides user resizing of docked elements at run time. To use a Splitter you can
    ///  dock any control to an edge of a container, and then dock the splitter to the same
    ///  edge. The splitter will then resize the control that is previous in the docking
    ///  order.
    /// </summary>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultEvent(nameof(SplitterMoved)),
    DefaultProperty(nameof(Dock)),
    SRDescription(nameof(SR.DescriptionSplitter)),
    Designer("System.Windows.Forms.Design.SplitterDesigner, " + AssemblyRef.SystemDesign)
    ]
    public class Splitter : Control
    {
        private const int DRAW_START = 1;
        private const int DRAW_MOVE = 2;
        private const int DRAW_END = 3;

        private const int defaultWidth = 3;

        private BorderStyle borderStyle = System.Windows.Forms.BorderStyle.None;
        private int minSize = 25;
        private int minExtra = 25;
        private Point anchor = Point.Empty;
        private Control splitTarget;
        private int splitSize = -1;
        private int splitterThickness = 3;
        private int initTargetSize;
        private int lastDrawSplit = -1;
        private int maxSize;
        private static readonly object EVENT_MOVING = new object();
        private static readonly object EVENT_MOVED = new object();

        // Cannot expose IMessageFilter.PreFilterMessage through this unsealed class
        private SplitterMessageFilter splitterMessageFilter = null;

        /// <summary>
        ///  Creates a new Splitter.
        /// </summary>
        public Splitter()
        : base()
        {
            SetStyle(ControlStyles.Selectable, false);
            TabStop = false;
            minSize = 25;
            minExtra = 25;

            Dock = DockStyle.Left;
        }

        /// <summary>
        ///  The current value of the anchor property. The anchor property
        ///  determines which edges of the control are anchored to the container's
        ///  edges.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DefaultValue(AnchorStyles.None)]
        public override AnchorStyles Anchor
        {
            get
            {
                return AnchorStyles.None;
            }
            set
            {
                // do nothing!
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AllowDrop
        {
            get
            {
                return base.AllowDrop;
            }
            set
            {
                base.AllowDrop = value;
            }
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(defaultWidth, defaultWidth);
            }
        }

        protected override Cursor DefaultCursor
        {
            get
            {
                switch (Dock)
                {
                    case DockStyle.Top:
                    case DockStyle.Bottom:
                        return Cursors.HSplit;
                    case DockStyle.Left:
                    case DockStyle.Right:
                        return Cursors.VSplit;
                }
                return base.DefaultCursor;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler ForeColorChanged
        {
            add => base.ForeColorChanged += value;
            remove => base.ForeColorChanged -= value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged
        {
            add => base.BackgroundImageChanged += value;
            remove => base.BackgroundImageChanged -= value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged
        {
            add => base.BackgroundImageLayoutChanged += value;
            remove => base.BackgroundImageLayoutChanged -= value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler FontChanged
        {
            add => base.FontChanged += value;
            remove => base.FontChanged -= value;
        }

        /// <summary>
        ///  Indicates what type of border the Splitter control has.  This value
        ///  comes from the System.Windows.Forms.BorderStyle enumeration.
        /// </summary>
        [
        DefaultValue(BorderStyle.None),
        SRCategory(nameof(SR.CatAppearance)),
        DispId(NativeMethods.ActiveX.DISPID_BORDERSTYLE),
        SRDescription(nameof(SR.SplitterBorderStyleDescr))
        ]
        public BorderStyle BorderStyle
        {
            get
            {
                return borderStyle;
            }

            set
            {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(BorderStyle));
                }

                if (borderStyle != value)
                {
                    borderStyle = value;
                    UpdateStyles();
                }
            }
        }

        /// <summary>
        ///  Returns the parameters needed to create the handle.  Inheriting classes
        ///  can override this to provide extra functionality.  They should not,
        ///  however, forget to call base.getCreateParams() first to get the struct
        ///  filled up with the basic info.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
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

        protected override ImeMode DefaultImeMode
        {
            get
            {
                return ImeMode.Disable;
            }
        }

        [
        Localizable(true),
        DefaultValue(DockStyle.Left)
        ]
        public override DockStyle Dock
        {
            get { return base.Dock; }

            set
            {

                if (!(value == DockStyle.Top || value == DockStyle.Bottom || value == DockStyle.Left || value == DockStyle.Right))
                {
                    throw new ArgumentException(SR.SplitterInvalidDockEnum);
                }

                int requestedSize = splitterThickness;

                base.Dock = value;
                switch (Dock)
                {
                    case DockStyle.Top:
                    case DockStyle.Bottom:
                        if (splitterThickness != -1)
                        {
                            Height = requestedSize;
                        }
                        break;
                    case DockStyle.Left:
                    case DockStyle.Right:
                        if (splitterThickness != -1)
                        {
                            Width = requestedSize;
                        }
                        break;
                }
            }
        }

        /// <summary>
        ///  Determines if the splitter is horizontal.
        /// </summary>
        private bool Horizontal
        {
            get
            {
                DockStyle dock = Dock;
                return dock == DockStyle.Left || dock == DockStyle.Right;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public ImeMode ImeMode
        {
            get
            {
                return base.ImeMode;
            }
            set
            {
                base.ImeMode = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler ImeModeChanged
        {
            add => base.ImeModeChanged += value;
            remove => base.ImeModeChanged -= value;
        }

        /// <summary>
        ///  The minExtra is this minimum size (in pixels) of the remaining
        ///  area of the container. This area is center of the container that
        ///  is not occupied by edge docked controls, this is the are that
        ///  would be used for any fill docked control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        DefaultValue(25),
        SRDescription(nameof(SR.SplitterMinExtraDescr))
        ]
        public int MinExtra
        {
            get
            {
                return minExtra;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                minExtra = value;
            }
        }

        /// <summary>
        ///  The minSize is the minimum size (in pixels) of the target of the
        ///  splitter. The target of a splitter is always the control adjacent
        ///  to the splitter, just prior in the dock order.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        DefaultValue(25),
        SRDescription(nameof(SR.SplitterMinSizeDescr))
        ]
        public int MinSize
        {
            get
            {
                return minSize;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                minSize = value;
            }
        }

        /// <summary>
        ///  The position of the splitter. If the splitter is not bound
        ///  to a control, SplitPosition will be -1.
        /// </summary>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.SplitterSplitPositionDescr))
        ]
        public int SplitPosition
        {
            get
            {
                if (splitSize == -1)
                {
                    splitSize = CalcSplitSize();
                }

                return splitSize;
            }
            set
            {
                // calculate maxSize and other bounding conditions
                SplitData spd = CalcSplitBounds();

                // this is not an else-if to handle the maxSize < minSize case...
                // ie. we give minSize priority over maxSize...
                if (value > maxSize)
                {
                    value = maxSize;
                }

                if (value < minSize)
                {
                    value = minSize;
                }

                // if (value == splitSize) return;  -- do we need this check?

                splitSize = value;
                DrawSplitBar(DRAW_END);

                if (spd.target == null)
                {
                    splitSize = -1;
                    return;
                }

                Rectangle bounds = spd.target.Bounds;
                switch (Dock)
                {
                    case DockStyle.Top:
                        bounds.Height = value;
                        break;
                    case DockStyle.Bottom:
                        bounds.Y += bounds.Height - splitSize;
                        bounds.Height = value;
                        break;
                    case DockStyle.Left:
                        bounds.Width = value;
                        break;
                    case DockStyle.Right:
                        bounds.X += bounds.Width - splitSize;
                        bounds.Width = value;
                        break;
                }
                spd.target.Bounds = bounds;
                Application.DoEvents();
                OnSplitterMoved(new SplitterEventArgs(Left, Top, (Left + bounds.Width / 2), (Top + bounds.Height / 2)));

            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool TabStop
        {
            get
            {
                return base.TabStop;
            }
            set
            {
                base.TabStop = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TabStopChanged
        {
            add => base.TabStopChanged += value;
            remove => base.TabStopChanged -= value;
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

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler Enter
        {
            add => base.Enter += value;
            remove => base.Enter -= value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyEventHandler KeyUp
        {
            add => base.KeyUp += value;
            remove => base.KeyUp -= value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyEventHandler KeyDown
        {
            add => base.KeyDown += value;
            remove => base.KeyDown -= value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyPressEventHandler KeyPress
        {
            add => base.KeyPress += value;
            remove => base.KeyPress -= value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler Leave
        {
            add => base.Leave += value;
            remove => base.Leave -= value;
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.SplitterSplitterMovingDescr))]
        public event SplitterEventHandler SplitterMoving
        {
            add => Events.AddHandler(EVENT_MOVING, value);
            remove => Events.RemoveHandler(EVENT_MOVING, value);
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.SplitterSplitterMovedDescr))]
        public event SplitterEventHandler SplitterMoved
        {
            add => Events.AddHandler(EVENT_MOVED, value);
            remove => Events.RemoveHandler(EVENT_MOVED, value);
        }

        /// <summary>
        ///  Draws the splitter bar at the current location. Will automatically
        ///  cleanup anyplace the splitter was drawn previously.
        /// </summary>
        private void DrawSplitBar(int mode)
        {
            if (mode != DRAW_START && lastDrawSplit != -1)
            {
                DrawSplitHelper(lastDrawSplit);
                lastDrawSplit = -1;
            }
            // Bail if drawing with no old point...
            //
            else if (mode != DRAW_START && lastDrawSplit == -1)
            {
                return;
            }

            if (mode != DRAW_END)
            {
                DrawSplitHelper(splitSize);
                lastDrawSplit = splitSize;
            }
            else
            {
                if (lastDrawSplit != -1)
                {
                    DrawSplitHelper(lastDrawSplit);
                }
                lastDrawSplit = -1;
            }
        }

        /// <summary>
        ///  Calculates the bounding rect of the split line. minWeight refers
        ///  to the minimum height or width of the splitline.
        /// </summary>
        private Rectangle CalcSplitLine(int splitSize, int minWeight)
        {
            Rectangle r = Bounds;
            Rectangle bounds = splitTarget.Bounds;
            switch (Dock)
            {
                case DockStyle.Top:
                    if (r.Height < minWeight)
                    {
                        r.Height = minWeight;
                    }

                    r.Y = bounds.Y + splitSize;
                    break;
                case DockStyle.Bottom:
                    if (r.Height < minWeight)
                    {
                        r.Height = minWeight;
                    }

                    r.Y = bounds.Y + bounds.Height - splitSize - r.Height;
                    break;
                case DockStyle.Left:
                    if (r.Width < minWeight)
                    {
                        r.Width = minWeight;
                    }

                    r.X = bounds.X + splitSize;
                    break;
                case DockStyle.Right:
                    if (r.Width < minWeight)
                    {
                        r.Width = minWeight;
                    }

                    r.X = bounds.X + bounds.Width - splitSize - r.Width;
                    break;
            }
            return r;
        }

        /// <summary>
        ///  Calculates the current size of the splitter-target.
        /// </summary>
        private int CalcSplitSize()
        {
            Control target = FindTarget();
            if (target == null)
            {
                return -1;
            }

            Rectangle r = target.Bounds;
            switch (Dock)
            {
                case DockStyle.Top:
                case DockStyle.Bottom:
                    return r.Height;
                case DockStyle.Left:
                case DockStyle.Right:
                    return r.Width;
                default:
                    return -1; // belts & braces
            }
        }

        /// <summary>
        ///  Calculates the bounding criteria for the splitter.
        /// </summary>
        private SplitData CalcSplitBounds()
        {
            SplitData spd = new SplitData();
            Control target = FindTarget();
            spd.target = target;
            if (target != null)
            {
                switch (target.Dock)
                {
                    case DockStyle.Left:
                    case DockStyle.Right:
                        initTargetSize = target.Bounds.Width;
                        break;
                    case DockStyle.Top:
                    case DockStyle.Bottom:
                        initTargetSize = target.Bounds.Height;
                        break;
                }
                Control parent = ParentInternal;
                ControlCollection children = parent.Controls;
                int count = children.Count;
                int dockWidth = 0, dockHeight = 0;
                for (int i = 0; i < count; i++)
                {
                    Control ctl = children[i];
                    if (ctl != target)
                    {
                        switch (((Control)ctl).Dock)
                        {
                            case DockStyle.Left:
                            case DockStyle.Right:
                                dockWidth += ctl.Width;
                                break;
                            case DockStyle.Top:
                            case DockStyle.Bottom:
                                dockHeight += ctl.Height;
                                break;
                        }
                    }
                }
                Size clientSize = parent.ClientSize;
                if (Horizontal)
                {
                    maxSize = clientSize.Width - dockWidth - minExtra;
                }
                else
                {
                    maxSize = clientSize.Height - dockHeight - minExtra;
                }
                spd.dockWidth = dockWidth;
                spd.dockHeight = dockHeight;
            }
            return spd;
        }

        /// <summary>
        ///  Draws the splitter line at the requested location. Should only be called
        ///  by drawSpltBar.
        /// </summary>
        private void DrawSplitHelper(int splitSize)
        {
            if (splitTarget == null)
            {
                return;
            }

            Rectangle r = CalcSplitLine(splitSize, 3);
            IntPtr parentHandle = ParentInternal.Handle;
            IntPtr dc = UnsafeNativeMethods.GetDCEx(new HandleRef(ParentInternal, parentHandle), NativeMethods.NullHandleRef, NativeMethods.DCX_CACHE | NativeMethods.DCX_LOCKWINDOWUPDATE);
            IntPtr halftone = ControlPaint.CreateHalftoneHBRUSH();
            IntPtr saveBrush = Gdi32.SelectObject(dc, halftone);
            SafeNativeMethods.PatBlt(new HandleRef(ParentInternal, dc), r.X, r.Y, r.Width, r.Height, NativeMethods.PATINVERT);
            Gdi32.SelectObject(dc, saveBrush);
            Gdi32.DeleteObject(halftone);
            User32.ReleaseDC(new HandleRef(ParentInternal, parentHandle), dc);
        }

        /// <summary>
        ///  Finds the target of the splitter. The target of the splitter is the
        ///  control that is "outside" or the splitter. For example, if the splitter
        ///  is docked left, the target is the control that is just to the left
        ///  of the splitter.
        /// </summary>
        private Control FindTarget()
        {
            Control parent = ParentInternal;
            if (parent == null)
            {
                return null;
            }

            ControlCollection children = parent.Controls;
            int count = children.Count;
            DockStyle dock = Dock;
            for (int i = 0; i < count; i++)
            {
                Control target = children[i];
                if (target != this)
                {
                    switch (dock)
                    {
                        case DockStyle.Top:
                            if (target.Bottom == Top)
                            {
                                return (Control)target;
                            }

                            break;
                        case DockStyle.Bottom:
                            if (target.Top == Bottom)
                            {
                                return (Control)target;
                            }

                            break;
                        case DockStyle.Left:
                            if (target.Right == Left)
                            {
                                return (Control)target;
                            }

                            break;
                        case DockStyle.Right:
                            if (target.Left == Right)
                            {
                                return (Control)target;
                            }

                            break;
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///  Calculates the split size based on the mouse position (x, y).
        /// </summary>
        private int GetSplitSize(int x, int y)
        {
            int delta;
            if (Horizontal)
            {
                delta = x - anchor.X;
            }
            else
            {
                delta = y - anchor.Y;
            }
            int size = 0;
            switch (Dock)
            {
                case DockStyle.Top:
                    size = splitTarget.Height + delta;
                    break;
                case DockStyle.Bottom:
                    size = splitTarget.Height - delta;
                    break;
                case DockStyle.Left:
                    size = splitTarget.Width + delta;
                    break;
                case DockStyle.Right:
                    size = splitTarget.Width - delta;
                    break;
            }
            return Math.Max(Math.Min(size, maxSize), minSize);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (splitTarget != null && e.KeyCode == Keys.Escape)
            {
                SplitEnd(false);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                SplitBegin(e.X, e.Y);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (splitTarget != null)
            {
                int x = e.X + Left;
                int y = e.Y + Top;
                Rectangle r = CalcSplitLine(GetSplitSize(e.X, e.Y), 0);
                int xSplit = r.X;
                int ySplit = r.Y;
                OnSplitterMoving(new SplitterEventArgs(x, y, xSplit, ySplit));
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (splitTarget != null)
            {
                int x = e.X + Left;
                int y = e.Y + Top;
                Rectangle r = CalcSplitLine(GetSplitSize(e.X, e.Y), 0);
                int xSplit = r.X;
                int ySplit = r.Y;
                SplitEnd(true);
            }
        }

        /// <summary>
        ///  Inherriting classes should override this method to respond to the
        ///  splitterMoving event. This event occurs while the splitter is
        ///  being moved by the user.
        /// </summary>
        protected virtual void OnSplitterMoving(SplitterEventArgs sevent)
        {
            ((SplitterEventHandler)Events[EVENT_MOVING])?.Invoke(this, sevent);

            if (splitTarget != null)
            {
                SplitMove(sevent.SplitX, sevent.SplitY);
            }
        }

        /// <summary>
        ///  Inherriting classes should override this method to respond to the
        ///  splitterMoved event. This event occurs when the user finishes
        ///  moving the splitter.
        /// </summary>
        protected virtual void OnSplitterMoved(SplitterEventArgs sevent)
        {
            ((SplitterEventHandler)Events[EVENT_MOVED])?.Invoke(this, sevent);

            if (splitTarget != null)
            {
                SplitMove(sevent.SplitX, sevent.SplitY);
            }
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (Horizontal)
            {
                if (width < 1)
                {
                    width = 3;
                }
                splitterThickness = width;
            }
            else
            {
                if (height < 1)
                {
                    height = 3;
                }
                splitterThickness = height;
            }
            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <summary>
        ///  Begins the splitter moving.
        /// </summary>
        private void SplitBegin(int x, int y)
        {
            SplitData spd = CalcSplitBounds();
            if (spd.target != null && (minSize < maxSize))
            {
                anchor = new Point(x, y);
                splitTarget = spd.target;
                splitSize = GetSplitSize(x, y);

                if (splitterMessageFilter != null)
                {
                    splitterMessageFilter = new SplitterMessageFilter(this);
                }
                Application.AddMessageFilter(splitterMessageFilter);

                CaptureInternal = true;
                DrawSplitBar(DRAW_START);
            }
        }

        /// <summary>
        ///  Finishes the split movement.
        /// </summary>
        private void SplitEnd(bool accept)
        {
            DrawSplitBar(DRAW_END);
            splitTarget = null;
            CaptureInternal = false;
            if (splitterMessageFilter != null)
            {
                Application.RemoveMessageFilter(splitterMessageFilter);
                splitterMessageFilter = null;
            }

            if (accept)
            {
                ApplySplitPosition();
            }
            else if (splitSize != initTargetSize)
            {
                SplitPosition = initTargetSize;
            }
            anchor = Point.Empty;
        }

        /// <summary>
        ///  Sets the split position to be the current split size. This is called
        ///  by splitEdit
        /// </summary>
        private void ApplySplitPosition()
        {
            SplitPosition = splitSize;
        }

        /// <summary>
        ///  Moves the splitter line to the splitSize for the mouse position
        ///  (x, y).
        /// </summary>
        private void SplitMove(int x, int y)
        {
            int size = GetSplitSize(x - Left + anchor.X, y - Top + anchor.Y);
            if (splitSize != size)
            {
                splitSize = size;
                DrawSplitBar(DRAW_MOVE);
            }
        }

        /// <summary>
        ///  Returns a string representation for this control.
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();
            return s + ", MinExtra: " + MinExtra.ToString(CultureInfo.CurrentCulture) + ", MinSize: " + MinSize.ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///  Return value holder...
        /// </summary>
        private class SplitData
        {
            public int dockWidth = -1;
            public int dockHeight = -1;
            internal Control target;
        }

        private class SplitterMessageFilter : IMessageFilter
        {
            private readonly Splitter owner = null;

            public SplitterMessageFilter(Splitter splitter)
            {
                owner = splitter;
            }

            /// <summary>
            /// </summary>
            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg >= WindowMessages.WM_KEYFIRST && m.Msg <= WindowMessages.WM_KEYLAST)
                {
                    if (m.Msg == WindowMessages.WM_KEYDOWN && unchecked((int)(long)m.WParam) == (int)Keys.Escape)
                    {
                        owner.SplitEnd(false);
                    }
                    return true;
                }
                return false;
            }
        }
    }
}


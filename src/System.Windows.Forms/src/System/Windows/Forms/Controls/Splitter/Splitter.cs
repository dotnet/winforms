// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
///  Provides user resizing of docked elements at run time. To use a Splitter you can
///  dock any control to an edge of a container, and then dock the splitter to the same
///  edge. The splitter will then resize the control that is previous in the docking
///  order.
/// </summary>
[DefaultEvent(nameof(SplitterMoved))]
[DefaultProperty(nameof(Dock))]
[SRDescription(nameof(SR.DescriptionSplitter))]
[Designer($"System.Windows.Forms.Design.SplitterDesigner, {AssemblyRef.SystemDesign}")]
public partial class Splitter : Control
{
    private const int DRAW_START = 1;
    private const int DRAW_MOVE = 2;
    private const int DRAW_END = 3;

    private const int DefaultWidth = 3;

    private BorderStyle _borderStyle = BorderStyle.None;
    private int _minSize = 25;
    private int _minExtra = 25;
    private Point _anchor = Point.Empty;
    private Control? _splitTarget;
    private int _splitSize = -1;
    private int _splitterThickness = 3;
    private int _initTargetSize;
    private int _lastDrawSplit = -1;
    private int _maxSize;
    private static readonly object s_movingEvent = new();
    private static readonly object s_movedEvent = new();

    // Cannot expose IMessageFilter.PreFilterMessage through this unsealed class
    private SplitterMessageFilter? _splitterMessageFilter;

    /// <summary>
    ///  Creates a new Splitter.
    /// </summary>
    public Splitter() : base()
    {
        SetStyle(ControlStyles.Selectable, false);
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        SetStyle(ControlStyles.ApplyThemingImplicitly, true);
#pragma warning restore WFO5001

        TabStop = false;
        _minSize = 25;
        _minExtra = 25;

        Dock = DockStyle.Left;
    }

    /// <summary>
    ///  The current value of the anchor property. The anchor property
    ///  determines which edges of the control are anchored to the container's
    ///  edges.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DefaultValue(AnchorStyles.None)]
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

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool AllowDrop
    {
        get => base.AllowDrop;
        set => base.AllowDrop = value;
    }

    /// <summary>
    ///  Deriving classes can override this to configure a default size for their control.
    ///  This is more efficient than setting the size in the control's constructor.
    /// </summary>
    protected override Size DefaultSize
    {
        get
        {
            return new Size(DefaultWidth, DefaultWidth);
        }
    }

    protected override Cursor DefaultCursor => Dock switch
    {
        DockStyle.Top or DockStyle.Bottom => Cursors.HSplit,
        DockStyle.Left or DockStyle.Right => Cursors.VSplit,
        _ => base.DefaultCursor,
    };

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color ForeColor
    {
        get => base.ForeColor;
        set => base.ForeColor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? ForeColorChanged
    {
        add => base.ForeColorChanged += value;
        remove => base.ForeColorChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Image? BackgroundImage
    {
        get => base.BackgroundImage;
        set => base.BackgroundImage = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageChanged
    {
        add => base.BackgroundImageChanged += value;
        remove => base.BackgroundImageChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override ImageLayout BackgroundImageLayout
    {
        get => base.BackgroundImageLayout;
        set => base.BackgroundImageLayout = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageLayoutChanged
    {
        add => base.BackgroundImageLayoutChanged += value;
        remove => base.BackgroundImageLayoutChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AllowNull]
    public override Font Font
    {
        get => base.Font;
        set => base.Font = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? FontChanged
    {
        add => base.FontChanged += value;
        remove => base.FontChanged -= value;
    }

    /// <summary>
    ///  Indicates what type of border the Splitter control has. This value
    ///  comes from the System.Windows.Forms.BorderStyle enumeration.
    /// </summary>
    [DefaultValue(BorderStyle.None)]
    [SRCategory(nameof(SR.CatAppearance))]
    [DispId(PInvokeCore.DISPID_BORDERSTYLE)]
    [SRDescription(nameof(SR.SplitterBorderStyleDescr))]
    public BorderStyle BorderStyle
    {
        get => _borderStyle;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_borderStyle != value)
            {
                _borderStyle = value;
                UpdateStyles();
            }
        }
    }

    protected override AccessibleObject CreateAccessibilityInstance()
        => new SplitterAccessibleObject(this);

    /// <summary>
    ///  Returns the parameters needed to create the handle. Inheriting classes
    ///  can override this to provide extra functionality. They should not,
    ///  however, forget to call base.getCreateParams() first to get the struct
    ///  filled up with the basic info.
    /// </summary>
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.Style &= ~(int)WINDOW_STYLE.WS_BORDER;
            cp.ExStyle &= ~(int)WINDOW_EX_STYLE.WS_EX_CLIENTEDGE;

            switch (_borderStyle)
            {
                case BorderStyle.Fixed3D:
                    cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_CLIENTEDGE;
                    break;
                case BorderStyle.FixedSingle:
                    cp.Style |= (int)WINDOW_STYLE.WS_BORDER;
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

    [Localizable(true)]
    [DefaultValue(DockStyle.Left)]
    public override DockStyle Dock
    {
        get => base.Dock;

        set
        {
            if (value is not (DockStyle.Top or DockStyle.Bottom or DockStyle.Left or DockStyle.Right))
            {
                throw new ArgumentException(SR.SplitterInvalidDockEnum);
            }

            int requestedSize = _splitterThickness;

            base.Dock = value;
            switch (Dock)
            {
                case DockStyle.Top:
                case DockStyle.Bottom:
                    if (_splitterThickness != -1)
                    {
                        Height = requestedSize;
                    }

                    break;
                case DockStyle.Left:
                case DockStyle.Right:
                    if (_splitterThickness != -1)
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
            return dock is DockStyle.Left or DockStyle.Right;
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new ImeMode ImeMode
    {
        get => base.ImeMode;
        set => base.ImeMode = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? ImeModeChanged
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
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [DefaultValue(25)]
    [SRDescription(nameof(SR.SplitterMinExtraDescr))]
    public int MinExtra
    {
        get
        {
            return _minExtra;
        }
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            _minExtra = value;
        }
    }

    /// <summary>
    ///  The minSize is the minimum size (in pixels) of the target of the
    ///  splitter. The target of a splitter is always the control adjacent
    ///  to the splitter, just prior in the dock order.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [DefaultValue(25)]
    [SRDescription(nameof(SR.SplitterMinSizeDescr))]
    public int MinSize
    {
        get
        {
            return _minSize;
        }
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            _minSize = value;
        }
    }

    /// <summary>
    ///  The position of the splitter. If the splitter is not bound
    ///  to a control, SplitPosition will be -1.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.SplitterSplitPositionDescr))]
    public int SplitPosition
    {
        get
        {
            if (_splitSize == -1)
            {
                _splitSize = CalcSplitSize();
            }

            return _splitSize;
        }
        set
        {
            // calculate maxSize and other bounding conditions
            SplitData spd = CalcSplitBounds();

            // this is not an else-if to handle the maxSize < minSize case...
            // ie. we give minSize priority over maxSize...
            if (value > _maxSize)
            {
                value = _maxSize;
            }

            if (value < _minSize)
            {
                value = _minSize;
            }

            _splitSize = value;
            DrawSplitBar(DRAW_END);

            if (spd._target is null)
            {
                _splitSize = -1;
                return;
            }

            Rectangle bounds = spd._target.Bounds;
            switch (Dock)
            {
                case DockStyle.Top:
                    bounds.Height = value;
                    break;
                case DockStyle.Bottom:
                    bounds.Y += bounds.Height - _splitSize;
                    bounds.Height = value;
                    break;
                case DockStyle.Left:
                    bounds.Width = value;
                    break;
                case DockStyle.Right:
                    bounds.X += bounds.Width - _splitSize;
                    bounds.Width = value;
                    break;
            }

            spd._target.Bounds = bounds;
            Application.DoEvents();
            OnSplitterMoved(new SplitterEventArgs(Left, Top, (Left + bounds.Width / 2), (Top + bounds.Height / 2)));
        }
    }

    internal override bool SupportsUiaProviders => true;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool TabStop
    {
        get => base.TabStop;
        set => base.TabStop = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TabStopChanged
    {
        add => base.TabStopChanged += value;
        remove => base.TabStopChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Bindable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TextChanged
    {
        add => base.TextChanged += value;
        remove => base.TextChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? Enter
    {
        add => base.Enter += value;
        remove => base.Enter -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyEventHandler? KeyUp
    {
        add => base.KeyUp += value;
        remove => base.KeyUp -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyEventHandler? KeyDown
    {
        add => base.KeyDown += value;
        remove => base.KeyDown -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyPressEventHandler? KeyPress
    {
        add => base.KeyPress += value;
        remove => base.KeyPress -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? Leave
    {
        add => base.Leave += value;
        remove => base.Leave -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.SplitterSplitterMovingDescr))]
    public event SplitterEventHandler? SplitterMoving
    {
        add => Events.AddHandler(s_movingEvent, value);
        remove => Events.RemoveHandler(s_movingEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.SplitterSplitterMovedDescr))]
    public event SplitterEventHandler? SplitterMoved
    {
        add => Events.AddHandler(s_movedEvent, value);
        remove => Events.RemoveHandler(s_movedEvent, value);
    }

    /// <summary>
    ///  Draws the splitter bar at the current location. Will automatically
    ///  cleanup anyplace the splitter was drawn previously.
    /// </summary>
    private void DrawSplitBar(int mode)
    {
        if (mode != DRAW_START && _lastDrawSplit != -1)
        {
            DrawSplitHelper(_lastDrawSplit);
            _lastDrawSplit = -1;
        }

        // Bail if drawing with no old point...
        //
        else if (mode != DRAW_START && _lastDrawSplit == -1)
        {
            return;
        }

        if (mode != DRAW_END)
        {
            DrawSplitHelper(_splitSize);
            _lastDrawSplit = _splitSize;
        }
        else
        {
            if (_lastDrawSplit != -1)
            {
                DrawSplitHelper(_lastDrawSplit);
            }

            _lastDrawSplit = -1;
        }
    }

    /// <summary>
    ///  Calculates the bounding rect of the split line. minWeight refers
    ///  to the minimum height or width of the splitline.
    /// </summary>
    private Rectangle CalcSplitLine(Control splitTarget, int splitSize, int minWeight)
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
        Control? target = FindTarget();
        if (target is null)
        {
            return -1;
        }

        Rectangle r = target.Bounds;
        return Dock switch
        {
            DockStyle.Top or DockStyle.Bottom => r.Height,
            DockStyle.Left or DockStyle.Right => r.Width,
            _ => -1, // belts & braces
        };
    }

    /// <summary>
    ///  Calculates the bounding criteria for the splitter.
    /// </summary>
    private SplitData CalcSplitBounds()
    {
        SplitData spd = new();
        Control? target = FindTarget();
        spd._target = target;
        if (target is not null)
        {
            switch (target.Dock)
            {
                case DockStyle.Left:
                case DockStyle.Right:
                    _initTargetSize = target.Bounds.Width;
                    break;
                case DockStyle.Top:
                case DockStyle.Bottom:
                    _initTargetSize = target.Bounds.Height;
                    break;
            }

            Control? parent = ParentInternal;
            if (parent is null)
            {
                return spd;
            }

            ControlCollection children = parent.Controls;
            int count = children.Count;
            int dockWidth = 0, dockHeight = 0;
            for (int i = 0; i < count; i++)
            {
                Control ctl = children[i];
                if (ctl != target)
                {
                    switch (ctl.Dock)
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
                _maxSize = clientSize.Width - dockWidth - _minExtra;
            }
            else
            {
                _maxSize = clientSize.Height - dockHeight - _minExtra;
            }

            spd.dockWidth = dockWidth;
            spd.dockHeight = dockHeight;
        }

        return spd;
    }

    /// <summary>
    ///  Draws the splitter line at the requested location. Should only be called
    ///  by drawSplitBar.
    /// </summary>
    private void DrawSplitHelper(int splitSize)
    {
        if (_splitTarget is null || ParentInternal is null)
        {
            return;
        }

        Rectangle r = CalcSplitLine(_splitTarget, splitSize, 3);
        using GetDcScope dc = new(ParentInternal.HWND, HRGN.Null, GET_DCX_FLAGS.DCX_CACHE | GET_DCX_FLAGS.DCX_LOCKWINDOWUPDATE);
        HBRUSH halftone = ControlPaint.CreateHalftoneHBRUSH();
        using ObjectScope halftoneScope = new(halftone);
        using SelectObjectScope selection = new(dc, halftone);
        PInvoke.PatBlt(dc, r.X, r.Y, r.Width, r.Height, ROP_CODE.PATINVERT);

        GC.KeepAlive(ParentInternal);
    }

    /// <summary>
    ///  Finds the target of the splitter. The target of the splitter is the
    ///  control that is "outside" or the splitter. For example, if the splitter
    ///  is docked left, the target is the control that is just to the left
    ///  of the splitter.
    /// </summary>
    private Control? FindTarget()
    {
        Control? parent = ParentInternal;
        if (parent is null)
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
                            return target;
                        }

                        break;
                    case DockStyle.Bottom:
                        if (target.Top == Bottom)
                        {
                            return target;
                        }

                        break;
                    case DockStyle.Left:
                        if (target.Right == Left)
                        {
                            return target;
                        }

                        break;
                    case DockStyle.Right:
                        if (target.Left == Right)
                        {
                            return target;
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
    private int GetSplitSize(Control splitTarget, int x, int y)
    {
        int delta;
        if (Horizontal)
        {
            delta = x - _anchor.X;
        }
        else
        {
            delta = y - _anchor.Y;
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

        return Math.Max(Math.Min(size, _maxSize), _minSize);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (_splitTarget is not null && e.KeyCode == Keys.Escape)
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
        if (_splitTarget is not null)
        {
            int x = e.X + Left;
            int y = e.Y + Top;
            Rectangle r = CalcSplitLine(_splitTarget, GetSplitSize(_splitTarget, e.X, e.Y), 0);
            int xSplit = r.X;
            int ySplit = r.Y;
            OnSplitterMoving(new SplitterEventArgs(x, y, xSplit, ySplit));
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        SplitEnd(true);
    }

    /// <summary>
    ///  Inheriting classes should override this method to respond to the
    ///  splitterMoving event. This event occurs while the splitter is
    ///  being moved by the user.
    /// </summary>
    protected virtual void OnSplitterMoving(SplitterEventArgs sevent)
    {
        ((SplitterEventHandler?)Events[s_movingEvent])?.Invoke(this, sevent);

        if (_splitTarget is not null)
        {
            SplitMove(_splitTarget, sevent.SplitX, sevent.SplitY);
        }
    }

    /// <summary>
    ///  Inheriting classes should override this method to respond to the
    ///  splitterMoved event. This event occurs when the user finishes
    ///  moving the splitter.
    /// </summary>
    protected virtual void OnSplitterMoved(SplitterEventArgs sevent)
    {
        ((SplitterEventHandler?)Events[s_movedEvent])?.Invoke(this, sevent);

        if (_splitTarget is not null)
        {
            SplitMove(_splitTarget, sevent.SplitX, sevent.SplitY);
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

            _splitterThickness = width;
        }
        else
        {
            if (height < 1)
            {
                height = 3;
            }

            _splitterThickness = height;
        }

        base.SetBoundsCore(x, y, width, height, specified);
    }

    /// <summary>
    ///  Begins the splitter moving.
    /// </summary>
    private void SplitBegin(int x, int y)
    {
        SplitData spd = CalcSplitBounds();
        if (spd._target is not null && (_minSize < _maxSize))
        {
            _anchor = new Point(x, y);
            _splitTarget = spd._target;
            _splitSize = GetSplitSize(_splitTarget, x, y);

            if (_splitterMessageFilter is not null)
            {
                _splitterMessageFilter = new SplitterMessageFilter(this);
            }

            Application.AddMessageFilter(_splitterMessageFilter);

            Capture = true;
            DrawSplitBar(DRAW_START);
        }
    }

    /// <summary>
    ///  Finishes the split movement.
    /// </summary>
    private void SplitEnd(bool accept)
    {
        DrawSplitBar(DRAW_END);
        _splitTarget = null;
        Capture = false;
        if (_splitterMessageFilter is not null)
        {
            Application.RemoveMessageFilter(_splitterMessageFilter);
            _splitterMessageFilter = null;
        }

        if (accept)
        {
            ApplySplitPosition();
        }
        else if (_splitSize != _initTargetSize)
        {
            SplitPosition = _initTargetSize;
        }

        _anchor = Point.Empty;
    }

    /// <summary>
    ///  Sets the split position to be the current split size. This is called
    ///  by splitEdit
    /// </summary>
    private void ApplySplitPosition()
    {
        SplitPosition = _splitSize;
    }

    /// <summary>
    ///  Moves the splitter line to the splitSize for the mouse position
    ///  (x, y).
    /// </summary>
    private void SplitMove(Control splitTarget, int x, int y)
    {
        int size = GetSplitSize(splitTarget, x - Left + _anchor.X, y - Top + _anchor.Y);
        if (_splitSize != size)
        {
            _splitSize = size;
            DrawSplitBar(DRAW_MOVE);
        }
    }

    /// <summary>
    ///  Returns a string representation for this control.
    /// </summary>
    public override string ToString()
    {
        string s = base.ToString();
        return $"{s}, MinExtra: {MinExtra}, MinSize: {MinSize}";
    }
}

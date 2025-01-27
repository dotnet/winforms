// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

[Designer($"System.Windows.Forms.Design.ToolStripPanelDesigner, {AssemblyRef.SystemDesign}")]
[ToolboxBitmap(typeof(ToolStripPanel), "ToolStripPanel_standalone")]
public partial class ToolStripPanel : ContainerControl, IArrangedElement
{
    private Orientation _orientation = Orientation.Horizontal;
    private Padding _rowMargin;
    private ToolStripRendererSwitcher? _rendererSwitcher;
    private BitVector32 _state;
    private readonly ToolStripContainer? _owner;

    private static readonly int s_propToolStripPanelRowCollection = PropertyStore.CreateKey();

    private static readonly int s_stateLocked = BitVector32.CreateMask();
    private static readonly int s_stateBeginInit = BitVector32.CreateMask(s_stateLocked);
    private static readonly int s_stateChangingZOrder = BitVector32.CreateMask(s_stateBeginInit);
    private static readonly int s_stateInJoin = BitVector32.CreateMask(s_stateChangingZOrder);
    private static readonly int s_stateEndInit = BitVector32.CreateMask(s_stateInJoin);
    private static readonly int s_stateLayoutSuspended = BitVector32.CreateMask(s_stateEndInit);
    private static readonly int s_stateRightToLeftChanged = BitVector32.CreateMask(s_stateLayoutSuspended);

    internal static readonly Padding s_dragMargin = new(10);

    private static readonly object s_eventRendererChanged = new();

    public ToolStripPanel()
    {
        const int LogicalRowLeftMargin = 3;
        _rowMargin = new(ScaleHelper.ScaleToDpi(LogicalRowLeftMargin, ScaleHelper.InitialSystemDpi), 0, 0, 0);

        SuspendLayout();
        AutoScaleMode = AutoScaleMode.None;
        InitFlowLayout();
        AutoSize = true;
        MinimumSize = Size.Empty; // consider 1,1
        _state[s_stateLocked | s_stateBeginInit | s_stateChangingZOrder] = false;
        TabStop = false;

        ToolStripManager.ToolStripPanels.Add(this);
        // not setting ControlStyles.AllPaintingInWmPaint as we don't do any painting in OnPaint... all
        // is done in OnPaintBackground... so its better to show the rafting container in WM_ERASEBACKGROUND.
        SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | /*ControlStyles.AllPaintingInWmPaint |*/ControlStyles.SupportsTransparentBackColor, true);
        SetStyle(ControlStyles.Selectable, false);
        ResumeLayout(true);
    }

    internal ToolStripPanel(ToolStripContainer owner)
        : this()
    {
        _owner = owner;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override bool AllowDrop
    {
        get => base.AllowDrop;
        set => base.AllowDrop = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override bool AutoScroll
    {
        get => base.AutoScroll;
        set => base.AutoScroll = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Size AutoScrollMargin
    {
        get => base.AutoScrollMargin;
        set => base.AutoScrollMargin = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Size AutoScrollMinSize
    {
        get => base.AutoScrollMinSize;
        set => base.AutoScrollMinSize = value;
    }

    [DefaultValue(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public override bool AutoSize
    {
        get => base.AutoSize;
        set => base.AutoSize = value;
    }

    ///  Override base AutoSizeChanged to we can change visibility/browsability attributes
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public new event EventHandler? AutoSizeChanged
    {
        add => base.AutoSizeChanged += value;
        remove => base.AutoSizeChanged -= value;
    }

    protected override Padding DefaultPadding
    {
        get { return Padding.Empty; }
    }

    protected override Padding DefaultMargin
    {
        get { return Padding.Empty; }
    }

    public Padding RowMargin
    {
        get { return _rowMargin; }
        set
        {
            _rowMargin = value;
            LayoutTransaction.DoLayout(this, this, "RowMargin");
        }
    }

    public override DockStyle Dock
    {
        get => base.Dock;
        set
        {
            base.Dock = value;

            if (value is DockStyle.Left or DockStyle.Right)
            {
                Orientation = Orientation.Vertical;
            }
            else
            {
                Orientation = Orientation.Horizontal;
            }
        }
    }

    internal Rectangle DragBounds
    {
        get
        {
            return LayoutUtils.InflateRect(ClientRectangle, s_dragMargin);
        }
    }

    internal bool IsInDesignMode
    {
        get
        {
            return DesignMode;
        }
    }

    public override LayoutEngine LayoutEngine
    {
        get
        {
            return FlowLayout.Instance;
        }
    }

    [DefaultValue(false)]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool Locked
    {
        get
        {
            return _state[s_stateLocked];
        }
        set
        {
            _state[s_stateLocked] = value;
        }
    }

    public Orientation Orientation
    {
        get
        {
            return _orientation;
        }
        set
        {
            if (_orientation != value)
            {
                _orientation = value;
                _rowMargin = LayoutUtils.FlipPadding(_rowMargin);
                InitFlowLayout();
                foreach (ToolStripPanelRow row in RowsInternal)
                {
                    row.OnOrientationChanged();
                }
            }
        }
    }

    private ToolStripRendererSwitcher RendererSwitcher
    {
        get
        {
            if (_rendererSwitcher is null)
            {
                _rendererSwitcher = new ToolStripRendererSwitcher(this);
                HandleRendererChanged(this, EventArgs.Empty);
                _rendererSwitcher.RendererChanged += HandleRendererChanged;
            }

            return _rendererSwitcher;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ToolStripRenderer Renderer
    {
        get
        {
            return RendererSwitcher.Renderer;
        }
        set
        {
            RendererSwitcher.Renderer = value;
        }
    }

    [SRDescription(nameof(SR.ToolStripRenderModeDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    public ToolStripRenderMode RenderMode
    {
        get
        {
            return RendererSwitcher.RenderMode;
        }
        set
        {
            RendererSwitcher.RenderMode = value;
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripRendererChanged))]
    public event EventHandler? RendererChanged
    {
        add => Events.AddHandler(s_eventRendererChanged, value);
        remove => Events.RemoveHandler(s_eventRendererChanged, value);
    }

    /// <summary>
    ///  Collection of child controls.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [SRDescription(nameof(SR.ToolStripPanelRowsDescr))]
    internal ToolStripPanelRowCollection RowsInternal
    {
        get
        {
            if (!Properties.TryGetValue(s_propToolStripPanelRowCollection, out ToolStripPanelRowCollection? rowCollection))
            {
                rowCollection = Properties.AddValue(s_propToolStripPanelRowCollection, CreateToolStripPanelRowCollection());
            }

            return rowCollection;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ToolStripPanelRowsDescr))]
    public ToolStripPanelRow[] Rows
    {
        get
        {
            ToolStripPanelRow[] rows = new ToolStripPanelRow[RowsInternal.Count];
            RowsInternal.CopyTo(rows, 0);
            return rows;
        }
    }

    internal override bool SupportsUiaProviders => true;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new int TabIndex
    {
        get => base.TabIndex;
        set => base.TabIndex = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TabIndexChanged
    {
        add => base.TabIndexChanged += value;
        remove => base.TabIndexChanged -= value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool TabStop
    {
        get => base.TabStop;
        set
        {
            SetStyle(ControlStyles.Selectable, value);

            base.TabStop = value;
        }
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

    #region ISupportInitialize

    public void BeginInit()
    {
        _state[s_stateBeginInit] = true;
    }

    public void EndInit()
    {
        _state[s_stateBeginInit] = false;
        _state[s_stateEndInit] = true;
        try
        {
            if (!_state[s_stateInJoin])
            {
                JoinControls();
            }
        }
        finally
        {
            _state[s_stateEndInit] = false;
        }
    }

    #endregion ISupportInitialize

    private ToolStripPanelRowCollection CreateToolStripPanelRowCollection() => new(this);

    protected override AccessibleObject CreateAccessibilityInstance() => new ToolStripPanelAccessibleObject(this);

    protected override ControlCollection CreateControlsInstance() => new ToolStripPanelControlCollection(this);

    /// <summary>
    ///  Disposes of the resources (other than memory) used by
    ///  the <see cref="ContainerControl"/>
    ///  .
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ToolStripManager.ToolStripPanels.Remove(this);
        }

        base.Dispose(disposing);
    }

    private void InitFlowLayout()
    {
        if (Orientation == Orientation.Horizontal)
        {
            FlowLayout.SetFlowDirection(this, FlowDirection.TopDown);
        }
        else
        {
            FlowLayout.SetFlowDirection(this, FlowDirection.LeftToRight);
        }

        FlowLayout.SetWrapContents(this, false);
    }

    private Point GetStartLocation(ToolStrip toolStripToDrag)
    {
        if (toolStripToDrag.IsCurrentlyDragging
            && Orientation == Orientation.Horizontal
            && toolStripToDrag.RightToLeft == RightToLeft.Yes)
        {
            // the grip is on the right side, not left.
            return new Point(toolStripToDrag.Right, toolStripToDrag.Top);
        }

        return toolStripToDrag.Location;
    }

    private void HandleRendererChanged(object? sender, EventArgs e)
    {
        OnRendererChanged(e);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnPaintBackground(PaintEventArgs e)
    {
        ToolStripPanelRenderEventArgs rea = new(e.Graphics, this);
        Renderer.DrawToolStripPanelBackground(rea);

        if (!rea.Handled)
        {
            base.OnPaintBackground(e);
        }
    }

    protected override void OnControlAdded(ControlEventArgs e)
    {
        base.OnControlAdded(e);

        if (!_state[s_stateBeginInit] && !_state[s_stateInJoin])
        {
            if (!_state[s_stateLayoutSuspended])
            {
                if (e.Control is not null)
                {
                    var toolStrip = e.Control as ToolStrip;
                    if (toolStrip is not null)
                    {
                        Join(toolStrip, e.Control.Location);
                    }
                }
            }
            else
            {
                BeginInit();
            }
        }
    }

    protected override void OnControlRemoved(ControlEventArgs e)
    {
        if (e.Control is ISupportToolStripPanel controlToBeDragged)
        {
            controlToBeDragged.ToolStripPanelRow?.ControlsInternal.Remove(e.Control);
        }

        base.OnControlRemoved(e);
    }

    protected override void OnLayout(LayoutEventArgs e)
    {
        if (e.AffectedComponent != ParentInternal && e.AffectedComponent as Control is not null)
        {
            if (e.AffectedComponent is ISupportToolStripPanel draggedControl && RowsInternal.Contains(draggedControl.ToolStripPanelRow!))
            {
                // there's a problem in the base onlayout... if toolstrip needs more space it talks to us
                // not the row that needs layout.
                LayoutTransaction.DoLayout(draggedControl.ToolStripPanelRow, e.AffectedComponent as IArrangedElement, e.AffectedProperty);
            }
        }

        base.OnLayout(e);
    }

    internal override void OnLayoutSuspended()
    {
        base.OnLayoutSuspended();
        _state[s_stateLayoutSuspended] = true;
    }

    internal override void OnLayoutResuming(bool resumeLayout)
    {
        base.OnLayoutResuming(resumeLayout);
        _state[s_stateLayoutSuspended] = false;
        if (_state[s_stateBeginInit])
        {
            EndInit();
        }
    }

    protected override void OnRightToLeftChanged(EventArgs e)
    {
        base.OnRightToLeftChanged(e);

        if (!_state[s_stateBeginInit])
        {
            if (Controls.Count > 0)
            {
                // rejoin the controls on the other side of the toolstrippanel.
                SuspendLayout();
                Control[] controls = new Control[Controls.Count];
                Point[] controlLocations = new Point[Controls.Count];
                int j = 0;
                foreach (ToolStripPanelRow row in RowsInternal)
                {
                    foreach (Control control in row.ControlsInternal)
                    {
                        controls[j] = control;
                        controlLocations[j] = new Point(row.Bounds.Width - control.Right, control.Top);
                        j++;
                    }
                }

                Controls.Clear();

                for (int i = 0; i < controls.Length; i++)
                {
                    var toolStrip = controls[i] as ToolStrip;
                    if (toolStrip is not null)
                    {
                        Join(toolStrip, controlLocations[i]);
                    }
                }

                ResumeLayout(true);
            }
        }
        else
        {
            _state[s_stateRightToLeftChanged] = true;
        }
    }

    protected virtual void OnRendererChanged(EventArgs e)
    {
        Renderer.InitializePanel(this);

        Invalidate();

        ((EventHandler?)Events[s_eventRendererChanged])?.Invoke(this, e);
    }

    /// <summary>
    ///  We want to Set ToolStripPanel at DesignTime when the ToolStripPanel is added to the Form,
    /// </summary>
    protected override void OnParentChanged(EventArgs e)
    {
        PerformUpdate();
        base.OnParentChanged(e);
    }

    protected override void OnDockChanged(EventArgs e)
    {
        PerformUpdate();
        base.OnDockChanged(e);
    }

    internal void PerformUpdate()
    {
        PerformUpdate(false);
    }

    internal void PerformUpdate(bool forceLayout)
    {
        if (!_state[s_stateBeginInit] && !_state[s_stateInJoin])
        {
            JoinControls(forceLayout);
        }
    }

    private void ResetRenderMode()
    {
        RendererSwitcher.ResetRenderMode();
    }

    private bool ShouldSerializeRenderMode()
    {
        return RendererSwitcher.ShouldSerializeRenderMode();
    }

    private bool ShouldSerializeDock()
    {
        return _owner is null && (Dock != DockStyle.None);
    }

    private void JoinControls()
    {
        JoinControls(false);
    }

    private void JoinControls(bool forceLayout)
    {
        // undone: config - shift to other container
        ToolStripPanelControlCollection? controls = Controls as ToolStripPanelControlCollection;
        if (controls is not null && controls.Count > 0)
        {
            controls.Sort();

            // since Join is going to mess with our order - make a copy. (ugh).
            Control[] controlArray = new Control[controls.Count];
            controls.CopyTo(controlArray, 0);
            for (int i = 0; i < controlArray.Length; i++)
            {
                int numRows = RowsInternal.Count;

                if (controlArray[i] is ISupportToolStripPanel draggedControl
                    && draggedControl.ToolStripPanelRow is not null
                    && !draggedControl.IsCurrentlyDragging)
                {
                    ToolStripPanelRow row = draggedControl.ToolStripPanelRow;
                    if (row.Bounds.Contains(controlArray[i].Location))
                    {
                        // this toolstrip does not require join.
                        continue;
                    }
                }

                if (controlArray[i].AutoSize)
                {
                    controlArray[i].Size = controlArray[i].PreferredSize;
                }

                var toolStrip = controlArray[i] as ToolStrip;
                if (toolStrip is not null)
                {
                    Join(toolStrip, controlArray[i].Location);
                }

                if (numRows < RowsInternal.Count || forceLayout)
                {
                    // OK this is weird but here we're in the midst of a suspend layout.
                    // the only way we can deterministically place these guys is to force a layout
                    // each time we've added a new row. Otherwise we won't find the correct
                    // row to add the control to (PointToRow will fail as Row.Bounds isn't set yet)
                    OnLayout(new LayoutEventArgs(this, PropertyNames.Rows));
                }
            }
        }

        _state[s_stateRightToLeftChanged] = false;
    }

    [ThreadStatic]
    private static FeedbackRectangle? t_feedbackRect;

    private void GiveToolStripPanelFeedback(ToolStrip toolStripToDrag, Point screenLocation)
    {
        if (Orientation == Orientation.Horizontal && RightToLeft == RightToLeft.Yes)
        {
            // paint the feedback in the correct location when RTL.Yes
            screenLocation.Offset(-toolStripToDrag.Width, 0);
        }

        CurrentFeedbackRect ??= new FeedbackRectangle(toolStripToDrag.ClientRectangle);

        if (!CurrentFeedbackRect.Visible)
        {
            toolStripToDrag.SuspendCaptureMode();

            try
            {
                CurrentFeedbackRect.Show(screenLocation);
                toolStripToDrag.Capture = true;
            }
            finally
            {
                toolStripToDrag.ResumeCaptureMode();
            }
        }
        else
        {
            CurrentFeedbackRect.Move(screenLocation);
        }
    }

    internal static void ClearDragFeedback()
    {
        FeedbackRectangle? oldFeedback = t_feedbackRect;
        t_feedbackRect = null;
        oldFeedback?.Dispose();
    }

    private static FeedbackRectangle? CurrentFeedbackRect
    {
        get => t_feedbackRect;
        set => t_feedbackRect = value;
    }

    public void Join(ToolStrip toolStripToDrag)
    {
        Join(toolStripToDrag, Point.Empty);
    }

    public void Join(ToolStrip toolStripToDrag, int row)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(row);

        Rectangle dragRect;
        if (row >= RowsInternal.Count)
        {
            dragRect = DragBounds;
        }
        else
        {
            dragRect = RowsInternal[row].DragBounds;
        }

        Point location;
        if (Orientation == Orientation.Horizontal)
        {
            location = new Point(0, dragRect.Bottom - 1);
        }
        else
        {
            location = new Point(dragRect.Right - 1, 0);
        }

        Join(toolStripToDrag, location);
    }

    public void Join(ToolStrip toolStripToDrag, int x, int y)
    {
        Join(toolStripToDrag, new Point(x, y));
    }

    public void Join(ToolStrip toolStripToDrag, Point location)
    {
        ArgumentNullException.ThrowIfNull(toolStripToDrag);

        if (!_state[s_stateBeginInit] && !_state[s_stateInJoin])
        {
            try
            {
                _state[s_stateInJoin] = true;
                toolStripToDrag.ParentInternal = this;
                MoveInsideContainer(toolStripToDrag, location);
            }
            finally
            {
                _state[s_stateInJoin] = false;
            }
        }
        else
        {
            Controls.Add(toolStripToDrag);
            toolStripToDrag.Location = location;
        }
    }

    internal void MoveControl(ToolStrip? toolStripToDrag, Point screenLocation)
    {
        if (toolStripToDrag is not ISupportToolStripPanel)
        {
            Debug.Fail("Move called on immovable object.");
            return;
        }

        Point clientLocation = PointToClient(screenLocation);
        if (!DragBounds.Contains(clientLocation))
        {
            MoveOutsideContainer(toolStripToDrag, screenLocation);
            return;
        }
        else
        {
            Join(toolStripToDrag, clientLocation);
        }
    }

    private void MoveInsideContainer(ToolStrip toolStripToDrag, Point clientLocation)
    {
        ISupportToolStripPanel draggedControl = toolStripToDrag;

        // If the point is not in this rafting container forward on to the appropriate container.
        if (draggedControl.IsCurrentlyDragging && !DragBounds.Contains(clientLocation))
        {
            return;
        }

        // We know we're moving inside the container.

        bool changedRow = false;

        ClearDragFeedback();

        // In design mode we get bogus values for client location.
        if (toolStripToDrag.Site is not null && toolStripToDrag.Site.DesignMode && IsHandleCreated)
        {
            if (clientLocation.X < 0 || clientLocation.Y < 0)
            {
                Point currentCursorLoc = PointToClient(WindowsFormsUtils.LastCursorPoint);
                if (ClientRectangle.Contains(currentCursorLoc))
                {
                    clientLocation = currentCursorLoc;
                }
            }
        }

        //
        // Point INSIDE this rafting container
        //

        ToolStripPanelRow? currentToolStripPanelRow = draggedControl.ToolStripPanelRow;

#if DEBUG
        bool debugModeOnly_ChangedContainers = currentToolStripPanelRow is null || (currentToolStripPanelRow.ToolStripPanel != this);
#endif

        bool pointInCurrentRow = false;
        if (currentToolStripPanelRow is not null && currentToolStripPanelRow.Visible && currentToolStripPanelRow.ToolStripPanel == this)
        {
            if (toolStripToDrag.IsCurrentlyDragging)
            {
                // Dragging with mouse, use DragBounds to check
                pointInCurrentRow = currentToolStripPanelRow.DragBounds.Contains(clientLocation);
            }
            else
            {
                // Location is set directly, use normal Bounds to check
                pointInCurrentRow = currentToolStripPanelRow.Bounds.Contains(clientLocation);
            }
        }

        if (pointInCurrentRow)
        {
            // Point INSIDE same rafting row
            draggedControl.ToolStripPanelRow?.MoveControl(toolStripToDrag, GetStartLocation(toolStripToDrag), clientLocation);
        }
        else
        {
            // Point OUTSIDE current rafting row.
            ToolStripPanelRow? row = PointToRow(clientLocation);
            if (row is null)
            {
                // There's no row at this point so lets create one.
                int index = RowsInternal.Count;

                if (Orientation == Orientation.Horizontal)
                {
                    // If it's above the first row, insert at the front.
                    index = (clientLocation.Y <= Padding.Left) ? 0 : index;
                }
                else
                {
                    // Orientation.Vertical
                    // if it's before the first row, insert at the front.
                    index = (clientLocation.X <= Padding.Left) ? 0 : index;
                }

                ToolStripPanelRow? previousRow = null;
                if (RowsInternal.Count > 0)
                {
                    if (index == 0)
                    {
                        previousRow = RowsInternal[0];
                    }
                    else if (index > 0)
                    {
                        previousRow = RowsInternal[index - 1];
                    }
                }

                if (previousRow is not null
                    && previousRow.ControlsInternal.Count == 1
                    && previousRow.ControlsInternal.Contains(toolStripToDrag))
                {
                    // If the previous row already contains this control it's futile to create a new row, we're just
                    // going to wind up disposing this one and causing great amounts of flicker.
                    row = previousRow;

                    // Move the ToolStrip to the new Location in the existing row.
                    if (toolStripToDrag.IsInDesignMode)
                    {
                        Point endLocation = (Orientation == Orientation.Horizontal) ? new Point(clientLocation.X, row.Bounds.Y) : new Point(row.Bounds.X, clientLocation.Y);
                        draggedControl.ToolStripPanelRow?.MoveControl(toolStripToDrag, GetStartLocation(toolStripToDrag), endLocation);
                    }
                }
                else
                {
                    // Create a new row and insert it.
                    row = new ToolStripPanelRow(this);
                    RowsInternal.Insert(index, row);
                }
            }
            else if (!row.CanMove(toolStripToDrag))
            {
                // There was a row, but we can't add the control to it, creating/inserting new row.
                int index = RowsInternal.IndexOf(row);

                if (currentToolStripPanelRow is not null && currentToolStripPanelRow.ControlsInternal.Count == 1)
                {
                    if (index > 0 && index - 1 == RowsInternal.IndexOf(currentToolStripPanelRow))
                    {
                        // Attempts to leave the current row failed as there's no space in the next row.
                        // Since there's only one control, just keep the row.
                        return;
                    }
                }

                row = new ToolStripPanelRow(this);
                RowsInternal.Insert(index, row);
                clientLocation.Y = row.Bounds.Y;
            }

            changedRow = (currentToolStripPanelRow != row);
            if (!changedRow)
            {
                if (currentToolStripPanelRow is not null && currentToolStripPanelRow.ControlsInternal.Count > 1)
                {
                    // force a leave/re-enter to occur.
                    currentToolStripPanelRow.LeaveRow(toolStripToDrag);
                    currentToolStripPanelRow = null;
                    changedRow = true;
                }
            }

            if (changedRow)
            {
                currentToolStripPanelRow?.LeaveRow(toolStripToDrag);
                row.JoinRow(toolStripToDrag, clientLocation);
            }

            if (changedRow && draggedControl.IsCurrentlyDragging)
            {
                // Force the layout of the new row.
                for (int i = 0; i < RowsInternal.Count; i++)
                {
                    LayoutTransaction.DoLayout(RowsInternal[i], this, PropertyNames.Rows);
                }

                if (RowsInternal.IndexOf(row) > 0)
                {
                    // When joining a new row, move the cursor to to the location of
                    // the grip, otherwise budging the mouse can pull it down into the next row.
                    Point cursorLoc = toolStripToDrag.PointToScreen(toolStripToDrag.GripRectangle.Location);
                    if (Orientation == Orientation.Vertical)
                    {
                        cursorLoc.X += toolStripToDrag.GripRectangle.Width / 2;
                        cursorLoc.Y = Cursor.Position.Y;
                    }
                    else
                    {
                        cursorLoc.Y += toolStripToDrag.GripRectangle.Height / 2;
                        cursorLoc.X = Cursor.Position.X;
                    }

                    Cursor.Position = cursorLoc;
                }
            }
        }

#if DEBUG
        Debug_VerifyOneToOneCellRowControlMatchup();

        if (draggedControl.IsCurrentlyDragging && changedRow && !debugModeOnly_ChangedContainers)
        {
            // if we have changed containers, we're in a SuspendLayout.
            Debug_VerifyNoOverlaps();
        }
#endif
    }

    private void MoveOutsideContainer(ToolStrip toolStripToDrag, Point screenLocation)
    {
        // look for another rafting container.
        ToolStripPanel? panel = ToolStripManager.ToolStripPanelFromPoint(toolStripToDrag, screenLocation);
        if (panel is not null)
        {
            using (new LayoutTransaction(panel, panel, null))
            {
                panel.MoveControl(toolStripToDrag, screenLocation);
            }

            toolStripToDrag.PerformLayout();
#if DEBUG
            ToolStrip draggedControl = toolStripToDrag;
            if (draggedControl.IsCurrentlyDragging)
            {
                Debug_VerifyNoOverlaps();
            }
#endif
        }
        else
        {
            GiveToolStripPanelFeedback(toolStripToDrag, screenLocation);
        }
    }

    /// <summary>
    ///  Given a point within the ToolStripPanel client area -
    ///  it returns the row. If no such row exists, returns null
    /// </summary>
    public ToolStripPanelRow? PointToRow(Point clientLocation)
    {
        // PERF: since we're using the PropertyStore for this.RowsInternal, its actually
        // faster to use foreach.
        foreach (ToolStripPanelRow row in RowsInternal)
        {
            Rectangle bounds = LayoutUtils.InflateRect(row.Bounds, row.Margin);

            // at this point we may not be sized correctly. Guess.
            if (ParentInternal is not null)
            {
                if (Orientation == Orientation.Horizontal && (bounds.Width == 0))
                {
                    bounds.Width = ParentInternal.DisplayRectangle.Width;
                }
                else if (Orientation == Orientation.Vertical && (bounds.Height == 0))
                {
                    bounds.Height = ParentInternal.DisplayRectangle.Height;
                }
            }

            if (bounds.Contains(clientLocation))
            {
                return row;
            }
        }

        return null;
    }

#if DEBUG
    [Conditional("DEBUG")]
    private void Debug_VerifyOneToOneCellRowControlMatchup()
    {
        for (int i = 0; i < RowsInternal.Count; i++)
        {
            ToolStripPanelRow row = RowsInternal[i];
            foreach (ToolStripPanelCell cell in row.Cells)
            {
                if (cell.Control is not null)
                {
                    ToolStripPanelRow? currentlyAssignedRow = ((ISupportToolStripPanel)cell.Control).ToolStripPanelRow;
                    if (currentlyAssignedRow != row)
                    {
                        int goodRowIndex = (currentlyAssignedRow is not null) ? RowsInternal.IndexOf(currentlyAssignedRow) : -1;
                        if (goodRowIndex == -1)
                        {
                            Debug.Fail($"ToolStripPanelRow has not been assigned!  Should be set to {i}.");
                        }
                        else
                        {
                            Debug.Fail($"Detected orphan cell! {cell.Control.Name} is in row {goodRowIndex}. It shouldn't have a cell in {i}! \r\n\r\nTurn on DEBUG_PAINT in ToolStripPanel and ToolStripPanelRow to investigate.");
                        }
                    }
                }
                else
                {
                    Debug.Fail("why do we have a cell with a null control in this row?");
                }
            }
        }
    }

    [Conditional("DEBUG")]
    private void Debug_VerifyNoOverlaps()
    {
        foreach (Control c1 in Controls)
        {
            foreach (Control c2 in Controls)
            {
                if (c1 == c2)
                {
                    continue;
                }

                Rectangle intersection = c1.Bounds;
                intersection.Intersect(c2.Bounds);

                if (!LayoutUtils.IsZeroWidthOrHeight(intersection))
                {
                    ISupportToolStripPanel draggedToolStrip1 = (c1 as ISupportToolStripPanel)!;
                    ISupportToolStripPanel draggedToolStrip2 = (c2 as ISupportToolStripPanel)!;

                    static object GetRow(ISupportToolStripPanel draggedToolStrip, ToolStripPanelRowCollection rows)
                    {
                        int rowIndex = rows.IndexOf(draggedToolStrip.ToolStripPanelRow!);
                        return rowIndex == -1
                            ? "unknown"
                            : rowIndex;
                    }

                    Debug.Fail($@"OVERLAP detection:
{c1.Name ?? ""}: {c1.Bounds} row {GetRow(draggedToolStrip1, RowsInternal)} row bounds {draggedToolStrip1.ToolStripPanelRow?.Bounds}
{c2.Name ?? ""}: {c2.Bounds} row {GetRow(draggedToolStrip2, RowsInternal)} row bounds {draggedToolStrip2.ToolStripPanelRow?.Bounds}");
                }
            }
        }
    }
#endif

    ArrangedElementCollection IArrangedElement.Children => RowsInternal;
}

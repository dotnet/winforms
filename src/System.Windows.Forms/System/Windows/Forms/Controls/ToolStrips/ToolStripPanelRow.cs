// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
#if DEBUG
using System.Globalization;
#endif
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

[ToolboxItem(false)]
public partial class ToolStripPanelRow : Component, IArrangedElement
{
    private Rectangle _bounds = Rectangle.Empty;
    private BitVector32 _state;
    private int _suspendCount;
    private ToolStripPanelRowManager? _rowManager;

    private readonly int _minAllowedWidth;

    private static readonly int s_stateVisible = BitVector32.CreateMask();
    private static readonly int s_stateDisposing = BitVector32.CreateMask(s_stateVisible);
    private static readonly int s_stateLocked = BitVector32.CreateMask(s_stateDisposing);
    private static readonly int s_stateInitialized = BitVector32.CreateMask(s_stateLocked);
    private static readonly int s_stateCachedBoundsMode = BitVector32.CreateMask(s_stateInitialized);
    private static readonly int s_stateInLayout = BitVector32.CreateMask(s_stateCachedBoundsMode);

    private static readonly int s_propControlsCollection = PropertyStore.CreateKey();

#if DEBUG
    private static int s_rowCreationCount;
    private readonly int _thisRowID;
#endif

    public ToolStripPanelRow(ToolStripPanel parent)
        : this(parent, true)
    {
    }

    internal ToolStripPanelRow(ToolStripPanel parent, bool visible)
    {
#if DEBUG
        _thisRowID = ++s_rowCreationCount;
#endif

        const int LogicalMinAllowedWidth = 50;
        _minAllowedWidth = ScaleHelper.ScaleToInitialSystemDpi(LogicalMinAllowedWidth);

        ToolStripPanel = parent;
        _state[s_stateVisible] = visible;
        _state[s_stateDisposing | s_stateLocked | s_stateInitialized] = false;

        using LayoutTransaction lt = new(parent, this, null);
        Margin = DefaultMargin;
        CommonProperties.SetAutoSize(this, true);
    }

    public Rectangle Bounds
    {
        get
        {
            return _bounds;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlControlsDescr))]
    public Control[] Controls
    {
        get
        {
            Control[] controls = new Control[ControlsInternal.Count];
            ControlsInternal.CopyTo(controls, 0);
            return controls;
        }
    }

    /// <summary>
    ///  Collection of child controls.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlControlsDescr))]
    internal ToolStripPanelRowControlCollection ControlsInternal
    {
        get
        {
            if (!Properties.TryGetValue(s_propControlsCollection, out ToolStripPanelRowControlCollection? controlsCollection))
            {
                controlsCollection = Properties.AddValue(s_propControlsCollection, CreateControlsInstance());
            }

            return controlsCollection;
        }
    }

    internal ArrangedElementCollection Cells => ControlsInternal.Cells;

    internal bool CachedBoundsMode
    {
        get => _state[s_stateCachedBoundsMode];
        set => _state[s_stateCachedBoundsMode] = value;
    }

    private ToolStripPanelRowManager RowManager
    {
        get
        {
            if (_rowManager is null)
            {
                _rowManager = Orientation == Orientation.Horizontal
                    ? new HorizontalRowManager(this)
                    : new VerticalRowManager(this);
                Initialized = true;
            }

            return _rowManager;
        }
    }

    protected virtual Padding DefaultMargin
    {
        get
        {
            ToolStripPanelCell? cell = RowManager.GetNextVisibleCell(0, forward: true);
            if (cell is not null && cell.DraggedControl is not null)
            {
                if (cell.DraggedControl.Stretch)
                {
                    Padding padding = ToolStripPanel.RowMargin;

                    // Clear out the padding.
                    if (Orientation == Orientation.Horizontal)
                    {
                        padding.Left = 0;
                        padding.Right = 0;
                    }
                    else
                    {
                        padding.Top = 0;
                        padding.Bottom = 0;
                    }

                    return padding;
                }
            }

            return ToolStripPanel.RowMargin;
        }
    }

    protected virtual Padding DefaultPadding
    {
        get { return Padding.Empty; }
    }

    public Rectangle DisplayRectangle
    {
        get
        {
            return RowManager.DisplayRectangle;
        }
    }

    public LayoutEngine LayoutEngine
    {
        get
        {
            return FlowLayout.Instance;
        }
    }

    internal bool Locked
    {
        get
        {
            return _state[s_stateLocked];
        }
    }

    private bool Initialized
    {
        get
        {
            return _state[s_stateInitialized];
        }
        set
        {
            _state[s_stateInitialized] = value;
        }
    }

    public Padding Margin
    {
        get { return CommonProperties.GetMargin(this); }
        set
        {
            if (Margin != value)
            {
                CommonProperties.SetMargin(this, value);
            }
        }
    }

    public virtual Padding Padding
    {
        get { return CommonProperties.GetPadding(this, DefaultPadding); }
        set
        {
            if (Padding != value)
            {
                CommonProperties.SetPadding(this, value);
            }
        }
    }

    internal Control ParentInternal
    {
        get
        {
            return ToolStripPanel;
        }
    }

    /// <summary>
    ///  Retrieves our internal property storage object. If you have a property
    ///  whose value is not always set, you should store it in here to save
    ///  space.
    /// </summary>
    internal PropertyStore Properties { get; } = new();

    public ToolStripPanel ToolStripPanel { get; }

    internal bool Visible
    {
        get
        {
            return _state[s_stateVisible];
        }
    }

    public Orientation Orientation
    {
        get
        {
            return ToolStripPanel.Orientation;
        }
    }

#if DEBUG
    internal void Debug_PrintRowID()
    {
        Debug.Write(_thisRowID.ToString(CultureInfo.CurrentCulture));
    }
#endif

    /// <summary>
    ///  returns true if there is enough space to "raft" the control
    ///  ow returns false
    /// </summary>
    public bool CanMove(ToolStrip toolStripToDrag)
    {
        return !ToolStripPanel.Locked && !Locked && RowManager.CanMove(toolStripToDrag);
    }

    private ToolStripPanelRowControlCollection CreateControlsInstance()
    {
        return new ToolStripPanelRowControlCollection(this);
    }

    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing)
            {
                _state[s_stateDisposing] = true;
                ControlsInternal.Clear();
            }
        }
        finally
        {
            _state[s_stateDisposing] = false;
            base.Dispose(disposing);
        }
    }

    protected internal virtual void OnControlAdded(Control control, int index)
    {
        // If previously added - remove.

        if (control is ISupportToolStripPanel controlToBeDragged)
        {
            controlToBeDragged.ToolStripPanelRow = this;
        }

        RowManager.OnControlAdded(control, index);
    }

    protected internal virtual void OnOrientationChanged()
    {
        _rowManager = null;
    }

    protected void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        ((IArrangedElement)this).PerformLayout(this, PropertyNames.Size);

        RowManager.OnBoundsChanged(oldBounds, newBounds);
    }

    protected internal virtual void OnControlRemoved(Control control, int index)
    {
        if (!_state[s_stateDisposing])
        {
            SuspendLayout();
            RowManager.OnControlRemoved(control, index);

            // if previously added - remove.

            if (control is ISupportToolStripPanel controlToBeDragged && controlToBeDragged.ToolStripPanelRow == this)
            {
                controlToBeDragged.ToolStripPanelRow = null;
            }

            ResumeLayout(true);
            if (ControlsInternal.Count <= 0)
            {
                ToolStripPanel.RowsInternal.Remove(this);
                Dispose();
            }
        }
    }

    internal Size GetMinimumSize(ToolStrip toolStrip)
    {
        if (toolStrip.MinimumSize == Size.Empty)
        {
            return new Size(_minAllowedWidth, _minAllowedWidth);
        }
        else
        {
            return toolStrip.MinimumSize;
        }
    }

    private void ApplyCachedBounds()
    {
        for (int i = 0; i < Cells.Count; i++)
        {
            IArrangedElement element = Cells[i];
            if (element.ParticipatesInLayout)
            {
                ToolStripPanelCell cell = (ToolStripPanelCell)element;
                element.SetBounds(cell.CachedBounds, BoundsSpecified.None);
                // Debug.Assert( cell.Control is null || cell.CachedBounds.Location == cell.Control.Bounds.Location,
                // "CachedBounds out of sync with bounds!");
            }
        }
    }

    protected virtual void OnLayout(LayoutEventArgs e)
    {
        if (Initialized && !_state[s_stateInLayout])
        {
            _state[s_stateInLayout] = true;
            try
            {
                Margin = DefaultMargin;
                CachedBoundsMode = true;
                try
                {
                    // don't layout in the constructor that's just tacky.
                    bool parentNeedsLayout = LayoutEngine.Layout(this, e);
                }
                finally
                {
                    CachedBoundsMode = false;
                }

                ToolStripPanelCell? cell = RowManager.GetNextVisibleCell(Cells.Count - 1, forward: false);
                if (cell is null)
                {
                    ApplyCachedBounds();
                }
                else if (Orientation == Orientation.Horizontal)
                {
                    OnLayoutHorizontalPostFix();
                }
                else
                {
                    OnLayoutVerticalPostFix();
                }
            }
            finally
            {
                _state[s_stateInLayout] = false;
            }
        }
    }

    private void OnLayoutHorizontalPostFix()
    {
        ToolStripPanelCell? cell = RowManager.GetNextVisibleCell(Cells.Count - 1, forward: false);
        if (cell is null)
        {
            ApplyCachedBounds();
            return;
        }

        // figure out how much space we actually need to free.
        int spaceToFree = cell.CachedBounds.Right - RowManager.DisplayRectangle.Right;

        if (spaceToFree <= 0)
        {
            // we're all good. Just apply the cached bounds.
            ApplyCachedBounds();
            return;
        }

        // STEP 1 remove empty space in the row.

        // since layout is suspended, we'll need to watch changes to the margin
        // as a result of calling FreeSpaceFromRow.
        int[] margins = new int[Cells.Count];
        for (int i = 0; i < Cells.Count; i++)
        {
            ToolStripPanelCell c = (ToolStripPanelCell)Cells[i];
            margins[i] = c.Margin.Left;
        }

        spaceToFree -= RowManager.FreeSpaceFromRow(spaceToFree);

        // now apply those changes to the cached bounds.
        for (int i = 0; i < Cells.Count; i++)
        {
            ToolStripPanelCell c = (ToolStripPanelCell)Cells[i];
            Rectangle cachedBounds = c.CachedBounds;
            cachedBounds.X -= Math.Max(0, margins[i] - c.Margin.Left);
            c.CachedBounds = cachedBounds;
        }

        if (spaceToFree <= 0)
        {
            ApplyCachedBounds();
            return;
        }

        // STEP 2 change the size of the remaining ToolStrips from Right to Left.
        int[]? cellOffsets = null;
        for (int i = Cells.Count - 1; i >= 0; i--)
        {
            ToolStripPanelCell currentCell = (ToolStripPanelCell)Cells[i];
            if (currentCell.Visible)
            {
                Size minSize = GetMinimumSize((ToolStrip)currentCell.Control);
                Rectangle cachedBounds = currentCell.CachedBounds;

                // found some space to free.
                if (cachedBounds.Width > minSize.Width)
                {
                    spaceToFree -= (cachedBounds.Width - minSize.Width);
                    // make sure we don't take more space than we need - if spaceToFree is less than 0, add back in.
                    cachedBounds.Width = (spaceToFree < 0) ? minSize.Width + -spaceToFree : minSize.Width;

                    // we're not re-performing a layout, so we need to adjust the next cell
                    for (int j = i + 1; j < Cells.Count; j++)
                    {
                        cellOffsets ??= new int[Cells.Count];

                        cellOffsets[j] += Math.Max(0, currentCell.CachedBounds.Width - cachedBounds.Width);
                    }

                    currentCell.CachedBounds = cachedBounds;
                }
            }

            if (spaceToFree <= 0)
            {
                break;
            }
        }

        // fixup for items before it shrinking.
        if (cellOffsets is not null)
        {
            for (int i = 0; i < Cells.Count; i++)
            {
                ToolStripPanelCell c = (ToolStripPanelCell)Cells[i];
                Rectangle cachedBounds = c.CachedBounds;
                cachedBounds.X -= cellOffsets[i];
                c.CachedBounds = cachedBounds;
            }
        }

        ApplyCachedBounds();
    }

    private void OnLayoutVerticalPostFix()
    {
        ToolStripPanelCell? cell = RowManager.GetNextVisibleCell(Cells.Count - 1, forward: false);
        if (cell is null)
        {
            ApplyCachedBounds();
            return;
        }

        // figure out how much space we actually need to free.
        int spaceToFree = cell.CachedBounds.Bottom - RowManager.DisplayRectangle.Bottom;

        if (spaceToFree <= 0)
        {
            // we're all good. Just apply the cached bounds.
            ApplyCachedBounds();
            return;
        }

        // STEP 1 remove empty space in the row.

        // since layout is suspended, we'll need to watch changes to the margin
        // as a result of calling FreeSpaceFromRow.
        int[] margins = new int[Cells.Count];
        for (int i = 0; i < Cells.Count; i++)
        {
            ToolStripPanelCell c = (ToolStripPanelCell)Cells[i];
            margins[i] = c.Margin.Top;
        }

        spaceToFree -= RowManager.FreeSpaceFromRow(spaceToFree);

        // now apply those changes to the cached bounds.
        for (int i = 0; i < Cells.Count; i++)
        {
            ToolStripPanelCell c = (ToolStripPanelCell)Cells[i];
            Rectangle cachedBounds = c.CachedBounds;
            cachedBounds.X = Math.Max(0, cachedBounds.X - margins[i] - c.Margin.Top);
            c.CachedBounds = cachedBounds;
        }

        if (spaceToFree <= 0)
        {
            ApplyCachedBounds();
            return;
        }

        // STEP 2 change the size of the remaining ToolStrips from Bottom to Top.
        int[]? cellOffsets = null;
        for (int i = Cells.Count - 1; i >= 0; i--)
        {
            ToolStripPanelCell currentCell = (ToolStripPanelCell)Cells[i];
            if (currentCell.Visible)
            {
                Size minSize = GetMinimumSize((ToolStrip)currentCell.Control);
                Rectangle cachedBounds = currentCell.CachedBounds;

                // found some space to free.
                if (cachedBounds.Height > minSize.Height)
                {
                    spaceToFree -= (cachedBounds.Height - minSize.Height);
                    // make sure we don't take more space than we need - if spaceToFree is less than 0, add back in.
                    cachedBounds.Height = (spaceToFree < 0) ? minSize.Height + -spaceToFree : minSize.Height;

                    // we're not re-performing a layout, so we need to adjust the next cell
                    for (int j = i + 1; j < Cells.Count; j++)
                    {
                        cellOffsets ??= new int[Cells.Count];

                        cellOffsets[j] += Math.Max(0, currentCell.CachedBounds.Height - cachedBounds.Height);
                    }

                    currentCell.CachedBounds = cachedBounds;
                }
            }

            if (spaceToFree <= 0)
            {
                break;
            }
        }

        // fixup for items before it shrinking.
        if (cellOffsets is not null)
        {
            for (int i = 0; i < Cells.Count; i++)
            {
                ToolStripPanelCell c = (ToolStripPanelCell)Cells[i];
                Rectangle cachedBounds = c.CachedBounds;
                cachedBounds.Y -= cellOffsets[i];
                c.CachedBounds = cachedBounds;
            }
        }

        ApplyCachedBounds();
    }

    private void SetBounds(Rectangle bounds)
    {
        if (bounds != _bounds)
        {
            Rectangle oldBounds = _bounds;

            _bounds = bounds;
            OnBoundsChanged(oldBounds, bounds);
        }
    }

    private void SuspendLayout()
    {
        _suspendCount++;
    }

    private void ResumeLayout(bool performLayout)
    {
        _suspendCount--;
        if (performLayout)
        {
            ((IArrangedElement)this).PerformLayout(this, null);
        }
    }

    ArrangedElementCollection IArrangedElement.Children
    {
        get
        {
            return Cells;
        }
    }

    /// <summary>
    ///  Should not be exposed as this returns an unexposed type.
    /// </summary>
    IArrangedElement IArrangedElement.Container
    {
        get
        {
            return ToolStripPanel;
        }
    }

    Rectangle IArrangedElement.DisplayRectangle
    {
        get
        {
            Rectangle displayRectangle = Bounds;

            return displayRectangle;
        }
    }

    bool IArrangedElement.ParticipatesInLayout
    {
        get
        {
            return Visible;
        }
    }

    PropertyStore IArrangedElement.Properties
    {
        get
        {
            return Properties;
        }
    }

    Size IArrangedElement.GetPreferredSize(Size constrainingSize)
    {
        Size preferredSize = LayoutEngine.GetPreferredSize(this, constrainingSize - Padding.Size) + Padding.Size;

        if (Orientation == Orientation.Horizontal && ParentInternal is not null)
        {
            preferredSize.Width = DisplayRectangle.Width;
        }
        else
        {
            preferredSize.Height = DisplayRectangle.Height;
        }

        return preferredSize;
    }

    // Sets the bounds for an element.
    void IArrangedElement.SetBounds(Rectangle bounds, BoundsSpecified specified)
    {
        // In this case the parent is telling us to refresh our bounds - don't call PerformLayout.
        SetBounds(bounds);
    }

    void IArrangedElement.PerformLayout(IArrangedElement container, string? propertyName)
    {
        if (_suspendCount <= 0)
        {
            OnLayout(new LayoutEventArgs(container, propertyName));
        }
    }

    internal Rectangle DragBounds => RowManager.DragBounds;

    internal void MoveControl(ToolStrip movingControl, Point startClientLocation, Point endClientLocation)
    {
        RowManager.MoveControl(movingControl, startClientLocation, endClientLocation);
    }

    internal void JoinRow(ToolStrip toolStripToDrag, Point locationToDrag)
    {
        RowManager.JoinRow(toolStripToDrag, locationToDrag);
    }

    internal void LeaveRow(ToolStrip toolStripToDrag)
    {
        RowManager.LeaveRow(toolStripToDrag);
        if (ControlsInternal.Count == 0)
        {
            ToolStripPanel.RowsInternal.Remove(this);
            Dispose();
        }
    }
}

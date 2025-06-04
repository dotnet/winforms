// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

public partial class ToolStripPanelRow
{
    private class HorizontalRowManager : ToolStripPanelRowManager
    {
        public HorizontalRowManager(ToolStripPanelRow owner) : base(owner)
        {
            owner.SuspendLayout();
            FlowLayoutSettings.WrapContents = false;
            FlowLayoutSettings.FlowDirection = FlowDirection.LeftToRight;
            owner.ResumeLayout(false);
        }

        public override Rectangle DisplayRectangle
        {
            get
            {
                Rectangle displayRect = ((IArrangedElement)Row).DisplayRectangle;

                if (ToolStripPanel is not null)
                {
                    Rectangle raftingDisplayRectangle = ToolStripPanel.DisplayRectangle;

                    if ((!ToolStripPanel.Visible || LayoutUtils.IsZeroWidthOrHeight(raftingDisplayRectangle)) && (ToolStripPanel.ParentInternal is not null))
                    {
                        // if were layed out before we're visible we have the wrong display rectangle, so we need to calculate it.
                        displayRect.Width = ToolStripPanel.ParentInternal.DisplayRectangle.Width - (ToolStripPanel.Margin.Horizontal + ToolStripPanel.Padding.Horizontal) - Row.Margin.Horizontal;
                    }
                    else
                    {
                        displayRect.Width = raftingDisplayRectangle.Width - Row.Margin.Horizontal;
                    }
                }

                return displayRect;
            }
        }

        public override Rectangle DragBounds
        {
            get
            {
                Rectangle dragBounds = Row.Bounds;
                int index = ToolStripPanel.RowsInternal.IndexOf(Row);

                if (index > 0)
                {
                    Rectangle previousRowBounds = ToolStripPanel.RowsInternal[index - 1].Bounds;
                    int y = previousRowBounds.Y + previousRowBounds.Height - (previousRowBounds.Height >> 2);

                    dragBounds.Height += dragBounds.Y - y;
                    dragBounds.Y = y;
                }

                if (index < ToolStripPanel.RowsInternal.Count - 1)
                {
                    Rectangle nextRowBounds = ToolStripPanel.RowsInternal[index + 1].Bounds;

                    dragBounds.Height += (nextRowBounds.Height >> 2) + Row.Margin.Bottom + ToolStripPanel.RowsInternal[index + 1].Margin.Top;
                }

                dragBounds.Width += Row.Margin.Horizontal + ToolStripPanel.Padding.Horizontal + 5;
                dragBounds.X -= Row.Margin.Left + ToolStripPanel.Padding.Left + 4;
                return dragBounds;
            }
        }

        /// <summary>
        ///  returns true if there is enough space to "raft" the control
        ///  ow returns false
        /// </summary>
        public override bool CanMove(ToolStrip toolStripToDrag)
        {
            if (base.CanMove(toolStripToDrag))
            {
                Size totalSize = Size.Empty;

                for (int i = 0; i < Row.ControlsInternal.Count; i++)
                {
                    totalSize += Row.GetMinimumSize((ToolStrip)Row.ControlsInternal[i]);
                }

                totalSize += Row.GetMinimumSize(toolStripToDrag);
                return totalSize.Width < DisplayRectangle.Width;
            }

            return false;
        }

        protected internal override int FreeSpaceFromRow(int spaceToFree)
        {
            int requiredSpace = spaceToFree;
            // take a look at the last guy. if his right edge exceeds
            // the new bounds, then we should go ahead and push him into view.

            if (spaceToFree > 0)
            {
                // we should shrink the last guy and then move him.
                ToolStripPanelCell? lastCellOnRow = GetNextVisibleCell(Row.Cells.Count - 1, forward: false);
                if (lastCellOnRow is null)
                {
                    return 0;
                }

                Padding cellMargin = lastCellOnRow.Margin;

                // only check margin.left as we are only concerned with getting right edge of
                // the toolstrip into view. (space after the fact doesn't count).
                if (cellMargin.Left >= spaceToFree)
                {
                    cellMargin.Left -= spaceToFree;
                    cellMargin.Right = 0;
                    spaceToFree = 0;
                }
                else
                {
                    spaceToFree -= lastCellOnRow.Margin.Left;
                    cellMargin.Left = 0;
                    cellMargin.Right = 0;
                }

                lastCellOnRow.Margin = cellMargin;

                // Start moving the toolstrips before this guy.
                spaceToFree -= MoveLeft(Row.Cells.Count - 1, spaceToFree);
            }

            return requiredSpace - Math.Max(0, spaceToFree);
        }

        public override void MoveControl(ToolStrip movingControl, Point clientStartLocation, Point clientEndLocation)
        {
            if (Row.Locked)
            {
                return;
            }

            if (DragBounds.Contains(clientEndLocation))
            {
                int index = Row.ControlsInternal.IndexOf(movingControl);
                int deltaX = clientEndLocation.X - clientStartLocation.X;

                if (deltaX < 0)
                {
                    // moving to the left
                    MoveLeft(index, deltaX * -1);
                }
                else
                {
                    MoveRight(index, deltaX);
                }
            }
            else
            {
                base.MoveControl(movingControl, clientStartLocation, clientEndLocation);
            }
        }

        private int MoveLeft(int index, int spaceToFree)
        {
            int freedSpace = 0;

            Row.SuspendLayout();
            try
            {
                if (spaceToFree == 0 || index < 0)
                {
                    return 0;
                }

                // remove all margins starting from the index.
                for (int i = index; i >= 0; i--)
                {
                    ToolStripPanelCell? cell = (ToolStripPanelCell)Row.Cells[i];
                    if (!cell.Visible && !cell.ControlInDesignMode)
                    {
                        continue;
                    }

                    int requiredSpace = spaceToFree - freedSpace;

                    Padding cellMargin = cell.Margin;

                    if (cellMargin.Horizontal >= requiredSpace)
                    {
                        freedSpace += requiredSpace;

                        cellMargin.Left -= requiredSpace;
                        cellMargin.Right = 0;
                        cell.Margin = cellMargin;
                    }
                    else
                    {
                        freedSpace += cell.Margin.Horizontal;
                        cellMargin.Left = 0;
                        cellMargin.Right = 0;
                        cell.Margin = cellMargin;
                    }

                    if (freedSpace >= spaceToFree)
                    {
                        // add the space we freed to the next guy.
                        if (index + 1 < Row.Cells.Count)
                        {
                            cell = GetNextVisibleCell(index + 1, /*forward*/true);
                            if (cell is not null)
                            {
                                cellMargin = cell.Margin;
                                cellMargin.Left += spaceToFree;
                                cell.Margin = cellMargin;
                            }
                        }

                        return spaceToFree;
                    }
                }
            }
            finally
            {
                Row.ResumeLayout(true);
            }

            return freedSpace;
        }

        private int MoveRight(int index, int spaceToFree)
        {
            int freedSpace = 0;
            Row.SuspendLayout();
            try
            {
                if (spaceToFree == 0 || index < 0 || index >= Row.ControlsInternal.Count)
                {
                    return 0;
                }

                ToolStripPanelCell? cell;
                Padding cellMargin;

                // remove all margins after this point in the index.
                for (int i = index + 1; i < Row.Cells.Count; i++)
                {
                    cell = (ToolStripPanelCell)Row.Cells[i];
                    if (!cell.Visible && !cell.ControlInDesignMode)
                    {
                        continue;
                    }

                    int requiredSpace = spaceToFree - freedSpace;

                    cellMargin = cell.Margin;

                    if (cellMargin.Horizontal >= requiredSpace)
                    {
                        freedSpace += requiredSpace;

                        cellMargin.Left -= requiredSpace;
                        cellMargin.Right = 0;
                        cell.Margin = cellMargin;
                    }
                    else
                    {
                        freedSpace += cell.Margin.Horizontal;
                        cellMargin.Left = 0;
                        cellMargin.Right = 0;
                        cell.Margin = cellMargin;
                    }

                    break;
                }

                // add in the space at the end of the row.
                if (Row.Cells.Count > 0 && (spaceToFree > freedSpace))
                {
                    ToolStripPanelCell? lastCell = GetNextVisibleCell(Row.Cells.Count - 1, forward: false);
                    if (lastCell is not null)
                    {
                        freedSpace += DisplayRectangle.Right - lastCell.Bounds.Right;
                    }
                    else
                    {
                        freedSpace += DisplayRectangle.Width;
                    }
                }

                // set the margin of the control that's moving.
                if (spaceToFree <= freedSpace)
                {
                    // add the space we freed to the first guy.
                    cell = GetNextVisibleCell(index, forward: true);
                    cell ??= Row.Cells[index] as ToolStripPanelCell;

                    Debug.Assert(cell is not null, "Don't expect cell to be null here, what's going on?");

                    if (cell is not null)
                    {
                        cellMargin = cell.Margin;
                        cellMargin.Left += spaceToFree;
                        cell.Margin = cellMargin;
                    }

                    return spaceToFree;
                }

                // Now start shrinking.
                for (int i = index + 1; i < Row.Cells.Count; i++)
                {
                    cell = (ToolStripPanelCell)Row.Cells[i];
                    if (!cell.Visible && !cell.ControlInDesignMode)
                    {
                        continue;
                    }

                    int requiredSpace = spaceToFree - freedSpace;

                    if (spaceToFree >= freedSpace)
                    {
                        Row.ResumeLayout(true);
                        return spaceToFree;
                    }
                }

                if (Row.Cells.Count == 1)
                {
                    cell = GetNextVisibleCell(index, forward: true);
                    if (cell is not null)
                    {
                        cellMargin = cell.Margin;
                        cellMargin.Left += freedSpace;
                        cell.Margin = cellMargin;
                    }
                }
            }
            finally
            {
                Row.ResumeLayout(true);
            }

            return freedSpace;
        }

        public override void LeaveRow(ToolStrip toolStripToDrag)
        {
            // this code is here to properly add space to the next control when the
            // toolStripToDrag has been removed from the row.
            Row.SuspendLayout();
            int index = Row.ControlsInternal.IndexOf(toolStripToDrag);
            if (index >= 0)
            {
                if (index < Row.ControlsInternal.Count - 1 /*not the last one in the row*/)
                {
                    ToolStripPanelCell cell = (ToolStripPanelCell)Row.Cells[index];
                    if (cell.Visible)
                    {
                        int spaceOccupiedByCell = cell.Margin.Horizontal + cell.Bounds.Width;

                        // add the space occupied by the cell to the next one.
                        ToolStripPanelCell? nextCell = GetNextVisibleCell(index + 1, forward: true);
                        if (nextCell is not null)
                        {
                            Padding nextCellMargin = nextCell.Margin;
                            nextCellMargin.Left += spaceOccupiedByCell;
                            nextCell.Margin = nextCellMargin;
                        }
                    }
                }

                // remove the control from the row.
                ((IList)Row.Cells).RemoveAt(index);
            }

            Row.ResumeLayout(true);
        }

        protected internal override void OnControlAdded(Control control, int index)
        {
        }

        protected internal override void OnControlRemoved(Control control, int index)
        {
        }

        public override void JoinRow(ToolStrip toolStripToDrag, Point locationToDrag)
        {
            int index;

            if (!Row.ControlsInternal.Contains(toolStripToDrag))
            {
                Row.SuspendLayout();

                try
                {
                    if (Row.ControlsInternal.Count > 0)
                    {
                        // walk through the columns and determine which column you want to insert into.
                        for (index = 0; index < Row.Cells.Count; index++)
                        {
                            ToolStripPanelCell cell = (ToolStripPanelCell)Row.Cells[index];
                            if (!cell.Visible && !cell.ControlInDesignMode)
                            {
                                continue;
                            }

                            // [:   ]  [: x  ]
                            if (Row.Cells[index].Bounds.Contains(locationToDrag))
                            {
                                break;
                            }

                            // take into account the following scenarios
                            //  [:   ]  x [:   ]
                            // x [:  ]    [:   ]
                            if (Row.Cells[index].Bounds.X >= locationToDrag.X)
                            {
                                break;
                            }
                        }

                        // Plop the new control in the midst of the row in question.
                        if (index < Row.ControlsInternal.Count)
                        {
                            Row.ControlsInternal.Insert(index, toolStripToDrag);
                        }
                        else
                        {
                            Row.ControlsInternal.Add(toolStripToDrag);
                        }

                        // since layout is suspended the control may not be set to its preferred size yet
                        int controlToDragWidth = (toolStripToDrag.AutoSize) ? toolStripToDrag.PreferredSize.Width : toolStripToDrag.Width;

                        //
                        // now make it look like it belongs in the row.
                        //
                        // PUSH the controls after it to the right

                        int requiredSpace = controlToDragWidth;
                        if (index == 0)
                        {
                            // make sure we account for the left side
                            requiredSpace += locationToDrag.X;
                        }

                        int freedSpace = 0;

                        if (index < Row.ControlsInternal.Count - 1)
                        {
                            ToolStripPanelCell nextCell = (ToolStripPanelCell)Row.Cells[index + 1];
                            Padding nextCellMargin = nextCell.Margin;

                            // if we've already got the empty space
                            // (available to us via the margin) use that.
                            if (nextCellMargin.Left > requiredSpace)
                            {
                                nextCellMargin.Left -= requiredSpace;
                                nextCell.Margin = nextCellMargin;
                                freedSpace = requiredSpace;
                            }
                            else
                            {
                                // otherwise we've got to
                                // push all controls after this point to the right
                                // this dumps the extra stuff into the margin of index+1
                                freedSpace = MoveRight(index + 1, requiredSpace - freedSpace);

                                // refetch the margin for "index+1" and remove the freed space
                                // from it - we want to actually put this to use on the control
                                // before this one - we're making room for the control at
                                // position "index"
                                if (freedSpace > 0)
                                {
                                    nextCellMargin = nextCell.Margin;
                                    nextCellMargin.Left = Math.Max(0, nextCellMargin.Left - freedSpace);
                                    nextCell.Margin = nextCellMargin;
                                }
                            }
                        }
                        else
                        {
                            // we're adding to the end.
                            ToolStripPanelCell? nextCell = GetNextVisibleCell(Row.Cells.Count - 2, forward: false);
                            ToolStripPanelCell? lastCell = GetNextVisibleCell(Row.Cells.Count - 1, forward: false);

                            // count the stuff at the end of the row as freed space
                            if (nextCell is not null && lastCell is not null)
                            {
                                Padding lastCellMargin = lastCell.Margin;
                                lastCellMargin.Left = Math.Max(0, locationToDrag.X - nextCell.Bounds.Right);
                                lastCell.Margin = lastCellMargin;
                                freedSpace = requiredSpace;
                            }
                        }

                        // If we still need more space, then...
                        // PUSH the controls before it to the left
                        if (freedSpace < requiredSpace && index > 0)
                        {
                            freedSpace = MoveLeft(index - 1, requiredSpace - freedSpace);
                        }

                        if (index == 0)
                        {
                            // if the index is zero and there were controls in the row
                            // we need to take care of pushing over the new cell.
                            if (freedSpace - controlToDragWidth > 0)
                            {
                                ToolStripPanelCell newCell = (ToolStripPanelCell)Row.Cells[index];
                                Padding newCellMargin = newCell.Margin;
                                newCellMargin.Left = freedSpace - controlToDragWidth;
                                newCell.Margin = newCellMargin;
                            }
                        }
                    }
                    else
                    {
                        // we're adding to the beginning.
                        Row.ControlsInternal.Add(toolStripToDrag);

#if DEBUG
                        ISupportToolStripPanel ctg = toolStripToDrag;
                        Debug.Assert(ctg.ToolStripPanelRow == Row, "we should now be in the new panel row.");
#endif
                        if (Row.Cells.Count > 0 || toolStripToDrag.IsInDesignMode)
                        {
                            // we're adding to the beginning.
                            ToolStripPanelCell? cell = GetNextVisibleCell(Row.Cells.Count - 1, forward: false);
                            if (cell is null && toolStripToDrag.IsInDesignMode)
                            {
                                cell = (ToolStripPanelCell)Row.Cells[Row.Cells.Count - 1];
                            }

                            if (cell is not null)
                            {
                                Padding cellMargin = cell.Margin;
                                cellMargin.Left = Math.Max(0, locationToDrag.X - Row.Margin.Left);
                                cell.Margin = cellMargin;
                            }
                        }
                    }
                }
                finally
                {
                    Row.ResumeLayout(true);
                }
            }
        }

        protected internal override void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.OnBoundsChanged(oldBounds, newBounds);
        }
    }
}

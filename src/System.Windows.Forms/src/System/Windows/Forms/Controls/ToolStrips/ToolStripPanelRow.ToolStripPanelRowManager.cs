// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class ToolStripPanelRow
{
    private abstract class ToolStripPanelRowManager
    {
        private FlowLayoutSettings? _flowLayoutSettings;

        public ToolStripPanelRowManager(ToolStripPanelRow owner)
        {
            Row = owner;
        }

        public virtual bool CanMove(ToolStrip toolStripToDrag)
        {
            if (toolStripToDrag is ISupportToolStripPanel toolStripToDragAsRaftingControl && toolStripToDragAsRaftingControl.Stretch)
            {
                return false;
            }

            foreach (Control control in Row.ControlsInternal)
            {
                if (control is ISupportToolStripPanel controlAsRaftingControl && controlAsRaftingControl.Stretch)
                {
                    return false;
                }
            }

            return true;
        }

        public virtual Rectangle DragBounds
        {
            get { return Rectangle.Empty; }
        }

        public virtual Rectangle DisplayRectangle
        {
            get { return Rectangle.Empty; }
        }

        public ToolStripPanel ToolStripPanel
        {
            get { return Row.ToolStripPanel; }
        }

        public ToolStripPanelRow Row { get; }

        public FlowLayoutSettings FlowLayoutSettings
        {
            get
            {
                _flowLayoutSettings ??= new FlowLayoutSettings(Row);

                return _flowLayoutSettings;
            }
        }

        protected internal virtual int FreeSpaceFromRow(int spaceToFree)
        {
            return 0;
        }

        protected virtual int Grow(int index, int growBy)
        {
            int freedSpace = 0;
            if (index >= 0 && index < Row.ControlsInternal.Count - 1)
            {
                ToolStripPanelCell cell = (ToolStripPanelCell)Row.Cells[index];
                if (cell.Visible)
                {
                    freedSpace = cell.Grow(growBy);
                }
            }

            return freedSpace;
        }

        public ToolStripPanelCell? GetNextVisibleCell(int index, bool forward)
        {
            if (forward)
            {
                for (int i = index; i < Row.Cells.Count; i++)
                {
                    ToolStripPanelCell cell = (ToolStripPanelCell)Row.Cells[i];
                    if ((cell.Visible || (Row.ToolStripPanel.Visible && cell.ControlInDesignMode)) && cell.ToolStripPanelRow == Row)
                    {
                        return cell;
                    }
                }
            }
            else
            {
                for (int i = index; i >= 0; i--)
                {
                    ToolStripPanelCell cell = (ToolStripPanelCell)Row.Cells[i];
                    if ((cell.Visible || (Row.ToolStripPanel.Visible && cell.ControlInDesignMode)) && cell.ToolStripPanelRow == Row)
                    {
                        return cell;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///  grows all controls after the index to be their preferred size.
        ///  reports back how much space was used.
        /// </summary>
        protected virtual int GrowControlsAfter(int index, int growBy)
        {
            if (growBy < 0)
            {
                Debug.Fail("why was a negative number given to growControlsAfter?");
                return 0;
            }

            int spaceToFree = growBy;

            for (int i = index + 1; i < Row.ControlsInternal.Count; i++)
            {
                // grow the n+1 item first if it was previously shrunk.
                int freedSpace = Grow(i, spaceToFree);

                if (freedSpace >= 0)
                {
                    spaceToFree -= freedSpace;
                    if (spaceToFree <= 0)
                    {
                        return growBy;
                    }
                }
            }

            return growBy - spaceToFree;
        }

        /// <summary>
        ///  grows all controls before the index to be their preferred size.
        ///  reports back how much space was used.
        /// </summary>
        protected virtual int GrowControlsBefore(int index, int growBy)
        {
            if (growBy < 0)
            {
                Debug.Fail("why was a negative number given to growControlsAfter?");
                return 0;
            }

            int spaceToFree = growBy;

            // grow the n-1 item first if it was previously shrunk.
            for (int i = index - 1; i >= 0; i--)
            {
                spaceToFree -= Grow(i, spaceToFree);
                if (spaceToFree <= 0)
                {
                    return growBy; // we've already gotten all the free space.
                }
            }

            return growBy - spaceToFree;
        }

        public virtual void MoveControl(ToolStrip movingControl, Point startClientLocation, Point endClientLocation)
        {
            // ToolStripPanel.Join(movingControl, endScreenLocation);
        }

        public virtual void LeaveRow(ToolStrip toolStripToDrag)
        {
        }

        public virtual void JoinRow(ToolStrip toolStripToDrag, Point locationToDrag)
        {
        }

        protected internal virtual void OnControlAdded(Control c, int index)
        {
        }

        protected internal virtual void OnControlRemoved(Control c, int index)
        {
        }

        protected internal virtual void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
        {
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class PropertyGridView
{
    internal class GridPositionData
    {
        private readonly List<GridEntryCollection> _expandedState;
        private readonly GridEntryCollection? _selectedItemTree;
        private readonly int _itemRow;

        public GridPositionData(PropertyGridView gridView)
        {
            _selectedItemTree = GetGridEntryHierarchy(gridView._selectedGridEntry);
            _expandedState = SaveHierarchyState(gridView.TopLevelGridEntries);
            _itemRow = gridView._selectedRow;
        }

        public GridEntry? Restore(PropertyGridView gridView)
        {
            gridView.RestoreHierarchyState(_expandedState);
            GridEntry? entry = gridView.FindEquivalentGridEntry(_selectedItemTree);

            if (entry is null)
            {
                return null;
            }

            gridView.SelectGridEntry(entry, pageIn: true);

            int delta = gridView._selectedRow - _itemRow;
            if (delta != 0 && gridView.ScrollBar.Visible)
            {
                if (_itemRow < gridView._visibleRows)
                {
                    delta += gridView.GetScrollOffset();

                    if (delta < 0)
                    {
                        delta = 0;
                    }
                    else if (delta > gridView.ScrollBar.Maximum)
                    {
                        delta = gridView.ScrollBar.Maximum - 1;
                    }

                    gridView.SetScrollOffset(delta);
                }
            }

            return entry;
        }
    }
}

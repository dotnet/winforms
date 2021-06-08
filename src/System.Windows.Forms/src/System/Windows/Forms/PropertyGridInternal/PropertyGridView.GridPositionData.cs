﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        internal class GridPositionData
        {
            private readonly ArrayList _expandedState;
            private readonly GridEntryCollection _selectedItemTree;
            private readonly int _itemRow;
            private readonly int _itemCount;

            public GridPositionData(PropertyGridView gridView)
            {
                _selectedItemTree = gridView.GetGridEntryHierarchy(gridView._selectedGridEntry);
                _expandedState = gridView.SaveHierarchyState(gridView._topLevelGridEntries);
                _itemRow = gridView._selectedRow;
                _itemCount = gridView.TotalProps;
            }

            public GridEntry Restore(PropertyGridView gridView)
            {
                gridView.RestoreHierarchyState(_expandedState);
                GridEntry entry = gridView.FindEquivalentGridEntry(_selectedItemTree);

                if (entry is not null)
                {
                    gridView.SelectGridEntry(entry, true);

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
                }

                return entry;
            }
        }
    }
}

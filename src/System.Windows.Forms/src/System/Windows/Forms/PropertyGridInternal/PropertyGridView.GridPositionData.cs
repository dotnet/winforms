// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        internal class GridPositionData
        {
            readonly ArrayList expandedState;
            readonly GridEntryCollection selectedItemTree;
            readonly int itemRow;
            readonly int itemCount;

            public GridPositionData(PropertyGridView gridView)
            {
                selectedItemTree = gridView.GetGridEntryHierarchy(gridView.selectedGridEntry);
                expandedState = gridView.SaveHierarchyState(gridView.topLevelGridEntries);
                itemRow = gridView.selectedRow;
                itemCount = gridView.totalProps;
            }

            public GridEntry Restore(PropertyGridView gridView)
            {
                gridView.RestoreHierarchyState(expandedState);
                GridEntry entry = gridView.FindEquivalentGridEntry(selectedItemTree);

                if (entry != null)
                {
                    gridView.SelectGridEntry(entry, true);

                    int delta = gridView.selectedRow - itemRow;
                    if (delta != 0 && gridView.ScrollBar.Visible)
                    {
                        if (itemRow < gridView.visibleRows)
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

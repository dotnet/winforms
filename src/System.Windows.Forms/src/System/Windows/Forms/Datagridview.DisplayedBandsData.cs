// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        internal class DisplayedBandsData
        {
            private bool dirty;

            private int firstDisplayedFrozenRow;
            private int firstDisplayedFrozenCol;
            private int numDisplayedFrozenRows;
            private int numDisplayedFrozenCols;
            private int numTotallyDisplayedFrozenRows;
            private int firstDisplayedScrollingRow;
            private int numDisplayedScrollingRows;
            private int numTotallyDisplayedScrollingRows;
            private int firstDisplayedScrollingCol;
            private int numDisplayedScrollingCols;
            private int lastTotallyDisplayedScrollingCol;
            private int lastDisplayedScrollingRow;
            private int lastDisplayedFrozenCol;
            private int lastDisplayedFrozenRow;
            private int oldFirstDisplayedScrollingRow;
            private int oldFirstDisplayedScrollingCol;
            private int oldNumDisplayedFrozenRows;
            private int oldNumDisplayedScrollingRows;

            private bool rowInsertionOccurred, columnInsertionOccurred;

            public DisplayedBandsData()
            {
                firstDisplayedFrozenRow = -1;
                firstDisplayedFrozenCol = -1;
                firstDisplayedScrollingRow = -1;
                firstDisplayedScrollingCol = -1;
                lastTotallyDisplayedScrollingCol = -1;
                lastDisplayedScrollingRow = -1;
                lastDisplayedFrozenCol = -1;
                lastDisplayedFrozenRow = -1;
                oldFirstDisplayedScrollingRow = -1;
                oldFirstDisplayedScrollingCol = -1;
            }

            public bool ColumnInsertionOccurred
            {
                get
                {
                    return columnInsertionOccurred;
                }
            }

            public bool Dirty
            {
                get
                {
                    return dirty;
                }
                set
                {
                    dirty = value;
                }
            }

            public int FirstDisplayedFrozenCol
            {
                set
                {
                    if (value != firstDisplayedFrozenCol)
                    {
                        EnsureDirtyState();
                        firstDisplayedFrozenCol = value;
                    }
                }
            }

            public int FirstDisplayedFrozenRow
            {
                set
                {
                    if (value != firstDisplayedFrozenRow)
                    {
                        EnsureDirtyState();
                        firstDisplayedFrozenRow = value;
                    }
                }
            }

            public int FirstDisplayedScrollingCol
            {
                get
                {
                    return firstDisplayedScrollingCol;
                }
                set
                {
                    if (value != firstDisplayedScrollingCol)
                    {
                        EnsureDirtyState();
                        firstDisplayedScrollingCol = value;
                    }
                }
            }

            public int FirstDisplayedScrollingRow
            {
                get
                {
                    return firstDisplayedScrollingRow;
                }
                set
                {
                    if (value != firstDisplayedScrollingRow)
                    {
                        EnsureDirtyState();
                        firstDisplayedScrollingRow = value;
                    }
                }
            }

            public int LastDisplayedFrozenCol
            {
                set
                {
                    if (value != lastDisplayedFrozenCol)
                    {
                        EnsureDirtyState();
                        lastDisplayedFrozenCol = value;
                    }
                }
            }

            public int LastDisplayedFrozenRow
            {
                set
                {
                    if (value != lastDisplayedFrozenRow)
                    {
                        EnsureDirtyState();
                        lastDisplayedFrozenRow = value;
                    }
                }
            }

            public int LastDisplayedScrollingRow
            {
                set
                {
                    if (value != lastDisplayedScrollingRow)
                    {
                        EnsureDirtyState();
                        lastDisplayedScrollingRow = value;
                    }
                }
            }

            public int LastTotallyDisplayedScrollingCol
            {
                get
                {
                    return lastTotallyDisplayedScrollingCol;
                }
                set
                {
                    if (value != lastTotallyDisplayedScrollingCol)
                    {
                        EnsureDirtyState();
                        lastTotallyDisplayedScrollingCol = value;
                    }
                }
            }

            public int NumDisplayedFrozenCols
            {
                get
                {
                    return numDisplayedFrozenCols;
                }
                set
                {
                    if (value != numDisplayedFrozenCols)
                    {
                        EnsureDirtyState();
                        numDisplayedFrozenCols = value;
                    }
                }
            }

            public int NumDisplayedFrozenRows
            {
                get
                {
                    return numDisplayedFrozenRows;
                }
                set
                {
                    if (value != numDisplayedFrozenRows)
                    {
                        EnsureDirtyState();
                        numDisplayedFrozenRows = value;
                    }
                }
            }

            public int NumDisplayedScrollingRows
            {
                get
                {
                    return numDisplayedScrollingRows;
                }
                set
                {
                    if (value != numDisplayedScrollingRows)
                    {
                        EnsureDirtyState();
                        numDisplayedScrollingRows = value;
                    }
                }
            }

            public int NumDisplayedScrollingCols
            {
                get
                {
                    return numDisplayedScrollingCols;
                }
                set
                {
                    if (value != numDisplayedScrollingCols)
                    {
                        EnsureDirtyState();
                        numDisplayedScrollingCols = value;
                    }
                }
            }

            public int NumTotallyDisplayedFrozenRows
            {
                get
                {
                    return numTotallyDisplayedFrozenRows;
                }
                set
                {
                    if (value != numTotallyDisplayedFrozenRows)
                    {
                        EnsureDirtyState();
                        numTotallyDisplayedFrozenRows = value;
                    }
                }
            }

            public int NumTotallyDisplayedScrollingRows
            {
                get
                {
                    return numTotallyDisplayedScrollingRows;
                }
                set
                {
                    if (value != numTotallyDisplayedScrollingRows)
                    {
                        EnsureDirtyState();
                        numTotallyDisplayedScrollingRows = value;
                    }
                }
            }

            public int OldFirstDisplayedScrollingCol
            {
                get
                {
                    return oldFirstDisplayedScrollingCol;
                }
            }

            public int OldFirstDisplayedScrollingRow
            {
                get
                {
                    return oldFirstDisplayedScrollingRow;
                }
            }

            public int OldNumDisplayedFrozenRows
            {
                get
                {
                    return oldNumDisplayedFrozenRows;
                }
            }

            public int OldNumDisplayedScrollingRows
            {
                get
                {
                    return oldNumDisplayedScrollingRows;
                }
            }

            public bool RowInsertionOccurred
            {
                get
                {
                    return rowInsertionOccurred;
                }
            }

            public void EnsureDirtyState()
            {
                if (!dirty)
                {
                    dirty = true;
                    rowInsertionOccurred = false;
                    columnInsertionOccurred = false;
                    SetOldValues();
                }
            }

            public void CorrectColumnIndexAfterInsertion(int columnIndex, int insertionCount)
            {
                EnsureDirtyState();
                if (oldFirstDisplayedScrollingCol != -1 && columnIndex <= oldFirstDisplayedScrollingCol)
                {
                    oldFirstDisplayedScrollingCol += insertionCount;
                }
                columnInsertionOccurred = true;
            }

            public void CorrectRowIndexAfterDeletion(int rowIndex)
            {
                EnsureDirtyState();
                if (oldFirstDisplayedScrollingRow != -1 && rowIndex <= oldFirstDisplayedScrollingRow)
                {
                    oldFirstDisplayedScrollingRow--;
                }
            }

            public void CorrectRowIndexAfterInsertion(int rowIndex, int insertionCount)
            {
                EnsureDirtyState();
                if (oldFirstDisplayedScrollingRow != -1 && rowIndex <= oldFirstDisplayedScrollingRow)
                {
                    oldFirstDisplayedScrollingRow += insertionCount;
                }
                rowInsertionOccurred = true;
                oldNumDisplayedScrollingRows += insertionCount;
                oldNumDisplayedFrozenRows += insertionCount;
            }

            private void SetOldValues()
            {
                oldFirstDisplayedScrollingRow = firstDisplayedScrollingRow;
                oldFirstDisplayedScrollingCol = firstDisplayedScrollingCol;
                oldNumDisplayedFrozenRows = numDisplayedFrozenRows;
                oldNumDisplayedScrollingRows = numDisplayedScrollingRows;
            }
        }
    }
}

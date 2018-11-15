// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
                this.firstDisplayedFrozenRow = -1;
                this.firstDisplayedFrozenCol = -1;
                this.firstDisplayedScrollingRow = -1;
                this.firstDisplayedScrollingCol = -1;
                this.lastTotallyDisplayedScrollingCol = -1;
                this.lastDisplayedScrollingRow = -1;
                this.lastDisplayedFrozenCol = -1;
                this.lastDisplayedFrozenRow = -1;
                this.oldFirstDisplayedScrollingRow = -1;
                this.oldFirstDisplayedScrollingCol = -1;
            }

            public bool ColumnInsertionOccurred
            {
                get
                {
                    return this.columnInsertionOccurred;
                }
            }

            public bool Dirty
            {
                get
                {
                    return this.dirty;
                }
                set
                {
                    this.dirty = value;
                }
            }

            public int FirstDisplayedFrozenCol
            {
                set
                {
                    if (value != this.firstDisplayedFrozenCol)
                    {
                        EnsureDirtyState();
                        this.firstDisplayedFrozenCol = value;
                    }
                }
            }

            public int FirstDisplayedFrozenRow
            {
                set
                {
                    if (value != this.firstDisplayedFrozenRow)
                    {
                        EnsureDirtyState();
                        this.firstDisplayedFrozenRow = value;
                    }
                }
            }

            public int FirstDisplayedScrollingCol
            {
                get
                {
                    return this.firstDisplayedScrollingCol;
                }
                set
                {
                    if (value != this.firstDisplayedScrollingCol)
                    {
                        EnsureDirtyState();
                        this.firstDisplayedScrollingCol = value;
                    }
                }
            }

            public int FirstDisplayedScrollingRow
            {
                get
                {
                    return this.firstDisplayedScrollingRow;
                }
                set
                {
                    if (value != this.firstDisplayedScrollingRow)
                    {
                        EnsureDirtyState();
                        this.firstDisplayedScrollingRow = value;
                    }
                }
            }

            public int LastDisplayedFrozenCol
            {
                set
                {
                    if (value != this.lastDisplayedFrozenCol)
                    {
                        EnsureDirtyState();
                        this.lastDisplayedFrozenCol = value;
                    }
                }
            }

            public int LastDisplayedFrozenRow
            {
                set
                {
                    if (value != this.lastDisplayedFrozenRow)
                    {
                        EnsureDirtyState();
                        this.lastDisplayedFrozenRow = value;
                    }
                }
            }

            public int LastDisplayedScrollingRow
            {
                set
                {
                    if (value != this.lastDisplayedScrollingRow)
                    {
                        EnsureDirtyState();
                        this.lastDisplayedScrollingRow = value;
                    }
                }
            }

            public int LastTotallyDisplayedScrollingCol
            {
                get
                {
                    return this.lastTotallyDisplayedScrollingCol;
                }
                set
                {
                    if (value != this.lastTotallyDisplayedScrollingCol)
                    {
                        EnsureDirtyState();
                        this.lastTotallyDisplayedScrollingCol = value;
                    }
                }
            }

            public int NumDisplayedFrozenCols
            {
                get
                {
                    return this.numDisplayedFrozenCols;
                }
                set
                {
                    if (value != this.numDisplayedFrozenCols)
                    {
                        EnsureDirtyState();
                        this.numDisplayedFrozenCols = value;
                    }
                }
            }

            public int NumDisplayedFrozenRows
            {
                get
                {
                    return this.numDisplayedFrozenRows;
                }
                set
                {
                    if (value != this.numDisplayedFrozenRows)
                    {
                        EnsureDirtyState();
                        this.numDisplayedFrozenRows = value;
                    }
                }
            }

            public int NumDisplayedScrollingRows
            {
                get
                {
                    return this.numDisplayedScrollingRows;
                }
                set
                {
                    if (value != this.numDisplayedScrollingRows)
                    {
                        EnsureDirtyState();
                        this.numDisplayedScrollingRows = value;
                    }
                }
            }

            public int NumDisplayedScrollingCols
            {
                get
                {
                    return this.numDisplayedScrollingCols;
                }
                set
                {
                    if (value != this.numDisplayedScrollingCols)
                    {
                        EnsureDirtyState();
                        this.numDisplayedScrollingCols = value;
                    }
                }
            }

            public int NumTotallyDisplayedFrozenRows
            {
                get
                {
                    return this.numTotallyDisplayedFrozenRows;
                }
                set
                {
                    if (value != this.numTotallyDisplayedFrozenRows)
                    {
                        EnsureDirtyState();
                        this.numTotallyDisplayedFrozenRows = value;
                    }
                }
            }

            public int NumTotallyDisplayedScrollingRows
            {
                get
                {
                    return this.numTotallyDisplayedScrollingRows;
                }
                set
                {
                    if (value != this.numTotallyDisplayedScrollingRows)
                    {
                        EnsureDirtyState();
                        this.numTotallyDisplayedScrollingRows = value;
                    }
                }
            }

            public int OldFirstDisplayedScrollingCol
            {
                get
                {
                    return this.oldFirstDisplayedScrollingCol;
                }
            }

            public int OldFirstDisplayedScrollingRow
            {
                get
                {
                    return this.oldFirstDisplayedScrollingRow;
                }
            }

            public int OldNumDisplayedFrozenRows
            {
                get
                {
                    return this.oldNumDisplayedFrozenRows;
                }
            }

            public int OldNumDisplayedScrollingRows
            {
                get
                {
                    return this.oldNumDisplayedScrollingRows;
                }
            }

            public bool RowInsertionOccurred
            {
                get
                {
                    return this.rowInsertionOccurred;
                }
            }

            public void EnsureDirtyState()
            {
                if (!this.dirty)
                {
                    this.dirty = true;
                    this.rowInsertionOccurred = false;
                    this.columnInsertionOccurred = false;
                    SetOldValues();
                }
            }

            public void CorrectColumnIndexAfterInsertion(int columnIndex, int insertionCount)
            {
                EnsureDirtyState();
                if (this.oldFirstDisplayedScrollingCol != -1 && columnIndex <= this.oldFirstDisplayedScrollingCol)
                {
                    this.oldFirstDisplayedScrollingCol += insertionCount;
                }
                this.columnInsertionOccurred = true;
            }

            public void CorrectRowIndexAfterDeletion(int rowIndex)
            {
                EnsureDirtyState();
                if (this.oldFirstDisplayedScrollingRow != -1 && rowIndex <= this.oldFirstDisplayedScrollingRow)
                {
                    this.oldFirstDisplayedScrollingRow--;
                }
            }

            public void CorrectRowIndexAfterInsertion(int rowIndex, int insertionCount)
            {
                EnsureDirtyState();
                if (this.oldFirstDisplayedScrollingRow != -1 && rowIndex <= this.oldFirstDisplayedScrollingRow)
                {
                    this.oldFirstDisplayedScrollingRow += insertionCount;
                }
                this.rowInsertionOccurred = true;
                this.oldNumDisplayedScrollingRows += insertionCount;
                this.oldNumDisplayedFrozenRows += insertionCount;
            }

            private void SetOldValues()
            {
                this.oldFirstDisplayedScrollingRow = this.firstDisplayedScrollingRow;
                this.oldFirstDisplayedScrollingCol = this.firstDisplayedScrollingCol;
                this.oldNumDisplayedFrozenRows = this.numDisplayedFrozenRows;
                this.oldNumDisplayedScrollingRows = this.numDisplayedScrollingRows;
            }
        }
    }
}
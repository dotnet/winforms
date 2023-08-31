// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class DataGridView
{
    internal class DisplayedBandsData
    {
        private bool _dirty;

        private int _firstDisplayedFrozenRow;
        private int _firstDisplayedFrozenCol;
        private int _numDisplayedFrozenRows;
        private int _numDisplayedFrozenCols;
        private int _numTotallyDisplayedFrozenRows;
        private int _firstDisplayedScrollingRow;
        private int _numDisplayedScrollingRows;
        private int _numTotallyDisplayedScrollingRows;
        private int _firstDisplayedScrollingCol;
        private int _numDisplayedScrollingCols;
        private int _lastTotallyDisplayedScrollingCol;
        private int _lastDisplayedScrollingRow;
        private int _lastDisplayedFrozenCol;
        private int _lastDisplayedFrozenRow;
        private int _oldFirstDisplayedScrollingRow;
        private int _oldFirstDisplayedScrollingCol;
        private int _oldNumDisplayedFrozenRows;
        private int _oldNumDisplayedScrollingRows;

        private bool _rowInsertionOccurred;
        private bool _columnInsertionOccurred;

        public DisplayedBandsData()
        {
            _firstDisplayedFrozenRow = -1;
            _firstDisplayedFrozenCol = -1;
            _firstDisplayedScrollingRow = -1;
            _firstDisplayedScrollingCol = -1;
            _lastTotallyDisplayedScrollingCol = -1;
            _lastDisplayedScrollingRow = -1;
            _lastDisplayedFrozenCol = -1;
            _lastDisplayedFrozenRow = -1;
            _oldFirstDisplayedScrollingRow = -1;
            _oldFirstDisplayedScrollingCol = -1;
        }

        public bool ColumnInsertionOccurred
        {
            get
            {
                return _columnInsertionOccurred;
            }
        }

        public bool Dirty
        {
            get
            {
                return _dirty;
            }
            set
            {
                _dirty = value;
            }
        }

        public int FirstDisplayedFrozenCol
        {
            set
            {
                if (value != _firstDisplayedFrozenCol)
                {
                    EnsureDirtyState();
                    _firstDisplayedFrozenCol = value;
                }
            }
        }

        public int FirstDisplayedFrozenRow
        {
            set
            {
                if (value != _firstDisplayedFrozenRow)
                {
                    EnsureDirtyState();
                    _firstDisplayedFrozenRow = value;
                }
            }
        }

        public int FirstDisplayedScrollingCol
        {
            get
            {
                return _firstDisplayedScrollingCol;
            }
            set
            {
                if (value != _firstDisplayedScrollingCol)
                {
                    EnsureDirtyState();
                    _firstDisplayedScrollingCol = value;
                }
            }
        }

        public int FirstDisplayedScrollingRow
        {
            get
            {
                return _firstDisplayedScrollingRow;
            }
            set
            {
                if (value != _firstDisplayedScrollingRow)
                {
                    EnsureDirtyState();
                    _firstDisplayedScrollingRow = value;
                }
            }
        }

        public int LastDisplayedFrozenCol
        {
            set
            {
                if (value != _lastDisplayedFrozenCol)
                {
                    EnsureDirtyState();
                    _lastDisplayedFrozenCol = value;
                }
            }
        }

        public int LastDisplayedFrozenRow
        {
            set
            {
                if (value != _lastDisplayedFrozenRow)
                {
                    EnsureDirtyState();
                    _lastDisplayedFrozenRow = value;
                }
            }
        }

        public int LastDisplayedScrollingRow
        {
            set
            {
                if (value != _lastDisplayedScrollingRow)
                {
                    EnsureDirtyState();
                    _lastDisplayedScrollingRow = value;
                }
            }
        }

        public int LastTotallyDisplayedScrollingCol
        {
            get
            {
                return _lastTotallyDisplayedScrollingCol;
            }
            set
            {
                if (value != _lastTotallyDisplayedScrollingCol)
                {
                    EnsureDirtyState();
                    _lastTotallyDisplayedScrollingCol = value;
                }
            }
        }

        public int NumDisplayedFrozenCols
        {
            get
            {
                return _numDisplayedFrozenCols;
            }
            set
            {
                if (value != _numDisplayedFrozenCols)
                {
                    EnsureDirtyState();
                    _numDisplayedFrozenCols = value;
                }
            }
        }

        public int NumDisplayedFrozenRows
        {
            get
            {
                return _numDisplayedFrozenRows;
            }
            set
            {
                if (value != _numDisplayedFrozenRows)
                {
                    EnsureDirtyState();
                    _numDisplayedFrozenRows = value;
                }
            }
        }

        public int NumDisplayedScrollingRows
        {
            get
            {
                return _numDisplayedScrollingRows;
            }
            set
            {
                if (value != _numDisplayedScrollingRows)
                {
                    EnsureDirtyState();
                    _numDisplayedScrollingRows = value;
                }
            }
        }

        public int NumDisplayedScrollingCols
        {
            get
            {
                return _numDisplayedScrollingCols;
            }
            set
            {
                if (value != _numDisplayedScrollingCols)
                {
                    EnsureDirtyState();
                    _numDisplayedScrollingCols = value;
                }
            }
        }

        public int NumTotallyDisplayedFrozenRows
        {
            get
            {
                return _numTotallyDisplayedFrozenRows;
            }
            set
            {
                if (value != _numTotallyDisplayedFrozenRows)
                {
                    EnsureDirtyState();
                    _numTotallyDisplayedFrozenRows = value;
                }
            }
        }

        public int NumTotallyDisplayedScrollingRows
        {
            get
            {
                return _numTotallyDisplayedScrollingRows;
            }
            set
            {
                if (value != _numTotallyDisplayedScrollingRows)
                {
                    EnsureDirtyState();
                    _numTotallyDisplayedScrollingRows = value;
                }
            }
        }

        public int OldFirstDisplayedScrollingCol
        {
            get
            {
                return _oldFirstDisplayedScrollingCol;
            }
        }

        public int OldFirstDisplayedScrollingRow
        {
            get
            {
                return _oldFirstDisplayedScrollingRow;
            }
        }

        public int OldNumDisplayedFrozenRows
        {
            get
            {
                return _oldNumDisplayedFrozenRows;
            }
        }

        public int OldNumDisplayedScrollingRows
        {
            get
            {
                return _oldNumDisplayedScrollingRows;
            }
        }

        public bool RowInsertionOccurred
        {
            get
            {
                return _rowInsertionOccurred;
            }
        }

        public void EnsureDirtyState()
        {
            if (!_dirty)
            {
                _dirty = true;
                _rowInsertionOccurred = false;
                _columnInsertionOccurred = false;
                SetOldValues();
            }
        }

        public void CorrectColumnIndexAfterInsertion(int columnIndex, int insertionCount)
        {
            EnsureDirtyState();
            if (_oldFirstDisplayedScrollingCol != -1 && columnIndex <= _oldFirstDisplayedScrollingCol)
            {
                _oldFirstDisplayedScrollingCol += insertionCount;
            }

            _columnInsertionOccurred = true;
        }

        public void CorrectRowIndexAfterDeletion(int rowIndex)
        {
            EnsureDirtyState();
            if (_oldFirstDisplayedScrollingRow != -1 && rowIndex <= _oldFirstDisplayedScrollingRow)
            {
                _oldFirstDisplayedScrollingRow--;
            }
        }

        public void CorrectRowIndexAfterInsertion(int rowIndex, int insertionCount)
        {
            EnsureDirtyState();
            if (_oldFirstDisplayedScrollingRow != -1 && rowIndex <= _oldFirstDisplayedScrollingRow)
            {
                _oldFirstDisplayedScrollingRow += insertionCount;
            }

            _rowInsertionOccurred = true;
            _oldNumDisplayedScrollingRows += insertionCount;
            _oldNumDisplayedFrozenRows += insertionCount;
        }

        private void SetOldValues()
        {
            _oldFirstDisplayedScrollingRow = _firstDisplayedScrollingRow;
            _oldFirstDisplayedScrollingCol = _firstDisplayedScrollingCol;
            _oldNumDisplayedFrozenRows = _numDisplayedFrozenRows;
            _oldNumDisplayedScrollingRows = _numDisplayedScrollingRows;
        }
    }
}

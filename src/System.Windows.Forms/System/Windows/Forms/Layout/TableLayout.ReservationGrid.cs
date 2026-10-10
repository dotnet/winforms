// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Layout;

internal partial class TableLayout
{
    // The ReservationGrid is used to track elements which span rows to prevent overlap.
    private sealed class ReservationGrid
    {
        private int _numColumns = 1;
        private readonly List<BitArray> _rows = [];
        private int _startIndex;

        public bool IsReserved(int column, int rowOffset)
        {
            // Check the logical offset before adding the head to avoid integer overflow.
            if (rowOffset >= _rows.Count - _startIndex)
            {
                return false;
            }

            int rowIndex = _startIndex + rowOffset;
            if (column >= _rows[rowIndex].Length)
            {
                return false;
            }

            return _rows[rowIndex][column];
        }

        public void Reserve(int column, int rowOffset)
        {
            Debug.Assert(!IsReserved(column, rowOffset), "we should not be reserving already reserved space.");
            int rowIndex = _startIndex + rowOffset;
            while (rowIndex >= _rows.Count)
            {
                _rows.Add(new BitArray(_numColumns));
            }

            // increase the length of the row if necessary
            if (column >= _rows[rowIndex].Length)
            {
                _rows[rowIndex].Length = column + 1;
                if (column >= _numColumns)
                {
                    _numColumns = column + 1;
                }
            }

            _rows[rowIndex][column] = true;
            Debug.Assert(IsReserved(column, rowOffset), "IsReserved/Reserved mismatch.");
        }

        // reserve all spaces taken by layoutInfo.Element, up till colStop
        public void ReserveAll(LayoutInfo layoutInfo, int rowStop, int colStop)
        {
            for (int rowOffset = 1; rowOffset < rowStop - layoutInfo.RowStart; rowOffset++)
            {
                for (int reservedCol = layoutInfo.ColumnStart; reservedCol < colStop; reservedCol++)
                {
                    Reserve(reservedCol, rowOffset);
                }
            }
        }

        public void AdvanceRow()
        {
            if (_startIndex >= _rows.Count)
            {
                return;
            }

            _startIndex++;

            if (_startIndex == _rows.Count)
            {
                _rows.Clear();
                _startIndex = 0;
            }
            else if (_startIndex >= _rows.Count - _startIndex)
            {
                // Compact once discarded rows are at least as numerous as live rows. This bounds retained
                // storage while keeping row advancement amortized constant time.
                _rows.RemoveRange(0, _startIndex);
                _startIndex = 0;
            }
        }
    }
}

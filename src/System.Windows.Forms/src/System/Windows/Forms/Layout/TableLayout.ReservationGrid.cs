﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Diagnostics;

namespace System.Windows.Forms.Layout
{
    internal partial class TableLayout
    {
        // The ReservationGrid is used to track elements which span rows to prevent overlap.
        private sealed class ReservationGrid
        {
            int _numColumns = 1;
            readonly ArrayList _rows = new ArrayList();

            public bool IsReserved(int column, int rowOffset)
            {
                if (rowOffset >= _rows.Count)
                {
                    return false;
                }

                if (column >= ((BitArray)_rows[rowOffset]).Length)
                {
                    return false;
                }

                return ((BitArray)_rows[rowOffset])[column];
            }

            public void Reserve(int column, int rowOffset)
            {
                Debug.Assert(!IsReserved(column, rowOffset), "we should not be reserving already reserved space.");
                while (rowOffset >= _rows.Count)
                {
                    _rows.Add(new BitArray(_numColumns));
                }

                //increase the length of the _rows[rowOffset] if necessary
                if (column >= ((BitArray)_rows[rowOffset]).Length)
                {
                    ((BitArray)_rows[rowOffset]).Length = column + 1;
                    if (column >= _numColumns)
                    {
                        _numColumns = column + 1;
                    }
                }

                ((BitArray)_rows[rowOffset])[column] = true;
                Debug.Assert(IsReserved(column, rowOffset), "IsReserved/Reserved mismatch.");
            }

            //reserve all spaces taken by layoutInfo.Element, up till colStop
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
                if (_rows.Count > 0)
                {
                    _rows.RemoveAt(0);
                }
            }
        }
    }
}

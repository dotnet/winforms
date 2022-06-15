// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Diagnostics;

namespace System.Windows.Forms
{
    public partial class DataGridViewRowCollection
    {
        private partial class RowComparer
        {
            private readonly DataGridView dataGridView;
            private readonly DataGridViewRowCollection dataGridViewRows;
            private readonly DataGridViewColumn dataGridViewSortedColumn;
            private readonly int sortedColumnIndex;
            private readonly IComparer customComparer;
            private readonly bool ascending;
            private static readonly ComparedObjectMax max = new ComparedObjectMax();

            public RowComparer(DataGridViewRowCollection dataGridViewRows, IComparer customComparer, bool ascending)
            {
                dataGridView = dataGridViewRows.DataGridView;
                this.dataGridViewRows = dataGridViewRows;
                dataGridViewSortedColumn = dataGridView.SortedColumn;
                if (dataGridViewSortedColumn is null)
                {
                    Debug.Assert(customComparer is not null);
                    sortedColumnIndex = -1;
                }
                else
                {
                    sortedColumnIndex = dataGridViewSortedColumn.Index;
                }

                this.customComparer = customComparer;
                this.ascending = ascending;
            }

            internal object GetComparedObject(int rowIndex)
            {
                if (dataGridView.NewRowIndex != -1)
                {
                    Debug.Assert(dataGridView.AllowUserToAddRowsInternal);
                    if (rowIndex == dataGridView.NewRowIndex)
                    {
                        return max;
                    }
                }

                if (customComparer is null)
                {
                    DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                    Debug.Assert(dataGridViewRow is not null);
                    Debug.Assert(sortedColumnIndex >= 0);
                    return dataGridViewRow.Cells[sortedColumnIndex].GetValueInternal(rowIndex);
                }
                else
                {
                    return dataGridViewRows[rowIndex]; // Unsharing compared rows!
                }
            }

            internal int CompareObjects(object value1, object value2, int rowIndex1, int rowIndex2)
            {
                if (value1 is ComparedObjectMax)
                {
                    return 1;
                }
                else if (value2 is ComparedObjectMax)
                {
                    return -1;
                }

                int result = 0;
                if (customComparer is null)
                {
                    if (!dataGridView.OnSortCompare(dataGridViewSortedColumn, value1, value2, rowIndex1, rowIndex2, out result))
                    {
                        if (!(value1 is IComparable) && !(value2 is IComparable))
                        {
                            if (value1 is null)
                            {
                                if (value2 is null)
                                {
                                    result = 0;
                                }
                                else
                                {
                                    result = 1;
                                }
                            }
                            else if (value2 is null)
                            {
                                result = -1;
                            }
                            else
                            {
                                result = Comparer.Default.Compare(value1.ToString(), value2.ToString());
                            }
                        }
                        else
                        {
                            result = Comparer.Default.Compare(value1, value2);
                        }

                        if (result == 0)
                        {
                            if (ascending)
                            {
                                result = rowIndex1 - rowIndex2;
                            }
                            else
                            {
                                result = rowIndex2 - rowIndex1;
                            }
                        }
                    }
                }
                else
                {
                    Debug.Assert(value1 is DataGridViewRow);
                    Debug.Assert(value2 is DataGridViewRow);
                    Debug.Assert(value1 is not null);
                    Debug.Assert(value2 is not null);
                    //

                    result = customComparer.Compare(value1, value2);
                }

                if (ascending)
                {
                    return result;
                }
                else
                {
                    return -result;
                }
            }
        }
    }
}

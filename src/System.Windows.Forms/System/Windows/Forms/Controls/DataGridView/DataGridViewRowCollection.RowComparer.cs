// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms;

public partial class DataGridViewRowCollection
{
    private partial class RowComparer
    {
        private readonly DataGridView _dataGridView;
        private readonly DataGridViewRowCollection _dataGridViewRows;
        private readonly DataGridViewColumn? _dataGridViewSortedColumn;
        private readonly int _sortedColumnIndex;
        private readonly IComparer _customComparer;
        private readonly bool _ascending;
        private static readonly ComparedObjectMax s_max = new();

        public RowComparer(DataGridViewRowCollection dataGridViewRows, IComparer customComparer, bool ascending)
        {
            _dataGridView = dataGridViewRows.DataGridView;
            _dataGridViewRows = dataGridViewRows;
            _dataGridViewSortedColumn = _dataGridView.SortedColumn;
            if (_dataGridViewSortedColumn is null)
            {
                Debug.Assert(customComparer is not null);
                _sortedColumnIndex = -1;
            }
            else
            {
                _sortedColumnIndex = _dataGridViewSortedColumn.Index;
            }

            _customComparer = customComparer;
            _ascending = ascending;
        }

        internal object? GetComparedObject(int rowIndex)
        {
            if (_dataGridView.NewRowIndex != -1)
            {
                Debug.Assert(_dataGridView.AllowUserToAddRowsInternal);
                if (rowIndex == _dataGridView.NewRowIndex)
                {
                    return s_max;
                }
            }

            if (_customComparer is null)
            {
                DataGridViewRow dataGridViewRow = _dataGridViewRows.SharedRow(rowIndex);
                Debug.Assert(dataGridViewRow is not null);
                Debug.Assert(_sortedColumnIndex >= 0);
                return dataGridViewRow.Cells[_sortedColumnIndex].GetValueInternal(rowIndex);
            }
            else
            {
                return _dataGridViewRows[rowIndex]; // Un-sharing compared rows!
            }
        }

        internal int CompareObjects(object? value1, object? value2, int rowIndex1, int rowIndex2)
        {
            if (value1 is ComparedObjectMax)
            {
                return 1;
            }
            else if (value2 is ComparedObjectMax)
            {
                return -1;
            }

            int result;
            if (_customComparer is null)
            {
                if (_dataGridViewSortedColumn is null)
                {
                    throw new InvalidOperationException();
                }

                if (!_dataGridView.OnSortCompare(
                    _dataGridViewSortedColumn,
                    value1,
                    value2,
                    rowIndex1,
                    rowIndex2,
                    out result))
                {
                    if (value1 is not IComparable && value2 is not IComparable)
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
                        if (_ascending)
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

                result = _customComparer.Compare(value1, value2);
            }

            if (_ascending)
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

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
        private class RowArrayList : ArrayList
        {
            private readonly DataGridViewRowCollection owner;
            private RowComparer rowComparer;

            public RowArrayList(DataGridViewRowCollection owner)
            {
                this.owner = owner;
            }

            public void CustomSort(RowComparer rowComparer)
            {
                Debug.Assert(rowComparer is not null);
                Debug.Assert(Count > 0);

                this.rowComparer = rowComparer;
                CustomQuickSort(0, Count - 1);
            }

            private void CustomQuickSort(int left, int right)
            {
                // Custom recursive QuickSort needed because of the notion of shared rows.
                // The indexes of the compared rows are required to do the comparisons.
                // For a study comparing the iterative and recursive versions of the QuickSort
                // see https://web.archive.org/web/20051125015050/http://www.mathcs.carleton.edu/courses/course_resources/cs227_w96/wightmaj/data.html
                // Is the recursive version going to cause trouble with large dataGridViews?
                do
                {
                    if (right - left < 2) // sort subarray of two elements
                    {
                        if (right - left > 0 && rowComparer.CompareObjects(rowComparer.GetComparedObject(left), rowComparer.GetComparedObject(right), left, right) > 0)
                        {
                            owner.SwapSortedRows(left, right);
                        }

                        return;
                    }

                    int k = (left + right) >> 1;
                    object x = Pivot(left, k, right);
                    int i = left + 1;
                    int j = right - 1;
                    do
                    {
                        while (k != i && rowComparer.CompareObjects(rowComparer.GetComparedObject(i), x, i, k) < 0)
                        {
                            i++;
                        }

                        while (k != j && rowComparer.CompareObjects(x, rowComparer.GetComparedObject(j), k, j) < 0)
                        {
                            j--;
                        }

                        Debug.Assert(i >= left && j <= right, "(i>=left && j<=right)  Sort failed - Is your IComparer bogus?");
                        if (i > j)
                        {
                            break;
                        }

                        if (i < j)
                        {
                            owner.SwapSortedRows(i, j);
                            if (i == k)
                            {
                                k = j;
                            }
                            else if (j == k)
                            {
                                k = i;
                            }
                        }

                        i++;
                        j--;
                    }
                    while (i <= j);

                    if (j - left <= right - i)
                    {
                        if (left < j)
                        {
                            CustomQuickSort(left, j);
                        }

                        left = i;
                    }
                    else
                    {
                        if (i < right)
                        {
                            CustomQuickSort(i, right);
                        }

                        right = j;
                    }
                }
                while (left < right);
            }

            private object Pivot(int left, int center, int right)
            {
                // find median-of-3 (left, center and right) and sort these 3 elements
                if (rowComparer.CompareObjects(rowComparer.GetComparedObject(left), rowComparer.GetComparedObject(center), left, center) > 0)
                {
                    owner.SwapSortedRows(left, center);
                }

                if (rowComparer.CompareObjects(rowComparer.GetComparedObject(left), rowComparer.GetComparedObject(right), left, right) > 0)
                {
                    owner.SwapSortedRows(left, right);
                }

                if (rowComparer.CompareObjects(rowComparer.GetComparedObject(center), rowComparer.GetComparedObject(right), center, right) > 0)
                {
                    owner.SwapSortedRows(center, right);
                }

                return rowComparer.GetComparedObject(center);
            }
        }
    }
}

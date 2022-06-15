// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms.Layout
{
    internal partial class TableLayout
    {
        // Private value type used by the Sort methods.
        private struct SorterObjectArray
        {
            private readonly object[] keys;
            private readonly IComparer comparer;

            internal SorterObjectArray(object[] keys, IComparer comparer)
            {
                if (comparer is null)
                {
                    comparer = Comparer.Default;
                }

                this.keys = keys;
                this.comparer = comparer;
            }

            internal void SwapIfGreaterWithItems(int a, int b)
            {
                if (a != b)
                {
                    try
                    {
                        if (comparer.Compare(keys[a], keys[b]) > 0)
                        {
                            object temp = keys[a];
                            keys[a] = keys[b];
                            keys[b] = temp;
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new ArgumentException();
                    }
                    catch (Exception)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            internal void QuickSort(int left, int right)
            {
                // Can use the much faster jit helpers for array access.
                do
                {
                    int i = left;
                    int j = right;

                    // pre-sort the low, middle (pivot), and high values in place.
                    // this improves performance in the face of already sorted data, or
                    // data that is made up of multiple sorted runs appended together.
                    int middle = GetMedian(i, j);
                    SwapIfGreaterWithItems(i, middle); // swap the low with the mid point
                    SwapIfGreaterWithItems(i, j);      // swap the low with the high
                    SwapIfGreaterWithItems(middle, j); // swap the middle with the high

                    object x = keys[middle];
                    do
                    {
                        // Add a try block here to detect IComparers (or their
                        // underlying IComparables, etc) that are bogus.
                        try
                        {
                            while (comparer.Compare(keys[i], x) < 0)
                            {
                                i++;
                            }

                            while (comparer.Compare(x, keys[j]) < 0)
                            {
                                j--;
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            throw new ArgumentException();
                        }
                        catch (Exception)
                        {
                            throw new InvalidOperationException();
                        }

                        if (i > j)
                        {
                            break;
                        }

                        if (i < j)
                        {
                            object key = keys[i];
                            keys[i] = keys[j];
                            keys[j] = key;
                        }

                        i++;
                        j--;
                    }
                    while (i <= j);
                    if (j - left <= right - i)
                    {
                        if (left < j)
                        {
                            QuickSort(left, j);
                        }

                        left = i;
                    }
                    else
                    {
                        if (i < right)
                        {
                            QuickSort(i, right);
                        }

                        right = j;
                    }
                }
                while (left < right);
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;

namespace System.Windows.Forms
{
    public partial class ListBox
    {
        public partial class SelectedIndexCollection : IList
        {
            /// <summary>
            ///  EntryEnumerator is an enumerator that will enumerate over
            ///  a given state mask.
            /// </summary>
            private class SelectedIndexEnumerator : IEnumerator
            {
                private readonly SelectedIndexCollection items;
                private int current;

                /// <summary>
                ///  Creates a new enumerator that will enumerate over the given state.
                /// </summary>
                public SelectedIndexEnumerator(SelectedIndexCollection items)
                {
                    this.items = items;
                    current = -1;
                }

                /// <summary>
                ///  Moves to the next element, or returns false if at the end.
                /// </summary>
                bool IEnumerator.MoveNext()
                {
                    if (current < items.Count - 1)
                    {
                        current++;
                        return true;
                    }
                    else
                    {
                        current = items.Count;
                        return false;
                    }
                }

                /// <summary>
                ///  Resets the enumeration back to the beginning.
                /// </summary>
                void IEnumerator.Reset()
                {
                    current = -1;
                }

                /// <summary>
                ///  Retrieves the current value in the enumerator.
                /// </summary>
                object IEnumerator.Current
                {
                    get
                    {
                        if (current == -1 || current == items.Count)
                        {
                            throw new InvalidOperationException(SR.ListEnumCurrentOutOfRange);
                        }

                        return items[current];
                    }
                }
            }
        }
    }
}

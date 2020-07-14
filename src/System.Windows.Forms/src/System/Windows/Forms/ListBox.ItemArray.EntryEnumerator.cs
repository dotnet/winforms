// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;

namespace System.Windows.Forms
{
    public partial class ListBox
    {
        internal partial class ItemArray
        {
            /// <summary>
            ///  EntryEnumerator is an enumerator that will enumerate over
            ///  a given state mask.
            /// </summary>
            private class EntryEnumerator : IEnumerator
            {
                private readonly ItemArray items;
                private readonly bool anyBit;
                private readonly int state;
                private int current;
                private readonly int version;

                /// <summary>
                ///  Creates a new enumerator that will enumerate over the given state.
                /// </summary>
                public EntryEnumerator(ItemArray items, int state, bool anyBit)
                {
                    this.items = items;
                    this.state = state;
                    this.anyBit = anyBit;
                    version = items.version;
                    current = -1;
                }

                /// <summary>
                ///  Moves to the next element, or returns false if at the end.
                /// </summary>
                bool IEnumerator.MoveNext()
                {
                    if (version != items.version)
                    {
                        throw new InvalidOperationException(SR.ListEnumVersionMismatch);
                    }

                    while (true)
                    {
                        if (current < items.count - 1)
                        {
                            current++;
                            if (anyBit)
                            {
                                if ((items.entries[current].state & state) != 0)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if ((items.entries[current].state & state) == state)
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            current = items.count;
                            return false;
                        }
                    }
                }

                /// <summary>
                ///  Resets the enumeration back to the beginning.
                /// </summary>
                void IEnumerator.Reset()
                {
                    if (version != items.version)
                    {
                        throw new InvalidOperationException(SR.ListEnumVersionMismatch);
                    }

                    current = -1;
                }

                /// <summary>
                ///  Retrieves the current value in the enumerator.
                /// </summary>
                object IEnumerator.Current
                {
                    get
                    {
                        if (current == -1 || current == items.count)
                        {
                            throw new InvalidOperationException(SR.ListEnumCurrentOutOfRange);
                        }

                        return items.entries[current].item;
                    }
                }
            }
        }
    }
}

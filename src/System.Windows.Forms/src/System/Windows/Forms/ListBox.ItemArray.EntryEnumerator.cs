// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
                private readonly ItemArray _items;
                private readonly bool _anyBit;
                private readonly int _state;
                private int _current;
                private readonly int _version;

                /// <summary>
                ///  Creates a new enumerator that will enumerate over the given state.
                /// </summary>
                public EntryEnumerator(ItemArray items, int state, bool anyBit)
                {
                    _items = items;
                    _state = state;
                    _anyBit = anyBit;
                    _version = items.Version;
                    _current = -1;
                }

                /// <summary>
                ///  Moves to the next element, or returns false if at the end.
                /// </summary>
                bool IEnumerator.MoveNext()
                {
                    if (_version != _items.Version)
                    {
                        throw new InvalidOperationException(SR.ListEnumVersionMismatch);
                    }

                    while (true)
                    {
                        if (_current < _items._count - 1)
                        {
                            _current++;
                            if (_anyBit)
                            {
                                if ((_items._entries[_current].state & _state) != 0)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if ((_items._entries[_current].state & _state) == _state)
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            _current = _items._count;
                            return false;
                        }
                    }
                }

                /// <summary>
                ///  Resets the enumeration back to the beginning.
                /// </summary>
                void IEnumerator.Reset()
                {
                    if (_version != _items.Version)
                    {
                        throw new InvalidOperationException(SR.ListEnumVersionMismatch);
                    }

                    _current = -1;
                }

                /// <summary>
                ///  Retrieves the current value in the enumerator.
                /// </summary>
                object IEnumerator.Current
                {
                    get
                    {
                        if (_current == -1 || _current == _items._count)
                        {
                            throw new InvalidOperationException(SR.ListEnumCurrentOutOfRange);
                        }

                        return _items._entries[_current].item;
                    }
                }
            }
        }
    }
}

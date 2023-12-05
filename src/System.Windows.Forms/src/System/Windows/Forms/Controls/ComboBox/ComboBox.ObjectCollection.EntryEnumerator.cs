// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms;

public partial class ComboBox
{
    public partial class ObjectCollection
    {
        /// <summary>
        ///  EntryEnumerator is an enumerator that will enumerate over
        ///  a given state mask.
        /// </summary>
        private class EntryEnumerator : IEnumerator
        {
            private readonly List<Entry> _entries;
            private int _current;

            /// <summary>
            ///  Creates a new enumerator that will enumerate over the given state.
            /// </summary>
            public EntryEnumerator(List<Entry> entries)
            {
                _entries = entries;
                _current = -1;
            }

            /// <summary>
            ///  Moves to the next element, or returns false if at the end.
            /// </summary>
            bool IEnumerator.MoveNext()
            {
                if (_current < _entries.Count - 1)
                {
                    _current++;
                    return true;
                }
                else
                {
                    _current = _entries.Count;
                    return false;
                }
            }

            /// <summary>
            ///  Resets the enumeration back to the beginning.
            /// </summary>
            void IEnumerator.Reset()
            {
                _current = -1;
            }

            /// <summary>
            ///  Retrieves the current value in the enumerator.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (_current == -1 || _current == _entries.Count)
                    {
                        throw new InvalidOperationException(SR.ListEnumCurrentOutOfRange);
                    }

                    return _entries[_current].Item;
                }
            }
        }
    }
}

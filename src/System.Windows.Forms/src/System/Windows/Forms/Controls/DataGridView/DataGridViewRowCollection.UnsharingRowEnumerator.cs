// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms;

public partial class DataGridViewRowCollection
{
    private class UnsharingRowEnumerator : IEnumerator
    {
        private readonly DataGridViewRowCollection _owner;
        private int _current;

        /// <summary>
        ///  Creates a new enumerator that will enumerate over the rows and un-share the accessed rows if needed.
        /// </summary>
        public UnsharingRowEnumerator(DataGridViewRowCollection owner)
        {
            _owner = owner;
            _current = -1;
        }

        /// <summary>
        ///  Moves to the next element, or returns false if at the end.
        /// </summary>
        bool IEnumerator.MoveNext()
        {
            if (_current < _owner.Count - 1)
            {
                _current++;
                return true;
            }
            else
            {
                _current = _owner.Count;
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
                if (_current == -1)
                {
                    throw new InvalidOperationException(SR.DataGridViewRowCollection_EnumNotStarted);
                }

                if (_current == _owner.Count)
                {
                    throw new InvalidOperationException(SR.DataGridViewRowCollection_EnumFinished);
                }

                return _owner[_current];
            }
        }
    }
}

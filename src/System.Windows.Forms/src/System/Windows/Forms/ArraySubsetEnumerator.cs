// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Diagnostics;

namespace System.Windows.Forms
{
    internal class ArraySubsetEnumerator : IEnumerator
    {
        private readonly object[] _array;
        private readonly int _total;
        private int _current;

        public ArraySubsetEnumerator(object[] array, int count)
        {
            Debug.Assert(count == 0 || array != null, "if array is null, count should be 0");
            Debug.Assert(array is null || count <= array.Length, "Trying to enumerate more than the array contains");
            _array = array;
            _total = count;
            _current = -1;
        }

        public bool MoveNext()
        {
            if (_current < _total - 1)
            {
                _current++;
                return true;
            }

            return false;
        }

        public void Reset() => _current = -1;

        public object Current => _current == -1 ? null : _array[_current];
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Windows.Forms
{
    internal class ArraySubsetEnumerator<T> : IEnumerator<T>
    {
        private readonly T[] _array;
        private readonly int _total;
        private int _current;

        public ArraySubsetEnumerator(T[] array, int count)
        {
            Debug.Assert(count == 0 || array != null, "if array is null, count should be 0");
            Debug.Assert(array == null || count <= array.Length, "Trying to enumerate more than the array contains");
            _array = array;
            _total = count;
            _current = -1;
        }

        void IDisposable.Dispose() { }

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

        public T Current => _current == -1 ? default : _array[_current];

        object IEnumerator.Current => Current;
    }
}

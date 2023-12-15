// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Drawing.Printing;

public partial class PrinterSettings
{
    private sealed class ArrayEnumerator : IEnumerator
    {
        private readonly object[] _array;
        private readonly int _endIndex;
        private int _index;
        private object? _item;

        public ArrayEnumerator(object[] array, int count)
        {
            _array = array;
            _endIndex = count;
        }

        public object? Current => _item;

        public bool MoveNext()
        {
            if (_index >= _endIndex)
                return false;
            _item = _array[_index++];
            return true;
        }

        public void Reset()
        {
            // Position enumerator before first item
            _index = 0;
            _item = null;
        }
    }
}

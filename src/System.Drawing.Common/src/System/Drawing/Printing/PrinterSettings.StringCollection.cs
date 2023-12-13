// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Drawing.Printing;

public partial class PrinterSettings
{
    public class StringCollection : ICollection
    {
        private string[] _array;

        /// <summary>
        ///  Initializes a new instance of the <see cref='StringCollection'/> class.
        /// </summary>
        public StringCollection(string[] array) => _array = array;

        /// <summary>
        ///  Gets a value indicating the number of strings.
        /// </summary>
        public int Count => _array.Length;

        /// <summary>
        ///  Gets the string with the specified index.
        /// </summary>
        public virtual string this[int index] => _array[index];

        public IEnumerator GetEnumerator() => new ArrayEnumerator(_array, Count);

        int ICollection.Count => Count;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        void ICollection.CopyTo(Array array, int index) => Array.Copy(_array, index, array, 0, _array.Length);

        public void CopyTo(string[] strings, int index) => Array.Copy(_array, index, strings, 0, _array.Length);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Add(string value)
        {
            string[] newArray = new string[Count + 1];
            ((ICollection)this).CopyTo(newArray, 0);
            newArray[Count] = value;
            _array = newArray;
            return Count;
        }
    }
}

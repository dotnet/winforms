// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Drawing.Printing;

public partial class PrinterSettings
{
    public class PaperSourceCollection : ICollection
    {
        private PaperSource[] _array;

        /// <summary>
        ///  Initializes a new instance of the <see cref='PaperSourceCollection'/> class.
        /// </summary>
        public PaperSourceCollection(PaperSource[] array)
        {
            _array = array;
        }

        /// <summary>
        ///  Gets a value indicating the number of paper sources.
        /// </summary>
        public int Count => _array.Length;

        /// <summary>
        ///  Gets the PaperSource with the specified index.
        /// </summary>
        public virtual PaperSource this[int index] => _array[index];

        public IEnumerator GetEnumerator() => new ArrayEnumerator(_array, Count);

        int ICollection.Count => Count;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        void ICollection.CopyTo(Array array, int index) => Array.Copy(_array, index, array, 0, _array.Length);

        public void CopyTo(PaperSource[] paperSources, int index) => Array.Copy(_array, index, paperSources, 0, _array.Length);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Add(PaperSource paperSource)
        {
            PaperSource[] newArray = new PaperSource[Count + 1];
            ((ICollection)this).CopyTo(newArray, 0);
            newArray[Count] = paperSource;
            _array = newArray;
            return Count;
        }
    }
}

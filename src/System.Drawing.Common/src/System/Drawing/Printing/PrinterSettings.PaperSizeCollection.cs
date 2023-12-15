// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Drawing.Printing;

public partial class PrinterSettings
{
    /// <summary>
    ///  Collection of <see cref="PaperSize"/> objects.
    /// </summary>
    public class PaperSizeCollection : ICollection
    {
        private PaperSize[] _array;

        /// <summary>
        ///  Initializes a new instance of the <see cref='PaperSizeCollection'/> class.
        /// </summary>
        public PaperSizeCollection(PaperSize[] array) => _array = array;

        /// <summary>
        ///  Gets a value indicating the number of paper sizes.
        /// </summary>
        public int Count => _array.Length;

        /// <summary>
        ///  Retrieves the <see cref="PaperSize"/> with the specified index.
        /// </summary>
        public virtual PaperSize this[int index] => _array[index];

        public IEnumerator GetEnumerator() => new ArrayEnumerator(_array, Count);

        int ICollection.Count => Count;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        void ICollection.CopyTo(Array array, int index) => Array.Copy(_array, index, array, 0, _array.Length);

        public void CopyTo(PaperSize[] paperSizes, int index) => Array.Copy(_array, index, paperSizes, 0, _array.Length);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Add(PaperSize paperSize)
        {
            PaperSize[] newArray = new PaperSize[Count + 1];
            ((ICollection)this).CopyTo(newArray, 0);
            newArray[Count] = paperSize;
            _array = newArray;
            return Count;
        }
    }
}

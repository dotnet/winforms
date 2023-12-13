// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Drawing.Printing;

public partial class PrinterSettings
{
    public class PrinterResolutionCollection : ICollection
    {
        private PrinterResolution[] _array;

        /// <summary>
        ///  Initializes a new instance of the <see cref='PrinterResolutionCollection'/> class.
        /// </summary>
        public PrinterResolutionCollection(PrinterResolution[] array) => _array = array;

        /// <summary>
        ///  Gets a value indicating the number of available printer resolutions.
        /// </summary>
        public int Count => _array.Length;

        /// <summary>
        ///  Retrieves the PrinterResolution with the specified index.
        /// </summary>
        public virtual PrinterResolution this[int index] => _array[index];

        public IEnumerator GetEnumerator() => new ArrayEnumerator(_array, Count);

        int ICollection.Count => Count;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        void ICollection.CopyTo(Array array, int index) => Array.Copy(_array, index, array, 0, _array.Length);

        public void CopyTo(PrinterResolution[] printerResolutions, int index) => Array.Copy(_array, index, printerResolutions, 0, _array.Length);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Add(PrinterResolution printerResolution)
        {
            PrinterResolution[] newArray = new PrinterResolution[Count + 1];
            ((ICollection)this).CopyTo(newArray, 0);
            newArray[Count] = printerResolution;
            _array = newArray;
            return Count;
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Drawing.Printing;

public partial class PrinterSettings
{
    public class StringCollection : ICollection, IEnumerable<string>
    {
        private readonly List<string> _list;

        /// <summary>
        ///  Initializes a new instance of the <see cref='StringCollection'/> class.
        /// </summary>
        public StringCollection(string[] array) => _list = [..array];

        /// <summary>
        ///  Gets a value indicating the number of strings.
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        ///  Gets the string with the specified index.
        /// </summary>
        public virtual string this[int index] => _list[index];

        public IEnumerator GetEnumerator() => ((IEnumerable)_list).GetEnumerator();

        int ICollection.Count => Count;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        void ICollection.CopyTo(Array array, int index) => ((ICollection)_list).CopyTo(array, index);

        public void CopyTo(string[] strings, int index) => ((ICollection)_list).CopyTo(strings, index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Add(string value)
        {
            _list.Add(value);
            return _list.Count;
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator() => _list.GetEnumerator();
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal sealed class GridEntryCollection : GridItemCollection
    {
        private readonly GridEntry _owner;

        public GridEntryCollection(GridEntry owner, GridEntry[] entries) : base(entries)
        {
            _owner = owner;
        }

        public void AddRange(GridEntry[] value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Debug.Assert(_entries != null, "Entries is initialized in the base class constructor.");
            var newArray = new GridEntry[_entries.Length + value.Length];
            _entries.CopyTo(newArray, 0);
            value.CopyTo(newArray, _entries.Length);
            _entries = newArray;
        }

        public void Clear() => _entries = Array.Empty<GridEntry>();

        public void CopyTo(Array dest, int index) => _entries.CopyTo(dest, index);

        internal GridEntry GetEntry(int index) => (GridEntry)_entries[index];

        internal int GetEntry(GridEntry child) => Array.IndexOf(_entries, child);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_owner != null)
                {
                    for (int i = 0; i < _entries.Length; i++)
                    {
                        if (_entries[i] != null)
                        {
                            ((GridEntry)_entries[i]).Dispose();
                            _entries[i] = null;
                        }
                    }

                    _entries = Array.Empty<GridEntry>();
                }
            }
        }

        ~GridEntryCollection() => Dispose(false);
    }
}

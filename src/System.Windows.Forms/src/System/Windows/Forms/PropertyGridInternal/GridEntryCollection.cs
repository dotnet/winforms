// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms.PropertyGridInternal {
    using System.Runtime.InteropServices;

    using System.Diagnostics;

    using System;
    using System.Collections;
    using System.Reflection;

    using System.Drawing.Design;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;
    using System.Drawing;
    using Microsoft.Win32;

    internal class GridEntryCollection : GridItemCollection {

        private GridEntry owner;
        
        public GridEntryCollection(GridEntry owner, GridEntry[] entries) : base(entries) {
            this.owner = owner;
        }
        
        public void AddRange(GridEntry[] value) {
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }
            if (entries != null) {
                GridEntry[] newArray = new GridEntry[entries.Length + value.Length];
                entries.CopyTo(newArray, 0);
                value.CopyTo(newArray, entries.Length);
                entries = newArray;
            }
            else {
                entries = (GridEntry[])value.Clone();
            }
        }                                       
                                       
        public void Clear() {
            entries = new GridEntry[0];            
        }
        
        public void CopyTo(Array dest, int index) {
            entries.CopyTo(dest, index);
        }
        
        internal GridEntry GetEntry(int index) {
            return (GridEntry)entries[index];
        }

         internal int GetEntry(GridEntry child) {
            return Array.IndexOf(entries, child);   
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (owner != null && entries != null) {
                    for (int i = 0; i < entries.Length; i++) {
                        if (entries[i] != null) {
                            ((GridEntry)entries[i]).Dispose();
                            entries[i] = null;
                        }
                    }
                    entries = new GridEntry[0];
                }
            }
        }

        ~GridEntryCollection() {
            Dispose(false);
        }
    }
}

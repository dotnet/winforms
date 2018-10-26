// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Design;    
    using System.Globalization;
    using System.Windows.Forms.Layout;
    using System.Reflection;

    /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="StyleCollection"]/*' />
    [Editor("System.Windows.Forms.Design.StyleCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
    public abstract class TableLayoutStyleCollection : IList {
        private IArrangedElement _owner;
        private ArrayList _innerList = new ArrayList();

        internal TableLayoutStyleCollection(IArrangedElement owner) {
            _owner = owner;
        }

        internal IArrangedElement Owner {
            get { return _owner; }
        }
        internal virtual string PropertyName {
            get { return null; }
        }

        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="StyleCollection.IList.Add"]/*' />
        /// <internalonly/>
        int IList.Add(object style) {
            EnsureNotOwned((TableLayoutStyle)style);
            ((TableLayoutStyle)style).Owner = this.Owner;
            int index = _innerList.Add(style);
            PerformLayoutIfOwned();
            return index;
        }

        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="StyleCollection.Add2"]/*' />
        /// <internalonly/>
        public int Add(TableLayoutStyle style) {
            return ((IList)this).Add(style);
        }

       
        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="StyleCollection.IList.Insert"]/*' />
        /// <internalonly/>
        void IList.Insert(int index, object style) {
            EnsureNotOwned((TableLayoutStyle)style);
            ((TableLayoutStyle)style).Owner = this.Owner;
            _innerList.Insert(index, style);
            PerformLayoutIfOwned();
            
        }

        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="StyleCollection.IList.this"]/*' />
        /// <internalonly/>
        object IList.this[int index] {
            get { return _innerList[index]; }
            set {
                TableLayoutStyle style = (TableLayoutStyle) value;
                EnsureNotOwned(style);
                style.Owner = this.Owner;
                _innerList[index] = style;
                PerformLayoutIfOwned();
            }
        }

        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="StyleCollection.IList.this2"]/*' />
        /// <internalonly/>
        public TableLayoutStyle this[int index] {
            get { return (TableLayoutStyle)((IList)this)[index]; }
            set { ((IList)this)[index] = value; }
        }

        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="StyleCollection.IList.Remove"]/*' />
        /// <internalonly/>
        void IList.Remove(object style) {
            ((TableLayoutStyle)style).Owner = null;
            _innerList.Remove(style); 
            PerformLayoutIfOwned();
        }

        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="StyleCollection.Clear"]/*' />
        public void Clear() {
            foreach(TableLayoutStyle style in _innerList) {
                style.Owner = null;
            }
            _innerList.Clear();
            PerformLayoutIfOwned();
        }
        
        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="StyleCollection.RemoveAt"]/*' />
        public void RemoveAt(int index) {
            TableLayoutStyle style = (TableLayoutStyle) _innerList[index];
            style.Owner = null;
            _innerList.RemoveAt(index);
            PerformLayoutIfOwned();
        }

        // These methods just forward to _innerList.
        bool IList.Contains(object style) { return _innerList.Contains(style); }
        int IList.IndexOf(object style) { return _innerList.IndexOf(style); }

        // These properties / methods just forward to _innerList and are item-type agnostic.
        bool IList.IsFixedSize { get {return _innerList.IsFixedSize;} }
        bool IList.IsReadOnly { get {return _innerList.IsReadOnly;} }
        void ICollection.CopyTo(System.Array array, int startIndex) { _innerList.CopyTo(array, startIndex); }
        
        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="StyleCollection.Count"]/*' />
        public int Count { get { return _innerList.Count; }}
        bool ICollection.IsSynchronized { get{ return _innerList.IsSynchronized; }}
        object ICollection.SyncRoot { get { return _innerList.SyncRoot; }}
        IEnumerator IEnumerable.GetEnumerator() { return _innerList.GetEnumerator(); }

        private void EnsureNotOwned(TableLayoutStyle style) {
            if(style.Owner != null) {
                throw new ArgumentException(string.Format(SR.OnlyOneControl, style.GetType().Name), "style");
            }
        }
        internal void EnsureOwnership(IArrangedElement owner) {
            _owner = owner;
            for (int i = 0; i < Count; i++) {
               this[i].Owner = owner;
            }
        }
        private void PerformLayoutIfOwned() {
            if (this.Owner != null) {
                LayoutTransaction.DoLayout(this.Owner, this.Owner, PropertyName);
            }
        }
    }
}


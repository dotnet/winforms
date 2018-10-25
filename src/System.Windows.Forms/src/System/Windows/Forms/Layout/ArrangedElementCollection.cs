// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Layout {

    using System.Collections;
    using System.Security.Permissions;

    /// <include file='doc\Layout.uex' path='docs/doc[@for="Layout.ArrangedElementCollection"]/*' />
    public class ArrangedElementCollection : IList
    {
        internal static ArrangedElementCollection Empty = new ArrangedElementCollection(0);
        
        // We wrap an ArrayList rather than inherit from CollectionBase because we
        // do not want to break binary compatibility with ControlCollection.
        private ArrayList _innerList;

        // Internal constructor prevents externals from getting a hold of one of these.
        // We'll open this up in Orcas.
        internal ArrangedElementCollection() {
            _innerList = new ArrayList(4);
        }

        internal ArrangedElementCollection(ArrayList innerList) {
            _innerList = innerList;
        }

        private ArrangedElementCollection(int size) {
            _innerList = new ArrayList(size);
        }
        
        internal ArrayList InnerList { 
            get { return _innerList; }
        }

        internal virtual IArrangedElement this[int index] {
            get { return (IArrangedElement) InnerList[index]; }
        }

        /// <include file='doc\ArrangedElementCollection.uex' path='docs/doc[@for="ArrangedElementCollection.Equals"]/*' />
        public override bool Equals(object obj) {
            ArrangedElementCollection other = obj as ArrangedElementCollection;

            if (other == null || Count != other.Count) {
                return false;
            }

            for (int i = 0; i < Count; i++) {
                if (this.InnerList[i] != other.InnerList[i]) {
                    return false;
                }
            }
            return true;
        }

        // Required if you override Equals.
        /// <include file='doc\ArrangedElementCollection.uex' path='docs/doc[@for="ArrangedElementCollection.GetHashCode"]/*' />
        public override int GetHashCode() { return base.GetHashCode(); }

        // Repositions a element in this list.
        internal void MoveElement(IArrangedElement element, int fromIndex, int toIndex) {
            int delta = toIndex - fromIndex;
        
            switch (delta) {
                case -1:
                case 1:
                    // simple swap
                    this.InnerList[fromIndex] = this.InnerList[toIndex];
                    break;
        
                default:
                    int start = 0;
                    int dest = 0;
        
                    // which direction are we moving?
                    if (delta > 0) {
                        // shift down by the delta to open the new spot
                        start = fromIndex + 1;
                        dest = fromIndex;
                    }
                    else {
                        // shift up by the delta to open the new spot
                        start = toIndex;
                        dest = toIndex + 1;
        
                        // make it positive
                        delta = -delta;
                    }
                    Copy(this, start, this, dest, delta);
                    break;
            }
            this.InnerList[toIndex] = element;
        }

        private static void Copy (ArrangedElementCollection sourceList, int sourceIndex, ArrangedElementCollection destinationList, int destinationIndex, int  length) {
            if(sourceIndex < destinationIndex) {
                // We need to copy from the back forward to prevent overwrite if source and
                // destination lists are the same, so we need to flip the source/dest indices
                // to point at the end of the spans to be copied.
                sourceIndex = sourceIndex + length;
                destinationIndex = destinationIndex + length;
                
                for(;length > 0; length--) {
                    destinationList.InnerList[--destinationIndex] = sourceList.InnerList[--sourceIndex];
                }
            } else {
                for(;length > 0; length--) {
                    destinationList.InnerList[destinationIndex++] = sourceList.InnerList[sourceIndex++];
                }
            }
        }
        
        #region IList Members
        void IList.Clear() { InnerList.Clear(); }
        bool IList.IsFixedSize { get { return InnerList.IsFixedSize; }}
        bool IList.Contains(object value) { return InnerList.Contains(value); }
        /// <include file='doc\ArrangedElementCollection.uex' path='docs/doc[@for="ArrangedElementCollection.IsReadOnly"]/*' />
        public virtual bool IsReadOnly { get { return InnerList.IsReadOnly; }}
        void IList.RemoveAt(int index) { InnerList.RemoveAt(index); }
        void IList.Remove(object value) { InnerList.Remove(value); }
        int IList.Add(object value) { return InnerList.Add(value); }
        int IList.IndexOf(object value) { return InnerList.IndexOf(value); }
        void IList.Insert(int index, object value) { throw new NotSupportedException(); /* InnerList.Insert(index, value); */ }

        /// <include file='doc\ArrangedElementCollection.uex' path='docs/doc[@for="ArrangedElementCollection.IList.this"]/*' />
        /// <internalonly/>
        object IList.this[int index] {
            get { return InnerList[index]; }            
            set { throw new NotSupportedException(); }
        }
        #endregion

        #region ICollection Members
        /// <include file='doc\ArrangedElementCollection.uex' path='docs/doc[@for="ArrangedElementCollection.Count"]/*' />
        public virtual int Count { 
            get { return InnerList.Count; }
        }
        object ICollection.SyncRoot { get { return InnerList.SyncRoot; }}
        /// <include file='doc\ArrangedElementCollection.uex' path='docs/doc[@for="ArrangedElementCollection.CopyTo"]/*' />
        public void CopyTo(Array array, int index) { InnerList.CopyTo(array, index); }
        bool ICollection.IsSynchronized { get { return InnerList.IsSynchronized; }}
        #endregion

        #region IEnumerable Members
        /// <include file='doc\ArrangedElementCollection.uex' path='docs/doc[@for="ArrangedElementCollection.GetEnumerator"]/*' />
        public virtual IEnumerator GetEnumerator() { return InnerList.GetEnumerator(); }
        #endregion
    }
}

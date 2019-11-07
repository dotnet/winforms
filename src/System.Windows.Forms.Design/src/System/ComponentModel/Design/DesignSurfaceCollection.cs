// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  Provides a read-only collection of design surfaces.
    /// </summary>
    public sealed class DesignSurfaceCollection : ICollection
    {
        private readonly DesignerCollection _designers;

        /// <summary>
        ///  Initializes a new instance of the DesignSurfaceCollection class
        /// </summary>
        internal DesignSurfaceCollection(DesignerCollection designers)
        {
            _designers = designers ?? new DesignerCollection(null);
        }

        /// <summary>
        ///  Gets number of design surfaces in the collection.
        /// </summary>
        public int Count => _designers.Count;

        /// <summary>
        ///  Gets or sets the document at the specified index.
        /// </summary>
        public DesignSurface this[int index]
        {
            get
            {
                IDesignerHost host = _designers[index];
                if (host.GetService(typeof(DesignSurface)) is DesignSurface surface)
                {
                    return surface;
                }

                throw new NotSupportedException();
            }
        }

        /// <summary>
        ///  Creates and retrieves a new enumerator for this collection.
        /// </summary>
        public IEnumerator GetEnumerator() => new DesignSurfaceEnumerator(_designers.GetEnumerator());

        int ICollection.Count => Count;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => null;

        void ICollection.CopyTo(Array array, int index)
        {
            foreach (DesignSurface surface in this)
            {
                array.SetValue(surface, index++);
            }
        }

        public void CopyTo(DesignSurface[] array, int index) => ((ICollection)this).CopyTo(array, index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        ///  Enumerator that performs the conversion from designer host 
        ///  to design surface.
        /// </summary>
        private class DesignSurfaceEnumerator : IEnumerator
        {
            private readonly IEnumerator _designerEnumerator;

            internal DesignSurfaceEnumerator(IEnumerator designerEnumerator)
            {
                _designerEnumerator = designerEnumerator;
            }

            public object Current
            {
                get
                {
                    IDesignerHost host = (IDesignerHost)_designerEnumerator.Current;
                    if (host.GetService(typeof(DesignSurface)) is DesignSurface surface)
                    {
                        return surface;
                    }

                    throw new NotSupportedException();
                }
            }

            public bool MoveNext() => _designerEnumerator.MoveNext();

            public void Reset() => _designerEnumerator.Reset();
        }
    }
}

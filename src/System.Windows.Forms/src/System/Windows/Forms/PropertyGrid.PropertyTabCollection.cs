// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace System.Windows.Forms
{
    public partial class PropertyGrid
    {
        public class PropertyTabCollection : ICollection
        {
            private readonly PropertyGrid _ownerPropertyGrid;

            internal PropertyTabCollection(PropertyGrid ownerPropertyGrid)
            {
                // Probably we should throw an ArgumentNullException(nameof(ownerPropertyGrid))
                // to make sure _ownerPropertyGrid never be null
                _ownerPropertyGrid = ownerPropertyGrid;
            }

            /// <summary>
            ///  Retrieves the number of member attributes.
            /// </summary>
            public int Count
            {
                get
                {
                    if (_ownerPropertyGrid is null)
                    {
                        return 0;
                    }

                    return _ownerPropertyGrid._tabs.Count;
                }
            }

            object ICollection.SyncRoot => this;

            bool ICollection.IsSynchronized => false;

            /// <summary>
            ///  Retrieves the member attribute with the specified index.
            /// </summary>
            public PropertyTab this[int index]
            {
                get
                {
                    if (_ownerPropertyGrid is null)
                    {
                        throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                    }

                    return _ownerPropertyGrid._tabs[index].Tab;
                }
            }

            public void AddTabType(Type propertyTabType)
            {
                if (_ownerPropertyGrid is null)
                {
                    throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                }

                _ownerPropertyGrid.AddTab(propertyTabType, PropertyTabScope.Global);
            }

            public void AddTabType(Type propertyTabType, PropertyTabScope tabScope)
            {
                if (_ownerPropertyGrid is null)
                {
                    throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                }

                _ownerPropertyGrid.AddTab(propertyTabType, tabScope);
            }

            /// <summary>
            ///  Clears the tabs of the given scope or smaller.
            ///  tabScope must be PropertyTabScope.Component or PropertyTabScope.Document.
            /// </summary>
            public void Clear(PropertyTabScope tabScope)
            {
                if (_ownerPropertyGrid is null)
                {
                    throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                }

                _ownerPropertyGrid.ClearTabs(tabScope);
            }

            void ICollection.CopyTo(Array dest, int index)
            {
                if (_ownerPropertyGrid is null)
                {
                    return;
                }

                if (_ownerPropertyGrid._tabs.Count > 0)
                {
                    Array.Copy(
                        _ownerPropertyGrid._tabs.Select(i => i.Tab).ToArray(),
                        0,
                        dest,
                        index,
                        _ownerPropertyGrid._tabs.Count);
                }
            }

            /// <summary>
            ///  Creates and retrieves a new enumerator for this collection.
            /// </summary>
            public IEnumerator GetEnumerator()
            {
                if (_ownerPropertyGrid is null)
                {
                    return Array.Empty<PropertyTab>().GetEnumerator();
                }

                return _ownerPropertyGrid._tabs.Select(i => i.Tab).GetEnumerator();
            }

            public void RemoveTabType(Type propertyTabType)
            {
                if (_ownerPropertyGrid is null)
                {
                    throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                }

                _ownerPropertyGrid.RemoveTab(propertyTabType);
            }
        }
    }
}

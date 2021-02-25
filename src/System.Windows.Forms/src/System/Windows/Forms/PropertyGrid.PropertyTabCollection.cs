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
            private readonly PropertyGrid _owner;

            internal PropertyTabCollection(PropertyGrid owner)
            {
                _owner = owner;
            }

            /// <summary>
            ///  Retrieves the number of member attributes.
            /// </summary>
            public int Count
            {
                get
                {
                    if (_owner is null)
                    {
                        return 0;
                    }
                    return _owner._viewTabs.Length;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            ///  Retrieves the member attribute with the specified index.
            /// </summary>
            public PropertyTab this[int index]
            {
                get
                {
                    if (_owner is null)
                    {
                        throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                    }
                    return _owner._viewTabs[index];
                }
            }

            public void AddTabType(Type propertyTabType)
            {
                if (_owner is null)
                {
                    throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                }
                _owner.AddTab(propertyTabType, PropertyTabScope.Global);
            }

            public void AddTabType(Type propertyTabType, PropertyTabScope tabScope)
            {
                if (_owner is null)
                {
                    throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                }
                _owner.AddTab(propertyTabType, tabScope);
            }

            /// <summary>
            ///  Clears the tabs of the given scope or smaller.
            ///  tabScope must be PropertyTabScope.Component or PropertyTabScope.Document.
            /// </summary>
            public void Clear(PropertyTabScope tabScope)
            {
                if (_owner is null)
                {
                    throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                }
                _owner.ClearTabs(tabScope);
            }

            void ICollection.CopyTo(Array dest, int index)
            {
                if (_owner is null)
                {
                    return;
                }
                if (_owner._viewTabs.Length > 0)
                {
                    System.Array.Copy(_owner._viewTabs, 0, dest, index, _owner._viewTabs.Length);
                }
            }
            /// <summary>
            ///  Creates and retrieves a new enumerator for this collection.
            /// </summary>
            public IEnumerator GetEnumerator()
            {
                if (_owner is null)
                {
                    return Array.Empty<PropertyTab>().GetEnumerator();
                }

                return _owner._viewTabs.GetEnumerator();
            }

            public void RemoveTabType(Type propertyTabType)
            {
                if (_owner is null)
                {
                    throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                }
                _owner.RemoveTab(propertyTabType);
            }
        }
    }
}

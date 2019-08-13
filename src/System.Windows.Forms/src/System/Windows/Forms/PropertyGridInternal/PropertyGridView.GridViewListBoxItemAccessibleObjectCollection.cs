// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        private class GridViewListBoxItemAccessibleObjectCollection : Hashtable
        {
            private readonly GridViewListBox _owningGridViewListBox;

            public GridViewListBoxItemAccessibleObjectCollection(GridViewListBox owningGridViewListBox)
            {
                _owningGridViewListBox = owningGridViewListBox;
            }

            public override object this[object key]
            {
                get
                {
                    if (!ContainsKey(key))
                    {
                        var itemAccessibleObject = new GridViewListBoxItemAccessibleObject(_owningGridViewListBox, key);
                        base[key] = itemAccessibleObject;
                    }

                    return base[key];
                }

                set
                {
                    base[key] = value;
                }
            }
        }

    }
}

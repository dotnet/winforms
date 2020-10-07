// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Runtime.Serialization;

namespace System.Windows.Forms
{
    public partial class ComboBox
    {
        internal class ComboBoxItemAccessibleObjectCollection : Hashtable
        {
            private readonly ComboBox _owningComboBoxBox;
            private readonly ObjectIDGenerator _idGenerator = new ObjectIDGenerator();

            public ComboBoxItemAccessibleObjectCollection(ComboBox owningComboBoxBox)
            {
                _owningComboBoxBox = owningComboBoxBox;
            }

            public override object this[object key]
            {
                get
                {
                    int id = GetId(key);
                    if (!ContainsKey(id))
                    {
                        var itemAccessibleObject = new ComboBoxItemAccessibleObject(_owningComboBoxBox, key);
                        base[id] = itemAccessibleObject;
                    }

                    return base[id];
                }

                set
                {
                    int id = GetId(key);
                    base[id] = value;
                }
            }

            public int GetId(object item)
            {
                return unchecked((int)_idGenerator.GetId(item, out var _));
            }
        }
    }
}

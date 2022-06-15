// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static System.Windows.Forms.ComboBox.ObjectCollection;

namespace System.Windows.Forms
{
    public partial class ComboBox
    {
        internal class ComboBoxItemAccessibleObjectCollection : Dictionary<Entry, ComboBoxItemAccessibleObject>
        {
            private readonly ComboBox _owningComboBox;

            public ComboBoxItemAccessibleObjectCollection(ComboBox owningComboBox)
            {
                _owningComboBox = owningComboBox;
            }

            public ComboBoxItemAccessibleObject GetComboBoxItemAccessibleObject(Entry key)
            {
                if (!ContainsKey(key))
                {
                    Add(key, new ComboBoxItemAccessibleObject(_owningComboBox, key));
                }

                return this[key];
            }
        }
    }
}

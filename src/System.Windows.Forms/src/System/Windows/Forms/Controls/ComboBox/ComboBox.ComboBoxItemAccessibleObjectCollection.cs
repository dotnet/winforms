// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static System.Windows.Forms.ComboBox.ObjectCollection;

namespace System.Windows.Forms;

public partial class ComboBox
{
    internal class ComboBoxItemAccessibleObjectCollection : Dictionary<Entry, ComboBoxItemAccessibleObject>
    {
        private readonly ComboBoxAccessibleObject _owningComboBoxAccessibleObject;

        public ComboBoxItemAccessibleObjectCollection(ComboBoxAccessibleObject owningComboBoxAccessibleObject)
        {
            _owningComboBoxAccessibleObject = owningComboBoxAccessibleObject;
        }

        public ComboBoxItemAccessibleObject GetComboBoxItemAccessibleObject(Entry key)
        {
            if (!ContainsKey(key) && _owningComboBoxAccessibleObject.TryGetOwnerAs(out ComboBox? owner))
            {
                Add(key, new ComboBoxItemAccessibleObject(owner, key));
            }

            return this[key];
        }
    }
}

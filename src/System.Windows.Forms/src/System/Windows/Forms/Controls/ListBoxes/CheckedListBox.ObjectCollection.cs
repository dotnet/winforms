// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class CheckedListBox
{
    public new class ObjectCollection : ListBox.ObjectCollection
    {
        private readonly CheckedListBox _owner;

        public ObjectCollection(CheckedListBox owner) : base(owner)
        {
            _owner = owner;
        }

        /// <summary>
        ///  Lets the user add an item to the listbox with the given initial value
        ///  for the Checked portion of the item.
        /// </summary>
        public int Add(object item, bool isChecked)
        {
            return Add(item, isChecked ? CheckState.Checked : CheckState.Unchecked);
        }

        /// <summary>
        ///  Lets the user add an item to the listbox with the given initial value
        ///  for the Checked portion of the item.
        /// </summary>
        public int Add(object item, CheckState check)
        {
            // validate the enum that's passed in here
            //
            // Valid values are 0-2 inclusive.
            SourceGenerated.EnumValidator.Validate(check, nameof(check));

            int index = Add(item);
            _owner.SetItemCheckState(index, check);

            return index;
        }
    }
}

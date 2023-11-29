// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class CheckedListBox
{
    internal class CheckedListBoxAccessibleObject : ListBoxAccessibleObject
    {
        public CheckedListBoxAccessibleObject(CheckedListBox owner) : base(owner)
        { }

        private protected override ListBoxItemAccessibleObject CreateItemAccessibleObject(ListBox listBox, ItemArray.Entry item)
            => new CheckedListBoxItemAccessibleObject((CheckedListBox)listBox, item, this);
    }
}

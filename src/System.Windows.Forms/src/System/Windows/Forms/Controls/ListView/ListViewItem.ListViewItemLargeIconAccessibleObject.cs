// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ListViewItem
{
    internal class ListViewItemLargeIconAccessibleObject : ListViewItemWithImageAccessibleObject
    {
        public ListViewItemLargeIconAccessibleObject(ListViewItem owningItem) : base(owningItem)
        {
        }

        protected override View View => View.LargeIcon;
    }
}

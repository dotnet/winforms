// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ListViewItem;

namespace System.Windows.Forms.Tests
{
    public class ListViewItem_ListViewItemDetailsAccessibleObjectTests
    {
        [WinFormsFact]
        public void ListViewItemDetailsAccessibleObject_Ctor_OwnerListViewItemCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ListViewItemDetailsAccessibleObject(null));
        }
    }
}

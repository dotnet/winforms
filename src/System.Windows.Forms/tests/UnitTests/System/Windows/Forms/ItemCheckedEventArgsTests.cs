// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class ItemCheckedEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ItemCheckedEventArgs_Ctor_NullListViewItem_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ItemCheckedEventArgs(null));
        }

        [WinFormsFact]
        public void Ctor_ListViewItem()
        {
            ListViewItem listViewItem = new();
            var e = new ItemCheckedEventArgs(listViewItem);
            Assert.Equal(listViewItem, e.Item);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListViewItemSelectionChangedEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_ListViewItem_Int_Bool_TestData()
        {
            yield return new object[] { null, -2, false };
            yield return new object[] { new ListViewItem(), -1, true };
            yield return new object[] { new ListViewItem(), 0, true };
        }

        [Theory]
        [MemberData(nameof(Ctor_ListViewItem_Int_Bool_TestData))]
        public void Ctor_ListViewItem_Int_Bool(ListViewItem item, int itemIndex, bool isSelected)
        {
            var e = new ListViewItemSelectionChangedEventArgs(item, itemIndex, isSelected);
            Assert.Equal(item, e.Item);
            Assert.Equal(itemIndex, e.ItemIndex);
            Assert.Equal(isSelected, e.IsSelected);
        }
    }
}

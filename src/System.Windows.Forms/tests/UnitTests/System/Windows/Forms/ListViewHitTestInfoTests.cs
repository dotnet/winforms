// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListViewHitTestInfoTests
    {
        public static IEnumerable<object[]> Ctor_ListViewItem_ListViewSubItem_ListViewHitTestLocations_TestData()
        {
            yield return new object[] { null, null, ListViewHitTestLocations.None };
            yield return new object[] { new ListViewItem(), new ListViewItem.ListViewSubItem(), (ListViewHitTestLocations)(ListViewHitTestLocations.None - 1) };
        }

        [Theory]
        [MemberData(nameof(Ctor_ListViewItem_ListViewSubItem_ListViewHitTestLocations_TestData))]
        public void ListViewHitTestInfo_Ctor_ListViewItem_ListViewSubItem_ListViewHitTestLocations(ListViewItem hitItem, ListViewItem.ListViewSubItem hitSubItem, ListViewHitTestLocations hitTestLocations)
        {
            var info = new ListViewHitTestInfo(hitItem, hitSubItem, hitTestLocations);
            Assert.Equal(hitItem, info.Item);
            Assert.Equal(hitSubItem, info.SubItem);
            Assert.Equal(hitTestLocations, info.Location);
        }
    }
}

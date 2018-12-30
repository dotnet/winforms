// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class SelectedGridItemChangedEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_GridItem_Object_TestData()
        {
            yield return new object[] { null, null };
            yield return new object[] { new SubGridItem(), new SubGridItem() };
        }

        [Theory]
        [MemberData(nameof(Ctor_GridItem_Object_TestData))]
        public void Ctor_GridItem_GridItem(GridItem oldSel, GridItem newSel)
        {
            var e = new SelectedGridItemChangedEventArgs(oldSel, newSel);
            Assert.Equal(oldSel, e.OldSelection);
            Assert.Equal(newSel, e.NewSelection);
        }

        private class SubGridItem : GridItem
        {
            public override GridItemCollection GridItems { get; }

            public override GridItemType GridItemType { get; }

            public override string Label { get; }

            public override GridItem Parent { get; }

            public override PropertyDescriptor PropertyDescriptor { get; }

            public override object Value { get; }

            public override bool Select() => false;
        }
    }
}

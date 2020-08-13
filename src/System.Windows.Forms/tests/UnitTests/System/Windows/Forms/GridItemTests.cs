// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class GridItemTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void GridItem_Expandable_Get_ReturnsFalse()
        {
            var item = new SubGridItem();
            Assert.False(item.Expandable);
        }

        [Fact]
        public void GridItem_Expanded_Get_ReturnsFalse()
        {
            var item = new SubGridItem();
            Assert.False(item.Expanded);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void GridItem_Expanded_Set_ThrowsNotSupportedException(bool value)
        {
            var item = new SubGridItem();
            Assert.Throws<NotSupportedException>(() => item.Expanded = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void GridItem_Tag_Set_GetReturnsExpected(object value)
        {
            var item = new SubGridItem
            {
                Tag = value
            };
            Assert.Same(value, item.Tag);

            // Set same.
            item.Tag = value;
            Assert.Same(value, item.Tag);
        }

        private class SubGridItem : GridItem
        {
            public override GridItemCollection GridItems => GridItemCollection.Empty;

            public override GridItemType GridItemType => GridItemType.Property;

            public override string Label => "label";

            public override GridItem Parent => null;

            public override PropertyDescriptor PropertyDescriptor => null;

            public override object Value => "value";

            public override bool Select() => true;
        }
    }
}

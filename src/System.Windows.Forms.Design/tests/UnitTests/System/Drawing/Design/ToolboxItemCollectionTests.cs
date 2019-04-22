// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Drawing.Design.Tests
{
    public class ToolboxItemCollectionTests
    {
        [Fact]
        public void ToolboxItemCollection_Ctor_ToolboxItemArray()
        {
            var item = new ToolboxItem();
            var collection = new ToolboxItemCollection(new ToolboxItem[] { item });
            Assert.Same(item, Assert.Single(collection));
            Assert.Same(item, collection[0]);
            Assert.True(collection.Contains(item));
            Assert.Equal(0, collection.IndexOf(item));
        }

        [Fact]
        public void ToolboxItemCollection_Ctor_NullToolboxItemArray_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("c", () => new ToolboxItemCollection((ToolboxItem[])null));
        }

        [Fact]
        public void ToolboxItemCollection_Ctor_ToolboxItemCollection()
        {
            var item = new ToolboxItem();
            var value = new ToolboxItemCollection(new ToolboxItem[] { item });
            var collection = new ToolboxItemCollection(value);
            Assert.Same(item, Assert.Single(collection));
            Assert.Same(item, collection[0]);
            Assert.True(collection.Contains(item));
            Assert.Equal(0, collection.IndexOf(item));
        }

        [Fact]
        public void ToolboxItemCollection_Ctor_NullToolboxItemCollection_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("c", () => new ToolboxItemCollection((ToolboxItemCollection)null));
        }

        [Fact]
        public void ToolboxItemCollection_Contains_NoSuchValue_ReturnsFalse()
        {
            var item = new ToolboxItem();
            var collection = new ToolboxItemCollection(new ToolboxItem[] { item });
            Assert.False(collection.Contains(new ToolboxItem { DisplayName = "Other" }));
            Assert.False(collection.Contains(null));
        }

        [Fact]
        public void ToolboxItemCollection_IndexOf_NoSuchValue_ReturnsNegativeOne()
        {
            var item = new ToolboxItem();
            var collection = new ToolboxItemCollection(new ToolboxItem[] { item });
            Assert.Equal(-1, collection.IndexOf(new ToolboxItem { DisplayName = "Other" }));
            Assert.Equal(-1, collection.IndexOf(null));
        }

        [Fact]
        public void ToolboxItemCollection_CopyTo_Invoke_Success()
        {
            var item = new ToolboxItem();
            var collection = new ToolboxItemCollection(new ToolboxItem[] { item });

            var array = new ToolboxItem[3];
            collection.CopyTo(array, 1);
            Assert.Equal(new ToolboxItem[] { null, item, null }, array);
        }
    }
}

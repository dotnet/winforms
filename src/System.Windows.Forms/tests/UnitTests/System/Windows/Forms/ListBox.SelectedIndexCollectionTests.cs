// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListBoxSelectedIndexCollectionTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ListBoxSelectedIndexCollection_Ctor_ListBox()
        {
            using var owner = new ListBox();
            var collection = new ListBox.SelectedIndexCollection(owner);
            Assert.Equal(0, collection.Count);
            Assert.True(collection.IsReadOnly);
        }

        [WinFormsFact]
        public void ListBoxSelectedIndexCollection_Ctor_NullOwner_ThrowsArgumentNullException()
        {
            using var owner = new ListBox();
            Assert.Throws<ArgumentNullException>("owner", () => new ListBox.SelectedIndexCollection(null));
        }

        [WinFormsFact]
        public void ListBoxSelectedIndexCollection_ICollection_Properties_GetReturnsExpected()
        {
            using var owner = new ListBox();
            ICollection collection = new ListBox.SelectedIndexCollection(owner);
            Assert.Equal(0, collection.Count);
            Assert.True(collection.IsSynchronized);
            Assert.Same(collection, collection.SyncRoot);
        }

        [WinFormsFact]
        public void ListBoxSelectedIndexCollection_IList_Properties_GetReturnsExpected()
        {
            using var owner = new ListBox();
            IList collection = new ListBox.SelectedIndexCollection(owner);
            Assert.Equal(0, collection.Count);
            Assert.True(collection.IsFixedSize);
            Assert.True(collection.IsReadOnly);
            Assert.True(collection.IsSynchronized);
            Assert.Same(collection, collection.SyncRoot);
        }

        [WinFormsTheory]
        [InlineData(-1, null)]
        [InlineData(0, null)]
        [InlineData(1, null)]
        [InlineData(-1, 1)]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(-1, "1")]
        [InlineData(0, "1")]
        [InlineData(1, "1")]
        public void ListBoxSelectedIndexCollection_IListItem_Set_ThrowsNotSupportedException(int index, object value)
        {
            using var owner = new ListBox();
            IList collection = new ListBox.SelectedIndexCollection(owner);
            Assert.Throws<NotSupportedException>(() => collection[index] = value);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData(1)]
        [InlineData("1")]
        public void ListBoxSelectedIndexCollection_IListAdd_Invoke_ThrowsNotSupportedException(object value)
        {
            using var owner = new ListBox();
            IList collection = new ListBox.SelectedIndexCollection(owner);
            Assert.Throws<NotSupportedException>(() => collection.Add(value));
        }

        [WinFormsFact]
        public void ListBoxSelectedIndexCollection_IListClear_Invoke_ThrowsNotSupportedException()
        {
            using var owner = new ListBox();
            IList collection = new ListBox.SelectedIndexCollection(owner);
            Assert.Throws<NotSupportedException>(() => collection.Clear());
        }

        [WinFormsTheory]
        [InlineData(-1, null)]
        [InlineData(0, null)]
        [InlineData(1, null)]
        [InlineData(-1, 1)]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(-1, "1")]
        [InlineData(0, "1")]
        [InlineData(1, "1")]
        public void ListBoxSelectedIndexCollection_IListInsert_Invoke_ThrowsNotSupportedException(int index, object value)
        {
            using var owner = new ListBox();
            IList collection = new ListBox.SelectedIndexCollection(owner);
            Assert.Throws<NotSupportedException>(() => collection.Insert(index, value));
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData(1)]
        [InlineData("1")]
        public void ListBoxSelectedIndexCollection_IListRemove_Invoke_ThrowsNotSupportedException(object value)
        {
            using var owner = new ListBox();
            IList collection = new ListBox.SelectedIndexCollection(owner);
            Assert.Throws<NotSupportedException>(() => collection.Remove(value));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ListBoxSelectedIndexCollection_IListRemoveAt_Invoke_ThrowsNotSupportedException(int index)
        {
            using var owner = new ListBox();
            IList collection = new ListBox.SelectedIndexCollection(owner);
            Assert.Throws<NotSupportedException>(() => collection.RemoveAt(index));
        }
    }
}

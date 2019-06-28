// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class BaseCollectionTests
    {
        [Fact]
        public void Ctor_Default()
        {
            var collection = new BaseCollection();
            Assert.False(collection.IsReadOnly);
            Assert.False(collection.IsSynchronized);
            Assert.Same(collection, collection.SyncRoot);
        }

        [Fact]
        public void Count_GetWithList_ReturnsExpected()
        {
            var collection = new SubCollection(new ArrayList { 1, 2, 3 });
            Assert.Equal(3, collection.Count);
        }

        [Fact]
        public void Count_GetDefault_ThrowsNullReferenceException()
        {
            var collection = new BaseCollection();
            Assert.Throws<NullReferenceException>(() => collection.Count);
        }

        [Fact]
        public void CopyTo_InvokeWithList_Success()
        {
            var collection = new SubCollection(new ArrayList { 1, 2, 3 });
            var array = new object[] { 0, 0, 0, 0, 4 };
            collection.CopyTo(array, 1);
            Assert.Equal(new object[] { 0, 1, 2, 3, 4 }, array);
        }

        [Fact]
        public void CopyTo_InvokeDefault_ThrowsNullReferenceException()
        {
            var collection = new BaseCollection();
            Assert.Throws<NullReferenceException>(() => collection.CopyTo(Array.Empty<object>(), 0));
        }

        [Fact]
        public void GetEnumerator_InvokeWithList_Success()
        {
            var collection = new SubCollection(new ArrayList { 1, 2, 3 });
            IEnumerator enumerator = collection.GetEnumerator();
            Assert.True(enumerator.MoveNext());
            Assert.Equal(1, enumerator.Current);
        }

        [Fact]
        public void GetEnumerator_InvokeDefault_ThrowsNullReferenceException()
        {
            var collection = new BaseCollection();
            Assert.Throws<NullReferenceException>(() => collection.CopyTo(Array.Empty<object>(), 0));
        }

        private class SubCollection : BaseCollection
        {
            private readonly ArrayList _list;

            public SubCollection(ArrayList list) => _list = list;

            protected override ArrayList List => _list;
        }
    }
}

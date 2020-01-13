// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class DesignerActionListCollectionTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void DesignerActionListCollection_Ctor_Default()
        {
            var collection = new DesignerActionListCollection();
            Assert.Empty(collection);
        }

        public static IEnumerable<object[]> Ctor_DesignerActionListArray_TestData()
        {
            yield return new object[] { Array.Empty<DesignerActionList>() };
            yield return new object[] { new DesignerActionList[] { new DesignerActionList(null), null } };
        }

        [Theory]
        [MemberData(nameof(Ctor_DesignerActionListArray_TestData))]
        public void DesignerActionListCollection_Ctor_DesignerActionListArray(DesignerActionList[] value)
        {
            var collection = new DesignerActionListCollection(value);
            Assert.Equal(value, collection.Cast<object>());
        }

        [Fact]
        public void DesignerActionListCollection_Ctor_NullValue_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("value", () => new DesignerActionListCollection(null));
        }

        [Fact]
        public void DesignerActionListCollection_Add_DesignerActionList_Success()
        {
            var collection = new DesignerActionListCollection();

            var value1 = new DesignerActionList(null);
            collection.Add(value1);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, collection[0]);
            Assert.True(collection.Contains(value1));
            Assert.Equal(0, collection.IndexOf(value1));

            var value2 = new DesignerActionList(null);
            collection.Add(value2);
            Assert.Equal(new object[] { value1, value2 }, collection.Cast<object>());
            Assert.True(collection.Contains(value2));
            Assert.Equal(1, collection.IndexOf(value2));

            collection.Add(null);
            Assert.Equal(new object[] { value1, value2, null }, collection.Cast<object>());
            Assert.True(collection.Contains(null));
            Assert.Equal(2, collection.IndexOf(null));
        }

        [Theory]
        [MemberData(nameof(Ctor_DesignerActionListArray_TestData))]
        public void DesignerActionListCollection_AddRange_DesignerActionListArray_Success(DesignerActionList[] value)
        {
            var collection = new DesignerActionListCollection();
            collection.AddRange(value);
            Assert.Equal(value, collection.Cast<object>());

            // Add again.
            collection.AddRange(value);
            Assert.Equal(value.Concat(value), collection.Cast<object>());
        }

        [Theory]
        [MemberData(nameof(Ctor_DesignerActionListArray_TestData))]
        public void DesignerActionListCollection_AddRange_DesignerActionListCollection_Success(DesignerActionList[] value)
        {
            var collection = new DesignerActionListCollection();
            collection.AddRange(new DesignerActionListCollection(value));
            Assert.Equal(value, collection.Cast<object>());

            // Add again.
            collection.AddRange(new DesignerActionListCollection(value));
            Assert.Equal(value.Concat(value), collection.Cast<object>());
        }

        [Fact]
        public void DesignerActionListCollection_AddRange_NullValue_ThrowsArgumentNullException()
        {
            var collection = new DesignerActionListCollection();
            Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((DesignerActionList[])null));
            Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((DesignerActionListCollection)null));
        }

        [Fact]
        public void DesignerActionListCollection_Insert_DesignerActionList_Success()
        {
            var collection = new DesignerActionListCollection();

            var value1 = new DesignerActionList(null);
            collection.Insert(0, value1);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, collection[0]);
            Assert.True(collection.Contains(value1));
            Assert.Equal(0, collection.IndexOf(value1));

            var value2 = new DesignerActionList(null);
            collection.Insert(0, value2);
            Assert.Equal(new object[] { value2, value1 }, collection.Cast<object>());
            Assert.True(collection.Contains(value2));
            Assert.Equal(0, collection.IndexOf(value2));

            collection.Insert(1, null);
            Assert.Equal(new object[] { value2, null, value1 }, collection.Cast<object>());
            Assert.True(collection.Contains(null));
            Assert.Equal(1, collection.IndexOf(null));
        }

        [Fact]
        public void DesignerActionListCollection_Remove_Invoke_Success()
        {
            var collection = new DesignerActionListCollection();
            var value = new DesignerActionList(null);
            collection.Add(value);
            Assert.Same(value, Assert.Single(collection));

            collection.Remove(value);
            Assert.Empty(collection);
            Assert.False(collection.Contains(value));
            Assert.Equal(-1, collection.IndexOf(value));

            collection.Add(null);
            collection.Remove(null);
            Assert.Empty(collection);
            Assert.False(collection.Contains(null));
            Assert.Equal(-1, collection.IndexOf(null));
        }

        [Fact]
        public void DesignerActionListCollection_Item_Set_GetReturnsExpected()
        {
            var collection = new DesignerActionListCollection();
            var value1 = new DesignerActionList(null);
            var value2 = new DesignerActionList(null);
            collection.Add(value1);
            Assert.Same(value1, Assert.Single(collection));

            collection[0] = value2;
            Assert.Same(value2, Assert.Single(collection));
            Assert.Same(value2, collection[0]);
            Assert.False(collection.Contains(value1));
            Assert.Equal(-1, collection.IndexOf(value1));
            Assert.True(collection.Contains(value2));
            Assert.Equal(0, collection.IndexOf(value2));

            collection[0] = null;
            Assert.Null(Assert.Single(collection));
            Assert.Null(collection[0]);
            Assert.False(collection.Contains(value2));
            Assert.Equal(-1, collection.IndexOf(value2));
            Assert.True(collection.Contains(null));
            Assert.Equal(0, collection.IndexOf(null));
        }

        [Fact]
        public void DesignerActionListCollection_CopyTo_Invoke_Success()
        {
            var collection = new DesignerActionListCollection();
            var value = new DesignerActionList(null);
            collection.Add(value);

            var array = new DesignerActionList[3];
            collection.CopyTo(array, 1);
            Assert.Equal(new DesignerActionList[] { null, value, null }, array);
        }

        [Fact]
        public void DesignerActionListCollection_Contains_NoSuchValue_ReturnsFalse()
        {
            var collection = new DesignerActionListCollection();
            var value = new DesignerActionList(null);
            collection.Add(value);

            Assert.False(collection.Contains(new DesignerActionList(null)));
            Assert.False(collection.Contains(null));
        }

        [Fact]
        public void DesignerActionListCollection_IndexOf_NoSuchValue_ReturnsNegativeOne()
        {
            var collection = new DesignerActionListCollection();
            var value = new DesignerActionList(null);
            collection.Add(value);

            Assert.Equal(-1, collection.IndexOf(new DesignerActionList(null)));
            Assert.Equal(-1, collection.IndexOf(null));
        }

        [Fact]
        public void DesignerActionListCollection_Clear_Success()
        {
            var collection = new DesignerActionListCollection();
            var value = new DesignerActionList(null);
            collection.Add(value);

            collection.Clear();
            Assert.Empty(collection);

            // Clear again.
            collection.Clear();
            Assert.Empty(collection);
        }
    }
}

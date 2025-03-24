// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

namespace System.ComponentModel.Design.Tests;

public class DesignerVerbCollectionTests
{
    [Fact]
    public void DesignerVerbCollection_Ctor_Default()
    {
        DesignerVerbCollection collection = new();
        Assert.Empty(collection);
    }

    public static IEnumerable<object[]> Ctor_DesignerVerbArray_TestData()
    {
        yield return new object[] { Array.Empty<DesignerVerb>() };
        yield return new object[] { new DesignerVerb[] { new(null, null), null } };
    }

    [Theory]
    [MemberData(nameof(Ctor_DesignerVerbArray_TestData))]
    public void DesignerVerbCollection_Ctor_DesignerVerbArray(DesignerVerb[] value)
    {
        DesignerVerbCollection collection = new(value);
        Assert.Equal(value, collection.Cast<object>());
    }

    [Fact]
    public void DesignerVerbCollection_Ctor_NullValue_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("value", () => new DesignerVerbCollection(null));
    }

    [Fact]
    public void DesignerVerbCollection_Add_DesignerVerb_Success()
    {
        DesignerVerbCollection collection = new();

        DesignerVerb value1 = new(null, null);
        collection.Add(value1);
        Assert.Same(value1, Assert.Single(collection));
        Assert.Same(value1, collection[0]);
        Assert.True(collection.Contains(value1));
        Assert.Equal(0, collection.IndexOf(value1));

        DesignerVerb value2 = new(null, null);
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
    [MemberData(nameof(Ctor_DesignerVerbArray_TestData))]
    public void DesignerVerbCollection_AddRange_DesignerVerbArray_Success(DesignerVerb[] value)
    {
        DesignerVerbCollection collection = new();
        collection.AddRange(value);
        Assert.Equal(value, collection.Cast<object>());

        // Add again.
        collection.AddRange(value);
        Assert.Equal(value.Concat(value), collection.Cast<object>());
    }

    [Theory]
    [MemberData(nameof(Ctor_DesignerVerbArray_TestData))]
    public void DesignerVerbCollection_AddRange_DesignerVerbCollection_Success(DesignerVerb[] value)
    {
        DesignerVerbCollection collection = new();
        collection.AddRange(new DesignerVerbCollection(value));
        Assert.Equal(value, collection.Cast<object>());

        // Add again.
        collection.AddRange(new DesignerVerbCollection(value));
        Assert.Equal(value.Concat(value), collection.Cast<object>());
    }

    [Fact]
    public void DesignerVerbCollection_AddRange_NullValue_ThrowsArgumentNullException()
    {
        DesignerVerbCollection collection = new();
        Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((DesignerVerb[])null));
        Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((DesignerVerbCollection)null));
    }

    [Fact]
    public void DesignerVerbCollection_Insert_DesignerVerb_Success()
    {
        DesignerVerbCollection collection = new();

        DesignerVerb value1 = new(null, null);
        collection.Insert(0, value1);
        Assert.Same(value1, Assert.Single(collection));
        Assert.Same(value1, collection[0]);
        Assert.True(collection.Contains(value1));
        Assert.Equal(0, collection.IndexOf(value1));

        DesignerVerb value2 = new(null, null);
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
    public void DesignerVerbCollection_Remove_Invoke_Success()
    {
        DesignerVerbCollection collection = new();
        DesignerVerb value = new(null, null);
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
    public void DesignerVerbCollection_Item_Set_GetReturnsExpected()
    {
        DesignerVerbCollection collection = new();
        DesignerVerb value1 = new(null, null);
        DesignerVerb value2 = new(null, null);
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
    public void DesignerVerbCollection_CopyTo_Invoke_Success()
    {
        DesignerVerbCollection collection = new();
        DesignerVerb value = new(null, null);
        collection.Add(value);

        var array = new DesignerVerb[3];
        collection.CopyTo(array, 1);
        Assert.Equal(new DesignerVerb[] { null, value, null }, array);
    }

    [Fact]
    public void DesignerVerbCollection_Contains_NoSuchValue_ReturnsFalse()
    {
        DesignerVerbCollection collection = new();
        DesignerVerb value = new(null, null);
        collection.Add(value);

        Assert.False(collection.Contains(new DesignerVerb(null, null)));
        Assert.False(collection.Contains(null));
    }

    [Fact]
    public void DesignerVerbCollection_IndexOf_NoSuchValue_ReturnsNegativeOne()
    {
        DesignerVerbCollection collection = new();
        DesignerVerb value = new(null, null);
        collection.Add(value);

        Assert.Equal(-1, collection.IndexOf(new DesignerVerb(null, null)));
        Assert.Equal(-1, collection.IndexOf(null));
    }

    [Fact]
    public void DesignerVerbCollection_Clear_Success()
    {
        DesignerVerbCollection collection = new();
        DesignerVerb value = new(null, null);
        collection.Add(value);

        collection.Clear();
        Assert.Empty(collection);

        // Clear again.
        collection.Clear();
        Assert.Empty(collection);
    }
}

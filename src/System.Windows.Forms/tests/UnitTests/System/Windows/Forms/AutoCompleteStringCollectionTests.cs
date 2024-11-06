// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Tests;

public class AutoCompleteStringCollectionTests
{
    [Fact]
    public void Ctor_Default()
    {
        AutoCompleteStringCollection collection = [];
        Assert.Empty(collection);
        Assert.False(collection.IsReadOnly);
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_AddRange_Invoke_Success()
    {
        AutoCompleteStringCollection collection = [];
        string[] values = ["1", "2", "3"];
        collection.AddRange(values);

        foreach (string s in values)
        {
            Assert.True(collection.Contains(s));
        }
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_AddRange_NullValue_ThrowsArgumentNullException()
    {
        AutoCompleteStringCollection collection = [];
        Assert.Throws<ArgumentNullException>("value", () => collection.AddRange(null));
    }

#nullable enable
    [WinFormsFact]
    public void AutoCompleteStringCollection_AddRange_NullValues_Nop()
    {
        AutoCompleteStringCollection collection = [null!];
        Assert.Empty(collection);
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_Add_NullValue_ThrowsArgumentNullException()
    {
        AutoCompleteStringCollection collection = [];
        Assert.Throws<ArgumentNullException>("value", () => collection.AddRange(null!));
    }
#nullable disable

    [WinFormsFact]
    public void AutoCompleteStringCollection_Contains_Invoke_ReturnsExpected()
    {
        AutoCompleteStringCollection collection = [];
        string s = "value";
        collection.Add(s);

        Assert.True(collection.Contains(s));
        Assert.False(collection.Contains("anotherValue"));
        Assert.False(collection.Contains(null));
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_Contains_Empty_ReturnsFalse()
    {
        AutoCompleteStringCollection collection = [];

        Assert.False(collection.Contains("value"));
        Assert.False(collection.Contains(null));
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_IListContains_Invoke_ReturnsExpected()
    {
        IList collection = new AutoCompleteStringCollection();
        string s = "value";
        collection.Add(s);

        Assert.True(collection.Contains(s));
        Assert.False(collection.Contains("anotherValue"));
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_IListContains_Empty_ReturnsFalse()
    {
        IList collection = new AutoCompleteStringCollection();

        Assert.False(collection.Contains("value"));
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_IndexOf_Invoke_ReturnsExpected()
    {
        AutoCompleteStringCollection collection = [];
        string s = "value";
        collection.Add(s);

        Assert.Equal(0, collection.IndexOf(s));
        Assert.Equal(-1, collection.IndexOf("anotherValue"));
        Assert.Equal(-1, collection.IndexOf(null));
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_IndexOf_Empty_ReturnsFalse()
    {
        AutoCompleteStringCollection collection = [];

        Assert.Equal(-1, collection.IndexOf("value"));
        Assert.Equal(-1, collection.IndexOf(null));
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_IListIndexOf_Invoke_ReturnsExpected()
    {
        IList collection = new AutoCompleteStringCollection();
        string s = "value";
        collection.Add(s);

        Assert.Equal(0, collection.IndexOf(s));
        Assert.Equal(-1, collection.IndexOf("anotherValue"));
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_IListIndexOf_Empty_ReturnsMinusOne()
    {
        IList collection = new AutoCompleteStringCollection();

        Assert.Equal(-1, collection.IndexOf("value"));
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_Insert_String_Success()
    {
        AutoCompleteStringCollection collection = [];
        string s = "value1";
        collection.Add("value2");
        collection.Insert(1, s);
        Assert.Equal(2, collection.Count);
        Assert.Same(s, collection[1]);
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_Insert_AlreadyInOtherCollection_GetReturnsExpected()
    {
        AutoCompleteStringCollection collection = ["value1"];

        AutoCompleteStringCollection otherCollection = [];

        string s = "value2";
        otherCollection.Add(s);

        // The string appears to belong to two collections.
        collection.Insert(0, s);
        Assert.Same(s, collection[0]);
        Assert.Equal(s, collection[0]);
        Assert.Equal(s, Assert.Single(otherCollection));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void AutoCompleteStringCollection_Insert_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
    {
        AutoCompleteStringCollection collection = [];
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, "value"));
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_IListInsert_String_Success()
    {
        IList collection = new AutoCompleteStringCollection();
        string s = "value1";
        collection.Add("value2");
        collection.Insert(1, s);
        Assert.Equal(2, collection.Count);
        Assert.Same(s, collection[1]);
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_IListInsert_NullItem_ThrowsArgumentOutOfRangeException()
    {
        IList collection = new AutoCompleteStringCollection();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(-1, null));
        Assert.Empty(collection);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void AutoCompleteStringCollection_IListInsert_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
    {
        IList collection = new AutoCompleteStringCollection();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, "value"));
    }

#nullable enable
    [WinFormsFact]
    public void AutoCompleteStringCollection_IListInsert_NullItem_Nop()
    {
        IList collection = new AutoCompleteStringCollection();
        collection.Insert(0, null);
        Assert.Empty(collection);
    }
#nullable disable

    [WinFormsFact]
    public void AutoCompleteStringCollection_Remove_String_Success()
    {
        AutoCompleteStringCollection collection = ["value"];

        // Remove null.
        collection.Remove(null);
        Assert.Same("value", Assert.Single(collection));

        collection.Remove("value");
        Assert.Empty(collection);

        // Remove again.
        collection.Remove("value");
        Assert.Empty(collection);
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_IListRemove_String_Success()
    {
        IList collection = new AutoCompleteStringCollection();
        collection.Add("value");

        collection.Remove("value");
        Assert.Empty(collection);

        // Remove again.
        collection.Remove("value");
        Assert.Empty(collection);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("text")]
    public void AutoCompleteStringCollection_IListRemove_InvalidItem_Nop(object value)
    {
        IList collection = new AutoCompleteStringCollection();
        collection.Add(value);

        collection.Remove(value);
        Assert.Empty(collection);
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_CopyTo_NonEmpty_Success()
    {
        AutoCompleteStringCollection collection = ["value"];

        string[] array = ["1", "2", "3"];
        collection.CopyTo(array, 1);
        Assert.Equal(new string[] { "1", "value", "3" }, array);
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_CopyTo_Empty_Nop()
    {
        AutoCompleteStringCollection collection = [];
        string[] array = ["1", "2", "3"];
        collection.CopyTo(array, 0);
        Assert.Equal(new string[] { "1", "2", "3" }, array);
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_GetEnumerator_InvokeEmpty_ReturnsExpected()
    {
        AutoCompleteStringCollection collection = [];

        IEnumerator enumerator = collection.GetEnumerator();
        for (int i = 0; i < 2; i++)
        {
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Move again.
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Reset.
            enumerator.Reset();
        }
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_GetEnumerator_InvokeNotEmpty_ReturnsExpected()
    {
        AutoCompleteStringCollection collection = ["2"];

        IEnumerator enumerator = collection.GetEnumerator();
        for (int i = 0; i < 2; i++)
        {
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Equal("2", enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Move again.
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Reset.
            enumerator.Reset();
        }
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_IListGetEnumerator_InvokeEmpty_ReturnsExpected()
    {
        IList collection = new AutoCompleteStringCollection();

        IEnumerator enumerator = collection.GetEnumerator();
        for (int i = 0; i < 2; i++)
        {
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Move again.
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Reset.
            enumerator.Reset();
        }
    }

    [WinFormsFact]
    public void AutoCompleteStringCollection_IListGetEnumerator_InvokeNotEmpty_ReturnsExpected()
    {
        IList collection = new AutoCompleteStringCollection();
        collection.Add("2");

        IEnumerator enumerator = collection.GetEnumerator();
        for (int i = 0; i < 2; i++)
        {
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Equal("2", enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Move again.
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Reset.
            enumerator.Reset();
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.misc.Tests;

public class CollectionHelperTests
{
    [Fact]
    public void HashtableCopyTo_throws_when_target_is_null()
    {
        Dictionary<string, string> source = [];
        Assert.Throws<ArgumentNullException>(() => source.HashtableCopyTo(target: null, index: 0));
    }

    [Fact]
    public void HashtableCopyTo_throws_when_target_rank_greather_than_one()
    {
        Dictionary<string, string> source = [];
        object[,] target = new object[3, 3];

        Assert.Throws<ArgumentException>(() => source.HashtableCopyTo(target, index: 0));
    }

    [Fact]
    public void HashtableCopyTo_throws_when_index_less_than_zero()
    {
        Dictionary<string, string> source = [];
        object[] target = new object[3];

        Assert.Throws<ArgumentOutOfRangeException>(() => source.HashtableCopyTo(target, index: -2));
    }

    [Fact]
    public void HashtableCopyTo_throws_when_index_greather_than_target_length()
    {
        Dictionary<string, string> source = [];
        object[] target = new object[3];

        Assert.Throws<ArgumentOutOfRangeException>(() => source.HashtableCopyTo(target, index: 5));
    }

    [Fact]
    public void HashtableCopyTo_throws_when_target_lowerbound_is_non_zero()
    {
        Dictionary<string, string> source = [];
        var target = Array.CreateInstance(typeof(double), [3], [2]);

        Assert.Throws<ArgumentException>(() => source.HashtableCopyTo(target, index: 0));
    }

    [Fact]
    public void HashtableCopyTo_throws_when_target_too_small()
    {
        Dictionary<string, string> source = new()
        {
            { "key-one", "value-one" },
            { "key-two", "value-two" },
        };
        object[] target = new object[3];

        Assert.Throws<ArgumentException>(() => source.HashtableCopyTo(target, index: 2));
    }

    [Fact]
    public void HashtableCopyTo_successfully_copies_to_KeyValuePair_array()
    {
        Dictionary<string, string> source = new()
        {
            { "key-one", "value-one" },
            { "key-two", "value-two" },
        };
        var target = new KeyValuePair<string, string>[2];

        source.HashtableCopyTo(target, index: 0);

        var firstTargetItem = target[0];
        Assert.Equal(typeof(KeyValuePair<string, string>), firstTargetItem.GetType());
        Assert.Equal("key-one", firstTargetItem.Key);
        Assert.Equal("value-one", firstTargetItem.Value);

        var secondTargetItem = target[1];
        Assert.Equal(typeof(KeyValuePair<string, string>), secondTargetItem.GetType());
        Assert.Equal("key-two", secondTargetItem.Key);
        Assert.Equal("value-two", secondTargetItem.Value);
    }

    [Fact]
    public void HashtableCopyTo_successfully_copies_to_DictionaryEntry_array()
    {
        Dictionary<string, string> source = new()
        {
            { "key-one", "value-one" },
            { "key-two", "value-two" },
        };
        var target = new DictionaryEntry[2];

        source.HashtableCopyTo(target, index: 0);

        var firstTargetItem = target[0];
        Assert.Equal(typeof(DictionaryEntry), firstTargetItem.GetType());
        Assert.Equal("key-one", (string)firstTargetItem.Key);
        Assert.Equal("value-one", (string)firstTargetItem.Value);

        var secondTargetItem = target[1];
        Assert.Equal(typeof(DictionaryEntry), secondTargetItem.GetType());
        Assert.Equal("key-two", (string)secondTargetItem.Key);
        Assert.Equal("value-two", (string)secondTargetItem.Value);
    }

    [Fact]
    public void HashtableCopyTo_successfully_copies_to_Object_array()
    {
        Dictionary<string, string> source = new()
        {
            { "key-one", "value-one" },
            { "key-two", "value-two" },
        };
        object[] target = new object[2];

        source.HashtableCopyTo(target, index: 0);

        var firstTargetItem = (DictionaryEntry)target[0];
        Assert.Equal("key-one", (string)firstTargetItem.Key);
        Assert.Equal("value-one", (string)firstTargetItem.Value);

        var secondTargetItem = (DictionaryEntry)target[1];
        Assert.Equal("key-two", (string)secondTargetItem.Key);
        Assert.Equal("value-two", (string)secondTargetItem.Value);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public class ContextMenuStripGroupCollectionTests
{
    [Fact]
    public void Indexer_ShouldCreateNewGroupIfKeyDoesNotExist()
    {
        ContextMenuStripGroupCollection collection = new();

        ContextMenuStripGroup group = collection["newKey"];

        group.Should().NotBeNull();
        group.Should().BeOfType<ContextMenuStripGroup>();
        collection.ContainsKey("newKey").Should().BeTrue();
    }

    [Fact]
    public void ContainsKey_ShouldReturnFalseIfKeyDoesNotExist()
    {
        ContextMenuStripGroupCollection collection = new();

        bool containsKey = collection.ContainsKey("nonExistentKey");

        containsKey.Should().BeFalse();
    }

    [Fact]
    public void Indexer_ShouldReturnSameGroupForSameKey()
    {
        ContextMenuStripGroupCollection collection = new();

        ContextMenuStripGroup group1 = collection["newKey"];
        ContextMenuStripGroup group2 = collection["newKey"];

        group1.Should().BeSameAs(group2);
    }

    [Fact]
    public void Indexer_ShouldReturnDifferentGroupsForDifferentKeys()
    {
        ContextMenuStripGroupCollection collection = new();

        ContextMenuStripGroup group1 = collection["key1"];
        ContextMenuStripGroup group2 = collection["key2"];

        group1.Should().NotBeSameAs(group2);
        collection.ContainsKey("key1").Should().BeTrue();
        collection.ContainsKey("key2").Should().BeTrue();
    }
}

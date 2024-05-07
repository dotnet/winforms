// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Tests;

public class CheckedListBox_CheckedIndexCollectionTests
{
    [WinFormsFact]
    public void CheckedIndexCollection_Ctor_OwnerIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("owner", () => { new CheckedListBox.CheckedIndexCollection(null); });
    }

    [WinFormsFact]
    public void CheckedIndexCollection_Properties_ReturnExpected()
    {
        using CheckedListBox checkedListBox = new();
        CheckedListBox.CheckedIndexCollection collection = new(checkedListBox);

        // Test properties: Count, SyncRoot, IsSynchronized, IsFixedSize, IsReadOnly
        collection.Count.Should().Be(0);
        ((ICollection)collection).SyncRoot.Should().BeSameAs(collection);
        ((ICollection)collection).IsSynchronized.Should().BeFalse();
        ((IList)collection).IsFixedSize.Should().BeTrue();
        collection.IsReadOnly.Should().BeTrue();
    }

    [WinFormsFact]
    public void CheckedIndexCollection_Methods_ThrowNotSupportedException()
    {
        using CheckedListBox checkedListBox = new();
        CheckedListBox.CheckedIndexCollection collection = new(checkedListBox);

        // Test methods: Add, Clear, Insert, Remove, RemoveAt that should throw NotSupportedException
        ((Action)(() => ((IList)collection)[0] = 1)).Should().Throw<NotSupportedException>();
        ((IList)collection).Invoking(c => c.Add(1)).Should().Throw<NotSupportedException>();
        ((IList)collection).Invoking(c => c.Clear()).Should().Throw<NotSupportedException>();
        ((IList)collection).Invoking(c => c.Insert(0, 1)).Should().Throw<NotSupportedException>();
        ((IList)collection).Invoking(c => c.Remove(1)).Should().Throw<NotSupportedException>();
        ((IList)collection).Invoking(c => c.RemoveAt(0)).Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void CheckedIndexCollection_Contains_ReturnsExpected()
    {
        using CheckedListBox checkedListBox = new();
        CheckedListBox.CheckedIndexCollection collection = new(checkedListBox);
        checkedListBox.Items.Add("item1", true);
        checkedListBox.Items.Add("item2", false);

        // Test Contains method
        collection.Contains(0).Should().BeTrue();
        collection.Contains(1).Should().BeFalse();
        ((IList)collection).Contains(0).Should().BeTrue();
        ((IList)collection).Contains(1).Should().BeFalse();
        ((IList)collection).Contains("invalid").Should().BeFalse();
    }

    [WinFormsFact]
    public void CheckedIndexCollection_IndexOf_ReturnsExpected()
    {
        using CheckedListBox checkedListBox = new();
        CheckedListBox.CheckedIndexCollection collection = new(checkedListBox);
        checkedListBox.Items.Add("item1", true);
        checkedListBox.Items.Add("item2", false);

        // Test IndexOf method
        collection.IndexOf(0).Should().Be(0);
        collection.IndexOf(1).Should().Be(-1);
        ((IList)collection).IndexOf(0).Should().Be(0);
        ((IList)collection).IndexOf(1).Should().Be(-1);
        ((IList)collection).IndexOf("invalid").Should().Be(-1);
    }
}

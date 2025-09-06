// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class NumericUpDownAccelerationCollectionTests
{
    [Fact]
    public void Clear_RemovesAllItems()
    {
        NumericUpDownAcceleration item1 = new(1, 1.0m);
        NumericUpDownAcceleration item2 = new(2, 2.0m);
        NumericUpDownAccelerationCollection collection = new();
        collection.Add(item1);
        collection.Add(item2);

        collection.Count.Should().Be(2);

        collection.Clear();

        collection.Count.Should().Be(0);
        collection.Contains(item1).Should().BeFalse();
        collection.Contains(item2).Should().BeFalse();
    }

    [Fact]
    public void Contains_ReturnsTrue_IfItemExists()
    {
        NumericUpDownAcceleration item = new(2, 1.5m);
        NumericUpDownAccelerationCollection collection = new();
        collection.Add(item);

        collection.Contains(item).Should().BeTrue();
    }

    [Fact]
    public void Contains_ReturnsFalse_IfItemDoesNotExist()
    {
        NumericUpDownAcceleration item = new(2, 1.5m);
        NumericUpDownAccelerationCollection collection = new();

        collection.Contains(item).Should().BeFalse();
    }

    [Fact]
    public void CopyTo_CopiesItemsToArray()
    {
        NumericUpDownAcceleration item1 = new(1, 1.0m);
        NumericUpDownAcceleration item2 = new(2, 2.0m);
        NumericUpDownAccelerationCollection collection = new();
        collection.Add(item1);
        collection.Add(item2);
        NumericUpDownAcceleration[] array = new NumericUpDownAcceleration[2];

        collection.CopyTo(array, 0);

        array[0].Should().Be(item1);
        array[1].Should().Be(item2);
    }

    [Fact]
    public void CopyTo_ThrowsArgumentException_IfArrayTooSmall()
    {
        NumericUpDownAcceleration item1 = new(1, 1.0m);
        NumericUpDownAcceleration item2 = new(2, 2.0m);
        NumericUpDownAccelerationCollection collection = new();
        collection.Add(item1);
        collection.Add(item2);
        NumericUpDownAcceleration[] array = new NumericUpDownAcceleration[1];

        collection.Invoking(c => c.CopyTo(array, 0))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CopyTo_ThrowsArgumentNullException_IfArrayIsNull()
    {
        NumericUpDownAccelerationCollection collection = new();
        collection.Add(new NumericUpDownAcceleration(1, 1.0m));

        collection.Invoking(c => c.CopyTo(null!, 0))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsReadOnly_AlwaysReturnsFalse()
    {
        NumericUpDownAccelerationCollection collection = new();
        collection.IsReadOnly.Should().BeFalse();
    }

    [Fact]
    public void Remove_RemovesItem_IfExists()
    {
        NumericUpDownAcceleration item1 = new(1, 1.0m);
        NumericUpDownAcceleration item2 = new(2, 2.0m);
        NumericUpDownAccelerationCollection collection = new();
        collection.Add(item1);
        collection.Add(item2);

        bool removed = collection.Remove(item1);

        removed.Should().BeTrue();
        collection.Count.Should().Be(1);
        collection.Contains(item1).Should().BeFalse();
        collection.Contains(item2).Should().BeTrue();
    }

    [Fact]
    public void Remove_ReturnsFalse_IfItemNotInCollection()
    {
        NumericUpDownAcceleration item1 = new(1, 1.0m);
        NumericUpDownAcceleration item2 = new(2, 2.0m);
        NumericUpDownAccelerationCollection collection = new();
        collection.Add(item1);

        bool removed = collection.Remove(item2);

        removed.Should().BeFalse();
        collection.Count.Should().Be(1);
        collection.Contains(item1).Should().BeTrue();
    }

    [Fact]
    public void AddRange_AddsAllItems_AndKeepsSorted()
    {
        NumericUpDownAcceleration item1 = new(5, 1.0m);
        NumericUpDownAcceleration item2 = new(2, 2.0m);
        NumericUpDownAcceleration item3 = new(3, 3.0m);
        NumericUpDownAccelerationCollection collection = new();

        collection.AddRange(item1, item2, item3);

        collection.Count.Should().Be(3);
        collection[0].Should().Be(item2);
        collection[1].Should().Be(item3);
        collection[2].Should().Be(item1);
    }

    [Fact]
    public void AddRange_ThrowsArgumentNullException_IfArrayIsNull()
    {
        NumericUpDownAccelerationCollection collection = new();

        collection.Invoking(c => c.AddRange(null!))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddRange_ThrowsArgumentNullException_IfAnyElementIsNull()
    {
        NumericUpDownAcceleration item1 = new(1, 1.0m);
        NumericUpDownAcceleration? item2 = null;
        NumericUpDownAcceleration item3 = new(2, 2.0m);
        NumericUpDownAccelerationCollection collection = new();

        collection.Invoking(c => c.AddRange(item1, item2!, item3))
            .Should().Throw<ArgumentNullException>();
    }
}

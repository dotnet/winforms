// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.PropertyGridInternal.Tests;

public class ArrayElementGridEntryTests
{
    private class TestGridEntry : GridEntry
    {
        public TestGridEntry(PropertyGrid ownerGrid, GridEntry? parent = null)
            : base(ownerGrid, parent)
        {
            PropertyValue = new int[] { 1, 2, 3 };
        }

        public override GridItemType GridItemType => GridItemType.Property;
        public override bool IsValueEditable => true;
        public override string PropertyLabel => "Test";
        public override Type? PropertyType => typeof(int[]);
        public override object? PropertyValue { get; set; }
        public override bool ShouldRenderReadOnly => false;
    }

    private static PropertyGrid CreatePropertyGrid() => new();

    [WinFormsFact]
    public void ArrayElementGridEntry_ConstructorAndProperties()
    {
        using PropertyGrid grid = CreatePropertyGrid();
        TestGridEntry parent = new(grid);
        ArrayElementGridEntry entry = new(grid, parent, 2);

        entry.Should().NotBeNull();
        entry.PropertyLabel.Should().Be("[2]");
        entry.GridItemType.Should().Be(GridItemType.ArrayValue);
        entry.IsValueEditable.Should().BeTrue();
        entry.PropertyType.Should().Be(typeof(int));
        entry.ShouldRenderReadOnly.Should().BeFalse();
        entry.PropertyValue.Should().Be(3);

        entry.PropertyValue = 10;
        entry.PropertyValue.Should().Be(10);
    }
}

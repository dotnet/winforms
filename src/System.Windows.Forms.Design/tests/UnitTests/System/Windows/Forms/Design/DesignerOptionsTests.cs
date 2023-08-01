// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Design.Tests;

public class DesignerOptionsTests
{
    [Fact]
    public void DesignerOptions_Ctor_Default()
    {
        DesignerOptions options = new();
        Assert.True(options.EnableInSituEditing);
        Assert.Equal(new Size(8, 8), options.GridSize);
        Assert.True(options.ObjectBoundSmartTagAutoShow);
        Assert.True(options.ShowGrid);
        Assert.True(options.SnapToGrid);
        Assert.False(options.UseOptimizedCodeGeneration);
        Assert.False(options.UseSmartTags);
        Assert.False(options.UseSnapLines);
    }

    [Theory]
    [BoolData]
    public void DesignerOptions_EnableInSituEditing_Set_GetReturnsExpected(bool value)
    {
        DesignerOptions options = new()
        {
            EnableInSituEditing = value
        };
        Assert.Equal(value, options.EnableInSituEditing);

        // Set same.
        options.EnableInSituEditing = value;
        Assert.Equal(value, options.EnableInSituEditing);

        // Set different.
        options.EnableInSituEditing = !value;
        Assert.Equal(!value, options.EnableInSituEditing);
    }

    public static IEnumerable<object[]> GridSize_Set_TestData()
    {
        yield return new object[] { new Size(0, 0), new Size(2, 2) };
        yield return new object[] { new Size(0, 2), new Size(2, 2) };
        yield return new object[] { new Size(2, 0), new Size(2, 2) };
        yield return new object[] { new Size(2, 2), new Size(2, 2) };
        yield return new object[] { new Size(200, 200), new Size(200, 200) };
        yield return new object[] { new Size(201, 200), new Size(200, 200) };
        yield return new object[] { new Size(200, 201), new Size(200, 200) };
    }

    [Theory]
    [MemberData(nameof(GridSize_Set_TestData))]
    public void DesignerOptions_GridSize_Set_GetReturnsExpected(Size value, Size expected)
    {
        DesignerOptions options = new()
        {
            GridSize = value
        };
        Assert.Equal(expected, options.GridSize);

        // Set same.
        options.GridSize = value;
        Assert.Equal(expected, options.GridSize);
    }

    [Theory]
    [BoolData]
    public void DesignerOptions_ObjectBoundSmartTagAutoShow_Set_GetReturnsExpected(bool value)
    {
        DesignerOptions options = new()
        {
            ObjectBoundSmartTagAutoShow = value
        };
        Assert.Equal(value, options.ObjectBoundSmartTagAutoShow);

        // Set same.
        options.ObjectBoundSmartTagAutoShow = value;
        Assert.Equal(value, options.ObjectBoundSmartTagAutoShow);

        // Set different.
        options.ObjectBoundSmartTagAutoShow = !value;
        Assert.Equal(!value, options.ObjectBoundSmartTagAutoShow);
    }

    [Theory]
    [BoolData]
    public void DesignerOptions_ShowGrid_Set_GetReturnsExpected(bool value)
    {
        DesignerOptions options = new()
        {
            ShowGrid = value
        };
        Assert.Equal(value, options.ShowGrid);

        // Set same.
        options.ShowGrid = value;
        Assert.Equal(value, options.ShowGrid);

        // Set different.
        options.ShowGrid = !value;
        Assert.Equal(!value, options.ShowGrid);
    }

    [Theory]
    [BoolData]
    public void DesignerOptions_SnapToGrid_Set_GetReturnsExpected(bool value)
    {
        DesignerOptions options = new()
        {
            SnapToGrid = value
        };
        Assert.Equal(value, options.SnapToGrid);

        // Set same.
        options.SnapToGrid = value;
        Assert.Equal(value, options.SnapToGrid);

        // Set different.
        options.SnapToGrid = !value;
        Assert.Equal(!value, options.SnapToGrid);
    }

    [Theory]
    [BoolData]
    public void DesignerOptions_UseOptimizedCodeGeneration_Set_GetReturnsExpected(bool value)
    {
        DesignerOptions options = new()
        {
            UseOptimizedCodeGeneration = value
        };
        Assert.Equal(value, options.UseOptimizedCodeGeneration);

        // Set same.
        options.UseOptimizedCodeGeneration = value;
        Assert.Equal(value, options.UseOptimizedCodeGeneration);

        // Set different.
        options.UseOptimizedCodeGeneration = !value;
        Assert.Equal(!value, options.UseOptimizedCodeGeneration);
    }

    [Theory]
    [BoolData]
    public void DesignerOptions_UseSmartTags_Set_GetReturnsExpected(bool value)
    {
        DesignerOptions options = new()
        {
            UseSmartTags = value
        };
        Assert.Equal(value, options.UseSmartTags);

        // Set same.
        options.UseSmartTags = value;
        Assert.Equal(value, options.UseSmartTags);

        // Set different.
        options.UseSmartTags = !value;
        Assert.Equal(!value, options.UseSmartTags);
    }

    [Theory]
    [BoolData]
    public void DesignerOptions_UseSnapLines_Set_GetReturnsExpected(bool value)
    {
        DesignerOptions options = new()
        {
            UseSnapLines = value
        };
        Assert.Equal(value, options.UseSnapLines);

        // Set same.
        options.UseSnapLines = value;
        Assert.Equal(value, options.UseSnapLines);

        // Set different.
        options.UseSnapLines = !value;
        Assert.Equal(!value, options.UseSnapLines);
    }
}

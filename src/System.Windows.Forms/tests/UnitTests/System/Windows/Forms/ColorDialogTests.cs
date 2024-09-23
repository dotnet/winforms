// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ColorDialogTests
{
    [WinFormsFact]
    public void ColorDialog_Ctor_Default()
    {
        using SubColorDialog dialog = new();
        Assert.True(dialog.AllowFullOpen);
        Assert.False(dialog.AnyColor);
        Assert.True(dialog.CanRaiseEvents);
        Assert.Equal(Color.Black, dialog.Color);
        Assert.Equal(Enumerable.Repeat(0x00FFFFFF, 16).ToArray(), dialog.CustomColors);
        Assert.NotSame(dialog.CustomColors, dialog.CustomColors);
        Assert.Null(dialog.Container);
        Assert.False(dialog.DesignMode);
        Assert.NotNull(dialog.Events);
        Assert.Same(dialog.Events, dialog.Events);
        Assert.NotEqual(IntPtr.Zero, dialog.Instance);
        Assert.Equal(dialog.Instance, dialog.Instance);
        Assert.False(dialog.FullOpen);
        Assert.Equal(0, dialog.Options);
        Assert.False(dialog.ShowHelp);
        Assert.False(dialog.SolidColorOnly);
        Assert.Null(dialog.Site);
        Assert.Null(dialog.Tag);
    }

    [WinFormsFact]
    public void ColorDialog_Ctor_Default_OverriddenReset()
    {
        using EmptyResetColorDialog dialog = new();
        Assert.True(dialog.AllowFullOpen);
        Assert.False(dialog.AnyColor);
        Assert.True(dialog.CanRaiseEvents);
        Assert.Equal(Color.Empty, dialog.Color);
        Assert.Equal(new int[16], dialog.CustomColors);
        Assert.NotSame(dialog.CustomColors, dialog.CustomColors);
        Assert.Null(dialog.Container);
        Assert.False(dialog.DesignMode);
        Assert.NotNull(dialog.Events);
        Assert.Same(dialog.Events, dialog.Events);
        Assert.False(dialog.FullOpen);
        Assert.NotEqual(IntPtr.Zero, dialog.Instance);
        Assert.Equal(dialog.Instance, dialog.Instance);
        Assert.Equal(0, dialog.Options);
        Assert.False(dialog.ShowHelp);
        Assert.False(dialog.SolidColorOnly);
        Assert.Null(dialog.Site);
        Assert.Null(dialog.Tag);
    }

    [WinFormsTheory]
    [InlineData(true, 0, 0x4)]
    [InlineData(false, 0x4, 0)]
    public void ColorDialog_AllowFullOpen_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubColorDialog dialog = new()
        {
            AllowFullOpen = value
        };
        Assert.Equal(value, dialog.AllowFullOpen);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.AllowFullOpen = value;
        Assert.Equal(value, dialog.AllowFullOpen);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.AllowFullOpen = !value;
        Assert.Equal(!value, dialog.AllowFullOpen);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(true, 0x100, 0)]
    [InlineData(false, 0, 0x100)]
    public void ColorDialog_AnyColor_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubColorDialog dialog = new()
        {
            AnyColor = value
        };
        Assert.Equal(value, dialog.AnyColor);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.AnyColor = value;
        Assert.Equal(value, dialog.AnyColor);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.AnyColor = !value;
        Assert.Equal(!value, dialog.AnyColor);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    public static IEnumerable<object[]> Color_Set_TestData()
    {
        yield return new object[] { Color.Empty, Color.Black };
        yield return new object[] { Color.Red, Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(Color_Set_TestData))]
    public void ColorDialog_Color_Set_GetReturnsExpected(Color value, Color expected)
    {
        using ColorDialog dialog = new()
        {
            Color = value
        };
        Assert.Equal(expected, dialog.Color);

        // Set same.
        dialog.Color = value;
        Assert.Equal(expected, dialog.Color);
    }

    [WinFormsTheory]
    [MemberData(nameof(Color_Set_TestData))]
    public void ColorDialog_Color_SetWithCustomOldValue_GetReturnsExpected(Color value, Color expected)
    {
        using ColorDialog dialog = new()
        {
            Color = Color.Blue
        };

        dialog.Color = value;
        Assert.Equal(expected, dialog.Color);

        // Set same.
        dialog.Color = value;
        Assert.Equal(expected, dialog.Color);
    }

    [WinFormsFact]
    public void ColorDialog_Color_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ColorDialog))[nameof(ColorDialog.Color)];
        using ColorDialog dialog = new();
        Assert.False(property.CanResetValue(dialog));

        dialog.Color = Color.Red;
        Assert.Equal(Color.Red, dialog.Color);
        Assert.True(property.CanResetValue(dialog));

        property.ResetValue(dialog);
        Assert.Equal(Color.Black, dialog.Color);
        Assert.False(property.CanResetValue(dialog));
    }

    [WinFormsFact]
    public void ColorDialog_Color_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ColorDialog))[nameof(ColorDialog.Color)];
        using ColorDialog dialog = new();
        Assert.False(property.ShouldSerializeValue(dialog));

        dialog.Color = Color.Red;
        Assert.Equal(Color.Red, dialog.Color);
        Assert.True(property.ShouldSerializeValue(dialog));

        property.ResetValue(dialog);
        Assert.Equal(Color.Black, dialog.Color);
        Assert.False(property.ShouldSerializeValue(dialog));
    }

    [WinFormsFact]
    public void ColorDialog_CustomColors_Get_ReturnsClone()
    {
        using ColorDialog dialog = new();
        int[] value1 = dialog.CustomColors;
        int[] value2 = dialog.CustomColors;
        Assert.NotSame(value1, value2);
        Assert.Equal(value1, value2);
        value1[0] = 1;
        Assert.Equal(0x00FFFFFF, value2[0]);
        Assert.Equal(0x00FFFFFF, dialog.CustomColors[0]);
    }

    public static IEnumerable<object[]> CustomColors_Set_TestData()
    {
        yield return new object[] { null, Enumerable.Repeat(0x00FFFFFF, 16).ToArray() };
        yield return new object[] { Array.Empty<int>(), Enumerable.Repeat(0x00FFFFFF, 16).ToArray() };
        yield return new object[] { new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 }.Concat(Enumerable.Repeat(0x00FFFFFF, 13)).ToArray() };
        yield return new object[] { Enumerable.Repeat(0, 16).ToArray(), Enumerable.Repeat(0, 16).ToArray() };
        yield return new object[] { Enumerable.Repeat(unchecked((int)0xFFFFFFFF), 16).ToArray(), Enumerable.Repeat(unchecked((int)0xFFFFFFFF), 16).ToArray() };
        yield return new object[] { Enumerable.Repeat(1, 16).ToArray(), Enumerable.Repeat(1, 16).ToArray() };
        yield return new object[] { Enumerable.Repeat(1, 20).ToArray(), Enumerable.Repeat(1, 16).ToArray() };
    }

    [WinFormsTheory]
    [MemberData(nameof(CustomColors_Set_TestData))]
    public void ColorDialog_CustomColors_Set_GetReturnsExpected(int[] value, int[] expected)
    {
        using ColorDialog dialog = new()
        {
            CustomColors = value
        };
        Assert.Equal(expected, dialog.CustomColors);
        Assert.NotSame(value, dialog.CustomColors);
        Assert.NotSame(dialog.CustomColors, dialog.CustomColors);

        // Set same.
        dialog.CustomColors = value;
        Assert.Equal(expected, dialog.CustomColors);
        Assert.NotSame(value, dialog.CustomColors);
        Assert.NotSame(dialog.CustomColors, dialog.CustomColors);
    }

    [WinFormsTheory]
    [MemberData(nameof(CustomColors_Set_TestData))]
    public void ColorDialog_CustomColors_SetWithCustomOldValue_GetReturnsExpected(int[] value, int[] expected)
    {
        using ColorDialog dialog = new()
        {
            CustomColors = new int[1]
        };

        dialog.CustomColors = value;
        Assert.Equal(expected, dialog.CustomColors);
        Assert.NotSame(value, dialog.CustomColors);
        Assert.NotSame(dialog.CustomColors, dialog.CustomColors);

        // Set same.
        dialog.CustomColors = value;
        Assert.Equal(expected, dialog.CustomColors);
        Assert.NotSame(value, dialog.CustomColors);
        Assert.NotSame(dialog.CustomColors, dialog.CustomColors);
    }

    [WinFormsFact]
    public void ColorDialog_CustomColors_Set_GetReturnsClone()
    {
        int[] value = Enumerable.Repeat(1, 16).ToArray();
        using ColorDialog dialog = new()
        {
            CustomColors = value
        };
        value[0] = 0;
        Assert.Equal(Enumerable.Repeat(1, 16), dialog.CustomColors);
    }

    [WinFormsTheory]
    [InlineData(true, 0x2, 0)]
    [InlineData(false, 0, 0x2)]
    public void ColorDialog_FullOpen_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubColorDialog dialog = new()
        {
            FullOpen = value
        };
        Assert.Equal(value, dialog.FullOpen);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.FullOpen = value;
        Assert.Equal(value, dialog.FullOpen);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.FullOpen = !value;
        Assert.Equal(!value, dialog.FullOpen);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(true, 0x8, 0)]
    [InlineData(false, 0, 0x8)]
    public void ColorDialog_ShowHelp_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubColorDialog dialog = new()
        {
            ShowHelp = value
        };
        Assert.Equal(value, dialog.ShowHelp);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.ShowHelp = value;
        Assert.Equal(value, dialog.ShowHelp);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.ShowHelp = !value;
        Assert.Equal(!value, dialog.ShowHelp);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(true, 0x80, 0)]
    [InlineData(false, 0, 0x80)]
    public void ColorDialog_SolidColorOnly_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubColorDialog dialog = new()
        {
            SolidColorOnly = value
        };
        Assert.Equal(value, dialog.SolidColorOnly);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.SolidColorOnly = value;
        Assert.Equal(value, dialog.SolidColorOnly);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.SolidColorOnly = !value;
        Assert.Equal(!value, dialog.SolidColorOnly);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsFact]
    public void ColorDialog_Reset_Invoke_Success()
    {
        using SubColorDialog dialog = new()
        {
            AllowFullOpen = false,
            AnyColor = true,
            Color = Color.Red,
            CustomColors = [1, 2, 3],
            FullOpen = true,
            ShowHelp = true,
            SolidColorOnly = true,
            Tag = "Tag",
        };

        dialog.Reset();
        Assert.True(dialog.AllowFullOpen);
        Assert.False(dialog.AnyColor);
        Assert.True(dialog.CanRaiseEvents);
        Assert.Equal(Color.Black, dialog.Color);
        Assert.Equal(Enumerable.Repeat(0x00FFFFFF, 16).ToArray(), dialog.CustomColors);
        Assert.NotSame(dialog.CustomColors, dialog.CustomColors);
        Assert.Null(dialog.Container);
        Assert.False(dialog.DesignMode);
        Assert.NotNull(dialog.Events);
        Assert.Same(dialog.Events, dialog.Events);
        Assert.False(dialog.FullOpen);
        Assert.NotEqual(IntPtr.Zero, dialog.Instance);
        Assert.Equal(dialog.Instance, dialog.Instance);
        Assert.Equal(0, dialog.Options);
        Assert.False(dialog.ShowHelp);
        Assert.False(dialog.SolidColorOnly);
        Assert.Null(dialog.Site);
        Assert.Equal("Tag", dialog.Tag);
    }

    [WinFormsFact]
    public void ColorDialog_ToString_Invoke_ReturnsExpected()
    {
        using SubColorDialog dialog = new();
        Assert.Equal("System.Windows.Forms.Tests.ColorDialogTests+SubColorDialog,  Color: Color [Black]", dialog.ToString());
    }

    private class SubColorDialog : ColorDialog
    {
        public new bool CanRaiseEvents => base.CanRaiseEvents;

        public new bool DesignMode => base.DesignMode;

        public new EventHandlerList Events => base.Events;

        public new IntPtr Instance => base.Instance;

        public new int Options => base.Options;
    }

    private class EmptyResetColorDialog : SubColorDialog
    {
        public override void Reset()
        {
        }
    }
}

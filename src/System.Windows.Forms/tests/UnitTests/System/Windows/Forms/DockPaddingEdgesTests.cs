// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Tests;

public class DockPaddingEdgesTests
{
    [WinFormsTheory]
    [InlineData(-2, -2, -2, -2, 0, 0, 0, 0, 0)]
    [InlineData(-1, -1, -1, -1, 0, 0, 0, 0, 0)]
    [InlineData(0, -1, -1, -1, 0, 0, 0, 0, 0)]
    [InlineData(-1, 0, -1, -1, 0, 0, 0, 0, 0)]
    [InlineData(-1, -1, 0, -1, 0, 0, 0, 0, 0)]
    [InlineData(-1, -1, -1, 0, 0, 0, 0, 0, 0)]
    [InlineData(0, 0, 0, 0, 0, 0, 0, 0, 0)]
    [InlineData(1, 1, 1, 1, 1, 1, 1, 1, 1)]
    [InlineData(1, 2, 3, 4, 0, 1, 2, 3, 4)]
    public void DockPaddingEdges_Properties_Get_ReturnsExpected(int left, int top, int right, int bottom, int expectedAll, int expectedLeft, int expectedTop, int expectedRight, int expectedBottom)
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(left, top, right, bottom)
        };
        ScrollableControl.DockPaddingEdges padding = owner.DockPadding;
        Assert.Equal(expectedAll, padding.All);
        Assert.Equal(expectedLeft, padding.Left);
        Assert.Equal(expectedTop, padding.Top);
        Assert.Equal(expectedRight, padding.Right);
        Assert.Equal(expectedBottom, padding.Bottom);
    }

    [WinFormsTheory]
    [InlineData(-2, 0, -1)]
    [InlineData(-1, 0, -1)]
    [InlineData(0, 0, 0)]
    [InlineData(1, 1, 1)]
    public void DockPaddingEdges_All_SetWithOwner_GetReturnsExpected(int value, int expectedValue, int expectedPaddingAll)
    {
        using ScrollableControl owner = new();
        ScrollableControl.DockPaddingEdges padding = owner.DockPadding;
        padding.All = value;
        Assert.Equal(expectedValue, padding.All);
        Assert.Equal(expectedValue, padding.Left);
        Assert.Equal(expectedValue, padding.Top);
        Assert.Equal(expectedValue, padding.Right);
        Assert.Equal(expectedValue, padding.Bottom);
        Assert.Equal(expectedPaddingAll, owner.Padding.All);
        Assert.Equal(expectedValue, owner.Padding.Left);
        Assert.Equal(expectedValue, owner.Padding.Top);
        Assert.Equal(expectedValue, owner.Padding.Right);
        Assert.Equal(expectedValue, owner.Padding.Bottom);
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void DockPaddingEdges_All_SetWithoutOwner_GetReturnsExpected(int value)
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ICloneable original = owner.DockPadding;
        ScrollableControl.DockPaddingEdges padding = (ScrollableControl.DockPaddingEdges)original.Clone();
        padding.All = value;
        Assert.Equal(value, padding.All);
        Assert.Equal(value, padding.Left);
        Assert.Equal(value, padding.Top);
        Assert.Equal(value, padding.Right);
        Assert.Equal(value, padding.Bottom);
    }

    [WinFormsTheory]
    [InlineData(-2, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    public void DockPaddingEdges_Left_SetWithOwner_GetReturnsExpected(int value, int expectedLeft)
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ScrollableControl.DockPaddingEdges padding = owner.DockPadding;
        padding.Left = value;
        Assert.Equal(0, padding.All);
        Assert.Equal(expectedLeft, padding.Left);
        Assert.Equal(2, padding.Top);
        Assert.Equal(3, padding.Right);
        Assert.Equal(4, padding.Bottom);
        Assert.Equal(-1, owner.Padding.All);
        Assert.Equal(expectedLeft, owner.Padding.Left);
        Assert.Equal(2, owner.Padding.Top);
        Assert.Equal(3, owner.Padding.Right);
        Assert.Equal(4, owner.Padding.Bottom);
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void DockPaddingEdges_Left_SetWithoutOwner_GetReturnsExpected(int value)
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ICloneable original = owner.DockPadding;
        ScrollableControl.DockPaddingEdges padding = (ScrollableControl.DockPaddingEdges)original.Clone();
        padding.Left = value;
        Assert.Equal(0, padding.All);
        Assert.Equal(value, padding.Left);
        Assert.Equal(2, padding.Top);
        Assert.Equal(3, padding.Right);
        Assert.Equal(4, padding.Bottom);
    }

    [WinFormsTheory]
    [InlineData(-2, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    public void DockPaddingEdges_Top_SetWithOwner_GetReturnsExpected(int value, int expectedTop)
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ScrollableControl.DockPaddingEdges padding = owner.DockPadding;
        padding.Top = value;
        Assert.Equal(0, padding.All);
        Assert.Equal(1, padding.Left);
        Assert.Equal(expectedTop, padding.Top);
        Assert.Equal(3, padding.Right);
        Assert.Equal(4, padding.Bottom);
        Assert.Equal(-1, owner.Padding.All);
        Assert.Equal(1, owner.Padding.Left);
        Assert.Equal(expectedTop, owner.Padding.Top);
        Assert.Equal(3, owner.Padding.Right);
        Assert.Equal(4, owner.Padding.Bottom);
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void DockPaddingEdges_Top_SetWithoutOwner_GetReturnsExpected(int value)
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ICloneable original = owner.DockPadding;
        ScrollableControl.DockPaddingEdges padding = (ScrollableControl.DockPaddingEdges)original.Clone();
        padding.Top = value;
        Assert.Equal(0, padding.All);
        Assert.Equal(1, padding.Left);
        Assert.Equal(value, padding.Top);
        Assert.Equal(3, padding.Right);
        Assert.Equal(4, padding.Bottom);
    }

    [WinFormsTheory]
    [InlineData(-2, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    public void DockPaddingEdges_Right_SetWithOwner_GetReturnsExpected(int value, int expectedRight)
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ScrollableControl.DockPaddingEdges padding = owner.DockPadding;
        padding.Right = value;
        Assert.Equal(0, padding.All);
        Assert.Equal(1, padding.Left);
        Assert.Equal(2, padding.Top);
        Assert.Equal(expectedRight, padding.Right);
        Assert.Equal(4, padding.Bottom);
        Assert.Equal(-1, owner.Padding.All);
        Assert.Equal(1, owner.Padding.Left);
        Assert.Equal(2, owner.Padding.Top);
        Assert.Equal(expectedRight, owner.Padding.Right);
        Assert.Equal(4, owner.Padding.Bottom);
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void DockPaddingEdges_Right_SetWithoutOwner_GetReturnsExpected(int value)
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ICloneable original = owner.DockPadding;
        ScrollableControl.DockPaddingEdges padding = (ScrollableControl.DockPaddingEdges)original.Clone();
        padding.Right = value;
        Assert.Equal(0, padding.All);
        Assert.Equal(1, padding.Left);
        Assert.Equal(2, padding.Top);
        Assert.Equal(value, padding.Right);
        Assert.Equal(4, padding.Bottom);
    }

    [WinFormsTheory]
    [InlineData(-2, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    public void DockPaddingEdges_Bottom_SetWithOwner_GetReturnsExpected(int value, int expectedBottom)
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ScrollableControl.DockPaddingEdges padding = owner.DockPadding;
        padding.Bottom = value;
        Assert.Equal(0, padding.All);
        Assert.Equal(1, padding.Left);
        Assert.Equal(2, padding.Top);
        Assert.Equal(3, padding.Right);
        Assert.Equal(expectedBottom, padding.Bottom);
        Assert.Equal(-1, owner.Padding.All);
        Assert.Equal(1, owner.Padding.Left);
        Assert.Equal(2, owner.Padding.Top);
        Assert.Equal(3, owner.Padding.Right);
        Assert.Equal(expectedBottom, owner.Padding.Bottom);
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void DockPaddingEdges_Bottom_SetWithoutOwner_GetReturnsExpected(int value)
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ICloneable original = owner.DockPadding;
        ScrollableControl.DockPaddingEdges padding = (ScrollableControl.DockPaddingEdges)original.Clone();
        padding.Bottom = value;
        Assert.Equal(0, padding.All);
        Assert.Equal(1, padding.Left);
        Assert.Equal(2, padding.Top);
        Assert.Equal(3, padding.Right);
        Assert.Equal(value, padding.Bottom);
    }

    public static IEnumerable<object[]> Equals_TestData()
    {
        ScrollableControl.DockPaddingEdges CreatePadding(int left, int top, int right, int bottom)
        {
            ScrollableControl owner = new()
            {
                Padding = new Padding(left, top, right, bottom)
            };
            return owner.DockPadding;
        }

        ScrollableControl.DockPaddingEdges CreateClonedPadding(int left, int top, int right, int bottom)
        {
            ICloneable cloneable = CreatePadding(left, top, right, bottom);
            return Assert.IsType<ScrollableControl.DockPaddingEdges>(cloneable.Clone());
        }

        yield return new object[] { CreatePadding(1, 2, 3, 4), CreatePadding(1, 2, 3, 4), true };
        yield return new object[] { CreatePadding(1, 2, 3, 4), CreateClonedPadding(1, 2, 3, 4), true };
        yield return new object[] { CreatePadding(1, 2, 3, 4), CreatePadding(1, 1, 3, 4), false };
        yield return new object[] { CreatePadding(1, 2, 3, 4), CreateClonedPadding(1, 1, 3, 4), false };
        yield return new object[] { CreatePadding(1, 2, 3, 4), CreatePadding(1, 2, 2, 4), false };
        yield return new object[] { CreatePadding(1, 2, 3, 4), CreateClonedPadding(1, 2, 2, 4), false };
        yield return new object[] { CreatePadding(1, 2, 3, 4), CreatePadding(1, 2, 3, 5), false };
        yield return new object[] { CreatePadding(1, 2, 3, 4), CreateClonedPadding(1, 2, 3, 5), false };

        yield return new object[] { CreateClonedPadding(1, 2, 3, 4), CreatePadding(1, 2, 3, 4), true };
        yield return new object[] { CreateClonedPadding(1, 2, 3, 4), CreateClonedPadding(1, 2, 3, 4), true };
        yield return new object[] { CreateClonedPadding(1, 2, 3, 4), CreatePadding(1, 1, 3, 4), false };
        yield return new object[] { CreateClonedPadding(1, 2, 3, 4), CreateClonedPadding(1, 1, 3, 4), false };
        yield return new object[] { CreateClonedPadding(1, 2, 3, 4), CreatePadding(1, 2, 2, 4), false };
        yield return new object[] { CreateClonedPadding(1, 2, 3, 4), CreateClonedPadding(1, 2, 2, 4), false };
        yield return new object[] { CreateClonedPadding(1, 2, 3, 4), CreatePadding(1, 2, 3, 5), false };
        yield return new object[] { CreateClonedPadding(1, 2, 3, 4), CreateClonedPadding(1, 2, 3, 5), false };

        yield return new object[] { CreatePadding(1, 2, 3, 4), new(), false };
        yield return new object[] { CreateClonedPadding(1, 2, 3, 4), new(), false };
        yield return new object[] { CreatePadding(1, 2, 3, 4), null, false };
        yield return new object[] { CreateClonedPadding(1, 2, 3, 4), null, false };
    }

    [WinFormsTheory]
    [MemberData(nameof(Equals_TestData))]
    public void DockPaddingEdges_Equals_Invoke_ReturnsExpected(ScrollableControl.DockPaddingEdges padding, object other, bool expected)
    {
        if (other is ScrollableControl.DockPaddingEdges)
        {
            Assert.Equal(expected, padding.GetHashCode().Equals(other.GetHashCode()));
        }

        Assert.Equal(expected, padding.Equals(other));
    }

    [WinFormsFact]
    public void DockPaddingEdges_GetHashCode_InvokeWithOwner_ReturnsExpected()
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ScrollableControl.DockPaddingEdges padding = owner.DockPadding;
        Assert.NotEqual(0, padding.GetHashCode());
        Assert.Equal(padding.GetHashCode(), padding.GetHashCode());
    }

    [WinFormsFact]
    public void DockPaddingEdges_GetHashCode_InvokeWithoutOwner_ReturnsExpected()
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ICloneable original = owner.DockPadding;
        ScrollableControl.DockPaddingEdges padding = (ScrollableControl.DockPaddingEdges)original.Clone();
        Assert.NotEqual(0, padding.GetHashCode());
        Assert.Equal(padding.GetHashCode(), padding.GetHashCode());
    }

    [WinFormsFact]
    public void DockPaddingEdges_Clone_InvokeWithOwner_ReturnsExpected()
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ICloneable original = owner.DockPadding;
        ScrollableControl.DockPaddingEdges padding = (ScrollableControl.DockPaddingEdges)original.Clone();
        Assert.Equal(0, padding.All);
        Assert.Equal(1, padding.Left);
        Assert.Equal(2, padding.Top);
        Assert.Equal(3, padding.Right);
        Assert.Equal(4, padding.Bottom);
    }

    [WinFormsFact]
    public void DockPaddingEdges_Clone_InvokeWithoutOwner_ReturnsExpected()
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ICloneable original1 = owner.DockPadding;
        ICloneable original2 = (ICloneable)original1.Clone();
        ScrollableControl.DockPaddingEdges padding = (ScrollableControl.DockPaddingEdges)original2.Clone();
        Assert.Equal(0, padding.All);
        Assert.Equal(1, padding.Left);
        Assert.Equal(2, padding.Top);
        Assert.Equal(3, padding.Right);
        Assert.Equal(4, padding.Bottom);
    }

    [WinFormsFact]
    public void DockPaddingEdges_ToString_InvokeWithOwner_ReturnsExpected()
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ScrollableControl.DockPaddingEdges padding = owner.DockPadding;
        Assert.Equal("{Left=1,Top=2,Right=3,Bottom=4}", padding.ToString());
    }

    [WinFormsFact]
    public void DockPaddingEdges_ToString_InvokeWithoutOwner_ReturnsExpected()
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ICloneable original = owner.DockPadding;
        ScrollableControl.DockPaddingEdges padding = (ScrollableControl.DockPaddingEdges)original.Clone();
        Assert.Equal("{Left=1,Top=2,Right=3,Bottom=4}", padding.ToString());
    }

    [WinFormsFact]
    public void DockPaddingEdges_AllPropertyDescriptor_ResetValue_SetsToZero()
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ScrollableControl.DockPaddingEdges padding = owner.DockPadding;
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(padding);
        PropertyDescriptor property = properties[nameof(ScrollableControl.DockPaddingEdges.All)];
        Assert.True(property.CanResetValue(padding));
        Assert.True(property.ShouldSerializeValue(padding));
        property.ResetValue(padding);

        Assert.Equal(0, padding.All);
        Assert.Equal(0, padding.Left);
        Assert.Equal(0, padding.Top);
        Assert.Equal(0, padding.Right);
        Assert.Equal(0, padding.Bottom);
    }

    [WinFormsFact]
    public void DockPaddingEdges_LeftPropertyDescriptor_ResetValue_SetsToZero()
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ScrollableControl.DockPaddingEdges padding = owner.DockPadding;
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(padding);
        PropertyDescriptor property = properties[nameof(ScrollableControl.DockPaddingEdges.Left)];
        Assert.True(property.CanResetValue(padding));
        Assert.True(property.ShouldSerializeValue(padding));
        property.ResetValue(padding);

        Assert.Equal(0, padding.All);
        Assert.Equal(0, padding.Left);
        Assert.Equal(2, padding.Top);
        Assert.Equal(3, padding.Right);
        Assert.Equal(4, padding.Bottom);
    }

    [WinFormsFact]
    public void DockPaddingEdges_TopPropertyDescriptor_ResetValue_SetsToZero()
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ScrollableControl.DockPaddingEdges padding = owner.DockPadding;
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(padding);
        PropertyDescriptor property = properties[nameof(ScrollableControl.DockPaddingEdges.Top)];
        Assert.True(property.CanResetValue(padding));
        Assert.True(property.ShouldSerializeValue(padding));
        property.ResetValue(padding);

        Assert.Equal(0, padding.All);
        Assert.Equal(1, padding.Left);
        Assert.Equal(0, padding.Top);
        Assert.Equal(3, padding.Right);
        Assert.Equal(4, padding.Bottom);
    }

    [WinFormsFact]
    public void DockPaddingEdges_RightPropertyDescriptor_ResetValue_SetsToZero()
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ScrollableControl.DockPaddingEdges padding = owner.DockPadding;
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(padding);
        PropertyDescriptor property = properties[nameof(ScrollableControl.DockPaddingEdges.Right)];
        Assert.True(property.CanResetValue(padding));
        Assert.True(property.ShouldSerializeValue(padding));
        property.ResetValue(padding);

        Assert.Equal(0, padding.All);
        Assert.Equal(1, padding.Left);
        Assert.Equal(2, padding.Top);
        Assert.Equal(0, padding.Right);
        Assert.Equal(4, padding.Bottom);
    }

    [WinFormsFact]
    public void DockPaddingEdges_BottomPropertyDescriptor_ResetValue_SetsToZero()
    {
        using ScrollableControl owner = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        ScrollableControl.DockPaddingEdges padding = owner.DockPadding;
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(padding);
        PropertyDescriptor property = properties[nameof(ScrollableControl.DockPaddingEdges.Bottom)];
        Assert.True(property.CanResetValue(padding));
        Assert.True(property.ShouldSerializeValue(padding));
        property.ResetValue(padding);

        Assert.Equal(0, padding.All);
        Assert.Equal(1, padding.Left);
        Assert.Equal(2, padding.Top);
        Assert.Equal(3, padding.Right);
        Assert.Equal(0, padding.Bottom);
    }

    [WinFormsFact]
    public void DockPaddingEdges_GetTypeConverter_ReturnsDockPaddingEdgesConverter()
    {
        Assert.IsType<ScrollableControl.DockPaddingEdgesConverter>(TypeDescriptor.GetConverter(typeof(ScrollableControl.DockPaddingEdges)));
    }
}

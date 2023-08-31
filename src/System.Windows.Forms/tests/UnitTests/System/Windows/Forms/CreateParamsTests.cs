// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.TestUtilities;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class CreateParamsTests
{
    [Theory]
    [StringWithNullData]
    public void CreateParams_ClassName_Set_GetReturnsExpected(string value)
    {
        var createParams = new CreateParams
        {
            ClassName = value
        };
        Assert.Equal(value, createParams.ClassName);
    }

    [Theory]
    [StringWithNullData]
    public void CreateParams_Caption_Set_GetReturnsExpected(string value)
    {
        var createParams = new CreateParams
        {
            Caption = value
        };
        Assert.Equal(value, createParams.Caption);
    }

    [Theory]
    [IntegerData<int>]
    public void CreateParams_Style_Set_GetReturnsExpected(int value)
    {
        var createParams = new CreateParams
        {
            Style = value
        };
        Assert.Equal(value, createParams.Style);
    }

    [Theory]
    [IntegerData<int>]
    public void CreateParams_ExStyle_Set_GetReturnsExpected(int value)
    {
        var createParams = new CreateParams
        {
            ExStyle = value
        };
        Assert.Equal(value, createParams.ExStyle);
    }

    [Theory]
    [IntegerData<int>]
    public void CreateParams_ClassStyle_Set_GetReturnsExpected(int value)
    {
        var createParams = new CreateParams
        {
            ClassStyle = value
        };
        Assert.Equal(value, createParams.ClassStyle);
    }

    [Theory]
    [IntegerData<int>]
    public void CreateParams_X_Set_GetReturnsExpected(int value)
    {
        var createParams = new CreateParams
        {
            X = value
        };
        Assert.Equal(value, createParams.X);
    }

    [Theory]
    [IntegerData<int>]
    public void CreateParams_Y_Set_GetReturnsExpected(int value)
    {
        var createParams = new CreateParams
        {
            Y = value
        };
        Assert.Equal(value, createParams.Y);
    }

    [Theory]
    [IntegerData<int>]
    public void CreateParams_Width_Set_GetReturnsExpected(int value)
    {
        var createParams = new CreateParams
        {
            Width = value
        };
        Assert.Equal(value, createParams.Width);
    }

    [Theory]
    [IntegerData<int>]
    public void CreateParams_Height_Set_GetReturnsExpected(int value)
    {
        var createParams = new CreateParams
        {
            Height = value
        };
        Assert.Equal(value, createParams.Height);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetIntPtrTheoryData))]
    public void CreateParams_Parent_Set_GetReturnsExpected(IntPtr value)
    {
        var createParams = new CreateParams
        {
            Parent = value
        };
        Assert.Equal(value, createParams.Parent);
    }

    [Theory]
    [StringWithNullData]
    public void CreateParams_Param_Set_GetReturnsExpected(string value)
    {
        var createParams = new CreateParams
        {
            Param = value
        };
        Assert.Equal(value, createParams.Param);
    }

    [Fact]
    public void CreateParams_ToString_Invoke_ReturnsExpected()
    {
        var createParams = new CreateParams
        {
            ClassName = "className",
            Caption = "caption",
            Style = 10,
            ExStyle = 11,
            X = 12,
            Y = 13,
            Width = 14,
            Height = 15,
            Parent = (IntPtr)16,
            Param = "param"
        };
        Assert.Equal("CreateParams {'className', 'caption', 0xa, 0xb, {12, 13, 14, 15}}", createParams.ToString());
    }
}

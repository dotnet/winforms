// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class PreviewKeyDownEventArgsTests
{
    [Theory]
    [InlineData(Keys.A)]
    [InlineData(Keys.Control | Keys.A)]
    [InlineData(Keys.Alt | Keys.A)]
    [InlineData(Keys.Shift | Keys.A)]
    [InlineData(Keys.Control)]
    [InlineData(Keys.Alt)]
    [InlineData(Keys.Shift)]
    [InlineData(Keys.Control | Keys.Alt | Keys.Shift | Keys.A)]
    [InlineData((Keys)(-1))]
    [InlineData((Keys)(0x5D))]
    [InlineData((Keys)(0xFF))]
    [InlineData(Keys.Control | Keys.Alt | Keys.Shift | (Keys)(0x5D))]
    public void Ctor_Keys(Keys keyData)
    {
        PreviewKeyDownEventArgs e = new(keyData);
        Assert.Equal(keyData, e.KeyData);
        Assert.Equal((keyData & Keys.Control) == Keys.Control, e.Control);
        Assert.Equal((keyData & Keys.Alt) == Keys.Alt, e.Alt);
        Assert.Equal((keyData & Keys.Shift) == Keys.Shift, e.Shift);
        Assert.Equal((keyData & Keys.Modifiers), e.Modifiers);
        Assert.Equal((int)(keyData & Keys.KeyCode), e.KeyValue);
        Assert.False(e.IsInputKey);

        if (Enum.IsDefined(typeof(Keys), e.KeyValue))
        {
            Assert.Equal((Keys)e.KeyValue, e.KeyCode);
        }
        else
        {
            Assert.Equal(Keys.None, e.KeyCode);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsInputKey_Set_GetReturnsExpected(bool value)
    {
        PreviewKeyDownEventArgs e = new(Keys.A)
        {
            IsInputKey = value
        };
        Assert.Equal(value, e.IsInputKey);
    }
}

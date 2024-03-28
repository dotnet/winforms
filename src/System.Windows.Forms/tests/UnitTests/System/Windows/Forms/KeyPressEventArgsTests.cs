// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class KeyPressEventArgsTests
{
    [Theory]
    [InlineData('\0')]
    [InlineData('a')]
    public void Ctor_Char(char keyChar)
    {
        KeyPressEventArgs e = new(keyChar);
        Assert.Equal(keyChar, e.KeyChar);
        Assert.False(e.Handled);
    }

    [Theory]
    [InlineData('\0')]
    [InlineData('a')]
    public void KeyChar_Set_GetReturnsExpected(char value)
    {
        KeyPressEventArgs e = new('b')
        {
            KeyChar = value
        };
        Assert.Equal(value, e.KeyChar);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Handled_Set_GetReturnsExpected(bool value)
    {
        KeyPressEventArgs e = new('a')
        {
            Handled = value
        };
        Assert.Equal(value, e.Handled);
    }
}

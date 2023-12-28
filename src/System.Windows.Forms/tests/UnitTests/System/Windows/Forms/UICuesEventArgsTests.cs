// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class UICuesEventArgsTests
{
    [Theory]
    [InlineData((UICues)(-1))]
    [InlineData(UICues.None)]
    [InlineData(UICues.ShowFocus)]
    [InlineData(UICues.ShowKeyboard)]
    [InlineData(UICues.Shown)]
    [InlineData(UICues.ChangeFocus)]
    [InlineData(UICues.ChangeKeyboard)]
    [InlineData(UICues.Changed)]
    [InlineData(UICues.Changed | UICues.Shown)]
    public void Ctor_UICues(UICues uicues)
    {
        UICuesEventArgs e = new(uicues);
        Assert.Equal((uicues & UICues.ShowFocus) != 0, e.ShowFocus);
        Assert.Equal((uicues & UICues.ShowKeyboard) != 0, e.ShowKeyboard);
        Assert.Equal((uicues & UICues.ChangeFocus) != 0, e.ChangeFocus);
        Assert.Equal((uicues & UICues.ChangeKeyboard) != 0, e.ChangeKeyboard);
        Assert.Equal(uicues & UICues.Changed, e.Changed);
    }
}

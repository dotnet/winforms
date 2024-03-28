// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class ControlEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Control_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new Button() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Control_TestData))]
    public void Ctor_Control(Control control)
    {
        ControlEventArgs e = new(control);
        Assert.Equal(control, e.Control);
    }
}

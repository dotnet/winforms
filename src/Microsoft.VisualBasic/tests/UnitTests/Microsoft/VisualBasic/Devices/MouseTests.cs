// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace Microsoft.VisualBasic.Devices.Tests;

public class MouseTests
{
    public static bool NoMousePresent => !SystemInformation.MousePresent;

    public static bool NoMouseWheelPresent => NoMousePresent || !SystemInformation.MouseWheelPresent;

    [ConditionalFact(typeof(SystemInformation), nameof(SystemInformation.MousePresent))]
    public void Mouse_ButtonsSwapped_Get_ReturnsExpected()
    {
        Mouse mouse = new();
        Assert.Equal(SystemInformation.MouseButtonsSwapped, mouse.ButtonsSwapped);
        Assert.Equal(mouse.ButtonsSwapped, mouse.ButtonsSwapped);
    }

    [ConditionalFact(nameof(NoMousePresent))]
    public void Mouse_ButtonsSwapped_GetNoMousePresent_ThrowsInvalidOperationException()
    {
        if (NoMousePresent)
        {
            Mouse mouse = new();
            Assert.Throws<InvalidOperationException>(() => mouse.ButtonsSwapped);
        }
    }

    [ConditionalFact(typeof(SystemInformation), nameof(SystemInformation.MousePresent))]
    public void Mouse_WheelExists_Get_ReturnsExpected()
    {
        Mouse mouse = new();
        Assert.Equal(SystemInformation.MouseWheelPresent, mouse.WheelExists);
        Assert.Equal(mouse.WheelExists, mouse.WheelExists);
    }

    [ConditionalFact(nameof(NoMousePresent))]
    public void Mouse_WheelExists_GetNoMousePresent_ThrowsInvalidOperationException()
    {
        if (NoMousePresent)
        {
            Mouse mouse = new();
            Assert.Throws<InvalidOperationException>(() => mouse.WheelExists);
        }
    }

    [ConditionalFact(typeof(SystemInformation), nameof(SystemInformation.MousePresent), nameof(SystemInformation.MouseWheelPresent))]
    public void Mouse_WheelScrollLines_Get_ReturnsExpected()
    {
        if (SystemInformation.MouseWheelPresent)
        {
            Mouse mouse = new();
            Assert.Equal(SystemInformation.MouseWheelScrollLines, mouse.WheelScrollLines);
            Assert.Equal(mouse.WheelScrollLines, mouse.WheelScrollLines);
        }
    }

    [ConditionalFact(nameof(NoMouseWheelPresent))]
    public void Mouse_WheelScrollLines_GetNoMouseWheelPresent_ThrowsInvalidOperationException()
    {
        if (NoMouseWheelPresent)
        {
            Mouse mouse = new();
            Assert.Throws<InvalidOperationException>(() => mouse.WheelScrollLines);
        }
    }
}

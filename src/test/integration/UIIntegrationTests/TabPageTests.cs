// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.UITests;

// Migrated from unit tests; see issue #4500.
public class TabPageTests
{
    [WinFormsTheory]
    [BoolData]
    public static void TabPage_BackColor_GetVisualStyles_ReturnsExpected(bool useVisualStyleBackColor)
    {
        Application.EnableVisualStyles();

        using TabPage control = new()
        {
            UseVisualStyleBackColor = useVisualStyleBackColor
        };
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
    }

    public static IEnumerable<object[]> BackColor_GetVisualStylesWithParent_TestData()
    {
        yield return new object[] { true, TabAppearance.Buttons, Control.DefaultBackColor };
        yield return new object[] { true, TabAppearance.FlatButtons, Control.DefaultBackColor };
        yield return new object[] { true, TabAppearance.Normal, Color.Transparent };
        yield return new object[] { false, TabAppearance.Buttons, Control.DefaultBackColor };
        yield return new object[] { false, TabAppearance.FlatButtons, Control.DefaultBackColor };
        yield return new object[] { false, TabAppearance.Normal, Control.DefaultBackColor };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_GetVisualStylesWithParent_TestData))]
    public static void TabPage_BackColor_GetVisualStylesWithParent_ReturnsExpected(bool useVisualStyleBackColor, TabAppearance parentAppearance, Color expected)
    {
        Application.EnableVisualStyles();

        using TabControl parent = new()
        {
            Appearance = parentAppearance
        };
        using TabPage control = new()
        {
            UseVisualStyleBackColor = useVisualStyleBackColor,
            Parent = parent
        };
        Assert.Equal(expected.ToArgb(), control.BackColor.ToArgb());
    }
}

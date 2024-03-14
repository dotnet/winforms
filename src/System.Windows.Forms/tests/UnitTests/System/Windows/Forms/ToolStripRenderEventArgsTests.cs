// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripRenderEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Null_Graphics_ToolStrip_TestData()
    {
        Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);

        yield return new object[] { null, null };
        yield return new object[] { null, new ToolStrip() };
        yield return new object[] { graphics, null };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Null_Graphics_ToolStrip_TestData))]
    public void ToolStripRenderEventArgs_Null_Graphics_ToolStrip_ThrowsArgumentNullException(Graphics g, ToolStrip toolStrip)
    {
        Assert.Throws<ArgumentNullException>(() => new ToolStripRenderEventArgs(g, toolStrip));
    }

    public static IEnumerable<object[]> Ctor_Graphics_ToolStrip_TestData()
    {
        yield return new object[] { new ToolStrip(), new Rectangle(0, 0, 100, 25), SystemColors.Control };
        yield return new object[] { new ToolStrip() { BackColor = Color.Red }, new Rectangle(0, 0, 100, 25), Color.Red };
        yield return new object[] { new ToolStripDropDown(), new Rectangle(0, 0, 100, 25), SystemColors.Menu };
        yield return new object[] { new MenuStrip(), new Rectangle(0, 0, 200, 24), SystemColors.MenuBar };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Graphics_ToolStrip_TestData))]
    public void ToolStripRenderEventArgs_Ctor_Graphics_ToolStrip(ToolStrip toolStrip, Rectangle expectedAffectedBounds, Color expectedBackColor)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);

        ToolStripRenderEventArgs e = new(graphics, toolStrip);

        Assert.Same(graphics, e.Graphics);
        Assert.Same(toolStrip, e.ToolStrip);
        Assert.Equal(expectedAffectedBounds, e.AffectedBounds);
        Assert.Equal(expectedBackColor, e.BackColor);
    }

    public static IEnumerable<object[]> Ctor_Graphics_ToolStrip_Rectangle_Color_TestData()
    {
        yield return new object[] { new ToolStrip(), new Rectangle(1, 2, 3, 4), Color.Empty, SystemColors.Control };
        yield return new object[] { new ToolStrip() { BackColor = Color.Red }, new Rectangle(1, 2, 3, 4), Color.Empty, Color.Red };
        yield return new object[] { new ToolStripDropDown(), new Rectangle(1, 2, 3, 4), Color.Empty, SystemColors.Menu };
        yield return new object[] { new MenuStrip(), new Rectangle(1, 2, 3, 4), Color.Empty, SystemColors.MenuBar };
        yield return new object[] { new ToolStrip() { BackColor = Color.Red }, new Rectangle(1, 2, 3, 4), Color.Blue, Color.Blue };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Graphics_ToolStrip_Rectangle_Color_TestData))]
    public void ToolStripRenderEventArgs_Ctor_Graphics_ToolStrip_Rectangle_Color(ToolStrip toolStrip, Rectangle affectedBounds, Color backColor, Color expectedBackColor)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);

        ToolStripRenderEventArgs e = new(graphics, toolStrip, affectedBounds, backColor);

        Assert.Same(graphics, e.Graphics);
        Assert.Same(toolStrip, e.ToolStrip);
        Assert.Equal(affectedBounds, e.AffectedBounds);
        Assert.Equal(expectedBackColor, e.BackColor);
    }

    public static IEnumerable<object[]> ConnectedArea_Empty_TestData()
    {
        yield return new object[] { new ToolStrip() };
        yield return new object[] { new ToolStripDropDown() };
        yield return new object[] { new ToolStripOverflow(new ToolStripButton()) };
        yield return new object[] { new ToolStripOverflow(new ToolStripDropDownButton()) };

        ToolStripDropDownButton ownedDropDownItem = new();
        ToolStrip owner = new();
        owner.Items.Add(ownedDropDownItem);
        yield return new object[] { new ToolStripOverflow(ownedDropDownItem) };
        yield return new object[] { new ToolStripOverflow(owner.OverflowButton) };
    }

    [WinFormsTheory]
    [MemberData(nameof(ConnectedArea_Empty_TestData))]
    public void ToolStripRenderEventArgs_ConnectedArea_Get_ReturnsEmpty(ToolStrip toolStrip)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ToolStripRenderEventArgs e = new(graphics, toolStrip, new Rectangle(1, 2, 3, 4), Color.Empty);
        Assert.Equal(Rectangle.Empty, e.ConnectedArea);
    }
}

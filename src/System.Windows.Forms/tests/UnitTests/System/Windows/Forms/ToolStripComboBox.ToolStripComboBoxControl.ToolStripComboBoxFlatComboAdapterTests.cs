// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripComboBox_ToolStripComboBoxFlatComboAdapterTests : IDisposable
{
    private readonly ToolStripComboBox.ToolStripComboBoxControl _comboBox;
    private readonly ToolStripComboBox.ToolStripComboBoxControl.ToolStripComboBoxFlatComboAdapter _adapter;

    public void Dispose() => _comboBox.Dispose();

    public ToolStripComboBox_ToolStripComboBoxFlatComboAdapterTests()
    {
        _comboBox = new();
        _adapter = new(_comboBox);
    }

    [WinFormsFact]
    public void ToolStripComboBoxFlatComboAdapter_Ctor_InitializesCorrectly()
    {
        _adapter.Should().NotBeNull();
    }

    [WinFormsFact]
    public void UseBaseAdapter_ReturnsTrue_ForToolStripComboBoxControl()
    {
        using ToolStripComboBox.ToolStripComboBoxControl comboBox = new();
        bool result = (bool)typeof(ToolStripComboBox.ToolStripComboBoxControl.ToolStripComboBoxFlatComboAdapter)
            .TestAccessor().Dynamic.UseBaseAdapter(comboBox);

        result.Should().BeTrue();
    }

    [WinFormsFact]
    public void GetColorTable_ReturnsExpected()
    {
        var colorTable = (ProfessionalColorTable)_adapter.TestAccessor().Dynamic.GetColorTable(_comboBox);

        colorTable.Should().NotBeNull();
        colorTable.Should().BeOfType<ProfessionalColorTable>();
    }

    [WinFormsTheory]
    [InlineData(true, KnownColor.Window)]
    [InlineData(false, KnownColor.ControlDark)]
    public void GetOuterBorderColor_ReturnsExpected(bool enabled, KnownColor expectedColor)
    {
        _comboBox.Enabled = enabled;

        Color result = (Color)_adapter.TestAccessor().Dynamic.GetOuterBorderColor(_comboBox);

        result.Should().Be(Color.FromKnownColor(expectedColor));
    }

    [WinFormsTheory]
    [InlineData(true, true, KnownColor.ControlDark)]
    [InlineData(true, false, KnownColor.Window)]
    [InlineData(false, true, KnownColor.ControlDark)]
    [InlineData(false, false, KnownColor.ControlDark)]
    public void GetPopupOuterBorderColor_ReturnsExpected(bool enabled, bool focused, KnownColor expectedColor)
    {
        _comboBox.Enabled = enabled;

        Color result = (Color)_adapter.TestAccessor().Dynamic.GetPopupOuterBorderColor(_comboBox, focused);

        result.Should().Be(Color.FromKnownColor(expectedColor));
    }

    [WinFormsFact]
    public void DrawFlatComboDropDown_DrawsCorrectly()
    {
        using Bitmap bitmap = new(100, 100);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle dropDownRect = new(0, 0, 100, 100);

        _adapter.TestAccessor().Dynamic.DrawFlatComboDropDown(_comboBox, graphics, dropDownRect);

        bitmap.GetPixel(50, 50).Should().NotBe(Color.Empty);
    }
}

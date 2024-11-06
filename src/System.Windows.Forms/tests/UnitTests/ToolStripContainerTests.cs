// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection;

namespace System.Windows.Forms.Tests;

public class ToolStripContainerTests : IDisposable
{
    private readonly ToolStripContainer _toolStripContainer;

    public ToolStripContainerTests()
    {
        _toolStripContainer = new();
    }

    public void Dispose()
    {
        _toolStripContainer.Dispose();
    }

    [WinFormsFact]
    public void ToolStripContainer_Constructor()
    {
        _toolStripContainer.Should().NotBeNull();
        _toolStripContainer.TopToolStripPanel.Should().NotBeNull();
        _toolStripContainer.BottomToolStripPanel.Should().NotBeNull();
        _toolStripContainer.LeftToolStripPanel.Should().NotBeNull();
        _toolStripContainer.RightToolStripPanel.Should().NotBeNull();
        _toolStripContainer.ContentPanel.Should().NotBeNull();
        _toolStripContainer.TopToolStripPanel.Dock.Should().Be(DockStyle.Top);
        _toolStripContainer.BottomToolStripPanel.Dock.Should().Be(DockStyle.Bottom);
        _toolStripContainer.LeftToolStripPanel.Dock.Should().Be(DockStyle.Left);
        _toolStripContainer.RightToolStripPanel.Dock.Should().Be(DockStyle.Right);
        _toolStripContainer.Controls.Should().NotBeNull();
        _toolStripContainer.Controls.Count.Should().Be(5);
    }

    [WinFormsTheory]
    [InlineData("AutoScrollMargin")]
    [InlineData("AutoScrollMinSize")]
    public void ToolStripContainer_SizeProperties_GetSet_ReturnsExpected(string propertyName)
    {
        Size value = new(10, 10);
        PropertyInfo propertyInfo = _toolStripContainer.GetType().GetProperty(propertyName);
        propertyInfo.SetValue(_toolStripContainer, value);
        Size result = (Size)propertyInfo.GetValue(_toolStripContainer);

        result.Should().Be(value);
    }

    [WinFormsFact]
    public void ToolStripContainer_BackColorChanged_AddRemoveEvent_MaintainsExpected()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) => callCount++;
        _toolStripContainer.BackColorChanged += handler;

        _toolStripContainer.BackColor = Color.Red;
        callCount.Should().Be(1);

        _toolStripContainer.BackColorChanged -= handler;

        _toolStripContainer.BackColor = Color.Blue;
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void ToolStripContainer_BackgroundImage_GetSet_ReturnsExpected()
    {
        using Image value = new Bitmap(10, 10);
        _toolStripContainer.BackgroundImage = value;

        _toolStripContainer.BackgroundImage.Should().Be(value);
    }

    [WinFormsFact]
    public void ToolStripContainer_BackgroundImageChanged_AddRemoveEvent_MaintainsExpected()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) => callCount++;
        _toolStripContainer.BackgroundImageChanged += handler;

        _toolStripContainer.BackgroundImage = new Bitmap(10, 10);
        callCount.Should().Be(1);

        _toolStripContainer.BackgroundImageChanged -= handler;

        _toolStripContainer.BackgroundImage = new Bitmap(20, 20);
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void ToolStripContainer_BackgroundImageLayoutChanged_AddRemoveEvent_MaintainsExpected()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) => callCount++;
        _toolStripContainer.BackgroundImageLayoutChanged += handler;

        _toolStripContainer.BackgroundImageLayout = ImageLayout.Center;
        callCount.Should().Be(1);

        _toolStripContainer.BackgroundImageLayoutChanged -= handler;

        _toolStripContainer.BackgroundImageLayout = ImageLayout.Stretch;
        callCount.Should().BeGreaterThan(1);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripContainer_BottomToolStripPanelVisible_GetSet_ReturnsExpected(bool value)
    {
        _toolStripContainer.BottomToolStripPanelVisible = value;
        _toolStripContainer.BottomToolStripPanelVisible.Should().Be(value);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripContainer_CausesValidation_GetSet_ReturnsExpected(bool value)
    {
        _toolStripContainer.CausesValidation = value;
        _toolStripContainer.CausesValidation.Should().Be(value);
    }

    [WinFormsFact]
    public void ToolStripContainer_ContextMenuStrip_GetSet_ReturnsExpected()
    {
        using ContextMenuStrip contextMenuStrip = new();

        _toolStripContainer.ContextMenuStrip = contextMenuStrip;
        _toolStripContainer.ContextMenuStrip.Should().Be(contextMenuStrip);

        _toolStripContainer.ContextMenuStrip = null;
        _toolStripContainer.ContextMenuStrip.Should().BeNull();
    }

    [WinFormsFact]
    public void ToolStripContainer_CausesValidationChanged_AddRemove_Success()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) => callCount++;
        _toolStripContainer.CausesValidationChanged += handler;
        _toolStripContainer.CausesValidation = !_toolStripContainer.CausesValidation;
        callCount.Should().Be(1);

        _toolStripContainer.CausesValidationChanged -= handler;
        _toolStripContainer.CausesValidation = !_toolStripContainer.CausesValidation;
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void ToolStripContainer_ContextMenuStripChanged_AddRemove_Success()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) => callCount++;
        _toolStripContainer.ContextMenuStripChanged += handler;
        _toolStripContainer.ContextMenuStrip = new ContextMenuStrip();

        callCount.Should().Be(1);
        _toolStripContainer.ContextMenuStripChanged -= handler;
        _toolStripContainer.ContextMenuStrip = null;
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void ToolStripContainer_Cursor_GetSet_ReturnsExpected()
    {
        var value = Cursors.Hand;
        _toolStripContainer.Cursor = value;
        _toolStripContainer.Cursor.Should().Be(value);

        value = Cursors.Default;
        _toolStripContainer.Cursor = value;
        _toolStripContainer.Cursor.Should().Be(value);
    }

    [WinFormsFact]
    public void ToolStripContainer_CursorChanged_AddRemove_Success()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) => callCount++;
        _toolStripContainer.CursorChanged += handler;
        _toolStripContainer.Cursor = Cursors.Hand;

        callCount.Should().Be(1);
        _toolStripContainer.CursorChanged -= handler;
        _toolStripContainer.Cursor = Cursors.Default;
        callCount.Should().Be(1);
    }

    [WinFormsTheory]
    [InlineData(typeof(Color), "Red")]
    [InlineData(typeof(Color), "Blue")]
    [InlineData(typeof(Color), "Black")]
    public void ToolStripContainer_ForeColor_GetSetAndEventFired_Success(Type colorType, string colorName)
    {
        int callCount = 0;
        EventHandler handler = (sender, e) => callCount++;
        _toolStripContainer.ForeColorChanged += handler;

        Color colorValue = (Color)colorType.GetProperty(colorName).GetValue(null);
        _toolStripContainer.ForeColor = colorValue;
        _toolStripContainer.ForeColor.Should().Be(colorValue);
        callCount.Should().Be(1);

        _toolStripContainer.ForeColorChanged -= handler;
        _toolStripContainer.ForeColor = Color.Red;
        callCount.Should().Be(1);
    }

    [WinFormsTheory]
    [InlineData("TopToolStripPanelVisible")]
    [InlineData("RightToolStripPanelVisible")]
    [InlineData("LeftToolStripPanelVisible")]
    public void ToolStripContainer_PanelVisible_GetSet_ReturnsExpected(string propertyName)
    {
        PropertyInfo propertyInfo = _toolStripContainer.GetType().GetProperty(propertyName);

        bool defaultValue = (bool)propertyInfo.GetValue(_toolStripContainer);
        defaultValue.Should().BeTrue();

        propertyInfo.SetValue(_toolStripContainer, true);
        bool value = (bool)propertyInfo.GetValue(_toolStripContainer);
        value.Should().BeTrue();

        propertyInfo.SetValue(_toolStripContainer, false);
        value = (bool)propertyInfo.GetValue(_toolStripContainer);
        value.Should().BeFalse();
    }
}

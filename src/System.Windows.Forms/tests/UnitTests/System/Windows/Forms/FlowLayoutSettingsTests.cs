// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Tests;

public class FlowLayoutSettingsTests
{
    [WinFormsTheory]
    [InlineData(FlowDirection.BottomUp, 1)]
    [InlineData(FlowDirection.LeftToRight, 1)]
    [InlineData(FlowDirection.RightToLeft, 1)]
    [InlineData(FlowDirection.TopDown, 1)]
    public void FlowLayoutSettings_FlowDirection_Set_GetReturnsExpected(FlowDirection value, int expectedLayoutCallCount)
    {
        using ToolStrip control = new()
        {
            LayoutStyle = ToolStripLayoutStyle.Flow
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("FlowDirection", e.AffectedProperty);
            layoutCallCount++;
        };
        FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(control.LayoutSettings);

        settings.FlowDirection = value;
        Assert.Equal(value, settings.FlowDirection);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same
        settings.FlowDirection = value;
        Assert.Equal(value, settings.FlowDirection);
        Assert.Equal(expectedLayoutCallCount * 2, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InvalidEnumData<FlowDirection>]
    public void FlowLayoutSettings_FlowDirection_SetInvalidValue_ThrowsInvalidEnumArgumentException(FlowDirection value)
    {
        using ToolStrip control = new()
        {
            LayoutStyle = ToolStripLayoutStyle.Flow
        };
        FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(control.LayoutSettings);
        Assert.Throws<InvalidEnumArgumentException>("value", () => settings.FlowDirection = value);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 1)]
    public void FlowLayoutSettings_WrapContents_Set_GetReturnsExpected(bool value, int expectedLayoutCallCount)
    {
        using ToolStrip control = new()
        {
            LayoutStyle = ToolStripLayoutStyle.Flow
        };
        FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(control.LayoutSettings);
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("WrapContents", e.AffectedProperty);
            layoutCallCount++;
        };

        settings.WrapContents = value;
        Assert.Equal(value, settings.WrapContents);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same
        settings.WrapContents = value;
        Assert.Equal(value, settings.WrapContents);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set different
        settings.WrapContents = !value;
        Assert.Equal(!value, settings.WrapContents);
        Assert.Equal(expectedLayoutCallCount + 1 + 1, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void FlowLayoutSettings_GetFlowBreak_InvokeValidControl_ReturnsExpected()
    {
        using ToolStrip control = new()
        {
            LayoutStyle = ToolStripLayoutStyle.Flow
        };
        FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(control.LayoutSettings);
        Assert.False(settings.GetFlowBreak(new Control()));
    }

    [WinFormsFact]
    public void FlowLayoutSettings_GetFlowBreak_NullChild_ThrowsArgumentNullException()
    {
        using ToolStrip control = new()
        {
            LayoutStyle = ToolStripLayoutStyle.Flow
        };
        FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(control.LayoutSettings);
        Assert.Throws<ArgumentNullException>("child", () => settings.GetFlowBreak(null));
    }

    [WinFormsFact]
    public void FlowLayoutSettings_GetFlowBreak_InvalidChild_ThrowsNotSupportedException()
    {
        using ToolStrip control = new()
        {
            LayoutStyle = ToolStripLayoutStyle.Flow
        };
        FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(control.LayoutSettings);
        Assert.Throws<NotSupportedException>(() => settings.GetFlowBreak(new object()));
    }

    [WinFormsTheory]
    [BoolData]
    public void FlowLayoutSettings_SetFlowBreak_Invoke_GetFlowBreakReturnsExpected(bool value)
    {
        using ToolStrip control = new()
        {
            LayoutStyle = ToolStripLayoutStyle.Flow
        };
        FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(control.LayoutSettings);
        using Control child = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;

        settings.SetFlowBreak(child, value);
        Assert.Equal(value, settings.GetFlowBreak(child));
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(child.IsHandleCreated);

        // Set same.
        settings.SetFlowBreak(child, value);
        Assert.Equal(value, settings.GetFlowBreak(child));
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(child.IsHandleCreated);

        // Set different.
        settings.SetFlowBreak(child, !value);
        Assert.Equal(!value, settings.GetFlowBreak(child));
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, childLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(child.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void FlowLayoutSettings_SetFlowBreak_InvokeControlWithParent_GetFlowBreakReturnsExpected(bool value, int expectedParentLayoutCallCount)
    {
        using ToolStrip control = new()
        {
            LayoutStyle = ToolStripLayoutStyle.Flow
        };
        FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(control.LayoutSettings);
        using Control parent = new();
        using Control child = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int childLayoutCallCount = 0;
        child.Layout += (sender, e) => childLayoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs eventArgs)
        {
            Assert.Same(parent, sender);
            Assert.Same(child, eventArgs.AffectedControl);
            Assert.Equal("FlowBreak", eventArgs.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            settings.SetFlowBreak(child, value);
            Assert.Equal(value, settings.GetFlowBreak(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);

            // Set same.
            settings.SetFlowBreak(child, value);
            Assert.Equal(value, settings.GetFlowBreak(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);

            // Set different.
            settings.SetFlowBreak(child, !value);
            Assert.Equal(!value, settings.GetFlowBreak(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount + 1, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [BoolData]
    public void FlowLayoutSettings_SetFlowBreak_NullChild_ThrowsArgumentNullException(bool value)
    {
        using ToolStrip control = new()
        {
            LayoutStyle = ToolStripLayoutStyle.Flow
        };
        FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(control.LayoutSettings);
        Assert.Throws<ArgumentNullException>("child", () => settings.SetFlowBreak(null, value));
    }

    [WinFormsTheory]
    [BoolData]
    public void FlowLayoutSettings_SetFlowBreak_InvalidChild_ThrowsNotSupportedException(bool value)
    {
        using ToolStrip control = new()
        {
            LayoutStyle = ToolStripLayoutStyle.Flow
        };
        FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(control.LayoutSettings);
        Assert.Throws<NotSupportedException>(() => settings.SetFlowBreak(new object(), value));
    }
}

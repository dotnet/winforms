// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design.Tests;

public class DocumentDesignerTests
{
    private sealed class TestDocumentDesigner : DocumentDesigner
    {
        public TestDocumentDesigner() { }

        public new GlyphCollection GetGlyphs(GlyphSelectionType selectionType) => base.GetGlyphs(selectionType);

        public bool CanDropComponentsPublic(DragEventArgs de) => CanDropComponents(de);

        public bool GetToolSupportedPublic(ToolboxItem tool) => GetToolSupported(tool);

        public object? IRootDesigner_GetView(ViewTechnology technology) =>
            ((IRootDesigner)this).GetView(technology);

        public ViewTechnology[] IRootDesigner_SupportedTechnologies =>
            ((IRootDesigner)this).SupportedTechnologies;

        public bool IToolboxUser_GetToolSupported(ToolboxItem tool) =>
            ((IToolboxUser)this).GetToolSupported(tool);

        public Control? IOleDragClient_GetControlForComponent(object component) =>
            ((IOleDragClient)this).GetControlForComponent(component);
    }

    [Fact]
    public void GetGlyphs_NotSelected_ReturnsEmpty()
    {
        TestDocumentDesigner designer = new();
        GlyphCollection glyphs = designer.GetGlyphs(GlyphSelectionType.NotSelected);

        glyphs.Should().NotBeNull();
        glyphs.Count.Should().Be(0);
    }

    [Fact]
    public void GetToolSupported_ReturnsTrueByDefault()
    {
        TestDocumentDesigner designer = new();
        ToolboxItem tool = new TestToolboxItem();

        bool result = designer.GetToolSupportedPublic(tool);

        result.Should().BeTrue();
    }

    [Fact]
    public void IRootDesigner_SupportedTechnologies_ReturnsExpected()
    {
        TestDocumentDesigner designer = new();

        ViewTechnology[] techs = designer.IRootDesigner_SupportedTechnologies;

        techs.Should().Contain(ViewTechnology.Default);
        techs.Should().Contain((ViewTechnology)1);
    }

    [Fact]
    public void IRootDesigner_GetView_ThrowsForUnknownTech()
    {
        TestDocumentDesigner designer = new();

        Action act = () => designer.IRootDesigner_GetView((ViewTechnology)42);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void IToolboxUser_GetToolSupported_DelegatesToGetToolSupported()
    {
        TestDocumentDesigner designer = new();
        ToolboxItem tool = new TestToolboxItem();

        bool result = designer.IToolboxUser_GetToolSupported(tool);

        result.Should().BeTrue();
    }

    [Fact]
    public void IOleDragClient_GetControlForComponent_ReturnsNullIfNoTray()
    {
        TestDocumentDesigner designer = new();

        Control? result = designer.IOleDragClient_GetControlForComponent(new object());

        result.Should().BeNull();
    }

    [Fact]
    public void CanDropComponents_ReturnsTrueIfNoTray()
    {
        TestDocumentDesigner designer = new();

        // Use a minimal valid DataObject for DragEventArgs
        DataObject data = new();
        DragEventArgs args = new(data, 0, 0, 0, DragDropEffects.None, DragDropEffects.None);

        bool result = designer.CanDropComponentsPublic(args);

        result.Should().BeTrue();
    }

    private sealed class TestToolboxItem : ToolboxItem
    {
        public TestToolboxItem() : base(typeof(Button))
        {
        }
    }
}

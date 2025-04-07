// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms.Design.Behavior;
using Moq;

namespace System.Windows.Forms.Design.Tests;
public class ParentControlDesignerTests : IDisposable
{
    private readonly ParentControlDesigner _designer;
    private readonly Control _control;

    public ParentControlDesignerTests()
    {
        _designer = new();
        _control = new();
        _designer.Initialize(_control);
    }

    public void Dispose()
    {
        _control.Dispose();
        _designer.Dispose();
    }

    [Fact]
    public void SnapLines_ReturnsExpectedSnapLines()
    {
        IList<SnapLine> snapLines = _designer.SnapLines.Cast<SnapLine>().ToList();

        snapLines.Should().NotBeNull();
        snapLines.Should().BeOfType<List<SnapLine>>();
        ((List<SnapLine>)snapLines).Should().NotBeEmpty();
        ((List<SnapLine>)snapLines).Should().OnlyHaveUniqueItems();

        ((List<SnapLine>)snapLines).Should()
            .Contain(sl => sl.SnapLineType == SnapLineType.Top)
            .And.Contain(sl => sl.SnapLineType == SnapLineType.Bottom)
            .And.Contain(sl => sl.SnapLineType == SnapLineType.Left)
            .And.Contain(sl => sl.SnapLineType == SnapLineType.Right);
    }

    [Fact]
    public void CanParent_ControlDesigner_ReturnsExpected()
    {
        Mock<ControlDesigner> mockControlDesigner = new();
        Mock<Control> mockControl = new();
        mockControlDesigner.Setup(cd => cd.Control).Returns(mockControl.Object);

        _designer.CanParent(mockControlDesigner.Object).Should().BeTrue();
    }

    [Fact]
    public void CanParent_Control_ReturnsExpected()
    {
        using TestControl testControl = new() { ContainsControl = false };

        _designer.CanParent(testControl).Should().BeTrue();
    }

    [Fact]
    public void GetGlyphs_SelectionTypeSelected_AddsContainerSelectorGlyph()
    {
        Mock<IServiceProvider> mockServiceProvider = new();
        Mock<ISite> mockSite = new();
        Mock<IDesignerHost> mockDesignerHost = new();
        Mock<IComponentChangeService> mockChangeService = new();
        _control.Site = mockSite.Object;
        mockSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(mockDesignerHost.Object);
        mockSite.Setup(s => s.GetService(typeof(IComponentChangeService))).Returns(mockChangeService.Object);

        Mock<DesignerFrame> mockDesignerFrame = new(mockSite.Object) { CallBase = true };
        BehaviorService behaviorService = new(mockServiceProvider.Object, mockDesignerFrame.Object);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(BehaviorService))).Returns(() => behaviorService);
        mockSite.Setup(s => s.GetService(typeof(BehaviorService))).Returns(behaviorService);

        _designer.TestAccessor().Dynamic._behaviorService = behaviorService;
        _designer.TestAccessor().Dynamic._host = mockDesignerHost.Object;
        _designer.TestAccessor().Dynamic._changeService = mockChangeService.Object;

        GlyphSelectionType selectionType = GlyphSelectionType.Selected;

        GlyphCollection glyphs = _designer.GetGlyphs(selectionType);

        glyphs.Should().BeOfType<GlyphCollection>();
        glyphs.OfType<ContainerSelectorGlyph>().Count().Should().Be(1);
    }

    [Fact]
    public void InitializeNewComponent_WithNullDefaultValues_DoesNotThrow()
    {
        _designer.Invoking(d => d.InitializeNewComponent(null)).Should().NotThrow();
    }

    [Fact]
    public void InitializeNewComponent_ShouldReparentControls_WhenAllowControlLassoIsTrue()
    {
        Mock<IDesignerHost> mockDesignerHost = new();
        Mock<ISite> mockSite = new();
        Mock<ISelectionService> mockSelectionService = new();
        Mock<IComponentChangeService> mockChangeService = new();

        using TestControl testComponent = new();
        using TestControl parentComponent = new();

        mockSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(mockDesignerHost.Object);
        mockSite.Setup(s => s.GetService(typeof(ISelectionService))).Returns(mockSelectionService.Object);
        mockSite.Setup(s => s.GetService(typeof(IComponentChangeService))).Returns(mockChangeService.Object);
        testComponent.Site = mockSite.Object;
        parentComponent.Site = mockSite.Object;

        mockDesignerHost.Setup(h => h.GetDesigner(parentComponent)).Returns(_designer);

        _designer.Initialize(testComponent);
        _designer.TestAccessor().Dynamic._changeService = mockChangeService.Object;

        Dictionary<string, object> defaultValues = new()
        {
            { "Size", new Size(100, 100) },
            { "Location", new Point(10, 10) },
            { "Parent", parentComponent }
        };

        DisableDragDrop(testComponent);
        DisableDragDrop(parentComponent);

        _designer.InitializeNewComponent(defaultValues);

        mockDesignerHost.Verify(h => h.GetDesigner(parentComponent), Times.AtMost(2));
    }

    public class TestControl : Control
    {
        public bool ContainsControl { get; set; }

        public new bool Contains(Control ctl)
        {
            return ContainsControl;
        }

        public new ControlCollection Controls => base.Controls;
    }

    private void DisableDragDrop(Control control)
    {
        if (control is null)
            return;

        control.AllowDrop = false;
        foreach (Control child in control.Controls)
        {
            DisableDragDrop(child);
        }
    }

    [Fact]
    public void AllowSetChildIndexOnDrop_ReturnsTrue()
    {
        _designer.AllowSetChildIndexOnDrop.Should().BeTrue();
    }

    [Fact]
    public void CanAddComponent_ReturnsTrue()
    {
        Mock<IComponent> mockComponent = new();
        _designer.CanAddComponent(mockComponent.Object).Should().BeTrue();
    }

    [Fact]
    public void EnableDragRect_ReturnsTrue()
    {
        bool enableDragRect = _designer.TestAccessor().Dynamic.EnableDragRect;
        enableDragRect.Should().BeTrue();
    }

    [Fact]
    public void GridSize_DefaultValue_ReturnsExpected()
    {
        Size gridSize = _designer.TestAccessor().Dynamic.GridSize;
        gridSize.Should().Be(new Size(8, 8));
    }

    [Fact]
    public void GridSize_SetValue_UpdatesGridSize()
    {
        Size newSize = new Size(10, 10);
        _designer.TestAccessor().Dynamic.GridSize = newSize;
        Size gridSize = _designer.TestAccessor().Dynamic.GridSize;
        gridSize.Should().Be(newSize);
    }

    [Fact]
    public void GridSize_SetValue_InvalidSize_ThrowsArgumentException()
    {
        Action action = () => _designer.TestAccessor().Dynamic.GridSize = new Size(1, 1);
        action.Should().Throw<TargetInvocationException>()
            .WithInnerException<ArgumentException>()
            .WithMessage("*'GridSize'*");

        action = () => _designer.TestAccessor().Dynamic.GridSize = new Size(201, 201);
        action.Should().Throw<TargetInvocationException>()
            .WithInnerException<ArgumentException>()
            .WithMessage("*'GridSize'*");
    }

    [Fact]
    public void MouseDragTool_DefaultValue_ReturnsNull()
    {
        ToolboxItem mouseDragTool = _designer.TestAccessor().Dynamic.MouseDragTool;
        mouseDragTool.Should().BeNull();
    }

    [Fact]
    public void MouseDragTool_SetValue_ReturnsExpected()
    {
        ToolboxItem toolboxItem = new(typeof(Button));
        _designer.TestAccessor().Dynamic._mouseDragTool = toolboxItem;
        ToolboxItem mouseDragTool = _designer.TestAccessor().Dynamic.MouseDragTool;
        mouseDragTool.Should().Be(toolboxItem);
    }

    [Fact]
    public void GetParentForComponent_ReturnsControl()
    {
        Mock<IComponent> mockComponent = new();
        Control parentControl = _designer.TestAccessor().Dynamic.GetParentForComponent(mockComponent.Object);
        parentControl.Should().Be(_control);
    }
}

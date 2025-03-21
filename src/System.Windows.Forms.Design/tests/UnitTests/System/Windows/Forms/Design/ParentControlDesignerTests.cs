// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
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
        _designer.Dispose();
        _control.Dispose();
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
        using Control control = new()
        {
            Site = mockSite.Object
        };
        mockSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(mockDesignerHost.Object);

        Mock<DesignerFrame> mockDesignerFrame = new(mockSite.Object) { CallBase = true };
        BehaviorService behaviorService = new(mockServiceProvider.Object, mockDesignerFrame.Object);
        mockSite.Setup(s => s.GetService(typeof(BehaviorService))).Returns(behaviorService);

        ParentControlDesigner designer = new();
        designer.Initialize(control);
        designer.TestAccessor().Dynamic._behaviorService = behaviorService;

        GlyphSelectionType selectionType = GlyphSelectionType.Selected;

        GlyphCollection glyphs = designer.GetGlyphs(selectionType);

        glyphs.Should().NotBeNull();
        glyphs.OfType<ContainerSelectorGlyph>().Count().Should().Be(1);
    }

    [Fact]
    public void InitializeNewComponent_WithNullDefaultValues_DoesNotThrow()
    {
        ParentControlDesigner designer = new();
        using Control control = new();
        designer.Initialize(control);

        designer.Invoking(d => d.InitializeNewComponent(null)).Should().NotThrow();
    }

    [Fact]
    public void InitializeNewComponent_ShouldReparentControls_WhenAllowControlLassoIsTrue()
    {
        Mock<IDesignerHost> mockDesignerHost = new();
        Mock<TestControl> mockComponent = new();
        Mock<TestControl> mockParentComponent = new();
        Mock<IServiceProvider> mockServiceProvider = new();
        Mock<ISite> mockSite = new();
        Mock<ISelectionService> mockSelectionService = new();

        using TestControl testComponent = new();
        using TestControl parentComponent = new();
        ParentControlDesigner parentControlDesigner = new();

        mockSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(mockDesignerHost.Object);
        mockSite.Setup(s => s.GetService(typeof(ISelectionService))).Returns(mockSelectionService.Object);
        testComponent.Site = mockSite.Object;
        parentComponent.Site = mockSite.Object;

        mockDesignerHost.Setup(h => h.GetDesigner(mockParentComponent.Object)).Returns(parentControlDesigner);

        parentControlDesigner.Initialize(testComponent);

        Dictionary<string, object> defaultValues = new()
        {
            { "Size", new Size(100, 100) },
            { "Location", new Point(10, 10) },
            { "Parent", mockParentComponent.Object }
        };

        DisableDragDrop(testComponent);
        DisableDragDrop(parentComponent);

        parentControlDesigner.InitializeNewComponent(defaultValues);

        mockDesignerHost.Verify(h => h.GetDesigner(mockParentComponent.Object), Times.AtMost(2));
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
}

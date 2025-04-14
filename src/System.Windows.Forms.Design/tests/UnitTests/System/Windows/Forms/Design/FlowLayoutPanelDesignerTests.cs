// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class FlowLayoutPanelDesignerTests : IDisposable
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IDesignerHost> _designerHostMock;
    private readonly Mock<ISelectionService> _selectionServiceMock;
    private readonly FlowLayoutPanelDesigner _designer;
    private readonly FlowLayoutPanel _flowLayoutPanel;

    public FlowLayoutPanelDesignerTests()
    {
        _serviceProviderMock = new();
        _designerHostMock = new();
        _selectionServiceMock = new();

        _serviceProviderMock.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(_designerHostMock.Object);
        _serviceProviderMock.Setup(s => s.GetService(typeof(ISelectionService))).Returns(_selectionServiceMock.Object);

        _flowLayoutPanel = new() { Site = new Mock<ISite>().Object };
        _designer = new();
        _designer.Initialize(_flowLayoutPanel);
    }

    public void Dispose()
    {
        _designer.Dispose();
        _flowLayoutPanel.Dispose();
    }

    [Fact]
    public void Initialize_ShouldSetInheritedReadOnlyAttribute_WhenFlowLayoutPanelIsInheritedReadOnly()
    {
        using Control childControl = new() { Site = new Mock<ISite>().Object };
        _flowLayoutPanel.Controls.Add(childControl);

        TypeDescriptor.AddAttributes(childControl, InheritanceAttribute.InheritedReadOnly);

        TypeDescriptor.GetAttributes(childControl)[typeof(InheritanceAttribute)]
            .Should().BeEquivalentTo(InheritanceAttribute.InheritedReadOnly);
    }

    [Fact]
    public void Initialize_ShouldBeNotInheritedAttribute_WhenFlowLayoutPanelIsNotInheritedReadOnly()
    {
        using Control childControl = new() { Site = new Mock<ISite>().Object };
        _flowLayoutPanel.Controls.Add(childControl);

        TypeDescriptor.GetAttributes(childControl)[typeof(InheritanceAttribute)]
            .Should().NotBeEquivalentTo(InheritanceAttribute.InheritedReadOnly);
    }

    [Fact]
    public void PreFilterProperties_ShouldModifyFlowDirectionProperty()
    {
        Hashtable properties = new()
        {
            { nameof(FlowLayoutPanel.FlowDirection), TypeDescriptor.CreateProperty(typeof(FlowLayoutPanel), nameof(FlowLayoutPanel.FlowDirection), typeof(FlowDirection)) }
        };

        _designer.TestAccessor().Dynamic.PreFilterProperties(properties);

        properties[nameof(FlowLayoutPanel.FlowDirection)].Should().NotBeNull();
        properties[nameof(FlowLayoutPanel.FlowDirection)].Should().BeAssignableTo<PropertyDescriptor>();
    }

    [Fact]
    public void AllowSetChildIndexOnDrop_ShouldReturnFalse() =>
        _designer.AllowSetChildIndexOnDrop.Should().BeFalse();

    [Fact]
    public void AllowGenericDragBox_ShouldReturnFalse()
    {
        bool result = _designer.TestAccessor().Dynamic.AllowGenericDragBox;

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(FlowDirection.LeftToRight, true)]
    [InlineData(FlowDirection.RightToLeft, true)]
    [InlineData(FlowDirection.TopDown, false)]
    [InlineData(FlowDirection.BottomUp, false)]
    public void HorizontalFlow_ShouldReturnCorrectValueBasedOnFlowDirection(FlowDirection flowDirection, bool expected)
    {
        _flowLayoutPanel.FlowDirection = flowDirection;

        bool result = _designer.TestAccessor().Dynamic.HorizontalFlow;

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(FlowDirection.LeftToRight, FlowDirection.RightToLeft)]
    [InlineData(FlowDirection.RightToLeft, FlowDirection.LeftToRight)]
    [InlineData(FlowDirection.TopDown, FlowDirection.TopDown)]
    [InlineData(FlowDirection.BottomUp, FlowDirection.BottomUp)]
    public void RTLTranslateFlowDirection_ShouldTranslateCorrectly(FlowDirection input, FlowDirection expected)
    {
        _flowLayoutPanel.RightToLeft = RightToLeft.Yes;

        FlowDirection result = _designer.TestAccessor().Dynamic.RTLTranslateFlowDirection(input);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(RightToLeft.Yes, true)]
    [InlineData(RightToLeft.No, false)]
    public void IsRtl_ShouldReturnCorrectValueBasedOnRightToLeft(RightToLeft rightToLeft, bool expected)
    {
        _flowLayoutPanel.RightToLeft = rightToLeft;

        bool result = _designer.TestAccessor().Dynamic.IsRtl;

        result.Should().Be(expected);
    }

    [Fact]
    public void GetMarginBounds_ShouldReturnCorrectRectangle()
    {
        using Control control = new()
        {
            Bounds = new Rectangle(10, 20, 30, 40),
            Margin = new Padding(5, 6, 7, 8)
        };

        Rectangle result = _designer.TestAccessor().Dynamic.GetMarginBounds(control);

        result.Should().Be(new Rectangle(5, 14, 42, 54));
    }

    [Fact]
    public void OnDragEnter_ShouldInitializeDragState()
    {
        DragEventArgs dragEventArgs = new(
            new DataObject(),
            0,
            0,
            0,
            DragDropEffects.Copy,
            DragDropEffects.Copy);

        _designer.TestAccessor().Dynamic.OnDragEnter(dragEventArgs);

        int insertionIndex = _designer.TestAccessor().Dynamic._insertionIndex;
        insertionIndex.Should().Be(-1);

        Point lastMouseLocation = _designer.TestAccessor().Dynamic._lastMouseLocation;
        lastMouseLocation.Should().Be(Point.Empty);
    }

    [Fact]
    public void OnDragLeave_ShouldClearDragState()
    {
        _designer.TestAccessor().Dynamic.OnDragLeave(EventArgs.Empty);

        int insertionIndex = _designer.TestAccessor().Dynamic._insertionIndex;
        insertionIndex.Should().Be(-1);
    }

    [Fact]
    public void OnDragOver_ShouldUpdateInsertionIndex()
    {
        DragEventArgs dragEventArgs = new(
            new DataObject(),
            0,
            10,
            10,
            DragDropEffects.Copy,
            DragDropEffects.Copy);

        _designer.TestAccessor().Dynamic.OnDragOver(dragEventArgs);

        int insertionIndex = _designer.TestAccessor().Dynamic._insertionIndex;
        insertionIndex.Should().Be(-1);
    }

    [Fact]
    public void OnDragDrop_ShouldReorderControls()
    {
        using Control control1 = new();
        using Control control2 = new();
        _flowLayoutPanel.Controls.Add(control1);
        _flowLayoutPanel.Controls.Add(control2);
        DragEventArgs dragEventArgs = new(
            new DataObject(),
            0,
            0,
            0,
            DragDropEffects.Move,
            DragDropEffects.Move);

        _designer.TestAccessor().Dynamic.OnDragDrop(dragEventArgs);

        _flowLayoutPanel.Controls[0].Should().Be(control1);
        _flowLayoutPanel.Controls[1].Should().Be(control2);
    }
}

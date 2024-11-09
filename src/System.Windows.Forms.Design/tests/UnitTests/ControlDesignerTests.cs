// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Forms.Design.Behavior;
using System.Windows.Forms.Design.Tests.Mocks;
using Moq;
using Windows.Win32;

namespace System.Windows.Forms.Design.Tests;

public class ControlDesignerTests : IDisposable
{
    private readonly TestControlDesigner _designer = new();
    private readonly Control _control = new();
    private readonly Mock<IDesignerHost> _mockDesignerHost = new();
    private readonly Mock<ISite> _mockSite;

    public ControlDesignerTests()
    {
        Mock<IDesignerHost> _mockDesignerHost = new();
        _mockDesignerHost
            .Setup(h => h.RootComponent)
            .Returns(_control);
        _mockDesignerHost
            .Setup(s => s.GetDesigner(It.IsAny<Control>()))
            .Returns(_designer);
        Mock<IComponentChangeService> mockComponentChangeService = new();
        _mockDesignerHost
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns(mockComponentChangeService.Object);
        _mockSite = CreateMockSiteWithDesignerHost(_mockDesignerHost.Object);
        _control.Site = _mockSite.Object;

        _designer.Initialize(_control);
    }

    public void Dispose()
    {
        _designer.Dispose();
        _control.Dispose();
    }

    [WinFormsFact]
    public void ControlDesigner_Ctor_Default()
    {
        using TestControlDesigner controlDesigner = new();

        controlDesigner.AutoResizeHandles.Should().BeFalse();
        controlDesigner.ControlSupportsSnaplines.Should().BeTrue();
        controlDesigner.ForceVisible.Should().BeTrue();
        controlDesigner.SerializePerformLayout.Should().BeFalse();

        Action action1 = () =>
        {
            using Control _ = controlDesigner.Control;
        };

        Action action2 = () =>
        {
            using IComponent component = controlDesigner.Component;
        };

        Action action3 = () => controlDesigner.GetParentComponentProperty();

        action1.Should().Throw<InvalidOperationException>();
        action2.Should().Throw<InvalidOperationException>();
        action3.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void ControlDesigner_PropertiesTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();

        controlDesigner.Initialize(button);

        controlDesigner.AssociatedComponents.Count.Should().Be(0);
        controlDesigner.IsRootDesigner.Should().BeFalse();
        controlDesigner.SnapLines.Should().NotBeNull();
        controlDesigner.SnapLines.Count.Should().Be(8);
        controlDesigner.StandardBehavior.Should().NotBeNull();
        controlDesigner.StandardBehavior.Cursor.Should().Be(Cursors.Default);
    }

    [Fact]
    public void InitializeControlDefaults()
    {
        _designer.GetAccessibleObjectField().Should().BeNull();
        _designer.GetBehaviorServiceProperty().Should().BeNull();
        _designer.AccessibilityObject.Should().NotBeNull();
        _designer.GetEnableDragRectProperty().Should().BeFalse();
        _designer.ParticipatesWithSnapLines.Should().BeTrue();
        (_designer.AutoResizeHandles = true).Should().BeTrue();
        _designer.AutoResizeHandles.Should().BeTrue();
        _designer.GetInheritanceAttributeProperty().Should().NotBeNull();
        _designer.NumberOfInternalControlDesigners().Should().Be(0);
        _designer.GetHitTestMethod(default).Should().BeFalse();
        _designer.HookChildControlsMethod(new Control());
    }

    [Theory]
    [InlineData(DockStyle.Top, SelectionRules.Moveable | SelectionRules.TopSizeable | SelectionRules.LeftSizeable | SelectionRules.RightSizeable)]
    [InlineData(DockStyle.Left, SelectionRules.Moveable | SelectionRules.TopSizeable | SelectionRules.LeftSizeable | SelectionRules.BottomSizeable)]
    [InlineData(DockStyle.Right, SelectionRules.Moveable | SelectionRules.TopSizeable | SelectionRules.BottomSizeable | SelectionRules.RightSizeable)]
    [InlineData(DockStyle.Bottom, SelectionRules.Moveable | SelectionRules.LeftSizeable | SelectionRules.BottomSizeable | SelectionRules.RightSizeable)]
    [InlineData(DockStyle.Fill, SelectionRules.Moveable | SelectionRules.TopSizeable | SelectionRules.LeftSizeable | SelectionRules.RightSizeable | SelectionRules.BottomSizeable)]
    public void DockStyle_DefinesProperSelectionRules(DockStyle dockStyle, SelectionRules selectionRulesParam)
    {
        SelectionRules defaultSelectionRules = _designer.SelectionRules;
        _control.Dock = dockStyle;
        SelectionRules finalSelectionRules = _designer.SelectionRules;

        using (new NoAssertContext())
        {
            finalSelectionRules &= ~selectionRulesParam;
        }

        _designer.SelectionRules.Should().Be(finalSelectionRules);
    }

    [Fact]
    public void BaseWndProc_Call_DoesNotThrow()
    {
        Action action = () =>
        {
            Message m = default;
            _designer.BaseWndProcMethod(ref m);
        };

        action.Should().NotThrow();
    }

    [Fact]
    public void CanBeParentedTo_WithValidParentControl_ReturnsTrue()
    {
        using ParentControlDesigner parentDesigner = new();
        using Button parentButton = new();
        parentDesigner.Initialize(parentButton);

        _designer.CanBeParentedTo(parentDesigner).Should().BeTrue();
    }

    [Theory]
    [BoolData]
    public void EnableDragDrop_DoesNotThrow(bool val)
    {
        Action action = () => _designer.EnableDragDropMethod(val);
        action.Should().NotThrow();
    }

    [Fact]
    public void Initialize_DoesNotThrow()
    {
        Action action = () =>
        {
            using TestControlDesigner controlDesigner = new();
            using Button button = new();

            controlDesigner.Initialize(button);
        };

        action.Should().NotThrow();
    }

    [Fact]
    public void Uninitialized_ShouldThrowInvalidOperationException()
    {
        Action action = () =>
        {
            using TestControlDesigner _designer = new();
            using Control control = _designer.Control;
        };

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void OnSetComponentDefaultsTest()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        Action action = _designer.OnSetComponentDefaults;
        action.Should().NotThrow();
#pragma warning restore CS0618
    }

    [Fact]
    public void OnContextMenu_DoesNotThrow()
    {
        Action action = () => _designer.OnContextMenuMethod(0, 0);
    }

    [Fact]
    public void OnCreateHandle_DoesNotThrow()
    {
        Action action = _designer.OnCreateHandleMethod;
        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void ControlDesigner_WndProc_InvokePaint_DoesNotThrow()
    {
        Action action = () =>
        {
            Message m = new Message { Msg = (int)PInvokeCore.WM_PAINT };
            _designer.TestAccessor().Dynamic.WndProc(ref m);
        };

        action.Should().NotThrow();
    }

    [Fact]
    public void ControlDesigner_AssociatedComponents_NullSite_ShouldBeEmpty()
    {
        using Control childControl = new();

        _designer.AssociatedComponents.Count.Should().Be(0);

        _control.Controls.Add(childControl);

        _designer.AssociatedComponents.Count.Should().Be(0);
    }

    [WinFormsFact]
    public void ControlDesigner_AssociatedComponentsCount_ShouldBeCorrectAmount()
    {
        var mockSite = MockSite.CreateMockSiteWithDesignerHost(_mockDesignerHost.Object);
        _control.Site = mockSite.Object;

        _designer.AssociatedComponents.Count.Should().Be(0);

        using Control childControl = new();
        childControl.Site = mockSite.Object;
        _control.Controls.Add(childControl);

        _designer.AssociatedComponents.Count.Should().Be(1);
    }

    [Fact]
    public void GetGlyphs_Locked_ReturnsLockedGlyphs()
    {
        Mock<IServiceProvider> mockServiceProvider = new();
        mockServiceProvider.Setup(s => s.GetService(It.IsAny<Type>())).Returns((object?)null);

        Mock<DesignerFrame> mockDesignerFrame = new(_control.Site!) { CallBase = true };
        BehaviorService behaviorService = new(mockServiceProvider.Object, mockDesignerFrame.Object);

        _designer.TestAccessor().Dynamic._behaviorService = behaviorService;
        _designer.TestAccessor().Dynamic.Locked = true;

        GlyphCollection glyphs = _designer.GetGlyphs(GlyphSelectionType.SelectedPrimary);

        glyphs.Count.Should().BeGreaterThan(0);
        glyphs.Should().BeOfType<GlyphCollection>();
        glyphs[0].Should().BeOfType<LockedHandleGlyph>();
    }

    [Fact]
    public void GetGlyphs_GlyphSelectionTypeNotSelected_ReturnsEmptyCollection()
    {
        GlyphCollection glyphs = _designer.GetGlyphs(GlyphSelectionType.NotSelected);

        glyphs.Count.Should().Be(0);
    }

    [Fact]
    public void GetGlyphs_WithNullBehaviorService_ThrowsException()
    {
        _designer.TestAccessor().Dynamic._behaviorService = null;

        Action action = () => _designer.GetGlyphs(GlyphSelectionType.SelectedPrimary);
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetGlyphs_NonSizeableControl_ReturnsNoResizeHandleGlyphs()
    {
        _control.Dock = DockStyle.Fill;
        _control.AutoSize = false;

        Mock<DesignerFrame> mockDesignerFrame = new(_mockSite.Object) { CallBase = true };
        Mock<IServiceProvider> mockServiceProvider = new();
        mockServiceProvider.Setup(s => s.GetService(It.IsAny<Type>())).Returns(mockServiceProvider);
        _mockSite.Setup(s => s.GetService(typeof(IServiceProvider))).Returns(mockServiceProvider.Object);
        BehaviorService behaviorService = new(mockServiceProvider.Object, mockDesignerFrame.Object);

        FieldInfo? behaviorServiceField = typeof(ControlDesigner).GetField("_behaviorService", BindingFlags.NonPublic | BindingFlags.Instance);
        behaviorServiceField?.SetValue(_designer, behaviorService);
        _mockSite.Setup(s => s.GetService(typeof(BehaviorService))).Returns(behaviorService);

        GlyphCollection glyphs = _designer.GetGlyphs(GlyphSelectionType.SelectedPrimary);

        glyphs[0].Should().BeOfType<NoResizeHandleGlyph>();
        ((SelectionRules)glyphs[0].TestAccessor().Dynamic.rules).Should().Be(SelectionRules.None);
        glyphs[1].Should().BeOfType<NoResizeSelectionBorderGlyph>();
        ((SelectionRules)glyphs[1].TestAccessor().Dynamic.rules).Should().Be(SelectionRules.None);
        glyphs[2].Should().BeOfType<NoResizeSelectionBorderGlyph>();
        ((SelectionRules)glyphs[2].TestAccessor().Dynamic.rules).Should().Be(SelectionRules.None);
        glyphs[3].Should().BeOfType<NoResizeSelectionBorderGlyph>();
        ((SelectionRules)glyphs[3].TestAccessor().Dynamic.rules).Should().Be(SelectionRules.None);
        glyphs[4].Should().BeOfType<NoResizeSelectionBorderGlyph>();
        ((SelectionRules)glyphs[4].TestAccessor().Dynamic.rules).Should().Be(SelectionRules.None);
    }

    [Fact]
    public void GetGlyphs_ResizableGlyphs_ReturnsExpected()
    {
        _control.Dock = DockStyle.None;
        _control.AutoSize = false;

        Mock<IServiceProvider> mockServiceProvider = new();
        mockServiceProvider.Setup(s => s.GetService(It.IsAny<Type>())).Returns((object?)null);
        _mockSite.Setup(s => s.GetService(typeof(IServiceProvider))).Returns(mockServiceProvider.Object);

        Mock<DesignerFrame> mockDesignerFrame = new(_mockSite.Object) { CallBase = true };
        BehaviorService behaviorService = new(mockServiceProvider.Object, mockDesignerFrame.Object);

        FieldInfo? behaviorServiceField = typeof(ControlDesigner).GetField("_behaviorService", BindingFlags.NonPublic | BindingFlags.Instance);
        behaviorServiceField?.SetValue(_designer, behaviorService);

        GlyphCollection glyphs = _designer.GetGlyphs(GlyphSelectionType.SelectedPrimary);

        glyphs[0].Should().BeOfType<GrabHandleGlyph>();
        ((SelectionRules)glyphs[0].TestAccessor().Dynamic.rules).Should().Be(SelectionRules.None);
        glyphs[1].Should().BeOfType<GrabHandleGlyph>();
        ((SelectionRules)glyphs[1].TestAccessor().Dynamic.rules).Should().Be(SelectionRules.TopSizeable | SelectionRules.LeftSizeable);
        glyphs[2].Should().BeOfType<GrabHandleGlyph>();
        ((SelectionRules)glyphs[2].TestAccessor().Dynamic.rules).Should().Be(SelectionRules.TopSizeable | SelectionRules.RightSizeable);
        glyphs[3].Should().BeOfType<GrabHandleGlyph>();
        ((SelectionRules)glyphs[3].TestAccessor().Dynamic.rules).Should().Be(SelectionRules.None);
        glyphs[4].Should().BeOfType<GrabHandleGlyph>();
        ((SelectionRules)glyphs[4].TestAccessor().Dynamic.rules).Should().Be(SelectionRules.BottomSizeable | SelectionRules.LeftSizeable);
        glyphs[5].Should().BeOfType<GrabHandleGlyph>();
        ((SelectionRules)glyphs[5].TestAccessor().Dynamic.rules).Should().Be(SelectionRules.BottomSizeable | SelectionRules.RightSizeable);
        glyphs[6].Should().BeOfType<GrabHandleGlyph>();
        ((SelectionRules)glyphs[6].TestAccessor().Dynamic.rules).Should().Be(SelectionRules.None);
        glyphs[7].Should().BeOfType<GrabHandleGlyph>();
        ((SelectionRules)glyphs[7].TestAccessor().Dynamic.rules).Should().Be(SelectionRules.None);
        glyphs[8].Should().BeOfType<SelectionBorderGlyph>();
        ((SelectionRules)glyphs[8].TestAccessor().Dynamic.rules).Should().Be(SelectionRules.TopSizeable);
        glyphs[9].Should().BeOfType<SelectionBorderGlyph>();
        ((SelectionRules)glyphs[9].TestAccessor().Dynamic.rules).Should().Be(SelectionRules.BottomSizeable);
        glyphs[10].Should().BeOfType<SelectionBorderGlyph>();
        ((SelectionRules)glyphs[10].TestAccessor().Dynamic.rules).Should().Be(SelectionRules.LeftSizeable);
        glyphs[11].Should().BeOfType<SelectionBorderGlyph>();
        ((SelectionRules)glyphs[11].TestAccessor().Dynamic.rules).Should().Be(SelectionRules.RightSizeable);
    }

    [Theory]
    [InlineData(DockingBehavior.Never, DockStyle.None)]
    [InlineData(DockingBehavior.AutoDock, DockStyle.Fill)]
    public void InitializeNewComponent_DockingBehavior_DefinesDockStyle(DockingBehavior dockingBehavior, DockStyle dockStyle)
    {
        TypeDescriptor.AddAttributes(_control, new DockingAttribute(dockingBehavior));

        Mock<ParentControlDesigner> mockParentDesigner = new();
        _mockDesignerHost.Setup(h => h.GetDesigner(It.IsAny<IComponent>())).Returns(mockParentDesigner.Object);

        Dictionary<string, object> defaultValues = new()
    {
        { "Parent", new Control() }
    };

        _designer.InitializeNewComponent(defaultValues);

        PropertyDescriptor? dockPropDescriptor = TypeDescriptor.GetProperties(_control)["Dock"];
        dockPropDescriptor.Should().NotBeNull();
        dockPropDescriptor.Should().BeAssignableTo<PropertyDescriptor>();
        dockPropDescriptor?.GetValue(_control).Should().Be(dockStyle);
    }

    [Fact]
    public void InitializeExistingComponent_DockingBehavior_DefinesDockStyle()
    {
        TypeDescriptor.AddAttributes(_control, new DockingAttribute(DockingBehavior.AutoDock));

        Mock<ParentControlDesigner> mockParentDesigner = new();
        _mockDesignerHost.Setup(h => h.GetDesigner(It.IsAny<IComponent>())).Returns(mockParentDesigner.Object);
        Dictionary<string, object> defaultValues = new()
        {
            { "Parent", new Control() }
        };

        Action action = () => _designer.InitializeExistingComponent(defaultValues);

        action.Should().Throw<NotImplementedException>(SR.NotImplementedByDesign);
    }

    public static Mock<ISite> CreateMockSiteWithDesignerHost(object designerHost)
    {
        Mock<ISite> mockSite = new();
        mockSite
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(designerHost);
        mockSite
            .Setup(s => s.GetService(typeof(IInheritanceService)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(IDictionaryService)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(IExtenderListService)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(DesignerActionService)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(ToolStripKeyboardHandlingService)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(ISupportInSituService)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(INestedContainer)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(ToolStripMenuItem)))
            .Returns((object?)null);

        Mock<IServiceProvider> mockServiceProvider = new();

        mockSite
            .Setup(s => s.GetService(typeof(IServiceProvider)))
            .Returns(mockServiceProvider.Object);
        mockSite
            .Setup(s => s.GetService(typeof(ToolStripAdornerWindowService)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(DesignerOptionService)))
            .Returns(mockServiceProvider.Object);

        Mock<ISelectionService> mockSelectionService = new();

        mockSite
            .Setup(s => s.GetService(typeof(ISelectionService)))
            .Returns(mockSelectionService.Object);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer?)null);
        mockSite
            .Setup(s => s.Name)
            .Returns("Site");
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.GetService(typeof(UndoEngine)))
            .Returns((object?)null);

        return mockSite;
    }
}

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
    private readonly ControlDesigner _designer = new();
    private readonly Control _control = new();

    public ControlDesignerTests()
    {
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
        Assert.False(controlDesigner.AutoResizeHandles);
        Assert.Throws<InvalidOperationException>(() => controlDesigner.Control);
        Assert.True(controlDesigner.ControlSupportsSnaplines);
        Assert.Throws<InvalidOperationException>(() => controlDesigner.Component);
        Assert.True(controlDesigner.ForceVisible);
        Assert.Throws<InvalidOperationException>(controlDesigner.GetParentComponentProperty);
        Assert.False(controlDesigner.SerializePerformLayout);
    }

    [WinFormsFact]
    public void ControlDesigner_PropertiesTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.Empty(controlDesigner.AssociatedComponents);
        Assert.False(controlDesigner.IsRootDesigner);
        Assert.NotNull(controlDesigner.SnapLines);
        Assert.Equal(8, controlDesigner.SnapLines.Count);
        Assert.NotNull(controlDesigner.StandardBehavior);
        Assert.Equal(Cursors.Default, controlDesigner.StandardBehavior.Cursor);
    }

    [Fact]
    public void AccessibleObjectField()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.Null(controlDesigner.GetAccessibleObjectField());
    }

    [Fact]
    public void BehaviorServiceProperty()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.Null(controlDesigner.GetBehaviorServiceProperty());
    }

    [Fact]
    public void AccessibilityObjectField()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.NotNull(controlDesigner.AccessibilityObject);
    }

    [Fact]
    public void EnableDragRectProperty()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.False(controlDesigner.GetEnableDragRectProperty());
    }

    [Fact]
    public void ParticipatesWithSnapLinesProperty()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.True(controlDesigner.ParticipatesWithSnapLines);
    }

    [Fact]
    public void AutoResizeHandlesProperty()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.True(controlDesigner.AutoResizeHandles = true);
        Assert.True(controlDesigner.AutoResizeHandles);
    }

    [Fact]
    public void SelectionRulesProperty()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);

        SelectionRules selectionRules;
        using (new NoAssertContext())
        {
            selectionRules = controlDesigner.SelectionRules;
        }

        Assert.Equal(SelectionRules.Visible | SelectionRules.AllSizeable | SelectionRules.Moveable, selectionRules);
    }

    [Fact]
    public void InheritanceAttributeProperty()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.NotNull(controlDesigner.GetInheritanceAttributeProperty());
    }

    [Fact]
    public void NumberOfInternalControlDesignersTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.Equal(0, controlDesigner.NumberOfInternalControlDesigners());
    }

    [Fact]
    public void BaseWndProcTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Message m = default;
        controlDesigner.BaseWndProcMethod(ref m);
    }

    [Fact]
    public void CanBeParentedToTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        using ParentControlDesigner parentDesigner = new();
        using Button parentButton = new();
        parentDesigner.Initialize(parentButton);
        Assert.True(controlDesigner.CanBeParentedTo(parentDesigner));
    }

    [Theory]
    [BoolData]
    public void EnableDragDropTest(bool val)
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        controlDesigner.EnableDragDropMethod(val);
    }

    [Fact]
    public void GetHitTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.False(controlDesigner.GetHitTestMethod(default));
    }

    [Fact]
    public void HookChildControlsTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        controlDesigner.HookChildControlsMethod(new Control());
    }

    [Fact]
    public void InitializeTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
    }

    [Fact]
    public void UninitializedTest()
    {
        using TestControlDesigner controlDesigner = new();
        Assert.Throws<InvalidOperationException>(() => controlDesigner.Control);
    }

    [Fact]
    public void OnSetComponentDefaultsTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
#pragma warning disable CS0618 // Type or member is obsolete
        controlDesigner.OnSetComponentDefaults();
#pragma warning restore CS0618
    }

    [Fact]
    public void OnContextMenuTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        controlDesigner.OnContextMenuMethod(0, 0);
    }

    [Fact]
    public void OnCreateHandleTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        controlDesigner.OnCreateHandleMethod();
    }

    [WinFormsFact]
    public void ControlDesigner_WndProc_InvokePaint_Success()
    {
        using ControlDesigner designer = new();
        using Button button = new();
        designer.Initialize(button);
        Message m = new Message { Msg = (int)PInvokeCore.WM_PAINT };
        designer.TestAccessor().Dynamic.WndProc(ref m);
    }

    [Fact]
    public void ControlDesigner_AssociatedComponents_NullSite_Test()
    {
        using Control childControl = new();

        Assert.Empty(_designer.AssociatedComponents);

        _control.Controls.Add(childControl);

        Assert.Empty(_designer.AssociatedComponents);
    }

    [WinFormsFact]
    public void ControlDesigner_AssociatedComponentsTest()
    {
        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Strict);
        mockDesignerHost
            .Setup(h => h.RootComponent)
            .Returns(_control);
        mockDesignerHost
            .Setup(s => s.GetDesigner(It.IsAny<Control>()))
            .Returns(() => null);
        var mockSite = MockSite.CreateMockSiteWithDesignerHost(mockDesignerHost.Object);
        _control.Site = mockSite.Object;

        Assert.Empty(_designer.AssociatedComponents);

        using Control childControl = new();
        childControl.Site = mockSite.Object;
        _control.Controls.Add(childControl);

        Assert.Single(_designer.AssociatedComponents);
    }

    [Fact]
    public void GetGlyphs_Locked_ReturnsLockedGlyphs()
    {
        Mock<IServiceProvider> mockServiceProvider = new();
        Mock<ISite> mockSite = new();
        mockServiceProvider.Setup(s => s.GetService(It.IsAny<Type>())).Returns(null);
        mockSite.Setup(s => s.GetService(typeof(IServiceProvider))).Returns(mockServiceProvider.Object);

        Mock<DesignerFrame> mockDesignerFrame = new(mockSite.Object) { CallBase = true };
        BehaviorService behaviorService = new(mockServiceProvider.Object, mockDesignerFrame.Object);

        FieldInfo? behaviorServiceField = typeof(ControlDesigner).GetField("_behaviorService", BindingFlags.NonPublic | BindingFlags.Instance);
        behaviorServiceField?.SetValue(_designer, behaviorService);
        _designer.TestAccessor().Dynamic.Locked = true;
        _designer.TestAccessor().Dynamic._host = new Mock<IDesignerHost>().Object;

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
        using Control control = new();
        control.Dock = DockStyle.Fill;
        control.AutoSize = false;
        using ControlDesigner designer = new();
        Mock<IDesignerHost> mockDesignerHost = new();
        mockDesignerHost
            .Setup(h => h.RootComponent)
            .Returns(control);
        mockDesignerHost
            .Setup(s => s.GetDesigner(It.IsAny<Control>()))
            .Returns(designer);
        Mock<IComponentChangeService> mockComponentChangeService = new();
        mockDesignerHost
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns(mockComponentChangeService.Object);

        Mock<ISite> mockSite = CreateMockSiteWithDesignerHost(mockDesignerHost.Object);
        control.Site = mockSite.Object;

        using Component component = new()
        {
            Site = mockSite.Object
        };

        designer.Initialize(control);

        Mock<DesignerFrame> mockDesignerFrame = new(mockSite.Object) { CallBase = true };
        Mock<IServiceProvider> mockServiceProvider = new();
        mockServiceProvider.Setup(s => s.GetService(It.IsAny<Type>())).Returns(mockServiceProvider);
        mockSite.Setup(s => s.GetService(typeof(IServiceProvider))).Returns(mockServiceProvider.Object);
        BehaviorService behaviorService = new(mockServiceProvider.Object, mockDesignerFrame.Object);

        FieldInfo? behaviorServiceField = typeof(ControlDesigner).GetField("_behaviorService", BindingFlags.NonPublic | BindingFlags.Instance);
        behaviorServiceField?.SetValue(designer, behaviorService);
        mockSite.Setup(s => s.GetService(typeof(BehaviorService))).Returns(behaviorService);

        GlyphCollection glyphs = designer.GetGlyphs(GlyphSelectionType.SelectedPrimary);

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
        Mock<ISite> mockSite = new();
        mockServiceProvider.Setup(s => s.GetService(It.IsAny<Type>())).Returns(null);
        mockSite.Setup(s => s.GetService(typeof(IServiceProvider))).Returns(mockServiceProvider.Object);

        Mock<DesignerFrame> mockDesignerFrame = new(mockSite.Object) { CallBase = true };
        BehaviorService behaviorService = new(mockServiceProvider.Object, mockDesignerFrame.Object);

        FieldInfo? behaviorServiceField = typeof(ControlDesigner).GetField("_behaviorService", BindingFlags.NonPublic | BindingFlags.Instance);
        behaviorServiceField?.SetValue(_designer, behaviorService);
        _designer.TestAccessor().Dynamic._host = new Mock<IDesignerHost>().Object;

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
        using Control control = new();
        using ControlDesigner designer = new();

        Mock<IDesignerHost> mockDesignerHost = new();
        mockDesignerHost
            .Setup(h => h.RootComponent)
            .Returns(control);
        mockDesignerHost
            .Setup(s => s.GetDesigner(It.IsAny<Control>()))
            .Returns(designer);
        Mock<IComponentChangeService> mockComponentChangeService = new();
        mockDesignerHost
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns(mockComponentChangeService.Object);

        TypeDescriptor.AddAttributes(control, new DockingAttribute(dockingBehavior));

        using Component component = new()
        {
            Site = MockSite.CreateMockSiteWithDesignerHost(mockDesignerHost.Object).Object
        };
        control.Site = component.Site;

        designer.Initialize(control);

        Mock<ParentControlDesigner> mockParentDesigner = new();
        mockDesignerHost.Setup(h => h.GetDesigner(It.IsAny<IComponent>())).Returns(mockParentDesigner.Object);

        Dictionary<string, object> defaultValues = new()
    {
        { "Parent", new Control() }
    };

        designer.InitializeNewComponent(defaultValues);

        PropertyDescriptor? dockPropDescriptor = TypeDescriptor.GetProperties(control)["Dock"];
        dockPropDescriptor.Should().NotBeNull();
        dockPropDescriptor.Should().BeAssignableTo<PropertyDescriptor>();
        dockPropDescriptor?.GetValue(control).Should().Be(dockStyle);
    }

    [Fact]
    public void InitializeExistingComponent_DockingBehavior_DefinesDockStyle()
    {
        using Control control = new();
        using ControlDesigner designer = new();
        Mock<IDesignerHost> mockDesignerHost = new();
        mockDesignerHost
            .Setup(h => h.RootComponent)
            .Returns(control);
        mockDesignerHost
            .Setup(s => s.GetDesigner(It.IsAny<Control>()))
            .Returns(designer);
        Mock<IComponentChangeService> mockComponentChangeService = new();
        mockDesignerHost
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns(mockComponentChangeService.Object);

        TypeDescriptor.AddAttributes(control, new DockingAttribute(DockingBehavior.AutoDock));

        using Component component = new()
        {
            Site = MockSite.CreateMockSiteWithDesignerHost(mockDesignerHost.Object).Object
        };
        control.Site = component.Site;
        designer.Initialize(control);
        Mock<ParentControlDesigner> mockParentDesigner = new();
        mockDesignerHost.Setup(h => h.GetDesigner(It.IsAny<IComponent>())).Returns(mockParentDesigner.Object);
        Dictionary<string, object> defaultValues = new()
    {
        { "Parent", new Control() }
    };
        Action action = () => designer.InitializeExistingComponent(defaultValues);
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
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(IDictionaryService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(IExtenderListService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(DesignerActionService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(ToolStripKeyboardHandlingService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(ISupportInSituService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(INestedContainer)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(ToolStripMenuItem)))
            .Returns(null);

        Mock<IServiceProvider> mockServiceProvider = new();

        mockSite
            .Setup(s => s.GetService(typeof(IServiceProvider)))
            .Returns(mockServiceProvider.Object);
        mockSite
            .Setup(s => s.GetService(typeof(ToolStripAdornerWindowService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(DesignerOptionService)))
            .Returns(mockServiceProvider.Object);

        Mock<ISelectionService> mockSelectionService = new();

        mockSite
            .Setup(s => s.GetService(typeof(ISelectionService)))
            .Returns(mockSelectionService.Object);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.Name)
            .Returns("Site");
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.GetService(typeof(UndoEngine)))
            .Returns(null);

        return mockSite;
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.Windows.Forms.Design.Behavior;
using System.Windows.Forms.Design.Tests.Mocks;
using Moq;
using Windows.Win32;

namespace System.Windows.Forms.Design.Tests;

public class ControlDesignerTests : IDisposable
{
    private readonly TestControlDesigner _designer = new();

    public void Dispose()
    {
        _designer.Dispose();
    }

    [WinFormsFact]
    public void ControlDesigner_Ctor_Default()
    {
        using TestControlDesigner controlDesigner = new(isInitialized: false);

        controlDesigner.AutoResizeHandles.Should().BeFalse();
        controlDesigner.ControlSupportsSnaplines.Should().BeTrue();
        controlDesigner.ForceVisible.Should().BeTrue();
        controlDesigner.SerializePerformLayout.Should().BeFalse();

        Action action1 = () =>
        {
            Control _ = controlDesigner.Control;
        };

        Action action2 = () =>
        {
            IComponent component = controlDesigner.Component;
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
        _designer.AutoResizeHandles.Should().BeFalse();
        _designer.GetInheritanceAttributeProperty().Should().NotBeNull();
        _designer.NumberOfInternalControlDesigners().Should().Be(0);
        _designer.GetHitTestMethod(default).Should().BeFalse();
        _designer.HookChildControlsMethod(_designer._control);
    }

    [Fact]
    public void AutoResizeHandles_Set_GetReturnsExpected()
    {
        _designer.AutoResizeHandles = true;
        _designer.AutoResizeHandles.Should().BeTrue();
    }

    [Theory]
    [InlineData(DockStyle.Top, SelectionRules.Moveable | SelectionRules.TopSizeable | SelectionRules.LeftSizeable | SelectionRules.RightSizeable)]
    [InlineData(DockStyle.Left, SelectionRules.Moveable | SelectionRules.TopSizeable | SelectionRules.LeftSizeable | SelectionRules.BottomSizeable)]
    [InlineData(DockStyle.Right, SelectionRules.Moveable | SelectionRules.TopSizeable | SelectionRules.BottomSizeable | SelectionRules.RightSizeable)]
    [InlineData(DockStyle.Bottom, SelectionRules.Moveable | SelectionRules.LeftSizeable | SelectionRules.BottomSizeable | SelectionRules.RightSizeable)]
    [InlineData(DockStyle.Fill, SelectionRules.Moveable | SelectionRules.TopSizeable | SelectionRules.LeftSizeable | SelectionRules.RightSizeable | SelectionRules.BottomSizeable)]
    public void DockStyle_DefinesProperSelectionRules(DockStyle dockStyle, SelectionRules selectionRulesParam)
    {
        _designer._control.Dock = dockStyle;
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
            using TestControlDesigner _designer = new(isInitialized: false);
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
        action.Should().NotThrow();
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

        _designer._control.Controls.Add(childControl);

        _designer.AssociatedComponents.Count.Should().Be(0);
    }

    [WinFormsFact]
    public void ControlDesigner_AssociatedComponentsCount_ShouldBeCorrectAmount()
    {
        var mockSite = MockSite.CreateMockSiteWithDesignerHost(_designer._mockDesignerHost.Object);
        _designer._control.Site = mockSite.Object;

        _designer.AssociatedComponents.Count.Should().Be(0);

        using Control childControl = new();
        childControl.Site = mockSite.Object;
        _designer._control.Controls.Add(childControl);

        _designer.AssociatedComponents.Count.Should().Be(1);
    }

    [Fact]
    public void GetGlyphs_Locked_ReturnsLockedGlyphs()
    {
        Mock<IServiceProvider> mockServiceProvider = new();
        mockServiceProvider.Setup(s => s.GetService(It.IsAny<Type>())).Returns((object?)null);

        Mock<DesignerFrame> mockDesignerFrame = new(_designer._control.Site!) { CallBase = true };
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
        _designer._control.Dock = DockStyle.Fill;
        _designer._control.AutoSize = false;

        Mock<DesignerFrame> mockDesignerFrame = new(_designer._mockSite.Object) { CallBase = true };
        Mock<IServiceProvider> mockServiceProvider = new();
        BehaviorService behaviorService = new(mockServiceProvider.Object, mockDesignerFrame.Object);
        _designer._mockSite.Setup(s => s.GetService(typeof(BehaviorService))).Returns(behaviorService);

        _designer.TestAccessor().Dynamic._behaviorService = behaviorService;

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
        _designer._control.Dock = DockStyle.None;
        _designer._control.AutoSize = false;

        Mock<IServiceProvider> mockServiceProvider = new();
        mockServiceProvider.Setup(s => s.GetService(It.IsAny<Type>())).Returns((object?)null);
        _designer._mockSite.Setup(s => s.GetService(typeof(IServiceProvider))).Returns(mockServiceProvider.Object);

        Mock<DesignerFrame> mockDesignerFrame = new(_designer._mockSite.Object) { CallBase = true };
        BehaviorService behaviorService = new(mockServiceProvider.Object, mockDesignerFrame.Object);

        _designer.TestAccessor().Dynamic._behaviorService = behaviorService;

        GlyphCollection glyphs = _designer.GetGlyphs(GlyphSelectionType.SelectedPrimary);

        var expectedGlyphs = new (Type glyphType, SelectionRules rules)[]
        {
            (typeof(GrabHandleGlyph), SelectionRules.None),
            (typeof(GrabHandleGlyph), SelectionRules.TopSizeable | SelectionRules.LeftSizeable),
            (typeof(GrabHandleGlyph), SelectionRules.TopSizeable | SelectionRules.RightSizeable),
            (typeof(GrabHandleGlyph), SelectionRules.None),
            (typeof(GrabHandleGlyph), SelectionRules.BottomSizeable | SelectionRules.LeftSizeable),
            (typeof(GrabHandleGlyph), SelectionRules.BottomSizeable | SelectionRules.RightSizeable),
            (typeof(GrabHandleGlyph), SelectionRules.None),
            (typeof(GrabHandleGlyph), SelectionRules.None),
            (typeof(SelectionBorderGlyph), SelectionRules.TopSizeable),
            (typeof(SelectionBorderGlyph), SelectionRules.BottomSizeable),
            (typeof(SelectionBorderGlyph), SelectionRules.LeftSizeable),
            (typeof(SelectionBorderGlyph), SelectionRules.RightSizeable),
        };

        for (int i = 0; i < expectedGlyphs.Length; i++)
        {
            glyphs[i].Should().BeOfType(expectedGlyphs[i].glyphType);
            ((SelectionRules)glyphs[i].TestAccessor().Dynamic.rules).Should().Be(expectedGlyphs[i].rules);
        }
    }

    [Theory]
    [InlineData(DockingBehavior.Never, DockStyle.None)]
    [InlineData(DockingBehavior.AutoDock, DockStyle.Fill)]
    public void InitializeNewComponent_DockingBehavior_DefinesDockStyle(DockingBehavior dockingBehavior, DockStyle dockStyle)
    {
        TypeDescriptor.AddAttributes(_designer._control, new DockingAttribute(dockingBehavior));

        Mock<ParentControlDesigner> mockParentDesigner = new();
        _designer._mockDesignerHost.Setup(h => h.GetDesigner(It.IsAny<IComponent>())).Returns(mockParentDesigner.Object);

        Dictionary<string, object> defaultValues = new()
        {
            { "Parent", new Control() }
        };

        _designer.InitializeNewComponent(defaultValues);

        PropertyDescriptor? dockPropDescriptor = TypeDescriptor.GetProperties(_designer._control)[nameof(Control.Dock)];
        dockPropDescriptor.Should().NotBeNull();
        dockPropDescriptor.Should().BeAssignableTo<PropertyDescriptor>();
        dockPropDescriptor?.GetValue(_designer._control).Should().Be(dockStyle);
    }

    [Fact]
    public void InitializeExistingComponent_DockingBehavior_DefinesDockStyle()
    {
        TypeDescriptor.AddAttributes(_designer._control, new DockingAttribute(DockingBehavior.AutoDock));

        Mock<ParentControlDesigner> mockParentDesigner = new();
        _designer._mockDesignerHost.Setup(h => h.GetDesigner(It.IsAny<IComponent>())).Returns(mockParentDesigner.Object);
        Dictionary<string, object> defaultValues = new()
        {
            { nameof(mockParentDesigner.Object.Component), new Control() }
        };

        Action action = () => _designer.InitializeExistingComponent(defaultValues);

        action.Should().Throw<NotImplementedException>(SR.NotImplementedByDesign);
    }

    [Fact]
    public void WndProc_CallsOnMouseDragEnd_WhenLeftMouseButtonReleased()
    {
        var msg = Message.Create(_designer._control.Handle, 0x0202, IntPtr.Zero, IntPtr.Zero);

        _designer.WndProc(ref msg);

        _designer.OnMouseDragEndCalled.Should().BeTrue();

        bool _ctrlSelect = (bool)_designer.TestAccessor().Dynamic._ctrlSelect;
        _ctrlSelect.Should().BeFalse();
    }
}

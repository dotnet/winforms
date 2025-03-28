// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms.Design.Behavior;
using Microsoft.CodeAnalysis;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public sealed class ToolStripEditorManagerTests : IDisposable
{
    private readonly BehaviorService _behaviorService;
    private readonly Mock<IDesignerHost> _mockDesignerHost;
    private readonly Mock<IComponent> _mockComponent;
    private readonly Mock<ISite> _mockSite;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly ToolStripEditorManager _editorManager;
    private readonly Control _editorControl;
    private readonly Rectangle _bounds;
    private readonly object _toolStripEditorControl;
    private readonly Type _toolStripEditorControlType;
    private readonly ConstructorInfo _constructor;
    private readonly ToolStripItem _toolStripItem;
    private readonly Mock<DesignerFrame> _mockDesignerFrame;

    public ToolStripEditorManagerTests()
    {
        _mockServiceProvider = new();
        _mockSite = new();
        _mockDesignerHost = new();
        _mockComponent = new();
        _mockDesignerFrame = new(_mockSite.Object) { CallBase = true };
        _behaviorService = new(_mockServiceProvider.Object, _mockDesignerFrame.Object);
        _toolStripItem = new ToolStripButton("Sample Button");

        _mockComponent.Setup(c => c.Site).Returns(_mockSite.Object);
        _mockSite.Setup(s => s.GetService(typeof(BehaviorService))).Returns(_behaviorService);
        _mockSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(_mockDesignerHost.Object);
        _editorManager = new(_mockComponent.Object);

        _editorControl = new();
        _bounds = new(10, 20, 100, 200);

        _toolStripEditorControlType = typeof(ToolStripEditorManager).GetNestedType("ToolStripEditorControl", BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("ToolStripEditorControl type not found.");

        _constructor = _toolStripEditorControlType?.GetConstructor([typeof(Control), typeof(Rectangle)])
            ?? throw new InvalidOperationException("Constructor for ToolStripEditorControl not found.");

        _toolStripEditorControl = _constructor.Invoke([_editorControl, _bounds]);
    }

    public void Dispose()
    {
        _behaviorService.Dispose();
        _editorControl.Dispose();
        _toolStripItem.Dispose();
    }

    [Fact]
    public void Constructor_InitializesBehaviorServiceAndDesignerHost()
    {
        _editorManager.Should().BeOfType<ToolStripEditorManager>();

        BehaviorService? behaviorService = _editorManager.TestAccessor().Dynamic._behaviorService;
        behaviorService.Should().Be(_behaviorService);

        IDesignerHost? designerHost = _editorManager.TestAccessor().Dynamic._designerHost;
        designerHost.Should().Be(_mockDesignerHost.Object);
    }

    [Fact]
    public void ActivateEditor_ShouldNotAddNewEditor_WhenItemIsNull()
    {
        Action act = () => _editorManager.ActivateEditor(null);
        act.Should().NotThrow();
    }

    [Fact]
    public void ActivateEditor_ShouldReturn_WhenItemIsSameAsCurrentItem()
    {
        _editorManager.TestAccessor().Dynamic._behaviorService = _behaviorService;
        _editorManager.TestAccessor().Dynamic._currentItem = _toolStripItem;

        Action act = () => _editorManager.ActivateEditor(_toolStripItem);
        act.Should().NotThrow();

        ToolStripItem currentItem = _editorManager.TestAccessor().Dynamic._currentItem;
        currentItem.Should().Be(_toolStripItem);
    }

    [Fact]
    public void ActivateEditor_ShouldDeactivateCurrentEditor_WhenEditorIsNotNull()
    {
        _editorManager.TestAccessor().Dynamic._behaviorService = _behaviorService;
        _editorManager.TestAccessor().Dynamic._editor = _toolStripEditorControl;
        _editorManager.TestAccessor().Dynamic._itemDesigner = new Mock<ToolStripItemDesigner>().Object;
        _editorManager.TestAccessor().Dynamic._currentItem = new ToolStripButton();

        _editorManager.ActivateEditor(null);

        _behaviorService.AdornerWindowControl.Controls.Cast<Control>().Should().NotContain((Control)_toolStripEditorControl);

        ToolStripTemplateNode editorUI = _editorManager.TestAccessor().Dynamic._editorUI;
        editorUI.Should().BeNull();

        object? editor = _editorManager.TestAccessor().Dynamic._editor;
        editor.Should().BeNull();

        ToolStripItem currentItem = _editorManager.TestAccessor().Dynamic._currentItem;
        currentItem.Should().BeNull();

        bool? isEditorActive = _editorManager.TestAccessor().Dynamic._itemDesigner.IsEditorActive;
        isEditorActive.Should().BeFalse();
    }

    [Fact]
    public void ActivateEditor_ShouldAddNewEditor_WhenItemIsNotNull()
    {
        Mock<ToolStrip> mockToolStrip = new();
        Mock<IComponent> mockComponent = mockToolStrip.As<IComponent>();
        Mock<ISite> mockSite = new();
        Mock<IDesignerHost> mockDesignerHost = new();
        Mock<DesignSurface> mockDesignSurface = new();
        mockComponent.Setup(c => c.Site).Returns(mockSite.Object);
        mockSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(mockDesignerHost.Object);
        mockSite.Setup(s => s.GetService(typeof(DesignSurface))).Returns(mockDesignSurface.Object);
        Mock<Control> mockRootComponent = new();
        mockDesignerHost.Setup(dh => dh.RootComponent).Returns(mockRootComponent.Object);

        Mock<ToolStripItemDesigner> mockToolStripItemDesigner = new();
        Mock<ToolStripTemplateNode> mockToolStripTemplateNode = new(MockBehavior.Default, mockComponent.Object, "SampleText");
        FieldInfo? centerLabelField = typeof(ToolStripTemplateNode).GetField("_centerLabel", BindingFlags.NonPublic | BindingFlags.Instance);
        centerLabelField?.SetValue(mockToolStripTemplateNode.Object, new ToolStripLabel("Test"));
        mockToolStripItemDesigner.Setup(d => d.Editor).Returns(mockToolStripTemplateNode.Object);
        _mockDesignerHost.Setup(dh => dh.GetDesigner(It.IsAny<ToolStripItem>())).Returns(mockToolStripItemDesigner.Object);

        _editorManager.ActivateEditor(_toolStripItem);

        ToolStripItem currentItem = _editorManager.TestAccessor().Dynamic._currentItem;
        currentItem.Should().Be(_toolStripItem);

        ToolStripTemplateNode editorUI = _editorManager.TestAccessor().Dynamic._editorUI;
        editorUI.Should().Be(mockToolStripTemplateNode.Object);

        ToolStripItemDesigner itemDesigner = _editorManager.TestAccessor().Dynamic._itemDesigner;
        itemDesigner.Should().Be(mockToolStripItemDesigner.Object);

        _mockDesignerHost.Verify(dh => dh.GetDesigner(_toolStripItem), Times.Once);
        mockToolStripItemDesigner.Object.IsEditorActive.Should().BeTrue();
    }

    [Fact]
    public void CloseManager_ShouldNotThrowException()
    {
        Action act = ToolStripEditorManager.CloseManager;
        act.Should().NotThrow();
    }

    [Fact]
    public void OnEditorResize_ShouldInvalidateAndUpdateBounds()
    {
        _editorManager.TestAccessor().Dynamic._editor = _toolStripEditorControl!;
        _editorManager.TestAccessor().Dynamic.OnEditorResize(_editorManager, EventArgs.Empty);

        Rectangle _editorManagerBounds = _editorManager.TestAccessor().Dynamic._lastKnownEditorBounds;
        _editorManagerBounds.X.Should().Be(_bounds.X);
        _editorManagerBounds.Y.Should().Be(_bounds.Y);
    }

    [Fact]
    public void ToolStripEditorControl_Constructor_InitializesProperties()
    {
        VerifyProperty("Bounds1", _toolStripEditorControl, _bounds);
        VerifyProperty(nameof(Location), _toolStripEditorControl, new Point(_bounds.X, _bounds.Y));
        VerifyProperty(nameof(Text), _toolStripEditorControl, "InSituEditorWrapper");
        VerifyProperty(nameof(Size), _toolStripEditorControl, new Size(_editorControl.Size.Width, _editorControl.Size.Height));
        _toolStripEditorControlType?.GetProperty("Controls")?.GetValue(_toolStripEditorControl).Should().BeOfType<Control.ControlCollection>();
    }

    private void VerifyProperty<T>(string propertyName, object targetObject, T expectedValue)
    {
        var propertyValue = _toolStripEditorControlType?.GetProperty(propertyName)?.GetValue(targetObject);
        propertyValue.Should().Be(expectedValue);
    }

    [Fact]
    public void ToolStripEditorControl_Bounds1_Setter_UpdatesBounds()
    {
        Rectangle newBounds = new(30, 40, 150, 250);
        PropertyInfo? boundsProperty = _toolStripEditorControlType?.GetProperty("Bounds1");

        if (boundsProperty is not null)
        {
            boundsProperty.SetValue(_toolStripEditorControl, newBounds);
            Rectangle? actualBounds = boundsProperty.GetValue(_toolStripEditorControl) as Rectangle?;
            actualBounds.Should().Be(newBounds);
        }
    }
}

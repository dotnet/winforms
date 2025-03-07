// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms.Design.Behavior;
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

    public ToolStripEditorManagerTests()
    {
        _mockServiceProvider = new();
        _mockSite = new();
        _mockDesignerHost = new();
        _mockComponent = new();
        _behaviorService = new(_mockServiceProvider.Object, new(_mockSite.Object));

        _mockComponent.Setup(c => c.Site).Returns(_mockSite.Object);
        _mockSite.Setup(s => s.GetService(typeof(BehaviorService))).Returns(_behaviorService);
        _mockSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(_mockDesignerHost.Object);
        _editorManager = new(_mockComponent.Object);

        _editorControl = new();
        _bounds = new(10, 20, 100, 200);
        _toolStripEditorControlType = typeof(ToolStripEditorManager).GetNestedType("ToolStripEditorControl", BindingFlags.NonPublic);
        _constructor = _toolStripEditorControlType.GetConstructor([typeof(Control), typeof(Rectangle)]);
        _toolStripEditorControl = _constructor.Invoke([_editorControl, _bounds]);
    }

    public void Dispose()
    {
        _behaviorService.Dispose();
        _editorControl.Dispose();
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

        PropertyInfo boundsProperty = _toolStripEditorControlType.GetProperty("Bounds1");
        Rectangle actualBounds = (Rectangle)boundsProperty.GetValue(_toolStripEditorControl);
        actualBounds.Should().Be(_bounds);
    }

    [Fact]
    public void ToolStripEditorControl_Constructor_InitializesProperties()
    {
        PropertyInfo boundsProperty = _toolStripEditorControlType.GetProperty("Bounds1");
        PropertyInfo locationProperty = _toolStripEditorControlType.GetProperty("Location");
        PropertyInfo textProperty = _toolStripEditorControlType.GetProperty("Text");
        PropertyInfo controlsProperty = _toolStripEditorControlType.GetProperty("Controls");
        PropertyInfo sizeProperty = _toolStripEditorControlType.GetProperty("Size");

        boundsProperty.GetValue(_toolStripEditorControl).Should().Be(_bounds);
        locationProperty.GetValue(_toolStripEditorControl).Should().Be(new Point(_bounds.X, _bounds.Y));
        textProperty.GetValue(_toolStripEditorControl).Should().Be("InSituEditorWrapper");
        controlsProperty.GetValue(_toolStripEditorControl).Should().BeOfType<Control.ControlCollection>();
        sizeProperty.GetValue(_toolStripEditorControl).Should().Be(new Size(_editorControl.Size.Width, _editorControl.Size.Height));
    }

    [Fact]
    public void ToolStripEditorControl_Bounds1_Setter_UpdatesBounds()
    {
        Rectangle newBounds = new(30, 40, 150, 250);
        _toolStripEditorControlType.GetProperty("Bounds1").SetValue(_toolStripEditorControl, newBounds);

        Rectangle actualBounds = (Rectangle)_toolStripEditorControlType.GetProperty("Bounds1").GetValue(_toolStripEditorControl);
        actualBounds.Should().Be(newBounds);
    }

    [Fact]
    public void ToolStripEditorControl_OnWrappedEditorResize_DoesNotThrow()
    {
        EventArgs eventArgs = EventArgs.Empty;
        Action act = () => _toolStripEditorControl.GetType().GetMethod("OnWrappedEditorResize", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(_toolStripEditorControl, [_toolStripEditorControl, eventArgs]);

        act.Should().NotThrow();
    }
}

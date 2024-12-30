// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Design.Tests;

public class ControlDesignerTransparentBehaviorTests : IDisposable
{
    private readonly TestControlDesigner _designer;
    private readonly TestControl _control;

    public ControlDesignerTransparentBehaviorTests()
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

    private class TestControlDesigner : ControlDesigner
    {
        public new void OnDragDrop(DragEventArgs e) => base.OnDragDrop(e);
        public new void OnDragEnter(DragEventArgs e) => base.OnDragEnter(e);
        public new void OnDragLeave(EventArgs e) => base.OnDragLeave(e);
        public new void OnDragOver(DragEventArgs e) => base.OnDragOver(e);
        public new void OnGiveFeedback(GiveFeedbackEventArgs e) => base.OnGiveFeedback(e);
    }

    private class TestControl : Control { }

    private Rectangle GetControlRect(ControlDesigner.TransparentBehavior behavior)
    {
        dynamic accessor = behavior.TestAccessor().Dynamic;
        return accessor._controlRect;
    }

    [Fact]
    public void TransparentBehavior_OnDragDrop_CallsDesignerOnDragDrop()
    {
        ControlDesigner.TransparentBehavior behavior = new(_designer);
        DragEventArgs dragEventArgs = new(null, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.None);

        behavior.OnDragDrop(null, dragEventArgs);

        Rectangle controlRect = GetControlRect(behavior);
        controlRect.Should().Be(Rectangle.Empty);
    }

    [Fact]
    public void TransparentBehavior_OnDragEnter_SetsControlRect()
    {
        ControlDesigner.TransparentBehavior behavior = new(_designer);
        DragEventArgs dragEventArgs = new(null, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.None);

        behavior.OnDragEnter(null, dragEventArgs);

        Rectangle controlRect = GetControlRect(behavior);
        controlRect.Should().NotBe(Rectangle.Empty);
        controlRect.Should().Be(_control.RectangleToScreen(_control.ClientRectangle));
    }

    [Fact]
    public void TransparentBehavior_OnDragLeave_ClearsControlRect()
    {
        ControlDesigner.TransparentBehavior behavior = new(_designer);
        behavior.OnDragEnter(null, new DragEventArgs(null, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.None));

        behavior.OnDragLeave(null, EventArgs.Empty);

        Rectangle controlRect = GetControlRect(behavior);
        controlRect.Should().Be(Rectangle.Empty);
    }

    [Fact]
    public void TransparentBehavior_OnDragOver_SetsEffectToNoneIfOutsideControlRect()
    {
        ControlDesigner.TransparentBehavior behavior = new(_designer);
        behavior.OnDragEnter(null, new DragEventArgs(null, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.None));
        DragEventArgs dragEventArgs = new(null, 0, int.MaxValue, int.MaxValue, DragDropEffects.Copy, DragDropEffects.Copy);

        behavior.OnDragOver(null, dragEventArgs);

        dragEventArgs.Effect.Should().Be(DragDropEffects.None);
    }

    [Fact]
    public void TransparentBehavior_OnGiveFeedback_CallsDesignerOnGiveFeedback()
    {
        ControlDesigner.TransparentBehavior behavior = new(_designer);
        GiveFeedbackEventArgs feedbackEventArgs = new(DragDropEffects.Copy, true);

        Exception? exception = Record.Exception(() => behavior.OnGiveFeedback(null, feedbackEventArgs));
        exception.Should().BeNull();
    }
}

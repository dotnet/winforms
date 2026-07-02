// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;

namespace System.Windows.Forms.Tests;

public partial class ControlTests
{
    [WinFormsFact]
    public void Control_VisualStylesMode_DefaultMatchesApplicationDefault()
    {
        using SubControlWithVisualStyles control = new();

        // With no parent and no explicit value, the control inherits the application-wide default.
        Assert.Equal(Application.DefaultVisualStylesMode, control.VisualStylesMode);
        Assert.Equal(Application.DefaultVisualStylesMode, control.DefaultVisualStylesModeAccessor);
        Assert.False(control.IsHandleCreated);
    }

    [Fact]
    public void VisualStylesMode_LatestPreview_HasExpectedValue()
    {
        Assert.Equal(short.MaxValue - 1, (short)VisualStylesMode.LatestPreview);
    }

    [WinFormsTheory]
    [InlineData(VisualStylesMode.Net11)]
    [InlineData(VisualStylesMode.LatestPreview)]
    [InlineData(VisualStylesMode.Latest)]
    public void Control_VisualStylesMode_Set_GetReturnsExpected(VisualStylesMode value)
    {
        using SubControlWithVisualStyles control = new() { VisualStylesMode = value };
        Assert.Equal(value, control.VisualStylesMode);
        Assert.False(control.IsHandleCreated);

        // Set the same value again - idempotent, no handle forced.
        control.VisualStylesMode = value;
        Assert.Equal(value, control.VisualStylesMode);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((VisualStylesMode)(-2))]
    [InlineData((VisualStylesMode)3)]
    [InlineData((VisualStylesMode)999)]
    public void Control_VisualStylesMode_SetInvalid_ThrowsInvalidEnumArgumentException(VisualStylesMode value)
    {
        using SubControlWithVisualStyles control = new();
        Assert.Throws<InvalidEnumArgumentException>(() => control.VisualStylesMode = value);
    }

    [WinFormsFact]
    public void Control_VisualStylesMode_SetInherit_ReinheritsFromParent()
    {
        using SubControlWithVisualStyles parent = new() { VisualStylesMode = VisualStylesMode.Net11 };
        using SubControlWithVisualStyles child = new();
        parent.Controls.Add(child);

        child.VisualStylesMode = VisualStylesMode.Disabled;
        Assert.Equal(VisualStylesMode.Disabled, child.VisualStylesMode);

        // Setting Inherit clears the local override so the child inherits from the parent again.
        child.VisualStylesMode = VisualStylesMode.Inherit;
        Assert.Equal(VisualStylesMode.Net11, child.VisualStylesMode);

        // The child is ambient again, so a later change on the parent flows through.
        parent.VisualStylesMode = VisualStylesMode.Classic;
        Assert.Equal(VisualStylesMode.Classic, child.VisualStylesMode);
    }

    [WinFormsFact]
    public void Control_VisualStylesMode_GetNeverReturnsInherit()
    {
        using SubControlWithVisualStyles control = new();

        // Inherit is an assignable sentinel, but the resolved value is never Inherit.
        control.VisualStylesMode = VisualStylesMode.Inherit;
        Assert.NotEqual(VisualStylesMode.Inherit, control.VisualStylesMode);
        Assert.Equal(Application.DefaultVisualStylesMode, control.VisualStylesMode);
    }

    [Fact]
    public void Application_SetDefaultVisualStylesMode_Inherit_ThrowsArgumentException()
    {
        // Inherit is the ambient sentinel and is rejected before any global state is mutated, so this
        // is safe to assert regardless of whether the default has already been set.
        Assert.Throws<ArgumentException>(() => Application.SetDefaultVisualStylesMode(VisualStylesMode.Inherit));
    }

    [WinFormsFact]
    public void Control_VisualStylesMode_SetWithHandler_CallsVisualStylesModeChanged()
    {
        using SubControlWithVisualStyles control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        control.VisualStylesModeChanged += handler;

        // Set a different value - the event fires.
        control.VisualStylesMode = VisualStylesMode.Net11;
        Assert.Equal(VisualStylesMode.Net11, control.VisualStylesMode);
        Assert.Equal(1, callCount);

        // Set the same value - the event does not fire.
        control.VisualStylesMode = VisualStylesMode.Net11;
        Assert.Equal(1, callCount);

        // Set another different value - the event fires again.
        control.VisualStylesMode = VisualStylesMode.Disabled;
        Assert.Equal(2, callCount);

        // Remove the handler - the event no longer reaches it.
        control.VisualStylesModeChanged -= handler;
        control.VisualStylesMode = VisualStylesMode.Net11;
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void Control_OnVisualStylesModeChanged_Invoke_CallsVisualStylesModeChanged(EventArgs eventArgs)
    {
        using SubControlWithVisualStyles control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with the handler subscribed.
        control.VisualStylesModeChanged += handler;
        control.OnVisualStylesModeChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove the handler - still callable, but the handler is not invoked.
        control.VisualStylesModeChanged -= handler;
        control.OnVisualStylesModeChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void Control_VisualStylesMode_AmbientBehaviorTest()
    {
        using SubControlWithVisualStyles parent = new();
        using SubControlWithVisualStyles child = new();
        parent.Controls.Add(child);

        // Setting the parent propagates to the child, which has no explicit value.
        parent.VisualStylesMode = VisualStylesMode.Net11;
        Assert.Equal(VisualStylesMode.Net11, parent.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Net11, child.VisualStylesMode);

        // The child can override the inherited value.
        child.VisualStylesMode = VisualStylesMode.Disabled;
        Assert.Equal(VisualStylesMode.Disabled, child.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Net11, parent.VisualStylesMode);

        // Setting the child back to the parent's value makes it ambient again, so a later
        // change on the parent flows through to the child once more.
        child.VisualStylesMode = VisualStylesMode.Net11;
        Assert.Equal(VisualStylesMode.Net11, child.VisualStylesMode);

        parent.VisualStylesMode = VisualStylesMode.Classic;
        Assert.Equal(VisualStylesMode.Classic, parent.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Classic, child.VisualStylesMode);
    }

    [WinFormsFact]
    public void Appearance_ToggleSwitch_HasExpectedValue()
    {
        Assert.Equal(2, (int)Appearance.ToggleSwitch);
    }

    private class SubControlWithVisualStyles : Control
    {
        public VisualStylesMode DefaultVisualStylesModeAccessor => base.DefaultVisualStylesMode;

        public new void OnVisualStylesModeChanged(EventArgs e) => base.OnVisualStylesModeChanged(e);

        public new void OnParentVisualStylesModeChanged(EventArgs e) => base.OnParentVisualStylesModeChanged(e);
    }
}

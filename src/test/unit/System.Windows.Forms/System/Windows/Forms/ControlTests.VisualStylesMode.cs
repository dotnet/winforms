// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;

namespace System.Windows.Forms.Tests;

public partial class ControlTests
{
    [WinFormsFact]
    public void Control_VisualStylesMode_DefaultIsInherit()
    {
        using SubControlWithVisualStyles control = new();
        PropertyDescriptor property = TypeDescriptor.GetProperties(control)[
            nameof(Control.VisualStylesMode)];
        AmbientValueAttribute ambientValue = Assert.IsType<AmbientValueAttribute>(
            property.Attributes[typeof(AmbientValueAttribute)]);

        Assert.Equal(VisualStylesMode.Inherit, control.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Inherit, property.GetValue(control));
        Assert.Equal(VisualStylesMode.Inherit, ambientValue.Value);
        Assert.Equal(Application.DefaultVisualStylesMode, control.DefaultVisualStylesModeAccessor);
        Assert.Equal(Application.DefaultVisualStylesMode, control.EffectiveVisualStylesModeAccessor);
        Assert.False(property.ShouldSerializeValue(control));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(VisualStylesMode.Net11)]
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
        Assert.Equal(VisualStylesMode.Inherit, child.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Net11, child.EffectiveVisualStylesModeAccessor);

        // The child is ambient again, so a later change on the parent flows through.
        parent.VisualStylesMode = VisualStylesMode.Classic;
        Assert.Equal(VisualStylesMode.Inherit, child.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Classic, child.EffectiveVisualStylesModeAccessor);
    }

    [WinFormsFact]
    public void Control_VisualStylesMode_InheritReportsRequestedValue()
    {
        using SubControlWithVisualStyles control = new();

        control.VisualStylesMode = VisualStylesMode.Inherit;

        Assert.Equal(VisualStylesMode.Inherit, control.VisualStylesMode);
        Assert.Equal(Application.DefaultVisualStylesMode, control.EffectiveVisualStylesModeAccessor);
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
    public void Control_OnVisualStylesModeChanged_InvalidatesBeforeRaisingEvent()
    {
        using SubControlWithVisualStyles control = new();
        control.CreateControl();
        bool invalidated = false;
        control.Invalidated += (sender, e) => invalidated = true;
        control.VisualStylesModeChanged += (sender, e) => Assert.True(invalidated);

        control.OnVisualStylesModeChanged(EventArgs.Empty);

        Assert.True(invalidated);
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
        Assert.Equal(VisualStylesMode.Inherit, child.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Net11, child.EffectiveVisualStylesModeAccessor);

        // The child can override the inherited value.
        child.VisualStylesMode = VisualStylesMode.Disabled;
        Assert.Equal(VisualStylesMode.Disabled, child.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Net11, parent.VisualStylesMode);

        // Setting the child back to the parent's value makes it ambient again, so a later
        // change on the parent flows through to the child once more.
        child.VisualStylesMode = VisualStylesMode.Net11;
        Assert.Equal(VisualStylesMode.Inherit, child.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Net11, child.EffectiveVisualStylesModeAccessor);

        parent.VisualStylesMode = VisualStylesMode.Classic;
        Assert.Equal(VisualStylesMode.Classic, parent.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Inherit, child.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Classic, child.EffectiveVisualStylesModeAccessor);
    }

    [WinFormsFact]
    public void Control_VisualStylesMode_ReparentingToDifferentMode_RaisesChanged()
    {
        using SubControlWithVisualStyles parent = new()
        {
            VisualStylesMode = VisualStylesMode.Net11
        };
        using SubControlWithVisualStyles child = new();
        int callCount = 0;
        child.VisualStylesModeChanged += (sender, e) => callCount++;

        child.AssignParent(parent);

        Assert.Equal(VisualStylesMode.Inherit, child.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Net11, child.EffectiveVisualStylesModeAccessor);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void Control_VisualStylesMode_ResetValue_ReinheritsAndRaisesChanged()
    {
        using SubControlWithVisualStyles parent = new()
        {
            VisualStylesMode = VisualStylesMode.Net11
        };
        using SubControlWithVisualStyles child = new()
        {
            VisualStylesMode = VisualStylesMode.Disabled
        };
        parent.Controls.Add(child);
        int callCount = 0;
        child.VisualStylesModeChanged += (sender, e) => callCount++;
        PropertyDescriptor property = TypeDescriptor.GetProperties(child)[nameof(Control.VisualStylesMode)];

        property.ResetValue(child);

        Assert.Equal(VisualStylesMode.Inherit, child.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Net11, child.EffectiveVisualStylesModeAccessor);
        Assert.False(property.ShouldSerializeValue(child));
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void Control_VisualStylesMode_ShouldSerializeValue_TracksRawOverride()
    {
        using SubControlWithVisualStyles parent = new()
        {
            VisualStylesMode = VisualStylesMode.Net11
        };
        using SubControlWithVisualStyles child = new();
        parent.Controls.Add(child);
        PropertyDescriptor property = TypeDescriptor.GetProperties(child)[
            nameof(Control.VisualStylesMode)];

        Assert.False(property.ShouldSerializeValue(child));

        child.VisualStylesMode = VisualStylesMode.Disabled;
        Assert.True(property.ShouldSerializeValue(child));

        property.ResetValue(child);
        Assert.Equal(VisualStylesMode.Inherit, child.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Net11, child.EffectiveVisualStylesModeAccessor);
        Assert.False(property.ShouldSerializeValue(child));

        child.VisualStylesMode = VisualStylesMode.Inherit;
        Assert.Equal(VisualStylesMode.Inherit, child.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Net11, child.EffectiveVisualStylesModeAccessor);
        Assert.False(property.ShouldSerializeValue(child));
    }

    [WinFormsFact]
    public void Control_VisualStylesMode_ParentChangeWithLocalValue_DoesNotRaiseChanged()
    {
        using SubControlWithVisualStyles parent = new()
        {
            VisualStylesMode = VisualStylesMode.Classic
        };
        using SubControlWithVisualStyles child = new()
        {
            VisualStylesMode = VisualStylesMode.Disabled
        };
        parent.Controls.Add(child);
        int callCount = 0;
        child.VisualStylesModeChanged += (sender, e) => callCount++;

        parent.VisualStylesMode = VisualStylesMode.Net11;

        Assert.Equal(VisualStylesMode.Disabled, child.VisualStylesMode);
        Assert.Equal(0, callCount);
    }

    [WinFormsFact]
    public void Control_VisualStylesMode_EffectiveEqualityInHighContrast_SkipsEventCascadeAndLayout()
    {
        using SubControlWithVisualStyles parent = new() { HighContrast = true };
        using SubControlWithVisualStyles child = new() { HighContrast = true };
        parent.Controls.Add(child);

        int parentChangedCallCount = 0;
        int childChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        parent.VisualStylesModeChanged += (sender, e) => parentChangedCallCount++;
        child.VisualStylesModeChanged += (sender, e) => childChangedCallCount++;
        parent.Layout += (sender, e) => parentLayoutCallCount++;

        parent.VisualStylesMode = VisualStylesMode.Net11;

        Assert.Equal(VisualStylesMode.Net11, parent.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Inherit, child.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Classic, parent.EffectiveVisualStylesModeAccessor);
        Assert.Equal(VisualStylesMode.Classic, child.EffectiveVisualStylesModeAccessor);
        Assert.Equal(0, parentChangedCallCount);
        Assert.Equal(0, childChangedCallCount);
        Assert.Equal(0, parentLayoutCallCount);
    }

    [WinFormsFact]
    public void Control_VisualStylesMode_ReparentingWithEffectiveEquality_DoesNotRaiseChanged()
    {
        using SubControlWithVisualStyles parent = new()
        {
            HighContrast = true,
            VisualStylesMode = VisualStylesMode.Net11
        };
        using SubControlWithVisualStyles child = new() { HighContrast = true };
        int changedCallCount = 0;
        child.VisualStylesModeChanged += (sender, e) => changedCallCount++;

        child.AssignParent(parent);

        Assert.Equal(VisualStylesMode.Inherit, child.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Classic, child.EffectiveVisualStylesModeAccessor);
        Assert.Equal(0, changedCallCount);
    }

    [WinFormsFact]
    public void Control_VisualStylesMode_DefaultImpact_RepaintsWithoutUpdatingStyles()
    {
        using SubControlWithVisualStyles control = new()
        {
            HighContrast = false,
            VisualStylesMode = VisualStylesMode.Classic
        };
        control.CreateControl();

        bool invalidated = false;
        int styleChangedCallCount = 0;
        control.Invalidated += (sender, e) => invalidated = true;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;

        control.VisualStylesMode = VisualStylesMode.Net11;

        Assert.True(invalidated);
        Assert.Equal(0, styleChangedCallCount);
    }

    [WinFormsTheory]
    [InlineData((int)VisualStylesModeChangeImpactForTest.None)]
    [InlineData((int)VisualStylesModeChangeImpactForTest.Repaint)]
    [InlineData((int)VisualStylesModeChangeImpactForTest.NonClientUpdate)]
    [InlineData((int)VisualStylesModeChangeImpactForTest.Metrics)]
    public void Control_VisualStylesMode_ImpactDispatcher_AppliesExpectedImpact(int impactValue)
    {
        VisualStylesModeChangeImpactForTest impact = (VisualStylesModeChangeImpactForTest)impactValue;

        using ImpactControl control = new()
        {
            HighContrast = false,
            Impact = impact,
            VisualStylesMode = VisualStylesMode.Classic
        };
        control.CreateControl();

        bool invalidated = false;
        int changedCallCount = 0;
        int layoutCallCount = 0;
        int styleChangedCallCount = 0;
        control.Invalidated += (sender, e) => invalidated = true;
        control.Layout += (sender, e) => layoutCallCount++;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        control.VisualStylesModeChanged += (sender, e) => changedCallCount++;

        control.VisualStylesMode = VisualStylesMode.Net11;

        Assert.Equal(1, changedCallCount);

        switch (impact)
        {
            case VisualStylesModeChangeImpactForTest.None:
                Assert.False(invalidated);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, layoutCallCount);
                break;
            case VisualStylesModeChangeImpactForTest.Repaint:
                Assert.True(invalidated);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, layoutCallCount);
                break;
            case VisualStylesModeChangeImpactForTest.NonClientUpdate:
                Assert.True(invalidated);
                Assert.Equal(1, styleChangedCallCount);
                Assert.Equal(0, layoutCallCount);
                break;
            case VisualStylesModeChangeImpactForTest.Metrics:
                Assert.True(invalidated);
                Assert.Equal(1, styleChangedCallCount);
                Assert.Equal(1, layoutCallCount);
                break;
            default:
                throw new InvalidOperationException($"Unexpected impact: {impact}");
        }
    }

    [WinFormsFact]
    public void Control_VisualStylesMode_NoneImpact_RaisesEventAndCascadesToChildren()
    {
        using ImpactControl parent = new()
        {
            HighContrast = false,
            Impact = VisualStylesModeChangeImpactForTest.None,
            VisualStylesMode = VisualStylesMode.Classic
        };
        using SubControlWithVisualStyles child = new() { HighContrast = false };
        parent.Controls.Add(child);
        child.CreateControl();

        int parentChangedCallCount = 0;
        int childChangedCallCount = 0;
        bool childInvalidated = false;
        parent.VisualStylesModeChanged += (sender, e) => parentChangedCallCount++;
        child.VisualStylesModeChanged += (sender, e) => childChangedCallCount++;
        child.Invalidated += (sender, e) => childInvalidated = true;

        parent.VisualStylesMode = VisualStylesMode.Net11;

        Assert.Equal(1, parentChangedCallCount);
        Assert.Equal(1, childChangedCallCount);
        Assert.True(childInvalidated);
    }

    [WinFormsFact]
    public void Control_VisualStylesMode_MetricsImpact_CoalescesLayoutPerContainer()
    {
        using SubControlWithVisualStyles parent = new()
        {
            HighContrast = false,
            VisualStylesMode = VisualStylesMode.Classic
        };
        using ImpactControl firstChild = new()
        {
            HighContrast = false,
            Impact = VisualStylesModeChangeImpactForTest.Metrics
        };
        using ImpactControl secondChild = new()
        {
            HighContrast = false,
            Impact = VisualStylesModeChangeImpactForTest.Metrics
        };
        parent.Controls.Add(firstChild);
        parent.Controls.Add(secondChild);

        int layoutCallCount = 0;
        parent.Layout += (sender, e) => layoutCallCount++;

        parent.VisualStylesMode = VisualStylesMode.Net11;

        Assert.Equal(1, layoutCallCount);
    }

    [WinFormsFact]
    public void Control_VisualStylesMode_ReentrantChange_SuppressesStaleChildCascade()
    {
        using SubControlWithVisualStyles parent = new()
        {
            HighContrast = false,
            VisualStylesMode = VisualStylesMode.Classic
        };
        using SubControlWithVisualStyles child = new() { HighContrast = false };
        parent.Controls.Add(child);

        bool reentered = false;
        int parentChangedCallCount = 0;
        int childChangedCallCount = 0;
        parent.VisualStylesModeChanged += (sender, e) =>
        {
            parentChangedCallCount++;
            if (!reentered)
            {
                reentered = true;
                parent.VisualStylesMode = VisualStylesMode.Latest;
            }
        };
        child.VisualStylesModeChanged += (sender, e) => childChangedCallCount++;

        parent.VisualStylesMode = VisualStylesMode.Net11;

        Assert.Equal(VisualStylesMode.Latest, parent.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Inherit, child.VisualStylesMode);
        Assert.Equal(VisualStylesMode.Latest, child.EffectiveVisualStylesModeAccessor);
        Assert.Equal(2, parentChangedCallCount);
        Assert.Equal(1, childChangedCallCount);
    }

    [WinFormsFact]
    public void Appearance_ToggleSwitch_HasExpectedValue()
    {
        Assert.Equal(2, (int)Appearance.ToggleSwitch);
    }

    private class SubControlWithVisualStyles : Control
    {
        public bool HighContrast { get; set; }

        public VisualStylesMode DefaultVisualStylesModeAccessor => base.DefaultVisualStylesMode;

        public VisualStylesMode EffectiveVisualStylesModeAccessor => base.EffectiveVisualStylesMode;

        public new void AssignParent(Control value) => base.AssignParent(value);

        public new void OnVisualStylesModeChanged(EventArgs e) => base.OnVisualStylesModeChanged(e);

        public new void OnParentVisualStylesModeChanged(EventArgs e) => base.OnParentVisualStylesModeChanged(e);

        internal override bool IsHighContrast => HighContrast;
    }

    private class ImpactControl : SubControlWithVisualStyles
    {
        public VisualStylesModeChangeImpactForTest Impact { get; set; }

        protected override VisualStylesModeChangeImpact GetVisualStylesModeChangeImpact(
            VisualStylesMode oldMode,
            VisualStylesMode newMode)
            => Impact switch
            {
                VisualStylesModeChangeImpactForTest.None => VisualStylesModeChangeImpact.None,
                VisualStylesModeChangeImpactForTest.Repaint => VisualStylesModeChangeImpact.Repaint,
                VisualStylesModeChangeImpactForTest.NonClientUpdate => VisualStylesModeChangeImpact.NonClientUpdate,
                VisualStylesModeChangeImpactForTest.Metrics => VisualStylesModeChangeImpact.Metrics,
                _ => throw new InvalidOperationException($"Unexpected impact: {Impact}"),
            };
    }

    private enum VisualStylesModeChangeImpactForTest
    {
        None,
        Repaint,
        NonClientUpdate,
        Metrics,
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.TrackBar;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class TrackBar_TrackBarChildAccessibleObjectTests
{
    [WinFormsFact]
    public void TrackBarChildAccessibleObject_Ctor_OwnerTrackBarCannotBeNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SubTrackBarChildAccessibleObject(null));
    }

    [WinFormsFact]
    public void TrackBarChildAccessibleObject_Bounds_IsEmptyRectangle_IfOwningControlIsNotCreated()
    {
        using TrackBar control = new();
        SubTrackBarChildAccessibleObject accessibleObject = new(control);

        Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TrackBarChildAccessibleObject_Bounds_IsEmptyRectangle_IfOwningControlIsNotVisible()
    {
        using TrackBar control = new();
        control.CreateControl();
        control.Visible = false;
        SubTrackBarChildAccessibleObject accessibleObject = new(control);

        Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TrackBarChildAccessibleObject_Bounds_IsNotEmptyRectangle()
    {
        using TrackBar control = new();
        control.CreateControl();
        SubTrackBarChildAccessibleObject accessibleObject = new(control);

        Assert.NotEqual(Rectangle.Empty, accessibleObject.Bounds);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TrackBarChildAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using TrackBar control = new();
        SubTrackBarChildAccessibleObject accessibleObject = new(control);

        Assert.Equal(control.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TrackBarChildAccessibleObject_FragmentRoot_ReturnsExpected()
    {
        using TrackBar control = new();
        SubTrackBarChildAccessibleObject accessibleObject = new(control);

        Assert.Equal(control.AccessibilityObject, accessibleObject.FragmentRoot);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void TrackBarChildAccessibleObject_IsDisplayed_ReturnsExpected(bool isVisible)
    {
        using TrackBar control = new();
        control.Visible = isVisible;
        SubTrackBarChildAccessibleObject accessibleObject = new(control);

        Assert.Equal(isVisible, accessibleObject.IsDisplayed);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TrackBarChildAccessibleObject_GetPropertyValue_ControlType_ReturnsExpected()
    {
        using TrackBar control = new();
        SubTrackBarChildAccessibleObject accessibleObject = new(control);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId, (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
    }

    [WinFormsTheory]
    [BoolData]
    public void TrackBarChildAccessibleObject_GetPropertyValue_IsEnabled_ReturnsExpected(bool isEnabled)
    {
        using TrackBar control = new();
        control.Enabled = isEnabled;
        SubTrackBarChildAccessibleObject accessibleObject = new(control);

        Assert.Equal(isEnabled, (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.False(control.IsHandleCreated);
    }

    private class SubTrackBarChildAccessibleObject : TrackBarChildAccessibleObject
    {
        public SubTrackBarChildAccessibleObject(TrackBar owningTrackBar) : base(owningTrackBar)
        { }
    }

    [WinFormsFact]
    public void TrackBarChildAccessibleObject_Role_ReturnsExpected()
    {
        using TrackBar control = new();
        var accessibleObject = new SubTrackBarChildAccessibleObject(control);
        accessibleObject.Role.Should().Be(AccessibleRole.None);
    }

    [WinFormsFact]
    public void TrackBarChildAccessibleObject_State_ReturnsExpected()
    {
        using TrackBar control = new();
        var accessibleObject = new SubTrackBarChildAccessibleObject(control);

        accessibleObject.State.Should().Be(AccessibleStates.None);
        control.IsHandleCreated.Should().BeFalse();
    }
}

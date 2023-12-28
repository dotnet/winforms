// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ScrollBar;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ScrollBar_ScrollBarChildAccessibleObjectTests
{
    [WinFormsFact]
    public void ScrollBarChildAccessibleObject_Ctor_Default()
    {
        ScrollBarChildAccessibleObject accessibleObject = new(null);
        Assert.Null(accessibleObject.OwningScrollBar);
    }

    [WinFormsFact]
    public void ScrollBarChildAccessibleObject_Bounds_IsEmptyRectangle_IfOwningControlIsNotCreated()
    {
        using SubScrollBar control = new();
        ScrollBarChildAccessibleObject accessibleObject = new(control);

        Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ScrollBarChildAccessibleObject_Bounds_IsEmptyRectangle_IfOwningControlIsNotVisible()
    {
        using SubScrollBar control = new();
        control.CreateControl();
        control.Visible = false;
        ScrollBarChildAccessibleObject accessibleObject = new(control);

        Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ScrollBarChildAccessibleObject_Bounds_IsNotEmptyRectangle()
    {
        using SubScrollBar control = new();
        control.CreateControl();
        ScrollBarChildAccessibleObject accessibleObject = new(control);

        Assert.NotEqual(Rectangle.Empty, accessibleObject.Bounds);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ScrollBarChildAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using SubScrollBar control = new();
        ScrollBarChildAccessibleObject accessibleObject = new(control);

        Assert.Equal(control.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ScrollBarChildAccessibleObject_FragmentRoot_ReturnsExpected()
    {
        using SubScrollBar control = new();
        ScrollBarChildAccessibleObject accessibleObject = new(control);

        Assert.Equal(control.AccessibilityObject, accessibleObject.FragmentRoot);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ScrollBarChildAccessibleObject_IsDisplayed_ReturnsExpected(bool isVisible)
    {
        using SubScrollBar control = new();
        control.Visible = isVisible;
        ScrollBarChildAccessibleObject accessibleObject = new(control);

        Assert.Equal(isVisible, accessibleObject.IsDisplayed);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ScrollBarChildAccessibleObject_GetPropertyValue_ControlType_ReturnsExpected()
    {
        ScrollBarChildAccessibleObject accessibleObject = new(null);
        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId, (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
    }

    [WinFormsTheory]
    [BoolData]
    public void ScrollBarChildAccessibleObject_GetPropertyValue_IsEnabled_ReturnsExpected(bool isEnabled)
    {
        using SubScrollBar control = new();
        control.Enabled = isEnabled;
        ScrollBarChildAccessibleObject accessibleObject = new(control);

        Assert.Equal(isEnabled, (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.False(control.IsHandleCreated);
    }

    private class SubScrollBar : ScrollBar
    { }
}

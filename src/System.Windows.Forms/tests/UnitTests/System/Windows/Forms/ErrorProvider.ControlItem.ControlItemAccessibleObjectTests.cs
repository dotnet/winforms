﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection;
using static System.Windows.Forms.ErrorProvider;
using static Interop;

namespace System.Windows.Forms.Tests;

public class ErrorProvider_ControlItem_ControlItemAccessibleObjectTests
{
    [WinFormsFact]
    public void ControlItemAccessibleObjectTests_Ctor_Default()
    {
        Type type = typeof(ControlItem)
            .GetNestedType("ControlItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null, null, null, null });

        Assert.Null(accessibleObject.TestAccessor().Dynamic._controlItem);
        Assert.Null(accessibleObject.TestAccessor().Dynamic._window);
        Assert.Null(accessibleObject.TestAccessor().Dynamic._control);
        Assert.Null(accessibleObject.TestAccessor().Dynamic._provider);
        Assert.Equal(AccessibleRole.Alert, accessibleObject.Role);
    }

    [WinFormsFact]
    public void ControlItemAccessibleObjectTests_Bounds_ReturnsEmptyRectangle_IfControlIsNotCreated()
    {
        using Control control = new();
        Type type = typeof(ControlItem)
            .GetNestedType("ControlItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null, null, control, null });

        Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ControlItemAccessibleObjectTests_Bounds_ReturnsNoEmptyRectangle_IfParentControlIsCreated()
    {
        using Control parentControl = new();
        using Control control = new();
        parentControl.Controls.Add(control);
        using ErrorProvider provider = new();
        ErrorWindow window = new(provider, control);
        ControlItem item = new(provider, control, (IntPtr)100);
        window.Add(item);
        parentControl.CreateControl();

        Type type = typeof(ControlItem)
            .GetNestedType("ControlItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { item, window, control, provider });

        Assert.NotEqual(Rectangle.Empty, accessibleObject.Bounds);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ControlItemAccessibleObjectTests_Parent_ReturnsExpected()
    {
        using Control control = new();
        using ErrorProvider provider = new();
        ErrorWindow window = new(provider, control);

        Type type = typeof(ControlItem)
            .GetNestedType("ControlItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null, window, control, provider });

        Assert.Equal(window.AccessibilityObject, accessibleObject.Parent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ControlItemAccessibleObjectTests_FragmentRoot_ReturnsExpected()
    {
        using Control control = new();
        using ErrorProvider provider = new();
        ErrorWindow window = new(provider, control);

        Type type = typeof(ControlItem)
            .GetNestedType("ControlItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null, window, control, provider });

        Assert.Equal(window.AccessibilityObject, accessibleObject.FragmentRoot);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ControlItemAccessibleObjectTests_IsReadOnly_ReturnsExpected()
    {
        Type type = typeof(ControlItem)
            .GetNestedType("ControlItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null, null, null, null });

        Assert.True(accessibleObject.IsReadOnly);
    }

    [WinFormsFact]
    public void ControlItemAccessibleObjectTests_State_ReturnsExpected()
    {
        Type type = typeof(ControlItem)
            .GetNestedType("ControlItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null, null, null, null });

        Assert.Equal(AccessibleStates.HasPopup | AccessibleStates.ReadOnly, accessibleObject.State);
    }

    [WinFormsTheory]
    [InlineData("")]
    [InlineData(null)]

    public void ControlItemAccessibleObjectTests_Name_ReturnsExpected_IfNameIsNullOrEmptyString(string testName)
    {
        string testError = "This is test string";
        using Control control = new();
        using ErrorProvider provider = new();
        ErrorWindow window = new(provider, control);
        ControlItem item = new(provider, control, (IntPtr)100) { Error = testError };
        window.Add(item);

        AccessibleObject accessibleObject = item.AccessibilityObject;
        accessibleObject.Name = testName;

        Assert.Equal(testError, accessibleObject.Name);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UiaCore.UIA.LegacyIAccessiblePatternId)]
    public void ControlItemAccessibleObjectTests_IsPatternSupported_ReturnsExpected(int patternId)
    {
        Type type = typeof(ControlItem)
            .GetNestedType("ControlItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null, null, null, null });

        Assert.True(accessibleObject.IsPatternSupported((UiaCore.UIA)patternId));
    }

    [WinFormsFact]
    public void ControlItemAccessibleObjectTests_IsIAccessibleExSupported_ReturnsExpected_IfItemIsNotNull()
    {
        using Control control = new();
        using ErrorProvider provider = new();
        ErrorWindow window = new(provider, control);
        ControlItem item = new(provider, control, (IntPtr)100);
        window.Add(item);

        Type type = typeof(ControlItem)
            .GetNestedType("ControlItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { item, window, control, provider });

        Assert.NotNull(accessibleObject.TestAccessor().Dynamic._controlItem);
        Assert.True(accessibleObject.IsIAccessibleExSupported());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ControlItemAccessibleObjectTests_ControlType_ReturnsExpected()
    {
        Type type = typeof(ControlItem)
            .GetNestedType("ControlItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null, null, null, null });

        Assert.Equal(UiaCore.UIA.ImageControlTypeId, accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
    }

    [WinFormsFact]
    public void ControlItemAccessibleObjectTests_GetPropertyValue_ReturnsExpected()
    {
        Type type = typeof(ControlItem)
           .GetNestedType("ControlItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null, null, null, null });

        Assert.Null(accessibleObject.GetPropertyValue(UiaCore.UIA.ValueValuePropertyId));
        Assert.Null(accessibleObject.GetPropertyValue(UiaCore.UIA.LegacyIAccessibleDefaultActionPropertyId));
        Assert.Equal(AccessibleStates.ReadOnly | AccessibleStates.HasPopup, accessibleObject.GetPropertyValue(UiaCore.UIA.LegacyIAccessibleStatePropertyId));
    }

    [WinFormsFact]
    public void ControlItemAccessibleObjectTests_GetChildId_ReturnsExpected()
    {
        using Control control = new();
        using ErrorProvider provider = new();
        ErrorWindow window = new(provider, control);

        ControlItem item1 = new(provider, control, (IntPtr)100);
        ControlItem item2 = new(provider, control, (IntPtr)200);
        ControlItem item3 = new(provider, control, (IntPtr)300);

        window.ControlItems.AddRange(new[] { item1, item2, item3 });

        Type type = typeof(ControlItem)
            .GetNestedType("ControlItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject1 = (AccessibleObject)Activator.CreateInstance(type, new object[] { item1, window, control, provider });
        var accessibleObject2 = (AccessibleObject)Activator.CreateInstance(type, new object[] { item2, window, control, provider });
        var accessibleObject3 = (AccessibleObject)Activator.CreateInstance(type, new object[] { item3, window, control, provider });

        // Make sure that returns index in window.Items collection + 1

        Assert.Equal(1, accessibleObject1.GetChildId());
        Assert.Equal(2, accessibleObject2.GetChildId());
        Assert.Equal(3, accessibleObject3.GetChildId());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ControlItemAccessibleObjectTests_FragmentNavigate_Parent_ReturnsExpected()
    {
        using Control control = new();
        using ErrorProvider provider = new();
        ErrorWindow window = new(provider, control);

        Type type = typeof(ControlItem)
            .GetNestedType("ControlItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null, window, control, provider });

        Assert.Equal(window.AccessibilityObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ControlItemAccessibleObjectTests_FragmentNavigate_NextSibling_ReturnsExpected()
    {
        using Control control = new();
        using ErrorProvider provider = new();
        ErrorWindow window = new(provider, control);

        ControlItem item1 = new(provider, control, (IntPtr)100);
        ControlItem item2 = new(provider, control, (IntPtr)200);
        ControlItem item3 = new(provider, control, (IntPtr)300);

        window.ControlItems.AddRange(new[] { item1, item2, item3 });

        AccessibleObject accessibleObject1 = item1.AccessibilityObject;
        AccessibleObject accessibleObject2 = item2.AccessibilityObject;
        AccessibleObject accessibleObject3 = item3.AccessibilityObject;

        // Window is null while controlItem isn't added to window.
        Assert.Null(accessibleObject1.TestAccessor().Dynamic._window);
        Assert.Null(accessibleObject2.TestAccessor().Dynamic._window);
        Assert.Null(accessibleObject3.TestAccessor().Dynamic._window);

        // So add the reference manually.
        accessibleObject1.TestAccessor().Dynamic._window = window;
        accessibleObject2.TestAccessor().Dynamic._window = window;
        accessibleObject3.TestAccessor().Dynamic._window = window;

        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ControlItemAccessibleObjectTests_FragmentNavigate_PreviousSibling_ReturnsExpected()
    {
        using Control control = new();
        using ErrorProvider provider = new();
        ErrorWindow window = new(provider, control);

        ControlItem item1 = new(provider, control, (IntPtr)100);
        ControlItem item2 = new(provider, control, (IntPtr)200);
        ControlItem item3 = new(provider, control, (IntPtr)300);

        window.ControlItems.AddRange(new[] { item1, item2, item3 });

        AccessibleObject accessibleObject1 = item1.AccessibilityObject;
        AccessibleObject accessibleObject2 = item2.AccessibilityObject;
        AccessibleObject accessibleObject3 = item3.AccessibilityObject;

        // Window is null while controlItem isn't added to window.
        Assert.Null(accessibleObject1.TestAccessor().Dynamic._window);
        Assert.Null(accessibleObject2.TestAccessor().Dynamic._window);
        Assert.Null(accessibleObject3.TestAccessor().Dynamic._window);

        // So add the reference manually.
        accessibleObject1.TestAccessor().Dynamic._window = window;
        accessibleObject2.TestAccessor().Dynamic._window = window;
        accessibleObject3.TestAccessor().Dynamic._window = window;

        Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }
}

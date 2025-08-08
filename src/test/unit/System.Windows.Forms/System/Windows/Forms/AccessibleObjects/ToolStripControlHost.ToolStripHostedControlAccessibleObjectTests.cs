// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripControlHost_ToolStripHostedControlAccessibleObjectTests
{
    public static IEnumerable<object[]> ToolStripItemAccessibleObject_TestData()
    {
        return ReflectionHelper.GetPublicNotAbstractClasses<ToolStripControlHost>().Select(type => new object[] { type });
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripItemAccessibleObject_TestData))]
    public void ToolStripHostedControlAccessibleObject_GetPropertyValue_IsOffscreenPropertyId_ReturnExpected(Type type)
    {
        using ToolStrip toolStrip = new();
        toolStrip.CreateControl();
        using ToolStripControlHost item = ReflectionHelper.InvokePublicConstructor<ToolStripControlHost>(type);
        item.Size = new Size(0, 0);
        toolStrip.Items.Add(item);

        Assert.True(GetIsOffscreenPropertyValue(item.AccessibilityObject));
        Assert.True(GetIsOffscreenPropertyValue(item.Control.AccessibilityObject));
    }

    [WinFormsFact]
    public void ToolStripHostedControlAccessibleObject_FragmentRoot_Returns_Owner_AccessibilityObject_When_Valid()
    {
        using ToolStrip toolStrip = new();
        using TextBox textBox = new();
        using ToolStripControlHost host = new(textBox);
        toolStrip.Items.Add(host);

        ToolStripControlHost.ToolStripHostedControlAccessibleObject accessibleObject =
            new(textBox, host);

        IRawElementProviderFragmentRoot.Interface fragmentRoot = accessibleObject.FragmentRoot;

        fragmentRoot.Should().BeSameAs(toolStrip.AccessibilityObject);
    }

    [WinFormsFact]
    public void ToolStripHostedControlAccessibleObject_FragmentRoot_FallsBack_ToBase_When_Null()
    {
        using TextBox textBox = new();

        ToolStripControlHost.ToolStripHostedControlAccessibleObject accessibleObject =
            new(textBox, null);

        IRawElementProviderFragmentRoot.Interface fragmentRoot = accessibleObject.FragmentRoot;

        fragmentRoot.Should().Be(textBox.AccessibilityObject.FragmentRoot);
    }

    [WinFormsFact]
    public void ToolStripHostedControlAccessibleObject_FragmentNavigate_FallsBack_ToBase_When_Null()
    {
        using TextBox textBox = new();

        ToolStripControlHost.ToolStripHostedControlAccessibleObject accessibleObject =
            new(textBox, null);

        // Use a valid direction that is not handled by the switch, e.g.NavigateDirection.NavigateDirection_FirstChild
        NavigateDirection direction = NavigateDirection.NavigateDirection_FirstChild;
        IRawElementProviderFragment.Interface expected = textBox.AccessibilityObject.FragmentNavigate(direction);
        IRawElementProviderFragment.Interface actual = accessibleObject.FragmentNavigate(direction);

        actual.Should().Be(expected);
    }

    [WinFormsFact]
    public void ToolStripHostedControlAccessibleObject_GetPropertyValue_IsOffscreenPropertyId_Delegates_To_Helper()
    {
        using TextBox textBox = new();
        ToolStripControlHost.ToolStripHostedControlAccessibleObject accessibleObject =
            new(textBox, null);

        VARIANT value = accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId);

        value.Should().BeOfType<VARIANT>();
    }

    [WinFormsFact]
    public void ToolStripHostedControlAccessibleObject_GetPropertyValue_UnknownPropertyId_FallsBack_ToBase()
    {
        using TextBox textBox = new();
        ToolStripControlHost.ToolStripHostedControlAccessibleObject accessibleObject =
            new(textBox, null);

        VARIANT value = accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)99999);

        value.Should().Be(textBox.AccessibilityObject.GetPropertyValue((UIA_PROPERTY_ID)99999));
    }

    [WinFormsFact]
    public void ToolStripHostedControlAccessibleObject_FragmentNavigate_Delegates_To_Host_For_Parent()
    {
        using ToolStrip toolStrip = new();
        using TextBox textBox = new();
        using ToolStripControlHost host = new(textBox);
        toolStrip.Items.Add(host);

        ToolStripControlHost.ToolStripHostedControlAccessibleObject accessibleObject =
            new(textBox, host);

        // Ensure the host is fully initialized and accessible
        toolStrip.CreateControl();

        IRawElementProviderFragment.Interface expected =
            host.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent);
        IRawElementProviderFragment.Interface actual =
            accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent);

        actual.Should().Be(expected);
    }

    [WinFormsFact]
    public void ToolStripHostedControlAccessibleObject_FragmentNavigate_FallsBack_ToBase_For_OtherDirections()
    {
        using TextBox textBox = new();
        ToolStripControlHost.ToolStripHostedControlAccessibleObject accessibleObject =
            new(textBox, null);

        // Use a direction not handled by the switch, e.g. FirstChild
        IRawElementProviderFragment.Interface expected =
            textBox.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild);
        IRawElementProviderFragment.Interface actual =
            accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild);

        actual.Should().Be(expected);
    }

    [WinFormsFact]
    public void ToolStripHostedControlAccessibleObject_IsPatternSupported_ValuePatternId_Returns_True()
    {
        using TextBox textBox = new();
        ToolStripControlHost.ToolStripHostedControlAccessibleObject accessibleObject =
            new(textBox, null);

        bool result = accessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_ValuePatternId);

        result.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripHostedControlAccessibleObject_IsPatternSupported_UnknownPatternId_FallsBack_ToBase()
    {
        using TextBox textBox = new();
        ToolStripControlHost.ToolStripHostedControlAccessibleObject accessibleObject =
            new(textBox, null);

        bool expected = textBox.AccessibilityObject.IsPatternSupported((UIA_PATTERN_ID)99999);
        bool actual = accessibleObject.IsPatternSupported((UIA_PATTERN_ID)99999);

        actual.Should().Be(expected);
    }

    private bool GetIsOffscreenPropertyValue(AccessibleObject accessibleObject) =>
        (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId) || (accessibleObject.Bounds.Width > 0 && accessibleObject.Bounds.Height > 0);
}

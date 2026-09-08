// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripControlHost_ToolStripHostedControlAccessibleObjectTests : IDisposable
{
    private readonly ToolStrip _toolStrip;
    private readonly TextBox _textBox;
    private readonly ToolStripControlHost _host;

    public ToolStripControlHost_ToolStripHostedControlAccessibleObjectTests()
    {
        _toolStrip = new ToolStrip();
        _textBox = new TextBox();
        _host = new ToolStripControlHost(_textBox);
        _toolStrip.Items.Add(_host);
    }

    public void Dispose()
    {
        _host.Dispose();
        _textBox.Dispose();
        _toolStrip.Dispose();
    }

    public static IEnumerable<object[]> ToolStripItemAccessibleObject_TestData()
    {
        return ReflectionHelper.GetPublicNotAbstractClasses<ToolStripControlHost>().Select(type => new object[] { type });
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripItemAccessibleObject_TestData))]
    public void ToolStripHostedControlAccessibleObject_GetPropertyValue_IsOffscreenPropertyId_ReturnExpected(Type type)
    {
        _toolStrip.CreateControl();
        using ToolStripControlHost item = ReflectionHelper.InvokePublicConstructor<ToolStripControlHost>(type);
        item.Size = new Size(0, 0);
        _toolStrip.Items.Add(item);

        GetIsOffscreenPropertyValue(item.AccessibilityObject).Should().BeTrue();
        GetIsOffscreenPropertyValue(item.Control.AccessibilityObject).Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripHostedControlAccessibleObject_FragmentRoot_Returns_Owner_AccessibilityObject_When_Valid()
    {
        ToolStripControlHost.ToolStripHostedControlAccessibleObject accessibleObject =
            new(_textBox, _host);
        IRawElementProviderFragmentRoot.Interface fragmentRoot = accessibleObject.FragmentRoot;

        fragmentRoot.Should().BeSameAs(_toolStrip.AccessibilityObject);
    }

    [WinFormsFact]
    public void ToolStripHostedControlAccessibleObject_FragmentRoot_FallsBack_ToBase_When_Null()
    {
        ToolStripControlHost.ToolStripHostedControlAccessibleObject accessibleObject =
            new(_textBox, null);
        IRawElementProviderFragmentRoot.Interface fragmentRoot = accessibleObject.FragmentRoot;

        fragmentRoot.Should().Be(_textBox.AccessibilityObject.FragmentRoot);
    }

    [WinFormsFact]
    public void ToolStripHostedControlAccessibleObject_FragmentNavigate_FallsBack_ToBase_When_Null()
    {
        ToolStripControlHost.ToolStripHostedControlAccessibleObject accessibleObject =
            new(_textBox, null);

        // Use a valid direction that is not handled by the switch, e.g. NavigateDirection.NavigateDirection_FirstChild
        NavigateDirection direction = NavigateDirection.NavigateDirection_FirstChild;
        IRawElementProviderFragment.Interface expected = _textBox.AccessibilityObject.FragmentNavigate(direction);
        IRawElementProviderFragment.Interface actual = accessibleObject.FragmentNavigate(direction);

        actual.Should().Be(expected);
    }

    [WinFormsFact]
    public void ToolStripHostedControlAccessibleObject_ReturnsVariant_ForIsOffscreenPropertyId()
    {
        ToolStripControlHost.ToolStripHostedControlAccessibleObject accessibleObject =
            new(_textBox, null);
        VARIANT value = accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId);

        value.Type.Should().Be(VARENUM.VT_BOOL);
        ((bool)value).Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripHostedControlAccessibleObject_GetPropertyValue_UnknownPropertyId_FallsBack_ToBase()
    {
        ToolStripControlHost.ToolStripHostedControlAccessibleObject accessibleObject =
            new(_textBox, null);
        VARIANT value = accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)99999);

        value.Should().Be(_textBox.AccessibilityObject.GetPropertyValue((UIA_PROPERTY_ID)99999));
    }

    [WinFormsFact]
    public void ToolStripHostedControlAccessibleObject_FragmentNavigate_Delegates_To_Host_For_Parent()
    {
        // Ensure the host is fully initialized and accessible
        _toolStrip.CreateControl();
        ToolStripControlHost.ToolStripHostedControlAccessibleObject accessibleObject =
            new(_textBox, _host);
        IRawElementProviderFragment.Interface expected =
            _host.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent);
        IRawElementProviderFragment.Interface actual =
            accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent);

        actual.Should().Be(expected);
    }

    [WinFormsFact]
    public void ToolStripHostedControlAccessibleObject_FragmentNavigate_FallsBack_ToBase_For_OtherDirections()
    {
        ToolStripControlHost.ToolStripHostedControlAccessibleObject accessibleObject =
            new(_textBox, null);

        // Use a direction not handled by the switch, e.g. FirstChild
        IRawElementProviderFragment.Interface expected =
            _textBox.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild);
        IRawElementProviderFragment.Interface actual =
            accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild);

        actual.Should().Be(expected);
    }

    [WinFormsFact]
    public void ToolStripHostedControlAccessibleObject_IsPatternSupported_ValuePatternId_Returns_True()
    {
        ToolStripControlHost.ToolStripHostedControlAccessibleObject accessibleObject =
            new(_textBox, null);
        bool result = accessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_ValuePatternId);

        result.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripHostedControlAccessibleObject_IsPatternSupported_UnknownPatternId_FallsBack_ToBase()
    {
        ToolStripControlHost.ToolStripHostedControlAccessibleObject accessibleObject =
            new(_textBox, null);
        bool expected = _textBox.AccessibilityObject.IsPatternSupported((UIA_PATTERN_ID)99999);
        bool actual = accessibleObject.IsPatternSupported((UIA_PATTERN_ID)99999);

        actual.Should().Be(expected);
    }

    private bool GetIsOffscreenPropertyValue(AccessibleObject accessibleObject) =>
        (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId) || (accessibleObject.Bounds.Width > 0 && accessibleObject.Bounds.Height > 0);
}

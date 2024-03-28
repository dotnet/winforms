// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class Splitter_SplitterAccessibleObjectTests
{
    [WinFormsFact]
    public void SplitterAccessibleObject_Ctor_NullControl_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("ownerControl", () => new Splitter.SplitterAccessibleObject(null));
    }

    [WinFormsFact]
    public void SplitterAccessibleObject_Ctor_Default()
    {
        using Splitter splitter = new();
        Assert.False(splitter.IsHandleCreated);
        var splitterAccessibleObject = new Splitter.SplitterAccessibleObject(splitter);

        Assert.NotNull(splitterAccessibleObject.Owner);
        Assert.False(splitter.IsHandleCreated);
    }

    [WinFormsFact]
    public void SplitterAccessibleObject_Description_ReturnsExpected()
    {
        using Splitter splitter = new()
        {
            AccessibleDescription = "TestDescription"
        };

        Assert.False(splitter.IsHandleCreated);
        var splitterAccessibleObject = new Splitter.SplitterAccessibleObject(splitter);

        Assert.Equal("TestDescription", splitterAccessibleObject.Description);
        Assert.False(splitter.IsHandleCreated);
    }

    [WinFormsFact]
    public void SplitterAccessibleObject_Name_ReturnsExpected()
    {
        using Splitter splitter = new()
        {
            AccessibleName = "TestName"
        };

        Assert.False(splitter.IsHandleCreated);
        var splitterAccessibleObject = new Splitter.SplitterAccessibleObject(splitter);

        Assert.Equal("TestName", splitterAccessibleObject.Name);
        Assert.False(splitter.IsHandleCreated);
    }

    [WinFormsFact]
    public void SplitterAccessibleObject_CustomRole_ReturnsExpected()
    {
        using Splitter splitter = new()
        {
            AccessibleRole = AccessibleRole.PushButton
        };

        Assert.False(splitter.IsHandleCreated);
        var splitterAccessibleObject = new Splitter.SplitterAccessibleObject(splitter);

        Assert.Equal(AccessibleRole.PushButton, splitterAccessibleObject.Role);
        Assert.False(splitter.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.Client)]
    [InlineData(false, AccessibleRole.None)]
    public void SplitterAccessibleObject_DefaultRole_ReturnsNone_IfControlIsNotCreated(bool createControl, AccessibleRole accessibleRole)
    {
        using Splitter splitter = new();

        if (createControl)
        {
            splitter.CreateControl();
        }

        var splitterAccessibleObject = new Splitter.SplitterAccessibleObject(splitter);

        Assert.Equal(accessibleRole, splitterAccessibleObject.Role);
        Assert.Equal(createControl, splitter.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_NamePropertyId, "TestName")]
    [InlineData((int)UIA_PROPERTY_ID.UIA_ControlTypePropertyId, (int)UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId)] // If AccessibleRole is Default
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId, true)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_AutomationIdPropertyId, "Splitter1")]
    public void SplitterAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
    {
        using Splitter splitter = new()
        {
            Name = "Splitter1",
            AccessibleName = "TestName"
        };

        Assert.False(splitter.IsHandleCreated);
        var splitterAccessibleObject = new Splitter.SplitterAccessibleObject(splitter);
        using VARIANT value = splitterAccessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyID);

        Assert.Equal(expected, value.ToObject());
        Assert.False(splitter.IsHandleCreated);
    }

    [WinFormsFact]
    public void SplitterAccessibleObject_IsPatternSupported_Invoke_ReturnsTrue_ForLegacyIAccessiblePattern()
    {
        using Splitter splitter = new()
        {
            Name = "Splitter1"
        };
        Assert.False(splitter.IsHandleCreated);
        var splitterAccessibleObject = new Splitter.SplitterAccessibleObject(splitter);

        Assert.True(splitterAccessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId));
        Assert.False(splitter.IsHandleCreated);
    }

    public static IEnumerable<object[]> SplitterAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
    {
        Array roles = Enum.GetValues(typeof(AccessibleRole));

        foreach (AccessibleRole role in roles)
        {
            if (role == AccessibleRole.Default)
            {
                continue; // The test checks custom roles
            }

            yield return new object[] { role };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SplitterAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void SplitterAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using Splitter splitter = new();
        splitter.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)splitter.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(splitter.IsHandleCreated);
    }
}

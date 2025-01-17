// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using LabelAccessibleObject = System.Windows.Forms.Label.LabelAccessibleObject;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class LabelAccessibleObjectTests
{
    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_NamePropertyId, "Address")]
    [InlineData((int)UIA_PROPERTY_ID.UIA_AutomationIdPropertyId, "Label1")]
    public void LabelAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, string expected)
    {
        using Label label = new()
        {
            Text = "Some test label text",
            Name = "Label1",
            AccessibleName = "Address"
        };

        LabelAccessibleObject accessibilityObject = (LabelAccessibleObject)label.AccessibilityObject;

        string value = ((BSTR)accessibilityObject.GetPropertyValue((UIA_PROPERTY_ID)propertyID)).ToStringAndFree();
        Assert.Equal(expected, value);
        Assert.False(label.IsHandleCreated);
    }

    [WinFormsFact]
    public void LabelAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
    {
        using Label label = new();
        label.Name = "Label1";
        label.Text = "Some test label text";
        LabelAccessibleObject labelAccessibleObject = new(label);

        Assert.False(label.IsHandleCreated);

        bool supportsLegacyIAccessiblePatternId = labelAccessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId);
        Assert.True(supportsLegacyIAccessiblePatternId);
    }

    [WinFormsFact]
    public void LabelAccessibleObject_LegacyIAccessible_Role_ReturnsExpected()
    {
        using Label label = new();
        label.Name = "Label1";
        label.Text = "Some test label text";
        label.AccessibleRole = AccessibleRole.Link;
        LabelAccessibleObject labelAccessibleObject = new(label);

        Assert.False(label.IsHandleCreated);
        Assert.Equal(AccessibleRole.Link, labelAccessibleObject.Role);
    }

    [WinFormsFact]
    public void LabelAccessibleObject_Role_IsStaticText_ByDefault()
    {
        using Label label = new();
        // AccessibleRole is not set = Default

        AccessibleRole actual = label.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.StaticText, actual);
        Assert.False(label.IsHandleCreated);
    }

    [WinFormsFact]
    public void LabelAccessibleObject_LegacyIAccessible_Description_ReturnsExpected()
    {
        string testAccDescription = "Test description";
        using Label label = new();
        label.Name = "Label1";
        label.Text = "Some test label text";
        label.AccessibleDescription = testAccDescription;
        LabelAccessibleObject labelAccessibleObject = new(label);

        Assert.False(label.IsHandleCreated);
        Assert.Equal(testAccDescription, labelAccessibleObject.Description);
    }

    [WinFormsFact]
    public void LabelAccessibleObject_ControlType_IsText_IfAccessibleRoleIsDefault()
    {
        using Label label = new();
        // AccessibleRole is not set = Default

        VARIANT actual = label.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_TextControlTypeId, (UIA_CONTROLTYPE_ID)(int)actual);
        Assert.False(label.IsHandleCreated);
    }

    public static IEnumerable<object[]> LabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(LabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void LabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using Label label = new();
        label.AccessibleRole = role;

        VARIANT actual = label.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, (UIA_CONTROLTYPE_ID)(int)actual);
        Assert.False(label.IsHandleCreated);
    }

    [WinFormsFact]
    public void LabelAccessibleObject_TextChanged_AutomationPropertyChanged_Raised()
    {
        const string newText = "New text";
        using LabelWithCustomAccessibleObject control = new(
            (propertyId, value) => propertyId == UIA_PROPERTY_ID.UIA_NamePropertyId && newText.Equals(value.ToObject()))
        {
            Text = "Text"
        };

        var accessibilityObject = control.AccessibilityObject as ControlAccessibleObjectWithNotificationCounter;
        Assert.NotNull(accessibilityObject);
        Assert.True(control.IsAccessibilityObjectCreated);
        Assert.Equal(0, accessibilityObject.RaiseAutomationNotificationCallCount);

        control.Text = newText;

        Assert.Equal(1, accessibilityObject.RaiseAutomationNotificationCallCount);
    }

    [WinFormsTheory]
    [InlineData("&File", false, "&File")]
    [InlineData("&File", true, "File")]
    public void LabelAccessibleObject_Name_ReturnsExpected_WithUseMnemonic(string text, bool useMnemonic, string expectedName)
    {
        using Label label = new() { Text = text, UseMnemonic = useMnemonic };
        LabelAccessibleObject accessibleObject = (LabelAccessibleObject)label.AccessibilityObject;

        string name = accessibleObject.Name;

        name.Should().Be(expectedName);
    }

    [WinFormsFact]
    public void LabelAccessibleObject_ThrowsArgumentNullException_WhenOwnerIsNull()
    {
        Action action = () => new LabelAccessibleObject(null);

        action.Should().Throw<ArgumentNullException>();
    }

    [WinFormsTheory]
    [InlineData("Test Accessible Name", "Test Accessible Name", false)]
    [InlineData("Test Accessible Name", "Test Accessible Name", true)]
    [InlineData("", null, false)]
    [InlineData("", null, true)]
    [InlineData(null, null, false)]
    [InlineData(null, null, true)]
    public void LabelAccessibleObject_Name_ReturnsExpected_WithVariousText(string labelText, string expectedName, bool useMnemonic)
    {
        using Label label = new() { Text = labelText, UseMnemonic = useMnemonic };
        LabelAccessibleObject accessibleObject = (LabelAccessibleObject)label.AccessibilityObject;

        string name = accessibleObject.Name;

        name.Should().Be(expectedName);
    }

    private class LabelWithCustomAccessibleObject : Label
    {
        private readonly Func<UIA_PROPERTY_ID, VARIANT, bool> _checkRaisedEvent;

        public LabelWithCustomAccessibleObject(Func<UIA_PROPERTY_ID, VARIANT, bool> checkRaisedEvent)
        {
            _checkRaisedEvent = checkRaisedEvent;
        }

        protected override AccessibleObject CreateAccessibilityInstance() => new ControlAccessibleObjectWithNotificationCounter(this, _checkRaisedEvent);
    }

    private class ControlAccessibleObjectWithNotificationCounter : Control.ControlAccessibleObject
    {
        private readonly Func<UIA_PROPERTY_ID, VARIANT, bool> _checkRaisedEvent;

        public ControlAccessibleObjectWithNotificationCounter(Control ownerControl, Func<UIA_PROPERTY_ID, VARIANT, bool> checkRaisedEvent) : base(ownerControl)
        {
            _checkRaisedEvent = checkRaisedEvent;
        }

        internal int RaiseAutomationNotificationCallCount { get; private set; }

        internal override bool RaiseAutomationPropertyChangedEvent(UIA_PROPERTY_ID propertyId, VARIANT oldValue, VARIANT newValue)
        {
            if (_checkRaisedEvent(propertyId, newValue))
            {
                RaiseAutomationNotificationCallCount++;
            }

            return base.RaiseAutomationPropertyChangedEvent(propertyId, oldValue, newValue);
        }
    }
}

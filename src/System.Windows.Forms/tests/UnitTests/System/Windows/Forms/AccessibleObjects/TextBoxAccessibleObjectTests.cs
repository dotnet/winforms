// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class TextBoxAccessibleObjectTests
{
    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsTextPatternAvailablePropertyId)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsTextPattern2AvailablePropertyId)]
    public void TextBoxAccessibleObject_TextPatternAvailable(int propertyId)
    {
        using TextBox textBox = new();
        AccessibleObject textBoxAccessibleObject = textBox.AccessibilityObject;

        // Interop.UiaCore.UIA accessible level (internal) is less than the test level (public) so it needs boxing and unboxing
        Assert.True((bool)textBoxAccessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyId));
        Assert.False(textBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_TextPatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_TextPattern2Id)]
    [InlineData((int)UIA_PATTERN_ID.UIA_ValuePatternId)]
    public void TextBoxAccessibleObject_PatternSupported(int patternId)
    {
        using TextBox textBox = new();
        AccessibleObject textBoxAccessibleObject = textBox.AccessibilityObject;

        // Interop.UiaCore.UIA accessible level (internal) is less than the test level (public) so it needs boxing and unboxing
        Assert.True(textBoxAccessibleObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
        Assert.False(textBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxAccessibilityObject_ControlType_IsEdit_IfAccessibleRoleIsDefault()
    {
        using TextBox textBox = new();
        textBox.CreateControl();
        // AccessibleRole is not set = Default

        var actual = (UIA_CONTROLTYPE_ID)(int)textBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_EditControlTypeId, actual);
        Assert.True(textBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxAccessibilityObject_Role_IsText_ByDefault()
    {
        using TextBox textBox = new();
        textBox.CreateControl();
        // AccessibleRole is not set = Default

        AccessibleRole actual = textBox.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.Text, actual);
        Assert.True(textBox.IsHandleCreated);
    }

    public static IEnumerable<object[]> TextBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(TextBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void TextBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using TextBox textBox = new();
        textBox.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)textBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(textBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxAccessibleObject_GetPropertyValue_Value_AccessDenied_WithUseSystemPasswordChar()
    {
        using TextBox textBox = new();
        textBox.UseSystemPasswordChar = true;
        textBox.Text = "some text";

        string actual = ((BSTR)textBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId)).ToStringAndFree();

        Assert.Equal(SR.AccessDenied, actual);
        Assert.True(textBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TextBoxAccessibleObject_IsPassword_IsExpected_WithUseSystemPasswordChar(bool useSystemPasswordChar)
    {
        using TextBox textBox = new();
        textBox.UseSystemPasswordChar = useSystemPasswordChar;

        bool actual = (bool)textBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsPasswordPropertyId);

        Assert.Equal(useSystemPasswordChar, actual);
        // Handle is recreated when setting UseSystemPasswordChar
        Assert.True(textBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Placeholder text", "Placeholder text")]
    public void TextBoxAccessibleObject_GetPropertyValue_HelpText_IsExpected(string placeholderText, string expectedHelpText)
    {
        using TextBox textBox = new() { PlaceholderText = placeholderText };

        string helpText = ((BSTR)textBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HelpTextPropertyId)).ToStringAndFree();

        Assert.Equal(expectedHelpText, helpText);
    }

    [WinFormsTheory]
    [InlineData('\0')]
    [InlineData('*')]
    public void TextBoxAccessibleObject_IsPassword_IsExpected_WithPasswordChar(char passwordChar)
    {
        using TextBox textBox = new();
        textBox.PasswordChar = passwordChar;

        bool actual = (bool)textBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsPasswordPropertyId);
        bool expected = passwordChar != '\0';

        Assert.Equal(expected, actual);
        // Handle is recreated when getting PasswordChar
        Assert.True(textBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxAccessibleObject_IsPassword_IsFalse_ForMultilineTextBox_WithUseSystemPasswordChar()
    {
        using TextBox textBox = new();
        textBox.Multiline = true;
        textBox.UseSystemPasswordChar = true;

        bool actual = (bool)textBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsPasswordPropertyId);

        Assert.False(actual);
        // Handle is recreated when setting UseSystemPasswordChar
        Assert.True(textBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData('\0')]
    [InlineData('*')]
    public void TextBoxAccessibleObject_IsPassword_IsExpected_ForMultilineTextBox_WithPasswordChar(char passwordChar)
    {
        using TextBox textBox = new();
        textBox.PasswordChar = passwordChar;
        textBox.Multiline = true;

        bool actual = (bool)textBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsPasswordPropertyId);
        bool expected = passwordChar != '\0';

        Assert.Equal(expected, actual);
        // Handle is recreated when getting PasswordChar
        Assert.True(textBox.IsHandleCreated);
    }
}

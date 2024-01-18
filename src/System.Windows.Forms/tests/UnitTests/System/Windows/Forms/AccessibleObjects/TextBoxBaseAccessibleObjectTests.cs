// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class TextBoxBaseAccessibleObjectTests
{
    [WinFormsFact]
    public void TextBoxBaseAccessibleObject_ctor_default()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();

        AccessibleObject textBoxAccessibleObject = textBoxBase.AccessibilityObject;
        Assert.NotNull(textBoxAccessibleObject);

        TextBoxBase.TextBoxBaseAccessibleObject textBoxBaseAccessibleObject = new(textBoxBase);
        Assert.NotNull(textBoxBaseAccessibleObject);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsTextPatternAvailablePropertyId)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsTextPattern2AvailablePropertyId)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsValuePatternAvailablePropertyId)]
    public void TextBoxBaseAccessibleObject_PatternAvailable(int propertyId)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        AccessibleObject textBoxAccessibleObject = textBoxBase.AccessibilityObject;

        // Interop.UiaCore.UIA accessible level (internal) is less than the test level (public) so it needs boxing and unboxing
        Assert.True((bool)textBoxAccessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyId));
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_TextPatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_TextPattern2Id)]
    [InlineData((int)UIA_PATTERN_ID.UIA_ValuePatternId)]
    public void TextBoxBaseAccessibleObject_PatternSupported(int patternId)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        AccessibleObject textBoxAccessibleObject = textBoxBase.AccessibilityObject;

        // Interop.UiaCore.UIA accessible level (internal) is less than the test level (public) so it needs boxing and unboxing
        Assert.True(textBoxAccessibleObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseAccessibleObject_SetValue_OwnerTextChanged()
    {
        const string text = "some text";
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        textBoxBase.CreateControl();

        AccessibleObject accessibleObject = textBoxBase.AccessibilityObject;
        accessibleObject.SetValue(text);

        Assert.Equal(text, textBoxBase.Text);
        Assert.True(textBoxBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TextBoxBaseAccessibleObject_IsReadOnly_ReturnsCorrectValue(bool readOnly)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        AccessibleObject accessibleObject = textBoxBase.AccessibilityObject;

        textBoxBase.ReadOnly = readOnly;
        Assert.Equal(textBoxBase.ReadOnly, accessibleObject.IsReadOnly);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseAccessibleObject_Name_ReturnsNullAsDefault()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        textBoxBase.CreateControl();
        AccessibleObject accessibleObject = textBoxBase.AccessibilityObject;
        Assert.Empty(accessibleObject.Name);
        Assert.True(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseAccessibleObject_Value_EqualsText()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        textBoxBase.CreateControl();
        textBoxBase.Text = "Some test text";
        AccessibleObject accessibleObject = textBoxBase.AccessibilityObject;
        Assert.Equal(textBoxBase.Text, accessibleObject.Value);
        Assert.True(textBoxBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(50, 20)]
    [InlineData(100, 10)]
    public void TextBoxBaseAccessibleObject_BoundingRectangle_IsCorrect(int width, int height)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase { Size = new Size(width, height) };
        textBoxBase.CreateControl();
        AccessibleObject accessibleObject = textBoxBase.AccessibilityObject;
        Rectangle expected = textBoxBase.RectangleToScreen(textBoxBase.ClientRectangle);
        Rectangle actual = accessibleObject.BoundingRectangle;

        Assert.True(actual.Contains(expected));

        Assert.True(textBoxBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.Text, (int)UIA_CONTROLTYPE_ID.UIA_EditControlTypeId)]
    [InlineData(false, AccessibleRole.None, (int)UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId)]
    public void TextBoxBaseAccessibleObject_ControlType_IsExpected_IfAccessibleRoleIsDefault(bool createControl, AccessibleRole expectedRole, int expectedType)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        // AccessibleRole is not set = Default

        if (createControl)
        {
            textBoxBase.CreateControl();
        }

        AccessibleObject accessibleObject = textBoxBase.AccessibilityObject;
        var actual = (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(expectedRole, accessibleObject.Role);
        Assert.Equal((UIA_CONTROLTYPE_ID)expectedType, actual);
        Assert.Equal(createControl, textBoxBase.IsHandleCreated);
    }

    public static IEnumerable<object[]> TextBoxBaseAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(TextBoxBaseAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void TextBoxBaseAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        textBoxBase.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)textBoxBase.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    private class SubTextBoxBase : TextBoxBase
    { }
}

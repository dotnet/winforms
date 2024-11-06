// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class PictureBox_PictureBoxAccessibleObjectTests
{
    [WinFormsFact]
    public void PictureBoxAccessibleObject_Ctor_NullControl_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("ownerControl", () => new PictureBox.PictureBoxAccessibleObject(null));
    }

    [WinFormsFact]
    public void PictureBoxAccessibleObject_Ctor_Default()
    {
        using PictureBox pictureBox = new();
        Assert.False(pictureBox.IsHandleCreated);
        var pictureBoxAccessibleObject = new PictureBox.PictureBoxAccessibleObject(pictureBox);

        Assert.NotNull(pictureBoxAccessibleObject.Owner);
        Assert.False(pictureBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void PictureBoxAccessibleObject_Description_ReturnsExpected()
    {
        using PictureBox pictureBox = new()
        {
            AccessibleDescription = "TestDescription",
        };

        Assert.False(pictureBox.IsHandleCreated);
        var pictureBoxAccessibleObject = new PictureBox.PictureBoxAccessibleObject(pictureBox);

        Assert.Equal("TestDescription", pictureBoxAccessibleObject.Description);
        Assert.False(pictureBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void PictureBoxAccessibleObject_Name_ReturnsExpected()
    {
        using PictureBox pictureBox = new()
        {
            AccessibleName = "TestName"
        };

        Assert.False(pictureBox.IsHandleCreated);
        var pictureBoxAccessibleObject = new PictureBox.PictureBoxAccessibleObject(pictureBox);

        Assert.Equal("TestName", pictureBoxAccessibleObject.Name);
        Assert.False(pictureBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void PictureBoxAccessibleObject_CustomRole_ReturnsExpected()
    {
        using PictureBox pictureBox = new()
        {
            AccessibleRole = AccessibleRole.PushButton
        };

        Assert.False(pictureBox.IsHandleCreated);
        var pictureBoxAccessibleObject = new PictureBox.PictureBoxAccessibleObject(pictureBox);

        Assert.Equal(AccessibleRole.PushButton, pictureBoxAccessibleObject.Role);
        Assert.False(pictureBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.Client)]
    [InlineData(false, AccessibleRole.None)]
    public void PictureBoxAccessibleObject_DefaultRole_ReturnsExpected(bool createControl, AccessibleRole accessibleRole)
    {
        using PictureBox pictureBox = new();

        if (createControl)
        {
            pictureBox.CreateControl();
        }

        var pictureBoxAccessibleObject = new PictureBox.PictureBoxAccessibleObject(pictureBox);
        Assert.Equal(accessibleRole, pictureBoxAccessibleObject.Role);
        Assert.Equal(createControl, pictureBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_NamePropertyId, "TestName")]
    [InlineData((int)UIA_PROPERTY_ID.UIA_ControlTypePropertyId, (int)UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId)] // If AccessibleRole is Default
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId, true)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_AutomationIdPropertyId, "PictureBox1")]
    public void PictureBoxAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
    {
        using PictureBox pictureBox = new()
        {
            Name = "PictureBox1",
            AccessibleName = "TestName"
        };

        PictureBox.PictureBoxAccessibleObject pictureBoxAccessibleObject = new(pictureBox);
        using VARIANT value = pictureBoxAccessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyID);

        Assert.Equal(expected, value.ToObject());
        Assert.False(pictureBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void PictureBoxAccessibleObject_IsPatternSupported_Invoke_ReturnsTrue_ForLegacyIAccessiblePatternId()
    {
        using PictureBox pictureBox = new();
        Assert.False(pictureBox.IsHandleCreated);
        var pictureBoxAccessibleObject = new PictureBox.PictureBoxAccessibleObject(pictureBox);

        Assert.True(pictureBoxAccessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId));
        Assert.False(pictureBox.IsHandleCreated);
    }

    public static IEnumerable<object[]> PictureBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(PictureBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void PictureBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using PictureBox pictureBox = new();
        pictureBox.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)pictureBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(pictureBox.IsHandleCreated);
    }
}

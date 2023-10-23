// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.UpDownBase;
using static System.Windows.Forms.UpDownBase.UpDownButtons;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class UpDownBase_UpDownButtons_UpDownButtonsAccessibleObject_DirectionButtonAccessibleObjectTests
{
    [WinFormsTheory]
    [InlineData(((int)UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId))]
    [InlineData(((int)UIA_PROPERTY_ID.UIA_IsInvokePatternAvailablePropertyId))]
    public void UpDownButtonsAccessibleObject_DirectionButtonAccessibleObject_GetPropertyValue_IsPatternSupported(int propertyId)
    {
        using SubUpDownBase upDownBase = new();
        using UpDownButtons upDownButtons = upDownBase.UpDownButtonsInternal;
        UpDownButtonsAccessibleObject accessibleObject = new(upDownButtons);
        // UpButton has 0 index
        AccessibleObject upButton = accessibleObject.GetChild(index: 0);
        bool actual = (bool)upButton.GetPropertyValue((UIA_PROPERTY_ID)propertyId);

        Assert.True(actual);
        Assert.False(upDownButtons.IsHandleCreated);
        Assert.False(upDownBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_LegacyIAccessibleRolePropertyId, (int)AccessibleRole.PushButton)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_LegacyIAccessibleStatePropertyId, (int)AccessibleStates.None)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_ValueValuePropertyId, null)]
    public void NumericUpDownAccessibleObject_DirectionButtonAccessibleObject_GetPropertyValue_ReturnsExpected(int property, object expected)
    {
        using SubUpDownBase upDownBase = new();
        using UpDownButtons upDownButtons = upDownBase.UpDownButtonsInternal;
        UpDownButtonsAccessibleObject accessibleObject = new(upDownButtons);
        // UpButton has 0 index
        AccessibleObject upButton = accessibleObject.GetChild(index: 0);
        VARIANT actual = upButton.GetPropertyValue((UIA_PROPERTY_ID)property);
        if (expected is null)
        {
            Assert.Equal(VARIANT.Empty, actual);
        }
        else
        {
            Assert.Equal(expected, (int)actual);
        }

        Assert.False(upDownBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void UpDownButtonsAccessibleObject_DirectionButtonAccessibleObject_FragmentNavigate_Parent_ReturnsExpected(int childIndex)
    {
        using SubUpDownBase upDownBase = new();
        using UpDownButtons upDownButtons = upDownBase.UpDownButtonsInternal;
        UpDownButtonsAccessibleObject accessibleObject = new(upDownButtons);

        // UpButton has 0 index, DownButton has 1 index
        AccessibleObject directionButton = accessibleObject.GetChild(childIndex);

        Assert.Equal(accessibleObject, directionButton.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.False(upDownButtons.IsHandleCreated);
        Assert.False(upDownBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void UpDownButtonsAccessibleObject_DirectionButtonAccessibleObject_FragmentNavigate_Child_ReturnsNull(int childIndex)
    {
        using SubUpDownBase upDownBase = new();
        using UpDownButtons upDownButtons = upDownBase.UpDownButtonsInternal;
        UpDownButtonsAccessibleObject accessibleObject = new(upDownButtons);

        // UpButton has 0 index, DownButton has 1 index
        AccessibleObject directionButton = accessibleObject.GetChild(childIndex);

        Assert.Null(directionButton.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(directionButton.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.False(upDownButtons.IsHandleCreated);
        Assert.False(upDownBase.IsHandleCreated);
    }

    private class SubUpDownBase : UpDownBase
    {
        protected override void UpdateEditText() => throw new NotImplementedException();

        public override void UpButton() => throw new NotImplementedException();

        public override void DownButton() => throw new NotImplementedException();
    }
}

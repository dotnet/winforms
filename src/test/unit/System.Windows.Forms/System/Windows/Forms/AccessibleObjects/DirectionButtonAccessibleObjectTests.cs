// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class DirectionButtonAccessibleObjectTests
{
    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void DirectionButtonAccessibleObject_Invoke_DoesNothing_IfControlIsNotCreated_InDomainUpDown(int childId)
    {
        using DomainUpDown domainUpDown = new();
        domainUpDown.Items.AddRange(new string[] { "First", "Second", "Third" });
        domainUpDown.SelectedIndex = 1; // Select the second item

        // UpButton has 0 childId, DownButton has 1 childId
        AccessibleObject directionButton = domainUpDown.UpDownButtonsInternal.AccessibilityObject.GetChild(childId);
        directionButton.Invoke();

        // The selected index is not changed
        Assert.Equal(1, domainUpDown.SelectedIndex);
        Assert.False(domainUpDown.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, 0)]
    [InlineData(1, 2)]
    public void DirectionButtonAccessibleObject_Invoke_ChangesValue_InDomainUpDown(int childId, int expectedIndex)
    {
        using DomainUpDown domainUpDown = new();
        domainUpDown.Items.AddRange(new string[] { "First", "Second", "Third" });
        domainUpDown.SelectedIndex = 1; // Select the second item
        domainUpDown.CreateControl();

        // UpButton has 0 childId, DownButton has 1 childId
        AccessibleObject directionButton = domainUpDown.UpDownButtonsInternal.AccessibilityObject.GetChild(childId);
        directionButton.Invoke();

        // The selected index is not changed
        Assert.Equal(expectedIndex, domainUpDown.SelectedIndex);
        Assert.True(domainUpDown.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void DirectionButtonAccessibleObject_Invoke_DoesNothing_IfControlIsNotCreated_InNumericUpDown(int childId)
    {
        using NumericUpDown numericUpDown = new();
        int testValue = 10;
        numericUpDown.Value = testValue;

        // UpButton has 0 childId, DownButton has 1 childId
        AccessibleObject directionButton = numericUpDown.UpDownButtonsInternal.AccessibilityObject.GetChild(childId);
        directionButton.Invoke();

        // The value is not changed
        Assert.Equal(testValue, numericUpDown.Value);
        Assert.False(numericUpDown.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, 10, 11)]
    [InlineData(1, 10, 9)]
    public void DirectionButtonAccessibleObject_Invoke_ChangesValue_InNumericUpDown(int childId, int testValue, int expected)
    {
        using NumericUpDown numericUpDown = new();
        numericUpDown.Value = testValue;
        numericUpDown.CreateControl();

        // UpButton has 0 childId, DownButton has 1 childId
        AccessibleObject directionButton = numericUpDown.UpDownButtonsInternal.AccessibilityObject.GetChild(childId);
        directionButton.Invoke();

        Assert.Equal(expected, numericUpDown.Value);
        Assert.True(numericUpDown.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, 1)]
    [InlineData(1, null)]
    public void DirectionButtonAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected_InDomainUpDown(int childId, int? expectedChildId)
    {
        using DomainUpDown domainUpDown = new();

        // UpButton has 0 childId, DownButton has 1 childId
        AccessibleObject directionButton = domainUpDown.UpDownButtonsInternal.AccessibilityObject.GetChild(childId);

        if (expectedChildId is null)
        {
            Assert.Null(directionButton.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        }
        else
        {
            AccessibleObject expected = domainUpDown.UpDownButtonsInternal.AccessibilityObject.GetChild(expectedChildId.Value);
            Assert.Equal(expected, directionButton.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        }

        Assert.False(domainUpDown.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, 1)]
    [InlineData(1, null)]
    public void DirectionButtonAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected_InNumericUpDown(int childId, int? expectedChildId)
    {
        using NumericUpDown numericUpDown = new();

        // UpButton has 0 childId, DownButton has 1 childId
        AccessibleObject directionButton = numericUpDown.UpDownButtonsInternal.AccessibilityObject.GetChild(childId);

        if (expectedChildId is null)
        {
            Assert.Null(directionButton.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        }
        else
        {
            AccessibleObject expected = numericUpDown.UpDownButtonsInternal.AccessibilityObject.GetChild(expectedChildId.Value);
            Assert.Equal(expected, directionButton.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        }

        Assert.False(numericUpDown.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, null)]
    [InlineData(1, 0)]
    public void DirectionButtonAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected_InDomainUpDown(int childId, int? expectedChildId)
    {
        using DomainUpDown domainUpDown = new();

        // UpButton has 0 childId, DownButton has 1 childId
        AccessibleObject directionButton = domainUpDown.UpDownButtonsInternal.AccessibilityObject.GetChild(childId);

        if (expectedChildId is null)
        {
            Assert.Null(directionButton.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        }
        else
        {
            AccessibleObject expected = domainUpDown.UpDownButtonsInternal.AccessibilityObject.GetChild(expectedChildId.Value);
            Assert.Equal(expected, directionButton.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        }

        Assert.False(domainUpDown.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, null)]
    [InlineData(1, 0)]
    public void DirectionButtonAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected_InNumericUpDown(int childId, int? expectedChildId)
    {
        using NumericUpDown numericUpDown = new();

        // UpButton has 0 childId, DownButton has 1 childId
        AccessibleObject directionButton = numericUpDown.UpDownButtonsInternal.AccessibilityObject.GetChild(childId);

        if (expectedChildId is null)
        {
            Assert.Null(directionButton.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        }
        else
        {
            AccessibleObject expected = numericUpDown.UpDownButtonsInternal.AccessibilityObject.GetChild(expectedChildId.Value);
            Assert.Equal(expected, directionButton.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        }

        Assert.False(numericUpDown.IsHandleCreated);
    }
}

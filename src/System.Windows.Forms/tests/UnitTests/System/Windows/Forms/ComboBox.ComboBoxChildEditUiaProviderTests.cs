﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms.Tests;

public class ComboBox_ComboBoxChildEditUiaProviderTests
{
    public static IEnumerable<object[]> ComboBoxChildEditUiaProvider_FragmentNavigate_PreviousSibling_ReturnsExpected_TestData()
    {
        foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
        {
            // A Combobox with ComboBoxStyle.DropDownList style does not support a ComboBoxChildEditUiaProvider
            if (comboBoxStyle == ComboBoxStyle.DropDownList)
            {
                continue;
            }

            foreach (bool createControl in new[] { true, false })
            {
                foreach (bool droppedDown in new[] { true, false })
                {
                    bool childListDisplayed = droppedDown || comboBoxStyle == ComboBoxStyle.Simple;
                    yield return new object[] { comboBoxStyle, createControl, droppedDown, childListDisplayed };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ComboBoxChildEditUiaProvider_FragmentNavigate_PreviousSibling_ReturnsExpected_TestData))]
    public void ComboBoxChildEditUiaProvider_FragmentNavigate_PreviousSibling_ReturnsExpected(
        ComboBoxStyle comboBoxStyle,
        bool createControl,
        bool droppedDown,
        bool childListDisplayed)
    {
        using ComboBox comboBox = new ComboBox
        {
            DropDownStyle = comboBoxStyle
        };

        if (createControl)
        {
            comboBox.CreateControl();
        }

        comboBox.DroppedDown = droppedDown;
        AccessibleObject previousItem = comboBox.ChildEditAccessibleObject
            .FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling) as AccessibleObject;

        Assert.Equal(!childListDisplayed, previousItem is null);
        Assert.Equal(childListDisplayed, previousItem == comboBox.ChildListAccessibleObject);
        Assert.True(comboBox.IsHandleCreated);
    }

    public static IEnumerable<object[]> ComboBoxChildAccessibleObject_FragmentNavigate_TestData()
    {
        foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
        {
            // A Combobox with ComboBoxStyle.DropDownList style does not support a ComboBoxChildEditUiaProvider
            if (comboBoxStyle == ComboBoxStyle.DropDownList)
            {
                continue;
            }

            foreach (bool createControl in new[] { true, false })
            {
                foreach (bool droppedDown in new[] { true, false })
                {
                    yield return new object[] { comboBoxStyle, createControl, droppedDown };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ComboBoxChildAccessibleObject_FragmentNavigate_TestData))]
    public void ComboBoxChildEditUiaProvider_FragmentNavigate_NextSibling_ReturnsExpected(
        ComboBoxStyle comboBoxStyle,
        bool createControl,
        bool droppedDown)
    {
        using ComboBox comboBox = new ComboBox
        {
            DropDownStyle = comboBoxStyle
        };

        if (createControl)
        {
            comboBox.CreateControl();
        }

        comboBox.DroppedDown = droppedDown;
        AccessibleObject nextItem = comboBox.ChildEditAccessibleObject
            .FragmentNavigate(UiaCore.NavigateDirection.NextSibling) as AccessibleObject;

        AccessibleObject expectedItem = comboBoxStyle != ComboBoxStyle.Simple
            ? ((ComboBox.ComboBoxAccessibleObject)comboBox.AccessibilityObject).DropDownButtonUiaProvider
            : null;

        Assert.Equal(expectedItem, nextItem);
        Assert.True(comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.Simple)]
    public void ComboBoxChildEditUiaProvider_SupportsTextPattern(ComboBoxStyle comboBoxStyle)
    {
        using ComboBox comboBox = new ComboBox
        {
            DropDownStyle = comboBoxStyle
        };

        comboBox.CreateControl();
        AccessibleObject accessibleObject = comboBox.ChildEditAccessibleObject;

        Assert.True((bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsTextPatternAvailablePropertyId));
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.Simple)]
    public void ComboBoxChildEditUiaProvider_SupportsTextPattern2(ComboBoxStyle comboBoxStyle)
    {
        using ComboBox comboBox = new ComboBox
        {
            DropDownStyle = comboBoxStyle
        };

        comboBox.CreateControl();
        AccessibleObject accessibleObject = comboBox.ChildEditAccessibleObject;

        Assert.True((bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsTextPattern2AvailablePropertyId));
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.Simple)]
    public void ComboBoxChildEditUiaProvider_SupportsValuePattern(ComboBoxStyle comboBoxStyle)
    {
        using ComboBox comboBox = new ComboBox
        {
            DropDownStyle = comboBoxStyle
        };

        comboBox.CreateControl();
        AccessibleObject accessibleObject = comboBox.ChildEditAccessibleObject;

        Assert.True((bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsValuePatternAvailablePropertyId));
    }

    [WinFormsFact]
    public void ComboBoxChildEditUiaProvider_GetPropertyValue_ReturnsExpected()
    {
        using ComboBox comboBox = new ComboBox();
        comboBox.CreateControl();
        AccessibleObject accessibleObject = comboBox.ChildEditAccessibleObject;

        Assert.Equal(SR.ComboBoxEditDefaultAccessibleName, accessibleObject.GetPropertyValue(UiaCore.UIA.NamePropertyId).ToString());
        Assert.Equal(SR.ComboBoxEditDefaultAccessibleName, accessibleObject.GetPropertyValue(UiaCore.UIA.LegacyIAccessibleNamePropertyId).ToString());
        Assert.Null(accessibleObject.GetPropertyValue(UiaCore.UIA.ValueValuePropertyId));

        comboBox.AccessibleName = "Combo AO name";

        Assert.Equal(comboBox.AccessibleName, accessibleObject.GetPropertyValue(UiaCore.UIA.NamePropertyId).ToString());
        Assert.Equal(comboBox.AccessibleName, accessibleObject.GetPropertyValue(UiaCore.UIA.LegacyIAccessibleNamePropertyId).ToString());
        Assert.Null(accessibleObject.GetPropertyValue(UiaCore.UIA.ValueValuePropertyId));
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests;

public class ComboBoxChildTextUiaProviderTests
{
    public static IEnumerable<object[]> ComboBoxChildTextUiaProvider_FragmentNavigate__TestData()
    {
        foreach (bool createControl in new[] { true, false })
        {
            foreach (bool droppedDown in new[] { true, false })
            {
                yield return new object[] { createControl, droppedDown };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ComboBoxChildTextUiaProvider_FragmentNavigate__TestData))]
    public void ComboBoxChildTextUiaProvider_FragmentNavigate_PreviousSibling_ReturnsExpected(
        bool createControl,
        bool droppedDown)
    {
        using ComboBox comboBox = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        if (createControl)
        {
            comboBox.CreateControl();
        }

        comboBox.DroppedDown = droppedDown;
        AccessibleObject previousItem = comboBox.ChildTextAccessibleObject
            .FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling) as AccessibleObject;

        Assert.Equal(!droppedDown, previousItem is null);
        Assert.Equal(droppedDown, previousItem == comboBox.ChildListAccessibleObject);
        Assert.True(comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ComboBoxChildTextUiaProvider_FragmentNavigate__TestData))]
    public void ComboBoxChildTextUiaProvider_FragmentNavigate_NextSibling_ReturnsExpected(
        bool createControl,
        bool droppedDown)
    {
        using ComboBox comboBox = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        if (createControl)
        {
            comboBox.CreateControl();
        }

        comboBox.DroppedDown = droppedDown;
        AccessibleObject nextItem = comboBox.ChildTextAccessibleObject
            .FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling) as AccessibleObject;

        AccessibleObject expectedItem = ((ComboBox.ComboBoxAccessibleObject)comboBox.AccessibilityObject).DropDownButtonUiaProvider;

        Assert.Equal(expectedItem, nextItem);
        Assert.True(comboBox.IsHandleCreated);
    }
}

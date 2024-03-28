// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests;

public class ComboBox_ComboBoxChildListUiaProviderTests
{
    public static IEnumerable<object[]> ChildListAccessibleObject_FragmentNavigate_TestData()
    {
        foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
        {
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
    [MemberData(nameof(ChildListAccessibleObject_FragmentNavigate_TestData))]
    public void ChildListAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected(
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
        AccessibleObject previousItem = comboBox.ChildListAccessibleObject
            .FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling) as AccessibleObject;

        AccessibleObject expectedItem = comboBoxStyle == ComboBoxStyle.Simple
            ? comboBox.ChildListAccessibleObject
            : null;

        Assert.Equal(expectedItem, previousItem);
        Assert.True(comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ChildListAccessibleObject_FragmentNavigate_TestData))]
    public void ChildListAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected(
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
        AccessibleObject nextItem = comboBox.ChildListAccessibleObject
            .FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling) as AccessibleObject;

        AccessibleObject expectedItem = comboBoxStyle == ComboBoxStyle.DropDownList
            ? comboBox.ChildTextAccessibleObject
            : comboBox.ChildEditAccessibleObject;

        Assert.Equal(expectedItem, nextItem);
        Assert.True(comboBox.IsHandleCreated);
    }

    public static IEnumerable<object[]> ChildListAccessibleObject_BoundingRectangle_ReturnsCorrectWidth_IfComboBoxIsScrollable_TestData()
    {
        foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
        {
            yield return new object[] { comboBoxStyle };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ChildListAccessibleObject_BoundingRectangle_ReturnsCorrectWidth_IfComboBoxIsScrollable_TestData))]
    public void ChildListAccessibleObject_BoundingRectangle_ReturnsCorrectWidth_IfComboBoxIsScrollable(ComboBoxStyle comboBoxStyle)
    {
        const int expectedWidth = 100;

        using ComboBox comboBox = new()
        {
            DropDownStyle = comboBoxStyle,
            IntegralHeight = false
        };
        comboBox.Items.AddRange(Enumerable.Range(0, 11).Cast<object>().ToArray());
        comboBox.CreateControl();

        if (comboBoxStyle == ComboBoxStyle.Simple)
        {
            comboBox.Size = new Size(expectedWidth, 150);
        }
        else
        {
            comboBox.Size = new Size(expectedWidth, comboBox.Size.Height);
            comboBox.DropDownHeight = 120;
            comboBox.DroppedDown = true;
        }

        IRawElementProviderFragment.Interface childListUiaProvider = comboBox.ChildListAccessibleObject;
        Assert.True(childListUiaProvider.get_BoundingRectangle(out UiaRect actual).Succeeded);

        Assert.Equal(expectedWidth, actual.width);
    }
}

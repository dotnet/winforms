// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class PropertyGridToolStripButton_PropertyGridToolStripButtonAccessibleObjectTests
{
    [WinFormsFact]
    public void PropertyGridToolStripButtonAccessibleObject_IsItemSelected_ReturnsExpected()
    {
        using PropertyGrid propertyGrid = new();
        propertyGrid.CreateControl();
        ToolStripButton[] toolStripButtons = propertyGrid.TestAccessor().Dynamic._viewSortButtons;
        AccessibleObject categoryButtonAccessibleObject = toolStripButtons[0].AccessibilityObject;
        AccessibleObject alphaButtonAccessibleObject = toolStripButtons[1].AccessibilityObject;

        Assert.True(categoryButtonAccessibleObject.IsItemSelected);
        Assert.False(alphaButtonAccessibleObject.IsItemSelected);

        alphaButtonAccessibleObject.SelectItem();

        Assert.False(categoryButtonAccessibleObject.IsItemSelected);
        Assert.True(alphaButtonAccessibleObject.IsItemSelected);
    }

    [WinFormsFact]
    public void PropertyGridToolStripButtonAccessibleObject_Role_IsRadiButton()
    {
        using PropertyGrid propertyGrid = new();
        ToolStripButton[] toolStripButtons = propertyGrid.TestAccessor().Dynamic._viewSortButtons;
        AccessibleObject accessibleObject = toolStripButtons[0].AccessibilityObject;

        Assert.Equal(AccessibleRole.RadioButton, accessibleObject.Role);
    }

    [WinFormsFact]
    public void PropertyGridToolStripButtonAccessibleObject_SelectionItemPatternSupported_ReturnsExpected()
    {
        using PropertyGrid propertyGrid = new();
        ToolStripButton[] toolStripButtons = propertyGrid.TestAccessor().Dynamic._viewSortButtons;
        ToolStripButton propertyPagesButton = propertyGrid.TestAccessor().Dynamic._viewPropertyPagesButton;
        AccessibleObject categoryButtonAccessibleObject = toolStripButtons[0].AccessibilityObject;
        AccessibleObject alphaButtonAccessibleObject = toolStripButtons[1].AccessibilityObject;
        AccessibleObject propertyPagesButtonAccessibleObject = propertyPagesButton.AccessibilityObject;

        Assert.True(categoryButtonAccessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_SelectionItemPatternId));
        Assert.True(alphaButtonAccessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_SelectionItemPatternId));

        // In accordance with the behavior of the PropertyGrid, the "Property Page" button does not have
        // the role of a "RadioButton" and therefore does not support the "SelectionItem" pattern
        Assert.False(propertyPagesButtonAccessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_SelectionItemPatternId));
    }

    [WinFormsFact]
    public void PropertyGridToolStripButtonAccessibleObject_GetPropertyValue_ControlType_IsRadioButton()
    {
        using PropertyGrid propertyGrid = new();
        ToolStripButton[] toolStripButtons = propertyGrid.TestAccessor().Dynamic._viewSortButtons;
        AccessibleObject accessibleObject = toolStripButtons[0].AccessibilityObject;

        UIA_CONTROLTYPE_ID actual = (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_RadioButtonControlTypeId, actual);
    }

    [WinFormsFact]
    public void PropertyGridToolStripButtonAccessibleObject_AddToSelection_UpdatesCheckedStateAndPropertySort()
    {
        using PropertyGrid propertyGrid = new();
        propertyGrid.CreateControl();
        ToolStripButton[] toolStripButtons = propertyGrid.TestAccessor().Dynamic._viewSortButtons;
        ToolStripButton categoryButton = toolStripButtons[0];
        ToolStripButton alphaButton = toolStripButtons[1];
        AccessibleObject alphaButtonAccessibleObject = alphaButton.AccessibilityObject;
        AccessibleObject categoryButtonAccessibleObject = categoryButton.AccessibilityObject;

        Assert.True(categoryButton.Checked);
        Assert.False(alphaButton.Checked);
        Assert.Equal(PropertySort.CategorizedAlphabetical, propertyGrid.PropertySort);

        alphaButtonAccessibleObject.AddToSelection();

        Assert.False(categoryButton.Checked);
        Assert.True(alphaButton.Checked);
        Assert.Equal(PropertySort.Alphabetical, propertyGrid.PropertySort);

        categoryButtonAccessibleObject.AddToSelection();

        Assert.True(categoryButton.Checked);
        Assert.False(alphaButton.Checked);
        Assert.Equal(PropertySort.CategorizedAlphabetical, propertyGrid.PropertySort);
    }

    [WinFormsFact]
    public void PropertyGridToolStripButtonAccessibleObject_Invoke_UpdatesCheckedStateAndPropertySort()
    {
        using PropertyGrid propertyGrid = new();
        propertyGrid.CreateControl();
        ToolStripButton[] toolStripButtons = propertyGrid.TestAccessor().Dynamic._viewSortButtons;
        ToolStripButton categoryButton = toolStripButtons[0];
        ToolStripButton alphaButton = toolStripButtons[1];
        AccessibleObject alphaButtonAccessibleObject = alphaButton.AccessibilityObject;
        AccessibleObject categoryButtonAccessibleObject = categoryButton.AccessibilityObject;

        Assert.True(categoryButton.Checked);
        Assert.False(alphaButton.Checked);
        Assert.Equal(PropertySort.CategorizedAlphabetical, propertyGrid.PropertySort);

        alphaButtonAccessibleObject.Invoke();

        Assert.False(categoryButton.Checked);
        Assert.True(alphaButton.Checked);
        Assert.Equal(PropertySort.Alphabetical, propertyGrid.PropertySort);

        categoryButtonAccessibleObject.Invoke();

        Assert.True(categoryButton.Checked);
        Assert.False(alphaButton.Checked);
        Assert.Equal(PropertySort.CategorizedAlphabetical, propertyGrid.PropertySort);
    }

    [WinFormsFact]
    public void PropertyGridToolStripButtonAccessibleObject_RemoveFromSelection_DoesNotUpdateCheckedStateAndPropertySort()
    {
        using PropertyGrid propertyGrid = new();
        propertyGrid.CreateControl();
        ToolStripButton[] toolStripButtons = propertyGrid.TestAccessor().Dynamic._viewSortButtons;
        ToolStripButton categoryButton = toolStripButtons[0];
        ToolStripButton alphaButton = toolStripButtons[1];
        AccessibleObject alphaButtonAccessibleObject = alphaButton.AccessibilityObject;
        AccessibleObject categoryButtonAccessibleObject = categoryButton.AccessibilityObject;

        Assert.True(categoryButton.Checked);
        Assert.False(alphaButton.Checked);
        Assert.Equal(PropertySort.CategorizedAlphabetical, propertyGrid.PropertySort);

        alphaButtonAccessibleObject.RemoveFromSelection();

        Assert.True(categoryButton.Checked);
        Assert.False(alphaButton.Checked);
        Assert.Equal(PropertySort.CategorizedAlphabetical, propertyGrid.PropertySort);
    }

    [WinFormsFact]
    public void PropertyGridToolStripButtonAccessibleObject_SelectItem_UpdatesCheckedStateAndPropertySort()
    {
        using PropertyGrid propertyGrid = new();
        propertyGrid.CreateControl();
        ToolStripButton[] toolStripButtons = propertyGrid.TestAccessor().Dynamic._viewSortButtons;
        ToolStripButton categoryButton = toolStripButtons[0];
        ToolStripButton alphaButton = toolStripButtons[1];
        AccessibleObject alphaButtonAccessibleObject = alphaButton.AccessibilityObject;
        AccessibleObject categoryButtonAccessibleObject = categoryButton.AccessibilityObject;

        Assert.True(categoryButton.Checked);
        Assert.False(alphaButton.Checked);
        Assert.Equal(PropertySort.CategorizedAlphabetical, propertyGrid.PropertySort);

        alphaButtonAccessibleObject.SelectItem();

        Assert.False(categoryButton.Checked);
        Assert.True(alphaButton.Checked);
        Assert.Equal(PropertySort.Alphabetical, propertyGrid.PropertySort);

        categoryButtonAccessibleObject.SelectItem();

        Assert.True(categoryButton.Checked);
        Assert.False(alphaButton.Checked);
        Assert.Equal(PropertySort.CategorizedAlphabetical, propertyGrid.PropertySort);
    }

    [WinFormsFact]
    public void PropertyGridToolStripButtonAccessibleObject_IsTogglePatternSupported_ReturnsExpected()
    {
        using PropertyGrid propertyGrid = new();
        ToolStripButton[] toolStripButtons = propertyGrid.TestAccessor().Dynamic._viewSortButtons;
        ToolStripButton propertyPagesButton = propertyGrid.TestAccessor().Dynamic._viewPropertyPagesButton;
        AccessibleObject categoryButtonAccessibleObject = toolStripButtons[0].AccessibilityObject;
        AccessibleObject alphaButtonAccessibleObject = toolStripButtons[1].AccessibilityObject;
        AccessibleObject propertyPagesButtonAccessibleObject = propertyPagesButton.AccessibilityObject;

        Assert.False(categoryButtonAccessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_TogglePatternId));
        Assert.False(alphaButtonAccessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_TogglePatternId));
        Assert.False(propertyPagesButtonAccessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_TogglePatternId));
    }
}

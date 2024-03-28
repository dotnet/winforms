// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.PropertyGridInternal;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.PropertyGridInternal.CategoryGridEntry;
using static System.Windows.Forms.PropertyGridInternal.PropertyGridView;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class CategoryGridEntryAccessibleObjectTests
{
    [WinFormsFact]
    public void CategoryGridEntryAccessibleObject_Ctor_Default()
    {
        using NoAssertContext context = new();
        CategoryGridEntryAccessibleObject accessibilityObject = new(null);

        Assert.Null(((IOwnedObject<GridEntry>)accessibilityObject).Owner);
    }

    [WinFormsFact]
    public void CategoryGridEntryAccessibleObject_Role_ReturnsExpected()
    {
        using NoAssertContext context = new();
        CategoryGridEntryAccessibleObject accessibilityObject = new(null);

        Assert.Equal(AccessibleRole.ButtonDropDownGrid, accessibilityObject.Role);
    }

    [WinFormsFact]
    public void CategoryGridEntryAccessibleObject_Column_ReturnsExpected()
    {
        using NoAssertContext context = new();
        CategoryGridEntryAccessibleObject accessibilityObject = new(null);

        Assert.Equal(0, accessibilityObject.Column);
    }

    [WinFormsFact]
    public void CategoryGridEntryAccessibleObject_ControlType_ReturnsExpected()
    {
        using NoAssertContext context = new();
        CategoryGridEntryAccessibleObject accessibilityObject = new(null);

        UIA_CONTROLTYPE_ID expected = UIA_CONTROLTYPE_ID.UIA_TreeItemControlTypeId;

        Assert.Equal(expected, (UIA_CONTROLTYPE_ID)(int)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_GridItemPatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_TableItemPatternId)]
    public void CategoryGridEntryAccessibleObject_IsPatternSupported_ReturnsExpected(int patternId)
    {
        using NoAssertContext context = new();
        CategoryGridEntryAccessibleObject accessibilityObject = new(null);

        Assert.True(accessibilityObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
    }

    [WinFormsFact]
    public void CategoryGridEntryAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using PropertyGrid control = new();
        using Button button = new();
        control.SelectedObject = button;
        PropertyGridView gridView = control.TestAccessor().Dynamic._gridView;

        AccessibleObject accessibilityObject = gridView.TopLevelGridEntries[0].AccessibilityObject;

        Assert.Equal(gridView.AccessibilityObject, accessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.False(control.IsHandleCreated);
        Assert.False(button.IsHandleCreated);
    }

    [WinFormsFact]
    public void CategoryGridEntryAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected()
    {
        using PropertyGrid control = new();
        using Button button = new();
        control.SelectedObject = button;
        PropertyGridView gridView = control.TestAccessor().Dynamic._gridView;

        AccessibleObject accessibilityObjectCategory1 = gridView.TopLevelGridEntries[0].AccessibilityObject;
        AccessibleObject accessibilityObjectCategory2 = gridView.TopLevelGridEntries[1].AccessibilityObject;
        AccessibleObject accessibilityObjectLastCategory = gridView.TopLevelGridEntries[^1].AccessibilityObject;

        Assert.Equal(accessibilityObjectCategory2, accessibilityObjectCategory1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibilityObjectLastCategory.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.False(control.IsHandleCreated);
        Assert.False(button.IsHandleCreated);
    }

    [WinFormsFact]
    public void CategoryGridEntryAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected()
    {
        using PropertyGrid control = new();
        using Button button = new();
        control.SelectedObject = button;
        PropertyGridView gridView = control.TestAccessor().Dynamic._gridView;

        AccessibleObject accessibilityObjectCategory1 = gridView.TopLevelGridEntries[0].AccessibilityObject;
        AccessibleObject accessibilityObjectCategory2 = gridView.TopLevelGridEntries[1].AccessibilityObject;

        Assert.Null(accessibilityObjectCategory1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibilityObjectCategory1, accessibilityObjectCategory2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
        Assert.False(button.IsHandleCreated);
    }

    [WinFormsFact]
    public void CategoryGridEntryAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected()
    {
        using PropertyGrid control = new();
        using Button button = new();
        control.SelectedObject = button;
        PropertyGridView gridView = control.TestAccessor().Dynamic._gridView;
        var category = (CategoryGridEntry)gridView.TopLevelGridEntries[0];
        var gridViewAccessibilityObject = (PropertyGridViewAccessibleObject)gridView.AccessibilityObject;

        AccessibleObject accessibilityObject = category.AccessibilityObject;
        AccessibleObject accessibilityObjectFirstItem = gridViewAccessibilityObject.GetFirstChildProperty(category);

        Assert.Equal(accessibilityObjectFirstItem, accessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.False(control.IsHandleCreated);
        Assert.False(button.IsHandleCreated);
    }

    [WinFormsFact]
    public void CategoryGridEntryAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected()
    {
        using PropertyGrid control = new();
        using Button button = new();
        control.SelectedObject = button;
        PropertyGridView gridView = control.TestAccessor().Dynamic._gridView;
        var category = (CategoryGridEntry)gridView.TopLevelGridEntries[0];
        var gridViewAccessibilityObject = (PropertyGridViewAccessibleObject)gridView.AccessibilityObject;

        AccessibleObject accessibilityObject = category.AccessibilityObject;
        AccessibleObject accessibilityObjectLastItem = gridViewAccessibilityObject.GetLastChildProperty(category);

        Assert.Equal(accessibilityObjectLastItem, accessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(control.IsHandleCreated);
        Assert.False(button.IsHandleCreated);
    }

    [WinFormsFact]
    public void CategoryGridEntryAccessibleObject_LocalizedControlType_ReturnsExpected()
    {
        using NoAssertContext context = new();
        CategoryGridEntryAccessibleObject accessibilityObject = new(null);

        string expected = SR.CategoryPropertyGridLocalizedControlType;

        Assert.Equal(expected, ((BSTR)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LocalizedControlTypePropertyId)).ToStringAndFree());
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_LegacyIAccessibleRolePropertyId, (int)AccessibleRole.ButtonDropDownGrid)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsGridItemPatternAvailablePropertyId, true)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsTableItemPatternAvailablePropertyId, true)]
    public void CategoryGridEntryAccessibleObject_GetPropertyValue_ReturnsExpected(int property, object expected)
    {
        using NoAssertContext context = new();
        CategoryGridEntryAccessibleObject accessibleObject = new(null);
        VARIANT actual = accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)property);

        Assert.Equal(expected, actual.ToObject());
    }

    [WinFormsFact]
    public void CategoryGridEntryAccessibleObjectWithControl_GetPropertyValue_ReturnsExpected()
    {
        using PropertyGrid control = new();
        using Button button = new();
        control.SelectedObject = button;
        PropertyGridView gridView = control.TestAccessor().Dynamic._gridView;
        var category = (CategoryGridEntry)gridView.TopLevelGridEntries[0];
        var gridViewAccessibilityObject = (PropertyGridViewAccessibleObject)gridView.AccessibilityObject;
        AccessibleObject accessibilityObject = category.AccessibilityObject;

        Assert.Equal("Collapse", ((BSTR)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId)).ToStringAndFree());
        Assert.Equal(VARIANT.Empty, accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId));
    }
}

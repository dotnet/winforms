// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.Control;

namespace System.Windows.Forms.PropertyGridInternal.Tests.AccessibleObjects;

public class PropertyGridView_GridViewTextBox_GridViewTextBoxAccessibleObjectTests
{
    [WinFormsFact]
    public void GridViewTextBoxAccessibleObject_created_for_string_property()
    {
        TestEntityWithTextField testEntity = new TestEntityWithTextField
        {
            TextProperty = "Test"
        };

        using PropertyGrid propertyGrid = new PropertyGrid
        {
            SelectedObject = testEntity
        };

        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
        int firstPropertyIndex = 1; // Index 0 corresponds to the category grid entry.
        PropertyDescriptorGridEntry gridEntry = (PropertyDescriptorGridEntry)propertyGridView.AccessibilityGetGridEntries()[firstPropertyIndex];
        PropertyDescriptorGridEntry selectedGridEntry = propertyGridView.TestAccessor().Dynamic._selectedGridEntry;

        Assert.Equal(gridEntry.PropertyName, selectedGridEntry.PropertyName);
        // Force the entry edit control Handle creation.
        // GridViewEditAccessibleObject exists, if its control is already created.
        // In UI case an entry edit control is created when an PropertyGridView gets focus.
        Assert.NotEqual(IntPtr.Zero, propertyGridView.TestAccessor().Dynamic.EditTextBox.Handle);

        AccessibleObject selectedGridEntryAccessibleObject = gridEntry.AccessibilityObject;
        IRawElementProviderFragment.Interface editFieldAccessibleObject = selectedGridEntryAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild);
        Assert.NotNull(editFieldAccessibleObject);

        Assert.Equal("GridViewTextBoxAccessibleObject", editFieldAccessibleObject.GetType().Name);
    }

    [WinFormsFact]
    public unsafe void GridViewTextBoxAccessibleObject_FragmentNavigate_navigates_correctly()
    {
        using PropertyGrid propertyGrid = new()
        {
            SelectedObject = Point.Empty
        };

        propertyGrid.CreateControl();
        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;

        int firstPropertyIndex = 2; // Index of Text property which has a RichEdit control as an editor.
        PropertyDescriptorGridEntry gridEntry = (PropertyDescriptorGridEntry)propertyGridView.AccessibilityGetGridEntries()[firstPropertyIndex];

        propertyGridView.TestAccessor().Dynamic._selectedGridEntry = gridEntry;

        // Force the entry edit control Handle creation.
        // GridViewEditAccessibleObject exists, if its control is already created.
        // In UI case an entry edit control is created when an PropertyGridView gets focus.
        Assert.NotEqual(IntPtr.Zero, propertyGridView.TestAccessor().Dynamic.EditTextBox.Handle);

        IRawElementProviderFragment.Interface editFieldAccessibleObject = gridEntry.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild);
        Assert.Equal("GridViewTextBoxAccessibleObject", editFieldAccessibleObject.GetType().Name);

        // The case with drop down holder:
        using PropertyGridView.DropDownHolder dropDownHolder = new(propertyGridView);
        dropDownHolder.CreateControl();
        propertyGridView.TestAccessor().Dynamic._dropDownHolder = dropDownHolder;

        dropDownHolder.TestAccessor().Dynamic.SetState(0x00000002, true); // Control class States.Visible flag
        IRawElementProviderFragment.Interface dropDownHolderAccessibleObject = gridEntry.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild);

        Assert.Equal("DropDownHolderAccessibleObject", dropDownHolderAccessibleObject.GetType().Name);
        Assert.True(propertyGridView.DropDownVisible);
        using ComScope<IRawElementProviderFragment> previousAccessibleObject = new(null);
        Assert.True(editFieldAccessibleObject.Navigate(NavigateDirection.NavigateDirection_PreviousSibling, previousAccessibleObject).Succeeded);
        Assert.False(previousAccessibleObject.IsNull);
        Assert.Same(dropDownHolder.AccessibilityObject, ComHelpers.GetObjectForIUnknown(previousAccessibleObject));
    }

    public class TestEntityWithTextField
    {
        public string TextProperty { get; set; }
    }

    [WinFormsFact]
    public void GridViewTextBoxAccessibleObject_ctor_default()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
        Type gridViewTextBoxType = typeof(PropertyGridView).GetNestedType("GridViewTextBox", BindingFlags.NonPublic);
        Assert.NotNull(gridViewTextBoxType);
        TextBox gridViewTextBox = (TextBox)Activator.CreateInstance(gridViewTextBoxType, gridView);
        Type accessibleObjectType = gridViewTextBoxType.GetNestedType("GridViewTextBoxAccessibleObject", BindingFlags.NonPublic);
        Assert.NotNull(accessibleObjectType);
        ControlAccessibleObject accessibleObject = (ControlAccessibleObject)Activator.CreateInstance(accessibleObjectType, gridViewTextBox);
        Assert.Equal(gridViewTextBox, accessibleObject.Owner);
    }

    [WinFormsFact]
    public void GridViewTextBoxAccessibleObject_ctor_ThrowsException_IfOwnerIsNull()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
        Type gridViewTextBoxType = typeof(PropertyGridView).GetNestedType("GridViewTextBox", BindingFlags.NonPublic);
        Assert.NotNull(gridViewTextBoxType);
        TextBox gridViewTextBox = (TextBox)Activator.CreateInstance(gridViewTextBoxType, gridView);
        Type accessibleObjectType = gridViewTextBoxType.GetNestedType("GridViewTextBoxAccessibleObject", BindingFlags.NonPublic);
        Assert.NotNull(accessibleObjectType);
        Assert.Throws<TargetInvocationException>(() => Activator.CreateInstance(accessibleObjectType, (TextBox)null));
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsTextPatternAvailablePropertyId)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsTextPattern2AvailablePropertyId)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsValuePatternAvailablePropertyId)]
    public void GridViewTextBoxAccessibleObject_GetPropertyValue_PatternsSuported(int propertyID)
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
        AccessibleObject accessibleObject = gridView.EditAccessibleObject;
        Assert.True((bool)accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyID));
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_ValuePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_TextPatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_TextPattern2Id)]
    public void GridViewTextBoxAccessibleObject_IsPatternSupported_PatternsSuported(int patternId)
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
        AccessibleObject accessibleObject = gridView.EditAccessibleObject;
        Assert.True(accessibleObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
    }

    [WinFormsFact]
    public void GridViewTextBoxAccessibleObject_ControlType_IsEdit_IfAccessibleRoleIsDefault()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
        AccessibleObject accessibleObject = gridView.EditAccessibleObject;

        // AccessibleRole is not set = Default

        VARIANT actual = accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_EditControlTypeId, (UIA_CONTROLTYPE_ID)(int)actual);
        Assert.False(propertyGrid.IsHandleCreated);
    }

    [WinFormsFact]
    public void GridViewTextBoxAccessibleObject_GetPropertyValue_FrameworkIdPropertyId_ReturnsExpected()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
        AccessibleObject accessibleObject = gridView.EditAccessibleObject;

        Assert.Equal("WinForm", ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_FrameworkIdPropertyId)).ToStringAndFree());
        Assert.False(propertyGrid.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.Text)]
    [InlineData(false, AccessibleRole.None)]
    public void GridViewTextBoxAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView gridView = propertyGrid.TestAccessor().GridView;

        // AccessibleRole is not set = Default

        if (createControl)
        {
            gridView.TestAccessor().Dynamic.EditTextBox.CreateControl(true); // "true" means ignoring Visible value
        }

        AccessibleRole actual = gridView.EditAccessibleObject.Role;

        Assert.Equal(expectedRole, actual);
        Assert.False(propertyGrid.IsHandleCreated);
    }

    [WinFormsFact]
    public void GridViewTextBoxAccessibleObject_RuntimeId_ReturnsNull()
    {
        using PropertyGrid propertyGrid = new() { SelectedObject = new TestEntityWithTextField() { TextProperty = "Test" } };

        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
        int firstPropertyIndex = 1; // Index 0 corresponds to the category grid entry.
        PropertyDescriptorGridEntry gridEntry = (PropertyDescriptorGridEntry)propertyGridView.AccessibilityGetGridEntries()[firstPropertyIndex];

        // Force the entry edit control Handle creation.
        // GridViewEditAccessibleObject exists, if its control is already created.
        // In UI case an entry edit control is created when an PropertyGridView gets focus.
        Assert.NotEqual(IntPtr.Zero, propertyGridView.TestAccessor().Dynamic.EditTextBox.Handle);

        AccessibleObject editFieldAccessibleObject = (AccessibleObject)gridEntry.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild);
        propertyGridView.TestAccessor().Dynamic._selectedGridEntry = null;

        Assert.NotNull(editFieldAccessibleObject.RuntimeId);
    }

    [WinFormsFact]
    public void GridViewTextBoxAccessibleObject_FragmentRoot_ReturnsExpected()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
        AccessibleObject accessibleObject = gridView.EditAccessibleObject;

        Assert.Equal(propertyGrid.AccessibilityObject, accessibleObject.FragmentRoot);
    }
}

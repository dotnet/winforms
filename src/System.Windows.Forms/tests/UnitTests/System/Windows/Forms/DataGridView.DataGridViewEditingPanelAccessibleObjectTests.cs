// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static Interop;

namespace System.Windows.Forms.Tests;

public class DataGridView_DataGridViewEditingPanelAccessibleObjectTests
{
    [WinFormsFact]
    public void DataGridViewEditingPanelAccessibleObject_FirstAndLastChildren_AreNull()
    {
        using DataGridView dataGridView = new DataGridView();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        AccessibleObject accessibleObject = dataGridView.EditingPanelAccessibleObject;

        // Exception does not appear when trying to get first Child
        UiaCore.IRawElementProviderFragment firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
        Assert.Null(firstChild);

        // Exception does not appear when trying to get last Child
        UiaCore.IRawElementProviderFragment lastChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild);
        Assert.Null(lastChild);
    }

    [WinFormsFact]
    public void DataGridViewEditingPanelAccessibleObject_EditedState_FirstAndLastChildren_AreNotNull()
    {
        using DataGridView dataGridView = new DataGridView();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        DataGridViewCell cell = dataGridView.Rows[0].Cells[0];
        dataGridView.CurrentCell = cell;
        dataGridView.BeginEdit(false);

        AccessibleObject accessibleObject = dataGridView.EditingPanelAccessibleObject;

        UiaCore.IRawElementProviderFragment firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
        Assert.NotNull(firstChild);

        UiaCore.IRawElementProviderFragment lastChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild);
        Assert.NotNull(lastChild);
    }

    [WinFormsFact]
    public void DataGridViewEditingPanelAccessibleObject_ControlType_IsPane_IfAccessibleRoleIsDefault()
    {
        using DataGridView dataGridView = new DataGridView();
        Panel editingPanel = dataGridView.EditingPanel;
        // AccessibleRole is not set = Default

        object actual = editingPanel.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId, actual);
        Assert.False(dataGridView.IsHandleCreated);
        Assert.False(editingPanel.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.Client)]
    [InlineData(false, AccessibleRole.None)]
    public void DataGridViewEditingPanelAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
    {
        using DataGridView dataGridView = new DataGridView();
        Panel editingPanel = dataGridView.EditingPanel;
        // AccessibleRole is not set = Default

        if (createControl)
        {
            editingPanel.CreateControl();
        }

        object actual = editingPanel.AccessibilityObject.Role;

        Assert.Equal(expectedRole, actual);
        Assert.Equal(createControl, editingPanel.IsHandleCreated);
    }

    public static IEnumerable<object[]> DataGridViewEditingPanelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(DataGridViewEditingPanelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void DataGridViewEditingPanelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using DataGridView dataGridView = new DataGridView();
        Panel editingPanel = dataGridView.EditingPanel;
        editingPanel.AccessibleRole = role;

        object actual = editingPanel.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(editingPanel.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewEditingPanelAccessibleObject_GetPropertyValue_ReturnsExpected()
    {
        using DataGridView dataGridView = new();
        Panel editingPanel = dataGridView.EditingPanel;

        Assert.True((bool)editingPanel.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId));
        Assert.Equal(string.Empty, editingPanel.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HelpTextPropertyId));
        Assert.Equal(SR.DataGridView_AccEditingPanelAccName, editingPanel.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId));
        Assert.Equal(SR.DataGridView_AccEditingPanelAccName, editingPanel.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleNamePropertyId));
        Assert.Null(editingPanel.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId));
        Assert.False((bool)editingPanel.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsPasswordPropertyId));
        Assert.False(editingPanel.IsHandleCreated);
    }
}

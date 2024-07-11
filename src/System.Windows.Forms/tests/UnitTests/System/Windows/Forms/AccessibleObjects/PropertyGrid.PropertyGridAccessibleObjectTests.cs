// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.PropertyGridInternal;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class PropertyGrid_PropertyGridAccessibleObjectTests
{
    [WinFormsFact]
    public void PropertyGridAccessibleObject_Ctor_Default()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGrid.PropertyGridAccessibleObject accessibleObject =
            new PropertyGrid.PropertyGridAccessibleObject(propertyGrid);

        Assert.NotNull(accessibleObject.Owner);
        Assert.Equal(propertyGrid, accessibleObject.Owner);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_TableItemPatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_GridItemPatternId)]
    public void GridEntryAccessibleObject_SupportsPattern(int pattern)
    {
        using PropertyGrid propertyGrid = new();
        using ComboBox comboBox = new();
        propertyGrid.SelectedObject = comboBox;
        GridEntry defaultGridEntry = propertyGrid.GetDefaultGridEntry();
        GridEntry parentGridEntry = defaultGridEntry.ParentGridEntry; // Category which has item pattern.
        AccessibleObject accessibleObject = parentGridEntry.AccessibilityObject;
        Assert.True(accessibleObject.IsPatternSupported((UIA_PATTERN_ID)pattern));
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_GridPatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_TablePatternId)]
    public void PropertyGridAccessibleObject_SupportsPattern(int pattern)
    {
        using PropertyGrid propertyGrid = new();
        propertyGrid.CreateControl();
        using ComboBox comboBox = new();
        propertyGrid.SelectedObject = comboBox;
        PropertyGrid.PropertyGridAccessibleObject propertyGridAccessibleObject =
            new PropertyGrid.PropertyGridAccessibleObject(propertyGrid);

        // First child should be PropertyGrid toolbox.
        AccessibleObject firstChild = (AccessibleObject)propertyGridAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild);

        // Second child entry should be PropertyGridView.
        AccessibleObject gridViewChild = (AccessibleObject)firstChild.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);

        Assert.True(gridViewChild.IsPatternSupported((UIA_PATTERN_ID)pattern));
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.Client)]
    [InlineData(false, AccessibleRole.None)]
    public void PropertyGridAccessibleObject_ControlType_IsPane_IfAccessibleRoleIsDefault(bool createControl, AccessibleRole expectedRole)
    {
        using PropertyGrid propertyGrid = new();
        // AccessibleRole is not set = Default

        if (createControl)
        {
            propertyGrid.CreateControl();
        }

        AccessibleObject accessibleObject = propertyGrid.AccessibilityObject;
        var actual = (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(expectedRole, accessibleObject.Role);
        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId, actual);
        Assert.Equal(createControl, propertyGrid.IsHandleCreated);
    }

    public static IEnumerable<object[]> PropertyGridAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(PropertyGridAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void PropertyGridAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using PropertyGrid propertyGrid = new();
        propertyGrid.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)propertyGrid.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(propertyGrid.IsHandleCreated);
    }

    [WinFormsFact]
    public void PropertyGridAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGrid.PropertyGridAccessibleObject accessibleObject = new(propertyGrid);

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
    }

    [WinFormsFact]
    public void PropertyGridAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGrid.PropertyGridAccessibleObject accessibleObject = new(propertyGrid);

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
    }

    [WinFormsFact]
    public void PropertyGridAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGrid.PropertyGridAccessibleObject accessibleObject = new(propertyGrid);

        AccessibleObject expected = propertyGrid.ToolbarAccessibleObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsFact]
    public void PropertyGridAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfToolbarHidden()
    {
        using PropertyGrid propertyGrid = new() { ToolbarVisible = false };
        PropertyGrid.PropertyGridAccessibleObject accessibleObject = new(propertyGrid);

        AccessibleObject expected = propertyGrid.GridViewAccessibleObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsFact]
    public void PropertyGridAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGrid.PropertyGridAccessibleObject accessibleObject = new(propertyGrid);

        AccessibleObject expected = propertyGrid.HelpPaneAccessibleObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsFact]
    public void PropertyGridAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfHelpHidden()
    {
        using PropertyGrid propertyGrid = new() { HelpVisible = false };

        // Assign an object with commands to make CommandsPane visible
        using Component component = new();
        component.Site = new SiteWithMenuCommands();
        propertyGrid.SelectedObject = component;

        PropertyGrid.PropertyGridAccessibleObject accessibleObject = new(propertyGrid);

        AccessibleObject expected = propertyGrid.CommandsPaneAccessibleObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsFact]
    public void PropertyGridAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfHelpAndCommandsHidden()
    {
        using PropertyGrid propertyGrid = new() { HelpVisible = false, CommandsVisibleIfAvailable = false };
        PropertyGrid.PropertyGridAccessibleObject accessibleObject = new(propertyGrid);

        AccessibleObject expected = propertyGrid.GridViewAccessibleObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    private class SiteWithMenuCommands : ISite
    {
        private class MenuCommandService : IMenuCommandService
        {
            public DesignerVerbCollection Verbs { get; } = new([new DesignerVerb("", null)]);

            public void AddCommand(MenuCommand command) => throw new NotImplementedException();

            public void AddVerb(DesignerVerb verb) => throw new NotImplementedException();

            public MenuCommand FindCommand(CommandID commandID) => throw new NotImplementedException();

            public bool GlobalInvoke(CommandID commandID) => throw new NotImplementedException();

            public void RemoveCommand(MenuCommand command) => throw new NotImplementedException();

            public void RemoveVerb(DesignerVerb verb) => throw new NotImplementedException();

            public void ShowContextMenu(CommandID menuID, int x, int y) => throw new NotImplementedException();
        }

        public IComponent Component { get; }

        public IContainer Container { get; }

        public bool DesignMode => false;

        public string Name { get; set; }

        public object GetService(Type serviceType)
        {
            return serviceType == typeof(IMenuCommandService) ? new MenuCommandService() : null;
        }
    }
}

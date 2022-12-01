// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.PropertyGridInternal;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class PropertyGrid_PropertyGridAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void PropertyGridAccessibleObject_Ctor_Default()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGrid.PropertyGridAccessibleObject accessibleObject =
                new PropertyGrid.PropertyGridAccessibleObject(propertyGrid);

            Assert.NotNull(accessibleObject.Owner);
            Assert.Equal(propertyGrid, accessibleObject.Owner);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.TableItemPatternId)]
        [InlineData((int)UiaCore.UIA.GridItemPatternId)]
        public void GridEntryAccessibleObject_SupportsPattern(int pattern)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using ComboBox comboBox = new ComboBox();
            propertyGrid.SelectedObject = comboBox;
            GridEntry defaultGridEntry = propertyGrid.GetDefaultGridEntry();
            GridEntry parentGridEntry = defaultGridEntry.ParentGridEntry; // Category which has item pattern.
            AccessibleObject accessibleObject = parentGridEntry.AccessibilityObject;
            Assert.True(accessibleObject.IsPatternSupported((UiaCore.UIA)pattern));
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.GridPatternId)]
        [InlineData((int)UiaCore.UIA.TablePatternId)]
        public void PropertyGridAccessibleObject_SupportsPattern(int pattern)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            propertyGrid.CreateControl();
            using ComboBox comboBox = new ComboBox();
            propertyGrid.SelectedObject = comboBox;
            PropertyGrid.PropertyGridAccessibleObject propertyGridAccessibleObject =
                new PropertyGrid.PropertyGridAccessibleObject(propertyGrid);

            // First child should be PropertyGrid toolbox.
            AccessibleObject firstChild = (AccessibleObject)propertyGridAccessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);

            // Second child entry should be PropertyGridView.
            AccessibleObject gridViewChild = (AccessibleObject)firstChild.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);

            Assert.True(gridViewChild.IsPatternSupported((UiaCore.UIA)pattern));
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Client)]
        [InlineData(false, AccessibleRole.None)]
        public void PropertyGridAccessibleObject_ControlType_IsPane_IfAccessibleRoleIsDefault(bool createControl, AccessibleRole expectedRole)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            // AccessibleRole is not set = Default

            if (createControl)
            {
                propertyGrid.CreateControl();
            }

            AccessibleObject accessibleObject = propertyGrid.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(expectedRole, accessibleObject.Role);
            Assert.Equal(UiaCore.UIA.PaneControlTypeId, actual);
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
            using PropertyGrid propertyGrid = new PropertyGrid();
            propertyGrid.AccessibleRole = role;

            object actual = propertyGrid.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(propertyGrid.IsHandleCreated);
        }

        [WinFormsFact]
        public void PropertyGridAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
        {
            using PropertyGrid propertyGrid = new();
            PropertyGrid.PropertyGridAccessibleObject accessibleObject = new(propertyGrid);

            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
        }

        [WinFormsFact]
        public void PropertyGridAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
        {
            using PropertyGrid propertyGrid = new();
            PropertyGrid.PropertyGridAccessibleObject accessibleObject = new(propertyGrid);

            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
        }

        [WinFormsFact]
        public void PropertyGridAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected()
        {
            using PropertyGrid propertyGrid = new();
            PropertyGrid.PropertyGridAccessibleObject accessibleObject = new(propertyGrid);

            AccessibleObject expected = propertyGrid.ToolbarAccessibleObject;

            Assert.Equal(expected, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
        }

        [WinFormsFact]
        public void PropertyGridAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfToolbarHidden()
        {
            using PropertyGrid propertyGrid = new() { ToolbarVisible = false };
            PropertyGrid.PropertyGridAccessibleObject accessibleObject = new(propertyGrid);

            AccessibleObject expected = propertyGrid.GridViewAccessibleObject;

            Assert.Equal(expected, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
        }

        [WinFormsFact]
        public void PropertyGridAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected()
        {
            using PropertyGrid propertyGrid = new();
            PropertyGrid.PropertyGridAccessibleObject accessibleObject = new(propertyGrid);

            AccessibleObject expected = propertyGrid.HelpPaneAccessibleObject;

            Assert.Equal(expected, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
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

            Assert.Equal(expected, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
        }

        [WinFormsFact]
        public void PropertyGridAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfHelpAndCommandsHidden()
        {
            using PropertyGrid propertyGrid = new() { HelpVisible = false, CommandsVisibleIfAvailable = false };
            PropertyGrid.PropertyGridAccessibleObject accessibleObject = new(propertyGrid);

            AccessibleObject expected = propertyGrid.GridViewAccessibleObject;

            Assert.Equal(expected, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
        }

        private class SiteWithMenuCommands : ISite
        {
            private class MenuCommandService : IMenuCommandService
            {
                public DesignerVerbCollection Verbs { get; } = new DesignerVerbCollection(new[] { new DesignerVerb("", null) });

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
}

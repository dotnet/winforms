// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.PropertyGridInternal;
using Xunit;
using static System.Windows.Forms.PropertyGridInternal.CategoryGridEntry;
using static System.Windows.Forms.PropertyGridInternal.PropertyGridView;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class CategoryGridEntryAccessibleObjectTests
    {
        [WinFormsFact]
        public void CategoryGridEntryAccessibleObject_Ctor_Default()
        {
            using NoAssertContext context = new();
            var accessibilityObject = new CategoryGridEntryAccessibleObject(null);

            Assert.Null(accessibilityObject.TestAccessor().Dynamic._owningCategoryGridEntry);
        }

        [WinFormsFact]
        public void CategoryGridEntryAccessibleObject_Role_ReturnsExpected()
        {
            using NoAssertContext context = new();
            var accessibilityObject = new CategoryGridEntryAccessibleObject(null);

            Assert.Equal(AccessibleRole.ButtonDropDownGrid, accessibilityObject.Role);
        }

        [WinFormsFact]
        public void CategoryGridEntryAccessibleObject_Column_ReturnsExpected()
        {
            using NoAssertContext context = new();
            var accessibilityObject = new CategoryGridEntryAccessibleObject(null);

            Assert.Equal(0, accessibilityObject.Column);
        }

        [WinFormsFact]
        public void CategoryGridEntryAccessibleObject_ControlType_ReturnsExpected()
        {
            using NoAssertContext context = new();
            var accessibilityObject = new CategoryGridEntryAccessibleObject(null);

            UiaCore.UIA expected = UiaCore.UIA.TreeItemControlTypeId;

            Assert.Equal(expected, accessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.GridItemPatternId)]
        [InlineData((int)UiaCore.UIA.TableItemPatternId)]
        public void CategoryGridEntryAccessibleObject_IsPatternSupported_ReturnsExpected(int patternId)
        {
            using NoAssertContext context = new();
            var accessibilityObject = new CategoryGridEntryAccessibleObject(null);

            Assert.True(accessibilityObject.IsPatternSupported((UiaCore.UIA)patternId));
        }

        [WinFormsFact]
        public void CategoryGridEntryAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
        {
            using PropertyGrid control = new();
            using Button button = new();
            control.SelectedObject = button;
            PropertyGridView gridView = control.TestAccessor().Dynamic._gridView;

            AccessibleObject accessibilityObject = gridView.TopLevelGridEntries[0].AccessibilityObject;

            Assert.Equal(gridView.AccessibilityObject, accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
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

            Assert.Equal(accessibilityObjectCategory2, accessibilityObjectCategory1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibilityObjectLastCategory.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
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

            Assert.Null(accessibilityObjectCategory1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(accessibilityObjectCategory1, accessibilityObjectCategory2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
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

            Assert.Equal(accessibilityObjectFirstItem, accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
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

            Assert.Equal(accessibilityObjectLastItem, accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.False(control.IsHandleCreated);
            Assert.False(button.IsHandleCreated);
        }

        [WinFormsFact]
        public void CategoryGridEntryAccessibleObject_LocalizedControlType_ReturnsExpected()
        {
            using NoAssertContext context = new();
            var accessibilityObject = new CategoryGridEntryAccessibleObject(null);

            string expected = SR.CategoryPropertyGridLocalizedControlType;

            Assert.Equal(expected, accessibilityObject.GetPropertyValue(UiaCore.UIA.LocalizedControlTypePropertyId));
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.LegacyIAccessibleRolePropertyId, AccessibleRole.ButtonDropDownGrid)]
        [InlineData((int)UiaCore.UIA.IsGridItemPatternAvailablePropertyId, true)]
        [InlineData((int)UiaCore.UIA.IsTableItemPatternAvailablePropertyId, true)]
        public void CategoryGridEntryAccessibleObject_GetPropertyValue_ReturnsExpected(int property, object expected)
        {
            using NoAssertContext context = new();
            CategoryGridEntryAccessibleObject accessibleObject = new(null);
            object actual = accessibleObject.GetPropertyValue((UiaCore.UIA)property);

            Assert.Equal(expected, actual);
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

            Assert.Equal("Collapse", accessibilityObject.GetPropertyValue(UiaCore.UIA.LegacyIAccessibleDefaultActionPropertyId));
            Assert.Null(accessibilityObject.GetPropertyValue(UiaCore.UIA.ValueValuePropertyId));
        }
    }
}

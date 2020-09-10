// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Reflection;
using Xunit;
using static System.Windows.Forms.Control;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    public class PropertyGridView_GridViewEdit_GridViewEditAccessibleObjectTests
    {
        [WinFormsFact]
        public void GridViewEditAccessibleObject_created_for_string_property()
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
            PropertyDescriptorGridEntry selectedGridEntry = propertyGridView.TestAccessor().Dynamic.selectedGridEntry;

            Assert.Equal(gridEntry.PropertyName, selectedGridEntry.PropertyName);

            AccessibleObject selectedGridEntryAccessibleObject = gridEntry.AccessibilityObject;
            UiaCore.IRawElementProviderFragment editFieldAccessibleObject = selectedGridEntryAccessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            Assert.NotNull(editFieldAccessibleObject);

            Assert.Equal("GridViewEditAccessibleObject", editFieldAccessibleObject.GetType().Name);
        }

        [WinFormsFact]
        public void GridViewEditAccessibleObject_FragmentNavigate_navigates_correctly()
        {
            using PropertyGrid propertyGrid = new PropertyGrid
            {
                SelectedObject = Point.Empty
            };
            propertyGrid.CreateControl();
            PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;

            int firstPropertyIndex = 2; // Index of Text property which has a RichEdit control as an editor.
            PropertyDescriptorGridEntry gridEntry = (PropertyDescriptorGridEntry)propertyGridView.AccessibilityGetGridEntries()[firstPropertyIndex];

            propertyGridView.TestAccessor().Dynamic.selectedGridEntry = gridEntry;

            UiaCore.IRawElementProviderFragment editFieldAccessibleObject = gridEntry.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            Assert.Equal("GridViewEditAccessibleObject", editFieldAccessibleObject.GetType().Name);

            // The case with drop down holder:
            using TestDropDownHolder dropDownHolder = new TestDropDownHolder(propertyGridView);
            dropDownHolder.CreateControl();
            propertyGridView.TestAccessor().Dynamic.dropDownHolder = dropDownHolder;

            dropDownHolder.SetState(0x00000002, true); // Control class States.Visible flag

            Assert.True(propertyGridView.DropDownVisible);
            object previousAccessibleObject = editFieldAccessibleObject.Navigate(UiaCore.NavigateDirection.PreviousSibling);
            Assert.NotNull(previousAccessibleObject);
            Assert.Same(dropDownHolder.AccessibilityObject, previousAccessibleObject);
        }

        public class TestEntityWithTextField
        {
            public string TextProperty { get; set; }
        }

        private class TestDropDownHolder : PropertyGridView.DropDownHolder
        {
            public TestDropDownHolder(PropertyGridView psheet)
                : base(psheet)
            {
            }

            internal void SetState(int flag, bool value)
            {
                SetState((States)flag, value);
            }
        }

        [WinFormsFact]
        public void GridViewEditAccessibleObjectt_ctor_default()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
            Type gridViewEditType = typeof(PropertyGridView).GetNestedType("GridViewEdit", BindingFlags.NonPublic);
            Assert.NotNull(gridViewEditType);
            TextBox gridViewEdit = (TextBox)Activator.CreateInstance(gridViewEditType, gridView);
            Type accessibleObjectType = gridViewEditType.GetNestedType("GridViewEditAccessibleObject", BindingFlags.NonPublic);
            Assert.NotNull(accessibleObjectType);
            ControlAccessibleObject accessibleObject = (ControlAccessibleObject)Activator.CreateInstance(accessibleObjectType, gridViewEdit);
            Assert.Equal(gridViewEdit, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void GridViewEditAccessibleObjectt_ctor_ThrowsException_IfOwnerIsNull()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
            Type gridViewEditType = typeof(PropertyGridView).GetNestedType("GridViewEdit", BindingFlags.NonPublic);
            Assert.NotNull(gridViewEditType);
            TextBox gridViewEdit = (TextBox)Activator.CreateInstance(gridViewEditType, gridView);
            Type accessibleObjectType = gridViewEditType.GetNestedType("GridViewEditAccessibleObject", BindingFlags.NonPublic);
            Assert.NotNull(accessibleObjectType);
            Assert.Throws<TargetInvocationException>(() => Activator.CreateInstance(accessibleObjectType, (TextBox)null));
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.IsTextPatternAvailablePropertyId)]
        [InlineData((int)UiaCore.UIA.IsTextPattern2AvailablePropertyId)]
        [InlineData((int)UiaCore.UIA.IsValuePatternAvailablePropertyId)]
        public void ToolStripTextBoxControlAccessibleObject_GetPropertyValue_PatternsSuported(int propertyID)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
            AccessibleObject accessibleObject = gridView.EditAccessibleObject;
            Assert.True((bool)accessibleObject.GetPropertyValue((UiaCore.UIA)propertyID));
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.ValuePatternId)]
        [InlineData((int)UiaCore.UIA.TextPatternId)]
        [InlineData((int)UiaCore.UIA.TextPattern2Id)]
        public void ToolStripTextBoxControlAccessibleObject_IsPatternSupported_PatternsSuported(int patternId)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
            AccessibleObject accessibleObject = gridView.EditAccessibleObject;
            Assert.True(accessibleObject.IsPatternSupported((UiaCore.UIA)patternId));
        }
    }
}

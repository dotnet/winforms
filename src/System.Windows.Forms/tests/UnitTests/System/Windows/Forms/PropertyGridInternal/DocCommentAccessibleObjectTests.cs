// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    public class DocCommentAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DocCommentAccessibleObject_Ctor_Default()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using DocComment docComment = new DocComment(propertyGrid);
            DocCommentAccessibleObject accessibleObject = new DocCommentAccessibleObject(docComment, propertyGrid);

            Assert.Equal(docComment, accessibleObject.Owner);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.False(docComment.IsHandleCreated);
        }

        [WinFormsFact]
        public void DocCommentAccessibleObject_ControlType_IsPane_IfAccessibleRoleIsDefault()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using DocComment docComment = new DocComment(propertyGrid);
            // AccessibleRole is not set = Default

            object actual = docComment.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.PaneControlTypeId, actual);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.False(docComment.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Client)]
        [InlineData(false, AccessibleRole.None)]
        public void DocCommentAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using DocComment docComment = new DocComment(propertyGrid);
            // AccessibleRole is not set = Default

            if (createControl)
            {
                docComment.CreateControl();
            }

            AccessibleRole actual = docComment.AccessibilityObject.Role;

            Assert.Equal(expectedRole, actual);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.Equal(createControl, docComment.IsHandleCreated);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class UpDownEditAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void UpDownEditAccessibleObject_ctor_default()
        {
            using UpDownBase upDown = new SubUpDownBase();
            using UpDownBase.UpDownEdit upDownEdit = new UpDownBase.UpDownEdit(upDown);
            UpDownBase.UpDownEdit.UpDownEditAccessibleObject accessibleObject = new UpDownBase.UpDownEdit.UpDownEditAccessibleObject(upDownEdit, upDown);
            Assert.Equal(upDownEdit, accessibleObject.Owner);
            Assert.False(upDown.IsHandleCreated);
            Assert.False(upDownEdit.IsHandleCreated);
        }

        [WinFormsFact]
        public void UpDownEditAccessibleObject_ctor_ThrowsException_IfParentIsNull()
        {
            using UpDownBase upDown = new SubUpDownBase();
            using UpDownBase.UpDownEdit upDownEdit = new UpDownBase.UpDownEdit(upDown);
            Assert.Throws<ArgumentNullException>(() => new UpDownBase.UpDownEdit.UpDownEditAccessibleObject(upDownEdit, null));
        }

        [WinFormsFact]
        public void UpDownEditAccessibleObject_ctor_ThrowsException_IfOwnerIsNull()
        {
            using UpDownBase upDown = new SubUpDownBase();
            Assert.Throws<ArgumentNullException>(() => new UpDownBase.UpDownEdit.UpDownEditAccessibleObject(null, upDown));
        }

        [WinFormsFact]
        public void UpDownEditAccessibleObject_IsIAccessibleExSupported_ReturnsTrue()
        {
            using UpDownBase upDown = new SubUpDownBase();
            using UpDownBase.UpDownEdit upDownEdit = new UpDownBase.UpDownEdit(upDown);
            AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
            Assert.True(accessibleObject.IsIAccessibleExSupported());
            Assert.False(upDown.IsHandleCreated);
            Assert.False(upDownEdit.IsHandleCreated);
        }

        [WinFormsFact]
        public void UpDownEditAccessibleObject_Custom_Name_ReturnsCorrectValue()
        {
            using UpDownBase upDown = new SubUpDownBase();
            string name = "Custom name";
            upDown.AccessibleName = name;
            using UpDownBase.UpDownEdit upDownEdit = new UpDownBase.UpDownEdit(upDown);
            AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
            Assert.Equal(name, accessibleObject.Name);
            Assert.False(upDown.IsHandleCreated);
            Assert.False(upDownEdit.IsHandleCreated);
        }

        [WinFormsFact]
        public void UpDownEditAccessibleObject_Default_Name_ReturnsNull()
        {
            using UpDownBase upDown = new SubUpDownBase();
            using UpDownBase.UpDownEdit upDownEdit = new UpDownBase.UpDownEdit(upDown);
            AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
            Assert.Null(accessibleObject.Name);
            Assert.False(upDown.IsHandleCreated);
            Assert.False(upDownEdit.IsHandleCreated);
        }

        [WinFormsFact]
        public void UpDownEditAccessibleObject_Default_Name_ReturnsExpected_NumericUpDown()
        {
            using NumericUpDown upDown = new();
            using UpDownBase.UpDownEdit upDownEdit = new UpDownBase.UpDownEdit(upDown);
            AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
            Assert.Equal(SR.EditDefaultAccessibleName, accessibleObject.Name);
            Assert.False(upDown.IsHandleCreated);
            Assert.False(upDownEdit.IsHandleCreated);
        }

        [WinFormsFact]
        public void UpDownEditAccessibleObject_Default_Name_ReturnsExpected_DomainUpDown()
        {
            using DomainUpDown upDown = new();
            using UpDownBase.UpDownEdit upDownEdit = new UpDownBase.UpDownEdit(upDown);
            AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
            Assert.Equal(SR.EditDefaultAccessibleName, accessibleObject.Name);
            Assert.False(upDown.IsHandleCreated);
            Assert.False(upDownEdit.IsHandleCreated);
        }

        [WinFormsFact]
        public void UpDownEditAccessibleObject_KeyboardShortcut_ReturnsParentsKeyboardShortcut()
        {
            using UpDownBase upDown = new SubUpDownBase();
            using UpDownBase.UpDownEdit upDownEdit = new UpDownBase.UpDownEdit(upDown);
            AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
            Assert.Equal(upDown.AccessibilityObject.KeyboardShortcut, accessibleObject.KeyboardShortcut);
            Assert.False(upDown.IsHandleCreated);
            Assert.False(upDownEdit.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void UpDownEditAccessibleObject_IsReadOnly_IsExpected(bool readOnly)
        {
            using UpDownBase upDown = new SubUpDownBase();
            using UpDownBase.UpDownEdit upDownEdit = new UpDownBase.UpDownEdit(upDown);
            upDownEdit.ReadOnly = readOnly;
            AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
            Assert.Equal(readOnly, accessibleObject.IsReadOnly);
            Assert.False(upDown.IsHandleCreated);
            Assert.False(upDownEdit.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.IsTextPatternAvailablePropertyId)]
        [InlineData((int)UiaCore.UIA.IsTextPattern2AvailablePropertyId)]
        public void UpDownEditAccessibleObject_GetPropertyValue_PatternsSuported(int propertyID)
        {
            using UpDownBase upDown = new SubUpDownBase();
            using UpDownBase.UpDownEdit upDownEdit = new UpDownBase.UpDownEdit(upDown);
            AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
            Assert.True((bool)accessibleObject.GetPropertyValue((UiaCore.UIA)propertyID));
            Assert.False(upDown.IsHandleCreated);
            Assert.False(upDownEdit.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.TextPatternId)]
        [InlineData((int)UiaCore.UIA.TextPattern2Id)]
        public void UpDownEditAccessibleObject_IsPatternSupported_PatternsSuported(int patternId)
        {
            using UpDownBase upDown = new SubUpDownBase();
            using UpDownBase.UpDownEdit upDownEdit = new UpDownBase.UpDownEdit(upDown);
            AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
            Assert.True(accessibleObject.IsPatternSupported((UiaCore.UIA)patternId));
            Assert.False(upDown.IsHandleCreated);
            Assert.False(upDownEdit.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Text, (int)UiaCore.UIA.EditControlTypeId)]
        [InlineData(false, AccessibleRole.None, (int)UiaCore.UIA.PaneControlTypeId)]
        public void UpDownEditAccessibleObject_ControlType_IsExpected_IfAccessibleRoleIsDefault(bool createControl, AccessibleRole expectedRole, int expectedType)
        {
            using UpDownBase upDown = new SubUpDownBase();
            using UpDownBase.UpDownEdit upDownEdit = new UpDownBase.UpDownEdit(upDown);
            // AccessibleRole is not set = Default

            if (createControl)
            {
                upDownEdit.CreateControl();
            }

            AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(expectedRole, accessibleObject.Role);
            Assert.Equal((UiaCore.UIA)expectedType, actual);
            Assert.Equal(createControl, upDownEdit.IsHandleCreated);
        }

        public static IEnumerable<object[]> UpDownEditAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(UpDownEditAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void UpDownEditAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using UpDownBase upDown = new SubUpDownBase();
            using UpDownBase.UpDownEdit upDownEdit = new UpDownBase.UpDownEdit(upDown);
            upDownEdit.AccessibleRole = role;

            object actual = upDownEdit.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(upDownEdit.IsHandleCreated);
        }

        private class SubUpDownBase : UpDownBase
        {
            public override void DownButton() { }

            public override void UpButton() { }

            protected override void UpdateEditText() { }
        }
    }
}

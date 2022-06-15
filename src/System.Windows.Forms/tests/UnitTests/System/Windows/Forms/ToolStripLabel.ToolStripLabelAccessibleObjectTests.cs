// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripLabel;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripLabel_ToolStripLabelAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripLabelAccessibleObject_Ctor_Default()
        {
            using ToolStripLabel toolStripLabel = new ToolStripLabel();
            ToolStripLabelAccessibleObject accessibleObject = new ToolStripLabelAccessibleObject(toolStripLabel);

            Assert.Equal(toolStripLabel, accessibleObject.Owner);
        }

        [WinFormsTheory]
        [InlineData(true, (int)UiaCore.UIA.HyperlinkControlTypeId)]
        [InlineData(false, (int)UiaCore.UIA.TextControlTypeId)]
        public void ToolStripLabelAccessibleObject_ControlType_IsExpected_IfAccessibleRoleIsDefault(bool isLink, int expectedType)
        {
            using ToolStripLabel toolStripLabel = new ToolStripLabel();
            toolStripLabel.IsLink = isLink;
            // AccessibleRole is not set = Default

            object actual = toolStripLabel.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal((UiaCore.UIA)expectedType, actual);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Link)]
        [InlineData(false, AccessibleRole.StaticText)]
        public void ToolStripLabelAccessibleObject_Role_IsExpected_ByDefault(bool isLink, AccessibleRole expectedRole)
        {
            using ToolStripLabel toolStripLabel = new ToolStripLabel();
            toolStripLabel.IsLink = isLink;
            // AccessibleRole is not set = Default

            AccessibleRole actual = toolStripLabel.AccessibilityObject.Role;

            Assert.Equal(expectedRole, actual);
        }

        public static IEnumerable<object[]> ToolStripLabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(ToolStripLabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ToolStripLabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ToolStripLabel toolStripLabel = new ToolStripLabel();
            toolStripLabel.AccessibleRole = role;

            Assert.Equal(AccessibleRoleControlTypeMap.GetControlType(role), toolStripLabel.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
            Assert.Null(toolStripLabel.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ValueValuePropertyId));
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleStates.ReadOnly | AccessibleStates.Focusable)]
        [InlineData(false, AccessibleStates.ReadOnly)]
        public void ToolStripLabelAccessibleObject_GetPropertyValue_LegacyIAccessibleStatePropertyId_ReturnsExpected(bool isLink, AccessibleStates expectedState)
        {
            using ToolStripLabel toolStripLabel = new ToolStripLabel() { IsLink = isLink };
            object actual = toolStripLabel.AccessibilityObject.GetPropertyValue(UiaCore.UIA.LegacyIAccessibleStatePropertyId);

            Assert.Equal(expectedState, actual);
        }
    }
}

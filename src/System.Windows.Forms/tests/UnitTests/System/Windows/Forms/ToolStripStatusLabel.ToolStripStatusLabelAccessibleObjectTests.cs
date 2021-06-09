// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static System.Windows.Forms.ToolStripStatusLabel;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripStatusLabel_ToolStripStatusLabelAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripStatusLabelAccessibleObject_Ctor_Default()
        {
            using ToolStripStatusLabel toolStripStatusLabel = new ToolStripStatusLabel();
            ToolStripStatusLabelAccessibleObject accessibleObject = new ToolStripStatusLabelAccessibleObject(toolStripStatusLabel);

            Assert.Equal(toolStripStatusLabel, accessibleObject.Owner);
        }

        [WinFormsTheory]
        [InlineData(true, (int)UiaCore.UIA.HyperlinkControlTypeId)]
        [InlineData(false, (int)UiaCore.UIA.TextControlTypeId)]
        public void ToolStripStatusLabelAccessibleObject_ControlType_IsExpected_IfAccessibleRoleIsDefault(bool isLink, int expectedType)
        {
            using ToolStripStatusLabel toolStripStatusLabel = new ToolStripStatusLabel();
            toolStripStatusLabel.IsLink = isLink;
            // AccessibleRole is not set = Default

            object actual = toolStripStatusLabel.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal((UiaCore.UIA)expectedType, actual);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Link)]
        [InlineData(false, AccessibleRole.StaticText)]
        public void ToolStripStatusLabelAccessibleObject_Role_IsExpected_ByDefault(bool isLink, AccessibleRole expectedRole)
        {
            using ToolStripStatusLabel toolStripStatusLabel = new ToolStripStatusLabel();
            toolStripStatusLabel.IsLink = isLink;
            // AccessibleRole is not set = Default

            AccessibleRole actual = toolStripStatusLabel.AccessibilityObject.Role;

            Assert.Equal(expectedRole, actual);
        }

        public static IEnumerable<object[]> ToolStripStatusLabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(ToolStripStatusLabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ToolStripStatusLabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ToolStripStatusLabel toolStripStatusLabel = new ToolStripStatusLabel();
            toolStripStatusLabel.AccessibleRole = role;

            object actual = toolStripStatusLabel.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
        }
    }
}

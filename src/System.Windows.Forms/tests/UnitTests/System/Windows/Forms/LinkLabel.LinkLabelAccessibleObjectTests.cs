// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static System.Windows.Forms.LinkLabel;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class LinkLabel_LinkLabelAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void LinkLabelAccessibleObject_Ctor_Default()
        {
            using LinkLabel linkLabel = new LinkLabel();
            LinkLabelAccessibleObject accessibleObject = new LinkLabelAccessibleObject(linkLabel);

            Assert.Equal(linkLabel, accessibleObject.Owner);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsFact]
        public void LinkLabelAccessibleObject_ControlType_IsText_IfAccessibleRoleIsDefault()
        {
            using LinkLabel linkLabel = new LinkLabel();
            // AccessibleRole is not set = Default

            object actual = linkLabel.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.TextControlTypeId, actual);
            Assert.False(linkLabel.IsHandleCreated);
        }

        public static IEnumerable<object[]> LinkLabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(LinkLabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void LinkLabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using LinkLabel linkLabel = new LinkLabel();
            linkLabel.AccessibleRole = role;

            object actual = linkLabel.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsFact]
        public void LinkLabelAccessibleObject_Role_IsStaticText_ByDefault()
        {
            using LinkLabel linkLabel = new LinkLabel();
            // AccessibleRole is not set = Default

            AccessibleRole actual = linkLabel.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.StaticText, actual);
            Assert.False(linkLabel.IsHandleCreated);
        }
    }
}

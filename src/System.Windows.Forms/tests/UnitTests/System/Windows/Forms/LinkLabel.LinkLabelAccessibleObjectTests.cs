// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.LinkLabel.Link;
using static Interop;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class LinkLabel_LinkLabelAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void LinkLabelAccessibleObject_Ctor_Default()
        {
            using LinkLabel linkLabel = new();
            LinkLabelAccessibleObject accessibleObject = new(linkLabel);

            Assert.Equal(linkLabel, accessibleObject.Owner);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsFact]
        public void LinkLabelAccessibleObject_ControlType_IsText_IfAccessibleRoleIsDefault()
        {
            using LinkLabel linkLabel = new();
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
            using LinkLabel linkLabel = new();
            linkLabel.AccessibleRole = role;

            object actual = linkLabel.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsFact]
        public void LinkLabelAccessibleObject_Role_IsStaticText_ByDefault()
        {
            using LinkLabel linkLabel = new();
            // AccessibleRole is not set = Default

            AccessibleRole actual = linkLabel.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.StaticText, actual);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(10)]
        public void LinkLabelAccessibleObject_GetChildCount_IsExpected(int linkCount)
        {
            using LinkLabel linkLabel = new();

            for (int i = 0; i < linkCount; i++)
            {
                linkLabel.Links.Add(new());
            }

            int actual = linkLabel.AccessibilityObject.GetChildCount();

            Assert.Equal(linkCount, actual);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(15)]
        [InlineData(27)]
        [InlineData(38)]
        [InlineData(49)]
        public void LinkLabelAccessibleObject_GetChild_IsExpectedValue(int childIndex)
        {
            using LinkLabel linkLabel = new();

            for (int index = 0; index < 50; index++)
            {
                linkLabel.Links.Add(new());
            }

            LinkAccessibleObject expected = linkLabel.Links[childIndex].AccessibleObject;
            var actual = (LinkAccessibleObject)linkLabel.AccessibilityObject.GetChild(childIndex);

            Assert.Equal(expected, actual);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1000)]
        public void LinkLabelAccessibleObject_GetChild_IsNull(int childIndex)
        {
            using LinkLabel linkLabel = new();

            for (int index = 0; index < 50; index++)
            {
                linkLabel.Links.Add(new());
            }

            Assert.Null(linkLabel.AccessibilityObject.GetChild(childIndex));
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void LinkLabelAccessibleObject_GetPropertyValue_Enabled_IsExpected(bool isEnabled)
        {
            using LinkLabel linkLabel = new();

            linkLabel.Enabled = isEnabled;
            bool actual = (bool)linkLabel.AccessibilityObject.GetPropertyValue(UiaCore.UIA.IsEnabledPropertyId);

            Assert.Equal(isEnabled, actual);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsFact]
        public void LinkLabelAccessibleObject_FragmentNavigate_IsExpected_NavigateToFirstChild()
        {
            using LinkLabel linkLabel = new();
            AccessibleObject accessibleObject = linkLabel.AccessibilityObject;

            for (int index = 0; index < 4; index++)
            {
                linkLabel.Links.Add(new());
            }

            Assert.Equal(linkLabel.Links[0].AccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsFact]
        public void LinkLabelAccessibleObject_FragmentNavigate_IsExpected_NavigateToLastChild()
        {
            using LinkLabel linkLabel = new();
            AccessibleObject accessibleObject = linkLabel.AccessibilityObject;

            for (int index = 0; index < 4; index++)
            {
                linkLabel.Links.Add(new());
            }

            Assert.Equal(linkLabel.Links[^1].AccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.LastChild));
            Assert.False(linkLabel.IsHandleCreated);
        }
    }
}

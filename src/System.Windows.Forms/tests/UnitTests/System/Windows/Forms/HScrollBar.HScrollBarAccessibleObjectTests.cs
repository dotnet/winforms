// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Xunit;
using static System.Windows.Forms.ScrollBar;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class HScrollBar_HScrollBarAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void HScrollBarAccessibleObject_ctor_ThrowsException_HScrollBarAccessibleObjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ScrollBarAccessibleObject(null));
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.ScrollBar)]
        [InlineData(false, AccessibleRole.None)]
        public void HScrollBarAccessibleObject_Ctor_Default(bool createControl, AccessibleRole accessibleRole)
        {
            using var scrollBar = new HScrollBar();

            if (createControl)
            {
                scrollBar.CreateControl();
            }

            AccessibleObject accessibleObject = scrollBar.AccessibilityObject;

            Assert.NotNull(accessibleObject);
            Assert.Equal(accessibleRole, accessibleObject.Role);
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Test")]
        public void HScrollBarAccessibleObject_Description_Get_ReturnsExpected(string accessibleDescription)
        {
            using HScrollBar scrollBar = new();
            scrollBar.AccessibleDescription = accessibleDescription;
            ScrollBarAccessibleObject accessibleObject =
                Assert.IsType<ScrollBarAccessibleObject>(scrollBar.AccessibilityObject);

            Assert.Equal(accessibleDescription, accessibleObject.Description);
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Test")]
        public void HScrollBarAccessibleObject_Name_Get_ReturnsExpected(string accessibleName)
        {
            using HScrollBar scrollBar = new();
            scrollBar.AccessibleName = accessibleName;
            ScrollBarAccessibleObject accessibleObject =
                Assert.IsType<ScrollBarAccessibleObject>(scrollBar.AccessibilityObject);

            Assert.Equal(accessibleName, accessibleObject.Name);
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsFact]
        public void HScrollBarAccessibleObject_ControlType_IsScrollBar_AccessibleRoleIsDefault()
        {
            using HScrollBar scrollBar = new();
            // AccessibleRole is not set = Default

            object actual = scrollBar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ScrollBarControlTypeId, actual);
            Assert.False(scrollBar.IsHandleCreated);
        }

        public static IEnumerable<object[]> HScrollBarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(HScrollBarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void HScrollBarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using HScrollBar scrollBar = new();
            scrollBar.AccessibleRole = role;

            object actual = scrollBar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void HScrollBarAccessibleObject_FragmentNavigate_Child_ReturnExpected(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
            var accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;

            Assert.Equal(accessibleObject.FirstLineButtonAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Equal(accessibleObject.LastLineButtonAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_BothButtonAreDisplayed_TestData))]
        public void HScrollBarAccessibleObject_GetChildCount_ReturnsFive_AllButtonsAreDisplayed(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;

            Assert.Equal(5, accessibleObject.GetChildCount());
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.HScrollBarAccessibleObject_FirstPageButtonIsHidden_TestData))]
        public void HScrollBarAccessibleObject_GetChildCount_ReturnsFour_FirstPageButtonIsHidden(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;

            Assert.Equal(4, accessibleObject.GetChildCount());
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.HScrollBarAccessibleObject_LastPageButtonIsHidden_TestData))]
        public void HScrollBarAccessibleObject_GetChildCount_ReturnsFour_LastPageButtonIsHidden(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;

            Assert.Equal(4, accessibleObject.GetChildCount());
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_MinimumEqualsMaximum_TestData))]
        public void HScrollBarAccessibleObject_GetChildCount_ReturnsThree_MinimumEqualsMaximum(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;

            Assert.Equal(3, accessibleObject.GetChildCount());
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void HScrollBarAccessibleObject_GetChildCount_ReturnsMunisOne_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
            ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;

            Assert.Equal(-1, accessibleObject.GetChildCount());
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_BothButtonAreDisplayed_TestData))]
        public void HScrollBarAccessibleObject_GetChild_ReturnExpected_AllButtonsAreDisplayed(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Equal(accessibleObject.FirstLineButtonAccessibleObject, accessibleObject.GetChild(0));
            Assert.Equal(accessibleObject.FirstPageButtonAccessibleObject, accessibleObject.GetChild(1));
            Assert.Equal(accessibleObject.ThumbAccessibleObject, accessibleObject.GetChild(2));
            Assert.Equal(accessibleObject.LastPageButtonAccessibleObject, accessibleObject.GetChild(3));
            Assert.Equal(accessibleObject.LastLineButtonAccessibleObject, accessibleObject.GetChild(4));
            Assert.Null(accessibleObject.GetChild(5));
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.HScrollBarAccessibleObject_FirstPageButtonIsHidden_TestData))]
        public void HScrollBarAccessibleObject_GetChild_ReturnExpected_FirstButtonIsHidden(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Equal(accessibleObject.FirstLineButtonAccessibleObject, accessibleObject.GetChild(0));
            Assert.Equal(accessibleObject.ThumbAccessibleObject, accessibleObject.GetChild(1));
            Assert.Equal(accessibleObject.LastPageButtonAccessibleObject, accessibleObject.GetChild(2));
            Assert.Equal(accessibleObject.LastLineButtonAccessibleObject, accessibleObject.GetChild(3));
            Assert.Null(accessibleObject.GetChild(4));
            Assert.Null(accessibleObject.GetChild(5));
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.HScrollBarAccessibleObject_LastPageButtonIsHidden_TestData))]
        public void HScrollBarAccessibleObject_GetChild_ReturnExpected_LastButtonIsHidden(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Equal(accessibleObject.FirstLineButtonAccessibleObject, accessibleObject.GetChild(0));
            Assert.Equal(accessibleObject.FirstPageButtonAccessibleObject, accessibleObject.GetChild(1));
            Assert.Equal(accessibleObject.ThumbAccessibleObject, accessibleObject.GetChild(2));
            Assert.Equal(accessibleObject.LastLineButtonAccessibleObject, accessibleObject.GetChild(3));
            Assert.Null(accessibleObject.GetChild(4));
            Assert.Null(accessibleObject.GetChild(5));
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_MinimumEqualsMaximum_TestData))]
        public void HScrollBarAccessibleObject_GetChild_ReturnExpected_MinimumEqualsMaximum(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Equal(accessibleObject.FirstLineButtonAccessibleObject, accessibleObject.GetChild(0));
            Assert.Equal(accessibleObject.ThumbAccessibleObject, accessibleObject.GetChild(1));
            Assert.Equal(accessibleObject.LastLineButtonAccessibleObject, accessibleObject.GetChild(2));
            Assert.Null(accessibleObject.GetChild(3));
            Assert.Null(accessibleObject.GetChild(4));
            Assert.Null(accessibleObject.GetChild(5));
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void HScrollBarAccessibleObject_GetChild_ReturnExpected_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
            ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Null(accessibleObject.GetChild(0));
            Assert.Null(accessibleObject.GetChild(1));
            Assert.Null(accessibleObject.GetChild(2));
            Assert.Null(accessibleObject.GetChild(3));
            Assert.Null(accessibleObject.GetChild(4));
            Assert.Null(accessibleObject.GetChild(5));
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void HScrollBarAccessibilityObject_GetPropertyValue_IsEnabledProperty_ReturnsExpected(bool enabled)
        {
            using HScrollBar scrollBar = new()
            {
                Enabled = enabled
            };

            Assert.Equal(enabled, scrollBar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.IsEnabledPropertyId));
            Assert.False(scrollBar.IsHandleCreated);
        }

        private HScrollBar GetHScrollBar(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            HScrollBar hScrollBar = new HScrollBar
            {
                RightToLeft = rightToLeft,
                Value = value,
                Minimum = minimum,
                Maximum = maximum,
                Size = new Size(200, 50)
            };

            if (createControl)
            {
                hScrollBar.CreateControl();
            }

            return hScrollBar;
        }
    }
}

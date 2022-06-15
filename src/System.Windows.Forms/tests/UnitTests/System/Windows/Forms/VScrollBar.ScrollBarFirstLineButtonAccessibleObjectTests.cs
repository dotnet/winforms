// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Xunit;
using static Interop;
using static System.Windows.Forms.ScrollBar;

namespace System.Windows.Forms.Tests
{
    public class VScrollBar_ScrollBarFirstLineButtonAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void VScrollBarFirstLineButtonAccessibleObject_GetChild_ReturnNull(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl, rightToLeft, minimum, maximum, value);
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Null(accessibleObject.GetChild(0));
            Assert.Null(accessibleObject.GetChild(1));
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void VScrollBarFirstLineButtonAccessibleObject_GetChildCount_ReturnMinusOne(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl, rightToLeft, minimum, maximum, value);
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.Equal(-1, accessibleObject.GetChildCount());
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void VScrollBarFirstLineButtonAccessibleObject_DefaultAction_ReturnNull_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.Null(accessibleObject.DefaultAction);
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void VScrollBarFirstLineButtonAccessibleObject_DefaultAction_ReturnNull_HandleIsCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.NotNull(accessibleObject.DefaultAction);
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void VScrollBarFirstLineButtonAccessibleObject_Description_ReturnNull_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.Null(accessibleObject.Description);
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void VScrollBarFirstLineButtonAccessibleObject_Description_ReturnNull_HandleIsCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.NotNull(accessibleObject.Description);
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void VScrollBarFirstLineButtonAccessibleObject_State_ReturnExpected(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl, rightToLeft, minimum, maximum, value);
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.Equal(AccessibleStates.None, accessibleObject.State);
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void VScrollBarFirstLineButtonAccessibleObject_Name_ReturnNull_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.Null(accessibleObject.Name);
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void VScrollBarFirstLineButtonAccessibleObject_Name_ReturnNull_HandleIsCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.NotNull(accessibleObject.Name);
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void VScrollBarFirstLineButtonAccessibleObject_Role_ReturnNotNull(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl, rightToLeft, minimum, maximum, value);
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);
            AccessibleRole expectedRole = createControl ? AccessibleRole.PushButton : AccessibleRole.None;

            Assert.Equal(expectedRole, accessibleObject.Role);
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void VScrollBarFirstLineButtonAccessibleObject_InvokePattern_Supported(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl, rightToLeft, minimum, maximum, value);
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.Equal(createControl, accessibleObject.IsPatternSupported(UiaCore.UIA.InvokePatternId));
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.VScrollBarAccessibleObject_FirstPageButtonIsDisplayed_TestData))]
        public void VScrollBarFirstLineButtonAccessibleObject_FragmentNavigate_ReturnExpected_FirstPageButtonIsDisplayed(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            var scrollBarAccessibleObject = (ScrollBar.ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.Equal(scrollBarAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.Equal(scrollBarAccessibleObject.FirstPageButtonAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.VScrollBarAccessibleObject_FirstPageButtonIsHidden_TestData))]
        public void VScrollBarFirstLineButtonAccessibleObject_FragmentNavigate_ReturnExpected_FirstPageButtonIsHidden(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            var scrollBarAccessibleObject = (ScrollBar.ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.Equal(scrollBarAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.Equal(scrollBarAccessibleObject.ThumbAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_MinimumEqualsMaximum_TestData))]
        public void VScrollBarFirstLineButtonAccessibleObject_FragmentNavigate_ReturnExpected_MinimumEqualsMaximum(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            var scrollBarAccessibleObject = (ScrollBar.ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.Equal(scrollBarAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.Equal(scrollBarAccessibleObject.ThumbAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void VScrollBarFirstLineButtonAccessibleObject_FragmentNavigate_ReturnNull_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
            var scrollBarAccessibleObject = (ScrollBar.ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(RightToLeft.Yes)]
        [InlineData(RightToLeft.No)]
        public void VScrollBarFirstLineButtonAccessibleObject_Invoke_DecreaseValue(RightToLeft rightToLeft)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum: 0, maximum: 100, value: 50);
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.Equal(50, scrollBar.Value);

            accessibleObject.Invoke();

            Assert.Equal(49, scrollBar.Value);

            accessibleObject.Invoke();

            Assert.Equal(48, scrollBar.Value);
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(RightToLeft.No, 0, 100)]
        [InlineData(RightToLeft.No, 0, 0)]
        [InlineData(RightToLeft.Yes, 0, 100)]
        [InlineData(RightToLeft.Yes, 0, 0)]
        public void VScrollBarFirstLineButtonAccessibleObject_Invoke_DoesNotWork_OutOfRange(RightToLeft rightToLeft, int value, int maximum)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum: 0, maximum, value);
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.Equal(value, scrollBar.Value);

            accessibleObject.Invoke();

            Assert.Equal(value, scrollBar.Value);
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void VScrollBarFirstLineButtonAccessibleObject_Invoke_DoesNotWork_IfHandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.Equal(value, scrollBar.Value);

            accessibleObject.Invoke();

            Assert.Equal(value, scrollBar.Value);
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_MinimumEqualsMaximum_TestData))]
        public void VScrollBarLineButtonAccessibleObject_Invoke_DoesNotWork_MinimumEqualsMaximum(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.Equal(value, scrollBar.Value);

            accessibleObject.Invoke();

            Assert.Equal(value, scrollBar.Value);
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void VScrollBarLineButtonAccessibleObject_GetPropertyValue_IsEnabledProperty_ReturnsTrue_OwnerEnabled(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl, rightToLeft, minimum, maximum, value);
            scrollBar.Enabled = true;
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.True((bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsEnabledPropertyId));
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void VScrollBarLineButtonAccessibleObject_GetPropertyValue_IsEnabledProperty_ReturnsFalse_OwnerDisabled(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl, rightToLeft, minimum, maximum, value);
            scrollBar.Enabled = false;
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.False((bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsEnabledPropertyId));
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void VScrollBarFirstLineButtonAccessibleObject_IsDisplayed_ReturnsTrue_OwnerVisible(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl, rightToLeft, minimum, maximum, value);
            scrollBar.Visible = true;
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.True(accessibleObject.IsDisplayed);
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void VScrollBarFirstLineButtonAccessibleObject_IsDisplayed_ReturnsFalse_OwnerInvisible(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl, rightToLeft, minimum, maximum, value);
            scrollBar.Visible = false;
            ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

            Assert.False(accessibleObject.IsDisplayed);
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        private VScrollBar GetVScrollBar(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            VScrollBar vScrollBar = new()
            {
                RightToLeft = rightToLeft,
                Value = value,
                Minimum = minimum,
                Maximum = maximum,
                SmallChange = 1,
                LargeChange = 2,
                Size = new Size(500, 50)
            };

            if (createControl)
            {
                vScrollBar.CreateControl();
            }

            return vScrollBar;
        }

        private ScrollBarFirstLineButtonAccessibleObject GetFirstLineButton(ScrollBar scrollBar)
        {
            var accessibleObject = (ScrollBar.ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            return accessibleObject.FirstLineButtonAccessibleObject;
        }
    }
}

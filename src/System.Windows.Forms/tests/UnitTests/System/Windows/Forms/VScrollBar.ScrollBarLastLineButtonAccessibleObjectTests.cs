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
    public class VScrollBar_ScrollBarLastLineButtonAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_GetChild_ReturnNull(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl, rightToLeft, minimum, maximum, value);
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Null(accessibleObject.GetChild(0));
            Assert.Null(accessibleObject.GetChild(1));
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_GetChildCount_ReturnMinusOne(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl, rightToLeft, minimum, maximum, value);
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.Equal(-1, accessibleObject.GetChildCount());
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_DefaultAction_ReturnNotNull_HandleIsCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.NotEmpty(accessibleObject.DefaultAction);
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_DefaultAction_ReturnNull_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.Null(accessibleObject.DefaultAction);
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_Description_ReturnNotNull_HandleIsCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.NotEmpty(accessibleObject.Description);
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_Description_ReturnNull_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.Null(accessibleObject.Description);
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_Name_ReturnNotNull_HandleIsCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.NotEmpty(accessibleObject.Name);
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_Name_ReturnNull_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.Null(accessibleObject.Name);
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_State_ReturnExpected(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl, rightToLeft, minimum, maximum, value);
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.Equal(AccessibleStates.None, accessibleObject.State);
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_Role_ReturnPushButton_HandleIsCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.Equal(AccessibleRole.PushButton, accessibleObject.Role);
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_Role_ReturnNone_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.Equal(AccessibleRole.None, accessibleObject.Role);
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_InvokePattern_Supported(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl, rightToLeft, minimum, maximum, value);
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.Equal(createControl, accessibleObject.IsPatternSupported(UiaCore.UIA.InvokePatternId));
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.VScrollBarAccessibleObject_LastPageButtonIsDisplayed_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_FragmentNavigate_ReturnsExpected_LastPageButtonIsDisplayed(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            var scrollBarAccessibleObject = (ScrollBar.ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.Equal(scrollBarAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.Equal(scrollBarAccessibleObject.LastPageButtonAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.VScrollBarAccessibleObject_LastPageButtonIsHidden_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_FragmentNavigate_ReturnsExpected_LastPageButtonIsHidden(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            var scrollBarAccessibleObject = (ScrollBar.ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.Equal(scrollBarAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.Equal(scrollBarAccessibleObject.ThumbAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_MinimumEqualsMaximum_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_FragmentNavigate_ReturnsExpected_MinimumEqualsMaximum(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            var scrollBarAccessibleObject = (ScrollBar.ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.Equal(scrollBarAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.Equal(scrollBarAccessibleObject.ThumbAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.VScrollBarAccessibleObject_LastPageButtonIsHidden_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_FragmentNavigate_ReturnsExpected_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
            var scrollBarAccessibleObject = (ScrollBar.ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

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
        public void VScrollBarLastLineButtonAccessibleObject_Invoke_IncreaseValue(RightToLeft rightToLeft)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum: 0, maximum: 100, value: 50);
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.Equal(50, scrollBar.Value);

            accessibleObject.Invoke();

            Assert.Equal(51, scrollBar.Value);

            accessibleObject.Invoke();

            Assert.Equal(52, scrollBar.Value);
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(RightToLeft.No, 99, 100)]
        [InlineData(RightToLeft.No, 0, 0)]
        [InlineData(RightToLeft.Yes, 99, 100)]
        [InlineData(RightToLeft.Yes, 0, 0)]
        public void VScrollBarLastLineButtonAccessibleObject_Invoke_DoesNotWork_OutOfRange(RightToLeft rightToLeft, int value, int maximum)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum: 0, maximum, value);
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.Equal(value, scrollBar.Value);

            accessibleObject.Invoke();

            Assert.Equal(value, scrollBar.Value);
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_Invoke_DoesNotWork_IfHandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.Equal(value, scrollBar.Value);

            accessibleObject.Invoke();

            Assert.Equal(value, scrollBar.Value);
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_MinimumEqualsMaximum_TestData))]
        public void VScrollBarLinePageButtonAccessibleObject_Invoke_DoesNotWork_MinimumEqualsMaximum(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.Equal(value, scrollBar.Value);

            accessibleObject.Invoke();

            Assert.Equal(value, scrollBar.Value);
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_GetPropertyValue_IsEnabledProperty_ReturnsTrue_OwnerEnabled(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl, rightToLeft, minimum, maximum, value);
            scrollBar.Enabled = true;
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.True((bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsEnabledPropertyId));
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_GetPropertyValue_IsEnabledProperty_ReturnsFalse_OwnerDisabled(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl, rightToLeft, minimum, maximum, value);
            scrollBar.Enabled = false;
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.False((bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsEnabledPropertyId));
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_IsDisplayed_ReturnsTrue_OwnerVisible(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl, rightToLeft, minimum, maximum, value);
            scrollBar.Visible = true;
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

            Assert.True(accessibleObject.IsDisplayed);
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void VScrollBarLastLineButtonAccessibleObject_IsDisplayed_ReturnsFalse_OwnerInvisible(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using VScrollBar scrollBar = GetVScrollBar(createControl, rightToLeft, minimum, maximum, value);
            scrollBar.Visible = false;
            ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

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
                Size = new Size(500, 50),
                SmallChange = 1,
                LargeChange = 2
            };

            if (createControl)
            {
                vScrollBar.CreateControl();
            }

            return vScrollBar;
        }

        private ScrollBarLastLineButtonAccessibleObject GetLastLineButton(ScrollBar scrollBar)
        {
            var accessibleObject = (ScrollBar.ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            return accessibleObject.LastLineButtonAccessibleObject;
        }
    }
}

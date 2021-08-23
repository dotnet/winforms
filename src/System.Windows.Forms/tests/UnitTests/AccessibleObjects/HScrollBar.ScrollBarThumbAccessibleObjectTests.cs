﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Xunit;
using static Interop;
using static System.Windows.Forms.ScrollBar;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class HScrollBar_ScrollBarThumbAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void HScrollBarThumbAccessibleObject_GetChild_ReturnNull(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
            ScrollBarThumbAccessibleObject accessibleObject = GetThumb(scrollBar);

            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Null(accessibleObject.GetChild(0));
            Assert.Null(accessibleObject.GetChild(1));
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void HScrollBarThumbAccessibleObject_GetChildCount_ReturnMinusOne(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
            ScrollBarThumbAccessibleObject accessibleObject = GetThumb(scrollBar);

            Assert.Equal(-1, accessibleObject.GetChildCount());
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void HScrollBarThumbAccessibleObject_DefaultAction_ReturnNotNull(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
            ScrollBarThumbAccessibleObject accessibleObject = GetThumb(scrollBar);

            Assert.Equal(string.Empty, accessibleObject.DefaultAction);
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void HScrollBarThumbAccessibleObject_Description_ReturnNotNull_HandleIsCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarThumbAccessibleObject accessibleObject = GetThumb(scrollBar);

            Assert.NotEmpty(accessibleObject.Description);
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void HScrollBarThumbAccessibleObject_Description_ReturnNull_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
            ScrollBarThumbAccessibleObject accessibleObject = GetThumb(scrollBar);

            Assert.Null(accessibleObject.Description);
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void HScrollBarThumbAccessibleObject_Name_ReturnNotNull_HandleIsCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarThumbAccessibleObject accessibleObject = GetThumb(scrollBar);

            Assert.NotEmpty(accessibleObject.Name);
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void HScrollBarThumbAccessibleObject_Name_ReturnNull_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
            ScrollBarThumbAccessibleObject accessibleObject = GetThumb(scrollBar);

            Assert.Null(accessibleObject.Name);
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void HScrollBarThumbAccessibleObject_Role_ReturnIndicator_HandleIsCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            ScrollBarThumbAccessibleObject accessibleObject = GetThumb(scrollBar);

            Assert.Equal(AccessibleRole.Indicator, accessibleObject.Role);
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void HScrollBarThumbAccessibleObject_Role_ReturnNone_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
            ScrollBarThumbAccessibleObject accessibleObject = GetThumb(scrollBar);

            Assert.Equal(AccessibleRole.None, accessibleObject.Role);
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void HScrollBarThumbAccessibleObject_State_ReturnExpected(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
            ScrollBarThumbAccessibleObject accessibleObject = GetThumb(scrollBar);

            Assert.Equal(AccessibleStates.None, accessibleObject.State);
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void HScrollBarThumbAccessibleObject_InvokePattern_Supported(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
            ScrollBarThumbAccessibleObject accessibleObject = GetThumb(scrollBar);

            Assert.False(accessibleObject.IsPatternSupported(UiaCore.UIA.InvokePatternId));
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_BothButtonAreDisplayed_TestData))]
        public void HScrollBarThumbAccessibleObject_FragmentNavigate_ReturnExpected_BothPageButtonsAreDisplayed(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            var scrollBarAccessibleObject = (ScrollBar.ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            ScrollBarThumbAccessibleObject accessibleObject = scrollBarAccessibleObject.ThumbAccessibleObject;

            Assert.Equal(scrollBarAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.Equal(scrollBarAccessibleObject.FirstPageButtonAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(scrollBarAccessibleObject.LastPageButtonAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.HScrollBarAccessibleObject_FirstPageButtonIsHidden_TestData))]
        public void HScrollBarThumbAccessibleObject_FragmentNavigate_ReturnExpected_FirstPageButtonIsHidden(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            var scrollBarAccessibleObject = (ScrollBar.ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            ScrollBarThumbAccessibleObject accessibleObject = scrollBarAccessibleObject.ThumbAccessibleObject;

            Assert.Equal(scrollBarAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.Equal(scrollBarAccessibleObject.FirstLineButtonAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(scrollBarAccessibleObject.LastPageButtonAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.HScrollBarAccessibleObject_LastPageButtonIsHidden_TestData))]
        public void HScrollBarThumbAccessibleObject_FragmentNavigate_ReturnExpected_LastPageButtonIsHidden(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            var scrollBarAccessibleObject = (ScrollBar.ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            ScrollBarThumbAccessibleObject accessibleObject = scrollBarAccessibleObject.ThumbAccessibleObject;

            Assert.Equal(scrollBarAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.Equal(scrollBarAccessibleObject.FirstPageButtonAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(scrollBarAccessibleObject.LastLineButtonAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_MinimumEqualsMaximum_TestData))]
        public void HScrollBarThumbAccessibleObject_FragmentNavigate_ReturnExpected_MinimumEqualsMaximum(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
            var scrollBarAccessibleObject = (ScrollBar.ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            ScrollBarThumbAccessibleObject accessibleObject = scrollBarAccessibleObject.ThumbAccessibleObject;

            Assert.Equal(scrollBarAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.Equal(scrollBarAccessibleObject.FirstLineButtonAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(scrollBarAccessibleObject.LastLineButtonAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
        public void HScrollBarThumbAccessibleObject_FragmentNavigate_ReturnNull_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
            ScrollBarThumbAccessibleObject accessibleObject = GetThumb(scrollBar);

            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void HScrollBarThumbAccessibleObject_GetPropertyValue_IsEnabledProperty_ReturnsTrue_OwnerEnabled(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
            scrollBar.Enabled = true;
            ScrollBarThumbAccessibleObject accessibleObject = GetThumb(scrollBar);

            Assert.True((bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsEnabledPropertyId));
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void HScrollBarThumbAccessibleObject_GetPropertyValue_IsEnabledProperty_ReturnsFalse_OwnerDisabled(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
            scrollBar.Enabled = false;
            ScrollBarThumbAccessibleObject accessibleObject = GetThumb(scrollBar);

            Assert.False((bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsEnabledPropertyId));
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void HScrollBarThumbAccessibleObject_IsDisplayed_ReturnsTrue_OwnerVisible(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
            scrollBar.Visible = true;
            ScrollBarThumbAccessibleObject accessibleObject = GetThumb(scrollBar);

            Assert.True(accessibleObject.IsDisplayed);
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
        public void HScrollBarThumbAccessibleObject_IsDisplayed_ReturnsFalse_OwnerInvisible(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
            scrollBar.Visible = false;
            ScrollBarThumbAccessibleObject accessibleObject = GetThumb(scrollBar);

            Assert.False(accessibleObject.IsDisplayed);
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        private HScrollBar GetHScrollBar(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            HScrollBar hScrollBar = new()
            {
                RightToLeft = rightToLeft,
                Value = value,
                Minimum = minimum,
                Maximum = maximum,
                Size = new Size(200, 50),
                SmallChange = 1,
                LargeChange = 2
            };

            if (createControl)
            {
                hScrollBar.CreateControl();
            }

            return hScrollBar;
        }

        private ScrollBarThumbAccessibleObject GetThumb(ScrollBar scrollBar)
        {
            var accessibleObject = (ScrollBar.ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
            return accessibleObject.ThumbAccessibleObject;
        }
    }
}

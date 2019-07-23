// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ScrollPropertiesTests
    {
        public static IEnumerable<object[]> Ctor_ScrollableControl_TestData()
        {
            yield return new object[] { new ScrollableControl() };
            yield return new object[] { null };
        }

        [Theory]
        [MemberData(nameof(Ctor_ScrollableControl_TestData))]
        public void ScrollProperties_Ctor_Control(ScrollableControl container)
        {
            var properties = new SubScrollProperties(container);
            Assert.Same(container, properties.ParentControl);
            Assert.True(properties.Enabled);
            Assert.Equal(10, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);
            Assert.Equal(100, properties.Maximum);
            Assert.Equal(0, properties.Minimum);
            Assert.Equal(0, properties.Value);
            Assert.False(properties.Visible);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollProperties_Enabled_Set_GetReturnsExpected(bool value)
        {
            var container = new ScrollableControl();
            var properties = new SubScrollProperties(container)
            {
                Enabled = value
            };
            Assert.Equal(value, properties.Enabled);

            // Set same.
            properties.Enabled = value;
            Assert.Equal(value, properties.Enabled);

            // Set different.
            properties.Enabled = !value;
            Assert.Equal(!value, properties.Enabled);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollProperties_Enabled_SetAutoScrollContainer_Nop(bool value)
        {
            var container = new ScrollableControl()
            {
                AutoScroll = true
            };
            var properties = new SubScrollProperties(container)
            {
                Enabled = value
            };
            Assert.True(properties.Enabled);

            // Set same.
            properties.Enabled = value;
            Assert.True(properties.Enabled);

            // Set different.
            properties.Enabled = !value;
            Assert.True(properties.Enabled);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollProperties_Enabled_SetNullContainer_GetReturnsExpected(bool value)
        {
            var properties = new SubScrollProperties(null)
            {
                Enabled = value
            };
            Assert.Equal(value, properties.Enabled);

            // Set same.
            properties.Enabled = value;
            Assert.Equal(value, properties.Enabled);

            // Set different.
            properties.Enabled = !value;
            Assert.Equal(!value, properties.Enabled);
        }

        public static IEnumerable<object[]> LargeChange_Set_TestData()
        {
            yield return new object[] { 10 };
            yield return new object[] { 12 };
        }

        [Theory]
        [MemberData(nameof(LargeChange_Set_TestData))]
        public void ScrollProperties_LargeChange_Set_GetReturnsExpected(int value)
        {
            var container = new ScrollableControl();
            var properties = new SubScrollProperties(container)
            {
                LargeChange = value
            };
            Assert.Equal(value, properties.LargeChange);

            // Set same.
            properties.LargeChange = value;
            Assert.Equal(value, properties.LargeChange);
        }

        [Theory]
        [MemberData(nameof(LargeChange_Set_TestData))]
        public void ScrollProperties_LargeChange_SetAutoScrollContainer_GetReturnsExpected(int value)
        {
            var container = new ScrollableControl
            {
                AutoScroll = true
            };
            var properties = new SubScrollProperties(container)
            {
                LargeChange = value
            };
            Assert.Equal(value, properties.LargeChange);

            // Set same.
            properties.LargeChange = value;
            Assert.Equal(value, properties.LargeChange);
        }

        [Theory]
        [MemberData(nameof(LargeChange_Set_TestData))]
        public void ScrollProperties_LargeChange_SetNullContainer_GetReturnsExpected(int value)
        {
            var properties = new SubScrollProperties(null)
            {
                LargeChange = value
            };
            Assert.Equal(value, properties.LargeChange);

            // Set same.
            properties.LargeChange = value;
            Assert.Equal(value, properties.LargeChange);
        }

        [Fact]
        public void ScrollProperties_LargeChange_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            var container = new ScrollableControl();
            var properties = new SubScrollProperties(container);
            Assert.Throws<ArgumentOutOfRangeException>("value", () => properties.LargeChange = -1);
            Assert.Equal(10, properties.LargeChange);
        }

        public static IEnumerable<object[]> SmallChange_Set_TestData()
        {
            yield return new object[] { 1, 1 };
            yield return new object[] { 8, 8 };
            yield return new object[] { 10, 10 };
            yield return new object[] { 12, 10 };
        }

        [Theory]
        [MemberData(nameof(SmallChange_Set_TestData))]
        public void ScrollProperties_SmallChange_Set_GetReturnsExpected(int value, int expectedValue)
        {
            var container = new ScrollableControl();
            var properties = new SubScrollProperties(container)
            {
                SmallChange = value
            };
            Assert.Equal(expectedValue, properties.SmallChange);

            // Set same.
            properties.SmallChange = value;
            Assert.Equal(expectedValue, properties.SmallChange);
        }

        [Theory]
        [MemberData(nameof(SmallChange_Set_TestData))]
        public void ScrollProperties_SmallChange_SetAutoScrollContainer_GetReturnsExpected(int value, int expectedValue)
        {
            var container = new ScrollableControl
            {
                AutoScroll = true
            };
            var properties = new SubScrollProperties(container)
            {
                SmallChange = value
            };
            Assert.Equal(expectedValue, properties.SmallChange);

            // Set same.
            properties.SmallChange = value;
            Assert.Equal(expectedValue, properties.SmallChange);
        }

        [Theory]
        [MemberData(nameof(SmallChange_Set_TestData))]
        public void ScrollProperties_SmallChange_SetNullContainer_ReturnsExpected(int value, int expectedValue)
        {
            var properties = new SubScrollProperties(null)
            {
                SmallChange = value
            };
            Assert.Equal(expectedValue, properties.SmallChange);

            // Set same.
            properties.SmallChange = value;
            Assert.Equal(expectedValue, properties.SmallChange);
        }

        [Fact]
        public void ScrollProperties_SmallChange_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            var container = new ScrollableControl();
            var properties = new SubScrollProperties(container);
            Assert.Throws<ArgumentOutOfRangeException>("value", () => properties.SmallChange = -1);
            Assert.Equal(1, properties.SmallChange);
        }

        public static IEnumerable<object[]> Maximum_Set_TestData()
        {
            yield return new object[] { 0, 1 };
            yield return new object[] { 8, 9 };
            yield return new object[] { 10, 10 };
            yield return new object[] { 50, 10 };
        }

        [Theory]
        [MemberData(nameof(Maximum_Set_TestData))]
        public void ScrollProperties_Maximum_Set_GetReturnsExpected(int value, int expectedLargeChange)
        {
            var container = new ScrollableControl();
            var properties = new SubScrollProperties(container)
            {
                Maximum = value
            };
            Assert.Equal(value, properties.Maximum);
            Assert.Equal(0, properties.Minimum);
            Assert.Equal(0, properties.Value);
            Assert.Equal(expectedLargeChange, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);

            // Set value.
            properties.Maximum = value;
            Assert.Equal(value, properties.Maximum);
            Assert.Equal(0, properties.Minimum);
            Assert.Equal(0, properties.Value);
            Assert.Equal(expectedLargeChange, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);
        }

        [Theory]
        [MemberData(nameof(Maximum_Set_TestData))]
        public void ScrollProperties_Maximum_SetNullContainer_GetReturnsExpected(int value, int expectedLargeChange)
        {
            var properties = new SubScrollProperties(null)
            {
                Maximum = value
            };
            Assert.Equal(value, properties.Maximum);
            Assert.Equal(0, properties.Minimum);
            Assert.Equal(0, properties.Value);
            Assert.Equal(expectedLargeChange, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);

            // Set value.
            properties.Maximum = value;
            Assert.Equal(value, properties.Maximum);
            Assert.Equal(0, properties.Minimum);
            Assert.Equal(0, properties.Value);
            Assert.Equal(expectedLargeChange, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);
        }

        [Fact]
        public void ScrollProperties_Maximum_SetLessThanValueAndMinimum_SetsValueAndMinimum()
        {
            var container = new ScrollableControl();
            var properties = new SubScrollProperties(container)
            {
                Value = 10,
                Minimum = 8,
                Maximum = 5
            };
            Assert.Equal(5, properties.Maximum);
            Assert.Equal(5, properties.Minimum);
            Assert.Equal(5, properties.Value);
            Assert.Equal(1, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);
        }

        [Fact]
        public void ScrollProperties_Maximum_SetNegative_SetsValueAndMinimum()
        {
            var container = new ScrollableControl();
            var properties = new SubScrollProperties(container)
            {
                Maximum = -1
            };
            Assert.Equal(-1, properties.Maximum);
            Assert.Equal(-1, properties.Minimum);
            Assert.Equal(-1, properties.Value);
            Assert.Equal(1, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(8)]
        public void ScrollProperties_Maximum_SetAutoScrollContainer_Nop(int value)
        {
            var container = new ScrollableControl()
            {
                AutoScroll = true
            };
            var properties = new SubScrollProperties(container)
            {
                Maximum = value
            };
            Assert.Equal(100, properties.Maximum);
            Assert.Equal(0, properties.Minimum);
            Assert.Equal(0, properties.Value);
            Assert.Equal(10, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);
        }

        public static IEnumerable<object[]> Minimum_Set_TestData()
        {
            yield return new object[] { 0 };
            yield return new object[] { 5 };
        }

        [Theory]
        [MemberData(nameof(Minimum_Set_TestData))]
        public void ScrollProperties_Minimum_Set_GetReturnsExpected(int value)
        {
            var container = new ScrollableControl();
            var properties = new SubScrollProperties(container)
            {
                Value = 5,
                Minimum = value
            };
            Assert.Equal(100, properties.Maximum);
            Assert.Equal(value, properties.Minimum);
            Assert.Equal(5, properties.Value);
            Assert.Equal(10, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);

            // Set same.
            properties.Minimum = value;
            Assert.Equal(100, properties.Maximum);
            Assert.Equal(value, properties.Minimum);
            Assert.Equal(5, properties.Value);
            Assert.Equal(10, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);
        }

        [Fact]
        public void ScrollProperties_Minimum_SetGreaterThanValueAndMaximum_SetsValueAndMinimum()
        {
            var container = new ScrollableControl();
            var properties = new SubScrollProperties(container)
            {
                Value = 10,
                Maximum = 8,
                Minimum = 12
            };
            Assert.Equal(12, properties.Maximum);
            Assert.Equal(12, properties.Minimum);
            Assert.Equal(12, properties.Value);
            Assert.Equal(1, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(8)]
        public void ScrollProperties_Minimum_SetAutoScrollContainer_Nop(int value)
        {
            var container = new ScrollableControl
            {
                AutoScroll = true
            };
            var properties = new SubScrollProperties(container)
            {
                Minimum = value
            };
            Assert.Equal(100, properties.Maximum);
            Assert.Equal(0, properties.Minimum);
            Assert.Equal(0, properties.Value);
            Assert.Equal(10, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);
        }

        [Theory]
        [MemberData(nameof(Minimum_Set_TestData))]
        public void ScrollProperties_Minimum_SetNullContainer_GetReturnsExpected(int value)
        {
            var properties = new SubScrollProperties(null)
            {
                Value = 5,
                Minimum = value
            };
            Assert.Equal(100, properties.Maximum);
            Assert.Equal(value, properties.Minimum);
            Assert.Equal(5, properties.Value);
            Assert.Equal(10, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);

            // Set same.
            properties.Minimum = value;
            Assert.Equal(100, properties.Maximum);
            Assert.Equal(value, properties.Minimum);
            Assert.Equal(5, properties.Value);
            Assert.Equal(10, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);
        }

        [Fact]
        public void ScrollProperties_Minimum_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            var container = new ScrollableControl();
            var properties = new SubScrollProperties(container);
            Assert.Throws<ArgumentOutOfRangeException>("value", () => properties.Minimum = -1);
            Assert.Equal(0, properties.Minimum);
        }

        public static IEnumerable<object[]> Value_Set_TestData()
        {
            yield return new object[] { 0 };
            yield return new object[] { 5 };
            yield return new object[] { 100 };
        }

        [Theory]
        [MemberData(nameof(Value_Set_TestData))]
        public void ScrollProperties_Value_Set_GetReturnsExpected(int value)
        {
            var container = new ScrollableControl();
            var properties = new SubScrollProperties(container)
            {
                Value = value
            };
            Assert.Equal(100, properties.Maximum);
            Assert.Equal(0, properties.Minimum);
            Assert.Equal(value, properties.Value);
            Assert.Equal(10, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);

            // Set same.
            properties.Value = value;
            Assert.Equal(100, properties.Maximum);
            Assert.Equal(0, properties.Minimum);
            Assert.Equal(value, properties.Value);
            Assert.Equal(10, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(100)]
        public void ScrollProperties_Value_SetAutoScrollContainer_GetReturnsExpected(int value)
        {
            var container = new ScrollableControl
            {
                AutoScroll = true
            };
            var properties = new SubScrollProperties(container)
            {
                Value = value
            };
            Assert.Equal(100, properties.Maximum);
            Assert.Equal(0, properties.Minimum);
            Assert.Equal(value, properties.Value);
            Assert.Equal(10, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);
        }

        [Theory]
        [MemberData(nameof(Value_Set_TestData))]
        public void ScrollProperties_Value_SetNullContainer_GetReturnsExpected(int value)
        {
            var properties = new SubScrollProperties(null)
            {
                Value = value
            };
            Assert.Equal(100, properties.Maximum);
            Assert.Equal(0, properties.Minimum);
            Assert.Equal(value, properties.Value);
            Assert.Equal(10, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);

            // Set same.
            properties.Value = value;
            Assert.Equal(100, properties.Maximum);
            Assert.Equal(0, properties.Minimum);
            Assert.Equal(value, properties.Value);
            Assert.Equal(10, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void ScrollProperties_Value_SetOutOfRange_ThrowsArgumentOutOfRangeException(int value)
        {
            var container = new ScrollableControl();
            var properties = new SubScrollProperties(container);
            Assert.Throws<ArgumentOutOfRangeException>("value", () => properties.Value = value);
            Assert.Equal(0, properties.Value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollProperties_Visible_Set_GetReturnsExpected(bool value)
        {
            var container = new ScrollableControl();
            var properties = new SubScrollProperties(container)
            {
                Visible = value
            };
            Assert.Equal(value, properties.Visible);

            // Set same.
            properties.Visible = value;
            Assert.Equal(value, properties.Visible);

            // Set different.
            properties.Visible = !value;
            Assert.Equal(!value, properties.Visible);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollProperties_Visible_SetAutoScrollContainer_Nop(bool value)
        {
            var container = new ScrollableControl()
            {
                AutoScroll = true
            };
            var properties = new SubScrollProperties(container)
            {
                Visible = value
            };
            Assert.False(properties.Visible);

            // Set same.
            properties.Visible = value;
            Assert.False(properties.Visible);

            // Set different.
            properties.Visible = !value;
            Assert.False(properties.Visible);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollProperties_Visible_SetNullContainer_GetReturnsExpected(bool value)
        {
            var properties = new SubScrollProperties(null)
            {
                Visible = value
            };
            Assert.Equal(value, properties.Visible);

            // Set same.
            properties.Visible = value;
            Assert.Equal(value, properties.Visible);

            // Set different.
            properties.Visible = !value;
            Assert.Equal(!value, properties.Visible);
        }

        private class SubScrollProperties : HScrollProperties
        {
            public SubScrollProperties(ScrollableControl container) : base(container)
            {
            }

            public new ScrollableControl ParentControl => base.ParentControl;
        }
    }
}

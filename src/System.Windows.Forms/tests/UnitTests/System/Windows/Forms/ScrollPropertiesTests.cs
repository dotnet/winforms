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
            Assert.Equal(container, properties.ParentControlEntry);
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
            var control = new ScrollableControl();
            var properties = new SubScrollProperties(control)
            {
                Enabled = value
            };
            Assert.Equal(value, properties.Enabled);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollProperties_Enabled_SetAutoScrollContainer_Nop(bool value)
        {
            var control = new ScrollableControl()
            {
                AutoScroll = true
            };
            var properties = new SubScrollProperties(control)
            {
                Enabled = value
            };
            Assert.True(properties.Enabled);
        }

        [Fact]
        public void ScrollProperties_Enabled_SetNullContainer_ThrowsNullReferenceException()
        {
            var properties = new SubScrollProperties(null);
            Assert.Throws<NullReferenceException>(() => properties.Enabled = false);
            Assert.True(properties.Enabled);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(12)]
        public void ScrollProperties_LargeChange_Set_GetReturnsExpected(int value)
        {
            var control = new ScrollableControl();
            var properties = new SubScrollProperties(control)
            {
                LargeChange = value
            };
            Assert.Equal(value, properties.LargeChange);

            // Set same.
            properties.LargeChange = value;
            Assert.Equal(value, properties.LargeChange);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(12)]
        public void ScrollProperties_LargeChange_SetAutoScrollContainer_GetReturnsExpected(int value)
        {
            var control = new ScrollableControl
            {
                AutoScroll = true
            };
            var properties = new SubScrollProperties(control)
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
            var control = new ScrollableControl();
            var properties = new SubScrollProperties(control);
            Assert.Throws<ArgumentOutOfRangeException>("value", () => properties.LargeChange = -1);
            Assert.Equal(10, properties.LargeChange);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(8, 8)]
        [InlineData(10, 10)]
        [InlineData(12, 10)]
        public void ScrollProperties_SmallChange_Set_GetReturnsExpected(int value, int expectedValue)
        {
            var control = new ScrollableControl();
            var properties = new SubScrollProperties(control)
            {
                SmallChange = value
            };
            Assert.Equal(expectedValue, properties.SmallChange);

            // Set same.
            properties.SmallChange = value;
            Assert.Equal(expectedValue, properties.SmallChange);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(8, 8)]
        [InlineData(10, 10)]
        [InlineData(12, 10)]
        public void ScrollProperties_SmallChange_SetAutoScrollContainer_GetReturnsExpected(int value, int expectedValue)
        {
            var control = new ScrollableControl
            {
                AutoScroll = true
            };
            var properties = new SubScrollProperties(control)
            {
                SmallChange = value
            };
            Assert.Equal(expectedValue, properties.SmallChange);

            // Set same.
            properties.SmallChange = value;
            Assert.Equal(expectedValue, properties.SmallChange);
        }

        [Fact]
        public void ScrollProperties_SmallChange_SetNullContainer_ThrowsNullReferenceException()
        {
            var properties = new SubScrollProperties(null);
            Assert.Throws<NullReferenceException>(() => properties.SmallChange = 10);
        }

        [Fact]
        public void ScrollProperties_SmallChange_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            var control = new ScrollableControl();
            var properties = new SubScrollProperties(control);
            Assert.Throws<ArgumentOutOfRangeException>("value", () => properties.SmallChange = -1);
            Assert.Equal(1, properties.SmallChange);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(8, 9)]
        [InlineData(10, 10)]
        [InlineData(50, 10)]
        public void ScrollProperties_Maximum_Set_GetReturnsExpected(int value, int expectedLargeChange)
        {
            var control = new ScrollableControl();
            var properties = new SubScrollProperties(control)
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
            var control = new ScrollableControl();
            var properties = new SubScrollProperties(control)
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
            var control = new ScrollableControl();
            var properties = new SubScrollProperties(control)
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
            var control = new ScrollableControl()
            {
                AutoScroll = true
            };
            var properties = new SubScrollProperties(control)
            {
                Maximum = value
            };
            Assert.Equal(100, properties.Maximum);
            Assert.Equal(0, properties.Minimum);
            Assert.Equal(0, properties.Value);
            Assert.Equal(10, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);
        }

        [Fact]
        public void ScrollProperties_Maximum_SetNullContainer_ThrowsNullReferenceException()
        {
            var properties = new SubScrollProperties(null);
            Assert.Throws<NullReferenceException>(() => properties.Maximum = 10);
            Assert.Equal(100, properties.Maximum);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        public void ScrollProperties_Minimum_Set_GetReturnsExpected(int value)
        {
            var control = new ScrollableControl();
            var properties = new SubScrollProperties(control)
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
            var control = new ScrollableControl();
            var properties = new SubScrollProperties(control)
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
            var control = new ScrollableControl
            {
                AutoScroll = true
            };
            var properties = new SubScrollProperties(control)
            {
                Minimum = value
            };
            Assert.Equal(100, properties.Maximum);
            Assert.Equal(0, properties.Minimum);
            Assert.Equal(0, properties.Value);
            Assert.Equal(10, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);
        }

        [Fact]
        public void ScrollProperties_Minimum_SetNullContainer_ThrowsNullReferenceException()
        {
            var properties = new SubScrollProperties(null);
            Assert.Throws<NullReferenceException>(() => properties.Minimum = 10);
            Assert.Equal(0, properties.Minimum);
        }

        [Fact]
        public void ScrollProperties_Minimum_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            var control = new ScrollableControl();
            var properties = new SubScrollProperties(control);
            Assert.Throws<ArgumentOutOfRangeException>("value", () => properties.Minimum = -1);
            Assert.Equal(0, properties.Minimum);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(100)]
        public void ScrollProperties_Value_Set_GetReturnsExpected(int value)
        {
            var control = new ScrollableControl();
            var properties = new SubScrollProperties(control)
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
            var control = new ScrollableControl
            {
                AutoScroll = true
            };
            var properties = new SubScrollProperties(control)
            {
                Value = value
            };
            Assert.Equal(100, properties.Maximum);
            Assert.Equal(0, properties.Minimum);
            Assert.Equal(value, properties.Value);
            Assert.Equal(10, properties.LargeChange);
            Assert.Equal(1, properties.SmallChange);
        }

        [Fact]
        public void ScrollProperties_Value_SetNullContainer_ThrowsNullReferenceException()
        {
            var properties = new SubScrollProperties(null);
            Assert.Throws<NullReferenceException>(() => properties.Value = 10);
            Assert.Equal(10, properties.Value);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void ScrollProperties_Value_SetOutOfRange_ThrowsArgumentOutOfRangeException(int value)
        {
            var control = new ScrollableControl();
            var properties = new SubScrollProperties(control);
            Assert.Throws<ArgumentOutOfRangeException>("value", () => properties.Value = value);
            Assert.Equal(0, properties.Value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollProperties_Visible_Set_GetReturnsExpected(bool value)
        {
            var control = new ScrollableControl();
            var properties = new SubScrollProperties(control)
            {
                Visible = value
            };
            Assert.Equal(value, properties.Visible);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollProperties_Visible_SetAutoScrollContainer_Nop(bool value)
        {
            var control = new ScrollableControl()
            {
                AutoScroll = true
            };
            var properties = new SubScrollProperties(control)
            {
                Visible = value
            };
            Assert.False(properties.Visible);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollProperties_Visible_SetNullContainer_ThrowsNullReferenceException(bool value)
        {
            var properties = new SubScrollProperties(null);
            Assert.Throws<NullReferenceException>(() => properties.Visible = value);
            Assert.False(properties.Visible);
        }

        private class SubScrollProperties : HScrollProperties
        {
            public SubScrollProperties(ScrollableControl container) : base(container)
            {
            }

            public ScrollableControl ParentControlEntry => ParentControl;
        }
    }
}

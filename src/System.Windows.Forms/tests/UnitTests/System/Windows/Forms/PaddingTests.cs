// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class PaddingTests
    {
        [Fact]
        public void Padding_Ctor_Default()
        {
            var padding = new Padding();
            Assert.Equal(-1, padding.All);
            Assert.Equal(0, padding.Left);
            Assert.Equal(0, padding.Top);
            Assert.Equal(0, padding.Right);
            Assert.Equal(0, padding.Bottom);
            Assert.Equal(0, padding.Horizontal);
            Assert.Equal(0, padding.Vertical);
            Assert.Equal(Size.Empty, padding.Size);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void Padding_Ctor_Int(int all)
        {
            var padding = new Padding(all);
            Assert.Equal(all, padding.All);
            Assert.Equal(all, padding.Left);
            Assert.Equal(all, padding.Top);
            Assert.Equal(all, padding.Right);
            Assert.Equal(all, padding.Bottom);
            Assert.Equal(all * 2, padding.Horizontal);
            Assert.Equal(all * 2, padding.Vertical);
            Assert.Equal(new Size(all * 2, all * 2), padding.Size);
        }

        [Theory]
        [InlineData(-1, -2, -3, -4, -1)]
        [InlineData(0, 0, 0, 0, 0)]
        [InlineData(1, 1, 1, 1, 1)]
        [InlineData(2, 1, 1, 1, -1)]
        [InlineData(1, 2, 1, 1, -1)]
        [InlineData(1, 1, 2, 1, -1)]
        [InlineData(1, 1, 1, 2, -1)]
        [InlineData(1, 2, 3, 4, -1)]
        public void Padding_Ctor_Int_Int_Int_Int(int left, int top, int right, int bottom, int expectedAll)
        {
            var padding = new Padding(left, top, right, bottom);
            Assert.Equal(expectedAll, padding.All);
            Assert.Equal(left, padding.Left);
            Assert.Equal(top, padding.Top);
            Assert.Equal(right, padding.Right);
            Assert.Equal(bottom, padding.Bottom);
            Assert.Equal(right + left, padding.Horizontal);
            Assert.Equal(top + bottom, padding.Vertical);
            Assert.Equal(new Size(right + left, top + bottom), padding.Size);
        }

        [Fact]
        public void Padding_Empty_Get_ReturnsExpected()
        {
            Padding padding = Padding.Empty;
            Assert.Equal(0, padding.All);
            Assert.Equal(0, padding.Left);
            Assert.Equal(0, padding.Top);
            Assert.Equal(0, padding.Right);
            Assert.Equal(0, padding.Bottom);
            Assert.Equal(0, padding.Horizontal);
            Assert.Equal(0, padding.Vertical);
            Assert.Equal(Size.Empty, padding.Size);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void Padding_All_Set_GetReturnsExpected(int value)
        {
            var padding = new Padding(1, 2, 3, 4)
            {
                All = value
            };
            Assert.Equal(value, padding.All);
            Assert.Equal(value, padding.Left);
            Assert.Equal(value, padding.Top);
            Assert.Equal(value, padding.Right);
            Assert.Equal(value, padding.Bottom);
        }

        [Fact]
        public void Padding_AllPropertyDescriptor_ResetValue_SetsToZero()
        {
            var padding = new Padding(1, 2, 3, 4);
            object boxedPadding = padding;
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(padding);
            PropertyDescriptor property = properties[nameof(Padding.All)];
            Assert.False(property.CanResetValue(boxedPadding));
            Assert.False(property.ShouldSerializeValue(boxedPadding));
            property.ResetValue(boxedPadding);

            Assert.Equal(0, ((Padding)boxedPadding).All);
            Assert.Equal(0, ((Padding)boxedPadding).Left);
            Assert.Equal(0, ((Padding)boxedPadding).Top);
            Assert.Equal(0, ((Padding)boxedPadding).Right);
            Assert.Equal(0, ((Padding)boxedPadding).Bottom);
            Assert.True(property.CanResetValue(boxedPadding));
            Assert.True(property.ShouldSerializeValue(boxedPadding));
        }

        [Fact]
        public void Padding_AllPropertyDescriptor_ResetValueOnAll_SetsToZero()
        {
            var padding = new Padding(1);
            object boxedPadding = padding;
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(padding);
            PropertyDescriptor property = properties[nameof(Padding.All)];
            Assert.True(property.CanResetValue(boxedPadding));
            Assert.True(property.ShouldSerializeValue(boxedPadding));
            property.ResetValue(boxedPadding);

            Assert.Equal(0, ((Padding)boxedPadding).All);
            Assert.Equal(0, ((Padding)boxedPadding).Left);
            Assert.Equal(0, ((Padding)boxedPadding).Top);
            Assert.Equal(0, ((Padding)boxedPadding).Right);
            Assert.Equal(0, ((Padding)boxedPadding).Bottom);
            Assert.True(property.CanResetValue(boxedPadding));
            Assert.True(property.ShouldSerializeValue(boxedPadding));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void Padding_All_SetOnAll_GetReturnsExpected(int value)
        {
            var padding = new Padding(2)
            {
                All = value
            };
            Assert.Equal(value, padding.All);
            Assert.Equal(value, padding.Left);
            Assert.Equal(value, padding.Top);
            Assert.Equal(value, padding.Right);
            Assert.Equal(value, padding.Bottom);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void Padding_Left_Set_GetReturnsExpected(int value)
        {
            var padding = new Padding(1, 2, 3, 4)
            {
                Left = value
            };
            Assert.Equal(-1, padding.All);
            Assert.Equal(value, padding.Left);
            Assert.Equal(2, padding.Top);
            Assert.Equal(3, padding.Right);
            Assert.Equal(4, padding.Bottom);
        }

        [Fact]
        public void Padding_LeftPropertyDescriptor_ResetValue_SetsToZero()
        {
            var padding = new Padding(1, 2, 3, 4);
            object boxedPadding = padding;
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(padding);
            PropertyDescriptor property = properties[nameof(Padding.Left)];
            Assert.True(property.CanResetValue(boxedPadding));
            Assert.True(property.ShouldSerializeValue(boxedPadding));
            property.ResetValue(boxedPadding);

            Assert.Equal(-1, ((Padding)boxedPadding).All);
            Assert.Equal(0, ((Padding)boxedPadding).Left);
            Assert.Equal(2, ((Padding)boxedPadding).Top);
            Assert.Equal(3, ((Padding)boxedPadding).Right);
            Assert.Equal(4, ((Padding)boxedPadding).Bottom);
            Assert.True(property.CanResetValue(boxedPadding));
            Assert.True(property.ShouldSerializeValue(boxedPadding));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void Padding_Left_SetOnAll_GetReturnsExpected(int value)
        {
            var padding = new Padding(5)
            {
                Left = value
            };
            Assert.Equal(-1, padding.All);
            Assert.Equal(value, padding.Left);
            Assert.Equal(5, padding.Top);
            Assert.Equal(5, padding.Right);
            Assert.Equal(5, padding.Bottom);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void Padding_Top_Set_GetReturnsExpected(int value)
        {
            var padding = new Padding(1, 2, 3, 4)
            {
                Top = value
            };
            Assert.Equal(-1, padding.All);
            Assert.Equal(1, padding.Left);
            Assert.Equal(value, padding.Top);
            Assert.Equal(3, padding.Right);
            Assert.Equal(4, padding.Bottom);
        }

        [Fact]
        public void Padding_TopPropertyDescriptor_ResetValue_SetsToZero()
        {
            var padding = new Padding(1, 2, 3, 4);
            object boxedPadding = padding;
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(padding);
            PropertyDescriptor property = properties[nameof(Padding.Top)];
            Assert.True(property.CanResetValue(boxedPadding));
            Assert.True(property.ShouldSerializeValue(boxedPadding));
            property.ResetValue(boxedPadding);

            Assert.Equal(-1, ((Padding)boxedPadding).All);
            Assert.Equal(1, ((Padding)boxedPadding).Left);
            Assert.Equal(0, ((Padding)boxedPadding).Top);
            Assert.Equal(3, ((Padding)boxedPadding).Right);
            Assert.Equal(4, ((Padding)boxedPadding).Bottom);
            Assert.True(property.CanResetValue(boxedPadding));
            Assert.True(property.ShouldSerializeValue(boxedPadding));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void Padding_Top_SetOnAll_GetReturnsExpected(int value)
        {
            var padding = new Padding(5)
            {
                Top = value
            };
            Assert.Equal(-1, padding.All);
            Assert.Equal(5, padding.Left);
            Assert.Equal(value, padding.Top);
            Assert.Equal(5, padding.Right);
            Assert.Equal(5, padding.Bottom);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void Padding_Right_Set_GetReturnsExpected(int value)
        {
            var padding = new Padding(1, 2, 3, 4)
            {
                Right = value
            };
            Assert.Equal(-1, padding.All);
            Assert.Equal(1, padding.Left);
            Assert.Equal(2, padding.Top);
            Assert.Equal(value, padding.Right);
            Assert.Equal(4, padding.Bottom);
        }

        [Fact]
        public void Padding_RightPropertyDescriptor_ResetValue_SetsToZero()
        {
            var padding = new Padding(1, 2, 3, 4);
            object boxedPadding = padding;
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(padding);
            PropertyDescriptor property = properties[nameof(Padding.Right)];
            Assert.True(property.CanResetValue(boxedPadding));
            Assert.True(property.ShouldSerializeValue(boxedPadding));
            property.ResetValue(boxedPadding);

            Assert.Equal(-1, ((Padding)boxedPadding).All);
            Assert.Equal(1, ((Padding)boxedPadding).Left);
            Assert.Equal(2, ((Padding)boxedPadding).Top);
            Assert.Equal(0, ((Padding)boxedPadding).Right);
            Assert.Equal(4, ((Padding)boxedPadding).Bottom);
            Assert.True(property.CanResetValue(boxedPadding));
            Assert.True(property.ShouldSerializeValue(boxedPadding));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void Padding_Right_SetOnAll_GetReturnsExpected(int value)
        {
            var padding = new Padding(5)
            {
                Right = value
            };
            Assert.Equal(-1, padding.All);
            Assert.Equal(5, padding.Left);
            Assert.Equal(5, padding.Top);
            Assert.Equal(value, padding.Right);
            Assert.Equal(5, padding.Bottom);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void Padding_Bottom_Set_GetReturnsExpected(int value)
        {
            var padding = new Padding(1, 2, 3, 4)
            {
                Bottom = value
            };
            Assert.Equal(-1, padding.All);
            Assert.Equal(1, padding.Left);
            Assert.Equal(2, padding.Top);
            Assert.Equal(3, padding.Right);
            Assert.Equal(value, padding.Bottom);
        }

        [Fact]
        public void Padding_BottomPropertyDescriptor_ResetValue_SetsToZero()
        {
            var padding = new Padding(1, 2, 3, 4);
            object boxedPadding = padding;
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(padding);
            PropertyDescriptor property = properties[nameof(Padding.Bottom)];
            Assert.True(property.CanResetValue(boxedPadding));
            Assert.True(property.ShouldSerializeValue(boxedPadding));
            property.ResetValue(boxedPadding);

            Assert.Equal(-1, ((Padding)boxedPadding).All);
            Assert.Equal(1, ((Padding)boxedPadding).Left);
            Assert.Equal(2, ((Padding)boxedPadding).Top);
            Assert.Equal(3, ((Padding)boxedPadding).Right);
            Assert.Equal(0, ((Padding)boxedPadding).Bottom);
            Assert.True(property.CanResetValue(boxedPadding));
            Assert.True(property.ShouldSerializeValue(boxedPadding));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void Padding_Bottom_SetOnAll_GetReturnsExpected(int value)
        {
            var padding = new Padding(5)
            {
                Bottom = value
            };
            Assert.Equal(-1, padding.All);
            Assert.Equal(5, padding.Left);
            Assert.Equal(5, padding.Top);
            Assert.Equal(5, padding.Right);
            Assert.Equal(value, padding.Bottom);
        }

        public static IEnumerable<object[]> Add_TestData()
        {
            yield return new object[] { new Padding(1, 2, 3, 4), new Padding(2, 3, 4, 5), new Padding(3, 5, 7, 9) };
        }

        [Theory]
        [MemberData(nameof(Add_TestData))]
        public void Padding_Add_Invoke_ReturnsExpected(Padding p1, Padding p2, Padding expected)
        {
            Assert.Equal(expected, p1 + p2);
            Assert.Equal(expected, Padding.Add(p1, p2));
        }

        public static IEnumerable<object[]> Subtract_TestData()
        {
            yield return new object[] { new Padding(2, 3, 4, 5), new Padding(1, 2, 3, 4), new Padding(1, 1, 1, 1) };
        }

        [Theory]
        [MemberData(nameof(Subtract_TestData))]
        public void Padding_Subtract_Invoke_ReturnsExpected(Padding p1, Padding p2, Padding expected)
        {
            Assert.Equal(expected, p1 - p2);
            Assert.Equal(expected, Padding.Subtract(p1, p2));
        }

        public static IEnumerable<object[]> Equals_TestData()
        {
            yield return new object[] { new Padding(1), new Padding(1), true };
            yield return new object[] { new Padding(1), new Padding(1, 1, 1, 1), true };
            yield return new object[] { new Padding(1), new Padding(1, 2, 1, 1), false };
            yield return new object[] { new Padding(1), new Padding(1, 1, 3, 1), false };
            yield return new object[] { new Padding(1), new Padding(1, 1, 1, 4), false };

            yield return new object[] { new Padding(1, 2, 3, 4), new Padding(1, 2, 3, 4), true };
            yield return new object[] { new Padding(1, 2, 3, 4), new Padding(1, 1, 3, 4), false };
            yield return new object[] { new Padding(1, 2, 3, 4), new Padding(1, 2, 2, 4), false };
            yield return new object[] { new Padding(1, 2, 3, 4), new Padding(1, 2, 3, 3), false };
            yield return new object[] { new Padding(1, 2, 3, 4), new Padding(1), false };

            yield return new object[] { new Padding(1, 2, 3, 4), new object(), false };
            yield return new object[] { new Padding(1, 2, 3, 4), null, false };
        }

        [Theory]
        [MemberData(nameof(Equals_TestData))]
        public void Padding_Equals_Invoke_ReturnsExpected(Padding padding, object other, bool expected)
        {
            if (other is Padding otherPadding)
            {
                Assert.Equal(expected, padding == otherPadding);
                Assert.Equal(!expected, padding != otherPadding);
                Assert.Equal(expected, padding.GetHashCode().Equals(otherPadding.GetHashCode()));
            }

            Assert.Equal(expected, padding.Equals(other));
        }

        [Fact]
        public void Padding_ToString_Invoke_ReturnsExpected()
        {
            var padding = new Padding(1, 2, 3, 4);
            Assert.Equal("{Left=1,Top=2,Right=3,Bottom=4}", padding.ToString());
        }

        [Fact]
        public void Padding_ToString_InvokeAll_ReturnsExpected()
        {
            var padding = new Padding(1);
            Assert.Equal("{Left=1,Top=1,Right=1,Bottom=1}", padding.ToString());
        }

        [Fact]
        public void Padding_TypeConverter_Get_ReturnsPaddingConverter()
        {
            var padding = new Padding(1);
            Assert.IsType<PaddingConverter>(TypeDescriptor.GetConverter(padding));
        }
    }
}

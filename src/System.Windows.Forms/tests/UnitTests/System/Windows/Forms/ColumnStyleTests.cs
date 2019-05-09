// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ColumnStyleTests
    {
        [Fact]
        public void ColumnStyle_Ctor_Default()
        {
            var style = new ColumnStyle();
            Assert.Equal(SizeType.AutoSize, style.SizeType);
            Assert.Equal(0, style.Width);
        }

        [Theory]
        [InlineData(SizeType.AutoSize)]
        [InlineData(SizeType.Absolute)]
        [InlineData((SizeType)(SizeType.AutoSize - 1))]
        [InlineData((SizeType)(SizeType.Percent + 1))]
        public void ColumnStyle_Ctor_SizeType(SizeType sizeType)
        {
            var style = new ColumnStyle(sizeType);
            Assert.Equal(sizeType, style.SizeType);
            Assert.Equal(0, style.Width);
        }

        [Theory]
        [InlineData(SizeType.AutoSize, 0)]
        [InlineData(SizeType.Absolute, 1)]
        [InlineData((SizeType)(SizeType.AutoSize - 1), 2)]
        [InlineData((SizeType)(SizeType.Percent + 1), 3)]
        public void ColumnStyle_Ctor_SizeType_Float(SizeType sizeType, float width)
        {
            var style = new ColumnStyle(sizeType, width);
            Assert.Equal(sizeType, style.SizeType);
            Assert.Equal(width, style.Width);
        }

        [Fact]
        public void ColumnStyle_Ctor_NegativeWidth_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("value", () => new ColumnStyle(SizeType.AutoSize, -1));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetNonNegativeIntTheoryData))]
        public void ColumnStyle_Width_Set_GetReturnsExpected(float value)
        {
            var style = new ColumnStyle
            {
                Width = value
            };
            Assert.Equal(value, style.Width);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetNonNegativeIntTheoryData))]
        public void ColumnStyle_Width_SetOwned_GetReturnsExpected(float value)
        {
            var panel = new TableLayoutPanel();
            var style = new ColumnStyle();
            panel.LayoutSettings.RowStyles.Add(style);

            style.Width = value;
            Assert.Equal(value, style.Width);
        }

        [Fact]
        public void ColumnStyle_Width_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            var style = new ColumnStyle();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => style.Width = -1);
        }
    }
}

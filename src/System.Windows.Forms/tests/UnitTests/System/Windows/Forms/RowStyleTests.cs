// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class RowStyleTests
    {
        [Fact]
        public void RowStyle_Ctor_Default()
        {
            var style = new RowStyle();
            Assert.Equal(SizeType.AutoSize, style.SizeType);
            Assert.Equal(0, style.Height);
        }

        [Theory]
        [InlineData(SizeType.AutoSize)]
        [InlineData(SizeType.Absolute)]
        [InlineData((SizeType)(SizeType.AutoSize - 1))]
        [InlineData((SizeType)(SizeType.Percent + 1))]
        public void RowStyle_Ctor_SizeType(SizeType sizeType)
        {
            var style = new RowStyle(sizeType);
            Assert.Equal(sizeType, style.SizeType);
            Assert.Equal(0, style.Height);
        }

        [Theory]
        [InlineData(SizeType.AutoSize, 0)]
        [InlineData(SizeType.Absolute, 1)]
        [InlineData((SizeType)(SizeType.AutoSize - 1), 2)]
        [InlineData((SizeType)(SizeType.Percent + 1), 3)]
        public void RowStyle_Ctor_SizeType_Float(SizeType sizeType, float height)
        {
            var style = new RowStyle(sizeType, height);
            Assert.Equal(sizeType, style.SizeType);
            Assert.Equal(height, style.Height);
        }

        [Fact]
        public void RowStyle_Ctor_NegativeHeight_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("value", () => new RowStyle(SizeType.AutoSize, -1));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetNonNegativeIntTheoryData))]
        public void RowStyle_Height_Set_GetReturnsExpected(float value)
        {
            var style = new RowStyle
            {
                Height = value
            };
            Assert.Equal(value, style.Height);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetNonNegativeIntTheoryData))]
        public void RowStyle_Height_SetOwned_GetReturnsExpected(float value)
        {
            var panel = new TableLayoutPanel();
            var style = new RowStyle();
            panel.LayoutSettings.RowStyles.Add(style);

            style.Height = value;
            Assert.Equal(value, style.Height);
        }

        [Fact]
        public void RowStyle_Height_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            var style = new RowStyle();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => style.Height = -1);
        }
    }
}

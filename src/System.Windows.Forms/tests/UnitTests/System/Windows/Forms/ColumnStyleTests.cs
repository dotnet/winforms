﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ColumnStyleTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ColumnStyle_Ctor_Default()
        {
            var style = new ColumnStyle();
            Assert.Equal(SizeType.AutoSize, style.SizeType);
            Assert.Equal(0, style.Width);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(SizeType))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(SizeType))]
        public void ColumnStyle_Ctor_SizeType(SizeType sizeType)
        {
            var style = new ColumnStyle(sizeType);
            Assert.Equal(sizeType, style.SizeType);
            Assert.Equal(0, style.Width);
        }

        [WinFormsTheory]
        [InlineData(SizeType.AutoSize, 0)]
        [InlineData(SizeType.Absolute, 1)]
        [InlineData(SizeType.Percent, 2)]
        [InlineData((SizeType)(SizeType.AutoSize - 1), 3)]
        [InlineData((SizeType)(SizeType.Percent + 1), 4)]
        public void ColumnStyle_Ctor_SizeType_Float(SizeType sizeType, float width)
        {
            var style = new ColumnStyle(sizeType, width);
            Assert.Equal(sizeType, style.SizeType);
            Assert.Equal(width, style.Width);
        }

        [WinFormsFact]
        public void ColumnStyle_Ctor_NegativeWidth_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("value", () => new ColumnStyle(SizeType.AutoSize, -1));
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(float.MaxValue)]
        public void ColumnStyle_Width_Set_GetReturnsExpected(float value)
        {
            var style = new ColumnStyle
            {
                Width = value
            };
            Assert.Equal(value, style.Width);

            // Set same.
            style.Width = value;
            Assert.Equal(value, style.Width);
        }

        [WinFormsTheory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(float.MaxValue, 1)]
        public void ColumnStyle_Width_SetWithOwner_GetReturnsExpected(float value, int expectedLayoutCallCount)
        {
            using var control = new TableLayoutPanel();
            var style = new ColumnStyle();
            control.LayoutSettings.RowStyles.Add(style);
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Style", e.AffectedProperty);
                layoutCallCount++;
            };

            style.Width = value;
            Assert.Equal(value, style.Width);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            style.Width = value;
            Assert.Equal(value, style.Width);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(float.MaxValue, 1)]
        public void ColumnStyle_Width_SetWithOwnerWithHandle_GetReturnsExpected(float value, int expectedLayoutCallCount)
        {
            using var control = new TableLayoutPanel();
            var style = new ColumnStyle();
            control.LayoutSettings.RowStyles.Add(style);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Style", e.AffectedProperty);
                layoutCallCount++;
            };

            style.Width = value;
            Assert.Equal(value, style.Width);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            style.Width = value;
            Assert.Equal(value, style.Width);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ColumnStyle_Width_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            var style = new ColumnStyle();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => style.Width = -1);
        }
    }
}

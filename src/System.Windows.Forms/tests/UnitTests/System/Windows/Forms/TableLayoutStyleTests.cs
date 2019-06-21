// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TableLayoutStyleTests
    {
        [Fact]
        public void TableLayoutStyle_Ctor_Default()
        {
            var style = new SubTableLayoutStyle();
            Assert.Equal(SizeType.AutoSize, style.SizeType);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(SizeType))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(SizeType))]
        public void TableLayoutStyle_SizeType_Set_GetReturnsExpected(SizeType value)
        {
            var style = new SubTableLayoutStyle
            {
                SizeType = value
            };
            Assert.Equal(value, style.SizeType);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(SizeType))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(SizeType))]
        public void TableLayoutStyle_SizeType_SetOwned_GetReturnsExpected(SizeType value)
        {
            var panel = new TableLayoutPanel();
            var style = new ColumnStyle();
            panel.LayoutSettings.RowStyles.Add(style);

            style.SizeType = value;
            Assert.Equal(value, style.SizeType);
        }

        [Theory]
        [InlineData(typeof(string), true)]
        [InlineData(typeof(InstanceDescriptor), true)]
        [InlineData(typeof(TableLayoutStyle), false)]
        [InlineData(typeof(int), false)]
        [InlineData(null, false)]
        public void TableLayoutStyle_ConverterCanConvertTo_Invoke_ReturnsExpected(Type destinationType, bool expected)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutStyle));
            Assert.Equal(expected, converter.CanConvertTo(destinationType));
        }

        public static IEnumerable<object[]> ConvertTo_TestData()
        {
            yield return new object[] { new RowStyle(SizeType.AutoSize, 1), typeof(RowStyle).GetConstructor(Array.Empty<Type>()), Array.Empty<object>() };
            yield return new object[] { new RowStyle(SizeType.Absolute, 1), typeof(RowStyle).GetConstructor(new Type[] { typeof(SizeType), typeof(int) }), new object[] { SizeType.Absolute, 1f } };
            yield return new object[] { new RowStyle(SizeType.Percent, 1), typeof(RowStyle).GetConstructor(new Type[] { typeof(SizeType), typeof(int) }), new object[] { SizeType.Percent, 1f } };
            yield return new object[] { new ColumnStyle(SizeType.AutoSize, 1), typeof(ColumnStyle).GetConstructor(Array.Empty<Type>()), Array.Empty<object>() };
            yield return new object[] { new ColumnStyle(SizeType.Absolute, 1), typeof(ColumnStyle).GetConstructor(new Type[] { typeof(SizeType), typeof(int) }), new object[] { SizeType.Absolute, 1f } };
            yield return new object[] { new ColumnStyle(SizeType.Percent, 1), typeof(ColumnStyle).GetConstructor(new Type[] { typeof(SizeType), typeof(int) }), new object[] { SizeType.Percent, 1f } };
        }

        [Theory]
        [MemberData(nameof(ConvertTo_TestData))]
        public void TableLayoutStyle_ConverterConvertTo_InstanceDescriptorRowAbsolutePercent_ReturnsExpected(object value, ConstructorInfo expectedConstructor, object[] expectedArguments)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutStyle));
            InstanceDescriptor descriptor = Assert.IsType<InstanceDescriptor>(converter.ConvertTo(value, typeof(InstanceDescriptor)));
            Assert.Equal(expectedConstructor, descriptor.MemberInfo);
            Assert.Equal(expectedArguments, descriptor.Arguments);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(SizeType))]
        public void TableLayoutStyle_ConverterConvertTo_InvalidSizeType_ThrowsNotSupportedException(SizeType sizeType)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutStyle));
            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(new SubTableLayoutStyle { SizeType = sizeType }, typeof(InstanceDescriptor)));
        }

        [Fact]
        public void TableLayoutStyle_ConverterConvertTo_NullDestinationType_ThrowsArgumentNullException()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutStyle));
            Assert.Throws<ArgumentNullException>("destinationType", () => converter.ConvertTo(new object(), null));
        }

        [Fact]
        public void TableLayoutStyle_ConverterConvertTo_ValueNotTableLayoutStyle_ThrowsNotSupportedException()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutStyle));
            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(1, typeof(InstanceDescriptor)));
        }

        [Theory]
        [InlineData(typeof(TableLayoutPanelCellPosition))]
        [InlineData(typeof(int))]
        public void TableLayoutStyle_ConverterConvertTo_InvalidDestinationType_ThrowsNotSupportedException(Type destinationType)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutStyle));
            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(new RowStyle(), destinationType));
        }

        private class SubTableLayoutStyle : TableLayoutStyle
        {
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TableLayoutPanelCellPositionTests
    {
        [Fact]
        public void TableLayoutPanelCellPosition_Ctor_Default()
        {
            var style = new TableLayoutPanelCellPosition();
            Assert.Equal(0, style.Column);
            Assert.Equal(0, style.Row);
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(0, 0)]
        [InlineData(1, 2)]
        public void TableLayoutPanelCellPosition_Ctor_Int_int(int column, int row)
        {
            var style = new TableLayoutPanelCellPosition(column, row);
            Assert.Equal(column, style.Column);
            Assert.Equal(row, style.Row);
        }

        [Fact]
        public void TableLayoutPanelCellPosition_Ctor_InvalidColumn_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("column", () => new TableLayoutPanelCellPosition(-2, 0));
        }

        [Fact]
        public void TableLayoutPanelCellPosition_Ctor_InvalidRow_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("row", () => new TableLayoutPanelCellPosition(0, -2));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void TableLayoutPanelCellPosition_Column_Set_GetReturnsExpected(int value)
        {
            var style = new TableLayoutPanelCellPosition
            {
                Column = value
            };
            Assert.Equal(value, style.Column);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void TableLayoutPanelCellPosition_Row_Set_GetReturnsExpected(int value)
        {
            var style = new TableLayoutPanelCellPosition
            {
                Row = value
            };
            Assert.Equal(value, style.Row);
        }

        public static IEnumerable<object[]> Equals_TestData()
        {
            yield return new object[] { new TableLayoutPanelCellPosition(1, 2), new TableLayoutPanelCellPosition(1, 2), true };
            yield return new object[] { new TableLayoutPanelCellPosition(1, 2), new TableLayoutPanelCellPosition(2, 2), false };
            yield return new object[] { new TableLayoutPanelCellPosition(1, 2), new TableLayoutPanelCellPosition(1, 3), false };

            yield return new object[] { new TableLayoutPanelCellPosition(1, 2), new object(), false };
            yield return new object[] { new TableLayoutPanelCellPosition(1, 2), null, false };
        }

        [Theory]
        [MemberData(nameof(Equals_TestData))]
        public void TableLayoutPanelCellPosition_Equals_Invoke_ReturnsExpected(TableLayoutPanelCellPosition position, object other, bool expected)
        {
            if (other is TableLayoutPanelCellPosition otherPosition)
            {
                Assert.Equal(expected, position.GetHashCode().Equals(other.GetHashCode()));
                Assert.Equal(expected, position == otherPosition);
                Assert.Equal(!expected, position != otherPosition);
            }
            Assert.Equal(expected, position.Equals(other));
        }

        [Fact]
        public void TableLayoutPanelCellPosition_ToString_Invoke_ReturnsExpected()
        {
            var position = new TableLayoutPanelCellPosition(1, 2);
            Assert.Equal("1,2", position.ToString());
        }

        public static TheoryData<Type, bool> CanConvertFromData =>
            CommonTestHelper.GetConvertFromTheoryData();

        [Theory]
        [MemberData(nameof(CanConvertFromData))]
        [InlineData(typeof(TableLayoutSettings), false)]
        [InlineData(typeof(string), true)]
        public void TableLayoutPanelCellPosition_ConverterCanConvertFrom_Invoke_ReturnsExpected(Type sourceType, bool expected)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            Assert.Equal(expected, converter.CanConvertFrom(sourceType));
        }

        public static IEnumerable<object[]> ConvertFrom_TestData()
        {
            yield return new object[] { "1,2", new TableLayoutPanelCellPosition(1, 2) };
            yield return new object[] { "  1 , 2 ", new TableLayoutPanelCellPosition(1, 2) };
            yield return new object[] { "   ", null };
            yield return new object[] { string.Empty, null };
        }

        [Theory]
        [MemberData(nameof(ConvertFrom_TestData))]
        public void TableLayoutPanelCellPosition_ConverterConvertFrom_String_ReturnsExpected(string value, object expected)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            Assert.Equal(expected, converter.ConvertFrom(value));
            Assert.Equal(expected, converter.ConvertFrom(null, null, value));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public void TableLayoutPanelCellPosition_ConvertFrom_InvalidValue_ThrowsNotSupportedException(object value)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(value));
        }

        [Theory]
        [InlineData("1")]
        [InlineData("1,2,3")]
        public void TableLayoutPanelCellPosition_ConverterConvertFrom_InvalidString_ThrowsArgumentException(string value)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            Assert.Throws<ArgumentException>("value", () => converter.ConvertFrom(value));
        }

        [Fact]
        public void TableLayoutPanelCellPosition_ConverterConvertFrom_InvalidColumn_ThrowsArgumentOutOfRangeException()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            Assert.Throws<ArgumentOutOfRangeException>("column", () => converter.ConvertFrom("-2,2"));
        }

        [Fact]
        public void TableLayoutPanelCellPosition_ConverterConvertFrom_InvalidRow_ThrowsArgumentOutOfRangeException()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            Assert.Throws<ArgumentOutOfRangeException>("row", () => converter.ConvertFrom("1,-2"));
        }

        [Theory]
        [InlineData(typeof(string), true)]
        [InlineData(typeof(InstanceDescriptor), true)]
        [InlineData(typeof(TableLayoutPanelCellPosition), false)]
        [InlineData(typeof(int), false)]
        [InlineData(null, false)]
        public void TableLayoutPanelCellPosition_ConverterCanConvertTo_Invoke_ReturnsExpected(Type destinationType, bool expected)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            Assert.Equal(expected, converter.CanConvertTo(destinationType));
        }

        [Fact]
        public void TableLayoutPanelCellPosition_ConverterConvertTo_InstanceDescriptor_ReturnsExpected()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            InstanceDescriptor descriptor = Assert.IsType<InstanceDescriptor>(converter.ConvertTo(new TableLayoutPanelCellPosition(1, 2), typeof(InstanceDescriptor)));
            Assert.Equal(typeof(TableLayoutPanelCellPosition).GetConstructor(new Type[] { typeof(int), typeof(int) }), descriptor.MemberInfo);
            Assert.Equal(new object[] { 1, 2 }, descriptor.Arguments);
            Assert.True(descriptor.IsComplete);
        }

        [Fact]
        public void TableLayoutPanelCellPosition_ConverterConvertTo_NullDestinationType_ThrowsArgumentNullException()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            Assert.Throws<ArgumentNullException>("destinationType", () => converter.ConvertTo(new object(), null));
        }

        [Fact]
        public void TableLayoutPanelCellPosition_ConverterConvertTo_ValueNotTableLayoutPanelCellPosition_ThrowsNotSupportedException()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(1, typeof(InstanceDescriptor)));
        }

        [Theory]
        [InlineData(typeof(TableLayoutPanelCellPosition))]
        [InlineData(typeof(int))]
        public void TableLayoutPanelCellPosition_ConverterConvertTo_InvalidDestinationType_ThrowsNotSupportedException(Type destinationType)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(new TableLayoutPanelCellPosition(), destinationType));
        }

        [Fact]
        public void TableLayoutPanelCellPosition_ConverterCreateInstance_ValidPropertyValues_ReturnsExpected()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            TableLayoutPanelCellPosition position = Assert.IsType<TableLayoutPanelCellPosition>(converter.CreateInstance(null, new Dictionary<string, object>
            {
                {nameof(TableLayoutPanelCellPosition.Column), 1},
                {nameof(TableLayoutPanelCellPosition.Row), 2}
            }));
            Assert.Equal(new TableLayoutPanelCellPosition(1, 2), position);
        }

        [Fact]
        public void TableLayoutPanelCellPosition_ConverterCreateInstance_NullPropertyValues_ThrowsArgumentNullException()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            Assert.Throws<ArgumentNullException>("propertyValues", () => converter.CreateInstance(null, null));
        }
        public static IEnumerable<object[]> CreateInstance_InvalidPropertyValueType_TestData()
        {
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(TableLayoutPanelCellPosition.Row), new object()},
                    {nameof(TableLayoutPanelCellPosition.Column), 2},
                }
            };
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(TableLayoutPanelCellPosition.Row), null},
                    {nameof(TableLayoutPanelCellPosition.Column), 2},
                }
            };
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(TableLayoutPanelCellPosition.Column), 2}
                }
            };

            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(TableLayoutPanelCellPosition.Row), 1},
                    {nameof(TableLayoutPanelCellPosition.Column), new object()}
                }
            };
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(TableLayoutPanelCellPosition.Row), 1},
                    {nameof(TableLayoutPanelCellPosition.Column), null}
                }
            };
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    {nameof(TableLayoutPanelCellPosition.Row), 1}
                }
            };
        }

        [Theory]
        [MemberData(nameof(CreateInstance_InvalidPropertyValueType_TestData))]
        public void TableLayoutPanelCellPosition_CreateInstance_InvalidPropertyValueType_ThrowsArgumentException(IDictionary propertyValues)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            Assert.Throws<ArgumentException>("propertyValues", () => converter.CreateInstance(null, propertyValues));
        }

        [Fact]
        public void TableLayoutPanelCellPosition_ConverterCreateInstance_InvalidColumn_ThrowsArgumentOutOfRangeException()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            Assert.Throws<ArgumentOutOfRangeException>("column", () => converter.CreateInstance(null, new Dictionary<string, object>
            {
                {nameof(TableLayoutPanelCellPosition.Column), -2},
                {nameof(TableLayoutPanelCellPosition.Row), 2}
            }));
        }

        [Fact]
        public void TableLayoutPanelCellPosition_ConverterCreateInstance_InvalidRow_ThrowsArgumentOutOfRangeException()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            Assert.Throws<ArgumentOutOfRangeException>("row", () => converter.CreateInstance(null, new Dictionary<string, object>
            {
                {nameof(TableLayoutPanelCellPosition.Column), 1},
                {nameof(TableLayoutPanelCellPosition.Row), -2}
            }));
        }

        [Fact]
        public void TableLayoutPanelCellPosition_ConverterGetCreateInstanceSupported_Invoke_ReturnsTrue()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            Assert.True(converter.GetCreateInstanceSupported());
        }

        [Fact]
        public void TableLayoutPanelCellPosition_ConverterGetProperties_Invoke_ReturnsExpected()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            PropertyDescriptorCollection properties = converter.GetProperties(null);
            Assert.Equal(2, properties.Count);
            Assert.Equal(nameof(TableLayoutPanelCellPosition.Column), properties[0].Name);
            Assert.Equal(nameof(TableLayoutPanelCellPosition.Row), properties[1].Name);
        }

        [Fact]
        public void TableLayoutPanelCellPosition_ConverterGetPropertiesSupported_Invoke_ReturnsTrue()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutPanelCellPosition));
            Assert.True(converter.GetPropertiesSupported());
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class ListViewSubItemConverterTests : IClassFixture<ThreadExceptionFixture>
    {
        public static TheoryData<Type, bool> CanConvertFromData =>
            CommonTestHelper.GetConvertFromTheoryData();

        [Theory]
        [MemberData(nameof(CanConvertFromData))]
        [InlineData(typeof(ListViewItem.ListViewSubItem), false)]
        [InlineData(typeof(string), false)]
        public void ListViewSubItemConverter_CanConvertFrom_Invoke_ReturnsExpected(Type sourceType, bool expected)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewItem.ListViewSubItem));
            Assert.Equal(expected, converter.CanConvertFrom(sourceType));
        }

        [Theory]
        [InlineData("value")]
        [InlineData(1)]
        [InlineData(null)]
        public void ListViewSubItemConverter_ConvertFrom_InvalidValue_ThrowsNotSupportedException(object value)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewItem.ListViewSubItem));
            Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(value));
        }

        [Theory]
        [InlineData(typeof(string), true)]
        [InlineData(typeof(InstanceDescriptor), true)]
        [InlineData(typeof(ListViewItem.ListViewSubItem), false)]
        [InlineData(typeof(int), false)]
        [InlineData(null, false)]
        public void ListViewSubItemConverter_CanConvertTo_Invoke_ReturnsExpected(Type destinationType, bool expected)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewItem.ListViewSubItem));
            Assert.Equal(expected, converter.CanConvertTo(destinationType));
        }

        public static IEnumerable<object[]> ConvertTo_InstanceDescriptor_TestData()
        {
            var item = new ListViewItem();
            yield return new object[]
            {
                new ListViewItem.ListViewSubItem(item, "text"),
                new Type[] { typeof(ListViewItem), typeof(string) },
                new object[] { null, "text" }
            };

            yield return new object[]
            {
                new ListViewItem.ListViewSubItem(null, "text", Color.Red, Color.Blue, SystemFonts.MenuFont),
                new Type[] { typeof(ListViewItem), typeof(string), typeof(Color), typeof(Color), typeof(Font) },
                new object[] { null, "text", Color.Red, Color.Blue, SystemFonts.MenuFont }
            };
        }

        [Theory]
        [MemberData(nameof(ConvertTo_InstanceDescriptor_TestData))]
        public void ListViewSubItemConverter_ConvertTo_InstanceDescriptor_ReturnsExpected(ListViewItem.ListViewSubItem value, Type[] parameterTypes, object[] arguments)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewItem.ListViewSubItem));
            InstanceDescriptor descriptor = Assert.IsType<InstanceDescriptor>(converter.ConvertTo(value, typeof(InstanceDescriptor)));
            Assert.Equal(typeof(ListViewItem.ListViewSubItem).GetConstructor(parameterTypes), descriptor.MemberInfo);
            Assert.Equal(arguments, descriptor.Arguments);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData(1, "1")]
        public void ListViewSubItemConverter_ConvertTo_String_ReturnsExpected(object value, string expected)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewItem.ListViewSubItem));
            Assert.Equal(expected, converter.ConvertTo(value, typeof(string)));
        }

        [Fact]
        public void ListViewSubItemConverter_ConvertTo_NullDestinationType_ThrowsArgumentNullException()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewItem.ListViewSubItem));
            Assert.Throws<ArgumentNullException>("destinationType", () => converter.ConvertTo(new object(), null));
        }

        [Fact]
        public void ListViewSubItemConverter_ConvertTo_ValueNotThrowsNotSupportedException()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewItem.ListViewSubItem));
            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(1, typeof(InstanceDescriptor)));
        }

        [Theory]
        [InlineData(typeof(ListViewItem.ListViewSubItem))]
        [InlineData(typeof(int))]
        public void ListViewSubItemConverter_ConvertTo_InvalidDestinationType_ThrowsNotSupportedException(Type destinationType)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewItem.ListViewSubItem));
            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(new ListViewItem.ListViewSubItem(), destinationType));
        }

        [Fact]
        public void ListViewSubItemConverter_GetPropertiesSupported_Invoke_ReturnsTrue()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewItem.ListViewSubItem));
            Assert.True(converter.GetPropertiesSupported(null));
        }

        [Fact]
        public void ListViewSubItemConverter_GetProperties_Invoke_ReturnsExpected()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewItem.ListViewSubItem));
            var item = new ListViewItem.ListViewSubItem();
            Assert.Equal(TypeDescriptor.GetProperties(item, null).Count, converter.GetProperties(null, item, null).Count);
        }

        [Fact]
        public void ListViewSubItemConverter_GetStandardValues_Invoke_ReturnsNull()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewItem.ListViewSubItem));
            Assert.Null(converter.GetStandardValues(null));
        }

        [Fact]
        public void ListViewSubItemConverter_GetStandardValuesExclusive_Invoke_ReturnsFalse()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewItem.ListViewSubItem));
            Assert.False(converter.GetStandardValuesExclusive(null));
        }

        [Fact]
        public void ListViewSubItemConverter_GetStandardValuesSupported_Invoke_ReturnsFalse()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewItem.ListViewSubItem));
            Assert.False(converter.GetStandardValuesSupported(null));
        }
    }
}

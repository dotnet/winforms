// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class ListViewGroupConverterTests : IClassFixture<ThreadExceptionFixture>
    {
        public static TheoryData<Type, bool> CanConvertFromData =>
            CommonTestHelper.GetConvertFromTheoryData();

        [Theory]
        [MemberData(nameof(CanConvertFromData))]
        [InlineData(typeof(ListViewGroup), false)]
        [InlineData(typeof(string), false)]
        public void ListViewGroupConverter_CanConvertFrom_Invoke_ReturnsExpected(Type sourceType, bool expected)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            Assert.Equal(expected, converter.CanConvertFrom(sourceType));
        }

        public static IEnumerable<object[]> CanConvertFrom_Context_TestData()
        {
            yield return new object[] { null, false };
            yield return new object[] { new object(), false };
            yield return new object[] { new ListViewItem(), true };

            var listView = new ListView();
            var item1 = new ListViewItem();
            listView.Items.Add(item1);
            yield return new object[] { item1, true };

            var listViewWithGroups = new ListView();
            var group = new ListViewGroup("name", "header");
            listViewWithGroups.Groups.Add(group);
            var item2 = new ListViewItem();
            listViewWithGroups.Items.Add(item2);
            yield return new object[] { item2, true };
        }

        [Theory]
        [MemberData(nameof(CanConvertFrom_Context_TestData))]
        public void ListViewGroupConverter_CanConvertFrom_Context_ReturnsExpected(object instance, bool expected)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.Instance)
                .Returns(instance);
            Assert.Equal(expected, converter.CanConvertFrom(mockContext.Object, typeof(string)));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("(none)")]
        public void ListViewGroupConverter_ConvertFrom_EmptyValue_ReturnsNull(string value)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            Assert.Null(converter.ConvertFrom(value));
        }

        [Theory]
        [InlineData("header")]
        [InlineData("  header  ")]
        public void ListViewGroupConverter_ConvertFrom_ValidContext_ReturnsExpected(string value)
        {
            var listView = new ListView();
            var group = new ListViewGroup("name", "header");
            listView.Groups.Add(group);
            var item = new ListViewItem();
            listView.Items.Add(item);

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.Instance)
                .Returns(item);
            Assert.Same(group, converter.ConvertFrom(mockContext.Object, null, value));
        }

        public static IEnumerable<object[]> ConvertFrom_InvalidContext_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
            yield return new object[] { new ListViewItem() };

            var listView = new ListView();
            var item1 = new ListViewItem();
            listView.Items.Add(item1);
            yield return new object[] { item1 };

            var listViewWithGroups = new ListView();
            listViewWithGroups.Groups.Add(new ListViewGroup("name", "header"));
            var item2 = new ListViewItem();
            listViewWithGroups.Items.Add(item2);
            yield return new object[] { item2 };
        }

        [Theory]
        [MemberData(nameof(ConvertFrom_InvalidContext_TestData))]
        public void ListViewGroupConverter_ConvertFrom_InvalidContext_ThrowsNotSupportedException(object instance)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.Instance)
                .Returns(instance);

            Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(mockContext.Object, null, "value"));
            Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(mockContext.Object, null, "HEADER"));
        }

        [Theory]
        [InlineData("(NONE)")]
        [InlineData("value")]
        [InlineData(1)]
        public void ListViewGroupConverter_ConvertFrom_InvalidValue_ThrowsNotSupportedException(object value)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(value));
        }

        [Theory]
        [InlineData(typeof(string), true)]
        [InlineData(typeof(InstanceDescriptor), true)]
        [InlineData(typeof(ListViewGroup), false)]
        [InlineData(typeof(int), false)]
        [InlineData(null, false)]
        public void ListViewGroupConverter_CanConvertTo_Invoke_ReturnsExpected(Type destinationType, bool expected)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            Assert.Equal(expected, converter.CanConvertTo(destinationType));
        }

        public static IEnumerable<object[]> CanConvertTo_Context_TestData()
        {
            yield return new object[] { null, true };
            yield return new object[] { new object(), true };
            yield return new object[] { new ListViewItem(), true };

            var listView = new ListView();
            var item1 = new ListViewItem();
            listView.Items.Add(item1);
            yield return new object[] { item1, true };

            var listViewWithGroups = new ListView();
            var group = new ListViewGroup("name", "header");
            listViewWithGroups.Groups.Add(group);
            var item2 = new ListViewItem();
            listViewWithGroups.Items.Add(item2);
            yield return new object[] { item2, true };
        }

        [Theory]
        [MemberData(nameof(CanConvertTo_Context_TestData))]
        public void ListViewGroupConverter_CanConvertTo_Context_ReturnsExpected(object instance, bool expected)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.Instance)
                .Returns(instance);
            Assert.Equal(expected, converter.CanConvertTo(mockContext.Object, typeof(string)));
        }

        public static IEnumerable<object[]> ConvertTo_InstanceDescriptor_TestData()
        {
            yield return new object[]
            {
                new ListViewGroup(),
                new Type[] { typeof(string), typeof(HorizontalAlignment) },
                new object[] { "ListViewGroup", HorizontalAlignment.Left }
            };
            yield return new object[]
            {
                new ListViewGroup("key", "headerText"),
                new Type[] { typeof(string), typeof(HorizontalAlignment) },
                new object[] { "headerText", HorizontalAlignment.Left }
            };
            yield return new object[]
            {
                new ListViewGroup("header"),
                new Type[] { typeof(string), typeof(HorizontalAlignment) },
                new object[] { "header", HorizontalAlignment.Left }
            };
            yield return new object[]
            {
                new ListViewGroup("header", HorizontalAlignment.Center),
                new Type[] { typeof(string), typeof(HorizontalAlignment) },
                new object[] { "header", HorizontalAlignment.Center }
            };
        }

        [Theory]
        [MemberData(nameof(ConvertTo_InstanceDescriptor_TestData))]
        public void ListViewGroupConverter_ConvertTo_InstanceDescriptor_ReturnsExpected(ListViewGroup value, Type[] parameterTypes, object[] arguments)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            InstanceDescriptor descriptor = Assert.IsType<InstanceDescriptor>(converter.ConvertTo(value, typeof(InstanceDescriptor)));
            Assert.Equal(typeof(ListViewGroup).GetConstructor(parameterTypes), descriptor.MemberInfo);
            Assert.Equal(arguments, descriptor.Arguments);
        }

        [Theory]
        [InlineData(null, "(none)")]
        [InlineData(1, "1")]
        public void ListViewGroupConverter_ConvertTo_String_ReturnsExpected(object value, string expected)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            Assert.Equal(expected, converter.ConvertTo(value, typeof(string)));
        }

        [Fact]
        public void ListViewGroupConverter_ConvertTo_NullDestinationType_ThrowsArgumentNullException()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            Assert.Throws<ArgumentNullException>("destinationType", () => converter.ConvertTo(new object(), null));
        }

        [Fact]
        public void ListViewGroupConverter_ConvertTo_ValueNotThrowsNotSupportedException()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(1, typeof(InstanceDescriptor)));
        }

        [Theory]
        [InlineData(typeof(ListViewGroup))]
        [InlineData(typeof(int))]
        public void ListViewGroupConverter_ConvertTo_InvalidDestinationType_ThrowsNotSupportedException(Type destinationType)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(new ListViewGroup(), destinationType));
        }

        [Fact]
        public void ListViewGroupConverter_GetPropertiesSupported_Invoke_ReturnsFalse()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            Assert.False(converter.GetPropertiesSupported(null));
        }

        [Fact]
        public void ListViewGroupConverter_GetProperties_Invoke_ReturnsNull()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            var item = new ListViewGroup();
            Assert.Null(converter.GetProperties(null, item, null));
        }

        public static IEnumerable<object[]> GetStandardValues_TestData()
        {
            yield return new object[] { null, null };
            yield return new object[] { new object(), null };
            yield return new object[] { new ListViewItem(), null };

            var listView = new ListView();
            var item1 = new ListViewItem();
            listView.Items.Add(item1);
            yield return new object[] { item1, new object[] { null } };

            var listViewWithGroups = new ListView();
            var group = new ListViewGroup("name", "header");
            listViewWithGroups.Groups.Add(group);
            var item2 = new ListViewItem();
            listViewWithGroups.Items.Add(item2);
            yield return new object[] { item2, new object[] { group, null } };
        }

        [Theory]
        [MemberData(nameof(GetStandardValues_TestData))]
        public void ListViewGroupConverter_GetStandardValues_HasContext_ReturnsExpected(object instance, object[] expected)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.Instance)
                .Returns(instance);
            Assert.Equal(expected, converter.GetStandardValues(mockContext.Object)?.Cast<object>());
        }

        [Fact]
        public void ListViewGroupConverter_GetStandardValues_NullContext_ReturnsNull()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            Assert.Null(converter.GetStandardValues(null));
        }

        [Fact]
        public void ListViewGroupConverter_GetStandardValuesExclusive_Invoke_ReturnsTrue()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            Assert.True(converter.GetStandardValuesExclusive(null));
        }

        [Fact]
        public void ListViewGroupConverter_GetStandardValuesSupported_Invoke_ReturnsTrue()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ListViewGroup));
            Assert.True(converter.GetStandardValuesSupported(null));
        }
    }
}

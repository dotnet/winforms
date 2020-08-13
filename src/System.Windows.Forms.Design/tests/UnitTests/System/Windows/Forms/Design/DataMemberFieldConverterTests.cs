// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class DataMemberFieldConverterTests : IClassFixture<ThreadExceptionFixture>
    {
        private static DataMemberFieldConverter s_converter = new DataMemberFieldConverter();
        private static ITypeDescriptorContext s_context = new MyTypeDescriptorContext();

        [Fact]
        public static void CanConvertFrom()
        {
            Assert.True(s_converter.CanConvertFrom(s_context, typeof(string)));
            Assert.True(s_converter.CanConvertFrom(s_context, typeof(InstanceDescriptor)));
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("(None)", "")]
        public static void ConvertFrom( object actual, object expected)
        {
            Assert.Equal(expected, s_converter.ConvertFrom(s_context, CultureInfo.CurrentCulture, actual));
        }

        [Theory]
        [InlineData("", typeof(string), "(none)")]
        [InlineData(null, typeof(string), "(none)")]
        [InlineData("FirstName", typeof(string), "FirstName")]
        public static void ConvertTo(object actual, Type expectedType, object expected)
        {
            Assert.Equal(expected, s_converter.ConvertTo(s_context, CultureInfo.CurrentCulture, actual, expectedType));
        }

        [Theory]
        [InlineData("", typeof(int))]
        [InlineData("FirstName", typeof(int))]
        public static void ConvertTo_ThrowsNotSupportedException(object actual, Type expectedType)
        {
            Assert.Throws<NotSupportedException>(
                () => s_converter.ConvertTo(s_context, CultureInfo.CurrentCulture, actual, expectedType));
        }
    }

    [Serializable]
    public class MyTypeDescriptorContext : ITypeDescriptorContext
    {
        public IContainer Container => null;
        public object Instance { get { return null; } }
        public PropertyDescriptor PropertyDescriptor { get { return null; } }
        public bool OnComponentChanging() { return true; }
        public void OnComponentChanged() { }
        public object GetService(Type serviceType) { return null; }
    }
}

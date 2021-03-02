// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Moq;
using Xunit;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    // NB: doesn't require thread affinity
    public class PropertiesTabTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void PropertiesTab_Ctor_Default()
        {
            var tab = new PropertiesTab();
            Assert.NotNull(tab.Bitmap);
            Assert.Same(tab.Bitmap, tab.Bitmap);
            Assert.Null(tab.Components);
            Assert.Equal("vs.properties", tab.HelpKeyword);
            Assert.NotEqual(tab.TabName, tab.HelpKeyword);
            Assert.NotEmpty(tab.TabName);
        }

        [Fact]
        public void PropertiesTab_GetDefaultProperty_InvokeWithoutDefaultProperty_ReturnsExpected()
        {
            var tab = new PropertiesTab();
            Assert.Null(tab.GetDefaultProperty(new ClassWithoutDefaultProperty()));
        }

        [Fact]
        public void PropertiesTab_GetDefaultProperty_InvokeWithoutDefaultWithNameProperty_ReturnsExpected()
        {
            var tab = new PropertiesTab();
            Assert.Equal("Name", tab.GetDefaultProperty(new ClassWithNameProperty()).Name);
        }

        [Fact]
        public void PropertiesTab_GetDefaultProperty_InvokeWithDefaultProperty_ReturnsExpected()
        {
            var tab = new PropertiesTab();
            Assert.Equal("Value", tab.GetDefaultProperty(new ClassWithDefaultProperty()).Name);
        }

        [Fact]
        public void PropertiesTab_GetDefaultProperty_InvokeNullGetProperties_ReturnsExpected()
        {
            var tab = new NullGetProperiesPropertiesTab();
            Assert.Null(tab.GetDefaultProperty(new ClassWithNameProperty()));
        }

        [Fact]
        public void PropertiesTab_GetDefaultProperty_InvokeNullComponent_ReturnsExpected()
        {
            var tab = new PropertiesTab();
            Assert.Null(tab.GetDefaultProperty(null));
        }

        [Fact]
        public void PropertiesTab_GetProperties_InvokeObject_ReturnsExpected()
        {
            var tab = new PropertiesTab();
            PropertyDescriptorCollection properties = tab.GetProperties(new ClassWithDefaultProperty());
            Assert.Equal(2, properties.Count);
            Assert.NotNull(properties[nameof(ClassWithDefaultProperty.Value)]);
            Assert.NotNull(properties[nameof(ClassWithDefaultProperty.BrowsableProperty)]);
        }

        [Fact]
        public void PropertiesTab_GetProperties_InvokeNullObject_ReturnsExpected()
        {
            var tab = new PropertiesTab();
            Assert.Empty(tab.GetProperties(null));
        }

        [Fact]
        public void PropertiesTab_GetProperties_InvokeObjectNullAttributes_ReturnsExpected()
        {
            var tab = new PropertiesTab();
            PropertyDescriptorCollection properties = tab.GetProperties(new ClassWithDefaultProperty(), null);
            Assert.Equal(2, properties.Count);
            Assert.NotNull(properties[nameof(ClassWithDefaultProperty.Value)]);
            Assert.NotNull(properties[nameof(ClassWithDefaultProperty.BrowsableProperty)]);
        }

        [Fact]
        public void PropertiesTab_GetProperties_InvokeObjectEmptyAttributes_ReturnsExpected()
        {
            var tab = new PropertiesTab();
            PropertyDescriptorCollection properties = tab.GetProperties(new ClassWithDefaultProperty(), Array.Empty<Attribute>());
            Assert.Equal(3, properties.Count);
            Assert.NotNull(properties[nameof(ClassWithDefaultProperty.Value)]);
            Assert.NotNull(properties[nameof(ClassWithDefaultProperty.NotBrowsableProperty)]);
            Assert.NotNull(properties[nameof(ClassWithDefaultProperty.BrowsableProperty)]);
        }

        [Fact]
        public void PropertiesTab_GetProperties_InvokeObjectCustomAttributes_ReturnsExpected()
        {
            var tab = new PropertiesTab();
            PropertyDescriptorCollection properties = tab.GetProperties(new ClassWithDefaultProperty(), new Attribute[] { new BrowsableAttribute(false) });
            Assert.Equal(1, properties.Count);
            Assert.NotNull(properties[nameof(ClassWithDefaultProperty.NotBrowsableProperty)]);
        }

        [Fact]
        public void PropertiesTab_GetProperties_InvokeNullObjectAttributes_ReturnsExpected()
        {
            var tab = new PropertiesTab();
            Assert.Empty(tab.GetProperties(null, null));
        }

        [Fact]
        public void PropertiesTab_GetProperties_InvokeObjectNullAttributesNullContext_ReturnsExpected()
        {
            var tab = new PropertiesTab();
            PropertyDescriptorCollection properties = tab.GetProperties(null, new ClassWithDefaultProperty(), null);
            Assert.Equal(2, properties.Count);
            Assert.NotNull(properties[nameof(ClassWithDefaultProperty.Value)]);
            Assert.NotNull(properties[nameof(ClassWithDefaultProperty.BrowsableProperty)]);
        }

        [Fact]
        public void PropertiesTab_GetProperties_InvokeObjectNullAttributesNullPropertyContextPropertiesSupported_ReturnsExpected()
        {
            var tab = new PropertiesTab();
            var mockTypeDescriptorContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockTypeDescriptorContext
                .Setup(c => c.PropertyDescriptor)
                .Returns((PropertyDescriptor)null)
                .Verifiable();
            PropertyDescriptorCollection properties = tab.GetProperties(mockTypeDescriptorContext.Object, new ClassWithDefaultProperty(), null);
            Assert.Equal(2, properties.Count);
            Assert.NotNull(properties[nameof(ClassWithDefaultProperty.Value)]);
            Assert.NotNull(properties[nameof(ClassWithDefaultProperty.BrowsableProperty)]);
            mockTypeDescriptorContext.Verify(c => c.PropertyDescriptor, Times.Once());
        }

        [Fact]
        public void PropertiesTab_GetProperties_InvokeObjectNullAttributesCustomPropertyContext_ReturnsExpected()
        {
            var tab = new PropertiesTab();
            var mockTypeDescriptorContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockTypeDescriptorContext
                .Setup(c => c.PropertyDescriptor)
                .Returns(TypeDescriptor.GetProperties(typeof(ParentClass))[0])
                .Verifiable();
            PropertyDescriptorCollection properties = tab.GetProperties(mockTypeDescriptorContext.Object, new ClassWithDefaultProperty(), null);
            Assert.Equal(2, properties.Count);
            Assert.NotNull(properties[nameof(ClassWithDefaultProperty.Value)]);
            Assert.NotNull(properties[nameof(ClassWithDefaultProperty.BrowsableProperty)]);
            mockTypeDescriptorContext.Verify(c => c.PropertyDescriptor, Times.Exactly(2));
        }

        [Fact]
        public void PropertiesTab_GetProperties_InvokeObjectNullAttributesCustomContextCustomTypeConverter_ReturnsExpected()
        {
            var tab = new PropertiesTab();
            var mockTypeDescriptorContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockTypeDescriptorContext
                .Setup(c => c.PropertyDescriptor)
                .Returns(TypeDescriptor.GetProperties(typeof(CustomTypeConverterParentClass))[0])
                .Verifiable();
            PropertyDescriptorCollection properties = tab.GetProperties(mockTypeDescriptorContext.Object, new ClassWithDefaultProperty(), null);
            Assert.Equal(1, properties.Count);
            Assert.NotNull(properties[nameof(ClassWithNameProperty.Name)]);
            mockTypeDescriptorContext.Verify(c => c.PropertyDescriptor, Times.Exactly(2));
        }

        [Fact]
        public void PropertiesTab_GetProperties_InvokeObjectEmptyAttributesNullContext_ReturnsExpected()
        {
            var tab = new PropertiesTab();
            PropertyDescriptorCollection properties = tab.GetProperties(null, new ClassWithDefaultProperty(), Array.Empty<Attribute>());
            Assert.Equal(3, properties.Count);
            Assert.NotNull(properties[nameof(ClassWithDefaultProperty.Value)]);
            Assert.NotNull(properties[nameof(ClassWithDefaultProperty.NotBrowsableProperty)]);
            Assert.NotNull(properties[nameof(ClassWithDefaultProperty.BrowsableProperty)]);
        }

        [Fact]
        public void PropertiesTab_GetProperties_InvokeObjectCustomAttributesNullContext_ReturnsExpected()
        {
            var tab = new PropertiesTab();
            PropertyDescriptorCollection properties = tab.GetProperties(null, new ClassWithDefaultProperty(), new Attribute[] { new BrowsableAttribute(false) });
            Assert.Equal(1, properties.Count);
            Assert.NotNull(properties[nameof(ClassWithDefaultProperty.NotBrowsableProperty)]);
        }

        [Fact]
        public void PropertiesTab_GetProperties_InvokeNullObjectAttributesContext_ReturnsExpected()
        {
            var tab = new PropertiesTab();
            Assert.Empty(tab.GetProperties(null, null, null));
        }

        private class NullGetProperiesPropertiesTab : PropertiesTab
        {
            public override PropertyDescriptorCollection GetProperties(object component) => null;
        }

        private class ClassWithoutDefaultProperty
        {
            public int Value { get; set; }
        }

        private class ClassWithNameProperty
        {
            public int Name { get; set; }
        }

        [DefaultProperty(nameof(ClassWithDefaultProperty.Value))]
        private class ClassWithDefaultProperty
        {
            public int Value { get; set; }

            [Browsable(false)]
            public int NotBrowsableProperty { get; set; }

            [Browsable(true)]
            public int BrowsableProperty { get; set; }
        }

        private class ParentClass
        {
            public ClassWithNameProperty Child { get; set; }
        }

        private class CustomTypeConverter : TypeConverter
        {
            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                return TypeDescriptor.GetProperties(typeof(ClassWithNameProperty));
            }

            public override bool GetPropertiesSupported(ITypeDescriptorContext context) => true;
        }

        private class CustomTypeConverterParentClass
        {
            [TypeConverter(typeof(CustomTypeConverter))]
            public ClassWithDefaultProperty Child { get; set; }
        }
    }
}

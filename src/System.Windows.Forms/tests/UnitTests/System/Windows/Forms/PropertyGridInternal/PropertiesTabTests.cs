// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Moq;

namespace System.Windows.Forms.PropertyGridInternal.Tests;

// NB: doesn't require thread affinity
public class PropertiesTabTests
{
    [Fact]
    public void PropertiesTab_Ctor_Default()
    {
        PropertiesTab tab = new();
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
        PropertiesTab tab = new();
        Assert.Null(tab.GetDefaultProperty(new ClassWithoutDefaultProperty()));
    }

    [Fact]
    public void PropertiesTab_GetDefaultProperty_InvokeWithoutDefaultWithNameProperty_ReturnsExpected()
    {
        PropertiesTab tab = new();
        Assert.Equal("Name", tab.GetDefaultProperty(new ClassWithNameProperty()).Name);
    }

    [Fact]
    public void PropertiesTab_GetDefaultProperty_InvokeWithDefaultProperty_ReturnsExpected()
    {
        PropertiesTab tab = new();
        Assert.Equal("Value", tab.GetDefaultProperty(new ClassWithDefaultProperty()).Name);
    }

    [Fact]
    public void PropertiesTab_GetDefaultProperty_InvokeNullGetProperties_ReturnsExpected()
    {
        NullGetPropertiesPropertiesTab tab = new();
        Assert.Null(tab.GetDefaultProperty(new ClassWithNameProperty()));
    }

    [Fact]
    public void PropertiesTab_GetDefaultProperty_InvokeNullComponent_ReturnsExpected()
    {
        PropertiesTab tab = new();
        Assert.Null(tab.GetDefaultProperty(null));
    }

    [Fact]
    public void PropertiesTab_GetProperties_InvokeObject_ReturnsExpected()
    {
        PropertiesTab tab = new();
        PropertyDescriptorCollection properties = tab.GetProperties(new ClassWithDefaultProperty());
        Assert.Equal(2, properties.Count);
        Assert.NotNull(properties[nameof(ClassWithDefaultProperty.Value)]);
        Assert.NotNull(properties[nameof(ClassWithDefaultProperty.BrowsableProperty)]);
    }

    [Fact]
    public void PropertiesTab_GetProperties_InvokeNullObject_ReturnsExpected()
    {
        PropertiesTab tab = new();
        Assert.Empty(tab.GetProperties(null));
    }

    [Fact]
    public void PropertiesTab_GetProperties_InvokeObjectNullAttributes_ReturnsExpected()
    {
        PropertiesTab tab = new();
        PropertyDescriptorCollection properties = tab.GetProperties(new ClassWithDefaultProperty(), null);
        Assert.Equal(2, properties.Count);
        Assert.NotNull(properties[nameof(ClassWithDefaultProperty.Value)]);
        Assert.NotNull(properties[nameof(ClassWithDefaultProperty.BrowsableProperty)]);
    }

    [Fact]
    public void PropertiesTab_GetProperties_InvokeObjectEmptyAttributes_ReturnsExpected()
    {
        PropertiesTab tab = new();
        PropertyDescriptorCollection properties = tab.GetProperties(new ClassWithDefaultProperty(), Array.Empty<Attribute>());
        Assert.Equal(3, properties.Count);
        Assert.NotNull(properties[nameof(ClassWithDefaultProperty.Value)]);
        Assert.NotNull(properties[nameof(ClassWithDefaultProperty.NotBrowsableProperty)]);
        Assert.NotNull(properties[nameof(ClassWithDefaultProperty.BrowsableProperty)]);
    }

    [Fact]
    public void PropertiesTab_GetProperties_InvokeObjectCustomAttributes_ReturnsExpected()
    {
        PropertiesTab tab = new();
        PropertyDescriptorCollection properties = tab.GetProperties(new ClassWithDefaultProperty(), [new BrowsableAttribute(false)]);
        Assert.Equal(1, properties.Count);
        Assert.NotNull(properties[nameof(ClassWithDefaultProperty.NotBrowsableProperty)]);
    }

    [Fact]
    public void PropertiesTab_GetProperties_InvokeNullObjectAttributes_ReturnsExpected()
    {
        PropertiesTab tab = new();
        Assert.Empty(tab.GetProperties(null, null));
    }

    [Fact]
    public void PropertiesTab_GetProperties_InvokeObjectNullAttributesNullContext_ReturnsExpected()
    {
        PropertiesTab tab = new();
        PropertyDescriptorCollection properties = tab.GetProperties(null, new ClassWithDefaultProperty(), null);
        Assert.Equal(2, properties.Count);
        Assert.NotNull(properties[nameof(ClassWithDefaultProperty.Value)]);
        Assert.NotNull(properties[nameof(ClassWithDefaultProperty.BrowsableProperty)]);
    }

    [Fact]
    public void PropertiesTab_GetProperties_InvokeObjectNullAttributesNullPropertyContextPropertiesSupported_ReturnsExpected()
    {
        PropertiesTab tab = new();
        Mock<ITypeDescriptorContext> mockTypeDescriptorContext = new(MockBehavior.Strict);
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
        PropertiesTab tab = new();
        Mock<ITypeDescriptorContext> mockTypeDescriptorContext = new(MockBehavior.Strict);
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
        PropertiesTab tab = new();
        Mock<ITypeDescriptorContext> mockTypeDescriptorContext = new(MockBehavior.Strict);
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
        PropertiesTab tab = new();
        PropertyDescriptorCollection properties = tab.GetProperties(null, new ClassWithDefaultProperty(), Array.Empty<Attribute>());
        Assert.Equal(3, properties.Count);
        Assert.NotNull(properties[nameof(ClassWithDefaultProperty.Value)]);
        Assert.NotNull(properties[nameof(ClassWithDefaultProperty.NotBrowsableProperty)]);
        Assert.NotNull(properties[nameof(ClassWithDefaultProperty.BrowsableProperty)]);
    }

    [Fact]
    public void PropertiesTab_GetProperties_InvokeObjectCustomAttributesNullContext_ReturnsExpected()
    {
        PropertiesTab tab = new();
        PropertyDescriptorCollection properties = tab.GetProperties(null, new ClassWithDefaultProperty(), [new BrowsableAttribute(false)]);
        Assert.Equal(1, properties.Count);
        Assert.NotNull(properties[nameof(ClassWithDefaultProperty.NotBrowsableProperty)]);
    }

    [Fact]
    public void PropertiesTab_GetProperties_InvokeNullObjectAttributesContext_ReturnsExpected()
    {
        PropertiesTab tab = new();
        Assert.Empty(tab.GetProperties(null, null, null));
    }

    private class NullGetPropertiesPropertiesTab : PropertiesTab
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

    [DefaultProperty(nameof(Value))]
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

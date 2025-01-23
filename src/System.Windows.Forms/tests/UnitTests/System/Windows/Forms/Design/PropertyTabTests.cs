// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using Moq;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Design.Tests;

// NB: doesn't require thread affinity
public class PropertyTabTests
{
    [Fact]
    public void PropertyTab_Ctor_Default()
    {
        SubPropertyTab tab = new();
        Assert.Null(tab.Bitmap);
        Assert.Null(tab.Components);
        Assert.Equal("TabName", tab.HelpKeyword);
    }

    [Fact]
    public void PropertyTab_Bitmap_GetValidMultipleTimes_ReturnsExpected()
    {
        CustomPropertyTab tab = new();
        Bitmap bitmap = tab.Bitmap;
        Assert.Equal(new Size(11, 22), tab.Bitmap.Size);
        Assert.Same(bitmap, tab.Bitmap);
    }

    [Fact]
    public void PropertyTab_Bitmap_GetInvalidMultipleTimes_ReturnsNull()
    {
        SubPropertyTab tab = new();
        Assert.Null(tab.Bitmap);
        Assert.Null(tab.Bitmap);
    }

    [Theory]
    [StringWithNullData]
    public void PropertyTab_CanExtend_Invoke_ReturnsTrue(object extendee)
    {
        SubPropertyTab tab = new();
        Assert.True(tab.CanExtend(extendee));
    }

    [Fact]
    public void PropertyTab_Dispose_WithBitmap_Success()
    {
        CustomPropertyTab tab = new();
        Bitmap bitmap = tab.Bitmap;

        tab.Dispose();
        Assert.Null(tab.Bitmap);
        Assert.Throws<ArgumentException>(() => bitmap.Size);

        // Dispose again.
        tab.Dispose();
        Assert.Null(tab.Bitmap);
        Assert.Throws<ArgumentException>(() => bitmap.Size);
    }

    [Fact]
    public void PropertyTab_Dispose_NoBitmap_Success()
    {
        SubPropertyTab tab = new();
        tab.Dispose();
        Assert.Null(tab.Bitmap);

        // Dispose again.
        tab.Dispose();
        Assert.Null(tab.Bitmap);
    }

    [Theory]
    [BoolData]
    public void PropertyTab_Dispose_NoBitmapDisposing_Success(bool disposing)
    {
        SubPropertyTab tab = new();
        tab.Dispose(disposing);
        Assert.Null(tab.Bitmap);

        // Dispose again.
        tab.Dispose(disposing);
        Assert.Null(tab.Bitmap);
    }

    [Theory]
    [BoolData]
    public void PropertyTab_Dispose_WithBitmapDisposing_Success(bool disposing)
    {
        CustomPropertyTab tab = new();
        Bitmap bitmap = tab.Bitmap;

        tab.Dispose(disposing);
        if (disposing)
        {
            Assert.Null(tab.Bitmap);
            Assert.Throws<ArgumentException>(() => bitmap.Size);
        }
        else
        {
            Assert.Same(bitmap, tab.Bitmap);
        }

        // Dispose again.
        tab.Dispose(disposing);
        if (disposing)
        {
            Assert.Null(tab.Bitmap);
            Assert.Throws<ArgumentException>(() => bitmap.Size);
        }
        else
        {
            Assert.Same(bitmap, tab.Bitmap);
        }
    }

    [Fact]
    public void PropertyTab_GetDefaultProperty_InvokeWithoutDefaultProperty_ReturnsExpected()
    {
        SubPropertyTab tab = new();
        Assert.Null(tab.GetDefaultProperty(new ClassWithoutDefaultProperty()));
    }

    [Fact]
    public void PropertyTab_GetDefaultProperty_InvokeWithDefaultProperty_ReturnsExpected()
    {
        SubPropertyTab tab = new();
        Assert.Equal("Value", tab.GetDefaultProperty(new ClassWithDefaultProperty()).Name);
    }

    [Fact]
    public void PropertyTab_GetDefaultProperty_InvokeNullComponent_ReturnsExpected()
    {
        SubPropertyTab tab = new();
        Assert.Null(tab.GetDefaultProperty(null));
    }

    [Theory]
    [StringWithNullData]
    public void PropertyTab_GetProperties_Invoke_ReturnsExpected(object component)
    {
        SubPropertyTab tab = new();
        PropertyDescriptorCollection result = TypeDescriptor.GetProperties(typeof(ClassWithDefaultProperty));
        int callCount = 0;
        tab.GetPropertiesAction = (componentResult, attributesResult) =>
        {
            Assert.Same(component, componentResult);
            Assert.Null(attributesResult);
            callCount++;

            return result;
        };
        Assert.Same(result, tab.GetProperties(component));
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> GetProperties_Attributes_TestData()
    {
        yield return new object[] { null, null, null };
        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        yield return new object[] { mockContext.Object, new(), Array.Empty<Attribute>() };
    }

    [Theory]
    [MemberData(nameof(GetProperties_Attributes_TestData))]
    public void PropertyTab_GetProperties_InvokeWithAttributes_ReturnsExpected(ITypeDescriptorContext context, object component, Attribute[] attributes)
    {
        SubPropertyTab tab = new();
        PropertyDescriptorCollection result = TypeDescriptor.GetProperties(typeof(ClassWithDefaultProperty));
        int callCount = 0;
        tab.GetPropertiesAction = (componentResult, attributesResult) =>
        {
            Assert.Same(component, componentResult);
            Assert.Same(attributes, attributesResult);
            callCount++;

            return result;
        };
        Assert.Same(result, tab.GetProperties(context, component, attributes));
        Assert.Equal(1, callCount);
    }

    private class SubPropertyTab : PropertyTab
    {
        public override string TabName => "TabName";

        public Func<object, Attribute[], PropertyDescriptorCollection> GetPropertiesAction { get; set; }

        public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
        {
            return GetPropertiesAction(component, attributes);
        }

        public new void Dispose(bool disposing) => base.Dispose(disposing);
    }

    private class CustomPropertyTab : SubPropertyTab
    {
    }

    private class ClassWithoutDefaultProperty
    {
        public int Value { get; set; }
    }

    [DefaultProperty(nameof(Value))]
    private class ClassWithDefaultProperty
    {
        public int Value { get; set; }
    }
}

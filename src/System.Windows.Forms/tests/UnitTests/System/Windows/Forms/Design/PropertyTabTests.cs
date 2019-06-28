// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class PropertyTabTests
    {
        [Fact]
        public void PropertyTab_Ctor_Default()
        {
            var tab = new SubPropertyTab();
            Assert.Null(tab.Bitmap);
            Assert.Null(tab.Components);
            Assert.Equal("TabName", tab.HelpKeyword);
        }

        [Fact]
        public void PropertyTab_Bitmap_GetValidMultipleTimes_ReturnsExpected()
        {
            var tab = new CustomPropertyTab();
            Bitmap bitmap = tab.Bitmap;
            Assert.Equal(new Size(11, 22), tab.Bitmap.Size);
            Assert.Same(bitmap, tab.Bitmap);
        }

        [Fact]
        public void PropertyTab_Bitmap_GetInvalidMultipleTimes_ReturnsNull()
        {
            var tab = new SubPropertyTab();
            Assert.Null(tab.Bitmap);
            Assert.Null(tab.Bitmap);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void PropertyTab_CanExtend_Invoke_ReturnsTrue(object extendee)
        {
            var tab = new SubPropertyTab();
            Assert.True(tab.CanExtend(extendee));
        }

        [Fact]
        public void PropertyTab_Dispose_WithBitmap_Success()
        {
            var tab = new CustomPropertyTab();
            Bitmap bitmap = tab.Bitmap;

            tab.Dispose();
            Assert.Null(tab.Bitmap);
            Assert.Throws<ArgumentException>(null, () => bitmap.Size);

            // Dispose again.
            tab.Dispose();
            Assert.Null(tab.Bitmap);
            Assert.Throws<ArgumentException>(null, () => bitmap.Size);
        }

        [Fact]
        public void PropertyTab_Dispose_NoBitmap_Success()
        {
            var tab = new SubPropertyTab();
            tab.Dispose();
            Assert.Null(tab.Bitmap);

            // Dispose again.
            tab.Dispose();
            Assert.Null(tab.Bitmap);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PropertyTab_Dispose_NoBitmapDisposing_Success(bool disposing)
        {
            var tab = new SubPropertyTab();
            tab.Dispose(disposing);
            Assert.Null(tab.Bitmap);

            // Dispose again.
            tab.Dispose(disposing);
            Assert.Null(tab.Bitmap);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PropertyTab_Dispose_WithBitmapDisposing_Success(bool disposing)
        {
            var tab = new CustomPropertyTab();
            Bitmap bitmap = tab.Bitmap;

            tab.Dispose(disposing);
            if (disposing)
            {
                Assert.Null(tab.Bitmap);
                Assert.Throws<ArgumentException>(null, () => bitmap.Size);
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
                Assert.Throws<ArgumentException>(null, () => bitmap.Size);
            }
            else
            {
                Assert.Same(bitmap, tab.Bitmap);
            }
        }

        [Fact]
        public void PropertyTab_GetDefaultProperty_Invoke_ReturnsExpected()
        {
            var tab = new SubPropertyTab();
            Assert.Equal("Value", tab.GetDefaultProperty(new ClassWithDefaultProperty()).Name);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void PropertyTab_GetProperties_Invoke_ReturnsExpected(object component)
        {
            var tab = new SubPropertyTab();
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
            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            yield return new object[] { mockContext.Object, new object(), Array.Empty<Attribute>() };
        }

        [Theory]
        [MemberData(nameof(GetProperties_Attributes_TestData))]
        public void PropertyTab_GetProperties_InvokeWithAttributes_ReturnsExpected(ITypeDescriptorContext context, object component, Attribute[] attributes)
        {
            var tab = new SubPropertyTab();
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

        [DefaultProperty(nameof(ClassWithDefaultProperty.Value))]
        private class ClassWithDefaultProperty
        {
            public int Value { get; set; }
        }
    }
}

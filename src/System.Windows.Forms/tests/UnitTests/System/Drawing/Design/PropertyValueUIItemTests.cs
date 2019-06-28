// Licensed to the .NET Foundation under one or more agreements.	
// The .NET Foundation licenses this file to you under the MIT license.	
// See the LICENSE file in the project root for more information.	

using System.Collections.Generic;
using System.ComponentModel;
using Xunit;

namespace System.Drawing.Design.Tests
{
    public class PropertyValueUIItemTests
    {
        public static IEnumerable<object[]> Ctor_Image_PropertyValueUIItemInvokeHandler_String_TestData()
        {
            yield return new object[] { new Bitmap(10, 10), (PropertyValueUIItemInvokeHandler)Dummy_PropertyValueUIItemInvokeHandler, null };
            yield return new object[] { new Bitmap(10, 10), (PropertyValueUIItemInvokeHandler)Dummy_PropertyValueUIItemInvokeHandler, string.Empty };
            yield return new object[] { new Bitmap(10, 10), (PropertyValueUIItemInvokeHandler)Dummy_PropertyValueUIItemInvokeHandler, "tooltip" };
        }

        [Theory]
        [MemberData(nameof(Ctor_Image_PropertyValueUIItemInvokeHandler_String_TestData))]
        public void PropertyValueUIItem_Ctor_Image_PropertyValueUIItemInvokeHandler_String(Image uiItemImage, PropertyValueUIItemInvokeHandler handler, string tooltip)
        {
            var item = new PropertyValueUIItem(uiItemImage, handler, tooltip);
            Assert.Same(uiItemImage, item.Image);
            Assert.Same(handler, item.InvokeHandler);
            Assert.Same(tooltip, item.ToolTip);
        }

        [Fact]
        public void PropertyValueUIItem_Ctor_NullUiItemImage_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("uiItemImage", () => new PropertyValueUIItem(null, Dummy_PropertyValueUIItemInvokeHandler, "tooltip"));
        }

        [Fact]
        public void PropertyValueUIItem_Ctor_NullHandler_ThrowsArgumentNullException()
        {
            using (var uiItemImage = new Bitmap(10, 10))
            {
                Assert.Throws<ArgumentNullException>("handler", () => new PropertyValueUIItem(uiItemImage, null, "tooltip"));
            }
        }

        private static void Dummy_PropertyValueUIItemInvokeHandler(ITypeDescriptorContext context, PropertyDescriptor propDesc, PropertyValueUIItem invokedItem) { }
    }
}

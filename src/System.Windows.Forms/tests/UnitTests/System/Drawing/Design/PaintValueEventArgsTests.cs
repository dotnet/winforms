// Licensed to the .NET Foundation under one or more agreements.	
// The .NET Foundation licenses this file to you under the MIT license.	
// See the LICENSE file in the project root for more information.	

using System.Collections.Generic;
using System.ComponentModel;
using Moq;
using Xunit;

namespace System.Drawing.Design.Tests
{
    public class PaintValueEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_ITypeDescriptorContext_Object_Graphics_Rectangle_TestData()
        {
            var image = new Bitmap (10, 10);
            Graphics graphics = Graphics.FromImage(image);
            yield return new object[] { null, null, graphics, Rectangle.Empty };
            yield return new object[] { new Mock<ITypeDescriptorContext>(MockBehavior.Strict).Object, new object(), graphics, new Rectangle(1, 2, 3, 4) };
        }

        [Theory]
        [MemberData(nameof(Ctor_ITypeDescriptorContext_Object_Graphics_Rectangle_TestData))]
        public void PaintValueEventArgs_Ctor_ITypeDescriptorContext_Object_Graphics_Rectangle(ITypeDescriptorContext context, object value, Graphics graphics, Rectangle bounds)
        {
            var e = new PaintValueEventArgs(context, value, graphics, bounds);
            Assert.Same(context, e.Context);
            Assert.Same(value, e.Value);
            Assert.Same(graphics, e.Graphics);
            Assert.Equal(bounds, e.Bounds);
        }

        [Fact]
        public void PaintValueEventArgs_Ctor_NullGraphics_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("graphics", () => new PaintValueEventArgs(null, new object(), null, Rectangle.Empty));
        }
    }
}

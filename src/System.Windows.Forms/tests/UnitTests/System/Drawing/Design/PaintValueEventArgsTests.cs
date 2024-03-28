// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Moq;

namespace System.Drawing.Design.Tests;

// NB: doesn't require thread affinity
public class PaintValueEventArgsTests
{
    public static IEnumerable<object[]> Ctor_ITypeDescriptorContext_Object_Rectangle_TestData()
    {
        yield return new object[] { null, null, Rectangle.Empty };
        yield return new object[] { new Mock<ITypeDescriptorContext>(MockBehavior.Strict).Object, new(), new Rectangle(1, 2, 3, 4) };
    }

    [Theory]
    [MemberData(nameof(Ctor_ITypeDescriptorContext_Object_Rectangle_TestData))]
    public void PaintValueEventArgs_Ctor_ITypeDescriptorContext_Object_Graphics_Rectangle(ITypeDescriptorContext context, object value, Rectangle bounds)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);

        PaintValueEventArgs e = new(context, value, graphics, bounds);
        Assert.Same(context, e.Context);
        Assert.Same(value, e.Value);
        Assert.Same(graphics, e.Graphics);
        Assert.Equal(bounds, e.Bounds);
    }

    [WinFormsFact]
    public void PaintValueEventArgs_Ctor_NullGraphics_ThrowsArgumentNullException()
    {
        AssertExtensions.Throws<ArgumentNullException>("graphics", () => new PaintValueEventArgs(null, new object(), null, Rectangle.Empty));
    }
}

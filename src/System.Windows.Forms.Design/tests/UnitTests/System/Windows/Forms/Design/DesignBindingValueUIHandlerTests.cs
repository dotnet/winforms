// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Design.Tests;

public class DesignBindingValueUIHandlerTests
{
    private readonly DesignBindingValueUIHandler _handler;

    public DesignBindingValueUIHandlerTests()
    {
        _handler = new();
    }

    [Fact]
    public void Dispose_DisposesDataBitmap_WhenDataBitmapIsNotNull()
    {
        using Bitmap bitmap = new(10, 10);
        _handler.TestAccessor().Dynamic._dataBitmap = bitmap;

        _handler.Dispose();

        Action action = () => bitmap.GetPixel(0, 0);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Dispose_DoesNotThrow_WhenDataBitmapIsNull()
    {
        Action action = _handler.Dispose;
        action.Should().NotThrow();
    }
}

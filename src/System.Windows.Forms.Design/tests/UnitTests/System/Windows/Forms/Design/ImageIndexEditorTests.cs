// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class ImageIndexEditorTests
{
    private readonly ImageIndexEditor _editor = new();
    private ITypeDescriptorContext? _context = new Mock<ITypeDescriptorContext>().Object;

    [Theory]
    [BoolData]
    public void GetPaintValueSupported_WhenContextIsNullOrNot_ReturnsTrue(bool hasContext)
    {
        _context = hasContext ? _context : null;

        bool result = _editor.GetPaintValueSupported(_context);

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("Text")]
    [InlineData(1)]
    public void PaintValue_WithStringOrIntOrNull_DoesNotThrow(object? value)
    {
        using Bitmap bitmap = new(10, 10);
        using var graphics = Graphics.FromImage(bitmap);
        PaintValueEventArgs paintValueEventArgs = new(_context, value, graphics, new Rectangle(0, 0, 10, 10));

        Action action = () => _editor.PaintValue(paintValueEventArgs);

        action.Should().NotThrow();
    }
}

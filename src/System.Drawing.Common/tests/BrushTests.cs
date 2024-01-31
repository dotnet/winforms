// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Tests;

public class BrushTests
{
    [Fact]
    public void SetNativeBrush_Brush_Success()
    {
        using SubBrush brush = new();
        brush.PublicSetNativeBrush(10);
        brush.PublicSetNativeBrush(IntPtr.Zero);

        brush.PublicSetNativeBrush(10);
        brush.PublicSetNativeBrush(IntPtr.Zero);
    }

    [Fact]
    public void Dispose_NoSuchEntryPoint_SilentyCatchesException()
    {
        SubBrush brush = new();
        brush.PublicSetNativeBrush(10);
        brush.Dispose();
    }

    private class SubBrush : Brush
    {
        public override object Clone() => this;
        public void PublicSetNativeBrush(nint brush) => SetNativeBrush(brush);

        protected override void Dispose(bool disposing)
        {
            // The pointers we're creating here are invalid and dangerous to dereference.
        }
    }
}

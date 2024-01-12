// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Gdi32Tests;

public class GetStockObjectTests
{
    [Theory]
    [InlineData((int)GET_STOCK_OBJECT_FLAGS.BLACK_BRUSH, 0x00000000, (uint)BRUSH_STYLE.BS_SOLID)]
    [InlineData((int)GET_STOCK_OBJECT_FLAGS.NULL_BRUSH, 0x00000000, (uint)BRUSH_STYLE.BS_HOLLOW)]
    [InlineData((int)GET_STOCK_OBJECT_FLAGS.WHITE_BRUSH, 0x00FFFFFF, (uint)BRUSH_STYLE.BS_SOLID)]
    public void GetStockBrushes(int id, uint color, uint brushStyle)
    {
        HGDIOBJ hgdiobj = PInvokeCore.GetStockObject((GET_STOCK_OBJECT_FLAGS)id);
        Assert.False(hgdiobj.IsNull);

        PInvokeCore.GetObject(hgdiobj, out LOGBRUSH logBrush);
        Assert.Equal(color, logBrush.lbColor);
        Assert.Equal((BRUSH_STYLE)brushStyle, logBrush.lbStyle);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Gdi32Tests
{
    public class GetStockObjectTests
    {
        [Theory]
        [InlineData((int)Gdi32.StockObject.BLACK_BRUSH, 0x00000000, (uint)Gdi32.BS.SOLID)]
        [InlineData((int)Gdi32.StockObject.NULL_BRUSH, 0x00000000, (uint)Gdi32.BS.HOLLOW)]
        [InlineData((int)Gdi32.StockObject.WHITE_BRUSH, 0x00FFFFFF, (uint)Gdi32.BS.SOLID)]
        public void GetStockBrushes(int id, uint color, uint brushStyle)
        {
            Gdi32.HGDIOBJ hgdiobj = Gdi32.GetStockObject((Gdi32.StockObject)id);
            Assert.False(hgdiobj.IsNull);

            Gdi32.GetObjectW(hgdiobj, out Gdi32.LOGBRUSH logBrush);
            Assert.Equal(color, logBrush.lbColor);
            Assert.Equal((Gdi32.BS)brushStyle, logBrush.lbStyle);
        }
    }
}

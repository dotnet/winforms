// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests.Interop.GdiPlus
{
    public class ARGBTests
    {
        [Theory]
        [InlineData(0x0000_0000, 0x00, 0x00, 0x00, 0x00)]
        [InlineData(0xFFFF_FFFF, 0xFF, 0xFF, 0xFF, 0xFF)]
        [InlineData(0xFF00_0000, 0x00, 0x00, 0x00, 0xFF)]
        [InlineData(0x00AA_0000, 0xAA, 0x00, 0x00, 0x00)]
        [InlineData(0x0000_BB00, 0x00, 0xBB, 0x00, 0x00)]
        [InlineData(0x0000_00CC, 0x00, 0x00, 0xCC, 0x00)]
        public void Construction_Raw(uint value, byte r, byte g, byte b, byte a)
        {
            ARGB fromValue = new ARGB((int)value);
            Assert.Equal(a, fromValue.A);
            Assert.Equal(r, fromValue.R);
            Assert.Equal(g, fromValue.G);
            Assert.Equal(b, fromValue.B);
            ARGB fromBytes = new ARGB(a, r, g, b);
            Assert.Equal((int)value, fromBytes.Value);
        }

        [Theory]
        [MemberData(nameof(Colors))]
        public void ToFromColor(Color color)
        {
            ARGB argb = color;
            Assert.Equal(color.ToArgb(), argb.Value);
            Color backAgain = argb;
            Assert.Equal(color.ToArgb(), backAgain.ToArgb());
        }

        public static TheoryData<Color> Colors =>
            new TheoryData<Color>
            {
                Color.CornflowerBlue,
                Color.Transparent,
                Color.BurlyWood
            };
    }
}

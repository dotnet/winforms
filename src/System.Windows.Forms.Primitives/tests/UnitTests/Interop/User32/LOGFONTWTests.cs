// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms.Primitives.Tests.Interop.User32
{
    public class LOGFONTWTests
    {
        [Fact]
        public unsafe void LogFont_Size()
        {
            Assert.Equal(92, sizeof(LOGFONTW));
        }

        [Fact]
        public unsafe void LogFont_FaceName()
        {
            LOGFONTW logFont = default;
            logFont.FaceName = "TwoFace";
            Assert.Equal("TwoFace", logFont.FaceName.ToString());

            // Set a smaller name to make sure it gets terminated properly.
            logFont.FaceName = "Face";
            Assert.Equal("Face", logFont.FaceName.ToString());

            // LOGFONT has space for 32 characters, we want to see it gets
            // cut to 31 to make room for the null.
            string bigString = new string('*', 32);

            logFont.FaceName = bigString;
            Assert.True(logFont.FaceName.SequenceEqual(bigString.AsSpan().Slice(1)));
        }

        [Fact]
        public unsafe void CreateFontIndirect()
        {
            LOGFONTW logFont = default;
            Gdi32.HFONT handle = Gdi32.CreateFontIndirectW(ref logFont);
            Assert.False(handle.IsNull);
            Assert.True(Gdi32.DeleteObject(handle).IsTrue());
        }
    }
}

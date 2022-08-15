// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

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
            logFont.lfFaceName = "TwoFace";
            Assert.Equal("TwoFace", logFont.lfFaceName.ToString());

            // Set a smaller name to make sure it gets terminated properly.
            logFont.lfFaceName = "Face";
            Assert.Equal("Face", logFont.lfFaceName.ToString());

            // LOGFONT has space for 32 characters, we want to see it gets
            // cut to 31 to make room for the null.
            string bigString = new('*', 32);

            logFont.lfFaceName = bigString;
            Assert.True(logFont.lfFaceName.AsSpan().SequenceEqual(bigString.AsSpan().Slice(1)));
        }

        [Fact]
        public unsafe void CreateFontIndirect()
        {
            LOGFONTW logFont = default;
            HFONT handle = PInvoke.CreateFontIndirect(&logFont);
            Assert.False(handle.IsNull);
            Assert.True(PInvoke.DeleteObject(handle));
        }
    }
}

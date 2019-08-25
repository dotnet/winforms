// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.Internal;
using Xunit;
using static Interop;


namespace System.Windows.Forms.Tests.InteropTests
{
    public class TextTests
    {
        [Fact]
        public unsafe void CharFormat_Size()
        {
            Assert.Equal(92, sizeof(NativeMethods.CHARFORMATW));
        }

        [Fact]
        public unsafe void CharFormat_FaceName()
        {
            NativeMethods.CHARFORMATW charFormat = default;
            charFormat.FaceName = "TwoFace";
            Assert.Equal("TwoFace", charFormat.FaceName.ToString());

            // Set a smaller name to make sure it gets terminated properly.
            charFormat.FaceName = "Face";
            Assert.Equal("Face", charFormat.FaceName.ToString());

            // CHARFORMAT has space for 32 characters, we want to see it gets
            // cut to 31 to make room for the null.
            string bigString = new string('*', 32);

            charFormat.FaceName = bigString;
            Assert.True(charFormat.FaceName.SequenceEqual(bigString.AsSpan().Slice(1)));
        }

        [Fact]
        public unsafe void LogFont_Size()
        {
            Assert.Equal(92, sizeof(NativeMethods.LOGFONTW));
        }

        [Fact]
        public unsafe void LogFont_FaceName()
        {
            NativeMethods.LOGFONTW logFont = default;
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
            NativeMethods.LOGFONTW logFont = default;
            IntPtr handle = IntUnsafeNativeMethods.CreateFontIndirectW(ref logFont);
            Assert.NotEqual(IntPtr.Zero, handle);
            Assert.True(Gdi32.DeleteObject(handle) != BOOL.FALSE);
        }
    }
}

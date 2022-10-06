// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Windows.Win32.Graphics.Gdi
{
    internal partial struct LOGFONTW
    {
        public const int LF_FACESIZE = 32;

        // LOGFONTW has space for 32 characters, we use this to ensure we cut to 31 to make room for the null.
        // We should never reference lfFaceName directly and use this property instead.
        public ReadOnlySpan<char> FaceName
        {
            [UnscopedRef]
            get => lfFaceName.AsSpan().SliceAtFirstNull();
            set => SpanHelpers.CopyAndTerminate(value, lfFaceName.AsSpan());
        }

        // Font.ToLogFont will copy LOGFONT into a blittable struct,
        // but we need to box it upfront so we can unbox.

        public static LOGFONTW FromFont(Font font)
        {
            object logFont = new LOGFONTW();
            font.ToLogFont(logFont);
            return (LOGFONTW)logFont;
        }

        public static LOGFONTW FromFont(Font font, global::System.Drawing.Graphics graphics)
        {
            object logFont = new LOGFONTW();
            font.ToLogFont(logFont, graphics);
            return (LOGFONTW)logFont;
        }
    }
}

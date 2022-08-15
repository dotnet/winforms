// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace Windows.Win32.Graphics.Gdi
{
    internal partial struct LOGFONTW
    {
        // Font.ToLogFont will copy LOGFONT into a blittable struct,
        // but we need to box it upfront so we can unbox.

        public static LOGFONTW FromFont(Font font)
        {
            LOGFONTW logFont = default;
            font.ToLogFont(logFont);
            return logFont;
        }

        public static LOGFONTW FromFont(Font font, global::System.Drawing.Graphics graphics)
        {
            LOGFONTW logFont = default;
            font.ToLogFont(logFont, graphics);
            return logFont;
        }
    }
}

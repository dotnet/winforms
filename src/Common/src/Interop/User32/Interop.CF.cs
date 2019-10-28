// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum CF : uint
        {
            TEXT = 1,
            BITMAP = 2,
            METAFILEPICT = 3,
            SYLK = 4,
            DIF = 5,
            TIFF = 6,
            OEMTEXT = 7,
            DIB = 8,
            PALETTE = 9,
            PENDATA = 10,
            RIFF = 11,
            WAVE = 12,
            UNICODETEXT = 13,
            ENHMETAFILE = 14,
            HDROP = 15,
            LOCALE = 16,
            DIBV5 = 17,
            OWNERDISPLAY = 0x0080,
            DSPTEXT = 0x0081,
            DSPBITMAP = 0x0082,
            DSPMETAFILEPICT = 0x0083,
            DSPENHMETAFILE = 0x008E,
            PRIVATEFIRST = 0x0200,
            PRIVATELAST = 0x02FF,
            GDIOBJFIRST = 0x0300,
            GDIOBJLAST = 0x03FF,
        }
    }
}

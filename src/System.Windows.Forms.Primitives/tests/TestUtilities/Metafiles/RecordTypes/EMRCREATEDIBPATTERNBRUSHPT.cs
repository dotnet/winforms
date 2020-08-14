// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Runtime.InteropServices;

namespace System.Windows.Forms.Metafiles
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMRCREATEDIBPATTERNBRUSHPT
    {
        public EMR emr;
        public uint ihBrush;            // Brush handle index
        public uint iUsage;             // Bitmap info color table usage
        public uint offBmi;             // Offset to the BITMAPINFO structure
        public uint cbBmi;              // Size of the BITMAPINFO structure
                                        // The bitmap info is followed by the bitmap
                                        // bits to form a packed DIB.
        public uint offBits;            // Offset to the bitmap bits
        public uint cbBits;             // Size of the bitmap bits

        public override string ToString()
            => $"[{nameof(EMRCREATEDIBPATTERNBRUSHPT)}] Index: {ihBrush}";
    }
}

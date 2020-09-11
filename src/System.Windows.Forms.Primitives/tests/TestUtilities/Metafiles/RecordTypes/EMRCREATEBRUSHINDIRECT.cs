// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMRCREATEBRUSHINDIRECT
    {
        public EMR emr;
        public uint ihBrush;
        public LOGBRUSH32 lb;

        public override string ToString()
            => $@"[{nameof(EMRCREATEBRUSHINDIRECT)}] Index: {ihBrush} Style: {lb.lbStyle} Color: {
                lb.lbColor.ToSystemColorString()}";

        // This structure is used exclusively in EMRCREATEBRUSHINDIRECT
        [StructLayout(LayoutKind.Sequential)]
        internal struct LOGBRUSH32
        {
            public Gdi32.BS lbStyle;
            public COLORREF lbColor;
            public uint lbHatch;

            public static implicit operator Gdi32.LOGBRUSH(LOGBRUSH32 logbrush) => new Gdi32.LOGBRUSH
            {
                lbStyle = logbrush.lbStyle,
                lbColor = logbrush.lbColor,
                lbHatch = (IntPtr)logbrush.lbHatch
            };
        }
    }
}

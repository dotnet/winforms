// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Runtime.InteropServices;

namespace System.Windows.Forms.Metafiles;

[StructLayout(LayoutKind.Sequential)]
internal struct EMRCREATEBRUSHINDIRECT
{
    public EMR emr;
    public uint ihBrush;
    public LOGBRUSH32 lb;

    public override readonly string ToString()
        => $@"[{nameof(EMRCREATEBRUSHINDIRECT)}] Index: {ihBrush} Style: {lb.lbStyle} Color: {lb.lbColor.ToSystemColorString()}";

    // This structure is used exclusively in EMRCREATEBRUSHINDIRECT
    [StructLayout(LayoutKind.Sequential)]
    internal struct LOGBRUSH32
    {
        public BRUSH_STYLE lbStyle;
        public COLORREF lbColor;
        public uint lbHatch;

        public static implicit operator LOGBRUSH(LOGBRUSH32 logbrush) => new()
        {
            lbStyle = logbrush.lbStyle,
            lbColor = logbrush.lbColor,
            lbHatch = logbrush.lbHatch
        };
    }
}

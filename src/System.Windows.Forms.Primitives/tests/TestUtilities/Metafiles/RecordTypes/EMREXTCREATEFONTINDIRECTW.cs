// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Runtime.InteropServices;

namespace System.Windows.Forms.Metafiles;

[StructLayout(LayoutKind.Sequential)]
internal struct EMREXTCREATEFONTINDIRECTW
{
    public EMR emr;
    public uint ihFont;
    public EXTLOGFONTW elfw;

    public override string ToString()
        => $@"[{nameof(EMREXTCREATEFONTINDIRECTW)}] Index: {ihFont} FaceName: '{elfw.elfLogFont.FaceName.ToString()}' Height: {elfw.elfLogFont.lfHeight} Weight: FW_{elfw.elfLogFont.lfWeight}";
}

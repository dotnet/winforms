// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMREXTCREATEFONTINDIRECTW
    {
        public EMR emr;
        public uint ihFont;
        public User32.EXTLOGFONTW elfw;

        public override string ToString()
            => $@"[{nameof(EMREXTCREATEFONTINDIRECTW)}] Index: {ihFont} FaceName: '{elfw.elfLogFont.FaceName.ToString()
                }' Height: {elfw.elfLogFont.lfHeight} Weight: FW_{elfw.elfLogFont.lfWeight}";
    }
}

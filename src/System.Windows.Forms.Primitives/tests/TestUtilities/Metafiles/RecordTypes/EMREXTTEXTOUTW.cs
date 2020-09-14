// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMREXTTEXTOUTW
    {
        public EMR emr;
        public RECT rclBounds;           // Inclusive-inclusive bounds in device units
        public GM iGraphicsMode;         // Current graphics mode
        public float exScale;            // X and Y scales from Page units to .01mm units
        public float eyScale;            //   if graphics mode is GM_COMPATIBLE.
        public EMRTEXT emrtext;          // This is followed by the string and spacing

        public override string ToString()
            => $@"[{nameof(EMREXTTEXTOUTW)}] Bounds: {rclBounds} Text: '{emrtext.GetText().ToString()}'";

        internal enum GM : uint
        {
            COMPATIBLE = 0x00000001,
            ADVANCED = 0x00000002
        }
    }
}

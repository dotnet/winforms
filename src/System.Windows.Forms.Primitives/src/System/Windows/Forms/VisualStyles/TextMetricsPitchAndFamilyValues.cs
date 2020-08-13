// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms.VisualStyles
{
    [Flags]
    public enum TextMetricsPitchAndFamilyValues
    {
        FixedPitch = Gdi32.TMPF.FIXED_PITCH,
        Vector = Gdi32.TMPF.VECTOR,
        TrueType = Gdi32.TMPF.TRUETYPE,
        Device = Gdi32.TMPF.DEVICE
    }
}

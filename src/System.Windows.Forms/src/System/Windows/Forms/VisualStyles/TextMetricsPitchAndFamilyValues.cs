// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles
{
    [Flags]
    public enum TextMetricsPitchAndFamilyValues
    {
        FixedPitch = 0x01,
        Vector = 0x02,
        TrueType = 0x04,
        Device = 0x08

        //		#define TMPF_FIXED_PITCH    0x01
        //		#define TMPF_VECTOR             0x02
        //		#define TMPF_DEVICE             0x08
        //		#define TMPF_TRUETYPE       0x04
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.VisualStyles;

[Flags]
public enum TextMetricsPitchAndFamilyValues
{
    FixedPitch = TMPF_FLAGS.TMPF_FIXED_PITCH,
    Vector = TMPF_FLAGS.TMPF_VECTOR,
    TrueType = TMPF_FLAGS.TMPF_TRUETYPE,
    Device = TMPF_FLAGS.TMPF_DEVICE
}

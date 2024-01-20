// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// font style constants (sdkinc\GDIplusEnums.h)

namespace System.Drawing;

/// <summary>
///  Specifies style information applied to String Digit Substitute.
/// </summary>
public enum StringDigitSubstitute
{
    User = GdiPlus.StringDigitSubstitute.StringDigitSubstituteUser,  // As NLS setting
    None = GdiPlus.StringDigitSubstitute.StringDigitSubstituteNone,
    National = GdiPlus.StringDigitSubstitute.StringDigitSubstituteNational,
    Traditional = GdiPlus.StringDigitSubstitute.StringDigitSubstituteTraditional
}

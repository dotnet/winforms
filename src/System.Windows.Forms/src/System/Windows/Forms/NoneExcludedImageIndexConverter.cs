// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Just returns false for IncludeNoneAsStandardValue
/// </summary>
internal sealed class NoneExcludedImageIndexConverter : ImageIndexConverter
{
    protected override bool IncludeNoneAsStandardValue
    {
        get
        {
            return false;
        }
    }
}

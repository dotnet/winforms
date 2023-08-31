// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms;

internal partial class SpecialFolderEnumConverter
{
    private class SpecialFolderEnumComparer : IComparer
    {
        public static readonly SpecialFolderEnumComparer Default = new();

        public int Compare(object? a, object? b)
        {
            return string.Compare(a?.ToString(), b?.ToString(), StringComparison.InvariantCulture);
        }
    }
}

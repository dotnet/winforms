// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.MSInternal", "CA905:SystemAndMicrosoftNamespacesRequireApproval", Scope = "namespace", Target = "System.Windows.Forms.VisualStyles")]

namespace System.Windows.Forms.VisualStyles
{
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum CheckBoxState
    {
        UncheckedNormal = 1,
        UncheckedHot = 2,
        UncheckedPressed = 3,
        UncheckedDisabled = 4,
        CheckedNormal = 5,
        CheckedHot = 6,
        CheckedPressed = 7,
        CheckedDisabled = 8,
        MixedNormal = 9,
        MixedHot = 10,
        MixedPressed = 11,
        MixedDisabled = 12
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.MSInternal", "CA905:SystemAndMicrosoftNamespacesRequireApproval", Scope = "namespace", Target = "System.Windows.Forms.VisualStyles")]

namespace System.Windows.Forms.VisualStyles
{
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Maps to native enum.")]
    public enum StringProperty
    {
        Text = 3201
        //TM_PROP(3201, TMT, TEXT,              STRING)
    }
}

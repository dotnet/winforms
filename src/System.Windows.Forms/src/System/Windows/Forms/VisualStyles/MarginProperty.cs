// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.MSInternal", "CA905:SystemAndMicrosoftNamespacesRequireApproval", Scope = "namespace", Target = "System.Windows.Forms.VisualStyles")]

namespace System.Windows.Forms.VisualStyles
{
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Maps to native enum.")]
    public enum MarginProperty
    {
    	SizingMargins = 3601,
    	ContentMargins = 3602,
    	CaptionMargins = 3603
    //		TM_PROP(3601, TMT, SIZINGMARGINS,     MARGINS)    // margins used for 9-grid sizing
    //		TM_PROP(3602, TMT, CONTENTMARGINS,    MARGINS)    // margins that define where content can be placed
    //		TM_PROP(3603, TMT, CAPTIONMARGINS,    MARGINS)    // margins that define where caption text can be placed
    }
}

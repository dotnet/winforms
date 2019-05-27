// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.MSInternal", "CA905:SystemAndMicrosoftNamespacesRequireApproval", Scope = "namespace", Target = "System.Windows.Forms.VisualStyles")]

namespace System.Windows.Forms.VisualStyles
{
    public enum FillType
    {
        Solid = 0,
        VerticalGradient = 1,
        HorizontalGradient = 2,
        RadialGradient = 3,
        TileImage = 4,
        //		TM_ENUM(0, FT, SOLID)
        //		TM_ENUM(1, FT, VERTGRADIENT)
        //		TM_ENUM(2, FT, HORZGRADIENT)
        //		TM_ENUM(3, FT, RADIALGRADIENT)
        //		TM_ENUM(4, FT, TILEIMAGE)
    }
}

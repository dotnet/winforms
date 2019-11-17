// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms.VisualStyles
{
    public enum EdgeStyle
    {
        Raised = (int)User32.EDGE.RAISED,
        Sunken = (int)User32.EDGE.SUNKEN,
        Etched = (int)User32.EDGE.ETCHED,
        Bump = (int)User32.EDGE.BUMP
    }
}

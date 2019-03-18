// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design.Editors
{
    [ComVisible(true)]
    [Guid("76d12d7e-b227-4417-9ce2-42642ffa896a")]
    internal enum ExpandCollapseState
    {
        Collapsed = 0,
        Expanded = 1,
        PartiallyExpanded = 2,
        LeafNode = 3
    }
}

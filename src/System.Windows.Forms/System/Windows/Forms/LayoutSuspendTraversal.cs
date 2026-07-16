// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#if NET11_0_OR_GREATER
/// <summary>
///  Specifies how layout suspension traverses a control tree while painting is suspended.
/// </summary>
public enum LayoutSuspendTraversal
{
    /// <summary>
    ///  Does not suspend layout.
    /// </summary>
    None = 0,

    /// <summary>
    ///  Suspends layout only for the target control.
    /// </summary>
    TopLevelOnly = 1,

    /// <summary>
    ///  Suspends layout for the target control and all its descendants.
    /// </summary>
    Traverse = 2,
}
#endif

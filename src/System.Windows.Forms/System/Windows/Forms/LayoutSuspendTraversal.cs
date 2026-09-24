// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#if NET11_0_OR_GREATER
/// <summary>
///  Specifies which controls in a target's control tree suspend layout while painting is suspended.
/// </summary>
public enum LayoutSuspendTraversal
{
    /// <summary>
    ///  Does not suspend layout. Painting is still suspended; use this value (or the parameterless
    ///  <see cref="ControlMutationExtensions.SuspendPainting(ISupportSuspendPainting)"/> overload) when
    ///  no layout suspension is wanted.
    /// </summary>
    None = 0,

    /// <summary>
    ///  Suspends layout for the target control only.
    /// </summary>
    TargetOnly = 1,

    /// <summary>
    ///  Suspends layout for the target control and every control in its subtree.
    /// </summary>
    TargetAndDescendants = 2,
}
#endif

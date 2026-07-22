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
    ///  Suspends layout for the target control only. The target does not re-lay-out its own children
    ///  while the scope is active; a nested container child can still perform its own layout.
    /// </summary>
    TargetOnly = 0,

    /// <summary>
    ///  Suspends layout for the target control and each of its immediate child controls. Because a
    ///  container suspends the layout of its own children, this additionally holds the layout of the
    ///  target's grandchildren, but not of any deeper descendants.
    /// </summary>
    TargetAndChildren = 1,

    /// <summary>
    ///  Suspends layout for the target control and every control in its subtree.
    /// </summary>
    TargetAndDescendants = 2,
}
#endif

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

using System.ComponentModel;

#if NET11_0_OR_GREATER
/// <summary>
///  Provides extension methods for batching synchronous WinForms UI mutations.
/// </summary>
public static class ControlMutationExtensions
{
    /// <summary>
    ///  Suspends painting for the specified target until the returned scope is disposed.
    /// </summary>
    /// <param name="target">The target whose painting should be suspended.</param>
    /// <returns>A scope that resumes painting when disposed.</returns>
    public static SuspendPaintingScope SuspendPainting(this ISupportSuspendPainting target)
        => new(target);

    /// <summary>
    ///  Suspends painting and the selected layout work until the returned scope is disposed.
    /// </summary>
    /// <param name="target">The target whose painting and layout should be suspended.</param>
    /// <param name="layoutSuspendTraversal">
    ///  A value that specifies which controls in the target's control tree should suspend layout.
    /// </param>
    /// <returns>A scope that resumes layout and painting when disposed.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidEnumArgumentException">
    ///  <paramref name="layoutSuspendTraversal"/> is not a valid <see cref="LayoutSuspendTraversal"/> value.
    /// </exception>
    /// <exception cref="InvalidOperationException"><paramref name="target"/> is not a <see cref="Control"/>.</exception>
    public static SuspendPaintingScope SuspendPainting(
        this ISupportSuspendPainting target,
        LayoutSuspendTraversal layoutSuspendTraversal)
        => new(target, layoutSuspendTraversal);

    /// <summary>
    ///  Suspends painting and layout for selected controls until the returned scope is disposed.
    /// </summary>
    /// <param name="target">The target whose painting and selected layout should be suspended.</param>
    /// <param name="suspendLayoutContainerFilter">
    ///  A predicate that returns <see langword="true"/> for each control whose layout should be suspended.
    /// </param>
    /// <returns>A scope that resumes layout and painting when disposed.</returns>
    /// <exception cref="ArgumentNullException">
    ///  <paramref name="target"/> or <paramref name="suspendLayoutContainerFilter"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException"><paramref name="target"/> is not a <see cref="Control"/>.</exception>
    public static SuspendPaintingScope SuspendPainting(
        this ISupportSuspendPainting target,
        Func<Control, bool> suspendLayoutContainerFilter)
        => new(target, suspendLayoutContainerFilter);
}
#endif

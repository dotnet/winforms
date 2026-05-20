// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

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
    ///  Suspends relocation work for the specified target until the returned scope is disposed.
    /// </summary>
    /// <param name="target">The target whose relocation work should be suspended.</param>
    /// <returns>A scope that resumes relocation work when disposed.</returns>
    public static SuspendRelocationScope SuspendRelocation(this ISupportSuspendRelocation target)
        => new(target);
}
#endif

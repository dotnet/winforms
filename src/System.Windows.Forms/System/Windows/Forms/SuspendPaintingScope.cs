// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#if NET11_0_OR_GREATER
/// <summary>
///  Suspends painting for a target until the scope is disposed.
/// </summary>
public readonly ref struct SuspendPaintingScope
{
    private readonly ISupportSuspendPainting? _target;

    /// <summary>
    ///  Initializes a new instance of the <see cref="SuspendPaintingScope"/> struct.
    /// </summary>
    /// <param name="target">The target whose painting should be suspended.</param>
    public SuspendPaintingScope(ISupportSuspendPainting? target)
    {
        _target = target;
        _target?.BeginSuspendPainting();
    }

    /// <summary>
    ///  Resumes painting for the target associated with this scope.
    /// </summary>
    public void Dispose() => _target?.EndSuspendPainting();
}
#endif

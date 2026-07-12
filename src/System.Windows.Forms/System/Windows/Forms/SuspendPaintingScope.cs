// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#if NET11_0_OR_GREATER
/// <summary>
///  Suspends painting for a target until the scope is disposed.
/// </summary>
/// <remarks>
///  <para>
///   This is a sealed class rather than a <c>ref struct</c> so the scope can span an
///   <see langword="await"/> in an asynchronous UI event handler (for example, suspending painting for
///   the duration of an async data reload). <see cref="Dispose"/> is idempotent: disposing the scope more
///   than once only resumes painting once.
///  </para>
/// </remarks>
public sealed class SuspendPaintingScope : IDisposable
{
    private ISupportSuspendPainting? _target;

    /// <summary>
    ///  Initializes a new instance of the <see cref="SuspendPaintingScope"/> class.
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
    public void Dispose()
    {
        // Idempotent: only the first Dispose call should resume painting, since the underlying
        // refcount on the target was only incremented once, in the constructor.
        _target?.EndSuspendPainting();
        _target = null;
    }
}
#endif

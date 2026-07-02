// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#if NET11_0_OR_GREATER
/// <summary>
///  Suspends relocation work for a target until the scope is disposed.
/// </summary>
/// <remarks>
///  <para>
///   This is a sealed class rather than a <c>ref struct</c> so the scope can span an
///   <see langword="await"/> in an asynchronous UI event handler. <see cref="Dispose"/> is idempotent:
///   disposing the scope more than once only resumes relocation work once.
///  </para>
/// </remarks>
public sealed class SuspendRelocationScope : IDisposable
{
    private ISupportSuspendRelocation? _target;

    /// <summary>
    ///  Initializes a new instance of the <see cref="SuspendRelocationScope"/> class.
    /// </summary>
    /// <param name="target">The target whose relocation work should be suspended.</param>
    public SuspendRelocationScope(ISupportSuspendRelocation? target)
    {
        _target = target;
        _target?.BeginSuspendRelocation();
    }

    /// <summary>
    ///  Resumes relocation work for the target associated with this scope.
    /// </summary>
    public void Dispose()
    {
        _target?.EndSuspendRelocation();
        _target = null;
    }
}
#endif

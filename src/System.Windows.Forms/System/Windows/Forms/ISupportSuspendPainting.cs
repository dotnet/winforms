// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#if NET11_0_OR_GREATER
/// <summary>
///  Provides methods for temporarily suspending and resuming painting.
/// </summary>
/// <remarks>
///  <para>
///   <see cref="Control"/> implements this interface explicitly, so a control instance does not surface
///   <see cref="BeginSuspendPainting"/> and <see cref="EndSuspendPainting"/> directly. Callers instead
///   obtain a disposable suspension scope through
///   <see cref="ControlMutationExtensions.SuspendPainting(ISupportSuspendPainting)"/>, which pairs the
///   begin/end calls with deterministic <see cref="IDisposable"/> cleanup. Types deriving from
///   <see cref="Control"/> customize the behavior by overriding
///   <see cref="Control.BeginSuspendPaintingCore"/> and <see cref="Control.EndSuspendPaintingCore"/>
///   rather than the explicit interface members.
///  </para>
/// </remarks>
public interface ISupportSuspendPainting
{
    /// <summary>
    ///  Begins a painting suspension region.
    /// </summary>
    void BeginSuspendPainting();

    /// <summary>
    ///  Ends a painting suspension region.
    /// </summary>
    void EndSuspendPainting();
}
#endif

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#if NET11_0_OR_GREATER
/// <summary>
///  Provides methods for temporarily suspending and resuming painting.
/// </summary>
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

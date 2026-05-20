// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#if NET11_0_OR_GREATER
/// <summary>
///  Provides methods for temporarily suspending and resuming relocation work.
/// </summary>
public interface ISupportSuspendRelocation
{
    /// <summary>
    ///  Begins a relocation suspension region.
    /// </summary>
    void BeginSuspendRelocation();

    /// <summary>
    ///  Ends a relocation suspension region.
    /// </summary>
    void EndSuspendRelocation();
}
#endif

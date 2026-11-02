// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Animation;

/// <summary>
///  Provides timing information for a single animation frame tick.
/// </summary>
internal readonly struct HighPrecisionTimerTick
{
    /// <summary>
    ///  The absolute timestamp of this tick from the timer's epoch.
    /// </summary>
    public TimeSpan Timestamp { get; init; }

    /// <summary>
    ///  The elapsed time since the last tick delivered to this registration.
    /// </summary>
    public TimeSpan Elapsed { get; init; }

    /// <summary>
    ///  The number of frames that were dropped (coalesced) since the last delivered tick.
    /// </summary>
    public int DroppedFrames { get; init; }

    /// <summary>
    ///  The zero-based frame index for this registration.
    /// </summary>
    public long FrameIndex { get; init; }
}

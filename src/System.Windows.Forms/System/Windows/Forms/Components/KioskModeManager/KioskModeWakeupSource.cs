// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#if NET11_0_OR_GREATER
/// <summary>
///  Specifies the source of a <see cref="KioskModeManager.Wakeup"/>
///  notification.
/// </summary>
public enum KioskModeWakeupSource
{
    /// <summary>
    ///  The wakeup notification was caused by keyboard activity.
    /// </summary>
    Keyboard = 0,

    /// <summary>
    ///  The wakeup notification was caused by mouse activity.
    /// </summary>
    Mouse = 1,

    /// <summary>
    ///  The wakeup notification was caused by the system resuming from a low
    ///  power state.
    /// </summary>
    PowerResume = 2,

    /// <summary>
    ///  The wakeup notification was caused by a Windows session activity, such
    ///  as logon or unlock.
    /// </summary>
    Session = 3,
}
#endif

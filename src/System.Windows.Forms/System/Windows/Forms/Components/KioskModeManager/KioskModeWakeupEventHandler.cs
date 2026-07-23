// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#if NET11_0_OR_GREATER
/// <summary>
///  Represents the method that will handle the
///  <see cref="KioskModeManager.Wakeup"/> event.
/// </summary>
/// <param name="sender">The source of the event.</param>
/// <param name="e">A <see cref="KioskModeWakeupEventArgs"/> that contains the event data.</param>
public delegate void KioskModeWakeupEventHandler(object? sender, KioskModeWakeupEventArgs e);
#endif

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#if NET11_0_OR_GREATER
/// <summary>
///  Provides data for the <see cref="KioskModeManager.Wakeup"/> event.
/// </summary>
/// <remarks>
///  <para>
///   The <see cref="Source"/> property identifies the kind of activity the
///   component observed. Applications can use this value to distinguish direct
///   user activity, such as mouse or keyboard input, from system activity such
///   as power resume or session unlock.
///  </para>
/// </remarks>
public class KioskModeWakeupEventArgs : EventArgs
{
    /// <summary>
    ///  Initializes a new instance of the
    ///  <see cref="KioskModeWakeupEventArgs"/> class.
    /// </summary>
    /// <param name="source">The source of the wakeup notification.</param>
    public KioskModeWakeupEventArgs(KioskModeWakeupSource source)
    {
        SourceGenerated.EnumValidator.Validate(source, nameof(source));
        Source = source;
    }

    /// <summary>
    ///  Gets the source of the wakeup notification.
    /// </summary>
    public KioskModeWakeupSource Source { get; }
}
#endif

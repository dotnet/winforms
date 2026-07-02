// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#if NET11_0_OR_GREATER
/// <summary>
///  Specifies how the application reacts to changes in the Windows Accessibility text-scale setting.
/// </summary>
/// <remarks>
///  <para>
///   The current implementation supports only <see cref="Unaware"/> and <see cref="Notify"/>. A future release may add
///   an automatic reaction mode.
///  </para>
/// </remarks>
public enum SystemTextSizeAwareness
{
    /// <summary>
    ///  Default. The application does not raise any text-scale-change notification.
    ///  Fully back-compatible behavior.
    /// </summary>
    Unaware = 0,

    /// <summary>
    ///  The application raises <see cref="Application.SystemTextSizeChanged"/> and
    ///  <see cref="Form.SystemTextSizeChanged"/> when the setting changes.
    ///  The application decides how to respond.
    /// </summary>
    Notify = 1
}
#endif

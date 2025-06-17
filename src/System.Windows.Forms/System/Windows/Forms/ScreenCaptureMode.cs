// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Enumeration defining the behavior when a form or control is captured in a screenshot.
/// </summary>
public enum ScreenCaptureMode
{
    /// <summary>
    ///  The form or control can be captured normally in screenshots. Default.
    /// </summary>
    Allow = 0,

    /// <summary>
    ///  The form or control appears blacked out in screenshots.
    /// </summary>
    HideContent = 1,

    /// <summary>
    ///  The form or control appears blurred in screenshots.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Using this option requires Windows 10 version 2004 or higher.
    ///  </para>
    /// </remarks>
    HideWindow = 2
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Enumeration defining the behavior when a form is attempted to be captured in a screen capture (clipboard, image file),
///  in a screen sharing video conference scenario, a video recording or the alike.
/// </summary>
public enum ScreenCaptureMode
{
    /// <summary>
    ///  The form can be captured normally in screen captures.
    ///  Default setting.
    /// </summary>
    Allow = 0,

    /// <summary>
    ///  Only the form's content is hidden in screenshots.
    ///  The form's borders and title bar remain visible.
    /// </summary>
    HideContent = 1,

    /// <summary>
    ///  The whole form is invisible in screenshots.
    /// </summary>
    HideWindow = 2
}

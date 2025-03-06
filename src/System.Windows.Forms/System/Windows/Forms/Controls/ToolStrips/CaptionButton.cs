// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Specifies the type of caption button to display.
/// </summary>
public enum CaptionButton
{
    /// <summary>
    ///  A Close button.
    /// </summary>
    Close = (int)DFCS_STATE.DFCS_CAPTIONCLOSE,

    /// <summary>
    ///  A Help button.
    /// </summary>
    Help = (int)DFCS_STATE.DFCS_CAPTIONHELP,

    /// <summary>
    ///  A Maximize button.
    /// </summary>
    Maximize = (int)DFCS_STATE.DFCS_CAPTIONMAX,

    /// <summary>
    ///  A Minimize button.
    /// </summary>
    Minimize = (int)DFCS_STATE.DFCS_CAPTIONMIN,

    /// <summary>
    ///  A Restore button.
    /// </summary>
    Restore = (int)DFCS_STATE.DFCS_CAPTIONRESTORE,
}

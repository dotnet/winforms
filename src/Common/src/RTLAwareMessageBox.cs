// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  The Show method displays a message box that can contain text, buttons, and symbols that inform and instruct the user.
///  This MessageBox will be RTL, if the resources for this dll have been localized to a RTL language.
/// </summary>
internal sealed class RTLAwareMessageBox
{
    /// <summary>
    ///  Displays a message box with specified text, caption, and style.
    ///  Makes the dialog RTL if the resources for this dll have been localized to a RTL language.
    /// </summary>
    public static DialogResult Show(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options)
    {
        if (IsRTLResources)
        {
            options |= (MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }

        return MessageBox.Show(owner, text, caption, buttons, icon, defaultButton, options);
    }

    /// <summary>
    ///  Tells whether the current resources for this dll have been localized for a RTL language.
    /// </summary>
    public static bool IsRTLResources
    {
        get
        {
            return SR.RTL != "RTL_False";
        }
    }
}

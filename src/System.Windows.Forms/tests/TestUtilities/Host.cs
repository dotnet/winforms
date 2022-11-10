// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;
using Windows.Win32;
using Windows.Win32.Foundation;
using static Interop.User32;

namespace System;

/// <summary>
///  Helper class to host dialogs for testing.
///  This class is typically passed as a parameter to a call to <see cref="CommonDialog.ShowDialog(IWin32Window?)"/>.
/// </summary>
internal class Host : Form
{
    private WM _messageToPost;

    /// <summary>
    ///  Constructs a form that posts <paramref name="messageToPost"/> to the dialog
    ///  it hosts once the form has been received <see cref="WM.ENTERIDLE"/>.
    /// </summary>
    /// <param name="messageToPost">
    ///  The message to be posted to the dialog the form hosts once the form has received <see cref="WM.ENTERIDLE"/>.
    /// </param>
    public Host(WM messageToPost)
    {
        _messageToPost = messageToPost;
    }

    protected override void WndProc(ref Message m)
    {
        if (m.MsgInternal == WM.ENTERIDLE && m.WParamInternal == (uint)MSGF.DIALOGBOX)
        {
            HWND dialogHandle = (HWND)m.LParamInternal;
            PInvoke.PostMessage(dialogHandle, _messageToPost);
        }

        base.WndProc(ref m);
    }
}

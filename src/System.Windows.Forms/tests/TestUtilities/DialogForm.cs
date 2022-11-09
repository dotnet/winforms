// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;
using Windows.Win32;
using Windows.Win32.Foundation;
using static Interop;

namespace System;

/// <summary>
///  Helper class to host dialogs for testing.
///  This is class typically passed as a parameter to a call to <see cref="CommonDialog.ShowDialog(IWin32Window?)"/>.
/// </summary>
internal class DialogForm : Form
{
    private User32.WM _message;
    private User32.WM _action;

    /// <summary>
    ///  Constructs a DialogForm that performs <paramref name="action"/> to
    ///  the dialog it is hosting once the <paramref name="message"/> has been received.
    /// </summary>
    /// <param name="message">
    ///  The message the form should be listening for.
    /// </param>
    /// <param name="action">
    ///  The action to be done to the dialog once the form has received <paramref name="message"/>.
    /// </param>
    public DialogForm(User32.WM message, User32.WM action)
    {
        _message = message;
        _action = action;
    }

    protected override void WndProc(ref Message m)
    {
        if (m.MsgInternal == _message && m.WParamInternal == (uint)MSGF.DIALOGBOX)
        {
            HWND dialogHandle = (HWND)m.LParamInternal;
            PInvoke.PostMessage(dialogHandle, _action);
        }

        base.WndProc(ref m);
    }
}

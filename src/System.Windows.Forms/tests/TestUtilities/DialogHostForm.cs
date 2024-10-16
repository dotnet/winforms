// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace System;

/// <summary>
///  Helper class to host dialogs for testing.
///  This class is typically passed as a parameter to a call to <see cref="CommonDialog.ShowDialog(IWin32Window?)"/>.
/// </summary>
internal class DialogHostForm : Form
{
    protected override void WndProc(ref Message m)
    {
        if (m.MsgInternal == PInvokeCore.WM_ENTERIDLE && m.WParamInternal == (uint)MSGF.DIALOGBOX)
        {
            OnDialogIdle((HWND)m.LParamInternal);
        }

        base.WndProc(ref m);
    }

    protected virtual void OnDialogIdle(HWND dialogHandle)
    {
        PInvokeCore.PostMessage(dialogHandle, PInvokeCore.WM_CLOSE);
    }

    protected static unsafe void Accept(HWND handle)
    {
        PInvokeCore.SendMessage(handle, PInvokeCore.WM_COMMAND, (WPARAM)(nint)MESSAGEBOX_RESULT.IDOK);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class TaskDialog
    {
        private class WindowSubclassHandler : Forms.WindowSubclassHandler
        {
            private readonly TaskDialog _taskDialog;

            public WindowSubclassHandler(TaskDialog taskDialog)
                : base(taskDialog?.Handle ?? throw new ArgumentNullException(nameof(taskDialog)))
            {
                _taskDialog = taskDialog;
            }

            protected override void WndProc(ref Message m)
            {
                switch ((User32.WM)m.Msg)
                {
                    case ContinueButtonClickHandlingMessage:
                        // We received the message which we posted earlier when
                        // handling a TDN_BUTTON_CLICKED notification, so we should
                        // no longer ignore such notifications.
                        // We do not forward the message to the base class.
                        _taskDialog._ignoreButtonClickedNotifications = false;

                        break;

                    default:
                        base.WndProc(ref m);
                        break;
                }
            }

            protected override bool CanCatchWndProcException(Exception ex) => _taskDialog.CanCatchCallbackException();

            protected override void HandleWndProcException(Exception ex) => _taskDialog.HandleCallbackException(ex);
        }
    }
}

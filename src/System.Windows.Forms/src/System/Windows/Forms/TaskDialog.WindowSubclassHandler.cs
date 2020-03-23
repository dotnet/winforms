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
            private bool _processedShowWindowMessage;

            public WindowSubclassHandler(TaskDialog taskDialog)
                : base(taskDialog?.Handle ?? throw new ArgumentNullException(nameof(taskDialog)))
            {
                _taskDialog = taskDialog;
            }

            protected override unsafe void WndProc(ref Message m)
            {
                switch ((User32.WM)m.Msg)
                {
                    case User32.WM.WINDOWPOSCHANGED:
                        base.WndProc(ref m);

                        ref User32.WINDOWPOS windowPos = ref *(User32.WINDOWPOS*)m.LParam;

                        if ((windowPos.flags & User32.SWP.SHOWWINDOW) == User32.SWP.SHOWWINDOW &&
                            !_processedShowWindowMessage)
                        {
                            _processedShowWindowMessage = true;
                        }

                        break;

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

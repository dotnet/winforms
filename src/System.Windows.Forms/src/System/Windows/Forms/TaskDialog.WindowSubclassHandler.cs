// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class TaskDialog
    {
        private class WindowSubclassHandler : Forms.WindowSubclassHandler
        {
            private readonly TaskDialog _taskDialog;

            private bool _processedShowWindowMessage;

            public WindowSubclassHandler(TaskDialog taskDialog)
                : base(taskDialog?._hwndDialog ?? throw new ArgumentNullException(nameof(taskDialog)))
            {
                _taskDialog = taskDialog;
            }

            protected override unsafe void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case Interop.WindowMessages.WM_WINDOWPOSCHANGED:
                        base.WndProc(ref m);

                        ref NativeMethods.WINDOWPOS windowPos = ref *(NativeMethods.WINDOWPOS*)m.LParam;

                        if ((windowPos.flags & NativeMethods.SWP_SHOWWINDOW) == NativeMethods.SWP_SHOWWINDOW &&
                            !_processedShowWindowMessage)
                        {
                            _processedShowWindowMessage = true;

                            // The task dialog window has been shown for the first time.
                            _taskDialog.OnShown(EventArgs.Empty);
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

            protected override bool CanCatchWndProcException(Exception ex)
            {
                return _taskDialog.CanCatchCallbackException();
            }

            protected override void HandleWndProcException(Exception ex)
            {
                _taskDialog.HandleCallbackException(ex);
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class PrintControllerWithStatusDialog
{
    private class BackgroundThread
    {
        internal bool _canceled;
        private readonly PrintControllerWithStatusDialog _parent;
        private readonly Thread _thread;
        private bool _alreadyStopped;
        private StatusDialog? _dialog;
        private readonly Lock _lock = new();

        // Called from any thread
        internal BackgroundThread(PrintControllerWithStatusDialog parent)
        {
            _parent = parent;

            // Calling Application.DoEvents() from within a paint event causes all sorts of problems,
            // so we need to put the dialog on its own thread.
            _thread = new Thread(new ThreadStart(Run));
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();
        }

        // Called from any thread
        internal void Stop()
        {
            lock (_lock)
            {
                if (_dialog is { } dialog && dialog.IsHandleCreated)
                {
                    dialog.BeginInvoke(new MethodInvoker(dialog.Close));
                    return;
                }

                _alreadyStopped = true;
            }
        }

        // Called from any thread
        internal void UpdateLabel()
        {
            if (_dialog is { } dialog && dialog.IsHandleCreated)
            {
                dialog.BeginInvoke(new MethodInvoker(ThreadUnsafeUpdateLabel));
                // Don't wait for a response
            }
        }

        // on correct thread
        private void Run()
        {
            try
            {
                lock (_lock)
                {
                    if (_alreadyStopped)
                    {
                        return;
                    }

                    _dialog = new StatusDialog(this, _parent._dialogTitle);
                    ThreadUnsafeUpdateLabel();
                    _dialog.Visible = true;
                }

                if (!_alreadyStopped)
                {
                    Application.Run(_dialog);
                }
            }
            finally
            {
                lock (_lock)
                {
                    _dialog?.Dispose();
                    _dialog = null;
                }
            }
        }

        // on correct thread
        private void ThreadUnsafeUpdateLabel()
        {
            // "page {0} of {1}"
            _dialog!._messageLabel.Text = string.Format(
                SR.PrintControllerWithStatusDialog_NowPrinting,
                _parent._pageNumber,
                _parent._document?.DocumentName);
        }
    }
}

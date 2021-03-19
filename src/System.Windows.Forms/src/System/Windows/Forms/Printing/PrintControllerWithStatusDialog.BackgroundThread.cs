// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing.Printing;
using System.Threading;

namespace System.Windows.Forms
{
    public partial class PrintControllerWithStatusDialog
    {
        private class BackgroundThread
        {
            private readonly PrintControllerWithStatusDialog parent;
            private StatusDialog dialog;
            private readonly Thread thread;
            internal bool canceled;
            private bool alreadyStopped;

            // Called from any thread
            internal BackgroundThread(PrintControllerWithStatusDialog parent)
            {
                this.parent = parent;

                // Calling Application.DoEvents() from within a paint event causes all sorts of problems,
                // so we need to put the dialog on its own thread.
                thread = new Thread(new ThreadStart(Run));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }

            // on correct thread
            private void Run()
            {
                //

                try
                {
                    lock (this)
                    {
                        if (alreadyStopped)
                        {
                            return;
                        }

                        dialog = new StatusDialog(this, parent.dialogTitle);
                        ThreadUnsafeUpdateLabel();
                        dialog.Visible = true;
                    }

                    if (!alreadyStopped)
                    {
                        Application.Run(dialog);
                    }
                }
                finally
                {
                    lock (this)
                    {
                        if (dialog != null)
                        {
                            dialog.Dispose();
                            dialog = null;
                        }
                    }
                }
            }

            // Called from any thread
            internal void Stop()
            {
                lock (this)
                {
                    if (dialog != null && dialog.IsHandleCreated)
                    {
                        dialog.BeginInvoke(new MethodInvoker(dialog.Close));
                        return;
                    }

                    alreadyStopped = true;
                }
            }

            // on correct thread
            private void ThreadUnsafeUpdateLabel()
            {
                // "page {0} of {1}"
                dialog.label1.Text = string.Format(SR.PrintControllerWithStatusDialog_NowPrinting,
                                                   parent.pageNumber, parent.document.DocumentName);
            }

            // Called from any thread
            internal void UpdateLabel()
            {
                if (dialog != null && dialog.IsHandleCreated)
                {
                    dialog.BeginInvoke(new MethodInvoker(ThreadUnsafeUpdateLabel));
                    // Don't wait for a response
                }
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Drawing.Printing;
using System.Threading;

namespace System.Windows.Forms
{
    public class PrintControllerWithStatusDialog : PrintController
    {
        private readonly PrintController underlyingController;
        private PrintDocument document;
        private BackgroundThread backgroundThread;
        private int pageNumber;
        private readonly string dialogTitle;

        public PrintControllerWithStatusDialog(PrintController underlyingController)
        : this(underlyingController, SR.PrintControllerWithStatusDialog_DialogTitlePrint)
        {
        }

        public PrintControllerWithStatusDialog(PrintController underlyingController, string dialogTitle)
        {
            this.underlyingController = underlyingController;
            this.dialogTitle = dialogTitle;
        }
        /// <summary>
        ///  This is new public property which notifies if this controller is used for PrintPreview.. so get the underlying Controller
        ///  and return its IsPreview Property.
        /// </summary>
        public override bool IsPreview
        {
            get
            {
                if (underlyingController != null)
                {
                    return underlyingController.IsPreview;
                }
                return false;
            }
        }

        /// <summary>
        ///  Implements StartPrint by delegating to the underlying controller.
        /// </summary>
        public override void OnStartPrint(PrintDocument document, PrintEventArgs e)
        {
            base.OnStartPrint(document, e);

            this.document = document;
            pageNumber = 1;

            if (SystemInformation.UserInteractive)
            {
                backgroundThread = new BackgroundThread(this); // starts running & shows dialog automatically
            }

            // OnStartPrint does the security check... lots of
            // extra setup to make sure that we tear down
            // correctly...
            //
            try
            {
                underlyingController.OnStartPrint(document, e);
            }
            catch
            {
                if (backgroundThread != null)
                {
                    backgroundThread.Stop();
                }
                throw;
            }
            finally
            {
                if (backgroundThread != null && backgroundThread.canceled)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        ///  Implements StartPage by delegating to the underlying controller.
        /// </summary>
        public override Graphics OnStartPage(PrintDocument document, PrintPageEventArgs e)
        {
            base.OnStartPage(document, e);

            if (backgroundThread != null)
            {
                backgroundThread.UpdateLabel();
            }
            Graphics result = underlyingController.OnStartPage(document, e);
            if (backgroundThread != null && backgroundThread.canceled)
            {
                e.Cancel = true;
            }
            return result;
        }

        /// <summary>
        ///  Implements EndPage by delegating to the underlying controller.
        /// </summary>
        public override void OnEndPage(PrintDocument document, PrintPageEventArgs e)
        {
            underlyingController.OnEndPage(document, e);
            if (backgroundThread != null && backgroundThread.canceled)
            {
                e.Cancel = true;
            }
            pageNumber++;

            base.OnEndPage(document, e);
        }

        /// <summary>
        ///  Implements EndPrint by delegating to the underlying controller.
        /// </summary>
        public override void OnEndPrint(PrintDocument document, PrintEventArgs e)
        {
            underlyingController.OnEndPrint(document, e);
            if (backgroundThread != null && backgroundThread.canceled)
            {
                e.Cancel = true;
            }

            if (backgroundThread != null)
            {
                backgroundThread.Stop();
            }

            base.OnEndPrint(document, e);
        }

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

        private class StatusDialog : Form
        {
            internal Label label1;
            private Button button1;
            private TableLayoutPanel tableLayoutPanel1;
            private readonly BackgroundThread backgroundThread;

            internal StatusDialog(BackgroundThread backgroundThread, string dialogTitle)
            {
                InitializeComponent();
                this.backgroundThread = backgroundThread;
                Text = dialogTitle;
                MinimumSize = Size;
            }

            /// <summary>
            ///  Tells whether the current resources for this dll have been
            ///  localized for a RTL language.
            /// </summary>
            private static bool IsRTLResources
            {
                get
                {
                    return SR.RTL != "RTL_False";
                }
            }

            private void InitializeComponent()
            {
                if (IsRTLResources)
                {
                    RightToLeft = RightToLeft.Yes;
                }

                tableLayoutPanel1 = new TableLayoutPanel();
                label1 = new Label();
                button1 = new Button();

                label1.AutoSize = true;
                label1.Location = new Point(8, 16);
                label1.TextAlign = ContentAlignment.MiddleCenter;
                label1.Size = new Size(240, 64);
                label1.TabIndex = 1;
                label1.Anchor = AnchorStyles.None;

                button1.AutoSize = true;
                button1.Size = new Size(75, 23);
                button1.TabIndex = 0;
                button1.Text = SR.PrintControllerWithStatusDialog_Cancel;
                button1.Location = new Point(88, 88);
                button1.Anchor = AnchorStyles.None;
                button1.Click += new EventHandler(button1_Click);

                tableLayoutPanel1.AutoSize = true;
                tableLayoutPanel1.ColumnCount = 1;
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
                tableLayoutPanel1.Location = new Point(0, 0);
                tableLayoutPanel1.RowCount = 2;
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                tableLayoutPanel1.TabIndex = 0;
                tableLayoutPanel1.Controls.Add(label1, 0, 0);
                tableLayoutPanel1.Controls.Add(button1, 0, 1);

                AutoScaleDimensions = new Size(6, 13);
                AutoScaleMode = AutoScaleMode.Font;
                MaximizeBox = false;
                ControlBox = false;
                MinimizeBox = false;
                Size clientSize = new Size(256, 122);
                if (DpiHelper.IsScalingRequired)
                {
                    ClientSize = DpiHelper.LogicalToDeviceUnits(clientSize);
                }
                else
                {
                    ClientSize = clientSize;
                }
                CancelButton = button1;
                SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
                Controls.Add(tableLayoutPanel1);
            }
            private void button1_Click(object sender, EventArgs e)
            {
                button1.Enabled = false;
                label1.Text = SR.PrintControllerWithStatusDialog_Canceling;
                backgroundThread.canceled = true;
            }
        }
    }
}

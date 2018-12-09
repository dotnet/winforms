// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Diagnostics;
    using System;
    using System.Threading;
    using System.Drawing;
    using Microsoft.Win32;
    using System.ComponentModel;
    using System.Drawing.Printing;
    using System.Security;
    using System.Security.Permissions;

    /// <include file='doc\PrintControllerWithStatusDialog.uex' path='docs/doc[@for="PrintControllerWithStatusDialog"]/*' />
    public class PrintControllerWithStatusDialog : PrintController {
        private PrintController underlyingController;
        private PrintDocument document;
        private BackgroundThread backgroundThread;
        private int pageNumber;
        private string dialogTitle;

        /// <include file='doc\PrintControllerWithStatusDialog.uex' path='docs/doc[@for="PrintControllerWithStatusDialog.PrintControllerWithStatusDialog"]/*' />
        public PrintControllerWithStatusDialog(PrintController underlyingController) 
        : this(underlyingController, string.Format(SR.PrintControllerWithStatusDialog_DialogTitlePrint)) {
        }

        /// <include file='doc\PrintControllerWithStatusDialog.uex' path='docs/doc[@for="PrintControllerWithStatusDialog.PrintControllerWithStatusDialog1"]/*' />
        public PrintControllerWithStatusDialog(PrintController underlyingController, string dialogTitle) {
            this.underlyingController = underlyingController;
            this.dialogTitle = dialogTitle;
        }
        /// <include file='doc\PreviewPrintController.uex' path='docs/doc[@for="PreviewPrintController.IsPreview"]/*' />
        /// <devdoc>
        ///    <para>
        ///       This is new public property which notifies if this controller is used for PrintPreview.. so get the underlying Controller 
        ///       and return its IsPreview Property.
        ///    </para>
        /// </devdoc>
        public override bool IsPreview {
            get {
                if (underlyingController != null)
                {
                    return underlyingController.IsPreview;
                }
                return false;
            }
        }

        /// <include file='doc\PrintControllerWithStatusDialog.uex' path='docs/doc[@for="PrintControllerWithStatusDialog.OnStartPrint"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Implements StartPrint by delegating to the underlying controller.
        ///    </para>
        /// </devdoc>
        public override void OnStartPrint(PrintDocument document, PrintEventArgs e) {
            base.OnStartPrint(document, e);

            this.document = document;
            pageNumber = 1;

            if (SystemInformation.UserInteractive) {
                backgroundThread = new BackgroundThread(this); // starts running & shows dialog automatically
            }

            // OnStartPrint does the security check... lots of 
            // extra setup to make sure that we tear down
            // correctly...
            //
            try {
                underlyingController.OnStartPrint(document, e);
            }
            catch {
                if (backgroundThread != null) {
                    backgroundThread.Stop();
                }
                throw;
            }
            finally {
                if (backgroundThread != null && backgroundThread.canceled) {
                    e.Cancel = true;
                }
            }
        }

        /// <include file='doc\PrintControllerWithStatusDialog.uex' path='docs/doc[@for="PrintControllerWithStatusDialog.OnStartPage"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Implements StartPage by delegating to the underlying controller.
        ///    </para>
        /// </devdoc>
        public override Graphics OnStartPage(PrintDocument document, PrintPageEventArgs e) {
            base.OnStartPage(document, e);

            if (backgroundThread != null) {
                backgroundThread.UpdateLabel();
            }
            Graphics result = underlyingController.OnStartPage(document, e);
            if (backgroundThread != null && backgroundThread.canceled){
                e.Cancel = true;
            }
            return result;
        }

        /// <include file='doc\PrintControllerWithStatusDialog.uex' path='docs/doc[@for="PrintControllerWithStatusDialog.OnEndPage"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Implements EndPage by delegating to the underlying controller.
        ///    </para>
        /// </devdoc>
        public override void OnEndPage(PrintDocument document, PrintPageEventArgs e) {
            underlyingController.OnEndPage(document, e);
            if (backgroundThread != null && backgroundThread.canceled) {
                e.Cancel = true;
            }
            pageNumber++;

            base.OnEndPage(document, e);
        }

        /// <include file='doc\PrintControllerWithStatusDialog.uex' path='docs/doc[@for="PrintControllerWithStatusDialog.OnEndPrint"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Implements EndPrint by delegating to the underlying controller.
        ///    </para>
        /// </devdoc>
        public override void OnEndPrint(PrintDocument document, PrintEventArgs e) {
            underlyingController.OnEndPrint(document, e);
            if (backgroundThread != null && backgroundThread.canceled) {
                e.Cancel = true;
            }

            if (backgroundThread != null) {
                backgroundThread.Stop();
            }

            base.OnEndPrint(document, e);
        }

        private class BackgroundThread {
            private PrintControllerWithStatusDialog parent;
            private StatusDialog dialog;
            private Thread thread;
            internal bool canceled = false;
            private bool alreadyStopped = false;

            // Called from any thread
            internal BackgroundThread(PrintControllerWithStatusDialog parent) {
                this.parent = parent;

                // Calling Application.DoEvents() from within a paint event causes all sorts of problems,
                // so we need to put the dialog on its own thread.
                thread = new Thread(new ThreadStart(Run));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }

            // on correct thread
            [
                UIPermission(SecurityAction.Assert, Window=UIPermissionWindow.AllWindows),
                SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.UnmanagedCode),
            ] 
            private void Run() {
                // 

                try {
                    lock (this) {
                        if (alreadyStopped) {
                            return;
                        }

                        dialog = new StatusDialog(this, parent.dialogTitle);
                        ThreadUnsafeUpdateLabel();
                        dialog.Visible = true;
                    }

                    if (!alreadyStopped) {
                        Application.Run(dialog);
                    }
                }
                finally {
                    lock (this) {
                        if (dialog != null) {
                            dialog.Dispose();
                            dialog = null;
                        }
                    }
                }
            }

            // Called from any thread
            internal void Stop() {
                lock (this) {
                    if (dialog != null && dialog.IsHandleCreated) {
                        dialog.BeginInvoke(new MethodInvoker(dialog.Close));
                        return;
                    }
                    alreadyStopped = true;
                }
            }

            // on correct thread
            private void ThreadUnsafeUpdateLabel() {
                // "page {0} of {1}"
                dialog.label1.Text = string.Format(SR.PrintControllerWithStatusDialog_NowPrinting, 
                                                   parent.pageNumber, parent.document.DocumentName);
            }

            // Called from any thread
            internal void UpdateLabel() {
                if (dialog != null && dialog.IsHandleCreated) {
                    dialog.BeginInvoke(new MethodInvoker(ThreadUnsafeUpdateLabel));
                    // Don't wait for a response
                }
            }
        }

        private class StatusDialog : Form {
            internal Label label1;
            private Button button1;
            private TableLayoutPanel tableLayoutPanel1;
            private BackgroundThread backgroundThread;

            internal StatusDialog(BackgroundThread backgroundThread, string dialogTitle) {
                
                InitializeComponent();
                this.backgroundThread = backgroundThread;
                this.Text = dialogTitle;
                this.MinimumSize = Size;
            }

            /// <devdoc>
            ///     Tells whether the current resources for this dll have been
            ///     localized for a RTL language.
            /// </devdoc>
            private static bool IsRTLResources {
                get {
                    return SR.RTL != "RTL_False";
                }
            }

            private void InitializeComponent() {
                if (IsRTLResources)
                {
                    this.RightToLeft = RightToLeft.Yes;
                }

                this.tableLayoutPanel1 = new TableLayoutPanel();
                this.label1 = new Label();
                this.button1 = new Button();

                label1.AutoSize = true;
                label1.Location = new Point(8, 16);
                label1.TextAlign = ContentAlignment.MiddleCenter;
                label1.Size = new Size(240, 64);
                label1.TabIndex = 1;
                label1.Anchor = AnchorStyles.None;
                                
                button1.AutoSize = true;
                button1.Size = new Size(75, 23);
                button1.TabIndex = 0;
                button1.Text = string.Format(SR.PrintControllerWithStatusDialog_Cancel);
                button1.Location = new Point(88, 88);
                button1.Anchor = AnchorStyles.None;
                button1.Click += new EventHandler(button1_Click);

                tableLayoutPanel1.AutoSize = true;
                tableLayoutPanel1.ColumnCount = 1;
                tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(SizeType.Percent, 100F));
                tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
                tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
                tableLayoutPanel1.RowCount = 2;
                tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(SizeType.Percent, 50F));
                tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(SizeType.Percent, 50F));
                tableLayoutPanel1.TabIndex = 0;
                tableLayoutPanel1.Controls.Add(label1, 0, 0);
                tableLayoutPanel1.Controls.Add(button1, 0, 1);
                
                this.AutoScaleDimensions = new Size(6, 13);
                this.AutoScaleMode = AutoScaleMode.Font;
                this.MaximizeBox = false;
                this.ControlBox = false;
                this.MinimizeBox = false;
                Size clientSize = new Size(256, 122);
                if (DpiHelper.IsScalingRequired) {
                    this.ClientSize = DpiHelper.LogicalToDeviceUnits(clientSize);
                }
                else {
                    this.ClientSize = clientSize;
                }
                this.CancelButton = button1;
                this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;                
                this.Controls.Add(tableLayoutPanel1);
            }
            private void button1_Click(object sender, System.EventArgs e) {
                button1.Enabled = false;
                label1.Text = string.Format(SR.PrintControllerWithStatusDialog_Canceling);
                backgroundThread.canceled = true;
            }
        }
    }
}

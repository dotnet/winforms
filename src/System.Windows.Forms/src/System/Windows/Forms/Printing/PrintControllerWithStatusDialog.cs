// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Drawing.Printing;

namespace System.Windows.Forms
{
    public partial class PrintControllerWithStatusDialog : PrintController
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
    }
}

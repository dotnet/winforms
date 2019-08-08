// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Allows users to select a printer and choose which
    ///  portions of the document to print.
    /// </summary>
    [DefaultProperty(nameof(Document))]
    [SRDescription(nameof(SR.DescriptionPrintDialog))]
    [Designer("System.Windows.Forms.Design.PrintDialogDesigner, " + AssemblyRef.SystemDesign)]
    // The only event this dialog has is HelpRequested, which isn't very useful
    public sealed class PrintDialog : CommonDialog
    {
        private const int printRangeMask = (int)(PrintRange.AllPages | PrintRange.SomePages
                                                  | PrintRange.Selection | PrintRange.CurrentPage);

        // If PrintDocument != null, settings == printDocument.PrinterSettings
        private PrinterSettings settings = null;
        private PrintDocument printDocument = null;

        // Implementing "current page" would require switching to PrintDlgEx, which is windows 2000 and later only
        private bool allowCurrentPage;

        private bool allowPages;
        private bool allowPrintToFile;
        private bool allowSelection;
        private bool printToFile;
        private bool showHelp;
        private bool showNetwork;

        /// <summary>
        ///  Initializes a new instance of the <see cref='PrintDialog'/> class.
        /// </summary>
        public PrintDialog()
        {
            Reset();
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the Current Page option button is enabled.
        /// </summary>
        [
        DefaultValue(false),
        SRDescription(nameof(SR.PDallowCurrentPageDescr))
        ]
        public bool AllowCurrentPage
        {
            get { return allowCurrentPage; }
            set { allowCurrentPage = value; }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the Pages option button is enabled.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.PDallowPagesDescr))
        ]
        public bool AllowSomePages
        {
            get { return allowPages; }
            set { allowPages = value; }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the Print to file check box is enabled.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.PDallowPrintToFileDescr))
        ]
        public bool AllowPrintToFile
        {
            get { return allowPrintToFile; }
            set { allowPrintToFile = value; }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the From... To... Page option button is enabled.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.PDallowSelectionDescr))
        ]
        public bool AllowSelection
        {
            get { return allowSelection; }
            set { allowSelection = value; }
        }

        /// <summary>
        ///  Gets or sets a value indicating the <see cref='PrintDocument'/> used to obtain <see cref='Drawing.Printing.PrinterSettings'/>.
        /// </summary>
        [
        SRCategory(nameof(SR.CatData)),
        DefaultValue(null),
        SRDescription(nameof(SR.PDdocumentDescr))
        ]
        public PrintDocument Document
        {
            get { return printDocument; }
            set
            {
                printDocument = value;
                if (printDocument == null)
                {
                    settings = new PrinterSettings();
                }
                else
                {
                    settings = printDocument.PrinterSettings;
                }
            }
        }

        private PageSettings PageSettings
        {
            get
            {
                if (Document == null)
                {
                    return PrinterSettings.DefaultPageSettings;
                }
                else
                {
                    return Document.DefaultPageSettings;
                }
            }
        }

        /// <summary>
        ///  Gets or sets the <see cref='Drawing.Printing.PrinterSettings'/> the
        ///  dialog box will be modifying.
        /// </summary>
        [
        SRCategory(nameof(SR.CatData)),
        DefaultValue(null),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.PDprinterSettingsDescr))
        ]
        public PrinterSettings PrinterSettings
        {
            get
            {

                if (settings == null)
                {
                    settings = new PrinterSettings();
                }
                return settings;
            }
            set
            {
                if (value != PrinterSettings)
                {
                    settings = value;
                    printDocument = null;
                }
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the Print to file check box is checked.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.PDprintToFileDescr))
        ]
        public bool PrintToFile
        {
            get { return printToFile; }
            set { printToFile = value; }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the Help button is displayed.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.PDshowHelpDescr))
        ]
        public bool ShowHelp
        {
            get { return showHelp; }
            set { showHelp = value; }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the Network button is displayed.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.PDshowNetworkDescr))
        ]
        public bool ShowNetwork
        {
            get { return showNetwork; }
            set { showNetwork = value; }
        }

        /// <summary>
        ///  UseEXDialog = true means to use the EX versions of the dialogs and to ignore the
        ///  ShowHelp & ShowNetwork properties.
        ///  UseEXDialog = false means to never use the EX versions of the dialog.
        ///  ShowHelp & ShowNetwork will work in this case.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription(nameof(SR.PDuseEXDialog))]
        public bool UseEXDialog { get; set; }

        private int GetFlags()
        {
            int flags = 0;

            // Only set this flag when using PRINTDLG and PrintDlg,
            // and not when using PrintDlgEx and PRINTDLGEX.
            if (!UseEXDialog)
            {
                flags |= NativeMethods.PD_ENABLEPRINTHOOK;
            }

            if (!allowCurrentPage)
            {
                flags |= NativeMethods.PD_NOCURRENTPAGE;
            }
            if (!allowPages)
            {
                flags |= NativeMethods.PD_NOPAGENUMS;
            }
            if (!allowPrintToFile)
            {
                flags |= NativeMethods.PD_DISABLEPRINTTOFILE;
            }
            if (!allowSelection)
            {
                flags |= NativeMethods.PD_NOSELECTION;
            }

            flags |= (int)PrinterSettings.PrintRange;

            if (printToFile)
            {
                flags |= NativeMethods.PD_PRINTTOFILE;
            }
            if (showHelp)
            {
                flags |= NativeMethods.PD_SHOWHELP;
            }
            if (!showNetwork)
            {
                flags |= NativeMethods.PD_NONETWORKBUTTON;
            }
            if (PrinterSettings.Collate)
            {
                flags |= NativeMethods.PD_COLLATE;
            }

            return flags;
        }

        /// <summary>
        ///  Resets all options, the last selected printer, and the page
        ///  settings to their default values.
        /// </summary>
        public override void Reset()
        {
            allowCurrentPage = false;
            allowPages = false;
            allowPrintToFile = true;
            allowSelection = false;
            printDocument = null;
            printToFile = false;
            settings = null;
            showHelp = false;
            showNetwork = true;
        }

        // Create a PRINTDLG with a few useful defaults.
        internal static NativeMethods.PRINTDLG CreatePRINTDLG()
        {
            NativeMethods.PRINTDLG data = null;
            if (IntPtr.Size == 4)
            {
                data = new NativeMethods.PRINTDLG_32();
            }
            else
            {
                data = new NativeMethods.PRINTDLG_64();
            }
            data.lStructSize = Marshal.SizeOf(data);
            data.hwndOwner = IntPtr.Zero;
            data.hDevMode = IntPtr.Zero;
            data.hDevNames = IntPtr.Zero;
            data.Flags = 0;
            data.hDC = IntPtr.Zero;
            data.nFromPage = 1;
            data.nToPage = 1;
            data.nMinPage = 0;
            data.nMaxPage = 9999;
            data.nCopies = 1;
            data.hInstance = IntPtr.Zero;
            data.lCustData = IntPtr.Zero;
            data.lpfnPrintHook = null;
            data.lpfnSetupHook = null;
            data.lpPrintTemplateName = null;
            data.lpSetupTemplateName = null;
            data.hPrintTemplate = IntPtr.Zero;
            data.hSetupTemplate = IntPtr.Zero;
            return data;
        }

        internal static NativeMethods.PRINTDLGEX CreatePRINTDLGEX()
        {
            NativeMethods.PRINTDLGEX data = new NativeMethods.PRINTDLGEX();
            data.lStructSize = Marshal.SizeOf(data);
            data.hwndOwner = IntPtr.Zero;
            data.hDevMode = IntPtr.Zero;
            data.hDevNames = IntPtr.Zero;
            data.hDC = IntPtr.Zero;
            data.Flags = 0;
            data.Flags2 = 0;
            data.ExclusionFlags = 0;
            data.nPageRanges = 0;
            data.nMaxPageRanges = 1;
            data.pageRanges = UnsafeNativeMethods.GlobalAlloc(NativeMethods.GPTR,
                                                              data.nMaxPageRanges * Marshal.SizeOf<NativeMethods.PRINTPAGERANGE>());
            data.nMinPage = 0;
            data.nMaxPage = 9999;
            data.nCopies = 1;
            data.hInstance = IntPtr.Zero;
            data.lpPrintTemplateName = null;
            data.nPropertyPages = 0;
            data.lphPropertyPages = IntPtr.Zero;
            data.nStartPage = NativeMethods.START_PAGE_GENERAL;
            data.dwResultAction = 0;
            return data;
        }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            var hookProcPtr = new NativeMethods.WndProc(HookProc);

            if (!UseEXDialog)
            {
                NativeMethods.PRINTDLG data = CreatePRINTDLG();
                return ShowPrintDialog(hwndOwner, hookProcPtr, data);
            }
            else
            {
                NativeMethods.PRINTDLGEX data = CreatePRINTDLGEX();
                return ShowPrintDialog(hwndOwner, data);
            }
        }

        private bool ShowPrintDialog(IntPtr hwndOwner, NativeMethods.WndProc hookProcPtr, NativeMethods.PRINTDLG data)
        {
            data.Flags = GetFlags();
            data.nCopies = (short)PrinterSettings.Copies;
            data.hwndOwner = hwndOwner;
            data.lpfnPrintHook = hookProcPtr;

            try
            {
                if (PageSettings == null)
                {
                    data.hDevMode = PrinterSettings.GetHdevmode();
                }
                else
                {
                    data.hDevMode = PrinterSettings.GetHdevmode(PageSettings);
                }

                data.hDevNames = PrinterSettings.GetHdevnames();
            }
            catch (InvalidPrinterException)
            {
                data.hDevMode = IntPtr.Zero;
                data.hDevNames = IntPtr.Zero;
                // Leave those fields null; Windows will fill them in
            }

            try
            {
                // Windows doesn't like it if page numbers are invalid
                if (AllowSomePages)
                {
                    if (PrinterSettings.FromPage < PrinterSettings.MinimumPage
                        || PrinterSettings.FromPage > PrinterSettings.MaximumPage)
                    {
                        throw new ArgumentException(string.Format(SR.PDpageOutOfRange, "FromPage"));
                    }

                    if (PrinterSettings.ToPage < PrinterSettings.MinimumPage
                        || PrinterSettings.ToPage > PrinterSettings.MaximumPage)
                    {
                        throw new ArgumentException(string.Format(SR.PDpageOutOfRange, "ToPage"));
                    }

                    if (PrinterSettings.ToPage < PrinterSettings.FromPage)
                    {
                        throw new ArgumentException(string.Format(SR.PDpageOutOfRange, "FromPage"));
                    }

                    data.nFromPage = (short)PrinterSettings.FromPage;
                    data.nToPage = (short)PrinterSettings.ToPage;
                    data.nMinPage = (short)PrinterSettings.MinimumPage;
                    data.nMaxPage = (short)PrinterSettings.MaximumPage;
                }

                if (!UnsafeNativeMethods.PrintDlg(data))
                {
                    return false;
                }

                UpdatePrinterSettings(data.hDevMode, data.hDevNames, data.nCopies, data.Flags, settings, PageSettings);

                PrintToFile = ((data.Flags & NativeMethods.PD_PRINTTOFILE) != 0);
                PrinterSettings.PrintToFile = PrintToFile;

                if (AllowSomePages)
                {
                    PrinterSettings.FromPage = data.nFromPage;
                    PrinterSettings.ToPage = data.nToPage;
                }

                // When the flag PD_USEDEVMODECOPIESANDCOLLATE is not set,
                // PRINTDLG.nCopies or PRINTDLG.nCopies indicates the number of copies the user wants
                // to print, and the PD_COLLATE flag in the Flags member indicates
                // whether the user wants to print them collated.
                if ((data.Flags & NativeMethods.PD_USEDEVMODECOPIESANDCOLLATE) == 0)
                {
                    PrinterSettings.Copies = data.nCopies;
                    PrinterSettings.Collate = ((data.Flags & NativeMethods.PD_COLLATE) == NativeMethods.PD_COLLATE);
                }

                return true;
            }
            finally
            {
                UnsafeNativeMethods.GlobalFree(new HandleRef(data, data.hDevMode));
                UnsafeNativeMethods.GlobalFree(new HandleRef(data, data.hDevNames));
            }
        }

        // Due to the nature of PRINTDLGEX vs PRINTDLG, separate but similar methods
        // are required for showing the print dialog on Win2k and newer OS'.
        private bool ShowPrintDialog(IntPtr hwndOwner, NativeMethods.PRINTDLGEX data)
        {
            data.Flags = GetFlags();
            data.nCopies = PrinterSettings.Copies;
            data.hwndOwner = hwndOwner;

            try
            {
                if (PageSettings == null)
                {
                    data.hDevMode = PrinterSettings.GetHdevmode();
                }
                else
                {
                    data.hDevMode = PrinterSettings.GetHdevmode(PageSettings);
                }

                data.hDevNames = PrinterSettings.GetHdevnames();
            }
            catch (InvalidPrinterException)
            {
                data.hDevMode = IntPtr.Zero;
                data.hDevNames = IntPtr.Zero;
                // Leave those fields null; Windows will fill them in
            }

            try
            {
                // Windows doesn't like it if page numbers are invalid
                if (AllowSomePages)
                {
                    if (PrinterSettings.FromPage < PrinterSettings.MinimumPage
                        || PrinterSettings.FromPage > PrinterSettings.MaximumPage)
                    {
                        throw new ArgumentException(string.Format(SR.PDpageOutOfRange, "FromPage"));
                    }

                    if (PrinterSettings.ToPage < PrinterSettings.MinimumPage
                        || PrinterSettings.ToPage > PrinterSettings.MaximumPage)
                    {
                        throw new ArgumentException(string.Format(SR.PDpageOutOfRange, "ToPage"));
                    }

                    if (PrinterSettings.ToPage < PrinterSettings.FromPage)
                    {
                        throw new ArgumentException(string.Format(SR.PDpageOutOfRange, "FromPage"));
                    }

                    unsafe
                    {
                        int* pageRangeField = (int*)data.pageRanges;
                        *pageRangeField = PrinterSettings.FromPage;
                        pageRangeField += 1;
                        *pageRangeField = PrinterSettings.ToPage;
                    }
                    data.nPageRanges = 1;

                    data.nMinPage = PrinterSettings.MinimumPage;
                    data.nMaxPage = PrinterSettings.MaximumPage;
                }

                //
                // The flags NativeMethods.PD_SHOWHELP and NativeMethods.PD_NONETWORKBUTTON don't work with
                // PrintDlgEx. So we have to strip them out.
                data.Flags &= ~(NativeMethods.PD_SHOWHELP | NativeMethods.PD_NONETWORKBUTTON);

                int hr = UnsafeNativeMethods.PrintDlgEx(data);
                if (NativeMethods.Failed(hr) || data.dwResultAction == NativeMethods.PD_RESULT_CANCEL)
                {
                    return false;
                }

                UpdatePrinterSettings(data.hDevMode, data.hDevNames, (short)data.nCopies, data.Flags, PrinterSettings, PageSettings);

                PrintToFile = ((data.Flags & NativeMethods.PD_PRINTTOFILE) != 0);
                PrinterSettings.PrintToFile = PrintToFile;
                if (AllowSomePages)
                {
                    unsafe
                    {
                        int* pageRangeField = (int*)data.pageRanges;
                        PrinterSettings.FromPage = *pageRangeField;
                        pageRangeField += 1;
                        PrinterSettings.ToPage = *pageRangeField;
                    }
                }

                // When the flag PD_USEDEVMODECOPIESANDCOLLATE is not set,
                // PRINTDLG.nCopies or PRINTDLG.nCopies indicates the number of copies the user wants
                // to print, and the PD_COLLATE flag in the Flags member indicates
                // whether the user wants to print them collated.
                if ((data.Flags & NativeMethods.PD_USEDEVMODECOPIESANDCOLLATE) == 0)
                {
                    PrinterSettings.Copies = (short)(data.nCopies);
                    PrinterSettings.Collate = ((data.Flags & NativeMethods.PD_COLLATE) == NativeMethods.PD_COLLATE);
                }

                // We should return true only if the user pressed the "Print" button while dismissing the dialog.
                return (data.dwResultAction == NativeMethods.PD_RESULT_PRINT);
            }
            finally
            {
                if (data.hDevMode != IntPtr.Zero)
                {
                    UnsafeNativeMethods.GlobalFree(new HandleRef(data, data.hDevMode));
                }

                if (data.hDevNames != IntPtr.Zero)
                {
                    UnsafeNativeMethods.GlobalFree(new HandleRef(data, data.hDevNames));
                }

                if (data.pageRanges != IntPtr.Zero)
                {
                    UnsafeNativeMethods.GlobalFree(new HandleRef(data, data.pageRanges));
                }
            }
        }

        // Due to the nature of PRINTDLGEX vs PRINTDLG, separate but similar methods
        // are required for updating the settings from the structure utilized by the dialog.
        // Take information from print dialog and put in PrinterSettings
        private static void UpdatePrinterSettings(IntPtr hDevMode, IntPtr hDevNames, short copies, int flags, PrinterSettings settings, PageSettings pageSettings)
        {
            // Mode
            settings.SetHdevmode(hDevMode);
            settings.SetHdevnames(hDevNames);

            if (pageSettings != null)
            {
                pageSettings.SetHdevmode(hDevMode);
            }

            //Check for Copies == 1 since we might get the Right number of Copies from hdevMode.dmCopies...
            if (settings.Copies == 1)
            {
                settings.Copies = copies;
            }

            settings.PrintRange = (PrintRange)(flags & printRangeMask);
        }

    }
}


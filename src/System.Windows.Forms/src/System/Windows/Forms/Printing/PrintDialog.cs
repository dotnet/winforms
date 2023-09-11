// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using static Interop.Comdlg32;
using Windows.Win32.UI.Controls.Dialogs;
using static Windows.Win32.System.Memory.GLOBAL_ALLOC_FLAGS;

namespace System.Windows.Forms;

/// <summary>
///  Allows users to select a printer and choose which
///  portions of the document to print.
/// </summary>
[DefaultProperty(nameof(Document))]
[SRDescription(nameof(SR.DescriptionPrintDialog))]
[Designer($"System.Windows.Forms.Design.PrintDialogDesigner, {AssemblyRef.SystemDesign}")]
// The only event this dialog has is HelpRequested, which isn't very useful
public sealed class PrintDialog : CommonDialog
{
    private const PRINTDLGEX_FLAGS printRangeMask = PRINTDLGEX_FLAGS.PD_ALLPAGES | PRINTDLGEX_FLAGS.PD_PAGENUMS | PRINTDLGEX_FLAGS.PD_SELECTION | PRINTDLGEX_FLAGS.PD_CURRENTPAGE;

    // If PrintDocument is not null, settings == printDocument.PrinterSettings
    private PrinterSettings? _printerSettings;
    private PrintDocument? _printDocument;

    // Implementing "current page" would require switching to PrintDlgEx, which is windows 2000 and later only
    private bool _allowCurrentPage;

    private bool _allowPages;
    private bool _allowPrintToFile;
    private bool _allowSelection;
    private bool _printToFile;
    private bool _showHelp;
    private bool _showNetwork;

    /// <summary>
    ///  Initializes a new instance of the <see cref="PrintDialog"/> class.
    /// </summary>
    public PrintDialog()
    {
        Reset();
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the Current Page option button is enabled.
    /// </summary>
    [DefaultValue(false)]
    [SRDescription(nameof(SR.PDallowCurrentPageDescr))]
    public bool AllowCurrentPage
    {
        get { return _allowCurrentPage; }
        set { _allowCurrentPage = value; }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the Pages option button is enabled.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.PDallowPagesDescr))]
    public bool AllowSomePages
    {
        get { return _allowPages; }
        set { _allowPages = value; }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the Print to file check box is enabled.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.PDallowPrintToFileDescr))]
    public bool AllowPrintToFile
    {
        get { return _allowPrintToFile; }
        set { _allowPrintToFile = value; }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the From... To... Page option button is enabled.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.PDallowSelectionDescr))]
    public bool AllowSelection
    {
        get { return _allowSelection; }
        set { _allowSelection = value; }
    }

    /// <summary>
    ///  Gets or sets a value indicating the <see cref="PrintDocument"/> used to obtain <see cref="Drawing.Printing.PrinterSettings"/>.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DefaultValue(null)]
    [SRDescription(nameof(SR.PDdocumentDescr))]
    public PrintDocument? Document
    {
        get { return _printDocument; }
        set
        {
            _printDocument = value;
            if (_printDocument is null)
            {
                _printerSettings = new PrinterSettings();
            }
            else
            {
                _printerSettings = _printDocument.PrinterSettings;
            }
        }
    }

    private PageSettings PageSettings
    {
        get
        {
            if (Document is null)
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
    ///  Gets or sets the <see cref="Drawing.Printing.PrinterSettings"/> the
    ///  dialog box will be modifying.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DefaultValue(null)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.PDprinterSettingsDescr))]
    [AllowNull]
    public PrinterSettings PrinterSettings
    {
        get
        {
            _printerSettings ??= new PrinterSettings();

            return _printerSettings;
        }
        set
        {
            if (value != PrinterSettings)
            {
                _printerSettings = value;
                _printDocument = null;
            }
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the Print to file check box is checked.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.PDprintToFileDescr))]
    public bool PrintToFile
    {
        get { return _printToFile; }
        set { _printToFile = value; }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the Help button is displayed.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.PDshowHelpDescr))]
    public bool ShowHelp
    {
        get { return _showHelp; }
        set { _showHelp = value; }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the Network button is displayed.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.PDshowNetworkDescr))]
    public bool ShowNetwork
    {
        get { return _showNetwork; }
        set { _showNetwork = value; }
    }

    /// <summary>
    ///  UseEXDialog = true means to use the EX versions of the dialogs and to ignore the
    ///  ShowHelp &amp; ShowNetwork properties.
    ///  UseEXDialog = false means to never use the EX versions of the dialog.
    ///  ShowHelp &amp; ShowNetwork will work in this case.
    /// </summary>
    [DefaultValue(false)]
    [SRDescription(nameof(SR.PDuseEXDialog))]
    public bool UseEXDialog { get; set; }

    private PRINTDLGEX_FLAGS GetFlags()
    {
        PRINTDLGEX_FLAGS flags = PRINTDLGEX_FLAGS.PD_ALLPAGES;

        // Only set this flag when using PRINTDLG and PrintDlg,
        // and not when using PrintDlgEx and PRINTDLGEX.
        if (!UseEXDialog)
        {
            flags |= PRINTDLGEX_FLAGS.PD_ENABLEPRINTHOOK;
        }

        if (!_allowCurrentPage)
        {
            flags |= PRINTDLGEX_FLAGS.PD_NOCURRENTPAGE;
        }

        if (!_allowPages)
        {
            flags |= PRINTDLGEX_FLAGS.PD_NOPAGENUMS;
        }

        if (!_allowPrintToFile)
        {
            flags |= PRINTDLGEX_FLAGS.PD_DISABLEPRINTTOFILE;
        }

        if (!_allowSelection)
        {
            flags |= PRINTDLGEX_FLAGS.PD_NOSELECTION;
        }

        flags |= (PRINTDLGEX_FLAGS)PrinterSettings.PrintRange;

        if (_printToFile)
        {
            flags |= PRINTDLGEX_FLAGS.PD_PRINTTOFILE;
        }

        if (_showHelp)
        {
            flags |= PRINTDLGEX_FLAGS.PD_SHOWHELP;
        }

        if (!_showNetwork)
        {
            flags |= PRINTDLGEX_FLAGS.PD_NONETWORKBUTTON;
        }

        if (PrinterSettings.Collate)
        {
            flags |= PRINTDLGEX_FLAGS.PD_COLLATE;
        }

        return flags;
    }

    /// <summary>
    ///  Resets all options, the last selected printer, and the page
    ///  settings to their default values.
    /// </summary>
    public override void Reset()
    {
        _allowCurrentPage = false;
        _allowPages = false;
        _allowPrintToFile = true;
        _allowSelection = false;
        _printDocument = null;
        _printToFile = false;
        _printerSettings = null;
        _showHelp = false;
        _showNetwork = true;
    }

    internal static unsafe NativeMethods.PRINTDLGEX CreatePRINTDLGEX()
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
        data.pageRanges = PInvoke.GlobalAlloc(GPTR, (uint)(data.nMaxPageRanges * sizeof(PRINTPAGERANGE)));
        data.nMinPage = 0;
        data.nMaxPage = 9999;
        data.nCopies = 1;
        data.hInstance = IntPtr.Zero;
        data.lpPrintTemplateName = null;
        data.nPropertyPages = 0;
        data.lphPropertyPages = IntPtr.Zero;
        data.nStartPage = unchecked((int)PInvoke.START_PAGE_GENERAL);
        data.dwResultAction = 0;
        return data;
    }

    protected override bool RunDialog(IntPtr hwndOwner)
    {
        if (!UseEXDialog)
        {
            return ShowPrintDialog(hwndOwner);
        }

        NativeMethods.PRINTDLGEX data = CreatePRINTDLGEX();
        return ShowPrintDialog(hwndOwner, data);
    }

    private unsafe bool ShowPrintDialog(IntPtr hwndOwner)
    {
        PRINTDLGW data;
        if (IntPtr.Size == 4)
        {
            data = new PRINTDLGW_32
            {
                lStructSize = (uint)sizeof(PRINTDLGW_32)
            };
        }
        else
        {
            data = new PRINTDLGW_64
            {
                lStructSize = (uint)sizeof(PRINTDLGW_64)
            };
        }

        data.nFromPage = 1;
        data.nToPage = 1;
        data.nMinPage = 0;
        data.nMaxPage = 9999;
        data.Flags = GetFlags();
        data.nCopies = (ushort)PrinterSettings.Copies;
        data.hwndOwner = hwndOwner;

        WNDPROC wndproc = HookProcInternal;
        data.lpfnPrintHook = Marshal.GetFunctionPointerForDelegate(wndproc);

        try
        {
            if (PageSettings is null)
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
            // Leave those fields null; Windows will fill them in
            data.hDevMode = IntPtr.Zero;
            data.hDevNames = IntPtr.Zero;
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

                data.nFromPage = (ushort)PrinterSettings.FromPage;
                data.nToPage = (ushort)PrinterSettings.ToPage;
                data.nMinPage = (ushort)PrinterSettings.MinimumPage;
                data.nMaxPage = (ushort)PrinterSettings.MaximumPage;
            }

            if (!PrintDlg(ref data))
            {
#if DEBUG
                var result = PInvoke.CommDlgExtendedError();
                Diagnostics.Debug.Assert(result == 0, $"PrintDlg returned non zero error code: {result}");
#endif
                return false;
            }

            UpdatePrinterSettings(data.hDevMode, data.hDevNames, (short)data.nCopies, data.Flags, _printerSettings!, PageSettings);

            PrintToFile = (data.Flags & PRINTDLGEX_FLAGS.PD_PRINTTOFILE) != 0;
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
            if ((data.Flags & PRINTDLGEX_FLAGS.PD_USEDEVMODECOPIESANDCOLLATE) == 0)
            {
                PrinterSettings.Copies = (short)data.nCopies;
                PrinterSettings.Collate = (data.Flags & PRINTDLGEX_FLAGS.PD_COLLATE) == PRINTDLGEX_FLAGS.PD_COLLATE;
            }

            return true;
        }
        finally
        {
            GC.KeepAlive(wndproc);
            PInvoke.GlobalFree((HGLOBAL)data.hDevMode);
            PInvoke.GlobalFree((HGLOBAL)data.hDevNames);
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
            if (PageSettings is null)
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

            // The flags NativeMethods.PD_SHOWHELP and NativeMethods.PD_NONETWORKBUTTON don't work with
            // PrintDlgEx. So we have to strip them out.
            data.Flags &= ~(PRINTDLGEX_FLAGS.PD_SHOWHELP | PRINTDLGEX_FLAGS.PD_NONETWORKBUTTON);

            HRESULT hr = UnsafeNativeMethods.PrintDlgEx(data);
            if (hr.Failed || data.dwResultAction == PInvoke.PD_RESULT_CANCEL)
            {
                return false;
            }

            UpdatePrinterSettings(data.hDevMode, data.hDevNames, (short)data.nCopies, data.Flags, PrinterSettings, PageSettings);

            PrintToFile = (data.Flags & PRINTDLGEX_FLAGS.PD_PRINTTOFILE) != 0;
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
            if ((data.Flags & PRINTDLGEX_FLAGS.PD_USEDEVMODECOPIESANDCOLLATE) == 0)
            {
                PrinterSettings.Copies = (short)(data.nCopies);
                PrinterSettings.Collate = (data.Flags & PRINTDLGEX_FLAGS.PD_COLLATE) == PRINTDLGEX_FLAGS.PD_COLLATE;
            }

            // We should return true only if the user pressed the "Print" button while dismissing the dialog.
            return data.dwResultAction == PInvoke.PD_RESULT_PRINT;
        }
        finally
        {
            if (data.hDevMode != IntPtr.Zero)
            {
                PInvoke.GlobalFree((HGLOBAL)data.hDevMode);
            }

            if (data.hDevNames != IntPtr.Zero)
            {
                PInvoke.GlobalFree((HGLOBAL)data.hDevNames);
            }

            if (data.pageRanges != IntPtr.Zero)
            {
                PInvoke.GlobalFree((HGLOBAL)data.pageRanges);
            }
        }
    }

    // Due to the nature of PRINTDLGEX vs PRINTDLG, separate but similar methods
    // are required for updating the settings from the structure utilized by the dialog.
    // Take information from print dialog and put in PrinterSettings
    private static void UpdatePrinterSettings(IntPtr hDevMode, IntPtr hDevNames, short copies, PRINTDLGEX_FLAGS flags, PrinterSettings settings, PageSettings? pageSettings)
    {
        // Mode
        settings.SetHdevmode(hDevMode);
        settings.SetHdevnames(hDevNames);

        pageSettings?.SetHdevmode(hDevMode);

        //Check for Copies == 1 since we might get the Right number of Copies from hdevMode.dmCopies...
        if (settings.Copies == 1)
        {
            settings.Copies = copies;
        }

        settings.PrintRange = (PrintRange)(flags & printRangeMask);
    }
}

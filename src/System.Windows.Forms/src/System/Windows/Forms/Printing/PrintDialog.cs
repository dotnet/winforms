// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using Windows.Win32.UI.Controls.Dialogs;

namespace System.Windows.Forms;

/// <summary>
///  Allows users to select a printer and choose which portions of the document to print.
/// </summary>
[DefaultProperty(nameof(Document))]
[SRDescription(nameof(SR.DescriptionPrintDialog))]
[Designer($"System.Windows.Forms.Design.PrintDialogDesigner, {AssemblyRef.SystemDesign}")]
public sealed class PrintDialog : CommonDialog
{
    // The only event this dialog has is HelpRequested, which isn't very useful

    private const PRINTDLGEX_FLAGS PrintRangeMask = PRINTDLGEX_FLAGS.PD_ALLPAGES
        | PRINTDLGEX_FLAGS.PD_PAGENUMS
        | PRINTDLGEX_FLAGS.PD_SELECTION
        | PRINTDLGEX_FLAGS.PD_CURRENTPAGE;

    // If PrintDocument is not null, settings == printDocument.PrinterSettings
    private PrinterSettings? _printerSettings;
    private PrintDocument? _printDocument;

    /// <summary>
    ///  Initializes a new instance of the <see cref="PrintDialog"/> class.
    /// </summary>
    public PrintDialog() => Reset();

    /// <summary>
    ///  Gets or sets a value indicating whether the Current Page option button is enabled.
    /// </summary>
    [DefaultValue(false)]
    [SRDescription(nameof(SR.PDallowCurrentPageDescr))]
    public bool AllowCurrentPage { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether the Pages option button is enabled.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.PDallowPagesDescr))]
    public bool AllowSomePages { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether the Print to file check box is enabled.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.PDallowPrintToFileDescr))]
    public bool AllowPrintToFile { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether the <b>Selection</b> option button is enabled.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.PDallowSelectionDescr))]
    public bool AllowSelection { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating the <see cref="PrintDocument"/> used to obtain
    ///  <see cref="Drawing.Printing.PrinterSettings"/>.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DefaultValue(null)]
    [SRDescription(nameof(SR.PDdocumentDescr))]
    public PrintDocument? Document
    {
        get => _printDocument;
        set
        {
            _printDocument = value;
            _printerSettings = _printDocument is null ? new PrinterSettings() : _printDocument.PrinterSettings;
        }
    }

    private PageSettings PageSettings => Document is null
        ? PrinterSettings.DefaultPageSettings
        : Document.DefaultPageSettings;

    /// <summary>
    ///  Gets or sets the <see cref="Drawing.Printing.PrinterSettings"/> the dialog box will be modifying.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DefaultValue(null)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.PDprinterSettingsDescr))]
    [AllowNull]
    public PrinterSettings PrinterSettings
    {
        get => _printerSettings ??= new PrinterSettings();
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
    public bool PrintToFile { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether the Help button is displayed.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.PDshowHelpDescr))]
    public bool ShowHelp { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether the Network button is displayed.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.PDshowNetworkDescr))]
    public bool ShowNetwork { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether the dialog should be shown in the Windows XP style for systems
    ///  running Windows XP Home Edition, Windows XP Professional, Windows Server 2003 or later.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   When this property is set to true, <see cref="ShowHelp"/> and <see cref="ShowNetwork"/> will be ignored as
    ///   these properties were made obsolete for Windows 2000 and later versions of Windows.
    ///  </para>
    /// </remarks>
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

        if (!AllowCurrentPage)
        {
            flags |= PRINTDLGEX_FLAGS.PD_NOCURRENTPAGE;
        }

        if (!AllowSomePages)
        {
            flags |= PRINTDLGEX_FLAGS.PD_NOPAGENUMS;
        }

        if (!AllowPrintToFile)
        {
            flags |= PRINTDLGEX_FLAGS.PD_DISABLEPRINTTOFILE;
        }

        if (!AllowSelection)
        {
            flags |= PRINTDLGEX_FLAGS.PD_NOSELECTION;
        }

        flags |= (PRINTDLGEX_FLAGS)PrinterSettings.PrintRange;

        if (PrintToFile)
        {
            flags |= PRINTDLGEX_FLAGS.PD_PRINTTOFILE;
        }

        if (ShowHelp)
        {
            flags |= PRINTDLGEX_FLAGS.PD_SHOWHELP;
        }

        if (!ShowNetwork)
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
    ///  Resets all options, the last selected printer, and the page settings to their default values.
    /// </summary>
    public override void Reset()
    {
        AllowCurrentPage = false;
        AllowSomePages = false;
        AllowPrintToFile = true;
        AllowSelection = false;
        _printDocument = null;
        PrintToFile = false;
        _printerSettings = null;
        ShowHelp = false;
        ShowNetwork = true;
    }

    protected override bool RunDialog(IntPtr hwndOwner) =>
        UseEXDialog ? ShowPrintDialogEx((HWND)hwndOwner) : ShowPrintDialog((HWND)hwndOwner);

    private unsafe bool ShowPrintDialog(HWND hwndOwner)
    {
        // Because of the packing any field after nCopies can't be accessed equivalently on both 32 and 64 bit.
        // This isn't pretty, but it avoids a lot of duplication.

        PRINTDLGW_32 dialogSettings32 = new()
        {
            lStructSize = (uint)sizeof(PRINTDLGW_32),
            lpfnPrintHook = HookProcFunctionPointer
        };

        PRINTDLGW_64 dialogSettings64 = new()
        {
            lStructSize = (uint)sizeof(PRINTDLGW_64),
            lpfnPrintHook = HookProcFunctionPointer
        };

        PRINTDLGW_64* dialogSettings = RuntimeInformation.ProcessArchitecture == Architecture.X86
            ? (PRINTDLGW_64*)&dialogSettings32
            : &dialogSettings64;

        dialogSettings->nFromPage = 1;
        dialogSettings->nToPage = 1;
        dialogSettings->nMaxPage = 9999;
        dialogSettings->Flags = GetFlags();
        dialogSettings->nCopies = (ushort)PrinterSettings.Copies;
        dialogSettings->hwndOwner = hwndOwner;

        try
        {
            dialogSettings->hDevMode = PageSettings is null
                ? (HGLOBAL)PrinterSettings.GetHdevmode()
                : (HGLOBAL)PrinterSettings.GetHdevmode(PageSettings);

            dialogSettings->hDevNames = (HGLOBAL)PrinterSettings.GetHdevnames();
        }
        catch (InvalidPrinterException)
        {
            Debug.Assert(dialogSettings->hDevMode.IsNull && dialogSettings->hDevNames.IsNull);

            // Leave these fields null; Windows will fill them in.
            dialogSettings->hDevMode = HGLOBAL.Null;
            dialogSettings->hDevNames = HGLOBAL.Null;
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

                dialogSettings->nFromPage = (ushort)PrinterSettings.FromPage;
                dialogSettings->nToPage = (ushort)PrinterSettings.ToPage;
                dialogSettings->nMinPage = (ushort)PrinterSettings.MinimumPage;
                dialogSettings->nMaxPage = (ushort)PrinterSettings.MaximumPage;
            }

            BOOL result = RuntimeInformation.ProcessArchitecture == Architecture.X86
                ? PInvoke.PrintDlg(&dialogSettings32)
                : PInvoke.PrintDlg(&dialogSettings64);

            if (!result)
            {
#if DEBUG
                var extendedResult = PInvoke.CommDlgExtendedError();
                if (extendedResult != COMMON_DLG_ERRORS.CDERR_GENERALCODES)
                {
                    Debug.Fail($"PrintDlg returned non zero error code: {extendedResult}");
                }
#endif
                return false;
            }

            UpdatePrinterSettings(
                dialogSettings->hDevMode,
                dialogSettings->hDevNames,
                (short)dialogSettings->nCopies,
                dialogSettings->Flags,
                PrinterSettings,
                PageSettings);

            PrintToFile = (dialogSettings->Flags & PRINTDLGEX_FLAGS.PD_PRINTTOFILE) != 0;
            PrinterSettings.PrintToFile = PrintToFile;

            if (AllowSomePages)
            {
                PrinterSettings.FromPage = dialogSettings->nFromPage;
                PrinterSettings.ToPage = dialogSettings->nToPage;
            }

            // When the flag PD_USEDEVMODECOPIESANDCOLLATE is not set, nCopies indicates the number of copies the user
            // wants to print, and the PD_COLLATE flag in the Flags member indicates whether the user wants to print
            // them collated.
            if (!dialogSettings->Flags.HasFlag(PRINTDLGEX_FLAGS.PD_USEDEVMODECOPIESANDCOLLATE))
            {
                PrinterSettings.Copies = (short)dialogSettings->nCopies;
                PrinterSettings.Collate = dialogSettings->Flags.HasFlag(PRINTDLGEX_FLAGS.PD_COLLATE);
            }

            return result;
        }
        finally
        {
            PInvokeCore.GlobalFree(dialogSettings->hDevMode);
            PInvokeCore.GlobalFree(dialogSettings->hDevNames);
        }
    }

    // Due to the nature of PRINTDLGEX vs PRINTDLG, separate but similar methods
    // are required for showing the print dialog on Win2k and newer OS'.
    private unsafe bool ShowPrintDialogEx(HWND hwndOwner)
    {
        PRINTPAGERANGE pageRange = default;

        PRINTDLGEXW dialogSettings = new()
        {
            lStructSize = (uint)sizeof(PRINTDLGEXW),
            nMaxPageRanges = 1,
            lpPageRanges = &pageRange,
            nMaxPage = 9999,
            nStartPage = PInvoke.START_PAGE_GENERAL,
            Flags = GetFlags(),
            nCopies = (uint)PrinterSettings.Copies,
            hwndOwner = hwndOwner
        };

        try
        {
            dialogSettings.hDevMode = PageSettings is null
                ? (HGLOBAL)PrinterSettings.GetHdevmode()
                : (HGLOBAL)PrinterSettings.GetHdevmode(PageSettings);

            dialogSettings.hDevNames = (HGLOBAL)PrinterSettings.GetHdevnames();
        }
        catch (InvalidPrinterException)
        {
            Debug.Assert(dialogSettings.hDevMode.IsNull && dialogSettings.hDevNames.IsNull);

            // Leave these fields null; Windows will fill them in
            dialogSettings.hDevMode = HGLOBAL.Null;
            dialogSettings.hDevNames = HGLOBAL.Null;
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

                pageRange.nFromPage = (uint)PrinterSettings.FromPage;
                pageRange.nToPage = (uint)PrinterSettings.ToPage;
                dialogSettings.nPageRanges = 1;

                dialogSettings.nMinPage = (uint)PrinterSettings.MinimumPage;
                dialogSettings.nMaxPage = (uint)PrinterSettings.MaximumPage;
            }

            // The flags NativeMethods.PD_SHOWHELP and NativeMethods.PD_NONETWORKBUTTON don't work with
            // PrintDlgEx. So we have to strip them out.
            dialogSettings.Flags &= ~(PRINTDLGEX_FLAGS.PD_SHOWHELP | PRINTDLGEX_FLAGS.PD_NONETWORKBUTTON);

            HRESULT hr = PInvokeCore.PrintDlgEx(&dialogSettings);
            if (hr.Failed || dialogSettings.dwResultAction == PInvoke.PD_RESULT_CANCEL)
            {
                return false;
            }

            UpdatePrinterSettings(
                dialogSettings.hDevMode,
                dialogSettings.hDevNames,
                (short)dialogSettings.nCopies,
                dialogSettings.Flags,
                PrinterSettings,
                PageSettings);

            PrintToFile = dialogSettings.Flags.HasFlag(PRINTDLGEX_FLAGS.PD_PRINTTOFILE);
            PrinterSettings.PrintToFile = PrintToFile;
            if (AllowSomePages)
            {
                PrinterSettings.FromPage = (int)pageRange.nFromPage;
                PrinterSettings.ToPage = (int)pageRange.nToPage;
            }

            // When the flag PD_USEDEVMODECOPIESANDCOLLATE is not set, nCopies indicates the number of copies the user
            // wants to print, and the PD_COLLATE flag in the Flags member indicates whether the user wants to print
            // them collated.
            if (!dialogSettings.Flags.HasFlag(PRINTDLGEX_FLAGS.PD_USEDEVMODECOPIESANDCOLLATE))
            {
                PrinterSettings.Copies = (short)dialogSettings.nCopies;
                PrinterSettings.Collate = dialogSettings.Flags.HasFlag(PRINTDLGEX_FLAGS.PD_COLLATE);
            }

            // We should return true only if the user pressed the "Print" button while dismissing the dialog.
            return dialogSettings.dwResultAction == PInvoke.PD_RESULT_PRINT;
        }
        finally
        {
            if (!dialogSettings.hDevMode.IsNull)
            {
                PInvokeCore.GlobalFree(dialogSettings.hDevMode);
            }

            if (dialogSettings.hDevNames.IsNull)
            {
                PInvokeCore.GlobalFree(dialogSettings.hDevNames);
            }
        }
    }

    // Due to the nature of PRINTDLGEX vs PRINTDLG, separate but similar methods
    // are required for updating the settings from the structure utilized by the dialog.
    // Take information from print dialog and put in PrinterSettings
    private static void UpdatePrinterSettings(
        IntPtr hDevMode,
        IntPtr hDevNames,
        short copies,
        PRINTDLGEX_FLAGS flags,
        PrinterSettings settings,
        PageSettings? pageSettings)
    {
        // Mode
        settings.SetHdevmode(hDevMode);
        settings.SetHdevnames(hDevNames);

        pageSettings?.SetHdevmode(hDevMode);

        // Check for Copies == 1 since we might get the Right number of Copies from dmCopies.
        if (settings.Copies == 1)
        {
            settings.Copies = copies;
        }

        settings.PrintRange = (PrintRange)(flags & PrintRangeMask);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Printing;
using System.Globalization;
using Windows.Win32.UI.Controls.Dialogs;

namespace System.Windows.Forms;

/// <summary>
///  Represents a dialog box that allows users to manipulate page settings,
///  including margins and paper orientation.
/// </summary>
[DefaultProperty(nameof(Document))]
[SRDescription(nameof(SR.DescriptionPageSetupDialog))]
public sealed class PageSetupDialog : CommonDialog
{
    // If PrintDocument is not null, pageSettings == printDocument.PageSettings
    private PrintDocument? _printDocument;
    private PageSettings? _pageSettings;
    private PrinterSettings? _printerSettings;

    private Margins? _minMargins;

    /// <summary>
    ///  Initializes a new instance of the <see cref="PageSetupDialog"/> class.
    /// </summary>
    public PageSetupDialog() => Reset();

    /// <summary>
    ///  Gets or sets a value indicating whether the margins section of the dialog box is enabled.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.PSDallowMarginsDescr))]
    public bool AllowMargins { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether the orientation section of the dialog box (landscape vs. portrait)
    ///  is enabled.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.PSDallowOrientationDescr))]
    public bool AllowOrientation { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether the paper section of the dialog box (paper size and paper source)
    ///  is enabled.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.PSDallowPaperDescr))]
    public bool AllowPaper { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether the Printer button is enabled.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.PSDallowPrinterDescr))]
    public bool AllowPrinter { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating the <see cref="PrintDocument"/> to get page settings from.
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
            if (_printDocument is not null)
            {
                _pageSettings = _printDocument.DefaultPageSettings;
                _printerSettings = _printDocument.PrinterSettings;
            }
        }
    }

    /// <summary>
    ///  This allows the user to override the current behavior where the Metric is converted to ThousandOfInch even
    ///  for METRIC MEASUREMENTSYSTEM which returns a HUNDREDSOFMILLIMETER value.
    /// </summary>
    [DefaultValue(false)]
    [SRDescription(nameof(SR.PSDenableMetricDescr))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public bool EnableMetric { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating the minimum margins the user is allowed to select,
    ///  in hundredths of an inch.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.PSDminMarginsDescr))]
    public Margins? MinMargins
    {
        get => _minMargins;
        set => _minMargins = value ?? new Margins(0, 0, 0, 0);
    }

    /// <summary>
    ///  Gets or sets a value indicating the page settings modified by the dialog box.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DefaultValue(null)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.PSDpageSettingsDescr))]
    public PageSettings? PageSettings
    {
        get => _pageSettings;
        set
        {
            _pageSettings = value;
            _printDocument = null;
        }
    }

    /// <summary>
    ///  Gets or sets the printer settings the dialog box will modify if the user clicks the Printer button.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DefaultValue(null)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.PSDprinterSettingsDescr))]
    public PrinterSettings? PrinterSettings
    {
        get => _printerSettings;
        set
        {
            _printerSettings = value;
            _printDocument = null;
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the Help button is visible.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.PSDshowHelpDescr))]
    public bool ShowHelp { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether the Network button is visible.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.PSDshowNetworkDescr))]
    public bool ShowNetwork { get; set; }

    private PAGESETUPDLG_FLAGS GetFlags()
    {
        PAGESETUPDLG_FLAGS flags = PAGESETUPDLG_FLAGS.PSD_ENABLEPAGESETUPHOOK;

        if (!AllowMargins)
        {
            flags |= PAGESETUPDLG_FLAGS.PSD_DISABLEMARGINS;
        }

        if (!AllowOrientation)
        {
            flags |= PAGESETUPDLG_FLAGS.PSD_DISABLEORIENTATION;
        }

        if (!AllowPaper)
        {
            flags |= PAGESETUPDLG_FLAGS.PSD_DISABLEPAPER;
        }

        if (!AllowPrinter || _printerSettings is null)
        {
            flags |= PAGESETUPDLG_FLAGS.PSD_DISABLEPRINTER;
        }

        if (ShowHelp)
        {
            flags |= PAGESETUPDLG_FLAGS.PSD_SHOWHELP;
        }

        if (!ShowNetwork)
        {
            flags |= PAGESETUPDLG_FLAGS.PSD_NONETWORKBUTTON;
        }

        if (_minMargins is not null)
        {
            flags |= PAGESETUPDLG_FLAGS.PSD_MINMARGINS;
        }

        if (_pageSettings?.Margins is not null)
        {
            flags |= PAGESETUPDLG_FLAGS.PSD_MARGINS;
        }

        return flags;
    }

    /// <summary>
    ///  Resets all options to their default values.
    /// </summary>
    public override void Reset()
    {
        AllowMargins = true;
        AllowOrientation = true;
        AllowPaper = true;
        AllowPrinter = true;
        MinMargins = null; // turns into Margin with all zeros
        _pageSettings = null;
        _printDocument = null;
        _printerSettings = null;
        ShowHelp = false;
        ShowNetwork = true;
    }

    // The next two methods are for designer support.

    private void ResetMinMargins() => MinMargins = null;

    /// <summary>
    ///  Indicates whether the <see cref="MinMargins"/> property should be persisted.
    /// </summary>
    private bool ShouldSerializeMinMargins() =>
        _minMargins is not null
            && (_minMargins.Left != 0
            || _minMargins.Right != 0
            || _minMargins.Top != 0
            || _minMargins.Bottom != 0);

    private static void UpdateSettings(
        PAGESETUPDLGW data,
        PageSettings pageSettings,
        PrinterSettings? printerSettings)
    {
        pageSettings.SetHdevmode(data.hDevMode);
        if (printerSettings is not null)
        {
            printerSettings.SetHdevmode(data.hDevMode);
            printerSettings.SetHdevnames(data.hDevNames);
        }

        Margins newMargins = new()
        {
            Left = data.rtMargin.left,
            Top = data.rtMargin.top,
            Right = data.rtMargin.right,
            Bottom = data.rtMargin.bottom
        };

        PrinterUnit fromUnit = ((data.Flags & PAGESETUPDLG_FLAGS.PSD_INHUNDREDTHSOFMILLIMETERS) != 0)
            ? PrinterUnit.HundredthsOfAMillimeter
            : PrinterUnit.ThousandthsOfAnInch;

        pageSettings.Margins = PrinterUnitConvert.Convert(newMargins, fromUnit, PrinterUnit.Display);
    }

    protected override unsafe bool RunDialog(IntPtr hwndOwner)
    {
        if (_pageSettings is null)
        {
            throw new ArgumentException(SR.PSDcantShowWithoutPage);
        }

        PAGESETUPDLGW dialogSettings = new()
        {
            lStructSize = (uint)sizeof(PAGESETUPDLGW),
            Flags = GetFlags(),
            hwndOwner = (HWND)hwndOwner,
            lpfnPageSetupHook = HookProcFunctionPointer
        };

        PrinterUnit toUnit = PrinterUnit.ThousandthsOfAnInch;

        // EnableMetric allows the users to choose between the AutoConversion or not.
        if (EnableMetric)
        {
            // Take the Units of Measurement while determining the Printer Units.
            Span<char> buffer = stackalloc char[2];
            int result;
            fixed (char* pBuffer = buffer)
            {
                result = PInvoke.GetLocaleInfoEx(
                    PInvoke.LOCALE_NAME_SYSTEM_DEFAULT,
                    PInvoke.LOCALE_IMEASURE,
                    pBuffer,
                    buffer.Length);
            }

            if (result > 0 && int.Parse(buffer, NumberStyles.Integer, CultureInfo.InvariantCulture) == 0)
            {
                toUnit = PrinterUnit.HundredthsOfAMillimeter;
            }
        }

        if (MinMargins is not null)
        {
            Margins margins = PrinterUnitConvert.Convert(MinMargins, PrinterUnit.Display, toUnit);
            dialogSettings.rtMinMargin.left = margins.Left;
            dialogSettings.rtMinMargin.top = margins.Top;
            dialogSettings.rtMinMargin.right = margins.Right;
            dialogSettings.rtMinMargin.bottom = margins.Bottom;
        }

        if (_pageSettings.Margins is not null)
        {
            Margins margins = PrinterUnitConvert.Convert(_pageSettings.Margins, PrinterUnit.Display, toUnit);
            dialogSettings.rtMargin.left = margins.Left;
            dialogSettings.rtMargin.top = margins.Top;
            dialogSettings.rtMargin.right = margins.Right;
            dialogSettings.rtMargin.bottom = margins.Bottom;
        }

        // Ensure that the margins are >= minMargins.
        // This is a requirement of the PAGESETUPDLG structure.
        dialogSettings.rtMargin.left = Math.Max(dialogSettings.rtMargin.left, dialogSettings.rtMinMargin.left);
        dialogSettings.rtMargin.top = Math.Max(dialogSettings.rtMargin.top, dialogSettings.rtMinMargin.top);
        dialogSettings.rtMargin.right = Math.Max(dialogSettings.rtMargin.right, dialogSettings.rtMinMargin.right);
        dialogSettings.rtMargin.bottom = Math.Max(dialogSettings.rtMargin.bottom, dialogSettings.rtMinMargin.bottom);

        PrinterSettings printer = _printerSettings ?? _pageSettings.PrinterSettings;

        dialogSettings.hDevMode = (HGLOBAL)printer.GetHdevmode(_pageSettings);
        dialogSettings.hDevNames = (HGLOBAL)printer.GetHdevnames();

        try
        {
            if (!PInvoke.PageSetupDlg(&dialogSettings))
            {
                return false;
            }

            // PrinterSettings, not printer
            UpdateSettings(dialogSettings, _pageSettings, _printerSettings);
            return true;
        }
        finally
        {
            PInvokeCore.GlobalFree(dialogSettings.hDevMode);
            PInvokeCore.GlobalFree(dialogSettings.hDevNames);
        }
    }
}

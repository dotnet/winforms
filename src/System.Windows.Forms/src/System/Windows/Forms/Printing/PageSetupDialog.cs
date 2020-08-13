// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing.Printing;
using System.Globalization;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a dialog box that allows users to manipulate page settings,
    ///  including margins and paper orientation.
    /// </summary>
    [DefaultProperty(nameof(Document))]
    [SRDescription(nameof(SR.DescriptionPageSetupDialog))]
    public sealed class PageSetupDialog : CommonDialog
    {
        // If PrintDocument != null, pageSettings == printDocument.PageSettings
        private PrintDocument _printDocument;
        private PageSettings _pageSettings;
        private PrinterSettings _printerSettings;

        private Margins _minMargins;

        /// <summary>
        ///  Initializes a new instance of the <see cref='PageSetupDialog'/> class.
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
        ///  Gets or sets a value indicating the <see cref='PrintDocument'/> to get page settings from.
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        [DefaultValue(null)]
        [SRDescription(nameof(SR.PDdocumentDescr))]
        public PrintDocument Document
        {
            get => _printDocument;
            set
            {
                _printDocument = value;
                if (_printDocument != null)
                {
                    _pageSettings = _printDocument.DefaultPageSettings;
                    _printerSettings = _printDocument.PrinterSettings;
                }
            }
        }

        /// <summary>
        ///  This allows the user to override the current behavior where the Metric is converted to ThousandOfInch even for METRIC MEASUREMENTSYSTEM
        ///  which returns a HUNDREDSOFMILLIMETER value.
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
        public Margins MinMargins
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
        public PageSettings PageSettings
        {
            get => _pageSettings;
            set
            {
                _pageSettings = value;
                _printDocument = null;
            }
        }

        /// <summary>
        ///  Gets
        ///  or sets the printer
        ///  settings the dialog box will modify if the user clicks the Printer button.
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        [DefaultValue(null)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.PSDprinterSettingsDescr))]
        public PrinterSettings PrinterSettings
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

        private Comdlg32.PSD GetFlags()
        {
            Comdlg32.PSD flags = Comdlg32.PSD.ENABLEPAGESETUPHOOK;

            if (!AllowMargins)
            {
                flags |= Comdlg32.PSD.DISABLEMARGINS;
            }

            if (!AllowOrientation)
            {
                flags |= Comdlg32.PSD.DISABLEORIENTATION;
            }

            if (!AllowPaper)
            {
                flags |= Comdlg32.PSD.DISABLEPAPER;
            }

            if (!AllowPrinter || _printerSettings is null)
            {
                flags |= Comdlg32.PSD.DISABLEPRINTER;
            }

            if (ShowHelp)
            {
                flags |= Comdlg32.PSD.SHOWHELP;
            }

            if (!ShowNetwork)
            {
                flags |= Comdlg32.PSD.NONETWORKBUTTON;
            }

            if (_minMargins != null)
            {
                flags |= Comdlg32.PSD.MINMARGINS;
            }

            if (_pageSettings.Margins != null)
            {
                flags |= Comdlg32.PSD.MARGINS;
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

        private void ResetMinMargins()
        {
            MinMargins = null;
        }

        /// <summary>
        ///  Indicates whether the <see cref='MinMargins'/>
        ///  property should be
        ///  persisted.
        /// </summary>
        private bool ShouldSerializeMinMargins()
        {
            return _minMargins.Left != 0
                || _minMargins.Right != 0
                || _minMargins.Top != 0
                || _minMargins.Bottom != 0;
        }

        private static void UpdateSettings(Comdlg32.PAGESETUPDLGW data, PageSettings pageSettings,
                                           PrinterSettings printerSettings)
        {
            pageSettings.SetHdevmode(data.hDevMode);
            if (printerSettings != null)
            {
                printerSettings.SetHdevmode(data.hDevMode);
                printerSettings.SetHdevnames(data.hDevNames);
            }

            Margins newMargins = new Margins
            {
                Left = data.rtMargin.left,
                Top = data.rtMargin.top,
                Right = data.rtMargin.right,
                Bottom = data.rtMargin.bottom
            };

            PrinterUnit fromUnit = ((data.Flags & Comdlg32.PSD.INHUNDREDTHSOFMILLIMETERS) != 0)
                                   ? PrinterUnit.HundredthsOfAMillimeter
                                   : PrinterUnit.ThousandthsOfAnInch;

            pageSettings.Margins = PrinterUnitConvert.Convert(newMargins, fromUnit, PrinterUnit.Display);
        }

        protected unsafe override bool RunDialog(IntPtr hwndOwner)
        {
            var hookProcPtr = new User32.WNDPROCINT(HookProc);
            if (_pageSettings is null)
            {
                throw new ArgumentException(SR.PSDcantShowWithoutPage);
            }

            var data = new Comdlg32.PAGESETUPDLGW();
            data.lStructSize = (uint)Marshal.SizeOf<Comdlg32.PAGESETUPDLGW>();
            data.Flags = GetFlags();
            data.hwndOwner = hwndOwner;
            data.lpfnPageSetupHook = hookProcPtr;

            PrinterUnit toUnit = PrinterUnit.ThousandthsOfAnInch;

            // EnableMetric allows the users to choose between the AutoConversion or not.
            if (EnableMetric)
            {
                //take the Units of Measurement while determining the PrinterUnits...
                Span<char> buffer = stackalloc char[2];
                int result;
                fixed (char* pBuffer = buffer)
                {
                    result = Kernel32.GetLocaleInfoEx(Kernel32.LOCALE_NAME_USER_DEFAULT, Kernel32.LCTYPE.IMEASURE, pBuffer, 2);
                }

                if (result > 0 && int.Parse(buffer, NumberStyles.Integer, CultureInfo.InvariantCulture) == 0)
                {
                    toUnit = PrinterUnit.HundredthsOfAMillimeter;
                }
            }

            if (MinMargins != null)
            {
                Margins margins = PrinterUnitConvert.Convert(MinMargins, PrinterUnit.Display, toUnit);
                data.rtMinMargin.left = margins.Left;
                data.rtMinMargin.top = margins.Top;
                data.rtMinMargin.right = margins.Right;
                data.rtMinMargin.bottom = margins.Bottom;
            }

            if (_pageSettings.Margins != null)
            {
                Margins margins = PrinterUnitConvert.Convert(_pageSettings.Margins, PrinterUnit.Display, toUnit);
                data.rtMargin.left = margins.Left;
                data.rtMargin.top = margins.Top;
                data.rtMargin.right = margins.Right;
                data.rtMargin.bottom = margins.Bottom;
            }

            // Ensure that the margins are >= minMargins.
            // This is a requirement of the PAGESETUPDLG structure.
            data.rtMargin.left = Math.Max(data.rtMargin.left, data.rtMinMargin.left);
            data.rtMargin.top = Math.Max(data.rtMargin.top, data.rtMinMargin.top);
            data.rtMargin.right = Math.Max(data.rtMargin.right, data.rtMinMargin.right);
            data.rtMargin.bottom = Math.Max(data.rtMargin.bottom, data.rtMinMargin.bottom);

            PrinterSettings printer = _printerSettings ?? _pageSettings.PrinterSettings;

            data.hDevMode = printer.GetHdevmode(_pageSettings);
            data.hDevNames = printer.GetHdevnames();

            try
            {
                if (Comdlg32.PageSetupDlgW(ref data).IsFalse())
                {
                    return false;
                }

                // PrinterSettings, not printer
                UpdateSettings(data, _pageSettings, _printerSettings);
                return true;
            }
            finally
            {
                Kernel32.GlobalFree(data.hDevMode);
                Kernel32.GlobalFree(data.hDevNames);
            }
        }
    }
}

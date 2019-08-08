// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing.Printing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a dialog box that allows users to manipulate page settings,
    ///  including margins and paper orientation.
    /// </summary>
    [DefaultProperty(nameof(Document))]
    [SRDescription(nameof(SR.DescriptionPageSetupDialog))]
    // The only event this dialog has is HelpRequested, which isn't very useful
    public sealed class PageSetupDialog : CommonDialog
    {
        // If PrintDocument != null, pageSettings == printDocument.PageSettings
        private PrintDocument printDocument = null;
        private PageSettings pageSettings = null;
        private PrinterSettings printerSettings = null;

        private bool allowMargins;
        private bool allowOrientation;
        private bool allowPaper;
        private bool allowPrinter;
        private Margins minMargins;
        private bool showHelp;
        private bool showNetwork;
        private bool enableMetric;

        /// <summary>
        ///  Initializes a new instance of the <see cref='PageSetupDialog'/> class.
        /// </summary>
        public PageSetupDialog()
        {
            Reset();
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the margins section of the dialog box is enabled.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.PSDallowMarginsDescr))
        ]
        public bool AllowMargins
        {
            get
            {
                return allowMargins;
            }
            set
            {
                allowMargins = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the orientation section of the dialog box (landscape vs. portrait)
        ///  is enabled.
            /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.PSDallowOrientationDescr))
        ]
        public bool AllowOrientation
        {
            get { return allowOrientation; }
            set { allowOrientation = value; }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the paper section of the dialog box (paper size and paper source)
        ///  is enabled.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.PSDallowPaperDescr))
        ]
        public bool AllowPaper
        {
            get { return allowPaper; }
            set { allowPaper = value; }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the Printer button is enabled.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.PSDallowPrinterDescr))
        ]
        public bool AllowPrinter
        {
            get { return allowPrinter; }
            set { allowPrinter = value; }
        }

        /// <summary>
        ///  Gets or sets a value indicating the <see cref='PrintDocument'/>
        ///  to get page settings from.
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
                if (printDocument != null)
                {
                    pageSettings = printDocument.DefaultPageSettings;
                    printerSettings = printDocument.PrinterSettings;
                }
            }
        }

        /// <summary>
        ///  This allows the user to override the current behavior where the Metric is converted to ThousandOfInch even for METRIC MEASUREMENTSYSTEM
        ///  which returns a HUNDREDSOFMILLIMETER value.
        /// </summary>
        [
        DefaultValue(false),
        SRDescription(nameof(SR.PSDenableMetricDescr)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public bool EnableMetric
        {
            get { return enableMetric; }
            set { enableMetric = value; }
        }

        /// <summary>
        ///  Gets or sets a value indicating the minimum margins the
        ///  user is allowed to select, in hundredths of an inch.
        /// </summary>
        [
        SRCategory(nameof(SR.CatData)),
        SRDescription(nameof(SR.PSDminMarginsDescr))
        ]
        public Margins MinMargins
        {
            get { return minMargins; }
            set
            {
                if (value == null)
                {
                    value = new Margins(0, 0, 0, 0);
                }

                minMargins = value;
            }
        }

        /// <summary>
        ///  Gets
        ///  or sets
        ///  a value indicating
        ///  the page settings modified by the dialog box.
        /// </summary>
        [
        SRCategory(nameof(SR.CatData)),
        DefaultValue(null),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.PSDpageSettingsDescr))
        ]
        public PageSettings PageSettings
        {
            get { return pageSettings; }
            set
            {
                pageSettings = value;
                printDocument = null;
            }
        }

        /// <summary>
        ///  Gets
        ///  or sets the printer
        ///  settings the dialog box will modify if the user clicks the Printer button.
        /// </summary>
        [
        SRCategory(nameof(SR.CatData)),
        DefaultValue(null),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.PSDprinterSettingsDescr))
        ]
        public PrinterSettings PrinterSettings
        {
            get { return printerSettings; }
            set
            {
                printerSettings = value;
                printDocument = null;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the Help button is visible.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.PSDshowHelpDescr))
        ]
        public bool ShowHelp
        {
            get { return showHelp; }
            set { showHelp = value; }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the Network button is visible.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.PSDshowNetworkDescr))
        ]
        public bool ShowNetwork
        {
            get { return showNetwork; }
            set { showNetwork = value; }
        }

        private int GetFlags()
        {
            int flags = 0;
            flags |= NativeMethods.PSD_ENABLEPAGESETUPHOOK;

            if (!allowMargins)
            {
                flags |= NativeMethods.PSD_DISABLEMARGINS;
            }

            if (!allowOrientation)
            {
                flags |= NativeMethods.PSD_DISABLEORIENTATION;
            }

            if (!allowPaper)
            {
                flags |= NativeMethods.PSD_DISABLEPAPER;
            }

            if (!allowPrinter || printerSettings == null)
            {
                flags |= NativeMethods.PSD_DISABLEPRINTER;
            }

            if (showHelp)
            {
                flags |= NativeMethods.PSD_SHOWHELP;
            }

            if (!showNetwork)
            {
                flags |= NativeMethods.PSD_NONETWORKBUTTON;
            }

            if (minMargins != null)
            {
                flags |= NativeMethods.PSD_MINMARGINS;
            }

            if (pageSettings.Margins != null)
            {
                flags |= NativeMethods.PSD_MARGINS;
            }

            //
            return flags;
        }

        /// <summary>
        ///  Resets all options to their default values.
        /// </summary>
        public override void Reset()
        {
            allowMargins = true;
            allowOrientation = true;
            allowPaper = true;
            allowPrinter = true;
            MinMargins = null; // turns into Margin with all zeros
            pageSettings = null;
            printDocument = null;
            printerSettings = null;
            showHelp = false;
            showNetwork = true;
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
            return minMargins.Left != 0
            || minMargins.Right != 0
            || minMargins.Top != 0
            || minMargins.Bottom != 0;
        }

        private static void UpdateSettings(NativeMethods.PAGESETUPDLG data, PageSettings pageSettings,
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
                Left = data.marginLeft,
                Top = data.marginTop,
                Right = data.marginRight,
                Bottom = data.marginBottom
            };

            PrinterUnit fromUnit = ((data.Flags & NativeMethods.PSD_INHUNDREDTHSOFMILLIMETERS) != 0)
                                   ? PrinterUnit.HundredthsOfAMillimeter
                                   : PrinterUnit.ThousandthsOfAnInch;

            pageSettings.Margins = PrinterUnitConvert.Convert(newMargins, fromUnit, PrinterUnit.Display);
        }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            NativeMethods.WndProc hookProcPtr = new NativeMethods.WndProc(HookProc);
            if (pageSettings == null)
            {
                throw new ArgumentException(SR.PSDcantShowWithoutPage);
            }

            NativeMethods.PAGESETUPDLG data = new NativeMethods.PAGESETUPDLG();
            data.lStructSize = Marshal.SizeOf(data);
            data.Flags = GetFlags();
            data.hwndOwner = hwndOwner;
            data.lpfnPageSetupHook = hookProcPtr;

            PrinterUnit toUnit = PrinterUnit.ThousandthsOfAnInch;

            // Below was a breaking change from RTM and EVERETT even though this was a correct FIX.
            // EnableMetric is a new Whidbey property which we allow the users to choose between the AutoConversion or not.
            if (EnableMetric)
            {
                //take the Units of Measurement while determining the PrinterUnits...
                //
                StringBuilder sb = new StringBuilder(2);
                int result = UnsafeNativeMethods.GetLocaleInfo(NativeMethods.LOCALE_USER_DEFAULT, NativeMethods.LOCALE_IMEASURE, sb, sb.Capacity);

                if (result > 0 && int.Parse(sb.ToString(), CultureInfo.InvariantCulture) == 0)
                {
                    toUnit = PrinterUnit.HundredthsOfAMillimeter;
                }
            }

            if (MinMargins != null)
            {
                Margins margins = PrinterUnitConvert.Convert(MinMargins, PrinterUnit.Display, toUnit);
                data.minMarginLeft = margins.Left;
                data.minMarginTop = margins.Top;
                data.minMarginRight = margins.Right;
                data.minMarginBottom = margins.Bottom;
            }

            if (pageSettings.Margins != null)
            {
                Margins margins = PrinterUnitConvert.Convert(pageSettings.Margins, PrinterUnit.Display, toUnit);
                data.marginLeft = margins.Left;
                data.marginTop = margins.Top;
                data.marginRight = margins.Right;
                data.marginBottom = margins.Bottom;
            }

            // Ensure that the margins are >= minMargins.
            // This is a requirement of the PAGESETUPDLG structure.
            //
            data.marginLeft = Math.Max(data.marginLeft, data.minMarginLeft);
            data.marginTop = Math.Max(data.marginTop, data.minMarginTop);
            data.marginRight = Math.Max(data.marginRight, data.minMarginRight);
            data.marginBottom = Math.Max(data.marginBottom, data.minMarginBottom);

            PrinterSettings printer = printerSettings ?? pageSettings.PrinterSettings;

            data.hDevMode = printer.GetHdevmode(pageSettings);
            data.hDevNames = printer.GetHdevnames();

            try
            {
                bool status = UnsafeNativeMethods.PageSetupDlg(data);
                if (!status)
                {
                    // Debug.WriteLine(Windows.CommonDialogErrorToString(Windows.CommDlgExtendedError()));
                    return false;
                }

                UpdateSettings(data, pageSettings, printerSettings); // yes, printerSettings, not printer
                return true;
            }
            finally
            {
                UnsafeNativeMethods.GlobalFree(new HandleRef(data, data.hDevMode));
                UnsafeNativeMethods.GlobalFree(new HandleRef(data, data.hDevNames));
            }
        }
    }
}


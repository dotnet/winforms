// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Printing;

/// <summary>
///  Specifies settings that apply to a single page.
/// </summary>
public unsafe class PageSettings : ICloneable
{
    private PrinterSettings _printerSettings;
    private TriState _color = TriState.Default;
    private PaperSize? _paperSize;
    private PaperSource? _paperSource;
    private PrinterResolution? _printerResolution;
    private TriState _landscape = TriState.Default;
    private Margins _margins = new();

    /// <summary>
    ///  Initializes a new instance of the <see cref='PageSettings'/> class using the default printer.
    /// </summary>
    public PageSettings() : this(new PrinterSettings())
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='PageSettings'/> class using the specified printer.
    /// </summary>
    public PageSettings(PrinterSettings printerSettings)
    {
        Debug.Assert(printerSettings is not null, "printerSettings == null");
        _printerSettings = printerSettings;
    }

    /// <summary>
    ///  Gets the bounds of the page, taking into account the Landscape property.
    /// </summary>
    public Rectangle Bounds
    {
        get
        {
            HGLOBAL modeHandle = (HGLOBAL)_printerSettings.GetHdevmode();
            Rectangle pageBounds = GetBounds(modeHandle);

            PInvokeCore.GlobalFree(modeHandle);
            return pageBounds;
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the page is printed in color.
    /// </summary>
    public bool Color
    {
        get => _color.IsDefault
            ? _printerSettings.GetModeField(ModeField.Color, (short)DEVMODE_COLOR.DMCOLOR_MONOCHROME) == (short)DEVMODE_COLOR.DMCOLOR_MONOCHROME
            : (bool)_color;
        set => _color = value;
    }

    /// <summary>
    ///  Returns the x dimension of the hard margin
    /// </summary>
    public float HardMarginX
    {
        get
        {
            using var hdc = _printerSettings.CreateDeviceContext(this);

            int dpiX = PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.LOGPIXELSX);
            int hardMarginX_DU = PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.PHYSICALOFFSETX);
            return hardMarginX_DU * 100 / dpiX;
        }
    }

    /// <summary>
    ///  Returns the y dimension of the hard margin.
    /// </summary>
    public float HardMarginY
    {
        get
        {
            using var hdc = _printerSettings.CreateDeviceContext(this);

            int dpiY = PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.LOGPIXELSY);
            int hardMarginY_DU = PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.PHYSICALOFFSETY);
            return hardMarginY_DU * 100 / dpiY;
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the page should be printed in landscape or portrait orientation.
    /// </summary>
    public bool Landscape
    {
        get => _landscape.IsDefault
            ? _printerSettings.GetModeField(ModeField.Orientation, (short)PInvokeCore.DMORIENT_PORTRAIT) == PInvokeCore.DMORIENT_LANDSCAPE
            : (bool)_landscape;
        set => _landscape = value;
    }

    /// <summary>
    ///  Gets or sets a value indicating the margins for this page.
    /// </summary>
    public Margins Margins
    {
        get => _margins;
        set => _margins = value;
    }

    /// <summary>
    ///  Gets or sets the paper size.
    /// </summary>
    public PaperSize PaperSize
    {
        get => GetPaperSize(HGLOBAL.Null);
        set => _paperSize = value;
    }

    /// <summary>
    ///  Gets or sets a value indicating the paper source (i.e. upper bin).
    /// </summary>
    public PaperSource PaperSource
    {
        get
        {
            if (_paperSource is not null)
            {
                return _paperSource;
            }

            HGLOBAL modeHandle = (HGLOBAL)_printerSettings.GetHdevmode();
            DEVMODEW* devmode = (DEVMODEW*)PInvokeCore.GlobalLock(modeHandle);

            PaperSource result = PaperSourceFromMode(devmode);

            PInvokeCore.GlobalUnlock(modeHandle);
            PInvokeCore.GlobalFree(modeHandle);

            return result;
        }
        set => _paperSource = value;
    }

    /// <summary>
    ///  Gets the PrintableArea for the printer. Units = 100ths of an inch.
    /// </summary>
    public RectangleF PrintableArea
    {
        get
        {
            RectangleF printableArea = default;
            using var hdc = _printerSettings.CreateInformationContext(this);

            int dpiX = PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.LOGPIXELSX);
            int dpiY = PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.LOGPIXELSY);
            if (!Landscape)
            {
                // Need to convert the printable area to 100th of an inch from the device units
                printableArea.X = (float)PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.PHYSICALOFFSETX) * 100 / dpiX;
                printableArea.Y = (float)PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.PHYSICALOFFSETY) * 100 / dpiY;
                printableArea.Width = (float)PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.HORZRES) * 100 / dpiX;
                printableArea.Height = (float)PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.VERTRES) * 100 / dpiY;
            }
            else
            {
                // Need to convert the printable area to 100th of an inch from the device units
                printableArea.Y = (float)PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.PHYSICALOFFSETX) * 100 / dpiX;
                printableArea.X = (float)PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.PHYSICALOFFSETY) * 100 / dpiY;
                printableArea.Height = (float)PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.HORZRES) * 100 / dpiX;
                printableArea.Width = (float)PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.VERTRES) * 100 / dpiY;
            }

            return printableArea;
        }
    }

    /// <summary>
    ///  Gets or sets the printer resolution for the page.
    /// </summary>
    public PrinterResolution PrinterResolution
    {
        get
        {
            if (_printerResolution is not null)
            {
                return _printerResolution;
            }

            HGLOBAL modeHandle = (HGLOBAL)_printerSettings.GetHdevmode();
            DEVMODEW* devmode = (DEVMODEW*)PInvokeCore.GlobalLock(modeHandle);
            PrinterResolution result = PrinterResolutionFromMode(devmode);

            PInvokeCore.GlobalUnlock(modeHandle);
            PInvokeCore.GlobalFree(modeHandle);

            return result;
        }
        set => _printerResolution = value;
    }

    /// <summary>
    ///  Gets or sets the associated printer settings.
    /// </summary>
    public PrinterSettings PrinterSettings
    {
        get => _printerSettings;
        set => _printerSettings = value ?? new PrinterSettings();
    }

    /// <summary>
    ///  Copies the settings and margins.
    /// </summary>
    public object Clone()
    {
        PageSettings result = (PageSettings)MemberwiseClone();
        result._margins = (Margins)_margins.Clone();
        return result;
    }

    /// <summary>
    ///  Copies the relevant information out of the PageSettings and into the handle.
    /// </summary>
    public void CopyToHdevmode(IntPtr hdevmode)
    {
        if (hdevmode == 0)
        {
            throw new ArgumentNullException(nameof(hdevmode));
        }

        DEVMODEW* devmode = (DEVMODEW*)PInvokeCore.GlobalLock((HGLOBAL)hdevmode);

        if (_color.IsNotDefault && devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_COLOR))
        {
            devmode->dmColor = _color.IsTrue ? DEVMODE_COLOR.DMCOLOR_COLOR : DEVMODE_COLOR.DMCOLOR_MONOCHROME;
        }

        if (_landscape.IsNotDefault && devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_ORIENTATION))
        {
            devmode->dmOrientation = _landscape.IsTrue ? (short)PInvokeCore.DMORIENT_LANDSCAPE : (short)PInvokeCore.DMORIENT_PORTRAIT;
        }

        if (_paperSize is not null)
        {
            if (devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_PAPERSIZE))
            {
                devmode->dmPaperSize = (short)_paperSize.RawKind;
            }

            bool setWidth = false;
            bool setLength = false;

            if (devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_PAPERLENGTH))
            {
                // dmPaperLength is always in tenths of millimeter but paperSizes are in hundredth of inch ..
                // so we need to convert :: use PrinterUnitConvert.Convert(value, PrinterUnit.
                // TenthsOfAMillimeter /*fromUnit*/, PrinterUnit.Display /*ToUnit*/)
                int length = PrinterUnitConvert.Convert(_paperSize.Height, PrinterUnit.Display, PrinterUnit.TenthsOfAMillimeter);
                devmode->dmPaperLength = (short)length;
                setLength = true;
            }

            if (devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_PAPERWIDTH))
            {
                int width = PrinterUnitConvert.Convert(_paperSize.Width, PrinterUnit.Display, PrinterUnit.TenthsOfAMillimeter);
                devmode->dmPaperWidth = (short)width;
                setWidth = true;
            }

            if (_paperSize.Kind == PaperKind.Custom)
            {
                if (!setLength)
                {
                    devmode->dmFields |= DEVMODE_FIELD_FLAGS.DM_PAPERLENGTH;
                    int length = PrinterUnitConvert.Convert(_paperSize.Height, PrinterUnit.Display, PrinterUnit.TenthsOfAMillimeter);
                    devmode->dmPaperLength = (short)length;
                }

                if (!setWidth)
                {
                    devmode->dmFields |= DEVMODE_FIELD_FLAGS.DM_PAPERWIDTH;
                    int width = PrinterUnitConvert.Convert(_paperSize.Width, PrinterUnit.Display, PrinterUnit.TenthsOfAMillimeter);
                    devmode->dmPaperWidth = (short)width;
                }
            }
        }

        if (_paperSource is not null && devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_DEFAULTSOURCE))
        {
            devmode->dmDefaultSource = (short)_paperSource.RawKind;
        }

        if (_printerResolution is not null)
        {
            if (_printerResolution.Kind == PrinterResolutionKind.Custom)
            {
                if (devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_PRINTQUALITY))
                {
                    devmode->dmPrintQuality = (short)_printerResolution.X;
                }

                if (devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_YRESOLUTION))
                {
                    devmode->dmYResolution = (short)_printerResolution.Y;
                }
            }
            else
            {
                if (devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_PRINTQUALITY))
                {
                    devmode->dmPrintQuality = (short)_printerResolution.Kind;
                }
            }
        }

        // It's possible this page has a DEVMODE for a different printer than the DEVMODE passed in here
        // (Ex: occurs when Doc.DefaultPageSettings.PrinterSettings.PrinterName != Doc.PrinterSettings.PrinterName)
        //
        // If the passed in devmode has fewer bytes than our buffer for the extra info, we want to skip the merge
        // as it will cause a buffer overrun.
        if (devmode->dmDriverExtra >= ExtraBytes)
        {
            fixed (char* n = _printerSettings.PrinterName)
            {
                int result = PInvoke.DocumentProperties(
                    HWND.Null,
                    HANDLE.Null,
                    n,
                    devmode,
                    devmode,
                    (uint)(DEVMODE_FIELD_FLAGS.DM_IN_BUFFER | DEVMODE_FIELD_FLAGS.DM_OUT_BUFFER));

                if (result < 0)
                {
                    PInvokeCore.GlobalFree((HGLOBAL)hdevmode);
                }
            }
        }

        PInvokeCore.GlobalUnlock((HGLOBAL)hdevmode);
    }

    private short ExtraBytes
    {
        get
        {
            HGLOBAL modeHandle = _printerSettings.GetHdevmodeInternal();
            DEVMODEW* devmode = (DEVMODEW*)PInvokeCore.GlobalLock(modeHandle);

            short result = devmode is null ? default : (short)devmode->dmDriverExtra;

            PInvokeCore.GlobalUnlock(modeHandle);
            PInvokeCore.GlobalFree(modeHandle);

            return result;
        }
    }

    internal Rectangle GetBounds(HGLOBAL modeHandle)
    {
        PaperSize size = GetPaperSize(modeHandle);
        return IsLandscape(modeHandle)
            ? new Rectangle(0, 0, size.Height, size.Width)
            : new Rectangle(0, 0, size.Width, size.Height);

        bool IsLandscape(HGLOBAL modeHandle) => _landscape.IsDefault
            ? _printerSettings.GetModeField(ModeField.Orientation, (short)PInvokeCore.DMORIENT_PORTRAIT, modeHandle) == PInvokeCore.DMORIENT_LANDSCAPE
            : (bool)_landscape;
    }

    private PaperSize GetPaperSize(HGLOBAL modeHandle)
    {
        if (_paperSize is not null)
        {
            return _paperSize;
        }

        bool ownHandle = false;
        if (modeHandle.IsNull)
        {
            modeHandle = (HGLOBAL)_printerSettings.GetHdevmode();
            ownHandle = true;
        }

        DEVMODEW* devmode = (DEVMODEW*)PInvokeCore.GlobalLock(modeHandle);

        PaperSize result = PaperSizeFromMode(devmode);

        PInvokeCore.GlobalUnlock(modeHandle);

        if (ownHandle)
        {
            PInvokeCore.GlobalFree(modeHandle);
        }

        return result;
    }

    private PaperSize PaperSizeFromMode(DEVMODEW* devmode)
    {
        PaperSize[] sizes = _printerSettings.Get_PaperSizes();
        if (devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_PAPERSIZE))
        {
            for (int i = 0; i < sizes.Length; i++)
            {
                if (sizes[i].RawKind == devmode->Anonymous1.Anonymous1.dmPaperSize)
                {
                    return sizes[i];
                }
            }
        }

        return new PaperSize(
            PaperKind.Custom,
            "custom",
            PrinterUnitConvert.Convert(devmode->Anonymous1.Anonymous1.dmPaperWidth, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display),
            PrinterUnitConvert.Convert(devmode->Anonymous1.Anonymous1.dmPaperLength, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display));
    }

    private PaperSource PaperSourceFromMode(DEVMODEW* devmode)
    {
        PaperSource[] sources = _printerSettings.Get_PaperSources();
        if (devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_DEFAULTSOURCE))
        {
            for (int i = 0; i < sources.Length; i++)
            {
                // The dmDefaultSource == to the RawKind in the PaperSource and not the Kind
                // if the PaperSource is populated with CUSTOM values.
                if ((short)sources[i].RawKind == devmode->dmDefaultSource)
                {
                    return sources[i];
                }
            }
        }

        return new PaperSource((PaperSourceKind)devmode->dmDefaultSource, "unknown");
    }

    private PrinterResolution PrinterResolutionFromMode(DEVMODEW* devmode)
    {
        PrinterResolution[] resolutions = _printerSettings.Get_PrinterResolutions();
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (devmode->dmPrintQuality >= 0
                && devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_PRINTQUALITY)
                && devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_YRESOLUTION))
            {
                if (resolutions[i].X == devmode->dmPrintQuality && resolutions[i].Y == devmode->dmYResolution)
                {
                    return resolutions[i];
                }
            }
            else
            {
                if (devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_PRINTQUALITY)
                    && resolutions[i].Kind == (PrinterResolutionKind)devmode->dmPrintQuality)
                {
                    return resolutions[i];
                }
            }
        }

        return new PrinterResolution(
            PrinterResolutionKind.Custom,
            devmode->dmPrintQuality,
            devmode->dmYResolution);
    }

    /// <summary>
    ///  Copies the relevant information out of the handle and into the PageSettings.
    /// </summary>
    public void SetHdevmode(IntPtr hdevmode)
    {
        if (hdevmode == 0)
        {
            throw new ArgumentException(SR.Format(SR.InvalidPrinterHandle, hdevmode));
        }

        DEVMODEW* devmode = (DEVMODEW*)PInvokeCore.GlobalLock((HGLOBAL)hdevmode);

        if (devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_COLOR))
        {
            _color = devmode->dmColor == DEVMODE_COLOR.DMCOLOR_COLOR;
        }

        if (devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_ORIENTATION))
        {
            _landscape = devmode->dmOrientation == PInvokeCore.DMORIENT_LANDSCAPE;
        }

        _paperSize = PaperSizeFromMode(devmode);
        _paperSource = PaperSourceFromMode(devmode);
        _printerResolution = PrinterResolutionFromMode(devmode);

        PInvokeCore.GlobalUnlock((HGLOBAL)hdevmode);
    }

    public override string ToString() =>
        $"[{nameof(PageSettings)}: Color={Color}, Landscape={Landscape}, Margins={Margins}, PaperSize={PaperSize}, PaperSource={PaperSource}, PrinterResolution={PrinterResolution}]";
}

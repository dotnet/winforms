// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Win32.Graphics.Printing;
using Windows.Win32.UI.Controls.Dialogs;

namespace System.Drawing.Printing;

/// <summary>
///  Information about how a document should be printed, including which printer to print it on.
/// </summary>
public unsafe partial class PrinterSettings : ICloneable
{
    private string? _printerName; // default printer.
    private string _driverName = "";
    private ushort _extraBytes;
    private byte[]? _extraInfo;

    private short _copies = -1;
    private Duplex _duplex = Duplex.Default;
    private TriState _collate = TriState.Default;
    private readonly PageSettings _defaultPageSettings;
    private int _fromPage;
    private int _toPage;
    private int _maxPage = 9999;
    private int _minPage;
    private PrintRange _printRange;

    private ushort _devmodeBytes;
    private byte[]? _cachedDevmode;

    /// <summary>
    ///  Initializes a new instance of the <see cref='PrinterSettings'/> class.
    /// </summary>
    public PrinterSettings()
    {
        _defaultPageSettings = new PageSettings(this);
    }

    /// <summary>
    ///  Gets a value indicating whether the printer supports duplex (double-sided) printing.
    /// </summary>
    public bool CanDuplex => DeviceCapabilities(PRINTER_DEVICE_CAPABILITIES.DC_DUPLEX) == 1;

    /// <summary>
    ///  Gets or sets the number of copies to print.
    /// </summary>
    public short Copies
    {
        get => _copies != -1 ? _copies : GetModeField(ModeField.Copies, 1);
        set
        {
            if (value < 0)
            {
                throw new ArgumentException(SR.Format(SR.InvalidLowBoundArgumentEx, nameof(value), value, 0));
            }

            _copies = value;
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the print out is collated.
    /// </summary>
    public bool Collate
    {
        get => _collate.IsDefault
            ? GetModeField(ModeField.Collate, (short)DEVMODE_COLLATE.DMCOLLATE_FALSE) == (short)DEVMODE_COLLATE.DMCOLLATE_TRUE
            : (bool)_collate;
        set => _collate = value;
    }

    /// <summary>
    ///  Gets the default page settings for this printer.
    /// </summary>
    public PageSettings DefaultPageSettings => _defaultPageSettings;

    // As far as I can tell, Windows no longer pays attention to driver names and output ports.
    // But I'm leaving this code in place in case I'm wrong.
    internal string DriverName => _driverName;

    /// <summary>
    ///  Gets or sets the printer's duplex setting.
    /// </summary>
    public Duplex Duplex
    {
        get => _duplex != Duplex.Default ? _duplex : (Duplex)GetModeField(ModeField.Duplex, (short)DEVMODE_DUPLEX.DMDUP_SIMPLEX);
        set
        {
            if (value is < Duplex.Default or > Duplex.Horizontal)
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Duplex));
            }

            _duplex = value;
        }
    }

    /// <summary>
    ///  Gets or sets the first page to print.
    /// </summary>
    public int FromPage
    {
        get => _fromPage;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException(SR.Format(SR.InvalidLowBoundArgumentEx, nameof(value), value, 0));
            }

            _fromPage = value;
        }
    }

    /// <summary>
    ///  Gets the names of all printers installed on the machine.
    /// </summary>
    public static StringCollection InstalledPrinters
    {
        get
        {
            // Note: The call to get the size of the buffer required for level 5 does not work properly on NT platforms.
            const uint Level = 4;

            uint bytesNeeded;
            uint count;

            bool success = PInvoke.EnumPrinters(
                PInvoke.PRINTER_ENUM_LOCAL | PInvoke.PRINTER_ENUM_CONNECTIONS,
                Name: null,
                Level,
                pPrinterEnum: null,
                0,
                &bytesNeeded,
                &count);

            if (!success)
            {
                WIN32_ERROR error = (WIN32_ERROR)Marshal.GetLastPInvokeError();
                if (error != WIN32_ERROR.ERROR_INSUFFICIENT_BUFFER)
                {
                    throw new Win32Exception((int)error);
                }
            }

            using BufferScope<byte> buffer = new((int)bytesNeeded);

            fixed (byte* b = buffer)
            {
                success = PInvoke.EnumPrinters(
                    PInvoke.PRINTER_ENUM_LOCAL | PInvoke.PRINTER_ENUM_CONNECTIONS,
                    Name: null,
                    Level,
                    b,
                    (uint)buffer.Length,
                    &bytesNeeded,
                    &count);

                if (!success)
                {
                    throw new Win32Exception();
                }

                string[] array = new string[count];

                ReadOnlySpan<PRINTER_INFO_4W> info = new(b, (int)count);

                for (int i = 0; i < count; i++)
                {
                    array[i] = new string(info[i].pPrinterName);
                }

                return new StringCollection(array);
            }
        }
    }

    /// <summary>
    ///  Gets a value indicating whether the <see cref='PrinterName'/> property designates the default printer.
    /// </summary>
    public bool IsDefaultPrinter => _printerName is null || _printerName == GetDefaultPrinterName();

    /// <summary>
    ///  Gets a value indicating whether the printer is a plotter, as opposed to a raster printer.
    /// </summary>
    public bool IsPlotter => GetDeviceCaps(GET_DEVICE_CAPS_INDEX.TECHNOLOGY) == PInvoke.DT_PLOTTER;

    /// <summary>
    ///  Gets a value indicating whether the <see cref='PrinterName'/> property designates a valid printer.
    /// </summary>
    public bool IsValid => DeviceCapabilities(PRINTER_DEVICE_CAPABILITIES.DC_COPIES) != -1;

    /// <summary>
    ///  Gets the angle, in degrees, which the portrait orientation is rotated to produce the landscape orientation.
    /// </summary>
    public int LandscapeAngle => DeviceCapabilities(PRINTER_DEVICE_CAPABILITIES.DC_ORIENTATION, defaultValue: 0);

    /// <summary>
    ///  Gets the maximum number of copies allowed by the printer.
    /// </summary>
    public int MaximumCopies => DeviceCapabilities(PRINTER_DEVICE_CAPABILITIES.DC_COPIES, defaultValue: 1);

    /// <summary>
    ///  Gets or sets the highest <see cref='FromPage'/> or <see cref='ToPage'/> which may be selected in a print dialog box.
    /// </summary>
    public int MaximumPage
    {
        get => _maxPage;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException(SR.Format(SR.InvalidLowBoundArgumentEx, nameof(value), value, 0));
            }

            _maxPage = value;
        }
    }

    /// <summary>
    /// Gets or sets the lowest <see cref='FromPage'/> or <see cref='ToPage'/> which may be selected in a print dialog box.
    /// </summary>
    public int MinimumPage
    {
        get => _minPage;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException(SR.Format(SR.InvalidLowBoundArgumentEx, nameof(value), value, 0));
            }

            _minPage = value;
        }
    }

    internal string OutputPort { get; set; } = "";

    /// <summary>
    ///  Indicates the name of the printer file.
    /// </summary>
    public string PrintFileName
    {
        get => OutputPort;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(value);
            }

            OutputPort = value;
        }
    }

    /// <summary>
    ///  Gets the paper sizes supported by this printer.
    /// </summary>
    public PaperSizeCollection PaperSizes => new(Get_PaperSizes());

    /// <summary>
    ///  Gets the paper sources available on this printer.
    /// </summary>
    public PaperSourceCollection PaperSources => new(Get_PaperSources());

    /// <summary>
    ///  Whether the print dialog has been displayed. In SafePrinting mode, a print dialog is required to print.
    ///  After printing, this property is set to false if the program does not have AllPrinting; this guarantees
    ///  a document is only printed once each time the print dialog is shown.
    /// </summary>
    internal bool PrintDialogDisplayed { get; set; }

    /// <summary>
    ///  Gets or sets the pages the user has asked to print.
    /// </summary>
    public PrintRange PrintRange
    {
        get => _printRange;
        set
        {
            if (!Enum.IsDefined(value))
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(PrintRange));
            }

            _printRange = value;
        }
    }

    /// <summary>
    ///  Indicates whether to print to a file instead of a port.
    /// </summary>
    public bool PrintToFile { get; set; }

    /// <summary>
    ///  Gets or sets the name of the printer.
    /// </summary>
    public string PrinterName
    {
        get => PrinterNameInternal;
        set => PrinterNameInternal = value;
    }

    private string PrinterNameInternal
    {
        get => _printerName ?? GetDefaultPrinterName();
        set
        {
            // Reset the DevMode and extra bytes.
            _cachedDevmode = null;
            _extraInfo = null;
            _printerName = value;
        }
    }

    /// <summary>
    ///  Gets the resolutions supported by this printer.
    /// </summary>
    public PrinterResolutionCollection PrinterResolutions => new(Get_PrinterResolutions());

    /// <summary>
    ///  If the image is a JPEG or a PNG (Image.RawFormat) and the printer returns true from
    ///  ExtEscape(CHECKJPEGFORMAT) or ExtEscape(CHECKPNGFORMAT) then this function returns true.
    /// </summary>
    public bool IsDirectPrintingSupported(ImageFormat imageFormat)
    {
        if (!imageFormat.Equals(ImageFormat.Jpeg) && !imageFormat.Equals(ImageFormat.Png))
        {
            return false;
        }

        using var hdc = CreateInformationContext(DefaultPageSettings);
        return IsDirectPrintingSupported(hdc, imageFormat, out _);
    }

    private static bool IsDirectPrintingSupported(HDC hdc, ImageFormat imageFormat, out int escapeFunction)
    {
        Debug.Assert(imageFormat == ImageFormat.Jpeg || imageFormat == ImageFormat.Png);

        escapeFunction = imageFormat.Equals(ImageFormat.Jpeg)
            ? (int)PInvoke.CHECKJPEGFORMAT
            : (int)PInvoke.CHECKPNGFORMAT;

        fixed (int* function = &escapeFunction)
        {
            int result = PInvoke.ExtEscape(
                hdc,
                (int)PInvoke.QUERYESCSUPPORT,
                sizeof(int),
                (PCSTR)(void*)&function,
                0,
                null);

            return result != 0;
        }
    }

    /// <summary>
    ///  This method utilizes the CHECKJPEGFORMAT/CHECKPNGFORMAT printer escape functions
    ///  to determine whether the printer can handle a JPEG image.
    ///
    ///  If the image is a JPEG or a PNG (Image.RawFormat) and the printer returns true
    ///  from ExtEscape(CHECKJPEGFORMAT) or ExtEscape(CHECKPNGFORMAT) then this function returns true.
    /// </summary>
    public bool IsDirectPrintingSupported(Image image)
    {
        ImageFormat imageFormat = image.RawFormat;

        if (!imageFormat.Equals(ImageFormat.Jpeg) && !imageFormat.Equals(ImageFormat.Png))
        {
            return false;
        }

        using var hdc = CreateInformationContext(DefaultPageSettings);

        if (!IsDirectPrintingSupported(hdc, imageFormat, out int escapeFunction))
        {
            return false;
        }

        using MemoryStream stream = new();
        image.Save(stream, image.RawFormat);

        byte[] pvImage = stream.ToArray();

        fixed (byte* b = pvImage)
        {
            uint driverReturnValue;
            int result = PInvoke.ExtEscape(
                hdc,
                escapeFunction,
                pvImage.Length,
                (PCSTR)b,
                sizeof(uint),
                (PSTR)(void*)&driverReturnValue);

            // -1 means some sort of failure
            Debug.Assert(result != -1);
            return result == 1;
        }
    }

    /// <summary>
    ///  Gets a value indicating whether the printer supports color printing.
    /// </summary>
    public bool SupportsColor => DeviceCapabilities(PRINTER_DEVICE_CAPABILITIES.DC_COLORDEVICE) == 1;

    /// <summary>
    ///  Gets or sets the last page to print.
    /// </summary>
    public int ToPage
    {
        get => _toPage;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException(SR.Format(SR.InvalidLowBoundArgumentEx, nameof(value), value, 0));
            }

            _toPage = value;
        }
    }

    /// <summary>
    ///  Creates an identical copy of this object.
    /// </summary>
    public object Clone()
    {
        PrinterSettings clone = (PrinterSettings)MemberwiseClone();
        clone.PrintDialogDisplayed = false;
        return clone;
    }

    internal CreateDcScope CreateDeviceContext(PageSettings pageSettings)
    {
        HGLOBAL modeHandle = GetHdevmodeInternal();

        try
        {
            // Copy the PageSettings to the DEVMODE.
            pageSettings.CopyToHdevmode(modeHandle);
            return CreateDeviceContext(modeHandle);
        }
        finally
        {
            PInvokeCore.GlobalFree(modeHandle);
        }
    }

    internal CreateDcScope CreateDeviceContext(HGLOBAL hdevmode)
    {
        DEVMODEW* devmode = (DEVMODEW*)PInvokeCore.GlobalLock(hdevmode);
        CreateDcScope hdc = new(DriverName, PrinterNameInternal, devmode, informationOnly: false);
        PInvokeCore.GlobalUnlock(hdevmode);
        return hdc;
    }

    // A read-only DC, which is faster than CreateHdc
    internal CreateDcScope CreateInformationContext(PageSettings pageSettings)
    {
        HGLOBAL modeHandle = GetHdevmodeInternal();

        try
        {
            // Copy the PageSettings to the DEVMODE.
            pageSettings.CopyToHdevmode(modeHandle);
            return CreateInformationContext(modeHandle);
        }
        finally
        {
            PInvokeCore.GlobalFree(modeHandle);
        }
    }

    // A read-only DC, which is faster than CreateHdc
    internal unsafe CreateDcScope CreateInformationContext(HGLOBAL hdevmode)
    {
        void* modePointer = PInvokeCore.GlobalLock(hdevmode);
        CreateDcScope dc = new(DriverName, PrinterNameInternal, (DEVMODEW*)modePointer, informationOnly: true);
        PInvokeCore.GlobalUnlock(hdevmode);
        return dc;
    }

    public Graphics CreateMeasurementGraphics() => CreateMeasurementGraphics(DefaultPageSettings);

    // whatever the call stack calling HardMarginX and HardMarginY here is safe
    public Graphics CreateMeasurementGraphics(bool honorOriginAtMargins)
    {
        Graphics g = CreateMeasurementGraphics();
        if (honorOriginAtMargins)
        {
            g.TranslateTransform(-_defaultPageSettings.HardMarginX, -_defaultPageSettings.HardMarginY);
            g.TranslateTransform(_defaultPageSettings.Margins.Left, _defaultPageSettings.Margins.Top);
        }

        return g;
    }

    public Graphics CreateMeasurementGraphics(PageSettings pageSettings)
    {
        // returns the Graphics object for the printer
        var hdc = CreateDeviceContext(pageSettings);
        Graphics g = Graphics.FromHdcInternal(hdc);
        g.PrintingHelper = new HdcHandle(hdc); // Graphics will dispose of the DeviceContext.
        return g;
    }

    // whatever the call stack calling HardMarginX and HardMarginY here is safe
    public Graphics CreateMeasurementGraphics(PageSettings pageSettings, bool honorOriginAtMargins)
    {
        Graphics g = CreateMeasurementGraphics();
        if (honorOriginAtMargins)
        {
            g.TranslateTransform(-pageSettings.HardMarginX, -pageSettings.HardMarginY);
            g.TranslateTransform(pageSettings.Margins.Left, pageSettings.Margins.Top);
        }

        return g;
    }

    // Use FastDeviceCapabilities where possible -- computing PrinterName is quite slow
    private int DeviceCapabilities(PRINTER_DEVICE_CAPABILITIES capability, void* output = null, int defaultValue = -1)
        => FastDeviceCapabilities(capability, PrinterName, output, defaultValue);

    // We pass PrinterName in as a parameter rather than computing it ourselves because it's expensive to compute.
    // We need to pass IntPtr.Zero since passing HDevMode is non-performant.
    private static int FastDeviceCapabilities(PRINTER_DEVICE_CAPABILITIES capability, string printerName, void* output = null, int defaultValue = -1)
    {
        fixed (char* pn = printerName)
        fixed (char* op = GetOutputPort())
        {
            int result = PInvoke.DeviceCapabilities(pn, op, capability, (PWSTR)output, null);
            return result == -1 ? defaultValue : result;
        }
    }

    private static string GetDefaultPrinterName() => GetDefaultName(1);

    private static string GetOutputPort() => GetDefaultName(2);

    private static string GetDefaultName(int slot)
    {
        PRINTDLGEXW dialogSettings = new()
        {
            lStructSize = (uint)sizeof(PRINTDLGEXW),
            Flags = PRINTDLGEX_FLAGS.PD_RETURNDEFAULT | PRINTDLGEX_FLAGS.PD_NOPAGENUMS,
            // PrintDlgEx requires a valid HWND
            hwndOwner = PInvokeCore.GetDesktopWindow(),
            nStartPage = PInvokeCore.START_PAGE_GENERAL
        };

        HRESULT status = PInvokeCore.PrintDlgEx(&dialogSettings);
        if (status.Failed)
        {
            return SR.NoDefaultPrinter;
        }

        HGLOBAL handle = dialogSettings.hDevNames;
        DEVNAMES* names = (DEVNAMES*)PInvokeCore.GlobalLock(handle);
        if (names is null)
        {
            throw new Win32Exception();
        }

        string name = slot switch
        {
            1 => new((char*)names + names->wDeviceOffset),
            2 => new((char*)names + names->wOutputOffset),
            _ => throw new InvalidOperationException()
        };

        PInvokeCore.GlobalUnlock(handle);

        // Windows allocates them, but we have to free them
        PInvokeCore.GlobalFree(dialogSettings.hDevNames);
        PInvokeCore.GlobalFree(dialogSettings.hDevMode);

        return name;
    }

    private int GetDeviceCaps(GET_DEVICE_CAPS_INDEX capability)
    {
        using var hdc = CreateInformationContext(DefaultPageSettings);
        return PInvokeCore.GetDeviceCaps(hdc, capability);
    }

    /// <summary>
    ///  Creates a handle to a DEVMODE structure which correspond too the printer settings.When you are done with the
    ///  handle, you must deallocate it yourself:
    ///    Kernel32.GlobalFree(handle);
    ///    Where "handle" is the return value from this method.
    /// </summary>
    public IntPtr GetHdevmode()
    {
        HGLOBAL modeHandle = GetHdevmodeInternal();
        _defaultPageSettings.CopyToHdevmode(modeHandle);
        return modeHandle;
    }

    internal unsafe HGLOBAL GetHdevmodeInternal()
    {
        // Getting the printer name is quite expensive if PrinterName is left default,
        // because it needs to figure out what the default printer is.
        fixed (char* n = PrinterNameInternal)
        {
            return GetHdevmodeInternal(n);
        }
    }

    private HGLOBAL GetHdevmodeInternal(char* printerName)
    {
        int result = -1;

        // Create DEVMODE
        result = PInvoke.DocumentProperties(
            default,
            default,
            printerName,
            null,
            (DEVMODEW*)null,
            0);

        if (result < 1)
        {
            throw new InvalidPrinterException(this);
        }

        HGLOBAL handle = PInvokeCore.GlobalAlloc(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE, (uint)result);
        DEVMODEW* devmode = (DEVMODEW*)PInvokeCore.GlobalLock(handle);

        // Get the DevMode only if its not cached.
        if (_cachedDevmode is not null)
        {
            Marshal.Copy(_cachedDevmode, 0, (nint)devmode, _devmodeBytes);
        }
        else
        {
            result = PInvoke.DocumentProperties(
                default,
                default,
                printerName,
                devmode,
                (DEVMODEW*)null,
                (uint)DEVMODE_FIELD_FLAGS.DM_OUT_BUFFER);

            if (result < 0)
            {
                throw new Win32Exception();
            }
        }

        if (_extraInfo is not null)
        {
            // Guard against buffer overrun attacks (since design allows client to set a new printer name without
            // updating the devmode)/ by checking for a large enough buffer size before copying the extra info buffer.
            if (_extraBytes <= devmode->dmDriverExtra)
            {
                Marshal.Copy(_extraInfo, 0, (nint)((byte*)devmode + devmode->dmSize), _extraBytes);
            }
        }

        if (devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_COPIES) && _copies != -1)
        {
            devmode->Anonymous1.Anonymous1.dmCopies = _copies;
        }

        if (devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_DUPLEX) && (int)_duplex != -1)
        {
            devmode->dmDuplex = (DEVMODE_DUPLEX)_duplex;
        }

        if (devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_COLLATE) && _collate.IsNotDefault)
        {
            devmode->dmCollate = _collate.IsTrue ? DEVMODE_COLLATE.DMCOLLATE_TRUE : DEVMODE_COLLATE.DMCOLLATE_FALSE;
        }

        result = PInvoke.DocumentProperties(
            default,
            default,
            printerName,
            devmode,
            devmode,
            (uint)(DEVMODE_FIELD_FLAGS.DM_IN_BUFFER | DEVMODE_FIELD_FLAGS.DM_OUT_BUFFER));

        if (result < 0)
        {
            PInvokeCore.GlobalFree(handle);
            PInvokeCore.GlobalUnlock(handle);
            return default;
        }

        PInvokeCore.GlobalUnlock(handle);
        return handle;
    }

    /// <summary>
    ///  Creates a handle to a DEVMODE structure which correspond to the printer and page settings.
    ///  When you are done with the handle, you must deallocate it yourself:
    ///    Kernel32.GlobalFree(handle);
    ///    Where "handle" is the return value from this method.
    /// </summary>
    public IntPtr GetHdevmode(PageSettings pageSettings)
    {
        IntPtr modeHandle = GetHdevmodeInternal();
        pageSettings.CopyToHdevmode(modeHandle);

        return modeHandle;
    }

    /// <summary>
    ///  Creates a handle to a DEVNAMES structure which correspond to the printer settings.
    ///  When you are done with the handle, you must deallocate it yourself:
    ///    Kernel32.GlobalFree(handle);
    ///    Where "handle" is the return value from this method.
    /// </summary>
    public unsafe IntPtr GetHdevnames()
    {
        string printerName = PrinterName;
        string driver = DriverName;
        string outPort = OutputPort;

        // Create DEVNAMES structure, offsets are in characters, not bytes

        // Add 4 for null terminators
        int namesChars = checked(4 + printerName.Length + driver.Length + outPort.Length);
        int offsetInChars = sizeof(DEVNAMES) / sizeof(char);
        int sizeInChars = checked(offsetInChars + namesChars);

        HGLOBAL handle = PInvokeCore.GlobalAlloc(
            GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT,
            (uint)(sizeof(char) * sizeInChars));

        DEVNAMES* devnames = (DEVNAMES*)PInvokeCore.GlobalLock(handle);
        Span<char> names = new((char*)devnames, sizeInChars);

        devnames->wDriverOffset = checked((ushort)offsetInChars);
        driver.AsSpan().CopyTo(names.Slice(offsetInChars, driver.Length));
        offsetInChars += (ushort)(driver.Length + 1);

        devnames->wDeviceOffset = checked((ushort)offsetInChars);
        printerName.AsSpan().CopyTo(names.Slice(offsetInChars, printerName.Length));
        offsetInChars += (ushort)(printerName.Length + 1);

        devnames->wOutputOffset = checked((ushort)offsetInChars);
        outPort.AsSpan().CopyTo(names.Slice(offsetInChars, outPort.Length));
        offsetInChars += (ushort)(outPort.Length + 1);

        devnames->wDefault = checked((ushort)offsetInChars);

        PInvokeCore.GlobalUnlock(handle);
        return handle;
    }

    // Handles creating then disposing a default DEVMODE
    internal short GetModeField(ModeField field, short defaultValue) => GetModeField(field, defaultValue, modeHandle: default);

    internal short GetModeField(ModeField field, short defaultValue, HGLOBAL modeHandle)
    {
        bool ownHandle = false;
        short result;
        try
        {
            if (modeHandle == 0)
            {
                try
                {
                    modeHandle = GetHdevmodeInternal();
                    ownHandle = true;
                }
                catch (InvalidPrinterException)
                {
                    return defaultValue;
                }
            }

            DEVMODEW* devmode = (DEVMODEW*)PInvokeCore.GlobalLock(modeHandle);
            switch (field)
            {
                case ModeField.Orientation:
                    result = devmode->dmOrientation;
                    break;
                case ModeField.PaperSize:
                    result = devmode->dmPaperSize;
                    break;
                case ModeField.PaperLength:
                    result = devmode->dmPaperLength;
                    break;
                case ModeField.PaperWidth:
                    result = devmode->dmPaperWidth;
                    break;
                case ModeField.Copies:
                    result = devmode->dmCopies;
                    break;
                case ModeField.DefaultSource:
                    result = devmode->dmDefaultSource;
                    break;
                case ModeField.PrintQuality:
                    result = devmode->dmPrintQuality;
                    break;
                case ModeField.Color:
                    result = (short)devmode->dmColor;
                    break;
                case ModeField.Duplex:
                    result = (short)devmode->dmDuplex;
                    break;
                case ModeField.YResolution:
                    result = devmode->dmYResolution;
                    break;
                case ModeField.TTOption:
                    result = (short)devmode->dmTTOption;
                    break;
                case ModeField.Collate:
                    result = (short)devmode->dmCollate;
                    break;
                default:
                    Debug.Fail("Invalid field in GetModeField");
                    result = defaultValue;
                    break;
            }

            PInvokeCore.GlobalUnlock(modeHandle);
        }
        finally
        {
            if (ownHandle)
            {
                PInvokeCore.GlobalFree(modeHandle);
            }
        }

        return result;
    }

    internal unsafe PaperSize[] Get_PaperSizes()
    {
        // Cache the name as the name will be computed on every call if the name is default
        string printerName = PrinterName;

        int result = FastDeviceCapabilities(PRINTER_DEVICE_CAPABILITIES.DC_PAPERNAMES, printerName);
        if (result == -1)
        {
            return [];
        }

        int count = result;

        // DC_PAPERNAMES is an array of fixed 64 char buffers
        const int NameLength = 64;
        using BufferScope<char> names = new(NameLength * count);
        fixed (char* n = names)
        {
            result = FastDeviceCapabilities(PRINTER_DEVICE_CAPABILITIES.DC_PAPERNAMES, printerName, n);
        }

        Debug.Assert(
            FastDeviceCapabilities(PRINTER_DEVICE_CAPABILITIES.DC_PAPERS, printerName) == count,
            "Not the same number of paper kinds as paper names?");

        Span<ushort> kinds = stackalloc ushort[count];
        fixed (ushort* k = kinds)
        {
            result = FastDeviceCapabilities(PRINTER_DEVICE_CAPABILITIES.DC_PAPERS, printerName, k);
        }

        Debug.Assert(
            FastDeviceCapabilities(PRINTER_DEVICE_CAPABILITIES.DC_PAPERSIZE, printerName) == count,
            "Not the same number of paper sizes as paper names?");

        Span<Size> sizes = stackalloc Size[count];
        fixed (Size* s = sizes)
        {
            result = FastDeviceCapabilities(PRINTER_DEVICE_CAPABILITIES.DC_PAPERSIZE, printerName, s);
        }

        PaperSize[] paperSizes = new PaperSize[count];
        for (int i = 0; i < count; i++)
        {
            paperSizes[i] = new PaperSize(
                (PaperKind)kinds[i],
                names.Slice(i * NameLength, NameLength).SliceAtFirstNull().ToString(),
                PrinterUnitConvert.Convert(sizes[i].Width, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display),
                PrinterUnitConvert.Convert(sizes[i].Height, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display));
        }

        return paperSizes;
    }

    internal unsafe PaperSource[] Get_PaperSources()
    {
        // Cache the name as the name will be computed on every call if the name is default
        string printerName = PrinterName;

        int result = FastDeviceCapabilities(PRINTER_DEVICE_CAPABILITIES.DC_BINNAMES, printerName);
        if (result == -1)
        {
            return [];
        }

        int count = result;

        // Contrary to documentation, DeviceCapabilities returns char[count, 24],
        // not char[count][24]
        // DC_BINNAMES is an array of fixed 64 char buffers
        const int NameLength = 24;
        using BufferScope<char> names = new(NameLength * count);
        fixed (char* n = names)
        {
            result = FastDeviceCapabilities(PRINTER_DEVICE_CAPABILITIES.DC_BINNAMES, printerName, n);
        }

        Debug.Assert(
            FastDeviceCapabilities(PRINTER_DEVICE_CAPABILITIES.DC_BINS, printerName) == count,
            "Not the same number of bin kinds as bin names?");

        Span<ushort> kinds = stackalloc ushort[count];
        fixed (ushort* k = kinds)
        {
            FastDeviceCapabilities(PRINTER_DEVICE_CAPABILITIES.DC_BINS, printerName, k);
        }

        PaperSource[] paperSources = new PaperSource[count];
        for (int i = 0; i < count; i++)
        {
            paperSources[i] = new PaperSource(
                (PaperSourceKind)kinds[i],
                names.Slice(i * NameLength, NameLength).SliceAtFirstNull().ToString());
        }

        return paperSources;
    }

    internal unsafe PrinterResolution[] Get_PrinterResolutions()
    {
        // Cache the name as the name will be computed on every call if the name is default
        string printerName = PrinterName;
        PrinterResolution[] result;

        int count = FastDeviceCapabilities(PRINTER_DEVICE_CAPABILITIES.DC_ENUMRESOLUTIONS, printerName);
        if (count == -1)
        {
            // Just return the standard values if custom resolutions are absent.
            result =
            [
                new(PrinterResolutionKind.High, -4, -1),
                new(PrinterResolutionKind.Medium, -3, -1),
                new(PrinterResolutionKind.Low, -2, -1),
                new(PrinterResolutionKind.Draft, -1, -1),
            ];

            return result;
        }

        result = new PrinterResolution[count + 4];
        result[0] = new(PrinterResolutionKind.High, -4, -1);
        result[1] = new(PrinterResolutionKind.Medium, -3, -1);
        result[2] = new(PrinterResolutionKind.Low, -2, -1);
        result[3] = new(PrinterResolutionKind.Draft, -1, -1);

        Span<Point> resolutions = stackalloc Point[count];

        fixed (Point* r = resolutions)
        {
            FastDeviceCapabilities(PRINTER_DEVICE_CAPABILITIES.DC_ENUMRESOLUTIONS, printerName, r);
        }

        for (int i = 0; i < count; i++)
        {
            Point resolution = resolutions[i];
            result[i + 4] = new PrinterResolution(PrinterResolutionKind.Custom, resolution.X, resolution.Y);
        }

        return result;
    }

    /// <summary>
    ///  Copies the relevant information out of the handle and into the PrinterSettings.
    /// </summary>
    public void SetHdevmode(IntPtr hdevmode)
    {
        if (hdevmode == 0)
            throw new ArgumentException(SR.Format(SR.InvalidPrinterHandle, hdevmode));

        DEVMODEW* devmode = (DEVMODEW*)PInvokeCore.GlobalLock((HGLOBAL)hdevmode);

        // Copy entire public devmode as a byte array.
        _devmodeBytes = devmode->dmSize;
        if (_devmodeBytes > 0)
        {
            _cachedDevmode = new byte[_devmodeBytes];
            Marshal.Copy((nint)devmode, _cachedDevmode, 0, _devmodeBytes);
        }

        // Copy private devmode as a byte array.
        _extraBytes = devmode->dmDriverExtra;
        if (_extraBytes > 0)
        {
            _extraInfo = new byte[_extraBytes];
            Marshal.Copy((nint)((byte*)devmode + devmode->dmSize), _extraInfo, 0, _extraBytes);
        }

        if (devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_COPIES))
        {
            _copies = devmode->dmCopies;
        }

        if (devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_DUPLEX))
        {
            _duplex = (Duplex)devmode->dmDuplex;
        }

        if (devmode->dmFields.HasFlag(DEVMODE_FIELD_FLAGS.DM_COLLATE))
        {
            _collate = devmode->dmCollate == DEVMODE_COLLATE.DMCOLLATE_TRUE;
        }

        PInvokeCore.GlobalUnlock((HGLOBAL)hdevmode);
    }

    /// <summary>
    ///  Copies the relevant information out of the handle and into the PrinterSettings.
    /// </summary>
    public void SetHdevnames(IntPtr hdevnames)
    {
        if (hdevnames == 0)
        {
            throw new ArgumentException(SR.Format(SR.InvalidPrinterHandle, hdevnames));
        }

        DEVNAMES* names = (DEVNAMES*)PInvokeCore.GlobalLock((HGLOBAL)hdevnames);

        _driverName = new((char*)names + names->wDriverOffset);
        _printerName = new((char*)names + names->wDeviceOffset);
        OutputPort = new((char*)names + names->wOutputOffset);

        PrintDialogDisplayed = true;

        PInvokeCore.GlobalUnlock((HGLOBAL)hdevnames);
    }

    public override string ToString() =>
        $"[PrinterSettings {PrinterName} Copies={Copies} Collate={Collate} Duplex={Duplex} FromPage={FromPage} LandscapeAngle={LandscapeAngle} MaximumCopies={MaximumCopies} OutputPort={OutputPort} ToPage={ToPage}]";
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Imaging;
using System.Drawing.Internal;
using System.IO;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Drawing.Printing;

/// <summary>
///  Information about how a document should be printed, including which printer to print it on.
/// </summary>
public partial class PrinterSettings : ICloneable
{
    // All read/write data is stored in managed code, and whenever we need to call Win32,
    // we create new DEVMODE and DEVNAMES structures.  We don't store device capabilities,
    // though.
    //
    // Also, all properties have hidden tri-state logic -- yes/no/default
    private const int Padding64Bit = 4;

    private string? _printerName; // default printer.
    private string _driverName = "";
    private short _extraBytes;
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

    private short _devmodeBytes;
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
    public bool CanDuplex => DeviceCapabilities(SafeNativeMethods.DC_DUPLEX, IntPtr.Zero, 0) == 1;

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
            ? GetModeField(ModeField.Collate, SafeNativeMethods.DMCOLLATE_FALSE) == SafeNativeMethods.DMCOLLATE_TRUE
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
        get => _duplex != Duplex.Default ? _duplex : (Duplex)GetModeField(ModeField.Duplex, SafeNativeMethods.DMDUP_SIMPLEX);
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
    public static unsafe StringCollection InstalledPrinters
    {
        get
        {
            int sizeOfStruct;

            // Note: The call to get the size of the buffer required for level 5 does not work properly on NT platforms.
            const int Level = 4;

            // PRINTER_INFO_4 is 12 or 24 bytes in size depending on the architecture.
            sizeOfStruct = IntPtr.Size == 8
                ? (IntPtr.Size * 2) + (sizeof(int) * 1) + Padding64Bit
                : (IntPtr.Size * 2) + (sizeof(int) * 1);

            Winspool.EnumPrinters(
                SafeNativeMethods.PRINTER_ENUM_LOCAL | SafeNativeMethods.PRINTER_ENUM_CONNECTIONS,
                null,
                Level,
                IntPtr.Zero,
                0,
                out int bufferSize,
                out _);

            IntPtr buffer = Marshal.AllocCoTaskMem(bufferSize);
            int returnCode = Winspool.EnumPrinters(
                SafeNativeMethods.PRINTER_ENUM_LOCAL | SafeNativeMethods.PRINTER_ENUM_CONNECTIONS,
                null,
                Level,
                buffer,
                bufferSize,
                out _,
                out int count);

            string[] array = new string[count];

            if (returnCode == 0)
            {
                Marshal.FreeCoTaskMem(buffer);
                throw new Win32Exception();
            }

            byte* pBuffer = (byte*)buffer;
            for (int i = 0; i < count; i++)
            {
                // The printer name is at offset 0
                IntPtr namePointer = *(IntPtr*)(pBuffer + (nint)i * sizeOfStruct);
                array[i] = Marshal.PtrToStringAuto(namePointer)!;
            }

            Marshal.FreeCoTaskMem(buffer);

            return new StringCollection(array);
        }
    }

    /// <summary>
    ///  Gets a value indicating whether the <see cref='PrinterName'/> property designates the default printer.
    /// </summary>
    public bool IsDefaultPrinter => _printerName is null || _printerName == GetDefaultPrinterName();

    /// <summary>
    ///  Gets a value indicating whether the printer is a plotter, as opposed to a raster printer.
    /// </summary>
    public bool IsPlotter => GetDeviceCaps(Gdi32.DeviceCapability.TECHNOLOGY) == Gdi32.DeviceTechnology.DT_PLOTTER;

    /// <summary>
    ///  Gets a value indicating whether the <see cref='PrinterName'/> property designates a valid printer.
    /// </summary>
    public bool IsValid => DeviceCapabilities(SafeNativeMethods.DC_COPIES, IntPtr.Zero, -1) != -1;

    /// <summary>
    ///  Gets the angle, in degrees, which the portrait orientation is rotated to produce the landscape orientation.
    /// </summary>
    public int LandscapeAngle => DeviceCapabilities(SafeNativeMethods.DC_ORIENTATION, IntPtr.Zero, 0);

    /// <summary>
    ///  Gets the maximum number of copies allowed by the printer.
    /// </summary>
    public int MaximumCopies => DeviceCapabilities(SafeNativeMethods.DC_COPIES, IntPtr.Zero, 1);

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
    ///  Whether the print dialog has been displayed.  In SafePrinting mode, a print dialog is required to print.
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
                throw new InvalidEnumArgumentException(nameof(value), unchecked((int)value), typeof(PrintRange));
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
        if (imageFormat.Equals(ImageFormat.Jpeg) || imageFormat.Equals(ImageFormat.Png))
        {
            int nEscape = imageFormat.Equals(ImageFormat.Jpeg) ? Gdi32.CHECKJPEGFORMAT : Gdi32.CHECKPNGFORMAT;

            using DeviceContext dc = CreateInformationContext(DefaultPageSettings);
            HandleRef hdc = new(dc, dc.Hdc);
            return Gdi32.ExtEscape(hdc, Gdi32.QUERYESCSUPPORT, sizeof(int), ref nEscape, 0, out int outData) > 0;
        }

        return false;
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
        bool isDirectPrintingSupported = false;
        if (image.RawFormat.Equals(ImageFormat.Jpeg) || image.RawFormat.Equals(ImageFormat.Png))
        {
            using MemoryStream stream = new();
            image.Save(stream, image.RawFormat);

            byte[] pvImage = stream.ToArray();

            int nEscape = image.RawFormat.Equals(ImageFormat.Jpeg) ? Gdi32.CHECKJPEGFORMAT : Gdi32.CHECKPNGFORMAT;

            using DeviceContext dc = CreateInformationContext(DefaultPageSettings);
            HandleRef hdc = new(dc, dc.Hdc);
            bool querySupported = Gdi32.ExtEscape(hdc, Gdi32.QUERYESCSUPPORT, sizeof(int), ref nEscape, 0, out int outData) > 0;
            if (querySupported)
            {
                isDirectPrintingSupported =
                    Gdi32.ExtEscape(hdc, nEscape, pvImage.Length, pvImage, sizeof(int), out outData) > 0
                    && (outData == 1);
            }
        }

        return isDirectPrintingSupported;
    }

    /// <summary>
    ///  Gets a value indicating whether the printer supports color printing.
    /// </summary>
    public bool SupportsColor => DeviceCapabilities(
        capability: SafeNativeMethods.DC_COLORDEVICE,
        pointerToBuffer: IntPtr.Zero,
        defaultValue: 0) == 1;

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

    internal DeviceContext CreateDeviceContext(PageSettings pageSettings)
    {
        IntPtr modeHandle = GetHdevmodeInternal();
        DeviceContext? dc = null;

        try
        {
            // Copy the PageSettings to the DEVMODE.
            pageSettings.CopyToHdevmode(modeHandle);
            dc = CreateDeviceContext(modeHandle);
        }
        finally
        {
            Kernel32.GlobalFree(modeHandle);
        }

        return dc;
    }

    internal DeviceContext CreateDeviceContext(IntPtr hdevmode)
    {
        IntPtr modePointer = Kernel32.GlobalLock(hdevmode);
        DeviceContext dc = DeviceContext.CreateDC(DriverName, PrinterNameInternal, fileName: null, modePointer);
        Kernel32.GlobalUnlock(hdevmode);
        return dc;
    }

    // A read-only DC, which is faster than CreateHdc
    internal DeviceContext CreateInformationContext(PageSettings pageSettings)
    {
        IntPtr modeHandle = GetHdevmodeInternal();
        DeviceContext dc;

        try
        {
            // Copy the PageSettings to the DEVMODE.
            pageSettings.CopyToHdevmode(modeHandle);
            dc = CreateInformationContext(modeHandle);
        }
        finally
        {
            Kernel32.GlobalFree(modeHandle);
        }

        return dc;
    }

    // A read-only DC, which is faster than CreateHdc
    internal DeviceContext CreateInformationContext(IntPtr hdevmode)
    {
        IntPtr modePointer = Kernel32.GlobalLock(hdevmode);
        DeviceContext dc = DeviceContext.CreateIC(DriverName, PrinterNameInternal, fileName: null, modePointer);
        Kernel32.GlobalUnlock(hdevmode);
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
        DeviceContext dc = CreateDeviceContext(pageSettings);
        Graphics g = Graphics.FromHdcInternal(dc.Hdc);
        g.PrintingHelper = dc; // Graphics will dispose of the DeviceContext.
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

    // Create a PRINTDLG with a few useful defaults.
    // Try to keep this consistent with PrintDialog.CreatePRINTDLG.
    private static unsafe void CreatePRINTDLGX86(out Comdlg32.PRINTDLGX86 data)
    {
        data = default;
        data.lStructSize = sizeof(Comdlg32.PRINTDLGX86);
        data.nFromPage = 1;
        data.nToPage = 1;
        data.nMinPage = 0;
        data.nMaxPage = 9999;
        data.nCopies = 1;
    }

    // Create a PRINTDLG with a few useful defaults.
    // Try to keep this consistent with PrintDialog.CreatePRINTDLG.
    private static unsafe void CreatePRINTDLG(out Comdlg32.PRINTDLG data)
    {
        data = default;
        data.lStructSize = sizeof(Comdlg32.PRINTDLG);
        data.nFromPage = 1;
        data.nToPage = 1;
        data.nMinPage = 0;
        data.nMaxPage = 9999;
        data.nCopies = 1;
    }

    // Use FastDeviceCapabilities where possible -- computing PrinterName is quite slow
    private int DeviceCapabilities(short capability, IntPtr pointerToBuffer, int defaultValue)
    {
        string printerName = PrinterName;
        return FastDeviceCapabilities(capability, pointerToBuffer, defaultValue, printerName);
    }

    // We pass PrinterName in as a parameter rather than computing it ourselves because it's expensive to compute.
    // We need to pass IntPtr.Zero since passing HDevMode is non-performant.
    private static int FastDeviceCapabilities(short capability, IntPtr pointerToBuffer, int defaultValue, string printerName)
    {
        int result = Winspool.DeviceCapabilities(
            printerName,
            GetOutputPort(),
            capability,
            pointerToBuffer,
            IntPtr.Zero);

        return result == -1 ? defaultValue : result;
    }

    // Called by get_PrinterName
    private static string GetDefaultPrinterName()
    {
        if (IntPtr.Size == 8)
        {
            CreatePRINTDLG(out Comdlg32.PRINTDLG data);
            data.Flags = SafeNativeMethods.PD_RETURNDEFAULT;
            bool status = Comdlg32.PrintDlg(ref data);

            if (!status)
            {
                return SR.NoDefaultPrinter;
            }

            IntPtr handle = data.hDevNames;
            IntPtr names = Kernel32.GlobalLock(handle);
            if (names == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            string name = ReadOneDEVNAME(names, 1);
            Kernel32.GlobalUnlock(handle);

            // Windows allocates them, but we have to free them
            Kernel32.GlobalFree(data.hDevNames);
            Kernel32.GlobalFree(data.hDevMode);

            return name;
        }
        else
        {
            CreatePRINTDLGX86(out Comdlg32.PRINTDLGX86 data);
            data.Flags = SafeNativeMethods.PD_RETURNDEFAULT;
            bool status = Comdlg32.PrintDlg(ref data);

            if (!status)
            {
                return SR.NoDefaultPrinter;
            }

            IntPtr handle = data.hDevNames;
            IntPtr names = Kernel32.GlobalLock(handle);
            if (names == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            string name = ReadOneDEVNAME(names, 1);
            Kernel32.GlobalUnlock(handle);

            // Windows allocates them, but we have to free them
            Kernel32.GlobalFree(data.hDevNames);
            Kernel32.GlobalFree(data.hDevMode);

            return name;
        }
    }

    // Called by get_OutputPort
    private static string GetOutputPort()
    {
        if (IntPtr.Size == 8)
        {
            CreatePRINTDLG(out Comdlg32.PRINTDLG data);
            data.Flags = SafeNativeMethods.PD_RETURNDEFAULT;
            bool status = Comdlg32.PrintDlg(ref data);
            if (!status)
                return SR.NoDefaultPrinter;

            IntPtr handle = data.hDevNames;
            IntPtr names = Kernel32.GlobalLock(handle);
            if (names == IntPtr.Zero)
                throw new Win32Exception();

            string name = ReadOneDEVNAME(names, 2);

            Kernel32.GlobalUnlock(handle);

            // Windows allocates them, but we have to free them
            Kernel32.GlobalFree(data.hDevNames);
            Kernel32.GlobalFree(data.hDevMode);

            return name;
        }
        else
        {
            CreatePRINTDLGX86(out Comdlg32.PRINTDLGX86 data);
            data.Flags = SafeNativeMethods.PD_RETURNDEFAULT;
            bool status = Comdlg32.PrintDlg(ref data);

            if (!status)
            {
                return SR.NoDefaultPrinter;
            }

            IntPtr handle = data.hDevNames;
            IntPtr names = Kernel32.GlobalLock(handle);
            if (names == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            string name = ReadOneDEVNAME(names, 2);

            Kernel32.GlobalUnlock(handle);

            // Windows allocates them, but we have to free them
            Kernel32.GlobalFree(data.hDevNames);
            Kernel32.GlobalFree(data.hDevMode);

            return name;
        }
    }

    private int GetDeviceCaps(Gdi32.DeviceCapability capability)
    {
        using DeviceContext dc = CreateInformationContext(DefaultPageSettings);
        return Gdi32.GetDeviceCaps(new HandleRef(dc, dc.Hdc), capability);
    }

    /// <summary>
    ///  Creates a handle to a DEVMODE structure which correspond too the printer settings.When you are done with the
    ///  handle, you must deallocate it yourself:
    ///    Kernel32.GlobalFree(handle);
    ///    Where "handle" is the return value from this method.
    /// </summary>
    public IntPtr GetHdevmode()
    {
        IntPtr modeHandle = GetHdevmodeInternal();
        _defaultPageSettings.CopyToHdevmode(modeHandle);
        return modeHandle;
    }

    internal IntPtr GetHdevmodeInternal()
    {
        // getting the printer name is quite expensive if PrinterName is left default,
        // because it needs to figure out what the default printer is
        return GetHdevmodeInternal(PrinterNameInternal);
    }

    private unsafe IntPtr GetHdevmodeInternal(string printer)
    {
        // Create DEVMODE
        int modeSize = Winspool.DocumentProperties(
            NativeMethods.NullHandleRef,
            NativeMethods.NullHandleRef,
            printer,
            IntPtr.Zero,
            NativeMethods.NullHandleRef,
            0);

        if (modeSize < 1)
        {
            throw new InvalidPrinterException(this);
        }

        IntPtr handle = Kernel32.GlobalAlloc(SafeNativeMethods.GMEM_MOVEABLE, (uint)modeSize); // cannot be <0 anyway
        IntPtr pointer = Kernel32.GlobalLock(handle);

        // Get the DevMode only if its not cached.
        if (_cachedDevmode is not null)
        {
            Marshal.Copy(_cachedDevmode, 0, pointer, _devmodeBytes);
        }
        else
        {
            int returnCode = Winspool.DocumentProperties(
                NativeMethods.NullHandleRef,
                NativeMethods.NullHandleRef,
                printer,
                pointer,
                NativeMethods.NullHandleRef,
                SafeNativeMethods.DM_OUT_BUFFER);

            if (returnCode < 0)
            {
                throw new Win32Exception();
            }
        }

        Gdi32.DEVMODE mode = Marshal.PtrToStructure<Gdi32.DEVMODE>(pointer)!;

        if (_extraInfo is not null)
        {
            // Guard against buffer overrun attacks (since design allows client to set a new printer name without
            // updating the devmode)/ by checking for a large enough buffer size before copying the extra info buffer.
            if (_extraBytes <= mode.dmDriverExtra)
            {
                IntPtr pointerOffset = (IntPtr)((byte*)pointer + mode.dmSize);
                Marshal.Copy(_extraInfo, 0, pointerOffset, _extraBytes);
            }
        }

        if ((mode.dmFields & SafeNativeMethods.DM_COPIES) == SafeNativeMethods.DM_COPIES && _copies != -1)
        {
            mode.dmCopies = _copies;
        }

        if ((mode.dmFields & SafeNativeMethods.DM_DUPLEX) == SafeNativeMethods.DM_DUPLEX && unchecked((int)_duplex) != -1)
        {
            mode.dmDuplex = unchecked((short)_duplex);
        }

        if ((mode.dmFields & SafeNativeMethods.DM_COLLATE) == SafeNativeMethods.DM_COLLATE && _collate.IsNotDefault)
        {
            mode.dmCollate = (short)(((bool)_collate) ? SafeNativeMethods.DMCOLLATE_TRUE : SafeNativeMethods.DMCOLLATE_FALSE);
        }

        Marshal.StructureToPtr(mode, pointer, false);

        int retCode = Winspool.DocumentProperties(
            NativeMethods.NullHandleRef,
            NativeMethods.NullHandleRef,
            printer,
            pointer,
            pointer,
            SafeNativeMethods.DM_IN_BUFFER | SafeNativeMethods.DM_OUT_BUFFER);

        if (retCode < 0)
        {
            Kernel32.GlobalFree(handle);
            Kernel32.GlobalUnlock(handle);
            return IntPtr.Zero;
        }

        Kernel32.GlobalUnlock(handle);
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
        string printerName = PrinterName; // the PrinterName property is slow when using the default printer
        string driver = DriverName;  // make sure we are writing out exactly the same string as we got the length of
        string outPort = OutputPort;

        // Create DEVNAMES structure
        // +4 for null terminator
        int namesCharacters = checked(4 + printerName.Length + driver.Length + outPort.Length);

        // 8 = size of fixed portion of DEVNAMES
        short offset = (short)(8 / Marshal.SystemDefaultCharSize); // Offsets are in characters, not bytes
        uint namesSize = (uint)checked(Marshal.SystemDefaultCharSize * (offset + namesCharacters)); // always >0
        IntPtr handle = Kernel32.GlobalAlloc(SafeNativeMethods.GMEM_MOVEABLE | SafeNativeMethods.GMEM_ZEROINIT, namesSize);
        IntPtr namesPointer = Kernel32.GlobalLock(handle);
        byte* pNamesPointer = (byte*)namesPointer;

        *(short*)(pNamesPointer) = offset; // wDriverOffset
        offset += WriteOneDEVNAME(driver, namesPointer, offset);
        *(short*)(pNamesPointer + 2) = offset; // wDeviceOffset
        offset += WriteOneDEVNAME(printerName, namesPointer, offset);
        *(short*)(pNamesPointer + 4) = offset; // wOutputOffset
        offset += WriteOneDEVNAME(outPort, namesPointer, offset);
        *(short*)(pNamesPointer + 6) = offset; // wDefault

        Kernel32.GlobalUnlock(handle);
        return handle;
    }

    // Handles creating then disposing a default DEVMODE
    internal short GetModeField(ModeField field, short defaultValue) => GetModeField(field, defaultValue, IntPtr.Zero);

    internal short GetModeField(ModeField field, short defaultValue, IntPtr modeHandle)
    {
        bool ownHandle = false;
        short result;
        try
        {
            if (modeHandle == IntPtr.Zero)
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

            IntPtr modePointer = Kernel32.GlobalLock(new HandleRef(this, modeHandle));
            Gdi32.DEVMODE mode = Marshal.PtrToStructure<Gdi32.DEVMODE>(modePointer)!;
            switch (field)
            {
                case ModeField.Orientation:
                    result = mode.dmOrientation;
                    break;
                case ModeField.PaperSize:
                    result = mode.dmPaperSize;
                    break;
                case ModeField.PaperLength:
                    result = mode.dmPaperLength;
                    break;
                case ModeField.PaperWidth:
                    result = mode.dmPaperWidth;
                    break;
                case ModeField.Copies:
                    result = mode.dmCopies;
                    break;
                case ModeField.DefaultSource:
                    result = mode.dmDefaultSource;
                    break;
                case ModeField.PrintQuality:
                    result = mode.dmPrintQuality;
                    break;
                case ModeField.Color:
                    result = mode.dmColor;
                    break;
                case ModeField.Duplex:
                    result = mode.dmDuplex;
                    break;
                case ModeField.YResolution:
                    result = mode.dmYResolution;
                    break;
                case ModeField.TTOption:
                    result = mode.dmTTOption;
                    break;
                case ModeField.Collate:
                    result = mode.dmCollate;
                    break;
                default:
                    Debug.Fail("Invalid field in GetModeField");
                    result = defaultValue;
                    break;
            }

            Kernel32.GlobalUnlock(new HandleRef(this, modeHandle));
        }
        finally
        {
            if (ownHandle)
            {
                Kernel32.GlobalFree(new HandleRef(this, modeHandle));
            }
        }

        return result;
    }

    internal unsafe PaperSize[] Get_PaperSizes()
    {
        string printerName = PrinterName; // this is quite expensive if PrinterName is left default

        int count = FastDeviceCapabilities(SafeNativeMethods.DC_PAPERNAMES, IntPtr.Zero, -1, printerName);
        if (count == -1)
        {
            return [];
        }

        int stringSize = Marshal.SystemDefaultCharSize * 64;
        IntPtr namesBuffer = Marshal.AllocCoTaskMem(checked(stringSize * count));
        FastDeviceCapabilities(SafeNativeMethods.DC_PAPERNAMES, namesBuffer, -1, printerName);

        Debug.Assert(
            FastDeviceCapabilities(SafeNativeMethods.DC_PAPERS, IntPtr.Zero, -1, printerName) == count,
            "Not the same number of paper kinds as paper names?");

        IntPtr kindsBuffer = Marshal.AllocCoTaskMem(2 * count);
        FastDeviceCapabilities(SafeNativeMethods.DC_PAPERS, kindsBuffer, -1, printerName);

        Debug.Assert(
            FastDeviceCapabilities(SafeNativeMethods.DC_PAPERSIZE, IntPtr.Zero, -1, printerName) == count,
            "Not the same number of paper kinds as paper names?");

        IntPtr dimensionsBuffer = Marshal.AllocCoTaskMem(8 * count);
        FastDeviceCapabilities(SafeNativeMethods.DC_PAPERSIZE, dimensionsBuffer, -1, printerName);

        PaperSize[] result = new PaperSize[count];
        byte* pNamesBuffer = (byte*)namesBuffer;
        short* pKindsBuffer = (short*)kindsBuffer;
        int* pDimensionsBuffer = (int*)dimensionsBuffer;
        for (int i = 0; i < count; i++)
        {
            string name = Marshal.PtrToStringAuto((nint)(pNamesBuffer + stringSize * (nint)i), 64)!;
            int index = name.IndexOf('\0');
            if (index > -1)
            {
                name = name[..index];
            }

            short kind = pKindsBuffer[i];
            int width = pDimensionsBuffer[i * 2];
            int height = pDimensionsBuffer[i * 2 + 1];
            result[i] = new PaperSize(
                (PaperKind)kind,
                name,
                PrinterUnitConvert.Convert(width, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display),
                PrinterUnitConvert.Convert(height, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display));
        }

        Marshal.FreeCoTaskMem(namesBuffer);
        Marshal.FreeCoTaskMem(kindsBuffer);
        Marshal.FreeCoTaskMem(dimensionsBuffer);
        return result;
    }

    internal unsafe PaperSource[] Get_PaperSources()
    {
        string printerName = PrinterName; // this is quite expensive if PrinterName is left default

        int count = FastDeviceCapabilities(SafeNativeMethods.DC_BINNAMES, IntPtr.Zero, -1, printerName);
        if (count == -1)
        {
            return [];
        }

        // Contrary to documentation, DeviceCapabilities returns char[count, 24],
        // not char[count][24]
        int stringSize = Marshal.SystemDefaultCharSize * 24;
        IntPtr namesBuffer = Marshal.AllocCoTaskMem(checked(stringSize * count));
        FastDeviceCapabilities(SafeNativeMethods.DC_BINNAMES, namesBuffer, -1, printerName);

        Debug.Assert(
            FastDeviceCapabilities(SafeNativeMethods.DC_BINS, IntPtr.Zero, -1, printerName) == count,
            "Not the same number of bin kinds as bin names?");

        IntPtr kindsBuffer = Marshal.AllocCoTaskMem(2 * count);
        FastDeviceCapabilities(SafeNativeMethods.DC_BINS, kindsBuffer, -1, printerName);

        byte* pNamesBuffer = (byte*)namesBuffer;
        short* pKindsBuffer = (short*)kindsBuffer;
        PaperSource[] result = new PaperSource[count];
        for (int i = 0; i < count; i++)
        {
            string name = Marshal.PtrToStringAuto((nint)(pNamesBuffer + stringSize * (nint)i), 24)!;
            int index = name.IndexOf('\0');
            if (index > -1)
            {
                name = name[..index];
            }

            short kind = pKindsBuffer[i];
            result[i] = new PaperSource((PaperSourceKind)kind, name);
        }

        Marshal.FreeCoTaskMem(namesBuffer);
        Marshal.FreeCoTaskMem(kindsBuffer);
        return result;
    }

    internal unsafe PrinterResolution[] Get_PrinterResolutions()
    {
        string printerName = PrinterName; // this is quite expensive if PrinterName is left default
        PrinterResolution[] result;

        int count = FastDeviceCapabilities(SafeNativeMethods.DC_ENUMRESOLUTIONS, IntPtr.Zero, -1, printerName);
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

        IntPtr buffer = Marshal.AllocCoTaskMem(checked(8 * count));
        FastDeviceCapabilities(SafeNativeMethods.DC_ENUMRESOLUTIONS, buffer, -1, printerName);

        byte* pBuffer = (byte*)buffer;
        for (int i = 0; i < count; i++)
        {
            int x = *(int*)(pBuffer + i * 8);
            int y = *(int*)(pBuffer + i * 8 + 4);
            result[i + 4] = new PrinterResolution(PrinterResolutionKind.Custom, x, y);
        }

        Marshal.FreeCoTaskMem(buffer);
        return result;
    }

    // names is pointer to DEVNAMES
    private static unsafe string ReadOneDEVNAME(IntPtr pDevnames, int slot)
    {
        byte* bDevNames = (byte*)pDevnames;
        int offset = Marshal.SystemDefaultCharSize * ((ushort*)bDevNames)[slot];
        string result = Marshal.PtrToStringAuto((nint)(bDevNames + offset))!;
        return result;
    }

    /// <summary>
    ///  Copies the relevant information out of the handle and into the PrinterSettings.
    /// </summary>
    public unsafe void SetHdevmode(IntPtr hdevmode)
    {
        if (hdevmode == IntPtr.Zero)
            throw new ArgumentException(SR.Format(SR.InvalidPrinterHandle, hdevmode));

        IntPtr pointer = Kernel32.GlobalLock(hdevmode);
        Gdi32.DEVMODE mode = Marshal.PtrToStructure<Gdi32.DEVMODE>(pointer)!;

        // Copy entire public devmode as a byte array...
        _devmodeBytes = mode.dmSize;
        if (_devmodeBytes > 0)
        {
            _cachedDevmode = new byte[_devmodeBytes];
            Marshal.Copy(pointer, _cachedDevmode, 0, _devmodeBytes);
        }

        // Copy private devmode as a byte array..
        _extraBytes = mode.dmDriverExtra;
        if (_extraBytes > 0)
        {
            _extraInfo = new byte[_extraBytes];
            Marshal.Copy((nint)((byte*)pointer + mode.dmSize), _extraInfo, 0, _extraBytes);
        }

        if ((mode.dmFields & SafeNativeMethods.DM_COPIES) == SafeNativeMethods.DM_COPIES)
        {
            _copies = mode.dmCopies;
        }

        if ((mode.dmFields & SafeNativeMethods.DM_DUPLEX) == SafeNativeMethods.DM_DUPLEX)
        {
            _duplex = (Duplex)mode.dmDuplex;
        }

        if ((mode.dmFields & SafeNativeMethods.DM_COLLATE) == SafeNativeMethods.DM_COLLATE)
        {
            _collate = (mode.dmCollate == SafeNativeMethods.DMCOLLATE_TRUE);
        }

        Kernel32.GlobalUnlock(hdevmode);
    }

    /// <summary>
    /// Copies the relevant information out of the handle and into the PrinterSettings.
    /// </summary>
    public void SetHdevnames(IntPtr hdevnames)
    {
        if (hdevnames == IntPtr.Zero)
        {
            throw new ArgumentException(SR.Format(SR.InvalidPrinterHandle, hdevnames));
        }

        IntPtr namesPointer = Kernel32.GlobalLock(hdevnames);

        _driverName = ReadOneDEVNAME(namesPointer, 0);
        _printerName = ReadOneDEVNAME(namesPointer, 1);
        OutputPort = ReadOneDEVNAME(namesPointer, 2);

        PrintDialogDisplayed = true;

        Kernel32.GlobalUnlock(hdevnames);
    }

    public override string ToString() =>
        $"[PrinterSettings {PrinterName} Copies={Copies} Collate={Collate} Duplex={Duplex} FromPage={FromPage} LandscapeAngle={LandscapeAngle} MaximumCopies={MaximumCopies} OutputPort={OutputPort} ToPage={ToPage}]";

    // Write null terminated string, return length of string in characters (including null)
    private static unsafe short WriteOneDEVNAME(string str, IntPtr bufferStart, int index)
    {
        str ??= "";
        byte* address = (byte*)bufferStart + (nint)index * Marshal.SystemDefaultCharSize;

        char[] data = str.ToCharArray();
        Marshal.Copy(data, 0, (nint)address, data.Length);
        *(short*)(address + data.Length * 2) = 0;

        return checked((short)(str.Length + 1));
    }
}

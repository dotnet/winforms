// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Internal;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Drawing.Printing;

/// <summary>
///  Specifies a print controller that sends information to a printer.
/// </summary>
public class StandardPrintController : PrintController
{
    private DeviceContext? _dc;
    private Graphics? _graphics;

    /// <summary>
    ///  Implements StartPrint for printing to a physical printer.
    /// </summary>
    public override void OnStartPrint(PrintDocument document, PrintEventArgs e)
    {
        Debug.Assert(_dc is null && _graphics is null, "PrintController methods called in the wrong order?");

        base.OnStartPrint(document, e);

        if (!document.PrinterSettings.IsValid)
            throw new InvalidPrinterException(document.PrinterSettings);

        Debug.Assert(_modeHandle is not null, "_modeHandle should have been set by PrintController.OnStartPrint");
        _dc = document.PrinterSettings.CreateDeviceContext(_modeHandle);

        Gdi32.DOCINFO info = new()
        {
            lpszDocName = document.DocumentName,
            lpszDatatype = null,
            fwType = 0,
            // Print to file is "FILE:"
            lpszOutput = document.PrinterSettings.PrintToFile ? document.PrinterSettings.OutputPort : null
        };

        int result = Gdi32.StartDoc(new HandleRef(_dc, _dc.Hdc), info);
        if (result <= 0)
        {
            int error = Marshal.GetLastPInvokeError();
            e.Cancel = error == SafeNativeMethods.ERROR_CANCELLED ? true : throw new Win32Exception(error);
        }
    }

    /// <summary>
    ///  Implements StartPage for printing to a physical printer.
    /// </summary>
    public override Graphics OnStartPage(PrintDocument document, PrintPageEventArgs e)
    {
        Debug.Assert(_dc is not null && _graphics is null, "PrintController methods called in the wrong order?");
        Debug.Assert(_modeHandle is not null);

        base.OnStartPage(document, e);
        e.PageSettings.CopyToHdevmode(_modeHandle);
        IntPtr modePointer = Kernel32.GlobalLock(new HandleRef(this, _modeHandle));
        try
        {
            IntPtr result = Gdi32.ResetDC(new HandleRef(_dc, _dc.Hdc), new HandleRef(null, modePointer));
            Debug.Assert(result == _dc.Hdc, "ResetDC didn't return the same handle I gave it");
        }
        finally
        {
            Kernel32.GlobalUnlock(new HandleRef(this, _modeHandle));
        }

        _graphics = Graphics.FromHdcInternal(_dc.Hdc);

        if (document.OriginAtMargins)
        {
            // Adjust the origin of the graphics object to be at the user-specified margin location.
            int dpiX = Gdi32.GetDeviceCaps(new HandleRef(_dc, _dc.Hdc), Gdi32.DeviceCapability.LOGPIXELSX);
            int dpiY = Gdi32.GetDeviceCaps(new HandleRef(_dc, _dc.Hdc), Gdi32.DeviceCapability.LOGPIXELSY);
            int hardMarginX_DU = Gdi32.GetDeviceCaps(new HandleRef(_dc, _dc.Hdc), Gdi32.DeviceCapability.PHYSICALOFFSETX);
            int hardMarginY_DU = Gdi32.GetDeviceCaps(new HandleRef(_dc, _dc.Hdc), Gdi32.DeviceCapability.PHYSICALOFFSETY);
            float hardMarginX = hardMarginX_DU * 100 / dpiX;
            float hardMarginY = hardMarginY_DU * 100 / dpiY;

            _graphics.TranslateTransform(-hardMarginX, -hardMarginY);
            _graphics.TranslateTransform(document.DefaultPageSettings.Margins.Left, document.DefaultPageSettings.Margins.Top);
        }

        int result2 = Gdi32.StartPage(new HandleRef(_dc, _dc.Hdc));
        return result2 <= 0 ? throw new Win32Exception() : _graphics;
    }

    /// <summary>
    ///  Implements EndPage for printing to a physical printer.
    /// </summary>
    public override void OnEndPage(PrintDocument document, PrintPageEventArgs e)
    {
        Debug.Assert(_dc is not null && _graphics is not null, "PrintController methods called in the wrong order?");

        try
        {
            int result = Gdi32.EndPage(new HandleRef(_dc, _dc.Hdc));
            if (result <= 0)
            {
                throw new Win32Exception();
            }
        }
        finally
        {
            _graphics.Dispose(); // Dispose of GDI+ Graphics; keep the DC
            _graphics = null;
        }

        base.OnEndPage(document, e);
    }

    /// <summary>
    ///  Implements EndPrint for printing to a physical printer.
    /// </summary>
    public override void OnEndPrint(PrintDocument document, PrintEventArgs e)
    {
        Debug.Assert(_dc is not null && _graphics is null, "PrintController methods called in the wrong order?");

        if (_dc is not null)
        {
            try
            {
                int result = (e.Cancel) ? Gdi32.AbortDoc(new HandleRef(_dc, _dc.Hdc)) : Gdi32.EndDoc(new HandleRef(_dc, _dc.Hdc));
                if (result <= 0)
                    throw new Win32Exception();
            }
            finally
            {
                _dc.Dispose();
                _dc = null;
            }
        }

        base.OnEndPrint(document, e);
    }
}

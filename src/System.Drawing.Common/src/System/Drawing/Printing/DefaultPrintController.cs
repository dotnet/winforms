// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Drawing.Printing;

/// <summary>
///  Specifies a print controller that sends information to a printer.
/// </summary>
public unsafe class StandardPrintController : PrintController, IHandle<HDC>
{
    private HdcHandle? _hdc;
    private Graphics? _graphics;

    HDC IHandle<HDC>.Handle => _hdc?.Handle ?? HDC.Null;

    /// <summary>
    ///  Implements StartPrint for printing to a physical printer.
    /// </summary>
    public override void OnStartPrint(PrintDocument document, PrintEventArgs e)
    {
        Debug.Assert(_hdc is null && _graphics is null, "PrintController methods called in the wrong order?");

        base.OnStartPrint(document, e);

        if (!document.PrinterSettings.IsValid)
            throw new InvalidPrinterException(document.PrinterSettings);

        Debug.Assert(_modeHandle is not null, "_modeHandle should have been set by PrintController.OnStartPrint");
        _hdc = new(document.PrinterSettings.CreateDeviceContext(_modeHandle));

        fixed (char* documentName = document.DocumentName)
        fixed (char* outputPort = document.PrinterSettings.OutputPort)
        {
            DOCINFOW info = new()
            {
                cbSize = sizeof(DOCINFOW),
                lpszDocName = documentName,
                lpszDatatype = null,
                fwType = 0,

                // Print to file is "FILE:"
                lpszOutput = document.PrinterSettings.PrintToFile ? outputPort : null
            };

            int result = PInvoke.StartDoc(this, info);
            if (result <= 0)
            {
                WIN32_ERROR error = (WIN32_ERROR)Marshal.GetLastPInvokeError();
                if (error == WIN32_ERROR.NO_ERROR)
                {
                    // StartDoc isn't documented as sets last error, but it does. Need updated SDK metadata.
                    // Can't use CsWin32 to generate the PInvoke as it won't do it as you normally should use
                    // Marshal.GetLastPInvokeError().
                    error = GetLastError();
                }

                e.Cancel = error == WIN32_ERROR.ERROR_CANCELLED ? true : throw new Win32Exception((int)error);
            }
        }
    }

    [DllImport(Libraries.Kernel32, ExactSpelling = true)]
    private static extern WIN32_ERROR GetLastError();

    /// <summary>
    ///  Implements StartPage for printing to a physical printer.
    /// </summary>
    public override Graphics OnStartPage(PrintDocument document, PrintPageEventArgs e)
    {
        Debug.Assert(_hdc is not null && _graphics is null, "PrintController methods called in the wrong order?");
        Debug.Assert(_modeHandle is not null);

        base.OnStartPage(document, e);
        e.PageSettings.CopyToHdevmode(_modeHandle);
        DEVMODEW* devmode = (DEVMODEW*)PInvokeCore.GlobalLock(_modeHandle);
        try
        {
            HDC result = PInvoke.ResetDCW(_hdc, devmode);
            Debug.Assert(result == _hdc, "ResetDC didn't return the same handle");
        }
        finally
        {
            PInvokeCore.GlobalUnlock(_modeHandle);
        }

        _graphics = Graphics.FromHdcInternal(_hdc);

        if (document.OriginAtMargins)
        {
            // Adjust the origin of the graphics object to be at the user-specified margin location.
            int dpiX = PInvokeCore.GetDeviceCaps(_hdc, GET_DEVICE_CAPS_INDEX.LOGPIXELSX);
            int dpiY = PInvokeCore.GetDeviceCaps(_hdc, GET_DEVICE_CAPS_INDEX.LOGPIXELSY);
            int hardMarginX_DU = PInvokeCore.GetDeviceCaps(_hdc, GET_DEVICE_CAPS_INDEX.PHYSICALOFFSETX);
            int hardMarginY_DU = PInvokeCore.GetDeviceCaps(_hdc, GET_DEVICE_CAPS_INDEX.PHYSICALOFFSETY);
            float hardMarginX = hardMarginX_DU * 100 / dpiX;
            float hardMarginY = hardMarginY_DU * 100 / dpiY;

            _graphics.TranslateTransform(-hardMarginX, -hardMarginY);
            _graphics.TranslateTransform(document.DefaultPageSettings.Margins.Left, document.DefaultPageSettings.Margins.Top);
        }

        int result2 = PInvoke.StartPage(this);
        return result2 <= 0 ? throw new Win32Exception() : _graphics;
    }

    /// <summary>
    ///  Implements EndPage for printing to a physical printer.
    /// </summary>
    public override void OnEndPage(PrintDocument document, PrintPageEventArgs e)
    {
        Debug.Assert(_hdc is not null && _graphics is not null, "PrintController methods called in the wrong order?");

        try
        {
            int result = PInvoke.EndPage(this);
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
        Debug.Assert(_hdc is not null && _graphics is null, "PrintController methods called in the wrong order?");

        if (_hdc is not null)
        {
            try
            {
                int result = e.Cancel ? PInvoke.AbortDoc(this) : PInvoke.EndDoc(this);
                if (result <= 0)
                    throw new Win32Exception();
            }
            finally
            {
                _hdc.Dispose();
                _hdc = null;
            }
        }

        base.OnEndPrint(document, e);
    }
}

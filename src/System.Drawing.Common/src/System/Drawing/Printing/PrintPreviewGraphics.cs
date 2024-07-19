// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Printing;

namespace System.Drawing;

/// <summary>
///  Retrieves the printer graphics during preview.
/// </summary>
internal sealed class PrintPreviewGraphics
{
    private readonly PrintPageEventArgs _printPageEventArgs;
    private readonly PrintDocument _printDocument;

    public PrintPreviewGraphics(PrintDocument document, PrintPageEventArgs e)
    {
        _printPageEventArgs = e;
        _printDocument = document;
    }

    /// <summary>
    ///  Gets the Visible bounds of this graphics object. Used during print preview.
    /// </summary>
    public RectangleF VisibleClipBounds
    {
        get
        {
            HGLOBAL hdevmode = _printPageEventArgs.PageSettings.PrinterSettings.GetHdevmodeInternal();

            using var hdc = _printPageEventArgs.PageSettings.PrinterSettings.CreateDeviceContext(hdevmode);
            using Graphics graphics = Graphics.FromHdcInternal(hdc);

            if (_printDocument.OriginAtMargins)
            {
                // Adjust the origin of the graphics object to be at the user-specified margin location
                // Note: Graphics.FromHdc internally calls SaveDC(hdc), we can still use the saved hdc to get the resolution.
                int dpiX = PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.LOGPIXELSX);
                int dpiY = PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.LOGPIXELSY);
                int hardMarginX_DU = PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.PHYSICALOFFSETX);
                int hardMarginY_DU = PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.PHYSICALOFFSETY);
                float hardMarginX = hardMarginX_DU * 100 / dpiX;
                float hardMarginY = hardMarginY_DU * 100 / dpiY;

                graphics.TranslateTransform(-hardMarginX, -hardMarginY);
                graphics.TranslateTransform(_printDocument.DefaultPageSettings.Margins.Left, _printDocument.DefaultPageSettings.Margins.Top);
            }

            return graphics.VisibleClipBounds;
        }
    }
}

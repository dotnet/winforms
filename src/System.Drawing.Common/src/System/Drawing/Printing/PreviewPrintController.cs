// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Imaging;

namespace System.Drawing.Printing;

/// <summary>
///  A PrintController which "prints" to a series of images.
/// </summary>
public class PreviewPrintController : PrintController
{
    private Graphics? _graphics;
    private HdcHandle? _hdc;
    private readonly List<PreviewPageInfo> _list = [];

    public override bool IsPreview => true;

    public virtual bool UseAntiAlias { get; set; }

    public PreviewPageInfo[] GetPreviewPageInfo() => [.. _list];

    /// <summary>
    ///  Implements StartPrint for generating print preview information.
    /// </summary>
    public override void OnStartPrint(PrintDocument document, PrintEventArgs e)
    {
        base.OnStartPrint(document, e);

        if (!document.PrinterSettings.IsValid)
        {
            throw new InvalidPrinterException(document.PrinterSettings);
        }

        // We need a DC as a reference; we don't actually draw on it.
        // We make sure to reuse the same one to improve performance.
        _hdc = new(document.PrinterSettings.CreateInformationContext(_modeHandle ?? HGLOBAL.Null));
    }

    /// <summary>
    ///  Implements StartEnd for generating print preview information.
    /// </summary>
    public override Graphics OnStartPage(PrintDocument document, PrintPageEventArgs e)
    {
        base.OnStartPage(document, e);

        if (e.CopySettingsToDevMode)
        {
            e.PageSettings.CopyToHdevmode(_modeHandle!);
        }

        Size size = e.PageBounds.Size;

        // Metafile framing rectangles apparently use hundredths of mm as their unit of measurement,
        // instead of the GDI+ standard hundredth of an inch.
        Size metafileSize = PrinterUnitConvert.Convert(size, PrinterUnit.Display, PrinterUnit.HundredthsOfAMillimeter);

        HDC hdc = _hdc ?? HDC.Null;

        // Create a Metafile which accepts only GDI+ commands since we are the ones creating and using this.
        // Framework creates a dual-mode EMF for each page in the preview. When these images are displayed in preview,
        // they are added to the dual-mode EMF. However, GDI+ breaks during this process if the image
        // is sufficiently large and has more than 254 colors. This code path can easily be avoided by requesting
        // an EmfPlusOnly EMF.
        Metafile metafile = new(
            hdc,
            new Rectangle(0, 0, metafileSize.Width, metafileSize.Height),
            Imaging.MetafileFrameUnit.GdiCompatible,
            Imaging.EmfType.EmfPlusOnly);

        PreviewPageInfo info = new(metafile, size);
        _list.Add(info);
        PrintPreviewGraphics printGraphics = new(document, e);
        _graphics = Graphics.FromImage(metafile);

        if (document.OriginAtMargins)
        {
            // Adjust the origin of the graphics object to be at the
            // user-specified margin location
            int dpiX = PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.LOGPIXELSX);
            int dpiY = PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.LOGPIXELSY);
            int hardMarginX_DU = PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.PHYSICALOFFSETX);
            int hardMarginY_DU = PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.PHYSICALOFFSETY);
            float hardMarginX = hardMarginX_DU * 100f / dpiX;
            float hardMarginY = hardMarginY_DU * 100f / dpiY;

            _graphics.TranslateTransform(-hardMarginX, -hardMarginY);
            _graphics.TranslateTransform(document.DefaultPageSettings.Margins.Left, document.DefaultPageSettings.Margins.Top);
        }

        _graphics.PrintingHelper = printGraphics;

        if (UseAntiAlias)
        {
            _graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            _graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias;
        }

        return _graphics;
    }

    public override void OnEndPage(PrintDocument document, PrintPageEventArgs e)
    {
        _graphics?.Dispose();
        _graphics = null;
        base.OnEndPage(document, e);
    }

    public override void OnEndPrint(PrintDocument document, PrintEventArgs e)
    {
        _hdc?.Dispose();
        _hdc = null;
        base.OnEndPrint(document, e);
    }
}

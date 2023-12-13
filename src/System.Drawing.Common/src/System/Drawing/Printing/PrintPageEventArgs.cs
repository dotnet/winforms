// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Printing;

/// <summary>
///  Provides data for the <see cref='PrintDocument.PrintPage'/> event.
/// </summary>
public class PrintPageEventArgs : EventArgs
{
    /// <summary>
    ///  Initializes a new instance of the <see cref='PrintPageEventArgs'/> class.
    /// </summary>
    public PrintPageEventArgs(Graphics? graphics, Rectangle marginBounds, Rectangle pageBounds, PageSettings pageSettings)
    {
        Graphics = graphics; // may be null, see PrintController
        MarginBounds = marginBounds;
        PageBounds = pageBounds;
        PageSettings = pageSettings;
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the print job should be canceled.
    /// </summary>
    public bool Cancel { get; set; }

    /// <summary>
    ///  Gets the <see cref='Drawing.Graphics'/> used to paint the item.
    /// </summary>
    public Graphics? Graphics { get; private set; }

    /// <summary>
    ///  Gets or sets a value indicating whether an additional page should be printed.
    /// </summary>
    public bool HasMorePages { get; set; }

    /// <summary>
    ///  Gets the rectangular area that represents the portion of the page between the margins.
    /// </summary>
    public Rectangle MarginBounds { get; }

    /// <summary>
    ///  Gets the rectangular area that represents the total area of the page.
    /// </summary>
    public Rectangle PageBounds { get; }

    /// <summary>
    ///  Gets the page settings for the current page.
    /// </summary>
    public PageSettings PageSettings { get; }

    /// <summary>
    ///  Apply page settings to the printer.
    /// </summary>
    internal bool CopySettingsToDevMode { get; set; } = true;

    /// <summary>
    ///  Disposes of the resources (other than memory) used by the <see cref='PrintPageEventArgs'/>.
    /// </summary>
    internal void Dispose() => Graphics?.Dispose();

    internal void SetGraphics(Graphics? value)
    {
        Debug.Assert(Graphics is null);
        Graphics = value;
    }
}

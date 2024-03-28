// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Drawing.Printing;

/// <summary>
///  Retrieves the resolution supported by a printer.
/// </summary>
public partial class PrinterResolution
{
    private PrinterResolutionKind _kind;

    /// <summary>
    ///  Initializes a new instance of the <see cref='PrinterResolution'/> class with default properties.
    /// </summary>
    public PrinterResolution()
    {
        _kind = PrinterResolutionKind.Custom;
    }

    internal PrinterResolution(PrinterResolutionKind kind, int x, int y)
    {
        _kind = kind;
        X = x;
        Y = y;
    }

    /// <summary>
    ///  Gets a value indicating the kind of printer resolution.
    /// </summary>
    public PrinterResolutionKind Kind
    {
        get => _kind;
        set
        {
            if (value is < PrinterResolutionKind.High or > PrinterResolutionKind.Custom)
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(PrinterResolutionKind));
            }

            _kind = value;
        }
    }

    /// <summary>
    ///  Gets the printer resolution in the horizontal direction, in dots per inch.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    ///  Gets the printer resolution in the vertical direction, in dots per inch.
    /// </summary>
    public int Y { get; set; }

    public override string ToString() => _kind != PrinterResolutionKind.Custom
        ? $"[PrinterResolution {Kind}]"
        : FormattableString.Invariant($"[PrinterResolution X={X} Y={Y}]");
}

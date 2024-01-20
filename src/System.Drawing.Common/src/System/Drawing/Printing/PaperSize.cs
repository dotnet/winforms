// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Drawing.Printing;

/// <summary>
///  Specifies the size of a piece of paper.
/// </summary>
public partial class PaperSize
{
    private PaperKind _kind;
    private string _name;

    // Standard hundredths of an inch units
    private int _width;
    private int _height;
    private readonly bool _createdByDefaultConstructor;

    /// <summary>
    ///  Initializes a new instance of the <see cref='PaperSize'/> class with default properties.
    /// </summary>
    public PaperSize()
    {
        _kind = PaperKind.Custom;
        _name = string.Empty;
        _createdByDefaultConstructor = true;
    }

    internal PaperSize(PaperKind kind, string name, int width, int height)
    {
        _kind = kind;
        _name = name;
        _width = width;
        _height = height;
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='PaperSize'/> class.
    /// </summary>
    public PaperSize(string name, int width, int height)
    {
        _kind = PaperKind.Custom;
        _name = name;
        _width = width;
        _height = height;
    }

    /// <summary>
    ///  Gets or sets the height of the paper, in hundredths of an inch.
    /// </summary>
    public int Height
    {
        get => _height;
        set
        {
            if (_kind != PaperKind.Custom && !_createdByDefaultConstructor)
            {
                throw new ArgumentException(SR.PSizeNotCustom, nameof(value));
            }

            _height = value;
        }
    }

    /// <summary>
    ///  Gets the type of paper.
    /// </summary>
    public PaperKind Kind
        => _kind is <= ((PaperKind)PInvoke.DMPAPER_LAST)
            and not (((PaperKind)PInvoke.DMPAPER_RESERVED_48) or ((PaperKind)PInvoke.DMPAPER_RESERVED_49))
            ? _kind
            : PaperKind.Custom;

    /// <summary>
    ///  Gets or sets the name of the type of paper.
    /// </summary>
    public string PaperName
    {
        get => _name;
        set
        {
            if (_kind != PaperKind.Custom && !_createdByDefaultConstructor)
            {
                throw new ArgumentException(SR.PSizeNotCustom, nameof(value));
            }

            _name = value;
        }
    }

    /// <summary>
    ///  Same as Kind, but values larger than or equal to DMPAPER_LAST do not map to PaperKind.Custom.
    /// </summary>
    public int RawKind
    {
        get => (int)_kind;
        set => _kind = (PaperKind)value;
    }

    /// <summary>
    ///  Gets or sets the width of the paper, in hundredths of an inch.
    /// </summary>
    public int Width
    {
        get => _width;
        set
        {
            if (_kind != PaperKind.Custom && !_createdByDefaultConstructor)
            {
                throw new ArgumentException(SR.PSizeNotCustom, nameof(value));
            }

            _width = value;
        }
    }

    public override string ToString() => $"[PaperSize {PaperName} Kind={Kind} Height={Height.ToString(CultureInfo.InvariantCulture)} Width={Width.ToString(CultureInfo.InvariantCulture)}]";
}

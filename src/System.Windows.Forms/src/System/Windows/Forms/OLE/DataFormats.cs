// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Core.Ole;

namespace System.Windows.Forms;

/// <summary>
///  Translates between WinForms text-based <see cref="Clipboard"/>
///  formats and Win32 32-bit signed integer-based clipboard
///  formats. Provides <see langword="static"/> methods to create new
/// <see cref="Clipboard"/> formats and add them to the Windows Registry.
/// </summary>
public static partial class DataFormats
{
    /// <summary>
    ///  Specifies the standard ANSI text format.
    /// </summary>
    public static readonly string Text = DataFormatNames.Text;

    /// <summary>
    ///  Specifies the standard Windows Unicode text format.
    /// </summary>
    public static readonly string UnicodeText = DataFormatNames.UnicodeText;

    /// <summary>
    ///  Specifies the Windows Device Independent Bitmap (DIB) format.
    /// </summary>
    public static readonly string Dib = DataFormatNames.Dib;

    /// <summary>
    ///  Specifies a Windows bitmap format.
    /// </summary>
    public static readonly string Bitmap = DataFormatNames.Bitmap;

    /// <summary>
    ///  Specifies the Windows enhanced metafile format.
    /// </summary>
    public static readonly string EnhancedMetafile = DataFormatNames.Emf;

    /// <summary>
    ///  Specifies the Windows metafile format, which WinForms does not directly use.
    /// </summary>
    public static readonly string MetafilePict = DataFormatNames.Wmf;

    /// <summary>
    ///  Specifies the Windows symbolic link format, which WinForms does not directly use.
    /// </summary>
    public static readonly string SymbolicLink = DataFormatNames.SymbolicLink;

    /// <summary>
    ///  Specifies the Windows data interchange format, which WinForms does not directly use.
    /// </summary>
    public static readonly string Dif = DataFormatNames.Dif;

    /// <summary>
    ///  Specifies the Tagged Image File Format (TIFF), which WinForms does not directly use.
    /// </summary>
    public static readonly string Tiff = DataFormatNames.Tiff;

    /// <summary>
    ///  Specifies the standard Windows original equipment manufacturer (OEM) text format.
    /// </summary>
    public static readonly string OemText = DataFormatNames.OemText;

    /// <summary>
    ///  Specifies the Windows palette format.
    /// </summary>
    public static readonly string Palette = DataFormatNames.Palette;

    /// <summary>
    ///  Specifies the Windows pen data format, which consists of pen strokes for handwriting
    ///  software; WinForms does not use this format.
    /// </summary>
    public static readonly string PenData = DataFormatNames.PenData;

    /// <summary>
    ///  Specifies the Resource Interchange File Format (RIFF) audio format, which WinForms does not directly use.
    /// </summary>
    public static readonly string Riff = DataFormatNames.Riff;

    /// <summary>
    ///  Specifies the wave audio format, which Win Forms does not directly use.
    /// </summary>
    public static readonly string WaveAudio = DataFormatNames.WaveAudio;

    /// <summary>
    ///  Specifies the Windows file drop format, which WinForms does not directly use.
    /// </summary>
    public static readonly string FileDrop = DataFormatNames.FileDrop;

    /// <summary>
    ///  Specifies the Windows culture format, which WinForms does not directly use.
    /// </summary>
    public static readonly string Locale = DataFormatNames.Locale;

    /// <summary>
    ///  Specifies text consisting of HTML data.
    /// </summary>
    public static readonly string Html = DataFormatNames.Html;

    /// <summary>
    ///  Specifies text consisting of Rich Text Format (RTF) data.
    /// </summary>
    public static readonly string Rtf = DataFormatNames.Rtf;

    /// <summary>
    ///  Specifies a comma-separated value (CSV) format, which is a common interchange format
    ///  used by spreadsheets. This format is not used directly by WinForms.
    /// </summary>
    public static readonly string CommaSeparatedValue = DataFormatNames.Csv;

    /// <summary>
    ///  Specifies the WinForms string class format, which WinForms uses to store string objects.
    /// </summary>
    public static readonly string StringFormat = DataFormatNames.String;

    /// <summary>
    ///  Specifies a format that encapsulates any type of WinForms object.
    /// </summary>
    public static readonly string Serializable = DataFormatNames.Serializable;

    /// <summary>
    ///  Gets a <see cref="Format"/> with the Windows Clipboard numeric ID and name for the specified format.
    /// </summary>
    public static Format GetFormat(string format)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(format);
        return DataFormatsCore<Format>.GetOrAddFormat(format);
    }

    /// <summary>
    ///  Gets a <see cref="Format"/> with the Windows Clipboard numeric ID and name for the specified ID.
    /// </summary>
    public static Format GetFormat(int id) => DataFormatsCore<Format>.GetOrAddFormat(id);
}

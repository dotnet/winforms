// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Core.OLE;

namespace System.Windows.Forms;

/// <summary>
///  Translates between WinForms text-based <see cref="Clipboard"/>
///  formats and Win32 32-bit signed integer-based clipboard
///  formats. Provides <see langword="static"/> methods to create new
/// <see cref="Clipboard"/> formats and add them to the Windows Registry.
/// </summary>
public static partial class DataFormats
{
#pragma warning disable IDE1006 // Naming Styles
    /// <summary>
    ///  Specifies the standard ANSI text format.
    /// </summary>
    public static readonly string Text = DesktopDataFormats.TextConstant;

    /// <summary>
    ///  Specifies the standard Windows Unicode text format.
    /// </summary>
    public static readonly string UnicodeText = DesktopDataFormats.UnicodeTextConstant;

    /// <summary>
    ///  Specifies the Windows Device Independent Bitmap (DIB) format.
    /// </summary>
    public static readonly string Dib = DesktopDataFormats.DibConstant;

    /// <summary>
    ///  Specifies a Windows bitmap format.
    /// </summary>
    public static readonly string Bitmap = DesktopDataFormats.BitmapConstant;

    /// <summary>
    ///  Specifies the Windows enhanced metafile format.
    /// </summary>
    public static readonly string EnhancedMetafile = DesktopDataFormats.EmfConstant;

    /// <summary>
    ///  Specifies the Windows metafile format, which WinForms does not directly use.
    /// </summary>
    public static readonly string MetafilePict = DesktopDataFormats.WmfConstant;

    /// <summary>
    ///  Specifies the Windows symbolic link format, which WinForms does not directly use.
    /// </summary>
    public static readonly string SymbolicLink = DesktopDataFormats.SymbolicLinkConstant;

    /// <summary>
    ///  Specifies the Windows data interchange format, which WinForms does not directly use.
    /// </summary>
    public static readonly string Dif = DesktopDataFormats.DifConstant;

    /// <summary>
    ///  Specifies the Tagged Image File Format (TIFF), which WinForms does not directly use.
    /// </summary>
    public static readonly string Tiff = DesktopDataFormats.TiffConstant;

    /// <summary>
    ///  Specifies the standard Windows original equipment manufacturer (OEM) text format.
    /// </summary>
    public static readonly string OemText = DesktopDataFormats.OemTextConstant;

    /// <summary>
    ///  Specifies the Windows palette format.
    /// </summary>
    public static readonly string Palette = DesktopDataFormats.PaletteConstant;

    /// <summary>
    ///  Specifies the Windows pen data format, which consists of pen strokes for handwriting
    ///  software; WinForms does not use this format.
    /// </summary>
    public static readonly string PenData = DesktopDataFormats.PenDataConstant;

    /// <summary>
    ///  Specifies the Resource Interchange File Format (RIFF) audio format, which WinForms does not directly use.
    /// </summary>
    public static readonly string Riff = DesktopDataFormats.RiffConstant;

    /// <summary>
    ///  Specifies the wave audio format, which Win Forms does not directly use.
    /// </summary>
    public static readonly string WaveAudio = DesktopDataFormats.WaveAudioConstant;

    /// <summary>
    ///  Specifies the Windows file drop format, which WinForms does not directly use.
    /// </summary>
    public static readonly string FileDrop = DesktopDataFormats.FileDropConstant;

    /// <summary>
    ///  Specifies the Windows culture format, which WinForms does not directly use.
    /// </summary>
    public static readonly string Locale = DesktopDataFormats.LocaleConstant;

    /// <summary>
    ///  Specifies text consisting of HTML data.
    /// </summary>
    public static readonly string Html = DesktopDataFormats.HtmlConstant;

    /// <summary>
    ///  Specifies text consisting of Rich Text Format (RTF) data.
    /// </summary>
    public static readonly string Rtf = DesktopDataFormats.RtfConstant;

    /// <summary>
    ///  Specifies a comma-separated value (CSV) format, which is a common interchange format
    ///  used by spreadsheets. This format is not used directly by WinForms.
    /// </summary>
    public static readonly string CommaSeparatedValue = DesktopDataFormats.CsvConstant;

    /// <summary>
    ///  Specifies the WinForms string class format, which WinForms uses to store string objects.
    /// </summary>
    public static readonly string StringFormat = DesktopDataFormats.StringConstant;

    /// <summary>
    ///  Specifies a format that encapsulates any type of WinForms object.
    /// </summary>
    public static readonly string Serializable = DesktopDataFormats.SerializableConstant;
#pragma warning restore IDE1006 // Naming Styles

    private static Format[]? s_formatList;

    private static int s_formatCount;

    private static readonly Lock s_internalSyncObject = new();

    /// <summary>
    ///  Gets a <see cref="Format"/> with the Windows Clipboard numeric ID and name for the specified format.
    /// </summary>
    public static Format GetFormat(string format)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(format);
        lock (s_internalSyncObject)
        {
            s_formatList ??= [];
            // It is much faster to do a case sensitive search here.
            // So do the case sensitive compare first, then the expensive one.
            for (int n = 0; n < s_formatCount; n++)
            {
                if (s_formatList[n].Name.Equals(format))
                {
                    return s_formatList[n];
                }
            }

            for (int n = 0; n < s_formatCount; n++)
            {
                if (string.Equals(s_formatList[n].Name, format, StringComparison.OrdinalIgnoreCase))
                {
                    return s_formatList[n];
                }
            }

            DesktopDataFormats.Format innerFormat = DesktopDataFormats.GetFormat(format);
            EnsureFormatSpace(1);
            s_formatList[s_formatCount] = new(innerFormat.Name, innerFormat.Id);
            return s_formatList[s_formatCount++];
        }
    }

    /// <summary>
    ///  Gets a <see cref="Format"/> with the Windows Clipboard numeric ID and name for the specified ID.
    /// </summary>
    public static Format GetFormat(int id)
    {
        lock (s_internalSyncObject)
        {
            s_formatList ??= [];
            // Win32 uses an unsigned 16 bit type as a format ID, thus stripping off the leading bits.
            // Registered format IDs are in the range 0xC000 through 0xFFFF, thus it's important
            // to represent format as an unsigned type.
            ushort strippedId = (ushort)(id & 0xFFFF);
            for (int n = 0; n < s_formatCount; n++)
            {
                if (s_formatList[n].Id == strippedId)
                {
                    return s_formatList[n];
                }
            }

            DesktopDataFormats.Format innerFormat = DesktopDataFormats.GetFormat(strippedId);
            EnsureFormatSpace(1);
            s_formatList[s_formatCount] = new(innerFormat.Name, innerFormat.Id);
            return s_formatList[s_formatCount++];
        }
    }

    /// <summary>
    ///  Ensures that we have enough room in our format list
    /// </summary>
    [MemberNotNull(nameof(s_formatList))]
    private static void EnsureFormatSpace(int size)
    {
        if (s_formatList is null || s_formatList.Length <= s_formatCount + size)
        {
            int newSize = s_formatCount + 20;

            Format[] newList = new Format[newSize];
            for (int n = 0; n < s_formatCount; n++)
            {
                newList[n] = s_formatList![n];
            }

            s_formatList = newList;
        }
    }
}

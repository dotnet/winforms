// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms;

/// <summary>
///  Translates between WinForms text-based <see cref="Clipboard"/>
///  formats and Win32 32-bit signed integer-based clipboard
///  formats. Provides <see langword="static"/> methods to create new
/// <see cref="Clipboard"/> formats and add them to the Windows Registry.
/// </summary>
public static partial class DataFormats
{
    internal const string TextConstant = "Text";
    internal const string UnicodeTextConstant = "UnicodeText";
    internal const string DibConstant = "DeviceIndependentBitmap";
    internal const string BitmapConstant = "Bitmap";
    internal const string EmfConstant = "EnhancedMetafile";
    internal const string WmfConstant = "MetaFilePict";
    internal const string SymbolicLinkConstant = "SymbolicLink";
    internal const string DifConstant = "DataInterchangeFormat";
    internal const string TiffConstant = "TaggedImageFileFormat";
    internal const string OemTextConstant = "OEMText";
    internal const string PaletteConstant = "Palette";
    internal const string PenDataConstant = "PenData";
    internal const string RiffConstant = "RiffAudio";
    internal const string WaveAudioConstant = "WaveAudio";
    internal const string FileDropConstant = "FileDrop";
    internal const string LocaleConstant = "Locale";
    internal const string HtmlConstant = "HTML Format";
    internal const string RtfConstant = "Rich Text Format";
    internal const string CsvConstant = "Csv";
    internal const string StringConstant = "System.String";
    internal const string SerializableConstant = "WindowsForms10PersistentObject";

    /// <summary>
    ///  Specifies the standard ANSI text format.
    /// </summary>
    public static readonly string Text = TextConstant;

    /// <summary>
    ///  Specifies the standard Windows Unicode text format.
    /// </summary>
    public static readonly string UnicodeText = UnicodeTextConstant;

    /// <summary>
    ///  Specifies the Windows Device Independent Bitmap (DIB) format.
    /// </summary>
    public static readonly string Dib = DibConstant;

    /// <summary>
    ///  Specifies a Windows bitmap format.
    /// </summary>
    public static readonly string Bitmap = BitmapConstant;

    /// <summary>
    ///  Specifies the Windows enhanced metafile format.
    /// </summary>
    public static readonly string EnhancedMetafile = EmfConstant;

    /// <summary>
    ///  Specifies the Windows metafile format, which WinForms does not directly use.
    /// </summary>
    public static readonly string MetafilePict = WmfConstant;

    /// <summary>
    ///  Specifies the Windows symbolic link format, which WinForms does not directly use.
    /// </summary>
    public static readonly string SymbolicLink = SymbolicLinkConstant;

    /// <summary>
    ///  Specifies the Windows data interchange format, which WinForms does not directly use.
    /// </summary>
    public static readonly string Dif = DifConstant;

    /// <summary>
    ///  Specifies the Tagged Image File Format (TIFF), which WinForms does not directly use.
    /// </summary>
    public static readonly string Tiff = TiffConstant;

    /// <summary>
    ///  Specifies the standard Windows original equipment manufacturer (OEM) text format.
    /// </summary>
    public static readonly string OemText = OemTextConstant;

    /// <summary>
    ///  Specifies the Windows palette format.
    /// </summary>
    public static readonly string Palette = PaletteConstant;

    /// <summary>
    ///  Specifies the Windows pen data format, which consists of pen strokes for handwriting
    ///  software; WinForms does not use this format.
    /// </summary>
    public static readonly string PenData = PenDataConstant;

    /// <summary>
    ///  Specifies the Resource Interchange File Format (RIFF) audio format, which WinForms does not directly use.
    /// </summary>
    public static readonly string Riff = RiffConstant;

    /// <summary>
    ///  Specifies the wave audio format, which Win Forms does not directly use.
    /// </summary>
    public static readonly string WaveAudio = WaveAudioConstant;

    /// <summary>
    ///  Specifies the Windows file drop format, which WinForms does not directly use.
    /// </summary>
    public static readonly string FileDrop = FileDropConstant;

    /// <summary>
    ///  Specifies the Windows culture format, which WinForms does not directly use.
    /// </summary>
    public static readonly string Locale = LocaleConstant;

    /// <summary>
    ///  Specifies text consisting of HTML data.
    /// </summary>
    public static readonly string Html = HtmlConstant;

    /// <summary>
    ///  Specifies text consisting of Rich Text Format (RTF) data.
    /// </summary>
    public static readonly string Rtf = RtfConstant;

    /// <summary>
    ///  Specifies a comma-separated value (CSV) format, which is a common interchange format
    ///  used by spreadsheets. This format is not used directly by WinForms.
    /// </summary>
    public static readonly string CommaSeparatedValue = CsvConstant;

    /// <summary>
    ///  Specifies the WinForms string class format, which WinForms uses to store string objects.
    /// </summary>
    public static readonly string StringFormat = StringConstant;

    /// <summary>
    ///  Specifies a format that encapsulates any type of WinForms object.
    /// </summary>
    public static readonly string Serializable = SerializableConstant;

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
            EnsurePredefined();

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

            // Need to add this format string
            uint formatId = PInvoke.RegisterClipboardFormat(format);
            if (formatId == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), SR.RegisterCFFailed);
            }

            EnsureFormatSpace(1);
            s_formatList[s_formatCount] = new Format(format, (int)formatId);
            return s_formatList[s_formatCount++];
        }
    }

    /// <summary>
    ///  Gets a <see cref="Format"/> with the Windows Clipboard numeric ID and name for the specified ID.
    /// </summary>
    public static Format GetFormat(int id) =>
        // Win32 uses an unsigned 16 bit type as a format ID, thus stripping off the leading bits.
        // Registered format IDs are in the range 0xC000 through 0xFFFF, thus it's important
        // to represent format as an unsigned type.
        GetFormat((ushort)(id & 0xFFFF));

    /// <inheritdoc cref="GetFormat(int)"/>
    internal static unsafe Format GetFormat(ushort id)
    {
        lock (s_internalSyncObject)
        {
            EnsurePredefined();

            for (int n = 0; n < s_formatCount; n++)
            {
                if (s_formatList[n].Id == id)
                {
                    return s_formatList[n];
                }
            }

            string? name = null;

            // The max length of the name of clipboard formats is equal to the max length
            // of a Win32 Atom of 255 chars. An additional null terminator character is added,
            // giving a required capacity of 256 chars.
            Span<char> formatName = stackalloc char[256];
            fixed (char* pFormatName = formatName)
            {
                int length = PInvoke.GetClipboardFormatName(id, pFormatName, 256);
                if (length != 0)
                {
                    name = formatName[..length].ToString();
                }
            }

            // This can happen if windows adds a standard format that we don't know about,
            // so we should play it safe.
            name ??= $"Format{id}";

            EnsureFormatSpace(1);
            s_formatList[s_formatCount] = new Format(name, id);
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

    /// <summary>
    ///  Ensures that the Win32 predefined formats are setup in our format list.
    ///  This is called anytime we need to search the list
    /// </summary>
    [MemberNotNull(nameof(s_formatList))]
    private static void EnsurePredefined()
    {
        if (s_formatCount == 0)
        {
            s_formatList =
            [
                // Text name                  Win32 format ID
                new(UnicodeTextConstant,       (int)CLIPBOARD_FORMAT.CF_UNICODETEXT),
                new(TextConstant,              (int)CLIPBOARD_FORMAT.CF_TEXT),
                new(BitmapConstant,            (int)CLIPBOARD_FORMAT.CF_BITMAP),
                new(WmfConstant,               (int)CLIPBOARD_FORMAT.CF_METAFILEPICT),
                new(EmfConstant,               (int)CLIPBOARD_FORMAT.CF_ENHMETAFILE),
                new(DifConstant,               (int)CLIPBOARD_FORMAT.CF_DIF),
                new(TiffConstant,              (int)CLIPBOARD_FORMAT.CF_TIFF),
                new(OemTextConstant,           (int)CLIPBOARD_FORMAT.CF_OEMTEXT),
                new(DibConstant,               (int)CLIPBOARD_FORMAT.CF_DIB),
                new(PaletteConstant,           (int)CLIPBOARD_FORMAT.CF_PALETTE),
                new(PenDataConstant,           (int)CLIPBOARD_FORMAT.CF_PENDATA),
                new(RiffConstant,              (int)CLIPBOARD_FORMAT.CF_RIFF),
                new(WaveAudioConstant,         (int)CLIPBOARD_FORMAT.CF_WAVE),
                new(SymbolicLinkConstant,      (int)CLIPBOARD_FORMAT.CF_SYLK),
                new(FileDropConstant,          (int)CLIPBOARD_FORMAT.CF_HDROP),
                new(LocaleConstant,            (int)CLIPBOARD_FORMAT.CF_LOCALE)
            ];

            s_formatCount = s_formatList.Length;
        }
        else
        {
            s_formatList ??= [];
        }
    }
}

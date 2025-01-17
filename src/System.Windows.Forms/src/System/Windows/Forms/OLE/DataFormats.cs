// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Private.Windows.Core.Ole;
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

    /// <summary>
    ///  Gets a <see cref="Format"/> with the Windows Clipboard numeric ID and name for the specified format.
    /// </summary>
    public static Format GetFormat(string format)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(format);
        return DataFormatsCore<Format>.GetOrAddFormat(format);
    }

    internal static partial class DataFormatsCore<T> where T : IDataFormat<T>
    {
        private static List<T>? s_formatList;
        private static T[]? s_predefinedFormatList;

        private static readonly Lock s_internalSyncObject = new();

        internal static T GetOrAddFormat(string format)
        {
            lock (s_internalSyncObject)
            {
                EnsurePredefined();

                // It is much faster to do a case sensitive search here.
                // So do the case sensitive compare first, then the expensive one.
                if (TryFindFormat(s_predefinedFormatList, format, StringComparison.Ordinal, out var found)
                    || TryFindFormat(s_formatList, format, StringComparison.Ordinal, out found)
                    || TryFindFormat(s_predefinedFormatList, format, StringComparison.OrdinalIgnoreCase, out found)
                    || TryFindFormat(s_formatList, format, StringComparison.OrdinalIgnoreCase, out found))
                {
                    return found;
                }

                // Need to add this format string
                uint formatId = PInvoke.RegisterClipboardFormat(format);
                if (formatId == 0)
                {
                    throw new Win32Exception(SR.RegisterCFFailed);
                }

                s_formatList ??= [];
                T newFormat = T.Create(format, (int)formatId);
                s_formatList.Add(newFormat);
                return newFormat;
            }

            static bool TryFindFormat(
                IReadOnlyList<T>? formats,
                string name,
                StringComparison comparison,
                [NotNullWhen(true)] out T? format)
            {
                if (formats is not null)
                {
                    for (int i = 0; i < formats.Count; i++)
                    {
                        format = formats[i];
                        if (string.Equals(format.Name, name, comparison))
                        {
                            return true;
                        }
                    }
                }

                format = default;
                return false;
            }
        }
    }

    /// <summary>
    ///  Gets a <see cref="Format"/> with the Windows Clipboard numeric ID and name for the specified ID.
    /// </summary>
    public static Format GetFormat(int id) => DataFormatsCore<Format>.GetOrAddFormat(id);

    internal static partial class DataFormatsCore<T> where T : IDataFormat<T>
    {
        internal static unsafe T GetOrAddFormat(int id)
        {
            // Win32 uses an unsigned 16 bit type as a format ID, thus stripping off the leading bits.
            // Registered format IDs are in the range 0xC000 through 0xFFFF, thus it's important
            // to represent format as an unsigned type.
            ushort shortId = (ushort)(id & 0xFFFF);

            lock (s_internalSyncObject)
            {
                EnsurePredefined();

                if (TryFindFormat(s_predefinedFormatList, shortId, out T? found)
                    || TryFindFormat(s_formatList, shortId, out found))
                {
                    return found;
                }

                // The max length of the name of clipboard formats is equal to the max length
                // of a Win32 Atom of 255 chars. An additional null terminator character is added,
                // giving a required capacity of 256 chars.

                string? name = null;
                Span<char> buffer = stackalloc char[256];
                fixed (char* pBuffer = buffer)
                {
                    int length = PInvoke.GetClipboardFormatName(shortId, pBuffer, 256);
                    if (length != 0)
                    {
                        name = buffer[..length].ToString();
                    }
                }

                // This can happen if windows adds a standard format that we don't know about,
                // so we should play it safe.
                name ??= $"Format{shortId}";

                s_formatList ??= [];
                T newFormat = T.Create(name, shortId);
                s_formatList.Add(newFormat);
                return newFormat;

                static bool TryFindFormat(
                    IReadOnlyList<T>? formats,
                    int id,
                    [NotNullWhen(true)] out T? format)
                {
                    if (formats is not null)
                    {
                        for (int i = 0; i < formats.Count; i++)
                        {
                            format = formats[i];
                            if (format.Id == id)
                            {
                                return true;
                            }
                        }
                    }

                    format = default;
                    return false;
                }
            }
        }

        /// <summary>
        ///  Ensures that the Win32 predefined formats are setup in our format list.
        ///  This is called anytime we need to search the list
        /// </summary>
        [MemberNotNull(nameof(s_predefinedFormatList))]
        private static void EnsurePredefined()
        {
            s_predefinedFormatList ??=
            [
                // Text name                        Win32 format ID
                T.Create(UnicodeTextConstant,       (int)CLIPBOARD_FORMAT.CF_UNICODETEXT),
                T.Create(TextConstant,              (int)CLIPBOARD_FORMAT.CF_TEXT),
                T.Create(BitmapConstant,            (int)CLIPBOARD_FORMAT.CF_BITMAP),
                T.Create(WmfConstant,               (int)CLIPBOARD_FORMAT.CF_METAFILEPICT),
                T.Create(EmfConstant,               (int)CLIPBOARD_FORMAT.CF_ENHMETAFILE),
                T.Create(DifConstant,               (int)CLIPBOARD_FORMAT.CF_DIF),
                T.Create(TiffConstant,              (int)CLIPBOARD_FORMAT.CF_TIFF),
                T.Create(OemTextConstant,           (int)CLIPBOARD_FORMAT.CF_OEMTEXT),
                T.Create(DibConstant,               (int)CLIPBOARD_FORMAT.CF_DIB),
                T.Create(PaletteConstant,           (int)CLIPBOARD_FORMAT.CF_PALETTE),
                T.Create(PenDataConstant,           (int)CLIPBOARD_FORMAT.CF_PENDATA),
                T.Create(RiffConstant,              (int)CLIPBOARD_FORMAT.CF_RIFF),
                T.Create(WaveAudioConstant,         (int)CLIPBOARD_FORMAT.CF_WAVE),
                T.Create(SymbolicLinkConstant,      (int)CLIPBOARD_FORMAT.CF_SYLK),
                T.Create(FileDropConstant,          (int)CLIPBOARD_FORMAT.CF_HDROP),
                T.Create(LocaleConstant,            (int)CLIPBOARD_FORMAT.CF_LOCALE)
            ];
        }
    }
}

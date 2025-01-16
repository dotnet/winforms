// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Private.Windows.Core.Resources;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.System.Ole;

namespace System.Private.Windows.Core.OLE;

/// <summary>
///  Translates between WinForms text-based <see cref="DesktopClipboard"/>
///  formats and Win32 32-bit signed integer-based clipboard
///  formats. Provides <see langword="static"/> methods to create new
/// <see cref="DesktopClipboard"/> formats and add them to the Windows Registry.
/// </summary>
internal static partial class DesktopDataFormats
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

    private static Format[]? s_formatList;

    private static int s_formatCount;

#if NET9_0_OR_GREATER
    private static readonly Lock s_internalSyncObject = new();
#else
    private static readonly object s_internalSyncObject = new();
#endif

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
            uint formatId = PInvokeCore.RegisterClipboardFormat(format);
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
                int length = PInvokeCore.GetClipboardFormatName(id, pFormatName, 256);
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

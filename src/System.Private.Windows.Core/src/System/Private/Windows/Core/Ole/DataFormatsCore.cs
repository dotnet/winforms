// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Private.Windows.Core.Resources;
using Windows.Win32;
using Windows.Win32.System.Ole;

namespace System.Private.Windows.Core.Ole;

internal static partial class DataFormatsCore<T> where T : IDataFormat<T>
{
    private static List<T>? s_formatList;
    private static T[]? s_predefinedFormatList;

#if NET9_0_OR_GREATER
    private static readonly Lock s_internalSyncObject = new();
#else
    private static readonly object s_internalSyncObject = new();
#endif

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
            uint formatId = PInvokeCore.RegisterClipboardFormat(format);
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
                int length = PInvokeCore.GetClipboardFormatName(shortId, pBuffer, 256);
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
            // Text name                                    Win32 format ID
            T.Create(DataFormatNames.UnicodeText,       (int)CLIPBOARD_FORMAT.CF_UNICODETEXT),
            T.Create(DataFormatNames.Text,              (int)CLIPBOARD_FORMAT.CF_TEXT),
            T.Create(DataFormatNames.Bitmap,            (int)CLIPBOARD_FORMAT.CF_BITMAP),
            T.Create(DataFormatNames.Wmf,               (int)CLIPBOARD_FORMAT.CF_METAFILEPICT),
            T.Create(DataFormatNames.Emf,               (int)CLIPBOARD_FORMAT.CF_ENHMETAFILE),
            T.Create(DataFormatNames.Dif,               (int)CLIPBOARD_FORMAT.CF_DIF),
            T.Create(DataFormatNames.Tiff,              (int)CLIPBOARD_FORMAT.CF_TIFF),
            T.Create(DataFormatNames.OemText,           (int)CLIPBOARD_FORMAT.CF_OEMTEXT),
            T.Create(DataFormatNames.Dib,               (int)CLIPBOARD_FORMAT.CF_DIB),
            T.Create(DataFormatNames.Palette,           (int)CLIPBOARD_FORMAT.CF_PALETTE),
            T.Create(DataFormatNames.PenData,           (int)CLIPBOARD_FORMAT.CF_PENDATA),
            T.Create(DataFormatNames.Riff,              (int)CLIPBOARD_FORMAT.CF_RIFF),
            T.Create(DataFormatNames.WaveAudio,         (int)CLIPBOARD_FORMAT.CF_WAVE),
            T.Create(DataFormatNames.SymbolicLink,      (int)CLIPBOARD_FORMAT.CF_SYLK),
            T.Create(DataFormatNames.FileDrop,          (int)CLIPBOARD_FORMAT.CF_HDROP),
            T.Create(DataFormatNames.Locale,            (int)CLIPBOARD_FORMAT.CF_LOCALE)
        ];
    }
}

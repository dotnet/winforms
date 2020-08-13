// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Translates between WinForms text-based <see cref='Clipboard'/>
    ///  formats and Win32 32-bit signed integer-based clipboard
    ///  formats. Provides <see langword='static'/> methods to create new
    /// <see cref='Clipboard'/> formats and add them to the Windows Registry.
    /// </summary>
    public static class DataFormats
    {
        /// <summary>
        ///  Specifies the standard ANSI text format. This <see langword='static'/>
        ///  field is read-only.
        /// </summary>
        public static readonly string Text = "Text";

        /// <summary>
        ///  Specifies the standard Windows Unicode text format.
        ///  This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string UnicodeText = "UnicodeText";

        /// <summary>
        ///  Specifies the Windows Device Independent Bitmap (DIB) format.
        ///  This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string Dib = "DeviceIndependentBitmap";

        /// <summary>
        ///  Specifies a Windows bitmap format.
        ///  This <see langword='static'/> field is read-only. </summary>
        public static readonly string Bitmap = "Bitmap";

        /// <summary>
        ///  Specifies the Windows enhanced metafile format.
        ///  This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string EnhancedMetafile = "EnhancedMetafile";

        /// <summary>
        ///  Specifies the Windows metafile format, which WinForms does not directly use.
        ///  This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string MetafilePict = "MetaFilePict";

        /// <summary>
        ///  Specifies the Windows symbolic link format, which WinForms does not directly use.
        ///  This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string SymbolicLink = "SymbolicLink";

        /// <summary>
        ///  Specifies the Windows data interchange format, which WinForms does not directly use.
        ///  This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string Dif = "DataInterchangeFormat";

        /// <summary>
        ///  Specifies the Tagged Image File Format (TIFF), which WinForms does not directly use.
        ///  This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string Tiff = "TaggedImageFileFormat";

        /// <summary>
        ///  Specifies the standard Windows original equipment manufacturer (OEM) text format.
        ///  This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string OemText = "OEMText";

        /// <summary>
        ///  Specifies the Windows palette format.
        ///  This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string Palette = "Palette";

        /// <summary>
        ///  Specifies the Windows pen data format, which consists of pen strokes for handwriting
        ///  software; Win Formsdoes not use this format.
        ///  This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string PenData = "PenData";

        /// <summary>
        ///  Specifies the Resource Interchange File Format (RIFF) audio format, which WinForms
        ///  does not directly use.
        ///  This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string Riff = "RiffAudio";

        /// <summary>
        ///  Specifies the wave audio format, which Win Forms does not
        ///  directly use. This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string WaveAudio = "WaveAudio";

        /// <summary>
        ///  Specifies the Windows file drop format, which WinForms does not directly use.
        ///  This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string FileDrop = "FileDrop";

        /// <summary>
        ///  Specifies the Windows culture format, which WinForms does not directly use.
        ///  This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string Locale = "Locale";

        /// <summary>
        ///  Specifies text consisting of HTML data.
        ///  This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string Html = "HTML Format";

        /// <summary>
        ///  Specifies text consisting of Rich Text Format (RTF) data.
        ///  This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string Rtf = "Rich Text Format";

        /// <summary>
        ///  Specifies a comma-separated value (CSV) format, which is a common interchange format
        ///  used by spreadsheets. This format is not used directly by WinForms.
        ///  This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string CommaSeparatedValue = "Csv";

        /// <summary>
        ///  Specifies the Win Forms string class format, which WinForms uses to store string
        ///  objects.
        ///  This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string StringFormat = typeof(string).FullName;

        /// <summary>
        ///  Specifies a format that encapsulates any type of WinForms object.
        ///  This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly string Serializable = Application.WindowsFormsVersion + "PersistentObject";

        private static Format[] s_formatList;
        private static int s_formatCount;

        private static readonly object s_internalSyncObject = new object();

        /// <summary>
        ///  Gets a <see cref='Format'/> with the Windows
        ///  Clipboard numeric ID and name for the specified format.
        /// </summary>
        public static Format GetFormat(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                throw new ArgumentException(nameof(format));
            }

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
                uint formatId = User32.RegisterClipboardFormatW(format);
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
        ///  Gets a <see cref='Format'/> with the Windows
        ///  Clipboard numeric ID and name for the specified ID.
        /// </summary>
        public static Format GetFormat(int id)
        {
            // Win32 uses an unsigned 16 bit type as a format ID, thus stripping off the leading bits.
            // Registered format IDs are in the range 0xC000 through 0xFFFF, thus it's important
            // to represent format as an unsigned type.
            ushort clampedId = (ushort)(id & 0xFFFF);

            lock (s_internalSyncObject)
            {
                EnsurePredefined();

                for (int n = 0; n < s_formatCount; n++)
                {
                    if (s_formatList[n].Id == clampedId)
                    {
                        return s_formatList[n];
                    }
                }

                string name = User32.GetClipboardFormatNameW(clampedId);
                if (name is null)
                {
                    // This can happen if windows adds a standard format that we don't know about,
                    // so we should play it safe.
                    name = "Format" + clampedId;
                }

                EnsureFormatSpace(1);
                s_formatList[s_formatCount] = new Format(name, clampedId);
                return s_formatList[s_formatCount++];
            }
        }

        /// <summary>
        ///  Ensures that we have enough room in our format list
        /// </summary>
        private static void EnsureFormatSpace(int size)
        {
            if (s_formatList is null || s_formatList.Length <= s_formatCount + size)
            {
                int newSize = s_formatCount + 20;

                Format[] newList = new Format[newSize];
                for (int n = 0; n < s_formatCount; n++)
                {
                    newList[n] = s_formatList[n];
                }

                s_formatList = newList;
            }
        }

        /// <summary>
        ///  Ensures that the Win32 predefined formats are setup in our format list.
        ///  This is called anytime we need to search the list
        /// </summary>
        private static void EnsurePredefined()
        {
            if (s_formatCount == 0)
            {
                s_formatList = new Format[]
                {
                    //         Text name        Win32 format ID
                    new Format(UnicodeText,       (int)User32.CF.UNICODETEXT),
                    new Format(Text,              (int)User32.CF.TEXT),
                    new Format(Bitmap,            (int)User32.CF.BITMAP),
                    new Format(MetafilePict,      (int)User32.CF.METAFILEPICT),
                    new Format(EnhancedMetafile,  (int)User32.CF.ENHMETAFILE),
                    new Format(Dif,               (int)User32.CF.DIF),
                    new Format(Tiff,              (int)User32.CF.TIFF),
                    new Format(OemText,           (int)User32.CF.OEMTEXT),
                    new Format(Dib,               (int)User32.CF.DIB),
                    new Format(Palette,           (int)User32.CF.PALETTE),
                    new Format(PenData,           (int)User32.CF.PENDATA),
                    new Format(Riff,              (int)User32.CF.RIFF),
                    new Format(WaveAudio,         (int)User32.CF.WAVE),
                    new Format(SymbolicLink,      (int)User32.CF.SYLK),
                    new Format(FileDrop,          (int)User32.CF.HDROP),
                    new Format(Locale,            (int)User32.CF.LOCALE)
                };

                s_formatCount = s_formatList.Length;
            }
        }

        /// <summary>
        ///  Represents a format type.
        /// </summary>
        public class Format
        {
            /// <summary>
            ///  Initializes a new instance of the <see cref='Format'/> class and
            ///  specifies whether a Win32 handle is expected with this format.
            /// </summary>
            public Format(string name, int id)
            {
                Name = name;
                Id = id;
            }

            /// <summary>
            ///  Specifies the name of this format.
            /// </summary>
            public string Name { get; }

            /// <summary>
            ///  Specifies the ID number for this format.
            /// </summary>
            public int Id { get; }
        }
    }
}

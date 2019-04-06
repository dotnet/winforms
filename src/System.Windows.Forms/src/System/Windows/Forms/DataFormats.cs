// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Text;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Translates between WinForms text-based <see cref='System.Windows.Forms.Clipboard'/>
    /// formats and <see langword='Microsoft.Win32'/> 32-bit signed integer-based clipboard
    /// formats. Provides <see langword='static '/> methods to create new
    /// <see cref='System.Windows.Forms.Clipboard'/> formats and add them to the Windows Registry.
    /// </devdoc>
    public static class DataFormats
    {
        /// <devdoc>
        /// Specifies the standard ANSI text format. This <see langword='static '/>
        /// field is read-only.
        /// </devdoc>
        public static readonly string Text = "Text";

        /// <devdoc>
        /// Specifies the standard Windows Unicode text format.
        /// This <see langword='static '/> field is read-only.
        /// </devdoc>
        public static readonly string UnicodeText = "UnicodeText";

        /// <devdoc>
        /// Specifies the Windows Device Independent Bitmap (DIB) format.
        /// This <see langword='static '/> field is read-only.
        /// </devdoc>
        public static readonly string Dib = "DeviceIndependentBitmap";

        /// <devdoc>
        /// Specifies a Windows bitmap format.
        /// This <see langword='static '/> field is read-only. </devdoc>
        public static readonly string Bitmap = "Bitmap";

        /// <devdoc>
        /// Specifies the Windows enhanced metafile format.
        /// This <see langword='static '/> field is read-only.
        /// </devdoc>
        public static readonly string EnhancedMetafile = "EnhancedMetafile";

        /// <devdoc>
        /// Specifies the Windows metafile format, which WinForms does not directly use.
        /// This <see langword='static '/> field is read-only.
        /// </devdoc>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Would be a breaking change to rename this")]
        public static readonly string MetafilePict = "MetaFilePict";

        /// <devdoc>
        /// Specifies the Windows symbolic link format, which WinForms does not directly use.
        /// This <see langword='static '/> field is read-only.
        /// </devdoc>
        public static readonly string SymbolicLink = "SymbolicLink";

        /// <devdoc>
        /// Specifies the Windows data interchange format, which WinForms does not directly use.
        /// This <see langword='static '/> field is read-only.
        /// </devdoc>
        public static readonly string Dif = "DataInterchangeFormat";

        /// <devdoc>
        /// Specifies the Tagged Image File Format (TIFF), which WinForms does not directly use.
        /// This <see langword='static '/> field is read-only.
        /// </devdoc>
        public static readonly string Tiff = "TaggedImageFileFormat";

        /// <devdoc>
        /// Specifies the standard Windows original equipment manufacturer (OEM) text format.
        /// This <see langword='static '/> field is read-only.
        /// </devdoc>
        public static readonly string OemText = "OEMText";

        /// <devdoc>
        /// Specifies the Windows palette format.
        /// This <see langword='static '/> field is read-only.
        /// </devdoc>
        public static readonly string Palette = "Palette";

        /// <devdoc>
        /// Specifies the Windows pen data format, which consists of pen strokes for handwriting
        /// software; Win Formsdoes not use this format.
        /// This <see langword='static '/> field is read-only.
        /// </devdoc>
        public static readonly string PenData = "PenData";

        /// <devdoc>
        /// Specifies the Resource Interchange File Format (RIFF) audio format, which WinForms
        /// does not directly use.
        /// This <see langword='static '/> field is read-only.
        /// </devdoc>
        public static readonly string Riff = "RiffAudio";

        /// <devdoc>
        /// Specifies the wave audio format, which Win Forms does not
        ///   directly use. This <see langword='static '/> field is read-only.
        /// </devdoc>
        public static readonly string WaveAudio = "WaveAudio";

        /// <devdoc>
        /// Specifies the Windows file drop format, which WinForms does not directly use.
        /// This <see langword='static '/> field is read-only.
        /// </devdoc>
        public static readonly string FileDrop = "FileDrop";

        /// <devdoc>
        /// Specifies the Windows culture format, which WinForms does not directly use.
        /// This <see langword='static '/> field is read-only.
        /// </devdoc>
        public static readonly string Locale = "Locale";

        /// <devdoc>
        /// Specifies text consisting of HTML data.
        /// This <see langword='static '/> field is read-only.
        /// </devdoc>
        public static readonly string Html = "HTML Format";

        /// <devdoc>
        /// Specifies text consisting of Rich Text Format (RTF) data.
        /// This <see langword='static '/> field is read-only.
        /// </devdoc>
        public static readonly string Rtf = "Rich Text Format";

        /// <devdoc>
        /// Specifies a comma-separated value (CSV) format, which is a common interchange format
        /// used by spreadsheets. This format is not used directly by WinForms.
        /// This <see langword='static '/> field is read-only.
        /// </devdoc>
        public static readonly string CommaSeparatedValue = "Csv";

        /// <devdoc>
        /// Specifies the Win Forms string class format, which WinForms uses to store string
        /// objects.
        /// This <see langword='static '/> field is read-only.
        /// </devdoc>
        public static readonly string StringFormat = typeof(string).FullName;

        /// <devdoc>
        /// Specifies a format that encapsulates any type of WinForms object.
        /// This <see langword='static '/> field is read-only.
        /// </devdoc>
        public static readonly string Serializable = Application.WindowsFormsVersion + "PersistentObject";

        private static Format[] s_formatList;
        private static int s_formatCount = 0;

        private static object s_internalSyncObject = new object();

        /// <devdoc>
        /// Gets a <see cref='System.Windows.Forms.DataFormats.Format'/> with the Windows
        /// Clipboard numeric ID and name for the specified format.
        /// </devdoc>
        public static Format GetFormat(string format)
        {
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
                int formatId = SafeNativeMethods.RegisterClipboardFormat(format);
                if (formatId == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), SR.RegisterCFFailed);
                }

                EnsureFormatSpace(1);
                s_formatList[s_formatCount] = new Format(format, formatId);
                return s_formatList[s_formatCount++];
            }
        }

        /// <devdoc>
        /// Gets a <see cref='System.Windows.Forms.DataFormats.Format'/> with the Windows
        /// Clipboard numeric ID and name for the specified ID.
        /// </devdoc>
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

                var nameBuilder = new StringBuilder(256);

                // This can happen if windows adds a standard format that we don't know about,
                // so we should play it safe.
                if (SafeNativeMethods.GetClipboardFormatName(clampedId, nameBuilder, nameBuilder.Capacity) == 0)
                {
                    nameBuilder.Length = 0;
                    nameBuilder.Append("Format").Append(clampedId);
                }

                EnsureFormatSpace(1);
                s_formatList[s_formatCount] = new Format(nameBuilder.ToString(), clampedId);

                return s_formatList[s_formatCount++];
            }
        }

        /// <devdoc>
        /// Ensures that we have enough room in our format list
        /// </devdoc>
        private static void EnsureFormatSpace(int size)
        {
            if (s_formatList == null || s_formatList.Length <= s_formatCount + size)
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

        /// <devdoc>
        /// Ensures that the Win32 predefined formats are setup in our format list.
        /// This is called anytime we need to search the list
        /// </devdoc>
        /// <internalonly/>
        private static void EnsurePredefined()
        {
            if (s_formatCount == 0)
            {
                s_formatList = new Format[]
                {
                    //         Text name        Win32 format ID      Data stored as a Win32 handle?
                    new Format(UnicodeText,  NativeMethods.CF_UNICODETEXT),
                    new Format(Text,         NativeMethods.CF_TEXT),
                    new Format(Bitmap,       NativeMethods.CF_BITMAP),
                    new Format(MetafilePict, NativeMethods.CF_METAFILEPICT),
                    new Format(EnhancedMetafile,  NativeMethods.CF_ENHMETAFILE),
                    new Format(Dif,          NativeMethods.CF_DIF),
                    new Format(Tiff,         NativeMethods.CF_TIFF),
                    new Format(OemText,      NativeMethods.CF_OEMTEXT),
                    new Format(Dib,          NativeMethods.CF_DIB),
                    new Format(Palette,      NativeMethods.CF_PALETTE),
                    new Format(PenData,      NativeMethods.CF_PENDATA),
                    new Format(Riff,         NativeMethods.CF_RIFF),
                    new Format(WaveAudio,    NativeMethods.CF_WAVE),
                    new Format(SymbolicLink, NativeMethods.CF_SYLK),
                    new Format(FileDrop,     NativeMethods.CF_HDROP),
                    new Format(Locale,       NativeMethods.CF_LOCALE)
                };

                s_formatCount = s_formatList.Length;
            }
        }

        /// <devdoc>
        /// Represents a format type.
        /// </devdoc>
        public class Format
        {
            /// <devdoc>
            /// Initializes a new instance of the <see cref='System.Windows.Forms.DataFormats.Format'/>
            /// class and specifies whether a <see langword='Win32 '/> handle is expected with this format.
            /// </devdoc>
            public Format(string name, int id)
            {
                Name = name;
                Id = id;
            }
            
            /// <devdoc>
            /// Specifies the name of this format.
            /// </devdoc>
            public string Name { get; }

            /// <devdoc>
            /// Specifies the ID number for this format.
            /// </devdoc>
            public int Id { get; }
        }
    }
}

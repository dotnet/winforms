// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Text;
    using System.Configuration.Assemblies;
    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Globalization;

    /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats"]/*' />
    /// <devdoc>
    ///    <para>Translates
    ///       between Win Forms text-based
    ///    <see cref='System.Windows.Forms.Clipboard'/> formats and <see langword='Microsoft.Win32'/> 32-bit signed integer-based 
    ///       clipboard formats. Provides <see langword='static '/> methods to create new <see cref='System.Windows.Forms.Clipboard'/> formats and add
    ///       them to the Windows Registry.</para>
    /// </devdoc>
    public class DataFormats {
        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.Text"]/*' />
        /// <devdoc>
        /// <para>Specifies the standard ANSI text format. This <see langword='static '/> 
        /// field is read-only.</para>
        /// </devdoc>
        public static readonly string Text          = "Text";

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.UnicodeText"]/*' />
        /// <devdoc>
        ///    <para>Specifies the standard Windows Unicode text format. This 
        ///    <see langword='static '/>
        ///    field is read-only.</para>
        /// </devdoc>
        public static readonly string UnicodeText   = "UnicodeText";

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.Dib"]/*' />
        /// <devdoc>
        ///    <para>Specifies the Windows Device Independent Bitmap (DIB) 
        ///       format. This <see langword='static '/>
        ///       field is read-only.</para>
        /// </devdoc>
        public static readonly string Dib           = "DeviceIndependentBitmap";

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.Bitmap"]/*' />
        /// <devdoc>
        /// <para>Specifies a Windows bitmap format. This <see langword='static '/> field is read-only.</para>
        /// </devdoc>
        public static readonly string Bitmap        = "Bitmap";

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.EnhancedMetafile"]/*' />
        /// <devdoc>
        ///    <para>Specifies the Windows enhanced metafile format. This 
        ///    <see langword='static '/> field is read-only.</para>
        /// </devdoc>
        public static readonly string EnhancedMetafile   = "EnhancedMetafile";

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.MetafilePict"]/*' />
        /// <devdoc>
        ///    <para>Specifies the Windows metafile format, which Win Forms 
        ///       does not directly use. This <see langword='static '/>
        ///       field is read-only.</para>
        /// </devdoc>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")] // Would be a breaking change to rename this
        public static readonly string MetafilePict  = "MetaFilePict";

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.SymbolicLink"]/*' />
        /// <devdoc>
        ///    <para>Specifies the Windows symbolic link format, which Win 
        ///       Forms does not directly use. This <see langword='static '/>
        ///       field is read-only.</para>
        /// </devdoc>
        public static readonly string SymbolicLink          = "SymbolicLink";

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.Dif"]/*' />
        /// <devdoc>
        ///    <para>Specifies the Windows data interchange format, which Win 
        ///       Forms does not directly use. This <see langword='static '/>
        ///       field is read-only.</para>
        /// </devdoc>
        public static readonly string Dif           = "DataInterchangeFormat";

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.Tiff"]/*' />
        /// <devdoc>
        ///    <para>Specifies the Tagged Image File Format (TIFF), which Win 
        ///       Forms does not directly use. This <see langword='static '/>
        ///       field is read-only.</para>
        /// </devdoc>
        public static readonly string Tiff          = "TaggedImageFileFormat";

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.OemText"]/*' />
        /// <devdoc>
        ///    <para>Specifies the standard Windows original equipment 
        ///       manufacturer (OEM) text format. This <see langword='static '/> field is read-only.</para>
        /// </devdoc>
        public static readonly string OemText       = "OEMText";
        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.Palette"]/*' />
        /// <devdoc>
        /// <para>Specifies the Windows palette format. This <see langword='static '/> 
        /// field is read-only.</para>
        /// </devdoc>
        public static readonly string Palette       = "Palette";

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.PenData"]/*' />
        /// <devdoc>
        ///    <para>Specifies the Windows pen data format, which consists of 
        ///       pen strokes for handwriting software; Win Forms does not use this format. This
        ///    <see langword='static '/> 
        ///    field is read-only.</para>
        /// </devdoc>
        public static readonly string PenData       = "PenData";

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.Riff"]/*' />
        /// <devdoc>
        ///    <para>Specifies the Resource Interchange File Format (RIFF) 
        ///       audio format, which Win Forms does not directly use. This <see langword='static '/> field is read-only.</para>
        /// </devdoc>
        public static readonly string Riff          = "RiffAudio";

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.WaveAudio"]/*' />
        /// <devdoc>
        ///    <para>Specifies the wave audio format, which Win Forms does not 
        ///       directly use. This <see langword='static '/> field is read-only.</para>
        /// </devdoc>
        public static readonly string WaveAudio          = "WaveAudio";

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.FileDrop"]/*' />
        /// <devdoc>
        ///    <para>Specifies the Windows file drop format, which Win Forms 
        ///       does not directly use. This <see langword='static '/>
        ///       field is read-only.</para>
        /// </devdoc>
        public static readonly string FileDrop         = "FileDrop";

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.Locale"]/*' />
        /// <devdoc>
        ///    <para>Specifies the Windows culture format, which Win Forms does 
        ///       not directly use. This <see langword='static '/> field is read-only.</para>
        /// </devdoc>
        public static readonly string Locale        = "Locale";

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.Html"]/*' />
        /// <devdoc>
        ///    <para>Specifies text consisting of HTML data. This 
        ///    <see langword='static '/> field is read-only.</para>
        /// </devdoc>
        public static readonly string Html          = "HTML Format";

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.Rtf"]/*' />
        /// <devdoc>
        ///    <para>Specifies text consisting of Rich Text Format (RTF) data. This 
        ///    <see langword='static '/> field is read-only.</para>
        /// </devdoc>
        public static readonly string Rtf       = "Rich Text Format";

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.CommaSeparatedValue"]/*' />
        /// <devdoc>
        ///    <para>Specifies a comma-separated value (CSV) format, which is a 
        ///       common interchange format used by spreadsheets. This format is not used directly
        ///       by Win Forms. This <see langword='static '/>
        ///       field is read-only.</para>
        /// </devdoc>
        public static readonly string CommaSeparatedValue           = "Csv";

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.StringFormat"]/*' />
        /// <devdoc>
        ///    <para>Specifies the Win Forms string class format, which Win 
        ///       Forms uses to store string objects. This <see langword='static '/>
        ///       field is read-only.</para>
        /// </devdoc>
        public static readonly string StringFormat   = typeof(string).FullName;

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.Serializable"]/*' />
        /// <devdoc>
        ///    <para>Specifies a format that encapsulates any type of Win Forms 
        ///       object. This <see langword='static '/> field is read-only.</para>
        /// </devdoc>
        public static readonly string Serializable     = Application.WindowsFormsVersion + "PersistentObject";

        
        private static Format[] formatList;
        private static int formatCount = 0;
        
        private static object internalSyncObject = new object();

        // not creatable...
        //
        private DataFormats() {
        }

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.GetFormat"]/*' />
        /// <devdoc>
        /// <para>Gets a <see cref='System.Windows.Forms.DataFormats.Format'/> with the Windows Clipboard numeric ID and name for the specified format.</para>
        /// </devdoc>
        public static Format GetFormat(string format) {
            lock(internalSyncObject) {
                EnsurePredefined();

                // It is much faster to do a case sensitive search here.  So do 
                // the case sensitive compare first, then the expensive one.
                //
                for (int n = 0; n < formatCount; n++) {
                    if (formatList[n].Name.Equals(format))
                        return formatList[n];
                }
                
                for (int n = 0; n < formatCount; n++) {
                    if (String.Equals(formatList[n].Name, format, StringComparison.OrdinalIgnoreCase))
                        return formatList[n];
                }
        
                // need to add this format string
                //
                int formatId = SafeNativeMethods.RegisterClipboardFormat(format);
                if (0 == formatId) {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), SR.RegisterCFFailed);
                }
        

                EnsureFormatSpace(1);
                formatList[formatCount] = new Format(format, formatId);
                return formatList[formatCount++];
            }
        }

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.GetFormat1"]/*' />
        /// <devdoc>
        /// <para>Gets a <see cref='System.Windows.Forms.DataFormats.Format'/> with the Windows Clipboard numeric
        ///    ID and name for the specified ID.</para>
        /// </devdoc>
        public static Format GetFormat(int id) {
            // Win32 uses an unsigned 16 bit type as a format ID, thus stripping off the leading bits.
            // Registered format IDs are in the range 0xC000 through 0xFFFF, thus it's important 
            // to represent format as an unsigned type.  
            return InternalGetFormat( null, (ushort)(id & 0xFFFF));
        }

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.InternalGetFormat"]/*' />
        /// <devdoc>
        ///     Allows a the new format name to be specified if the requested format is not
        ///     in the list
        /// </devdoc>
        /// <internalonly/>
        private static Format InternalGetFormat(string strName, ushort id) {
            lock(internalSyncObject) {
                EnsurePredefined();

                for (int n = 0; n < formatCount; n++) {
                    if (formatList[n].Id == id)
                        return formatList[n];
                }

                StringBuilder s = new StringBuilder(128);

                // This can happen if windows adds a standard format that we don't know about,
                // so we should play it safe.
                //
                if (0 == SafeNativeMethods.GetClipboardFormatName(id, s, s.Capacity)) {
                    s.Length = 0;
                    if (strName == null) {
                        s.Append( "Format" ).Append( id );
                    }
                    else {
                        s.Append( strName );
                    }
                }

                EnsureFormatSpace(1);
                formatList[formatCount] = new Format(s.ToString(), id);

                return formatList[formatCount++];
            }
        }


        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.EnsureFormatSpace"]/*' />
        /// <devdoc>
        ///     ensures that we have enough room in our format list
        /// </devdoc>
        /// <internalonly/>
        private static void EnsureFormatSpace(int size) {
            if (null == formatList || formatList.Length <= formatCount + size) {
                int newSize = formatCount + 20;

                Format[] newList = new Format[newSize];

                for (int n = 0; n < formatCount; n++) {
                    newList[n] = formatList[n];
                }
                formatList = newList;
            }                   
        }

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.EnsurePredefined"]/*' />
        /// <devdoc>
        ///     ensures that the Win32 predefined formats are setup in our format list.  This
        ///     is called anytime we need to search the list
        /// </devdoc>
        /// <internalonly/>
        private static void EnsurePredefined() {

            if (0 == formatCount) {
                formatList = new Format [] {
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

                formatCount = formatList.Length;
            }
        }

        /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.Format"]/*' />
        /// <devdoc>
        ///    <para>Represents a format type.</para>
        /// </devdoc>
        public class Format {
            readonly string name;
            readonly int id;
            
            /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.Format.Name"]/*' />
            /// <devdoc>
            ///    <para>
            ///       Specifies the
            ///       name of this format. This field is read-only.
            ///       
            ///    </para>
            /// </devdoc>
            public string Name {
                get {
                    return name;
                }
            }

            /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.Format.Id"]/*' />
            /// <devdoc>
            ///    <para>
            ///       Specifies the ID
            ///       number for this format. This field is read-only.
            ///    </para>
            /// </devdoc>
            public int Id {
                get {
                    return id;
                }
            }

            /// <include file='doc\DataFormats.uex' path='docs/doc[@for="DataFormats.Format.Format"]/*' />
            /// <devdoc>
            /// <para>Initializes a new instance of the <see cref='System.Windows.Forms.DataFormats.Format'/> class and specifies whether a
            /// <see langword='Win32 '/> 
            /// handle is expected with this format.</para>
            /// </devdoc>
            public Format(string name, int id) {
                this.name = name;
                this.id = id;
            }
        }
    }
}

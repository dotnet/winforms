// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Security;

    using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Collections;
    
    /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard"]/*' />
    /// <devdoc>
    ///    <para>Provides methods to place data on and retrieve data from the system clipboard. This class cannot be inherited.</para>
    /// </devdoc>
    public sealed class Clipboard {

        // not creatable...
        //
        private Clipboard() {
        }

        // 
        // Checks the validity of format while setting data into ClipBoard
        //
        private static bool IsFormatValid(DataObject data) {
            return IsFormatValid(data.GetFormats());
        }

        internal static bool IsFormatValid(string[] formats) {

            Debug.Assert(formats != null, "Null returned from GetFormats");
            if (formats != null) {
                if (formats.Length <= 4) {
                    for (int i = 0; i < formats.Length; i++) {
                        switch (formats[i]) {
                            case "Text":
                            case "UnicodeText":
                            case "System.String":
                            case "Csv":
                                break;
                            default:
                                return false;

                        }
                    }
                    return true;
                }
            }
            return false;
        }

        internal static bool IsFormatValid(FORMATETC[] formats) {

            Debug.Assert(formats != null, "Null returned from GetFormats");
            if (formats != null) {
                if (formats.Length <= 4) {
                    for (int i = 0; i < formats.Length; i++) {
                        short format = formats[i].cfFormat;
                        if (format != NativeMethods.CF_TEXT &&
                            format !=  NativeMethods.CF_UNICODETEXT && 
                            format !=  DataFormats.GetFormat("System.String").Id &&
                            format !=  DataFormats.GetFormat("Csv").Id) {
                                return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.SetDataObject"]/*' />
        /// <devdoc>
        /// <para>Places nonpersistent data on the system <see cref='System.Windows.Forms.Clipboard'/>.</para>
        /// </devdoc>
        public static void SetDataObject(object data) {
            SetDataObject(data, false);
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.SetDataObject2"]/*' />
        /// <devdoc>
        /// <para>Overload that uses default values for retryTimes and retryDelay.</para>
        /// </devdoc>
        public static void SetDataObject(object data, bool copy) {
            SetDataObject(data, copy, 10 /*retryTimes*/, 100 /*retryDelay*/);
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.SetDataObject1"]/*' />
        /// <devdoc>
        /// <para>Places data on the system <see cref='System.Windows.Forms.Clipboard'/> and uses copy to specify whether the data 
        ///    should remain on the <see cref='System.Windows.Forms.Clipboard'/>
        ///    after the application exits.</para>
        /// </devdoc>
        [UIPermission(SecurityAction.Demand, Clipboard=UIPermissionClipboard.OwnClipboard)]
        public static void SetDataObject(object data, bool copy, int retryTimes, int retryDelay) {
            if (Application.OleRequired() != System.Threading.ApartmentState.STA) {
                throw new System.Threading.ThreadStateException(SR.ThreadMustBeSTA);
            }

            if (data == null) {
                throw new ArgumentNullException(nameof(data));
            }

            if (retryTimes < 0) {
                throw new ArgumentOutOfRangeException(nameof(retryTimes), string.Format(SR.InvalidLowBoundArgumentEx, "retryTimes", retryTimes.ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
            }

            if (retryDelay < 0) {
                throw new ArgumentOutOfRangeException(nameof(retryDelay), string.Format(SR.InvalidLowBoundArgumentEx, "retryDelay", retryDelay.ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
            }

            
            DataObject dataObject = null;
            if (!(data is IComDataObject)) {
                dataObject = new DataObject(data);
            }

            bool restrictedFormats = false;

            // If the caller doesnt have AllClipBoard permission then we allow only restricted formats.
            // These formats are TEXT, UNICODETEXT,String and CSV
            try 
            {
                IntSecurity.ClipboardRead.Demand();
            }
            catch (SecurityException)
            {
                // We dont have allClipBoard so we can set only data in the following formats
                // TEXT, UNICODETEXT, and CSV.
                restrictedFormats = true;
            }

            // Compute the format of the "data" passed in iff setText == true;
            
            if (restrictedFormats)
            {
                if (dataObject == null)
                {
                    dataObject = data as DataObject;
                }
                if (!IsFormatValid(dataObject))
                {
                    throw new SecurityException(SR.ClipboardSecurityException);
                }
            }

            if (dataObject != null) {
                dataObject.RestrictedFormats = restrictedFormats;
            }
            int hr, retry = retryTimes;
            
            IntSecurity.UnmanagedCode.Assert();

            try
            {
                do {
                    if (data is IComDataObject) {
                        hr = UnsafeNativeMethods.OleSetClipboard((IComDataObject)data);
                    }
                    else {
                        hr = UnsafeNativeMethods.OleSetClipboard(dataObject);
                    }
                    if (hr != 0) {
                        if (retry == 0) {
                            ThrowIfFailed(hr);
                        }
                        retry--;
                        System.Threading.Thread.Sleep(retryDelay /*ms*/);
                    }
                }
                while (hr != 0);

                if (copy) {
                    retry = retryTimes;
                    do {
                        hr = UnsafeNativeMethods.OleFlushClipboard();
                        if (hr != 0) {
                            if (retry == 0) {
                                ThrowIfFailed(hr);
                            }
                            retry--;
                            System.Threading.Thread.Sleep(retryDelay /*ms*/);
                        }
                    }
                    while (hr != 0);
                }
            }
            finally
            {
                CodeAccessPermission.RevertAssert();   
            }
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.GetDataObject"]/*' />
        /// <devdoc>
        ///    <para>Retrieves the data that is currently on the system
        ///    <see cref='System.Windows.Forms.Clipboard'/>.</para>
        /// </devdoc>
        public static IDataObject GetDataObject() {
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "ClipboardRead Demanded");
            IntSecurity.ClipboardRead.Demand();

            if (Application.OleRequired() != System.Threading.ApartmentState.STA) {

                // only throw if a message loop was started. This makes the case of trying
                // to query the clipboard from your finalizer or non-ui MTA thread
                // silently fail, instead of making your app die.
                //
                // however, if you are trying to write a normal windows forms app and 
                // forget to set the STAThread attribute, we will correctly report
                // an error to aid in debugging.
                //
                if (Application.MessageLoop) {
                    throw new System.Threading.ThreadStateException(SR.ThreadMustBeSTA);
                }
                else {
                    return null;
                }
            }
            // We need to retry the GetDataObject() since the clipBaord is busy sometimes and hence the GetDataObject would fail with ClipBoardException.
            return GetDataObject(10 /*retryTimes*/, 100 /*retryDelay*/);
        }

        // Private method to help accessing clipBoard for know retries before failing.
        private static IDataObject GetDataObject(int retryTimes, int retryDelay)
        {
            IComDataObject dataObject = null;
            int hr, retry = retryTimes;
            do {
                hr = UnsafeNativeMethods.OleGetClipboard(ref dataObject);
                if (hr != 0) {
                    if (retry == 0) {
                        ThrowIfFailed(hr);
                    }
                    retry--;
                    System.Threading.Thread.Sleep(retryDelay /*ms*/);
                }
            }
            while (hr != 0);

            if (dataObject != null) {
                if (dataObject is IDataObject && !Marshal.IsComObject(dataObject)) {
                    return (IDataObject)dataObject;
                }
                else {
                    return new DataObject(dataObject);
                }
            }
            return null;
        }

        // <-- WHIDBEY ADDITIONS 

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.Clear"]/*' />
        public static void Clear() {
            Clipboard.SetDataObject(new DataObject());
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.ContainsAudio"]/*' />
        public static bool ContainsAudio() {
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null) {
                return dataObject.GetDataPresent(DataFormats.WaveAudio, false);
            }

            return false;
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.ContainsData"]/*' />
        public static bool ContainsData(string format) {
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null) {
                return dataObject.GetDataPresent(format, false);
            }

            return false;
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.ContainsFileDropList"]/*' />
        public static bool ContainsFileDropList() {
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null) {
                return dataObject.GetDataPresent(DataFormats.FileDrop, true);
            }

            return false;
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.ContainsImage"]/*' />
        public static bool ContainsImage() {
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null) {
                return dataObject.GetDataPresent(DataFormats.Bitmap, true);
            }

            return false;
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.ContainsText"]/*' />
        public static bool ContainsText() {
            if (Environment.OSVersion.Platform != System.PlatformID.Win32NT ||
                Environment.OSVersion.Version.Major < 5) 
            {
                return ContainsText(TextDataFormat.Text);
            }
            else {
                return ContainsText(TextDataFormat.UnicodeText);
            }
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.ContainsText1"]/*' />
        public static bool ContainsText(TextDataFormat format) {
            // valid values are 0x0-0x4 inclusive
            if (!ClientUtils.IsEnumValid(format, (int)format, (int)TextDataFormat.Text, (int)TextDataFormat.CommaSeparatedValue)){
                throw new InvalidEnumArgumentException(nameof(format), (int)format, typeof(TextDataFormat));
            }

            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null) {
                return dataObject.GetDataPresent(ConvertToDataFormats(format), false);
            }

            return false;
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.GetAudioStream"]/*' />
        public static Stream GetAudioStream() {
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null) {
                return dataObject.GetData(DataFormats.WaveAudio, false) as Stream;
            }

            return null;
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.GetData"]/*' />
        public static object GetData(string format) {
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null) {
                return dataObject.GetData(format);
            }

            return null;
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.GetFileDropList"]/*' />
        public static StringCollection GetFileDropList() {
            IDataObject dataObject = Clipboard.GetDataObject();
            StringCollection retVal = new StringCollection();

            if (dataObject != null) {
                string[] strings = dataObject.GetData(DataFormats.FileDrop, true) as string[];
                if (strings != null) {
                    retVal.AddRange(strings);
                }
            }

            return retVal;
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.GetImage"]/*' />
        public static Image GetImage() {
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null) {
                return dataObject.GetData(DataFormats.Bitmap, true) as Image;
            }

            return null;
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.GetText"]/*' />
        public static string GetText() {
            // Pass in Text format for Win98...
            if (Environment.OSVersion.Platform != System.PlatformID.Win32NT ||
                Environment.OSVersion.Version.Major < 5) 
            {
                return GetText(TextDataFormat.Text);
            }
            else {
                return GetText(TextDataFormat.UnicodeText);
            }
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.GetText1"]/*' />
        public static string GetText(TextDataFormat format) {
            // valid values are 0x0 to 0x4 inclusive
            if (!ClientUtils.IsEnumValid(format, (int)format, (int)TextDataFormat.Text, (int)TextDataFormat.CommaSeparatedValue))
            {
                throw new InvalidEnumArgumentException(nameof(format), (int)format, typeof(TextDataFormat));
            }

            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null) {
                string text = dataObject.GetData(ConvertToDataFormats(format), false) as string;
                if (text != null) {
                    return text;
                }
            }

            return String.Empty;
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.SetAudio"]/*' />
        public static void SetAudio(byte[] audioBytes) {
            if (audioBytes == null) {
                throw new ArgumentNullException(nameof(audioBytes));
            }
            SetAudio(new MemoryStream(audioBytes));
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.SetAudio1"]/*' />
        public static void SetAudio(Stream audioStream) {
            if (audioStream == null) {
                throw new ArgumentNullException(nameof(audioStream));
            }
            IDataObject dataObject = new DataObject();
            dataObject.SetData(DataFormats.WaveAudio, false, audioStream);
            Clipboard.SetDataObject(dataObject, true);
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.SetData"]/*' />
        public static void SetData(string format, object data) {
            //Note: We delegate argument checking to IDataObject.SetData, if it wants to do so.
            IDataObject dataObject = new DataObject();
            dataObject.SetData(format, data);
            Clipboard.SetDataObject(dataObject, true);
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.SetFileDropList"]/*' />
        public static void SetFileDropList(StringCollection filePaths) {
            if (filePaths == null) {
                throw new ArgumentNullException(nameof(filePaths));
            }
            // throw Argument exception for zero-length filepath collection.
            if (filePaths.Count == 0)
            {
                throw new ArgumentException(SR.CollectionEmptyException);
            }

            //Validate the paths to make sure they don't contain invalid characters
            foreach (string path in filePaths) {
                try {
                    string temp = Path.GetFullPath(path);
                }
                catch (Exception e) {
                    if (ClientUtils.IsSecurityOrCriticalException(e)) {
                        throw;
                    }

                    throw new ArgumentException(string.Format(SR.Clipboard_InvalidPath, path, "filePaths"), e);
                }
            }

            if (filePaths.Count > 0) {
                IDataObject dataObject = new DataObject();
                string[] strings = new string[filePaths.Count];
                filePaths.CopyTo(strings, 0);
                dataObject.SetData(DataFormats.FileDrop, true, strings);
                Clipboard.SetDataObject(dataObject, true);
            }
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.SetImage"]/*' />
        public static void SetImage(Image image) {
            if (image == null) {
                throw new ArgumentNullException(nameof(image));
            }
            IDataObject dataObject = new DataObject();
            dataObject.SetData(DataFormats.Bitmap, true, image);
            Clipboard.SetDataObject(dataObject, true);
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.SetText"]/*' />
        public static void SetText(string text) {
            // Pass in Text format for Win98...
            if (Environment.OSVersion.Platform != System.PlatformID.Win32NT ||
                Environment.OSVersion.Version.Major < 5) 
            {
                SetText(text, TextDataFormat.Text);
            }
            else {
               SetText(text, TextDataFormat.UnicodeText);
            }
        }

        /// <include file='doc\Clipboard.uex' path='docs/doc[@for="Clipboard.SetText1"]/*' />
        public static void SetText(string text, TextDataFormat format) {
            if (String.IsNullOrEmpty(text)) {
                throw new ArgumentNullException(nameof(text));
            }

            //valid values are 0x0 to 0x4
            if (!ClientUtils.IsEnumValid(format, (int)format, (int)TextDataFormat.Text, (int)TextDataFormat.CommaSeparatedValue))
            {
                throw new InvalidEnumArgumentException(nameof(format), (int)format, typeof(TextDataFormat));
            }

            IDataObject dataObject = new DataObject();
            dataObject.SetData(ConvertToDataFormats(format), false, text);
            Clipboard.SetDataObject(dataObject, true);
        }

        private static string ConvertToDataFormats(TextDataFormat format) {
            switch (format) {
            case TextDataFormat.Text: 
                return DataFormats.Text;
                
            case TextDataFormat.UnicodeText: 
                return DataFormats.UnicodeText;
            
            case TextDataFormat.Rtf: 
                return DataFormats.Rtf;
                
            case TextDataFormat.Html: 
                return DataFormats.Html;
                
            case TextDataFormat.CommaSeparatedValue: 
                return DataFormats.CommaSeparatedValue;
            }

            return DataFormats.UnicodeText;
        }

        // END - WHIDBEY ADDITIONS -->

        private static void ThrowIfFailed(int hr) {
            // 
            if (hr != 0) {
                ExternalException e = new ExternalException(SR.ClipboardOperationFailed, hr);
                throw e;
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using static Interop;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides methods to place data on and retrieve data from the system clipboard. This class cannot be inherited.
    /// </summary>
    public static class Clipboard
    {
        /// <summary>
        ///  Places nonpersistent data on the system <see cref='Clipboard'/>.
        /// </summary>
        public static void SetDataObject(object data)
        {
            SetDataObject(data, false);
        }

        /// <summary>
        ///  Overload that uses default values for retryTimes and retryDelay.
        /// </summary>
        public static void SetDataObject(object data, bool copy)
        {
            SetDataObject(data, copy, retryTimes: 10, retryDelay: 100);
        }

        /// <summary>
        ///  Places data on the system <see cref='Clipboard'/> and uses copy to specify whether the data
        ///  should remain on the <see cref='Clipboard'/> after the application exits.
        /// </summary>
        public static void SetDataObject(object data, bool copy, int retryTimes, int retryDelay)
        {
            if (Application.OleRequired() != ApartmentState.STA)
            {
                throw new Threading.ThreadStateException(SR.ThreadMustBeSTA);
            }

            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (retryTimes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retryTimes), retryTimes, string.Format(SR.InvalidLowBoundArgumentEx, nameof(retryTimes), retryTimes, 0));
            }

            if (retryDelay < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retryDelay), retryDelay, string.Format(SR.InvalidLowBoundArgumentEx, nameof(retryDelay), retryDelay, 0));
            }

            DataObject dataObject = null;
            if (!(data is IComDataObject))
            {
                dataObject = new DataObject(data);
            }

            HRESULT hr;
            int retry = retryTimes;
            do
            {
                if (data is IComDataObject ido)
                {
                    hr = Ole32.OleSetClipboard(ido);
                }
                else
                {
                    hr = Ole32.OleSetClipboard(dataObject);
                }

                if (hr != HRESULT.S_OK)
                {
                    if (retry == 0)
                    {
                        throw new ExternalException(SR.ClipboardOperationFailed, (int)hr);
                    }

                    retry--;
                    Thread.Sleep(millisecondsTimeout: retryDelay);
                }
            }
            while (hr != 0);

            if (copy)
            {
                retry = retryTimes;
                do
                {
                    hr = Ole32.OleFlushClipboard();
                    if (hr != HRESULT.S_OK)
                    {
                        if (retry == 0)
                        {
                            throw new ExternalException(SR.ClipboardOperationFailed, (int)hr);
                        }

                        retry--;
                        Thread.Sleep(millisecondsTimeout: retryDelay);
                    }
                }
                while (hr != 0);
            }
        }

        /// <summary>
        ///  Retrieves the data that is currently on the system <see cref='Clipboard'/>.
        /// </summary>
        public static IDataObject GetDataObject()
        {
            if (Application.OleRequired() != ApartmentState.STA)
            {
                // Only throw if a message loop was started. This makes the case of trying
                // to query the clipboard from your finalizer or non-ui MTA thread
                // silently fail, instead of making your app die.
                //
                // however, if you are trying to write a normal windows forms app and
                // forget to set the STAThread attribute, we will correctly report
                // an error to aid in debugging.
                if (Application.MessageLoop)
                {
                    throw new ThreadStateException(SR.ThreadMustBeSTA);
                }

                return null;
            }

            // We need to retry the GetDataObject() since the clipBaord is busy sometimes and hence the GetDataObject would fail with ClipBoardException.
            return GetDataObject(retryTimes: 10, retryDelay: 100);
        }

        /// <remarks>
        ///  Private method to help accessing clipBoard for know retries before failing.
        /// </remarks>
        private static IDataObject GetDataObject(int retryTimes, int retryDelay)
        {
            IComDataObject dataObject = null;
            HRESULT hr;
            int retry = retryTimes;
            do
            {
                hr = Ole32.OleGetClipboard(ref dataObject);
                if (hr != HRESULT.S_OK)
                {
                    if (retry == 0)
                    {
                        throw new ExternalException(SR.ClipboardOperationFailed, (int)hr);
                    }

                    retry--;
                    Thread.Sleep(millisecondsTimeout: retryDelay);
                }
            }
            while (hr != 0);

            if (dataObject != null)
            {
                if (dataObject is IDataObject ido && !Marshal.IsComObject(dataObject))
                {
                    return ido;
                }

                return new DataObject(dataObject);
            }

            return null;
        }

        public static void Clear()
        {
            Clipboard.SetDataObject(new DataObject());
        }

        public static bool ContainsAudio()
        {
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null)
            {
                return dataObject.GetDataPresent(DataFormats.WaveAudio, false);
            }

            return false;
        }

        public static bool ContainsData(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                return false;
            }

            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null)
            {
                return dataObject.GetDataPresent(format, false);
            }

            return false;
        }

        public static bool ContainsFileDropList()
        {
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null)
            {
                return dataObject.GetDataPresent(DataFormats.FileDrop, true);
            }

            return false;
        }

        public static bool ContainsImage()
        {
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null)
            {
                return dataObject.GetDataPresent(DataFormats.Bitmap, true);
            }

            return false;
        }

        public static bool ContainsText() => ContainsText(TextDataFormat.UnicodeText);

        public static bool ContainsText(TextDataFormat format)
        {
            if (!ClientUtils.IsEnumValid(format, (int)format, (int)TextDataFormat.Text, (int)TextDataFormat.CommaSeparatedValue))
            {
                throw new InvalidEnumArgumentException(nameof(format), (int)format, typeof(TextDataFormat));
            }

            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null)
            {
                return dataObject.GetDataPresent(ConvertToDataFormats(format), false);
            }

            return false;
        }

        public static Stream GetAudioStream()
        {
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null)
            {
                return dataObject.GetData(DataFormats.WaveAudio, false) as Stream;
            }

            return null;
        }

        public static object GetData(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                return null;
            }

            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null)
            {
                return dataObject.GetData(format);
            }

            return null;
        }

        public static StringCollection GetFileDropList()
        {
            IDataObject dataObject = Clipboard.GetDataObject();
            StringCollection retVal = new StringCollection();

            if (dataObject != null)
            {
                if (dataObject.GetData(DataFormats.FileDrop, true) is string[] strings)
                {
                    retVal.AddRange(strings);
                }
            }

            return retVal;
        }

        public static Image GetImage()
        {
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null)
            {
                return dataObject.GetData(DataFormats.Bitmap, true) as Image;
            }

            return null;
        }

        public static string GetText() => GetText(TextDataFormat.UnicodeText);

        public static string GetText(TextDataFormat format)
        {
            if (!ClientUtils.IsEnumValid(format, (int)format, (int)TextDataFormat.Text, (int)TextDataFormat.CommaSeparatedValue))
            {
                throw new InvalidEnumArgumentException(nameof(format), (int)format, typeof(TextDataFormat));
            }

            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null)
            {
                if (dataObject.GetData(ConvertToDataFormats(format), false) is string text)
                {
                    return text;
                }
            }

            return string.Empty;
        }

        public static void SetAudio(byte[] audioBytes)
        {
            if (audioBytes is null)
            {
                throw new ArgumentNullException(nameof(audioBytes));
            }

            SetAudio(new MemoryStream(audioBytes));
        }

        public static void SetAudio(Stream audioStream)
        {
            if (audioStream is null)
            {
                throw new ArgumentNullException(nameof(audioStream));
            }

            IDataObject dataObject = new DataObject();
            dataObject.SetData(DataFormats.WaveAudio, false, audioStream);
            Clipboard.SetDataObject(dataObject, true);
        }

        public static void SetData(string format, object data)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                if (format is null)
                {
                    throw new ArgumentNullException(nameof(format));
                }

                throw new ArgumentException(SR.DataObjectWhitespaceEmptyFormatNotAllowed, nameof(format));
            }

            // Note: We delegate argument checking to IDataObject.SetData, if it wants to do so.
            IDataObject dataObject = new DataObject();
            dataObject.SetData(format, data);
            Clipboard.SetDataObject(dataObject, true);
        }

        public static void SetFileDropList(StringCollection filePaths)
        {
            if (filePaths is null)
            {
                throw new ArgumentNullException(nameof(filePaths));
            }
            if (filePaths.Count == 0)
            {
                throw new ArgumentException(SR.CollectionEmptyException);
            }

            // Validate the paths to make sure they don't contain invalid characters
            foreach (string path in filePaths)
            {
                try
                {
                    Path.GetFullPath(path);
                }
                catch (Exception e) when (!ClientUtils.IsCriticalException(e))
                {
                    throw new ArgumentException(string.Format(SR.Clipboard_InvalidPath, path, "filePaths"), e);
                }
            }

            if (filePaths.Count > 0)
            {
                IDataObject dataObject = new DataObject();
                string[] strings = new string[filePaths.Count];
                filePaths.CopyTo(strings, 0);
                dataObject.SetData(DataFormats.FileDrop, true, strings);
                Clipboard.SetDataObject(dataObject, true);
            }
        }

        public static void SetImage(Image image)
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            IDataObject dataObject = new DataObject();
            dataObject.SetData(DataFormats.Bitmap, true, image);
            Clipboard.SetDataObject(dataObject, true);
        }

        public static void SetText(string text) => SetText(text, TextDataFormat.UnicodeText);

        public static void SetText(string text, TextDataFormat format)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }
            if (!ClientUtils.IsEnumValid(format, (int)format, (int)TextDataFormat.Text, (int)TextDataFormat.CommaSeparatedValue))
            {
                throw new InvalidEnumArgumentException(nameof(format), (int)format, typeof(TextDataFormat));
            }

            IDataObject dataObject = new DataObject();
            dataObject.SetData(ConvertToDataFormats(format), false, text);
            Clipboard.SetDataObject(dataObject, true);
        }

        private static string ConvertToDataFormats(TextDataFormat format)
        {
            switch (format)
            {
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
    }
}

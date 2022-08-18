// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;
using System.Drawing;
using System.Runtime.InteropServices;
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
        ///  Places nonpersistent data on the system <see cref="Clipboard"/>.
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
        ///  Places data on the system <see cref="Clipboard"/> and uses copy to specify whether the data
        ///  should remain on the <see cref="Clipboard"/> after the application exits.
        /// </summary>
        public static void SetDataObject(object data, bool copy, int retryTimes, int retryDelay)
        {
            if (Application.OleRequired() != ApartmentState.STA)
            {
                throw new ThreadStateException(SR.ThreadMustBeSTA);
            }

            ArgumentNullException.ThrowIfNull(data);

            if (retryTimes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retryTimes), retryTimes, string.Format(SR.InvalidLowBoundArgumentEx, nameof(retryTimes), retryTimes, 0));
            }

            if (retryDelay < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retryDelay), retryDelay, string.Format(SR.InvalidLowBoundArgumentEx, nameof(retryDelay), retryDelay, 0));
            }

            DataObject? dataObject = null;
            if (data is not IComDataObject)
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

                if (hr != HRESULT.Values.S_OK)
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
                    if (hr != HRESULT.Values.S_OK)
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
        ///  Retrieves the data that is currently on the system <see cref="Clipboard"/>.
        /// </summary>
        public static IDataObject? GetDataObject()
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

            // We need to retry the GetDataObject() since the clipBoard is busy sometimes and hence the GetDataObject would fail with ClipBoardException.
            return GetDataObject(retryTimes: 10, retryDelay: 100);
        }

        /// <remarks>
        ///  Private method to help accessing clipBoard for know retries before failing.
        /// </remarks>
        private static IDataObject? GetDataObject(int retryTimes, int retryDelay)
        {
            IComDataObject? dataObject = null;
            HRESULT hr;
            int retry = retryTimes;
            do
            {
                hr = Ole32.OleGetClipboard(ref dataObject);
                if (hr != HRESULT.Values.S_OK)
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

            if (dataObject is not null)
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
            SetDataObject(new DataObject());
        }

        public static bool ContainsAudio()
        {
            IDataObject? dataObject = GetDataObject();
            if (dataObject is not null)
            {
                return dataObject.GetDataPresent(DataFormats.WaveAudio, false);
            }

            return false;
        }

        public static bool ContainsData(string? format)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                return false;
            }

            IDataObject? dataObject = GetDataObject();
            if (dataObject is not null)
            {
                return dataObject.GetDataPresent(format, false);
            }

            return false;
        }

        public static bool ContainsFileDropList()
        {
            IDataObject? dataObject = GetDataObject();
            if (dataObject is not null)
            {
                return dataObject.GetDataPresent(DataFormats.FileDrop, true);
            }

            return false;
        }

        public static bool ContainsImage()
        {
            IDataObject? dataObject = GetDataObject();
            if (dataObject is not null)
            {
                return dataObject.GetDataPresent(DataFormats.Bitmap, true);
            }

            return false;
        }

        public static bool ContainsText() => ContainsText(TextDataFormat.UnicodeText);

        public static bool ContainsText(TextDataFormat format)
        {
            SourceGenerated.EnumValidator.Validate(format, nameof(format));

            IDataObject? dataObject = GetDataObject();
            if (dataObject is not null)
            {
                return dataObject.GetDataPresent(ConvertToDataFormats(format), false);
            }

            return false;
        }

        public static Stream? GetAudioStream()
        {
            IDataObject? dataObject = GetDataObject();
            if (dataObject is not null)
            {
                return dataObject.GetData(DataFormats.WaveAudio, false) as Stream;
            }

            return null;
        }

        public static object? GetData(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                return null;
            }

            IDataObject? dataObject = GetDataObject();
            if (dataObject is not null)
            {
                return dataObject.GetData(format);
            }

            return null;
        }

        public static StringCollection GetFileDropList()
        {
            IDataObject? dataObject = GetDataObject();
            StringCollection retVal = new StringCollection();

            if (dataObject is not null)
            {
                if (dataObject.GetData(DataFormats.FileDrop, true) is string[] strings)
                {
                    retVal.AddRange(strings);
                }
            }

            return retVal;
        }

        public static Image? GetImage()
        {
            IDataObject? dataObject = GetDataObject();
            if (dataObject is not null)
            {
                return dataObject.GetData(DataFormats.Bitmap, true) as Image;
            }

            return null;
        }

        public static string GetText() => GetText(TextDataFormat.UnicodeText);

        public static string GetText(TextDataFormat format)
        {
            SourceGenerated.EnumValidator.Validate(format, nameof(format));

            IDataObject? dataObject = GetDataObject();
            if (dataObject is not null)
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
            ArgumentNullException.ThrowIfNull(audioBytes);

            SetAudio(new MemoryStream(audioBytes));
        }

        public static void SetAudio(Stream audioStream)
        {
            ArgumentNullException.ThrowIfNull(audioStream);

            IDataObject dataObject = new DataObject();
            dataObject.SetData(DataFormats.WaveAudio, false, audioStream);
            SetDataObject(dataObject, true);
        }

        public static void SetData(string format, object data)
        {
            ArgumentNullException.ThrowIfNull(format);
            if (string.IsNullOrWhiteSpace(format))
            {
                throw new ArgumentException(SR.DataObjectWhitespaceEmptyFormatNotAllowed, nameof(format));
            }

            // Note: We delegate argument checking to IDataObject.SetData, if it wants to do so.
            IDataObject dataObject = new DataObject();
            dataObject.SetData(format, data);
            SetDataObject(dataObject, true);
        }

        public static void SetFileDropList(StringCollection filePaths)
        {
            ArgumentNullException.ThrowIfNull(filePaths);

            if (filePaths.Count == 0)
            {
                throw new ArgumentException(SR.CollectionEmptyException);
            }

            // Validate the paths to make sure they don't contain invalid characters
            foreach (string? path in filePaths)
            {
                try
                {
                    Path.GetFullPath(path!);
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
                SetDataObject(dataObject, true);
            }
        }

        public static void SetImage(Image image)
        {
            ArgumentNullException.ThrowIfNull(image);

            IDataObject dataObject = new DataObject();
            dataObject.SetData(DataFormats.Bitmap, true, image);
            SetDataObject(dataObject, true);
        }

        public static void SetText(string text) => SetText(text, TextDataFormat.UnicodeText);

        public static void SetText(string text, TextDataFormat format)
        {
            text.ThrowIfNullOrEmpty();
            SourceGenerated.EnumValidator.Validate(format, nameof(format));

            IDataObject dataObject = new DataObject();
            dataObject.SetData(ConvertToDataFormats(format), false, text);
            SetDataObject(dataObject, true);
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

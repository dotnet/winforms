// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods to place data on and retrieve data from the system clipboard. This class cannot be inherited.
/// </summary>
public static class Clipboard
{
    /// <summary>
    ///  Places nonpersistent data on the system <see cref="Clipboard"/>.
    /// </summary>
    public static void SetDataObject(object data) => SetDataObject(data, copy: false);

    /// <summary>
    ///  Overload that uses default values for retryTimes and retryDelay.
    /// </summary>
    public static void SetDataObject(object data, bool copy) =>
        SetDataObject(data, copy, retryTimes: 10, retryDelay: 100);

    /// <summary>
    ///  Places data on the system <see cref="Clipboard"/> and uses copy to specify whether the data
    ///  should remain on the <see cref="Clipboard"/> after the application exits.
    /// </summary>
    public static unsafe void SetDataObject(object data, bool copy, int retryTimes, int retryDelay)
    {
        if (Application.OleRequired() != ApartmentState.STA)
        {
            throw new ThreadStateException(SR.ThreadMustBeSTA);
        }

        ArgumentNullException.ThrowIfNull(data);
        ArgumentOutOfRangeException.ThrowIfNegative(retryTimes);
        ArgumentOutOfRangeException.ThrowIfNegative(retryDelay);

        // Always wrap the data in our DataObject since we know how to retrieve our DataObject from the proxy OleGetClipboard returns.
        DataObject wrappedData = data is DataObject { IsWrappedForClipboard: true } alreadyWrapped
            ? alreadyWrapped
            : new DataObject(data) { IsWrappedForClipboard = true };
        using var dataObject = ComHelpers.GetComScope<Com.IDataObject>(wrappedData);

        HRESULT hr;
        int retry = retryTimes;
        while ((hr = PInvoke.OleSetClipboard(dataObject)).Failed)
        {
            if (--retry < 0)
            {
                throw new ExternalException(SR.ClipboardOperationFailed, (int)hr);
            }

            Thread.Sleep(millisecondsTimeout: retryDelay);
        }

        if (copy)
        {
            retry = retryTimes;
            while ((hr = PInvoke.OleFlushClipboard()).Failed)
            {
                if (--retry < 0)
                {
                    throw new ExternalException(SR.ClipboardOperationFailed, (int)hr);
                }

                Thread.Sleep(millisecondsTimeout: retryDelay);
            }
        }
    }

    /// <summary>
    ///  Retrieves the data that is currently on the system <see cref="Clipboard"/>.
    /// </summary>
    public static unsafe IDataObject? GetDataObject()
    {
        if (Application.OleRequired() != ApartmentState.STA)
        {
            // Only throw if a message loop was started. This makes the case of trying to query the clipboard from the
            // finalizer or non-ui MTA thread silently fail, instead of making the app die.
            return Application.MessageLoop ? throw new ThreadStateException(SR.ThreadMustBeSTA) : null;
        }

        int retryTimes = 10;
        using ComScope<Com.IDataObject> proxyDataObject = new(null);
        HRESULT hr;
        while ((hr = PInvoke.OleGetClipboard(proxyDataObject)).Failed)
        {
            if (--retryTimes < 0)
            {
                throw new ExternalException(SR.ClipboardOperationFailed, (int)hr);
            }

            Thread.Sleep(millisecondsTimeout: 100);
        }

        // OleGetClipboard always returns a proxy. The proxy forwards all IDataObject method calls to the real data object,
        // without giving out the real data object. If the data placed on the clipboard is not one of our CCWs or the clipboard
        // has been flushed, marshal will create a wrapper around the proxy for us to use. However, if the data placed on
        // the clipboard is one of our own and the clipboard has not been flushed, we need to retrieve the real data object
        // pointer in order to retrieve the original managed object via ComWrappers. To do this, we must query for an
        // interface that is not known to the proxy e.g. IComCallableWrapper. If we are able to query for IComCallableWrapper
        // it means that the real data object is one of our CCWs and we've retrieved it successfully,
        // otherwise it is not ours and we will use the wrapped proxy.
        IUnknown* target = default;
        var realDataObject = proxyDataObject.TryQuery<IComCallableWrapper>(out hr);
        if (hr.Succeeded)
        {
            target = realDataObject.AsUnknown;
        }
        else
        {
            target = proxyDataObject.AsUnknown;
        }

        if (!ComHelpers.TryGetObjectForIUnknown(target, out object? managedDataObject))
        {
            target->Release();
            return null;
        }

        if (managedDataObject is not Com.IDataObject.Interface dataObject)
        {
            // We always wrap data set on the Clipboard in a DataObject, so if we do not have
            // a IDataObject.Interface this means built-in com support is turned off and
            // we have a proxy where there is no way to retrieve the original data object
            // pointer from it likely because either the clipboard was flushed or the data on the
            // clipboard is from another process. We need to mimic built-in com behavior and wrap the proxy ourselves.
            // DataObject will ref count proxyDataObject properly to take ownership.
            return new DataObject(proxyDataObject.Value);
        }

        if (dataObject is DataObject { IsWrappedForClipboard: true } wrappedData)
        {
            // There is a DataObject on the clipboard that we placed there. If the real data object
            // implements IDataObject, we want to unwrap it and return it. Otherwise return
            // the DataObject as is.
            return wrappedData.TryUnwrapInnerIDataObject();
        }

        // We did not place the data on the clipboard. Fall back to old behavior.
        return dataObject is IDataObject ido && !Marshal.IsComObject(dataObject)
            ? ido
            : new DataObject(dataObject);
    }

    /// <summary>
    ///  Removes all data from the Clipboard.
    /// </summary>
    public static void Clear() => SetDataObject(new DataObject());

    /// <summary>
    ///  Indicates whether there is data on the Clipboard in the <see cref="DataFormats.WaveAudio"/> format.
    /// </summary>
    public static bool ContainsAudio() => ContainsData(DataFormats.WaveAudioConstant);

    /// <summary>
    ///  Indicates whether there is data on the Clipboard that is in the specified format
    ///  or can be converted to that format.
    /// </summary>
    public static bool ContainsData(string? format) =>
        !string.IsNullOrWhiteSpace(format) && ContainsData(format, autoConvert: false);

    private static bool ContainsData(string format, bool autoConvert) =>
        GetDataObject() is { } dataObject && dataObject.GetDataPresent(format, autoConvert: autoConvert);

    /// <summary>
    ///  Indicates whether there is data on the Clipboard that is in the <see cref="DataFormats.FileDrop"/> format
    ///  or can be converted to that format.
    /// </summary>
    public static bool ContainsFileDropList() => ContainsData(DataFormats.FileDrop, autoConvert: true);

    /// <summary>
    ///  Indicates whether there is data on the Clipboard that is in the <see cref="DataFormats.Bitmap"/> format
    ///  or can be converted to that format.
    /// </summary>
    public static bool ContainsImage() => ContainsData(DataFormats.Bitmap, autoConvert: true);

    /// <summary>
    ///  Indicates whether there is text data on the Clipboard in <see cref="TextDataFormat.UnicodeText"/> format.
    /// </summary>
    public static bool ContainsText() => ContainsText(TextDataFormat.UnicodeText);

    /// <summary>
    ///  Indicates whether there is text data on the Clipboard in the format indicated by the specified
    ///  <see cref="TextDataFormat"/> value.
    /// </summary>
    public static bool ContainsText(TextDataFormat format)
    {
        SourceGenerated.EnumValidator.Validate(format, nameof(format));
        return ContainsData(ConvertToDataFormats(format));
    }

    /// <summary>
    ///  Retrieves an audio stream from the <see cref="Clipboard"/>.
    /// </summary>
    public static Stream? GetAudioStream() => GetData(DataFormats.WaveAudioConstant) as Stream;

    /// <summary>
    ///  Retrieves data from the <see cref="Clipboard"/> in the specified format.
    /// </summary>
    /// <exception cref="ThreadStateException">
    ///  The current thread is not in single-threaded apartment (STA) mode.
    /// </exception>
    public static object? GetData(string format) =>
        string.IsNullOrWhiteSpace(format) ? null : GetData(format, autoConvert: false);

    private static object? GetData(string format, bool autoConvert) =>
        GetDataObject() is { } dataObject ? dataObject.GetData(format, autoConvert) : null;

    /// <summary>
    ///  Retrieves a collection of file names from the <see cref="Clipboard"/>.
    /// </summary>
    public static StringCollection GetFileDropList()
    {
        StringCollection result = [];

        if (GetData(DataFormats.FileDropConstant, autoConvert: true) is string[] strings)
        {
            result.AddRange(strings);
        }

        return result;
    }

    /// <summary>
    ///  Retrieves an image from the <see cref="Clipboard"/>.
    /// </summary>
    public static Image? GetImage() => GetData(DataFormats.Bitmap, autoConvert: true) as Image;

    /// <summary>
    ///  Retrieves text data from the <see cref="Clipboard"/> in the <see cref="TextDataFormat.UnicodeText"/> format.
    /// </summary>
    public static string GetText() => GetText(TextDataFormat.UnicodeText);

    /// <summary>
    ///  Retrieves text data from the <see cref="Clipboard"/> in the format indicated by the specified
    ///  <see cref="TextDataFormat"/> value.
    /// </summary>
    public static string GetText(TextDataFormat format)
    {
        SourceGenerated.EnumValidator.Validate(format, nameof(format));
        return GetData(ConvertToDataFormats(format)) as string ?? string.Empty;
    }

    /// <summary>
    ///  Clears the <see cref="Clipboard"/> and then adds data in the <see cref="DataFormats.WaveAudio"/> format.
    /// </summary>
    public static void SetAudio(byte[] audioBytes) => SetAudio(new MemoryStream(audioBytes.OrThrowIfNull()));

    /// <summary>
    ///  Clears the <see cref="Clipboard"/> and then adds data in the <see cref="DataFormats.WaveAudio"/> format.
    /// </summary>
    public static void SetAudio(Stream audioStream) =>
        SetDataObject(new DataObject(DataFormats.WaveAudioConstant, audioStream.OrThrowIfNull()), copy: true);

    /// <summary>
    ///  Clears the Clipboard and then adds data in the specified format.
    /// </summary>
    public static void SetData(string format, object data)
    {
        if (string.IsNullOrWhiteSpace(format.OrThrowIfNull()))
        {
            throw new ArgumentException(SR.DataObjectWhitespaceEmptyFormatNotAllowed, nameof(format));
        }

        // Note: We delegate argument checking to IDataObject.SetData, if it wants to do so.
        SetDataObject(new DataObject(format, data), copy: true);
    }

    /// <summary>
    ///  Clears the Clipboard and then adds a collection of file names in the <see cref="DataFormats.FileDrop"/> format.
    /// </summary>
    public static void SetFileDropList(StringCollection filePaths)
    {
        if (filePaths.OrThrowIfNull().Count == 0)
        {
            throw new ArgumentException(SR.CollectionEmptyException);
        }

        // Validate the paths to make sure they don't contain invalid characters
        string[] filePathsArray = new string[filePaths.Count];
        filePaths.CopyTo(filePathsArray, 0);

        foreach (string path in filePathsArray)
        {
            // These are the only error states for Path.GetFullPath
            if (string.IsNullOrEmpty(path) || path.Contains('\0'))
            {
                throw new ArgumentException(string.Format(SR.Clipboard_InvalidPath, path, nameof(filePaths)));
            }
        }

        SetDataObject(new DataObject(DataFormats.FileDropConstant, autoConvert: true, filePathsArray), copy: true);
    }

    /// <summary>
    ///  Clears the Clipboard and then adds an <see cref="Image"/> in the <see cref="DataFormats.Bitmap"/> format.
    /// </summary>
    public static void SetImage(Image image) =>
        SetDataObject(new DataObject(DataFormats.BitmapConstant, autoConvert: true, image.OrThrowIfNull()), copy: true);

    /// <summary>
    ///  Clears the Clipboard and then adds text data in the <see cref="TextDataFormat.UnicodeText"/> format.
    /// </summary>
    public static void SetText(string text) => SetText(text, TextDataFormat.UnicodeText);

    /// <summary>
    ///  Clears the Clipboard and then adds text data in the format indicated by the specified
    ///  <see cref="TextDataFormat"/> value.
    /// </summary>
    public static void SetText(string text, TextDataFormat format)
    {
        text.ThrowIfNullOrEmpty();
        SourceGenerated.EnumValidator.Validate(format, nameof(format));
        SetDataObject(new DataObject(ConvertToDataFormats(format), text), copy: true);
    }

    private static string ConvertToDataFormats(TextDataFormat format) => format switch
    {
        TextDataFormat.Text => DataFormats.Text,
        TextDataFormat.UnicodeText => DataFormats.UnicodeText,
        TextDataFormat.Rtf => DataFormats.Rtf,
        TextDataFormat.Html => DataFormats.Html,
        TextDataFormat.CommaSeparatedValue => DataFormats.CommaSeparatedValue,
        _ => DataFormats.UnicodeText,
    };
}

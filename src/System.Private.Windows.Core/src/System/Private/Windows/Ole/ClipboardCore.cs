// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;

namespace System.Private.Windows.Ole;

/// <summary>
///  Contains platform-agnostic clipboard operations.
/// </summary>
internal static unsafe class ClipboardCore<TOleServices>
    where TOleServices : IOleServices
{
    /// <summary>
    ///  The number of times to retry OLE clipboard operations.
    /// </summary>
    private const int OleRetryCount = 10;

    /// <summary>
    ///  The amount of time in milliseconds to sleep between retrying OLE clipboard operations.
    /// </summary>
    private const int OleRetryDelay = 100;

    /// <summary>
    ///  Removes all data from the Clipboard.
    /// </summary>
    /// <returns>An <see cref="HRESULT"/> indicating the success or failure of the operation.</returns>
    internal static HRESULT Clear()
    {
        TOleServices.EnsureThreadState();

        HRESULT result;
        int retryCount = OleRetryCount;

        while ((result = PInvokeCore.OleSetClipboard(null)).Failed)
        {
            if (--retryCount < 0)
            {
                break;
            }

            Thread.Sleep(millisecondsTimeout: OleRetryDelay);
        }

        return result;
    }

    /// <summary>
    ///  Attempts to set the specified data on the Clipboard.
    /// </summary>
    /// <param name="dataObject">The data object to set on the Clipboard.</param>
    /// <param name="copy">Indicates whether to copy the data to the Clipboard.</param>
    /// <param name="retryTimes">The number of times to retry the operation if it fails.</param>
    /// <param name="retryDelay">The amount of time in milliseconds to wait between retries.</param>
    /// <returns>An <see cref="HRESULT"/> indicating the success or failure of the operation.</returns>
    internal static HRESULT SetData(
        IComVisibleDataObject dataObject,
        bool copy,
        int retryTimes = OleRetryCount,
        int retryDelay = OleRetryDelay)
    {
        TOleServices.EnsureThreadState();

        ArgumentOutOfRangeException.ThrowIfNegative(retryTimes);
        ArgumentOutOfRangeException.ThrowIfNegative(retryDelay);

        using var iDataObject = ComHelpers.GetComScope<IDataObject>(dataObject);

        HRESULT result;
        int retry = OleRetryCount;
        while ((result = PInvokeCore.OleSetClipboard(iDataObject)).Failed)
        {
            if (--retry < 0)
            {
                return result;
            }

            Thread.Sleep(millisecondsTimeout: OleRetryDelay);
        }

        if (copy)
        {
            retry = retryTimes;
            while ((result = PInvokeCore.OleFlushClipboard()).Failed)
            {
                if (--retry < 0)
                {
                    return result;
                }

                Thread.Sleep(millisecondsTimeout: retryDelay);
            }
        }

        return result;
    }

    /// <summary>
    ///  Attempts to retrieve data from the Clipboard.
    /// </summary>
    /// <param name="proxyDataObject">The proxy data object retrieved from the Clipboard.</param>
    /// <param name="originalObject">The original object retrieved from the Clipboard, if available.</param>
    /// <returns>An <see cref="HRESULT"/> indicating the success or failure of the operation.</returns>
    public static HRESULT TryGetData(out ComScope<IDataObject> proxyDataObject, out object? originalObject)
    {
        TOleServices.EnsureThreadState();

        proxyDataObject = new(null);
        originalObject = null;

        int retryTimes = OleRetryCount;
        HRESULT result;

        while ((result = PInvokeCore.OleGetClipboard(proxyDataObject)).Failed)
        {
            if (--retryTimes < 0)
            {
                return result;
            }

            Thread.Sleep(millisecondsTimeout: OleRetryDelay);
        }

        // OleGetClipboard always returns a proxy. The proxy forwards all IDataObject method calls to the real data object,
        // without giving out the real data object. If the data placed on the clipboard is not one of our CCWs or the clipboard
        // has been flushed, a wrapper around the proxy for us to use will be given. However, if the data placed on
        // the clipboard is one of our own and the clipboard has not been flushed, we need to retrieve the real data object
        // pointer in order to retrieve the original managed object via ComWrappers if an IDataObject was set on the clipboard.
        // To do this, we must query for an interface that is not known to the proxy e.g. IComCallableWrapper.
        // If we are able to query for IComCallableWrapper it means that the real data object is one of our CCWs and we've retrieved it successfully,
        // otherwise it is not ours and we will use the wrapped proxy.
        using ComScope<IComCallableWrapper> realDataObject = proxyDataObject.TryQuery<IComCallableWrapper>(out HRESULT wrapperResult);

        if (wrapperResult.Succeeded)
        {
            ComHelpers.TryUnwrapComWrapperCCW(realDataObject.AsUnknown, out originalObject);
        }

        return result;
    }

    /// <summary>
    ///  Checks if the specified <paramref name="format"/> is valid and compatible with the specified <paramref name="type"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is intended to be used as a pre-validation step to give a more useful error to callers.
    ///  </para>
    /// </remarks>
    internal static bool IsValidTypeForFormat(Type type, string format)
    {
        if (string.IsNullOrWhiteSpace(format))
        {
            return false;
        }

        if (IsValidPredefinedFormatTypeCombination(format, type))
        {
            return true;
        }

        throw new NotSupportedException(string.Format(
           SR.ClipboardOrDragDrop_InvalidFormatTypeCombination,
           type.FullName,
           format));

        static bool IsValidPredefinedFormatTypeCombination(string format, Type type) => format switch
        {
            DataFormatNames.Text
                or DataFormatNames.UnicodeText
                or DataFormatNames.String
                or DataFormatNames.Rtf
                or DataFormatNames.Html
                or DataFormatNames.OemText => typeof(string) == type,

            DataFormatNames.FileDrop
                or DataFormatNames.FileNameAnsi
                or DataFormatNames.FileNameUnicode => typeof(string[]) == type,

            _ => TOleServices.IsValidTypeForFormat(type, format)
        };
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using Windows.Win32.System.Com;

namespace System.Private.Windows.Ole;

internal static unsafe class ClipboardRetry
{
    [ThreadStatic]
    private static bool s_blockReentrancy;

    internal delegate HRESULT Operation(in ComScope<IDataObject> dataObject);

    internal static HRESULT Execute(
        Operation operation,
        in ComScope<IDataObject> dataObject,
        int retryTimes,
        int retryDelay)
    {
        if (s_blockReentrancy)
        {
            // Sent-message handlers run before the outer clipboard operation can resume.
            // A nested call can replace its data. Retrying inside a delayed-render callback
            // also stalls the reader, which cannot release the clipboard until the callback returns.
            // Only our dispatch is guarded; native OLE callbacks outside it remain unchanged.
            return HRESULT.CLIPBRD_E_CANT_OPEN;
        }

        HRESULT result;
        while ((result = operation(in dataObject)).Failed)
        {
            if (--retryTimes < 0)
            {
                return result;
            }

            HRESULT waitResult = WaitForRetry(retryDelay);
            if (waitResult.Failed)
            {
                return waitResult;
            }
        }

        return result;
    }

    private static HRESULT WaitForRetry(int millisecondsTimeout)
    {
        if (millisecondsTimeout <= 0)
        {
            Thread.Sleep(millisecondsTimeout);
            return HRESULT.S_OK;
        }

        uint start = unchecked((uint)Environment.TickCount);
        uint remaining = (uint)millisecondsTimeout;
        while (true)
        {
            // A clipboard reader can hold the clipboard open while sending us a delayed-render request.
            uint result = (uint)PInvokeCore.MsgWaitForMultipleObjectsEx(
                0,
                null,
                remaining,
                QUEUE_STATUS_FLAGS.QS_SENDMESSAGE,
                MSG_WAIT_FOR_MULTIPLE_OBJECTS_EX_FLAGS.MWMO_INPUTAVAILABLE);
            if (result == (uint)WAIT_EVENT.WAIT_FAILED)
            {
                return HRESULT.FromWin32((WIN32_ERROR)Marshal.GetLastWin32Error());
            }

            if (result == (uint)WAIT_EVENT.WAIT_TIMEOUT)
            {
                return HRESULT.S_OK;
            }

            s_blockReentrancy = true;
            try
            {
                // PeekMessage dispatches all pending sent messages, not just clipboard-render requests.
                // Posted messages stay queued; reentrant clipboard operations are rejected.
                MSG message;
                PInvokeCore.PeekMessage(
                    &message,
                    HWND.Null,
                    0,
                    0,
                    PEEK_MESSAGE_REMOVE_TYPE.PM_NOREMOVE | PEEK_MESSAGE_REMOVE_TYPE.PM_QS_SENDMESSAGE);
            }
            finally
            {
                s_blockReentrancy = false;
            }

            uint elapsed = unchecked((uint)Environment.TickCount - start);
            if (elapsed >= (uint)millisecondsTimeout)
            {
                return HRESULT.S_OK;
            }

            remaining = (uint)millisecondsTimeout - elapsed;
        }
    }
}

/// <summary>
///  Contains platform-agnostic clipboard operations.
/// </summary>
internal static unsafe class ClipboardCore<TOleServices>
#if NET
    where TOleServices : IOleServices
#else
    where TOleServices : IOleServices, new()
#endif
{
#if NETFRAMEWORK
    private static readonly TOleServices s_oleServices = new();
#endif

    /// <summary>
    ///  The number of times to retry OLE clipboard operations.
    /// </summary>
    private const int OleRetryCount = 10;

    /// <summary>
    ///  The amount of time in milliseconds to sleep between retrying OLE clipboard operations.
    /// </summary>
    private const int OleRetryDelay = 100;

    private static HRESULT OleSetClipboard(in ComScope<IDataObject> dataObject)
    {
#if NET
        return TOleServices.OleSetClipboard(dataObject);
#else
        return s_oleServices.OleSetClipboard(dataObject);
#endif
    }

    private static HRESULT OleFlushClipboard(in ComScope<IDataObject> _)
    {
#if NET
        return TOleServices.OleFlushClipboard();
#else
        return s_oleServices.OleFlushClipboard();
#endif
    }

    private static HRESULT OleGetClipboard(in ComScope<IDataObject> dataObject)
    {
#if NET
        return TOleServices.OleGetClipboard(dataObject);
#else
        return s_oleServices.OleGetClipboard(dataObject);
#endif
    }

    /// <summary>
    ///  Removes all data from the Clipboard.
    /// </summary>
    /// <returns>An <see cref="HRESULT"/> indicating the success or failure of the operation.</returns>
    internal static HRESULT Clear(
        int retryTimes = OleRetryCount,
        int retryDelay = OleRetryDelay)
    {
#if NET
        TOleServices.EnsureThreadState();
#else
        s_oleServices.EnsureThreadState();
#endif

        return ClipboardRetry.Execute(OleSetClipboard, default, retryTimes, retryDelay);
    }

    /// <summary>
    ///  Flushes data to the Clipboard.
    /// </summary>
    /// <returns>An <see cref="HRESULT"/> indicating the success or failure of the operation.</returns>
    internal static HRESULT Flush(
        int retryTimes = OleRetryCount,
        int retryDelay = OleRetryDelay)
    {
#if NET
        TOleServices.EnsureThreadState();
#else
        s_oleServices.EnsureThreadState();
#endif

        return ClipboardRetry.Execute(OleFlushClipboard, default, retryTimes, retryDelay);
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
#if NET
        TOleServices.EnsureThreadState();
#else
        s_oleServices.EnsureThreadState();
#endif

        ArgumentOutOfRangeException.ThrowIfNegative(retryTimes);
        ArgumentOutOfRangeException.ThrowIfNegative(retryDelay);

        using var iDataObject = ComHelpers.GetComScope<IDataObject>(dataObject);

        HRESULT result = ClipboardRetry.Execute(OleSetClipboard, in iDataObject, retryTimes, retryDelay);
        if (result.Failed || !copy)
        {
            return result;
        }

        return ClipboardRetry.Execute(OleFlushClipboard, default, retryTimes, retryDelay);
    }

    /// <summary>
    ///  Attempts to retrieve data from the Clipboard.
    /// </summary>
    /// <param name="proxyDataObject">The proxy data object retrieved from the Clipboard.</param>
    /// <param name="originalObject">The original object retrieved from the Clipboard, if available.</param>
    /// <returns>An <see cref="HRESULT"/> indicating the success or failure of the operation.</returns>
    /// <inheritdoc cref="SetData(IComVisibleDataObject, bool, int, int)"/>
    internal static HRESULT TryGetData(
        out ComScope<IDataObject> proxyDataObject,
        out object? originalObject,
        int retryTimes = OleRetryCount,
        int retryDelay = OleRetryDelay)
    {
#if NET
        TOleServices.EnsureThreadState();
#else
        s_oleServices.EnsureThreadState();
#endif

        proxyDataObject = new(null);
        originalObject = null;

        HRESULT result = ClipboardRetry.Execute(OleGetClipboard, in proxyDataObject, retryTimes, retryDelay);
        if (result.Failed)
        {
            return result;
        }

#if NETFRAMEWORK
        // We don't have ComWrappers on .NET Framework, use built-in COM interop to get an RCW, which will be castable
        // to the original object if it came from a .NET interop generated CCW.
        using var unknown = proxyDataObject.Query<IUnknown>();
        originalObject = Marshal.GetObjectForIUnknown((nint)unknown.Value);
        return result;
#else
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
#endif
    }

    /// <summary>
    ///  Returns true if the given <paramref name="object"/> is currently on the Clipboard.
    /// </summary>
    /// <remarks>
    ///  <para>This is meant to emulate the OleIsCurrentClipboard API.</para>
    /// </remarks>
    /// <param name="object">The object to check.</param>
    /// <returns>'true' if <paramref name="object"/> is currently on the Clipboard.</returns>
    /// <inheritdoc cref="SetData(IComVisibleDataObject, bool, int, int)"/>
    internal static bool IsObjectOnClipboard(
        object @object,
        int retryTimes = OleRetryCount,
        int retryDelay = OleRetryDelay)
    {
        if (@object is null)
        {
            return false;
        }

        HRESULT result = TryGetData(
            out ComScope<IDataObject> proxyDataObject,
            out object? originalObject,
            retryTimes,
            retryDelay);

        // Need to ensure we release the ref count on the proxy object.
        using (proxyDataObject)
        {
            return result.Succeeded && @object == originalObject;
        }
    }

    /// <summary>
    ///  Returns the data that is currently on the clipboard as the platform specified <typeparamref name="TIDataObject"/>.
    /// </summary>
    internal static HRESULT GetDataObject<TDataObject, TIDataObject>(
        out TIDataObject? dataObject,
        int retryTimes = OleRetryCount,
        int retryDelay = OleRetryDelay)
        where TDataObject : class, IDataObjectInternal<TDataObject, TIDataObject>, TIDataObject, new()
        where TIDataObject : class
    {
        dataObject = default;

        HRESULT result = TryGetData(
            out ComScope<IDataObject> proxyDataObject,
            out object? originalObject,
            retryTimes,
            retryDelay);

        // Need to ensure we release the ref count on the proxy object.
        using (proxyDataObject)
        {
            if (result.Failed)
            {
                Debug.Assert(proxyDataObject.IsNull);
                return result;
            }

            if (originalObject is TDataObject dataObjectInternal
                && dataObjectInternal.TryUnwrapUserDataObject(out TIDataObject? userObject))
            {
                // We have an original user object that we want to return.
                dataObject = userObject;
                return result;
            }

            // Original data given wasn't an IDataObject, give the proxy value back.
            // (Creating the DataObject will add a reference to the proxy.)
#if NET
            dataObject = TDataObject.Create(proxyDataObject.Value);
#else
            dataObject = DataObjectFactory<TDataObject, TIDataObject>.Instance.Create(proxyDataObject.Value);
#endif
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

#if NET
            _ => TOleServices.IsValidTypeForFormat(type, format)
#else
            _ => s_oleServices.IsValidTypeForFormat(type, format)
#endif
        };
    }

    internal static void SetFileDropList(StringCollection filePaths)
    {
#if NET
        IComVisibleDataObject dataObject = TOleServices.CreateDataObject();
#else
        IComVisibleDataObject dataObject = s_oleServices.CreateDataObject();
#endif
        dataObject.SetFileDropList(filePaths);
        SetData(dataObject, copy: true);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
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

#if NET9_0_OR_GREATER
    private static readonly Lock s_currentDataObjectLock = new();
#else
    private static readonly object s_currentDataObjectLock = new();
#endif
    // Keep the managed owner alive for delayed OLE rendering. This is a bounded, single-owner cache; the shared layer
    // has no clipboard-change notification, so external replacement is detected and released on the next operation.
    // The agile pointer preserves the exact COM identity needed for that ownership check.
    private static IComVisibleDataObject? s_currentDataObject;
    private static AgileComPointer<IDataObject>? s_currentDataObjectPointer;

    /// <summary>
    ///  Removes all data from the Clipboard.
    /// </summary>
    /// <returns>An <see cref="HRESULT"/> indicating the success or failure of the operation.</returns>
    internal static HRESULT Clear(
        int retryTimes = OleRetryCount,
        int retryDelay = OleRetryDelay)
    {
        TOleServices.EnsureThreadState();

        HRESULT result;
        int retryCount = retryTimes;

        while ((result = TOleServices.OleSetClipboard(null)).Failed)
        {
            if (--retryCount < 0)
            {
                break;
            }

            Thread.Sleep(millisecondsTimeout: retryDelay);
        }

        if (result.Succeeded)
        {
            ClearCurrentDataObject();
        }

        return result;
    }

    /// <summary>
    ///  Flushes data to the Clipboard.
    /// </summary>
    /// <returns>An <see cref="HRESULT"/> indicating the success or failure of the operation.</returns>
    internal static HRESULT Flush(
        int retryTimes = OleRetryCount,
        int retryDelay = OleRetryDelay)
    {
        TOleServices.EnsureThreadState();

        HRESULT result;
        int retryCount = retryTimes;

        while ((result = TOleServices.OleFlushClipboard()).Failed)
        {
            if (--retryCount < 0)
            {
                break;
            }

            Thread.Sleep(millisecondsTimeout: retryDelay);
        }

        if (result.Succeeded)
        {
            ClearCurrentDataObject();
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
        int retryCount = retryTimes;
        while ((result = TOleServices.OleSetClipboard(iDataObject)).Failed)
        {
            if (--retryCount < 0)
            {
                return result;
            }

            Thread.Sleep(millisecondsTimeout: retryDelay);
        }

        SetCurrentDataObject(dataObject, iDataObject.Value);

        if (copy)
        {
            retryCount = retryTimes;
            while ((result = TOleServices.OleFlushClipboard()).Failed)
            {
                if (--retryCount < 0)
                {
                    return result;
                }

                Thread.Sleep(millisecondsTimeout: retryDelay);
            }

            ClearCurrentDataObject();
        }

        return result;
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
        TOleServices.EnsureThreadState();

        proxyDataObject = new(null);
        originalObject = null;

        int retryCount = retryTimes;
        HRESULT result;

        while ((result = TOleServices.OleGetClipboard(proxyDataObject)).Failed)
        {
            if (--retryCount < 0)
            {
                return result;
            }

            Thread.Sleep(millisecondsTimeout: retryDelay);
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
    /// <param name="unwrapUserDataObject">
    ///  <see langword="true"/> to return the original managed data object when possible; <see langword="false"/> to
    ///  always return a wrapper around the OLE proxy. WinForms preserves managed object identity, while WPF uses the
    ///  proxy to preserve its legacy Windows format-conversion behavior.
    /// </param>
    internal static HRESULT GetDataObject<TDataObject, TIDataObject>(
        out TIDataObject? dataObject,
        int retryTimes = OleRetryCount,
        int retryDelay = OleRetryDelay,
        bool unwrapUserDataObject = true)
        where TDataObject : class, IDataObjectInternal<TDataObject, TIDataObject>, TIDataObject
        where TIDataObject : class
    {
        TOleServices.EnsureThreadState();

        dataObject = default;

        if (TryGetCurrentDataObject<TDataObject, TIDataObject>(
            unwrapUserDataObject,
            out TIDataObject? currentDataObject))
        {
            dataObject = currentDataObject;
            return HRESULT.S_OK;
        }

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

            if (unwrapUserDataObject
                && originalObject is TDataObject dataObjectInternal
                && dataObjectInternal.TryUnwrapUserDataObject(out TIDataObject? userObject))
            {
                // We have an original user object that we want to return.
                dataObject = userObject;
                return result;
            }

            // Original data given wasn't an IDataObject, give the proxy value back.
            // (Creating the DataObject will add a reference to the proxy.)
            dataObject = TDataObject.Create(proxyDataObject.Value);
        }

        return result;
    }

    private static bool TryGetCurrentDataObject<TDataObject, TIDataObject>(
        bool unwrapUserDataObject,
        [NotNullWhen(true)] out TIDataObject? dataObject)
        where TDataObject : class, IDataObjectInternal<TDataObject, TIDataObject>, TIDataObject
        where TIDataObject : class
    {
        lock (s_currentDataObjectLock)
        {
            if (s_currentDataObject is not TDataObject dataObjectInternal
                || s_currentDataObjectPointer is null)
            {
                dataObject = null;
                return false;
            }

            using ComScope<IDataObject> iDataObject =
                s_currentDataObjectPointer.TryGetInterface(out HRESULT interfaceResult);

            // The originating apartment may have exited, or clipboard ownership may have changed externally.
            // In either case discard the stale cache and fall back to the current OLE clipboard proxy.
            if (interfaceResult.Failed || TOleServices.OleIsCurrentClipboard(iDataObject) != HRESULT.S_OK)
            {
                ClearCurrentDataObjectNoLock();
                dataObject = null;
                return false;
            }

            if (unwrapUserDataObject)
            {
                return dataObjectInternal.TryUnwrapUserDataObject(out dataObject);
            }

            dataObject = null;
            return false;
        }
    }

    private static void SetCurrentDataObject(IComVisibleDataObject dataObject, IDataObject* iDataObject)
    {
        AgileComPointer<IDataObject>? dataObjectPointer = new(iDataObject, takeOwnership: false);
        AgileComPointer<IDataObject>? previousDataObjectPointer = null;

        try
        {
            lock (s_currentDataObjectLock)
            {
                previousDataObjectPointer = s_currentDataObjectPointer;
                s_currentDataObject = dataObject;
                s_currentDataObjectPointer = dataObjectPointer;
                dataObjectPointer = null;
            }
        }
        finally
        {
            // If replacing the previous cache entry fails, do not leak the newly registered GIT cookie.
            dataObjectPointer?.Dispose();

            // Revoking the previous pointer can re-enter SetData. Dispose it only after publishing the replacement so
            // a re-entrant update cannot be overwritten by this call.
            previousDataObjectPointer?.Dispose();
        }
    }

    private static void ClearCurrentDataObject()
    {
        lock (s_currentDataObjectLock)
        {
            ClearCurrentDataObjectNoLock();
        }
    }

    private static void ClearCurrentDataObjectNoLock()
    {
        // Clear the shared state before releasing COM resources because revocation can invoke re-entrant callbacks.
        AgileComPointer<IDataObject>? dataObjectPointer = s_currentDataObjectPointer;
        s_currentDataObjectPointer = null;
        s_currentDataObject = null;
        dataObjectPointer?.Dispose();
    }

    /// <summary>
    ///  Returns whether data in the specified native clipboard format is available or can be synthesized by Windows.
    /// </summary>
    /// <remarks>
    ///  <para>This is consumed by WPF's <c>Clipboard.Contains*</c> APIs through the PresentationCore friend assembly.
    ///  Keeping the P/Invoke here avoids duplicating native clipboard declarations in WPF.</para>
    /// </remarks>
    internal static bool IsClipboardFormatAvailable(uint format) =>
        PInvokeCore.IsClipboardFormatAvailable(format).Value != 0;

    /// <summary>
    ///  Adds shared and platform-specific synonyms for the specified data format.
    /// </summary>
    /// <remarks>
    ///  <para>Platform-specific mappings are kept separate so WPF can restore aliases such as <c>BitmapSource</c> without
    ///  exposing WPF-only formats from WinForms data objects.</para>
    /// </remarks>
    internal static void AddMappedFormats(string format, ICollection<string> formats)
    {
        DataFormatNames.AddMappedFormats(format, formats);
        TOleServices.AddMappedFormats(format, formats);
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

    internal static void SetFileDropList(StringCollection filePaths)
    {
        IComVisibleDataObject dataObject = TOleServices.CreateDataObject();
        dataObject.SetFileDropList(filePaths);
        SetData(dataObject, copy: true);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;

namespace System.Private.Windows.Ole;

/// <summary>
///  Platform specific OLE services.
/// </summary>
internal unsafe interface IOleServices
{
    /// <summary>
    ///  This method is called to validate that OLE is initialized and
    ///  that the current thread is a single-threaded apartment (STA).
    /// </summary>
    /// <exception cref="ThreadStateException">Current thread is not in the right state for OLE.</exception>
    static abstract void EnsureThreadState();

    /// <summary>
    ///  Called after unsuccessfully performing clipboard <see cref="TYMED.TYMED_HGLOBAL"/> serialization.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   <paramref name="pformatetc"/> and <paramref name="pmedium"/> are checked for validity before this call.
    ///  </para>
    /// </remarks>
    /// <param name="format">The data format that is being serialized.</param>
    /// <inheritdoc cref="IDataObject.GetDataHere(FORMATETC*, STGMEDIUM*)"/>
    static abstract HRESULT GetDataHere(string format, object data, FORMATETC* pformatetc, STGMEDIUM* pmedium);

    /// <summary>
    ///  If the <typeparamref name="T"/> is a the requested format this method will attempt to extract it
    ///  from the <paramref name="dataObject"/>.
    /// </summary>
    /// <param name="format">The data format that is being requested.</param>
    /// <remarks>
    ///  <para>
    ///   This is intended for platform specific logic, notably for bitmaps and metafiles.
    ///  </para>
    /// </remarks>
    /// <returns><see langword="true"/> if a bitmap was extracted.</returns>
    static abstract bool TryGetObjectFromDataObject<T>(
        IDataObject* dataObject,
        string format,
        [NotNullWhen(true)] out T data);

    /// <summary>
    ///  Returns true if the given <paramref name="type"/> is a valid type for the given <paramref name="format"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Basic predefined formats that map to <see langword="string"/> are checked before this call.
    ///  </para>
    /// </remarks>
    static abstract bool IsValidTypeForFormat(Type type, string format);

    /// <summary>
    ///  Allows the given <typeparamref name="T"/> to pass pre-validation without a resolver.
    /// </summary>
    static abstract bool AllowTypeWithoutResolver<T>();

    /// <summary>
    ///  Allows custom validation or adapting of <see cref="DataStore{TOleServices}"/> data and formats.
    /// </summary>
    /// <param name="format">
    ///  The format to be checked against <paramref name="data"/>, can be modified to another format name if
    ///  <paramref name="autoConvert"/> is true.
    /// </param>
    /// <param name="data">The data to be checked against <paramref name="data"/>.</param>
    static abstract void ValidateDataStoreData(ref string format, bool autoConvert, object? data);

    /// <summary>
    ///  Creates an <see cref="IComVisibleDataObject"/> instance.
    /// </summary>
    static abstract IComVisibleDataObject CreateDataObject();

    /// <inheritdoc cref="PInvokeCore.OleGetClipboard(IDataObject**)"/>/>
    static abstract HRESULT OleGetClipboard(IDataObject** dataObject);

    /// <inheritdoc cref="PInvokeCore.OleSetClipboard(IDataObject*)"/>
    static abstract HRESULT OleSetClipboard(IDataObject* dataObject);

    /// <inheritdoc cref="PInvokeCore.OleFlushClipboard"/>
    static abstract HRESULT OleFlushClipboard();
}

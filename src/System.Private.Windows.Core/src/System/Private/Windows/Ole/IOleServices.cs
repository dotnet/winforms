// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;

namespace System.Private.Windows.Ole;

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
    static abstract HRESULT GetDataHere(string format, object data, FORMATETC* pformatetc, STGMEDIUM* pmedium);

    /// <summary>
    ///  If the <typeparamref name="T"/> is a bitmap this method will attempt to extract it
    ///  from the <paramref name="dataObject"/>.
    /// </summary>
    /// <returns><see langword="true"/> if a bitmap was extracted.</returns>
    static abstract bool TryGetBitmapFromDataObject<T>(
        IDataObject* dataObject,
        [NotNullWhen(true)] out T data);

    /// <summary>
    ///  Allows the given <typeparamref name="T"/> to pass pre-validation without a resolver.
    /// </summary>
    static abstract bool AllowTypeWithoutResolver<T>();

    /// <summary>
    ///  Allows custom validation or adapting of <see cref="DataStore{TOleServices}"/> data and formats.
    /// </summary>
    static abstract void ValidateDataStoreData(ref string format, bool autoConvert, object? data);
}

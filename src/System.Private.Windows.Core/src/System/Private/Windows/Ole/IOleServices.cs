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
#if NET
    static abstract
#endif
    void EnsureThreadState();

    /// <summary>
    ///  Renders clipboard data into a platform-specific native medium, or acts as a fallback when
    ///  <see cref="TYMED.TYMED_HGLOBAL"/> serialization is not applicable.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   <paramref name="pformatetc"/> and <paramref name="pmedium"/> are checked for validity before this call.
    ///  </para>
    /// </remarks>
    /// <param name="format">The data format that is being serialized.</param>
    /// <inheritdoc cref="IDataObject.GetDataHere(FORMATETC*, STGMEDIUM*)"/>
#if NET
    static abstract
#endif
    HRESULT GetDataHere(string format, object data, FORMATETC* pformatetc, STGMEDIUM* pmedium);

    /// <summary>
    ///  Returns whether the platform can render the specified format using one of the requested native media.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Platform-specific media are excluded by default. This prevents <c>QueryGetData</c> from advertising a
    ///   medium that the platform hook cannot actually render.
    ///  </para>
    /// </remarks>
#if NET
    static virtual bool IsNativeTymedSupported(string format, TYMED tymed) => false;
#else
    bool IsNativeTymedSupported(string format, TYMED tymed);
#endif

    /// <summary>
    ///  Returns whether HGLOBAL takes precedence over the native enhanced-metafile medium when both are requested.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   WPF opts in to preserve .NET 9's HGLOBAL-first EMF behavior, including requests that fail HGLOBAL rendering.
    ///   The default retains the shared adapter's native-first behavior for platforms that opt into native EMF.
    ///   Other native media are unaffected.
    ///  </para>
    /// </remarks>
#if NET
    static virtual bool PreferHGlobalOverEnhMf => false;
#else
    bool PreferHGlobalOverEnhMf { get; }
#endif

    /// <summary>
    ///  Returns whether an object can enter HGLOBAL serialization after stream and standard-format rendering.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is a general object-serialization veto, not an EMF-only check. WPF uses it to reject metafile
    ///   serialization as in .NET 9; the default leaves WinForms serialization unchanged.
    ///  </para>
    ///  <para>
    ///   Returning false rejects the request with DV_E_TYMED without serializing an error payload.
    ///   Raw streams and standard-format conversions have already been handled and are not subject to this veto.
    ///  </para>
    /// </remarks>
#if NET
    static virtual bool AllowHGlobalSerialization(string format, object data) => true;
#else
    bool AllowHGlobalSerialization(string format, object data);
#endif

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
#if NET
    static abstract
#endif
    bool TryGetObjectFromDataObject<T>(
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
#if NET
    static abstract
#endif
    bool IsValidTypeForFormat(Type type, string format);

    /// <summary>
    ///  Allows the given <typeparamref name="T"/> to pass pre-validation without a resolver.
    /// </summary>
#if NET
    static abstract
#endif
    bool AllowTypeWithoutResolver<T>();

    /// <summary>
    ///  Adds platform-specific synonyms for the specified data format.
    /// </summary>
    /// <remarks>
    ///  <para>The default is intentionally empty. Platforms opt in only to mappings that are meaningful to their object
    ///  model; for example, WPF adds its <c>BitmapSource</c> format while WinForms retains the shared mappings.</para>
    /// </remarks>
#if NET
    static virtual void AddMappedFormats(string format, ICollection<string> formats)
    {
    }
#else
    void AddMappedFormats(string format, ICollection<string> formats);
#endif

    /// <summary>
    ///  Allows custom validation or adapting of <see cref="DataStore{TOleServices}"/> data and formats.
    /// </summary>
    /// <param name="format">
    ///  The format to be checked against <paramref name="data"/>, can be modified to another format name if
    ///  <paramref name="autoConvert"/> is true.
    /// </param>
    /// <param name="data">The data to be checked against <paramref name="data"/>.</param>
#if NET
    static abstract
#endif
    void ValidateDataStoreData(ref string format, bool autoConvert, object? data);

    /// <summary>
    ///  Creates an <see cref="IComVisibleDataObject"/> instance.
    /// </summary>
#if NET
    static abstract
#endif
    IComVisibleDataObject CreateDataObject();

    /// <inheritdoc cref="PInvokeCore.OleGetClipboard(IDataObject**)"/>
#if NET
    static abstract
#endif
    HRESULT OleGetClipboard(IDataObject** dataObject);

    /// <inheritdoc cref="PInvokeCore.OleSetClipboard(IDataObject*)"/>
#if NET
    static abstract
#endif
    HRESULT OleSetClipboard(IDataObject* dataObject);

    /// <inheritdoc cref="PInvokeCore.OleIsCurrentClipboard(IDataObject*)"/>
#if NET
    static virtual HRESULT OleIsCurrentClipboard(IDataObject* dataObject) =>
        PInvokeCore.OleIsCurrentClipboard(dataObject);
#else
    HRESULT OleIsCurrentClipboard(IDataObject* dataObject);
#endif

    /// <inheritdoc cref="PInvokeCore.OleFlushClipboard"/>
#if NET
    static abstract
#endif
    HRESULT OleFlushClipboard();
}

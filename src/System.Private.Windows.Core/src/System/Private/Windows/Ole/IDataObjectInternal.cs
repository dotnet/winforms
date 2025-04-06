// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;
using Windows.Win32.System.Com;

namespace System.Private.Windows.Ole;

/// <summary>
///  Internal interface for a data object class.
/// </summary>
/// <remarks>
///  <para>
///   The methods here abstract the platform specific IDataObject and ITypedDataObject interfaces
///   in both WPF and WinForms. It composes the two interfaces.
///  </para>
/// </remarks>
internal interface IDataObjectInternal
{
    /// <summary>
    ///  Retrieves the data associated with the specified data format, using
    ///  <paramref name="autoConvert"/> to determine whether to convert the data to the format.
    /// </summary>
    object? GetData(string format, bool autoConvert);

    /// <summary>
    ///  Retrieves the data associated with the specified data format.
    /// </summary>
    object? GetData(string format);

    /// <summary>
    ///  Retrieves the data associated with the specified class type format.
    /// </summary>
    object? GetData(Type format);

    /// <summary>
    ///  Determines whether data stored in this instance is associated with the
    ///  specified format, using <paramref name="autoConvert"/> to determine whether
    ///  to convert the data to the format.
    /// </summary>
    bool GetDataPresent(string format, bool autoConvert);

    /// <summary>
    ///  Determines whether data stored in this instance is associated with, or
    ///  can be converted to, the specified format.
    /// </summary>
    bool GetDataPresent(string format);

    /// <summary>
    ///  Determines whether data stored in this instance is associated with, or
    ///  can be converted to, the specified format.
    /// </summary>
    bool GetDataPresent(Type format);

    /// <summary>
    ///  Gets a list of all formats that data stored in this instance is associated
    ///  with or can be converted to, using <paramref name="autoConvert"/> to determine
    ///  whether to retrieve all formats that the data can be converted to, or
    ///  only native data formats.
    /// </summary>
    string[] GetFormats(bool autoConvert);

    /// <summary>
    ///  Gets a list of all formats that data stored in this instance is
    ///  associated with or can be converted to.
    /// </summary>
    string[] GetFormats();

    /// <summary>
    ///  Stores the specified data and its associated format in this instance,
    ///  using autoConvert to specify whether the data can be converted to
    ///  another format.
    /// </summary>
    void SetData(string format, bool autoConvert, object? data);

    /// <summary>
    ///  Stores the specified data and its associated format in this instance.
    /// </summary>
    void SetData(string format, object? data);

    /// <summary>
    ///  Stores the specified data and its associated class type in this instance.
    /// </summary>
    void SetData(Type format, object? data);

    /// <summary>
    ///  Stores the specified data in this instance, using the class of the
    ///  data for the format.
    /// </summary>
    void SetData(object? data);

    /// <summary>
    ///  Retrieves data associated with data format named after <typeparamref name="T"/>,
    ///  if that data is of type <typeparamref name="T"/>.
    /// </summary>
    /// <inheritdoc cref="TryGetData{T}(string, Func{TypeName, Type}, bool, out T)"/>
    bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        [NotNullWhen(true), MaybeNullWhen(false)] out T data);

    /// <summary>
    ///  Retrieves data associated with the specified format if that data is of type <typeparamref name="T"/>.
    /// </summary>
    /// <inheritdoc cref="TryGetData{T}(string, Func{TypeName, Type}, bool, out T)"/>
    bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data);

    /// <summary>
    ///  Retrieves data in a specified format if that data is of type <typeparamref name="T"/>,
    ///  optionally converting the data to the specified format.
    /// </summary>
    /// <inheritdoc cref="TryGetData{T}(string, Func{TypeName, Type}, bool, out T)"/>
    bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data);

    /// <summary>
    ///  Retrieves typed data associated with the specified data format.
    /// </summary>
    /// <param name="resolver">
    ///  A user-provided function that maps <see cref="TypeName"/> to <see cref="Type"/>.
    /// </param>
    /// <param name="format">
    ///  A string that specifies what format to retrieve the data as. See the DataFormats class for
    ///  a set of predefined data formats.
    /// </param>
    /// <param name="autoConvert">
    ///  <see langword="true"/> to attempt to automatically convert the data to the specified format;
    ///  <see langword="false"/> for no data format conversion.
    /// </param>
    /// <param name="data">
    ///  A data object with the data in the specified format, or <see langword="default"/> if the data is not available
    ///  in the specified format or is of a wrong type.
    /// </param>
    /// <returns>
    ///  <see langword="true"/> if the data of this format is present and the value is of a matching type and that
    ///  value can be successfully retrieved.
    /// </returns>
    /// <remarks>
    ///  <para>
    ///   The <paramref name="resolver"/> should throw <see cref="NotSupportedException"/> for types it does not
    ///   want loaded. The consuming method should honor this and not load the requested type. The resolver can also
    ///   return <see langword="null"/> to indicate that it wants default handling, which may also result in a
    ///   resolution failure.
    ///  </para>
    /// </remarks>
    bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        Func<TypeName, Type?> resolver,
#pragma warning restore CS3001
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data);
}

/// <summary>
///  Platform typed data object interface. Provides methods to construct and unwrap platform specific data objects.
/// </summary>
internal unsafe interface IDataObjectInternal<TDataObject, TIDataObject> : IDataObjectInternal
    where TDataObject : class, TIDataObject
    where TIDataObject : class
{
    static abstract TDataObject Create();
    static abstract TDataObject Create(IDataObject* dataObject);
    static abstract TDataObject Create(object data);
    static abstract IDataObjectInternal Wrap(TIDataObject data);

    /// <summary>
    ///  Unwraps the user IDataObject instance when applicable.
    /// </summary>
    /// <param name="dataObject">
    ///  <para>
    ///   This is used to return the "original" IDataObject instance when getting data back from OLE. Providing the
    ///   original instance back allows casting to the original type, which historically happened through "magic"
    ///   casting support for built-in COM interop.
    ///  </para>
    ///  <para>
    ///   Now that we use ComWrappers, we can't rely on the "magic" casting support. Instead, we provide the original
    ///   object back when we're able to unwrap it. The unfortunate caveat is that the behavior of calling through
    ///   the OLE IDataObject proxy results in different behavior than calling through the original object. This
    ///   primarily happens through `autoConvert` scenarios, where no such concept exists in the COM interfaces. As
    ///   such, when calling through the COM interface, "autoConvert" is always considered to be <see langword="true"/>.
    ///   To mitigate the COM caveat, we do not give back the original DataObject if we created it implicitly via
    ///   Clipboard.SetData. This allows the calls to go through the proxy, which gets the expected "autoConvert"
    ///   behavior.
    ///  </para>
    ///  <para>
    ///   One potential alternative would be to wrap the created IDataObject in an adapter that mimics the
    ///   "autoConvert" behavior. This would avoid BinaryFormat serialization when in process and not copying.
    ///  </para>
    /// </param>
    /// <returns>true when a data object was returned.</returns>
    bool TryUnwrapUserDataObject([NotNullWhen(true)] out TIDataObject? dataObject);
}

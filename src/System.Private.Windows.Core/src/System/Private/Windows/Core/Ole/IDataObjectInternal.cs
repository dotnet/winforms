// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Private.Windows.Core.Ole;

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
    ///  Determines whether data stored in this instance is  associated with the
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
    ///  <see cref="BinaryFormatter"/> compatible overload that retrieves typed data associated with the specified data format.
    /// </summary>
    /// <param name="resolver">
    ///  A user-provided function that defines a closure of <see cref="Type"/>s that can be retrieved from
    ///  the exchange medium.
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
    ///  A data object with the data in the specified format, or <see langword="null"/> if the data is not available
    ///  in the specified format or is of a wrong type.
    /// </param>
    /// <returns>
    ///  <see langword="true"/> if the data of this format is present and the value is
    ///  of a matching type and that value can be successfully retrieved, or <see langword="false"/>
    ///  if the format is not present or the value is not of the right type.
    /// </returns>
    /// <remarks>
    ///  <para>
    ///   Implement this method for backward compatibility with binary formatted data when binary formatters are enabled.
    ///  </para>
    /// </remarks>
    bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        Func<TypeName, Type> resolver,
#pragma warning restore CS3001
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data);
}

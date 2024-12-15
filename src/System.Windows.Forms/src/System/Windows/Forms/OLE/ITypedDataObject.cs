// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Windows.Forms;

/// <summary>
///  Provides a format-independent mechanism for reading data of a specified <see cref="Type"/>.
/// </summary>
/// <remarks>
///  <para>
///   Implement this interface to use your data object with <see cref="Clipboard.TryGetData{T}(string, out T)"/>
///   family of methods as well as in the drag and drop operations. This interface will ensure that only
///   data of the specified <see cref="Type"/> is exchanged. Otherwise the APIs that specify a <see cref="Type"/> parameter
///   will throw a <see cref="NotSupportedException"/>. This is replacement of <see cref="IDataObject"/>
///   interface, implement this interface as well. Otherwise the APIs that specify a <see cref="Type"/> parameter
///   will throw a <see cref="NotSupportedException"/>.
///  </para>
/// </remarks>
public interface ITypedDataObject : IDataObject
{
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
    ///  A string that specifies what format to retrieve the data as. See the <see cref="DataFormats"/> class for
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

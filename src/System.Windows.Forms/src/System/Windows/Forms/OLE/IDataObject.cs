// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;

namespace System.Windows.Forms;

/// <summary>
///  Provides a format-independent mechanism for transferring data.
/// </summary>
public interface IDataObject
{
    /// <summary>
    ///  Retrieves the data associated with the specified data format, using
    ///  <paramref name="autoConvert"/> to determine whether to convert the data to the  format.
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
    ///  Retrieves the data associated with the specified data format, using
    ///  <paramref name="autoConvert"/> to determine whether to convert the data to the  format,
    ///  if that data is assignable to <typeparamref name="T"/>.
    ///  Will use <paramref name="resolver"/> with the binary formatter if needed.
    ///  <paramref name="resolver"/> is implemented by the user and should return the allowed types or
    ///  throw a <see cref="NotSupportedException"/>.
    /// </summary>
    bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        Func<TypeName, Type> resolver,
#pragma warning restore CS3001
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        data = default;
        if (GetData(format, autoConvert) is T result)
        {
            data = result;
            return true;
        }

        return false;
    }

    /// <summary>
    ///  Retrieves the data associated with the specified data format, using
    ///  <paramref name="autoConvert"/> to determine whether to convert the data to another format,
    ///  if that data is of type <typeparamref name="T"/>.
    /// </summary>
    bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        data = default;
        if (GetData(format, autoConvert) is T result)
        {
            data = result;
            return true;
        }

        return false;
    }

    /// <summary>
    ///  Retrieves the data associated with the specified data format if that data is of type <typeparamref name="T"/>.
    /// </summary>
    bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        data = default;
        if (GetData(format) is T result)
        {
            data = result;
            return true;
        }

        return false;
    }

    /// <summary>
    ///  Retrieves the data associated with data format named after <typeparamref name="T"/>,
    ///  if that data is of type <typeparamref name="T"/>.
    /// </summary>
    bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        data = default;
        if (GetData(typeof(T)) is T result)
        {
            data = result;
            return true;
        }

        return false;
    }

    /// <summary>
    ///  Determines whether data stored in this instance is  associated with the
    ///  specified format, using autoConvert to determine whether to convert the
    ///  data to the format.
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
    ///  Gets a list of all formats that data stored in this instance is
    ///  associated with or can be converted to, using <paramref name="autoConvert"/> to determine
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
}

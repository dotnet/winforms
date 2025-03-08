// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Ole;

namespace System.Windows.Forms;

/// <summary>
///  Provides a format-independent mechanism for transferring data.
/// </summary>
/// <remarks>
///  <para>
///   When implementing a <see cref="IDataObject"/>, consider implementing <see cref="ITypedDataObject"/>
///   interface instead. This interface will ensure that only data of a specified <see cref="Type"/>
///   is exchanged. If <see cref="ITypedDataObject"/> is not implemented by a data object exchanged
///   in the clipboard or drag and drop scenarios, the APIs that specify a <see cref="Type"/>,
///   such as <see cref="Clipboard.TryGetData{T}(string, out T)"/>, will throw a <see cref="NotSupportedException"/>.
///  </para>
/// </remarks>
public interface IDataObject
{
    /// <inheritdoc cref="IDataObjectInternal.GetData(string, bool)" />
    object? GetData(string format, bool autoConvert);

    /// <inheritdoc cref="IDataObjectInternal.GetData(string)" />
    object? GetData(string format);

    /// <inheritdoc cref="IDataObjectInternal.GetData(Type)" />
    object? GetData(Type format);

    /// <inheritdoc cref="IDataObjectInternal.GetDataPresent(string, bool)" />
    bool GetDataPresent(string format, bool autoConvert);

    /// <inheritdoc cref="IDataObjectInternal.GetDataPresent(string)" />
    bool GetDataPresent(string format);

    /// <inheritdoc cref="IDataObjectInternal.GetDataPresent(Type)" />
    bool GetDataPresent(Type format);

    /// <inheritdoc cref="IDataObjectInternal.GetFormats(bool)" />
    string[] GetFormats(bool autoConvert);

    /// <inheritdoc cref="IDataObjectInternal.GetFormats()" />
    string[] GetFormats();

    /// <inheritdoc cref="IDataObjectInternal.SetData(string, bool, object?)" />
    void SetData(string format, bool autoConvert, object? data);

    /// <inheritdoc cref="IDataObjectInternal.SetData(string, object?)" />
    void SetData(string format, object? data);

    /// <inheritdoc cref="IDataObjectInternal.SetData(Type, object?)" />
    void SetData(Type format, object? data);

    /// <inheritdoc cref="IDataObjectInternal.SetData(object?)" />
    void SetData(object? data);
}

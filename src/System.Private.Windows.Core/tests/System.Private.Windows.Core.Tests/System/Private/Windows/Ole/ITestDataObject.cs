// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Ole;

/// <summary>
///  This is meant to emulate the public IDataObject interface for DataObjectCore.
/// </summary>
public interface ITestDataObject
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

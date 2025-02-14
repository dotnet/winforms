// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Ole;

internal static unsafe class DataObjectCore<TDataObject>
    where TDataObject : IComVisibleDataObject
{
    /// <summary>
    ///  JSON serialize the data only if the format is not a restricted deserialization format and the data is not an intrinsic type.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> or <paramref name="format"/> is null.</exception>
    /// <exception cref="ArgumentException">
    ///  <paramref name="format"/> is empty, whitespace, or a predefined format -or- <paramref name="data"/> isa a DataObject.
    /// </exception>
    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.SerializeToUtf8Bytes<TValue>(TValue, JsonSerializerOptions)")]
    internal static IJsonData TryJsonSerialize<T>(string format, T data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(format);
        ArgumentNullException.ThrowIfNull(data);

        if (DataFormatNames.IsPredefinedFormat(format))
        {
            throw new ArgumentException(SR.ClipboardOrDragDrop_CannotJsonSerializePredefinedFormat, nameof(format));
        }

        if (typeof(T).IsAssignableTo(typeof(TDataObject)))
        {
            throw new ArgumentException(SR.ClipboardOrDragDrop_CannotJsonSerializeDataObject, nameof(data));
        }

        return IJsonData.Create(data);
    }
}

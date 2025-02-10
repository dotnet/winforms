// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Private.Windows.Ole;

internal static unsafe class DataObjectCore<TRuntime, TDataFormat, TDataObject, TIDataObject>
    where TRuntime : IRuntime<TDataFormat>
    where TDataFormat : IDataFormat<TDataFormat>
    where TDataObject : IComVisibleDataObject
{
    /// <summary>
    ///  JSON serialize the data only if the format is not a restricted deserialization format and the data is not an intrinsic type.
    /// </summary>
    /// <returns>
    ///  The passed in <paramref name="data"/> as is if the format is restricted. Otherwise the JSON serialized <paramref name="data"/>.
    /// </returns>
    internal static object TryJsonSerialize<T>(string format, T data)
    {
        if (string.IsNullOrWhiteSpace(format.OrThrowIfNull()))
        {
            throw new ArgumentException(SR.DataObjectWhitespaceEmptyFormatNotAllowed, nameof(format));
        }

        data.OrThrowIfNull(nameof(data));

        if (typeof(T) == typeof(TDataObject))
        {
            throw new InvalidOperationException(string.Format(SR.ClipboardOrDragDrop_CannotJsonSerializeDataObject, "SetData"));
        }

        return DataFormatNames.IsRestrictedFormat(format) || TRuntime.IsSupportedType<T>()
            ? data
            : new JsonData<T>() { JsonBytes = JsonSerializer.SerializeToUtf8Bytes(data) };
    }

    internal static Composition<TRuntime, TDataFormat> CreateComposition() =>
        Composition<TRuntime, TDataFormat>.CreateFromManagedDataObject(new DataStore<TRuntime>());

    internal static Composition<TRuntime, TDataFormat> CreateComposition(IDataObject* data) =>
        Composition<TRuntime, TDataFormat>.CreateFromNativeDataObject(data);

    internal static Composition<TRuntime, TDataFormat> CreateComposition(
        object data,
        Func<TIDataObject, IDataObjectInternal> adapterFactory)
    {
        if (data is IDataObjectInternal internalDataObject)
        {
            return Composition<TRuntime, TDataFormat>.CreateFromManagedDataObject(internalDataObject);
        }
        else if (data is TIDataObject iDataObject)
        {
            return Composition<TRuntime, TDataFormat>.CreateFromManagedDataObject(adapterFactory(iDataObject));
        }
        else if (data is ComTypes.IDataObject comDataObject)
        {
            return Composition<TRuntime, TDataFormat>.CreateFromRuntimeDataObject(comDataObject);
        }

        var composition = Composition<TRuntime, TDataFormat>.CreateFromManagedDataObject(new DataStore<TRuntime>());
        composition.SetData(data);
        return composition;
    }
}

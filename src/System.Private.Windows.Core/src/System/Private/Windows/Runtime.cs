// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Formats.Nrbf;
using System.Private.Windows.Nrbf;
using System.Private.Windows.Ole;
using System.Reflection.Metadata;
using Windows.Win32.System.Com;

namespace System.Private.Windows;

/// <summary>
///  Abstract runtime implementation base class that provides the necessary services for the platform.
/// </summary>
/// <typeparam name="TDataFormat">The platform specific DataFormat struct type.</typeparam>
/// <typeparam name="TNrbfSerializer">The platform specific NrbfSerializer type.</typeparam>
/// <typeparam name="TOleServices">The platform specific OLE services type.</typeparam>
internal abstract unsafe class Runtime<TDataFormat, TNrbfSerializer, TOleServices>
    : IRuntime<TDataFormat>
    where TDataFormat : IDataFormat<TDataFormat>
    where TNrbfSerializer : INrbfSerializer
    where TOleServices : IOleServices
{
    static void IOleServices.EnsureThreadState() => TOleServices.EnsureThreadState();
    static HRESULT IOleServices.GetDataHere(string format, object data, FORMATETC* pformatetc, STGMEDIUM* pmedium) =>
        TOleServices.GetDataHere(format, data, pformatetc, pmedium);
    static bool IOleServices.TryGetBitmapFromDataObject<T>(IDataObject* dataObject, [NotNullWhen(true)] out T data) =>
        TOleServices.TryGetBitmapFromDataObject(dataObject, out data);
    static bool IOleServices.AllowTypeWithoutResolver<T>() => TOleServices.AllowTypeWithoutResolver<T>();
    static void IOleServices.ValidateDataStoreData(ref string format, bool autoConvert, object? data) =>
        TOleServices.ValidateDataStoreData(ref format, autoConvert, data);
    static bool INrbfSerializer.IsSupportedType<T>() =>
        TNrbfSerializer.IsSupportedType<T>();
    static bool INrbfSerializer.TryBindToType(TypeName typeName, [NotNullWhen(true)] out Type? type) =>
        TNrbfSerializer.TryBindToType(typeName, out type);
    static bool INrbfSerializer.TryGetObject(SerializationRecord record, [NotNullWhen(true)] out object? value) =>
        TNrbfSerializer.TryGetObject(record, out value);
    static bool INrbfSerializer.TryWriteObject(Stream stream, object value) =>
        TNrbfSerializer.TryWriteObject(stream, value);
}

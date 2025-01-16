// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Core.OLE;
using System.Reflection.Metadata;

namespace System.Windows.Forms;

/// <summary>
///  Adapter class so that our internal OLE classes can understand public IDataObject interfaces.
/// </summary>
internal class DataObjectAdapter : ITypedDataObjectDesktop
{
    public DataObjectAdapter(IDataObject original)
    {
        OriginalDataObject = original.OrThrowIfNull();
    }

    internal IDataObject OriginalDataObject { get; init; }

    object? IDataObjectDesktop.GetData(string format, bool autoConvert) => OriginalDataObject.GetData(format, autoConvert);
    object? IDataObjectDesktop.GetData(string format) => OriginalDataObject.GetData(format);
    object? IDataObjectDesktop.GetData(Type format) => OriginalDataObject.GetData(format);
    bool IDataObjectDesktop.GetDataPresent(string format, bool autoConvert) => OriginalDataObject.GetDataPresent(format, autoConvert);
    bool IDataObjectDesktop.GetDataPresent(string format) => OriginalDataObject.GetDataPresent(format);
    bool IDataObjectDesktop.GetDataPresent(Type format) => OriginalDataObject.GetDataPresent(format);
    string[] IDataObjectDesktop.GetFormats(bool autoConvert) => OriginalDataObject.GetFormats(autoConvert);
    string[] IDataObjectDesktop.GetFormats() => OriginalDataObject.GetFormats();
    void IDataObjectDesktop.SetData(string format, bool autoConvert, object? data) => OriginalDataObject.SetData(format, autoConvert, data);
    void IDataObjectDesktop.SetData(string format, object? data) => OriginalDataObject.SetData(format, data);
    void IDataObjectDesktop.SetData(Type format, object? data) => OriginalDataObject.SetData(format, data);
    void IDataObjectDesktop.SetData(object? data) => OriginalDataObject.SetData(data);

    public bool TryGetData<[DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes)(-1))] T>([MaybeNullWhen(false), NotNullWhen(true)] out T data) => OriginalDataObject.TryGetData(out data);
    public bool TryGetData<[DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes)(-1))] T>(string format, [MaybeNullWhen(false), NotNullWhen(true)] out T data) => OriginalDataObject.TryGetData(format, out data);
    public bool TryGetData<[DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes)(-1))] T>(string format, bool autoConvert, [MaybeNullWhen(false), NotNullWhen(true)] out T data) => OriginalDataObject.TryGetData(format, autoConvert, out data);
    public bool TryGetData<[DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes)(-1))] T>(string format, Func<TypeName, Type> resolver, bool autoConvert, [MaybeNullWhen(false), NotNullWhen(true)] out T data) => OriginalDataObject.TryGetData(format, resolver, autoConvert, out data);
}

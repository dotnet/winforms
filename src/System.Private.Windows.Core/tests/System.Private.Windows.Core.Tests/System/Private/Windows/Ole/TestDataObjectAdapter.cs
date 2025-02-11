// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;

namespace System.Private.Windows.Ole;

internal class TestDataObjectAdapter : ITestDataObject, IDataObjectInternal
{
    internal ITestDataObject DataObject { get; }

    internal TestDataObjectAdapter(ITestDataObject dataObject) => DataObject = dataObject;

    internal static IDataObjectInternal Create(ITestDataObject dataObject) => new TestDataObjectAdapter(dataObject);

    public object? GetData(string format, bool autoConvert) => DataObject.GetData(format, autoConvert);
    public object? GetData(string format) => DataObject.GetData(format);
    public object? GetData(Type format) => DataObject.GetData(format);
    public bool GetDataPresent(string format, bool autoConvert) => DataObject.GetDataPresent(format, autoConvert);
    public bool GetDataPresent(string format) => DataObject.GetDataPresent(format);
    public bool GetDataPresent(Type format) => DataObject.GetDataPresent(format);
    public string[] GetFormats(bool autoConvert) => DataObject.GetFormats(autoConvert);
    public string[] GetFormats() => DataObject.GetFormats();
    public void SetData(string format, bool autoConvert, object? data) => DataObject.SetData(format, autoConvert, data);
    public void SetData(string format, object? data) => DataObject.SetData(format, data);
    public void SetData(Type format, object? data) => DataObject.SetData(format, data);
    public void SetData(object? data) => DataObject.SetData(data);

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        [MaybeNullWhen(false), NotNullWhen(true)] out T data) =>
        GetTypedDataObjectOrThrow(DataObject).TryGetData(out data);
    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        [MaybeNullWhen(false), NotNullWhen(true)] out T data) =>
        GetTypedDataObjectOrThrow(DataObject).TryGetData(format, out data);
    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
        GetTypedDataObjectOrThrow(DataObject).TryGetData(format, autoConvert, out data);
    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        Func<TypeName, Type?> resolver,
        bool autoConvert,
        [MaybeNullWhen(false), NotNullWhen(true)] out T data) =>
        GetTypedDataObjectOrThrow(DataObject).TryGetData(format, resolver, autoConvert, out data);

    private static IDataObjectInternal GetTypedDataObjectOrThrow(ITestDataObject dataObject)
    {
        // (If we add the analagous ITestTypedDataObject, this should be updated to use that instead)

        // Mimic the behavior of DataObjectExtensions
        ArgumentNullException.ThrowIfNull(dataObject);

        if (dataObject is not IDataObjectInternal typed)
        {
            throw new NotSupportedException();
        }

        return typed;
    }
}

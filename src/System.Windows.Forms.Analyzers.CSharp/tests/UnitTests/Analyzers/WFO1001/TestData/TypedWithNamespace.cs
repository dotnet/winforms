using System.Windows;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using System.Runtime.Versioning;

namespace System.Windows.Forms.Analyzers.CSharp.Tests;

internal class TypedWithNamespace : Forms.ITypedDataObject
{
    public object? GetData(string format, bool autoConvert) => null;
    public object? GetData(string format) => null;
    public object? GetData(Type format) => null;
    public bool GetDataPresent(string format, bool autoConvert) => false;
    public bool GetDataPresent(string format) => false;
    public bool GetDataPresent(Type format) => false;
    public string[] GetFormats(bool autoConvert) => ["thing1"];
    public string[] GetFormats() => ["thing1"];
    public void SetData(string format, bool autoConvert, object? data) { }
    public void SetData(string format, object? data) { }
    public void SetData(Type format, object? data) { }
    public void SetData(object? data) { }

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>([MaybeNullWhen(false), NotNullWhen(true)] out T data) =>
    throw new NotImplementedException();
    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string format, [MaybeNullWhen(false), NotNullWhen(true)] out T data) =>
        throw new NotImplementedException();
    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string format, bool autoConvert, [MaybeNullWhen(false), NotNullWhen(true)] out T data) =>
        throw new NotImplementedException();
    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string format, Func<TypeName, Type> resolver, bool autoConvert, [MaybeNullWhen(false), NotNullWhen(true)] out T data) =>
        throw new NotImplementedException();
}

using System.Windows;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using System.Runtime.Versioning;

namespace System.Windows.Forms.Analyzers.CSharp.Tests;

internal class UntypedWithNamespace : Forms.IDataObject
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
}

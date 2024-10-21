// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;

namespace System.Windows.Forms.Tests;

#nullable enable

// Test default implementation of IDataObject.TryGetData<T> overloads.
public partial class IDataObjectTests
{
    [Fact]
    public void IDataObject_TryGetData_Invoke_ReturnsFalse()
    {
        DefaultTryGetMethodsDataObject dataObject = new();
        ((IDataObject)dataObject).TryGetData(out string? _).Should().BeFalse();
        dataObject.GetDataType_Count.Should().Be(1);
        dataObject.GetDataString_Count.Should().Be(0);
        dataObject.GetDataStringBool_Count.Should().Be(0);
    }

    [Fact]
    public void IDataObject_TryGetData_InvokeString_ReturnsFalse()
    {
        DefaultTryGetMethodsDataObject dataObject = new();
        ((IDataObject)dataObject).TryGetData("TryGetDataString", out string? _).Should().BeFalse();
        dataObject.GetDataType_Count.Should().Be(0);
        dataObject.GetDataString_Count.Should().Be(1);
        dataObject.GetDataStringBool_Count.Should().Be(0);
    }

    [Theory]
    [BoolData]
    public void IDataObject_TryGetData_InvokeStringBool_ReturnsFalse(bool autoConvert)
    {
        DefaultTryGetMethodsDataObject dataObject = new();
        ((IDataObject)dataObject).TryGetData("TryGetDataStringBool", autoConvert, out string? _).Should().BeFalse();
        dataObject.GetDataType_Count.Should().Be(0);
        dataObject.GetDataString_Count.Should().Be(0);
        dataObject.GetDataStringBool_Count.Should().Be(1);
    }

    [Theory]
    [BoolData]
    public void IDataObject_TryGetData_InvokeStringResolverBool_ReturnsFalse(bool autoConvert)
    {
        DefaultTryGetMethodsDataObject dataObject = new();
        ((IDataObject)dataObject).TryGetData("TryGetDataStringBool", Resolver, autoConvert, out string? _).Should().BeFalse();
        dataObject.GetDataType_Count.Should().Be(0);
        dataObject.GetDataString_Count.Should().Be(0);
        dataObject.GetDataStringBool_Count.Should().Be(1);
    }

    [Fact]
    public void IDataObject_TryGetData_Invoke_ReturnsTrue()
    {
        DefaultTryGetMethodsDataObject dataObject = new();
        ((IDataObject)dataObject).TryGetData(out TryGetData1? _).Should().BeTrue();
        dataObject.GetDataType_Count.Should().Be(1);
        dataObject.GetDataString_Count.Should().Be(0);
        dataObject.GetDataStringBool_Count.Should().Be(0);
    }

    [Fact]
    public void IDataObject_TryGetData_InvokeString_ReturnsTrue()
    {
        DefaultTryGetMethodsDataObject dataObject = new();
        ((IDataObject)dataObject).TryGetData("TryGetDataString", out TryGetDataString? _).Should().BeTrue();
        dataObject.GetDataType_Count.Should().Be(0);
        dataObject.GetDataString_Count.Should().Be(1);
        dataObject.GetDataStringBool_Count.Should().Be(0);
    }

    [Theory]
    [BoolData]
    public void IDataObject_TryGetData_InvokeStringBool_ReturnsTrue(bool autoConvert)
    {
        DefaultTryGetMethodsDataObject dataObject = new();
        ((IDataObject)dataObject).TryGetData("TryGetDataStringBool", autoConvert, out TryGetDataStringBool? _).Should().BeTrue();
        dataObject.GetDataType_Count.Should().Be(0);
        dataObject.GetDataString_Count.Should().Be(0);
        dataObject.GetDataStringBool_Count.Should().Be(1);
    }

    [Theory]
    [BoolData]
    public void IDataObject_TryGetData_InvokeStringResolverBool_ReturnsTrue(bool autoConvert)
    {
        DefaultTryGetMethodsDataObject dataObject = new();
        ((IDataObject)dataObject).TryGetData("TryGetDataStringBool", Resolver, autoConvert, out TryGetDataStringBool? _).Should().BeTrue();
        dataObject.GetDataType_Count.Should().Be(0);
        dataObject.GetDataString_Count.Should().Be(0);
        dataObject.GetDataStringBool_Count.Should().Be(1);
    }

    private static Type Resolver(TypeName typeName) => throw new NotImplementedException();

    internal class TryGetData1() { }
    private class TryGetDataString() { }
    private class TryGetDataStringBool() { }

    internal class DefaultTryGetMethodsDataObject : IDataObject
    {
        internal int GetDataStringBool_Count { get; set; }
        /// <devdoc>
        ///  Invoked from  <see cref="IDataObject.TryGetData{T}(string, bool, out T)"/>
        ///  and from <see cref="IDataObject.TryGetData{T}(string, Func{TypeName, Type}, bool, out T)"/>.
        /// </devdoc>
        public object? GetData(string format, bool autoConvert)
        {
            GetDataStringBool_Count++;
            return format == "TryGetDataStringBool" ? new TryGetDataStringBool() : null;
        }

        internal int GetDataString_Count { get; set; }
        /// <devdoc>
        ///  Invoked from  <see cref="IDataObject.TryGetData{T}(string, out T)"/>
        /// </devdoc>
        public object? GetData(string format)
        {
            GetDataString_Count++;
            return format == "TryGetDataString" ? new TryGetDataString() : null;
        }

        internal int GetDataType_Count { get; set; }
        /// <devdoc>
        ///  Invoked from  <see cref="IDataObject.TryGetData{T}(out T)"/>
        /// </devdoc>
        public object? GetData(Type format)
        {
            GetDataType_Count++;
            return format == typeof(TryGetData1) ? new TryGetData1() : null;
        }

        public bool GetDataPresent(string format, bool autoConvert) => throw new NotImplementedException();
        public bool GetDataPresent(string format) => throw new NotImplementedException();
        public bool GetDataPresent(Type format) => throw new NotImplementedException();
        public string[] GetFormats(bool autoConvert) => throw new NotImplementedException();
        public string[] GetFormats() => throw new NotImplementedException();
        public void SetData(string format, bool autoConvert, object? data) => throw new NotImplementedException();
        public void SetData(string format, object? data) => throw new NotImplementedException();
        public void SetData(Type format, object? data) => throw new NotImplementedException();
        public void SetData(object? data) => throw new NotImplementedException();
    }
}

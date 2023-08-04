﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.System.Variant;
using Windows.Win32.System.Com.StructuredStorage;

namespace System.Windows.Forms.Tests;

public unsafe class AxHost_PropertyBagStreamTests
{
    [Fact]
    public void PropertyBagStream_WriteReadRoundTrip_FormatterEnabled()
    {
        using var formatterScope = new BinaryFormatterScope(enable: true);
        AxHost.PropertyBagStream bag = new();
        HRESULT hr = bag.Write("Integer", (VARIANT)42);
        Assert.True(hr.Succeeded);
        NameClass obj = new() { Name = "Hamlet" };
        hr = bag.Write("Object", VARIANT.FromObject(obj));
        Assert.True(hr.Succeeded);

        using MemoryStream stream = new();
        bag.Save(stream);
        Assert.NotEqual(0, stream.Length);
        stream.Position = 0;

        AxHost.PropertyBagStream newBag = new(stream);

        VARIANT integer = new();
        hr = newBag.Read("Integer", ref integer, null);
        Assert.True(hr.Succeeded);
        Assert.Equal(42, (int)integer);

        VARIANT dispatch = new();
        hr = newBag.Read("Object", ref dispatch, null);
        Assert.True(hr.Succeeded);
        Assert.Equal(obj.Name, ((NameClass)dispatch.ToObject()).Name);
    }

    [Fact]
    public void PropertyBagStream_WriteReadRoundTrip_FormatterDisabled()
    {
        using var formatterScope = new BinaryFormatterScope(enable: false);
        AxHost.PropertyBagStream bag = new();
        HRESULT hr = bag.Write("Integer", (VARIANT)42);
        Assert.True(hr.Succeeded);
        NameClass obj = new() { Name = "Hamlet" };
        hr = bag.Write("Object", VARIANT.FromObject(obj));
        Assert.True(hr.Succeeded);

        using MemoryStream stream = new();
        Assert.Throws<NotSupportedException>(() => bag.Save(stream));
        Assert.Equal(0, stream.Position);
    }

    [Theory]
    [MemberData(nameof(TestData_PrimitiveValues))]
    public void PropertyBagStream_WriteReadRoundTrip_Primitives_FormatterDisabled(object value)
    {
        using var formatterScope = new BinaryFormatterScope(enable: false);

        AxHost.PropertyBagStream bag = new();
        using VARIANT variant = default;
        Marshal.GetNativeVariantForObject(value, (nint)(void*)&variant);
        string name = value.GetType().FullName!;

        HRESULT hr = bag.Write(value.GetType().FullName!, variant);
        Assert.True(hr.Succeeded);

        using MemoryStream stream = new();
        bag.Save(stream);
        Assert.NotEqual(0, stream.Length);
        stream.Position = 0;

        IPropertyBag.Interface newBag = new AxHost.PropertyBagStream(stream);
        using VARIANT result = new();
        hr = newBag.Read(name, &result, null);
        Assert.True(hr.Succeeded);
        Assert.Equal(variant.ToObject(), result.ToObject());
    }

    public static TheoryData<object> TestData_PrimitiveValues => new()
    {
        int.MaxValue,
        uint.MaxValue,
        long.MaxValue,
        ulong.MaxValue,
        short.MaxValue,
        ushort.MaxValue,
        byte.MaxValue,
        sbyte.MaxValue,
        true,
        float.MaxValue,
        double.MaxValue,
        char.MaxValue,
        // TimeSpan has no VARIANT conversion
        // TimeSpan.MaxValue,
        DateTime.MaxValue,
        decimal.MaxValue,
        "RightRound"
    };

    [Serializable]
    private class NameClass
    {
        public string Name { get; set; }
    }
}

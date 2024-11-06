// Licensed to the .NET Foundation under one or more agreements.
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
        using BinaryFormatterScope formatterScope = new(enable: true);
        AxHost.PropertyBagStream bag = new();
        // cs/deserialization-unexpected-subtypes
        HRESULT hr = bag.Write("Integer", (VARIANT)42); // CodeQL[SM02229] : Testing legacy feature. This is a safe use of VARIANT because the data is trusted and the types are controlled and validated.
        Assert.True(hr.Succeeded);
        NameClass obj = new() { Name = "Hamlet" };
        // cs/deserialization-unexpected-subtypes
        hr = bag.Write("Object", VARIANT.FromObject(obj)); // CodeQL[SM02229] : Testing legacy feature. This is a safe use of VARIANT because the data is trusted and the types are controlled and validated.
        Assert.True(hr.Succeeded);

        using MemoryStream stream = new();
        bag.Save(stream);
        Assert.NotEqual(0, stream.Length);
        stream.Position = 0;

        AxHost.PropertyBagStream newBag = new(stream);

        VARIANT integer = default;
        hr = newBag.Read("Integer", ref integer, null);
        Assert.True(hr.Succeeded);
        Assert.Equal(42, (int)integer);

        VARIANT dispatch = default;
        hr = newBag.Read("Object", ref dispatch, null);
        Assert.True(hr.Succeeded);
        Assert.Equal(obj.Name, ((NameClass)dispatch.ToObject()).Name);
    }

    [Fact]
    public void PropertyBagStream_WriteReadRoundTrip_FormatterDisabled()
    {
        using BinaryFormatterScope formatterScope = new(enable: false);
        AxHost.PropertyBagStream bag = new();

        // cs/deserialization-unexpected-subtypes
        HRESULT hr = bag.Write("Integer", (VARIANT)42); // CodeQL[SM02229] : Testing legacy feature. This is a safe use of VARIANT because the data is trusted and the types are controlled and validated.
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
        using BinaryFormatterScope formatterScope = new(enable: false);

        AxHost.PropertyBagStream bag = new();
        using VARIANT variant = default;
        Marshal.GetNativeVariantForObject(value, (nint)(void*)&variant);
        string name = value.GetType().FullName!;

        // cs/deserialization-unexpected-subtypes
        HRESULT hr = bag.Write(value.GetType().FullName!, variant); // CodeQL[SM02229] : Testing legacy feature. This is a safe use of VARIANT because the data is trusted and the types are controlled and validated.
        Assert.True(hr.Succeeded);

        using MemoryStream stream = new();
        bag.Save(stream);
        Assert.NotEqual(0, stream.Length);
        stream.Position = 0;

        IPropertyBag.Interface newBag = new AxHost.PropertyBagStream(stream);
        using VARIANT result = default;
        hr = newBag.Read(name, &result, null);
        Assert.True(hr.Succeeded);
        Assert.Equal(variant.ToObject(), result.ToObject());
    }

    public static TheoryData<object> TestData_PrimitiveValues =>
    [
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
    ];

    [Serializable]
    private class NameClass
    {
        public string Name { get; set; }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.System.Variant;
using static Windows.Win32.System.Ole.FDEX_PROP_FLAGS;

namespace Windows.Win32.System.Ole.Tests;

public unsafe class ClassPropertyDispatchAdapterTests
{
    [Fact]
    public void ClassPropertyDispatchAdapter_SingleClass()
    {
        TestClass testClass = new();
        ClassPropertyDispatchAdapter adapter = new(testClass);

        adapter.TryGetNextDispId(PInvokeCore.DISPID_STARTENUM, out int result).Should().BeTrue();
        result.Should().Be(0x10000);
        adapter.TryGetMemberName(result, out string? name).Should().BeTrue();
        name.Should().Be("Value");
        adapter.TryGetMemberProperties(result, out var flags).Should().BeTrue();
        flags.Should().Be(fdexPropCanGet | fdexPropCanPut | fdexPropCannotPutRef | fdexPropCannotCall | fdexPropCannotConstruct | fdexPropCannotSourceEvents);

        VARIANT parameter = (VARIANT)42;
        Com.DISPPARAMS dispParams = new()
        {
            cArgs = 1,
            rgvarg = &parameter
        };

        HRESULT hr = adapter.Invoke(result, 0, Com.DISPATCH_FLAGS.DISPATCH_PROPERTYPUT, &dispParams, null);
        hr.Succeeded.Should().BeTrue();
        testClass.Value.Should().Be(42);
        VARIANT returnValue = default;
        hr = adapter.Invoke(result, 0, Com.DISPATCH_FLAGS.DISPATCH_PROPERTYGET, &dispParams, &returnValue);
        hr.Succeeded.Should().BeTrue();
        ((int)returnValue).Should().Be(42);

        adapter.TryGetNextDispId(result, out result).Should().BeTrue();
        result.Should().Be(0x10001);
        adapter.TryGetMemberName(result, out name).Should().BeTrue();
        name.Should().Be("Name");
        adapter.TryGetMemberProperties(result, out flags).Should().BeTrue();
        flags.Should().Be(fdexPropCanGet | fdexPropCannotPut | fdexPropCannotPutRef | fdexPropCannotCall | fdexPropCannotConstruct | fdexPropCannotSourceEvents);
        hr = adapter.Invoke(result, 0, Com.DISPATCH_FLAGS.DISPATCH_PROPERTYGET, &dispParams, &returnValue);
        hr.Succeeded.Should().BeTrue();
        ((BSTR)returnValue).ToStringAndFree().Should().Be("Q");

        adapter.TryGetNextDispId(result, out result).Should().BeTrue();
        result.Should().Be(0x1);
        adapter.TryGetMemberName(result, out name).Should().BeTrue();
        name.Should().Be("Percent");
    }

    [Fact]
    public void ClassPropertyDispatchAdapter_ConflictingDispID()
    {
        TestClass testClass = new();
        TestClass2 testClass2 = new();

        ClassPropertyDispatchAdapter adapter = new(testClass, new(testClass2));
        adapter.TryGetDispID("Percent", out int dispId).Should().BeTrue();
        dispId.Should().Be(0x10002);
        adapter.TryGetDispID("CollidingDispId", out dispId).Should().BeTrue();
        dispId.Should().Be(0x1);

        adapter = new(testClass2, new(testClass));
        adapter.TryGetDispID("Percent", out dispId).Should().BeTrue();
        dispId.Should().Be(0x1);
        adapter.TryGetDispID("CollidingDispId", out dispId).Should().BeTrue();
        dispId.Should().Be(0x10002);
    }

    public class TestClass
    {
        public int Value { get; set; }
        public string Name { get; } = "Q";

        [DispId(1)]
        public float Percent { get; set; }
    }

    public class TestClass2
    {
        [DispId(1)]
        public int CollidingDispId { get; set; }
    }
}

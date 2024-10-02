// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class PropertyStoreTests
{
    public static TheoryData<int, object?> PropertyStore_TryGetValue_Exists_TestData()
    {
        return new TheoryData<int, object?>()
        {
            { 1, true },
            { 2, (byte)1 },
            { 3, (sbyte)-1 },
            { 4, 'a' },
            { 5, 1.0m },
            { 6, 1.0d },
            { 7, 1.0f },
            { 8, 1 },
            { 9, (uint)1 },
            { 10, 1L },
            { 11, 1UL },
            { 12, (short)1 },
            { 13, (ushort)1 },
            { 14, new() },
            { 15, Color.Red },
            { 16, FormWindowState.Maximized }
        };
    }

    [Theory]
    [MemberData(nameof(PropertyStore_TryGetValue_Exists_TestData))]
    public void PropertyStore_TryGetValue_Exists(int key, object? value)
    {
        PropertyStore store = new();
        store.AddValue(key, value);
        store.ContainsKey(key).Should().BeTrue();
        store.TryGetValue(key, out object? outValue).Should().BeTrue();
        value.Should().Be(outValue);
    }

    [Theory]
    [MemberData(nameof(PropertyStore_TryGetValue_Exists_TestData))]
    public void PropertyStore_TryGetValue_Exists_Null(int key, object? value)
    {
        PropertyStore store = new();
        store.AddValue<object?>(key, null);
        store.ContainsKey(key).Should().BeTrue();
        store.TryGetValueOrNull(key, out object? outValue).Should().BeTrue();
        value.Should().NotBe(outValue);
    }

    [Theory]
    [MemberData(nameof(PropertyStore_TryGetValue_Exists_TestData))]
    public void PropertyStore_TryGetValue_NotExists(int key, object? value)
    {
        PropertyStore store = new();
        store.AddValue<object?>(key, null);
        store.RemoveValue(key);
        store.ContainsKey(key).Should().BeFalse();
        store.TryGetValue(key, out object? outValue).Should().BeFalse();
        value.Should().NotBe(outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Enum_Unset_IsDefault()
    {
        PropertyStore store = new();
        store.ContainsKey(1).Should().BeFalse();
        store.TryGetValue(1, out FormWindowState outValue).Should().BeFalse();
        FormWindowState windowState = default;
        windowState.Should().Be(outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Struct_Unset_IsDefault()
    {
        PropertyStore store = new();
        store.ContainsKey(1).Should().BeFalse();
        store.TryGetValue(1, out Color outValue).Should().BeFalse();
        Color color = default;
        color.Should().Be(outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Primitive_Unset_IsDefault()
    {
        PropertyStore store = new();
        store.ContainsKey(1).Should().BeFalse();
        store.TryGetValue(1, out int outValue).Should().BeFalse();
        int intDefault = default;
        intDefault.Should().Be(outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Enum_Null()
    {
        PropertyStore store = new();
        store.AddValue<object?>(1, null);
        store.ContainsKey(1).Should().BeTrue();
        Action action = () => store.TryGetValue(1, out FormWindowState outValue);
        action.Should().Throw<InvalidCastException>();
    }

    [Fact]
    public void PropertyStore_TryGetValue_Struct_Null()
    {
        PropertyStore store = new();
        store.AddValue<object?>(1, null);
        store.ContainsKey(1).Should().BeTrue();
        Action action = () => store.TryGetValue(1, out Color outValue);
        action.Should().Throw<InvalidCastException>();
    }

    [Fact]
    public void PropertyStore_TryGetValue_Primitive_Null()
    {
        PropertyStore store = new();
        store.AddValue<object?>(1, null);
        store.ContainsKey(1).Should().BeTrue();
        Action action = () => store.TryGetValue(1, out int outValue);
        action.Should().Throw<InvalidCastException>();
    }

    [Fact]
    public void PropertyStore_TryGetValue_Enum_Unset_Nullable()
    {
        PropertyStore store = new();
        store.ContainsKey(1).Should().BeFalse();
        store.TryGetValue(1, out FormWindowState? outValue).Should().BeFalse();
        outValue.Should().BeNull();
    }

    [Fact]
    public void PropertyStore_TryGetValue_Struct_Unset_Nullable()
    {
        PropertyStore store = new();
        store.ContainsKey(1).Should().BeFalse();
        store.TryGetValue(1, out Color? outValue).Should().BeFalse();
        outValue.Should().BeNull();
    }

    [Fact]
    public void PropertyStore_TryGetValue_Primitive_Unset_Nullable()
    {
        PropertyStore store = new();
        store.ContainsKey(1).Should().BeFalse();
        store.TryGetValue(1, out int? outValue).Should().BeFalse();
        outValue.Should().BeNull();
    }

    [Fact]
    public void PropertyStore_Rectangle_UpdateDoesNotAllocate()
    {
        PropertyStore store = new();
        Rectangle one = new(1, 2, 3, 4);
        Rectangle two = new(5, 6, 7, 8);
        store.AddValue(1, one);
        long currentBytes = GC.GetAllocatedBytesForCurrentThread();
        store.AddValue(1, two);
        currentBytes = GC.GetAllocatedBytesForCurrentThread() - currentBytes;
        currentBytes.Should().Be(0);

        store.TryGetValue(1, out Rectangle result).Should().BeTrue();
        result.Should().Be(two);
    }

    [Fact]
    public void PropertyStore_Rectangle_WrongExistingType()
    {
        PropertyStore store = new();
        Rectangle one = new(1, 2, 3, 4);

        // Check stored null
        store.AddValue<object?>(1, null);
        store.AddValue(1, one);
        store.TryGetValue(1, out Rectangle result).Should().BeTrue();
        result.Should().Be(one);

        // Check stored wrong type
        store.AddValue(1, DateTime.Now);
        store.AddValue(1, one);
        store.TryGetValue(1, out result).Should().BeTrue();
        result.Should().Be(one);
    }

    [Fact]
    public void PropertyStore_Padding_UpdateDoesNotAllocate()
    {
        PropertyStore store = new();
        Padding one = new(1, 2, 3, 4);
        Padding two = new(5, 6, 7, 8);
        store.AddValue(1, one);
        long currentBytes = GC.GetAllocatedBytesForCurrentThread();
        store.AddValue(1, two);
        currentBytes = GC.GetAllocatedBytesForCurrentThread() - currentBytes;
        currentBytes.Should().Be(0);

        store.TryGetValue(1, out Padding result).Should().BeTrue();
        result.Should().Be(two);
    }

    [Fact]
    public void PropertyStore_AddOrRemoveString_GetStringOrEmptyString_Expected()
    {
        PropertyStore store = new();
        store.AddOrRemoveString(1, null);
        store.ContainsKey(1).Should().BeFalse();
        store.GetStringOrEmptyString(1).Should().Be(string.Empty);

        store.AddOrRemoveString(1, string.Empty);
        store.ContainsKey(1).Should().BeFalse();
        store.GetStringOrEmptyString(1).Should().Be(string.Empty);

        store.AddOrRemoveString(1, "test");
        store.ContainsKey(1).Should().BeTrue();
        store.GetStringOrEmptyString(1).Should().Be("test");

        Action action = () => store.AddOrRemoveValue(1, 1);
        action.Should().Throw<InvalidCastException>();

        store.RemoveValue(1);
        store.AddOrRemoveValue(1, 1);
        store.ContainsKey(1).Should().BeTrue();
        action = () => store.GetStringOrEmptyString(1);
        action.Should().Throw<InvalidCastException>();
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Bool()
    {
        bool value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(1, value);
        store.ContainsKey(1).Should().BeFalse();
        value.Should().Be(store.GetValueOrDefault<bool>(1));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Byte()
    {
        byte value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(1, value);
        store.ContainsKey(1).Should().BeFalse();
        value.Should().Be(store.GetValueOrDefault<byte>(1));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_SByte()
    {
        sbyte value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(1, value);
        store.ContainsKey(1).Should().BeFalse();
        value.Should().Be(store.GetValueOrDefault<sbyte>(1));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Char()
    {
        char value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(1, value);
        store.ContainsKey(1).Should().BeFalse();
        value.Should().Be(store.GetValueOrDefault<char>(1));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Decimal()
    {
        decimal value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(1, value);
        store.ContainsKey(1).Should().BeFalse();
        value.Should().Be(store.GetValueOrDefault<decimal>(1));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Double()
    {
        double value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(1, value);
        store.ContainsKey(1).Should().BeFalse();
        value.Should().Be(store.GetValueOrDefault<double>(1));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Float()
    {
        float value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(1, value);
        store.ContainsKey(1).Should().BeFalse();
        value.Should().Be(store.GetValueOrDefault<float>(1));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Int()
    {
        int value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(1, value);
        store.ContainsKey(1).Should().BeFalse();
        value.Should().Be(store.GetValueOrDefault<int>(1));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_UInt()
    {
        uint value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(1, value);
        store.ContainsKey(1).Should().BeFalse();
        value.Should().Be(store.GetValueOrDefault<uint>(1));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Long()
    {
        long value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(1, value);
        store.ContainsKey(1).Should().BeFalse();
        value.Should().Be(store.GetValueOrDefault<long>(1));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_ULong()
    {
        ulong value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(1, value);
        store.ContainsKey(1).Should().BeFalse();
        value.Should().Be(store.GetValueOrDefault<ulong>(1));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Short()
    {
        short value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(1, value);
        store.ContainsKey(1).Should().BeFalse();
        value.Should().Be(store.GetValueOrDefault<short>(1));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_UShort()
    {
        ushort value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(1, value);
        store.ContainsKey(1).Should().BeFalse();
        value.Should().Be(store.GetValueOrDefault<ushort>(1));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Object()
    {
        object? value = null;
        PropertyStore store = new();
        store.AddOrRemoveValue(1, value);
        store.ContainsKey(1).Should().BeFalse();
        value.Should().Be(store.GetValueOrDefault<object>(1));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Color()
    {
        Color value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(1, value);
        store.ContainsKey(1).Should().BeFalse();
        value.Should().Be(store.GetValueOrDefault<Color>(1));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_FormWindowState()
    {
        FormWindowState value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(1, value);
        store.ContainsKey(1).Should().BeFalse();
        value.Should().Be(store.GetValueOrDefault<FormWindowState>(1));
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class PropertyStoreTests
{
    private static readonly int s_bool = PropertyStore.CreateKey();
    private static readonly int s_byte = PropertyStore.CreateKey();
    private static readonly int s_sbyte = PropertyStore.CreateKey();
    private static readonly int s_char = PropertyStore.CreateKey();
    private static readonly int s_decimal = PropertyStore.CreateKey();
    private static readonly int s_double = PropertyStore.CreateKey();
    private static readonly int s_float = PropertyStore.CreateKey();
    private static readonly int s_int = PropertyStore.CreateKey();
    private static readonly int s_uint = PropertyStore.CreateKey();
    private static readonly int s_long = PropertyStore.CreateKey();
    private static readonly int s_ulong = PropertyStore.CreateKey();
    private static readonly int s_short = PropertyStore.CreateKey();
    private static readonly int s_ushort = PropertyStore.CreateKey();
    private static readonly int s_object = PropertyStore.CreateKey();
    private static readonly int s_color = PropertyStore.CreateKey();
    private static readonly int s_formWindowState = PropertyStore.CreateKey();

    public static TheoryData<int, object?> PropertyStore_TryGetValue_Exists_TestData()
    {
        return new TheoryData<int, object?>()
        {
            { s_bool, true },
            { s_byte, (byte)1 },
            { s_sbyte, (sbyte)-1 },
            { s_char, 'a' },
            { s_decimal, 1.0m },
            { s_double, 1.0d },
            { s_float, 1.0f },
            { s_int, 1 },
            { s_uint, (uint)1 },
            { s_long, 1L },
            { s_ulong, 1UL },
            { s_short, (short)1 },
            { s_ushort, (ushort)1 },
            { s_object, new() },
            { s_color, Color.Red },
            { s_formWindowState, FormWindowState.Maximized }
        };
    }

    [Theory]
    [MemberData(nameof(PropertyStore_TryGetValue_Exists_TestData))]
    public void PropertyStore_TryGetValue_Exists(int key, object? value)
    {
        PropertyStore store = new();
        store.SetObject(key, value);
        Assert.True(store.ContainsKey(key), "PropertyStore does not contain key.");
        Assert.True(store.TryGetObject(key, out object? outValue));
        Assert.Equal(value, outValue);
    }

    [Theory]
    [MemberData(nameof(PropertyStore_TryGetValue_Exists_TestData))]
    public void PropertyStore_TryGetValue_Exists_Null(int key, object? value)
    {
        PropertyStore store = new();
        store.SetObject(key, null);
        Assert.True(store.ContainsKey(key), "PropertyStore does not contain key.");
        Assert.True(store.TryGetObject(key, out object? outValue));
        Assert.NotEqual(value, outValue);
    }

    [Theory]
    [MemberData(nameof(PropertyStore_TryGetValue_Exists_TestData))]
    public void PropertyStore_TryGetValue_NotExists(int key, object? value)
    {
        PropertyStore store = new();
        store.SetObject(key, null);
        store.RemoveValue(key);
        Assert.False(store.ContainsKey(key), "PropertyStore contains key.");
        Assert.False(store.TryGetObject(key, out object? outValue), "PropertyStore contains key.");
        Assert.NotEqual(value, outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Enum_Unset_IsDefault()
    {
        PropertyStore store = new();
        Assert.False(store.ContainsKey(s_formWindowState), "PropertyStore contains key.");
        Assert.False(store.TryGetObject(s_formWindowState, out FormWindowState outValue), "PropertyStore contains key.");
        FormWindowState windowState = default;
        Assert.Equal(windowState, outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Struct_Unset_IsDefault()
    {
        PropertyStore store = new();
        Assert.False(store.ContainsKey(s_color), "PropertyStore contains key.");
        Assert.False(store.TryGetObject(s_color, out Color outValue), "PropertyStore contains key.");
        Color color = default;
        Assert.Equal(color, outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Primitive_Unset_IsDefault()
    {
        PropertyStore store = new();
        Assert.False(store.ContainsKey(s_int), "PropertyStore does not contain key.");
        Assert.False(store.TryGetObject(s_int, out int outValue), "PropertyStore contains key.");
        int intDefault = default;
        Assert.Equal(intDefault, outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Enum_Null()
    {
        PropertyStore store = new();
        store.SetObject(s_formWindowState, null);
        Assert.True(store.ContainsKey(s_formWindowState), "PropertyStore contains key.");
        Assert.True(store.TryGetObject(s_formWindowState, out FormWindowState outValue), "PropertyStore contains key.");
        FormWindowState windowState = default;
        Assert.Equal(windowState, outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Struct_Null()
    {
        PropertyStore store = new();
        store.SetObject(s_color, null);
        Assert.True(store.ContainsKey(s_color), "PropertyStore does not contain key.");
        Assert.True(store.TryGetObject(s_color, out Color outValue), "PropertyStore does not contain key.");
        Color color = default;
        Assert.Equal(color, outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Primitive_Null()
    {
        PropertyStore store = new();
        store.SetObject(s_int, null);
        Assert.True(store.ContainsKey(s_int), "PropertyStore does not contain key.");
        Assert.True(store.TryGetObject(s_int, out int outValue), "PropertyStore does not contain key.");
        int intDefault = default;
        Assert.Equal(intDefault, outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Enum_Nullable()
    {
        PropertyStore store = new();
        store.SetObject(s_formWindowState, null);
        Assert.True(store.ContainsKey(s_formWindowState), "PropertyStore does not contain key.");
        Assert.True(store.TryGetObject(s_formWindowState, out FormWindowState? outValue), "PropertyStore does not contain key.");
        Assert.Null(outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Struct_Nullable()
    {
        PropertyStore store = new();
        store.SetObject(s_color, null);
        Assert.True(store.ContainsKey(s_color), "PropertyStore does not contain key.");
        Assert.True(store.TryGetObject(s_color, out Color? outValue), "PropertyStore does not contain key.");
        Assert.Null(outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Primitive_Nullable()
    {
        PropertyStore store = new();
        store.SetObject(s_int, null);
        Assert.True(store.ContainsKey(s_int), "PropertyStore contains key.");
        Assert.True(store.TryGetObject(s_int, out int? outValue), "PropertyStore contains key.");
        Assert.Null(outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Enum_Unset_Nullable()
    {
        PropertyStore store = new();
        Assert.False(store.ContainsKey(s_formWindowState), "PropertyStore contains key.");
        Assert.False(store.TryGetObject(s_formWindowState, out FormWindowState? outValue), "PropertyStore contains key.");
        Assert.Null(outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Struct_Unset_Nullable()
    {
        PropertyStore store = new();
        Assert.False(store.ContainsKey(s_color), "PropertyStore contains key.");
        Assert.False(store.TryGetObject(s_color, out Color? outValue), "PropertyStore contains key.");
        Assert.Null(outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Primitive_Unset_Nullable()
    {
        PropertyStore store = new();
        Assert.False(store.ContainsKey(s_int), "PropertyStore contains key.");
        Assert.False(store.TryGetObject(s_int, out int? outValue), "PropertyStore contains key.");
        Assert.Null(outValue);
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
        int key = s_bool;
        bool value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(key, value);
        Assert.False(store.ContainsKey(key), "PropertyStore contains key.");
        Assert.Equal(value, store.GetValueOrDefault<bool>(key));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Byte()
    {
        int key = s_byte;
        byte value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(key, value);
        Assert.False(store.ContainsKey(key), "PropertyStore contains key.");
        Assert.Equal(value, store.GetValueOrDefault<byte>(key));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_SByte()
    {
        int key = s_sbyte;
        sbyte value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(key, value);
        Assert.False(store.ContainsKey(key), "PropertyStore contains key.");
        Assert.Equal(value, store.GetValueOrDefault<sbyte>(key));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Char()
    {
        int key = s_char;
        char value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(key, value);
        Assert.False(store.ContainsKey(key), "PropertyStore contains key.");
        Assert.Equal(value, store.GetValueOrDefault<char>(key));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Decimal()
    {
        int key = s_decimal;
        decimal value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(key, value);
        Assert.False(store.ContainsKey(key), "PropertyStore contains key.");
        Assert.Equal(value, store.GetValueOrDefault<decimal>(key));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Double()
    {
        int key = s_double;
        double value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(key, value);
        Assert.False(store.ContainsKey(key), "PropertyStore contains key.");
        Assert.Equal(value, store.GetValueOrDefault<double>(key));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Float()
    {
        int key = s_float;
        float value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(key, value);
        Assert.False(store.ContainsKey(key), "PropertyStore contains key.");
        Assert.Equal(value, store.GetValueOrDefault<float>(key));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Int()
    {
        int key = s_int;
        int value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(key, value);
        Assert.False(store.ContainsKey(key), "PropertyStore contains key.");
        Assert.Equal(value, store.GetValueOrDefault<int>(key));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_UInt()
    {
        int key = s_uint;
        uint value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(key, value);
        Assert.False(store.ContainsKey(key), "PropertyStore contains key.");
        Assert.Equal(value, store.GetValueOrDefault<uint>(key));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Long()
    {
        int key = s_long;
        long value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(key, value);
        Assert.False(store.ContainsKey(key), "PropertyStore contains key.");
        Assert.Equal(value, store.GetValueOrDefault<long>(key));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_ULong()
    {
        int key = s_ulong;
        ulong value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(key, value);
        Assert.False(store.ContainsKey(key), "PropertyStore contains key.");
        Assert.Equal(value, store.GetValueOrDefault<ulong>(key));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Short()
    {
        int key = s_short;
        short value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(key, value);
        Assert.False(store.ContainsKey(key), "PropertyStore contains key.");
        Assert.Equal(value, store.GetValueOrDefault<short>(key));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_UShort()
    {
        int key = s_ushort;
        ushort value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(key, value);
        Assert.False(store.ContainsKey(key), "PropertyStore contains key.");
        Assert.Equal(value, store.GetValueOrDefault<ushort>(key));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Object()
    {
        int key = s_object;
        object? value = null;
        PropertyStore store = new();
        store.AddOrRemoveValue(key, value);
        Assert.False(store.ContainsKey(key), "PropertyStore contains key.");
        Assert.Equal(value, store.GetValueOrDefault<object>(key));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_Color()
    {
        int key = s_color;
        Color value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(key, value);
        Assert.False(store.ContainsKey(key), "PropertyStore contains key.");
        Assert.Equal(value, store.GetValueOrDefault<Color>(key));
    }

    [Fact]
    public void PropertyStore_AddOrRemoveValue_DefaultValuesRemoved_FormWindowState()
    {
        int key = s_formWindowState;
        FormWindowState value = default;
        PropertyStore store = new();
        store.AddOrRemoveValue(key, value);
        Assert.False(store.ContainsKey(key), "PropertyStore contains key.");
        Assert.Equal(value, store.GetValueOrDefault<FormWindowState>(key));
    }
}

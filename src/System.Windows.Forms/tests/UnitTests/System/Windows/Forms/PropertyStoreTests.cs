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
        Assert.True(store.ContainsObject(key), "PropertyStore does not contain key.");
        Assert.True(store.TryGetObject(key, out object? outValue));
        Assert.Equal(value, outValue);
    }

    [Theory]
    [MemberData(nameof(PropertyStore_TryGetValue_Exists_TestData))]
    public void PropertyStore_TryGetValue_Exists_Null(int key, object? value)
    {
        PropertyStore store = new();
        store.SetObject(key, null);
        Assert.True(store.ContainsObject(key), "PropertyStore does not contain key.");
        Assert.True(store.TryGetObject(key, out object? outValue));
        Assert.NotEqual(value, outValue);
    }

    [Theory]
    [MemberData(nameof(PropertyStore_TryGetValue_Exists_TestData))]
    public void PropertyStore_TryGetValue_NotExists(int key, object? value)
    {
        PropertyStore store = new();
        store.SetObject(key, null);
        store.RemoveObject(key);
        Assert.False(store.ContainsObject(key), "PropertyStore contains key.");
        Assert.False(store.TryGetObject(key, out object? outValue), "PropertyStore contains key.");
        Assert.NotEqual(value, outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Enum_Unset_IsDefault()
    {
        PropertyStore store = new();
        Assert.False(store.ContainsObject(s_formWindowState), "PropertyStore contains key.");
        Assert.False(store.TryGetObject(s_formWindowState, out FormWindowState outValue), "PropertyStore contains key.");
        FormWindowState windowState = default;
        Assert.Equal(windowState, outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Struct_Unset_IsDefault()
    {
        PropertyStore store = new();
        Assert.False(store.ContainsObject(s_color), "PropertyStore contains key.");
        Assert.False(store.TryGetObject(s_color, out Color outValue), "PropertyStore contains key.");
        Color color = default;
        Assert.Equal(color, outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Primitive_Unset_IsDefault()
    {
        PropertyStore store = new();
        Assert.False(store.ContainsObject(s_int), "PropertyStore does not contain key.");
        Assert.False(store.TryGetObject(s_int, out int outValue), "PropertyStore contains key.");
        int intDefault = default;
        Assert.Equal(intDefault, outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Enum_Null()
    {
        PropertyStore store = new();
        store.SetObject(s_formWindowState, null);
        Assert.True(store.ContainsObject(s_formWindowState), "PropertyStore contains key.");
        Assert.True(store.TryGetObject(s_formWindowState, out FormWindowState outValue), "PropertyStore contains key.");
        FormWindowState windowState = default;
        Assert.Equal(windowState, outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Struct_Null()
    {
        PropertyStore store = new();
        store.SetObject(s_color, null);
        Assert.True(store.ContainsObject(s_color), "PropertyStore does not contain key.");
        Assert.True(store.TryGetObject(s_color, out Color outValue), "PropertyStore does not contain key.");
        Color color = default;
        Assert.Equal(color, outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Primitive_Null()
    {
        PropertyStore store = new();
        store.SetObject(s_int, null);
        Assert.True(store.ContainsObject(s_int), "PropertyStore does not contain key.");
        Assert.True(store.TryGetObject(s_int, out int outValue), "PropertyStore does not contain key.");
        int intDefault = default;
        Assert.Equal(intDefault, outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Enum_Nullable()
    {
        PropertyStore store = new();
        store.SetObject(s_formWindowState, null);
        Assert.True(store.ContainsObject(s_formWindowState), "PropertyStore does not contain key.");
        Assert.True(store.TryGetObject(s_formWindowState, out FormWindowState? outValue), "PropertyStore does not contain key.");
        Assert.Null(outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Struct_Nullable()
    {
        PropertyStore store = new();
        store.SetObject(s_color, null);
        Assert.True(store.ContainsObject(s_color), "PropertyStore does not contain key.");
        Assert.True(store.TryGetObject(s_color, out Color? outValue), "PropertyStore does not contain key.");
        Assert.Null(outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Primitive_Nullable()
    {
        PropertyStore store = new();
        store.SetObject(s_int, null);
        Assert.True(store.ContainsObject(s_int), "PropertyStore contains key.");
        Assert.True(store.TryGetObject(s_int, out int? outValue), "PropertyStore contains key.");
        Assert.Null(outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Enum_Unset_Nullable()
    {
        PropertyStore store = new();
        Assert.False(store.ContainsObject(s_formWindowState), "PropertyStore contains key.");
        Assert.False(store.TryGetObject(s_formWindowState, out FormWindowState? outValue), "PropertyStore contains key.");
        Assert.Null(outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Struct_Unset_Nullable()
    {
        PropertyStore store = new();
        Assert.False(store.ContainsObject(s_color), "PropertyStore contains key.");
        Assert.False(store.TryGetObject(s_color, out Color? outValue), "PropertyStore contains key.");
        Assert.Null(outValue);
    }

    [Fact]
    public void PropertyStore_TryGetValue_Primitive_Unset_Nullable()
    {
        PropertyStore store = new();
        Assert.False(store.ContainsObject(s_int), "PropertyStore contains key.");
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
}

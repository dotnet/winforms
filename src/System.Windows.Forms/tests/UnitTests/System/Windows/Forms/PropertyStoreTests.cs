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

    public static IEnumerable<object[]> PropertyStore_TryGetValue_Exists_TestData()
    {
        yield return new object[] { s_bool, true };
        yield return new object[] { s_byte, (byte)1 };
        yield return new object[] { s_sbyte, (sbyte)-1 };
        yield return new object[] { s_char, 'a' };
        yield return new object[] { s_decimal, 1.0m };
        yield return new object[] { s_double, 1.0d };
        yield return new object[] { s_float, 1.0f };
        yield return new object[] { s_int, 1 };
        yield return new object[] { s_uint, (uint)1 };
        yield return new object[] { s_long, 1L };
        yield return new object[] { s_ulong, 1UL };
        yield return new object[] { s_short, (short)1 };
        yield return new object[] { s_ushort, (ushort)1 };
        yield return new object[] { s_object, new object() };
        yield return new object[] { s_color, Color.Red };
        yield return new object[] { s_formWindowState, FormWindowState.Maximized };
    }

    [WinFormsTheory]
    [MemberData(nameof(PropertyStore_TryGetValue_Exists_TestData))]
    public void PropertyStore_TryGetValue_Exists(int key, object? value)
    {
        PropertyStore properties = new PropertyStore();
        properties.SetObject(key, value);
        Assert.True(properties.ContainsObject(key), "PropertyStore does not contain key.");
        Assert.True(properties.TryGetObject(key, out object? outValue));
        Assert.Equal(value, outValue);
    }

    [WinFormsTheory]
    [MemberData(nameof(PropertyStore_TryGetValue_Exists_TestData))]
    public void PropertyStore_TryGetValue_Exists_Null(int key, object? value)
    {
        PropertyStore properties = new PropertyStore();
        properties.SetObject(key, null);
        Assert.True(properties.ContainsObject(key), "PropertyStore does not contain key.");
        Assert.True(properties.TryGetObject(key, out object? outValue));
        Assert.NotEqual(value, outValue);
    }

    [WinFormsTheory]
    [MemberData(nameof(PropertyStore_TryGetValue_Exists_TestData))]
    public void PropertyStore_TryGetValue_NotExists(int key, object? value)
    {
        PropertyStore properties = new PropertyStore();
        properties.SetObject(key, null);
        properties.RemoveObject(key);
        Assert.False(properties.ContainsObject(key), "PropertyStore contains key.");
        Assert.False(properties.TryGetObject(key, out object? outValue), "PropertyStore contains key.");
        Assert.NotEqual(value, outValue);
    }

    [WinFormsFact]
    public void PropertyStore_TryGetValue_Enum_Unset_IsDefault()
    {
        PropertyStore properties = new PropertyStore();
        Assert.False(properties.ContainsObject(s_formWindowState), "PropertyStore contains key.");
        Assert.False(properties.TryGetObject(s_formWindowState, out FormWindowState outValue), "PropertyStore contains key.");
        FormWindowState windowState = default;
        Assert.Equal(windowState, outValue);
    }

    [WinFormsFact]
    public void PropertyStore_TryGetValue_Struct_Unset_IsDefault()
    {
        PropertyStore properties = new PropertyStore();
        Assert.False(properties.ContainsObject(s_color), "PropertyStore contains key.");
        Assert.False(properties.TryGetObject(s_color, out Color outValue), "PropertyStore contains key.");
        Color color = default;
        Assert.Equal(color, outValue);
    }

    [WinFormsFact]
    public void PropertyStore_TryGetValue_Int_Unset_IsDefault()
    {
        PropertyStore properties = new PropertyStore();
        Assert.False(properties.ContainsObject(s_int), "PropertyStore does not contain key.");
        Assert.False(properties.TryGetObject(s_int, out int outValue), "PropertyStore contains key.");
        int intDefault = default;
        Assert.Equal(intDefault, outValue);
    }

    [WinFormsFact]
    public void PropertyStore_TryGetValue_Enum_Null()
    {
        PropertyStore properties = new PropertyStore();
        properties.SetObject(s_formWindowState, null);
        Assert.True(properties.ContainsObject(s_formWindowState), "PropertyStore contains key.");
        Assert.True(properties.TryGetObject(s_formWindowState, out FormWindowState outValue), "PropertyStore contains key.");
        FormWindowState windowState = default;
        Assert.Equal(windowState, outValue);
    }

    [WinFormsFact]
    public void PropertyStore_TryGetValue_Struct_Null()
    {
        PropertyStore properties = new PropertyStore();
        properties.SetObject(s_color, null);
        Assert.True(properties.ContainsObject(s_color), "PropertyStore does not contain key.");
        Assert.True(properties.TryGetObject(s_color, out Color outValue), "PropertyStore does not contain key.");
        Color color = default;
        Assert.Equal(color, outValue);
    }

    [WinFormsFact]
    public void PropertyStore_TryGetValue_Int_Null()
    {
        PropertyStore properties = new PropertyStore();
        properties.SetObject(s_int, null);
        Assert.True(properties.ContainsObject(s_int), "PropertyStore does not contain key.");
        Assert.True(properties.TryGetObject(s_int, out int outValue), "PropertyStore does not contain key.");
        int intDefault = default;
        Assert.Equal(intDefault, outValue);
    }

    [WinFormsFact]
    public void PropertyStore_TryGetValue_Enum_Nullable()
    {
        PropertyStore properties = new PropertyStore();
        properties.SetObject(s_formWindowState, null);
        Assert.True(properties.ContainsObject(s_formWindowState), "PropertyStore does not contain key.");
        Assert.True(properties.TryGetObject(s_formWindowState, out FormWindowState? outValue), "PropertyStore does not contain key.");
        Assert.Null(outValue);
    }

    [WinFormsFact]
    public void PropertyStore_TryGetValue_Struct_Nullable()
    {
        PropertyStore properties = new PropertyStore();
        properties.SetObject(s_color, null);
        Assert.True(properties.ContainsObject(s_color), "PropertyStore does not contain key.");
        Assert.True(properties.TryGetObject(s_color, out Color? outValue), "PropertyStore does not contain key.");
        Assert.Null(outValue);
    }

    [WinFormsFact]
    public void PropertyStore_TryGetValue_Int_Nullable()
    {
        PropertyStore properties = new PropertyStore();
        properties.SetObject(s_int, null);
        Assert.True(properties.ContainsObject(s_int), "PropertyStore contains key.");
        Assert.True(properties.TryGetObject(s_int, out int? outValue), "PropertyStore contains key.");
        Assert.Null(outValue);
    }

    [WinFormsFact]
    public void PropertyStore_TryGetValue_Enum_Unset_Nullable()
    {
        PropertyStore properties = new PropertyStore();
        Assert.False(properties.ContainsObject(s_formWindowState), "PropertyStore contains key.");
        Assert.False(properties.TryGetObject(s_formWindowState, out FormWindowState? outValue), "PropertyStore contains key.");
        Assert.Null(outValue);
    }

    [WinFormsFact]
    public void PropertyStore_TryGetValue_Struct_Unset_Nullable()
    {
        PropertyStore properties = new PropertyStore();
        Assert.False(properties.ContainsObject(s_color), "PropertyStore contains key.");
        Assert.False(properties.TryGetObject(s_color, out Color? outValue), "PropertyStore contains key.");
        Assert.Null(outValue);
    }

    [WinFormsFact]
    public void PropertyStore_TryGetValue_Int_Unset_Nullable()
    {
        PropertyStore properties = new PropertyStore();
        Assert.False(properties.ContainsObject(s_int), "PropertyStore contains key.");
        Assert.False(properties.TryGetObject(s_int, out int? outValue), "PropertyStore contains key.");
        Assert.Null(outValue);
    }
}



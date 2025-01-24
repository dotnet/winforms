// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.TestUtilities;

internal class ManagedDataObject : IDataObject
{
    public static string s_format = nameof(SerializableTestData);
    protected SerializableTestData? _data;

    public object? GetData(string format, bool autoConvert) => format == s_format ? _data : null;
    public object? GetData(string format) => format == s_format ? _data : null;
    public object? GetData(Type format) => null;
    public bool GetDataPresent(string format, bool autoConvert) => format == s_format && _data is not null;
    public bool GetDataPresent(string format) => format == s_format && _data is not null;
    public bool GetDataPresent(Type format) => false;
    public string[] GetFormats(bool autoConvert) => [s_format];
    public string[] GetFormats() => [s_format];
    public void SetData(string format, bool autoConvert, object? data)
    {
        if (format == s_format)
        {
            _data = data as SerializableTestData;
        }
    }

    public void SetData(string format, object? data)
    {
        if (format == s_format)
        {
            _data = data as SerializableTestData;
        }
    }

    public void SetData(Type format, object? data) => _data = data as SerializableTestData;
    public void SetData(object? data) => _data = data as SerializableTestData;
}

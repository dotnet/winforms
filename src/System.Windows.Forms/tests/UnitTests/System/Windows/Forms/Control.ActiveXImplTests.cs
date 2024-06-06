// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections;
using System.Drawing;
using System.Runtime.Serialization;
using Windows.Win32.System.Com;

namespace System.Windows.Forms.Tests;

public unsafe class Control_ActiveXImplTests
{
    [WinFormsFact]
    public void ActiveXImpl_SaveLoad_RoundTrip_FormatterDisabled()
    {
        using BinaryFormatterScope formatterScope = new(enable: false);
        using Control control = new();
        control.BackColor = Color.Bisque;
        IPersistStreamInit.Interface persistStream = control;

        using MemoryStream memoryStream = new();
        using var istream = memoryStream.ToIStream();
        HRESULT hr = persistStream.Save(istream.Value, fClearDirty: BOOL.FALSE);
        Assert.True(hr.Succeeded);
        control.BackColor = Color.Honeydew;

        istream.Value->Seek(0, SeekOrigin.Begin);
        hr = persistStream.Load(istream.Value);
        Assert.True(hr.Succeeded);
        Assert.Equal(Color.Bisque, control.BackColor);
    }

    [WinFormsFact]
    public void ActiveXImpl_SaveLoad_BinaryFormatterProperty_FormatterDisabled()
    {
        using BinaryFormatterScope formatterScope = new(enable: false);
        using MyControl control = new();

        // We need to have a type that doesn't have a TypeConverter that implements ISerializable to hit the
        // BinaryFormatter code path.
        SerializableStruct myValue = new() { Value = "HelloThere" };
        control.SerializableValue = myValue;
        IPersistStreamInit.Interface persistStream = control;

        using MemoryStream memoryStream = new();
        using var istream = memoryStream.ToIStream();
        var istreamPointer = istream.Value;
        Assert.Throws<NotSupportedException>(() => persistStream.Save(istreamPointer, fClearDirty: BOOL.FALSE));
    }

    [WinFormsFact]
    public void ActiveXImpl_SaveLoad_BinaryFormatterProperty_FormatterDisabledSupported()
    {
        using BinaryFormatterScope formatterScope = new(enable: false);
        using BinaryFormatterPropertiesControl control = new();

        // We need to have a type that doesn't have a TypeConverter that implements ISerializable to hit the
        // BinaryFormatter code path.
        control.Table = new Hashtable() { { "Whas", "sup" } };

        IPersistStreamInit.Interface persistStream = control;

        using MemoryStream memoryStream = new();
        using var istream = memoryStream.ToIStream();
        HRESULT hr = persistStream.Save(istream.Value, fClearDirty: BOOL.FALSE);
        Assert.True(hr.Succeeded);
        control.Table = default;

        istream.Value->Seek(0, SeekOrigin.Begin);
        hr = persistStream.Load(istream.Value);
        Assert.True(hr.Succeeded);
        Assert.Equal("sup", control.Table!["Whas"]);
    }

    private class MyControl : Control
    {
        public SerializableStruct SerializableValue { get; set; }
    }

    private class BinaryFormatterPropertiesControl : Control
    {
        public Hashtable? Table { get; set; }
    }

    [Serializable]
    public struct SerializableStruct : ISerializable
    {
        public string? Value { get; set; }

        public readonly void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Value), Value, typeof(string));
        }

        private SerializableStruct(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            Value = (string?)serializationInfo.GetValue(nameof(Value), typeof(string));
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Runtime.Serialization;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

namespace System.Windows.Forms.Tests;

public unsafe partial class Control_ActiveXImplTests
{
    [WinFormsFact]
    public void ActiveXImpl_SaveLoad_BinaryFormatterProperty_FormatterEnabled()
    {
        using MyControl control = new();

        // We need to have a type that doesn't have a TypeConverter that implements ISerializable to hit the
        // BinaryFormatter code path.
        SerializableStruct myValue = new() { Value = "HelloThere" };
        control.SerializableValue = myValue;
        IPersistStreamInit.Interface persistStream = control;

        using MemoryStream memoryStream = new();
        using var istream = memoryStream.ToIStream();
        HRESULT hr = persistStream.Save(istream.Value, fClearDirty: BOOL.FALSE);
        Assert.True(hr.Succeeded);
        control.SerializableValue = default;

        istream.Value->Seek(0, SeekOrigin.Begin);
        hr = persistStream.Load(istream.Value);
        Assert.True(hr.Succeeded);
        Assert.Equal(myValue, control.SerializableValue);
    }

    private class MyControl : Control
    {
        public SerializableStruct SerializableValue { get; set; }
    }

    private class BinaryFormatterPropertiesControl : Control
    {
        public Hashtable Table { get; set; }
    }

    [Serializable]
    public struct SerializableStruct : ISerializable
    {
        public string Value { get; set; }

        public readonly void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Value), Value, typeof(string));
        }

        private SerializableStruct(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            Value = (string)serializationInfo.GetValue(nameof(Value), typeof(string));
        }
    }
}

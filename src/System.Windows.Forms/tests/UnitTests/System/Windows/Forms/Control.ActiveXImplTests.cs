// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.Serialization;
using Windows.Win32.System.Com;
using static Interop.Ole32;

namespace System.Windows.Forms.Tests;

public unsafe class Control_ActiveXImplTests
{
    [WinFormsFact]
    public void ActiveXImpl_SaveLoad_RoundTrip_FormatterEnabled()
    {
        using var formatterScope = new BinaryFormatterScope(enable: true);
        using Control control = new();
        control.BackColor = Color.Bisque;
        IPersistStreamInit.Interface persistStream = control;

        using MemoryStream memoryStream = new();
        using var istream = ComHelpers.GetComScope<IStream>(new GPStream(memoryStream));
        HRESULT hr = persistStream.Save(istream.Value, fClearDirty: BOOL.FALSE);
        Assert.True(hr.Succeeded);
        control.BackColor = Color.Honeydew;

        istream.Value->Seek(0, SeekOrigin.Begin);
        hr = persistStream.Load(istream.Value);
        Assert.True(hr.Succeeded);
        Assert.Equal(Color.Bisque, control.BackColor);
    }

    [WinFormsFact]
    public void ActiveXImpl_SaveLoad_RoundTrip_FormatterDisabled()
    {
        using var formatterScope = new BinaryFormatterScope(enable: false);
        using Control control = new();
        control.BackColor = Color.Bisque;
        IPersistStreamInit.Interface persistStream = control;

        using MemoryStream memoryStream = new();
        using var istream = ComHelpers.GetComScope<IStream>(new GPStream(memoryStream));
        var istreamPointer = istream.Value;

        // Even though there are no properties that need BinaryFormatter in this case, it is ultimately saving
        // AxHost.PropertyBagStream, which itself uses BinaryFormatter to save its Hashtable. There is never
        // anything put in this Hashtable other than string/string pairs, so we *could* convert it to NOT use the
        // BinaryFormatter for the bag itself. There would have to be a config switch most likely and a compat
        // piece to look at the incoming stream to see if it is a BinaryFormatted stream.
        Assert.Throws<NotSupportedException>(() => persistStream.Save(istreamPointer, fClearDirty: BOOL.FALSE));
    }

    [WinFormsFact]
    public void ActiveXImpl_SaveLoad_BinaryFormatterProperty_FormatterEnabled()
    {
        using var formatterScope = new BinaryFormatterScope(enable: true);
        using MyControl control = new();

        // We need to have a type that doesn't have a TypeConverter that implements ISerializable to hit the
        // BinaryFormatter code path.
        SerializableStruct myValue = new() { Value = "HelloThere" };
        control.SerializableValue = myValue;
        IPersistStreamInit.Interface persistStream = control;

        using MemoryStream memoryStream = new();
        using var istream = ComHelpers.GetComScope<IStream>(new GPStream(memoryStream));
        HRESULT hr = persistStream.Save(istream.Value, fClearDirty: BOOL.FALSE);
        Assert.True(hr.Succeeded);
        control.SerializableValue = default;

        istream.Value->Seek(0, SeekOrigin.Begin);
        hr = persistStream.Load(istream.Value);
        Assert.True(hr.Succeeded);
        Assert.Equal(myValue, control.SerializableValue);
    }

    [WinFormsFact]
    public void ActiveXImpl_SaveLoad_BinaryFormatterProperty_FormatterDisabled()
    {
        using var formatterScope = new BinaryFormatterScope(enable: false);
        using MyControl control = new();

        // We need to have a type that doesn't have a TypeConverter that implements ISerializable to hit the
        // BinaryFormatter code path.
        SerializableStruct myValue = new() { Value = "HelloThere" };
        control.SerializableValue = myValue;
        IPersistStreamInit.Interface persistStream = control;

        using MemoryStream memoryStream = new();
        using var istream = ComHelpers.GetComScope<IStream>(new GPStream(memoryStream));
        var istreamPointer = istream.Value;
        Assert.Throws<NotSupportedException>(() => persistStream.Save(istreamPointer, fClearDirty: BOOL.FALSE));
    }

    private class MyControl : Control
    {
        public SerializableStruct SerializableValue { get; set; }
    }

    [Serializable]
    public struct SerializableStruct : ISerializable
    {
        public string Value { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Value), Value, typeof(string));
        }

        private SerializableStruct(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            Value = (string)serializationInfo.GetValue(nameof(Value), typeof(string));
        }
    }
}

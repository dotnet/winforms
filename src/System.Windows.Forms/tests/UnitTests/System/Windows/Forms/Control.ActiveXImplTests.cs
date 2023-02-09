﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.Serialization;
using Windows.Win32.System.Com;
using Xunit;
using static Interop.Ole32;

namespace System.Windows.Forms.Tests;

public unsafe class Control_ActiveXImplTests : IClassFixture<ThreadExceptionFixture>
{
    [WinFormsFact]
    public void ActiveXImpl_SaveLoad_RoundTrip()
    {
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
    public void ActiveXImpl_SaveLoad_BinaryFormatterProperty()
    {
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

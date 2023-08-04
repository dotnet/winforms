// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Drawing;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Windows.Forms.Tests;

public class DataObject_BitmapBinderTests
{
    private static readonly SerializationBinder s_serializationBinder
        = (SerializationBinder)Activator.CreateInstance(typeof(DataObject).Assembly.GetType("System.Windows.Forms.DataObject+BitmapBinder"));

    [WinFormsTheory]
    [MemberData(nameof(AllowedSerializationTypes))]
    public void BitmapBinder_BindToName_AllowedSerializationTypes(object value)
    {
        using BinaryFormatterScope formatterScope = new(enable: true);
        using (value as IDisposable)
        {
            using MemoryStream stream = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            BinaryFormatter formatter = new()
            {
                Binder = s_serializationBinder
            };
#pragma warning restore

            formatter.Serialize(stream, value);
            Assert.True(stream.Length > 0);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(AllowedSerializationTypes))]
    public void BitmapBinder_BindToType_AllowedSerializationTypes(object value)
    {
        using BinaryFormatterScope formatterScope = new(enable: true);
        using (value as IDisposable)
        {
            using MemoryStream stream = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            BinaryFormatter formatter = new();
#pragma warning restore
            formatter.Serialize(stream, value);
            Assert.True(stream.Length > 0);
            stream.Position = 0;

            formatter = new()
            {
                Binder = s_serializationBinder
            };

            // cs/dangerous-binary-deserialization
            object deserialized = formatter.Deserialize(stream); // CodeQL [SM03722] : Safe use because input stream is controlled contains strings and Bitmap which is instantiated by a binder. 
            Assert.NotNull(deserialized);

            if (value is not Bitmap)
            {
                Assert.Equal(value, deserialized);
            }
        }
    }

    public static TheoryData<object> AllowedSerializationTypes => new()
    {
        "Information At your Fingertips",
        new string[] { "Hello" },
        new Bitmap(5, 5)
    };

    [WinFormsTheory]
    [MemberData(nameof(DisallowedSerializationTypes))]
    public void BitmapBinder_BindToName_DisallowedSerializationTypes(object value)
    {
        using BinaryFormatterScope formatterScope = new(enable: true);
        using MemoryStream stream = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        BinaryFormatter formatter = new()
        {
            Binder = s_serializationBinder
        };
#pragma warning restore SYSLIB0011

        Assert.Throws<SerializationException>(() => formatter.Serialize(stream, value));
    }

    [WinFormsTheory]
    [MemberData(nameof(DisallowedSerializationTypes))]
    public void BitmapBinder_BindToType_DisallowedSerializationTypes(object value)
    {
        using BinaryFormatterScope formatterScope = new(enable: true);
        using MemoryStream stream = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        BinaryFormatter formatter = new();
#pragma warning restore SYSLIB0011
        formatter.Serialize(stream, value);
        Assert.True(stream.Length > 0);
        stream.Position = 0;

        formatter = new()
        {
            Binder = s_serializationBinder
        };

        Assert.Throws<SerializationException>(() => formatter.Deserialize(stream));
    }

    public static TheoryData<object> DisallowedSerializationTypes => new()
    {
        new List<string>() { "Hello" },
        new Hashtable() { { "Silver", "Hammer" } }
    };
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
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
            stream.Length.Should().BeGreaterThan(0);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(AllowedSerializationTypes))]
    public unsafe void BitmapBinder_BindToType_AllowedSerializationTypes(object value)
    {
        using BinaryFormatterScope formatterScope = new(enable: true);
        using (value as IDisposable)
        {
            using MemoryStream stream = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            // cs/binary-formatter-without-binder
            BinaryFormatter formatter = new(); // CodeQL [SM04191] This is a test. Safe because the deserialization process is performed on trusted data and the types are controlled and validated.
#pragma warning restore
            formatter.Serialize(stream, value);
            stream.Length.Should().BeGreaterThan(0);
            stream.Position = 0;

            formatter = new()
            {
                Binder = s_serializationBinder
            };

            // cs/dangerous-binary-deserialization
            object deserialized = formatter.Deserialize(stream); // CodeQL [SM03722] : Testing legacy feature. Safe use because input stream is controlled contains strings and Bitmap which is instantiated by a binder. 
            deserialized.Should().NotBeNull();

            if (value is not Bitmap bitmap)
            {
                deserialized.Should().BeEquivalentTo(value);
            }
            else
            {
                Bitmap deserializedBitmap = deserialized.Should().BeOfType<Bitmap>().Which;
                BitmapData originalData = bitmap.LockBits(default, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData deserializedData = deserializedBitmap.LockBits(default, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                try
                {
                    ReadOnlySpan<byte> originalBytes = new((byte*)originalData.Scan0, originalData.Stride * originalData.Height);
                    ReadOnlySpan<byte> deserializedBytes = new((byte*)deserializedData.Scan0, deserializedData.Stride * deserializedData.Height);
                    deserializedBytes.SequenceEqual(originalBytes).Should().BeTrue();
                }
                finally
                {
                    bitmap.UnlockBits(originalData);
                    deserializedBitmap.UnlockBits(deserializedData);
                }
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

        Action action = () => formatter.Serialize(stream, value);
        action.Should().Throw<SerializationException>();
    }

    [WinFormsTheory]
    [MemberData(nameof(DisallowedSerializationTypes))]
    public void BitmapBinder_BindToType_DisallowedSerializationTypes(object value)
    {
        using BinaryFormatterScope formatterScope = new(enable: true);
        using MemoryStream stream = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        // cs/binary-formatter-without-binder
        BinaryFormatter formatter = new(); // CodeQL [SM04191] : This is a test. Safe use because the deserialization process is performed on trusted data and the types are controlled and validated.
#pragma warning restore SYSLIB0011
        formatter.Serialize(stream, value);
        stream.Length.Should().BeGreaterThan(0);
        stream.Position = 0;

        formatter = new()
        {
            Binder = s_serializationBinder
        };

        Action action = () => formatter.Deserialize(stream);
        action.Should().Throw<SerializationException>();
    }

    public static TheoryData<object> DisallowedSerializationTypes => new()
    {
        new List<string>() { "Hello" },
        new Hashtable() { { "Silver", "Hammer" } }
    };
}

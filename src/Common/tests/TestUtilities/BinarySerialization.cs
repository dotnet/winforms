// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Formats.Nrbf;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace System;

public static class BinarySerialization
{
    /// <summary>
    ///  Ensures the list of types marked as serializable under <paramref name="assemblyUnderTest"/> matches
    ///  <paramref name="serializableTypes"/>. If not, <see cref="NotSupportedException"/> is thrown.
    /// </summary>
    public static void EnsureSerializableAttribute(Assembly assemblyUnderTest, HashSet<string> serializableTypes)
    {
        foreach (Type type in assemblyUnderTest.GetTypes())
        {
            var attributes = Attribute.GetCustomAttributes(type);
            string? fullName = type.FullName;
            if (fullName is null)
            {
                continue;
            }

            if (!attributes.Any(a => a is SerializableAttribute))
            {
                // The type isn't marked as serializable, verify it is not one of the types
                // that we expect to be serializable
                if (serializableTypes.Contains(fullName))
                {
                    throw new NotSupportedException($"Serializable attribute is expected on {fullName}");
                }

                continue;
            }

            if (attributes.Any(a => a is CompilerGeneratedAttribute))
            {
                // ignore compiler generated types, we have no control over them
                continue;
            }

            // Ensure SerializableAttribute is not added to any types not in the known list.
            if (serializableTypes.Contains(fullName))
            {
                // we have marked the type as serializable, all good
                continue;
            }

            throw new NotSupportedException($"Serializable attribute is not expected on {type.FullName}");
        }
    }

    /// <summary>
    ///  Binary deserializes a base 64 string to <typeparamref name="T"/>. <paramref name="blob"/> is binary
    ///  deserialized with <see cref="BinaryFormatter"/> with <paramref name="assemblyStyle"/> taken into account.
    /// </summary>
#pragma warning disable SYSLIB0050 // Type or member is obsolete
    public static T EnsureDeserialize<T>(string blob, FormatterAssemblyStyle assemblyStyle = FormatterAssemblyStyle.Simple)
    {
        object @object = FromBase64String(blob, assemblyStyle);
        Assert.NotNull(@object);
        return Assert.IsType<T>(@object);

        static object FromBase64String(string base64String, FormatterAssemblyStyle assemblyStyle)
        {
            byte[] raw = Convert.FromBase64String(base64String);
            return FromByteArray(raw, assemblyStyle);
        }

        static object FromByteArray(byte[] raw, FormatterAssemblyStyle assemblyStyle)
        {
            using MemoryStream serializedStream = new(raw);
            return DeserializeFromStream(serializedStream);
        }
    }

    /// <summary>
    ///  Returns a base 64 string of the binary serialized <paramref name="object"/>.
    ///  <paramref name="object"/> is binary serialized using <see cref="BinaryFormatter"/>
    ///  with <paramref name="assemblyFormat"/> taken into account.
    /// </summary>
    public static string ToBase64String(
        object @object,
        FormatterAssemblyStyle assemblyFormat = FormatterAssemblyStyle.Simple)
    {
        byte[] raw = ToByteArray(@object, assemblyFormat);
        return Convert.ToBase64String(raw);

        static byte[] ToByteArray(object @object, FormatterAssemblyStyle assemblyFormat)
        {
            using MemoryStream stream = new();
            stream.WriteBinaryFormat(@object, assemblyFormat: assemblyFormat);
            return stream.ToArray();
        }
    }

    /// <summary>
    ///  Serializes the specified object using <see cref="BinaryFormatter"/>.
    /// </summary>
    /// <param name="object">The object to clone.</param>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="assemblyFormat">The assembly format to use.</param>
    /// <param name="typeFormat">The type format to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when the specified object is <see langword="null"/>.</exception>
    public static void WriteBinaryFormat(
        this Stream stream,
        object @object,
        FormatterAssemblyStyle assemblyFormat = FormatterAssemblyStyle.Simple,
        FormatterTypeStyle typeFormat = FormatterTypeStyle.TypesAlways)
    {
        ArgumentNullException.ThrowIfNull(@object);

#pragma warning disable SYSLIB0011 // Type or member is obsolete
        BinaryFormatter formatter = new() // CodeQL [SM04191]: Safe use because the deserialization process is performed on trusted data and the types are controlled and validated.
        {
#pragma warning restore SYSLIB0011

            AssemblyFormat = assemblyFormat,
            TypeFormat = typeFormat
        };

        using BinaryFormatterScope scope = new(enable: true);
        formatter.Serialize(stream, @object);
    }

    /// <summary>
    ///  Deserializes the specified stream using <see cref="BinaryFormatter"/>.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="assemblyFormat">The assembly format to use.</param>
    /// <param name="binder">The serialization binder to use, if any.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the specified stream is <see langword="null"/>.</exception>
    /// <exception cref="SerializationException">Thrown when an error occurs during deserialization.</exception>
    public static object DeserializeFromStream(
        Stream stream,
        FormatterAssemblyStyle assemblyFormat = FormatterAssemblyStyle.Simple,
        SerializationBinder? binder = null)
    {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        BinaryFormatter formatter = new() // CodeQL [SM04191]: Safe use because the deserialization process is performed on trusted data and the types are controlled and validated.
        {
#pragma warning restore SYSLIB0011 // Type or member is obsolete
            AssemblyFormat = assemblyFormat,
            Binder = binder
        };

        using BinaryFormatterScope scope = new(enable: true);
        return formatter.Deserialize(stream);  // CodeQL[SM03722] : Testing legacy feature. This is a safe use of BinaryFormatter because the data is trusted and the types are controlled and validated.
    }
#pragma warning restore SYSLIB0050

#pragma warning disable SYSLIB5005 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    /// <summary>
    ///  Formats the given object to a binary stream and decodes it using the NRBF deserializer.
    /// </summary>
    /// <param name="object">The object to binary format.</param>
    /// <inheritdoc cref="NrbfDecoder.Decode(Stream, out IReadOnlyDictionary{SerializationRecordId, SerializationRecord}, PayloadOptions?, bool)"/>
    public static SerializationRecord SerializeAndDecode(
        this object @object,
        out IReadOnlyDictionary<SerializationRecordId, SerializationRecord> recordMap)
    {
        using MemoryStream stream = new();
        stream.WriteBinaryFormat(@object);
        stream.Position = 0;
        return NrbfDecoder.Decode(stream, out recordMap, options: null, leaveOpen: true);
    }

    public static SerializationRecord SerializeAndDecode(this object @object) =>
        SerializeAndDecode(@object, out _);
#pragma warning restore SYSLIB5005
}

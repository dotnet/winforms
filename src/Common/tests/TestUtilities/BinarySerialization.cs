// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace System;

public static class BinarySerialization
{
    /// <summary>
    ///  Ensures the list of types marked as serializable under <paramref name="assemblyUnderTest"/> matches <paramref name="serializableTypes"/>.
    ///  If not, <see cref="NotSupportedException"/> is thrown.
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
                // the type isn't marked as serializable, verify it is not one of the types
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
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            // cs/binary-formatter-without-binder
            BinaryFormatter binaryFormatter = new() // CodeQL [SM04191] : Safe use because the deserialization process is performed on trusted data and the types are controlled and validated.
            {
                AssemblyFormat = assemblyStyle
            };
#pragma warning restore SYSLIB0011

            using MemoryStream serializedStream = new(raw);

            // cs/dangerous-binary-deserialization
            return binaryFormatter.Deserialize(serializedStream); // CodeQL[SM03722] : Testing legacy feature. This is a safe use of BinaryFormatter because the data is trusted and the types are controlled and validated.
        }
    }

    /// <summary>
    ///  Returns a base 64 string of the binary serialized <paramref name="object"/>.
    ///  <paramref name="object"/> is binary serialized using <see cref="BinaryFormatter"/>
    ///  with <paramref name="assemblyStyle"/> taken into account.
    /// </summary>
    public static string ToBase64String(object @object,
        FormatterAssemblyStyle assemblyStyle = FormatterAssemblyStyle.Simple)
    {
        byte[] raw = ToByteArray(@object, assemblyStyle);
        return Convert.ToBase64String(raw);

        static byte[] ToByteArray(object obj, FormatterAssemblyStyle assemblyStyle)
        {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            // cs/binary-formatter-without-binder
            BinaryFormatter binaryFormatter = new() // CodeQL [SM04191]: Safe use because the deserialization process is performed on trusted data and the types are controlled and validated.
            {
                AssemblyFormat = assemblyStyle
            };
#pragma warning restore SYSLIB0011

            using MemoryStream stream = new();
            binaryFormatter.Serialize(stream, obj);
            return stream.ToArray();
        }
    }
#pragma warning restore SYSLIB0050
}

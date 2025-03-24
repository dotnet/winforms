// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Private.Windows.Nrbf;

#pragma warning disable CA2300 // Do not use insecure deserializer BinaryFormatter
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable SYSLIB0011 // Type or member is obsolete
#pragma warning disable SYSLIB0050 // Type or member is obsolete

/// <summary>
///  Provides helper methods for working with <see cref="BinaryFormatter"/>.
/// </summary>
public static class BinaryFormatterHelpers
{
    /// <summary>
    ///  Clones the specified object using <see cref="BinaryFormatter"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to clone.</typeparam>
    /// <param name="object">The object to clone.</param>
    /// <param name="surrogate">The serialization surrogate to use, if any.</param>
    /// <param name="assemblyFormat">The assembly format to use.</param>
    /// <param name="typeFormat">The type format to use.</param>
    /// <returns>A clone of the specified object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the specified object is <see langword="null"/>.</exception>
    internal static T Clone<T>(T @object,
        ISerializationSurrogate? surrogate = null,
        FormatterAssemblyStyle assemblyFormat = FormatterAssemblyStyle.Full,
        FormatterTypeStyle typeFormat = FormatterTypeStyle.TypesAlways)
    {
        ArgumentNullException.ThrowIfNull(@object);

        BinaryFormatter formatter;
        if (surrogate is null)
        {
            formatter = new();
        }
        else
        {
            StreamingContext context = default;
            SurrogateSelector selector = new();
            selector.AddSurrogate(@object.GetType(), context, surrogate);
            formatter = new(selector, context);
        }

        formatter.AssemblyFormat = assemblyFormat;
        formatter.TypeFormat = typeFormat;

        using var stream = new MemoryStream();
        formatter.Serialize(stream, @object);
        Assert.NotEqual(0, stream.Position);
        stream.Position = 0;
        return (T)formatter.Deserialize(stream);
    }

    /// <summary>
    ///  Serializes the specified object to a byte array using <see cref="BinaryFormatter"/>.
    /// </summary>
    /// <param name="object">The object to serialize.</param>
    /// <param name="assemblyStyle">The assembly style to use.</param>
    /// <returns>A byte array representing the serialized object.</returns>
    public static byte[] ToByteArray(
        object @object,
        FormatterAssemblyStyle assemblyStyle = FormatterAssemblyStyle.Full)
    {
        BinaryFormatter formatter = new()
        {
            AssemblyFormat = assemblyStyle
        };

        using MemoryStream stream = new();
        formatter.Serialize(stream, @object);
        return stream.ToArray();
    }

    /// <summary>
    ///  Converts the specified object to a Base64 string using <see cref="BinaryFormatter"/>.
    /// </summary>
    /// <param name="object">The object to convert.</param>
    /// <param name="assemblyStyle">The assembly style to use.</param>
    /// <returns>A Base64 string representing the serialized object.</returns>
    public static string ToBase64String(
        object @object,
        FormatterAssemblyStyle assemblyStyle = FormatterAssemblyStyle.Full)
    {
        byte[] raw = ToByteArray(@object, assemblyStyle);
        return Convert.ToBase64String(raw);
    }

    /// <summary>
    ///  Deserializes an object from a byte array using <see cref="BinaryFormatter"/>.
    /// </summary>
    /// <param name="raw">The byte array to deserialize.</param>
    /// <param name="assemblyStyle">The assembly style to use.</param>
    /// <returns>The deserialized object.</returns>
    public static object FromByteArray(
        byte[] raw,
        FormatterAssemblyStyle assemblyStyle = FormatterAssemblyStyle.Full)
    {
        BinaryFormatter binaryFormatter = new()
        {
            AssemblyFormat = assemblyStyle
        };

        using var serializedStream = new MemoryStream(raw);
        return binaryFormatter.Deserialize(serializedStream);
    }

    /// <summary>
    ///  Deserializes an object from a Base64 string using <see cref="BinaryFormatter"/>.
    /// </summary>
    /// <param name="base64String">The Base64 string to deserialize.</param>
    /// <param name="assemblyStyle">The assembly style to use.</param>
    /// <returns>The deserialized object.</returns>
    public static object FromBase64String(
        string base64String,
        FormatterAssemblyStyle assemblyStyle = FormatterAssemblyStyle.Full)
    {
        byte[] raw = Convert.FromBase64String(base64String);
        return FromByteArray(raw, assemblyStyle);
    }

    /// <summary>
    ///  Serializes the specified object to the provided stream using <see cref="BinaryFormatter"/>.
    /// </summary>
    /// <param name="object"></param>
    /// <param name="stream"></param>
    public static void SerializeToStream(object @object, Stream stream)
    {
        long position = stream.Position;
        new BinaryFormatter().Serialize(stream, @object);
        stream.Position = position;
    }
}

#pragma warning restore CA2300 // Do not use insecure deserializer BinaryFormatter
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning restore SYSLIB0011 // Type or member is obsolete
#pragma warning restore SYSLIB0050 // Type or member is obsolete

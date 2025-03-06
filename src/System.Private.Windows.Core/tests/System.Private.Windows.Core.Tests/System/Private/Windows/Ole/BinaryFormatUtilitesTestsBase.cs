// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Private.Windows.BinaryFormat;
using System.Reflection.Metadata;

namespace System.Private.Windows.Ole.Tests;

public abstract class BinaryFormatUtilitesTestsBase : IDisposable
{
    public enum DataType
    {
        Json,
        BinaryFormat
    }

    public MemoryStream CreateStream<T>(DataType dataType, T data) where T : notnull
    {
        MemoryStream stream = new();
        if (dataType == DataType.Json)
        {
            BinaryFormatWriter.TryWriteJsonData(stream, IJsonData.Create(data));
        }
        else
        {
            WriteObjectToStream(stream, data, "test");
        }

        stream.Position = 0;
        return stream;
    }

    protected MemoryStream Stream { get; }

    public BinaryFormatUtilitesTestsBase() => Stream = new();

    public void Dispose() => Stream.Dispose();

    protected abstract void WriteObjectToStream(MemoryStream stream, object data, string format);

    protected abstract bool TryReadObjectFromStream<T>(
        MemoryStream stream,
        bool untypedRequest,
        string format,
        Func<TypeName, Type>? resolver,
        [NotNullWhen(true)] out T? @object);

    protected void WriteObjectToStream(object value, bool restrictSerialization = false) =>
        WriteObjectToStream(Stream, value, restrictSerialization ? DataFormatNames.String : "test");

    protected bool TryReadObjectFromStream<T>(out T? @object)
    {
        Stream.Position = 0;
        return TryReadObjectFromStream(Stream, untypedRequest: true, "test", resolver: null, out @object);
    }

    protected bool TryReadPredefinedObjectFromStream<T>(out T? @object)
    {
        Stream.Position = 0;
        return TryReadObjectFromStream(Stream, untypedRequest: true, DataFormatNames.String, resolver: null, out @object);
    }

    protected bool ReadObjectFromStream<T>(bool restrictDeserialization, Func<TypeName, Type>? resolver, out T? @object)
    {
        Stream.Position = 0;

        return TryReadObjectFromStream(
            Stream,
            untypedRequest: false,
            restrictDeserialization ? DataFormatNames.String : "test",
            resolver,
            out @object);
    }

    protected bool TryReadObjectFromStream<T>(Func<TypeName, Type>? resolver, out T? @object)
    {
        Stream.Position = 0;
        return TryReadObjectFromStream(Stream, untypedRequest: false, "test", resolver, out @object);
    }

    protected bool ReadPredefinedObjectFromStream<T>(Func<TypeName, Type>? resolver, out T? @object)
    {
        return TryReadObjectFromStream(Stream, untypedRequest: false, DataFormatNames.String, resolver, out @object);
    }

    protected bool RoundTripObject<T>(object value, out T? @object)
    {
        // This is equivalent to SetData/GetData methods with unbounded formats,
        // and works with the BinaryFormat AppContext switches.
        WriteObjectToStream(value);
        return TryReadObjectFromStream(out @object);
    }

    protected bool RoundTripObject_PredefinedFormat<T>(object value, out T? @object)
    {
        // This is equivalent to SetData/GetData methods using registered OLE formats, resolves only the known types
        WriteObjectToStream(value, restrictSerialization: true);
        return TryReadPredefinedObjectFromStream(out @object);
    }

    protected bool RoundTripOfType<T>(object value, out T? @object)
    {
        // This is equivalent to SetData/TryGetData<T> methods using unbounded OLE formats,
        // and works with the BinaryFormat AppContext switches.
        WriteObjectToStream(value);
        return TryReadObjectFromStream(NotSupportedResolver, out @object);
    }

    protected bool RoundTripOfType_PredefinedFormat<T>(object value, out T? @object)
    {
        // This is equivalent to SetData/TryGetData<T> methods using OLE formats. Deserialization is Predefined
        // to known types.
        WriteObjectToStream(value, restrictSerialization: true);
        Stream.Position = 0;
        return ReadPredefinedObjectFromStream(NotSupportedResolver, out @object);
    }

    protected bool RoundTripOfType<T>(object value, Func<TypeName, Type>? resolver, out T? @object)
    {
        // This is equivalent to SetData/TryGetData<T> methods using unbounded formats,
        // serialization is Predefined by the resolver and BinaryFormat AppContext switches.
        WriteObjectToStream(value);
        return TryReadObjectFromStream(resolver, out @object);
    }

    protected static Type NotSupportedResolver(TypeName typeName) =>
        throw new NotSupportedException($"Can't resolve {typeName.AssemblyQualifiedName}");
}

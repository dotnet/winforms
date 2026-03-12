// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Formats.Nrbf;
using System.Private.Windows.Nrbf;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Private.Windows.Ole;

internal static partial class BinaryFormatUtilities<TNrbfSerializer>
#if NET
    where TNrbfSerializer : INrbfSerializer
#else
    where TNrbfSerializer : INrbfSerializer, new()
#endif
{
#if NETFRAMEWORK
    private static readonly INrbfSerializer s_nrbfSerializer = new TNrbfSerializer();
#endif

    /// <summary>
    ///  Writes an object to the provided memory stream.
    /// </summary>
    /// <param name="stream">The memory stream to write the object to.</param>
    /// <param name="data">The object to write to the stream.</param>
    /// <param name="format">The format of the data being written.</param>
    /// <exception cref="NotSupportedException">
    ///  Thrown when <see cref="BinaryFormatter"/> is not supported.
    /// </exception>
    internal static void WriteObjectToStream(MemoryStream stream, object data, string format)
    {
        Debug.Assert(data is not null);
        long position = stream.Position;

        try
        {
#if NET
            if (TNrbfSerializer.TryWriteObject(stream, data))
#else
            if (s_nrbfSerializer.TryWriteObject(stream, data!))
#endif
            {
                return;
            }
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
            // Being extra cautious here, but the Try method above should never throw in normal circumstances.
            Debug.Fail($"Unexpected exception writing binary formatted data. {ex.Message}");
        }

        if (!FeatureSwitches.EnableUnsafeBinaryFormatterSerialization)
        {
            // BinaryFormatter isn't enabled for anything.
            throw new NotSupportedException(SR.BinaryFormatterNotSupported);
        }

        if (!CoreAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization)
        {
            // BinaryFormatter is enabled, but not for clipboard or drag/drop.
            throw new NotSupportedException(SR.BinaryFormatter_NotSupported_InClipboardOrDragDrop);
        }

        if (DataFormatNames.IsPredefinedFormat(format))
        {
            // Serializing is never unsafe, but matching the exception on read for convenience.
            throw new RestrictedTypeDeserializationException(SR.UnexpectedClipboardType);
        }

        stream.Position = position;
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        new BinaryFormatter().Serialize(stream, data);
#pragma warning restore SYSLIB0011
    }

    /// <summary>
    ///  Reads an object from the provided memory stream.
    /// </summary>
    /// <typeparam name="T">The type of the object to read.</typeparam>
    /// <param name="stream">The memory stream to read the object from.</param>
    /// <param name="request">The data request containing the format and resolver.</param>
    /// <returns>The deserialized object, or <see langword="null"/> if the data is not of the given type.</returns>
    /// <exception cref="NotSupportedException">Thrown when <see cref="BinaryFormatter"/> is not enabled.</exception>
    internal static bool TryReadObjectFromStream<T>(
        MemoryStream stream,
        ref readonly DataRequest request,
        [NotNullWhen(true)] out T? @object)
    {
        Debug.Assert(request.TypedRequest || typeof(T) == typeof(object), "Untyped requests should always be asking for object");

        @object = default;
        object? value;

        if (typeof(T) == typeof(MemoryStream))
        {
            // Explicitly asked for a MemoryStream, return the stream as is.
            @object = (T)(object)stream;
            return true;
        }

        long startPosition = stream.Position;

        SerializationRecord? record = null;
        try
        {
            record = stream.DecodeNrbf();
            if (typeof(T) == typeof(SerializationRecord))
            {
                // If SerializationRecord was explicitly requested, return the decoded stream.
                // This allows the caller to manually inspect and handle legacy data.
                @object = (T)(object)record;
                return true;
            }
        }
        catch (NotSupportedException)
        {
            // Allow falling through for unsupported Nrbf data (such as non-zero based arrays).
        }
        finally
        {
            stream.Position = startPosition;
        }

        TypeBinder<TNrbfSerializer> binder = new(typeof(T), in request);

        if (record is not null)
        {
            // Try our implicit deserialization.
#if NET
            if (TNrbfSerializer.TryBindToType(record.TypeName, out Type? type))
#else
            if (s_nrbfSerializer.TryBindToType(record.TypeName, out Type? type))
#endif
            {
                if (request.TypedRequest
                    // If we can't match the root exactly, then we fall back to the binder.
                    && !(type == typeof(T) || binder.BindToType(record.TypeName).IsAssignableTo(typeof(T))))
                {
                    return false;
                }

#if NET
                if (TNrbfSerializer.TryGetObject(record, out value))
#else
                if (s_nrbfSerializer.TryGetObject(record, out value))
#endif
                {
                    @object = (T)value;
                    return true;
                }
#if NET
                else if (TNrbfSerializer.IsFullySupportedType(type))
#else
                else if (s_nrbfSerializer.IsFullySupportedType(type))
#endif
                {
                    // The serializer fully supports this type, but can't deserialize it.
                    // Don't let it fall through to the BinaryFormatter.
                    return false;
                }
            }

#if OLE_JSON
            if (type is null)
            {
                // Serializer didn't recognize the type, look for and deserialize a JSON object.
                var (isJsonData, isValidType) = record.TryGetObjectFromJson(binder, out @object);
                if (isJsonData)
                {
                    return isValidType;
                }
            }
#endif

            // JSON type info is nested, so this has to come after the JSON attempt.
            if (request.TypedRequest && !typeof(T).Matches(record.TypeName, TypeNameComparison.AllButAssemblyVersion))
            {
                if (!binder.BindToType(record.TypeName).IsAssignableTo(typeof(T)))
                {
                    // Typed request where the root type is not what was requested.
                    // Untyped requests are allowed to deserialize any type.
                    return false;
                }
            }
        }

        if (request.TypedRequest && request.Resolver is null)
        {
            // Never allow the BinaryFormatter without an explicit resolver. This ensures users know that they
            // cannot hit the BinaryFormatter under any circumstances from other TryGet APIs when working with
            // primitive types or JSON serialized objects.
            throw new NotSupportedException(string.Format(SR.ClipboardOrDragDrop_UseTypedAPI, typeof(T).FullName));
        }

        if (!FeatureSwitches.EnableUnsafeBinaryFormatterSerialization)
        {
            // BinaryFormatter isn't enabled for anything.
            throw new NotSupportedException(SR.BinaryFormatterNotSupported);
        }

        if (!CoreAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization)
        {
            // BinaryFormatter is enabled, but not for clipboard or drag/drop.
            throw new NotSupportedException(SR.BinaryFormatter_NotSupported_InClipboardOrDragDrop);
        }

        if (DataFormatNames.IsPredefinedFormat(request.Format))
        {
            // These format should have been handled above (binary formatted primitive or array of primitives).
            // We should never let them get to the actual BinaryFormatter or do any other unbounded deserialization.
            throw new RestrictedTypeDeserializationException(SR.UnexpectedClipboardType);
        }

        // If we want to attempt our own general NRBF deserialization, we would do it here.
        // If we do, we should never fall back to the BinaryFormatter if the binder fails
        // to bind a type.

        binder ??= new(typeof(T), in request);

#pragma warning disable SYSLIB0011, SYSLIB0050 // Type or member is obsolete
#pragma warning disable CA2300 // Do not use insecure deserializer BinaryFormatter
#pragma warning disable CA2302 // Ensure BinaryFormatter.Binder is set before calling BinaryFormatter.Deserialize
        try
        {
            value = new BinaryFormatter()
            {
                Binder = binder,
                // Don't consider assembly versions when deserializing.
                AssemblyFormat = FormatterAssemblyStyle.Simple
            }.Deserialize(stream);
        }
        catch (SerializationException e) when (e.InnerException is NotSupportedException nse)
        {
            // Rethrow the inner exception to allow the exception to flow up through TryGetHGLOBALData.
            throw nse;
        }
        finally
        {
            stream.Position = startPosition;
        }
#pragma warning restore CA2300
#pragma warning restore CA2302
#pragma warning restore SYSLIB0050, SYSLIB0011

        if (value is T t)
        {
            @object = t;
            return true;
        }

        return false;
    }
}

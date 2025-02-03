// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Formats.Nrbf;
using System.Private.Windows.Nrbf;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Private.Windows.Ole;

internal static class BinaryFormatUtilities<TNrbfSerializer> where TNrbfSerializer : INrbfSerializer
{
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
            if (TNrbfSerializer.TryWriteObject(stream, data))
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

        if (DataFormatNames.IsRestrictedFormat(format))
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
        @object = default;
        object? value = null;

        long startPosition = stream.Position;

        SerializationRecord? record = null;
        IReadOnlyDictionary<SerializationRecordId, SerializationRecord>? recordMap = null;
        try
        {
            record = stream.DecodeNrbf(out recordMap);
        }
        catch (NotSupportedException)
        {
            // Allow falling through for unsupported Nrbf data (such as non-zero based arrays).
        }

        TypeBinder<TNrbfSerializer>? binder = null;

        if (record is not null && recordMap is not null)
        {
            // Look for and deserialize a JSON object.
            binder = new(typeof(T), in request);
            var (isJsonData, isValidType) = record.TryGetObjectFromJson(binder, out @object);
            if (isJsonData)
            {
                return isValidType;
            }

            // Try our implicit deserialization.
            if (TNrbfSerializer.TryBindToType(record.TypeName, out Type? type)
                && type.IsAssignableTo(typeof(T))
                && TNrbfSerializer.TryGetObject(record, out value)
                && value is T matching)
            {
                @object = matching;
                return true;
            }
        }

        if (!request.UntypedRequest
            && record is not null
            && !typeof(T).MatchExceptAssemblyVersion(record.TypeName)
            && (TryResolve(request.Resolver, record.TypeName) is not { } resolvedType
                || !resolvedType.MatchExceptAssemblyVersion(record.TypeName)))
        {
            // Typed request where the root type is not what was requested.
            // Untyped requests are allowed to deserialize any type.
            return false;
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

        if (DataFormatNames.IsRestrictedFormat(request.Format))
        {
            // These format should have been handled above (binary formatted primitive or array of primitives).
            // We should never let them get to the actual BinaryFormatter or do any other unbounded deserialization.
            throw new RestrictedTypeDeserializationException(SR.UnexpectedClipboardType);
        }

        // If we want to attempt our own general NRBF deserialization, we would do it here.
        // If we do, we should never fall back to the BinaryFormatter if the binder fails
        // to bind the type.

        stream.Position = startPosition;
        binder ??= new(typeof(T), in request);

#pragma warning disable SYSLIB0011, SYSLIB0050 // Type or member is obsolete
#pragma warning disable CA2300 // Do not use insecure deserializer BinaryFormatter
#pragma warning disable CA2302 // Ensure BinaryFormatter.Binder is set before calling BinaryFormatter.Deserialize
        try
        {
            value = new BinaryFormatter()
            {
                Binder = binder,
                AssemblyFormat = FormatterAssemblyStyle.Simple
            }.Deserialize(stream);
        }
        catch (SerializationException e) when (e.InnerException is NotSupportedException nse)
        {
            // Rethrow the inner exception to allow the exception to flow up through TryGetHGLOBALData.
            throw nse;
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

        static Type? TryResolve(Func<TypeName, Type>? resolver, TypeName typeName)
        {
            try
            {
                return resolver?.Invoke(typeName);
            }
            catch (Exception e) when (!e.IsCriticalException())
            {
                return null;
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Formats.Nrbf;
using System.Private.Windows.Core.BinaryFormat;
using System.Reflection.Metadata;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms.BinaryFormat;
using System.Windows.Forms.Nrbf;
using System.Windows.Forms.Primitives;

namespace System.Windows.Forms;

public unsafe partial class DataObject
{
    internal unsafe partial class Composition
    {
        internal static class BinaryFormatUtilities
        {
            internal static void WriteObjectToStream(MemoryStream stream, object data, bool restrictSerialization)
            {
                long position = stream.Position;

                try
                {
                    if (WinFormsBinaryFormatWriter.TryWriteCommonObject(stream, data))
                    {
                        return;
                    }
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    // Being extra cautious here, but the Try method above should never throw in normal circumstances.
                    Debug.Fail($"Unexpected exception writing binary formatted data. {ex.Message}");
                }

                if (restrictSerialization)
                {
                    throw new SerializationException(string.Format(SR.UnexpectedTypeForClipboardFormat, data.GetType().FullName));
                }

                // This check is to help in trimming scenarios with a trim warning on a call to
                // BinaryFormatter.Serialize(), which has a RequiresUnreferencedCode annotation.
                // If the flag is false, the trimmer will not generate a warning, since BinaryFormatter.Serialize(),
                // will not be called,
                // If the flag is true, the trimmer will generate a warning for calling a method that has a
                // RequiresUnreferencedCode annotation.
                if (!EnableUnsafeBinaryFormatterInNativeObjectSerialization)
                {
                    throw new NotSupportedException(SR.BinaryFormatterNotSupported);
                }

                if (!LocalAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization)
                {
                    throw new NotSupportedException(SR.BinaryFormatter_NotSupported_InClipboardOrDragDrop);
                }

                stream.Position = position;
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                new BinaryFormatter().Serialize(stream, data);
#pragma warning restore SYSLIB0011
            }

            internal static object? ReadObjectFromStream<T>(
                MemoryStream stream,
                Func<TypeName, Type>? resolver,
                bool legacyMode)
            {
                long startPosition = stream.Position;
                SerializationRecord? record;
                SerializationBinder binder = new Binder(typeof(T), resolver, legacyMode);
                IReadOnlyDictionary<SerializationRecordId, SerializationRecord> recordMap;

                try
                {
                    record = stream.Decode(out recordMap);
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    // Couldn't parse for some reason, let BinaryFormatter handle the legacy invocation.
                    // The typed APIs can't compare the specified type when the root record is not available.
                    if (legacyMode && LocalAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization)
                    {
                        stream.Position = startPosition;
                        return ReadObjectWithBinaryFormatter<T>(stream, binder);
                    }

                    // For example offset arrays throw from the decoder -
                    // https://learn.microsoft.com/dotnet/api/system.array.createinstance?#system-array-createinstance(system-type-system-int32()-system-int32())
                    if (ex is NotSupportedException)
                    {
                        throw;
                    }

                    throw ExceptionDispatchInfo.SetRemoteStackTrace(
                        new NotSupportedException(ex.Message, ex), ex.StackTrace ?? string.Empty);
                }

                // For the new TryGet APIs, ensure that the stream contains the requested type,
                // or type that can be assigned to the requested type.
                Type type = typeof(T);
                if (!legacyMode && !type.MatchExceptAssemblyVersion(record.TypeName))
                {
                    if (record.TryGetObjectFromJson<T>((ITypeResolver)binder, out object? data))
                    {
                        return data;
                    }

                    if (!TypeNameIsAssignableToType(record.TypeName, type, (ITypeResolver)binder))
                    {
                        // If clipboard contains an exception from SetData, we will get its message and throw.
                        if (record.TypeName.FullName == typeof(NotSupportedException).FullName
                            && record.TryGetNotSupportedException(out object? @object)
                            && @object is NotSupportedException exception)
                        {
                            throw new NotSupportedException(string.Format(SR.NotSupportedExceptionOnClipboard, exception.Message));
                        }

                        return null;
                    }
                }

                if (record.TryGetCommonObject(out object? value))
                {
                    return value;
                }

                if (!LocalAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization)
                {
                    throw new NotSupportedException(string.Format(
                        SR.BinaryFormatter_NotSupported_InClipboardOrDragDrop_UseTypedAPI,
                        typeof(T).FullName));
                }

                // NRBF deserializer fixes some known BinaryFormatter issues:
                // 1. Doesn't allow arrays that have a non-zero base index (can't create these in C# or VB)
                // 2. Only allows IObjectReference types that contain primitives (to avoid observable cycle
                //    dependencies to indeterminate state)
                // But it usually requires a resolver. Resolver is not available in the legacy mode,
                // so we will fall back to BinaryFormatter in that case.
                if (LocalAppContextSwitches.ClipboardDragDropEnableNrbfSerialization)
                {
                    try
                    {
                        return record.Deserialize(recordMap, (ITypeResolver)binder);
                    }
                    catch (Exception ex) when (!ex.IsCriticalException() && legacyMode)
                    {
                    }
                }

                stream.Position = startPosition;
                return ReadObjectWithBinaryFormatter<T>(stream, binder);
            }

            internal static object? ReadRestrictedObjectFromStream<T>(
                MemoryStream stream,
                Func<TypeName, Type>? resolver,
                bool legacyMode)
            {
                long startPosition = stream.Position;
                SerializationRecord? record;

                try
                {
                    record = stream.Decode();
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    throw new RestrictedTypeDeserializationException(SR.UnexpectedClipboardType);
                }

                // For the new TryGet APIs, ensure that the stream contains the requested type,
                // or type that can be assigned to the requested type.
                Type type = typeof(T);
                if (!legacyMode && !type.MatchExceptAssemblyVersion(record.TypeName))
                {
                    if (!TypeNameIsAssignableToType(record.TypeName, type, new Binder(type, resolver, legacyMode)))
                    {
                        // If clipboard contains an exception from SetData, we will get its message and throw.
                        if (record.TypeName.FullName == typeof(NotSupportedException).FullName
                            && record.TryGetNotSupportedException(out object? @object)
                            && @object is NotSupportedException exception)
                        {
                            throw new NotSupportedException(exception.Message);
                        }

                        return null;
                    }
                }

                return record.TryGetCommonObject(out object? value)
                    ? value
                    : throw new RestrictedTypeDeserializationException(SR.UnexpectedClipboardType);
            }

            private static bool TypeNameIsAssignableToType(TypeName typeName, Type type, ITypeResolver resolver)
            {
                try
                {
                    return resolver.GetType(typeName)?.IsAssignableTo(type) == true;
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    // Clipboard contains a wrong type, we want the typed API to return false to the caller.
                }

                return false;
            }

            private static object? ReadObjectWithBinaryFormatter<T>(MemoryStream stream, SerializationBinder binder)
            {
                // This check is to help in trimming scenarios with a trim warning on a call to BinaryFormatter.Deserialize(),
                // which has a RequiresUnreferencedCode annotation.
                // If the flag is false, the trimmer will not generate a warning, since BinaryFormatter.Deserialize() will not be called,
                // If the flag is true, the trimmer will generate a warning for calling a method that has a RequiresUnreferencedCode annotation.
                if (!EnableUnsafeBinaryFormatterInNativeObjectSerialization)
                {
                    throw new NotSupportedException(SR.BinaryFormatterNotSupported);
                }

#pragma warning disable SYSLIB0011, SYSLIB0050 // Type or member is obsolete
#pragma warning disable CA2300 // Do not use insecure deserializer BinaryFormatter
#pragma warning disable CA2302 // Ensure BinaryFormatter.Binder is set before calling BinaryFormatter.Deserialize
                // cs/dangerous-binary-deserialization
                return new BinaryFormatter()
                {
                    Binder = binder,
                    AssemblyFormat = FormatterAssemblyStyle.Simple
                }.Deserialize(stream); // CodeQL[SM03722] : BinaryFormatter is intended to be used as a fallback for unsupported types. Users must explicitly opt into this behavior.
#pragma warning restore CA2300
#pragma warning restore CA2302
#pragma warning restore SYSLIB0050, SYSLIB0011
            }
        }
    }
}

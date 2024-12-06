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
                    // TODO (TanyaSo): localize string
                    throw new NotSupportedException("BinaryFormatter is not supported in Clipboard or drag and drop scenarios.");
                }

                stream.Position = position;
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                new BinaryFormatter()
                {
                    Binder = restrictSerialization ? new BitmapBinder() : null
                }.Serialize(stream, data);
#pragma warning restore SYSLIB0011
            }

            internal static object? ReadObjectFromStream<T>(
                MemoryStream stream,
                bool restrictDeserialization,
                Func<TypeName, Type>? resolver,
                bool legacyMode)
            {
                long startPosition = stream.Position;
                SerializationRecord? record;

                SerializationBinder binder = restrictDeserialization
                    ? new BitmapBinder()
                    : new DataObject.Composition.Binder(typeof(T), resolver, legacyMode);

                IReadOnlyDictionary<SerializationRecordId, SerializationRecord> recordMap;
                try
                {
                    record = stream.Decode(out recordMap);
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    // Couldn't parse for some reason, let BinaryFormatter handle the legacy invocation.
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
                if (!legacyMode && !typeof(T).MatchExceptAssemblyVersion(record.TypeName))
                {
#if false // TODO (TanyaSo): - modify TryGetObjectFromJson to take a resolver and rename to HasJsonData???
                    // Return true if the payload contains valid JsonData<T> struct, type matches or not
                    // run IsAssignable in the JSON method
                    if (record.TryGetObjectFromJson(binder.GetType, out object? data))
                    {
                        return data;
                    }
#endif

                    if (!TypeNameIsAssignableToType(record.TypeName, typeof(T), (ITypeResolver)binder))
                    {
                        return null;
                    }
                }

                if (record.TryGetCommonObject(out object? value))
                {
                    return value;
                }

                if (!LocalAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization)
                {
                    // TODO (TanyaSo): localize string
                    throw new NotSupportedException(string.Format(
                        "BinaryFormatter is not supported in Clipboard or drag and drop scenarios." +
                            "  Enable it and use 'TryGetData<T>' API with a 'resolver'" +
                            " function that defines a set of allowed types, to deserialize '{0}'.",
                        typeof(T).FullName));
                }

                // NRBF deserializer is more secure than the BinaryFormatter is:
                // 1. Doesn't allow arrays that have a non-zero base index (can't create these in C# or VB)
                // 2. Only allows IObjectReference types that contain primitives (to avoid observable cycle
                //    dependencies to indeterminate state)
                // But it usually requires a resolver.
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

            private static bool TypeNameIsAssignableToType(TypeName typeName, Type type, ITypeResolver resolver)
            {
                try
                {
                    return resolver.GetType(typeName)?.IsAssignableTo(type) == true;
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    // Clipboard contains a wrong type, we want the API to return false to the caller.
                    // TODO (TanyaSo): this does not special-case the NotSupportedException from the failed SetData call., but we probably want to
                    // bubble it up to the user app.
                    // if (typeName.FullName == typeof(NotSupportedException).FullName)
                    // {
                    //    throw new NotSupportedException("SetData had failed.");
                    // }
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

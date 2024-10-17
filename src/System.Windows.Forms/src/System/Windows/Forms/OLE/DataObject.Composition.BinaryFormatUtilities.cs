// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Formats.Nrbf;
using System.Reflection.Metadata;
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
                bool success = false;

                try
                {
                    success = WinFormsBinaryFormatWriter.TryWriteCommonObject(stream, data);
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    // Being extra cautious here, but the Try method above should never throw in normal circumstances.
                    Debug.Fail($"Unexpected exception writing binary formatted data. {ex.Message}");
                }

#pragma warning disable SYSLIB0011 // Type or member is obsolete
                if (!success)
                {
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
                        throw new NotSupportedException($"Using BinaryFormatter is not supported in WinForms Clipboard or drag and drop scenarios");
                    }

                    stream.Position = position;
                    new BinaryFormatter()
                    {
                        Binder = restrictSerialization ? new BitmapBinder() : null
                    }.Serialize(stream, data);
                }
#pragma warning restore SYSLIB0011
            }

            internal static object? ReadObjectFromStream<T>(
                MemoryStream stream,
                Func<TypeName, Type>? resolver,
                bool restrictDeserialization,
                bool legacyMode)
            {
                long startPosition = stream.Position;
                SerializationRecord? record;

                SerializationBinder binder = restrictDeserialization
                    ? new BitmapBinder()
                    : new DataObject.Composition.Binder(typeof(T), resolver, legacyMode);

                try
                {
                    record = stream.Decode();
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    // Couldn't parse for some reason, let the BinaryFormatter try to handle the legacy invocation.
                    if (legacyMode && LocalAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization)
                    {
                        stream.Position = startPosition;
                        return ReadObjectWithBinaryFormatter<T>(stream, binder);
                    }

                    // For example offset arrays throw from the decoder -
                    // https://learn.microsoft.com/dotnet/api/system.array.createinstance?#system-array-createinstance(system-type-system-int32()-system-int32())
                    throw new NotSupportedException("Clipboard content can't be validated.", ex);
                }

                // For the new TryGet APIs, ensure that the stream contains the requested type,
                // or type that can be assigned to the requested type.
                if (!legacyMode
                    && !record.TypeNameMatches<T>()
                    && (resolver is null || !TypeNameIsAssignableToType(record.TypeName, typeof(T), resolver)))
                {
                    return null;
                }

                if (record.TryGetCommonObject(out object? value))
                {
                    return value;
                }

                if (LocalAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization)
                {
                    stream.Position = startPosition;
                    return ReadObjectWithBinaryFormatter<T>(stream, binder);
                }

                return null;
            }

            // TanyaSo: this does not special-case the NotSupported exception, but we probably want to always deserialize it.
            private static bool TypeNameIsAssignableToType(TypeName typeName, Type type, Func<TypeName, Type> resolver)
            {
                Type? resolvedType = null;
                try
                {
                    resolvedType = resolver(typeName);
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    // If the type can't be resolved, we can't determine if it's assignable.
                    return false;
                }

                return resolvedType?.IsAssignableTo(type) == true;
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
                // cs/dangerous-binary-deserialization
                return new BinaryFormatter()
                {
                    Binder = binder,
                    AssemblyFormat = FormatterAssemblyStyle.Simple
                }.Deserialize(stream); // CodeQL[SM03722] : BinaryFormatter is intended to be used as a fall-back for unsupported types. Users must explicitly opt into this behavior.
#pragma warning restore SYSLIB0050, SYSLIB0011
            }
        }
    }
}

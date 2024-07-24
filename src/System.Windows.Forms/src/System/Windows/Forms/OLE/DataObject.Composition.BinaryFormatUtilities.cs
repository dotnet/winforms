// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Core.BinaryFormat;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms.BinaryFormat;

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
                    success = WinFormsBinaryFormatWriter.TryWriteObject(stream, data);
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    // Being extra cautious here, but the Try method above should never throw in normal circumstances.
                    Debug.Fail($"Unexpected exception writing binary formatted data. {ex.Message}");
                }

#pragma warning disable SYSLIB0011 // Type or member is obsolete
                if (!success)
                {
                    // This check is to help in trimming scenarios with a trim warning on a call to BinaryFormatter.Serialize(), which has a RequiresUnreferencedCode annotation.
                    // If the flag is false, the trimmer will not generate a warning, since BinaryFormatter.Serialize(), will not be called,
                    // If the flag is true, the trimmer will generate a warning for calling a method that has a RequiresUnreferencedCode annotation.
                    if (!EnableUnsafeBinaryFormatterInNativeObjectSerialization)
                    {
                        throw new NotSupportedException(SR.BinaryFormatterNotSupported);
                    }

                    stream.Position = position;
                    new BinaryFormatter()
                    {
                        Binder = restrictSerialization ? new BitmapBinder() : null
                    }.Serialize(stream, data);
                }
#pragma warning restore SYSLIB0011
            }

            internal static object? ReadObjectFromStream<T>(MemoryStream stream, Func<TypeName, Type> resolver, bool restrictDeserialization, bool legacyMode)
            {
                long startPosition = stream.Position;

                BinaryFormattedObject binaryFormattedObject;
                SerializationBinder binder = restrictDeserialization ? new BitmapBinder() : new ComposedBinder(typeof(T), resolver, legacyMode);
                BinaryFormattedObject.Options options = new()
                {
                    Binder = binder
                };

                try
                {
                    binaryFormattedObject = new(stream, options);
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    // offset array case.
                    if (legacyMode) // TanyaSo + switch on
                    {
                        return ReadObjectWithBinaryFormatterDeserializer<T>(stream, binder);
                    }

                    // Invalid stream.
                    return default;
                }

                try
                {
                    if (!binaryFormattedObject.Contains<T>())
                    {
                        return default;
                    }

                    if (binaryFormattedObject.TryGetClipboardObject(out object? value))
                    {
                        return value;
                    }

                    value = binaryFormattedObject.Deserialize();
                    Debug.Assert(value is T, $"{nameof(BinaryFormattedObject)} throws in case of corrupted or unsupported data," +
                        $" it should not deserialize an unexpected type.");
                    return value;
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    // Couldn't parse for some reason, let the BinaryFormatter try to handle it.
                }

                if (resolver is null && !legacyMode && !restrictDeserialization)
                {
                    // TanyaSo - NotSupportedException?
                    return default;
                }

                stream.Position = startPosition;
                return ReadObjectWithBinaryFormatterDeserializer<T>(stream, binder);
            }

            private static object? ReadObjectWithBinaryFormatterDeserializer<T>(MemoryStream stream, SerializationBinder binder)
            {
                // This check is to help in trimming scenarios with a trim warning on a call to BinaryFormatter.Deserialize(), which has a RequiresUnreferencedCode annotation.
                // If the flag is false, the trimmer will not generate a warning, since BinaryFormatter.Deserialize() will not be called,
                // If the flag is true, the trimmer will generate a warning for calling a method that has a RequiresUnreferencedCode annotation.
                if (!EnableUnsafeBinaryFormatterInNativeObjectSerialization)
                {
                    return new NotSupportedException(SR.BinaryFormatterNotSupported);
                }

#pragma warning disable SYSLIB0011 // Type or member is obsolete
#pragma warning disable SYSLIB0050 // Type or member is obsolete
                return new BinaryFormatter()
                {
                    Binder = binder,
                    AssemblyFormat = FormatterAssemblyStyle.Simple
                }.Deserialize(stream);
#pragma warning restore SYSLIB0050
#pragma warning restore SYSLIB0011
            }
        }
    }
}

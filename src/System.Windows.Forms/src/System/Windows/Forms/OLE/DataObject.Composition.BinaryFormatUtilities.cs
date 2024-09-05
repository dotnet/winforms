// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms.BinaryFormat;
using System.Windows.Forms.Nrbf;

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
                    // This check is to help in trimming scenarios with a trim warning on a call to BinaryFormatter.Serialize(),
                    // which has a RequiresUnreferencedCode annotation.
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

            internal static object ReadObjectFromStream(MemoryStream stream, bool restrictDeserialization)
            {
                long startPosition = stream.Position;
                try
                {
                    if (stream.Decode().TryGetObject(out object? value))
                    {
                        return value;
                    }
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    // Couldn't parse for some reason, let the BinaryFormatter try to handle it.
                }

                // This check is to help in trimming scenarios with a trim warning on a call to BinaryFormatter.Deserialize(),
                // which has a RequiresUnreferencedCode annotation.
                // If the flag is false, the trimmer will not generate a warning, since BinaryFormatter.Deserialize() will not be called,
                // If the flag is true, the trimmer will generate a warning for calling a method that has a RequiresUnreferencedCode annotation.
                if (!EnableUnsafeBinaryFormatterInNativeObjectSerialization)
                {
                    throw new NotSupportedException(SR.BinaryFormatterNotSupported);
                }

                stream.Position = startPosition;

#pragma warning disable SYSLIB0011 // Type or member is obsolete
#pragma warning disable SYSLIB0050 // Type or member is obsolete
                // cs/dangerous-binary-deserialization
                return new BinaryFormatter()
                {
                    Binder = restrictDeserialization ? new BitmapBinder() : null,
                    AssemblyFormat = FormatterAssemblyStyle.Simple
                }.Deserialize(stream); // CodeQL[SM03722] : BinaryFormatter is intended to be used as a fallback for unsupported types. Users must explicitly opt into this behavior.
#pragma warning restore SYSLIB0050
#pragma warning restore SYSLIB0011
            }
        }
    }
}

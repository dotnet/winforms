// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Core.BinaryFormat;
using System.Private.Windows.Core.Nrbf;
using System.Private.Windows.Core.Resources;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Private.Windows.Core.OLE;

internal abstract unsafe partial class DesktopDataObject
{
    internal abstract unsafe partial class Composition
    {
        internal abstract class BinaryFormatUtilities
        {
            /// <summary>
            ///  Gets the appropriate binary format writer for application.
            /// </summary>
            public abstract IBinaryFormatWriter GetBinaryFormatWriter();

            /// <summary>
            ///  Reads the object of type T from the stream.
            /// </summary>
            /// <param name="stream">The stream to read from.</param>
            /// <param name="resolver"></param>
            /// <param name="legacyMode">Indicates whether this was called from legacy method family <see cref="IDataObjectDesktop.GetData(string)"/></param>
            /// <returns>The object of type T if successfully read. Otherwise, <see langword="null"/>.</returns>
            internal abstract object? ReadObjectFromStream<T>(
                MemoryStream stream,
                Func<TypeName, Type>? resolver,
                bool legacyMode);

            /// <inheritdoc cref="ReadObjectFromStream{T}(MemoryStream, Func{TypeName, Type}?, bool)"/>
            internal abstract object? ReadRestrictedObjectFromStream<T>(
                MemoryStream stream,
                Func<TypeName, Type>? resolver,
                bool legacyMode);

            internal void WriteObjectToStream(MemoryStream stream, object data, bool restrictSerialization)
            {
                long position = stream.Position;

                try
                {
                    // probably need an interface for this. Maybe an abstract class ?
                    if (GetBinaryFormatWriter().TryWriteCommonObject(stream, data))
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

                if (!LocalAppContextSwitchesCore.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization)
                {
                    throw new NotSupportedException(SR.BinaryFormatter_NotSupported_InClipboardOrDragDrop);
                }

                stream.Position = position;
#pragma warning disable SYSLIB0011 // Type or member is obsolete
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                new BinaryFormatter().Serialize(stream, data);
#pragma warning restore IL2026
#pragma warning restore SYSLIB0011
            }

            [RequiresUnreferencedCode("Calls System.Private.Windows.Core.BinaryFormat.ITypeResolver.GetType(TypeName)")]
            protected static bool TypeNameIsAssignableToType(TypeName typeName, Type type, ITypeResolver resolver)
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

            protected static object? ReadObjectWithBinaryFormatter<T>(MemoryStream stream, SerializationBinder binder)
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
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                return new BinaryFormatter()
                {
                    Binder = binder,
                    AssemblyFormat = FormatterAssemblyStyle.Simple
                }.Deserialize(stream); // CodeQL[SM03722] : BinaryFormatter is intended to be used as a fallback for unsupported types. Users must explicitly opt into this behavior.
#pragma warning restore IL2026
#pragma warning restore CA2300
#pragma warning restore CA2302
#pragma warning restore SYSLIB0050, SYSLIB0011
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Formats.Nrbf;
using System.Private.Windows.Core.BinaryFormat;
using System.Private.Windows.Core.Nrbf;
using System.Private.Windows.Core.OLE;
using System.Reflection.Metadata;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Windows.Forms.BinaryFormat;
using System.Windows.Forms.Nrbf;
using CoreSR = System.Private.Windows.Core.Resources.SR;

namespace System.Windows.Forms;

internal partial class WinFormsDataObject
{
    internal unsafe partial class Composition
    {
        internal class BinaryFormatUtilities : DesktopDataObject.Composition.BinaryFormatUtilities
        {
            public override IBinaryFormatWriter GetBinaryFormatWriter() => new WinFormsBinaryFormatWriter();

            internal override object? ReadObjectFromStream<T>(MemoryStream stream, Func<TypeName, Type>? resolver, bool legacyMode)
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
                    if (legacyMode && LocalAppContextSwitchesCore.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization)
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
                            throw new NotSupportedException(exception.Message);
                        }

                        return null;
                    }
                }

                if (record.TryGetCommonObject(out object? value))
                {
                    return value;
                }

                if (!LocalAppContextSwitchesCore.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization)
                {
                    throw new NotSupportedException(string.Format(
                        CoreSR.BinaryFormatter_NotSupported_InClipboardOrDragDrop_UseTypedAPI,
                        typeof(T).FullName));
                }

                // NRBF deserializer is more secure than the BinaryFormatter is:
                // 1. Doesn't allow arrays that have a non-zero base index (can't create these in C# or VB)
                // 2. Only allows IObjectReference types that contain primitives (to avoid observable cycle
                //    dependencies to indeterminate state)
                // But it usually requires a resolver. Resolver is not available in the legacy mode,
                // so we will fall back to BinaryFormatter in that case.
                if (LocalAppContextSwitchesCore.ClipboardDragDropEnableNrbfSerialization)
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

            internal override object? ReadRestrictedObjectFromStream<T>(MemoryStream stream, Func<TypeName, Type>? resolver, bool legacyMode)
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
        }
    }
}

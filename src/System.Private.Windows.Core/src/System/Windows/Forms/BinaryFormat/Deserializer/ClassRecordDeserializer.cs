// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Windows.Forms.BinaryFormat.Deserializer;

#pragma warning disable SYSLIB0050 // Type or member is obsolete

/// <summary>
///  Base class for deserializing <see cref="ClassRecord"/>s.
/// </summary>
internal abstract class ClassRecordDeserializer : ObjectRecordDeserializer
{
    private protected ClassRecordDeserializer(ClassRecord classRecord, object @object, IDeserializer deserializer)
        : base(classRecord, deserializer)
    {
        Object = @object;
    }

    [RequiresUnreferencedCode("Calls System.Windows.Forms.BinaryFormat.BinaryFormattedObject.TypeResolver.GetType(String, Id)")]
    internal static ObjectRecordDeserializer Create(ClassRecord classRecord, IDeserializer deserializer)
    {
        Type type = deserializer.TypeResolver.GetType(classRecord.Name, classRecord.LibraryId);
        Id id = classRecord.ObjectId;

        ISerializationSurrogate? surrogate = deserializer.GetSurrogate(type);

        if (!type.IsSerializable && surrogate is null)
        {
            // SurrogateSelectors allow populating types that are not marked as serializable.
            throw new SerializationException($"Type '{type}' is not marked as serializable.");
        }

        object @object = RuntimeHelpers.GetUninitializedObject(type);

        if (@object is IObjectReference)
        {
            // Special case for DBNull. Normally we don't allow IObjectReference
            return HandleObjectReference(type, classRecord, deserializer);
        }

        // Invoke any OnDeserializing methods.
        SerializationEvents.GetOnDeserializingForType(type, @object)?.Invoke(deserializer.Options.StreamingContext);

        ObjectRecordDeserializer? recordDeserializer;

        if (surrogate is not null || typeof(ISerializable).IsAssignableFrom(type))
        {
            recordDeserializer = new ClassRecordSerializationInfoDeserializer(classRecord, @object, type, surrogate, deserializer);
        }
        else
        {
            // Directly set fields for non-ISerializable types.
            recordDeserializer = new ClassRecordFieldInfoDeserializer(classRecord, @object, type, deserializer);
        }

        return recordDeserializer;

        static ObjectRecordDeserializer HandleObjectReference(Type type, ClassRecord classRecord, IDeserializer deserializer)
        {
            // Arbitrary IObjectReference handling is complicated and at risk of denial of service attacks.
            // System.UnitySerializationHolder is the only .NET system type that implements this and is
            // used to encapsulate DBNull, so we'll special case it.
            if (type != deserializer.TypeResolver.GetType("System.UnitySerializationHolder", Id.Null)
                || (int)classRecord["UnityType"]! != 0x0002)
            {
                throw new SerializationException($"Type '{type}' is not allowed to implement IObjectReference.");
            }

            return new NoOpRecordDeserializer(classRecord, DBNull.Value, deserializer);
        }
    }

    private class NoOpRecordDeserializer : ObjectRecordDeserializer
    {
        internal NoOpRecordDeserializer(ObjectRecord objectRecord, object @object, IDeserializer deserializer)
            : base(objectRecord, deserializer)
        {
            Object = @object;
        }

        internal override Id Continue()
        {
            Deserializer.CompleteObject(ObjectRecord.ObjectId);
            return Id.Null;
        }
    }
}

#pragma warning restore SYSLIB0050 // Type or member is obsolete


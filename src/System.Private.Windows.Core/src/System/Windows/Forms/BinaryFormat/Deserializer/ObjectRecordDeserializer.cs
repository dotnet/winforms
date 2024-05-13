// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.BinaryFormat;

namespace System.Windows.Forms.BinaryFormat.Deserializer;

#pragma warning disable SYSLIB0050 // Type or member is obsolete

/// <summary>
///  Base class for <see cref="BinaryFormat.ObjectRecord"/> deserialization.
/// </summary>
internal abstract partial class ObjectRecordDeserializer
{
    // Used to indicate that the value is missing from the deserialized objects.
    private protected static object s_missingValueSentinel = new();

    internal SerializationRecord ObjectRecord { get; }

    [AllowNull]
    internal object Object { get; private protected set; }

    private protected IDeserializer Deserializer { get; }

    private protected ObjectRecordDeserializer(SerializationRecord objectRecord, IDeserializer deserializer)
    {
        Deserializer = deserializer;
        ObjectRecord = objectRecord;
    }

    /// <summary>
    ///  Continue parsing.
    /// </summary>
    /// <returns>The id that is necessary to complete parsing or <see cref="Id.Null"/> if complete.</returns>
    internal abstract Id Continue();

    /// <summary>
    ///  Gets the actual object for a member value primitive or record. Returns <see cref="s_missingValueSentinel"/> if
    ///  the object record has not been encountered yet.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private protected (object? value, Id id) UnwrapMemberValue(SerializationRecord? memberValue)
    {
        // NullRecord is expressed via the public API by just returning a null
        if (memberValue is null)
        {
            return (null, Id.Null);
        }

        return memberValue switch
        {
            // String
            PrimitiveTypeRecord<string> primitive => (primitive.Value, Id.Null),
            // Class record
            ClassRecord classRecord => TryGetObject(classRecord.ObjectId),
            // Primitive type record
            PrimitiveTypeRecord<bool> primitive => (primitive.Value, Id.Null),
            PrimitiveTypeRecord<byte> primitive => (primitive.Value, Id.Null),
            PrimitiveTypeRecord<sbyte> primitive => (primitive.Value, Id.Null),
            PrimitiveTypeRecord<char> primitive => (primitive.Value, Id.Null),
            PrimitiveTypeRecord<short> primitive => (primitive.Value, Id.Null),
            PrimitiveTypeRecord<ushort> primitive => (primitive.Value, Id.Null),
            PrimitiveTypeRecord<int> primitive => (primitive.Value, Id.Null),
            PrimitiveTypeRecord<uint> primitive => (primitive.Value, Id.Null),
            PrimitiveTypeRecord<long> primitive => (primitive.Value, Id.Null),
            PrimitiveTypeRecord<ulong> primitive => (primitive.Value, Id.Null),
            PrimitiveTypeRecord<float> primitive => (primitive.Value, Id.Null),
            PrimitiveTypeRecord<double> primitive => (primitive.Value, Id.Null),
            PrimitiveTypeRecord<decimal> primitive => (primitive.Value, Id.Null),
            PrimitiveTypeRecord<TimeSpan> primitive => (primitive.Value, Id.Null),
            PrimitiveTypeRecord<DateTime> primitive => (primitive.Value, Id.Null),
            // At this point should be an inline primitive
            _ => throw new SerializationException($"Unexpected member type '{memberValue.GetType()}'."),
        };

        (object? value, Id id) TryGetObject(Id id)
        {
            if (!Deserializer.DeserializedObjects.TryGetValue(id, out object? value))
            {
                return (s_missingValueSentinel, id);
            }

            ValidateNewMemberObjectValue(value);
            return (value, id);
        }
    }

    /// <summary>
    ///  Called for new non-primitive reference types.
    /// </summary>
    private protected virtual void ValidateNewMemberObjectValue(object value) { }

    /// <summary>
    ///  Returns <see langword="true"/> if the given record's value needs an updater applied.
    /// </summary>
    private protected bool DoesValueNeedUpdated(object value, Id valueRecord) =>
        // Null Id is a primitive of some sort.
        !valueRecord.IsNull
            // IObjectReference is going to have it's object replaced.
            && (value is IObjectReference
                // Value types that aren't "complete" need to be reapplied.
                || (Deserializer.IncompleteObjects.Contains(valueRecord) && value.GetType().IsValueType));

    [RequiresUnreferencedCode("Calls System.Windows.Forms.BinaryFormat.Deserializer.ClassRecordParser.Create(ClassRecord, IDeserializer)")]
    internal static ObjectRecordDeserializer Create(Id id, SerializationRecord record, IDeserializer deserializer) => record switch
    {
        ClassRecord classRecord => ClassRecordDeserializer.Create(classRecord, deserializer),
        ArrayRecord arrayRecord => new ArrayRecordDeserializer(arrayRecord, deserializer),
        _ => throw new SerializationException($"Unexpected record type for {id}.")
    };
}

#pragma warning restore SYSLIB0050 // Type or member is obsolete

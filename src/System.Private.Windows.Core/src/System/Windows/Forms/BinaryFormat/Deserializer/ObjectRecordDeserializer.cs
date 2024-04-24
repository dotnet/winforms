// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Windows.Forms.BinaryFormat.Deserializer;

/// <summary>
///  Base class for <see cref="BinaryFormat.ObjectRecord"/> deserialization.
/// </summary>
internal abstract partial class ObjectRecordDeserializer
{
    // Used to indicate that the value is missing from the deserialized objects.
    private protected static object s_missingValueSentinel = new();

    /// <summary>
    ///  When <see langword="true"/>, <see cref="Continue"/> no longer needs to be called. The object may not
    ///  be complete, but the members have all been parsed.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Objects are considered incomplete if they contain references to value types that need completed or if
    ///   they have not yet finished evaluating all of their member data. They are also considered incomplete if
    ///   they have <see cref="ISerializable"/> or a surrogate that has not yet been called with their
    ///   <see cref="SerializationInfo"/>.
    ///  </para>
    /// </remarks>
    internal bool IsParsingComplete { get; private protected set; }

    internal ObjectRecord ObjectRecord { get; }

    private protected IDeserializer Deserializer { get; }

    private protected ObjectRecordDeserializer(ObjectRecord objectRecord, IDeserializer deserializer)
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
    ///  Gets the actual object for a member value record. Returns <see cref="s_missingValueSentinel"/> if
    ///  the object record has not been encountered yet.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private protected (object? value, Id id) GetMemberValue(object? memberValueRecord) => memberValueRecord switch
    {
        BinaryObjectString binaryString => (binaryString.Value, Id.Null),
        ObjectRecord objectRecord => Deserializer.DeserializedObjects.TryGetValue(objectRecord.ObjectId, out object? value)
            ? (value, objectRecord.ObjectId) : (s_missingValueSentinel, objectRecord.ObjectId),
        MemberReference memberReference => Deserializer.DeserializedObjects.TryGetValue(memberReference.IdRef, out object? value)
            ? (value, memberReference.IdRef) : (s_missingValueSentinel, memberReference.IdRef),
        MemberPrimitiveTyped memberPrimitiveTyped => (memberPrimitiveTyped.Value, Id.Null),
        ObjectNull or null => (null, Id.Null),
        _ => TypeInfo.GetPrimitiveType(memberValueRecord.GetType()) != default
            ? (memberValueRecord, Id.Null)
            : throw new SerializationException($"Unexpected member type '{memberValueRecord.GetType()}'."),
    };

    [RequiresUnreferencedCode("Calls System.Windows.Forms.BinaryFormat.Deserializer.ClassRecordParser.Create(ClassRecord, IDeserializer)")]
    internal static ObjectRecordDeserializer Create(Id id, IRecord record, IDeserializer deserializer) => record switch
    {
        ClassRecord classRecord => ClassRecordDeserializer.Create(classRecord, deserializer),
        ArrayRecord<object?> arrayRecord => new ArrayRecordDeserialzer(arrayRecord, deserializer),
        _ => throw new SerializationException($"Unexpected record type for {id}.")
    };
}

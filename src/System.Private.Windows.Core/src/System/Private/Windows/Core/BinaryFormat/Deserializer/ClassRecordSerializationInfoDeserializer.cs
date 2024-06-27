// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System.Windows.Forms.BinaryFormat.Deserializer;

#pragma warning disable SYSLIB0050 // Type or member is obsolete

/// <summary>
///  Deserializer for <see cref="ClassRecord"/>s that use <see cref="SerializationInfo"/> to initialize class state.
/// </summary>
/// <remarks>
///  <para>
///   This is used either because the class implements <see cref="ISerializable"/> or because a surrogate was used.
///  </para>
/// </remarks>
internal sealed class ClassRecordSerializationInfoDeserializer : ClassRecordDeserializer
{
    private readonly ClassRecord _classRecord;
    private readonly SerializationInfo _serializationInfo;
    private readonly ISerializationSurrogate? _surrogate;
    private int _currentMemberIndex;

    internal ClassRecordSerializationInfoDeserializer(
        ClassRecord classRecord,
        object @object,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type,
        ISerializationSurrogate? surrogate,
        IDeserializer deserializer) : base(classRecord, @object, type, deserializer)
    {
        _classRecord = classRecord;
        _surrogate = surrogate;
        _serializationInfo = new(type, BinaryFormattedObject.DefaultConverter);
    }

    internal override Id Continue()
    {
        // There is no benefit to changing order here so we can keep the same order as the serialized data.
        while (_currentMemberIndex < _classRecord.MemberNames.Count)
        {
            string memberName = _classRecord.MemberNames[_currentMemberIndex];
            (object? memberValue, Id reference) = UnwrapMemberValue(_classRecord.MemberValues[_currentMemberIndex]);

            if (s_missingValueSentinel == memberValue)
            {
                // Record has not been encountered yet, need to pend iteration.
                return reference;
            }

            if (memberValue is not null && DoesValueNeedUpdated(memberValue, reference))
            {
                Deserializer.PendValueUpdater(new SerializationInfoValueUpdater(
                    _classRecord.ObjectId,
                    reference,
                    _serializationInfo,
                    memberName));
            }

            _serializationInfo.AddValue(memberName, memberValue);
            _currentMemberIndex++;
        }

        // Register after all members are parsed, to ensure that completion events are triggered in depth-first order.
        RegisterCompleteEvents();

        // We can't complete these in the same way we do with direct field sets as user code can dereference the
        // reference type members from the SerializationInfo that aren't fully completed (due to cycles). With direct
        // field sets it doesn't matter if the referenced object isn't fully completed. Waiting until the graph is
        // fully parsed to allow cycles the best chance to resolve as much as possible without having to walk the
        // entire graph from this point to make a determination.
        //
        // The same issue applies to "complete" events, which is why we pend them as well.
        //
        // If we were confident that there were no cycles in the graph to this point we could apply directly
        // if there were no pending value types (which should also not happen if there are no cycles).

        PendingSerializationInfo pending = new(_classRecord.ObjectId, _serializationInfo, _surrogate);
        Deserializer.PendSerializationInfo(pending);

        // No more missing member refs.
        return Id.Null;
    }
}

#pragma warning restore SYSLIB0050 // Type or member is obsolete

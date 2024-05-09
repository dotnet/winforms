﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace System.Windows.Forms.BinaryFormat.Deserializer;

#pragma warning disable SYSLIB0050 // Type or member is obsolete

/// <summary>
///  Deserializer for <see cref="ClassRecord"/>s that directly set fields.
/// </summary>
internal sealed class ClassRecordFieldInfoDeserializer : ClassRecordDeserializer
{
    private readonly ClassRecord _classRecord;
    private readonly MemberInfo[] _fieldInfo;
    private int _currentFieldIndex;
    private readonly bool _isValueType;
    private bool _hasFixups;

    internal ClassRecordFieldInfoDeserializer(
        ClassRecord classRecord,
        object @object,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type type,
        IDeserializer deserializer)
        : base(classRecord, @object, type, deserializer)
    {
        _classRecord = classRecord;
        _fieldInfo = FormatterServices.GetSerializableMembers(type);
        _isValueType = type.IsValueType;
    }

    internal override Id Continue()
    {
        // When directly setting fields we need to populate fields with primitive types before we
        // can add the object to the deserialized object list to handle value types. This ensures
        // partialially filled boxed value types in the collection are assigned (and unboxed) in
        // this path (non-ISerializable) with nothing directly pending other than reference types.

        Debug.Assert(_fieldInfo is not null);

        // Note that while fields must have member data, fields are not required for all member data.

        while (_currentFieldIndex < _fieldInfo.Length)
        {
            // FormatterServices *never* returns anything but fields.
            FieldInfo field = (FieldInfo)_fieldInfo[_currentFieldIndex];
            object? rawValue;
            try
            {
                rawValue = _classRecord[field.Name];
            }
            catch (KeyNotFoundException)
            {
                if (Deserializer.Options.AssemblyMatching == FormatterAssemblyStyle.Simple
                    || field.GetCustomAttribute<OptionalFieldAttribute>() is not null)
                {
                    _currentFieldIndex++;
                    continue;
                }

                throw new SerializationException($"Could not find field '{field.Name}' data for type '{field.DeclaringType!.Name}'.");
            }

            (object? memberValue, Id reference) = UnwrapMemberValue(rawValue);
            if (s_missingValueSentinel == memberValue)
            {
                // Record has not been encountered yet, need to pend iteration.
                return reference;
            }

            field.SetValue(Object, memberValue);

            if (memberValue is not null && DoesValueNeedUpdated(memberValue, reference))
            {
                // Need to track a fixup for this field.
                _hasFixups = true;
                Deserializer.PendValueUpdater(new FieldValueUpdater(_classRecord.ObjectId, reference, field));
            }

            _currentFieldIndex++;
        }

        // Register after all members are parsed, to ensure that completion events are triggered in depth-first order.
        RegisterCompleteEvents();

        if (!_hasFixups || !_isValueType)
        {
            // We can be used for completion even with fixups if we're not a value type as our fixups won't need to be
            // copied to propogate them. Note that surrogates cannot replace our created instance for reference types.
            Deserializer.CompleteObject(_classRecord.ObjectId);
        }

        // No more missing member refs.
        return Id.Null;
    }
}

#pragma warning restore SYSLIB0050 // Type or member is obsolete

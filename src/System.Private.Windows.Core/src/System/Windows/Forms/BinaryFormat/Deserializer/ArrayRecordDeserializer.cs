// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization.BinaryFormat;

namespace System.Windows.Forms.BinaryFormat.Deserializer;

internal sealed class ArrayRecordDeserializer : ObjectRecordDeserializer
{
    private readonly ArrayRecord _arrayRecord;
    private readonly Type _elementType;
    private readonly Array _array;
    private readonly Array? _result;
    private int _index;
    private bool _hasFixups;

    [RequiresUnreferencedCode("Calls System.Windows.Forms.BinaryFormat.BinaryFormattedObject.TypeResolver.GetType(TypeName, AssemblyNameInfo)")]
    internal ArrayRecordDeserializer(ArrayRecord arrayRecord, IDeserializer deserializer)
        : base(arrayRecord, deserializer)
    {
        if (arrayRecord.ArrayType is not (ArrayType.Single or ArrayType.Jagged or ArrayType.Rectangular))
        {
            throw new NotSupportedException("Only arrays with zero offsets are supported.");
        }

        // Other array types are handled directly (ArraySinglePrimitive and ArraySingleString).
        Debug.Assert(arrayRecord.RecordType is not (RecordType.ArraySingleString or RecordType.ArraySinglePrimitive));

        _arrayRecord = arrayRecord;
        _elementType = deserializer.TypeResolver.GetType(arrayRecord.ElementTypeName, arrayRecord.ElementTypeLibraryName);
        Type expectedArrayType = arrayRecord.ArrayType switch
        {
            ArrayType.Rectangular => _elementType.MakeArrayType(arrayRecord.Rank),
            _ => _elementType.MakeArrayType()
        };
        // Tricky part: for arrays of classes/structs the following record allocates and array of class records
        // (because the payload reader can not load types, instantiate objects and rehydrate them)
        _array = arrayRecord.ToArray(expectedArrayType, maxLength: Array.MaxLength);

        Type elementType = _array.GetType();
        while (elementType.IsArray)
        {
            elementType = elementType.GetElementType()!;
        }

        // If following is false, then it's an array or primitive types that has already been materialized properly
        if (elementType == typeof(object) || elementType == typeof(ClassRecord))
        {
            int[] lengths = new int[arrayRecord.Rank];
            for (int dimension = 0; dimension < lengths.Length; dimension++)
            {
                lengths[dimension] = _array.GetLength(dimension);
            }

            Object = _result = Array.CreateInstance(_elementType, lengths);
        }
        else
        {
            Object = _array;
        }
    }

    // adsitnik: it may compile, but most likely won't work without any changes
    internal override Id Continue()
    {
        if (_result is null)
        {
            return Id.Null;
        }

        while (_index < _arrayRecord.Length)
        {
            // TODO: adsitnik: handle multi-dimensional arrays
            object? memberValue = _array.GetValue(_index);
            _result.SetValue(memberValue, _index);
            Id reference = memberValue is ClassRecord classRecord ? classRecord.ObjectId : Id.Null;

            if (s_missingValueSentinel == memberValue)
            {
                // Record has not been encountered yet, need to pend iteration.
                return reference;
            }

            if (memberValue is not null && DoesValueNeedUpdated(memberValue, reference))
            {
                // Need to track a fixup for this index.
                _hasFixups = true;
                Deserializer.PendValueUpdater(new ArrayUpdater(_arrayRecord.ObjectId, reference, _index));
            }

            if (memberValue is null && !_elementType.IsValueType)
            {
                // No point in setting a null. If we're a value type, let it flow to throw when setting.
                _index++;
                continue;
            }

            if (_elementType.IsValueType)
            {
                _result.SetArrayValueByFlattenedIndex(memberValue, _index);
            }
            else
            {
                Span<object?> flatSpan = _result.GetArrayData<object?>();
                flatSpan[_index] = memberValue;
            }

            _index++;
        }

        // No more missing member refs.

        if (!_hasFixups)
        {
            Deserializer.CompleteObject(_arrayRecord.ObjectId);
        }

        return Id.Null;
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System.Windows.Forms.BinaryFormat.Deserializer;

internal sealed class ArrayRecordDeserialzer : ObjectRecordDeserializer
{
    private readonly ArrayRecord<object?> _arrayRecord;
    private readonly BinaryArrayType _arrayType;
    private readonly Type _elementType;
    private readonly Array _array;
    private int _index;
    private bool _hasFixups;

    [RequiresUnreferencedCode("Calls System.Windows.Forms.BinaryFormat.BinaryFormattedObject.TypeResolver.GetType(String, Id)")]
    internal ArrayRecordDeserialzer(ArrayRecord<object?> arrayRecord, IDeserializer deserializer)
        : base(arrayRecord, deserializer)
    {
        _arrayRecord = arrayRecord;

        if (arrayRecord is not IBinaryArray binaryArray)
        {
            // Other array types are handled directly (ArraySinglePrimitive and ArraySingleString).
            Debug.Assert(arrayRecord is ArraySingleObject);
            _elementType = typeof(object);
            _arrayType = BinaryArrayType.Single;
            _array = Array.CreateInstance(_elementType, arrayRecord.Length);
        }
        else
        {
            (BinaryType binaryType, object? info) = binaryArray.TypeInfo[0];

            _elementType = binaryType switch
            {
                BinaryType.SystemClass => info is Type type
                    ? type
                    : deserializer.TypeResolver.GetType((string)info!, Id.Null),
                BinaryType.Class => info is Type type
                    ? type
                    : deserializer.TypeResolver.GetType(
                        ((ClassTypeInfo)info!).TypeName,
                        ((ClassTypeInfo)info!).LibraryId),
                BinaryType.String => typeof(string),
                // Jagged array
                BinaryType.PrimitiveArray => ((PrimitiveType)info!).GetArrayPrimitiveTypeType(),
                _ => throw new SerializationException($"Unexpected BinaryArray type: {_elementType}")
            };

            _arrayType = binaryArray.ArrayType;
            if (_arrayType is not (BinaryArrayType.Single or BinaryArrayType.Jagged or BinaryArrayType.Rectangular))
            {
                throw new NotSupportedException("Only arrays with zero offsets are supported.");
            }

            _array = _arrayType is BinaryArrayType.Rectangular
                ? Array.CreateInstance(_elementType, binaryArray.Lengths.ToArray())
                : Array.CreateInstance(_elementType, arrayRecord.Length);
        }

        Object = _array;
    }

    internal override Id Continue()
    {
        while (_index < _arrayRecord.Length)
        {
            (object? memberValue, Id reference) = GetMemberValue(_arrayRecord.ArrayObjects[_index]);

            if (s_missingValueSentinel == memberValue)
            {
                // Record has not been encountered yet, need to pend iteration.
                return reference;
            }

            if (!reference.IsNull
                && Deserializer.IncompleteObjects.Contains(reference)
                && memberValue!.GetType().IsValueType)
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

            if (_arrayType is not BinaryArrayType.Rectangular || _elementType.IsValueType)
            {
                _array.SetArrayValueByFlattenedIndex(memberValue, _index);
            }
            else
            {
                Span<object?> flatSpan = _array.GetArrayData<object?>();
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

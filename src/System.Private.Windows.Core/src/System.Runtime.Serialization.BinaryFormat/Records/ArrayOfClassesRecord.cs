// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.Serialization.BinaryFormat;

internal sealed class ArrayOfClassesRecord : ArrayRecord<ClassRecord>
{
    internal ArrayOfClassesRecord(ArrayInfo arrayInfo, MemberTypeInfo memberTypeInfo, RecordMap recordMap)
        : base(arrayInfo)
    {
        MemberTypeInfo = memberTypeInfo;
        RecordMap = recordMap;
        Records = [];
    }

    public override RecordType RecordType => RecordType.BinaryArray;

    internal List<SerializationRecord> Records { get; }

    private MemberTypeInfo MemberTypeInfo { get; }

    private RecordMap RecordMap { get; }

    protected override ClassRecord?[] ToArrayOfT(bool allowNulls)
    {
        ClassRecord?[] result = new ClassRecord?[Length];

        int resultIndex = 0;
        foreach (SerializationRecord record in Records)
        {
            SerializationRecord actual = record is MemberReferenceRecord referenceRecord
                ? referenceRecord.GetReferencedRecord()
                : record;

            if (actual is ClassRecord classRecord)
            {
                result[resultIndex++] = classRecord;
            }
            else
            {
                if (!allowNulls)
                {
                    ThrowHelper.ThrowArrayContainedNull();
                }

                int nullCount = ((NullsRecord)actual).NullCount;
                do
                {
                    result[resultIndex++] = null;
                    nullCount--;
                }
                while (nullCount > 0);
            }
        }

        return result;
    }

    private protected override void AddValue(object value) => Records.Add((SerializationRecord)value);

    internal override (AllowedRecordTypes allowed, PrimitiveType primitiveType) GetAllowedRecordType()
    {
        (AllowedRecordTypes allowed, PrimitiveType primitiveType) = MemberTypeInfo.GetNextAllowedRecordType(0);

        if (allowed != AllowedRecordTypes.None)
        {
            // It's an array, it can also contain multiple nulls
            return (allowed | AllowedRecordTypes.Nulls, primitiveType);
        }

        return (allowed, primitiveType);
    }

    internal override bool IsElementType(Type typeElement)
        => MemberTypeInfo.IsElementType(typeElement, RecordMap);
}

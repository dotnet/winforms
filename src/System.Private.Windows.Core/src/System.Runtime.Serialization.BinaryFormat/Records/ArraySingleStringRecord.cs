﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.Serialization.BinaryFormat;

/// <summary>
///  Single dimensional array of strings.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/3d98fd60-d2b4-448a-ac0b-3cd8dea41f9d">
///    [MS-NRBF] 2.4.3.4
///   </see>
///  </para>
/// </remarks>
internal sealed class ArraySingleStringRecord : ArrayRecord<string?>
{
    private ArraySingleStringRecord(ArrayInfo arrayInfo) : base(arrayInfo) => Records = [];

    public override RecordType RecordType => RecordType.ArraySingleString;

    private List<SerializationRecord> Records { get; }

    public override bool IsTypeNameMatching(Type type) => type == typeof(string[]);

    internal override bool IsElementType(Type typeElement) => typeElement == typeof(string);

    internal static ArraySingleStringRecord Parse(BinaryReader reader)
        => new(ArrayInfo.Parse(reader));

    internal override (AllowedRecordTypes allowed, PrimitiveType primitiveType) GetAllowedRecordType()
    {
        // An array of string can consist of string(s), null(s) and reference(s) to string(s).
        const AllowedRecordTypes allowedTypes = AllowedRecordTypes.BinaryObjectString
            | AllowedRecordTypes.Nulls | AllowedRecordTypes.MemberReference;

        return (allowedTypes, default);
    }

    private protected override void AddValue(object value) => Records.Add((SerializationRecord)value);

    protected override string?[] ToArrayOfT(bool allowNulls)
    {
        string?[] values = new string?[Length];

        for (int recordIndex = 0, valueIndex = 0; recordIndex < Records.Count; recordIndex++)
        {
            SerializationRecord record = Records[recordIndex];

            if (record is MemberReferenceRecord memberReference)
            {
                record = memberReference.GetReferencedRecord();

                if (record is not BinaryObjectStringRecord)
                {
                    // TODO: consider throwing this exception as soon as we read the referenced record.
                    // It would require registering reference validation checks.
                    throw new SerializationException("The string array contained a reference to non-string.");
                }
            }

            if (record is BinaryObjectStringRecord stringRecord)
            {
                values[valueIndex++] = stringRecord.Value;
                continue;
            }

            if (!allowNulls)
            {
                throw new SerializationException("The array contained null(s).");
            }

            int nullCount = ((NullsRecord)record).NullCount;
            do
            {
                values[valueIndex++] = null;
                nullCount--;
            }
            while (nullCount > 0);
        }

        return values;
    }
}

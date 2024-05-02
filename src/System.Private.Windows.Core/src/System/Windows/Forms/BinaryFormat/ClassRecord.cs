// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Base class for class records.
/// </summary>
/// <remarks>
///  <para>
///   Includes the values for the class (which trail the record)
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/c9bc3af3-5a0c-4b29-b517-1b493b51f7bb">
///    [MS-NRBF] 2.3
///   </see>.
///  </para>
/// </remarks>
internal abstract class ClassRecord : ObjectRecord
{
    internal ClassInfo ClassInfo { get; }
    public IReadOnlyList<object?> MemberValues { get; }
    public MemberTypeInfo MemberTypeInfo { get; }

    public string Name => ClassInfo.Name;
    public override Id ObjectId => ClassInfo.ObjectId;
    public virtual Id LibraryId => Id.Null;

    public IReadOnlyList<string> MemberNames => ClassInfo.MemberNames;

    public object? this[string memberName]
    {
        get
        {
            for (int i = 0; i < ClassInfo.MemberNames.Count; i++)
            {
                string current = ClassInfo.MemberNames[i];
                if (current == memberName)
                {
                    return MemberValues[i];
                }
            }

            throw new KeyNotFoundException();
        }
    }

    private protected ClassRecord(ClassInfo classInfo, MemberTypeInfo memberTypeInfo, IReadOnlyList<object?> memberValues)
    {
        ClassInfo = classInfo;
        MemberValues = memberValues;
        MemberTypeInfo = memberTypeInfo;
    }

    /// <summary>
    ///  Reads object member values using <paramref name="memberTypeInfo"/>.
    /// </summary>
    private protected static IReadOnlyList<object?> ReadObjectMemberValues(
        BinaryFormattedObject.IParseState state,
        MemberTypeInfo memberTypeInfo)
    {
        int count = memberTypeInfo.Count;
        if (count == 0)
        {
            return [];
        }

        ArrayBuilder<object?> memberValues = new(count);
        foreach ((BinaryType type, object? info) in memberTypeInfo)
        {
            object value = ReadValue(state, type, info);
            if (value is not ObjectNull nullValue)
            {
                memberValues.Add(value);
                continue;
            }

            if (nullValue.NullCount != 1)
            {
                throw new SerializationException("Member values can only have one null assigned.");
            }

            memberValues.Add(null);
        }

        return memberValues.ToArray();
    }

    /// <summary>
    ///  Writes <paramref name="memberValues"/> as specified by the <paramref name="memberTypeInfo"/>
    /// </summary>
    private protected static void WriteValuesFromMemberTypeInfo(
        BinaryWriter writer,
        MemberTypeInfo memberTypeInfo,
        IReadOnlyList<object?> memberValues)
    {
        for (int i = 0; i < memberTypeInfo.Count; i++)
        {
            (BinaryType type, object? info) = memberTypeInfo[i];
            object? memberValue = memberValues[i];

            switch (type)
            {
                case BinaryType.Primitive:
                    WritePrimitiveType(writer, (PrimitiveType)info!, memberValue!);
                    break;
                case BinaryType.String:
                case BinaryType.Object:
                case BinaryType.StringArray:
                case BinaryType.PrimitiveArray:
                case BinaryType.Class:
                case BinaryType.SystemClass:
                case BinaryType.ObjectArray:
                    if (memberValue is IRecord record)
                    {
                        record.Write(writer);
                    }
                    else if (memberValue is null)
                    {
                        NullRecord.Write(writer, 1);
                    }
                    else
                    {
                        throw new InvalidOperationException("Not an IRecord or null.");
                    }

                    break;
                default:
                    throw new ArgumentException("Invalid binary type.", nameof(memberTypeInfo));
            }
        }
    }
}

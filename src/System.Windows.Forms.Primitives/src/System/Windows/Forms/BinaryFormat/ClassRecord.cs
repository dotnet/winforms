// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
internal abstract class ClassRecord : Record
{
    internal ClassInfo ClassInfo { get; }
    public IReadOnlyList<object> MemberValues { get; }

    public string Name => ClassInfo.Name;
    public virtual Id ObjectId => ClassInfo.ObjectId;
    public IReadOnlyList<string> MemberNames => ClassInfo.MemberNames;

    public object this[string memberName]
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

    private protected ClassRecord(ClassInfo classInfo, IReadOnlyList<object> memberValues)
    {
        ClassInfo = classInfo;
        MemberValues = memberValues;
    }

    private protected static List<object> ReadDataFromClassInfo(BinaryReader reader, RecordMap recordMap, ClassInfo info)
    {
        // Not sure what gets us into this state yet.
        return ReadRecords(reader, recordMap, info.MemberNames.Count);
    }

    private protected static IReadOnlyList<object> ReadDataFromMemberTypeInfo(
        BinaryReader reader,
        RecordMap recordMap,
        MemberTypeInfo memberTypeInfo)
    {
        List<object> memberValues = new();
        memberValues.EnsureCapacity(memberTypeInfo.Count);
        foreach ((BinaryType type, object? info) in memberTypeInfo)
        {
            switch (type)
            {
                case BinaryType.Primitive:
                    memberValues.Add(ReadPrimitiveType(reader, (PrimitiveType)info!));
                    break;
                case BinaryType.String:
                    memberValues.Add(reader.ReadLengthPrefixedString());
                    break;
                case BinaryType.Object:
                case BinaryType.StringArray:
                case BinaryType.PrimitiveArray:
                case BinaryType.Class:
                case BinaryType.SystemClass:
                case BinaryType.ObjectArray:
                    memberValues.Add(ReadBinaryFormatRecord(reader, recordMap));
                    break;
                default:
                    throw new SerializationException();
            }
        }

        return memberValues;
    }

    private protected static void WriteDataFromMemberTypeInfo(BinaryWriter writer, MemberTypeInfo memberTypeInfo, IReadOnlyList<object> memberValues)
    {
        for (int i = 0; i < memberTypeInfo.Count; i++)
        {
            (BinaryType type, object? info) = memberTypeInfo[i];
            switch (type)
            {
                case BinaryType.Primitive:
                    WritePrimitiveType(writer, (PrimitiveType)info!, memberValues[i]);
                    break;
                case BinaryType.String:
                    writer.WriteLengthPrefixedString((string)memberValues[i]);
                    break;
                case BinaryType.Object:
                case BinaryType.StringArray:
                case BinaryType.PrimitiveArray:
                case BinaryType.Class:
                case BinaryType.SystemClass:
                case BinaryType.ObjectArray:
                    ((IRecord)memberValues[i]).Write(writer);
                    break;
                default:
                    throw new SerializationException();
            }
        }
    }
}

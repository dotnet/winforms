// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Core.BinaryFormat.Serializer;

/// <summary>
///  Base class for class records.
/// </summary>
/// <remarks>
///  <para>
///   Includes the values for the class (which trail the record)
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/c9bc3af3-5a0c-4b29-b517-1b493b51f7bb">
///    [MS-NRBF] 2.3.
///   </see>
///  </para>
/// </remarks>
internal abstract class ClassRecord : ObjectRecord
{
    internal ClassInfo ClassInfo { get; }
    public IReadOnlyList<object?> MemberValues { get; }
    internal IReadOnlyList<MemberTypeInfo> MemberTypeInfo { get; }

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

    private protected ClassRecord(
        ClassInfo classInfo,
        IReadOnlyList<MemberTypeInfo> memberTypeInfo,
        IReadOnlyList<object?> memberValues)
    {
        ClassInfo = classInfo;
        MemberValues = memberValues;
        MemberTypeInfo = memberTypeInfo;
    }

    /// <summary>
    ///  Writes <paramref name="memberValues"/> as specified by the <paramref name="memberTypeInfo"/>
    /// </summary>
    private protected static void WriteValuesFromMemberTypeInfo(
        BinaryWriter writer,
        IReadOnlyList<MemberTypeInfo> memberTypeInfo,
        IReadOnlyList<object?> memberValues)
    {
        for (int i = 0; i < memberTypeInfo.Count; i++)
        {
            object? memberValue = memberValues[i];

            switch (memberTypeInfo[i].Type)
            {
                case BinaryType.Primitive:
                    WritePrimitiveType(writer, (PrimitiveType)memberTypeInfo[i].Info!, memberValue!);
                    break;
                case BinaryType.String:
                case BinaryType.Object:
                case BinaryType.StringArray:
                case BinaryType.PrimitiveArray:
                case BinaryType.Class:
                case BinaryType.SystemClass:
                case BinaryType.ObjectArray:
                    if (memberValue is IWritableRecord record)
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

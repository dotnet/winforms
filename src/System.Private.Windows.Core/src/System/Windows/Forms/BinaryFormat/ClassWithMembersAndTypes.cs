// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Class information with type info and the source library.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/847b0b6a-86af-4203-8ed0-f84345f845b9">
///    [MS-NRBF] 2.3.2.1
///   </see>
///  </para>
/// </remarks>
internal sealed class ClassWithMembersAndTypes :
    ClassRecord,
    IRecord<ClassWithMembersAndTypes>,
    IBinaryFormatParseable<ClassWithMembersAndTypes>
{
    public override Id LibraryId { get; }

    public ClassWithMembersAndTypes(
        ClassInfo classInfo,
        Id libraryId,
        MemberTypeInfo memberTypeInfo,
        IReadOnlyList<object?> memberValues)
        : base(classInfo, memberTypeInfo, memberValues)
    {
        LibraryId = libraryId;
    }

    public ClassWithMembersAndTypes(
        ClassInfo classInfo,
        Id libraryId,
        MemberTypeInfo memberTypeInfo,
        params object[] memberValues)
        : this(classInfo, libraryId, memberTypeInfo, (IReadOnlyList<object>)memberValues)
    {
    }

    public static RecordType RecordType => RecordType.ClassWithMembersAndTypes;

    static ClassWithMembersAndTypes IBinaryFormatParseable<ClassWithMembersAndTypes>.Parse(
        BinaryFormattedObject.ParseState state)
    {
        ClassInfo classInfo = ClassInfo.Parse(state.Reader, out Count memberCount);
        MemberTypeInfo memberTypeInfo = MemberTypeInfo.Parse(state.Reader, memberCount);

        ClassWithMembersAndTypes record = new(
            classInfo,
            state.Reader.ReadInt32(),
            memberTypeInfo,
            ReadValuesFromMemberTypeInfo(state, memberTypeInfo));

        // Index this record by the id of the embedded ClassInfo's object id.
        state.RecordMap[record.ClassInfo.ObjectId] = record;
        return record;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)RecordType);
        ClassInfo.Write(writer);
        MemberTypeInfo.Write(writer);
        writer.Write(LibraryId);
        WriteValuesFromMemberTypeInfo(writer, MemberTypeInfo, MemberValues);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Core.BinaryFormat.Serializer;

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
    IRecord<ClassWithMembersAndTypes>
{
    public override Id LibraryId { get; }

    public ClassWithMembersAndTypes(
        ClassInfo classInfo,
        Id libraryId,
        IReadOnlyList<MemberTypeInfo> memberTypeInfo,
        IReadOnlyList<object?> memberValues)
        : base(classInfo, memberTypeInfo, memberValues)
    {
        LibraryId = libraryId;
    }

    public ClassWithMembersAndTypes(
        ClassInfo classInfo,
        Id libraryId,
        IReadOnlyList<MemberTypeInfo> memberTypeInfo,
        params object[] memberValues)
        : this(classInfo, libraryId, memberTypeInfo, (IReadOnlyList<object>)memberValues)
    {
    }

    public static RecordType RecordType => RecordType.ClassWithMembersAndTypes;

    private protected override void Write(BinaryWriter writer)
    {
        writer.Write((byte)RecordType);
        ClassInfo.Write(writer);
        writer.Write(MemberTypeInfo);
        writer.Write(LibraryId);
        WriteValuesFromMemberTypeInfo(writer, MemberTypeInfo, MemberValues);
    }
}

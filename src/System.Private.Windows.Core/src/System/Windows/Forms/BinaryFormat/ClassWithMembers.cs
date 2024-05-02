// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Class information with the source library.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/ebbdad88-91fe-48ae-a985-661f9cc7e0de">
///    [MS-NRBF] 2.3.2.2
///   </see>
///  </para>
/// </remarks>
internal sealed class ClassWithMembers : ClassRecord, IRecord<ClassWithMembers>, IBinaryFormatParseable<ClassWithMembers>
{
    public override Id LibraryId { get; }

    private ClassWithMembers(ClassInfo classInfo, Id libraryId, MemberTypeInfo memberTypeInfo, IReadOnlyList<object?> memberValues)
        : base(classInfo, memberTypeInfo, memberValues)
    {
        LibraryId = libraryId;
    }

    public static RecordType RecordType => RecordType.ClassWithMembers;

    static ClassWithMembers IBinaryFormatParseable<ClassWithMembers>.Parse(
        BinaryFormattedObject.IParseState state)
    {
        ClassInfo classInfo = ClassInfo.Parse(state.Reader, out _);
        Id libraryId = state.Reader.ReadInt32();
        MemberTypeInfo memberTypeInfo = MemberTypeInfo.CreateFromClassInfoAndLibrary(state, classInfo, libraryId);
        ClassWithMembers record = new(
            classInfo,
            libraryId,
            memberTypeInfo,
            ReadObjectMemberValues(state, memberTypeInfo));

        // Index this record by the id of the embedded ClassInfo's object id.
        state.RecordMap[classInfo.ObjectId] = record;
        return record;
    }

    public override void Write(BinaryWriter writer)
    {
        // Really shouldn't be writing this record type. It isn't as safe as the typed variant
        // and saves very little space.
        throw new NotSupportedException();
    }
}

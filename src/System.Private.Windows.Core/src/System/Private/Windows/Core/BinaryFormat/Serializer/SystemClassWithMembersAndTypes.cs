// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Core.BinaryFormat.Serializer;

/// <summary>
///  System class information with type info.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/ecb47445-831f-4ef5-9c9b-afd4d06e3657">
///    [MS-NRBF] 2.3.2.3
///   </see>
///  </para>
/// </remarks>
internal sealed class SystemClassWithMembersAndTypes :
    ClassRecord,
    IRecord<SystemClassWithMembersAndTypes>
{
    public SystemClassWithMembersAndTypes(
        ClassInfo classInfo,
        IReadOnlyList<MemberTypeInfo> memberTypeInfo,
        IReadOnlyList<object?> memberValues)
        : base(classInfo, memberTypeInfo, memberValues)
    {
    }

    public SystemClassWithMembersAndTypes(
        ClassInfo classInfo,
        IReadOnlyList<MemberTypeInfo> memberTypeInfo,
        params object?[] memberValues)
        : this(classInfo, memberTypeInfo, (IReadOnlyList<object?>)memberValues)
    {
    }

    public static RecordType RecordType => RecordType.SystemClassWithMembersAndTypes;

    private protected override void Write(BinaryWriter writer)
    {
        writer.Write((byte)RecordType);
        ClassInfo.Write(writer);
        writer.Write(MemberTypeInfo);
        WriteValuesFromMemberTypeInfo(writer, MemberTypeInfo, MemberValues);
    }
}

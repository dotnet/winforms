﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Class information that references another class record's metadata.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/2d168388-37f4-408a-b5e0-e48dbce73e26">
///    [MS-NRBF] 2.3.2.5
///   </see>
///  </para>
/// </remarks>
internal sealed class ClassWithId : ClassRecord, IRecord<ClassWithId>, IBinaryFormatParseable<ClassWithId>
{
    private readonly ClassRecord _metadataClass;

    public override Id ObjectId { get; }
    public override Id LibraryId { get; }

    /// <summary>
    ///  The ObjectId of a prior <see cref="SystemClassWithMembers"/>, <see cref="SystemClassWithMembersAndTypes"/>,
    ///  <see cref="ClassWithMembers"/>, or <see cref="ClassWithMembersAndTypes"/>.
    /// </summary>
    public Id MetadataId { get; }

    public ClassWithId(Id id, ClassRecord metadataClass, IReadOnlyList<object?> memberValues)
        : base(metadataClass.ClassInfo, metadataClass.MemberTypeInfo, memberValues)
    {
        ObjectId = id;
        MetadataId = metadataClass.ObjectId;
        LibraryId = metadataClass.LibraryId;
        _metadataClass = metadataClass;
    }

    public ClassWithId(Id id, ClassRecord metadataClass, params object?[] memberValues)
        : this(id, metadataClass, (IReadOnlyList<object?>)memberValues)
    {
    }

    public static RecordType RecordType => RecordType.ClassWithId;

    static ClassWithId IBinaryFormatParseable<ClassWithId>.Parse(
        BinaryFormattedObject.ParseState state)
    {
        Id objectId = state.Reader.ReadInt32();
        Id metadataId = state.Reader.ReadInt32();

        if (state.RecordMap[metadataId] is not ClassRecord referencedRecord)
        {
            throw new SerializationException();
        }

        ClassWithId record = new(
            objectId,
            referencedRecord,
            ReadValuesFromMemberTypeInfo(state, referencedRecord.MemberTypeInfo));

        state.RecordMap[record.ObjectId] = record;
        return record;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)RecordType);
        writer.Write(ObjectId);
        writer.Write(MetadataId);

        switch (_metadataClass)
        {
            case ClassWithMembersAndTypes classWithMembersAndTypes:
                WriteValuesFromMemberTypeInfo(writer, classWithMembersAndTypes.MemberTypeInfo, MemberValues);
                break;
            case SystemClassWithMembersAndTypes systemClassWithMembersAndTypes:
                WriteValuesFromMemberTypeInfo(writer, systemClassWithMembersAndTypes.MemberTypeInfo, MemberValues);
                break;
            case ClassWithMembers or SystemClassWithMembers:
                WriteRecords(writer, MemberValues);
                break;
            default:
                throw new SerializationException();
        }
    }

    // The following implicit conversion is to facilitate lookup of related records
    // using the correct identifier.

    public static implicit operator Id(ClassWithId value) => value.MetadataId;
}

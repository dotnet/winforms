// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;
using System.Private.Windows.Core.BinaryFormat;
using FormatTests.Common;

namespace FormatTests.FormattedObject;

public class NullRecordTests : NullRecordTests<FormattedObjectSerializer>
{
    [Fact]
    public void NullObjectArrayRecords_MultipleSerializedAs()
    {
        object?[] items = new object?[10];
        using ManualParser parser = new(items);

        IRecord record = parser.ReadRecord();
        record.Should().BeOfType<SerializationHeader>();

        RecordType recordType = (RecordType)parser.Reader.ReadByte();
        recordType.Should().Be(RecordType.ArraySingleObject);
        Id id = ArrayInfo.Parse(parser.Reader, out Count length);
        id.Should().Be(1);
        length.Should().Be(items.Length);

        record = parser.ReadRecord();
        record.Should().BeOfType<NullRecord.ObjectNullMultiple256>()
            .Subject.NullCount.Should().Be(items.Length);
    }

    [Fact]
    public void TupleWithNull_SerializedAs()
    {
        Tuple<object?> tuple = new(null);
        using ManualParser parser = new(tuple);

        IRecord record = parser.ReadRecord();
        record.Should().BeOfType<SerializationHeader>();

        RecordType recordType = (RecordType)parser.Reader.ReadByte();
        recordType.Should().Be(RecordType.SystemClassWithMembersAndTypes);
        ClassInfo classInfo = ClassInfo.Parse(parser.Reader, out Count memberCount);
        var memberTypeInfo = MemberTypeInfo.Parse(parser.Reader, memberCount);

        recordType = (RecordType)parser.Reader.ReadByte();
        recordType.Should().Be(RecordType.ObjectNull);
    }

    [Fact]
    public void TupleWithTwoNulls_SerializedAs()
    {
        Tuple<object?, object?> tuple = new(null, null);
        using ManualParser parser = new(tuple);

        IRecord record = parser.ReadRecord();
        record.Should().BeOfType<SerializationHeader>();

        RecordType recordType = (RecordType)parser.Reader.ReadByte();
        recordType.Should().Be(RecordType.SystemClassWithMembersAndTypes);
        ClassInfo classInfo = ClassInfo.Parse(parser.Reader, out Count memberCount);
        var memberTypeInfo = MemberTypeInfo.Parse(parser.Reader, memberCount);

        recordType = (RecordType)parser.Reader.ReadByte();
        recordType.Should().Be(RecordType.ObjectNull);
    }

    public override void NullRecord_ZeroByte()
    {
        // BinaryFormattedObject blocks before it creates any objects
        Action action = base.NullRecord_ZeroByte;
        action.Should().Throw<SerializationException>();
    }

    public override void NullRecord_ZeroInt()
    {
        // BinaryFormattedObject blocks before it creates any objects
        Action action = base.NullRecord_ZeroInt;
        action.Should().Throw<SerializationException>();
    }

    public override void NullObjectArray_NotEnoughNulls()
    {
        Action action = base.NullObjectArray_NotEnoughNulls;
        action.Should().Throw<SerializationException>();
    }

    public override void Tuple_WithMultipleNullCount()
    {
        // While it is ok per spec to write out multiple null count records for members, this is never done by the
        // BinaryFormatter and it greatly complicates the deserialization logic. BinaryFormattedObject rejects this.
        // (But does allow multiple null records for arrays, where they do normally occur.)
        Action action = base.Tuple_WithMultipleNullCount;
        action.Should().Throw<SerializationException>();
    }

    public override void Tuple_WithZeroNullCount()
    {
        Action action = base.Tuple_WithZeroNullCount;
        action.Should().Throw<SerializationException>();
    }

    public override void NullRecord_WrongLength_WithTuple(int nullCount)
    {
        Action action = () => base.NullRecord_WrongLength_WithTuple(nullCount);
        action.Should().Throw<SerializationException>();
    }

    [Fact]
    public void ObjectNullMultiple256_ThrowsOverflowOnWrite()
    {
        // We read a byte on the way in so there is nothing to check.

        NullRecord.ObjectNullMultiple256 objectNull = new(1000);

        using BinaryWriter writer = new(new MemoryStream());
        Action action = () => objectNull.Write(writer);
        action.Should().Throw<OverflowException>();
    }

    [Fact]
    public void ObjectNullMultiple256_WritesCorrectly()
    {
        NullRecord.ObjectNullMultiple256 objectNull = new(0xCA);

        byte[] buffer = new byte[2];
        using BinaryWriter writer = new(new MemoryStream(buffer));
        objectNull.Write(writer);
        buffer.Should().BeEquivalentTo(new byte[] { (byte)NullRecord.ObjectNullMultiple256.RecordType, 0xCA });
    }
}

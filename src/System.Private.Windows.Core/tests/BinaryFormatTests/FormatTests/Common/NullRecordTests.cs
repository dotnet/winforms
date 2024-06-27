// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;
using System.Private.Windows.Core.BinaryFormat;

namespace FormatTests.Common;

public abstract class NullRecordTests<T> : SerializationTest<T> where T : ISerializer
{
    [Theory]
    // One over, with ObjectNullMultiple256
    [InlineData(11)]
    // ObjectNullMultiple
    [InlineData(256)]
    public void NullObjectArray_CorruptedNullCount(int nullCount)
    {
        MemoryStream stream = new();
        using (BinaryFormatWriterScope scope = new(stream))
        {
            scope.Writer.Write((byte)RecordType.ArraySingleObject);

            // Id
            scope.Writer.Write(1);

            // Length
            scope.Writer.Write(10);

            NullRecord.Write(scope, nullCount);
        }

        stream.Position = 0;
        Action action = () => Deserialize(stream);
        action.Should().Throw<SerializationException>();
    }

    [Fact]
    public virtual void NullObjectArray_NotEnoughNulls()
    {
        MemoryStream stream = new();
        using (BinaryFormatWriterScope scope = new(stream))
        {
            scope.Writer.Write((byte)RecordType.ArraySingleObject);

            // Id
            scope.Writer.Write(1);

            // Length
            scope.Writer.Write(10);

            NullRecord.Write(scope, 9);
        }

        stream.Position = 0;
        object deserialized = Deserialize(stream);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-2)]
    [InlineData(int.MinValue)]
    public void NullRecord_Negative(int count)
    {
        MemoryStream stream = new();
        using (BinaryFormatWriterScope scope = new(stream))
        {
            scope.Writer.Write((byte)RecordType.ArraySingleObject);

            // Id
            scope.Writer.Write(1);

            // Length
            scope.Writer.Write(1);

            scope.Writer.Write((byte)RecordType.ObjectNullMultiple);
            scope.Writer.Write(count);
        }

        stream.Position = 0;

        // BinaryFormattedObject blocks before it creates any objects
        Action action = () => Deserialize(stream);
        action.Should().Throw<SerializationException>();
    }

    [Fact]
    public virtual void NullRecord_ZeroByte()
    {
        MemoryStream stream = new();
        using (BinaryFormatWriterScope scope = new(stream))
        {
            scope.Writer.Write((byte)RecordType.ArraySingleObject);

            // Id
            scope.Writer.Write(1);

            // Length
            scope.Writer.Write(1);

            scope.Writer.Write((byte)RecordType.ObjectNullMultiple256);
            scope.Writer.Write((byte)0);
        }

        stream.Position = 0;

        object deserialized = Deserialize(stream);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    public virtual void NullRecord_WrongLength_WithTuple(int nullCount)
    {
        Tuple<object?> tuple = new(null);
        Stream stream = Serialize(tuple);
        BinaryFormattedObject format = new(stream);

        stream.Position = 0;

        using (BinaryFormatWriterScope scope = new(stream))
        {
            scope.Writer.Write((byte)RecordType.ArraySingleObject);

            // Id
            scope.Writer.Write(1);

            // Length
            scope.Writer.Write(2);

            // Write a member reference to the tuple, followed by a null record with zero length.
            new MemberReference(2).Write(scope);
            scope.Writer.Write((byte)RecordType.ObjectNullMultiple256);
            scope.Writer.Write((byte)nullCount);

            // Write the Tuple out with an updated Id
            scope.Writer.Write((byte)RecordType.SystemClassWithMembersAndTypes);
            var record = (SystemClassWithMembersAndTypes)format[(Id)1];
            ClassInfo classInfo = record.ClassInfo;
            new ClassInfo(2, classInfo.Name, classInfo.MemberNames).Write(scope);
            scope.Writer.Write(record.MemberTypeInfo);
            NullRecord.Write(scope, 1);
        }

        stream.Position = 0;

        object deserialized = Deserialize(stream);
    }

    [Fact]
    public virtual void NullRecord_ZeroInt()
    {
        MemoryStream stream = new();
        using (BinaryFormatWriterScope scope = new(stream))
        {
            scope.Writer.Write((byte)RecordType.ArraySingleObject);

            // Id
            scope.Writer.Write(1);

            // Length
            scope.Writer.Write(1);

            scope.Writer.Write((byte)RecordType.ObjectNullMultiple);
            scope.Writer.Write(0);
        }

        stream.Position = 0;

        object deserialized = Deserialize(stream);
    }

    [Fact]
    public virtual void NullRecord_FollowingReferenceable()
    {
        MemoryStream stream = new();
        using (BinaryFormatWriterScope scope = new(stream))
        {
            new ArraySingleObject(1, [null]).Write(scope);
            NullRecord.Write(scope, 1);
        }

        stream.Position = 0;

        // Not technically valid, the BinaryFormatter gets a null ref that it turns into SerializationException.
        // (According to the specification you shouldn't have null records that don't follow object or array records.)
        object deserialized = Deserialize(stream);

        deserialized.Should().BeEquivalentTo(new object?[1]);
    }

    [Fact]
    public virtual void NullRecord_BeforeReferenceable()
    {
        MemoryStream stream = new();
        using (BinaryFormatWriterScope scope = new(stream))
        {
            NullRecord.Write(scope, 1);
            new ArraySingleObject(1, [null]).Write(scope);
        }

        stream.Position = 0;

        // Not technically valid, the BinaryFormatter gets a null ref that it turns into SerializationException.
        // (According to the specification you shouldn't have null records that don't follow object or array records.)
        object deserialized = Deserialize(stream);

        deserialized.Should().BeEquivalentTo(new object?[1]);
    }

    [Fact]
    public virtual void Tuple_WithMultipleNullCount()
    {
        Tuple<object?> tuple = new(null);
        Stream stream = Serialize(tuple);
        BinaryFormattedObject format = new(stream);

        stream.Position = 0;
        using (BinaryFormatWriterScope scope = new(stream))
        {
            scope.Writer.Write((byte)RecordType.SystemClassWithMembersAndTypes);
            var record = (SystemClassWithMembersAndTypes)format[(Id)1];
            record.ClassInfo.Write(scope);
            scope.Writer.Write(record.MemberTypeInfo);
            NullRecord.Write(scope, 2);
        }

        stream.Position = 0;

        object deserialized = Deserialize(stream);
    }

    [Fact]
    public virtual void Tuple_WithZeroNullCount()
    {
        Tuple<object?> tuple = new(null);
        Stream stream = Serialize(tuple);
        BinaryFormattedObject format = new(stream);

        stream.Position = 0;
        using (BinaryFormatWriterScope scope = new(stream))
        {
            scope.Writer.Write((byte)RecordType.SystemClassWithMembersAndTypes);
            var record = (SystemClassWithMembersAndTypes)format[(Id)1];
            record.ClassInfo.Write(scope);
            scope.Writer.Write(record.MemberTypeInfo);

            scope.Writer.Write((byte)RecordType.ObjectNullMultiple);
            scope.Writer.Write(0);
        }

        stream.Position = 0;

        object deserialized = Deserialize(stream);
    }
}

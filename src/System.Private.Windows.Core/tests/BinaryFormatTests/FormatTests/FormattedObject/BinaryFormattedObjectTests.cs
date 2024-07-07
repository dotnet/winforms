// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Private.Windows.Core.BinaryFormat;
using FormatTests.Common;

namespace FormatTests.FormattedObject;

public class BinaryFormattedObjectTests : SerializationTest<FormattedObjectSerializer>
{
    [Fact]
    public void ReadHeader()
    {
        BinaryFormattedObject format = new(Serialize("Hello World."));
        format.RootRecord.Id.Should().Be(1);
    }

    [Theory]
    [InlineData("Hello there.")]
    [InlineData("")]
    [InlineData("Embedded\0 Null.")]
    public void ReadBinaryObjectString(string testString)
    {
        BinaryFormattedObject format = new(Serialize(testString));
        BinaryObjectString stringRecord = (BinaryObjectString)format[1];
        stringRecord.ObjectId.Should().Be(1);
        stringRecord.Value.Should().Be(testString);
    }

    [Fact]
    public void ReadEmptyHashTable()
    {
        BinaryFormattedObject format = new(Serialize(new Hashtable()));

        SystemClassWithMembersAndTypes systemClass = (SystemClassWithMembersAndTypes)format[1];
        systemClass.ObjectId.Should().Be(1);
        systemClass.Name.Should().Be("System.Collections.Hashtable");
        systemClass.MemberNames.Should().BeEquivalentTo(
        [
            "LoadFactor",
            "Version",
            "Comparer",
            "HashCodeProvider",
            "HashSize",
            "Keys",
            "Values"
        ]);

        systemClass.MemberTypeInfo.Should().BeEquivalentTo(new MemberTypeInfo[]
        {
            new(BinaryType.Primitive, PrimitiveType.Single),
            new(BinaryType.Primitive, PrimitiveType.Int32),
            new(BinaryType.SystemClass, "System.Collections.IComparer"),
            new(BinaryType.SystemClass, "System.Collections.IHashCodeProvider"),
            new(BinaryType.Primitive, PrimitiveType.Int32),
            new(BinaryType.ObjectArray, null),
            new(BinaryType.ObjectArray, null)
        });

        systemClass.MemberValues.Should().BeEquivalentTo(new object?[]
        {
            0.72f,
            0,
            null,
            null,
            3,
            new MemberReference(2),
            new MemberReference(3)
        });

        ArraySingleObject array = (ArraySingleObject)format[2];
        array.ObjectId.Should().Be(2);
        array.Length.Should().Be(0);

        array = (ArraySingleObject)format[3];
        array.ObjectId.Should().Be(3);
        array.Length.Should().Be(0);
    }

    [Fact]
    public void ReadHashTableWithStringPair()
    {
        BinaryFormattedObject format = new(Serialize(new Hashtable()
        {
            { "This", "That" }
        }));

        SystemClassWithMembersAndTypes systemClass = (SystemClassWithMembersAndTypes)format[1];

        systemClass.MemberTypeInfo.Should().BeEquivalentTo(new MemberTypeInfo[]
        {
            new(BinaryType.Primitive, PrimitiveType.Single),
            new(BinaryType.Primitive, PrimitiveType.Int32),
            new(BinaryType.SystemClass, "System.Collections.IComparer"),
            new(BinaryType.SystemClass, "System.Collections.IHashCodeProvider"),
            new(BinaryType.Primitive, PrimitiveType.Int32),
            new(BinaryType.ObjectArray, null),
            new(BinaryType.ObjectArray, null)
        });

        systemClass.MemberValues.Should().BeEquivalentTo(new object?[]
        {
            0.72f,
            1,
            null,
            null,
            3,
            new MemberReference(2),
            new MemberReference(3)
        });

        ArraySingleObject array = (ArraySingleObject)format[2];
        array.ObjectId.Should().Be(2);
        array.Length.Should().Be(1);
        BinaryObjectString value = (BinaryObjectString)array.ArrayObjects[0]!;
        value.ObjectId.Should().Be(4);
        value.Value.Should().Be("This");

        array = (ArraySingleObject)format[3];
        array.ObjectId.Should().Be(3);
        array.Length.Should().Be(1);
        value = (BinaryObjectString)array.ArrayObjects[0]!;
        value.ObjectId.Should().Be(5);
        value.Value.Should().Be("That");
    }

    [Fact]
    public void ReadHashTableWithRepeatedStrings()
    {
        BinaryFormattedObject format = new(Serialize(new Hashtable()
        {
            { "This", "That" },
            { "TheOther", "This" },
            { "That", "This" }
        }));

        // The collections themselves get ids first before the strings do.
        // Everything in the second array is a string reference.
        ArraySingleObject array = (ArraySingleObject)format[3];
        array.ObjectId.Should().Be(3);
        array[0].Should().BeOfType<MemberReference>();
        array[1].Should().BeOfType<MemberReference>();
        array[2].Should().BeOfType<MemberReference>();
    }

    [Fact]
    public void ReadHashTableWithNullValues()
    {
        BinaryFormattedObject format = new(Serialize(new Hashtable()
        {
            { "Yowza", null },
            { "Youza", null },
            { "Meeza", null }
        }));

        SystemClassWithMembersAndTypes systemClass = (SystemClassWithMembersAndTypes)format[1];

        systemClass.MemberValues.Should().BeEquivalentTo(new object?[]
        {
            0.72f,
            4,
            null,
            null,
            7,
            new MemberReference(2),
            new MemberReference(3)
        });

        ArrayRecord<object> array = (ArrayRecord<object>)format[(MemberReference)systemClass.MemberValues[5]!];

        array.ObjectId.Should().Be(2);
        array.Length.Should().Be(3);
        BinaryObjectString value = (BinaryObjectString)array.ArrayObjects[0];
        value.ObjectId.Should().Be(4);
        value.Value.Should().BeOneOf("Yowza", "Youza", "Meeza");

        array = (ArrayRecord<object>)format[(MemberReference)systemClass["Values"]!];
        array.ObjectId.Should().Be(3);
        array.Length.Should().Be(3);
        array.ArrayObjects[0].Should().BeNull();
    }

    [Fact]
    public void ReadObject()
    {
        BinaryFormattedObject format = new(Serialize(new object()));
        format[1].Should().BeOfType<SystemClassWithMembersAndTypes>();
    }

    [Fact]
    public void ReadStruct()
    {
        ValueTuple<int> tuple = new(355);
        BinaryFormattedObject format = new(Serialize(tuple));
        format[1].Should().BeOfType<SystemClassWithMembersAndTypes>();
    }

    [Fact]
    public void ReadSimpleSerializableObject()
    {
        BinaryFormattedObject format = new(Serialize(new SimpleSerializableObject()));

        ClassWithMembersAndTypes @class = (ClassWithMembersAndTypes)format.RootRecord;
        @class.ObjectId.Should().Be(1);
        @class.Name.Should().Be(typeof(SimpleSerializableObject).FullName);
        @class.MemberNames.Should().BeEmpty();
        @class.LibraryId.Should().Be(2);
        @class.MemberTypeInfo.Should().BeEmpty();

        format[@class.LibraryId].Should().BeOfType<BinaryLibrary>()
            .Which.LibraryName.Should().Be(typeof(BinaryFormattedObjectTests).Assembly.FullName);
    }

    [Fact]
    public void ReadNestedSerializableObject()
    {
        BinaryFormattedObject format = new(Serialize(new NestedSerializableObject()));

        ClassWithMembersAndTypes @class = (ClassWithMembersAndTypes)format.RootRecord;
        @class.ObjectId.Should().Be(1);
        @class.Name.Should().Be(typeof(NestedSerializableObject).FullName);
        @class.MemberNames.Should().BeEquivalentTo(["_object", "_meaning"]);
        @class.LibraryId.Should().Be(2);
        @class.MemberTypeInfo.Should().BeEquivalentTo(new MemberTypeInfo[]
        {
            new(BinaryType.Class, new ClassTypeInfo(typeof(SimpleSerializableObject).FullName!, 2)),
            new(BinaryType.Primitive, PrimitiveType.Int32)
        });

        @class.MemberValues.Should().BeEquivalentTo(new object?[]
        {
            new MemberReference(3),
            42
        });

        @class = (ClassWithMembersAndTypes)format[3];
        @class.ObjectId.Should().Be(3);
        @class.Name.Should().Be(typeof(SimpleSerializableObject).FullName);
        @class.MemberNames.Should().BeEmpty();
        @class.LibraryId.Should().Be(2);
        @class.MemberTypeInfo.Should().BeEmpty();
    }

    [Fact]
    public void ReadTwoIntObject()
    {
        BinaryFormattedObject format = new(Serialize(new TwoIntSerializableObject()));

        ClassWithMembersAndTypes @class = (ClassWithMembersAndTypes)format.RootRecord;
        @class.ObjectId.Should().Be(1);
        @class.Name.Should().Be(typeof(TwoIntSerializableObject).FullName);
        @class.MemberNames.Should().BeEquivalentTo(["_value", "_meaning"]);
        @class.LibraryId.Should().Be(2);
        @class.MemberTypeInfo.Should().BeEquivalentTo(new MemberTypeInfo[]
        {
            new(BinaryType.Primitive, PrimitiveType.Int32),
            new(BinaryType.Primitive, PrimitiveType.Int32)
        });

        @class.MemberValues.Should().BeEquivalentTo(new object?[]
        {
            1970,
            42
        });
    }

    [Fact]
    public void ReadRepeatedNestedObject()
    {
        BinaryFormattedObject format = new(Serialize(new RepeatedNestedSerializableObject()));
        ClassWithMembersAndTypes firstClass = (ClassWithMembersAndTypes)format[3];
        ClassWithId classWithId = (ClassWithId)format[4];
        classWithId.MetadataId.Should().Be(firstClass.ObjectId);
        classWithId.MemberValues.Should().BeEquivalentTo(new object[] { 1970, 42 });
    }

    [Fact]
    public void ReadPrimitiveArray()
    {
        BinaryFormattedObject format = new(Serialize(new int[] { 10, 9, 8, 7 }));

        ArraySinglePrimitive<int> array = (ArraySinglePrimitive<int>)format[1];
        array.Length.Should().Be(4);
        array.PrimitiveType.Should().Be(PrimitiveType.Int32);
        array.ArrayObjects.Should().BeEquivalentTo(new object[] { 10, 9, 8, 7 });
    }

    [Fact]
    public void ReadStringArray()
    {
        BinaryFormattedObject format = new(Serialize(new string[] { "Monday", "Tuesday", "Wednesday" }));
        ArraySingleString array = (ArraySingleString)format[1];
        array.ObjectId.Should().Be(1);
        array.Length.Should().Be(3);
        array.ArrayObjects[0].Should().Be("Monday");
    }

    [Fact]
    public void ReadStringArrayWithNulls()
    {
        BinaryFormattedObject format = new(Serialize(new string?[] { "Monday", null, "Wednesday", null, null, null }));
        ArraySingleString array = (ArraySingleString)format[1];
        array.ObjectId.Should().Be(1);
        array.Length.Should().Be(6);
        array.ArrayObjects.Should().BeEquivalentTo(new object?[]
        {
            "Monday",
            null,
            "Wednesday",
            null,
            null,
            null
        });
    }

    [Fact]
    public void ReadDuplicatedStringArray()
    {
        BinaryFormattedObject format = new(Serialize(new string[] { "Monday", "Tuesday", "Monday" }));
        ArraySingleString array = (ArraySingleString)format[1];
        array.ObjectId.Should().Be(1);
        array.Length.Should().Be(3);
        array.ArrayObjects[0].Should().BeSameAs(array.ArrayObjects[2]);
    }

    [Fact]
    public void ReadObjectWithNullableObjects()
    {
        BinaryFormattedObject format = new(Serialize(new ObjectWithNullableObjects()));
        ClassWithMembersAndTypes classRecord = (ClassWithMembersAndTypes)format.RootRecord;
        format[classRecord.LibraryId].Should().BeOfType<BinaryLibrary>();
    }

    [Fact]
    public void ReadNestedObjectWithNullableObjects()
    {
        BinaryFormattedObject format = new(Serialize(new NestedObjectWithNullableObjects()));
        ClassWithMembersAndTypes classRecord = (ClassWithMembersAndTypes)format.RootRecord;
        format[classRecord.LibraryId].Should().BeOfType<BinaryLibrary>();
    }

    [Serializable]
    private class SimpleSerializableObject
    {
    }

#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414  // Field is assigned but its value is never used
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
#pragma warning disable CA1823 // Avoid unused private fields
    [Serializable]
    private class ObjectWithNullableObjects
    {
        public object? First;
        public object? Second;
        public object? Third;
    }

    [Serializable]
    private class NestedObjectWithNullableObjects
    {
        public ObjectWithNullableObjects? First;
        public ObjectWithNullableObjects? Second;
        public ObjectWithNullableObjects? Third = new();
    }

    [Serializable]
    private class NestedSerializableObject
    {
        private readonly SimpleSerializableObject _object = new();
        private readonly int _meaning = 42;
    }

    [Serializable]
    private class TwoIntSerializableObject
    {
        private readonly int _value = 1970;
        private readonly int _meaning = 42;
    }

    [Serializable]
    private class RepeatedNestedSerializableObject
    {
        private readonly TwoIntSerializableObject _first = new();
        private readonly TwoIntSerializableObject _second = new();
    }
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore CS0414  // Field is assigned but its value is never used
#pragma warning restore CS0649  // Field is never assigned to, and will always have its default value null
#pragma warning restore CA1823 // Avoid unused private fields
}

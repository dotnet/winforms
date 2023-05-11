// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Windows.Forms.BinaryFormat.Tests;

public class ListTests
{
    [Fact]
    public void List_Int_ParseEmpty()
    {
        BinaryFormattedObject format = new List<int>().SerializeAndParse();
        SystemClassWithMembersAndTypes classInfo = (SystemClassWithMembersAndTypes)format[1];

        // Note that T types are serialized as the mscorlib type.
        classInfo.Name.Should().Be(
            "System.Collections.Generic.List`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]");

        classInfo.ClassInfo.MemberNames.Should().BeEquivalentTo(new string[]
        {
            "_items",
            // This is something that wouldn't be needed if List<T> implemented ISerializable. If we format
            // we can save any extra unused array spots.
            "_size",
            // It is a bit silly that _version gets serialized, it's only use is as a check to see if
            // the collection is modified while it is being enumerated.
            "_version"
        });
        classInfo.MemberTypeInfo[0].Should().Be((BinaryType.PrimitiveArray, PrimitiveType.Int32));
        classInfo.MemberTypeInfo[1].Should().Be((BinaryType.Primitive, PrimitiveType.Int32));
        classInfo.MemberTypeInfo[2].Should().Be((BinaryType.Primitive, PrimitiveType.Int32));
        classInfo["_items"].Should().BeOfType<MemberReference>();
        classInfo["_size"].Should().Be(0);
        classInfo["_version"].Should().Be(0);

        ArraySinglePrimitive array = (ArraySinglePrimitive)format[2];
        array.Length.Should().Be(0);
    }

    [Fact]
    public void List_String_ParseEmpty()
    {
        BinaryFormattedObject format = new List<string>().SerializeAndParse();
        SystemClassWithMembersAndTypes classInfo = (SystemClassWithMembersAndTypes)format[1];
        classInfo.ClassInfo.Name.Should().StartWith("System.Collections.Generic.List`1[[System.String,");
        classInfo.MemberTypeInfo[0].Should().Be((BinaryType.StringArray, null));
        classInfo["_items"].Should().BeOfType<MemberReference>();

        ArraySingleString array = (ArraySingleString)format[2];
        array.Length.Should().Be(0);
    }

    [Theory]
    [MemberData(nameof(PrimitiveLists_TestData))]
    public void List_Primitive_Write(IList list)
    {
        using MemoryStream stream = new();

        switch (list)
        {
            case List<int> intList:
                BinaryFormatWriter.WritePrimitiveList(stream, intList);
                break;
            case List<float> floatList:
                BinaryFormatWriter.WritePrimitiveList(stream, floatList);
                break;
            case List<byte> byteList:
                BinaryFormatWriter.WritePrimitiveList(stream, byteList);
                break;
            case List<char> charList:
                BinaryFormatWriter.WritePrimitiveList(stream, charList);
                break;
            default:
                throw new InvalidOperationException();
        }

        stream.Position = 0;

        using var formatterScope = new BinaryFormatterScope(enable: true);
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        BinaryFormatter formatter = new();
        IList deserialized = (IList)formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011

        deserialized.Should().BeEquivalentTo(list);
    }

    [Theory]
    [MemberData(nameof(PrimitiveLists_TestData))]
    public void List_Primitive_Read(IList list)
    {
        BinaryFormattedObject format = list.SerializeAndParse();

        object? deserialized;
        bool success = list switch
        {
            List<int> => format.TryGetPrimitiveList<int>(out deserialized),
            List<float> => format.TryGetPrimitiveList<float>(out deserialized),
            List<byte> => format.TryGetPrimitiveList<byte>(out deserialized),
            List<char> => format.TryGetPrimitiveList<char>(out deserialized),
            _ => throw new InvalidOperationException(),
        };

        success.Should().BeTrue();
        deserialized.Should().BeEquivalentTo(list);
    }

    public static TheoryData<IList> PrimitiveLists_TestData => new()
    {
        new List<int>(),
        new List<float>() { 3.14f },
        new List<float>() { float.NaN, float.PositiveInfinity, float.NegativeInfinity, float.NegativeZero },
        new List<int>() { 1, 3, 4, 5, 6, 7 },
        new List<byte>() { 0xDE, 0xAD, 0xBE, 0xEF },
        new List<char>() { 'a', 'b',  'c', 'd', 'e', 'f', 'g', 'h' },
        new List<char>() { 'a', '\0',  'c' },
    };
}

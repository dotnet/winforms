// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Formats.Nrbf;
using System.Reflection.Metadata;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Private.Windows.Nrbf;

public class CoreNrbfSerializerTests
{
    public static TheoryData<object, bool> TryWriteObjectData => new()
    {
        { 123, true },
        { "test", true },
        { new object(), false }
    };

    [Theory]
    [MemberData(nameof(TryWriteObjectData))]
    public void TryWriteObject_TryGetObject_RoundTrip(object input, bool expectedResult)
    {
        using MemoryStream stream = new();
        bool result = CoreNrbfSerializer.TryWriteObject(stream, input);
        result.Should().Be(expectedResult);

        if (!expectedResult)
        {
            stream.Position.Should().Be(0);
            return;
        }

        stream.Position = 0;
        SerializationRecord record = NrbfDecoder.Decode(stream);
        CoreNrbfSerializer.TryGetObject(record, out object? value).Should().BeTrue();
        value.Should().Be(input);
    }

    [Theory]
    [MemberData(nameof(TryWriteObjectData))]
    public void BinaryFormatterWrite_TryGetObject_RoundTrip(object input, bool expectedResult)
    {
        using MemoryStream stream = new();
        using (BinaryFormatterScope scope = new(enable: true))
        {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            new BinaryFormatter().Serialize(stream, input);
#pragma warning restore SYSLIB0011
        }

        stream.Position = 0;
        SerializationRecord record = NrbfDecoder.Decode(stream, leaveOpen: true);
        CoreNrbfSerializer.TryGetObject(record, out object? value).Should().Be(expectedResult);
        if (expectedResult)
        {
            value.Should().Be(input);
        }
    }

    public static TheoryData<string, bool, Type?> TryBindToTypeData => new()
    {
        { typeof(int).FullName!, true, typeof(int) },
        { $"{typeof(int).FullName!}, {typeof(int).Assembly.FullName}", true, typeof(int) },
        { $"{typeof(int).FullName!}, {Assemblies.Mscorlib}", true, typeof(int) },
        { typeof(int[]).FullName!, true, typeof(int[]) },
        { $"{typeof(int[]).FullName!}, {typeof(int[]).Assembly.FullName}", true, typeof(int[]) },
        { $"{typeof(int[]).FullName!}, {Assemblies.Mscorlib}", true, typeof(int[]) },
        { typeof(List<int>).FullName!, true, typeof(List<int>) },
        { $"{typeof(List<int>).FullName!}, {typeof(List<int>).Assembly.FullName}", true, typeof(List<int>) },
        { "Int32", false, null },
        { "My.Int32", false, null },
        { "Unknown.Type", false, null }
    };

    [Theory]
    [MemberData(nameof(TryBindToTypeData))]
    public void TryBindToType_ShouldReturnExpectedResult(string typeNameString, bool expectedResult, Type? expectedType)
    {
        TypeName typeName = TypeName.Parse(typeNameString);
        bool result = CoreNrbfSerializer.TryBindToType(typeName, out Type? type);
        result.Should().Be(expectedResult);
        type.Should().Be(expectedType);
    }

    public static TheoryData<Type, bool> IsSupportedTypeData => new()
    {
        { typeof(int), true },
        { typeof(object), false }
    };

    [Theory]
    [MemberData(nameof(IsSupportedTypeData))]
    public void IsSupportedType_ShouldReturnExpectedResult(Type type, bool expectedResult)
    {
        CoreNrbfSerializer.IsFullySupportedType(type).Should().Be(expectedResult);
    }
}

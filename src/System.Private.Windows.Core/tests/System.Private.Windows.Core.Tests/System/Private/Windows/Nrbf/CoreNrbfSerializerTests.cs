// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Formats.Nrbf;
using System.Reflection;
using System.Reflection.Metadata;

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
    public void TryWriteObject_ShouldReturnExpectedResult(object input, bool expectedResult)
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

    public static TheoryData<string, bool, Type?> TryBindToTypeData => new()
    {
        { typeof(int).FullName!, true, typeof(int) },
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
        MethodInfo method = typeof(CoreNrbfSerializer).GetMethod("IsSupportedType")!.MakeGenericMethod(type);
        bool result = (bool)method.Invoke(null, null)!;
        result.Should().Be(expectedResult);
    }
}

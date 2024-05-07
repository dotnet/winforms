// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using BinaryFormatTests;
using BinaryFormatTests.FormatterTests;
using System.Runtime.Serialization.Formatters;

namespace FormatTests.Common;

public abstract class BasicObjectTests<T> : SerializationTest<T> where T : ISerializer
{
    private protected abstract bool SkipOffsetArrays { get; }

    [Theory]
    [MemberData(nameof(SerializableObjects))]
    public void DeserializeStoredObjects(object value, TypeSerializableValue[] serializedData)
    {
        _ = value;

        int platformIndex = serializedData.GetPlatformIndex();
        for (int i = 0; i < serializedData.Length; i++)
            for (FormatterAssemblyStyle assemblyMatching = 0; assemblyMatching <= FormatterAssemblyStyle.Full; assemblyMatching++)
            {
                object deserialized = DeserializeFromBase64Chars(serializedData[i].Base64Blob, assemblyMatching: assemblyMatching);

                if (deserialized is StringComparer)
                {
                    // StringComparer derived classes are not public and they don't serialize the actual type.
                    value.Should().BeAssignableTo<StringComparer>();
                }
                else
                {
                    deserialized.Should().BeOfType(value.GetType());
                }

                bool isSamePlatform = i == platformIndex;
                EqualityExtensions.CheckEquals(value, deserialized, isSamePlatform);
            }
    }

    [Theory]
    [MemberData(nameof(BasicObjectsRoundtrip_MemberData))]
    public void BasicObjectsRoundtrip(
        object value,
        FormatterAssemblyStyle assemblyMatching,
        FormatterTypeStyle typeStyle)
    {
        object deserialized = RoundTrip(value, typeStyle: typeStyle, assemblyMatching: assemblyMatching);

        // string.Empty and DBNull are both singletons
        if (!ReferenceEquals(value, string.Empty)
            && value is not DBNull
            && value is Array array
            && array.Length > 0)
        {
            deserialized.Should().NotBeSameAs(value);
        }

        EqualityExtensions.CheckEquals(value, deserialized, isSamePlatform: true);
    }

    public static TheoryData<object, TypeSerializableValue[]> SerializableObjects()
    {
        List<(object Object, TypeSerializableValue[] Serialized)> data = [];

        foreach (var value in BinaryFormatterTests.RawSerializableObjects())
        {
            // Explicitly not supporting offset arrays
            if (value.Item1 is Array array && (array.GetLowerBound(0) != 0))
            {
                continue;
            }

            data.Add(value);
        }

        // Doing two steps for debugging purposes. Can add a .Skip() to get to the failing scenario.

        TheoryData<object, TypeSerializableValue[]> theoryData = [];

        foreach (var item in data.Skip(0))
        {
            theoryData.Add(item.Object, item.Serialized);
        }

        return theoryData;
    }

    public static TheoryData<object, FormatterAssemblyStyle, FormatterTypeStyle> BasicObjectsRoundtrip_MemberData()
    {
        List<(object Object, FormatterAssemblyStyle AssemblyStyle, FormatterTypeStyle TypeStyle)> data = new(7000);

        foreach (object[]? record in SerializableObjects())
        {
            foreach (FormatterAssemblyStyle assemblyFormat in new[] { FormatterAssemblyStyle.Full, FormatterAssemblyStyle.Simple })
            {
                foreach (FormatterTypeStyle typeFormat in new[] { FormatterTypeStyle.TypesAlways, FormatterTypeStyle.TypesWhenNeeded, FormatterTypeStyle.XsdString })
                {
                    data.Add((record[0], assemblyFormat, typeFormat));
                }
            }
        }

        TheoryData<object, FormatterAssemblyStyle, FormatterTypeStyle> theoryData = [];

        foreach (var item in data.Skip(0))
        {
            theoryData.Add(item.Object, item.AssemblyStyle, item.TypeStyle);
        }

        return theoryData;
    }
}

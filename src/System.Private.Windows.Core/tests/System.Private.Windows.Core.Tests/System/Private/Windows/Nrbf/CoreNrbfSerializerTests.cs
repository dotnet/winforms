// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Formats.Nrbf;
using System.Reflection.Metadata;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Private.Windows.Nrbf;

public class CoreNrbfSerializerTests
{
    public static TheoryData<object, bool> TryWriteObjectData => new()
    {
        { 123, true },
        { "test", true },
        { new object(), false },
        { new Hashtable() { { "key", "value" } }, true },
        { new Hashtable(StringComparer.OrdinalIgnoreCase) { { "key", "value" } }, true }
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

    [Fact]
    public void HashtableType_IsNotFullySupportedType()
    {
        // Hashtable is not fully supported but can be round-tripped through TryWriteObject/TryGetObject
        CoreNrbfSerializer.IsFullySupportedType(typeof(Hashtable)).Should().BeFalse();
    }
    
    [Theory]
    [MemberData(nameof(HashtableTestData))]
    public void Hashtable_TryWriteObject_TryGetObject_RoundTrip(Hashtable hashtable, bool expectSuccessfulDeserialization)
    {
        using MemoryStream stream = new();
        bool result = CoreNrbfSerializer.TryWriteObject(stream, hashtable);
        result.Should().BeTrue(); // All Hashtables should be serializable
        
        stream.Position = 0;
        SerializationRecord record = NrbfDecoder.Decode(stream);
        bool deserializationResult = CoreNrbfSerializer.TryGetObject(record, out object? value);
        deserializationResult.Should().Be(expectSuccessfulDeserialization);
        
        // When deserialization is expected to succeed, verify the key-value pairs
        if (expectSuccessfulDeserialization)
        {
            value.Should().BeOfType<Hashtable>();
            Hashtable? deserializedTable = value as Hashtable;
            deserializedTable!.Count.Should().Be(hashtable.Count);
            
            foreach (DictionaryEntry entry in hashtable)
            {
                deserializedTable.Contains(entry.Key).Should().BeTrue();
                deserializedTable[entry.Key].Should().Be(entry.Value);
            }
        }
    }
    
    [Theory]
    [MemberData(nameof(HashtableTestData))]
    public void BinaryFormatter_Hashtable_TryGetObject_RoundTrip(Hashtable hashtable, bool expectSuccessfulDeserialization) 
    {
        using MemoryStream stream = new();
        using (BinaryFormatterScope scope = new(enable: true))
        {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            new BinaryFormatter().Serialize(stream, hashtable);
#pragma warning restore SYSLIB0011
        }

        stream.Position = 0;
        SerializationRecord record = NrbfDecoder.Decode(stream, leaveOpen: true);
        bool deserializationResult = CoreNrbfSerializer.TryGetObject(record, out object? value);
        deserializationResult.Should().Be(expectSuccessfulDeserialization);

        // When deserialization is expected to succeed, verify the key-value pairs
        if (expectSuccessfulDeserialization)
        {
            value.Should().BeOfType<Hashtable>();
            Hashtable? deserializedTable = value as Hashtable;
            deserializedTable!.Count.Should().Be(hashtable.Count);
            
            foreach (DictionaryEntry entry in hashtable)
            {
                deserializedTable.Contains(entry.Key).Should().BeTrue();
                deserializedTable[entry.Key].Should().Be(entry.Value);
            }
        }
    }
    
    public static TheoryData<Hashtable, bool> HashtableTestData => new()
    {
        // Standard hashtable should deserialize correctly
        { new Hashtable() { { "key", "value" } }, true },
        { new Hashtable() { { 1, 2 }, { "text", 42 } }, true },
        
        // Hashtable with custom comparer won't be deserialized by TryGetPrimitiveHashtable
        { new Hashtable(StringComparer.OrdinalIgnoreCase) { { "key", "value" } }, false },
        { new Hashtable(StringComparer.CurrentCulture) { { "key", "value" } }, false },
        
        // Hashtable with hash code provider won't be deserialized by TryGetPrimitiveHashtable
        { new Hashtable(new CustomHashCodeProvider()) { { "key", "value" } }, false },
        { new Hashtable(new CustomHashCodeProvider(), StringComparer.OrdinalIgnoreCase) { { "key", "value" } }, false }
    };
    
    [Fact]
    public void HashtableWithCustomComparer_PreservesData_EvenWhenNotDeserialized()
    {
        // Create a hashtable with a custom comparer
        Hashtable originalHashtable = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Key", "Value" }
        };

        // Serialize with BinaryFormatter
        using MemoryStream stream = new();
        using (BinaryFormatterScope scope = new(enable: true))
        {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            new BinaryFormatter().Serialize(stream, originalHashtable);
#pragma warning restore SYSLIB0011
        }

        // First verify that CoreNrbfSerializer can't deserialize it
        stream.Position = 0;
        SerializationRecord record = NrbfDecoder.Decode(stream, leaveOpen: true);
        CoreNrbfSerializer.TryGetObject(record, out _).Should().BeFalse();

        // Now verify that BinaryFormatter can deserialize it with all data intact
        stream.Position = 0;
        using (BinaryFormatterScope scope = new(enable: true))
        {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            Hashtable deserializedHashtable = (Hashtable)new BinaryFormatter().Deserialize(stream);
#pragma warning restore SYSLIB0011

            // Verify key-value data
            deserializedHashtable.Count.Should().Be(originalHashtable.Count);
            deserializedHashtable["Key"].Should().Be("Value");

            // Verify case-insensitivity was preserved (comparer was not lost)
            deserializedHashtable["key"].Should().Be("Value");
        }
    }

    [Fact]
    public void HashtableWithHashCodeProvider_PreservesData_EvenWhenNotDeserialized()
    {
        // Create a hashtable with a custom hash code provider
        Hashtable originalHashtable = new(new CustomHashCodeProvider())
        {
            { "Key", "Value" }
        };

        // Serialize with BinaryFormatter
        using MemoryStream stream = new();
        using (BinaryFormatterScope scope = new(enable: true))
        {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            new BinaryFormatter().Serialize(stream, originalHashtable);
#pragma warning restore SYSLIB0011
        }

        // First verify that CoreNrbfSerializer can't deserialize it
        stream.Position = 0;
        SerializationRecord record = NrbfDecoder.Decode(stream, leaveOpen: true);
        CoreNrbfSerializer.TryGetObject(record, out _).Should().BeFalse();

        // Now verify that BinaryFormatter can deserialize it with all data intact
        stream.Position = 0;
        using (BinaryFormatterScope scope = new(enable: true))
        {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            Hashtable deserializedHashtable = (Hashtable)new BinaryFormatter().Deserialize(stream);
#pragma warning restore SYSLIB0011

            // Verify key-value data
            deserializedHashtable.Count.Should().Be(originalHashtable.Count);
            deserializedHashtable["Key"].Should().Be("Value");
            
            // Verify the hash code provider was preserved
            deserializedHashtable.GetType().GetField("_keycomparer", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .Should().NotBeNull();
        }
    }
    
    [Serializable]
    private class CustomHashCodeProvider : IHashCodeProvider
    {
        public int GetHashCode(object obj) => obj?.GetHashCode() ?? 0;
    }
}
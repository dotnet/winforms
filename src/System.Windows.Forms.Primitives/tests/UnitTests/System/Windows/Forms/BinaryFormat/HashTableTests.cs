﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Windows.Forms.BinaryFormat.Tests;

public class HashTableTests
{
    [Fact]
    public void HashTable_GetObjectData()
    {
        Hashtable hashtable = new()
        {
            { "This", "That" }
        };

        // The converter isn't used for this scenario and can be a no-op.
#pragma warning disable SYSLIB0050 // Type or member is obsolete
        SerializationInfo info = new(typeof(Hashtable), FormatterConverterStub.Instance);
#pragma warning restore SYSLIB0050
#pragma warning disable SYSLIB0051 // Type or member is obsolete
        hashtable.GetObjectData(info, default);
#pragma warning restore SYSLIB0051
        info.MemberCount.Should().Be(7);

        var enumerator = info.GetEnumerator();

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Name.Should().Be("LoadFactor");
        enumerator.Current.Value.Should().Be(0.72f);

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Name.Should().Be("Version");
        enumerator.Current.Value.Should().Be(1);

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Name.Should().Be("Comparer");
        enumerator.Current.Value.Should().BeNull();

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Name.Should().Be("HashCodeProvider");
        enumerator.Current.Value.Should().BeNull();

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Name.Should().Be("HashSize");
        enumerator.Current.Value.Should().Be(3);

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Name.Should().Be("Keys");
        enumerator.Current.Value.Should().BeEquivalentTo(new object[] { "This" });

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Name.Should().Be("Values");
        enumerator.Current.Value.Should().BeEquivalentTo(new object[] { "That" });
    }

    [Theory]
    [MemberData(nameof(Hashtables_TestData))]
    public void WriteHashtables(Hashtable hashtable)
    {
        using MemoryStream stream = new();
        BinaryFormatWriter.WriteHashtableOfStrings(stream, hashtable);
        stream.Position = 0;

        using var formatterScope = new BinaryFormatterScope(enable: true);
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        BinaryFormatter formatter = new();
        Hashtable deserialized = (Hashtable)formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011

        deserialized.Count.Should().Be(hashtable.Count);
        foreach (var key in hashtable.Keys)
        {
            deserialized[key].Should().Be(hashtable[key]);
        }
    }

    [Theory]
    [MemberData(nameof(Hashtables_TestData))]
    public void ReadHashtables(Hashtable hashtable)
    {
        Hashtable deserialized = BinaryFormatReader.ReadHashtableOfStrings(hashtable.Serialize());

        deserialized.Count.Should().Be(hashtable.Count);
        foreach (var key in hashtable.Keys)
        {
            deserialized[key].Should().Be(hashtable[key]);
        }
    }

    public static TheoryData<Hashtable> Hashtables_TestData = new()
    {
        new Hashtable(),
        new Hashtable()
        {
            { "This", "That" }
        },
        new Hashtable()
        {
            { "This", "That" },
            { "TheOther", "This" },
            { "That", "This" }
        },
        new Hashtable()
        {
            { "This", "That" },
            { "TheOther", "This" },
            { "That", "This" }
        },
        new Hashtable()
        {
            { "Yowza", null },
            { "Youza", null },
            { "Meeza", null }
        },
        new Hashtable()
        {
            { "Yowza", null },
            { "Youza", "Binks" },
            { "Meeza", null }
        },
        new Hashtable()
        {
            { "Yowza", "Binks" },
            { "Youza", "Binks" },
            { "Meeza", null }
        },
        // Stress the string interning
        MakeRepeatedHashtable(50, "Ditto"),
        MakeRepeatedHashtable(100, "..."),
        // Cross over into ObjectNullMultiple
        MakeRepeatedHashtable(255, null),
        MakeRepeatedHashtable(256, null),
        MakeRepeatedHashtable(257, null)
    };

    private static Hashtable MakeRepeatedHashtable(int countOfEntries, object? value)
    {
        Hashtable result = new(countOfEntries);
        for (int i = 1; i <= countOfEntries; i++)
        {
            result.Add($"Entry{i}", value);
        }

        return result;
    }

#pragma warning disable SYSLIB0050 // Type or member is obsolete
    private sealed class FormatterConverterStub : IFormatterConverter
    {
        public static IFormatterConverter Instance { get; } = new FormatterConverterStub();
#pragma warning restore SYSLIB0050

        public object Convert(object value, Type type) => throw new NotImplementedException();
        public object Convert(object value, TypeCode typeCode) => throw new NotImplementedException();
        public bool ToBoolean(object value) => throw new NotImplementedException();
        public byte ToByte(object value) => throw new NotImplementedException();
        public char ToChar(object value) => throw new NotImplementedException();
        public DateTime ToDateTime(object value) => throw new NotImplementedException();
        public decimal ToDecimal(object value) => throw new NotImplementedException();
        public double ToDouble(object value) => throw new NotImplementedException();
        public short ToInt16(object value) => throw new NotImplementedException();
        public int ToInt32(object value) => throw new NotImplementedException();
        public long ToInt64(object value) => throw new NotImplementedException();
        public sbyte ToSByte(object value) => throw new NotImplementedException();
        public float ToSingle(object value) => throw new NotImplementedException();
        public string? ToString(object value) => throw new NotImplementedException();
        public ushort ToUInt16(object value) => throw new NotImplementedException();
        public uint ToUInt32(object value) => throw new NotImplementedException();
        public ulong ToUInt64(object value) => throw new NotImplementedException();
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Private.Windows.Core.BinaryFormat;
using FormatTests.Common;
using Record = System.Private.Windows.Core.BinaryFormat.Serializer.Record;
using System.Formats.Nrbf;
using System.Windows.Forms.Nrbf;
using System.Globalization;
using System.Runtime.Serialization;
using System.Private.Windows.Core.BinaryFormat.Serializer;

namespace FormatTests.FormattedObject;

public class PrimitiveTypeTests : SerializationTest
{
    [Theory]
    [MemberData(nameof(RoundTrip_Data))]
    public void WriteReadPrimitiveValue_RoundTrip(byte type, object value)
    {
        MemoryStream stream = new();
        using (BinaryWriter writer = new(stream, Encoding.UTF8, leaveOpen: true))
        {
            TestRecord.WritePrimitiveValue(writer, (PrimitiveType)type, value);
        }

        stream.Position = 0;

        using BinaryReader reader = new(stream);
        object result = TestRecord.ReadPrimitiveValue(reader, (PrimitiveType)type);
        result.Should().Be(value);
    }

    public static TheoryData<byte, object> RoundTrip_Data => new()
    {
        { (byte)PrimitiveType.Int64, 0L },
        { (byte)PrimitiveType.Int64, -1L },
        { (byte)PrimitiveType.Int64, 1L },
        { (byte)PrimitiveType.Int64, long.MaxValue },
        { (byte)PrimitiveType.Int64, long.MinValue },
        { (byte)PrimitiveType.UInt64, ulong.MaxValue },
        { (byte)PrimitiveType.UInt64, ulong.MinValue },
        { (byte)PrimitiveType.Int32, 0 },
        { (byte)PrimitiveType.Int32, -1 },
        { (byte)PrimitiveType.Int32, 1 },
        { (byte)PrimitiveType.Int32, int.MaxValue },
        { (byte)PrimitiveType.Int32, int.MinValue },
        { (byte)PrimitiveType.UInt32, uint.MaxValue },
        { (byte)PrimitiveType.UInt32, uint.MinValue },
        { (byte)PrimitiveType.Int16, (short)0 },
        { (byte)PrimitiveType.Int16, (short)-1 },
        { (byte)PrimitiveType.Int16, (short)1 },
        { (byte)PrimitiveType.Int16, short.MaxValue },
        { (byte)PrimitiveType.Int16, short.MinValue },
        { (byte)PrimitiveType.UInt16, ushort.MaxValue },
        { (byte)PrimitiveType.UInt16, ushort.MinValue },
        { (byte)PrimitiveType.SByte, (sbyte)0 },
        { (byte)PrimitiveType.SByte, (sbyte)-1 },
        { (byte)PrimitiveType.SByte, (sbyte)1 },
        { (byte)PrimitiveType.SByte, sbyte.MaxValue },
        { (byte)PrimitiveType.SByte, sbyte.MinValue },
        { (byte)PrimitiveType.Byte, byte.MinValue },
        { (byte)PrimitiveType.Byte, byte.MaxValue },
        { (byte)PrimitiveType.Boolean, true },
        { (byte)PrimitiveType.Boolean, false },
        { (byte)PrimitiveType.Single, 0.0f },
        { (byte)PrimitiveType.Single, -1.0f },
        { (byte)PrimitiveType.Single, 1.0f },
        { (byte)PrimitiveType.Single, float.MaxValue },
        { (byte)PrimitiveType.Single, float.MinValue },
        { (byte)PrimitiveType.Single, float.NegativeZero },
        { (byte)PrimitiveType.Single, float.NaN },
        { (byte)PrimitiveType.Single, float.NegativeInfinity },
        { (byte)PrimitiveType.Double, 0.0d },
        { (byte)PrimitiveType.Double, -1.0d },
        { (byte)PrimitiveType.Double, 1.0d },
        { (byte)PrimitiveType.Double, double.MaxValue },
        { (byte)PrimitiveType.Double, double.MinValue },
        { (byte)PrimitiveType.Double, double.NegativeZero },
        { (byte)PrimitiveType.Double, double.NaN },
        { (byte)PrimitiveType.Double, double.NegativeInfinity },
        { (byte)PrimitiveType.TimeSpan, TimeSpan.MinValue },
        { (byte)PrimitiveType.TimeSpan, TimeSpan.MaxValue },
        { (byte)PrimitiveType.DateTime, DateTime.MinValue },
        { (byte)PrimitiveType.DateTime, DateTime.MaxValue },
    };

    [Theory]
    [MemberData(nameof(Primitive_Data))]
    [MemberData(nameof(Primitive_ExtendedData))]
    public void BinaryFormatWriter_WritePrimitive(object value)
    {
        MemoryStream stream = new();
        BinaryFormatWriter.WritePrimitive(stream, value);
        stream.Position = 0;

        // cs/binary-formatter-without-binder
        BinaryFormatter formatter = new(); // CodeQL [SM04191] : This is a test. Safe use because the deserialization process is performed on trusted data and the types are controlled and validated.

        // cs/dangerous-binary-deserialization
        object deserialized = formatter.Deserialize(stream); // CodeQL [SM03722] : Testing legacy feature. This is a safe use of BinaryFormatter because the data is trusted and the types are controlled and validated.
        deserialized.Should().Be(value);
    }

    [Theory]
    [MemberData(nameof(Primitive_Data))]
    [MemberData(nameof(Primitive_ExtendedData))]
    public void SerializationRecord_ReadPrimitive(object value)
    {
        SerializationRecord rootRecord = NrbfDecoder.Decode(Serialize(value));
        rootRecord.TryGetPrimitiveType(out object? deserialized).Should().BeTrue();
        deserialized.Should().Be(value);
    }

    public static TheoryData<object> Primitive_Data => new()
    {
        int.MaxValue,
        uint.MaxValue,
        long.MaxValue,
        ulong.MaxValue,
        short.MaxValue,
        ushort.MaxValue,
        byte.MaxValue,
        sbyte.MaxValue,
        true,
        float.MaxValue,
        double.MaxValue,
        char.MaxValue
    };

    public static TheoryData<object> Primitive_ExtendedData => new()
    {
        TimeSpan.MaxValue,
        DateTime.MaxValue,
        decimal.MaxValue,
        (nint)1918,
        (nuint)2020,
        "Roundabout"
    };

    internal class TestRecord : Record
    {
        public static void WritePrimitiveValue(BinaryWriter writer, PrimitiveType type, object value)
            => WritePrimitiveType(writer, type, value);

        public static object ReadPrimitiveValue(BinaryReader reader, PrimitiveType primitiveType)
             => primitiveType switch
             {
                 PrimitiveType.Boolean => reader.ReadBoolean(),
                 PrimitiveType.Byte => reader.ReadByte(),
                 PrimitiveType.SByte => reader.ReadSByte(),
                 PrimitiveType.Char => reader.ReadChar(),
                 PrimitiveType.Int16 => reader.ReadInt16(),
                 PrimitiveType.UInt16 => reader.ReadUInt16(),
                 PrimitiveType.Int32 => reader.ReadInt32(),
                 PrimitiveType.UInt32 => reader.ReadUInt32(),
                 PrimitiveType.Int64 => reader.ReadInt64(),
                 PrimitiveType.UInt64 => reader.ReadUInt64(),
                 PrimitiveType.Single => reader.ReadSingle(),
                 PrimitiveType.Double => reader.ReadDouble(),
                 PrimitiveType.Decimal => decimal.Parse(reader.ReadString(), CultureInfo.InvariantCulture),
                 PrimitiveType.DateTime => reader.ReadDateTime(),
                 PrimitiveType.TimeSpan => new TimeSpan(reader.ReadInt64()),
                 // String is handled with a record, never on it's own
                 _ => throw new SerializationException($"Failure trying to read primitive '{primitiveType}'"),
             };

        private protected override void Write(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}

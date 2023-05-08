// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;

namespace System.Windows.Forms.BinaryFormat.Tests;

public class PrimitiveTypes
{
    [Theory]
    [MemberData(nameof(RoundTrip_Data))]
    public void RoundTripPrimitiveTypes(byte type, object value)
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

    internal class TestRecord : Record
    {
        public static void WritePrimitiveValue(BinaryWriter writer, PrimitiveType type, object value)
            => WritePrimitiveType(writer, type, value);

        public static object ReadPrimitiveValue(BinaryReader reader, PrimitiveType type)
            => ReadPrimitiveType(reader, type);

        public override void Write(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}

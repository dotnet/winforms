// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers.Binary;

namespace System.Private.Windows.Ole;

internal sealed class EmfPayload
{
    private const uint EmfEofRecord = 14;
    private const uint EmfHeaderRecord = 1;
    private const uint EmfRectangleRecord = 43;
    private const uint EmfSignature = 0x464D4520;
    private const int MinimumHeaderSize = 88;

    private EmfPayload(byte[] bits)
    {
        Bits = bits;
        RecordCount = BinaryPrimitives.ReadUInt32LittleEndian(bits.AsSpan(52));
        RecordTypes = ReadRecordTypes(bits);
    }

    public byte[] Bits { get; }

    public uint RecordCount { get; }

    public IReadOnlyList<uint> RecordTypes { get; }

    public static EmfPayload FromHandle(nint handle)
    {
        byte[] bits = EmfNativeMethods.GetBits(handle);

        // Validate the fixed ENHMETAHEADER fields before walking the variable-length record stream.
        bits.Length.Should().BeGreaterThanOrEqualTo(MinimumHeaderSize);
        BinaryPrimitives.ReadUInt32LittleEndian(bits).Should().Be(EmfHeaderRecord);
        BinaryPrimitives.ReadUInt32LittleEndian(bits.AsSpan(4)).Should().BeGreaterThanOrEqualTo(MinimumHeaderSize);
        BinaryPrimitives.ReadUInt32LittleEndian(bits.AsSpan(40)).Should().Be(EmfSignature);
        BinaryPrimitives.ReadUInt32LittleEndian(bits.AsSpan(48)).Should().Be((uint)bits.Length);
        BinaryPrimitives.ReadUInt32LittleEndian(bits.AsSpan(52)).Should().BeGreaterThan(2);
        EmfPayload payload = new(bits);
        payload.RecordTypes.Should().StartWith(EmfHeaderRecord);
        payload.RecordTypes.Should().Contain(EmfRectangleRecord);
        payload.RecordTypes.Should().EndWith(EmfEofRecord);
        payload.RecordTypes.Should().HaveCount((int)payload.RecordCount);

        return payload;
    }

    private static IReadOnlyList<uint> ReadRecordTypes(byte[] bits)
    {
        List<uint> recordTypes = [];
        int offset = 0;

        while (offset <= bits.Length - 8)
        {
            ReadOnlySpan<byte> record = bits.AsSpan(offset);
            uint type = BinaryPrimitives.ReadUInt32LittleEndian(record);
            uint size = BinaryPrimitives.ReadUInt32LittleEndian(record[4..]);

            // Every EMF record is DWORD-aligned and must remain within the byte count declared by the header.
            size.Should().BeGreaterThanOrEqualTo(8);
            (size % 4).Should().Be(0);
            size.Should().BeLessThanOrEqualTo((uint)(bits.Length - offset));

            recordTypes.Add(type);
            offset = checked(offset + (int)size);

            if (type == EmfEofRecord)
            {
                break;
            }
        }

        offset.Should().Be(bits.Length);

        return recordTypes;
    }
}

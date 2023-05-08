// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Primitive value other than <see langword="string"/>.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/c0a190b2-762c-46b9-89f2-c7dabecfc084">
///    [MS-NRBF] 2.5.1
///   </see>
///  </para>
/// </remarks>
internal sealed class MemberPrimitiveTyped : Record, IRecord<MemberPrimitiveTyped>
{
    public PrimitiveType PrimitiveType { get; }
    public object Value { get; }

    public MemberPrimitiveTyped(PrimitiveType primitiveType, object value)
    {
        PrimitiveType = primitiveType;
        Value = value;
    }

    public static RecordType RecordType => RecordType.MemberPrimitiveTyped;

    static MemberPrimitiveTyped IBinaryFormatParseable<MemberPrimitiveTyped>.Parse(
        BinaryReader reader,
        RecordMap recordMap)
    {
        PrimitiveType primitiveType = (PrimitiveType)reader.ReadByte();
        return new(
            primitiveType,
            ReadPrimitiveType(reader, primitiveType));
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)RecordType);
        WritePrimitiveType(writer, PrimitiveType, Value);
    }
}

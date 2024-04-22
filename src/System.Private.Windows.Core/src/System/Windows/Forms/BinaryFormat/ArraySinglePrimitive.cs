// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Single dimensional array of a primitive type.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/3a50a305-5f32-48a1-a42a-c34054db310b">
///    [MS-NRBF] 2.4.3.3
///   </see>
///  </para>
/// </remarks>
internal sealed class ArraySinglePrimitive<T> : ArrayRecord<T>, IRecord<ArraySinglePrimitive<T>>, IPrimitiveTypeRecord
    where T : unmanaged
{
    public PrimitiveType PrimitiveType { get; }

    public static RecordType RecordType => RecordType.ArraySinglePrimitive;

    public ArraySinglePrimitive(Id objectId, IReadOnlyList<T> arrayObjects)
        : base(new ArrayInfo(objectId, arrayObjects.Count), arrayObjects)
    {
        PrimitiveType = TypeInfo.GetPrimitiveType(typeof(T));
    }

    static ArraySinglePrimitive<T> IBinaryFormatParseable<ArraySinglePrimitive<T>>.Parse(
        BinaryReader reader,
        RecordMap recordMap)
    {
        Id id = ArrayInfo.Parse(reader, out Count length);
        PrimitiveType primitiveType = (PrimitiveType)reader.ReadByte();
        Debug.Assert(typeof(T) == primitiveType.GetPrimitiveTypeType());

        ArraySinglePrimitive<T> record = new(id, ReadPrimitiveTypes<T>(reader, length));
        recordMap[record.ObjectId] = record;
        return record;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)RecordType);
        ArrayInfo.Write(writer);
        writer.Write((byte)PrimitiveType);
        WritePrimitiveTypes(writer, ArrayObjects);
    }
}

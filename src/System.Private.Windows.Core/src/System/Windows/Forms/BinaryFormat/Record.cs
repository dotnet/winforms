// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Runtime.Serialization;

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Base record class.
/// </summary>
internal abstract class Record : IRecord
{
    /// <summary>
    ///  Reads a primitive of <paramref name="primitiveType"/> from the given <paramref name="reader"/>.
    /// </summary>
    private protected static object ReadPrimitiveType(BinaryReader reader, PrimitiveType primitiveType) => primitiveType switch
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

    /// <summary>
    ///  Writes <paramref name="value"/> as <paramref name="primitiveType"/> to the given <paramref name="writer"/>.
    /// </summary>
    private protected static unsafe void WritePrimitiveType(BinaryWriter writer, PrimitiveType primitiveType, object value)
    {
        switch (primitiveType)
        {
            case PrimitiveType.Boolean:
                writer.Write((bool)value);
                break;
            case PrimitiveType.Byte:
                writer.Write((byte)value);
                break;
            case PrimitiveType.Char:
                writer.Write((char)value);
                break;
            case PrimitiveType.Decimal:
                writer.Write(((decimal)value).ToString(CultureInfo.InvariantCulture));
                break;
            case PrimitiveType.Double:
                writer.Write((double)value);
                break;
            case PrimitiveType.Int16:
                writer.Write((short)value);
                break;
            case PrimitiveType.Int32:
                writer.Write((int)value);
                break;
            case PrimitiveType.Int64:
                writer.Write((long)value);
                break;
            case PrimitiveType.SByte:
                writer.Write((sbyte)value);
                break;
            case PrimitiveType.Single:
                writer.Write((float)value);
                break;
            case PrimitiveType.TimeSpan:
                writer.Write(((TimeSpan)value).Ticks);
                break;
            case PrimitiveType.DateTime:
                writer.Write((DateTime)value);
                break;
            case PrimitiveType.UInt16:
                writer.Write((ushort)value);
                break;
            case PrimitiveType.UInt32:
                writer.Write((uint)value);
                break;
            case PrimitiveType.UInt64:
                writer.Write((ulong)value);
                break;
            // String is handled with a record, never on it's own
            case PrimitiveType.Null:
            case PrimitiveType.String:
            default:
                throw new ArgumentException("Invalid primitive type.", nameof(primitiveType));
        }
    }

    /// <summary>
    ///  Reads the next record.
    /// </summary>
    /// <exception cref="NotImplementedException">Found a multidimensional array.</exception>
    /// <exception cref="NotSupportedException">Found a remote method invocation record.</exception>
    /// <exception cref="SerializationException">Unknown or corrupted data.</exception>
    internal static IRecord ReadBinaryFormatRecord(BinaryFormattedObject.IParseState state)
    {
        RecordType recordType = (RecordType)state.Reader.ReadByte();

        return recordType switch
        {
            RecordType.SerializedStreamHeader => ReadSpecificRecord<SerializationHeader>(state),
            RecordType.ClassWithId => ReadSpecificRecord<ClassWithId>(state),
            RecordType.SystemClassWithMembers => ReadSpecificRecord<SystemClassWithMembers>(state),
            RecordType.ClassWithMembers => ReadSpecificRecord<ClassWithMembers>(state),
            RecordType.SystemClassWithMembersAndTypes => ReadSpecificRecord<SystemClassWithMembersAndTypes>(state),
            RecordType.ClassWithMembersAndTypes => ReadSpecificRecord<ClassWithMembersAndTypes>(state),
            RecordType.BinaryObjectString => ReadSpecificRecord<BinaryObjectString>(state),
            RecordType.BinaryArray => BinaryArray.Parse(state),
            RecordType.MemberPrimitiveTyped => ReadSpecificRecord<MemberPrimitiveTyped>(state),
            RecordType.MemberReference => ReadSpecificRecord<MemberReference>(state),
            RecordType.ObjectNull => ReadSpecificRecord<ObjectNull>(state),
            RecordType.MessageEnd => ReadSpecificRecord<MessageEnd>(state),
            RecordType.BinaryLibrary => ReadSpecificRecord<BinaryLibrary>(state),
            RecordType.ObjectNullMultiple256 => ReadSpecificRecord<NullRecord.ObjectNullMultiple256>(state),
            RecordType.ObjectNullMultiple => ReadSpecificRecord<NullRecord.ObjectNullMultiple>(state),
            RecordType.ArraySinglePrimitive => ReadArraySinglePrimitive(state),
            RecordType.ArraySingleObject => ReadSpecificRecord<ArraySingleObject>(state),
            RecordType.ArraySingleString => ReadSpecificRecord<ArraySingleString>(state),
            RecordType.MethodCall => throw new NotSupportedException(),
            RecordType.MethodReturn => throw new NotSupportedException(),
            _ => throw new SerializationException("Invalid record type."),
        };

        static IRecord ReadArraySinglePrimitive(BinaryFormattedObject.IParseState state)
        {
            // Special casing to avoid excessive boxing/unboxing.
            Id id = ArrayInfo.Parse(state.Reader, out Count length);
            PrimitiveType primitiveType = (PrimitiveType)state.Reader.ReadByte();

            IRecord record = primitiveType switch
            {
                PrimitiveType.Boolean => new ArraySinglePrimitive<bool>(id, state.Reader.ReadPrimitiveArray<bool>(length)),
                PrimitiveType.Byte => new ArraySinglePrimitive<byte>(id, state.Reader.ReadPrimitiveArray<byte>(length)),
                PrimitiveType.SByte => new ArraySinglePrimitive<sbyte>(id, state.Reader.ReadPrimitiveArray<sbyte>(length)),
                PrimitiveType.Char => new ArraySinglePrimitive<char>(id, state.Reader.ReadPrimitiveArray<char>(length)),
                PrimitiveType.Int16 => new ArraySinglePrimitive<short>(id, state.Reader.ReadPrimitiveArray<short>(length)),
                PrimitiveType.UInt16 => new ArraySinglePrimitive<ushort>(id, state.Reader.ReadPrimitiveArray<ushort>(length)),
                PrimitiveType.Int32 => new ArraySinglePrimitive<int>(id, state.Reader.ReadPrimitiveArray<int>(length)),
                PrimitiveType.UInt32 => new ArraySinglePrimitive<uint>(id, state.Reader.ReadPrimitiveArray<uint>(length)),
                PrimitiveType.Int64 => new ArraySinglePrimitive<long>(id, state.Reader.ReadPrimitiveArray<long>(length)),
                PrimitiveType.UInt64 => new ArraySinglePrimitive<ulong>(id, state.Reader.ReadPrimitiveArray<ulong>(length)),
                PrimitiveType.Single => new ArraySinglePrimitive<float>(id, state.Reader.ReadPrimitiveArray<float>(length)),
                PrimitiveType.Double => new ArraySinglePrimitive<double>(id, state.Reader.ReadPrimitiveArray<double>(length)),
                PrimitiveType.Decimal => new ArraySinglePrimitive<decimal>(id, state.Reader.ReadPrimitiveArray<decimal>(length)),
                PrimitiveType.DateTime => new ArraySinglePrimitive<DateTime>(id, state.Reader.ReadPrimitiveArray<DateTime>(length)),
                PrimitiveType.TimeSpan => new ArraySinglePrimitive<TimeSpan>(id, state.Reader.ReadPrimitiveArray<TimeSpan>(length)),
                _ => throw new SerializationException($"Invalid primitive type '{primitiveType}'"),
            };

            state.RecordMap[id] = record;
            return record;
        }

        static TRecord ReadSpecificRecord<TRecord>(BinaryFormattedObject.IParseState state)
            where TRecord : class, IRecord<TRecord>, IBinaryFormatParseable<TRecord> => TRecord.Parse(state);
    }

    /// <summary>
    ///  Writes records, coalescing null records into single entries.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///  <paramref name="objects"/> contained an object that isn't a record.
    /// </exception>
    private protected static void WriteRecords(BinaryWriter writer, IReadOnlyList<object?> objects)
    {
        int nullCount = 0;

        // Indexing for performance and avoiding writing an enumerator of ListConverter.
        for (int i = 0; i < objects.Count; i++)
        {
            object? @object = objects[i];

            // Aggregate consecutive null records.
            if (@object is null)
            {
                nullCount++;
                continue;
            }

            if (nullCount > 0)
            {
                NullRecord.Write(writer, nullCount);
                nullCount = 0;
            }

            if (@object is not IRecord record || record is NullRecord)
            {
                throw new ArgumentException("Invalid record.", nameof(objects));
            }

            record.Write(writer);
        }

        if (nullCount > 0)
        {
            NullRecord.Write(writer, nullCount);
        }
    }

    /// <summary>
    ///  Reads an object member value of <paramref name="type"/> with optional clarifying <paramref name="typeInfo"/>.
    /// </summary>
    /// <exception cref="SerializationException"><paramref name="type"/> was unexpected.</exception>
    private protected static object ReadValue(
        BinaryFormattedObject.IParseState state,
        BinaryType type,
        object? typeInfo)
    {
        if (type == BinaryType.Primitive)
        {
            return ReadPrimitiveType(state.Reader, (PrimitiveType)typeInfo!);
        }

        if (type == BinaryType.String)
        {
            IRecord stringRecord = ReadBinaryFormatRecord(state);

            if (stringRecord is not (BinaryObjectString or ObjectNull or MemberReference))
            {
                throw new SerializationException($"Expected string record, found {stringRecord.GetType().Name}");
            }

            return stringRecord;
        }

        // BinaryLibrary records can be dumped in front of any member reference.
        IRecord record;
        while ((record = ReadReference()) is BinaryLibrary)
        {
        }

        return record;

        IRecord ReadReference() => type switch
        {
            BinaryType.Object
                or BinaryType.StringArray
                or BinaryType.PrimitiveArray
                or BinaryType.Class
                or BinaryType.SystemClass
                or BinaryType.ObjectArray => ReadBinaryFormatRecord(state),
            _ => throw new SerializationException($"Invalid binary type {type}."),
        };
    }

    public abstract void Write(BinaryWriter writer);
}

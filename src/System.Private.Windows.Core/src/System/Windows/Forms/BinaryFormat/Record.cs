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
    ///  Reads <paramref name="count"/> primitives of <paramref name="primitiveType"/> from the given <paramref name="reader"/>.
    /// </summary>
    private protected static IReadOnlyList<object> ReadPrimitiveTypes(BinaryReader reader, PrimitiveType primitiveType, int count)
    {
        List<object> values = new(Math.Min(count, BinaryFormattedObject.MaxNewCollectionSize));
        for (int i = 0; i < count; i++)
        {
            values.Add(ReadPrimitiveType(reader, primitiveType));
        }

        return values;
    }

    private protected static IReadOnlyList<T> ReadPrimitiveTypes<T>(BinaryReader reader, int count)
        where T : unmanaged
    {
        // Special casing byte for performance.
        if (typeof(T) == typeof(byte))
        {
            byte[] bytes = reader.ReadBytes(count);
            return (IReadOnlyList<T>)(object)bytes;
        }

        List<T> values = new(Math.Min(count, BinaryFormattedObject.MaxNewCollectionSize));
        for (int i = 0; i < count; i++)
        {
            if (typeof(T) == typeof(bool))
            {
                values.Add((T)(object)reader.ReadBoolean());
            }
            else if (typeof(T) == typeof(sbyte))
            {
                values.Add((T)(object)reader.ReadSByte());
            }
            else if (typeof(T) == typeof(char))
            {
                values.Add((T)(object)reader.ReadChar());
            }
            else if (typeof(T) == typeof(short))
            {
                values.Add((T)(object)reader.ReadInt16());
            }
            else if (typeof(T) == typeof(ushort))
            {
                values.Add((T)(object)reader.ReadUInt16());
            }
            else if (typeof(T) == typeof(int))
            {
                values.Add((T)(object)reader.ReadInt32());
            }
            else if (typeof(T) == typeof(uint))
            {
                values.Add((T)(object)reader.ReadUInt32());
            }
            else if (typeof(T) == typeof(long))
            {
                values.Add((T)(object)reader.ReadInt64());
            }
            else if (typeof(T) == typeof(ulong))
            {
                values.Add((T)(object)reader.ReadUInt64());
            }
            else if (typeof(T) == typeof(float))
            {
                values.Add((T)(object)reader.ReadSingle());
            }
            else if (typeof(T) == typeof(double))
            {
                values.Add((T)(object)reader.ReadDouble());
            }
            else if (typeof(T) == typeof(decimal))
            {
                values.Add((T)(object)decimal.Parse(reader.ReadString(), CultureInfo.InvariantCulture));
            }
            else if (typeof(T) == typeof(DateTime))
            {
                values.Add((T)(object)reader.ReadDateTime());
            }
            else if (typeof(T) == typeof(TimeSpan))
            {
                values.Add((T)(object)new TimeSpan(reader.ReadInt64()));
            }
            else
            {
                throw new SerializationException($"Failure trying to read primitive '{typeof(T)}'");
            }
        }

        return values;
    }

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
    ///  Writes <paramref name="values"/> as <paramref name="primitiveType"/> to the given <paramref name="writer"/>.
    /// </summary>
    private protected static void WritePrimitiveTypes(BinaryWriter writer, PrimitiveType primitiveType, IReadOnlyList<object> values)
    {
        for (int i = 0; i < values.Count; i++)
        {
            WritePrimitiveType(writer, primitiveType, values[i]);
        }
    }

    private protected static void WritePrimitiveTypes<T>(BinaryWriter writer, IReadOnlyList<T> values)
        where T : unmanaged
    {
        // Special casing byte[] for performance.
        if (typeof(T) == typeof(byte))
        {
            if (values is byte[] byteArray)
            {
                writer.Write(byteArray);
                return;
            }
            else if (values is ArraySegment<byte> arraySegment)
            {
                writer.Write(arraySegment);
                return;
            }
        }

        for (int i = 0; i < values.Count; i++)
        {
            if (typeof(T) == typeof(bool))
            {
                writer.Write((bool)(object)values[i]);
            }
            else if (typeof(T) == typeof(byte))
            {
                writer.Write((byte)(object)values[i]);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                writer.Write((sbyte)(object)values[i]);
            }
            else if (typeof(T) == typeof(char))
            {
                writer.Write((char)(object)values[i]);
            }
            else if (typeof(T) == typeof(short))
            {
                writer.Write((short)(object)values[i]);
            }
            else if (typeof(T) == typeof(ushort))
            {
                writer.Write((ushort)(object)values[i]);
            }
            else if (typeof(T) == typeof(int))
            {
                writer.Write((int)(object)values[i]);
            }
            else if (typeof(T) == typeof(uint))
            {
                writer.Write((uint)(object)values[i]);
            }
            else if (typeof(T) == typeof(long))
            {
                writer.Write((long)(object)values[i]);
            }
            else if (typeof(T) == typeof(ulong))
            {
                writer.Write((ulong)(object)values[i]);
            }
            else if (typeof(T) == typeof(float))
            {
                writer.Write((float)(object)values[i]);
            }
            else if (typeof(T) == typeof(double))
            {
                writer.Write((double)(object)values[i]);
            }
            else if (typeof(T) == typeof(decimal))
            {
                writer.Write(((decimal)(object)values[i]).ToString(CultureInfo.InvariantCulture));
            }
            else if (typeof(T) == typeof(DateTime))
            {
                writer.Write((DateTime)(object)values[i]);
            }
            else if (typeof(T) == typeof(TimeSpan))
            {
                writer.Write(((TimeSpan)(object)values[i]).Ticks);
            }
            else
            {
                throw new SerializationException($"Failure trying to write primitive '{typeof(T)}'");
            }
        }
    }

    /// <summary>
    ///  Reads the next record from the given <paramref name="reader"/>.
    /// </summary>
    /// <exception cref="NotImplementedException">Found a multidimensional array.</exception>
    /// <exception cref="NotSupportedException">Found a remote method invocation record.</exception>
    /// <exception cref="SerializationException">Unknown or corrupted data.</exception>
    internal static IRecord ReadBinaryFormatRecord(BinaryReader reader, RecordMap recordMap)
    {
        RecordType recordType = (RecordType)reader.ReadByte();

        return recordType switch
        {
            RecordType.SerializedStreamHeader => ReadSpecificRecord<SerializationHeader>(recordMap),
            RecordType.ClassWithId => ReadSpecificRecord<ClassWithId>(recordMap),
            RecordType.SystemClassWithMembers => ReadSpecificRecord<SystemClassWithMembers>(recordMap),
            RecordType.ClassWithMembers => ReadSpecificRecord<ClassWithMembers>(recordMap),
            RecordType.SystemClassWithMembersAndTypes => ReadSpecificRecord<SystemClassWithMembersAndTypes>(recordMap),
            RecordType.ClassWithMembersAndTypes => ReadSpecificRecord<ClassWithMembersAndTypes>(recordMap),
            RecordType.BinaryObjectString => ReadSpecificRecord<BinaryObjectString>(recordMap),
            // The BinaryArray record is used for all types of arrays, but we currently only support single dimension.
            RecordType.BinaryArray => ReadSpecificRecord<BinaryArray>(recordMap),
            RecordType.MemberPrimitiveTyped => ReadSpecificRecord<MemberPrimitiveTyped>(recordMap),
            RecordType.MemberReference => ReadSpecificRecord<MemberReference>(recordMap),
            RecordType.ObjectNull => ReadSpecificRecord<ObjectNull>(recordMap),
            RecordType.MessageEnd => ReadSpecificRecord<MessageEnd>(recordMap),
            RecordType.BinaryLibrary => ReadSpecificRecord<BinaryLibrary>(recordMap),
            RecordType.ObjectNullMultiple256 => ReadSpecificRecord<NullRecord.ObjectNullMultiple256>(recordMap),
            RecordType.ObjectNullMultiple => ReadSpecificRecord<NullRecord.ObjectNullMultiple>(recordMap),
            RecordType.ArraySinglePrimitive => ReadArraySinglePrimitive(recordMap),
            RecordType.ArraySingleObject => ReadSpecificRecord<ArraySingleObject>(recordMap),
            RecordType.ArraySingleString => ReadSpecificRecord<ArraySingleString>(recordMap),
            RecordType.MethodCall => throw new NotSupportedException(),
            RecordType.MethodReturn => throw new NotSupportedException(),
            _ => throw new SerializationException("Invalid record type."),
        };

        IRecord ReadArraySinglePrimitive(RecordMap recordMap)
        {
            // Special casing to avoid excessive boxing/unboxing.
            Id id = ArrayInfo.Parse(reader, out Count length);
            PrimitiveType primitiveType = (PrimitiveType)reader.ReadByte();

            IRecord record = primitiveType switch
            {
                PrimitiveType.Boolean => new ArraySinglePrimitive<bool>(id, ReadPrimitiveTypes<bool>(reader, length)),
                PrimitiveType.Byte => new ArraySinglePrimitive<byte>(id, ReadPrimitiveTypes<byte>(reader, length)),
                PrimitiveType.SByte => new ArraySinglePrimitive<sbyte>(id, ReadPrimitiveTypes<sbyte>(reader, length)),
                PrimitiveType.Char => new ArraySinglePrimitive<char>(id, ReadPrimitiveTypes<char>(reader, length)),
                PrimitiveType.Int16 => new ArraySinglePrimitive<short>(id, ReadPrimitiveTypes<short>(reader, length)),
                PrimitiveType.UInt16 => new ArraySinglePrimitive<ushort>(id, ReadPrimitiveTypes<ushort>(reader, length)),
                PrimitiveType.Int32 => new ArraySinglePrimitive<int>(id, ReadPrimitiveTypes<int>(reader, length)),
                PrimitiveType.UInt32 => new ArraySinglePrimitive<uint>(id, ReadPrimitiveTypes<uint>(reader, length)),
                PrimitiveType.Int64 => new ArraySinglePrimitive<long>(id, ReadPrimitiveTypes<long>(reader, length)),
                PrimitiveType.UInt64 => new ArraySinglePrimitive<ulong>(id, ReadPrimitiveTypes<ulong>(reader, length)),
                PrimitiveType.Single => new ArraySinglePrimitive<float>(id, ReadPrimitiveTypes<float>(reader, length)),
                PrimitiveType.Double => new ArraySinglePrimitive<double>(id, ReadPrimitiveTypes<double>(reader, length)),
                PrimitiveType.Decimal => new ArraySinglePrimitive<decimal>(id, ReadPrimitiveTypes<decimal>(reader, length)),
                PrimitiveType.DateTime => new ArraySinglePrimitive<DateTime>(id, ReadPrimitiveTypes<DateTime>(reader, length)),
                PrimitiveType.TimeSpan => new ArraySinglePrimitive<TimeSpan>(id, ReadPrimitiveTypes<TimeSpan>(reader, length)),
                _ => throw new SerializationException($"Failure trying to read primitive '{primitiveType}'"),
            };

            recordMap[id] = record;
            return record;
        }

        unsafe TRecord ReadSpecificRecord<TRecord>(RecordMap recordMap) where TRecord : class, IRecord<TRecord>
        {
            return TRecord.Parse(reader, recordMap);
        }
    }

    /// <summary>
    ///  Reads records, expanding null records into individual entries.
    /// </summary>
    private protected static List<object> ReadRecords(BinaryReader reader, RecordMap recordMap, Count count)
    {
        List<object> objects = new(Math.Min(count, BinaryFormattedObject.MaxNewCollectionSize));

        for (int i = 0; i < count; i++)
        {
            var record = ReadBinaryFormatRecord(reader, recordMap);
            if (record is not NullRecord nullRecord)
            {
                objects.Add(record);
            }
            else
            {
                i += nullRecord.NullCount - 1;
                if (i >= count)
                {
                    throw new SerializationException();
                }

                for (int j = 0; j < nullRecord.NullCount; j++)
                {
                    objects.Add(ObjectNull.Instance);
                }
            }
        }

        return objects;
    }

    /// <summary>
    ///  Writes records, coalescing null records into single entries.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///  <paramref name="objects"/> contained an object that isn't a record.
    /// </exception>
    private protected static void WriteRecords(BinaryWriter writer, IReadOnlyList<object> objects)
    {
        int nullCount = 0;

        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] is not IRecord record)
            {
                throw new ArgumentException("Invalid record.", nameof(objects));
            }

            // Aggregate consecutive null records.
            if (record is NullRecord)
            {
                nullCount++;
                continue;
            }

            if (nullCount > 0)
            {
                NullRecord.Write(writer, nullCount);
                nullCount = 0;
            }

            record.Write(writer);
        }

        if (nullCount > 0)
        {
            NullRecord.Write(writer, nullCount);
        }
    }

    /// <summary>
    ///  Reads object member values using <paramref name="memberTypeInfo"/>.
    /// </summary>
    private protected static IReadOnlyList<object> ReadValuesFromMemberTypeInfo(
        BinaryReader reader,
        RecordMap recordMap,
        MemberTypeInfo memberTypeInfo)
    {
        List<object> memberValues = new(memberTypeInfo.Count);
        foreach ((BinaryType type, object? info) in memberTypeInfo)
        {
            memberValues.Add(ReadValue(reader, recordMap, type, info));
        }

        return memberValues;
    }

    /// <summary>
    ///  Reads an object member value of <paramref name="type"/> with optional clarifying <paramref name="typeInfo"/>.
    /// </summary>
    /// <exception cref="SerializationException"><paramref name="type"/> was unexpected.</exception>
    private protected static object ReadValue(
        BinaryReader reader,
        RecordMap recordMap,
        BinaryType type,
        object? typeInfo)
    {
        if (type == BinaryType.Primitive)
        {
            return ReadPrimitiveType(reader, (PrimitiveType)typeInfo!);
        }

        if (type == BinaryType.String)
        {
            return ReadBinaryFormatRecord(reader, recordMap);
        }

        // BinaryLibrary records can be dumped in front of any member reference.
        object record;
        while ((record = ReadReference()) is BinaryLibrary)
        {
        }

        return record;

        object ReadReference() => type switch
        {
            BinaryType.Object
                or BinaryType.StringArray
                or BinaryType.PrimitiveArray
                or BinaryType.Class
                or BinaryType.SystemClass
                or BinaryType.ObjectArray => ReadBinaryFormatRecord(reader, recordMap),
            _ => throw new SerializationException("Invalid binary type."),
        };
    }

    /// <summary>
    ///  Writes <paramref name="memberValues"/> as specified by the <paramref name="memberTypeInfo"/>
    /// </summary>
    private protected static void WriteValuesFromMemberTypeInfo(
        BinaryWriter writer,
        MemberTypeInfo memberTypeInfo,
        IReadOnlyList<object> memberValues)
    {
        for (int i = 0; i < memberTypeInfo.Count; i++)
        {
            (BinaryType type, object? info) = memberTypeInfo[i];
            switch (type)
            {
                case BinaryType.Primitive:
                    WritePrimitiveType(writer, (PrimitiveType)info!, memberValues[i]);
                    break;
                case BinaryType.String:
                case BinaryType.Object:
                case BinaryType.StringArray:
                case BinaryType.PrimitiveArray:
                case BinaryType.Class:
                case BinaryType.SystemClass:
                case BinaryType.ObjectArray:
                    ((IRecord)memberValues[i]).Write(writer);
                    break;
                default:
                    throw new ArgumentException("Invalid binary type.", nameof(memberTypeInfo));
            }
        }
    }

    public abstract void Write(BinaryWriter writer);
}

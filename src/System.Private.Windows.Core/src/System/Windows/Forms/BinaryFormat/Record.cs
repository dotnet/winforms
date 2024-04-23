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

    private protected static IReadOnlyList<T> ReadPrimitiveTypes<T>(BinaryReader reader, int count)
        where T : unmanaged
    {
        // Special casing simple primitives for performance.
        if (typeof(T) == typeof(bool)
            || typeof(T) == typeof(byte)
            || typeof(T) == typeof(sbyte)
            || typeof(T) == typeof(char)
            || typeof(T) == typeof(short)
            || typeof(T) == typeof(ushort)
            || typeof(T) == typeof(int)
            || typeof(T) == typeof(uint)
            || typeof(T) == typeof(long)
            || typeof(T) == typeof(ulong)
            || typeof(T) == typeof(float)
            || typeof(T) == typeof(double))
        {
            return reader.ReadPrimitiveArray<T>(count);
        }

        List<T> values = new(Math.Min(count, BinaryFormattedObject.MaxNewCollectionSize));
        for (int i = 0; i < count; i++)
        {
            if (typeof(T) == typeof(decimal))
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
                throw new SerializationException($"Invalid primitive type '{typeof(T)}'");
            }
        }

        // For simplicity in the deserialization model we'll always return an array.
        return [.. values];
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
    ///  Reads the next record.
    /// </summary>
    /// <exception cref="NotImplementedException">Found a multidimensional array.</exception>
    /// <exception cref="NotSupportedException">Found a remote method invocation record.</exception>
    /// <exception cref="SerializationException">Unknown or corrupted data.</exception>
    internal static IRecord ReadBinaryFormatRecord(BinaryFormattedObject.ParseState state)
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

        static IRecord ReadArraySinglePrimitive(BinaryFormattedObject.ParseState state)
        {
            // Special casing to avoid excessive boxing/unboxing.
            Id id = ArrayInfo.Parse(state.Reader, out Count length);
            PrimitiveType primitiveType = (PrimitiveType)state.Reader.ReadByte();

            IRecord record = primitiveType switch
            {
                PrimitiveType.Boolean => new ArraySinglePrimitive<bool>(id, ReadPrimitiveTypes<bool>(state.Reader, length)),
                PrimitiveType.Byte => new ArraySinglePrimitive<byte>(id, ReadPrimitiveTypes<byte>(state.Reader, length)),
                PrimitiveType.SByte => new ArraySinglePrimitive<sbyte>(id, ReadPrimitiveTypes<sbyte>(state.Reader, length)),
                PrimitiveType.Char => new ArraySinglePrimitive<char>(id, ReadPrimitiveTypes<char>(state.Reader, length)),
                PrimitiveType.Int16 => new ArraySinglePrimitive<short>(id, ReadPrimitiveTypes<short>(state.Reader, length)),
                PrimitiveType.UInt16 => new ArraySinglePrimitive<ushort>(id, ReadPrimitiveTypes<ushort>(state.Reader, length)),
                PrimitiveType.Int32 => new ArraySinglePrimitive<int>(id, ReadPrimitiveTypes<int>(state.Reader, length)),
                PrimitiveType.UInt32 => new ArraySinglePrimitive<uint>(id, ReadPrimitiveTypes<uint>(state.Reader, length)),
                PrimitiveType.Int64 => new ArraySinglePrimitive<long>(id, ReadPrimitiveTypes<long>(state.Reader, length)),
                PrimitiveType.UInt64 => new ArraySinglePrimitive<ulong>(id, ReadPrimitiveTypes<ulong>(state.Reader, length)),
                PrimitiveType.Single => new ArraySinglePrimitive<float>(id, ReadPrimitiveTypes<float>(state.Reader, length)),
                PrimitiveType.Double => new ArraySinglePrimitive<double>(id, ReadPrimitiveTypes<double>(state.Reader, length)),
                PrimitiveType.Decimal => new ArraySinglePrimitive<decimal>(id, ReadPrimitiveTypes<decimal>(state.Reader, length)),
                PrimitiveType.DateTime => new ArraySinglePrimitive<DateTime>(id, ReadPrimitiveTypes<DateTime>(state.Reader, length)),
                PrimitiveType.TimeSpan => new ArraySinglePrimitive<TimeSpan>(id, ReadPrimitiveTypes<TimeSpan>(state.Reader, length)),
                _ => throw new SerializationException($"Invalid primitive type '{primitiveType}'"),
            };

            state.RecordMap[id] = record;
            return record;
        }

        unsafe static TRecord ReadSpecificRecord<TRecord>(BinaryFormattedObject.ParseState state) where TRecord : class, IRecord<TRecord>, IBinaryFormatParseable<TRecord>
        {
            return TRecord.Parse(state);
        }
    }

    /// <summary>
    ///  Reads records, expanding null records into individual entries.
    /// </summary>
    private protected static List<object> ReadRecords(BinaryFormattedObject.ParseState state, Count count)
    {
        List<object> objects = new(Math.Min(count, BinaryFormattedObject.MaxNewCollectionSize));

        for (int i = 0; i < count; i++)
        {
            IRecord record = ReadBinaryFormatRecord(state);
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
    private protected static void WriteRecords(BinaryWriter writer, IReadOnlyList<object?> objects)
    {
        int nullCount = 0;

        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] is not IRecord record)
            {
                throw new ArgumentException("Invalid record.", nameof(objects));
            }

            // Aggregate consecutive null records.
            if (record is NullRecord or null)
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
        BinaryFormattedObject.ParseState state,
        MemberTypeInfo memberTypeInfo)
    {
        List<object> memberValues = new(Math.Min(memberTypeInfo.Count, BinaryFormattedObject.MaxNewCollectionSize));
        foreach ((BinaryType type, object? info) in memberTypeInfo)
        {
            memberValues.Add(ReadValue(state, type, info));
        }

        return memberValues;
    }

    /// <summary>
    ///  Reads a count of object member values of <paramref name="type"/> with optional clarifying <paramref name="typeInfo"/>.
    /// </summary>
    /// <exception cref="SerializationException"><paramref name="type"/> was unexpected.</exception>
    private protected static IReadOnlyList<object?> ReadValues(
        BinaryFormattedObject.ParseState state,
        BinaryType type,
        object? typeInfo,
        int count)
    {
        List<object?> memberValues = new(Math.Min(count, BinaryFormattedObject.MaxNewCollectionSize));
        for (int i = 0; i < count; i++)
        {
            object value = ReadValue(state, type, typeInfo);
            if (value is not NullRecord nullRecord)
            {
                memberValues.Add(value);
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
                    memberValues.Add(null);
                }
            }
        }

        return memberValues;
    }

    /// <summary>
    ///  Reads an object member value of <paramref name="type"/> with optional clarifying <paramref name="typeInfo"/>.
    /// </summary>
    /// <exception cref="SerializationException"><paramref name="type"/> was unexpected.</exception>
    private protected static object ReadValue(
        BinaryFormattedObject.ParseState state,
        BinaryType type,
        object? typeInfo)
    {
        if (type == BinaryType.Primitive)
        {
            return ReadPrimitiveType(state.Reader, (PrimitiveType)typeInfo!);
        }

        if (type == BinaryType.String)
        {
            return ReadBinaryFormatRecord(state);
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
                or BinaryType.ObjectArray => ReadBinaryFormatRecord(state),
            _ => throw new SerializationException("Invalid binary type."),
        };
    }

    /// <summary>
    ///  Writes <paramref name="memberValues"/> as specified by the <paramref name="memberTypeInfo"/>
    /// </summary>
    private protected static void WriteValuesFromMemberTypeInfo(
        BinaryWriter writer,
        MemberTypeInfo memberTypeInfo,
        IReadOnlyList<object?> memberValues)
    {
        for (int i = 0; i < memberTypeInfo.Count; i++)
        {
            (BinaryType type, object? info) = memberTypeInfo[i];
            switch (type)
            {
                case BinaryType.Primitive:
                    WritePrimitiveType(writer, (PrimitiveType)info!, memberValues[i]!);
                    break;
                case BinaryType.String:
                case BinaryType.Object:
                case BinaryType.StringArray:
                case BinaryType.PrimitiveArray:
                case BinaryType.Class:
                case BinaryType.SystemClass:
                case BinaryType.ObjectArray:
                    ((IRecord)memberValues[i]!).Write(writer);
                    break;
                default:
                    throw new ArgumentException("Invalid binary type.", nameof(memberTypeInfo));
            }
        }
    }

    public abstract void Write(BinaryWriter writer);
}

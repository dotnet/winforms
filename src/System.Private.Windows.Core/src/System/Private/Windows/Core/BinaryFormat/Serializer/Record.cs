// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Private.Windows.Core.BinaryFormat.Serializer;

/// <summary>
///  Base record class.
/// </summary>
internal abstract class Record : IWritableRecord
{
    // Prevent creating outside of the assembly.
    private protected Record() { }

    Id IRecord.Id => Id;

    private protected virtual Id Id => Id.Null;

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
    ///  Writes records, coalescing null records into single entries.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///  <paramref name="objects"/> contained an object that isn't a record.
    /// </exception>
    private protected static void WriteRecords(BinaryWriter writer, IReadOnlyList<object?> objects, bool coalesceNulls)
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
                if (coalesceNulls)
                {
                    continue;
                }
            }

            if (nullCount > 0)
            {
                NullRecord.Write(writer, nullCount);
                nullCount = 0;
            }

            if (@object is not IWritableRecord record || record is NullRecord)
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

    private protected abstract void Write(BinaryWriter writer);
    void IWritableRecord.Write(BinaryWriter writer) => Write(writer);
}

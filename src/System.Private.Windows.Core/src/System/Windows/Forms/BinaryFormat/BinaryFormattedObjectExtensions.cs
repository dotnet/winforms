// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Drawing;
using System.Formats.Nrbf;

namespace System.Windows.Forms.BinaryFormat;

internal static class BinaryFormattedObjectExtensions
{
    internal delegate bool TryGetDelegate(BinaryFormattedObject format, [NotNullWhen(true)] out object? value);

    internal static bool TryGet(TryGetDelegate get, BinaryFormattedObject format, [NotNullWhen(true)] out object? value)
    {
        try
        {
            return get(format, out value);
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidCastException)
        {
            // This should only really happen with corrupted data.
            Debug.Fail(ex.Message);
            value = default;
            return false;
        }
    }

    /// <summary>
    ///  Tries to get this object as a <see cref="PointF"/>.
    /// </summary>
    public static bool TryGetPointF(this BinaryFormattedObject format, [NotNullWhen(true)] out object? value)
    {
        return TryGet(Get, format, out value);

        static bool Get(BinaryFormattedObject format, [NotNullWhen(true)] out object? value)
        {
            value = default;

            if (format.RootRecord is not ClassRecord classInfo
                || !classInfo.TypeNameMatches(typeof(PointF))
                || !classInfo.HasMember("x")
                || !classInfo.HasMember("y"))
            {
                return false;
            }

            value = new PointF(classInfo.GetSingle("x"), classInfo.GetSingle("y"));

            return true;
        }
    }

    /// <summary>
    ///  Tries to get this object as a <see cref="RectangleF"/>.
    /// </summary>
    public static bool TryGetRectangleF(this BinaryFormattedObject format, [NotNullWhen(true)] out object? value)
    {
        return TryGet(Get, format, out value);

        static bool Get(BinaryFormattedObject format, [NotNullWhen(true)] out object? value)
        {
            value = default;

            if (format.RootRecord is not ClassRecord classInfo
                || !classInfo.TypeNameMatches(typeof(RectangleF))
                || !classInfo.HasMember("x")
                || !classInfo.HasMember("y")
                || !classInfo.HasMember("width")
                || !classInfo.HasMember("height"))
            {
                return false;
            }

            value = new RectangleF(
                classInfo.GetSingle("x"),
                classInfo.GetSingle("y"),
                classInfo.GetSingle("width"),
                classInfo.GetSingle("height"));

            return true;
        }
    }

    /// <summary>
    ///  Tries to get this object as a primitive type or string.
    /// </summary>
    /// <returns><see langword="true"/> if this represented a primitive type or string.</returns>
    public static bool TryGetPrimitiveType(this BinaryFormattedObject format, [NotNullWhen(true)] out object? value)
    {
        return TryGet(Get, format, out value);

        static bool Get(BinaryFormattedObject format, [NotNullWhen(true)] out object? value)
        {
            if (format.RootRecord.RecordType is SerializationRecordType.BinaryObjectString)
            {
                value = ((PrimitiveTypeRecord<string>)format.RootRecord).Value;
                return true;
            }
            else if (format.RootRecord.RecordType is SerializationRecordType.MemberPrimitiveTyped)
            {
                value = GetMemberPrimitiveTypedValue(format.RootRecord);
                return true;
            }

            value = null;
            return false;
        }
    }

    internal static object GetMemberPrimitiveTypedValue(this SerializationRecord record)
    {
        Debug.Assert(record.RecordType is SerializationRecordType.MemberPrimitiveTyped);

        return record switch
        {
            PrimitiveTypeRecord<string> primitive => primitive.Value,
            PrimitiveTypeRecord<bool> primitive => primitive.Value,
            PrimitiveTypeRecord<byte> primitive => primitive.Value,
            PrimitiveTypeRecord<sbyte> primitive => primitive.Value,
            PrimitiveTypeRecord<char> primitive => primitive.Value,
            PrimitiveTypeRecord<short> primitive => primitive.Value,
            PrimitiveTypeRecord<ushort> primitive => primitive.Value,
            PrimitiveTypeRecord<int> primitive => primitive.Value,
            PrimitiveTypeRecord<uint> primitive => primitive.Value,
            PrimitiveTypeRecord<long> primitive => primitive.Value,
            PrimitiveTypeRecord<ulong> primitive => primitive.Value,
            PrimitiveTypeRecord<float> primitive => primitive.Value,
            PrimitiveTypeRecord<double> primitive => primitive.Value,
            PrimitiveTypeRecord<decimal> primitive => primitive.Value,
            PrimitiveTypeRecord<TimeSpan> primitive => primitive.Value,
            PrimitiveTypeRecord<DateTime> primitive => primitive.Value,
            PrimitiveTypeRecord<IntPtr> primitive => primitive.Value,
            _ => ((PrimitiveTypeRecord<UIntPtr>)record).Value
        };
    }

    /// <summary>
    ///  Tries to get this object as a <see cref="List{T}"/> of <see cref="PrimitiveType"/>.
    /// </summary>
    public static bool TryGetPrimitiveList(this BinaryFormattedObject format, [NotNullWhen(true)] out object? list)
    {
        return TryGet(Get, format, out list);

        static bool Get(BinaryFormattedObject format, [NotNullWhen(true)] out object? list)
        {
            list = null;

            if (format.RootRecord is not ClassRecord classInfo
                || !classInfo.HasMember("_items")
                || !classInfo.HasMember("_size")
                || classInfo.GetRawValue("_size") is not int size
                || !classInfo.TypeName.IsConstructedGenericType
                || classInfo.TypeName.GetGenericTypeDefinition().Name != typeof(List<>).Name
                || classInfo.TypeName.GetGenericArguments().Length != 1
                || classInfo.GetRawValue("_items") is not ArrayRecord arrayRecord
                || !IsPrimitiveArrayRecord(arrayRecord))
            {
                return false;
            }

            // BinaryFormatter serializes the entire backing array, so we need to trim it down to the size of the list.
            list = arrayRecord switch
            {
                SZArrayRecord<string> ar => ar.GetArray().CreateTrimmedList(size),
                SZArrayRecord<bool> ar => ar.GetArray().CreateTrimmedList(size),
                SZArrayRecord<byte> ar => ar.GetArray().CreateTrimmedList(size),
                SZArrayRecord<sbyte> ar => ar.GetArray().CreateTrimmedList(size),
                SZArrayRecord<char> ar => ar.GetArray().CreateTrimmedList(size),
                SZArrayRecord<short> ar => ar.GetArray().CreateTrimmedList(size),
                SZArrayRecord<ushort> ar => ar.GetArray().CreateTrimmedList(size),
                SZArrayRecord<int> ar => ar.GetArray().CreateTrimmedList(size),
                SZArrayRecord<uint> ar => ar.GetArray().CreateTrimmedList(size),
                SZArrayRecord<long> ar => ar.GetArray().CreateTrimmedList(size),
                SZArrayRecord<ulong> ar => ar.GetArray().CreateTrimmedList(size),
                SZArrayRecord<float> ar => ar.GetArray().CreateTrimmedList(size),
                SZArrayRecord<double> ar => ar.GetArray().CreateTrimmedList(size),
                SZArrayRecord<decimal> ar => ar.GetArray().CreateTrimmedList(size),
                SZArrayRecord<TimeSpan> ar => ar.GetArray().CreateTrimmedList(size),
                SZArrayRecord<DateTime> ar => ar.GetArray().CreateTrimmedList(size),
                _ => throw new InvalidOperationException()
            };

            return true;
        }
    }

    /// <summary>
    ///  Tries to get this object as a <see cref="ArrayList"/> of <see cref="PrimitiveType"/> values.
    /// </summary>
    public static bool TryGetPrimitiveArrayList(this BinaryFormattedObject format, [NotNullWhen(true)] out object? value)
    {
        return TryGet(Get, format, out value);

        static bool Get(BinaryFormattedObject format, [NotNullWhen(true)] out object? value)
        {
            value = null;

            if (format.RootRecord is not ClassRecord classInfo
                || !classInfo.TypeNameMatches(typeof(ArrayList))
                || !classInfo.HasMember("_items")
                || !classInfo.HasMember("_size")
                || classInfo.GetRawValue("_size") is not int size
                || classInfo.GetRawValue("_items") is not SZArrayRecord<object> arrayRecord
                || size > arrayRecord.Length)
            {
                return false;
            }

            ArrayList arrayList = new(size);
            object?[] array = arrayRecord.GetArray();
            for (int i = 0; i < size; i++)
            {
                if (array[i] is SerializationRecord)
                {
                    return false;
                }

                arrayList.Add(array[i]);
            }

            value = arrayList;
            return true;
        }
    }

    /// <summary>
    ///  Tries to get this object as an <see cref="Array"/> of primitive types.
    /// </summary>
    public static bool TryGetPrimitiveArray(this BinaryFormattedObject format, [NotNullWhen(true)] out object? value)
    {
        return TryGet(Get, format, out value);

        static bool Get(BinaryFormattedObject format, [NotNullWhen(true)] out object? value)
        {
            if (!IsPrimitiveArrayRecord(format.RootRecord))
            {
                value = null;
                return false;
            }

            value = format.RootRecord switch
            {
                SZArrayRecord<string> ar => ar.GetArray(),
                SZArrayRecord<bool> ar => ar.GetArray(),
                SZArrayRecord<byte> ar => ar.GetArray(),
                SZArrayRecord<sbyte> ar => ar.GetArray(),
                SZArrayRecord<char> ar => ar.GetArray(),
                SZArrayRecord<short> ar => ar.GetArray(),
                SZArrayRecord<ushort> ar => ar.GetArray(),
                SZArrayRecord<int> ar => ar.GetArray(),
                SZArrayRecord<uint> ar => ar.GetArray(),
                SZArrayRecord<long> ar => ar.GetArray(),
                SZArrayRecord<ulong> ar => ar.GetArray(),
                SZArrayRecord<float> ar => ar.GetArray(),
                SZArrayRecord<double> ar => ar.GetArray(),
                SZArrayRecord<decimal> ar => ar.GetArray(),
                SZArrayRecord<TimeSpan> ar => ar.GetArray(),
                SZArrayRecord<DateTime> ar => ar.GetArray(),
                _ => throw new InvalidOperationException()
            };

            return value is not null;
        }
    }

    /// <summary>
    ///  Tries to get this object as a binary formatted <see cref="Hashtable"/> of <see cref="PrimitiveType"/> keys and values.
    /// </summary>
    public static bool TryGetPrimitiveHashtable(this BinaryFormattedObject format, [NotNullWhen(true)] out Hashtable? hashtable)
    {
        bool success = format.TryGetPrimitiveHashtable(out object? value);
        hashtable = (Hashtable?)value;
        return success;
    }

    /// <summary>
    ///  Tries to get this object as a binary formatted <see cref="Hashtable"/> of <see cref="PrimitiveType"/> keys and values.
    /// </summary>
    public static bool TryGetPrimitiveHashtable(this BinaryFormattedObject format, [NotNullWhen(true)] out object? hashtable)
    {
        return TryGet(Get, format, out hashtable);

        static bool Get(BinaryFormattedObject format, [NotNullWhen(true)] out object? hashtable)
        {
            hashtable = null;

            if (format.RootRecord.RecordType != SerializationRecordType.SystemClassWithMembersAndTypes
                || format.RootRecord is not ClassRecord classInfo
                || !classInfo.TypeNameMatches(typeof(Hashtable))
                || !classInfo.HasMember("Keys")
                || !classInfo.HasMember("Values")
                // Note that hashtables with custom comparers and/or hash code providers will have non null Comparer
                || classInfo.GetSerializationRecord("Comparer") is not null
                || classInfo.GetSerializationRecord("Keys") is not SZArrayRecord<object?> keysRecord
                || classInfo.GetSerializationRecord("Values") is not SZArrayRecord<object?> valuesRecord
                || keysRecord.Length != valuesRecord.Length)
            {
                return false;
            }

            Hashtable temp = new(keysRecord.Length);
            object?[] keys = keysRecord.GetArray();
            object?[] values = valuesRecord.GetArray();
            for (int i = 0; i < keys.Length; i++)
            {
                object? key = keys[i];
                object? value = values[i];

                if (key is null or SerializationRecord || value is SerializationRecord)
                {
                    return false;
                }

                temp[key] = value;
            }

            hashtable = temp;
            return true;
        }
    }

    /// <summary>
    ///  Tries to get this object as a binary formatted <see cref="NotSupportedException"/>.
    /// </summary>
    public static bool TryGetNotSupportedException(
        this BinaryFormattedObject format,
        out object? exception)
    {
        return TryGet(Get, format, out exception);

        static bool Get(BinaryFormattedObject format, [NotNullWhen(true)] out object? exception)
        {
            exception = null;

            if (format.RootRecord is not ClassRecord classInfo
                || classInfo.TypeNameMatches(typeof(NotSupportedException)))
            {
                return false;
            }

            exception = new NotSupportedException(classInfo.GetString("Message"));
            return true;
        }
    }

    /// <summary>
    ///  Try to get a supported .NET type object (not WinForms).
    /// </summary>
    public static bool TryGetFrameworkObject(
        this BinaryFormattedObject format,
        [NotNullWhen(true)] out object? value)
        => format.TryGetPrimitiveType(out value)
            || format.TryGetPrimitiveList(out value)
            || format.TryGetPrimitiveArray(out value)
            || format.TryGetPrimitiveArrayList(out value)
            || format.TryGetPrimitiveHashtable(out value)
            || format.TryGetRectangleF(out value)
            || format.TryGetPointF(out value)
            || format.TryGetNotSupportedException(out value);

    private static bool IsPrimitiveArrayRecord(SerializationRecord serializationRecord)
        => serializationRecord.RecordType is SerializationRecordType.ArraySingleString or SerializationRecordType.ArraySinglePrimitive;
}

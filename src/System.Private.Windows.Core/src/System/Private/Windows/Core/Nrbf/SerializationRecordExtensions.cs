// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Formats.Nrbf;
using System.Private.Windows.Core.BinaryFormat;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;

namespace System.Windows.Forms.Nrbf;

internal static class SerializationRecordExtensions
{
    /// <summary>
    ///  Converts the given exception to a <see cref="SerializationException"/> if needed, nesting the original exception
    ///  and assigning the original stack trace.
    /// </summary>
    private static SerializationException ConvertToSerializationException(this Exception ex)
        => ex is SerializationException serializationException
            ? serializationException
            : (SerializationException)ExceptionDispatchInfo.SetRemoteStackTrace(
                new SerializationException(ex.Message, ex),
                ex.StackTrace ?? string.Empty);

    internal static SerializationRecord Decode(this Stream stream)
    {
        try
        {
            return NrbfDecoder.Decode(stream, leaveOpen: true);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidCastException or ArithmeticException or IOException)
        {
            // Make the exception easier to catch, but retain the original stack trace.
            throw ex.ConvertToSerializationException();
        }
        catch (TargetInvocationException ex)
        {
            throw ExceptionDispatchInfo.Capture(ex.InnerException!).SourceException.ConvertToSerializationException();
        }
    }

    internal static SerializationRecord Decode(this Stream stream, out IReadOnlyDictionary<SerializationRecordId, SerializationRecord> recordMap)
    {
        try
        {
            return NrbfDecoder.Decode(stream, out recordMap, leaveOpen: true);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidCastException or ArithmeticException or IOException)
        {
            // Make the exception easier to catch, but retain the original stack trace.
            throw ex.ConvertToSerializationException();
        }
        catch (TargetInvocationException ex)
        {
            throw ExceptionDispatchInfo.Capture(ex.InnerException!).SourceException.ConvertToSerializationException();
        }
    }

    /// <summary>
    ///  Deserializes the <see cref="SerializationRecord"/> to an object.
    /// </summary>
    [RequiresUnreferencedCode("Ultimately calls resolver for type names in the data.")]
    public static object? Deserialize(
        this SerializationRecord rootRecord,
        IReadOnlyDictionary<SerializationRecordId, SerializationRecord> recordMap,
        ITypeResolver typeResolver)
    {
        DeserializationOptions options = new()
        {
            TypeResolver = typeResolver
        };

        try
        {
            return Deserializer.Deserialize(rootRecord.Id, recordMap, options);
        }
        catch (SerializationException ex)
        {
            throw ExceptionDispatchInfo.SetRemoteStackTrace(
                new NotSupportedException(ex.Message, ex), ex.StackTrace ?? string.Empty);
        }
    }

    internal delegate bool TryGetDelegate(SerializationRecord record, [NotNullWhen(true)] out object? value);

    internal static bool TryGet(TryGetDelegate get, SerializationRecord record, [NotNullWhen(true)] out object? value)
    {
        try
        {
            return get(record, out value);
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
    ///  Tries to get this object as a <see cref="Point"/>.
    /// </summary>
    public static bool TryGetPoint(this SerializationRecord record, [NotNullWhen(true)] out object? value)
    {
        return TryGet(Get, record, out value);

        static bool Get(SerializationRecord record, [NotNullWhen(true)] out object? value)
        {
            value = default;

            if (record is not ClassRecord classInfo
                 || !classInfo.TypeNameMatches(typeof(Point))
                 || !classInfo.HasMember("x")
                 || !classInfo.HasMember("y"))
            {
                return false;
            }

            value = new Point(classInfo.GetInt32("x"), classInfo.GetInt32("y"));

            return true;
        }
    }

    /// <summary>
    ///  Tries to get this object as a <see cref="Size"/>.
    /// </summary>
    public static bool TryGetSize(this SerializationRecord record, [NotNullWhen(true)] out object? value)
    {
        return TryGet(Get, record, out value);

        static bool Get(SerializationRecord record, [NotNullWhen(true)] out object? value)
        {
            value = default;

            if (record is not ClassRecord classInfo
                || !classInfo.TypeNameMatches(typeof(Size))
                || !classInfo.HasMember("height")
                || !classInfo.HasMember("width"))
            {
                return false;
            }

            value = new Size(classInfo.GetInt32("width"), classInfo.GetInt32("height"));

            return true;
        }
    }

    /// <summary>
    ///  Tries to get this object as a <see cref="Rectangle"/>.
    /// </summary>
    public static bool TryGetRectangle(this SerializationRecord record, [NotNullWhen(true)] out object? value)
    {
        return TryGet(Get, record, out value);

        static bool Get(SerializationRecord record, [NotNullWhen(true)] out object? value)
        {
            value = default;

            if (record is not ClassRecord classInfo
                || !classInfo.TypeNameMatches(typeof(Rectangle))
                || !classInfo.HasMember("x")
                || !classInfo.HasMember("y")
                || !classInfo.HasMember("width")
                || !classInfo.HasMember("height"))
            {
                return false;
            }

            value = new Rectangle(
                classInfo.GetInt32("x"),
                classInfo.GetInt32("y"),
                classInfo.GetInt32("width"),
                classInfo.GetInt32("height"));

            return true;
        }
    }

    /// <summary>
    ///  Tries to get this object as a <see cref="PointF"/>.
    /// </summary>
    public static bool TryGetPointF(this SerializationRecord record, [NotNullWhen(true)] out object? value)
    {
        return TryGet(Get, record, out value);

        static bool Get(SerializationRecord record, [NotNullWhen(true)] out object? value)
        {
            value = default;

            if (record is not ClassRecord classInfo
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
    ///  Tries to get this object as a <see cref="SizeF"/>.
    /// </summary>
    public static bool TryGetSizeF(this SerializationRecord record, [NotNullWhen(true)] out object? value)
    {
        return TryGet(Get, record, out value);

        static bool Get(SerializationRecord record, [NotNullWhen(true)] out object? value)
        {
            value = default;

            if (record is not ClassRecord classInfo
                || !classInfo.TypeNameMatches(typeof(SizeF))
                || !classInfo.HasMember("height")
                || !classInfo.HasMember("width"))
            {
                return false;
            }

            value = new SizeF(classInfo.GetSingle("width"), classInfo.GetSingle("height"));

            return true;
        }
    }

    /// <summary>
    ///  Tries to get this object as a <see cref="RectangleF"/>.
    /// </summary>
    public static bool TryGetRectangleF(this SerializationRecord record, [NotNullWhen(true)] out object? value)
    {
        return TryGet(Get, record, out value);

        static bool Get(SerializationRecord record, [NotNullWhen(true)] out object? value)
        {
            value = default;

            if (record is not ClassRecord classInfo
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
    ///  Tries to get this object as a <see cref="Color"/>.
    /// </summary>
    public static bool TryGetColor(this SerializationRecord record, [NotNullWhen(true)] out object? value)
    {
        return TryGet(Get, record, out value);

        static bool Get(SerializationRecord record, [NotNullWhen(true)] out object? value)
        {
            value = default;

            if (record is not ClassRecord classInfo
                || !classInfo.TypeNameMatches(typeof(Color))
                || !classInfo.HasMember("name")
                || !classInfo.HasMember("value")
                || !classInfo.HasMember("knownColor")
                || !classInfo.HasMember("state"))
            {
                return false;
            }

            ConstructorInfo? ctor = typeof(Color).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic, [typeof(long), typeof(short), typeof(string), typeof(KnownColor)]);
            if (ctor is null)
            {
                return false;
            }

            value = ctor.Invoke([
                classInfo.GetInt64("value"),
                classInfo.GetInt16("state"),
                classInfo.GetString("name"),
                (KnownColor)Convert.ToInt32(classInfo.GetInt16("knownColor"))]);

            return true;
        }
    }

    /// <summary>
    ///  Tries to get this object as a primitive type or string.
    /// </summary>
    /// <returns><see langword="true"/> if this represented a primitive type or string.</returns>
    public static bool TryGetPrimitiveType(this SerializationRecord record, [NotNullWhen(true)] out object? value)
    {
        return TryGet(Get, record, out value);

        static bool Get(SerializationRecord record, [NotNullWhen(true)] out object? value)
        {
            if (record.RecordType is SerializationRecordType.BinaryObjectString)
            {
                value = ((PrimitiveTypeRecord<string>)record).Value;
                return true;
            }
            else if (record.RecordType is SerializationRecordType.MemberPrimitiveTyped)
            {
                value = GetMemberPrimitiveTypedValue(record);
                return true;
            }

            value = null;
            return false;
        }
    }

    internal static object GetMemberPrimitiveTypedValue(this SerializationRecord record)
    {
        Debug.Assert(record.RecordType is SerializationRecordType.MemberPrimitiveTyped);
        return ((PrimitiveTypeRecord)record).Value;
    }

    /// <summary>
    ///  Tries to get this object as a <see cref="List{T}"/> of <see cref="PrimitiveType"/>.
    /// </summary>
    public static bool TryGetPrimitiveList(this SerializationRecord record, [NotNullWhen(true)] out object? list)
    {
        return TryGet(Get, record, out list);

        static bool Get(SerializationRecord record, [NotNullWhen(true)] out object? list)
        {
            list = null;

            if (record is not ClassRecord classInfo
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
    public static bool TryGetPrimitiveArrayList(this SerializationRecord record, [NotNullWhen(true)] out object? value)
    {
        return TryGet(Get, record, out value);

        static bool Get(SerializationRecord record, [NotNullWhen(true)] out object? value)
        {
            value = null;

            if (record is not ClassRecord classInfo
                || !classInfo.TypeNameMatches(typeof(ArrayList))
                || !classInfo.HasMember("_items")
                || !classInfo.HasMember("_size")
                || classInfo.GetRawValue("_size") is not int size
                || classInfo.GetRawValue("_items") is not SZArrayRecord<SerializationRecord> arrayRecord
                || size > arrayRecord.Length)
            {
                return false;
            }

            ArrayList arrayList = new(size);
            SerializationRecord?[] array = arrayRecord.GetArray();
            for (int i = 0; i < size; i++)
            {
                SerializationRecord? elementRecord = array[i];
                if (elementRecord is null)
                {
                    arrayList.Add(null);
                }
                else if (elementRecord is PrimitiveTypeRecord primitiveTypeRecord)
                {
                    arrayList.Add(primitiveTypeRecord.Value);
                }
                else
                {
                    // It was a complex type (represented as a ClassRecord or an ArrayRecord)
                    return false;
                }
            }

            value = arrayList;
            return true;
        }
    }

    /// <summary>
    ///  Tries to get this object as an <see cref="Array"/> of primitive types.
    /// </summary>
    public static bool TryGetPrimitiveArray(this SerializationRecord record, [NotNullWhen(true)] out object? value)
    {
        return TryGet(Get, record, out value);

        static bool Get(SerializationRecord record, [NotNullWhen(true)] out object? value)
        {
            if (!IsPrimitiveArrayRecord(record))
            {
                value = null;
                return false;
            }

            value = record switch
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
    public static bool TryGetPrimitiveHashtable(this SerializationRecord record, [NotNullWhen(true)] out Hashtable? hashtable)
    {
        bool success = record.TryGetPrimitiveHashtable(out object? value);
        hashtable = (Hashtable?)value;
        return success;
    }

    /// <summary>
    ///  Tries to get this object as a binary formatted <see cref="Hashtable"/> of <see cref="PrimitiveType"/> keys and values.
    /// </summary>
    public static bool TryGetPrimitiveHashtable(this SerializationRecord record, [NotNullWhen(true)] out object? hashtable)
    {
        return TryGet(Get, record, out hashtable);

        static bool Get(SerializationRecord record, [NotNullWhen(true)] out object? hashtable)
        {
            hashtable = null;

            if (record.RecordType != SerializationRecordType.SystemClassWithMembersAndTypes
                || record is not ClassRecord classInfo
                || !classInfo.TypeNameMatches(typeof(Hashtable))
                || !classInfo.HasMember("Keys")
                || !classInfo.HasMember("Values")
                // Note that hashtables with custom comparers and/or hash code providers will have non null Comparer
                || classInfo.GetSerializationRecord("Comparer") is not null
                || classInfo.GetSerializationRecord("Keys") is not SZArrayRecord<SerializationRecord?> keysRecord
                || classInfo.GetSerializationRecord("Values") is not SZArrayRecord<SerializationRecord?> valuesRecord
                || keysRecord.Length != valuesRecord.Length)
            {
                return false;
            }

            Hashtable temp = new(keysRecord.Length);
            SerializationRecord?[] keys = keysRecord.GetArray();
            SerializationRecord?[] values = valuesRecord.GetArray();
            for (int i = 0; i < keys.Length; i++)
            {
                SerializationRecord? key = keys[i];
                SerializationRecord? value = values[i];

                if (key is null || key is not PrimitiveTypeRecord primitiveKey)
                {
                    return false;
                }

                if (value is null)
                {
                    temp[primitiveKey.Value] = null; // null values are allowed
                }
                else if (value is PrimitiveTypeRecord primitiveValue)
                {
                    temp[primitiveKey.Value] = primitiveValue.Value;
                }
                else
                {
                    // It was a complex type (represented as a ClassRecord or an ArrayRecord)
                    return false;
                }
            }

            hashtable = temp;
            return true;
        }
    }

    /// <summary>
    ///  Tries to get this object as a binary formatted <see cref="NotSupportedException"/>.
    /// </summary>
    public static bool TryGetNotSupportedException(
        this SerializationRecord record,
        out object? exception)
    {
        return TryGet(Get, record, out exception);

        static bool Get(SerializationRecord record, [NotNullWhen(true)] out object? exception)
        {
            exception = null;

            if (record is not ClassRecord classInfo
                || !classInfo.TypeNameMatches(typeof(NotSupportedException)))
            {
                return false;
            }

            exception = new NotSupportedException(classInfo.GetString("Message"));
            return true;
        }
    }

    /// <summary>
    ///  Try to get a supported .NET type object (not WinForms) that has no <see cref="TypeConverter"/>.
    /// </summary>
    public static bool TryGetFrameworkObject(
        this SerializationRecord record,
        [NotNullWhen(true)] out object? value)
        => record.TryGetPrimitiveType(out value)
            || record.TryGetPrimitiveList(out value)
            || record.TryGetPrimitiveArray(out value)
            || record.TryGetPrimitiveArrayList(out value)
            || record.TryGetPrimitiveHashtable(out value)
            || record.TryGetRectangleF(out value)
            || record.TryGetPointF(out value)
            || record.TryGetNotSupportedException(out value);

    /// <summary>
    ///  Try to get a supported System.Drawing.Primitives object that has a <see cref="TypeConverter"/>.
    ///  This method is used for Clipboard payload deserialization. ResX deserialization uses <see cref="TypeConverter"/>s for these types.
    /// </summary>
    public static bool TryGetDrawingPrimitivesObject(
        this SerializationRecord record,
        [NotNullWhen(true)] out object? value) =>
        record.TryGetPoint(out value)
            || record.TryGetSize(out value)
            || record.TryGetSizeF(out value)
            || record.TryGetRectangle(out value)
            || record.TryGetColor(out value);

    private static bool IsPrimitiveArrayRecord(SerializationRecord serializationRecord) =>
        serializationRecord.RecordType is SerializationRecordType.ArraySingleString or SerializationRecordType.ArraySinglePrimitive;
}

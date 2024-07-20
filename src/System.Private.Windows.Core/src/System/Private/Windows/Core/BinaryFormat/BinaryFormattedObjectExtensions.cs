// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace System.Private.Windows.Core.BinaryFormat;

internal static class BinaryFormattedObjectExtensions
{
    /// <summary>
    ///  Type names for <see cref="SystemClassWithMembersAndTypes"/> that are raw primitives.
    /// </summary>
    private static bool IsPrimitiveTypeClassName(ReadOnlySpan<char> typeName)
        => TypeInfo.GetPrimitiveType(typeName) switch
        {
            PrimitiveType.Boolean => true,
            PrimitiveType.Byte => true,
            PrimitiveType.Char => true,
            PrimitiveType.Double => true,
            PrimitiveType.Int32 => true,
            PrimitiveType.Int64 => true,
            PrimitiveType.SByte => true,
            PrimitiveType.Single => true,
            PrimitiveType.Int16 => true,
            PrimitiveType.UInt16 => true,
            PrimitiveType.UInt32 => true,
            PrimitiveType.UInt64 => true,
            _ => false,
        };

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

            if (format.RootRecord is not ClassWithMembersAndTypes classInfo
                || format[classInfo.LibraryId] is not BinaryLibrary binaryLibrary
                || binaryLibrary.LibraryName != TypeInfo.SystemDrawingAssemblyName
                || classInfo.Name != typeof(PointF).FullName
                || classInfo.MemberValues.Count != 2)
            {
                return false;
            }

            value = new PointF((float)classInfo["x"]!, (float)classInfo["y"]!);

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

            if (format.RootRecord is not ClassWithMembersAndTypes classInfo
                || format[classInfo.LibraryId] is not BinaryLibrary binaryLibrary
                || binaryLibrary.LibraryName != TypeInfo.SystemDrawingAssemblyName
                || classInfo.Name != typeof(RectangleF).FullName
                || classInfo.MemberValues.Count != 4)
            {
                return false;
            }

            value = new RectangleF(
                (float)classInfo["x"]!,
                (float)classInfo["y"]!,
                (float)classInfo["width"]!,
                (float)classInfo["height"]!);

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
            value = default;

            if (format.RootRecord is BinaryObjectString binaryString)
            {
                value = binaryString.Value;
                return true;
            }

            if (format.RootRecord is not SystemClassWithMembersAndTypes systemClass)
            {
                return false;
            }

            if (IsPrimitiveTypeClassName(systemClass.Name) && systemClass.MemberTypeInfo[0].Type == BinaryType.Primitive)
            {
                value = systemClass.MemberValues[0]!;
                return true;
            }

            if (systemClass.Name == typeof(TimeSpan).FullName)
            {
                value = new TimeSpan((long)systemClass.MemberValues[0]!);
                return true;
            }

            switch (systemClass.Name)
            {
                case TypeInfo.TimeSpanType:
                    value = new TimeSpan((long)systemClass.MemberValues[0]!);
                    return true;
                case TypeInfo.DateTimeType:
                    ulong ulongValue = (ulong)systemClass["dateData"]!;
                    value = Unsafe.As<ulong, DateTime>(ref ulongValue);
                    return true;
                case TypeInfo.DecimalType:
                    ReadOnlySpan<int> bits =
                    [
                        (int)systemClass["lo"]!,
                        (int)systemClass["mid"]!,
                        (int)systemClass["hi"]!,
                        (int)systemClass["flags"]!
                    ];

                    value = new decimal(bits);
                    return true;
                case TypeInfo.IntPtrType:
                    // Rehydrating still throws even though casting doesn't any more
                    value = checked((nint)(long)systemClass.MemberValues[0]!);
                    return true;
                case TypeInfo.UIntPtrType:
                    value = checked((nuint)(ulong)systemClass.MemberValues[0]!);
                    return true;
                default:
                    return false;
            }
        }
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

            const string ListTypeName = "System.Collections.Generic.List`1[[";

            if (format.RootRecord is not SystemClassWithMembersAndTypes classInfo
                || !classInfo.Name.StartsWith(ListTypeName, StringComparison.Ordinal)
                || classInfo["_items"] is not MemberReference reference
                || format[reference] is not ArrayRecord array)
            {
                return false;
            }

            int commaIndex = classInfo.Name.IndexOf(',');
            if (commaIndex == -1)
            {
                return false;
            }

            ReadOnlySpan<char> typeName = classInfo.Name.AsSpan()[ListTypeName.Length..commaIndex];
            PrimitiveType primitiveType = TypeInfo.GetPrimitiveType(typeName);

            int size;
            try
            {
                // Lists serialize the entire backing array.
                if ((size = (int)classInfo["_size"]!) > array.Length)
                {
                    return false;
                }
            }
            catch (KeyNotFoundException)
            {
                return false;
            }

            switch (primitiveType)
            {
                case default(PrimitiveType):
                    return false;
                case PrimitiveType.String:
                    if (array is ArraySingleString stringArray)
                    {
                        List<string?> stringList = new(size);
                        stringList.AddRange(stringArray);
                        list = stringList;
                        return true;
                    }

                    return false;
            }

            if (array is not IPrimitiveTypeRecord primitiveArray || primitiveArray.PrimitiveType != primitiveType)
            {
                return false;
            }

            // BinaryFormatter serializes the entire backing array, so we need to trim it down to the size of the list.
            list = primitiveType switch
            {
                PrimitiveType.Boolean => ((ArrayRecord<bool>)array).ArrayObjects.CreateTrimmedList(size),
                PrimitiveType.Byte => ((ArrayRecord<byte>)array).ArrayObjects.CreateTrimmedList(size),
                PrimitiveType.Char => ((ArrayRecord<char>)array).ArrayObjects.CreateTrimmedList(size),
                PrimitiveType.Decimal => ((ArrayRecord<decimal>)array).ArrayObjects.CreateTrimmedList(size),
                PrimitiveType.Double => ((ArrayRecord<double>)array).ArrayObjects.CreateTrimmedList(size),
                PrimitiveType.Int16 => ((ArrayRecord<short>)array).ArrayObjects.CreateTrimmedList(size),
                PrimitiveType.Int32 => ((ArrayRecord<int>)array).ArrayObjects.CreateTrimmedList(size),
                PrimitiveType.Int64 => ((ArrayRecord<long>)array).ArrayObjects.CreateTrimmedList(size),
                PrimitiveType.SByte => ((ArrayRecord<sbyte>)array).ArrayObjects.CreateTrimmedList(size),
                PrimitiveType.Single => ((ArrayRecord<float>)array).ArrayObjects.CreateTrimmedList(size),
                PrimitiveType.TimeSpan => ((ArrayRecord<TimeSpan>)array).ArrayObjects.CreateTrimmedList(size),
                PrimitiveType.DateTime => ((ArrayRecord<DateTime>)array).ArrayObjects.CreateTrimmedList(size),
                PrimitiveType.UInt16 => ((ArrayRecord<ushort>)array).ArrayObjects.CreateTrimmedList(size),
                PrimitiveType.UInt32 => ((ArrayRecord<uint>)array).ArrayObjects.CreateTrimmedList(size),
                PrimitiveType.UInt64 => ((ArrayRecord<ulong>)array).ArrayObjects.CreateTrimmedList(size),
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

            if (format.RootRecord is not SystemClassWithMembersAndTypes classInfo
                || classInfo.Name != typeof(ArrayList).FullName
                || format[2] is not ArraySingleObject array)
            {
                return false;
            }

            int size;
            try
            {
                // Lists serialize the entire backing array.
                if ((size = (int)classInfo["_size"]!) > array.Length)
                {
                    return false;
                }
            }
            catch (KeyNotFoundException)
            {
                return false;
            }

            ArrayList arrayList = new(size);
            for (int i = 0; i < size; i++)
            {
                if (!format.TryGetPrimitiveRecordValueOrNull((IRecord)array[i]!, out object? item))
                {
                    return false;
                }

                arrayList.Add(item);
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
            value = null;
            if (format.RootRecord is not ArrayRecord array)
            {
                return false;
            }

            if (array is ArraySingleString stringArray)
            {
                value = stringArray.ToArray();
                return true;
            }

            if (array is not IPrimitiveTypeRecord primitiveArray)
            {
                return false;
            }

            value = primitiveArray.PrimitiveType switch
            {
                PrimitiveType.Boolean => ((ArrayRecord<bool>)primitiveArray).ArrayObjects,
                PrimitiveType.Byte => ((ArrayRecord<byte>)primitiveArray).ArrayObjects,
                PrimitiveType.Char => ((ArrayRecord<char>)primitiveArray).ArrayObjects,
                PrimitiveType.Decimal => ((ArrayRecord<decimal>)primitiveArray).ArrayObjects,
                PrimitiveType.Double => ((ArrayRecord<double>)primitiveArray).ArrayObjects,
                PrimitiveType.Int16 => ((ArrayRecord<short>)primitiveArray).ArrayObjects,
                PrimitiveType.Int32 => ((ArrayRecord<int>)primitiveArray).ArrayObjects,
                PrimitiveType.Int64 => ((ArrayRecord<long>)primitiveArray).ArrayObjects,
                PrimitiveType.SByte => ((ArrayRecord<sbyte>)primitiveArray).ArrayObjects,
                PrimitiveType.Single => ((ArrayRecord<float>)primitiveArray).ArrayObjects,
                PrimitiveType.TimeSpan => ((ArrayRecord<TimeSpan>)primitiveArray).ArrayObjects,
                PrimitiveType.DateTime => ((ArrayRecord<DateTime>)primitiveArray).ArrayObjects,
                PrimitiveType.UInt16 => ((ArrayRecord<ushort>)primitiveArray).ArrayObjects,
                PrimitiveType.UInt32 => ((ArrayRecord<uint>)primitiveArray).ArrayObjects,
                PrimitiveType.UInt64 => ((ArrayRecord<ulong>)primitiveArray).ArrayObjects,
                _ => null
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

            // Note that hashtables with custom comparers and/or hash code providers will have that information before
            // the value pair arrays.
            if (format.RootRecord is not SystemClassWithMembersAndTypes classInfo
                || classInfo.Name != TypeInfo.HashtableType
                || format[2] is not ArraySingleObject keys
                || format[3] is not ArraySingleObject values
                || keys.Length != values.Length)
            {
                return false;
            }

            Hashtable temp = new(keys.Length);
            for (int i = 0; i < keys.Length; i++)
            {
                if (!format.TryGetPrimitiveRecordValue((IRecord?)keys[i], out object? key)
                    || !format.TryGetPrimitiveRecordValueOrNull((IRecord?)values[i], out object? value))
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
    ///  Tries to get the value for the given <paramref name="record"/> if it represents a <see cref="PrimitiveType"/>
    ///  that isn't <see cref="PrimitiveType.Null"/>.
    /// </summary>
    public static bool TryGetPrimitiveRecordValue(
        this BinaryFormattedObject format,
        IRecord? record,
        [NotNullWhen(true)] out object? value)
    {
        format.TryGetPrimitiveRecordValueOrNull(record, out value);
        return value is not null;
    }

    /// <summary>
    ///  Tries to get the value for the given <paramref name="record"/> if it represents a <see cref="PrimitiveType"/>.
    /// </summary>
    public static bool TryGetPrimitiveRecordValueOrNull(
        this BinaryFormattedObject format,
        IRecord? record,
        out object? value)
    {
        value = null;
        if (record is ObjectNull or null)
        {
            return true;
        }

        value = format.RecordMap.Dereference(record) switch
        {
            BinaryObjectString valueString => valueString.Value,
            MemberPrimitiveTyped primitive => primitive.Value,
            _ => null,
        };

        return value is not null;
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

            if (format.RootRecord is not SystemClassWithMembersAndTypes classInfo
                || classInfo.Name != TypeInfo.NotSupportedExceptionType)
            {
                return false;
            }

            exception = new NotSupportedException(classInfo["Message"]!.ToString());
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

    /// <summary>
    ///  Dereferences <see cref="MemberReference"/> records.
    /// </summary>
    public static IRecord Dereference(this IReadOnlyRecordMap recordMap, IRecord record) => record switch
    {
        MemberReference reference => recordMap[reference.IdRef],
        _ => record
    };

    internal static void Write(this IWritableRecord record, BinaryWriter writer) => record.Write(writer);
}

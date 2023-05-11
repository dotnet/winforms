// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Windows.Forms.BinaryFormat;

internal static class BinaryFormattedObjectExtensions
{
    private static readonly string[] s_systemPrimitiveTypeNames = new string[]
    {
        typeof(int).FullName!,
        typeof(long).FullName!,
        typeof(bool).FullName!,
        typeof(char).FullName!,
        typeof(float).FullName!,
        typeof(double).FullName!,
        typeof(sbyte).FullName!,
        typeof(byte).FullName!,
        typeof(short).FullName!,
        typeof(ushort).FullName!,
        typeof(uint).FullName!,
        typeof(ulong).FullName!,
    };

    /// <summary>
    ///  Trys to get this object as a primitive type or string.
    /// </summary>
    public static bool TryGetString(this BinaryFormattedObject format, [NotNullWhen(true)] out string? value)
    {
        if (format.RecordCount < 3 || format[1] is not BinaryObjectString binaryString)
        {
            value = null;
            return false;
        }

        value = binaryString.Value;
        return false;
    }

    /// <summary>
    ///  Trys to get this object as a primitive type or string.
    /// </summary>
    /// <returns><see langword="true"/> if this represented a primitive type or string.</returns>
    public static bool TryGetPrimitiveType(this BinaryFormattedObject format, [NotNullWhen(true)] out object? value)
    {
        value = null;
        if (format.RecordCount < 3)
        {
            return false;
        }

        if (format[1] is BinaryObjectString binaryString)
        {
            value = binaryString.Value;
            return true;
        }

        if (format[1] is not SystemClassWithMembersAndTypes systemClass)
        {
            return false;
        }

        if (s_systemPrimitiveTypeNames.Contains(systemClass.Name)
            && systemClass.MemberTypeInfo[0].Type == BinaryType.Primitive)
        {
            value = systemClass.MemberValues[0];
            return true;
        }

        if (systemClass.Name == typeof(TimeSpan).FullName)
        {
            value = new TimeSpan((long)systemClass.MemberValues[0]);
            return true;
        }

        if (systemClass.Name == typeof(DateTime).FullName)
        {
            ulong ulongValue = (ulong)systemClass["dateData"];
            value = Unsafe.As<ulong, DateTime>(ref ulongValue);
            return true;
        }

        if (systemClass.Name == typeof(nint).FullName)
        {
            // Rehydrating still throws even though casting doesn't any more
            value = checked((nint)(long)systemClass.MemberValues[0]);
            return true;
        }

        if (systemClass.Name == typeof(nuint).FullName)
        {
            value = checked((nuint)(ulong)systemClass.MemberValues[0]);
            return true;
        }

        // Handle decimal, nint, nuint, TimeSpan, DateTime
        if (systemClass.Name == typeof(decimal).FullName)
        {
            Span<int> bits = stackalloc int[4]
            {
                (int)systemClass["lo"],
                (int)systemClass["mid"],
                (int)systemClass["hi"],
                (int)systemClass["flags"]
            };

            value = new decimal(bits);
            return true;
        }

        return false;
    }

    public static bool TryGetPrimitiveList<T>(this BinaryFormattedObject format, [NotNullWhen(true)] out object? list)
        where T : unmanaged
    {
        bool success = format.TryGetPrimitiveList(out List<T>? tList);
        list = tList;
        return success;
    }

    /// <summary>
    ///  Reads a binary formatted primitive list.
    /// </summary>
    /// <exception cref="NotSupportedException"><typeparamref name="T"/> was not primitive.</exception>
    /// <exception cref="SerializationException">Data isn't a valid <see cref="List{T}"/>.</exception>
    public static bool TryGetPrimitiveList<T>(this BinaryFormattedObject format, [NotNullWhen(true)] out List<T>? list)
        where T : unmanaged
    {
        if (!typeof(T).IsPrimitive)
        {
            throw new NotSupportedException($"{nameof(T)} is not primitive.");
        }

        list = null;

        if (format.RecordCount != 4
            || format[1] is not SystemClassWithMembersAndTypes classInfo
            || !classInfo.Name.StartsWith($"System.Collections.Generic.List`1[[{typeof(T).FullName}", StringComparison.Ordinal)
            || format[2] is not ArraySinglePrimitive array
            || array.PrimitiveType != Record.GetPrimitiveType(typeof(T)))
        {
            return false;
        }

        int count;
        try
        {
            count = (int)classInfo["_size"];
        }
        catch (Exception ex)
        {
            throw ex.ConvertToSerializationException();
        }

        List<T> newList = new(count);
        newList.AddRange(array.Take(count).Cast<T>());
        list = newList;
        return true;
    }

    /// <summary>
    ///  Gets a binary formatted <see cref="Hashtable"/>. Only accepts <see langword="string"/> keys
    ///  and <see langword="string"/>, <see langword="null"/>, or primitive values.
    /// </summary>
    /// <exception cref="SerializationException">
    ///  Data isn't a valid primitive only <see cref="Hashtable"/>.
    /// </exception>
    public static bool TryGetPrimitiveHashtable(this BinaryFormattedObject format, [NotNullWhen(true)] out Hashtable? hashtable)
    {
        hashtable = null;

        if (format.RecordCount < 5
            || format[1] is not SystemClassWithMembersAndTypes classInfo
            || classInfo.Name != "System.Collections.Hashtable"
            || format[2] is not ArraySingleObject keys
            || format[3] is not ArraySingleObject values
            || keys.Length != values.Length)
        {
            return false;
        }

        hashtable = new(keys.Length);
        for (int i = 0; i < keys.Length; i++)
        {
            object key = keys[i] switch
            {
                BinaryObjectString keyString => keyString.Value,
                MemberReference reference => format[reference.IdRef] switch
                {
                    BinaryObjectString refString => refString.Value,
                    _ => throw new SerializationException()
                },
                MemberPrimitiveTyped primitive => primitive.Value,
                _ => throw new SerializationException(),
            };

            hashtable[key] = values[i] switch
            {
                BinaryObjectString valueString => valueString.Value,
                ObjectNull => null,
                MemberReference reference => format[reference.IdRef] switch
                {
                    BinaryObjectString refString => refString.Value,
                    _ => throw new SerializationException()
                },
                MemberPrimitiveTyped primitive => primitive.Value,
                _ => throw new SerializationException(),
            };
        }

        return true;
    }
}

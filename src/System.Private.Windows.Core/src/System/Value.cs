// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.CompilerServices;

namespace System;

/// <summary>
///  A struct that can hold any value type or reference type without boxing primitive types or enums. Behavior matches
///  casting to/from <see langword="object"/>.
/// </summary>
/// <devdoc>
///  Everything in this struct is designed to be as fast as possible. Changing logic should not be done without
///  detailed performance measurements.
/// </devdoc>
internal readonly partial struct Value
{
    // Do not add more fields to this struct. It is important that it stays 16 bytes in size for maximum efficiency.

    private readonly Union _union;
    private readonly object? _object;

    /// <summary>
    ///  Creates a new <see cref="Value"/> with the given <see langword="object"/>. To avoid boxing enums, use the
    ///  <see cref="Create{T}(T)"/> method instead.
    /// </summary>
    public Value(object? value)
    {
        _object = value;
        _union = default;
    }

    /// <summary>
    ///  The <see cref="System.Type"/> of the value stored in this <see cref="Value"/>.
    /// </summary>
    public readonly Type? Type
    {
        get
        {
            Type? type;
            if (_object is null)
            {
                type = null;
            }
            else if (_object is TypeFlag typeFlag)
            {
                type = typeFlag.Type;
            }
            else
            {
                type = _object.GetType();

                if (_union.UInt64 != 0)
                {
                    Debug.Assert(type.IsArray);

                    // We have an ArraySegment
                    if (type == typeof(byte[]))
                    {
                        type = typeof(ArraySegment<byte>);
                    }
                    else if (type == typeof(char[]))
                    {
                        type = typeof(ArraySegment<char>);
                    }
                    else
                    {
                        Debug.Fail($"Unexpected type {type.Name}.");
                    }
                }
            }

            return type;
        }
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowInvalidCast(Type? from, Type to) =>
        throw new InvalidCastException($"{from?.Name ?? "<null>"} cannot be cast to {to.Name}");

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowInvalidOperation() => throw new InvalidOperationException();

    #region Byte
    public Value(byte value)
    {
        _object = TypeFlags.Byte;
        _union.Byte = value;
    }

    public Value(byte? value)
    {
        if (value.HasValue)
        {
            _object = TypeFlags.Byte;
            _union.Byte = value.Value;
        }
        else
        {
            _object = null;
        }
    }

    public static implicit operator Value(byte value) => new(value);
    public static explicit operator byte(in Value value) => value.GetValue<byte>();
    public static implicit operator Value(byte? value) => new(value);
    public static explicit operator byte?(in Value value) => value.GetValue<byte?>();
    #endregion

    #region SByte
    public Value(sbyte value)
    {
        _object = TypeFlags.SByte;
        _union.SByte = value;
    }

    public Value(sbyte? value)
    {
        if (value.HasValue)
        {
            _object = TypeFlags.SByte;
            _union.SByte = value.Value;
        }
        else
        {
            _object = null;
        }
    }

    public static implicit operator Value(sbyte value) => new(value);
    public static explicit operator sbyte(in Value value) => value.GetValue<sbyte>();
    public static implicit operator Value(sbyte? value) => new(value);
    public static explicit operator sbyte?(in Value value) => value.GetValue<sbyte?>();
    #endregion

    #region Boolean
    public Value(bool value)
    {
        _object = TypeFlags.Boolean;
        _union.Boolean = value;
    }

    public Value(bool? value)
    {
        if (value.HasValue)
        {
            _object = TypeFlags.Boolean;
            _union.Boolean = value.Value;
        }
        else
        {
            _object = null;
        }
    }

    public static implicit operator Value(bool value) => new(value);
    public static explicit operator bool(in Value value) => value.GetValue<bool>();
    public static implicit operator Value(bool? value) => new(value);
    public static explicit operator bool?(in Value value) => value.GetValue<bool?>();
    #endregion

    #region Char
    public Value(char value)
    {
        _object = TypeFlags.Char;
        _union.Char = value;
    }

    public Value(char? value)
    {
        if (value.HasValue)
        {
            _object = TypeFlags.Char;
            _union.Char = value.Value;
        }
        else
        {
            _object = null;
        }
    }

    public static implicit operator Value(char value) => new(value);
    public static explicit operator char(in Value value) => value.GetValue<char>();
    public static implicit operator Value(char? value) => new(value);
    public static explicit operator char?(in Value value) => value.GetValue<char?>();
    #endregion

    #region Int16
    public Value(short value)
    {
        _object = TypeFlags.Int16;
        _union.Int16 = value;
    }

    public Value(short? value)
    {
        if (value.HasValue)
        {
            _object = TypeFlags.Int16;
            _union.Int16 = value.Value;
        }
        else
        {
            _object = null;
        }
    }

    public static implicit operator Value(short value) => new(value);
    public static explicit operator short(in Value value) => value.GetValue<short>();
    public static implicit operator Value(short? value) => new(value);
    public static explicit operator short?(in Value value) => value.GetValue<short?>();
    #endregion

    #region Int32
    public Value(int value)
    {
        _object = TypeFlags.Int32;
        _union.Int32 = value;
    }

    public Value(int? value)
    {
        if (value.HasValue)
        {
            _object = TypeFlags.Int32;
            _union.Int32 = value.Value;
        }
        else
        {
            _object = null;
        }
    }

    public static implicit operator Value(int value) => new(value);
    public static explicit operator int(in Value value) => value.GetValue<int>();
    public static implicit operator Value(int? value) => new(value);
    public static explicit operator int?(in Value value) => value.GetValue<int?>();
    #endregion

    #region Int64
    public Value(long value)
    {
        _object = TypeFlags.Int64;
        _union.Int64 = value;
    }

    public Value(long? value)
    {
        if (value.HasValue)
        {
            _object = TypeFlags.Int64;
            _union.Int64 = value.Value;
        }
        else
        {
            _object = null;
        }
    }

    public static implicit operator Value(long value) => new(value);
    public static explicit operator long(in Value value) => value.GetValue<long>();
    public static implicit operator Value(long? value) => new(value);
    public static explicit operator long?(in Value value) => value.GetValue<long?>();
    #endregion

    #region UInt16
    public Value(ushort value)
    {
        _object = TypeFlags.UInt16;
        _union.UInt16 = value;
    }

    public Value(ushort? value)
    {
        if (value.HasValue)
        {
            _object = TypeFlags.UInt16;
            _union.UInt16 = value.Value;
        }
        else
        {
            _object = null;
        }
    }

    public static implicit operator Value(ushort value) => new(value);
    public static explicit operator ushort(in Value value) => value.GetValue<ushort>();
    public static implicit operator Value(ushort? value) => new(value);
    public static explicit operator ushort?(in Value value) => value.GetValue<ushort?>();
    #endregion

    #region UInt32
    public Value(uint value)
    {
        _object = TypeFlags.UInt32;
        _union.UInt32 = value;
    }

    public Value(uint? value)
    {
        if (value.HasValue)
        {
            _object = TypeFlags.UInt32;
            _union.UInt32 = value.Value;
        }
        else
        {
            _object = null;
        }
    }

    public static implicit operator Value(uint value) => new(value);
    public static explicit operator uint(in Value value) => value.GetValue<uint>();
    public static implicit operator Value(uint? value) => new(value);
    public static explicit operator uint?(in Value value) => value.GetValue<uint?>();
    #endregion

    #region UInt64
    public Value(ulong value)
    {
        _object = TypeFlags.UInt64;
        _union.UInt64 = value;
    }

    public Value(ulong? value)
    {
        if (value.HasValue)
        {
            _object = TypeFlags.UInt64;
            _union.UInt64 = value.Value;
        }
        else
        {
            _object = null;
        }
    }

    public static implicit operator Value(ulong value) => new(value);
    public static explicit operator ulong(in Value value) => value.GetValue<ulong>();
    public static implicit operator Value(ulong? value) => new(value);
    public static explicit operator ulong?(in Value value) => value.GetValue<ulong?>();
    #endregion

    #region Single
    public Value(float value)
    {
        _object = TypeFlags.Single;
        _union.Single = value;
    }

    public Value(float? value)
    {
        if (value.HasValue)
        {
            _object = TypeFlags.Single;
            _union.Single = value.Value;
        }
        else
        {
            _object = null;
        }
    }

    public static implicit operator Value(float value) => new(value);
    public static explicit operator float(in Value value) => value.GetValue<float>();
    public static implicit operator Value(float? value) => new(value);
    public static explicit operator float?(in Value value) => value.GetValue<float?>();
    #endregion

    #region Double
    public Value(double value)
    {
        _object = TypeFlags.Double;
        _union.Double = value;
    }

    public Value(double? value)
    {
        if (value.HasValue)
        {
            _object = TypeFlags.Double;
            _union.Double = value.Value;
        }
        else
        {
            _object = null;
        }
    }

    public static implicit operator Value(double value) => new(value);
    public static explicit operator double(in Value value) => value.GetValue<double>();
    public static implicit operator Value(double? value) => new(value);
    public static explicit operator double?(in Value value) => value.GetValue<double?>();
    #endregion

    #region Size
    public Value(Size value)
    {
        _object = TypeFlags.Size;
        _union.Size = value;
    }

    public Value(Size? value)
    {
        if (value.HasValue)
        {
            _object = TypeFlags.Size;
            _union.Size = value.Value;
        }
        else
        {
            _object = null;
        }
    }

    public static implicit operator Value(Size value) => new(value);
    public static explicit operator Size(in Value value) => value.GetValue<Size>();
    public static implicit operator Value(Size? value) => new(value);
    public static explicit operator Size?(in Value value) => value.GetValue<Size?>();
    #endregion

    #region Point
    public Value(Point value)
    {
        _object = TypeFlags.Point;
        _union.Point = value;
    }

    public Value(Point? value)
    {
        if (value.HasValue)
        {
            _object = TypeFlags.Point;
            _union.Point = value.Value;
        }
        else
        {
            _object = null;
        }
    }

    public static implicit operator Value(Point value) => new(value);
    public static explicit operator Point(in Value value) => value.GetValue<Point>();
    public static implicit operator Value(Point? value) => new(value);
    public static explicit operator Point?(in Value value) => value.GetValue<Point?>();
    #endregion

    #region Color
    public Value(Color value)
    {
        if (PackedColor.TryCreate(value, out PackedColor packed))
        {
            _object = TypeFlags.PackedColor;
            _union.PackedColor = packed;
        }
        else
        {
            // Named colors can't be packed, so we have to box them.
            _object = value;
        }
    }

    public Value(Color? value)
    {
        if (!value.HasValue)
        {
            _object = null;
        }
        else
        {
            this = new(value.Value);
        }
    }

    public static implicit operator Value(Color value) => new(value);
    public static explicit operator Color(in Value value) => value.GetValue<Color>();
    public static implicit operator Value(Color? value) => new(value);
    public static explicit operator Color?(in Value value) => value.GetValue<Color?>();
    #endregion

    #region DateTimeOffset
    public Value(DateTimeOffset value)
    {
        TimeSpan offset = value.Offset;
        if (offset.Ticks == 0)
        {
            // This is a UTC time
            _union.Ticks = value.Ticks;
            _object = TypeFlags.UtcDateTimeOffset;
        }
        else if (PackedDateTimeOffset.TryCreate(value, offset, out PackedDateTimeOffset packed))
        {
            _union.PackedDateTimeOffset = packed;
            _object = TypeFlags.PackedDateTimeOffset;
        }
        else
        {
            _object = value;
        }
    }

    public Value(DateTimeOffset? value)
    {
        if (!value.HasValue)
        {
            _object = null;
        }
        else
        {
            this = new(value.Value);
        }
    }

    public static implicit operator Value(DateTimeOffset value) => new(value);
    public static explicit operator DateTimeOffset(in Value value) => value.GetValue<DateTimeOffset>();
    public static implicit operator Value(DateTimeOffset? value) => new(value);
    public static explicit operator DateTimeOffset?(in Value value) => value.GetValue<DateTimeOffset?>();
    #endregion

    #region DateTime
    public Value(DateTime value)
    {
        _union.DateTime = value;
        _object = TypeFlags.DateTime;
    }

    public Value(DateTime? value)
    {
        if (value.HasValue)
        {
            _object = TypeFlags.DateTime;
            _union.DateTime = value.Value;
        }
        else
        {
            _object = value;
        }
    }

    public static implicit operator Value(DateTime value) => new(value);
    public static explicit operator DateTime(in Value value) => value.GetValue<DateTime>();
    public static implicit operator Value(DateTime? value) => new(value);
    public static explicit operator DateTime?(in Value value) => value.GetValue<DateTime?>();
    #endregion

    #region ArraySegment
    public Value(ArraySegment<byte> segment)
    {
        byte[]? array = segment.Array;
        ArgumentNullException.ThrowIfNull(array, nameof(segment));

        _object = array;
        if (segment.Offset == 0 && segment.Count == 0)
        {
            _union.UInt64 = ulong.MaxValue;
        }
        else
        {
            _union.Segment = (segment.Offset, segment.Count);
        }
    }

    public static implicit operator Value(ArraySegment<byte> value) => new(value);
    public static explicit operator ArraySegment<byte>(in Value value) => value.GetValue<ArraySegment<byte>>();

    public Value(ArraySegment<char> segment)
    {
        char[]? array = segment.Array;
        ArgumentNullException.ThrowIfNull(array, nameof(segment));

        _object = array;
        if (segment.Offset == 0 && segment.Count == 0)
        {
            _union.UInt64 = ulong.MaxValue;
        }
        else
        {
            _union.Segment = (segment.Offset, segment.Count);
        }
    }

    public static implicit operator Value(ArraySegment<char> value) => new(value);
    public static explicit operator ArraySegment<char>(in Value value) => value.GetValue<ArraySegment<char>>();
    #endregion

    #region Decimal
    public static implicit operator Value(decimal value) => new(value);
    public static explicit operator decimal(in Value value) => value.GetValue<decimal>();
    public static implicit operator Value(decimal? value) => value.HasValue ? new(value.Value) : new(value);
    public static explicit operator decimal?(in Value value) => value.GetValue<decimal?>();
    #endregion

    #region T

    /// <summary>
    ///  Creates a new <see cref="Value"/> with the given value. This method can always be used and avoids boxing enums.
    /// </summary>
    public static Value Create<T>(T value)
    {
        // Explicit cast for types we don't box
        if (typeof(T) == typeof(bool))
            return new(Unsafe.As<T, bool>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(byte))
            return new(Unsafe.As<T, byte>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(sbyte))
            return new(Unsafe.As<T, sbyte>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(char))
            return new(Unsafe.As<T, char>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(short))
            return new(Unsafe.As<T, short>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(int))
            return new(Unsafe.As<T, int>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(long))
            return new(Unsafe.As<T, long>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(ushort))
            return new(Unsafe.As<T, ushort>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(uint))
            return new(Unsafe.As<T, uint>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(ulong))
            return new(Unsafe.As<T, ulong>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(float))
            return new(Unsafe.As<T, float>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(double))
            return new(Unsafe.As<T, double>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(DateTime))
            return new(Unsafe.As<T, DateTime>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(DateTimeOffset))
            return new(Unsafe.As<T, DateTimeOffset>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(Color))
            return new(Unsafe.As<T, Color>(ref Unsafe.AsRef(in value)));

        if (typeof(T) == typeof(bool?))
            return new(Unsafe.As<T, bool?>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(byte?))
            return new(Unsafe.As<T, byte?>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(sbyte?))
            return new(Unsafe.As<T, sbyte?>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(char?))
            return new(Unsafe.As<T, char?>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(short?))
            return new(Unsafe.As<T, short?>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(int?))
            return new(Unsafe.As<T, int?>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(long?))
            return new(Unsafe.As<T, long?>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(ushort?))
            return new(Unsafe.As<T, ushort?>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(uint?))
            return new(Unsafe.As<T, uint?>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(ulong?))
            return new(Unsafe.As<T, ulong?>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(float?))
            return new(Unsafe.As<T, float?>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(double?))
            return new(Unsafe.As<T, double?>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(DateTime?))
            return new(Unsafe.As<T, DateTime?>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(DateTimeOffset?))
            return new(Unsafe.As<T, DateTimeOffset?>(ref Unsafe.AsRef(in value)));

        if (typeof(T) == typeof(ArraySegment<byte>))
            return new(Unsafe.As<T, ArraySegment<byte>>(ref Unsafe.AsRef(in value)));
        if (typeof(T) == typeof(ArraySegment<char>))
            return new(Unsafe.As<T, ArraySegment<char>>(ref Unsafe.AsRef(in value)));

        if (typeof(T).IsEnum)
        {
            Debug.Assert(Unsafe.SizeOf<T>() <= sizeof(ulong));
            return new Value(StraightCastFlag<T>.Instance, Unsafe.As<T, ulong>(ref value));
        }

        return new Value(value);
    }

    [SkipLocalsInit]
    private Value(object o, ulong u)
    {
        Unsafe.SkipInit(out _union);
        _object = o;
        _union.UInt64 = u;
    }

    /// <summary>
    ///  Tries to get the value stored in this <see cref="Value"/> as the given type. Returns <see langword="true"/> if
    ///  the type matches.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   All types can be requested as <see langword="object"/>. Primitive types can be requested as their own type or
    ///   as a nullable of that type. Enums can be requested as their own type or a nullable of that type.
    ///  </para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly unsafe bool TryGetValue<T>(out T value)
    {
        bool success;

        // Checking the type gets all of the non-relevant compares elided by the JIT
        if (_object is not null && ((typeof(T) == typeof(bool) && _object == TypeFlags.Boolean)
            || (typeof(T) == typeof(byte) && _object == TypeFlags.Byte)
            || (typeof(T) == typeof(char) && _object == TypeFlags.Char)
            || (typeof(T) == typeof(double) && _object == TypeFlags.Double)
            || (typeof(T) == typeof(short) && _object == TypeFlags.Int16)
            || (typeof(T) == typeof(int) && _object == TypeFlags.Int32)
            || (typeof(T) == typeof(long) && _object == TypeFlags.Int64)
            || (typeof(T) == typeof(sbyte) && _object == TypeFlags.SByte)
            || (typeof(T) == typeof(float) && _object == TypeFlags.Single)
            || (typeof(T) == typeof(ushort) && _object == TypeFlags.UInt16)
            || (typeof(T) == typeof(uint) && _object == TypeFlags.UInt32)
            || (typeof(T) == typeof(ulong) && _object == TypeFlags.UInt64)
            || (typeof(T) == typeof(Size) && _object == TypeFlags.Size)
            || (typeof(T) == typeof(Point) && _object == TypeFlags.Point)))
        {
            value = Unsafe.As<Union, T>(ref Unsafe.AsRef(in _union));
            success = true;
        }
        else if (typeof(T) == typeof(Color) && _object == TypeFlags.PackedColor)
        {
            Color color = _union.PackedColor.Extract();
            value = Unsafe.As<Color, T>(ref Unsafe.AsRef(in color));
            success = true;
        }
        else if (typeof(T) == typeof(DateTime) && _object == TypeFlags.DateTime)
        {
            value = Unsafe.As<DateTime, T>(ref Unsafe.AsRef(in _union.DateTime));
            success = true;
        }
        else if (typeof(T) == typeof(DateTimeOffset) && _object == TypeFlags.UtcDateTimeOffset)
        {
            DateTimeOffset dto = new(_union.Ticks, TimeSpan.Zero);
            value = Unsafe.As<DateTimeOffset, T>(ref Unsafe.AsRef(in dto));
            success = true;
        }
        else if (typeof(T) == typeof(DateTimeOffset) && _object == TypeFlags.PackedDateTimeOffset)
        {
            DateTimeOffset dto = _union.PackedDateTimeOffset.Extract();
            value = Unsafe.As<DateTimeOffset, T>(ref Unsafe.AsRef(in dto));
            success = true;
        }
        else if (typeof(T).IsValueType)
        {
            success = TryGetValueSlow(out value);
        }
        else
        {
            success = TryGetObjectSlow(out value);
        }

        return success;
    }

    private readonly bool TryGetValueSlow<T>(out T value)
    {
        // Single return has a significant performance benefit.

        bool result = false;

        if (_object is null)
        {
            // A null is stored, it can only be assigned to a reference type or nullable.
            value = default!;
            result = Nullable.GetUnderlyingType(typeof(T)) is not null;
        }
        else if (typeof(T).IsEnum && _object is TypeFlag<T> typeFlag)
        {
            value = typeFlag.To(in this);
            result = true;
        }
        else if (_object is T t)
        {
            value = t;
            result = true;
        }
        else if (typeof(T) == typeof(ArraySegment<byte>))
        {
            ulong bits = _union.UInt64;
            if (bits != 0 && _object is byte[] byteArray)
            {
                ArraySegment<byte> segment = bits != ulong.MaxValue
                    ? new(byteArray, _union.Segment.Offset, _union.Segment.Count)
                    : new(byteArray, 0, 0);
                value = Unsafe.As<ArraySegment<byte>, T>(ref segment);
                result = true;
            }
            else
            {
                value = default!;
            }
        }
        else if (typeof(T) == typeof(ArraySegment<char>))
        {
            ulong bits = _union.UInt64;
            if (bits != 0 && _object is char[] charArray)
            {
                ArraySegment<char> segment = bits != ulong.MaxValue
                    ? new(charArray, _union.Segment.Offset, _union.Segment.Count)
                    : new(charArray, 0, 0);
                value = Unsafe.As<ArraySegment<char>, T>(ref segment);
                result = true;
            }
            else
            {
                value = default!;
            }
        }
        else if (typeof(T) == typeof(int?) && _object == TypeFlags.Int32)
        {
            int? @int = _union.Int32;
            value = Unsafe.As<int?, T>(ref Unsafe.AsRef(in @int));
            result = true;
        }
        else if (typeof(T) == typeof(long?) && _object == TypeFlags.Int64)
        {
            long? @long = _union.Int64;
            value = Unsafe.As<long?, T>(ref Unsafe.AsRef(in @long));
            result = true;
        }
        else if (typeof(T) == typeof(bool?) && _object == TypeFlags.Boolean)
        {
            bool? @bool = _union.Boolean;
            value = Unsafe.As<bool?, T>(ref Unsafe.AsRef(in @bool));
            result = true;
        }
        else if (typeof(T) == typeof(float?) && _object == TypeFlags.Single)
        {
            float? single = _union.Single;
            value = Unsafe.As<float?, T>(ref Unsafe.AsRef(in single));
            result = true;
        }
        else if (typeof(T) == typeof(double?) && _object == TypeFlags.Double)
        {
            double? @double = _union.Double;
            value = Unsafe.As<double?, T>(ref Unsafe.AsRef(in @double));
            result = true;
        }
        else if (typeof(T) == typeof(uint?) && _object == TypeFlags.UInt32)
        {
            uint? @uint = _union.UInt32;
            value = Unsafe.As<uint?, T>(ref Unsafe.AsRef(in @uint));
            result = true;
        }
        else if (typeof(T) == typeof(ulong?) && _object == TypeFlags.UInt64)
        {
            ulong? @ulong = _union.UInt64;
            value = Unsafe.As<ulong?, T>(ref Unsafe.AsRef(in @ulong));
            result = true;
        }
        else if (typeof(T) == typeof(char?) && _object == TypeFlags.Char)
        {
            char? @char = _union.Char;
            value = Unsafe.As<char?, T>(ref Unsafe.AsRef(in @char));
            result = true;
        }
        else if (typeof(T) == typeof(short?) && _object == TypeFlags.Int16)
        {
            short? @short = _union.Int16;
            value = Unsafe.As<short?, T>(ref Unsafe.AsRef(in @short));
            result = true;
        }
        else if (typeof(T) == typeof(ushort?) && _object == TypeFlags.UInt16)
        {
            ushort? @ushort = _union.UInt16;
            value = Unsafe.As<ushort?, T>(ref Unsafe.AsRef(in @ushort));
            result = true;
        }
        else if (typeof(T) == typeof(byte?) && _object == TypeFlags.Byte)
        {
            byte? @byte = _union.Byte;
            value = Unsafe.As<byte?, T>(ref Unsafe.AsRef(in @byte));
            result = true;
        }
        else if (typeof(T) == typeof(sbyte?) && _object == TypeFlags.SByte)
        {
            sbyte? @sbyte = _union.SByte;
            value = Unsafe.As<sbyte?, T>(ref Unsafe.AsRef(in @sbyte));
            result = true;
        }
        else if (typeof(T) == typeof(Color?) && _object == TypeFlags.PackedColor)
        {
            Color? color = _union.PackedColor.Extract();
            value = Unsafe.As<Color?, T>(ref Unsafe.AsRef(in color));
            result = true;
        }
        else if (typeof(T) == typeof(DateTime?) && _object == TypeFlags.DateTime)
        {
            DateTime? dateTime = _union.DateTime;
            value = Unsafe.As<DateTime?, T>(ref Unsafe.AsRef(in dateTime));
            result = true;
        }
        else if (typeof(T) == typeof(DateTimeOffset?) && _object == TypeFlags.UtcDateTimeOffset)
        {
            DateTimeOffset? dto = new DateTimeOffset(_union.Ticks, TimeSpan.Zero);
            value = Unsafe.As<DateTimeOffset?, T>(ref Unsafe.AsRef(in dto));
            result = true;
        }
        else if (typeof(T) == typeof(DateTimeOffset?) && _object == TypeFlags.PackedDateTimeOffset)
        {
            DateTimeOffset? dto = _union.PackedDateTimeOffset.Extract();
            value = Unsafe.As<DateTimeOffset?, T>(ref Unsafe.AsRef(in dto));
            result = true;
        }
        else if (Nullable.GetUnderlyingType(typeof(T)) is Type underlyingType
            && underlyingType.IsEnum
            && _object is TypeFlag underlyingTypeFlag
            && underlyingTypeFlag.Type == underlyingType)
        {
            // Asked for a nullable enum and we've got that type.

            // We've got multiple layouts, depending on the size of the enum backing field. We can't use the
            // nullable itself (e.g. default(T)) as a template as it gets treated specially by the runtime.

            int size = Unsafe.SizeOf<T>();

            switch (size)
            {
                case (2):
                    NullableTemplate<byte> byteTemplate = new(_union.Byte);
                    value = Unsafe.As<NullableTemplate<byte>, T>(ref Unsafe.AsRef(in byteTemplate));
                    result = true;
                    break;
                case (4):
                    NullableTemplate<ushort> ushortTemplate = new(_union.UInt16);
                    value = Unsafe.As<NullableTemplate<ushort>, T>(ref Unsafe.AsRef(in ushortTemplate));
                    result = true;
                    break;
                case (8):
                    NullableTemplate<uint> uintTemplate = new(_union.UInt32);
                    value = Unsafe.As<NullableTemplate<uint>, T>(ref Unsafe.AsRef(in uintTemplate));
                    result = true;
                    break;
                case (16):
                    NullableTemplate<ulong> ulongTemplate = new(_union.UInt64);
                    value = Unsafe.As<NullableTemplate<ulong>, T>(ref Unsafe.AsRef(in ulongTemplate));
                    result = true;
                    break;
                default:
                    ThrowInvalidOperation();
                    value = default!;
                    result = false;
                    break;
            }
        }
        else
        {
            value = default!;
            result = false;
        }

        return result;
    }

    private readonly bool TryGetObjectSlow<T>(out T value)
    {
        // Single return has a significant performance benefit.

        bool result = false;

        if (_object is null)
        {
            value = default!;
        }
        else if (typeof(T) == typeof(char[]))
        {
            if (_union.UInt64 == 0 && _object is char[])
            {
                value = (T)_object;
                result = true;
            }
            else
            {
                // Don't allow "implicit" cast to array if we stored a segment.
                value = default!;
                result = false;
            }
        }
        else if (typeof(T) == typeof(byte[]))
        {
            if (_union.UInt64 == 0 && _object is byte[])
            {
                value = (T)_object;
                result = true;
            }
            else
            {
                // Don't allow "implicit" cast to array if we stored a segment.
                value = default!;
                result = false;
            }
        }
        else if (typeof(T) == typeof(object))
        {
            // This case must also come before the _object is T case to make sure we don't leak our flags.
            if (_object is TypeFlag flag)
            {
                value = (T)flag.ToObject(this);
                result = true;
            }
            else if (_union.UInt64 != 0 && _object is char[] chars)
            {
                value = _union.UInt64 != ulong.MaxValue
                    ? (T)(object)new ArraySegment<char>(chars, _union.Segment.Offset, _union.Segment.Count)
                    : (T)(object)new ArraySegment<char>(chars, 0, 0);
                result = true;
            }
            else if (_union.UInt64 != 0 && _object is byte[] bytes)
            {
                value = _union.UInt64 != ulong.MaxValue
                    ? (T)(object)new ArraySegment<byte>(bytes, _union.Segment.Offset, _union.Segment.Count)
                    : (T)(object)new ArraySegment<byte>(bytes, 0, 0);
                result = true;
            }
            else
            {
                value = (T)_object;
                result = true;
            }
        }
        else if (_object is T t)
        {
            value = t;
            result = true;
        }
        else
        {
            value = default!;
            result = false;
        }

        return result;
    }

    /// <summary>
    ///  Gets the value as the specified <typeparamref name="T"/>.
    /// </summary>
    /// <exception cref="InvalidCastException">
    ///  The value is not of type <typeparamref name="T"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T GetValue<T>()
    {
        if (!TryGetValue(out T value))
        {
            ThrowInvalidCast(Type, typeof(T));
        }

        return value;
    }
    #endregion
}

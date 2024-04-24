// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Windows.Forms.BinaryFormat;

internal static class SerializationExtensions
{
    /// <summary>
    ///  Get a typed value. Hard casts.
    /// </summary>
    public static T? GetValue<T>(this SerializationInfo info, string name) => (T?)info.GetValue(name, typeof(T));

    /// <summary>
    ///  Converts the given exception to a <see cref="SerializationException"/> if needed, nesting the original exception
    ///  and assigning the original stack trace.
    /// </summary>
    public static SerializationException ConvertToSerializationException(this Exception ex)
        => ex is SerializationException serializationException
            ? serializationException
            : (SerializationException)ExceptionDispatchInfo.SetRemoteStackTrace(
                new SerializationException(ex.Message, ex),
                ex.StackTrace ?? string.Empty);

    /// <summary>
    ///  Gets a span over any array, including multi-dimensional arrays.
    /// </summary>
    public static Span<T> GetArrayData<T>(this Array array)
    {
        if (array.GetType().UnderlyingSystemType.IsAssignableFrom(typeof(T)))
        {
            throw new InvalidCastException($"Cannot cast array of type {array.GetType().UnderlyingSystemType} to {typeof(T)}.");
        }

        Span<T> data = MemoryMarshal.CreateSpan(ref Unsafe.As<byte, T>(
            ref MemoryMarshal.GetArrayDataReference(array)),
            checked((int)array.LongLength));

        return data;
    }
}

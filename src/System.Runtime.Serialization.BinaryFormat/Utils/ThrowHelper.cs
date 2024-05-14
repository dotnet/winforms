// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.Serialization.BinaryFormat;

internal static class ThrowHelper
{
    internal static void ThrowUnexpectedNullRecordCount()
        => throw new SerializationException("Unexpected Null Record count.");

    internal static Exception InvalidPrimitiveType(PrimitiveType primitiveType)
        => new SerializationException($"Invalid primitive type: {primitiveType}");

    internal static Exception InvalidBinaryType(BinaryType binaryType)
        => new SerializationException($"Invalid binary type: {binaryType}");

    internal static void ThrowMaxArrayLength(int limit, uint actual)
        => throw new SerializationException(
            $"The serialized array length ({actual}) was larger that the configured limit {limit}");

    internal static void ThrowArrayContainedNull()
        => throw new SerializationException("The array contained null(s)");

    internal static void ThrowTypeMismatch(Type expected)
        => throw new SerializationException($"The payload was expected to contain an instance of {expected}, while it contained TODO");

    internal static void ThrowEndOfStreamException()
        => throw new EndOfStreamException();
}

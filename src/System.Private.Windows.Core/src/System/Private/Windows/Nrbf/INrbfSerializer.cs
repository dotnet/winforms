// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Formats.Nrbf;
using System.Reflection.Metadata;

namespace System.Private.Windows.Nrbf;

internal interface INrbfSerializer
{
    /// <summary>
    ///  Tries to write supported objects to a stream.
    /// </summary>
    static abstract bool TryWriteObject(Stream stream, object value);

    /// <summary>
    ///  Tries to read supported objects from a <see cref="SerializationRecord"/>.
    /// </summary>
    static abstract bool TryGetObject(SerializationRecord record, [NotNullWhen(true)] out object? value);

    /// <summary>
    ///  Tries to bind the given type name to a supported type.
    /// </summary>
    static abstract bool TryBindToType(TypeName typeName, [NotNullWhen(true)] out Type? type);

    /// <summary>
    ///  Returns <see langword="true"/> if the type is supported by the serializer.
    /// </summary>
    static abstract bool IsSupportedType<T>();
}

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
    ///  Returns <see langword="true"/> if the type is fully supported by the serializer.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This should only return <see langword="true"/> for types that are expected to be safe to deserialize.
    ///  </para>
    ///  <para>
    ///   <see cref="TryBindToType(TypeName, out Type?)"/> and <see cref="TryGetObject(SerializationRecord, out object?)"/>
    ///   can handle some types partially.
    ///  </para>
    /// </remarks>
    /// <devdoc>
    ///  All types should be vetted to ensure that corrupted primitive data in the BinaryFormatter deserialization
    ///  process. There can be no possibility of cycles (direct or indirect self-reference) or ways to break object
    ///  state with bad primitive data (invalid enum values, etc). If the type has a serialization constructor, it
    ///  must be safe to call with any data. If the type does not have a serialization constructor, it must be safe
    ///  to populate all instance fields with any data.
    ///
    ///  We make no guarantees here about the safety of using the BinaryFormatter with these types. This is a best
    ///  effort to help avoid corruption exploitation.
    /// </devdoc>
    static abstract bool IsFullySupportedType(Type type);
}

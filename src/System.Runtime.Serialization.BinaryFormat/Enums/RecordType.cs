// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.Serialization.BinaryFormat;

/// <summary>
/// Record type.
/// </summary>
/// <remarks>
///  <para>
///   The enumeration does not contain all values supported by the <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/954a0657-b901-4813-9398-4ec732fe8b32">
///   [MS-NRBF] 2.1.2.1</see>, but only those supported by the <seealso cref="PayloadReader"/>.
///  </para>
/// </remarks>
public enum RecordType : byte
{
    SerializedStreamHeader,
    ClassWithId,
    SystemClassWithMembers,
    ClassWithMembers,
    SystemClassWithMembersAndTypes,
    ClassWithMembersAndTypes,
    BinaryObjectString,
    BinaryArray,
    MemberPrimitiveTyped,
    MemberReference,
    ObjectNull,
    MessageEnd,
    BinaryLibrary,
    ObjectNullMultiple256,
    ObjectNullMultiple,
    ArraySinglePrimitive,
    ArraySingleObject,
    ArraySingleString
}

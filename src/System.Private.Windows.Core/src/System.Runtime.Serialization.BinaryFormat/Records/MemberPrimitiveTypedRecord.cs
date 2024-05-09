﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.Serialization.BinaryFormat;

/// <summary>
///  Primitive value other than <see langword="string"/>.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/c0a190b2-762c-46b9-89f2-c7dabecfc084">
///    [MS-NRBF] 2.5.1
///   </see>
///  </para>
/// </remarks>
internal sealed class MemberPrimitiveTypedRecord<T> : PrimitiveTypeRecord<T>
    where T : unmanaged
{
    internal MemberPrimitiveTypedRecord(T value, bool pretend = false) : base(value)
    {
        RecordType = pretend ? RecordType.MemberPrimitiveTyped : RecordType.SystemClassWithMembersAndTypes;
    }

    public override RecordType RecordType { get; }
}

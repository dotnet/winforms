﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Array information structure.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/8fac763f-e46d-43a1-b360-80eb83d2c5fb">
///    [MS-NRBF] 2.4.2.1
///   </see>
///  </para>
/// </remarks>
internal readonly struct ArrayInfo : IBinaryWriteable
{
    public Id ObjectId { get; }
    public Count Length { get; }

    public ArrayInfo(Id objectId, Count length)
    {
        Length = length;
        ObjectId = objectId;
    }

    public static ArrayInfo Parse(BinaryReader reader, out Count length)
    {
        ArrayInfo arrayInfo = new(reader.ReadInt32(), reader.ReadInt32());
        length = arrayInfo.Length;
        return arrayInfo;
    }

    public readonly void Write(BinaryWriter writer)
    {
        writer.Write(ObjectId);
        writer.Write(Length);
    }
}

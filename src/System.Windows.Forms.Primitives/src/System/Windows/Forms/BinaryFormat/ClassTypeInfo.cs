// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Identifies a class by it's name and library id.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/844b24dd-9f82-426e-9b98-05334307a239">
///    [MS-NRBF] 2.1.1.8
///   </see>
///  </para>
/// </remarks>
internal readonly struct ClassTypeInfo : IBinaryWriteable
{
    public readonly string TypeName;
    public readonly Id LibraryId;

    public ClassTypeInfo(string typeName, Id libraryId)
    {
        TypeName = typeName;
        LibraryId = libraryId;
    }

    public static ClassTypeInfo Parse(BinaryReader reader) => new(
        reader.ReadLengthPrefixedString(),
        reader.ReadInt32());

    public void Write(BinaryWriter writer)
    {
        writer.WriteLengthPrefixedString(TypeName);
        writer.Write(LibraryId);
    }
}

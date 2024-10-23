// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Core.BinaryFormat.Serializer;

/// <summary>
///  Member type info.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/aa509b5a-620a-4592-a5d8-7e9613e0a03e">
///    [MS-NRBF] 2.3.1.2
///   </see>
///  </para>
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal readonly struct MemberTypeInfo
{
    internal BinaryType Type { get; init; }
    internal object? Info { get; init; }

    internal MemberTypeInfo(BinaryType type, object? info)
    {
        Type = type;
        Info = info;
    }

    private string DebuggerDisplay => Info switch
    {
        null => Type.ToString()!,
        Type type => type.Name,
        PrimitiveType primitive => primitive.ToString()!,
        ClassTypeInfo info => info.ToString()!,
        string typeName => typeName,
        _ => throw new InvalidOperationException()
    };
}

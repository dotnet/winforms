// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Core.BinaryFormat.Serializer;

namespace System.Private.Windows.Core.BinaryFormat;

internal static class BinaryFormattedObjectExtensions
{
    /// <summary>
    ///  Dereferences <see cref="MemberReference"/> records.
    /// </summary>
    public static IRecord Dereference(this IReadOnlyRecordMap recordMap, IRecord record) => record switch
    {
        MemberReference reference => recordMap[reference.IdRef],
        _ => record
    };

    internal static void Write(this IWritableRecord record, BinaryWriter writer) => record.Write(writer);
}

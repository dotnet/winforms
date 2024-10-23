// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Private.Windows.Core.BinaryFormat.Serializer;

namespace System.Private.Windows.Core.BinaryFormat;

internal static class ListConverter
{
    public static ListConverter<object?, object?> GetPrimitiveConverter(
        IList values,
        StringRecordsCollection strings) => new(
            values,
            (object? value) => value switch
            {
                null => null,
                string stringValue => strings.GetStringRecord(stringValue),
                _ => new MemberPrimitiveTyped(value)
            });
}

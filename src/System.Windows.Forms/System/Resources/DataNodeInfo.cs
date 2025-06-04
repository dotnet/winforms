// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Resources;
internal class DataNodeInfo
{
    public required string Name { get; set; }
    public string? Comment { get; set; }
    public string? TypeName { get; set; }
    public string? MimeType { get; set; }
    public string ValueData { get; set; } = string.Empty;
    public Point ReaderPosition; // Only used to track position in the reader

    internal DataNodeInfo Clone() => new()
    {
        Name = Name,
        Comment = Comment,
        TypeName = TypeName,
        MimeType = MimeType,
        ValueData = ValueData,
        ReaderPosition = ReaderPosition
    };
}

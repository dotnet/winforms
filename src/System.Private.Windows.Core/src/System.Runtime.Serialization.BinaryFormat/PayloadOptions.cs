// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;

namespace System.Runtime.Serialization.BinaryFormat;

public sealed class PayloadOptions
{
    public PayloadOptions() { }

    public TypeNameParseOptions? TypeNameParseOptions { get; set; }

    // public int MaxMemberCount { get; set; } = 100;

    // public bool AreCustomOffsetArraysAllowed { get; set; } = false;

    // public bool AreJaggedArraysAllowed { get; set; } = false;

    // public int MaxArrayLength { get; set; }

    // public int MaxArrayRank { get; set; }

    // public HashSet<Type> AllowedRootRecordTypes { get; } = new();

    // public HashSet<Type> AllowedTypes { get; } = new();
}

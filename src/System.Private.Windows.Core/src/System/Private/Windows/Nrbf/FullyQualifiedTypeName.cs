// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace System.Private.Windows.Nrbf;

/// <summary>
///  Represents a fully qualified type name split into assembly name and type name.
/// </summary>
internal readonly struct FullyQualifiedTypeName : IEquatable<FullyQualifiedTypeName>
{
    public FullyQualifiedTypeName(string fullTypeName, string assemblyName)
    {
        Debug.Assert(!string.IsNullOrWhiteSpace(assemblyName));
        Debug.Assert(!string.IsNullOrWhiteSpace(fullTypeName));

        AssemblyName = assemblyName;
        FullTypeName = fullTypeName;
    }

    [Required]
    public string AssemblyName { get; init; }

    [Required]
    public string FullTypeName { get; init; }

    public bool Equals(FullyQualifiedTypeName other) =>
        other.AssemblyName == AssemblyName && other.FullTypeName == FullTypeName;

    public override bool Equals(object? obj) => obj is FullyQualifiedTypeName other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(AssemblyName, FullTypeName);

    public override string ToString() => $"{FullTypeName}, {AssemblyName}";
}

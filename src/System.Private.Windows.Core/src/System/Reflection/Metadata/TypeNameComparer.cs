// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Reflection.Metadata;

/// <summary>
///  Compares <see cref="TypeName"/>s.
/// </summary>
internal sealed class TypeNameComparer : IEqualityComparer<TypeName>
{
    private readonly TypeNameComparison _comparison;

    private TypeNameComparer(TypeNameComparison comparison) => _comparison = comparison;

    /// <summary>
    ///  Creates a comparer that does a fully qualified type name match.
    /// </summary>
    internal static TypeNameComparer FullyQualifiedMatch { get; } = new(TypeNameComparison.All);

    /// <summary>
    ///  Creates a comparer that does a full type name match. Ignores the assembly.
    /// </summary>
    internal static TypeNameComparer FullNameMatch { get; } = new(TypeNameComparison.TypeFullName);

    /// <summary>
    ///  Creates a comparer that does a full type name and assembly name match.
    ///  Ignores assembly version, culture, and public key token.
    /// </summary>
    internal static TypeNameComparer FullNameAndAssemblyNameMatch { get; } =
        new(TypeNameComparison.TypeFullName | TypeNameComparison.AssemblyName);

    public bool Equals(TypeName? x, TypeName? y)
    {
        if (x is null && y is null)
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return x.Matches(y, _comparison);
    }

    public int GetHashCode(TypeName obj)
    {
        if (obj is null)
        {
            return 0;
        }

        if (obj.IsArray)
        {
            return true.GetHashCode() ^ obj.GetArrayRank() ^ GetHashCode(obj.GetElementType());
        }

        int hashCode;
        if (obj.IsConstructedGenericType)
        {
            hashCode = "constructed".GetHashCode() ^ GetHashCode(obj.GetGenericTypeDefinition());
            foreach (TypeName genericName in obj.GetGenericArguments())
            {
                hashCode ^= GetHashCode(genericName);
            }

            return hashCode;
        }

        hashCode = obj.FullName.GetHashCode();
        if (obj.AssemblyName is AssemblyNameInfo info)
        {
            if (_comparison.HasFlag(TypeNameComparison.AssemblyName))
            {
                hashCode ^= info.Name.GetHashCode();
            }

            if (_comparison.HasFlag(TypeNameComparison.AssemblyVersion) && info.Version is not null)
            {
                hashCode ^= info.Version.GetHashCode();
            }

            if (_comparison.HasFlag(TypeNameComparison.AssemblyCultureName) && info.CultureName is not null)
            {
                hashCode ^= info.CultureName.GetHashCode();
            }

            if (_comparison.HasFlag(TypeNameComparison.AssemblyPublicKeyToken) && !info.PublicKeyOrToken.IsDefaultOrEmpty)
            {
                // The hash code for ImmutableArray<byte> is the instance, not the contents.
                foreach (byte b in info.PublicKeyOrToken)
                {
                    hashCode ^= b.GetHashCode();
                }
            }
        }

        return hashCode;
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace System;

/// <summary>
///  Helper methods for comparing <see cref="Type"/>s and <see cref="TypeName"/>s.
/// </summary>
internal static class TypeExtensions
{
    /// <summary>
    ///  Match type <paramref name="type"/> against <paramref name="typeName"/>.
    /// </summary>
    /// <param name="type">The type to match.</param>
    /// <param name="typeName">The type name to match against.</param>
    /// <param name="comparison">Comparison options.</param>
    internal static bool Matches(
        this Type type,
        TypeName typeName,
        TypeNameComparison comparison = TypeNameComparison.All)
    {
        // based on https://github.com/dotnet/runtime/blob/1474fc3fafca26b4b051be7dacdba8ac2804c56e/src/libraries/System.Formats.Nrbf/src/System/Formats/Nrbf/SerializationRecord.cs#L68

        Debug.Assert(type is not null);

        // We don't need to check for pointers and references to arrays,
        // as it's impossible to serialize them with BinaryFormatter.
        if (type is null || type.IsPointer || type.IsByRef)
        {
            return false;
        }

        // At first, check the non-allocating properties for mismatch.
        if (type.IsArray != typeName.IsArray
            || type.IsConstructedGenericType != typeName.IsConstructedGenericType
            || type.IsNested != typeName.IsNested
            || (type.IsArray && type.GetArrayRank() != typeName.GetArrayRank())
            || type.IsSZArray != typeName.IsSZArray // int[] vs int[*]
            )
        {
            return false;
        }

        if (!AssemblyNamesMatch(type, typeName.AssemblyName, comparison))
        {
            return false;
        }

        if (type.FullName == typeName.FullName)
        {
            return true;
        }

        if (typeName.IsArray)
        {
            return Matches(type.GetElementType()!, typeName.GetElementType(), comparison);
        }

        if (type.IsConstructedGenericType)
        {
            if (!Matches(type.GetGenericTypeDefinition(), typeName.GetGenericTypeDefinition(), comparison))
            {
                return false;
            }

            ImmutableArray<TypeName> genericNames = typeName.GetGenericArguments();
            Type[] genericTypes = type.GetGenericArguments();

            if (genericNames.Length != genericTypes.Length)
            {
                return false;
            }

            for (int i = 0; i < genericTypes.Length; i++)
            {
                if (!Matches(genericTypes[i], genericNames[i], comparison))
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    /// <summary>
    ///  Matches type name <paramref name="x"/> against <paramref name="y"/>.
    /// </summary>
    /// <inheritdoc cref="Matches(Type, TypeName, TypeNameComparison)"/>
    internal static bool Matches(this TypeName x, TypeName y, TypeNameComparison comparison = TypeNameComparison.All)
    {
        if (x.IsArray != y.IsArray
            || x.IsConstructedGenericType != y.IsConstructedGenericType
            || x.IsNested != y.IsNested
            || (x.IsArray && x.GetArrayRank() != y.GetArrayRank())
            || x.IsSZArray != y.IsSZArray // int[] vs int[*]
            )
        {
            return false;
        }

        if (!AssemblyNamesMatch(x.AssemblyName, y.AssemblyName, comparison))
        {
            return false;
        }

        if (x.FullName == y.FullName)
        {
            return true;
        }

        if (y.IsArray)
        {
            return Matches(x.GetElementType(), y.GetElementType(), comparison);
        }

        if (x.IsConstructedGenericType)
        {
            if (!Matches(x.GetGenericTypeDefinition(), y.GetGenericTypeDefinition(), comparison))
            {
                return false;
            }

            ImmutableArray<TypeName> genericNamesY = y.GetGenericArguments();
            ImmutableArray<TypeName> genericNamesX = x.GetGenericArguments();

            if (genericNamesX.Length != genericNamesY.Length)
            {
                return false;
            }

            for (int i = 0; i < genericNamesX.Length; i++)
            {
                if (!Matches(genericNamesX[i], genericNamesY[i], comparison))
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    /// <summary>
    ///  Matches the given type's assembly name against the given <paramref name="assemblyNameInfo"/>.
    /// </summary>
    /// <param name="type">A type to match assembly info against.</param>
    /// <param name="assemblyNameInfo">Assembly name info to match against.</param>
    /// <param name="comparison">Comparison options.</param>
    /// <returns><see langword="true"/> if the assembly names meet the specified criteria.</returns>
    private static bool AssemblyNamesMatch(Type type, AssemblyNameInfo? assemblyNameInfo, TypeNameComparison comparison)
    {
        if (comparison == TypeNameComparison.TypeFullName)
        {
            // No assembly name comparison is requested.
            return true;
        }

        if (assemblyNameInfo is null)
        {
            return false;
        }

        AssemblyName assemblyName = type.Assembly.GetName();

        // Type names are case sensitive and ordinal.
        return (!comparison.HasFlag(TypeNameComparison.AssemblyName) || assemblyName.Name == assemblyNameInfo.Name)
            && (!comparison.HasFlag(TypeNameComparison.AssemblyCultureName) || assemblyName.CultureName == assemblyNameInfo.CultureName)
            && (!comparison.HasFlag(TypeNameComparison.AssemblyVersion) || assemblyName.Version == assemblyNameInfo.Version)
            && (!comparison.HasFlag(TypeNameComparison.AssemblyPublicKeyToken)
                // ImmutableArray equality is instance equality.
                || ComparePublicKeys(assemblyName.GetPublicKeyToken().AsSpan(), assemblyNameInfo.PublicKeyOrToken.AsSpan()));
    }

    /// <summary>
    ///  Matches the given assembly names against each other.
    /// </summary>
    /// <param name="name1">The first assembly name to match.</param>
    /// <param name="name2">The second assembly name to match.</param>
    /// <inheritdoc cref="AssemblyNamesMatch(Type, AssemblyNameInfo?, TypeNameComparison)"/>
    private static bool AssemblyNamesMatch(AssemblyNameInfo? name1, AssemblyNameInfo? name2, TypeNameComparison comparison)
    {
        if (comparison == TypeNameComparison.TypeFullName)
        {
            // No assembly name comparison is requested.
            return true;
        }

        if (name1 is null && name2 is null)
        {
            return true;
        }

        if (name1 is null || name2 is null)
        {
            return false;
        }

        // Type names are case sensitive and ordinal.
        return (!comparison.HasFlag(TypeNameComparison.AssemblyName) || name1.Name == name2.Name)
            && (!comparison.HasFlag(TypeNameComparison.AssemblyCultureName) || name1.CultureName == name2.CultureName)
            && (!comparison.HasFlag(TypeNameComparison.AssemblyVersion) || name1.Version == name2.Version)
            && (!comparison.HasFlag(TypeNameComparison.AssemblyPublicKeyToken)
                // ImmutableArray equality is instance equality.
                || ComparePublicKeys(name1.PublicKeyOrToken.AsSpan(), name2.PublicKeyOrToken.AsSpan()));
    }

    /// <summary>
    ///  Convert <paramref name="type"/> to <see cref="TypeName"/>. Take into account type forwarding in order
    ///  to create <see cref="TypeName"/> compatible with the type names serialized to the binary format.This
    ///  method removes nullability wrapper from the top level type only because <see cref="TypeName"/> in the
    ///  serialization root record is not nullable, but the generic types could be nullable.
    /// </summary>
    internal static TypeName ToTypeName(this Type type)
    {
        // Unwrap type that is matched against the root record type.
        type = type.UnwrapIfNullable();
        return TypeName.Parse(type.AssemblyQualifiedName ?? type.FullName);
    }

    /// <summary>
    ///  If <paramref name="type"/> is a nullable type, return the underlying type; otherwise, return <paramref name="type"/>.
    /// </summary>
    internal static Type UnwrapIfNullable(this Type type) =>
        type.IsGenericType && !type.IsGenericTypeDefinition && type.GetGenericTypeDefinition() == typeof(Nullable<>)
            ? type.GetGenericArguments()[0]
            : type;

    /// <summary>
    ///  Helper method that allows non-allocating conversion of a interpolated string to a <see cref="TypeName"/>.
    /// </summary>
    internal static TypeName ToTypeName(ref ValueStringBuilder builder)
    {
        using (builder)
        {
            return TypeName.Parse(builder.AsSpan());
        }
    }

    /// <summary>
    ///  Compares two public keys by their token value. Handles comparing public key tokens to full public keys.
    /// </summary>
    private static bool ComparePublicKeys(ReadOnlySpan<byte> publicKey1, ReadOnlySpan<byte> publicKey2)
    {
        if (publicKey1.Length == publicKey2.Length)
        {
            return publicKey1.SequenceEqual(publicKey2);
        }

        if (publicKey1.Length == 0 || publicKey2.Length == 0)
        {
            return false;
        }

        const int PublicKeyTokenLength = 8;

        return publicKey1.Length == PublicKeyTokenLength
            ? TryComparePublicKeyTokenToKey(publicKey1, publicKey2)
            : TryComparePublicKeyTokenToKey(publicKey2, publicKey1);

        static bool TryComparePublicKeyTokenToKey(ReadOnlySpan<byte> publicKeyToken, ReadOnlySpan<byte> publicKey)
        {
            try
            {
                AssemblyName name = new();
                name.SetPublicKey(publicKey.ToArray());
                return publicKeyToken.SequenceEqual(name.GetPublicKeyToken());
            }
            catch (Exception e) when (!e.IsCriticalException())
            {
                // Generating the public key token validates the public key, and it will throw if invalid.
                Debug.Fail(e.Message);
                return false;
            }
        }
    }
}

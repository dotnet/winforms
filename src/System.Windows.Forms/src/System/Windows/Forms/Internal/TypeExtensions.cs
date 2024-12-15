// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms;

/// <summary>
///  Helper methods for comparing <see cref="Type"/>s and <see cref="TypeName"/>s.
/// </summary>
internal static class TypeExtensions
{
    private static readonly Type s_forwardedFromAttributeType = typeof(TypeForwardedFromAttribute);

    /// <summary>
    ///  Get the full assembly name this <paramref name="type"/> is forwarded from.
    /// </summary>
    /// <returns>
    ///  <see langword="true"/> if the <paramref name="type"/> is forwarded from another assembly;
    ///  otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryGetForwardedFromName(this Type type, [NotNullWhen(true)] out string? name)
    {
        name = default;

        // Special case types like arrays.
        Type attributedType = type;
        while (attributedType.HasElementType)
        {
            attributedType = attributedType.GetElementType()!;
        }

        object[] attributes = attributedType.GetCustomAttributes(s_forwardedFromAttributeType, inherit: false);
        if (attributes.Length > 0 && attributes[0] is TypeForwardedFromAttribute attribute)
        {
            name = attribute.AssemblyFullName;
            return true;
        }

        return false;
    }

    /// <summary>
    ///  The entry point for matching <paramref name="type"/> to <paramref name="typeName"/>. The top level <see cref="Type"/>
    ///  can be nullable, as the user would request a nullable type read from the clipboard payload, but the root record would
    ///  serialize a non-nullable type, thus <paramref name="typeName"/> from the root record is not nullable.
    /// </summary>
    public static bool MatchExceptAssemblyVersion(this Type type, TypeName typeName)
    {
        type = Formatter.NullableUnwrap(type);

        return type.MatchLessAssemblyVersion(typeName);
    }

    /// <summary>
    ///  Match namespace-qualified type names and assembly names with no version.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///  Read the <see cref="TypeForwardedFromAttribute"/> from the <paramref name="type"/> to match the unified assembly names,
    ///  because <paramref name="typeName"/> had been serialized with the forwarded from assembly name by our serializers.
    ///  </para>
    /// </remarks>
    // based on https://github.com/dotnet/runtime/blob/1474fc3fafca26b4b051be7dacdba8ac2804c56e/src/libraries/System.Formats.Nrbf/src/System/Formats/Nrbf/SerializationRecord.cs#L68
    private static bool MatchLessAssemblyVersion(this Type type, TypeName typeName)
    {
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

        if (!type.TryGetForwardedFromName(out string? name))
        {
            name = type.Assembly.FullName;
        }

        if (!AssemblyNamesLessVersionMatch(name, typeName.AssemblyName))
        {
            return false;
        }

        if (type.FullName == typeName.FullName)
        {
            return true;
        }

        if (typeName.IsArray)
        {
            return MatchLessAssemblyVersion(type.GetElementType()!, typeName.GetElementType());
        }

        if (type.IsConstructedGenericType)
        {
            if (!MatchLessAssemblyVersion(type.GetGenericTypeDefinition(), typeName.GetGenericTypeDefinition()))
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
                if (!MatchLessAssemblyVersion(genericTypes[i], genericNames[i]))
                {
                    return false;
                }
            }

            return true;
        }

        return false;

        static bool AssemblyNamesLessVersionMatch(string? fullName, AssemblyNameInfo? nameInfo)
        {
            if (string.Equals(fullName, nameInfo?.FullName, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (fullName is null || nameInfo is null)
            {
                return false;
            }

            if (!AssemblyNameInfo.TryParse(fullName, out AssemblyNameInfo? nameInfo1))
            {
                return false;
            }

            // Match everything except for the versions.
            return nameInfo.Name == nameInfo1.Name
                && ((nameInfo.CultureName ?? string.Empty) == nameInfo1.CultureName)
                && nameInfo.PublicKeyOrToken.AsSpan().SequenceEqual(nameInfo1.PublicKeyOrToken.AsSpan());
        }
    }

    /// <summary>
    ///  Match <see cref="TypeName"/>s using all information that had been set by the caller.
    /// </summary>
    public static bool Matches(this TypeName x, TypeName y)
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

        if (!AssemblyNamesMatch(x.AssemblyName, y.AssemblyName))
        {
            return false;
        }

        if (x.FullName == y.FullName)
        {
            return true;
        }

        if (y.IsArray)
        {
            return Matches(x.GetElementType(), y.GetElementType());
        }

        if (x.IsConstructedGenericType)
        {
            if (!Matches(x.GetGenericTypeDefinition(), y.GetGenericTypeDefinition()))
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
                if (!Matches(genericNamesX[i], genericNamesY[i]))
                {
                    return false;
                }
            }

            return true;
        }

        return false;

        static bool AssemblyNamesMatch(AssemblyNameInfo? name1, AssemblyNameInfo? name2)
        {
            if (name1 is null && name2 is null)
            {
                return true;
            }

            if (name1 is null || name2 is null)
            {
                return false;
            }

            // Case-sensitive comparisons.
            return name1.Name == name2.Name
                && name1.CultureName == name2.CultureName
                && name1.Version == name2.Version
                && name1.PublicKeyOrToken.AsSpan().SequenceEqual(name2.PublicKeyOrToken.AsSpan());
        }
    }

    /// <summary>
    ///  Convert <paramref name="type"/> to <see cref="TypeName"/>. Take into account type forwarding in order
    ///  to create <see cref="TypeName"/> compatible with the type names serialized to the binary format.This
    ///  method removes nullability wrapper from the top level type only because <see cref="TypeName"/> in the
    ///  serialization root record is not nullable, but the generic types could be nullable.
    /// </summary>
    public static TypeName ToTypeName(this Type type)
    {
        // Unwrap type that is matched against the root record type.
        type = Formatter.NullableUnwrap(type);
        if (!type.TryGetForwardedFromName(out string? assemblyName))
        {
            assemblyName = type.Assembly.FullName;
        }

        return TypeName.Parse($"{GetTypeFullName(type)}, {assemblyName}");

        static string GetTypeFullName(Type type)
        {
            if (type.IsConstructedGenericType)
            {
                Type[] genericArguments = type.GetGenericArguments();
                string[] genericTypeNames = new string[genericArguments.Length];
                for (int i = 0; i < type.GetGenericArguments().Length; i++)
                {
                    Type generic = genericArguments[i];
                    // Keep Nullable wrappers for types inside generic types.
                    if (!generic.TryGetForwardedFromName(out string? name))
                    {
                        name = generic.Assembly.FullName;
                    }

                    genericTypeNames[i] = $"[{GetTypeFullName(generic)}, {name}]";
                }

                return $"{type.Namespace}.{type.Name}[{string.Join(",", genericTypeNames)}]";
            }

            return type.FullName.OrThrowIfNull();
        }
    }
}

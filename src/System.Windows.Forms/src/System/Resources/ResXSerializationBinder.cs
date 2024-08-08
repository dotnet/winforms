// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Reflection.Metadata;
using System.Runtime.Serialization;

namespace System.Resources;

/// <summary>
///  This class implements a partial type resolver for the BinaryFormatter can provide custom type name binding to
///  or from types.
/// </summary>
/// <remarks>
///  <para>
///   The key usage of this type is to attempt to redirect to / from .NET Framework type names.
///  </para>
/// </remarks>
internal class ResXSerializationBinder : SerializationBinder
{
    private readonly ITypeResolutionService? _typeResolver;
    private readonly Func<Type?, string>? _typeNameConverter;

    /// <param name="typeResolver">
    ///  The custom type resolution service used to bind names to a specific <see cref="Type"/>. Only
    ///  <see cref="ITypeResolutionService.GetType(string)"/> is called by this binder.
    /// </param>
    internal ResXSerializationBinder(ITypeResolutionService? typeResolver) => _typeResolver = typeResolver;

    /// <param name="typeNameConverter">
    ///  The type name converter to use for binding a <see cref="Type"/> to a custom name. This is passed in through
    ///  constructors on <see cref="ResXDataNode"/> such as <see cref="ResXDataNode(string, object?, Func{Type?, string}?)"/>
    /// </param>
    internal ResXSerializationBinder(Func<Type?, string>? typeNameConverter) => _typeNameConverter = typeNameConverter;

    public override Type? BindToType(
        string assemblyName,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] string typeName)
    {
        if (_typeResolver is null || !TypeName.TryParse($"{typeName}, {assemblyName}".AsSpan(), out TypeName? parsed))
        {
            // cs/deserialization/nullbindtotype
            return null; // CodeQL [SM04225] : This class is meant to redirect to .NET Framework type names. If this cannot be done, the default binder should be used.
        }

        Type? type = _typeResolver.GetType(parsed.AssemblyQualifiedName);
        if (type is not null)
        {
            return type;
        }

        if (parsed.AssemblyName is { } assemblyNameInfo)
        {
            // Try the name without the version.
            string fullyQualifiedNameWithoutVersion =
                $"{typeName}, {new AssemblyNameInfo(
                    assemblyNameInfo.Name,
                    version: null,
                    assemblyNameInfo.CultureName,
                    assemblyNameInfo.Flags,
                    assemblyNameInfo.PublicKeyOrToken).FullName}";
            type = _typeResolver.GetType(fullyQualifiedNameWithoutVersion);
        }

        // If that didn't work, try the simple name.
        type ??= _typeResolver.GetType(parsed.FullName);

        // Hand back what we found or null to let the default loader take over.
        // cs/deserialization/nullbindtotype
        return type; // CodeQL[SM04225] : This binder isn't intended as a security facility; it's allowable for us to return null.
    }

    public override void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
    {
        // Normally, we don't change the type name when changing the target framework, only the assembly name.
        // Setting the out values to null indicates that we want default handling.

        if (_typeNameConverter is not null)
        {
            // Allow the specified type name converter to modify the type name.
            string? assemblyQualifiedTypeName = MultitargetUtil.GetAssemblyQualifiedName(serializedType, _typeNameConverter);
            if (!string.IsNullOrEmpty(assemblyQualifiedTypeName)
                && TypeName.TryParse(assemblyQualifiedTypeName.AsSpan(), out TypeName? parsed)
                && parsed.AssemblyName is { } assemblyInfo)
            {
                // Set the custom assembly name.
                assemblyName = assemblyInfo.FullName;

                // Customize the type name only if it changed.
                typeName = string.Equals(parsed.FullName, serializedType.FullName, StringComparison.Ordinal)
                    ? null
                    : parsed.FullName;

                return;
            }
        }

        base.BindToName(serializedType, out assemblyName, out typeName);
    }
}

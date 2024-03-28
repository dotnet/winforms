// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
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
        if (_typeResolver is null)
        {
            return null;
        }

        // Try the fully-qualified name first.
        typeName = $"{typeName}, {assemblyName}";

        Type? type = _typeResolver.GetType(typeName);
        if (type is not null)
        {
            return type;
        }

        string[] typeParts = typeName.Split(',', StringSplitOptions.TrimEntries);

        if (typeParts.Length > 2)
        {
            string partialName = typeParts[0];

            // Strip out the version.
            for (int i = 1; i < typeParts.Length; ++i)
            {
                string typePart = typeParts[i];
                if (!typePart.StartsWith("Version=", StringComparison.Ordinal)
                    && !typePart.StartsWith("version=", StringComparison.Ordinal))
                {
                    partialName = $"{partialName}, {typePart}";
                }
            }

            // Try the name without the version.
            type = _typeResolver.GetType(partialName);

            // If that didn't work, try the simple name.
            type ??= _typeResolver.GetType(typeParts[0]);
        }

        // Hand back what we found or null to let the default loader take over.
        return type;
    }

    public override void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
    {
        // Normally, we don't change the type name when changing the target framework, only the assembly name.
        // Setting the out values to null indicates that we want default handling.

        if (_typeNameConverter is not null)
        {
            // Allow the specified type name converter to modify the type name.
            string? assemblyQualifiedTypeName = MultitargetUtil.GetAssemblyQualifiedName(serializedType, _typeNameConverter);
            if (!string.IsNullOrEmpty(assemblyQualifiedTypeName))
            {
                // Split the assembly name from the type name.
                int pos = assemblyQualifiedTypeName.IndexOf(',');
                if (pos > 0 && pos < assemblyQualifiedTypeName.Length - 1)
                {
                    // Set the custom assembly name.
                    assemblyName = assemblyQualifiedTypeName[(pos + 1)..].TrimStart();

                    // Customize the type name only if it changed.
                    string newTypeName = assemblyQualifiedTypeName[..pos];
                    typeName = string.Equals(newTypeName, serializedType.FullName, StringComparison.Ordinal)
                        ? null
                        : newTypeName;

                    return;
                }
            }
        }

        base.BindToName(serializedType, out assemblyName, out typeName);
    }
}

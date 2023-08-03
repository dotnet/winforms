// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Resources;

/// <summary>
///  Helper class supporting Multitarget type assembly qualified name resolution for ResX API.
/// </summary>
internal static class MultitargetUtil
{
    /// <summary>
    ///  This method gets the assembly qualified name for the corresponding type, prefering
    ///  the <paramref name="typeNameConverter"/> if provided.
    /// </summary>
    public static string? GetAssemblyQualifiedName(Type type, Func<Type, string>? typeNameConverter)
    {
        if (type is null)
        {
            return null;
        }

        string? assemblyQualifiedName = null;

        if (typeNameConverter is not null)
        {
            try
            {
                assemblyQualifiedName = typeNameConverter(type);
            }
            catch (Exception e) when (!e.IsCriticalException())
            {
            }
        }

        return string.IsNullOrEmpty(assemblyQualifiedName) ? type.AssemblyQualifiedName : assemblyQualifiedName;
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;

namespace System.Private.Windows.BinaryFormat;

/// <summary>
///  Resolver for types.
/// </summary>
internal interface ITypeResolver
{
    /// <summary>
    ///  Resolves the given type name against the specified library. Throws if the type cannot be resolved.
    /// </summary>
    Type BindToType(TypeName typeName);

    /// <summary>
    ///  Tries to resolve the given type name against the specified library.
    /// </summary>
    bool TryBindToType(TypeName typeName, [NotNullWhen(true)] out Type? type);
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Reflection.Metadata;

[Flags]
internal enum TypeNameComparison
{
    /// <summary>
    ///  Use the full name of the type (default). (Namespace + Name)
    /// </summary>
    TypeFullName = 0b00000000,

    /// <summary>
    ///  Use the assembly name of the type.
    /// </summary>
    AssemblyName = 0b00000010,

    /// <summary>
    ///  Use the assembly culture of the type.
    /// </summary>
    AssemblyCultureName = 0b00000100,

    /// <summary>
    ///  Use the assembly version of the type.
    /// </summary>
    AssemblyVersion = 0b00001000,

    /// <summary>
    ///  Use the assembly public key token of the type.
    /// </summary>
    AssemblyPublicKeyToken = 0b00010000,

    /// <summary>
    ///  Use all parts of the fully qualified type name.
    /// </summary>
    All = TypeFullName | AssemblyName | AssemblyCultureName | AssemblyVersion | AssemblyPublicKeyToken,

    /// <summary>
    ///  Match all parts of the fully qualified type name except the assembly version.
    /// </summary>
    AllButAssemblyVersion = TypeFullName | AssemblyName | AssemblyCultureName | AssemblyPublicKeyToken
}

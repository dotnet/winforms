﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;

namespace System.Windows.Forms.BinaryFormat;

internal sealed partial class BinaryFormattedObject
{
    /// <summary>
    ///  Resolver for types.
    /// </summary>
    internal interface ITypeResolver
    {
        /// <summary>
        ///  Resolves the given type name against the specified library.
        /// </summary>
        /// <param name="libraryName">The library id, or <see cref="Id.Null"/> for the "system" assembly.</param>
        [RequiresUnreferencedCode("Calls System.Reflection.Assembly.GetType(String)")]
        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type GetType(TypeName typeName, AssemblyNameInfo libraryName);
    }
}

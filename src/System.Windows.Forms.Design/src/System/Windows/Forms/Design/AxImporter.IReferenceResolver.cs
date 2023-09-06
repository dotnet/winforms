// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design;

public partial class AxImporter
{
    /// <summary>
    ///  Provides methods to resolve references to ActiveX libraries, COM type libraries or assemblies,
    ///  or managed assemblies.
    /// </summary>
    public interface IReferenceResolver
    {
        /// <summary>
        ///  Resolves a reference to the specified assembly.
        /// </summary>
        /// <returns>A fully qualified path to an assembly.</returns>
        string? ResolveManagedReference(string assemName);

        /// <summary>
        ///  Resolves a reference to the specified type library that contains an COM component.
        /// </summary>
        /// <returns>A fully qualified path to an assembly.</returns>
        string? ResolveComReference(UCOMITypeLib typeLib);

        /// <summary>
        ///  Resolves a reference to the specified assembly that contains a COM component.
        /// </summary>
        /// <returns>A fully qualified path to an assembly.</returns>
        string? ResolveComReference(AssemblyName name);

        /// <summary>
        ///  Resolves a reference to the specified type library that contains an ActiveX control.
        /// </summary>
        /// <returns>A fully qualified path to an assembly.</returns>
        string? ResolveActiveXReference(UCOMITypeLib typeLib);
    }
}

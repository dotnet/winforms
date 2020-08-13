// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  Provides multitarget type name resolution services in a design-time environment.
    /// </summary>
    public interface IMultitargetHelperService
    {
        /// <summary>
        ///  To be implemented by a VS component that can get resolve a Type for the target framework and return type.AssemblyQualifiedName.
        /// </summary>
        string GetAssemblyQualifiedName(Type type);
    }
}

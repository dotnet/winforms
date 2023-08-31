// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Resources.Tools;

/// <summary>
///  <para>
///   Defines an interface that enables the strongly typed resource builder (<see cref="StronglyTypedResourceBuilder"/>
///   object) to determine which types and properties are available so it can emit the correct Code Document Object
///   Model (CodeDOM) tree.
///  </para>
///  <para>
///   This API supports the product infrastructure and is not intended to be used directly from your code.
///  </para>
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface ITargetAwareCodeDomProvider
{
    /// <summary>
    ///  <para>
    ///   Indicates whether the specified type on the project target framework has a specified named property.
    ///  </para>
    ///  <para>
    ///   This API supports the product infrastructure and is not intended to be used directly from your code.
    ///  </para>
    /// </summary>
    /// <param name="type">The type whose properties are to be queried.</param>
    /// <param name="propertyName">The name of the property to find in <paramref name="type"/>.</param>
    /// <param name="isWritable">A flag that indicates whether the property must include a get accessor.</param>
    /// <returns>
    ///  <see langword="true"/> if <paramref name="type"/> on the project target framework has a property named
    ///  <paramref name="propertyName"/>; otherwise, <see langword="false"/>.
    /// </returns>
    bool SupportsProperty(Type type, string propertyName, bool isWritable);
}

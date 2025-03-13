// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public enum GridItemType
{
    /// <summary>
    ///  The <see cref="GridItem"/> corresponds to a property.
    /// </summary>
    Property,

    /// <summary>
    ///  The <see cref="GridItem"/> is a category name.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Typical categories include "Behavior", "Layout", "Data", and "Appearance".
    ///  </para>
    /// </remarks>
    Category,

    /// <summary>
    ///  The <see cref="GridItem"/> is an element of an array.
    /// </summary>
    ArrayValue,

    /// <summary>
    ///  The <see cref="GridItem"/> is the root in the grid hierarchy.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This item represents the selection in the <see cref="PropertyGrid"/>.
    ///  </para>
    /// </remarks>
    Root
}

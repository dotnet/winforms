// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#pragma warning disable RS0016 // Add public types and members to the declared API
[Obsolete("MenuMerge has been deprecated.")]
public enum MenuMerge
{
    /// <summary>
    ///  The <see cref='MenuItem'/> is added to the
    ///  existing <see cref='MenuItem'/> objects in a
    ///  merged menu.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    Add = 0,

    /// <summary>
    ///  The <see cref='MenuItem'/> replaces the
    ///  existing <see cref='MenuItem'/> at the same
    ///  position in a merged menu.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    Replace = 1,

    /// <summary>
    ///  Subitems of this <see cref='MenuItem'/> are merged
    ///  with those of existing <see cref='MenuItem'/>
    ///  objects at the same position in a merged menu.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    MergeItems = 2,

    /// <summary>
    ///  The <see cref='MenuItem'/> is not included in a
    ///  merged menu.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    Remove = 3,
}

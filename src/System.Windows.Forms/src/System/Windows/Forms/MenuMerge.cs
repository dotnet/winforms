// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the behavior of a <see cref='MenuItem'/> when it is merged with items in another menu.
    /// </summary>
    public enum MenuMerge
    {
        /// <summary>
        ///  The <see cref='MenuItem'/> is added to the
        ///  existing <see cref='MenuItem'/> objects in a
        ///  merged menu.
        /// </summary>
        Add = 0,

        /// <summary>
        ///  The <see cref='MenuItem'/> replaces the
        ///  existing <see cref='MenuItem'/> at the same
        ///  position in a merged menu.
        /// </summary>
        Replace = 1,

        /// <summary>
        ///  Subitems of this <see cref='MenuItem'/> are merged
        ///  with those of existing <see cref='MenuItem'/>
        ///  objects at the same position in a merged menu.
        /// </summary>
        MergeItems = 2,

        /// <summary>
        ///  The <see cref='MenuItem'/> is not included in a
        ///  merged menu.
        /// </summary>
        Remove = 3,
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Possible values for property grid sorting mode
    /// </summary>
    public enum PropertySort
    {
        /// <summary>
        ///  Properties will not be sorted, rather they will be displayed in the
        ///  order that they are retrieved from the TypeDescriptor.
        /// </summary>
        NoSort = 0,

        /// <summary>
        ///  Properties are sorted as a flat, alphabetical list.
        /// </summary>
        Alphabetical = 1,

        /// <summary>
        ///  Properties are under category groups, which are defined by the
        ///  properties themselves.
        /// </summary>
        Categorized = 2,

        /// <summary>
        ///  Properties are under category groups, which are defined by the
        ///  properties themselves, and are alphabetical within those groups.
        /// </summary>
        CategorizedAlphabetical = Alphabetical | Categorized,
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies how an accessible object will be selected or receive focus.
    /// </summary>
    [Flags]
    public enum AccessibleSelection
    {
        /// <summary>
        ///  The selection or focus of an object is unchanged.
        /// </summary>
        None = 0,

        /// <summary>
        ///  Assigns focus to an object and makes it the anchor, which is the
        ///  starting point for the selection. Can be combined with
        /// <see cref='TakeSelection'/>, <see cref='ExtendSelection'/>,
        /// <see cref='AddSelection'/>, or  <see cref='RemoveSelection'/>.
        /// </summary>
        TakeFocus = 1,

        /// <summary>
        ///  Selects the object and deselects all other objects in the container.
        /// </summary>
        TakeSelection = 2,

        /// <summary>
        ///  Selects all objects between the anchor and the selected object.
        /// </summary>
        ExtendSelection = 4,

        /// <summary>
        ///  Adds the object to the selection.
        /// </summary>
        AddSelection = 8,

        /// <summary>
        ///  Removes the object from the selection.
        /// </summary>
        RemoveSelection = 16,
    }
}

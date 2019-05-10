// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies how an accessible object will be selected or receive focus.
    /// </devdoc>
    [Flags]
    public enum AccessibleSelection
    {
        /// <devdoc>
        /// The selection or focus of an object is unchanged.
        /// </devdoc>
        None = 0,

        /// <devdoc>
        ///    <para>
        /// Assigns focus to an object and makes it the anchor, which is the
        /// starting point for the selection. Can be combined with
        /// <see cref='TakeSelection'/>, <see cref='ExtendSelection'/>,
        /// <see cref='AddSelection'/>, or  <see cref='RemoveSelection'/>.
        /// </devdoc>
        TakeFocus = 1,

        /// <devdoc>
        /// Selects the object and deselects all other objects in the container.
        /// </devdoc>
        TakeSelection = 2,

        /// <devdoc>
        /// Selects all objects between the anchor and the selected object.
        /// </devdoc>
        ExtendSelection = 4,

        /// <devdoc>
        /// Adds the object to the selection.
        /// </devdoc>
        AddSelection = 8,

        /// <devdoc>
        /// Removes the object from the selection.
        /// </devdoc>
        RemoveSelection = 16,
    }
}

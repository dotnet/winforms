// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///     This class represents the arguments describing a BehaviorDragDrop event
    ///     fired by the BehaviorService.
    /// </summary>
    public class BehaviorDragDropEventArgs : EventArgs
    {
        /// <summary>
        ///     Constructor.  This class is created by the BehaviorService directly
        ///     before a drag operation begins.
        /// </summary>
        public BehaviorDragDropEventArgs(ICollection dragComponents)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Returns the list of IComponents currently being dragged.
        /// </summary>
        public ICollection DragComponents => throw new NotImplementedException(SR.NotImplementedByDesign);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  Provides data for the <see cref="DesignerActionService.DesignerActionListsChanged" /> event.
    /// </summary>
    /// <remarks>
    ///  The <c>DesignerActionListsChangedEventArgs</c> class is used by the <see cref="DesignerActionService"/> class
    ///  to signify that a <see cref="DesignerActionList"/> was added or removed from the related object.
    /// </remarks>
    public class DesignerActionListsChangedEventArgs : EventArgs
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="DesignerActionListsChangedEventArgs" /> class with the specified related object,
        ///  the type of change, and the remaining action lists for the related object.
        /// </summary>
        /// <param name="relatedObject">The related object.</param>
        /// <param name="changeType">One of the enumeration values that specifies the type of change.</param>
        /// <param name="actionLists">A collection that represents the remaining action lists for the related object.</param>
        public DesignerActionListsChangedEventArgs(object relatedObject, DesignerActionListsChangedType changeType, DesignerActionListCollection actionLists)
        {
            RelatedObject = relatedObject;
            ChangeType = changeType;
            ActionLists = actionLists;
        }

        /// <summary>
        ///  Gets the object that is associated with the event.
        /// </summary>
        /// <value>The object that is associated with the event.</value>
        public object RelatedObject { get; }

        /// <summary>
        ///  Gets the type of change that caused the event to be raised.
        /// </summary>
        /// <value>One of the enumeration values that specifies the type of change.</value>
        public DesignerActionListsChangedType ChangeType { get; }

        /// <summary>
        ///  Gets the collection that contains the remaining action lists for the related object.
        /// </summary>
        /// <value>The collection that contains the remaining action lists for the related object.</value>
        public DesignerActionListCollection ActionLists { get; }
    }
}

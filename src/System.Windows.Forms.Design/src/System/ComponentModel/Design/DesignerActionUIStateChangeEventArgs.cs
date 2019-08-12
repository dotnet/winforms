// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  This EventArgs class is used by the DesignerActionService to signify that there has been a change in DesignerActionLists (added or removed) on the related object.
    /// </summary>
    public class DesignerActionUIStateChangeEventArgs : EventArgs
    {
        /// <summary>
        ///  Constructor that requires the object in question, the type of change and the remaining actionlists left for the object on the related object.
        /// </summary>
        public DesignerActionUIStateChangeEventArgs(object relatedObject, DesignerActionUIStateChangeType changeType)
        {
            RelatedObject = relatedObject;
            ChangeType = changeType;
        }

        /// <summary>
        ///  The object this change is related to.
        /// </summary>
        public object RelatedObject { get; }

        /// <summary>
        ///  The type of changed that caused the related event to be thrown.
        /// </summary>
        public DesignerActionUIStateChangeType ChangeType { get; }
    }
}

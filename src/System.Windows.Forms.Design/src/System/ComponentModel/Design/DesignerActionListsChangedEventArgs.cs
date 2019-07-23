// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    /// <summary>
    /// This EventArgs class is used by the DesignerActionService to signify that there has been a change in DesignerActionLists (added or removed) on the related object.
    /// </summary>
    public class DesignerActionListsChangedEventArgs : EventArgs
    {
        private readonly object _relatedObject;
        private readonly DesignerActionListCollection _actionLists;
        private readonly DesignerActionListsChangedType _changeType; //type of change

        /// <summary>
        /// Constructor that requires the object in question, the type of change and the remaining actionlists left for the object. on the related object.
        /// </summary>
        public DesignerActionListsChangedEventArgs(object relatedObject, DesignerActionListsChangedType changeType, DesignerActionListCollection actionLists)
        {
            _relatedObject = relatedObject;
            _changeType = changeType;
            _actionLists = actionLists;
        }

        /// <summary>
        /// The type of changed that caused the related event to be thrown.
        /// </summary>
        public DesignerActionListsChangedType ChangeType
        {
            get => _changeType;
        }

        /// <summary>
        /// The object this change is related to.
        /// </summary>
        public object RelatedObject
        {
            get => _relatedObject;
        }

        /// <summary>
        /// The remaining actionlists left for the related object.
        /// </summary>
        public DesignerActionListCollection ActionLists
        {
            get => _actionLists;
        }
    }
}

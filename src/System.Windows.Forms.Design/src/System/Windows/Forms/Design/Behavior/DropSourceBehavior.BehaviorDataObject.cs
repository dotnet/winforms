// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms.Design.Behavior
{
    internal sealed partial class DropSourceBehavior
    {
        /// <summary>
        ///  This class extends from <see cref="DataObject"/> and carries additional
        ///  information such as: the list of Controls currently being dragged and the drag 'Source'.
        /// </summary>
        internal class BehaviorDataObject : DataObject
        {
            private readonly DropSourceBehavior _sourceBehavior;

            public BehaviorDataObject(ICollection dragComponents, Control source, DropSourceBehavior sourceBehavior) : base()
            {
                DragComponents = dragComponents;
                Source = source;
                _sourceBehavior = sourceBehavior;
                Target = null;
            }

            public Control Source { get; }

            public ICollection DragComponents { get; }

            public IComponent? Target { get; set; }

            internal void EndDragDrop(bool allowSetChildIndexOnDrop) => _sourceBehavior.EndDragDrop(allowSetChildIndexOnDrop);

            internal void CleanupDrag() => _sourceBehavior.CleanupDrag();

            internal ArrayList GetSortedDragControls(ref int primaryControlIndex) => _sourceBehavior.GetSortedDragControls(ref primaryControlIndex);
        }
    }
}

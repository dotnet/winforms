// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Design.Behavior;

internal sealed partial class DropSourceBehavior
{
    /// <summary>
    ///  This class extends from <see cref="DataObject"/> and carries additional
    ///  information such as: the list of Controls currently being dragged and the drag 'Source'.
    /// </summary>
    internal class BehaviorDataObject : DataObject
    {
        private readonly DropSourceBehavior _sourceBehavior;

        public BehaviorDataObject(List<IComponent> dragComponents, Control source, DropSourceBehavior sourceBehavior) : base()
        {
            DragComponents = dragComponents;
            Source = source;
            _sourceBehavior = sourceBehavior;
            Target = null;
        }

        public Control Source { get; }

        public List<IComponent> DragComponents { get; }

        public IComponent? Target { get; set; }

        internal void EndDragDrop(bool allowSetChildIndexOnDrop) => _sourceBehavior.EndDragDrop(allowSetChildIndexOnDrop);

        internal void CleanupDrag() => _sourceBehavior.CleanupDrag();

        internal List<IComponent> GetSortedDragControls(out int primaryControlIndex) => _sourceBehavior.GetSortedDragControls(out primaryControlIndex);
    }
}

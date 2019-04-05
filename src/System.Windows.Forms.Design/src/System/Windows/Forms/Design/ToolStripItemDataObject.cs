// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Wrapper class for DataObject. This wrapped object is passed when a ToolStripItem is Drag-Dropped during DesignTime.
    /// </summary>
    internal class ToolStripItemDataObject : DataObject
    {
        private readonly ArrayList _dragComponents;
        private readonly ToolStrip _owner;
        private readonly ToolStripItem _primarySelection;
        internal ToolStripItemDataObject(ArrayList dragComponents, ToolStripItem primarySelection, ToolStrip owner) : base()
        {
            _dragComponents = dragComponents;
            _owner = owner;
            _primarySelection = primarySelection;
        }

        internal ArrayList DragComponents
        {
            get => _dragComponents;
        }

        internal ToolStrip Owner
        {
            get => _owner;
        }

        internal ToolStripItem PrimarySelection
        {
            get => _primarySelection;
        }
    }
}

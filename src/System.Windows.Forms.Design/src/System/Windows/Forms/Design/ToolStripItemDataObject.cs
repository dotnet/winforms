// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

/// <summary>
///  Wrapper class for DataObject. This wrapped object is passed when a ToolStripItem is Drag-Dropped during DesignTime.
/// </summary>
internal class ToolStripItemDataObject : DataObject
{
    internal ToolStripItemDataObject(List<ToolStripItem> dragComponents, ToolStripItem primarySelection, ToolStrip owner) : base()
    {
        DragComponents = dragComponents;
        Owner = owner;
        PrimarySelection = primarySelection;
    }

    internal List<ToolStripItem> DragComponents { get; }

    internal ToolStrip Owner { get; }

    internal ToolStripItem PrimarySelection { get; }
}

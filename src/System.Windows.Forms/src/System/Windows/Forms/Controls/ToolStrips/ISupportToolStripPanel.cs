// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal interface ISupportToolStripPanel
{
    ToolStripPanelRow? ToolStripPanelRow { get; set; }

    ToolStripPanelCell? ToolStripPanelCell { get; }

    bool Stretch { get; set; }

    bool IsCurrentlyDragging { get; }

    void BeginDrag();

    void EndDrag();
}

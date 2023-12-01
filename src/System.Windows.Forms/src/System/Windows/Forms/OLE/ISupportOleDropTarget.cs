// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public interface IDropTarget
{
    void OnDragEnter(DragEventArgs e);
    void OnDragLeave(EventArgs e);
    void OnDragDrop(DragEventArgs e);
    void OnDragOver(DragEventArgs e);
}

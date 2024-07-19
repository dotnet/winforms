// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

internal class ContextMenuStripGroup
{
    private List<ToolStripItem>? _items;

    public List<ToolStripItem> Items => _items ??= [];
}

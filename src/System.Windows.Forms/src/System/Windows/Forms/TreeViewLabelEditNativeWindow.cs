// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal class TreeViewLabelEditNativeWindow : LabelEditNativeWindow
{
    private readonly WeakReference<TreeView> _owningTreeView;

    public TreeViewLabelEditNativeWindow(TreeView owningTreeView) : base(owningTreeView) => _owningTreeView = new(owningTreeView);

    public override AccessibleObject? AccessibilityObject => _owningTreeView.TryGetTarget(out TreeView? target) ? new TreeViewLabelEditAccessibleObject(target, this) : null;
}

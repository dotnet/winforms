// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal unsafe class TreeViewLabelEditAccessibleObject : LabelEditAccessibleObject
{
    private readonly TreeView _owningTreeView;
    private readonly WeakReference<LabelEditNativeWindow> _labelEdit;

    public TreeViewLabelEditAccessibleObject(TreeView owningTreeView, LabelEditNativeWindow labelEdit) : base(owningTreeView, labelEdit)
    {
        _owningTreeView = owningTreeView.OrThrowIfNull();
        _labelEdit = new(labelEdit);
    }

    private protected override string? AutomationId => _owningTreeView._editNode?.AccessibilityObject.Name;

    public override AccessibleObject? Parent => _owningTreeView._editNode?.AccessibilityObject;
}

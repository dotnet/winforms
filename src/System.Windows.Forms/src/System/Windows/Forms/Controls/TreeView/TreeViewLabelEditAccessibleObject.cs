// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

internal sealed unsafe class TreeViewLabelEditAccessibleObject : LabelEditAccessibleObject
{
    private readonly WeakReference<TreeView> _owningTreeView;

    public TreeViewLabelEditAccessibleObject(TreeView owningTreeView, TreeViewLabelEditNativeWindow labelEdit) : base(owningTreeView, labelEdit)
    {
        ArgumentNullException.ThrowIfNull(owningTreeView);
        _owningTreeView = new(owningTreeView);
    }

    private protected override string? AutomationId =>
        _owningTreeView.TryGetTarget(out TreeView? target)
            ? target._editNode?.AccessibilityObject?.Name
            : null;

    internal override IRawElementProviderFragmentRoot.Interface? FragmentRoot =>
        _owningTreeView.TryGetTarget(out TreeView? target)
            ? target.AccessibilityObject
            : null;

    public override AccessibleObject? Parent =>
        _owningTreeView.TryGetTarget(out TreeView? target)
            ? target._editNode?.AccessibilityObject
            : null;

    private protected override bool IsInternal => true;

    internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
        propertyID switch
        {
            UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => _owningTreeView.TryGetTarget(out TreeView? target) ? (VARIANT)target.Enabled : VARIANT.False,
            _ => base.GetPropertyValue(propertyID),
        };
}

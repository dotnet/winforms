// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal class ListViewLabelEditNativeWindow : LabelEditNativeWindow
{
    private readonly WeakReference<ListView> _owningListView;

    public ListViewLabelEditNativeWindow(ListView owningListView) : base(owningListView) => _owningListView = new(owningListView);

    public override AccessibleObject? AccessibilityObject => _owningListView.TryGetTarget(out ListView? target) ? new ListViewLabelEditAccessibleObject(target, this) : null;
}

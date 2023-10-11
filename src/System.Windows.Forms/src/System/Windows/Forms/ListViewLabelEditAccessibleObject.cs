// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static System.Windows.Forms.ListViewItem;
using UiaCore = Interop.UiaCore;

namespace System.Windows.Forms;

internal unsafe class ListViewLabelEditAccessibleObject : LabelEditAccessibleObject
{
    private const string LIST_VIEW_LABEL_EDIT_AUTOMATION_ID = "1";

    private readonly ListView _owningListView;
    private readonly ListViewSubItem? _owningListViewSubItem;
    private readonly ListViewItem? _owingListViewItem;
    private readonly WeakReference<LabelEditNativeWindow> _labelEdit;
    private readonly LabelEditUiaTextProvider _textProvider;

    public ListViewLabelEditAccessibleObject(ListView owningListView, LabelEditNativeWindow labelEdit) : base(owningListView, labelEdit)
    {
        _owningListView = owningListView.OrThrowIfNull();
        _owningListViewSubItem = owningListView._listViewSubItem;
        _owingListViewItem = owningListView._selectedItem;
        _labelEdit = new(labelEdit);
        UseStdAccessibleObjects(labelEdit.Handle);
        _textProvider = new(owningListView, labelEdit, this);
    }

    private protected override string AutomationId => LIST_VIEW_LABEL_EDIT_AUTOMATION_ID;

    public override AccessibleObject? Parent => _owningListViewSubItem is null
        ? _owingListViewItem?.AccessibilityObject
        : _owningListView.View == View.Tile
            ? _owingListViewItem?.AccessibilityObject
            : _owningListViewSubItem?.AccessibilityObject;

    internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        => direction switch
        {
            UiaCore.NavigateDirection.NextSibling
                => _owningListView.View == View.Tile ? _owingListViewItem?.SubItems[1].AccessibilityObject : null,
            _ => base.FragmentNavigate(direction)
        };
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms;

public partial class DataGridView
{
    internal class DataGridViewEditingPanelAccessibleObject : ControlAccessibleObject
    {
        private readonly WeakReference<DataGridView> _ownerDataGridView;
        private int[]? _runtimeId;

        public DataGridViewEditingPanelAccessibleObject(DataGridView dataGridView, Panel panel) : base(panel)
        {
            _ownerDataGridView = new(dataGridView);
        }

        internal override Rectangle BoundingRectangle
            => this.TryGetOwnerAs(out Panel? owner) ? owner.AccessibilityObject.Bounds : default;

        internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            => _ownerDataGridView.TryGetTarget(out var owner)
                ? owner.AccessibilityObject
                : UiaCore.StubFragmentRoot.Instance;

        internal override int[] RuntimeId
            => _runtimeId ??= this.TryGetOwnerAs(out Panel? owner) ? owner.AccessibilityObject.RuntimeId : base.RuntimeId;

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            if (!_ownerDataGridView.TryGetTarget(out var owner))
            {
                return null;
            }

            switch (direction)
            {
                case UiaCore.NavigateDirection.Parent:
                    DataGridViewCell currentCell = owner.CurrentCell;
                    if (currentCell is not null && owner.IsCurrentCellInEditMode)
                    {
                        return owner.AccessibilityObject;
                    }

                    break;
                case UiaCore.NavigateDirection.FirstChild:
                case UiaCore.NavigateDirection.LastChild:
                    return owner.EditingControlAccessibleObject;
            }

            return base.FragmentNavigate(direction);
        }

        public override string? Name => SR.DataGridView_AccEditingPanelAccName;

        internal override void SetFocus()
        {
            if (this.IsOwnerHandleCreated(out Panel? owner) && owner.CanFocus)
            {
                owner.Focus();
            }
        }

        internal override object? GetPropertyValue(UiaCore.UIA propertyId) =>
            propertyId switch
            {
                UiaCore.UIA.AccessKeyPropertyId => this.TryGetOwnerAs(out Panel? owner)
                    ? owner.AccessibilityObject.KeyboardShortcut
                    : null,
                UiaCore.UIA.ControlTypePropertyId => this.GetOwnerAccessibleRole() == AccessibleRole.Default
                    // If we don't set a default role for the accessible object it will be retrieved from Windows.
                    // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                    ? UiaCore.UIA.PaneControlTypeId
                    : base.GetPropertyValue(propertyId),
                UiaCore.UIA.HasKeyboardFocusPropertyId
                    => _ownerDataGridView.TryGetTarget(out var owner) && owner.CurrentCell is not null,
                UiaCore.UIA.IsContentElementPropertyId => true,
                UiaCore.UIA.IsControlElementPropertyId => true,
                UiaCore.UIA.IsEnabledPropertyId => _ownerDataGridView.TryGetTarget(out var owner) && owner.Enabled,
                UiaCore.UIA.IsKeyboardFocusablePropertyId => true,
                UiaCore.UIA.ProviderDescriptionPropertyId => SR.DataGridViewEditingPanelUiaProviderDescription,
                _ => base.GetPropertyValue(propertyId)
            };
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.TreeNode;

namespace System.Windows.Forms;

public partial class TreeView
{
    internal sealed class TreeViewAccessibleObject : ControlAccessibleObject
    {
        public TreeViewAccessibleObject(TreeView owningTreeView) : base(owningTreeView) { }

        internal override Rectangle BoundingRectangle => this.IsOwnerHandleCreated(out ListBox? owner) ?
            owner.GetToolNativeScreenRectangle() : Rectangle.Empty;

        internal override IRawElementProviderFragment.Interface? ElementProviderFromPoint(double x, double y)
            => HitTest((int)x, (int)y) ?? base.ElementProviderFromPoint(x, y);

        internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => this;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_FirstChild => GetChild(0),
                NavigateDirection.NavigateDirection_LastChild => GetChild(GetChildCount() - 1),
                _ => base.FragmentNavigate(direction),
            };

        public override AccessibleObject? GetChild(int index)
            => index >= 0 && index < GetChildCount() && this.TryGetOwnerAs(out TreeView? owningTreeView)
                ? owningTreeView.Nodes[index].AccessibilityObject
                : null;

        private protected override bool IsInternal => true;

        public override int GetChildCount() =>
            this.TryGetOwnerAs(out TreeView? owningTreeView) ? owningTreeView.Nodes.Count : base.GetChildCount();

        internal override int GetChildIndex(AccessibleObject? child)
            => child is TreeNodeAccessibleObject node ? node.Index : -1;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_TreeControlTypeId,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)(this.TryGetOwnerAs(out TreeView? owningTreeView)
                    && owningTreeView.Enabled && owningTreeView.Nodes.Count == 0),
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)(this.TryGetOwnerAs(out TreeView? owningTreeView) && owningTreeView.Enabled),
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focusable),
                _ => base.GetPropertyValue(propertyID)
            };

        public override AccessibleObject? HitTest(int x, int y)
        {
            if (!this.IsOwnerHandleCreated(out TreeView? owningTreeView))
            {
                return null;
            }

            Point p = owningTreeView.PointToClient(new Point(x, y));
            TreeNode? node = owningTreeView.GetNodeAt(p);

            if (node is not null)
            {
                return node.AccessibilityObject;
            }

            return Bounds.Contains(x, y) ? this : null;
        }

        public override AccessibleStates State
        {
            get
            {
                AccessibleStates state = AccessibleStates.Focusable;

                if (!this.TryGetOwnerAs(out TreeView? owningTreeView))
                {
                    return AccessibleStates.None;
                }

                if (owningTreeView.Focused)
                {
                    state |= AccessibleStates.Focused;
                }

                if (!owningTreeView.Enabled)
                {
                    state |= AccessibleStates.Unavailable;
                }

                return state;
            }
        }

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId => true,
                UIA_PATTERN_ID.UIA_SelectionPatternId => true,
                _ => base.IsPatternSupported(patternId),
            };

        #region Selection Pattern

        internal override bool IsSelectionRequired => this.TryGetOwnerAs(out TreeView? owningTreeView) &&
            owningTreeView.Nodes.Count != 0;

        internal override IRawElementProviderSimple.Interface[]? GetSelection()
            => this.IsOwnerHandleCreated(out TreeView? _) && GetSelected() is IRawElementProviderSimple.Interface selected
                ? [selected]
                : [];

        #endregion
    }
}

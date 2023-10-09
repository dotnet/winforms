﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static Interop;
using static Interop.UiaCore;
using static System.Windows.Forms.TreeNode;

namespace System.Windows.Forms;

public partial class TreeView
{
    internal class TreeViewAccessibleObject : ControlAccessibleObject
    {
        public TreeViewAccessibleObject(TreeView owningTreeView) : base(owningTreeView) { }

        internal override IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
            => HitTest((int)x, (int)y) ?? base.ElementProviderFromPoint(x, y);

        internal override IRawElementProviderFragmentRoot FragmentRoot => this;

        internal override IRawElementProviderFragment? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.FirstChild => GetChild(0),
                NavigateDirection.LastChild => GetChild(GetChildCount() - 1),
                _ => base.FragmentNavigate(direction),
            };

        public override AccessibleObject? GetChild(int index)
            => index >= 0 && index < GetChildCount() && this.TryGetOwnerAs(out TreeView? owningTreeView)
                ? owningTreeView.Nodes[index].AccessibilityObject
                : null;

        public override int GetChildCount() =>
            this.TryGetOwnerAs(out TreeView? owningTreeView) ? owningTreeView.Nodes.Count : base.GetChildCount();

        internal override int GetChildIndex(AccessibleObject? child)
            => child is TreeNodeAccessibleObject node ? node.Index : -1;

        internal override object? GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => UIA_CONTROLTYPE_ID.UIA_TreeControlTypeId,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => this.TryGetOwnerAs(out TreeView? owningTreeView)
                    && owningTreeView.Enabled && owningTreeView.Nodes.Count == 0,
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => this.TryGetOwnerAs(out TreeView? owningTreeView) && owningTreeView.Enabled,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
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

        internal override int[] RuntimeId
        {
            get
            {
                if (this.TryGetOwnerAs(out TreeView? owningTreeView))
                {
                    return new int[]
                    {
                        RuntimeIDFirstItem,
                        PARAM.ToInt(owningTreeView.InternalHandle),
                        owningTreeView.GetHashCode()
                    };
                }

                return base.RuntimeId;
            }
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

        internal override UiaCore.IRawElementProviderSimple[]? GetSelection()
        {
            if (this.IsOwnerHandleCreated(out TreeView? _) && GetSelected() is UiaCore.IRawElementProviderSimple selected)
            {
                return new[] { selected };
            }

            return Array.Empty<UiaCore.IRawElementProviderSimple>();
        }

        #endregion
    }
}

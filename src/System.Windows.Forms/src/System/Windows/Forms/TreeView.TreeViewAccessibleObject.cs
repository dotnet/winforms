// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;
using static Interop.UiaCore;
using static System.Windows.Forms.TreeNode;

namespace System.Windows.Forms
{
    public partial class TreeView
    {
        internal class TreeViewAccessibleObject : ControlAccessibleObject
        {
            private readonly TreeView _owningTreeView;

            public TreeViewAccessibleObject(TreeView owningTreeView) : base(owningTreeView)
            {
                _owningTreeView = owningTreeView;
            }

            internal override IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
                => HitTest((int)x, (int)y) ?? base.ElementProviderFromPoint(x, y);

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => this;

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.FirstChild => GetChild(0),
                    UiaCore.NavigateDirection.LastChild => GetChild(GetChildCount() - 1),
                    _ => base.FragmentNavigate(direction),
                };

            public override AccessibleObject? GetChild(int index)
                => index >= 0 && index < GetChildCount()
                    ? _owningTreeView.Nodes[index].AccessibilityObject
                    : null;

            public override int GetChildCount() => _owningTreeView.Nodes.Count;

            internal override int GetChildIndex(AccessibleObject? child)
                => child is TreeNodeAccessibleObject node ? node.Index : -1;

            internal override object? GetPropertyValue(UIA propertyID)
                => propertyID switch
                {
                    UIA.ControlTypePropertyId => UIA.TreeControlTypeId,
                    UIA.HasKeyboardFocusPropertyId => _owningTreeView.Enabled && _owningTreeView.Nodes.Count == 0,
                    UIA.IsEnabledPropertyId => _owningTreeView.Enabled,
                    UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                    _ => base.GetPropertyValue(propertyID)
                };

            public override AccessibleObject? HitTest(int x, int y)
            {
                if (!_owningTreeView.IsHandleCreated)
                {
                    return null;
                }

                Point p = _owningTreeView.PointToClient(new Point(x, y));
                TreeNode node = _owningTreeView.GetNodeAt(p);

                if (node is not null)
                {
                    return node.AccessibilityObject;
                }

                if (Bounds.Contains(x, y))
                {
                    return this;
                }

                return null;
            }

            internal override int[] RuntimeId
                => new int[]
                {
                    RuntimeIDFirstItem,
                    PARAM.ToInt(_owningTreeView.InternalHandle),
                    _owningTreeView.GetHashCode()
                };

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = AccessibleStates.Focusable;

                    if (_owningTreeView.Focused)
                    {
                        state |= AccessibleStates.Focused;
                    }

                    if (!_owningTreeView.Enabled)
                    {
                        state |= AccessibleStates.Unavailable;
                    }

                    return state;
                }
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    UiaCore.UIA.LegacyIAccessiblePatternId => true,
                    UiaCore.UIA.SelectionPatternId => true,
                    _ => base.IsPatternSupported(patternId),
                };

            #region Selection Pattern

            internal override bool IsSelectionRequired => _owningTreeView.Nodes.Count != 0;

            internal override UiaCore.IRawElementProviderSimple[]? GetSelection()
            {
                if (_owningTreeView.IsHandleCreated && GetSelected() is UiaCore.IRawElementProviderSimple selected)
                {
                    return new[] { selected };
                }

                return Array.Empty<UiaCore.IRawElementProviderSimple>();
            }

            #endregion
        }
    }
}

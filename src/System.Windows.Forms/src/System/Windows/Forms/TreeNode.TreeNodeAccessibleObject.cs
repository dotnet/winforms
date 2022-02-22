// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class TreeNode
    {
        internal class TreeNodeAccessibleObject : AccessibleObject
        {
            private readonly TreeNode _owningTreeNode;
            private readonly TreeView _owningTreeView;

            public TreeNodeAccessibleObject(TreeNode owningTreeNode, TreeView owningTreeView)
            {
                _owningTreeNode = owningTreeNode.OrThrowIfNull();
                _owningTreeView = owningTreeView.OrThrowIfNull();
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (!_owningTreeView.IsHandleCreated || !_owningTreeNode.IsVisible)
                    {
                        return Rectangle.Empty;
                    }

                    return _owningTreeNode.RectangleToScreen(_owningTreeNode.Bounds);
                }
            }

            public override string DefaultAction
            {
                get
                {
                    if (_owningTreeView.CheckBoxes)
                    {
                        return _owningTreeNode.Checked
                            ? SR.AccessibleActionUncheck
                            : SR.AccessibleActionCheck;
                    }

                    UiaCore.ExpandCollapseState expandCollapseState = ExpandCollapseState;
                    if (expandCollapseState == UiaCore.ExpandCollapseState.LeafNode)
                    {
                        return string.Empty;
                    }

                    return expandCollapseState == UiaCore.ExpandCollapseState.Expanded
                        ? SR.AccessibleActionCollapse
                        : SR.AccessibleActionExpand;
                }
            }

            public override void DoDefaultAction()
            {
                if (_owningTreeView.CheckBoxes)
                {
                    Toggle();
                }

                if (_owningTreeNode.IsExpanded)
                {
                    Collapse();
                }
                else
                {
                    Expand();
                }

                return;
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
                => _owningTreeView.AccessibilityObject;

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.Parent
                        => Parent ?? _owningTreeView.AccessibilityObject,
                    UiaCore.NavigateDirection.FirstChild
                        => _owningTreeNode.IsExpanded
                            ? _owningTreeNode.FirstNode?.AccessibilityObject
                            : null,
                    UiaCore.NavigateDirection.LastChild
                        => _owningTreeNode.IsExpanded
                            ? _owningTreeNode.LastNode?.AccessibilityObject
                            : null,
                    UiaCore.NavigateDirection.NextSibling
                        => _owningTreeNode.NextNode?.AccessibilityObject,
                    UiaCore.NavigateDirection.PreviousSibling
                        => _owningTreeNode.PrevNode?.AccessibilityObject,
                    _ => base.FragmentNavigate(direction),
                };

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId
                        => _owningTreeView.CheckBoxes
                            ? UiaCore.UIA.CheckBoxControlTypeId
                            : UiaCore.UIA.TreeItemControlTypeId,
                    UiaCore.UIA.IsEnabledPropertyId
                        => _owningTreeView.Enabled,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId
                        => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                    UiaCore.UIA.HasKeyboardFocusPropertyId
                        => (State & AccessibleStates.Focused) == AccessibleStates.Focused,
                    _ => base.GetPropertyValue(propertyID)
                };

            public override AccessibleObject? HitTest(int x, int y)
                => _owningTreeView.AccessibilityObject.HitTest(x, y);

            internal int Index => _owningTreeView.Nodes.IndexOf(_owningTreeNode);

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    UiaCore.UIA.ExpandCollapsePatternId => true,
                    UiaCore.UIA.LegacyIAccessiblePatternId => true,
                    UiaCore.UIA.ScrollItemPatternId => true,
                    UiaCore.UIA.SelectionItemPatternId => true,
                    UiaCore.UIA.TogglePatternId when _owningTreeView.CheckBoxes => true,
                    UiaCore.UIA.ValuePatternId when _owningTreeView.LabelEdit => true,
                    _ => base.IsPatternSupported(patternId),
                };

            public override string? Name => _owningTreeNode.Text;

            public override AccessibleObject? Parent => _owningTreeNode.Parent?.AccessibilityObject;

            public override AccessibleRole Role
                => _owningTreeView.CheckBoxes
                    ? AccessibleRole.CheckButton
                    : AccessibleRole.OutlineItem;

            internal override int[] RuntimeId
                => new int[]
                {
                    RuntimeIDFirstItem,
                    PARAM.ToInt(_owningTreeView.InternalHandle),
                    _owningTreeNode.GetHashCode()
                };

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable;

                    if (!_owningTreeNode.IsVisible)
                    {
                        state |= AccessibleStates.Invisible | AccessibleStates.Offscreen;
                    }

                    if (ExpandCollapseState == UiaCore.ExpandCollapseState.Expanded)
                    {
                        state |= AccessibleStates.Expanded;
                    }
                    else if (ExpandCollapseState == UiaCore.ExpandCollapseState.Collapsed)
                    {
                        state |= AccessibleStates.Collapsed;
                    }

                    if (_owningTreeNode.IsSelected)
                    {
                        state |= AccessibleStates.Focused | AccessibleStates.Selected;
                    }

                    if (!_owningTreeView.Enabled)
                    {
                        state |= AccessibleStates.Unavailable;
                    }

                    return state;
                }
            }

            #region Expand-Collapse Pattern

            internal override void Expand()
            {
                if (_owningTreeNode.Nodes.Count == 0)
                {
                    return;
                }

                _owningTreeNode.Expand();
            }

            internal override void Collapse()
            {
                if (_owningTreeNode.Nodes.Count == 0)
                {
                    return;
                }

                _owningTreeNode.Collapse();
            }

            internal override UiaCore.ExpandCollapseState ExpandCollapseState
            {
                get
                {
                    if (_owningTreeNode.Nodes.Count == 0)
                    {
                        return UiaCore.ExpandCollapseState.LeafNode;
                    }

                    return _owningTreeNode.IsExpanded
                        ? UiaCore.ExpandCollapseState.Expanded
                        : UiaCore.ExpandCollapseState.Collapsed;
                }
            }

            #endregion

            #region Scroll Item Pattern

            internal override void ScrollIntoView()
            {
                if (!_owningTreeView.IsHandleCreated || !_owningTreeView.Enabled)
                {
                    return;
                }

                // We don't need to scroll if the item is visible.
                if (_owningTreeNode.IsVisible)
                {
                    return;
                }

                _owningTreeView.TopNode = _owningTreeNode;
            }

            #endregion

            #region Selection Item Pattern

            internal override bool IsItemSelected => _owningTreeNode.IsSelected;

            internal override UiaCore.IRawElementProviderSimple? ItemSelectionContainer
                => _owningTreeView.AccessibilityObject;

            #endregion

            #region Toggle Pattern

            internal override void Toggle() => _owningTreeNode.Checked = !_owningTreeNode.Checked;

            internal override UiaCore.ToggleState ToggleState
                => _owningTreeNode.Checked ? UiaCore.ToggleState.On : UiaCore.ToggleState.Off;

            #endregion

            #region Value Pattern

            public override string? Value => _owningTreeNode.Text;

            #endregion
        }
    }
}

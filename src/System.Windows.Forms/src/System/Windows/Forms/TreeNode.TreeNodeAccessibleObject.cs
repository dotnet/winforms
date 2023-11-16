// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static Interop;

namespace System.Windows.Forms;

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

                ExpandCollapseState expandCollapseState = ExpandCollapseState;
                if (expandCollapseState == ExpandCollapseState.ExpandCollapseState_LeafNode)
                {
                    return string.Empty;
                }

                return expandCollapseState == ExpandCollapseState.ExpandCollapseState_Expanded
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

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_Parent
                    => Parent ?? _owningTreeView.AccessibilityObject,
                NavigateDirection.NavigateDirection_FirstChild
                     => _owningTreeNode.IsEditing
                        ? _owningTreeView._labelEdit?.AccessibilityObject
                        : _owningTreeNode.IsExpanded
                            ? _owningTreeNode.FirstNode?.AccessibilityObject
                            : null,
                NavigateDirection.NavigateDirection_LastChild
                    => _owningTreeNode.IsExpanded
                        ? _owningTreeNode.LastNode?.AccessibilityObject
                        : null,
                NavigateDirection.NavigateDirection_NextSibling
                    => _owningTreeNode.NextNode?.AccessibilityObject,
                NavigateDirection.NavigateDirection_PreviousSibling
                    => _owningTreeNode.PrevNode?.AccessibilityObject,
                _ => base.FragmentNavigate(direction),
            };

        internal override void SelectItem()
        {
            _owningTreeView.SelectedNode = _owningTreeNode;
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_TreeItemControlTypeId,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focused),
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)_owningTreeView.Enabled,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focusable),
                _ => base.GetPropertyValue(propertyID)
            };

        public override AccessibleObject? HitTest(int x, int y)
            => _owningTreeView.AccessibilityObject.HitTest(x, y);

        internal int Index => _owningTreeView.Nodes.IndexOf(_owningTreeNode);

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_ExpandCollapsePatternId => _owningTreeNode._childNodes.Count > 0,
                UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId => true,
                UIA_PATTERN_ID.UIA_ScrollItemPatternId => true,
                UIA_PATTERN_ID.UIA_SelectionItemPatternId => true,
                UIA_PATTERN_ID.UIA_TogglePatternId when _owningTreeView.CheckBoxes => true,
                UIA_PATTERN_ID.UIA_ValuePatternId when _owningTreeView.LabelEdit => true,
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

                if (ExpandCollapseState == ExpandCollapseState.ExpandCollapseState_Expanded)
                {
                    state |= AccessibleStates.Expanded;
                }
                else if (ExpandCollapseState == ExpandCollapseState.ExpandCollapseState_Collapsed)
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

        internal override ExpandCollapseState ExpandCollapseState
        {
            get
            {
                if (_owningTreeNode.Nodes.Count == 0)
                {
                    return ExpandCollapseState.ExpandCollapseState_LeafNode;
                }

                return _owningTreeNode.IsExpanded
                    ? ExpandCollapseState.ExpandCollapseState_Expanded
                    : ExpandCollapseState.ExpandCollapseState_Collapsed;
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

        internal override IRawElementProviderSimple.Interface? ItemSelectionContainer
            => _owningTreeView.AccessibilityObject;

        #endregion

        #region Toggle Pattern

        internal override void Toggle() => _owningTreeNode.Checked = !_owningTreeNode.Checked;

        internal override ToggleState ToggleState
            => _owningTreeNode.Checked ? ToggleState.ToggleState_On : ToggleState.ToggleState_Off;

        #endregion

        #region Value Pattern

        public override string? Value => _owningTreeNode.Text;

        #endregion
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.TestUtilities;
using Xunit;
using static System.Windows.Forms.TreeNode;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class TreeNodeAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void TreeNodeAccessibleObject_Ctor_Default()
        {
            using TreeView control = new();
            TreeNode node = new(control);

            Assert.NotNull(node.AccessibilityObject);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_Ctor_ThrowsException_IfOwningNodeIsNull()
        {
            using TreeView control = new();

            Assert.Throws<ArgumentNullException>(() => new TreeNodeAccessibleObject(null, control));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_Ctor_ThrowsException_IfOwningTreeIsNull()
        {
            TreeNode node = new();

            Assert.Throws<ArgumentNullException>(() => new TreeNodeAccessibleObject(node, null));
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_DefaultAction_IsEmptyString_IfNodeIsLeaf()
        {
            using TreeView control = new() { CheckBoxes = false };
            TreeNode node = new(control);

            Assert.Empty(node.AccessibilityObject.DefaultAction);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TreeNodeAccessibleObject_DefaultAction_ReturnsExpected_IfNodeIsParent(bool isExpanded)
        {
            using TreeView control = new() { CheckBoxes = false };
            TreeNode node = new TreeNode("Root node", new TreeNode[] { new() });
            control.Nodes.Add(node);
            if (isExpanded)
            {
                node.Expand();
            }

            string expected = isExpanded ? SR.AccessibleActionCollapse : SR.AccessibleActionExpand;

            Assert.Equal(expected, node.AccessibilityObject.DefaultAction);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TreeNodeAccessibleObject_DefaultAction_ReturnsExpected_IfNodesAreCheckBoxes(bool isChecked)
        {
            using TreeView control = new() { CheckBoxes = true };
            TreeNode node = new(control) { Checked = isChecked };

            string expected = isChecked ? SR.AccessibleActionUncheck : SR.AccessibleActionCheck;

            Assert.Equal(expected, node.AccessibilityObject.DefaultAction);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TreeNodeAccessibleObject_DoDefaultAction_ToggleNodeCheckBox_IfNodesAreCheckBoxes(bool isChecked)
        {
            using TreeView control = new() { CheckBoxes = true };
            TreeNode node = new(control) { Checked = isChecked };

            Assert.Equal(isChecked, node.Checked);

            node.AccessibilityObject.DoDefaultAction();

            Assert.Equal(!isChecked, node.Checked);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TreeNodeAccessibleObject_DoDefaultAction_ExpandOrCollapse_IfNodeIsNotLeaf(bool isExpanded)
        {
            using TreeView control = new();
            TreeNode node = new TreeNode("Root node", new TreeNode[] { new() });
            control.Nodes.Add(node);
            if (isExpanded)
            {
                node.Expand();
            }

            Assert.Equal(isExpanded, node.IsExpanded);

            node.AccessibilityObject.DoDefaultAction();

            Assert.Equal(!isExpanded, node.IsExpanded);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_FragmentRoot_ReturnsTree()
        {
            using TreeView control = new();
            TreeNode node = new(control);

            Assert.Equal(control.AccessibilityObject, node.AccessibilityObject.FragmentRoot);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_FragmentNavigate_Parent_ReturnsTree()
        {
            using TreeView control = new();
            TreeNode node = new(control);

            var actual = (AccessibleObject)node.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.Parent);

            Assert.Equal(control.AccessibilityObject, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_FragmentNavigate_Parent_ReturnsParent()
        {
            using TreeView control = new();
            TreeNode node = new();
            control.Nodes.Add(new TreeNode("Root node", new[] { node }));

            var actual = (AccessibleObject)node.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.Parent);

            Assert.Equal(node.Parent.AccessibilityObject, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TreeNodeAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected(bool isExpanded)
        {
            using TreeView control = new();
            TreeNode node = new TreeNode("Root node", new TreeNode[] { new(), new(), new() });
            control.Nodes.Add(node);
            if (isExpanded)
            {
                node.Expand();
            }

            // If node is collapsed, child is not visible, so returns null instead of child ao.
            AccessibleObject expected = isExpanded ? node.FirstNode?.AccessibilityObject : null;

            Assert.Equal(expected, node.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TreeNodeAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected(bool isExpanded)
        {
            using TreeView control = new();
            TreeNode node = new TreeNode("Root node", new TreeNode[] { new(), new(), new() });
            control.Nodes.Add(node);
            if (isExpanded)
            {
                node.Expand();
            }

            // If node is collapsed, child is not visible, so returns null instead of child ao.
            AccessibleObject expected = isExpanded ? node.LastNode?.AccessibilityObject : null;

            Assert.Equal(expected, node.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected()
        {
            using TreeView control = new();
            control.Nodes.AddRange(new TreeNode[] { new(), new(), new() });

            AccessibleObject accessibleObject1 = control.Nodes[0].AccessibilityObject;
            AccessibleObject accessibleObject2 = control.Nodes[1].AccessibilityObject;
            AccessibleObject accessibleObject3 = control.Nodes[2].AccessibilityObject;

            Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected()
        {
            using TreeView control = new();
            control.Nodes.AddRange(new TreeNode[] { new(), new(), new() });

            AccessibleObject accessibleObject1 = control.Nodes[0].AccessibilityObject;
            AccessibleObject accessibleObject2 = control.Nodes[1].AccessibilityObject;
            AccessibleObject accessibleObject3 = control.Nodes[2].AccessibilityObject;

            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_GetPropertyValue_ControlType_IsCheckBox_IfNodesAreCheckBoxes()
        {
            using TreeView control = new() { CheckBoxes = true };
            TreeNode node = new(control);

            UiaCore.UIA expected = UiaCore.UIA.CheckBoxControlTypeId;

            Assert.Equal(expected, node.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_GetPropertyValue_ControlType_IsTreeItem_IfNodesAreNotCheckBoxes()
        {
            using TreeView control = new() { CheckBoxes = false };
            TreeNode node = new(control);

            UiaCore.UIA expected = UiaCore.UIA.TreeItemControlTypeId;

            Assert.Equal(expected, node.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.ExpandCollapsePatternId)]
        [InlineData((int)UiaCore.UIA.LegacyIAccessiblePatternId)]
        [InlineData((int)UiaCore.UIA.ScrollItemPatternId)]
        [InlineData((int)UiaCore.UIA.SelectionItemPatternId)]
        public void TreeNodeAccessibleObject_IsPatternSupported_IfCommonNodes(int patternId)
        {
            using TreeView control = new();
            TreeNode node = new(control);

            Assert.True(node.AccessibilityObject.IsPatternSupported((UiaCore.UIA)patternId));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.ExpandCollapsePatternId)]
        [InlineData((int)UiaCore.UIA.LegacyIAccessiblePatternId)]
        [InlineData((int)UiaCore.UIA.ScrollItemPatternId)]
        [InlineData((int)UiaCore.UIA.SelectionItemPatternId)]
        [InlineData((int)UiaCore.UIA.TogglePatternId)]
        public void TreeNodeAccessibleObject_IsPatternSupported_IfNodesAreCheckBoxes(int patternId)
        {
            using TreeView control = new() { CheckBoxes = true };
            TreeNode node = new(control);

            Assert.True(node.AccessibilityObject.IsPatternSupported((UiaCore.UIA)patternId));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.ExpandCollapsePatternId)]
        [InlineData((int)UiaCore.UIA.LegacyIAccessiblePatternId)]
        [InlineData((int)UiaCore.UIA.ScrollItemPatternId)]
        [InlineData((int)UiaCore.UIA.SelectionItemPatternId)]
        [InlineData((int)UiaCore.UIA.ValuePatternId)]
        public void TreeNodeAccessibleObject_IsPatternSupported_IfNodesAreEditable(int patternId)
        {
            using TreeView control = new() { LabelEdit = true };
            TreeNode node = new(control);

            Assert.True(node.AccessibilityObject.IsPatternSupported((UiaCore.UIA)patternId));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_Index_ReturnsExpected()
        {
            using TreeView control = new();
            control.Nodes.AddRange(new TreeNode[] { new(), new(), new() });

            TreeNodeAccessibleObject accessibleObject1 = control.Nodes[0].AccessibilityObject;
            TreeNodeAccessibleObject accessibleObject2 = control.Nodes[1].AccessibilityObject;
            TreeNodeAccessibleObject accessibleObject3 = control.Nodes[2].AccessibilityObject;

            Assert.Equal(0, accessibleObject1.Index);
            Assert.Equal(1, accessibleObject2.Index);
            Assert.Equal(2, accessibleObject3.Index);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_Name_EqualsText()
        {
            using TreeView control = new();
            string testText = "This is test string for Text property of TreeNode.";
            TreeNode node = new(control) { Text = testText };

            Assert.Equal(testText, node.AccessibilityObject.Name);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_Parent_ReturnsNull_IfNodeHasNoParent()
        {
            using TreeView control = new();
            TreeNode node = new(control);

            Assert.Null(node.AccessibilityObject.Parent);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_Parent_ReturnsParent()
        {
            using TreeView control = new();
            TreeNode node = new();

            control.Nodes.Add(new TreeNode("Root node", new[] { node }));

            Assert.Equal(node.Parent.AccessibilityObject, node.AccessibilityObject.Parent);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_Role_IsOutlineItem()
        {
            using TreeView control = new();
            TreeNode node = new(control);

            Assert.Equal(AccessibleRole.OutlineItem, node.AccessibilityObject.Role);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_Role_IsCheckButton_IfNodesAreCheckBoxes()
        {
            using TreeView control = new() { CheckBoxes = true };
            TreeNode node = new(control);

            Assert.Equal(AccessibleRole.CheckButton, node.AccessibilityObject.Role);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_Expand_WorksExpected_IfNodeIsCollapsed()
        {
            using TreeView control = new();
            TreeNode node = new("Root node", new TreeNode[] { new() });

            control.Nodes.Add(node);

            Assert.False(node.IsExpanded);

            node.AccessibilityObject.Expand();

            Assert.True(node.IsExpanded);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_Expand_DoesNothing_IfNodeHasNoChild()
        {
            using TreeView control = new();
            TreeNode node = new();

            control.Nodes.Add(node);

            Assert.False(node.IsExpanded);

            node.AccessibilityObject.Expand();

            Assert.False(node.IsExpanded);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_Collapse_WorksExpected_IfNodeIsExpanded()
        {
            using TreeView control = new();
            TreeNode node = new("Root node", new TreeNode[] { new() });

            control.Nodes.Add(node);

            node.Expand();
            Assert.True(node.IsExpanded);

            node.AccessibilityObject.Collapse();

            Assert.False(node.IsExpanded);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_ExpandCollapseState_ReturnsLeafNode_IfNodeHasNoChild()
        {
            using TreeView control = new();
            TreeNode node = new(control);

            Assert.Equal(UiaCore.ExpandCollapseState.LeafNode, node.AccessibilityObject.ExpandCollapseState);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TreeNodeAccessibleObject_ExpandCollapseState_ReturnsExpected(bool isExpanded)
        {
            using TreeView control = new();
            TreeNode node = new("Root node", new TreeNode[] { new() });

            control.Nodes.Add(node);

            if (isExpanded)
            {
                node.Expand();
            }

            UiaCore.ExpandCollapseState expected = isExpanded
                    ? UiaCore.ExpandCollapseState.Expanded
                    : UiaCore.ExpandCollapseState.Collapsed;

            Assert.Equal(expected, node.AccessibilityObject.ExpandCollapseState);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_ItemSelectionContainer_ReturnsExpected()
        {
            using TreeView control = new();
            TreeNode node = new(control);

            Assert.Equal(control.AccessibilityObject, node.AccessibilityObject.ItemSelectionContainer);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TreeNodeAccessibleObject_Toggle_WorksExpected(bool isChecked)
        {
            using TreeView control = new() { CheckBoxes = true };
            TreeNode node = new(control) { Checked = isChecked };

            Assert.Equal(isChecked, node.Checked);

            node.AccessibilityObject.Toggle();

            Assert.Equal(!isChecked, node.Checked);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TreeNodeAccessibleObject_ToggleState_WorksExpected(bool isChecked)
        {
            using TreeView control = new() { CheckBoxes = true };
            TreeNode node = new(control) { Checked = isChecked };

            UiaCore.ToggleState expected = isChecked ? UiaCore.ToggleState.On : UiaCore.ToggleState.Off;

            Assert.Equal(expected, node.AccessibilityObject.ToggleState);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeNodeAccessibleObject_Value_EqualsText()
        {
            using TreeView control = new() { LabelEdit = true };
            string testText = "This is test string for Text property of TreeNode.";
            TreeNode node = new(control) { Text = testText };

            Assert.Equal(testText, node.AccessibilityObject.Value);
            Assert.False(control.IsHandleCreated);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms.TestUtilities;

namespace System.Windows.Forms.Tests;

public class TreeNodeTests
{
    [WinFormsFact]
    public void TreeNode_Ctor_Default()
    {
        TreeNode node = new();
        Assert.Equal(Color.Empty, node.BackColor);
        Assert.Equal(Rectangle.Empty, node.Bounds);
        Assert.False(node.Checked);
        Assert.Null(node.ContextMenuStrip);
        Assert.Null(node.FirstNode);
        Assert.Equal(Color.Empty, node.ForeColor);
        Assert.Throws<InvalidOperationException>(() => node.FullPath);
        Assert.Equal(-1, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.Equal(0, node.Index);
        Assert.False(node.IsEditing);
        Assert.False(node.IsExpanded);
        Assert.False(node.IsSelected);
        Assert.False(node.IsVisible);
        Assert.Null(node.LastNode);
        Assert.Equal(0, node.Level);
        Assert.Empty(node.Name);
        Assert.Null(node.NextNode);
        Assert.Null(node.NextVisibleNode);
        Assert.Null(node.NodeFont);
        Assert.Empty(node.Nodes);
        Assert.Same(node.Nodes, node.Nodes);
        Assert.Null(node.Parent);
        Assert.Null(node.PrevNode);
        Assert.Null(node.PrevVisibleNode);
        Assert.Equal(-1, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.Equal(-1, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.Null(node.Tag);
        Assert.Empty(node.Text);
        Assert.Empty(node.ToolTipText);
        Assert.Null(node.TreeView);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TreeNode_Ctor_String(string text, string expectedText)
    {
        TreeNode node = new(text);
        Assert.Equal(Color.Empty, node.BackColor);
        Assert.Equal(Rectangle.Empty, node.Bounds);
        Assert.False(node.Checked);
        Assert.Null(node.ContextMenuStrip);
        Assert.Null(node.FirstNode);
        Assert.Equal(Color.Empty, node.ForeColor);
        Assert.Throws<InvalidOperationException>(() => node.FullPath);
        Assert.Equal(-1, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.Equal(0, node.Index);
        Assert.False(node.IsEditing);
        Assert.False(node.IsExpanded);
        Assert.False(node.IsSelected);
        Assert.False(node.IsVisible);
        Assert.Null(node.LastNode);
        Assert.Equal(0, node.Level);
        Assert.Empty(node.Name);
        Assert.Null(node.NextNode);
        Assert.Null(node.NextVisibleNode);
        Assert.Null(node.NodeFont);
        Assert.Empty(node.Nodes);
        Assert.Same(node.Nodes, node.Nodes);
        Assert.Null(node.Parent);
        Assert.Null(node.PrevNode);
        Assert.Null(node.PrevVisibleNode);
        Assert.Equal(-1, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.Equal(-1, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.Null(node.Tag);
        Assert.Equal(expectedText, node.Text);
        Assert.Empty(node.ToolTipText);
        Assert.Null(node.TreeView);
    }

    [WinFormsFact]
    public void TreeNode_Ctor_String_TreeNodeArray()
    {
        // Work around: We cannot serialize type System.Windows.Forms.TreeNode[] because it lives in the GAC.
        TreeNode_Ctor_String_TreeNodeArray_Helper(null, Array.Empty<TreeNode>(), string.Empty);
        TreeNode_Ctor_String_TreeNodeArray_Helper(string.Empty, Array.Empty<TreeNode>(), string.Empty);
        TreeNode_Ctor_String_TreeNodeArray_Helper("text", [new()], "text");
        TreeNode_Ctor_String_TreeNodeArray_Helper("text", [new(), new("text")], "text");
    }

    private void TreeNode_Ctor_String_TreeNodeArray_Helper(string text, TreeNode[] children, string expectedText)
    {
        TreeNode node = new(text, children);
        Assert.Equal(Color.Empty, node.BackColor);
        Assert.Equal(Rectangle.Empty, node.Bounds);
        Assert.False(node.Checked);
        Assert.Null(node.ContextMenuStrip);
        Assert.Same(children.FirstOrDefault(), node.FirstNode);
        Assert.Equal(Color.Empty, node.ForeColor);
        Assert.Throws<InvalidOperationException>(() => node.FullPath);
        Assert.Equal(-1, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.Equal(0, node.Index);
        Assert.False(node.IsEditing);
        Assert.False(node.IsExpanded);
        Assert.False(node.IsSelected);
        Assert.False(node.IsVisible);
        Assert.Same(children.LastOrDefault(), node.LastNode);
        Assert.Equal(0, node.Level);
        Assert.Empty(node.Name);
        Assert.Null(node.NextNode);
        Assert.Null(node.NextVisibleNode);
        Assert.Null(node.NodeFont);
        Assert.Equal(children, node.Nodes.Cast<TreeNode>());
        Assert.Same(node.Nodes, node.Nodes);
        Assert.Null(node.Parent);
        Assert.Null(node.PrevNode);
        Assert.Null(node.PrevVisibleNode);
        Assert.Equal(-1, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.Equal(-1, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.Null(node.Tag);
        Assert.Equal(expectedText, node.Text);
        Assert.Empty(node.ToolTipText);
        Assert.Null(node.TreeView);
    }

    [WinFormsTheory]
    [InlineData(null, -1, -1, "")]
    [InlineData(null, 0, 0, "")]
    [InlineData("text", 1, 1, "text")]
    [InlineData("text", 1, 14, "text")]
    public void TreeNode_Ctor_String_Int_Int(string text, int imageIndex, int selectedImageIndex, string expectedText)
    {
        TreeNode node = new(text, imageIndex, selectedImageIndex);
        Assert.Equal(Color.Empty, node.BackColor);
        Assert.Equal(Rectangle.Empty, node.Bounds);
        Assert.False(node.Checked);
        Assert.Null(node.ContextMenuStrip);
        Assert.Null(node.FirstNode);
        Assert.Equal(Color.Empty, node.ForeColor);
        Assert.Throws<InvalidOperationException>(() => node.FullPath);
        Assert.Equal(imageIndex, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.Equal(0, node.Index);
        Assert.False(node.IsEditing);
        Assert.False(node.IsExpanded);
        Assert.False(node.IsSelected);
        Assert.False(node.IsVisible);
        Assert.Null(node.LastNode);
        Assert.Equal(0, node.Level);
        Assert.Empty(node.Name);
        Assert.Null(node.NextNode);
        Assert.Null(node.NextVisibleNode);
        Assert.Null(node.NodeFont);
        Assert.Empty(node.Nodes);
        Assert.Same(node.Nodes, node.Nodes);
        Assert.Null(node.Parent);
        Assert.Null(node.PrevNode);
        Assert.Null(node.PrevVisibleNode);
        Assert.Equal(selectedImageIndex, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.Equal(-1, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.Null(node.Tag);
        Assert.Equal(expectedText, node.Text);
        Assert.Empty(node.ToolTipText);
        Assert.Null(node.TreeView);
    }

    [WinFormsFact]
    public void TreeNode_Ctor_String_Int_Int_TreeNodeArray()
    {
        // Work around: We cannot serialize type System.Windows.Forms.TreeNode[] because it lives in the GAC.
        TreeNode_Ctor_String_Int_Int_TreeNodeArray_Helper(null, -1, -1, Array.Empty<TreeNode>(), "");
        TreeNode_Ctor_String_Int_Int_TreeNodeArray_Helper(null, 0, 0, Array.Empty<TreeNode>(), "");
        TreeNode_Ctor_String_Int_Int_TreeNodeArray_Helper("text", 1, 1, [new()], "text");
        TreeNode_Ctor_String_Int_Int_TreeNodeArray_Helper("text", 1, 14, [new(), new("text")], "text");
    }

    private void TreeNode_Ctor_String_Int_Int_TreeNodeArray_Helper(string text, int imageIndex, int selectedImageIndex, TreeNode[] children, string expectedText)
    {
        TreeNode node = new(text, imageIndex, selectedImageIndex, children);
        Assert.Equal(Color.Empty, node.BackColor);
        Assert.Equal(Rectangle.Empty, node.Bounds);
        Assert.False(node.Checked);
        Assert.Null(node.ContextMenuStrip);
        Assert.Same(children.FirstOrDefault(), node.FirstNode);
        Assert.Equal(Color.Empty, node.ForeColor);
        Assert.Throws<InvalidOperationException>(() => node.FullPath);
        Assert.Equal(imageIndex, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.Equal(0, node.Index);
        Assert.False(node.IsEditing);
        Assert.False(node.IsExpanded);
        Assert.False(node.IsSelected);
        Assert.False(node.IsVisible);
        Assert.Same(children.LastOrDefault(), node.LastNode);
        Assert.Equal(0, node.Level);
        Assert.Empty(node.Name);
        Assert.Null(node.NextNode);
        Assert.Null(node.NextVisibleNode);
        Assert.Null(node.NodeFont);
        Assert.Equal(children, node.Nodes.Cast<TreeNode>());
        Assert.Same(node.Nodes, node.Nodes);
        Assert.Null(node.Parent);
        Assert.Null(node.PrevNode);
        Assert.Null(node.PrevVisibleNode);
        Assert.Equal(selectedImageIndex, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.Equal(-1, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.Null(node.Tag);
        Assert.Equal(expectedText, node.Text);
        Assert.Empty(node.ToolTipText);
        Assert.Null(node.TreeView);
    }

    [WinFormsFact]
    public void TreeNode_Ctor_NullChildren_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("nodes", () => new TreeNode("text", null));
        Assert.Throws<ArgumentNullException>("nodes", () => new TreeNode("text", 0, 0, null));
    }

    [WinFormsFact]
    public void TreeNode_Ctor_NullValueInChildren_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("node", () => new TreeNode("text", [null]));
        Assert.Throws<ArgumentNullException>("node", () => new TreeNode("text", 0, 0, [null]));
    }

    [WinFormsTheory]
    [InlineData(-3)]
    public void TreeNode_Ctor_InvalidImageIndex_ThrowsArgumentOutOfRangeException(int imageIndex)
    {
        Assert.Throws<ArgumentOutOfRangeException>("value", () => new TreeNode("text", imageIndex, 0));
        Assert.Throws<ArgumentOutOfRangeException>("value", () => new TreeNode("text", imageIndex, 0, Array.Empty<TreeNode>()));
    }

    [WinFormsTheory]
    [InlineData(-3)]
    public void TreeNode_Ctor_InvalidSelectedImageIndex_ThrowsArgumentOutOfRangeException(int selectedImageIndex)
    {
        Assert.Throws<ArgumentOutOfRangeException>("value", () => new TreeNode("text", 0, selectedImageIndex));
        Assert.Throws<ArgumentOutOfRangeException>("value", () => new TreeNode("text", 0, selectedImageIndex, Array.Empty<TreeNode>()));
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
    public void TreeNode_BackColor_Set_GetReturnsExpected(Color value)
    {
        TreeNode node = new()
        {
            BackColor = value
        };
        Assert.Equal(value, node.BackColor);

        // Set same.
        node.BackColor = value;
        Assert.Equal(value, node.BackColor);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
    public void TreeNode_BackColor_SetWithTreeView_GetReturnsExpected(Color value)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        node.BackColor = value;
        Assert.Equal(value, node.BackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.BackColor = value;
        Assert.Equal(value, node.BackColor);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> BackColor_SetWithTreeViewWithHandle_TestData()
    {
        yield return new object[] { Color.Empty, 0 };
        yield return new object[] { Color.Red, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_SetWithTreeViewWithHandle_TestData))]
    public void TreeNode_BackColor_SetWithTreeViewWithHandle_GetReturnsExpected(Color value, int expectedInvalidatedCallCount)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        node.BackColor = value;
        Assert.Equal(value, node.BackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        node.BackColor = value;
        Assert.Equal(value, node.BackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> BackColor_SetWithCustomOldValue_TestData()
    {
        yield return new object[] { Color.Empty };
        yield return new object[] { Color.Blue };
        yield return new object[] { Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_SetWithCustomOldValue_TestData))]
    public void TreeNode_BackColor_SetWithCustomOldValue_GetReturnsExpected(Color value)
    {
        TreeNode node = new()
        {
            BackColor = Color.Blue
        };

        node.BackColor = value;
        Assert.Equal(value, node.BackColor);

        // Set same.
        node.BackColor = value;
        Assert.Equal(value, node.BackColor);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
    public void TreeNode_BackColor_SetWithCustomOldValueWithTreeView_GetReturnsExpected(Color value)
    {
        using TreeView control = new();
        TreeNode node = new()
        {
            BackColor = Color.Blue
        };
        control.Nodes.Add(node);

        node.BackColor = value;
        Assert.Equal(value, node.BackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.BackColor = value;
        Assert.Equal(value, node.BackColor);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> BackColor_SetWithCustomOldValueWithTreeViewWithHandle_TestData()
    {
        yield return new object[] { Color.Empty, 1 };
        yield return new object[] { Color.Red, 1 };
        yield return new object[] { Color.Blue, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_SetWithCustomOldValueWithTreeViewWithHandle_TestData))]
    public void TreeNode_BackColor_SetWithCustomOldValueWithTreeViewWithHandle_GetReturnsExpected(Color value, int expectedInvalidatedCallCount)
    {
        using TreeView control = new();
        TreeNode node = new()
        {
            BackColor = Color.Blue
        };
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        node.BackColor = value;
        Assert.Equal(value, node.BackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        node.BackColor = value;
        Assert.Equal(value, node.BackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TreeNode_BackColor_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TreeNode))[nameof(TreeNode.BackColor)];
        TreeNode node = new();
        Assert.False(property.CanResetValue(node));

        node.BackColor = Color.Red;
        Assert.Equal(Color.Red, node.BackColor);
        Assert.False(property.CanResetValue(node));

        property.ResetValue(node);
        Assert.Equal(Color.Red, node.BackColor);
        Assert.False(property.CanResetValue(node));
    }

    [WinFormsFact]
    public void TreeNode_BackColor_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TreeNode))[nameof(TreeNode.BackColor)];
        TreeNode node = new();
        Assert.False(property.ShouldSerializeValue(node));

        node.BackColor = Color.Red;
        Assert.Equal(Color.Red, node.BackColor);
        Assert.True(property.ShouldSerializeValue(node));

        property.ResetValue(node);
        Assert.Equal(Color.Red, node.BackColor);
        Assert.True(property.ShouldSerializeValue(node));
    }

    [WinFormsFact]
    public void TreeNode_Bounds_GetWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        Rectangle bounds = node.Bounds;
        Assert.True(bounds.X > 0);
        Assert.Equal(0, bounds.Y);
        Assert.True(bounds.Width > 0);
        Assert.True(bounds.Height > 0);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_Bounds_GetWithTreeViewWithHandle_Success()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Rectangle bounds = node.Bounds;
        Assert.True(bounds.X > 0);
        Assert.Equal(0, bounds.Y);
        Assert.True(bounds.Width > 0);
        Assert.True(bounds.Height > 0);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Bounds_Get_CustomGetItemRect_TestData()
    {
        yield return new object[] { default(RECT), Rectangle.Empty };
        yield return new object[] { new RECT(1, 2, 3, 4), new Rectangle(1, 2, 2, 2) };

        yield return new object[] { new RECT(0, 1, 3, 4), new Rectangle(0, 1, 3, 3) };
        yield return new object[] { new RECT(1, 0, 3, 4), new Rectangle(1, 0, 2, 4) };
        yield return new object[] { new RECT(1, 2, 0, 4), new Rectangle(1, 2, -1, 2) };
        yield return new object[] { new RECT(1, 2, 3, 0), new Rectangle(1, 2, 2, -2) };
        yield return new object[] { new RECT(0, 0, 3, 4), new Rectangle(0, 0, 3, 4) };

        yield return new object[] { new RECT(-1, 0, 3, 4), new Rectangle(-1, 0, 4, 4) };
        yield return new object[] { new RECT(1, -2, 3, 4), new Rectangle(1, -2, 2, 6) };
        yield return new object[] { new RECT(1, 2, -3, 4), new Rectangle(1, 2, -4, 2) };
        yield return new object[] { new RECT(1, 2, 3, -4), new Rectangle(1, 2, 2, -6) };
        yield return new object[] { new RECT(-3, -4, -1, -2), new Rectangle(-3, -4, 2, 2) };

        yield return new object[] { new RECT(1, 118, 3, 4), new Rectangle(1, 118, 2, -114) };
        yield return new object[] { new RECT(1, 117, 3, 4), new Rectangle(1, 117, 2, -113) };
        yield return new object[] { new RECT(94, 2, 3, 4), new Rectangle(94, 2, -91, 2) };
        yield return new object[] { new RECT(93, 2, 3, 4), new Rectangle(93, 2, -90, 2) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Bounds_Get_CustomGetItemRect_TestData))]
    public void TreeNode_Bounds_GetCustomGetItemRect_ReturnsExpected(object getItemRectResult, Rectangle expected)
    {
        using CustomGetItemRectTreeView control = new()
        {
            GetItemRectResult = (RECT)getItemRectResult
        };
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        Assert.Equal(expected, node.Bounds);
    }

    [WinFormsFact]
    public void TreeNode_Bounds_GetInvalidGetItemRect_ReturnsExpected()
    {
        using InvalidGetItemRectTreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.MakeInvalid = true;

        Assert.Equal(Rectangle.Empty, node.Bounds);
    }

    [WinFormsFact]
    public void TreeNode_Bounds_GetWithTreeViewDisposed_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.Dispose();
        Assert.Equal(Rectangle.Empty, node.Bounds);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void TreeNode_Checked_Set_GetReturnsExpected(bool value)
    {
        TreeNode node = new()
        {
            Checked = value
        };
        Assert.Equal(value, node.Checked);

        // Set same
        node.Checked = value;
        Assert.Equal(value, node.Checked);

        // Set different
        node.Checked = !value;
        Assert.Equal(!value, node.Checked);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void TreeNode_Checked_SetWithTreeView_GetReturnsExpected(bool checkBoxes, bool value)
    {
        using TreeView control = new()
        {
            CheckBoxes = checkBoxes
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        node.Checked = value;
        Assert.Equal(value, node.Checked);
        Assert.False(control.IsHandleCreated);

        // Set same
        node.Checked = value;
        Assert.Equal(value, node.Checked);
        Assert.False(control.IsHandleCreated);

        // Set different
        node.Checked = !value;
        Assert.Equal(!value, node.Checked);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void TreeNode_Checked_SetWithTreeViewWithHandle_GetReturnsExpected(bool checkBoxes, bool value)
    {
        using TreeView control = new()
        {
            CheckBoxes = checkBoxes
        };
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        node.Checked = value;
        Assert.Equal(value, node.Checked);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same
        node.Checked = value;
        Assert.Equal(value, node.Checked);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different
        node.Checked = !value;
        Assert.Equal(!value, node.Checked);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, true, 8192)]
    [InlineData(true, false, 4096)]
    [InlineData(false, true, 8192)]
    [InlineData(false, false, 4096)]
    public void TreeNode_Checked_GetItemState_ReturnsExpected(bool checkBoxes, bool @checked, int expectedValue)
    {
        using TreeView control = new()
        {
            CheckBoxes = checkBoxes
        };
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        node.Checked = @checked;
        TVITEMW item = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_STATE,
            stateMask = TREE_VIEW_ITEM_STATE_FLAGS.TVIS_STATEIMAGEMASK,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref item));
        Assert.Equal((TREE_VIEW_ITEM_STATE_FLAGS)expectedValue, item.state);
    }

    [WinFormsTheory]
    [BoolData]
    public void TreeNode_Checked_SetWithTreeViewDisposed_GetReturnsExpected(bool value)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.Dispose();

        node.Checked = value;
        Assert.Equal(value, node.Checked);
        Assert.False(control.IsHandleCreated);

        // Set same
        node.Checked = value;
        Assert.Equal(value, node.Checked);
        Assert.False(control.IsHandleCreated);

        // Set different
        node.Checked = !value;
        Assert.Equal(!value, node.Checked);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
    public void TreeNode_ForeColor_Set_GetReturnsExpected(Color value)
    {
        TreeNode node = new()
        {
            ForeColor = value
        };
        Assert.Equal(value, node.ForeColor);

        // Set same.
        node.ForeColor = value;
        Assert.Equal(value, node.ForeColor);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
    public void TreeNode_ForeColor_SetWithTreeView_GetReturnsExpected(Color value)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        node.ForeColor = value;
        Assert.Equal(value, node.ForeColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.ForeColor = value;
        Assert.Equal(value, node.ForeColor);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ForeColor_SetWithTreeViewWithHandle_TestData()
    {
        yield return new object[] { Color.Empty, 0 };
        yield return new object[] { Color.Red, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_SetWithTreeViewWithHandle_TestData))]
    public void TreeNode_ForeColor_SetWithTreeViewWithHandle_GetReturnsExpected(Color value, int expectedInvalidatedCallCount)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        node.ForeColor = value;
        Assert.Equal(value, node.ForeColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        node.ForeColor = value;
        Assert.Equal(value, node.ForeColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> ForeColor_SetWithCustomOldValue_TestData()
    {
        yield return new object[] { Color.Empty };
        yield return new object[] { Color.Blue };
        yield return new object[] { Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_SetWithCustomOldValue_TestData))]
    public void TreeNode_ForeColor_SetWithCustomOldValue_GetReturnsExpected(Color value)
    {
        TreeNode node = new()
        {
            ForeColor = Color.Blue
        };

        node.ForeColor = value;
        Assert.Equal(value, node.ForeColor);

        // Set same.
        node.ForeColor = value;
        Assert.Equal(value, node.ForeColor);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
    public void TreeNode_ForeColor_SetWithCustomOldValueWithTreeView_GetReturnsExpected(Color value)
    {
        using TreeView control = new();
        TreeNode node = new()
        {
            ForeColor = Color.Blue
        };
        control.Nodes.Add(node);

        node.ForeColor = value;
        Assert.Equal(value, node.ForeColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.ForeColor = value;
        Assert.Equal(value, node.ForeColor);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ForeColor_SetWithCustomOldValueWithTreeViewWithHandle_TestData()
    {
        yield return new object[] { Color.Empty, 1 };
        yield return new object[] { Color.Red, 1 };
        yield return new object[] { Color.Blue, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_SetWithCustomOldValueWithTreeViewWithHandle_TestData))]
    public void TreeNode_ForeColor_SetWithCustomOldValueWithTreeViewWithHandle_GetReturnsExpected(Color value, int expectedInvalidatedCallCount)
    {
        using TreeView control = new();
        TreeNode node = new()
        {
            ForeColor = Color.Blue
        };
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        node.ForeColor = value;
        Assert.Equal(value, node.ForeColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        node.ForeColor = value;
        Assert.Equal(value, node.ForeColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TreeNode_ForeColor_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TreeNode))[nameof(TreeNode.ForeColor)];
        TreeNode node = new();
        Assert.False(property.CanResetValue(node));

        node.ForeColor = Color.Red;
        Assert.Equal(Color.Red, node.ForeColor);
        Assert.False(property.CanResetValue(node));

        property.ResetValue(node);
        Assert.Equal(Color.Red, node.ForeColor);
        Assert.False(property.CanResetValue(node));
    }

    [WinFormsFact]
    public void TreeNode_ForeColor_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TreeNode))[nameof(TreeNode.ForeColor)];
        TreeNode node = new();
        Assert.False(property.ShouldSerializeValue(node));

        node.ForeColor = Color.Red;
        Assert.Equal(Color.Red, node.ForeColor);
        Assert.True(property.ShouldSerializeValue(node));

        property.ResetValue(node);
        Assert.Equal(Color.Red, node.ForeColor);
        Assert.True(property.ShouldSerializeValue(node));
    }

    [WinFormsFact]
    public void TreeNode_Handle_GetWithoutTreeView_ReturnsExpected()
    {
        TreeNode node = new();
        IntPtr handle = node.Handle;
        Assert.Equal(IntPtr.Zero, handle);
        Assert.Equal(handle, node.Handle);
    }

    [WinFormsFact]
    public void TreeNode_Handle_GetWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        IntPtr handle = node.Handle;
        Assert.NotEqual(IntPtr.Zero, handle);
        Assert.Equal(handle, node.Handle);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true, 8192)]
    [InlineData(true, false, 4096)]
    [InlineData(false, true, 0)]
    [InlineData(false, false, 0)]
    public void TreeNode_Handle_GetChecked_ReturnsExpected(bool checkBoxes, bool @checked, int expectedValue)
    {
        using TreeView control = new()
        {
            CheckBoxes = checkBoxes
        };
        TreeNode node = new()
        {
            Checked = @checked
        };
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        TVITEMW item = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_STATE,
            stateMask = TREE_VIEW_ITEM_STATE_FLAGS.TVIS_STATEIMAGEMASK,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref item));
        Assert.Equal((TREE_VIEW_ITEM_STATE_FLAGS)expectedValue, item.state);
    }

    [WinFormsFact]
    public void TreeNode_Handle_GetWithTreeViewDisposed_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.Dispose();
        IntPtr handle = node.Handle;
        Assert.NotEqual(IntPtr.Zero, handle);
        Assert.Equal(handle, node.Handle);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> TreeNode_ImageIndex_TestData()
    {
        // { image index, expected index without image, expected index with image }
        yield return new object[] { -2, -2, -2 };
        yield return new object[] { -1, -1, -1 };
        yield return new object[] { 0, -1, 0 };
        yield return new object[] { 1, -1, 0 };
        yield return new object[] { 2, -1, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(TreeNode_ImageIndex_TestData))]
    public void TreeNode_ImageIndex_SetWithoutTreeView_GetReturnsExpected(int value, int expectedWithoutImage, int expectedWithImage)
    {
        TreeNode node = new()
        {
            ImageIndex = value
        };
        Assert.Equal(value, node.ImageIndex);
        Assert.Empty(node.ImageKey);

        // Set same.
        node.ImageIndex = value;
        Assert.Equal(value, node.ImageIndex);
        Assert.Empty(node.ImageKey);

        // Set tree view.
        using TreeView control = new();
        control.Nodes.Add(node);
        Assert.Equal(value, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.False(control.IsHandleCreated);

        // Add image list.
        using ImageList imageList = new();
        control.ImageList = imageList;
        Assert.Equal(expectedWithoutImage, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.False(control.IsHandleCreated);

        // Add to image list.
        using Bitmap image = new(10, 10);
        imageList.Images.Add(image);
        Assert.Equal(expectedWithImage, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(TreeNode_ImageIndex_TestData))]
    public void TreeNode_ImageIndex_SetWithImageKey_GetReturnsExpected(int value, int expectedWithoutImage, int expectedWithImage)
    {
        TreeNode node = new()
        {
            ImageKey = "ImageKey",
            ImageIndex = value
        };
        Assert.Equal(value, node.ImageIndex);
        Assert.Equal(ImageList.Indexer.DefaultKey, node.ImageKey);

        // Set same.
        node.ImageIndex = value;
        Assert.Equal(value, node.ImageIndex);
        Assert.Equal(ImageList.Indexer.DefaultKey, node.ImageKey);

        // Set tree view.
        using TreeView control = new();
        control.Nodes.Add(node);
        Assert.Equal(value, node.ImageIndex);
        Assert.Equal(ImageList.Indexer.DefaultKey, node.ImageKey);
        Assert.False(control.IsHandleCreated);

        // Add image list.
        using ImageList imageList = new();
        control.ImageList = imageList;
        Assert.Equal(expectedWithoutImage, node.ImageIndex);
        Assert.Equal(ImageList.Indexer.DefaultKey, node.ImageKey);
        Assert.False(control.IsHandleCreated);

        // Add to image list.
        using Bitmap image = new(10, 10);
        imageList.Images.Add(image);
        Assert.Equal(expectedWithImage, node.ImageIndex);
        Assert.Equal(ImageList.Indexer.DefaultKey, node.ImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(TreeNode_ImageIndex_TestData))]
    public void TreeNode_ImageIndex_SetWithTreeView_GetReturnsExpected(int value, int expectedWithoutImage, int expectedWithImage)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        node.ImageIndex = value;
        Assert.Equal(value, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.ImageIndex = value;
        Assert.Equal(value, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.False(control.IsHandleCreated);

        // Add image list.
        using ImageList imageList = new();
        control.ImageList = imageList;
        Assert.Equal(expectedWithoutImage, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.False(control.IsHandleCreated);

        // Add to image list.
        using Bitmap image = new(10, 10);
        imageList.Images.Add(image);
        Assert.Equal(expectedWithImage, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(TreeNode_ImageIndex_TestData))]
    public void TreeNode_ImageIndex_SetWithTreeViewWithEmptyList_GetReturnsExpected(int value, int expectedWithoutImage, int expectedWithImage)
    {
        using ImageList imageList = new();
        using TreeView control = new()
        {
            ImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        node.ImageIndex = value;
        Assert.Equal(expectedWithoutImage, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.ImageIndex = value;
        Assert.Equal(expectedWithoutImage, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.False(control.IsHandleCreated);

        // Add to image list.
        using Bitmap image = new(10, 10);
        imageList.Images.Add(image);
        Assert.Equal(expectedWithImage, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-2, -2)]
    [InlineData(-1, -1)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    public void TreeNode_ImageIndex_SetWithTreeViewWithNotEmptyList_GetReturnsExpected(int value, int expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image1);
        imageList.Images.Add(image2);
        using TreeView control = new()
        {
            ImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        node.ImageIndex = value;
        Assert.Equal(expected, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.ImageIndex = value;
        Assert.Equal(expected, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(TreeNode_ImageIndex_TestData))]
    public void TreeNode_ImageIndex_SetWithTreeViewWithHandle_GetReturnsExpected(int value, int expectedWithoutImage, int expectedWithImage)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        node.ImageIndex = value;
        Assert.Equal(value, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        node.ImageIndex = value;
        Assert.Equal(value, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add image list.
        using ImageList imageList = new();
        control.ImageList = imageList;
        Assert.Equal(expectedWithoutImage, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add to image list.
        using Bitmap image = new(10, 10);
        imageList.Images.Add(image);
        Assert.Equal(expectedWithImage, node.ImageIndex);
        Assert.Empty(node.ImageKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-2, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    public void TreeNode_ImageIndex_GetItemWithoutImageList_Success(int value, int expected)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.ImageIndex = value;
        TVITEMW item = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_IMAGE,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref item));
        Assert.Equal(expected, item.iImage);
    }

    [WinFormsTheory]
    [InlineData(-2, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    public void TreeNode_ImageIndex_GetItemWithEmptyImageList_Success(int value, int expected)
    {
        using ImageList imageList = new();
        using TreeView control = new()
        {
            ImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.ImageIndex = value;
        TVITEMW item = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_IMAGE,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref item));
        Assert.Equal(expected, item.iImage);
    }

    [WinFormsTheory]
    [InlineData(-2, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    public void TreeNode_ImageIndex_GetItemWithImageList_Success(int value, int expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image1);
        imageList.Images.Add(image2);
        using TreeView control = new()
        {
            ImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.ImageIndex = value;
        TVITEMW item = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_IMAGE,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref item));
        Assert.Equal(expected, item.iImage);
    }

    [WinFormsTheory]
    [InlineData(-2)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void TreeNode_ImageIndex_SetWithTreeViewDisposed_GetReturnsExpected(int value)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.Dispose();

        node.ImageIndex = value;
        Assert.Equal(value, node.ImageIndex);
        Assert.False(control.IsHandleCreated);

        // Set same
        node.ImageIndex = value;
        Assert.Equal(value, node.ImageIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-3)]
    public void TreeNode_ImageIndex_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
    {
        TreeNode node = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => node.ImageIndex = value);
    }

    [WinFormsFact]
    public void TreeNode_ImageIndex_SetInvalidSetItem_ThrowsInvalidOperationException()
    {
        using InvalidSetItemTreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.MakeInvalid = true;
        node.ImageIndex = 0;
        Assert.Equal(0, node.ImageIndex);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TreeNode_ImageKey_SetWithoutTreeView_GetReturnsExpected(string value, string expected)
    {
        TreeNode node = new()
        {
            ImageKey = value
        };
        Assert.Equal(expected, node.ImageKey);
        Assert.Equal(-1, node.ImageIndex);

        // Set same.
        node.ImageKey = value;
        Assert.Equal(expected, node.ImageKey);
        Assert.Equal(-1, node.ImageIndex);
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("ImageKey", "ImageKey")]
    public void TreeNode_ImageKey_SetWithImageIndex_GetReturnsExpected(string value, string expectedImageKey)
    {
        TreeNode node = new()
        {
            ImageIndex = 0,
            ImageKey = value
        };
        Assert.Equal(expectedImageKey, node.ImageKey);
        Assert.Equal(ImageList.Indexer.DefaultIndex, node.ImageIndex);

        // Set same.
        node.ImageKey = value;
        Assert.Equal(expectedImageKey, node.ImageKey);
        Assert.Equal(ImageList.Indexer.DefaultIndex, node.ImageIndex);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TreeNode_ImageKey_SetWithTreeView_GetReturnsExpected(string value, string expected)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        node.ImageKey = value;
        Assert.Equal(expected, node.ImageKey);
        Assert.Equal(-1, node.ImageIndex);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.ImageKey = value;
        Assert.Equal(expected, node.ImageKey);
        Assert.Equal(-1, node.ImageIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TreeNode_ImageKey_SetWithTreeViewWithEmptyList_GetReturnsExpected(string value, string expected)
    {
        using ImageList imageList = new();
        using TreeView control = new()
        {
            ImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        node.ImageKey = value;
        Assert.Equal(expected, node.ImageKey);
        Assert.Equal(-1, node.ImageIndex);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.ImageKey = value;
        Assert.Equal(expected, node.ImageKey);
        Assert.Equal(-1, node.ImageIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Image1", "Image1")]
    [InlineData("image1", "image1")]
    [InlineData("Image2", "Image2")]
    [InlineData("NoSuchImage", "NoSuchImage")]
    public void TreeNode_ImageKey_SetWithTreeViewWithNotEmptyList_GetReturnsExpected(string value, string expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add("Image1", image1);
        imageList.Images.Add("Image2", image2);
        using TreeView control = new()
        {
            ImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        node.ImageKey = value;
        Assert.Equal(expected, node.ImageKey);
        Assert.Equal(-1, node.ImageIndex);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.ImageKey = value;
        Assert.Equal(expected, node.ImageKey);
        Assert.Equal(-1, node.ImageIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TreeNode_ImageKey_SetWithTreeViewWithHandle_GetReturnsExpected(string value, string expected)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        node.ImageKey = value;
        Assert.Equal(expected, node.ImageKey);
        Assert.Equal(-1, node.ImageIndex);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        node.ImageKey = value;
        Assert.Equal(expected, node.ImageKey);
        Assert.Equal(-1, node.ImageIndex);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Image1")]
    [InlineData("image1")]
    [InlineData("Image2")]
    [InlineData("NoSuchImage")]
    public void TreeNode_ImageKey_GetItemWithoutImageList_Success(string value)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.ImageKey = value;
        TVITEMW column = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_IMAGE,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref column));
        Assert.Equal(0, column.iImage);
    }

    [WinFormsTheory]
    [InlineData(null, 0)]
    [InlineData("", 0)]
    [InlineData("Image1", 0)]
    [InlineData("image1", 0)]
    [InlineData("Image2", 0)]
    [InlineData("NoSuchImage", 0)]
    public void TreeNode_ImageKey_GetItemWithEmptyImageList_Success(string value, int expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        using TreeView control = new()
        {
            ImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.ImageKey = value;
        TVITEMW column = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_IMAGE,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref column));
        Assert.Equal(expected, column.iImage);
    }

    [WinFormsTheory]
    [InlineData(null, 0)]
    [InlineData("", 0)]
    [InlineData("Image1", 0)]
    [InlineData("image1", 0)]
    [InlineData("Image2", 1)]
    [InlineData("NoSuchImage", 0)]
    public void TreeNode_ImageKey_GetItemWithImageList_Success(string value, int expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add("Image1", image1);
        imageList.Images.Add("Image2", image2);
        using TreeView control = new()
        {
            ImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.ImageKey = value;
        TVITEMW column = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_IMAGE,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref column));
        Assert.Equal(expected, column.iImage);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TreeNode_ImageKey_SetWithTreeViewDisposed_GetReturnsExpected(string value, string expected)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.Dispose();

        node.ImageKey = value;
        Assert.Equal(expected, node.ImageKey);
        Assert.False(control.IsHandleCreated);

        // Set same
        node.ImageKey = value;
        Assert.Equal(expected, node.ImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_ImageKey_SetInvalidSetItem_ThrowsInvalidOperationException()
    {
        using InvalidSetItemTreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.MakeInvalid = true;
        node.ImageKey = "Key";
        Assert.Equal("Key", node.ImageKey);
    }

    [WinFormsFact]
    public void TreeNode_Index_GetWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.Equal(0, node.Index);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_Index_GetWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0, node.Index);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TreeView_Index_GetWithParent_ReturnsExpected()
    {
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);
        Assert.Equal(0, node1.Index);
        Assert.Equal(1, node2.Index);
        Assert.Equal(2, node3.Index);
        Assert.Equal(0, node4.Index);
    }

    [WinFormsFact]
    public void TreeView_Index_GetWithParentWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        control.Nodes.Add(parent);
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);

        Assert.Equal(0, node1.Index);
        Assert.Equal(1, node2.Index);
        Assert.Equal(2, node3.Index);
        Assert.Equal(0, node4.Index);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeView_Index_GetWithParentWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        control.Nodes.Add(parent);
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0, node1.Index);
        Assert.Equal(1, node2.Index);
        Assert.Equal(2, node3.Index);
        Assert.Equal(0, node4.Index);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TreeNode_IsEditing_GetWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.False(node.IsEditing);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_IsEditing_GetWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.False(node.IsEditing);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TreeNode_IsEditing_GetWithTreeViewDisposed_ReturnsFalse()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.Dispose();
        Assert.False(node.IsEditing);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_IsExpanded_GetWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.False(node.IsExpanded);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_IsExpanded_GetWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.False(node.IsExpanded);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0, false)]
    [InlineData((int)TREE_VIEW_ITEM_STATE_FLAGS.TVIS_BOLD, false)]
    [InlineData((int)TREE_VIEW_ITEM_STATE_FLAGS.TVIS_EXPANDED, true)]
    [InlineData((int)(TREE_VIEW_ITEM_STATE_FLAGS.TVIS_EXPANDED | TREE_VIEW_ITEM_STATE_FLAGS.TVIS_BOLD), true)]
    [InlineData((int)TREE_VIEW_ITEM_STATE_FLAGS.TVIS_EXPANDEDONCE, false)]
    public void TreeNode_IsExpanded_GetCustomGetItem_ReturnsExpected(int getItemStateResult, bool expected)
    {
        using CustomGetItemTreeView control = new()
        {
            GetItemStateResult = (TREE_VIEW_ITEM_STATE_FLAGS)getItemStateResult
        };
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        Assert.Equal(expected, node.IsExpanded);
    }

    [WinFormsTheory]
    [InlineData(0, false)]
    [InlineData((int)TREE_VIEW_ITEM_STATE_FLAGS.TVIS_BOLD, false)]
    [InlineData((int)TREE_VIEW_ITEM_STATE_FLAGS.TVIS_EXPANDED, true)]
    [InlineData((int)(TREE_VIEW_ITEM_STATE_FLAGS.TVIS_EXPANDED | TREE_VIEW_ITEM_STATE_FLAGS.TVIS_BOLD), true)]
    [InlineData((int)TREE_VIEW_ITEM_STATE_FLAGS.TVIS_EXPANDEDONCE, false)]
    public void TreeNode_IsExpanded_GetInvalidGetItem_ReturnsExpected(int getItemStateResult, bool expected)
    {
        using InvalidGetItemTreeView control = new()
        {
            GetItemStateResult = (TREE_VIEW_ITEM_STATE_FLAGS)getItemStateResult
        };
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.MakeInvalid = true;
        Assert.Equal(expected, node.IsExpanded);
    }

    [WinFormsFact]
    public void TreeNode_IsExpanded_GetWithTreeViewDisposed_ReturnsFalse()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.Dispose();
        Assert.False(node.IsExpanded);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_IsSelected_GetWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.False(node.IsSelected);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_IsSelected_GetWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.False(node.IsSelected);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0, false)]
    [InlineData((int)TREE_VIEW_ITEM_STATE_FLAGS.TVIS_BOLD, false)]
    [InlineData((int)TREE_VIEW_ITEM_STATE_FLAGS.TVIS_SELECTED, true)]
    [InlineData((int)(TREE_VIEW_ITEM_STATE_FLAGS.TVIS_SELECTED | TREE_VIEW_ITEM_STATE_FLAGS.TVIS_BOLD), true)]
    public void TreeNode_IsSelected_GetCustomGetItem_ReturnsExpected(int getItemStateResult, bool expected)
    {
        using CustomGetItemTreeView control = new()
        {
            GetItemStateResult = (TREE_VIEW_ITEM_STATE_FLAGS)getItemStateResult
        };
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        Assert.Equal(expected, node.IsSelected);
    }

    [WinFormsTheory]
    [InlineData(0, false)]
    [InlineData((int)TREE_VIEW_ITEM_STATE_FLAGS.TVIS_BOLD, false)]
    [InlineData((int)TREE_VIEW_ITEM_STATE_FLAGS.TVIS_SELECTED, true)]
    [InlineData((int)(TREE_VIEW_ITEM_STATE_FLAGS.TVIS_SELECTED | TREE_VIEW_ITEM_STATE_FLAGS.TVIS_BOLD), true)]
    public void TreeNode_IsSelected_GetInvalidGetItem_ReturnsExpected(int getItemStateResult, bool expected)
    {
        using InvalidGetItemTreeView control = new()
        {
            GetItemStateResult = (TREE_VIEW_ITEM_STATE_FLAGS)getItemStateResult
        };
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.MakeInvalid = true;
        Assert.Equal(expected, node.IsSelected);
    }

    [WinFormsFact]
    public void TreeNode_IsSelected_GetWithTreeViewDisposed_ReturnsFalse()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.Dispose();
        Assert.False(node.IsSelected);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_IsVisible_GetWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.False(node.IsVisible);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_IsVisible_GetWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.True(node.IsVisible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> IsVisible_Get_CustomGetItemRect_TestData()
    {
        yield return new object[] { default(RECT), false };
        yield return new object[] { new RECT(1, 2, 3, 4), true };

        yield return new object[] { new RECT(0, 1, 3, 4), true };
        yield return new object[] { new RECT(1, 0, 3, 4), true };
        yield return new object[] { new RECT(1, 2, 0, 4), false };
        yield return new object[] { new RECT(1, 2, 3, 0), false };
        yield return new object[] { new RECT(0, 0, 3, 4), true };

        yield return new object[] { new RECT(-1, 0, 3, 4), true };
        yield return new object[] { new RECT(1, -2, 3, 4), true };
        yield return new object[] { new RECT(1, 2, -3, 4), false };
        yield return new object[] { new RECT(1, 2, 3, -4), false };
        yield return new object[] { new RECT(-3, -4, -1, -2), false };

        yield return new object[] { new RECT(1, 118, 3, 4), false };
        yield return new object[] { new RECT(1, 117, 3, 4), false };
        yield return new object[] { new RECT(94, 2, 3, 4), true };
        yield return new object[] { new RECT(93, 2, 3, 4), true };
    }

    [WinFormsTheory]
    [MemberData(nameof(IsVisible_Get_CustomGetItemRect_TestData))]
    public void TreeNode_IsVisible_GetCustomGetItemRect_ReturnsExpected(object getItemRectResult, bool expected)
    {
        using CustomGetItemRectTreeView control = new()
        {
            GetItemRectResult = (RECT)getItemRectResult
        };
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        Assert.Equal(expected, node.IsVisible);
    }

    [WinFormsFact]
    public void TreeNode_IsVisible_GetInvalidGetItemRect_ReturnsExpected()
    {
        using InvalidGetItemRectTreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.MakeInvalid = true;

        Assert.False(node.IsVisible);
    }

    [WinFormsFact]
    public void TreeNode_IsVisible_GetWithTreeViewDisposed_ReturnsFalse()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.Dispose();
        Assert.False(node.IsVisible);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_Level_GetWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.Equal(0, node.Level);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_Level_GetWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0, node.Level);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TreeView_Level_GetWithParent_ReturnsExpected()
    {
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);
        Assert.Equal(1, node1.Level);
        Assert.Equal(1, node2.Level);
        Assert.Equal(1, node3.Level);
        Assert.Equal(2, node4.Level);
    }

    [WinFormsFact]
    public void TreeView_Level_GetWithParentWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        control.Nodes.Add(parent);
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);

        Assert.Equal(1, node1.Level);
        Assert.Equal(1, node2.Level);
        Assert.Equal(1, node3.Level);
        Assert.Equal(2, node4.Level);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeView_Level_GetWithParentWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        control.Nodes.Add(parent);
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(1, node1.Level);
        Assert.Equal(1, node2.Level);
        Assert.Equal(1, node3.Level);
        Assert.Equal(2, node4.Level);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TreeNode_Name_Set_GetReturnsExpected(string value, string expected)
    {
        TreeNode node = new()
        {
            Name = value
        };
        Assert.Equal(expected, node.Name);

        // Set same.
        node.Name = value;
        Assert.Equal(expected, node.Name);
    }

    [WinFormsFact]
    public void TreeNode_NextNode_GetWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.Null(node.NextNode);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_NextNode_GetWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Null(node.NextNode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TreeView_NextNode_GetWithParent_ReturnsExpected()
    {
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);
        Assert.Equal(node2, node1.NextNode);
        Assert.Equal(node3, node2.NextNode);
        Assert.Null(node3.NextNode);
        Assert.Null(node4.NextNode);
    }

    [WinFormsFact]
    public void TreeView_NextNode_GetWithParentWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        control.Nodes.Add(parent);
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);

        Assert.Equal(node2, node1.NextNode);
        Assert.Equal(node3, node2.NextNode);
        Assert.Null(node3.NextNode);
        Assert.Null(node4.NextNode);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeView_NextNode_GetWithParentWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        control.Nodes.Add(parent);
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(node2, node1.NextNode);
        Assert.Equal(node3, node2.NextNode);
        Assert.Null(node3.NextNode);
        Assert.Null(node4.NextNode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TreeNode_NextVisibleNode_GetWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.Null(node.NextVisibleNode);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_NextVisibleNode_GetWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Null(node.NextVisibleNode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TreeNode_NextVisibleNode_GetWithTreeViewDisposed_ReturnsFalse()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.Dispose();
        Assert.Null(node.NextVisibleNode);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeView_NextVisibleNode_GetWithParent_ReturnsExpected()
    {
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);
        Assert.Null(node1.NextVisibleNode);
        Assert.Null(node2.NextVisibleNode);
        Assert.Null(node3.NextVisibleNode);
        Assert.Null(node4.NextVisibleNode);
    }

    [WinFormsFact]
    public void TreeView_NextVisibleNode_GetWithParentWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        control.Nodes.Add(parent);
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);

        Assert.Null(node1.NextVisibleNode);
        Assert.Null(node2.NextVisibleNode);
        Assert.Null(node3.NextVisibleNode);
        Assert.Null(node4.NextVisibleNode);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeView_NextVisibleNode_GetWithParentWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        control.Nodes.Add(parent);
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Null(node1.NextVisibleNode);
        Assert.Null(node2.NextVisibleNode);
        Assert.Null(node3.NextVisibleNode);
        Assert.Null(node4.NextVisibleNode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TreeNode_NextVisibleNode_GetCustomGetNextItem_ReturnsExpected()
    {
        using CustomGetNextItemTreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.MakeCustom = true;
        control.GetNextItemResult = node.Handle;
        try
        {
            Assert.Same(node, node.NextVisibleNode);
        }
        finally
        {
            control.MakeCustom = false;
        }
    }

    public static IEnumerable<object[]> NextVisibleNode_InvalidGetNextItem_TestData()
    {
        yield return new object[] { IntPtr.Zero };
        yield return new object[] { (IntPtr)1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(NextVisibleNode_InvalidGetNextItem_TestData))]
    public void TreeNode_NextVisibleNode_GetInvalidGetNextItem_ReturnsExpected(IntPtr getNextItemResult)
    {
        using CustomGetNextItemTreeView control = new()
        {
            GetNextItemResult = getNextItemResult
        };
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.MakeCustom = true;
        try
        {
            Assert.Null(node.NextVisibleNode);
        }
        finally
        {
            control.MakeCustom = false;
        }
    }

    private class CustomGetNextItemTreeView : TreeView
    {
        public IntPtr GetNextItemResult { get; set; }
        public bool MakeCustom { get; set; }

        protected override void WndProc(ref Message m)
        {
            if (MakeCustom && m.Msg == (int)PInvoke.TVM_GETNEXTITEM)
            {
                Assert.Equal((IntPtr)PInvoke.TVGN_NEXTVISIBLE, m.WParam);
                Assert.NotEqual(IntPtr.Zero, m.LParam);
                m.Result = GetNextItemResult;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsFact]
    public void TreeNode_Parent_GetWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.Null(node.Parent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_Parent_GetWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Null(node.Parent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TreeView_Parent_GetWithParent_ReturnsExpected()
    {
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);
        Assert.Equal(parent, node1.Parent);
        Assert.Equal(parent, node2.Parent);
        Assert.Equal(parent, node3.Parent);
        Assert.Equal(node3, node4.Parent);
    }

    [WinFormsFact]
    public void TreeView_Parent_GetWithParentWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        control.Nodes.Add(parent);
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);

        Assert.Equal(parent, node1.Parent);
        Assert.Equal(parent, node2.Parent);
        Assert.Equal(parent, node3.Parent);
        Assert.Equal(node3, node4.Parent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeView_Parent_GetWithParentWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        control.Nodes.Add(parent);
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(parent, node1.Parent);
        Assert.Equal(parent, node2.Parent);
        Assert.Equal(parent, node3.Parent);
        Assert.Equal(node3, node4.Parent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TreeNode_PrevNode_GetWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.Null(node.PrevNode);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_PrevNode_GetWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Null(node.PrevNode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TreeView_PrevNode_GetWithParent_ReturnsExpected()
    {
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);
        Assert.Null(node1.PrevNode);
        Assert.Equal(node1, node2.PrevNode);
        Assert.Equal(node2, node3.PrevNode);
        Assert.Null(node4.PrevNode);
    }

    [WinFormsFact]
    public void TreeView_PrevNode_GetWithParentWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        control.Nodes.Add(parent);
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);

        Assert.Null(node1.PrevNode);
        Assert.Equal(node1, node2.PrevNode);
        Assert.Equal(node2, node3.PrevNode);
        Assert.Null(node4.PrevNode);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeView_PrevNode_GetWithParentWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        control.Nodes.Add(parent);
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Null(node1.PrevNode);
        Assert.Equal(node1, node2.PrevNode);
        Assert.Equal(node2, node3.PrevNode);
        Assert.Null(node4.PrevNode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TreeNode_PrevVisibleNode_GetWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.Null(node.PrevVisibleNode);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_PrevVisibleNode_GetWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Null(node.PrevVisibleNode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TreeNode_PrevVisibleNode_GetWithTreeViewDisposed_ReturnsFalse()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.Dispose();
        Assert.Null(node.PrevVisibleNode);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeView_PrevVisibleNode_GetWithParent_ReturnsExpected()
    {
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);
        Assert.Null(node1.PrevVisibleNode);
        Assert.Null(node2.PrevVisibleNode);
        Assert.Null(node3.PrevVisibleNode);
        Assert.Null(node4.PrevVisibleNode);
    }

    [WinFormsFact]
    public void TreeView_PrevVisibleNode_GetWithParentWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        control.Nodes.Add(parent);
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);

        Assert.Null(node1.PrevVisibleNode);
        Assert.Null(node2.PrevVisibleNode);
        Assert.Null(node3.PrevVisibleNode);
        Assert.Null(node4.PrevVisibleNode);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeView_PrevVisibleNode_GetWithParentWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        control.Nodes.Add(parent);
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Null(node1.PrevVisibleNode);
        Assert.Null(node2.PrevVisibleNode);
        Assert.Null(node3.PrevVisibleNode);
        Assert.Null(node4.PrevVisibleNode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TreeNode_PrevVisibleNode_GetCustomGetPreviousItem_ReturnsExpected()
    {
        using CustomGetPreviousItemTreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.MakeCustom = true;
        control.GetPreviousItemResult = node.Handle;
        try
        {
            Assert.Same(node, node.PrevVisibleNode);
        }
        finally
        {
            control.MakeCustom = false;
        }
    }

    public static IEnumerable<object[]> PrevVisibleNode_InvalidGetPreviousItemResult_TestData()
    {
        yield return new object[] { IntPtr.Zero };
        yield return new object[] { (IntPtr)1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(PrevVisibleNode_InvalidGetPreviousItemResult_TestData))]
    public void TreeNode_PrevVisibleNode_InvalidGetPreviousItemResult_ReturnsExpected(IntPtr getPreviousItemResult)
    {
        using CustomGetPreviousItemTreeView control = new()
        {
            GetPreviousItemResult = getPreviousItemResult
        };
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.MakeCustom = true;
        try
        {
            Assert.Null(node.PrevVisibleNode);
        }
        finally
        {
            control.MakeCustom = false;
        }
    }

    private class CustomGetPreviousItemTreeView : TreeView
    {
        public IntPtr GetPreviousItemResult { get; set; }
        public bool MakeCustom { get; set; }

        protected override void WndProc(ref Message m)
        {
            if (MakeCustom && m.Msg == (int)PInvoke.TVM_GETNEXTITEM)
            {
                Assert.Equal((IntPtr)PInvoke.TVGN_PREVIOUSVISIBLE, m.WParam);
                Assert.NotEqual(IntPtr.Zero, m.LParam);
                m.Result = GetPreviousItemResult;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void TreeNode_NodeFont_Set_GetReturnsExpected(Font value)
    {
        TreeNode node = new()
        {
            NodeFont = value
        };
        Assert.Equal(value, node.NodeFont);

        // Set same.
        node.NodeFont = value;
        Assert.Equal(value, node.NodeFont);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void TreeNode_NodeFont_SetWithTreeView_GetReturnsExpected(Font value)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        node.NodeFont = value;
        Assert.Equal(value, node.NodeFont);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.NodeFont = value;
        Assert.Equal(value, node.NodeFont);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> NodeFont_SetWithTreeViewWithHandle_TestData()
    {
        yield return new object[] { null, 0 };
        yield return new object[] { new Font("Arial", 8.25f), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(NodeFont_SetWithTreeViewWithHandle_TestData))]
    public void TreeNode_NodeFont_SetWithTreeViewWithHandle_GetReturnsExpected(Font value, int expectedInvalidatedCallCount)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        node.NodeFont = value;
        Assert.Equal(value, node.NodeFont);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        node.NodeFont = value;
        Assert.Equal(value, node.NodeFont);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void TreeNode_NodeFont_SetWithCustomOldValue_GetReturnsExpected(Font value)
    {
        using Font oldValue = new("Arial", 1);
        TreeNode node = new()
        {
            NodeFont = oldValue
        };

        node.NodeFont = value;
        Assert.Equal(value, node.NodeFont);

        // Set same.
        node.NodeFont = value;
        Assert.Equal(value, node.NodeFont);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void TreeNode_NodeFont_SetWithCustomOldValueWithTreeView_GetReturnsExpected(Font value)
    {
        using TreeView control = new();
        using Font oldValue = new("Arial", 1);
        TreeNode node = new()
        {
            NodeFont = oldValue
        };
        control.Nodes.Add(node);

        node.NodeFont = value;
        Assert.Equal(value, node.NodeFont);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.NodeFont = value;
        Assert.Equal(value, node.NodeFont);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> NodeFont_SetWithCustomOldValueWithTreeViewWithHandle_TestData()
    {
        yield return new object[] { null, 1 };
        yield return new object[] { new Font("Arial", 8.25f), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(NodeFont_SetWithCustomOldValueWithTreeViewWithHandle_TestData))]
    public void TreeNode_NodeFont_SetWithCustomOldValueWithTreeViewWithHandle_GetReturnsExpected(Font value, int expectedInvalidatedCallCount)
    {
        using TreeView control = new();
        using Font oldValue = new("Arial", 1);
        TreeNode node = new()
        {
            NodeFont = oldValue
        };
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        node.NodeFont = value;
        Assert.Equal(value, node.NodeFont);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        node.NodeFont = value;
        Assert.Equal(value, node.NodeFont);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-1, -1)]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    public void TreeNode_SelectedImageIndex_SetWithoutTreeView_GetReturnsExpected(int value, int expectedWithImage)
    {
        TreeNode node = new()
        {
            SelectedImageIndex = value
        };
        Assert.Equal(value, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);

        // Set same.
        node.SelectedImageIndex = value;
        Assert.Equal(value, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);

        // Set tree view.
        using TreeView control = new();
        control.Nodes.Add(node);
        Assert.Equal(value, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.False(control.IsHandleCreated);

        // Add image list.
        using ImageList imageList = new();
        control.ImageList = imageList;
        Assert.Equal(-1, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.False(control.IsHandleCreated);

        // Add to image list.
        using Bitmap image = new(10, 10);
        imageList.Images.Add(image);
        Assert.Equal(expectedWithImage, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(TreeNode_ImageIndex_TestData))]
    public void TreeNode_SelectedImageIndex_SetWithSelectedImageKey_GetReturnsExpected(int value, int expectedWithoutImage, int expectedWithImage)
    {
        TreeNode node = new()
        {
            SelectedImageKey = "SelectedImageKey",
            SelectedImageIndex = value
        };
        Assert.Equal(value, node.SelectedImageIndex);
        Assert.Equal(ImageList.Indexer.DefaultKey, node.SelectedImageKey);

        // Set same.
        node.SelectedImageIndex = value;
        Assert.Equal(value, node.SelectedImageIndex);
        Assert.Equal(ImageList.Indexer.DefaultKey, node.SelectedImageKey);

        // Set tree view.
        using TreeView control = new();
        control.Nodes.Add(node);
        Assert.Equal(value, node.SelectedImageIndex);
        Assert.Equal(ImageList.Indexer.DefaultKey, node.SelectedImageKey);
        Assert.False(control.IsHandleCreated);

        // Add image list.
        using ImageList imageList = new();
        control.ImageList = imageList;
        Assert.Equal(expectedWithoutImage, node.SelectedImageIndex);
        Assert.Equal(ImageList.Indexer.DefaultKey, node.SelectedImageKey);
        Assert.False(control.IsHandleCreated);

        // Add to image list.
        using Bitmap image = new(10, 10);
        imageList.Images.Add(image);
        Assert.Equal(expectedWithImage, node.SelectedImageIndex);
        Assert.Equal(ImageList.Indexer.DefaultKey, node.SelectedImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(TreeNode_ImageIndex_TestData))]
    public void TreeNode_SelectedImageIndex_SetWithTreeView_GetReturnsExpected(int value, int expectedWithoutImage, int expectedWithImage)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        node.SelectedImageIndex = value;
        Assert.Equal(value, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.SelectedImageIndex = value;
        Assert.Equal(value, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.False(control.IsHandleCreated);

        // Add image list.
        using ImageList imageList = new();
        control.ImageList = imageList;
        Assert.Equal(expectedWithoutImage, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.False(control.IsHandleCreated);

        // Add to image list.
        using Bitmap image = new(10, 10);
        imageList.Images.Add(image);
        Assert.Equal(expectedWithImage, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(TreeNode_ImageIndex_TestData))]
    public void TreeNode_SelectedImageIndex_SetWithTreeViewWithEmptyList_GetReturnsExpected(int value, int expectedWithoutImage, int expectedWithImage)
    {
        using ImageList imageList = new();
        using TreeView control = new()
        {
            ImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        node.SelectedImageIndex = value;
        Assert.Equal(expectedWithoutImage, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.SelectedImageIndex = value;
        Assert.Equal(expectedWithoutImage, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.False(control.IsHandleCreated);

        // Add to image list.
        using Bitmap image = new(10, 10);
        imageList.Images.Add(image);
        Assert.Equal(expectedWithImage, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-2, -2)]
    [InlineData(-1, -1)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    public void TreeNode_SelectedImageIndex_SetWithTreeViewWithNotEmptyList_GetReturnsExpected(int value, int expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image1);
        imageList.Images.Add(image2);
        using TreeView control = new()
        {
            ImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        node.SelectedImageIndex = value;
        Assert.Equal(expected, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.SelectedImageIndex = value;
        Assert.Equal(expected, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(TreeNode_ImageIndex_TestData))]
    public void TreeNode_SelectedImageIndex_SetWithTreeViewWithHandle_GetReturnsExpected(int value, int expectedWithoutImage, int expectedWithImage)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        node.SelectedImageIndex = value;
        Assert.Equal(value, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        node.SelectedImageIndex = value;
        Assert.Equal(value, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add image list.
        using ImageList imageList = new();
        control.ImageList = imageList;
        Assert.Equal(expectedWithoutImage, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add to image list.
        using Bitmap image = new(10, 10);
        imageList.Images.Add(image);
        Assert.Equal(expectedWithImage, node.SelectedImageIndex);
        Assert.Empty(node.SelectedImageKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-2, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    public void TreeNode_SelectedImageIndex_GetItemWithoutImageList_Success(int value, int expected)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.SelectedImageIndex = value;
        TVITEMW item = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_IMAGE,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref item));
        Assert.Equal(expected, item.iSelectedImage);
    }

    [WinFormsTheory]
    [InlineData(-2, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    public void TreeNode_SelectedImageIndex_GetItemWithEmptyImageList_Success(int value, int expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        using TreeView control = new()
        {
            ImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.SelectedImageIndex = value;
        TVITEMW item = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_SELECTEDIMAGE,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref item));
        Assert.Equal(expected, item.iSelectedImage);
    }

    [WinFormsTheory]
    [InlineData(-2, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    public void TreeNode_SelectedImageIndex_GetItemWithImageList_Success(int value, int expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image1);
        imageList.Images.Add(image2);
        using TreeView control = new()
        {
            ImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.SelectedImageIndex = value;
        TVITEMW item = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_SELECTEDIMAGE,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref item));
        Assert.Equal(expected, item.iSelectedImage);
    }

    [WinFormsTheory]
    [InlineData(-2)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void TreeNode_SelectedImageIndex_SetWithTreeViewDisposed_GetReturnsExpected(int value)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.Dispose();

        node.SelectedImageIndex = value;
        Assert.Equal(value, node.SelectedImageIndex);
        Assert.False(control.IsHandleCreated);

        // Set same
        node.SelectedImageIndex = value;
        Assert.Equal(value, node.SelectedImageIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-3)]
    public void TreeNode_SelectedImageIndex_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
    {
        TreeNode node = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => node.SelectedImageIndex = value);
    }

    [WinFormsFact]
    public void TreeNode_SelectedImageIndex_SetInvalidSetItem_ThrowsInvalidOperationException()
    {
        using InvalidSetItemTreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.MakeInvalid = true;
        node.SelectedImageIndex = 0;
        Assert.Equal(0, node.SelectedImageIndex);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TreeNode_SelectedImageKey_SetWithoutTreeView_GetReturnsExpected(string value, string expected)
    {
        TreeNode node = new()
        {
            SelectedImageKey = value
        };
        Assert.Equal(expected, node.SelectedImageKey);
        Assert.Equal(-1, node.SelectedImageIndex);

        // Set same.
        node.SelectedImageKey = value;
        Assert.Equal(expected, node.SelectedImageKey);
        Assert.Equal(-1, node.SelectedImageIndex);
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("SelectedImageKey", "SelectedImageKey")]
    public void TreeNode_SelectedImageKey_SetWithSelectedImageIndex_GetReturnsExpected(string value, string expectedSelectedImageKey)
    {
        TreeNode node = new()
        {
            SelectedImageIndex = 0,
            SelectedImageKey = value
        };
        Assert.Equal(expectedSelectedImageKey, node.SelectedImageKey);
        Assert.Equal(ImageList.Indexer.DefaultIndex, node.SelectedImageIndex);

        // Set same.
        node.SelectedImageKey = value;
        Assert.Equal(expectedSelectedImageKey, node.SelectedImageKey);
        Assert.Equal(ImageList.Indexer.DefaultIndex, node.SelectedImageIndex);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TreeNode_SelectedImageKey_SetWithTreeView_GetReturnsExpected(string value, string expected)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        node.SelectedImageKey = value;
        Assert.Equal(expected, node.SelectedImageKey);
        Assert.Equal(-1, node.SelectedImageIndex);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.SelectedImageKey = value;
        Assert.Equal(expected, node.SelectedImageKey);
        Assert.Equal(-1, node.SelectedImageIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TreeNode_SelectedImageKey_SetWithTreeViewWithEmptyList_GetReturnsExpected(string value, string expected)
    {
        using ImageList imageList = new();
        using TreeView control = new()
        {
            ImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        node.SelectedImageKey = value;
        Assert.Equal(expected, node.SelectedImageKey);
        Assert.Equal(-1, node.SelectedImageIndex);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.SelectedImageKey = value;
        Assert.Equal(expected, node.SelectedImageKey);
        Assert.Equal(-1, node.SelectedImageIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Image1", "Image1")]
    [InlineData("image1", "image1")]
    [InlineData("Image2", "Image2")]
    [InlineData("NoSuchImage", "NoSuchImage")]
    public void TreeNode_SelectedImageKey_SetWithTreeViewWithNotEmptyList_GetReturnsExpected(string value, string expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add("Image1", image1);
        imageList.Images.Add("Image2", image2);
        using TreeView control = new()
        {
            ImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        node.SelectedImageKey = value;
        Assert.Equal(expected, node.SelectedImageKey);
        Assert.Equal(-1, node.SelectedImageIndex);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.SelectedImageKey = value;
        Assert.Equal(expected, node.SelectedImageKey);
        Assert.Equal(-1, node.SelectedImageIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TreeNode_SelectedImageKey_SetWithTreeViewWithHandle_GetReturnsExpected(string value, string expected)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        node.SelectedImageKey = value;
        Assert.Equal(expected, node.SelectedImageKey);
        Assert.Equal(-1, node.SelectedImageIndex);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        node.SelectedImageKey = value;
        Assert.Equal(expected, node.SelectedImageKey);
        Assert.Equal(-1, node.SelectedImageIndex);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Image1")]
    [InlineData("image1")]
    [InlineData("Image2")]
    [InlineData("NoSuchImage")]
    public void TreeNode_SelectedImageKey_GetItemWithoutImageList_Success(string value)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.SelectedImageKey = value;
        TVITEMW column = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_SELECTEDIMAGE,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref column));
        Assert.Equal(0, column.iSelectedImage);
    }

    [WinFormsTheory]
    [InlineData(null, 0)]
    [InlineData("", 0)]
    [InlineData("Image1", 0)]
    [InlineData("image1", 0)]
    [InlineData("Image2", 0)]
    [InlineData("NoSuchImage", 0)]
    public void TreeNode_SelectedImageKey_GetItemWithEmptyImageList_Success(string value, int expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        using TreeView control = new()
        {
            ImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.SelectedImageKey = value;
        TVITEMW column = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_SELECTEDIMAGE,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref column));
        Assert.Equal(expected, column.iSelectedImage);
    }

    [WinFormsTheory]
    [InlineData(null, 0)]
    [InlineData("", 0)]
    [InlineData("Image1", 0)]
    [InlineData("image1", 0)]
    [InlineData("Image2", 1)]
    [InlineData("NoSuchImage", 0)]
    public void TreeNode_SelectedImageKey_GetItemWithImageList_Success(string value, int expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add("Image1", image1);
        imageList.Images.Add("Image2", image2);
        using TreeView control = new()
        {
            ImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.SelectedImageKey = value;
        TVITEMW column = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_SELECTEDIMAGE,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref column));
        Assert.Equal(expected, column.iSelectedImage);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TreeNode_SelectedImageKey_SetWithTreeViewDisposed_GetReturnsExpected(string value, string expected)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.Dispose();

        node.SelectedImageKey = value;
        Assert.Equal(expected, node.SelectedImageKey);
        Assert.False(control.IsHandleCreated);

        // Set same
        node.SelectedImageKey = value;
        Assert.Equal(expected, node.SelectedImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_SelectedImageKey_SetInvalidSetItem_ThrowsInvalidOperationException()
    {
        using InvalidSetItemTreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.MakeInvalid = true;
        node.SelectedImageKey = "Key";
        Assert.Equal("Key", node.SelectedImageKey);
    }

    [WinFormsTheory]
    [InlineData(-1, -1)]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(14, 0)]
    public void TreeNode_StateImageIndex_SetWithoutTreeView_GetReturnsExpected(int value, int expectedWithImage)
    {
        TreeNode node = new()
        {
            StateImageIndex = value
        };
        Assert.Equal(value, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);

        // Set same.
        node.StateImageIndex = value;
        Assert.Equal(value, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);

        // Set tree view.
        using TreeView control = new();
        control.Nodes.Add(node);
        Assert.Equal(value, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.False(control.IsHandleCreated);

        // Add image list.
        using ImageList imageList = new();
        control.StateImageList = imageList;
        Assert.Equal(-1, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.False(control.IsHandleCreated);

        // Add to image list.
        using Bitmap image = new(10, 10);
        imageList.Images.Add(image);
        Assert.Equal(expectedWithImage, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1, -1)]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(14, 0)]
    public void TreeNode_StateImageIndex_SetWithStateImageKey_GetReturnsExpected(int value, int expectedWithImage)
    {
        TreeNode node = new()
        {
            StateImageKey = "StateImageKey",
            StateImageIndex = value
        };
        Assert.Equal(value, node.StateImageIndex);
        Assert.Equal(ImageList.Indexer.DefaultKey, node.StateImageKey);

        // Set same.
        node.StateImageIndex = value;
        Assert.Equal(value, node.StateImageIndex);
        Assert.Equal(ImageList.Indexer.DefaultKey, node.StateImageKey);

        // Set tree view.
        using TreeView control = new();
        control.Nodes.Add(node);
        Assert.Equal(value, node.StateImageIndex);
        Assert.Equal(ImageList.Indexer.DefaultKey, node.StateImageKey);
        Assert.False(control.IsHandleCreated);

        // Add image list.
        using ImageList imageList = new();
        control.StateImageList = imageList;
        Assert.Equal(-1, node.StateImageIndex);
        Assert.Equal(ImageList.Indexer.DefaultKey, node.StateImageKey);
        Assert.False(control.IsHandleCreated);

        // Add to image list.
        using Bitmap image = new(10, 10);
        imageList.Images.Add(image);
        Assert.Equal(expectedWithImage, node.StateImageIndex);
        Assert.Equal(ImageList.Indexer.DefaultKey, node.StateImageKey);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> StateImageIndex_SetWithTreeView_TestData()
    {
        foreach (bool checkBoxes in new bool[] { true, false })
        {
            yield return new object[] { checkBoxes, -1, -1 };
            yield return new object[] { checkBoxes, 0, 0 };
            yield return new object[] { checkBoxes, 1, 0 };
            yield return new object[] { checkBoxes, 2, 0 };
            yield return new object[] { checkBoxes, 14, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(StateImageIndex_SetWithTreeView_TestData))]
    public void TreeNode_StateImageIndex_SetWithTreeView_GetReturnsExpected(bool checkBoxes, int value, int expectedWithImage)
    {
        using TreeView control = new()
        {
            CheckBoxes = checkBoxes
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        node.StateImageIndex = value;
        Assert.Equal(value, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.StateImageIndex = value;
        Assert.Equal(value, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.False(control.IsHandleCreated);

        // Add image list.
        using ImageList imageList = new();
        control.StateImageList = imageList;
        Assert.Equal(-1, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.False(control.IsHandleCreated);

        // Add to image list.
        using Bitmap image = new(10, 10);
        imageList.Images.Add(image);
        Assert.Equal(expectedWithImage, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(StateImageIndex_SetWithTreeView_TestData))]
    public void TreeNode_StateImageIndex_SetWithTreeViewWithEmptyList_GetReturnsExpected(bool checkBoxes, int value, int expectedWithImage)
    {
        using ImageList imageList = new();
        using TreeView control = new()
        {
            CheckBoxes = checkBoxes,
            StateImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        node.StateImageIndex = value;
        Assert.Equal(-1, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.StateImageIndex = value;
        Assert.Equal(-1, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.False(control.IsHandleCreated);

        // Add to image list.
        using Bitmap image = new(10, 10);
        imageList.Images.Add(image);
        Assert.Equal(expectedWithImage, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> StateImageIndex_SetWithTreeViewNotEmptyList_TestData()
    {
        foreach (bool checkBoxes in new bool[] { true, false })
        {
            yield return new object[] { checkBoxes, -1, -1 };
            yield return new object[] { checkBoxes, 0, 0 };
            yield return new object[] { checkBoxes, 1, 1 };
            yield return new object[] { checkBoxes, 2, 1 };
            yield return new object[] { checkBoxes, 14, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(StateImageIndex_SetWithTreeViewNotEmptyList_TestData))]
    public void TreeNode_StateImageIndex_SetWithTreeViewWithNotEmptyList_GetReturnsExpected(bool checkBoxes, int value, int expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image1);
        imageList.Images.Add(image2);
        using TreeView control = new()
        {
            CheckBoxes = checkBoxes,
            StateImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        node.StateImageIndex = value;
        Assert.Equal(expected, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.StateImageIndex = value;
        Assert.Equal(expected, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> StateImageIndex_SetWithTreeViewWithHandle_TestData()
    {
        foreach (bool checkBoxes in new bool[] { true, false })
        {
            yield return new object[] { checkBoxes, -1, checkBoxes ? 1 : 0 };
            yield return new object[] { checkBoxes, 0, checkBoxes ? 1 : 0 };
            yield return new object[] { checkBoxes, 1, checkBoxes ? 1 : 0 };
            yield return new object[] { checkBoxes, 2, checkBoxes ? 1 : 0 };
            yield return new object[] { checkBoxes, 14, checkBoxes ? 1 : 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(StateImageIndex_SetWithTreeViewWithHandle_TestData))]
    public void TreeNode_StateImageIndex_SetWithTreeViewWithHandle_GetReturnsExpected(bool checkBoxes, int value, int expectedCreatedCallCount)
    {
        using TreeView control = new()
        {
            CheckBoxes = checkBoxes
        };
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        node.StateImageIndex = value;
        Assert.Equal(value, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        node.StateImageIndex = value;
        Assert.Equal(value, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add image list.
        using ImageList imageList = new();
        control.StateImageList = imageList;
        Assert.Equal(-1, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, -1, 4096)]
    [InlineData(true, 0, 4096)]
    [InlineData(true, 1, 4096)]
    [InlineData(true, 14, 4096)]
    [InlineData(false, -1, 0)]
    [InlineData(false, 0, 4096)]
    [InlineData(false, 1, 8192)]
    [InlineData(false, 14, 61440)]
    public void TreeNode_StateImageIndex_GetColumnWithoutImageList_Success(bool checkBoxes, int value, int expected)
    {
        using TreeView control = new()
        {
            CheckBoxes = checkBoxes
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.StateImageIndex = value;
        TVITEMW item = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_STATE,
            stateMask = TREE_VIEW_ITEM_STATE_FLAGS.TVIS_STATEIMAGEMASK,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref item));
        Assert.Equal((TREE_VIEW_ITEM_STATE_FLAGS)expected, item.state);
    }

    [WinFormsTheory]
    [InlineData(-1, 0)]
    [InlineData(0, 4096)]
    [InlineData(1, 8192)]
    [InlineData(2, 12288)]
    [InlineData(14, 61440)]
    public void TreeNode_StateImageIndex_GetColumnWithEmptyImageList_Success(int value, int expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        using TreeView control = new()
        {
            StateImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.StateImageIndex = value;
        TVITEMW item = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_STATE,
            stateMask = TREE_VIEW_ITEM_STATE_FLAGS.TVIS_STATEIMAGEMASK,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref item));
        Assert.Equal((TREE_VIEW_ITEM_STATE_FLAGS)expected, item.state);
    }

    [WinFormsTheory]
    [InlineData(-1, 0)]
    [InlineData(0, 4096)]
    [InlineData(1, 8192)]
    [InlineData(2, 12288)]
    [InlineData(14, 61440)]
    public void TreeNode_StateImageIndex_GetColumnWithImageList_Success(int value, int expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image1);
        imageList.Images.Add(image2);
        using TreeView control = new()
        {
            StateImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.StateImageIndex = value;
        TVITEMW item = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_STATE,
            stateMask = TREE_VIEW_ITEM_STATE_FLAGS.TVIS_STATEIMAGEMASK,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref item));
        Assert.Equal((TREE_VIEW_ITEM_STATE_FLAGS)expected, item.state);
    }

    [WinFormsTheory]
    [InlineData(true, -1, -1)]
    [InlineData(true, 0, 0)]
    [InlineData(true, 1, 0)]
    [InlineData(true, 14, 0)]
    [InlineData(false, -1, -1)]
    [InlineData(false, 0, 0)]
    [InlineData(false, 1, 0)]
    [InlineData(false, 14, 0)]
    public void TreeNode_StateImageIndex_SetWithTreeViewDisposed_GetReturnsExpected(bool checkBoxes, int value, int expectedWithImage)
    {
        using TreeView control = new()
        {
            CheckBoxes = checkBoxes
        };
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.Dispose();

        node.StateImageIndex = value;
        Assert.Equal(value, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.False(control.IsHandleCreated);

        // Set same
        node.StateImageIndex = value;
        Assert.Equal(value, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.False(control.IsHandleCreated);

        // Add image list.
        using ImageList imageList = new();
        control.StateImageList = imageList;
        Assert.Equal(-1, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.False(control.IsHandleCreated);

        // Add to image list.
        using Bitmap image = new(10, 10);
        imageList.Images.Add(image);
        Assert.Equal(expectedWithImage, node.StateImageIndex);
        Assert.Empty(node.StateImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-2)]
    [InlineData(15)]
    public void TreeNode_StateImageIndex_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
    {
        TreeNode node = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => node.StateImageIndex = value);
    }

    [WinFormsFact]
    public void TreeNode_StateImageIndex_SetInvalidSetItem_ThrowsInvalidOperationException()
    {
        using InvalidSetItemTreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.MakeInvalid = true;
        node.StateImageIndex = 0;
        Assert.Equal(0, node.StateImageIndex);

        // Add image list.
        using ImageList imageList = new();
        control.StateImageList = imageList;
        Assert.Equal(-1, node.StateImageIndex);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TreeNode_StateImageKey_SetWithoutTreeView_GetReturnsExpected(string value, string expected)
    {
        TreeNode node = new()
        {
            StateImageKey = value
        };
        Assert.Equal(expected, node.StateImageKey);
        Assert.Equal(-1, node.StateImageIndex);

        // Set same.
        node.StateImageKey = value;
        Assert.Equal(expected, node.StateImageKey);
        Assert.Equal(-1, node.StateImageIndex);

        // Set tree view.
        using TreeView control = new();
        control.Nodes.Add(node);
        Assert.Equal(expected, node.StateImageKey);
        Assert.Equal(-1, node.StateImageIndex);

        // Add image list.
        using ImageList imageList = new();
        control.StateImageList = imageList;
        Assert.Equal(expected, node.StateImageKey);
        Assert.Equal(-1, node.StateImageIndex);
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("StateImageKey", "StateImageKey")]
    public void TreeNode_StateImageKey_SetWithStateImageIndex_GetReturnsExpected(string value, string expectedStateImageKey)
    {
        TreeNode node = new()
        {
            StateImageIndex = 0,
            StateImageKey = value
        };
        Assert.Equal(expectedStateImageKey, node.StateImageKey);
        Assert.Equal(ImageList.Indexer.DefaultIndex, node.StateImageIndex);

        // Set same.
        node.StateImageKey = value;
        Assert.Equal(expectedStateImageKey, node.StateImageKey);
        Assert.Equal(ImageList.Indexer.DefaultIndex, node.StateImageIndex);

        // Set tree view.
        using TreeView control = new();
        control.Nodes.Add(node);
        Assert.Equal(expectedStateImageKey, node.StateImageKey);
        Assert.Equal(ImageList.Indexer.DefaultIndex, node.StateImageIndex);

        // Add image list.
        using ImageList imageList = new();
        control.StateImageList = imageList;
        Assert.Equal(expectedStateImageKey, node.StateImageKey);
        Assert.Equal(ImageList.Indexer.DefaultIndex, node.StateImageIndex);
    }

    public static IEnumerable<object[]> StateImageKey_SetWithTreeView_TestData()
    {
        foreach (bool checkBoxes in new bool[] { true, false })
        {
            yield return new object[] { checkBoxes, null, string.Empty };
            yield return new object[] { checkBoxes, string.Empty, string.Empty };
            yield return new object[] { checkBoxes, "text", "text" };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(StateImageKey_SetWithTreeView_TestData))]
    public void TreeNode_StateImageKey_SetWithTreeView_GetReturnsExpected(bool checkBoxes, string value, string expected)
    {
        using TreeView control = new()
        {
            CheckBoxes = checkBoxes
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        node.StateImageKey = value;
        Assert.Equal(expected, node.StateImageKey);
        Assert.Equal(-1, node.StateImageIndex);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.StateImageKey = value;
        Assert.Equal(expected, node.StateImageKey);
        Assert.Equal(-1, node.StateImageIndex);
        Assert.False(control.IsHandleCreated);

        // Add image list.
        using ImageList imageList = new();
        control.StateImageList = imageList;
        Assert.Equal(expected, node.StateImageKey);
        Assert.Equal(-1, node.StateImageIndex);
    }

    [WinFormsTheory]
    [MemberData(nameof(StateImageKey_SetWithTreeView_TestData))]
    public void TreeNode_StateImageKey_SetWithTreeViewWithEmptyList_GetReturnsExpected(bool checkBoxes, string value, string expected)
    {
        using ImageList imageList = new();
        using TreeView control = new()
        {
            CheckBoxes = checkBoxes,
            StateImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        node.StateImageKey = value;
        Assert.Equal(expected, node.StateImageKey);
        Assert.Equal(-1, node.StateImageIndex);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.StateImageKey = value;
        Assert.Equal(expected, node.StateImageKey);
        Assert.Equal(-1, node.StateImageIndex);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> StateImageKey_SetWithTreeViewWithImageList_TestData()
    {
        foreach (bool checkBoxes in new bool[] { true, false })
        {
            yield return new object[] { checkBoxes, null, "" };
            yield return new object[] { checkBoxes, "", "" };
            yield return new object[] { checkBoxes, "Image1", "Image1" };
            yield return new object[] { checkBoxes, "image1", "image1" };
            yield return new object[] { checkBoxes, "Image2", "Image2" };
            yield return new object[] { checkBoxes, "NoSuchImage", "NoSuchImage" };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(StateImageKey_SetWithTreeViewWithImageList_TestData))]
    public void TreeNode_StateImageKey_SetWithTreeViewWithNotEmptyList_GetReturnsExpected(bool checkBoxes, string value, string expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add("Image1", image1);
        imageList.Images.Add("Image2", image2);
        using TreeView control = new()
        {
            CheckBoxes = checkBoxes,
            StateImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        node.StateImageKey = value;
        Assert.Equal(expected, node.StateImageKey);
        Assert.Equal(-1, node.StateImageIndex);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.StateImageKey = value;
        Assert.Equal(expected, node.StateImageKey);
        Assert.Equal(-1, node.StateImageIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(StateImageKey_SetWithTreeView_TestData))]
    public void TreeNode_StateImageKey_SetWithTreeViewWithHandle_GetReturnsExpected(bool checkBoxes, string value, string expected)
    {
        using TreeView control = new()
        {
            CheckBoxes = checkBoxes
        };
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        node.StateImageKey = value;
        Assert.Equal(expected, node.StateImageKey);
        Assert.Equal(-1, node.StateImageIndex);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        node.StateImageKey = value;
        Assert.Equal(expected, node.StateImageKey);
        Assert.Equal(-1, node.StateImageIndex);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Image1")]
    [InlineData("image1")]
    [InlineData("Image2")]
    [InlineData("NoSuchImage")]
    public void TreeNode_StateImageKey_GetColumnWithoutImageList_Success(string value)
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.StateImageKey = value;
        TVITEMW column = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_STATE,
            stateMask = TREE_VIEW_ITEM_STATE_FLAGS.TVIS_STATEIMAGEMASK,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref column));
        Assert.Equal((TREE_VIEW_ITEM_STATE_FLAGS)0, column.state);
    }

    [WinFormsTheory]
    [InlineData(true, null, 4096)]
    [InlineData(true, "", 4096)]
    [InlineData(true, "Image1", 4096)]
    [InlineData(true, "image1", 4096)]
    [InlineData(true, "Image2", 4096)]
    [InlineData(true, "NoSuchImage", 4096)]
    [InlineData(false, null, 0)]
    [InlineData(false, "", 0)]
    [InlineData(false, "Image1", 0)]
    [InlineData(false, "image1", 0)]
    [InlineData(false, "Image2", 0)]
    [InlineData(false, "NoSuchImage", 0)]
    public void TreeNode_StateImageKey_GetColumnWithEmptyImageList_Success(bool checkBoxes, string value, int expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        using TreeView control = new()
        {
            CheckBoxes = checkBoxes,
            StateImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.StateImageKey = value;
        TVITEMW column = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_STATE,
            stateMask = TREE_VIEW_ITEM_STATE_FLAGS.TVIS_STATEIMAGEMASK,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref column));
        Assert.Equal((TREE_VIEW_ITEM_STATE_FLAGS)expected, column.state);
    }

    [WinFormsTheory]
    [InlineData(true, null, 4096)]
    [InlineData(true, "", 4096)]
    [InlineData(true, "Image1", 4096)]
    [InlineData(true, "image1", 4096)]
    [InlineData(true, "Image2", 4096)]
    [InlineData(true, "NoSuchImage", 4096)]
    [InlineData(false, null, 0)]
    [InlineData(false, "", 0)]
    [InlineData(false, "Image1", 4096)]
    [InlineData(false, "image1", 4096)]
    [InlineData(false, "Image2", 8192)]
    [InlineData(false, "NoSuchImage", 0)]
    public void TreeNode_StateImageKey_GetColumnWithImageList_Success(bool checkBoxes, string value, int expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add("Image1", image1);
        imageList.Images.Add("Image2", image2);
        using TreeView control = new()
        {
            CheckBoxes = checkBoxes,
            StateImageList = imageList
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.StateImageKey = value;
        TVITEMW column = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_STATE,
            stateMask = TREE_VIEW_ITEM_STATE_FLAGS.TVIS_STATEIMAGEMASK,
            hItem = node.HTREEITEM
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref column));
        Assert.Equal((TREE_VIEW_ITEM_STATE_FLAGS)expected, column.state);
    }

    [WinFormsTheory]
    [MemberData(nameof(StateImageKey_SetWithTreeView_TestData))]
    public void TreeNode_StateImageKey_SetWithTreeViewDisposed_GetReturnsExpected(bool checkBoxes, string value, string expected)
    {
        using TreeView control = new()
        {
            CheckBoxes = checkBoxes
        };
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.Dispose();

        node.StateImageKey = value;
        Assert.Equal(expected, node.StateImageKey);
        Assert.False(control.IsHandleCreated);

        // Set same
        node.StateImageKey = value;
        Assert.Equal(expected, node.StateImageKey);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_StateImageKey_SetInvalidSetItem_ThrowsInvalidOperationException()
    {
        using InvalidSetItemTreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.MakeInvalid = true;
        node.StateImageKey = "Key";
        Assert.Equal("Key", node.StateImageKey);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void TreeNode_Tag_Set_GetReturnsExpected(string value)
    {
        TreeNode node = new()
        {
            Tag = value
        };
        Assert.Same(value, node.Tag);

        // Set same.
        node.Tag = value;
        Assert.Same(value, node.Tag);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TreeNode_Text_Set_GetReturnsExpected(string value, string expected)
    {
        TreeNode node = new()
        {
            Text = value
        };
        Assert.Equal(expected, node.Text);

        // Set same.
        node.Text = value;
        Assert.Equal(expected, node.Text);
    }

    public static IEnumerable<object[]> Text_SetWithTreeView_TestData()
    {
        foreach (bool scrollable in new bool[] { true, false })
        {
            yield return new object[] { scrollable, null, string.Empty };
            yield return new object[] { scrollable, string.Empty, string.Empty };
            yield return new object[] { scrollable, "text", "text" };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Text_SetWithTreeView_TestData))]
    public void TreeNode_Text_SetWithTreeView_GetReturnsExpected(bool scrollable, string value, string expected)
    {
        using TreeView control = new()
        {
            Scrollable = scrollable
        };
        TreeNode node = new();
        control.Nodes.Add(node);

        node.Text = value;
        Assert.Equal(expected, node.Text);
        Assert.False(control.IsHandleCreated);

        // Set same.
        node.Text = value;
        Assert.Equal(expected, node.Text);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Text_SetWithTreeView_TestData))]
    public void TreeNode_Text_SetWithTreeViewWithHandle_GetReturnsExpected(bool scrollable, string value, string expected)
    {
        using TreeView control = new()
        {
            Scrollable = scrollable
        };
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        node.Text = value;
        Assert.Equal(expected, node.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        node.Text = value;
        Assert.Equal(expected, node.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Text_SetWithTreeView_TestData))]
    public unsafe void TreeNode_Text_GetItem_Success(bool scrollable, string value, string expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using TreeView control = new()
        {
            Scrollable = scrollable
        };
        TreeNode node = new();
        control.Nodes.Add(node);
        char* textBuffer = stackalloc char[256];

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        node.Text = value;
        TVITEMW item = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_TEXT,
            hItem = node.HTREEITEM,
            cchTextMax = 256,
            pszText = textBuffer
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.TVM_GETITEMW, 0, ref item));
        Assert.Equal(expected, new string((char*)item.pszText));
    }

    [WinFormsTheory]
    [MemberData(nameof(Text_SetWithTreeView_TestData))]
    public void TreeNode_Text_SetWithTreeViewDisposed_GetReturnsExpected(bool scrollable, string value, string expected)
    {
        using TreeView control = new()
        {
            Scrollable = scrollable
        };
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.Dispose();

        node.Text = value;
        Assert.Equal(expected, node.Text);
        Assert.False(control.IsHandleCreated);

        // Set same
        node.Text = value;
        Assert.Equal(expected, node.Text);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void TreeNode_ToolTipText_Set_GetReturnsExpected(string value)
    {
        TreeNode node = new()
        {
            ToolTipText = value
        };
        Assert.Same(value, node.ToolTipText);

        // Set same.
        node.ToolTipText = value;
        Assert.Same(value, node.ToolTipText);
    }

    [WinFormsFact]
    public void TreeNode_TreeView_GetWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);

        Assert.Same(control, node.TreeView);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNode_TreeView_GetWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode node = new();
        control.Nodes.Add(node);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Same(control, node.TreeView);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TreeView_TreeView_GetWithTreeView_ReturnsExpected()
    {
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);
        Assert.Null(node1.TreeView);
        Assert.Null(node2.TreeView);
        Assert.Null(node3.TreeView);
        Assert.Null(node4.TreeView);
    }

    [WinFormsFact]
    public void TreeView_TreeView_GetWithTreeViewWithTreeView_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        control.Nodes.Add(parent);
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);

        Assert.Same(control, node1.TreeView);
        Assert.Same(control, node2.TreeView);
        Assert.Same(control, node3.TreeView);
        Assert.Same(control, node4.TreeView);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeView_TreeView_GetWithTreeViewWithTreeViewWithHandle_ReturnsExpected()
    {
        using TreeView control = new();
        TreeNode parent = new();
        TreeNode node1 = new();
        TreeNode node2 = new();
        TreeNode node3 = new();
        TreeNode node4 = new();
        control.Nodes.Add(parent);
        parent.Nodes.Add(node1);
        parent.Nodes.Add(node2);
        parent.Nodes.Add(node3);
        node3.Nodes.Add(node4);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Same(control, node1.TreeView);
        Assert.Same(control, node2.TreeView);
        Assert.Same(control, node3.TreeView);
        Assert.Same(control, node4.TreeView);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TreeNode_InvokeGetSelfAndChildNodes_ReturnsExpected()
    {
        TreeNode rootNode = new();
        TreeNode subNodeLevel1 = new();
        TreeNode subNodeLevel2 = new();
        TreeNode subNodeLevel3 = new();
        subNodeLevel2.Nodes.Add(subNodeLevel3);
        subNodeLevel1.Nodes.Add(subNodeLevel2);
        rootNode.Nodes.Add(subNodeLevel1);
        List<TreeNode> nodes = rootNode.GetSelfAndChildNodes();
        Assert.Equal(4, nodes.Count);
        Assert.Contains(rootNode, nodes);
        Assert.Contains(subNodeLevel1, nodes);
        Assert.Contains(subNodeLevel2, nodes);
        Assert.Contains(subNodeLevel3, nodes);
    }

    [WinFormsFact]
    public void TreeNode_InvokeAdd_WithoutTree_DoesNotAddNodeToTrackList()
    {
        TreeNode treeNode = new();
        TreeNode treeSubNode = new();
        treeNode.Nodes.Add(treeSubNode);

        Assert.False((bool)KeyboardToolTipStateMachine.Instance.TestAccessor().Dynamic.IsToolTracked(treeNode));
        Assert.False((bool)KeyboardToolTipStateMachine.Instance.TestAccessor().Dynamic.IsToolTracked(treeSubNode));
    }

    [WinFormsTheory]
    [InlineData(10, 10)]
    [InlineData(-10, -10)]
    [InlineData(0, 0)]
    public void TreeNodeAccessibleObject_HitTest_ReturnsExpected(int x, int y)
    {
        using TreeView treeView = new();
        TreeNode treeNode = new("Node 1");
        treeView.Nodes.Add(treeNode);

        TreeNode.TreeNodeAccessibleObject accessibleObject = new(treeNode, treeView);
        AccessibleObject hitTestResult = accessibleObject.HitTest(x, y);

        if (x < 0 || y < 0)
        {
            hitTestResult.Should().BeNull();
        }
        else
        {
            hitTestResult.Should().Be(treeView.AccessibilityObject.HitTest(x, y));
        }
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData(typeof(ContextMenuStrip))]
    public void TreeNode_ContextMenuStrip_Set_GetReturnsExpected(object value)
    {
        TreeNode treeNode = new()
        {
            ContextMenuStrip = value as ContextMenuStrip
        };

        treeNode.ContextMenuStrip.Should().Be(value as ContextMenuStrip);
    }

    [WinFormsTheory]
    [InlineData("Node1", "Node2")]
    public void TreeNode_FromHandle_Invoke_ReturnsExpected(string nodeName1, string nodeName2)
    {
        using TreeView treeView = new();

        TreeNode node1 = new(nodeName1);
        TreeNode node2 = new(nodeName2);
        treeView.Nodes.Add(node1);
        treeView.CreateControl();

        TreeNode result1 = TreeNode.FromHandle(treeView, node1.Handle);
        result1.Should().Be(node1);

        TreeNode result2 = TreeNode.FromHandle(treeView, node2.Handle);
        result2.Should().BeNull();
    }

    [WinFormsTheory]
    [BoolData]
    public void TreeNode_RowBounds_Get_ReturnsExpected(bool isNodeRemoved)
    {
        using TreeView treeView = new();
        TreeNode treeNode = new();
        treeView.Nodes.Add(treeNode);

        if (isNodeRemoved)
        {
            treeNode.Remove();
        }

        if (isNodeRemoved)
        {
            treeNode.RowBounds.Should().Be(Rectangle.Empty);
        }
        else
        {
            treeNode.RowBounds.Should().NotBe(Rectangle.Empty);
        }
    }

    [WinFormsTheory]
    [InlineData("Node1", "ChildNode1", "ChildNode2", true)]
    [InlineData("Node2", "ChildNode3", "ChildNode4", false)]
    public void TreeNode_Clone_ReturnsExpected(string nodeName, string childNode1, string childNode2, bool isChecked)
    {
        TreeNode treeNode = new()
        {
            Text = nodeName,
            Checked = isChecked
        };

        treeNode.Nodes.Add(new TreeNode(childNode1));
        treeNode.Nodes.Add(new TreeNode(childNode2));
        TreeNode clonedNode = (TreeNode)treeNode.Clone();

        clonedNode.Text.Should().Be(treeNode.Text);
        clonedNode.Checked.Should().Be(treeNode.Checked);
        clonedNode.Nodes.Count.Should().Be(treeNode.Nodes.Count);
        clonedNode.Nodes[0].Text.Should().Be(treeNode.Nodes[0].Text);
        clonedNode.Nodes[1].Text.Should().Be(treeNode.Nodes[1].Text);
    }

    [WinFormsTheory]
    [BoolData]
    public void TreeNode_EnsureVisible_InvokeWithTreeView_CallsExpected(bool initiallyVisible)
    {
        using TreeView treeView = new();
        TreeNode treeNode = new();
        treeView.Nodes.Add(treeNode);

        if (initiallyVisible)
        {
            treeNode.EnsureVisible();
            treeNode.IsVisible.Should().BeTrue();
        }

        treeNode.EnsureVisible();
        treeNode.IsVisible.Should().BeTrue();
    }

    [WinFormsFact]
    public void TreeNode_EnsureVisible_InvokeWithoutTreeView_NoAction()
    {
        TreeNode treeNode = new();

        treeNode.Invoking(tn => tn.EnsureVisible()).Should().NotThrow();
    }

    [WinFormsFact]
    public void TreeNode_EnsureVisible_InvokeWithDisposedTreeView_NoAction()
    {
        using TreeView treeView = new();
        TreeNode treeNode = new();
        treeView.Nodes.Add(treeNode);
        treeView.Dispose();

        treeNode.Invoking(tn => tn.EnsureVisible()).Should().NotThrow();
    }

    private TreeNode CreateTreeNodeStructure(int depth, int breadth, bool initiallyExpanded)
    {
        TreeNode node = new();
        if (depth > 0)
        {
            for (int i = 0; i < breadth; i++)
            {
                node.Nodes.Add(CreateTreeNodeStructure(depth - 1, breadth, initiallyExpanded));
            }
        }

        if (initiallyExpanded)
        {
            node.Expand();
        }

        return node;
    }

    private void CheckExpandedState(TreeNode node, bool expectedState)
    {
        node.IsExpanded.Should().Be(expectedState);
        foreach (TreeNode child in node.Nodes)
        {
            CheckExpandedState(child, expectedState);
        }
    }

    [WinFormsTheory]
    [InlineData(0, false, true)]
    [InlineData(1, false, false)]
    [InlineData(1, true, false)]
    public void TreeNode_ExpandCollapse_Invoke_Success(int depth, bool initiallyExpanded, bool collapse)
    {
        var node = CreateTreeNodeStructure(depth, 2, initiallyExpanded);
        CheckExpandedState(node, initiallyExpanded);

        if (collapse)
        {
            node.ExpandAll();
            CheckExpandedState(node, true);

            node.Collapse();
            CheckExpandedState(node, false);
        }
        else
        {
            node.ExpandAll();
            CheckExpandedState(node, true);
        }
    }

    [WinFormsTheory]
    [InlineData(0, false, 0)]
    [InlineData(2, true, 4)]
    public void TreeNode_GetNodeCount_ReturnsExpected(int childrenCount, bool includeSubTrees, int expectedCount)
    {
        TreeNode node = new();
        for (int i = 0; i < childrenCount; i++)
        {
            TreeNode child = new();
            child.Nodes.Add(new TreeNode());
            node.Nodes.Add(child);
        }

        int result = node.GetNodeCount(includeSubTrees);
        result.Should().Be(expectedCount);
    }

    [WinFormsFact]
    public void TreeNode_Toggle_WhenNodeIsExpanded_CollapsesNode()
    {
        using TreeView treeView = new();

        TreeNode node = new();
        treeView.Nodes.Add(node);

        node.Toggle();
        node.IsExpanded.Should().BeTrue();

        node.Toggle();
        node.IsExpanded.Should().BeFalse();
    }

    private class InvalidSetItemTreeView : TreeView
    {
        public bool MakeInvalid { get; set; }

        protected override void WndProc(ref Message m)
        {
            if (MakeInvalid && m.Msg == (int)PInvoke.TVM_SETITEMW)
            {
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }
    }

    private class CustomGetItemTreeView : TreeView
    {
        public TREE_VIEW_ITEM_STATE_FLAGS GetItemStateResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvoke.TVM_GETITEMW)
            {
                TVITEMW* pItem = (TVITEMW*)m.LParam;
                pItem->state = GetItemStateResult;
                Assert.Equal(IntPtr.Zero, m.WParam);
                m.Result = 1;
                return;
            }

            base.WndProc(ref m);
        }
    }

    private class InvalidGetItemTreeView : TreeView
    {
        public bool MakeInvalid { get; set; }

        public TREE_VIEW_ITEM_STATE_FLAGS GetItemStateResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (MakeInvalid && m.Msg == (int)PInvoke.TVM_GETITEMW)
            {
                TVITEMW* pItem = (TVITEMW*)m.LParam;
                pItem->state = GetItemStateResult;
                Assert.Equal(IntPtr.Zero, m.WParam);
                m.Result = 0;
                return;
            }

            base.WndProc(ref m);
        }
    }

    private class CustomGetItemRectTreeView : TreeView
    {
        public RECT GetItemRectResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvoke.TVM_GETITEMRECT)
            {
                RECT* pRect = (RECT*)m.LParam;
                *pRect = GetItemRectResult;
                Assert.Equal(1, m.WParam);
                m.Result = 1;
                return;
            }

            base.WndProc(ref m);
        }
    }

    private class InvalidGetItemRectTreeView : TreeView
    {
        public bool MakeInvalid { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (MakeInvalid && m.Msg == (int)PInvoke.TVM_GETITEMRECT)
            {
                RECT* pRect = (RECT*)m.LParam;
                *pRect = new RECT(1, 2, 3, 4);
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }
    }

    private class SubTreeNode : TreeNode
    {
        public SubTreeNode(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
        {
        }

        public new void Deserialize(SerializationInfo serializationInfo, StreamingContext context) => base.Deserialize(serializationInfo, context);
    }
}

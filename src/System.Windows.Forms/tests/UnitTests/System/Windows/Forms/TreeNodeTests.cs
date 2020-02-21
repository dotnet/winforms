// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TreeNodeTests
    {
        [Fact]
        public void TreeNode_Ctor_Default()
        {
            var treeNode = new TreeNode();
            Assert.Equal(Color.Empty, treeNode.BackColor);
            Assert.Equal(Rectangle.Empty, treeNode.Bounds);
            Assert.False(treeNode.Checked);
            Assert.Null(treeNode.ContextMenuStrip);
            Assert.Null(treeNode.FirstNode);
            Assert.Equal(Color.Empty, treeNode.ForeColor);
            Assert.Throws<InvalidOperationException>(() => treeNode.FullPath);
            Assert.Equal(-1, treeNode.ImageIndex);
            Assert.Empty(treeNode.ImageKey);
            Assert.Equal(0, treeNode.Index);
            Assert.False(treeNode.IsEditing);
            Assert.False(treeNode.IsExpanded);
            Assert.False(treeNode.IsSelected);
            Assert.False(treeNode.IsVisible);
            Assert.Null(treeNode.LastNode);
            Assert.Equal(0, treeNode.Level);
            Assert.Empty(treeNode.Name);
            Assert.Null(treeNode.NextNode);
            Assert.Null(treeNode.NextVisibleNode);
            Assert.Null(treeNode.NodeFont);
            Assert.Empty(treeNode.Nodes);
            Assert.Same(treeNode.Nodes, treeNode.Nodes);
            Assert.Null(treeNode.Parent);
            Assert.Null(treeNode.PrevNode);
            Assert.Null(treeNode.PrevVisibleNode);
            Assert.Equal(-1, treeNode.SelectedImageIndex);
            Assert.Empty(treeNode.SelectedImageKey);
            Assert.Equal(-1, treeNode.StateImageIndex);
            Assert.Empty(treeNode.StateImageKey);
            Assert.Null(treeNode.Tag);
            Assert.Empty(treeNode.Text);
            Assert.Empty(treeNode.ToolTipText);
            Assert.Null(treeNode.TreeView);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void TreeNode_Ctor_String(string text, string expectedText)
        {
            var treeNode = new TreeNode(text);
            Assert.Equal(Color.Empty, treeNode.BackColor);
            Assert.Equal(Rectangle.Empty, treeNode.Bounds);
            Assert.False(treeNode.Checked);
            Assert.Null(treeNode.ContextMenuStrip);
            Assert.Null(treeNode.FirstNode);
            Assert.Equal(Color.Empty, treeNode.ForeColor);
            Assert.Throws<InvalidOperationException>(() => treeNode.FullPath);
            Assert.Equal(-1, treeNode.ImageIndex);
            Assert.Empty(treeNode.ImageKey);
            Assert.Equal(0, treeNode.Index);
            Assert.False(treeNode.IsEditing);
            Assert.False(treeNode.IsExpanded);
            Assert.False(treeNode.IsSelected);
            Assert.False(treeNode.IsVisible);
            Assert.Null(treeNode.LastNode);
            Assert.Equal(0, treeNode.Level);
            Assert.Empty(treeNode.Name);
            Assert.Null(treeNode.NextNode);
            Assert.Null(treeNode.NextVisibleNode);
            Assert.Null(treeNode.NodeFont);
            Assert.Empty(treeNode.Nodes);
            Assert.Same(treeNode.Nodes, treeNode.Nodes);
            Assert.Null(treeNode.Parent);
            Assert.Null(treeNode.PrevNode);
            Assert.Null(treeNode.PrevVisibleNode);
            Assert.Equal(-1, treeNode.SelectedImageIndex);
            Assert.Empty(treeNode.SelectedImageKey);
            Assert.Equal(-1, treeNode.StateImageIndex);
            Assert.Empty(treeNode.StateImageKey);
            Assert.Null(treeNode.Tag);
            Assert.Equal(expectedText, treeNode.Text);
            Assert.Empty(treeNode.ToolTipText);
            Assert.Null(treeNode.TreeView);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void TreeNode_BackColor_Set_GetReturnsExpected(Color value)
        {
            var treeNode = new TreeNode
            {
                BackColor = value
            };
            Assert.Equal(value, treeNode.BackColor);

            // Set same.
            treeNode.BackColor = value;
            Assert.Equal(value, treeNode.BackColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void TreeNode_BackColor_SetWithTreeView_GetReturnsExpected(Color value)
        {
            var treeView = new TreeView();
            int invalidatedCallCount = 0;
            treeView.Invalidated += (sender, e) => invalidatedCallCount++;
            var treeNode = new TreeNode();
            treeView.Nodes.Add(treeNode);

            treeNode.BackColor = value;
            Assert.Equal(value, treeNode.BackColor);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeNode.BackColor = value;
            Assert.Equal(value, treeNode.BackColor);
            Assert.Equal(0, invalidatedCallCount);
        }

        public static IEnumerable<object[]> BackColor_SetWithTreeViewWithHandle_TestData()
        {
            yield return new object[] { Color.Empty, 0 };
            yield return new object[] { Color.Red, 1 };
        }

        [Theory]
        [MemberData(nameof(BackColor_SetWithTreeViewWithHandle_TestData))]
        public void TreeNode_BackColor_SetWithTreeViewWithHandle_GetReturnsExpected(Color value, int expectedInvalidatedCallCount)
        {
            var treeView = new TreeView();
            int invalidatedCallCount = 0;
            treeView.Invalidated += (sender, e) => invalidatedCallCount++;
            var treeNode = new TreeNode();
            treeView.Nodes.Add(treeNode);
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeNode.BackColor = value;
            Assert.Equal(value, treeNode.BackColor);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Set same.
            treeNode.BackColor = value;
            Assert.Equal(value, treeNode.BackColor);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        }

        public static IEnumerable<object[]> BackColor_SetWithBackColor_TestData()
        {
            yield return new object[] { Color.Empty };
            yield return new object[] { Color.Blue };
            yield return new object[] { Color.Red };
        }

        [Theory]
        [MemberData(nameof(BackColor_SetWithBackColor_TestData))]
        public void TreeNode_BackColor_SetWithBackColor_GetReturnsExpected(Color value)
        {
            var treeNode = new TreeNode
            {
                BackColor = Color.Blue
            };

            treeNode.BackColor = value;
            Assert.Equal(value, treeNode.BackColor);

            // Set same.
            treeNode.BackColor = value;
            Assert.Equal(value, treeNode.BackColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void TreeNode_BackColor_SetWithBackColorWithTreeView_GetReturnsExpected(Color value)
        {
            var treeView = new TreeView();
            int invalidatedCallCount = 0;
            treeView.Invalidated += (sender, e) => invalidatedCallCount++;
            var treeNode = new TreeNode
            {
                BackColor = Color.Blue
            };
            treeView.Nodes.Add(treeNode);

            treeNode.BackColor = value;
            Assert.Equal(value, treeNode.BackColor);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeNode.BackColor = value;
            Assert.Equal(value, treeNode.BackColor);
            Assert.Equal(0, invalidatedCallCount);
        }

        public static IEnumerable<object[]> BackColor_SetWithBackColorWithTreeViewWithHandle_TestData()
        {
            yield return new object[] { Color.Empty, 1 };
            yield return new object[] { Color.Red, 1 };
            yield return new object[] { Color.Blue, 0 };
        }

        [Theory]
        [MemberData(nameof(BackColor_SetWithBackColorWithTreeViewWithHandle_TestData))]
        public void TreeNode_BackColor_SetWithBackColorWithTreeViewWithHandle_GetReturnsExpected(Color value, int expectedInvalidatedCallCount)
        {
            var treeView = new TreeView();
            int invalidatedCallCount = 0;
            treeView.Invalidated += (sender, e) => invalidatedCallCount++;
            var treeNode = new TreeNode
            {
                BackColor = Color.Blue
            };
            treeView.Nodes.Add(treeNode);
            Assert.NotEqual(IntPtr.Zero, treeNode.Handle);

            treeNode.BackColor = value;
            Assert.Equal(value, treeNode.BackColor);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Set same.
            treeNode.BackColor = value;
            Assert.Equal(value, treeNode.BackColor);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        }

        [Fact]
        public void TreeNode_Bounds_GetWithTreeView_CreatesHandle()
        {
            var treeView = new TreeView();
            TreeNodeCollection collection = treeView.Nodes;
            TreeNode treeNode = collection.Add("text");
            Assert.False(treeView.Created);
            Rectangle bounds = treeNode.Bounds;
            Assert.True(bounds.X > 0);
            Assert.Equal(0, bounds.Y);
            Assert.True(bounds.Width > 0);
            Assert.True(bounds.Height > 0);
            Assert.True(treeView.Created);
        }

        [Fact]
        public void TreeNode_Bounds_GetWithTreeViewDisposed_ReturnsEmpty()
        {
            var treeView = new TreeView();
            TreeNodeCollection collection = treeView.Nodes;
            TreeNode treeNode = collection.Add("text");
            treeView.Dispose();
            Assert.Equal(Rectangle.Empty, treeNode.Bounds);
            Assert.False(treeView.Created);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TreeNode_Checked_Set_GetReturnsExpected(bool value)
        {
            var treeNode = new TreeNode
            {
                Checked = value
            };
            Assert.Equal(value, treeNode.Checked);

            // Set same
            treeNode.Checked = value;
            Assert.Equal(value, treeNode.Checked);

            // Set different
            treeNode.Checked = !value;
            Assert.Equal(!value, treeNode.Checked);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TreeNode_Checked_SetWithTreeView_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView();
            var treeNode = new TreeNode();
            treeView.Nodes.Add(treeNode);

            treeNode.Checked = value;
            Assert.Equal(value, treeNode.Checked);

            // Set same
            treeNode.Checked = value;
            Assert.Equal(value, treeNode.Checked);

            // Set different
            treeNode.Checked = !value;
            Assert.Equal(!value, treeNode.Checked);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TreeNode_Checked_SetWithTreeViewWithHandle_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView();
            var treeNode = new TreeNode();
            treeView.Nodes.Add(treeNode);
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeNode.Checked = value;
            Assert.Equal(value, treeNode.Checked);

            // Set same
            treeNode.Checked = value;
            Assert.Equal(value, treeNode.Checked);

            // Set different
            treeNode.Checked = !value;
            Assert.Equal(!value, treeNode.Checked);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TreeNode_Checked_SetWithTreeViewDisposed_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView();
            var treeNode = new TreeNode();
            treeView.Nodes.Add(treeNode);
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);
            treeView.Dispose();

            treeNode.Checked = value;
            Assert.Equal(value, treeNode.Checked);

            // Set same
            treeNode.Checked = value;
            Assert.Equal(value, treeNode.Checked);

            // Set different
            treeNode.Checked = !value;
            Assert.Equal(!value, treeNode.Checked);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void TreeNode_ForeColor_Set_GetReturnsExpected(Color value)
        {
            var treeNode = new TreeNode
            {
                ForeColor = value
            };
            Assert.Equal(value, treeNode.ForeColor);

            // Set same.
            treeNode.ForeColor = value;
            Assert.Equal(value, treeNode.ForeColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void TreeNode_ForeColor_SetWithTreeView_GetReturnsExpected(Color value)
        {
            var treeView = new TreeView();
            int invalidatedCallCount = 0;
            treeView.Invalidated += (sender, e) => invalidatedCallCount++;
            var treeNode = new TreeNode();
            treeView.Nodes.Add(treeNode);

            treeNode.ForeColor = value;
            Assert.Equal(value, treeNode.ForeColor);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeNode.ForeColor = value;
            Assert.Equal(value, treeNode.ForeColor);
            Assert.Equal(0, invalidatedCallCount);
        }

        public static IEnumerable<object[]> ForeColor_SetWithTreeViewWithHandle_TestData()
        {
            yield return new object[] { Color.Empty, 0 };
            yield return new object[] { Color.Red, 1 };
        }

        [Theory]
        [MemberData(nameof(ForeColor_SetWithTreeViewWithHandle_TestData))]
        public void TreeNode_ForeColor_SetWithTreeViewWithHandle_GetReturnsExpected(Color value, int expectedInvalidatedCallCount)
        {
            var treeView = new TreeView();
            int invalidatedCallCount = 0;
            treeView.Invalidated += (sender, e) => invalidatedCallCount++;
            var treeNode = new TreeNode();
            treeView.Nodes.Add(treeNode);
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeNode.ForeColor = value;
            Assert.Equal(value, treeNode.ForeColor);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Set same.
            treeNode.ForeColor = value;
            Assert.Equal(value, treeNode.ForeColor);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        }

        public static IEnumerable<object[]> ForeColor_SetWithForeColor_TestData()
        {
            yield return new object[] { Color.Empty };
            yield return new object[] { Color.Blue };
            yield return new object[] { Color.Red };
        }

        [Theory]
        [MemberData(nameof(ForeColor_SetWithForeColor_TestData))]
        public void TreeNode_ForeColor_SetWithForeColor_GetReturnsExpected(Color value)
        {
            var treeNode = new TreeNode
            {
                ForeColor = Color.Blue
            };

            treeNode.ForeColor = value;
            Assert.Equal(value, treeNode.ForeColor);

            // Set same.
            treeNode.ForeColor = value;
            Assert.Equal(value, treeNode.ForeColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void TreeNode_ForeColor_SetWithForeColorWithTreeView_GetReturnsExpected(Color value)
        {
            var treeView = new TreeView();
            int invalidatedCallCount = 0;
            treeView.Invalidated += (sender, e) => invalidatedCallCount++;
            var treeNode = new TreeNode
            {
                ForeColor = Color.Blue
            };
            treeView.Nodes.Add(treeNode);

            treeNode.ForeColor = value;
            Assert.Equal(value, treeNode.ForeColor);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeNode.ForeColor = value;
            Assert.Equal(value, treeNode.ForeColor);
            Assert.Equal(0, invalidatedCallCount);
        }

        public static IEnumerable<object[]> ForeColor_SetWithForeColorWithTreeViewWithHandle_TestData()
        {
            yield return new object[] { Color.Empty, 1 };
            yield return new object[] { Color.Red, 1 };
            yield return new object[] { Color.Blue, 0 };
        }

        [Theory]
        [MemberData(nameof(ForeColor_SetWithForeColorWithTreeViewWithHandle_TestData))]
        public void TreeNode_ForeColor_SetWithForeColorWithTreeViewWithHandle_GetReturnsExpected(Color value, int expectedInvalidatedCallCount)
        {
            var treeView = new TreeView();
            int invalidatedCallCount = 0;
            treeView.Invalidated += (sender, e) => invalidatedCallCount++;
            var treeNode = new TreeNode
            {
                ForeColor = Color.Blue
            };
            treeView.Nodes.Add(treeNode);
            Assert.NotEqual(IntPtr.Zero, treeNode.Handle);

            treeNode.ForeColor = value;
            Assert.Equal(value, treeNode.ForeColor);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Set same.
            treeNode.ForeColor = value;
            Assert.Equal(value, treeNode.ForeColor);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        }

        [Fact]
        public void TreeNode_Handle_GetWithoutTreeView_ReturnsExpected()
        {
            var treeNode = new TreeNode();
            IntPtr handle = treeNode.Handle;
            Assert.Equal(IntPtr.Zero, handle);
            Assert.Equal(handle, treeNode.Handle);
        }

        [Fact]
        public void TreeNode_Handle_GetWithTreeView_ReturnsExpected()
        {
            var treeView = new TreeView();
            var treeNode = new TreeNode();
            treeView.Nodes.Add(treeNode);
            IntPtr handle = treeNode.Handle;
            Assert.NotEqual(IntPtr.Zero, handle);
            Assert.Equal(handle, treeNode.Handle);
            Assert.True(treeView.Created);
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);
        }

        [Fact]
        public void TreeNode_Handle_GetWithTreeViewDisposed_ReturnsExpected()
        {
            var treeView = new TreeView();
            var treeNode = new TreeNode();
            treeView.Nodes.Add(treeNode);
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);
            treeView.Dispose();
            IntPtr handle = treeNode.Handle;
            Assert.NotEqual(IntPtr.Zero, handle);
            Assert.Equal(handle, treeNode.Handle);
            Assert.False(treeView.Created);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Moq;
using WinForms.Common.Tests;
using Xunit;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms.Tests
{
    public class TreeViewTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void TreeView_Ctor_Default()
        {
            using var control = new SubTreeView();
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoSize);
            Assert.Equal(SystemColors.Window, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(BorderStyle.Fixed3D, control.BorderStyle);
            Assert.Equal(97, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 121, 97), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.False(control.CheckBoxes);
            Assert.Equal(117, control.ClientSize.Width);
            Assert.Equal(93, control.ClientSize.Height);
            Assert.Equal(new Rectangle(0, 0, 117, 93), control.ClientRectangle);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(new Size(121, 97), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 117, 93), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.DoubleBuffered);
            Assert.Equal(TreeViewDrawMode.Normal, control.DrawMode);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.False(control.FullRowSelect);
            Assert.False(control.HasChildren);
            Assert.Equal(97, control.Height);
            Assert.True(control.HideSelection);
            Assert.False(control.HotTracking);
            Assert.Equal(-1, control.ImageIndex);
            Assert.Equal(string.Empty, control.ImageKey);
            Assert.Null(control.ImageList);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.Equal(19, control.Indent);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.Equal(Control.DefaultFont.Height + 3, control.ItemHeight);
            Assert.False(control.LabelEdit);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Equal(Color.Empty, control.LineColor);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Same(control.Nodes, control.Nodes);
            Assert.Empty(control.Nodes);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal("\\", control.PathSeparator);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.Equal(new Size(121, 97), control.PreferredSize);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(121, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.False(control.RightToLeftLayout);
            Assert.True(control.Scrollable);
            Assert.Equal(-1, control.SelectedImageIndex);
            Assert.Equal(string.Empty, control.SelectedImageKey);
            Assert.Null(control.SelectedNode);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowLines);
            Assert.True(control.ShowKeyboardCues);
            Assert.False(control.ShowNodeToolTips);
            Assert.True(control.ShowPlusMinus);
            Assert.True(control.ShowRootLines);
            Assert.Null(control.Site);
            Assert.Equal(new Size(121, 97), control.Size);
            Assert.False(control.Sorted);
            Assert.Null(control.StateImageList);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.Null(control.TopNode);
            Assert.Null(control.TreeViewNodeSorter);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.Equal(0, control.VisibleCount);
            Assert.Equal(121, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeView_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubTreeView();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTreeView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56010007, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeView_CreateParams_GetDefaultWithHandle_ReturnsExpected()
        {
            using var control = new SubTreeView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTreeView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56010007, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(BorderStyle.None, 0x56010007, 0)]
        [InlineData(BorderStyle.Fixed3D, 0x56010007, 0x200)]
        [InlineData(BorderStyle.FixedSingle, 0x56810007, 0)]
        public void TreeView_CreateParams_GetBorder_ReturnsExpected(BorderStyle borderStyle, int expectedStyle, int expectedExStyle)
        {
            using var control = new SubTreeView
            {
                BorderStyle = borderStyle
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTreeView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(expectedExStyle, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0x56010007)]
        [InlineData(false, 0x56012007)]
        public void TreeView_CreateParams_GetScrollable_ReturnsExpected(bool scrollable, int expectedStyle)
        {
            using var control = new SubTreeView
            {
                Scrollable = scrollable
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTreeView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0x56010007)]
        [InlineData(false, 0x56010027)]
        public void TreeView_CreateParams_GetHideSelection_ReturnsExpected(bool hideSelection, int expectedStyle)
        {
            using var control = new SubTreeView
            {
                HideSelection = hideSelection
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTreeView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0x5601000F)]
        [InlineData(false, 0x56010007)]
        public void TreeView_CreateParams_GetLabelEdit_ReturnsExpected(bool labelEdit, int expectedStyle)
        {
            using var control = new SubTreeView
            {
                LabelEdit = labelEdit
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTreeView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0x56010007)]
        [InlineData(false, 0x56010005)]
        public void TreeView_CreateParams_GetShowLines_ReturnsExpected(bool showLines, int expectedStyle)
        {
            using var control = new SubTreeView
            {
                ShowLines = showLines
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTreeView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0x56010007)]
        [InlineData(false, 0x56010006)]
        public void TreeView_CreateParams_GetShowPlusMinus_ReturnsExpected(bool showPlusMinus, int expectedStyle)
        {
            using var control = new SubTreeView
            {
                ShowPlusMinus = showPlusMinus
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTreeView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0x56010007)]
        [InlineData(false, 0x56010003)]
        public void TreeView_CreateParams_GetShowRootLines_ReturnsExpected(bool showRootLines, int expectedStyle)
        {
            using var control = new SubTreeView
            {
                ShowRootLines = showRootLines
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTreeView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0x56010207)]
        [InlineData(false, 0x56010007)]
        public void TreeView_CreateParams_GetHotTracking_ReturnsExpected(bool hotTracking, int expectedStyle)
        {
            using var control = new SubTreeView
            {
                HotTracking = hotTracking
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTreeView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0x56011007)]
        [InlineData(false, 0x56010007)]
        public void TreeView_CreateParams_GetFullRowSelect_ReturnsExpected(bool fullRowSelect, int expectedStyle)
        {
            using var control = new SubTreeView
            {
                FullRowSelect = fullRowSelect
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTreeView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(1)]
        [InlineData(2)]
        public void TreeView_CreateParams_GetItemHeight_ReturnsExpected(int value)
        {
            using var control = new SubTreeView
            {
                ItemHeight = value
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTreeView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56010007, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(1)]
        [InlineData(2)]
        public void TreeView_CreateParams_GetItemHeightWithHandle_ReturnsExpected(int itemHeight)
        {
            using var control = new SubTreeView
            {
                ItemHeight = itemHeight
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTreeView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56010007, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(11, 0x56014007, 1)]
        [InlineData(12, 0x56010007, 0)]
        public void TreeView_CreateParams_GetItemHeightInSetterWithHandle_ReturnsExpected(int itemHeight, int expectedStyle, int expectedCreatedCallCount)
        {
            using var control = new SubTreeView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) =>
            {
                CreateParams createParams = control.CreateParams;
                Assert.Null(createParams.Caption);
                Assert.Equal("SysTreeView32", createParams.ClassName);
                Assert.Equal(0x8, createParams.ClassStyle);
                Assert.Equal(0x200, createParams.ExStyle);
                Assert.Equal(97, createParams.Height);
                Assert.Equal(IntPtr.Zero, createParams.Parent);
                Assert.Null(createParams.Param);
                Assert.Equal(expectedStyle, createParams.Style);
                Assert.Equal(121, createParams.Width);
                Assert.Equal(0, createParams.X);
                Assert.Equal(0, createParams.Y);
                Assert.Same(createParams, control.CreateParams);
                Assert.True(control.IsHandleCreated);
                createdCallCount++;
            };
            control.ItemHeight = itemHeight;
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void TreeView_CreateParams_GetShowNodeTooltips_ReturnsExpected(bool designMode, bool showNodeTooltips)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(designMode);
            using var control = new SubTreeView
            {
                Site = mockSite.Object,
                ShowNodeToolTips = showNodeTooltips
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTreeView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56010007, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, 0x56010007)]
        [InlineData(true, false, 0x56010007)]
        [InlineData(false, true, 0x56010807)]
        [InlineData(false, false, 0x56010007)]
        public void TreeView_CreateParams_GetShowNodeTooltipsWithHandle_ReturnsExpected(bool designMode, bool showNodeTooltips, int expectedStyle)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(designMode);
            using var control = new SubTreeView
            {
                Site = mockSite.Object,
                ShowNodeToolTips = showNodeTooltips
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTreeView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TreeView_CreateParams_GetCheckBoxes_ReturnsExpected(bool checkBoxes)
        {
            using var control = new SubTreeView
            {
                CheckBoxes = checkBoxes
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTreeView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56010007, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0x56010107)]
        [InlineData(false, 0x56010007)]
        public void TreeView_CreateParams_GetCheckBoxesWithHandle_ReturnsExpected(bool checkBoxes, int expectedStyle)
        {
            using var control = new SubTreeView
            {
                CheckBoxes = checkBoxes
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTreeView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(RightToLeft.Inherit, true, 0x56010007, 0x200)]
        [InlineData(RightToLeft.No, true, 0x56010007, 0x200)]
        [InlineData(RightToLeft.Yes, true, 0x56010007, 0x400200)]
        [InlineData(RightToLeft.Inherit, false, 0x56010007, 0x200)]
        [InlineData(RightToLeft.No, false, 0x56010007, 0x200)]
        [InlineData(RightToLeft.Yes, false, 0x56010047, 0x7200)]
        public void TreeView_CreateParams_GetRightToLeft_ReturnsExpected(RightToLeft rightToLeft, bool rightToLeftLayout, int expectedStyle, int expectedExStyle)
        {
            using var control = new SubTreeView
            {
                RightToLeft = rightToLeft,
                RightToLeftLayout = rightToLeftLayout
            };
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTreeView32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(expectedExStyle, createParams.ExStyle);
            Assert.Equal(97, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(121, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
        }

        public static IEnumerable<object[]> BackColor_Set_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.Window };
            yield return new object[] { Color.Red, Color.Red };
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_Set_TestData))]
        public void TreeView_BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new TreeView
            {
                BackColor = value
            };
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> BackColor_SetWithHandle_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.Window, 0 };
            yield return new object[] { Color.Red, Color.Red, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_SetWithHandle_TestData))]
        public void TreeView_BackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
        {
            using var control = new TreeView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void BackColor_SetWithHandler_CallsBackColorChanged()
        {
            using var control = new TreeView();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BackColorChanged += handler;

            // Set different.
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(1, callCount);

            // Set same.
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(1, callCount);

            // Set different.
            control.BackColor = Color.Empty;
            Assert.Equal(SystemColors.Window, control.BackColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void TreeView_BackgroundImage_Set_GetReturnsExpected(Image value)
        {
            using var control = new TreeView
            {
                BackgroundImage = value
            };
            Assert.Same(value, control.BackgroundImage);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackgroundImage = value;
            Assert.Same(value, control.BackgroundImage);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeView_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
        {
            using var control = new TreeView();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BackgroundImageChanged += handler;

            // Set different.
            using var image1 = new Bitmap(10, 10);
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Equal(1, callCount);

            // Set same.
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Equal(1, callCount);

            // Set different.
            using var image2 = new Bitmap(10, 10);
            control.BackgroundImage = image2;
            Assert.Same(image2, control.BackgroundImage);
            Assert.Equal(2, callCount);

            // Set null.
            control.BackgroundImage = null;
            Assert.Null(control.BackgroundImage);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.BackgroundImageChanged -= handler;
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Equal(3, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void TreeView_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
        {
            using var control = new TreeView
            {
                BackgroundImageLayout = value
            };
            Assert.Equal(value, control.BackgroundImageLayout);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackgroundImageLayout = value;
            Assert.Equal(value, control.BackgroundImageLayout);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeView_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
        {
            using var control = new TreeView();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BackgroundImageLayoutChanged += handler;

            // Set different.
            control.BackgroundImageLayout = ImageLayout.Center;
            Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
            Assert.Equal(1, callCount);

            // Set same.
            control.BackgroundImageLayout = ImageLayout.Center;
            Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
            Assert.Equal(1, callCount);

            // Set different.
            control.BackgroundImageLayout = ImageLayout.Stretch;
            Assert.Equal(ImageLayout.Stretch, control.BackgroundImageLayout);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.BackgroundImageLayoutChanged -= handler;
            control.BackgroundImageLayout = ImageLayout.Center;
            Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ImageLayout))]
        public void BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
        {
            using var control = new TreeView();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(BorderStyle))]
        public void BorderStyle_Set_GetReturnsExpected(BorderStyle value)
        {
            using var treeView = new TreeView
            {
                BorderStyle = value
            };
            Assert.Equal(value, treeView.BorderStyle);

            // Set same.
            treeView.BorderStyle = value;
            Assert.Equal(value, treeView.BorderStyle);
        }

        [WinFormsFact]
        public void BorderStyle_SetWithUpdateStylesHandler_CallsStyleChangedDoesNotCallInvalidated()
        {
            using var treeView = new TreeView
            {
                BorderStyle = BorderStyle.Fixed3D
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;

            // Set different.
            treeView.BorderStyle = BorderStyle.None;
            Assert.Equal(BorderStyle.None, treeView.BorderStyle);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.BorderStyle = BorderStyle.None;
            Assert.Equal(BorderStyle.None, treeView.BorderStyle);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.BorderStyle = BorderStyle.FixedSingle;
            Assert.Equal(BorderStyle.FixedSingle, treeView.BorderStyle);
            Assert.Equal(2, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.BorderStyle = BorderStyle.Fixed3D;
            Assert.Equal(BorderStyle.Fixed3D, treeView.BorderStyle);
            Assert.Equal(2, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsFact]
        public void BorderStyle_SetWithInvalidatedWithHandle_CallsStyleChangedCallsInvalidated()
        {
            using var treeView = new TreeView
            {
                BorderStyle = BorderStyle.Fixed3D
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            // Set different.
            treeView.BorderStyle = BorderStyle.None;
            Assert.Equal(BorderStyle.None, treeView.BorderStyle);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set same.
            treeView.BorderStyle = BorderStyle.None;
            Assert.Equal(BorderStyle.None, treeView.BorderStyle);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set different.
            treeView.BorderStyle = BorderStyle.FixedSingle;
            Assert.Equal(BorderStyle.FixedSingle, treeView.BorderStyle);
            Assert.Equal(2, styleChangedCallCount);
            Assert.Equal(2, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.BorderStyle = BorderStyle.Fixed3D;
            Assert.Equal(BorderStyle.Fixed3D, treeView.BorderStyle);
            Assert.Equal(2, styleChangedCallCount);
            Assert.Equal(2, invalidatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(BorderStyle))]
        public void BorderStyle_SetInvalid_ThrowsInvalidEnumArgumentException(BorderStyle value)
        {
            using var treeView = new TreeView();
            Assert.Throws<InvalidEnumArgumentException>("value", () => treeView.BorderStyle = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void CheckBoxes_Set_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView
            {
                CheckBoxes = value
            };
            Assert.Equal(value, treeView.CheckBoxes);

            // Set same.
            treeView.CheckBoxes = value;
            Assert.Equal(value, treeView.CheckBoxes);

            // Set different.
            treeView.CheckBoxes = !value;
            Assert.Equal(!value, treeView.CheckBoxes);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void CheckBoxes_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.CheckBoxes = value;
            Assert.Equal(value, treeView.CheckBoxes);

            // Set same.
            treeView.CheckBoxes = value;
            Assert.Equal(value, treeView.CheckBoxes);

            // Set different.
            treeView.CheckBoxes = !value;
            Assert.Equal(!value, treeView.CheckBoxes);
        }

        [WinFormsFact]
        public void CheckBoxes_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            using var treeView = new TreeView
            {
                CheckBoxes = false
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;

            // Set different.
            treeView.CheckBoxes = true;
            Assert.True(treeView.CheckBoxes);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.CheckBoxes = true;
            Assert.True(treeView.CheckBoxes);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.CheckBoxes = false;
            Assert.False(treeView.CheckBoxes);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.CheckBoxes = true;
            Assert.True(treeView.CheckBoxes);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsFact]
        public void CheckBoxes_SetWithUpdateStylesHandlerWithHandle_CallsStyleChangedCallsInvalidated()
        {
            using var treeView = new TreeView
            {
                CheckBoxes = false
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            // Set different.
            treeView.CheckBoxes = true;
            Assert.True(treeView.CheckBoxes);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set same.
            treeView.CheckBoxes = true;
            Assert.True(treeView.CheckBoxes);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set different.
            treeView.CheckBoxes = false;
            Assert.False(treeView.CheckBoxes);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.CheckBoxes = true;
            Assert.True(treeView.CheckBoxes);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DoubleBuffered_Set_GetReturnsExpected(bool value)
        {
            using var treeView = new SubTreeView
            {
                DoubleBuffered = value
            };
            Assert.Equal(value, treeView.DoubleBuffered);

            // Set same.
            treeView.DoubleBuffered = value;
            Assert.Equal(value, treeView.DoubleBuffered);

            // Set different.
            treeView.DoubleBuffered = !value;
            Assert.Equal(!value, treeView.DoubleBuffered);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DoubleBuffered_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var treeView = new SubTreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.DoubleBuffered = value;
            Assert.Equal(value, treeView.DoubleBuffered);

            // Set same.
            treeView.DoubleBuffered = value;
            Assert.Equal(value, treeView.DoubleBuffered);

            // Set different.
            treeView.DoubleBuffered = !value;
            Assert.Equal(!value, treeView.DoubleBuffered);
        }

        [WinFormsFact]
        public void DoubleBuffered_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            using var treeView = new SubTreeView
            {
                DoubleBuffered = false
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;

            // Set different.
            treeView.DoubleBuffered = true;
            Assert.True(treeView.DoubleBuffered);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.DoubleBuffered = true;
            Assert.True(treeView.DoubleBuffered);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.DoubleBuffered = false;
            Assert.False(treeView.DoubleBuffered);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.DoubleBuffered = true;
            Assert.True(treeView.DoubleBuffered);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsFact]
        public void DoubleBuffered_SetWithUpdateStylesHandlerWithHandle_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            using var treeView = new SubTreeView
            {
                DoubleBuffered = false
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            // Set different.
            treeView.DoubleBuffered = true;
            Assert.True(treeView.DoubleBuffered);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.DoubleBuffered = true;
            Assert.True(treeView.DoubleBuffered);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.DoubleBuffered = false;
            Assert.False(treeView.DoubleBuffered);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.DoubleBuffered = true;
            Assert.True(treeView.DoubleBuffered);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TreeViewDrawMode))]
        public void DrawMode_Set_GetReturnsExpected(TreeViewDrawMode value)
        {
            using var control = new TreeView
            {
                DrawMode = value
            };
            Assert.Equal(value, control.DrawMode);

            // Set same.
            control.DrawMode = value;
            Assert.Equal(value, control.DrawMode);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TreeViewDrawMode))]
        public void DrawMode_SetWithHandle_GetReturnsExpected(TreeViewDrawMode value)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.DrawMode = value;
            Assert.Equal(value, treeView.DrawMode);

            // Set same.
            treeView.DrawMode = value;
            Assert.Equal(value, treeView.DrawMode);
        }

        [WinFormsFact]
        public void DrawMode_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            using var treeView = new SubTreeView
            {
                DrawMode = TreeViewDrawMode.Normal
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;

            // Set different.
            treeView.DrawMode = TreeViewDrawMode.OwnerDrawText;
            Assert.Equal(TreeViewDrawMode.OwnerDrawText, treeView.DrawMode);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.DrawMode = TreeViewDrawMode.OwnerDrawText;
            Assert.Equal(TreeViewDrawMode.OwnerDrawText, treeView.DrawMode);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.DrawMode = TreeViewDrawMode.OwnerDrawAll;
            Assert.Equal(TreeViewDrawMode.OwnerDrawAll, treeView.DrawMode);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.DrawMode = TreeViewDrawMode.Normal;
            Assert.Equal(TreeViewDrawMode.Normal, treeView.DrawMode);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsFact]
        public void DrawMode_SetWithUpdateStylesHandlerWithHandle_DoesNotCallStyleChangedCallsInvalidated()
        {
            using var treeView = new SubTreeView
            {
                DrawMode = TreeViewDrawMode.Normal
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            // Set different.
            treeView.DrawMode = TreeViewDrawMode.OwnerDrawText;
            Assert.Equal(TreeViewDrawMode.OwnerDrawText, treeView.DrawMode);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set same.
            treeView.DrawMode = TreeViewDrawMode.OwnerDrawText;
            Assert.Equal(TreeViewDrawMode.OwnerDrawText, treeView.DrawMode);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set different.
            treeView.DrawMode = TreeViewDrawMode.OwnerDrawAll;
            Assert.Equal(TreeViewDrawMode.OwnerDrawAll, treeView.DrawMode);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(2, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.DrawMode = TreeViewDrawMode.Normal;
            Assert.Equal(TreeViewDrawMode.Normal, treeView.DrawMode);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(2, invalidatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TreeViewDrawMode))]
        public void DrawMode_SetInvalid_ThrowsInvalidEnumArgumentException(TreeViewDrawMode value)
        {
            using var treeView = new TreeView();
            Assert.Throws<InvalidEnumArgumentException>("value", () => treeView.DrawMode = value);
        }

        public static IEnumerable<object[]> ForeColor_Set_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.WindowText };
            yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3) };
            yield return new object[] { Color.White, Color.White };
            yield return new object[] { Color.Black, Color.Black };
            yield return new object[] { Color.Red, Color.Red };
        }

        [WinFormsTheory]
        [MemberData(nameof(ForeColor_Set_TestData))]
        public void ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new TreeView
            {
                ForeColor = value
            };
            Assert.Equal(expected, control.ForeColor);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
        }

        [WinFormsTheory]
        [MemberData(nameof(ForeColor_Set_TestData))]
        public void ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new TreeView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
        }

        [WinFormsFact]
        public void ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            using var control = new TreeView();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.ForeColorChanged += handler;

            // Set different.
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(1, callCount);

            // Set same.
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(1, callCount);

            // Set different.
            control.ForeColor = Color.Empty;
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ForeColorChanged -= handler;
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FullRowSelect_Set_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView
            {
                FullRowSelect = value
            };
            Assert.Equal(value, treeView.FullRowSelect);

            // Set same.
            treeView.FullRowSelect = value;
            Assert.Equal(value, treeView.FullRowSelect);

            // Set different.
            treeView.FullRowSelect = !value;
            Assert.Equal(!value, treeView.FullRowSelect);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FullRowSelect_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.FullRowSelect = value;
            Assert.Equal(value, treeView.FullRowSelect);

            // Set same.
            treeView.FullRowSelect = value;
            Assert.Equal(value, treeView.FullRowSelect);

            // Set different.
            treeView.FullRowSelect = !value;
            Assert.Equal(!value, treeView.FullRowSelect);
        }

        [WinFormsFact]
        public void FullRowSelect_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            using var treeView = new TreeView
            {
                FullRowSelect = false
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;

            // Set different.
            treeView.FullRowSelect = true;
            Assert.True(treeView.FullRowSelect);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.FullRowSelect = true;
            Assert.True(treeView.FullRowSelect);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.FullRowSelect = false;
            Assert.False(treeView.FullRowSelect);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.FullRowSelect = true;
            Assert.True(treeView.FullRowSelect);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsFact]
        public void FullRowSelect_SetWithUpdateStylesHandlerWithHandle_CallsStyleChangedCallsInvalidated()
        {
            using var treeView = new TreeView
            {
                FullRowSelect = false
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            // Set different.
            treeView.FullRowSelect = true;
            Assert.True(treeView.FullRowSelect);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set same.
            treeView.FullRowSelect = true;
            Assert.True(treeView.FullRowSelect);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set different.
            treeView.FullRowSelect = false;
            Assert.False(treeView.FullRowSelect);
            Assert.Equal(2, styleChangedCallCount);
            Assert.Equal(2, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.FullRowSelect = true;
            Assert.True(treeView.FullRowSelect);
            Assert.Equal(2, styleChangedCallCount);
            Assert.Equal(2, invalidatedCallCount);
        }

        [WinFormsFact]
        public void TreeView_Handle_GetVersion_ReturnsExpected()
        {
            using var control = new TreeView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal((IntPtr)5, User32.SendMessageW(control.Handle, (User32.WM)CCM.GETVERSION));
        }

        public static IEnumerable<object[]> Handle_CustomGetVersion_TestData()
        {
            yield return new object[] { IntPtr.Zero, 1 };
            yield return new object[] { (IntPtr)4, 1 };
            yield return new object[] { (IntPtr)5, 0 };
            yield return new object[] { (IntPtr)6, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Handle_CustomGetVersion_TestData))]
        public void TreeView_Handle_CustomGetVersion_Success(IntPtr getVersionResult, int expectedSetVersionCallCount)
        {
            using var control = new CustomGetVersionTreeView
            {
                GetVersionResult = getVersionResult
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(expectedSetVersionCallCount, control.SetVersionCallCount);
        }

        private class CustomGetVersionTreeView : TreeView
        {
            public IntPtr GetVersionResult { get; set; }
            public int SetVersionCallCount { get; set; }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == (int)CCM.GETVERSION)
                {
                    Assert.Equal(IntPtr.Zero, m.WParam);
                    Assert.Equal(IntPtr.Zero, m.LParam);
                    m.Result = GetVersionResult;
                    return;
                }
                else if (m.Msg == (int)CCM.SETVERSION)
                {
                    Assert.Equal((IntPtr)5, m.WParam);
                    Assert.Equal(IntPtr.Zero, m.LParam);
                    SetVersionCallCount++;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void HideSelection_Set_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView
            {
                HideSelection = value
            };
            Assert.Equal(value, treeView.HideSelection);

            // Set same.
            treeView.HideSelection = value;
            Assert.Equal(value, treeView.HideSelection);

            // Set different.
            treeView.HideSelection = !value;
            Assert.Equal(!value, treeView.HideSelection);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void HideSelection_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.HideSelection = value;
            Assert.Equal(value, treeView.HideSelection);

            // Set same.
            treeView.HideSelection = value;
            Assert.Equal(value, treeView.HideSelection);

            // Set different.
            treeView.HideSelection = !value;
            Assert.Equal(!value, treeView.HideSelection);
        }

        [WinFormsFact]
        public void HideSelection_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            using var treeView = new TreeView
            {
                HideSelection = true
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;

            // Set different.
            treeView.HideSelection = false;
            Assert.False(treeView.HideSelection);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.HideSelection = false;
            Assert.False(treeView.HideSelection);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.HideSelection = true;
            Assert.True(treeView.HideSelection);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.HideSelection = false;
            Assert.False(treeView.HideSelection);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsFact]
        public void HideSelection_SetWithUpdateStylesHandlerWithHandle_CallsStyleChangedCallsInvalidated()
        {
            using var treeView = new TreeView
            {
                HideSelection = true
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            // Set different.
            treeView.HideSelection = false;
            Assert.False(treeView.HideSelection);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set same.
            treeView.HideSelection = false;
            Assert.False(treeView.HideSelection);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set different.
            treeView.HideSelection = true;
            Assert.True(treeView.HideSelection);
            Assert.Equal(2, styleChangedCallCount);
            Assert.Equal(2, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.HideSelection = false;
            Assert.False(treeView.HideSelection);
            Assert.Equal(2, styleChangedCallCount);
            Assert.Equal(2, invalidatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void HotTracking_Set_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView
            {
                HotTracking = value
            };
            Assert.Equal(value, treeView.HotTracking);

            // Set same.
            treeView.HotTracking = value;
            Assert.Equal(value, treeView.HotTracking);

            // Set different.
            treeView.HotTracking = !value;
            Assert.Equal(!value, treeView.HotTracking);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void HotTracking_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.HotTracking = value;
            Assert.Equal(value, treeView.HotTracking);

            // Set same.
            treeView.HotTracking = value;
            Assert.Equal(value, treeView.HotTracking);

            // Set different.
            treeView.HotTracking = !value;
            Assert.Equal(!value, treeView.HotTracking);
        }

        [WinFormsFact]
        public void HotTracking_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            using var treeView = new TreeView
            {
                HotTracking = false
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;

            // Set different.
            treeView.HotTracking = true;
            Assert.True(treeView.HotTracking);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.HotTracking = true;
            Assert.True(treeView.HotTracking);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.HotTracking = false;
            Assert.False(treeView.HotTracking);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.HotTracking = true;
            Assert.True(treeView.HotTracking);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsFact]
        public void HotTracking_SetWithUpdateStylesHandlerWithHandle_CallsStyleChangedCallsInvalidated()
        {
            using var treeView = new TreeView
            {
                HotTracking = false
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            // Set different.
            treeView.HotTracking = true;
            Assert.True(treeView.HotTracking);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set same.
            treeView.HotTracking = true;
            Assert.True(treeView.HotTracking);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set different.
            treeView.HotTracking = false;
            Assert.False(treeView.HotTracking);
            Assert.Equal(2, styleChangedCallCount);
            Assert.Equal(2, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.HotTracking = true;
            Assert.True(treeView.HotTracking);
            Assert.Equal(2, styleChangedCallCount);
            Assert.Equal(2, invalidatedCallCount);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ImageIndex_SetWithoutImageList_GetReturnsExpected(int value)
        {
            using var treeView = new TreeView
            {
                ImageIndex = value
            };
            Assert.Equal(-1, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);

            // Set same.
            treeView.ImageIndex = value;
            Assert.Equal(-1, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ImageIndex_SetWithoutImageListWithImageKey_GetReturnsExpected(int value)
        {
            using var treeView = new TreeView
            {
                ImageKey = "imageKey",
                ImageIndex = value
            };
            Assert.Equal(-1, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);

            // Set same.
            treeView.ImageIndex = value;
            Assert.Equal(-1, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ImageIndex_SetWithEmptyImageList_GetReturnsExpected(int value)
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                ImageList = imageList,
                ImageIndex = value
            };
            Assert.Equal(0, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);

            // Set same.
            treeView.ImageIndex = value;
            Assert.Equal(0, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ImageIndex_SetWithEmptyImageListWithImageKey_GetReturnsExpected(int value)
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                ImageKey = "imageKey",
                ImageList = imageList,
                ImageIndex = value
            };
            Assert.Equal(0, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);

            // Set same.
            treeView.ImageIndex = value;
            Assert.Equal(0, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);
        }

        [WinFormsTheory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        public void ImageIndex_SetWithImageList_GetReturnsExpected(int value, int expected)
        {
            using var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add(new Bitmap(10, 10));
            using var treeView = new TreeView
            {
                ImageList = imageList,
                ImageIndex = value
            };
            Assert.Equal(expected, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);

            // Set same.
            treeView.ImageIndex = value;
            Assert.Equal(expected, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);
        }

        [WinFormsTheory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        public void ImageIndex_SetWithImageListWithImageKey_GetReturnsExpected(int value, int expected)
        {
            using var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add("imageKey", new Bitmap(10, 10));
            using var treeView = new TreeView
            {
                ImageKey = "imageKey",
                ImageList = imageList,
                ImageIndex = value
            };
            Assert.Equal(expected, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);

            // Set same.
            treeView.ImageIndex = value;
            Assert.Equal(expected, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ImageIndex_SetWithoutImageListWithHandle_GetReturnsExpected(int value)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.ImageIndex = value;
            Assert.Equal(-1, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);

            // Set same.
            treeView.ImageIndex = value;
            Assert.Equal(-1, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ImageIndex_SetWithEmptyImageListWithHandle_GetReturnsExpected(int value)
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                ImageList = imageList
            };
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.ImageIndex = value;
            Assert.Equal(0, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);

            // Set same.
            treeView.ImageIndex = value;
            Assert.Equal(0, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);
        }

        [WinFormsTheory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        public void ImageIndex_SetWithImageListWithHandle_GetReturnsExpected(int value, int expected)
        {
            using var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add(new Bitmap(10, 10));
            using var treeView = new TreeView
            {
                ImageList = imageList
            };
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);

            treeView.ImageIndex = value;
            Assert.Equal(expected, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);

            // Set same.
            treeView.ImageIndex = value;
            Assert.Equal(expected, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);
        }

        [WinFormsFact]
        public void ImageIndex_SetInvalid_Throws()
        {
            using var treeView = new TreeView();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => treeView.ImageIndex = -2);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("(none)", "")]
        public void ImageKey_SetWithoutImageList_GetReturnsExpected(string value, string expected)
        {
            using var treeView = new TreeView
            {
                ImageKey = value
            };
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(-1, treeView.ImageIndex);

            // Set same.
            treeView.ImageKey = value;
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(-1, treeView.ImageIndex);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("(none)", "")]
        public void ImageKey_SetWithoutImageListWithImageIndex_GetReturnsExpected(string value, string expected)
        {
            using var treeView = new TreeView
            {
                ImageIndex = 1,
                ImageKey = value
            };
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(-1, treeView.ImageIndex);

            // Set same.
            treeView.ImageKey = value;
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(-1, treeView.ImageIndex);
        }

        public static IEnumerable<object[]> ImageKey_Set_TestData()
        {
            yield return new object[] { null, "", 0 };
            yield return new object[] { "", "", 0 };
            yield return new object[] { "reasonable", "reasonable", -1 };
            yield return new object[] { "(none)", "", 0 };
            yield return new object[] { "imageKey", "imageKey", -1 };
            yield return new object[] { "ImageKey", "ImageKey", -1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void ImageKey_SetWithEmptyImageList_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                ImageList = imageList,
                ImageKey = value
            };
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(expectedImageIndex, treeView.ImageIndex);

            // Set same.
            treeView.ImageKey = value;
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(expectedImageIndex, treeView.ImageIndex);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void ImageKey_SetWithEmptyImageListWithImageIndex_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                ImageIndex = 1,
                ImageList = imageList,
                ImageKey = value
            };
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(expectedImageIndex, treeView.ImageIndex);

            // Set same.
            treeView.ImageKey = value;
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(expectedImageIndex, treeView.ImageIndex);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void ImageKey_SetWithNonEmptyImageList_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            using var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add("imageKey", new Bitmap(10, 10));
            using var treeView = new TreeView
            {
                ImageList = imageList,
                ImageKey = value
            };
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(expectedImageIndex, treeView.ImageIndex);

            // Set same.
            treeView.ImageKey = value;
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(expectedImageIndex, treeView.ImageIndex);
        }

        [WinFormsTheory]
        [InlineData(null, "", 0)]
        [InlineData("", "", 1)]
        [InlineData("reasonable", "reasonable", -1)]
        [InlineData("(none)", "", 0)]
        [InlineData("imageKey", "imageKey", -1)]
        [InlineData("ImageKey", "ImageKey", -1)]
        public void ImageKey_SetWithNonEmptyImageListWithImageIndex_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            using var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add("imageKey", new Bitmap(10, 10));
            using var treeView = new TreeView
            {
                ImageIndex = 1,
                ImageList = imageList,
                ImageKey = value
            };
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(expectedImageIndex, treeView.ImageIndex);

            // Set same.
            treeView.ImageKey = value;
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(expectedImageIndex, treeView.ImageIndex);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("(none)", "")]
        public void ImageKey_SetWithoutImageListWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.ImageKey = value;
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(-1, treeView.ImageIndex);

            // Set same.
            treeView.ImageKey = value;
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(-1, treeView.ImageIndex);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void ImageKey_SetWithEmptyImageListWithHandle_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                ImageList = imageList
            };
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.ImageKey = value;
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(expectedImageIndex, treeView.ImageIndex);

            // Set same.
            treeView.ImageKey = value;
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(expectedImageIndex, treeView.ImageIndex);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void ImageKey_SetWithNonEmptyImageListWithHandle_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            using var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add("imageKey", new Bitmap(10, 10));
            using var treeView = new TreeView
            {
                ImageList = imageList
            };
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.ImageKey = value;
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(expectedImageIndex, treeView.ImageIndex);

            // Set same.
            treeView.ImageKey = value;
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(expectedImageIndex, treeView.ImageIndex);
        }

        public static IEnumerable<object[]> ImageList_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ImageList() };

            using var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            yield return new object[] { imageList };
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageList_TestData))]
        public void ImageList_Set_GetReturnsExpected(ImageList value)
        {
            using var treeView = new TreeView
            {
                ImageList = value
            };
            Assert.Same(value, treeView.ImageList);

            // Set same.
            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageList_TestData))]
        public void ImageList_SetWithCheckboxes_GetReturnsExpected(ImageList value)
        {
            using var treeView = new TreeView
            {
                CheckBoxes = true,
                ImageList = value
            };
            Assert.Same(value, treeView.ImageList);

            // Set same.
            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageList_TestData))]
        public void ImageList_SetWithNonNullOldValue_GetReturnsExpected(ImageList value)
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                ImageList = imageList
            };

            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);

            // Set same.
            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageList_TestData))]
        public void ImageList_SetWithStateImageList_GetReturnsExpected(ImageList value)
        {
            using var treeView = new TreeView
            {
                StateImageList = value,
                ImageList = value
            };
            Assert.Same(value, treeView.ImageList);

            // Set same.
            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageList_TestData))]
        public void ImageList_SetWithHandle_GetReturnsExpected(ImageList value)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);

            // Set same.
            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageList_TestData))]
        public void ImageList_SetWithHandleWithCheckBoxes_GetReturnsExpected(ImageList value)
        {
            using var treeView = new TreeView
            {
                CheckBoxes = true
            };
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);

            // Set same.
            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageList_TestData))]
        public void ImageList_SetWithNonNullOldValueWithHandle_GetReturnsExpected(ImageList value)
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                ImageList = imageList
            };
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);

            // Set same.
            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageList_TestData))]
        public void ImageList_SetWithStateImageListWithHandle_GetReturnsExpected(ImageList value)
        {
            using var treeView = new TreeView
            {
                StateImageList = value
            };
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);

            // Set same.
            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);
        }

        [WinFormsFact]
        public void ImageList_Dispose_DetachesFromTreeView()
        {
            using var imageList1 = new ImageList();
            using var imageList2 = new ImageList();
            using var treeView = new TreeView
            {
                ImageList = imageList1
            };
            Assert.Same(imageList1, treeView.ImageList);

            imageList1.Dispose();
            Assert.Null(treeView.ImageList);

            // Make sure we detached the setter.
            treeView.ImageList = imageList2;
            imageList1.Dispose();
            Assert.Same(imageList2, treeView.ImageList);
        }

        [WinFormsFact]
        public void ImageList_CreateHandle_DetachesFromTreeView()
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                ImageList = imageList
            };
            Assert.Same(imageList, treeView.ImageList);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);
        }

        [WinFormsFact]
        public void ImageList_CreateHandleWithHandle_DetachesFromTreeView()
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                ImageList = imageList
            };
            Assert.Same(imageList, treeView.ImageList);
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);
        }

        [WinFormsFact]
        public void ImageList_RecreateHandle_DetachesFromTreeView()
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                ImageList = imageList
            };
            Assert.Same(imageList, treeView.ImageList);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);

            imageList.ImageSize = new Size(10, 10);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);
        }

        [WinFormsFact]
        public void ImageList_RecreateHandleWithHandle_DetachesFromTreeView()
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                ImageList = imageList
            };
            Assert.Same(imageList, treeView.ImageList);
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);

            imageList.ImageSize = new Size(10, 10);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);
        }

        [WinFormsTheory]
        [InlineData(-1, 19)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(5, 5)]
        [InlineData(6, 6)]
        [InlineData(32000, 32000)]
        public void Indent_Set_GetReturnsExpected(int value, int expected)
        {
            using var treeView = new TreeView
            {
                Indent = value
            };
            Assert.Equal(expected, treeView.Indent);

            // Set same.
            treeView.Indent = value;
            Assert.Equal(expected, treeView.Indent);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(32000)]
        public void Indent_SetWithHandle_GetReturnsExpected(int value)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.Indent = value;
            Assert.True(treeView.Indent > 0);

            // Set same.
            treeView.Indent = value;
            Assert.True(treeView.Indent > 0);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(32001)]
        public void Indent_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
        {
            using var treeView = new TreeView();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => treeView.Indent = value);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(32001)]
        public void Indent_SetInvalidWithCustomValue_ThrowsArgumentOutOfRangeException(int indent)
        {
            using var treeView = new TreeView
            {
                Indent = 1
            };
            Assert.Throws<ArgumentOutOfRangeException>("value", () => treeView.Indent = indent);
        }

        public static IEnumerable<object[]> ItemHeight_Get_TestData()
        {
            var font = new Font(FontFamily.GenericSansSerif, 100);
            yield return new object[] { font, true, TreeViewDrawMode.Normal, font.Height + 3 };
            yield return new object[] { font, true, TreeViewDrawMode.OwnerDrawText, font.Height + 3 };
            yield return new object[] { font, true, TreeViewDrawMode.OwnerDrawAll, font.Height + 3 };
            yield return new object[] { font, false, TreeViewDrawMode.Normal, font.Height + 3 };
            yield return new object[] { font, false, TreeViewDrawMode.OwnerDrawText, font.Height + 3 };
            yield return new object[] { font, false, TreeViewDrawMode.OwnerDrawAll, font.Height + 3 };

            Font smallFont = new Font(FontFamily.GenericSansSerif, 2);
            yield return new object[] { smallFont, true, TreeViewDrawMode.Normal, smallFont.Height + 3 };
            yield return new object[] { smallFont, true, TreeViewDrawMode.OwnerDrawText, smallFont.Height + 3 };
            yield return new object[] { smallFont, true, TreeViewDrawMode.OwnerDrawAll, 16 };
            yield return new object[] { smallFont, false, TreeViewDrawMode.Normal, smallFont.Height + 3 };
            yield return new object[] { smallFont, false, TreeViewDrawMode.OwnerDrawText, smallFont.Height + 3 };
            yield return new object[] { smallFont, false, TreeViewDrawMode.OwnerDrawAll, smallFont.Height + 3 };
        }

        [WinFormsTheory]
        [MemberData(nameof(ItemHeight_Get_TestData))]
        public void ItemHeight_Get_ReturnsExpected(Font font, bool checkBoxes, TreeViewDrawMode drawMode, int expectedHeight)
        {
            using var treeView = new TreeView
            {
                Font = font,
                CheckBoxes = checkBoxes,
                DrawMode = drawMode
            };
            Assert.Equal(expectedHeight, treeView.ItemHeight);
        }
        public static IEnumerable<object[]> ItemHeight_Set_TestData()
        {
            yield return new object[] { -1, Control.DefaultFont.Height + 3 };
            yield return new object[] { 1, 1 };
            yield return new object[] { 2, 2 };
            yield return new object[] { 32766, 32766 };
        }

        [WinFormsTheory]
        [MemberData(nameof(ItemHeight_Set_TestData))]
        public void TreeView_ItemHeight_Set_GetReturnsExpected(int value, int expected)
        {
            using var control = new TreeView
            {
                ItemHeight = value
            };
            Assert.Equal(expected, control.ItemHeight);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ItemHeight = value;
            Assert.Equal(expected, control.ItemHeight);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-1, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 0)]
        [InlineData(32766, 0)]
        public void TreeView_ItemHeight_SetWithHandle_GetReturnsExpected(int value, int expectedCreatedCallCount)
        {
            using var control = new TreeView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ItemHeight = value;
            Assert.True(control.ItemHeight > 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.ItemHeight = value;
            Assert.True(control.ItemHeight > 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(32767)]
        public void TreeView_ItemHeight_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
        {
            using var control = new TreeView();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.ItemHeight = value);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(32767)]
        public void TreeView_ItemHeight_SetInvalidWithCustomValue_ThrowsArgumentOutOfRangeException(int indent)
        {
            using var control = new TreeView
            {
                ItemHeight = 1
            };
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.ItemHeight = indent);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void LabelEdit_Set_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView
            {
                LabelEdit = value
            };
            Assert.Equal(value, treeView.LabelEdit);

            // Set same.
            treeView.LabelEdit = value;
            Assert.Equal(value, treeView.LabelEdit);

            // Set different.
            treeView.LabelEdit = !value;
            Assert.Equal(!value, treeView.LabelEdit);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void LabelEdit_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.LabelEdit = value;
            Assert.Equal(value, treeView.LabelEdit);

            // Set same.
            treeView.LabelEdit = value;
            Assert.Equal(value, treeView.LabelEdit);

            // Set different.
            treeView.LabelEdit = !value;
            Assert.Equal(!value, treeView.LabelEdit);
        }

        [WinFormsFact]
        public void LabelEdit_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            using var treeView = new TreeView
            {
                LabelEdit = false
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;

            // Set different.
            treeView.LabelEdit = true;
            Assert.True(treeView.LabelEdit);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.LabelEdit = true;
            Assert.True(treeView.LabelEdit);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.LabelEdit = false;
            Assert.False(treeView.LabelEdit);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.LabelEdit = true;
            Assert.True(treeView.LabelEdit);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsFact]
        public void LabelEdit_SetWithUpdateStylesHandlerWithHandle_CallsStyleChangedCallsInvalidated()
        {
            using var treeView = new TreeView
            {
                LabelEdit = false
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            // Set different.
            treeView.LabelEdit = true;
            Assert.True(treeView.LabelEdit);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set same.
            treeView.LabelEdit = true;
            Assert.True(treeView.LabelEdit);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set different.
            treeView.LabelEdit = false;
            Assert.False(treeView.LabelEdit);
            Assert.Equal(2, styleChangedCallCount);
            Assert.Equal(2, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.LabelEdit = true;
            Assert.True(treeView.LabelEdit);
            Assert.Equal(2, styleChangedCallCount);
            Assert.Equal(2, invalidatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void LineColor_Set_GetReturnsExpected(Color value)
        {
            using var treeView = new TreeView
            {
                LineColor = value
            };
            Assert.Equal(value, treeView.LineColor);

            // Set same.
            treeView.LineColor = value;
            Assert.Equal(value, treeView.LineColor);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void PathSeparator_Set_GetReturnsExpected(string value)
        {
            using var treeView = new TreeView
            {
                PathSeparator = value
            };
            Assert.Same(value, treeView.PathSeparator);

            // Set same.
            treeView.PathSeparator = value;
            Assert.Same(value, treeView.PathSeparator);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingNormalizedTheoryData))]
        public void TreeView_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
        {
            using var control = new TreeView
            {
                Padding = value
            };
            Assert.Equal(expected, control.Padding);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingNormalizedTheoryData))]
        public void TreeView_Padding_SetWithHandle_GetReturnsExpected(Padding value, Padding expected)
        {
            using var control = new TreeView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void TreeView_Padding_SetWithHandler_CallsPaddingChanged()
        {
            using var control = new TreeView();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Equal(EventArgs.Empty, e);
                callCount++;
            };
            control.PaddingChanged += handler;

            // Set different.
            var padding1 = new Padding(1);
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(1, callCount);

            // Set same.
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(1, callCount);

            // Set different.
            var padding2 = new Padding(2);
            control.Padding = padding2;
            Assert.Equal(padding2, control.Padding);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.PaddingChanged -= handler;
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [InlineData(RightToLeft.Yes, true, 1)]
        [InlineData(RightToLeft.Yes, false, 0)]
        [InlineData(RightToLeft.No, true, 1)]
        [InlineData(RightToLeft.No, false, 0)]
        [InlineData(RightToLeft.Inherit, true, 1)]
        [InlineData(RightToLeft.Inherit, false, 0)]
        public void TrackBar_RightToLeftLayout_Set_GetReturnsExpected(RightToLeft rightToLeft, bool value, int expectedLayoutCallCount)
        {
            using var control = new TreeView
            {
                RightToLeft = rightToLeft
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("RightToLeftLayout", e.AffectedProperty);
                layoutCallCount++;
            };

            control.RightToLeftLayout = value;
            Assert.Equal(value, control.RightToLeftLayout);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.RightToLeftLayout = value;
            Assert.Equal(value, control.RightToLeftLayout);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.RightToLeftLayout = !value;
            Assert.Equal(!value, control.RightToLeftLayout);
            Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(RightToLeft.Yes, true, 1, 1, 2)]
        [InlineData(RightToLeft.Yes, false, 0, 0, 1)]
        [InlineData(RightToLeft.No, true, 1, 0, 0)]
        [InlineData(RightToLeft.No, false, 0, 0, 0)]
        [InlineData(RightToLeft.Inherit, true, 1, 0, 0)]
        [InlineData(RightToLeft.Inherit, false, 0, 0, 0)]
        public void TrackBar_RightToLeftLayout_SetWithHandle_GetReturnsExpected(RightToLeft rightToLeft, bool value, int expectedLayoutCallCount, int expectedCreatedCallCount1, int expectedCreatedCallCount2)
        {
            using var control = new TrackBar
            {
                RightToLeft = rightToLeft
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("RightToLeftLayout", e.AffectedProperty);
                layoutCallCount++;
            };

            control.RightToLeftLayout = value;
            Assert.Equal(value, control.RightToLeftLayout);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount1, createdCallCount);

            // Set same.
            control.RightToLeftLayout = value;
            Assert.Equal(value, control.RightToLeftLayout);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount1, createdCallCount);

            // Set different.
            control.RightToLeftLayout = !value;
            Assert.Equal(!value, control.RightToLeftLayout);
            Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount2, createdCallCount);
        }

        [WinFormsFact]
        public void TrackBar_RightToLeftLayout_SetWithHandler_CallsRightToLeftLayoutChanged()
        {
            using var control = new TrackBar
            {
                RightToLeftLayout = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.RightToLeftLayoutChanged += handler;

            // Set different.
            control.RightToLeftLayout = false;
            Assert.False(control.RightToLeftLayout);
            Assert.Equal(1, callCount);

            // Set same.
            control.RightToLeftLayout = false;
            Assert.False(control.RightToLeftLayout);
            Assert.Equal(1, callCount);

            // Set different.
            control.RightToLeftLayout = true;
            Assert.True(control.RightToLeftLayout);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.RightToLeftLayoutChanged -= handler;
            control.RightToLeftLayout = false;
            Assert.False(control.RightToLeftLayout);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void TrackBar_RightToLeftLayout_SetWithHandlerInDisposing_DoesNotRightToLeftLayoutChanged()
        {
            using var control = new TrackBar
            {
                RightToLeft = RightToLeft.Yes
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int callCount = 0;
            control.RightToLeftLayoutChanged += (sender, e) => callCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            int disposedCallCount = 0;
            control.Disposed += (sender, e) =>
            {
                control.RightToLeftLayout = true;
                Assert.True(control.RightToLeftLayout);
                Assert.Equal(0, callCount);
                Assert.Equal(0, createdCallCount);
                disposedCallCount++;
            };

            control.Dispose();
            Assert.Equal(1, disposedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Scrollable_Set_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView
            {
                Scrollable = value
            };
            Assert.Equal(value, treeView.Scrollable);

            // Set same.
            treeView.Scrollable = value;
            Assert.Equal(value, treeView.Scrollable);

            // Set different.
            treeView.Scrollable = !value;
            Assert.Equal(!value, treeView.Scrollable);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Scrollable_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.Scrollable = value;
            Assert.Equal(value, treeView.Scrollable);

            // Set same.
            treeView.Scrollable = value;
            Assert.Equal(value, treeView.Scrollable);

            // Set different.
            treeView.Scrollable = !value;
            Assert.Equal(!value, treeView.Scrollable);
        }

        [WinFormsFact]
        public void Scrollable_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            using var treeView = new TreeView
            {
                Scrollable = true
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;

            // Set different.
            treeView.Scrollable = false;
            Assert.False(treeView.Scrollable);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.Scrollable = false;
            Assert.False(treeView.Scrollable);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.Scrollable = true;
            Assert.True(treeView.Scrollable);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.Scrollable = false;
            Assert.False(treeView.Scrollable);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsFact]
        public void Scrollable_SetWithUpdateStylesHandlerWithHandle_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            using var treeView = new TreeView
            {
                Scrollable = true
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            // Set different.
            treeView.Scrollable = false;
            Assert.False(treeView.Scrollable);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.Scrollable = false;
            Assert.False(treeView.Scrollable);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.Scrollable = true;
            Assert.True(treeView.Scrollable);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.Scrollable = false;
            Assert.False(treeView.Scrollable);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void SelectedImageIndex_SetWithoutImageList_GetReturnsExpected(int value)
        {
            using var treeView = new TreeView
            {
                SelectedImageIndex = value
            };
            Assert.Equal(-1, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);

            // Set same.
            treeView.SelectedImageIndex = value;
            Assert.Equal(-1, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void SelectedImageIndex_SetWithoutImageListWithImageKey_GetReturnsExpected(int value)
        {
            using var treeView = new TreeView
            {
                ImageKey = "imageKey",
                SelectedImageIndex = value
            };
            Assert.Equal(-1, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);

            // Set same.
            treeView.SelectedImageIndex = value;
            Assert.Equal(-1, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void SelectedImageIndex_SetWithEmptyImageList_GetReturnsExpected(int value)
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                ImageList = imageList,
                SelectedImageIndex = value
            };
            Assert.Equal(0, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);

            // Set same.
            treeView.SelectedImageIndex = value;
            Assert.Equal(0, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void SelectedImageIndex_SetWithEmptyImageListWithImageKey_GetReturnsExpected(int value)
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                ImageKey = "imageKey",
                ImageList = imageList,
                SelectedImageIndex = value
            };
            Assert.Equal(0, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);

            // Set same.
            treeView.SelectedImageIndex = value;
            Assert.Equal(0, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);
        }

        [WinFormsTheory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        public void SelectedImageIndex_SetWithImageList_GetReturnsExpected(int value, int expected)
        {
            using var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add(new Bitmap(10, 10));
            using var treeView = new TreeView
            {
                ImageList = imageList,
                SelectedImageIndex = value
            };
            Assert.Equal(expected, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);

            // Set same.
            treeView.SelectedImageIndex = value;
            Assert.Equal(expected, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);
        }

        [WinFormsTheory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        public void SelectedImageIndex_SetWithImageListWithImageKey_GetReturnsExpected(int value, int expected)
        {
            using var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add("imageKey", new Bitmap(10, 10));
            using var treeView = new TreeView
            {
                ImageKey = "imageKey",
                ImageList = imageList,
                SelectedImageIndex = value
            };
            Assert.Equal(expected, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);

            // Set same.
            treeView.SelectedImageIndex = value;
            Assert.Equal(expected, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void SelectedImageIndex_SetWithoutImageListWithHandle_GetReturnsExpected(int value)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.SelectedImageIndex = value;
            Assert.Equal(-1, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);

            // Set same.
            treeView.SelectedImageIndex = value;
            Assert.Equal(-1, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void SelectedImageIndex_SetWithEmptyImageListWithHandle_GetReturnsExpected(int value)
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                ImageList = imageList
            };
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.SelectedImageIndex = value;
            Assert.Equal(0, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);

            // Set same.
            treeView.SelectedImageIndex = value;
            Assert.Equal(0, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);
        }

        [WinFormsTheory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        public void SelectedImageIndex_SetWithImageListWithHandle_GetReturnsExpected(int value, int expected)
        {
            using var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add(new Bitmap(10, 10));
            using var treeView = new TreeView
            {
                ImageList = imageList
            };
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);

            treeView.SelectedImageIndex = value;
            Assert.Equal(expected, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);

            // Set same.
            treeView.SelectedImageIndex = value;
            Assert.Equal(expected, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);
        }

        [WinFormsFact]
        public void SelectedImageIndex_SetInvalid_Throws()
        {
            using var treeView = new TreeView();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => treeView.SelectedImageIndex = -2);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("(none)", "")]
        public void SelectedImageKey_SetWithoutImageList_GetReturnsExpected(string value, string expected)
        {
            using var treeView = new TreeView
            {
                SelectedImageKey = value
            };
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(-1, treeView.SelectedImageIndex);

            // Set same.
            treeView.SelectedImageKey = value;
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(-1, treeView.SelectedImageIndex);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("(none)", "")]
        public void SelectedImageKey_SetWithoutImageListWithImageIndex_GetReturnsExpected(string value, string expected)
        {
            using var treeView = new TreeView
            {
                SelectedImageIndex = 1,
                SelectedImageKey = value
            };
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(-1, treeView.SelectedImageIndex);

            // Set same.
            treeView.SelectedImageKey = value;
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(-1, treeView.SelectedImageIndex);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void SelectedImageKey_SetWithEmptyImageList_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                ImageList = imageList,
                SelectedImageKey = value
            };
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(expectedImageIndex, treeView.SelectedImageIndex);

            // Set same.
            treeView.SelectedImageKey = value;
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(expectedImageIndex, treeView.SelectedImageIndex);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void SelectedImageKey_SetWithEmptyImageListWithImageIndex_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                SelectedImageIndex = 1,
                ImageList = imageList,
                SelectedImageKey = value
            };
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(expectedImageIndex, treeView.SelectedImageIndex);

            // Set same.
            treeView.SelectedImageKey = value;
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(expectedImageIndex, treeView.SelectedImageIndex);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void SelectedImageKey_SetWithNonEmptyImageList_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            using var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add("imageKey", new Bitmap(10, 10));
            using var treeView = new TreeView
            {
                ImageList = imageList,
                SelectedImageKey = value
            };
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(expectedImageIndex, treeView.SelectedImageIndex);

            // Set same.
            treeView.SelectedImageKey = value;
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(expectedImageIndex, treeView.SelectedImageIndex);
        }

        [WinFormsTheory]
        [InlineData(null, "", 0)]
        [InlineData("", "", 1)]
        [InlineData("reasonable", "reasonable", -1)]
        [InlineData("(none)", "", 0)]
        [InlineData("imageKey", "imageKey", -1)]
        [InlineData("ImageKey", "ImageKey", -1)]
        public void SelectedImageKey_SetWithNonEmptyImageListWithImageIndex_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            using var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add("imageKey", new Bitmap(10, 10));
            using var treeView = new TreeView
            {
                SelectedImageIndex = 1,
                ImageList = imageList,
                SelectedImageKey = value
            };
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(expectedImageIndex, treeView.SelectedImageIndex);

            // Set same.
            treeView.SelectedImageKey = value;
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(expectedImageIndex, treeView.SelectedImageIndex);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("(none)", "")]
        public void SelectedImageKey_SetWithoutImageListWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.SelectedImageKey = value;
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(-1, treeView.SelectedImageIndex);

            // Set same.
            treeView.SelectedImageKey = value;
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(-1, treeView.SelectedImageIndex);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void SelectedImageKey_SetWithEmptyImageListWithHandle_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                ImageList = imageList
            };
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.SelectedImageKey = value;
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(expectedImageIndex, treeView.SelectedImageIndex);

            // Set same.
            treeView.SelectedImageKey = value;
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(expectedImageIndex, treeView.SelectedImageIndex);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void SelectedImageKey_SetWithNonEmptyImageListWithHandle_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            using var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add("imageKey", new Bitmap(10, 10));
            using var treeView = new TreeView
            {
                ImageList = imageList
            };
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.SelectedImageKey = value;
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(expectedImageIndex, treeView.SelectedImageIndex);

            // Set same.
            treeView.SelectedImageKey = value;
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(expectedImageIndex, treeView.SelectedImageIndex);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ShowLines_Set_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView
            {
                ShowLines = value
            };
            Assert.Equal(value, treeView.ShowLines);

            // Set same.
            treeView.ShowLines = value;
            Assert.Equal(value, treeView.ShowLines);

            // Set different.
            treeView.ShowLines = !value;
            Assert.Equal(!value, treeView.ShowLines);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ShowLines_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.ShowLines = value;
            Assert.Equal(value, treeView.ShowLines);

            // Set same.
            treeView.ShowLines = value;
            Assert.Equal(value, treeView.ShowLines);

            // Set different.
            treeView.ShowLines = !value;
            Assert.Equal(!value, treeView.ShowLines);
        }

        [WinFormsFact]
        public void ShowLines_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            using var treeView = new TreeView
            {
                ShowLines = false
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;

            // Set different.
            treeView.ShowLines = true;
            Assert.True(treeView.ShowLines);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.ShowLines = true;
            Assert.True(treeView.ShowLines);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.ShowLines = false;
            Assert.False(treeView.ShowLines);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.ShowLines = true;
            Assert.True(treeView.ShowLines);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsFact]
        public void ShowLines_SetWithUpdateStylesHandlerWithHandle_CallsStyleChangedCallsInvalidated()
        {
            using var treeView = new TreeView
            {
                ShowLines = true
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            // Set different.
            treeView.ShowLines = false;
            Assert.False(treeView.ShowLines);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set same.
            treeView.ShowLines = false;
            Assert.False(treeView.ShowLines);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set different.
            treeView.ShowLines = true;
            Assert.True(treeView.ShowLines);
            Assert.Equal(2, styleChangedCallCount);
            Assert.Equal(2, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.ShowLines = false;
            Assert.False(treeView.ShowLines);
            Assert.Equal(2, styleChangedCallCount);
            Assert.Equal(2, invalidatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ShowNodeToolTips_Set_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView
            {
                ShowNodeToolTips = value
            };
            Assert.Equal(value, treeView.ShowNodeToolTips);

            // Set same.
            treeView.ShowNodeToolTips = value;
            Assert.Equal(value, treeView.ShowNodeToolTips);

            // Set different.
            treeView.ShowNodeToolTips = !value;
            Assert.Equal(!value, treeView.ShowNodeToolTips);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ShowNodeToolTips_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.ShowNodeToolTips = value;
            Assert.Equal(value, treeView.ShowNodeToolTips);

            // Set same.
            treeView.ShowNodeToolTips = value;
            Assert.Equal(value, treeView.ShowNodeToolTips);

            // Set different.
            treeView.ShowNodeToolTips = !value;
            Assert.Equal(!value, treeView.ShowNodeToolTips);
        }

        [WinFormsFact]
        public void ShowNodeToolTips_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            using var treeView = new TreeView
            {
                ShowNodeToolTips = false
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;

            // Set different.
            treeView.ShowNodeToolTips = true;
            Assert.True(treeView.ShowNodeToolTips);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.ShowNodeToolTips = true;
            Assert.True(treeView.ShowNodeToolTips);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.ShowNodeToolTips = false;
            Assert.False(treeView.ShowNodeToolTips);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.ShowNodeToolTips = true;
            Assert.True(treeView.ShowNodeToolTips);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsFact]
        public void ShowNodeToolTips_SetWithUpdateStylesHandlerWithHandle_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            using var treeView = new TreeView
            {
                ShowNodeToolTips = true
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            // Set different.
            treeView.ShowNodeToolTips = true;
            Assert.True(treeView.ShowNodeToolTips);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.ShowNodeToolTips = true;
            Assert.True(treeView.ShowNodeToolTips);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.ShowNodeToolTips = false;
            Assert.False(treeView.ShowNodeToolTips);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.ShowNodeToolTips = true;
            Assert.True(treeView.ShowNodeToolTips);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ShowPlusMinus_Set_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView
            {
                ShowPlusMinus = value
            };
            Assert.Equal(value, treeView.ShowPlusMinus);

            // Set same.
            treeView.ShowPlusMinus = value;
            Assert.Equal(value, treeView.ShowPlusMinus);

            // Set different.
            treeView.ShowPlusMinus = !value;
            Assert.Equal(!value, treeView.ShowPlusMinus);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ShowPlusMinus_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.ShowPlusMinus = value;
            Assert.Equal(value, treeView.ShowPlusMinus);

            // Set same.
            treeView.ShowPlusMinus = value;
            Assert.Equal(value, treeView.ShowPlusMinus);

            // Set different.
            treeView.ShowPlusMinus = !value;
            Assert.Equal(!value, treeView.ShowPlusMinus);
        }

        [WinFormsFact]
        public void ShowPlusMinus_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            using var treeView = new TreeView
            {
                ShowPlusMinus = true
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;

            // Set different.
            treeView.ShowPlusMinus = false;
            Assert.False(treeView.ShowPlusMinus);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.ShowPlusMinus = false;
            Assert.False(treeView.ShowPlusMinus);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.ShowPlusMinus = true;
            Assert.True(treeView.ShowPlusMinus);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.ShowPlusMinus = false;
            Assert.False(treeView.ShowPlusMinus);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsFact]
        public void ShowPlusMinus_SetWithUpdateStylesHandlerWithHandle_CallsStyleChangedCallsInvalidated()
        {
            using var treeView = new TreeView
            {
                ShowPlusMinus = true
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            // Set different.
            treeView.ShowPlusMinus = false;
            Assert.False(treeView.ShowPlusMinus);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set same.
            treeView.ShowPlusMinus = false;
            Assert.False(treeView.ShowPlusMinus);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set different.
            treeView.ShowPlusMinus = true;
            Assert.True(treeView.ShowPlusMinus);
            Assert.Equal(2, styleChangedCallCount);
            Assert.Equal(2, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.ShowPlusMinus = false;
            Assert.False(treeView.ShowPlusMinus);
            Assert.Equal(2, styleChangedCallCount);
            Assert.Equal(2, invalidatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ShowRootLines_Set_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView
            {
                ShowRootLines = value
            };
            Assert.Equal(value, treeView.ShowRootLines);

            // Set same.
            treeView.ShowRootLines = value;
            Assert.Equal(value, treeView.ShowRootLines);

            // Set different.
            treeView.ShowRootLines = !value;
            Assert.Equal(!value, treeView.ShowRootLines);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ShowRootLines_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.ShowRootLines = value;
            Assert.Equal(value, treeView.ShowRootLines);

            // Set same.
            treeView.ShowRootLines = value;
            Assert.Equal(value, treeView.ShowRootLines);

            // Set different.
            treeView.ShowRootLines = !value;
            Assert.Equal(!value, treeView.ShowRootLines);
        }

        [WinFormsFact]
        public void ShowRootLines_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            using var treeView = new TreeView
            {
                ShowRootLines = true
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;

            // Set different.
            treeView.ShowRootLines = false;
            Assert.False(treeView.ShowRootLines);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.ShowRootLines = false;
            Assert.False(treeView.ShowRootLines);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.ShowRootLines = true;
            Assert.True(treeView.ShowRootLines);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.ShowRootLines = false;
            Assert.False(treeView.ShowRootLines);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsFact]
        public void ShowRootLines_SetWithUpdateStylesHandlerWithHandle_CallsStyleChangedCallsInvalidated()
        {
            using var treeView = new TreeView
            {
                ShowRootLines = true
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            // Set different.
            treeView.ShowRootLines = false;
            Assert.False(treeView.ShowRootLines);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set same.
            treeView.ShowRootLines = false;
            Assert.False(treeView.ShowRootLines);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(1, invalidatedCallCount);

            // Set different.
            treeView.ShowRootLines = true;
            Assert.True(treeView.ShowRootLines);
            Assert.Equal(2, styleChangedCallCount);
            Assert.Equal(2, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.ShowRootLines = false;
            Assert.False(treeView.ShowRootLines);
            Assert.Equal(2, styleChangedCallCount);
            Assert.Equal(2, invalidatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Sorted_Set_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView
            {
                Sorted = value
            };
            Assert.Equal(value, treeView.Sorted);

            // Set same.
            treeView.Sorted = value;
            Assert.Equal(value, treeView.Sorted);

            // Set different.
            treeView.Sorted = !value;
            Assert.Equal(!value, treeView.Sorted);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Sorted_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.Sorted = value;
            Assert.Equal(value, treeView.Sorted);

            // Set same.
            treeView.Sorted = value;
            Assert.Equal(value, treeView.Sorted);

            // Set different.
            treeView.Sorted = !value;
            Assert.Equal(!value, treeView.Sorted);
        }

        [WinFormsFact]
        public void Sorted_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            using var treeView = new TreeView
            {
                Sorted = false
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;

            // Set different.
            treeView.Sorted = true;
            Assert.True(treeView.Sorted);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.Sorted = true;
            Assert.True(treeView.Sorted);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.Sorted = false;
            Assert.False(treeView.Sorted);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.Sorted = true;
            Assert.True(treeView.Sorted);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsFact]
        public void Sorted_SetWithUpdateStylesHandlerWithHandle_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            using var treeView = new TreeView
            {
                Sorted = false
            };
            int styleChangedCallCount = 0;
            int invalidatedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                styleChangedCallCount++;
            };
            InvalidateEventHandler invalidatedHandler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.NotNull(e);
                invalidatedCallCount++;
            };
            treeView.StyleChanged += styleChangedHandler;
            treeView.Invalidated += invalidatedHandler;
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            // Set different.
            treeView.Sorted = true;
            Assert.True(treeView.Sorted);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.Sorted = true;
            Assert.True(treeView.Sorted);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.Sorted = false;
            Assert.False(treeView.Sorted);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.Sorted = true;
            Assert.True(treeView.Sorted);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageList_TestData))]
        public void StateImageList_Set_GetReturnsExpected(ImageList value)
        {
            using var treeView = new TreeView
            {
                StateImageList = value
            };
            Assert.Same(value, treeView.StateImageList);

            // Set same.
            treeView.StateImageList = value;
            Assert.Same(value, treeView.StateImageList);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageList_TestData))]
        public void StateImageList_SetWithNonNullOldValue_GetReturnsExpected(ImageList value)
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                StateImageList = imageList
            };

            treeView.StateImageList = value;
            Assert.Same(value, treeView.StateImageList);

            // Set same.
            treeView.StateImageList = value;
            Assert.Same(value, treeView.StateImageList);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageList_TestData))]
        public void StateImageList_SetWithHandle_GetReturnsExpected(ImageList value)
        {
            using var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.StateImageList = value;
            Assert.Same(value, treeView.StateImageList);

            // Set same.
            treeView.StateImageList = value;
            Assert.Same(value, treeView.StateImageList);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageList_TestData))]
        public void StateImageList_SetWithNonNullOldValueWithHandle_GetReturnsExpected(ImageList value)
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                StateImageList = imageList
            };
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.StateImageList = value;
            Assert.Same(value, treeView.StateImageList);

            // Set same.
            treeView.StateImageList = value;
            Assert.Same(value, treeView.StateImageList);
        }

        [WinFormsFact]
        public void StateImageList_Dispose_DetachesFromTreeView()
        {
            using var imageList1 = new ImageList();
            using var imageList2 = new ImageList();
            using var treeView = new TreeView
            {
                StateImageList = imageList1
            };
            Assert.Same(imageList1, treeView.StateImageList);

            imageList1.Dispose();
            Assert.Null(treeView.StateImageList);

            // Make sure we detached the setter.
            treeView.StateImageList = imageList2;
            imageList1.Dispose();
            Assert.Same(imageList2, treeView.StateImageList);
        }

        [WinFormsFact]
        public void StateImageList_CreateHandle_DetachesFromTreeView()
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                StateImageList = imageList
            };
            Assert.Same(imageList, treeView.StateImageList);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);
        }

        [WinFormsFact]
        public void StateImageList_CreateHandleWithHandle_DetachesFromTreeView()
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                StateImageList = imageList
            };
            Assert.Same(imageList, treeView.StateImageList);
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);
        }

        [WinFormsFact]
        public void StateImageList_RecreateHandle_DetachesFromTreeView()
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                StateImageList = imageList
            };
            Assert.Same(imageList, treeView.StateImageList);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);

            imageList.ImageSize = new Size(10, 10);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);
        }

        [WinFormsFact]
        public void StateImageList_RecreateHandleWithHandle_DetachesFromTreeView()
        {
            using var imageList = new ImageList();
            using var treeView = new TreeView
            {
                StateImageList = imageList
            };
            Assert.Same(imageList, treeView.StateImageList);
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);

            imageList.ImageSize = new Size(10, 10);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void Text_Set_GetReturnsExpected(string value, string expected)
        {
            using var control = new TreeView
            {
                Text = value
            };
            Assert.Equal(expected, control.Text);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void TreeView_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var control = new TreeView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Text_SetWithHandler_CallsTextChanged()
        {
            using var control = new TreeView();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.TextChanged += handler;

            // Set different.
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(1, callCount);

            // Set same.
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(1, callCount);

            // Set different.
            control.Text = "otherText";
            Assert.Equal("otherText", control.Text);
            Assert.Equal(2, callCount);

            // Set null.
            control.Text = null;
            Assert.Empty(control.Text);
            Assert.Equal(3, callCount);

            // Set empty.
            control.Text = string.Empty;
            Assert.Empty(control.Text);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.TextChanged -= handler;
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(3, callCount);
        }

        public static IEnumerable<object[]> TreeViewNodeSorter_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { StringComparer.CurrentCulture };
        }

        [WinFormsTheory]
        [MemberData(nameof(TreeViewNodeSorter_TestData))]
        public void TreeViewNodeSorter_Set_GetReturnsExpected(IComparer value)
        {
            using var treeView = new TreeView
            {
                TreeViewNodeSorter = value
            };
            Assert.Same(value, treeView.TreeViewNodeSorter);

            // Set same.
            treeView.TreeViewNodeSorter = value;
            Assert.Same(value, treeView.TreeViewNodeSorter);
        }

        [WinFormsFact]
        public void AddExistingNodeAsChild_ThrowsArgumentException()
        {
            using var treeView = new TreeView();
            var node = new TreeNode();
            treeView.Nodes.Add(node);

            Assert.Throws<ArgumentException>(() => node.Nodes.Add(node));
        }

        [WinFormsFact]
        public void TreeView_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubTreeView();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        public static IEnumerable<object[]> GetNodeAt_Empty_TestData()
        {
            yield return new object[] { new Point(int.MinValue, int.MinValue) };
            yield return new object[] { new Point(-1, -2) };
            yield return new object[] { new Point(-1, 0) };
            yield return new object[] { new Point(-1, 2) };
            yield return new object[] { new Point(0, -2) };
            yield return new object[] { new Point(0, 0) };
            yield return new object[] { new Point(0, 2) };
            yield return new object[] { new Point(1, -2) };
            yield return new object[] { new Point(1, 0) };
            yield return new object[] { new Point(1, 2) };
            yield return new object[] { new Point(int.MaxValue, int.MaxValue) };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetNodeAt_Empty_TestData))]
        public void TreeView_GetNodeAt_InvokePointEmpty_ReturnsNull(Point pt)
        {
            using var control = new TreeView();
            Assert.Null(control.GetNodeAt(pt));
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.Null(control.GetNodeAt(pt));
            Assert.True(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> GetNodeAt_NotEmptyValid_TestData()
        {
            yield return new object[] { new Point(0, 0) };
            yield return new object[] { new Point(20, 0) };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetNodeAt_NotEmptyValid_TestData))]
        public void TreeView_GetNodeAt_InvokePointNotEmptyValid_Success(Point pt)
        {
            using var control = new TreeView();
            var node1 = new TreeNode("Some Long Text");
            control.Nodes.Add(node1);
            Assert.Same(node1, control.GetNodeAt(pt));
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.Same(node1, control.GetNodeAt(pt));
            Assert.True(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> GetNodeAt_NotEmptyInvalid_TestData()
        {
            yield return new object[] { new Point(int.MinValue, int.MinValue) };
            yield return new object[] { new Point(-1, -2) };
            yield return new object[] { new Point(-1, 0) };
            yield return new object[] { new Point(-1, 2) };
            yield return new object[] { new Point(0, -2) };
            yield return new object[] { new Point(1, -2) };
            yield return new object[] { new Point(int.MaxValue, int.MaxValue) };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetNodeAt_NotEmptyInvalid_TestData))]
        public void TreeView_GetNodeAt_InvokePointNotEmptyInvalid_Success(Point pt)
        {
            using var control = new TreeView();
            var node1 = new TreeNode("Some Long Text");
            control.Nodes.Add(node1);
            Assert.Null(control.GetNodeAt(pt));
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.Null(control.GetNodeAt(pt));
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetNodeAt_Empty_TestData))]
        public void TreeView_GetNodeAt_InvokePointEmptyWithHandle_Success(Point pt)
        {
            using var control = new TreeView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Null(control.GetNodeAt(pt));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Null(control.GetNodeAt(pt));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetNodeAt_NotEmptyValid_TestData))]
        public void TreeView_GetNodeAt_InvokePointNotEmptyValidWithHandle_Success(Point pt)
        {
            using var control = new TreeView();
            var node1 = new TreeNode("Some Long Text");
            control.Nodes.Add(node1);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Same(node1, control.GetNodeAt(pt));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Same(node1, control.GetNodeAt(pt));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetNodeAt_NotEmptyInvalid_TestData))]
        public void TreeView_GetNodeAt_InvokePointNotEmptyInvalidWithHandle_Success(Point pt)
        {
            using var control = new TreeView();
            var node1 = new TreeNode("Some Long Text");
            control.Nodes.Add(node1);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Null(control.GetNodeAt(pt));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Null(control.GetNodeAt(pt));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetNodeAt_Empty_TestData))]
        public void TreeView_GetNodeAt_InvokeIntIntEmpty_ReturnsNull(Point pt)
        {
            using var control = new TreeView();
            Assert.Null(control.GetNodeAt(pt.X, pt.Y));
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.Null(control.GetNodeAt(pt.X, pt.Y));
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetNodeAt_NotEmptyValid_TestData))]
        public void TreeView_GetNodeAt_InvokeIntIntNotEmptyValid_Success(Point pt)
        {
            using var control = new TreeView();
            var node1 = new TreeNode("Some Long Text");
            control.Nodes.Add(node1);
            Assert.Same(node1, control.GetNodeAt(pt.X, pt.Y));
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.Same(node1, control.GetNodeAt(pt.X, pt.Y));
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetNodeAt_NotEmptyInvalid_TestData))]
        public void TreeView_GetNodeAt_InvokeIntIntNotEmptyInvalid_Success(Point pt)
        {
            using var control = new TreeView();
            var node1 = new TreeNode("Some Long Text");
            control.Nodes.Add(node1);
            Assert.Null(control.GetNodeAt(pt.X, pt.Y));
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.Null(control.GetNodeAt(pt.X, pt.Y));
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetNodeAt_Empty_TestData))]
        public void TreeView_GetNodeAt_InvokeIntIntEmptyWithHandle_Success(Point pt)
        {
            using var control = new TreeView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Null(control.GetNodeAt(pt.X, pt.Y));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Null(control.GetNodeAt(pt.X, pt.Y));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetNodeAt_NotEmptyValid_TestData))]
        public void TreeView_GetNodeAt_InvokeIntIntNotEmptyValidWithHandle_Success(Point pt)
        {
            using var control = new TreeView();
            var node1 = new TreeNode("Some Long Text");
            control.Nodes.Add(node1);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Same(node1, control.GetNodeAt(pt.X, pt.Y));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Same(node1, control.GetNodeAt(pt.X, pt.Y));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetNodeAt_NotEmptyInvalid_TestData))]
        public void TreeView_GetNodeAt_InvokeIntIntNotEmptyInvalidWithHandle_Success(Point pt)
        {
            using var control = new TreeView();
            var node1 = new TreeNode("Some Long Text");
            control.Nodes.Add(node1);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Null(control.GetNodeAt(pt.X, pt.Y));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Null(control.GetNodeAt(pt.X, pt.Y));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, false)]
        [InlineData(ControlStyles.UserPaint, false)]
        [InlineData(ControlStyles.Opaque, false)]
        [InlineData(ControlStyles.ResizeRedraw, false)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, false)]
        [InlineData(ControlStyles.StandardClick, false)]
        [InlineData(ControlStyles.Selectable, true)]
        [InlineData(ControlStyles.UserMouse, false)]
        [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
        [InlineData(ControlStyles.StandardDoubleClick, true)]
        [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
        [InlineData(ControlStyles.CacheText, false)]
        [InlineData(ControlStyles.EnableNotifyMessage, false)]
        [InlineData(ControlStyles.DoubleBuffer, false)]
        [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
        [InlineData(ControlStyles.UseTextForAccessibility, false)]
        [InlineData((ControlStyles)0, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void TreeView_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubTreeView();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void TreeView_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubTreeView();
            Assert.False(control.GetTopLevel());
        }

        public static IEnumerable<object[]> HitTest_Empty_TestData()
        {
            yield return new object[] { new Point(int.MinValue, int.MinValue), TreeViewHitTestLocations.AboveClientArea | TreeViewHitTestLocations.LeftOfClientArea };
            yield return new object[] { new Point(-1, -2), TreeViewHitTestLocations.AboveClientArea | TreeViewHitTestLocations.LeftOfClientArea };
            yield return new object[] { new Point(-1, 0), TreeViewHitTestLocations.LeftOfClientArea };
            yield return new object[] { new Point(-1, 2), TreeViewHitTestLocations.LeftOfClientArea };
            yield return new object[] { new Point(0, -2), TreeViewHitTestLocations.AboveClientArea };
            yield return new object[] { new Point(0, 0), TreeViewHitTestLocations.None };
            yield return new object[] { new Point(0, 2), TreeViewHitTestLocations.None };
            yield return new object[] { new Point(1, -2), TreeViewHitTestLocations.AboveClientArea };
            yield return new object[] { new Point(1, 0), TreeViewHitTestLocations.None };
            yield return new object[] { new Point(1, 2), TreeViewHitTestLocations.None };
            yield return new object[] { new Point(int.MaxValue, int.MaxValue), TreeViewHitTestLocations.BelowClientArea | TreeViewHitTestLocations.RightOfClientArea };
        }

        [WinFormsTheory]
        [MemberData(nameof(HitTest_Empty_TestData))]
        public void TreeView_HitTest_InvokePointEmpty_Success(Point pt, TreeViewHitTestLocations expectedLocations)
        {
            using var control = new TreeView();
            TreeViewHitTestInfo result = control.HitTest(pt);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Null(result.Node);
            Assert.True(control.IsHandleCreated);

            // Call again.
            result = control.HitTest(pt);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Null(result.Node);
            Assert.True(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> HitTest_NotEmptyValid_TestData()
        {
            yield return new object[] { new Point(0, 0), TreeViewHitTestLocations.Indent };
        }

        [WinFormsTheory]
        [MemberData(nameof(HitTest_NotEmptyValid_TestData))]
        public void TreeView_HitTest_InvokePointNotEmptyValid_Success(Point pt, TreeViewHitTestLocations expectedLocations)
        {
            using var control = new TreeView();
            var node1 = new TreeNode("Some Long Text");
            control.Nodes.Add(node1);
            TreeViewHitTestInfo result = control.HitTest(pt);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Same(node1, result.Node);
            Assert.True(control.IsHandleCreated);

            // Call again.
            result = control.HitTest(pt);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Same(node1, result.Node);
            Assert.True(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> HitTest_NotEmptyInvalid_TestData()
        {
            yield return new object[] { new Point(int.MinValue, int.MinValue), TreeViewHitTestLocations.AboveClientArea | TreeViewHitTestLocations.LeftOfClientArea };
            yield return new object[] { new Point(-1, -2), TreeViewHitTestLocations.AboveClientArea | TreeViewHitTestLocations.LeftOfClientArea };
            yield return new object[] { new Point(-1, 0), TreeViewHitTestLocations.LeftOfClientArea };
            yield return new object[] { new Point(-1, 2), TreeViewHitTestLocations.LeftOfClientArea };
            yield return new object[] { new Point(0, -2), TreeViewHitTestLocations.AboveClientArea };
            yield return new object[] { new Point(1, -2), TreeViewHitTestLocations.AboveClientArea };
            yield return new object[] { new Point(int.MaxValue, int.MaxValue), TreeViewHitTestLocations.BelowClientArea | TreeViewHitTestLocations.RightOfClientArea };
        }

        [WinFormsTheory]
        [MemberData(nameof(HitTest_NotEmptyInvalid_TestData))]
        public void TreeView_HitTest_InvokePointNotEmptyInvalid_Success(Point pt, TreeViewHitTestLocations expectedLocations)
        {
            using var control = new TreeView();
            var node1 = new TreeNode("Some Long Text");
            control.Nodes.Add(node1);
            TreeViewHitTestInfo result = control.HitTest(pt);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Null(result.Node);
            Assert.True(control.IsHandleCreated);

            // Call again.
            result = control.HitTest(pt);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Null(result.Node);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(HitTest_Empty_TestData))]
        public void TreeView_HitTest_InvokePointEmptyWithHandle_Success(Point pt, TreeViewHitTestLocations expectedLocations)
        {
            using var control = new TreeView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            TreeViewHitTestInfo result = control.HitTest(pt);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Null(result.Node);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            result = control.HitTest(pt);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Null(result.Node);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(HitTest_NotEmptyValid_TestData))]
        public void TreeView_HitTest_InvokePointNotEmptyValidWithHandle_Success(Point pt, TreeViewHitTestLocations expectedLocations)
        {
            using var control = new TreeView();
            var node1 = new TreeNode("Some Long Text");
            control.Nodes.Add(node1);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            TreeViewHitTestInfo result = control.HitTest(pt);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Same(node1, result.Node);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            result = control.HitTest(pt);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Same(node1, result.Node);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(HitTest_NotEmptyInvalid_TestData))]
        public void TreeView_HitTest_InvokePointNotEmptyInvalidWithHandle_Success(Point pt, TreeViewHitTestLocations expectedLocations)
        {
            using var control = new TreeView();
            var node1 = new TreeNode("Some Long Text");
            control.Nodes.Add(node1);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            TreeViewHitTestInfo result = control.HitTest(pt);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Null(result.Node);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            result = control.HitTest(pt);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Null(result.Node);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(HitTest_Empty_TestData))]
        public void TreeView_HitTest_InvokeIntIntEmpty_Success(Point pt, TreeViewHitTestLocations expectedLocations)
        {
            using var control = new TreeView();
            TreeViewHitTestInfo result = control.HitTest(pt.X, pt.Y);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Null(result.Node);
            Assert.True(control.IsHandleCreated);

            // Call again.
            result = control.HitTest(pt.X, pt.Y);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Null(result.Node);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(HitTest_NotEmptyValid_TestData))]
        public void TreeView_HitTest_InvokeIntIntNotEmptyValid_Success(Point pt, TreeViewHitTestLocations expectedLocations)
        {
            using var control = new TreeView();
            var node1 = new TreeNode("Some Long Text");
            control.Nodes.Add(node1);
            TreeViewHitTestInfo result = control.HitTest(pt.X, pt.Y);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Same(node1, result.Node);
            Assert.True(control.IsHandleCreated);

            // Call again.
            result = control.HitTest(pt.X, pt.Y);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Same(node1, result.Node);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(HitTest_NotEmptyInvalid_TestData))]
        public void TreeView_HitTest_InvokeIntIntNotEmptyInvalid_Success(Point pt, TreeViewHitTestLocations expectedLocations)
        {
            using var control = new TreeView();
            var node1 = new TreeNode("Some Long Text");
            control.Nodes.Add(node1);
            TreeViewHitTestInfo result = control.HitTest(pt.X, pt.Y);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Null(result.Node);
            Assert.True(control.IsHandleCreated);

            // Call again.
            result = control.HitTest(pt.X, pt.Y);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Null(result.Node);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(HitTest_Empty_TestData))]
        public void TreeView_HitTest_InvokeIntIntEmptyWithHandle_Success(Point pt, TreeViewHitTestLocations expectedLocations)
        {
            using var control = new TreeView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            TreeViewHitTestInfo result = control.HitTest(pt.X, pt.Y);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Null(result.Node);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            result = control.HitTest(pt.X, pt.Y);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Null(result.Node);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(HitTest_NotEmptyValid_TestData))]
        public void TreeView_HitTest_InvokeIntIntNotEmptyValidWithHandle_Success(Point pt, TreeViewHitTestLocations expectedLocations)
        {
            using var control = new TreeView();
            var node1 = new TreeNode("Some Long Text");
            control.Nodes.Add(node1);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            TreeViewHitTestInfo result = control.HitTest(pt.X, pt.Y);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Same(node1, result.Node);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            result = control.HitTest(pt.X, pt.Y);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Same(node1, result.Node);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(HitTest_NotEmptyInvalid_TestData))]
        public void TreeView_HitTest_InvokeIntIntNotEmptyInvalidWithHandle_Success(Point pt, TreeViewHitTestLocations expectedLocations)
        {
            using var control = new TreeView();
            var node1 = new TreeNode("Some Long Text");
            control.Nodes.Add(node1);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            TreeViewHitTestInfo result = control.HitTest(pt.X, pt.Y);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Null(result.Node);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            result = control.HitTest(pt.X, pt.Y);
            Assert.Equal(expectedLocations, result.Location);
            Assert.Null(result.Node);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> TreeViewEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new TreeViewEventArgs(null) };
            yield return new object[] { new TreeViewEventArgs(new TreeNode()) };
        }

        [WinFormsTheory]
        [MemberData(nameof(TreeViewEventArgs_TestData))]
        public void TreeView_OnAfterCheck_Invoke_CallsAfterCheck(TreeViewEventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            TreeViewEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.AfterCheck += handler;
            control.OnAfterCheck(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.AfterCheck -= handler;
            control.OnAfterCheck(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(TreeViewEventArgs_TestData))]
        public void TreeView_OnAfterCollapse_Invoke_CallsAfterCollapse(TreeViewEventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            TreeViewEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.AfterCollapse += handler;
            control.OnAfterCollapse(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.AfterCollapse -= handler;
            control.OnAfterCollapse(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(TreeViewEventArgs_TestData))]
        public void TreeView_OnAfterExpand_Invoke_CallsAfterExpand(TreeViewEventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            TreeViewEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.AfterExpand += handler;
            control.OnAfterExpand(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.AfterExpand -= handler;
            control.OnAfterExpand(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> NodeLabelEditEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new NodeLabelEditEventArgs(null) };
            yield return new object[] { new NodeLabelEditEventArgs(new TreeNode()) };
            yield return new object[] { new NodeLabelEditEventArgs(new TreeNode(), "label") };
        }

        [WinFormsTheory]
        [MemberData(nameof(NodeLabelEditEventArgs_TestData))]
        public void TreeView_OnAfterLabelEdit_Invoke_CallsAfterLabelEdit(NodeLabelEditEventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            NodeLabelEditEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.AfterLabelEdit += handler;
            control.OnAfterLabelEdit(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.AfterLabelEdit -= handler;
            control.OnAfterLabelEdit(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(TreeViewEventArgs_TestData))]
        public void TreeView_OnAfterSelect_Invoke_CallsOnAfterSelect(TreeViewEventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            TreeViewEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.AfterSelect += handler;
            control.OnAfterSelect(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.AfterSelect -= handler;
            control.OnAfterSelect(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> TreeViewCancelEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new TreeViewCancelEventArgs(null, false, TreeViewAction.ByKeyboard) };
            yield return new object[] { new TreeViewCancelEventArgs(new TreeNode(), true, TreeViewAction.ByKeyboard) };
        }

        [WinFormsTheory]
        [MemberData(nameof(TreeViewCancelEventArgs_TestData))]
        public void TreeView_OnBeforeExpand_Invoke_CallsBeforeExpand(TreeViewCancelEventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            TreeViewCancelEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.BeforeExpand += handler;
            control.OnBeforeExpand(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.BeforeExpand -= handler;
            control.OnBeforeExpand(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(NodeLabelEditEventArgs_TestData))]
        public void TreeView_OnBeforeLabelEdit_Invoke_CallsBeforeLabelEdit(NodeLabelEditEventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            NodeLabelEditEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.BeforeLabelEdit += handler;
            control.OnBeforeLabelEdit(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.BeforeLabelEdit -= handler;
            control.OnBeforeLabelEdit(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(TreeViewCancelEventArgs_TestData))]
        public void TreeView_OnBeforeCollapse_Invoke_CallsBeforeCollapse(TreeViewCancelEventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            TreeViewCancelEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.BeforeCollapse += handler;
            control.OnBeforeCollapse(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.BeforeCollapse -= handler;
            control.OnBeforeCollapse(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(TreeViewCancelEventArgs_TestData))]
        public void TreeView_OnBeforeSelect_Invoke_CallsBeforeSelect(TreeViewCancelEventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            TreeViewCancelEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.BeforeSelect += handler;
            control.OnBeforeSelect(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.BeforeSelect -= handler;
            control.OnBeforeSelect(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> OnDrawNode_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new DrawTreeNodeEventArgs(null, null, Rectangle.Empty, TreeNodeStates.Checked) };

            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);
            yield return new object[] { new DrawTreeNodeEventArgs(graphics, new TreeNode(), new Rectangle(1, 2, 3, 4), TreeNodeStates.Checked) };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnDrawNode_TestData))]
        public void TreeView_OnDrawNode_Invoke_CallsDrawNode(DrawTreeNodeEventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            DrawTreeNodeEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.DrawNode += handler;
            control.OnDrawNode(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.DrawNode -= handler;
            control.OnDrawNode(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> OnItemDrag_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ItemDragEventArgs(MouseButtons.None) };
            yield return new object[] { new ItemDragEventArgs(MouseButtons.Right, new object()) };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnItemDrag_TestData))]
        public void TreeView_OnItemDrag_Invoke_CallsItemDrag(ItemDragEventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            ItemDragEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.ItemDrag += handler;
            control.OnItemDrag(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.ItemDrag -= handler;
            control.OnItemDrag(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> OnKeyDown_TestData()
        {
            yield return new object[] { new KeyEventArgs(Keys.None) };
            yield return new object[] { new KeyEventArgs(Keys.A) };
            yield return new object[] { new KeyEventArgs(Keys.Space) };
            yield return new object[] { new KeyEventArgs(Keys.Control | Keys.Space) };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnKeyDown_TestData))]
        public void TreeView_OnKeyDown_Invoke_CallsKeyDown(KeyEventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            KeyEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.KeyDown += handler;
            control.OnKeyDown(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.KeyDown -= handler;
            control.OnKeyDown(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> OnKeyDown_WithSelectedNode_TestData()
        {
            yield return new object[] { false, true, false, new KeyEventArgs(Keys.None), 0, 0, false };
            yield return new object[] { false, true, false, new KeyEventArgs(Keys.A), 0, 0, false };
            yield return new object[] { false, true, false, new KeyEventArgs(Keys.Space), 1, 1, true };
            yield return new object[] { false, true, true, new KeyEventArgs(Keys.Space), 1, 0, true };
            yield return new object[] { false, true, false, new KeyEventArgs(Keys.Control | Keys.Space), 1, 1, true };
            yield return new object[] { false, true, true, new KeyEventArgs(Keys.Control | Keys.Space), 1, 0, true };

            yield return new object[] { true, true, false, new KeyEventArgs(Keys.Space), 0, 0, true };
            yield return new object[] { true, false, false, new KeyEventArgs(Keys.Space), 0, 0, true };
            yield return new object[] { false, false, false, new KeyEventArgs(Keys.Space), 0, 0, false };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnKeyDown_WithSelectedNode_TestData))]
        public void TreeView_OnKeyDown_InvokeWithSelectedNode_CallsKeyDown(bool handled, bool checkBoxes, bool cancel, KeyEventArgs eventArgs, int expectedBeforeCheckCallCount, int expectedAfterCheckCallCount, bool expectedHandled)
        {
            using var control = new SubTreeView
            {
                CheckBoxes = checkBoxes
            };
            var node = new TreeNode();
            control.Nodes.Add(node);
            control.SelectedNode = node;

            int callCount = 0;
            KeyEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
                e.Handled = handled;
            };
            int beforeCheckCallCount = 0;
            TreeViewCancelEventHandler beforeCheckHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(TreeViewAction.ByKeyboard, e.Action);
                Assert.False(e.Cancel);
                beforeCheckCallCount++;
                e.Cancel = cancel;
            };
            int afterCheckCallCount = 0;
            TreeViewEventHandler afterCheckHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(TreeViewAction.ByKeyboard, e.Action);
                afterCheckCallCount++;
            };

            // Call with handler.
            control.KeyDown += handler;
            control.BeforeCheck += beforeCheckHandler;
            control.AfterCheck += afterCheckHandler;
            control.OnKeyDown(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedAfterCheckCallCount == 1, node.Checked);
            Assert.Equal(expectedHandled, eventArgs.Handled);
            Assert.Equal(expectedBeforeCheckCallCount, beforeCheckCallCount);
            Assert.Equal(expectedAfterCheckCallCount, afterCheckCallCount);

            // Call again.
            control.OnKeyDown(eventArgs);
            Assert.Equal(2, callCount);
            Assert.False(node.Checked);
            Assert.Equal(expectedHandled, eventArgs.Handled);
            Assert.Equal(expectedBeforeCheckCallCount * 2, beforeCheckCallCount);
            Assert.Equal(expectedAfterCheckCallCount * 2, afterCheckCallCount);

            // Remove handler.
            control.KeyDown -= handler;
            control.BeforeCheck -= beforeCheckHandler;
            control.AfterCheck -= afterCheckHandler;
            control.OnKeyDown(eventArgs);
            Assert.Equal(2, callCount);
            Assert.Equal(expectedHandled, eventArgs.Handled);
            Assert.Equal(expectedBeforeCheckCallCount * 2, beforeCheckCallCount);
            Assert.Equal(expectedAfterCheckCallCount * 2, afterCheckCallCount);
        }

        [WinFormsFact]
        public void TreeView_OnKeyDown_NullEventArgs_ThrowsNullReferenceException()
        {
            using var control = new SubTreeView();
            Assert.Throws<NullReferenceException>(() => control.OnKeyDown(null));
        }

        public static IEnumerable<object[]> OnKeyPress_TestData()
        {
            yield return new object[] { true, new KeyPressEventArgs('\0'), true };
            yield return new object[] { true, new KeyPressEventArgs('a'), true };
            yield return new object[] { true, new KeyPressEventArgs(' '), true };

            yield return new object[] { false, new KeyPressEventArgs('\0'), false };
            yield return new object[] { false, new KeyPressEventArgs('a'), false };
            yield return new object[] { false, new KeyPressEventArgs(' '), true };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnKeyPress_TestData))]
        public void TreeView_OnKeyPress_Invoke_CallsKeyPress(bool handled, KeyPressEventArgs eventArgs, bool expectedHandled)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            KeyPressEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                e.Handled = handled;
                callCount++;
            };

            // Call with handler.
            control.KeyPress += handler;
            control.OnKeyPress(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedHandled, eventArgs.Handled);

            // Remove handler.
            control.KeyPress -= handler;
            control.OnKeyPress(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedHandled, eventArgs.Handled);
        }

        [WinFormsFact]
        public void TreeView_OnKeyPress_NullEventArgs_ThrowsNullReferenceException()
        {
            using var control = new SubTreeView();
            Assert.Throws<NullReferenceException>(() => control.OnKeyPress(null));
        }

        public static IEnumerable<object[]> OnKeyUp_TestData()
        {
            yield return new object[] { true, new KeyEventArgs(Keys.None), true };
            yield return new object[] { true, new KeyEventArgs(Keys.A), true };
            yield return new object[] { true, new KeyEventArgs(Keys.Space), true };
            yield return new object[] { true, new KeyEventArgs(Keys.Control | Keys.Space), true };

            yield return new object[] { false, new KeyEventArgs(Keys.None), false };
            yield return new object[] { false, new KeyEventArgs(Keys.A), false };
            yield return new object[] { false, new KeyEventArgs(Keys.Space), true };
            yield return new object[] { false, new KeyEventArgs(Keys.Control | Keys.Space), true };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnKeyUp_TestData))]
        public void TreeView_OnKeyUp_Invoke_CallsKeyUp(bool handled, KeyEventArgs eventArgs, bool expectedHandled)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            KeyEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                e.Handled = handled;
                callCount++;
            };

            // Call with handler.
            control.KeyUp += handler;
            control.OnKeyUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedHandled, eventArgs.Handled);

            // Remove handler.
            control.KeyUp -= handler;
            control.OnKeyUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedHandled, eventArgs.Handled);
        }

        [WinFormsFact]
        public void TreeView_OnKeyUp_NullEventArgs_ThrowsNullReferenceException()
        {
            using var control = new SubTreeView();
            Assert.Throws<NullReferenceException>(() => control.OnKeyUp(null));
        }

        /*
        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TreeView_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                //Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleCreated += handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);
        }
        */

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TreeView_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubTreeView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.False(control.GetStyle(ControlStyles.UserPaint));

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleCreated += handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnHandleCreated_WithHandleWithProperties_TestData()
        {
            yield return new object[] { Array.Empty<Image>(), null, false };
            yield return new object[] { new Image[] { new Bitmap(10, 10) }, null, true };
            yield return new object[] { Array.Empty<Image>(), new EventArgs(), false };
            yield return new object[] { new Image[] { new Bitmap(10, 10) }, new EventArgs(), true };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnHandleCreated_WithHandleWithProperties_TestData))]
        public void TreeView_OnHandleCreated_InvokeWithHandleWithProperties_CallsHandleCreated(Image[] images, EventArgs eventArgs, bool expectedStateImageListHandleCreated)
        {
            using var imageList = new ImageList();
            using var stateImageList = new ImageList();
            stateImageList.Images.AddRange(images);
            using var control = new SubTreeView
            {
                CheckBoxes = true,
                ShowNodeToolTips = true,
                BackColor = Color.Red,
                ForeColor = Color.Blue,
                LineColor = Color.Yellow,
                ImageList = imageList,
                StateImageList = stateImageList,
                Indent = 10,
                ItemHeight = 11
            };
            var node = new TreeNode();
            control.Nodes.Add(node);
            control.SelectedNode = node;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.False(control.GetStyle(ControlStyles.UserPaint));

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleCreated += handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.CheckBoxes);
            Assert.True(control.ShowNodeToolTips);
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(Color.Blue, control.ForeColor);
            Assert.Equal(Color.Yellow, control.LineColor);
            Assert.Same(imageList, control.ImageList);
            Assert.True(control.ImageList.HandleCreated);
            Assert.Same(stateImageList, control.StateImageList);
            Assert.Equal(expectedStateImageListHandleCreated, control.StateImageList.HandleCreated);
            Assert.Equal(10, control.Indent);
            Assert.Equal(11, control.ItemHeight);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.CheckBoxes);
            Assert.True(control.ShowNodeToolTips);
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(Color.Blue, control.ForeColor);
            Assert.Equal(Color.Yellow, control.LineColor);
            Assert.Same(imageList, control.ImageList);
            Assert.True(control.ImageList.HandleCreated);
            Assert.Same(stateImageList, control.StateImageList);
            Assert.Equal(expectedStateImageListHandleCreated, control.StateImageList.HandleCreated);
            Assert.Equal(10, control.Indent);
            Assert.Equal(11, control.ItemHeight);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnHandleCreated_WithHandleWithProperties_TestData))]
        public void TreeView_OnHandleCreated_InvokeWithHandleWithPropertiesDesignMode_CallsHandleCreated(Image[] images, EventArgs eventArgs, bool expectedStateImageListHandleCreated)
        {
            using var imageList = new ImageList();
            using var stateImageList = new ImageList();
            stateImageList.Images.AddRange(images);
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            using var control = new SubTreeView
            {
                CheckBoxes = true,
                ShowNodeToolTips = true,
                BackColor = Color.Red,
                ForeColor = Color.Blue,
                LineColor = Color.Yellow,
                ImageList = imageList,
                StateImageList = stateImageList,
                Indent = 10,
                ItemHeight = 11,
                Site = mockSite.Object
            };
            var node = new TreeNode();
            control.Nodes.Add(node);
            control.SelectedNode = node;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.False(control.GetStyle(ControlStyles.UserPaint));

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleCreated += handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.CheckBoxes);
            Assert.True(control.ShowNodeToolTips);
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(Color.Blue, control.ForeColor);
            Assert.Equal(Color.Yellow, control.LineColor);
            Assert.Same(imageList, control.ImageList);
            Assert.True(control.ImageList.HandleCreated);
            Assert.Same(stateImageList, control.StateImageList);
            Assert.Equal(expectedStateImageListHandleCreated, control.StateImageList.HandleCreated);
            Assert.Equal(10, control.Indent);
            Assert.Equal(11, control.ItemHeight);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.CheckBoxes);
            Assert.True(control.ShowNodeToolTips);
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(Color.Blue, control.ForeColor);
            Assert.Equal(Color.Yellow, control.LineColor);
            Assert.Same(imageList, control.ImageList);
            Assert.True(control.ImageList.HandleCreated);
            Assert.Same(stateImageList, control.StateImageList);
            Assert.Equal(expectedStateImageListHandleCreated, control.StateImageList.HandleCreated);
            Assert.Equal(10, control.Indent);
            Assert.Equal(11, control.ItemHeight);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TreeView_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleDestroyed += handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TreeView_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubTreeView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleDestroyed += handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TreeView_OnHandleDestroyed_InvokeWithHandleWithSelectedNode_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubTreeView();
            var node = new TreeNode();
            control.Nodes.Add(node);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.SelectedNode = node;

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleDestroyed += handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Same(node, control.SelectedNode);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Same(node, control.SelectedNode);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TreeView_OnHandleDestroyed_InvokeWithHandleWithStateImageList_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var imageList = new ImageList();
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);

            using var control = new SubTreeView
            {
                StateImageList = imageList
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                //Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleDestroyed += handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Same(imageList, control.StateImageList);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Same(imageList, control.StateImageList);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TreeView_OnMouseHover_Invoke_CallsMouseHover(EventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int nodeCallCount = 0;
            control.NodeMouseHover += (sender, e) => nodeCallCount++;

            // Call with handler.
            control.MouseHover += handler;
            control.OnMouseHover(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, nodeCallCount);
            Assert.True(control.IsHandleCreated);

            // Call again.
            control.OnMouseHover(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, nodeCallCount);
            Assert.True(control.IsHandleCreated);

            // Call leave.
            control.OnMouseLeave(null);
            control.OnMouseHover(eventArgs);
            Assert.Equal(2, callCount);
            Assert.Equal(0, nodeCallCount);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.MouseHover -= handler;
            control.OnMouseHover(eventArgs);
            Assert.Equal(2, callCount);
            Assert.Equal(0, nodeCallCount);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TreeView_OnMouseHover_InvokeWithHandle_CallsMouseHover(EventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int nodeCallCount = 0;
            control.NodeMouseHover += (sender, e) => nodeCallCount++;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // Call with handler.
            control.MouseHover += handler;
            control.OnMouseHover(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, nodeCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.OnMouseHover(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, nodeCallCount);
            Assert.True(control.IsHandleCreated);

            // Call leave.
            control.OnMouseLeave(null);
            control.OnMouseHover(eventArgs);
            Assert.Equal(2, callCount);
            Assert.Equal(0, nodeCallCount);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.MouseHover -= handler;
            control.OnMouseHover(eventArgs);
            Assert.Equal(2, callCount);
            Assert.Equal(0, nodeCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TreeView_OnMouseLeave_Invoke_CallsMouseLeave(EventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseLeave += handler;
            control.OnMouseLeave(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.MouseLeave -= handler;
            control.OnMouseLeave(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> TreeNodeMouseClickEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new TreeNodeMouseClickEventArgs(null, MouseButtons.None, 0, 0, 0) };
            yield return new object[] { new TreeNodeMouseClickEventArgs(new TreeNode(), MouseButtons.Right, 1, 2, 3) };
        }

        [WinFormsTheory]
        [MemberData(nameof(TreeNodeMouseClickEventArgs_TestData))]
        public void TreeView_OnNodeMouseClick_Invoke_CallsNodeMouseClick(TreeNodeMouseClickEventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            TreeNodeMouseClickEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.NodeMouseClick += handler;
            control.OnNodeMouseClick(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.NodeMouseClick -= handler;
            control.OnNodeMouseClick(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(TreeNodeMouseClickEventArgs_TestData))]
        public void TreeView_OnNodeMouseDoubleClick_Invoke_CallsNodeMouseDoubleClick(TreeNodeMouseClickEventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            TreeNodeMouseClickEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.NodeMouseDoubleClick += handler;
            control.OnNodeMouseDoubleClick(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.NodeMouseDoubleClick -= handler;
            control.OnNodeMouseDoubleClick(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> OnNodeMouseHover_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new TreeNodeMouseHoverEventArgs(null) };
            yield return new object[] { new TreeNodeMouseHoverEventArgs(new TreeNode()) };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnNodeMouseHover_TestData))]
        public void TreeView_OnNodeMouseHover_Invoke_CallsNodeMouseHover(TreeNodeMouseHoverEventArgs eventArgs)
        {
            using var control = new SubTreeView();
            int callCount = 0;
            TreeNodeMouseHoverEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.NodeMouseHover += handler;
            control.OnNodeMouseHover(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.NodeMouseHover -= handler;
            control.OnNodeMouseHover(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> OnRightToLeftLayoutChanged_TestData()
        {
            yield return new object[] { RightToLeft.Yes, null };
            yield return new object[] { RightToLeft.Yes, new EventArgs() };
            yield return new object[] { RightToLeft.No, null };
            yield return new object[] { RightToLeft.No, new EventArgs() };
            yield return new object[] { RightToLeft.Inherit, null };
            yield return new object[] { RightToLeft.Inherit, new EventArgs() };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnRightToLeftLayoutChanged_TestData))]
        public void TreeView_OnRightToLeftLayoutChanged_Invoke_CallsRightToLeftLayoutChanged(RightToLeft rightToLeft, EventArgs eventArgs)
        {
            using var control = new SubTreeView
            {
                RightToLeft = rightToLeft
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.RightToLeftLayoutChanged += handler;
            control.OnRightToLeftLayoutChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.RightToLeftLayoutChanged -= handler;
            control.OnRightToLeftLayoutChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnRightToLeftLayoutChanged_WithHandle_TestData()
        {
            yield return new object[] { RightToLeft.Yes, null, 1 };
            yield return new object[] { RightToLeft.Yes, new EventArgs(), 1 };
            yield return new object[] { RightToLeft.No, null, 0 };
            yield return new object[] { RightToLeft.No, new EventArgs(), 0 };
            yield return new object[] { RightToLeft.Inherit, null, 0 };
            yield return new object[] { RightToLeft.Inherit, new EventArgs(), 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnRightToLeftLayoutChanged_WithHandle_TestData))]
        public void TreeView_OnRightToLeftLayoutChanged_InvokeWithHandle_CallsRightToLeftLayoutChanged(RightToLeft rightToLeft, EventArgs eventArgs, int expectedCreatedCallCount)
        {
            using var control = new SubTreeView
            {
                RightToLeft = rightToLeft
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.RightToLeftLayoutChanged += handler;
            control.OnRightToLeftLayoutChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Remove handler.
            control.RightToLeftLayoutChanged -= handler;
            control.OnRightToLeftLayoutChanged(eventArgs);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount * 2, createdCallCount);
        }

        [WinFormsFact]
        public void TreeView_OnRightToLeftLayoutChanged_InvokeInDisposing_DoesNotCallRightToLeftLayoutChanged()
        {
            using var control = new SubTreeView
            {
                RightToLeft = RightToLeft.Yes
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int callCount = 0;
            control.RightToLeftLayoutChanged += (sender, e) => callCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            int disposedCallCount = 0;
            control.Disposed += (sender, e) =>
            {
                control.OnRightToLeftLayoutChanged(EventArgs.Empty);
                Assert.Equal(0, callCount);
                Assert.Equal(0, createdCallCount);
                disposedCallCount++;
            };

            control.Dispose();
            Assert.Equal(1, disposedCallCount);
        }

        private class SubTreeView : TreeView
        {
            public new bool CanEnableIme => base.CanEnableIme;

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new CreateParams CreateParams => base.CreateParams;

            public new Cursor DefaultCursor => base.DefaultCursor;

            public new ImeMode DefaultImeMode => base.DefaultImeMode;

            public new Padding DefaultMargin => base.DefaultMargin;

            public new Size DefaultMaximumSize => base.DefaultMaximumSize;

            public new Size DefaultMinimumSize => base.DefaultMinimumSize;

            public new Padding DefaultPadding => base.DefaultPadding;

            public new Size DefaultSize => base.DefaultSize;

            public new bool DesignMode => base.DesignMode;

            public new bool DoubleBuffered
            {
                get => base.DoubleBuffered;
                set => base.DoubleBuffered = value;
            }

            public new EventHandlerList Events => base.Events;

            public new int FontHeight
            {
                get => base.FontHeight;
                set => base.FontHeight = value;
            }

            public new ImeMode ImeModeBase
            {
                get => base.ImeModeBase;
                set => base.ImeModeBase = value;
            }

            public new bool ResizeRedraw
            {
                get => base.ResizeRedraw;
                set => base.ResizeRedraw = value;
            }

            public new bool ShowFocusCues => base.ShowFocusCues;

            public new bool ShowKeyboardCues => base.ShowKeyboardCues;

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public new void OnAfterCheck(TreeViewEventArgs e) => base.OnAfterCheck(e);

            public new void OnAfterCollapse(TreeViewEventArgs e) => base.OnAfterCollapse(e);

            public new void OnAfterExpand(TreeViewEventArgs e) => base.OnAfterExpand(e);

            public new void OnAfterLabelEdit(NodeLabelEditEventArgs e) => base.OnAfterLabelEdit(e);

            public new void OnAfterSelect(TreeViewEventArgs e) => base.OnAfterSelect(e);

            public new void OnBeforeCheck(TreeViewCancelEventArgs e) => base.OnBeforeCheck(e);

            public new void OnBeforeCollapse(TreeViewCancelEventArgs e) => base.OnBeforeCollapse(e);

            public new void OnBeforeExpand(TreeViewCancelEventArgs e) => base.OnBeforeExpand(e);

            public new void OnBeforeLabelEdit(NodeLabelEditEventArgs e) => base.OnBeforeLabelEdit(e);

            public new void OnBeforeSelect(TreeViewCancelEventArgs e) => base.OnBeforeSelect(e);

            public new void OnDrawNode(DrawTreeNodeEventArgs e) => base.OnDrawNode(e);

            public new void OnItemDrag(ItemDragEventArgs e) => base.OnItemDrag(e);

            public new void OnKeyDown(KeyEventArgs e) => base.OnKeyDown(e);

            public new void OnKeyPress(KeyPressEventArgs e) => base.OnKeyPress(e);

            public new void OnKeyUp(KeyEventArgs e) => base.OnKeyUp(e);

            public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

            public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

            public new void OnMouseHover(EventArgs e) => base.OnMouseHover(e);

            public new void OnMouseLeave(EventArgs e) => base.OnMouseLeave(e);

            public new void OnNodeMouseClick(TreeNodeMouseClickEventArgs e) => base.OnNodeMouseClick(e);

            public new void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e) => base.OnNodeMouseDoubleClick(e);

            public new void OnNodeMouseHover(TreeNodeMouseHoverEventArgs e) => base.OnNodeMouseHover(e);

            public new void OnRightToLeftLayoutChanged(EventArgs e) => base.OnRightToLeftLayoutChanged(e);
        }
    }
}

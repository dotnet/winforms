// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TreeViewTests
    {
        [Fact]
        public void Ctor_Default()
        {
            var treeView = new SubTreeView();
            Assert.Equal(SystemColors.Window, treeView.BackColor);
            Assert.Null(treeView.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, treeView.BackgroundImageLayout);
            Assert.Equal(BorderStyle.Fixed3D, treeView.BorderStyle);
            Assert.False(treeView.CheckBoxes);
            Assert.Equal(new Size(121, 97), treeView.DefaultSize);
            Assert.Equal(0, treeView.DisplayRectangle.X);
            Assert.Equal(0, treeView.DisplayRectangle.Y);
            Assert.True(treeView.DisplayRectangle.Width > 0);
            Assert.True(treeView.DisplayRectangle.Height > 0);
            Assert.False(treeView.DoubleBuffered);
            Assert.Equal(TreeViewDrawMode.Normal, treeView.DrawMode);
            Assert.Equal(SystemColors.WindowText, treeView.ForeColor);
            Assert.False(treeView.FullRowSelect);
            Assert.True(treeView.HideSelection);
            Assert.False(treeView.HotTracking);
            Assert.Equal(-1, treeView.ImageIndex);
            Assert.Equal(string.Empty, treeView.ImageKey);
            Assert.NotNull(treeView.ImageIndexer);
            Assert.Equal(0, treeView.ImageIndexer.Index);
            Assert.Null(treeView.ImageList);
            Assert.Equal(19, treeView.Indent);
            Assert.Equal(Control.DefaultFont.Height + 3, treeView.ItemHeight);
            Assert.False(treeView.LabelEdit);
            Assert.Equal(Color.Empty, treeView.LineColor);
            Assert.Same(treeView.Nodes, treeView.Nodes);
            Assert.Empty(treeView.Nodes);
            Assert.Equal(Padding.Empty, treeView.Padding);
            Assert.Equal("\\", treeView.PathSeparator);
            Assert.False(treeView.RightToLeftLayout);
            Assert.NotNull(treeView.root);
            Assert.True(treeView.Scrollable);
            Assert.Equal(-1, treeView.SelectedImageIndex);
            Assert.Equal(string.Empty, treeView.SelectedImageKey);
            Assert.NotNull(treeView.SelectedImageIndexer);
            Assert.Equal(0, treeView.SelectedImageIndexer.Index);
            Assert.Null(treeView.SelectedNode);
            Assert.Equal(new Size(121, 97), treeView.Size);
            Assert.True(treeView.ShowLines);
            Assert.False(treeView.ShowNodeToolTips);
            Assert.True(treeView.ShowPlusMinus);
            Assert.True(treeView.ShowRootLines);
            Assert.False(treeView.Sorted);
            Assert.Null(treeView.StateImageList);
            Assert.Empty(treeView.Text);
            Assert.Null(treeView.TopNode);
            Assert.Null(treeView.TreeViewNodeSorter);
            Assert.True(treeView.Visible);
            Assert.Equal(0, treeView.VisibleCount);
        }

        public static IEnumerable<object[]> BackColor_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.Window };
            yield return new object[] { Color.Red, Color.Red };
        }

        [Theory]
        [MemberData(nameof(BackColor_TestData))]
        public void BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            var control = new TreeView
            {
                BackColor = value
            };
            Assert.Equal(expected, control.BackColor);

            // Set same.
            control.BackColor  = value;
            Assert.Equal(expected, control.BackColor);
        }

        [Fact]
        public void BackColor_SetWithHandler_CallsBackColorChanged()
        {
            var control = new TreeView();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void BackgroundImage_Set_GetReturnsExpected(Image value)
        {
            var control = new TreeView
            {
                BackgroundImage = value
            };
            Assert.Equal(value, control.BackgroundImage);

            // Set same.
            control.BackgroundImage  = value;
            Assert.Equal(value, control.BackgroundImage);
        }

        [Fact]
        public void BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
        {
            var control = new TreeView();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BackgroundImageChanged += handler;

            // Set different.
            var image1 = new Bitmap(10, 10);
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Equal(1, callCount);

            // Set same.
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Equal(1, callCount);

            // Set different.
            var image2 = new Bitmap(10, 10);
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
        {
            var control = new TreeView
            {
                BackgroundImageLayout = value
            };
            Assert.Equal(value, control.BackgroundImageLayout);

            // Set same.
            control.BackgroundImageLayout = value;
            Assert.Equal(value, control.BackgroundImageLayout);
        }

        [Fact]
        public void BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
        {
            var control = new TreeView();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ImageLayout))]
        public void BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
        {
            var control = new TreeView();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(BorderStyle))]
        public void BorderStyle_Set_GetReturnsExpected(BorderStyle value)
        {
            var treeView = new TreeView
            {
                BorderStyle = value
            };
            Assert.Equal(value, treeView.BorderStyle);

            // Set same.
            treeView.BorderStyle = value;
            Assert.Equal(value, treeView.BorderStyle);
        }

        [Fact]
        public void BorderStyle_SetWithUpdateStylesHandler_CallsStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new TreeView
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

        [Fact]
        public void BorderStyle_SetWithInvalidatedWithHandle_CallsStyleChangedCallsInvalidated()
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(BorderStyle))]
        public void BorderStyle_SetInvalid_ThrowsInvalidEnumArgumentException(BorderStyle value)
        {
            var treeView = new TreeView();
            Assert.Throws<InvalidEnumArgumentException>("value", () => treeView.BorderStyle = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void CheckBoxes_Set_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void CheckBoxes_SetWithHandle_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView();
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

        [Fact]
        public void CheckBoxes_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new TreeView
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

        [Fact]
        public void CheckBoxes_SetWithUpdateStylesHandlerWithHandle_CallsStyleChangedCallsInvalidated()
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DoubleBuffered_Set_GetReturnsExpected(bool value)
        {
            var treeView = new SubTreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DoubleBuffered_SetWithHandle_GetReturnsExpected(bool value)
        {
            var treeView = new SubTreeView();
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

        [Fact]
        public void DoubleBuffered_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new SubTreeView
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

        [Fact]
        public void DoubleBuffered_SetWithUpdateStylesHandlerWithHandle_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new SubTreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TreeViewDrawMode))]
        public void DrawMode_Set_GetReturnsExpected(TreeViewDrawMode value)
        {
            var control = new TreeView
            {
                DrawMode = value
            };
            Assert.Equal(value, control.DrawMode);

            // Set same.
            control.DrawMode = value;
            Assert.Equal(value, control.DrawMode);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TreeViewDrawMode))]
        public void DrawMode_SetWithHandle_GetReturnsExpected(TreeViewDrawMode value)
        {
            var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.DrawMode = value;
            Assert.Equal(value, treeView.DrawMode);

            // Set same.
            treeView.DrawMode = value;
            Assert.Equal(value, treeView.DrawMode);
        }

        [Fact]
        public void DrawMode_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new SubTreeView
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

        [Fact]
        public void DrawMode_SetWithUpdateStylesHandlerWithHandle_DoesNotCallStyleChangedCallsInvalidated()
        {
            var treeView = new SubTreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TreeViewDrawMode))]
        public void DrawMode_SetInvalid_ThrowsInvalidEnumArgumentException(TreeViewDrawMode value)
        {
            var treeView = new TreeView();
            Assert.Throws<InvalidEnumArgumentException>("value", () => treeView.DrawMode = value);
        }

        public static IEnumerable<object[]> ForeColor_Set_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.WindowText };
            yield return new object[] { Color.Red, Color.Red };
        }

        [Theory]
        [MemberData(nameof(ForeColor_Set_TestData))]
        public void ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            var control = new TreeView
            {
                ForeColor = value
            };
            Assert.Equal(expected, control.ForeColor);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
        }

        [Fact]
        public void ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            var control = new TreeView();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FullRowSelect_Set_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FullRowSelect_SetWithHandle_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView();
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

        [Fact]
        public void FullRowSelect_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new TreeView
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

        [Fact]
        public void FullRowSelect_SetWithUpdateStylesHandlerWithHandle_CallsStyleChangedCallsInvalidated()
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void HideSelection_Set_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void HideSelection_SetWithHandle_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView();
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

        [Fact]
        public void HideSelection_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new TreeView
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

        [Fact]
        public void HideSelection_SetWithUpdateStylesHandlerWithHandle_CallsStyleChangedCallsInvalidated()
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void HotTracking_Set_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void HotTracking_SetWithHandle_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView();
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

        [Fact]
        public void HotTracking_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new TreeView
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

        [Fact]
        public void HotTracking_SetWithUpdateStylesHandlerWithHandle_CallsStyleChangedCallsInvalidated()
        {
            var treeView = new TreeView
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

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ImageIndex_SetWithoutImageList_GetReturnsExpected(int value)
        {
            var treeView = new TreeView
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

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ImageIndex_SetWithoutImageListWithImageKey_GetReturnsExpected(int value)
        {
            var treeView = new TreeView
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

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ImageIndex_SetWithEmptyImageList_GetReturnsExpected(int value)
        {
            var imageList = new ImageList();
            var treeView = new TreeView
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

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ImageIndex_SetWithEmptyImageListWithImageKey_GetReturnsExpected(int value)
        {
            var imageList = new ImageList();
            var treeView = new TreeView
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

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        public void ImageIndex_SetWithImageList_GetReturnsExpected(int value, int expected)
        {
            var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add(new Bitmap(10, 10));
            var treeView = new TreeView
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

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        public void ImageIndex_SetWithImageListWithImageKey_GetReturnsExpected(int value, int expected)
        {
            var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add("imageKey", new Bitmap(10, 10));
            var treeView = new TreeView
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

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ImageIndex_SetWithoutImageListWithHandle_GetReturnsExpected(int value)
        {
            var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.ImageIndex = value;
            Assert.Equal(-1, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);

            // Set same.
            treeView.ImageIndex = value;
            Assert.Equal(-1, treeView.ImageIndex);
            Assert.Empty(treeView.ImageKey);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ImageIndex_SetWithEmptyImageListWithHandle_GetReturnsExpected(int value)
        {
            var imageList = new ImageList();
            var treeView = new TreeView
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

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        public void ImageIndex_SetWithImageListWithHandle_GetReturnsExpected(int value, int expected)
        {
            var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add(new Bitmap(10, 10));
            var treeView = new TreeView
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

        [Fact]
        public void ImageIndex_SetInvalid_Throws()
        {
            var treeView = new TreeView();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => treeView.ImageIndex = -2);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("(none)", "")]
        public void ImageKey_SetWithoutImageList_GetReturnsExpected(string value, string expected)
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("(none)", "")]
        public void ImageKey_SetWithoutImageListWithImageIndex_GetReturnsExpected(string value, string expected)
        {
            var treeView = new TreeView
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
            yield return new object[] { "", "",  0 };
            yield return new object[] { "reasonable", "reasonable",  -1 };
            yield return new object[] { "(none)", "",  0 };
            yield return new object[] { "imageKey", "imageKey", -1 };
            yield return new object[] { "ImageKey", "ImageKey", -1 };
        }

        [Theory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void ImageKey_SetWithEmptyImageList_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            var imageList = new ImageList();
            var treeView = new TreeView
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

        [Theory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void ImageKey_SetWithEmptyImageListWithImageIndex_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            var imageList = new ImageList();
            var treeView = new TreeView
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

        [Theory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void ImageKey_SetWithNonEmptyImageList_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add("imageKey", new Bitmap(10, 10));
            var treeView = new TreeView
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

        [Theory]
        [InlineData(null, "", 0)]
        [InlineData("", "",  1)]
        [InlineData("reasonable", "reasonable",  -1)]
        [InlineData("(none)", "",  0)]
        [InlineData("imageKey", "imageKey", -1)]
        [InlineData("ImageKey", "ImageKey", -1)]
        public void ImageKey_SetWithNonEmptyImageListWithImageIndex_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add("imageKey", new Bitmap(10, 10));
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("(none)", "")]
        public void ImageKey_SetWithoutImageListWithHandle_GetReturnsExpected(string value, string expected)
        {
            var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.ImageKey = value;
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(-1, treeView.ImageIndex);

            // Set same.
            treeView.ImageKey = value;
            Assert.Equal(expected, treeView.ImageKey);
            Assert.Equal(-1, treeView.ImageIndex);
        }

        [Theory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void ImageKey_SetWithEmptyImageListWithHandle_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            var imageList = new ImageList();
            var treeView = new TreeView
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

        [Theory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void ImageKey_SetWithNonEmptyImageListWithHandle_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add("imageKey", new Bitmap(10, 10));
            var treeView = new TreeView
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

            var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            yield return new object[] { imageList };
        }

        [Theory]
        [MemberData(nameof(ImageList_TestData))]
        public void ImageList_Set_GetReturnsExpected(ImageList value)
        {
            var treeView = new TreeView
            {
                ImageList = value
            };
            Assert.Same(value, treeView.ImageList);

            // Set same.
            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);
        }

        [Theory]
        [MemberData(nameof(ImageList_TestData))]
        public void ImageList_SetWithCheckboxes_GetReturnsExpected(ImageList value)
        {
            var treeView = new TreeView
            {
                CheckBoxes = true,
                ImageList = value
            };
            Assert.Same(value, treeView.ImageList);

            // Set same.
            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);
        }

        [Theory]
        [MemberData(nameof(ImageList_TestData))]
        public void ImageList_SetWithNonNullOldValue_GetReturnsExpected(ImageList value)
        {
            var treeView = new TreeView
            {
                ImageList = new ImageList()
            };

            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);

            // Set same.
            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);
        }

        [Theory]
        [MemberData(nameof(ImageList_TestData))]
        public void ImageList_SetWithStateImageList_GetReturnsExpected(ImageList value)
        {
            var treeView = new TreeView
            {
                StateImageList = value,
                ImageList = value
            };
            Assert.Same(value, treeView.ImageList);

            // Set same.
            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);
        }

        [Theory]
        [MemberData(nameof(ImageList_TestData))]
        public void ImageList_SetWithHandle_GetReturnsExpected(ImageList value)
        {
            var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);

            // Set same.
            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);
        }

        [Theory]
        [MemberData(nameof(ImageList_TestData))]
        public void ImageList_SetWithHandleWithCheckBoxes_GetReturnsExpected(ImageList value)
        {
            var treeView = new TreeView
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

        [Theory]
        [MemberData(nameof(ImageList_TestData))]
        public void ImageList_SetWithNonNullOldValueWithHandle_GetReturnsExpected(ImageList value)
        {
            var treeView = new TreeView
            {
                ImageList = new ImageList()
            };
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);

            // Set same.
            treeView.ImageList = value;
            Assert.Same(value, treeView.ImageList);
        }

        [Theory]
        [MemberData(nameof(ImageList_TestData))]
        public void ImageList_SetWithStateImageListWithHandle_GetReturnsExpected(ImageList value)
        {
            var treeView = new TreeView
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

        [Fact]
        public void ImageList_Dispose_DetachesFromTreeView()
        {
            var imageList1 = new ImageList();
            var imageList2 = new ImageList();
            var treeView = new TreeView
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

        [Fact]
        public void ImageList_CreateHandle_DetachesFromTreeView()
        {
            var imageList = new ImageList();
            var treeView = new TreeView
            {
                ImageList = imageList
            };
            Assert.Same(imageList, treeView.ImageList);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);
        }

        [Fact]
        public void ImageList_CreateHandleWithHandle_DetachesFromTreeView()
        {
            var imageList = new ImageList();
            var treeView = new TreeView
            {
                ImageList = imageList
            };
            Assert.Same(imageList, treeView.ImageList);
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);
        }

        [Fact]
        public void ImageList_RecreateHandle_DetachesFromTreeView()
        {
            var imageList = new ImageList();
            var treeView = new TreeView
            {
                ImageList = imageList
            };
            Assert.Same(imageList, treeView.ImageList);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);

            imageList.ImageSize = new Size(10, 10);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);
        }

        [Fact]
        public void ImageList_RecreateHandleWithHandle_DetachesFromTreeView()
        {
            var imageList = new ImageList();
            var treeView = new TreeView
            {
                ImageList = imageList
            };
            Assert.Same(imageList, treeView.ImageList);
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);

            imageList.ImageSize = new Size(10, 10);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);
        }

        [Theory]
        [InlineData(-1, 19)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(5, 5)]
        [InlineData(6, 6)]
        [InlineData(32000, 32000)]
        public void Indent_Set_GetReturnsExpected(int value, int expected)
        {
            var treeView = new TreeView
            {
                Indent = value
            };
            Assert.Equal(expected, treeView.Indent);

            // Set same.
            treeView.Indent = value;
            Assert.Equal(expected, treeView.Indent);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(32000)]
        public void Indent_SetWithHandle_GetReturnsExpected(int value)
        {
            var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.Indent = value;
            Assert.True(treeView.Indent > 0);

            // Set same.
            treeView.Indent = value;
            Assert.True(treeView.Indent > 0);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(32001)]
        public void Indent_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
        {
            var treeView = new TreeView();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => treeView.Indent = value);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(32001)]
        public void Indent_SetInvalidWithCustomValue_ThrowsArgumentOutOfRangeException(int indent)
        {
            var treeView = new TreeView
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

        [Theory]
        [MemberData(nameof(ItemHeight_Get_TestData))]
        public void ItemHeight_Get_ReturnsExpected(Font font, bool checkBoxes, TreeViewDrawMode drawMode, int expectedHeight)
        {
            var treeView = new TreeView
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

        [Theory]
        [MemberData(nameof(ItemHeight_Set_TestData))]
        public void ItemHeight_Set_GetReturnsExpected(int value, int expected)
        {
            var treeView = new TreeView
            {
                ItemHeight = value
            };
            Assert.Equal(expected, treeView.ItemHeight);

            // Set same.
            treeView.ItemHeight = value;
            Assert.Equal(expected, treeView.ItemHeight);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(32766)]
        public void ItemHeight_SetWithHandle_GetReturnsExpected(int value)
        {
            var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.ItemHeight = value;
            Assert.True(treeView.ItemHeight > 0);

            // Set same.
            treeView.ItemHeight = value;
            Assert.True(treeView.ItemHeight > 0);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(32767)]
        public void ItemHeight_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
        {
            var treeView = new TreeView();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => treeView.ItemHeight = value);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(32767)]
        public void ItemHeight_SetInvalidWithCustomValue_ThrowsArgumentOutOfRangeException(int indent)
        {
            var treeView = new TreeView
            {
                ItemHeight = 1
            };
            Assert.Throws<ArgumentOutOfRangeException>("value", () => treeView.ItemHeight = indent);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void LabelEdit_Set_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void LabelEdit_SetWithHandle_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView();
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

        [Fact]
        public void LabelEdit_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new TreeView
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

        [Fact]
        public void LabelEdit_SetWithUpdateStylesHandlerWithHandle_CallsStyleChangedCallsInvalidated()
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void LineColor_Set_GetReturnsExpected(Color value)
        {
            var treeView = new TreeView
            {
                LineColor = value
            };
            Assert.Equal(value, treeView.LineColor);

            // Set same.
            treeView.LineColor = value;
            Assert.Equal(value, treeView.LineColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void PathSeparator_Set_GetReturnsExpected(string value)
        {
            var treeView = new TreeView
            {
                PathSeparator = value
            };
            Assert.Same(value, treeView.PathSeparator);

            // Set same.
            treeView.PathSeparator = value;
            Assert.Same(value, treeView.PathSeparator);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingNormalizedTheoryData))]
        public void Padding_Set_GetReturnsExpected(Padding value, Padding expected)
        {
            var control = new TreeView
            {
                Padding = value
            };
            Assert.Equal(expected, control.Padding);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
        }

        [Fact]
        public void Padding_SetWithHandler_CallsPaddingChanged()
        {
            var control = new TreeView();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.PaddingChanged += handler;

            // Set different.
            control.Padding = new Padding(1);
            Assert.Equal(new Padding(1), control.Padding);
            Assert.Equal(1, callCount);

            // Set same.
            control.Padding = new Padding(1);
            Assert.Equal(new Padding(1), control.Padding);
            Assert.Equal(1, callCount);

            // Set different.
            control.Padding = new Padding(2);
            Assert.Equal(new Padding(2), control.Padding);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.PaddingChanged -= handler;
            control.Padding = new Padding(1);
            Assert.Equal(new Padding(1), control.Padding);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void RightToLeftLayout_Set_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView
            {
                RightToLeftLayout = value
            };
            Assert.Equal(value, treeView.RightToLeftLayout);

            // Set same.
            treeView.RightToLeftLayout = value;
            Assert.Equal(value, treeView.RightToLeftLayout);

            // Set different.
            treeView.RightToLeftLayout = !value;
            Assert.Equal(!value, treeView.RightToLeftLayout);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void RightToLeftLayout_SetWithHandle_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.RightToLeftLayout = value;
            Assert.Equal(value, treeView.RightToLeftLayout);

            // Set same.
            treeView.RightToLeftLayout = value;
            Assert.Equal(value, treeView.RightToLeftLayout);

            // Set different.
            treeView.RightToLeftLayout = !value;
            Assert.Equal(!value, treeView.RightToLeftLayout);
        }

        [Fact]
        public void RightToLeftLayout_SetWithHandler_CallsRightToLeftLayoutChanged()
        {
            var treeView = new TreeView
            {
                RightToLeftLayout = false
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(treeView, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            treeView.RightToLeftLayoutChanged += handler;

            // Set different.
            treeView.RightToLeftLayout = true;
            Assert.True(treeView.RightToLeftLayout);
            Assert.Equal(1, callCount);

            // Set same.
            treeView.RightToLeftLayout = true;
            Assert.True(treeView.RightToLeftLayout);
            Assert.Equal(1, callCount);

            // Set different.
            treeView.RightToLeftLayout = false;
            Assert.False(treeView.RightToLeftLayout);
            Assert.Equal(2, callCount);

            // Remove handler.
            treeView.RightToLeftLayoutChanged -= handler;
            treeView.RightToLeftLayout = true;
            Assert.True(treeView.RightToLeftLayout);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void RightToLeftLayout_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new TreeView
            {
                RightToLeftLayout = false
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
            treeView.RightToLeftLayout = true;
            Assert.True(treeView.RightToLeftLayout);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.RightToLeftLayout = true;
            Assert.True(treeView.RightToLeftLayout);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.RightToLeftLayout = false;
            Assert.False(treeView.RightToLeftLayout);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.RightToLeftLayout = true;
            Assert.True(treeView.RightToLeftLayout);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Fact]
        public void RightToLeftLayout_SetWithUpdateStylesHandlerWithHandle_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new TreeView
            {
                RightToLeftLayout = false
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
            treeView.RightToLeftLayout = true;
            Assert.True(treeView.RightToLeftLayout);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            treeView.RightToLeftLayout = true;
            Assert.True(treeView.RightToLeftLayout);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            treeView.RightToLeftLayout = false;
            Assert.False(treeView.RightToLeftLayout);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            treeView.StyleChanged -= styleChangedHandler;
            treeView.Invalidated -= invalidatedHandler;
            treeView.RightToLeftLayout = true;
            Assert.True(treeView.RightToLeftLayout);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Scrollable_Set_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Scrollable_SetWithHandle_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView();
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

        [Fact]
        public void Scrollable_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new TreeView
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

        [Fact]
        public void Scrollable_SetWithUpdateStylesHandlerWithHandle_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new TreeView
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

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void SelectedImageIndex_SetWithoutImageList_GetReturnsExpected(int value)
        {
            var treeView = new TreeView
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

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void SelectedImageIndex_SetWithoutImageListWithImageKey_GetReturnsExpected(int value)
        {
            var treeView = new TreeView
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

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void SelectedImageIndex_SetWithEmptyImageList_GetReturnsExpected(int value)
        {
            var imageList = new ImageList();
            var treeView = new TreeView
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

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void SelectedImageIndex_SetWithEmptyImageListWithImageKey_GetReturnsExpected(int value)
        {
            var imageList = new ImageList();
            var treeView = new TreeView
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

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        public void SelectedImageIndex_SetWithImageList_GetReturnsExpected(int value, int expected)
        {
            var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add(new Bitmap(10, 10));
            var treeView = new TreeView
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

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        public void SelectedImageIndex_SetWithImageListWithImageKey_GetReturnsExpected(int value, int expected)
        {
            var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add("imageKey", new Bitmap(10, 10));
            var treeView = new TreeView
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

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void SelectedImageIndex_SetWithoutImageListWithHandle_GetReturnsExpected(int value)
        {
            var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.SelectedImageIndex = value;
            Assert.Equal(-1, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);

            // Set same.
            treeView.SelectedImageIndex = value;
            Assert.Equal(-1, treeView.SelectedImageIndex);
            Assert.Empty(treeView.SelectedImageKey);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void SelectedImageIndex_SetWithEmptyImageListWithHandle_GetReturnsExpected(int value)
        {
            var imageList = new ImageList();
            var treeView = new TreeView
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

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        public void SelectedImageIndex_SetWithImageListWithHandle_GetReturnsExpected(int value, int expected)
        {
            var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add(new Bitmap(10, 10));
            var treeView = new TreeView
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

        [Fact]
        public void SelectedImageIndex_SetInvalid_Throws()
        {
            var treeView = new TreeView();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => treeView.SelectedImageIndex = -2);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("(none)", "")]
        public void SelectedImageKey_SetWithoutImageList_GetReturnsExpected(string value, string expected)
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("(none)", "")]
        public void SelectedImageKey_SetWithoutImageListWithImageIndex_GetReturnsExpected(string value, string expected)
        {
            var treeView = new TreeView
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

        [Theory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void SelectedImageKey_SetWithEmptyImageList_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            var imageList = new ImageList();
            var treeView = new TreeView
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

        [Theory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void SelectedImageKey_SetWithEmptyImageListWithImageIndex_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            var imageList = new ImageList();
            var treeView = new TreeView
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

        [Theory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void SelectedImageKey_SetWithNonEmptyImageList_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add("imageKey", new Bitmap(10, 10));
            var treeView = new TreeView
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

        [Theory]
        [InlineData(null, "", 0)]
        [InlineData("", "",  1)]
        [InlineData("reasonable", "reasonable",  -1)]
        [InlineData("(none)", "",  0)]
        [InlineData("imageKey", "imageKey", -1)]
        [InlineData("ImageKey", "ImageKey", -1)]
        public void SelectedImageKey_SetWithNonEmptyImageListWithImageIndex_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add("imageKey", new Bitmap(10, 10));
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("(none)", "")]
        public void SelectedImageKey_SetWithoutImageListWithHandle_GetReturnsExpected(string value, string expected)
        {
            var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.SelectedImageKey = value;
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(-1, treeView.SelectedImageIndex);

            // Set same.
            treeView.SelectedImageKey = value;
            Assert.Equal(expected, treeView.SelectedImageKey);
            Assert.Equal(-1, treeView.SelectedImageIndex);
        }

        [Theory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void SelectedImageKey_SetWithEmptyImageListWithHandle_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            var imageList = new ImageList();
            var treeView = new TreeView
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

        [Theory]
        [MemberData(nameof(ImageKey_Set_TestData))]
        public void SelectedImageKey_SetWithNonEmptyImageListWithHandle_GetReturnsExpected(string value, string expected, int expectedImageIndex)
        {
            var imageList = new ImageList();
            imageList.Images.Add(new Bitmap(10, 10));
            imageList.Images.Add("imageKey", new Bitmap(10, 10));
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ShowLines_Set_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ShowLines_SetWithHandle_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView();
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

        [Fact]
        public void ShowLines_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new TreeView
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

        [Fact]
        public void ShowLines_SetWithUpdateStylesHandlerWithHandle_CallsStyleChangedCallsInvalidated()
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ShowNodeToolTips_Set_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ShowNodeToolTips_SetWithHandle_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView();
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

        [Fact]
        public void ShowNodeToolTips_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new TreeView
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

        [Fact]
        public void ShowNodeToolTips_SetWithUpdateStylesHandlerWithHandle_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ShowPlusMinus_Set_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ShowPlusMinus_SetWithHandle_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView();
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

        [Fact]
        public void ShowPlusMinus_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new TreeView
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

        [Fact]
        public void ShowPlusMinus_SetWithUpdateStylesHandlerWithHandle_CallsStyleChangedCallsInvalidated()
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ShowRootLines_Set_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ShowRootLines_SetWithHandle_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView();
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

        [Fact]
        public void ShowRootLines_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new TreeView
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

        [Fact]
        public void ShowRootLines_SetWithUpdateStylesHandlerWithHandle_CallsStyleChangedCallsInvalidated()
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Sorted_Set_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Sorted_SetWithHandle_GetReturnsExpected(bool value)
        {
            var treeView = new TreeView();
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

        [Fact]
        public void Sorted_SetWithUpdateStylesHandler_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new TreeView
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

        [Fact]
        public void Sorted_SetWithUpdateStylesHandlerWithHandle_DoesNotCallStyleChangedDoesNotCallInvalidated()
        {
            var treeView = new TreeView
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

        [Theory]
        [MemberData(nameof(ImageList_TestData))]
        public void StateImageList_Set_GetReturnsExpected(ImageList value)
        {
            var treeView = new TreeView
            {
                StateImageList = value
            };
            Assert.Same(value, treeView.StateImageList);

            // Set same.
            treeView.StateImageList = value;
            Assert.Same(value, treeView.StateImageList);
        }

        [Theory]
        [MemberData(nameof(ImageList_TestData))]
        public void StateImageList_SetWithNonNullOldValue_GetReturnsExpected(ImageList value)
        {
            var treeView = new TreeView
            {
                StateImageList = new ImageList()
            };

            treeView.StateImageList = value;
            Assert.Same(value, treeView.StateImageList);

            // Set same.
            treeView.StateImageList = value;
            Assert.Same(value, treeView.StateImageList);
        }

        [Theory]
        [MemberData(nameof(ImageList_TestData))]
        public void StateImageList_SetWithHandle_GetReturnsExpected(ImageList value)
        {
            var treeView = new TreeView();
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.StateImageList = value;
            Assert.Same(value, treeView.StateImageList);

            // Set same.
            treeView.StateImageList = value;
            Assert.Same(value, treeView.StateImageList);
        }

        [Theory]
        [MemberData(nameof(ImageList_TestData))]
        public void StateImageList_SetWithNonNullOldValueWithHandle_GetReturnsExpected(ImageList value)
        {
            var treeView = new TreeView
            {
                StateImageList = new ImageList()
            };
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);

            treeView.StateImageList = value;
            Assert.Same(value, treeView.StateImageList);

            // Set same.
            treeView.StateImageList = value;
            Assert.Same(value, treeView.StateImageList);
        }

        [Fact]
        public void StateImageList_Dispose_DetachesFromTreeView()
        {
            var imageList1 = new ImageList();
            var imageList2 = new ImageList();
            var treeView = new TreeView
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

        [Fact]
        public void StateImageList_CreateHandle_DetachesFromTreeView()
        {
            var imageList = new ImageList();
            var treeView = new TreeView
            {
                StateImageList = imageList
            };
            Assert.Same(imageList, treeView.StateImageList);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);
        }

        [Fact]
        public void StateImageList_CreateHandleWithHandle_DetachesFromTreeView()
        {
            var imageList = new ImageList();
            var treeView = new TreeView
            {
                StateImageList = imageList
            };
            Assert.Same(imageList, treeView.StateImageList);
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);
        }

        [Fact]
        public void StateImageList_RecreateHandle_DetachesFromTreeView()
        {
            var imageList = new ImageList();
            var treeView = new TreeView
            {
                StateImageList = imageList
            };
            Assert.Same(imageList, treeView.StateImageList);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);

            imageList.ImageSize = new Size(10, 10);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);
        }

        [Fact]
        public void StateImageList_RecreateHandleWithHandle_DetachesFromTreeView()
        {
            var imageList = new ImageList();
            var treeView = new TreeView
            {
                StateImageList = imageList
            };
            Assert.Same(imageList, treeView.StateImageList);
            Assert.NotEqual(IntPtr.Zero, treeView.Handle);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);

            imageList.ImageSize = new Size(10, 10);
            Assert.NotEqual(IntPtr.Zero, imageList.Handle);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void Text_Set_GetReturnsExpected(string value, string expected)
        {
            var treeView = new TreeView
            {
                Text = value
            };
            Assert.Equal(expected, treeView.Text);

            // Set same.
            treeView.Text = value;
            Assert.Equal(expected, treeView.Text);
        }

        [Fact]
        public void Text_SetWithHandler_CallsTextChanged()
        {
            var control = new TreeView();
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

        [Theory]
        [MemberData(nameof(TreeViewNodeSorter_TestData))]
        public void TreeViewNodeSorter_Set_GetReturnsExpected(IComparer value)
        {
            var treeView = new TreeView
            {
                TreeViewNodeSorter = value
            };
            Assert.Same(value, treeView.TreeViewNodeSorter);

            // Set same.
            treeView.TreeViewNodeSorter = value;
            Assert.Same(value, treeView.TreeViewNodeSorter);
        }

        [Fact]
        public void AddExistingNodeAsChild_ThrowsArgumentException()
        {
            var treeView = new TreeView();
            var node = new TreeNode();
            treeView.Nodes.Add(node);

            Assert.Throws<ArgumentException>(() => node.Nodes.Add(node));
        }

        private class SubTreeView : TreeView
        {
            public new Size DefaultSize => base.DefaultSize;

            public new bool DoubleBuffered
            {
                get => base.DoubleBuffered;
                set => base.DoubleBuffered = value;
            }
        }
    }
}

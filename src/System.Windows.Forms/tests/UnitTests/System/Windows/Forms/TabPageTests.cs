﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TabPageTests
    {
        [WinFormsFact]
        public void TabPage_Ctor_Default()
        {
            using var control = new SubTabPage();
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoScroll);
            Assert.Equal(Size.Empty, control.AutoScrollMargin);
            Assert.Equal(Size.Empty, control.AutoScrollMinSize);
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.False(control.AutoSize);
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(BorderStyle.None, control.BorderStyle);
            Assert.Equal(100, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.ClientRectangle);
            Assert.Equal(new Size(200, 100), control.ClientSize);
            Assert.Null(control.Container);
            Assert.True(control.CausesValidation);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.IsType<TabPage.TabPageControlCollection>(control.Controls);
            Assert.False(control.Created);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(new Size(200, 100), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.NotNull(control.DockPadding);
            Assert.Same(control.DockPadding, control.DockPadding);
            Assert.Equal(0, control.DockPadding.Top);
            Assert.Equal(0, control.DockPadding.Bottom);
            Assert.Equal(0, control.DockPadding.Left);
            Assert.Equal(0, control.DockPadding.Right);
            Assert.False(control.DoubleBuffered);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(100, control.Height);
            Assert.NotNull(control.HorizontalScroll);
            Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
            Assert.False(control.HScroll);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal(Size.Empty, control.PreferredSize);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(200, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(new Size(200, 100), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.False(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseVisualStyleBackColor);
            Assert.True(control.Visible);
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.False(control.VScroll);
            Assert.Equal(200, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabPage_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubTabPage();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x10000, createParams.ExStyle);
            Assert.Equal(100, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56000000, createParams.Style);
            Assert.Equal(200, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [Theory]
        [MemberData(nameof(ControlTests.Anchor_Set_TestData), MemberType = typeof(ControlTests))]
        public void TabPage_Anchor_Set_GetReturnsExpected(AnchorStyles value, AnchorStyles expected)
        {
            var control = new TabPage
            {
                Anchor = value
            };
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);

            // Set same.
            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);
        }

        [Theory]
        [MemberData(nameof(ControlTests.Anchor_Set_TestData), MemberType = typeof(ControlTests))]
        public void TabPage_Anchor_SetWithOldValue_GetReturnsExpected(AnchorStyles value, AnchorStyles expected)
        {
            var control = new TabPage
            {
                Anchor = AnchorStyles.Left
            };

            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);

            // Set same.
            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);
        }

        [Theory]
        [MemberData(nameof(ControlTests.Anchor_SetWithDock_TestData), MemberType = typeof(ControlTests))]
        public void TabPage_Anchor_SetWithDock_GetReturnsExpected(DockStyle dock, AnchorStyles value, AnchorStyles expectedAnchor, DockStyle expectedDock)
        {
            var control = new TabPage
            {
                Dock = dock
            };

            control.Anchor = value;
            Assert.Equal(expectedAnchor, control.Anchor);
            Assert.Equal(expectedDock, control.Dock);

            // Set same.
            control.Anchor = value;
            Assert.Equal(expectedAnchor, control.Anchor);
            Assert.Equal(expectedDock, control.Dock);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TabPage_AutoSize_Set_GetReturnsExpected(bool value)
        {
            var control = new TabPage
            {
                AutoSize = value
            };
            Assert.Equal(value, control.AutoSize);

            // Set same.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);

            // Set different.
            control.AutoSize = !value;
            Assert.Equal(!value, control.AutoSize);
        }

        [Fact]
        public void TabPage_AutoSize_SetWithHandler_CallsAutoSizeChanged()
        {
            var control = new TabPage
            {
                AutoSize = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.AutoSizeChanged += handler;

            // Set different.
            control.AutoSize = false;
            Assert.False(control.AutoSize);
            Assert.Equal(1, callCount);

            // Set same.
            control.AutoSize = false;
            Assert.False(control.AutoSize);
            Assert.Equal(1, callCount);

            // Set different.
            control.AutoSize = true;
            Assert.True(control.AutoSize);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.AutoSizeChanged -= handler;
            control.AutoSize = false;
            Assert.False(control.AutoSize);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoSizeMode))]
        public void TabPage_AutoSizeMode_Set_GetReturnsExpected(AutoSizeMode value)
        {
            var page = new TabPage
            {
                AutoSizeMode = value
            };
            Assert.Equal(AutoSizeMode.GrowOnly, page.AutoSizeMode);

            // Set same.
            page.AutoSizeMode = value;
            Assert.Equal(AutoSizeMode.GrowOnly, page.AutoSizeMode);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoSizeMode))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AutoSizeMode))]
        public void TabPage_AutoSizeMode_SetWithParent_GetReturnsExpected(AutoSizeMode value)
        {
            var parent = new TabControl();
            var page = new TabPage
            {
                Parent = parent,
                AutoSizeMode = value
            };
            Assert.Equal(AutoSizeMode.GrowOnly, page.AutoSizeMode);

            // Set same.
            page.AutoSizeMode = value;
            Assert.Equal(AutoSizeMode.GrowOnly, page.AutoSizeMode);
        }

        public static IEnumerable<object[]> BackColor_Get_TestData()
        {
            yield return new object[] { true, null, Control.DefaultBackColor };
            yield return new object[] { false, null, Control.DefaultBackColor };

            yield return new object[] { true, new TabControl { Appearance = TabAppearance.Normal }, Color.Transparent };
            yield return new object[] { false, new TabControl { Appearance = TabAppearance.Normal }, Control.DefaultBackColor };

            yield return new object[] { true, new TabControl { Appearance = TabAppearance.Buttons }, Control.DefaultBackColor };
            yield return new object[] { false, new TabControl { Appearance = TabAppearance.Buttons }, Control.DefaultBackColor };

            yield return new object[] { true, new TabControl { Appearance = TabAppearance.FlatButtons }, Control.DefaultBackColor };
            yield return new object[] { false, new TabControl { Appearance = TabAppearance.FlatButtons }, Control.DefaultBackColor };
        }

        [Theory]
        [MemberData(nameof(BackColor_Get_TestData))]
        public void TabPage_BackColor_GetWithParent_ReturnsExpected(bool useVisualStyleBackColor, TabControl parent, Color expected)
        {
            if (!Application.RenderWithVisualStyles)
            {
                return;
            }
            var page = new TabPage
            {
                UseVisualStyleBackColor = useVisualStyleBackColor,
                Parent = parent
            };
            Assert.Equal(expected, page.BackColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void TabPage_BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            var control = new TabPage
            {
                BackColor = value
            };
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.UseVisualStyleBackColor);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.UseVisualStyleBackColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void TabPage_BackColor_SetWithUseVisualStyleBackColor_GetReturnsExpected(Color value, Color expected)
        {
            var control = new TabPage
            {
                UseVisualStyleBackColor = true,
                BackColor = value
            };
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.UseVisualStyleBackColor);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.UseVisualStyleBackColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void TabPage_BackColor_SetDesignMode_GetReturnsExpected(Color value, Color expected)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            var control = new TabPage
            {
                Site = mockSite.Object,
                BackColor = value
            };
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.UseVisualStyleBackColor);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.UseVisualStyleBackColor);
        }

        [Fact]
        public void TabPage_BackColor_SetWithHandler_CallsBackColorChanged()
        {
            var control = new TabPage();
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
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DockStyle))]
        public void TabPage_Dock_Set_GetReturnsExpected(DockStyle value)
        {
            var control = new TabPage
            {
                Dock = value
            };
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);

            // Set same.
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DockStyle))]
        public void TabPage_Dock_SetWithOldValue_GetReturnsExpected(DockStyle value)
        {
            var control = new TabPage
            {
                Dock = DockStyle.Top
            };
             
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);

            // Set same.
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        }

        [Theory]
        [MemberData(nameof(ControlTests.Dock_SetWithAnchor_TestData), MemberType = typeof(ControlTests))]
        public void TabPage_Dock_SetWithAnchor_GetReturnsExpected(AnchorStyles anchor, DockStyle value, AnchorStyles expectedAnchor)
        {
            var control = new TabPage
            {
                Anchor = anchor
            };
             
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(expectedAnchor, control.Anchor);

            // Set same.
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(expectedAnchor, control.Anchor);
        }

        [Fact]
        public void TabPage_Dock_SetWithHandler_CallsDockChanged()
        {
            var control = new TabPage();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.DockChanged += handler;

            // Set different.
            control.Dock = DockStyle.Top;
            Assert.Equal(DockStyle.Top, control.Dock);
            Assert.Equal(1, callCount);

            // Set same.
            control.Dock = DockStyle.Top;
            Assert.Equal(DockStyle.Top, control.Dock);
            Assert.Equal(1, callCount);

            // Set different.
            control.Dock = DockStyle.Left;
            Assert.Equal(DockStyle.Left, control.Dock);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.DockChanged -= handler;
            control.Dock = DockStyle.Top;
            Assert.Equal(DockStyle.Top, control.Dock);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DockStyle))]
        public void TabPage_Dock_SetInvalid_ThrowsInvalidEnumArgumentException(DockStyle value)
        {
            var control = new TabPage();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.Dock = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TabPage_Enabled_Set_GetReturnsExpected(bool value)
        {
            var control = new TabPage
            {
                Enabled = value
            };
            Assert.Equal(value, control.Enabled);

            // Set same.
            control.Enabled = value;
            Assert.Equal(value, control.Enabled);

            // Set different.
            control.Enabled = value;
            Assert.Equal(value, control.Enabled);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TabPage_Enabled_SetWithHandle_GetReturnsExpected(bool value)
        {
            var control = new TabPage();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Enabled = value;
            Assert.Equal(value, control.Enabled);

            // Set same.
            control.Enabled = value;
            Assert.Equal(value, control.Enabled);

            // Set different.
            control.Enabled = value;
            Assert.Equal(value, control.Enabled);
        }

        [Fact]
        public void TabPage_Enabled_SetWithHandler_CallsEnabledChanged()
        {
            var control = new TabPage
            {
                Enabled = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.EnabledChanged += handler;

            // Set different.
            control.Enabled = false;
            Assert.False(control.Enabled);
            Assert.Equal(1, callCount);

            // Set same.
            control.Enabled = false;
            Assert.False(control.Enabled);
            Assert.Equal(1, callCount);

            // Set different.
            control.Enabled = true;
            Assert.True(control.Enabled);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.EnabledChanged -= handler;
            control.Enabled = false;
            Assert.False(control.Enabled);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPointTheoryData))]
        public void TabPage_Location_Set_GetReturnsExpected(Point value)
        {
            var control = new TabPage
            {
                Location = value
            };
            Assert.Equal(value, control.Location);

            // Set same.
            control.Location = value;
            Assert.Equal(value, control.Location);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPointTheoryData))]
        public void TabPage_Location_SetWithParent_GetReturnsExpected(Point value)
        {
            var parent = new TabControl();
            var control = new TabPage
            {
                Parent = parent,
                Location = value
            };
            Assert.Equal(value, control.Location);

            // Set same.
            control.Location = value;
            Assert.Equal(value, control.Location);
        }

        [Fact]
        public void TabPage_Location_SetWithHandle_DoesNotCallInvalidate()
        {
            var control = new TabPage();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            // Set different.
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            control.Location = new Point(2, 3);
            Assert.Equal(new Point(2, 3), control.Location);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void TabPage_Location_SetWithHandleWithTransparentBackColor_DoesNotCallInvalidate(bool supportsTransparentBackgroundColor, int expectedInvalidatedCallCount)
        {
            var control = new SubTabPage();
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            control.BackColor = Color.FromArgb(254, 255, 255, 255);
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackgroundColor);

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            // Set different.
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Set same.
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Set different.
            control.Location = new Point(2, 3);
            Assert.Equal(new Point(2, 3), control.Location);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
        }

        [Fact]
        public void TabPage_Location_SetWithHandler_CallsLocationChanged()
        {
            var control = new TabPage();
            int locationChangedCallCount = 0;
            EventHandler locationChangedHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                locationChangedCallCount++;
            };
            control.LocationChanged += locationChangedHandler;
            int moveCallCount = 0;
            EventHandler moveHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                moveCallCount++;
            };
            control.Move += moveHandler;

            // Set different.
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);

            // Set same.
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);

            // Set different x.
            control.Location = new Point(2, 2);
            Assert.Equal(new Point(2, 2), control.Location);
            Assert.Equal(2, locationChangedCallCount);
            Assert.Equal(2, moveCallCount);

            // Set different y.
            control.Location = new Point(2, 3);
            Assert.Equal(new Point(2, 3), control.Location);
            Assert.Equal(3, locationChangedCallCount);
            Assert.Equal(3, moveCallCount);

            // Remove handler.
            control.LocationChanged -= locationChangedHandler;
            control.Move -= moveHandler;
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(3, locationChangedCallCount);
            Assert.Equal(3, moveCallCount);
        }

        public static IEnumerable<object[]> Parent_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new TabControl() };
        }

        [Theory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void TabPage_Parent_Set_GetReturnsExpected(Control value)
        {
            var control = new TabPage
            {
                Parent = value
            };
            Assert.Same(value, control.Parent);

            // Set same.
            control.Parent = value;
            Assert.Same(value, control.Parent);
        }

        [Theory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void TabPage_Parent_SetWithNonNullOldParent_GetReturnsExpected(Control value)
        {
            var oldParent = new TabControl();
            var control = new TabPage
            {
                Parent = oldParent
            };
            
            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.Empty(oldParent.Controls);

            // Set same.
            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.Empty(oldParent.Controls);
        }

        [Fact]
        public void TabPage_Parent_SetNonNull_AddsToControls()
        {
            var parent = new TabControl();
            var control = new TabPage
            {
                Parent = parent
            };
            Assert.Same(parent, control.Parent);
            Assert.Same(control, Assert.Single(parent.Controls));

            // Set same.
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Same(control, Assert.Single(parent.Controls));
        }

        [Fact]
        public void TabPage_Parent_SetWithHandler_CallsParentChanged()
        {
            var parent = new TabControl();
            var control = new TabPage();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.ParentChanged += handler;

            // Set different.
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Equal(1, callCount);

            // Set same.
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Equal(1, callCount);

            // Set null.
            control.Parent = null;
            Assert.Null(control.Parent);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ParentChanged -= handler;
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void TabPage_Parent_SetSame_ThrowsArgumentException()
        {
            var control = new TabPage();
            Assert.Throws<ArgumentException>(null, () => control.Parent = control);
            Assert.Null(control.Parent);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TabPage_UseVisualStyleBackColor_Set_GetReturnsExpected(bool value)
        {
            var tabPage = new TabPage
            {
                UseVisualStyleBackColor = value
            };
            Assert.Equal(value, tabPage.UseVisualStyleBackColor);
            
            // Set same.
            tabPage.UseVisualStyleBackColor = value;
            Assert.Equal(value, tabPage.UseVisualStyleBackColor);
            
            // Set different.
            tabPage.UseVisualStyleBackColor = !value;
            Assert.Equal(!value, tabPage.UseVisualStyleBackColor);
        }

        [Fact]
        public void TabPage_UseVisualStyleBackColor_SetWithHandle_CallsInvalidate()
        {
            var tabPage = new TabPage();
            Assert.NotEqual(IntPtr.Zero, tabPage.Handle);
            int invalidatedCallCount = 0;
            tabPage.Invalidated += (sender, e) => invalidatedCallCount++;

            // Set different.
            tabPage.UseVisualStyleBackColor = true;
            Assert.True(tabPage.UseVisualStyleBackColor);
            Assert.Equal(1, invalidatedCallCount);
            
            // Set same.
            tabPage.UseVisualStyleBackColor = true;
            Assert.True(tabPage.UseVisualStyleBackColor);
            Assert.Equal(1, invalidatedCallCount);

            // Set different.
            tabPage.UseVisualStyleBackColor = false;
            Assert.False(tabPage.UseVisualStyleBackColor);
            Assert.Equal(2, invalidatedCallCount);
        }

        [Fact]
        public void TabPage_ToString_Invoke_ReturnsExpected()
        {
            var page = new TabPage();
            Assert.Equal("TabPage: {}", page.ToString());
        }

        private class SubTabPage : TabPage
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

            public new bool HScroll
            {
                get => base.HScroll;
                set => base.HScroll = value;
            }

            public new bool ResizeRedraw
            {
                get => base.ResizeRedraw;
                set => base.ResizeRedraw = value;
            }

            public new bool VScroll
            {
                get => base.VScroll;
                set => base.VScroll = value;
            }

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
        }
    }
}

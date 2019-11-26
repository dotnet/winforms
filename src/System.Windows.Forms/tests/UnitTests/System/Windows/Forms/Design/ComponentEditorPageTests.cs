// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class ComponentEditorPageTests
    {
        public ComponentEditorPageTests()
        {
            Application.ThreadException += (sender, e) => throw new Exception(e.Exception.StackTrace.ToString());
        }

        [WinFormsFact]
        public void ComponentEditorPagePanel_Ctor_Default()
        {
            using var control = new SubComponentEditorPage();
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
            Assert.False(control.CommitOnDeactivate);
            Assert.Null(control.Component);
            Assert.Null(control.Container);
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
            Assert.True(control.FirstActivate);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(100, control.Height);
            Assert.NotNull(control.HorizontalScroll);
            Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
            Assert.False(control.HScroll);
            Assert.NotNull(control.Icon);
            Assert.Same(control.Icon, control.Icon);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Loading);
            Assert.False(control.LoadRequired);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.PageSite);
            Assert.Null(control.Parent);
            Assert.Equal(Size.Empty, control.PreferredSize);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(200, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Equal(new Size(200, 100), control.Size);
            Assert.Null(control.Site);
            Assert.Equal(0, control.TabIndex);
            Assert.False(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Empty(control.Title);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.Visible);
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.False(control.VScroll);
            Assert.Equal(200, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComponentEditorPage_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubComponentEditorPage();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x10000, createParams.ExStyle);
            Assert.Equal(100, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x46000000, createParams.Style);
            Assert.Equal(200, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ComponentEditorPage_AutoSize_Set_GetReturnsExpected(bool value)
        {
            var control = new SubComponentEditorPage
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
        public void ComponentEditorPage_AutoSize_SetWithHandler_CallsAutoSizeChanged()
        {
            var control = new SubComponentEditorPage
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
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ComponentEditorPage_CommitOnDeactivate_Set_GetReturnsExpected(bool value)
        {
            var page = new SubComponentEditorPage
            {
                CommitOnDeactivate = value
            };
            Assert.Equal(value, page.CommitOnDeactivate);

            // Set same.
            page.CommitOnDeactivate = value;
            Assert.Equal(value, page.CommitOnDeactivate);

            // Set different.
            page.CommitOnDeactivate = value;
            Assert.Equal(value, page.CommitOnDeactivate);
        }

        public static IEnumerable<object[]> Component_Set_TestData()
        {
            yield return new object[] { null };
            var mockComponent = new Mock<IComponent>(MockBehavior.Strict);
            mockComponent
                .Setup(c => c.Dispose());
            yield return new object[] { mockComponent.Object };
        }

        [Theory]
        [MemberData(nameof(Component_Set_TestData))]
        public void ComponentEditorPage_Component_Set_GetReturnsExpected(IComponent value)
        {
            var page = new SubComponentEditorPage
            {
                Component = value
            };
            Assert.Same(value, page.Component);
            Assert.Same(value, page.GetSelectedComponent());

            // Set same.
            page.Component = value;
            Assert.Same(value, page.Component);
            Assert.Same(value, page.GetSelectedComponent());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ComponentEditorPage_FirstActivate_Set_GetReturnsExpected(bool value)
        {
            var page = new SubComponentEditorPage
            {
                FirstActivate = value
            };
            Assert.Equal(value, page.FirstActivate);

            // Set same.
            page.FirstActivate = value;
            Assert.Equal(value, page.FirstActivate);

            // Set different.
            page.FirstActivate = value;
            Assert.Equal(value, page.FirstActivate);
        }

        [Fact]
        public void ComponentEditorPage_Icon_Set_GetReturnsExpected()
        {
            using (var value = Icon.FromHandle(new Bitmap(10, 10).GetHicon()))
            {
                var page = new SubComponentEditorPage
                {
                    Icon = value
                };
                Assert.Same(value, page.Icon);

                // Set same.
                page.Icon = value;
                Assert.Same(value, page.Icon);

                // Set null.
                page.Icon = null;
                Assert.NotSame(value, page.Icon);
                Assert.NotNull(page.Icon);
                Assert.Same(page.Icon, page.Icon);
            }
        }

        [Theory]
        [InlineData(-1, true)]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void ComponentEditorPage_Loading_Set_GetReturnsExpected(int value, bool expectedIsLoading)
        {
            var page = new SubComponentEditorPage
            {
                Loading = value
            };
            Assert.Equal(value, page.Loading);
            Assert.Equal(expectedIsLoading, page.IsLoading());

            // Set same.
            page.Loading = value;
            Assert.Equal(value, page.Loading);
            Assert.Equal(expectedIsLoading, page.IsLoading());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ComponentEditorPage_LoadRequired_Set_GetReturnsExpected(bool value)
        {
            var page = new SubComponentEditorPage
            {
                LoadRequired = value
            };
            Assert.Equal(value, page.LoadRequired);

            // Set same.
            page.LoadRequired = value;
            Assert.Equal(value, page.LoadRequired);

            // Set different.
            page.LoadRequired = value;
            Assert.Equal(value, page.LoadRequired);
        }

        public static IEnumerable<object[]> PageSite_TestData()
        {
            yield return new object[] { null };
            var mockComponentEditorPageSite = new Mock<IComponentEditorPageSite>(MockBehavior.Strict);
            yield return new object[] { mockComponentEditorPageSite.Object };
        }

        [Theory]
        [MemberData(nameof(PageSite_TestData))]
        public void ComponentEditorPage_PageSite_Set_GetReturnsExpected(IComponentEditorPageSite value)
        {
            var page = new SubComponentEditorPage
            {
                PageSite = value
            };
            Assert.Same(value, page.PageSite);

            // Set same.
            page.PageSite = value;
            Assert.Same(value, page.PageSite);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ComponentEditorPage_Text_Set_GetReturnsExpected(string value, string expected)
        {
            var control = new SubComponentEditorPage
            {
                Text = value
            };
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected, control.Title);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected, control.Title);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ComponentEditorPage_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
        {
            var control = new SubComponentEditorPage();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected, control.Title);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected, control.Title);
        }

        [Fact]
        public void ComponentEditorPage_Text_SetWithHandler_CallsTextChanged()
        {
            var control = new SubComponentEditorPage();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(EventArgs.Empty, e);
                callCount++;
            };
            control.TextChanged += handler;

            // Set different.
            control.Text = "text";
            Assert.Same("text", control.Text);
            Assert.Same("text", control.Title);
            Assert.Equal(1, callCount);

            // Set same.
            control.Text = "text";
            Assert.Same("text", control.Text);
            Assert.Same("text", control.Title);
            Assert.Equal(1, callCount);

            // Set different.
            control.Text = null;
            Assert.Empty(control.Text);
            Assert.Empty(control.Title);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.TextChanged -= handler;
            control.Text = "text";
            Assert.Same("text", control.Text);
            Assert.Same("text", control.Title);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ComponentEditorPage_Activate_Invoke_SetsVisible(bool loadRequired, int expectedLoadComponentCallCount)
        {
            var page = new SubComponentEditorPage
            {
                LoadRequired = loadRequired
            };
            page.Activate();
            Assert.Equal(expectedLoadComponentCallCount, page.LoadComponentCallCount);
            Assert.Equal(0, page.Loading);
            Assert.False(page.LoadRequired);
            Assert.True(page.Visible);

            // Call again.
            page.Activate();
            Assert.Equal(expectedLoadComponentCallCount, page.LoadComponentCallCount);
            Assert.Equal(0, page.Loading);
            Assert.False(page.LoadRequired);
            Assert.True(page.Visible);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ComponentEditorPage_ApplyChanges_Invoke_CallsSaveComponent(bool loadRequired)
        {
            var page = new SubComponentEditorPage
            {
                LoadRequired = loadRequired
            };
            page.ApplyChanges();
            Assert.Equal(1, page.SaveComponentCallCount);
            Assert.Equal(0, page.Loading);
            Assert.Equal(loadRequired, page.LoadRequired);

            // Call again.
            page.ApplyChanges();
            Assert.Equal(2, page.SaveComponentCallCount);
            Assert.Equal(0, page.Loading);
            Assert.Equal(loadRequired, page.LoadRequired);
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ComponentEditorPage_Deactivate_InvokeActivated_SetsInvisible(bool loadRequired, int expectedLoadComponentCallCount)
        {
            var page = new SubComponentEditorPage
            {
                LoadRequired = loadRequired
            };
            page.Activate();
            Assert.Equal(expectedLoadComponentCallCount, page.LoadComponentCallCount);
            Assert.Equal(0, page.Loading);
            Assert.False(page.LoadRequired);
            Assert.True(page.Visible);

            page.Deactivate();
            Assert.Equal(expectedLoadComponentCallCount, page.LoadComponentCallCount);
            Assert.Equal(0, page.Loading);
            Assert.False(page.LoadRequired);
            Assert.False(page.Visible);

            // Call again.
            page.Deactivate();
            Assert.Equal(expectedLoadComponentCallCount, page.LoadComponentCallCount);
            Assert.Equal(0, page.Loading);
            Assert.False(page.LoadRequired);
            Assert.False(page.Visible);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ComponentEditorPage_Deactivate_InvokeNotActivated_SetsInvisible(bool loadRequired)
        {
            var page = new SubComponentEditorPage
            {
                LoadRequired = loadRequired
            };
            page.Deactivate();
            Assert.Equal(0, page.LoadComponentCallCount);
            Assert.Equal(0, page.Loading);
            Assert.Equal(loadRequired, page.LoadRequired);
            Assert.False(page.Visible);

            // Call again.
            page.Deactivate();
            Assert.Equal(0, page.LoadComponentCallCount);
            Assert.Equal(0, page.Loading);
            Assert.Equal(loadRequired, page.LoadRequired);
            Assert.False(page.Visible);
        }

        [Fact]
        public void ComponentEditorPage_EnterLoadingMode_Invoke_IncrementsLoading()
        {
            var page = new SubComponentEditorPage();
            page.EnterLoadingMode();
            Assert.Equal(1, page.Loading);

            // Call again.
            page.EnterLoadingMode();
            Assert.Equal(2, page.Loading);
        }

        [Fact]
        public void ComponentEditorPage_EnterLoadingMode_ExitLoadingMode_Resets()
        {
            var page = new SubComponentEditorPage();
            page.EnterLoadingMode();
            Assert.Equal(1, page.Loading);

            page.ExitLoadingMode();
            Assert.Equal(0, page.Loading);
        }

        [Fact]
        public void ComponentEditorPage_ExitLoadingMode_Invoke_DoesNotDecrementLoading()
        {
            var page = new SubComponentEditorPage();
            page.ExitLoadingMode();
            Assert.Equal(0, page.Loading);

            // Call again.
            page.ExitLoadingMode();
            Assert.Equal(0, page.Loading);
        }

        [Fact]
        public void ComponentEditorPage_ExitLoadingMode_EnterLoadingMode_IncrementsLoading()
        {
            var page = new SubComponentEditorPage();
            page.ExitLoadingMode();
            Assert.Equal(0, page.Loading);

            page.EnterLoadingMode();
            Assert.Equal(1, page.Loading);
        }

        [Fact]
        public void ComponentEditorPage_GetControl_InvokeDefault_ReturnsSame()
        {
            var page = new SubComponentEditorPage();
            Assert.Same(page, page.GetControl());
        }

        [Fact]
        public void ComponentEditorPage_GetSelectedComponent_InvokeDefault_ReturnsNull()
        {
            var page = new SubComponentEditorPage();
            Assert.Null(page.GetSelectedComponent());
        }

        [Fact]
        public void ComponentEditorPage_IsFirstActivate_Invoke_ReturnsTrue()
        {
            var page = new SubComponentEditorPage();
            Assert.True(page.IsFirstActivate());
        }

        [Fact]
        public void ComponentEditorPage_IsLoading_Invoke_ReturnsFalse()
        {
            var page = new SubComponentEditorPage();
            Assert.False(page.IsLoading());
        }

        [Fact]
        public void ComponentEditorPage_IsPageMessage_Invoke_ReturnsExpected()
        {
            var page = new SubComponentEditorPage();
            var message = new Message();
            Assert.True(page.IsPageMessage(ref message));
            Assert.Equal(1, page.PreProcessMessageCallCount);
        }

        [WinFormsFact]
        public void ComponentEditorPage_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubComponentEditorPage();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, true)]
        [InlineData(ControlStyles.UserPaint, true)]
        [InlineData(ControlStyles.Opaque, false)]
        [InlineData(ControlStyles.ResizeRedraw, false)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, false)]
        [InlineData(ControlStyles.StandardClick, true)]
        [InlineData(ControlStyles.Selectable, false)]
        [InlineData(ControlStyles.UserMouse, false)]
        [InlineData(ControlStyles.SupportsTransparentBackColor, true)]
        [InlineData(ControlStyles.StandardDoubleClick, true)]
        [InlineData(ControlStyles.AllPaintingInWmPaint, false)]
        [InlineData(ControlStyles.CacheText, false)]
        [InlineData(ControlStyles.EnableNotifyMessage, false)]
        [InlineData(ControlStyles.DoubleBuffer, false)]
        [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
        [InlineData(ControlStyles.UseTextForAccessibility, true)]
        [InlineData((ControlStyles)0, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void ComponentEditorPage_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubComponentEditorPage();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void ComponentEditorPage_OnApplyComplete_Invoke_SetsLoadRequired(bool visible, bool expectedLoadRequired)
        {
            var page = new SubComponentEditorPage
            {
                Visible = visible
            };
            Assert.Equal(visible, page.Visible);
            page.OnApplyComplete();
            Assert.Equal(0, page.LoadComponentCallCount);
            Assert.Equal(0, page.Loading);
            Assert.Equal(expectedLoadRequired, page.LoadRequired);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void ComponentEditorPage_ReloadComponent_Invoke_SetsLoadRequired(bool visible, bool expectedLoadRequired)
        {
            var page = new SubComponentEditorPage
            {
                Visible = visible
            };
            Assert.Equal(visible, page.Visible);
            page.ReloadComponent();
            Assert.Equal(0, page.LoadComponentCallCount);
            Assert.Equal(0, page.Loading);
            Assert.Equal(expectedLoadRequired, page.LoadRequired);
        }

        [Theory]
        [MemberData(nameof(Component_Set_TestData))]
        public void ComponentEditorPage_SetComponent_Invoke_SetsComponent(IComponent component)
        {
            var page = new SubComponentEditorPage();
            page.SetComponent(component);
            Assert.Same(component, page.Component);
            Assert.Equal(0, page.LoadComponentCallCount);
            Assert.Equal(0, page.Loading);
            Assert.True(page.LoadRequired);

            // Set same.
            page.SetComponent(component);
            Assert.Same(component, page.Component);
            Assert.Equal(0, page.LoadComponentCallCount);
            Assert.Equal(0, page.Loading);
            Assert.True(page.LoadRequired);
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        public void ComponentEditorPage_SetDirty_InvokeWithPageSites_CallsSetDirty(int loading, int expectedSetDirtyCallCount)
        {
            var mockComponentEditorPageSite = new Mock<IComponentEditorPageSite>(MockBehavior.Strict);
            mockComponentEditorPageSite
                .Setup(s => s.SetDirty())
                .Verifiable();
            var page = new SubComponentEditorPage
            {
                PageSite = mockComponentEditorPageSite.Object,
                Loading = loading
            };
            page.SetDirty();
            mockComponentEditorPageSite.Verify(s => s.SetDirty(), Times.Exactly(expectedSetDirtyCallCount));

            // Call again.
            page.SetDirty();
            mockComponentEditorPageSite.Verify(s => s.SetDirty(), Times.Exactly(expectedSetDirtyCallCount * 2));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ComponentEditorPage_SetDirty_InvokeWithoutPageSite_Nop(int loading)
        {
            var page = new SubComponentEditorPage
            {
                Loading = loading
            };
            page.SetDirty();
            page.SetDirty();
        }

        [Fact]
        public void ComponentEditorPage_SetSite_Invoke_SetsPageSite()
        {
            var control = new Control();
            var controlSite = new Mock<IComponentEditorPageSite>(MockBehavior.Strict);
            controlSite
                .Setup(s => s.GetControl())
                .Returns(control)
                .Verifiable();
            var noControlSite = new Mock<IComponentEditorPageSite>(MockBehavior.Strict);
            noControlSite
                .Setup(s => s.GetControl())
                .Returns<Control>(null)
                .Verifiable();
            var page = new SubComponentEditorPage();
            page.SetSite(controlSite.Object);
            Assert.Same(controlSite.Object, page.PageSite);
            Assert.Same(page, Assert.Single(control.Controls));
            Assert.Equal(0, page.LoadComponentCallCount);
            Assert.Equal(0, page.Loading);
            Assert.False(page.LoadRequired);

            // Set same.
            page.SetSite(controlSite.Object);
            Assert.Same(controlSite.Object, page.PageSite);
            Assert.Same(page, Assert.Single(control.Controls));
            Assert.Equal(0, page.LoadComponentCallCount);
            Assert.Equal(0, page.Loading);
            Assert.False(page.LoadRequired);

            // Set different.
            page.SetSite(noControlSite.Object);
            Assert.Same(noControlSite.Object, page.PageSite);
            Assert.Same(page, Assert.Single(control.Controls));
            Assert.Equal(0, page.LoadComponentCallCount);
            Assert.Equal(0, page.Loading);
            Assert.False(page.LoadRequired);

            // Set null.
            page.SetSite(null);
            Assert.Null(page.PageSite);
            Assert.Same(page, Assert.Single(control.Controls));
            Assert.Equal(0, page.LoadComponentCallCount);
            Assert.Equal(0, page.Loading);
            Assert.False(page.LoadRequired);
        }

        [Fact]
        public void ComponentEditorPage_ShowHelp_Invoke_Nop()
        {
            var page = new SubComponentEditorPage();
            page.ShowHelp();
            page.ShowHelp();
        }

        [Fact]
        public void ComponentEditorPage_SupportsHelp_Invoke_ReturnsFalse()
        {
            var page = new SubComponentEditorPage();
            Assert.False(page.SupportsHelp());
            Assert.False(page.SupportsHelp());
        }

        private class SubComponentEditorPage : ComponentEditorPage
        {
            public new bool CanEnableIme => base.CanEnableIme;

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new IComponent Component
            {
                get => base.Component;
                set => base.Component = value;
            }

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

            public new bool FirstActivate
            {
                get => base.FirstActivate;
                set => base.FirstActivate = value;
            }

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

            public new bool HScroll
            {
                get => base.HScroll;
                set => base.HScroll = value;
            }

            public new int Loading
            {
                get => base.Loading;
                set => base.Loading = value;
            }

            public new bool LoadRequired
            {
                get => base.LoadRequired;
                set => base.LoadRequired = value;
            }

            public new IComponentEditorPageSite PageSite
            {
                get => base.PageSite;
                set => base.PageSite = value;
            }

            public new bool VScroll
            {
                get => base.VScroll;
                set => base.VScroll = value;
            }

            public new void EnterLoadingMode() => base.EnterLoadingMode();

            public new void ExitLoadingMode() => base.ExitLoadingMode();
            
            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new Control GetControl() => base.GetControl();

            public new IComponent GetSelectedComponent() => base.GetSelectedComponent();

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool IsFirstActivate() => base.IsFirstActivate();

            public new bool IsLoading() => base.IsLoading();

            public int LoadComponentCallCount { get; set; }

            protected override void LoadComponent() => LoadComponentCallCount++;

            public int PreProcessMessageCallCount { get; set; }

            public override bool PreProcessMessage(ref Message msg)
            {
                PreProcessMessageCallCount++;
                return true;
            }

            public new void ReloadComponent() => base.ReloadComponent();

            public int SaveComponentCallCount { get; set; }

            protected override void SaveComponent() => SaveComponentCallCount++;

            public new void SetDirty() => base.SetDirty();
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using Moq;
using Moq.Protected;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class ComponentEditorPageTests
    {
        [Fact]
        public void ComponentEditorPagePanel_Ctor_Default()
        {
            var page = new SubComponentEditorPage();
            Assert.False(page.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, page.Anchor);
            Assert.False(page.AutoScroll);
            Assert.Equal(Size.Empty, page.AutoScrollMargin);
            Assert.Equal(Size.Empty, page.AutoScrollMinSize);
            Assert.Equal(Point.Empty, page.AutoScrollPosition);
            Assert.False(page.AutoSize);
            Assert.Equal(AutoSizeMode.GrowOnly, page.AutoSizeMode);
            Assert.Equal(Control.DefaultBackColor, page.BackColor);
            Assert.Null(page.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, page.BackgroundImageLayout);
            Assert.Null(page.BindingContext);
            Assert.Equal(BorderStyle.None, page.BorderStyle);
            Assert.Equal(100, page.Bottom);
            Assert.Equal(new Rectangle(0, 0, 200, 100), page.Bounds);
            Assert.True(page.CanRaiseEvents);
            Assert.Equal(new Rectangle(0, 0, 200, 100), page.ClientRectangle);
            Assert.Equal(new Size(200, 100), page.ClientSize);
            Assert.Null(page.Container);
            Assert.True(page.CausesValidation);
            Assert.False(page.CommitOnDeactivate);
            Assert.Null(page.Component);
            Assert.Empty(page.Controls);
            Assert.Same(page.Controls, page.Controls);
            Assert.False(page.Created);
            Assert.Same(Cursors.Default, page.Cursor);
            Assert.Same(Cursors.Default, page.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, page.DefaultImeMode);
            Assert.Equal(new Padding(3), page.DefaultMargin);
            Assert.Equal(Size.Empty, page.DefaultMaximumSize);
            Assert.Equal(Size.Empty, page.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, page.DefaultPadding);
            Assert.Equal(new Size(200, 100), page.DefaultSize);
            Assert.False(page.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 200, 100), page.DisplayRectangle);
            Assert.Equal(DockStyle.None, page.Dock);
            Assert.NotNull(page.DockPadding);
            Assert.Same(page.DockPadding, page.DockPadding);
            Assert.Equal(0, page.DockPadding.Top);
            Assert.Equal(0, page.DockPadding.Bottom);
            Assert.Equal(0, page.DockPadding.Left);
            Assert.Equal(0, page.DockPadding.Right);
            Assert.True(page.Enabled);
            Assert.NotNull(page.Events);
            Assert.Same(page.Events, page.Events);
            Assert.True(page.FirstActivate);
            Assert.Equal(Control.DefaultFont, page.Font);
            Assert.Equal(Control.DefaultForeColor, page.ForeColor);
            Assert.False(page.HasChildren);
            Assert.Equal(100, page.Height);
            Assert.NotNull(page.HorizontalScroll);
            Assert.Same(page.HorizontalScroll, page.HorizontalScroll);
            Assert.False(page.HScroll);
            Assert.NotNull(page.Icon);
            Assert.Same(page.Icon, page.Icon);
            Assert.Equal(ImeMode.NoControl, page.ImeMode);
            Assert.Equal(ImeMode.NoControl, page.ImeModeBase);
            Assert.Equal(0, page.Left);
            Assert.Equal(0, page.Loading);
            Assert.False(page.LoadRequired);
            Assert.Equal(Point.Empty, page.Location);
            Assert.Equal(Padding.Empty, page.Padding);
            Assert.Null(page.PageSite);
            Assert.Equal(200, page.Right);
            Assert.Equal(RightToLeft.No, page.RightToLeft);
            Assert.Equal(new Size(200, 100), page.Size);
            Assert.Equal(0, page.TabIndex);
            Assert.False(page.TabStop);
            Assert.Empty(page.Text);
            Assert.Empty(page.Title);
            Assert.Equal(0, page.Top);
            Assert.False(page.Visible);
            Assert.NotNull(page.VerticalScroll);
            Assert.Same(page.VerticalScroll, page.VerticalScroll);
            Assert.False(page.VScroll);
            Assert.Equal(200, page.Width);
        }

        [Fact]
        public void ComponentEditorPage_CreateParams_GetDefault_ReturnsExpected()
        {
            var control = new SubComponentEditorPage();
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
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
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

            public new EventHandlerList Events => base.Events;

            public new bool FirstActivate
            {
                get => base.FirstActivate;
                set => base.FirstActivate = value;
            }

            public new ImeMode ImeModeBase => base.ImeModeBase;

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

            public new Control GetControl() => base.GetControl();

            public new IComponent GetSelectedComponent() => base.GetSelectedComponent();

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

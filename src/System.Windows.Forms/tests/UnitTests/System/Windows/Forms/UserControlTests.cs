// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;
using Moq;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class UserControlTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void UserControl_Ctor_Default()
        {
            using var control = new SubUserControl();
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Null(control.ActiveControl);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoScroll);
            Assert.Equal(SizeF.Empty, control.AutoScaleDimensions);
            Assert.Equal(new SizeF(1, 1), control.AutoScaleFactor);
            Assert.Equal(Size.Empty, control.AutoScrollMargin);
            Assert.Equal(AutoScaleMode.Inherit, control.AutoScaleMode);
            Assert.Equal(Size.Empty, control.AutoScrollMinSize);
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.NotNull(control.BindingContext);
            Assert.Same(control.BindingContext, control.BindingContext);
            Assert.Equal(150, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 150, 150), control.Bounds);
            Assert.False(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, 150, 150), control.ClientRectangle);
            Assert.Equal(new Size(150, 150), control.ClientSize);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Equal(SizeF.Empty, control.CurrentAutoScaleDimensions);
            Assert.Equal(Cursors.Default, control.Cursor);
            Assert.Equal(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(new Size(150, 150), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 150, 150), control.DisplayRectangle);
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
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(150, control.Height);
            Assert.NotNull(control.HorizontalScroll);
            Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
            Assert.False(control.HScroll);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
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
            Assert.Equal(150, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(150, 150), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.False(control.VScroll);
            Assert.Equal(150, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void UserControl_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubUserControl();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x10000, createParams.ExStyle);
            Assert.Equal(150, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56010000, createParams.Style);
            Assert.Equal(150, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(BorderStyle.None, 0x56010000, 0x10000)]
        [InlineData(BorderStyle.Fixed3D, 0x56010000, 0x10200)]
        [InlineData(BorderStyle.FixedSingle, 0x56810000, 0x10000)]
        public void UserControl_CreateParams_GetBorderStyle_ReturnsExpected(BorderStyle borderStyle, int expectedStyle, int expectedExStyle)
        {
            using var control = new SubUserControl
            {
                BorderStyle = borderStyle
            };
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(expectedExStyle, createParams.ExStyle);
            Assert.Equal(150, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(150, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void UserControl_AutoSize_Set_GetReturnsExpected(bool value)
        {
            using var control = new UserControl();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.AutoSize = !value;
            Assert.Equal(!value, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void UserControl_AutoSize_SetWithHandler_CallsAutoSizeChanged()
        {
            using var control = new UserControl
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

        [WinFormsTheory]
        [InlineData(AutoSizeMode.GrowAndShrink, 1)]
        [InlineData(AutoSizeMode.GrowOnly, 0)]
        public void UserControl_AutoSizeMode_Set_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
        {
            using var control = new SubUserControl();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("AutoSize", e.AffectedProperty);
                layoutCallCount++;
            };

            control.AutoSizeMode = value;
            Assert.Equal(value, control.AutoSizeMode);
            Assert.Equal(value, control.GetAutoSizeMode());
            Assert.False(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);

            // Set same.
            control.AutoSizeMode = value;
            Assert.Equal(value, control.AutoSizeMode);
            Assert.Equal(value, control.GetAutoSizeMode());
            Assert.False(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        }

        [WinFormsTheory]
        [InlineData(AutoSizeMode.GrowAndShrink, 1)]
        [InlineData(AutoSizeMode.GrowOnly, 0)]
        public void UserControl_AutoSizeMode_SetDesignMode_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new AmbientProperties());
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            using var control = new SubUserControl
            {
                Site = mockSite.Object
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("AutoSize", e.AffectedProperty);
                layoutCallCount++;
            };

            control.AutoSizeMode = value;
            Assert.Equal(value, control.AutoSizeMode);
            Assert.Equal(value, control.GetAutoSizeMode());
            Assert.False(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);

            // Set same.
            control.AutoSizeMode = value;
            Assert.Equal(value, control.AutoSizeMode);
            Assert.Equal(value, control.GetAutoSizeMode());
            Assert.False(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        }

        [WinFormsTheory]
        [InlineData(AutoSizeMode.GrowAndShrink, 1)]
        [InlineData(AutoSizeMode.GrowOnly, 0)]
        public void UserControl_AutoSizeMode_SetWithParent_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new SubUserControl
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("AutoSize", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                control.AutoSizeMode = value;
                Assert.Equal(value, control.AutoSizeMode);
                Assert.Equal(value, control.GetAutoSizeMode());
                Assert.False(control.IsHandleCreated);
                Assert.Equal(0, layoutCallCount);
                Assert.False(parent.IsHandleCreated);
                Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);

                // Set same.
                control.AutoSizeMode = value;
                Assert.Equal(value, control.AutoSizeMode);
                Assert.Equal(value, control.GetAutoSizeMode());
                Assert.False(control.IsHandleCreated);
                Assert.Equal(0, layoutCallCount);
                Assert.False(parent.IsHandleCreated);
                Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsTheory]
        [InlineData(AutoSizeMode.GrowAndShrink, 1)]
        [InlineData(AutoSizeMode.GrowOnly, 0)]
        public void UserControl_AutoSizeMode_SetDesignModeWithParent_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
        {
            using var parent = new Control();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new AmbientProperties());
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            using var control = new SubUserControl
            {
                Parent = parent,
                Site = mockSite.Object
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("AutoSize", e.AffectedProperty);
                layoutCallCount++;
            };
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
            parent.Layout += parentHandler;

            try
            {
                control.AutoSizeMode = value;
                Assert.Equal(value, control.AutoSizeMode);
                Assert.Equal(value, control.GetAutoSizeMode());
                Assert.False(control.IsHandleCreated);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.False(parent.IsHandleCreated);
                Assert.Equal(0, parentLayoutCallCount);

                // Set same.
                control.AutoSizeMode = value;
                Assert.Equal(value, control.AutoSizeMode);
                Assert.Equal(value, control.GetAutoSizeMode());
                Assert.False(control.IsHandleCreated);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.False(parent.IsHandleCreated);
                Assert.Equal(0, parentLayoutCallCount);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsTheory]
        [InlineData(AutoSizeMode.GrowAndShrink, 1)]
        [InlineData(AutoSizeMode.GrowOnly, 0)]
        public void UserControl_AutoSizeMode_SetWithCustomLayoutEngineParent_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
        {
            using var parent = new CustomLayoutEngineControl();
            using var control = new SubUserControl
            {
                Parent = parent
            };
            parent.SetLayoutEngine(new SubLayoutEngine());
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("AutoSize", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;
            try
            {
                control.AutoSizeMode = value;
                Assert.Equal(value, control.AutoSizeMode);
                Assert.Equal(value, control.GetAutoSizeMode());
                Assert.False(control.IsHandleCreated);
                Assert.Equal(0, layoutCallCount);
                Assert.False(parent.IsHandleCreated);
                Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);

                // Set same.
                control.AutoSizeMode = value;
                Assert.Equal(value, control.AutoSizeMode);
                Assert.Equal(value, control.GetAutoSizeMode());
                Assert.False(control.IsHandleCreated);
                Assert.Equal(0, layoutCallCount);
                Assert.False(parent.IsHandleCreated);
                Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        private class CustomLayoutEngineControl : Control
        {
            private LayoutEngine _layoutEngine;

            public CustomLayoutEngineControl()
            {
                _layoutEngine = new Control().LayoutEngine;
            }

            public void SetLayoutEngine(LayoutEngine layoutEngine)
            {
                _layoutEngine = layoutEngine;
            }

            public override LayoutEngine LayoutEngine => _layoutEngine;
        }

        private class SubLayoutEngine : LayoutEngine
        {
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoSizeMode))]
        public void UserControl_AutoSizeMode_SetWithHandle_GetReturnsExpected(AutoSizeMode value)
        {
            using var control = new SubUserControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.AutoSizeMode = value;
            Assert.Equal(value, control.AutoSizeMode);
            Assert.Equal(value, control.GetAutoSizeMode());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AutoSizeMode = value;
            Assert.Equal(value, control.AutoSizeMode);
            Assert.Equal(value, control.GetAutoSizeMode());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(AutoSizeMode.GrowAndShrink, 1)]
        [InlineData(AutoSizeMode.GrowOnly, 0)]
        public void UserControl_AutoSizeMode_SetWithParentWithHandle_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new SubUserControl
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("AutoSize", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            try
            {
                control.AutoSizeMode = value;
                Assert.Equal(value, control.AutoSizeMode);
                Assert.Equal(value, control.GetAutoSizeMode());
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);

                // Set same.
                control.AutoSizeMode = value;
                Assert.Equal(value, control.AutoSizeMode);
                Assert.Equal(value, control.GetAutoSizeMode());
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsTheory]
        [InlineData(AutoSizeMode.GrowAndShrink, 1)]
        [InlineData(AutoSizeMode.GrowOnly, 0)]
        public void UserControl_AutoSizeMode_SetWithCustomLayoutEngineParentWithHandle_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
        {
            using var parent = new CustomLayoutEngineControl();
            using var control = new SubUserControl
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("AutoSize", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            try
            {
                control.AutoSizeMode = value;
                Assert.Equal(value, control.AutoSizeMode);
                Assert.Equal(value, control.GetAutoSizeMode());
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);

                // Set same.
                control.AutoSizeMode = value;
                Assert.Equal(value, control.AutoSizeMode);
                Assert.Equal(value, control.GetAutoSizeMode());
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AutoSizeMode))]
        public void UserControl_AutoSizeMode_SetInvalid_ThrowsInvalidEnumArgumentException(AutoSizeMode value)
        {
            using var control = new UserControl();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.AutoSizeMode = value);
        }

        public static IEnumerable<object[]> AutoValidate_Set_TestData()
        {
            yield return new object[] { AutoValidate.Disable, AutoValidate.Disable };
            yield return new object[] { AutoValidate.EnablePreventFocusChange, AutoValidate.EnablePreventFocusChange };
            yield return new object[] { AutoValidate.EnableAllowFocusChange, AutoValidate.EnableAllowFocusChange };
            yield return new object[] { AutoValidate.Inherit, AutoValidate.EnablePreventFocusChange };
        }

        [WinFormsTheory]
        [MemberData(nameof(AutoValidate_Set_TestData))]
        public void UserControl_AutoValidate_Set_GetReturnsExpected(AutoValidate value, AutoValidate expected)
        {
            using var control = new UserControl
            {
                AutoValidate = value
            };
            Assert.Equal(expected, control.AutoValidate);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AutoValidate = value;
            Assert.Equal(expected, control.AutoValidate);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void UserControl_AutoValidate_SetWithHandler_CallsAutoValidateChanged()
        {
            using var control = new UserControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.AutoValidateChanged += handler;

            // Set different.
            control.AutoValidate = AutoValidate.EnablePreventFocusChange;
            Assert.Equal(AutoValidate.EnablePreventFocusChange, control.AutoValidate);
            Assert.Equal(1, callCount);

            // Set same.
            control.AutoValidate = AutoValidate.EnablePreventFocusChange;
            Assert.Equal(AutoValidate.EnablePreventFocusChange, control.AutoValidate);
            Assert.Equal(1, callCount);

            // Set different.
            control.AutoValidate = AutoValidate.EnableAllowFocusChange;
            Assert.Equal(AutoValidate.EnableAllowFocusChange, control.AutoValidate);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.AutoValidateChanged -= handler;
            control.AutoValidate = AutoValidate.Disable;
            Assert.False(control.AutoSize);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AutoValidate))]
        public void UserControl_AutoValidate_SetInvalidValue_ThrowsInvalidEnumArgumentException(AutoValidate value)
        {
            using var control = new UserControl();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.AutoValidate = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(BorderStyle))]
        public void UserControl_BorderStyle_Set_GetReturnsExpected(BorderStyle value)
        {
            using var control = new UserControl
            {
                BorderStyle = value
            };
            Assert.Equal(value, control.BorderStyle);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BorderStyle = value;
            Assert.Equal(value, control.BorderStyle);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(BorderStyle.Fixed3D, 1)]
        [InlineData(BorderStyle.FixedSingle, 1)]
        [InlineData(BorderStyle.None, 0)]
        public void UserControl_BorderStyle_SetWithHandle_GetReturnsExpected(BorderStyle value, int expectedInvalidatedCallCount)
        {
            using var control = new UserControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.BorderStyle = value;
            Assert.Equal(value, control.BorderStyle);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.BorderStyle = value;
            Assert.Equal(value, control.BorderStyle);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(BorderStyle))]
        public void UserControl_BorderStyle_SetInvalid_ThrowsInvalidEnumArgumentException(BorderStyle value)
        {
            using var control = new UserControl();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.BorderStyle = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void UserControl_Text_Set_GetReturnsExpected(string value, string expected)
        {
            using var control = new UserControl
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
        public void UserControl_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var control = new UserControl();
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
        public void UserControl_Text_SetWithHandler_CallsTextChanged()
        {
            using var control = new UserControl();
            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Equal(EventArgs.Empty, e);
                callCount++;
            }
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
            control.Text = null;
            Assert.Empty(control.Text);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.TextChanged -= handler;
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void UserControl_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubUserControl();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        [WinFormsTheory]
        [InlineData(0, true)]
        [InlineData(SubUserControl.ScrollStateAutoScrolling, false)]
        [InlineData(SubUserControl.ScrollStateFullDrag, false)]
        [InlineData(SubUserControl.ScrollStateHScrollVisible, false)]
        [InlineData(SubUserControl.ScrollStateUserHasScrolled, false)]
        [InlineData(SubUserControl.ScrollStateVScrollVisible, false)]
        [InlineData(int.MaxValue, false)]
        [InlineData((-1), false)]
        public void UserControl_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
        {
            using var control = new SubUserControl();
            Assert.Equal(expected, control.GetScrollState(bit));
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, true)]
        [InlineData(ControlStyles.UserPaint, true)]
        [InlineData(ControlStyles.Opaque, false)]
        [InlineData(ControlStyles.ResizeRedraw, false)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, false)]
        [InlineData(ControlStyles.StandardClick, true)]
        [InlineData(ControlStyles.Selectable, true)]
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
        public void UserControl_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubUserControl();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void UserControl_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubUserControl();
            Assert.False(control.GetTopLevel());
        }

        [WinFormsFact]
        public void UserControl_OnCreateControl_Invoke_Nop()
        {
            using var control = new SubUserControl();
            int bindingContextChangedCallCount = 0;
            int loadChangedCallCount = 0;
            control.BindingContextChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(bindingContextChangedCallCount, loadChangedCallCount);
                bindingContextChangedCallCount++;
            };
            control.Load += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(bindingContextChangedCallCount - 1, loadChangedCallCount);
                loadChangedCallCount++;
            };
            control.OnCreateControl();
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(1, bindingContextChangedCallCount);
            Assert.Equal(1, loadChangedCallCount);

            control.OnCreateControl();
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(2, bindingContextChangedCallCount);
            Assert.Equal(2, loadChangedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void UserControl_OnLoad_Invoke_CallsLoad(EventArgs eventArgs)
        {
            using var control = new SubUserControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Load += handler;
            control.OnLoad(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Load -= handler;
            control.OnLoad(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void UserControl_OnMouseDown_Invoke_CallsMouseDown(MouseEventArgs eventArgs)
        {
            using var control = new SubUserControl();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseDown += handler;
            control.OnMouseDown(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseDown -= handler;
            control.OnMouseDown(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void UserControl_OnMouseDown_InvokeWithHandle_CallsMouseDown(MouseEventArgs eventArgs)
        {
            using var control = new SubUserControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseDown += handler;
            control.OnMouseDown(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.MouseDown -= handler;
            control.OnMouseDown(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void UserControl_OnResize_Invoke_CallsResize(EventArgs eventArgs)
        {
            using var control = new SubUserControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };

            // Call with handler.
            control.Resize += handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.Resize -= handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void UserControl_OnResize_InvokeWithBackgroundImage_CallsResize(EventArgs eventArgs)
        {
            using var backgroundImage = new Bitmap(10, 10);
            using var control = new SubUserControl
            {
                BackgroundImage = backgroundImage
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };

            // Call with handler.
            control.Resize += handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.Resize -= handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnResize_WithHandle_TestData()
        {
            yield return new object[] { true, null, 1 };
            yield return new object[] { true, new EventArgs(), 1 };
            yield return new object[] { false, null, 0 };
            yield return new object[] { false, new EventArgs(), 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnResize_WithHandle_TestData))]
        public void UserControl_OnResize_InvokeWithHandle_CallsResize(bool resizeRedraw, EventArgs eventArgs, int expectedInvalidatedCallCount)
        {
            using var control = new SubUserControl();
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // Call with handler.
            control.Resize += handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.Resize -= handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnResize_WithHandle_TestData))]
        public void UserControl_OnResize_InvokeWithBackgroundImageWithHandle_CallsResize(bool resizeRedraw, EventArgs eventArgs, int expectedInvalidatedCallCount)
        {
            using var backgroundImage = new Bitmap(10, 10);
            using var control = new SubUserControl
            {
                BackgroundImage = backgroundImage
            };
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // Call with handler.
            control.Resize += handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.Resize -= handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal((expectedInvalidatedCallCount + 1) * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void UserControl_ValidateChildren_InvokeWithoutChildren_ReturnsTrue()
        {
            using var control = new UserControl();
            Assert.True(control.ValidateChildren());
        }

        public static IEnumerable<object[]> ValidateChildren_TestData()
        {
            yield return new object[] { true, 0 };
            yield return new object[] { false, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(ValidateChildren_TestData))]
        public void UserControl_ValidateChildren_InvokeWithChildren_ReturnsExpected(bool cancel, int expectedCallCount)
        {
            using var control = new UserControl();
            using var child1 = new Control();
            using var grandchild1 = new Control();
            child1.Controls.Add(grandchild1);
            using var child2 = new UserControl();
            using var grandchild2 = new Control();
            child2.Controls.Add(grandchild2);
            using var child3 = new TabControl();
            using var grandchild3 = new TabPage();
            child3.Controls.Add(grandchild3);
            using var child4 = new SubControl();
            child4.SetStyle(ControlStyles.Selectable, false);
            using var child5 = new SubControl
            {
                Enabled = false
            };
            using var child6 = new SubControl
            {
                Visible = false
            };
            using var child7 = new SubControl
            {
                TabStop = false
            };
            using var child8 = new SubControl
            {
                CausesValidation = false
            };
            control.Controls.Add(child1);
            control.Controls.Add(child2);
            control.Controls.Add(child3);
            control.Controls.Add(child4);
            control.Controls.Add(child5);
            control.Controls.Add(child6);
            control.Controls.Add(child7);
            control.Controls.Add(child8);

            int validatingCallCount = 0;
            control.Validating += (sender, e) => validatingCallCount++;
            int validatedCallCount = 0;
            control.Validated += (sender, e) => validatedCallCount++;

            int child1ValidatingCallCount = 0;
            child1.Validating += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.False(e.Cancel);
                child1ValidatingCallCount++;
                e.Cancel = cancel;
            };
            int child1ValidatedCallCount = 0;
            child1.Validated += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                child1ValidatedCallCount++;
            };
            int grandchild1ValidatingCallCount = 0;
            grandchild1.Validating += (sender, e) =>
            {
                Assert.Same(grandchild1, sender);
                Assert.False(e.Cancel);
                grandchild1ValidatingCallCount++;
            };
            int grandchild1ValidatedCallCount = 0;
            grandchild1.Validated += (sender, e) => grandchild1ValidatedCallCount++;
            int child2ValidatingCallCount = 0;
            child2.Validating += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.False(e.Cancel);
                child2ValidatingCallCount++;
            };
            int child2ValidatedCallCount = 0;
            child2.Validated += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                child2ValidatedCallCount++;
            };
            int grandchild2ValidatingCallCount = 0;
            grandchild2.Validating += (sender, e) =>
            {
                Assert.Same(grandchild2, sender);
                Assert.False(e.Cancel);
                grandchild2ValidatingCallCount++;
            };
            int grandchild2ValidatedCallCount = 0;
            grandchild2.Validated += (sender, e) =>
            {
                Assert.Same(grandchild2, sender);
                Assert.Same(EventArgs.Empty, e);
                grandchild2ValidatedCallCount++;
            };
            int child3ValidatingCallCount = 0;
            child3.Validating += (sender, e) =>
            {
                Assert.Same(child3, sender);
                Assert.False(e.Cancel);
                child3ValidatingCallCount++;
            };
            int child3ValidatedCallCount = 0;
            child3.Validated += (sender, e) =>
            {
                Assert.Same(child3, sender);
                Assert.Same(EventArgs.Empty, e);
                child3ValidatedCallCount++;
            };
            int grandchild3ValidatingCallCount = 0;
            grandchild3.Validating += (sender, e) => grandchild3ValidatingCallCount++;
            int grandchild3ValidatedCallCount = 0;
            grandchild3.Validated += (sender, e) => grandchild3ValidatedCallCount++;
            int child4ValidatingCallCount = 0;
            child4.Validating += (sender, e) => child4ValidatingCallCount++;
            int child4ValidatedCallCount = 0;
            child4.Validated += (sender, e) => child4ValidatedCallCount++;
            int child5ValidatingCallCount = 0;
            child5.Validating += (sender, e) =>
            {
                Assert.Same(child5, sender);
                Assert.False(e.Cancel);
                child5ValidatingCallCount++;
            };
            int child5ValidatedCallCount = 0;
            child5.Validated += (sender, e) =>
            {
                Assert.Same(child5, sender);
                Assert.Same(EventArgs.Empty, e);
                child5ValidatedCallCount++;
            };
            int child6ValidatingCallCount = 0;
            child6.Validating += (sender, e) =>
            {
                Assert.Same(child6, sender);
                Assert.False(e.Cancel);
                child6ValidatingCallCount++;
            };
            int child6ValidatedCallCount = 0;
            child6.Validated += (sender, e) =>
            {
                Assert.Same(child6, sender);
                Assert.Same(EventArgs.Empty, e);
                child6ValidatedCallCount++;
            };
            int child7ValidatingCallCount = 0;
            child7.Validating += (sender, e) =>
            {
                Assert.Same(child7, sender);
                Assert.False(e.Cancel);
                child7ValidatingCallCount++;
            };
            int child7ValidatedCallCount = 0;
            child7.Validated += (sender, e) =>
            {
                Assert.Same(child7, sender);
                Assert.Same(EventArgs.Empty, e);
                child7ValidatedCallCount++;
            };
            int child8ValidatingCallCount = 0;
            child8.Validating += (sender, e) => child8ValidatingCallCount++;
            int child8ValidatedCallCount = 0;
            child8.Validated += (sender, e) => child8ValidatedCallCount++;

            Assert.Equal(!cancel, control.ValidateChildren());
            Assert.Equal(0, validatingCallCount);
            Assert.Equal(0, validatedCallCount);
            Assert.Equal(1, child1ValidatingCallCount);
            Assert.Equal(expectedCallCount, child1ValidatedCallCount);
            Assert.Equal(0, grandchild1ValidatingCallCount);
            Assert.Equal(0, grandchild1ValidatedCallCount);
            Assert.Equal(1, child2ValidatingCallCount);
            Assert.Equal(1, child2ValidatedCallCount);
            Assert.Equal(1, grandchild2ValidatingCallCount);
            Assert.Equal(1, grandchild2ValidatedCallCount);
            Assert.Equal(1, child3ValidatingCallCount);
            Assert.Equal(1, child3ValidatedCallCount);
            Assert.Equal(0, grandchild3ValidatingCallCount);
            Assert.Equal(0, grandchild3ValidatedCallCount);
            Assert.Equal(0, child4ValidatingCallCount);
            Assert.Equal(0, child4ValidatedCallCount);
            Assert.Equal(1, child5ValidatingCallCount);
            Assert.Equal(1, child5ValidatedCallCount);
            Assert.Equal(1, child6ValidatingCallCount);
            Assert.Equal(1, child6ValidatedCallCount);
            Assert.Equal(1, child7ValidatingCallCount);
            Assert.Equal(1, child7ValidatedCallCount);
            Assert.Equal(0, child8ValidatingCallCount);
            Assert.Equal(0, child8ValidatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ValidationConstraints))]
        public void UserControl_ValidateChildren_InvokeValidationConstraintsWithoutChildren_ReturnsTrue(ValidationConstraints validationConstraints)
        {
            using var control = new UserControl();
            Assert.True(control.ValidateChildren(validationConstraints));
        }

        public static IEnumerable<object[]> ValidateChildren_ValidationConstraints_TestData()
        {
            yield return new object[] { ValidationConstraints.ImmediateChildren, true, 0, 0, 0, 1, 1, 1, 1 };
            yield return new object[] { ValidationConstraints.ImmediateChildren, false, 1, 0, 0, 1, 1, 1, 1 };

            yield return new object[] { ValidationConstraints.Selectable, true, 0, 1, 0, 0, 1, 1, 1 };
            yield return new object[] { ValidationConstraints.Selectable, false, 1, 1, 0, 0, 1, 1, 1 };

            yield return new object[] { ValidationConstraints.Enabled, true, 0, 1, 1, 1, 0, 1, 1 };
            yield return new object[] { ValidationConstraints.Enabled, false, 1, 1, 1, 1, 0, 1, 1 };

            yield return new object[] { ValidationConstraints.Visible, true, 0, 1, 0, 1, 1, 0, 1 };
            yield return new object[] { ValidationConstraints.Visible, false, 1, 1, 0, 1, 1, 0, 1 };

            yield return new object[] { ValidationConstraints.TabStop, true, 0, 1, 0, 1, 1, 1, 0 };
            yield return new object[] { ValidationConstraints.TabStop, false, 1, 1, 0, 1, 1, 1, 0 };

            yield return new object[] { ValidationConstraints.None, true, 0, 1, 1, 1, 1, 1, 1 };
            yield return new object[] { ValidationConstraints.None, false, 1, 1, 1, 1, 1, 1, 1 };

            yield return new object[] { ValidationConstraints.ImmediateChildren | ValidationConstraints.Selectable | ValidationConstraints.Enabled | ValidationConstraints.Visible | ValidationConstraints.TabStop, true, 0, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { ValidationConstraints.ImmediateChildren | ValidationConstraints.Selectable | ValidationConstraints.Enabled | ValidationConstraints.Visible | ValidationConstraints.TabStop, false, 1, 0, 0, 0, 0, 0, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(ValidateChildren_ValidationConstraints_TestData))]
        public void UserControl_ValidateChildren_InvokeValidationConstraintsWithChildren_ReturnsExpected(ValidationConstraints validationConstraints, bool cancel, int expectedChild1CallCount, int expectedGrandchild2CallCount, int expectedGrandchild3CallCount, int expectedChild4CallCount, int expectedChild5CallCount, int expectedChild6CallCount, int expectedChild7CallCount)
        {
            using var control = new UserControl();
            using var child1 = new Control();
            using var grandchild1 = new Control();
            child1.Controls.Add(grandchild1);
            using var child2 = new UserControl();
            using var grandchild2 = new Control();
            child2.Controls.Add(grandchild2);
            using var child3 = new TabControl();
            using var grandchild3 = new TabPage();
            child3.Controls.Add(grandchild3);
            using var child4 = new SubControl();
            child4.SetStyle(ControlStyles.Selectable, false);
            using var child5 = new SubControl
            {
                Enabled = false
            };
            using var child6 = new SubControl
            {
                Visible = false
            };
            using var child7 = new SubControl
            {
                TabStop = false
            };
            using var child8 = new SubControl
            {
                CausesValidation = false
            };
            control.Controls.Add(child1);
            control.Controls.Add(child2);
            control.Controls.Add(child3);
            control.Controls.Add(child4);
            control.Controls.Add(child5);
            control.Controls.Add(child6);
            control.Controls.Add(child7);
            control.Controls.Add(child8);

            int validatingCallCount = 0;
            control.Validating += (sender, e) => validatingCallCount++;
            int validatedCallCount = 0;
            control.Validated += (sender, e) => validatedCallCount++;

            int child1ValidatingCallCount = 0;
            child1.Validating += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.False(e.Cancel);
                child1ValidatingCallCount++;
                e.Cancel = cancel;
            };
            int child1ValidatedCallCount = 0;
            child1.Validated += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                child1ValidatedCallCount++;
            };
            int grandchild1ValidatingCallCount = 0;
            grandchild1.Validating += (sender, e) =>
            {
                Assert.Same(grandchild1, sender);
                Assert.False(e.Cancel);
                grandchild1ValidatingCallCount++;
            };
            int grandchild1ValidatedCallCount = 0;
            grandchild1.Validated += (sender, e) => grandchild1ValidatedCallCount++;
            int child2ValidatingCallCount = 0;
            child2.Validating += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.False(e.Cancel);
                child2ValidatingCallCount++;
            };
            int child2ValidatedCallCount = 0;
            child2.Validated += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                child2ValidatedCallCount++;
            };
            int grandchild2ValidatingCallCount = 0;
            grandchild2.Validating += (sender, e) =>
            {
                Assert.Same(grandchild2, sender);
                Assert.False(e.Cancel);
                grandchild2ValidatingCallCount++;
            };
            int grandchild2ValidatedCallCount = 0;
            grandchild2.Validated += (sender, e) =>
            {
                Assert.Same(grandchild2, sender);
                Assert.Same(EventArgs.Empty, e);
                grandchild2ValidatedCallCount++;
            };
            int child3ValidatingCallCount = 0;
            child3.Validating += (sender, e) =>
            {
                Assert.Same(child3, sender);
                Assert.False(e.Cancel);
                child3ValidatingCallCount++;
            };
            int child3ValidatedCallCount = 0;
            child3.Validated += (sender, e) =>
            {
                Assert.Same(child3, sender);
                Assert.Same(EventArgs.Empty, e);
                child3ValidatedCallCount++;
            };
            int grandchild3ValidatingCallCount = 0;
            grandchild3.Validating += (sender, e) => grandchild3ValidatingCallCount++;
            int grandchild3ValidatedCallCount = 0;
            grandchild3.Validated += (sender, e) => grandchild3ValidatedCallCount++;
            int child4ValidatingCallCount = 0;
            child4.Validating += (sender, e) => child4ValidatingCallCount++;
            int child4ValidatedCallCount = 0;
            child4.Validated += (sender, e) => child4ValidatedCallCount++;
            int child5ValidatingCallCount = 0;
            child5.Validating += (sender, e) =>
            {
                Assert.Same(child5, sender);
                Assert.False(e.Cancel);
                child5ValidatingCallCount++;
            };
            int child5ValidatedCallCount = 0;
            child5.Validated += (sender, e) =>
            {
                Assert.Same(child5, sender);
                Assert.Same(EventArgs.Empty, e);
                child5ValidatedCallCount++;
            };
            int child6ValidatingCallCount = 0;
            child6.Validating += (sender, e) =>
            {
                Assert.Same(child6, sender);
                Assert.False(e.Cancel);
                child6ValidatingCallCount++;
            };
            int child6ValidatedCallCount = 0;
            child6.Validated += (sender, e) =>
            {
                Assert.Same(child6, sender);
                Assert.Same(EventArgs.Empty, e);
                child6ValidatedCallCount++;
            };
            int child7ValidatingCallCount = 0;
            child7.Validating += (sender, e) =>
            {
                Assert.Same(child7, sender);
                Assert.False(e.Cancel);
                child7ValidatingCallCount++;
            };
            int child7ValidatedCallCount = 0;
            child7.Validated += (sender, e) =>
            {
                Assert.Same(child7, sender);
                Assert.Same(EventArgs.Empty, e);
                child7ValidatedCallCount++;
            };
            int child8ValidatingCallCount = 0;
            child8.Validating += (sender, e) => child8ValidatingCallCount++;
            int child8ValidatedCallCount = 0;
            child8.Validated += (sender, e) => child8ValidatedCallCount++;

            Assert.Equal(!cancel, control.ValidateChildren(validationConstraints));
            Assert.Equal(0, validatingCallCount);
            Assert.Equal(0, validatedCallCount);
            Assert.Equal(1, child1ValidatingCallCount);
            Assert.Equal(expectedChild1CallCount, child1ValidatedCallCount);
            Assert.Equal(0, grandchild1ValidatingCallCount);
            Assert.Equal(0, grandchild1ValidatedCallCount);
            Assert.Equal(1, child2ValidatingCallCount);
            Assert.Equal(1, child2ValidatedCallCount);
            Assert.Equal(expectedGrandchild2CallCount, grandchild2ValidatingCallCount);
            Assert.Equal(expectedGrandchild2CallCount, grandchild2ValidatedCallCount);
            Assert.Equal(1, child3ValidatingCallCount);
            Assert.Equal(1, child3ValidatedCallCount);
            Assert.Equal(expectedGrandchild3CallCount, grandchild3ValidatingCallCount);
            Assert.Equal(expectedGrandchild3CallCount, grandchild3ValidatedCallCount);
            Assert.Equal(expectedChild4CallCount, child4ValidatingCallCount);
            Assert.Equal(expectedChild4CallCount, child4ValidatedCallCount);
            Assert.Equal(expectedChild5CallCount, child5ValidatingCallCount);
            Assert.Equal(expectedChild5CallCount, child5ValidatedCallCount);
            Assert.Equal(expectedChild6CallCount, child6ValidatingCallCount);
            Assert.Equal(expectedChild6CallCount, child6ValidatedCallCount);
            Assert.Equal(expectedChild7CallCount, child7ValidatingCallCount);
            Assert.Equal(expectedChild7CallCount, child7ValidatedCallCount);
            Assert.Equal(0, child8ValidatingCallCount);
            Assert.Equal(0, child8ValidatedCallCount);
        }

        [WinFormsTheory]
        [InlineData((ValidationConstraints)(-1))]
        [InlineData((ValidationConstraints)(0x20))]
        public void UserControl_ValidateChildren_InvalidValidationConstraints_ThrowsInvalidEnumArgumentException(ValidationConstraints validationConstraints)
        {
            using var control = new UserControl();
            Assert.Throws<InvalidEnumArgumentException>("validationConstraints", () => control.ValidateChildren(validationConstraints));
        }

        [WinFormsFact]
        public void UserControl_WndProc_InvokeMouseHoverWithHandle_Success()
        {
            using var control = new SubUserControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            int callCount = 0;
            control.MouseHover += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            var m = new Message
            {
                Msg = (int)User32.WM.MOUSEHOVER,
                Result = (IntPtr)250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void UserControl_WndProc_InvokeSetFocusWithHandle_Success()
        {
            using var child1 = new Control();
            using var child2 = new Control();
            using var control = new SubUserControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            var m = new Message
            {
                Msg = (int)User32.WM.SETFOCUS,
                Result = (IntPtr)250
            };
            control.WndProc(ref m);
            Assert.Equal((IntPtr)250, m.Result);
            Assert.Same(child1, control.ActiveControl);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void UserControl_WndProc_InvokeSetFocusWithActiveControlWithHandle_Success()
        {
            using var child1 = new Control();
            using var child2 = new Control();
            using var control = new SubUserControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);
            control.ActiveControl = child2;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            var m = new Message
            {
                Msg = (int)User32.WM.SETFOCUS,
                Result = (IntPtr)250
            };
            control.WndProc(ref m);
            Assert.Equal((IntPtr)250, m.Result);
            Assert.Same(child2, control.ActiveControl);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        private class SubControl : Control
        {
            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
        }

        public class SubUserControl : UserControl
        {
            public new const int ScrollStateAutoScrolling = UserControl.ScrollStateAutoScrolling;

            public new const int ScrollStateHScrollVisible = UserControl.ScrollStateHScrollVisible;

            public new const int ScrollStateVScrollVisible = UserControl.ScrollStateVScrollVisible;

            public new const int ScrollStateUserHasScrolled = UserControl.ScrollStateUserHasScrolled;

            public new const int ScrollStateFullDrag = UserControl.ScrollStateFullDrag;

            public new SizeF AutoScaleFactor => base.AutoScaleFactor;

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

            public new bool HScroll
            {
                get => base.HScroll;
                set => base.HScroll = value;
            }

            public new bool VScroll
            {
                get => base.VScroll;
                set => base.VScroll = value;
            }

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetScrollState(int bit) => base.GetScrollState(bit);

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public new void OnCreateControl() => base.OnCreateControl();

            public new void OnLoad(EventArgs e) => base.OnLoad(e);

            public new void OnMouseDown(MouseEventArgs e) => base.OnMouseDown(e);

            public new void OnResize(EventArgs e) => base.OnResize(e);

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);

            public new void WndProc(ref Message m) => base.WndProc(ref m);
        }
    }
}

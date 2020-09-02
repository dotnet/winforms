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
using static Interop.UiaCore;
using static Interop.User32;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class ButtonTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void Button_Ctor_Default()
        {
            using var control = new SubButton();
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoEllipsis);
            Assert.False(control.AutoSize);
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(23, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 75, 23), control.Bounds);
            Assert.False(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Size(75, 23), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 75, 23), control.ClientRectangle);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Disable, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(new Size(75, 23), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(DialogResult.None, control.DialogResult);
            Assert.Equal(new Rectangle(0, 0, 75, 23), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.True(control.DoubleBuffered);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.NotNull(control.FlatAppearance);
            Assert.Same(control.FlatAppearance, control.FlatAppearance);
            Assert.Equal(FlatStyle.Standard, control.FlatStyle);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(23, control.Height);
            Assert.Null(control.Image);
            Assert.Equal(ContentAlignment.MiddleCenter, control.ImageAlign);
            Assert.Equal(-1, control.ImageIndex);
            Assert.Empty(control.ImageKey);
            Assert.Null(control.ImageList);
            Assert.Equal(ImeMode.Disable, control.ImeMode);
            Assert.Equal(ImeMode.Disable, control.ImeModeBase);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsDefault);
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
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.Equal(new Size(75, 23), control.PreferredSize);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.True(control.ResizeRedraw);
            Assert.Equal(75, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(75, 23), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(ContentAlignment.MiddleCenter, control.TextAlign);
            Assert.Equal(TextImageRelation.Overlay, control.TextImageRelation);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.True(control.UseCompatibleTextRendering);
            Assert.True(control.UseMnemonic);
            Assert.True(control.UseVisualStyleBackColor);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.Equal(75, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void Button_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubButton();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("Button", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(23, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x5601000B, createParams.Style);
            Assert.Equal(75, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0x5601000B)]
        [InlineData(false, 0x56010000)]
        public void Button_CreateParams_GetUserPaint_ReturnsExpected(bool userPaint, int expectedStyle)
        {
            using var control = new SubButton();
            control.SetStyle(ControlStyles.UserPaint, userPaint);

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("Button", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(23, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(75, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(FlatStyle.Flat, true, 0x5601000B)]
        [InlineData(FlatStyle.Flat, false, 0x5601000B)]
        [InlineData(FlatStyle.Popup, true, 0x5601000B)]
        [InlineData(FlatStyle.Popup, false, 0x5601000B)]
        [InlineData(FlatStyle.Standard, true, 0x5601000B)]
        [InlineData(FlatStyle.Standard, false, 0x5601000B)]
        [InlineData(FlatStyle.System, true, 0x56012F01)]
        [InlineData(FlatStyle.System, false, 0x56012F00)]
        public void Button_CreateParams_GetIsDefault_ReturnsExpected(FlatStyle flatStyle, bool isDefault, int expectedStyle)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle,
                IsDefault = isDefault
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("Button", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(23, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(75, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> CreateParams_GetIsDefault_TestData()
        {
            foreach (FlatStyle flatStyle in new FlatStyle[] { FlatStyle.Flat, FlatStyle.Popup, FlatStyle.Standard })
            {
                foreach (ContentAlignment textAlign in Enum.GetValues(typeof(ContentAlignment)))
                {
                    yield return new object[] { flatStyle, RightToLeft.Yes, textAlign, 0x5601000B, 0x7000 };
                    yield return new object[] { flatStyle, RightToLeft.No, textAlign, 0x5601000B, 0 };
                    yield return new object[] { flatStyle, RightToLeft.Inherit, textAlign, 0x5601000B, 0 };
                }
            }

            yield return new object[] { FlatStyle.System, RightToLeft.Yes, ContentAlignment.BottomLeft, 0x56012A00, 0x6000 };
            yield return new object[] { FlatStyle.System, RightToLeft.Yes, ContentAlignment.BottomCenter, 0x56012B00, 0x6000 };
            yield return new object[] { FlatStyle.System, RightToLeft.Yes, ContentAlignment.BottomRight, 0x56012900, 0x6000 };
            yield return new object[] { FlatStyle.System, RightToLeft.Yes, ContentAlignment.MiddleLeft, 0x56012E00, 0x6000 };
            yield return new object[] { FlatStyle.System, RightToLeft.Yes, ContentAlignment.MiddleCenter, 0x56012F00, 0x6000 };
            yield return new object[] { FlatStyle.System, RightToLeft.Yes, ContentAlignment.MiddleRight, 0x56012D00, 0x6000 };
            yield return new object[] { FlatStyle.System, RightToLeft.Yes, ContentAlignment.TopLeft, 0x56012600, 0x6000 };
            yield return new object[] { FlatStyle.System, RightToLeft.Yes, ContentAlignment.TopCenter, 0x56012700, 0x6000 };
            yield return new object[] { FlatStyle.System, RightToLeft.Yes, ContentAlignment.TopRight, 0x56012500, 0x6000 };

            yield return new object[] { FlatStyle.System, RightToLeft.No, ContentAlignment.BottomLeft, 0x56012900, 0 };
            yield return new object[] { FlatStyle.System, RightToLeft.No, ContentAlignment.BottomCenter, 0x56012B00, 0 };
            yield return new object[] { FlatStyle.System, RightToLeft.No, ContentAlignment.BottomRight, 0x56012A00, 0 };
            yield return new object[] { FlatStyle.System, RightToLeft.No, ContentAlignment.MiddleLeft, 0x56012D00, 0 };
            yield return new object[] { FlatStyle.System, RightToLeft.No, ContentAlignment.MiddleCenter, 0x56012F00, 0 };
            yield return new object[] { FlatStyle.System, RightToLeft.No, ContentAlignment.MiddleRight, 0x56012E00, 0 };
            yield return new object[] { FlatStyle.System, RightToLeft.No, ContentAlignment.TopLeft, 0x56012500, 0 };
            yield return new object[] { FlatStyle.System, RightToLeft.No, ContentAlignment.TopCenter, 0x56012700, 0 };
            yield return new object[] { FlatStyle.System, RightToLeft.No, ContentAlignment.TopRight, 0x56012600, 0 };

            yield return new object[] { FlatStyle.System, RightToLeft.Inherit, ContentAlignment.BottomLeft, 0x56012900, 0 };
            yield return new object[] { FlatStyle.System, RightToLeft.Inherit, ContentAlignment.BottomCenter, 0x56012B00, 0 };
            yield return new object[] { FlatStyle.System, RightToLeft.Inherit, ContentAlignment.BottomRight, 0x56012A00, 0 };
            yield return new object[] { FlatStyle.System, RightToLeft.Inherit, ContentAlignment.MiddleLeft, 0x56012D00, 0 };
            yield return new object[] { FlatStyle.System, RightToLeft.Inherit, ContentAlignment.MiddleCenter, 0x56012F00, 0 };
            yield return new object[] { FlatStyle.System, RightToLeft.Inherit, ContentAlignment.MiddleRight, 0x56012E00, 0 };
            yield return new object[] { FlatStyle.System, RightToLeft.Inherit, ContentAlignment.TopLeft, 0x56012500, 0 };
            yield return new object[] { FlatStyle.System, RightToLeft.Inherit, ContentAlignment.TopCenter, 0x56012700, 0 };
            yield return new object[] { FlatStyle.System, RightToLeft.Inherit, ContentAlignment.TopRight, 0x56012600, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(CreateParams_GetIsDefault_TestData))]
        public void Button_CreateParams_GetTextAlign_ReturnsExpected(FlatStyle flatStyle, RightToLeft rightToLeft, ContentAlignment textAlign, int expectedStyle, int expectedExStyle)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle,
                RightToLeft = rightToLeft,
                TextAlign = textAlign
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("Button", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(expectedExStyle, createParams.ExStyle);
            Assert.Equal(23, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(75, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoSizeMode))]
        public void Button_AutoSizeMode_Set_GetReturnsExpected(AutoSizeMode value)
        {
            using var control = new SubButton();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.AutoSizeMode = value;
            Assert.Equal(value, control.AutoSizeMode);
            Assert.Equal(value, control.GetAutoSizeMode());
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);

            // Set same.
            control.AutoSizeMode = value;
            Assert.Equal(value, control.AutoSizeMode);
            Assert.Equal(value, control.GetAutoSizeMode());
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);
        }

        [WinFormsTheory]
        [InlineData(AutoSizeMode.GrowAndShrink, 1)]
        [InlineData(AutoSizeMode.GrowOnly, 0)]
        public void Button_AutoSizeMode_SetWithParent_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new SubButton
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
        public void Button_AutoSizeMode_SetWithCustomLayoutEngineParent_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
        {
            using var parent = new CustomLayoutEngineControl();
            using var control = new SubButton
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
                _layoutEngine = base.LayoutEngine;
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
        public void Button_AutoSizeMode_SetWithHandle_GetReturnsExpected(AutoSizeMode value)
        {
            using var control = new SubButton();
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
        public void Button_AutoSizeMode_SetWithParentWithHandle_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new SubButton
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
        public void Button_AutoSizeMode_SetWithCustomLayoutEngineParentWithHandle_GetReturnsExpected(AutoSizeMode value, int expectedLayoutCallCount)
        {
            using var parent = new CustomLayoutEngineControl();
            using var control = new SubButton
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

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsFact(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        public void Button_AutoSize_SetCachedPreferredSize_DoesNotInvalidate()
        {
            using var control = new Button
            {
                FlatStyle = FlatStyle.System,
                AutoSizeMode = AutoSizeMode.GrowOnly,
                Text = "text"
            };
            Assert.Equal(new Size(75, 24), control.GetPreferredSize(Size.Empty));

            // Set font.
            Size newSize = TextRenderer.MeasureText(control.Text, control.Font);
            control.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Assert.Equal(new Size(75, 24), control.GetPreferredSize(Size.Empty));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AutoSizeMode))]
        public void Button_AutoSizeMode_SetInvalid_ThrowsInvalidEnumArgumentException(AutoSizeMode value)
        {
            using var control = new Button();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.AutoSizeMode = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DialogResult))]
        public void Button_DialogResult_Set_GetReturnsExpected(DialogResult value)
        {
            using var control = new Button
            {
                DialogResult = value
            };
            Assert.Equal(value, control.DialogResult);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.DialogResult = value;
            Assert.Equal(value, control.DialogResult);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DialogResult))]
        public void Button_DialogResult_SetWithHandle_GetReturnsExpected(DialogResult value)
        {
            using var control = new Button();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.DialogResult = value;
            Assert.Equal(value, control.DialogResult);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.DialogResult = value;
            Assert.Equal(value, control.DialogResult);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DialogResult))]
        public void Button_DialogResult_SetInvalidValue_ThrowsInvalidEnumArgumentException(DialogResult value)
        {
            using var control = new Button();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.DialogResult = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetFontTheoryData))]
        public void Button_Font_Set_GetReturnsExpected(Font value)
        {
            using var control = new SubButton
            {
                Font = value
            };
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.False(control.IsHandleCreated);
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsFact(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        public void Button_Font_SetCachedPreferredSize_Invalidates()
        {
            using var font = new Font("Arial", 100f);
            using var control = new Button
            {
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlatStyle = FlatStyle.System,
                Text = "text"
            };
            Size oldSize = TextRenderer.MeasureText(control.Text, control.Font);
            Assert.Equal(oldSize + new Size(14, 9), control.GetPreferredSize(Size.Empty));

            // Set font.
            Size newSize = TextRenderer.MeasureText(control.Text, font);
            control.Font = font;
            Assert.Equal(newSize + new Size(14, 9), control.GetPreferredSize(Size.Empty));
        }

        [WinFormsFact]
        public void Button_Font_SetWithHandler_CallsFontChanged()
        {
            using var control = new Button();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.FontChanged += handler;

            // Set different.
            using var font1 = new Font("Arial", 8.25f);
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Equal(1, callCount);

            // Set same.
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Equal(1, callCount);

            // Set different.
            using var font2 = SystemFonts.DialogFont;
            control.Font = font2;
            Assert.Same(font2, control.Font);
            Assert.Equal(2, callCount);

            // Set null.
            control.Font = null;
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.FontChanged -= handler;
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Equal(3, callCount);
        }

        public static IEnumerable<object[]> Text_Set_TestData()
        {
            foreach (bool autoSize in new bool[] { true, false })
            {
                yield return new object[] { autoSize, null, string.Empty };
                yield return new object[] { autoSize, string.Empty, string.Empty };
                yield return new object[] { autoSize, "text", "text" };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Text_Set_TestData))]
        public void Button_Text_Set_GetReturnsExpected(bool autoSize, string value, string expected)
        {
            using var control = new SubButton
            {
                AutoSize = autoSize
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Text_SetWithParent_TestData()
        {
            yield return new object[] { true, null, string.Empty, 0 };
            yield return new object[] { true, string.Empty, string.Empty, 0 };
            yield return new object[] { true, "text", "text", 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Text_SetWithParent_TestData))]
        public void Button_Text_SetWithParent_GetReturnsExpected(bool autoSize, string value, string expected, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new Button
            {
                AutoSize = autoSize,
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Text", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                control.Text = value;
                Assert.Equal(expected, control.Text);
                Assert.Equal(expectedParentLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);

                // Set same.
                control.Text = value;
                Assert.Equal(expected, control.Text);
                Assert.Equal(expectedParentLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        public static IEnumerable<object[]> Text_SetWithHandle_TestData()
        {
            foreach (bool autoSize in new bool[] { true, false })
            {
                yield return new object[] { autoSize, null, string.Empty, 0 };
                yield return new object[] { autoSize, string.Empty, string.Empty, 0 };
                yield return new object[] { autoSize, "text", "text", 1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Text_SetWithHandle_TestData))]
        public void Button_Text_SetWithHandle_GetReturnsExpected(bool autoSize, string value, string expected, int expectedInvalidatedCallCount)
        {
            using var control = new SubButton
            {
                AutoSize = autoSize
            };
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
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Text_SetWithParentWithHandle_TestData()
        {
            yield return new object[] { true, null, string.Empty, 0 };
            yield return new object[] { true, string.Empty, string.Empty, 0 };
            yield return new object[] { true, "text", "text", 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Text_SetWithParentWithHandle_TestData))]
        public void Button_Text_SetWithParentWithHandle_GetReturnsExpected(bool autoSize, string value, string expected, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new Button
            {
                AutoSize = autoSize,
                Parent = parent
            };
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Text", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                control.Text = value;
                Assert.Equal(expected, control.Text);
                Assert.Equal(expectedParentLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(expectedParentLayoutCallCount * 2, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);

                // Set same.
                control.Text = value;
                Assert.Equal(expected, control.Text);
                Assert.Equal(expectedParentLayoutCallCount, layoutCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(expectedParentLayoutCallCount * 2, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsFact(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        public void Button_Text_SetCachedPreferredSize_Invalidates()
        {
            using var control = new Button
            {
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlatStyle = FlatStyle.System,
                Text = "text"
            };
            Size oldSize = TextRenderer.MeasureText(control.Text, control.Font);
            Assert.Equal(oldSize + new Size(14, 9), control.GetPreferredSize(Size.Empty));

            // Set font.
            Size newSize = TextRenderer.MeasureText("NewText", control.Font);
            control.Text = "NextText";
            Assert.Equal(newSize + new Size(14, 9), control.GetPreferredSize(Size.Empty));
        }

        [WinFormsFact]
        public void Button_Text_SetWithHandler_CallsTextChanged()
        {
            using var control = new SubButton();
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(FlatStyle))]
        public void Button_CreateAccessibilityInstance_Invoke_ReturnsExpected_IfHandleIsNotCreated(FlatStyle flatStyle)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle
            };

            Assert.False(control.IsHandleCreated);
            Button.ButtonBaseAccessibleObject instance = Assert.IsAssignableFrom<Button.ButtonBaseAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.NotNull(instance);
            Assert.Same(control, instance.Owner);
            Assert.Equal(AccessibleStates.None, instance.State);
            Assert.Equal(AccessibleRole.None, instance.Role);
            Assert.NotSame(control.CreateAccessibilityInstance(), instance);
            Assert.NotSame(control.AccessibilityObject, instance);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(FlatStyle))]
        public void Button_CreateAccessibilityInstance_Invoke_ReturnsExpected_IfHandleIsCreated(FlatStyle flatStyle)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle
            };

            control.CreateControl();
            Assert.True(control.IsHandleCreated);
            Button.ButtonBaseAccessibleObject instance = Assert.IsAssignableFrom<Button.ButtonBaseAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.NotNull(instance);
            Assert.Same(control, instance.Owner);
            Assert.Equal(AccessibleStates.Focusable, instance.State);
            Assert.Equal(AccessibleRole.PushButton, instance.Role);
            Assert.NotSame(control.CreateAccessibilityInstance(), instance);
            Assert.NotSame(control.AccessibilityObject, instance);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, FlatStyle.Flat, AccessibleStates.Pressed | AccessibleStates.Focusable, AccessibleRole.PushButton)]
        [InlineData(true, FlatStyle.Popup, AccessibleStates.Pressed | AccessibleStates.Focusable, AccessibleRole.PushButton)]
        [InlineData(true, FlatStyle.Standard, AccessibleStates.Pressed | AccessibleStates.Focusable, AccessibleRole.PushButton)]
        [InlineData(true, FlatStyle.System, AccessibleStates.Focusable, AccessibleRole.PushButton)]
        [InlineData(false, FlatStyle.Flat, AccessibleStates.None, AccessibleRole.None)]
        [InlineData(false, FlatStyle.Popup, AccessibleStates.None, AccessibleRole.None)]
        [InlineData(false, FlatStyle.Standard, AccessibleStates.None, AccessibleRole.None)]
        [InlineData(false, FlatStyle.System, AccessibleStates.None, AccessibleRole.None)]
        public void Button_CreateAccessibilityInstance_InvokeMouseDown_ReturnsExpected(bool createControl, FlatStyle flatStyle, AccessibleStates expectedState, AccessibleRole expectedRole)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle
            };

            if (createControl)
            {
                control.CreateControl();
            }

            Assert.Equal(createControl, control.IsHandleCreated);

            control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            Button.ButtonBaseAccessibleObject instance = Assert.IsAssignableFrom<Button.ButtonBaseAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.NotNull(instance);
            Assert.Same(control, instance.Owner);
            Assert.Equal(expectedState, instance.State);
            Assert.Equal(expectedRole, instance.Role);
            Assert.NotSame(control.CreateAccessibilityInstance(), instance);
            Assert.NotSame(control.AccessibilityObject, instance);
            Assert.Equal(createControl, control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(FlatStyle))]
        public void Button_CreateAccessibilityInstance_InvokeWithCustomRole_ReturnsExpected_IfHandleIsCreated(FlatStyle flatStyle)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle,
                AccessibleRole = AccessibleRole.HelpBalloon
            };

            control.CreateControl();
            Assert.True(control.IsHandleCreated);
            Button.ButtonBaseAccessibleObject instance = Assert.IsAssignableFrom<Button.ButtonBaseAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.NotNull(instance);
            Assert.Same(control, instance.Owner);
            Assert.Equal(AccessibleStates.Focusable, instance.State);
            Assert.Equal(AccessibleRole.HelpBalloon, instance.Role);
            Assert.NotSame(control.CreateAccessibilityInstance(), instance);
            Assert.NotSame(control.AccessibilityObject, instance);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(FlatStyle))]
        public void Button_CreateAccessibilityInstance_InvokeWithCustomRole_ReturnsExpected_IfHandleIsNotCreated(FlatStyle flatStyle)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle,
                AccessibleRole = AccessibleRole.HelpBalloon
            };

            Assert.False(control.IsHandleCreated);
            Button.ButtonBaseAccessibleObject instance = Assert.IsAssignableFrom<Button.ButtonBaseAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.NotNull(instance);
            Assert.Same(control, instance.Owner);
            Assert.Equal(AccessibleStates.None, instance.State);
            Assert.Equal(AccessibleRole.HelpBalloon, instance.Role);
            Assert.NotSame(control.CreateAccessibilityInstance(), instance);
            Assert.NotSame(control.AccessibilityObject, instance);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(FlatStyle))]
        public void Button_CreateAccessibilityInstance_InvokeDoDefaultAction_CallsOnClick_IfHandleIsCreated(FlatStyle flatStyle)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle
            };

            control.CreateControl();
            int callCount = 0;
            control.Click += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            Button.ButtonBaseAccessibleObject instance = Assert.IsAssignableFrom<Button.ButtonBaseAccessibleObject>(control.CreateAccessibilityInstance());
            instance.DoDefaultAction();
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(FlatStyle))]
        public void Button_CreateAccessibilityInstance_InvokeDoDefaultAction_CallsOnClick_IfHandleIsNotCreated(FlatStyle flatStyle)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle
            };

            int callCount = 0;
            control.Click += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            Button.ButtonBaseAccessibleObject instance = Assert.IsAssignableFrom<Button.ButtonBaseAccessibleObject>(control.CreateAccessibilityInstance());
            instance.DoDefaultAction();
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void Button_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubButton();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        public static IEnumerable<object[]> GetPreferredSize_System_TestData()
        {
            yield return new object[] { AutoSizeMode.GrowAndShrink, Size.Empty, new Size(14, 9) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(-1, -2), new Size(14, 9) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(-2, -1), new Size(14, 9) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(-1, -1), new Size(14, 9) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(1, 2), new Size(14, 9) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(2, 1), new Size(14, 9) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(1, 1), new Size(14, 9) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(10, 20), new Size(14, 9) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(30, 40), new Size(14, 9) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(30, int.MaxValue), new Size(14, 9) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, 40), new Size(14, 9) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, int.MaxValue), new Size(14, 9) };

            yield return new object[] { AutoSizeMode.GrowOnly, Size.Empty, new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(-1, -2), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(-2, -1), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(-1, -1), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(1, 2), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(2, 1), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(1, 1), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(10, 20), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(30, 40), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(30, int.MaxValue), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(int.MaxValue, 40), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(int.MaxValue, int.MaxValue), new Size(75, 23) };
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsTheory(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        [MemberData(nameof(GetPreferredSize_System_TestData))]
        public void Button_GetPreferredSize_InvokeSystem_ReturnsExpected(AutoSizeMode autoSizeMode, Size proposedSize, Size expected)
        {
            using var control = new SubButton
            {
                FlatStyle = FlatStyle.System,
                AutoSizeMode = autoSizeMode
            };
            Size size = control.GetPreferredSize(proposedSize);
            Assert.Equal(expected, size);
            Assert.False(control.IsHandleCreated);

            // Call again.
            size = control.GetPreferredSize(proposedSize);
            Assert.Equal(expected, size);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> GetPreferredSize_SystemWithPadding_TestData()
        {
            yield return new object[] { AutoSizeMode.GrowAndShrink, Size.Empty, new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(-1, -2), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(-2, -1), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(-1, -1), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(1, 2), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(2, 1), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(1, 1), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(10, 20), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(30, 40), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(30, int.MaxValue), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, 40), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, int.MaxValue), new Size(18, 15) };

            yield return new object[] { AutoSizeMode.GrowOnly, Size.Empty, new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(-1, -2), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(-2, -1), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(-1, -1), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(1, 2), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(2, 1), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(1, 1), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(10, 20), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(30, 40), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(30, int.MaxValue), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(int.MaxValue, 40), new Size(75, 23) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(int.MaxValue, int.MaxValue), new Size(75, 23) };
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsTheory(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        [MemberData(nameof(GetPreferredSize_SystemWithPadding_TestData))]
        public void Button_GetPreferredSize_InvokeSystemWithPadding_ReturnsExpected(AutoSizeMode autoSizeMode, Size proposedSize, Size expected)
        {
            using var control = new SubButton
            {
                FlatStyle = FlatStyle.System,
                AutoSizeMode = autoSizeMode,
                Padding = new Padding(1, 2, 3, 4)
            };
            Size size = control.GetPreferredSize(proposedSize);
            Assert.Equal(expected, size);
            Assert.False(control.IsHandleCreated);

            // Call again.
            size = control.GetPreferredSize(proposedSize);
            Assert.Equal(expected, size);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> GetPreferredSize_SystemWithPaddingSmallSize_TestData()
        {
            yield return new object[] { AutoSizeMode.GrowAndShrink, Size.Empty, new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(-1, -2), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(-2, -1), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(-1, -1), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(1, 2), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(2, 1), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(1, 1), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(10, 20), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(30, 40), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(30, int.MaxValue), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, 40), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, int.MaxValue), new Size(18, 15) };

            yield return new object[] { AutoSizeMode.GrowOnly, Size.Empty, new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(-1, -2), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(-2, -1), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(-1, -1), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(1, 2), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(2, 1), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(1, 1), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(10, 20), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(30, 40), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(30, int.MaxValue), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(int.MaxValue, 40), new Size(18, 15) };
            yield return new object[] { AutoSizeMode.GrowOnly, new Size(int.MaxValue, int.MaxValue), new Size(18, 15) };
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsTheory(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        [MemberData(nameof(GetPreferredSize_SystemWithPaddingSmallSize_TestData))]
        public void Button_GetPreferredSize_InvokeSystemWithPaddingSmallSize_ReturnsExpected(AutoSizeMode autoSizeMode, Size proposedSize, Size expected)
        {
            using var control = new SubButton
            {
                FlatStyle = FlatStyle.System,
                AutoSizeMode = autoSizeMode,
                Size = new Size(14, 9),
                Padding = new Padding(1, 2, 3, 4)
            };
            Size size = control.GetPreferredSize(proposedSize);
            Assert.Equal(expected, size);
            Assert.False(control.IsHandleCreated);

            // Call again.
            size = control.GetPreferredSize(proposedSize);
            Assert.Equal(expected, size);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> GetPreferredSize_SystemWithText_TestData()
        {
            yield return new object[] { Size.Empty };
            yield return new object[] { new Size(-1, -2) };
            yield return new object[] { new Size(-2, -1) };
            yield return new object[] { new Size(-1, -1) };
            yield return new object[] { new Size(1, 2) };
            yield return new object[] { new Size(2, 1) };
            yield return new object[] { new Size(1, 1) };
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsTheory(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        [MemberData(nameof(GetPreferredSize_SystemWithText_TestData))]
        public void Button_GetPreferredSize_InvokeSystemGrowOnlyWithText_ReturnsExpected(Size proposedSize)
        {
            using var control = new SubButton
            {
                FlatStyle = FlatStyle.System,
                AutoSizeMode = AutoSizeMode.GrowOnly,
                Padding = new Padding(1, 2, 3, 4),
                Text = "Text"
            };
            Size minSize = TextRenderer.MeasureText(control.Text, control.Font);

            Size size = control.GetPreferredSize(proposedSize);
            Assert.Equal(new Size(75, minSize.Height + 6 + 9), size);
            Assert.False(control.IsHandleCreated);

            // Call again.
            size = control.GetPreferredSize(proposedSize);
            Assert.Equal(new Size(75, minSize.Height + 6 + 9), size);
            Assert.False(control.IsHandleCreated);
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsTheory(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        [MemberData(nameof(GetPreferredSize_SystemWithText_TestData))]
        public void Button_GetPreferredSize_InvokeSystemGrowAndShrinkWithText_ReturnsExpected(Size proposedSize)
        {
            using var control = new SubButton
            {
                FlatStyle = FlatStyle.System,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(1, 2, 3, 4),
                Text = "Text"
            };
            Size minSize = TextRenderer.MeasureText(control.Text, control.Font);

            Size size = control.GetPreferredSize(proposedSize);
            Assert.Equal(minSize + new Size(14, 9) + new Size(4, 6), size);
            Assert.False(control.IsHandleCreated);

            // Call again.
            size = control.GetPreferredSize(proposedSize);
            Assert.Equal(minSize + new Size(14, 9) + new Size(4, 6), size);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> GetPreferredSize_Flat_TestData()
        {
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, Size.Empty, new Size(8, 8) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(-1, -2), new Size(8, 8) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(-2, -1), new Size(8, 8) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(-1, -1), new Size(8, 8) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(1, 2), new Size(8, 8) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(2, 1), new Size(8, 8) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(1, 1), new Size(8, 8) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(10, 20), new Size(8, 8) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(30, 40), new Size(8, 8) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(30, int.MaxValue), new Size(8, 8) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, 40), new Size(8, 8) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, int.MaxValue), new Size(8, 8) };

            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, Size.Empty, new Size(6, 6) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(-1, -2), new Size(6, 6) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(-2, -1), new Size(6, 6) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(-1, -1), new Size(6, 6) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(1, 2), new Size(6, 6) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(2, 1), new Size(6, 6) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(1, 1), new Size(6, 6) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(10, 20), new Size(6, 6) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(30, 40), new Size(6, 6) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(30, int.MaxValue), new Size(6, 6) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, 40), new Size(6, 6) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, int.MaxValue), new Size(6, 6) };

            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, Size.Empty, new Size(6, 6) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(-1, -2), new Size(6, 6) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(-2, -1), new Size(6, 6) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(-1, -1), new Size(6, 6) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(1, 2), new Size(6, 6) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(2, 1), new Size(6, 6) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(1, 1), new Size(6, 6) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(10, 20), new Size(6, 6) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(30, 40), new Size(6, 6) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(30, int.MaxValue), new Size(6, 6) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, 40), new Size(6, 6) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, int.MaxValue), new Size(6, 6) };

            foreach (FlatStyle flatStyle in new FlatStyle[] { FlatStyle.Flat, FlatStyle.Popup, FlatStyle.Standard })
            {
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, Size.Empty, new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(-1, -2), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(-2, -1), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(-1, -1), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(1, 2), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(2, 1), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(1, 1), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(10, 20), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(30, 40), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(30, int.MaxValue), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(int.MaxValue, 40), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(int.MaxValue, int.MaxValue), new Size(75, 23) };
            }
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsTheory(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        [MemberData(nameof(GetPreferredSize_Flat_TestData))]
        public void Button_GetPreferredSize_InvokeFlat_ReturnsExpected(FlatStyle flatStyle, AutoSizeMode autoSizeMode, Size proposedSize, Size expected)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle,
                AutoSizeMode = autoSizeMode
            };
            Size size = control.GetPreferredSize(proposedSize);
            Assert.Equal(expected, size);
            Assert.False(control.IsHandleCreated);

            // Call again.
            size = control.GetPreferredSize(proposedSize);
            Assert.Equal(expected, size);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> GetPreferredSize_FlatWithPadding_TestData()
        {
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, Size.Empty, new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(-1, -2), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(-2, -1), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(-1, -1), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(1, 2), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(2, 1), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(1, 1), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(10, 20), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(30, 40), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(30, int.MaxValue), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, 40), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, int.MaxValue), new Size(12, 14) };

            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, Size.Empty, new Size(10, 12) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(-1, -2), new Size(10, 12) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(-2, -1), new Size(10, 12) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(-1, -1), new Size(10, 12) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(1, 2), new Size(10, 12) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(2, 1), new Size(10, 12) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(1, 1), new Size(10, 12) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(10, 20), new Size(10, 12) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(30, 40), new Size(10, 12) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(30, int.MaxValue), new Size(10, 12) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, 40), new Size(10, 12) };
            yield return new object[] { FlatStyle.Popup, AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, int.MaxValue), new Size(10, 12) };

            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, Size.Empty, new Size(10, 12) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(-1, -2), new Size(10, 12) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(-2, -1), new Size(10, 12) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(-1, -1), new Size(10, 12) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(1, 2), new Size(10, 12) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(2, 1), new Size(10, 12) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(1, 1), new Size(10, 12) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(10, 20), new Size(10, 12) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(30, 40), new Size(10, 12) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(30, int.MaxValue), new Size(10, 12) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, 40), new Size(10, 12) };
            yield return new object[] { FlatStyle.Standard, AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, int.MaxValue), new Size(10, 12) };

            foreach (FlatStyle flatStyle in new FlatStyle[] { FlatStyle.Flat, FlatStyle.Popup, FlatStyle.Standard })
            {
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, Size.Empty, new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(-1, -2), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(-2, -1), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(-1, -1), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(1, 2), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(2, 1), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(1, 1), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(10, 20), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(30, 40), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(30, int.MaxValue), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(int.MaxValue, 40), new Size(75, 23) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(int.MaxValue, int.MaxValue), new Size(75, 23) };
            }
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsTheory(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        [MemberData(nameof(GetPreferredSize_FlatWithPadding_TestData))]
        public void Button_GetPreferredSize_InvokeFlatWithPadding_ReturnsExpected(FlatStyle flatStyle, AutoSizeMode autoSizeMode, Size proposedSize, Size expected)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle,
                AutoSizeMode = autoSizeMode,
                Padding = new Padding(1, 2, 3, 4)
            };
            Size size = control.GetPreferredSize(proposedSize);
            Assert.Equal(expected, size);
            Assert.False(control.IsHandleCreated);

            // Call again.
            size = control.GetPreferredSize(proposedSize);
            Assert.Equal(expected, size);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> GetPreferredSize_FlatWithPaddingSmallSize_TestData()
        {
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, Size.Empty, new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(-1, -2), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(-2, -1), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(-1, -1), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(1, 2), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(2, 1), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(1, 1), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(10, 20), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(30, 40), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(30, int.MaxValue), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, 40), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, int.MaxValue), new Size(12, 14) };

            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowOnly, Size.Empty, new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowOnly, new Size(-1, -2), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowOnly, new Size(-2, -1), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowOnly, new Size(-1, -1), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowOnly, new Size(1, 2), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowOnly, new Size(2, 1), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowOnly, new Size(1, 1), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowOnly, new Size(10, 20), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowOnly, new Size(30, 40), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowOnly, new Size(30, int.MaxValue), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowOnly, new Size(int.MaxValue, 40), new Size(12, 14) };
            yield return new object[] { FlatStyle.Flat, AutoSizeMode.GrowOnly, new Size(int.MaxValue, int.MaxValue), new Size(12, 14) };

            foreach (FlatStyle flatStyle in new FlatStyle[] { FlatStyle.Popup, FlatStyle.Standard })
            {
                yield return new object[] { flatStyle, AutoSizeMode.GrowAndShrink, Size.Empty, new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowAndShrink, new Size(-1, -2), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowAndShrink, new Size(-2, -1), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowAndShrink, new Size(-1, -1), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowAndShrink, new Size(1, 2), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowAndShrink, new Size(2, 1), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowAndShrink, new Size(1, 1), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowAndShrink, new Size(10, 20), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowAndShrink, new Size(30, 40), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowAndShrink, new Size(30, int.MaxValue), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, 40), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowAndShrink, new Size(int.MaxValue, int.MaxValue), new Size(10, 12) };

                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, Size.Empty, new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(-1, -2), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(-2, -1), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(-1, -1), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(1, 2), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(2, 1), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(1, 1), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(10, 20), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(30, 40), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(30, int.MaxValue), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(int.MaxValue, 40), new Size(10, 12) };
                yield return new object[] { flatStyle, AutoSizeMode.GrowOnly, new Size(int.MaxValue, int.MaxValue), new Size(10, 12) };
            }
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsTheory(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        [MemberData(nameof(GetPreferredSize_FlatWithPaddingSmallSize_TestData))]
        public void Button_GetPreferredSize_InvokeFlatWithPaddingSmallSize_ReturnsExpected(FlatStyle flatStyle, AutoSizeMode autoSizeMode, Size proposedSize, Size expected)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle,
                AutoSizeMode = autoSizeMode,
                Size = new Size(6, 6),
                Padding = new Padding(1, 2, 3, 4)
            };
            Size size = control.GetPreferredSize(proposedSize);
            Assert.Equal(expected, size);
            Assert.False(control.IsHandleCreated);

            // Call again.
            size = control.GetPreferredSize(proposedSize);
            Assert.Equal(expected, size);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> GetPreferredSize_FlatWithText_TestData()
        {
            yield return new object[] { Size.Empty };
            yield return new object[] { new Size(-1, -2) };
            yield return new object[] { new Size(-2, -1) };
            yield return new object[] { new Size(-1, -1) };
            yield return new object[] { new Size(1, 2) };
            yield return new object[] { new Size(2, 1) };
            yield return new object[] { new Size(1, 1) };
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsTheory(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        [MemberData(nameof(GetPreferredSize_FlatWithText_TestData))]
        public void Button_GetPreferredSize_InvokeFlatGrowOnlyWithText_ReturnsExpected(Size proposedSize)
        {
            using var control = new SubButton
            {
                FlatStyle = FlatStyle.Flat,
                AutoSizeMode = AutoSizeMode.GrowOnly,
                Padding = new Padding(1, 2, 3, 4),
                Text = "Text"
            };
            Size minSize = TextRenderer.MeasureText(control.Text, control.Font);

            Size size = control.GetPreferredSize(proposedSize);
            Assert.Equal(new Size(75, minSize.Height + 13 + 8), size);
            Assert.False(control.IsHandleCreated);

            // Call again.
            size = control.GetPreferredSize(proposedSize);
            Assert.Equal(new Size(75, minSize.Height + 13 + 8), size);
            Assert.False(control.IsHandleCreated);
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsTheory(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        [MemberData(nameof(GetPreferredSize_FlatWithText_TestData))]
        public void Button_GetPreferredSize_InvokeFlatGrowAndShrinkWithText_ReturnsExpected(Size proposedSize)
        {
            using var control = new SubButton
            {
                FlatStyle = FlatStyle.Flat,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(1, 2, 3, 4),
                Text = "Text"
            };
            Size minSize = TextRenderer.MeasureText(control.Text, control.Font);

            Size size = control.GetPreferredSize(proposedSize);
            Assert.Equal(minSize + new Size(11, 15) + new Size(4, 6), size);
            Assert.False(control.IsHandleCreated);

            // Call again.
            size = control.GetPreferredSize(proposedSize);
            Assert.Equal(minSize + new Size(11, 15) + new Size(4, 6), size);
            Assert.False(control.IsHandleCreated);
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsTheory(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        [MemberData(nameof(GetPreferredSize_FlatWithText_TestData))]
        public void Button_GetPreferredSize_InvokePopupGrowOnlyWithText_ReturnsExpected(Size proposedSize)
        {
            using var control = new SubButton
            {
                FlatStyle = FlatStyle.Popup,
                AutoSizeMode = AutoSizeMode.GrowOnly,
                Padding = new Padding(1, 2, 3, 4),
                Text = "Text"
            };
            Size minSize = TextRenderer.MeasureText(control.Text, control.Font);

            Size size = control.GetPreferredSize(proposedSize);
            Assert.Equal(new Size(75, minSize.Height + 13 + 6), size);
            Assert.False(control.IsHandleCreated);

            // Call again.
            size = control.GetPreferredSize(proposedSize);
            Assert.Equal(new Size(75, minSize.Height + 13 + 6), size);
            Assert.False(control.IsHandleCreated);
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsTheory(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        [MemberData(nameof(GetPreferredSize_FlatWithText_TestData))]
        public void Button_GetPreferredSize_InvokePopupGrowAndShrinkWithText_ReturnsExpected(Size proposedSize)
        {
            using var control = new SubButton
            {
                FlatStyle = FlatStyle.Popup,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(1, 2, 3, 4),
                Text = "Text"
            };
            Size minSize = TextRenderer.MeasureText(control.Text, control.Font);

            Size size = control.GetPreferredSize(proposedSize);
            Assert.Equal(minSize + new Size(9, 13) + new Size(4, 6), size);
            Assert.False(control.IsHandleCreated);

            // Call again.
            size = control.GetPreferredSize(proposedSize);
            Assert.Equal(minSize + new Size(9, 13) + new Size(4, 6), size);
            Assert.False(control.IsHandleCreated);
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsTheory(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        [MemberData(nameof(GetPreferredSize_FlatWithText_TestData))]
        public void Button_GetPreferredSize_InvokeStandardGrowOnlyWithText_ReturnsExpected(Size proposedSize)
        {
            using var control = new SubButton
            {
                FlatStyle = FlatStyle.Standard,
                AutoSizeMode = AutoSizeMode.GrowOnly,
                Padding = new Padding(1, 2, 3, 4),
                Text = "Text"
            };
            Size minSize = TextRenderer.MeasureText(control.Text, control.Font);

            Size size = control.GetPreferredSize(proposedSize);
            Assert.Equal(new Size(75, minSize.Height + 13 + 6), size);
            Assert.False(control.IsHandleCreated);

            // Call again.
            size = control.GetPreferredSize(proposedSize);
            Assert.Equal(new Size(75, minSize.Height + 13 + 6), size);
            Assert.False(control.IsHandleCreated);
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsTheory(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        [MemberData(nameof(GetPreferredSize_FlatWithText_TestData))]
        public void Button_GetPreferredSize_InvokeStandardGrowAndShrinkWithText_ReturnsExpected(Size proposedSize)
        {
            using var control = new SubButton
            {
                FlatStyle = FlatStyle.Standard,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(1, 2, 3, 4),
                Text = "Text"
            };
            Size minSize = TextRenderer.MeasureText(control.Text, control.Font);

            Size size = control.GetPreferredSize(proposedSize);
            Assert.Equal(minSize + new Size(9, 13) + new Size(4, 6), size);
            Assert.False(control.IsHandleCreated);

            // Call again.
            size = control.GetPreferredSize(proposedSize);
            Assert.Equal(minSize + new Size(9, 13) + new Size(4, 6), size);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, false)]
        [InlineData(ControlStyles.UserPaint, true)]
        [InlineData(ControlStyles.Opaque, true)]
        [InlineData(ControlStyles.ResizeRedraw, true)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, false)]
        [InlineData(ControlStyles.StandardClick, false)]
        [InlineData(ControlStyles.Selectable, true)]
        [InlineData(ControlStyles.UserMouse, true)]
        [InlineData(ControlStyles.SupportsTransparentBackColor, true)]
        [InlineData(ControlStyles.StandardDoubleClick, false)]
        [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
        [InlineData(ControlStyles.CacheText, true)]
        [InlineData(ControlStyles.EnableNotifyMessage, false)]
        [InlineData(ControlStyles.DoubleBuffer, false)]
        [InlineData(ControlStyles.OptimizedDoubleBuffer, true)]
        [InlineData(ControlStyles.UseTextForAccessibility, true)]
        [InlineData((ControlStyles)0, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void Button_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubButton();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void Button_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubButton();
            Assert.False(control.GetTopLevel());
        }

        public static IEnumerable<object[]> NotifyDefault_TestData()
        {
            foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
            {
                yield return new object[] { flatStyle, true };
                yield return new object[] { flatStyle, false };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(NotifyDefault_TestData))]
        public void Button_NotifyDefault_Invoke_GetReturnsExpected(FlatStyle flatStyle, bool value)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle
            };

            control.NotifyDefault(value);
            Assert.Equal(value, control.IsDefault);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.NotifyDefault(value);
            Assert.Equal(value, control.IsDefault);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.NotifyDefault(!value);
            Assert.Equal(!value, control.IsDefault);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> NotifyDefault_WithHandle_TestData()
        {
            yield return new object[] { FlatStyle.Flat, true, 1, 0, 2, 0 };
            yield return new object[] { FlatStyle.Popup, true, 1, 0, 2, 0 };
            yield return new object[] { FlatStyle.Standard, true, 1, 0, 2, 0 };
            yield return new object[] { FlatStyle.System, true, 1, 1, 2, 2 };

            yield return new object[] { FlatStyle.Flat, false, 0, 0, 1, 0 };
            yield return new object[] { FlatStyle.Popup, false, 0, 0, 1, 0 };
            yield return new object[] { FlatStyle.Standard, false, 0, 0, 1, 0 };
            yield return new object[] { FlatStyle.System, false, 0, 0, 1, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(NotifyDefault_WithHandle_TestData))]
        public void Button_NotifyDefault_InvokeWithHandle_GetReturnsExpected(FlatStyle flatStyle, bool value, int expectedInvalidatedCallCount1, int expectedStyleChangeCallCount1, int expectedInvalidatedCallCount2, int expectedStyleChangeCallCount2)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.NotifyDefault(value);
            Assert.Equal(value, control.IsDefault);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(expectedStyleChangeCallCount1, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.NotifyDefault(value);
            Assert.Equal(value, control.IsDefault);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(expectedStyleChangeCallCount1, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.NotifyDefault(!value);
            Assert.Equal(!value, control.IsDefault);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
            Assert.Equal(expectedStyleChangeCallCount2, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(1, 2)]
        [InlineData(0, 0)]
        [InlineData(-1, -2)]
        public void Button_RescaleConstantsForDpi_Invoke_Nop(int deviceDpiOld, int deviceDpiNew)
        {
            using var control = new SubButton();
            control.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Button_OnClick_Invoke_CallsClick(EventArgs eventArgs)
        {
            using var control = new SubButton();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Click += handler;
            control.OnClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.Click -= handler;
            control.OnClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Button_OnClick_InvokeWithForm_CallsClick(EventArgs eventArgs)
        {
            using var form = new Form();
            using var parent = new Control
            {
                Parent = form
            };
            using var control = new SubButton
            {
                Parent = parent,
                DialogResult = DialogResult.Yes
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Click += handler;
            control.OnClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(DialogResult.Yes, form.DialogResult);

            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
            Assert.False(form.IsHandleCreated);

            // Remove handler.
            control.Click -= handler;
            control.OnClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(DialogResult.Yes, form.DialogResult);

            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
            Assert.False(form.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Button_OnClick_InvokeWithHandle_CallsClick(EventArgs eventArgs)
        {
            using var control = new SubButton();
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
            control.Click += handler;
            control.OnClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.Click -= handler;
            control.OnClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Button_OnClick_InvokeWithFormWithHandle_CallsClick(EventArgs eventArgs)
        {
            using var form = new Form();
            using var parent = new Control
            {
                Parent = form
            };
            using var control = new SubButton
            {
                Parent = parent,
                DialogResult = DialogResult.Yes
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
            control.Click += handler;
            control.OnClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(DialogResult.Yes, form.DialogResult);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.False(parent.IsHandleCreated);
            Assert.False(form.IsHandleCreated);

            // Remove handler.
            control.Click -= handler;
            control.OnClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(DialogResult.Yes, form.DialogResult);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.False(parent.IsHandleCreated);
            Assert.False(form.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Button_OnDoubleClick_Invoke_CallsDoubleClick(EventArgs eventArgs)
        {
            using var control = new SubButton();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.DoubleClick += handler;
            control.OnDoubleClick(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.DoubleClick -= handler;
            control.OnDoubleClick(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Button_OnFontChanged_Invoke_CallsFontChanged(EventArgs eventArgs)
        {
            using var control = new SubButton();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.FontChanged += handler;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.FontChanged -= handler;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Button_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubButton();
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
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Button_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubButton();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
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
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Button_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubButton();
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
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Button_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubButton();
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
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void Button_OnMouseDoubleClick_Invoke_CallsMouseDoubleClick(MouseEventArgs eventArgs)
        {
            using var control = new SubButton();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseDoubleClick += handler;
            control.OnMouseDoubleClick(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.MouseDoubleClick -= handler;
            control.OnMouseDoubleClick(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> OnMouseDown_TestData()
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                yield return new object[] { enabled, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0) };
                yield return new object[] { enabled, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0) };
                yield return new object[] { enabled, new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0) };
                yield return new object[] { enabled, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0) };
                yield return new object[] { enabled, new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0) };
                yield return new object[] { enabled, new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0) };
                yield return new object[] { enabled, new MouseEventArgs(MouseButtons.Right, 2, 0, 0, 0) };
                yield return new object[] { enabled, new MouseEventArgs(MouseButtons.Left, 3, 0, 0, 0) };
                yield return new object[] { enabled, new MouseEventArgs(MouseButtons.Right, 3, 0, 0, 0) };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseDown_TestData))]
        public void Button_OnMouseDown_Invoke_CallsMouseDown(bool enabled, MouseEventArgs eventArgs)
        {
            using var control = new SubButton
            {
                Enabled = enabled
            };
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

        public static IEnumerable<object[]> OnMouseDown_WithHandle_TestData()
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                yield return new object[] { enabled, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0), 0 };
                yield return new object[] { enabled, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0), 1 };
                yield return new object[] { enabled, new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0), 0 };
                yield return new object[] { enabled, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0), 1 };
                yield return new object[] { enabled, new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0), 0 };
                yield return new object[] { enabled, new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0), 1 };
                yield return new object[] { enabled, new MouseEventArgs(MouseButtons.Right, 2, 0, 0, 0), 0 };
                yield return new object[] { enabled, new MouseEventArgs(MouseButtons.Left, 3, 0, 0, 0), 1 };
                yield return new object[] { enabled, new MouseEventArgs(MouseButtons.Right, 3, 0, 0, 0), 0 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseDown_WithHandle_TestData))]
        public void Button_OnMouseDown_InvokeWithHandle_CallsMouseDown(bool enabled, MouseEventArgs eventArgs, int expectedInvalidatedCallCount)
        {
            using var control = new SubButton
            {
                Enabled = enabled
            };
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
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.MouseDown -= handler;
            control.OnMouseDown(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Button_OnMouseDown_NullE_ReturnsExpected()
        {
            using var control = new SubButton();
            Assert.Throws<NullReferenceException>(() => control.OnMouseDown(null));
        }

        public static IEnumerable<object[]> OnMouseEnter_TestData()
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                foreach (bool autoEllipsis in new bool[] { true, false })
                {
                    foreach (string text in new string[] { null, string.Empty, "text" })
                    {
                        yield return new object[] { enabled, autoEllipsis, text, null };
                        yield return new object[] { enabled, autoEllipsis, text, new EventArgs() };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseEnter_TestData))]
        public void Button_OnMouseEnter_Invoke_CallsMouseEnter(bool enabled, bool autoEllipsis, string text, EventArgs eventArgs)
        {
            using var control = new SubButton
            {
                Enabled = enabled,
                AutoEllipsis = autoEllipsis,
                Text = text
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseEnter += handler;
            control.OnMouseEnter(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseEnter -= handler;
            control.OnMouseEnter(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseEnter_TestData))]
        public void Button_OnMouseEnter_InvokeDesignMode_CallsMouseEnter(bool enabled, bool autoEllipsis, string text, EventArgs eventArgs)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            mockSite
                .Setup(s => s.Name)
                .Returns((string)null);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            using var control = new SubButton
            {
                Enabled = enabled,
                AutoEllipsis = autoEllipsis,
                Text = text,
                Site = mockSite.Object
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseEnter += handler;
            control.OnMouseEnter(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseEnter -= handler;
            control.OnMouseEnter(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseEnter_TestData))]
        public void Button_OnMouseEnter_InvokeWithHandle_CallsMouseEnter(bool enabled, bool autoEllipsis, string text, EventArgs eventArgs)
        {
            using var control = new SubButton
            {
                Enabled = enabled,
                AutoEllipsis = autoEllipsis,
                Text = text
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
            control.MouseEnter += handler;
            control.OnMouseEnter(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.MouseEnter -= handler;
            control.OnMouseEnter(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseEnter_TestData))]
        public void Button_OnMouseEnter_InvokeDesignModeWithHandle_CallsMouseEnter(bool enabled, bool autoEllipsis, string text, EventArgs eventArgs)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            mockSite
                .Setup(s => s.Name)
                .Returns((string)null);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            using var control = new SubButton
            {
                Enabled = enabled,
                AutoEllipsis = autoEllipsis,
                Text = text,
                Site = mockSite.Object
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
            control.MouseEnter += handler;
            control.OnMouseEnter(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.MouseEnter -= handler;
            control.OnMouseEnter(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> OnMouseLeave_TestData()
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                foreach (bool autoEllipsis in new bool[] { true, false })
                {
                    foreach (string text in new string[] { null, string.Empty, "text" })
                    {
                        yield return new object[] { enabled, autoEllipsis, text, null };
                        yield return new object[] { enabled, autoEllipsis, text, new EventArgs() };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseLeave_TestData))]
        public void Button_OnMouseLeave_Invoke_CallsMouseLeave(bool enabled, bool autoEllipsis, string text, EventArgs eventArgs)
        {
            using var control = new SubButton
            {
                Enabled = enabled,
                AutoEllipsis = autoEllipsis,
                Text = text
            };
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
            Assert.Equal(autoEllipsis, control.IsHandleCreated);

            // Remove handler.
            control.MouseLeave -= handler;
            control.OnMouseLeave(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(autoEllipsis, control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseLeave_TestData))]
        public void Button_OnMouseLeave_InvokeWithHandle_CallsMouseLeave(bool enabled, bool autoEllipsis, string text, EventArgs eventArgs)
        {
            using var control = new SubButton
            {
                Enabled = enabled,
                AutoEllipsis = autoEllipsis,
                Text = text
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
            control.MouseLeave += handler;
            control.OnMouseLeave(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.MouseLeave -= handler;
            control.OnMouseLeave(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> OnMouseUp_TestData()
        {
            foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
            {
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.None, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Left, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Middle, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Right, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.None, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Left, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Middle, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Right, 1, 2, 3, 4) };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseUp_TestData))]
        public void Button_OnMouseUp_Invoke_CallsMouseUp(FlatStyle flatStyle, MouseEventArgs eventArgs)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle
            };
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int clickCallCount = 0;
            control.Click += (sender, e) => clickCallCount++;
            int mouseClickCallCount = 0;
            control.MouseClick += (sender, e) => mouseClickCallCount++;

            // Call with handler.
            control.MouseUp += handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, clickCallCount);
            Assert.Equal(0, mouseClickCallCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseUp -= handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, clickCallCount);
            Assert.Equal(0, mouseClickCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnMouseUp_MouseDown_TestData()
        {
            foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
            {
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.None, 1, 2, 3, 4), false };
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Left, 1, 2, 3, 4), true };
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Middle, 1, 2, 3, 4), false };
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Right, 1, 2, 3, 4), false };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.None, 1, 2, 3, 4), false };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Left, 1, 2, 3, 4), true };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Middle, 1, 2, 3, 4), false };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Right, 1, 2, 3, 4), false };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseUp_MouseDown_TestData))]
        public void Button_OnMouseUp_InvokeMouseDown_CallsMouseUp(FlatStyle flatStyle, MouseEventArgs eventArgs, bool expectedIsHandleCreated)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle
            };
            control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int clickCallCount = 0;
            control.Click += (sender, e) => clickCallCount++;
            int mouseClickCallCount = 0;
            control.MouseClick += (sender, e) => mouseClickCallCount++;

            // Call with handler.
            control.MouseUp += handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, clickCallCount);
            Assert.Equal(0, mouseClickCallCount);
            Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);

            // Remove handler.
            control.MouseUp -= handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, clickCallCount);
            Assert.Equal(0, mouseClickCallCount);
            Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseUp_TestData))]
        public void Button_OnMouseUp_InvokeWithHandle_CallsMouseUp(FlatStyle flatStyle, MouseEventArgs eventArgs)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle
            };
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
            int clickCallCount = 0;
            control.Click += (sender, e) => clickCallCount++;
            int mouseClickCallCount = 0;
            control.MouseClick += (sender, e) => mouseClickCallCount++;

            // Call with handler.
            control.MouseUp += handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, clickCallCount);
            Assert.Equal(0, mouseClickCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.MouseUp -= handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, clickCallCount);
            Assert.Equal(0, mouseClickCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> OnMouseUp_WithHandle_TestData()
        {
            foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
            {
                int expectedCallCount = flatStyle != FlatStyle.System ? 1 : 0;
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.None, 1, 2, 3, 4), 0 };
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Left, 1, 2, 3, 4), expectedCallCount };
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Middle, 1, 2, 3, 4), 0 };
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Right, 1, 2, 3, 4), 0 };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.None, 1, 2, 3, 4), 0 };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Left, 1, 2, 3, 4), expectedCallCount };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Middle, 1, 2, 3, 4), 0 };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Right, 1, 2, 3, 4), 0 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseUp_WithHandle_TestData))]
        public void Button_OnMouseUp_InvokeMouseDownWithHandle_CallsMouseUp(FlatStyle flatStyle, MouseEventArgs eventArgs, int expectedInvalidatedCallCount)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle
            };
            control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
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
            int clickCallCount = 0;
            control.Click += (sender, e) => clickCallCount++;
            int mouseClickCallCount = 0;
            control.MouseClick += (sender, e) => mouseClickCallCount++;

            // Call with handler.
            control.MouseUp += handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, clickCallCount);
            Assert.Equal(0, mouseClickCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.MouseUp -= handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, clickCallCount);
            Assert.Equal(0, mouseClickCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Button_OnMouseUp_NullE_ThrowsNullReferenceException()
        {
            using var control = new SubButton();
            Assert.Throws<NullReferenceException>(() => control.OnMouseUp(null));
        }

        public static IEnumerable<object[]> OnTextChanged_TestData()
        {
            foreach (bool autoSize in new bool[] { true, false })
            {
                yield return new object[] { autoSize, null };
                yield return new object[] { autoSize, new EventArgs() };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnTextChanged_TestData))]
        public void Button_OnTextChanged_Invoke_CallsTextChanged(bool autoSize, EventArgs eventArgs)
        {
            using var control = new SubButton
            {
                AutoSize = autoSize
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.TextChanged += handler;
            control.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.TextChanged -= handler;
            control.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnTextChanged_WithParent_TestData()
        {
            yield return new object[] { true, null, 1 };
            yield return new object[] { true, new EventArgs(), 1 };
            yield return new object[] { false, null, 0 };
            yield return new object[] { false, new EventArgs(), 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnTextChanged_WithParent_TestData))]
        public void Button_OnTextChanged_InvokeWithParent_CallsTextChanged(bool autoSize, EventArgs eventArgs, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new SubButton
            {
                AutoSize = autoSize,
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Text", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                int callCount = 0;
                EventHandler handler = (sender, e) =>
                {
                    Assert.Same(control, sender);
                    Assert.Same(eventArgs, e);
                    callCount++;
                };

                // Call with handler.
                control.TextChanged += handler;
                control.OnTextChanged(eventArgs);
                Assert.Equal(1, callCount);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);

                // Remove handler.
                control.TextChanged -= handler;
                control.OnTextChanged(eventArgs);
                Assert.Equal(1, callCount);
                Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnTextChanged_TestData))]
        public void Button_OnTextChanged_InvokeWithHandle_CallsTextChanged(bool autoSize, EventArgs eventArgs)
        {
            using var control = new SubButton
            {
                AutoSize = autoSize
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.TextChanged += handler;
            control.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.TextChanged -= handler;
            control.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnTextChanged_WithParent_TestData))]
        public void Button_OnTextChanged_InvokeWithParentWithHandle_CallsTextChanged(bool autoSize, EventArgs eventArgs, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new SubButton
            {
                AutoSize = autoSize,
                Parent = parent
            };
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Text", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                int callCount = 0;
                EventHandler handler = (sender, e) =>
                {
                    Assert.Same(control, sender);
                    Assert.Same(eventArgs, e);
                    callCount++;
                };

                // Call with handler.
                control.TextChanged += handler;
                control.OnTextChanged(eventArgs);
                Assert.Equal(1, callCount);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(1, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);

                // Remove handler.
                control.TextChanged -= handler;
                control.OnTextChanged(eventArgs);
                Assert.Equal(1, callCount);
                Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(2, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void Button_PerformClick_Invoke_CallsClick()
        {
            using var control = new SubButton();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };

            // Call with handler.
            control.Click += handler;
            control.PerformClick();
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.Click -= handler;
            control.PerformClick();
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void Button_PerformClick_InvokeWithForm_CallsClick()
        {
            using var form = new Form();
            using var parent = new Control
            {
                Parent = form
            };
            using var control = new SubButton
            {
                Parent = parent,
                DialogResult = DialogResult.Yes
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };

            // Call with handler.
            control.Click += handler;
            control.PerformClick();
            Assert.Equal(0, callCount);
            Assert.Equal(DialogResult.None, form.DialogResult);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
            Assert.False(form.IsHandleCreated);

            // Remove handler.
            control.Click -= handler;
            control.PerformClick();
            Assert.Equal(0, callCount);
            Assert.Equal(DialogResult.None, form.DialogResult);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
            Assert.False(form.IsHandleCreated);
        }

        [WinFormsFact]
        public void Button_PerformClick_InvokeWithHandle_CallsClick()
        {
            using var control = new SubButton();
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
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };

            // Call with handler.
            control.Click += handler;
            control.PerformClick();
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.Click -= handler;
            control.PerformClick();
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Button_PerformClick_InvokeWithFormWithHandle_CallsClick()
        {
            using var form = new Form();
            using var parent = new Control
            {
                Parent = form
            };
            using var control = new SubButton
            {
                Parent = parent,
                DialogResult = DialogResult.Yes
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
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };

            // Call with handler.
            control.Click += handler;
            control.PerformClick();
            Assert.Equal(0, callCount);
            Assert.Equal(DialogResult.None, form.DialogResult);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.False(parent.IsHandleCreated);
            Assert.False(form.IsHandleCreated);

            // Remove handler.
            control.Click -= handler;
            control.PerformClick();
            Assert.Equal(0, callCount);
            Assert.Equal(DialogResult.None, form.DialogResult);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.False(parent.IsHandleCreated);
            Assert.False(form.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, "", 'a', 0)]
        [InlineData(true, "", char.MinValue, 0)]
        [InlineData(true, "&a", 'a', 1)]
        [InlineData(true, "&a", 'b', 0)]
        [InlineData(true, "&&a", 'a', 0)]
        [InlineData(true, "&", 'a', 0)]
        [InlineData(true, "text", 'a', 0)]
        [InlineData(false, "", 'a', 0)]
        [InlineData(false, "", char.MinValue, 0)]
        [InlineData(false, "&a", 'a', 0)]
        [InlineData(false, "&a", 'b', 0)]
        [InlineData(false, "&&a", 'a', 0)]
        [InlineData(false, "&", 'a', 0)]
        [InlineData(false, "text", 'a', 0)]
        public void Button_ProcessMnemonic_Invoke_ReturnsExpected(bool useMnemonic, string text, char charCode, int expectedClickCallCount)
        {
            using var control = new SubButton
            {
                UseMnemonic = useMnemonic,
                Text = text
            };
            int clickCallCount = 0;
            control.Click += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                clickCallCount++;
            };
            Assert.Equal(expectedClickCallCount != 0, control.ProcessMnemonic(charCode));
            Assert.Equal(expectedClickCallCount, clickCallCount);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, "", 'a')]
        [InlineData(true, "", char.MinValue)]
        [InlineData(true, "&a", 'a')]
        [InlineData(true, "&a", 'b')]
        [InlineData(true, "&&a", 'a')]
        [InlineData(true, "&", 'a')]
        [InlineData(true, "text", 'a')]
        [InlineData(false, "", 'a')]
        [InlineData(false, "", char.MinValue)]
        [InlineData(false, "&a", 'a')]
        [InlineData(false, "&a", 'b')]
        [InlineData(false, "&&a", 'a')]
        [InlineData(false, "&", 'a')]
        [InlineData(false, "text", 'a')]
        public void Button_ProcessMnemonic_InvokeCantProcessMnemonic_ReturnsFalse(bool useMnemonic, string text, char charCode)
        {
            using var control = new SubButton
            {
                UseMnemonic = useMnemonic,
                Text = text,
                Enabled = false
            };
            int clickCallCount = 0;
            control.Click += (sender, e) => clickCallCount++;
            Assert.False(control.ProcessMnemonic(charCode));
            Assert.Equal(0, clickCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void Button_ToString_Invoke_ReturnsExpected()
        {
            using var control = new Button();
            Assert.Equal("System.Windows.Forms.Button, Text: ", control.ToString());
        }

        [WinFormsFact]
        public void Button_RaiseAutomationEvent_Invoke_Success_IfControlIsCreated()
        {
            using var button = new TestButton();
            Assert.False(button.IsHandleCreated);
            button.CreateControl();
            var accessibleObject = (SubButtonAccessibleObject)button.AccessibilityObject;
            Assert.Equal(0, accessibleObject.RaiseAutomationEventCallsCount);
            Assert.Equal(0, accessibleObject.RaiseAutomationPropertyChangedEventCallsCount);

            button.PerformClick();

            Assert.Equal(1, accessibleObject.RaiseAutomationEventCallsCount);
            Assert.Equal(1, accessibleObject.RaiseAutomationPropertyChangedEventCallsCount);
            Assert.True(button.IsHandleCreated);
        }

        [WinFormsFact]
        public void Button_RaiseAutomationEvent_IsNotInvoked_IfControlIsNotCreated()
        {
            using var button = new TestButton();
            var accessibleObject = (SubButtonAccessibleObject)button.AccessibilityObject;
            Assert.Equal(0, accessibleObject.RaiseAutomationEventCallsCount);
            Assert.Equal(0, accessibleObject.RaiseAutomationPropertyChangedEventCallsCount);

            button.PerformClick();

            Assert.Equal(0, accessibleObject.RaiseAutomationEventCallsCount);
            Assert.Equal(0, accessibleObject.RaiseAutomationPropertyChangedEventCallsCount);
            Assert.False(button.IsHandleCreated);
        }

        [WinFormsFact]
        public void Button_ToString_InvokeShortText_ReturnsExpected()
        {
            using var control = new Button
            {
                Text = "Text"
            };
            Assert.Equal("System.Windows.Forms.Button, Text: Text", control.ToString());
        }

        public static IEnumerable<object[]> WndProc_EraseBkgndWithoutHandleWithoutWParam_TestData()
        {
            foreach (bool opaque in new bool[] { true, false })
            {
                yield return new object[] { true, true, opaque, IntPtr.Zero };
                yield return new object[] { true, false, opaque, IntPtr.Zero };
                yield return new object[] { false, true, opaque, IntPtr.Zero };
                yield return new object[] { false, false, opaque, IntPtr.Zero };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(WndProc_EraseBkgndWithoutHandleWithoutWParam_TestData))]
        public void Button_WndProc_InvokeEraseBkgndWithoutHandleWithoutWParam_Success(bool userPaint, bool allPaintingInWmPaint, bool opaque, IntPtr expectedResult)
        {
            using (new NoAssertContext())
            {
                using var control = new SubButton();
                control.SetStyle(ControlStyles.UserPaint, userPaint);
                control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
                control.SetStyle(ControlStyles.Opaque, opaque);
                int paintCallCount = 0;
                control.Paint += (sender, e) => paintCallCount++;

                var m = new Message
                {
                    Msg = (int)User32.WM.ERASEBKGND,
                    Result = (IntPtr)250
                };
                control.WndProc(ref m);
                Assert.Equal(expectedResult, m.Result);
                Assert.False(control.IsHandleCreated);
                Assert.Equal(0, paintCallCount);
            }
        }

        public static IEnumerable<object[]> WndProc_EraseBkgnd_TestData()
        {
            foreach (bool userPaint in new bool[] { true, false })
            {
                foreach (bool allPaintingInWmPaint in new bool[] { true, false })
                {
                    foreach (bool opaque in new bool[] { true, false })
                    {
                        yield return new object[] { userPaint, allPaintingInWmPaint, opaque };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(WndProc_EraseBkgnd_TestData))]
        public void Button_WndProc_InvokeEraseBkgndWithoutHandleWithWParam_Success(bool userPaint, bool allPaintingInWmPaint, bool opaque)
        {
            using (new NoAssertContext())
            {
                using var control = new SubButton();
                control.SetStyle(ControlStyles.UserPaint, userPaint);
                control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
                control.SetStyle(ControlStyles.Opaque, opaque);
                int paintCallCount = 0;
                control.Paint += (sender, e) => paintCallCount++;

                using var image = new Bitmap(10, 10);
                using Graphics graphics = Graphics.FromImage(image);
                IntPtr hdc = graphics.GetHdc();
                try
                {
                    var m = new Message
                    {
                        Msg = (int)User32.WM.ERASEBKGND,
                        WParam = hdc,
                        Result = (IntPtr)250
                    };
                    control.WndProc(ref m);
                    Assert.Equal(IntPtr.Zero, m.Result);
                    Assert.False(control.IsHandleCreated);
                    Assert.Equal(0, paintCallCount);
                }
                finally
                {
                    graphics.ReleaseHdc();
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(WndProc_EraseBkgnd_TestData))]
        public void Button_WndProc_InvokeEraseBkgndWithHandleWithoutWParam_Success(bool userPaint, bool allPaintingInWmPaint, bool opaque)
        {
            using var control = new SubButton();
            control.SetStyle(ControlStyles.UserPaint, userPaint);
            control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
            control.SetStyle(ControlStyles.Opaque, opaque);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int paintCallCount = 0;
            control.Paint += (sender, e) => paintCallCount++;

            var m = new Message
            {
                Msg = (int)User32.WM.ERASEBKGND,
                Result = (IntPtr)250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(0, paintCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(WndProc_EraseBkgnd_TestData))]
        public void Button_WndProc_InvokeEraseBkgndWithHandleWithWParam_Success(bool userPaint, bool allPaintingInWmPaint, bool opaque)
        {
            using var control = new SubButton();
            control.SetStyle(ControlStyles.UserPaint, userPaint);
            control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
            control.SetStyle(ControlStyles.Opaque, opaque);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int paintCallCount = 0;
            control.Paint += (sender, e) => paintCallCount++;

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            IntPtr hdc = graphics.GetHdc();
            try
            {
                var m = new Message
                {
                    Msg = (int)User32.WM.ERASEBKGND,
                    WParam = hdc,
                    Result = (IntPtr)250
                };
                control.WndProc(ref m);
                Assert.Equal(IntPtr.Zero, m.Result);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.Equal(0, paintCallCount);
            }
            finally
            {
                graphics.ReleaseHdc();
            }
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(FlatStyle))]
        public void Button_WndProc_InvokeMouseHoverWithHandle_Success(FlatStyle flatStyle)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle
            };
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
                Msg = (int)WM.MOUSEHOVER,
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

        public static IEnumerable<object[]> WndProc_ReflectCommandWithoutHandle_TestData()
        {
            yield return new object[] { FlatStyle.Flat, IntPtr.Zero, (IntPtr)250, 1 };
            yield return new object[] { FlatStyle.Flat, PARAM.FromLowHigh(0, (int)BN.CLICKED), (IntPtr)250, 1 };
            yield return new object[] { FlatStyle.Flat, PARAM.FromLowHigh(123, (int)BN.CLICKED), (IntPtr)250, 1 };
            yield return new object[] { FlatStyle.Flat, PARAM.FromLowHigh(123, 456), (IntPtr)250, 0 };

            yield return new object[] { FlatStyle.Popup, IntPtr.Zero, (IntPtr)250, 1 };
            yield return new object[] { FlatStyle.Popup, PARAM.FromLowHigh(0, (int)BN.CLICKED), (IntPtr)250, 1 };
            yield return new object[] { FlatStyle.Popup, PARAM.FromLowHigh(123, (int)BN.CLICKED), (IntPtr)250, 1 };
            yield return new object[] { FlatStyle.Popup, PARAM.FromLowHigh(123, 456), (IntPtr)250, 0 };

            yield return new object[] { FlatStyle.Standard, IntPtr.Zero, (IntPtr)250, 1 };
            yield return new object[] { FlatStyle.Standard, PARAM.FromLowHigh(0, (int)BN.CLICKED), (IntPtr)250, 1 };
            yield return new object[] { FlatStyle.Standard, PARAM.FromLowHigh(123, (int)BN.CLICKED), (IntPtr)250, 1 };
            yield return new object[] { FlatStyle.Standard, PARAM.FromLowHigh(123, 456), (IntPtr)250, 0 };

            yield return new object[] { FlatStyle.System, IntPtr.Zero, (IntPtr)250, 1 };
            yield return new object[] { FlatStyle.System, PARAM.FromLowHigh(0, (int)BN.CLICKED), (IntPtr)250, 1 };
            yield return new object[] { FlatStyle.System, PARAM.FromLowHigh(123, (int)BN.CLICKED), (IntPtr)250, 1 };
            yield return new object[] { FlatStyle.System, PARAM.FromLowHigh(123, 456), (IntPtr)250, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(WndProc_ReflectCommandWithoutHandle_TestData))]
        public void Button_WndProc_InvokeReflectCommandWithoutHandle_Success(FlatStyle flatStyle, IntPtr wParam, IntPtr expectedResult, int expectedCallCount)
        {
            using (new NoAssertContext())
            {
                using var control = new SubButton
                {
                    FlatStyle = flatStyle
                };
                int callCount = 0;
                control.Click += (sender, e) =>
                {
                    Assert.Same(control, sender);
                    Assert.Same(EventArgs.Empty, e);
                    callCount++;
                };

                var m = new Message
                {
                    Msg = (int)(WM.REFLECT | WM.COMMAND),
                    WParam = wParam,
                    Result = (IntPtr)250
                };
                control.WndProc(ref m);
                Assert.Equal(expectedResult, m.Result);
                Assert.Equal(expectedCallCount, callCount);
                Assert.False(control.IsHandleCreated);
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(WndProc_ReflectCommandWithoutHandle_TestData))]
        public void Button_WndProc_InvokeReflectCommandWithHandle_Success(FlatStyle flatStyle, IntPtr wParam, IntPtr expectedResult, int expectedCallCount)
        {
            using var control = new SubButton
            {
                FlatStyle = flatStyle
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int callCount = 0;
            control.Click += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };

            var m = new Message
            {
                Msg = (int)(WM.REFLECT | WM.COMMAND),
                WParam = wParam,
                Result = (IntPtr)250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.Equal(expectedCallCount, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        private class SubButton : Button
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

            public new bool IsDefault
            {
                get => base.IsDefault;
                set => base.IsDefault = value;
            }

            public new bool ResizeRedraw
            {
                get => base.ResizeRedraw;
                set => base.ResizeRedraw = value;
            }

            public new bool ShowFocusCues => base.ShowFocusCues;

            public new bool ShowKeyboardCues => base.ShowKeyboardCues;

            public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public new void OnClick(EventArgs e) => base.OnClick(e);

            public new void OnDoubleClick(EventArgs e) => base.OnDoubleClick(e);

            public new void OnFontChanged(EventArgs e) => base.OnFontChanged(e);

            public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

            public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

            public new void OnMouseDoubleClick(MouseEventArgs e) => base.OnMouseDoubleClick(e);

            public new void OnMouseDown(MouseEventArgs e) => base.OnMouseDown(e);

            public new void OnMouseEnter(EventArgs eventargs) => base.OnMouseEnter(eventargs);

            public new void OnMouseLeave(EventArgs eventargs) => base.OnMouseLeave(eventargs);

            public new void OnMouseUp(MouseEventArgs eventargs) => base.OnMouseUp(eventargs);

            public new void OnTextChanged(EventArgs e) => base.OnTextChanged(e);

            public new bool ProcessMnemonic(char charCode) => base.ProcessMnemonic(charCode);

            public new void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew) => base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);

            public new void WndProc(ref Message m) => base.WndProc(ref m);
        }

        private class TestButton : Button
        {
            protected override AccessibleObject CreateAccessibilityInstance()
            {
                return new SubButtonAccessibleObject(this);
            }
        }

        private class SubButtonAccessibleObject : Button.ButtonAccessibleObject
        {
            public SubButtonAccessibleObject(Button owner) : base(owner)
            {
                RaiseAutomationEventCallsCount = 0;
                RaiseAutomationPropertyChangedEventCallsCount = 0;
            }

            public int RaiseAutomationEventCallsCount { get; private set; }

            public int RaiseAutomationPropertyChangedEventCallsCount { get; private set; }

            internal override bool RaiseAutomationEvent(UIA eventId)
            {
                if (Owner.IsHandleCreated)
                {
                    RaiseAutomationEventCallsCount++;
                }

                return base.RaiseAutomationEvent(eventId);
            }

            internal override bool RaiseAutomationPropertyChangedEvent(UIA propertyId, object oldValue, object newValue)
            {
                if (Owner.IsHandleCreated)
                {
                    RaiseAutomationPropertyChangedEventCallsCount++;
                }

                return base.RaiseAutomationPropertyChangedEvent(propertyId, oldValue, newValue);
            }
        }
    }
}

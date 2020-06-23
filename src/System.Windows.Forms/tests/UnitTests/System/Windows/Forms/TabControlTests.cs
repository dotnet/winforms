// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Moq;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class TabControlTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void TabControl_Ctor_Default()
        {
            using var control = new SubTabControl();
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.Equal(TabAlignment.Top, control.Alignment);
            Assert.False(control.AllowDrop);
            Assert.Equal(TabAppearance.Normal, control.Appearance);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(100, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.ClientRectangle);
            Assert.Equal(new Size(200, 100), control.ClientSize);
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
            Assert.Equal(new Size(200, 100), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.DoubleBuffered);
            Assert.Equal(TabDrawMode.Normal, control.DrawMode);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(100, control.Height);
            Assert.False(control.HotTrack);
            Assert.Null(control.ImageList);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.Equal(Size.Empty, control.ItemSize);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.False(control.Multiline);
            Assert.Equal(new Point(6, 3), control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal(new Size(200, 100), control.PreferredSize);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(200, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.False(control.RightToLeftLayout);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedTab);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.False(control.ShowToolTips);
            Assert.Null(control.Site);
            Assert.Equal(new Size(200, 100), control.Size);
            Assert.Equal(TabSizeMode.Normal, control.SizeMode);
            Assert.Equal(0, control.TabCount);
            Assert.Equal(0, control.TabIndex);
            Assert.Empty(control.TabPages);
            Assert.Same(control.TabPages, control.TabPages);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.Equal(200, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControl_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubTabControl();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTabControl32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(100, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56010800, createParams.Style);
            Assert.Equal(200, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0x56010A00)]
        [InlineData(false, 0x56010800)]
        public void TabControl_CreateParams_GetMultiline_ReturnsExpected(bool multiline, int expectedStyle)
        {
            using var control = new SubTabControl
            {
                Multiline = multiline
            };
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTabControl32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(100, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(200, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
        }

        [WinFormsTheory]
        [InlineData(TabDrawMode.Normal, 0x56010800)]
        [InlineData(TabDrawMode.OwnerDrawFixed, 0x56012800)]
        public void TabControl_CreateParams_GetDrawMode_ReturnsExpected(TabDrawMode drawMode, int expectedStyle)
        {
            using var control = new SubTabControl
            {
                DrawMode = drawMode
            };
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTabControl32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(100, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(200, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
        }

        [WinFormsTheory]
        [InlineData(true, true, 0x56010800)]
        [InlineData(false, true, 0x56010800)]
        [InlineData(true, false, 0x56014800)]
        [InlineData(false, false, 0x56010800)]
        public void TabControl_CreateParams_GetShowToolTips_ReturnsExpected(bool ShowToolTips, bool designMode, int expectedStyle)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(designMode);
            mockSite
                .Setup(s => s.Container)
                .Returns<IContainer>(null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            using var control = new SubTabControl
            {
                ShowToolTips = ShowToolTips,
                Site = mockSite.Object
            };
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTabControl32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(100, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(200, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
        }

        [WinFormsTheory]
        [InlineData(TabAlignment.Bottom, 0x56010802)]
        [InlineData(TabAlignment.Left, 0x56010A80)]
        [InlineData(TabAlignment.Right, 0x56010A82)]
        [InlineData(TabAlignment.Top, 0x56010800)]
        public void TabControl_CreateParams_GetAlignment_ReturnsExpected(TabAlignment alignment, int expectedStyle)
        {
            using var control = new SubTabControl
            {
                Alignment = alignment
            };
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTabControl32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(100, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(200, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
        }

        [WinFormsTheory]
        [InlineData(true, 0x56010840)]
        [InlineData(false, 0x56010800)]
        public void TabControl_CreateParams_GetHotTrack_ReturnsExpected(bool hotTrack, int expectedStyle)
        {
            using var control = new SubTabControl
            {
                HotTrack = hotTrack
            };
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTabControl32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(100, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(200, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
        }

        [WinFormsTheory]
        [InlineData(TabAppearance.Normal, TabAlignment.Bottom, 0x56010802)]
        [InlineData(TabAppearance.Normal, TabAlignment.Left, 0x56010A80)]
        [InlineData(TabAppearance.Normal, TabAlignment.Right, 0x56010A82)]
        [InlineData(TabAppearance.Normal, TabAlignment.Top, 0x56010800)]
        [InlineData(TabAppearance.Buttons, TabAlignment.Bottom, 0x56010902)]
        [InlineData(TabAppearance.Buttons, TabAlignment.Left, 0x56010B80)]
        [InlineData(TabAppearance.Buttons, TabAlignment.Right, 0x56010B82)]
        [InlineData(TabAppearance.Buttons, TabAlignment.Top, 0x56010900)]
        [InlineData(TabAppearance.FlatButtons, TabAlignment.Bottom, 0x56010902)]
        [InlineData(TabAppearance.FlatButtons, TabAlignment.Left, 0x56010B80)]
        [InlineData(TabAppearance.FlatButtons, TabAlignment.Right, 0x56010B82)]
        [InlineData(TabAppearance.FlatButtons, TabAlignment.Top, 0x56010908)]
        public void TabControl_CreateParams_GetAppearance_ReturnsExpected(TabAppearance appearance, TabAlignment alignment, int expectedStyle)
        {
            using var control = new SubTabControl
            {
                Appearance = appearance,
                Alignment = alignment
            };
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTabControl32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(100, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(200, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
        }

        [WinFormsTheory]
        [InlineData(TabSizeMode.FillToRight, 0x56010000)]
        [InlineData(TabSizeMode.Fixed, 0x56010400)]
        [InlineData(TabSizeMode.Normal, 0x56010800)]
        public void TabControl_CreateParams_GetSizeMode_ReturnsExpected(TabSizeMode sizeMode, int expectedStyle)
        {
            using var control = new SubTabControl
            {
                SizeMode = sizeMode
            };
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTabControl32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(100, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(200, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
        }

        [WinFormsTheory]
        [InlineData(RightToLeft.Inherit, true, 0x56010800, 0x0)]
        [InlineData(RightToLeft.No, true, 0x56010800, 0x0)]
        [InlineData(RightToLeft.Yes, true, 0x56010800, 0x500000)]
        [InlineData(RightToLeft.Inherit, false, 0x56010800, 0x0)]
        [InlineData(RightToLeft.No, false, 0x56010800, 0x0)]
        [InlineData(RightToLeft.Yes, false, 0x56010800, 0x7000)]
        public void TabControl_CreateParams_GetRightToLeft_ReturnsExpected(RightToLeft rightToLeft, bool rightToLeftLayout, int expectedStyle, int expectedExStyle)
        {
            using var control = new SubTabControl
            {
                RightToLeft = rightToLeft,
                RightToLeftLayout = rightToLeftLayout
            };
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SysTabControl32", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(expectedExStyle, createParams.ExStyle);
            Assert.Equal(100, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(200, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
        }

        public static IEnumerable<object[]> Alignment_Set_TestData()
        {
            yield return new object[] { true, TabAlignment.Top, true };
            yield return new object[] { true, TabAlignment.Bottom, true };
            yield return new object[] { true, TabAlignment.Left, true };
            yield return new object[] { true, TabAlignment.Right, true };

            yield return new object[] { false, TabAlignment.Top, false };
            yield return new object[] { false, TabAlignment.Bottom, false };
            yield return new object[] { false, TabAlignment.Left, true };
            yield return new object[] { false, TabAlignment.Right, true };
        }

        [WinFormsTheory]
        [MemberData(nameof(Alignment_Set_TestData))]
        public void TabControl_Alignment_Set_GetReturnsExpected(bool multiline, TabAlignment value, bool expectedMultiline)
        {
            using var control = new TabControl
            {
                Multiline = multiline,
                Alignment = value
            };
            Assert.Equal(value, control.Alignment);
            Assert.Equal(expectedMultiline, control.Multiline);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Alignment = value;
            Assert.Equal(value, control.Alignment);
            Assert.Equal(expectedMultiline, control.Multiline);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Alignment_SetWithHandle_TestData()
        {
            yield return new object[] { true, TabAlignment.Top, true, 0 };
            yield return new object[] { true, TabAlignment.Bottom, true, 1 };
            yield return new object[] { true, TabAlignment.Left, true, 1 };
            yield return new object[] { true, TabAlignment.Right, true, 1 };

            yield return new object[] { false, TabAlignment.Top, false, 0 };
            yield return new object[] { false, TabAlignment.Bottom, false, 1 };
            yield return new object[] { false, TabAlignment.Left, true, 1 };
            yield return new object[] { false, TabAlignment.Right, true, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Alignment_SetWithHandle_TestData))]
        public void TabControl_Alignment_SetWithHandle_GetReturnsExpected(bool multiline, TabAlignment value, bool expectedMultiline, int expectedCreatedCallCount)
        {
            using var control = new TabControl
            {
                Multiline = multiline
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Alignment = value;
            Assert.Equal(value, control.Alignment);
            Assert.Equal(expectedMultiline, control.Multiline);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.Alignment = value;
            Assert.Equal(value, control.Alignment);
            Assert.Equal(expectedMultiline, control.Multiline);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TabAlignment))]
        public void TabControl_Alignment_SetInvalidValue_ThrowsInvalidEnumArgumentException(TabAlignment value)
        {
            using var control = new TabControl();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.Alignment = value);
        }

        [WinFormsTheory]
        [InlineData(TabAlignment.Bottom)]
        [InlineData(TabAlignment.Left)]
        [InlineData(TabAlignment.Right)]
        public void TabControl_Appearance_GetFlatButtonsWithAlignment_ReturnsExpected(TabAlignment alignment)
        {
            using var control = new TabControl
            {
                Appearance = TabAppearance.FlatButtons,
                Alignment = alignment
            };
            Assert.Equal(TabAppearance.Buttons, control.Appearance);
            Assert.Equal(alignment, control.Alignment);
            Assert.False(control.IsHandleCreated);

            control.Alignment = TabAlignment.Top;
            Assert.Equal(TabAppearance.FlatButtons, control.Appearance);
            Assert.Equal(TabAlignment.Top, control.Alignment);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TabAppearance))]
        public void TabControl_Appearance_Set_GetReturnsExpected(TabAppearance value)
        {
            using var control = new TabControl
            {
                Appearance = value
            };
            Assert.Equal(value, control.Appearance);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Appearance = value;
            Assert.Equal(value, control.Appearance);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(TabAppearance.Normal, 0)]
        [InlineData(TabAppearance.Buttons, 1)]
        [InlineData(TabAppearance.FlatButtons, 1)]
        public void TabControl_Appearance_SetWithHandle_GetReturnsExpected(TabAppearance value, int expectedCreatedCallCount)
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Appearance = value;
            Assert.Equal(value, control.Appearance);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedCreatedCallCount, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.Appearance = value;
            Assert.Equal(value, control.Appearance);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedCreatedCallCount, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TabAppearance))]
        public void TabControl_Appearance_SetInvalidValue_ThrowsInvalidEnumArgumentException(TabAppearance value)
        {
            using var control = new TabControl();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.Appearance = value);
        }

        public static IEnumerable<object[]> BackColor_Set_TestData()
        {
            yield return new object[] { Color.Red };
            yield return new object[] { Color.FromArgb(254, 1, 2, 3) };
            yield return new object[] { Color.Empty };
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_Set_TestData))]
        public void TabControl_BackColor_Set_GetReturnsExpected(Color value)
        {
            using var control = new TabControl
            {
                BackColor = value
            };
            Assert.Equal(SystemColors.Control, control.BackColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackColor = value;
            Assert.Equal(SystemColors.Control, control.BackColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_Set_TestData))]
        public void TabControl_BackColor_SetWithHandle_GetReturnsExpected(Color value)
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.BackColor = value;
            Assert.Equal(SystemColors.Control, control.BackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.BackColor = value;
            Assert.Equal(SystemColors.Control, control.BackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void TabControl_BackColor_SetWithHandler_DoesNotCallBackColorChanged()
        {
            using var control = new TabControl();
            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            }
            control.BackColorChanged += handler;

            // Set different.
            control.BackColor = Color.Red;
            Assert.Equal(SystemColors.Control, control.BackColor);
            Assert.Equal(0, callCount);

            // Set same.
            control.BackColor = Color.Red;
            Assert.Equal(SystemColors.Control, control.BackColor);
            Assert.Equal(0, callCount);

            // Set different.
            control.BackColor = Color.Empty;
            Assert.Equal(SystemColors.Control, control.BackColor);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.BackColor = Color.Red;
            Assert.Equal(SystemColors.Control, control.BackColor);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void TabControl_BackgroundImage_Set_GetReturnsExpected(Image value)
        {
            using var control = new TabControl
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
        public void TabControl_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
        {
            using var control = new TabControl();
            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            }
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
        public void TabControl_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
        {
            using var control = new SubTabControl
            {
                BackgroundImageLayout = value
            };
            Assert.Equal(value, control.BackgroundImageLayout);
            Assert.False(control.DoubleBuffered);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackgroundImageLayout = value;
            Assert.Equal(value, control.BackgroundImageLayout);
            Assert.False(control.DoubleBuffered);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControl_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
        {
            using var control = new TabControl();
            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            }
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
        public void TabControl_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
        {
            using var control = new TabControl();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
        }

        [WinFormsFact]
        public void TabControl_DisplayRectangle_Get_ReturnsExpectedAndCreatesHandle()
        {
            using var control = new TabControl();
            Rectangle displayRectangle = control.DisplayRectangle;
            Assert.True(displayRectangle.X >= 0);
            Assert.True(displayRectangle.Y >= 0);
            Assert.Equal(200 - displayRectangle.X * 2, control.DisplayRectangle.Width);
            Assert.Equal(100 - displayRectangle.Y * 2, control.DisplayRectangle.Height);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(displayRectangle, control.DisplayRectangle);
        }

        [WinFormsFact]
        public void TabControl_DisplayRectangle_GetWithHandle_ReturnsExpected()
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Rectangle displayRectangle = control.DisplayRectangle;
            Assert.True(displayRectangle.X >= 0);
            Assert.True(displayRectangle.Y >= 0);
            Assert.Equal(200 - displayRectangle.X * 2, control.DisplayRectangle.Width);
            Assert.Equal(100 - displayRectangle.Y * 2, control.DisplayRectangle.Height);
            Assert.Equal(displayRectangle, control.DisplayRectangle);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void TabControl_DisplayRectangle_GetDisposed_ReturnsExpected()
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.Dispose();

            Rectangle displayRectangle = control.DisplayRectangle;
            Assert.True(displayRectangle.X >= 0);
            Assert.True(displayRectangle.Y >= 0);
            Assert.Equal(200 - displayRectangle.X * 2, control.DisplayRectangle.Width);
            Assert.Equal(100 - displayRectangle.Y * 2, control.DisplayRectangle.Height);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(displayRectangle, control.DisplayRectangle);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TabControl_DoubleBuffered_Get_ReturnsExpected(bool value)
        {
            using var control = new SubTabControl();
            control.SetStyle(ControlStyles.OptimizedDoubleBuffer, value);
            Assert.Equal(value, control.DoubleBuffered);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TabControl_DoubleBuffered_Set_GetReturnsExpected(bool value)
        {
            using var control = new SubTabControl
            {
                DoubleBuffered = value
            };
            Assert.Equal(value, control.DoubleBuffered);
            Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.DoubleBuffered = value;
            Assert.Equal(value, control.DoubleBuffered);
            Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.DoubleBuffered = !value;
            Assert.Equal(!value, control.DoubleBuffered);
            Assert.Equal(!value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TabControl_DoubleBuffered_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new SubTabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.DoubleBuffered = value;
            Assert.Equal(value, control.DoubleBuffered);
            Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.DoubleBuffered = value;
            Assert.Equal(value, control.DoubleBuffered);
            Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.DoubleBuffered = !value;
            Assert.Equal(!value, control.DoubleBuffered);
            Assert.Equal(!value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TabDrawMode))]
        public void TabControl_DrawMode_Set_GetReturnsExpected(TabDrawMode value)
        {
            using var control = new TabControl
            {
                DrawMode = value
            };
            Assert.Equal(value, control.DrawMode);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.DrawMode = value;
            Assert.Equal(value, control.DrawMode);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(TabDrawMode.Normal, 0)]
        [InlineData(TabDrawMode.OwnerDrawFixed, 1)]
        public void TabControl_DrawMode_SetWithHandle_GetReturnsExpected(TabDrawMode value, int expectedCreatedCallCount)
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.DrawMode = value;
            Assert.Equal(value, control.DrawMode);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.DrawMode = value;
            Assert.Equal(value, control.DrawMode);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TabDrawMode))]
        public void TabControl_DrawMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(TabDrawMode value)
        {
            using var control = new TabControl();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.DrawMode = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void TabControl_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new TabControl
            {
                ForeColor = value
            };
            Assert.Equal(expected, control.ForeColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> ForeColor_SetWithHandle_TestData()
        {
            yield return new object[] { Color.Red, Color.Red, 1 };
            yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3), 1 };
            yield return new object[] { Color.Empty, Control.DefaultForeColor, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(ForeColor_SetWithHandle_TestData))]
        public void TabControl_ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void TabControl_ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            using var control = new TabControl();
            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            }
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
            Assert.Equal(TabControl.DefaultForeColor, control.ForeColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ForeColorChanged -= handler;
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void TabControl_Handle_GetNoImageList_Success()
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(IntPtr.Zero, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.TCM.GETIMAGELIST, IntPtr.Zero, IntPtr.Zero));
        }

        [WinFormsFact]
        public void TabControl_Handle_GetWithImageList_Success()
        {
            using var imageList = new ImageList();
            using var control = new TabControl
            {
                ImageList = imageList
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(imageList.Handle, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.TCM.GETIMAGELIST, IntPtr.Zero, IntPtr.Zero));
        }

        [WinFormsFact]
        public void TabControl_Handle_GetItemsEmpty_Success()
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(IntPtr.Zero, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.TCM.GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero));
        }

        [WinFormsTheory]
        [InlineData("Text", "Text")]
        [InlineData("&&Text", "&&Text")]
        [InlineData("&", "&&")]
        [InlineData("&Text", "&&Text")]
        public unsafe void TabControl_Handle_GetItems_Success(string text, string expectedText)
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage
            {
                Text = text,
                ImageIndex = 1
            };
            using var page3 = new NullTextTabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            control.TabPages.Add(page3);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal((IntPtr)3, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.TCM.GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero));

            char* buffer = stackalloc char[256];
            ComCtl32.TCITEMW item = default;
            item.cchTextMax = int.MaxValue;
            item.pszText = buffer;
            item.dwStateMask = (ComCtl32.TCIS)uint.MaxValue;
            item.mask = (ComCtl32.TCIF)uint.MaxValue;

            // Get item 0.
            Assert.Equal((IntPtr)1, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.TCM.GETITEMW, (IntPtr)0, ref item));
            Assert.Equal(ComCtl32.TCIS.BUTTONPRESSED, item.dwState);
            Assert.Equal(IntPtr.Zero, item.lParam);
            Assert.Equal(int.MaxValue, item.cchTextMax);
            Assert.Empty(new string(item.pszText));
            Assert.Equal(-1, item.iImage);

            // Get item 1.
            Assert.Equal((IntPtr)1, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.TCM.GETITEMW, (IntPtr)1, ref item));
            Assert.Equal((ComCtl32.TCIS)0, item.dwState);
            Assert.Equal(IntPtr.Zero, item.lParam);
            Assert.Equal(int.MaxValue, item.cchTextMax);
            Assert.Equal(expectedText, new string(item.pszText));
            Assert.Equal(1, item.iImage);

            // Get item 2.
            Assert.Equal((IntPtr)1, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.TCM.GETITEMW, (IntPtr)2, ref item));
            Assert.Equal((ComCtl32.TCIS)0, item.dwState);
            Assert.Equal(IntPtr.Zero, item.lParam);
            Assert.Equal(int.MaxValue, item.cchTextMax);
            Assert.Empty(new string(item.pszText));
            Assert.Equal(-1, item.iImage);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TabControl_HotTrack_Set_GetReturnsExpected(bool value)
        {
            using var control = new SubTabControl
            {
                HotTrack = value
            };
            Assert.Equal(value, control.HotTrack);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.HotTrack = value;
            Assert.Equal(value, control.HotTrack);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.HotTrack = !value;
            Assert.Equal(!value, control.HotTrack);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void TabControl_HotTrack_SetWithHandle_GetReturnsExpected(bool value, int expectedCreatedCallCount)
        {
            using var control = new SubTabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.HotTrack = value;
            Assert.Equal(value, control.HotTrack);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.HotTrack = value;
            Assert.Equal(value, control.HotTrack);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set different.
            control.HotTrack = !value;
            Assert.Equal(!value, control.HotTrack);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount + 1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
        }

        public static IEnumerable<object[]> ImageList_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ImageList() };
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageList_Set_TestData))]
        public void TabControl_ImageList_Set_GetReturnsExpected(ImageList value)
        {
            using var control = new TabControl
            {
                ImageList = value
            };
            Assert.Same(value, control.ImageList);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ImageList = value;
            Assert.Same(value, control.ImageList);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageList_Set_TestData))]
        public void TabControl_ImageList_SetWithNonNullOldValue_GetReturnsExpected(ImageList value)
        {
            using var oldValue = new ImageList();
            using var control = new TabControl
            {
                ImageList = oldValue
            };

            control.ImageList = value;
            Assert.Same(value, control.ImageList);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ImageList = value;
            Assert.Same(value, control.ImageList);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageList_Set_TestData))]
        public void TabControl_ImageList_SetWithHandle_GetReturnsExpected(ImageList value)
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ImageList = value;
            Assert.Same(value, control.ImageList);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ImageList = value;
            Assert.Same(value, control.ImageList);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ImageList_Set_TestData))]
        public void TabControl_ImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected(ImageList value)
        {
            using var oldValue = new ImageList();
            using var control = new TabControl
            {
                ImageList = oldValue
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ImageList = value;
            Assert.Same(value, control.ImageList);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ImageList = value;
            Assert.Same(value, control.ImageList);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void TabControl_ImageList_SetWithTabPages_Success()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);

            using var imageList = new ImageList();
            using var image1 = new Bitmap(10, 10);
            using var image2 = new Bitmap(10, 10);
            using var image3 = new Bitmap(10, 10);
            imageList.Images.Add(image1);
            imageList.Images.Add(image2);
            imageList.Images.Add(image3);
            control.ImageList = imageList;
            Assert.Same(imageList, control.ImageList);
            Assert.False(control.IsHandleCreated);

            // Set null.
            control.ImageList = null;
            Assert.Null(control.ImageList);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControl_ImageList_Set_CreatesImageHandle()
        {
            using var control = new TabControl();
            using var imageList =  new ImageList();
            control.ImageList = imageList;
            Assert.True(imageList.HandleCreated);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControl_ImageList_SetGetImageListWithHandle_Success()
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            // Set non-null.
            using var imageList =  new ImageList();
            control.ImageList = imageList;
            Assert.True(imageList.HandleCreated);
            Assert.Equal(imageList.Handle, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.TCM.GETIMAGELIST, IntPtr.Zero, IntPtr.Zero));

            // Set null.
            control.ImageList = null;
            Assert.Equal(IntPtr.Zero, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.TCM.GETIMAGELIST, IntPtr.Zero, IntPtr.Zero));
        }

        [WinFormsFact]
        public void TabControl_ImageList_Dispose_DetachesFromTabControl()
        {
            using var imageList1 = new ImageList();
            using var imageList2 = new ImageList();
            using var control = new TabControl
            {
                ImageList = imageList1
            };
            Assert.Same(imageList1, control.ImageList);

            imageList1.Dispose();
            Assert.Null(control.ImageList);
            Assert.False(control.IsHandleCreated);

            // Make sure we detached the setter.
            control.ImageList = imageList2;
            imageList1.Dispose();
            Assert.Same(imageList2, control.ImageList);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControl_ImageList_DisposeWithHandle_DetachesFromTabControl()
        {
            using var imageList1 = new ImageList();
            using var imageList2 = new ImageList();
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ImageList = imageList1;
            Assert.Same(imageList1, control.ImageList);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            imageList1.Dispose();
            Assert.Null(control.ImageList);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Make sure we detached the setter.
            control.ImageList = imageList2;
            imageList1.Dispose();
            Assert.Same(imageList2, control.ImageList);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void TabControl_ImageList_RecreateHandle_Nop()
        {
            using var imageList1 = new ImageList();
            int recreateCallCount1 = 0;
            imageList1.RecreateHandle += (sender, e) => recreateCallCount1++;
            using var imageList2 = new ImageList();
            using var control = new TabControl
            {
                ImageList = imageList1
            };
            Assert.Same(imageList1, control.ImageList);
            Assert.Equal(0, recreateCallCount1);

            imageList1.ImageSize = new Size(1, 2);
            Assert.Equal(1, recreateCallCount1);
            Assert.Same(imageList1, control.ImageList);
            Assert.False(control.IsHandleCreated);

            // Make sure we detached the setter.
            control.ImageList = imageList2;
            imageList1.ImageSize = new Size(2, 3);
            Assert.Equal(2, recreateCallCount1);
            Assert.Same(imageList2, control.ImageList);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControl_ImageList_RecreateHandleWithHandle_Success()
        {
            using var imageList1 = new ImageList();
            int recreateCallCount1 = 0;
            imageList1.RecreateHandle += (sender, e) => recreateCallCount1++;
            using var imageList2 = new ImageList();
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ImageList = imageList1;
            Assert.Same(imageList1, control.ImageList);
            Assert.Equal(0, recreateCallCount1);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            imageList1.ImageSize = new Size(1, 2);
            Assert.Equal(1, recreateCallCount1);
            Assert.Same(imageList1, control.ImageList);
            Assert.Equal(imageList1.Handle, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.TCM.GETIMAGELIST, IntPtr.Zero, IntPtr.Zero));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Make sure we detached the setter.
            control.ImageList = imageList2;
            imageList1.ImageSize = new Size(2, 3);
            Assert.Equal(2, recreateCallCount1);
            Assert.Same(imageList2, control.ImageList);
            Assert.Equal(imageList2.Handle, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.TCM.GETIMAGELIST, IntPtr.Zero, IntPtr.Zero));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void TabControl_ItemSize_GetEmptyWithHandle_ReturnsExpected()
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Size size = control.ItemSize;
            Assert.Equal(0, size.Width);
            Assert.True(size.Height > 0);
            Assert.Equal(size, control.ItemSize);
        }

        [WinFormsFact]
        public void TabControl_ItemSize_GetNotEmptyWithHandle_ReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Size size = control.ItemSize;
            Assert.True(size.Width > 0);
            Assert.True(size.Height > 0);
            Assert.Equal(size, control.ItemSize);
        }

        public static IEnumerable<object[]> ItemSize_Set_TestData()
        {
            yield return new object[] { new Size(0, 0) };
            yield return new object[] { new Size(0, 16) };
            yield return new object[] { new Size(16, 0) };
            yield return new object[] { new Size(16, 16) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ItemSize_Set_TestData))]
        public void TabControl_ItemSize_Set_GetReturnsExpected(Size value)
        {
            using var control = new TabControl
            {
                ItemSize = value
            };
            Assert.Equal(value, control.ItemSize);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ItemSize = value;
            Assert.Equal(value, control.ItemSize);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> ItemSize_SetWithHandle_TestData()
        {
            yield return new object[] { new Size(0, 16) };
            yield return new object[] { new Size(16, 0) };
            yield return new object[] { new Size(16, 16) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ItemSize_SetWithHandle_TestData))]
        public void TabControl_ItemSize_SetWithHandle_GetReturnsExpected(Size value)
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ItemSize = value;
            Assert.Equal(value, control.ItemSize);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ItemSize = value;
            Assert.Equal(value, control.ItemSize);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(4, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void TabControl_SetEmptyWithHandle_GetReturnsExpected()
        {
            using var control = new TabControl
            {
                ItemSize = new Size(16, 16)
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ItemSize = Size.Empty;
            Size size = control.ItemSize;
            Assert.Equal(0, size.Width);
            Assert.True(size.Height > 0);
            Assert.Equal(size, control.ItemSize);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void TabControl_ItemSize_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TabControl))[nameof(TabControl.ItemSize)];
            using var control = new TabControl();
            Assert.False(property.CanResetValue(control));

            control.ItemSize = new Size(1, 0);
            Assert.Equal(new Size(1, 0), control.ItemSize);
            Assert.True(property.CanResetValue(control));

            control.ItemSize = new Size(0, 1);
            Assert.Equal(new Size(0, 1), control.ItemSize);
            Assert.True(property.CanResetValue(control));

            control.ItemSize = new Size(1, 2);
            Assert.Equal(new Size(1, 2), control.ItemSize);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(Size.Empty, control.ItemSize);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void TabControl_ItemSize_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TabControl))[nameof(TabControl.ItemSize)];
            using var control = new TabControl();
            Assert.False(property.ShouldSerializeValue(control));

            control.ItemSize = new Size(1, 0);
            Assert.Equal(new Size(1, 0), control.ItemSize);
            Assert.True(property.ShouldSerializeValue(control));

            control.ItemSize = new Size(0, 1);
            Assert.Equal(new Size(0, 1), control.ItemSize);
            Assert.True(property.ShouldSerializeValue(control));

            control.ItemSize = new Size(1, 2);
            Assert.Equal(new Size(1, 2), control.ItemSize);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(Size.Empty, control.ItemSize);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsFact]
        public void TabControl_ItemSize_SetNegativeWidth_ThrowsArgumentOutOfRangeException()
        {
            using var control = new TabControl();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.ItemSize = new Size(-1, 1));
        }

        [WinFormsFact]
        public void TabControl_ItemSize_SetNegativeHeight_ThrowsArgumentOutOfRangeException()
        {
            using var control = new TabControl();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.ItemSize = new Size(1, -1));
        }

        public static IEnumerable<object[]> Multiline_Set_TestData()
        {
            yield return new object[] { TabAlignment.Bottom, true, TabAlignment.Bottom, TabAlignment.Bottom };
            yield return new object[] { TabAlignment.Left, true, TabAlignment.Left, TabAlignment.Top };
            yield return new object[] { TabAlignment.Right, true, TabAlignment.Right, TabAlignment.Top };
            yield return new object[] { TabAlignment.Top, true, TabAlignment.Top, TabAlignment.Top };
            yield return new object[] { TabAlignment.Bottom, false, TabAlignment.Bottom, TabAlignment.Bottom };
            yield return new object[] { TabAlignment.Left, false, TabAlignment.Top, TabAlignment.Top };
            yield return new object[] { TabAlignment.Right, false, TabAlignment.Top, TabAlignment.Top };
            yield return new object[] { TabAlignment.Top, false, TabAlignment.Top, TabAlignment.Top };
        }

        [WinFormsTheory]
        [MemberData(nameof(Multiline_Set_TestData))]
        public void TabControl_Multiline_Set_GetReturnsExpected(TabAlignment alignment, bool value, TabAlignment expectedAlignment1, TabAlignment expectedAlignment2)
        {
            using var control = new SubTabControl
            {
                Alignment = alignment,
                Multiline = value
            };
            Assert.Equal(value, control.Multiline);
            Assert.Equal(expectedAlignment1, control.Alignment);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Multiline = value;
            Assert.Equal(value, control.Multiline);
            Assert.Equal(expectedAlignment1, control.Alignment);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.Multiline = !value;
            Assert.Equal(!value, control.Multiline);
            Assert.Equal(expectedAlignment2, control.Alignment);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Multiline_SetWithHandle_TestData()
        {
            yield return new object[] { TabAlignment.Bottom, true, TabAlignment.Bottom, 1, TabAlignment.Bottom };
            yield return new object[] { TabAlignment.Left, true, TabAlignment.Left, 0, TabAlignment.Top };
            yield return new object[] { TabAlignment.Right, true, TabAlignment.Right, 0, TabAlignment.Top };
            yield return new object[] { TabAlignment.Top, true, TabAlignment.Top, 1, TabAlignment.Top };
            yield return new object[] { TabAlignment.Bottom, false, TabAlignment.Bottom, 0, TabAlignment.Bottom };
            yield return new object[] { TabAlignment.Left, false, TabAlignment.Top, 1, TabAlignment.Top };
            yield return new object[] { TabAlignment.Right, false, TabAlignment.Top, 1, TabAlignment.Top };
            yield return new object[] { TabAlignment.Top, false, TabAlignment.Top, 0, TabAlignment.Top };
        }

        [WinFormsTheory]
        [MemberData(nameof(Multiline_SetWithHandle_TestData))]
        public void TabControl_Multiline_SetWithHandle_GetReturnsExpected(TabAlignment alignment, bool value, TabAlignment expectedAlignment1, int expectedCreatedCallCount, TabAlignment expectedAlignment2)
        {
            using var control = new SubTabControl
            {
                Alignment = alignment
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Multiline = value;
            Assert.Equal(value, control.Multiline);
            Assert.Equal(expectedAlignment1, control.Alignment);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.Multiline = value;
            Assert.Equal(value, control.Multiline);
            Assert.Equal(expectedAlignment1, control.Alignment);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set different.
            control.Multiline = !value;
            Assert.Equal(expectedAlignment2, control.Alignment);
            Assert.Equal(!value, control.Multiline);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount + 1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
        }

        public static IEnumerable<object[]> Padding_Set_TestData()
        {
            yield return new object[] { new Point(0, 0) };
            yield return new object[] { new Point(0, 16) };
            yield return new object[] { new Point(16, 0) };
            yield return new object[] { new Point(16, 16) };
        }

        [WinFormsTheory]
        [MemberData(nameof(Padding_Set_TestData))]
        public void TabControl_Padding_Set_GetReturnsExpected(Point value)
        {
            using var control = new TabControl
            {
                Padding = value
            };
            Assert.Equal(value, control.Padding);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Padding = value;
            Assert.Equal(value, control.Padding);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Padding_Set_TestData))]
        public void TabControl_Padding_SetWithHandle_GetReturnsExpected(Point value)
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Padding = value;
            Assert.Equal(value, control.Padding);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(1, createdCallCount);

            // Set same.
            control.Padding = value;
            Assert.Equal(value, control.Padding);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(1, createdCallCount);
        }

        [WinFormsFact]
        public void TabControl_Padding_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TabControl))[nameof(TabControl.Padding)];
            using var control = new TabControl();
            Assert.False(property.CanResetValue(control));

            control.Padding = new Point(1, 0);
            Assert.Equal(new Point(1, 0), control.Padding);
            Assert.True(property.CanResetValue(control));

            control.Padding = new Point(0, 1);
            Assert.Equal(new Point(0, 1), control.Padding);
            Assert.True(property.CanResetValue(control));

            control.Padding = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Padding);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(new Point(6, 3), control.Padding);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void TabControl_Padding_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TabControl))[nameof(TabControl.Padding)];
            using var control = new TabControl();
            Assert.False(property.ShouldSerializeValue(control));

            control.Padding = new Point(1, 0);
            Assert.Equal(new Point(1, 0), control.Padding);
            Assert.True(property.ShouldSerializeValue(control));

            control.Padding = new Point(0, 1);
            Assert.Equal(new Point(0, 1), control.Padding);
            Assert.True(property.ShouldSerializeValue(control));

            control.Padding = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Padding);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(new Point(6, 3), control.Padding);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsFact]
        public void TabControl_Padding_SetNegativeX_ThrowsArgumentOutOfRangeException()
        {
            using var control = new TabControl();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.Padding = new Point(-1, 1));
        }

        [WinFormsFact]
        public void TabControl_Padding_SetNegativeY_ThrowsArgumentOutOfRangeException()
        {
            using var control = new TabControl();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.Padding = new Point(1, -1));
        }

        [WinFormsTheory]
        [InlineData(RightToLeft.Yes, true, 1)]
        [InlineData(RightToLeft.Yes, false, 0)]
        [InlineData(RightToLeft.No, true, 1)]
        [InlineData(RightToLeft.No, false, 0)]
        [InlineData(RightToLeft.Inherit, true, 1)]
        [InlineData(RightToLeft.Inherit, false, 0)]
        public void TabControl_RightToLeftLayout_Set_GetReturnsExpected(RightToLeft rightToLeft, bool value, int expectedLayoutCallCount)
        {
            using var control = new TabControl
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
        public void TabControl_RightToLeftLayout_SetWithHandle_GetReturnsExpected(RightToLeft rightToLeft, bool value, int expectedLayoutCallCount, int expectedCreatedCallCount1, int expectedCreatedCallCount2)
        {
            using var control = new TabControl
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
            Assert.Equal(expectedCreatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount1, createdCallCount);

            // Set same.
            control.RightToLeftLayout = value;
            Assert.Equal(value, control.RightToLeftLayout);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount1, createdCallCount);

            // Set different.
            control.RightToLeftLayout = !value;
            Assert.Equal(!value, control.RightToLeftLayout);
            Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount2, createdCallCount);
        }

        [WinFormsFact]
        public void TabControl_RightToLeftLayout_SetWithHandler_CallsRightToLeftLayoutChanged()
        {
            using var control = new TabControl
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
        public void TabControl_RightToLeftLayout_SetWithHandlerInDisposing_DoesNotRightToLeftLayoutChanged()
        {
            using var control = new TabControl
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

        [WinFormsFact]
        public void TabControl_RowCount_Get_ReturnsExpectedAndCreatesHandle()
        {
            using var control = new TabControl();
            Assert.Equal(0, control.RowCount);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControl_RowCount_GetWithHandle_ReturnsExpected()
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(0, control.RowCount);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControl_SelectedIndex_GetWithHandle_ReturnsExpected()
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(-1, control.SelectedIndex);
        }

        [WinFormsFact]
        public void TabControl_SelectedIndex_GetWithPagesWithHandle_ReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(0, control.SelectedIndex);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void TabControl_SelectedIndex_Set_GetReturnsExpected(int value)
        {
            using var control = new TabControl
            {
                SelectedIndex = value
            };
            Assert.Equal(value, control.SelectedIndex);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectedIndex = value;
            Assert.Equal(value, control.SelectedIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void TabControl_SelectedIndex_SetWithPages_GetReturnsExpected(int value)
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);

            control.SelectedIndex = value;
            Assert.Equal(value, control.SelectedIndex);
            Assert.Equal(value < 0 || value >= control.TabPages.Count ? null : control.TabPages[value], control.SelectedTab);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);

            // Set same.
            control.SelectedIndex = value;
            Assert.Equal(value, control.SelectedIndex);
            Assert.Equal(value < 0 || value >= control.TabPages.Count ? null : control.TabPages[value], control.SelectedTab);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void TabControl_SelectedIndex_SetWithHandle_GetReturnsExpected(int value)
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SelectedIndex = value;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedTab);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.SelectedIndex = value;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedTab);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(-1, -1, new bool[] { false, false })]
        [InlineData(0, 0, new bool[] { true, false })]
        [InlineData(1, 1, new bool[] { false, true })]
        [InlineData(2, 0, new bool[] { true, false })]
        public void TabControl_SelectedIndex_SetWithPagesWithHandle_GetReturnsExpected(int value, int expected, bool[] expectedVisible)
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SelectedIndex = value;
            Assert.Equal(expected, control.SelectedIndex);
            Assert.Equal(expected == -1 ? null : control.TabPages[expected], control.SelectedTab);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedVisible, control.TabPages.Cast<TabPage>().Select(p => p.Visible));
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.SelectedIndex = value;
            Assert.Equal(expected, control.SelectedIndex);
            Assert.Equal(expected == -1 ? null : control.TabPages[expected], control.SelectedTab);
            Assert.Equal(expectedVisible, control.TabPages.Cast<TabPage>().Select(p => p.Visible));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void TabControl_SelectedIndex_SetWithHandler_CallsSelectedIndexChanged()
        {
            using var control = new TabControl();
            int deselectingCallCount = 0;
            int deselectedCallCount = 0;
            int selectingCallCount = 0;
            int selectedCallCount = 0;
            int selectedIndexChangedCallCount = 0;
            void deselectingHandler(object sender, TabControlCancelEventArgs e) => deselectingCallCount++;
            void deselectedHandler(object sender, TabControlEventArgs e) => deselectedCallCount++;
            void selectingHandler(object sender, TabControlCancelEventArgs e) => selectingCallCount++;
            void selectedHandler(object sender, TabControlEventArgs e) => selectedCallCount++;
            void selectedIndexChangedHandler(object sender, EventArgs e) => selectedIndexChangedCallCount++;
            control.Deselecting += deselectingHandler;
            control.Deselected += deselectedHandler;
            control.Selecting += selectingHandler;
            control.Selected += selectedHandler;
            control.SelectedIndexChanged += selectedIndexChangedHandler;

            // Set different.
            control.SelectedIndex = 0;
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal(0, deselectingCallCount);
            Assert.Equal(0, deselectedCallCount);
            Assert.Equal(0, selectingCallCount);
            Assert.Equal(0, selectedCallCount);
            Assert.Equal(0, selectedIndexChangedCallCount);

            // Set same.
            control.SelectedIndex = 0;
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal(0, deselectingCallCount);
            Assert.Equal(0, deselectedCallCount);
            Assert.Equal(0, selectingCallCount);
            Assert.Equal(0, selectedCallCount);
            Assert.Equal(0, selectedIndexChangedCallCount);

            // Set different.
            control.SelectedIndex = 1;
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal(0, deselectingCallCount);
            Assert.Equal(0, deselectedCallCount);
            Assert.Equal(0, selectingCallCount);
            Assert.Equal(0, selectedCallCount);
            Assert.Equal(0, selectedIndexChangedCallCount);

            // Remove handler.
            control.Deselecting -= deselectingHandler;
            control.Deselected -= deselectedHandler;
            control.Selecting -= selectingHandler;
            control.Selected -= selectedHandler;
            control.SelectedIndexChanged -= selectedIndexChangedHandler;
            control.SelectedIndex = 0;
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal(0, deselectingCallCount);
            Assert.Equal(0, deselectedCallCount);
            Assert.Equal(0, selectingCallCount);
            Assert.Equal(0, selectedCallCount);
            Assert.Equal(0, selectedIndexChangedCallCount);
        }

        [WinFormsFact]
        public void TabControl_SelectedIndex_SetWithHandleWithHandler_CallsSelectedIndexChanged()
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int deselectingCallCount = 0;
            int deselectedCallCount = 0;
            int selectingCallCount = 0;
            int selectedCallCount = 0;
            int selectedIndexChangedCallCount = 0;
            void deselectingHandler(object sender, TabControlCancelEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Equal(TabControlAction.Deselecting, e.Action);
                Assert.False(e.Cancel);
                Assert.Null(e.TabPage);
                Assert.Equal(control.SelectedIndex, e.TabPageIndex);
                Assert.Equal(deselectingCallCount, deselectedCallCount);
                Assert.Equal(deselectingCallCount, selectingCallCount);
                Assert.Equal(deselectingCallCount, selectedCallCount);
                Assert.Equal(deselectingCallCount, selectedIndexChangedCallCount);
                deselectingCallCount++;
            }
            void deselectedHandler(object sender, TabControlEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Equal(TabControlAction.Deselected, e.Action);
                Assert.Null(e.TabPage);
                Assert.Equal(control.SelectedIndex, e.TabPageIndex);
                Assert.Equal(deselectedCallCount, selectingCallCount);
                Assert.Equal(deselectedCallCount, selectedCallCount);
                Assert.Equal(deselectedCallCount, selectedIndexChangedCallCount);
                deselectedCallCount++;
            }
            void selectingHandler(object sender, TabControlCancelEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Equal(TabControlAction.Selecting, e.Action);
                Assert.False(e.Cancel);
                Assert.Null(e.TabPage);
                Assert.Equal(control.SelectedIndex, e.TabPageIndex);
                Assert.Equal(selectingCallCount, selectedCallCount);
                Assert.Equal(selectingCallCount, selectedIndexChangedCallCount);
                selectingCallCount++;
            }
            void selectedHandler(object sender, TabControlEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Equal(TabControlAction.Selected, e.Action);
                Assert.Null(e.TabPage);
                Assert.Equal(control.SelectedIndex, e.TabPageIndex);
                Assert.Equal(selectedCallCount, selectedIndexChangedCallCount);
                selectedCallCount++;
            }
            void selectedIndexChangedHandler(object sender, EventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                selectedIndexChangedCallCount++;
            }
            control.Deselecting += deselectingHandler;
            control.Deselected += deselectedHandler;
            control.Selecting += selectingHandler;
            control.Selected += selectedHandler;
            control.SelectedIndexChanged += selectedIndexChangedHandler;

            // Set different.
            control.SelectedIndex = 0;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(1, deselectingCallCount);
            Assert.Equal(1, deselectedCallCount);
            Assert.Equal(1, selectingCallCount);
            Assert.Equal(1, selectedCallCount);
            Assert.Equal(1, selectedIndexChangedCallCount);

            // Set same.
            control.SelectedIndex = 0;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(2, deselectingCallCount);
            Assert.Equal(2, deselectedCallCount);
            Assert.Equal(2, selectingCallCount);
            Assert.Equal(2, selectedCallCount);
            Assert.Equal(2, selectedIndexChangedCallCount);

            // Set different.
            control.SelectedIndex = 1;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(3, deselectingCallCount);
            Assert.Equal(3, deselectedCallCount);
            Assert.Equal(3, selectingCallCount);
            Assert.Equal(3, selectedCallCount);
            Assert.Equal(3, selectedIndexChangedCallCount);

            // Remove handler.
            control.Deselecting -= deselectingHandler;
            control.Deselected -= deselectedHandler;
            control.Selecting -= selectingHandler;
            control.Selected -= selectedHandler;
            control.SelectedIndexChanged -= selectedIndexChangedHandler;
            control.SelectedIndex = 0;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(3, deselectingCallCount);
            Assert.Equal(3, deselectedCallCount);
            Assert.Equal(3, selectingCallCount);
            Assert.Equal(3, selectedCallCount);
            Assert.Equal(3, selectedIndexChangedCallCount);
        }

        [WinFormsFact]
        public void TabControl_SelectedIndex_SetWithHandleWithHandlerCancelDeselecting_DoesNotCallSelectedIndexChanged()
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int deselectingCallCount = 0;
            int deselectedCallCount = 0;
            int selectingCallCount = 0;
            int selectedCallCount = 0;
            int selectedIndexChangedCallCount = 0;
            void deselectingHandler(object sender, TabControlCancelEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Equal(TabControlAction.Deselecting, e.Action);
                Assert.False(e.Cancel);
                Assert.Null(e.TabPage);
                Assert.Equal(control.SelectedIndex, e.TabPageIndex);
                Assert.Equal(0, deselectedCallCount);
                Assert.Equal(0, selectingCallCount);
                Assert.Equal(0, selectedCallCount);
                Assert.Equal(0, selectedIndexChangedCallCount);
                deselectingCallCount++;

                e.Cancel = true;
            }
            void deselectedHandler(object sender, TabControlEventArgs e) => deselectedCallCount++;
            void selectingHandler(object sender, TabControlCancelEventArgs e) => selectingCallCount++;
            void selectedHandler(object sender, TabControlEventArgs e) => selectedCallCount++;
            void selectedIndexChangedHandler(object sender, EventArgs e) => selectedIndexChangedCallCount++;
            control.Deselecting += deselectingHandler;
            control.Deselected += deselectedHandler;
            control.Selecting += selectingHandler;
            control.Selected += selectedHandler;
            control.SelectedIndexChanged += selectedIndexChangedHandler;

            // Set different.
            control.SelectedIndex = 0;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(1, deselectingCallCount);
            Assert.Equal(0, deselectedCallCount);
            Assert.Equal(0, selectingCallCount);
            Assert.Equal(0, selectedCallCount);
            Assert.Equal(0, selectedIndexChangedCallCount);

            // Set same.
            control.SelectedIndex = 0;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(2, deselectingCallCount);
            Assert.Equal(0, deselectedCallCount);
            Assert.Equal(0, selectingCallCount);
            Assert.Equal(0, selectedCallCount);
            Assert.Equal(0, selectedIndexChangedCallCount);

            // Set different.
            control.SelectedIndex = 1;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(3, deselectingCallCount);
            Assert.Equal(0, deselectedCallCount);
            Assert.Equal(0, selectingCallCount);
            Assert.Equal(0, selectedCallCount);
            Assert.Equal(0, selectedIndexChangedCallCount);

            // Remove handler.
            control.Deselecting -= deselectingHandler;
            control.Deselected -= deselectedHandler;
            control.Selecting -= selectingHandler;
            control.Selected -= selectedHandler;
            control.SelectedIndexChanged -= selectedIndexChangedHandler;
            control.SelectedIndex = 0;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(3, deselectingCallCount);
            Assert.Equal(0, deselectedCallCount);
            Assert.Equal(0, selectingCallCount);
            Assert.Equal(0, selectedCallCount);
            Assert.Equal(0, selectedIndexChangedCallCount);
        }

        [WinFormsFact]
        public void TabControl_SelectedIndex_SetWithHandleWithHandlerCancelSelecting_DoesNotCallSelectedIndexChanged()
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int deselectingCallCount = 0;
            int deselectedCallCount = 0;
            int selectingCallCount = 0;
            int selectedCallCount = 0;
            int selectedIndexChangedCallCount = 0;
            void deselectingHandler(object sender, TabControlCancelEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Equal(TabControlAction.Deselecting, e.Action);
                Assert.False(e.Cancel);
                Assert.Null(e.TabPage);
                Assert.Equal(control.SelectedIndex, e.TabPageIndex);
                Assert.Equal(deselectingCallCount, deselectedCallCount);
                Assert.Equal(deselectingCallCount, selectingCallCount);
                Assert.Equal(0, selectedCallCount);
                Assert.Equal(0, selectedIndexChangedCallCount);
                deselectingCallCount++;
            }
            void deselectedHandler(object sender, TabControlEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Equal(TabControlAction.Deselected, e.Action);
                Assert.Null(e.TabPage);
                Assert.Equal(control.SelectedIndex, e.TabPageIndex);
                Assert.Equal(deselectedCallCount, selectingCallCount);
                Assert.Equal(0, selectedCallCount);
                Assert.Equal(0, selectedIndexChangedCallCount);
                deselectedCallCount++;
            }
            void selectingHandler(object sender, TabControlCancelEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Equal(TabControlAction.Selecting, e.Action);
                Assert.False(e.Cancel);
                Assert.Null(e.TabPage);
                Assert.Equal(control.SelectedIndex, e.TabPageIndex);
                Assert.Equal(0, selectedCallCount);
                Assert.Equal(0, selectedIndexChangedCallCount);
                selectingCallCount++;

                e.Cancel = true;
            }
            void selectedHandler(object sender, TabControlEventArgs e) => selectedCallCount++;
            void selectedIndexChangedHandler(object sender, EventArgs e) => selectedIndexChangedCallCount++;
            control.Deselecting += deselectingHandler;
            control.Deselected += deselectedHandler;
            control.Selecting += selectingHandler;
            control.Selected += selectedHandler;
            control.SelectedIndexChanged += selectedIndexChangedHandler;

            // Set different.
            control.SelectedIndex = 0;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(1, deselectingCallCount);
            Assert.Equal(1, deselectedCallCount);
            Assert.Equal(1, selectingCallCount);
            Assert.Equal(0, selectedCallCount);
            Assert.Equal(0, selectedIndexChangedCallCount);

            // Set same.
            control.SelectedIndex = 0;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(2, deselectingCallCount);
            Assert.Equal(2, deselectedCallCount);
            Assert.Equal(2, selectingCallCount);
            Assert.Equal(0, selectedCallCount);
            Assert.Equal(0, selectedIndexChangedCallCount);

            // Set different.
            control.SelectedIndex = 1;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(3, deselectingCallCount);
            Assert.Equal(3, deselectedCallCount);
            Assert.Equal(3, selectingCallCount);
            Assert.Equal(0, selectedCallCount);
            Assert.Equal(0, selectedIndexChangedCallCount);

            // Remove handler.
            control.Deselecting -= deselectingHandler;
            control.Deselected -= deselectedHandler;
            control.Selecting -= selectingHandler;
            control.Selected -= selectedHandler;
            control.SelectedIndexChanged -= selectedIndexChangedHandler;
            control.SelectedIndex = 0;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(3, deselectingCallCount);
            Assert.Equal(3, deselectedCallCount);
            Assert.Equal(3, selectingCallCount);
            Assert.Equal(0, selectedCallCount);
            Assert.Equal(0, selectedIndexChangedCallCount);
        }

        [WinFormsFact]
        public void TabControl_SelectedIndex_SetInvalid_ThrowsArgumentOutOfRangeException()
        {
            using var control = new TabControl();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.SelectedIndex = -2);
        }

        [WinFormsFact]
        public void TabControl_SelectedTab_GetWithHandle_ReturnsExpected()
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Null(control.SelectedTab);
        }

        [WinFormsFact]
        public void TabControl_SelectedTab_GetWithPagesWithHandle_ReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Same(page1, control.SelectedTab);
            Assert.True(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void TabControl_SelectedTab_GetWithInvalidIndexNotGotPages_ReturnsNull(int value)
        {
            using var control = new TabControl
            {
                SelectedIndex = value
            };
            Assert.Null(control.SelectedTab);

            Assert.Empty(control.TabPages);
            Assert.Null(control.SelectedTab);
        }

        public static IEnumerable<object[]> SelectedTab_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new TabPage() };
        }

        [WinFormsTheory]
        [MemberData(nameof(SelectedTab_Set_TestData))]
        public void TabControl_SelectedTab_SetWithoutPages_GetReturnsExpected(TabPage value)
        {
            using var control = new TabControl
            {
                SelectedTab = value
            };
            Assert.Null(control.SelectedTab);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectedTab = value;
            Assert.Null(control.SelectedTab);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControl_SelectedTab_SetWithPages_GetReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            using var page3 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);

            // Set valid.
            control.SelectedTab = page2;
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Set same.
            control.SelectedTab = page2;
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Set invalid.
            control.SelectedTab = page3;
            Assert.Null(control.SelectedTab);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Set different.
            control.SelectedTab = page1;
            Assert.Same(page1, control.SelectedTab);
            Assert.Equal(0, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Set invalid.
            control.SelectedTab = null;
            Assert.Null(control.SelectedTab);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(SelectedTab_Set_TestData))]
        public void TabControl_SelectedTab_SetWithoutPagesWithHandle_GetReturnsExpected(TabPage value)
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SelectedTab = value;
            Assert.Null(control.SelectedTab);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.SelectedTab = value;
            Assert.Null(control.SelectedTab);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void TabControl_SelectedTab_SetWithPagesWithHandle_GetReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            using var page3 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int pageInvalidatedCallCount1 = 0;
            page1.Invalidated += (sender, e) => pageInvalidatedCallCount1++;
            int pageStyleChangedCallCount1 = 0;
            page1.StyleChanged += (sender, e) => pageStyleChangedCallCount1++;
            int pageCreatedCallCount1 = 0;
            page1.HandleCreated += (sender, e) => pageCreatedCallCount1++;
            int pageInvalidatedCallCount2 = 0;
            page2.Invalidated += (sender, e) => pageInvalidatedCallCount2++;
            int pageStyleChangedCallCount2 = 0;
            page2.StyleChanged += (sender, e) => pageStyleChangedCallCount2++;
            int pageCreatedCallCount2 = 0;
            page2.HandleCreated += (sender, e) => pageCreatedCallCount2++;
            Assert.True(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Set valid.
            control.SelectedTab = page2;
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.True(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Set same.
            control.SelectedTab = page2;
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.True(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Set invalid.
            control.SelectedTab = page3;
            Assert.Null(control.SelectedTab);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Set different.
            control.SelectedTab = page1;
            Assert.Same(page1, control.SelectedTab);
            Assert.Equal(0, control.SelectedIndex);
            Assert.True(page1.Visible);
            Assert.False(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Set invalid.
            control.SelectedTab = null;
            Assert.Null(control.SelectedTab);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TabSizeMode))]
        public void TabControl_SizeMode_Set_GetReturnsExpected(TabSizeMode value)
        {
            using var control = new TabControl
            {
                SizeMode = value
            };
            Assert.Equal(value, control.SizeMode);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SizeMode = value;
            Assert.Equal(value, control.SizeMode);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(TabSizeMode.Normal, 0)]
        [InlineData(TabSizeMode.FillToRight, 1)]
        [InlineData(TabSizeMode.Fixed, 1)]
        public void TabControl_SizeMode_SetWithHandle_GetReturnsExpected(TabSizeMode value, int expectedCreatedCallCount)
        {
            using var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SizeMode = value;
            Assert.Equal(value, control.SizeMode);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.SizeMode = value;
            Assert.Equal(value, control.SizeMode);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TabSizeMode))]
        public void TabControl_SizeMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(TabSizeMode value)
        {
            using var control = new TabControl();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.SizeMode = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TabControl_ShowToolTips_Set_GetReturnsExpected(bool value)
        {
            using var control = new SubTabControl
            {
                ShowToolTips = value
            };
            Assert.Equal(value, control.ShowToolTips);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ShowToolTips = value;
            Assert.Equal(value, control.ShowToolTips);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.ShowToolTips = !value;
            Assert.Equal(!value, control.ShowToolTips);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void TabControl_ShowToolTips_SetWithHandle_GetReturnsExpected(bool value, int expectedCreatedCallCount)
        {
            using var control = new SubTabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ShowToolTips = value;
            Assert.Equal(value, control.ShowToolTips);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.ShowToolTips = value;
            Assert.Equal(value, control.ShowToolTips);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set different.
            control.ShowToolTips = !value;
            Assert.Equal(!value, control.ShowToolTips);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount + 1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void TabControl_Text_Set_GetReturnsExpected(string value, string expected)
        {
            using var control = new TabControl
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
        public void TabControl_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var control = new TabControl();
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
        public void TabControl_Text_SetWithHandler_CallsTextChanged()
        {
            using var control = new TabControl();
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
        public void TabControl_CreateControlsInstance_Invoke_ReturnsExpected()
        {
            using var control = new SubTabControl();
            Control.ControlCollection controls = Assert.IsType<TabControl.ControlCollection>(control.CreateControlsInstance());
            Assert.Empty(controls);
            Assert.Same(control, controls.Owner);
            Assert.False(controls.IsReadOnly);
            Assert.NotSame(controls, control.CreateControlsInstance());
        }

        [WinFormsFact]
        public void TabControl_CreateHandle_Invoke_Success()
        {
            using var control = new SubTabControl();
            control.CreateHandle();
            Assert.False(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
        }

        [WinFormsFact]
        public void TabControl_DeselectTab_InvokeTabPageWithoutHandle_GetReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);

            // Deselect first.
            control.DeselectTab(page1);
            Assert.Null(control.SelectedTab);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Select and deselect first.
            control.SelectTab(page1);
            control.DeselectTab(page1);
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Deselect again.
            control.DeselectTab(page1);
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Deselect last.
            control.DeselectTab(page2);
            Assert.Same(page1, control.SelectedTab);
            Assert.Equal(0, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControl_DeselectTab_InvokeTabPageWithHandle_GetReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int pageInvalidatedCallCount1 = 0;
            page1.Invalidated += (sender, e) => pageInvalidatedCallCount1++;
            int pageStyleChangedCallCount1 = 0;
            page1.StyleChanged += (sender, e) => pageStyleChangedCallCount1++;
            int pageCreatedCallCount1 = 0;
            page1.HandleCreated += (sender, e) => pageCreatedCallCount1++;
            int pageInvalidatedCallCount2 = 0;
            page2.Invalidated += (sender, e) => pageInvalidatedCallCount2++;
            int pageStyleChangedCallCount2 = 0;
            page2.StyleChanged += (sender, e) => pageStyleChangedCallCount2++;
            int pageCreatedCallCount2 = 0;
            page2.HandleCreated += (sender, e) => pageCreatedCallCount2++;
            Assert.True(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Deselect first.
            control.DeselectTab(page1);
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.True(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Select and deselect first.
            control.SelectTab(0);
            control.DeselectTab(page1);
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.True(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Deselect again.
            control.DeselectTab(page1);
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.True(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Deselect last.
            control.DeselectTab(page2);
            Assert.Same(page1, control.SelectedTab);
            Assert.Equal(0, control.SelectedIndex);
            Assert.True(page1.Visible);
            Assert.False(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(2, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);
        }

        [WinFormsFact]
        public void TabControl_DeselectTab_InvalidTabPageNameWithoutPages_ThrowsArgumentOutOfRangeException()
        {
            using var control = new TabControl();
            using var page = new TabPage();
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.DeselectTab(page));
        }

        [WinFormsFact]
        public void TabControl_DeselectTab_InvalidTabPageNameWithPages_ThrowsArgumentOutOfRangeException()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            using var page3 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.DeselectTab(page3));
        }

        [WinFormsFact]
        public void TabControl_DeselectTab_InvokeStringWithoutHandle_GetReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage
            {
                Name = "Name1"
            };
            using var page2 = new TabPage
            {
                Name = "Name2"
            };
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);

            // Deselect first.
            control.DeselectTab("Name1");
            Assert.Null(control.SelectedTab);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Select and deselect first.
            control.SelectTab("Name1");
            control.DeselectTab("Name1");
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Deselect again.
            control.DeselectTab("Name1");
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Deselect last.
            control.DeselectTab("Name2");
            Assert.Same(page1, control.SelectedTab);
            Assert.Equal(0, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControl_DeselectTab_InvokeStringWithHandle_GetReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage
            {
                Name = "Name1"
            };
            using var page2 = new TabPage
            {
                Name = "Name2"
            };
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int pageInvalidatedCallCount1 = 0;
            page1.Invalidated += (sender, e) => pageInvalidatedCallCount1++;
            int pageStyleChangedCallCount1 = 0;
            page1.StyleChanged += (sender, e) => pageStyleChangedCallCount1++;
            int pageCreatedCallCount1 = 0;
            page1.HandleCreated += (sender, e) => pageCreatedCallCount1++;
            int pageInvalidatedCallCount2 = 0;
            page2.Invalidated += (sender, e) => pageInvalidatedCallCount2++;
            int pageStyleChangedCallCount2 = 0;
            page2.StyleChanged += (sender, e) => pageStyleChangedCallCount2++;
            int pageCreatedCallCount2 = 0;
            page2.HandleCreated += (sender, e) => pageCreatedCallCount2++;
            Assert.True(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Deselect first.
            control.DeselectTab("Name1");
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.True(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Select and deselect first.
            control.SelectTab(0);
            control.DeselectTab("Name1");
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.True(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Deselect again.
            control.DeselectTab("Name1");
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.True(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Deselect last.
            control.DeselectTab("Name2");
            Assert.Same(page1, control.SelectedTab);
            Assert.Equal(0, control.SelectedIndex);
            Assert.True(page1.Visible);
            Assert.False(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(2, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);
        }

        [WinFormsFact]
        public void TabControl_DeselectTab_NullTabPageName_ThrowsArgumentNullException()
        {
            using var control = new TabControl();
            Assert.Throws<ArgumentNullException>("tabPageName", () => control.DeselectTab((string)null));
        }

        [WinFormsTheory]
        [InlineData("")]
        [InlineData("NoSuchName")]
        public void TabControl_DeselectTab_InvalidTabPageNameWithoutPages_ThrowsArgumentNullException(string tabPageName)
        {
            using var control = new TabControl();
            Assert.Throws<ArgumentNullException>("tabPage", () => control.DeselectTab(tabPageName));
        }

        [WinFormsTheory]
        [InlineData("")]
        [InlineData("NoSuchName")]
        public void TabControl_DeselectTab_InvalidTabPageNameWithPages_ThrowsArgumentNullException(string tabPageName)
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.Throws<ArgumentNullException>("tabPage", () => control.DeselectTab(tabPageName));
        }

        [WinFormsFact]
        public void TabControl_DeselectTab_InvokeIntWithoutHandle_GetReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);

            // Deselect first.
            control.DeselectTab(0);
            Assert.Null(control.SelectedTab);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Select and deselect first.
            control.SelectTab(0);
            control.DeselectTab(0);
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Deselect again.
            control.DeselectTab(0);
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Deselect last.
            control.DeselectTab(1);
            Assert.Same(page1, control.SelectedTab);
            Assert.Equal(0, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControl_DeselectTab_InvokeIntWithHandle_GetReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int pageInvalidatedCallCount1 = 0;
            page1.Invalidated += (sender, e) => pageInvalidatedCallCount1++;
            int pageStyleChangedCallCount1 = 0;
            page1.StyleChanged += (sender, e) => pageStyleChangedCallCount1++;
            int pageCreatedCallCount1 = 0;
            page1.HandleCreated += (sender, e) => pageCreatedCallCount1++;
            int pageInvalidatedCallCount2 = 0;
            page2.Invalidated += (sender, e) => pageInvalidatedCallCount2++;
            int pageStyleChangedCallCount2 = 0;
            page2.StyleChanged += (sender, e) => pageStyleChangedCallCount2++;
            int pageCreatedCallCount2 = 0;
            page2.HandleCreated += (sender, e) => pageCreatedCallCount2++;
            Assert.True(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Deselect first.
            control.DeselectTab(0);
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.True(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Select and deselect first.
            control.SelectTab(0);
            control.DeselectTab(0);
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.True(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Deselect again.
            control.DeselectTab(0);
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.True(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Deselect last.
            control.DeselectTab(1);
            Assert.Same(page1, control.SelectedTab);
            Assert.Equal(0, control.SelectedIndex);
            Assert.True(page1.Visible);
            Assert.False(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(2, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void TabControl_DeselectTab_InvalidIndexWithoutPages_ThrowsArgumentOutOfRangeException(int index)
        {
            using var control = new TabControl();
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.DeselectTab(index));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(2)]
        [InlineData(3)]
        public void TabControl_DeselectTab_InvalidIndexWithPages_ThrowsArgumentOutOfRangeException(int index)
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.DeselectTab(index));
        }

        [WinFormsFact]
        public void TabControl_Dispose_InvokeWithImageList_DetachesImageList()
        {
            using var control = new TabControl();
            using var imageList = new ImageList();
            control.ImageList = imageList;
            control.Dispose();
            Assert.Same(imageList, control.ImageList);

            imageList.Dispose();
            Assert.Same(imageList, control.ImageList);
        }

        [WinFormsFact]
        public void TabControl_Dispose_InvokeDisposingWithImageList_DetachesImageList()
        {
            using var control = new SubTabControl();
            using var imageList = new ImageList();
            control.ImageList = imageList;
            control.Dispose(true);
            Assert.Same(imageList, control.ImageList);

            imageList.Dispose();
            Assert.Same(imageList, control.ImageList);
        }

        [WinFormsFact]
        public void TabControl_Dispose_InvokeNotDisposingWithImageList_DoesNotDetachImageList()
        {
            using var control = new SubTabControl();
            using var imageList = new ImageList();
            control.ImageList = imageList;
            control.Dispose(false);
            Assert.Same(imageList, control.ImageList);

            imageList.Dispose();
            Assert.Null(control.ImageList);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        public void TabControl_GetControl_Invoke_ReturnsExpected(int index)
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.Same(control.TabPages[index], control.GetControl(index));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void TabControl_GetControl_InvokeWithoutPages_ThrowsArgumentOutOfRangeException(int index)
        {
            using var control = new SubTabControl();
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetControl(index));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(2)]
        [InlineData(3)]
        public void TabControl_GetControl_InvokeInvalidIndexWithPages_ThrowsArgumentOutOfRangeException(int index)
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetControl(index));
        }

        [WinFormsFact]
        public void TabControl_GetTabRect_InvokeWithoutHandle_ReturnsExpectedAndCreatedHandle()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);

            Rectangle rect1 = control.GetTabRect(0);
            Assert.True(rect1.X >= 0);
            Assert.True(rect1.Y >= 0);
            Assert.True(rect1.Width > 0);
            Assert.True(rect1.Height > 0);
            Assert.Equal(rect1, control.GetTabRect(0));
            Assert.True(control.IsHandleCreated);

            Rectangle rect2 = control.GetTabRect(1);
            Assert.True(rect2.X >= rect1.X + rect1.Width);
            Assert.Equal(rect2.Y, rect1.Y);
            Assert.True(rect2.Width > 0);
            Assert.True(rect2.Height > 0);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControl_GetTabRect_InvokeWithHandle_ReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Rectangle rect1 = control.GetTabRect(0);
            Assert.True(rect1.X >= 0);
            Assert.True(rect1.Y >= 0);
            Assert.True(rect1.Width > 0);
            Assert.True(rect1.Height > 0);
            Assert.Equal(rect1, control.GetTabRect(0));
            Assert.True(control.IsHandleCreated);

            Rectangle rect2 = control.GetTabRect(1);
            Assert.True(rect2.X >= rect1.X + rect1.Width);
            Assert.Equal(rect2.Y, rect1.Y);
            Assert.True(rect2.Width > 0);
            Assert.True(rect2.Height > 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> GetTabRect_InvokeCustomGetItemRect_TestData()
        {
            yield return new object[] { new RECT(), Rectangle.Empty };
            yield return new object[] { new RECT(1, 2, 3, 4), new Rectangle(1, 2, 2, 2) };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetTabRect_InvokeCustomGetItemRect_TestData))]
        public void TabControl_GetTabRect_InvokeCustomGetItemRect_ReturnsExpected(object getItemRectResult, Rectangle expected)
        {
            using var control = new CustomGetItemRectTabControl
            {
                GetItemRectResult = (RECT)getItemRectResult
            };
            using var page = new TabPage();
            control.TabPages.Add(page);
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Equal(expected, control.GetTabRect(0));
        }

        private class CustomGetItemRectTabControl : TabControl
        {
            public RECT GetItemRectResult { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (m.Msg == (int)ComCtl32.TCM.GETITEMRECT)
                {
                    RECT* pRect = (RECT*)m.LParam;
                    *pRect = GetItemRectResult;
                    m.Result = (IntPtr)1;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsFact]
        public void TabControl_GetTabRect_InvokeInvalidGetItemRect_ReturnsExpected()
        {
            using var control = new InvalidGetItemRectTabControl();
            using var page = new TabPage();
            control.TabPages.Add(page);
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.MakeInvalid = true;
            Assert.Equal(new Rectangle(1, 2, 2, 2), control.GetTabRect(0));
        }

        private class InvalidGetItemRectTabControl : TabControl
        {
            public bool MakeInvalid { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (MakeInvalid && m.Msg == (int)ComCtl32.TCM.GETITEMRECT)
                {
                    RECT* pRect = (RECT*)m.LParam;
                    *pRect = new RECT(1, 2, 3, 4);
                    m.Result = IntPtr.Zero;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsFact]
        public void TabControl_GetTabRect_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException()
        {
            using var control = new TabControl();
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(-1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(0));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(1));
        }

        [WinFormsFact]
        public void TabControl_GetTabRect_InvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            control.TabPages.Add(page1);

            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(-1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(2));
        }

        [WinFormsFact]
        public void TabControl_GetTabRect_InvalidIndexWithHandleEmpty_ThrowsArgumentOutOfRangeException()
        {
            using var control = new TabControl();
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(-1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(0));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(1));
        }

        [WinFormsFact]
        public void TabControl_GetTabRect_InvalidIndexWithHandleNotEmpty_ThrowsArgumentOutOfRangeException()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            control.TabPages.Add(page1);
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(-1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetTabRect(2));
        }

        [WinFormsFact]
        public void TabControl_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubTabControl();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        [WinFormsFact]
        public void TabControl_GetItems_InvokeWithoutPages_ReturnsExpected()
        {
            using var control = new SubTabControl();
            object[] result = Assert.IsType<TabPage[]>(control.GetItems());
            Assert.Empty(result);
            Assert.NotSame(result, control.GetItems());
        }

        [WinFormsFact]
        public void TabControl_GetItems_InvokeWithPages_ReturnsExpected()
        {
            using var control = new SubTabControl();
            using var page1 = new TabPage();
            using var page2 = new SubTabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            object[] result = Assert.IsType<TabPage[]>(control.GetItems());
            Assert.Equal(new object[] { page1, page2 }, result);
            Assert.NotSame(result, control.GetItems());
        }

        [WinFormsTheory]
        [InlineData(typeof(object))]
        [InlineData(typeof(TabPage))]
        public void TabControl_GetItems_InvokeTypeWithoutPages_ReturnsExpected(Type baseType)
        {
            using var control = new SubTabControl();
            object[] result = control.GetItems(baseType);
            Assert.Empty(result);
            Assert.IsType(baseType.MakeArrayType(), result);
            Assert.NotSame(result, control.GetItems());
        }

        [WinFormsTheory]
        [InlineData(typeof(TabPage))]
        [InlineData(typeof(Control))]
        [InlineData(typeof(object))]
        public void TabControl_GetItems_InvokeTypeBaseTypeWithPages_ReturnsExpected(Type baseType)
        {
            using var control = new SubTabControl();
            using var page1 = new TabPage();
            using var page2 = new SubTabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            object[] result = control.GetItems(baseType);
            Assert.IsType(baseType.MakeArrayType(), result);
            Assert.Equal(new object[] { page1, page2 }, result);
            Assert.NotSame(result, control.GetItems(baseType));
        }

        [WinFormsFact]
        public void TabControl_GetItems_InvokeTypeSubTypeWithPages_ReturnsExpected()
        {
            using var control = new SubTabControl();
            using var page = new SubTabPage();
            control.TabPages.Add(page);
            object[] result = Assert.IsType<SubTabPage[]>(control.GetItems(typeof(SubTabPage)));
            Assert.Equal(new object[] { page }, result);
            Assert.NotSame(result, control.GetItems(typeof(SubTabPage)));
        }

        [WinFormsFact]
        public void TabControl_GetItems_InvokeNullBaseType_ThrowsArgumentNullException()
        {
            using var control = new SubTabControl();
            Assert.Throws<ArgumentNullException>("elementType", () => control.GetItems(null));
        }

        [WinFormsTheory]
        [InlineData(typeof(int))]
        public void TabControl_GetItems_InvokeInvalidTypeWithoutPages_ThrowsInvalidCastException(Type baseType)
        {
            using var control = new SubTabControl();
            Assert.Throws<InvalidCastException>(() => control.GetItems(baseType));
        }

        [WinFormsTheory]
        [InlineData(typeof(SubTabPage))]
        [InlineData(typeof(int))]
        public void TabControl_GetItems_InvokeInvalidTypeWithPages_ThrowsInvalidCastException(Type baseType)
        {
            using var control = new SubTabControl();
            using var page1 = new TabPage();
            using var page2 = new SubTabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.Throws<InvalidCastException>(() => control.GetItems(baseType));
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, false)]
        [InlineData(ControlStyles.UserPaint, false)]
        [InlineData(ControlStyles.Opaque, false)]
        [InlineData(ControlStyles.ResizeRedraw, false)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, false)]
        [InlineData(ControlStyles.StandardClick, true)]
        [InlineData(ControlStyles.Selectable, true)]
        [InlineData(ControlStyles.UserMouse, false)]
        [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
        [InlineData(ControlStyles.StandardDoubleClick, true)]
        [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
        [InlineData(ControlStyles.CacheText, false)]
        [InlineData(ControlStyles.EnableNotifyMessage, false)]
        [InlineData(ControlStyles.DoubleBuffer, false)]
        [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
        [InlineData(ControlStyles.UseTextForAccessibility, true)]
        [InlineData((ControlStyles)0, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void TabControl_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubTabControl();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void TabControl_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubTabControl();
            Assert.False(control.GetTopLevel());
        }

        [WinFormsFact]
        public void TabControl_GetToolTipText_Invoke_ReturnsExpectd()
        {
            using var control = new SubTabControl();
            using var item = new TabPage
            {
                ToolTipText = "text"
            };
            Assert.Equal("text", control.GetToolTipText(item));
        }

        [WinFormsFact]
        public void TabControl_GetToolTipText_NullItem_ThrowsArgumentNullException()
        {
            using var control = new SubTabControl();
            Assert.Throws<ArgumentNullException>("item", () => control.GetToolTipText(null));
        }

        [WinFormsFact]
        public void TabControl_GetToolTipText_ItemNotTabPage_ThrowsArgumentException()
        {
            using var control = new SubTabControl();
            Assert.Throws<ArgumentException>("item", () => control.GetToolTipText(new object()));
        }

        public static IEnumerable<object[]> IsInputKey_TestData()
        {
            yield return new object[] { Keys.Tab, false };
            yield return new object[] { Keys.Return, false };
            yield return new object[] { Keys.Escape, false };
            yield return new object[] { Keys.A, false };
            yield return new object[] { Keys.C, false };
            yield return new object[] { Keys.Insert, false };
            yield return new object[] { Keys.Space, false };
            yield return new object[] { Keys.Home, true };
            yield return new object[] { Keys.End, true };
            ;
            yield return new object[] { Keys.Back, false };
            yield return new object[] { Keys.Next, true };
            yield return new object[] { Keys.Prior, true };
            yield return new object[] { Keys.Delete, false };
            yield return new object[] { Keys.D0, false };
            yield return new object[] { Keys.NumPad0, false };
            yield return new object[] { Keys.F1, false };
            yield return new object[] { Keys.F2, false };
            yield return new object[] { Keys.F3, false };
            yield return new object[] { Keys.F4, false };
            yield return new object[] { Keys.RButton, false };
            yield return new object[] { Keys.PageUp, true };
            yield return new object[] { Keys.PageDown, true };
            yield return new object[] { Keys.None, false };

            yield return new object[] { Keys.Control | Keys.Tab, false };
            yield return new object[] { Keys.Control | Keys.Return, false };
            yield return new object[] { Keys.Control | Keys.Escape, false };
            yield return new object[] { Keys.Control | Keys.A, false };
            yield return new object[] { Keys.Control | Keys.C, false };
            yield return new object[] { Keys.Control | Keys.Insert, false };
            yield return new object[] { Keys.Control | Keys.Space, false };
            yield return new object[] { Keys.Control | Keys.Home, true };
            yield return new object[] { Keys.Control | Keys.End, true };
            ;
            yield return new object[] { Keys.Control | Keys.Back, false };
            yield return new object[] { Keys.Control | Keys.Next, true };
            yield return new object[] { Keys.Control | Keys.Prior, true };
            yield return new object[] { Keys.Control | Keys.Delete, false };
            yield return new object[] { Keys.Control | Keys.D0, false };
            yield return new object[] { Keys.Control | Keys.NumPad0, false };
            yield return new object[] { Keys.Control | Keys.F1, false };
            yield return new object[] { Keys.Control | Keys.F2, false };
            yield return new object[] { Keys.Control | Keys.F3, false };
            yield return new object[] { Keys.Control | Keys.F4, false };
            yield return new object[] { Keys.Control | Keys.RButton, false };
            yield return new object[] { Keys.Control | Keys.PageUp, true };
            yield return new object[] { Keys.Control | Keys.PageDown, true };
            yield return new object[] { Keys.Control | Keys.None, false };

            yield return new object[] { Keys.Alt | Keys.Tab, false };
            yield return new object[] { Keys.Alt | Keys.Up, false };
            yield return new object[] { Keys.Alt | Keys.Down, false };
            yield return new object[] { Keys.Alt | Keys.Left, false };
            yield return new object[] { Keys.Alt | Keys.Right, false };
            yield return new object[] { Keys.Alt | Keys.Return, false };
            yield return new object[] { Keys.Alt | Keys.Escape, false };
            yield return new object[] { Keys.Alt | Keys.A, false };
            yield return new object[] { Keys.Alt | Keys.C, false };
            yield return new object[] { Keys.Alt | Keys.Insert, false };
            yield return new object[] { Keys.Alt | Keys.Space, false };
            yield return new object[] { Keys.Alt | Keys.Home, false };
            yield return new object[] { Keys.Alt | Keys.End, false };
            ;
            yield return new object[] { Keys.Alt | Keys.Back, false };
            yield return new object[] { Keys.Alt | Keys.Next, false };
            yield return new object[] { Keys.Alt | Keys.Prior, false };
            yield return new object[] { Keys.Alt | Keys.Delete, false };
            yield return new object[] { Keys.Alt | Keys.D0, false };
            yield return new object[] { Keys.Alt | Keys.NumPad0, false };
            yield return new object[] { Keys.Alt | Keys.F1, false };
            yield return new object[] { Keys.Alt | Keys.F2, false };
            yield return new object[] { Keys.Alt | Keys.F3, false };
            yield return new object[] { Keys.Alt | Keys.F4, false };
            yield return new object[] { Keys.Alt | Keys.RButton, false };
            yield return new object[] { Keys.Alt | Keys.PageUp, false };
            yield return new object[] { Keys.Alt | Keys.PageDown, false };
            yield return new object[] { Keys.Alt | Keys.None, false };
        }

        [WinFormsTheory]
        [MemberData(nameof(IsInputKey_TestData))]
        [InlineData(Keys.Up, false)]
        [InlineData(Keys.Down, false)]
        [InlineData(Keys.Left, false)]
        [InlineData(Keys.Right, false)]
        [InlineData(Keys.Control | Keys.Up, false)]
        [InlineData(Keys.Control | Keys.Down, false)]
        [InlineData(Keys.Control | Keys.Left, false)]
        [InlineData(Keys.Control | Keys.Right, false)]
        public void TabControl_IsInputKey_InvokeWithoutHandle_ReturnsExpected(Keys keyData, bool expected)
        {
            using var control = new SubTabControl();
            Assert.Equal(expected, control.IsInputKey(keyData));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(IsInputKey_TestData))]
        [InlineData(Keys.Up, true)]
        [InlineData(Keys.Down, true)]
        [InlineData(Keys.Left, true)]
        [InlineData(Keys.Right, true)]
        [InlineData(Keys.Control | Keys.Up, true)]
        [InlineData(Keys.Control | Keys.Down, true)]
        [InlineData(Keys.Control | Keys.Left, true)]
        [InlineData(Keys.Control | Keys.Right, true)]
        public void TabControl_IsInputKey_InvokeWithHandle_ReturnsExpected(Keys keyData, bool expected)
        {
            using var control = new SubTabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(expected, control.IsInputKey(keyData));
            Assert.True(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> TabControlEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new TabControlEventArgs(null, 0, TabControlAction.Deselecting) };
        }

        [WinFormsTheory]
        [MemberData(nameof(TabControlEventArgs_TestData))]
        public void TabControl_OnDeselected_Invoke_CallsDeselected(TabControlEventArgs eventArgs)
        {
            using var control = new SubTabControl();
            int callCount = 0;
            void handler(object sender, TabControlEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            }

            // Call with handler.
            control.Deselected += handler;
            control.OnDeselected(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Deselected -= handler;
            control.OnDeselected(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(TabControlEventArgs_TestData))]
        public void TabControl_OnDeselected_InvokeWithSelectedTab_CallsDeselected(TabControlEventArgs eventArgs)
        {
            using var control = new SubTabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            control.SelectedIndex = 0;
            int callCount = 0;
            int leaveCallCount1 = 0;
            int leaveCallCount2 = 0;
            void handler(object sender, TabControlEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                Assert.Equal(callCount, leaveCallCount1);
                callCount++;
            }
            void leaveHandler1(object sender, EventArgs e)
            {
                Assert.Same(page1, sender);
                Assert.Same(EventArgs.Empty, e);
                leaveCallCount1++;
            }
            void leaveHandler2(object sender, EventArgs e) => leaveCallCount2++;

            // Call with handler.
            control.Deselected += handler;
            page1.Leave += leaveHandler1;
            page2.Leave += leaveHandler2;
            control.OnDeselected(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, leaveCallCount1);
            Assert.Equal(0, leaveCallCount2);

            // Remove handler.
            control.Deselected -= handler;
            page1.Leave -= leaveHandler1;
            page2.Leave -= leaveHandler2;
            control.OnDeselected(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, leaveCallCount1);
            Assert.Equal(0, leaveCallCount2);
        }

        public static IEnumerable<object[]> TabControlCancelEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new TabControlCancelEventArgs(null, 0, true, TabControlAction.Deselecting) };
        }

        [WinFormsTheory]
        [MemberData(nameof(TabControlCancelEventArgs_TestData))]
        public void TabControl_OnDeselecting_Invoke_CallsDeselecting(TabControlCancelEventArgs eventArgs)
        {
            using var control = new SubTabControl();
            int callCount = 0;
            void handler(object sender, TabControlCancelEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            }

            // Call with handler.
            control.Deselecting += handler;
            control.OnDeselecting(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Deselecting -= handler;
            control.OnDeselecting(eventArgs);
            Assert.Equal(1, callCount);
        }
        public static IEnumerable<object[]> DrawItemEventArgs_TestData()
        {
            using var bitmap = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(bitmap);
            yield return new object[] { null };
            yield return new object[] { new DrawItemEventArgs(graphics, null, new Rectangle(1, 2, 3, 4), 0, DrawItemState.Checked) };
        }

        [WinFormsTheory]
        [MemberData(nameof(DrawItemEventArgs_TestData))]
        public void TabControl_OnDrawItem_Invoke_CallsDrawItem(DrawItemEventArgs eventArgs)
        {
            using var control = new SubTabControl();
            int callCount = 0;
            void handler(object sender, DrawItemEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            }

            // Call with handler.
            control.DrawItem += handler;
            control.OnDrawItem(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.DrawItem -= handler;
            control.OnDrawItem(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TabControl_OnEnter_Invoke_CallsEnter(EventArgs eventArgs)
        {
            using var control = new SubTabControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Enter += handler;
            control.OnEnter(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Enter -= handler;
            control.OnEnter(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TabControl_OnEnter_InvokeWithSelectedTab_CallsEnter(EventArgs eventArgs)
        {
            using var control = new SubTabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            control.SelectedTab = page2;

            int callCount = 0;
            int childCallCount1 = 0;
            int childCallCount2 = 0;
            page1.Enter += (sender, e) => childCallCount1++;
            page2.Enter += (sender, e) =>
            {
                Assert.Same(page2, sender);
                Assert.Same(eventArgs, e);
                childCallCount2++;
            };
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                Assert.Equal(0, childCallCount1);
                Assert.Equal(0, childCallCount2);
                callCount++;
            };

            // Call with handler.
            control.Enter += handler;
            control.OnEnter(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(1, childCallCount2);

            // Remove handler.
            control.Enter -= handler;
            control.OnEnter(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(2, childCallCount2);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TabControl_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubTabControl();
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
        public void TabControl_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubTabControl();
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
        public void TabControl_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubTabControl();
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
        public void TabControl_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubTabControl();
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
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TabControl_OnLeave_Invoke_CallsLeave(EventArgs eventArgs)
        {
            using var control = new SubTabControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Leave += handler;
            control.OnLeave(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Leave -= handler;
            control.OnLeave(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TabControl_OnLeave_InvokeWithSelectedTab_CallsLeave(EventArgs eventArgs)
        {
            using var control = new SubTabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            control.SelectedTab = page2;

            int callCount = 0;
            int childCallCount1 = 0;
            int childCallCount2 = 0;
            page1.Leave += (sender, e) => childCallCount1++;
            page2.Leave += (sender, e) =>
            {
                Assert.Same(page2, sender);
                Assert.Same(eventArgs, e);
                childCallCount2++;
            };
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                Assert.Equal(0, childCallCount1);
                Assert.Equal(1, childCallCount2);
                callCount++;
            };

            // Call with handler.
            control.Leave += handler;
            control.OnLeave(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(1, childCallCount2);

            // Remove handler.
            control.Leave -= handler;
            control.OnLeave(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(2, childCallCount2);
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
        public void TabControl_OnRightToLeftLayoutChanged_Invoke_CallsRightToLeftLayoutChanged(RightToLeft rightToLeft, EventArgs eventArgs)
        {
            using var control = new SubTabControl
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
        public void TabControl_OnRightToLeftLayoutChanged_InvokeWithHandle_CallsRightToLeftLayoutChanged(RightToLeft rightToLeft, EventArgs eventArgs, int expectedCreatedCallCount)
        {
            using var control = new SubTabControl
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
            Assert.Equal(expectedCreatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Remove handler.
            control.RightToLeftLayoutChanged -= handler;
            control.OnRightToLeftLayoutChanged(eventArgs);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount * 2, createdCallCount);
        }

        [WinFormsFact]
        public void TabControl_OnRightToLeftLayoutChanged_InvokeInDisposing_DoesNotCallRightToLeftLayoutChanged()
        {
            using var control = new SubTabControl
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

        [WinFormsTheory]
        [MemberData(nameof(TabControlEventArgs_TestData))]
        public void TabControl_OnSelected_InvokeWithSelectedTab_CallsSelected(TabControlEventArgs eventArgs)
        {
            using var control = new SubTabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            control.SelectedIndex = 0;
            int callCount = 0;
            int enterCallCount1 = 0;
            int enterCallCount2 = 0;
            void handler(object sender, TabControlEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                Assert.Equal(callCount, enterCallCount1);
                callCount++;
            }
            void enterHandler1(object sender, EventArgs e)
            {
                Assert.Same(page1, sender);
                Assert.Same(EventArgs.Empty, e);
                enterCallCount1++;
            }
            void enterHandler2(object sender, EventArgs e) => enterCallCount2++;

            // Call with handler.
            control.Selected += handler;
            page1.Enter += enterHandler1;
            page2.Enter += enterHandler2;
            control.OnSelected(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, enterCallCount1);
            Assert.Equal(0, enterCallCount2);

            // Remove handler.
            control.Selected -= handler;
            page1.Enter -= enterHandler1;
            page2.Enter -= enterHandler2;
            control.OnSelected(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, enterCallCount1);
            Assert.Equal(0, enterCallCount2);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TabControl_OnSelectedIndexChanged_Invoke_CallsSelectedIndexChanged(EventArgs eventArgs)
        {
            using var control = new SubTabControl();
            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            }

            // Call with handler.
            control.SelectedIndexChanged += handler;
            control.OnSelectedIndexChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.SelectedIndexChanged -= handler;
            control.OnSelectedIndexChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TabControl_OnSelectedIndexChanged_InvokeWithPages_CallsSelectedIndexChanged(EventArgs eventArgs)
        {
            using var control = new SubTabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            }

            // Call with handler.
            control.SelectedIndexChanged += handler;
            control.OnSelectedIndexChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);

            // Remove handler.
            control.SelectedIndexChanged -= handler;
            control.OnSelectedIndexChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TabControl_OnSelectedIndexChanged_InvokeWithHandle_CallsSelectedIndexChanged(EventArgs eventArgs)
        {
            using var control = new SubTabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            }

            // Call with handler.
            control.SelectedIndexChanged += handler;
            control.OnSelectedIndexChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.OnSelectedIndexChanged(eventArgs);
            Assert.Equal(2, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.SelectedIndexChanged -= handler;
            control.OnSelectedIndexChanged(eventArgs);
            Assert.Equal(2, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TabControl_OnSelectedIndexChanged_InvokeWithHandleWithPages_CallsSelectedIndexChanged(EventArgs eventArgs)
        {
            using var control = new SubTabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            }

            // Call with handler.
            control.SelectedIndexChanged += handler;
            control.OnSelectedIndexChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.Visible);
            Assert.False(page2.Visible);

            // Remove handler.
            control.SelectedIndexChanged -= handler;
            control.OnSelectedIndexChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.Visible);
            Assert.False(page2.Visible);
        }

        [WinFormsTheory]
        [MemberData(nameof(TabControlCancelEventArgs_TestData))]
        public void TabControl_OnSelecting_Invoke_CallsSelecting(TabControlCancelEventArgs eventArgs)
        {
            using var control = new SubTabControl();
            int callCount = 0;
            void handler(object sender, TabControlCancelEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            }

            // Call with handler.
            control.Selecting += handler;
            control.OnSelecting(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Selecting -= handler;
            control.OnSelecting(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void TabControl_RecreateHandle_InvokeWithoutHandle_Nop()
        {
            using var control = new SubTabControl();
            control.RecreateHandle();
            Assert.False(control.IsHandleCreated);

            // Invoke again.
            control.RecreateHandle();
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControl_RecreateHandle_InvokeEmptyWithHandle_Success()
        {
            using var control = new SubTabControl();
            IntPtr handle1 = control.Handle;
            Assert.NotEqual(IntPtr.Zero, handle1);
            Assert.Empty(control.Controls);
            Assert.Empty(control.TabPages);
            Assert.True(control.IsHandleCreated);

            control.RecreateHandle();
            IntPtr handle2 = control.Handle;
            Assert.NotEqual(IntPtr.Zero, handle2);
            Assert.NotEqual(handle1, handle2);
            Assert.Empty(control.Controls);
            Assert.Empty(control.TabPages);
            Assert.True(control.IsHandleCreated);

            // Invoke again.
            control.RecreateHandle();
            IntPtr handle3 = control.Handle;
            Assert.NotEqual(IntPtr.Zero, handle3);
            Assert.NotEqual(handle2, handle3);
            Assert.Empty(control.Controls);
            Assert.Empty(control.TabPages);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControl_RecreateHandle_InvokeNotEmptyWithHandle_Success()
        {
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            using var control = new SubTabControl();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);

            IntPtr handle1 = control.Handle;
            Assert.NotEqual(IntPtr.Zero, handle1);
            Assert.Equal(new Control[] { page1, page2 }, control.Controls.Cast<Control>());
            Assert.Equal(new TabPage[] { page1, page2 }, control.TabPages.Cast<TabPage>());
            Assert.True(control.IsHandleCreated);

            control.RecreateHandle();
            IntPtr handle2 = control.Handle;
            Assert.NotEqual(IntPtr.Zero, handle2);
            Assert.NotEqual(handle1, handle2);
            Assert.Equal(new Control[] { page1, page2 }, control.Controls.Cast<Control>());
            Assert.Equal(new TabPage[] { page1, page2 }, control.TabPages.Cast<TabPage>());
            Assert.True(control.IsHandleCreated);

            // Invoke again.
            control.RecreateHandle();
            IntPtr handle3 = control.Handle;
            Assert.NotEqual(IntPtr.Zero, handle3);
            Assert.NotEqual(handle2, handle3);
            Assert.Equal(new Control[] { page1, page2 }, control.Controls.Cast<Control>());
            Assert.Equal(new TabPage[] { page1, page2 }, control.TabPages.Cast<TabPage>());
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("Text", "Text")]
        [InlineData("&&Text", "&&Text")]
        [InlineData("&", "&&")]
        [InlineData("&Text", "&&Text")]
        public unsafe void TabControl_RecreateHandle_GetItemsWithHandle_Success(string text, string expectedText)
        {
            using var control = new SubTabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage
            {
                Text = text,
                ImageIndex = 1
            };
            using var page3 = new NullTextTabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            control.TabPages.Add(page3);

            control.RecreateHandle();
            Assert.Equal((IntPtr)3, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.TCM.GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero));

            char* buffer = stackalloc char[256];
            ComCtl32.TCITEMW item = default;
            item.cchTextMax = int.MaxValue;
            item.pszText = buffer;
            item.dwStateMask = (ComCtl32.TCIS)uint.MaxValue;
            item.mask = (ComCtl32.TCIF)uint.MaxValue;

            // Get item 0.
            Assert.Equal((IntPtr)1, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.TCM.GETITEMW, (IntPtr)0, ref item));
            Assert.Equal(ComCtl32.TCIS.BUTTONPRESSED, item.dwState);
            Assert.Equal(IntPtr.Zero, item.lParam);
            Assert.Equal(int.MaxValue, item.cchTextMax);
            Assert.Empty(new string(item.pszText));
            Assert.Equal(-1, item.iImage);

            // Get item 1.
            Assert.Equal((IntPtr)1, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.TCM.GETITEMW, (IntPtr)1, ref item));
            Assert.Equal((ComCtl32.TCIS)0, item.dwState);
            Assert.Equal(IntPtr.Zero, item.lParam);
            Assert.Equal(int.MaxValue, item.cchTextMax);
            Assert.Equal(expectedText, new string(item.pszText));
            Assert.Equal(1, item.iImage);

            // Get item 2.
            Assert.Equal((IntPtr)1, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.TCM.GETITEMW, (IntPtr)2, ref item));
            Assert.Equal((ComCtl32.TCIS)0, item.dwState);
            Assert.Equal(IntPtr.Zero, item.lParam);
            Assert.Equal(int.MaxValue, item.cchTextMax);
            Assert.Empty(new string(item.pszText));
            Assert.Equal(-1, item.iImage);
        }

        [WinFormsFact]
        public void TabControl_RemoveAll_InvokeEmpty_Success()
        {
            using var control = new SubTabControl();
            int layoutCallCount = 0;
            void layoutHandler(object sender, LayoutEventArgs e) => layoutCallCount++;
            control.Layout += layoutHandler;
            int controlRemovedCallCount = 0;
            control.ControlRemoved += (sender, e) => controlRemovedCallCount++;

            try
            {
                control.RemoveAll();
                Assert.Empty(control.TabPages);
                Assert.Empty(control.Controls);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, controlRemovedCallCount);
                Assert.False(control.IsHandleCreated);

                // RemoveAll again.
                control.RemoveAll();
                Assert.Empty(control.TabPages);
                Assert.Empty(control.Controls);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, controlRemovedCallCount);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                control.Layout -= layoutHandler;
            }
        }

        [WinFormsFact]
        public void TabControl_RemoveAll_InvokeNotEmpty_Success()
        {
            using var control = new SubTabControl();
            using var child1 = new TabPage();
            using var child2 = new TabPage();
            using var child3 = new TabPage();
            control.TabPages.Add(child1);
            control.TabPages.Add(child2);
            control.TabPages.Add(child3);
            int layoutCallCount = 0;
            void layoutHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(child3, e.AffectedControl);
                Assert.Equal("Parent", e.AffectedProperty);
                layoutCallCount++;
            }
            control.Layout += layoutHandler;
            int controlRemovedCallCount = 0;
            control.ControlRemoved += (sender, e) => controlRemovedCallCount++;

            try
            {
                control.RemoveAll();
                Assert.Empty(control.TabPages);
                Assert.Empty(control.Controls);
                Assert.Null(child1.Parent);
                Assert.Null(child2.Parent);
                Assert.Null(child3.Parent);
                Assert.Equal(1, layoutCallCount);
                Assert.Equal(3, controlRemovedCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
                Assert.False(child3.IsHandleCreated);

                // RemoveAll again.
                control.RemoveAll();
                Assert.Empty(control.TabPages);
                Assert.Empty(control.Controls);
                Assert.Null(child1.Parent);
                Assert.Null(child2.Parent);
                Assert.Null(child3.Parent);
                Assert.Equal(1, layoutCallCount);
                Assert.Equal(3, controlRemovedCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
                Assert.False(child3.IsHandleCreated);
            }
            finally
            {
                control.Layout -= layoutHandler;
            }
        }

        [WinFormsFact]
        public void TabControl_RemoveAll_InvokeEmptyWithHandle_Success()
        {
            using var control = new SubTabControl();

            int controlRemovedCallCount = 0;
            control.ControlRemoved += (sender, e) => controlRemovedCallCount++;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int parentInvalidatedCallCount = 0;
            control.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            control.HandleCreated += (sender, e) => parentCreatedCallCount++;
            int layoutCallCount = 0;
            void layoutHandler(object sender, LayoutEventArgs e) => layoutCallCount++;
            control.Layout += layoutHandler;

            try
            {
                control.RemoveAll();
                Assert.Empty(control.TabPages);
                Assert.Empty(control.Controls);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, controlRemovedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);

                // RemoveAll again.
                control.RemoveAll();
                Assert.Empty(control.TabPages);
                Assert.Empty(control.Controls);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, controlRemovedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
            }
            finally
            {
                control.Layout -= layoutHandler;
            }
        }

        [WinFormsFact]
        public void TabControl_RemoveAll_InvokeNotEmptyWithHandle_Success()
        {
            using var control = new SubTabControl();
            using var child1 = new TabPage();
            using var child2 = new TabPage();
            using var child3 = new TabPage();
            control.TabPages.Add(child1);
            control.TabPages.Add(child2);
            control.TabPages.Add(child3);

            int controlRemovedCallCount = 0;
            control.ControlRemoved += (sender, e) => controlRemovedCallCount++;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int parentInvalidatedCallCount = 0;
            control.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            control.HandleCreated += (sender, e) => parentCreatedCallCount++;
            int layoutCallCount = 0;
            void layoutHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(child3, e.AffectedControl);
                Assert.Equal("Parent", e.AffectedProperty);
                layoutCallCount++;
            };
            control.Layout += layoutHandler;

            try
            {
                control.RemoveAll();
                Assert.Empty(control.TabPages);
                Assert.Empty(control.Controls);
                Assert.Null(child1.Parent);
                Assert.Null(child2.Parent);
                Assert.Null(child3.Parent);
                Assert.Equal(1, layoutCallCount);
                Assert.Equal(3, controlRemovedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
                Assert.True(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
                Assert.False(child3.IsHandleCreated);

                // RemoveAll again.
                control.RemoveAll();
                Assert.Empty(control.TabPages);
                Assert.Empty(control.Controls);
                Assert.Null(child1.Parent);
                Assert.Null(child2.Parent);
                Assert.Null(child3.Parent);
                Assert.Equal(1, layoutCallCount);
                Assert.Equal(3, controlRemovedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
                Assert.True(child1.IsHandleCreated);
                Assert.False(child2.IsHandleCreated);
                Assert.False(child3.IsHandleCreated);
            }
            finally
            {
                control.Layout -= layoutHandler;
            }
        }

        [WinFormsFact]
        public void TabControl_RemoveAll_GetItemsWithHandle_Success()
        {
            using var control = new SubTabControl();
            using var child1 = new TabPage();
            using var child2 = new TabPage();
            using var child3 = new TabPage();
            control.TabPages.Add(child1);
            control.TabPages.Add(child2);
            control.TabPages.Add(child3);

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.RemoveAll();
            Assert.Equal((IntPtr)0, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.TCM.GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero));
        }

        [WinFormsFact]
        public void TabControl_SelectTab_InvokeTabPageWithPages_GetReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);

            // Set valid.
            control.SelectTab(page2);
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Set same.
            control.SelectTab(page2);
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Set different.
            control.SelectTab(page1);
            Assert.Same(page1, control.SelectedTab);
            Assert.Equal(0, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControl_SelectTab_InvokeTabPageWithPagesWithHandle_GetReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int pageInvalidatedCallCount1 = 0;
            page1.Invalidated += (sender, e) => pageInvalidatedCallCount1++;
            int pageStyleChangedCallCount1 = 0;
            page1.StyleChanged += (sender, e) => pageStyleChangedCallCount1++;
            int pageCreatedCallCount1 = 0;
            page1.HandleCreated += (sender, e) => pageCreatedCallCount1++;
            int pageInvalidatedCallCount2 = 0;
            page2.Invalidated += (sender, e) => pageInvalidatedCallCount2++;
            int pageStyleChangedCallCount2 = 0;
            page2.StyleChanged += (sender, e) => pageStyleChangedCallCount2++;
            int pageCreatedCallCount2 = 0;
            page2.HandleCreated += (sender, e) => pageCreatedCallCount2++;
            Assert.True(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Set valid.
            control.SelectTab(page2);
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.True(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Set same.
            control.SelectTab(page2);
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.True(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Set different.
            control.SelectTab(page1);
            Assert.Same(page1, control.SelectedTab);
            Assert.Equal(0, control.SelectedIndex);
            Assert.True(page1.Visible);
            Assert.False(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);
        }

        [WinFormsFact]
        public void TabControl_SelectTab_NullTabPage_ThrowsArgumentNullException()
        {
            using var control = new TabControl();
            Assert.Throws<ArgumentNullException>("tabPage", () => control.SelectTab((TabPage)null));
        }

        [WinFormsFact]
        public void TabControl_SelectTab_NoSuchTabPageWithoutPages_ThrowsArgumentOutOfRangeException()
        {
            using var control = new TabControl();
            using var page = new TabPage();
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.SelectTab(page));
        }

        [WinFormsFact]
        public void TabControl_SelectTab_NoSuchTabPageWithPages_ThrowsArgumentOutOfRangeException()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            using var page3 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.SelectTab(page3));
        }

        [WinFormsFact]
        public void TabControl_SelectTab_InvokeStringWithPages_GetReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage
            {
                Name = "Name1"
            };
            using var page2 = new TabPage
            {
                Name = "Name2"
            };
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);

            // Set valid.
            control.SelectTab("Name2");
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Set same.
            control.SelectTab("Name2");
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Set different.
            control.SelectTab("Name1");
            Assert.Same(page1, control.SelectedTab);
            Assert.Equal(0, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControl_SelectTab_InvokeStringWithPagesWithHandle_GetReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage
            {
                Name = "Name1"
            };
            using var page2 = new TabPage
            {
                Name = "Name2"
            };
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int pageInvalidatedCallCount1 = 0;
            page1.Invalidated += (sender, e) => pageInvalidatedCallCount1++;
            int pageStyleChangedCallCount1 = 0;
            page1.StyleChanged += (sender, e) => pageStyleChangedCallCount1++;
            int pageCreatedCallCount1 = 0;
            page1.HandleCreated += (sender, e) => pageCreatedCallCount1++;
            int pageInvalidatedCallCount2 = 0;
            page2.Invalidated += (sender, e) => pageInvalidatedCallCount2++;
            int pageStyleChangedCallCount2 = 0;
            page2.StyleChanged += (sender, e) => pageStyleChangedCallCount2++;
            int pageCreatedCallCount2 = 0;
            page2.HandleCreated += (sender, e) => pageCreatedCallCount2++;
            Assert.True(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Set valid.
            control.SelectTab("Name2");
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.True(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Set same.
            control.SelectTab("Name2");
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.True(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Set different.
            control.SelectTab("Name1");
            Assert.Same(page1, control.SelectedTab);
            Assert.Equal(0, control.SelectedIndex);
            Assert.True(page1.Visible);
            Assert.False(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);
        }

        [WinFormsFact]
        public void TabControl_SelectTab_NullTabPageName_ThrowsArgumentNullException()
        {
            using var control = new TabControl();
            Assert.Throws<ArgumentNullException>("tabPageName", () => control.SelectTab((string)null));
        }

        [WinFormsTheory]
        [InlineData("")]
        [InlineData("NoSuchName")]
        public void TabControl_SelectTab_NoSuchTabPageNameWithoutPages_ThrowsArgumentOutOfRangeException(string tabPageName)
        {
            using var control = new TabControl();
            Assert.Throws<ArgumentNullException>("tabPage", () => control.SelectTab(tabPageName));
        }

        [WinFormsTheory]
        [InlineData("")]
        [InlineData("NoSuchName")]
        public void TabControl_SelectTab_NoSuchTabPageNameWithPages_ThrowsArgumentOutOfRangeException(string tabPageName)
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            using var page3 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.Throws<ArgumentNullException>("tabPage", () => control.SelectTab(tabPageName));
        }

        [WinFormsFact]
        public void TabControl_SelectTab_InvokeIntWithPages_GetReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);

            // Set valid.
            control.SelectTab(1);
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Set same.
            control.SelectTab(1);
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Set different.
            control.SelectTab(0);
            Assert.Same(page1, control.SelectedTab);
            Assert.Equal(0, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.False(page2.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControl_SelectTab_InvokeIntWithPagesWithHandle_GetReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int pageInvalidatedCallCount1 = 0;
            page1.Invalidated += (sender, e) => pageInvalidatedCallCount1++;
            int pageStyleChangedCallCount1 = 0;
            page1.StyleChanged += (sender, e) => pageStyleChangedCallCount1++;
            int pageCreatedCallCount1 = 0;
            page1.HandleCreated += (sender, e) => pageCreatedCallCount1++;
            int pageInvalidatedCallCount2 = 0;
            page2.Invalidated += (sender, e) => pageInvalidatedCallCount2++;
            int pageStyleChangedCallCount2 = 0;
            page2.StyleChanged += (sender, e) => pageStyleChangedCallCount2++;
            int pageCreatedCallCount2 = 0;
            page2.HandleCreated += (sender, e) => pageCreatedCallCount2++;
            Assert.True(page1.IsHandleCreated);
            Assert.False(page2.IsHandleCreated);

            // Set valid.
            control.SelectTab(1);
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.True(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Set same.
            control.SelectTab(1);
            Assert.Same(page2, control.SelectedTab);
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(page1.Visible);
            Assert.True(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);

            // Set different.
            control.SelectTab(0);
            Assert.Same(page1, control.SelectedTab);
            Assert.Equal(0, control.SelectedIndex);
            Assert.True(page1.Visible);
            Assert.False(page2.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(page1.IsHandleCreated);
            Assert.Equal(1, pageInvalidatedCallCount1);
            Assert.Equal(0, pageStyleChangedCallCount1);
            Assert.Equal(0, pageCreatedCallCount1);
            Assert.True(page2.IsHandleCreated);
            Assert.Equal(0, pageInvalidatedCallCount2);
            Assert.Equal(0, pageStyleChangedCallCount2);
            Assert.Equal(1, pageCreatedCallCount2);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void TabControl_SelectTab_InvalidIndexWithoutPages_ThrowsArgumentOutOfRangeException(int index)
        {
            using var control = new TabControl();
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.SelectTab(index));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(2)]
        [InlineData(3)]
        public void TabControl_SelectTab_InvalidIndexWithPages_ThrowsArgumentOutOfRangeException(int index)
        {
            using var control = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage();
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.SelectTab(index));
        }

        [WinFormsFact]
        public void TabControl_ToString_InvokeEmpty_ReturnsExpected()
        {
            using var control = new TabControl();
            Assert.Equal("System.Windows.Forms.TabControl, TabPages.Count: 0", control.ToString());
        }

        [WinFormsFact]
        public void TabControl_ToString_InvokeNotEmpty_ReturnsExpected()
        {
            using var control = new TabControl();
            using var page1 = new TabPage("text1");
            using var page2 = new TabPage("text2");
            control.TabPages.Add(page1);
            control.TabPages.Add(page2);
            Assert.Equal("System.Windows.Forms.TabControl, TabPages.Count: 2, TabPages[0]: TabPage: {text1}", control.ToString());
        }

        private class SubTabPage : TabPage
        {
        }

        private class NullTextTabPage : TabPage
        {
            public override string Text
            {
                get => null;
                set { }
            }
        }

        public class SubTabControl : TabControl
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

            public new Control.ControlCollection CreateControlsInstance() => base.CreateControlsInstance();

            public new void CreateHandle() => base.CreateHandle();

            public new void Dispose(bool disposing) => base.Dispose(disposing);

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new object[] GetItems() => base.GetItems();

            public new object[] GetItems(Type baseType) => base.GetItems(baseType);

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public new string GetToolTipText(object item) => base.GetToolTipText(item);

            public new bool IsInputKey(Keys keyData) => base.IsInputKey(keyData);

            public new void OnDeselected(TabControlEventArgs e) => base.OnDeselected(e);

            public new void OnDeselecting(TabControlCancelEventArgs e) => base.OnDeselecting(e);

            public new void OnDrawItem(DrawItemEventArgs e) => base.OnDrawItem(e);

            public new void OnEnter(EventArgs e) => base.OnEnter(e);

            public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

            public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

            public new void OnLeave(EventArgs e) => base.OnLeave(e);

            public new void OnRightToLeftLayoutChanged(EventArgs e) => base.OnRightToLeftLayoutChanged(e);

            public new void OnSelected(TabControlEventArgs e) => base.OnSelected(e);

            public new void OnSelectedIndexChanged(EventArgs e) => base.OnSelectedIndexChanged(e);

            public new void OnSelecting(TabControlCancelEventArgs e) => base.OnSelecting(e);

            public new void RecreateHandle() => base.RecreateHandle();

            public new void RemoveAll() => base.RemoveAll();

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
        }
    }
}

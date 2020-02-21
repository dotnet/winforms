// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

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
            Assert.True(control.UseMnemonic);
            Assert.True(control.UseCompatibleTextRendering);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.UseVisualStyleBackColor);
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
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TextImageRelation))]
        public void ToolStripItem_TextImageRelation_Set_GetReturnsExpected(TextImageRelation value)
        {
            var button = new SubButton
            {
                TextImageRelation = value
            };
            Assert.Equal(value, button.TextImageRelation);

            // Set same.
            button.TextImageRelation = value;
            Assert.Equal(value, button.TextImageRelation);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TextImageRelation))]
        [InlineData((TextImageRelation)3)]
        [InlineData((TextImageRelation)5)]
        [InlineData((TextImageRelation)6)]
        [InlineData((TextImageRelation)7)]
        public void Button_TextImageRelation_SetInvalid_ThrowsInvalidEnumArgumentException(TextImageRelation value)
        {
            var button = new SubButton();
            Assert.Throws<InvalidEnumArgumentException>("value", () => button.TextImageRelation = value);
        }

        /// <summary>
        ///  Data for the AutoSizeModeGetSet test
        /// </summary>
        public static TheoryData<AutoSizeMode> AutoSizeModeGetSetData =>
            CommonTestHelper.GetEnumTheoryData<AutoSizeMode>();

        [WinFormsTheory]
        [MemberData(nameof(AutoSizeModeGetSetData))]
        public void Button_AutoSizeModeGetSet(AutoSizeMode expected)
        {
            var button = new Button
            {
                AutoSizeMode = expected
            };

            Assert.Equal(expected, button.AutoSizeMode);
        }

        /// <summary>
        ///  Data for the AutoSizeModeGetSetInvalid test
        /// </summary>
        public static TheoryData<AutoSizeMode> AutoSizeModeGetSetInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<AutoSizeMode>();

        [WinFormsTheory]
        [MemberData(nameof(AutoSizeModeGetSetInvalidData))]
        public void Button_AutoSizeModeGetSetInvalid(AutoSizeMode expected)
        {
            var button = new Button();

            // act & assert
            InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>(() => button.AutoSizeMode = expected);
            Assert.Equal("value", ex.ParamName);
        }

        [WinFormsFact]
        public void Button_CreateFlatAdapter()
        {
            var button = new Button();

            ButtonInternal.ButtonBaseAdapter adaptor = button.CreateFlatAdapter();

            Assert.NotNull(adaptor);
        }

        [WinFormsFact]
        public void Button_CreatePopupAdapter()
        {
            var button = new Button();

            ButtonInternal.ButtonBaseAdapter adaptor = button.CreatePopupAdapter();

            Assert.NotNull(adaptor);
        }

        [WinFormsFact]
        public void Button_CreateStandardAdapter()
        {
            var button = new Button();

            ButtonInternal.ButtonBaseAdapter adaptor = button.CreateStandardAdapter();

            Assert.NotNull(adaptor);
        }

        public static TheoryData<Button, Size, Size> GetPreferredSizeCoreData =>
            GetPreferredSizeCoreTestData();

        [WinFormsTheory]
        [MemberData(nameof(GetPreferredSizeCoreData))]
        public void Button_GetPreferredSizeCore(Button button, Size proposed, Size expected)
        {
            Size actual = button.GetPreferredSizeCore(proposed);

            Assert.Equal(expected, actual);
        }

        /// <summary>
        ///  Data for the DialogResultGetSet test
        /// </summary>
        public static TheoryData<DialogResult> DialogResultGetSetData =>
            CommonTestHelper.GetEnumTheoryData<DialogResult>();

        [WinFormsTheory]
        [MemberData(nameof(DialogResultGetSetData))]
        public void Button_DialogResultGetSet(DialogResult expected)
        {
            var button = new Button
            {
                DialogResult = expected
            };

            Assert.Equal(expected, button.DialogResult);
        }

        /// <summary>
        ///  Data for the DialogResultGetSetInvalid test
        /// </summary>
        public static TheoryData<DialogResult> DialogResultGetSetInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<DialogResult>();

        [WinFormsTheory]
        [MemberData(nameof(DialogResultGetSetInvalidData))]
        public void Button_DialogResultGetSetInvalid(DialogResult expected)
        {
            var button = new Button();

            // act & assert
            InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>(() => button.DialogResult = expected);
            Assert.Equal("value", ex.ParamName);
        }

        /// <summary>
        ///  Data for the NotifyDefault test
        /// </summary>
        public static TheoryData<bool> NotifyDefaultData =>
            CommonTestHelper.GetBoolTheoryData();

        [WinFormsTheory]
        [MemberData(nameof(NotifyDefaultData))]
        public void Button_NotifyDefault(bool expected)
        {
            var button = new Button();

            button.NotifyDefault(expected);

            Assert.Equal(expected, button.IsDefault);
        }

        [WinFormsFact]
        public void Button_PerformClick()
        {
            var wasClicked = false;
            var button = new Button();
            button.Click += (sender, args) => wasClicked = true;

            button.PerformClick();

            Assert.True(wasClicked);
        }

        [WinFormsFact]
        public void Button_ToStringTest()
        {
            var button = new Button
            {
                Text = "Hello World!"
            };
            var expected = "System.Windows.Forms.Button, Text: " + button.Text;

            var actual = button.ToString();

            Assert.Equal(expected, actual);
        }

        [WinFormsFact]
        public void Button_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubButton();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        // helper method to generate data for the GetPreferredSizeCore test
        private static TheoryData<Button, Size, Size> GetPreferredSizeCoreTestData()
        {
            var data = new TheoryData<Button, Size, Size>();

            // first code path is FlatStyle != FlatStyle.System, AutoSizeMode = GrowAndShrink
            var b1 = new Button
            {
                FlatStyle = FlatStyle.Flat,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            var proposed1 = new Size(5, 5);
            var expected1 = new Size(8, 8);
            data.Add(b1, proposed1, expected1);

            // second code path is FlatStyle != FlatStyle.System, AutoSizeMode != GrowAndShrink
            var b2 = new Button
            {
                FlatStyle = FlatStyle.Flat,
                AutoSizeMode = AutoSizeMode.GrowOnly
            };
            var proposed2 = new Size(5, 5);
            var expected2 = new Size(75, 23);
            data.Add(b2, proposed2, expected2);

            // third code path is FlatStyle == FlatStyle.System, button systemSize.Width is invalid
            // and AutoSizeMode = GrowAndShrink
            var b3 = new Button
            {
                // text and font need to be set since the code measures the size of the text
                Text = "Hello World!",
                Font = new Drawing.Font(Drawing.FontFamily.GenericMonospace, 1.5f),
                FlatStyle = FlatStyle.System,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            var proposed3 = new Size(100, 200);
            var expected3 = new Size(28, 12);
            data.Add(b3, proposed3, expected3);

            // fourth code path is FlatStyle == FlatStyle.System, button systemSize.Width is valid
            // and AutoSizeMode != GrowAndShrink
            var b4 = new Button
            {
                // text and font need to be set since the code measures the size of the text
                Text = "Hello World!",
                Font = new Drawing.Font(Drawing.FontFamily.GenericMonospace, 1.5f),
                FlatStyle = FlatStyle.System,
                AutoSizeMode = AutoSizeMode.GrowOnly
            };
            var proposed4 = new Size(100, 200);
            // call getPreferredSizeCore once so the systemSize gets set
            b4.GetPreferredSizeCore(proposed4);
            var expected4 = new Size(75, 23);
            data.Add(b4, proposed4, expected4);

            return data;
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

        public class SubButton : Button
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

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);
        }
    }
}

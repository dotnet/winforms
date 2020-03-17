// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class DomainUpDownTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DomainUpDown_Ctor_Default()
        {
            using var control = new SubDomainUpDown();
            Assert.Null(control.ActiveControl);
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
            Assert.Equal(SystemColors.Window, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.NotNull(control.BindingContext);
            Assert.Same(control.BindingContext, control.BindingContext);
            Assert.Equal(BorderStyle.Fixed3D, control.BorderStyle);
            Assert.Equal(control.PreferredHeight, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 120, control.PreferredHeight), control.Bounds);
            Assert.False(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CausesValidation);
            Assert.False(control.ChangingText);
            Assert.Equal(new Rectangle(0, 0, 116, Control.DefaultFont.Height + 3), control.ClientRectangle);
            Assert.Equal(new Size(116, Control.DefaultFont.Height + 3), control.ClientSize);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.NotEmpty(control.Controls);
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
            Assert.Equal(new Size(120, control.PreferredHeight), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 116, Control.DefaultFont.Height + 3), control.DisplayRectangle);
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
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.True(control.HasChildren);
            Assert.Equal(control.PreferredHeight, control.Height);
            Assert.NotNull(control.HorizontalScroll);
            Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
            Assert.False(control.HScroll);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.True(control.InterceptArrowKeys);
            Assert.False(control.InvokeRequired);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.Empty(control.Items);
            Assert.Same(control.Items, control.Items);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal(Control.DefaultFont.Height + 7, control.PreferredHeight);
            Assert.Equal(new Size(control.PreferredHeight - 3, 23), control.PreferredSize);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.False(control.ReadOnly);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.True(control.ResizeRedraw);
            Assert.Equal(120, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(120, control.PreferredHeight), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(HorizontalAlignment.Left, control.TextAlign);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.Equal(LeftRightAlignment.Right, control.UpDownAlign);
            Assert.False(control.UserEdit);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.False(control.VScroll);
            Assert.Equal(120, control.Width);
            Assert.False(control.Wrap);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DomainUpDown_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubDomainUpDown();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x10200, createParams.ExStyle);
            Assert.Equal(control.PreferredHeight, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56010000, createParams.Style);
            Assert.Equal(120, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingNormalizedTheoryData))]
        public void DomainUpDown_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
        {
            using var control = new DomainUpDown
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
        public void DomainUpDown_Padding_SetWithHandle_GetReturnsExpected(Padding value, Padding expected)
        {
            using var control = new DomainUpDown();
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
        public void DomainUpDown_Padding_SetWithHandler_CallsPaddingChanged()
        {
            using var control = new DomainUpDown();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Same(EventArgs.Empty, e);
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

        [WinFormsFact]
        public void DomainUpDown_SelectedIndex_SetEmpty_Nop()
        {
            using var control = new SubDomainUpDown
            {
                SelectedIndex = -1
            };
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.Text);
            Assert.False(control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectedIndex = -1;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.Text);
            Assert.False(control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0, "Item1", "Item1", true)]
        [InlineData(1, "Item2", "Item2", true)]
        [InlineData(-1, null, "", false)]
        public void DomainUpDown_SelectedIndex_SetNotEmpty_GetReturnsExpected(int value, object expected, string expectedText, bool expectedUserEdit)
        {
            using var control = new SubDomainUpDown();
            control.Items.Add("Item1");
            control.Items.Add("Item2");

            control.SelectedIndex = value;
            Assert.Equal(value, control.SelectedIndex);
            Assert.Equal(expected, control.SelectedItem);
            Assert.Equal(expectedText, control.Text);
            Assert.False(control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectedIndex = value;
            Assert.Equal(value, control.SelectedIndex);
            Assert.Equal(expected, control.SelectedItem);
            Assert.Equal(expectedText, control.Text);
            Assert.False(control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);

            // Set none.
            control.SelectedIndex = -1;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Equal(expectedText, control.Text);
            Assert.Equal(expectedUserEdit, control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0, "Item1", "Item1", true)]
        [InlineData(1, "Item2", "Item2", true)]
        [InlineData(-1, null, "", false)]
        public void DomainUpDown_SelectedIndex_SetUserEdit_GetReturnsExpected(int value, object expected, string expectedText, bool expectedUserEdit)
        {
            using var control = new SubDomainUpDown
            {
                UserEdit = true
            };
            control.Items.Add("Item1");
            control.Items.Add("Item2");

            control.SelectedIndex = value;
            Assert.Equal(value, control.SelectedIndex);
            Assert.Equal(expected, control.SelectedItem);
            Assert.Equal(expectedText, control.Text);
            Assert.Equal(!expectedUserEdit, control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectedIndex = value;
            Assert.Equal(value, control.SelectedIndex);
            Assert.Equal(expected, control.SelectedItem);
            Assert.Equal(expectedText, control.Text);
            Assert.Equal(!expectedUserEdit, control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);

            // Set none.
            control.SelectedIndex = -1;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Equal(expectedText, control.Text);
            Assert.True(control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DomainUpDown_SelectedIndex_SetWithHandler_CallsSelectedItemChanged()
        {
            using var control = new DomainUpDown();
            control.Items.Add("Item1");
            control.Items.Add("Item2");

            int textChangedCallCount = 0;
            int callCount = 0;
            EventHandler textChangedHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                textChangedCallCount++;
            };
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(textChangedCallCount - 1, callCount);
                callCount++;
            };
            control.TextChanged += textChangedHandler;
            control.SelectedItemChanged += handler;

            // Set different.
            control.SelectedIndex = 0;
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal(1, textChangedCallCount);
            Assert.Equal(1, callCount);

            // Set same.
            control.SelectedIndex = 0;
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal(1, textChangedCallCount);
            Assert.Equal(1, callCount);

            // Set different.
            control.SelectedIndex = 1;
            Assert.Equal(1, control.SelectedIndex);
            Assert.Equal(2, textChangedCallCount);
            Assert.Equal(2, callCount);

            // Set none.
            control.SelectedIndex = -1;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(2, textChangedCallCount);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.TextChanged -= textChangedHandler;
            control.SelectedItemChanged -= handler;
            control.SelectedIndex = 0;
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal(2, textChangedCallCount);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DomainUpDown_SelectedIndex_SetInvalidValueEmpty_ThrowsArgumentOutOfRangeException(int value)
        {
            using var control = new DomainUpDown();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.SelectedIndex = value);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(1)]
        [InlineData(2)]
        public void DomainUpDown_SelectedIndex_SetInvalidValueNotEmpty_ThrowsArgumentOutOfRangeException(int value)
        {
            using var control = new DomainUpDown();
            control.Items.Add("Item");
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.SelectedIndex = value);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("NoSuchItem")]
        public void DomainUpDown_SelectedItem_SetEmpty_Nop(object value)
        {
            using var control = new SubDomainUpDown
            {
                SelectedItem = value
            };
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.Text);
            Assert.False(control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectedItem = value;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.Text);
            Assert.False(control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("Item1", 0, "Item1", "Item1", true)]
        [InlineData("Item2", 1, "Item2", "Item2", true)]
        [InlineData("NoSuchItem", -1, null, "", false)]
        [InlineData(null, -1, null, "", false)]
        public void DomainUpDown_SelectedItem_SetNotEmpty_GetReturnsExpected(object value, int expectedSelectedIndex, object expected, string expectedText, bool expectedUserEdit)
        {
            using var control = new SubDomainUpDown();
            control.Items.Add("Item1");
            control.Items.Add("Item2");

            control.SelectedItem = value;
            Assert.Equal(expectedSelectedIndex, control.SelectedIndex);
            Assert.Equal(expected, control.SelectedItem);
            Assert.Equal(expectedText, control.Text);
            Assert.False(control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectedItem = value;
            Assert.Equal(expectedSelectedIndex, control.SelectedIndex);
            Assert.Equal(expected, control.SelectedItem);
            Assert.Equal(expectedText, control.Text);
            Assert.False(control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);

            // Set no such item.
            control.SelectedItem = "NoSuchItem";
            Assert.Equal(expectedSelectedIndex, control.SelectedIndex);
            Assert.Equal(expected, control.SelectedItem);
            Assert.Equal(expectedText, control.Text);
            Assert.False(control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);

            // Set none.
            control.SelectedItem = null;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Equal(expectedText, control.Text);
            Assert.Equal(expectedUserEdit, control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("Item1", 0, "Item1", "Item1", true)]
        [InlineData("Item2", 1, "Item2", "Item2", true)]
        [InlineData("NoSuchItem", -1, null, "", false)]
        [InlineData(null, -1, null, "", false)]
        public void DomainUpDown_SelectedItem_SetUserEdit_GetReturnsExpected(object value, int expectedSelectedIndex, object expected, string expectedText, bool expectedUserEdit)
        {
            using var control = new SubDomainUpDown
            {
                UserEdit = true
            };
            control.Items.Add("Item1");
            control.Items.Add("Item2");

            control.SelectedItem = value;
            Assert.Equal(expectedSelectedIndex, control.SelectedIndex);
            Assert.Equal(expected, control.SelectedItem);
            Assert.Equal(expectedText, control.Text);
            Assert.Equal(!expectedUserEdit, control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectedItem = value;
            Assert.Equal(expectedSelectedIndex, control.SelectedIndex);
            Assert.Equal(expected, control.SelectedItem);
            Assert.Equal(expectedText, control.Text);
            Assert.Equal(!expectedUserEdit, control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);

            // Set no such item.
            control.SelectedItem = "NoSuchItem";
            Assert.Equal(expectedSelectedIndex, control.SelectedIndex);
            Assert.Equal(expected, control.SelectedItem);
            Assert.Equal(expectedText, control.Text);
            Assert.Equal(!expectedUserEdit, control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);

            // Set none.
            control.SelectedItem = null;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedItem);
            Assert.Equal(expectedText, control.Text);
            Assert.True(control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DomainUpDown_SelectedItem_SetWithHandler_CallsSelectedItemChanged()
        {
            using var control = new DomainUpDown();
            control.Items.Add("Item1");
            control.Items.Add("Item2");

            int textChangedCallCount = 0;
            int callCount = 0;
            EventHandler textChangedHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                textChangedCallCount++;
            };
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(textChangedCallCount - 1, callCount);
                callCount++;
            };
            control.TextChanged += textChangedHandler;
            control.SelectedItemChanged += handler;

            // Set different.
            control.SelectedItem = "Item1";
            Assert.Equal("Item1", control.SelectedItem);
            Assert.Equal(1, textChangedCallCount);
            Assert.Equal(1, callCount);

            // Set same.
            control.SelectedItem = "Item1";
            Assert.Equal("Item1", control.SelectedItem);
            Assert.Equal(1, textChangedCallCount);
            Assert.Equal(1, callCount);

            // Set different.
            control.SelectedItem = "Item2";
            Assert.Equal("Item2", control.SelectedItem);
            Assert.Equal(2, textChangedCallCount);
            Assert.Equal(2, callCount);

            // Set none.
            control.SelectedItem = null;
            Assert.Null(control.SelectedItem);
            Assert.Equal(2, textChangedCallCount);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.TextChanged -= textChangedHandler;
            control.SelectedItemChanged -= handler;
            control.SelectedItem = "Item1";
            Assert.Equal("Item1", control.SelectedItem);
            Assert.Equal(2, textChangedCallCount);
            Assert.Equal(2, callCount);
        }

        public static IEnumerable<object[]> Sorted_Set_TestData()
        {
            foreach (bool userEdit in new bool[] { true, false })
            {
                yield return new object[] { userEdit, true };
                yield return new object[] { userEdit, false };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Sorted_Set_TestData))]
        public void DomainUpDown_Sorted_Set_GetReturnsExpected(bool userEdit, bool value)
        {
            using var control = new SubDomainUpDown
            {
                UserEdit = userEdit,
                Sorted = value
            };
            Assert.Equal(value, control.Sorted);
            Assert.Empty(control.Items);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Empty(control.Items);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.Sorted = !value;
            Assert.Equal(!value, control.Sorted);
            Assert.Empty(control.Items);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Sorted_WithItems_TestData()
        {
            foreach (bool userEdit in new bool[] { true, false })
            {
                yield return new object[] { userEdit, true, new string[] { "a", "a", "B", "c", "d" } };
                yield return new object[] { userEdit, false, new string[] { "c", "B", "a", "a", "d" } };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Sorted_WithItems_TestData))]
        public void DomainUpDown_Sorted_SetWithItems_GetReturnsExpected(bool userEdit, bool value, string[] expectedItems)
        {
            using var control = new SubDomainUpDown();
            control.Items.Add("c");
            control.Items.Add("B");
            control.Items.Add("a");
            control.Items.Add("a");
            control.Items.Add("d");
            control.UserEdit = userEdit;

            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Equal(expectedItems, control.Items.Cast<string>());
            Assert.Equal(-1, control.SelectedIndex);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Equal(expectedItems, control.Items.Cast<string>());
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.Sorted = !value;
            Assert.Equal(!value, control.Sorted);
            Assert.Equal(new string[] { "a", "a", "B", "c", "d" }, control.Items.Cast<string>());
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Sorted_WithItemsWithSelection_TestData()
        {
            yield return new object[] { true, true, new string[] { "a", "a", "B", "c", "d" }, -1 };
            yield return new object[] { false, true, new string[] { "a", "a", "B", "c", "d" }, 1 };
            yield return new object[] { true, false, new string[] { "c", "B", "a", "a", "d" }, -1 };
            yield return new object[] { false, false, new string[] { "c", "B", "a", "a", "d" }, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Sorted_WithItemsWithSelection_TestData))]
        public void DomainUpDown_Sorted_SetWithItemsWithSelection_GetReturnsExpected(bool userEdit, bool value, string[] expectedItems, int expectedSelectedIndex)
        {
            using var control = new SubDomainUpDown();
            control.Items.Add("c");
            control.Items.Add("B");
            control.Items.Add("a");
            control.Items.Add("a");
            control.Items.Add("d");
            control.SelectedItem = "B";
            control.UserEdit = userEdit;

            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Equal(expectedItems, control.Items.Cast<string>());
            Assert.Equal(expectedSelectedIndex, control.SelectedIndex);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Equal(expectedItems, control.Items.Cast<string>());
            Assert.Equal(expectedSelectedIndex, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.Sorted = !value;
            Assert.Equal(!value, control.Sorted);
            Assert.Equal(new string[] { "a", "a", "B", "c", "d" }, control.Items.Cast<string>());
            Assert.Equal(expectedSelectedIndex, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Sorted_Set_TestData))]
        public void DomainUpDown_Sorted_SetWithHandle_GetReturnsExpected(bool userEdit, bool value)
        {
            using var control = new SubDomainUpDown
            {
                UserEdit = userEdit
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Empty(control.Items);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Empty(control.Items);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.Sorted = !value;
            Assert.Equal(!value, control.Sorted);
            Assert.Empty(control.Items);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Sorted_WithItems_TestData))]
        public void DomainUpDown_Sorted_SetWithItemsWithHandle_GetReturnsExpected(bool userEdit, bool value, string[] expectedItems)
        {
            using var control = new SubDomainUpDown();
            control.Items.Add("c");
            control.Items.Add("B");
            control.Items.Add("a");
            control.Items.Add("a");
            control.Items.Add("d");
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            control.UserEdit = userEdit;

            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Equal(expectedItems, control.Items.Cast<string>());
            Assert.Equal(-1, control.SelectedIndex);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Equal(expectedItems, control.Items.Cast<string>());
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.Sorted = !value;
            Assert.Equal(!value, control.Sorted);
            Assert.Equal(new string[] { "a", "a", "B", "c", "d" }, control.Items.Cast<string>());
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Sorted_WithItemsWithSelection_TestData))]
        public void DomainUpDown_Sorted_SetWithItemsWithSelectionWithHandle_GetReturnsExpected(bool userEdit, bool value, string[] expectedItems, int expectedSelectedIndex)
        {
            using var control = new SubDomainUpDown();
            control.Items.Add("c");
            control.Items.Add("B");
            control.Items.Add("a");
            control.Items.Add("a");
            control.Items.Add("d");
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            control.SelectedItem = "B";
            control.UserEdit = userEdit;

            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Equal(expectedItems, control.Items.Cast<string>());
            Assert.Equal(expectedSelectedIndex, control.SelectedIndex);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Sorted = value;
            Assert.Equal(value, control.Sorted);
            Assert.Equal(expectedItems, control.Items.Cast<string>());
            Assert.Equal(expectedSelectedIndex, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.Sorted = !value;
            Assert.Equal(!value, control.Sorted);
            Assert.Equal(new string[] { "a", "a", "B", "c", "d" }, control.Items.Cast<string>());
            Assert.Equal(expectedSelectedIndex, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DomainUpDown_Wrap_Set_GetReturnsExpected(bool value)
        {
            using var control = new DomainUpDown
            {
                Wrap = value
            };
            Assert.Equal(value, control.Wrap);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Wrap = value;
            Assert.Equal(value, control.Wrap);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.Wrap = !value;
            Assert.Equal(!value, control.Wrap);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DomainUpDown_Wrap_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new DomainUpDown();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Wrap = value;
            Assert.Equal(value, control.Wrap);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Wrap = value;
            Assert.Equal(value, control.Wrap);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.Wrap = !value;
            Assert.Equal(!value, control.Wrap);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void DomainUpDown_CreateAccessibilityInstance_Invoke_ReturnsExpected()
        {
            using var control = new SubDomainUpDown();
            Control.ControlAccessibleObject instance = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.NotNull(instance);
            Assert.Same(control, instance.Owner);
            Assert.Equal(AccessibleRole.SpinButton, instance.Role);
            Assert.NotSame(control.CreateAccessibilityInstance(), instance);
            Assert.NotSame(control.AccessibilityObject, instance);
        }

        [WinFormsFact]
        public void DomainUpDown_CreateAccessibilityInstance_InvokeWithCustomRole_ReturnsExpected()
        {
            using var control = new SubDomainUpDown
            {
                AccessibleRole = AccessibleRole.HelpBalloon
            };
            Control.ControlAccessibleObject instance = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.NotNull(instance);
            Assert.Same(control, instance.Owner);
            Assert.Equal(AccessibleRole.HelpBalloon, instance.Role);
            Assert.NotSame(control.CreateAccessibilityInstance(), instance);
            Assert.NotSame(control.AccessibilityObject, instance);
        }

        public static IEnumerable<object[]> DownButton_TestData()
        {
            foreach (bool userEdit in new bool[] { true, false })
            {
                foreach (bool wrap in new bool[] { true, false })
                {
                    yield return new object[] { userEdit, wrap };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(DownButton_TestData))]
        public void DomainUpDown_DownButton_InvokeWithoutItems_Nop(bool userEdit, bool wrap)
        {
            using var control = new SubDomainUpDown
            {
                UserEdit = userEdit,
                Wrap = wrap
            };

            control.DownButton();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);

            // Call again.
            control.DownButton();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);
        }

        [WinFormsTheory]
        [MemberData(nameof(DownButton_TestData))]
        public void DomainUpDown_DownButton_InvokeEmpty_Nop(bool userEdit, bool wrap)
        {
            using var control = new SubDomainUpDown
            {
                UserEdit = userEdit,
                Wrap = wrap
            };
            Assert.Empty(control.Items);

            control.DownButton();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);

            // Call again.
            control.DownButton();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);
        }

        public static IEnumerable<object[]> DownButton_WithItems_TestData()
        {
            yield return new object[] { true, true, 0 };
            yield return new object[] { true, false, 2 };
            yield return new object[] { false, true, 0 };
            yield return new object[] { false, false, 2 };
        }

        [WinFormsTheory]
        [MemberData(nameof(DownButton_WithItems_TestData))]
        public void DomainUpDown_DownButton_InvokeWithItems_Nop(bool userEdit, bool wrap, int expectedWrapSelectedIndex)
        {
            using var control = new SubDomainUpDown
            {
                UserEdit = userEdit,
                Wrap = wrap
            };
            control.Items.Add("a");
            control.Items.Add("b");
            control.Items.Add("c");

            control.DownButton();
            Assert.Equal(0, control.SelectedIndex);
            Assert.False(control.UserEdit);

            // Call again.
            control.DownButton();
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(control.UserEdit);

            // Call again.
            control.DownButton();
            Assert.Equal(2, control.SelectedIndex);
            Assert.False(control.UserEdit);

            // Call again.
            control.DownButton();
            Assert.Equal(expectedWrapSelectedIndex, control.SelectedIndex);
            Assert.False(control.UserEdit);
        }

        [WinFormsFact]
        public void DomainUpDown_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubDomainUpDown();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        [WinFormsTheory]
        [InlineData(0, true)]
        [InlineData(SubDomainUpDown.ScrollStateAutoScrolling, false)]
        [InlineData(SubDomainUpDown.ScrollStateFullDrag, false)]
        [InlineData(SubDomainUpDown.ScrollStateHScrollVisible, false)]
        [InlineData(SubDomainUpDown.ScrollStateUserHasScrolled, false)]
        [InlineData(SubDomainUpDown.ScrollStateVScrollVisible, false)]
        [InlineData(int.MaxValue, false)]
        [InlineData((-1), false)]
        public void DomainUpDown_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
        {
            using var control = new SubDomainUpDown();
            Assert.Equal(expected, control.GetScrollState(bit));
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, true)]
        [InlineData(ControlStyles.UserPaint, true)]
        [InlineData(ControlStyles.Opaque, true)]
        [InlineData(ControlStyles.ResizeRedraw, true)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, true)]
        [InlineData(ControlStyles.StandardClick, false)]
        [InlineData(ControlStyles.Selectable, true)]
        [InlineData(ControlStyles.UserMouse, false)]
        [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
        [InlineData(ControlStyles.StandardDoubleClick, true)]
        [InlineData(ControlStyles.AllPaintingInWmPaint, false)]
        [InlineData(ControlStyles.CacheText, false)]
        [InlineData(ControlStyles.EnableNotifyMessage, false)]
        [InlineData(ControlStyles.DoubleBuffer, false)]
        [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
        [InlineData(ControlStyles.UseTextForAccessibility, false)]
        [InlineData((ControlStyles)0, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void DomainUpDown_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubDomainUpDown();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void DomainUpDown_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubDomainUpDown();
            Assert.False(control.GetTopLevel());
        }

        [WinFormsTheory]
        [InlineData("cow", 0, 3)]
        [InlineData("cow", 4, 3)]
        [InlineData("foo", 0, 0)]
        [InlineData("foo", 3, 4)]
        [InlineData("foo", 4, 4)]
        [InlineData("foo", 5, 0)]
        [InlineData("foo", 100, 0)]
        [InlineData("foo", -1, 4)]
        [InlineData("foo", -100, 4)]
        [InlineData("foo2", 0, 1)]
        [InlineData("foo5", 0, -1)]
        [InlineData("foo5", 4, -1)]
        [InlineData("", 0, -1)]
        public void DomainUpDown_MatchIndex(string text, int start, int expected)
        {
            using var control = new DomainUpDown();
            control.Items.Add("foo1");
            control.Items.Add("foo2");
            control.Items.Add("foo3");
            control.Items.Add("Cowman");
            control.Items.Add("foo4");
            Assert.Equal(expected, control.MatchIndex(text, false, start));
        }

        [WinFormsFact]
        public void DomainUpDown_MatchIndex_NullText_ThrowsNullReferenceException()
        {
            using var control = new DomainUpDown();
            control.Items.Add("item1");
            Assert.Throws<NullReferenceException>(() => control.MatchIndex(null, false, 0));
        }

        public static IEnumerable<object[]> OnChanged_TestData()
        {
            yield return new object[] { null, null };
            yield return new object[] { new object(), new EventArgs() };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnChanged_TestData))]
        public void DomainUpDown_OnChanged_Invoke_CallsSelectedItemChanged(object source, EventArgs eventArgs)
        {
            using var control = new SubDomainUpDown();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.SelectedItemChanged += handler;
            control.OnSelectedItemChanged(source, eventArgs);
            Assert.Equal(1, callCount);

           // Remove handler.
           control.SelectedItemChanged -= handler;
           control.OnSelectedItemChanged(source, eventArgs);
           Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnChanged_TestData))]
        public void DomainUpDown_OnSelectedItemChanged_Invoke_CallsSelectedItemChanged(object source, EventArgs eventArgs)
        {
            using var control = new SubDomainUpDown();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.SelectedItemChanged += handler;
            control.OnSelectedItemChanged(source, eventArgs);
            Assert.Equal(1, callCount);

           // Remove handler.
           control.SelectedItemChanged -= handler;
           control.OnSelectedItemChanged(source, eventArgs);
           Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> UpButton_TestData()
        {
            foreach (bool userEdit in new bool[] { true, false })
            {
                foreach (bool wrap in new bool[] { true, false })
                {
                    yield return new object[] { userEdit, wrap };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(UpButton_TestData))]
        public void DomainUpDown_UpButton_InvokeWithoutItems_Nop(bool userEdit, bool wrap)
        {
            using var control = new SubDomainUpDown
            {
                UserEdit = userEdit,
                Wrap = wrap
            };

            control.UpButton();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);

            // Call again.
            control.UpButton();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);
        }

        [WinFormsTheory]
        [MemberData(nameof(UpButton_TestData))]
        public void DomainUpDown_UpButton_InvokeEmpty_Nop(bool userEdit, bool wrap)
        {
            using var control = new SubDomainUpDown
            {
                UserEdit = userEdit,
                Wrap = wrap
            };
            Assert.Empty(control.Items);

            control.UpButton();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);

            // Call again.
            control.UpButton();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(userEdit, control.UserEdit);
        }

        public static IEnumerable<object[]> UpButton_WithItems_TestData()
        {
            yield return new object[] { true, true, 2, 1 };
            yield return new object[] { true, false, 0, 0 };
            yield return new object[] { false, true, 2, 1 };
            yield return new object[] { false, false, 0, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(UpButton_WithItems_TestData))]
        public void DomainUpDown_UpButton_InvokeWithItems_Nop(bool userEdit, bool wrap, int expectedWrapSelectedIndex1, int expectedWrapSelectedIndex2)
        {
            using var control = new SubDomainUpDown
            {
                UserEdit = userEdit,
                Wrap = wrap
            };
            control.Items.Add("a");
            control.Items.Add("b");
            control.Items.Add("c");
            control.SelectedIndex = 2;

            control.UpButton();
            Assert.Equal(1, control.SelectedIndex);
            Assert.False(control.UserEdit);

            // Call again.
            control.UpButton();
            Assert.Equal(0, control.SelectedIndex);
            Assert.False(control.UserEdit);

            // Call again.
            control.UpButton();
            Assert.Equal(expectedWrapSelectedIndex1, control.SelectedIndex);
            Assert.False(control.UserEdit);

            // Call again.
            control.UpButton();
            Assert.Equal(expectedWrapSelectedIndex2, control.SelectedIndex);
            Assert.False(control.UserEdit);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void DomainUpDown_UpdateEditText_InvokeEmpty_Success(bool userEdit, bool changingText)
        {
            using var control = new SubDomainUpDown
            {
                UserEdit = userEdit,
                ChangingText = changingText
            };
            control.UpdateEditText();
            Assert.Empty(control.Text);
            Assert.False(control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.UpdateEditText();
            Assert.Empty(control.Text);
            Assert.False(control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void DomainUpDown_UpdateEditText_InvokeNotEmpty_Success(bool userEdit, bool changingText)
        {
            using var control = new SubDomainUpDown();
            control.Items.Add("Item1");
            control.Items.Add("Item2");
            control.UserEdit = userEdit;
            control.ChangingText = changingText;

            control.UpdateEditText();
            Assert.Empty(control.Text);
            Assert.False(control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.UpdateEditText();
            Assert.Empty(control.Text);
            Assert.False(control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void DomainUpDown_UpdateEditText_InvokeNotEmptyWithSelection_Success(bool userEdit, bool changingText)
        {
            using var control = new SubDomainUpDown();
            control.Items.Add("Item1");
            control.Items.Add("Item2");
            control.SelectedIndex = 0;
            control.Text = "Text";
            control.UserEdit = userEdit;
            control.ChangingText = changingText;

            control.UpdateEditText();
            Assert.Equal("Item1", control.Text);
            Assert.False(control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.UpdateEditText();
            Assert.Equal("Item1", control.Text);
            Assert.False(control.UserEdit);
            Assert.False(control.ChangingText);
            Assert.False(control.IsHandleCreated);
        }

        public class SubDomainUpDown : DomainUpDown
        {
            public new const int ScrollStateAutoScrolling = DomainUpDown.ScrollStateAutoScrolling;

            public new const int ScrollStateHScrollVisible = DomainUpDown.ScrollStateHScrollVisible;

            public new const int ScrollStateVScrollVisible = DomainUpDown.ScrollStateVScrollVisible;

            public new const int ScrollStateUserHasScrolled = DomainUpDown.ScrollStateUserHasScrolled;

            public new const int ScrollStateFullDrag = DomainUpDown.ScrollStateFullDrag;

            public new SizeF AutoScaleFactor => base.AutoScaleFactor;

            public new bool CanEnableIme => base.CanEnableIme;

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new bool ChangingText
            {
                get => base.ChangingText;
                set => base.ChangingText = value;
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

            public new int FontHeight
            {
                get => base.FontHeight;
                set => base.FontHeight = value;
            }

            public new bool HScroll
            {
                get => base.HScroll;
                set => base.HScroll = value;
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

            public new bool UserEdit
            {
                get => base.UserEdit;
                set => base.UserEdit = value;
            }

            public new bool VScroll
            {
                get => base.VScroll;
                set => base.VScroll = value;
            }

            public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetScrollState(int bit) => base.GetScrollState(bit);

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public new void OnChanged(object source, EventArgs e) => base.OnChanged(source, e);

            public new void OnTextBoxKeyPress(object source, KeyPressEventArgs e) => base.OnTextBoxKeyPress(source, e);

            public new void OnSelectedItemChanged(object source, EventArgs e) => base.OnSelectedItemChanged(source, e);

            public new void UpdateEditText() => base.UpdateEditText();

            public new void ValidateEditText() => base.ValidateEditText();

            public new void WndProc(ref Message m) => base.WndProc(ref m);
        }
    }
}

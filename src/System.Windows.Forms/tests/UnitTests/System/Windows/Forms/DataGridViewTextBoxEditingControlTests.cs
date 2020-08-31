// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class DataGridViewTextBoxEditingControlTests : IClassFixture<ThreadExceptionFixture>
    {
        private static int s_preferredHeight = Control.DefaultFont.Height + SystemInformation.BorderSize.Height * 4 + 3;

        [WinFormsFact]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_Ctor_Default()
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
            Assert.False(control.AcceptsReturn);
            Assert.False(control.AcceptsTab);
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.Empty(control.AutoCompleteCustomSource);
            Assert.Same(control.AutoCompleteCustomSource, control.AutoCompleteCustomSource);
            Assert.Equal(AutoCompleteMode.None, control.AutoCompleteMode);
            Assert.Equal(AutoCompleteSource.None, control.AutoCompleteSource);
            Assert.True(control.AutoSize);
            Assert.Equal(SystemColors.Window, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(BorderStyle.Fixed3D, control.BorderStyle);
            Assert.Equal(control.PreferredHeight, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 100, control.PreferredHeight), control.Bounds);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CanSelect);
            Assert.False(control.CanUndo);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(CharacterCasing.Normal, control.CharacterCasing);
            Assert.Equal(new Size(96, control.PreferredHeight - 4), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 96, control.PreferredHeight - 4), control.ClientRectangle);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.Same(Cursors.IBeam, control.Cursor);
            Assert.Same(Cursors.IBeam, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(new Size(100, control.PreferredHeight), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 96, control.PreferredHeight - 4), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.DoubleBuffered);
            Assert.Null(control.EditingControlDataGridView);
            Assert.Empty(Assert.IsType<string>(control.EditingControlFormattedValue));
            Assert.Equal(0, control.EditingControlRowIndex);
            Assert.False(control.EditingControlValueChanged);
            Assert.Same(Cursors.Default, control.EditingPanelCursor);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(control.PreferredHeight, control.Height);
            Assert.True(control.HideSelection);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Empty(control.Lines);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(32767, control.MaxLength);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.False(control.Modified);
            Assert.False(control.Multiline);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.Equal(4, control.PreferredSize.Width);
            Assert.True(control.PreferredSize.Height > 0);
            Assert.True(control.PreferredHeight > 0);
            Assert.False(control.ReadOnly);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.False(control.RepositionEditingControlOnValueChange);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(100, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(ScrollBars.None, control.ScrollBars);
            Assert.Empty(control.SelectedText);
            Assert.Equal(0, control.SelectionLength);
            Assert.Equal(0, control.SelectionStart);
            Assert.True(control.ShortcutsEnabled);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(100, control.PreferredHeight), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.False(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(HorizontalAlignment.Left, control.TextAlign);
            Assert.Equal(0, control.TextLength);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseSystemPasswordChar);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.Equal(100, control.Width);
            Assert.True(control.WordWrap);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("Edit", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(control.PreferredHeight, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x560000C0, createParams.Style);
            Assert.Equal(100, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> EditingControlDataGridView_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new DataGridView() };
        }

        [WinFormsTheory]
        [MemberData(nameof(EditingControlDataGridView_Set_TestData))]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_EditingControlDataGridView_Set_GetReturnsExpected(DataGridView value)
        {
            using var control = new DataGridViewTextBoxEditingControl
            {
                EditingControlDataGridView = value
            };
            Assert.Same(value, control.EditingControlDataGridView);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.EditingControlDataGridView = value;
            Assert.Same(value, control.EditingControlDataGridView);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData(1)]
        [InlineData("CustomValue")]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_EditingControlFormattedValue_GetCustomGetEditingControlFormattedValue_ReturnsExpected(object result)
        {
            int callCount = 0;
            object action(DataGridViewDataErrorContexts context)
            {
                Assert.Equal(DataGridViewDataErrorContexts.Formatting, context);
                callCount++;
                return result;
            }
            using var control = new CustomGetEditingControlFormattedValueDataGridViewTextBoxEditingControl
            {
                GetEditingControlFormattedValueAction = action
            };
            Assert.Equal(result, control.EditingControlFormattedValue);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [InlineData(null, "", false)]
        [InlineData("", "", false)]
        [InlineData("text", "text", true)]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_EditingControlFormattedValue_Set_GetReturnsExpected(string value, string expected, bool expectedValueChanged)
        {
            using var control = new DataGridViewTextBoxEditingControl
            {
                EditingControlFormattedValue = value
            };
            Assert.Equal(expected, control.EditingControlFormattedValue);
            Assert.Equal(expectedValueChanged, control.EditingControlValueChanged);
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.EditingControlFormattedValue = value;
            Assert.Equal(expected, control.EditingControlFormattedValue);
            Assert.Equal(expectedValueChanged, control.EditingControlValueChanged);
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null, "", false)]
        [InlineData("", "", false)]
        [InlineData("text", "text", true)]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_EditingControlFormattedValue_SetWithDataGridView_GetReturnsExpected(string value, string expected, bool expectedValueChanged)
        {
            using var dataGridView = new DataGridView
            {
                ColumnCount = 1
            };
            using var control = new DataGridViewTextBoxEditingControl
            {
                EditingControlDataGridView = dataGridView,
                EditingControlFormattedValue = value
            };
            Assert.Equal(expected, control.EditingControlFormattedValue);
            Assert.Equal(expectedValueChanged, control.EditingControlValueChanged);
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.Equal(expectedValueChanged, dataGridView.IsCurrentCellDirty);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.EditingControlFormattedValue = value;
            Assert.Equal(expected, control.EditingControlFormattedValue);
            Assert.Equal(expectedValueChanged, control.EditingControlValueChanged);
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.Equal(expectedValueChanged, dataGridView.IsCurrentCellDirty);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_EditingControlFormattedValue_SetWithDataGridViewNoColumns_GetReturnsExpected()
        {
            using var dataGridView = new DataGridView();
            using var control = new DataGridViewTextBoxEditingControl
            {
                EditingControlDataGridView = dataGridView
            };
            Assert.Throws<InvalidOperationException>(() => control.EditingControlFormattedValue = "text");
            Assert.Equal("text", control.EditingControlFormattedValue);
            Assert.Equal("text", control.Text);
            Assert.Equal(4, control.TextLength);
            Assert.True(control.EditingControlValueChanged);
            Assert.True(dataGridView.IsCurrentCellDirty);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.EditingControlFormattedValue = "text";
            Assert.Equal("text", control.EditingControlFormattedValue);
            Assert.Equal("text", control.Text);
            Assert.Equal(4, control.TextLength);
            Assert.True(control.EditingControlValueChanged);
            Assert.True(dataGridView.IsCurrentCellDirty);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_EditingControlFormattedValue_SetNonStringValue_ThrowsInvalidCastException()
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
            Assert.Throws<InvalidCastException>(() => control.EditingControlFormattedValue = new object());
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_EditingControlRowIndex_Set_GetReturnsExpected(int value)
        {
            using var control = new DataGridViewTextBoxEditingControl
            {
                EditingControlRowIndex = value
            };
            Assert.Equal(value, control.EditingControlRowIndex);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.EditingControlRowIndex = value;
            Assert.Equal(value, control.EditingControlRowIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_EditingControlValueChanged_Set_GetReturnsExpected(bool value)
        {
            using var control = new DataGridViewTextBoxEditingControl
            {
                EditingControlValueChanged = value
            };
            Assert.Equal(value, control.EditingControlValueChanged);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.EditingControlValueChanged = value;
            Assert.Equal(value, control.EditingControlValueChanged);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.EditingControlValueChanged = !value;
            Assert.Equal(!value, control.EditingControlValueChanged);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null, "", false)]
        [InlineData("", "", false)]
        [InlineData("text", "text", true)]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_Text_Set_GetReturnsExpected(string value, string expected, bool expectedValueChanged)
        {
            using var control = new DataGridViewTextBoxEditingControl
            {
                Text = value
            };
            Assert.Equal(expected, control.EditingControlFormattedValue);
            Assert.Equal(expectedValueChanged, control.EditingControlValueChanged);
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.EditingControlFormattedValue);
            Assert.Equal(expectedValueChanged, control.EditingControlValueChanged);
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null, "", false)]
        [InlineData("", "", false)]
        [InlineData("text", "text", true)]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_Text_SetWithDataGridView_GetReturnsExpected(string value, string expected, bool expectedValueChanged)
        {
            using var dataGridView = new DataGridView
            {
                ColumnCount = 1
            };
            using var control = new DataGridViewTextBoxEditingControl
            {
                EditingControlDataGridView = dataGridView,
                Text = value
            };
            Assert.Equal(expected, control.EditingControlFormattedValue);
            Assert.Equal(expectedValueChanged, control.EditingControlValueChanged);
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.Equal(expectedValueChanged, dataGridView.IsCurrentCellDirty);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.EditingControlFormattedValue);
            Assert.Equal(expectedValueChanged, control.EditingControlValueChanged);
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.Equal(expectedValueChanged, dataGridView.IsCurrentCellDirty);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_Text_SetWithDataGridViewNoColumns_GetReturnsExpected()
        {
            using var dataGridView = new DataGridView();
            using var control = new DataGridViewTextBoxEditingControl
            {
                EditingControlDataGridView = dataGridView
            };
            Assert.Throws<InvalidOperationException>(() => control.Text = "text");
            Assert.Equal("text", control.EditingControlFormattedValue);
            Assert.True(control.EditingControlValueChanged);
            Assert.Equal("text", control.Text);
            Assert.Equal(4, control.TextLength);
            Assert.True(dataGridView.IsCurrentCellDirty);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Text = "text";
            Assert.Equal("text", control.EditingControlFormattedValue);
            Assert.True(control.EditingControlValueChanged);
            Assert.Equal("text", control.Text);
            Assert.Equal(4, control.TextLength);
            Assert.True(dataGridView.IsCurrentCellDirty);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null, "", false)]
        [InlineData("", "", false)]
        [InlineData("text", "text", true)]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_Text_SetWithHandle_GetReturnsExpected(string value, string expected, bool expectedValueChanged)
        {
            using var control = new DataGridViewTextBoxEditingControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Text = value;
            Assert.Equal(expected, control.EditingControlFormattedValue);
            Assert.Equal(expectedValueChanged, control.EditingControlValueChanged);
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.EditingControlFormattedValue);
            Assert.Equal(expectedValueChanged, control.EditingControlValueChanged);
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_Text_SetWithHandler_CallsTextChanged()
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
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

        [WinFormsFact]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_Text_SetWithHandlerWithHandle_CallsTextChanged()
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
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
                Assert.Equal(EventArgs.Empty, e);
                callCount++;
            };
            control.TextChanged += handler;

            // Set different.
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.Text = null;
            Assert.Empty(control.Text);
            Assert.Equal(2, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.TextChanged -= handler;
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(2, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> ApplyCellStyleToEditingDataGridViewTextBoxEditingControl_TestData()
        {
            yield return new object[] { new DataGridViewCellStyle(), null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Left, false };

            using var font = new Font("Arial", 8.25f);
            var customStyle = new DataGridViewCellStyle
            {
                Font = font,
                BackColor = Color.Gray,
                ForeColor = Color.Green
            };
            yield return new object[] { customStyle, font, Color.Gray, Color.Green, false, HorizontalAlignment.Left, false };

            var transparentStyle = new DataGridViewCellStyle
            {
                Font = font,
                BackColor = Color.FromArgb(0x12, 0x34, 0x56, 0x78),
                ForeColor = Color.FromArgb(0x23, 0x45, 0x67, 0x80)
            };
            yield return new object[] { transparentStyle, font, Color.FromArgb(0xFF, 0x34, 0x56, 0x78), Color.FromArgb(0x23, 0x45, 0x67, 0x80), false, HorizontalAlignment.Left, false };

            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.BottomCenter
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Center, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.BottomCenter
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, true, HorizontalAlignment.Center, true
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.BottomLeft
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Left, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.BottomLeft
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, true, HorizontalAlignment.Left, true
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.BottomRight
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Right, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.BottomRight
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, true, HorizontalAlignment.Right, true
            };

            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Center, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, true, HorizontalAlignment.Center, true
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Left, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, true, HorizontalAlignment.Left, true
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Right, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, true, HorizontalAlignment.Right, true
            };

            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.TopCenter
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Center, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.TopCenter
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, true, HorizontalAlignment.Center, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.TopLeft
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Left, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.TopLeft
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, true, HorizontalAlignment.Left, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.TopRight
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Right, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.TopRight
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, true, HorizontalAlignment.Right, false
            };
        }

        [WinFormsTheory]
        [MemberData(nameof(ApplyCellStyleToEditingDataGridViewTextBoxEditingControl_TestData))]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_ApplyCellStyleToEditingDataGridViewTextBoxEditingControl_Invoke_Success(DataGridViewCellStyle dataGridViewCellStyle, Font expectedFont, Color expectedBackColor, Color expectedForeColor, bool expectedWordWrap, HorizontalAlignment expectedTextAlign, bool expectedRepositionOnValueChange)
        {
            using var oldFont = new Font("Arial", 8.25f);
            using var control = new DataGridViewTextBoxEditingControl
            {
                Font = oldFont,
                BackColor = Color.Red,
                ForeColor = Color.Blue,
                WordWrap = false
            };
            control.ApplyCellStyleToEditingControl(dataGridViewCellStyle);
            Assert.Equal(expectedFont ?? Control.DefaultFont, control.Font);
            Assert.Equal(expectedBackColor, control.BackColor);
            Assert.Equal(expectedForeColor, control.ForeColor);
            Assert.Equal(expectedWordWrap, control.WordWrap);
            Assert.Equal(expectedTextAlign, control.TextAlign);
            Assert.Equal(expectedRepositionOnValueChange, control.RepositionEditingControlOnValueChange);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> ApplyCellStyleToEditingDataGridViewTextBoxEditingControl_WithDataGridView_TestData()
        {
            yield return new object[] { new DataGridViewCellStyle(), null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Left, false };

            using var font = new Font("Arial", 8.25f);
            var customStyle = new DataGridViewCellStyle
            {
                Font = font,
                BackColor = Color.Gray,
                ForeColor = Color.Green
            };
            yield return new object[] { customStyle, font, Color.Gray, SystemColors.Control, Color.Green, false, HorizontalAlignment.Left, false };

            var transparentStyle = new DataGridViewCellStyle
            {
                Font = font,
                BackColor = Color.FromArgb(0x12, 0x34, 0x56, 0x78),
                ForeColor = Color.FromArgb(0x23, 0x45, 0x67, 0x80)
            };
            yield return new object[] { transparentStyle, font, Color.FromArgb(0xFF, 0x34, 0x56, 0x78), Color.FromArgb(0xFF, 0x34, 0x56, 0x78), Color.FromArgb(0x23, 0x45, 0x67, 0x80), false, HorizontalAlignment.Left, false };

            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.BottomCenter
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Center, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.BottomCenter
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, true, HorizontalAlignment.Center, true
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.BottomLeft
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Left, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.BottomLeft
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, true, HorizontalAlignment.Left, true
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.BottomRight
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Right, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.BottomRight
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, true, HorizontalAlignment.Right, true
            };

            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Center, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, true, HorizontalAlignment.Center, true
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Left, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, true, HorizontalAlignment.Left, true
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Right, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, true, HorizontalAlignment.Right, true
            };

            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.TopCenter
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Center, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.TopCenter
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, true, HorizontalAlignment.Center, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.TopLeft
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Left, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.TopLeft
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, true, HorizontalAlignment.Left, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.TopRight
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, false, HorizontalAlignment.Right, false
            };
            yield return new object[]
            {
                new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.TopRight
                },
                null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xFF, 0x00, 0x00, 0x00), SystemColors.WindowText, true, HorizontalAlignment.Right, false
            };
        }

        [WinFormsTheory]
        [MemberData(nameof(ApplyCellStyleToEditingDataGridViewTextBoxEditingControl_WithDataGridView_TestData))]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_ApplyCellStyleToEditingDataGridViewTextBoxEditingControl_InvokeWithDataGridView_Success(DataGridViewCellStyle dataGridViewCellStyle, Font expectedFont, Color expectedBackColor, Color expectedEditingPanelBackColor, Color expectedForeColor, bool expectedWordWrap, HorizontalAlignment expectedTextAlign, bool expectedRepositionOnValueChange)
        {
            using var dataGridView = new DataGridView();
            using var oldFont = new Font("Arial", 8.25f);
            using var control = new DataGridViewTextBoxEditingControl
            {
                EditingControlDataGridView = dataGridView,
                Font = oldFont,
                BackColor = Color.Red,
                ForeColor = Color.Blue,
                WordWrap = false
            };
            control.ApplyCellStyleToEditingControl(dataGridViewCellStyle);
            Assert.Equal(expectedFont ?? Control.DefaultFont, control.Font);
            Assert.Equal(expectedBackColor, control.BackColor);
            Assert.Equal(expectedEditingPanelBackColor, dataGridView.EditingPanel.BackColor);
            Assert.Equal(expectedForeColor, control.ForeColor);
            Assert.Equal(expectedWordWrap, control.WordWrap);
            Assert.Equal(expectedTextAlign, control.TextAlign);
            Assert.Equal(expectedRepositionOnValueChange, control.RepositionEditingControlOnValueChange);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_ApplyCellStyleToEditingDataGridViewTextBoxEditingControl_NullDataGridViewCellStyle_ThrowsArgumentNullException()
        {
            using var control = new DataGridViewTextBoxEditingControl();
            Assert.Throws<ArgumentNullException>("dataGridViewCellStyle", () => control.ApplyCellStyleToEditingControl(null));
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Text)]
        [InlineData(false, AccessibleRole.None)]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_CreateAccessibilityInstance_Invoke_ReturnsExpected(bool createControl, AccessibleRole expectedAccessibleRole)
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
            if (createControl)
            {
                control.CreateControl();
            }

            Assert.Equal(createControl, control.IsHandleCreated);
            Control.ControlAccessibleObject instance = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.Equal(createControl, control.IsHandleCreated);
            Assert.NotNull(instance);
            Assert.Same(control, instance.Owner);
            Assert.Equal(expectedAccessibleRole, instance.Role);
            Assert.NotSame(control.CreateAccessibilityInstance(), instance);
            Assert.NotSame(control.Text, instance);
        }

        [WinFormsFact]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_CreateAccessibilityInstance_InvokeWithCustomRole_ReturnsExpected()
        {
            using var control = new SubDataGridViewTextBoxEditingControl
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

        public static IEnumerable<object[]> EditingControlWantsInputKey_TestData()
        {
            foreach (bool valueChanged in new bool[] { true, false })
            {
                foreach (bool multiline in new bool[] { true, false })
                {
                    foreach (bool acceptsReturn in new bool[] { true, false })
                    {
                        foreach (bool dataGridViewWantsInputKey in new bool[] { true, false })
                        {
                            yield return new object[] { RightToLeft.Yes, valueChanged, multiline, acceptsReturn, Keys.Right, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.No, valueChanged, multiline, acceptsReturn, Keys.Right, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.Yes, valueChanged, multiline, acceptsReturn, Keys.Left, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.No, valueChanged, multiline, acceptsReturn, Keys.Left, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.Yes, valueChanged, multiline, acceptsReturn, Keys.Down, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.No, valueChanged, multiline, acceptsReturn, Keys.Down, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.Yes, valueChanged, multiline, acceptsReturn, Keys.Up, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.No, valueChanged, multiline, acceptsReturn, Keys.Up, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.Yes, valueChanged, multiline, acceptsReturn, Keys.Home, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.No, valueChanged, multiline, acceptsReturn, Keys.Home, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.Yes, valueChanged, multiline, acceptsReturn, Keys.End, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.No, valueChanged, multiline, acceptsReturn, Keys.End, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.Yes, valueChanged, multiline, acceptsReturn, Keys.Prior, dataGridViewWantsInputKey, !dataGridViewWantsInputKey || valueChanged };
                            yield return new object[] { RightToLeft.No, valueChanged, multiline, acceptsReturn, Keys.Prior, dataGridViewWantsInputKey, !dataGridViewWantsInputKey || valueChanged };
                            yield return new object[] { RightToLeft.Yes, valueChanged, multiline, acceptsReturn, Keys.Next, dataGridViewWantsInputKey, !dataGridViewWantsInputKey || valueChanged };
                            yield return new object[] { RightToLeft.No, valueChanged, multiline, acceptsReturn, Keys.Next, dataGridViewWantsInputKey, !dataGridViewWantsInputKey || valueChanged };
                            yield return new object[] { RightToLeft.Yes, valueChanged, multiline, acceptsReturn, Keys.Delete, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.No, valueChanged, multiline, acceptsReturn, Keys.Delete, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.Yes, valueChanged, multiline, acceptsReturn, Keys.Enter, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.No, valueChanged, multiline, acceptsReturn, Keys.Enter, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.Yes, valueChanged, multiline, acceptsReturn, Keys.Enter | Keys.Shift | Keys.Control, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.No, valueChanged, multiline, acceptsReturn, Keys.Enter | Keys.Shift | Keys.Control, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.Yes, valueChanged, multiline, acceptsReturn, Keys.Enter | Keys.Shift, dataGridViewWantsInputKey, !dataGridViewWantsInputKey || (multiline && acceptsReturn) };
                            yield return new object[] { RightToLeft.No, valueChanged, multiline, acceptsReturn, Keys.Enter | Keys.Shift, dataGridViewWantsInputKey, !dataGridViewWantsInputKey || (multiline && acceptsReturn) };
                            yield return new object[] { RightToLeft.Yes, valueChanged, multiline, acceptsReturn, Keys.A, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                            yield return new object[] { RightToLeft.No, valueChanged, multiline, acceptsReturn, Keys.A, dataGridViewWantsInputKey, !dataGridViewWantsInputKey };
                        }
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(EditingControlWantsInputKey_TestData))]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_EditingControlWantsInputKey_InvokeEmpty_ReturnsExpected(RightToLeft rightToLeft, bool valueChanged, bool multiline, bool acceptsReturn, Keys keyData, bool dataGridViewWantsInputKey, bool expected)
        {
            using var control = new DataGridViewTextBoxEditingControl
            {
                RightToLeft = rightToLeft,
                EditingControlValueChanged = valueChanged,
                Multiline = multiline,
                AcceptsReturn = acceptsReturn
            };
            Assert.Equal(expected, control.EditingControlWantsInputKey(keyData, dataGridViewWantsInputKey));
            Assert.False(control.IsHandleCreated);

            // Call again.
            Assert.Equal(expected, control.EditingControlWantsInputKey(keyData, dataGridViewWantsInputKey));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DataGridViewDataErrorContexts))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DataGridViewDataErrorContexts))]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_GetEditingControlFormattedValue_Invoke_ReturnsExpected(DataGridViewDataErrorContexts context)
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
            Assert.Empty(Assert.IsType<string>(control.GetEditingControlFormattedValue(context)));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DataGridViewDataErrorContexts))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DataGridViewDataErrorContexts))]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_GetEditingControlFormattedValue_InvokeWithText_ReturnsExpected(DataGridViewDataErrorContexts context)
        {
            using var control = new SubDataGridViewTextBoxEditingControl
            {
                Text = "text"
            };
            Assert.Equal("text", Assert.IsType<string>(control.GetEditingControlFormattedValue(context)));
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, false)]
        [InlineData(ControlStyles.UserPaint, false)]
        [InlineData(ControlStyles.Opaque, false)]
        [InlineData(ControlStyles.ResizeRedraw, false)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, true)]
        [InlineData(ControlStyles.StandardClick, false)]
        [InlineData(ControlStyles.Selectable, true)]
        [InlineData(ControlStyles.UserMouse, false)]
        [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
        [InlineData(ControlStyles.StandardDoubleClick, false)]
        [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
        [InlineData(ControlStyles.CacheText, false)]
        [InlineData(ControlStyles.EnableNotifyMessage, false)]
        [InlineData(ControlStyles.DoubleBuffer, false)]
        [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
        [InlineData(ControlStyles.UseTextForAccessibility, false)]
        [InlineData((ControlStyles)0, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
            Assert.False(control.GetTopLevel());
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_OnGotFocus_Invoke_CallsGotFocus(EventArgs eventArgs)
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.GotFocus += handler;
            control.OnGotFocus(eventArgs);
            Assert.Equal(1, callCount);

            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.GotFocus -= handler;
            control.OnGotFocus(eventArgs);
            Assert.Equal(1, callCount);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_OnGotFocus_InvokeWithHandle_CallsGotFocus(EventArgs eventArgs)
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
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
            control.GotFocus += handler;
            control.OnGotFocus(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.GotFocus -= handler;
            control.OnGotFocus(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridViewTextBoxEditingControl_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
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
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridViewTextBoxEditingControl_OnHandleCreated_InvokeWithDataGridView_CallsHandleCreated(EventArgs eventArgs)
        {
            using var dataGridView = new DataGridView();
            using var control = new SubDataGridViewTextBoxEditingControl
            {
                EditingControlDataGridView = dataGridView
            };
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
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnHandleCreated_WithHandle_TestData()
        {
            foreach (bool userPaint in new bool[] { true, false })
            {
                yield return new object[] { userPaint, null };
                yield return new object[] { userPaint, new EventArgs() };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnHandleCreated_WithHandle_TestData))]
        public void DataGridViewTextBoxEditingControl_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(bool userPaint, EventArgs eventArgs)
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
            control.SetStyle(ControlStyles.UserPaint, userPaint);
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
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_OnMouseWheel_Invoke_CallsMouseWheel(MouseEventArgs eventArgs)
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) => callCount++;

            // Call with handler.
            control.MouseWheel += handler;
            control.OnMouseWheel(eventArgs);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.MouseWheel -= handler;
            control.OnMouseWheel(eventArgs);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_OnMouseWheel_InvokeWithDataGridView_CallsMouseWheel(MouseEventArgs eventArgs)
        {
            using var dataGridView = new DataGridView();
            int dataGridViewCallCount = 0;
            dataGridView.MouseWheel += (sender, e) =>
            {
                Assert.Same(dataGridView, sender);
                Assert.Same(eventArgs, e);
                dataGridViewCallCount++;
            };
            using var control = new SubDataGridViewTextBoxEditingControl
            {
                EditingControlDataGridView = dataGridView
            };
            int callCount = 0;
            MouseEventHandler handler = (sender, e) => callCount++;

            // Call with handler.
            control.MouseWheel += handler;
            control.OnMouseWheel(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(1, dataGridViewCallCount);

            // Remove handler.
            control.MouseWheel -= handler;
            control.OnMouseWheel(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(2, dataGridViewCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_OnTextChanged_Invoke_CallsTextChanged(EventArgs eventArgs)
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
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
            Assert.True(control.EditingControlValueChanged);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.TextChanged -= handler;
            control.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.EditingControlValueChanged);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_OnTextChanged_InvokeWithDataGridView_CallsTextChanged(EventArgs eventArgs)
        {
            using var dataGridView = new DataGridView
            {
                ColumnCount = 1
            };
            using var control = new SubDataGridViewTextBoxEditingControl
            {
                EditingControlDataGridView = dataGridView
            };
            int callCount = 0;
            EventHandler handler = (sender, e) => callCount++;

            // Call with handler.
            control.TextChanged += handler;
            control.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.EditingControlValueChanged);
            Assert.True(dataGridView.IsCurrentCellDirty);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.TextChanged -= handler;
            control.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.EditingControlValueChanged);
            Assert.True(dataGridView.IsCurrentCellDirty);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridViewTextBoxEditingDataGridViewTextBoxEditingControl_OnTextChanged_InvokeWithDataGridViewNoColumns_CallsTextChanged(EventArgs eventArgs)
        {
            using var dataGridView = new DataGridView();
            using var control = new SubDataGridViewTextBoxEditingControl
            {
                EditingControlDataGridView = dataGridView
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.TextChanged += handler;
            Assert.Throws<InvalidOperationException>(() => control.OnTextChanged(eventArgs));
            Assert.Equal(1, callCount);
            Assert.True(control.EditingControlValueChanged);
            Assert.True(dataGridView.IsCurrentCellDirty);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.TextChanged -= handler;
            control.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.EditingControlValueChanged);
            Assert.True(dataGridView.IsCurrentCellDirty);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewTextBoxEditingControl_PrepareEditingControlForEdit_InvokeSelectAll_Success()
        {
            var dataGridView = new DataGridView
            {
                ColumnCount = 1
            };
            using var control = new DataGridViewTextBoxEditingControl
            {
                EditingControlDataGridView = dataGridView,
                Text = "text"
            };
            control.PrepareEditingControlForEdit(selectAll: true);
            Assert.Equal("text", control.SelectedText);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(4, control.SelectionLength);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.PrepareEditingControlForEdit(selectAll: true);
            Assert.Equal("text", control.SelectedText);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(4, control.SelectionLength);
            Assert.False(control.IsHandleCreated);

            // Deselect.
            control.PrepareEditingControlForEdit(selectAll: false);
            Assert.Empty(control.SelectedText);
            Assert.Equal(4, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewTextBoxEditingControl_PrepareEditingControlForEdit_InvokeNotSelectAll_Success()
        {
            var dataGridView = new DataGridView
            {
                ColumnCount = 1
            };
            using var control = new DataGridViewTextBoxEditingControl
            {
                EditingControlDataGridView = dataGridView,
                Text = "text"
            };
            control.PrepareEditingControlForEdit(selectAll: false);
            Assert.Empty(control.SelectedText);
            Assert.Equal(4, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.PrepareEditingControlForEdit(selectAll: false);
            Assert.Empty(control.SelectedText);
            Assert.Equal(4, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.False(control.IsHandleCreated);

            // Select all.
            control.PrepareEditingControlForEdit(selectAll: true);
            Assert.Equal("text", control.SelectedText);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(4, control.SelectionLength);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> ProcessKeyEventArgs_TestData()
        {
            foreach (bool handled in new bool[] { true, false })
            {
                foreach (IntPtr wParam in new IntPtr[] { (IntPtr)Keys.Enter, (IntPtr)Keys.LineFeed, (IntPtr)Keys.A, (IntPtr)2 })
                {
                    if (wParam != (IntPtr)Keys.Enter)
                    {
                        yield return new object[] { (int)User32.WM.CHAR, wParam, '2', handled, handled, 1, 0, 0, (IntPtr)50 };
                        yield return new object[] { (int)User32.WM.CHAR, wParam, '1', handled, handled, 1, 0, 0, (IntPtr)49 };
                    }

                    yield return new object[] { (int)User32.WM.SYSCHAR, wParam, '2', handled, handled, 1, 0, 0, (IntPtr)50 };
                    yield return new object[] { (int)User32.WM.SYSCHAR, wParam, '1', handled, handled, 1, 0, 0, (IntPtr)49 };
                    yield return new object[] { (int)User32.WM.IME_CHAR, wParam, '2', handled, handled, 1, 0, 0, (IntPtr)50 };
                    yield return new object[] { (int)User32.WM.IME_CHAR, wParam, '1', handled, handled, 1, 0, 0, (IntPtr)49 };
                    yield return new object[] { (int)User32.WM.KEYDOWN, wParam, '2', handled, handled, 0, 1, 0, wParam };
                    yield return new object[] { (int)User32.WM.SYSKEYDOWN, wParam, '2', handled, handled, 0, 1, 0, wParam };
                    yield return new object[] { (int)User32.WM.KEYUP, wParam, '2', handled, handled, 0, 0, 1, wParam };
                    yield return new object[] { (int)User32.WM.SYSKEYUP, wParam, '2', handled, handled, 0, 0, 1, wParam };
                    yield return new object[] { 0, wParam, '2', handled, handled, 0, 0, 1, wParam };
                }

                yield return new object[] { (int)User32.WM.CHAR, (IntPtr)Keys.Enter, '2', handled, true, 0, 0, 0, (IntPtr)Keys.Enter };
                yield return new object[] { (int)User32.WM.CHAR, (IntPtr)Keys.Enter, '1', handled, true, 0, 0, 0, (IntPtr)Keys.Enter };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ProcessKeyEventArgs_TestData))]
        public void DataGridViewTextBoxEditingControl_ProcessKeyEventArgs_InvokeWithoutParent_ReturnsFalse(int msg, IntPtr wParam, char newChar, bool handled, bool expected, int expectedKeyPressCallCount, int expectedKeyDownCallCount, int expectedKeyUpCallCount, IntPtr expectedWParam)
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
            int keyPressCallCount = 0;
            control.KeyPress += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal((char)wParam, e.KeyChar);
                e.KeyChar = newChar;
                e.Handled = handled;
                keyPressCallCount++;
            };
            int keyDownCallCount = 0;
            control.KeyDown += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal((int)wParam, e.KeyValue);
                e.Handled = handled;
                keyDownCallCount++;
            };
            int keyUpCallCount = 0;
            control.KeyUp += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal((int)wParam, e.KeyValue);
                e.Handled = handled;
                keyUpCallCount++;
            };
            var m = new Message
            {
                Msg = msg,
                WParam = wParam
            };
            Assert.Equal(expected, control.ProcessKeyEventArgs(ref m));
            Assert.Equal(expectedKeyPressCallCount, keyPressCallCount);
            Assert.Equal(expectedKeyDownCallCount, keyDownCallCount);
            Assert.Equal(expectedKeyUpCallCount, keyUpCallCount);
            Assert.Equal(expectedWParam, m.WParam);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ProcessKeyEventArgs_TestData))]
        public void DataGridViewTextBoxEditingControl_ProcessKeyEventArgs_InvokeWithParent_ReturnsFalse(int msg, IntPtr wParam, char newChar, bool handled, bool expected, int expectedKeyPressCallCount, int expectedKeyDownCallCount, int expectedKeyUpCallCount, IntPtr expectedWParam)
        {
            using var parent = new Control();
            using var control = new SubDataGridViewTextBoxEditingControl
            {
                Parent = parent
            };
            int keyPressCallCount = 0;
            control.KeyPress += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal((char)wParam, e.KeyChar);
                e.KeyChar = newChar;
                e.Handled = handled;
                keyPressCallCount++;
            };
            int keyDownCallCount = 0;
            control.KeyDown += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal((int)wParam, e.KeyValue);
                e.Handled = handled;
                keyDownCallCount++;
            };
            int keyUpCallCount = 0;
            control.KeyUp += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal((int)wParam, e.KeyValue);
                e.Handled = handled;
                keyUpCallCount++;
            };
            var m = new Message
            {
                Msg = msg,
                WParam = wParam
            };
            Assert.Equal(expected, control.ProcessKeyEventArgs(ref m));
            Assert.Equal(expectedKeyPressCallCount, keyPressCallCount);
            Assert.Equal(expectedKeyDownCallCount, keyDownCallCount);
            Assert.Equal(expectedKeyUpCallCount, keyUpCallCount);
            Assert.Equal(expectedWParam, m.WParam);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ProcessKeyEventArgs_TestData))]
        public void DataGridViewTextBoxEditingControl_ProcessKeyEventArgs_InvokeWithCustomParent_ReturnsFalse(int msg, IntPtr wParam, char newChar, bool handled, bool expected, int expectedKeyPressCallCount, int expectedKeyDownCallCount, int expectedKeyUpCallCount, IntPtr expectedWParam)
        {
            int callCount = 0;
            bool action(Message m)
            {
                callCount++;
                return true;
            }
            using var parent = new CustomProcessKeyEventArgsControl
            {
                ProcessKeyEventArgsAction = action
            };
            using var control = new SubDataGridViewTextBoxEditingControl
            {
                Parent = parent
            };
            int keyPressCallCount = 0;
            control.KeyPress += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal((char)wParam, e.KeyChar);
                e.KeyChar = newChar;
                e.Handled = handled;
                keyPressCallCount++;
            };
            int keyDownCallCount = 0;
            control.KeyDown += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal((int)wParam, e.KeyValue);
                e.Handled = handled;
                keyDownCallCount++;
            };
            int keyUpCallCount = 0;
            control.KeyUp += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal((int)wParam, e.KeyValue);
                e.Handled = handled;
                keyUpCallCount++;
            };
            var m = new Message
            {
                Msg = msg,
                WParam = wParam
            };
            Assert.Equal(expected, control.ProcessKeyEventArgs(ref m));
            Assert.Equal(0, callCount);
            Assert.Equal(expectedKeyPressCallCount, keyPressCallCount);
            Assert.Equal(expectedKeyDownCallCount, keyDownCallCount);
            Assert.Equal(expectedKeyUpCallCount, keyUpCallCount);
            Assert.Equal(expectedWParam, m.WParam);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)User32.WM.CHAR)]
        [InlineData((int)User32.WM.SYSCHAR)]
        public void DataGridViewTextBoxEditingControl_ProcessKeyEventArgs_InvokeCharAfterImeChar_Success(int msg)
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
            int keyPressCallCount = 0;
            control.KeyPress += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(0, e.KeyChar);
                e.Handled = true;
                keyPressCallCount++;
            };
            var charM = new Message
            {
                Msg = msg
            };
            var imeM = new Message
            {
                Msg = (int)User32.WM.IME_CHAR
            };

            // Char.
            Assert.True(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(1, keyPressCallCount);
            Assert.False(control.IsHandleCreated);

            // Ime, Char.
            Assert.True(control.ProcessKeyEventArgs(ref imeM));
            Assert.Equal(2, keyPressCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(2, keyPressCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(3, keyPressCallCount);
            Assert.False(control.IsHandleCreated);

            // Ime, Ime, Char.
            Assert.True(control.ProcessKeyEventArgs(ref imeM));
            Assert.Equal(4, keyPressCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.ProcessKeyEventArgs(ref imeM));
            Assert.Equal(5, keyPressCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(5, keyPressCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(5, keyPressCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(6, keyPressCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)User32.WM.KEYDOWN)]
        [InlineData((int)User32.WM.SYSKEYDOWN)]
        [InlineData((int)User32.WM.KEYUP)]
        [InlineData((int)User32.WM.SYSKEYUP)]
        public void DataGridViewTextBoxEditingControl_ProcessKeyEventArgs_InvokeNonCharAfterImeChar_Success(int msg)
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
            int keyPressCallCount = 0;
            control.KeyPress += (sender, e) =>
            {
                e.Handled = true;
                keyPressCallCount++;
            };
            int keyCallCount = 0;
            control.KeyDown += (sender, e) =>
            {
                e.Handled = true;
                keyCallCount++;
            };
            control.KeyUp += (sender, e) =>
            {
                e.Handled = true;
                keyCallCount++;
            };
            var charM = new Message
            {
                Msg = msg
            };
            var imeM = new Message
            {
                Msg = (int)User32.WM.IME_CHAR
            };

            // Non-Char.
            Assert.True(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(0, keyPressCallCount);
            Assert.Equal(1, keyCallCount);
            Assert.False(control.IsHandleCreated);

            // Ime, Non-Char.
            Assert.True(control.ProcessKeyEventArgs(ref imeM));
            Assert.Equal(1, keyPressCallCount);
            Assert.Equal(1, keyCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(1, keyPressCallCount);
            Assert.Equal(2, keyCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(1, keyPressCallCount);
            Assert.Equal(3, keyCallCount);
            Assert.False(control.IsHandleCreated);

            // Ime, Ime, Non-Char.
            Assert.True(control.ProcessKeyEventArgs(ref imeM));
            Assert.Equal(2, keyPressCallCount);
            Assert.Equal(3, keyCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.ProcessKeyEventArgs(ref imeM));
            Assert.Equal(3, keyPressCallCount);
            Assert.Equal(3, keyCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(3, keyPressCallCount);
            Assert.Equal(4, keyCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(3, keyPressCallCount);
            Assert.Equal(5, keyCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(3, keyPressCallCount);
            Assert.Equal(6, keyCallCount);
            Assert.False(control.IsHandleCreated);
        }

        private class CustomProcessKeyEventArgsControl : Control
        {
            public Func<Message, bool> ProcessKeyEventArgsAction { get; set; }

            protected override bool ProcessKeyEventArgs(ref Message m) => ProcessKeyEventArgsAction(m);

            public new bool ProcessKeyMessage(ref Message m) => base.ProcessKeyMessage(ref m);
        }

        [WinFormsTheory]
        [MemberData(nameof(ProcessKeyEventArgs_TestData))]
        public void DataGridViewTextBoxEditingControl_ProcessKeyMessage_InvokeWithoutParent_ReturnsFalse(int msg, IntPtr wParam, char newChar, bool handled, bool expected, int expectedKeyPressCallCount, int expectedKeyDownCallCount, int expectedKeyUpCallCount, IntPtr expectedWParam)
        {
            using var control = new SubDataGridViewTextBoxEditingControl();
            int keyPressCallCount = 0;
            control.KeyPress += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal((char)wParam, e.KeyChar);
                e.KeyChar = newChar;
                e.Handled = handled;
                keyPressCallCount++;
            };
            int keyDownCallCount = 0;
            control.KeyDown += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal((int)wParam, e.KeyValue);
                e.Handled = handled;
                keyDownCallCount++;
            };
            int keyUpCallCount = 0;
            control.KeyUp += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal((int)wParam, e.KeyValue);
                e.Handled = handled;
                keyUpCallCount++;
            };
            var m = new Message
            {
                Msg = msg,
                WParam = wParam
            };
            Assert.Equal(expected, control.ProcessKeyMessage(ref m));
            Assert.Equal(expectedKeyPressCallCount, keyPressCallCount);
            Assert.Equal(expectedKeyDownCallCount, keyDownCallCount);
            Assert.Equal(expectedKeyUpCallCount, keyUpCallCount);
            Assert.Equal(expectedWParam, m.WParam);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ProcessKeyEventArgs_TestData))]
        public void DataGridViewTextBoxEditingControl_ProcessKeyMessage_InvokeWithParent_ReturnsFalse(int msg, IntPtr wParam, char newChar, bool handled, bool expected, int expectedKeyPressCallCount, int expectedKeyDownCallCount, int expectedKeyUpCallCount, IntPtr expectedWParam)
        {
            using var parent = new Control();
            using var control = new SubDataGridViewTextBoxEditingControl
            {
                Parent = parent
            };
            int keyPressCallCount = 0;
            control.KeyPress += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal((char)wParam, e.KeyChar);
                e.KeyChar = newChar;
                e.Handled = handled;
                keyPressCallCount++;
            };
            int keyDownCallCount = 0;
            control.KeyDown += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal((int)wParam, e.KeyValue);
                e.Handled = handled;
                keyDownCallCount++;
            };
            int keyUpCallCount = 0;
            control.KeyUp += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal((int)wParam, e.KeyValue);
                e.Handled = handled;
                keyUpCallCount++;
            };
            var m = new Message
            {
                Msg = msg,
                WParam = wParam
            };
            Assert.Equal(expected, control.ProcessKeyMessage(ref m));
            Assert.Equal(expectedKeyPressCallCount, keyPressCallCount);
            Assert.Equal(expectedKeyDownCallCount, keyDownCallCount);
            Assert.Equal(expectedKeyUpCallCount, keyUpCallCount);
            Assert.Equal(expectedWParam, m.WParam);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ProcessKeyEventArgs_TestData))]
        public void DataGridViewTextBoxEditingControl_ProcessKeyMessage_InvokeWithCustomParent_ReturnsFalse(int msg, IntPtr wParam, char newChar, bool handled, bool expected, int expectedKeyPressCallCount, int expectedKeyDownCallCount, int expectedKeyUpCallCount, IntPtr expectedWParam)
        {
            int callCount = 0;
            bool action(Message m)
            {
                callCount++;
                return true;
            }
            using var parent = new CustomProcessKeyEventArgsControl
            {
                ProcessKeyEventArgsAction = action
            };
            using var control = new SubDataGridViewTextBoxEditingControl
            {
                Parent = parent
            };
            int keyPressCallCount = 0;
            control.KeyPress += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal((char)wParam, e.KeyChar);
                e.KeyChar = newChar;
                e.Handled = handled;
                keyPressCallCount++;
            };
            int keyDownCallCount = 0;
            control.KeyDown += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal((int)wParam, e.KeyValue);
                e.Handled = handled;
                keyDownCallCount++;
            };
            int keyUpCallCount = 0;
            control.KeyUp += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal((int)wParam, e.KeyValue);
                e.Handled = handled;
                keyUpCallCount++;
            };
            var m = new Message
            {
                Msg = msg,
                WParam = wParam
            };
            Assert.Equal(expected, control.ProcessKeyMessage(ref m));
            Assert.Equal(0, callCount);
            Assert.Equal(expectedKeyPressCallCount, keyPressCallCount);
            Assert.Equal(expectedKeyDownCallCount, keyDownCallCount);
            Assert.Equal(expectedKeyUpCallCount, keyUpCallCount);
            Assert.Equal(expectedWParam, m.WParam);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewTextBoxEditingControl_ProcessKeyMessage_InvokeWithCustomParentProcessKeyPreview_ReturnsExpected(bool result)
        {
            int callCount = 0;
            bool action(Message actualM)
            {
                Assert.Equal(1, actualM.Msg);
                callCount++;
                return result;
            }
            using var parent = new CustomProcessControl
            {
                ProcessKeyPreviewAction = action
            };
            using var control = new SubDataGridViewTextBoxEditingControl
            {
                Parent = parent
            };
            var m = new Message
            {
                Msg = 1
            };
            Assert.Equal(result, control.ProcessKeyMessage(ref m));
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewTextBoxEditingControl_ProcessKeyMessage_InvokeWithCustomProcessKeyEventArgs_ReturnsExpected(bool result)
        {
            int callCount = 0;
            bool action(Message actualM)
            {
                Assert.Equal(1, actualM.Msg);
                callCount++;
                return result;
            }
            using var control = new CustomProcessKeyEventArgsControl
            {
                ProcessKeyEventArgsAction = action
            };
            var m = new Message
            {
                Msg = 1
            };
            Assert.Equal(result, control.ProcessKeyMessage(ref m));
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, 0, true)]
        [InlineData(true, false, 0, true)]
        [InlineData(false, true, 1, true)]
        [InlineData(false, false, 1, false)]
        public void DataGridViewTextBoxEditingControl_ProcessKeyMessage_InvokeWithCustomParentProcessKeyPreviewCustomProcessKeyEventArgs_ReturnsExpected(bool parentResult, bool result, int expectedCallCount, bool expectedResult)
        {
            int parentCallCount = 0;
            bool parentAction(Message actualM)
            {
                Assert.Equal(1, actualM.Msg);
                parentCallCount++;
                return parentResult;
            }
            using var parent = new CustomProcessControl
            {
                ProcessKeyPreviewAction = parentAction
            };
            int callCount = 0;
            bool action(Message actualM)
            {
                Assert.Equal(1, actualM.Msg);
                callCount++;
                return result;
            }
            using var control = new CustomProcessKeyEventArgsControl
            {
                Parent = parent,
                ProcessKeyEventArgsAction = action
            };
            var m = new Message
            {
                Msg = 1
            };
            Assert.Equal(expectedResult, control.ProcessKeyMessage(ref m));
            Assert.Equal(1, parentCallCount);
            Assert.Equal(expectedCallCount, callCount);
            Assert.False(control.IsHandleCreated);
        }

        private class CustomProcessControl : Control
        {
            public Func<Message, Keys, bool> ProcessCmdKeyAction { get; set; }

            protected override bool ProcessCmdKey(ref Message msg, Keys keyData) => ProcessCmdKeyAction(msg, keyData);

            public Func<char, bool> ProcessDialogCharAction { get; set; }

            protected override bool ProcessDialogChar(char charCode) => ProcessDialogCharAction(charCode);

            public Func<Keys, bool> ProcessDialogKeyAction { get; set; }

            protected override bool ProcessDialogKey(Keys keyData) => ProcessDialogKeyAction(keyData);

            public Func<Message, bool> ProcessKeyPreviewAction { get; set; }

            protected override bool ProcessKeyPreview(ref Message m) => ProcessKeyPreviewAction(m);
        }

        private class CustomGetEditingControlFormattedValueDataGridViewTextBoxEditingControl : DataGridViewTextBoxEditingControl
        {
            public Func<DataGridViewDataErrorContexts, object> GetEditingControlFormattedValueAction { get; set; }

            public override object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
            {
                return GetEditingControlFormattedValueAction(context);
            }
        }

        private class SubDataGridViewTextBoxEditingControl : DataGridViewTextBoxEditingControl
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

            public new bool IsHandleCreated => base.IsHandleCreated;

            public new bool ResizeRedraw
            {
                get => base.ResizeRedraw;
                set => base.ResizeRedraw = value;
            }

            public new bool ShowFocusCues => base.ShowFocusCues;

            public new bool ShowKeyboardCues => base.ShowKeyboardCues;

            public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new void CreateControl() => base.CreateControl();

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public new void OnGotFocus(EventArgs e) => base.OnGotFocus(e);

            public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

            public new void OnMouseWheel(MouseEventArgs e) => base.OnMouseWheel(e);

            public new void OnTextChanged(EventArgs e) => base.OnTextChanged(e);

            public new bool ProcessKeyEventArgs(ref Message m) => base.ProcessKeyEventArgs(ref m);

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
        }
    }
}

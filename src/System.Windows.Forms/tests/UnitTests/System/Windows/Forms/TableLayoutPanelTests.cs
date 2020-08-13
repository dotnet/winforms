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

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class TableLayoutPanelTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void TableLayoutPanel_Ctor_Default()
        {
            using var control = new SubTableLayoutPanel();
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
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
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.False(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(TableLayoutPanelCellBorderStyle.None, control.CellBorderStyle);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.ClientRectangle);
            Assert.Equal(new Size(200, 100), control.ClientSize);
            Assert.Equal(0, control.ColumnCount);
            Assert.Empty(control.ColumnStyles);
            Assert.Same(control.LayoutSettings.ColumnStyles, control.ColumnStyles);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Equal(Cursors.Default, control.Cursor);
            Assert.Equal(Cursors.Default, control.DefaultCursor);
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
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(TableLayoutPanelGrowStyle.AddRows, control.GrowStyle);
            Assert.False(control.HasChildren);
            Assert.Equal(100, control.Height);
            Assert.NotNull(control.HorizontalScroll);
            Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
            Assert.False(control.HScroll);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.NotNull(control.LayoutSettings);
            Assert.Same(control.LayoutSettings, control.LayoutSettings);
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
            Assert.Equal(0, control.RowCount);
            Assert.Empty(control.RowStyles);
            Assert.Same(control.LayoutSettings.RowStyles, control.RowStyles);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Equal(new Size(200, 100), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.False(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.False(control.VScroll);
            Assert.Equal(200, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TableLayoutPanel_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubTableLayoutPanel();
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(BorderStyle))]
        public void TableLayoutPanel_BorderStyle_Set_GetReturnsExpected(BorderStyle value)
        {
            using var control = new TableLayoutPanel()
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
        public void TableLayoutPanel_BorderStyle_SetWithHandle_GetReturnsExpected(BorderStyle value, int expectedInvalidatedCallCount)
        {
            using var control = new TableLayoutPanel();
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
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.BorderStyle = value;
            Assert.Equal(value, control.BorderStyle);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(BorderStyle))]
        public void TableLayoutPanel_BorderStyle_SetInvalid_ThrowsInvalidEnumArgumentException(BorderStyle value)
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.BorderStyle = value);
        }

        public static IEnumerable<object[]> CellBorderStyle_Set_TestData()
        {
            foreach (bool resizeRedraw in new bool[] { true, false })
            {
                yield return new object[] { resizeRedraw, TableLayoutPanelCellBorderStyle.Inset, true, 1 };
                yield return new object[] { resizeRedraw, TableLayoutPanelCellBorderStyle.InsetDouble, true, 1 };
                yield return new object[] { resizeRedraw, TableLayoutPanelCellBorderStyle.None, resizeRedraw, 1 };
                yield return new object[] { resizeRedraw, TableLayoutPanelCellBorderStyle.Outset, true, 1 };
                yield return new object[] { resizeRedraw, TableLayoutPanelCellBorderStyle.OutsetDouble, true, 1 };
                yield return new object[] { resizeRedraw, TableLayoutPanelCellBorderStyle.OutsetPartial, true, 1 };
                yield return new object[] { resizeRedraw, TableLayoutPanelCellBorderStyle.Single, true, 1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(CellBorderStyle_Set_TestData))]
        public void TableLayoutPanel_CellBorderStyle_Set_GetReturnsExpected(bool resizeRedraw, TableLayoutPanelCellBorderStyle value, bool expectedResizeRedraw, int expectedLayoutCallCount)
        {
            using var control = new SubTableLayoutPanel();
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("CellBorderStyle", e.AffectedProperty);
                layoutCallCount++;
            };

            control.CellBorderStyle = value;
            Assert.Equal(value, control.CellBorderStyle);
            Assert.Equal(expectedResizeRedraw, control.GetStyle(ControlStyles.ResizeRedraw));
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.CellBorderStyle = value;
            Assert.Equal(value, control.CellBorderStyle);
            Assert.Equal(expectedResizeRedraw, control.GetStyle(ControlStyles.ResizeRedraw));
            Assert.Equal(expectedLayoutCallCount * 2, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(CellBorderStyle_Set_TestData))]
        public void TableLayoutPanel_CellBorderStyle_SetWithHandle_GetReturnsExpected(bool resizeRedraw, TableLayoutPanelCellBorderStyle value, bool expectedResizeRedraw, int expectedLayoutCallCount)
        {
            using var control = new SubTableLayoutPanel();
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
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
                Assert.Equal("CellBorderStyle", e.AffectedProperty);
                layoutCallCount++;
            };

            control.CellBorderStyle = value;
            Assert.Equal(value, control.CellBorderStyle);
            Assert.Equal(expectedResizeRedraw, control.GetStyle(ControlStyles.ResizeRedraw));
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.CellBorderStyle = value;
            Assert.Equal(value, control.CellBorderStyle);
            Assert.Equal(expectedResizeRedraw, control.GetStyle(ControlStyles.ResizeRedraw));
            Assert.Equal(expectedLayoutCallCount * 2, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount * 4, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TableLayoutPanelCellBorderStyle))]
        public void TableLayoutPanel_CellBorderStyle_SetInvalid_ThrowsArgumentOutOfRangeException(TableLayoutPanelCellBorderStyle value)
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.CellBorderStyle = value);
        }

        [WinFormsTheory]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        public void TableLayoutPanel_ColumnCount_Set_GetReturnsExpected(int value, int expectedLayoutCallCount)
        {
            using var control = new TableLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Columns", e.AffectedProperty);
                layoutCallCount++;
            };

            control.ColumnCount = value;
            Assert.Equal(value, control.ColumnCount);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ColumnCount = value;
            Assert.Equal(value, control.ColumnCount);
            Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TableLayoutPanel_ColumnCount_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.ColumnCount = -1);
        }

        [WinFormsFact]
        public void TableLayoutPanel_Controls_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TableLayoutPanel))[nameof(Control.Controls)];
            using var control = new TableLayoutPanel();
            Assert.False(property.CanResetValue(control));

            Assert.Empty(control.Controls);
            Assert.False(property.CanResetValue(control));

            using var child = new Control();
            control.Controls.Add(child);
            Assert.Same(child, Assert.Single(control.Controls));
            Assert.False(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Same(child, Assert.Single(control.Controls));
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void TableLayoutPanel_Controls_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TableLayoutPanel))[nameof(Control.Controls)];
            using var control = new TableLayoutPanel();
            Assert.False(property.ShouldSerializeValue(control));

            Assert.Empty(control.Controls);
            Assert.False(property.ShouldSerializeValue(control));

            using var child = new Control();
            control.Controls.Add(child);
            Assert.Same(child, Assert.Single(control.Controls));
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Same(child, Assert.Single(control.Controls));
            Assert.True(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize, 1)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows, 0)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns, 1)]
        public void TableLayoutPanel_GrowStyle_Set_GetReturnsExpected(TableLayoutPanelGrowStyle value, int expectedLayoutCallCount)
        {
            using var control = new TableLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("GrowStyle", e.AffectedProperty);
                layoutCallCount++;
            };

            control.GrowStyle = value;
            Assert.Equal(value, control.GrowStyle);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.GrowStyle = value;
            Assert.Equal(value, control.GrowStyle);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TableLayoutPanelGrowStyle))]
        public void TableLayoutPanel_GrowStyle_SetInvalid_ThrowsArgumentOutOfRangeException(TableLayoutPanelGrowStyle value)
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.GrowStyle = value);
        }

        [WinFormsFact]
        public void TableLayoutPanel_LayoutSettings_SetEmptyStub_Success()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));

            using var control = new TableLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("LayoutSettings", e.AffectedProperty);
                layoutCallCount++;
            };

            control.LayoutSettings = settings;
            Assert.NotSame(settings, control.LayoutSettings);
            Assert.Empty(control.LayoutSettings.ColumnStyles);
            Assert.Empty(control.LayoutSettings.RowStyles);
            Assert.Equal(1, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.LayoutSettings = settings;
            Assert.NotSame(settings, control.LayoutSettings);
            Assert.Empty(control.LayoutSettings.ColumnStyles);
            Assert.Empty(control.LayoutSettings.RowStyles);
            Assert.Equal(2, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TableLayoutPanel_LayoutSettings_SetStubWithoutControls_Success()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            var columnStyle = new ColumnStyle(SizeType.Percent, 1);
            var rowStyle = new RowStyle(SizeType.Percent, 1);
            settings.ColumnStyles.Add(columnStyle);
            settings.RowStyles.Add(rowStyle);

            using var control = new TableLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("LayoutSettings", e.AffectedProperty);
                layoutCallCount++;
            };

            control.LayoutSettings = settings;
            Assert.NotSame(settings, control.LayoutSettings);
            Assert.Equal(columnStyle, Assert.Single(control.LayoutSettings.ColumnStyles));
            Assert.Equal(rowStyle, Assert.Single(control.LayoutSettings.RowStyles));
            Assert.Equal(1, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.LayoutSettings = settings;
            Assert.NotSame(settings, control.LayoutSettings);
            Assert.Empty(control.LayoutSettings.ColumnStyles);
            Assert.Empty(control.LayoutSettings.RowStyles);
            Assert.Equal(2, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TableLayoutPanel_LayoutSettings_SetStubWithControls_Success()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            var columnStyle = new ColumnStyle(SizeType.Percent, 1);
            var rowStyle = new RowStyle(SizeType.Percent, 1);

            settings.SetColumnSpan("name", 1);
            settings.SetRowSpan("name", 2);
            settings.SetColumn("name", 3);
            settings.SetRow("name", 4);
            settings.SetColumnSpan("noSuchName", 5);
            settings.SetRowSpan("noSuchName", 6);
            settings.SetColumn("noSuchName", 7);
            settings.SetRow("noSuchName", 8);
            settings.ColumnStyles.Add(columnStyle);
            settings.RowStyles.Add(rowStyle);

            using var control = new TableLayoutPanel();
            using var controlWithName = new Control { Name = "name" };
            using var controlWithDefaultName = new Control();
            using var controlWithoutName = new ControlWithNullName();
            control.Controls.Add(controlWithName);
            control.Controls.Add(controlWithDefaultName);
            control.Controls.Add(controlWithoutName);
            int layoutCallCount = 0;
            void layoutHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("LayoutSettings", e.AffectedProperty);
                layoutCallCount++;
            };
            control.Layout += layoutHandler;

            try
            {
                control.LayoutSettings = settings;
                Assert.NotSame(settings, control.LayoutSettings);
                Assert.Equal(columnStyle, Assert.Single(control.LayoutSettings.ColumnStyles));
                Assert.Equal(rowStyle, Assert.Single(control.LayoutSettings.RowStyles));
                Assert.Equal(1, control.LayoutSettings.GetColumnSpan(controlWithName));
                Assert.Equal(2, control.LayoutSettings.GetRowSpan(controlWithName));
                Assert.Equal(3, control.LayoutSettings.GetColumn(controlWithName));
                Assert.Equal(4, control.LayoutSettings.GetRow(controlWithName));
                Assert.Equal(1, layoutCallCount);
                Assert.False(control.IsHandleCreated);

                // Set same.
                control.LayoutSettings = settings;
                Assert.NotSame(settings, control.LayoutSettings);
                Assert.Empty(control.LayoutSettings.ColumnStyles);
                Assert.Empty(control.LayoutSettings.RowStyles);
                Assert.Equal(2, layoutCallCount);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                control.Layout -= layoutHandler;
            }
        }

        [WinFormsFact]
        public void TableLayoutPanel_LayoutSettings_SetStub_ResetsOriginalSettings()
        {
            var converter = new TableLayoutSettingsTypeConverter();
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(converter.ConvertFrom(@"<?xml version=""1.0"" encoding=""utf-16""?><Root />"));
            settings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1));
            settings.RowStyles.Add(new RowStyle(SizeType.Percent, 1));

            using var control = new TableLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("LayoutSettings", e.AffectedProperty);
                layoutCallCount++;
            };

            control.LayoutSettings = settings;
            Assert.NotSame(settings, control.LayoutSettings);
            Assert.Empty(settings.ColumnStyles);
            Assert.Empty(settings.ColumnStyles);
            Assert.Equal(1, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.LayoutSettings = settings;
            Assert.NotSame(settings, control.LayoutSettings);
            Assert.Empty(settings.ColumnStyles);
            Assert.Empty(settings.ColumnStyles);
            Assert.Equal(2, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TableLayoutPanel_LayoutSettings_SetNull_ThrowsNNotSupportedException()
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<NotSupportedException>(() => control.LayoutSettings = null);
        }

        [WinFormsFact]
        public void TableLayoutPanel_LayoutSettings_SetNonStub_ThrowsNNotSupportedException()
        {
            var otherControl = new TableLayoutPanel();

            using var control = new TableLayoutPanel();
            Assert.Throws<NotSupportedException>(() => control.LayoutSettings = otherControl.LayoutSettings);
        }

        [WinFormsTheory]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        public void TableLayoutPanel_RowCount_Set_GetReturnsExpected(int value, int expectedLayoutCallCount)
        {
            using var control = new TableLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Rows", e.AffectedProperty);
                layoutCallCount++;
            };

            control.RowCount = value;
            Assert.Equal(value, control.RowCount);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.RowCount = value;
            Assert.Equal(value, control.RowCount);
            Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TableLayoutPanel_RowCount_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.RowCount = -1);
        }

        [WinFormsFact]
        public void TableLayoutPanel_CreateControlsInstance_Invoke_ReturnsExpected()
        {
            using var control = new SubTableLayoutPanel();
            TableLayoutControlCollection controls = Assert.IsType<TableLayoutControlCollection>(control.CreateControlsInstance());
            Assert.Empty(controls);
            Assert.Same(control, controls.Owner);
            Assert.False(controls.IsReadOnly);
            Assert.NotSame(controls, control.CreateControlsInstance());
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubTableLayoutPanel();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetCellPosition_NoSuchControl_ReturnsExpected()
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            Assert.Equal(new TableLayoutPanelCellPosition(-1, -1), control.GetCellPosition(child));
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetCellPosition_NullControl_ThrowsArgumentNullException()
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => control.GetCellPosition(null));
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetCellPosition_InvalidColumn_ThrowsArgumentOutOfRangeException()
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            control.SetCellPosition(child, new TableLayoutPanelCellPosition { Column = -2 });
            Assert.Throws<ArgumentOutOfRangeException>("column", () => control.GetCellPosition(child));
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetCellPosition_InvalidRow_ThrowsArgumentOutOfRangeException()
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            control.SetCellPosition(child, new TableLayoutPanelCellPosition { Row = -2 });
            Assert.Throws<ArgumentOutOfRangeException>("row", () => control.GetCellPosition(child));
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetColumn_NoSuchControl_ReturnsExpected()
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            Assert.Equal(-1, control.GetColumn(child));
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetColumn_NullControl_ThrowsArgumentNullException()
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => control.GetColumn(null));
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetColumnSpan_NoSuchControl_ReturnsExpected()
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            Assert.Equal(1, control.GetColumnSpan(child));
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetColumnSpan_NullControl_ThrowsArgumentNullException()
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => control.GetColumnSpan(null));
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetColumnWidths_NoColumns_ReturnsExpectd()
        {
            var panel = new TableLayoutPanel();
            Assert.Empty(panel.GetColumnWidths());
        }

        [WinFormsTheory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetColumnWidths_InvokeNoChildren_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            using var control = new Button();
            var panel = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle
            };
            Assert.Equal(new int[] { 0, 0, 200 }, panel.GetColumnWidths());
        }

        [WinFormsTheory]
        [InlineData(1, 1, 10, 11)]
        [InlineData(2, 2, 11, 12)]
        [InlineData(4, 4, 10, 11)]
        public void TableLayoutPanel_GetControlFromPosition_ControlExistsAddedToContainer_ReturnsExpected(int columnSpan, int rowSpan, int column, int row)
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            control.Controls.Add(child);
            control.SetColumnSpan(child, columnSpan);
            control.SetRowSpan(child, rowSpan);
            control.SetColumn(child, 10);
            control.SetRow(child, 11);

            Assert.Equal(child, control.GetControlFromPosition(column, row));

            // Call again to test caching.
            Assert.Equal(child, control.GetControlFromPosition(column, row));

            // Invalidate the column cache.
            control.SetColumn(child, 20);
            Assert.Null(control.GetControlFromPosition(column, row));

            // Revalidate the column cache.
            control.SetColumn(child, 10);
            Assert.Same(child, control.GetControlFromPosition(column, row));

            // Invalidate the row cache.
            control.SetRow(child, 20);
            Assert.Null(control.GetControlFromPosition(column, row));

            // Revalidate the row cache.
            control.SetRow(child, 11);
            Assert.Same(child, control.GetControlFromPosition(column, row));
        }

        [WinFormsTheory]
        [InlineData(1, 1, 9, 11)]
        [InlineData(1, 1, 10, 10)]
        [InlineData(1, 1, 11, 11)]
        [InlineData(1, 1, 10, 12)]
        [InlineData(2, 2, 8, 9)]
        [InlineData(2, 2, 9, 10)]
        [InlineData(4, 4, 9, 10)]
        public void TableLayoutPanel_GetControlFromPosition_OutOfRange_ReturnsNull(int columnSpan, int rowSpan, int column, int row)
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            control.Controls.Add(child);
            control.SetColumnSpan(child, columnSpan);
            control.SetRowSpan(child, rowSpan);
            control.SetColumn(child, 10);
            control.SetRow(child, 11);

            Assert.Null(control.GetControlFromPosition(column, row));

            // Call again to test caching.
            Assert.Null(control.GetControlFromPosition(column, row));
        }

        [WinFormsTheory]
        [InlineData(1, 1, 10, 11)]
        [InlineData(2, 2, 11, 12)]
        [InlineData(4, 4, 10, 11)]
        public void TableLayoutPanel_GetControlFromPosition_ControlExistsNotVisible_ReturnsNull(int columnSpan, int rowSpan, int column, int row)
        {
            using var child = new Control
            {
                Visible = false
            };
            using var control = new TableLayoutPanel();
            control.Controls.Add(child);
            control.SetColumnSpan(child, columnSpan);
            control.SetRowSpan(child, rowSpan);
            control.SetColumn(child, 10);
            control.SetRow(child, 11);

            Assert.Null(control.GetControlFromPosition(column, row));

            // Call again to test caching.
            Assert.Null(control.GetControlFromPosition(column, row));
        }

        [WinFormsTheory]
        [InlineData(1, 1, 10, 11)]
        [InlineData(2, 2, 11, 12)]
        [InlineData(4, 4, 10, 11)]
        public void TableLayoutPanel_GetControlFromPosition_ControlExistsButNotAddedToContainer_ReturnsNull(int columnSpan, int rowSpan, int column, int row)
        {
            using var child = new Control
            {
                Visible = false
            };
            using var control = new TableLayoutPanel();
            control.SetColumnSpan(child, columnSpan);
            control.SetRowSpan(child, rowSpan);
            control.SetColumn(child, 10);
            control.SetRow(child, 11);

            Assert.Null(control.GetControlFromPosition(column, row));

            // Call again to test caching.
            Assert.Null(control.GetControlFromPosition(column, row));
        }

        [WinFormsTheory]
        [InlineData(1, 1, 10, 11)]
        [InlineData(2, 2, 11, 12)]
        [InlineData(4, 4, 10, 11)]
        public void TableLayoutPanel_GetControlFromPosition_ControlExistsButNotAddedToContainerWithChildren_ReturnsNull(int columnSpan, int rowSpan, int column, int row)
        {
            using var otherChild = new Control();
            using var child = new Control
            {
                Visible = false
            };
            using var control = new TableLayoutPanel();
            control.Controls.Add(otherChild);
            control.SetColumnSpan(child, columnSpan);
            control.SetRowSpan(child, rowSpan);
            control.SetColumn(child, 10);
            control.SetRow(child, 11);

            Assert.Null(control.GetControlFromPosition(column, row));

            // Call again to test caching.
            Assert.Null(control.GetControlFromPosition(column, row));
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetControlFromPosition_NoSuchControl_ReturnsExpected()
        {
            using var control = new TableLayoutPanel();
            Assert.Null(control.GetControlFromPosition(0, 0));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        public void TableLayoutPanel_GetControlFromPosition_NegativeColumn_ThrowsArgumentOutOfRangeException(int column)
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("column", () => control.GetControlFromPosition(column, 0));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        public void TableLayoutPanel_GetControlFromPosition_NegativeRow_ThrowsArgumentOutOfRangeException(int row)
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("row", () => control.GetControlFromPosition(0, row));
        }

        [WinFormsTheory]
        [InlineData(1, 1)]
        [InlineData(2, 3)]
        public void TableLayoutPanel_GetPositionFromControl_ControlExists_ReturnsExpected(int columnSpan, int rowSpan)
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            control.Controls.Add(child);
            control.SetColumn(child, 1);
            control.SetRow(child, 2);
            control.SetColumnSpan(child, columnSpan);
            control.SetRowSpan(child, rowSpan);

            Assert.Equal(new TableLayoutPanelCellPosition(1, 2), control.GetPositionFromControl(child));
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetPositionFromControl_NotVisibleSuchControl_ReturnsExpected()
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            control.Controls.Add(child);
            control.SetColumn(child, 1);
            control.SetRow(child, 2);

            Assert.Equal(new TableLayoutPanelCellPosition(1, 2), control.GetPositionFromControl(child));
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetPositionFromControl_NoSuchControl_ReturnsExpected()
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            control.SetColumn(child, 1);
            control.SetRow(child, 2);

            Assert.Equal(new TableLayoutPanelCellPosition(-1, -1), control.GetPositionFromControl(child));
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetPositionFromControl_NullControl_ReturnsExpected()
        {
            using var control = new TableLayoutPanel();
            Assert.Equal(new TableLayoutPanelCellPosition(-1, -1), control.GetPositionFromControl(null));
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetRow_NoSuchControl_ReturnsExpected()
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            Assert.Equal(-1, control.GetRow(child));
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetRow_NullControl_ThrowsArgumentNullException()
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => control.GetRow(null));
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetRowHeights_NoRows_ReturnsExpectd()
        {
            using var control = new TableLayoutPanel();
            Assert.Empty(control.GetRowHeights());
        }

        [WinFormsTheory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetRowHeights_InvokeNoChildren_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            using var control = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle
            };
            Assert.Equal(new int[] { 0, 100 }, control.GetRowHeights());
        }

        [WinFormsTheory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetRowHeights_CustomAutoSizeStyles_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            using var control = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle,
                ClientSize = new Size(70, 80)
            };
            control.RowStyles.Add(new RowStyle(SizeType.AutoSize, 20));
            Assert.Equal(new int[] { 0, 80 }, control.GetRowHeights());

            control.RowStyles.Add(new RowStyle(SizeType.AutoSize, 30));
            Assert.Equal(new int[] { 0, 80 }, control.GetRowHeights());

            control.RowStyles.Add(new RowStyle(SizeType.AutoSize, 6));
            Assert.Equal(new int[] { 0, 80 }, control.GetRowHeights());
        }

        [WinFormsTheory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetRowHeights_CustomAbsoluteStyles_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            using var control = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle
            };
            control.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));
            Assert.Equal(new int[] { 10, 90 }, control.GetRowHeights());

            control.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));
            Assert.Equal(new int[] { 10, 90 }, control.GetRowHeights());

            control.RowStyles.Add(new RowStyle(SizeType.Absolute, 6));
            Assert.Equal(new int[] { 10, 90 }, control.GetRowHeights());
        }

        [WinFormsTheory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetRowHeights_CustomPercentStyles_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            using var control = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle,
                ClientSize = new Size(70, 80)
            };
            control.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            Assert.Equal(new int[] { 80, 0 }, control.GetRowHeights());

            control.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            Assert.Equal(new int[] { 32, 48 }, control.GetRowHeights());

            control.RowStyles.Add(new RowStyle(SizeType.Percent, 6));
            Assert.Equal(new int[] { 32, 48 }, control.GetRowHeights());
        }

        [WinFormsTheory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetRowHeights_CustomStylesPercentOverflow_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            using var control = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle,
                ClientSize = new Size(70, 80)
            };
            control.RowStyles.Add(new RowStyle(SizeType.Percent, 75));
            Assert.Equal(new int[] { 80, 0 }, control.GetRowHeights());

            control.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            Assert.Equal(new int[] { -20, 100 }, control.GetRowHeights());
        }

        [WinFormsTheory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetRowHeights_CustomStylesPercentAutoSize_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            using var control = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle,
                ClientSize = new Size(70, 80)
            };
            control.RowStyles.Add(new RowStyle(SizeType.Percent, 75));
            Assert.Equal(new int[] { 80, 0 }, control.GetRowHeights());

            control.RowStyles.Add(new RowStyle(SizeType.AutoSize, 100));
            Assert.Equal(new int[] { 80, 0 }, control.GetRowHeights());
        }

        [WinFormsTheory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetRowHeights_CustomStylesAbsoluteAutoSize_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            using var control = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle,
                ClientSize = new Size(70, 80)
            };
            control.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            Assert.Equal(new int[] { 50, 30 }, control.GetRowHeights());

            control.RowStyles.Add(new RowStyle(SizeType.AutoSize, 100));
            Assert.Equal(new int[] { 50, 30 }, control.GetRowHeights());
        }

        [WinFormsTheory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetRowHeights_CustomStylesAbsoluteOverflow_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            using var control = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle,
                ClientSize = new Size(70, 80)
            };
            control.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            Assert.Equal(new int[] { 100, 0 }, control.GetRowHeights());

            control.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            Assert.Equal(new int[] { 100, 100 }, control.GetRowHeights());
        }

        [WinFormsTheory]
        [InlineData(TableLayoutPanelGrowStyle.FixedSize)]
        [InlineData(TableLayoutPanelGrowStyle.AddRows)]
        [InlineData(TableLayoutPanelGrowStyle.AddColumns)]
        public void TableLayoutPanel_GetRowHeights_CustomInvalidStyles_ReturnsExpected(TableLayoutPanelGrowStyle growStyle)
        {
            using var control = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = growStyle,
                ClientSize = new Size(50, 50)
            };
            control.RowStyles.Add(new RowStyle((SizeType)(SizeType.AutoSize - 1), 22));
            Assert.Equal(new int[] { 0, 50 }, control.GetRowHeights());

            control.RowStyles.Add(new RowStyle((SizeType)(SizeType.AutoSize - 1), 22));
            Assert.Equal(new int[] { 0, 50 }, control.GetRowHeights());

            control.RowStyles.Add(new RowStyle((SizeType)(SizeType.AutoSize - 1), 6));
            Assert.Equal(new int[] { 0, 50 }, control.GetRowHeights());
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetRowSpan_NoSuchControl_ReturnsExpected()
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            Assert.Equal(1, control.GetRowSpan(child));
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetRowSpan_NullControl_ThrowsArgumentNullException()
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => control.GetRowSpan(null));
        }

        [WinFormsTheory]
        [InlineData(0, true)]
        [InlineData(SubTableLayoutPanel.ScrollStateAutoScrolling, false)]
        [InlineData(SubTableLayoutPanel.ScrollStateFullDrag, false)]
        [InlineData(SubTableLayoutPanel.ScrollStateHScrollVisible, false)]
        [InlineData(SubTableLayoutPanel.ScrollStateUserHasScrolled, false)]
        [InlineData(SubTableLayoutPanel.ScrollStateVScrollVisible, false)]
        [InlineData(int.MaxValue, false)]
        [InlineData((-1), false)]
        public void TableLayoutPanel_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
        {
            using var control = new SubTableLayoutPanel();
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
        public void TableLayoutPanel_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubTableLayoutPanel();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void TableLayoutPanel_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubTableLayoutPanel();
            Assert.False(control.GetTopLevel());
        }

        public static IEnumerable<object[]> OnCellPaint_TestData()
        {
            yield return new object[] { null };

            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);
            yield return new object[] { new TableLayoutCellPaintEventArgs(graphics, Rectangle.Empty, Rectangle.Empty, 0, 0) };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnCellPaint_TestData))]
        public void TableLayoutPanel_OnCellPaint_Invoke_CallsCellPaint(TableLayoutCellPaintEventArgs eventArgs)
        {
            using var control = new SubTableLayoutPanel();
            int callCount = 0;
            TableLayoutCellPaintEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.CellPaint += handler;
            control.OnCellPaint(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.CellPaint -= handler;
            control.OnCellPaint(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TableLayoutPanel_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubTableLayoutPanel();
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
        public void TableLayoutPanel_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubTableLayoutPanel();
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
        public void TableLayoutPanel_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubTableLayoutPanel();
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

        public static IEnumerable<object[]> OnLayout_TestData()
        {
            yield return new object[] { new LayoutEventArgs(null, null) };
            yield return new object[] { new LayoutEventArgs(new Control(), null) };
            yield return new object[] { new LayoutEventArgs(new Control(), string.Empty) };
            yield return new object[] { new LayoutEventArgs(new Control(), "ChildIndex") };
            yield return new object[] { new LayoutEventArgs(new Control(), "Visible") };
            yield return new object[] { new LayoutEventArgs(new Control(), "Items") };
            yield return new object[] { new LayoutEventArgs(new Control(), "Rows") };
            yield return new object[] { new LayoutEventArgs(new Control(), "Columns") };
            yield return new object[] { new LayoutEventArgs(new Control(), "RowStyles") };
            yield return new object[] { new LayoutEventArgs(new Control(), "ColumnStyles") };
            yield return new object[] { new LayoutEventArgs(new Control(), "TableIndex") };
            yield return new object[] { new LayoutEventArgs(new Control(), "GrowStyle") };
            yield return new object[] { new LayoutEventArgs(new Control(), "CellBorderStyle") };
            yield return new object[] { new LayoutEventArgs(new Control(), "LayoutSettings") };
            yield return new object[] { new LayoutEventArgs(new Control(), "NoSuchProperty") };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnLayout_TestData))]
        public void TableLayoutPanel_OnLayout_Invoke_CallsLayout(LayoutEventArgs eventArgs)
        {
            using var control = new SubTableLayoutPanel();
            int callCount = 0;
            LayoutEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Layout += handler;
            control.OnLayout(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.Layout -= handler;
            control.OnLayout(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnLayout_TestData))]
        public void TableLayoutPanel_OnLayout_InvokeWithHandle_CallsLayout(LayoutEventArgs eventArgs)
        {
            using var control = new SubTableLayoutPanel();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int callCount = 0;
            LayoutEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Layout += handler;
            control.OnLayout(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.Layout -= handler;
            control.OnLayout(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void TableLayoutPanel_OnLayout_InvokeNullE_ThrowsNullReferenceException()
        {
            using var control = new SubTableLayoutPanel();
            Assert.Throws<NullReferenceException>(() => control.OnLayout(null));
        }

        public static IEnumerable<object[]> OnPaintBackground_TestData()
        {
            foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
            {
                foreach (TableLayoutPanelCellBorderStyle cellBorderStyle in Enum.GetValues(typeof(TableLayoutPanelCellBorderStyle)))
                {
                    foreach (bool hScroll in new bool[] { true, false })
                    {
                        foreach (bool vScroll in new bool[] { true, false })
                        {
                            yield return new object[] { rightToLeft, cellBorderStyle, hScroll, vScroll };
                        }
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnPaintBackground_TestData))]
        public void TableLayoutPanel_OnPaintBackground_InvokeEmpty_Success(RightToLeft rightToLeft, TableLayoutPanelCellBorderStyle cellBorderStyle, bool hScroll, bool vScroll)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, new Rectangle(1, 2, 3, 4));

            using var control = new SubTableLayoutPanel
            {
                RightToLeft = rightToLeft,
                CellBorderStyle = cellBorderStyle,
                HScroll = hScroll,
                VScroll = vScroll,
            };
            int cellPaintCallCount = 0;
            control.CellPaint += (sender, e) => cellPaintCallCount++;
            int callCount = 0;
            PaintEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Paint += handler;
            control.OnPaintBackground(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(0, cellPaintCallCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.Paint -= handler;
            control.OnPaintBackground(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(0, cellPaintCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnPaintBackground_TestData))]
        public void TableLayoutPanel_OnPaintBackground_InvokeNotEmpty_Success(RightToLeft rightToLeft, TableLayoutPanelCellBorderStyle cellBorderStyle, bool hScroll, bool vScroll)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, new Rectangle(15, 2, 5, 20));

            using var control = new SubTableLayoutPanel
            {
                RightToLeft = rightToLeft,
                CellBorderStyle = cellBorderStyle,
                HScroll = hScroll,
                VScroll = vScroll,
            };
            int cellPaintCallCount = 0;
            control.CellPaint += (sender, e) =>
            {
                Assert.Same(eventArgs.Graphics, e.Graphics);
                Assert.Equal(eventArgs.ClipRectangle, e.ClipRectangle);
                Assert.Equal(1, e.Column);
                if ((cellPaintCallCount % 2) == 0)
                {
                    Assert.Equal(0, e.Row);
                }
                else
                {
                    Assert.Equal(1, e.Row);
                }
                cellPaintCallCount++;
            };
            int callCount = 0;
            PaintEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            using var child1 = new Control();
            control.Controls.Add(child1);
            control.SetRow(child1, 0);
            control.SetColumn(child1, 1);

            using var child2 = new Control();
            control.Controls.Add(child2);
            control.SetRow(child2, 1);
            control.SetColumn(child2, 1);

            control.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));
            control.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));
            control.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));
            control.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));

            // Call with handler.
            control.Paint += handler;
            control.OnPaintBackground(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(2, cellPaintCallCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.Paint -= handler;
            control.OnPaintBackground(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(4, cellPaintCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TableLayoutPanel_OnPaintBackground_NullEventArgs_ThrowsArgumentNullException()
        {
            using var control = new SubTableLayoutPanel();
            Assert.Throws<ArgumentNullException>(() => control.OnPaintBackground(null));
        }

        public static IEnumerable<object[]> SetCellPosition_TestData()
        {
            yield return new object[] { new TableLayoutPanelCellPosition(-1, -1) };
            yield return new object[] { new TableLayoutPanelCellPosition(0, -1) };
            yield return new object[] { new TableLayoutPanelCellPosition(-1, 0) };
            yield return new object[] { new TableLayoutPanelCellPosition(0, 0) };
            yield return new object[] { new TableLayoutPanelCellPosition(1, 2) };
        }

        [WinFormsTheory]
        [MemberData(nameof(SetCellPosition_TestData))]
        public void TableLayoutPanel_SetCellPosition_Invoke_GetReturnsExpected(TableLayoutPanelCellPosition value)
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int childLayoutCallCount = 0;
            child.Layout += (sender, e) => childLayoutCallCount++;

            control.SetCellPosition(child, value);
            Assert.Equal(value, control.GetCellPosition(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);

            // Set same.
            control.SetCellPosition(child, value);
            Assert.Equal(value, control.GetCellPosition(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);
        }

        public static IEnumerable<object[]> SetCellPosition_ControlWithParent_TestData()
        {
            yield return new object[] { new TableLayoutPanelCellPosition(-1, -1), 1 };
            yield return new object[] { new TableLayoutPanelCellPosition(0, -1), 1 };
            yield return new object[] { new TableLayoutPanelCellPosition(-1, 0), 1 };
            yield return new object[] { new TableLayoutPanelCellPosition(0, 0), 1 };
            yield return new object[] { new TableLayoutPanelCellPosition(1, 2), 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(SetCellPosition_ControlWithParent_TestData))]
        public void TableLayoutPanel_SetCellPosition_InvokeControlWithParent_GetReturnsExpected(TableLayoutPanelCellPosition value, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var child = new Control
            {
                Parent = parent
            };
            using var control = new TableLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int childLayoutCallCount = 0;
            child.Layout += (sender, e) => childLayoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs eventArgs)
            {
                Assert.Same(parent, sender);
                Assert.Same(child, eventArgs.AffectedControl);
                Assert.Equal("TableIndex", eventArgs.AffectedProperty);
                parentLayoutCallCount++;
            }
            parent.Layout += parentHandler;

            try
            {
                control.SetCellPosition(child, value);
                Assert.Equal(value, control.GetCellPosition(child));
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, childLayoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(child.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);

                // Set same.
                control.SetCellPosition(child, value);
                Assert.Equal(value, control.GetCellPosition(child));
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, childLayoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(child.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void TableLayoutPanel_SetCellPosition_InvokeMultipleTimes_GetReturnsExpected()
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            control.SetCellPosition(child, new TableLayoutPanelCellPosition(1, 1));
            Assert.Equal(new TableLayoutPanelCellPosition(1, 1), control.GetCellPosition(child));

            control.SetCellPosition(child, new TableLayoutPanelCellPosition(2, 2));
            Assert.Equal(new TableLayoutPanelCellPosition(2, 2), control.GetCellPosition(child));
        }

        [WinFormsFact]
        public void TableLayoutPanel_SetCellPosition_NullControl_ThrowsArgumentNullException()
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => control.SetCellPosition(null, new TableLayoutPanelCellPosition()));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(int.MaxValue)]
        public void TableLayoutPanel_SetColumn_Invoke_GetReturnsExpected(int value)
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int childLayoutCallCount = 0;
            child.Layout += (sender, e) => childLayoutCallCount++;

            control.SetColumn(child, value);
            Assert.Equal(value, control.GetColumn(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);

            // Set same.
            control.SetColumn(child, value);
            Assert.Equal(value, control.GetColumn(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-1, 1)]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(int.MaxValue, 1)]
        public void TableLayoutPanel_SetColumn_InvokeControlWithParent_GetReturnsExpected(int value, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var child = new Control
            {
                Parent = parent
            };
            using var control = new TableLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int childLayoutCallCount = 0;
            child.Layout += (sender, e) => childLayoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs eventArgs)
            {
                Assert.Same(parent, sender);
                Assert.Same(child, eventArgs.AffectedControl);
                Assert.Equal("TableIndex", eventArgs.AffectedProperty);
                parentLayoutCallCount++;
            }
            parent.Layout += parentHandler;

            try
            {
                control.SetColumn(child, value);
                Assert.Equal(value, control.GetColumn(child));
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, childLayoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(child.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);

                // Set same.
                control.SetColumn(child, value);
                Assert.Equal(value, control.GetColumn(child));
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, childLayoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(child.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void TableLayoutPanel_SetColumn_InvokeMultipleTimes_GetReturnsExpected()
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            control.SetColumn(child, 1);
            Assert.Equal(1, control.GetColumn(child));

            control.SetColumn(child, 2);
            Assert.Equal(2, control.GetColumn(child));
        }

        [WinFormsFact]
        public void TableLayoutPanel_SetColumn_NullControl_ThrowsArgumentNullException()
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => control.SetColumn(null, 1));
        }

        [WinFormsFact]
        public void TableLayoutPanel_SetColumn_InvalidColumn_ThrowsArgumentOutOfRangeException()
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("column", () => control.SetColumn(new Control(), -2));
        }

        [WinFormsTheory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(int.MaxValue)]
        public void TableLayoutPanel_SetColumnSpan_Invoke_GetReturnsExpected(int value)
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int childLayoutCallCount = 0;
            child.Layout += (sender, e) => childLayoutCallCount++;

            control.SetColumnSpan(child, value);
            Assert.Equal(value, control.GetColumnSpan(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);

            // Set same.
            control.SetColumnSpan(child, value);
            Assert.Equal(value, control.GetColumnSpan(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(int.MaxValue, 1)]
        public void TableLayoutPanel_SetColumnSpan_InvokeControlWithParent_GetReturnsExpected(int value, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var child = new Control
            {
                Parent = parent
            };
            using var control = new TableLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int childLayoutCallCount = 0;
            child.Layout += (sender, e) => childLayoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs eventArgs)
            {
                Assert.Same(parent, sender);
                Assert.Same(child, eventArgs.AffectedControl);
                Assert.Equal("ColumnSpan", eventArgs.AffectedProperty);
                parentLayoutCallCount++;
            }
            parent.Layout += parentHandler;

            try
            {
                control.SetColumnSpan(child, value);
                Assert.Equal(value, control.GetColumnSpan(child));
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, childLayoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(child.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);

                // Set same.
                control.SetColumnSpan(child, value);
                Assert.Equal(value, control.GetColumnSpan(child));
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, childLayoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount + 1, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(child.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void TableLayoutPanel_SetColumnSpan_InvokeMultipleTimes_GetReturnsExpected()
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();

            control.SetColumnSpan(child, 1);
            Assert.Equal(1, control.GetColumnSpan(child));

            control.SetColumnSpan(child, 2);
            Assert.Equal(2, control.GetColumnSpan(child));
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        public void TableLayoutPanel_SetColumnSpan_NullControl_ThrowsArgumentNullException(int value)
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => control.SetColumnSpan(null, value));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        public void TableLayoutPanel_SetColumnSpan_InvalidValue_ThrowsArgumentOutOfRangeException(int value)
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.SetColumnSpan(new Control(), value));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(int.MaxValue)]
        public void TableLayoutPanel_SetRow_Invoke_GetReturnsExpected(int value)
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int childLayoutCallCount = 0;
            child.Layout += (sender, e) => childLayoutCallCount++;

            control.SetRow(child, value);
            Assert.Equal(value, control.GetRow(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);

            // Set same.
            control.SetRow(child, value);
            Assert.Equal(value, control.GetRow(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-1, 1)]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(int.MaxValue, 1)]
        public void TableLayoutPanel_SetRow_InvokeControlWithParent_GetReturnsExpected(int value, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var child = new Control
            {
                Parent = parent
            };
            using var control = new TableLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int childLayoutCallCount = 0;
            child.Layout += (sender, e) => childLayoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs eventArgs)
            {
                Assert.Same(parent, sender);
                Assert.Same(child, eventArgs.AffectedControl);
                Assert.Equal("TableIndex", eventArgs.AffectedProperty);
                parentLayoutCallCount++;
            }
            parent.Layout += parentHandler;

            try
            {
                control.SetRow(child, value);
                Assert.Equal(value, control.GetRow(child));
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, childLayoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(child.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);

                // Set same.
                control.SetRow(child, value);
                Assert.Equal(value, control.GetRow(child));
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, childLayoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(child.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void TableLayoutPanel_SetRow_InvokeMultipleTimes_GetReturnsExpected()
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            control.SetRow(child, 1);
            Assert.Equal(1, control.GetRow(child));

            control.SetRow(child, 2);
            Assert.Equal(2, control.GetRow(child));
        }

        [WinFormsFact]
        public void TableLayoutPanel_SetRow_NullControl_ThrowsArgumentNullException()
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => control.SetRow(null, 1));
        }

        [WinFormsFact]
        public void TableLayoutPanel_SetRow_InvalidRow_ThrowsArgumentOutOfRangeException()
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("row", () => control.SetRow(new Control(), -2));
        }

        [WinFormsTheory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(int.MaxValue)]
        public void TableLayoutPanel_SetRowSpan_Invoke_GetReturnsExpected(int value)
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int childLayoutCallCount = 0;
            child.Layout += (sender, e) => childLayoutCallCount++;

            control.SetRowSpan(child, value);
            Assert.Equal(value, control.GetRowSpan(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);

            // Set same.
            control.SetRowSpan(child, value);
            Assert.Equal(value, control.GetRowSpan(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(int.MaxValue, 1)]
        public void TableLayoutPanel_SetRowSpan_InvokeControlWithParent_GetReturnsExpected(int value, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var child = new Control
            {
                Parent = parent
            };
            using var control = new TableLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int childLayoutCallCount = 0;
            child.Layout += (sender, e) => childLayoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs eventArgs)
            {
                Assert.Same(parent, sender);
                Assert.Same(child, eventArgs.AffectedControl);
                Assert.Equal("RowSpan", eventArgs.AffectedProperty);
                parentLayoutCallCount++;
            }
            parent.Layout += parentHandler;

            try
            {
                control.SetRowSpan(child, value);
                Assert.Equal(value, control.GetRowSpan(child));
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, childLayoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(child.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);

                // Set same.
                control.SetRowSpan(child, value);
                Assert.Equal(value, control.GetRowSpan(child));
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, childLayoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount + 1, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(child.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void TableLayoutPanel_SetRowSpan_InvokeMultipleTimes_GetReturnsExpected()
        {
            using var child = new Control();
            using var control = new TableLayoutPanel();
            control.SetRowSpan(child, 1);
            Assert.Equal(1, control.GetRowSpan(child));

            control.SetRowSpan(child, 2);
            Assert.Equal(2, control.GetRowSpan(child));
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        public void TableLayoutPanel_SetRowSpan_NullControl_ThrowsArgumentNullException(int value)
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => control.SetRowSpan(null, value));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        public void TableLayoutPanel_SetRowSpan_InvalidValue_ThrowsArgumentOutOfRangeException(int value)
        {
            using var control = new TableLayoutPanel();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.SetRowSpan(new Control(), value));
        }

        [WinFormsFact]
        public void TableLayoutPanel_IExtenderProviderCanExtend_InvokeParent_ReturnsTrue()
        {
            using var extendee = new Control();
            using var control = new TableLayoutPanel();
            extendee.Parent = control;

            IExtenderProvider extenderProvider = control;
            Assert.True(extenderProvider.CanExtend(extendee));
        }

        public static IEnumerable<object[]> IExtenderProvider_CanExtend_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
            yield return new object[] { new Control() };
        }

        [WinFormsTheory]
        [MemberData(nameof(IExtenderProvider_CanExtend_TestData))]
        public void TableLayoutPanel_IExtenderProviderCanExtend_NotControl_ReturnsFalse(object obj)
        {
            using var control = new TableLayoutPanel();
            IExtenderProvider extenderProvider = control;
            Assert.False(extenderProvider.CanExtend(obj));
        }

        [TypeDescriptionProvider(typeof(CustomTypeDescriptionProvider))]
        private class ControlWithNullName : Control
        {
        }

        private class CustomTypeDescriptionProvider : TypeDescriptionProvider
        {
            public CustomTypeDescriptionProvider()
            {
            }

            public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
            {
                var mockDescriptor = new Mock<ICustomTypeDescriptor>(MockBehavior.Strict);
                mockDescriptor
                    .Setup(c => c.GetProperties())
                    .Returns(new PropertyDescriptorCollection(Array.Empty<PropertyDescriptor>()));
                return mockDescriptor.Object;
            }
        }

        private class SubTableLayoutPanel : TableLayoutPanel
        {
            public new const int ScrollStateAutoScrolling = TableLayoutPanel.ScrollStateAutoScrolling;

            public new const int ScrollStateHScrollVisible = TableLayoutPanel.ScrollStateHScrollVisible;

            public new const int ScrollStateVScrollVisible = TableLayoutPanel.ScrollStateVScrollVisible;

            public new const int ScrollStateUserHasScrolled = TableLayoutPanel.ScrollStateUserHasScrolled;

            public new const int ScrollStateFullDrag = TableLayoutPanel.ScrollStateFullDrag;

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

            public new bool ShowFocusCues => base.ShowFocusCues;

            public new bool ShowKeyboardCues => base.ShowKeyboardCues;

            public new bool VScroll
            {
                get => base.VScroll;
                set => base.VScroll = value;
            }

            public new ControlCollection CreateControlsInstance() => base.CreateControlsInstance();

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetScrollState(int bit) => base.GetScrollState(bit);

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public new void OnCellPaint(TableLayoutCellPaintEventArgs e) => base.OnCellPaint(e);

            public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

            public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

            public new void OnLayout(LayoutEventArgs levent) => base.OnLayout(levent);

            public new void OnPaintBackground(PaintEventArgs e) => base.OnPaintBackground(e);

            public new void ScaleControl(SizeF factor, BoundsSpecified specified) => base.ScaleControl(factor, specified);

            public new void ScaleCore(float dx, float dy) => base.ScaleCore(dx, dy);

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
        }
    }
}

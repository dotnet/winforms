// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.VisualStyles;
using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class DataGridViewHeaderCellTests : ControlTestBase
    {
        public DataGridViewHeaderCellTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        public static IEnumerable<object[]> MouseDownUnsharesRow_WithDataGridView_TestData()
        {
            foreach (bool enableHeadersVisualStyles in new bool[] { true, false })
            {
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), false };
            }
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_MouseDownUnsharesRow_InvokeWithDataGridView_ReturnsExpected()
        {
            foreach (object[] testData in MouseDownUnsharesRow_WithDataGridView_TestData())
            {
                bool enableHeadersVisualStyles = (bool)testData[0];
                DataGridViewCellMouseEventArgs e = (DataGridViewCellMouseEventArgs)testData[1];
                bool expected = (bool)testData[2];

                Application.EnableVisualStyles();

                using var cellTemplate = new SubDataGridViewHeaderCell();
                using var column = new DataGridViewColumn
                {
                    CellTemplate = cellTemplate
                };
                using var control = new DataGridView
                {
                    EnableHeadersVisualStyles = enableHeadersVisualStyles
                };
                control.Columns.Add(column);
                SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
                Assert.Equal(expected, cell.MouseDownUnsharesRow(e));
                Assert.Equal(ButtonState.Normal, cell.ButtonState);
                Assert.False(control.IsHandleCreated);
            }
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_MouseUpUnsharesRow_InvokeWithDataGridView_ReturnsExpected()
        {
            foreach (object[] testData in MouseDownUnsharesRow_WithDataGridView_TestData())
            {
                bool enableHeadersVisualStyles = (bool)testData[0];
                DataGridViewCellMouseEventArgs e = (DataGridViewCellMouseEventArgs)testData[1];
                bool expected = (bool)testData[2];

                Application.EnableVisualStyles();

                using var cellTemplate = new SubDataGridViewHeaderCell();
                using var column = new DataGridViewColumn
                {
                    CellTemplate = cellTemplate
                };
                using var control = new DataGridView
                {
                    EnableHeadersVisualStyles = enableHeadersVisualStyles
                };
                control.Columns.Add(column);
                SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
                Assert.Equal(expected, cell.MouseUpUnsharesRow(e));
                Assert.Equal(ButtonState.Normal, cell.ButtonState);
                Assert.False(control.IsHandleCreated);
            }
        }

        public static IEnumerable<object[]> MouseLeaveUnsharesRow_WithDataGridViewMouseDown_TestData()
        {
            ButtonState expected = VisualStyleRenderer.IsSupported ? ButtonState.Pushed : ButtonState.Normal;
            yield return new object[] { true, -2, expected };
            yield return new object[] { true, -1, expected };
            yield return new object[] { true, 0, expected };
            yield return new object[] { true, 1, expected };
            yield return new object[] { false, -2, ButtonState.Normal };
            yield return new object[] { false, -1, ButtonState.Normal };
            yield return new object[] { false, 0, ButtonState.Normal };
            yield return new object[] { false, 1, ButtonState.Normal };
        }

        [WinFormsTheory]
        [MemberData(nameof(MouseLeaveUnsharesRow_WithDataGridViewMouseDown_TestData))]
        public void DataGridViewHeaderCell_MouseLeaveUnsharesRow_InvokeWithDataGridViewMouseDown_ReturnsExpected(bool enableHeadersVisualStyles, int rowIndex, ButtonState expectedButtonState)
        {
            Application.EnableVisualStyles();

            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            cell.OnMouseDown(new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)));
            Assert.Equal(enableHeadersVisualStyles && VisualStyleRenderer.IsSupported, cell.MouseLeaveUnsharesRow(rowIndex));
            Assert.Equal(expectedButtonState, cell.ButtonState);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnMouseDown_WithDataGridView_TestData()
        {
            foreach (bool enableHeadersVisualStyles in new bool[] { true, false })
            {
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles ? ButtonState.Pushed : ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles ? ButtonState.Pushed : ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles ? ButtonState.Pushed : ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles ? ButtonState.Pushed : ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles ? ButtonState.Pushed : ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), ButtonState.Normal };
            }

            yield return new object[] { false, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), ButtonState.Normal };
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_OnMouseDown_InvokeWithDataGridView_Nop()
        {
            foreach (object[] testData in OnMouseDown_WithDataGridView_TestData())
            {
                bool enableHeadersVisualStyles = (bool)testData[0];
                DataGridViewCellMouseEventArgs e = (DataGridViewCellMouseEventArgs)testData[1];
                ButtonState expectedButtonState = (ButtonState)testData[2];

                Application.EnableVisualStyles();

                using var cellTemplate = new SubDataGridViewHeaderCell();
                using var column = new DataGridViewColumn
                {
                    CellTemplate = cellTemplate
                };
                using var control = new DataGridView
                {
                    EnableHeadersVisualStyles = enableHeadersVisualStyles
                };
                control.Columns.Add(column);
                SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
                cell.OnMouseDown(e);
                Assert.Equal(expectedButtonState, cell.ButtonState);
                Assert.False(control.IsHandleCreated);
            }
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_OnMouseDown_InvalidRowIndexVisualStyles_ThrowsArgumentOutOfRangeException()
        {
            Application.EnableVisualStyles();

            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = true
            };
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            var e = new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.OnMouseDown(e));
            Assert.Equal(VisualStyleRenderer.IsSupported ? ButtonState.Pushed : ButtonState.Normal, cell.ButtonState);
        }

        [WinFormsTheory(Skip = "Crashes the test host process.")]
        [InlineData(true, -1)]
        [InlineData(true, 0)]
        [InlineData(false, -2)]
        [InlineData(false, -1)]
        [InlineData(false, 0)]
        [InlineData(false, 1)]
        public void DataGridViewHeaderCell_OnMouseLeave_InvokeWithDataGridViewMouseDown_Nop(bool enableHeadersVisualStyles, int rowIndex)
        {
            Application.EnableVisualStyles();

            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            cell.OnMouseDown(new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)));
            cell.OnMouseLeave(rowIndex);
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory(Skip ="Crashes the test host process.")]
        [InlineData(-2)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_OnMouseLeave_InvalidRowIndexVisualStyles_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            Application.EnableVisualStyles();

            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = true
            };
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            cell.OnMouseDown(new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)));
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.OnMouseLeave(rowIndex));
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_OnMouseUp_InvalidRowIndexVisualStyles_ThrowsArgumentOutOfRangeException()
        {
            Application.EnableVisualStyles();

            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = true
            };
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            var e = new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.OnMouseUp(e));
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
        }

        public static IEnumerable<object[]> OnMouseUp_WithDataGridViewMouseDown_TestData()
        {
            foreach (bool enableHeadersVisualStyles in new bool[] { true, false })
            {
                ButtonState expectedButtonState1 = enableHeadersVisualStyles && VisualStyleRenderer.IsSupported ? ButtonState.Pushed : ButtonState.Normal;
                ButtonState expectedButtonState2 = enableHeadersVisualStyles && VisualStyleRenderer.IsSupported ? ButtonState.Normal : expectedButtonState1;
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), expectedButtonState2 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), expectedButtonState2 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), expectedButtonState2 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), expectedButtonState2 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), expectedButtonState2 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), expectedButtonState1 };
            }

            yield return new object[] { false, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), ButtonState.Normal };
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_OnMouseUp_InvokeWithDataGridViewMouseDown_ReturnsExpected()
        {
            foreach (object[] testData in OnMouseUp_WithDataGridViewMouseDown_TestData())
            {
                bool enableHeadersVisualStyles = (bool)testData[0];
                DataGridViewCellMouseEventArgs e = (DataGridViewCellMouseEventArgs)testData[1];
                ButtonState expectedButtonState = (ButtonState)testData[2];

                Application.EnableVisualStyles();

                using var cellTemplate = new SubDataGridViewHeaderCell();
                using var column = new DataGridViewColumn
                {
                    CellTemplate = cellTemplate
                };
                using var control = new DataGridView
                {
                    EnableHeadersVisualStyles = enableHeadersVisualStyles
                };
                control.Columns.Add(column);
                SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
                cell.OnMouseDown(new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)));
                cell.OnMouseUp(e);
                Assert.Equal(expectedButtonState, cell.ButtonState);
                Assert.False(control.IsHandleCreated);
            }
        }

        public class SubDataGridViewHeaderCell : DataGridViewHeaderCell
        {
            public new ButtonState ButtonState => base.ButtonState;

            public new void Dispose(bool disposing) => base.Dispose(disposing);

            public new Size GetSize(int rowIndex) => base.GetSize(rowIndex);

            public new object GetValue(int rowIndex) => base.GetValue(rowIndex);

            public new bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e) => base.MouseDownUnsharesRow(e);

            public new bool MouseEnterUnsharesRow(int rowIndex) => base.MouseEnterUnsharesRow(rowIndex);

            public new bool MouseLeaveUnsharesRow(int rowIndex) => base.MouseLeaveUnsharesRow(rowIndex);

            public new bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e) => base.MouseUpUnsharesRow(e);

            public new void OnMouseDown(DataGridViewCellMouseEventArgs e) => base.OnMouseDown(e);

            public new void OnMouseEnter(int rowIndex) => base.OnMouseEnter(rowIndex);

            public new void OnMouseLeave(int rowIndex) => base.OnMouseLeave(rowIndex);

            public new void OnMouseUp(DataGridViewCellMouseEventArgs e) => base.OnMouseUp(e);

            public new void Paint(Graphics graphics,
                                  Rectangle clipBounds,
                                  Rectangle cellBounds,
                                  int rowIndex,
                                  DataGridViewElementStates dataGridViewElementState,
                                  object value,
                                  object formattedValue,
                                  string errorText,
                                  DataGridViewCellStyle cellStyle,
                                  DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                  DataGridViewPaintParts paintParts)
            {
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            }
        }
    }
}

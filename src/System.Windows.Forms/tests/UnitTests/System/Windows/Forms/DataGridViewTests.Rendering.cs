// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Numerics;
using System.Windows.Forms.Metafiles;
using static System.Windows.Forms.Metafiles.DataHelpers;

namespace System.Windows.Forms.Tests;

public partial class DataGridViewTests
{
    [WinFormsFact]
    public void DataGridView_GridColor_Rendering()
    {
        using Form form = new();

        // Only want to render one cell to validate
        using DataGridView dataGrid = new()
        {
            GridColor = Color.Blue,
            ColumnCount = 1,
            RowCount = 1,
            ColumnHeadersVisible = false,
            RowHeadersVisible = false
        };

        form.Controls.Add(dataGrid);

        using EmfScope emf = new();
        DeviceContextState state = new(emf);

        dataGrid.PrintToMetafile(emf);

        Assert.Equal(new Rectangle(0, 0, 240, 150), dataGrid.Bounds);
        Assert.Equal(new Size(100, 25), dataGrid[0, 0].Size);

        Rectangle bounds = dataGrid.Bounds;

        // For whatever reason GDI+ renders as polylines scaled 16x with a 1/16th world transform applied.
        // For test readability we'll transform the points from our coordinates to the logical coordinates.
        Matrix3x2 oneSixteenth = Matrix3x2.CreateScale(0.0625f);
        Matrix3x2 times16 = Matrix3x2.CreateScale(16.0f);

        // This is the default pen style GDI+ renders with
        PEN_STYLE penStyle = PEN_STYLE.PS_SOLID | PEN_STYLE.PS_JOIN_ROUND | PEN_STYLE.PS_COSMETIC |
            PEN_STYLE.PS_ENDCAP_FLAT | PEN_STYLE.PS_JOIN_MITER | PEN_STYLE.PS_GEOMETRIC;

        // Don't really care about the bounds, just the actual shapes/lines
        emf.Validate(
            state,
            // The datagrid background
            Validate.Polygon16(
                bounds: null,
                PointArray(times16, 1, 1, 1, 149, 239, 149, 239, 1, 1, 1),
                State.Pen(1, Color.Empty, PEN_STYLE.PS_NULL),
                State.Brush(SystemColors.ButtonShadow, BRUSH_STYLE.BS_SOLID),
                State.Transform(oneSixteenth)),
            // Left cell border
            Validate.Polyline16(
                bounds: null,
                PointArray(times16, 1, 1, 1, 26),
                State.Pen(16, Color.Blue, penStyle),
                State.Transform(oneSixteenth)),
            // Right cell border
            Validate.Polyline16(
                bounds: null,
                PointArray(times16, 101, 1, 101, 26),
                State.Pen(16, Color.Blue, penStyle),
                State.Transform(oneSixteenth)),
            // Top cell border
            Validate.Polyline16(
                bounds: null,
                PointArray(times16, 1, 1, 101, 1),
                State.Pen(16, Color.Blue, penStyle),
                State.Transform(oneSixteenth)),
            // Bottom cell border
            Validate.Polyline16(
                bounds: null,
                PointArray(times16, 1, 26, 101, 26),
                State.Pen(16, Color.Blue, penStyle),
                State.Transform(oneSixteenth)),
            // Cell background
            Validate.Polygon16(
                bounds: null,
                PointArray(times16, 2, 2, 2, 26, 101, 26, 101, 2, 2, 2),
                State.Pen(1, Color.Empty, PEN_STYLE.PS_NULL),
                State.Brush(SystemColors.ButtonHighlight, BRUSH_STYLE.BS_SOLID),
                State.Transform(oneSixteenth)),
            // Datagrid border
            Validate.Polygon16(
                bounds: null,
                PointArray(times16, 0, 0, 239, 0, 239, 149, 0, 149),
                State.Pen(16, SystemColors.Desktop, penStyle),
                State.Brush(Color.Empty, BRUSH_STYLE.BS_NULL),
                State.Transform(oneSixteenth)));
    }

    [WinFormsFact]
    public void DataGridView_DefaultCellStyles_Rendering()
    {
        #region DataGridView setup

        using Font formFont1 = new("Times New Roman", 12F, FontStyle.Regular);
        using Form form = new Form
        {
            Font = formFont1,
            Size = new Size(700, 200)
        };

        using Font customCellStyleFont = new("Tahoma", 8.25F, FontStyle.Regular);
        using Font customColumnHeaderFont = new("Consolas", 14F, FontStyle.Italic);
        using Font customRowHeaderFont = new("Arial", 9F, FontStyle.Bold);

        DataGridViewCellStyle defaultCellStyle = new()
        {
            Font = customCellStyleFont,

            // We must supply a completely initialised instance, else we'd be receiving a copy
            // refer to DefaultCellStyle implementation

            Alignment = DataGridViewContentAlignment.MiddleLeft,
            BackColor = SystemColors.Info,
            ForeColor = Color.Maroon,
            SelectionBackColor = SystemColors.Highlight,
            SelectionForeColor = SystemColors.HighlightText,
            WrapMode = DataGridViewTriState.False
        };

        using DataGridView dataGridView = new DataGridView
        {
            DefaultCellStyle = defaultCellStyle,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { Font = customColumnHeaderFont },
            RowHeadersDefaultCellStyle = new DataGridViewCellStyle { Font = customRowHeaderFont },

            Dock = DockStyle.Fill,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
        };

        DataGridViewTextBoxColumn column1 = new() { HeaderText = "Style" };
        DataGridViewTextBoxColumn column2 = new() { HeaderText = "Font" };
        dataGridView.Columns.AddRange(new[] { column1, column2, });
        dataGridView.Rows.Add(nameof(DataGridView.DefaultCellStyle), customCellStyleFont.ToString());
        dataGridView.Rows.Add(nameof(DataGridView.ColumnHeadersDefaultCellStyle), customColumnHeaderFont.ToString());
        dataGridView.Rows.Add(nameof(DataGridView.RowHeadersDefaultCellStyle), customRowHeaderFont.ToString());
        for (int i = 0; i < dataGridView.Rows.Count; i++)
        {
            dataGridView.Rows[i].HeaderCell.Value = $"Row {i + 1}";
        }

        dataGridView.RowHeadersWidth = 100;

        Assert.Same(customCellStyleFont, dataGridView.DefaultCellStyle.Font);
        Assert.Same(customColumnHeaderFont, dataGridView.ColumnHeadersDefaultCellStyle.Font);
        Assert.Same(customRowHeaderFont, dataGridView.RowHeadersDefaultCellStyle.Font);

        // Add the datagridview to the form, this will trigger Font change via OnFontChanged
        form.Controls.Add(dataGridView);

        #endregion

        using EmfScope emf = new();
        DeviceContextState state = new(emf);

        Assert.NotEqual(IntPtr.Zero, dataGridView.Handle);
        dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

        dataGridView.PrintToMetafile(emf);

        // We only care about font styles, nothing else
        emf.Validate(
            state,

            // Column headers
            Validate.SkipTo(
                Validate.TextOut(column1.HeaderText, stateValidators: State.FontFace(customColumnHeaderFont.Name))),
            Validate.SkipTo(
                Validate.TextOut(column2.HeaderText, stateValidators: State.FontFace(customColumnHeaderFont.Name))),

            // Row1
            Validate.SkipTo(
                Validate.TextOut("Row 1", stateValidators: State.FontFace(customRowHeaderFont.Name))),
            Validate.SkipTo(
                Validate.TextOut(nameof(DataGridView.DefaultCellStyle), stateValidators: State.FontFace(customCellStyleFont.Name))),
            Validate.SkipTo(
                Validate.TextOut(customCellStyleFont.ToString(), stateValidators: State.FontFace(customCellStyleFont.Name))),

            // Row2
            Validate.SkipTo(
                Validate.TextOut("Row 2", stateValidators: State.FontFace(customRowHeaderFont.Name))),
            Validate.SkipTo(
                Validate.TextOut(nameof(DataGridView.ColumnHeadersDefaultCellStyle), stateValidators: State.FontFace(customCellStyleFont.Name))),
            Validate.SkipTo(
                Validate.TextOut(customColumnHeaderFont.ToString(), stateValidators: State.FontFace(customCellStyleFont.Name))),

            // Row3
            Validate.SkipTo(
                Validate.TextOut("Row 3", stateValidators: State.FontFace(customRowHeaderFont.Name))),
            Validate.SkipTo(
                Validate.TextOut(nameof(DataGridView.RowHeadersDefaultCellStyle), stateValidators: State.FontFace(customCellStyleFont.Name))),
            Validate.SkipTo(
                Validate.TextOut(customRowHeaderFont.ToString(), stateValidators: State.FontFace(customCellStyleFont.Name))),

            Validate.SkipAll());
    }
}

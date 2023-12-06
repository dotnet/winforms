// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

public class DataGridViewCellPaintingEventArgs : HandledEventArgs, IDataGridViewCellEventArgs
{
    private readonly DataGridView _dataGridView;

    public DataGridViewCellPaintingEventArgs(
        DataGridView dataGridView,
        Graphics graphics,
        Rectangle clipBounds,
        Rectangle cellBounds,
        int rowIndex,
        int columnIndex,
        DataGridViewElementStates cellState,
        object? value,
        object? formattedValue,
        string? errorText,
        DataGridViewCellStyle cellStyle,
        DataGridViewAdvancedBorderStyle? advancedBorderStyle,
        DataGridViewPaintParts paintParts)
    {
        if ((paintParts & ~DataGridViewPaintParts.All) != 0)
        {
            throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewPaintPartsCombination, nameof(paintParts)), nameof(paintParts));
        }

        _dataGridView = dataGridView.OrThrowIfNull();
        Graphics = graphics.OrThrowIfNull();
        ClipBounds = clipBounds;
        CellBounds = cellBounds;
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
        State = cellState;
        Value = value;
        FormattedValue = formattedValue;
        ErrorText = errorText;
        CellStyle = cellStyle.OrThrowIfNull();
        AdvancedBorderStyle = advancedBorderStyle;
        PaintParts = paintParts;
    }

    internal DataGridViewCellPaintingEventArgs(DataGridView dataGridView)
    {
        Debug.Assert(dataGridView is not null);
        _dataGridView = dataGridView;
    }

    public Graphics? Graphics { get; private set; }

    public DataGridViewAdvancedBorderStyle? AdvancedBorderStyle { get; private set; }

    public Rectangle CellBounds { get; private set; }

    public Rectangle ClipBounds { get; private set; }

    public int RowIndex { get; private set; }

    public int ColumnIndex { get; private set; }

    public DataGridViewElementStates State { get; private set; }

    public object? Value { get; private set; }

    public object? FormattedValue { get; private set; }

    public string? ErrorText { get; private set; }

    public DataGridViewCellStyle? CellStyle { get; private set; }

    public DataGridViewPaintParts PaintParts { get; private set; }

    public void Paint(Rectangle clipBounds, DataGridViewPaintParts paintParts)
    {
        if (RowIndex < -1 || RowIndex >= _dataGridView.Rows.Count)
        {
            throw new InvalidOperationException(SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange);
        }

        if (ColumnIndex < -1 || ColumnIndex >= _dataGridView.Columns.Count)
        {
            throw new InvalidOperationException(SR.DataGridViewElementPaintingEventArgs_ColumnIndexOutOfRange);
        }

        // We check inside PaintInternal if some of the parameters are null.
        _dataGridView
            .GetCellInternal(ColumnIndex, RowIndex)
            .PaintInternal(
                Graphics!,
                clipBounds,
                CellBounds,
                RowIndex,
                State,
                Value,
                FormattedValue,
                ErrorText,
                CellStyle!,
                AdvancedBorderStyle!,
                paintParts);
    }

    public void PaintBackground(Rectangle clipBounds, bool cellsPaintSelectionBackground)
    {
        if (RowIndex < -1 || RowIndex >= _dataGridView.Rows.Count)
        {
            throw new InvalidOperationException(SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange);
        }

        if (ColumnIndex < -1 || ColumnIndex >= _dataGridView.Columns.Count)
        {
            throw new InvalidOperationException(SR.DataGridViewElementPaintingEventArgs_ColumnIndexOutOfRange);
        }

        DataGridViewPaintParts paintParts = DataGridViewPaintParts.Background | DataGridViewPaintParts.Border;
        if (cellsPaintSelectionBackground)
        {
            paintParts |= DataGridViewPaintParts.SelectionBackground;
        }

        // We check inside PaintInternal if some of the parameters are null.
        _dataGridView
            .GetCellInternal(ColumnIndex, RowIndex)
            .PaintInternal(
                Graphics!,
                clipBounds,
                CellBounds,
                RowIndex,
                State,
                Value,
                FormattedValue,
                ErrorText,
                CellStyle!,
                AdvancedBorderStyle!,
                paintParts);
    }

    public void PaintContent(Rectangle clipBounds)
    {
        if (RowIndex < -1 || RowIndex >= _dataGridView.Rows.Count)
        {
            throw new InvalidOperationException(SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange);
        }

        if (ColumnIndex < -1 || ColumnIndex >= _dataGridView.Columns.Count)
        {
            throw new InvalidOperationException(SR.DataGridViewElementPaintingEventArgs_ColumnIndexOutOfRange);
        }

        // We check inside PaintInternal if some of the parameters are null.
        _dataGridView
            .GetCellInternal(ColumnIndex, RowIndex)
            .PaintInternal(
                Graphics!,
                clipBounds,
                CellBounds,
                RowIndex,
                State,
                Value,
                FormattedValue,
                ErrorText,
                CellStyle!,
                AdvancedBorderStyle!,
                DataGridViewPaintParts.ContentBackground | DataGridViewPaintParts.ContentForeground | DataGridViewPaintParts.ErrorIcon);
    }

    internal void SetProperties(
        Graphics graphics,
        Rectangle clipBounds,
        Rectangle cellBounds,
        int rowIndex,
        int columnIndex,
        DataGridViewElementStates cellState,
        object? value,
        object? formattedValue,
        string? errorText,
        DataGridViewCellStyle cellStyle,
        DataGridViewAdvancedBorderStyle advancedBorderStyle,
        DataGridViewPaintParts paintParts)
    {
        Debug.Assert(graphics is not null);

        Graphics = graphics;
        ClipBounds = clipBounds;
        CellBounds = cellBounds;
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
        State = cellState;
        Value = value;
        FormattedValue = formattedValue;
        ErrorText = errorText;
        CellStyle = cellStyle;
        AdvancedBorderStyle = advancedBorderStyle;
        PaintParts = paintParts;
        Handled = false;
    }
}

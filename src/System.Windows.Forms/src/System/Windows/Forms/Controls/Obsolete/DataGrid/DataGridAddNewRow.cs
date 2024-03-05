// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

#nullable disable
[Obsolete("DataGridAddNewRow has been deprecated.")]
internal class DataGridAddNewRow : DataGridRow
{
    public DataGridAddNewRow(DataGrid dGrid, DataGridTableStyle gridTable, int rowNum)
        : base(dGrid, gridTable, rowNum)
    {
        throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool DataBound
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    public override void OnEdit()
    {
        throw new PlatformNotSupportedException();
    }

    public override void OnRowLeave()
    {
        throw new PlatformNotSupportedException();
    }

    internal override void LoseChildFocus(Rectangle rowHeader, bool alignToRight)
    {
    }

    internal override bool ProcessTabKey(Keys keyData, Rectangle rowHeaders, bool alignToRight)
    {
        return false;
    }

    public override int Paint(Graphics g, Rectangle bounds, Rectangle trueRowBounds, int firstVisibleColumn, int columnCount)
    {
        throw new PlatformNotSupportedException();
    }

    public override int Paint(Graphics g,
                              Rectangle bounds,
                              Rectangle trueRowBounds,
                              int firstVisibleColumn,
                              int columnCount,
                              bool alignToRight)
    {
        throw new PlatformNotSupportedException();
    }

    protected override void PaintCellContents(Graphics g, Rectangle cellBounds, DataGridColumnStyle column,
                                              Brush backBr, Brush foreBrush, bool alignToRight)
    {
        if (DataBound)
        {
            CurrencyManager listManager = DataGrid.ListManager;
            column.Paint(g, cellBounds, listManager, this.RowNumber, alignToRight);
        }
        else
        {
            base.PaintCellContents(g, cellBounds, column, backBr, foreBrush, alignToRight);
        }
    }
}

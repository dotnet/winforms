// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms;

#pragma warning disable RS0016
#nullable disable
[Obsolete("DataGridTextBoxColumn has been deprecated.  Use DataGridViewTextBoxColumn instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
public class DataGridTextBoxColumn : DataGridColumnStyle
{
    private int xMargin = 2;
    private int yMargin = 1;

    private DataGridTextBox edit;

    private string oldValue;
    private int editRow = -1;

    public DataGridTextBoxColumn() : this(null, null)
    {
        edit = new DataGridTextBox();
        throw new PlatformNotSupportedException();
    }

    public DataGridTextBoxColumn(PropertyDescriptor prop)
    : this(prop, null, false)
    {
        throw new PlatformNotSupportedException();
    }

    public DataGridTextBoxColumn(PropertyDescriptor prop, string format) : this(prop, format, false)
    {
        throw new PlatformNotSupportedException();
    }

    public DataGridTextBoxColumn(PropertyDescriptor prop, string format, bool isDefault) : base(prop, isDefault)
    {
        throw new PlatformNotSupportedException();
    }

    public DataGridTextBoxColumn(PropertyDescriptor prop, bool isDefault) : this(prop, null, isDefault)
    {
        throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public virtual TextBox TextBox
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal override bool KeyPress(int rowNum, Keys keyData)
    {
        if (edit.IsInEditOrNavigateMode)
            return base.KeyPress(rowNum, keyData);

        return false;
    }

    protected override void SetDataGridInColumn(DataGrid value)
    {
        base.SetDataGridInColumn(value);
        if (edit.ParentInternal is not null)
        {
            edit.ParentInternal.Controls.Remove(edit);
        }

        if (value is not null)
        {
            value.Controls.Add(edit);
        }

        // we have to tell the edit control about its dataGrid
        edit.SetDataGrid(value);
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override PropertyDescriptor PropertyDescriptor
    {
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public string Format
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

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public IFormatProvider FormatInfo
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

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override bool ReadOnly
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

    private static void DebugOut(string s)
    {
        Debug.WriteLineIf(CompModSwitches.DGEditColumnEditing.TraceVerbose, "DGEditColumnEditing: " + s);
    }

    protected internal override void ConcedeFocus()
    {
        edit.Bounds = Rectangle.Empty;
    }

    protected void HideEditBox()
    {
        bool wasFocused = edit.Focused;
        edit.Visible = false;

        if (wasFocused && DataGridTableStyle is not null && DataGridTableStyle.DataGrid is not null && DataGridTableStyle.DataGrid.CanFocus)
        {
            Debug.Assert(!edit.Focused, "the edit control just conceeded focus to the dataGrid");
        }
    }

    protected internal override void UpdateUI(CurrencyManager source, int rowNum, string displayText)
    {
        edit.Text = GetText(GetColumnValueAtRow(source, rowNum));
        if (!edit.ReadOnly && displayText is not null)
            edit.Text = displayText;
    }

    protected void EndEdit()
    {
        edit.IsInEditOrNavigateMode = true;
        DebugOut("Ending Edit");
        Invalidate();
    }

    protected internal override Size GetPreferredSize(Graphics g, object value)
    {
        Size extents = Size.Ceiling(g.MeasureString(GetText(value), DataGridTableStyle.DataGrid.Font));
        extents.Width += xMargin * 2 + this.DataGridTableStyle.GridLineWidth;
        extents.Height += yMargin;
        return extents;
    }

    protected internal override int GetMinimumHeight()
    {
        // why + 3? cause we have to give some way to the edit box.
        return FontHeight + yMargin + 3;
    }

    protected internal override int GetPreferredHeight(Graphics g, object value)
    {
        int newLineIndex = 0;
        int newLines = 0;
        string valueString = GetText(value);
        while (newLineIndex != -1 && newLineIndex < valueString.Length)
        {
            newLineIndex = valueString.IndexOf("\r\n", newLineIndex + 1);
            newLines++;
        }

        return FontHeight * newLines + yMargin;
    }

    protected internal override void Abort(int rowNum)
    {
        RollBack();
        HideEditBox();
        EndEdit();
    }

    protected internal override void EnterNullValue()
    {
        if (ReadOnly)
            return;

        // if the edit box is not visible, then
        // do not put the edit text in it
        if (!edit.Visible)
            return;

        // if we are editing, then we should be able to enter alt-0 in a cell.
        //
        if (!edit.IsInEditOrNavigateMode)
            return;

        edit.Text = NullText;
        // edit.Visible = true;
        edit.IsInEditOrNavigateMode = false;
        // tell the dataGrid that there is an edit:
        if (DataGridTableStyle is not null && DataGridTableStyle.DataGrid is not null)
            DataGridTableStyle.DataGrid.ColumnStartedEditing(edit.Bounds);
    }

    protected internal override bool Commit(CurrencyManager dataSource, int rowNum)
    {
        // always hide the edit box
        // HideEditBox();
        edit.Bounds = Rectangle.Empty;

        if (edit.IsInEditOrNavigateMode)
            return true;

        try
        {
            object value = edit.Text;
            if (NullText.Equals(value))
            {
                value = Convert.DBNull;
                edit.Text = NullText;
            }
            else if (FormatInfo is not null)
            {
                edit.Text = value.ToString();
            }

            SetColumnValueAtRow(dataSource, rowNum, value);
        }
        catch
        {
            RollBack();
            return false;
        }

        DebugOut("OnCommit completed without Exception.");
        EndEdit();
        return true;
    }

    protected internal override void Edit(CurrencyManager source,
                                int rowNum,
                                Rectangle bounds,
                                bool readOnly,
                                string displayText,
                                bool cellIsVisible)
    {
        DebugOut("Begining Edit, rowNum :" + rowNum.ToString(CultureInfo.InvariantCulture));

        Rectangle originalBounds = bounds;

        edit.ReadOnly = readOnly || ReadOnly || DataGridTableStyle.ReadOnly;

        edit.Text = GetText(GetColumnValueAtRow(source, rowNum));
        if (!edit.ReadOnly && displayText is not null)
        {
            // tell the grid that we are changing stuff
            DataGridTableStyle.DataGrid.ColumnStartedEditing(bounds);
            // tell the edit control that the user changed it
            edit.IsInEditOrNavigateMode = false;
            edit.Text = displayText;
        }

        if (cellIsVisible)
        {
            bounds.Offset(xMargin, 2 * yMargin);
            bounds.Width -= xMargin;
            bounds.Height -= 2 * yMargin;
            DebugOut("edit bounds: " + bounds.ToString());
            edit.Bounds = bounds;

            edit.Visible = true;

            edit.TextAlign = Alignment;
        }
        else
        {
            edit.Bounds = Rectangle.Empty;
            // edit.Bounds = originalBounds;
            // edit.Visible = false;
        }

        edit.RightToLeft = this.DataGridTableStyle.DataGrid.RightToLeft;

        editRow = rowNum;

        if (!edit.ReadOnly)
        {
            oldValue = edit.Text;
        }

        // select the text even if the text box is read only
        // because the navigation code in the DataGridTextBox::ProcessKeyMessage
        // uses the SelectedText property
        if (displayText is null)
            edit.SelectAll();
        else
        {
            int end = edit.Text.Length;
            edit.Select(end, 0);
        }

        if (edit.Visible)
            DataGridTableStyle.DataGrid.Invalidate(originalBounds);
    }

    internal override string GetDisplayText(object value)
    {
        return GetText(value);
    }

    private string GetText(object value)
    {
        if (value is DBNull)
            return NullText;
        else if (value is IFormattable)
        {
            try
            {
                return ((IFormattable)value).ToString();
            }
            catch
            {
                // CONSIDER: maybe we should fall back to using the typeConverter?
            }
        }

        return (value is not null ? value.ToString() : "");
    }

    protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight)
    {
        string text = GetText(GetColumnValueAtRow(source, rowNum));
        PaintText(g, bounds, text, alignToRight);
    }

    protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum,
                                 Brush backBrush, Brush foreBrush, bool alignToRight)
    {
        string text = GetText(GetColumnValueAtRow(source, rowNum));
        PaintText(g, bounds, text, backBrush, foreBrush, alignToRight);
    }

    protected void PaintText(Graphics g, Rectangle bounds, string text, bool alignToRight)
    {
        PaintText(g, bounds, text, DataGridTableStyle.BackBrush, DataGridTableStyle.ForeBrush, alignToRight);
    }

    protected void PaintText(Graphics g, Rectangle textBounds, string text, Brush backBrush, Brush foreBrush, bool alignToRight)
    {
        Rectangle rect = textBounds;

        StringFormat format = new StringFormat();
        if (alignToRight)
        {
            format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
        }

        format.Alignment = Alignment == HorizontalAlignment.Left ? StringAlignment.Near : Alignment == HorizontalAlignment.Center ? StringAlignment.Center : StringAlignment.Far;

        // do not wrap the text
        //
        format.FormatFlags |= StringFormatFlags.NoWrap;

        g.FillRectangle(backBrush, rect);
        // by design, painting  leaves a little padding around the rectangle.
        // so do not deflate the rectangle.
        rect.Offset(0, 2 * yMargin);
        rect.Height -= 2 * yMargin;
        g.DrawString(text, DataGridTableStyle.DataGrid.Font, foreBrush, rect, format);
        format.Dispose();
    }

    private void RollBack()
    {
        Debug.Assert(!edit.IsInEditOrNavigateMode, "Must be editing to rollback changes...");
        edit.Text = oldValue;
    }

    protected internal override void ReleaseHostedControl()
    {
        if (edit.ParentInternal is not null)
        {
            edit.ParentInternal.Controls.Remove(edit);
        }
    }

    protected internal override void Paint(Graphics g1, Graphics g, Rectangle bounds, CurrencyManager source, int rowNum) => throw new NotImplementedException();
}

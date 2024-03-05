// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

#pragma warning disable RS0016
#nullable disable
[Obsolete("DataGridBoolColumn has been deprecated.")]
public class DataGridBoolColumn : DataGridColumnStyle
{
    private const int idealCheckSize = 14;

    private bool isEditing;
    private bool isSelected;
    private int editingRow = -1;
    private object currentValue = Convert.DBNull;

    private object trueValue = true;
    private object falseValue = false;
    private object nullValue = Convert.DBNull;

    private static readonly object EventTrueValue = new object();
    private static readonly object EventFalseValue = new object();
    private static readonly object EventAllowNull = new object();

    public DataGridBoolColumn() : base()
    {
        throw new PlatformNotSupportedException();
    }

    public DataGridBoolColumn(PropertyDescriptor prop)
        : base(prop)
    {
        throw new PlatformNotSupportedException();
    }

    public DataGridBoolColumn(PropertyDescriptor prop, bool isDefault)
        : base(prop, isDefault)
    {
        throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public object TrueValue
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
    public event EventHandler TrueValueChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public object FalseValue
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
    public event EventHandler FalseValueChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public object NullValue
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

    protected internal override void ConcedeFocus()
    {
        base.ConcedeFocus();
        isSelected = false;
        isEditing = false;
    }

    private Rectangle GetCheckBoxBounds(Rectangle bounds, bool alignToRight)
    {
        if (alignToRight)
            return new Rectangle(bounds.X + ((bounds.Width - idealCheckSize) / 2),
                                 bounds.Y + ((bounds.Height - idealCheckSize) / 2),
                                 bounds.Width < idealCheckSize ? bounds.Width : idealCheckSize,
                                 idealCheckSize);
        else
            return new Rectangle(Math.Max(0, bounds.X + ((bounds.Width - idealCheckSize) / 2)),
                                 Math.Max(0, bounds.Y + ((bounds.Height - idealCheckSize) / 2)),
                                 bounds.Width < idealCheckSize ? bounds.Width : idealCheckSize,
                                 idealCheckSize);
    }

    protected internal override object GetColumnValueAtRow(CurrencyManager lm, int row)
    {
        object baseValue = base.GetColumnValueAtRow(lm, row);
        object value = Convert.DBNull;
        if (baseValue.Equals(trueValue))
        {
            value = true;
        }
        else if (baseValue.Equals(falseValue))
        {
            value = false;
        }

        return value;
    }

    private bool IsReadOnly()
    {
        bool ret = ReadOnly;
        if (DataGridTableStyle is not null)
        {
            ret = ret || DataGridTableStyle.ReadOnly;
            if (DataGridTableStyle.DataGrid is not null)
                ret = ret || DataGridTableStyle.DataGrid.ReadOnly;
        }

        return ret;
    }

    protected internal override void SetColumnValueAtRow(CurrencyManager lm, int row, object value)
    {
        object baseValue = null;
        if (true.Equals(value))
        {
            baseValue = TrueValue;
        }
        else if (false.Equals(value))
        {
            baseValue = FalseValue;
        }
        else if (Convert.IsDBNull(value))
        {
            baseValue = NullValue;
        }

        currentValue = baseValue;
        base.SetColumnValueAtRow(lm, row, baseValue);
    }

    protected internal override Size GetPreferredSize(Graphics g, object value)
    {
        return new Size(idealCheckSize + 2, idealCheckSize + 2);
    }

    protected internal override int GetMinimumHeight()
    {
        return idealCheckSize + 2;
    }

    protected internal override int GetPreferredHeight(Graphics g, object value)
    {
        return idealCheckSize + 2;
    }

    protected internal override void Abort(int rowNum)
    {
        isSelected = false;
        isEditing = false;
        Invalidate();
        return;
    }

    protected internal override bool Commit(CurrencyManager dataSource, int rowNum)
    {
        isSelected = false;
        // always invalidate
        Invalidate();
        if (!isEditing)
            return true;

        SetColumnValueAtRow(dataSource, rowNum, currentValue);
        isEditing = false;
        return true;
    }

    protected internal override void Edit(CurrencyManager source,
                                int rowNum,
                                Rectangle bounds,
                                bool readOnly,
                                string displayText,
                                bool cellIsVisible)
    {
        // toggle state right now...
        isSelected = true;

        // move the focus away from the previous column and give it to the grid
        DataGrid grid = DataGridTableStyle.DataGrid;

        if (!readOnly && !IsReadOnly())
        {
            editingRow = rowNum;
            currentValue = GetColumnValueAtRow(source, rowNum);
        }

        base.Invalidate();
    }

    internal override bool KeyPress(int rowNum, Keys keyData)
    {
        if (isSelected && editingRow == rowNum && !IsReadOnly())
        {
            if ((keyData & Keys.KeyCode) == Keys.Space)
            {
                ToggleValue();
                Invalidate();
                return true;
            }
        }

        return base.KeyPress(rowNum, keyData);
    }

    internal override bool MouseDown(int rowNum, int x, int y)
    {
        base.MouseDown(rowNum, x, y);
        if (isSelected && editingRow == rowNum && !IsReadOnly())
        {
            ToggleValue();
            Invalidate();
            return true;
        }

        return false;
    }

    private void OnTrueValueChanged(EventArgs e)
    {
        EventHandler eh = Events[EventTrueValue] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    private void OnFalseValueChanged(EventArgs e)
    {
        EventHandler eh = Events[EventFalseValue] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    private void OnAllowNullChanged(EventArgs e)
    {
        EventHandler eh = Events[EventAllowNull] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight)
    {
        Paint(g, bounds, source, rowNum, DataGridTableStyle.BackBrush, DataGridTableStyle.ForeBrush, alignToRight);
    }

    protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum,
                                 Brush backBrush, Brush foreBrush,
                                 bool alignToRight)
    {
        object value = (isEditing && editingRow == rowNum) ? currentValue : GetColumnValueAtRow(source, rowNum);
        ButtonState checkedState = ButtonState.Inactive;
        if (!Convert.IsDBNull(value))
        {
            checkedState = ((bool)value ? ButtonState.Checked : ButtonState.Normal);
        }

        Rectangle box = GetCheckBoxBounds(bounds, alignToRight);

        Region r = g.Clip;
        g.ExcludeClip(box);

        Brush selectionBrush = DataGridTableStyle.IsDefault ? DataGridTableStyle.DataGrid.SelectionBackBrush : DataGridTableStyle.SelectionBackBrush;
        if (isSelected && editingRow == rowNum && !IsReadOnly())
        {
            g.FillRectangle(selectionBrush, bounds);
        }
        else
            g.FillRectangle(backBrush, bounds);
        g.Clip = r;

        if (checkedState == ButtonState.Inactive)
        {
            ControlPaint.DrawMixedCheckBox(g, box, ButtonState.Checked);
        }
        else
        {
            ControlPaint.DrawCheckBox(g, box, checkedState);
        }

        // if the column is read only we should still show selection
        if (IsReadOnly() && isSelected && source.Position == rowNum)
        {
            bounds.Inflate(-1, -1);
            Pen pen = new Pen(selectionBrush);
            pen.DashStyle = Drawing.Drawing2D.DashStyle.Dash;
            g.DrawRectangle(pen, bounds);
            pen.Dispose();
            // restore the bounds rectangle
            bounds.Inflate(1, 1);
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool AllowNull
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
    public event EventHandler AllowNullChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    protected internal override void EnterNullValue()
    {
        // do not throw an exception when the column is marked as readOnly or
        // does not allowNull
        if (!AllowNull || IsReadOnly())
            return;
        if (currentValue is not DBNull)
        {
            currentValue = Convert.DBNull;
            Invalidate();
        }
    }

    private void ResetNullValue()
    {
        NullValue = Convert.DBNull;
    }

    private bool ShouldSerializeNullValue()
    {
        return nullValue is not DBNull;
    }

    private void ToggleValue()
    {
        if (currentValue is bool && ((bool)currentValue) == false)
        {
            currentValue = true;
        }
        else
        {
            if (AllowNull)
            {
                if (Convert.IsDBNull(currentValue))
                {
                    currentValue = false;
                }
                else
                {
                    currentValue = Convert.DBNull;
                }
            }
            else
            {
                currentValue = false;
            }
        }

        // we started editing
        isEditing = true;
        // tell the dataGrid that things are changing
        // we put Rectangle.Empty cause toggle will invalidate the row anyhow
        DataGridTableStyle.DataGrid.ColumnStartedEditing(Rectangle.Empty);
    }

    protected internal override void Paint(Graphics g1, Graphics g, Rectangle bounds, CurrencyManager source, int rowNum) => throw new NotImplementedException();
}

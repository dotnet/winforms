// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Windows.Forms;

public partial class DataGridViewComboBoxEditingControl : ComboBox, IDataGridViewEditingControl
{
    private DataGridView? _dataGridView;
    private bool _valueChanged;
    private int _rowIndex;

    public DataGridViewComboBoxEditingControl() : base()
    {
        TabStop = false;
    }

    protected override AccessibleObject CreateAccessibilityInstance()
    {
        DataGridViewComboBoxEditingControlAccessibleObject controlAccessibleObject = new(this);
        _dataGridView?.SetAccessibleObjectParent(controlAccessibleObject);
        return controlAccessibleObject;
    }

    public virtual DataGridView? EditingControlDataGridView
    {
        get
        {
            return _dataGridView;
        }
        set
        {
            _dataGridView = value;
        }
    }

    [AllowNull]
    public virtual object EditingControlFormattedValue
    {
        get => GetEditingControlFormattedValue(DataGridViewDataErrorContexts.Formatting);
        set
        {
            if (value is string valueStr)
            {
                Text = valueStr;
                if (string.Compare(valueStr, Text, true, CultureInfo.CurrentCulture) != 0)
                {
                    SelectedIndex = -1;
                }
            }
        }
    }

    public virtual int EditingControlRowIndex
    {
        get => _rowIndex;
        set => _rowIndex = value;
    }

    public virtual bool EditingControlValueChanged
    {
        get => _valueChanged;
        set => _valueChanged = value;
    }

    public virtual Cursor EditingPanelCursor => Cursors.Default;

    public virtual bool RepositionEditingControlOnValueChange => false;

    public virtual void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
    {
        Font = dataGridViewCellStyle.Font;
        if (dataGridViewCellStyle.BackColor.A < 255)
        {
            // Our ComboBox does not support transparent back colors
            Color opaqueBackColor = Color.FromArgb(255, dataGridViewCellStyle.BackColor);
            BackColor = opaqueBackColor;
            _dataGridView!.EditingPanel.BackColor = opaqueBackColor;
        }
        else
        {
            BackColor = dataGridViewCellStyle.BackColor;
        }

        ForeColor = dataGridViewCellStyle.ForeColor;
    }

    public virtual bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
    {
        var maskedKeyData = keyData & Keys.KeyCode;
        if (maskedKeyData == Keys.Down ||
            maskedKeyData == Keys.Up ||
            (DroppedDown && (maskedKeyData == Keys.Escape)) ||
            maskedKeyData == Keys.Enter)
        {
            return true;
        }

        return !dataGridViewWantsInputKey;
    }

    public virtual object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => Text;

    public virtual void PrepareEditingControlForEdit(bool selectAll)
    {
        if (selectAll)
        {
            SelectAll();
        }
    }

    private void NotifyDataGridViewOfValueChange()
    {
        _valueChanged = true;
        _dataGridView!.NotifyCurrentCellDirty(true);
    }

    protected override void OnSelectedIndexChanged(EventArgs e)
    {
        base.OnSelectedIndexChanged(e);
        if (SelectedIndex != -1)
        {
            NotifyDataGridViewOfValueChange();
        }
    }

    internal override void ReleaseUiaProvider(HWND handle)
    {
        if (TryGetAccessibilityObject(out AccessibleObject? accessibleObject))
        {
            ((DataGridViewComboBoxEditingControlAccessibleObject)accessibleObject).ClearParent();
        }

        base.ReleaseUiaProvider(handle);
    }
}

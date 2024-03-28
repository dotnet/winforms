// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[ToolboxBitmap(typeof(DataGridViewCheckBoxColumn), "DataGridViewCheckBoxColumn")]
public class DataGridViewCheckBoxColumn : DataGridViewColumn
{
    public DataGridViewCheckBoxColumn()
        : this(threeState: false)
    {
    }

    public DataGridViewCheckBoxColumn(bool threeState)
        : base(new DataGridViewCheckBoxCell(threeState))
    {
        DataGridViewCellStyle defaultCellStyle = new DataGridViewCellStyle
        {
            AlignmentInternal = DataGridViewContentAlignment.MiddleCenter,
            NullValue = threeState ? CheckState.Indeterminate : false
        };

        DefaultCellStyle = defaultCellStyle;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override DataGridViewCell? CellTemplate
    {
        get => base.CellTemplate;
        set
        {
            if (value is not null and not DataGridViewCheckBoxCell)
            {
                throw new InvalidCastException(string.Format(SR.DataGridViewTypeColumn_WrongCellTemplateType, "System.Windows.Forms.DataGridViewCheckBoxCell"));
            }

            base.CellTemplate = value;
        }
    }

    private DataGridViewCheckBoxCell? CheckBoxCellTemplate => (DataGridViewCheckBoxCell?)CellTemplate;

    [Browsable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_ColumnDefaultCellStyleDescr))]
    [AllowNull]
    public override DataGridViewCellStyle DefaultCellStyle
    {
        get => base.DefaultCellStyle;
        set => base.DefaultCellStyle = value;
    }

    [DefaultValue(null)]
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.DataGridView_CheckBoxColumnFalseValueDescr))]
    [TypeConverter(typeof(StringConverter))]
    [MemberNotNull(nameof(CheckBoxCellTemplate))]
    public object? FalseValue
    {
        get
        {
            if (CheckBoxCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return CheckBoxCellTemplate.FalseValue;
        }
        set
        {
            if (FalseValue == value)
            {
                return;
            }

            CheckBoxCellTemplate.FalseValueInternal = value;
            if (DataGridView is null)
            {
                return;
            }

            DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
            int rowCount = dataGridViewRows.Count;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is DataGridViewCheckBoxCell dataGridViewCell)
                {
                    dataGridViewCell.FalseValueInternal = value;
                }
            }

            DataGridView.InvalidateColumn(Index);
        }
    }

    [DefaultValue(FlatStyle.Standard)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_CheckBoxColumnFlatStyleDescr))]
    [MemberNotNull(nameof(CheckBoxCellTemplate))]
    public FlatStyle FlatStyle
    {
        get
        {
            if (CheckBoxCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return CheckBoxCellTemplate.FlatStyle;
        }
        set
        {
            if (FlatStyle == value)
            {
                return;
            }

            CheckBoxCellTemplate.FlatStyle = value;
            if (DataGridView is null)
            {
                return;
            }

            DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
            int rowCount = dataGridViewRows.Count;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is DataGridViewCheckBoxCell dataGridViewCell)
                {
                    dataGridViewCell.FlatStyleInternal = value;
                }
            }

            DataGridView.OnColumnCommonChange(Index);
        }
    }

    [DefaultValue(null)]
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.DataGridView_CheckBoxColumnIndeterminateValueDescr))]
    [TypeConverter(typeof(StringConverter))]
    [MemberNotNull(nameof(CheckBoxCellTemplate))]
    public object? IndeterminateValue
    {
        get
        {
            if (CheckBoxCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return CheckBoxCellTemplate.IndeterminateValue;
        }
        set
        {
            if (IndeterminateValue == value)
            {
                return;
            }

            CheckBoxCellTemplate.IndeterminateValueInternal = value;
            if (DataGridView is null)
            {
                return;
            }

            DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
            int rowCount = dataGridViewRows.Count;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is DataGridViewCheckBoxCell dataGridViewCell)
                {
                    dataGridViewCell.IndeterminateValueInternal = value;
                }
            }

            DataGridView.InvalidateColumn(Index);
        }
    }

    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_CheckBoxColumnThreeStateDescr))]
    [MemberNotNull(nameof(CheckBoxCellTemplate))]
    public bool ThreeState
    {
        get
        {
            if (CheckBoxCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return CheckBoxCellTemplate.ThreeState;
        }
        set
        {
            if (ThreeState == value)
            {
                return;
            }

            CheckBoxCellTemplate.ThreeStateInternal = value;
            if (DataGridView is not null)
            {
                DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
                int rowCount = dataGridViewRows.Count;
                for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                {
                    DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                    if (dataGridViewRow.Cells[Index] is DataGridViewCheckBoxCell dataGridViewCell)
                    {
                        dataGridViewCell.ThreeStateInternal = value;
                    }
                }

                DataGridView.InvalidateColumn(Index);
            }

            if (value && DefaultCellStyle.NullValue is bool boolValue && !boolValue)
            {
                DefaultCellStyle.NullValue = CheckState.Indeterminate;
            }
            else if (!value && DefaultCellStyle.NullValue is CheckState state && state == CheckState.Indeterminate)
            {
                DefaultCellStyle.NullValue = false;
            }
        }
    }

    [DefaultValue(null)]
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.DataGridView_CheckBoxColumnTrueValueDescr))]
    [TypeConverter(typeof(StringConverter))]
    [MemberNotNull(nameof(CheckBoxCellTemplate))]
    public object? TrueValue
    {
        get
        {
            if (CheckBoxCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return CheckBoxCellTemplate.TrueValue;
        }
        set
        {
            if (TrueValue == value)
            {
                return;
            }

            CheckBoxCellTemplate.TrueValueInternal = value;
            if (DataGridView is null)
            {
                return;
            }

            DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
            int rowCount = dataGridViewRows.Count;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is DataGridViewCheckBoxCell dataGridViewCell)
                {
                    dataGridViewCell.TrueValueInternal = value;
                }
            }

            DataGridView.InvalidateColumn(Index);
        }
    }

    private bool ShouldSerializeDefaultCellStyle()
    {
        object defaultNullValue;
        if (CellTemplate is not DataGridViewCheckBoxCell templateCell)
        {
            Debug.Fail("we can't compute the default cell style w/o a template cell");
            return true;
        }

        defaultNullValue = templateCell.ThreeState ? CheckState.Indeterminate : false;

        if (!HasDefaultCellStyle)
        {
            return false;
        }

        DataGridViewCellStyle defaultCellStyle = DefaultCellStyle;

        return (!defaultCellStyle.BackColor.IsEmpty ||
                !defaultCellStyle.ForeColor.IsEmpty ||
                !defaultCellStyle.SelectionBackColor.IsEmpty ||
                !defaultCellStyle.SelectionForeColor.IsEmpty ||
                defaultCellStyle.Font is not null ||
                !defaultNullValue.Equals(defaultCellStyle.NullValue) ||
                !defaultCellStyle.IsDataSourceNullValueDefault ||
                !string.IsNullOrEmpty(defaultCellStyle.Format) ||
                !defaultCellStyle.FormatProvider.Equals(Globalization.CultureInfo.CurrentCulture) ||
                defaultCellStyle.Alignment != DataGridViewContentAlignment.MiddleCenter ||
                defaultCellStyle.WrapMode != DataGridViewTriState.NotSet ||
                defaultCellStyle.Tag is not null ||
                !defaultCellStyle.Padding.Equals(Padding.Empty));
    }

    public override string ToString() =>
        $"DataGridViewCheckBoxColumn {{ Name={Name}, Index={Index} }}";
}

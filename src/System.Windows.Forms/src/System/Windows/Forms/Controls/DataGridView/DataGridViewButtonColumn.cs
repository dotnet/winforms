// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
[ToolboxBitmap(typeof(DataGridViewButtonColumn), "DataGridViewButtonColumn")]
public class DataGridViewButtonColumn : DataGridViewColumn
{
    private static readonly Type s_columnType = typeof(DataGridViewButtonColumn);
    private string? _text;

    public DataGridViewButtonColumn()
        : base(new DataGridViewButtonCell())
    {
        DataGridViewCellStyle defaultCellStyle = new DataGridViewCellStyle
        {
            AlignmentInternal = DataGridViewContentAlignment.MiddleCenter
        };
        DefaultCellStyle = defaultCellStyle;
    }

    private DataGridViewButtonCell? ButtonCellTemplate => (DataGridViewButtonCell?)CellTemplate;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override DataGridViewCell? CellTemplate
    {
        get => base.CellTemplate;
        set
        {
            if (value is not null and not DataGridViewButtonCell)
            {
                throw new InvalidCastException(string.Format(SR.DataGridViewTypeColumn_WrongCellTemplateType, "System.Windows.Forms.DataGridViewButtonCell"));
            }

            base.CellTemplate = value;
        }
    }

    [Browsable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_ColumnDefaultCellStyleDescr))]
    [AllowNull]
    public override DataGridViewCellStyle DefaultCellStyle
    {
        get => base.DefaultCellStyle;
        set => base.DefaultCellStyle = value;
    }

    [DefaultValue(FlatStyle.Standard)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_ButtonColumnFlatStyleDescr))]
    [MemberNotNull(nameof(ButtonCellTemplate))]
    public FlatStyle FlatStyle
    {
        get
        {
            if (ButtonCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return ButtonCellTemplate.FlatStyle;
        }
        set
        {
            if (FlatStyle == value)
            {
                return;
            }

            ButtonCellTemplate.FlatStyle = value;
            if (DataGridView is null)
            {
                return;
            }

            DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
            int rowCount = dataGridViewRows.Count;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is DataGridViewButtonCell dataGridViewCell)
                {
                    dataGridViewCell.FlatStyleInternal = value;
                }
            }

            DataGridView.OnColumnCommonChange(Index);
        }
    }

    [DefaultValue(null)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_ButtonColumnTextDescr))]
    public string? Text
    {
        get
        {
            return _text;
        }
        set
        {
            if (string.Equals(value, _text, StringComparison.Ordinal))
            {
                return;
            }

            _text = value;
            if (DataGridView is null)
            {
                return;
            }

            if (UseColumnTextForButtonValue)
            {
                DataGridView.OnColumnCommonChange(Index);
            }
            else
            {
                DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
                int rowCount = dataGridViewRows.Count;
                for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                {
                    DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                    if (dataGridViewRow.Cells[Index] is DataGridViewButtonCell dataGridViewCell && dataGridViewCell.UseColumnTextForButtonValue)
                    {
                        DataGridView.OnColumnCommonChange(Index);
                        return;
                    }
                }

                DataGridView.InvalidateColumn(Index);
            }
        }
    }

    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_ButtonColumnUseColumnTextForButtonValueDescr))]
    [MemberNotNull(nameof(ButtonCellTemplate))]
    public bool UseColumnTextForButtonValue
    {
        get
        {
            if (ButtonCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return ButtonCellTemplate.UseColumnTextForButtonValue;
        }
        set
        {
            if (UseColumnTextForButtonValue == value)
            {
                return;
            }

            ButtonCellTemplate.UseColumnTextForButtonValueInternal = value;
            if (DataGridView is null)
            {
                return;
            }

            DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
            int rowCount = dataGridViewRows.Count;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is DataGridViewButtonCell dataGridViewCell)
                {
                    dataGridViewCell.UseColumnTextForButtonValueInternal = value;
                }
            }

            DataGridView.OnColumnCommonChange(Index);
        }
    }

    public override object Clone()
    {
        DataGridViewButtonColumn dataGridViewColumn;
        Type thisType = GetType();

        if (thisType == s_columnType) // performance improvement
        {
            dataGridViewColumn = new DataGridViewButtonColumn();
        }
        else
        {
            dataGridViewColumn = (DataGridViewButtonColumn)Activator.CreateInstance(thisType)!;
        }

        CloneInternal(dataGridViewColumn);
        dataGridViewColumn.Text = _text;

        return dataGridViewColumn;
    }

    private bool ShouldSerializeDefaultCellStyle()
    {
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
                !defaultCellStyle.IsNullValueDefault ||
                !defaultCellStyle.IsDataSourceNullValueDefault ||
                !string.IsNullOrEmpty(defaultCellStyle.Format) ||
                !defaultCellStyle.FormatProvider.Equals(Globalization.CultureInfo.CurrentCulture) ||
                defaultCellStyle.Alignment != DataGridViewContentAlignment.MiddleCenter ||
                defaultCellStyle.WrapMode != DataGridViewTriState.NotSet ||
                defaultCellStyle.Tag is not null ||
                !defaultCellStyle.Padding.Equals(Padding.Empty));
    }

    public override string ToString() =>
        $"DataGridViewButtonColumn {{ Name={Name}, Index={Index} }}";
}

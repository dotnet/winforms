// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms
{
    [ToolboxBitmap(typeof(DataGridViewButtonColumn), "DataGridViewButtonColumn")]
    public class DataGridViewButtonColumn : DataGridViewColumn
    {
        private static readonly Type columnType = typeof(DataGridViewButtonColumn);

        private string text;

        public DataGridViewButtonColumn()
            : base(new DataGridViewButtonCell())
        {
            DataGridViewCellStyle defaultCellStyle = new DataGridViewCellStyle
            {
                AlignmentInternal = DataGridViewContentAlignment.MiddleCenter
            };
            DefaultCellStyle = defaultCellStyle;
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                if (value != null && !(value is DataGridViewButtonCell))
                {
                    throw new InvalidCastException(string.Format(SR.DataGridViewTypeColumn_WrongCellTemplateType, "System.Windows.Forms.DataGridViewButtonCell"));
                }
                base.CellTemplate = value;
            }
        }

        [
            Browsable(true),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ColumnDefaultCellStyleDescr))
        ]
        public override DataGridViewCellStyle DefaultCellStyle
        {
            get
            {
                return base.DefaultCellStyle;
            }
            set
            {
                base.DefaultCellStyle = value;
            }
        }

        [
            DefaultValue(FlatStyle.Standard),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ButtonColumnFlatStyleDescr))
        ]
        public FlatStyle FlatStyle
        {
            get
            {
                if (CellTemplate == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                return ((DataGridViewButtonCell)CellTemplate).FlatStyle;
            }
            set
            {
                if (FlatStyle != value)
                {
                    ((DataGridViewButtonCell)CellTemplate).FlatStyle = value;
                    if (DataGridView != null)
                    {
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
            }
        }

        [
            DefaultValue(null),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ButtonColumnTextDescr))
        ]
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (!string.Equals(value, text, StringComparison.Ordinal))
                {
                    text = value;
                    if (DataGridView != null)
                    {
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
            }
        }

        [
            DefaultValue(false),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ButtonColumnUseColumnTextForButtonValueDescr))
        ]
        public bool UseColumnTextForButtonValue
        {
            get
            {
                if (CellTemplate == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                return ((DataGridViewButtonCell)CellTemplate).UseColumnTextForButtonValue;
            }
            set
            {
                if (UseColumnTextForButtonValue != value)
                {
                    ((DataGridViewButtonCell)CellTemplate).UseColumnTextForButtonValueInternal = value;
                    if (DataGridView != null)
                    {
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
            }
        }

        public override object Clone()
        {
            DataGridViewButtonColumn dataGridViewColumn;
            Type thisType = GetType();

            if (thisType == columnType) //performance improvement
            {
                dataGridViewColumn = new DataGridViewButtonColumn();
            }
            else
            {
                dataGridViewColumn = (DataGridViewButtonColumn)System.Activator.CreateInstance(thisType);
            }
            if (dataGridViewColumn != null)
            {
                base.CloneInternal(dataGridViewColumn);
                dataGridViewColumn.Text = text;
            }
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
                    defaultCellStyle.Font != null ||
                    !defaultCellStyle.IsNullValueDefault ||
                    !defaultCellStyle.IsDataSourceNullValueDefault ||
                    !string.IsNullOrEmpty(defaultCellStyle.Format) ||
                    !defaultCellStyle.FormatProvider.Equals(System.Globalization.CultureInfo.CurrentCulture) ||
                    defaultCellStyle.Alignment != DataGridViewContentAlignment.MiddleCenter ||
                    defaultCellStyle.WrapMode != DataGridViewTriState.NotSet ||
                    defaultCellStyle.Tag != null ||
                    !defaultCellStyle.Padding.Equals(Padding.Empty));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append("DataGridViewButtonColumn { Name=");
            sb.Append(Name);
            sb.Append(", Index=");
            sb.Append(Index.ToString(CultureInfo.CurrentCulture));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}

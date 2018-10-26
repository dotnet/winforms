// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Text;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Diagnostics.CodeAnalysis;

    /// <include file='doc\DataGridViewButtonColumn.uex' path='docs/doc[@for="DataGridViewButtonColumn"]/*' />
    [ToolboxBitmapAttribute(typeof(DataGridViewButtonColumn), "DataGridViewButtonColumn.bmp")]
    public class DataGridViewButtonColumn : DataGridViewColumn
    {
        private static Type columnType = typeof(DataGridViewButtonColumn);

        private string text;

        /// <include file='doc\DataGridViewButtonColumn.uex' path='docs/doc[@for="DataGridViewButtonColumn.DataGridViewButtonColumn"]/*' />
        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors") // Can't think of a workaround.
        ]
        public DataGridViewButtonColumn()
            : base(new DataGridViewButtonCell())
        {
            DataGridViewCellStyle defaultCellStyle = new DataGridViewCellStyle();
            defaultCellStyle.AlignmentInternal = DataGridViewContentAlignment.MiddleCenter;
            this.DefaultCellStyle = defaultCellStyle;
        }

        /// <include file='doc\DataGridViewButtonColumn.uex' path='docs/doc[@for="DataGridViewButtonColumn.CellTemplate"]/*' />
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
                if (value != null && !(value is System.Windows.Forms.DataGridViewButtonCell))
                {
                    throw new InvalidCastException(string.Format(SR.DataGridViewTypeColumn_WrongCellTemplateType, "System.Windows.Forms.DataGridViewButtonCell"));
                }
                base.CellTemplate = value;
            }
        }

        /// <include file='doc\DataGridViewButtonColumn.uex' path='docs/doc[@for="DataGridViewButtonColumn.DefaultCellStyle"]/*' />
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

        /// <include file='doc\DataGridViewButtonColumn.uex' path='docs/doc[@for="DataGridViewButtonColumn.FlatStyle"]/*' />
        [
            DefaultValue(FlatStyle.Standard),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ButtonColumnFlatStyleDescr))
        ]
        public FlatStyle FlatStyle
        {
            get
            {
                if (this.CellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return ((DataGridViewButtonCell) this.CellTemplate).FlatStyle;
            }
            set
            {
                if (this.FlatStyle != value)
                {
                    ((DataGridViewButtonCell)this.CellTemplate).FlatStyle = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewButtonCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewButtonCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.FlatStyleInternal = value;
                            }
                        }
                        this.DataGridView.OnColumnCommonChange(this.Index);
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewButtonColumn.uex' path='docs/doc[@for="DataGridViewButtonColumn.Text"]/*' />
        [
            DefaultValue(null),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ButtonColumnTextDescr))
        ]
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                if (!string.Equals(value, this.text, StringComparison.Ordinal))
                {
                    this.text = value;
                    if (this.DataGridView != null)
                    {
                        if (this.UseColumnTextForButtonValue)
                        {
                            this.DataGridView.OnColumnCommonChange(this.Index);
                        }
                        else
                        {
                            DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                            int rowCount = dataGridViewRows.Count;
                            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                            {
                                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                                DataGridViewButtonCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewButtonCell;
                                if (dataGridViewCell != null && dataGridViewCell.UseColumnTextForButtonValue)
                                {
                                    this.DataGridView.OnColumnCommonChange(this.Index);
                                    return;
                                }
                            }
                            this.DataGridView.InvalidateColumn(this.Index);
                        }
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewButtonColumn.uex' path='docs/doc[@for="DataGridViewButtonColumn.UseColumnTextForButtonValue"]/*' />
        [
            DefaultValue(false),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ButtonColumnUseColumnTextForButtonValueDescr))
        ]
        public bool UseColumnTextForButtonValue
        {
            get
            {
                if (this.CellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return ((DataGridViewButtonCell)this.CellTemplate).UseColumnTextForButtonValue;
            }
            set
            {
                if (this.UseColumnTextForButtonValue != value)
                {
                    ((DataGridViewButtonCell)this.CellTemplate).UseColumnTextForButtonValueInternal = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewButtonCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewButtonCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.UseColumnTextForButtonValueInternal = value;
                            }
                        }
                        this.DataGridView.OnColumnCommonChange(this.Index);
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewButtonColumn.uex' path='docs/doc[@for="DataGridViewButtonColumn.Clone"]/*' />
        public override object Clone()
        {
            DataGridViewButtonColumn dataGridViewColumn;
            Type thisType = this.GetType();

            if (thisType == columnType) //performance improvement
            {
                dataGridViewColumn = new DataGridViewButtonColumn();
            }
            else
            {
                dataGridViewColumn = (DataGridViewButtonColumn) System.Activator.CreateInstance(thisType);
            }
            if (dataGridViewColumn != null)
            {
                base.CloneInternal(dataGridViewColumn);
                dataGridViewColumn.Text = this.text;
            }
            return dataGridViewColumn;
        }

        private bool ShouldSerializeDefaultCellStyle()
        {
            if (!this.HasDefaultCellStyle)
            {
                return false;
            }

            DataGridViewCellStyle defaultCellStyle = this.DefaultCellStyle;

            return (!defaultCellStyle.BackColor.IsEmpty || 
                    !defaultCellStyle.ForeColor.IsEmpty ||
                    !defaultCellStyle.SelectionBackColor.IsEmpty || 
                    !defaultCellStyle.SelectionForeColor.IsEmpty ||
                    defaultCellStyle.Font != null ||
                    !defaultCellStyle.IsNullValueDefault ||
                    !defaultCellStyle.IsDataSourceNullValueDefault ||
                    !String.IsNullOrEmpty(defaultCellStyle.Format) ||
                    !defaultCellStyle.FormatProvider.Equals(System.Globalization.CultureInfo.CurrentCulture) ||
                    defaultCellStyle.Alignment != DataGridViewContentAlignment.MiddleCenter ||
                    defaultCellStyle.WrapMode != DataGridViewTriState.NotSet ||
                    defaultCellStyle.Tag !=  null ||
                    !defaultCellStyle.Padding.Equals(Padding.Empty));
        }

        /// <include file='doc\DataGridViewButtonColumn.uex' path='docs/doc[@for="DataGridViewButtonColumn.ToString"]/*' />
        public override string ToString() 
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append("DataGridViewButtonColumn { Name=");
            sb.Append(this.Name);
            sb.Append(", Index=");
            sb.Append(this.Index.ToString(CultureInfo.CurrentCulture));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}

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
    
    /// <include file='doc\DataGridViewTextBoxColumn.uex' path='docs/doc[@for="DataGridViewTextBoxColumn"]/*' />
    [ToolboxBitmapAttribute(typeof(DataGridViewTextBoxColumn), "DataGridViewTextBoxColumn.bmp")]
    public class DataGridViewTextBoxColumn : DataGridViewColumn
    {
        private const int DATAGRIDVIEWTEXTBOXCOLUMN_maxInputLength = 32767;

        /// <include file='doc\DataGridViewTextBoxColumn.uex' path='docs/doc[@for="DataGridViewTextBoxColumn.DataGridViewTextBoxColumn"]/*' />
        public DataGridViewTextBoxColumn() : base(new DataGridViewTextBoxCell())
        {
            this.SortMode = DataGridViewColumnSortMode.Automatic;
        }

        /// <include file='doc\DataGridViewTextBoxColumn.uex' path='docs/doc[@for="DataGridViewTextBoxColumn.CellTemplate"]/*' />
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
                if (value != null && !(value is System.Windows.Forms.DataGridViewTextBoxCell))
                {
                    throw new InvalidCastException(string.Format(SR.DataGridViewTypeColumn_WrongCellTemplateType, "System.Windows.Forms.DataGridViewTextBoxCell"));
                }
                base.CellTemplate = value;
            }
        }

        /// <include file='doc\DataGridViewTextBoxColumn.uex' path='docs/doc[@for="DataGridViewTextBoxColumn.MaxInputLength"]/*' />
        [
            DefaultValue(DATAGRIDVIEWTEXTBOXCOLUMN_maxInputLength),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_TextBoxColumnMaxInputLengthDescr))
        ]
        public int MaxInputLength
        {
            get
            {
                if (this.TextBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return this.TextBoxCellTemplate.MaxInputLength;
            }
            set
            {
                if (this.MaxInputLength != value)
                {
                    this.TextBoxCellTemplate.MaxInputLength = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewTextBoxCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewTextBoxCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.MaxInputLength = value;
                            }
                        }
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewTextBoxColumn.uex' path='docs/doc[@for="DataGridViewTextBoxColumn.SortMode"]/*' />
        [
            DefaultValue(DataGridViewColumnSortMode.Automatic)
        ]
        public new DataGridViewColumnSortMode SortMode
        {
            get
            {
                return base.SortMode;
            }
            set
            {
                base.SortMode = value;
            }
        }

        private DataGridViewTextBoxCell TextBoxCellTemplate
        {
            get
            {
                return (DataGridViewTextBoxCell) this.CellTemplate;
            }
        }

        /// <include file='doc\DataGridViewTextBoxColumn.uex' path='docs/doc[@for="DataGridViewTextBoxColumn.ToString"]/*' />
        public override string ToString() 
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append("DataGridViewTextBoxColumn { Name=");
            sb.Append(this.Name);
            sb.Append(", Index=");
            sb.Append(this.Index.ToString(CultureInfo.CurrentCulture));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}

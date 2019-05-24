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

    [ToolboxBitmapAttribute(typeof(DataGridViewTextBoxColumn), "DataGridViewTextBoxColumn")]
    public class DataGridViewTextBoxColumn : DataGridViewColumn
    {
        private const int DATAGRIDVIEWTEXTBOXCOLUMN_maxInputLength = 32767;

        public DataGridViewTextBoxColumn() : base(new DataGridViewTextBoxCell())
        {
            SortMode = DataGridViewColumnSortMode.Automatic;
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
                if (value != null && !(value is System.Windows.Forms.DataGridViewTextBoxCell))
                {
                    throw new InvalidCastException(string.Format(SR.DataGridViewTypeColumn_WrongCellTemplateType, "System.Windows.Forms.DataGridViewTextBoxCell"));
                }
                base.CellTemplate = value;
            }
        }

        [
            DefaultValue(DATAGRIDVIEWTEXTBOXCOLUMN_maxInputLength),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_TextBoxColumnMaxInputLengthDescr))
        ]
        public int MaxInputLength
        {
            get
            {
                if (TextBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return TextBoxCellTemplate.MaxInputLength;
            }
            set
            {
                if (MaxInputLength != value)
                {
                    TextBoxCellTemplate.MaxInputLength = value;
                    if (DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewTextBoxCell dataGridViewCell = dataGridViewRow.Cells[Index] as DataGridViewTextBoxCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.MaxInputLength = value;
                            }
                        }
                    }
                }
            }
        }

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
                return (DataGridViewTextBoxCell)CellTemplate;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append("DataGridViewTextBoxColumn { Name=");
            sb.Append(Name);
            sb.Append(", Index=");
            sb.Append(Index.ToString(CultureInfo.CurrentCulture));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}

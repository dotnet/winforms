// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace System.Windows.Forms
{
    [ToolboxBitmap(typeof(DataGridViewCheckBoxColumn), "DataGridViewCheckBoxColumn")]
    public class DataGridViewCheckBoxColumn : DataGridViewColumn
    {
        public DataGridViewCheckBoxColumn() : this(false)
        {
        }

        public DataGridViewCheckBoxColumn(bool threeState)
            : base(new DataGridViewCheckBoxCell(threeState))
        {
            DataGridViewCellStyle defaultCellStyle = new DataGridViewCellStyle
            {
                AlignmentInternal = DataGridViewContentAlignment.MiddleCenter
            };
            if (threeState)
            {
                defaultCellStyle.NullValue = CheckState.Indeterminate;
            }
            else
            {
                defaultCellStyle.NullValue = false;
            }
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
                if (value != null && !(value is DataGridViewCheckBoxCell))
                {
                    throw new InvalidCastException(string.Format(SR.DataGridViewTypeColumn_WrongCellTemplateType, "System.Windows.Forms.DataGridViewCheckBoxCell"));
                }
                base.CellTemplate = value;
            }
        }

        private DataGridViewCheckBoxCell CheckBoxCellTemplate
        {
            get
            {
                return (DataGridViewCheckBoxCell)CellTemplate;
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
            DefaultValue(null),
            SRCategory(nameof(SR.CatData)),
            SRDescription(nameof(SR.DataGridView_CheckBoxColumnFalseValueDescr)),
            TypeConverter(typeof(StringConverter))
        ]
        public object FalseValue
        {
            get
            {
                if (CheckBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                return CheckBoxCellTemplate.FalseValue;
            }
            set
            {
                if (FalseValue != value)
                {
                    CheckBoxCellTemplate.FalseValueInternal = value;
                    if (DataGridView != null)
                    {
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
            }
        }

        [
            DefaultValue(FlatStyle.Standard),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_CheckBoxColumnFlatStyleDescr))
        ]
        public FlatStyle FlatStyle
        {
            get
            {
                if (CheckBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                return CheckBoxCellTemplate.FlatStyle;
            }
            set
            {
                if (FlatStyle != value)
                {
                    CheckBoxCellTemplate.FlatStyle = value;
                    if (DataGridView != null)
                    {
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
            }
        }

        [
            DefaultValue(null),
            SRCategory(nameof(SR.CatData)),
            SRDescription(nameof(SR.DataGridView_CheckBoxColumnIndeterminateValueDescr)),
            TypeConverter(typeof(StringConverter))
        ]
        public object IndeterminateValue
        {
            get
            {
                if (CheckBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                return CheckBoxCellTemplate.IndeterminateValue;
            }
            set
            {
                if (IndeterminateValue != value)
                {
                    CheckBoxCellTemplate.IndeterminateValueInternal = value;
                    if (DataGridView != null)
                    {
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
            }
        }

        [
            DefaultValue(false),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_CheckBoxColumnThreeStateDescr))
        ]
        public bool ThreeState
        {
            get
            {
                if (CheckBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                return CheckBoxCellTemplate.ThreeState;
            }
            set
            {
                if (ThreeState != value)
                {
                    CheckBoxCellTemplate.ThreeStateInternal = value;
                    if (DataGridView != null)
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

                    if (value &&
                        DefaultCellStyle.NullValue is bool &&
                        (bool)DefaultCellStyle.NullValue == false)
                    {
                        DefaultCellStyle.NullValue = CheckState.Indeterminate;
                    }
                    else if (!value &&
                             DefaultCellStyle.NullValue is CheckState &&
                             (CheckState)DefaultCellStyle.NullValue == CheckState.Indeterminate)
                    {
                        DefaultCellStyle.NullValue = false;
                    }
                }
            }
        }

        [
            DefaultValue(null),
            SRCategory(nameof(SR.CatData)),
            SRDescription(nameof(SR.DataGridView_CheckBoxColumnTrueValueDescr)),
            TypeConverter(typeof(StringConverter))
        ]
        public object TrueValue
        {
            get
            {
                if (CheckBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                return CheckBoxCellTemplate.TrueValue;
            }
            set
            {
                if (TrueValue != value)
                {
                    CheckBoxCellTemplate.TrueValueInternal = value;
                    if (DataGridView != null)
                    {
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
            }
        }

        private bool ShouldSerializeDefaultCellStyle()
        {
            object defaultNullValue;
            if (!(CellTemplate is DataGridViewCheckBoxCell templateCell))
            {
                Debug.Fail("we can't compute the default cell style w/o a template cell");
                return true;
            }

            if (templateCell.ThreeState)
            {
                defaultNullValue = CheckState.Indeterminate;
            }
            else
            {
                defaultNullValue = false;
            }

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
                    !defaultCellStyle.NullValue.Equals(defaultNullValue) ||
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
            sb.Append("DataGridViewCheckBoxColumn { Name=");
            sb.Append(Name);
            sb.Append(", Index=");
            sb.Append(Index.ToString(CultureInfo.CurrentCulture));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}

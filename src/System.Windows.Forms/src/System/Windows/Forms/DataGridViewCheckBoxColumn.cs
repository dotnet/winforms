﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Text;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    
    [ToolboxBitmapAttribute(typeof(DataGridViewCheckBoxColumn), "DataGridViewCheckBoxColumn")]
    public class DataGridViewCheckBoxColumn : DataGridViewColumn
    {
        public DataGridViewCheckBoxColumn() : this(false)
        {
        }

        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors") // Can't think of a workaround.
        ]
        public DataGridViewCheckBoxColumn(bool threeState)
            : base(new DataGridViewCheckBoxCell(threeState))
        {
            DataGridViewCellStyle defaultCellStyle = new DataGridViewCellStyle();
            defaultCellStyle.AlignmentInternal = DataGridViewContentAlignment.MiddleCenter;
            if (threeState)
            {
                defaultCellStyle.NullValue = CheckState.Indeterminate;
            }
            else
            {
                defaultCellStyle.NullValue = false;
            }
            this.DefaultCellStyle = defaultCellStyle;
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
                if (value != null && !(value is System.Windows.Forms.DataGridViewCheckBoxCell))
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
                return (DataGridViewCheckBoxCell) this.CellTemplate;
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
                if (this.CheckBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return this.CheckBoxCellTemplate.FalseValue;
            }
            set
            {
                if (this.FalseValue != value)
                {
                    this.CheckBoxCellTemplate.FalseValueInternal = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewCheckBoxCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewCheckBoxCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.FalseValueInternal = value;
                            }
                        }
                        this.DataGridView.InvalidateColumn(this.Index);
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
                if (this.CheckBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return this.CheckBoxCellTemplate.FlatStyle;
            }
            set
            {
                if (this.FlatStyle != value)
                {
                    this.CheckBoxCellTemplate.FlatStyle = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewCheckBoxCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewCheckBoxCell;
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
                if (this.CheckBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return this.CheckBoxCellTemplate.IndeterminateValue;
            }
            set
            {
                if (this.IndeterminateValue != value)
                {
                    this.CheckBoxCellTemplate.IndeterminateValueInternal = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewCheckBoxCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewCheckBoxCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.IndeterminateValueInternal = value;
                            }
                        }
                        this.DataGridView.InvalidateColumn(this.Index);
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
                if (this.CheckBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return this.CheckBoxCellTemplate.ThreeState;
            }
            [
                SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes") // OK to cast a NullValue into a bool and CheckState
            ]
            set
            {
                if (this.ThreeState != value)
                {
                    this.CheckBoxCellTemplate.ThreeStateInternal = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewCheckBoxCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewCheckBoxCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.ThreeStateInternal = value;
                            }
                        }
                        this.DataGridView.InvalidateColumn(this.Index);
                    }
                    
                    if (value && 
                        this.DefaultCellStyle.NullValue is bool && 
                        (bool) this.DefaultCellStyle.NullValue == false)
                    {
                        this.DefaultCellStyle.NullValue = CheckState.Indeterminate;
                    }
                    else if (!value && 
                             this.DefaultCellStyle.NullValue is CheckState && 
                             (CheckState) this.DefaultCellStyle.NullValue == CheckState.Indeterminate)
                    {
                        this.DefaultCellStyle.NullValue = false;
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
                if (this.CheckBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return this.CheckBoxCellTemplate.TrueValue;
            }
            set
            {
                if (this.TrueValue != value)
                {
                    this.CheckBoxCellTemplate.TrueValueInternal = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewCheckBoxCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewCheckBoxCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.TrueValueInternal = value;
                            }
                        }
                        this.DataGridView.InvalidateColumn(this.Index);
                    }
                }
            }
        }

        private bool ShouldSerializeDefaultCellStyle()
        {
            object defaultNullValue;
            DataGridViewCheckBoxCell templateCell = this.CellTemplate as DataGridViewCheckBoxCell;
            if (templateCell == null)
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
                    !defaultCellStyle.NullValue.Equals(defaultNullValue) ||
                    !defaultCellStyle.IsDataSourceNullValueDefault ||
                    !string.IsNullOrEmpty(defaultCellStyle.Format) ||
                    !defaultCellStyle.FormatProvider.Equals(System.Globalization.CultureInfo.CurrentCulture) ||
                    defaultCellStyle.Alignment != DataGridViewContentAlignment.MiddleCenter ||
                    defaultCellStyle.WrapMode != DataGridViewTriState.NotSet ||
                    defaultCellStyle.Tag !=  null ||
                    !defaultCellStyle.Padding.Equals(Padding.Empty));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append("DataGridViewCheckBoxColumn { Name=");
            sb.Append(this.Name);
            sb.Append(", Index=");
            sb.Append(this.Index.ToString(CultureInfo.CurrentCulture));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}

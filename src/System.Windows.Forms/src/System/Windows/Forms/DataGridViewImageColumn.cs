﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Text;
    using System.Drawing;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    [ToolboxBitmapAttribute(typeof(DataGridViewImageColumn), "DataGridViewImageColumn")]
    public class DataGridViewImageColumn : DataGridViewColumn
    {
        private static Type columnType = typeof(DataGridViewImageColumn);
        private Image image;
        private Icon icon;

        public DataGridViewImageColumn() : this(false /*valuesAreIcons*/)
        {
        }

        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors") // Can't think of a workaround.
        ]
        public DataGridViewImageColumn(bool valuesAreIcons)
            : base(new DataGridViewImageCell(valuesAreIcons))
        {
            DataGridViewCellStyle defaultCellStyle = new DataGridViewCellStyle();
            defaultCellStyle.AlignmentInternal = DataGridViewContentAlignment.MiddleCenter;
            if (valuesAreIcons)
            {
                defaultCellStyle.NullValue = DataGridViewImageCell.ErrorIcon;
            }
            else
            {
                defaultCellStyle.NullValue = DataGridViewImageCell.ErrorBitmap;
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
                if (value != null && !(value is System.Windows.Forms.DataGridViewImageCell))
                {
                    throw new InvalidCastException(string.Format(SR.DataGridViewTypeColumn_WrongCellTemplateType, "System.Windows.Forms.DataGridViewImageCell"));
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
            Browsable(true),
            DefaultValue(""),
            SRCategory(nameof(SR.CatAppearance)), 
            SRDescription(nameof(SR.DataGridViewImageColumn_DescriptionDescr))
        ]
        public string Description
        {
            get
            {
                if (this.CellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return this.ImageCellTemplate.Description;
            }
            set
            {
                if (this.CellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                this.ImageCellTemplate.Description = value;
                if (this.DataGridView != null)
                {
                    DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                    int rowCount = dataGridViewRows.Count;
                    for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                    {
                        DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                        DataGridViewImageCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewImageCell;
                        if (dataGridViewCell != null)
                        {
                            dataGridViewCell.Description = value;
                        }
                    }
                }
            }
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public Icon Icon
        {
            get
            {
                return this.icon;
            }
            set
            {
                this.icon = value;
                if (this.DataGridView != null)
                {
                    this.DataGridView.OnColumnCommonChange(this.Index);
                }
            }
        }

        [
            DefaultValue(null),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridViewImageColumn_ImageDescr))
        ]
        public Image Image
        {
            get
            {
                return this.image;
            }
            set
            {
                this.image = value;
                if (this.DataGridView != null)
                {
                    this.DataGridView.OnColumnCommonChange(this.Index);
                }
            }
        }

        private DataGridViewImageCell ImageCellTemplate
        {
            get
            {
                return (DataGridViewImageCell) this.CellTemplate;
            }
        }

        [
            DefaultValue(DataGridViewImageCellLayout.Normal),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridViewImageColumn_ImageLayoutDescr))
        ]
        public DataGridViewImageCellLayout ImageLayout
        {
            get
            {
                if (this.CellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                DataGridViewImageCellLayout imageLayout = this.ImageCellTemplate.ImageLayout;
                if (imageLayout == DataGridViewImageCellLayout.NotSet)
                {
                    imageLayout = DataGridViewImageCellLayout.Normal;
                }
                return imageLayout;
            }
            set
            {
                if (this.ImageLayout != value)
                {
                    this.ImageCellTemplate.ImageLayout = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewImageCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewImageCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.ImageLayoutInternal = value;
                            }
                        }
                        this.DataGridView.OnColumnCommonChange(this.Index);
                    }
                }
            }
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool ValuesAreIcons
        {
            get
            {
                if (this.ImageCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return this.ImageCellTemplate.ValueIsIcon;
            }
            set
            {
                if (this.ValuesAreIcons != value)
                {
                    this.ImageCellTemplate.ValueIsIconInternal = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewImageCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewImageCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.ValueIsIconInternal = value;
                            }
                        }
                        this.DataGridView.OnColumnCommonChange(this.Index);
                    }
                    
                    if (value && 
                        this.DefaultCellStyle.NullValue is Bitmap && 
                        (Bitmap) this.DefaultCellStyle.NullValue == DataGridViewImageCell.ErrorBitmap)
                    {
                        this.DefaultCellStyle.NullValue = DataGridViewImageCell.ErrorIcon;
                    }
                    else if (!value && 
                             this.DefaultCellStyle.NullValue is Icon && 
                             (Icon) this.DefaultCellStyle.NullValue == DataGridViewImageCell.ErrorIcon)
                    {
                        this.DefaultCellStyle.NullValue = DataGridViewImageCell.ErrorBitmap;
                    }
                }
            }
        }

        public override object Clone()
        {
            DataGridViewImageColumn dataGridViewColumn;
            Type thisType = this.GetType();

            if (thisType == columnType) //performance improvement
            {
                dataGridViewColumn = new DataGridViewImageColumn();
            }
            else
            {
                // 

                dataGridViewColumn = (DataGridViewImageColumn)System.Activator.CreateInstance(thisType);
            }
            if (dataGridViewColumn != null)
            {
                base.CloneInternal(dataGridViewColumn);
                dataGridViewColumn.Icon = this.icon;
                dataGridViewColumn.Image = this.image;
            }
            return dataGridViewColumn;
        }

        private bool ShouldSerializeDefaultCellStyle()
        {
            DataGridViewImageCell templateCell = this.CellTemplate as DataGridViewImageCell;
            if (templateCell == null)
            {
                Debug.Fail("we can't compute the default cell style w/o a template cell");
                return true;
            }

            if (!this.HasDefaultCellStyle)
            {
                return false;
            }

            object defaultNullValue;
            if (templateCell.ValueIsIcon)
            {
                defaultNullValue = DataGridViewImageCell.ErrorIcon;
            }
            else
            {
                defaultNullValue = DataGridViewImageCell.ErrorBitmap;
            }

            DataGridViewCellStyle defaultCellStyle = this.DefaultCellStyle;

            return (!defaultCellStyle.BackColor.IsEmpty ||
                    !defaultCellStyle.ForeColor.IsEmpty ||
                    !defaultCellStyle.SelectionBackColor.IsEmpty ||
                    !defaultCellStyle.SelectionForeColor.IsEmpty ||
                    defaultCellStyle.Font != null ||
                    !defaultNullValue.Equals(defaultCellStyle.NullValue) ||
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
            sb.Append("DataGridViewImageColumn { Name=");
            sb.Append(this.Name);
            sb.Append(", Index=");
            sb.Append(this.Index.ToString(CultureInfo.CurrentCulture));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}

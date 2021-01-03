﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace System.Windows.Forms
{
    [ToolboxBitmap(typeof(DataGridViewImageColumn), "DataGridViewImageColumn")]
    public class DataGridViewImageColumn : DataGridViewColumn
    {
        private static readonly Type s_columnType = typeof(DataGridViewImageColumn);
        private Image _image;
        private Icon _icon;

        public DataGridViewImageColumn() : this(false /*valuesAreIcons*/)
        {
        }

        public DataGridViewImageColumn(bool valuesAreIcons)
            : base(new DataGridViewImageCell(valuesAreIcons))
        {
            DataGridViewCellStyle defaultCellStyle = new DataGridViewCellStyle
            {
                AlignmentInternal = DataGridViewContentAlignment.MiddleCenter
            };
            if (valuesAreIcons)
            {
                defaultCellStyle.NullValue = DataGridViewImageCell.ErrorIcon;
            }
            else
            {
                defaultCellStyle.NullValue = DataGridViewImageCell.ErrorBitmap;
            }
            DefaultCellStyle = defaultCellStyle;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override DataGridViewCell CellTemplate
        {
            get => base.CellTemplate;
            set
            {
                if (value is not null && !(value is DataGridViewImageCell))
                {
                    throw new InvalidCastException(string.Format(SR.DataGridViewTypeColumn_WrongCellTemplateType, "System.Windows.Forms.DataGridViewImageCell"));
                }
                base.CellTemplate = value;
            }
        }

        [Browsable(true)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.DataGridView_ColumnDefaultCellStyleDescr))]
        public override DataGridViewCellStyle DefaultCellStyle
        {
            get => base.DefaultCellStyle;
            set => base.DefaultCellStyle = value;
        }

        [Browsable(true)]
        [DefaultValue("")]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.DataGridViewImageColumn_DescriptionDescr))]
        public string Description
        {
            get
            {
                if (CellTemplate is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                return ImageCellTemplate.Description;
            }
            set
            {
                if (CellTemplate is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                ImageCellTemplate.Description = value;
                if (DataGridView is not null)
                {
                    DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
                    int rowCount = dataGridViewRows.Count;
                    for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                    {
                        DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                        if (dataGridViewRow.Cells[Index] is DataGridViewImageCell dataGridViewCell)
                        {
                            dataGridViewCell.Description = value;
                        }
                    }
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Icon Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                if (DataGridView is not null)
                {
                    DataGridView.OnColumnCommonChange(Index);
                }
            }
        }

        [DefaultValue(null)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.DataGridViewImageColumn_ImageDescr))]
        public Image Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                if (DataGridView is not null)
                {
                    DataGridView.OnColumnCommonChange(Index);
                }
            }
        }

        private DataGridViewImageCell ImageCellTemplate
        {
            get
            {
                return (DataGridViewImageCell)CellTemplate;
            }
        }

        [DefaultValue(DataGridViewImageCellLayout.Normal)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.DataGridViewImageColumn_ImageLayoutDescr))]
        public DataGridViewImageCellLayout ImageLayout
        {
            get
            {
                if (CellTemplate is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                DataGridViewImageCellLayout imageLayout = ImageCellTemplate.ImageLayout;
                if (imageLayout == DataGridViewImageCellLayout.NotSet)
                {
                    imageLayout = DataGridViewImageCellLayout.Normal;
                }
                return imageLayout;
            }
            set
            {
                if (ImageLayout != value)
                {
                    ImageCellTemplate.ImageLayout = value;
                    if (DataGridView is not null)
                    {
                        DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            if (dataGridViewRow.Cells[Index] is DataGridViewImageCell dataGridViewCell)
                            {
                                dataGridViewCell.ImageLayoutInternal = value;
                            }
                        }
                        DataGridView.OnColumnCommonChange(Index);
                    }
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ValuesAreIcons
        {
            get
            {
                if (ImageCellTemplate is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                return ImageCellTemplate.ValueIsIcon;
            }
            set
            {
                if (ValuesAreIcons != value)
                {
                    ImageCellTemplate.ValueIsIconInternal = value;
                    if (DataGridView is not null)
                    {
                        DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            if (dataGridViewRow.Cells[Index] is DataGridViewImageCell dataGridViewCell)
                            {
                                dataGridViewCell.ValueIsIconInternal = value;
                            }
                        }
                        DataGridView.OnColumnCommonChange(Index);
                    }

                    if (value &&
                        DefaultCellStyle.NullValue is Bitmap &&
                        (Bitmap)DefaultCellStyle.NullValue == DataGridViewImageCell.ErrorBitmap)
                    {
                        DefaultCellStyle.NullValue = DataGridViewImageCell.ErrorIcon;
                    }
                    else if (!value &&
                             DefaultCellStyle.NullValue is Icon &&
                             (Icon)DefaultCellStyle.NullValue == DataGridViewImageCell.ErrorIcon)
                    {
                        DefaultCellStyle.NullValue = DataGridViewImageCell.ErrorBitmap;
                    }
                }
            }
        }

        public override object Clone()
        {
            DataGridViewImageColumn dataGridViewColumn;
            Type thisType = GetType();

            if (thisType == s_columnType) //performance improvement
            {
                dataGridViewColumn = new DataGridViewImageColumn();
            }
            else
            {
                //

                dataGridViewColumn = (DataGridViewImageColumn)System.Activator.CreateInstance(thisType);
            }
            if (dataGridViewColumn is not null)
            {
                base.CloneInternal(dataGridViewColumn);
                dataGridViewColumn.Icon = _icon;
                dataGridViewColumn.Image = _image;
            }
            return dataGridViewColumn;
        }

        private bool ShouldSerializeDefaultCellStyle()
        {
            if (!(CellTemplate is DataGridViewImageCell templateCell))
            {
                Debug.Fail("we can't compute the default cell style w/o a template cell");
                return true;
            }

            if (!HasDefaultCellStyle)
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

            DataGridViewCellStyle defaultCellStyle = DefaultCellStyle;

            return (!defaultCellStyle.BackColor.IsEmpty ||
                    !defaultCellStyle.ForeColor.IsEmpty ||
                    !defaultCellStyle.SelectionBackColor.IsEmpty ||
                    !defaultCellStyle.SelectionForeColor.IsEmpty ||
                    defaultCellStyle.Font is not null ||
                    !defaultNullValue.Equals(defaultCellStyle.NullValue) ||
                    !defaultCellStyle.IsDataSourceNullValueDefault ||
                    !string.IsNullOrEmpty(defaultCellStyle.Format) ||
                    !defaultCellStyle.FormatProvider.Equals(System.Globalization.CultureInfo.CurrentCulture) ||
                    defaultCellStyle.Alignment != DataGridViewContentAlignment.MiddleCenter ||
                    defaultCellStyle.WrapMode != DataGridViewTriState.NotSet ||
                    defaultCellStyle.Tag is not null ||
                    !defaultCellStyle.Padding.Equals(Padding.Empty));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append("DataGridViewImageColumn { Name=");
            sb.Append(Name);
            sb.Append(", Index=");
            sb.Append(Index.ToString(CultureInfo.CurrentCulture));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}

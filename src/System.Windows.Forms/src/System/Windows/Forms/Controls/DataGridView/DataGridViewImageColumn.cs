// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
[ToolboxBitmap(typeof(DataGridViewImageColumn), "DataGridViewImageColumn")]
public class DataGridViewImageColumn : DataGridViewColumn
{
    private static readonly Type s_columnType = typeof(DataGridViewImageColumn);
    private Image? _image;
    private Icon? _icon;

    public DataGridViewImageColumn()
        : this(valuesAreIcons: false)
    {
    }

    public DataGridViewImageColumn(bool valuesAreIcons)
        : base(new DataGridViewImageCell(valuesAreIcons))
    {
        DataGridViewCellStyle defaultCellStyle = new()
        {
            AlignmentInternal = DataGridViewContentAlignment.MiddleCenter,
            NullValue = valuesAreIcons
                ? DataGridViewImageCell.ErrorIcon
                : DataGridViewImageCell.ErrorBitmap
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
            if (value is not null and not DataGridViewImageCell)
            {
                throw new InvalidCastException(string.Format(SR.DataGridViewTypeColumn_WrongCellTemplateType, "System.Windows.Forms.DataGridViewImageCell"));
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

    [Browsable(true)]
    [DefaultValue("")]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridViewImageColumn_DescriptionDescr))]
    [AllowNull]
    [MemberNotNull(nameof(ImageCellTemplate))]
    public string Description
    {
        get
        {
            if (ImageCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return ImageCellTemplate.Description;
        }

        set
        {
            if (Description == value)
            {
                return;
            }

            ImageCellTemplate.Description = value;
            if (DataGridView is null)
            {
                return;
            }

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

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Icon? Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            DataGridView?.OnColumnCommonChange(Index);
        }
    }

    [DefaultValue(null)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridViewImageColumn_ImageDescr))]
    public Image? Image
    {
        get => _image;
        set
        {
            _image = value;
            DataGridView?.OnColumnCommonChange(Index);
        }
    }

    private DataGridViewImageCell? ImageCellTemplate => (DataGridViewImageCell?)CellTemplate;

    [DefaultValue(DataGridViewImageCellLayout.Normal)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridViewImageColumn_ImageLayoutDescr))]
    [MemberNotNull(nameof(ImageCellTemplate))]
    public DataGridViewImageCellLayout ImageLayout
    {
        get
        {
            if (ImageCellTemplate is null)
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
            if (ImageLayout == value)
            {
                return;
            }

            ImageCellTemplate.ImageLayout = value;
            if (DataGridView is null)
            {
                return;
            }

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

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [MemberNotNull(nameof(ImageCellTemplate))]
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
            if (ValuesAreIcons == value)
            {
                return;
            }

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
                DefaultCellStyle.NullValue is Bitmap bitmap &&
                bitmap == DataGridViewImageCell.ErrorBitmap)
            {
                DefaultCellStyle.NullValue = DataGridViewImageCell.ErrorIcon;
            }
            else if (!value &&
                     DefaultCellStyle.NullValue is Icon icon &&
                     icon == DataGridViewImageCell.ErrorIcon)
            {
                DefaultCellStyle.NullValue = DataGridViewImageCell.ErrorBitmap;
            }
        }
    }

    public override object Clone()
    {
        DataGridViewImageColumn dataGridViewColumn;
        Type thisType = GetType();

        if (thisType == s_columnType) // performance improvement
        {
            dataGridViewColumn = new DataGridViewImageColumn();
        }
        else
        {
            dataGridViewColumn = (DataGridViewImageColumn)Activator.CreateInstance(thisType)!;
        }

        CloneInternal(dataGridViewColumn);
        dataGridViewColumn.Icon = _icon;
        dataGridViewColumn.Image = _image;

        return dataGridViewColumn;
    }

    private bool ShouldSerializeDefaultCellStyle()
    {
        if (CellTemplate is not DataGridViewImageCell templateCell)
        {
            Debug.Fail("we can't compute the default cell style w/o a template cell");
            return true;
        }

        if (!HasDefaultCellStyle)
        {
            return false;
        }

        object defaultNullValue = templateCell.ValueIsIcon
            ? DataGridViewImageCell.ErrorIcon
            : DataGridViewImageCell.ErrorBitmap;

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
        $"DataGridViewImageColumn {{ Name={Name}, Index={Index} }}";
}

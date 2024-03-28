// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms;

[Designer($"System.Windows.Forms.Design.DataGridViewComboBoxColumnDesigner, {AssemblyRef.SystemDesign}")]
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
[ToolboxBitmap(typeof(DataGridViewComboBoxColumn), "DataGridViewComboBoxColumn")]
public class DataGridViewComboBoxColumn : DataGridViewColumn
{
    private static readonly Type s_columnType = typeof(DataGridViewComboBoxColumn);

    public DataGridViewComboBoxColumn()
        : base(new DataGridViewComboBoxCell())
    {
        ((DataGridViewComboBoxCell)base.CellTemplate!).TemplateComboBoxColumn = this;
    }

    [Browsable(true)]
    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_ComboBoxColumnAutoCompleteDescr))]
    [MemberNotNull(nameof(ComboBoxCellTemplate))]
    public bool AutoComplete
    {
        get
        {
            if (ComboBoxCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return ComboBoxCellTemplate.AutoComplete;
        }
        set
        {
            if (AutoComplete == value)
            {
                return;
            }

            ComboBoxCellTemplate.AutoComplete = value;
            if (DataGridView is null)
            {
                return;
            }

            DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
            int rowCount = dataGridViewRows.Count;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is DataGridViewComboBoxCell dataGridViewCell)
                {
                    dataGridViewCell.AutoComplete = value;
                }
            }
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override DataGridViewCell? CellTemplate
    {
        get => base.CellTemplate;
        set
        {
            DataGridViewComboBoxCell? dataGridViewComboBoxCell = value as DataGridViewComboBoxCell;
            if (value is not null && dataGridViewComboBoxCell is null)
            {
                throw new InvalidCastException(string.Format(SR.DataGridViewTypeColumn_WrongCellTemplateType, "System.Windows.Forms.DataGridViewComboBoxCell"));
            }

            base.CellTemplate = value;
            if (dataGridViewComboBoxCell is not null)
            {
                dataGridViewComboBoxCell.TemplateComboBoxColumn = this;
            }
        }
    }

    private DataGridViewComboBoxCell? ComboBoxCellTemplate => (DataGridViewComboBoxCell?)CellTemplate;

    [DefaultValue(null)]
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.DataGridView_ComboBoxColumnDataSourceDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [AttributeProvider(typeof(IListSource))]
    [MemberNotNull(nameof(ComboBoxCellTemplate))]
    public object? DataSource
    {
        get
        {
            if (ComboBoxCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return ComboBoxCellTemplate.DataSource;
        }
        set
        {
            if (DataSource == value)
            {
                return;
            }

            ComboBoxCellTemplate.DataSource = value;
            if (DataGridView is null)
            {
                return;
            }

            DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
            int rowCount = dataGridViewRows.Count;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is DataGridViewComboBoxCell dataGridViewCell)
                {
                    dataGridViewCell.DataSource = value;
                }
            }

            DataGridView.OnColumnCommonChange(Index);
        }
    }

    [DefaultValue("")]
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.DataGridView_ComboBoxColumnDisplayMemberDescr))]
    [TypeConverter($"System.Windows.Forms.Design.DataMemberFieldConverter, {AssemblyRef.SystemDesign}")]
    [Editor($"System.Windows.Forms.Design.DataMemberFieldEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [MemberNotNull(nameof(ComboBoxCellTemplate))]
    [AllowNull]
    public string DisplayMember
    {
        get
        {
            if (ComboBoxCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return ComboBoxCellTemplate.DisplayMember;
        }
        set
        {
            if (DisplayMember == value)
            {
                return;
            }

            ComboBoxCellTemplate.DisplayMember = value;
            if (DataGridView is null)
            {
                return;
            }

            DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
            int rowCount = dataGridViewRows.Count;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is DataGridViewComboBoxCell dataGridViewCell)
                {
                    dataGridViewCell.DisplayMember = value;
                }
            }

            DataGridView.OnColumnCommonChange(Index);
        }
    }

    [DefaultValue(DataGridViewComboBoxDisplayStyle.DropDownButton)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_ComboBoxColumnDisplayStyleDescr))]
    [MemberNotNull(nameof(ComboBoxCellTemplate))]
    public DataGridViewComboBoxDisplayStyle DisplayStyle
    {
        get
        {
            if (ComboBoxCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return ComboBoxCellTemplate.DisplayStyle;
        }
        set
        {
            if (DisplayStyle == value)
            {
                return;
            }

            ComboBoxCellTemplate.DisplayStyle = value;
            if (DataGridView is null)
            {
                return;
            }

            DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
            int rowCount = dataGridViewRows.Count;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is DataGridViewComboBoxCell dataGridViewCell)
                {
                    dataGridViewCell.DisplayStyleInternal = value;
                }
            }

            // Calling InvalidateColumn instead of OnColumnCommonChange because DisplayStyle does not affect preferred size.
            DataGridView.InvalidateColumn(Index);
        }
    }

    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_ComboBoxColumnDisplayStyleForCurrentCellOnlyDescr))]
    [MemberNotNull(nameof(ComboBoxCellTemplate))]
    public bool DisplayStyleForCurrentCellOnly
    {
        get
        {
            if (ComboBoxCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return ComboBoxCellTemplate.DisplayStyleForCurrentCellOnly;
        }
        set
        {
            if (DisplayStyleForCurrentCellOnly == value)
            {
                return;
            }

            ComboBoxCellTemplate.DisplayStyleForCurrentCellOnly = value;
            if (DataGridView is null)
            {
                return;
            }

            DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
            int rowCount = dataGridViewRows.Count;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is DataGridViewComboBoxCell dataGridViewCell)
                {
                    dataGridViewCell.DisplayStyleForCurrentCellOnlyInternal = value;
                }
            }

            // Calling InvalidateColumn instead of OnColumnCommonChange because DisplayStyleForCurrentCellOnly does not affect preferred size.
            DataGridView.InvalidateColumn(Index);
        }
    }

    [DefaultValue(1)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_ComboBoxColumnDropDownWidthDescr))]
    [MemberNotNull(nameof(ComboBoxCellTemplate))]
    public int DropDownWidth
    {
        get
        {
            if (ComboBoxCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return ComboBoxCellTemplate.DropDownWidth;
        }
        set
        {
            if (DropDownWidth == value)
            {
                return;
            }

            ComboBoxCellTemplate.DropDownWidth = value;
            if (DataGridView is null)
            {
                return;
            }

            DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
            int rowCount = dataGridViewRows.Count;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is DataGridViewComboBoxCell dataGridViewCell)
                {
                    dataGridViewCell.DropDownWidth = value;
                }
            }
        }
    }

    [DefaultValue(FlatStyle.Standard)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_ComboBoxColumnFlatStyleDescr))]
    [MemberNotNull(nameof(ComboBoxCellTemplate))]
    public FlatStyle FlatStyle
    {
        get
        {
            if (ComboBoxCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return ComboBoxCellTemplate.FlatStyle;
        }
        set
        {
            if (FlatStyle == value)
            {
                return;
            }

            ComboBoxCellTemplate.FlatStyle = value;
            if (DataGridView is null)
            {
                return;
            }

            DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
            int rowCount = dataGridViewRows.Count;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is DataGridViewComboBoxCell dataGridViewCell)
                {
                    dataGridViewCell.FlatStyleInternal = value;
                }
            }

            DataGridView.OnColumnCommonChange(Index);
        }
    }

    [Editor($"System.Windows.Forms.Design.StringCollectionEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.DataGridView_ComboBoxColumnItemsDescr))]
    public DataGridViewComboBoxCell.ObjectCollection Items
    {
        get
        {
            if (ComboBoxCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return ComboBoxCellTemplate.GetItems(DataGridView);
        }
    }

    [DefaultValue("")]
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.DataGridView_ComboBoxColumnValueMemberDescr))]
    [TypeConverter($"System.Windows.Forms.Design.DataMemberFieldConverter, {AssemblyRef.SystemDesign}")]
    [Editor($"System.Windows.Forms.Design.DataMemberFieldEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [MemberNotNull(nameof(ComboBoxCellTemplate))]
    [AllowNull]
    public string ValueMember
    {
        get
        {
            if (ComboBoxCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return ComboBoxCellTemplate.ValueMember;
        }
        set
        {
            if (ValueMember == value)
            {
                return;
            }

            ComboBoxCellTemplate.ValueMember = value;
            if (DataGridView is null)
            {
                return;
            }

            DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
            int rowCount = dataGridViewRows.Count;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is DataGridViewComboBoxCell dataGridViewCell)
                {
                    dataGridViewCell.ValueMember = value;
                }
            }

            DataGridView.OnColumnCommonChange(Index);
        }
    }

    [DefaultValue(DataGridViewComboBoxCell.DefaultMaxDropDownItems)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_ComboBoxColumnMaxDropDownItemsDescr))]
    [MemberNotNull(nameof(ComboBoxCellTemplate))]
    public int MaxDropDownItems
    {
        get
        {
            if (ComboBoxCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return ComboBoxCellTemplate.MaxDropDownItems;
        }
        set
        {
            if (MaxDropDownItems == value)
            {
                return;
            }

            ComboBoxCellTemplate.MaxDropDownItems = value;
            if (DataGridView is null)
            {
                return;
            }

            DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
            int rowCount = dataGridViewRows.Count;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is DataGridViewComboBoxCell dataGridViewCell)
                {
                    dataGridViewCell.MaxDropDownItems = value;
                }
            }
        }
    }

    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_ComboBoxColumnSortedDescr))]
    [MemberNotNull(nameof(ComboBoxCellTemplate))]
    public bool Sorted
    {
        get
        {
            if (ComboBoxCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return ComboBoxCellTemplate.Sorted;
        }
        set
        {
            if (Sorted == value)
            {
                return;
            }

            ComboBoxCellTemplate.Sorted = value;
            if (DataGridView is null)
            {
                return;
            }

            DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
            int rowCount = dataGridViewRows.Count;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is DataGridViewComboBoxCell dataGridViewCell)
                {
                    dataGridViewCell.Sorted = value;
                }
            }
        }
    }

    public override object Clone()
    {
        DataGridViewComboBoxColumn dataGridViewColumn;
        Type thisType = GetType();

        if (thisType == s_columnType) // performance improvement
        {
            dataGridViewColumn = new DataGridViewComboBoxColumn();
        }
        else
        {
            dataGridViewColumn = (DataGridViewComboBoxColumn)Activator.CreateInstance(thisType)!;
        }

        CloneInternal(dataGridViewColumn);
        dataGridViewColumn.ComboBoxCellTemplate!.TemplateComboBoxColumn = dataGridViewColumn;

        return dataGridViewColumn;
    }

    internal void OnItemsCollectionChanged()
    {
        // Items collection of the CellTemplate was changed.
        // Update the items collection of each existing DataGridViewComboBoxCell in the column.
        if (DataGridView is null)
        {
            return;
        }

        DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
        int rowCount = dataGridViewRows.Count;
        object[] items = [.. ComboBoxCellTemplate!.Items.InnerArray];
        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
            if (dataGridViewRow.Cells[Index] is DataGridViewComboBoxCell dataGridViewCell)
            {
                dataGridViewCell.Items.ClearInternal();
                dataGridViewCell.Items.AddRangeInternal(items);
            }
        }

        DataGridView.OnColumnCommonChange(Index);
    }

    /// <summary>
    /// </summary>
    public override string ToString() =>
        $"DataGridViewComboBoxColumn {{ Name={Name}, Index={Index} }}";
}

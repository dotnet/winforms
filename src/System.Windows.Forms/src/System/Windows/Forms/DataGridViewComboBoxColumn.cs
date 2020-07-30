// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Text;

namespace System.Windows.Forms
{
    [Designer("System.Windows.Forms.Design.DataGridViewComboBoxColumnDesigner, " + AssemblyRef.SystemDesign)]
    [ToolboxBitmap(typeof(DataGridViewComboBoxColumn), "DataGridViewComboBoxColumn")]
    public class DataGridViewComboBoxColumn : DataGridViewColumn
    {
        private static readonly Type s_columnType = typeof(DataGridViewComboBoxColumn);

        public DataGridViewComboBoxColumn() : base(new DataGridViewComboBoxCell())
        {
            ((DataGridViewComboBoxCell)base.CellTemplate).TemplateComboBoxColumn = this;
        }

        [Browsable(true)]
        [DefaultValue(true)]
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.DataGridView_ComboBoxColumnAutoCompleteDescr))]
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
                if (AutoComplete != value)
                {
                    ComboBoxCellTemplate.AutoComplete = value;
                    if (DataGridView != null)
                    {
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
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override DataGridViewCell CellTemplate
        {
            get => base.CellTemplate;
            set
            {
                DataGridViewComboBoxCell dataGridViewComboBoxCell = value as DataGridViewComboBoxCell;
                if (value != null && dataGridViewComboBoxCell is null)
                {
                    throw new InvalidCastException(string.Format(SR.DataGridViewTypeColumn_WrongCellTemplateType, "System.Windows.Forms.DataGridViewComboBoxCell"));
                }
                base.CellTemplate = value;
                if (value != null)
                {
                    dataGridViewComboBoxCell.TemplateComboBoxColumn = this;
                }
            }
        }

        private DataGridViewComboBoxCell ComboBoxCellTemplate
        {
            get
            {
                return (DataGridViewComboBoxCell)CellTemplate;
            }
        }

        [DefaultValue(null)]
        [SRCategory(nameof(SR.CatData))]
        [SRDescription(nameof(SR.DataGridView_ComboBoxColumnDataSourceDescr))]
        [RefreshProperties(RefreshProperties.Repaint)]
        [AttributeProvider(typeof(IListSource))]
        public object DataSource
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
                if (ComboBoxCellTemplate is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                ComboBoxCellTemplate.DataSource = value;
                if (DataGridView != null)
                {
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
        }

        [DefaultValue("")]
        [SRCategory(nameof(SR.CatData))]
        [SRDescription(nameof(SR.DataGridView_ComboBoxColumnDisplayMemberDescr))]
        [TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, " + AssemblyRef.SystemDesign)]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
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
                if (ComboBoxCellTemplate is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                ComboBoxCellTemplate.DisplayMember = value;
                if (DataGridView != null)
                {
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
        }

        [DefaultValue(DataGridViewComboBoxDisplayStyle.DropDownButton)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.DataGridView_ComboBoxColumnDisplayStyleDescr))]
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
                if (ComboBoxCellTemplate is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                ComboBoxCellTemplate.DisplayStyle = value;
                if (DataGridView != null)
                {
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
        }

        [DefaultValue(false)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.DataGridView_ComboBoxColumnDisplayStyleForCurrentCellOnlyDescr))]
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
                if (ComboBoxCellTemplate is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                ComboBoxCellTemplate.DisplayStyleForCurrentCellOnly = value;
                if (DataGridView != null)
                {
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
        }

        [DefaultValue(1)]
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.DataGridView_ComboBoxColumnDropDownWidthDescr))]
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
                if (DropDownWidth != value)
                {
                    ComboBoxCellTemplate.DropDownWidth = value;
                    if (DataGridView != null)
                    {
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
            }
        }

        [DefaultValue(FlatStyle.Standard)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.DataGridView_ComboBoxColumnFlatStyleDescr))]
        public FlatStyle FlatStyle
        {
            get
            {
                if (CellTemplate is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                return ((DataGridViewComboBoxCell)CellTemplate).FlatStyle;
            }
            set
            {
                if (FlatStyle != value)
                {
                    ((DataGridViewComboBoxCell)CellTemplate).FlatStyle = value;
                    if (DataGridView != null)
                    {
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
            }
        }

        [Editor("System.Windows.Forms.Design.StringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
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
        [TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, " + AssemblyRef.SystemDesign)]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
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
                if (ComboBoxCellTemplate is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                ComboBoxCellTemplate.ValueMember = value;
                if (DataGridView != null)
                {
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
        }

        [DefaultValue(DataGridViewComboBoxCell.DefaultMaxDropDownItems)]
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.DataGridView_ComboBoxColumnMaxDropDownItemsDescr))]
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
                if (MaxDropDownItems != value)
                {
                    ComboBoxCellTemplate.MaxDropDownItems = value;
                    if (DataGridView != null)
                    {
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
            }
        }

        [DefaultValue(false)]
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.DataGridView_ComboBoxColumnSortedDescr))]
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
                if (Sorted != value)
                {
                    ComboBoxCellTemplate.Sorted = value;
                    if (DataGridView != null)
                    {
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
            }
        }

        public override object Clone()
        {
            DataGridViewComboBoxColumn dataGridViewColumn;
            Type thisType = GetType();

            if (thisType == s_columnType) //performance improvement
            {
                dataGridViewColumn = new DataGridViewComboBoxColumn();
            }
            else
            {
                //

                dataGridViewColumn = (DataGridViewComboBoxColumn)System.Activator.CreateInstance(thisType);
            }
            if (dataGridViewColumn != null)
            {
                base.CloneInternal(dataGridViewColumn);
                ((DataGridViewComboBoxCell)dataGridViewColumn.CellTemplate).TemplateComboBoxColumn = dataGridViewColumn;
            }
            return dataGridViewColumn;
        }

        internal void OnItemsCollectionChanged()
        {
            // Items collection of the CellTemplate was changed.
            // Update the items collection of each existing DataGridViewComboBoxCell in the column.
            if (DataGridView != null)
            {
                DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
                int rowCount = dataGridViewRows.Count;
                object[] items = ((DataGridViewComboBoxCell)CellTemplate).Items.InnerArray.ToArray();
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
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append("DataGridViewComboBoxColumn { Name=");
            sb.Append(Name);
            sb.Append(", Index=");
            sb.Append(Index.ToString(CultureInfo.CurrentCulture));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}

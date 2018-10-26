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
    using System.Drawing.Design;
    using System.Globalization;
    
    /// <include file='doc\DataGridViewComboBoxColumn.uex' path='docs/doc[@for="DataGridViewComboBoxColumn"]/*' />
    [
        Designer("System.Windows.Forms.Design.DataGridViewComboBoxColumnDesigner, " + AssemblyRef.SystemDesign),
        ToolboxBitmapAttribute(typeof(DataGridViewComboBoxColumn), "DataGridViewComboBoxColumn.bmp")
    ]
    public class DataGridViewComboBoxColumn : DataGridViewColumn
    {
        private static Type columnType = typeof(DataGridViewComboBoxColumn);

        /// <include file='doc\DataGridViewComboBoxColumn.uex' path='docs/doc[@for="DataGridViewComboBoxColumn.DataGridViewComboBoxColumn"]/*' />
        public DataGridViewComboBoxColumn() : base(new DataGridViewComboBoxCell())
        {
            ((DataGridViewComboBoxCell)base.CellTemplate).TemplateComboBoxColumn = this;
        }

        /// <include file='doc\DataGridViewComboBoxColumn.uex' path='docs/doc[@for="DataGridViewComboBoxColumn.AutoComplete"]/*' />
        [
            Browsable(true),
            DefaultValue(true),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_ComboBoxColumnAutoCompleteDescr))
        ]
        public bool AutoComplete
        {
            get
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return this.ComboBoxCellTemplate.AutoComplete;
            }
            set
            {
                if (this.AutoComplete != value)
                {
                    this.ComboBoxCellTemplate.AutoComplete = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewComboBoxCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewComboBoxCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.AutoComplete = value;
                            }
                        }
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxColumn.uex' path='docs/doc[@for="DataGridViewComboBoxColumn.CellTemplate"]/*' />
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
                DataGridViewComboBoxCell dataGridViewComboBoxCell = value as DataGridViewComboBoxCell;
                if (value != null && dataGridViewComboBoxCell == null)
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
                return (DataGridViewComboBoxCell) this.CellTemplate;
            }
        }

        /// <include file='doc\DataGridViewComboBoxColumn.uex' path='docs/doc[@for="DataGridViewComboBoxColumn.DataSource"]/*' />
        [
            DefaultValue(null),
            SRCategory(nameof(SR.CatData)),
            SRDescription(nameof(SR.DataGridView_ComboBoxColumnDataSourceDescr)),
            RefreshProperties(RefreshProperties.Repaint),
            AttributeProvider(typeof(IListSource)),
        ]
        public object DataSource
        {
            get
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return this.ComboBoxCellTemplate.DataSource;
            }
            set
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                this.ComboBoxCellTemplate.DataSource = value;
                if (this.DataGridView != null)
                {
                    DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                    int rowCount = dataGridViewRows.Count;
                    for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                    {
                        DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                        DataGridViewComboBoxCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewComboBoxCell;
                        if (dataGridViewCell != null)
                        {
                            dataGridViewCell.DataSource = value;
                        }
                    }
                    this.DataGridView.OnColumnCommonChange(this.Index);
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxColumn.uex' path='docs/doc[@for="DataGridViewComboBoxColumn.DisplayMember"]/*' />
        [
            DefaultValue(""),
            SRCategory(nameof(SR.CatData)),
            SRDescription(nameof(SR.DataGridView_ComboBoxColumnDisplayMemberDescr)),
            TypeConverterAttribute("System.Windows.Forms.Design.DataMemberFieldConverter, " + AssemblyRef.SystemDesign),
            Editor("System.Windows.Forms.Design.DataMemberFieldEditor, " + AssemblyRef.SystemDesign, typeof(System.Drawing.Design.UITypeEditor))
        ]
        public string DisplayMember
        {
            get 
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return this.ComboBoxCellTemplate.DisplayMember;
            }
            set 
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                this.ComboBoxCellTemplate.DisplayMember = value;
                if (this.DataGridView != null)
                {
                    DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                    int rowCount = dataGridViewRows.Count;
                    for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                    {
                        DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                        DataGridViewComboBoxCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewComboBoxCell;
                        if (dataGridViewCell != null)
                        {
                            dataGridViewCell.DisplayMember = value;
                        }
                    }
                    this.DataGridView.OnColumnCommonChange(this.Index);
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxColumn.uex' path='docs/doc[@for="DataGridViewComboBoxColumn.DisplayStyle"]/*' />
        [
            DefaultValue(DataGridViewComboBoxDisplayStyle.DropDownButton),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ComboBoxColumnDisplayStyleDescr))
        ]
        public DataGridViewComboBoxDisplayStyle DisplayStyle
        {
            get
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return this.ComboBoxCellTemplate.DisplayStyle;
            }
            set
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                this.ComboBoxCellTemplate.DisplayStyle = value;
                if (this.DataGridView != null)
                {
                    DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                    int rowCount = dataGridViewRows.Count;
                    for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                    {
                        DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                        DataGridViewComboBoxCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewComboBoxCell;
                        if (dataGridViewCell != null)
                        {
                            dataGridViewCell.DisplayStyleInternal = value;
                        }
                    }
                    // Calling InvalidateColumn instead of OnColumnCommonChange because DisplayStyle does not affect preferred size.
                    this.DataGridView.InvalidateColumn(this.Index);
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxColumn.uex' path='docs/doc[@for="DataGridViewComboBoxColumn.DisplayStyleForCurrentCellOnly"]/*' />
        [
            DefaultValue(false),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ComboBoxColumnDisplayStyleForCurrentCellOnlyDescr))
        ]
        public bool DisplayStyleForCurrentCellOnly
        {
            get
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return this.ComboBoxCellTemplate.DisplayStyleForCurrentCellOnly;
            }
            set
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                this.ComboBoxCellTemplate.DisplayStyleForCurrentCellOnly = value;
                if (this.DataGridView != null)
                {
                    DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                    int rowCount = dataGridViewRows.Count;
                    for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                    {
                        DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                        DataGridViewComboBoxCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewComboBoxCell;
                        if (dataGridViewCell != null)
                        {
                            dataGridViewCell.DisplayStyleForCurrentCellOnlyInternal = value;
                        }
                    }
                    // Calling InvalidateColumn instead of OnColumnCommonChange because DisplayStyleForCurrentCellOnly does not affect preferred size.
                    this.DataGridView.InvalidateColumn(this.Index);
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxColumn.uex' path='docs/doc[@for="DataGridViewComboBoxColumn.DropDownWidth"]/*' />
        [
            DefaultValue(1),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_ComboBoxColumnDropDownWidthDescr))
        ]
        public int DropDownWidth
        {
            get
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return this.ComboBoxCellTemplate.DropDownWidth;
            }
            set
            {
                if (this.DropDownWidth != value)
                {
                    this.ComboBoxCellTemplate.DropDownWidth = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewComboBoxCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewComboBoxCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.DropDownWidth = value;
                            }
                        }
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxColumn.uex' path='docs/doc[@for="DataGridViewComboBoxColumn.FlatStyle"]/*' />
        [
            DefaultValue(FlatStyle.Standard),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ComboBoxColumnFlatStyleDescr)),
        ]
        public FlatStyle FlatStyle
        {
            get
            {
                if (this.CellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return ((DataGridViewComboBoxCell) this.CellTemplate).FlatStyle;
            }
            set
            {
                if (this.FlatStyle != value)
                {
                    ((DataGridViewComboBoxCell)this.CellTemplate).FlatStyle = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewComboBoxCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewComboBoxCell;
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

        /// <include file='doc\DataGridViewComboBoxColumn.uex' path='docs/doc[@for="DataGridViewComboBoxColumn.Items"]/*' />
        [
            Editor("System.Windows.Forms.Design.StringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
            SRCategory(nameof(SR.CatData)),
            SRDescription(nameof(SR.DataGridView_ComboBoxColumnItemsDescr))
        ]
        public DataGridViewComboBoxCell.ObjectCollection Items
        {
            get
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return this.ComboBoxCellTemplate.GetItems(this.DataGridView);
            }
        }

        /// <include file='doc\DataGridViewComboBoxColumn.uex' path='docs/doc[@for="DataGridViewComboBoxColumn.ValueMember"]/*' />
        [
            DefaultValue(""),
            SRCategory(nameof(SR.CatData)),
            SRDescription(nameof(SR.DataGridView_ComboBoxColumnValueMemberDescr)),
            TypeConverterAttribute("System.Windows.Forms.Design.DataMemberFieldConverter, " + AssemblyRef.SystemDesign),
            Editor("System.Windows.Forms.Design.DataMemberFieldEditor, " + AssemblyRef.SystemDesign, typeof(System.Drawing.Design.UITypeEditor))
        ]
        public string ValueMember
        {
            get 
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return this.ComboBoxCellTemplate.ValueMember;
            }
            set 
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                this.ComboBoxCellTemplate.ValueMember = value;
                if (this.DataGridView != null)
                {
                    DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                    int rowCount = dataGridViewRows.Count;
                    for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                    {
                        DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                        DataGridViewComboBoxCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewComboBoxCell;
                        if (dataGridViewCell != null)
                        {
                            dataGridViewCell.ValueMember = value;
                        }
                    }
                    this.DataGridView.OnColumnCommonChange(this.Index);
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxColumn.uex' path='docs/doc[@for="DataGridViewComboBoxColumn.MaxDropDownItems"]/*' />
        [
            DefaultValue(DataGridViewComboBoxCell.DATAGRIDVIEWCOMBOBOXCELL_defaultMaxDropDownItems),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_ComboBoxColumnMaxDropDownItemsDescr))
        ]
        public int MaxDropDownItems
        {
            get
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return this.ComboBoxCellTemplate.MaxDropDownItems;
            }
            set 
            {
                if (this.MaxDropDownItems != value)
                {
                    this.ComboBoxCellTemplate.MaxDropDownItems = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewComboBoxCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewComboBoxCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.MaxDropDownItems = value;
                            }
                        }
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxColumn.uex' path='docs/doc[@for="DataGridViewComboBoxColumn.Sorted"]/*' />
        [
            DefaultValue(false),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_ComboBoxColumnSortedDescr))
        ]
        public bool Sorted
        {
            get 
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return this.ComboBoxCellTemplate.Sorted;
            }
            set 
            {
                if (this.Sorted != value)
                {
                    this.ComboBoxCellTemplate.Sorted = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewComboBoxCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewComboBoxCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.Sorted = value;
                            }
                        }
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxColumn.uex' path='docs/doc[@for="DataGridViewComboBoxColumn.Clone"]/*' />
        public override object Clone()
        {
            DataGridViewComboBoxColumn dataGridViewColumn;
            Type thisType = this.GetType();

            if (thisType == columnType) //performance improvement
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
                ((DataGridViewComboBoxCell) dataGridViewColumn.CellTemplate).TemplateComboBoxColumn = dataGridViewColumn;
            }
            return dataGridViewColumn;
        }

        internal void OnItemsCollectionChanged()
        {
            // Items collection of the CellTemplate was changed.
            // Update the items collection of each existing DataGridViewComboBoxCell in the column.
            if (this.DataGridView != null)
            {
                DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                int rowCount = dataGridViewRows.Count;
                object[] items = ((DataGridViewComboBoxCell)this.CellTemplate).Items.InnerArray.ToArray();
                for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                {
                    DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                    DataGridViewComboBoxCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewComboBoxCell;
                    if (dataGridViewCell != null)
                    {
                        dataGridViewCell.Items.ClearInternal();
                        dataGridViewCell.Items.AddRangeInternal(items);
                    }
                }
                this.DataGridView.OnColumnCommonChange(this.Index);
            }
        }

        /// <include file='doc\DataGridViewComboBoxColumn.uex' path='docs/doc[@for="DataGridViewComboBoxColumn.ToString"]/*' />
        /// <devdoc>
        ///    <para></para>
        /// </devdoc>
        public override string ToString() 
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append("DataGridViewComboBoxColumn { Name=");
            sb.Append(this.Name);
            sb.Append(", Index=");
            sb.Append(this.Index.ToString(CultureInfo.CurrentCulture));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}

﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Text;
    using System.Drawing;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Globalization;

    [ToolboxBitmapAttribute(typeof(DataGridViewLinkColumn), "DataGridViewLinkColumn")]
    public class DataGridViewLinkColumn : DataGridViewColumn
    {
        private static Type columnType = typeof(DataGridViewLinkColumn);

        private string text;

        public DataGridViewLinkColumn() : base(new DataGridViewLinkCell())
        {
        }

        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_LinkColumnActiveLinkColorDescr))
        ]
        public Color ActiveLinkColor
        {
            get
            {
                if (this.CellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return ((DataGridViewLinkCell)this.CellTemplate).ActiveLinkColor;
            }
            set
            {
                if (!this.ActiveLinkColor.Equals(value))
                {
                    ((DataGridViewLinkCell)this.CellTemplate).ActiveLinkColorInternal = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewLinkCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewLinkCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.ActiveLinkColorInternal = value;
                            }
                        }
                        this.DataGridView.InvalidateColumn(this.Index);
                    }
                }
            }
        }

        private bool ShouldSerializeActiveLinkColor()
        {
            if (SystemInformation.HighContrast)
            {
                return !this.ActiveLinkColor.Equals(SystemColors.HotTrack);
            }

            return !this.ActiveLinkColor.Equals(LinkUtilities.IEActiveLinkColor);
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
                if (value != null && !(value is System.Windows.Forms.DataGridViewLinkCell))
                {
                    throw new InvalidCastException(string.Format(SR.DataGridViewTypeColumn_WrongCellTemplateType, "System.Windows.Forms.DataGridViewLinkCell"));
                }
                base.CellTemplate = value;
            }
        }

        [
            DefaultValue(LinkBehavior.SystemDefault),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_LinkColumnLinkBehaviorDescr))
        ]
        public LinkBehavior LinkBehavior
        {
            get
            {
                if (this.CellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return ((DataGridViewLinkCell)this.CellTemplate).LinkBehavior;
            }
            set
            {
                if (!this.LinkBehavior.Equals(value))
                {
                    ((DataGridViewLinkCell)this.CellTemplate).LinkBehavior = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewLinkCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewLinkCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.LinkBehaviorInternal = value;
                            }
                        }
                        this.DataGridView.InvalidateColumn(this.Index);
                    }
                }
            }
        }

        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_LinkColumnLinkColorDescr))
        ]
        public Color LinkColor
        {
            get
            {
                if (this.CellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return ((DataGridViewLinkCell)this.CellTemplate).LinkColor;
            }
            set
            {
                if (!this.LinkColor.Equals(value))
                {
                    ((DataGridViewLinkCell)this.CellTemplate).LinkColorInternal = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewLinkCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewLinkCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.LinkColorInternal = value;
                            }
                        }
                        this.DataGridView.InvalidateColumn(this.Index);
                    }
                }
            }
        }

        private bool ShouldSerializeLinkColor()
        {
            if (SystemInformation.HighContrast)
            {
                return !this.LinkColor.Equals(SystemColors.HotTrack);
            }

            return !this.LinkColor.Equals(LinkUtilities.IELinkColor);
        }

        [
            DefaultValue(null),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_LinkColumnTextDescr))
        ]
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                if (!string.Equals(value, this.text, StringComparison.Ordinal))
                {
                    this.text = value;
                    if (this.DataGridView != null)
                    {
                        if (this.UseColumnTextForLinkValue)
                        {
                            this.DataGridView.OnColumnCommonChange(this.Index);
                        }
                        else
                        {
                            DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                            int rowCount = dataGridViewRows.Count;
                            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                            {
                                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                                DataGridViewLinkCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewLinkCell;
                                if (dataGridViewCell != null && dataGridViewCell.UseColumnTextForLinkValue)
                                {
                                    this.DataGridView.OnColumnCommonChange(this.Index);
                                    return;
                                }
                            }
                            this.DataGridView.InvalidateColumn(this.Index);
                        }
                    }
                }
            }
        }

        [
            DefaultValue(true),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_LinkColumnTrackVisitedStateDescr))
        ]
        public bool TrackVisitedState
        {
            get
            {
                if (this.CellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return ((DataGridViewLinkCell)this.CellTemplate).TrackVisitedState;
            }
            set
            {
                if (this.TrackVisitedState != value)
                {
                    ((DataGridViewLinkCell)this.CellTemplate).TrackVisitedStateInternal = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewLinkCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewLinkCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.TrackVisitedStateInternal = value;
                            }
                        }
                        this.DataGridView.InvalidateColumn(this.Index);
                    }
                }
            }
        }

        [
            DefaultValue(false),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_LinkColumnUseColumnTextForLinkValueDescr))
        ]
        public bool UseColumnTextForLinkValue
        {
            get
            {
                if (this.CellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return ((DataGridViewLinkCell)this.CellTemplate).UseColumnTextForLinkValue;
            }
            set
            {
                if (this.UseColumnTextForLinkValue != value)
                {
                    ((DataGridViewLinkCell)this.CellTemplate).UseColumnTextForLinkValueInternal = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewLinkCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewLinkCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.UseColumnTextForLinkValueInternal = value;
                            }
                        }
                        this.DataGridView.OnColumnCommonChange(this.Index);
                    }
                }
            }
        }

        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_LinkColumnVisitedLinkColorDescr))
        ]
        public Color VisitedLinkColor
        {
            get
            {
                if (this.CellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_CellTemplateRequired));
                }
                return ((DataGridViewLinkCell)this.CellTemplate).VisitedLinkColor;
            }
            set
            {
                if (!this.VisitedLinkColor.Equals(value))
                {
                    ((DataGridViewLinkCell)this.CellTemplate).VisitedLinkColorInternal = value;
                    if (this.DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = this.DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewLinkCell dataGridViewCell = dataGridViewRow.Cells[this.Index] as DataGridViewLinkCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.VisitedLinkColorInternal = value;
                            }
                        }
                        this.DataGridView.InvalidateColumn(this.Index);
                    }
                }
            }
        }

        private bool ShouldSerializeVisitedLinkColor()
        {
            if (SystemInformation.HighContrast)
            {
                return !this.VisitedLinkColor.Equals(SystemColors.HotTrack);
            }

            return !this.VisitedLinkColor.Equals(LinkUtilities.IEVisitedLinkColor);
        }

        public override object Clone()
        {
            DataGridViewLinkColumn dataGridViewColumn;
            Type thisType = this.GetType();

            if (thisType == columnType) //performance improvement
            {
                dataGridViewColumn = new DataGridViewLinkColumn();
            }
            else
            {
                // 

                dataGridViewColumn = (DataGridViewLinkColumn)System.Activator.CreateInstance(thisType);
            }
            if (dataGridViewColumn != null)
            {
                base.CloneInternal(dataGridViewColumn);
                dataGridViewColumn.Text = this.text;
            }
            return dataGridViewColumn;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append("DataGridViewLinkColumn { Name=");
            sb.Append(this.Name);
            sb.Append(", Index=");
            sb.Append(this.Index.ToString(CultureInfo.CurrentCulture));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}

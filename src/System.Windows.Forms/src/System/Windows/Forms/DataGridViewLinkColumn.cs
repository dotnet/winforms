// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace System.Windows.Forms
{
    [ToolboxBitmap(typeof(DataGridViewLinkColumn), "DataGridViewLinkColumn")]
    public class DataGridViewLinkColumn : DataGridViewColumn
    {
        private static readonly Type columnType = typeof(DataGridViewLinkColumn);

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
                if (CellTemplate == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                return ((DataGridViewLinkCell)CellTemplate).ActiveLinkColor;
            }
            set
            {
                if (!ActiveLinkColor.Equals(value))
                {
                    ((DataGridViewLinkCell)CellTemplate).ActiveLinkColorInternal = value;
                    if (DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            if (dataGridViewRow.Cells[Index] is DataGridViewLinkCell dataGridViewCell)
                            {
                                dataGridViewCell.ActiveLinkColorInternal = value;
                            }
                        }
                        DataGridView.InvalidateColumn(Index);
                    }
                }
            }
        }

        private bool ShouldSerializeActiveLinkColor()
        {
            if (SystemInformation.HighContrast)
            {
                return !ActiveLinkColor.Equals(SystemColors.HotTrack);
            }

            return !ActiveLinkColor.Equals(LinkUtilities.IEActiveLinkColor);
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
                if (value != null && !(value is DataGridViewLinkCell))
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
                if (CellTemplate == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                return ((DataGridViewLinkCell)CellTemplate).LinkBehavior;
            }
            set
            {
                if (!LinkBehavior.Equals(value))
                {
                    ((DataGridViewLinkCell)CellTemplate).LinkBehavior = value;
                    if (DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            if (dataGridViewRow.Cells[Index] is DataGridViewLinkCell dataGridViewCell)
                            {
                                dataGridViewCell.LinkBehaviorInternal = value;
                            }
                        }
                        DataGridView.InvalidateColumn(Index);
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
                if (CellTemplate == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                return ((DataGridViewLinkCell)CellTemplate).LinkColor;
            }
            set
            {
                if (!LinkColor.Equals(value))
                {
                    ((DataGridViewLinkCell)CellTemplate).LinkColorInternal = value;
                    if (DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            if (dataGridViewRow.Cells[Index] is DataGridViewLinkCell dataGridViewCell)
                            {
                                dataGridViewCell.LinkColorInternal = value;
                            }
                        }
                        DataGridView.InvalidateColumn(Index);
                    }
                }
            }
        }

        private bool ShouldSerializeLinkColor()
        {
            if (SystemInformation.HighContrast)
            {
                return !LinkColor.Equals(SystemColors.HotTrack);
            }

            return !LinkColor.Equals(LinkUtilities.IELinkColor);
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
                return text;
            }
            set
            {
                if (!string.Equals(value, text, StringComparison.Ordinal))
                {
                    text = value;
                    if (DataGridView != null)
                    {
                        if (UseColumnTextForLinkValue)
                        {
                            DataGridView.OnColumnCommonChange(Index);
                        }
                        else
                        {
                            DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
                            int rowCount = dataGridViewRows.Count;
                            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                            {
                                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                                if (dataGridViewRow.Cells[Index] is DataGridViewLinkCell dataGridViewCell && dataGridViewCell.UseColumnTextForLinkValue)
                                {
                                    DataGridView.OnColumnCommonChange(Index);
                                    return;
                                }
                            }
                            DataGridView.InvalidateColumn(Index);
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
                if (CellTemplate == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                return ((DataGridViewLinkCell)CellTemplate).TrackVisitedState;
            }
            set
            {
                if (TrackVisitedState != value)
                {
                    ((DataGridViewLinkCell)CellTemplate).TrackVisitedStateInternal = value;
                    if (DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            if (dataGridViewRow.Cells[Index] is DataGridViewLinkCell dataGridViewCell)
                            {
                                dataGridViewCell.TrackVisitedStateInternal = value;
                            }
                        }
                        DataGridView.InvalidateColumn(Index);
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
                if (CellTemplate == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                return ((DataGridViewLinkCell)CellTemplate).UseColumnTextForLinkValue;
            }
            set
            {
                if (UseColumnTextForLinkValue != value)
                {
                    ((DataGridViewLinkCell)CellTemplate).UseColumnTextForLinkValueInternal = value;
                    if (DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            if (dataGridViewRow.Cells[Index] is DataGridViewLinkCell dataGridViewCell)
                            {
                                dataGridViewCell.UseColumnTextForLinkValueInternal = value;
                            }
                        }
                        DataGridView.OnColumnCommonChange(Index);
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
                if (CellTemplate == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
                }
                return ((DataGridViewLinkCell)CellTemplate).VisitedLinkColor;
            }
            set
            {
                if (!VisitedLinkColor.Equals(value))
                {
                    ((DataGridViewLinkCell)CellTemplate).VisitedLinkColorInternal = value;
                    if (DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            if (dataGridViewRow.Cells[Index] is DataGridViewLinkCell dataGridViewCell)
                            {
                                dataGridViewCell.VisitedLinkColorInternal = value;
                            }
                        }
                        DataGridView.InvalidateColumn(Index);
                    }
                }
            }
        }

        private bool ShouldSerializeVisitedLinkColor()
        {
            if (SystemInformation.HighContrast)
            {
                return !VisitedLinkColor.Equals(SystemColors.HotTrack);
            }

            return !VisitedLinkColor.Equals(LinkUtilities.IEVisitedLinkColor);
        }

        public override object Clone()
        {
            DataGridViewLinkColumn dataGridViewColumn;
            Type thisType = GetType();

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
                dataGridViewColumn.Text = text;
            }
            return dataGridViewColumn;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append("DataGridViewLinkColumn { Name=");
            sb.Append(Name);
            sb.Append(", Index=");
            sb.Append(Index.ToString(CultureInfo.CurrentCulture));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
[ToolboxBitmap(typeof(DataGridViewLinkColumn), "DataGridViewLinkColumn")]
public class DataGridViewLinkColumn : DataGridViewColumn
{
    private static readonly Type s_columnType = typeof(DataGridViewLinkColumn);

    private string? _text;

    public DataGridViewLinkColumn()
        : base(new DataGridViewLinkCell())
    {
    }

    private DataGridViewLinkCell? LinkCellTemplate => (DataGridViewLinkCell?)CellTemplate;

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_LinkColumnActiveLinkColorDescr))]
    [MemberNotNull(nameof(LinkCellTemplate))]
    public Color ActiveLinkColor
    {
        get
        {
            if (LinkCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return LinkCellTemplate.ActiveLinkColor;
        }
        set
        {
            if (ActiveLinkColor.Equals(value))
            {
                return;
            }

            LinkCellTemplate.ActiveLinkColorInternal = value;
            if (DataGridView is null)
            {
                return;
            }

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

    private bool ShouldSerializeActiveLinkColor()
    {
        if (SystemInformation.HighContrast)
        {
            return !ActiveLinkColor.Equals(SystemColors.HotTrack);
        }

        return !ActiveLinkColor.Equals(LinkUtilities.IEActiveLinkColor);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override DataGridViewCell? CellTemplate
    {
        get => base.CellTemplate;
        set
        {
            if (value is not null and not DataGridViewLinkCell)
            {
                throw new InvalidCastException(string.Format(SR.DataGridViewTypeColumn_WrongCellTemplateType, "System.Windows.Forms.DataGridViewLinkCell"));
            }

            base.CellTemplate = value;
        }
    }

    [DefaultValue(LinkBehavior.SystemDefault)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_LinkColumnLinkBehaviorDescr))]
    [MemberNotNull(nameof(LinkCellTemplate))]
    public LinkBehavior LinkBehavior
    {
        get
        {
            if (LinkCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return LinkCellTemplate.LinkBehavior;
        }
        set
        {
            if (LinkBehavior.Equals(value))
            {
                return;
            }

            LinkCellTemplate.LinkBehavior = value;
            if (DataGridView is null)
            {
                return;
            }

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

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_LinkColumnLinkColorDescr))]
    [MemberNotNull(nameof(LinkCellTemplate))]
    public Color LinkColor
    {
        get
        {
            if (LinkCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return LinkCellTemplate.LinkColor;
        }
        set
        {
            if (LinkColor.Equals(value))
            {
                return;
            }

            LinkCellTemplate.LinkColorInternal = value;
            if (DataGridView is null)
            {
                return;
            }

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

    private bool ShouldSerializeLinkColor()
    {
        if (SystemInformation.HighContrast)
        {
            return !LinkColor.Equals(SystemColors.HotTrack);
        }

        return !LinkColor.Equals(LinkUtilities.IELinkColor);
    }

    [DefaultValue(null)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_LinkColumnTextDescr))]
    public string? Text
    {
        get => _text;
        set
        {
            if (string.Equals(value, _text, StringComparison.Ordinal))
            {
                return;
            }

            _text = value;
            if (DataGridView is null)
            {
                return;
            }

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

    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_LinkColumnTrackVisitedStateDescr))]
    [MemberNotNull(nameof(LinkCellTemplate))]
    public bool TrackVisitedState
    {
        get
        {
            if (LinkCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return LinkCellTemplate.TrackVisitedState;
        }
        set
        {
            if (TrackVisitedState == value)
            {
                return;
            }

            LinkCellTemplate.TrackVisitedStateInternal = value;
            if (DataGridView is null)
            {
                return;
            }

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

    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_LinkColumnUseColumnTextForLinkValueDescr))]
    [MemberNotNull(nameof(LinkCellTemplate))]
    public bool UseColumnTextForLinkValue
    {
        get
        {
            if (LinkCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return LinkCellTemplate.UseColumnTextForLinkValue;
        }
        set
        {
            if (UseColumnTextForLinkValue == value)
            {
                return;
            }

            LinkCellTemplate.UseColumnTextForLinkValueInternal = value;
            if (DataGridView is null)
            {
                return;
            }

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

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_LinkColumnVisitedLinkColorDescr))]
    [MemberNotNull(nameof(LinkCellTemplate))]
    public Color VisitedLinkColor
    {
        get
        {
            if (LinkCellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridViewColumn_CellTemplateRequired);
            }

            return LinkCellTemplate.VisitedLinkColor;
        }
        set
        {
            if (VisitedLinkColor.Equals(value))
            {
                return;
            }

            LinkCellTemplate.VisitedLinkColorInternal = value;
            if (DataGridView is null)
            {
                return;
            }

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

        if (thisType == s_columnType) // performance improvement
        {
            dataGridViewColumn = new DataGridViewLinkColumn();
        }
        else
        {
            dataGridViewColumn = (DataGridViewLinkColumn)Activator.CreateInstance(thisType)!;
        }

        CloneInternal(dataGridViewColumn);
        dataGridViewColumn.Text = _text;

        return dataGridViewColumn;
    }

    public override string ToString() =>
        $"DataGridViewLinkColumn {{ Name={Name}, Index={Index} }}";
}

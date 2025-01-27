// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  Identifies a row in the dataGridView.
/// </summary>
[TypeConverter(typeof(DataGridViewRowConverter))]
public partial class DataGridViewRow : DataGridViewBand
{
    private static readonly Type s_rowType = typeof(DataGridViewRow);
    private static int s_defaultHeight = -1;
    private static readonly int s_propRowErrorText = PropertyStore.CreateKey();
    private static readonly int s_propRowAccessibilityObject = PropertyStore.CreateKey();

    private const DataGridViewAutoSizeRowCriteriaInternal InvalidDataGridViewAutoSizeRowCriteriaInternalMask =
        ~(DataGridViewAutoSizeRowCriteriaInternal.Header | DataGridViewAutoSizeRowCriteriaInternal.AllColumns);

    private const int DefaultMinRowThickness = 3;

    private DataGridViewCellCollection? _rowCells;

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataGridViewRow"/> class.
    /// </summary>
    public DataGridViewRow() : base()
    {
        MinimumThickness = DefaultMinRowThickness;
        Thickness = DefaultHeight;
    }

    [Browsable(false)]
    public AccessibleObject AccessibilityObject
    {
        get
        {
            if (!Properties.TryGetValue(s_propRowAccessibilityObject, out AccessibleObject? result))
            {
                result = CreateAccessibilityInstance();
                Properties.AddValue(s_propRowAccessibilityObject, result);
            }

            return result;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public DataGridViewCellCollection Cells => _rowCells ??= CreateCellsInstance();

    [DefaultValue(null)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_RowContextMenuStripDescr))]
    public override ContextMenuStrip? ContextMenuStrip
    {
        get => base.ContextMenuStrip;
        set => base.ContextMenuStrip = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public object? DataBoundItem
    {
        get
        {
            if (DataGridView is not null
                && DataGridView.DataConnection is not null
                && DataGridView.DataConnection.CurrencyManager is not null
                && Index > -1
                && Index != DataGridView.NewRowIndex)
            {
                return DataGridView.DataConnection.CurrencyManager[Index];
            }
            else
            {
                return null;
            }
        }
    }

    [Browsable(true)]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_RowDefaultCellStyleDescr))]
    [AllowNull]
    public override DataGridViewCellStyle DefaultCellStyle
    {
        get => base.DefaultCellStyle;
        set
        {
            if (DataGridView is not null && Index == -1)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, nameof(DefaultCellStyle)));
            }

            base.DefaultCellStyle = value;
        }
    }

    [Browsable(false)]
    public override bool Displayed
    {
        get
        {
            if (DataGridView is not null && Index == -1)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedRow, nameof(Displayed)));
            }

            return GetDisplayed(Index);
        }
    }

    [DefaultValue(0)]
    [NotifyParentProperty(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_RowDividerHeightDescr))]
    public int DividerHeight
    {
        get => DividerThickness;
        set
        {
            if (DataGridView is not null && Index == -1)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, nameof(DividerHeight)));
            }

            DividerThickness = value;
        }
    }

    [DefaultValue("")]
    [NotifyParentProperty(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_RowErrorTextDescr))]
    public string ErrorText
    {
        get
        {
            Debug.Assert(Index >= -1);
            return GetErrorText(Index);
        }
        set => ErrorTextInternal = value;
    }

    private static int DefaultHeight
    {
        get
        {
            if (s_defaultHeight == -1)
            {
                s_defaultHeight = Control.DefaultFont.Height + 9;
            }

            return s_defaultHeight;
        }
    }

    private string ErrorTextInternal
    {
        get => Properties.GetStringOrEmptyString(s_propRowErrorText);
        set
        {
            if (Properties.AddOrRemoveString(s_propRowErrorText, value))
            {
                DataGridView?.OnRowErrorTextChanged(this);
            }
        }
    }

    [Browsable(false)]
    public override bool Frozen
    {
        get
        {
            if (DataGridView is not null && Index == -1)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedRow, nameof(Frozen)));
            }

            return GetFrozen(Index);
        }
        set
        {
            if (DataGridView is not null && Index == -1)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, nameof(Frozen)));
            }

            base.Frozen = value;
        }
    }

    private bool HasErrorText => Properties.ContainsKey(s_propRowErrorText);

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public DataGridViewRowHeaderCell HeaderCell
    {
        get => (DataGridViewRowHeaderCell)HeaderCellCore;
        set => HeaderCellCore = value;
    }

    [NotifyParentProperty(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_RowHeightDescr))]
    public int Height
    {
        get => Thickness;
        set
        {
            if (DataGridView is not null && Index == -1)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, nameof(Height)));
            }

            Thickness = value;
        }
    }

    public override DataGridViewCellStyle InheritedStyle
    {
        get
        {
            if (Index == -1)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedRow, nameof(InheritedStyle)));
            }

            DataGridViewCellStyle inheritedRowStyle = new();
            BuildInheritedRowStyle(Index, inheritedRowStyle);
            return inheritedRowStyle;
        }
    }

    internal bool IsAccessibilityObjectCreated => Properties.ContainsKey(s_propRowAccessibilityObject);

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsNewRow
    {
        get => DataGridView is not null && DataGridView.NewRowIndex == Index;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MinimumHeight
    {
        get => MinimumThickness;
        set
        {
            if (DataGridView is not null && Index == -1)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, nameof(MinimumHeight)));
            }

            MinimumThickness = value;
        }
    }

    [Browsable(true)]
    [DefaultValue(false)]
    [NotifyParentProperty(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_RowReadOnlyDescr))]
    public override bool ReadOnly
    {
        get
        {
            if (DataGridView is not null && Index == -1)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedRow, nameof(ReadOnly)));
            }

            return GetReadOnly(Index);
        }
        set => base.ReadOnly = value;
    }

    [NotifyParentProperty(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_RowResizableDescr))]
    public override DataGridViewTriState Resizable
    {
        get
        {
            if (DataGridView is not null && Index == -1)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedRow, nameof(Resizable)));
            }

            return GetResizable(Index);
        }
        set => base.Resizable = value;
    }

    public override bool Selected
    {
        get
        {
            if (DataGridView is not null && Index == -1)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedRow, nameof(Selected)));
            }

            return GetSelected(Index);
        }
        set => base.Selected = value;
    }

    public override DataGridViewElementStates State
    {
        get
        {
            if (DataGridView is not null && Index == -1)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedRow, nameof(State)));
            }

            return GetState(Index);
        }
    }

    [Browsable(false)]
    public override bool Visible
    {
        get
        {
            if (DataGridView is not null && Index == -1)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedRow, nameof(Visible)));
            }

            return GetVisible(Index);
        }
        set
        {
            if (DataGridView is not null && Index == -1)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, nameof(Visible)));
            }

            base.Visible = value;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual DataGridViewAdvancedBorderStyle AdjustRowHeaderBorderStyle(
        DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStyleInput,
        DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder,
        bool singleVerticalBorderAdded,
        bool singleHorizontalBorderAdded,
        bool isFirstDisplayedRow,
        bool isLastVisibleRow)
    {
        if (DataGridView is not null && DataGridView.ApplyVisualStylesToHeaderCells)
        {
            switch (dataGridViewAdvancedBorderStyleInput.All)
            {
                case DataGridViewAdvancedCellBorderStyle.Inset:
                    if (isFirstDisplayedRow && !DataGridView.ColumnHeadersVisible)
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                    }
                    else
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.None;
                    }

                    dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                    dataGridViewAdvancedBorderStylePlaceholder.RightInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                    dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = DataGridViewAdvancedCellBorderStyle.None;
                    return dataGridViewAdvancedBorderStylePlaceholder;

                case DataGridViewAdvancedCellBorderStyle.Outset:
                    if (isFirstDisplayedRow && !DataGridView.ColumnHeadersVisible)
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                    }
                    else
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.None;
                    }

                    dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                    dataGridViewAdvancedBorderStylePlaceholder.RightInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                    dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = DataGridViewAdvancedCellBorderStyle.None;
                    return dataGridViewAdvancedBorderStylePlaceholder;

                case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                    if (isFirstDisplayedRow && !DataGridView.ColumnHeadersVisible)
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                    }
                    else
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.None;
                    }

                    if (DataGridView.RightToLeftInternal)
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                    }
                    else
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                    }

                    dataGridViewAdvancedBorderStylePlaceholder.RightInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                    dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = DataGridViewAdvancedCellBorderStyle.None;
                    return dataGridViewAdvancedBorderStylePlaceholder;

                case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                    if (isFirstDisplayedRow && !DataGridView.ColumnHeadersVisible)
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                    }
                    else
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.None;
                    }

                    if (DataGridView.RightToLeftInternal)
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                    }
                    else
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                    }

                    dataGridViewAdvancedBorderStylePlaceholder.RightInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                    dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = DataGridViewAdvancedCellBorderStyle.None;
                    return dataGridViewAdvancedBorderStylePlaceholder;

                case DataGridViewAdvancedCellBorderStyle.InsetDouble:
                    if (isFirstDisplayedRow && !DataGridView.ColumnHeadersVisible)
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                    }
                    else
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.None;
                    }

                    if (DataGridView.RightToLeftInternal)
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                    }
                    else
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                    }

                    dataGridViewAdvancedBorderStylePlaceholder.RightInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                    dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = DataGridViewAdvancedCellBorderStyle.None;
                    return dataGridViewAdvancedBorderStylePlaceholder;

                case DataGridViewAdvancedCellBorderStyle.Single:
                    if (isFirstDisplayedRow && !DataGridView.ColumnHeadersVisible)
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.Single;
                    }
                    else
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.None;
                    }

                    dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.Single;
                    dataGridViewAdvancedBorderStylePlaceholder.RightInternal = DataGridViewAdvancedCellBorderStyle.Single;
                    dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = DataGridViewAdvancedCellBorderStyle.None;
                    return dataGridViewAdvancedBorderStylePlaceholder;
            }
        }
        else
        {
            switch (dataGridViewAdvancedBorderStyleInput.All)
            {
                case DataGridViewAdvancedCellBorderStyle.Inset:
                    if (isFirstDisplayedRow && singleHorizontalBorderAdded)
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                        dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                        dataGridViewAdvancedBorderStylePlaceholder.RightInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                        return dataGridViewAdvancedBorderStylePlaceholder;
                    }

                    break;

                case DataGridViewAdvancedCellBorderStyle.Outset:
                    if (isFirstDisplayedRow && singleHorizontalBorderAdded)
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                        dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                        dataGridViewAdvancedBorderStylePlaceholder.RightInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                        return dataGridViewAdvancedBorderStylePlaceholder;
                    }

                    break;

                case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                    if (DataGridView is not null && DataGridView.RightToLeftInternal)
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                        dataGridViewAdvancedBorderStylePlaceholder.RightInternal = DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                    }
                    else
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                        dataGridViewAdvancedBorderStylePlaceholder.RightInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                    }

                    if (isFirstDisplayedRow)
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridView is not null && DataGridView.ColumnHeadersVisible ? DataGridViewAdvancedCellBorderStyle.Outset : DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                    }
                    else
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.OutsetPartial;
                    }

                    dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = isLastVisibleRow ? DataGridViewAdvancedCellBorderStyle.Outset : DataGridViewAdvancedCellBorderStyle.OutsetPartial;
                    return dataGridViewAdvancedBorderStylePlaceholder;

                case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                    if (DataGridView is not null && DataGridView.RightToLeftInternal)
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                        dataGridViewAdvancedBorderStylePlaceholder.RightInternal = DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                    }
                    else
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                        dataGridViewAdvancedBorderStylePlaceholder.RightInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                    }

                    if (isFirstDisplayedRow)
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridView is not null && DataGridView.ColumnHeadersVisible ? DataGridViewAdvancedCellBorderStyle.Outset : DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                    }
                    else
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                    }

                    dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                    return dataGridViewAdvancedBorderStylePlaceholder;

                case DataGridViewAdvancedCellBorderStyle.InsetDouble:
                    if (DataGridView is not null && DataGridView.RightToLeftInternal)
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                        dataGridViewAdvancedBorderStylePlaceholder.RightInternal = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                    }
                    else
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                        dataGridViewAdvancedBorderStylePlaceholder.RightInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                    }

                    if (isFirstDisplayedRow)
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridView is not null && DataGridView.ColumnHeadersVisible ? DataGridViewAdvancedCellBorderStyle.Inset : DataGridViewAdvancedCellBorderStyle.InsetDouble;
                    }
                    else
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                    }

                    dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                    return dataGridViewAdvancedBorderStylePlaceholder;

                case DataGridViewAdvancedCellBorderStyle.Single:
                    if (!isFirstDisplayedRow || (DataGridView is not null && DataGridView.ColumnHeadersVisible))
                    {
                        dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.Single;
                        dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.None;
                        dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = DataGridViewAdvancedCellBorderStyle.Single;
                        dataGridViewAdvancedBorderStylePlaceholder.RightInternal = DataGridViewAdvancedCellBorderStyle.Single;
                        return dataGridViewAdvancedBorderStylePlaceholder;
                    }

                    break;
            }
        }

        return dataGridViewAdvancedBorderStyleInput;
    }

    private void BuildInheritedRowHeaderCellStyle(DataGridViewCellStyle inheritedCellStyle)
    {
        Debug.Assert(inheritedCellStyle is not null);

        DataGridViewCellStyle? cellStyle = null;
        if (HeaderCell.HasStyle)
        {
            cellStyle = HeaderCell.Style;
            Debug.Assert(cellStyle is not null);
        }

        DataGridViewCellStyle rowHeadersStyle = DataGridView!.RowHeadersDefaultCellStyle;
        Debug.Assert(rowHeadersStyle is not null);

        DataGridViewCellStyle dataGridViewStyle = DataGridView.DefaultCellStyle;
        Debug.Assert(dataGridViewStyle is not null);

        if (cellStyle is not null && !cellStyle.BackColor.IsEmpty)
        {
            inheritedCellStyle.BackColor = cellStyle.BackColor;
        }
        else if (!rowHeadersStyle.BackColor.IsEmpty)
        {
            inheritedCellStyle.BackColor = rowHeadersStyle.BackColor;
        }
        else
        {
            inheritedCellStyle.BackColor = dataGridViewStyle.BackColor;
        }

        if (cellStyle is not null && !cellStyle.ForeColor.IsEmpty)
        {
            inheritedCellStyle.ForeColor = cellStyle.ForeColor;
        }
        else if (!rowHeadersStyle.ForeColor.IsEmpty)
        {
            inheritedCellStyle.ForeColor = rowHeadersStyle.ForeColor;
        }
        else
        {
            inheritedCellStyle.ForeColor = dataGridViewStyle.ForeColor;
        }

        if (cellStyle is not null && !cellStyle.SelectionBackColor.IsEmpty)
        {
            inheritedCellStyle.SelectionBackColor = cellStyle.SelectionBackColor;
        }
        else if (!rowHeadersStyle.SelectionBackColor.IsEmpty)
        {
            inheritedCellStyle.SelectionBackColor = rowHeadersStyle.SelectionBackColor;
        }
        else
        {
            inheritedCellStyle.SelectionBackColor = dataGridViewStyle.SelectionBackColor;
        }

        if (cellStyle is not null && !cellStyle.SelectionForeColor.IsEmpty)
        {
            inheritedCellStyle.SelectionForeColor = cellStyle.SelectionForeColor;
        }
        else if (!rowHeadersStyle.SelectionForeColor.IsEmpty)
        {
            inheritedCellStyle.SelectionForeColor = rowHeadersStyle.SelectionForeColor;
        }
        else
        {
            inheritedCellStyle.SelectionForeColor = dataGridViewStyle.SelectionForeColor;
        }

        if (cellStyle is not null && cellStyle.Font is not null)
        {
            inheritedCellStyle.Font = cellStyle.Font;
        }
        else if (rowHeadersStyle.Font is not null)
        {
            inheritedCellStyle.Font = rowHeadersStyle.Font;
        }
        else
        {
            inheritedCellStyle.Font = dataGridViewStyle.Font;
        }

        if (cellStyle is not null && !cellStyle.IsNullValueDefault)
        {
            inheritedCellStyle.NullValue = cellStyle.NullValue;
        }
        else if (!rowHeadersStyle.IsNullValueDefault)
        {
            inheritedCellStyle.NullValue = rowHeadersStyle.NullValue;
        }
        else
        {
            inheritedCellStyle.NullValue = dataGridViewStyle.NullValue;
        }

        if (cellStyle is not null && !cellStyle.IsDataSourceNullValueDefault)
        {
            inheritedCellStyle.DataSourceNullValue = cellStyle.DataSourceNullValue;
        }
        else if (!rowHeadersStyle.IsDataSourceNullValueDefault)
        {
            inheritedCellStyle.DataSourceNullValue = rowHeadersStyle.DataSourceNullValue;
        }
        else
        {
            inheritedCellStyle.DataSourceNullValue = dataGridViewStyle.DataSourceNullValue;
        }

        if (cellStyle is not null && cellStyle.Format.Length != 0)
        {
            inheritedCellStyle.Format = cellStyle.Format;
        }
        else if (rowHeadersStyle.Format.Length != 0)
        {
            inheritedCellStyle.Format = rowHeadersStyle.Format;
        }
        else
        {
            inheritedCellStyle.Format = dataGridViewStyle.Format;
        }

        if (cellStyle is not null && !cellStyle.IsFormatProviderDefault)
        {
            inheritedCellStyle.FormatProvider = cellStyle.FormatProvider;
        }
        else if (!rowHeadersStyle.IsFormatProviderDefault)
        {
            inheritedCellStyle.FormatProvider = rowHeadersStyle.FormatProvider;
        }
        else
        {
            inheritedCellStyle.FormatProvider = dataGridViewStyle.FormatProvider;
        }

        if (cellStyle is not null && cellStyle.Alignment != DataGridViewContentAlignment.NotSet)
        {
            inheritedCellStyle.AlignmentInternal = cellStyle.Alignment;
        }
        else if (rowHeadersStyle is not null && rowHeadersStyle.Alignment != DataGridViewContentAlignment.NotSet)
        {
            inheritedCellStyle.AlignmentInternal = rowHeadersStyle.Alignment;
        }
        else
        {
            Debug.Assert(dataGridViewStyle.Alignment != DataGridViewContentAlignment.NotSet);
            inheritedCellStyle.AlignmentInternal = dataGridViewStyle.Alignment;
        }

        if (cellStyle is not null && cellStyle.WrapMode != DataGridViewTriState.NotSet)
        {
            inheritedCellStyle.WrapModeInternal = cellStyle.WrapMode;
        }
        else if (rowHeadersStyle is not null && rowHeadersStyle.WrapMode != DataGridViewTriState.NotSet)
        {
            inheritedCellStyle.WrapModeInternal = rowHeadersStyle.WrapMode;
        }
        else
        {
            Debug.Assert(dataGridViewStyle.WrapMode != DataGridViewTriState.NotSet);
            inheritedCellStyle.WrapModeInternal = dataGridViewStyle.WrapMode;
        }

        if (cellStyle is not null && cellStyle.Tag is not null)
        {
            inheritedCellStyle.Tag = cellStyle.Tag;
        }
        else if (rowHeadersStyle?.Tag is not null)
        {
            inheritedCellStyle.Tag = rowHeadersStyle.Tag;
        }
        else
        {
            inheritedCellStyle.Tag = dataGridViewStyle.Tag;
        }

        if (cellStyle is not null && cellStyle.Padding != Padding.Empty)
        {
            inheritedCellStyle.PaddingInternal = cellStyle.Padding;
        }
        else if (rowHeadersStyle is not null && rowHeadersStyle.Padding != Padding.Empty)
        {
            inheritedCellStyle.PaddingInternal = rowHeadersStyle.Padding;
        }
        else
        {
            inheritedCellStyle.PaddingInternal = dataGridViewStyle.Padding;
        }
    }

    private void BuildInheritedRowStyle(int rowIndex, DataGridViewCellStyle inheritedRowStyle)
    {
        Debug.Assert(inheritedRowStyle is not null);
        Debug.Assert(rowIndex >= 0);
        Debug.Assert(DataGridView is not null);

        DataGridViewCellStyle? rowStyle = null;
        if (HasDefaultCellStyle)
        {
            rowStyle = DefaultCellStyle;
            Debug.Assert(rowStyle is not null);
        }

        DataGridViewCellStyle dataGridViewStyle = DataGridView.DefaultCellStyle;
        Debug.Assert(dataGridViewStyle is not null);

        DataGridViewCellStyle rowsDefaultCellStyle = DataGridView.RowsDefaultCellStyle;
        Debug.Assert(rowsDefaultCellStyle is not null);

        DataGridViewCellStyle alternatingRowsDefaultCellStyle = DataGridView.AlternatingRowsDefaultCellStyle;
        Debug.Assert(alternatingRowsDefaultCellStyle is not null);

        if (rowStyle is not null && !rowStyle.BackColor.IsEmpty)
        {
            inheritedRowStyle.BackColor = rowStyle.BackColor;
        }
        else if (!rowsDefaultCellStyle.BackColor.IsEmpty && (rowIndex % 2 == 0 || alternatingRowsDefaultCellStyle.BackColor.IsEmpty))
        {
            inheritedRowStyle.BackColor = rowsDefaultCellStyle.BackColor;
        }
        else if (rowIndex % 2 == 1 && !alternatingRowsDefaultCellStyle.BackColor.IsEmpty)
        {
            inheritedRowStyle.BackColor = alternatingRowsDefaultCellStyle.BackColor;
        }
        else
        {
            inheritedRowStyle.BackColor = dataGridViewStyle.BackColor;
        }

        if (rowStyle is not null && !rowStyle.ForeColor.IsEmpty)
        {
            inheritedRowStyle.ForeColor = rowStyle.ForeColor;
        }
        else if (!rowsDefaultCellStyle.ForeColor.IsEmpty && (rowIndex % 2 == 0 || alternatingRowsDefaultCellStyle.ForeColor.IsEmpty))
        {
            inheritedRowStyle.ForeColor = rowsDefaultCellStyle.ForeColor;
        }
        else if (rowIndex % 2 == 1 && !alternatingRowsDefaultCellStyle.ForeColor.IsEmpty)
        {
            inheritedRowStyle.ForeColor = alternatingRowsDefaultCellStyle.ForeColor;
        }
        else
        {
            inheritedRowStyle.ForeColor = dataGridViewStyle.ForeColor;
        }

        if (rowStyle is not null && !rowStyle.SelectionBackColor.IsEmpty)
        {
            inheritedRowStyle.SelectionBackColor = rowStyle.SelectionBackColor;
        }
        else if (!rowsDefaultCellStyle.SelectionBackColor.IsEmpty && (rowIndex % 2 == 0 || alternatingRowsDefaultCellStyle.SelectionBackColor.IsEmpty))
        {
            inheritedRowStyle.SelectionBackColor = rowsDefaultCellStyle.SelectionBackColor;
        }
        else if (rowIndex % 2 == 1 && !alternatingRowsDefaultCellStyle.SelectionBackColor.IsEmpty)
        {
            inheritedRowStyle.SelectionBackColor = alternatingRowsDefaultCellStyle.SelectionBackColor;
        }
        else
        {
            inheritedRowStyle.SelectionBackColor = dataGridViewStyle.SelectionBackColor;
        }

        if (rowStyle is not null && !rowStyle.SelectionForeColor.IsEmpty)
        {
            inheritedRowStyle.SelectionForeColor = rowStyle.SelectionForeColor;
        }
        else if (!rowsDefaultCellStyle.SelectionForeColor.IsEmpty && (rowIndex % 2 == 0 || alternatingRowsDefaultCellStyle.SelectionForeColor.IsEmpty))
        {
            inheritedRowStyle.SelectionForeColor = rowsDefaultCellStyle.SelectionForeColor;
        }
        else if (rowIndex % 2 == 1 && !alternatingRowsDefaultCellStyle.SelectionForeColor.IsEmpty)
        {
            inheritedRowStyle.SelectionForeColor = alternatingRowsDefaultCellStyle.SelectionForeColor;
        }
        else
        {
            inheritedRowStyle.SelectionForeColor = dataGridViewStyle.SelectionForeColor;
        }

        if (rowStyle is not null && rowStyle.Font is not null)
        {
            inheritedRowStyle.Font = rowStyle.Font;
        }
        else if (rowsDefaultCellStyle.Font is not null &&
                 (rowIndex % 2 == 0 || alternatingRowsDefaultCellStyle.Font is null))
        {
            inheritedRowStyle.Font = rowsDefaultCellStyle.Font;
        }
        else if (rowIndex % 2 == 1 && alternatingRowsDefaultCellStyle.Font is not null)
        {
            inheritedRowStyle.Font = alternatingRowsDefaultCellStyle.Font;
        }
        else
        {
            inheritedRowStyle.Font = dataGridViewStyle.Font;
        }

        if (rowStyle is not null && !rowStyle.IsNullValueDefault)
        {
            inheritedRowStyle.NullValue = rowStyle.NullValue;
        }
        else if (!rowsDefaultCellStyle.IsNullValueDefault &&
                 (rowIndex % 2 == 0 || alternatingRowsDefaultCellStyle.IsNullValueDefault))
        {
            inheritedRowStyle.NullValue = rowsDefaultCellStyle.NullValue;
        }
        else if (rowIndex % 2 == 1 && !alternatingRowsDefaultCellStyle.IsNullValueDefault)
        {
            inheritedRowStyle.NullValue = alternatingRowsDefaultCellStyle.NullValue;
        }
        else
        {
            inheritedRowStyle.NullValue = dataGridViewStyle.NullValue;
        }

        if (rowStyle is not null && !rowStyle.IsDataSourceNullValueDefault)
        {
            inheritedRowStyle.DataSourceNullValue = rowStyle.DataSourceNullValue;
        }
        else if (!rowsDefaultCellStyle.IsDataSourceNullValueDefault &&
                 (rowIndex % 2 == 0 || alternatingRowsDefaultCellStyle.IsDataSourceNullValueDefault))
        {
            inheritedRowStyle.DataSourceNullValue = rowsDefaultCellStyle.DataSourceNullValue;
        }
        else if (rowIndex % 2 == 1 && !alternatingRowsDefaultCellStyle.IsDataSourceNullValueDefault)
        {
            inheritedRowStyle.DataSourceNullValue = alternatingRowsDefaultCellStyle.DataSourceNullValue;
        }
        else
        {
            inheritedRowStyle.DataSourceNullValue = dataGridViewStyle.DataSourceNullValue;
        }

        if (rowStyle is not null && rowStyle.Format.Length != 0)
        {
            inheritedRowStyle.Format = rowStyle.Format;
        }
        else if (rowsDefaultCellStyle.Format.Length != 0 && (rowIndex % 2 == 0 || alternatingRowsDefaultCellStyle.Format.Length == 0))
        {
            inheritedRowStyle.Format = rowsDefaultCellStyle.Format;
        }
        else if (rowIndex % 2 == 1 && alternatingRowsDefaultCellStyle.Format.Length != 0)
        {
            inheritedRowStyle.Format = alternatingRowsDefaultCellStyle.Format;
        }
        else
        {
            inheritedRowStyle.Format = dataGridViewStyle.Format;
        }

        if (rowStyle is not null && !rowStyle.IsFormatProviderDefault)
        {
            inheritedRowStyle.FormatProvider = rowStyle.FormatProvider;
        }
        else if (!rowsDefaultCellStyle.IsFormatProviderDefault && (rowIndex % 2 == 0 || alternatingRowsDefaultCellStyle.IsFormatProviderDefault))
        {
            inheritedRowStyle.FormatProvider = rowsDefaultCellStyle.FormatProvider;
        }
        else if (rowIndex % 2 == 1 && !alternatingRowsDefaultCellStyle.IsFormatProviderDefault)
        {
            inheritedRowStyle.FormatProvider = alternatingRowsDefaultCellStyle.FormatProvider;
        }
        else
        {
            inheritedRowStyle.FormatProvider = dataGridViewStyle.FormatProvider;
        }

        if (rowStyle is not null && rowStyle.Alignment != DataGridViewContentAlignment.NotSet)
        {
            inheritedRowStyle.AlignmentInternal = rowStyle.Alignment;
        }
        else if (rowsDefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet && (rowIndex % 2 == 0 || alternatingRowsDefaultCellStyle.Alignment == DataGridViewContentAlignment.NotSet))
        {
            inheritedRowStyle.AlignmentInternal = rowsDefaultCellStyle.Alignment;
        }
        else if (rowIndex % 2 == 1 && alternatingRowsDefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet)
        {
            inheritedRowStyle.AlignmentInternal = alternatingRowsDefaultCellStyle.Alignment;
        }
        else
        {
            Debug.Assert(dataGridViewStyle.Alignment != DataGridViewContentAlignment.NotSet);
            inheritedRowStyle.AlignmentInternal = dataGridViewStyle.Alignment;
        }

        if (rowStyle is not null && rowStyle.WrapMode != DataGridViewTriState.NotSet)
        {
            inheritedRowStyle.WrapModeInternal = rowStyle.WrapMode;
        }
        else if (rowsDefaultCellStyle.WrapMode != DataGridViewTriState.NotSet && (rowIndex % 2 == 0 || alternatingRowsDefaultCellStyle.WrapMode == DataGridViewTriState.NotSet))
        {
            inheritedRowStyle.WrapModeInternal = rowsDefaultCellStyle.WrapMode;
        }
        else if (rowIndex % 2 == 1 && alternatingRowsDefaultCellStyle.WrapMode != DataGridViewTriState.NotSet)
        {
            inheritedRowStyle.WrapModeInternal = alternatingRowsDefaultCellStyle.WrapMode;
        }
        else
        {
            Debug.Assert(dataGridViewStyle.WrapMode != DataGridViewTriState.NotSet);
            inheritedRowStyle.WrapModeInternal = dataGridViewStyle.WrapMode;
        }

        if (rowStyle is not null && rowStyle.Tag is not null)
        {
            inheritedRowStyle.Tag = rowStyle.Tag;
        }
        else if (rowsDefaultCellStyle.Tag is not null && (rowIndex % 2 == 0 || alternatingRowsDefaultCellStyle.Tag is null))
        {
            inheritedRowStyle.Tag = rowsDefaultCellStyle.Tag;
        }
        else if (rowIndex % 2 == 1 && alternatingRowsDefaultCellStyle.Tag is not null)
        {
            inheritedRowStyle.Tag = alternatingRowsDefaultCellStyle.Tag;
        }
        else
        {
            inheritedRowStyle.Tag = dataGridViewStyle.Tag;
        }

        if (rowStyle is not null && rowStyle.Padding != Padding.Empty)
        {
            inheritedRowStyle.PaddingInternal = rowStyle.Padding;
        }
        else if (rowsDefaultCellStyle.Padding != Padding.Empty &&
                 (rowIndex % 2 == 0 || alternatingRowsDefaultCellStyle.Padding == Padding.Empty))
        {
            inheritedRowStyle.PaddingInternal = rowsDefaultCellStyle.Padding;
        }
        else if (rowIndex % 2 == 1 && alternatingRowsDefaultCellStyle.Padding != Padding.Empty)
        {
            inheritedRowStyle.PaddingInternal = alternatingRowsDefaultCellStyle.Padding;
        }
        else
        {
            inheritedRowStyle.PaddingInternal = dataGridViewStyle.Padding;
        }
    }

    public override object Clone()
    {
        DataGridViewRow dataGridViewRow;
        Type thisType = GetType();

        if (thisType == s_rowType)
        {
            // Performance improvement
            dataGridViewRow = new DataGridViewRow();
        }
        else
        {
            dataGridViewRow = (DataGridViewRow)Activator.CreateInstance(thisType)!;
        }

        if (dataGridViewRow is not null)
        {
            CloneInternal(dataGridViewRow);
            if (HasErrorText)
            {
                dataGridViewRow.ErrorText = ErrorTextInternal;
            }

            if (HasHeaderCell)
            {
                dataGridViewRow.HeaderCell = (DataGridViewRowHeaderCell)HeaderCell.Clone();
            }

            dataGridViewRow.CloneCells(this);
        }

        return dataGridViewRow!;
    }

    private void CloneCells(DataGridViewRow rowTemplate)
    {
        int cellsCount = rowTemplate.Cells.Count;
        if (cellsCount > 0)
        {
            DataGridViewCell[] cells = new DataGridViewCell[cellsCount];
            for (int i = 0; i < cellsCount; i++)
            {
                DataGridViewCell dataGridViewCell = rowTemplate.Cells[i];
                DataGridViewCell dgvcNew = (DataGridViewCell)dataGridViewCell.Clone();
                cells[i] = dgvcNew;
            }

            Cells.AddRange(cells);
        }
    }

    protected virtual AccessibleObject CreateAccessibilityInstance()
    {
        return new DataGridViewRowAccessibleObject(this);
    }

    public void CreateCells(DataGridView dataGridView)
    {
        ArgumentNullException.ThrowIfNull(dataGridView);

        if (DataGridView is not null)
        {
            throw new InvalidOperationException(SR.DataGridView_RowAlreadyBelongsToDataGridView);
        }

        DataGridViewCellCollection cells = Cells;
        // Clearing up the potential existing cells. We fill up the cells collection from scratch.
        cells.Clear();
        DataGridViewColumnCollection dataGridViewColumns = dataGridView.Columns;
        foreach (DataGridViewColumn dataGridViewColumn in dataGridViewColumns)
        {
            if (dataGridViewColumn.CellTemplate is null)
            {
                throw new InvalidOperationException(SR.DataGridView_AColumnHasNoCellTemplate);
            }

            DataGridViewCell dgvcNew = (DataGridViewCell)dataGridViewColumn.CellTemplate.Clone();
            cells.Add(dgvcNew);
        }
    }

    public void CreateCells(DataGridView dataGridView, params object[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        // Intentionally not being strict about this. We just take what we get.
        CreateCells(dataGridView);

        Debug.Assert(Cells.Count == dataGridView.Columns.Count);
        SetValuesInternal(values);
    }

    /// <summary>
    ///  Constructs the new instance of the Cells collection objects. Subclasses
    ///  should not call base.CreateCellsInstance.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual DataGridViewCellCollection CreateCellsInstance()
    {
        return new DataGridViewCellCollection(this);
    }

    internal void DetachFromDataGridView()
    {
        if (DataGridView is not null)
        {
            DataGridView = null;
            Index = -1;
            if (HasHeaderCell)
            {
                HeaderCell.DataGridView = null;
            }

            foreach (DataGridViewCell dataGridViewCell in Cells)
            {
                dataGridViewCell.DataGridView = null;
                if (dataGridViewCell.Selected)
                {
                    dataGridViewCell.SelectedInternal = false;
                }
            }

            if (Selected)
            {
                SelectedInternal = false;
            }
        }

        Debug.Assert(Index == -1);
        Debug.Assert(!Selected);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal virtual void DrawFocus(
        Graphics graphics,
        Rectangle clipBounds,
        Rectangle bounds,
        int rowIndex,
        DataGridViewElementStates rowState,
        DataGridViewCellStyle cellStyle,
        bool cellsPaintSelectionBackground)
    {
        if (DataGridView is null)
        {
            throw new InvalidOperationException(SR.DataGridView_RowDoesNotYetBelongToDataGridView);
        }

        ArgumentNullException.ThrowIfNull(graphics);
        ArgumentNullException.ThrowIfNull(cellStyle);

        Color backColor;
        if (cellsPaintSelectionBackground && (rowState & DataGridViewElementStates.Selected) != 0)
        {
            backColor = cellStyle.SelectionBackColor;
        }
        else
        {
            backColor = cellStyle.BackColor;
        }

        ControlPaint.DrawFocusRectangle(graphics, bounds, Color.Empty, backColor);
    }

    public ContextMenuStrip? GetContextMenuStrip(int rowIndex)
    {
        ContextMenuStrip? contextMenuStrip = ContextMenuStripInternal;
        if (DataGridView is not null)
        {
            if (rowIndex == -1)
            {
                throw new InvalidOperationException(SR.DataGridView_InvalidOperationOnSharedRow);
            }

            ArgumentOutOfRangeException.ThrowIfNegative(rowIndex);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(rowIndex, DataGridView.Rows.Count);

            if (DataGridView.VirtualMode || DataGridView.DataSource is not null)
            {
                contextMenuStrip = DataGridView.OnRowContextMenuStripNeeded(rowIndex, contextMenuStrip);
            }
        }

        return contextMenuStrip;
    }

    internal bool GetDisplayed(int rowIndex)
    {
        return (GetState(rowIndex) & DataGridViewElementStates.Displayed) != 0;
    }

    public string GetErrorText(int rowIndex)
    {
        string errorText = ErrorTextInternal;
        if (DataGridView is not null)
        {
            if (rowIndex == -1)
            {
                throw new InvalidOperationException(SR.DataGridView_InvalidOperationOnSharedRow);
            }

            ArgumentOutOfRangeException.ThrowIfNegative(rowIndex);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(rowIndex, DataGridView.Rows.Count);

            if (string.IsNullOrEmpty(errorText) &&
                DataGridView.DataSource is not null &&
                rowIndex != DataGridView.NewRowIndex)
            {
                errorText = DataGridView.DataConnection!.GetError(rowIndex);
            }

            if (DataGridView.DataSource is not null || DataGridView.VirtualMode)
            {
                errorText = DataGridView.OnRowErrorTextNeeded(rowIndex, errorText);
            }
        }

        return errorText;
    }

    internal bool GetFrozen(int rowIndex)
    {
        return (GetState(rowIndex) & DataGridViewElementStates.Frozen) != 0;
    }

    internal int GetHeight(int rowIndex)
    {
        GetHeightInfo(rowIndex, out int height, out _);
        return height;
    }

    internal int GetMinimumHeight(int rowIndex)
    {
        Debug.Assert(rowIndex >= -1);
        GetHeightInfo(rowIndex, out _, out int minimumHeight);
        return minimumHeight;
    }

    public virtual int GetPreferredHeight(int rowIndex, DataGridViewAutoSizeRowMode autoSizeRowMode, bool fixedWidth)
    {
        if (((DataGridViewAutoSizeRowCriteriaInternal)autoSizeRowMode & InvalidDataGridViewAutoSizeRowCriteriaInternalMask) != 0)
        {
            throw new InvalidEnumArgumentException(nameof(autoSizeRowMode), (int)autoSizeRowMode, typeof(DataGridViewAutoSizeRowMode));
        }

        if (DataGridView is null)
        {
            return -1;
        }

        ArgumentOutOfRangeException.ThrowIfNegative(rowIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(rowIndex, DataGridView.Rows.Count);

        int preferredRowThickness = 0, preferredCellThickness;
        // take into account the preferred height of the header cell if displayed and cared about
        if (DataGridView.RowHeadersVisible &&
            (((DataGridViewAutoSizeRowCriteriaInternal)autoSizeRowMode) & DataGridViewAutoSizeRowCriteriaInternal.Header) != 0)
        {
            if (fixedWidth ||
                DataGridView.RowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.EnableResizing ||
                DataGridView.RowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.DisableResizing)
            {
                preferredRowThickness = Math.Max(preferredRowThickness, HeaderCell.GetPreferredHeight(rowIndex, DataGridView.RowHeadersWidth));
            }
            else
            {
                preferredRowThickness = Math.Max(preferredRowThickness, HeaderCell.GetPreferredSize(rowIndex).Height);
            }
        }

        if ((((DataGridViewAutoSizeRowCriteriaInternal)autoSizeRowMode) & DataGridViewAutoSizeRowCriteriaInternal.AllColumns) != 0)
        {
            foreach (DataGridViewCell dataGridViewCell in Cells)
            {
                DataGridViewColumn dataGridViewColumn = DataGridView.Columns[dataGridViewCell.ColumnIndex];
                if (dataGridViewColumn.Visible)
                {
                    if (fixedWidth ||
                        ((((DataGridViewAutoSizeColumnCriteriaInternal)dataGridViewColumn.InheritedAutoSizeMode) & (DataGridViewAutoSizeColumnCriteriaInternal.AllRows | DataGridViewAutoSizeColumnCriteriaInternal.DisplayedRows)) == 0))
                    {
                        preferredCellThickness = dataGridViewCell.GetPreferredHeight(rowIndex, dataGridViewColumn.Width);
                    }
                    else
                    {
                        preferredCellThickness = dataGridViewCell.GetPreferredSize(rowIndex).Height;
                    }

                    if (preferredRowThickness < preferredCellThickness)
                    {
                        preferredRowThickness = preferredCellThickness;
                    }
                }
            }
        }

        return preferredRowThickness;
    }

    internal bool GetReadOnly(int rowIndex)
    {
        return (GetState(rowIndex) & DataGridViewElementStates.ReadOnly) != 0 ||
               (DataGridView is not null && DataGridView.ReadOnly);
    }

    internal DataGridViewTriState GetResizable(int rowIndex)
    {
        if ((GetState(rowIndex) & DataGridViewElementStates.ResizableSet) != 0)
        {
            return ((GetState(rowIndex) & DataGridViewElementStates.Resizable) != 0) ? DataGridViewTriState.True : DataGridViewTriState.False;
        }

        if (DataGridView is not null)
        {
            return DataGridView.AllowUserToResizeRows ? DataGridViewTriState.True : DataGridViewTriState.False;
        }
        else
        {
            return DataGridViewTriState.NotSet;
        }
    }

    internal bool GetSelected(int rowIndex)
    {
        return (GetState(rowIndex) & DataGridViewElementStates.Selected) != 0;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual DataGridViewElementStates GetState(int rowIndex)
    {
        if (DataGridView is not null)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(rowIndex);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(rowIndex, DataGridView.Rows.Count);
        }

        if (DataGridView is null || DataGridView.Rows.SharedRow(rowIndex).Index != -1)
        {
            if (rowIndex != Index)
            {
                throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(rowIndex), rowIndex), nameof(rowIndex));
            }

            return base.State;
        }
        else
        {
            return DataGridView.Rows.GetRowState(rowIndex);
        }
    }

    internal bool GetVisible(int rowIndex)
    {
        return (GetState(rowIndex) & DataGridViewElementStates.Visible) != 0;
    }

    internal void OnSharedStateChanged(int sharedRowIndex, DataGridViewElementStates elementState)
    {
        Debug.Assert(DataGridView is not null);
        DataGridView.Rows.InvalidateCachedRowCount(elementState);
        DataGridView.Rows.InvalidateCachedRowsHeight(elementState);
        DataGridView.OnDataGridViewElementStateChanged(this, sharedRowIndex, elementState);
    }

    internal void OnSharedStateChanging(int sharedRowIndex, DataGridViewElementStates elementState)
    {
        Debug.Assert(DataGridView is not null);
        DataGridView.OnDataGridViewElementStateChanging(this, sharedRowIndex, elementState);
    }

    protected internal virtual void Paint(
        Graphics graphics,
        Rectangle clipBounds,
        Rectangle rowBounds,
        int rowIndex,
        DataGridViewElementStates rowState,
        bool isFirstDisplayedRow,
        bool isLastVisibleRow)
    {
        if (DataGridView is null)
        {
            throw new InvalidOperationException(SR.DataGridView_RowDoesNotYetBelongToDataGridView);
        }

        ArgumentNullException.ThrowIfNull(graphics);

        DataGridView dataGridView = DataGridView;
        DataGridViewRow sharedRow = dataGridView.Rows.SharedRow(rowIndex);
        DataGridViewCellStyle inheritedRowStyle = new();
        BuildInheritedRowStyle(rowIndex, inheritedRowStyle);
        DataGridViewRowPrePaintEventArgs dgvrprepe = dataGridView.RowPrePaintEventArgs;
        dgvrprepe.SetProperties(graphics,
                                clipBounds,
                                rowBounds,
                                rowIndex,
                                rowState,
                                sharedRow.GetErrorText(rowIndex),
                                inheritedRowStyle,
                                isFirstDisplayedRow,
                                isLastVisibleRow);
        dataGridView.OnRowPrePaint(dgvrprepe);
        if (dgvrprepe.Handled)
        {
            return;
        }

        DataGridViewPaintParts paintParts = dgvrprepe.PaintParts;
        Rectangle updatedClipBounds = dgvrprepe.ClipBounds;

        // first paint the potential row header
        PaintHeader(graphics,
                    updatedClipBounds,
                    rowBounds,
                    rowIndex,
                    rowState,
                    isFirstDisplayedRow,
                    isLastVisibleRow,
                    paintParts);

        // then paint the inner cells
        PaintCells(graphics,
                   updatedClipBounds,
                   rowBounds,
                   rowIndex,
                   rowState,
                   isFirstDisplayedRow,
                   isLastVisibleRow,
                   paintParts);

        sharedRow = dataGridView.Rows.SharedRow(rowIndex);
        BuildInheritedRowStyle(rowIndex, inheritedRowStyle);
        DataGridViewRowPostPaintEventArgs dgvrpostpe = dataGridView.RowPostPaintEventArgs;
        dgvrpostpe.SetProperties(graphics,
                                 updatedClipBounds,
                                 rowBounds,
                                 rowIndex,
                                 rowState,
                                 sharedRow.GetErrorText(rowIndex),
                                 inheritedRowStyle,
                                 isFirstDisplayedRow,
                                 isLastVisibleRow);
        dataGridView.OnRowPostPaint(dgvrpostpe);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal virtual void PaintCells(
        Graphics graphics,
        Rectangle clipBounds,
        Rectangle rowBounds,
        int rowIndex,
        DataGridViewElementStates rowState,
        bool isFirstDisplayedRow,
        bool isLastVisibleRow,
        DataGridViewPaintParts paintParts)
    {
        if (DataGridView is null)
        {
            throw new InvalidOperationException(SR.DataGridView_RowDoesNotYetBelongToDataGridView);
        }

        ArgumentNullException.ThrowIfNull(graphics);

        if (paintParts is < DataGridViewPaintParts.None or > DataGridViewPaintParts.All)
        {
            throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewPaintPartsCombination, "paintParts"));
        }

        DataGridView dataGridView = DataGridView;
        Rectangle cellBounds = rowBounds;
        int cx = (dataGridView.RowHeadersVisible ? dataGridView.RowHeadersWidth : 0);
        bool isFirstDisplayedColumn = true;
        DataGridViewCell cell;
        DataGridViewCellStyle inheritedCellStyle = new();
        DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder = new(), dgvabsEffective;

        // first paint the potential visible frozen cells
        DataGridViewColumn? dataGridViewColumn = dataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);
        DataGridViewElementStates cellState;
        DataGridViewColumn? dataGridViewColumnNext;
        while (dataGridViewColumn is not null)
        {
            cell = Cells[dataGridViewColumn.Index];
            cellBounds.Width = dataGridViewColumn.Thickness;
            if (dataGridView.SingleVerticalBorderAdded && isFirstDisplayedColumn)
            {
                cellBounds.Width++;
            }

            Debug.Assert(cellBounds.Width > 0);
            if (dataGridView.RightToLeftInternal)
            {
                cellBounds.X = rowBounds.Right - cx - cellBounds.Width;
            }
            else
            {
                cellBounds.X = rowBounds.X + cx;
            }

            dataGridViewColumnNext = dataGridView.Columns.GetNextColumn(dataGridViewColumn,
                DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen,
                DataGridViewElementStates.None);

            if (clipBounds.IntersectsWith(cellBounds))
            {
                cellState = cell.CellStateFromColumnRowStates(rowState);
                if (Index != -1)
                {
                    cellState |= cell.State;
                }

                cell.GetInheritedStyle(inheritedCellStyle, rowIndex, includeColors: true);

                dgvabsEffective = cell.AdjustCellBorderStyle(dataGridView.AdvancedCellBorderStyle, dataGridViewAdvancedBorderStylePlaceholder,
                    dataGridView.SingleVerticalBorderAdded,
                    dataGridView.SingleHorizontalBorderAdded,
                    isFirstDisplayedColumn,
                    isFirstDisplayedRow);

                cell.PaintWork(graphics,
                                clipBounds,
                                cellBounds,
                                rowIndex,
                                cellState,
                                inheritedCellStyle,
                                dgvabsEffective,
                                paintParts);
            }

            cx += cellBounds.Width;
            if (cx >= rowBounds.Width)
            {
                break;
            }

            dataGridViewColumn = dataGridViewColumnNext;
            isFirstDisplayedColumn = false;
        }

        // then paint the visible scrolling ones
        Rectangle dataBounds = rowBounds;

        if (cx < dataBounds.Width)
        {
            if (dataGridView.FirstDisplayedScrollingColumnIndex >= 0)
            {
                if (!dataGridView.RightToLeftInternal)
                {
                    dataBounds.X -= dataGridView.FirstDisplayedScrollingColumnHiddenWidth;
                }

                dataBounds.Width += dataGridView.FirstDisplayedScrollingColumnHiddenWidth;

                Region? clipRegion = null;
                if (dataGridView.FirstDisplayedScrollingColumnHiddenWidth > 0)
                {
                    clipRegion = graphics.Clip;
                    Rectangle rowRect = rowBounds;
                    if (!dataGridView.RightToLeftInternal)
                    {
                        rowRect.X += cx;
                    }

                    rowRect.Width -= cx;
                    graphics.SetClip(rowRect);
                }

                dataGridViewColumn = dataGridView.Columns[dataGridView.FirstDisplayedScrollingColumnIndex];
                Debug.Assert(dataGridViewColumn.Visible && !dataGridViewColumn.Frozen);

                while (dataGridViewColumn is not null)
                {
                    cell = Cells[dataGridViewColumn.Index];
                    cellBounds.Width = dataGridViewColumn.Thickness;
                    if (dataGridView.SingleVerticalBorderAdded && isFirstDisplayedColumn)
                    {
                        cellBounds.Width++;
                    }

                    Debug.Assert(cellBounds.Width > 0);
                    if (dataGridView.RightToLeftInternal)
                    {
                        cellBounds.X = dataBounds.Right - cx - cellBounds.Width;
                    }
                    else
                    {
                        cellBounds.X = dataBounds.X + cx;
                    }

                    dataGridViewColumnNext = dataGridView.Columns.GetNextColumn(dataGridViewColumn,
                        DataGridViewElementStates.Visible,
                        DataGridViewElementStates.None);

                    if (clipBounds.IntersectsWith(cellBounds))
                    {
                        cellState = cell.CellStateFromColumnRowStates(rowState);
                        if (Index != -1)
                        {
                            cellState |= cell.State;
                        }

                        cell.GetInheritedStyle(inheritedCellStyle, rowIndex, includeColors: true);

                        dgvabsEffective = cell.AdjustCellBorderStyle(dataGridView.AdvancedCellBorderStyle, dataGridViewAdvancedBorderStylePlaceholder,
                            dataGridView.SingleVerticalBorderAdded,
                            dataGridView.SingleHorizontalBorderAdded,
                            isFirstDisplayedColumn,
                            isFirstDisplayedRow);

                        cell.PaintWork(graphics,
                                       clipBounds,
                                       cellBounds,
                                       rowIndex,
                                       cellState,
                                       inheritedCellStyle,
                                       dgvabsEffective,
                                       paintParts);
                    }

                    cx += cellBounds.Width;
                    if (cx >= dataBounds.Width)
                    {
                        break;
                    }

                    dataGridViewColumn = dataGridViewColumnNext;
                    isFirstDisplayedColumn = false;
                }

                if (clipRegion is not null)
                {
                    graphics.Clip = clipRegion;
                    clipRegion.Dispose();
                }
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal virtual void PaintHeader(
        Graphics graphics,
        Rectangle clipBounds,
        Rectangle rowBounds,
        int rowIndex,
        DataGridViewElementStates rowState,
        bool isFirstDisplayedRow,
        bool isLastVisibleRow,
        DataGridViewPaintParts paintParts)
    {
        if (DataGridView is null)
        {
            throw new InvalidOperationException(SR.DataGridView_RowDoesNotYetBelongToDataGridView);
        }

        ArgumentNullException.ThrowIfNull(graphics);

        if (paintParts is < DataGridViewPaintParts.None or > DataGridViewPaintParts.All)
        {
            throw new InvalidEnumArgumentException(nameof(paintParts), (int)paintParts, typeof(DataGridViewPaintParts));
        }

        DataGridView dataGridView = DataGridView;
        if (dataGridView.RowHeadersVisible)
        {
            Rectangle cellBounds = rowBounds;
            cellBounds.Width = dataGridView.RowHeadersWidth;
            Debug.Assert(cellBounds.Width > 0);
            if (dataGridView.RightToLeftInternal)
            {
                cellBounds.X = rowBounds.Right - cellBounds.Width;
            }

            if (clipBounds.IntersectsWith(cellBounds))
            {
                DataGridViewCellStyle inheritedCellStyle = new();
                DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder = new(), dgvabsEffective;
                BuildInheritedRowHeaderCellStyle(inheritedCellStyle);
                dgvabsEffective = AdjustRowHeaderBorderStyle(dataGridView.AdvancedRowHeadersBorderStyle,
                    dataGridViewAdvancedBorderStylePlaceholder,
                    dataGridView.SingleVerticalBorderAdded,
                    dataGridView.SingleHorizontalBorderAdded,
                    isFirstDisplayedRow,
                    isLastVisibleRow);
                HeaderCell.PaintWork(graphics,
                    clipBounds,
                    cellBounds,
                    rowIndex,
                    rowState,
                    inheritedCellStyle,
                    dgvabsEffective,
                    paintParts);
            }
        }
    }

    // ShouldSerialize and Reset Methods are being used by Designer via reflection.
    private void ResetHeight()
    {
        Height = DefaultHeight;
    }

    internal void ReleaseUiaProvider()
    {
        if (!IsAccessibilityObjectCreated)
        {
            return;
        }

        PInvoke.UiaDisconnectProvider(AccessibilityObject);
        Properties.RemoveValue(s_propRowAccessibilityObject);
    }

    internal void SetReadOnlyCellCore(DataGridViewCell dataGridViewCell, bool readOnly)
    {
        Debug.Assert(Index == -1);
        if (ReadOnly && !readOnly)
        {
            // All cells need to switch to ReadOnly except for dataGridViewCell which needs to be !ReadOnly,
            // plus the row become !ReadOnly.
            foreach (DataGridViewCell dataGridViewCellTmp in Cells)
            {
                dataGridViewCellTmp.ReadOnlyInternal = true;
            }

            dataGridViewCell.ReadOnlyInternal = false;
            ReadOnly = false;
        }
        else if (!ReadOnly && readOnly)
        {
            // dataGridViewCell alone becomes ReadOnly
            dataGridViewCell.ReadOnlyInternal = true;
        }
    }

    public bool SetValues(params object[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (DataGridView is not null)
        {
            if (DataGridView.VirtualMode)
            {
                throw new InvalidOperationException(SR.DataGridView_InvalidOperationInVirtualMode);
            }

            if (Index == -1)
            {
                throw new InvalidOperationException(SR.DataGridView_InvalidOperationOnSharedRow);
            }
        }

        return SetValuesInternal(values);
    }

    internal bool SetValuesInternal(params object[] values)
    {
        Debug.Assert(values is not null);
        bool setResult = true;
        DataGridViewCellCollection cells = Cells;
        int cellCount = cells.Count;
        for (int columnIndex = 0; columnIndex < cells.Count; columnIndex++)
        {
            if (columnIndex == values.Length)
            {
                break;
            }

            if (!cells[columnIndex].SetValueInternal(Index, values[columnIndex]))
            {
                setResult = false;
            }
        }

        return setResult && values.Length <= cellCount;
    }

    // ShouldSerialize and Reset Methods are being used by Designer via reflection.
    private bool ShouldSerializeHeight()
    {
        return Height != DefaultHeight;
    }

    public override string ToString()
    {
        return $"DataGridViewRow {{ Index={Index} }}";
    }
}

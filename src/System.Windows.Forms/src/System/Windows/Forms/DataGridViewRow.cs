// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Identifies a row in the dataGridView.
    /// </summary>
    [TypeConverter(typeof(DataGridViewRowConverter))]
    public class DataGridViewRow : DataGridViewBand
    {
        private static readonly Type s_rowType = typeof(DataGridViewRow);
        private static readonly int s_propRowErrorText = PropertyStore.CreateKey();
        private static readonly int s_propRowAccessibilityObject = PropertyStore.CreateKey();

        private const DataGridViewAutoSizeRowCriteriaInternal InvalidDataGridViewAutoSizeRowCriteriaInternalMask = ~(DataGridViewAutoSizeRowCriteriaInternal.Header | DataGridViewAutoSizeRowCriteriaInternal.AllColumns);

        private const int DefaultMinRowThickness = 3;

        private DataGridViewCellCollection _rowCells;

        /// <summary>
        ///  Initializes a new instance of the <see cref='DataGridViewRow'/> class.
        /// </summary>
        public DataGridViewRow() : base()
        {
            MinimumThickness = DefaultMinRowThickness;
            Thickness = Control.DefaultFont.Height + 9;
        }

        [Browsable(false)]
        public AccessibleObject AccessibilityObject
        {
            get
            {
                AccessibleObject result = (AccessibleObject)Properties.GetObject(s_propRowAccessibilityObject);
                if (result == null)
                {
                    result = CreateAccessibilityInstance();
                    Properties.SetObject(s_propRowAccessibilityObject, result);
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
        public override ContextMenuStrip ContextMenuStrip
        {
            get => base.ContextMenuStrip;
            set => base.ContextMenuStrip = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public object DataBoundItem
        {
            get
            {
                if (DataGridView != null &&
                    DataGridView.DataConnection != null &&
                    Index > -1 &&
                    Index != DataGridView.NewRowIndex)
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
        public override DataGridViewCellStyle DefaultCellStyle
        {
            get => base.DefaultCellStyle;
            set
            {
                if (DataGridView != null && Index == -1)
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
                if (DataGridView != null && Index == -1)
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
                if (DataGridView != null && Index == -1)
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

        private string ErrorTextInternal
        {
            get
            {
                object errorText = Properties.GetObject(s_propRowErrorText);
                return (string)errorText ?? string.Empty;
            }
            set
            {
                string errorText = ErrorTextInternal;
                if (!string.IsNullOrEmpty(value) || Properties.ContainsObject(s_propRowErrorText))
                {
                    Properties.SetObject(s_propRowErrorText, value);
                }
                if (DataGridView != null && !errorText.Equals(ErrorTextInternal))
                {
                    DataGridView.OnRowErrorTextChanged(this);
                }
            }
        }

        [Browsable(false)]
        public override bool Frozen
        {
            get
            {
                if (DataGridView != null && Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedRow, nameof(Frozen)));
                }

                return GetFrozen(Index);
            }
            set
            {
                if (DataGridView != null && Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, nameof(Frozen)));
                }

                base.Frozen = value;
            }
        }

        private bool HasErrorText
        {
            get => Properties.ContainsObject(s_propRowErrorText) && Properties.GetObject(s_propRowErrorText) != null;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataGridViewRowHeaderCell HeaderCell
        {
            get => (DataGridViewRowHeaderCell)base.HeaderCellCore;
            set => base.HeaderCellCore = value;
        }

        [DefaultValue(22)]
        [NotifyParentProperty(true)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.DataGridView_RowHeightDescr))]
        public int Height
        {
            get => Thickness;
            set
            {
                if (DataGridView != null && Index == -1)
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

                var inheritedRowStyle = new DataGridViewCellStyle();
                BuildInheritedRowStyle(Index, inheritedRowStyle);
                return inheritedRowStyle;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsNewRow
        {
            get => DataGridView != null && DataGridView.NewRowIndex == Index;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MinimumHeight
        {
            get => MinimumThickness;
            set
            {
                if (DataGridView != null && Index == -1)
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
                if (DataGridView != null && Index == -1)
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
                if (DataGridView != null && Index == -1)
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
                if (DataGridView != null && Index == -1)
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
                if (DataGridView != null && Index == -1)
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
                if (DataGridView != null && Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedRow, nameof(Visible)));
                }

                return GetVisible(Index);
            }
            set
            {
                if (DataGridView != null && Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, nameof(Visible)));
                }

                base.Visible = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual DataGridViewAdvancedBorderStyle AdjustRowHeaderBorderStyle(DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStyleInput,
            DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder,
            bool singleVerticalBorderAdded,
            bool singleHorizontalBorderAdded,
            bool isFirstDisplayedRow,
            bool isLastVisibleRow)
        {
            if (DataGridView != null && DataGridView.ApplyVisualStylesToHeaderCells)
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
                        if (DataGridView != null && DataGridView.RightToLeftInternal)
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
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridView != null && DataGridView.ColumnHeadersVisible ? DataGridViewAdvancedCellBorderStyle.Outset : DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                        }
                        else
                        {
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.OutsetPartial;
                        }
                        dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = isLastVisibleRow ? DataGridViewAdvancedCellBorderStyle.Outset : DataGridViewAdvancedCellBorderStyle.OutsetPartial;
                        return dataGridViewAdvancedBorderStylePlaceholder;

                    case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                        if (DataGridView != null && DataGridView.RightToLeftInternal)
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
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridView != null && DataGridView.ColumnHeadersVisible ? DataGridViewAdvancedCellBorderStyle.Outset : DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                        }
                        else
                        {
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                        }
                        dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                        return dataGridViewAdvancedBorderStylePlaceholder;

                    case DataGridViewAdvancedCellBorderStyle.InsetDouble:
                        if (DataGridView != null && DataGridView.RightToLeftInternal)
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
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridView != null && DataGridView.ColumnHeadersVisible ? DataGridViewAdvancedCellBorderStyle.Inset : DataGridViewAdvancedCellBorderStyle.InsetDouble;
                        }
                        else
                        {
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                        }
                        dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                        return dataGridViewAdvancedBorderStylePlaceholder;

                    case DataGridViewAdvancedCellBorderStyle.Single:
                        if (!isFirstDisplayedRow || (DataGridView != null && DataGridView.ColumnHeadersVisible))
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
            Debug.Assert(inheritedCellStyle != null);

            DataGridViewCellStyle cellStyle = null;
            if (HeaderCell.HasStyle)
            {
                cellStyle = HeaderCell.Style;
                Debug.Assert(cellStyle != null);
            }

            DataGridViewCellStyle rowHeadersStyle = DataGridView.RowHeadersDefaultCellStyle;
            Debug.Assert(rowHeadersStyle != null);

            DataGridViewCellStyle dataGridViewStyle = DataGridView.DefaultCellStyle;
            Debug.Assert(dataGridViewStyle != null);

            if (cellStyle != null && !cellStyle.BackColor.IsEmpty)
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

            if (cellStyle != null && !cellStyle.ForeColor.IsEmpty)
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

            if (cellStyle != null && !cellStyle.SelectionBackColor.IsEmpty)
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

            if (cellStyle != null && !cellStyle.SelectionForeColor.IsEmpty)
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

            if (cellStyle != null && cellStyle.Font != null)
            {
                inheritedCellStyle.Font = cellStyle.Font;
            }
            else if (rowHeadersStyle.Font != null)
            {
                inheritedCellStyle.Font = rowHeadersStyle.Font;
            }
            else
            {
                inheritedCellStyle.Font = dataGridViewStyle.Font;
            }

            if (cellStyle != null && !cellStyle.IsNullValueDefault)
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

            if (cellStyle != null && !cellStyle.IsDataSourceNullValueDefault)
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

            if (cellStyle != null && cellStyle.Format.Length != 0)
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

            if (cellStyle != null && !cellStyle.IsFormatProviderDefault)
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

            if (cellStyle != null && cellStyle.Alignment != DataGridViewContentAlignment.NotSet)
            {
                inheritedCellStyle.AlignmentInternal = cellStyle.Alignment;
            }
            else if (rowHeadersStyle != null && rowHeadersStyle.Alignment != DataGridViewContentAlignment.NotSet)
            {
                inheritedCellStyle.AlignmentInternal = rowHeadersStyle.Alignment;
            }
            else
            {
                Debug.Assert(dataGridViewStyle.Alignment != DataGridViewContentAlignment.NotSet);
                inheritedCellStyle.AlignmentInternal = dataGridViewStyle.Alignment;
            }

            if (cellStyle != null && cellStyle.WrapMode != DataGridViewTriState.NotSet)
            {
                inheritedCellStyle.WrapModeInternal = cellStyle.WrapMode;
            }
            else if (rowHeadersStyle != null && rowHeadersStyle.WrapMode != DataGridViewTriState.NotSet)
            {
                inheritedCellStyle.WrapModeInternal = rowHeadersStyle.WrapMode;
            }
            else
            {
                Debug.Assert(dataGridViewStyle.WrapMode != DataGridViewTriState.NotSet);
                inheritedCellStyle.WrapModeInternal = dataGridViewStyle.WrapMode;
            }

            if (cellStyle != null && cellStyle.Tag != null)
            {
                inheritedCellStyle.Tag = cellStyle.Tag;
            }
            else if (rowHeadersStyle.Tag != null)
            {
                inheritedCellStyle.Tag = rowHeadersStyle.Tag;
            }
            else
            {
                inheritedCellStyle.Tag = dataGridViewStyle.Tag;
            }

            if (cellStyle != null && cellStyle.Padding != Padding.Empty)
            {
                inheritedCellStyle.PaddingInternal = cellStyle.Padding;
            }
            else if (rowHeadersStyle.Padding != Padding.Empty)
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
            Debug.Assert(inheritedRowStyle != null);
            Debug.Assert(rowIndex >= 0);
            Debug.Assert(DataGridView != null);

            DataGridViewCellStyle rowStyle = null;
            if (HasDefaultCellStyle)
            {
                rowStyle = DefaultCellStyle;
                Debug.Assert(rowStyle != null);
            }

            DataGridViewCellStyle dataGridViewStyle = DataGridView.DefaultCellStyle;
            Debug.Assert(dataGridViewStyle != null);

            DataGridViewCellStyle rowsDefaultCellStyle = DataGridView.RowsDefaultCellStyle;
            Debug.Assert(rowsDefaultCellStyle != null);

            DataGridViewCellStyle alternatingRowsDefaultCellStyle = DataGridView.AlternatingRowsDefaultCellStyle;
            Debug.Assert(alternatingRowsDefaultCellStyle != null);

            if (rowStyle != null && !rowStyle.BackColor.IsEmpty)
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

            if (rowStyle != null && !rowStyle.ForeColor.IsEmpty)
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

            if (rowStyle != null && !rowStyle.SelectionBackColor.IsEmpty)
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

            if (rowStyle != null && !rowStyle.SelectionForeColor.IsEmpty)
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

            if (rowStyle != null && rowStyle.Font != null)
            {
                inheritedRowStyle.Font = rowStyle.Font;
            }
            else if (rowsDefaultCellStyle.Font != null &&
                     (rowIndex % 2 == 0 || alternatingRowsDefaultCellStyle.Font == null))
            {
                inheritedRowStyle.Font = rowsDefaultCellStyle.Font;
            }
            else if (rowIndex % 2 == 1 && alternatingRowsDefaultCellStyle.Font != null)
            {
                inheritedRowStyle.Font = alternatingRowsDefaultCellStyle.Font;
            }
            else
            {
                inheritedRowStyle.Font = dataGridViewStyle.Font;
            }

            if (rowStyle != null && !rowStyle.IsNullValueDefault)
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

            if (rowStyle != null && !rowStyle.IsDataSourceNullValueDefault)
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

            if (rowStyle != null && rowStyle.Format.Length != 0)
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

            if (rowStyle != null && !rowStyle.IsFormatProviderDefault)
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

            if (rowStyle != null && rowStyle.Alignment != DataGridViewContentAlignment.NotSet)
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

            if (rowStyle != null && rowStyle.WrapMode != DataGridViewTriState.NotSet)
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

            if (rowStyle != null && rowStyle.Tag != null)
            {
                inheritedRowStyle.Tag = rowStyle.Tag;
            }
            else if (rowsDefaultCellStyle.Tag != null && (rowIndex % 2 == 0 || alternatingRowsDefaultCellStyle.Tag == null))
            {
                inheritedRowStyle.Tag = rowsDefaultCellStyle.Tag;
            }
            else if (rowIndex % 2 == 1 && alternatingRowsDefaultCellStyle.Tag != null)
            {
                inheritedRowStyle.Tag = alternatingRowsDefaultCellStyle.Tag;
            }
            else
            {
                inheritedRowStyle.Tag = dataGridViewStyle.Tag;
            }

            if (rowStyle != null && rowStyle.Padding != Padding.Empty)
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
                dataGridViewRow = (DataGridViewRow)Activator.CreateInstance(thisType);
            }

            if (dataGridViewRow != null)
            {
                base.CloneInternal(dataGridViewRow);
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

            return dataGridViewRow;
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
            if (dataGridView == null)
            {
                throw new ArgumentNullException(nameof(dataGridView));
            }
            if (DataGridView != null)
            {
                throw new InvalidOperationException(SR.DataGridView_RowAlreadyBelongsToDataGridView);
            }

            DataGridViewCellCollection cells = Cells;
            // Clearing up the potential existing cells. We fill up the cells collection from scratch.
            cells.Clear();
            DataGridViewColumnCollection dataGridViewColumns = dataGridView.Columns;
            foreach (DataGridViewColumn dataGridViewColumn in dataGridViewColumns)
            {
                if (dataGridViewColumn.CellTemplate == null)
                {
                    throw new InvalidOperationException(SR.DataGridView_AColumnHasNoCellTemplate);
                }

                DataGridViewCell dgvcNew = (DataGridViewCell)dataGridViewColumn.CellTemplate.Clone();
                cells.Add(dgvcNew);
            }
        }
        public void CreateCells(DataGridView dataGridView, params object[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

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
            if (DataGridView != null)
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
        protected internal virtual void DrawFocus(Graphics graphics,
            Rectangle clipBounds,
            Rectangle bounds,
            int rowIndex,
            DataGridViewElementStates rowState,
            DataGridViewCellStyle cellStyle,
            bool cellsPaintSelectionBackground)
        {
            if (DataGridView == null)
            {
                throw new InvalidOperationException(SR.DataGridView_RowDoesNotYetBelongToDataGridView);
            }
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

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

        public ContextMenuStrip GetContextMenuStrip(int rowIndex)
        {
            ContextMenuStrip contextMenuStrip = ContextMenuStripInternal;
            if (DataGridView != null)
            {
                if (rowIndex == -1)
                {
                    throw new InvalidOperationException(SR.DataGridView_InvalidOperationOnSharedRow);
                }
                if (rowIndex < 0 || rowIndex >= DataGridView.Rows.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }

                if (DataGridView.VirtualMode || DataGridView.DataSource != null)
                {
                    contextMenuStrip = DataGridView.OnRowContextMenuStripNeeded(rowIndex, contextMenuStrip);
                }
            }

            return contextMenuStrip;
        }

        internal bool GetDisplayed(int rowIndex)
        {
            // You would think that only attached and visible rows can be displayed.
            // Actually this assertion is wrong when the row is being deleted.
            // Debug.Assert(!displayed || (DataGridView != null && DataGridView.Visible && GetVisible(rowIndex)));
            return (GetState(rowIndex) & DataGridViewElementStates.Displayed) != 0;
        }

        public string GetErrorText(int rowIndex)
        {
            string errorText = ErrorTextInternal;
            if (DataGridView != null)
            {
                if (rowIndex == -1)
                {
                    throw new InvalidOperationException(SR.DataGridView_InvalidOperationOnSharedRow);
                }
                if (rowIndex < 0 || rowIndex >= DataGridView.Rows.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }

                if (string.IsNullOrEmpty(errorText) &&
                    DataGridView.DataSource != null &&
                    rowIndex != DataGridView.NewRowIndex)
                {
                    errorText = DataGridView.DataConnection.GetError(rowIndex);
                }
                if (DataGridView.DataSource != null || DataGridView.VirtualMode)
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
            Debug.Assert(rowIndex >= -1);
            GetHeightInfo(rowIndex, out int height, out int minimumHeight);
            return height;
        }

        internal int GetMinimumHeight(int rowIndex)
        {
            Debug.Assert(rowIndex >= -1);
            GetHeightInfo(rowIndex, out int height, out int minimumHeight);
            return minimumHeight;
        }

        public virtual int GetPreferredHeight(int rowIndex, DataGridViewAutoSizeRowMode autoSizeRowMode, bool fixedWidth)
        {
            if (((DataGridViewAutoSizeRowCriteriaInternal)autoSizeRowMode & InvalidDataGridViewAutoSizeRowCriteriaInternalMask) != 0)
            {
                throw new InvalidEnumArgumentException(nameof(autoSizeRowMode), (int)autoSizeRowMode, typeof(DataGridViewAutoSizeRowMode));
            }
            if (!(DataGridView == null || (rowIndex >= 0 && rowIndex < DataGridView.Rows.Count)))
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            if (DataGridView == null)
            {
                return -1;
            }

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
                   (DataGridView != null && DataGridView.ReadOnly);
        }

        internal DataGridViewTriState GetResizable(int rowIndex)
        {
            if ((GetState(rowIndex) & DataGridViewElementStates.ResizableSet) != 0)
            {
                return ((GetState(rowIndex) & DataGridViewElementStates.Resizable) != 0) ? DataGridViewTriState.True : DataGridViewTriState.False;
            }

            if (DataGridView != null)
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
            if (!(DataGridView == null || (rowIndex >= 0 && rowIndex < DataGridView.Rows.Count)))
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            if (DataGridView == null || DataGridView.Rows.SharedRow(rowIndex).Index != -1)
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
            Debug.Assert(DataGridView != null);
            DataGridView.Rows.InvalidateCachedRowCount(elementState);
            DataGridView.Rows.InvalidateCachedRowsHeight(elementState);
            DataGridView.OnDataGridViewElementStateChanged(this, sharedRowIndex, elementState);
        }

        internal void OnSharedStateChanging(int sharedRowIndex, DataGridViewElementStates elementState)
        {
            Debug.Assert(DataGridView != null);
            DataGridView.OnDataGridViewElementStateChanging(this, sharedRowIndex, elementState);
        }
        protected internal virtual void Paint(Graphics graphics,
            Rectangle clipBounds,
            Rectangle rowBounds,
            int rowIndex,
            DataGridViewElementStates rowState,
            bool isFirstDisplayedRow,
            bool isLastVisibleRow)
        {
            if (DataGridView == null)
            {
                throw new InvalidOperationException(SR.DataGridView_RowDoesNotYetBelongToDataGridView);
            }
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            DataGridView dataGridView = DataGridView;
            Rectangle updatedClipBounds = clipBounds;
            DataGridViewRow sharedRow = dataGridView.Rows.SharedRow(rowIndex);
            DataGridViewCellStyle inheritedRowStyle = new DataGridViewCellStyle();
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
            updatedClipBounds = dgvrprepe.ClipBounds;

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
        protected internal virtual void PaintCells(Graphics graphics,
            Rectangle clipBounds,
            Rectangle rowBounds,
            int rowIndex,
            DataGridViewElementStates rowState,
            bool isFirstDisplayedRow,
            bool isLastVisibleRow,
            DataGridViewPaintParts paintParts)
        {
            if (DataGridView == null)
            {
                throw new InvalidOperationException(SR.DataGridView_RowDoesNotYetBelongToDataGridView);
            }
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }
            if (paintParts < DataGridViewPaintParts.None || paintParts > DataGridViewPaintParts.All)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewPaintPartsCombination, "paintParts"));
            }

            DataGridView dataGridView = DataGridView;
            Rectangle cellBounds = rowBounds;
            int cx = (dataGridView.RowHeadersVisible ? dataGridView.RowHeadersWidth : 0);
            bool isFirstDisplayedColumn = true;
            DataGridViewElementStates cellState = DataGridViewElementStates.None;
            DataGridViewCell cell;
            DataGridViewCellStyle inheritedCellStyle = new DataGridViewCellStyle();
            DataGridViewColumn dataGridViewColumnNext = null;
            DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder = new DataGridViewAdvancedBorderStyle(), dgvabsEffective;

            // first paint the potential visible frozen cells
            DataGridViewColumn dataGridViewColumn = dataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);
            while (dataGridViewColumn != null)
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

                    cell.GetInheritedStyle(inheritedCellStyle, rowIndex, true);

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

                    Region clipRegion = null;
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

                    dataGridViewColumn = (DataGridViewColumn)dataGridView.Columns[dataGridView.FirstDisplayedScrollingColumnIndex];
                    Debug.Assert(dataGridViewColumn.Visible && !dataGridViewColumn.Frozen);

                    while (dataGridViewColumn != null)
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

                            cell.GetInheritedStyle(inheritedCellStyle, rowIndex, true);

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

                    if (clipRegion != null)
                    {
                        graphics.Clip = clipRegion;
                        clipRegion.Dispose();
                    }
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected internal virtual void PaintHeader(Graphics graphics,
            Rectangle clipBounds,
            Rectangle rowBounds,
            int rowIndex,
            DataGridViewElementStates rowState,
            bool isFirstDisplayedRow,
            bool isLastVisibleRow,
            DataGridViewPaintParts paintParts)
        {
            if (DataGridView == null)
            {
                throw new InvalidOperationException(SR.DataGridView_RowDoesNotYetBelongToDataGridView);
            }
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }
            if (paintParts < DataGridViewPaintParts.None || paintParts > DataGridViewPaintParts.All)
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
                    DataGridViewCellStyle inheritedCellStyle = new DataGridViewCellStyle();
                    DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder = new DataGridViewAdvancedBorderStyle(), dgvabsEffective;
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
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (DataGridView != null)
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
            Debug.Assert(values != null);
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

        public override string ToString()
        {
            var sb = new StringBuilder(36);
            sb.Append("DataGridViewRow { Index=");
            sb.Append(Index.ToString(CultureInfo.CurrentCulture));
            sb.Append(" }");
            return sb.ToString();
        }

        [ComVisible(true)]
        protected class DataGridViewRowAccessibleObject : AccessibleObject
        {
            private int[] runtimeId;
            private DataGridViewRow owner;
            private DataGridViewSelectedRowCellsAccessibleObject selectedCellsAccessibilityObject = null;

            public DataGridViewRowAccessibleObject()
            {
            }

            public DataGridViewRowAccessibleObject(DataGridViewRow owner)
            {
                this.owner = owner;
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (owner == null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    if (owner.DataGridView == null)
                    {
                        return Rectangle.Empty;
                    }

                    Rectangle rowRect = owner.DataGridView.RectangleToScreen(owner.DataGridView.GetRowDisplayRectangle(owner.Index, false /*cutOverflow*/));

                    int horizontalScrollBarHeight = 0;
                    if (owner.DataGridView.HorizontalScrollBarVisible)
                    {
                        horizontalScrollBarHeight = owner.DataGridView.HorizontalScrollBarHeight;
                    }

                    Rectangle dataGridViewRect = ParentPrivate.Bounds;

                    int columnHeadersHeight = 0;
                    if (owner.DataGridView.ColumnHeadersVisible)
                    {
                        columnHeadersHeight = owner.DataGridView.ColumnHeadersHeight;
                    }

                    int rowRectBottom = rowRect.Bottom;
                    if ((dataGridViewRect.Bottom - horizontalScrollBarHeight) < rowRectBottom)
                    {
                        rowRectBottom = dataGridViewRect.Bottom - owner.DataGridView.BorderWidth - horizontalScrollBarHeight;
                    }

                    if ((dataGridViewRect.Top + columnHeadersHeight) > rowRect.Top)
                    {
                        rowRect.Height = 0;
                    }
                    else
                    {
                        rowRect.Height = rowRectBottom - rowRect.Top;
                    }

                    return rowRect;
                }
            }
            public override string Name
            {
                get
                {
                    if (owner == null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    return string.Format(SR.DataGridView_AccRowName, owner.Index.ToString(CultureInfo.CurrentCulture));
                }
            }
            public DataGridViewRow Owner
            {
                get => owner;
                set
                {
                    if (owner != null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerAlreadySet);
                    }

                    owner = value;
                }
            }
            public override AccessibleObject Parent => ParentPrivate;

            private AccessibleObject ParentPrivate
            {
                get
                {
                    if (owner == null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    return owner.DataGridView?.AccessibilityObject;
                }
            }
            public override AccessibleRole Role => AccessibleRole.Row;

            internal override int[] RuntimeId
            {
                get
                {
                    if (runtimeId == null)
                    {
                        runtimeId = new int[]
                        {
                            RuntimeIDFirstItem, // first item is static - 0x2a,
                            Parent.GetHashCode(),
                            GetHashCode()
                        };
                    }

                    return runtimeId;
                }
            }

            private AccessibleObject SelectedCellsAccessibilityObject
            {
                get
                {
                    if (owner == null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    if (selectedCellsAccessibilityObject == null)
                    {
                        selectedCellsAccessibilityObject = new DataGridViewSelectedRowCellsAccessibleObject(owner);
                    }
                    return selectedCellsAccessibilityObject;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    if (owner == null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    AccessibleStates accState = AccessibleStates.Selectable;

                    bool allCellsAreSelected = true;
                    if (owner.Selected)
                    {
                        allCellsAreSelected = true;
                    }
                    else
                    {
                        for (int i = 0; i < owner.Cells.Count; i++)
                        {
                            if (!owner.Cells[i].Selected)
                            {
                                allCellsAreSelected = false;
                                break;
                            }
                        }
                    }

                    if (allCellsAreSelected)
                    {
                        accState |= AccessibleStates.Selected;
                    }

                    if (owner.DataGridView != null)
                    {
                        Rectangle rowBounds = owner.DataGridView.GetRowDisplayRectangle(owner.Index, true /*cutOverflow*/);
                        if (!rowBounds.IntersectsWith(owner.DataGridView.ClientRectangle))
                        {
                            accState |= AccessibleStates.Offscreen;
                        }
                    }

                    return accState;
                }
            }
            public override string Value
            {
                get
                {
                    if (owner == null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }
                    if (owner.DataGridView != null && owner.DataGridView.AllowUserToAddRows && owner.Index == owner.DataGridView.NewRowIndex)
                    {
                        return SR.DataGridView_AccRowCreateNew;
                    }

                    StringBuilder sb = new StringBuilder(1024);

                    int childCount = GetChildCount();

                    // filter out the row header acc object even when DataGridView::RowHeadersVisible is turned on
                    int startIndex = owner.DataGridView != null && owner.DataGridView.RowHeadersVisible ? 1 : 0;

                    for (int i = startIndex; i < childCount; i++)
                    {
                        AccessibleObject cellAccObj = GetChild(i);
                        if (cellAccObj != null)
                        {
                            sb.Append(cellAccObj.Value);
                        }

                        if (i != childCount - 1)
                        {
                            sb.Append(';');
                        }
                    }

                    return sb.ToString();
                }
            }

            public override AccessibleObject GetChild(int index)
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                }

                if (owner.DataGridView == null)
                {
                    return null;
                }

                if (index == 0 && owner.DataGridView.RowHeadersVisible)
                {
                    return owner.HeaderCell.AccessibilityObject;
                }
                else
                {
                    // decrement the index because the first child is the RowHeaderCell AccessibilityObject
                    if (owner.DataGridView.RowHeadersVisible)
                    {
                        index--;
                    }
                    Debug.Assert(index >= 0);
                    int columnIndex = owner.DataGridView.Columns.ActualDisplayIndexToColumnIndex(index, DataGridViewElementStates.Visible);
                    return owner.Cells[columnIndex].AccessibilityObject;
                }
            }

            public override int GetChildCount()
            {
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                }
                if (owner.DataGridView == null)
                {
                    return 0;
                }

                int result = owner.DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible);
                if (owner.DataGridView.RowHeadersVisible)
                {
                    // + 1 comes from the row header cell accessibility object
                    result++;
                }

                return result;
            }

            public override AccessibleObject GetSelected() => SelectedCellsAccessibilityObject;

            public override AccessibleObject GetFocused()
            {
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                }

                if (owner.DataGridView != null &&
                    owner.DataGridView.Focused &&
                    owner.DataGridView.CurrentCell != null &&
                    owner.DataGridView.CurrentCell.RowIndex == owner.Index)
                {
                    return owner.DataGridView.CurrentCell.AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                }

                if (owner.DataGridView == null)
                {
                    return null;
                }

                switch (navigationDirection)
                {
                    case AccessibleNavigation.Down:
                    case AccessibleNavigation.Next:
                        if (owner.Index != owner.DataGridView.Rows.GetLastRow(DataGridViewElementStates.Visible))
                        {
                            int nextVisibleRow = owner.DataGridView.Rows.GetNextRow(owner.Index, DataGridViewElementStates.Visible);
                            int actualDisplayIndex = owner.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible, 0, nextVisibleRow);
                            if (owner.DataGridView.ColumnHeadersVisible)
                            {
                                return owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex + 1);
                            }
                            else
                            {
                                return owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex);
                            }
                        }
                        else
                        {
                            return null;
                        }
                    case AccessibleNavigation.Up:
                    case AccessibleNavigation.Previous:
                        if (owner.Index != owner.DataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible))
                        {
                            int previousVisibleRow = owner.DataGridView.Rows.GetPreviousRow(owner.Index, DataGridViewElementStates.Visible);
                            int actualDisplayIndex = owner.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible, 0, previousVisibleRow);
                            if (owner.DataGridView.ColumnHeadersVisible)
                            {
                                return owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex + 1);
                            }
                            else
                            {
                                return owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex);
                            }
                        }
                        else if (owner.DataGridView.ColumnHeadersVisible)
                        {
                            // return the top row header acc obj
                            return ParentPrivate.GetChild(0);
                        }
                        else
                        {
                            // if this is the first row and the DataGridView RowHeaders are not visible return null;
                            return null;
                        }
                    case AccessibleNavigation.FirstChild:
                        if (GetChildCount() == 0)
                        {
                            return null;
                        }
                        else
                        {
                            return GetChild(0);
                        }
                    case AccessibleNavigation.LastChild:
                        int childCount = GetChildCount();
                        if (childCount == 0)
                        {
                            return null;
                        }
                        else
                        {
                            return GetChild(childCount - 1);
                        }
                    default:
                        return null;
                }
            }

            public override void Select(AccessibleSelection flags)
            {
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                }

                DataGridView dataGridView = owner.DataGridView;
                if (dataGridView == null)
                {
                    return;
                }
                if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
                {
                    dataGridView.Focus();
                }
                if ((flags & AccessibleSelection.TakeSelection) == AccessibleSelection.TakeSelection)
                {
                    if (owner.Cells.Count > 0)
                    {
                        if (dataGridView.CurrentCell != null && dataGridView.CurrentCell.OwningColumn != null)
                        {
                            dataGridView.CurrentCell = owner.Cells[dataGridView.CurrentCell.OwningColumn.Index]; // Do not change old selection
                        }
                        else
                        {
                            int firstVisibleCell = dataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible).Index;
                            if (firstVisibleCell > -1)
                            {
                                dataGridView.CurrentCell = owner.Cells[firstVisibleCell]; // Do not change old selection
                            }
                        }
                    }
                }

                if ((flags & AccessibleSelection.AddSelection) == AccessibleSelection.AddSelection && (flags & AccessibleSelection.TakeSelection) == 0)
                {
                    if (dataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect || dataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
                    {
                        owner.Selected = true;
                    }
                }

                if ((flags & AccessibleSelection.RemoveSelection) == AccessibleSelection.RemoveSelection &&
                    (flags & (AccessibleSelection.AddSelection | AccessibleSelection.TakeSelection)) == 0)
                {
                    owner.Selected = false;
                }
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
            {
                {
                    if (Owner == null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    DataGridView dataGridView = owner.DataGridView;

                    switch (direction)
                    {
                        case UnsafeNativeMethods.NavigateDirection.Parent:
                            return Parent;
                        case UnsafeNativeMethods.NavigateDirection.NextSibling:
                            return Navigate(AccessibleNavigation.Next);
                        case UnsafeNativeMethods.NavigateDirection.PreviousSibling:
                            return Navigate(AccessibleNavigation.Previous);
                        case UnsafeNativeMethods.NavigateDirection.FirstChild:
                            return Navigate(AccessibleNavigation.FirstChild);
                        case UnsafeNativeMethods.NavigateDirection.LastChild:
                            return Navigate(AccessibleNavigation.LastChild);
                        default:
                            return null;
                    }
                }
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return owner.DataGridView.AccessibilityObject;
                }
            }

            internal override bool IsPatternSupported(int patternId)
            {
                return patternId.Equals(NativeMethods.UIA_LegacyIAccessiblePatternId);
            }

            internal override object GetPropertyValue(int propertyId)
            {
                switch (propertyId)
                {
                    case NativeMethods.UIA_NamePropertyId:
                        return Name;
                    case NativeMethods.UIA_IsEnabledPropertyId:
                        return Owner.DataGridView.Enabled;
                    case NativeMethods.UIA_HelpTextPropertyId:
                        return Help ?? string.Empty;
                    case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                    case NativeMethods.UIA_HasKeyboardFocusPropertyId:
                    case NativeMethods.UIA_IsPasswordPropertyId:
                        return false;
                    case NativeMethods.UIA_IsOffscreenPropertyId:
                        return (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
                    case NativeMethods.UIA_AccessKeyPropertyId:
                        return string.Empty;
                }

                return base.GetPropertyValue(propertyId);
            }
        }

        private class DataGridViewSelectedRowCellsAccessibleObject : AccessibleObject
        {
            private readonly DataGridViewRow owner;

            internal DataGridViewSelectedRowCellsAccessibleObject(DataGridViewRow owner)
            {
                this.owner = owner;
            }

            public override string Name => SR.DataGridView_AccSelectedRowCellsName;

            public override AccessibleObject Parent => owner.AccessibilityObject;

            public override AccessibleRole Role => AccessibleRole.Grouping;

            public override AccessibleStates State
            {
                get => AccessibleStates.Selected | AccessibleStates.Selectable;
            }

            public override string Value => Name;

            public override AccessibleObject GetChild(int index)
            {
                if (index < GetChildCount())
                {
                    int selectedCellsCount = -1;
                    for (int i = 1; i < owner.AccessibilityObject.GetChildCount(); i++)
                    {
                        if ((owner.AccessibilityObject.GetChild(i).State & AccessibleStates.Selected) == AccessibleStates.Selected)
                        {
                            selectedCellsCount++;
                        }

                        if (selectedCellsCount == index)
                        {
                            return owner.AccessibilityObject.GetChild(i);
                        }
                    }

                    Debug.Assert(false, "we should have found already the selected cell");
                    return null;
                }
                else
                {
                    return null;
                }
            }

            public override int GetChildCount()
            {
                int selectedCellsCount = 0;

                // start the enumeration from 1, because the first acc obj in the data grid view row is the row header cell
                for (int i = 1; i < owner.AccessibilityObject.GetChildCount(); i++)
                {
                    if ((owner.AccessibilityObject.GetChild(i).State & AccessibleStates.Selected) == AccessibleStates.Selected)
                    {
                        selectedCellsCount++;
                    }
                }

                return selectedCellsCount;
            }

            public override AccessibleObject GetSelected() => this;

            public override AccessibleObject GetFocused()
            {
                if (owner.DataGridView.CurrentCell != null && owner.DataGridView.CurrentCell.Selected)
                {
                    return owner.DataGridView.CurrentCell.AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                switch (navigationDirection)
                {
                    case AccessibleNavigation.FirstChild:
                        if (GetChildCount() > 0)
                        {
                            return GetChild(0);
                        }
                        else
                        {
                            return null;
                        }
                    case AccessibleNavigation.LastChild:
                        if (GetChildCount() > 0)
                        {
                            return GetChild(GetChildCount() - 1);
                        }
                        else
                        {
                            return null;
                        }
                    default:
                        return null;
                }
            }
        }
    }
}

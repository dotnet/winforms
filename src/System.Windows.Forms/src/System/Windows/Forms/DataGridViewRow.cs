// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Text;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms.VisualStyles;
    using System.Security.Permissions;
    using System.Globalization;

    /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow"]/*' />
    /// <devdoc>
    ///    <para>Identifies a row in the dataGridView.</para>
    /// </devdoc>
    [
    TypeConverterAttribute(typeof(DataGridViewRowConverter))    
    ]
    public class DataGridViewRow : DataGridViewBand
    {
        private static Type rowType = typeof(DataGridViewRow);
        private static readonly int PropRowErrorText = PropertyStore.CreateKey();
        private static readonly int PropRowAccessibilityObject = PropertyStore.CreateKey();

        private const DataGridViewAutoSizeRowCriteriaInternal invalidDataGridViewAutoSizeRowCriteriaInternalMask = ~(DataGridViewAutoSizeRowCriteriaInternal.Header | DataGridViewAutoSizeRowCriteriaInternal.AllColumns);

        internal const int defaultMinRowThickness = 3;

        private DataGridViewCellCollection rowCells;

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.DataGridViewRow"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.DataGridViewRow'/> class.
        ///    </para>
        /// </devdoc>
        public DataGridViewRow() : base()
        {
            this.bandIsRow = true;
            this.MinimumThickness = defaultMinRowThickness;
            this.Thickness = Control.DefaultFont.Height + 9;
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.AccessibilityObject"]/*' />
        [
            Browsable(false)
        ]
        public AccessibleObject AccessibilityObject
        {
            get
            {
                AccessibleObject result = (AccessibleObject) this.Properties.GetObject(PropRowAccessibilityObject);

                if (result == null)
                {
                    result = this.CreateAccessibilityInstance();
                    this.Properties.SetObject(PropRowAccessibilityObject, result);
                }

                return result;
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.Cells"]/*' />
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        ]
        public DataGridViewCellCollection Cells
        {
            get
            {
                if (this.rowCells == null)
                {
                    this.rowCells = CreateCellsInstance();
                }
                return this.rowCells;
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.ContextMenuStrip"]/*' />
        [
            DefaultValue(null),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_RowContextMenuStripDescr))
        ]
        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return base.ContextMenuStrip;
            }
            set
            {
                base.ContextMenuStrip = value;
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.DataBoundItem"]/*' />
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public object DataBoundItem
        {
            get
            {
                if (this.DataGridView != null &&
                    this.DataGridView.DataConnection != null &&
                    this.Index > -1 &&
                    this.Index != this.DataGridView.NewRowIndex)
                {
                    return this.DataGridView.DataConnection.CurrencyManager[this.Index];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.DefaultCellStyle"]/*' />
        [
            Browsable(true),
            NotifyParentProperty(true),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_RowDefaultCellStyleDescr))
        ]
        public override DataGridViewCellStyle DefaultCellStyle
        {
            get
            {
                return base.DefaultCellStyle;
            }
            set
            {
                if (this.DataGridView != null && this.Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, "DefaultCellStyle"));
                }
                base.DefaultCellStyle = value;
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.Displayed"]/*' />
        [
            Browsable(false)
        ]
        public override bool Displayed
        {
            get
            {
                if (this.DataGridView != null && this.Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedRow, "Displayed"));
                }
                return GetDisplayed(this.Index);
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.DividerHeight"]/*' />
        [
            DefaultValue(0),
            NotifyParentProperty(true),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_RowDividerHeightDescr))
        ]
        public int DividerHeight
        {
            get 
            {
                return this.DividerThickness;
            }
            set 
            {
                if (this.DataGridView != null && this.Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, "DividerHeight"));
                }
                this.DividerThickness = value;
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.ErrorText"]/*' />
        [
            DefaultValue(""),
            NotifyParentProperty(true),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_RowErrorTextDescr))
        ]
        public string ErrorText
        {
            get
            {
                Debug.Assert(this.Index >= -1);
                return GetErrorText(this.Index);
            }
            set
            {
                this.ErrorTextInternal = value;
            }
        }

        private string ErrorTextInternal
        {
            get
            {
                object errorText = this.Properties.GetObject(PropRowErrorText);
                return (errorText == null) ? string.Empty : (string)errorText;
            }
            set
            {
                string errorText = this.ErrorTextInternal;
                if (!string.IsNullOrEmpty(value) || this.Properties.ContainsObject(PropRowErrorText))
                {
                    this.Properties.SetObject(PropRowErrorText, value);
                }
                if (this.DataGridView != null && !errorText.Equals(this.ErrorTextInternal))
                {
                    this.DataGridView.OnRowErrorTextChanged(this);
                }
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.Frozen"]/*' />
        [
            Browsable(false),
        ]
        public override bool Frozen
        {
            get
            {
                if (this.DataGridView != null && this.Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedRow, "Frozen"));
                }
                return GetFrozen(this.Index);
            }
            set
            {
                if (this.DataGridView != null && this.Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, "Frozen"));
                }
                base.Frozen = value;
            }
        }

        internal bool HasErrorText
        {
            get
            {
                return this.Properties.ContainsObject(PropRowErrorText) && this.Properties.GetObject(PropRowErrorText) != null;
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.HeaderCell"]/*' />
        [
            Browsable(false), 
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public DataGridViewRowHeaderCell HeaderCell
        {
            get
            {
                return (DataGridViewRowHeaderCell) base.HeaderCellCore;
            }
            set
            {
                base.HeaderCellCore = value;
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.Height"]/*' />
        [
            DefaultValue(22),
            NotifyParentProperty(true),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_RowHeightDescr))
        ]
        public int Height
        {
            get
            {
                return this.Thickness;
            }
            set
            {
                if (this.DataGridView != null && this.Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, "Height"));
                }
                this.Thickness = value;
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.InheritedStyle"]/*' />
        public override DataGridViewCellStyle InheritedStyle
        {
            get
            {
                if (this.Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedRow, "InheritedStyle"));
                }

                DataGridViewCellStyle inheritedRowStyle = new DataGridViewCellStyle();
                BuildInheritedRowStyle(this.Index, inheritedRowStyle);
                return inheritedRowStyle;
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.IsNewRow"]/*' />
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool IsNewRow
        {
            get
            {
                return this.DataGridView != null && this.DataGridView.NewRowIndex == this.Index;
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.MinimumHeight"]/*' />
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int MinimumHeight
        {
            get
            {
                return this.MinimumThickness;
            }
            set
            {
                if (this.DataGridView != null && this.Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, "MinimumHeight"));
                }
                this.MinimumThickness = value;
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.ReadOnly"]/*' />
        [
            Browsable(true),
            DefaultValue(false),
            NotifyParentProperty(true),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_RowReadOnlyDescr))
        ]
        public override bool ReadOnly
        {
            get
            {
                if (this.DataGridView != null && this.Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedRow, "ReadOnly"));
                }
                return GetReadOnly(this.Index);
            }
            set
            {
                base.ReadOnly = value;
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.Resizable"]/*' />
        [
            NotifyParentProperty(true),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_RowResizableDescr))
        ]
        public override DataGridViewTriState Resizable
        {
            get
            {
                if (this.DataGridView != null && this.Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedRow, "Resizable"));
                }
                return GetResizable(this.Index);
            }
            set
            {
                base.Resizable = value;
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.Selected"]/*' />
        public override bool Selected
        {
            get
            {
                if (this.DataGridView != null && this.Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedRow, "Selected"));
                }
                return GetSelected(this.Index);
            }
            set
            {
                base.Selected = value;
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.State"]/*' />
        public override DataGridViewElementStates State
        {
            get
            {
                if (this.DataGridView != null && this.Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedRow, "State"));
                }
                return GetState(this.Index);
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.Visible"]/*' />
        [
            Browsable(false)
        ]
        public override bool Visible
        {
            get
            {
                if (this.DataGridView != null && this.Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedRow, "Visible"));
                }
                return GetVisible(this.Index);
            }
            set
            {
                if (this.DataGridView != null && this.Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, "Visible"));
                }
                base.Visible = value;
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.AdjustRowHeaderBorderStyle"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual DataGridViewAdvancedBorderStyle AdjustRowHeaderBorderStyle(DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStyleInput,
            DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder,
            bool singleVerticalBorderAdded,
            bool singleHorizontalBorderAdded,
            bool isFirstDisplayedRow, 
            bool isLastVisibleRow)
        {
            if (this.DataGridView != null && this.DataGridView.ApplyVisualStylesToHeaderCells)
            {
                switch (dataGridViewAdvancedBorderStyleInput.All)
                {
                    case DataGridViewAdvancedCellBorderStyle.Inset:
                        if (isFirstDisplayedRow && !this.DataGridView.ColumnHeadersVisible)
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
                        if (isFirstDisplayedRow && !this.DataGridView.ColumnHeadersVisible)
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
                        if (isFirstDisplayedRow && !this.DataGridView.ColumnHeadersVisible)
                        {
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                        }
                        else
                        {
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.None;
                        }
                        if (this.DataGridView != null && this.DataGridView.RightToLeftInternal)
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
                        if (isFirstDisplayedRow && !this.DataGridView.ColumnHeadersVisible)
                        {
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                        }
                        else
                        {
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.None;
                        }
                        if (this.DataGridView != null && this.DataGridView.RightToLeftInternal)
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
                        if (isFirstDisplayedRow && !this.DataGridView.ColumnHeadersVisible)
                        {
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                        }
                        else
                        {
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.None;
                        }
                        if (this.DataGridView != null && this.DataGridView.RightToLeftInternal)
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
                        if (isFirstDisplayedRow && !this.DataGridView.ColumnHeadersVisible)
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
                        if (this.DataGridView != null && this.DataGridView.RightToLeftInternal)
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
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = this.DataGridView.ColumnHeadersVisible ? DataGridViewAdvancedCellBorderStyle.Outset : DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                        }
                        else
                        {
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.OutsetPartial;
                        }
                        dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = isLastVisibleRow ? DataGridViewAdvancedCellBorderStyle.Outset : DataGridViewAdvancedCellBorderStyle.OutsetPartial;
                        return dataGridViewAdvancedBorderStylePlaceholder;

                    case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                        if (this.DataGridView != null && this.DataGridView.RightToLeftInternal)
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
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = this.DataGridView.ColumnHeadersVisible ? DataGridViewAdvancedCellBorderStyle.Outset : DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                        }
                        else
                        {
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                        }
                        dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = DataGridViewAdvancedCellBorderStyle.Outset;
                        return dataGridViewAdvancedBorderStylePlaceholder;

                    case DataGridViewAdvancedCellBorderStyle.InsetDouble:
                        if (this.DataGridView != null && this.DataGridView.RightToLeftInternal)
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
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = this.DataGridView.ColumnHeadersVisible ? DataGridViewAdvancedCellBorderStyle.Inset : DataGridViewAdvancedCellBorderStyle.InsetDouble;
                        }
                        else
                        {
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                        }
                        dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = DataGridViewAdvancedCellBorderStyle.Inset;
                        return dataGridViewAdvancedBorderStylePlaceholder;

                    case DataGridViewAdvancedCellBorderStyle.Single:
                        if (!isFirstDisplayedRow || this.DataGridView.ColumnHeadersVisible)
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
            if (this.HeaderCell.HasStyle)
            {
                cellStyle = this.HeaderCell.Style;
                Debug.Assert(cellStyle != null);
            }

            DataGridViewCellStyle rowHeadersStyle = this.DataGridView.RowHeadersDefaultCellStyle;
            Debug.Assert(rowHeadersStyle != null);

            DataGridViewCellStyle dataGridViewStyle = this.DataGridView.DefaultCellStyle;
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
            Debug.Assert(this.DataGridView != null);

            DataGridViewCellStyle rowStyle = null;
            if (this.HasDefaultCellStyle)
            {
                rowStyle = this.DefaultCellStyle;
                Debug.Assert(rowStyle != null);
            }

            DataGridViewCellStyle dataGridViewStyle = this.DataGridView.DefaultCellStyle;
            Debug.Assert(dataGridViewStyle != null);

            DataGridViewCellStyle rowsDefaultCellStyle = this.DataGridView.RowsDefaultCellStyle;
            Debug.Assert(rowsDefaultCellStyle != null);

            DataGridViewCellStyle alternatingRowsDefaultCellStyle = this.DataGridView.AlternatingRowsDefaultCellStyle;
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

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.Clone"]/*' />
        public override object Clone()
        {
            DataGridViewRow dataGridViewRow;
            Type thisType = this.GetType();

            if (thisType == rowType) //performance improvement
            {
                dataGridViewRow = new DataGridViewRow();
            }
            else
            {
                // 

                dataGridViewRow = (DataGridViewRow) System.Activator.CreateInstance(thisType);
            }
            if (dataGridViewRow != null)
            {
                base.CloneInternal(dataGridViewRow);
                if (this.HasErrorText)
                {
                    dataGridViewRow.ErrorText = this.ErrorTextInternal;
                }
                if (this.HasHeaderCell)
                {
                    dataGridViewRow.HeaderCell = (DataGridViewRowHeaderCell) this.HeaderCell.Clone();
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
                for (int i = 0; i < cellsCount; i ++)
                {
                    DataGridViewCell dataGridViewCell = rowTemplate.Cells[i];
                    DataGridViewCell dgvcNew = (DataGridViewCell) dataGridViewCell.Clone();
                    cells[i] = dgvcNew;
                }
                this.Cells.AddRange(cells);
            }

            // 


            /* object[] args = new object[1];
            foreach (DataGridViewCell tc in bandTemplate.Cells)
            {
                args[0] = tc;
   
                DataGridViewCell dgvcNew = (DataGridViewCell) System.Activator.CreateInstance(tc.GetType(), args);
                Cells.Add(dgvcNew);
            } */
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.CreateAccessibilityInstance"]/*' />
        protected virtual AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewRowAccessibleObject(this);
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.CreateCells1"]/*' />
        public void CreateCells(DataGridView dataGridView)
        {
            if (dataGridView == null)
            {
                throw new ArgumentNullException(nameof(dataGridView));
            }
            if (this.DataGridView != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_RowAlreadyBelongsToDataGridView));
            }
            DataGridViewCellCollection cells = this.Cells;
            // Clearing up the potential existing cells. We fill up the cells collection from scratch.
            cells.Clear();
            DataGridViewColumnCollection dataGridViewColumns = dataGridView.Columns;
            foreach (DataGridViewColumn dataGridViewColumn in dataGridViewColumns)
            {
                if (dataGridViewColumn.CellTemplate == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_AColumnHasNoCellTemplate));
                }
                DataGridViewCell dgvcNew = (DataGridViewCell)dataGridViewColumn.CellTemplate.Clone();
                cells.Add(dgvcNew);
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.CreateCells2"]/*' />
        public void CreateCells(DataGridView dataGridView, params object[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            /* Intentionally not being strict about this. We just take what we get.
            if (dataGridView.Columns.Count != values.Length)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_WrongValueCount), "values");
            }*/

            CreateCells(dataGridView);

            Debug.Assert(this.Cells.Count == dataGridView.Columns.Count);
            SetValuesInternal(values);
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.CreateCellsInstance"]/*' />
        /// <devdoc>
        ///     Constructs the new instance of the Cells collection objects. Subclasses
        ///     should not call base.CreateCellsInstance.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual DataGridViewCellCollection CreateCellsInstance()
        {
            return new DataGridViewCellCollection(this);
        }

        internal void DetachFromDataGridView()
        {
            if (this.DataGridView != null)
            {
                this.DataGridViewInternal = null;
                this.IndexInternal = -1;
                if (this.HasHeaderCell)
                {
                  this.HeaderCell.DataGridViewInternal = null;
                }
                foreach (DataGridViewCell dataGridViewCell in this.Cells)
                {
                    dataGridViewCell.DataGridViewInternal = null;
                    if (dataGridViewCell.Selected)
                    {
                        dataGridViewCell.SelectedInternal = false;
                    }
                }
                if (this.Selected)
                {
                    this.SelectedInternal = false;
                }
            }
            Debug.Assert(this.Index == -1);
            Debug.Assert(!this.Selected);
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.DrawFocus"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected internal virtual void DrawFocus(Graphics graphics, 
            Rectangle clipBounds,
            Rectangle bounds,
            int rowIndex,
            DataGridViewElementStates rowState,
            DataGridViewCellStyle cellStyle,
            bool cellsPaintSelectionBackground)
        {
            if (this.DataGridView == null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_RowDoesNotYetBelongToDataGridView));
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

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.GetContextMenuStrip"]/*' />
        public ContextMenuStrip GetContextMenuStrip(int rowIndex)
        {
            ContextMenuStrip contextMenuStrip = this.ContextMenuStripInternal;
            if (this.DataGridView != null)
            {
                if (rowIndex == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidOperationOnSharedRow));
                }
                if (rowIndex < 0 || rowIndex >= this.DataGridView.Rows.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                if (this.DataGridView.VirtualMode || this.DataGridView.DataSource != null)
                {
                    contextMenuStrip = this.DataGridView.OnRowContextMenuStripNeeded(rowIndex, contextMenuStrip);
                }
            }
            return contextMenuStrip;
        }

        internal bool GetDisplayed(int rowIndex)
        {
            // You would think that only attached and visible rows can be displayed.
            // Actually this assertion is wrong when the row is being deleted.
            // Debug.Assert(!displayed || (this.DataGridView != null && this.DataGridView.Visible && GetVisible(rowIndex)));
            return (GetState(rowIndex) & DataGridViewElementStates.Displayed) != 0;
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.GetErrorText"]/*' />
        public string GetErrorText(int rowIndex)
        {
            string errorText = this.ErrorTextInternal;
            if (this.DataGridView != null)
            {
                if (rowIndex == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidOperationOnSharedRow));
                }
                if (rowIndex < 0 || rowIndex >= this.DataGridView.Rows.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                if (string.IsNullOrEmpty(errorText) &&
                    this.DataGridView.DataSource != null &&
                    rowIndex != this.DataGridView.NewRowIndex)
                {
                    errorText = this.DataGridView.DataConnection.GetError(rowIndex);
                }
                if (this.DataGridView.DataSource != null || this.DataGridView.VirtualMode)
                {
                    errorText = this.DataGridView.OnRowErrorTextNeeded(rowIndex, errorText);
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
            int height, minimumHeight;
            GetHeightInfo(rowIndex, out height, out minimumHeight);
            return height;
        }

        internal int GetMinimumHeight(int rowIndex)
        {
            Debug.Assert(rowIndex >= -1);
            int height, minimumHeight;
            GetHeightInfo(rowIndex, out height, out minimumHeight);
            return minimumHeight;
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.GetPreferredHeight"]/*' />
        public virtual int GetPreferredHeight(int rowIndex, DataGridViewAutoSizeRowMode autoSizeRowMode, bool fixedWidth)
        {
            // not using IsEnumValid here because this is a flags enum, using mask instead.
            if (((DataGridViewAutoSizeRowCriteriaInternal) autoSizeRowMode & invalidDataGridViewAutoSizeRowCriteriaInternalMask) != 0)
            {
                throw new InvalidEnumArgumentException(nameof(autoSizeRowMode), (int) autoSizeRowMode, typeof(DataGridViewAutoSizeRowMode));
            }
            if (!(this.DataGridView == null || (rowIndex >= 0 && rowIndex < this.DataGridView.Rows.Count)))
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            if (this.DataGridView == null)
            {
                return -1;
            }

            int preferredRowThickness = 0, preferredCellThickness;
            // take into account the preferred height of the header cell if displayed and cared about
            if (this.DataGridView.RowHeadersVisible && 
                (((DataGridViewAutoSizeRowCriteriaInternal) autoSizeRowMode) & DataGridViewAutoSizeRowCriteriaInternal.Header) != 0)
            {
                if (fixedWidth ||
                    this.DataGridView.RowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.EnableResizing ||
                    this.DataGridView.RowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.DisableResizing)
                {
                    preferredRowThickness = Math.Max(preferredRowThickness, this.HeaderCell.GetPreferredHeight(rowIndex, this.DataGridView.RowHeadersWidth));
                }
                else
                {
                    preferredRowThickness = Math.Max(preferredRowThickness, this.HeaderCell.GetPreferredSize(rowIndex).Height);
                }
            }
            if ((((DataGridViewAutoSizeRowCriteriaInternal) autoSizeRowMode) & DataGridViewAutoSizeRowCriteriaInternal.AllColumns) != 0)
            {
                foreach (DataGridViewCell dataGridViewCell in this.Cells)
                {
                    DataGridViewColumn dataGridViewColumn = this.DataGridView.Columns[dataGridViewCell.ColumnIndex];
                    if (dataGridViewColumn.Visible)
                    {
                        if (fixedWidth ||
                            ((((DataGridViewAutoSizeColumnCriteriaInternal) dataGridViewColumn.InheritedAutoSizeMode) & (DataGridViewAutoSizeColumnCriteriaInternal.AllRows | DataGridViewAutoSizeColumnCriteriaInternal.DisplayedRows)) == 0))
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
            return (this.GetState(rowIndex) & DataGridViewElementStates.ReadOnly) != 0 ||
                   (this.DataGridView != null && this.DataGridView.ReadOnly);
        }

        internal DataGridViewTriState GetResizable(int rowIndex)
        {
            if ((GetState(rowIndex) & DataGridViewElementStates.ResizableSet) != 0)
            {
                return ((GetState(rowIndex) & DataGridViewElementStates.Resizable) != 0) ? DataGridViewTriState.True : DataGridViewTriState.False;
            }
            if (this.DataGridView != null)
            {
                return this.DataGridView.AllowUserToResizeRows ? DataGridViewTriState.True : DataGridViewTriState.False;
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

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.GetState"]/*' />
        [
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public virtual DataGridViewElementStates GetState(int rowIndex)
        {
            if (!(this.DataGridView == null || (rowIndex >= 0 && rowIndex < this.DataGridView.Rows.Count)))
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            if (this.DataGridView == null || this.DataGridView.Rows.SharedRow(rowIndex).Index != -1)
            {
                if (rowIndex != this.Index)
                {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, "rowIndex", rowIndex.ToString(CultureInfo.CurrentCulture)));
                }
                return base.State;
            }
            else 
            {
                return this.DataGridView.Rows.GetRowState(rowIndex);
            }
        }

        internal bool GetVisible(int rowIndex)
        {
            return (GetState(rowIndex) & DataGridViewElementStates.Visible) != 0;
        }

        internal void OnSharedStateChanged(int sharedRowIndex, DataGridViewElementStates elementState)
        {
            Debug.Assert(this.DataGridView != null);
            this.DataGridView.Rows.InvalidateCachedRowCount(elementState);
            this.DataGridView.Rows.InvalidateCachedRowsHeight(elementState);
            this.DataGridView.OnDataGridViewElementStateChanged(this, sharedRowIndex, elementState);
        }

        internal void OnSharedStateChanging(int sharedRowIndex, DataGridViewElementStates elementState)
        {
            Debug.Assert(this.DataGridView != null);
            this.DataGridView.OnDataGridViewElementStateChanging(this, sharedRowIndex, elementState);
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.Paint"]/*' />
        protected internal virtual void Paint(Graphics graphics,
            Rectangle clipBounds,
            Rectangle rowBounds,
            int rowIndex,
            DataGridViewElementStates rowState,
            bool isFirstDisplayedRow,
            bool isLastVisibleRow)
        {
            if (this.DataGridView == null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_RowDoesNotYetBelongToDataGridView));
            }
            DataGridView dataGridView = this.DataGridView;
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

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.PaintCells2"]/*' />
        [
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        protected internal virtual void PaintCells(Graphics graphics,
            Rectangle clipBounds,
            Rectangle rowBounds,
            int rowIndex,
            DataGridViewElementStates rowState,
            bool isFirstDisplayedRow,
            bool isLastVisibleRow,
            DataGridViewPaintParts paintParts)
        {
            if (this.DataGridView == null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_RowDoesNotYetBelongToDataGridView));
            }
            if ((int) paintParts < (int) DataGridViewPaintParts.None || (int) paintParts > (int) DataGridViewPaintParts.All)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewPaintPartsCombination, "paintParts"));
            }

            DataGridView dataGridView = this.DataGridView;
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
                cell = this.Cells[dataGridViewColumn.Index];
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
                    if (this.Index != -1)
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
                        cell = this.Cells[dataGridViewColumn.Index];
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
                            if (this.Index != -1)
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

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.PaintHeader"]/*' />
        [
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        protected internal virtual void PaintHeader(Graphics graphics, 
            Rectangle clipBounds,
            Rectangle rowBounds,
            int rowIndex,
            DataGridViewElementStates rowState,
            bool isFirstDisplayedRow,
            bool isLastVisibleRow,
            DataGridViewPaintParts paintParts)
        {
            if (this.DataGridView == null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_RowDoesNotYetBelongToDataGridView));
            }
            
            // not using ClientUtils.IsValidEnum here because this is a flags enum.  
            // everything is valid between 0x0 and 0x7F.
            if ((int) paintParts < (int) DataGridViewPaintParts.None || (int) paintParts > (int) DataGridViewPaintParts.All)
            {
                throw new InvalidEnumArgumentException(nameof(paintParts), (int)paintParts, typeof(DataGridViewPaintParts));
            }
            DataGridView dataGridView = this.DataGridView;
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
                    this.HeaderCell.PaintWork(graphics,
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
            Debug.Assert(this.Index == -1);
            if (this.ReadOnly && !readOnly)
            {
                // All cells need to switch to ReadOnly except for dataGridViewCell which needs to be !ReadOnly,
                // plus the row become !ReadOnly.
                foreach (DataGridViewCell dataGridViewCellTmp in this.Cells)
                {
                    dataGridViewCellTmp.ReadOnlyInternal = true;
                }
                dataGridViewCell.ReadOnlyInternal = false;
                this.ReadOnly = false;
            }
            else if (!this.ReadOnly && readOnly)
            {
                // dataGridViewCell alone becomes ReadOnly
                dataGridViewCell.ReadOnlyInternal = true;
            }
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.SetValues"]/*' />
        public bool SetValues(params object[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (this.DataGridView != null)
            {
                if (this.DataGridView.VirtualMode)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidOperationInVirtualMode));
                }
                if (this.Index == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidOperationOnSharedRow));
                }
            }

            return SetValuesInternal(values);
        }

        internal bool SetValuesInternal(params object[] values)
        {
            Debug.Assert(values != null);
            bool setResult = true;
            DataGridViewCellCollection cells = this.Cells;
            int cellCount = cells.Count;
            for (int columnIndex=0; columnIndex < cells.Count; columnIndex++)
            {
                if (columnIndex == values.Length)
                {
                    break;
                }
                if (!cells[columnIndex].SetValueInternal(this.Index, values[columnIndex]))
                {
                    setResult = false;
                }
            }
            return setResult && values.Length <= cellCount;
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRow.ToString"]/*' />
        /// <devdoc>
        ///    <para></para>
        /// </devdoc>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(36);
            sb.Append("DataGridViewRow { Index=");
            sb.Append(this.Index.ToString(CultureInfo.CurrentCulture));
            sb.Append(" }");
            return sb.ToString();
        }

        /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRowAccessibleObject"]/*' />
        [
            System.Runtime.InteropServices.ComVisible(true)
        ]
        protected class DataGridViewRowAccessibleObject : AccessibleObject
        {
            private int[] runtimeId; 
            private DataGridViewRow owner;
            private DataGridViewSelectedRowCellsAccessibleObject selectedCellsAccessibilityObject = null;

            /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRowAccessibleObject.DataGridViewRowAccessibleObject1"]/*' />
            public DataGridViewRowAccessibleObject()
            {
            }

            /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRowAccessibleObject.DataGridViewRowAccessibleObject2"]/*' />
            public DataGridViewRowAccessibleObject(DataGridViewRow owner)
            {
                this.owner = owner;
            }

            /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRowAccessibleObject.Bounds"]/*' />
            public override Rectangle Bounds
            {
                get {
                    Rectangle rowRect;
                    if (this.owner == null)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewRowAccessibleObject_OwnerNotSet));
                    }
                    if (this.owner.Index < this.owner.DataGridView.FirstDisplayedScrollingRowIndex)
                    {
                        // the row is scrolled off the DataGridView
                        // get the Accessible bounds for the following visible row
                        int visibleRowIndex = this.owner.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible, 0, this.owner.Index);
                        rowRect = this.ParentPrivate.GetChild(visibleRowIndex
                                                              + 1                      // + 1 because the first acc obj in the DataGridView is the top row header
                                                              + 1).Bounds;             // + 1 because we want to get the bounds for the next visible row

                        rowRect.Y -= this.owner.Height;
                        rowRect.Height = this.owner.Height;

                    }
                    else if (this.owner.Index >= this.owner.DataGridView.FirstDisplayedScrollingRowIndex &&
                        this.owner.Index < this.owner.DataGridView.FirstDisplayedScrollingRowIndex + this.owner.DataGridView.DisplayedRowCount(true /*includePartialRow*/))
                    {
                        rowRect = this.owner.DataGridView.GetRowDisplayRectangle(this.owner.Index, false /*cutOverflow*/);
                        rowRect = this.owner.DataGridView.RectangleToScreen(rowRect);
                    }
                    else
                    {
                        // the row is scrolled off the DataGridView
                        // use the Accessible bounds for the previous visible row
                        int visibleRowIndex = this.owner.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible, 0, this.owner.Index);

                        // This is a tricky scenario
                        // If Visible of Row 0 is false, then visibleRowIndex is not the previous visible row.
                        // It turns out to be the current row, this will cause a stack overflow.
                        // We have to prevent this.
                        if (this.owner.DataGridView.Rows[0].Visible == false)
                        {
                            visibleRowIndex--;
                        }

                        // we don't have to decrement the visible row index if the first acc obj in the data grid view is the top column header 
                        if (!this.owner.DataGridView.ColumnHeadersVisible)
                        {
                            visibleRowIndex--;
                        }

                        rowRect = this.ParentPrivate.GetChild(visibleRowIndex).Bounds;
                        rowRect.Y += rowRect.Height;
                        rowRect.Height = this.owner.Height;
                    }

                    return rowRect;
                }
            }

            /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRowAccessibleObject.Name"]/*' />
            public override string Name
            {
                get
                {
                    if (this.owner == null)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewRowAccessibleObject_OwnerNotSet));
                    }
                    return string.Format(SR.DataGridView_AccRowName, this.owner.Index.ToString(CultureInfo.CurrentCulture));
                }
            }

            /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRowAccessibleObject.Owner"]/*' />
            public DataGridViewRow Owner
            {
                get
                {
                    return this.owner;
                }
                set
                {
                    if (this.owner != null)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewRowAccessibleObject_OwnerAlreadySet));
                    }
                    this.owner = value;
                }
            }

            /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRowAccessibleObject.Parent"]/*' />
            public override AccessibleObject Parent
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    return this.ParentPrivate;
                }
            }

            private AccessibleObject ParentPrivate
            {
                get
                {
                    if (this.owner == null)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewRowAccessibleObject_OwnerNotSet));
                    }
                    return this.owner.DataGridView.AccessibilityObject;
                }
            }

            /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRowAccessibleObject.Role"]/*' />
            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.Row;
                }
            }

            internal override int[] RuntimeId
            {
                get
                {
                    if (AccessibilityImprovements.Level3 && runtimeId == null)
                    {
                        runtimeId = new int[3];
                        runtimeId[0] = RuntimeIDFirstItem; // first item is static - 0x2a
                        runtimeId[1] = this.Parent.GetHashCode();
                        runtimeId[2] = this.GetHashCode();
                    }

                    return runtimeId;
                }
            }

            private AccessibleObject SelectedCellsAccessibilityObject
            {
                get
                {
                    if (this.owner == null)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewRowAccessibleObject_OwnerNotSet));
                    }
                    if (this.selectedCellsAccessibilityObject == null)
                    {
                        this.selectedCellsAccessibilityObject = new DataGridViewSelectedRowCellsAccessibleObject(this.owner);
                    }
                    return this.selectedCellsAccessibilityObject;
                }
            }

            /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRowAccessibleObject.State"]/*' />
            public override AccessibleStates State
            {
                get
                {
                    if (this.owner == null)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewRowAccessibleObject_OwnerNotSet));
                    }

                    AccessibleStates accState = AccessibleStates.Selectable;

                    bool allCellsAreSelected = true;
                    if (this.owner.Selected)
                    {
                        allCellsAreSelected = true;
                    }
                    else
                    {
                        for (int i = 0; i < this.owner.Cells.Count; i ++)
                        {
                            if (!this.owner.Cells[i].Selected)
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

                    Rectangle rowBounds = this.owner.DataGridView.GetRowDisplayRectangle(this.owner.Index, true /*cutOverflow*/);
                    if (!rowBounds.IntersectsWith(this.owner.DataGridView.ClientRectangle))
                    {
                        accState |= AccessibleStates.Offscreen;
                    }

                    return accState;
                }
            }

            /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRowAccessibleObject.Value"]/*' />
            public override string Value
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    if (this.owner == null)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewRowAccessibleObject_OwnerNotSet));
                    }
                    if (this.owner.DataGridView.AllowUserToAddRows && this.owner.Index == this.owner.DataGridView.NewRowIndex)
                    {
                        return string.Format(SR.DataGridView_AccRowCreateNew);
                    }

                    StringBuilder sb = new StringBuilder(1024);

                    int childCount = this.GetChildCount();

                    // filter out the row header acc object even when DataGridView::RowHeadersVisible is turned on
                    int startIndex = this.owner.DataGridView.RowHeadersVisible ? 1 : 0;

                    for (int i = startIndex; i < childCount; i++)
                    {
                        AccessibleObject cellAccObj = this.GetChild(i);
                        if (cellAccObj != null)
                        {
                            sb.Append(cellAccObj.Value);
                        }

                        if (i != childCount - 1)
                        {
                            sb.Append(";");
                        }
                    }

                    return sb.ToString();
                }
            }

            /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRowAccessibleObject.GetChild"]/*' />
            public override AccessibleObject GetChild(int index)
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                if (this.owner == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewRowAccessibleObject_OwnerNotSet));
                }
                if (index == 0 && this.owner.DataGridView.RowHeadersVisible)
                {
                    return this.owner.HeaderCell.AccessibilityObject;
                }
                else
                {
                    // decrement the index because the first child is the RowHeaderCell AccessibilityObject
                    if (this.owner.DataGridView.RowHeadersVisible)
                    {
                        index --;
                    }
                    Debug.Assert(index >= 0);
                    int columnIndex = this.owner.DataGridView.Columns.ActualDisplayIndexToColumnIndex(index, DataGridViewElementStates.Visible);
                    return this.owner.Cells[columnIndex].AccessibilityObject;
                }
            }

            /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRowAccessibleObject.GetChildCount"]/*' />
            public override int GetChildCount()
            {
                if (this.owner == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewRowAccessibleObject_OwnerNotSet));
                }
                int result = this.owner.DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible);

                if (this.owner.DataGridView.RowHeadersVisible)
                {
                    // + 1 comes from the row header cell accessibility object
                    result ++;
                }

                return result;
            }

            /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRowAccessibleObject.GetSelected"]/*' />
            public override AccessibleObject GetSelected()
            {
                return this.SelectedCellsAccessibilityObject;
            }

            /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRowAccessibleObject.GetFocused"]/*' />
            public override AccessibleObject GetFocused()
            {
                if (this.owner == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewRowAccessibleObject_OwnerNotSet));
                }
                if (this.owner.DataGridView.Focused && this.owner.DataGridView.CurrentCell != null && this.owner.DataGridView.CurrentCell.RowIndex == this.owner.Index)
                {
                    return this.owner.DataGridView.CurrentCell.AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRowAccessibleObject.Navigate"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                if (this.owner == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewRowAccessibleObject_OwnerNotSet));
                }
                switch (navigationDirection)
                {
                    case AccessibleNavigation.Down:
                    case AccessibleNavigation.Next:
                        if (this.owner.Index != this.owner.DataGridView.Rows.GetLastRow(DataGridViewElementStates.Visible))
                        {
                            int nextVisibleRow = this.owner.DataGridView.Rows.GetNextRow(this.owner.Index, DataGridViewElementStates.Visible);
                            int actualDisplayIndex = this.owner.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible, 0, nextVisibleRow);
                            if (this.owner.DataGridView.ColumnHeadersVisible)
                            {
                                return this.owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex + 1);
                            }
                            else
                            {
                                return this.owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex);
                            }
                        }
                        else
                        {
                            return null;
                        }
                    case AccessibleNavigation.Up:
                    case AccessibleNavigation.Previous:
                        if (this.owner.Index != this.owner.DataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible))
                        {
                            int previousVisibleRow = this.owner.DataGridView.Rows.GetPreviousRow(this.owner.Index, DataGridViewElementStates.Visible);
                            int actualDisplayIndex = this.owner.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible, 0, previousVisibleRow);
                            if (this.owner.DataGridView.ColumnHeadersVisible)
                            {
                                return this.owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex + 1);
                            }
                            else
                            {
                                return this.owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex);
                            }
                        }
                        else if (this.owner.DataGridView.ColumnHeadersVisible)
                        {
                            // return the top row header acc obj
                            return this.ParentPrivate.GetChild(0);
                        }
                        else
                        {
                            // if this is the first row and the DataGridView RowHeaders are not visible return null;
                            return null;
                        }
                    case AccessibleNavigation.FirstChild:
                        if (this.GetChildCount() == 0)
                        {
                            return null;
                        }
                        else
                        {
                            return this.GetChild(0);
                        }
                    case AccessibleNavigation.LastChild:
                        int childCount = this.GetChildCount();
                        if (childCount == 0)
                        {
                            return null;
                        }
                        else
                        {
                            return this.GetChild(childCount - 1);
                        }
                    default:
                        return null;
                }
            }
            
            /// <include file='doc\DataGridViewRow.uex' path='docs/doc[@for="DataGridViewRowAccessibleObject.Select"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void Select(AccessibleSelection flags)
            {
                if (this.owner == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewRowAccessibleObject_OwnerNotSet));
                }

                DataGridView dataGridView = this.owner.DataGridView;
                if (dataGridView == null)
                {
                    return;
                }
                if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
                {
                    dataGridView.FocusInternal();
                }
                if ((flags & AccessibleSelection.TakeSelection) == AccessibleSelection.TakeSelection)
                {
                    if (this.owner.Cells.Count > 0)
                    {
                        if (dataGridView.CurrentCell != null && dataGridView.CurrentCell.OwningColumn != null)
                        {
                            dataGridView.CurrentCell = this.owner.Cells[dataGridView.CurrentCell.OwningColumn.Index]; // Do not change old selection
                        }
                        else
                        {
                            int firstVisibleCell = dataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible).Index;
                            if (firstVisibleCell > -1)
                            {
                                dataGridView.CurrentCell = this.owner.Cells[firstVisibleCell]; // Do not change old selection
                            }
                        }
                    }
                }

                if ((flags & AccessibleSelection.AddSelection) == AccessibleSelection.AddSelection && (flags & AccessibleSelection.TakeSelection) == 0)
                {
                    if (dataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect || dataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
                    {
                        this.owner.Selected = true;
                    }
                }

                if ((flags & AccessibleSelection.RemoveSelection) == AccessibleSelection.RemoveSelection &&
                    (flags & (AccessibleSelection.AddSelection | AccessibleSelection.TakeSelection)) == 0)
                {
                    this.owner.Selected = false;
                }
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
            {
                {
                    if (Owner == null)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewRowAccessibleObject_OwnerNotSet));
                    }

                    var dataGridView = this.Owner.DataGridView;

                    switch (direction)
                    {
                        case UnsafeNativeMethods.NavigateDirection.Parent:
                            return this.Parent;
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
                    return this.Owner.DataGridView.AccessibilityObject;
                }
            }

            #region IRawElementProviderSimple Implementation

            internal override bool IsPatternSupported(int patternId)
            {
                return patternId.Equals(NativeMethods.UIA_LegacyIAccessiblePatternId);
            }

            internal override object GetPropertyValue(int propertyId)
            {
                if (AccessibilityImprovements.Level3)
                {
                    switch (propertyId)
                    {
                        case NativeMethods.UIA_NamePropertyId:
                            return this.Name;
                        case NativeMethods.UIA_IsEnabledPropertyId:
                            return Owner.DataGridView.Enabled;
                        case NativeMethods.UIA_HelpTextPropertyId:
                            return this.Help ?? string.Empty;
                        case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                        case NativeMethods.UIA_HasKeyboardFocusPropertyId:
                        case NativeMethods.UIA_IsPasswordPropertyId:
                        case NativeMethods.UIA_IsOffscreenPropertyId:
                            return false;
                        case NativeMethods.UIA_AccessKeyPropertyId:
                            return string.Empty;
                    }
                }

                return base.GetPropertyValue(propertyId);
            }

            #endregion
        }

        private class DataGridViewSelectedRowCellsAccessibleObject : AccessibleObject
        {
            DataGridViewRow owner;

            internal DataGridViewSelectedRowCellsAccessibleObject(DataGridViewRow owner)
            {
                this.owner = owner;
            }

            public override string Name
            {
                get
                {
                    return string.Format(SR.DataGridView_AccSelectedRowCellsName);
                }
            }

            public override AccessibleObject Parent
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    return this.owner.AccessibilityObject;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.Grouping;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    return AccessibleStates.Selected | AccessibleStates.Selectable;
                }
            }

            public override string Value
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    return this.Name;
                }
            }

            public override AccessibleObject GetChild(int index)
            {
                if (index < this.GetChildCount())
                {
                    int selectedCellsCount = -1;
                    for (int i = 1; i < this.owner.AccessibilityObject.GetChildCount(); i ++)
                    {
                        if ((this.owner.AccessibilityObject.GetChild(i).State & AccessibleStates.Selected) == AccessibleStates.Selected)
                        {
                            selectedCellsCount ++;
                        }

                        if (selectedCellsCount == index)
                        {
                            return this.owner.AccessibilityObject.GetChild(i);
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
                for (int i = 1; i < this.owner.AccessibilityObject.GetChildCount(); i ++)
                {
                    if ((this.owner.AccessibilityObject.GetChild(i).State & AccessibleStates.Selected) == AccessibleStates.Selected)
                    {
                        selectedCellsCount ++;
                    }
                }

                return selectedCellsCount;
            }

            public override AccessibleObject GetSelected()
            {
                return this;
            }

            public override AccessibleObject GetFocused()
            {
                if (this.owner.DataGridView.CurrentCell != null && this.owner.DataGridView.CurrentCell.Selected)
                {
                    return this.owner.DataGridView.CurrentCell.AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
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

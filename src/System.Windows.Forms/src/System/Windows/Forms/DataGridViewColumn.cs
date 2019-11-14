// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Base class for the columns in a data grid view.
    /// </summary>
    [
        Designer("System.Windows.Forms.Design.DataGridViewColumnDesigner, " + AssemblyRef.SystemDesign),
        TypeConverter(typeof(DataGridViewColumnConverter)),
        ToolboxItem(false),
        DesignTimeVisible(false)
    ]
    public class DataGridViewColumn : DataGridViewBand, IComponent
    {
        private const float DATAGRIDVIEWCOLUMN_defaultFillWeight = 100F;
        private const int DATAGRIDVIEWCOLUMN_defaultWidth = 100;
        private const int DATAGRIDVIEWCOLUMN_defaultMinColumnThickness = 5;

        private const byte DATAGRIDVIEWCOLUMN_automaticSort = 0x01;
        private const byte DATAGRIDVIEWCOLUMN_programmaticSort = 0x02;
        private const byte DATAGRIDVIEWCOLUMN_isDataBound = 0x04;
        private const byte DATAGRIDVIEWCOLUMN_isBrowsableInternal = 0x08;
        private const byte DATAGRIDVIEWCOLUMN_displayIndexHasChangedInternal = 0x10;

        private byte flags;  // see DATAGRIDVIEWCOLUMN_ consts above
        private string name;
        private int displayIndex;
        private float fillWeight, usedFillWeight;
        private DataGridViewAutoSizeColumnMode autoSizeMode;
        private string dataPropertyName = string.Empty;

        // needed for IComponent
        private EventHandler disposed = null;

        private static readonly int PropDataGridViewColumnValueType = PropertyStore.CreateKey();

        /// <summary>
        ///  Initializes a new instance of the <see cref='DataGridViewColumn'/> class.
        /// </summary>
        public DataGridViewColumn() : this((DataGridViewCell)null)
        {
        }

        public DataGridViewColumn(DataGridViewCell cellTemplate) : base()
        {
            fillWeight = DATAGRIDVIEWCOLUMN_defaultFillWeight;
            usedFillWeight = DATAGRIDVIEWCOLUMN_defaultFillWeight;
            Thickness = ScaleToCurrentDpi(DATAGRIDVIEWCOLUMN_defaultWidth);
            MinimumThickness = ScaleToCurrentDpi(DATAGRIDVIEWCOLUMN_defaultMinColumnThickness);
            name = string.Empty;
            displayIndex = -1;
            CellTemplate = cellTemplate;
            autoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
        }

        /// <summary>
        ///  Scale to current device dpi settings
        /// </summary>
        /// <param name="value"> initial value</param>
        /// <returns> scaled metric</returns>
        private int ScaleToCurrentDpi(int value)
        {
            return DpiHelper.IsScalingRequirementMet ? DpiHelper.LogicalToDeviceUnits(value) : value;
        }

        [
            SRCategory(nameof(SR.CatLayout)),
            DefaultValue(DataGridViewAutoSizeColumnMode.NotSet),
            SRDescription(nameof(SR.DataGridViewColumn_AutoSizeModeDescr)),
            RefreshProperties(RefreshProperties.Repaint)
        ]
        public DataGridViewAutoSizeColumnMode AutoSizeMode
        {
            get
            {
                return autoSizeMode;
            }
            set
            {
                switch (value)
                {
                    case DataGridViewAutoSizeColumnMode.NotSet:
                    case DataGridViewAutoSizeColumnMode.None:
                    case DataGridViewAutoSizeColumnMode.ColumnHeader:
                    case DataGridViewAutoSizeColumnMode.AllCellsExceptHeader:
                    case DataGridViewAutoSizeColumnMode.AllCells:
                    case DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader:
                    case DataGridViewAutoSizeColumnMode.DisplayedCells:
                    case DataGridViewAutoSizeColumnMode.Fill:
                        break;
                    default:
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewAutoSizeColumnMode));
                }
                if (autoSizeMode != value)
                {
                    if (Visible && DataGridView != null)
                    {
                        if (!DataGridView.ColumnHeadersVisible &&
                            (value == DataGridViewAutoSizeColumnMode.ColumnHeader ||
                             (value == DataGridViewAutoSizeColumnMode.NotSet && DataGridView.AutoSizeColumnsMode == DataGridViewAutoSizeColumnsMode.ColumnHeader)))
                        {
                            throw new InvalidOperationException(SR.DataGridViewColumn_AutoSizeCriteriaCannotUseInvisibleHeaders);
                        }
                        if (Frozen &&
                            (value == DataGridViewAutoSizeColumnMode.Fill ||
                             (value == DataGridViewAutoSizeColumnMode.NotSet && DataGridView.AutoSizeColumnsMode == DataGridViewAutoSizeColumnsMode.Fill)))
                        {
                            // Cannot set the inherited auto size mode to Fill when the column is frozen
                            throw new InvalidOperationException(SR.DataGridViewColumn_FrozenColumnCannotAutoFill);
                        }
                    }
                    DataGridViewAutoSizeColumnMode previousInheritedMode = InheritedAutoSizeMode;
                    bool previousInheritedModeAutoSized = previousInheritedMode != DataGridViewAutoSizeColumnMode.Fill &&
                                                          previousInheritedMode != DataGridViewAutoSizeColumnMode.None &&
                                                          previousInheritedMode != DataGridViewAutoSizeColumnMode.NotSet;
                    autoSizeMode = value;
                    if (DataGridView == null)
                    {
                        if (InheritedAutoSizeMode != DataGridViewAutoSizeColumnMode.Fill &&
                            InheritedAutoSizeMode != DataGridViewAutoSizeColumnMode.None &&
                            InheritedAutoSizeMode != DataGridViewAutoSizeColumnMode.NotSet)
                        {
                            if (!previousInheritedModeAutoSized)
                            {
                                // Save current column width for later reuse
                                CachedThickness = Thickness;
                            }
                        }
                        else
                        {
                            if (Thickness != CachedThickness && previousInheritedModeAutoSized)
                            {
                                // Restoring cached column width
                                ThicknessInternal = CachedThickness;
                            }
                        }
                    }
                    else
                    {
                        DataGridView.OnAutoSizeColumnModeChanged(this, previousInheritedMode);
                    }
                }
            }
        }

        // TypeConverter of the PropertyDescriptor attached to this column
        // in databound cases. Null otherwise.
        internal TypeConverter BoundColumnConverter { get; set; }

        internal int BoundColumnIndex { get; set; } = -1;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual DataGridViewCell CellTemplate { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Type CellType => CellTemplate?.GetType();

        [
            DefaultValue(null),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_ColumnContextMenuStripDescr))
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

        [
            Browsable(true),
            DefaultValue(""),
            TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, " + AssemblyRef.SystemDesign),
            Editor("System.Windows.Forms.Design.DataGridViewColumnDataPropertyNameEditor, " + AssemblyRef.SystemDesign, typeof(Drawing.Design.UITypeEditor)),
            SRDescription(nameof(SR.DataGridView_ColumnDataPropertyNameDescr)),
            SRCategory(nameof(SR.CatData))
        ]
        public string DataPropertyName
        {
            get
            {
                return dataPropertyName;
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                if (value != dataPropertyName)
                {
                    dataPropertyName = value;
                    if (DataGridView != null)
                    {
                        DataGridView.OnColumnDataPropertyNameChanged(this);
                    }
                }
            }
        }

        [
            Browsable(true),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ColumnDefaultCellStyleDescr))
        ]
        public override DataGridViewCellStyle DefaultCellStyle
        {
            get
            {
                return base.DefaultCellStyle;
            }
            set
            {
                base.DefaultCellStyle = value;
            }
        }

        private bool ShouldSerializeDefaultCellStyle()
        {
            if (!HasDefaultCellStyle)
            {
                return false;
            }

            DataGridViewCellStyle defaultCellStyle = DefaultCellStyle;

            return (!defaultCellStyle.BackColor.IsEmpty ||
                    !defaultCellStyle.ForeColor.IsEmpty ||
                    !defaultCellStyle.SelectionBackColor.IsEmpty ||
                    !defaultCellStyle.SelectionForeColor.IsEmpty ||
                    defaultCellStyle.Font != null ||
                    !defaultCellStyle.IsNullValueDefault ||
                    !defaultCellStyle.IsDataSourceNullValueDefault ||
                    !string.IsNullOrEmpty(defaultCellStyle.Format) ||
                    !defaultCellStyle.FormatProvider.Equals(System.Globalization.CultureInfo.CurrentCulture) ||
                    defaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet ||
                    defaultCellStyle.WrapMode != DataGridViewTriState.NotSet ||
                    defaultCellStyle.Tag != null ||
                    !defaultCellStyle.Padding.Equals(Padding.Empty));
        }

        internal int DesiredFillWidth { get; set; }

        internal int DesiredMinimumWidth { get; set; }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int DisplayIndex
        {
            get
            {
                return displayIndex;
            }
            set
            {
                if (displayIndex != value)
                {
                    if (value == int.MaxValue)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.DataGridViewColumn_DisplayIndexTooLarge, int.MaxValue));
                    }
                    if (DataGridView != null)
                    {
                        if (value < 0)
                        {
                            throw new ArgumentOutOfRangeException(nameof(value), value, SR.DataGridViewColumn_DisplayIndexNegative);
                        }
                        if (value >= DataGridView.Columns.Count)
                        {
                            throw new ArgumentOutOfRangeException(nameof(value), value, SR.DataGridViewColumn_DisplayIndexExceedsColumnCount);
                        }

                        // Will throw an error if a visible frozen column is placed inside a non-frozen area or vice-versa.
                        DataGridView.OnColumnDisplayIndexChanging(this, value);
                        displayIndex = value;
                        try
                        {
                            DataGridView.InDisplayIndexAdjustments = true;
                            DataGridView.OnColumnDisplayIndexChanged_PreNotification();
                            DataGridView.OnColumnDisplayIndexChanged(this);
                            DataGridView.OnColumnDisplayIndexChanged_PostNotification();
                        }
                        finally
                        {
                            DataGridView.InDisplayIndexAdjustments = false;
                        }
                    }
                    else
                    {
                        if (value < -1)
                        {
                            throw new ArgumentOutOfRangeException(nameof(DisplayIndex), value, SR.DataGridViewColumn_DisplayIndexTooNegative);
                        }
                        displayIndex = value;
                    }
                }
            }
        }

        internal bool DisplayIndexHasChanged
        {
            get
            {
                return (flags & DATAGRIDVIEWCOLUMN_displayIndexHasChangedInternal) != 0;
            }
            set
            {
                if (value)
                {
                    flags |= (byte)DATAGRIDVIEWCOLUMN_displayIndexHasChangedInternal;
                }
                else
                {
                    flags = (byte)(flags & ~DATAGRIDVIEWCOLUMN_displayIndexHasChangedInternal);
                }
            }
        }

        internal int DisplayIndexInternal
        {
            set
            {
                Debug.Assert(value >= -1);
                Debug.Assert(value < int.MaxValue);

                displayIndex = value;
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public event EventHandler Disposed
        {
            add => disposed += value;
            remove => disposed -= value;
        }

        [
            DefaultValue(0),
            SRCategory(nameof(SR.CatLayout)),
            SRDescription(nameof(SR.DataGridView_ColumnDividerWidthDescr))
        ]
        public int DividerWidth
        {
            get
            {
                return DividerThickness;
            }
            set
            {
                DividerThickness = value;
            }
        }

        [
            SRCategory(nameof(SR.CatLayout)),
            DefaultValue(DATAGRIDVIEWCOLUMN_defaultFillWeight),
            SRDescription(nameof(SR.DataGridViewColumn_FillWeightDescr)),
        ]
        public float FillWeight
        {
            get
            {
                return fillWeight;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgument, nameof(FillWeight), value, 0));
                }
                if (value > (float)ushort.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidHighBoundArgumentEx, nameof(FillWeight), value, ushort.MaxValue));
                }
                if (DataGridView != null)
                {
                    DataGridView.OnColumnFillWeightChanging(this, value);
                    fillWeight = value;
                    DataGridView.OnColumnFillWeightChanged(this);
                }
                else
                {
                    fillWeight = value;
                }
            }
        }

        internal float FillWeightInternal
        {
            set
            {
                Debug.Assert(value > 0);
                fillWeight = value;
            }
        }

        [
            DefaultValue(false),
            RefreshProperties(RefreshProperties.All),
            SRCategory(nameof(SR.CatLayout)),
            SRDescription(nameof(SR.DataGridView_ColumnFrozenDescr))
        ]
        public override bool Frozen
        {
            get
            {
                return base.Frozen;
            }
            set
            {
                base.Frozen = value;
            }
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public DataGridViewColumnHeaderCell HeaderCell
        {
            get
            {
                return (DataGridViewColumnHeaderCell)base.HeaderCellCore;
            }
            set
            {
                base.HeaderCellCore = value;
            }
        }

        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ColumnHeaderTextDescr)),
            Localizable(true)
        ]
        public string HeaderText
        {
            get
            {
                if (HasHeaderCell)
                {
                    if (HeaderCell.Value is string headerValue)
                    {
                        return headerValue;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if ((value != null || HasHeaderCell) &&
                    HeaderCell.ValueType != null &&
                    HeaderCell.ValueType.IsAssignableFrom(typeof(string)))
                {
                    HeaderCell.Value = value;
                }
            }
        }

        private bool ShouldSerializeHeaderText()
        {
            return HasHeaderCell && ((DataGridViewColumnHeaderCell)HeaderCell).ContainsLocalValue;
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public DataGridViewAutoSizeColumnMode InheritedAutoSizeMode
        {
            get
            {
                return GetInheritedAutoSizeMode(DataGridView);
            }
        }

        [
            Browsable(false)
        ]
        public override DataGridViewCellStyle InheritedStyle
        {
            get
            {
                DataGridViewCellStyle columnStyle = null;
                Debug.Assert(Index > -1);
                if (HasDefaultCellStyle)
                {
                    columnStyle = DefaultCellStyle;
                    Debug.Assert(columnStyle != null);
                }

                if (DataGridView == null)
                {
                    return columnStyle;
                }

                DataGridViewCellStyle inheritedCellStyleTmp = new DataGridViewCellStyle();
                DataGridViewCellStyle dataGridViewStyle = DataGridView.DefaultCellStyle;
                Debug.Assert(dataGridViewStyle != null);

                if (columnStyle != null && !columnStyle.BackColor.IsEmpty)
                {
                    inheritedCellStyleTmp.BackColor = columnStyle.BackColor;
                }
                else
                {
                    inheritedCellStyleTmp.BackColor = dataGridViewStyle.BackColor;
                }

                if (columnStyle != null && !columnStyle.ForeColor.IsEmpty)
                {
                    inheritedCellStyleTmp.ForeColor = columnStyle.ForeColor;
                }
                else
                {
                    inheritedCellStyleTmp.ForeColor = dataGridViewStyle.ForeColor;
                }

                if (columnStyle != null && !columnStyle.SelectionBackColor.IsEmpty)
                {
                    inheritedCellStyleTmp.SelectionBackColor = columnStyle.SelectionBackColor;
                }
                else
                {
                    inheritedCellStyleTmp.SelectionBackColor = dataGridViewStyle.SelectionBackColor;
                }

                if (columnStyle != null && !columnStyle.SelectionForeColor.IsEmpty)
                {
                    inheritedCellStyleTmp.SelectionForeColor = columnStyle.SelectionForeColor;
                }
                else
                {
                    inheritedCellStyleTmp.SelectionForeColor = dataGridViewStyle.SelectionForeColor;
                }

                if (columnStyle != null && columnStyle.Font != null)
                {
                    inheritedCellStyleTmp.Font = columnStyle.Font;
                }
                else
                {
                    inheritedCellStyleTmp.Font = dataGridViewStyle.Font;
                }

                if (columnStyle != null && !columnStyle.IsNullValueDefault)
                {
                    inheritedCellStyleTmp.NullValue = columnStyle.NullValue;
                }
                else
                {
                    inheritedCellStyleTmp.NullValue = dataGridViewStyle.NullValue;
                }

                if (columnStyle != null && !columnStyle.IsDataSourceNullValueDefault)
                {
                    inheritedCellStyleTmp.DataSourceNullValue = columnStyle.DataSourceNullValue;
                }
                else
                {
                    inheritedCellStyleTmp.DataSourceNullValue = dataGridViewStyle.DataSourceNullValue;
                }

                if (columnStyle != null && columnStyle.Format.Length != 0)
                {
                    inheritedCellStyleTmp.Format = columnStyle.Format;
                }
                else
                {
                    inheritedCellStyleTmp.Format = dataGridViewStyle.Format;
                }

                if (columnStyle != null && !columnStyle.IsFormatProviderDefault)
                {
                    inheritedCellStyleTmp.FormatProvider = columnStyle.FormatProvider;
                }
                else
                {
                    inheritedCellStyleTmp.FormatProvider = dataGridViewStyle.FormatProvider;
                }

                if (columnStyle != null && columnStyle.Alignment != DataGridViewContentAlignment.NotSet)
                {
                    inheritedCellStyleTmp.AlignmentInternal = columnStyle.Alignment;
                }
                else
                {
                    Debug.Assert(dataGridViewStyle.Alignment != DataGridViewContentAlignment.NotSet);
                    inheritedCellStyleTmp.AlignmentInternal = dataGridViewStyle.Alignment;
                }

                if (columnStyle != null && columnStyle.WrapMode != DataGridViewTriState.NotSet)
                {
                    inheritedCellStyleTmp.WrapModeInternal = columnStyle.WrapMode;
                }
                else
                {
                    Debug.Assert(dataGridViewStyle.WrapMode != DataGridViewTriState.NotSet);
                    inheritedCellStyleTmp.WrapModeInternal = dataGridViewStyle.WrapMode;
                }

                if (columnStyle != null && columnStyle.Tag != null)
                {
                    inheritedCellStyleTmp.Tag = columnStyle.Tag;
                }
                else
                {
                    inheritedCellStyleTmp.Tag = dataGridViewStyle.Tag;
                }

                if (columnStyle != null && columnStyle.Padding != Padding.Empty)
                {
                    inheritedCellStyleTmp.PaddingInternal = columnStyle.Padding;
                }
                else
                {
                    inheritedCellStyleTmp.PaddingInternal = dataGridViewStyle.Padding;
                }

                return inheritedCellStyleTmp;
            }
        }

        internal bool IsBrowsableInternal
        {
            get
            {
                return (flags & DATAGRIDVIEWCOLUMN_isBrowsableInternal) != 0;
            }
            set
            {
                if (value)
                {
                    flags |= (byte)DATAGRIDVIEWCOLUMN_isBrowsableInternal;
                }
                else
                {
                    flags = (byte)(flags & ~DATAGRIDVIEWCOLUMN_isBrowsableInternal);
                }
            }
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool IsDataBound
        {
            get
            {
                return IsDataBoundInternal;
            }
        }

        internal bool IsDataBoundInternal
        {
            get
            {
                return (flags & DATAGRIDVIEWCOLUMN_isDataBound) != 0;
            }
            set
            {
                if (value)
                {
                    flags |= (byte)DATAGRIDVIEWCOLUMN_isDataBound;
                }
                else
                {
                    flags = (byte)(flags & ~DATAGRIDVIEWCOLUMN_isDataBound);
                }
            }
        }

        [
            DefaultValue(DATAGRIDVIEWCOLUMN_defaultMinColumnThickness),
            Localizable(true),
            SRCategory(nameof(SR.CatLayout)),
            SRDescription(nameof(SR.DataGridView_ColumnMinimumWidthDescr)),
            RefreshProperties(RefreshProperties.Repaint)
        ]
        public int MinimumWidth
        {
            get
            {
                return MinimumThickness;
            }
            set
            {
                MinimumThickness = value;
            }
        }

        [
            Browsable(false)
        ]
        public string Name
        {
            get
            {
                //
                // Change needed to bring the design time and the runtime "Name" property together.
                // The ExtenderProvider adds a "Name" property of its own. It does this for all IComponents.
                // The "Name" property added by the ExtenderProvider interacts only w/ the Site property.
                // The Control class' Name property can be changed only thru the "Name" property provided by the
                // Extender Service.
                //
                // However, the user can change the DataGridView::Name property in the DataGridViewEditColumnDialog.
                // So while the Control can fall back to Site.Name if the user did not explicitly set Control::Name,
                // the DataGridViewColumn should always go first to the Site.Name to retrieve the name.
                //
                // NOTE: one side effect of bringing together the design time and the run time "Name" properties is that DataGridViewColumn::Name changes.
                // However, DataGridView does not fire ColumnNameChanged event.
                // We can't fix this because ISite does not provide Name change notification. So in effect
                // DataGridViewColumn does not know when its name changed.
                // I talked w/ MarkRi and he is perfectly fine w/ DataGridViewColumn::Name changing w/o ColumnNameChanged
                // being fired.
                //
                if (Site != null && !string.IsNullOrEmpty(Site.Name))
                {
                    name = Site.Name;
                }

                return name;
            }
            set
            {
                string oldName = name;
                if (string.IsNullOrEmpty(value))
                {
                    name = string.Empty;
                }
                else
                {
                    name = value;
                }

                if (DataGridView != null && !string.Equals(name, oldName, StringComparison.Ordinal))
                {
                    DataGridView.OnColumnNameChanged(this);
                }
            }
        }

        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_ColumnReadOnlyDescr))
        ]
        public override bool ReadOnly
        {
            get
            {
                return base.ReadOnly;
            }
            set
            {
                if (IsDataBound &&
                    DataGridView != null &&
                    DataGridView.DataConnection != null &&
                    BoundColumnIndex != -1 &&
                    DataGridView.DataConnection.DataFieldIsReadOnly(BoundColumnIndex) &&
                    !value)
                {
                    throw new InvalidOperationException(SR.DataGridView_ColumnBoundToAReadOnlyFieldMustRemainReadOnly);
                }
                base.ReadOnly = value;
            }
        }

        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_ColumnResizableDescr))
        ]
        public override DataGridViewTriState Resizable
        {
            get
            {
                return base.Resizable;
            }
            set
            {
                base.Resizable = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ISite Site { get; set; }

        [
            DefaultValue(DataGridViewColumnSortMode.NotSortable),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_ColumnSortModeDescr))
        ]
        public DataGridViewColumnSortMode SortMode
        {
            get
            {
                if ((flags & DATAGRIDVIEWCOLUMN_automaticSort) != 0x00)
                {
                    return DataGridViewColumnSortMode.Automatic;
                }
                else if ((flags & DATAGRIDVIEWCOLUMN_programmaticSort) != 0x00)
                {
                    return DataGridViewColumnSortMode.Programmatic;
                }
                else
                {
                    return DataGridViewColumnSortMode.NotSortable;
                }
            }
            set
            {
                if (value != SortMode)
                {
                    if (value != DataGridViewColumnSortMode.NotSortable)
                    {
                        if (DataGridView != null &&
                            !DataGridView.InInitialization &&
                            value == DataGridViewColumnSortMode.Automatic &&
                            (DataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect ||
                            DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect))
                        {
                            throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_SortModeAndSelectionModeClash, (value).ToString(), DataGridView.SelectionMode.ToString()));
                        }
                        if (value == DataGridViewColumnSortMode.Automatic)
                        {
                            flags = (byte)(flags & ~DATAGRIDVIEWCOLUMN_programmaticSort);
                            flags |= (byte)DATAGRIDVIEWCOLUMN_automaticSort;
                        }
                        else
                        {
                            flags = (byte)(flags & ~DATAGRIDVIEWCOLUMN_automaticSort);
                            flags |= (byte)DATAGRIDVIEWCOLUMN_programmaticSort;
                        }
                    }
                    else
                    {
                        flags = (byte)(flags & ~DATAGRIDVIEWCOLUMN_automaticSort);
                        flags = (byte)(flags & ~DATAGRIDVIEWCOLUMN_programmaticSort);
                    }
                    if (DataGridView != null)
                    {
                        DataGridView.OnColumnSortModeChanged(this);
                    }
                }
            }
        }

        [
            DefaultValue(""),
            Localizable(true),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ColumnToolTipTextDescr))
        ]
        public string ToolTipText
        {
            get
            {
                return HeaderCell.ToolTipText;
            }
            set
            {
                if (string.Compare(ToolTipText, value, false /*ignore case*/, CultureInfo.InvariantCulture) != 0)
                {
                    HeaderCell.ToolTipText = value;

                    if (DataGridView != null)
                    {
                        DataGridView.OnColumnToolTipTextChanged(this);
                    }
                }
            }
        }

        internal float UsedFillWeight
        {
            get
            {
                return usedFillWeight;
            }
            set
            {
                Debug.Assert(value > 0);
                usedFillWeight = value;
            }
        }

        [
            Browsable(false),
            DefaultValue(null),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public Type ValueType
        {
            get
            {
                return (Type)Properties.GetObject(PropDataGridViewColumnValueType);
            }
            set
            {
                // what should we do when we modify the ValueType in the dataGridView column???
                Properties.SetObject(PropDataGridViewColumnValueType, value);
            }
        }

        [
            DefaultValue(true),
            Localizable(true),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ColumnVisibleDescr))
        ]
        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
            }
        }

        [
            SRCategory(nameof(SR.CatLayout)),
            Localizable(true),
            SRDescription(nameof(SR.DataGridView_ColumnWidthDescr)),
            RefreshProperties(RefreshProperties.Repaint)
        ]
        public int Width
        {
            get
            {
                return Thickness;
            }
            set
            {
                Thickness = value;
            }
        }

        public override object Clone()
        {
            //

            DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)System.Activator.CreateInstance(GetType());
            if (dataGridViewColumn != null)
            {
                CloneInternal(dataGridViewColumn);
            }
            return dataGridViewColumn;
        }

        private protected void CloneInternal(DataGridViewColumn dataGridViewColumn)
        {
            base.CloneInternal(dataGridViewColumn);

            dataGridViewColumn.name = Name;
            dataGridViewColumn.displayIndex = -1;
            dataGridViewColumn.HeaderText = HeaderText;
            dataGridViewColumn.DataPropertyName = DataPropertyName;
            dataGridViewColumn.CellTemplate = (DataGridViewCell)CellTemplate?.Clone();

            if (HasHeaderCell)
            {
                dataGridViewColumn.HeaderCell = (DataGridViewColumnHeaderCell)HeaderCell.Clone();
            }

            dataGridViewColumn.AutoSizeMode = AutoSizeMode;
            dataGridViewColumn.SortMode = SortMode;
            dataGridViewColumn.FillWeightInternal = FillWeight;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    //
                    lock (this)
                    {
                        Site?.Container?.Remove(this);
                        disposed?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        internal DataGridViewAutoSizeColumnMode GetInheritedAutoSizeMode(DataGridView dataGridView)
        {
            if (dataGridView != null && autoSizeMode == DataGridViewAutoSizeColumnMode.NotSet)
            {
                switch (dataGridView.AutoSizeColumnsMode)
                {
                    case DataGridViewAutoSizeColumnsMode.AllCells:
                        return DataGridViewAutoSizeColumnMode.AllCells;

                    case DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader:
                        return DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

                    case DataGridViewAutoSizeColumnsMode.DisplayedCells:
                        return DataGridViewAutoSizeColumnMode.DisplayedCells;

                    case DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader:
                        return DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;

                    case DataGridViewAutoSizeColumnsMode.ColumnHeader:
                        return DataGridViewAutoSizeColumnMode.ColumnHeader;

                    case DataGridViewAutoSizeColumnsMode.Fill:
                        return DataGridViewAutoSizeColumnMode.Fill;

                    default: // None
                        return DataGridViewAutoSizeColumnMode.None;
                }
            }
            return autoSizeMode;
        }

        public virtual int GetPreferredWidth(DataGridViewAutoSizeColumnMode autoSizeColumnMode, bool fixedHeight)
        {
            if (autoSizeColumnMode == DataGridViewAutoSizeColumnMode.NotSet ||
                autoSizeColumnMode == DataGridViewAutoSizeColumnMode.None ||
                autoSizeColumnMode == DataGridViewAutoSizeColumnMode.Fill)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_NeedColumnAutoSizingCriteria, "autoSizeColumnMode"));
            }
            switch (autoSizeColumnMode)
            {
                case DataGridViewAutoSizeColumnMode.NotSet:
                case DataGridViewAutoSizeColumnMode.None:
                case DataGridViewAutoSizeColumnMode.ColumnHeader:
                case DataGridViewAutoSizeColumnMode.AllCellsExceptHeader:
                case DataGridViewAutoSizeColumnMode.AllCells:
                case DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader:
                case DataGridViewAutoSizeColumnMode.DisplayedCells:
                case DataGridViewAutoSizeColumnMode.Fill:
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(autoSizeColumnMode), (int)autoSizeColumnMode, typeof(DataGridViewAutoSizeColumnMode));
            }

            DataGridView dataGridView = DataGridView;

            Debug.Assert(dataGridView == null || Index > -1);

            if (dataGridView == null)
            {
                return -1;
            }

            DataGridViewAutoSizeColumnCriteriaInternal autoSizeColumnCriteriaInternal = (DataGridViewAutoSizeColumnCriteriaInternal)autoSizeColumnMode;
            Debug.Assert(autoSizeColumnCriteriaInternal == DataGridViewAutoSizeColumnCriteriaInternal.Header ||
                autoSizeColumnCriteriaInternal == DataGridViewAutoSizeColumnCriteriaInternal.AllRows ||
                autoSizeColumnCriteriaInternal == DataGridViewAutoSizeColumnCriteriaInternal.DisplayedRows ||
                autoSizeColumnCriteriaInternal == (DataGridViewAutoSizeColumnCriteriaInternal.Header | DataGridViewAutoSizeColumnCriteriaInternal.AllRows) ||
                autoSizeColumnCriteriaInternal == (DataGridViewAutoSizeColumnCriteriaInternal.Header | DataGridViewAutoSizeColumnCriteriaInternal.DisplayedRows));

            int preferredColumnThickness = 0, preferredCellThickness, rowIndex;
            DataGridViewRow dataGridViewRow;
            Debug.Assert(dataGridView.ColumnHeadersVisible || autoSizeColumnCriteriaInternal != DataGridViewAutoSizeColumnCriteriaInternal.Header);

            // take into account the preferred width of the header cell if displayed and cared about
            if (dataGridView.ColumnHeadersVisible &&
                (autoSizeColumnCriteriaInternal & DataGridViewAutoSizeColumnCriteriaInternal.Header) != 0)
            {
                if (fixedHeight)
                {
                    preferredCellThickness = HeaderCell.GetPreferredWidth(-1, dataGridView.ColumnHeadersHeight);
                }
                else
                {
                    preferredCellThickness = HeaderCell.GetPreferredSize(-1).Width;
                }
                if (preferredColumnThickness < preferredCellThickness)
                {
                    preferredColumnThickness = preferredCellThickness;
                }
            }
            if ((autoSizeColumnCriteriaInternal & DataGridViewAutoSizeColumnCriteriaInternal.AllRows) != 0)
            {
                for (rowIndex = dataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible);
                    rowIndex != -1;
                    rowIndex = dataGridView.Rows.GetNextRow(rowIndex, DataGridViewElementStates.Visible))
                {
                    dataGridViewRow = dataGridView.Rows.SharedRow(rowIndex);
                    if (fixedHeight)
                    {
                        preferredCellThickness = dataGridViewRow.Cells[Index].GetPreferredWidth(rowIndex, dataGridViewRow.Thickness);
                    }
                    else
                    {
                        preferredCellThickness = dataGridViewRow.Cells[Index].GetPreferredSize(rowIndex).Width;
                    }
                    if (preferredColumnThickness < preferredCellThickness)
                    {
                        preferredColumnThickness = preferredCellThickness;
                    }
                }
            }
            else if ((autoSizeColumnCriteriaInternal & DataGridViewAutoSizeColumnCriteriaInternal.DisplayedRows) != 0)
            {
                int displayHeight = dataGridView.LayoutInfo.Data.Height;
                int cy = 0;

                rowIndex = dataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);
                while (rowIndex != -1 && cy < displayHeight)
                {
                    dataGridViewRow = dataGridView.Rows.SharedRow(rowIndex);
                    if (fixedHeight)
                    {
                        preferredCellThickness = dataGridViewRow.Cells[Index].GetPreferredWidth(rowIndex, dataGridViewRow.Thickness);
                    }
                    else
                    {
                        preferredCellThickness = dataGridViewRow.Cells[Index].GetPreferredSize(rowIndex).Width;
                    }
                    if (preferredColumnThickness < preferredCellThickness)
                    {
                        preferredColumnThickness = preferredCellThickness;
                    }
                    cy += dataGridViewRow.Thickness;
                    rowIndex = dataGridView.Rows.GetNextRow(rowIndex,
                        DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);
                }

                if (cy < displayHeight)
                {
                    rowIndex = dataGridView.DisplayedBandsInfo.FirstDisplayedScrollingRow;
                    while (rowIndex != -1 && cy < displayHeight)
                    {
                        dataGridViewRow = dataGridView.Rows.SharedRow(rowIndex);
                        if (fixedHeight)
                        {
                            preferredCellThickness = dataGridViewRow.Cells[Index].GetPreferredWidth(rowIndex, dataGridViewRow.Thickness);
                        }
                        else
                        {
                            preferredCellThickness = dataGridViewRow.Cells[Index].GetPreferredSize(rowIndex).Width;
                        }
                        if (preferredColumnThickness < preferredCellThickness)
                        {
                            preferredColumnThickness = preferredCellThickness;
                        }
                        cy += dataGridViewRow.Thickness;
                        rowIndex = dataGridView.Rows.GetNextRow(rowIndex, DataGridViewElementStates.Visible);
                    }
                }
            }
            return preferredColumnThickness;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append("DataGridViewColumn { Name=");
            sb.Append(Name);
            sb.Append(", Index=");
            sb.Append(Index.ToString(CultureInfo.CurrentCulture));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}

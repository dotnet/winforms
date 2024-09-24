// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

/// <summary>
///  Base class for the columns in a data grid view.
/// </summary>
[Designer($"System.Windows.Forms.Design.DataGridViewColumnDesigner, {AssemblyRef.SystemDesign}")]
[DesignTimeVisible(false)]
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
[ToolboxItem(false)]
[TypeConverter(typeof(DataGridViewColumnConverter))]
public class DataGridViewColumn : DataGridViewBand, IComponent
{
    private const float DefaultFillWeight = 100F;
    private const int DefaultWidth = 100;
    private const int DefaultMinColumnThickness = 5;

    private const byte AutomaticSort = 0x01;
    private const byte ProgrammaticSort = 0x02;
    private const byte ColumnIsDataBound = 0x04;
    private const byte ColumnIsBrowsableInternal = 0x08;
    private const byte DisplayIndexHasChangedInternal = 0x10;

    private byte _flags;  // see DATAGRIDVIEWCOLUMN_ constants above
    private string _name;
    private int _displayIndex;
    private float _fillWeight, _usedFillWeight;
    private DataGridViewAutoSizeColumnMode _autoSizeMode;
    private string _dataPropertyName = string.Empty;

    // needed for IComponent
    private EventHandler? _disposed;

    private static readonly int s_propDataGridViewColumnValueType = PropertyStore.CreateKey();

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataGridViewColumn"/> class.
    /// </summary>
    public DataGridViewColumn()
        : this(cellTemplate: null)
    {
    }

    public DataGridViewColumn(DataGridViewCell? cellTemplate)
        : base()
    {
        _fillWeight = DefaultFillWeight;
        _usedFillWeight = DefaultFillWeight;
        Thickness = ScaleToCurrentDpi(DefaultWidth);
        MinimumThickness = ScaleToCurrentDpi(DefaultMinColumnThickness);
        _name = string.Empty;
        _displayIndex = -1;
        CellTemplate = cellTemplate;
        _autoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
    }

    /// <summary>
    ///  Scale to current device dpi settings
    /// </summary>
    /// <param name="value"> initial value</param>
    /// <returns> scaled metric</returns>
    private static int ScaleToCurrentDpi(int value) =>
        ScaleHelper.IsScalingRequirementMet ? ScaleHelper.ScaleToDpi(value, ScaleHelper.InitialSystemDpi) : value;

    [SRCategory(nameof(SR.CatLayout))]
    [DefaultValue(DataGridViewAutoSizeColumnMode.NotSet)]
    [SRDescription(nameof(SR.DataGridViewColumn_AutoSizeModeDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    public DataGridViewAutoSizeColumnMode AutoSizeMode
    {
        get => _autoSizeMode;
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

            if (_autoSizeMode != value)
            {
                if (Visible && DataGridView is not null)
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
                bool previousInheritedModeAutoSized = previousInheritedMode is not DataGridViewAutoSizeColumnMode.Fill
                    and not DataGridViewAutoSizeColumnMode.None
                    and not DataGridViewAutoSizeColumnMode.NotSet;
                _autoSizeMode = value;
                if (DataGridView is null)
                {
                    if (InheritedAutoSizeMode is not DataGridViewAutoSizeColumnMode.Fill
                        and not DataGridViewAutoSizeColumnMode.None
                        and not DataGridViewAutoSizeColumnMode.NotSet)
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
    internal TypeConverter? BoundColumnConverter { get; set; }

    internal int BoundColumnIndex { get; set; } = -1;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual DataGridViewCell? CellTemplate { get; set; }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Type? CellType => CellTemplate?.GetType();

    [DefaultValue(null)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_ColumnContextMenuStripDescr))]
    public override ContextMenuStrip? ContextMenuStrip
    {
        get => base.ContextMenuStrip;
        set => base.ContextMenuStrip = value;
    }

    [Browsable(true)]
    [DefaultValue("")]
    [TypeConverter($"System.Windows.Forms.Design.DataMemberFieldConverter, {AssemblyRef.SystemDesign}")]
    [Editor($"System.Windows.Forms.Design.DataGridViewColumnDataPropertyNameEditor, {AssemblyRef.SystemDesign}", typeof(Drawing.Design.UITypeEditor))]
    [SRDescription(nameof(SR.DataGridView_ColumnDataPropertyNameDescr))]
    [SRCategory(nameof(SR.CatData))]
    [AllowNull]
    public string DataPropertyName
    {
        get => _dataPropertyName;
        set
        {
            value ??= string.Empty;

            if (value != _dataPropertyName)
            {
                _dataPropertyName = value;
                DataGridView?.OnColumnDataPropertyNameChanged(this);
            }
        }
    }

    [Browsable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_ColumnDefaultCellStyleDescr))]
    [AllowNull]
    public override DataGridViewCellStyle DefaultCellStyle
    {
        get => base.DefaultCellStyle;
        set => base.DefaultCellStyle = value;
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
                defaultCellStyle.Font is not null ||
                !defaultCellStyle.IsNullValueDefault ||
                !defaultCellStyle.IsDataSourceNullValueDefault ||
                !string.IsNullOrEmpty(defaultCellStyle.Format) ||
                !defaultCellStyle.FormatProvider.Equals(CultureInfo.CurrentCulture) ||
                defaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet ||
                defaultCellStyle.WrapMode != DataGridViewTriState.NotSet ||
                defaultCellStyle.Tag is not null ||
                !defaultCellStyle.Padding.Equals(Padding.Empty));
    }

    internal int DesiredFillWidth { get; set; }

    internal int DesiredMinimumWidth { get; set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int DisplayIndex
    {
        get => _displayIndex;
        set
        {
            if (_displayIndex != value)
            {
                if (value == int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.DataGridViewColumn_DisplayIndexTooLarge, int.MaxValue));
                }

                if (DataGridView is not null)
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
                    _displayIndex = value;
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

                    _displayIndex = value;
                }
            }
        }
    }

    internal bool DisplayIndexHasChanged
    {
        get => (_flags & DisplayIndexHasChangedInternal) != 0;
        set
        {
            if (value)
            {
                _flags |= DisplayIndexHasChangedInternal;
            }
            else
            {
                _flags = (byte)(_flags & ~DisplayIndexHasChangedInternal);
            }
        }
    }

    internal int DisplayIndexInternal
    {
        set
        {
            Debug.Assert(value >= -1);
            Debug.Assert(value < int.MaxValue);

            _displayIndex = value;
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public event EventHandler? Disposed
    {
        add => _disposed += value;
        remove => _disposed -= value;
    }

    [DefaultValue(0)]
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.DataGridView_ColumnDividerWidthDescr))]
    public int DividerWidth
    {
        get => DividerThickness;
        set => DividerThickness = value;
    }

    [SRCategory(nameof(SR.CatLayout))]
    [DefaultValue(DefaultFillWeight)]
    [SRDescription(nameof(SR.DataGridViewColumn_FillWeightDescr))]
    public float FillWeight
    {
        get => _fillWeight;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, ushort.MaxValue);

            if (DataGridView is not null)
            {
                DataGridView.OnColumnFillWeightChanging(this, value);
                _fillWeight = value;
                DataGridView.OnColumnFillWeightChanged(this);
            }
            else
            {
                _fillWeight = value;
            }
        }
    }

    internal float FillWeightInternal
    {
        set
        {
            Debug.Assert(value > 0);
            _fillWeight = value;
        }
    }

    [DefaultValue(false)]
    [RefreshProperties(RefreshProperties.All)]
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.DataGridView_ColumnFrozenDescr))]
    public override bool Frozen
    {
        get => base.Frozen;
        set => base.Frozen = value;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public DataGridViewColumnHeaderCell HeaderCell
    {
        get => (DataGridViewColumnHeaderCell)HeaderCellCore;
        set => HeaderCellCore = value;
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_ColumnHeaderTextDescr))]
    [Localizable(true)]
    [AllowNull]
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
            if ((value is not null || HasHeaderCell) &&
                HeaderCell.ValueType is not null &&
                HeaderCell.ValueType.IsAssignableFrom(typeof(string)))
            {
                HeaderCell.Value = value;
            }
        }
    }

    private bool ShouldSerializeHeaderText() => HasHeaderCell && HeaderCell.ContainsLocalValue;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DataGridViewAutoSizeColumnMode InheritedAutoSizeMode => GetInheritedAutoSizeMode(DataGridView);

    [Browsable(false)]
    public override DataGridViewCellStyle? InheritedStyle
    {
        get
        {
            DataGridViewCellStyle? columnStyle = null;
            if (HasDefaultCellStyle)
            {
                columnStyle = DefaultCellStyle;
                Debug.Assert(columnStyle is not null);
            }

            if (DataGridView is null)
            {
                return columnStyle;
            }

            DataGridViewCellStyle inheritedCellStyleTmp = new();
            DataGridViewCellStyle dataGridViewStyle = DataGridView.DefaultCellStyle;
            Debug.Assert(dataGridViewStyle is not null);

            if (columnStyle is not null && !columnStyle.BackColor.IsEmpty)
            {
                inheritedCellStyleTmp.BackColor = columnStyle.BackColor;
            }
            else
            {
                inheritedCellStyleTmp.BackColor = dataGridViewStyle.BackColor;
            }

            if (columnStyle is not null && !columnStyle.ForeColor.IsEmpty)
            {
                inheritedCellStyleTmp.ForeColor = columnStyle.ForeColor;
            }
            else
            {
                inheritedCellStyleTmp.ForeColor = dataGridViewStyle.ForeColor;
            }

            if (columnStyle is not null && !columnStyle.SelectionBackColor.IsEmpty)
            {
                inheritedCellStyleTmp.SelectionBackColor = columnStyle.SelectionBackColor;
            }
            else
            {
                inheritedCellStyleTmp.SelectionBackColor = dataGridViewStyle.SelectionBackColor;
            }

            if (columnStyle is not null && !columnStyle.SelectionForeColor.IsEmpty)
            {
                inheritedCellStyleTmp.SelectionForeColor = columnStyle.SelectionForeColor;
            }
            else
            {
                inheritedCellStyleTmp.SelectionForeColor = dataGridViewStyle.SelectionForeColor;
            }

            if (columnStyle is not null && columnStyle.Font is not null)
            {
                inheritedCellStyleTmp.Font = columnStyle.Font;
            }
            else
            {
                inheritedCellStyleTmp.Font = dataGridViewStyle.Font;
            }

            if (columnStyle is not null && !columnStyle.IsNullValueDefault)
            {
                inheritedCellStyleTmp.NullValue = columnStyle.NullValue;
            }
            else
            {
                inheritedCellStyleTmp.NullValue = dataGridViewStyle.NullValue;
            }

            if (columnStyle is not null && !columnStyle.IsDataSourceNullValueDefault)
            {
                inheritedCellStyleTmp.DataSourceNullValue = columnStyle.DataSourceNullValue;
            }
            else
            {
                inheritedCellStyleTmp.DataSourceNullValue = dataGridViewStyle.DataSourceNullValue;
            }

            if (columnStyle is not null && columnStyle.Format.Length != 0)
            {
                inheritedCellStyleTmp.Format = columnStyle.Format;
            }
            else
            {
                inheritedCellStyleTmp.Format = dataGridViewStyle.Format;
            }

            if (columnStyle is not null && !columnStyle.IsFormatProviderDefault)
            {
                inheritedCellStyleTmp.FormatProvider = columnStyle.FormatProvider;
            }
            else
            {
                inheritedCellStyleTmp.FormatProvider = dataGridViewStyle.FormatProvider;
            }

            if (columnStyle is not null && columnStyle.Alignment != DataGridViewContentAlignment.NotSet)
            {
                inheritedCellStyleTmp.AlignmentInternal = columnStyle.Alignment;
            }
            else
            {
                Debug.Assert(dataGridViewStyle.Alignment != DataGridViewContentAlignment.NotSet);
                inheritedCellStyleTmp.AlignmentInternal = dataGridViewStyle.Alignment;
            }

            if (columnStyle is not null && columnStyle.WrapMode != DataGridViewTriState.NotSet)
            {
                inheritedCellStyleTmp.WrapModeInternal = columnStyle.WrapMode;
            }
            else
            {
                Debug.Assert(dataGridViewStyle.WrapMode != DataGridViewTriState.NotSet);
                inheritedCellStyleTmp.WrapModeInternal = dataGridViewStyle.WrapMode;
            }

            if (columnStyle is not null && columnStyle.Tag is not null)
            {
                inheritedCellStyleTmp.Tag = columnStyle.Tag;
            }
            else
            {
                inheritedCellStyleTmp.Tag = dataGridViewStyle.Tag;
            }

            if (columnStyle is not null && columnStyle.Padding != Padding.Empty)
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
        get => (_flags & ColumnIsBrowsableInternal) != 0;
        set
        {
            if (value)
            {
                _flags |= ColumnIsBrowsableInternal;
            }
            else
            {
                _flags = (byte)(_flags & ~ColumnIsBrowsableInternal);
            }
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsDataBound => IsDataBoundInternal;

    internal bool IsDataBoundInternal
    {
        get => (_flags & ColumnIsDataBound) != 0;
        set
        {
            if (value)
            {
                _flags |= ColumnIsDataBound;
            }
            else
            {
                _flags = (byte)(_flags & ~ColumnIsDataBound);
            }
        }
    }

    [DefaultValue(DefaultMinColumnThickness)]
    [Localizable(true)]
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.DataGridView_ColumnMinimumWidthDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    public int MinimumWidth
    {
        get => MinimumThickness;
        set => MinimumThickness = value;
    }

    [Browsable(false)]
    [AllowNull]
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
            // NOTE: one side effect of bringing together the design time and the run time "Name" properties is that
            // DataGridViewColumn::Name changes. However, DataGridView does not fire ColumnNameChanged event.
            // We can't fix this because ISite does not provide Name change notification. So in effect
            // DataGridViewColumn does not know when its name changed.
            // I talked w/ MarkRi and he is perfectly fine w/ DataGridViewColumn::Name changing w/o ColumnNameChanged
            // being fired.
            //
            if (Site is not null && !string.IsNullOrEmpty(Site.Name))
            {
                _name = Site.Name;
            }

            return _name;
        }
        set
        {
            string oldName = _name;
            if (string.IsNullOrEmpty(value))
            {
                _name = string.Empty;
            }
            else
            {
                _name = value;
            }

            if (DataGridView is not null && !string.Equals(_name, oldName, StringComparison.Ordinal))
            {
                DataGridView.OnColumnNameChanged(this);
            }
        }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_ColumnReadOnlyDescr))]
    public override bool ReadOnly
    {
        get => base.ReadOnly;
        set
        {
            if (IsDataBound &&
                DataGridView is not null &&
                DataGridView.DataConnection is not null &&
                BoundColumnIndex != -1 &&
                DataGridView.DataConnection.DataFieldIsReadOnly(BoundColumnIndex) &&
                !value)
            {
                throw new InvalidOperationException(SR.DataGridView_ColumnBoundToAReadOnlyFieldMustRemainReadOnly);
            }

            base.ReadOnly = value;
        }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_ColumnResizableDescr))]
    public override DataGridViewTriState Resizable
    {
        get => base.Resizable;
        set => base.Resizable = value;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ISite? Site { get; set; }

    [DefaultValue(DataGridViewColumnSortMode.NotSortable)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DataGridView_ColumnSortModeDescr))]
    public DataGridViewColumnSortMode SortMode
    {
        get
        {
            if ((_flags & AutomaticSort) != 0x00)
            {
                return DataGridViewColumnSortMode.Automatic;
            }
            else if ((_flags & ProgrammaticSort) != 0x00)
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
                    if (DataGridView is not null &&
                        !DataGridView.InInitialization &&
                        value == DataGridViewColumnSortMode.Automatic &&
                        (DataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect ||
                        DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect))
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_SortModeAndSelectionModeClash, (value).ToString(), DataGridView.SelectionMode.ToString()));
                    }

                    if (value == DataGridViewColumnSortMode.Automatic)
                    {
                        _flags = (byte)(_flags & ~ProgrammaticSort);
                        _flags |= AutomaticSort;
                    }
                    else
                    {
                        _flags = (byte)(_flags & ~AutomaticSort);
                        _flags |= ProgrammaticSort;
                    }
                }
                else
                {
                    _flags = (byte)(_flags & ~AutomaticSort);
                    _flags = (byte)(_flags & ~ProgrammaticSort);
                }

                DataGridView?.OnColumnSortModeChanged(this);
            }
        }
    }

    [DefaultValue("")]
    [Localizable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_ColumnToolTipTextDescr))]
    [AllowNull]
    public string ToolTipText
    {
        get => HeaderCell.ToolTipText;
        set
        {
            if (string.Compare(ToolTipText, value, ignoreCase: false, CultureInfo.InvariantCulture) != 0)
            {
                HeaderCell.ToolTipText = value;

                DataGridView?.OnColumnToolTipTextChanged(this);
            }
        }
    }

    internal float UsedFillWeight
    {
        get => _usedFillWeight;
        set
        {
            Debug.Assert(value > 0);
            _usedFillWeight = value;
        }
    }

    [Browsable(false)]
    [DefaultValue(null)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Type? ValueType
    {
        get => Properties.GetValueOrDefault<Type>(s_propDataGridViewColumnValueType);
        set => Properties.AddOrRemoveValue(s_propDataGridViewColumnValueType, value);
    }

    [DefaultValue(true)]
    [Localizable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DataGridView_ColumnVisibleDescr))]
    public override bool Visible
    {
        get => base.Visible;
        set => base.Visible = value;
    }

    [SRCategory(nameof(SR.CatLayout))]
    [Localizable(true)]
    [SRDescription(nameof(SR.DataGridView_ColumnWidthDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    public int Width
    {
        get => Thickness;
        set => Thickness = value;
    }

    public override object Clone()
    {
        DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)Activator.CreateInstance(GetType())!;
        CloneInternal(dataGridViewColumn);

        return dataGridViewColumn;
    }

    private protected void CloneInternal(DataGridViewColumn dataGridViewColumn)
    {
        base.CloneInternal(dataGridViewColumn);

        dataGridViewColumn._name = Name;
        dataGridViewColumn._displayIndex = -1;
        dataGridViewColumn.HeaderText = HeaderText;
        dataGridViewColumn.DataPropertyName = DataPropertyName;
        dataGridViewColumn.CellTemplate = (DataGridViewCell?)CellTemplate?.Clone();

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
                lock (this)
                {
                    Site?.Container?.Remove(this);
                    _disposed?.Invoke(this, EventArgs.Empty);
                }
            }

            // If you are adding releasing unmanaged resources code here (disposing == false), you need to remove this
            // class type (and all of its subclasses) from check in DataGridViewElement()
            // constructor and DataGridViewElement_Subclasses_SuppressFinalizeCall test!
            // Also consider to modify ~DataGridViewBand() description.
        }
        finally
        {
            base.Dispose(disposing);
        }
    }

    internal DataGridViewAutoSizeColumnMode GetInheritedAutoSizeMode(DataGridView? dataGridView)
    {
        if (dataGridView is not null && _autoSizeMode == DataGridViewAutoSizeColumnMode.NotSet)
        {
            return dataGridView.AutoSizeColumnsMode switch
            {
                DataGridViewAutoSizeColumnsMode.AllCells => DataGridViewAutoSizeColumnMode.AllCells,
                DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader => DataGridViewAutoSizeColumnMode.AllCellsExceptHeader,
                DataGridViewAutoSizeColumnsMode.DisplayedCells => DataGridViewAutoSizeColumnMode.DisplayedCells,
                DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader => DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader,
                DataGridViewAutoSizeColumnsMode.ColumnHeader => DataGridViewAutoSizeColumnMode.ColumnHeader,
                DataGridViewAutoSizeColumnsMode.Fill => DataGridViewAutoSizeColumnMode.Fill,
                _ => DataGridViewAutoSizeColumnMode.None,
            };
        }

        return _autoSizeMode;
    }

    public virtual int GetPreferredWidth(DataGridViewAutoSizeColumnMode autoSizeColumnMode, bool fixedHeight)
    {
        if (autoSizeColumnMode is DataGridViewAutoSizeColumnMode.NotSet
            or DataGridViewAutoSizeColumnMode.None
            or DataGridViewAutoSizeColumnMode.Fill)
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

        DataGridView? dataGridView = DataGridView;

        Debug.Assert(dataGridView is null || Index > -1);

        if (dataGridView is null)
        {
            return -1;
        }

        DataGridViewAutoSizeColumnCriteriaInternal autoSizeColumnCriteriaInternal = (DataGridViewAutoSizeColumnCriteriaInternal)autoSizeColumnMode;
        Debug.Assert(autoSizeColumnCriteriaInternal is DataGridViewAutoSizeColumnCriteriaInternal.Header
            or DataGridViewAutoSizeColumnCriteriaInternal.AllRows
            or DataGridViewAutoSizeColumnCriteriaInternal.DisplayedRows
            or (DataGridViewAutoSizeColumnCriteriaInternal.Header | DataGridViewAutoSizeColumnCriteriaInternal.AllRows)
            or (DataGridViewAutoSizeColumnCriteriaInternal.Header | DataGridViewAutoSizeColumnCriteriaInternal.DisplayedRows));

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

    public override string ToString() =>
        $"DataGridViewColumn {{ Name={Name}, Index={Index} }}";
}

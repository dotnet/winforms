// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public partial class DataGridViewComboBoxCell : DataGridViewCell
{
    private static readonly int s_propComboBoxCellDataSource = PropertyStore.CreateKey();
    private static readonly int s_propComboBoxCellDisplayMember = PropertyStore.CreateKey();
    private static readonly int s_propComboBoxCellValueMember = PropertyStore.CreateKey();
    private static readonly int s_propComboBoxCellItems = PropertyStore.CreateKey();
    private static readonly int s_propComboBoxCellDropDownWidth = PropertyStore.CreateKey();
    private static readonly int s_propComboBoxCellMaxDropDownItems = PropertyStore.CreateKey();
    private static readonly int s_propComboBoxCellEditingComboBox = PropertyStore.CreateKey();
    private static readonly int s_propComboBoxCellValueMemberProp = PropertyStore.CreateKey();
    private static readonly int s_propComboBoxCellDisplayMemberProp = PropertyStore.CreateKey();
    private static readonly int s_propComboBoxCellDataManager = PropertyStore.CreateKey();
    private static readonly int s_propComboBoxCellColumnTemplate = PropertyStore.CreateKey();
    private static readonly int s_propComboBoxCellFlatStyle = PropertyStore.CreateKey();
    private static readonly int s_propComboBoxCellDisplayStyle = PropertyStore.CreateKey();
    private static readonly int s_propComboBoxCellDisplayStyleForCurrentCellOnly = PropertyStore.CreateKey();

    private const byte Margin = 3;
    private const byte NonXPTriangleHeight = 4;
    private const byte NonXPTriangleWidth = 7;
    private const byte HorizontalTextMarginLeft = 0;
    private const byte VerticalTextMarginTopWithWrapping = 0;
    private const byte VerticalTextMarginTopWithoutWrapping = 1;

    internal const int DefaultMaxDropDownItems = 8;

    private static readonly Type s_defaultFormattedValueType = typeof(string);
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.Interfaces)]
    private static readonly Type s_defaultEditType = typeof(DataGridViewComboBoxEditingControl);
    private static readonly Type s_defaultValueType = typeof(object);
    private static readonly Type s_cellType = typeof(DataGridViewComboBoxCell);

    private DataGridViewComboBoxCellFlags _flags;
    private static bool s_mouseInDropDownButtonBounds;
    private static int s_cachedDropDownWidth = -1;

    // Autosizing changed for VS
    // We need to make ItemFromComboBoxDataSource as fast as possible because ItemFromComboBoxDataSource is getting called a lot
    // during AutoSize. To do that we keep a copy of the key and the value.
    // private object keyUsedDuringAutoSize    = null;
    // private object valueUsedDuringAutoSize  = null;

    private static bool s_isScalingInitialized;
    private const int Offset2Pixels = 2;
    private static int s_offset2X = Offset2Pixels;
    private static int s_offset2Y = Offset2Pixels;
    private static byte s_nonXPTriangleHeight = NonXPTriangleHeight;
    private static byte s_nonXPTriangleWidth = NonXPTriangleWidth;

    public DataGridViewComboBoxCell()
    {
        _flags = DataGridViewComboBoxCellFlags.CellAutoComplete;
        if (!s_isScalingInitialized)
        {
            if (ScaleHelper.IsScalingRequired)
            {
                s_offset2X = ScaleHelper.ScaleToInitialSystemDpi(Offset2Pixels);
                s_offset2Y = ScaleHelper.ScaleToInitialSystemDpi(Offset2Pixels);
                s_nonXPTriangleWidth = (byte)ScaleHelper.ScaleToInitialSystemDpi(NonXPTriangleWidth);
                s_nonXPTriangleHeight = (byte)ScaleHelper.ScaleToInitialSystemDpi(NonXPTriangleHeight);
            }

            s_isScalingInitialized = true;
        }
    }

    /// <summary>
    ///  Creates a new AccessibleObject for this DataGridViewComboBoxCell instance.
    ///  The AccessibleObject instance returned by this method supports ControlType UIA property.
    /// </summary>
    /// <returns>
    ///  AccessibleObject for this DataGridViewComboBoxCell instance.
    /// </returns>
    protected override AccessibleObject CreateAccessibilityInstance() => new DataGridViewComboBoxCellAccessibleObject(this);

    [DefaultValue(true)]
    public virtual bool AutoComplete
    {
        get => _flags.HasFlag(DataGridViewComboBoxCellFlags.CellAutoComplete);
        set
        {
            if (value == AutoComplete)
            {
                return;
            }

            if (value)
            {
                _flags |= DataGridViewComboBoxCellFlags.CellAutoComplete;
            }
            else
            {
                _flags &= ~DataGridViewComboBoxCellFlags.CellAutoComplete;
            }

            if (OwnsEditingComboBox(RowIndex))
            {
                if (value)
                {
                    EditingComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
                    EditingComboBox.AutoCompleteMode = AutoCompleteMode.Append;
                }
                else
                {
                    EditingComboBox.AutoCompleteMode = AutoCompleteMode.None;
                    EditingComboBox.AutoCompleteSource = AutoCompleteSource.None;
                }
            }
        }
    }

    private CurrencyManager? DataManager
    {
        get => GetDataManager(DataGridView);
        set => Properties.AddOrRemoveValue(s_propComboBoxCellDataManager, value);
    }

    public virtual object? DataSource
    {
        get => Properties.GetValueOrDefault<object>(s_propComboBoxCellDataSource);
        set
        {
            // Same check as for ListControl's DataSource
            if (value is not null and not (IList or IListSource))
            {
                throw new ArgumentException(SR.BadDataSourceForComplexBinding);
            }

            object? originalValue = Properties.AddOrRemoveValue(s_propComboBoxCellDataSource, value);
            if (originalValue == value)
            {
                return;
            }

            // Invalidate the currency manager
            DataManager = null;

            UnwireDataSource();
            WireDataSource(value);

            // Invalidate existing Items collection
            CreateItemsFromDataSource = true;
            s_cachedDropDownWidth = -1;

            try
            {
                InitializeDisplayMemberPropertyDescriptor(DisplayMember);
            }
            catch (Exception exception) when (!exception.IsCriticalException())
            {
                Debug.Assert(DisplayMember is not null && DisplayMember.Length > 0);
                DisplayMemberInternal = null;
            }

            try
            {
                InitializeValueMemberPropertyDescriptor(ValueMember);
            }
            catch (Exception exception) when (!exception.IsCriticalException())
            {
                Debug.Assert(ValueMember is not null && ValueMember.Length > 0);
                ValueMemberInternal = null;
            }

            if (value is null)
            {
                DisplayMemberInternal = null;
                ValueMemberInternal = null;
            }

            if (OwnsEditingComboBox(RowIndex))
            {
                EditingComboBox.DataSource = value;
                InitializeComboBoxText();
            }
            else
            {
                OnCommonChange();
            }
        }
    }

    [DefaultValue("")]
    [AllowNull]
    public virtual string DisplayMember
    {
        get => Properties.GetStringOrEmptyString(s_propComboBoxCellDisplayMember);
        set
        {
            DisplayMemberInternal = value;
            if (OwnsEditingComboBox(RowIndex))
            {
                EditingComboBox.DisplayMember = value;
                InitializeComboBoxText();
            }
            else
            {
                OnCommonChange();
            }
        }
    }

    private string? DisplayMemberInternal
    {
        set
        {
            InitializeDisplayMemberPropertyDescriptor(value);
            Properties.AddOrRemoveString(s_propComboBoxCellDisplayMember, value);
        }
    }

    private PropertyDescriptor? DisplayMemberProperty
    {
        get => Properties.GetValueOrDefault<PropertyDescriptor?>(s_propComboBoxCellDisplayMemberProp);
        set => Properties.AddOrRemoveValue(s_propComboBoxCellDisplayMemberProp, value);
    }

    [DefaultValue(DataGridViewComboBoxDisplayStyle.DropDownButton)]
    public DataGridViewComboBoxDisplayStyle DisplayStyle
    {
        get => Properties.GetValueOrDefault(s_propComboBoxCellDisplayStyle, DataGridViewComboBoxDisplayStyle.DropDownButton);
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            DataGridViewComboBoxDisplayStyle originalValue = Properties.AddOrRemoveValue(
                s_propComboBoxCellDisplayStyle,
                value,
                defaultValue: DataGridViewComboBoxDisplayStyle.DropDownButton);

            if (value != originalValue && DataGridView is not null)
            {
                if (RowIndex != -1)
                {
                    DataGridView.InvalidateCell(this);
                }
                else
                {
                    DataGridView.InvalidateColumnInternal(ColumnIndex);
                }
            }
        }
    }

    internal DataGridViewComboBoxDisplayStyle DisplayStyleInternal
    {
        set
        {
            Debug.Assert(value is >= DataGridViewComboBoxDisplayStyle.ComboBox and <= DataGridViewComboBoxDisplayStyle.Nothing);
            Properties.AddOrRemoveValue(s_propComboBoxCellDisplayStyle, value, defaultValue: DataGridViewComboBoxDisplayStyle.DropDownButton);
        }
    }

    [DefaultValue(false)]
    public bool DisplayStyleForCurrentCellOnly
    {
        get => Properties.GetValueOrDefault<bool>(s_propComboBoxCellDisplayStyleForCurrentCellOnly);
        set
        {
            bool originalValue = Properties.AddOrRemoveValue(s_propComboBoxCellDisplayStyleForCurrentCellOnly, value);
            if (originalValue != value && DataGridView is not null)
            {
                if (RowIndex != -1)
                {
                    DataGridView.InvalidateCell(this);
                }
                else
                {
                    DataGridView.InvalidateColumnInternal(ColumnIndex);
                }
            }
        }
    }

    internal bool DisplayStyleForCurrentCellOnlyInternal
    {
        set => Properties.AddOrRemoveValue(s_propComboBoxCellDisplayStyleForCurrentCellOnly, value);
    }

    private Type DisplayType
    {
        get
        {
            if (DisplayMemberProperty is not null)
            {
                return DisplayMemberProperty.PropertyType;
            }
            else if (ValueMemberProperty is not null)
            {
                return ValueMemberProperty.PropertyType;
            }
            else
            {
                return s_defaultFormattedValueType;
            }
        }
    }

    private TypeConverter DisplayTypeConverter => DataGridView is not null
        ? DataGridView.GetCachedTypeConverter(DisplayType)
        : TypeDescriptor.GetConverter(DisplayType);

    [DefaultValue(1)]
    public virtual int DropDownWidth
    {
        get => Properties.GetValueOrDefault(s_propComboBoxCellDropDownWidth, 1);
        set
        {
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(DropDownWidth), value, string.Format(SR.DataGridViewComboBoxCell_DropDownWidthOutOfRange, 1));
            }

            Properties.AddOrRemoveValue(s_propComboBoxCellDropDownWidth, value, defaultValue: 1);
            if (OwnsEditingComboBox(RowIndex))
            {
                EditingComboBox.DropDownWidth = value;
            }
        }
    }

    private DataGridViewComboBoxEditingControl? EditingComboBox
    {
        get => Properties.GetValueOrDefault<DataGridViewComboBoxEditingControl?>(s_propComboBoxCellEditingComboBox);
        set => Properties.AddOrRemoveValue(s_propComboBoxCellEditingComboBox, value);
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.Interfaces)]
    public override Type EditType => s_defaultEditType;

    [DefaultValue(FlatStyle.Standard)]
    public FlatStyle FlatStyle
    {
        get => Properties.GetValueOrDefault(s_propComboBoxCellFlatStyle, FlatStyle.Standard);
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            FlatStyle originalValue = Properties.AddOrRemoveValue(s_propComboBoxCellFlatStyle, value, defaultValue: FlatStyle.Standard);
            if (value != originalValue)
            {
                OnCommonChange();
            }
        }
    }

    internal FlatStyle FlatStyleInternal
    {
        set
        {
            Debug.Assert(value is >= FlatStyle.Flat and <= FlatStyle.System);
            Properties.AddOrRemoveValue(s_propComboBoxCellFlatStyle, value, defaultValue: FlatStyle.Standard);
        }
    }

    public override Type FormattedValueType => s_defaultFormattedValueType;

    internal bool HasItems => Properties.ContainsKey(s_propComboBoxCellItems);

    [Browsable(false)]
    public virtual ObjectCollection Items => GetItems(DataGridView);

    [DefaultValue(DefaultMaxDropDownItems)]
    public virtual int MaxDropDownItems
    {
        get => Properties.GetValueOrDefault(s_propComboBoxCellMaxDropDownItems, DefaultMaxDropDownItems);
        set
        {
            if (value is < 1 or > 100)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(MaxDropDownItems),
                    value,
                    string.Format(SR.DataGridViewComboBoxCell_MaxDropDownItemsOutOfRange, 1, 100));
            }

            Properties.AddOrRemoveValue(s_propComboBoxCellMaxDropDownItems, value, defaultValue: DefaultMaxDropDownItems);
            if (OwnsEditingComboBox(RowIndex))
            {
                EditingComboBox.MaxDropDownItems = value;
            }
        }
    }

    private bool PaintXPThemes
    {
        get
        {
            bool paintFlat = FlatStyle is FlatStyle.Flat or FlatStyle.Popup;
            return !paintFlat && DataGridView.ApplyVisualStylesToInnerCells;
        }
    }

    private static bool PostXPThemesExist =>
        VisualStyleRenderer.IsElementDefined(VisualStyleElement.ComboBox.ReadOnlyButton.Normal);

    [DefaultValue(false)]
    public virtual bool Sorted
    {
        get => _flags.HasFlag(DataGridViewComboBoxCellFlags.CellSorted);
        set
        {
            if (value == Sorted)
            {
                return;
            }

            if (value)
            {
                if (DataSource is null)
                {
                    Items.SortInternal();
                }
                else
                {
                    throw new ArgumentException(SR.ComboBoxSortWithDataSource);
                }

                _flags |= DataGridViewComboBoxCellFlags.CellSorted;
            }
            else
            {
                _flags &= ~DataGridViewComboBoxCellFlags.CellSorted;
            }

            if (OwnsEditingComboBox(RowIndex))
            {
                EditingComboBox.Sorted = value;
            }
        }
    }

    internal DataGridViewComboBoxColumn? TemplateComboBoxColumn
    {
        get => Properties.GetValueOrDefault<DataGridViewComboBoxColumn>(s_propComboBoxCellColumnTemplate);
        set => Properties.AddOrRemoveValue(s_propComboBoxCellColumnTemplate, value);
    }

    [DefaultValue("")]
    [AllowNull]
    public virtual string ValueMember
    {
        get => Properties.GetStringOrEmptyString(s_propComboBoxCellValueMember);
        set
        {
            ValueMemberInternal = value;
            if (OwnsEditingComboBox(RowIndex))
            {
                EditingComboBox.ValueMember = value;
                InitializeComboBoxText();
            }
            else
            {
                OnCommonChange();
            }
        }
    }

    private string? ValueMemberInternal
    {
        set
        {
            InitializeValueMemberPropertyDescriptor(value);
            Properties.AddOrRemoveString(s_propComboBoxCellValueMember, value);
        }
    }

    private PropertyDescriptor? ValueMemberProperty
    {
        get => Properties.GetValueOrDefault<PropertyDescriptor?>(s_propComboBoxCellValueMemberProp);
        set => Properties.AddOrRemoveValue(s_propComboBoxCellValueMemberProp, value);
    }

    public override Type ValueType
    {
        get
        {
            if (ValueMemberProperty is not null)
            {
                return ValueMemberProperty.PropertyType;
            }
            else if (DisplayMemberProperty is not null)
            {
                return DisplayMemberProperty.PropertyType;
            }
            else
            {
                Type? baseValueType = base.ValueType;
                if (baseValueType is not null)
                {
                    return baseValueType;
                }

                return s_defaultValueType;
            }
        }
    }

    // Called when the row that owns the editing control gets unshared.
    internal override void CacheEditingControl()
    {
        Debug.Assert(DataGridView is not null);
        EditingComboBox = DataGridView.EditingControl as DataGridViewComboBoxEditingControl;
    }

    private void CheckDropDownList(int x, int y, int rowIndex)
    {
        Debug.Assert(DataGridView is not null);
        Debug.Assert(EditingComboBox is not null);
        DataGridViewAdvancedBorderStyle dgvabsPlaceholder = new();
        DataGridViewAdvancedBorderStyle dgvabsEffective = AdjustCellBorderStyle(
            DataGridView.AdvancedCellBorderStyle,
            dgvabsPlaceholder,
            singleVerticalBorderAdded: false,
            singleHorizontalBorderAdded: false,
            isFirstDisplayedColumn: false,
            isFirstDisplayedRow: false);
        DataGridViewCellStyle cellStyle = GetInheritedStyle(inheritedCellStyle: null, rowIndex, includeColors: false);
        Rectangle borderAndPaddingWidths = BorderWidths(dgvabsEffective);
        borderAndPaddingWidths.X += cellStyle.Padding.Left;
        borderAndPaddingWidths.Y += cellStyle.Padding.Top;
        borderAndPaddingWidths.Width += cellStyle.Padding.Right;
        borderAndPaddingWidths.Height += cellStyle.Padding.Bottom;
        Size size = GetSize(rowIndex);
        Size adjustedSize = new(size.Width - borderAndPaddingWidths.X - borderAndPaddingWidths.Width,
                                     size.Height - borderAndPaddingWidths.Y - borderAndPaddingWidths.Height);

        int dropHeight;
        using (var screen = GdiCache.GetScreenDCGraphics())
        {
            dropHeight = Math.Min(GetDropDownButtonHeight(screen, cellStyle), adjustedSize.Height - 2);
        }

        int dropWidth = Math.Min(SystemInformation.HorizontalScrollBarThumbWidth, adjustedSize.Width - 2 * Margin - 1);

        if (dropHeight > 0 && dropWidth > 0 &&
            y >= borderAndPaddingWidths.Y + 1 &&
            y <= borderAndPaddingWidths.Y + 1 + dropHeight)
        {
            if (DataGridView.RightToLeftInternal)
            {
                if (x >= borderAndPaddingWidths.X + 1 &&
                    x <= borderAndPaddingWidths.X + dropWidth + 1)
                {
                    EditingComboBox.DroppedDown = true;
                }
            }
            else
            {
                if (x >= size.Width - borderAndPaddingWidths.Width - dropWidth - 1 &&
                    x <= size.Width - borderAndPaddingWidths.Width - 1)
                {
                    EditingComboBox.DroppedDown = true;
                }
            }
        }
    }

    private void CheckNoDataSource()
    {
        if (DataSource is not null)
        {
            throw new ArgumentException(SR.DataSourceLocksItems);
        }
    }

    private void ComboBox_DropDown(object? sender, EventArgs e)
    {
        Debug.Assert(DataGridView is not null);
        Debug.Assert(EditingComboBox is not null);

        ComboBox comboBox = EditingComboBox;
        if (OwningColumn is not DataGridViewComboBoxColumn owningComboBoxColumn)
        {
            return;
        }

        DataGridViewAutoSizeColumnMode autoSizeColumnMode = owningComboBoxColumn.GetInheritedAutoSizeMode(DataGridView);
        if (autoSizeColumnMode is not DataGridViewAutoSizeColumnMode.ColumnHeader
            and not DataGridViewAutoSizeColumnMode.Fill
            and not DataGridViewAutoSizeColumnMode.None)
        {
            if (DropDownWidth == 1)
            {
                // Owning ComboBox column is autosized based on inner cells.
                // Resize the dropdown list based on the max width of the items.
                if (s_cachedDropDownWidth == -1)
                {
                    int maxPreferredWidth = -1;
                    if ((HasItems || CreateItemsFromDataSource) && Items.Count > 0)
                    {
                        foreach (object item in Items)
                        {
                            Size preferredSize = TextRenderer.MeasureText(comboBox.GetItemText(item), comboBox.Font);
                            if (preferredSize.Width > maxPreferredWidth)
                            {
                                maxPreferredWidth = preferredSize.Width;
                            }
                        }
                    }

                    s_cachedDropDownWidth = maxPreferredWidth + 2 + SystemInformation.VerticalScrollBarWidth;
                }

                Debug.Assert(s_cachedDropDownWidth >= 1);
                PInvokeCore.SendMessage(comboBox, PInvoke.CB_SETDROPPEDWIDTH, (WPARAM)s_cachedDropDownWidth);
            }
        }
        else
        {
            // The dropdown width may have been previously adjusted to the items because of the owning column autosized.
            // The dropdown width needs to be realigned to the DropDownWidth property value.
            int dropDownWidth = (int)PInvokeCore.SendMessage(comboBox, PInvoke.CB_GETDROPPEDWIDTH);
            if (dropDownWidth != DropDownWidth)
            {
                PInvokeCore.SendMessage(comboBox, PInvoke.CB_SETDROPPEDWIDTH, (WPARAM)DropDownWidth);
            }
        }
    }

    public override object Clone()
    {
        DataGridViewComboBoxCell dataGridViewCell;
        Type thisType = GetType();

        if (thisType == s_cellType) // performance improvement
        {
            dataGridViewCell = new DataGridViewComboBoxCell();
        }
        else
        {
            dataGridViewCell = (DataGridViewComboBoxCell)Activator.CreateInstance(thisType)!;
        }

        CloneInternal(dataGridViewCell);
        dataGridViewCell.DropDownWidth = DropDownWidth;
        dataGridViewCell.MaxDropDownItems = MaxDropDownItems;
        dataGridViewCell.CreateItemsFromDataSource = false;
        dataGridViewCell.DataSource = DataSource;
        dataGridViewCell.DisplayMember = DisplayMember;
        dataGridViewCell.ValueMember = ValueMember;
        if (HasItems && DataSource is null && Items.Count > 0)
        {
            dataGridViewCell.Items.AddRangeInternal(Items.InnerArray.ToArray());
        }

        dataGridViewCell.AutoComplete = AutoComplete;
        dataGridViewCell.Sorted = Sorted;
        dataGridViewCell.FlatStyleInternal = FlatStyle;
        dataGridViewCell.DisplayStyleInternal = DisplayStyle;
        dataGridViewCell.DisplayStyleForCurrentCellOnlyInternal = DisplayStyleForCurrentCellOnly;
        return dataGridViewCell;
    }

    private bool CreateItemsFromDataSource
    {
        get => _flags.HasFlag(DataGridViewComboBoxCellFlags.CellCreateItemsFromDataSource);
        set
        {
            if (value)
            {
                _flags |= DataGridViewComboBoxCellFlags.CellCreateItemsFromDataSource;
            }
            else
            {
                _flags &= ~DataGridViewComboBoxCellFlags.CellCreateItemsFromDataSource;
            }
        }
    }

    private void DataSource_Disposed(object? sender, EventArgs e)
    {
        Debug.Assert(sender == DataSource, "How can we get dispose notification from anything other than our DataSource?");
        DataSource = null;
    }

    private void DataSource_Initialized(object? sender, EventArgs e)
    {
        Debug.Assert(sender == DataSource);
        Debug.Assert(DataSource is ISupportInitializeNotification);
        Debug.Assert(_flags.HasFlag(DataGridViewComboBoxCellFlags.DataSourceInitializedHookedUp));

        // Unhook the Initialized event.
        if (DataSource is ISupportInitializeNotification dsInit)
        {
            dsInit.Initialized -= DataSource_Initialized;
        }

        // The wait is over: DataSource is initialized.
        _flags &= ~DataGridViewComboBoxCellFlags.DataSourceInitializedHookedUp;

        // Check the DisplayMember and ValueMember values - will throw if values don't match existing fields.
        InitializeDisplayMemberPropertyDescriptor(DisplayMember);
        InitializeValueMemberPropertyDescriptor(ValueMember);
    }

    public override void DetachEditingControl()
    {
        DataGridView? dgv = DataGridView;
        if (dgv is null || dgv.EditingControl is null)
        {
            throw new InvalidOperationException();
        }

        if (EditingComboBox is not null &&
            _flags.HasFlag(DataGridViewComboBoxCellFlags.DropDownHookedUp))
        {
            EditingComboBox.DropDown -= ComboBox_DropDown;
            _flags &= ~DataGridViewComboBoxCellFlags.DropDownHookedUp;
        }

        EditingComboBox = null;
        base.DetachEditingControl();
    }

    protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
    {
        ArgumentNullException.ThrowIfNull(cellStyle);

        if (DataGridView is null || rowIndex < 0 || OwningColumn is null)
        {
            return Rectangle.Empty;
        }

        object? value = GetValue(rowIndex);
        object? formattedValue = GetEditedFormattedValue(
            value,
            rowIndex,
            ref cellStyle,
            DataGridViewDataErrorContexts.Formatting);

        ComputeBorderStyleCellStateAndCellBounds(
            rowIndex,
            out DataGridViewAdvancedBorderStyle dgvabsEffective,
            out DataGridViewElementStates cellState,
            out Rectangle cellBounds);

        Rectangle contentBounds = PaintPrivate(
            graphics,
            cellBounds,
            cellBounds,
            rowIndex,
            cellState,
            formattedValue,
            errorText: null,    // contentBounds is independent of errorText
            cellStyle,
            dgvabsEffective,
            out _,   // not used
            DataGridViewPaintParts.ContentForeground,
            computeContentBounds: true,
            computeErrorIconBounds: false,
            computeDropDownButtonRect: false,
            paint: false);

#if DEBUG
        Rectangle contentBoundsDebug = PaintPrivate(
            graphics,
            cellBounds,
            cellBounds,
            rowIndex,
            cellState,
            formattedValue,
            GetErrorText(rowIndex),
            cellStyle,
            dgvabsEffective,
            out _, // not used
            DataGridViewPaintParts.ContentForeground,
            computeContentBounds: true,
            computeErrorIconBounds: false,
            computeDropDownButtonRect: false,
            paint: false);
        Debug.Assert(contentBoundsDebug.Equals(contentBounds));
#endif

        return contentBounds;
    }

    private CurrencyManager? GetDataManager(DataGridView? dataGridView)
    {
        CurrencyManager? cm = Properties.GetValueOrDefault<CurrencyManager?>(s_propComboBoxCellDataManager);
        if (cm is null && DataSource is not null && dataGridView?.BindingContext is not null && !(DataSource == Convert.DBNull))
        {
            if (DataSource is ISupportInitializeNotification dsInit && !dsInit.IsInitialized)
            {
                if (!_flags.HasFlag(DataGridViewComboBoxCellFlags.DataSourceInitializedHookedUp))
                {
                    dsInit.Initialized += DataSource_Initialized;
                    _flags |= DataGridViewComboBoxCellFlags.DataSourceInitializedHookedUp;
                }
            }
            else
            {
                cm = (CurrencyManager)dataGridView.BindingContext[DataSource];
                DataManager = cm;
            }
        }

        return cm;
    }

    private protected override string? GetDefaultToolTipText()
    {
        if (string.IsNullOrEmpty(Value?.ToString()?.Trim(' ')) || Value is DBNull)
        {
            return SR.DefaultDataGridViewComboBoxCellTollTipText;
        }

        return null;
    }

    private int GetDropDownButtonHeight(Graphics graphics, DataGridViewCellStyle cellStyle)
    {
        int adjustment = 4;
        if (PaintXPThemes)
        {
            adjustment = PostXPThemesExist ? 8 : 6;
        }

        return MeasureTextHeight(graphics, " ", cellStyle.Font!, int.MaxValue, TextFormatFlags.Default) + adjustment;
    }

    protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
    {
        ArgumentNullException.ThrowIfNull(cellStyle);

        if (DataGridView is null ||
            rowIndex < 0 ||
            OwningColumn is null ||
            !DataGridView.ShowCellErrors ||
            string.IsNullOrEmpty(GetErrorText(rowIndex)))
        {
            return Rectangle.Empty;
        }

        object? value = GetValue(rowIndex);
        object? formattedValue = GetEditedFormattedValue(
            value,
            rowIndex,
            ref cellStyle,
            DataGridViewDataErrorContexts.Formatting);

        ComputeBorderStyleCellStateAndCellBounds(rowIndex, out DataGridViewAdvancedBorderStyle dgvabsEffective, out DataGridViewElementStates cellState, out Rectangle cellBounds);

        Rectangle errorIconBounds = PaintPrivate(
            graphics,
            cellBounds,
            cellBounds,
            rowIndex,
            cellState,
            formattedValue,
            GetErrorText(rowIndex),
            cellStyle,
            dgvabsEffective,
            out Rectangle dropDownButtonRect,   // not used
            DataGridViewPaintParts.ContentForeground,
            computeContentBounds: false,
            computeErrorIconBounds: true,
            computeDropDownButtonRect: false,
            paint: false);

#if DEBUG
        Rectangle errorIconBoundsDebug = PaintPrivate(
            graphics,
            cellBounds,
            cellBounds,
            rowIndex,
            cellState,
            formattedValue,
            GetErrorText(rowIndex),
            cellStyle,
            dgvabsEffective,
            out _, // not used
            DataGridViewPaintParts.ContentForeground,
            computeContentBounds: false,
            computeErrorIconBounds: true,
            computeDropDownButtonRect: false,
            paint: false);
        Debug.Assert(errorIconBoundsDebug.Equals(errorIconBounds));
#endif

        return errorIconBounds;
    }

    protected override object? GetFormattedValue(
        object? value,
        int rowIndex,
        ref DataGridViewCellStyle cellStyle,
        TypeConverter? valueTypeConverter,
        TypeConverter? formattedValueTypeConverter,
        DataGridViewDataErrorContexts context)
    {
        if (valueTypeConverter is null)
        {
            if (ValueMemberProperty is not null)
            {
                valueTypeConverter = ValueMemberProperty.Converter;
            }
            else if (DisplayMemberProperty is not null)
            {
                valueTypeConverter = DisplayMemberProperty.Converter;
            }
        }

        if (value is null || ((ValueType is not null && !ValueType.IsAssignableFrom(value.GetType())) && value != DBNull.Value))
        {
            // Do not raise the DataError event if the value is null and the row is the 'new row'.

            if (value is null)
            {
                return base.GetFormattedValue(
                    value: null,
                    rowIndex,
                    ref cellStyle,
                    valueTypeConverter,
                    formattedValueTypeConverter,
                    context);
            }

            if (DataGridView is not null)
            {
                DataGridViewDataErrorEventArgs dgvdee = new(
                    new FormatException(SR.DataGridViewComboBoxCell_InvalidValue),
                    ColumnIndex,
                    rowIndex,
                    context);
                RaiseDataError(dgvdee);
                if (dgvdee.ThrowException)
                {
                    throw dgvdee.Exception;
                }
            }

            return base.GetFormattedValue(
                value,
                rowIndex,
                ref cellStyle,
                valueTypeConverter,
                formattedValueTypeConverter,
                context);
        }

        string? strValue = value as string;
        if ((DataManager is not null && (ValueMemberProperty is not null || DisplayMemberProperty is not null)) ||
            !string.IsNullOrEmpty(ValueMember) || !string.IsNullOrEmpty(DisplayMember))
        {
            if (!LookupDisplayValue(rowIndex, value, out object? displayValue))
            {
                if (value == DBNull.Value)
                {
                    displayValue = DBNull.Value;
                }
                else if (strValue is not null && string.IsNullOrEmpty(strValue) && DisplayType == typeof(string))
                {
                    displayValue = string.Empty;
                }
                else if (DataGridView is not null)
                {
                    DataGridViewDataErrorEventArgs dgvdee = new(
                        new ArgumentException(SR.DataGridViewComboBoxCell_InvalidValue),
                        ColumnIndex,
                        rowIndex,
                        context);
                    RaiseDataError(dgvdee);
                    if (dgvdee.ThrowException)
                    {
                        throw dgvdee.Exception;
                    }

                    if (OwnsEditingComboBox(rowIndex))
                    {
                        ((IDataGridViewEditingControl)EditingComboBox).EditingControlValueChanged = true;
                        DataGridView.NotifyCurrentCellDirty(true);
                    }
                }
            }

            return base.GetFormattedValue(
                displayValue,
                rowIndex,
                ref cellStyle,
                DisplayTypeConverter,
                formattedValueTypeConverter,
                context);
        }
        else
        {
            if (!Items.Contains(value)
                && value != DBNull.Value
                && (!(value is string) || !string.IsNullOrEmpty(strValue)))
            {
                if (DataGridView is not null)
                {
                    DataGridViewDataErrorEventArgs dgvdee = new(
                        new ArgumentException(SR.DataGridViewComboBoxCell_InvalidValue),
                        ColumnIndex,
                        rowIndex,
                        context);
                    RaiseDataError(dgvdee);
                    if (dgvdee.ThrowException)
                    {
                        throw dgvdee.Exception;
                    }
                }

                if (Items.Count > 0)
                {
                    value = Items[0];
                }
                else
                {
                    value = string.Empty;
                }
            }

            return base.GetFormattedValue(
                value,
                rowIndex,
                ref cellStyle,
                valueTypeConverter,
                formattedValueTypeConverter,
                context);
        }
    }

    internal string? GetItemDisplayText(object item)
    {
        object? displayValue = GetItemDisplayValue(item);
        return displayValue is not null
            ? Convert.ToString(displayValue, CultureInfo.CurrentCulture)
            : string.Empty;
    }

    internal object? GetItemDisplayValue(object item)
    {
        Debug.Assert(item is not null);
        bool displayValueSet = false;
        object? displayValue = null;
        if (DisplayMemberProperty is not null)
        {
            displayValue = DisplayMemberProperty.GetValue(item);
            displayValueSet = true;
        }
        else if (ValueMemberProperty is not null)
        {
            displayValue = ValueMemberProperty.GetValue(item);
            displayValueSet = true;
        }
        else if (!string.IsNullOrEmpty(DisplayMember))
        {
            PropertyDescriptor? propDesc = TypeDescriptor.GetProperties(item).Find(DisplayMember, ignoreCase: true);
            if (propDesc is not null)
            {
                displayValue = propDesc.GetValue(item);
                displayValueSet = true;
            }
        }
        else if (!string.IsNullOrEmpty(ValueMember))
        {
            PropertyDescriptor? propDesc = TypeDescriptor.GetProperties(item).Find(ValueMember, ignoreCase: true);
            if (propDesc is not null)
            {
                displayValue = propDesc.GetValue(item);
                displayValueSet = true;
            }
        }

        if (!displayValueSet)
        {
            displayValue = item;
        }

        return displayValue;
    }

    internal ObjectCollection GetItems(DataGridView? dataGridView)
    {
        if (!Properties.TryGetValue(s_propComboBoxCellItems, out ObjectCollection? items))
        {
            items = new ObjectCollection(this);
            Properties.AddValue(s_propComboBoxCellItems, items);
        }

        if (CreateItemsFromDataSource)
        {
            items.ClearInternal();
            CurrencyManager? dataManager = GetDataManager(dataGridView);
            if (dataManager is not null && dataManager.Count != -1)
            {
                object[] newItems = new object[dataManager.Count];
                for (int i = 0; i < newItems.Length; i++)
                {
                    newItems[i] = dataManager[i]!;
                }

                items.AddRangeInternal(newItems);
            }

            // Do not clear the CreateItemsFromDataSource flag when the data source has not been initialized yet
            if (dataManager is not null || !_flags.HasFlag(DataGridViewComboBoxCellFlags.DataSourceInitializedHookedUp))
            {
                CreateItemsFromDataSource = false;
            }
        }

        return items;
    }

    internal object? GetItemValue(object item)
    {
        bool valueSet = false;
        object? value = null;
        if (ValueMemberProperty is not null)
        {
            value = ValueMemberProperty.GetValue(item);
            valueSet = true;
        }
        else if (DisplayMemberProperty is not null)
        {
            value = DisplayMemberProperty.GetValue(item);
            valueSet = true;
        }
        else if (!string.IsNullOrEmpty(ValueMember))
        {
            PropertyDescriptor? propDesc = TypeDescriptor.GetProperties(item).Find(ValueMember, ignoreCase: true);
            if (propDesc is not null)
            {
                value = propDesc.GetValue(item);
                valueSet = true;
            }
        }

        if (!valueSet && !string.IsNullOrEmpty(DisplayMember))
        {
            PropertyDescriptor? propDesc = TypeDescriptor.GetProperties(item).Find(DisplayMember, ignoreCase: true);
            if (propDesc is not null)
            {
                value = propDesc.GetValue(item);
                valueSet = true;
            }
        }

        if (!valueSet)
        {
            value = item;
        }

        return value;
    }

    protected override Size GetPreferredSize(
        Graphics graphics,
        DataGridViewCellStyle cellStyle,
        int rowIndex,
        Size constraintSize)
    {
        if (DataGridView is null)
        {
            return new Size(-1, -1);
        }

        ArgumentNullException.ThrowIfNull(cellStyle);

        DataGridViewFreeDimension freeDimension = GetFreeDimensionFromConstraint(constraintSize);
        Rectangle borderWidthsRect = StdBorderWidths;
        int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
        int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;
        TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);

        string? formattedValue = GetFormattedValue(rowIndex, ref cellStyle, DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.PreferredSize) as string;

        Size preferredSize =
            MeasureTextSize(graphics, string.IsNullOrEmpty(formattedValue) ? " " : formattedValue, cellStyle.Font!, flags);

        if (freeDimension == DataGridViewFreeDimension.Height)
        {
            preferredSize.Width = 0;
        }
        else if (freeDimension == DataGridViewFreeDimension.Width)
        {
            preferredSize.Height = 0;
        }

        if (freeDimension != DataGridViewFreeDimension.Height)
        {
            preferredSize.Width += SystemInformation.HorizontalScrollBarThumbWidth + 1 + (2 * Margin) + borderAndPaddingWidths;
            if (DataGridView.ShowCellErrors)
            {
                // Making sure that there is enough room for the potential error icon
                preferredSize.Width = Math.Max(
                    preferredSize.Width,
                    borderAndPaddingWidths + SystemInformation.HorizontalScrollBarThumbWidth + 1 + (IconMarginWidth * 2) + s_iconsWidth);
            }
        }

        if (freeDimension != DataGridViewFreeDimension.Width)
        {
            if (FlatStyle is FlatStyle.Flat or FlatStyle.Popup)
            {
                preferredSize.Height += 6;
            }
            else
            {
                preferredSize.Height += 8;
            }

            preferredSize.Height += borderAndPaddingHeights;
            if (DataGridView.ShowCellErrors)
            {
                // Making sure that there is enough room for the potential error icon
                preferredSize.Height = Math.Max(preferredSize.Height, borderAndPaddingHeights + IconMarginHeight * 2 + s_iconsHeight);
            }
        }

        return preferredSize;
    }

    private void InitializeComboBoxText()
    {
        Debug.Assert(EditingComboBox is not null);
        ((IDataGridViewEditingControl)EditingComboBox).EditingControlValueChanged = false;
        int rowIndex = ((IDataGridViewEditingControl)EditingComboBox).EditingControlRowIndex;
        Debug.Assert(rowIndex > -1);
        DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(
            inheritedCellStyle: null,
            rowIndex,
            includeColors: false);
        EditingComboBox.Text = (string?)GetFormattedValue(
            GetValue(rowIndex),
            rowIndex,
            ref dataGridViewCellStyle,
            valueTypeConverter: null,
            formattedValueTypeConverter: null,
            DataGridViewDataErrorContexts.Formatting);
    }

    public override void InitializeEditingControl(
        int rowIndex,
        object? initialFormattedValue,
        DataGridViewCellStyle dataGridViewCellStyle)
    {
        Debug.Assert(DataGridView is not null &&
                     DataGridView.EditingControl is not null);
        Debug.Assert(!ReadOnly);
        base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
        if (DataGridView.EditingControl is ComboBox comboBox)
        {
            // Use the selection BackColor for the editing panel when the cell is selected
            if ((GetInheritedState(rowIndex) & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
            {
                DataGridView.EditingPanel.BackColor = dataGridViewCellStyle.SelectionBackColor;
            }

            // We need the comboBox to be parented by a control which has a handle or else the native ComboBox ends up
            // w/ its parentHwnd pointing to the WinFormsParkingWindow.
            IntPtr h;
            if (comboBox.ParentInternal is not null)
            {
                h = comboBox.ParentInternal.Handle;
            }

            h = comboBox.Handle; // make sure that assigning the DataSource property does not assert.
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.FormattingEnabled = true;
            comboBox.MaxDropDownItems = MaxDropDownItems;
            comboBox.DropDownWidth = DropDownWidth;
            comboBox.DataSource = null;
            comboBox.ValueMember = null;
            comboBox.Items.Clear();

            /* Don't set the position inside the currency manager blindly to 0 because it may be the case that
               the DataGridView and the DataGridViewComboBoxCell share the same DataManager.
               Then setting the position on the DataManager will also set the position on the DataGridView.
               And this causes problems when changing position inside the DataGridView.
            if (this.DataManager is not null && this.DataManager.Count > 0)
            {
                this.DataManager.Position = 0;
            }
            */

            comboBox.DataSource = DataSource;
            comboBox.DisplayMember = DisplayMember;
            comboBox.ValueMember = ValueMember;
            if (HasItems && DataSource is null && Items.Count > 0)
            {
                comboBox.Items.AddRange([.. Items.InnerArray]);
            }

            comboBox.Sorted = Sorted;
            comboBox.FlatStyle = FlatStyle;
            if (AutoComplete)
            {
                comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
                comboBox.AutoCompleteMode = AutoCompleteMode.Append;
            }
            else
            {
                comboBox.AutoCompleteMode = AutoCompleteMode.None;
                comboBox.AutoCompleteSource = AutoCompleteSource.None;
            }

            if (initialFormattedValue is not string initialFormattedValueStr)
            {
                initialFormattedValueStr = string.Empty;
            }

            comboBox.Text = initialFormattedValueStr;

            if (!_flags.HasFlag(DataGridViewComboBoxCellFlags.DropDownHookedUp))
            {
                comboBox.DropDown += ComboBox_DropDown;
                _flags |= DataGridViewComboBoxCellFlags.DropDownHookedUp;
            }

            s_cachedDropDownWidth = -1;

            EditingComboBox = DataGridView.EditingControl as DataGridViewComboBoxEditingControl;
            if (GetHeight(rowIndex) > 21)
            {
                Rectangle rectBottomSection = DataGridView.GetCellDisplayRectangle(ColumnIndex, rowIndex, true);
                rectBottomSection.Y += 21;
                rectBottomSection.Height -= 21;
                DataGridView.Invalidate(rectBottomSection);
            }
        }
    }

    private void InitializeDisplayMemberPropertyDescriptor(string? displayMember)
    {
        if (DataManager is null)
        {
            return;
        }

        if (string.IsNullOrEmpty(displayMember))
        {
            DisplayMemberProperty = null;
            return;
        }

        BindingMemberInfo displayBindingMember = new(displayMember);

        // Make the DataManager point to the sublist inside this.DataSource.
        // We already check inside GetDataManager in DataManager property if these are null.
        DataManager = (CurrencyManager)DataGridView!.BindingContext![DataSource!, displayBindingMember.BindingPath];

        PropertyDescriptorCollection props = DataManager.GetItemProperties();
        PropertyDescriptor? displayMemberProperty = props.Find(displayBindingMember.BindingField, true);
        if (displayMemberProperty is null)
        {
            throw new ArgumentException(string.Format(SR.DataGridViewComboBoxCell_FieldNotFound, displayMember));
        }
        else
        {
            DisplayMemberProperty = displayMemberProperty;
        }
    }

    private void InitializeValueMemberPropertyDescriptor(string? valueMember)
    {
        if (DataManager is null)
        {
            return;
        }

        if (string.IsNullOrEmpty(valueMember))
        {
            ValueMemberProperty = null;
            return;
        }

        BindingMemberInfo valueBindingMember = new(valueMember);

        // make the DataManager point to the sublist inside this.DataSource
        // We already check inside GetDataManager in DataManager property if these are null.
        DataManager = (CurrencyManager)DataGridView!.BindingContext![DataSource!, valueBindingMember.BindingPath];

        PropertyDescriptorCollection props = DataManager.GetItemProperties();
        PropertyDescriptor? valueMemberProperty = props.Find(valueBindingMember.BindingField, true);
        if (valueMemberProperty is null)
        {
            throw new ArgumentException(string.Format(SR.DataGridViewComboBoxCell_FieldNotFound, valueMember));
        }
        else
        {
            ValueMemberProperty = valueMemberProperty;
        }
    }

    /// <summary>
    ///  Find the item in the ComboBox currency manager for the current cell
    ///  This can be horribly inefficient and it uses reflection which makes it expensive
    ///  - ripe for optimization
    /// </summary>
    private object? ItemFromComboBoxDataSource(PropertyDescriptor property, object key)
    {
        ArgumentNullException.ThrowIfNull(key);

        // if (key == this.keyUsedDuringAutoSize)
        // {
        //    return this.valueUsedDuringAutoSize;
        // }

        Debug.Assert(property is not null);
        Debug.Assert(DataManager is not null);
        object? item = null;

        if ((DataManager.List is IBindingList bindingList) && bindingList.SupportsSearching)
        {
            int index = bindingList.Find(property, key);
            if (index != -1)
            {
                item = DataManager.List[index];
            }
        }
        else
        {
            // Otherwise walk across the items looking for the item we want
            for (int i = 0; i < DataManager.List.Count; i++)
            {
                object? itemTmp = DataManager.List[i];
                object? value = property.GetValue(itemTmp);
                if (key.Equals(value))
                {
                    item = itemTmp;
                    break;
                }
            }
        }

        return item;
    }

    private object? ItemFromComboBoxItems(int rowIndex, string field, object key)
    {
        Debug.Assert(!string.IsNullOrEmpty(field));

        object? item = null;
        if (OwnsEditingComboBox(rowIndex))
        {
            // It is likely that the item looked for is the selected item.
            item = EditingComboBox.SelectedItem;
            object? displayValue = null;
            PropertyDescriptor? propDesc = TypeDescriptor.GetProperties(item!).Find(field, ignoreCase: true);
            if (propDesc is not null)
            {
                displayValue = propDesc.GetValue(item);
            }

            if (displayValue is null || !displayValue.Equals(key))
            {
                // No, the selected item is not looked for.
                item = null; // Need to loop through all the items
            }
        }

        if (item is null)
        {
            foreach (object itemCandidate in Items)
            {
                object? displayValue = null;
                PropertyDescriptor? propDesc = TypeDescriptor.GetProperties(itemCandidate).Find(field, ignoreCase: true);
                if (propDesc is not null)
                {
                    displayValue = propDesc.GetValue(itemCandidate);
                }

                if (displayValue is not null && displayValue.Equals(key))
                {
                    // Found the item.
                    item = itemCandidate;
                    break;
                }
            }
        }

        if (item is null)
        {
            // The provided field could be wrong - try to match the key against an actual item
            if (OwnsEditingComboBox(rowIndex))
            {
                // It is likely that the item looked for is the selected item.
                item = EditingComboBox.SelectedItem;
                if (item is null || !item.Equals(key))
                {
                    item = null;
                }
            }

            if (item is null && Items.Contains(key))
            {
                item = key;
            }
        }

        return item;
    }

    public override bool KeyEntersEditMode(KeyEventArgs e)
    {
        if (((char.IsLetterOrDigit((char)e.KeyCode) && !(e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F24)) ||
             (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.Divide) ||
             (e.KeyCode >= Keys.OemSemicolon && e.KeyCode <= Keys.Oem102) ||
             (e.KeyCode == Keys.Space && !e.Shift) ||
             (e.KeyCode == Keys.F4) ||
             ((e.KeyCode == Keys.Down || e.KeyCode == Keys.Up) && e.Alt)) &&
            (!e.Alt || (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)) &&
            !e.Control)
        {
            return true;
        }

        return base.KeyEntersEditMode(e);
    }

    /// <summary>
    ///  Lookup the display text for the given value.
    ///
    ///  We use the value and ValueMember to look up the item in the
    ///  ComboBox datasource. We then use DisplayMember to get the
    ///  text to display.
    /// </summary>
    private bool LookupDisplayValue(int rowIndex, object value, out object? displayValue)
    {
        Debug.Assert(value is not null);
        Debug.Assert(ValueMemberProperty is not null || DisplayMemberProperty is not null ||
                     !string.IsNullOrEmpty(ValueMember) || !string.IsNullOrEmpty(DisplayMember));

        object? item;
        if (DisplayMemberProperty is not null || ValueMemberProperty is not null)
        {
            // Now look up the item in the ComboBox datasource - this can be horribly inefficient
            // and it uses reflection which makes it expensive - ripe for optimization
            item = ItemFromComboBoxDataSource((ValueMemberProperty ?? DisplayMemberProperty)!, value);
        }
        else
        {
            // Find the item in the Items collection based on the provided ValueMember or DisplayMember
            item = ItemFromComboBoxItems(rowIndex, string.IsNullOrEmpty(ValueMember) ? DisplayMember : ValueMember, value);
        }

        if (item is null)
        {
            displayValue = null;
            return false;
        }

        // Now we've got the item for the value - we can get the display text using the DisplayMember

        // DisplayMember & ValueMember may be null in which case we will use the item itself
        displayValue = GetItemDisplayValue(item);
        return true;
    }

    /// <summary>
    ///  Lookup the value for the given display value.
    ///
    ///  We use the display value and DisplayMember to look up the item in the
    ///  ComboBox datasource. We then use ValueMember to get the value.
    /// </summary>
    private bool LookupValue(object? formattedValue, out object? value)
    {
        if (formattedValue is null)
        {
            value = null;
            return true;
        }

        Debug.Assert(DisplayMemberProperty is not null || ValueMemberProperty is not null ||
                     !string.IsNullOrEmpty(DisplayMember) || !string.IsNullOrEmpty(ValueMember));

        object? item;
        if (DisplayMemberProperty is not null || ValueMemberProperty is not null)
        {
            // Now look up the item in the DataGridViewComboBoxCell datasource - this can be horribly inefficient
            // and it uses reflection which makes it expensive - ripe for optimization
            item = ItemFromComboBoxDataSource((DisplayMemberProperty ?? ValueMemberProperty)!, formattedValue);
        }
        else
        {
            // Find the item in the Items collection based on the provided DisplayMember or ValueMember
            item = ItemFromComboBoxItems(RowIndex, string.IsNullOrEmpty(DisplayMember) ? ValueMember : DisplayMember, formattedValue);
        }

        if (item is null)
        {
            value = null;
            return false;
        }

        // Now we've got the item for the value - we can get the value using the ValueMember
        value = GetItemValue(item);
        return true;
    }

    protected override void OnDataGridViewChanged()
    {
        if (DataGridView is not null)
        {
            // Will throw an error if DataGridView is set and a member is invalid
            InitializeDisplayMemberPropertyDescriptor(DisplayMember);
            InitializeValueMemberPropertyDescriptor(ValueMember);
        }

        base.OnDataGridViewChanged();
    }

    protected override void OnEnter(int rowIndex, bool throughMouseClick)
    {
        if (DataGridView is null)
        {
            return;
        }

        if (throughMouseClick && DataGridView.EditMode != DataGridViewEditMode.EditOnEnter)
        {
            _flags |= DataGridViewComboBoxCellFlags.IgnoreNextMouseClick;
        }
    }

    private void OnItemsCollectionChanged()
    {
        if (TemplateComboBoxColumn is not null)
        {
            Debug.Assert(TemplateComboBoxColumn.CellTemplate == this);
            TemplateComboBoxColumn.OnItemsCollectionChanged();
        }

        s_cachedDropDownWidth = -1;
        if (OwnsEditingComboBox(RowIndex))
        {
            InitializeComboBoxText();
        }
        else
        {
            OnCommonChange();
        }
    }

    protected override void OnLeave(int rowIndex, bool throughMouseClick)
    {
        if (DataGridView is null)
        {
            return;
        }

        _flags &= ~DataGridViewComboBoxCellFlags.IgnoreNextMouseClick;
    }

    protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
    {
        if (DataGridView is null)
        {
            return;
        }

        Debug.Assert(e.ColumnIndex == ColumnIndex);
        Point ptCurrentCell = DataGridView.CurrentCellAddress;
        if (ptCurrentCell.X == e.ColumnIndex && ptCurrentCell.Y == e.RowIndex)
        {
            if (_flags.HasFlag(DataGridViewComboBoxCellFlags.IgnoreNextMouseClick))
            {
                _flags &= ~DataGridViewComboBoxCellFlags.IgnoreNextMouseClick;
            }
            else if ((EditingComboBox is null || !EditingComboBox.DroppedDown) &&
                     DataGridView.EditMode != DataGridViewEditMode.EditProgrammatically &&
                     DataGridView.BeginEdit(selectAll: true))
            {
                if (EditingComboBox is not null && DisplayStyle != DataGridViewComboBoxDisplayStyle.Nothing)
                {
                    CheckDropDownList(e.X, e.Y, e.RowIndex);
                }
            }
        }
    }

    protected override void OnMouseEnter(int rowIndex)
    {
        if (DataGridView is null)
        {
            return;
        }

        if (DisplayStyle == DataGridViewComboBoxDisplayStyle.ComboBox && FlatStyle == FlatStyle.Popup)
        {
            DataGridView.InvalidateCell(ColumnIndex, rowIndex);
        }

        base.OnMouseEnter(rowIndex);
    }

    protected override void OnMouseLeave(int rowIndex)
    {
        if (DataGridView is null)
        {
            return;
        }

        if (s_mouseInDropDownButtonBounds)
        {
            s_mouseInDropDownButtonBounds = false;
            if (ColumnIndex >= 0 &&
                rowIndex >= 0 &&
                (FlatStyle == FlatStyle.Standard || FlatStyle == FlatStyle.System) &&
                DataGridView.ApplyVisualStylesToInnerCells)
            {
                DataGridView.InvalidateCell(ColumnIndex, rowIndex);
            }
        }

        if (DisplayStyle == DataGridViewComboBoxDisplayStyle.ComboBox && FlatStyle == FlatStyle.Popup)
        {
            DataGridView.InvalidateCell(ColumnIndex, rowIndex);
        }

        base.OnMouseEnter(rowIndex);
    }

    protected override void OnMouseMove(DataGridViewCellMouseEventArgs e)
    {
        if (DataGridView is null)
        {
            return;
        }

        if ((FlatStyle == FlatStyle.Standard || FlatStyle == FlatStyle.System) && DataGridView.ApplyVisualStylesToInnerCells)
        {
            if (OwningColumn is null)
            {
                return;
            }

            int rowIndex = e.RowIndex;
            DataGridViewCellStyle cellStyle = GetInheritedStyle(inheritedCellStyle: null, rowIndex, includeColors: false);

            // get the border style
            bool singleVerticalBorderAdded = !DataGridView.RowHeadersVisible && DataGridView.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single;
            bool singleHorizontalBorderAdded = !DataGridView.ColumnHeadersVisible && DataGridView.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single;
            bool isFirstDisplayedRow = rowIndex == DataGridView.FirstDisplayedScrollingRowIndex;
            bool isFirstDisplayedColumn = OwningColumn.Index == DataGridView.FirstDisplayedColumnIndex;
            bool isFirstDisplayedScrollingColumn = OwningColumn.Index == DataGridView.FirstDisplayedScrollingColumnIndex;
            DataGridViewAdvancedBorderStyle dgvabsEffective, dgvabsPlaceholder;
            dgvabsPlaceholder = new DataGridViewAdvancedBorderStyle();
            dgvabsEffective = AdjustCellBorderStyle(
                DataGridView.AdvancedCellBorderStyle,
                dgvabsPlaceholder,
                singleVerticalBorderAdded,
                singleHorizontalBorderAdded,
                isFirstDisplayedRow,
                isFirstDisplayedColumn);

            Rectangle cellBounds = DataGridView.GetCellDisplayRectangle(OwningColumn.Index, rowIndex, cutOverflow: false);
            if (isFirstDisplayedScrollingColumn)
            {
                cellBounds.X -= DataGridView.FirstDisplayedScrollingColumnHiddenWidth;
                cellBounds.Width += DataGridView.FirstDisplayedScrollingColumnHiddenWidth;
            }

            DataGridViewElementStates rowState = DataGridView.Rows.GetRowState(rowIndex);
            DataGridViewElementStates cellState = CellStateFromColumnRowStates(rowState);
            cellState |= State;

            Rectangle dropDownButtonRect;
            using (var screen = GdiCache.GetScreenDCGraphics())
            {
                PaintPrivate(
                    screen,
                    cellBounds,
                    cellBounds,
                    rowIndex,
                    cellState,
                    formattedValue: null,   // dropDownButtonRect is independent of formattedValue
                    errorText: null,        // dropDownButtonRect is independent of errorText
                    cellStyle,
                    dgvabsEffective,
                    out dropDownButtonRect,
                    DataGridViewPaintParts.ContentForeground,
                    computeContentBounds: false,
                    computeErrorIconBounds: false,
                    computeDropDownButtonRect: true,
                    paint: false);
            }

            bool newMouseInDropDownButtonBounds = dropDownButtonRect.Contains(DataGridView.PointToClient(Control.MousePosition));
            if (newMouseInDropDownButtonBounds != s_mouseInDropDownButtonBounds)
            {
                s_mouseInDropDownButtonBounds = newMouseInDropDownButtonBounds;
                DataGridView.InvalidateCell(e.ColumnIndex, rowIndex);
            }
        }

        base.OnMouseMove(e);
    }

    [MemberNotNullWhen(true, nameof(EditingComboBox))]
    private bool OwnsEditingComboBox(int rowIndex)
    {
        return rowIndex != -1 && EditingComboBox is not null && rowIndex == ((IDataGridViewEditingControl)EditingComboBox).EditingControlRowIndex;
    }

    protected override void Paint(
        Graphics graphics,
        Rectangle clipBounds,
        Rectangle cellBounds,
        int rowIndex,
        DataGridViewElementStates elementState,
        object? value,
        object? formattedValue,
        string? errorText,
        DataGridViewCellStyle cellStyle,
        DataGridViewAdvancedBorderStyle advancedBorderStyle,
        DataGridViewPaintParts paintParts)
    {
        ArgumentNullException.ThrowIfNull(cellStyle);

        PaintPrivate(graphics,
            clipBounds,
            cellBounds,
            rowIndex,
            elementState,
            formattedValue,
            errorText,
            cellStyle,
            advancedBorderStyle,
            out _,     // not used
            paintParts,
            computeContentBounds: false,
            computeErrorIconBounds: false,
            computeDropDownButtonRect: false,
            paint: true);
    }

    // PaintPrivate is used in four places that need to duplicate the paint code:
    // 1. DataGridViewCell::Paint method
    // 2. DataGridViewCell::GetContentBounds
    // 3. DataGridViewCell::GetErrorIconBounds
    // 4. DataGridViewCell::OnMouseMove - to compute the dropDownButtonRect
    //
    // if computeContentBounds is true then PaintPrivate returns the contentBounds
    // else if computeErrorIconBounds is true then PaintPrivate returns the errorIconBounds
    // else it returns Rectangle.Empty;
    //
    // PaintPrivate uses the computeDropDownButtonRect to determine if it should compute the dropDownButtonRect
    private Rectangle PaintPrivate(
        Graphics g,
        Rectangle clipBounds,
        Rectangle cellBounds,
        int rowIndex,
        DataGridViewElementStates elementState,
        object? formattedValue,
        string? errorText,
        DataGridViewCellStyle cellStyle,
        DataGridViewAdvancedBorderStyle advancedBorderStyle,
        out Rectangle dropDownButtonRect,
        DataGridViewPaintParts paintParts,
        bool computeContentBounds,
        bool computeErrorIconBounds,
        bool computeDropDownButtonRect,
        bool paint)
    {
        // Parameter checking.
        // One bit and one bit only should be turned on
        Debug.Assert(paint || computeContentBounds || computeErrorIconBounds || computeDropDownButtonRect);
        Debug.Assert(!paint || !computeContentBounds || !computeErrorIconBounds || !computeDropDownButtonRect);
        Debug.Assert(!paint || !computeContentBounds || !computeDropDownButtonRect || !computeErrorIconBounds);
        Debug.Assert(!computeContentBounds || !computeErrorIconBounds || !paint || !computeDropDownButtonRect);
        Debug.Assert(!computeContentBounds || !computeErrorIconBounds || !computeDropDownButtonRect || !paint);
        Debug.Assert(!computeErrorIconBounds || !paint || !computeContentBounds || !computeDropDownButtonRect);
        Debug.Assert(!computeErrorIconBounds || !paint || !computeDropDownButtonRect || !computeContentBounds);
        Debug.Assert(cellStyle is not null);

        Rectangle resultBounds = Rectangle.Empty;
        dropDownButtonRect = Rectangle.Empty;

        if (DataGridView is null)
        {
            return resultBounds;
        }

        bool paintFlat = FlatStyle is FlatStyle.Flat or FlatStyle.Popup;
        bool paintPopup = FlatStyle == FlatStyle.Popup &&
                          DataGridView.MouseEnteredCellAddress.Y == rowIndex &&
                          DataGridView.MouseEnteredCellAddress.X == ColumnIndex;

        bool paintXPThemes = !paintFlat && DataGridView.ApplyVisualStylesToInnerCells;
        bool paintPostXPThemes = paintXPThemes && PostXPThemesExist;

        ComboBoxState comboBoxState = ComboBoxState.Normal;
        if (DataGridView.MouseEnteredCellAddress.Y == rowIndex &&
            DataGridView.MouseEnteredCellAddress.X == ColumnIndex &&
            s_mouseInDropDownButtonBounds)
        {
            comboBoxState = ComboBoxState.Hot;
        }

        if (paint && PaintBorder(paintParts))
        {
            PaintBorder(g, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
        }

        Rectangle borderWidths = BorderWidths(advancedBorderStyle);
        Rectangle valBounds = cellBounds;
        valBounds.Offset(borderWidths.X, borderWidths.Y);
        valBounds.Width -= borderWidths.Right;
        valBounds.Height -= borderWidths.Bottom;

        Point ptCurrentCell = DataGridView.CurrentCellAddress;
        bool cellCurrent = ptCurrentCell.X == ColumnIndex && ptCurrentCell.Y == rowIndex;
        bool cellEdited = cellCurrent && DataGridView.EditingControl is not null;
        bool cellSelected = (elementState & DataGridViewElementStates.Selected) != 0;
        bool drawComboBox = DisplayStyle == DataGridViewComboBoxDisplayStyle.ComboBox &&
                            ((DisplayStyleForCurrentCellOnly && cellCurrent) || !DisplayStyleForCurrentCellOnly);
        bool drawDropDownButton = DisplayStyle != DataGridViewComboBoxDisplayStyle.Nothing &&
                            ((DisplayStyleForCurrentCellOnly && cellCurrent) || !DisplayStyleForCurrentCellOnly);

        Color brushColor = PaintSelectionBackground(paintParts) && cellSelected && !cellEdited
            ? cellStyle.SelectionBackColor
            : cellStyle.BackColor;
        using var brush = paint && !brushColor.HasTransparency() ? brushColor.GetCachedSolidBrushScope() : default;

        if (paint && PaintBackground(paintParts) && !brushColor.HasTransparency() && valBounds.Width > 0 && valBounds.Height > 0)
        {
            PaintPadding(g, valBounds, cellStyle, brush!, DataGridView.RightToLeftInternal);
        }

        if (cellStyle.Padding != Padding.Empty)
        {
            if (DataGridView.RightToLeftInternal)
            {
                valBounds.Offset(cellStyle.Padding.Right, cellStyle.Padding.Top);
            }
            else
            {
                valBounds.Offset(cellStyle.Padding.Left, cellStyle.Padding.Top);
            }

            valBounds.Width -= cellStyle.Padding.Horizontal;
            valBounds.Height -= cellStyle.Padding.Vertical;
        }

        if (paint && valBounds.Width > 0 && valBounds.Height > 0)
        {
            if (paintXPThemes && drawComboBox)
            {
                if (paintPostXPThemes && PaintBackground(paintParts) && !brushColor.HasTransparency())
                {
                    g.FillRectangle(brush!, valBounds.Left, valBounds.Top, valBounds.Width, valBounds.Height);
                }

                if (PaintContentBackground(paintParts))
                {
                    if (paintPostXPThemes)
                    {
                        DataGridViewComboBoxCellRenderer.DrawBorder(g, valBounds);
                    }
                    else
                    {
                        DataGridViewComboBoxCellRenderer.DrawTextBox(g, valBounds, comboBoxState);
                    }
                }

                if (!paintPostXPThemes &&
                    PaintBackground(paintParts) && !brushColor.HasTransparency() && valBounds.Width > 2 && valBounds.Height > 2)
                {
                    g.FillRectangle(brush!, valBounds.Left + 1, valBounds.Top + 1, valBounds.Width - 2, valBounds.Height - 2);
                }
            }
            else if (PaintBackground(paintParts) && !brushColor.HasTransparency())
            {
                if (paintPostXPThemes && drawDropDownButton && !drawComboBox)
                {
                    g.DrawRectangle(SystemPens.ControlLightLight, new Rectangle(valBounds.X, valBounds.Y, valBounds.Width - 1, valBounds.Height - 1));
                }
                else
                {
                    g.FillRectangle(brush!, valBounds.Left, valBounds.Top, valBounds.Width, valBounds.Height);
                }
            }
        }

        int dropWidth = Math.Min(SystemInformation.HorizontalScrollBarThumbWidth, valBounds.Width - 2 * Margin - 1);

        if (!cellEdited)
        {
            int dropHeight;
            if (paintXPThemes || paintFlat)
            {
                dropHeight = Math.Min(GetDropDownButtonHeight(g, cellStyle), paintPostXPThemes ? valBounds.Height : valBounds.Height - 2);
            }
            else
            {
                dropHeight = Math.Min(GetDropDownButtonHeight(g, cellStyle), valBounds.Height - 4);
            }

            if (dropWidth > 0 && dropHeight > 0)
            {
                Rectangle dropRect;
                if (paintXPThemes || paintFlat)
                {
                    if (paintPostXPThemes)
                    {
                        dropRect = new Rectangle(
                            DataGridView.RightToLeftInternal ? valBounds.Left : valBounds.Right - dropWidth,
                            valBounds.Top,
                            dropWidth,
                            dropHeight);
                    }
                    else
                    {
                        dropRect = new Rectangle(
                            DataGridView.RightToLeftInternal ? valBounds.Left + 1 : valBounds.Right - dropWidth - 1,
                            valBounds.Top + 1,
                            dropWidth,
                            dropHeight);
                    }
                }
                else
                {
                    dropRect = new Rectangle(
                        DataGridView.RightToLeftInternal ? valBounds.Left + 2 : valBounds.Right - dropWidth - 2,
                        valBounds.Top + 2,
                        dropWidth,
                        dropHeight);
                }

                if (paintPostXPThemes && drawDropDownButton && !drawComboBox)
                {
                    dropDownButtonRect = valBounds;
                }
                else
                {
                    dropDownButtonRect = dropRect;
                }

                if (paint && PaintContentBackground(paintParts))
                {
                    if (drawDropDownButton)
                    {
                        if (paintFlat)
                        {
                            g.FillRectangle(SystemBrushes.Control, dropRect);
                        }
                        else if (paintXPThemes)
                        {
                            if (paintPostXPThemes)
                            {
                                if (drawComboBox)
                                {
                                    DataGridViewComboBoxCellRenderer.DrawDropDownButton(g, dropRect, comboBoxState, DataGridView.RightToLeftInternal);
                                }
                                else
                                {
                                    DataGridViewComboBoxCellRenderer.DrawReadOnlyButton(g, valBounds, comboBoxState);
                                    DataGridViewComboBoxCellRenderer.DrawDropDownButton(g, dropRect, ComboBoxState.Normal);
                                }

                                if (SystemInformation.HighContrast)
                                {
                                    // In the case of ComboBox style, background is not filled in. In the case of
                                    // DrawReadOnlyButton uses theming API to render CP_READONLY COMBOBOX part that
                                    // renders the background, this API does not have "selected" state, thus always
                                    // uses BackColor.
                                    brushColor = cellStyle.BackColor;
                                }
                            }
                            else
                            {
                                DataGridViewComboBoxCellRenderer.DrawDropDownButton(g, dropRect, comboBoxState);
                            }
                        }
                        else
                        {
                            g.FillRectangle(SystemBrushes.Control, dropRect);
                        }
                    }

                    if (!paintFlat && !paintXPThemes && (drawComboBox || drawDropDownButton))
                    {
                        Pen pen = SystemInformation.HighContrast ? SystemPens.ControlLight : SystemPens.Control;

                        // top + left
                        if (drawDropDownButton)
                        {
                            g.DrawLine(pen, dropRect.X, dropRect.Y,
                                    dropRect.X + dropRect.Width - 1, dropRect.Y);
                            g.DrawLine(pen, dropRect.X, dropRect.Y,
                                    dropRect.X, dropRect.Y + dropRect.Height - 1);
                        }

                        // the bounds around the ComboBox control
                        if (drawComboBox)
                        {
                            g.DrawLine(pen, valBounds.X, valBounds.Y + valBounds.Height - 1,
                                    valBounds.X + valBounds.Width - 1, valBounds.Y + valBounds.Height - 1);
                            g.DrawLine(pen, valBounds.X + valBounds.Width - 1, valBounds.Y,
                                    valBounds.X + valBounds.Width - 1, valBounds.Y + valBounds.Height - 1);
                        }

                        pen = SystemPens.ControlDarkDark;

                        // bottom + right
                        if (drawDropDownButton)
                        {
                            g.DrawLine(pen, dropRect.X, dropRect.Y + dropRect.Height - 1,
                                    dropRect.X + dropRect.Width - 1, dropRect.Y + dropRect.Height - 1);
                            g.DrawLine(pen, dropRect.X + dropRect.Width - 1, dropRect.Y,
                                    dropRect.X + dropRect.Width - 1, dropRect.Y + dropRect.Height - 1);
                        }

                        // the bounds around the ComboBox control
                        if (drawComboBox)
                        {
                            g.DrawLine(pen, valBounds.X, valBounds.Y,
                                    valBounds.X + valBounds.Width - 2, valBounds.Y);
                            g.DrawLine(pen, valBounds.X, valBounds.Y,
                                    valBounds.X, valBounds.Y + valBounds.Height - 1);
                        }

                        // Top + Left inset
                        pen = SystemPens.ControlLightLight;
                        if (drawDropDownButton)
                        {
                            g.DrawLine(pen, dropRect.X + 1, dropRect.Y + 1,
                                    dropRect.X + dropRect.Width - 2, dropRect.Y + 1);
                            g.DrawLine(pen, dropRect.X + 1, dropRect.Y + 1,
                                    dropRect.X + 1, dropRect.Y + dropRect.Height - 2);
                        }

                        // Bottom + Right inset
                        pen = SystemPens.ControlDark;
                        if (drawDropDownButton)
                        {
                            g.DrawLine(pen, dropRect.X + 1, dropRect.Y + dropRect.Height - 2,
                                    dropRect.X + dropRect.Width - 2, dropRect.Y + dropRect.Height - 2);
                            g.DrawLine(pen, dropRect.X + dropRect.Width - 2, dropRect.Y + 1,
                                    dropRect.X + dropRect.Width - 2, dropRect.Y + dropRect.Height - 2);
                        }
                    }

                    if (dropWidth >= 5 && dropHeight >= 3 && drawDropDownButton)
                    {
                        if (paintFlat)
                        {
                            Point middle = new(dropRect.Left + dropRect.Width / 2, dropRect.Top + dropRect.Height / 2);
                            // if the width is odd - favor pushing it over one pixel right.
                            middle.X += (dropRect.Width % 2);
                            // if the height is odd - favor pushing it over one pixel down.
                            middle.Y += (dropRect.Height % 2);

                            g.FillPolygon(
                                SystemBrushes.ControlText,
                                (ReadOnlySpan<Point>)
                                [
                                    new(middle.X - s_offset2X, middle.Y - 1),
                                    new(middle.X + s_offset2X + 1, middle.Y - 1),
                                    new(middle.X, middle.Y + s_offset2Y)
                                ]);
                        }
                        else if (!paintXPThemes)
                        {
                            // XPThemes already painted the drop down button

                            // the down arrow looks better when it's fatten up by a pixel
                            dropRect.X--;
                            dropRect.Width++;

                            Point middle = new(dropRect.Left + (dropRect.Width - 1) / 2,
                                    dropRect.Top + (dropRect.Height + s_nonXPTriangleHeight) / 2);
                            // if the width is event - favor pushing it over one pixel right.
                            middle.X += ((dropRect.Width + 1) % 2);
                            // if the height is odd - favor pushing it over one pixel down.
                            middle.Y += (dropRect.Height % 2);
                            Point pt1 = new(middle.X - (s_nonXPTriangleWidth - 1) / 2, middle.Y - s_nonXPTriangleHeight);
                            Point pt2 = new(middle.X + (s_nonXPTriangleWidth - 1) / 2, middle.Y - s_nonXPTriangleHeight);
                            g.FillPolygon(SystemBrushes.ControlText, (ReadOnlySpan<Point>)[pt1, pt2, middle]);
                            // quirk in GDI+ : if we don't draw the line below then the top right most pixel of the DropDown triangle will not paint
                            // Would think that g.FillPolygon would have painted that...
                            g.DrawLine(SystemPens.ControlText, pt1.X, pt1.Y, pt2.X, pt2.Y);

                            // slim down the drop rect
                            dropRect.X++;
                            dropRect.Width--;
                        }
                    }

                    if (paintPopup && drawComboBox)
                    {
                        // draw a dark border around the dropdown rect if we are in popup mode
                        dropRect.Y--;
                        dropRect.Height++;
                        g.DrawRectangle(SystemPens.ControlDark, dropRect);
                    }
                }
            }
        }

        Rectangle errorBounds = valBounds;
        Rectangle textBounds = Rectangle.Inflate(valBounds, -2, -2);

        if (paintPostXPThemes)
        {
            if (!DataGridView.RightToLeftInternal)
            {
                textBounds.X--;
            }

            textBounds.Width++;
        }

        if (drawDropDownButton)
        {
            if (paintXPThemes || paintFlat)
            {
                errorBounds.Width -= dropWidth;
                textBounds.Width -= dropWidth;
                if (DataGridView.RightToLeftInternal)
                {
                    errorBounds.X += dropWidth;
                    textBounds.X += dropWidth;
                }
            }
            else
            {
                errorBounds.Width -= dropWidth + 1;
                textBounds.Width -= dropWidth + 1;
                if (DataGridView.RightToLeftInternal)
                {
                    errorBounds.X += dropWidth + 1;
                    textBounds.X += dropWidth + 1;
                }
            }
        }

        if (textBounds.Width > 1 && textBounds.Height > 1)
        {
            if (cellCurrent &&
                !cellEdited &&
                PaintFocus(paintParts) &&
                DataGridView.ShowFocusCues &&
                DataGridView.Focused &&
                paint)
            {
                // Draw focus rectangle
                if (paintFlat)
                {
                    Rectangle focusBounds = textBounds;
                    if (!DataGridView.RightToLeftInternal)
                    {
                        focusBounds.X--;
                    }

                    focusBounds.Width++;
                    focusBounds.Y--;
                    focusBounds.Height += 2;
                    ControlPaint.DrawFocusRectangle(g, focusBounds, Color.Empty, cellStyle.ForeColor);
                }
                else if (paintPostXPThemes)
                {
                    Rectangle focusBounds = textBounds;
                    focusBounds.X++;
                    focusBounds.Width -= 2;
                    focusBounds.Y++;
                    focusBounds.Height -= 2;
                    if (focusBounds.Width > 0 && focusBounds.Height > 0)
                    {
                        ControlPaint.DrawFocusRectangle(g, focusBounds, Color.Empty, cellStyle.ForeColor);
                    }
                }
                else
                {
                    ControlPaint.DrawFocusRectangle(g, textBounds, Color.Empty, cellStyle.ForeColor);
                }
            }

            if (paintPopup)
            {
                valBounds.Width--;
                valBounds.Height--;
                if (!cellEdited && paint && PaintContentBackground(paintParts) && drawComboBox)
                {
                    g.DrawRectangle(SystemPens.ControlDark, valBounds);
                }
            }

            if (formattedValue is string formattedString)
            {
                // Font independent margins
                int verticalTextMarginTop = cellStyle.WrapMode == DataGridViewTriState.True ? VerticalTextMarginTopWithWrapping : VerticalTextMarginTopWithoutWrapping;
                if (DataGridView.RightToLeftInternal)
                {
                    textBounds.Offset(HorizontalTextMarginLeft, verticalTextMarginTop);
                    textBounds.Width += 2 - HorizontalTextMarginLeft;
                }
                else
                {
                    textBounds.Offset(HorizontalTextMarginLeft - 1, verticalTextMarginTop);
                    textBounds.Width += 1 - HorizontalTextMarginLeft;
                }

                textBounds.Height -= verticalTextMarginTop;

                if (textBounds.Width > 0 && textBounds.Height > 0)
                {
                    TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
                    if (!cellEdited && paint)
                    {
                        if (PaintContentForeground(paintParts))
                        {
                            if ((flags & TextFormatFlags.SingleLine) != 0)
                            {
                                flags |= TextFormatFlags.EndEllipsis;
                            }

                            Color textColor;
                            if (paintPostXPThemes && (drawDropDownButton || drawComboBox) && !SystemInformation.HighContrast)
                            {
                                textColor = DataGridViewComboBoxCellRenderer.VisualStyleRenderer.GetColor(ColorProperty.TextColor);
                            }
                            else
                            {
                                textColor = cellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor;
                            }

                            TextRenderer.DrawText(
                                g,
                                formattedString,
                                cellStyle.Font,
                                textBounds,
                                textColor,
                                flags);
                        }
                    }
                    else if (computeContentBounds)
                    {
                        resultBounds = DataGridViewUtilities.GetTextBounds(textBounds, formattedString, flags, cellStyle);
                    }
                }
            }

            if (DataGridView.ShowCellErrors && paint && PaintErrorIcon(paintParts))
            {
                PaintErrorIcon(g, cellStyle, rowIndex, cellBounds, errorBounds, errorText);
                if (cellEdited)
                {
                    return Rectangle.Empty;
                }
            }
        }

        if (computeErrorIconBounds)
        {
            if (!string.IsNullOrEmpty(errorText))
            {
                resultBounds = ComputeErrorIconBounds(errorBounds);
            }
            else
            {
                resultBounds = Rectangle.Empty;
            }
        }

        return resultBounds;
    }

    public override object? ParseFormattedValue(
        object? formattedValue,
        DataGridViewCellStyle cellStyle,
        TypeConverter? formattedValueTypeConverter,
        TypeConverter? valueTypeConverter)
    {
        if (valueTypeConverter is null)
        {
            if (ValueMemberProperty is not null)
            {
                valueTypeConverter = ValueMemberProperty.Converter;
            }
            else if (DisplayMemberProperty is not null)
            {
                valueTypeConverter = DisplayMemberProperty.Converter;
            }
        }

        // Find the item given its display value
        if ((DataManager is not null &&
            (DisplayMemberProperty is not null || ValueMemberProperty is not null)) ||
            !string.IsNullOrEmpty(DisplayMember) || !string.IsNullOrEmpty(ValueMember))
        {
            object? value = ParseFormattedValueInternal(
                DisplayType,
                formattedValue,
                cellStyle,
                formattedValueTypeConverter,
                DisplayTypeConverter);
            object? originalValue = value;
            if (!LookupValue(originalValue, out value))
            {
                if (originalValue == DBNull.Value)
                {
                    value = DBNull.Value;
                }
                else
                {
                    throw new FormatException(string.Format(CultureInfo.CurrentCulture, SR.Formatter_CantConvert, value, DisplayType));
                }
            }

            return value;
        }
        else
        {
            return ParseFormattedValueInternal(
                ValueType,
                formattedValue,
                cellStyle,
                formattedValueTypeConverter,
                valueTypeConverter);
        }
    }

    internal override void ReleaseUiaProvider()
    {
        EditingComboBox?.ReleaseUiaProvider(HWND.Null);

        base.ReleaseUiaProvider();
    }

    /// <summary>
    ///  Gets the row Index and column Index of the cell.
    /// </summary>
    public override string ToString() => $"DataGridViewComboBoxCell {{ ColumnIndex={ColumnIndex}, RowIndex={RowIndex} }}";

    private void UnwireDataSource()
    {
        if (DataSource is IComponent component)
        {
            component.Disposed -= DataSource_Disposed;
        }

        if (DataSource is ISupportInitializeNotification dsInit && _flags.HasFlag(DataGridViewComboBoxCellFlags.DataSourceInitializedHookedUp))
        {
            // If we previously hooked the data source's ISupportInitializeNotification
            // Initialized event, then unhook it now (we don't always hook this event,
            // only if we needed to because the data source was previously uninitialized)
            dsInit.Initialized -= DataSource_Initialized;
            _flags &= ~DataGridViewComboBoxCellFlags.DataSourceInitializedHookedUp;
        }
    }

    private void WireDataSource(object? dataSource)
    {
        // If the source is a component, then hook the Disposed event,
        // so we know when the component is deleted from the form
        if (dataSource is IComponent component)
        {
            component.Disposed += DataSource_Disposed;
        }
    }
}

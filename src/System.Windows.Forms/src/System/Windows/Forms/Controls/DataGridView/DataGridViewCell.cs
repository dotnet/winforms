// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  Identifies a cell in the dataGridView.
/// </summary>
[TypeConverter(typeof(DataGridViewCellConverter))]
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.Interfaces)]
public abstract partial class DataGridViewCell : DataGridViewElement, ICloneable, IDisposable, IKeyboardToolTip
{
    private const TextFormatFlags TextFormatSupportedFlags = TextFormatFlags.SingleLine
        | /*TextFormatFlags.NoFullWidthCharacterBreak |*/ TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix;

    private const int ContrastThreshold = 1000;
    private const int HighContrastThreshold = 2000;
    private const int MaxToolTipLength = 288;
    private const int MaxToolTipCutOff = 256;
    private const byte FlagAreaNotSet = 0x00;
    private const byte FlagDataArea = 0x01;
    private const byte FlagErrorArea = 0x02;
    private protected const byte IconMarginWidth = 4;      // 4 pixels of margin on the left and right of icons
    private protected const byte IconMarginHeight = 4;     // 4 pixels of margin on the top and bottom of icons
    private const byte IconsWidth = 12;           // all icons are 12 pixels wide - make sure that it stays that way
    private const byte IconsHeight = 11;          // all icons are 11 pixels tall - make sure that it stays that way

    private static bool s_isScalingInitialized;
    private protected static byte s_iconsWidth = IconsWidth;
    private protected static byte s_iconsHeight = IconsHeight;

    private protected static readonly int s_propCellValue = PropertyStore.CreateKey();
    private static readonly int s_propCellContextMenuStrip = PropertyStore.CreateKey();
    private static readonly int s_propCellErrorText = PropertyStore.CreateKey();
    private static readonly int s_propCellStyle = PropertyStore.CreateKey();
    private static readonly int s_propCellValueType = PropertyStore.CreateKey();
    private static readonly int s_propCellTag = PropertyStore.CreateKey();
    private static readonly int s_propCellToolTipText = PropertyStore.CreateKey();
    private static readonly int s_propCellAccessibilityObject = PropertyStore.CreateKey();

    private static Bitmap? s_errorBmp;

    /// <summary>
    /// Contains non-empty neighboring cells around the current cell.
    /// Used in <see cref="IKeyboardToolTip.GetNeighboringToolsRectangles"/> method.
    /// </summary>
    private readonly List<Rectangle> _nonEmptyNeighbors;

    private static readonly Type s_stringType = typeof(string);        // cache the string type for performance

    private byte _flags;
    private bool _useDefaultToolTipText;  // The tooltip text of this cell has not been set by a customer yet.

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataGridViewCell"/> class.
    /// </summary>
    protected DataGridViewCell()
        : base()
    {
        if (!s_isScalingInitialized)
        {
            s_iconsWidth = (byte)ScaleHelper.ScaleToInitialSystemDpi(IconsWidth);
            s_iconsHeight = (byte)ScaleHelper.ScaleToInitialSystemDpi(IconsHeight);
            s_isScalingInitialized = true;
        }

        Properties = new PropertyStore();
        State = DataGridViewElementStates.None;
        _nonEmptyNeighbors = [];
        _useDefaultToolTipText = true;
    }

    // NOTE: currently this finalizer is unneeded (empty). See https://github.com/dotnet/winforms/issues/6858.
    // All classes that are not need to be finalized must be checked in DataGridViewElement() constructor. Consider to modify it if needed.
    ~DataGridViewCell() => Dispose(disposing: false);

    [Browsable(false)]
    public AccessibleObject AccessibilityObject
    {
        get
        {
            AccessibleObject? result = (AccessibleObject?)Properties.GetObject(s_propCellAccessibilityObject);
            if (result is null)
            {
                result = CreateAccessibilityInstance();
                Properties.SetObject(s_propCellAccessibilityObject, result);
            }

            return result;
        }
    }

    /// <summary>
    ///  Gets or sets the Index of a column in the <see cref="DataGridView"/> control.
    /// </summary>
    public int ColumnIndex => OwningColumn?.Index ?? -1;

    [Browsable(false)]
    public Rectangle ContentBounds => GetContentBounds(RowIndex);

    [DefaultValue(null)]
    public virtual ContextMenuStrip? ContextMenuStrip
    {
        get => GetContextMenuStrip(RowIndex);
        set => ContextMenuStripInternal = value;
    }

    private ContextMenuStrip? ContextMenuStripInternal
    {
        get => (ContextMenuStrip?)Properties.GetObject(s_propCellContextMenuStrip);
        set
        {
            ContextMenuStrip? oldValue = (ContextMenuStrip?)Properties.GetObject(s_propCellContextMenuStrip);
            if (oldValue != value)
            {
                EventHandler disposedHandler = new(DetachContextMenuStrip);
                if (oldValue is not null)
                {
                    oldValue.Disposed -= disposedHandler;
                }

                Properties.SetObject(s_propCellContextMenuStrip, value);
                if (value is not null)
                {
                    value.Disposed += disposedHandler;
                }

                DataGridView?.OnCellContextMenuStripChanged(this);
            }
        }
    }

    private byte CurrentMouseLocation
    {
        get => (byte)(_flags & (FlagDataArea | FlagErrorArea));
        set
        {
            _flags = (byte)(_flags & ~(FlagDataArea | FlagErrorArea));
            _flags |= value;
        }
    }

    [Browsable(false)]
    public virtual object? DefaultNewRowValue => null;

    [Browsable(false)]
    public virtual bool Displayed
    {
        get
        {
            if (DataGridView is null)
            {
                // No detached element is displayed.
                return false;
            }

            if (RowIndex >= 0 && ColumnIndex >= 0)
            {
                Debug.Assert(DataGridView.Rows.GetRowState(RowIndex) == DataGridView.Rows.SharedRow(RowIndex).State);
                return OwningColumn!.Displayed && OwningRow!.Displayed;
            }

            return false;
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public object? EditedFormattedValue
    {
        get
        {
            if (DataGridView is null)
            {
                return null;
            }

            Debug.Assert(RowIndex >= -1);
            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(inheritedCellStyle: null, RowIndex, includeColors: false);
            return GetEditedFormattedValue(GetValue(RowIndex), RowIndex, ref dataGridViewCellStyle, DataGridViewDataErrorContexts.Formatting);
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.Interfaces)]
    public virtual Type? EditType => typeof(DataGridViewTextBoxEditingControl);

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Rectangle ErrorIconBounds => GetErrorIconBounds(RowIndex);

    [AllowNull]
    [Browsable(false)]
    public string ErrorText
    {
        get => GetErrorText(RowIndex);
        set => ErrorTextInternal = value;
    }

    [AllowNull]
    private string ErrorTextInternal
    {
        get => (string?)Properties.GetObject(s_propCellErrorText) ?? string.Empty;
        set
        {
            string errorText = ErrorTextInternal;
            if (!string.IsNullOrEmpty(value) || Properties.ContainsObject(s_propCellErrorText))
            {
                Properties.SetObject(s_propCellErrorText, value);
            }

            if (DataGridView is not null && !errorText.Equals(ErrorTextInternal))
            {
                DataGridView.OnCellErrorTextChanged(this);
            }
        }
    }

    [Browsable(false)]
    public object? FormattedValue
    {
        get
        {
            if (DataGridView is null)
            {
                return null;
            }

            Debug.Assert(RowIndex >= -1);
            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(inheritedCellStyle: null, RowIndex, includeColors: false);
            return GetFormattedValue(RowIndex, ref dataGridViewCellStyle, DataGridViewDataErrorContexts.Formatting);
        }
    }

    [Browsable(false)]
    public virtual Type? FormattedValueType => ValueType;

    private TypeConverter? FormattedValueTypeConverter
    {
        get
        {
            TypeConverter? formattedValueTypeConverter = null;
            if (FormattedValueType is not null)
            {
                if (DataGridView is not null)
                {
                    formattedValueTypeConverter = DataGridView.GetCachedTypeConverter(FormattedValueType);
                }
                else
                {
                    formattedValueTypeConverter = TypeDescriptor.GetConverter(FormattedValueType);
                }
            }

            return formattedValueTypeConverter;
        }
    }

    [Browsable(false)]
    public virtual bool Frozen
    {
        get
        {
            if (DataGridView is not null && RowIndex >= 0 && ColumnIndex >= 0)
            {
                Debug.Assert(DataGridView.Rows.GetRowState(RowIndex) == DataGridView.Rows.SharedRow(RowIndex).State);
                return OwningColumn!.Frozen && OwningRow!.Frozen;
            }
            else if (OwningRow is not null && (OwningRow.DataGridView is null || RowIndex >= 0))
            {
                return OwningRow.Frozen;
            }

            return false;
        }
    }

    private bool HasErrorText
    {
        get => Properties.ContainsObjectThatIsNotNull(s_propCellErrorText);
    }

    [Browsable(false)]
    public bool HasStyle => Properties.ContainsObjectThatIsNotNull(s_propCellStyle);

    internal bool HasToolTipText => Properties.ContainsObjectThatIsNotNull(s_propCellToolTipText);

    internal bool HasValue => Properties.ContainsObjectThatIsNotNull(s_propCellValue);

    private protected virtual bool HasValueType
    {
        get => Properties.ContainsObjectThatIsNotNull(s_propCellValueType);
    }

    #region IKeyboardToolTip implementation
    bool IKeyboardToolTip.CanShowToolTipsNow() => Visible && DataGridView is not null;

    Rectangle IKeyboardToolTip.GetNativeScreenRectangle() => AccessibilityObject.Bounds;

    /// <summary>
    ///  The method looks for 8 cells around the current cell to find the optimal tooltip position in
    ///  <see cref="ToolTip.GetOptimalToolTipPosition"/> method. The optimal tooltip position is the position
    ///  outside <see cref="DataGridView"/> or on top of an empty cell. This is done so that tooltips do not
    ///  overlap the text of other cells whenever possible.
    /// </summary>
    /// <returns>
    ///  Non-empty neighboring cells around the current cell.
    /// </returns>
    IList<Rectangle> IKeyboardToolTip.GetNeighboringToolsRectangles()
    {
        _nonEmptyNeighbors.Clear();

        if (DataGridView is null)
        {
            return _nonEmptyNeighbors;
        }

        for (int i = RowIndex - 1; i <= RowIndex + 1; i++)
        {
            if (i < 0 || i >= DataGridView.Rows.Count - 1)
            {
                continue;
            }

            for (int j = ColumnIndex - 1; j <= ColumnIndex + 1; j++)
            {
                if (j < 0 || j > DataGridView.Columns.Count - 1
                    || string.IsNullOrEmpty(DataGridView.Rows[i].Cells[j].Value?.ToString()))
                {
                    continue;
                }

                _nonEmptyNeighbors.Add(DataGridView.Rows[i].Cells[j].AccessibilityObject.Bounds);
            }
        }

        return _nonEmptyNeighbors;
    }

    bool IKeyboardToolTip.IsHoveredWithMouse() => false;

    bool IKeyboardToolTip.HasRtlModeEnabled() => DataGridView is not null && DataGridView.RightToLeft == RightToLeft.Yes;

    bool IKeyboardToolTip.AllowsToolTip() => true;

    IWin32Window? IKeyboardToolTip.GetOwnerWindow() => DataGridView;

    void IKeyboardToolTip.OnHooked(ToolTip toolTip) => OnKeyboardToolTipHook(toolTip);

    internal virtual void OnKeyboardToolTipHook(ToolTip toolTip) { }

    void IKeyboardToolTip.OnUnhooked(ToolTip toolTip) => OnKeyboardToolTipUnhook(toolTip);

    internal virtual void OnKeyboardToolTipUnhook(ToolTip toolTip) { }

    string? IKeyboardToolTip.GetCaptionForTool(ToolTip toolTip)
    {
        if (DataGridView is not null && DataGridView.ShowCellErrors && !string.IsNullOrEmpty(ErrorText))
        {
            return ErrorText;
        }

        return ToolTipText;
    }

    bool IKeyboardToolTip.ShowsOwnToolTip() => true;

    bool IKeyboardToolTip.IsBeingTabbedTo() => IsBeingTabbedTo();

    internal virtual bool IsBeingTabbedTo() => Control.AreCommonNavigationalKeysDown();

    bool IKeyboardToolTip.AllowsChildrenToShowToolTips() => true;
    #endregion

    [Browsable(false)]
    public DataGridViewElementStates InheritedState => GetInheritedState(RowIndex);

    [Browsable(false)]
    public DataGridViewCellStyle InheritedStyle
    {
        get
        {
            // this.RowIndex could be -1 if:
            // - the developer makes a mistake & calls dataGridView1.Rows.SharedRow(y).Cells(x).InheritedStyle.
            // - the InheritedStyle of a ColumnHeaderCell is accessed.
            return GetInheritedStyleInternal(RowIndex);
        }
    }

    internal bool IsAccessibilityObjectCreated => Properties.GetObject(s_propCellAccessibilityObject) is AccessibleObject;

    /// <summary>
    ///  Indicates whether or not the parent grid view for this element has an accessible object associated with it.
    /// </summary>
    internal bool IsParentAccessibilityObjectCreated => DataGridView is not null && DataGridView.IsAccessibilityObjectCreated;

    [Browsable(false)]
    public bool IsInEditMode
    {
        get
        {
            if (DataGridView is null)
            {
                return false;
            }

            if (RowIndex == -1)
            {
                throw new InvalidOperationException(SR.DataGridView_InvalidOperationOnSharedCell);
            }

            Point ptCurrentCell = DataGridView.CurrentCellAddress;
            return ptCurrentCell.X != -1 &&
                   ptCurrentCell.X == ColumnIndex &&
                   ptCurrentCell.Y == RowIndex &&
                   DataGridView.IsCurrentCellInEditMode;
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public DataGridViewColumn? OwningColumn { get; internal set; }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public DataGridViewRow? OwningRow { get; internal set; }

    [Browsable(false)]
    public Size PreferredSize => GetPreferredSize(RowIndex);

    internal PropertyStore Properties { get; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual bool ReadOnly
    {
        get
        {
            if ((State & DataGridViewElementStates.ReadOnly) != 0)
            {
                return true;
            }

            if (OwningRow is not null && (OwningRow.DataGridView is null || RowIndex >= 0) && OwningRow.ReadOnly)
            {
                return true;
            }

            if (DataGridView is not null && RowIndex >= 0 && ColumnIndex >= 0)
            {
                Debug.Assert(DataGridView.Rows.GetRowState(RowIndex) == DataGridView.Rows.SharedRow(RowIndex).State);
                return OwningColumn!.ReadOnly;
            }

            return false;
        }
        set
        {
            if (DataGridView is not null)
            {
                if (RowIndex == -1)
                {
                    throw new InvalidOperationException(SR.DataGridView_InvalidOperationOnSharedCell);
                }

                Debug.Assert(ColumnIndex >= 0);
                // When the whole grid is read-only, we ignore the request.
                if (value != ReadOnly && !DataGridView.ReadOnly)
                {
                    DataGridView.OnDataGridViewElementStateChanging(this, -1, DataGridViewElementStates.ReadOnly);
                    DataGridView.SetReadOnlyCellCore(ColumnIndex, RowIndex, value); // this may trigger a call to set_ReadOnlyInternal
                }
            }
            else
            {
                if (OwningRow is null)
                {
                    if (value != ReadOnly)
                    {
                        // We do not allow the read-only flag of a cell to be changed before it is added to a row.
                        throw new InvalidOperationException(SR.DataGridViewCell_CannotSetReadOnlyState);
                    }
                }
                else
                {
                    OwningRow.SetReadOnlyCellCore(this, value);
                }
            }
        }
    }

    internal bool ReadOnlyInternal
    {
        set
        {
            if (value)
            {
                State |= DataGridViewElementStates.ReadOnly;
            }
            else
            {
                State &= ~DataGridViewElementStates.ReadOnly;
            }

            DataGridView?.OnDataGridViewElementStateChanged(this, -1, DataGridViewElementStates.ReadOnly);
        }
    }

    [Browsable(false)]
    public virtual bool Resizable
    {
        get
        {
            if (OwningRow is not null && (OwningRow.DataGridView is null || RowIndex >= 0) && OwningRow.Resizable == DataGridViewTriState.True)
            {
                return true;
            }

            if (DataGridView is not null && RowIndex >= 0 && ColumnIndex >= 0)
            {
                Debug.Assert(DataGridView.Rows.GetRowState(RowIndex) == DataGridView.Rows.SharedRow(RowIndex).State);
                return OwningColumn!.Resizable == DataGridViewTriState.True;
            }

            return false;
        }
    }

    /// <summary>
    ///  Gets or sets the index of a row in the <see cref="DataGridView"/> control.
    /// </summary>
    [Browsable(false)]
    public int RowIndex => OwningRow?.Index ?? -1;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual bool Selected
    {
        get
        {
            if ((State & DataGridViewElementStates.Selected) != 0)
            {
                return true;
            }

            if (OwningRow is not null && (OwningRow.DataGridView is null || RowIndex >= 0) && OwningRow.Selected)
            {
                return true;
            }

            if (DataGridView is not null && RowIndex >= 0 && ColumnIndex >= 0)
            {
                Debug.Assert(DataGridView.Rows.GetRowState(RowIndex) == DataGridView.Rows.SharedRow(RowIndex).State);
                return OwningColumn!.Selected;
            }

            return false;
        }
        set
        {
            if (DataGridView is not null)
            {
                if (RowIndex == -1)
                {
                    throw new InvalidOperationException(SR.DataGridView_InvalidOperationOnSharedCell);
                }

                Debug.Assert(ColumnIndex >= 0);
                DataGridView.SetSelectedCellCoreInternal(ColumnIndex, RowIndex, value); // this may trigger a call to set_SelectedInternal
            }
            else if (value)
            {
                // We do not allow the selection of a cell to be set before the row gets added to the dataGridView.
                throw new InvalidOperationException(SR.DataGridViewCell_CannotSetSelectedState);
            }
        }
    }

    internal bool SelectedInternal
    {
        set
        {
            Debug.Assert(value != Selected);
            if (value)
            {
                State |= DataGridViewElementStates.Selected;
            }
            else
            {
                State &= ~DataGridViewElementStates.Selected;
            }

            DataGridView?.OnDataGridViewElementStateChanged(this, -1, DataGridViewElementStates.Selected);
        }
    }

    [Browsable(false)]
    public Size Size => GetSize(RowIndex);

    private protected Rectangle StdBorderWidths
    {
        get
        {
            if (DataGridView is not null)
            {
                DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder = new(), dgvabsEffective;
                dgvabsEffective = AdjustCellBorderStyle(
                    DataGridView.AdvancedCellBorderStyle,
                    dataGridViewAdvancedBorderStylePlaceholder,
                    singleVerticalBorderAdded: false,
                    singleHorizontalBorderAdded: false,
                    isFirstDisplayedColumn: false,
                    isFirstDisplayedRow: false);
                return BorderWidths(dgvabsEffective);
            }
            else
            {
                return Rectangle.Empty;
            }
        }
    }

    [Browsable(true)]
    [AllowNull]
    public DataGridViewCellStyle Style
    {
        get
        {
            DataGridViewCellStyle? dataGridViewCellStyle = (DataGridViewCellStyle?)Properties.GetObject(s_propCellStyle);
            if (dataGridViewCellStyle is null)
            {
                dataGridViewCellStyle = new DataGridViewCellStyle();
                dataGridViewCellStyle.AddScope(DataGridView, DataGridViewCellStyleScopes.Cell);
                Properties.SetObject(s_propCellStyle, dataGridViewCellStyle);
            }

            return dataGridViewCellStyle;
        }
        set
        {
            DataGridViewCellStyle? dataGridViewCellStyle = null;
            if (HasStyle)
            {
                dataGridViewCellStyle = Style;
                dataGridViewCellStyle.RemoveScope(DataGridViewCellStyleScopes.Cell);
            }

            if (value is not null || Properties.ContainsObject(s_propCellStyle))
            {
                value?.AddScope(DataGridView, DataGridViewCellStyleScopes.Cell);

                Properties.SetObject(s_propCellStyle, value);
            }

            if (((dataGridViewCellStyle is not null && value is null) ||
                (dataGridViewCellStyle is null && value is not null) ||
                (dataGridViewCellStyle is not null && value is not null && !dataGridViewCellStyle.Equals(Style))) && DataGridView is not null)
            {
                DataGridView.OnCellStyleChanged(this);
            }
        }
    }

    [SRCategory(nameof(SR.CatData))]
    [Localizable(false)]
    [Bindable(true)]
    [SRDescription(nameof(SR.ControlTagDescr))]
    [DefaultValue(null)]
    [TypeConverter(typeof(StringConverter))]
    public object? Tag
    {
        get => Properties.GetObject(s_propCellTag);
        set
        {
            if (value is not null || Properties.ContainsObject(s_propCellTag))
            {
                Properties.SetObject(s_propCellTag, value);
            }
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public string ToolTipText
    {
        get => GetToolTipText(RowIndex) ?? string.Empty;
        set
        {
            ToolTipTextInternal = value;
            _useDefaultToolTipText = false;
        }
    }

    [AllowNull]
    private string ToolTipTextInternal
    {
        get => (string?)Properties.GetObject(s_propCellToolTipText) ?? string.Empty;
        set
        {
            string toolTipText = ToolTipTextInternal;
            if (!string.IsNullOrEmpty(value) || Properties.ContainsObject(s_propCellToolTipText))
            {
                Properties.SetObject(s_propCellToolTipText, value);
            }

            if (DataGridView is not null && !toolTipText.Equals(ToolTipTextInternal))
            {
                DataGridView.OnCellToolTipTextChanged(this);
            }
        }
    }

    [Browsable(false)]
    public object? Value
    {
        get
        {
            Debug.Assert(RowIndex >= -1);
            return GetValue(RowIndex);
        }
        set
        {
            Debug.Assert(RowIndex >= -1);
            SetValue(RowIndex, value);
        }
    }

    [Browsable(false)]
    public virtual Type? ValueType
    {
        get
        {
            Type? cellValueType = (Type?)Properties.GetObject(s_propCellValueType);
            if (cellValueType is null && OwningColumn is not null)
            {
                cellValueType = OwningColumn.ValueType;
            }

            return cellValueType;
        }
        set
        {
            if (value is not null || Properties.ContainsObject(s_propCellValueType))
            {
                Properties.SetObject(s_propCellValueType, value);
            }
        }
    }

    private TypeConverter? ValueTypeConverter
    {
        get
        {
            TypeConverter? valueTypeConverter = null;
            if (OwningColumn is not null)
            {
                valueTypeConverter = OwningColumn.BoundColumnConverter;
            }

            if (valueTypeConverter is null && ValueType is not null)
            {
                if (DataGridView is not null)
                {
                    valueTypeConverter = DataGridView.GetCachedTypeConverter(ValueType);
                }
                else
                {
                    valueTypeConverter = TypeDescriptor.GetConverter(ValueType);
                }
            }

            return valueTypeConverter;
        }
    }

    [Browsable(false)]
    public virtual bool Visible
    {
        get
        {
            if (DataGridView is not null && RowIndex >= 0 && ColumnIndex >= 0)
            {
                Debug.Assert(DataGridView.Rows.GetRowState(RowIndex) == DataGridView.Rows.SharedRow(RowIndex).State);
                return OwningColumn!.Visible && OwningRow!.Visible;
            }
            else if (OwningRow is not null && (OwningRow.DataGridView is null || RowIndex >= 0))
            {
                return OwningRow.Visible;
            }

            return false;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual DataGridViewAdvancedBorderStyle AdjustCellBorderStyle(
        DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStyleInput,
        DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder,
        bool singleVerticalBorderAdded,
        bool singleHorizontalBorderAdded,
        bool isFirstDisplayedColumn,
        bool isFirstDisplayedRow)
    {
        ArgumentNullException.ThrowIfNull(dataGridViewAdvancedBorderStyleInput);
        ArgumentNullException.ThrowIfNull(dataGridViewAdvancedBorderStylePlaceholder);

        switch (dataGridViewAdvancedBorderStyleInput.All)
        {
            case DataGridViewAdvancedCellBorderStyle.Single:
                if (DataGridView is not null && DataGridView.RightToLeftInternal)
                {
                    dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.Single;
                    dataGridViewAdvancedBorderStylePlaceholder.RightInternal = (isFirstDisplayedColumn && singleVerticalBorderAdded) ? DataGridViewAdvancedCellBorderStyle.Single : DataGridViewAdvancedCellBorderStyle.None;
                }
                else
                {
                    dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = (isFirstDisplayedColumn && singleVerticalBorderAdded) ? DataGridViewAdvancedCellBorderStyle.Single : DataGridViewAdvancedCellBorderStyle.None;
                    dataGridViewAdvancedBorderStylePlaceholder.RightInternal = DataGridViewAdvancedCellBorderStyle.Single;
                }

                dataGridViewAdvancedBorderStylePlaceholder.TopInternal = (isFirstDisplayedRow && singleHorizontalBorderAdded) ? DataGridViewAdvancedCellBorderStyle.Single : DataGridViewAdvancedCellBorderStyle.None;
                dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = DataGridViewAdvancedCellBorderStyle.Single;
                return dataGridViewAdvancedBorderStylePlaceholder;

            case DataGridViewAdvancedCellBorderStyle.NotSet:
                if (DataGridView is not null && DataGridView.AdvancedCellBorderStyle == dataGridViewAdvancedBorderStyleInput)
                {
                    switch (DataGridView.CellBorderStyle)
                    {
                        case DataGridViewCellBorderStyle.SingleVertical:
                            if (DataGridView.RightToLeftInternal)
                            {
                                dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.Single;
                                dataGridViewAdvancedBorderStylePlaceholder.RightInternal = (isFirstDisplayedColumn && singleVerticalBorderAdded) ? DataGridViewAdvancedCellBorderStyle.Single : DataGridViewAdvancedCellBorderStyle.None;
                            }
                            else
                            {
                                dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = (isFirstDisplayedColumn && singleVerticalBorderAdded) ? DataGridViewAdvancedCellBorderStyle.Single : DataGridViewAdvancedCellBorderStyle.None;
                                dataGridViewAdvancedBorderStylePlaceholder.RightInternal = DataGridViewAdvancedCellBorderStyle.Single;
                            }

                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = DataGridViewAdvancedCellBorderStyle.None;
                            dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = DataGridViewAdvancedCellBorderStyle.None;
                            return dataGridViewAdvancedBorderStylePlaceholder;

                        case DataGridViewCellBorderStyle.SingleHorizontal:
                            dataGridViewAdvancedBorderStylePlaceholder.LeftInternal = DataGridViewAdvancedCellBorderStyle.None;
                            dataGridViewAdvancedBorderStylePlaceholder.RightInternal = DataGridViewAdvancedCellBorderStyle.None;
                            dataGridViewAdvancedBorderStylePlaceholder.TopInternal = (isFirstDisplayedRow && singleHorizontalBorderAdded) ? DataGridViewAdvancedCellBorderStyle.Single : DataGridViewAdvancedCellBorderStyle.None;
                            dataGridViewAdvancedBorderStylePlaceholder.BottomInternal = DataGridViewAdvancedCellBorderStyle.Single;
                            return dataGridViewAdvancedBorderStylePlaceholder;
                    }
                }

                break;
        }

        return dataGridViewAdvancedBorderStyleInput;
    }

    protected virtual Rectangle BorderWidths(DataGridViewAdvancedBorderStyle advancedBorderStyle)
    {
        ArgumentNullException.ThrowIfNull(advancedBorderStyle);

        Rectangle rect = new Rectangle
        {
            X = (advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.None) ? 0 : 1
        };
        if (advancedBorderStyle.Left is DataGridViewAdvancedCellBorderStyle.OutsetDouble or DataGridViewAdvancedCellBorderStyle.InsetDouble)
        {
            rect.X++;
        }

        rect.Y = (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None) ? 0 : 1;
        if (advancedBorderStyle.Top is DataGridViewAdvancedCellBorderStyle.OutsetDouble or DataGridViewAdvancedCellBorderStyle.InsetDouble)
        {
            rect.Y++;
        }

        rect.Width = (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None) ? 0 : 1;
        if (advancedBorderStyle.Right is DataGridViewAdvancedCellBorderStyle.OutsetDouble or DataGridViewAdvancedCellBorderStyle.InsetDouble)
        {
            rect.Width++;
        }

        rect.Height = (advancedBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.None) ? 0 : 1;
        if (advancedBorderStyle.Bottom is DataGridViewAdvancedCellBorderStyle.OutsetDouble or DataGridViewAdvancedCellBorderStyle.InsetDouble)
        {
            rect.Height++;
        }

        if (OwningColumn is not null)
        {
            if (DataGridView is not null && DataGridView.RightToLeftInternal)
            {
                rect.X += OwningColumn.DividerWidth;
            }
            else
            {
                rect.Width += OwningColumn.DividerWidth;
            }
        }

        if (OwningRow is not null)
        {
            rect.Height += OwningRow.DividerHeight;
        }

        return rect;
    }

    // Called when the row that owns the editing control gets unshared.
    // Too late in the product cycle to make this a public method.
    internal virtual void CacheEditingControl()
    {
    }

    internal DataGridViewElementStates CellStateFromColumnRowStates(DataGridViewElementStates rowState)
    {
        Debug.Assert(DataGridView is not null);
        Debug.Assert(ColumnIndex >= 0);
        DataGridViewElementStates orFlags = DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.Selected;
        DataGridViewElementStates andFlags = DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Visible;
        DataGridViewElementStates cellState = (OwningColumn!.State & orFlags);
        cellState |= (rowState & orFlags);
        cellState |= ((OwningColumn.State & andFlags) & (rowState & andFlags));
        return cellState;
    }

    protected virtual bool ClickUnsharesRow(DataGridViewCellEventArgs e) => false;

    internal bool ClickUnsharesRowInternal(DataGridViewCellEventArgs e) => ClickUnsharesRow(e);

    internal void CloneInternal(DataGridViewCell dataGridViewCell)
    {
        if (HasValueType)
        {
            dataGridViewCell.ValueType = ValueType;
        }

        if (HasStyle)
        {
            dataGridViewCell.Style = new DataGridViewCellStyle(Style);
        }

        if (HasErrorText)
        {
            dataGridViewCell.ErrorText = ErrorTextInternal;
        }

        if (HasToolTipText)
        {
            dataGridViewCell.ToolTipText = ToolTipTextInternal;
        }

        if (ContextMenuStripInternal is not null)
        {
            dataGridViewCell.ContextMenuStrip = ContextMenuStripInternal.Clone();
        }

        dataGridViewCell.State = State & ~DataGridViewElementStates.Selected;
        dataGridViewCell.Tag = Tag;

        if (DataGridView is not null && DataGridView.KeyboardToolTip is not null)
        {
            KeyboardToolTipStateMachine.Instance.Hook(dataGridViewCell, DataGridView.KeyboardToolTip);
        }
    }

    public virtual object Clone()
    {
        DataGridViewCell dataGridViewCell = (DataGridViewCell)Activator.CreateInstance(GetType())!;
        CloneInternal(dataGridViewCell);
        return dataGridViewCell;
    }

    internal static int ColorDistance(Color color1, Color color2)
    {
        int deltaR = color1.R - color2.R;
        int deltaG = color1.G - color2.G;
        int deltaB = color1.B - color2.B;
        return deltaR * deltaR + deltaG * deltaG + deltaB * deltaB;
    }

    internal void ComputeBorderStyleCellStateAndCellBounds(int rowIndex,
        out DataGridViewAdvancedBorderStyle dgvabsEffective,
        out DataGridViewElementStates cellState,
        out Rectangle cellBounds)
    {
        Debug.Assert(DataGridView is not null);
        bool singleVerticalBorderAdded = !DataGridView.RowHeadersVisible && DataGridView.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single;
        bool singleHorizontalBorderAdded = !DataGridView.ColumnHeadersVisible && DataGridView.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single;
        DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder = new();

        if (rowIndex > -1 && OwningColumn is not null)
        {
            // Inner cell case
            dgvabsEffective = AdjustCellBorderStyle(DataGridView.AdvancedCellBorderStyle,
                dataGridViewAdvancedBorderStylePlaceholder,
                singleVerticalBorderAdded,
                singleHorizontalBorderAdded,
                isFirstDisplayedColumn: ColumnIndex == DataGridView.FirstDisplayedColumnIndex,
                isFirstDisplayedRow: rowIndex == DataGridView.FirstDisplayedRowIndex);
            DataGridViewElementStates rowState = DataGridView.Rows.GetRowState(rowIndex);
            cellState = CellStateFromColumnRowStates(rowState);
            cellState |= State;
        }
        else if (OwningColumn is not null)
        {
            // Column header cell case
            Debug.Assert(rowIndex == -1);
            Debug.Assert(this is DataGridViewColumnHeaderCell, "if the row index == -1 and we have an owning column this should be a column header cell");
            DataGridViewColumn? dataGridViewColumn = DataGridView.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None);
            bool isLastVisibleColumn = (dataGridViewColumn is not null && dataGridViewColumn.Index == ColumnIndex);
            dgvabsEffective = DataGridView.AdjustColumnHeaderBorderStyle(DataGridView.AdvancedColumnHeadersBorderStyle,
                dataGridViewAdvancedBorderStylePlaceholder,
                ColumnIndex == DataGridView.FirstDisplayedColumnIndex,
                isLastVisibleColumn);
            cellState = OwningColumn.State | State;
        }
        else if (OwningRow is not null)
        {
            // Row header cell case
            Debug.Assert(this is DataGridViewRowHeaderCell);
            dgvabsEffective = OwningRow.AdjustRowHeaderBorderStyle(DataGridView.AdvancedRowHeadersBorderStyle,
                dataGridViewAdvancedBorderStylePlaceholder,
                singleVerticalBorderAdded,
                singleHorizontalBorderAdded,
                isFirstDisplayedRow: rowIndex == DataGridView.FirstDisplayedRowIndex,
                isLastVisibleRow: rowIndex == DataGridView.Rows.GetLastRow(DataGridViewElementStates.Visible));
            cellState = OwningRow.GetState(rowIndex) | State;
        }
        else
        {
            Debug.Assert(OwningColumn is null);
            Debug.Assert(OwningRow is null);
            Debug.Assert(rowIndex == -1);
            // TopLeft header cell case
            dgvabsEffective = DataGridView.AdjustedTopLeftHeaderBorderStyle;
            cellState = State;
        }

        cellBounds = new Rectangle(new Point(0, 0), GetSize(rowIndex));
    }

    internal Rectangle ComputeErrorIconBounds(Rectangle cellValueBounds)
    {
        if (cellValueBounds.Width >= IconMarginWidth * 2 + s_iconsWidth &&
            cellValueBounds.Height >= IconMarginHeight * 2 + s_iconsHeight)
        {
            Rectangle bmpRect = new(DataGridView!.RightToLeftInternal ?
                                  cellValueBounds.Left + IconMarginWidth :
                                  cellValueBounds.Right - IconMarginWidth - s_iconsWidth,
                                  cellValueBounds.Y + (cellValueBounds.Height - s_iconsHeight) / 2,
                                  s_iconsWidth,
                                  s_iconsHeight);
            return bmpRect;
        }
        else
        {
            return Rectangle.Empty;
        }
    }

    protected virtual bool ContentClickUnsharesRow(DataGridViewCellEventArgs e) => false;

    internal bool ContentClickUnsharesRowInternal(DataGridViewCellEventArgs e) => ContentClickUnsharesRow(e);

    protected virtual bool ContentDoubleClickUnsharesRow(DataGridViewCellEventArgs e) => false;

    internal bool ContentDoubleClickUnsharesRowInternal(DataGridViewCellEventArgs e) => ContentDoubleClickUnsharesRow(e);

    protected virtual AccessibleObject CreateAccessibilityInstance() => new DataGridViewCellAccessibleObject(this);

    private void DetachContextMenuStrip(object? sender, EventArgs e) => ContextMenuStripInternal = null;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual void DetachEditingControl()
    {
        DataGridView? dataGridView = DataGridView;
        if (dataGridView?.EditingControl is null)
        {
            throw new InvalidOperationException();
        }

        if (dataGridView.EditingControl.ParentInternal is not null)
        {
            if (dataGridView.EditingControl.ContainsFocus)
            {
                if (dataGridView.GetContainerControl() is ContainerControl cc && (dataGridView.EditingControl == cc.ActiveControl || dataGridView.EditingControl.Contains(cc.ActiveControl)))
                {
                    dataGridView.Focus();
                }
                else
                {
                    // We don't want the grid to get the keyboard focus
                    // when the editing control gets parented to the parking window,
                    // because some other window is in the middle of receiving the focus.
                    PInvoke.SetFocus(default);
                }
            }

            Debug.Assert(dataGridView.EditingControl.ParentInternal == dataGridView.EditingPanel);
            Debug.Assert(dataGridView.EditingPanel.Controls.Contains(dataGridView.EditingControl));
            dataGridView.EditingPanel.Controls.Remove(dataGridView.EditingControl);
            Debug.Assert(dataGridView.EditingControl.ParentInternal is null);

            if (AccessibleRestructuringNeeded)
            {
                dataGridView.EditingControlAccessibleObject!.SetParent(null);
                AccessibilityObject.SetDetachableChild(null);

                AccessibilityObject.RaiseStructureChangedEvent(StructureChangeType.StructureChangeType_ChildRemoved, dataGridView.EditingControlAccessibleObject.RuntimeId);
            }
        }

        if (dataGridView.EditingPanel.ParentInternal is not null)
        {
            Debug.Assert(dataGridView.EditingPanel.ParentInternal == dataGridView);
            Debug.Assert(dataGridView.Controls.Contains(dataGridView.EditingPanel));
            ((DataGridView.DataGridViewControlCollection)dataGridView.Controls).RemoveInternal(dataGridView.EditingPanel);
            Debug.Assert(dataGridView.EditingPanel.ParentInternal is null);
        }

        Debug.Assert(dataGridView.EditingControl.ParentInternal is null);
        Debug.Assert(dataGridView.EditingPanel.ParentInternal is null);
        Debug.Assert(dataGridView.EditingPanel.Controls.Count == 0);

        // Since the tooltip is removed when the editing control is shown,
        // the CurrentMouseLocation is reset to DATAGRIDVIEWCELL_flagAreaNotSet
        // so that the tooltip appears again on mousemove after the editing.
        CurrentMouseLocation = FlagAreaNotSet;
    }

    /// <summary>
    /// Gets the value indicating whether DataGridView editing control should be processed in accessible
    /// hierarchy restructuring. This check is necessary to not restructure the accessible hierarchy for
    /// custom editing controls and for derived classes as inherited accessibility may differ or
    /// may be not inherited at all.
    /// </summary>
    /// <returns>True if accessible hierarchy should be manually recreated for the cell and editing control, otherwise False.</returns>
    private bool AccessibleRestructuringNeeded
    {
        get
        {
            // Get the type of the editing control and cache it to not call expensive GetType() method repeatedly.
            Debug.Assert(DataGridView?.EditingControl is not null);
            Type editingControlType = DataGridView.EditingControl.GetType();

            return
                IsAccessibilityObjectCreated &&
                ((editingControlType == typeof(DataGridViewComboBoxEditingControl) && !editingControlType.IsSubclassOf(typeof(DataGridViewComboBoxEditingControl))) ||
                (editingControlType == typeof(DataGridViewTextBoxEditingControl) && !editingControlType.IsSubclassOf(typeof(DataGridViewTextBoxEditingControl))));
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            ContextMenuStrip? contextMenuStrip = ContextMenuStripInternal;
            if (contextMenuStrip is not null)
            {
                contextMenuStrip.Disposed -= new EventHandler(DetachContextMenuStrip);
            }
        }

        // If you are adding releasing unmanaged resources code here (disposing == false), you need to remove this class type
        // (and all of its subclasses) from check in DataGridViewElement() constructor and DataGridViewElement_Subclasses_SuppressFinalizeCall test!
        // Also consider to modify ~DataGridViewCell() description.
    }

    protected virtual bool DoubleClickUnsharesRow(DataGridViewCellEventArgs e) => false;

    internal bool DoubleClickUnsharesRowInternal(DataGridViewCellEventArgs e) => DoubleClickUnsharesRow(e);

    protected virtual bool EnterUnsharesRow(int rowIndex, bool throughMouseClick) => false;

    internal bool EnterUnsharesRowInternal(int rowIndex, bool throughMouseClick) =>
        EnterUnsharesRow(rowIndex, throughMouseClick);

    internal static void FormatPlainText(
        string? s,
        bool csv,
        TextWriter output,
        ref bool escapeApplied)
    {
        if (s is null)
        {
            return;
        }

        int cb = s.Length;
        for (int i = 0; i < cb; i++)
        {
            char ch = s[i];
            switch (ch)
            {
                case '"':
                    if (csv)
                    {
                        output.Write("\"\"");
                        escapeApplied = true;
                    }
                    else
                    {
                        output.Write('"');
                    }

                    break;
                case ',':
                    if (csv)
                    {
                        escapeApplied = true;
                    }

                    output.Write(',');
                    break;
                case '\t':
                    if (!csv)
                    {
                        output.Write(' ');
                    }
                    else
                    {
                        output.Write('\t');
                    }

                    break;
                default:
                    output.Write(ch);
                    break;
            }
        }

        if (escapeApplied)
        {
            output.Write('"'); // terminating double-quote.
            // the caller is responsible for inserting the opening double-quote.
        }
    }

    // Code taken from ASP.NET file xsp\System\Web\httpserverutility.cs
    internal static void FormatPlainTextAsHtml(string? s, TextWriter output)
    {
        if (s is null)
        {
            return;
        }

        int cb = s.Length;
        char prevCh = '\0';

        for (int i = 0; i < cb; i++)
        {
            char ch = s[i];
            switch (ch)
            {
                case '<':
                    output.Write("&lt;");
                    break;
                case '>':
                    output.Write("&gt;");
                    break;
                case '"':
                    output.Write("&quot;");
                    break;
                case '&':
                    output.Write("&amp;");
                    break;
                case ' ':
                    if (prevCh == ' ')
                    {
                        output.Write("&nbsp;");
                    }
                    else
                    {
                        output.Write(ch);
                    }

                    break;
                case '\r':
                    // Ignore \r, only handle \n
                    break;
                case '\n':
                    output.Write("<br>");
                    break;
                //
                default:
                    // The seemingly arbitrary 160 comes from RFC
                    // Code taken from ASP.NET file xsp\System\Web\httpserverutility.cs
                    // Don't entity encode high chars (160 to 256)
                    if (ch is >= (char)160 and < (char)256)
                    {
                        output.Write("&#");
                        output.Write(((int)ch).ToString(NumberFormatInfo.InvariantInfo));
                        output.Write(';');
                        break;
                    }

                    output.Write(ch);
                    break;
            }

            prevCh = ch;
        }
    }

    private static Bitmap GetBitmap(string bitmapName) =>
        ScaleHelper.GetIconResourceAsBitmap(typeof(DataGridViewCell), bitmapName, new Size(s_iconsWidth, s_iconsHeight));

    protected virtual object? GetClipboardContent(
        int rowIndex,
        bool firstCell,
        bool lastCell,
        bool inFirstRow,
        bool inLastRow,
        string format)
    {
        if (DataGridView is null)
        {
            return null;
        }

        // Header Cell classes override this implementation - this implementation is only for inner cells
        ArgumentOutOfRangeException.ThrowIfNegative(rowIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(rowIndex, DataGridView.Rows.Count);

        // Assuming (like in other places in this class) that the formatted value is independent of the style colors.
        DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(inheritedCellStyle: null, rowIndex, includeColors: false);
        object? formattedValue = null;
        if (DataGridView.IsSharedCellSelected(this, rowIndex))
        {
            formattedValue = GetEditedFormattedValue(GetValue(rowIndex), rowIndex, ref dataGridViewCellStyle, DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.ClipboardContent);
        }

        StringBuilder sb = new(64);

        if (string.Equals(format, DataFormats.Html, StringComparison.OrdinalIgnoreCase))
        {
            if (firstCell)
            {
                if (inFirstRow)
                {
                    sb.Append("<TABLE>");
                }

                sb.Append("<TR>");
            }

            sb.Append("<TD>");
            if (formattedValue is not null)
            {
                using StringWriter sw = new(sb, CultureInfo.CurrentCulture);
                FormatPlainTextAsHtml(formattedValue.ToString(), sw);
            }
            else
            {
                sb.Append("&nbsp;");
            }

            sb.Append("</TD>");
            if (lastCell)
            {
                sb.Append("</TR>");
                if (inLastRow)
                {
                    sb.Append("</TABLE>");
                }
            }

            return sb.ToString();
        }
        else
        {
            bool csv = string.Equals(format, DataFormats.CommaSeparatedValue, StringComparison.OrdinalIgnoreCase);
            if (csv ||
                string.Equals(format, DataFormats.Text, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(format, DataFormats.UnicodeText, StringComparison.OrdinalIgnoreCase))
            {
                if (formattedValue is not null)
                {
                    if (firstCell && lastCell && inFirstRow && inLastRow)
                    {
                        sb.Append(formattedValue.ToString());
                    }
                    else
                    {
                        bool escapeApplied = false;
                        int insertionPoint = sb.Length;
                        using StringWriter sw = new(sb, CultureInfo.CurrentCulture);
                        FormatPlainText(formattedValue.ToString(), csv, sw, ref escapeApplied);
                        if (escapeApplied)
                        {
                            Debug.Assert(csv);
                            sb.Insert(insertionPoint, '"');
                        }
                    }
                }

                if (lastCell)
                {
                    if (!inLastRow)
                    {
                        sb.Append((char)Keys.Return);
                        sb.Append((char)Keys.LineFeed);
                    }
                }
                else
                {
                    sb.Append(csv ? ',' : (char)Keys.Tab);
                }

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
    }

    internal object? GetClipboardContentInternal(
        int rowIndex,
        bool firstCell,
        bool lastCell,
        bool inFirstRow,
        bool inLastRow,
        string format)
    {
        return GetClipboardContent(
            rowIndex,
            firstCell,
            lastCell,
            inFirstRow,
            inLastRow,
            format);
    }

    internal ContextMenuStrip? GetContextMenuStrip(int rowIndex)
    {
        ContextMenuStrip? contextMenuStrip = ContextMenuStripInternal;
        if (DataGridView is not null &&
            (DataGridView.VirtualMode || DataGridView.DataSource is not null))
        {
            contextMenuStrip = DataGridView.OnCellContextMenuStripNeeded(ColumnIndex, rowIndex, contextMenuStrip);
        }

        return contextMenuStrip;
    }

    internal (Color darkColor, Color lightColor) GetContrastedColors(Color baseline)
    {
        Debug.Assert(DataGridView is not null);

        int darkDistance = ColorDistance(baseline, Application.SystemColors.ControlDark);
        int lightDistance = ColorDistance(baseline, Application.SystemColors.ControlLightLight);

        Color darkColor;
        Color lightColor;

        if (SystemInformation.HighContrast)
        {
            darkColor = darkDistance < HighContrastThreshold
                ? ControlPaint.DarkDark(baseline)
                : Application.SystemColors.ControlDark;

            lightColor = lightDistance < HighContrastThreshold
                ? ControlPaint.LightLight(baseline)
                : Application.SystemColors.ControlLightLight;
        }
        else
        {
            darkColor = darkDistance < ContrastThreshold
                ? ControlPaint.Dark(baseline)
                : Application.SystemColors.WindowFrame;

            lightColor = lightDistance < ContrastThreshold
                ? ControlPaint.Light(baseline)
                : Application.SystemColors.ControlLightLight;
        }

        return (darkColor, lightColor);
    }

    public Rectangle GetContentBounds(int rowIndex)
    {
        if (DataGridView is null)
        {
            return Rectangle.Empty;
        }

        DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(inheritedCellStyle: null, rowIndex, includeColors: false);

        using var screen = GdiCache.GetScreenDCGraphics();
        return GetContentBounds(screen, dataGridViewCellStyle, rowIndex);
    }

    protected virtual Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex) => Rectangle.Empty;

    private protected virtual string? GetDefaultToolTipText() => string.Empty;

    internal object? GetEditedFormattedValue(
        object? value,
        int rowIndex,
        ref DataGridViewCellStyle dataGridViewCellStyle,
        DataGridViewDataErrorContexts context)
    {
        Debug.Assert(DataGridView is not null);
        Point ptCurrentCell = DataGridView.CurrentCellAddress;
        if (ColumnIndex == ptCurrentCell.X && rowIndex == ptCurrentCell.Y)
        {
            IDataGridViewEditingControl? dataGridViewEditingControl = (IDataGridViewEditingControl?)DataGridView.EditingControl;
            if (dataGridViewEditingControl is not null)
            {
                return dataGridViewEditingControl.GetEditingControlFormattedValue(context);
            }

            if (this is IDataGridViewEditingCell dgvecell && DataGridView.IsCurrentCellInEditMode)
            {
                return dgvecell.GetEditingCellFormattedValue(context);
            }

            return GetFormattedValue(
                value,
                rowIndex,
                ref dataGridViewCellStyle,
                valueTypeConverter: null,
                formattedValueTypeConverter: null,
                context);
        }

        return GetFormattedValue(
            value,
            rowIndex,
            ref dataGridViewCellStyle,
            valueTypeConverter: null,
            formattedValueTypeConverter: null,
            context);
    }

    public object? GetEditedFormattedValue(int rowIndex, DataGridViewDataErrorContexts context)
    {
        if (DataGridView is null)
        {
            return null;
        }

        DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(inheritedCellStyle: null, rowIndex, includeColors: false);
        return GetEditedFormattedValue(GetValue(rowIndex), rowIndex, ref dataGridViewCellStyle, context);
    }

    internal Rectangle GetErrorIconBounds(int rowIndex)
    {
        DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(inheritedCellStyle: null, rowIndex, includeColors: false);
        using var screen = GdiCache.GetScreenDCGraphics();
        return GetErrorIconBounds(screen, dataGridViewCellStyle, rowIndex);
    }

    protected virtual Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex) => Rectangle.Empty;

    protected internal virtual string GetErrorText(int rowIndex)
    {
        string errorText = string.Empty;
        object? objErrorText = Properties.GetObject(s_propCellErrorText);
        if (objErrorText is not null)
        {
            errorText = (string)objErrorText;
        }
        else if (DataGridView is not null &&
                 rowIndex != -1 &&
                 rowIndex != DataGridView.NewRowIndex &&
                 OwningColumn is not null &&
                 OwningColumn.IsDataBound &&
                 DataGridView.DataConnection is not null)
        {
            errorText = DataGridView.DataConnection.GetError(OwningColumn.BoundColumnIndex, ColumnIndex, rowIndex);
        }

        if (DataGridView is not null && (DataGridView.VirtualMode || DataGridView.DataSource is not null) &&
            ColumnIndex >= 0 && rowIndex >= 0)
        {
            errorText = DataGridView.OnCellErrorTextNeeded(ColumnIndex, rowIndex, errorText);
        }

        return errorText;
    }

    internal object? GetFormattedValue(int rowIndex, ref DataGridViewCellStyle cellStyle, DataGridViewDataErrorContexts context)
    {
        if (DataGridView is null)
        {
            return null;
        }
        else
        {
            return GetFormattedValue(
                GetValue(rowIndex),
                rowIndex,
                ref cellStyle,
                valueTypeConverter: null,
                formattedValueTypeConverter: null,
                context);
        }
    }

    protected virtual object? GetFormattedValue(
        object? value,
        int rowIndex,
        ref DataGridViewCellStyle cellStyle,
        TypeConverter? valueTypeConverter,
        TypeConverter? formattedValueTypeConverter,
        DataGridViewDataErrorContexts context)
    {
        if (DataGridView is null)
        {
            return null;
        }

        DataGridViewCellFormattingEventArgs gdvcfe = DataGridView.OnCellFormatting(
            ColumnIndex,
            rowIndex,
            value,
            FormattedValueType,
            cellStyle);
        cellStyle = gdvcfe.CellStyle;
        bool formattingApplied = gdvcfe.FormattingApplied;
        object? formattedValue = gdvcfe.Value;
        bool checkFormattedValType = true;

        if (!formattingApplied &&
            FormattedValueType is not null &&
            (formattedValue is null || !FormattedValueType.IsAssignableFrom(formattedValue.GetType())))
        {
            try
            {
                formattedValue = Formatter.FormatObject(
                    formattedValue,
                    FormattedValueType,
                    sourceConverter: valueTypeConverter ?? ValueTypeConverter,
                    targetConverter: formattedValueTypeConverter ?? FormattedValueTypeConverter,
                    cellStyle.Format,
                    cellStyle.FormatProvider,
                    cellStyle.NullValue,
                    cellStyle.DataSourceNullValue);
            }
            catch (Exception exception) when (!exception.IsCriticalException())
            {
                // Formatting failed, raise OnDataError event.
                DataGridViewDataErrorEventArgs dgvdee = new(
                    exception,
                    ColumnIndex,
                    rowIndex,
                    context);
                RaiseDataError(dgvdee);
                if (dgvdee.ThrowException)
                {
                    throw dgvdee.Exception;
                }

                checkFormattedValType = false;
            }
        }

        if (checkFormattedValType &&
            (formattedValue is null || FormattedValueType is null || !FormattedValueType.IsAssignableFrom(formattedValue.GetType())))
        {
            if (formattedValue is null &&
                cellStyle.NullValue is null &&
                FormattedValueType is not null &&
                !typeof(ValueType).IsAssignableFrom(FormattedValueType))
            {
                // null is an acceptable formatted value
                return null;
            }

            Exception exception;
            if (FormattedValueType is null)
            {
                exception = new FormatException(SR.DataGridViewCell_FormattedValueTypeNull);
            }
            else
            {
                exception = new FormatException(SR.DataGridViewCell_FormattedValueHasWrongType);
            }

            DataGridViewDataErrorEventArgs dgvdee = new(
                exception,
                ColumnIndex,
                rowIndex,
                context);
            RaiseDataError(dgvdee);
            if (dgvdee.ThrowException)
            {
                throw dgvdee.Exception;
            }
        }

        return formattedValue;
    }

    internal static DataGridViewFreeDimension GetFreeDimensionFromConstraint(Size constraintSize)
    {
        if (constraintSize.Width < 0 || constraintSize.Height < 0)
        {
            throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(constraintSize), constraintSize));
        }

        if (constraintSize.Width == 0)
        {
            if (constraintSize.Height == 0)
            {
                return DataGridViewFreeDimension.Both;
            }
            else
            {
                return DataGridViewFreeDimension.Width;
            }
        }
        else
        {
            if (constraintSize.Height == 0)
            {
                return DataGridViewFreeDimension.Height;
            }
            else
            {
                throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(constraintSize), constraintSize));
            }
        }
    }

    internal int GetHeight(int rowIndex)
    {
        if (DataGridView is null)
        {
            return -1;
        }

        Debug.Assert(OwningRow is not null);
        return OwningRow.GetHeight(rowIndex);
    }

    public virtual ContextMenuStrip? GetInheritedContextMenuStrip(int rowIndex)
    {
        if (DataGridView is not null)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(rowIndex);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(rowIndex, DataGridView.Rows.Count);

            if (ColumnIndex < 0)
            {
                throw new InvalidOperationException();
            }

            Debug.Assert(ColumnIndex < DataGridView.Columns.Count);
        }

        ContextMenuStrip? contextMenuStrip = GetContextMenuStrip(rowIndex);
        if (contextMenuStrip is not null)
        {
            return contextMenuStrip;
        }

        if (OwningRow is not null)
        {
            contextMenuStrip = OwningRow.GetContextMenuStrip(rowIndex);
            if (contextMenuStrip is not null)
            {
                return contextMenuStrip;
            }
        }

        if (OwningColumn is not null)
        {
            contextMenuStrip = OwningColumn.ContextMenuStrip;
            if (contextMenuStrip is not null)
            {
                return contextMenuStrip;
            }
        }

        if (DataGridView is not null)
        {
            return DataGridView.ContextMenuStrip;
        }
        else
        {
            return null;
        }
    }

    public virtual DataGridViewElementStates GetInheritedState(int rowIndex)
    {
        DataGridViewElementStates state = State | DataGridViewElementStates.ResizableSet;

        if (DataGridView is null)
        {
            Debug.Assert(RowIndex == -1);
            if (rowIndex != -1)
            {
                throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(rowIndex), rowIndex));
            }

            if (OwningRow is not null)
            {
                state |= (OwningRow.GetState(-1) & (DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible));
                if (OwningRow.GetResizable(rowIndex) == DataGridViewTriState.True)
                {
                    state |= DataGridViewElementStates.Resizable;
                }
            }

            return state;
        }

        // Header Cell classes override this implementation - this implementation is only for inner cells
        ArgumentOutOfRangeException.ThrowIfNegative(rowIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(rowIndex, DataGridView.Rows.Count);

        Debug.Assert(OwningColumn is not null);
        Debug.Assert(OwningRow is not null);
        Debug.Assert(ColumnIndex >= 0);

        if (DataGridView.Rows.SharedRow(rowIndex) != OwningRow)
        {
            throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(rowIndex), rowIndex));
        }

        DataGridViewElementStates rowEffectiveState = DataGridView.Rows.GetRowState(rowIndex);
        state |= (rowEffectiveState & (DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected));
        state |= (OwningColumn.State & (DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected));

        if (OwningRow.GetResizable(rowIndex) == DataGridViewTriState.True ||
            OwningColumn.Resizable == DataGridViewTriState.True)
        {
            state |= DataGridViewElementStates.Resizable;
        }

        if (OwningColumn.Visible && OwningRow.GetVisible(rowIndex))
        {
            state |= DataGridViewElementStates.Visible;
            if (OwningColumn.Displayed && OwningRow.GetDisplayed(rowIndex))
            {
                state |= DataGridViewElementStates.Displayed;
            }
        }

        if (OwningColumn.Frozen && OwningRow.GetFrozen(rowIndex))
        {
            state |= DataGridViewElementStates.Frozen;
        }

#if DEBUG
        DataGridViewElementStates stateDebug = DataGridViewElementStates.ResizableSet;
        if (Displayed)
        {
            stateDebug |= DataGridViewElementStates.Displayed;
        }

        if (Frozen)
        {
            stateDebug |= DataGridViewElementStates.Frozen;
        }

        if (ReadOnly)
        {
            stateDebug |= DataGridViewElementStates.ReadOnly;
        }

        if (Resizable)
        {
            stateDebug |= DataGridViewElementStates.Resizable;
        }

        if (Selected)
        {
            stateDebug |= DataGridViewElementStates.Selected;
        }

        if (Visible)
        {
            stateDebug |= DataGridViewElementStates.Visible;
        }

        Debug.Assert(state == stateDebug || DataGridView.Rows.SharedRow(rowIndex).Index == -1);
#endif
        return state;
    }

    public virtual DataGridViewCellStyle GetInheritedStyle(DataGridViewCellStyle? inheritedCellStyle, int rowIndex, bool includeColors)
    {
        if (DataGridView is null)
        {
            throw new InvalidOperationException(SR.DataGridView_CellNeedsDataGridViewForInheritedStyle);
        }

        ArgumentOutOfRangeException.ThrowIfNegative(rowIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(rowIndex, DataGridView.Rows.Count);

        if (ColumnIndex < 0)
        {
            throw new InvalidOperationException();
        }

        Debug.Assert(ColumnIndex < DataGridView.Columns.Count);

        DataGridViewCellStyle inheritedCellStyleTmp;
        if (inheritedCellStyle is null)
        {
            inheritedCellStyleTmp = DataGridView.PlaceholderCellStyle;
            if (!includeColors)
            {
                inheritedCellStyleTmp.BackColor = Color.Empty;
                inheritedCellStyleTmp.ForeColor = Color.Empty;
                inheritedCellStyleTmp.SelectionBackColor = Color.Empty;
                inheritedCellStyleTmp.SelectionForeColor = Color.Empty;
            }
        }
        else
        {
            inheritedCellStyleTmp = inheritedCellStyle;
        }

        DataGridViewCellStyle? cellStyle = null;
        if (HasStyle)
        {
            cellStyle = Style;
            Debug.Assert(cellStyle is not null);
        }

        DataGridViewCellStyle? rowStyle = null;
        if (DataGridView.Rows.SharedRow(rowIndex).HasDefaultCellStyle)
        {
            rowStyle = DataGridView.Rows.SharedRow(rowIndex).DefaultCellStyle;
            Debug.Assert(rowStyle is not null);
        }

        DataGridViewCellStyle? columnStyle = null;
        if (OwningColumn is not null && OwningColumn.HasDefaultCellStyle)
        {
            columnStyle = OwningColumn.DefaultCellStyle;
            Debug.Assert(columnStyle is not null);
        }

        DataGridViewCellStyle dataGridViewStyle = DataGridView.DefaultCellStyle;
        Debug.Assert(dataGridViewStyle is not null);

        if (includeColors)
        {
            if (cellStyle is not null && !cellStyle.BackColor.IsEmpty)
            {
                inheritedCellStyleTmp.BackColor = cellStyle.BackColor;
            }
            else if (rowStyle is not null && !rowStyle.BackColor.IsEmpty)
            {
                inheritedCellStyleTmp.BackColor = rowStyle.BackColor;
            }
            else if (!DataGridView.RowsDefaultCellStyle.BackColor.IsEmpty &&
                (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.BackColor.IsEmpty))
            {
                inheritedCellStyleTmp.BackColor = DataGridView.RowsDefaultCellStyle.BackColor;
            }
            else if (rowIndex % 2 == 1 && !DataGridView.AlternatingRowsDefaultCellStyle.BackColor.IsEmpty)
            {
                inheritedCellStyleTmp.BackColor = DataGridView.AlternatingRowsDefaultCellStyle.BackColor;
            }
            else if (columnStyle is not null && !columnStyle.BackColor.IsEmpty)
            {
                inheritedCellStyleTmp.BackColor = columnStyle.BackColor;
            }
            else
            {
                inheritedCellStyleTmp.BackColor = dataGridViewStyle.BackColor;
            }

            if (cellStyle is not null && !cellStyle.ForeColor.IsEmpty)
            {
                inheritedCellStyleTmp.ForeColor = cellStyle.ForeColor;
            }
            else if (rowStyle is not null && !rowStyle.ForeColor.IsEmpty)
            {
                inheritedCellStyleTmp.ForeColor = rowStyle.ForeColor;
            }
            else if (!DataGridView.RowsDefaultCellStyle.ForeColor.IsEmpty &&
                (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.ForeColor.IsEmpty))
            {
                inheritedCellStyleTmp.ForeColor = DataGridView.RowsDefaultCellStyle.ForeColor;
            }
            else if (rowIndex % 2 == 1 && !DataGridView.AlternatingRowsDefaultCellStyle.ForeColor.IsEmpty)
            {
                inheritedCellStyleTmp.ForeColor = DataGridView.AlternatingRowsDefaultCellStyle.ForeColor;
            }
            else if (columnStyle is not null && !columnStyle.ForeColor.IsEmpty)
            {
                inheritedCellStyleTmp.ForeColor = columnStyle.ForeColor;
            }
            else
            {
                inheritedCellStyleTmp.ForeColor = dataGridViewStyle.ForeColor;
            }

            if (cellStyle is not null && !cellStyle.SelectionBackColor.IsEmpty)
            {
                inheritedCellStyleTmp.SelectionBackColor = cellStyle.SelectionBackColor;
            }
            else if (rowStyle is not null && !rowStyle.SelectionBackColor.IsEmpty)
            {
                inheritedCellStyleTmp.SelectionBackColor = rowStyle.SelectionBackColor;
            }
            else if (!DataGridView.RowsDefaultCellStyle.SelectionBackColor.IsEmpty &&
                (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.SelectionBackColor.IsEmpty))
            {
                inheritedCellStyleTmp.SelectionBackColor = DataGridView.RowsDefaultCellStyle.SelectionBackColor;
            }
            else if (rowIndex % 2 == 1 && !DataGridView.AlternatingRowsDefaultCellStyle.SelectionBackColor.IsEmpty)
            {
                inheritedCellStyleTmp.SelectionBackColor = DataGridView.AlternatingRowsDefaultCellStyle.SelectionBackColor;
            }
            else if (columnStyle is not null && !columnStyle.SelectionBackColor.IsEmpty)
            {
                inheritedCellStyleTmp.SelectionBackColor = columnStyle.SelectionBackColor;
            }
            else
            {
                inheritedCellStyleTmp.SelectionBackColor = dataGridViewStyle.SelectionBackColor;
            }

            if (cellStyle is not null && !cellStyle.SelectionForeColor.IsEmpty)
            {
                inheritedCellStyleTmp.SelectionForeColor = cellStyle.SelectionForeColor;
            }
            else if (rowStyle is not null && !rowStyle.SelectionForeColor.IsEmpty)
            {
                inheritedCellStyleTmp.SelectionForeColor = rowStyle.SelectionForeColor;
            }
            else if (!DataGridView.RowsDefaultCellStyle.SelectionForeColor.IsEmpty &&
                (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.SelectionForeColor.IsEmpty))
            {
                inheritedCellStyleTmp.SelectionForeColor = DataGridView.RowsDefaultCellStyle.SelectionForeColor;
            }
            else if (rowIndex % 2 == 1 && !DataGridView.AlternatingRowsDefaultCellStyle.SelectionForeColor.IsEmpty)
            {
                inheritedCellStyleTmp.SelectionForeColor = DataGridView.AlternatingRowsDefaultCellStyle.SelectionForeColor;
            }
            else if (columnStyle is not null && !columnStyle.SelectionForeColor.IsEmpty)
            {
                inheritedCellStyleTmp.SelectionForeColor = columnStyle.SelectionForeColor;
            }
            else
            {
                inheritedCellStyleTmp.SelectionForeColor = dataGridViewStyle.SelectionForeColor;
            }
        }

        if (cellStyle is not null && cellStyle.Font is not null)
        {
            inheritedCellStyleTmp.Font = cellStyle.Font;
        }
        else if (rowStyle is not null && rowStyle.Font is not null)
        {
            inheritedCellStyleTmp.Font = rowStyle.Font;
        }
        else if (DataGridView.RowsDefaultCellStyle.Font is not null &&
            (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.Font is null))
        {
            inheritedCellStyleTmp.Font = DataGridView.RowsDefaultCellStyle.Font;
        }
        else if (rowIndex % 2 == 1 && DataGridView.AlternatingRowsDefaultCellStyle.Font is not null)
        {
            inheritedCellStyleTmp.Font = DataGridView.AlternatingRowsDefaultCellStyle.Font;
        }
        else if (columnStyle is not null && columnStyle.Font is not null)
        {
            inheritedCellStyleTmp.Font = columnStyle.Font;
        }
        else
        {
            inheritedCellStyleTmp.Font = dataGridViewStyle.Font;
        }

        if (cellStyle is not null && !cellStyle.IsNullValueDefault)
        {
            inheritedCellStyleTmp.NullValue = cellStyle.NullValue;
        }
        else if (rowStyle is not null && !rowStyle.IsNullValueDefault)
        {
            inheritedCellStyleTmp.NullValue = rowStyle.NullValue;
        }
        else if (!DataGridView.RowsDefaultCellStyle.IsNullValueDefault &&
                 (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.IsNullValueDefault))
        {
            inheritedCellStyleTmp.NullValue = DataGridView.RowsDefaultCellStyle.NullValue;
        }
        else if (rowIndex % 2 == 1 &&
                 !DataGridView.AlternatingRowsDefaultCellStyle.IsNullValueDefault)
        {
            inheritedCellStyleTmp.NullValue = DataGridView.AlternatingRowsDefaultCellStyle.NullValue;
        }
        else if (columnStyle is not null && !columnStyle.IsNullValueDefault)
        {
            inheritedCellStyleTmp.NullValue = columnStyle.NullValue;
        }
        else
        {
            inheritedCellStyleTmp.NullValue = dataGridViewStyle.NullValue;
        }

        if (cellStyle is not null && !cellStyle.IsDataSourceNullValueDefault)
        {
            inheritedCellStyleTmp.DataSourceNullValue = cellStyle.DataSourceNullValue;
        }
        else if (rowStyle is not null && !rowStyle.IsDataSourceNullValueDefault)
        {
            inheritedCellStyleTmp.DataSourceNullValue = rowStyle.DataSourceNullValue;
        }
        else if (!DataGridView.RowsDefaultCellStyle.IsDataSourceNullValueDefault &&
            (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.IsDataSourceNullValueDefault))
        {
            inheritedCellStyleTmp.DataSourceNullValue = DataGridView.RowsDefaultCellStyle.DataSourceNullValue;
        }
        else if (rowIndex % 2 == 1 &&
            !DataGridView.AlternatingRowsDefaultCellStyle.IsDataSourceNullValueDefault)
        {
            inheritedCellStyleTmp.DataSourceNullValue = DataGridView.AlternatingRowsDefaultCellStyle.DataSourceNullValue;
        }
        else if (columnStyle is not null && !columnStyle.IsDataSourceNullValueDefault)
        {
            inheritedCellStyleTmp.DataSourceNullValue = columnStyle.DataSourceNullValue;
        }
        else
        {
            inheritedCellStyleTmp.DataSourceNullValue = dataGridViewStyle.DataSourceNullValue;
        }

        if (cellStyle is not null && cellStyle.Format.Length != 0)
        {
            inheritedCellStyleTmp.Format = cellStyle.Format;
        }
        else if (rowStyle is not null && rowStyle.Format.Length != 0)
        {
            inheritedCellStyleTmp.Format = rowStyle.Format;
        }
        else if (DataGridView.RowsDefaultCellStyle.Format.Length != 0 &&
            (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.Format.Length == 0))
        {
            inheritedCellStyleTmp.Format = DataGridView.RowsDefaultCellStyle.Format;
        }
        else if (rowIndex % 2 == 1 && DataGridView.AlternatingRowsDefaultCellStyle.Format.Length != 0)
        {
            inheritedCellStyleTmp.Format = DataGridView.AlternatingRowsDefaultCellStyle.Format;
        }
        else if (columnStyle is not null && columnStyle.Format.Length != 0)
        {
            inheritedCellStyleTmp.Format = columnStyle.Format;
        }
        else
        {
            inheritedCellStyleTmp.Format = dataGridViewStyle.Format;
        }

        if (cellStyle is not null && !cellStyle.IsFormatProviderDefault)
        {
            inheritedCellStyleTmp.FormatProvider = cellStyle.FormatProvider;
        }
        else if (rowStyle is not null && !rowStyle.IsFormatProviderDefault)
        {
            inheritedCellStyleTmp.FormatProvider = rowStyle.FormatProvider;
        }
        else if (!DataGridView.RowsDefaultCellStyle.IsFormatProviderDefault &&
                 (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.IsFormatProviderDefault))
        {
            inheritedCellStyleTmp.FormatProvider = DataGridView.RowsDefaultCellStyle.FormatProvider;
        }
        else if (rowIndex % 2 == 1 && !DataGridView.AlternatingRowsDefaultCellStyle.IsFormatProviderDefault)
        {
            inheritedCellStyleTmp.FormatProvider = DataGridView.AlternatingRowsDefaultCellStyle.FormatProvider;
        }
        else if (columnStyle is not null && !columnStyle.IsFormatProviderDefault)
        {
            inheritedCellStyleTmp.FormatProvider = columnStyle.FormatProvider;
        }
        else
        {
            inheritedCellStyleTmp.FormatProvider = dataGridViewStyle.FormatProvider;
        }

        if (cellStyle is not null && cellStyle.Alignment != DataGridViewContentAlignment.NotSet)
        {
            inheritedCellStyleTmp.AlignmentInternal = cellStyle.Alignment;
        }
        else if (rowStyle is not null && rowStyle.Alignment != DataGridViewContentAlignment.NotSet)
        {
            inheritedCellStyleTmp.AlignmentInternal = rowStyle.Alignment;
        }
        else if (DataGridView.RowsDefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet &&
            (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.Alignment == DataGridViewContentAlignment.NotSet))
        {
            inheritedCellStyleTmp.AlignmentInternal = DataGridView.RowsDefaultCellStyle.Alignment;
        }
        else if (rowIndex % 2 == 1 && DataGridView.AlternatingRowsDefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet)
        {
            inheritedCellStyleTmp.AlignmentInternal = DataGridView.AlternatingRowsDefaultCellStyle.Alignment;
        }
        else if (columnStyle is not null && columnStyle.Alignment != DataGridViewContentAlignment.NotSet)
        {
            inheritedCellStyleTmp.AlignmentInternal = columnStyle.Alignment;
        }
        else
        {
            Debug.Assert(dataGridViewStyle.Alignment != DataGridViewContentAlignment.NotSet);
            inheritedCellStyleTmp.AlignmentInternal = dataGridViewStyle.Alignment;
        }

        if (cellStyle is not null && cellStyle.WrapMode != DataGridViewTriState.NotSet)
        {
            inheritedCellStyleTmp.WrapModeInternal = cellStyle.WrapMode;
        }
        else if (rowStyle is not null && rowStyle.WrapMode != DataGridViewTriState.NotSet)
        {
            inheritedCellStyleTmp.WrapModeInternal = rowStyle.WrapMode;
        }
        else if (DataGridView.RowsDefaultCellStyle.WrapMode != DataGridViewTriState.NotSet &&
            (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.WrapMode == DataGridViewTriState.NotSet))
        {
            inheritedCellStyleTmp.WrapModeInternal = DataGridView.RowsDefaultCellStyle.WrapMode;
        }
        else if (rowIndex % 2 == 1 && DataGridView.AlternatingRowsDefaultCellStyle.WrapMode != DataGridViewTriState.NotSet)
        {
            inheritedCellStyleTmp.WrapModeInternal = DataGridView.AlternatingRowsDefaultCellStyle.WrapMode;
        }
        else if (columnStyle is not null && columnStyle.WrapMode != DataGridViewTriState.NotSet)
        {
            inheritedCellStyleTmp.WrapModeInternal = columnStyle.WrapMode;
        }
        else
        {
            Debug.Assert(dataGridViewStyle.WrapMode != DataGridViewTriState.NotSet);
            inheritedCellStyleTmp.WrapModeInternal = dataGridViewStyle.WrapMode;
        }

        if (cellStyle is not null && cellStyle.Tag is not null)
        {
            inheritedCellStyleTmp.Tag = cellStyle.Tag;
        }
        else if (rowStyle is not null && rowStyle.Tag is not null)
        {
            inheritedCellStyleTmp.Tag = rowStyle.Tag;
        }
        else if (DataGridView.RowsDefaultCellStyle.Tag is not null &&
            (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.Tag is null))
        {
            inheritedCellStyleTmp.Tag = DataGridView.RowsDefaultCellStyle.Tag;
        }
        else if (rowIndex % 2 == 1 && DataGridView.AlternatingRowsDefaultCellStyle.Tag is not null)
        {
            inheritedCellStyleTmp.Tag = DataGridView.AlternatingRowsDefaultCellStyle.Tag;
        }
        else if (columnStyle is not null && columnStyle.Tag is not null)
        {
            inheritedCellStyleTmp.Tag = columnStyle.Tag;
        }
        else
        {
            inheritedCellStyleTmp.Tag = dataGridViewStyle.Tag;
        }

        if (cellStyle is not null && cellStyle.Padding != Padding.Empty)
        {
            inheritedCellStyleTmp.PaddingInternal = cellStyle.Padding;
        }
        else if (rowStyle is not null && rowStyle.Padding != Padding.Empty)
        {
            inheritedCellStyleTmp.PaddingInternal = rowStyle.Padding;
        }
        else if (DataGridView.RowsDefaultCellStyle.Padding != Padding.Empty &&
            (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.Padding == Padding.Empty))
        {
            inheritedCellStyleTmp.PaddingInternal = DataGridView.RowsDefaultCellStyle.Padding;
        }
        else if (rowIndex % 2 == 1 && DataGridView.AlternatingRowsDefaultCellStyle.Padding != Padding.Empty)
        {
            inheritedCellStyleTmp.PaddingInternal = DataGridView.AlternatingRowsDefaultCellStyle.Padding;
        }
        else if (columnStyle is not null && columnStyle.Padding != Padding.Empty)
        {
            inheritedCellStyleTmp.PaddingInternal = columnStyle.Padding;
        }
        else
        {
            inheritedCellStyleTmp.PaddingInternal = dataGridViewStyle.Padding;
        }

        return inheritedCellStyleTmp;
    }

    internal DataGridViewCellStyle GetInheritedStyleInternal(int rowIndex) =>
        GetInheritedStyle(inheritedCellStyle: null, rowIndex, includeColors: true);

    internal int GetPreferredHeight(int rowIndex, int width)
    {
        Debug.Assert(width > 0);

        if (DataGridView is null)
        {
            return -1;
        }

        DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(inheritedCellStyle: null, rowIndex, includeColors: false);
        using var screen = GdiCache.GetScreenDCGraphics();
        return GetPreferredSize(screen, dataGridViewCellStyle, rowIndex, new Size(width, 0)).Height;
    }

    internal Size GetPreferredSize(int rowIndex)
    {
        if (DataGridView is null)
        {
            return new Size(-1, -1);
        }

        DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(inheritedCellStyle: null, rowIndex, includeColors: false);
        using var screen = GdiCache.GetScreenDCGraphics();
        return GetPreferredSize(screen, dataGridViewCellStyle, rowIndex, Size.Empty);
    }

    protected virtual Size GetPreferredSize(
        Graphics graphics,
        DataGridViewCellStyle cellStyle,
        int rowIndex,
        Size constraintSize) => new(-1, -1);

    internal static int GetPreferredTextHeight(
        Graphics g,
        bool rightToLeft,
        string text,
        DataGridViewCellStyle cellStyle,
        int maxWidth,
        out bool widthTruncated)
    {
        Debug.Assert(maxWidth > 0);

        TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(rightToLeft, cellStyle.Alignment, cellStyle.WrapMode);
        if (cellStyle.WrapMode == DataGridViewTriState.True)
        {
            return MeasureTextHeight(g, text, cellStyle.Font, maxWidth, flags, out widthTruncated);
        }
        else
        {
            Size size = MeasureTextSize(g, text, cellStyle.Font, flags);
            widthTruncated = size.Width > maxWidth;
            return size.Height;
        }
    }

    internal int GetPreferredWidth(int rowIndex, int height)
    {
        Debug.Assert(height > 0);

        if (DataGridView is null)
        {
            return -1;
        }

        DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(inheritedCellStyle: null, rowIndex, includeColors: false);
        using var screen = GdiCache.GetScreenDCGraphics();
        return GetPreferredSize(screen, dataGridViewCellStyle, rowIndex, new Size(0, height)).Width;
    }

    protected virtual Size GetSize(int rowIndex)
    {
        if (DataGridView is null)
        {
            return new Size(-1, -1);
        }

        if (rowIndex == -1)
        {
            throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedCell, "Size"));
        }

        Debug.Assert(OwningColumn is not null);
        Debug.Assert(OwningRow is not null);
        return new Size(OwningColumn.Thickness, OwningRow.GetHeight(rowIndex));
    }

    private protected string? GetInternalToolTipText(int rowIndex)
    {
        string? toolTipText = ToolTipTextInternal;
        if (DataGridView is not null &&
            (DataGridView.VirtualMode || DataGridView.DataSource is not null))
        {
            toolTipText = DataGridView.OnCellToolTipTextNeeded(ColumnIndex, rowIndex, toolTipText);
        }

        return toolTipText;
    }

    private string? GetToolTipText(int rowIndex)
    {
        string? toolTipText = GetInternalToolTipText(rowIndex);

        if (ColumnIndex < 0 || RowIndex < 0)
        {
            return toolTipText;  // Cells in the Unit tests have ColumnIndex & RowIndex < 0 and
                                 //  we should return an expected result. It doesn't have an impact on UI cells.
        }

        if (string.IsNullOrEmpty(toolTipText))
        {
            if (!_useDefaultToolTipText)
            {
                return string.Empty;
            }

            return GetDefaultToolTipText() ?? GetToolTipTextWithoutMnemonic(Value?.ToString());
        }

        return toolTipText;
    }

    private protected static string? GetToolTipTextWithoutMnemonic(string? toolTipText)
    {
        if (WindowsFormsUtils.ContainsMnemonic(toolTipText))
        {
            toolTipText = string.Join(string.Empty, toolTipText.Split('&'));
        }

        return toolTipText;
    }

    protected virtual object? GetValue(int rowIndex)
    {
        DataGridView? dataGridView = DataGridView;
        if (dataGridView is not null)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(rowIndex);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(rowIndex, dataGridView.Rows.Count);

            if (ColumnIndex < 0)
            {
                throw new InvalidOperationException();
            }

            Debug.Assert(ColumnIndex < dataGridView.Columns.Count);
        }

        if (dataGridView is null ||
            (dataGridView.AllowUserToAddRowsInternal && rowIndex > -1 && rowIndex == dataGridView.NewRowIndex && rowIndex != dataGridView.CurrentCellAddress.Y) ||
            (!dataGridView.VirtualMode && OwningColumn is not null && !OwningColumn.IsDataBound) ||
            rowIndex == -1 ||
            ColumnIndex == -1)
        {
            return Properties.GetObject(s_propCellValue);
        }
        else if (OwningColumn is not null && OwningColumn.IsDataBound)
        {
            DataGridView.DataGridViewDataConnection? dataConnection = dataGridView.DataConnection;
            if (dataConnection is null)
            {
                return null;
            }
            else if ((dataConnection.CurrencyManager?.Count ?? 0) <= rowIndex)
            {
                return Properties.GetObject(s_propCellValue);
            }
            else
            {
                return dataConnection.GetValue(OwningColumn.BoundColumnIndex, ColumnIndex, rowIndex);
            }
        }
        else
        {
            Debug.Assert(rowIndex >= 0);
            Debug.Assert(ColumnIndex >= 0);
            return dataGridView.OnCellValueNeeded(ColumnIndex, rowIndex);
        }
    }

    internal object? GetValueInternal(int rowIndex) => GetValue(rowIndex);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual void InitializeEditingControl(int rowIndex, object? initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
    {
        DataGridView? dgv = DataGridView;
        if (dgv is null || dgv.EditingControl is null)
        {
            throw new InvalidOperationException();
        }

        // Only add the control to the dataGridView's children if this hasn't been done yet since
        // InitializeEditingControl can be called several times.
        if (dgv.EditingControl.ParentInternal is null)
        {
            // Add editing control to the dataGridView hierarchy
            dgv.EditingControl.CausesValidation = dgv.CausesValidation;
            dgv.EditingPanel.CausesValidation = dgv.CausesValidation;
            dgv.EditingControl.Visible = true;
            Debug.Assert(!dgv.EditingPanel.ContainsFocus);
            dgv.EditingPanel.Visible = false;
            Debug.Assert(dgv.EditingPanel.ParentInternal is null);
            dgv.Controls.Add(dgv.EditingPanel);
            dgv.EditingPanel.Controls.Add(dgv.EditingControl);
            Debug.Assert(dgv.IsSharedCellVisible(this, rowIndex));
        }

        Debug.Assert(dgv.EditingControl.ParentInternal == dgv.EditingPanel);
        Debug.Assert(dgv.EditingPanel.ParentInternal == dgv);

        if (AccessibleRestructuringNeeded)
        {
            // We have already checked that EditingControl is not null.
            dgv.EditingControlAccessibleObject!.SetParent(AccessibilityObject);
            AccessibilityObject.SetDetachableChild(dgv.EditingControlAccessibleObject);
        }
    }

    protected virtual bool KeyDownUnsharesRow(KeyEventArgs e, int rowIndex) => false;

    internal bool KeyDownUnsharesRowInternal(KeyEventArgs e, int rowIndex) =>
        KeyDownUnsharesRow(e, rowIndex);

    public virtual bool KeyEntersEditMode(KeyEventArgs e) => false;

    protected virtual bool KeyPressUnsharesRow(KeyPressEventArgs e, int rowIndex) => false;

    internal bool KeyPressUnsharesRowInternal(KeyPressEventArgs e, int rowIndex) =>
        KeyPressUnsharesRow(e, rowIndex);

    protected virtual bool KeyUpUnsharesRow(KeyEventArgs e, int rowIndex) => false;

    internal bool KeyUpUnsharesRowInternal(KeyEventArgs e, int rowIndex) =>
        KeyUpUnsharesRow(e, rowIndex);

    protected virtual bool LeaveUnsharesRow(int rowIndex, bool throughMouseClick) => false;

    internal bool LeaveUnsharesRowInternal(int rowIndex, bool throughMouseClick) =>
        LeaveUnsharesRow(rowIndex, throughMouseClick);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static int MeasureTextHeight(
        Graphics graphics,
        string? text,
        Font font,
        int maxWidth,
        TextFormatFlags flags) => MeasureTextHeight(
            graphics,
            text,
            font,
            maxWidth,
            flags,
            out bool _);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static int MeasureTextHeight(
        Graphics graphics,
        string? text,
        Font font,
        int maxWidth,
        TextFormatFlags flags,
        out bool widthTruncated)
    {
        ArgumentNullException.ThrowIfNull(graphics);
        ArgumentNullException.ThrowIfNull(font);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxWidth);

        if (!DataGridViewUtilities.ValidTextFormatFlags(flags))
        {
            throw new InvalidEnumArgumentException(nameof(flags), (int)flags, typeof(TextFormatFlags));
        }

        flags &= TextFormatSupportedFlags;
        // Don't use passed in graphics so we can optimize measurement
        Size requiredSize = TextRenderer.MeasureText(text, font, new Size(maxWidth, int.MaxValue), flags);
        widthTruncated = (requiredSize.Width > maxWidth);
        return requiredSize.Height;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Size MeasureTextPreferredSize(
        Graphics graphics,
        string? text,
        Font font,
        float maxRatio,
        TextFormatFlags flags)
    {
        ArgumentNullException.ThrowIfNull(graphics);
        ArgumentNullException.ThrowIfNull(font);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxRatio);

        if (!DataGridViewUtilities.ValidTextFormatFlags(flags))
        {
            throw new InvalidEnumArgumentException(nameof(flags), (int)flags, typeof(TextFormatFlags));
        }

        if (string.IsNullOrEmpty(text))
        {
            return new Size(0, 0);
        }

        Size textOneLineSize = MeasureTextSize(graphics, text, font, flags);
        if (textOneLineSize.Width / textOneLineSize.Height <= maxRatio)
        {
            return textOneLineSize;
        }

        flags &= TextFormatSupportedFlags;
        float maxWidth = textOneLineSize.Width * textOneLineSize.Width / (float)textOneLineSize.Height / maxRatio * 1.1F;
        Size textSize;
        do
        {
            // Don't use passed in graphics so we can optimize measurement
            textSize = TextRenderer.MeasureText(text, font, new Size((int)maxWidth, int.MaxValue), flags);
            if (textSize.Width / textSize.Height <= maxRatio || textSize.Width > (int)maxWidth)
            {
                return textSize;
            }

            maxWidth = textSize.Width * 0.9F;
        }
        while (maxWidth > 1.0F);
        return textSize;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Size MeasureTextSize(
        Graphics graphics,
        string? text,
        Font font,
        TextFormatFlags flags)
    {
        ArgumentNullException.ThrowIfNull(graphics);
        ArgumentNullException.ThrowIfNull(font);

        if (!DataGridViewUtilities.ValidTextFormatFlags(flags))
        {
            throw new InvalidEnumArgumentException(nameof(flags), (int)flags, typeof(TextFormatFlags));
        }

        flags &= TextFormatSupportedFlags;
        // Don't use passed in graphics so we can optimize measurement
        return TextRenderer.MeasureText(text, font, new Size(int.MaxValue, int.MaxValue), flags);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static int MeasureTextWidth(
        Graphics graphics,
        string? text,
        Font font,
        int maxHeight,
        TextFormatFlags flags)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxHeight);

        Size oneLineSize = MeasureTextSize(graphics, text, font, flags);
        if (oneLineSize.Height >= maxHeight || (flags & TextFormatFlags.SingleLine) != 0)
        {
            return oneLineSize.Width;
        }
        else
        {
            flags &= TextFormatSupportedFlags;
            int lastFittingWidth = oneLineSize.Width;
            float maxWidth = lastFittingWidth * 0.9F;
            Size textSize;
            do
            {
                // Don't use passed in graphics so we can optimize measurement
                textSize = TextRenderer.MeasureText(text, font, new Size((int)maxWidth, maxHeight), flags);
                if (textSize.Height > maxHeight || textSize.Width > (int)maxWidth)
                {
                    return lastFittingWidth;
                }
                else
                {
                    lastFittingWidth = (int)maxWidth;
                    maxWidth = textSize.Width * 0.9F;
                }
            }
            while (maxWidth > 1.0F);
            Debug.Assert(textSize.Height <= maxHeight);
            return lastFittingWidth;
        }
    }

    protected virtual bool MouseClickUnsharesRow(DataGridViewCellMouseEventArgs e) => false;

    internal bool MouseClickUnsharesRowInternal(DataGridViewCellMouseEventArgs e) =>
        MouseClickUnsharesRow(e);

    protected virtual bool MouseDoubleClickUnsharesRow(DataGridViewCellMouseEventArgs e) => false;

    internal bool MouseDoubleClickUnsharesRowInternal(DataGridViewCellMouseEventArgs e) =>
        MouseDoubleClickUnsharesRow(e);

    protected virtual bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e) => false;

    internal bool MouseDownUnsharesRowInternal(DataGridViewCellMouseEventArgs e) =>
        MouseDownUnsharesRow(e);

    protected virtual bool MouseEnterUnsharesRow(int rowIndex) => false;

    internal bool MouseEnterUnsharesRowInternal(int rowIndex) =>
        MouseEnterUnsharesRow(rowIndex);

    protected virtual bool MouseLeaveUnsharesRow(int rowIndex) => false;

    internal bool MouseLeaveUnsharesRowInternal(int rowIndex) =>
        MouseLeaveUnsharesRow(rowIndex);

    protected virtual bool MouseMoveUnsharesRow(DataGridViewCellMouseEventArgs e) => false;

    internal bool MouseMoveUnsharesRowInternal(DataGridViewCellMouseEventArgs e) =>
        MouseMoveUnsharesRow(e);

    protected virtual bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e) => false;

    internal bool MouseUpUnsharesRowInternal(DataGridViewCellMouseEventArgs e) =>
        MouseUpUnsharesRow(e);

    private void OnCellDataAreaMouseEnterInternal(int rowIndex)
    {
        Debug.Assert(DataGridView is not null);
        if (!DataGridView.ShowCellToolTips)
        {
            return;
        }

        // Don't show a tooltip for edited cells with an editing control
        Point ptCurrentCell = DataGridView.CurrentCellAddress;
        if (ptCurrentCell.X != -1 &&
            ptCurrentCell.X == ColumnIndex &&
            ptCurrentCell.Y == rowIndex &&
            DataGridView.EditingControl is not null)
        {
            Debug.Assert(DataGridView.IsCurrentCellInEditMode);
            return;
        }

        // get the tool tip string
        string? toolTipText = GetInternalToolTipText(rowIndex);

        if (string.IsNullOrEmpty(toolTipText))
        {
            if (FormattedValueType == s_stringType)
            {
                if (rowIndex != -1 && OwningRow is not null && OwningColumn is not null)
                {
                    int width = GetPreferredWidth(rowIndex, OwningRow.Height);
                    int height = GetPreferredHeight(rowIndex, OwningColumn.Width);

                    if (OwningColumn.Width < width || OwningRow.Height < height)
                    {
                        DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(inheritedCellStyle: null, rowIndex, includeColors: false);
                        string? editedFormattedValue = GetEditedFormattedValue(
                            GetValue(rowIndex),
                            rowIndex,
                            ref dataGridViewCellStyle,
                            DataGridViewDataErrorContexts.Display) as string;
                        if (!string.IsNullOrEmpty(editedFormattedValue))
                        {
                            toolTipText = TruncateToolTipText(editedFormattedValue);
                        }
                    }
                }
                else if ((rowIndex != -1 && OwningRow is not null && DataGridView.RowHeadersVisible && DataGridView.RowHeadersWidth > 0 && OwningColumn is null) ||
                         rowIndex == -1)
                {
                    // we are on a header cell.
                    Debug.Assert(this is DataGridViewHeaderCell);
                    string? stringValue = GetValue(rowIndex) as string;
                    if (!string.IsNullOrEmpty(stringValue))
                    {
                        DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(inheritedCellStyle: null, rowIndex, includeColors: false);

                        using var screen = GdiCache.GetScreenDCGraphics();
                        Rectangle contentBounds = GetContentBounds(screen, dataGridViewCellStyle, rowIndex);

                        bool widthTruncated = false;
                        int preferredHeight = 0;
                        if (contentBounds.Width > 0)
                        {
                            preferredHeight = GetPreferredTextHeight(
                                screen,
                                DataGridView.RightToLeftInternal,
                                stringValue,
                                dataGridViewCellStyle,
                                contentBounds.Width,
                                out widthTruncated);
                        }
                        else
                        {
                            widthTruncated = true;
                        }

                        if (preferredHeight > contentBounds.Height || widthTruncated)
                        {
                            toolTipText = TruncateToolTipText(stringValue);
                        }
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(toolTipText))
        {
            KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);
            DataGridView.ActivateToolTip(activate: true, toolTipText, ColumnIndex, rowIndex);
        }

        // for debugging
        // Console.WriteLine("OnCellDATA_AreaMouseENTER. ToolTipText : " + toolTipText);
    }

    private void OnCellDataAreaMouseLeaveInternal()
    {
        if (DataGridView is null || DataGridView.IsDisposed)
        {
            return;
        }

        DataGridView.ActivateToolTip(activate: false, string.Empty, -1, -1);
        // for debugging
        // Console.WriteLine("OnCellDATA_AreaMouseLEAVE");
    }

    private void OnCellErrorAreaMouseEnterInternal(int rowIndex)
    {
        string errorText = GetErrorText(rowIndex);
        Debug.Assert(!string.IsNullOrEmpty(errorText), "if we entered the cell error area then an error was painted, so we should have an error");
        KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);
        DataGridView?.ActivateToolTip(activate: true, errorText, ColumnIndex, rowIndex);

        // for debugging
        // Console.WriteLine("OnCellERROR_AreaMouseENTER. ErrorText : " + errorText);
    }

    private void OnCellErrorAreaMouseLeaveInternal()
    {
        DataGridView?.ActivateToolTip(activate: false, string.Empty, -1, -1);
        // for debugging
        // Console.WriteLine("OnCellERROR_AreaMouseLEAVE");
    }

    protected virtual void OnClick(DataGridViewCellEventArgs e)
    {
    }

    internal void OnClickInternal(DataGridViewCellEventArgs e) => OnClick(e);

    internal void OnCommonChange()
    {
        if (DataGridView is not null && !DataGridView.IsDisposed && !DataGridView.Disposing)
        {
            if (RowIndex == -1)
            {
                // Invalidate and autosize column
                DataGridView.OnColumnCommonChange(ColumnIndex);
            }
            else
            {
                // Invalidate and autosize cell
                DataGridView.OnCellCommonChange(ColumnIndex, RowIndex);
            }
        }
    }

    protected virtual void OnContentClick(DataGridViewCellEventArgs e)
    {
    }

    internal void OnContentClickInternal(DataGridViewCellEventArgs e) =>
        OnContentClick(e);

    protected virtual void OnContentDoubleClick(DataGridViewCellEventArgs e)
    {
    }

    internal void OnContentDoubleClickInternal(DataGridViewCellEventArgs e) =>
        OnContentDoubleClick(e);

    protected virtual void OnDoubleClick(DataGridViewCellEventArgs e)
    {
    }

    internal void OnDoubleClickInternal(DataGridViewCellEventArgs e) =>
        OnDoubleClick(e);

    protected virtual void OnEnter(int rowIndex, bool throughMouseClick)
    {
    }

    internal void OnEnterInternal(int rowIndex, bool throughMouseClick) =>
        OnEnter(rowIndex, throughMouseClick);

    internal void OnKeyDownInternal(KeyEventArgs e, int rowIndex) =>
        OnKeyDown(e, rowIndex);

    protected virtual void OnKeyDown(KeyEventArgs e, int rowIndex)
    {
    }

    internal void OnKeyPressInternal(KeyPressEventArgs e, int rowIndex) =>
        OnKeyPress(e, rowIndex);

    protected virtual void OnKeyPress(KeyPressEventArgs e, int rowIndex)
    {
    }

    protected virtual void OnKeyUp(KeyEventArgs e, int rowIndex)
    {
    }

    internal void OnKeyUpInternal(KeyEventArgs e, int rowIndex) =>
        OnKeyUp(e, rowIndex);

    protected virtual void OnLeave(int rowIndex, bool throughMouseClick)
    {
    }

    internal void OnLeaveInternal(int rowIndex, bool throughMouseClick) =>
        OnLeave(rowIndex, throughMouseClick);

    protected virtual void OnMouseClick(DataGridViewCellMouseEventArgs e)
    {
    }

    internal void OnMouseClickInternal(DataGridViewCellMouseEventArgs e) =>
        OnMouseClick(e);

    protected virtual void OnMouseDoubleClick(DataGridViewCellMouseEventArgs e)
    {
    }

    internal void OnMouseDoubleClickInternal(DataGridViewCellMouseEventArgs e) =>
        OnMouseDoubleClick(e);

    protected virtual void OnMouseDown(DataGridViewCellMouseEventArgs e)
    {
    }

    internal void OnMouseDownInternal(DataGridViewCellMouseEventArgs e)
    {
        if (DataGridView is null)
        {
            return;
        }

        DataGridView.CellMouseDownInContentBounds = GetContentBounds(e.RowIndex).Contains(e.X, e.Y);

        if (((ColumnIndex < 0 || e.RowIndex < 0) && DataGridView.ApplyVisualStylesToHeaderCells) ||
            ((ColumnIndex >= 0 && e.RowIndex >= 0) && DataGridView.ApplyVisualStylesToInnerCells))
        {
            DataGridView.InvalidateCell(ColumnIndex, e.RowIndex);
        }

        OnMouseDown(e);
    }

    protected virtual void OnMouseEnter(int rowIndex)
    {
    }

    internal void OnMouseEnterInternal(int rowIndex)
    {
        OnMouseEnter(rowIndex);
    }

    protected virtual void OnMouseLeave(int rowIndex)
    {
    }

    internal void OnMouseLeaveInternal(int rowIndex)
    {
        switch (CurrentMouseLocation)
        {
            case FlagDataArea:
                OnCellDataAreaMouseLeaveInternal();
                break;
            case FlagErrorArea:
                OnCellErrorAreaMouseLeaveInternal();
                break;
            case FlagAreaNotSet:
                break;
            default:
                Debug.Assert(false, "there are only three possible choices for the CurrentMouseLocation");
                break;
        }

        CurrentMouseLocation = FlagAreaNotSet;
        OnMouseLeave(rowIndex);
    }

    protected virtual void OnMouseMove(DataGridViewCellMouseEventArgs e)
    {
    }

    internal void OnMouseMoveInternal(DataGridViewCellMouseEventArgs e)
    {
        byte mouseLocation = CurrentMouseLocation;
        UpdateCurrentMouseLocation(e);
        Debug.Assert(CurrentMouseLocation != FlagAreaNotSet);
        switch (mouseLocation)
        {
            case FlagAreaNotSet:
                if (CurrentMouseLocation == FlagDataArea)
                {
                    OnCellDataAreaMouseEnterInternal(e.RowIndex);
                }
                else
                {
                    OnCellErrorAreaMouseEnterInternal(e.RowIndex);
                }

                break;
            case FlagDataArea:
                if (CurrentMouseLocation == FlagErrorArea)
                {
                    OnCellDataAreaMouseLeaveInternal();
                    OnCellErrorAreaMouseEnterInternal(e.RowIndex);
                }

                break;
            case FlagErrorArea:
                if (CurrentMouseLocation == FlagDataArea)
                {
                    OnCellErrorAreaMouseLeaveInternal();
                    OnCellDataAreaMouseEnterInternal(e.RowIndex);
                }

                break;
            default:
                Debug.Fail("there are only three choices for CurrentMouseLocation");
                break;
        }

        OnMouseMove(e);
    }

    protected virtual void OnMouseUp(DataGridViewCellMouseEventArgs e)
    {
    }

    internal void OnMouseUpInternal(DataGridViewCellMouseEventArgs e)
    {
        if (DataGridView is null)
        {
            return;
        }

        int x = e.X;
        int y = e.Y;

        if (((ColumnIndex < 0 || e.RowIndex < 0) && DataGridView.ApplyVisualStylesToHeaderCells) ||
            ((ColumnIndex >= 0 && e.RowIndex >= 0) && DataGridView.ApplyVisualStylesToInnerCells))
        {
            DataGridView.InvalidateCell(ColumnIndex, e.RowIndex);
        }

        if (e.Button == MouseButtons.Left && GetContentBounds(e.RowIndex).Contains(x, y))
        {
            DataGridView.OnCommonCellContentClick(e.ColumnIndex, e.RowIndex, e.Clicks > 1);
        }

        if (e.ColumnIndex < DataGridView.Columns.Count && e.RowIndex < DataGridView.Rows.Count)
        {
            OnMouseUp(e);
        }
    }

    protected override void OnDataGridViewChanged()
    {
        if (HasStyle)
        {
            if (DataGridView is null)
            {
                Style.RemoveScope(DataGridViewCellStyleScopes.Cell);
            }
            else
            {
                Style.AddScope(DataGridView, DataGridViewCellStyleScopes.Cell);
            }
        }

        base.OnDataGridViewChanged();
    }

    protected virtual void Paint(
        Graphics graphics,
        Rectangle clipBounds,
        Rectangle cellBounds,
        int rowIndex,
        DataGridViewElementStates cellState,
        object? value,
        object? formattedValue,
        string? errorText,
        DataGridViewCellStyle cellStyle,
        DataGridViewAdvancedBorderStyle advancedBorderStyle,
        DataGridViewPaintParts paintParts)
    {
    }

    internal void PaintInternal(
        Graphics graphics,
        Rectangle clipBounds,
        Rectangle cellBounds,
        int rowIndex,
        DataGridViewElementStates cellState,
        object? value,
        object? formattedValue,
        string? errorText,
        DataGridViewCellStyle cellStyle,
        DataGridViewAdvancedBorderStyle advancedBorderStyle,
        DataGridViewPaintParts paintParts)
    {
        Paint(
            graphics,
            clipBounds,
            cellBounds,
            rowIndex,
            cellState,
            value,
            formattedValue,
            errorText,
            cellStyle,
            advancedBorderStyle,
            paintParts);
    }

    internal static bool PaintBackground(DataGridViewPaintParts paintParts)
    {
        return (paintParts & DataGridViewPaintParts.Background) != 0;
    }

    internal static bool PaintBorder(DataGridViewPaintParts paintParts)
    {
        return (paintParts & DataGridViewPaintParts.Border) != 0;
    }

    protected virtual void PaintBorder(
        Graphics graphics,
        Rectangle clipBounds,
        Rectangle bounds,
        DataGridViewCellStyle cellStyle,
        DataGridViewAdvancedBorderStyle advancedBorderStyle)
    {
        ArgumentNullException.ThrowIfNull(graphics);
        ArgumentNullException.ThrowIfNull(cellStyle);
        ArgumentNullException.ThrowIfNull(advancedBorderStyle);

        if (DataGridView is null)
        {
            return;
        }

        // Using system colors for non-single grid colors for now
        int y1, y2;

        using var penBackColor = cellStyle.BackColor.GetCachedPenScope();
        using var penGridColor = DataGridView.GridPenColor.GetCachedPenScope();

        (Color darkColor, Color lightColor) = GetContrastedColors(cellStyle.BackColor);
        using var penControlDark = darkColor.GetCachedPenScope();
        using var penControlLightLight = lightColor.GetCachedPenScope();

        int dividerThickness = OwningColumn?.DividerWidth ?? 0;
        if (dividerThickness != 0)
        {
            if (dividerThickness > bounds.Width)
            {
                dividerThickness = bounds.Width;
            }

            Color dividerWidthColor = advancedBorderStyle.Right switch
            {
                DataGridViewAdvancedCellBorderStyle.Single => DataGridView.GridPenColor,
                DataGridViewAdvancedCellBorderStyle.Inset => Application.SystemColors.ControlLightLight,
                _ => Application.SystemColors.ControlDark,
            };

            using var dividerWidthBrush = dividerWidthColor.GetCachedSolidBrushScope();
            graphics.FillRectangle(
                dividerWidthBrush,
                DataGridView.RightToLeftInternal ? bounds.X : bounds.Right - dividerThickness,
                bounds.Y,
                dividerThickness,
                bounds.Height);

            if (DataGridView.RightToLeftInternal)
            {
                bounds.X += dividerThickness;
            }

            bounds.Width -= dividerThickness;
            if (bounds.Width <= 0)
            {
                return;
            }
        }

        dividerThickness = OwningRow?.DividerHeight ?? 0;
        if (dividerThickness != 0)
        {
            if (dividerThickness > bounds.Height)
            {
                dividerThickness = bounds.Height;
            }

            Color dividerHeightColor = advancedBorderStyle.Bottom switch
            {
                DataGridViewAdvancedCellBorderStyle.Single => DataGridView.GridPenColor,
                DataGridViewAdvancedCellBorderStyle.Inset => Application.SystemColors.ControlLightLight,
                _ => Application.SystemColors.ControlDark,
            };

            using var dividerHeightColorBrush = dividerHeightColor.GetCachedSolidBrushScope();
            graphics.FillRectangle(
                dividerHeightColorBrush,
                bounds.X, bounds.Bottom - dividerThickness, bounds.Width, dividerThickness);
            bounds.Height -= dividerThickness;
            if (bounds.Height <= 0)
            {
                return;
            }
        }

        if (advancedBorderStyle.All == DataGridViewAdvancedCellBorderStyle.None)
        {
            return;
        }

        switch (advancedBorderStyle.Left)
        {
            case DataGridViewAdvancedCellBorderStyle.Single:
                graphics.DrawLine(penGridColor, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                break;

            case DataGridViewAdvancedCellBorderStyle.Inset:
                graphics.DrawLine(penControlDark, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                break;

            case DataGridViewAdvancedCellBorderStyle.Outset:
                graphics.DrawLine(penControlLightLight, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                break;

            case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                y1 = bounds.Y + 2;
                y2 = bounds.Bottom - 3;
                if (advancedBorderStyle.Top is DataGridViewAdvancedCellBorderStyle.OutsetDouble or
                    DataGridViewAdvancedCellBorderStyle.InsetDouble)
                {
                    y1++;
                }
                else if (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None)
                {
                    y1--;
                }

                graphics.DrawLine(penBackColor, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                graphics.DrawLine(penControlLightLight, bounds.X, y1, bounds.X, y2);
                break;

            case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                y1 = bounds.Y + 1;
                y2 = bounds.Bottom - 1;
                if (advancedBorderStyle.Top is DataGridViewAdvancedCellBorderStyle.OutsetPartial or
                    DataGridViewAdvancedCellBorderStyle.None)
                {
                    y1--;
                }

                if (advancedBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.OutsetPartial)
                {
                    y2++;
                }

                graphics.DrawLine(penControlDark, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                graphics.DrawLine(penControlLightLight, bounds.X + 1, y1, bounds.X + 1, y2);
                break;

            case DataGridViewAdvancedCellBorderStyle.InsetDouble:
                y1 = bounds.Y + 1;
                y2 = bounds.Bottom - 1;
                if (advancedBorderStyle.Top is DataGridViewAdvancedCellBorderStyle.OutsetPartial or
                    DataGridViewAdvancedCellBorderStyle.None)
                {
                    y1--;
                }

                if (advancedBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.OutsetPartial)
                {
                    y2++;
                }

                graphics.DrawLine(penControlLightLight, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                graphics.DrawLine(penControlDark, bounds.X + 1, y1, bounds.X + 1, y2);
                break;
        }

        switch (advancedBorderStyle.Right)
        {
            case DataGridViewAdvancedCellBorderStyle.Single:
                graphics.DrawLine(penGridColor, bounds.Right - 1, bounds.Y, bounds.Right - 1, bounds.Bottom - 1);
                break;

            case DataGridViewAdvancedCellBorderStyle.Inset:
                graphics.DrawLine(penControlLightLight, bounds.Right - 1, bounds.Y, bounds.Right - 1, bounds.Bottom - 1);
                break;

            case DataGridViewAdvancedCellBorderStyle.Outset:
                graphics.DrawLine(penControlDark, bounds.Right - 1, bounds.Y, bounds.Right - 1, bounds.Bottom - 1);
                break;

            case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                y1 = bounds.Y + 2;
                y2 = bounds.Bottom - 3;
                if (advancedBorderStyle.Top is DataGridViewAdvancedCellBorderStyle.OutsetDouble or
                    DataGridViewAdvancedCellBorderStyle.InsetDouble)
                {
                    y1++;
                }
                else if (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None)
                {
                    y1--;
                }

                graphics.DrawLine(penBackColor, bounds.Right - 1, bounds.Y, bounds.Right - 1, bounds.Bottom - 1);
                graphics.DrawLine(penControlDark, bounds.Right - 1, y1, bounds.Right - 1, y2);
                break;

            case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                y1 = bounds.Y + 1;
                y2 = bounds.Bottom - 1;
                if (advancedBorderStyle.Top is DataGridViewAdvancedCellBorderStyle.OutsetPartial or
                    DataGridViewAdvancedCellBorderStyle.None)
                {
                    y1--;
                }

                if (advancedBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.OutsetPartial)
                {
                    y2++;
                }

                graphics.DrawLine(penControlDark, bounds.Right - 2, bounds.Y, bounds.Right - 2, bounds.Bottom - 1);
                graphics.DrawLine(penControlLightLight, bounds.Right - 1, y1, bounds.Right - 1, y2);
                break;

            case DataGridViewAdvancedCellBorderStyle.InsetDouble:
                y1 = bounds.Y + 1;
                y2 = bounds.Bottom - 1;
                if (advancedBorderStyle.Top is DataGridViewAdvancedCellBorderStyle.OutsetPartial or
                    DataGridViewAdvancedCellBorderStyle.None)
                {
                    y1--;
                }

                if (advancedBorderStyle.Bottom is DataGridViewAdvancedCellBorderStyle.OutsetPartial or
                    DataGridViewAdvancedCellBorderStyle.Inset)
                {
                    y2++;
                }

                graphics.DrawLine(penControlLightLight, bounds.Right - 2, bounds.Y, bounds.Right - 2, bounds.Bottom - 1);
                graphics.DrawLine(penControlDark, bounds.Right - 1, y1, bounds.Right - 1, y2);
                break;
        }

        int x1, x2;
        switch (advancedBorderStyle.Top)
        {
            case DataGridViewAdvancedCellBorderStyle.Single:
                graphics.DrawLine(penGridColor, bounds.X, bounds.Y, bounds.Right - 1, bounds.Y);
                break;

            case DataGridViewAdvancedCellBorderStyle.Inset:
                x1 = bounds.X;
                x2 = bounds.Right - 1;
                if (advancedBorderStyle.Left is DataGridViewAdvancedCellBorderStyle.OutsetDouble or
                    DataGridViewAdvancedCellBorderStyle.InsetDouble)
                {
                    x1++;
                }

                if (advancedBorderStyle.Right is DataGridViewAdvancedCellBorderStyle.Inset or
                    DataGridViewAdvancedCellBorderStyle.Outset)
                {
                    x2--;
                }

                graphics.DrawLine(penControlDark, x1, bounds.Y, x2, bounds.Y);
                break;

            case DataGridViewAdvancedCellBorderStyle.Outset:
                x1 = bounds.X;
                x2 = bounds.Right - 1;
                if (advancedBorderStyle.Left is DataGridViewAdvancedCellBorderStyle.OutsetDouble or
                    DataGridViewAdvancedCellBorderStyle.InsetDouble)
                {
                    x1++;
                }

                if (advancedBorderStyle.Right is DataGridViewAdvancedCellBorderStyle.Inset or
                    DataGridViewAdvancedCellBorderStyle.Outset)
                {
                    x2--;
                }

                graphics.DrawLine(penControlLightLight, x1, bounds.Y, x2, bounds.Y);
                break;

            case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                x1 = bounds.X;
                x2 = bounds.Right - 1;
                if (advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.None)
                {
                    x1++;
                    if (advancedBorderStyle.Left is DataGridViewAdvancedCellBorderStyle.OutsetDouble or
                        DataGridViewAdvancedCellBorderStyle.InsetDouble)
                    {
                        x1++;
                    }
                }

                if (advancedBorderStyle.Right != DataGridViewAdvancedCellBorderStyle.None)
                {
                    x2--;
                    if (advancedBorderStyle.Right is DataGridViewAdvancedCellBorderStyle.OutsetDouble or
                        DataGridViewAdvancedCellBorderStyle.InsetDouble)
                    {
                        x2--;
                    }
                }

                graphics.DrawLine(penBackColor, x1, bounds.Y, x2, bounds.Y);
                graphics.DrawLine(penControlLightLight, x1 + 1, bounds.Y, x2 - 1, bounds.Y);
                break;

            case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                x1 = bounds.X;
                if (advancedBorderStyle.Left is not DataGridViewAdvancedCellBorderStyle.OutsetPartial and
                    not DataGridViewAdvancedCellBorderStyle.None)
                {
                    x1++;
                }

                x2 = bounds.Right - 2;
                if (advancedBorderStyle.Right is DataGridViewAdvancedCellBorderStyle.OutsetPartial or
                    DataGridViewAdvancedCellBorderStyle.None)
                {
                    x2++;
                }

                graphics.DrawLine(penControlDark, bounds.X, bounds.Y, bounds.Right - 1, bounds.Y);
                graphics.DrawLine(penControlLightLight, x1, bounds.Y + 1, x2, bounds.Y + 1);
                break;

            case DataGridViewAdvancedCellBorderStyle.InsetDouble:
                x1 = bounds.X;
                if (advancedBorderStyle.Left is not DataGridViewAdvancedCellBorderStyle.OutsetPartial and
                    not DataGridViewAdvancedCellBorderStyle.None)
                {
                    x1++;
                }

                x2 = bounds.Right - 2;
                if (advancedBorderStyle.Right is DataGridViewAdvancedCellBorderStyle.OutsetPartial or
                    DataGridViewAdvancedCellBorderStyle.None)
                {
                    x2++;
                }

                graphics.DrawLine(penControlLightLight, bounds.X, bounds.Y, bounds.Right - 1, bounds.Y);
                graphics.DrawLine(penControlDark, x1, bounds.Y + 1, x2, bounds.Y + 1);
                break;
        }

        switch (advancedBorderStyle.Bottom)
        {
            case DataGridViewAdvancedCellBorderStyle.Single:
                graphics.DrawLine(penGridColor, bounds.X, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
                break;

            case DataGridViewAdvancedCellBorderStyle.Inset:
                x2 = bounds.Right - 1;
                if (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.InsetDouble)
                {
                    x2--;
                }

                graphics.DrawLine(penControlLightLight, bounds.X, bounds.Bottom - 1, x2, bounds.Bottom - 1);
                break;

            case DataGridViewAdvancedCellBorderStyle.Outset:
                x1 = bounds.X;
                x2 = bounds.Right - 1;
                if (advancedBorderStyle.Right is DataGridViewAdvancedCellBorderStyle.InsetDouble or
                    DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                {
                    x2--;
                }

                graphics.DrawLine(penControlDark, x1, bounds.Bottom - 1, x2, bounds.Bottom - 1);
                break;

            case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                x1 = bounds.X;
                x2 = bounds.Right - 1;
                if (advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.None)
                {
                    x1++;
                    if (advancedBorderStyle.Left is DataGridViewAdvancedCellBorderStyle.OutsetDouble or
                        DataGridViewAdvancedCellBorderStyle.InsetDouble)
                    {
                        x1++;
                    }
                }

                if (advancedBorderStyle.Right != DataGridViewAdvancedCellBorderStyle.None)
                {
                    x2--;
                    if (advancedBorderStyle.Right is DataGridViewAdvancedCellBorderStyle.OutsetDouble or
                        DataGridViewAdvancedCellBorderStyle.InsetDouble)
                    {
                        x2--;
                    }
                }

                graphics.DrawLine(penBackColor, x1, bounds.Bottom - 1, x2, bounds.Bottom - 1);
                graphics.DrawLine(penControlDark, x1 + 1, bounds.Bottom - 1, x2 - 1, bounds.Bottom - 1);
                break;
        }
    }

    internal static bool PaintContentBackground(DataGridViewPaintParts paintParts)
    {
        return (paintParts & DataGridViewPaintParts.ContentBackground) != 0;
    }

    internal static bool PaintContentForeground(DataGridViewPaintParts paintParts)
    {
        return (paintParts & DataGridViewPaintParts.ContentForeground) != 0;
    }

    protected virtual void PaintErrorIcon(
        Graphics graphics,
        Rectangle clipBounds,
        Rectangle cellValueBounds,
        string? errorText)
    {
        ArgumentNullException.ThrowIfNull(graphics);

        if (DataGridView is null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(errorText) &&
            cellValueBounds.Width >= IconMarginWidth * 2 + s_iconsWidth &&
            cellValueBounds.Height >= IconMarginHeight * 2 + s_iconsHeight)
        {
            PaintErrorIcon(graphics, ComputeErrorIconBounds(cellValueBounds));
        }
    }

    private static void PaintErrorIcon(Graphics graphics, Rectangle iconBounds)
    {
        ArgumentNullException.ThrowIfNull(graphics);

        s_errorBmp ??= GetBitmap("DataGridViewRow.error");

        lock (s_errorBmp)
        {
            graphics.DrawImage(s_errorBmp, iconBounds, 0, 0, s_iconsWidth, s_iconsHeight, GraphicsUnit.Pixel);
        }
    }

    internal void PaintErrorIcon(
        Graphics graphics,
        DataGridViewCellStyle cellStyle,
        int rowIndex,
        Rectangle cellBounds,
        Rectangle cellValueBounds,
        string? errorText)
    {
        if (!string.IsNullOrEmpty(errorText) &&
            cellValueBounds.Width >= IconMarginWidth * 2 + s_iconsWidth &&
            cellValueBounds.Height >= IconMarginHeight * 2 + s_iconsHeight)
        {
            Rectangle iconBounds = GetErrorIconBounds(graphics, cellStyle, rowIndex);
            if (iconBounds.Width >= IconMarginWidth && iconBounds.Height >= s_iconsHeight)
            {
                iconBounds.X += cellBounds.X;
                iconBounds.Y += cellBounds.Y;
                PaintErrorIcon(graphics, iconBounds);
            }
        }
    }

    internal static bool PaintErrorIcon(DataGridViewPaintParts paintParts)
    {
        return (paintParts & DataGridViewPaintParts.ErrorIcon) != 0;
    }

    internal static bool PaintFocus(DataGridViewPaintParts paintParts)
    {
        return (paintParts & DataGridViewPaintParts.Focus) != 0;
    }

    internal static void PaintPadding(
        Graphics graphics,
        Rectangle bounds,
        DataGridViewCellStyle cellStyle,
        Brush br,
        bool rightToLeft)
    {
        Rectangle rectPadding;
        if (rightToLeft)
        {
            rectPadding = new Rectangle(bounds.X, bounds.Y, cellStyle.Padding.Right, bounds.Height);
            graphics.FillRectangle(br, rectPadding);
            rectPadding.X = bounds.Right - cellStyle.Padding.Left;
            rectPadding.Width = cellStyle.Padding.Left;
            graphics.FillRectangle(br, rectPadding);
            rectPadding.X = bounds.Left + cellStyle.Padding.Right;
        }
        else
        {
            rectPadding = new Rectangle(bounds.X, bounds.Y, cellStyle.Padding.Left, bounds.Height);
            graphics.FillRectangle(br, rectPadding);
            rectPadding.X = bounds.Right - cellStyle.Padding.Right;
            rectPadding.Width = cellStyle.Padding.Right;
            graphics.FillRectangle(br, rectPadding);
            rectPadding.X = bounds.Left + cellStyle.Padding.Left;
        }

        rectPadding.Y = bounds.Y;
        rectPadding.Width = bounds.Width - cellStyle.Padding.Horizontal;
        rectPadding.Height = cellStyle.Padding.Top;
        graphics.FillRectangle(br, rectPadding);
        rectPadding.Y = bounds.Bottom - cellStyle.Padding.Bottom;
        rectPadding.Height = cellStyle.Padding.Bottom;
        graphics.FillRectangle(br, rectPadding);
    }

    internal static bool PaintSelectionBackground(DataGridViewPaintParts paintParts)
    {
        return (paintParts & DataGridViewPaintParts.SelectionBackground) != 0;
    }

    internal void PaintWork(Graphics graphics,
        Rectangle clipBounds,
        Rectangle cellBounds,
        int rowIndex,
        DataGridViewElementStates cellState,
        DataGridViewCellStyle cellStyle,
        DataGridViewAdvancedBorderStyle advancedBorderStyle,
        DataGridViewPaintParts paintParts)
    {
        Debug.Assert(DataGridView is not null);
        DataGridView dataGridView = DataGridView;
        int columnIndex = ColumnIndex;
        object? formattedValue;
        object? value = GetValue(rowIndex);
        string errorText = GetErrorText(rowIndex);
        if (columnIndex > -1 && rowIndex > -1)
        {
            formattedValue = GetEditedFormattedValue(
                value,
                rowIndex,
                ref cellStyle,
                DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.Display);
        }
        else
        {
            // No formatting applied on header cells.
            formattedValue = value;
        }

        DataGridViewCellPaintingEventArgs dgvcpe = dataGridView.CellPaintingEventArgs;
        dgvcpe.SetProperties(
            graphics,
            clipBounds,
            cellBounds,
            rowIndex,
            columnIndex,
            cellState,
            value,
            formattedValue,
            errorText,
            cellStyle,
            advancedBorderStyle,
            paintParts);
        dataGridView.OnCellPainting(dgvcpe);
        if (dgvcpe.Handled)
        {
            return;
        }

        Paint(
            graphics,
            clipBounds,
            cellBounds,
            rowIndex,
            cellState,
            value,
            formattedValue,
            errorText,
            cellStyle,
            advancedBorderStyle,
            paintParts);
    }

    public virtual object? ParseFormattedValue(
        object? formattedValue,
        DataGridViewCellStyle cellStyle,
        TypeConverter? formattedValueTypeConverter,
        TypeConverter? valueTypeConverter)
    {
        return ParseFormattedValueInternal(
            ValueType!,
            formattedValue,
            cellStyle,
            formattedValueTypeConverter,
            valueTypeConverter);
    }

    internal object? ParseFormattedValueInternal(
        Type valueType,
        object? formattedValue,
        DataGridViewCellStyle cellStyle,
        TypeConverter? formattedValueTypeConverter,
        TypeConverter? valueTypeConverter)
    {
        ArgumentNullException.ThrowIfNull(cellStyle);

        if (FormattedValueType is null)
        {
            throw new FormatException(SR.DataGridViewCell_FormattedValueTypeNull);
        }

        if (valueType is null)
        {
            throw new FormatException(SR.DataGridViewCell_ValueTypeNull);
        }

        if (formattedValue is null ||
            !FormattedValueType.IsAssignableFrom(formattedValue.GetType()))
        {
            throw new ArgumentException(SR.DataGridViewCell_FormattedValueHasWrongType, nameof(formattedValue));
        }

        return Formatter.ParseObject(
            formattedValue,
            valueType,
            FormattedValueType,
            valueTypeConverter ?? ValueTypeConverter,
            formattedValueTypeConverter ?? FormattedValueTypeConverter,
            cellStyle.FormatProvider,
            cellStyle.NullValue,
            cellStyle.IsDataSourceNullValueDefault ? Formatter.GetDefaultDataSourceNullValue(valueType) : cellStyle.DataSourceNullValue);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual void PositionEditingControl(
        bool setLocation,
        bool setSize,
        Rectangle cellBounds,
        Rectangle cellClip,
        DataGridViewCellStyle cellStyle,
        bool singleVerticalBorderAdded,
        bool singleHorizontalBorderAdded,
        bool isFirstDisplayedColumn,
        bool isFirstDisplayedRow)
    {
        Rectangle editingControlBounds = PositionEditingPanel(
            cellBounds,
            cellClip,
            cellStyle,
            singleVerticalBorderAdded,
            singleHorizontalBorderAdded,
            isFirstDisplayedColumn,
            isFirstDisplayedRow);

        if (DataGridView?.EditingControl is not null)
        {
            if (setLocation)
            {
                DataGridView.EditingControl.Location = new Point(editingControlBounds.X, editingControlBounds.Y);
            }

            if (setSize)
            {
                DataGridView.EditingControl.Size = new Size(editingControlBounds.Width, editingControlBounds.Height);
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    // Positions the editing panel and returns the normal bounds of the editing control, within the editing panel.
    public virtual Rectangle PositionEditingPanel(
        Rectangle cellBounds,
        Rectangle cellClip,
        DataGridViewCellStyle cellStyle,
        bool singleVerticalBorderAdded,
        bool singleHorizontalBorderAdded,
        bool isFirstDisplayedColumn,
        bool isFirstDisplayedRow)
    {
        if (DataGridView is null)
        {
            throw new InvalidOperationException();
        }

        DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder = new();

        DataGridViewAdvancedBorderStyle dgvabsEffective = AdjustCellBorderStyle(
            DataGridView.AdvancedCellBorderStyle,
            dataGridViewAdvancedBorderStylePlaceholder,
            singleVerticalBorderAdded,
            singleHorizontalBorderAdded,
            isFirstDisplayedColumn,
            isFirstDisplayedRow);

        Rectangle borderAndPaddingWidths = BorderWidths(dgvabsEffective);
        borderAndPaddingWidths.X += cellStyle.Padding.Left;
        borderAndPaddingWidths.Y += cellStyle.Padding.Top;
        borderAndPaddingWidths.Width += cellStyle.Padding.Right;
        borderAndPaddingWidths.Height += cellStyle.Padding.Bottom;
        int xEditingPanel, wEditingPanel = cellBounds.Width;
        int xEditingControl, wEditingControl;
        int yEditingPanel, hEditingPanel = cellBounds.Height;
        int yEditingControl, hEditingControl;

        if (cellClip.X - cellBounds.X >= borderAndPaddingWidths.X)
        {
            xEditingPanel = cellClip.X;
            wEditingPanel -= cellClip.X - cellBounds.X;
        }
        else
        {
            xEditingPanel = cellBounds.X + borderAndPaddingWidths.X;
            wEditingPanel -= borderAndPaddingWidths.X;
        }

        if (cellClip.Right <= cellBounds.Right - borderAndPaddingWidths.Width)
        {
            wEditingPanel -= cellBounds.Right - cellClip.Right;
        }
        else
        {
            wEditingPanel -= borderAndPaddingWidths.Width;
        }

        xEditingControl = cellBounds.X - cellClip.X;
        wEditingControl = cellBounds.Width - borderAndPaddingWidths.X - borderAndPaddingWidths.Width;
        if (cellClip.Y - cellBounds.Y >= borderAndPaddingWidths.Y)
        {
            yEditingPanel = cellClip.Y;
            hEditingPanel -= cellClip.Y - cellBounds.Y;
        }
        else
        {
            yEditingPanel = cellBounds.Y + borderAndPaddingWidths.Y;
            hEditingPanel -= borderAndPaddingWidths.Y;
        }

        if (cellClip.Bottom <= cellBounds.Bottom - borderAndPaddingWidths.Height)
        {
            hEditingPanel -= cellBounds.Bottom - cellClip.Bottom;
        }
        else
        {
            hEditingPanel -= borderAndPaddingWidths.Height;
        }

        yEditingControl = cellBounds.Y - cellClip.Y;
        hEditingControl = cellBounds.Height - borderAndPaddingWidths.Y - borderAndPaddingWidths.Height;
        DataGridView.EditingPanel.Location = new Point(xEditingPanel, yEditingPanel);
        DataGridView.EditingPanel.Size = new Size(wEditingPanel, hEditingPanel);
        return new Rectangle(xEditingControl, yEditingControl, wEditingControl, hEditingControl);
    }

    internal virtual void ReleaseUiaProvider()
    {
        if (!IsAccessibilityObjectCreated)
        {
            return;
        }

        PInvoke.UiaDisconnectProvider(AccessibilityObject);
        Properties.SetObject(s_propCellAccessibilityObject, null);
    }

    protected virtual bool SetValue(int rowIndex, object? value)
    {
        object? originalValue = null;
        DataGridView? dataGridView = DataGridView;
        if (dataGridView is not null && !dataGridView.InSortOperation)
        {
            originalValue = GetValue(rowIndex);
        }

        if (dataGridView is not null && OwningColumn is not null && OwningColumn.IsDataBound)
        {
            DataGridView.DataGridViewDataConnection? dataConnection = dataGridView.DataConnection;
            if (dataConnection is null)
            {
                return false;
            }
            else if ((dataConnection.CurrencyManager?.Count ?? 0) <= rowIndex)
            {
                if (value is not null || Properties.ContainsObject(s_propCellValue))
                {
                    Properties.SetObject(s_propCellValue, value);
                }
            }
            else
            {
                if (dataConnection.PushValue(OwningColumn.BoundColumnIndex, ColumnIndex, rowIndex, value))
                {
                    if (DataGridView is null || OwningRow is null || OwningRow.DataGridView is null)
                    {
                        // As a result of pushing the value in the back end, the data grid view row and/or data grid view cell
                        // became disconnected from the DataGridView.
                        // Return true because the operation succeeded.
                        // However, because the row which was edited became disconnected  from the DataGridView,
                        // do not mark the current row in the data grid view as being dirty.
                        // And because the data grid view cell which was edited became disconnected from the data grid view
                        // do not fire CellValueChanged event.
                        return true;
                    }

                    if (OwningRow.Index == DataGridView.CurrentCellAddress.Y)
                    {
                        // The user programmatically changed a value in the current row.
                        // The DataGridView already opened a transaction for the current row.
                        // All is left to do is to mark the current row in the DataGridView as being dirty.
                        DataGridView.IsCurrentRowDirtyInternal = true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        else if (dataGridView is null ||
            !dataGridView.VirtualMode ||
            rowIndex == -1 ||
            ColumnIndex == -1)
        {
            if (value is not null || Properties.ContainsObject(s_propCellValue))
            {
                Properties.SetObject(s_propCellValue, value);
            }
        }
        else
        {
            Debug.Assert(rowIndex >= 0);
            Debug.Assert(ColumnIndex >= 0);
            dataGridView.OnCellValuePushed(ColumnIndex, rowIndex, value);
        }

        if (dataGridView is not null &&
            !dataGridView.InSortOperation &&
            ((originalValue is null && value is not null) ||
             (originalValue is not null && value is null) ||
             (originalValue is not null && !originalValue.Equals(value))))
        {
            RaiseCellValueChanged(new DataGridViewCellEventArgs(ColumnIndex, rowIndex));
        }

        return true;
    }

    internal bool SetValueInternal(int rowIndex, object? value)
    {
        return SetValue(rowIndex, value);
    }

    internal static bool TextFitsInBounds(Graphics graphics, string text, Font font, Size maxBounds, TextFormatFlags flags)
    {
        int requiredHeight = MeasureTextHeight(graphics, text, font, maxBounds.Width, flags, out bool widthTruncated);
        return requiredHeight <= maxBounds.Height && !widthTruncated;
    }

    /// <summary>
    ///  Gets the row Index and column Index of the cell.
    /// </summary>
    public override string ToString()
    {
        return $"DataGridViewCell {{ ColumnIndex={ColumnIndex}, RowIndex={RowIndex} }}";
    }

    private static string TruncateToolTipText(string toolTipText)
    {
        if (toolTipText.Length > MaxToolTipLength)
        {
            return $"{toolTipText.AsSpan(0, MaxToolTipCutOff)}...";
        }

        return toolTipText;
    }

    private void UpdateCurrentMouseLocation(DataGridViewCellMouseEventArgs e)
    {
        if (GetErrorIconBounds(e.RowIndex).Contains(e.X, e.Y))
        {
            CurrentMouseLocation = FlagErrorArea;
        }
        else
        {
            CurrentMouseLocation = FlagDataArea;
        }
    }
}

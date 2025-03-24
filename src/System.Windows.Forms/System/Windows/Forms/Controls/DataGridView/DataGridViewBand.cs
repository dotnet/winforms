// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
///  Identifies a band or column in the dataGridView.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public class DataGridViewBand : DataGridViewElement, ICloneable, IDisposable
{
    private static readonly int s_propContextMenuStrip = PropertyStore.CreateKey();
    private static readonly int s_propDefaultCellStyle = PropertyStore.CreateKey();
    private static readonly int s_propDefaultHeaderCellType = PropertyStore.CreateKey();
    private static readonly int s_propDividerThickness = PropertyStore.CreateKey();
    private static readonly int s_propHeaderCell = PropertyStore.CreateKey();
    private static readonly int s_propUserData = PropertyStore.CreateKey();

    internal const int MinBandThickness = 2;
    internal const int MaxBandThickness = 65536;

    private int _thickness;
    private int _minimumThickness;

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataGridViewBand"/> class.
    /// </summary>
    internal DataGridViewBand()
    {
    }

    // NOTE: currently this finalizer is unneeded (empty). See https://github.com/dotnet/winforms/issues/6858.
    // All classes that are not need to be finalized must be checked in DataGridViewElement() constructor.
    // Consider to modify it if needed.
    ~DataGridViewBand() => Dispose(false);

    internal int CachedThickness { get; set; }

    [DefaultValue(null)]
    public virtual ContextMenuStrip? ContextMenuStrip
    {
        get
        {
            if (IsRow)
            {
                return ((DataGridViewRow)this).GetContextMenuStrip(Index);
            }

            return ContextMenuStripInternal;
        }
        set => ContextMenuStripInternal = value;
    }

    internal ContextMenuStrip? ContextMenuStripInternal
    {
        get => Properties.GetValueOrDefault<ContextMenuStrip>(s_propContextMenuStrip);
        set
        {
            ContextMenuStrip? oldValue = Properties.AddOrRemoveValue(s_propContextMenuStrip, value);
            if (oldValue != value)
            {
                if (oldValue is not null)
                {
                    oldValue.Disposed -= DetachContextMenuStrip;
                }

                if (value is not null)
                {
                    value.Disposed += DetachContextMenuStrip;
                }

                DataGridView?.OnBandContextMenuStripChanged(this);
            }
        }
    }

    [Browsable(false)]
    [AllowNull]
    public virtual DataGridViewCellStyle DefaultCellStyle
    {
        get
        {
            if (!Properties.TryGetValue(s_propDefaultCellStyle, out DataGridViewCellStyle? style))
            {
                style = new DataGridViewCellStyle();
                style.AddScope(DataGridView, IsRow ? DataGridViewCellStyleScopes.Row : DataGridViewCellStyleScopes.Column);
                Properties.AddValue(s_propDefaultCellStyle, style);
            }

            return style;
        }
        set
        {
            DataGridViewCellStyle? style = null;
            if (HasDefaultCellStyle)
            {
                style = DefaultCellStyle;
                style.RemoveScope(IsRow ? DataGridViewCellStyleScopes.Row : DataGridViewCellStyleScopes.Column);
            }

            value?.AddScope(DataGridView, IsRow ? DataGridViewCellStyleScopes.Row : DataGridViewCellStyleScopes.Column);
            Properties.AddOrRemoveValue(s_propDefaultCellStyle, value);

            if (DataGridView is not null &&
               ((style is not null ^ value is not null) ||
               (style is not null && value is not null && !style.Equals(DefaultCellStyle))))
            {
                DataGridView.OnBandDefaultCellStyleChanged(this);
            }
        }
    }

    [Browsable(false)]
    public Type DefaultHeaderCellType
    {
        get
        {
            if (Properties.TryGetValue(s_propDefaultHeaderCellType, out Type? type))
            {
                return type;
            }

            return IsRow ? typeof(DataGridViewRowHeaderCell) : typeof(DataGridViewColumnHeaderCell);
        }
        set
        {
            if (value is not null && !typeof(DataGridViewHeaderCell).IsAssignableFrom(value))
            {
                throw new ArgumentException(
                    string.Format(
                        SR.DataGridView_WrongType,
                        nameof(DefaultHeaderCellType),
                        "System.Windows.Forms.DataGridViewHeaderCell"),
                    nameof(value));
            }

            Properties.AddOrRemoveValue(s_propDefaultHeaderCellType, value);
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual bool Displayed
    {
        get
        {
            Debug.Assert(!IsRow);
            return (State & DataGridViewElementStates.Displayed) != 0;
        }
        internal set
        {
            Debug.Assert(value != Displayed);
            if (value)
            {
                State |= DataGridViewElementStates.Displayed;
            }
            else
            {
                State &= ~DataGridViewElementStates.Displayed;
            }

            if (DataGridView is not null)
            {
                OnStateChanged(DataGridViewElementStates.Displayed);
            }
        }
    }

    internal int DividerThickness
    {
        get => Properties.GetValueOrDefault<int>(s_propDividerThickness);
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxBandThickness);

            if (value != DividerThickness)
            {
                Properties.AddValue(s_propDividerThickness, value);
                DataGridView?.OnBandDividerThicknessChanged(this);
            }
        }
    }

    [DefaultValue(false)]
    public virtual bool Frozen
    {
        get
        {
            Debug.Assert(!IsRow);
            return (State & DataGridViewElementStates.Frozen) != 0;
        }
        set
        {
            if (((State & DataGridViewElementStates.Frozen) != 0) != value)
            {
                OnStateChanging(DataGridViewElementStates.Frozen);
                if (value)
                {
                    State |= DataGridViewElementStates.Frozen;
                }
                else
                {
                    State &= ~DataGridViewElementStates.Frozen;
                }

                OnStateChanged(DataGridViewElementStates.Frozen);
            }
        }
    }

    [Browsable(false)]
    public bool HasDefaultCellStyle => Properties.ContainsKey(s_propDefaultCellStyle);

    internal bool HasDefaultHeaderCellType => Properties.ContainsKey(s_propDefaultHeaderCellType);

    internal bool HasHeaderCell => Properties.ContainsKey(s_propHeaderCell);

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    protected DataGridViewHeaderCell HeaderCellCore
    {
        get
        {
            if (Properties.TryGetValue(s_propHeaderCell, out DataGridViewHeaderCell? headerCell))
            {
                return headerCell;
            }

            Type cellType = DefaultHeaderCellType;

            headerCell = (DataGridViewHeaderCell)Activator.CreateInstance(cellType)!;
            headerCell.DataGridView = DataGridView;

            if (IsRow)
            {
                headerCell.OwningRow = (DataGridViewRow)this;   // may be a shared row
                Properties.AddValue(s_propHeaderCell, headerCell);
            }
            else
            {
                DataGridViewColumn? dataGridViewColumn = this as DataGridViewColumn;
                headerCell.OwningColumn = dataGridViewColumn;

                // Set the headerCell in the property store before setting the SortOrder.
                Properties.AddValue(s_propHeaderCell, headerCell);
                if (DataGridView is not null && DataGridView.SortedColumn == dataGridViewColumn)
                {
                    DataGridViewColumnHeaderCell? dataGridViewColumnHeaderCell = headerCell as DataGridViewColumnHeaderCell;
                    Debug.Assert(dataGridViewColumnHeaderCell is not null);
                    dataGridViewColumnHeaderCell.SortGlyphDirection = DataGridView.SortOrder;
                }
            }

            return headerCell;
        }
        set
        {
            DataGridViewHeaderCell? priorValue = Properties.GetValueOrDefault<DataGridViewHeaderCell?>(s_propHeaderCell);

            if (priorValue is not null)
            {
                priorValue.DataGridView = null;
                if (IsRow)
                {
                    priorValue.OwningRow = null;
                }
                else
                {
                    priorValue.OwningColumn = null;
                    ((DataGridViewColumnHeaderCell)priorValue).SortGlyphDirectionInternal = SortOrder.None;
                }
            }

            if (value is not null)
            {
                if (IsRow)
                {
                    if (value is not DataGridViewRowHeaderCell)
                    {
                        throw new ArgumentException(
                            string.Format(
                                SR.DataGridView_WrongType,
                                nameof(DataGridViewRow.HeaderCell),
                                "System.Windows.Forms.DataGridViewRowHeaderCell"),
                            nameof(value));
                    }

                    // A HeaderCell can only be used by one band.
                    if (value.OwningRow is not null)
                    {
                        value.OwningRow.HeaderCell = null;
                    }

                    Debug.Assert(value.OwningRow is null);
                    value.OwningRow = (DataGridViewRow)this;   // may be a shared row
                }
                else
                {
                    if (value is not DataGridViewColumnHeaderCell dataGridViewColumnHeaderCell)
                    {
                        throw new ArgumentException(
                            string.Format(
                                SR.DataGridView_WrongType,
                                nameof(DataGridViewColumn.HeaderCell),
                                "System.Windows.Forms.DataGridViewColumnHeaderCell"),
                            nameof(value));
                    }

                    // A HeaderCell can only be used by one band.
                    if (value.OwningColumn is not null)
                    {
                        value.OwningColumn.HeaderCell = null;
                    }

                    Debug.Assert(dataGridViewColumnHeaderCell.SortGlyphDirection == SortOrder.None);
                    Debug.Assert(value.OwningColumn is null);
                    value.OwningColumn = (DataGridViewColumn)this;
                }

                Debug.Assert(value.DataGridView is null);
                value.DataGridView = DataGridView;
            }

            Properties.AddOrRemoveValue(s_propHeaderCell, value);

            if (DataGridView is not null && !Equals(priorValue, value))
            {
                DataGridView.OnBandHeaderCellChanged(this);
            }
        }
    }

    [Browsable(false)]
    public int Index { get; internal set; } = -1;

    [Browsable(false)]
    public virtual DataGridViewCellStyle? InheritedStyle => null;

    protected bool IsRow => this is DataGridViewRow;

    internal int MinimumThickness
    {
        get
        {
            if (IsRow && Index > -1)
            {
                GetHeightInfo(Index, out _, out int minimumHeight);
                return minimumHeight;
            }

            return _minimumThickness;
        }
        set
        {
            if (_minimumThickness != value)
            {
                if (value < MinBandThickness)
                {
                    if (IsRow)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.DataGridViewBand_MinimumHeightSmallerThanOne, MinBandThickness));
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.DataGridViewBand_MinimumWidthSmallerThanOne, MinBandThickness));
                    }
                }

                if (Thickness < value)
                {
                    // Force the new minimum width on potential auto fill column.
                    if (DataGridView is not null && !IsRow)
                    {
                        DataGridView.OnColumnMinimumWidthChanging((DataGridViewColumn)this, value);
                    }

                    Thickness = value;
                }

                _minimumThickness = value;
                DataGridView?.OnBandMinimumThicknessChanged(this);
            }
        }
    }

    /// <summary>
    ///  Contains all properties that are not always set.
    /// </summary>
    private protected PropertyStore Properties { get; private set; } = new();

    [DefaultValue(false)]
    public virtual bool ReadOnly
    {
        get
        {
            Debug.Assert(!IsRow);
            return ((State & DataGridViewElementStates.ReadOnly) != 0 ||
                (DataGridView is not null && DataGridView.ReadOnly));
        }
        set
        {
            if (DataGridView is not null)
            {
                if (DataGridView.ReadOnly)
                {
                    // if (!value): Trying to make a band read-write when the whole grid is read-only.
                    // if (value):  Trying to make a band read-only when the whole grid is read-only.
                    // Ignoring the request and returning.
                    return;
                }

                // this may trigger a call to set_ReadOnlyInternal
                if (IsRow)
                {
                    if (Index == -1)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, nameof(ReadOnly)));
                    }

                    OnStateChanging(DataGridViewElementStates.ReadOnly);
                    DataGridView.SetReadOnlyRowCore(Index, value);
                }
                else
                {
                    Debug.Assert(Index >= 0);
                    OnStateChanging(DataGridViewElementStates.ReadOnly);
                    DataGridView.SetReadOnlyColumnCore(Index, value);
                }
            }
            else
            {
                if (((State & DataGridViewElementStates.ReadOnly) != 0) != value)
                {
                    if (value)
                    {
                        if (IsRow)
                        {
                            foreach (DataGridViewCell? dataGridViewCell in ((DataGridViewRow)this).Cells)
                            {
                                if (dataGridViewCell!.ReadOnly)
                                {
                                    dataGridViewCell.ReadOnlyInternal = false;
                                }
                            }
                        }

                        State |= DataGridViewElementStates.ReadOnly;
                    }
                    else
                    {
                        State &= ~DataGridViewElementStates.ReadOnly;
                    }
                }
            }
        }
    }

    internal bool ReadOnlyInternal
    {
        set
        {
            Debug.Assert(value != ReadOnly);
            if (value)
            {
                State |= DataGridViewElementStates.ReadOnly;
            }
            else
            {
                State &= ~DataGridViewElementStates.ReadOnly;
            }

            Debug.Assert(DataGridView is not null);
            OnStateChanged(DataGridViewElementStates.ReadOnly);
        }
    }

    [Browsable(true)]
    public virtual DataGridViewTriState Resizable
    {
        get
        {
            Debug.Assert(!IsRow);
            if ((State & DataGridViewElementStates.ResizableSet) != 0)
            {
                return ((State & DataGridViewElementStates.Resizable) != 0) ? DataGridViewTriState.True : DataGridViewTriState.False;
            }

            if (DataGridView is null)
            {
                return DataGridViewTriState.NotSet;
            }

            return DataGridView.AllowUserToResizeColumns ? DataGridViewTriState.True : DataGridViewTriState.False;
        }
        set
        {
            if (!Enum.IsDefined(value))
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewTriState));
            }

            DataGridViewTriState oldResizable = Resizable;
            if (value == DataGridViewTriState.NotSet)
            {
                State &= ~DataGridViewElementStates.ResizableSet;
            }
            else
            {
                State |= DataGridViewElementStates.ResizableSet;
                if (((State & DataGridViewElementStates.Resizable) != 0) != (value == DataGridViewTriState.True))
                {
                    if (value == DataGridViewTriState.True)
                    {
                        State |= DataGridViewElementStates.Resizable;
                    }
                    else
                    {
                        State &= ~DataGridViewElementStates.Resizable;
                    }
                }
            }

            if (oldResizable != Resizable)
            {
                OnStateChanged(DataGridViewElementStates.Resizable);
            }
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual bool Selected
    {
        get
        {
            Debug.Assert(!IsRow);
            return (State & DataGridViewElementStates.Selected) != 0;
        }
        set
        {
            if (DataGridView is not null)
            {
                // this may trigger a call to set_SelectedInternal
                if (IsRow)
                {
                    if (Index == -1)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, nameof(Selected)));
                    }

                    if (DataGridView.SelectionMode is DataGridViewSelectionMode.FullRowSelect or DataGridViewSelectionMode.RowHeaderSelect)
                    {
                        DataGridView.SetSelectedRowCoreInternal(Index, value);
                    }
                }
                else
                {
                    Debug.Assert(Index >= 0);
                    if (DataGridView.SelectionMode is DataGridViewSelectionMode.FullColumnSelect or DataGridViewSelectionMode.ColumnHeaderSelect)
                    {
                        DataGridView.SetSelectedColumnCoreInternal(Index, value);
                    }
                }
            }
            else if (value)
            {
                // We do not allow the selection of a band before it gets added to the dataGridView.
                throw new InvalidOperationException(SR.DataGridViewBand_CannotSelect);
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

            if (DataGridView is not null)
            {
                OnStateChanged(DataGridViewElementStates.Selected);
            }
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? Tag
    {
        get => Properties.GetValueOrDefault<object?>(s_propUserData);
        set => Properties.AddOrRemoveValue(s_propUserData, value);
    }

    internal int Thickness
    {
        get
        {
            if (IsRow && Index > -1)
            {
                GetHeightInfo(Index, out int height, out _);
                return height;
            }

            return _thickness;
        }
        set
        {
            int minimumThickness = MinimumThickness;
            if (value < minimumThickness)
            {
                value = minimumThickness;
            }

            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxBandThickness);

            bool setThickness = true;
            if (IsRow)
            {
                if (DataGridView is not null && DataGridView.AutoSizeRowsMode != DataGridViewAutoSizeRowsMode.None)
                {
                    CachedThickness = value;
                    setThickness = false;
                }
            }
            else
            {
                DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)this;
                DataGridViewAutoSizeColumnMode inheritedAutoSizeMode = dataGridViewColumn.InheritedAutoSizeMode;
                if (inheritedAutoSizeMode is not DataGridViewAutoSizeColumnMode.Fill and
                    not DataGridViewAutoSizeColumnMode.None and
                    not DataGridViewAutoSizeColumnMode.NotSet)
                {
                    CachedThickness = value;
                    setThickness = false;
                }
                else if (inheritedAutoSizeMode == DataGridViewAutoSizeColumnMode.Fill && DataGridView is not null)
                {
                    if (dataGridViewColumn.Visible)
                    {
                        _ = DataGridView.Handle;
                        DataGridView.AdjustFillingColumn(dataGridViewColumn, value);
                        setThickness = false;
                    }
                }
            }

            if (setThickness && _thickness != value)
            {
                DataGridView?.OnBandThicknessChanging();
                ThicknessInternal = value;
            }
        }
    }

    internal int ThicknessInternal
    {
        get => _thickness;
        set
        {
            Debug.Assert(_thickness != value);
            Debug.Assert(value >= _minimumThickness);
            Debug.Assert(value <= MaxBandThickness);

            _thickness = value;
            DataGridView?.OnBandThicknessChanged(this);
        }
    }

    [DefaultValue(true)]
    public virtual bool Visible
    {
        get
        {
            Debug.Assert(!IsRow);
            return (State & DataGridViewElementStates.Visible) != 0;
        }
        set
        {
            if (((State & DataGridViewElementStates.Visible) != 0) != value)
            {
                if (DataGridView is not null &&
                    IsRow &&
                    DataGridView.NewRowIndex != -1 &&
                    DataGridView.NewRowIndex == Index &&
                    !value)
                {
                    // the 'new' row cannot be made invisible.
                    throw new InvalidOperationException(SR.DataGridViewBand_NewRowCannotBeInvisible);
                }

                OnStateChanging(DataGridViewElementStates.Visible);
                if (value)
                {
                    State |= DataGridViewElementStates.Visible;
                }
                else
                {
                    State &= ~DataGridViewElementStates.Visible;
                }

                OnStateChanged(DataGridViewElementStates.Visible);
            }
        }
    }

    public virtual object Clone()
    {
        DataGridViewBand band = (DataGridViewBand)Activator.CreateInstance(GetType())!;
        CloneInternal(band);
        return band;
    }

    private protected void CloneInternal(DataGridViewBand dataGridViewBand)
    {
        dataGridViewBand.Properties = new PropertyStore();
        dataGridViewBand.Index = -1;
        if (!IsRow || Index >= 0 || DataGridView is null)
        {
            dataGridViewBand.State = State & ~(DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed);
        }

        dataGridViewBand._thickness = Thickness;
        dataGridViewBand.MinimumThickness = MinimumThickness;
        dataGridViewBand.CachedThickness = CachedThickness;
        dataGridViewBand.DividerThickness = DividerThickness;
        dataGridViewBand.Tag = Tag;
        if (HasDefaultCellStyle)
        {
            dataGridViewBand.DefaultCellStyle = new DataGridViewCellStyle(DefaultCellStyle);
        }

        if (HasDefaultHeaderCellType)
        {
            dataGridViewBand.DefaultHeaderCellType = DefaultHeaderCellType;
        }

        if (ContextMenuStripInternal is not null)
        {
            dataGridViewBand.ContextMenuStrip = ContextMenuStripInternal.Clone();
        }
    }

    private void DetachContextMenuStrip(object? sender, EventArgs e)
    {
        ContextMenuStripInternal = null;
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
                contextMenuStrip.Disposed -= DetachContextMenuStrip;
            }
        }

        // If you are adding releasing unmanaged resources code here (disposing == false),
        // you need to remove this class type (and all of its subclasses) from check in DataGridViewElement() constructor
        // and DataGridViewElement_Subclasses_SuppressFinalizeCall test!
        // Also consider to modify ~DataGridViewBand() description.
    }

    internal void GetHeightInfo(int rowIndex, out int height, out int minimumHeight)
    {
        Debug.Assert(IsRow);
        if (DataGridView is not null &&
            (DataGridView.VirtualMode || DataGridView.DataSource is not null) &&
            DataGridView.AutoSizeRowsMode == DataGridViewAutoSizeRowsMode.None)
        {
            Debug.Assert(rowIndex > -1);
            DataGridViewRowHeightInfoNeededEventArgs e = DataGridView.OnRowHeightInfoNeeded(rowIndex, _thickness, _minimumThickness);
            height = e.Height;
            minimumHeight = e.MinimumHeight;
            return;
        }

        height = _thickness;
        minimumHeight = _minimumThickness;
    }

    private void OnStateChanged(DataGridViewElementStates elementState)
    {
        if (DataGridView is not null)
        {
            // maybe move this code into OnDataGridViewElementStateChanged
            if (IsRow)
            {
                // we could be smarter about what needs to be invalidated.
                DataGridView.Rows.InvalidateCachedRowCount(elementState);
                DataGridView.Rows.InvalidateCachedRowsHeight(elementState);
                if (Index != -1)
                {
                    DataGridView.OnDataGridViewElementStateChanged(this, -1, elementState);
                }
            }
            else
            {
                // we could be smarter about what needs to be invalidated.
                DataGridView.Columns.InvalidateCachedColumnCount(elementState);
                DataGridView.Columns.InvalidateCachedColumnsWidth(elementState);
                DataGridView.OnDataGridViewElementStateChanged(this, -1, elementState);
            }
        }
    }

    private void OnStateChanging(DataGridViewElementStates elementState)
    {
        if (DataGridView is not null)
        {
            if (IsRow)
            {
                if (Index != -1)
                {
                    DataGridView.OnDataGridViewElementStateChanging(this, -1, elementState);
                }
            }
            else
            {
                DataGridView.OnDataGridViewElementStateChanging(this, -1, elementState);
            }
        }
    }

    protected override void OnDataGridViewChanged()
    {
        if (HasDefaultCellStyle)
        {
            if (DataGridView is null)
            {
                DefaultCellStyle.RemoveScope(IsRow ? DataGridViewCellStyleScopes.Row : DataGridViewCellStyleScopes.Column);
            }
            else
            {
                DefaultCellStyle.AddScope(DataGridView, IsRow ? DataGridViewCellStyleScopes.Row : DataGridViewCellStyleScopes.Column);
            }
        }

        base.OnDataGridViewChanged();
    }

    private bool ShouldSerializeDefaultHeaderCellType() => Properties.ContainsKey(s_propDefaultHeaderCellType);

    // internal because DataGridViewColumn needs to access it
    internal bool ShouldSerializeResizable() => (State & DataGridViewElementStates.ResizableSet) != 0;

    public override string ToString() => $"DataGridViewBand {{ Index={Index} }}";
}

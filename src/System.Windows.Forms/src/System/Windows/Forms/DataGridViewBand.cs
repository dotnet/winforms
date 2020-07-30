// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Identifies a band or column in the dataGridView.
    /// </summary>
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
        ///  Initializes a new instance of the <see cref='DataGridViewBand'/> class.
        /// </summary>
        internal DataGridViewBand()
        {
        }

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
            get => (ContextMenuStrip?)Properties.GetObject(s_propContextMenuStrip);
            set
            {
                ContextMenuStrip? oldValue = (ContextMenuStrip?)Properties.GetObject(s_propContextMenuStrip);
                if (oldValue != value)
                {
                    EventHandler disposedHandler = new EventHandler(DetachContextMenuStrip);
                    if (oldValue != null)
                    {
                        oldValue.Disposed -= disposedHandler;
                    }

                    Properties.SetObject(s_propContextMenuStrip, value);
                    if (value != null)
                    {
                        value.Disposed += disposedHandler;
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
                DataGridViewCellStyle? style = (DataGridViewCellStyle?)Properties.GetObject(s_propDefaultCellStyle);
                if (style is null)
                {
                    style = new DataGridViewCellStyle();
                    style.AddScope(DataGridView, IsRow ? DataGridViewCellStyleScopes.Row : DataGridViewCellStyleScopes.Column);
                    Properties.SetObject(s_propDefaultCellStyle, style);
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
                if (value != null || Properties.ContainsObject(s_propDefaultCellStyle))
                {
                    value?.AddScope(DataGridView, IsRow ? DataGridViewCellStyleScopes.Row : DataGridViewCellStyleScopes.Column);
                    Properties.SetObject(s_propDefaultCellStyle, value);
                }

                if (DataGridView != null &&
                   ((style != null ^ value != null) ||
                   (style != null && value != null && !style.Equals(DefaultCellStyle))))
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
                Type? type = (Type?)Properties.GetObject(s_propDefaultHeaderCellType);
                if (type != null)
                {
                    return type;
                }

                if (IsRow)
                {
                    return typeof(DataGridViewRowHeaderCell);
                }
                else
                {
                    return typeof(DataGridViewColumnHeaderCell);
                }
            }
            set
            {
                if (value != null || Properties.ContainsObject(s_propDefaultHeaderCellType))
                {
                    if (!typeof(DataGridViewHeaderCell).IsAssignableFrom(value))
                    {
                        throw new ArgumentException(string.Format(SR.DataGridView_WrongType, nameof(DefaultHeaderCellType), "System.Windows.Forms.DataGridViewHeaderCell"), nameof(value));
                    }

                    Properties.SetObject(s_propDefaultHeaderCellType, value);
                }
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
                    State = State | DataGridViewElementStates.Displayed;
                }
                else
                {
                    State = State & ~DataGridViewElementStates.Displayed;
                }
                if (DataGridView != null)
                {
                    OnStateChanged(DataGridViewElementStates.Displayed);
                }
            }
        }

        internal int DividerThickness
        {
            get
            {
                int dividerThickness = Properties.GetInteger(s_propDividerThickness, out bool found);
                return found ? dividerThickness : 0;
            }
            set
            {
                if (value < 0)
                {
                    if (IsRow)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidLowBoundArgumentEx, nameof(DataGridViewRow.DividerHeight), value, 0));
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidLowBoundArgumentEx, nameof(DataGridViewColumn.DividerWidth), value, 0));
                    }
                }
                if (value > MaxBandThickness)
                {
                    if (IsRow)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidHighBoundArgumentEx, nameof(DataGridViewRow.DividerHeight), value, MaxBandThickness));
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidHighBoundArgumentEx, nameof(DataGridViewColumn.DividerWidth), value, MaxBandThickness));
                    }
                }

                if (value != DividerThickness)
                {
                    Properties.SetInteger(s_propDividerThickness, value);
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
                        State = State | DataGridViewElementStates.Frozen;
                    }
                    else
                    {
                        State = State & ~DataGridViewElementStates.Frozen;
                    }
                    OnStateChanged(DataGridViewElementStates.Frozen);
                }
            }
        }

        [Browsable(false)]
        public bool HasDefaultCellStyle
        {
            get => Properties.ContainsObject(s_propDefaultCellStyle) && Properties.GetObject(s_propDefaultCellStyle) != null;
        }

        internal bool HasDefaultHeaderCellType
        {
            get => Properties.ContainsObject(s_propDefaultHeaderCellType) && Properties.GetObject(s_propDefaultHeaderCellType) != null;
        }

        internal bool HasHeaderCell
        {
            get => Properties.ContainsObject(s_propHeaderCell) && Properties.GetObject(s_propHeaderCell) != null;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [AllowNull]
        protected DataGridViewHeaderCell HeaderCellCore
        {
            get
            {
                DataGridViewHeaderCell? headerCell = (DataGridViewHeaderCell?)Properties.GetObject(s_propHeaderCell);
                if (headerCell is null)
                {
                    Type cellType = DefaultHeaderCellType;

                    headerCell = (DataGridViewHeaderCell)Activator.CreateInstance(cellType)!;
                    headerCell.DataGridView = DataGridView;
                    if (IsRow)
                    {
                        headerCell.OwningRow = (DataGridViewRow)this;   // may be a shared row
                        Properties.SetObject(s_propHeaderCell, headerCell);
                    }
                    else
                    {
                        DataGridViewColumn? dataGridViewColumn = this as DataGridViewColumn;
                        headerCell.OwningColumn = dataGridViewColumn;
                        // Set the headerCell in the property store before setting the SortOrder.
                        Properties.SetObject(s_propHeaderCell, headerCell);
                        if (DataGridView != null && DataGridView.SortedColumn == dataGridViewColumn)
                        {
                            DataGridViewColumnHeaderCell? dataGridViewColumnHeaderCell = headerCell as DataGridViewColumnHeaderCell;
                            Debug.Assert(dataGridViewColumnHeaderCell != null);
                            dataGridViewColumnHeaderCell.SortGlyphDirection = DataGridView.SortOrder;
                        }
                    }
                }

                return headerCell;
            }
            set
            {
                DataGridViewHeaderCell? headerCell = (DataGridViewHeaderCell?)Properties.GetObject(s_propHeaderCell);
                if (value != null || Properties.ContainsObject(s_propHeaderCell))
                {
                    if (headerCell != null)
                    {
                        headerCell.DataGridView = null;
                        if (IsRow)
                        {
                            headerCell.OwningRow = null;
                        }
                        else
                        {
                            headerCell.OwningColumn = null;
                            ((DataGridViewColumnHeaderCell)headerCell).SortGlyphDirectionInternal = SortOrder.None;
                        }
                    }

                    if (value != null)
                    {
                        if (IsRow)
                        {
                            if (!(value is DataGridViewRowHeaderCell))
                            {
                                throw new ArgumentException(string.Format(SR.DataGridView_WrongType, nameof(DataGridViewRow.HeaderCell), "System.Windows.Forms.DataGridViewRowHeaderCell"), nameof(value));
                            }

                            // A HeaderCell can only be used by one band.
                            if (value.OwningRow != null)
                            {
                                value.OwningRow.HeaderCell = null;
                            }
                            Debug.Assert(value.OwningRow is null);
                            value.OwningRow = (DataGridViewRow)this;   // may be a shared row
                        }
                        else
                        {
                            if (!(value is DataGridViewColumnHeaderCell dataGridViewColumnHeaderCell))
                            {
                                throw new ArgumentException(string.Format(SR.DataGridView_WrongType, nameof(DataGridViewColumn.HeaderCell), "System.Windows.Forms.DataGridViewColumnHeaderCell"), nameof(value));
                            }

                            // A HeaderCell can only be used by one band.
                            if (value.OwningColumn != null)
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

                    Properties.SetObject(s_propHeaderCell, value);
                }
                if (((value is null && headerCell != null) || (value != null && headerCell is null) || (value != null && headerCell != null && !headerCell.Equals(value))) && DataGridView != null)
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
                    GetHeightInfo(Index, out int height, out int minimumHeight);
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
                            throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.DataGridViewBand_MinimumHeightSmallerThanOne, DataGridViewBand.MinBandThickness));
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.DataGridViewBand_MinimumWidthSmallerThanOne, DataGridViewBand.MinBandThickness));
                        }
                    }

                    if (Thickness < value)
                    {
                        // Force the new minimum width on potential auto fill column.
                        if (DataGridView != null && !IsRow)
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
        private protected PropertyStore Properties { get; private set; } = new PropertyStore();

        [DefaultValue(false)]
        public virtual bool ReadOnly
        {
            get
            {
                Debug.Assert(!IsRow);
                return ((State & DataGridViewElementStates.ReadOnly) != 0 ||
                    (DataGridView != null && DataGridView.ReadOnly));
            }
            set
            {
                if (DataGridView != null)
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
                            State = State | DataGridViewElementStates.ReadOnly;
                        }
                        else
                        {
                            State = State & ~DataGridViewElementStates.ReadOnly;
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
                    State = State | DataGridViewElementStates.ReadOnly;
                }
                else
                {
                    State = State & ~DataGridViewElementStates.ReadOnly;
                }

                Debug.Assert(DataGridView != null);
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
                if (!Enum.IsDefined(typeof(DataGridViewTriState), value))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewTriState));
                }

                DataGridViewTriState oldResizable = Resizable;
                if (value == DataGridViewTriState.NotSet)
                {
                    State = State & ~DataGridViewElementStates.ResizableSet;
                }
                else
                {
                    State = State | DataGridViewElementStates.ResizableSet;
                    if (((State & DataGridViewElementStates.Resizable) != 0) != (value == DataGridViewTriState.True))
                    {
                        if (value == DataGridViewTriState.True)
                        {
                            State = State | DataGridViewElementStates.Resizable;
                        }
                        else
                        {
                            State = State & ~DataGridViewElementStates.Resizable;
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
                if (DataGridView != null)
                {
                    // this may trigger a call to set_SelectedInternal
                    if (IsRow)
                    {
                        if (Index == -1)
                        {
                            throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, nameof(Selected)));
                        }
                        if (DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect || DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
                        {
                            DataGridView.SetSelectedRowCoreInternal(Index, value);
                        }
                    }
                    else
                    {
                        Debug.Assert(Index >= 0);
                        if (DataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect || DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
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
                    State = State | DataGridViewElementStates.Selected;
                }
                else
                {
                    State = State & ~DataGridViewElementStates.Selected;
                }

                if (DataGridView != null)
                {
                    OnStateChanged(DataGridViewElementStates.Selected);
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object? Tag
        {
            get => Properties.GetObject(s_propUserData);
            set
            {
                if (value != null || Properties.ContainsObject(s_propUserData))
                {
                    Properties.SetObject(s_propUserData, value);
                }
            }
        }

        internal int Thickness
        {
            get
            {
                if (IsRow && Index > -1)
                {
                    GetHeightInfo(Index, out int height, out int minimumHeight);
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
                if (value > MaxBandThickness)
                {
                    if (IsRow)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidHighBoundArgumentEx, nameof(DataGridViewRow.Height), value, MaxBandThickness));
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidHighBoundArgumentEx, nameof(DataGridViewColumn.Width), value, MaxBandThickness));
                    }
                }

                bool setThickness = true;
                if (IsRow)
                {
                    if (DataGridView != null && DataGridView.AutoSizeRowsMode != DataGridViewAutoSizeRowsMode.None)
                    {
                        CachedThickness = value;
                        setThickness = false;
                    }
                }
                else
                {
                    DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)this;
                    DataGridViewAutoSizeColumnMode inheritedAutoSizeMode = dataGridViewColumn.InheritedAutoSizeMode;
                    if (inheritedAutoSizeMode != DataGridViewAutoSizeColumnMode.Fill &&
                        inheritedAutoSizeMode != DataGridViewAutoSizeColumnMode.None &&
                        inheritedAutoSizeMode != DataGridViewAutoSizeColumnMode.NotSet)
                    {
                        CachedThickness = value;
                        setThickness = false;
                    }
                    else if (inheritedAutoSizeMode == DataGridViewAutoSizeColumnMode.Fill && DataGridView != null)
                    {
                        if (dataGridViewColumn.Visible)
                        {
                            IntPtr handle = DataGridView.Handle;
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
                    if (DataGridView != null &&
                        IsRow &&
                        DataGridView.NewRowIndex != -1 &&
                        DataGridView.NewRowIndex == Index &&
                        !value)
                    {
                        // the 'new' row cannot be made invisble.
                        throw new InvalidOperationException(SR.DataGridViewBand_NewRowCannotBeInvisible);
                    }

                    OnStateChanging(DataGridViewElementStates.Visible);
                    if (value)
                    {
                        State = State | DataGridViewElementStates.Visible;
                    }
                    else
                    {
                        State = State & ~DataGridViewElementStates.Visible;
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
            if (ContextMenuStripInternal != null)
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
                if (contextMenuStrip != null)
                {
                    contextMenuStrip.Disposed -= new EventHandler(DetachContextMenuStrip);
                }
            }
        }

        internal void GetHeightInfo(int rowIndex, out int height, out int minimumHeight)
        {
            Debug.Assert(IsRow);
            if (DataGridView != null &&
                (DataGridView.VirtualMode || DataGridView.DataSource != null) &&
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
            if (DataGridView != null)
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
            if (DataGridView != null)
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

        private bool ShouldSerializeDefaultHeaderCellType()
        {
            return Properties.GetObject(s_propDefaultHeaderCellType) != null;
        }

        // internal because DataGridViewColumn needs to access it
        internal bool ShouldSerializeResizable()
        {
            return (State & DataGridViewElementStates.ResizableSet) != 0;
        }

        public override string ToString()
        {
            var builder = new StringBuilder(36);
            builder.Append("DataGridViewBand { Index=");
            builder.Append(Index);
            builder.Append(" }");
            return builder.ToString();
        }
    }
}

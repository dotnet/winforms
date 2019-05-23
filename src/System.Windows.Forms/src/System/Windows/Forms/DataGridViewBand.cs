// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace System.Windows.Forms
{
    /// <summary>
    /// Identifies a band or column in the dataGridView.
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

        // Contains all properties that are not always set.
        private PropertyStore _propertyStore;
        private int _thickness;
        private int _minimumThickness;
        private int _bandIndex;
        internal bool _bandIsRow;

        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.DataGridViewBand'/> class.
        /// </summary>
        internal DataGridViewBand()
        {
            _propertyStore = new PropertyStore();
            _bandIndex = -1;
        }

        ~DataGridViewBand() => Dispose(false);

        internal int CachedThickness { get; set; }

        [DefaultValue(null)]
        public virtual ContextMenuStrip ContextMenuStrip
        {
            get
            {
                if (_bandIsRow)
                {
                    return ((DataGridViewRow)this).GetContextMenuStrip(Index);
                }

                return ContextMenuStripInternal;
            }
            set => ContextMenuStripInternal = value;
        }

        internal ContextMenuStrip ContextMenuStripInternal
        {
            get => (ContextMenuStrip)Properties.GetObject(s_propContextMenuStrip);
            set
            {
                ContextMenuStrip oldValue = (ContextMenuStrip)Properties.GetObject(s_propContextMenuStrip);
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
        public virtual DataGridViewCellStyle DefaultCellStyle
        {
            get
            {
                DataGridViewCellStyle style = (DataGridViewCellStyle)Properties.GetObject(s_propDefaultCellStyle);
                if (style == null)
                {
                    style = new DataGridViewCellStyle();
                    style.AddScope(DataGridView, _bandIsRow ? DataGridViewCellStyleScopes.Row : DataGridViewCellStyleScopes.Column);
                    Properties.SetObject(s_propDefaultCellStyle, style);
                }

                return style;
            }
            set
            {
                DataGridViewCellStyle style = null;
                if (HasDefaultCellStyle)
                {
                    style = DefaultCellStyle;
                    style.RemoveScope(_bandIsRow ? DataGridViewCellStyleScopes.Row : DataGridViewCellStyleScopes.Column);
                }
                if (value != null || Properties.ContainsObject(s_propDefaultCellStyle))
                {
                    value?.AddScope(DataGridView, _bandIsRow ? DataGridViewCellStyleScopes.Row : DataGridViewCellStyleScopes.Column);
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
                Type type = (Type)Properties.GetObject(s_propDefaultHeaderCellType);
                if (type != null)
                {
                    return type;
                }

                if (_bandIsRow)
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
                Debug.Assert(!_bandIsRow);
                return (State & DataGridViewElementStates.Displayed) != 0;
            }
        }

        internal bool DisplayedInternal
        {
            set
            {
                Debug.Assert(value != Displayed);
                if (value)
                {
                    StateInternal = State | DataGridViewElementStates.Displayed;
                }
                else
                {
                    StateInternal = State & ~DataGridViewElementStates.Displayed;
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
                    if (_bandIsRow)
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
                    if (_bandIsRow)
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
                Debug.Assert(!_bandIsRow);
                return (State & DataGridViewElementStates.Frozen) != 0;
            }
            set
            {
                if (((State & DataGridViewElementStates.Frozen) != 0) != value)
                {
                    OnStateChanging(DataGridViewElementStates.Frozen);
                    if (value)
                    {
                        StateInternal = State | DataGridViewElementStates.Frozen;
                    }
                    else
                    {
                        StateInternal = State & ~DataGridViewElementStates.Frozen;
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
        protected DataGridViewHeaderCell HeaderCellCore
        {
            get
            {
                DataGridViewHeaderCell headerCell = (DataGridViewHeaderCell)Properties.GetObject(s_propHeaderCell);
                if (headerCell == null)
                {
                    Type cellType = DefaultHeaderCellType;

                    headerCell = (DataGridViewHeaderCell)Activator.CreateInstance(cellType);
                    headerCell.DataGridViewInternal = DataGridView;
                    if (_bandIsRow)
                    {
                        headerCell.OwningRowInternal = (DataGridViewRow)this;   // may be a shared row
                        Properties.SetObject(s_propHeaderCell, headerCell);
                    }
                    else
                    {
                        DataGridViewColumn dataGridViewColumn = this as DataGridViewColumn;
                        headerCell.OwningColumnInternal = dataGridViewColumn;
                        // Set the headerCell in the property store before setting the SortOrder.
                        Properties.SetObject(s_propHeaderCell, headerCell);
                        if (DataGridView != null && DataGridView.SortedColumn == dataGridViewColumn)
                        {
                            DataGridViewColumnHeaderCell dataGridViewColumnHeaderCell = headerCell as DataGridViewColumnHeaderCell;
                            Debug.Assert(dataGridViewColumnHeaderCell != null);
                            dataGridViewColumnHeaderCell.SortGlyphDirection = DataGridView.SortOrder;
                        }
                    }
                }

                return headerCell;
            }
            set
            {
                DataGridViewHeaderCell headerCell = (DataGridViewHeaderCell)Properties.GetObject(s_propHeaderCell);
                if (value != null || Properties.ContainsObject(s_propHeaderCell))
                {
                    if (headerCell != null)
                    {
                        headerCell.DataGridViewInternal = null;
                        if (_bandIsRow)
                        {
                            headerCell.OwningRowInternal = null;
                        }
                        else
                        {
                            headerCell.OwningColumnInternal = null;
                            ((DataGridViewColumnHeaderCell)headerCell).SortGlyphDirectionInternal = SortOrder.None;
                        }
                    }

                    if (value != null)
                    {
                        if (_bandIsRow)
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
                            Debug.Assert(value.OwningRow == null);
                            value.OwningRowInternal = (DataGridViewRow)this;   // may be a shared row
                        }
                        else
                        {
                            DataGridViewColumnHeaderCell dataGridViewColumnHeaderCell = value as DataGridViewColumnHeaderCell;
                            if (dataGridViewColumnHeaderCell == null)
                            {
                                throw new ArgumentException(string.Format(SR.DataGridView_WrongType, nameof(DataGridViewColumn.HeaderCell), "System.Windows.Forms.DataGridViewColumnHeaderCell"), nameof(value));
                            }

                            // A HeaderCell can only be used by one band.
                            if (value.OwningColumn != null)
                            {
                                value.OwningColumn.HeaderCell = null;
                            }
                            Debug.Assert(dataGridViewColumnHeaderCell.SortGlyphDirection == SortOrder.None);
                            Debug.Assert(value.OwningColumn == null);
                            value.OwningColumnInternal = (DataGridViewColumn)this;
                        }
                        Debug.Assert(value.DataGridView == null);
                        value.DataGridViewInternal = DataGridView;
                    }

                    Properties.SetObject(s_propHeaderCell, value);
                }
                if (((value == null && headerCell != null) || (value != null && headerCell == null) || (value != null && headerCell != null && !headerCell.Equals(value))) && DataGridView != null)
                {
                    DataGridView.OnBandHeaderCellChanged(this);
                }
            }
        }

        [Browsable(false)]
        public int Index => _bandIndex;

        internal int IndexInternal
        {
            set => _bandIndex = value;
        }

        [Browsable(false)]
        public virtual DataGridViewCellStyle InheritedStyle => null;

        protected bool IsRow => _bandIsRow;

        internal int MinimumThickness
        {
            get
            {
                if (_bandIsRow && _bandIndex > -1)
                {
                    GetHeightInfo(_bandIndex, out int height, out int minimumHeight);
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
                        if (_bandIsRow)
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
                        if (DataGridView != null && !_bandIsRow)
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

        internal PropertyStore Properties => _propertyStore;

        [DefaultValue(false)]
        public virtual bool ReadOnly
        {
            get
            {
                Debug.Assert(!_bandIsRow);
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
                    if (_bandIsRow)
                    {
                        if (_bandIndex == -1)
                        {
                            throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, nameof(ReadOnly)));
                        }
    
                        OnStateChanging(DataGridViewElementStates.ReadOnly);
                        DataGridView.SetReadOnlyRowCore(_bandIndex, value);
                    }
                    else
                    {
                        Debug.Assert(_bandIndex >= 0);
                        OnStateChanging(DataGridViewElementStates.ReadOnly);
                        DataGridView.SetReadOnlyColumnCore(_bandIndex, value);
                    }
                }
                else
                {
                    if (((State & DataGridViewElementStates.ReadOnly) != 0) != value)
                    {
                        if (value)
                        {
                            if (_bandIsRow)
                            {
                                foreach (DataGridViewCell dataGridViewCell in ((DataGridViewRow)this).Cells)
                                {
                                    if (dataGridViewCell.ReadOnly)
                                    {
                                        dataGridViewCell.ReadOnlyInternal = false;
                                    }
                                }
                            }
                            StateInternal = State | DataGridViewElementStates.ReadOnly;
                        }
                        else
                        {
                            StateInternal = State & ~DataGridViewElementStates.ReadOnly;
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
                    StateInternal = State | DataGridViewElementStates.ReadOnly;
                }
                else
                {
                    StateInternal = State & ~DataGridViewElementStates.ReadOnly;
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
                Debug.Assert(!_bandIsRow);
                if ((State & DataGridViewElementStates.ResizableSet) != 0)
                {
                    return ((State & DataGridViewElementStates.Resizable) != 0) ? DataGridViewTriState.True : DataGridViewTriState.False;
                }
                if (DataGridView == null)
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
                    StateInternal = State & ~DataGridViewElementStates.ResizableSet;
                }
                else
                {
                    StateInternal = State | DataGridViewElementStates.ResizableSet;
                    if (((State & DataGridViewElementStates.Resizable) != 0) != (value == DataGridViewTriState.True))
                    {
                        if (value == DataGridViewTriState.True)
                        {
                            StateInternal = State | DataGridViewElementStates.Resizable;
                        }
                        else
                        {
                            StateInternal = State & ~DataGridViewElementStates.Resizable;
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
                Debug.Assert(!_bandIsRow);
                return (State & DataGridViewElementStates.Selected) != 0;
            }
            set
            {
                if (DataGridView != null)
                {
                    // this may trigger a call to set_SelectedInternal
                    if (_bandIsRow)
                    {
                        if (_bandIndex == -1)
                        {
                            throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, nameof(Selected)));
                        }
                        if (DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect || DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
                        {
                            DataGridView.SetSelectedRowCoreInternal(_bandIndex, value);
                        }
                    }
                    else
                    {
                        Debug.Assert(_bandIndex >= 0);
                        if (DataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect || DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
                        {
                            DataGridView.SetSelectedColumnCoreInternal(_bandIndex, value);
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
                    StateInternal = State | DataGridViewElementStates.Selected;
                }
                else
                {
                    StateInternal = State & ~DataGridViewElementStates.Selected;
                }

                if (DataGridView != null)
                {
                    OnStateChanged(DataGridViewElementStates.Selected);
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Tag
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
                if (_bandIsRow && _bandIndex > -1)
                {
                    GetHeightInfo(_bandIndex, out int height, out int minimumHeight);
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
                    if (_bandIsRow)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidHighBoundArgumentEx, nameof(DataGridViewRow.Height), value, MaxBandThickness));
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidHighBoundArgumentEx, nameof(DataGridViewColumn.Width), value, MaxBandThickness));
                    }
                }

                bool setThickness = true;
                if (_bandIsRow)
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
                Debug.Assert(!_bandIsRow);
                return (State & DataGridViewElementStates.Visible) != 0;
            }
            set
            {
                if (((State & DataGridViewElementStates.Visible) != 0) != value)
                {
                    if (DataGridView != null &&
                        _bandIsRow &&
                        DataGridView.NewRowIndex != -1 &&
                        DataGridView.NewRowIndex == _bandIndex &&
                        !value)
                    {
                        // the 'new' row cannot be made invisble.
                        throw new InvalidOperationException(SR.DataGridViewBand_NewRowCannotBeInvisible);
                    }
        
                    OnStateChanging(DataGridViewElementStates.Visible);
                    if (value)
                    {
                        StateInternal = State | DataGridViewElementStates.Visible;
                    }
                    else
                    {
                        StateInternal = State & ~DataGridViewElementStates.Visible;
                    }
                    OnStateChanged(DataGridViewElementStates.Visible);
                }
            }
        }

        public virtual object Clone()
        {
            DataGridViewBand band = (DataGridViewBand)Activator.CreateInstance(GetType());
            if (band != null)
            {
                CloneInternal(band);
            }
            return band;
        }

        internal void CloneInternal(DataGridViewBand dataGridViewBand)
        {
            dataGridViewBand._propertyStore = new PropertyStore();
            dataGridViewBand._bandIndex = -1;
            dataGridViewBand._bandIsRow = _bandIsRow;
            if (!_bandIsRow || _bandIndex >= 0 || DataGridView == null)
            {
                dataGridViewBand.StateInternal = State & ~(DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed);
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

        private void DetachContextMenuStrip(object sender, EventArgs e)
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
                ContextMenuStrip contextMenuStrip = (ContextMenuStrip)ContextMenuStripInternal;
                if (contextMenuStrip != null)
                {
                    contextMenuStrip.Disposed -= new EventHandler(DetachContextMenuStrip);
                }
            }
        }

        internal void GetHeightInfo(int rowIndex, out int height, out int minimumHeight)
        {
            Debug.Assert(_bandIsRow);
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

        internal void OnStateChanged(DataGridViewElementStates elementState)
        {
            if (DataGridView != null)
            {
                // maybe move this code into OnDataGridViewElementStateChanged
                if (_bandIsRow)
                {
                    // we could be smarter about what needs to be invalidated.
                    DataGridView.Rows.InvalidateCachedRowCount(elementState);
                    DataGridView.Rows.InvalidateCachedRowsHeight(elementState);
                    if (_bandIndex != -1)
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
                if (_bandIsRow)
                {
                    if (_bandIndex != -1)
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
                if (DataGridView == null)
                {
                    DefaultCellStyle.RemoveScope(_bandIsRow ? DataGridViewCellStyleScopes.Row : DataGridViewCellStyleScopes.Column);
                }
                else
                {
                    DefaultCellStyle.AddScope(DataGridView, _bandIsRow ? DataGridViewCellStyleScopes.Row : DataGridViewCellStyleScopes.Column);
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

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Text;
    using System;
    using System.Globalization;
    using System.Security;
    using System.Security.Permissions;
        
    /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand"]/*' />
    /// <devdoc>
    ///    <para>Identifies a band or column in the dataGridView.</para>
    /// </devdoc>
    public class DataGridViewBand : DataGridViewElement, ICloneable, IDisposable
    {
        private static readonly int PropContextMenuStrip = PropertyStore.CreateKey();
        private static readonly int PropDefaultCellStyle = PropertyStore.CreateKey();
        private static readonly int PropDefaultHeaderCellType = PropertyStore.CreateKey();
        private static readonly int PropDividerThickness = PropertyStore.CreateKey();
        private static readonly int PropHeaderCell = PropertyStore.CreateKey();
        private static readonly int PropUserData = PropertyStore.CreateKey();

        internal const int minBandThickness = 2;
        internal const int maxBandThickness = 65536;

        private PropertyStore propertyStore;          // Contains all properties that are not always set.
        private int thickness, cachedThickness;
        private int minimumThickness;
        private int bandIndex;
        internal bool bandIsRow;

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.DataGridViewBand"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.DataGridViewBand'/> class.
        ///    </para>
        /// </devdoc>
        internal DataGridViewBand()
        {
            this.propertyStore = new PropertyStore();
            this.bandIndex = -1;
        }
        
        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.Finalize"]/*' />
        ~DataGridViewBand() 
        {
            Dispose(false);
        }

        internal int CachedThickness
        {
            get
            {
                return this.cachedThickness;
            }
            set
            {
                this.cachedThickness = value;
            }
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.ContextMenu"]/*' />
        [
            DefaultValue(null)
        ]
        public virtual ContextMenuStrip ContextMenuStrip
        {
            get
            {
                if (this.bandIsRow)
                {
                    return ((DataGridViewRow) this).GetContextMenuStrip(this.Index);
                }
                return this.ContextMenuStripInternal;
            }
            set
            {
                this.ContextMenuStripInternal = value;
            }
        }

        internal ContextMenuStrip ContextMenuStripInternal
        {
            get
            {
                return (ContextMenuStrip)this.Properties.GetObject(PropContextMenuStrip);
            }
            set
            {
                ContextMenuStrip oldValue = (ContextMenuStrip)this.Properties.GetObject(PropContextMenuStrip);
                if (oldValue != value)
                {
                    EventHandler disposedHandler = new EventHandler(DetachContextMenuStrip);
                    if (oldValue != null)
                    {
                        oldValue.Disposed -= disposedHandler;
                    }
                    this.Properties.SetObject(PropContextMenuStrip, value);
                    if (value != null)
                    {
                        value.Disposed += disposedHandler;
                    }
                    if (this.DataGridView != null)
                    {
                        this.DataGridView.OnBandContextMenuStripChanged(this);
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.DefaultCellStyle"]/*' />
        [
            Browsable(false)
        ]
        public virtual DataGridViewCellStyle DefaultCellStyle
        {
            get
            {
                DataGridViewCellStyle dgvcs = (DataGridViewCellStyle)this.Properties.GetObject(PropDefaultCellStyle);
                if (dgvcs == null)
                {
                    dgvcs = new DataGridViewCellStyle();
                    dgvcs.AddScope(this.DataGridView, 
                        this.bandIsRow ? DataGridViewCellStyleScopes.Row : DataGridViewCellStyleScopes.Column);
                    this.Properties.SetObject(PropDefaultCellStyle, dgvcs);
                }
                return dgvcs;
            }
            set
            {
                DataGridViewCellStyle dgvcs = null;
                if (this.HasDefaultCellStyle)
                {
                    dgvcs = this.DefaultCellStyle;
                    dgvcs.RemoveScope(this.bandIsRow ? DataGridViewCellStyleScopes.Row : DataGridViewCellStyleScopes.Column);
                }
                if (value != null || this.Properties.ContainsObject(PropDefaultCellStyle))
                {
                    if (value != null)
                    {
                        value.AddScope(this.DataGridView, 
                            this.bandIsRow ? DataGridViewCellStyleScopes.Row : DataGridViewCellStyleScopes.Column);
                    }
                    this.Properties.SetObject(PropDefaultCellStyle, value);
                }
                if (((dgvcs != null && value == null) || 
                    (dgvcs == null && value != null) || 
                    (dgvcs != null && value != null && !dgvcs.Equals(this.DefaultCellStyle))) && this.DataGridView != null)
                {
                    this.DataGridView.OnBandDefaultCellStyleChanged(this);
                }
            }
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.DefaultHeaderCellType"]/*' />
        [
            Browsable(false)
        ]
        public Type DefaultHeaderCellType
        {
            get
            {
                Type dhct = (Type)this.Properties.GetObject(PropDefaultHeaderCellType);
                if (dhct == null)
                {
                    if (this.bandIsRow)
                    {
                        dhct = typeof(System.Windows.Forms.DataGridViewRowHeaderCell);
                    }
                    else
                    {
                        dhct = typeof(System.Windows.Forms.DataGridViewColumnHeaderCell);
                    }
                }
                return dhct;
            }
            set
            {
                if (value != null || this.Properties.ContainsObject(PropDefaultHeaderCellType))
                {
                    if (Type.GetType("System.Windows.Forms.DataGridViewHeaderCell").IsAssignableFrom(value))
                    {
                        this.Properties.SetObject(PropDefaultHeaderCellType, value);
                    }
                    else
                    {
                        throw new ArgumentException(string.Format(SR.DataGridView_WrongType, "DefaultHeaderCellType", "System.Windows.Forms.DataGridViewHeaderCell"));
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.Displayed"]/*' />
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual bool Displayed
        {
            get
            {
                Debug.Assert(!this.bandIsRow);
                bool displayed = (this.State & DataGridViewElementStates.Displayed) != 0;
                // Only attached and visible columns can be displayed.
                // Debug.Assert(!displayed || (this.DataGridView != null && this.DataGridView.Visible && this.Visible));
                return displayed;
            }
        }

        internal bool DisplayedInternal
        {
            set
            {
                Debug.Assert(value != this.Displayed);
                if (value)
                {
                    this.StateInternal = this.State | DataGridViewElementStates.Displayed;
                }
                else
                {
                    this.StateInternal = this.State & ~DataGridViewElementStates.Displayed;
                }
                if (this.DataGridView != null)
                {
                    OnStateChanged(DataGridViewElementStates.Displayed);
                }
            }
        }

        internal int DividerThickness
        {
            get
            {
                bool found;
                int dividerThickness = this.Properties.GetInteger(PropDividerThickness, out found);
                return found ? dividerThickness : 0;
            }
            set
            {
                if (value < 0)
                {
                    if (this.bandIsRow)
                    {
                        throw new ArgumentOutOfRangeException("DividerHeight", string.Format(SR.InvalidLowBoundArgumentEx, "DividerHeight", (value).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("DividerWidth", string.Format(SR.InvalidLowBoundArgumentEx, "DividerWidth", (value).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                    }
                }
                if (value > maxBandThickness)
                {
                    if (this.bandIsRow)
                    {
                        throw new ArgumentOutOfRangeException("DividerHeight", string.Format(SR.InvalidHighBoundArgumentEx, "DividerHeight", (value).ToString(CultureInfo.CurrentCulture), (maxBandThickness).ToString(CultureInfo.CurrentCulture)));
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("DividerWidth", string.Format(SR.InvalidHighBoundArgumentEx, "DividerWidth", (value).ToString(CultureInfo.CurrentCulture), (maxBandThickness).ToString(CultureInfo.CurrentCulture)));
                    }
                }
                if (value != this.DividerThickness)
                {
                    this.Properties.SetInteger(PropDividerThickness, (int)value);
                    if (this.DataGridView != null)
                    {
                        this.DataGridView.OnBandDividerThicknessChanged(this);
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.Frozen"]/*' />
        [
            DefaultValue(false),
        ]
        public virtual bool Frozen
        {
            get
            {
                Debug.Assert(!this.bandIsRow);
                return (this.State & DataGridViewElementStates.Frozen) != 0;
            }
            set
            {
                if (((this.State & DataGridViewElementStates.Frozen) != 0) != value)
                {
                    OnStateChanging(DataGridViewElementStates.Frozen);
                    if (value)
                    {
                        this.StateInternal = this.State | DataGridViewElementStates.Frozen;
                    }
                    else
                    {
                        this.StateInternal = this.State & ~DataGridViewElementStates.Frozen;
                    }
                    OnStateChanged(DataGridViewElementStates.Frozen);
                }
            }
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.HasDefaultCellStyle"]/*' />
        [
            Browsable(false)
        ]
        public bool HasDefaultCellStyle
        {
            get
            {
                return this.Properties.ContainsObject(PropDefaultCellStyle) && this.Properties.GetObject(PropDefaultCellStyle) != null;
            }
        }

        internal bool HasDefaultHeaderCellType
        {
            get
            {
                return this.Properties.ContainsObject(PropDefaultHeaderCellType) && this.Properties.GetObject(PropDefaultHeaderCellType) != null;
            }
        }

        internal bool HasHeaderCell
        {
            get
            {
                return this.Properties.ContainsObject(PropHeaderCell) && this.Properties.GetObject(PropHeaderCell) != null;
            }
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.HeaderCellCore"]/*' />
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        protected DataGridViewHeaderCell HeaderCellCore
        {
            get
            {
                DataGridViewHeaderCell headerCell = (DataGridViewHeaderCell)this.Properties.GetObject(PropHeaderCell);
                if (headerCell == null)
                {
                    Type cellType = this.DefaultHeaderCellType;

                    headerCell = (DataGridViewHeaderCell) SecurityUtils.SecureCreateInstance(cellType);
                    headerCell.DataGridViewInternal = this.DataGridView;
                    if (this.bandIsRow)
                    {
                        headerCell.OwningRowInternal = (DataGridViewRow)this;   // may be a shared row
                        this.Properties.SetObject(PropHeaderCell, headerCell);
                    }
                    else
                    {
                        DataGridViewColumn dataGridViewColumn = this as DataGridViewColumn;
                        headerCell.OwningColumnInternal = dataGridViewColumn;
                        // Set the headerCell in the property store before setting the SortOrder.
                        this.Properties.SetObject(PropHeaderCell, headerCell);
                        if (this.DataGridView != null && this.DataGridView.SortedColumn == dataGridViewColumn)
                        {
                            DataGridViewColumnHeaderCell dataGridViewColumnHeaderCell = headerCell as DataGridViewColumnHeaderCell;
                            Debug.Assert(dataGridViewColumnHeaderCell != null);
                            dataGridViewColumnHeaderCell.SortGlyphDirection = this.DataGridView.SortOrder;
                        }
                    }
                }
                return headerCell;
            }
            set
            {
                DataGridViewHeaderCell headerCell = (DataGridViewHeaderCell)this.Properties.GetObject(PropHeaderCell);
                if (value != null || this.Properties.ContainsObject(PropHeaderCell))
                {
                    if (headerCell != null)
                    {
                        headerCell.DataGridViewInternal = null;
                        if (this.bandIsRow)
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
                        if (this.bandIsRow)
                        {
                            if (!(value is DataGridViewRowHeaderCell))
                            {
                                throw new ArgumentException(string.Format(SR.DataGridView_WrongType, "HeaderCell", "System.Windows.Forms.DataGridViewRowHeaderCell"));
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
                                throw new ArgumentException(string.Format(SR.DataGridView_WrongType, "HeaderCell", "System.Windows.Forms.DataGridViewColumnHeaderCell"));
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
                        value.DataGridViewInternal = this.DataGridView;
                    }

                    this.Properties.SetObject(PropHeaderCell, value);
                }
                if (((value == null && headerCell != null) || (value != null && headerCell == null) || (value != null && headerCell != null && !headerCell.Equals(value))) && this.DataGridView != null)
                {
                    this.DataGridView.OnBandHeaderCellChanged(this);
                }
            }
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.Index"]/*' />
        /// <devdoc>
        /// <para></para>
        /// </devdoc>
        [
            Browsable(false)
        ]
        public int Index
        {
            get
            {
                return this.bandIndex;
            }
        }

        internal int IndexInternal
        {
            set
            {
                this.bandIndex = value;
            }
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.InheritedStyle"]/*' />
        [
            Browsable(false)
        ]
        public virtual DataGridViewCellStyle InheritedStyle
        {
            get
            {
                return null;
            }
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.IsRow"]/*' />
        protected bool IsRow
        {
            get
            {
                return this.bandIsRow;
            }
        }

        internal int MinimumThickness
        {
            get
            {
                if (this.bandIsRow && this.bandIndex > -1)
                {
                    int height, minimumHeight;
                    GetHeightInfo(this.bandIndex, out height, out minimumHeight);
                    return minimumHeight;
                }
                return this.minimumThickness;
            }
            set
            {
                if (this.minimumThickness != value)
                {
                    if (value < minBandThickness)
                    {
                        if (this.bandIsRow)
                        {
                            throw new ArgumentOutOfRangeException("MinimumHeight", value, string.Format(SR.DataGridViewBand_MinimumHeightSmallerThanOne, (DataGridViewBand.minBandThickness).ToString(CultureInfo.CurrentCulture)));
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException("MinimumWidth", value, string.Format(SR.DataGridViewBand_MinimumWidthSmallerThanOne, (DataGridViewBand.minBandThickness).ToString(CultureInfo.CurrentCulture)));
                        }
                    }
                    if (this.Thickness < value)
                    {
                        // Force the new minimum width on potential auto fill column.
                        if (this.DataGridView != null && !this.bandIsRow)
                        {
                            this.DataGridView.OnColumnMinimumWidthChanging((DataGridViewColumn)this, value);
                        }
                        this.Thickness = value;
                    }
                    this.minimumThickness = value;
                    if (this.DataGridView != null)
                    {
                        this.DataGridView.OnBandMinimumThicknessChanged(this);
                    }
                }
            }
        }

        internal PropertyStore Properties
        {
            get
            {
                return this.propertyStore;
            }
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.ReadOnly"]/*' />
        [
            DefaultValue(false)
        ]
        public virtual bool ReadOnly
        {
            get
            {
                Debug.Assert(!this.bandIsRow);
                return ((this.State & DataGridViewElementStates.ReadOnly) != 0 ||
                    (this.DataGridView != null && this.DataGridView.ReadOnly));
            }
            set
            {
                if (this.DataGridView != null)
                {
                    if (this.DataGridView.ReadOnly)
                    {
                        // if (!value): Trying to make a band read-write when the whole grid is read-only.
                        // if (value):  Trying to make a band read-only when the whole grid is read-only.
                        // Ignoring the request and returning.
                        return;
                    }

                    // this may trigger a call to set_ReadOnlyInternal
                    if (this.bandIsRow)
                    {
                        if (this.bandIndex == -1)
                        {
                            throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, "ReadOnly"));
                        }
                        OnStateChanging(DataGridViewElementStates.ReadOnly);
                        this.DataGridView.SetReadOnlyRowCore(this.bandIndex, value);
                    }
                    else
                    {
                        Debug.Assert(this.bandIndex >= 0);
                        OnStateChanging(DataGridViewElementStates.ReadOnly);
                        this.DataGridView.SetReadOnlyColumnCore(this.bandIndex, value);
                    }
                }
                else
                {
                    if (((this.State & DataGridViewElementStates.ReadOnly) != 0) != value)
                    {
                        if (value)
                        {
                            if (this.bandIsRow)
                            {
                                foreach (DataGridViewCell dataGridViewCell in ((DataGridViewRow) this).Cells)
                                {
                                    if (dataGridViewCell.ReadOnly)
                                    {
                                        dataGridViewCell.ReadOnlyInternal = false;
                                    }
                                }
                            }
                            this.StateInternal = this.State | DataGridViewElementStates.ReadOnly;
                        }
                        else
                        {
                            this.StateInternal = this.State & ~DataGridViewElementStates.ReadOnly;
                        }
                    }
                }
            }
        }

        internal bool ReadOnlyInternal
        {
            set
            {
                Debug.Assert(value != this.ReadOnly);
                if (value)
                {
                    this.StateInternal = this.State | DataGridViewElementStates.ReadOnly;
                }
                else
                {
                    this.StateInternal = this.State & ~DataGridViewElementStates.ReadOnly;
                }
                Debug.Assert(this.DataGridView != null);
                OnStateChanged(DataGridViewElementStates.ReadOnly);
            }
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.Resizable"]/*' />
        [
            Browsable(true)
        ]
        public virtual DataGridViewTriState Resizable
        {
            get
            {
                Debug.Assert(!this.bandIsRow);
                if ((this.State & DataGridViewElementStates.ResizableSet) != 0)
                {
                    return ((this.State & DataGridViewElementStates.Resizable) != 0) ? DataGridViewTriState.True : DataGridViewTriState.False;
                }
                if (this.DataGridView != null)
                {
                    return this.DataGridView.AllowUserToResizeColumns ? DataGridViewTriState.True : DataGridViewTriState.False;
                }
                else
                {
                    return DataGridViewTriState.NotSet;
                }
            }
            set
            {
                DataGridViewTriState oldResizable = this.Resizable;
                if (value == DataGridViewTriState.NotSet)
                {
                    this.StateInternal = this.State & ~DataGridViewElementStates.ResizableSet;
                }
                else
                {
                    this.StateInternal = this.State | DataGridViewElementStates.ResizableSet;
                    if (((this.State & DataGridViewElementStates.Resizable) != 0) != (value == DataGridViewTriState.True))
                    {
                        if (value == DataGridViewTriState.True)
                        {
                            this.StateInternal = this.State | DataGridViewElementStates.Resizable;
                        }
                        else
                        {
                            Debug.Assert(value == DataGridViewTriState.False, "TriState only supports NotSet, True, False");
                            this.StateInternal = this.State & ~DataGridViewElementStates.Resizable;
                        }
                    }
                }
                if (oldResizable != this.Resizable)
                {
                    OnStateChanged(DataGridViewElementStates.Resizable);
                }
            }
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.Selected"]/*' />
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual bool Selected
        {
            get
            {
                Debug.Assert(!this.bandIsRow);
                return (this.State & DataGridViewElementStates.Selected) != 0;
            }
            set
            {
                if (this.DataGridView != null)
                {
                    // this may trigger a call to set_SelectedInternal
                    if (this.bandIsRow)
                    {
                        if (this.bandIndex == -1)
                        {
                            throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertySetOnSharedRow, "Selected"));
                        }
                        if (this.DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect || this.DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
                        {
                            this.DataGridView.SetSelectedRowCoreInternal(this.bandIndex, value);
                        }
                    }
                    else
                    {
                        Debug.Assert(this.bandIndex >= 0);
                        if (this.DataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect || this.DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
                        {
                            this.DataGridView.SetSelectedColumnCoreInternal(this.bandIndex, value);
                        }
                    }
                }
                else if (value)
                {
                    // We do not allow the selection of a band before it gets added to the dataGridView.
                    throw new InvalidOperationException(string.Format(SR.DataGridViewBand_CannotSelect));
                }
            }
        }

        internal bool SelectedInternal
        {
            set
            {
                Debug.Assert(value != this.Selected);
                if (value)
                {
                    this.StateInternal = this.State | DataGridViewElementStates.Selected;
                }
                else
                {
                    this.StateInternal = this.State & ~DataGridViewElementStates.Selected;
                }
                if (this.DataGridView != null)
                {
                    OnStateChanged(DataGridViewElementStates.Selected);
                }
            }
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.Tag"]/*' />
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public object Tag
        {
            get
            {
                return Properties.GetObject(PropUserData);
            }
            set
            {
                if (value != null || this.Properties.ContainsObject(PropUserData))
                {
                    Properties.SetObject(PropUserData, value);
                }
            }
        }

        internal int Thickness
        {
            get
            {
                if (this.bandIsRow && this.bandIndex > -1)
                {
                    int height, minimumHeight;
                    GetHeightInfo(this.bandIndex, out height, out minimumHeight);
                    return height;
                }
                return this.thickness;
            }
            set
            {
                int minimumThickness = this.MinimumThickness;
                if (value < minimumThickness)
                {
                    value = minimumThickness;
                }
                if (value > maxBandThickness)
                {
                    if (this.bandIsRow)
                    {
                        throw new ArgumentOutOfRangeException("Height", string.Format(SR.InvalidHighBoundArgumentEx, "Height", (value).ToString(CultureInfo.CurrentCulture), (maxBandThickness).ToString(CultureInfo.CurrentCulture)));
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Width", string.Format(SR.InvalidHighBoundArgumentEx, "Width", (value).ToString(CultureInfo.CurrentCulture), (maxBandThickness).ToString(CultureInfo.CurrentCulture)));
                    }
                }
                bool setThickness = true;
                if (this.bandIsRow)
                {
                    if (this.DataGridView != null && this.DataGridView.AutoSizeRowsMode != DataGridViewAutoSizeRowsMode.None)
                    {
                        this.cachedThickness = value;
                        setThickness = false;
                    }
                }
                else
                {
                    DataGridViewColumn dataGridViewColumn = (DataGridViewColumn) this;
                    DataGridViewAutoSizeColumnMode inheritedAutoSizeMode = dataGridViewColumn.InheritedAutoSizeMode;
                    if (inheritedAutoSizeMode != DataGridViewAutoSizeColumnMode.Fill &&
                        inheritedAutoSizeMode != DataGridViewAutoSizeColumnMode.None &&
                        inheritedAutoSizeMode != DataGridViewAutoSizeColumnMode.NotSet)
                    {
                        this.cachedThickness = value;
                        setThickness = false;
                    }
                    else if (inheritedAutoSizeMode == DataGridViewAutoSizeColumnMode.Fill && this.DataGridView != null)
                    {
                        if (dataGridViewColumn.Visible)
                        {
                            IntPtr handle = this.DataGridView.Handle;
                            this.DataGridView.AdjustFillingColumn(dataGridViewColumn, value);
                            setThickness = false;
                        }
                    }
                }

                if (setThickness && this.thickness != value)
                {
                    if (this.DataGridView != null)
                    {
                        this.DataGridView.OnBandThicknessChanging();
                    }
                    this.ThicknessInternal = value;
                }
            }
        }

        internal int ThicknessInternal
        {
            get
            {
                return this.thickness;
            }
            set
            {
                Debug.Assert(this.thickness != value);
                Debug.Assert(value >= this.minimumThickness);
                Debug.Assert(value <= maxBandThickness);

                this.thickness = value;
                if (this.DataGridView != null)
                {
                    this.DataGridView.OnBandThicknessChanged(this);
                }
            }
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.Visible"]/*' />
        [
            DefaultValue(true),
        ]
        public virtual bool Visible
        {
            get
            {
                Debug.Assert(!this.bandIsRow);
                return (this.State & DataGridViewElementStates.Visible) != 0;
            }
            set
            {
                if (((this.State & DataGridViewElementStates.Visible) != 0) != value)
                {
                    if (this.DataGridView != null && 
                        this.bandIsRow && 
                        this.DataGridView.NewRowIndex != -1 && 
                        this.DataGridView.NewRowIndex == this.bandIndex && 
                        !value)
                    {
                        // the 'new' row cannot be made invisble.
                        throw new InvalidOperationException(string.Format(SR.DataGridViewBand_NewRowCannotBeInvisible));
                    }
                    OnStateChanging(DataGridViewElementStates.Visible);
                    if (value)
                    {
                        this.StateInternal = this.State | DataGridViewElementStates.Visible;
                    }
                    else
                    {
                        this.StateInternal = this.State & ~DataGridViewElementStates.Visible;
                    }
                    OnStateChanged(DataGridViewElementStates.Visible);
                }
            }
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.Clone"]/*' />
        public virtual object Clone()
        {
            DataGridViewBand dataGridViewBand = (DataGridViewBand) System.Activator.CreateInstance(this.GetType());
            if (dataGridViewBand != null)
            {
                CloneInternal(dataGridViewBand);
            }
            return dataGridViewBand;
        }

        internal void CloneInternal(DataGridViewBand dataGridViewBand)
        {
            dataGridViewBand.propertyStore = new PropertyStore();
            dataGridViewBand.bandIndex = -1;
            dataGridViewBand.bandIsRow = this.bandIsRow;
            if (!this.bandIsRow || this.bandIndex >= 0 || this.DataGridView == null)
            {
                dataGridViewBand.StateInternal = this.State & ~(DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed);
            }
            dataGridViewBand.thickness = this.Thickness;
            dataGridViewBand.MinimumThickness = this.MinimumThickness;
            dataGridViewBand.cachedThickness = this.CachedThickness;
            dataGridViewBand.DividerThickness = this.DividerThickness;
            dataGridViewBand.Tag = this.Tag;
            if (this.HasDefaultCellStyle)
            {
                dataGridViewBand.DefaultCellStyle = new DataGridViewCellStyle(this.DefaultCellStyle);
            }
            if (this.HasDefaultHeaderCellType)
            {
                dataGridViewBand.DefaultHeaderCellType = this.DefaultHeaderCellType;
            }
            if (this.ContextMenuStripInternal != null)
            {
                dataGridViewBand.ContextMenuStrip = this.ContextMenuStripInternal.Clone();
            }
        }

        private void DetachContextMenuStrip(object sender, EventArgs e)
        {
            this.ContextMenuStripInternal = null;
        }
        
        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.Dispose"]/*' />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.Dispose2"]/*' />
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) 
            {
                ContextMenuStrip contextMenuStrip = (ContextMenuStrip)this.ContextMenuStripInternal;
                if (contextMenuStrip != null)
                {
                    contextMenuStrip.Disposed -= new EventHandler(DetachContextMenuStrip);
                }
            }
        }

        internal void GetHeightInfo(int rowIndex, out int height, out int minimumHeight)
        {
            Debug.Assert(this.bandIsRow);
            if (this.DataGridView != null &&
                (this.DataGridView.VirtualMode || this.DataGridView.DataSource != null) &&
                this.DataGridView.AutoSizeRowsMode == DataGridViewAutoSizeRowsMode.None)
            {
                Debug.Assert(rowIndex > -1);
                DataGridViewRowHeightInfoNeededEventArgs dgvrhine = this.DataGridView.OnRowHeightInfoNeeded(rowIndex, this.thickness, this.minimumThickness);
                height = dgvrhine.Height;
                minimumHeight = dgvrhine.MinimumHeight;
                return;
            }
            height = this.thickness;
            minimumHeight = this.minimumThickness;
        }

        internal void OnStateChanged(DataGridViewElementStates elementState)
        {
            if (this.DataGridView != null)
            {
                // maybe move this code into OnDataGridViewElementStateChanged
                if (this.bandIsRow)
                {
                    // we could be smarter about what needs to be invalidated.
                    this.DataGridView.Rows.InvalidateCachedRowCount(elementState);
                    this.DataGridView.Rows.InvalidateCachedRowsHeight(elementState);
                    if (this.bandIndex != -1)
                    {
                        this.DataGridView.OnDataGridViewElementStateChanged(this, -1, elementState);
                    }
                }
                else
                {
                    // we could be smarter about what needs to be invalidated.
                    this.DataGridView.Columns.InvalidateCachedColumnCount(elementState);
                    this.DataGridView.Columns.InvalidateCachedColumnsWidth(elementState);
                    this.DataGridView.OnDataGridViewElementStateChanged(this, -1, elementState);
                }
            }
        }

        private void OnStateChanging(DataGridViewElementStates elementState) 
        {
            if (this.DataGridView != null)
            {
                if (this.bandIsRow)
                {
                    if (this.bandIndex != -1)
                    {
                        this.DataGridView.OnDataGridViewElementStateChanging(this, -1, elementState);
                    }
                }
                else
                {
                    this.DataGridView.OnDataGridViewElementStateChanging(this, -1, elementState);
                }
            }
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.OnDataGridViewChanged"]/*' />
        protected override void OnDataGridViewChanged()
        {
            if (this.HasDefaultCellStyle)
            {
                if (this.DataGridView == null)
                {
                    this.DefaultCellStyle.RemoveScope(this.bandIsRow ? DataGridViewCellStyleScopes.Row : DataGridViewCellStyleScopes.Column);
                }
                else
                {
                    this.DefaultCellStyle.AddScope(this.DataGridView, 
                        this.bandIsRow ? DataGridViewCellStyleScopes.Row : DataGridViewCellStyleScopes.Column);
                }
            }
            base.OnDataGridViewChanged();
        }

        private bool ShouldSerializeDefaultHeaderCellType() 
        {
            Type dhct = (Type)this.Properties.GetObject(PropDefaultHeaderCellType);
            return dhct != null;
        }

        // internal because DataGridViewColumn needs to access it
        internal bool ShouldSerializeResizable()
        {
            return (this.State & DataGridViewElementStates.ResizableSet) != 0;
        }

        /// <include file='doc\DataGridViewBand.uex' path='docs/doc[@for="DataGridViewBand.ToString"]/*' />
        /// <devdoc>
        ///    <para></para>
        /// </devdoc>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(36);
            sb.Append("DataGridViewBand { Index=");
            sb.Append(this.Index.ToString(CultureInfo.CurrentCulture));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}

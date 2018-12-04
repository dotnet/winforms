// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Code taken from ASP.NET file xsp\System\Web\httpserverutility.cs
// Don't entity encode high chars (160 to 256)
#define ENTITY_ENCODE_HIGH_ASCII_CHARS

namespace System.Windows.Forms
{
    using System;
    using System.IO;
    using System.Text;
    using System.Diagnostics;
    using System.Drawing;
    using System.Reflection;
    using System.Globalization;
    using System.ComponentModel;
    using System.Windows.Forms.Internal;
    using System.Security.Permissions;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;

    /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell"]/*' />
    /// <devdoc>
    ///    <para>Identifies a cell in the dataGridView.</para>
    /// </devdoc>
    [
        TypeConverterAttribute(typeof(DataGridViewCellConverter))
    ]
    public abstract class DataGridViewCell : DataGridViewElement, ICloneable, IDisposable
    {
        private const TextFormatFlags textFormatSupportedFlags = TextFormatFlags.SingleLine | /*TextFormatFlags.NoFullWidthCharacterBreak |*/ TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix;
        private const int DATAGRIDVIEWCELL_constrastThreshold = 1000;
        private const int DATAGRIDVIEWCELL_highConstrastThreshold = 2000;
        private const int DATAGRIDVIEWCELL_maxToolTipLength = 288;
        private const int DATAGRIDVIEWCELL_maxToolTipCutOff = 256;
        private const int DATAGRIDVIEWCELL_toolTipEllipsisLength = 3;
        private const string DATAGRIDVIEWCELL_toolTipEllipsis = "...";
        private const byte DATAGRIDVIEWCELL_flagAreaNotSet = 0x00;
        private const byte DATAGRIDVIEWCELL_flagDataArea = 0x01;
        private const byte DATAGRIDVIEWCELL_flagErrorArea = 0x02;
        internal const byte DATAGRIDVIEWCELL_iconMarginWidth = 4;      // 4 pixels of margin on the left and right of icons
        internal const byte DATAGRIDVIEWCELL_iconMarginHeight = 4;     // 4 pixels of margin on the top and bottom of icons
        private const byte DATAGRIDVIEWCELL_iconsWidth = 12;          // all icons are 12 pixels wide - make sure that it stays that way
        private const byte DATAGRIDVIEWCELL_iconsHeight = 11;         // all icons are 11 pixels tall - make sure that it stays that way

        private static bool isScalingInitialized = false;
        internal static byte iconsWidth = DATAGRIDVIEWCELL_iconsWidth;
        internal static byte iconsHeight = DATAGRIDVIEWCELL_iconsHeight;

        internal static readonly int PropCellValue = PropertyStore.CreateKey();
        private static readonly int PropCellContextMenuStrip = PropertyStore.CreateKey();
        private static readonly int PropCellErrorText = PropertyStore.CreateKey();
        private static readonly int PropCellStyle = PropertyStore.CreateKey();
        private static readonly int PropCellValueType = PropertyStore.CreateKey();
        private static readonly int PropCellTag = PropertyStore.CreateKey();
        private static readonly int PropCellToolTipText = PropertyStore.CreateKey();
        private static readonly int PropCellAccessibilityObject = PropertyStore.CreateKey();

        private static Bitmap errorBmp = null;

        private PropertyStore propertyStore;          // Contains all properties that are not always set.
        private DataGridViewRow owningRow;
        private DataGridViewColumn owningColumn;

        private static Type stringType = typeof(string);        // cache the string type for performance

        private byte flags;  // see DATAGRIDVIEWCELL_flag* consts above

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.DataGridViewCell"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.DataGridViewCell'/> class.
        ///    </para>
        /// </devdoc>
        protected DataGridViewCell() : base()
        {
            if (!isScalingInitialized) {
                if (DpiHelper.IsScalingRequired) {
                    iconsWidth = (byte)DpiHelper.LogicalToDeviceUnitsX(DATAGRIDVIEWCELL_iconsWidth);
                    iconsHeight = (byte)DpiHelper.LogicalToDeviceUnitsY(DATAGRIDVIEWCELL_iconsHeight);
                }
                isScalingInitialized = true;
            }

            this.propertyStore = new PropertyStore();
            this.StateInternal = DataGridViewElementStates.None;
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.Finalize"]/*' />
        ~DataGridViewCell() 
        {
            Dispose(false);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.AccessibilityObject"]/*' />
        [
            Browsable(false)
        ]
        public AccessibleObject AccessibilityObject
        {
            get
            {
                AccessibleObject result = (AccessibleObject) this.Properties.GetObject(PropCellAccessibilityObject);
                if (result == null)
                {
                    result = this.CreateAccessibilityInstance();
                    this.Properties.SetObject(PropCellAccessibilityObject, result);
                }

                return result;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.ColumnIndex"]/*' />
        /// <devdoc>
        /// <para>Gets or sets the Index of a column in the <see cref='System.Windows.Forms.DataGrid'/> control.</para>
        /// </devdoc>
        public int ColumnIndex
        {
            get
            {
                if (this.owningColumn == null)
                {
                    return -1;
                }
                return this.owningColumn.Index;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.ContentBounds"]/*' />
        [
            Browsable(false)
        ]
        public Rectangle ContentBounds
        {
            get
            {
                return GetContentBounds(this.RowIndex);
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.ContextMenuStrip"]/*' />
        [
            DefaultValue(null)
        ]
        public virtual ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return GetContextMenuStrip(this.RowIndex);
            }
            set
            {
                this.ContextMenuStripInternal = value;
            }
        }

        private ContextMenuStrip ContextMenuStripInternal
        {
            get
            {
                return (ContextMenuStrip)this.Properties.GetObject(PropCellContextMenuStrip);
            }
            set
            {
                ContextMenuStrip oldValue = (ContextMenuStrip)this.Properties.GetObject(PropCellContextMenuStrip);
                if (oldValue != value)
                {
                    EventHandler disposedHandler = new EventHandler(DetachContextMenuStrip);
                    if (oldValue != null)
                    {
                        oldValue.Disposed -= disposedHandler;
                    }
                    this.Properties.SetObject(PropCellContextMenuStrip, value);
                    if (value != null)
                    {
                        value.Disposed += disposedHandler;
                    }
                    if (this.DataGridView != null)
                    {
                        this.DataGridView.OnCellContextMenuStripChanged(this);
                    }
                }
            }
        }

        private byte CurrentMouseLocation
        {
            get
            {
                return (byte) (this.flags & (DATAGRIDVIEWCELL_flagDataArea | DATAGRIDVIEWCELL_flagErrorArea));
            }
            set
            {
                this.flags = (byte)(this.flags & ~(DATAGRIDVIEWCELL_flagDataArea | DATAGRIDVIEWCELL_flagErrorArea));
                this.flags |= value;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.DefaultNewRowValue"]/*' />
        [
            Browsable(false)
        ]
        public virtual object DefaultNewRowValue
        {
            get
            {
                return null;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.Displayed"]/*' />
        [
            Browsable(false)
        ]
        public virtual bool Displayed
        {
            get
            {
                Debug.Assert((this.State & DataGridViewElementStates.Displayed) == 0);

                if (this.DataGridView == null)
                {
                    // No detached element is displayed.
                    return false;
                }

                if (this.DataGridView != null && this.RowIndex >= 0 && this.ColumnIndex >= 0)
                {
                    Debug.Assert(this.DataGridView.Rows.GetRowState(this.RowIndex) == this.DataGridView.Rows.SharedRow(this.RowIndex).State);
                    return this.owningColumn.Displayed && this.owningRow.Displayed;
                }

                return false;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.EditedFormattedValue"]/*' />
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public object EditedFormattedValue
        {
            get
            {
                if (this.DataGridView == null)
                {
                    return null;
                }
                Debug.Assert(this.RowIndex >= -1);
                DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, this.RowIndex, false);
                return GetEditedFormattedValue(GetValue(this.RowIndex), this.RowIndex, ref dataGridViewCellStyle, DataGridViewDataErrorContexts.Formatting);
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.EditType"]/*' />
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public virtual Type EditType
        {
            get
            {
                return typeof(System.Windows.Forms.DataGridViewTextBoxEditingControl);
            }
        }

        private static Bitmap ErrorBitmap
        {
            get
            {
                if (errorBmp == null)
                {
                    errorBmp = GetBitmap("DataGridViewRow.error.bmp");
                }
                return errorBmp;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.ErrorIconBounds"]/*' />
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods") // ErrorIconBounds/GetErrorIconBounds existence is intentional
        ]
        public Rectangle ErrorIconBounds
        {
            get
            {
                return GetErrorIconBounds(this.RowIndex);
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.ErrorText"]/*' />
        [
            Browsable(false)
        ]
        public string ErrorText
        {
            get
            {
                return GetErrorText(this.RowIndex);
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
                object errorText = this.Properties.GetObject(PropCellErrorText);
                return (errorText == null) ? string.Empty : (string)errorText;
            }
            set
            {
                string errorText = this.ErrorTextInternal;
                if (!string.IsNullOrEmpty(value) || this.Properties.ContainsObject(PropCellErrorText))
                {
                    this.Properties.SetObject(PropCellErrorText, value);
                }
                if (this.DataGridView != null && !errorText.Equals(this.ErrorTextInternal))
                {
                    this.DataGridView.OnCellErrorTextChanged(this);
                }
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.FormattedValue"]/*' />
        [
            Browsable(false)
        ]
        public object FormattedValue
        {
            get
            {
                if (this.DataGridView == null)
                {
                    return null;
                }
                Debug.Assert(this.RowIndex >= -1);
                DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, this.RowIndex, false);
                return GetFormattedValue(this.RowIndex, ref dataGridViewCellStyle, DataGridViewDataErrorContexts.Formatting);
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.FormattedValueType"]/*' />
        [
            Browsable(false)
        ]
        public virtual Type FormattedValueType
        {
            get
            {
                return this.ValueType;
            }
        }

        private TypeConverter FormattedValueTypeConverter
        {
            get
            {
                TypeConverter formattedValueTypeConverter = null;
                if (this.FormattedValueType != null)
                {
                    if (this.DataGridView != null)
                    {
                        formattedValueTypeConverter = this.DataGridView.GetCachedTypeConverter(this.FormattedValueType);
                    }
                    else
                    {
                        formattedValueTypeConverter = TypeDescriptor.GetConverter(this.FormattedValueType);
                    }
                }
                return formattedValueTypeConverter;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.Frozen"]/*' />
        [
            Browsable(false)
        ]
        public virtual bool Frozen
        {
            get
            {
                Debug.Assert((this.State & DataGridViewElementStates.Frozen) == 0);

                if (this.DataGridView != null && this.RowIndex >= 0 && this.ColumnIndex >= 0)
                {
                    Debug.Assert(this.DataGridView.Rows.GetRowState(this.RowIndex) == this.DataGridView.Rows.SharedRow(this.RowIndex).State);
                    return this.owningColumn.Frozen && this.owningRow.Frozen;
                }
                else if (this.owningRow != null && (this.owningRow.DataGridView == null || this.RowIndex >= 0))
                {
                    return this.owningRow.Frozen;
                }
                return false;
            }
        }

        internal bool HasErrorText
        {
            get
            {
                return this.Properties.ContainsObject(PropCellErrorText) && this.Properties.GetObject(PropCellErrorText) != null;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.HasStyle"]/*' />
        [
            Browsable(false)
        ]
        public bool HasStyle
        {
            get
            {
                return this.Properties.ContainsObject(PropCellStyle) && this.Properties.GetObject(PropCellStyle) != null;
            }
        }

        internal bool HasToolTipText
        {
            get
            {
                return this.Properties.ContainsObject(PropCellToolTipText) && this.Properties.GetObject(PropCellToolTipText) != null;
            }
        }

        internal bool HasValue
        {
            get
            {
                return this.Properties.ContainsObject(PropCellValue) && this.Properties.GetObject(PropCellValue) != null;
            }
        }

        internal virtual bool HasValueType
        {
            get
            {
                return this.Properties.ContainsObject(PropCellValueType) && this.Properties.GetObject(PropCellValueType) != null;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.InheritedState"]/*' />
        [
            Browsable(false),
            SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods") // InheritedState/GetInheritedState existence is intentional
        ]
        public DataGridViewElementStates InheritedState
        {
            get
            {
                return GetInheritedState(this.RowIndex);
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.InheritedStyle"]/*' />
        [
            Browsable(false),
            SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods") // InheritedStyle/GetInheritedStyle existence is intentional
        ]
        public DataGridViewCellStyle InheritedStyle
        {
            get
            {
                // this.RowIndex could be -1 if:
                // - the developer makes a mistake & calls dataGridView1.Rows.SharedRow(y).Cells(x).InheritedStyle.
                // - the InheritedStyle of a ColumnHeaderCell is accessed.
                return GetInheritedStyleInternal(this.RowIndex);
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.IsInEditMode"]/*' />
        [
            Browsable(false)
        ]
        public bool IsInEditMode 
        {
            get
            {
                if (this.DataGridView == null)
                {
                    return false;
                }
                if (this.RowIndex == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidOperationOnSharedCell));
                }
                Point ptCurrentCell = this.DataGridView.CurrentCellAddress;
                return ptCurrentCell.X != -1 &&
                       ptCurrentCell.X == this.ColumnIndex &&
                       ptCurrentCell.Y == this.RowIndex &&
                       this.DataGridView.IsCurrentCellInEditMode;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OwningColumn"]/*' />
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public DataGridViewColumn OwningColumn
        {
            get
            {
                return this.owningColumn;
            }
        }

        internal DataGridViewColumn OwningColumnInternal
        {
            set
            {
                this.owningColumn = value;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OwningRow"]/*' />
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public DataGridViewRow OwningRow
        {
            get
            {
                return this.owningRow;
            }
        }

        internal DataGridViewRow OwningRowInternal
        {
            set
            {
                this.owningRow = value;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.PreferredSize"]/*' />
        [
            Browsable(false)
        ]
        public Size PreferredSize
        {
            get
            {
                return GetPreferredSize(this.RowIndex);
            }
        }

        internal PropertyStore Properties
        {
            get
            {
                return this.propertyStore;
            }
        }
        
        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.ReadOnly"]/*' />
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual bool ReadOnly
        {
            get
            {
                if ((this.State & DataGridViewElementStates.ReadOnly) != 0)
                {
                    return true;
                }
                if (this.owningRow != null && (this.owningRow.DataGridView == null || this.RowIndex >= 0) && this.owningRow.ReadOnly)
                {
                    return true;
                }
                if (this.DataGridView != null && this.RowIndex >= 0 && this.ColumnIndex >= 0)
                {
                    Debug.Assert(this.DataGridView.Rows.GetRowState(this.RowIndex) == this.DataGridView.Rows.SharedRow(this.RowIndex).State);
                    return this.owningColumn.ReadOnly;
                }
                return false;
            }
            set
            {
                if (this.DataGridView != null)
                {
                    if (this.RowIndex == -1)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidOperationOnSharedCell));
                    }
                    Debug.Assert(this.ColumnIndex >= 0);
                    // When the whole grid is read-only, we ignore the request.
                    if (value != this.ReadOnly && !this.DataGridView.ReadOnly)
                    {
                        this.DataGridView.OnDataGridViewElementStateChanging(this, -1, DataGridViewElementStates.ReadOnly);
                        this.DataGridView.SetReadOnlyCellCore(this.ColumnIndex, this.RowIndex, value); // this may trigger a call to set_ReadOnlyInternal
                    }
                }
                else
                {
                    if (this.owningRow == null)
                    {
                        if (value != this.ReadOnly)
                        {
                            // We do not allow the read-only flag of a cell to be changed before it is added to a row.
                            throw new InvalidOperationException(string.Format(SR.DataGridViewCell_CannotSetReadOnlyState));
                        }
                    }
                    else
                    {
                        this.owningRow.SetReadOnlyCellCore(this, value);
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
                if (this.DataGridView != null)
                {
                    this.DataGridView.OnDataGridViewElementStateChanged(this, -1, DataGridViewElementStates.ReadOnly);
                }
            }
        }
        
        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.Resizable"]/*' />
        [
            Browsable(false)
        ]
        public virtual bool Resizable
        {
            get
            {
                Debug.Assert((this.State & DataGridViewElementStates.Resizable) == 0);

                if (this.owningRow != null && (this.owningRow.DataGridView == null || this.RowIndex >= 0) && this.owningRow.Resizable == DataGridViewTriState.True)
                {
                    return true;
                }

                if (this.DataGridView != null && this.RowIndex >= 0 && this.ColumnIndex >= 0)
                {
                    Debug.Assert(this.DataGridView.Rows.GetRowState(this.RowIndex) == this.DataGridView.Rows.SharedRow(this.RowIndex).State);
                    return this.owningColumn.Resizable == DataGridViewTriState.True;
                }

                return false;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.RowIndex"]/*' />
        /// <devdoc>
        /// <para>Gets or sets the index of a row in the <see cref='System.Windows.Forms.DataGrid'/> control.</para>
        /// </devdoc>
        [
            Browsable(false)
        ]
        public int RowIndex
        {
            get
            {
                if (this.owningRow == null)
                {
                    return -1;
                }
                return this.owningRow.Index;
            }
        }
        
        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.Selected"]/*' />
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual bool Selected
        {
            get
            {
                if ((this.State & DataGridViewElementStates.Selected) != 0)
                {
                    return true;
                }

                if (this.owningRow != null && (this.owningRow.DataGridView == null || this.RowIndex >= 0) && this.owningRow.Selected)
                {
                    return true;
                }

                if (this.DataGridView != null && this.RowIndex >= 0 && this.ColumnIndex >= 0)
                {
                    Debug.Assert(this.DataGridView.Rows.GetRowState(this.RowIndex) == this.DataGridView.Rows.SharedRow(this.RowIndex).State);
                    return this.owningColumn.Selected;
                }

                return false;
            }
            set
            {
                if (this.DataGridView != null)
                {
                    if (this.RowIndex == -1)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidOperationOnSharedCell));
                    }
                    Debug.Assert(this.ColumnIndex >= 0);
                    this.DataGridView.SetSelectedCellCoreInternal(this.ColumnIndex, this.RowIndex, value); // this may trigger a call to set_SelectedInternal
                }
                else if (value)
                {
                    // We do not allow the selection of a cell to be set before the row gets added to the dataGridView.
                    throw new InvalidOperationException(string.Format(SR.DataGridViewCell_CannotSetSelectedState));
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
                    this.DataGridView.OnDataGridViewElementStateChanged(this, -1, DataGridViewElementStates.Selected);
                }
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.Size"]/*' />
        [
            Browsable(false),
            SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods") // Size/GetSize existence is intentional
        ]
        public Size Size
        {
            get
            {
                return GetSize(this.RowIndex);
            }
        }

        internal Rectangle StdBorderWidths
        {
            get
            {
                if (this.DataGridView != null)
                {
                    DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder = new DataGridViewAdvancedBorderStyle(), dgvabsEffective;
                    dgvabsEffective = AdjustCellBorderStyle(this.DataGridView.AdvancedCellBorderStyle,
                        dataGridViewAdvancedBorderStylePlaceholder,
                        false /*singleVerticalBorderAdded*/,
                        false /*singleHorizontalBorderAdded*/,
                        false /*isFirstDisplayedColumn*/,
                        false /*isFirstDisplayedRow*/);
                    return BorderWidths(dgvabsEffective);
                }
                else
                {
                    return Rectangle.Empty;
                }
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.Style"]/*' />
        [
            Browsable(true)
        ]
        public DataGridViewCellStyle Style
        {
            get
            {
                DataGridViewCellStyle dgvcs = (DataGridViewCellStyle)this.Properties.GetObject(PropCellStyle);
                if (dgvcs == null)
                {
                    dgvcs = new DataGridViewCellStyle();
                    dgvcs.AddScope(this.DataGridView, DataGridViewCellStyleScopes.Cell);
                    this.Properties.SetObject(PropCellStyle, dgvcs);
                }
                return dgvcs;
            }
            set
            {
                DataGridViewCellStyle dgvcs = null;
                if (this.HasStyle)
                {
                    dgvcs = this.Style;
                    dgvcs.RemoveScope(DataGridViewCellStyleScopes.Cell);
                }
                if (value != null || this.Properties.ContainsObject(PropCellStyle))
                {
                    if (value != null)
                    {
                        value.AddScope(this.DataGridView, DataGridViewCellStyleScopes.Cell);
                    }
                    this.Properties.SetObject(PropCellStyle, value);
                }
                if (((dgvcs != null && value == null) || 
                    (dgvcs == null && value != null) || 
                    (dgvcs != null && value != null && !dgvcs.Equals(this.Style))) && this.DataGridView != null)
                {
                    this.DataGridView.OnCellStyleChanged(this);
                }
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.Tag"]/*' />
        [
            SRCategory(nameof(SR.CatData)),
            Localizable(false), 
            Bindable(true), 
            SRDescription(nameof(SR.ControlTagDescr)), 
            DefaultValue(null), 
            TypeConverter(typeof(StringConverter))
        ]
        public object Tag
        {
            get
            {
                return this.Properties.GetObject(PropCellTag);
            }
            set
            {
                if (value != null || this.Properties.ContainsObject(PropCellTag))
                {
                    this.Properties.SetObject(PropCellTag, value);
                }
            }
        }
        
        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.ToolTipText"]/*' />
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public string ToolTipText
        {
            get
            {
                return GetToolTipText(this.RowIndex);
            }
            set
            {
                this.ToolTipTextInternal = value;
            }
        }

        private string ToolTipTextInternal
        {
            get
            {
                object toolTipText = this.Properties.GetObject(PropCellToolTipText);
                return (toolTipText == null) ? string.Empty : (string)toolTipText;
            }
            set
            {
                string toolTipText = this.ToolTipTextInternal;
                if (!String.IsNullOrEmpty(value) || this.Properties.ContainsObject(PropCellToolTipText))
                {
                    this.Properties.SetObject(PropCellToolTipText, value);
                }
                if (this.DataGridView != null && !toolTipText.Equals(this.ToolTipTextInternal))
                {
                    this.DataGridView.OnCellToolTipTextChanged(this);
                }
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.Value"]/*' />
        [
            Browsable(false)
        ]
        public object Value
        {
            get
            {
                Debug.Assert(this.RowIndex >= -1);
                return GetValue(this.RowIndex);
            }
            set
            {
                Debug.Assert(this.RowIndex >= -1);
                SetValue(this.RowIndex, value);
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.ValueType"]/*' />
        [
            Browsable(false)
        ]
        public virtual Type ValueType
        {
            get
            {
                Type cellValueType = (Type) this.Properties.GetObject(PropCellValueType);
                if (cellValueType == null && this.OwningColumn != null)
                {
                    cellValueType = this.OwningColumn.ValueType;
                }

                return cellValueType;
            }
            set
            {
                if (value != null || this.Properties.ContainsObject(PropCellValueType))
                {
                    this.Properties.SetObject(PropCellValueType, value);
                }
            }
        }

        private TypeConverter ValueTypeConverter
        {
            get
            {
                TypeConverter valueTypeConverter = null;
                if (this.OwningColumn != null)
                {
                    valueTypeConverter = this.OwningColumn.BoundColumnConverter;
                }
                if (valueTypeConverter == null && this.ValueType != null)
                {
                    if (this.DataGridView != null)
                    {
                        valueTypeConverter = this.DataGridView.GetCachedTypeConverter(this.ValueType);
                    }
                    else
                    {
                        valueTypeConverter = TypeDescriptor.GetConverter(this.ValueType);
                    }
                }
                return valueTypeConverter;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.Visible"]/*' />
        [
            Browsable(false)
        ]
        public virtual bool Visible
        {
            get
            {
                Debug.Assert((this.State & DataGridViewElementStates.Visible) == 0);

                if (this.DataGridView != null && this.RowIndex >= 0 && this.ColumnIndex >= 0)
                {
                    Debug.Assert(this.DataGridView.Rows.GetRowState(this.RowIndex) == this.DataGridView.Rows.SharedRow(this.RowIndex).State);
                    return this.owningColumn.Visible && this.owningRow.Visible;
                }
                else if (this.owningRow != null && (this.owningRow.DataGridView == null || this.RowIndex >= 0))
                {
                    return this.owningRow.Visible;
                }
                return false;
            }
        }


        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.AdjustCellBorderStyle"]/*' />
        [
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public virtual DataGridViewAdvancedBorderStyle AdjustCellBorderStyle(DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStyleInput,
            DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder,
            bool singleVerticalBorderAdded,
            bool singleHorizontalBorderAdded,
            bool isFirstDisplayedColumn,
            bool isFirstDisplayedRow)
        {
            switch (dataGridViewAdvancedBorderStyleInput.All)
            {
                case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                    Debug.Fail("DataGridViewRow.AdjustCellBorderStyle - Unexpected DataGridViewAdvancedCellBorderStyle.OutsetPartial");
                    break;

                case DataGridViewAdvancedCellBorderStyle.Single:
                    if (this.DataGridView != null && this.DataGridView.RightToLeftInternal)
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
                    if (this.DataGridView != null && this.DataGridView.AdvancedCellBorderStyle == dataGridViewAdvancedBorderStyleInput)
                    {
                        switch (this.DataGridView.CellBorderStyle)
                        {
                            case DataGridViewCellBorderStyle.SingleVertical:
                                if (this.DataGridView.RightToLeftInternal)
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

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.BorderWidths"]/*' />
        protected virtual Rectangle BorderWidths(DataGridViewAdvancedBorderStyle advancedBorderStyle)
        {
            Rectangle rect = new Rectangle();

            rect.X = (advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.None) ? 0 : 1;
            if (advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.OutsetDouble || advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.InsetDouble)
            {
                rect.X++;
            }

            rect.Y = (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None) ? 0 : 1;
            if (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.OutsetDouble || advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.InsetDouble)
            {
                rect.Y++;
            }

            rect.Width = (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None) ? 0 : 1;
            if (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.OutsetDouble || advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.InsetDouble)
            {
                rect.Width++;
            }

            rect.Height = (advancedBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.None) ? 0 : 1;
            if (advancedBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.OutsetDouble || advancedBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.InsetDouble)
            {
                rect.Height++;
            }

            if (this.owningColumn != null)
            {
                if (this.DataGridView != null && this.DataGridView.RightToLeftInternal)
                {
                    rect.X += this.owningColumn.DividerWidth;
                }
                else
                {
                    rect.Width += this.owningColumn.DividerWidth;
                }
            }
            if (this.owningRow != null)
            {
                rect.Height += this.owningRow.DividerHeight;
            }

            return rect;
        }

        // Called when the row that owns the editing control gets unshared.
        // Too late in the product cycle to make this a public method.
        internal virtual void CacheEditingControl()
        {
        }

        /* Unused at this point.
        internal DataGridViewElementStates CellStateFromColumnRowStates()
        {
            Debug.Assert(this.DataGridView != null);
            Debug.Assert(this.RowIndex >= 0);
            return CellStateFromColumnRowStates(this.owningRow.State);
        }*/
        
        internal DataGridViewElementStates CellStateFromColumnRowStates(DataGridViewElementStates rowState)
        {
            Debug.Assert(this.DataGridView != null);
            Debug.Assert(this.ColumnIndex >= 0);
            DataGridViewElementStates orFlags = DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.Selected;
            DataGridViewElementStates andFlags = DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Visible;
            DataGridViewElementStates cellState = (this.owningColumn.State & orFlags);
            cellState |= (rowState & orFlags);
            cellState |= ((this.owningColumn.State & andFlags) & (rowState & andFlags));
            return cellState;
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.ClickUnsharesRow"]/*' />
        [
            SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly") // Unshares is OK.
        ]
        protected virtual bool ClickUnsharesRow(DataGridViewCellEventArgs e)
        {
            return false;
        }

        internal bool ClickUnsharesRowInternal(DataGridViewCellEventArgs e)
        {
            return ClickUnsharesRow(e);
        }

        internal void CloneInternal(DataGridViewCell dataGridViewCell)
        {
            if (this.HasValueType)
            {
                dataGridViewCell.ValueType = this.ValueType;
            }
            if (this.HasStyle)
            {
                dataGridViewCell.Style = new DataGridViewCellStyle(this.Style);
            }
            if (this.HasErrorText)
            {
                dataGridViewCell.ErrorText = this.ErrorTextInternal;
            }
            if (this.HasToolTipText)
            {
                dataGridViewCell.ToolTipText = this.ToolTipTextInternal;
            }
            if (this.ContextMenuStripInternal != null)
            {
                dataGridViewCell.ContextMenuStrip = this.ContextMenuStripInternal.Clone();
            }
            dataGridViewCell.StateInternal = this.State & ~DataGridViewElementStates.Selected;
            dataGridViewCell.Tag = this.Tag;
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.Clone"]/*' />
        public virtual object Clone()
        {
            DataGridViewCell dataGridViewCell = (DataGridViewCell) System.Activator.CreateInstance(this.GetType());
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
            Debug.Assert(this.DataGridView != null);
            bool singleVerticalBorderAdded = !this.DataGridView.RowHeadersVisible && this.DataGridView.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single;
            bool singleHorizontalBorderAdded = !this.DataGridView.ColumnHeadersVisible && this.DataGridView.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single;
            DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder = new DataGridViewAdvancedBorderStyle();

            if (rowIndex > -1 && this.OwningColumn != null)
            {
                // Inner cell case
                dgvabsEffective = AdjustCellBorderStyle(this.DataGridView.AdvancedCellBorderStyle,
                    dataGridViewAdvancedBorderStylePlaceholder,
                    singleVerticalBorderAdded,
                    singleHorizontalBorderAdded,
                    this.ColumnIndex == this.DataGridView.FirstDisplayedColumnIndex /*isFirstDisplayedColumn*/,
                    rowIndex == this.DataGridView.FirstDisplayedRowIndex /*isFirstDisplayedRow*/);
                DataGridViewElementStates rowState = this.DataGridView.Rows.GetRowState(rowIndex);
                cellState = this.CellStateFromColumnRowStates(rowState);
                cellState |= this.State;
            }
            else if (this.OwningColumn != null)
            {
                // Column header cell case
                Debug.Assert(rowIndex == -1);
                Debug.Assert(this is DataGridViewColumnHeaderCell, "if the row index == -1 and we have an owning column this should be a column header cell");
                DataGridViewColumn dataGridViewColumn = this.DataGridView.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None);
                bool isLastVisibleColumn = (dataGridViewColumn != null && dataGridViewColumn.Index == this.ColumnIndex);
                dgvabsEffective = this.DataGridView.AdjustColumnHeaderBorderStyle(this.DataGridView.AdvancedColumnHeadersBorderStyle, 
                    dataGridViewAdvancedBorderStylePlaceholder,
                    this.ColumnIndex == this.DataGridView.FirstDisplayedColumnIndex, 
                    isLastVisibleColumn);
                cellState = this.OwningColumn.State | this.State;
            }
            else if (this.OwningRow != null)
            {
                // Row header cell case
                Debug.Assert(this is DataGridViewRowHeaderCell);
                dgvabsEffective = this.OwningRow.AdjustRowHeaderBorderStyle(this.DataGridView.AdvancedRowHeadersBorderStyle,
                    dataGridViewAdvancedBorderStylePlaceholder,
                    singleVerticalBorderAdded,
                    singleHorizontalBorderAdded,
                    rowIndex == this.DataGridView.FirstDisplayedRowIndex /*isFirstDisplayedRow*/,
                    rowIndex == this.DataGridView.Rows.GetLastRow(DataGridViewElementStates.Visible) /*isLastVisibleRow*/);
                cellState = this.OwningRow.GetState(rowIndex) | this.State;
            }
            else
            {
                Debug.Assert(this.OwningColumn == null);
                Debug.Assert(this.OwningRow == null);
                Debug.Assert(rowIndex == -1);
                // TopLeft header cell case
                dgvabsEffective = this.DataGridView.AdjustedTopLeftHeaderBorderStyle;
                cellState = this.State;
            }

            cellBounds = new Rectangle(new Point(0, 0), GetSize(rowIndex)); 
        }

        internal Rectangle ComputeErrorIconBounds(Rectangle cellValueBounds)
        {
            if (cellValueBounds.Width >= DATAGRIDVIEWCELL_iconMarginWidth * 2 + iconsWidth &&
                cellValueBounds.Height >= DATAGRIDVIEWCELL_iconMarginHeight * 2 + iconsHeight)
            {
                Rectangle bmpRect = new Rectangle(this.DataGridView.RightToLeftInternal ?
                                      cellValueBounds.Left + DATAGRIDVIEWCELL_iconMarginWidth :
                                      cellValueBounds.Right - DATAGRIDVIEWCELL_iconMarginWidth - iconsWidth,
                                      cellValueBounds.Y + (cellValueBounds.Height - iconsHeight) / 2,
                                      iconsWidth,
                                      iconsHeight);
                return bmpRect;
            }
            else
            {
                return Rectangle.Empty;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.ContentClickUnsharesRow"]/*' />
        [
            SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly") // Unshares is OK.
        ]
        protected virtual bool ContentClickUnsharesRow(DataGridViewCellEventArgs e)
        {
            return false;
        }

        internal bool ContentClickUnsharesRowInternal(DataGridViewCellEventArgs e)
        {
            return ContentClickUnsharesRow(e);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.ContentDoubleClickUnsharesRow"]/*' />
        [
            SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly") // Unshares is OK.
        ]
        protected virtual bool ContentDoubleClickUnsharesRow(DataGridViewCellEventArgs e)
        {
            return false;
        }

        internal bool ContentDoubleClickUnsharesRowInternal(DataGridViewCellEventArgs e)
        {
            return ContentDoubleClickUnsharesRow(e);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.CreateAccessibilityInstance"]/*' />
        protected virtual AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewCellAccessibleObject(this);
        }

        private void DetachContextMenuStrip(object sender, EventArgs e)
        {
            this.ContextMenuStripInternal = null;
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.DetachEditingControl"]/*' />
        [
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public virtual void DetachEditingControl()
        {
            DataGridView dgv = this.DataGridView;
            if (dgv == null || dgv.EditingControl == null)
            {
                throw new InvalidOperationException();
            }
            if (dgv.EditingControl.ParentInternal != null)
            {
                if (dgv.EditingControl.ContainsFocus)
                {
                    ContainerControl cc = dgv.GetContainerControlInternal() as ContainerControl;
                    if (cc != null && (dgv.EditingControl == cc.ActiveControl || dgv.EditingControl.Contains(cc.ActiveControl)))
                    {
                        dgv.FocusInternal();
                    }
                    else
                    {
                        // We don't want the grid to get the keyboard focus
                        // when the editing control gets parented to the parking window, 
                        // because some other window is in the middle of receiving the focus.
                        UnsafeNativeMethods.SetFocus(new HandleRef(null, IntPtr.Zero));
                    }
                }
                Debug.Assert(dgv.EditingControl.ParentInternal == dgv.EditingPanel);
                Debug.Assert(dgv.EditingPanel.Controls.Contains(dgv.EditingControl));
                dgv.EditingPanel.Controls.Remove(dgv.EditingControl);
                Debug.Assert(dgv.EditingControl.ParentInternal == null);
            }
            if (dgv.EditingPanel.ParentInternal != null)
            {
                Debug.Assert(dgv.EditingPanel.ParentInternal == dgv);
                Debug.Assert(dgv.Controls.Contains(dgv.EditingPanel));
                ((DataGridView.DataGridViewControlCollection)dgv.Controls).RemoveInternal(dgv.EditingPanel);
                Debug.Assert(dgv.EditingPanel.ParentInternal == null);
            }

            Debug.Assert(dgv.EditingControl.ParentInternal == null);
            Debug.Assert(dgv.EditingPanel.ParentInternal == null);
            Debug.Assert(dgv.EditingPanel.Controls.Count == 0);

            // Since the tooltip is removed when the editing control is shown,
            // the CurrentMouseLocation is reset to DATAGRIDVIEWCELL_flagAreaNotSet 
            // so that the tooltip appears again on mousemove after the editing.
            this.CurrentMouseLocation = DATAGRIDVIEWCELL_flagAreaNotSet;
        }
        
        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.Dispose"]/*' />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.Dispose2"]/*' />
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

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.DoubleClickUnsharesRow"]/*' />
        [
            SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly") // Unshares is OK.
        ]
        protected virtual bool DoubleClickUnsharesRow(DataGridViewCellEventArgs e)
        {
            return false;
        }

        internal bool DoubleClickUnsharesRowInternal(DataGridViewCellEventArgs e)
        {
            return DoubleClickUnsharesRow(e);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.EnterUnsharesRow"]/*' />
        [
            SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly") // Unshares is OK.
        ]
        protected virtual bool EnterUnsharesRow(int rowIndex, bool throughMouseClick)
        {
            return false;
        }

        internal bool EnterUnsharesRowInternal(int rowIndex, bool throughMouseClick)
        {
            return EnterUnsharesRow(rowIndex, throughMouseClick);
        }

        internal static void FormatPlainText(string s, bool csv, TextWriter output, ref bool escapeApplied)
        {
            if (s == null)
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
        internal static void FormatPlainTextAsHtml(string s, TextWriter output)
        {
            if (s == null)
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
                        #if ENTITY_ENCODE_HIGH_ASCII_CHARS
                        // The seemingly arbitrary 160 comes from RFC
                        if (ch >= 160 && ch < 256)
                        {
                            output.Write("&#");
                            output.Write(((int)ch).ToString(NumberFormatInfo.InvariantInfo));
                            output.Write(';');
                            break;
                        }
                        #endif // ENTITY_ENCODE_HIGH_ASCII_CHARS
                        output.Write(ch);
                        break;
                }
                prevCh = ch;
            }
        }

        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        private static Bitmap GetBitmap(string bitmapName)
        {
            Bitmap b = new Bitmap(typeof(DataGridViewCell), bitmapName);
            b.MakeTransparent();
            if (DpiHelper.IsScalingRequired) {
                Bitmap scaledBitmap = DpiHelper.CreateResizedBitmap(b, new Size(iconsWidth, iconsHeight));
                if (scaledBitmap != null) {
                    b.Dispose();
                    b = scaledBitmap;
                }
            }
            return b;
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.GetClipboardContent"]/*' />
        protected virtual object GetClipboardContent(int rowIndex,
                                                     bool firstCell,
                                                     bool lastCell,
                                                     bool inFirstRow,
                                                     bool inLastRow,
                                                     string format)
        {
            if (this.DataGridView == null)
            {
                return null;
            }
            // Header Cell classes override this implementation - this implementation is only for inner cells
            if (rowIndex < 0 || rowIndex >= this.DataGridView.Rows.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            // Assuming (like in other places in this class) that the formatted value is independent of the style colors.
            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false);
            object formattedValue = null;
            if (this.DataGridView.IsSharedCellSelected(this, rowIndex))
            {
                formattedValue = GetEditedFormattedValue(GetValue(rowIndex), rowIndex, ref dataGridViewCellStyle, DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.ClipboardContent);
            }

            StringBuilder sb = new StringBuilder(64);

            if (String.Equals(format, DataFormats.Html, StringComparison.OrdinalIgnoreCase))
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
                if (formattedValue != null)
                {
                    FormatPlainTextAsHtml(formattedValue.ToString(), new StringWriter(sb, CultureInfo.CurrentCulture));
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
                bool csv = String.Equals(format, DataFormats.CommaSeparatedValue, StringComparison.OrdinalIgnoreCase);
                if (csv ||
                    String.Equals(format, DataFormats.Text, StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(format, DataFormats.UnicodeText, StringComparison.OrdinalIgnoreCase))
                {
                    if (formattedValue != null)
                    {
                        if (firstCell && lastCell && inFirstRow && inLastRow)
                        {
                            sb.Append(formattedValue.ToString());
                        }
                        else
                        {
                            bool escapeApplied = false;
                            int insertionPoint = sb.Length;
                            FormatPlainText(formattedValue.ToString(), csv, new StringWriter(sb, CultureInfo.CurrentCulture), ref escapeApplied);
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

        internal object GetClipboardContentInternal(int rowIndex, 
                                                    bool firstCell, 
                                                    bool lastCell, 
                                                    bool inFirstRow, 
                                                    bool inLastRow, 
                                                    string format)
        {
            return GetClipboardContent(rowIndex, firstCell, lastCell, inFirstRow, inLastRow, format);
        }

        internal ContextMenuStrip GetContextMenuStrip(int rowIndex)
        {
            ContextMenuStrip contextMenuStrip = this.ContextMenuStripInternal;
            if (this.DataGridView != null &&
                (this.DataGridView.VirtualMode || this.DataGridView.DataSource != null))
            {
                contextMenuStrip = this.DataGridView.OnCellContextMenuStripNeeded(this.ColumnIndex, rowIndex, contextMenuStrip);
            }
            return contextMenuStrip;
        }

        internal void GetContrastedPens(Color baseline, ref Pen darkPen, ref Pen lightPen)
        {
            Debug.Assert(this.DataGridView != null);

            int darkDistance = ColorDistance(baseline, SystemColors.ControlDark);
            int lightDistance = ColorDistance(baseline, SystemColors.ControlLightLight);

            if (SystemInformation.HighContrast)
            {
                if (darkDistance < DATAGRIDVIEWCELL_highConstrastThreshold)
                {
                    darkPen = this.DataGridView.GetCachedPen(ControlPaint.DarkDark(baseline));
                }
                else
                {
                    darkPen = this.DataGridView.GetCachedPen(SystemColors.ControlDark);
                }
                if (lightDistance < DATAGRIDVIEWCELL_highConstrastThreshold)
                {
                    lightPen = this.DataGridView.GetCachedPen(ControlPaint.LightLight(baseline));
                }
                else
                {
                    lightPen = this.DataGridView.GetCachedPen(SystemColors.ControlLightLight);
                }
            }
            else
            {
                if (darkDistance < DATAGRIDVIEWCELL_constrastThreshold)
                {
                    darkPen = this.DataGridView.GetCachedPen(ControlPaint.Dark(baseline));
                }
                else
                {
                    darkPen = this.DataGridView.GetCachedPen(SystemColors.ControlDark);
                }
                if (lightDistance < DATAGRIDVIEWCELL_constrastThreshold)
                {
                    lightPen = this.DataGridView.GetCachedPen(ControlPaint.Light(baseline));
                }
                else
                {
                    lightPen = this.DataGridView.GetCachedPen(SystemColors.ControlLightLight);
                }
            }
        }

#if DGV_GDI
        internal void GetContrastedWindowsPens(Color baseline, ref WindowsPen darkPen, ref WindowsPen lightPen)
        {
            Debug.Assert(this.DataGridView != null);

            int darkDistance = ColorDistance(baseline, SystemColors.ControlDark);
            int lightDistance = ColorDistance(baseline, SystemColors.ControlLightLight);

            if (SystemInformation.HighContrast)
            {
                if (darkDistance < DATAGRIDVIEWCELL_highConstrastThreshold)
                {
                    darkPen = this.DataGridView.GetCachedWindowsPen(ControlPaint.DarkDark(baseline));
                }
                else
                {
                    darkPen = this.DataGridView.GetCachedWindowsPen(SystemColors.ControlDark);
                }
                if (lightDistance < DATAGRIDVIEWCELL_highConstrastThreshold)
                {
                    lightPen = this.DataGridView.GetCachedWindowsPen(ControlPaint.LightLight(baseline));
                }
                else
                {
                    lightPen = this.DataGridView.GetCachedWindowsPen(SystemColors.ControlLightLight);
                }
            }
            else
            {
                if (darkDistance < DATAGRIDVIEWCELL_constrastThreshold)
                {
                    darkPen = this.DataGridView.GetCachedWindowsPen(ControlPaint.Dark(baseline));
                }
                else
                {
                    darkPen = this.DataGridView.GetCachedWindowsPen(SystemColors.ControlDark);
                }
                if (lightDistance < DATAGRIDVIEWCELL_constrastThreshold)
                {
                    lightPen = this.DataGridView.GetCachedWindowsPen(ControlPaint.Light(baseline));
                }
                else
                {
                    lightPen = this.DataGridView.GetCachedWindowsPen(SystemColors.ControlLightLight);
                }
            }
        }
#endif // DGV_GDI

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.GetContentBounds1"]/*' />
        public Rectangle GetContentBounds(int rowIndex)
        {
            if (this.DataGridView == null)
            {
                return Rectangle.Empty;
            }
            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false /*includeColors*/);
            using (Graphics g = WindowsFormsUtils.CreateMeasurementGraphics())
            {
                return GetContentBounds(g, dataGridViewCellStyle, rowIndex);
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.GetContentBounds2"]/*' />
        protected virtual Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            return Rectangle.Empty;
        }

        internal object GetEditedFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle dataGridViewCellStyle, DataGridViewDataErrorContexts context)
        {
            Debug.Assert(this.DataGridView != null);
            Point ptCurrentCell = this.DataGridView.CurrentCellAddress;
            if (this.ColumnIndex == ptCurrentCell.X && rowIndex == ptCurrentCell.Y)
            {
                IDataGridViewEditingControl dgvectl = (IDataGridViewEditingControl)this.DataGridView.EditingControl;
                if (dgvectl != null)
                {
                    return dgvectl.GetEditingControlFormattedValue(context);
                }
                IDataGridViewEditingCell dgvecell = this as IDataGridViewEditingCell;
                if (dgvecell != null && this.DataGridView.IsCurrentCellInEditMode)
                {
                    return dgvecell.GetEditingCellFormattedValue(context);
                }
                return GetFormattedValue(value, rowIndex, ref dataGridViewCellStyle, null, null, context);
            }
            return GetFormattedValue(value, rowIndex, ref dataGridViewCellStyle, null, null, context);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.GetEditedFormattedValue"]/*' />
        public object GetEditedFormattedValue(int rowIndex, DataGridViewDataErrorContexts context)
        {
            if (this.DataGridView == null)
            {
                return null;
            }
            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false /*includeColors*/);
            return GetEditedFormattedValue(GetValue(rowIndex), rowIndex, ref dataGridViewCellStyle, context);
        }

        internal Rectangle GetErrorIconBounds(int rowIndex)
        {
            Debug.Assert(this.DataGridView != null);
            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false /*includeColors*/);
            using (Graphics g = WindowsFormsUtils.CreateMeasurementGraphics())
            {
                return GetErrorIconBounds(g, dataGridViewCellStyle, rowIndex);
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.GetErrorIconBounds"]/*' />
        protected virtual Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            return Rectangle.Empty;
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.GetErrorText"]/*' />
        protected internal virtual string GetErrorText(int rowIndex)
        {
            string errorText = string.Empty;
            object objErrorText = this.Properties.GetObject(PropCellErrorText);
            if (objErrorText != null)
            {
                errorText = (string) objErrorText;
            }
            else if (this.DataGridView != null && 
                     rowIndex != -1 &&
                     rowIndex != this.DataGridView.NewRowIndex &&
                     this.OwningColumn != null &&
                     this.OwningColumn.IsDataBound &&
                     this.DataGridView.DataConnection != null)
            {
                errorText = this.DataGridView.DataConnection.GetError(this.OwningColumn.BoundColumnIndex, this.ColumnIndex, rowIndex);
            }

            if (this.DataGridView != null && (this.DataGridView.VirtualMode || this.DataGridView.DataSource != null) &&
                this.ColumnIndex >= 0 && rowIndex >= 0)
            {
                errorText = this.DataGridView.OnCellErrorTextNeeded(this.ColumnIndex, rowIndex, errorText);
            }
            return errorText;
        }

        internal object GetFormattedValue(int rowIndex, ref DataGridViewCellStyle cellStyle, DataGridViewDataErrorContexts context)
        {
            if (this.DataGridView == null)
            {
                return null;
            }
            else
            {
                return GetFormattedValue(GetValue(rowIndex), rowIndex, ref cellStyle, null, null, context);
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.GetFormattedValue"]/*' />
        [
            SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")  // using ref is OK.
        ]
        protected virtual object GetFormattedValue(object value, 
                                                   int rowIndex, 
                                                   ref DataGridViewCellStyle cellStyle, 
                                                   TypeConverter valueTypeConverter,
                                                   TypeConverter formattedValueTypeConverter,
                                                   DataGridViewDataErrorContexts context)
        {
            if (this.DataGridView == null)
            {
                return null;
            }

            DataGridViewCellFormattingEventArgs gdvcfe = this.DataGridView.OnCellFormatting(this.ColumnIndex, rowIndex, value, this.FormattedValueType, cellStyle);
            cellStyle = gdvcfe.CellStyle;
            bool formattingApplied = gdvcfe.FormattingApplied;
            Object formattedValue = gdvcfe.Value;
            bool checkFormattedValType = true;

            if (!formattingApplied &&
                this.FormattedValueType != null &&
                (formattedValue == null || !this.FormattedValueType.IsAssignableFrom(formattedValue.GetType())))
            {
                try
                {
                    formattedValue = Formatter.FormatObject(formattedValue,
                                                            this.FormattedValueType,
                                                            valueTypeConverter == null ? this.ValueTypeConverter : valueTypeConverter, /*sourceConverter*/
                                                            formattedValueTypeConverter == null ? this.FormattedValueTypeConverter : formattedValueTypeConverter, /*targetConverter*/
                                                            cellStyle.Format,
                                                            cellStyle.FormatProvider,
                                                            cellStyle.NullValue,
                                                            cellStyle.DataSourceNullValue);
                }
                catch (Exception exception)
                {
                    if (ClientUtils.IsCriticalException(exception))
                    {
                        throw;
                    }
                    // Formatting failed, raise OnDataError event.
                    DataGridViewDataErrorEventArgs dgvdee = new DataGridViewDataErrorEventArgs(exception,
                        this.ColumnIndex,
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
                (formattedValue == null || this.FormattedValueType == null || !this.FormattedValueType.IsAssignableFrom(formattedValue.GetType())))
            {
                if (formattedValue == null &&
                    cellStyle.NullValue == null &&
                    this.FormattedValueType != null &&
                    !typeof(System.ValueType).IsAssignableFrom(this.FormattedValueType))
                {
                    // null is an acceptable formatted value 
                    return null;
                }
                Exception exception = null;
                if (this.FormattedValueType == null)
                {
                    exception = new FormatException(string.Format(SR.DataGridViewCell_FormattedValueTypeNull));
                }
                else
                {
                    exception = new FormatException(string.Format(SR.DataGridViewCell_FormattedValueHasWrongType));
                }
                DataGridViewDataErrorEventArgs dgvdee = new DataGridViewDataErrorEventArgs(exception,
                    this.ColumnIndex,
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

        static internal DataGridViewFreeDimension GetFreeDimensionFromConstraint(Size constraintSize)
        {
            if (constraintSize.Width < 0 || constraintSize.Height < 0)
            {
                throw new ArgumentException(string.Format(SR.InvalidArgument, "constraintSize", constraintSize.ToString()));
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
                    throw new ArgumentException(string.Format(SR.InvalidArgument, "constraintSize", constraintSize.ToString()));
                }
            }
        }

        internal int GetHeight(int rowIndex)
        {
            if (this.DataGridView == null)
            {
                return -1;
            }
            Debug.Assert(this.owningRow != null);
            return this.owningRow.GetHeight(rowIndex);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.GetInheritedContextMenuStrip"]/*' />
        public virtual ContextMenuStrip GetInheritedContextMenuStrip(int rowIndex)
        {
            if (this.DataGridView != null)
            {
                if (rowIndex < 0 || rowIndex >= this.DataGridView.Rows.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                if (this.ColumnIndex < 0)
                {
                    throw new InvalidOperationException();
                }
                Debug.Assert(this.ColumnIndex < this.DataGridView.Columns.Count);
            }

            ContextMenuStrip contextMenuStrip = GetContextMenuStrip(rowIndex);
            if (contextMenuStrip != null)
            {
                return contextMenuStrip;
            }
            
            if (this.owningRow != null)
            {
                contextMenuStrip = this.owningRow.GetContextMenuStrip(rowIndex);
                if (contextMenuStrip != null)
                {
                    return contextMenuStrip;
                }
            }

            if (this.owningColumn != null)
            {
                contextMenuStrip = this.owningColumn.ContextMenuStrip;
                if (contextMenuStrip != null)
                {
                    return contextMenuStrip;
                }
            }

            if (this.DataGridView != null)
            {
                return this.DataGridView.ContextMenuStrip;
            }
            else
            {
                return null;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.GetInheritedState"]/*' />
        public virtual DataGridViewElementStates GetInheritedState(int rowIndex)
        {
            DataGridViewElementStates state = this.State | DataGridViewElementStates.ResizableSet;

            if (this.DataGridView == null)
            {
                Debug.Assert(this.RowIndex == -1);
                if (rowIndex != -1)
                {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, "rowIndex", rowIndex.ToString(CultureInfo.CurrentCulture)));
                }
                if (this.owningRow != null)
                {
                    state |= (this.owningRow.GetState(-1) & (DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible));
                    if (this.owningRow.GetResizable(rowIndex) == DataGridViewTriState.True)
                    {
                        state |= DataGridViewElementStates.Resizable;
                    }
                }
                return state;
            }

            // Header Cell classes override this implementation - this implementation is only for inner cells
            if (rowIndex < 0 || rowIndex >= this.DataGridView.Rows.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            Debug.Assert(this.owningColumn != null);
            Debug.Assert(this.owningRow != null);
            Debug.Assert(this.ColumnIndex >= 0);

            if (this.DataGridView.Rows.SharedRow(rowIndex) != this.owningRow)
            {
                throw new ArgumentException(string.Format(SR.InvalidArgument, "rowIndex", rowIndex.ToString(CultureInfo.CurrentCulture)));
            }

            DataGridViewElementStates rowEffectiveState = this.DataGridView.Rows.GetRowState(rowIndex);
            state |= (rowEffectiveState & (DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected));
            state |= (this.owningColumn.State & (DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected));

            if (this.owningRow.GetResizable(rowIndex) == DataGridViewTriState.True ||
                this.owningColumn.Resizable == DataGridViewTriState.True)
            {
                state |= DataGridViewElementStates.Resizable;
            }
            if (this.owningColumn.Visible && this.owningRow.GetVisible(rowIndex))
            {
                state |= DataGridViewElementStates.Visible;
                if (this.owningColumn.Displayed && this.owningRow.GetDisplayed(rowIndex))
                {
                    state |= DataGridViewElementStates.Displayed;
                }
            }
            if (this.owningColumn.Frozen && this.owningRow.GetFrozen(rowIndex))
            {
                state |= DataGridViewElementStates.Frozen;
            }

#if DEBUG
            DataGridViewElementStates stateDebug = DataGridViewElementStates.ResizableSet;
            if (this.Displayed)
            {
                stateDebug |= DataGridViewElementStates.Displayed;
            }
            if (this.Frozen)
            {
                stateDebug |= DataGridViewElementStates.Frozen;
            }
            if (this.ReadOnly)
            {
                stateDebug |= DataGridViewElementStates.ReadOnly;
            }
            if (this.Resizable)
            {
                stateDebug |= DataGridViewElementStates.Resizable;
            }
            if (this.Selected)
            {
                stateDebug |= DataGridViewElementStates.Selected;
            }
            if (this.Visible)
            {
                stateDebug |= DataGridViewElementStates.Visible;
            }
            Debug.Assert(state == stateDebug || this.DataGridView.Rows.SharedRow(rowIndex).Index == -1);
#endif
            return state;
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.GetInheritedStyle"]/*' />
        public virtual DataGridViewCellStyle GetInheritedStyle(DataGridViewCellStyle inheritedCellStyle, int rowIndex, bool includeColors)
        {
            if (this.DataGridView == null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_CellNeedsDataGridViewForInheritedStyle));
            }
            if (rowIndex < 0 || rowIndex >= this.DataGridView.Rows.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            if (this.ColumnIndex < 0)
            {
                throw new InvalidOperationException();
            }
            Debug.Assert(this.ColumnIndex < this.DataGridView.Columns.Count);

            DataGridViewCellStyle inheritedCellStyleTmp;
            if (inheritedCellStyle == null)
            {
                inheritedCellStyleTmp = this.DataGridView.PlaceholderCellStyle;
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

            DataGridViewCellStyle cellStyle = null;
            if (this.HasStyle)
            {
                cellStyle = this.Style;
                Debug.Assert(cellStyle != null);
            }

            DataGridViewCellStyle rowStyle = null;
            if (this.DataGridView.Rows.SharedRow(rowIndex).HasDefaultCellStyle)
            {
                rowStyle = this.DataGridView.Rows.SharedRow(rowIndex).DefaultCellStyle;
                Debug.Assert(rowStyle != null);
            }

            DataGridViewCellStyle columnStyle = null;
            if (this.owningColumn.HasDefaultCellStyle)
            {
                columnStyle = this.owningColumn.DefaultCellStyle;
                Debug.Assert(columnStyle != null);
            }

            DataGridViewCellStyle dataGridViewStyle = this.DataGridView.DefaultCellStyle;
            Debug.Assert(dataGridViewStyle != null);

            if (includeColors)
            {
                if (cellStyle != null && !cellStyle.BackColor.IsEmpty)
                {
                    inheritedCellStyleTmp.BackColor = cellStyle.BackColor;
                } 
                else if (rowStyle != null && !rowStyle.BackColor.IsEmpty)
                {
                    inheritedCellStyleTmp.BackColor = rowStyle.BackColor;
                }
                else if (!this.DataGridView.RowsDefaultCellStyle.BackColor.IsEmpty &&
                    (rowIndex % 2 == 0 || this.DataGridView.AlternatingRowsDefaultCellStyle.BackColor.IsEmpty))
                {
                    inheritedCellStyleTmp.BackColor = this.DataGridView.RowsDefaultCellStyle.BackColor;
                }
                else if (rowIndex % 2 == 1 && !this.DataGridView.AlternatingRowsDefaultCellStyle.BackColor.IsEmpty)
                {
                    inheritedCellStyleTmp.BackColor = this.DataGridView.AlternatingRowsDefaultCellStyle.BackColor;
                }
                else if (columnStyle != null && !columnStyle.BackColor.IsEmpty)
                {
                    inheritedCellStyleTmp.BackColor = columnStyle.BackColor;
                }
                else
                {
                    inheritedCellStyleTmp.BackColor = dataGridViewStyle.BackColor;
                }

                if (cellStyle != null && !cellStyle.ForeColor.IsEmpty)
                {
                    inheritedCellStyleTmp.ForeColor = cellStyle.ForeColor;
                } 
                else if (rowStyle != null && !rowStyle.ForeColor.IsEmpty)
                {
                    inheritedCellStyleTmp.ForeColor = rowStyle.ForeColor;
                }
                else if (!this.DataGridView.RowsDefaultCellStyle.ForeColor.IsEmpty &&
                    (rowIndex % 2 == 0 || this.DataGridView.AlternatingRowsDefaultCellStyle.ForeColor.IsEmpty))
                {
                    inheritedCellStyleTmp.ForeColor = this.DataGridView.RowsDefaultCellStyle.ForeColor;
                }
                else if (rowIndex % 2 == 1 && !this.DataGridView.AlternatingRowsDefaultCellStyle.ForeColor.IsEmpty)
                {
                    inheritedCellStyleTmp.ForeColor = this.DataGridView.AlternatingRowsDefaultCellStyle.ForeColor;
                }
                else if (columnStyle != null && !columnStyle.ForeColor.IsEmpty)
                {
                    inheritedCellStyleTmp.ForeColor = columnStyle.ForeColor;
                }
                else
                {
                    inheritedCellStyleTmp.ForeColor = dataGridViewStyle.ForeColor;
                }

                if (cellStyle != null && !cellStyle.SelectionBackColor.IsEmpty)
                {
                    inheritedCellStyleTmp.SelectionBackColor = cellStyle.SelectionBackColor;
                } 
                else if (rowStyle != null && !rowStyle.SelectionBackColor.IsEmpty)
                {
                    inheritedCellStyleTmp.SelectionBackColor = rowStyle.SelectionBackColor;
                }
                else if (!this.DataGridView.RowsDefaultCellStyle.SelectionBackColor.IsEmpty &&
                    (rowIndex % 2 == 0 || this.DataGridView.AlternatingRowsDefaultCellStyle.SelectionBackColor.IsEmpty))
                {
                    inheritedCellStyleTmp.SelectionBackColor = this.DataGridView.RowsDefaultCellStyle.SelectionBackColor;
                }
                else if (rowIndex % 2 == 1 && !this.DataGridView.AlternatingRowsDefaultCellStyle.SelectionBackColor.IsEmpty)
                {
                    inheritedCellStyleTmp.SelectionBackColor = this.DataGridView.AlternatingRowsDefaultCellStyle.SelectionBackColor;
                }
                else if (columnStyle != null && !columnStyle.SelectionBackColor.IsEmpty)
                {
                    inheritedCellStyleTmp.SelectionBackColor = columnStyle.SelectionBackColor;
                }
                else
                {
                    inheritedCellStyleTmp.SelectionBackColor = dataGridViewStyle.SelectionBackColor;
                }

                if (cellStyle != null && !cellStyle.SelectionForeColor.IsEmpty)
                {
                    inheritedCellStyleTmp.SelectionForeColor = cellStyle.SelectionForeColor;
                } 
                else if (rowStyle != null && !rowStyle.SelectionForeColor.IsEmpty)
                {
                    inheritedCellStyleTmp.SelectionForeColor = rowStyle.SelectionForeColor;
                }
                else if (!this.DataGridView.RowsDefaultCellStyle.SelectionForeColor.IsEmpty &&
                    (rowIndex % 2 == 0 || this.DataGridView.AlternatingRowsDefaultCellStyle.SelectionForeColor.IsEmpty))
                {
                    inheritedCellStyleTmp.SelectionForeColor = this.DataGridView.RowsDefaultCellStyle.SelectionForeColor;
                }
                else if (rowIndex % 2 == 1 && !this.DataGridView.AlternatingRowsDefaultCellStyle.SelectionForeColor.IsEmpty)
                {
                    inheritedCellStyleTmp.SelectionForeColor = this.DataGridView.AlternatingRowsDefaultCellStyle.SelectionForeColor;
                }
                else if (columnStyle != null && !columnStyle.SelectionForeColor.IsEmpty)
                {
                    inheritedCellStyleTmp.SelectionForeColor = columnStyle.SelectionForeColor;
                }
                else
                {
                    inheritedCellStyleTmp.SelectionForeColor = dataGridViewStyle.SelectionForeColor;
                }
            }

            if (cellStyle != null && cellStyle.Font != null)
            {
                inheritedCellStyleTmp.Font = cellStyle.Font;
            } 
            else if (rowStyle != null && rowStyle.Font != null)
            {
                inheritedCellStyleTmp.Font = rowStyle.Font;
            }
            else if (this.DataGridView.RowsDefaultCellStyle.Font != null &&
                (rowIndex % 2 == 0 || this.DataGridView.AlternatingRowsDefaultCellStyle.Font == null))
            {
                inheritedCellStyleTmp.Font = this.DataGridView.RowsDefaultCellStyle.Font;
            }
            else if (rowIndex % 2 == 1 && this.DataGridView.AlternatingRowsDefaultCellStyle.Font != null)
            {
                inheritedCellStyleTmp.Font = this.DataGridView.AlternatingRowsDefaultCellStyle.Font;
            }
            else if (columnStyle != null && columnStyle.Font != null)
            {
                inheritedCellStyleTmp.Font = columnStyle.Font;
            }
            else
            {
                inheritedCellStyleTmp.Font = dataGridViewStyle.Font;
            }

            if (cellStyle != null && !cellStyle.IsNullValueDefault)
            {
                inheritedCellStyleTmp.NullValue = cellStyle.NullValue;
            }
            else if (rowStyle != null && !rowStyle.IsNullValueDefault)
            {
                inheritedCellStyleTmp.NullValue = rowStyle.NullValue;
            }
            else if (!this.DataGridView.RowsDefaultCellStyle.IsNullValueDefault &&
                     (rowIndex % 2 == 0 || this.DataGridView.AlternatingRowsDefaultCellStyle.IsNullValueDefault))
            {
                inheritedCellStyleTmp.NullValue = this.DataGridView.RowsDefaultCellStyle.NullValue;
            }
            else if (rowIndex % 2 == 1 &&
                     !this.DataGridView.AlternatingRowsDefaultCellStyle.IsNullValueDefault)
            {
                inheritedCellStyleTmp.NullValue = this.DataGridView.AlternatingRowsDefaultCellStyle.NullValue;
            }
            else if (columnStyle != null && !columnStyle.IsNullValueDefault)
            {
                inheritedCellStyleTmp.NullValue = columnStyle.NullValue;
            }
            else
            {
                inheritedCellStyleTmp.NullValue = dataGridViewStyle.NullValue;
            }

            if (cellStyle != null && !cellStyle.IsDataSourceNullValueDefault)
            {
                inheritedCellStyleTmp.DataSourceNullValue = cellStyle.DataSourceNullValue;
            }
            else if (rowStyle != null && !rowStyle.IsDataSourceNullValueDefault)
            {
                inheritedCellStyleTmp.DataSourceNullValue = rowStyle.DataSourceNullValue;
            }
            else if (!this.DataGridView.RowsDefaultCellStyle.IsDataSourceNullValueDefault &&
                (rowIndex % 2 == 0 || this.DataGridView.AlternatingRowsDefaultCellStyle.IsDataSourceNullValueDefault))
            {
                inheritedCellStyleTmp.DataSourceNullValue = this.DataGridView.RowsDefaultCellStyle.DataSourceNullValue;
            }
            else if (rowIndex % 2 == 1 &&
                !this.DataGridView.AlternatingRowsDefaultCellStyle.IsDataSourceNullValueDefault)
            {
                inheritedCellStyleTmp.DataSourceNullValue = this.DataGridView.AlternatingRowsDefaultCellStyle.DataSourceNullValue;
            }
            else if (columnStyle != null && !columnStyle.IsDataSourceNullValueDefault)
            {
                inheritedCellStyleTmp.DataSourceNullValue = columnStyle.DataSourceNullValue;
            }
            else
            {
                inheritedCellStyleTmp.DataSourceNullValue = dataGridViewStyle.DataSourceNullValue;
            }

            if (cellStyle != null && cellStyle.Format.Length != 0)
            {
                inheritedCellStyleTmp.Format = cellStyle.Format;
            } 
            else if (rowStyle != null && rowStyle.Format.Length != 0)
            {
                inheritedCellStyleTmp.Format = rowStyle.Format;
            }
            else if (this.DataGridView.RowsDefaultCellStyle.Format.Length != 0 && 
                (rowIndex % 2 == 0 || this.DataGridView.AlternatingRowsDefaultCellStyle.Format.Length == 0))
            {
                inheritedCellStyleTmp.Format = this.DataGridView.RowsDefaultCellStyle.Format;
            }
            else if (rowIndex % 2 == 1 && this.DataGridView.AlternatingRowsDefaultCellStyle.Format.Length != 0)
            {
                inheritedCellStyleTmp.Format = this.DataGridView.AlternatingRowsDefaultCellStyle.Format;
            }
            else if (columnStyle != null && columnStyle.Format.Length != 0)
            {
                inheritedCellStyleTmp.Format = columnStyle.Format;
            }
            else
            {
                inheritedCellStyleTmp.Format = dataGridViewStyle.Format;
            }

            if (cellStyle != null && !cellStyle.IsFormatProviderDefault)
            {
                inheritedCellStyleTmp.FormatProvider = cellStyle.FormatProvider;
            }
            else if (rowStyle != null && !rowStyle.IsFormatProviderDefault)
            {
                inheritedCellStyleTmp.FormatProvider = rowStyle.FormatProvider;
            }
            else if (!this.DataGridView.RowsDefaultCellStyle.IsFormatProviderDefault &&
                     (rowIndex % 2 == 0 || this.DataGridView.AlternatingRowsDefaultCellStyle.IsFormatProviderDefault))
            {
                inheritedCellStyleTmp.FormatProvider = this.DataGridView.RowsDefaultCellStyle.FormatProvider;
            }
            else if (rowIndex % 2 == 1 && !this.DataGridView.AlternatingRowsDefaultCellStyle.IsFormatProviderDefault)
            {
                inheritedCellStyleTmp.FormatProvider = this.DataGridView.AlternatingRowsDefaultCellStyle.FormatProvider;
            }
            else if (columnStyle != null && !columnStyle.IsFormatProviderDefault)
            {
                inheritedCellStyleTmp.FormatProvider = columnStyle.FormatProvider;
            }
            else
            {
                inheritedCellStyleTmp.FormatProvider = dataGridViewStyle.FormatProvider;
            }

            if (cellStyle != null && cellStyle.Alignment != DataGridViewContentAlignment.NotSet)
            {
                inheritedCellStyleTmp.AlignmentInternal = cellStyle.Alignment;
            } 
            else if (rowStyle != null && rowStyle.Alignment != DataGridViewContentAlignment.NotSet)
            {
                inheritedCellStyleTmp.AlignmentInternal = rowStyle.Alignment;
            }
            else if (this.DataGridView.RowsDefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet && 
                (rowIndex % 2 == 0 || this.DataGridView.AlternatingRowsDefaultCellStyle.Alignment == DataGridViewContentAlignment.NotSet))
            {
                inheritedCellStyleTmp.AlignmentInternal = this.DataGridView.RowsDefaultCellStyle.Alignment;
            }
            else if (rowIndex % 2 == 1 && this.DataGridView.AlternatingRowsDefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet)
            {
                inheritedCellStyleTmp.AlignmentInternal = this.DataGridView.AlternatingRowsDefaultCellStyle.Alignment;
            }
            else if (columnStyle != null && columnStyle.Alignment != DataGridViewContentAlignment.NotSet)
            {
                inheritedCellStyleTmp.AlignmentInternal = columnStyle.Alignment;
            }
            else
            {
                Debug.Assert(dataGridViewStyle.Alignment != DataGridViewContentAlignment.NotSet);
                inheritedCellStyleTmp.AlignmentInternal = dataGridViewStyle.Alignment;
            }

            if (cellStyle != null && cellStyle.WrapMode != DataGridViewTriState.NotSet)
            {
                inheritedCellStyleTmp.WrapModeInternal = cellStyle.WrapMode;
            } 
            else if (rowStyle != null && rowStyle.WrapMode != DataGridViewTriState.NotSet)
            {
                inheritedCellStyleTmp.WrapModeInternal = rowStyle.WrapMode;
            }
            else if (this.DataGridView.RowsDefaultCellStyle.WrapMode != DataGridViewTriState.NotSet && 
                (rowIndex % 2 == 0 || this.DataGridView.AlternatingRowsDefaultCellStyle.WrapMode == DataGridViewTriState.NotSet))
            {
                inheritedCellStyleTmp.WrapModeInternal = this.DataGridView.RowsDefaultCellStyle.WrapMode;
            }
            else if (rowIndex % 2 == 1 && this.DataGridView.AlternatingRowsDefaultCellStyle.WrapMode != DataGridViewTriState.NotSet)
            {
                inheritedCellStyleTmp.WrapModeInternal = this.DataGridView.AlternatingRowsDefaultCellStyle.WrapMode;
            }
            else if (columnStyle != null && columnStyle.WrapMode != DataGridViewTriState.NotSet)
            {
                inheritedCellStyleTmp.WrapModeInternal = columnStyle.WrapMode;
            }
            else
            {
                Debug.Assert(dataGridViewStyle.WrapMode != DataGridViewTriState.NotSet);
                inheritedCellStyleTmp.WrapModeInternal = dataGridViewStyle.WrapMode;
            }

            if (cellStyle != null && cellStyle.Tag != null)
            {
                inheritedCellStyleTmp.Tag = cellStyle.Tag;
            }
            else if (rowStyle != null && rowStyle.Tag != null)
            {
                inheritedCellStyleTmp.Tag = rowStyle.Tag;
            }
            else if (this.DataGridView.RowsDefaultCellStyle.Tag != null && 
                (rowIndex % 2 == 0 || this.DataGridView.AlternatingRowsDefaultCellStyle.Tag == null))
            {
                inheritedCellStyleTmp.Tag = this.DataGridView.RowsDefaultCellStyle.Tag;
            }
            else if (rowIndex % 2 == 1 && this.DataGridView.AlternatingRowsDefaultCellStyle.Tag != null)
            {
                inheritedCellStyleTmp.Tag = this.DataGridView.AlternatingRowsDefaultCellStyle.Tag;
            }
            else if (columnStyle != null && columnStyle.Tag != null)
            {
                inheritedCellStyleTmp.Tag = columnStyle.Tag;
            }
            else
            {
                inheritedCellStyleTmp.Tag = dataGridViewStyle.Tag;
            }

            if (cellStyle != null && cellStyle.Padding != Padding.Empty)
            {
                inheritedCellStyleTmp.PaddingInternal = cellStyle.Padding;
            }
            else if (rowStyle != null && rowStyle.Padding != Padding.Empty)
            {
                inheritedCellStyleTmp.PaddingInternal = rowStyle.Padding;
            }
            else if (this.DataGridView.RowsDefaultCellStyle.Padding != Padding.Empty && 
                (rowIndex % 2 == 0 || this.DataGridView.AlternatingRowsDefaultCellStyle.Padding == Padding.Empty))
            {
                inheritedCellStyleTmp.PaddingInternal = this.DataGridView.RowsDefaultCellStyle.Padding;
            }
            else if (rowIndex % 2 == 1 && this.DataGridView.AlternatingRowsDefaultCellStyle.Padding != Padding.Empty)
            {
                inheritedCellStyleTmp.PaddingInternal = this.DataGridView.AlternatingRowsDefaultCellStyle.Padding;
            }
            else if (columnStyle != null && columnStyle.Padding != Padding.Empty)
            {
                inheritedCellStyleTmp.PaddingInternal = columnStyle.Padding;
            }
            else
            {
                inheritedCellStyleTmp.PaddingInternal = dataGridViewStyle.Padding;
            }

            return inheritedCellStyleTmp;
        }

        internal DataGridViewCellStyle GetInheritedStyleInternal(int rowIndex)
        {
            return GetInheritedStyle(null, rowIndex, true /*includeColors*/);
        }

        internal int GetPreferredHeight(int rowIndex, int width)
        {
            Debug.Assert(width > 0);

            if (this.DataGridView == null)
            {
                return -1;
            }

            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false);
            using( Graphics g = WindowsFormsUtils.CreateMeasurementGraphics())
            {
                return GetPreferredSize(g, dataGridViewCellStyle, rowIndex, new Size(width, 0)).Height;
            }
        }

        internal Size GetPreferredSize(int rowIndex)
        {
            if (this.DataGridView == null)
            {
                return new Size(-1, -1);
            }
            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false);
            using (Graphics g = WindowsFormsUtils.CreateMeasurementGraphics())
            {
                return GetPreferredSize(g, dataGridViewCellStyle, rowIndex, Size.Empty);
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.GetPreferredSize"]/*' />
        protected virtual Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
        {
            return new Size(-1, -1);
        }

        internal static int GetPreferredTextHeight(Graphics g,
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
                return DataGridViewCell.MeasureTextHeight(g, text, cellStyle.Font, maxWidth, flags, out widthTruncated);
            }
            else
            {
                Size size = DataGridViewCell.MeasureTextSize(g, text, cellStyle.Font, flags);
                widthTruncated = size.Width > maxWidth;
                return size.Height;
            }
        }

        internal int GetPreferredWidth(int rowIndex, int height)
        {
            Debug.Assert(height > 0);

            if (this.DataGridView == null)
            {
                return -1;
            }

            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false);
            using (Graphics g = WindowsFormsUtils.CreateMeasurementGraphics())
            {
                return GetPreferredSize(g, dataGridViewCellStyle, rowIndex, new Size(0, height)).Width;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.GetSize"]/*' />
        protected virtual Size GetSize(int rowIndex)
        {
            if (this.DataGridView == null)
            {
                return new Size(-1, -1);
            }
            if (rowIndex == -1)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedCell, "Size"));
            }
            Debug.Assert(this.owningColumn != null);
            Debug.Assert(this.owningRow != null);
            return new Size(this.owningColumn.Thickness, this.owningRow.GetHeight(rowIndex));
        }

        private string GetToolTipText(int rowIndex)
        {
            string toolTipText = this.ToolTipTextInternal;
            if (this.DataGridView != null &&
                (this.DataGridView.VirtualMode || this.DataGridView.DataSource != null))
            {
                toolTipText = this.DataGridView.OnCellToolTipTextNeeded(this.ColumnIndex, rowIndex, toolTipText);
            }
            return toolTipText;
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.GetValue"]/*' />
        protected virtual object GetValue(int rowIndex)
        {
            DataGridView dataGridView = this.DataGridView;
            if (dataGridView != null)
            {
                if (rowIndex < 0 || rowIndex >= dataGridView.Rows.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                if (this.ColumnIndex < 0)
                {
                    throw new InvalidOperationException();
                }
                Debug.Assert(this.ColumnIndex < dataGridView.Columns.Count);
            }
            if (dataGridView == null ||
                (dataGridView.AllowUserToAddRowsInternal && rowIndex > -1 && rowIndex == dataGridView.NewRowIndex && rowIndex != dataGridView.CurrentCellAddress.Y) ||
                (!dataGridView.VirtualMode && this.OwningColumn != null && !this.OwningColumn.IsDataBound) ||
                rowIndex == -1 ||
                this.ColumnIndex == -1)
            {
                return this.Properties.GetObject(PropCellValue);
            }
            else if (this.OwningColumn != null && this.OwningColumn.IsDataBound)
            {
                DataGridView.DataGridViewDataConnection dataConnection = dataGridView.DataConnection;
                if (dataConnection == null)
                {
                    return null;
                }
                else if (dataConnection.CurrencyManager.Count <= rowIndex)
                {
                    return this.Properties.GetObject(PropCellValue);
                }
                else
                {
                    return dataConnection.GetValue(this.OwningColumn.BoundColumnIndex, this.ColumnIndex, rowIndex);
                }
            }
            else
            {
                Debug.Assert(rowIndex >= 0);
                Debug.Assert(this.ColumnIndex >= 0);
                return dataGridView.OnCellValueNeeded(this.ColumnIndex, rowIndex);
            }
        }

        internal object GetValueInternal(int rowIndex)
        {
            return GetValue(rowIndex);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.InitializeEditingControl"]/*' />
        [
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public virtual void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            DataGridView dgv = this.DataGridView;
            if (dgv == null || dgv.EditingControl == null)
            {
                throw new InvalidOperationException();
            }
            // Only add the control to the dataGridView's children if this hasn't been done yet since
            // InitializeEditingControl can be called several times.
            if (dgv.EditingControl.ParentInternal == null)
            {
                // Add editing control to the dataGridView hierarchy
                dgv.EditingControl.CausesValidation = dgv.CausesValidation;
                dgv.EditingPanel.CausesValidation = dgv.CausesValidation;
                dgv.EditingControl.Visible = true;
                Debug.Assert(!dgv.EditingPanel.ContainsFocus);
                dgv.EditingPanel.Visible = false;
                Debug.Assert(dgv.EditingPanel.ParentInternal == null);
                dgv.Controls.Add(dgv.EditingPanel);
                dgv.EditingPanel.Controls.Add(dgv.EditingControl);
                Debug.Assert(dgv.IsSharedCellVisible(this, rowIndex));
            }
            Debug.Assert(dgv.EditingControl.ParentInternal == dgv.EditingPanel);
            Debug.Assert(dgv.EditingPanel.ParentInternal == dgv);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.KeyDownUnsharesRow"]/*' />
        [
            SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly") // Unshares is OK.
        ]
        protected virtual bool KeyDownUnsharesRow(KeyEventArgs e, int rowIndex)
        {
            return false;
        }

        internal bool KeyDownUnsharesRowInternal(KeyEventArgs e, int rowIndex)
        {
            return KeyDownUnsharesRow(e, rowIndex);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.KeyEntersEditMode"]/*' />
        public virtual bool KeyEntersEditMode(KeyEventArgs e)
        {
            return false;
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.KeyPressUnsharesRow"]/*' />
        [
            SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly") // Unshares is OK.
        ]
        protected virtual bool KeyPressUnsharesRow(KeyPressEventArgs e, int rowIndex)
        {
            return false;
        }

        internal bool KeyPressUnsharesRowInternal(KeyPressEventArgs e, int rowIndex)
        {
            return KeyPressUnsharesRow(e, rowIndex);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.KeyUpUnsharesRow"]/*' />
        [
            SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly") // Unshares is OK.
        ]
        protected virtual bool KeyUpUnsharesRow(KeyEventArgs e, int rowIndex)
        {
            return false;
        }

        internal bool KeyUpUnsharesRowInternal(KeyEventArgs e, int rowIndex)
        {
            return KeyUpUnsharesRow(e, rowIndex);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.LeaveUnsharesRow"]/*' />
        [
            SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly") // Unshares is OK.
        ]
        protected virtual bool LeaveUnsharesRow(int rowIndex, bool throughMouseClick)
        {
            return false;
        }

        internal bool LeaveUnsharesRowInternal(int rowIndex, bool throughMouseClick)
        {
            return LeaveUnsharesRow(rowIndex, throughMouseClick);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.MeasureTextHeight1"]/*' />
        [
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public static int MeasureTextHeight(Graphics graphics, string text, Font font, int maxWidth, TextFormatFlags flags)
        {
            bool widthTruncated;
            return DataGridViewCell.MeasureTextHeight(graphics, text, font, maxWidth, flags, out widthTruncated);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.MeasureTextHeight2"]/*' />
        [
            EditorBrowsable(EditorBrowsableState.Advanced),
            SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters"), // We don't want to use IDeviceContext here.
            SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters") // out param OK here.
        ]
        public static int MeasureTextHeight(Graphics graphics, string text, Font font, int maxWidth, TextFormatFlags flags, out bool widthTruncated)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            if (font == null)
            {
                throw new ArgumentNullException(nameof(font));
            }

            if (maxWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxWidth), string.Format(SR.InvalidLowBoundArgument, "maxWidth", (maxWidth).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
            }

            if (!DataGridViewUtilities.ValidTextFormatFlags(flags))
            {
                throw new InvalidEnumArgumentException(nameof(flags), (int) flags, typeof(TextFormatFlags));
            }

            flags &= textFormatSupportedFlags;
            // Dont use passed in graphics so we can optimze measurement
            Size requiredSize = TextRenderer.MeasureText(text, font, new Size(maxWidth, System.Int32.MaxValue), flags);
            widthTruncated = (requiredSize.Width > maxWidth);
            return requiredSize.Height;
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.MeasureTextPreferredSize"]/*' />
        [
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public static Size MeasureTextPreferredSize(Graphics graphics, string text, Font font, float maxRatio, TextFormatFlags flags)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            if (font == null)
            {
                throw new ArgumentNullException(nameof(font));
            }

            if (maxRatio <= 0.0F)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRatio), string.Format(SR.InvalidLowBoundArgument, "maxRatio", (maxRatio).ToString(CultureInfo.CurrentCulture), "0.0"));
            }

            if (!DataGridViewUtilities.ValidTextFormatFlags(flags))
            {
                throw new InvalidEnumArgumentException(nameof(flags), (int) flags, typeof(TextFormatFlags));
            }

            if (string.IsNullOrEmpty(text))
            {
                return new Size(0, 0);
            }

            Size textOneLineSize = DataGridViewCell.MeasureTextSize(graphics, text, font, flags);
            if ((float)(textOneLineSize.Width / textOneLineSize.Height) <= maxRatio)
            {
                return textOneLineSize;
            }

            flags &= textFormatSupportedFlags;
            float maxWidth = (float) (textOneLineSize.Width * textOneLineSize.Width) / (float) textOneLineSize.Height / maxRatio * 1.1F;
            Size textSize;
            do
            {
                // Dont use passed in graphics so we can optimze measurement
                textSize = TextRenderer.MeasureText(text, font, new Size((int)maxWidth, System.Int32.MaxValue), flags);
                if ((float)(textSize.Width / textSize.Height) <= maxRatio || textSize.Width > (int)maxWidth)
                {
                    return textSize;
                }
                maxWidth = (float)textSize.Width * 0.9F;
            }
            while (maxWidth > 1.0F);
            return textSize;
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.MeasureTextSize"]/*' />
        [
            EditorBrowsable(EditorBrowsableState.Advanced),
            SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters") // We don't want to use IDeviceContext here.
        ]
        public static Size MeasureTextSize(Graphics graphics, string text, Font font, TextFormatFlags flags)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            if (font == null)
            {
                throw new ArgumentNullException(nameof(font));
            }

            if (!DataGridViewUtilities.ValidTextFormatFlags(flags))
            {
                throw new InvalidEnumArgumentException(nameof(flags), (int) flags, typeof(TextFormatFlags));
            }

            flags &= textFormatSupportedFlags;
            // Dont use passed in graphics so we can optimze measurement
            return TextRenderer.MeasureText(text, font, new Size(System.Int32.MaxValue, System.Int32.MaxValue), flags);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.MeasureTextWidth"]/*' />
        [
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public static int MeasureTextWidth(Graphics graphics, string text, Font font, int maxHeight, TextFormatFlags flags)
        {
            if (maxHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxHeight), string.Format(SR.InvalidLowBoundArgument, "maxHeight", (maxHeight).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
            }

            Size oneLineSize = DataGridViewCell.MeasureTextSize(graphics, text, font, flags);
            if (oneLineSize.Height >= maxHeight || (flags & TextFormatFlags.SingleLine) != 0)
            {
                return oneLineSize.Width;
            }
            else
            {
                flags &= textFormatSupportedFlags;
                int lastFittingWidth = oneLineSize.Width;
                float maxWidth = (float) lastFittingWidth * 0.9F;
                Size textSize;
                do
                {
                    // Dont use passed in graphics so we can optimze measurement
                    textSize = TextRenderer.MeasureText(text, font, new Size((int)maxWidth, maxHeight), flags);
                    if (textSize.Height > maxHeight || textSize.Width > (int)maxWidth)
                    {
                        return lastFittingWidth;
                    }
                    else
                    {
                        lastFittingWidth = (int)maxWidth;
                        maxWidth = (float)textSize.Width * 0.9F;
                    }
                }
                while (maxWidth > 1.0F);
                Debug.Assert(textSize.Height <= maxHeight);
                return lastFittingWidth;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.MouseClickUnsharesRow"]/*' />
        [
            SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly") // Unshares is OK.
        ]
        protected virtual bool MouseClickUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return false;
        }

        internal bool MouseClickUnsharesRowInternal(DataGridViewCellMouseEventArgs e)
        {
            return MouseClickUnsharesRow(e);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.MouseDoubleClickUnsharesRow"]/*' />
        [
            SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly") // Unshares is OK.
        ]
        protected virtual bool MouseDoubleClickUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return false;
        }

        internal bool MouseDoubleClickUnsharesRowInternal(DataGridViewCellMouseEventArgs e)
        {
            return MouseDoubleClickUnsharesRow(e);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.MouseDownUnsharesRow"]/*' />
        [
            SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly") // Unshares is OK.
        ]
        protected virtual bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return false;
        }

        internal bool MouseDownUnsharesRowInternal(DataGridViewCellMouseEventArgs e)
        {
            return MouseDownUnsharesRow(e);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.MouseEnterUnsharesRow"]/*' />
        [
            SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly") // Unshares is OK.
        ]
        protected virtual bool MouseEnterUnsharesRow(int rowIndex)
        {
            return false;
        }

        internal bool MouseEnterUnsharesRowInternal(int rowIndex)
        {
            return MouseEnterUnsharesRow(rowIndex);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.MouseLeaveUnsharesRow"]/*' />
        [
            SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly") // Unshares is OK.
        ]
        protected virtual bool MouseLeaveUnsharesRow(int rowIndex)
        {
            return false;
        }

        internal bool MouseLeaveUnsharesRowInternal(int rowIndex)
        {
            return MouseLeaveUnsharesRow(rowIndex);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.MouseMoveUnsharesRow"]/*' />
        [
            SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly") // Unshares is OK.
        ]
        protected virtual bool MouseMoveUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return false;
        }

        internal bool MouseMoveUnsharesRowInternal(DataGridViewCellMouseEventArgs e)
        {
            return MouseMoveUnsharesRow(e);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.MouseUpUnsharesRow"]/*' />
        [
            SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly") // Unshares is OK.
        ]
        protected virtual bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return false;
        }

        internal bool MouseUpUnsharesRowInternal(DataGridViewCellMouseEventArgs e)
        {
            return MouseUpUnsharesRow(e);
        }

        private void OnCellDataAreaMouseEnterInternal(int rowIndex)
        {
            Debug.Assert(this.DataGridView != null);
            if (!this.DataGridView.ShowCellToolTips)
            {
                return;
            }

            // Don't show a tooltip for edited cells with an editing control
            Point ptCurrentCell = this.DataGridView.CurrentCellAddress;
            if (ptCurrentCell.X != -1 &&
                ptCurrentCell.X == this.ColumnIndex &&
                ptCurrentCell.Y == rowIndex &&
                this.DataGridView.EditingControl != null)
            {
                Debug.Assert(this.DataGridView.IsCurrentCellInEditMode);
                return;
            }
            
            // get the tool tip string
            string toolTipText = GetToolTipText(rowIndex);

            if (String.IsNullOrEmpty(toolTipText))
            {
                if (this.FormattedValueType == stringType)
                {
                    if (rowIndex != -1 && this.OwningColumn != null)
                    {
                        int width = GetPreferredWidth(rowIndex, this.OwningRow.Height);
                        int height = GetPreferredHeight(rowIndex, this.OwningColumn.Width);

                        if (this.OwningColumn.Width < width || this.OwningRow.Height < height)
                        {
                            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false /*includeColors*/);
                            string editedFormattedValue = GetEditedFormattedValue(GetValue(rowIndex),
                                                                                    rowIndex,
                                                                                    ref dataGridViewCellStyle,
                                                                                    DataGridViewDataErrorContexts.Display) as string;
                            if (!string.IsNullOrEmpty(editedFormattedValue))
                            {
                                toolTipText = TruncateToolTipText(editedFormattedValue);
                            }
                        }
                    }
                    else if ((rowIndex != -1 && this.OwningRow != null && this.DataGridView.RowHeadersVisible && this.DataGridView.RowHeadersWidth > 0 && this.OwningColumn == null) ||
                             rowIndex == -1)
                    {
                        // we are on a header cell.
                        Debug.Assert(this is DataGridViewHeaderCell);
                        string stringValue = GetValue(rowIndex) as string;
                        if (!string.IsNullOrEmpty(stringValue))
                        {
                            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false);

                            using (Graphics g = WindowsFormsUtils.CreateMeasurementGraphics())
                            {
                                Rectangle contentBounds = GetContentBounds(g, dataGridViewCellStyle, rowIndex);

                                bool widthTruncated = false;
                                int preferredHeight = 0;
                                if (contentBounds.Width > 0)
                                {
                                    preferredHeight = DataGridViewCell.GetPreferredTextHeight(g,
                                                                                              this.DataGridView.RightToLeftInternal,
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
            }
            else if (this.DataGridView.IsRestricted)
            {
                // In semi trust (demand for AllWindows fails), we truncate the tooltip at 256 if it exceeds 288 characters.
                toolTipText = TruncateToolTipText(toolTipText);
            }

            if (!String.IsNullOrEmpty(toolTipText))
            {
                this.DataGridView.ActivateToolTip(true /*activate*/, toolTipText, this.ColumnIndex, rowIndex);
            }

            // for debugging
            // Console.WriteLine("OnCellDATA_AreaMouseENTER. ToolTipText : " + toolTipText);
        }

        private void OnCellDataAreaMouseLeaveInternal()
        {
            if (this.DataGridView.IsDisposed)
            {
                return;
            }

            this.DataGridView.ActivateToolTip(false /*activate*/, string.Empty, -1, -1);
            // for debugging
            // Console.WriteLine("OnCellDATA_AreaMouseLEAVE");
        }

        private void OnCellErrorAreaMouseEnterInternal(int rowIndex)
        {
            string errorText = GetErrorText(rowIndex);
            Debug.Assert(!String.IsNullOrEmpty(errorText), "if we entered the cell error area then an error was painted, so we should have an error");
            this.DataGridView.ActivateToolTip(true /*activate*/, errorText, this.ColumnIndex, rowIndex);

            // for debugging
            // Console.WriteLine("OnCellERROR_AreaMouseENTER. ErrorText : " + errorText);
        }

        private void OnCellErrorAreaMouseLeaveInternal()
        {
            this.DataGridView.ActivateToolTip(false /*activate*/, string.Empty, -1, -1);
            // for debugging
            // Console.WriteLine("OnCellERROR_AreaMouseLEAVE");
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OnClick"]/*' />
        protected virtual void OnClick(DataGridViewCellEventArgs e)
        {
        }

        internal void OnClickInternal(DataGridViewCellEventArgs e)
        {
            OnClick(e);
        }

        internal void OnCommonChange()
        {
            if (this.DataGridView != null && !this.DataGridView.IsDisposed && !this.DataGridView.Disposing)
            {
                if (this.RowIndex == -1)
                {
                    // Invalidate and autosize column
                    this.DataGridView.OnColumnCommonChange(this.ColumnIndex);
                }
                else
                {
                    // Invalidate and autosize cell
                    this.DataGridView.OnCellCommonChange(this.ColumnIndex, this.RowIndex);
                }
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OnContentClick"]/*' />
        protected virtual void OnContentClick(DataGridViewCellEventArgs e)
        {
        }

        internal void OnContentClickInternal(DataGridViewCellEventArgs e)
        {
            OnContentClick(e);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OnContentDoubleClick"]/*' />
        protected virtual void OnContentDoubleClick(DataGridViewCellEventArgs e)
        {
        }

        internal void OnContentDoubleClickInternal(DataGridViewCellEventArgs e)
        {
            OnContentDoubleClick(e);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OnDoubleClick"]/*' />
        protected virtual void OnDoubleClick(DataGridViewCellEventArgs e)
        {
        }

        internal void OnDoubleClickInternal(DataGridViewCellEventArgs e)
        {
            OnDoubleClick(e);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OnEnter"]/*' />
        protected virtual void OnEnter(int rowIndex, bool throughMouseClick)
        {
        }

        internal void OnEnterInternal(int rowIndex, bool throughMouseClick)
        {
            OnEnter(rowIndex, throughMouseClick);
        }

        internal void OnKeyDownInternal(KeyEventArgs e, int rowIndex)
        {
            OnKeyDown(e, rowIndex);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OnKeyDown"]/*' />
        protected virtual void OnKeyDown(KeyEventArgs e, int rowIndex)
        {
        }

        internal void OnKeyPressInternal(KeyPressEventArgs e, int rowIndex)
        {
            OnKeyPress(e, rowIndex);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OnKeyPress"]/*' />
        protected virtual void OnKeyPress(KeyPressEventArgs e, int rowIndex)
        {
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OnKeyUp"]/*' />
        protected virtual void OnKeyUp(KeyEventArgs e, int rowIndex)
        {
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OnKeyUp"]/*' />
        internal void OnKeyUpInternal(KeyEventArgs e, int rowIndex)
        {
            OnKeyUp(e, rowIndex);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OnLeave"]/*' />
        protected virtual void OnLeave(int rowIndex, bool throughMouseClick)
        {
        }

        internal void OnLeaveInternal(int rowIndex, bool throughMouseClick)
        {
            OnLeave(rowIndex, throughMouseClick);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OnMouseClick"]/*' />
        protected virtual void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
        }

        internal void OnMouseClickInternal(DataGridViewCellMouseEventArgs e)
        {
            OnMouseClick(e);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OnMouseDoubleClick"]/*' />
        protected virtual void OnMouseDoubleClick(DataGridViewCellMouseEventArgs e)
        {
        }

        internal void OnMouseDoubleClickInternal(DataGridViewCellMouseEventArgs e)
        {
            OnMouseDoubleClick(e);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OnMouseDown"]/*' />
        protected virtual void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
        }

        internal void OnMouseDownInternal(DataGridViewCellMouseEventArgs e)
        {
            this.DataGridView.CellMouseDownInContentBounds = GetContentBounds(e.RowIndex).Contains(e.X, e.Y);

            if (((this.ColumnIndex < 0 || e.RowIndex < 0) && this.DataGridView.ApplyVisualStylesToHeaderCells) ||
                ((this.ColumnIndex >= 0 && e.RowIndex >= 0) && this.DataGridView.ApplyVisualStylesToInnerCells))
            {
                DataGridView.InvalidateCell(this.ColumnIndex, e.RowIndex);
            }
            OnMouseDown(e);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OnMouseEnter"]/*' />
        protected virtual void OnMouseEnter(int rowIndex)
        {
        }

        internal void OnMouseEnterInternal(int rowIndex)
        {
            OnMouseEnter(rowIndex);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OnMouseLeave"]/*' />
        protected virtual void OnMouseLeave(int rowIndex)
        {
        }

        internal void OnMouseLeaveInternal(int rowIndex)
        {
            switch (this.CurrentMouseLocation)
            {
                case DATAGRIDVIEWCELL_flagDataArea:
                    OnCellDataAreaMouseLeaveInternal();
                    break;
                case DATAGRIDVIEWCELL_flagErrorArea:
                    OnCellErrorAreaMouseLeaveInternal();
                    break;
                case DATAGRIDVIEWCELL_flagAreaNotSet:
                    break;
                default:
                    Debug.Assert(false, "there are only three possible choices for the CurrentMouseLocation");
                    break;
            }

            this.CurrentMouseLocation = DATAGRIDVIEWCELL_flagAreaNotSet;
            OnMouseLeave(rowIndex);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OnMouseMove"]/*' />
        protected virtual void OnMouseMove(DataGridViewCellMouseEventArgs e)
        {
        }

        internal void OnMouseMoveInternal(DataGridViewCellMouseEventArgs e)
        {
            byte mouseLocation = this.CurrentMouseLocation;
            UpdateCurrentMouseLocation(e);
            Debug.Assert(this.CurrentMouseLocation != DATAGRIDVIEWCELL_flagAreaNotSet);
            switch (mouseLocation)
            {
                case DATAGRIDVIEWCELL_flagAreaNotSet:
                    if (this.CurrentMouseLocation == DATAGRIDVIEWCELL_flagDataArea)
                    {
                        OnCellDataAreaMouseEnterInternal(e.RowIndex);
                    }
                    else
                    {
                        OnCellErrorAreaMouseEnterInternal(e.RowIndex);
                    }
                    break;
                case DATAGRIDVIEWCELL_flagDataArea:
                    if (this.CurrentMouseLocation == DATAGRIDVIEWCELL_flagErrorArea)
                    {
                        OnCellDataAreaMouseLeaveInternal();
                        OnCellErrorAreaMouseEnterInternal(e.RowIndex);
                    }
                    break;
                case DATAGRIDVIEWCELL_flagErrorArea:
                    if (this.CurrentMouseLocation == DATAGRIDVIEWCELL_flagDataArea)
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

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OnMouseUp"]/*' />
        protected virtual void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
        }

        internal void OnMouseUpInternal(DataGridViewCellMouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;

            if (((this.ColumnIndex < 0 || e.RowIndex < 0) && this.DataGridView.ApplyVisualStylesToHeaderCells) ||
                ((this.ColumnIndex >= 0 && e.RowIndex >= 0) && this.DataGridView.ApplyVisualStylesToInnerCells))
            {
                this.DataGridView.InvalidateCell(this.ColumnIndex, e.RowIndex);
            }

            if (e.Button == MouseButtons.Left && GetContentBounds(e.RowIndex).Contains(x, y))
            {
                this.DataGridView.OnCommonCellContentClick(e.ColumnIndex, e.RowIndex, e.Clicks > 1);
            }

            if (this.DataGridView != null && e.ColumnIndex < this.DataGridView.Columns.Count && e.RowIndex < this.DataGridView.Rows.Count)
            {
                OnMouseUp(e);
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.OnDataGridViewChanged"]/*' />
        protected override void OnDataGridViewChanged()
        {
            if (this.HasStyle)
            {
                if (this.DataGridView == null)
                {
                    this.Style.RemoveScope(DataGridViewCellStyleScopes.Cell);
                }
                else
                {
                    this.Style.AddScope(this.DataGridView, DataGridViewCellStyleScopes.Cell);
                }
            }
            base.OnDataGridViewChanged();
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.Paint"]/*' />
        protected virtual void Paint(Graphics graphics,
            Rectangle clipBounds,
            Rectangle cellBounds,
            int rowIndex,
            DataGridViewElementStates cellState,
            object value,
            object formattedValue,
            string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
        }

        internal void PaintInternal(Graphics graphics,
            Rectangle clipBounds,
            Rectangle cellBounds,
            int rowIndex,
            DataGridViewElementStates cellState,
            object value,
            object formattedValue,
            string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            Paint(graphics,
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

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.PaintBorder"]/*' />
        protected virtual void PaintBorder(Graphics graphics,
            Rectangle clipBounds,
            Rectangle bounds,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (this.DataGridView == null)
            {
                return;
            }

            // Using system colors for non-single grid colors for now
            int y1, y2;
            
            Pen penControlDark = null, penControlLightLight = null;
            Pen penBackColor = this.DataGridView.GetCachedPen(cellStyle.BackColor);
            Pen penGridColor = this.DataGridView.GridPen;

            GetContrastedPens(cellStyle.BackColor, ref penControlDark, ref penControlLightLight);

            int dividerThickness = this.owningColumn == null ? 0 : this.owningColumn.DividerWidth;
            if (dividerThickness != 0)
            {
                if (dividerThickness > bounds.Width)
                {
                    dividerThickness = bounds.Width;
                }
                Color dividerWidthColor;
                switch (advancedBorderStyle.Right)
                {
                    case DataGridViewAdvancedCellBorderStyle.Single:
                        dividerWidthColor = this.DataGridView.GridPen.Color;
                        break;

                    case DataGridViewAdvancedCellBorderStyle.Inset:
                        dividerWidthColor = SystemColors.ControlLightLight;
                        break;

                    default:   /* ie DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.None */
                        dividerWidthColor = SystemColors.ControlDark;
                        break;
                }
                graphics.FillRectangle(this.DataGridView.GetCachedBrush(dividerWidthColor), 
                                this.DataGridView.RightToLeftInternal ? bounds.X : bounds.Right - dividerThickness, 
                                bounds.Y, 
                                dividerThickness, 
                                bounds.Height);
                if (this.DataGridView.RightToLeftInternal)
                {
                    bounds.X += dividerThickness;
                }
                bounds.Width -= dividerThickness;
                if (bounds.Width <= 0)
                {
                    return;
                }
            }

            dividerThickness = this.owningRow == null ? 0 : this.owningRow.DividerHeight;
            if (dividerThickness != 0)
            {
                if (dividerThickness > bounds.Height)
                {
                    dividerThickness = bounds.Height;
                }
                Color dividerHeightColor;
                switch (advancedBorderStyle.Bottom)
                {
                    case DataGridViewAdvancedCellBorderStyle.Single:
                        dividerHeightColor = this.DataGridView.GridPen.Color;
                        break;

                    case DataGridViewAdvancedCellBorderStyle.Inset:
                        dividerHeightColor = SystemColors.ControlLightLight;
                        break;

                    default:   /* ie DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.None */
                        dividerHeightColor = SystemColors.ControlDark;
                        break;
                }
                graphics.FillRectangle(this.DataGridView.GetCachedBrush(dividerHeightColor), bounds.X, bounds.Bottom - dividerThickness, bounds.Width, dividerThickness);
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
                    if (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                        advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.InsetDouble)
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
                    if (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.OutsetPartial ||
                        advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None)
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
                    if (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.OutsetPartial ||
                        advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None)
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
                    if (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                        advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.InsetDouble)
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
                    if (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.OutsetPartial || 
                        advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None)
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
                    if (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.OutsetPartial || 
                        advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None)
                    {
                        y1--;
                    }
                    if (advancedBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.OutsetPartial ||
                        advancedBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.Inset)
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
                    if (advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                        advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.InsetDouble)
                    {
                        x1++; 
                    }
                    if (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.Inset ||
                        advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.Outset)
                    {
                        x2--; 
                    }
                    graphics.DrawLine(penControlDark, x1, bounds.Y, x2, bounds.Y);
                    break;

                case DataGridViewAdvancedCellBorderStyle.Outset:
                    x1 = bounds.X;
                    x2 = bounds.Right - 1;
                    if (advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                        advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.InsetDouble)
                    {
                        x1++; 
                    }
                    if (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.Inset ||
                        advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.Outset)
                    {
                        x2--; 
                    }
                    graphics.DrawLine(penControlLightLight, x1, bounds.Y, x2, bounds.Y);
                    break;

                case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                    x1 = bounds.X;
                    x2 = bounds.Right - 1;
                    if (advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.None /* && advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.OutsetPartial*/)
                    {
                        x1++;
                        if (advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                            advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.InsetDouble)
                        {
                            x1++;
                        }
                    }
                    if (advancedBorderStyle.Right != DataGridViewAdvancedCellBorderStyle.None /* && advancedBorderStyle.Right != DataGridViewAdvancedCellBorderStyle.OutsetPartial*/)
                    {
                        x2--;
                        if (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.OutsetDouble || 
                            advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.InsetDouble)
                        {
                            x2--;
                        }
                    }
                    graphics.DrawLine(penBackColor, x1, bounds.Y, x2, bounds.Y);
                    graphics.DrawLine(penControlLightLight, x1 + 1, bounds.Y, x2 - 1, bounds.Y);
                    break;

                case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                    x1 = bounds.X;
                    if (advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.OutsetPartial &&
                        advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.None)
                    {
                        x1++;
                    }
                    x2 = bounds.Right - 2;
                    if (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.OutsetPartial ||
                        advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None)
                    {
                        x2++;
                    }
                    graphics.DrawLine(penControlDark, bounds.X, bounds.Y, bounds.Right - 1, bounds.Y);
                    graphics.DrawLine(penControlLightLight, x1, bounds.Y + 1, x2, bounds.Y + 1);
                    break;

                case DataGridViewAdvancedCellBorderStyle.InsetDouble:
                    x1 = bounds.X;
                    if (advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.OutsetPartial &&
                        advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.None)
                    {
                        x1++;
                    }
                    x2 = bounds.Right - 2;
                    if (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.OutsetPartial ||
                        advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None)
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
                    if (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.InsetDouble || 
                        advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                    {
                        x2--;
                    }
                    graphics.DrawLine(penControlDark, x1, bounds.Bottom - 1, x2, bounds.Bottom - 1);
                    break;

                case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                    x1 = bounds.X;
                    x2 = bounds.Right - 1;
                    if (advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.None /* && advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.OutsetPartial*/)
                    {
                        x1++;
                        if (advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                            advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.InsetDouble)
                        {
                            x1++;
                        }
                    }
                    if (advancedBorderStyle.Right != DataGridViewAdvancedCellBorderStyle.None /* && advancedBorderStyle.Right != DataGridViewAdvancedCellBorderStyle.OutsetPartial*/)
                    {
                        x2--;
                        if (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                            advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.InsetDouble)
                        {
                            x2--;
                        }
                    }
                    graphics.DrawLine(penBackColor, x1, bounds.Bottom - 1, x2, bounds.Bottom - 1);
                    graphics.DrawLine(penControlDark, x1 + 1, bounds.Bottom - 1, x2 - 1, bounds.Bottom - 1);
                    break;
            }

#if DGV_GDI
            // Using system colors for non-single grid colors for now
            int y1, y2;

            WindowsPen penControlDark = null, penControlLightLight = null;
            WindowsPen penBackColor = this.DataGridView.GetCachedWindowsPen(cellStyle.BackColor);
            WindowsPen penGridColor = this.DataGridView.GetCachedWindowsPen(this.DataGridView.GridColor);

            GetContrastedWindowsPens(cellStyle.BackColor, ref penControlDark, ref penControlLightLight);

            using (WindowsGraphics windowsGraphics = WindowsGraphics.FromGraphics(graphics))
            {
                int dividerThickness = this.owningColumn == null ? 0 : this.owningColumn.DividerWidth;
                if (dividerThickness != 0)
                {
                    if (dividerThickness > bounds.Width)
                    {
                        dividerThickness = bounds.Width;
                    }
                    Color dividerWidthColor;
                    switch (advancedBorderStyle.Right)
                    {
                        case DataGridViewAdvancedCellBorderStyle.Single:
                            dividerWidthColor = this.DataGridView.GridPen.Color;
                            break;

                        case DataGridViewAdvancedCellBorderStyle.Inset:
                            dividerWidthColor = SystemColors.ControlLightLight;
                            break;

                        default:   /* ie DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.None */
                            dividerWidthColor = SystemColors.ControlDark;
                            break;
                    }
                    windowsGraphics.FillRectangle(this.DataGridView.GetCachedWindowsBrush(dividerWidthColor),
                                    this.DataGridView.RightToLeftInternal ? bounds.X : bounds.Right - dividerThickness,
                                    bounds.Y,
                                    dividerThickness,
                                    bounds.Height);
                    if (this.DataGridView.RightToLeftInternal)
                    {
                        bounds.X += dividerThickness;
                    }
                    bounds.Width -= dividerThickness;
                    if (bounds.Width <= 0)
                    {
                        return;
                    }
                }

                dividerThickness = this.owningRow == null ? 0 : this.owningRow.DividerHeight;
                if (dividerThickness != 0)
                {
                    if (dividerThickness > bounds.Height)
                    {
                        dividerThickness = bounds.Height;
                    }
                    Color dividerHeightColor;
                    switch (advancedBorderStyle.Bottom)
                    {
                        case DataGridViewAdvancedCellBorderStyle.Single:
                            dividerHeightColor = this.DataGridView.GridPen.Color;
                            break;

                        case DataGridViewAdvancedCellBorderStyle.Inset:
                            dividerHeightColor = SystemColors.ControlLightLight;
                            break;

                        default:   /* ie DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.None */
                            dividerHeightColor = SystemColors.ControlDark;
                            break;
                    }
                    windowsGraphics.FillRectangle(this.DataGridView.GetCachedWindowsBrush(dividerHeightColor), bounds.X, bounds.Bottom - dividerThickness, bounds.Width, dividerThickness);
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
                        windowsGraphics.DrawLine(penGridColor, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                        break;

                    case DataGridViewAdvancedCellBorderStyle.Inset:
                        windowsGraphics.DrawLine(penControlDark, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                        break;

                    case DataGridViewAdvancedCellBorderStyle.Outset:
                        windowsGraphics.DrawLine(penControlLightLight, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                        break;

                    case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                        y1 = bounds.Y + 2;
                        y2 = bounds.Bottom - 3;
                        if (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                            advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.InsetDouble)
                        {
                            y1++;
                        }
                        else if (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None)
                        {
                            y1--;
                        }
                        windowsGraphics.DrawLine(penBackColor, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                        windowsGraphics.DrawLine(penControlLightLight, bounds.X, y1, bounds.X, y2);
                        break;

                    case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                        y1 = bounds.Y + 1;
                        y2 = bounds.Bottom - 1;
                        if (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.OutsetPartial ||
                            advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None)
                        {
                            y1--;
                        }
                        if (advancedBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.OutsetPartial)
                        {
                            y2++;
                        }
                        windowsGraphics.DrawLine(penControlDark, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                        windowsGraphics.DrawLine(penControlLightLight, bounds.X + 1, y1, bounds.X + 1, y2);
                        break;

                    case DataGridViewAdvancedCellBorderStyle.InsetDouble:
                        y1 = bounds.Y + 1;
                        y2 = bounds.Bottom - 1;
                        if (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.OutsetPartial ||
                            advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None)
                        {
                            y1--;
                        }
                        if (advancedBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.OutsetPartial)
                        {
                            y2++;
                        }
                        windowsGraphics.DrawLine(penControlLightLight, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                        windowsGraphics.DrawLine(penControlDark, bounds.X + 1, y1, bounds.X + 1, y2);
                        break;
                }

                switch (advancedBorderStyle.Right)
                {
                    case DataGridViewAdvancedCellBorderStyle.Single:
                        windowsGraphics.DrawLine(penGridColor, bounds.Right - 1, bounds.Y, bounds.Right - 1, bounds.Bottom - 1);
                        break;

                    case DataGridViewAdvancedCellBorderStyle.Inset:
                        windowsGraphics.DrawLine(penControlLightLight, bounds.Right - 1, bounds.Y, bounds.Right - 1, bounds.Bottom - 1);
                        break;

                    case DataGridViewAdvancedCellBorderStyle.Outset:
                        windowsGraphics.DrawLine(penControlDark, bounds.Right - 1, bounds.Y, bounds.Right - 1, bounds.Bottom - 1);
                        break;

                    case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                        y1 = bounds.Y + 2;
                        y2 = bounds.Bottom - 3;
                        if (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                            advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.InsetDouble)
                        {
                            y1++;
                        }
                        else if (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None)
                        {
                            y1--;
                        }
                        windowsGraphics.DrawLine(penBackColor, bounds.Right - 1, bounds.Y, bounds.Right - 1, bounds.Bottom - 1);
                        windowsGraphics.DrawLine(penControlDark, bounds.Right - 1, y1, bounds.Right - 1, y2);
                        break;

                    case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                        y1 = bounds.Y + 1;
                        y2 = bounds.Bottom - 1;
                        if (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.OutsetPartial ||
                            advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None)
                        {
                            y1--;
                        }
                        if (advancedBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.OutsetPartial)
                        {
                            y2++;
                        }
                        windowsGraphics.DrawLine(penControlDark, bounds.Right - 2, bounds.Y, bounds.Right - 2, bounds.Bottom - 1);
                        windowsGraphics.DrawLine(penControlLightLight, bounds.Right - 1, y1, bounds.Right - 1, y2);
                        break;

                    case DataGridViewAdvancedCellBorderStyle.InsetDouble:
                        y1 = bounds.Y + 1;
                        y2 = bounds.Bottom - 1;
                        if (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.OutsetPartial ||
                            advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None)
                        {
                            y1--;
                        }
                        if (advancedBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.OutsetPartial ||
                            advancedBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.Inset)
                        {
                            y2++;
                        }
                        windowsGraphics.DrawLine(penControlLightLight, bounds.Right - 2, bounds.Y, bounds.Right - 2, bounds.Bottom - 1);
                        windowsGraphics.DrawLine(penControlDark, bounds.Right - 1, y1, bounds.Right - 1, y2);
                        break;
                }

                int x1, x2;
                switch (advancedBorderStyle.Top)
                {
                    case DataGridViewAdvancedCellBorderStyle.Single:
                        windowsGraphics.DrawLine(penGridColor, bounds.X, bounds.Y, bounds.Right - 1, bounds.Y);
                        break;

                    case DataGridViewAdvancedCellBorderStyle.Inset:
                        x1 = bounds.X;
                        x2 = bounds.Right - 1;
                        if (advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                            advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.InsetDouble)
                        {
                            x1++;
                        }
                        if (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.Inset ||
                            advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.Outset)
                        {
                            x2--;
                        }
                        windowsGraphics.DrawLine(penControlDark, x1, bounds.Y, x2, bounds.Y);
                        break;

                    case DataGridViewAdvancedCellBorderStyle.Outset:
                        x1 = bounds.X;
                        x2 = bounds.Right - 1;
                        if (advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                            advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.InsetDouble)
                        {
                            x1++;
                        }
                        if (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.Inset ||
                            advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.Outset)
                        {
                            x2--;
                        }
                        windowsGraphics.DrawLine(penControlLightLight, x1, bounds.Y, x2, bounds.Y);
                        break;

                    case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                        x1 = bounds.X;
                        x2 = bounds.Right - 1;
                        if (advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.None /* && advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.OutsetPartial*/)
                        {
                            x1++;
                            if (advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                                advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.InsetDouble)
                            {
                                x1++;
                            }
                        }
                        if (advancedBorderStyle.Right != DataGridViewAdvancedCellBorderStyle.None /* && advancedBorderStyle.Right != DataGridViewAdvancedCellBorderStyle.OutsetPartial*/)
                        {
                            x2--;
                            if (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                                advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.InsetDouble)
                            {
                                x2--;
                            }
                        }
                        windowsGraphics.DrawLine(penBackColor, x1, bounds.Y, x2, bounds.Y);
                        windowsGraphics.DrawLine(penControlLightLight, x1 + 1, bounds.Y, x2 - 1, bounds.Y);
                        break;

                    case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                        x1 = bounds.X;
                        if (advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.OutsetPartial &&
                            advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.None)
                        {
                            x1++;
                        }
                        x2 = bounds.Right - 2;
                        if (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.OutsetPartial ||
                            advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None)
                        {
                            x2++;
                        }
                        windowsGraphics.DrawLine(penControlDark, bounds.X, bounds.Y, bounds.Right - 1, bounds.Y);
                        windowsGraphics.DrawLine(penControlLightLight, x1, bounds.Y + 1, x2, bounds.Y + 1);
                        break;

                    case DataGridViewAdvancedCellBorderStyle.InsetDouble:
                        x1 = bounds.X;
                        if (advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.OutsetPartial &&
                            advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.None)
                        {
                            x1++;
                        }
                        x2 = bounds.Right - 2;
                        if (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.OutsetPartial ||
                            advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None)
                        {
                            x2++;
                        }
                        windowsGraphics.DrawLine(penControlLightLight, bounds.X, bounds.Y, bounds.Right - 1, bounds.Y);
                        windowsGraphics.DrawLine(penControlDark, x1, bounds.Y + 1, x2, bounds.Y + 1);
                        break;
                }

                switch (advancedBorderStyle.Bottom)
                {
                    case DataGridViewAdvancedCellBorderStyle.Single:
                        windowsGraphics.DrawLine(penGridColor, bounds.X, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
                        break;

                    case DataGridViewAdvancedCellBorderStyle.Inset:
                        x2 = bounds.Right - 1;
                        if (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.InsetDouble)
                        {
                            x2--;
                        }
                        windowsGraphics.DrawLine(penControlLightLight, bounds.X, bounds.Bottom - 1, x2, bounds.Bottom - 1);
                        break;

                    case DataGridViewAdvancedCellBorderStyle.Outset:
                        x1 = bounds.X;
                        x2 = bounds.Right - 1;
                        if (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.InsetDouble ||
                            advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                        {
                            x2--;
                        }
                        windowsGraphics.DrawLine(penControlDark, x1, bounds.Bottom - 1, x2, bounds.Bottom - 1);
                        break;

                    case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                        x1 = bounds.X;
                        x2 = bounds.Right - 1;
                        if (advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.None /* && advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.OutsetPartial*/)
                        {
                            x1++;
                            if (advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                                advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.InsetDouble)
                            {
                                x1++;
                            }
                        }
                        if (advancedBorderStyle.Right != DataGridViewAdvancedCellBorderStyle.None /* && advancedBorderStyle.Right != DataGridViewAdvancedCellBorderStyle.OutsetPartial*/)
                        {
                            x2--;
                            if (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                                advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.InsetDouble)
                            {
                                x2--;
                            }
                        }
                        windowsGraphics.DrawLine(penBackColor, x1, bounds.Bottom - 1, x2, bounds.Bottom - 1);
                        windowsGraphics.DrawLine(penControlDark, x1 + 1, bounds.Bottom - 1, x2 - 1, bounds.Bottom - 1);
                        break;
                }
            }
#endif // DGV_GDI
        }

        internal static bool PaintContentBackground(DataGridViewPaintParts paintParts)
        {
            return (paintParts & DataGridViewPaintParts.ContentBackground) != 0;
        }

        internal static bool PaintContentForeground(DataGridViewPaintParts paintParts)
        {
            return (paintParts & DataGridViewPaintParts.ContentForeground) != 0;
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.PaintErrorIcon"]/*' />
        protected virtual void PaintErrorIcon(Graphics graphics, Rectangle clipBounds, Rectangle cellValueBounds, string errorText)
        {
            if (!string.IsNullOrEmpty(errorText) &&
                cellValueBounds.Width >= DATAGRIDVIEWCELL_iconMarginWidth*2+iconsWidth &&
                cellValueBounds.Height >= DATAGRIDVIEWCELL_iconMarginHeight * 2 + iconsHeight)
            {
                PaintErrorIcon(graphics, ComputeErrorIconBounds(cellValueBounds));
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2002:DoNotLockOnObjectsWithWeakIdentity")]
        private static void PaintErrorIcon(Graphics graphics, Rectangle iconBounds)
        {
            Bitmap bmp = DataGridViewCell.ErrorBitmap;
            if (bmp != null)
            {
                lock (bmp)
                {
                    graphics.DrawImage(bmp, iconBounds, 0, 0, iconsWidth, iconsHeight, GraphicsUnit.Pixel);
                }
            }
        }

        internal void PaintErrorIcon(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Rectangle cellBounds, Rectangle cellValueBounds, string errorText)
        {
            if (!string.IsNullOrEmpty(errorText) &&
                cellValueBounds.Width >= DATAGRIDVIEWCELL_iconMarginWidth * 2 + iconsWidth &&
                cellValueBounds.Height >= DATAGRIDVIEWCELL_iconMarginHeight * 2 + iconsHeight)
            {
                Rectangle iconBounds = GetErrorIconBounds(graphics, cellStyle, rowIndex);
                if (iconBounds.Width >= DATAGRIDVIEWCELL_iconMarginWidth && iconBounds.Height >= iconsHeight)
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

        static internal void PaintPadding(Graphics graphics, 
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
            Debug.Assert(this.DataGridView != null);
            DataGridView dataGridView = this.DataGridView;
            int columnIndex = this.ColumnIndex;
            object formattedValue, value = GetValue(rowIndex);
            string errorText = GetErrorText(rowIndex);
            if (columnIndex > -1 && rowIndex > -1)
            {
                formattedValue = GetEditedFormattedValue(value, rowIndex, ref cellStyle, DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.Display);
            }
            else
            {
                // No formatting applied on header cells.
                formattedValue = value;
            }

            DataGridViewCellPaintingEventArgs dgvcpe = dataGridView.CellPaintingEventArgs;
            dgvcpe.SetProperties(graphics, 
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

            Paint(graphics, 
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

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.ParseFormattedValue"]/*' />
        public virtual object ParseFormattedValue(object formattedValue, 
                                                  DataGridViewCellStyle cellStyle,
                                                  TypeConverter formattedValueTypeConverter,
                                                  TypeConverter valueTypeConverter)
        {
            return ParseFormattedValueInternal(this.ValueType, formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
        }

        internal object ParseFormattedValueInternal(Type valueType,
                                                    object formattedValue,
                                                    DataGridViewCellStyle cellStyle,
                                                    TypeConverter formattedValueTypeConverter,
                                                    TypeConverter valueTypeConverter)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }
            if (this.FormattedValueType == null)
            {
                throw new FormatException(string.Format(SR.DataGridViewCell_FormattedValueTypeNull));
            }
            if (valueType == null)
            {
                throw new FormatException(string.Format(SR.DataGridViewCell_ValueTypeNull));
            }
            if (formattedValue == null ||
                !this.FormattedValueType.IsAssignableFrom(formattedValue.GetType()))
            {
                throw new ArgumentException(string.Format(SR.DataGridViewCell_FormattedValueHasWrongType), "formattedValue");
            }
            return Formatter.ParseObject(formattedValue,
                                         valueType,
                                         this.FormattedValueType,
                                         valueTypeConverter == null ? this.ValueTypeConverter : valueTypeConverter /*sourceConverter*/,
                                         formattedValueTypeConverter == null ? this.FormattedValueTypeConverter : formattedValueTypeConverter /*targetConverter*/,
                                         cellStyle.FormatProvider, 
                                         cellStyle.NullValue,
                                         cellStyle.IsDataSourceNullValueDefault ? Formatter.GetDefaultDataSourceNullValue(valueType) : cellStyle.DataSourceNullValue);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.PositionEditingControl"]/*' />
        [
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
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
            Rectangle editingControlBounds = PositionEditingPanel(cellBounds, 
                                                                  cellClip, 
                                                                  cellStyle, 
                                                                  singleVerticalBorderAdded, 
                                                                  singleHorizontalBorderAdded, 
                                                                  isFirstDisplayedColumn, 
                                                                  isFirstDisplayedRow);
            if (setLocation)
            {
                this.DataGridView.EditingControl.Location = new Point(editingControlBounds.X, editingControlBounds.Y);
            }
            if (setSize)
            {
                this.DataGridView.EditingControl.Size = new Size(editingControlBounds.Width, editingControlBounds.Height);
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.PositionEditingPanel"]/*' />
        [
            EditorBrowsable(EditorBrowsableState.Advanced),
            SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters") // singleVerticalBorderAdded/singleHorizontalBorderAdded names are OK
        ]
        // Positions the editing panel and returns the normal bounds of the editing control, within the editing panel.
        public virtual Rectangle PositionEditingPanel(Rectangle cellBounds, 
                                                      Rectangle cellClip, 
                                                      DataGridViewCellStyle cellStyle, 
                                                      bool singleVerticalBorderAdded, 
                                                      bool singleHorizontalBorderAdded, 
                                                      bool isFirstDisplayedColumn, 
                                                      bool isFirstDisplayedRow)
        {
            if (this.DataGridView == null)
            {
                throw new InvalidOperationException();
            }

            DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder = new DataGridViewAdvancedBorderStyle(), dgvabsEffective;

            dgvabsEffective = AdjustCellBorderStyle(this.DataGridView.AdvancedCellBorderStyle,
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
            this.DataGridView.EditingPanel.Location = new Point(xEditingPanel, yEditingPanel);
            this.DataGridView.EditingPanel.Size = new Size(wEditingPanel, hEditingPanel);
            /* 
            if (this.DataGridView.RightToLeftInternal)
            {
                xEditingControl = wEditingPanel - xEditingControl - wEditingControl;
            }
            */
            return new Rectangle(xEditingControl, yEditingControl, wEditingControl, hEditingControl);
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.SetValue"]/*' />
        protected virtual bool SetValue(int rowIndex, object value)
        {
            object originalValue = null;
            DataGridView dataGridView = this.DataGridView;
            if (dataGridView != null && !dataGridView.InSortOperation)
            {
                originalValue = GetValue(rowIndex);
            }

            if (dataGridView != null && this.OwningColumn != null && this.OwningColumn.IsDataBound)
            {
                DataGridView.DataGridViewDataConnection dataConnection = dataGridView.DataConnection;
                if (dataConnection == null)
                {
                    return false;
                }
                else if (dataConnection.CurrencyManager.Count <= rowIndex)
                {
                    if (value != null || this.Properties.ContainsObject(PropCellValue))
                    {
                        this.Properties.SetObject(PropCellValue, value);
                    }
                }
                else
                {
                    if (dataConnection.PushValue(this.OwningColumn.BoundColumnIndex, this.ColumnIndex, rowIndex, value))
                    {
                        if (this.DataGridView == null || this.OwningRow == null || this.OwningRow.DataGridView == null)
                        {
                            // As a result of pushing the value in the back end, the data grid view row and/or data grid view cell
                            // became disconnected from the DataGridView.
                            // Return true because the operation succeded.
                            // However, because the row which was edited became disconnected  from the DataGridView, 
                            // do not mark the current row in the data grid view as being dirty.
                            // And because the data grid view cell which was edited became disconnected from the data grid view
                            // do not fire CellValueChanged event.
                            return true;
                        }

                        if (this.OwningRow.Index == this.DataGridView.CurrentCellAddress.Y)
                        {
                            // The user programatically changed a value in the current row.
                            // The DataGridView already opened a transaction for the current row.
                            // All is left to do is to mark the current row in the DataGridView as being dirty.
                            this.DataGridView.IsCurrentRowDirtyInternal = true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (dataGridView == null ||
                !dataGridView.VirtualMode ||
                rowIndex == -1 ||
                this.ColumnIndex == -1)
            {
                if (value != null || this.Properties.ContainsObject(PropCellValue))
                {
                    this.Properties.SetObject(PropCellValue, value);
                }
            }
            else
            {
                Debug.Assert(rowIndex >= 0);
                Debug.Assert(this.ColumnIndex >= 0);
                dataGridView.OnCellValuePushed(this.ColumnIndex, rowIndex, value);
            }

            if (dataGridView != null &&
                !dataGridView.InSortOperation &&
                ((originalValue == null && value != null) ||
                 (originalValue != null && value == null) ||
                 (originalValue != null && !value.Equals(originalValue))))
            {
                RaiseCellValueChanged(new DataGridViewCellEventArgs(this.ColumnIndex, rowIndex));
            }
            return true;
        }

        internal bool SetValueInternal(int rowIndex, object value)
        {
            return SetValue(rowIndex, value);
        }

        internal static bool TextFitsInBounds(Graphics graphics, string text, Font font, Size maxBounds, TextFormatFlags flags)
        {
            bool widthTruncated;
            int requiredHeight = DataGridViewCell.MeasureTextHeight(graphics, text, font, maxBounds.Width, flags, out widthTruncated);
            return requiredHeight <= maxBounds.Height && !widthTruncated;
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCell.ToString"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the row Index and column Index of the cell.
        ///    </para>
        /// </devdoc>
        public override string ToString()
        {
            return "DataGridViewCell { ColumnIndex=" + this.ColumnIndex.ToString(CultureInfo.CurrentCulture) + ", RowIndex=" + this.RowIndex.ToString(CultureInfo.CurrentCulture) + " }";
        }

        private static string TruncateToolTipText(string toolTipText)
        {
            if (toolTipText.Length > DATAGRIDVIEWCELL_maxToolTipLength)
            {
                StringBuilder sb = new StringBuilder(toolTipText.Substring(0, DATAGRIDVIEWCELL_maxToolTipCutOff), DATAGRIDVIEWCELL_maxToolTipCutOff + DATAGRIDVIEWCELL_toolTipEllipsisLength);
                sb.Append(DATAGRIDVIEWCELL_toolTipEllipsis);
                return sb.ToString();
            }
            return toolTipText;
        }

        private void UpdateCurrentMouseLocation(DataGridViewCellMouseEventArgs e)
        {
            if (GetErrorIconBounds(e.RowIndex).Contains(e.X, e.Y))
            {
                this.CurrentMouseLocation = DATAGRIDVIEWCELL_flagErrorArea;
            }
            else
            {
                this.CurrentMouseLocation = DATAGRIDVIEWCELL_flagDataArea;
            }
        }

        /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject"]/*' />
        [
            System.Runtime.InteropServices.ComVisible(true)
        ]
        protected class DataGridViewCellAccessibleObject : AccessibleObject
        {
            private int[] runtimeId = null; // Used by UIAutomation

            DataGridViewCell owner;

            /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject.DataGridViewCellAccessibleObject1"]/*' />
            public DataGridViewCellAccessibleObject()
            {
            }

            /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject.DataGridViewCellAccessibleObject2"]/*' />
            public DataGridViewCellAccessibleObject(DataGridViewCell owner)
            {
                this.owner = owner;
            }

            /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject.Bounds"]/*' />
            public override Rectangle Bounds
            {
                get
                {
                    return this.GetAccessibleObjectBounds(GetAccessibleObjectParent());
                }
            }

            /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject.DefaultAction"]/*' />
            public override string DefaultAction
            {
                get
                {
                    if (this.Owner == null)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewCellAccessibleObject_OwnerNotSet));
                    }
                    if (!this.Owner.ReadOnly)
                    {
                        return string.Format(SR.DataGridView_AccCellDefaultAction);
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
            }

            /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject.Help"]/*' />
            public override string Help
            {
                get
                {
                    if (AccessibilityImprovements.Level2)
                    {
                        return null;
                    }

                    return this.owner.GetType().Name + "(" + owner.GetType().BaseType.Name + ")";
                }
            }

            /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject.Name"]/*' />
            public override string Name
            {
                get
                {
                    if (this.owner == null)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewCellAccessibleObject_OwnerNotSet));
                    }
                    if (this.owner.OwningColumn != null)
                    {
                        string name = string.Format(SR.DataGridView_AccDataGridViewCellName, this.owner.OwningColumn.HeaderText, this.owner.OwningRow.Index);

                        if (AccessibilityImprovements.Level3 && owner.OwningColumn.SortMode != DataGridViewColumnSortMode.NotSortable)
                        {
                            DataGridViewCell dataGridViewCell = this.Owner;
                            DataGridView dataGridView = dataGridViewCell.DataGridView;

                            if (dataGridViewCell.OwningColumn != null &&
                                dataGridViewCell.OwningColumn == dataGridView.SortedColumn)
                            {
                                name += ", " + (dataGridView.SortOrder == SortOrder.Ascending
                                    ? SR.SortedAscendingAccessibleStatus
                                    : SR.SortedDescendingAccessibleStatus);
                            }
                            else
                            {
                                name += ", " + SR.NotSortedAccessibleStatus;
                            }
                        }

                        return name;
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
            }

            /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject.Owner"]/*' />
            public DataGridViewCell Owner
            {
                get
                {
                    return this.owner;
                }
                set
                {
                    if (this.owner != null)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewCellAccessibleObject_OwnerAlreadySet));
                    }
                    this.owner = value;
                }
            }

            /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject.Parent"]/*' />
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
                        throw new InvalidOperationException(string.Format(SR.DataGridViewCellAccessibleObject_OwnerNotSet));
                    }
                    if (this.owner.OwningRow == null)
                    {
                        return null;
                    }
                    else
                    {
                        return this.owner.OwningRow.AccessibilityObject;
                    }
                }
            }

            /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject.Role"]/*' />
            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.Cell;
                }
            }

            /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject.State"]/*' />
            public override AccessibleStates State
            {
                get
                {
                    if (this.owner == null)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewCellAccessibleObject_OwnerNotSet));
                    }

                    AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable;
                    if (this.owner == this.owner.DataGridView.CurrentCell)
                    {
                        state |= AccessibleStates.Focused;
                    }

                    if (this.owner.Selected)
                    {
                        state |= AccessibleStates.Selected;
                    }

                    if (AccessibilityImprovements.Level1 && this.owner.ReadOnly)
                    {
                        state |= AccessibleStates.ReadOnly;
                    }

                    Rectangle cellBounds;
                    if (this.owner.OwningColumn != null && this.owner.OwningRow != null)
                    {
                        cellBounds = this.owner.DataGridView.GetCellDisplayRectangle(this.owner.OwningColumn.Index, this.owner.OwningRow.Index, false /*cutOverflow*/);
                    }
                    else if (this.owner.OwningRow != null)
                    {
                        cellBounds = this.owner.DataGridView.GetCellDisplayRectangle(-1, this.owner.OwningRow.Index, false /*cutOverflow*/);
                    }
                    else if (this.owner.OwningColumn != null)
                    {
                        cellBounds = this.owner.DataGridView.GetCellDisplayRectangle(this.owner.OwningColumn.Index, -1, false /*cutOverflow*/);
                    }
                    else
                    {
                        cellBounds = this.owner.DataGridView.GetCellDisplayRectangle(-1, -1, false /*cutOverflow*/);
                    }

                    if (!cellBounds.IntersectsWith(this.owner.DataGridView.ClientRectangle))
                    {
                        state |= AccessibleStates.Offscreen;
                    }

                    return state;
                }
            }

            /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject.Value"]/*' />
            public override string Value
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    if (this.owner == null)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewCellAccessibleObject_OwnerNotSet));
                    }

                    object formattedValue = this.owner.FormattedValue;
                    string formattedValueAsString = formattedValue as string;
                    if (formattedValue == null || (formattedValueAsString  != null && String.IsNullOrEmpty(formattedValueAsString)))
                    {
                        return string.Format(SR.DataGridView_AccNullValue);
                    }
                    else if (formattedValueAsString != null)
                    {
                        return formattedValueAsString;
                    }
                    else if (this.owner.OwningColumn != null)
                    {
                        TypeConverter converter = this.owner.FormattedValueTypeConverter;
                        if (converter != null && converter.CanConvertTo(typeof(string)))
                        {
                            return converter.ConvertToString(formattedValue);
                        }
                        else
                        {
                            return formattedValue.ToString();
                        }
                    }
                    else
                    {
                        return String.Empty;
                    }
                }

                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                set
                {
                    if (this.owner is DataGridViewHeaderCell)
                    {
                        return;
                    }

                    if (this.owner.ReadOnly)
                    {
                        return;
                    }

                    if (this.owner.OwningRow == null)
                    {
                        return;
                    }

                    if (this.owner.DataGridView.IsCurrentCellInEditMode)
                    {
                        // EndEdit before setting the accessible object value.
                        // This way the value being edited is validated.
                        this.owner.DataGridView.EndEdit();
                    }

                    DataGridViewCellStyle dataGridViewCellStyle = this.owner.InheritedStyle;

                    // Format string "True" to boolean True.
                    object formattedValue = this.owner.GetFormattedValue(value,
                                                                         this.owner.OwningRow.Index,
                                                                         ref dataGridViewCellStyle,
                                                                         null /*formattedValueTypeConverter*/ ,
                                                                         null /*valueTypeConverter*/,
                                                                         DataGridViewDataErrorContexts.Formatting);
                    // Parse the formatted value and push it into the back end.
                    this.owner.Value = owner.ParseFormattedValue(formattedValue,
                                                                 dataGridViewCellStyle,
                                                                 null /*formattedValueTypeConverter*/,
                                                                 null /*valueTypeConverter*/);
                }
            }

            /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject.DoDefaultAction"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void DoDefaultAction()
            {
                if (this.owner == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewCellAccessibleObject_OwnerNotSet));
                }

                DataGridViewCell dataGridViewCell = (DataGridViewCell)this.Owner;
                DataGridView dataGridView = dataGridViewCell.DataGridView;

                if (dataGridViewCell is DataGridViewHeaderCell)
                {
                    return;
                }

                if (dataGridView != null && dataGridViewCell.RowIndex == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidOperationOnSharedCell));
                }

                Select(AccessibleSelection.TakeFocus | AccessibleSelection.TakeSelection);
                Debug.Assert(dataGridView.CurrentCell == dataGridViewCell, "the result of selecting the cell should have made this cell the current cell");

                if (dataGridViewCell.ReadOnly)
                {
                    // don't edit if the cell is read only
                    return;
                }

                if (dataGridViewCell.EditType != null)
                {
                    if (dataGridView.InBeginEdit || dataGridView.InEndEdit)
                    {
                        // don't enter or exit editing mode if the control
                        // is in the middle of doing that already.
                        return;
                    }
                    if (dataGridView.IsCurrentCellInEditMode)
                    {
                        // stop editing
                        dataGridView.EndEdit();
                    }
                    else if (dataGridView.EditMode != DataGridViewEditMode.EditProgrammatically)
                    {
                        // start editing
                        dataGridView.BeginEdit(true /*selectAll*/);
                    }
                }
            }

            internal Rectangle GetAccessibleObjectBounds(AccessibleObject parentAccObject)
            {
                if (this.owner == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewCellAccessibleObject_OwnerNotSet));
                }

                if (this.owner.OwningColumn == null)
                {
                    return Rectangle.Empty;
                }

                // use the accessibility bounds from the parent row acc obj
                Rectangle rowRect = parentAccObject.Bounds;
                Rectangle columnRect;

                int firstVisibleColumnIndex = this.owner.DataGridView.Columns.ColumnIndexToActualDisplayIndex(this.owner.DataGridView.FirstDisplayedScrollingColumnIndex, DataGridViewElementStates.Visible);
                int visibleColumnIndex = this.owner.DataGridView.Columns.ColumnIndexToActualDisplayIndex(this.owner.ColumnIndex, DataGridViewElementStates.Visible);

                bool rowHeadersVisible = this.owner.DataGridView.RowHeadersVisible;
                if (visibleColumnIndex < firstVisibleColumnIndex)
                {
                    // Get the bounds for the cell to the RIGHT
                    columnRect = parentAccObject.GetChild(visibleColumnIndex
                                                          + 1                                       // + 1 for the next cell to the RIGHT
                                                          + (rowHeadersVisible ? 1 : 0)).Bounds;    // + 1 but only if the row headers are visible

                    // From the bounds of the cell to the RIGHT decrement the width of the owning column
                    if (this.Owner.DataGridView.RightToLeft == RightToLeft.No)
                    {
                        columnRect.X -= this.owner.OwningColumn.Width;
                    }
                    else
                    {
                        columnRect.X = columnRect.Right;
                    }
                    columnRect.Width = this.owner.OwningColumn.Width;
                }
                else if (visibleColumnIndex == firstVisibleColumnIndex)
                {
                    columnRect = this.owner.DataGridView.GetColumnDisplayRectangle(this.owner.ColumnIndex, false /*cutOverflow*/);
                    int negOffset = this.owner.DataGridView.FirstDisplayedScrollingColumnHiddenWidth;

                    if (negOffset != 0)
                    {
                        if (this.owner.DataGridView.RightToLeft == RightToLeft.No)
                        {
                            columnRect.X -= negOffset;
                        }
                        columnRect.Width += negOffset;
                    }
                    columnRect = this.owner.DataGridView.RectangleToScreen(columnRect);
                }
                else
                {
                    // Get the bounds for the cell to the LEFT
                    columnRect = parentAccObject.GetChild(visibleColumnIndex
                                                          - 1                                       // -1 because we want the previous cell to the LEFT
                                                          + (rowHeadersVisible ? 1 : 0)).Bounds;    // +1 but only if the row headers are visible

                    // From the bounds of the cell to the LEFT increment the width of the owning column
                    if (this.owner.DataGridView.RightToLeft == RightToLeft.No)
                    {
                        columnRect.X = columnRect.Right;
                    }
                    else
                    {
                        columnRect.X -= this.owner.OwningColumn.Width;
                    }

                    columnRect.Width = this.owner.OwningColumn.Width;
                }

                rowRect.X = columnRect.X;
                rowRect.Width = columnRect.Width;

                return rowRect;
            }

            private AccessibleObject GetAccessibleObjectParent()
            {
                // If this is one of our types, use the shortcut provided by ParentPrivate property.
                // Otherwise, use the Parent property.
                if (this.owner is DataGridViewButtonCell ||
                    this.owner is DataGridViewCheckBoxCell ||
                    this.owner is DataGridViewComboBoxCell ||
                    this.owner is DataGridViewImageCell ||
                    this.owner is DataGridViewLinkCell ||
                    this.owner is DataGridViewTextBoxCell)
                {
                    return this.ParentPrivate;
                }
                else
                {
                    return this.Parent;
                }
            }

            /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject.GetChild"]/*' />
            public override AccessibleObject GetChild(int index)
            {
                if (this.owner == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewCellAccessibleObject_OwnerNotSet));
                }

                if (this.owner.DataGridView.EditingControl != null &&
                    this.owner.DataGridView.IsCurrentCellInEditMode &&
                    this.owner.DataGridView.CurrentCell == this.owner &&
                    index == 0)
                {
                    return this.owner.DataGridView.EditingControl.AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject.GetChildCount"]/*' />
            public override int GetChildCount()
            {
                if (this.owner == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewCellAccessibleObject_OwnerNotSet));
                }

                if (this.owner.DataGridView.EditingControl != null &&
                    this.owner.DataGridView.IsCurrentCellInEditMode &&
                    this.owner.DataGridView.CurrentCell == this.owner)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject.GetFocused"]/*' />
            public override AccessibleObject GetFocused()
            {
                return null;
            }

            /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject.GetSelected"]/*' />
            public override AccessibleObject GetSelected()
            {
                return null;
            }

            /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject.Navigate"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                if (this.owner == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewCellAccessibleObject_OwnerNotSet));
                }

                if (this.owner.OwningColumn == null || this.owner.OwningRow == null)
                {
                    return null;
                }

                switch (navigationDirection)
                {
                    case AccessibleNavigation.Right:
                        if (this.owner.DataGridView.RightToLeft == RightToLeft.No)
                        {
                            return NavigateForward(true /*wrapAround*/);
                        }
                        else
                        {
                            return NavigateBackward(true /*wrapAround*/);
                        }
                    case AccessibleNavigation.Next:
                        return NavigateForward(false /*wrapAround*/);
                    case AccessibleNavigation.Left:
                        if (this.owner.DataGridView.RightToLeft == RightToLeft.No)
                        {
                            return NavigateBackward(true /*wrapAround*/);
                        }
                        else
                        {
                            return NavigateForward(true /*wrapAround*/);
                        }
                    case AccessibleNavigation.Previous:
                        return NavigateBackward(false /*wrapAround*/);
                    case AccessibleNavigation.Up:
                        if (this.owner.OwningRow.Index == this.owner.DataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible))
                        {
                            if (this.owner.DataGridView.ColumnHeadersVisible)
                            {
                                // Return the column header accessible object.
                                return this.owner.OwningColumn.HeaderCell.AccessibilityObject;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            int previousVisibleRow = this.owner.DataGridView.Rows.GetPreviousRow(this.owner.OwningRow.Index, DataGridViewElementStates.Visible);
                            return this.owner.DataGridView.Rows[previousVisibleRow].Cells[this.owner.OwningColumn.Index].AccessibilityObject;
                        }
                    case AccessibleNavigation.Down:
                        if (this.owner.OwningRow.Index == this.owner.DataGridView.Rows.GetLastRow(DataGridViewElementStates.Visible))
                        {
                            return null;
                        }
                        else
                        {
                            int nextVisibleRow = this.owner.DataGridView.Rows.GetNextRow(this.owner.OwningRow.Index, DataGridViewElementStates.Visible);
                            return this.owner.DataGridView.Rows[nextVisibleRow].Cells[this.owner.OwningColumn.Index].AccessibilityObject;
                        }
                    default:
                        return null;
                }
            }

            private AccessibleObject NavigateBackward(bool wrapAround)
            {
                if (this.owner.OwningColumn == this.owner.DataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible))
                {
                    if (wrapAround)
                    {
                        // Return the last accessible object in the previous row
                        AccessibleObject previousRow = this.Owner.OwningRow.AccessibilityObject.Navigate(AccessibleNavigation.Previous);
                        if (previousRow != null && previousRow.GetChildCount() > 0)
                        {
                            return previousRow.GetChild(previousRow.GetChildCount() - 1);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        // return the row header cell if the row headers are visible.
                        if (this.owner.DataGridView.RowHeadersVisible)
                        {
                            return this.owner.OwningRow.AccessibilityObject.GetChild(0);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    int previousVisibleColumnIndex = this.owner.DataGridView.Columns.GetPreviousColumn(this.owner.OwningColumn,
                                                                                                       DataGridViewElementStates.Visible,
                                                                                                       DataGridViewElementStates.None).Index;
                    return this.owner.OwningRow.Cells[previousVisibleColumnIndex].AccessibilityObject;
                }
            }

            private AccessibleObject NavigateForward(bool wrapAround)
            {
                if (this.owner.OwningColumn == this.owner.DataGridView.Columns.GetLastColumn(DataGridViewElementStates.Visible,
                                                                                             DataGridViewElementStates.None))
                {

                    if (wrapAround)
                    {
                        // Return the first cell in the next visible row.
                        //
                        AccessibleObject nextRow = this.Owner.OwningRow.AccessibilityObject.Navigate(AccessibleNavigation.Next);
                        if (nextRow != null && nextRow.GetChildCount() > 0)
                        {
                            if (this.Owner.DataGridView.RowHeadersVisible)
                            {
                                return nextRow.GetChild(1);
                            }
                            else
                            {
                                return nextRow.GetChild(0);
                            }
                        }
                        else
                        {
                            return null;
                        }

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    int nextVisibleColumnIndex = this.owner.DataGridView.Columns.GetNextColumn(this.owner.OwningColumn,
                                                                                               DataGridViewElementStates.Visible,
                                                                                               DataGridViewElementStates.None).Index;
                    return this.owner.OwningRow.Cells[nextVisibleColumnIndex].AccessibilityObject;
                }
            }

            /// <include file='doc\DataGridViewCell.uex' path='docs/doc[@for="DataGridViewCellAccessibleObject.Select"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void Select(AccessibleSelection flags)
            {
                if (this.owner == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewCellAccessibleObject_OwnerNotSet));
                }
                if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
                {
                    this.owner.DataGridView.FocusInternal();
                }
                if ((flags & AccessibleSelection.TakeSelection) == AccessibleSelection.TakeSelection)
                {
                    this.owner.Selected = true;
                    this.owner.DataGridView.CurrentCell = this.owner; // Do not change old selection
                }
                if ((flags & AccessibleSelection.AddSelection) == AccessibleSelection.AddSelection)
                {
                    // it seems that in any circumstances a cell can become selected
                    this.owner.Selected = true;
                }
                if ((flags & AccessibleSelection.RemoveSelection) == AccessibleSelection.RemoveSelection &&
                    (flags & (AccessibleSelection.AddSelection | AccessibleSelection.TakeSelection)) == 0)
                {
                    this.owner.Selected = false;
                }
            }

            internal override int[] RuntimeId
            {
                get
                {
                    if (runtimeId == null)
                    {
                        runtimeId = new int[2];
                        runtimeId[0] = RuntimeIDFirstItem; // first item is static - 0x2a
                        runtimeId[1] = this.GetHashCode();
                    }

                    return runtimeId;
                }
            }

            internal override bool IsIAccessibleExSupported()
            {
                if (AccessibilityImprovements.Level2)
                {
                    return true;
                }

                return base.IsIAccessibleExSupported();
            }

            #region IRawElementProviderFragment Implementation

            internal override Rectangle BoundingRectangle
            {
                get
                {
                    return this.Bounds;
                }
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return owner.DataGridView.AccessibilityObject;
                }
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
            {
                if (this.owner == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewCellAccessibleObject_OwnerNotSet));
                }

                if (this.owner.OwningColumn == null || this.owner.OwningRow == null)
                {
                    return null;
                }

                switch (direction)
                {
                    case UnsafeNativeMethods.NavigateDirection.Parent:
                        return this.owner.OwningRow.AccessibilityObject;
                    case UnsafeNativeMethods.NavigateDirection.NextSibling:
                        return NavigateForward(false);
                    case UnsafeNativeMethods.NavigateDirection.PreviousSibling:
                        return NavigateBackward(false);
                    case UnsafeNativeMethods.NavigateDirection.FirstChild:
                    case UnsafeNativeMethods.NavigateDirection.LastChild:
                        if (this.owner.DataGridView.CurrentCell == this.owner &&
                            this.owner.DataGridView.EditingControl != null)
                        {
                            return this.owner.DataGridView.EditingPanelAccessibleObject;
                        }
                        break;
                    default:
                        return null;
                }

                return null;
            }

            #endregion

            #region IRawElementProviderSimple Implementation

            internal override object GetPropertyValue(int propertyID)
            {
                if (AccessibilityImprovements.Level3)
                {
                    switch (propertyID)
                    {
                        case NativeMethods.UIA_NamePropertyId:
                            return this.Name;
                        case NativeMethods.UIA_HasKeyboardFocusPropertyId:
                            return owner.Selected;
                        case NativeMethods.UIA_IsEnabledPropertyId:
                            return owner.DataGridView.Enabled;
                        case NativeMethods.UIA_HelpTextPropertyId:
                            return this.Help ?? string.Empty;
                        case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                            return (this.State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                        case NativeMethods.UIA_IsPasswordPropertyId:
                            return false;
                        case NativeMethods.UIA_IsOffscreenPropertyId:
                            return (this.State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
                        case NativeMethods.UIA_AccessKeyPropertyId:
                            return string.Empty;
                        case NativeMethods.UIA_GridItemContainingGridPropertyId:
                            return this.Owner.DataGridView.AccessibilityObject;
                    }
                }

                if (propertyID == NativeMethods.UIA_IsTableItemPatternAvailablePropertyId)
                {
                    return IsPatternSupported(NativeMethods.UIA_TableItemPatternId);
                }
                else if (propertyID == NativeMethods.UIA_IsGridItemPatternAvailablePropertyId)
                {
                    return IsPatternSupported(NativeMethods.UIA_GridItemPatternId);
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override bool IsPatternSupported(int patternId)
            {
                if (AccessibilityImprovements.Level3 &&
                    (patternId.Equals(NativeMethods.UIA_LegacyIAccessiblePatternId) ||
                    patternId.Equals(NativeMethods.UIA_InvokePatternId)))
                {
                    return true;
                }

                if ((patternId == NativeMethods.UIA_TableItemPatternId ||
                    patternId == NativeMethods.UIA_GridItemPatternId) && 
                    // We don't want to implement patterns for header cells
                    this.owner.ColumnIndex != -1 && this.owner.RowIndex != -1)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            #endregion

            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetRowHeaderItems()
            {
                if (this.owner.DataGridView.RowHeadersVisible && this.owner.OwningRow.HasHeaderCell)
                {
                    return new UnsafeNativeMethods.IRawElementProviderSimple[1] { this.owner.OwningRow.HeaderCell.AccessibilityObject };
                }

                return null;
            }

            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetColumnHeaderItems()
            {
                if (this.owner.DataGridView.ColumnHeadersVisible && this.owner.OwningColumn.HasHeaderCell)
                {
                    return new UnsafeNativeMethods.IRawElementProviderSimple[1] { this.owner.OwningColumn.HeaderCell.AccessibilityObject };
                }

                return null;
            }

            internal override int Row
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    return this.owner.OwningRow != null ? this.owner.OwningRow.Index : -1;
                }
            }

            internal override int Column
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    return this.owner.OwningColumn != null ? this.owner.OwningColumn.Index : -1;
                }
            }

            internal override UnsafeNativeMethods.IRawElementProviderSimple ContainingGrid
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    return this.owner.DataGridView.AccessibilityObject;
                }
            }
        }
    }
}

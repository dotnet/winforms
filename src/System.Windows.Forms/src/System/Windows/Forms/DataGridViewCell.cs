// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Identifies a cell in the dataGridView.
    /// </summary>
    [TypeConverter(typeof(DataGridViewCellConverter))]
    public abstract class DataGridViewCell : DataGridViewElement, ICloneable, IDisposable, IKeyboardToolTip
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

        private readonly PropertyStore propertyStore;          // Contains all properties that are not always set.

        /// <summary>
        /// Contains non-empty neighboring cells around the current cell. 
        /// Used in <see cref='IKeyboardToolTip.GetNeighboringToolsRectangles'/> method.
        /// </summary>
        private readonly List<Rectangle> _nonEmptyNeighbors;

        private static readonly Type stringType = typeof(string);        // cache the string type for performance

        private byte flags;  // see DATAGRIDVIEWCELL_flag* consts above
        
        private bool _useDefaultToolTipText;  //  The tooltip text of this cell has not been set by a customer yet.

        /// <summary>
        ///  Initializes a new instance of the <see cref='DataGridViewCell'/> class.
        /// </summary>
        protected DataGridViewCell() : base()
        {
            if (!isScalingInitialized)
            {
                if (DpiHelper.IsScalingRequired)
                {
                    iconsWidth = (byte)DpiHelper.LogicalToDeviceUnitsX(DATAGRIDVIEWCELL_iconsWidth);
                    iconsHeight = (byte)DpiHelper.LogicalToDeviceUnitsY(DATAGRIDVIEWCELL_iconsHeight);
                }
                isScalingInitialized = true;
            }

            propertyStore = new PropertyStore();
            State = DataGridViewElementStates.None;
            _nonEmptyNeighbors = new List<Rectangle>();
            _useDefaultToolTipText = true;
        }

        ~DataGridViewCell()
        {
            Dispose(false);
        }

        [
            Browsable(false)
        ]
        public AccessibleObject AccessibilityObject
        {
            get
            {
                AccessibleObject result = (AccessibleObject)Properties.GetObject(PropCellAccessibilityObject);
                if (result == null)
                {
                    result = CreateAccessibilityInstance();
                    Properties.SetObject(PropCellAccessibilityObject, result);
                }

                return result;
            }
        }

        /// <summary>
        ///  Gets or sets the Index of a column in the <see cref='DataGrid'/> control.
        /// </summary>
        public int ColumnIndex => OwningColumn?.Index ?? -1;

        [
            Browsable(false)
        ]
        public Rectangle ContentBounds
        {
            get
            {
                return GetContentBounds(RowIndex);
            }
        }

        [
            DefaultValue(null)
        ]
        public virtual ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return GetContextMenuStrip(RowIndex);
            }
            set
            {
                ContextMenuStripInternal = value;
            }
        }

        private ContextMenuStrip ContextMenuStripInternal
        {
            get
            {
                return (ContextMenuStrip)Properties.GetObject(PropCellContextMenuStrip);
            }
            set
            {
                ContextMenuStrip oldValue = (ContextMenuStrip)Properties.GetObject(PropCellContextMenuStrip);
                if (oldValue != value)
                {
                    EventHandler disposedHandler = new EventHandler(DetachContextMenuStrip);
                    if (oldValue != null)
                    {
                        oldValue.Disposed -= disposedHandler;
                    }
                    Properties.SetObject(PropCellContextMenuStrip, value);
                    if (value != null)
                    {
                        value.Disposed += disposedHandler;
                    }
                    if (DataGridView != null)
                    {
                        DataGridView.OnCellContextMenuStripChanged(this);
                    }
                }
            }
        }

        private byte CurrentMouseLocation
        {
            get
            {
                return (byte)(flags & (DATAGRIDVIEWCELL_flagDataArea | DATAGRIDVIEWCELL_flagErrorArea));
            }
            set
            {
                flags = (byte)(flags & ~(DATAGRIDVIEWCELL_flagDataArea | DATAGRIDVIEWCELL_flagErrorArea));
                flags |= value;
            }
        }

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

        [
            Browsable(false)
        ]
        public virtual bool Displayed
        {
            get
            {
                Debug.Assert((State & DataGridViewElementStates.Displayed) == 0);

                if (DataGridView == null)
                {
                    // No detached element is displayed.
                    return false;
                }

                if (RowIndex >= 0 && ColumnIndex >= 0)
                {
                    Debug.Assert(DataGridView.Rows.GetRowState(RowIndex) == DataGridView.Rows.SharedRow(RowIndex).State);
                    return OwningColumn.Displayed && OwningRow.Displayed;
                }

                return false;
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public object EditedFormattedValue
        {
            get
            {
                if (DataGridView == null)
                {
                    return null;
                }
                Debug.Assert(RowIndex >= -1);
                DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, RowIndex, false);
                return GetEditedFormattedValue(GetValue(RowIndex), RowIndex, ref dataGridViewCellStyle, DataGridViewDataErrorContexts.Formatting);
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public virtual Type EditType
        {
            get
            {
                return typeof(DataGridViewTextBoxEditingControl);
            }
        }

        private static Bitmap ErrorBitmap
        {
            get
            {
                if (errorBmp == null)
                {
                    errorBmp = GetBitmap("DataGridViewRow.error");
                }
                return errorBmp;
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced),
        ]
        public Rectangle ErrorIconBounds
        {
            get
            {
                return GetErrorIconBounds(RowIndex);
            }
        }

        [
            Browsable(false)
        ]
        public string ErrorText
        {
            get
            {
                return GetErrorText(RowIndex);
            }
            set
            {
                ErrorTextInternal = value;
            }
        }

        private string ErrorTextInternal
        {
            get
            {
                object errorText = Properties.GetObject(PropCellErrorText);
                return (errorText == null) ? string.Empty : (string)errorText;
            }
            set
            {
                string errorText = ErrorTextInternal;
                if (!string.IsNullOrEmpty(value) || Properties.ContainsObject(PropCellErrorText))
                {
                    Properties.SetObject(PropCellErrorText, value);
                }
                if (DataGridView != null && !errorText.Equals(ErrorTextInternal))
                {
                    DataGridView.OnCellErrorTextChanged(this);
                }
            }
        }

        [
            Browsable(false)
        ]
        public object FormattedValue
        {
            get
            {
                if (DataGridView == null)
                {
                    return null;
                }
                Debug.Assert(RowIndex >= -1);
                DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, RowIndex, false);
                return GetFormattedValue(RowIndex, ref dataGridViewCellStyle, DataGridViewDataErrorContexts.Formatting);
            }
        }

        [
            Browsable(false)
        ]
        public virtual Type FormattedValueType
        {
            get
            {
                return ValueType;
            }
        }

        private TypeConverter FormattedValueTypeConverter
        {
            get
            {
                TypeConverter formattedValueTypeConverter = null;
                if (FormattedValueType != null)
                {
                    if (DataGridView != null)
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

        [
            Browsable(false)
        ]
        public virtual bool Frozen
        {
            get
            {
                Debug.Assert((State & DataGridViewElementStates.Frozen) == 0);

                if (DataGridView != null && RowIndex >= 0 && ColumnIndex >= 0)
                {
                    Debug.Assert(DataGridView.Rows.GetRowState(RowIndex) == DataGridView.Rows.SharedRow(RowIndex).State);
                    return OwningColumn.Frozen && OwningRow.Frozen;
                }
                else if (OwningRow != null && (OwningRow.DataGridView == null || RowIndex >= 0))
                {
                    return OwningRow.Frozen;
                }
                return false;
            }
        }

        private bool HasErrorText
        {
            get => Properties.ContainsObject(PropCellErrorText) && Properties.GetObject(PropCellErrorText) != null;
        }

        [Browsable(false)]
        public bool HasStyle
        {
            get
            {
                return Properties.ContainsObject(PropCellStyle) && Properties.GetObject(PropCellStyle) != null;
            }
        }

        internal bool HasToolTipText
        {
            get
            {
                return Properties.ContainsObject(PropCellToolTipText) && Properties.GetObject(PropCellToolTipText) != null;
            }
        }

        internal bool HasValue
        {
            get
            {
                return Properties.ContainsObject(PropCellValue) && Properties.GetObject(PropCellValue) != null;
            }
        }

        private protected virtual bool HasValueType
        {
            get => Properties.ContainsObject(PropCellValueType) && Properties.GetObject(PropCellValueType) != null;
        }

        #region IKeyboardToolTip implementation
        bool IKeyboardToolTip.CanShowToolTipsNow() => Visible && DataGridView != null;

        Rectangle IKeyboardToolTip.GetNativeScreenRectangle() => AccessibilityObject.Bounds;

        /// <summary>
        ///  The method looks for 8 cells around the current cell 
        ///  to find the optimal tooltip position in <see cref='ToolTip.GetOptimalToolTipPosition'/> method.
        ///  The optimal tooltip position is the position outside DataGridView or on top of an empty cell. 
        ///  This is done so that tooltips do not overlap the text of other cells whenever possible.
        /// </summary>
        /// <returns>
        ///  Non-empty neighboring cells around the current cell.
        /// </returns>
        IList<Rectangle> IKeyboardToolTip.GetNeighboringToolsRectangles()
        {
            _nonEmptyNeighbors.Clear();

            if (DataGridView == null)
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

        bool IKeyboardToolTip.HasRtlModeEnabled() => DataGridView.RightToLeft == RightToLeft.Yes;

        bool IKeyboardToolTip.AllowsToolTip() => true;

        IWin32Window IKeyboardToolTip.GetOwnerWindow() => DataGridView;

        void IKeyboardToolTip.OnHooked(ToolTip toolTip) => OnKeyboardToolTipHook(toolTip);

        internal virtual void OnKeyboardToolTipHook(ToolTip toolTip) { }

        void IKeyboardToolTip.OnUnhooked(ToolTip toolTip) => OnKeyboardToolTipUnhook(toolTip);

        internal virtual void OnKeyboardToolTipUnhook(ToolTip toolTip) { }

        string IKeyboardToolTip.GetCaptionForTool(ToolTip toolTip)
        {
            if (DataGridView.ShowCellErrors && !string.IsNullOrEmpty(ErrorText))
            {
                return ErrorText;
            }

            return ToolTipText;
        }

        bool IKeyboardToolTip.ShowsOwnToolTip() => true;

        bool IKeyboardToolTip.IsBeingTabbedTo() => IsBeingTabbedTo();

        internal virtual bool IsBeingTabbedTo() => DataGridView.AreCommonNavigationalKeysDown();

        bool IKeyboardToolTip.AllowsChildrenToShowToolTips() => true;
        #endregion

        [Browsable(false)]
        public DataGridViewElementStates InheritedState
        {
            get
            {
                return GetInheritedState(RowIndex);
            }
        }

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

        [
            Browsable(false)
        ]
        public bool IsInEditMode
        {
            get
            {
                if (DataGridView == null)
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
        public DataGridViewColumn OwningColumn { get; internal set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public DataGridViewRow OwningRow { get; internal set; }

        [
            Browsable(false)
        ]
        public Size PreferredSize
        {
            get
            {
                return GetPreferredSize(RowIndex);
            }
        }

        internal PropertyStore Properties
        {
            get
            {
                return propertyStore;
            }
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual bool ReadOnly
        {
            get
            {
                if ((State & DataGridViewElementStates.ReadOnly) != 0)
                {
                    return true;
                }
                if (OwningRow != null && (OwningRow.DataGridView == null || RowIndex >= 0) && OwningRow.ReadOnly)
                {
                    return true;
                }
                if (DataGridView != null && RowIndex >= 0 && ColumnIndex >= 0)
                {
                    Debug.Assert(DataGridView.Rows.GetRowState(RowIndex) == DataGridView.Rows.SharedRow(RowIndex).State);
                    return OwningColumn.ReadOnly;
                }
                return false;
            }
            set
            {
                if (DataGridView != null)
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
                    if (OwningRow == null)
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
                Debug.Assert(value != ReadOnly);
                if (value)
                {
                    State = State | DataGridViewElementStates.ReadOnly;
                }
                else
                {
                    State = State & ~DataGridViewElementStates.ReadOnly;
                }

                DataGridView?.OnDataGridViewElementStateChanged(this, -1, DataGridViewElementStates.ReadOnly);
            }
        }

        [
            Browsable(false)
        ]
        public virtual bool Resizable
        {
            get
            {
                Debug.Assert((State & DataGridViewElementStates.Resizable) == 0);

                if (OwningRow != null && (OwningRow.DataGridView == null || RowIndex >= 0) && OwningRow.Resizable == DataGridViewTriState.True)
                {
                    return true;
                }

                if (DataGridView != null && RowIndex >= 0 && ColumnIndex >= 0)
                {
                    Debug.Assert(DataGridView.Rows.GetRowState(RowIndex) == DataGridView.Rows.SharedRow(RowIndex).State);
                    return OwningColumn.Resizable == DataGridViewTriState.True;
                }

                return false;
            }
        }

        /// <summary>
        ///  Gets or sets the index of a row in the <see cref='DataGrid'/> control.
        /// </summary>
        [Browsable(false)]
        public int RowIndex => OwningRow?.Index ?? -1;

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual bool Selected
        {
            get
            {
                if ((State & DataGridViewElementStates.Selected) != 0)
                {
                    return true;
                }

                if (OwningRow != null && (OwningRow.DataGridView == null || RowIndex >= 0) && OwningRow.Selected)
                {
                    return true;
                }

                if (DataGridView != null && RowIndex >= 0 && ColumnIndex >= 0)
                {
                    Debug.Assert(DataGridView.Rows.GetRowState(RowIndex) == DataGridView.Rows.SharedRow(RowIndex).State);
                    return OwningColumn.Selected;
                }

                return false;
            }
            set
            {
                if (DataGridView != null)
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
                    State = State | DataGridViewElementStates.Selected;
                }
                else
                {
                    State = State & ~DataGridViewElementStates.Selected;
                }
                if (DataGridView != null)
                {
                    DataGridView.OnDataGridViewElementStateChanged(this, -1, DataGridViewElementStates.Selected);
                }
            }
        }

        [Browsable(false)]
        public Size Size
        {
            get
            {
                return GetSize(RowIndex);
            }
        }

        private protected Rectangle StdBorderWidths
        {
            get
            {
                if (DataGridView != null)
                {
                    DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder = new DataGridViewAdvancedBorderStyle(), dgvabsEffective;
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

        [
            Browsable(true)
        ]
        public DataGridViewCellStyle Style
        {
            get
            {
                DataGridViewCellStyle dgvcs = (DataGridViewCellStyle)Properties.GetObject(PropCellStyle);
                if (dgvcs == null)
                {
                    dgvcs = new DataGridViewCellStyle();
                    dgvcs.AddScope(DataGridView, DataGridViewCellStyleScopes.Cell);
                    Properties.SetObject(PropCellStyle, dgvcs);
                }
                return dgvcs;
            }
            set
            {
                DataGridViewCellStyle dgvcs = null;
                if (HasStyle)
                {
                    dgvcs = Style;
                    dgvcs.RemoveScope(DataGridViewCellStyleScopes.Cell);
                }
                if (value != null || Properties.ContainsObject(PropCellStyle))
                {
                    if (value != null)
                    {
                        value.AddScope(DataGridView, DataGridViewCellStyleScopes.Cell);
                    }
                    Properties.SetObject(PropCellStyle, value);
                }
                if (((dgvcs != null && value == null) ||
                    (dgvcs == null && value != null) ||
                    (dgvcs != null && value != null && !dgvcs.Equals(Style))) && DataGridView != null)
                {
                    DataGridView.OnCellStyleChanged(this);
                }
            }
        }

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
                return Properties.GetObject(PropCellTag);
            }
            set
            {
                if (value != null || Properties.ContainsObject(PropCellTag))
                {
                    Properties.SetObject(PropCellTag, value);
                }
            }
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public string ToolTipText
        {
            get
            {
                return GetToolTipText(RowIndex);
            }
            set
            {
                ToolTipTextInternal = value;
                _useDefaultToolTipText = false;
            }
        }

        private string ToolTipTextInternal
        {
            get
            {
                object toolTipText = Properties.GetObject(PropCellToolTipText);
                return (toolTipText == null) ? string.Empty : (string)toolTipText;
            }
            set
            {
                string toolTipText = ToolTipTextInternal;
                if (!string.IsNullOrEmpty(value) || Properties.ContainsObject(PropCellToolTipText))
                {
                    Properties.SetObject(PropCellToolTipText, value);
                }
                if (DataGridView != null && !toolTipText.Equals(ToolTipTextInternal))
                {
                    DataGridView.OnCellToolTipTextChanged(this);
                }
            }
        }

        [
            Browsable(false)
        ]
        public object Value
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

        [
            Browsable(false)
        ]
        public virtual Type ValueType
        {
            get
            {
                Type cellValueType = (Type)Properties.GetObject(PropCellValueType);
                if (cellValueType == null && OwningColumn != null)
                {
                    cellValueType = OwningColumn.ValueType;
                }

                return cellValueType;
            }
            set
            {
                if (value != null || Properties.ContainsObject(PropCellValueType))
                {
                    Properties.SetObject(PropCellValueType, value);
                }
            }
        }

        private TypeConverter ValueTypeConverter
        {
            get
            {
                TypeConverter valueTypeConverter = null;
                if (OwningColumn != null)
                {
                    valueTypeConverter = OwningColumn.BoundColumnConverter;
                }
                if (valueTypeConverter == null && ValueType != null)
                {
                    if (DataGridView != null)
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

        [
            Browsable(false)
        ]
        public virtual bool Visible
        {
            get
            {
                Debug.Assert((State & DataGridViewElementStates.Visible) == 0);

                if (DataGridView != null && RowIndex >= 0 && ColumnIndex >= 0)
                {
                    Debug.Assert(DataGridView.Rows.GetRowState(RowIndex) == DataGridView.Rows.SharedRow(RowIndex).State);
                    return OwningColumn.Visible && OwningRow.Visible;
                }
                else if (OwningRow != null && (OwningRow.DataGridView == null || RowIndex >= 0))
                {
                    return OwningRow.Visible;
                }
                return false;
            }
        }

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
            if (dataGridViewAdvancedBorderStyleInput == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewAdvancedBorderStyleInput));
            }
            if (dataGridViewAdvancedBorderStylePlaceholder == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewAdvancedBorderStylePlaceholder));
            }

            switch (dataGridViewAdvancedBorderStyleInput.All)
            {
                case DataGridViewAdvancedCellBorderStyle.Single:
                    if (DataGridView != null && DataGridView.RightToLeftInternal)
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
                    if (DataGridView != null && DataGridView.AdvancedCellBorderStyle == dataGridViewAdvancedBorderStyleInput)
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
            if (advancedBorderStyle == null)
            {
                throw new ArgumentNullException(nameof(advancedBorderStyle));
            }

            Rectangle rect = new Rectangle
            {
                X = (advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.None) ? 0 : 1
            };
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

            if (OwningColumn != null)
            {
                if (DataGridView != null && DataGridView.RightToLeftInternal)
                {
                    rect.X += OwningColumn.DividerWidth;
                }
                else
                {
                    rect.Width += OwningColumn.DividerWidth;
                }
            }
            if (OwningRow != null)
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
            Debug.Assert(DataGridView != null);
            Debug.Assert(ColumnIndex >= 0);
            DataGridViewElementStates orFlags = DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.Selected;
            DataGridViewElementStates andFlags = DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Visible;
            DataGridViewElementStates cellState = (OwningColumn.State & orFlags);
            cellState |= (rowState & orFlags);
            cellState |= ((OwningColumn.State & andFlags) & (rowState & andFlags));
            return cellState;
        }

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
            if (ContextMenuStripInternal != null)
            {
                dataGridViewCell.ContextMenuStrip = ContextMenuStripInternal.Clone();
            }
            dataGridViewCell.State = State & ~DataGridViewElementStates.Selected;
            dataGridViewCell.Tag = Tag;

            if (DataGridView != null)
            {
                KeyboardToolTipStateMachine.Instance.Hook(dataGridViewCell, DataGridView.KeyboardToolTip);
            }
        }

        public virtual object Clone()
        {
            DataGridViewCell dataGridViewCell = (DataGridViewCell)System.Activator.CreateInstance(GetType());
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
            Debug.Assert(DataGridView != null);
            bool singleVerticalBorderAdded = !DataGridView.RowHeadersVisible && DataGridView.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single;
            bool singleHorizontalBorderAdded = !DataGridView.ColumnHeadersVisible && DataGridView.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single;
            DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder = new DataGridViewAdvancedBorderStyle();

            if (rowIndex > -1 && OwningColumn != null)
            {
                // Inner cell case
                dgvabsEffective = AdjustCellBorderStyle(DataGridView.AdvancedCellBorderStyle,
                    dataGridViewAdvancedBorderStylePlaceholder,
                    singleVerticalBorderAdded,
                    singleHorizontalBorderAdded,
                    ColumnIndex == DataGridView.FirstDisplayedColumnIndex /*isFirstDisplayedColumn*/,
                    rowIndex == DataGridView.FirstDisplayedRowIndex /*isFirstDisplayedRow*/);
                DataGridViewElementStates rowState = DataGridView.Rows.GetRowState(rowIndex);
                cellState = CellStateFromColumnRowStates(rowState);
                cellState |= State;
            }
            else if (OwningColumn != null)
            {
                // Column header cell case
                Debug.Assert(rowIndex == -1);
                Debug.Assert(this is DataGridViewColumnHeaderCell, "if the row index == -1 and we have an owning column this should be a column header cell");
                DataGridViewColumn dataGridViewColumn = DataGridView.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None);
                bool isLastVisibleColumn = (dataGridViewColumn != null && dataGridViewColumn.Index == ColumnIndex);
                dgvabsEffective = DataGridView.AdjustColumnHeaderBorderStyle(DataGridView.AdvancedColumnHeadersBorderStyle,
                    dataGridViewAdvancedBorderStylePlaceholder,
                    ColumnIndex == DataGridView.FirstDisplayedColumnIndex,
                    isLastVisibleColumn);
                cellState = OwningColumn.State | State;
            }
            else if (OwningRow != null)
            {
                // Row header cell case
                Debug.Assert(this is DataGridViewRowHeaderCell);
                dgvabsEffective = OwningRow.AdjustRowHeaderBorderStyle(DataGridView.AdvancedRowHeadersBorderStyle,
                    dataGridViewAdvancedBorderStylePlaceholder,
                    singleVerticalBorderAdded,
                    singleHorizontalBorderAdded,
                    rowIndex == DataGridView.FirstDisplayedRowIndex /*isFirstDisplayedRow*/,
                    rowIndex == DataGridView.Rows.GetLastRow(DataGridViewElementStates.Visible) /*isLastVisibleRow*/);
                cellState = OwningRow.GetState(rowIndex) | State;
            }
            else
            {
                Debug.Assert(OwningColumn == null);
                Debug.Assert(OwningRow == null);
                Debug.Assert(rowIndex == -1);
                // TopLeft header cell case
                dgvabsEffective = DataGridView.AdjustedTopLeftHeaderBorderStyle;
                cellState = State;
            }

            cellBounds = new Rectangle(new Point(0, 0), GetSize(rowIndex));
        }

        internal Rectangle ComputeErrorIconBounds(Rectangle cellValueBounds)
        {
            if (cellValueBounds.Width >= DATAGRIDVIEWCELL_iconMarginWidth * 2 + iconsWidth &&
                cellValueBounds.Height >= DATAGRIDVIEWCELL_iconMarginHeight * 2 + iconsHeight)
            {
                Rectangle bmpRect = new Rectangle(DataGridView.RightToLeftInternal ?
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

        protected virtual bool ContentClickUnsharesRow(DataGridViewCellEventArgs e)
        {
            return false;
        }

        internal bool ContentClickUnsharesRowInternal(DataGridViewCellEventArgs e)
        {
            return ContentClickUnsharesRow(e);
        }

        protected virtual bool ContentDoubleClickUnsharesRow(DataGridViewCellEventArgs e)
        {
            return false;
        }

        internal bool ContentDoubleClickUnsharesRowInternal(DataGridViewCellEventArgs e)
        {
            return ContentDoubleClickUnsharesRow(e);
        }

        protected virtual AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewCellAccessibleObject(this);
        }

        private void DetachContextMenuStrip(object sender, EventArgs e)
        {
            ContextMenuStripInternal = null;
        }

        [
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public virtual void DetachEditingControl()
        {
            DataGridView dgv = DataGridView;
            if (dgv == null || dgv.EditingControl == null)
            {
                throw new InvalidOperationException();
            }
            if (dgv.EditingControl.ParentInternal != null)
            {
                if (dgv.EditingControl.ContainsFocus)
                {
                    if (dgv.GetContainerControl() is ContainerControl cc && (dgv.EditingControl == cc.ActiveControl || dgv.EditingControl.Contains(cc.ActiveControl)))
                    {
                        dgv.Focus();
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

                if (AccessibleRestructuringNeeded)
                {
                    if (dgv.EditingControl is DataGridViewTextBoxEditingControl)
                    {
                        dgv.TextBoxControlWasDetached = true;
                    }

                    if (dgv.EditingControl is DataGridViewComboBoxEditingControl)
                    {
                        dgv.ComboBoxControlWasDetached = true;
                    }

                    dgv.EditingControlAccessibleObject.SetParent(null);
                    AccessibilityObject.SetDetachableChild(null);

                    AccessibilityObject.RaiseStructureChangedEvent(UnsafeNativeMethods.StructureChangeType.ChildRemoved, dgv.EditingControlAccessibleObject.RuntimeId);
                }
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
            CurrentMouseLocation = DATAGRIDVIEWCELL_flagAreaNotSet;
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
                Type editingControlType = DataGridView.EditingControl.GetType();

                return
                    (editingControlType == typeof(DataGridViewComboBoxEditingControl) && !editingControlType.IsSubclassOf(typeof(DataGridViewComboBoxEditingControl))) ||
                    (editingControlType == typeof(DataGridViewTextBoxEditingControl) && !editingControlType.IsSubclassOf(typeof(DataGridViewTextBoxEditingControl)));
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
                ContextMenuStrip contextMenuStrip = (ContextMenuStrip)ContextMenuStripInternal;
                if (contextMenuStrip != null)
                {
                    contextMenuStrip.Disposed -= new EventHandler(DetachContextMenuStrip);
                }
            }
        }

        protected virtual bool DoubleClickUnsharesRow(DataGridViewCellEventArgs e)
        {
            return false;
        }

        internal bool DoubleClickUnsharesRowInternal(DataGridViewCellEventArgs e)
        {
            return DoubleClickUnsharesRow(e);
        }

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
                        // The seemingly arbitrary 160 comes from RFC
                        // Code taken from ASP.NET file xsp\System\Web\httpserverutility.cs
                        // Don't entity encode high chars (160 to 256)
                        if (ch >= 160 && ch < 256)
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

        private static Bitmap GetBitmap(string bitmapName)
        {
            Bitmap b = DpiHelper.GetBitmapFromIcon(typeof(DataGridViewCell), bitmapName);
            if (DpiHelper.IsScalingRequired)
            {
                Bitmap scaledBitmap = DpiHelper.CreateResizedBitmap(b, new Size(iconsWidth, iconsHeight));
                if (scaledBitmap != null)
                {
                    b.Dispose();
                    b = scaledBitmap;
                }
            }
            return b;
        }

        protected virtual object GetClipboardContent(int rowIndex,
                                                     bool firstCell,
                                                     bool lastCell,
                                                     bool inFirstRow,
                                                     bool inLastRow,
                                                     string format)
        {
            if (DataGridView == null)
            {
                return null;
            }
            // Header Cell classes override this implementation - this implementation is only for inner cells
            if (rowIndex < 0 || rowIndex >= DataGridView.Rows.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            // Assuming (like in other places in this class) that the formatted value is independent of the style colors.
            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false);
            object formattedValue = null;
            if (DataGridView.IsSharedCellSelected(this, rowIndex))
            {
                formattedValue = GetEditedFormattedValue(GetValue(rowIndex), rowIndex, ref dataGridViewCellStyle, DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.ClipboardContent);
            }

            StringBuilder sb = new StringBuilder(64);

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
                bool csv = string.Equals(format, DataFormats.CommaSeparatedValue, StringComparison.OrdinalIgnoreCase);
                if (csv ||
                    string.Equals(format, DataFormats.Text, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(format, DataFormats.UnicodeText, StringComparison.OrdinalIgnoreCase))
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
            ContextMenuStrip contextMenuStrip = ContextMenuStripInternal;
            if (DataGridView != null &&
                (DataGridView.VirtualMode || DataGridView.DataSource != null))
            {
                contextMenuStrip = DataGridView.OnCellContextMenuStripNeeded(ColumnIndex, rowIndex, contextMenuStrip);
            }
            return contextMenuStrip;
        }

        internal void GetContrastedPens(Color baseline, ref Pen darkPen, ref Pen lightPen)
        {
            Debug.Assert(DataGridView != null);

            int darkDistance = ColorDistance(baseline, SystemColors.ControlDark);
            int lightDistance = ColorDistance(baseline, SystemColors.ControlLightLight);

            if (SystemInformation.HighContrast)
            {
                if (darkDistance < DATAGRIDVIEWCELL_highConstrastThreshold)
                {
                    darkPen = DataGridView.GetCachedPen(ControlPaint.DarkDark(baseline));
                }
                else
                {
                    darkPen = DataGridView.GetCachedPen(SystemColors.ControlDark);
                }
                if (lightDistance < DATAGRIDVIEWCELL_highConstrastThreshold)
                {
                    lightPen = DataGridView.GetCachedPen(ControlPaint.LightLight(baseline));
                }
                else
                {
                    lightPen = DataGridView.GetCachedPen(SystemColors.ControlLightLight);
                }
            }
            else
            {
                if (darkDistance < DATAGRIDVIEWCELL_constrastThreshold)
                {
                    darkPen = DataGridView.GetCachedPen(ControlPaint.Dark(baseline));
                }
                else
                {
                    darkPen = DataGridView.GetCachedPen(SystemColors.ControlDark);
                }
                if (lightDistance < DATAGRIDVIEWCELL_constrastThreshold)
                {
                    lightPen = DataGridView.GetCachedPen(ControlPaint.Light(baseline));
                }
                else
                {
                    lightPen = DataGridView.GetCachedPen(SystemColors.ControlLightLight);
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

        public Rectangle GetContentBounds(int rowIndex)
        {
            if (DataGridView == null)
            {
                return Rectangle.Empty;
            }
            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false /*includeColors*/);
            using (Graphics g = WindowsFormsUtils.CreateMeasurementGraphics())
            {
                return GetContentBounds(g, dataGridViewCellStyle, rowIndex);
            }
        }

        protected virtual Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            return Rectangle.Empty;
        }

        private protected virtual string GetDefaultToolTipText()
        {
            return string.Empty;
        }

        internal object GetEditedFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle dataGridViewCellStyle, DataGridViewDataErrorContexts context)
        {
            Debug.Assert(DataGridView != null);
            Point ptCurrentCell = DataGridView.CurrentCellAddress;
            if (ColumnIndex == ptCurrentCell.X && rowIndex == ptCurrentCell.Y)
            {
                IDataGridViewEditingControl dgvectl = (IDataGridViewEditingControl)DataGridView.EditingControl;
                if (dgvectl != null)
                {
                    return dgvectl.GetEditingControlFormattedValue(context);
                }
                if (this is IDataGridViewEditingCell dgvecell && DataGridView.IsCurrentCellInEditMode)
                {
                    return dgvecell.GetEditingCellFormattedValue(context);
                }
                return GetFormattedValue(value, rowIndex, ref dataGridViewCellStyle, null, null, context);
            }
            return GetFormattedValue(value, rowIndex, ref dataGridViewCellStyle, null, null, context);
        }

        public object GetEditedFormattedValue(int rowIndex, DataGridViewDataErrorContexts context)
        {
            if (DataGridView == null)
            {
                return null;
            }
            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false /*includeColors*/);
            return GetEditedFormattedValue(GetValue(rowIndex), rowIndex, ref dataGridViewCellStyle, context);
        }

        internal Rectangle GetErrorIconBounds(int rowIndex)
        {
            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false /*includeColors*/);
            using (Graphics g = WindowsFormsUtils.CreateMeasurementGraphics())
            {
                return GetErrorIconBounds(g, dataGridViewCellStyle, rowIndex);
            }
        }

        protected virtual Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            return Rectangle.Empty;
        }

        protected internal virtual string GetErrorText(int rowIndex)
        {
            string errorText = string.Empty;
            object objErrorText = Properties.GetObject(PropCellErrorText);
            if (objErrorText != null)
            {
                errorText = (string)objErrorText;
            }
            else if (DataGridView != null &&
                     rowIndex != -1 &&
                     rowIndex != DataGridView.NewRowIndex &&
                     OwningColumn != null &&
                     OwningColumn.IsDataBound &&
                     DataGridView.DataConnection != null)
            {
                errorText = DataGridView.DataConnection.GetError(OwningColumn.BoundColumnIndex, ColumnIndex, rowIndex);
            }

            if (DataGridView != null && (DataGridView.VirtualMode || DataGridView.DataSource != null) &&
                ColumnIndex >= 0 && rowIndex >= 0)
            {
                errorText = DataGridView.OnCellErrorTextNeeded(ColumnIndex, rowIndex, errorText);
            }
            return errorText;
        }

        internal object GetFormattedValue(int rowIndex, ref DataGridViewCellStyle cellStyle, DataGridViewDataErrorContexts context)
        {
            if (DataGridView == null)
            {
                return null;
            }
            else
            {
                return GetFormattedValue(GetValue(rowIndex), rowIndex, ref cellStyle, null, null, context);
            }
        }

        protected virtual object GetFormattedValue(object value,
                                                   int rowIndex,
                                                   ref DataGridViewCellStyle cellStyle,
                                                   TypeConverter valueTypeConverter,
                                                   TypeConverter formattedValueTypeConverter,
                                                   DataGridViewDataErrorContexts context)
        {
            if (DataGridView == null)
            {
                return null;
            }

            DataGridViewCellFormattingEventArgs gdvcfe = DataGridView.OnCellFormatting(ColumnIndex, rowIndex, value, FormattedValueType, cellStyle);
            cellStyle = gdvcfe.CellStyle;
            bool formattingApplied = gdvcfe.FormattingApplied;
            object formattedValue = gdvcfe.Value;
            bool checkFormattedValType = true;

            if (!formattingApplied &&
                FormattedValueType != null &&
                (formattedValue == null || !FormattedValueType.IsAssignableFrom(formattedValue.GetType())))
            {
                try
                {
                    formattedValue = Formatter.FormatObject(formattedValue,
                                                            FormattedValueType,
                                                            valueTypeConverter ?? ValueTypeConverter, /*sourceConverter*/
                                                            formattedValueTypeConverter ?? FormattedValueTypeConverter, /*targetConverter*/
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
                (formattedValue == null || FormattedValueType == null || !FormattedValueType.IsAssignableFrom(formattedValue.GetType())))
            {
                if (formattedValue == null &&
                    cellStyle.NullValue == null &&
                    FormattedValueType != null &&
                    !typeof(ValueType).IsAssignableFrom(FormattedValueType))
                {
                    // null is an acceptable formatted value
                    return null;
                }
                Exception exception = null;
                if (FormattedValueType == null)
                {
                    exception = new FormatException(SR.DataGridViewCell_FormattedValueTypeNull);
                }
                else
                {
                    exception = new FormatException(SR.DataGridViewCell_FormattedValueHasWrongType);
                }
                DataGridViewDataErrorEventArgs dgvdee = new DataGridViewDataErrorEventArgs(exception,
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

        static internal DataGridViewFreeDimension GetFreeDimensionFromConstraint(Size constraintSize)
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
            if (DataGridView == null)
            {
                return -1;
            }

            Debug.Assert(OwningRow != null);
            return OwningRow.GetHeight(rowIndex);
        }

        public virtual ContextMenuStrip GetInheritedContextMenuStrip(int rowIndex)
        {
            if (DataGridView != null)
            {
                if (rowIndex < 0 || rowIndex >= DataGridView.Rows.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                if (ColumnIndex < 0)
                {
                    throw new InvalidOperationException();
                }
                Debug.Assert(ColumnIndex < DataGridView.Columns.Count);
            }

            ContextMenuStrip contextMenuStrip = GetContextMenuStrip(rowIndex);
            if (contextMenuStrip != null)
            {
                return contextMenuStrip;
            }

            if (OwningRow != null)
            {
                contextMenuStrip = OwningRow.GetContextMenuStrip(rowIndex);
                if (contextMenuStrip != null)
                {
                    return contextMenuStrip;
                }
            }

            if (OwningColumn != null)
            {
                contextMenuStrip = OwningColumn.ContextMenuStrip;
                if (contextMenuStrip != null)
                {
                    return contextMenuStrip;
                }
            }

            if (DataGridView != null)
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

            if (DataGridView == null)
            {
                Debug.Assert(RowIndex == -1);
                if (rowIndex != -1)
                {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(rowIndex), rowIndex));
                }
                if (OwningRow != null)
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
            if (rowIndex < 0 || rowIndex >= DataGridView.Rows.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            Debug.Assert(OwningColumn != null);
            Debug.Assert(OwningRow != null);
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

        public virtual DataGridViewCellStyle GetInheritedStyle(DataGridViewCellStyle inheritedCellStyle, int rowIndex, bool includeColors)
        {
            if (DataGridView == null)
            {
                throw new InvalidOperationException(SR.DataGridView_CellNeedsDataGridViewForInheritedStyle);
            }
            if (rowIndex < 0 || rowIndex >= DataGridView.Rows.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            if (ColumnIndex < 0)
            {
                throw new InvalidOperationException();
            }
            Debug.Assert(ColumnIndex < DataGridView.Columns.Count);

            DataGridViewCellStyle inheritedCellStyleTmp;
            if (inheritedCellStyle == null)
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

            DataGridViewCellStyle cellStyle = null;
            if (HasStyle)
            {
                cellStyle = Style;
                Debug.Assert(cellStyle != null);
            }

            DataGridViewCellStyle rowStyle = null;
            if (DataGridView.Rows.SharedRow(rowIndex).HasDefaultCellStyle)
            {
                rowStyle = DataGridView.Rows.SharedRow(rowIndex).DefaultCellStyle;
                Debug.Assert(rowStyle != null);
            }

            DataGridViewCellStyle columnStyle = null;
            if (OwningColumn.HasDefaultCellStyle)
            {
                columnStyle = OwningColumn.DefaultCellStyle;
                Debug.Assert(columnStyle != null);
            }

            DataGridViewCellStyle dataGridViewStyle = DataGridView.DefaultCellStyle;
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
                else if (!DataGridView.RowsDefaultCellStyle.BackColor.IsEmpty &&
                    (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.BackColor.IsEmpty))
                {
                    inheritedCellStyleTmp.BackColor = DataGridView.RowsDefaultCellStyle.BackColor;
                }
                else if (rowIndex % 2 == 1 && !DataGridView.AlternatingRowsDefaultCellStyle.BackColor.IsEmpty)
                {
                    inheritedCellStyleTmp.BackColor = DataGridView.AlternatingRowsDefaultCellStyle.BackColor;
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
                else if (!DataGridView.RowsDefaultCellStyle.ForeColor.IsEmpty &&
                    (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.ForeColor.IsEmpty))
                {
                    inheritedCellStyleTmp.ForeColor = DataGridView.RowsDefaultCellStyle.ForeColor;
                }
                else if (rowIndex % 2 == 1 && !DataGridView.AlternatingRowsDefaultCellStyle.ForeColor.IsEmpty)
                {
                    inheritedCellStyleTmp.ForeColor = DataGridView.AlternatingRowsDefaultCellStyle.ForeColor;
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
                else if (!DataGridView.RowsDefaultCellStyle.SelectionBackColor.IsEmpty &&
                    (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.SelectionBackColor.IsEmpty))
                {
                    inheritedCellStyleTmp.SelectionBackColor = DataGridView.RowsDefaultCellStyle.SelectionBackColor;
                }
                else if (rowIndex % 2 == 1 && !DataGridView.AlternatingRowsDefaultCellStyle.SelectionBackColor.IsEmpty)
                {
                    inheritedCellStyleTmp.SelectionBackColor = DataGridView.AlternatingRowsDefaultCellStyle.SelectionBackColor;
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
                else if (!DataGridView.RowsDefaultCellStyle.SelectionForeColor.IsEmpty &&
                    (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.SelectionForeColor.IsEmpty))
                {
                    inheritedCellStyleTmp.SelectionForeColor = DataGridView.RowsDefaultCellStyle.SelectionForeColor;
                }
                else if (rowIndex % 2 == 1 && !DataGridView.AlternatingRowsDefaultCellStyle.SelectionForeColor.IsEmpty)
                {
                    inheritedCellStyleTmp.SelectionForeColor = DataGridView.AlternatingRowsDefaultCellStyle.SelectionForeColor;
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
            else if (DataGridView.RowsDefaultCellStyle.Font != null &&
                (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.Font == null))
            {
                inheritedCellStyleTmp.Font = DataGridView.RowsDefaultCellStyle.Font;
            }
            else if (rowIndex % 2 == 1 && DataGridView.AlternatingRowsDefaultCellStyle.Font != null)
            {
                inheritedCellStyleTmp.Font = DataGridView.AlternatingRowsDefaultCellStyle.Font;
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
            else if (DataGridView.RowsDefaultCellStyle.Format.Length != 0 &&
                (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.Format.Length == 0))
            {
                inheritedCellStyleTmp.Format = DataGridView.RowsDefaultCellStyle.Format;
            }
            else if (rowIndex % 2 == 1 && DataGridView.AlternatingRowsDefaultCellStyle.Format.Length != 0)
            {
                inheritedCellStyleTmp.Format = DataGridView.AlternatingRowsDefaultCellStyle.Format;
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
            else if (!DataGridView.RowsDefaultCellStyle.IsFormatProviderDefault &&
                     (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.IsFormatProviderDefault))
            {
                inheritedCellStyleTmp.FormatProvider = DataGridView.RowsDefaultCellStyle.FormatProvider;
            }
            else if (rowIndex % 2 == 1 && !DataGridView.AlternatingRowsDefaultCellStyle.IsFormatProviderDefault)
            {
                inheritedCellStyleTmp.FormatProvider = DataGridView.AlternatingRowsDefaultCellStyle.FormatProvider;
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
            else if (DataGridView.RowsDefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet &&
                (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.Alignment == DataGridViewContentAlignment.NotSet))
            {
                inheritedCellStyleTmp.AlignmentInternal = DataGridView.RowsDefaultCellStyle.Alignment;
            }
            else if (rowIndex % 2 == 1 && DataGridView.AlternatingRowsDefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet)
            {
                inheritedCellStyleTmp.AlignmentInternal = DataGridView.AlternatingRowsDefaultCellStyle.Alignment;
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
            else if (DataGridView.RowsDefaultCellStyle.WrapMode != DataGridViewTriState.NotSet &&
                (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.WrapMode == DataGridViewTriState.NotSet))
            {
                inheritedCellStyleTmp.WrapModeInternal = DataGridView.RowsDefaultCellStyle.WrapMode;
            }
            else if (rowIndex % 2 == 1 && DataGridView.AlternatingRowsDefaultCellStyle.WrapMode != DataGridViewTriState.NotSet)
            {
                inheritedCellStyleTmp.WrapModeInternal = DataGridView.AlternatingRowsDefaultCellStyle.WrapMode;
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
            else if (DataGridView.RowsDefaultCellStyle.Tag != null &&
                (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.Tag == null))
            {
                inheritedCellStyleTmp.Tag = DataGridView.RowsDefaultCellStyle.Tag;
            }
            else if (rowIndex % 2 == 1 && DataGridView.AlternatingRowsDefaultCellStyle.Tag != null)
            {
                inheritedCellStyleTmp.Tag = DataGridView.AlternatingRowsDefaultCellStyle.Tag;
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
            else if (DataGridView.RowsDefaultCellStyle.Padding != Padding.Empty &&
                (rowIndex % 2 == 0 || DataGridView.AlternatingRowsDefaultCellStyle.Padding == Padding.Empty))
            {
                inheritedCellStyleTmp.PaddingInternal = DataGridView.RowsDefaultCellStyle.Padding;
            }
            else if (rowIndex % 2 == 1 && DataGridView.AlternatingRowsDefaultCellStyle.Padding != Padding.Empty)
            {
                inheritedCellStyleTmp.PaddingInternal = DataGridView.AlternatingRowsDefaultCellStyle.Padding;
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

            if (DataGridView == null)
            {
                return -1;
            }

            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false);
            using (Graphics g = WindowsFormsUtils.CreateMeasurementGraphics())
            {
                return GetPreferredSize(g, dataGridViewCellStyle, rowIndex, new Size(width, 0)).Height;
            }
        }

        internal Size GetPreferredSize(int rowIndex)
        {
            if (DataGridView == null)
            {
                return new Size(-1, -1);
            }
            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false);
            using (Graphics g = WindowsFormsUtils.CreateMeasurementGraphics())
            {
                return GetPreferredSize(g, dataGridViewCellStyle, rowIndex, Size.Empty);
            }
        }

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

            if (DataGridView == null)
            {
                return -1;
            }

            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false);
            using (Graphics g = WindowsFormsUtils.CreateMeasurementGraphics())
            {
                return GetPreferredSize(g, dataGridViewCellStyle, rowIndex, new Size(0, height)).Width;
            }
        }

        protected virtual Size GetSize(int rowIndex)
        {
            if (DataGridView == null)
            {
                return new Size(-1, -1);
            }
            if (rowIndex == -1)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidPropertyGetOnSharedCell, "Size"));
            }

            Debug.Assert(OwningColumn != null);
            Debug.Assert(OwningRow != null);
            return new Size(OwningColumn.Thickness, OwningRow.GetHeight(rowIndex));
        }

        private protected string GetInternalToolTipText(int rowIndex)
        {
            string toolTipText = ToolTipTextInternal;
            if (DataGridView != null &&
                (DataGridView.VirtualMode || DataGridView.DataSource != null))
            {
                toolTipText = DataGridView.OnCellToolTipTextNeeded(ColumnIndex, rowIndex, toolTipText);
            }

            return toolTipText;
        }

        private string GetToolTipText(int rowIndex)
        {
            string toolTipText = GetInternalToolTipText(rowIndex);
                                   
            if (ColumnIndex < 0 || RowIndex < 0)
            {
                return toolTipText;  //  Cells in the Unit tests have ColumnIndex & RowIndex < 0 and 
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
        
        private protected string GetToolTipTextWithoutMnemonic(string toolTipText)
        {
            if (WindowsFormsUtils.ContainsMnemonic(toolTipText))
            {
                toolTipText = string.Join("", toolTipText.Split('&'));
            }

            return toolTipText;
        }

        protected virtual object GetValue(int rowIndex)
        {
            DataGridView dataGridView = DataGridView;
            if (dataGridView != null)
            {
                if (rowIndex < 0 || rowIndex >= dataGridView.Rows.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                if (ColumnIndex < 0)
                {
                    throw new InvalidOperationException();
                }
                Debug.Assert(ColumnIndex < dataGridView.Columns.Count);
            }
            if (dataGridView == null ||
                (dataGridView.AllowUserToAddRowsInternal && rowIndex > -1 && rowIndex == dataGridView.NewRowIndex && rowIndex != dataGridView.CurrentCellAddress.Y) ||
                (!dataGridView.VirtualMode && OwningColumn != null && !OwningColumn.IsDataBound) ||
                rowIndex == -1 ||
                ColumnIndex == -1)
            {
                return Properties.GetObject(PropCellValue);
            }
            else if (OwningColumn != null && OwningColumn.IsDataBound)
            {
                DataGridView.DataGridViewDataConnection dataConnection = dataGridView.DataConnection;
                if (dataConnection == null)
                {
                    return null;
                }
                else if (dataConnection.CurrencyManager.Count <= rowIndex)
                {
                    return Properties.GetObject(PropCellValue);
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

        internal object GetValueInternal(int rowIndex)
        {
            return GetValue(rowIndex);
        }

        [
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public virtual void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            DataGridView dgv = DataGridView;
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

            if (AccessibleRestructuringNeeded &&
                ((dgv.ComboBoxControlWasDetached && dgv.EditingControl is DataGridViewComboBoxEditingControl) ||
                (dgv.TextBoxControlWasDetached && dgv.EditingControl is DataGridViewTextBoxEditingControl)))
            {
                // Recreate control handle is necessary for cases when the same control was detached and then
                // reattached to clear accessible hierarchy cache and not announce previous editing cell.
                dgv.EditingControl.RecreateHandleCore();

                dgv.ComboBoxControlWasDetached = false;
                dgv.TextBoxControlWasDetached = false;
            }
        }

        protected virtual bool KeyDownUnsharesRow(KeyEventArgs e, int rowIndex)
        {
            return false;
        }

        internal bool KeyDownUnsharesRowInternal(KeyEventArgs e, int rowIndex)
        {
            return KeyDownUnsharesRow(e, rowIndex);
        }

        public virtual bool KeyEntersEditMode(KeyEventArgs e)
        {
            return false;
        }

        protected virtual bool KeyPressUnsharesRow(KeyPressEventArgs e, int rowIndex)
        {
            return false;
        }

        internal bool KeyPressUnsharesRowInternal(KeyPressEventArgs e, int rowIndex)
        {
            return KeyPressUnsharesRow(e, rowIndex);
        }

        protected virtual bool KeyUpUnsharesRow(KeyEventArgs e, int rowIndex)
        {
            return false;
        }

        internal bool KeyUpUnsharesRowInternal(KeyEventArgs e, int rowIndex)
        {
            return KeyUpUnsharesRow(e, rowIndex);
        }

        protected virtual bool LeaveUnsharesRow(int rowIndex, bool throughMouseClick)
        {
            return false;
        }

        internal bool LeaveUnsharesRowInternal(int rowIndex, bool throughMouseClick)
        {
            return LeaveUnsharesRow(rowIndex, throughMouseClick);
        }

        [
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public static int MeasureTextHeight(Graphics graphics, string text, Font font, int maxWidth, TextFormatFlags flags)
        {
            return DataGridViewCell.MeasureTextHeight(graphics, text, font, maxWidth, flags, out bool widthTruncated);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
                throw new ArgumentOutOfRangeException(nameof(maxWidth), string.Format(SR.InvalidLowBoundArgument, "maxWidth", (maxWidth).ToString(CultureInfo.CurrentCulture), 0));
            }

            if (!DataGridViewUtilities.ValidTextFormatFlags(flags))
            {
                throw new InvalidEnumArgumentException(nameof(flags), (int)flags, typeof(TextFormatFlags));
            }

            flags &= textFormatSupportedFlags;
            // Dont use passed in graphics so we can optimze measurement
            Size requiredSize = TextRenderer.MeasureText(text, font, new Size(maxWidth, int.MaxValue), flags);
            widthTruncated = (requiredSize.Width > maxWidth);
            return requiredSize.Height;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
                throw new InvalidEnumArgumentException(nameof(flags), (int)flags, typeof(TextFormatFlags));
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
            float maxWidth = (float)(textOneLineSize.Width * textOneLineSize.Width) / (float)textOneLineSize.Height / maxRatio * 1.1F;
            Size textSize;
            do
            {
                // Dont use passed in graphics so we can optimze measurement
                textSize = TextRenderer.MeasureText(text, font, new Size((int)maxWidth, int.MaxValue), flags);
                if ((float)(textSize.Width / textSize.Height) <= maxRatio || textSize.Width > (int)maxWidth)
                {
                    return textSize;
                }
                maxWidth = (float)textSize.Width * 0.9F;
            }
            while (maxWidth > 1.0F);
            return textSize;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
                throw new InvalidEnumArgumentException(nameof(flags), (int)flags, typeof(TextFormatFlags));
            }

            flags &= textFormatSupportedFlags;
            // Dont use passed in graphics so we can optimze measurement
            return TextRenderer.MeasureText(text, font, new Size(int.MaxValue, int.MaxValue), flags);
        }

        [
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public static int MeasureTextWidth(Graphics graphics, string text, Font font, int maxHeight, TextFormatFlags flags)
        {
            if (maxHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxHeight), string.Format(SR.InvalidLowBoundArgument, "maxHeight", (maxHeight).ToString(CultureInfo.CurrentCulture), 0));
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
                float maxWidth = (float)lastFittingWidth * 0.9F;
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

        protected virtual bool MouseClickUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return false;
        }

        internal bool MouseClickUnsharesRowInternal(DataGridViewCellMouseEventArgs e)
        {
            return MouseClickUnsharesRow(e);
        }

        protected virtual bool MouseDoubleClickUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return false;
        }

        internal bool MouseDoubleClickUnsharesRowInternal(DataGridViewCellMouseEventArgs e)
        {
            return MouseDoubleClickUnsharesRow(e);
        }

        protected virtual bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return false;
        }

        internal bool MouseDownUnsharesRowInternal(DataGridViewCellMouseEventArgs e)
        {
            return MouseDownUnsharesRow(e);
        }

        protected virtual bool MouseEnterUnsharesRow(int rowIndex)
        {
            return false;
        }

        internal bool MouseEnterUnsharesRowInternal(int rowIndex)
        {
            return MouseEnterUnsharesRow(rowIndex);
        }

        protected virtual bool MouseLeaveUnsharesRow(int rowIndex)
        {
            return false;
        }

        internal bool MouseLeaveUnsharesRowInternal(int rowIndex)
        {
            return MouseLeaveUnsharesRow(rowIndex);
        }

        protected virtual bool MouseMoveUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return false;
        }

        internal bool MouseMoveUnsharesRowInternal(DataGridViewCellMouseEventArgs e)
        {
            return MouseMoveUnsharesRow(e);
        }

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
            Debug.Assert(DataGridView != null);
            if (!DataGridView.ShowCellToolTips)
            {
                return;
            }

            // Don't show a tooltip for edited cells with an editing control
            Point ptCurrentCell = DataGridView.CurrentCellAddress;
            if (ptCurrentCell.X != -1 &&
                ptCurrentCell.X == ColumnIndex &&
                ptCurrentCell.Y == rowIndex &&
                DataGridView.EditingControl != null)
            {
                Debug.Assert(DataGridView.IsCurrentCellInEditMode);
                return;
            }

            // get the tool tip string
            string toolTipText = GetToolTipText(rowIndex);

            if (string.IsNullOrEmpty(toolTipText))
            {
                if (FormattedValueType == stringType)
                {
                    if (rowIndex != -1 && OwningColumn != null)
                    {
                        int width = GetPreferredWidth(rowIndex, OwningRow.Height);
                        int height = GetPreferredHeight(rowIndex, OwningColumn.Width);

                        if (OwningColumn.Width < width || OwningRow.Height < height)
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
                    else if ((rowIndex != -1 && OwningRow != null && DataGridView.RowHeadersVisible && DataGridView.RowHeadersWidth > 0 && OwningColumn == null) ||
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
            }

            if (!string.IsNullOrEmpty(toolTipText))
            {
                KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);
                DataGridView.ActivateToolTip(true /*activate*/, toolTipText, ColumnIndex, rowIndex);
            }

            // for debugging
            // Console.WriteLine("OnCellDATA_AreaMouseENTER. ToolTipText : " + toolTipText);
        }

        private void OnCellDataAreaMouseLeaveInternal()
        {
            if (DataGridView.IsDisposed)
            {
                return;
            }

            DataGridView.ActivateToolTip(false /*activate*/, string.Empty, -1, -1);
            // for debugging
            // Console.WriteLine("OnCellDATA_AreaMouseLEAVE");
        }

        private void OnCellErrorAreaMouseEnterInternal(int rowIndex)
        {
            string errorText = GetErrorText(rowIndex);
            Debug.Assert(!string.IsNullOrEmpty(errorText), "if we entered the cell error area then an error was painted, so we should have an error");
            KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);
            DataGridView.ActivateToolTip(true /*activate*/, errorText, ColumnIndex, rowIndex);

            // for debugging
            // Console.WriteLine("OnCellERROR_AreaMouseENTER. ErrorText : " + errorText);
        }

        private void OnCellErrorAreaMouseLeaveInternal()
        {
            DataGridView.ActivateToolTip(false /*activate*/, string.Empty, -1, -1);
            // for debugging
            // Console.WriteLine("OnCellERROR_AreaMouseLEAVE");
        }

        protected virtual void OnClick(DataGridViewCellEventArgs e)
        {
        }

        internal void OnClickInternal(DataGridViewCellEventArgs e)
        {
            OnClick(e);
        }

        internal void OnCommonChange()
        {
            if (DataGridView != null && !DataGridView.IsDisposed && !DataGridView.Disposing)
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

        internal void OnContentClickInternal(DataGridViewCellEventArgs e)
        {
            OnContentClick(e);
        }

        protected virtual void OnContentDoubleClick(DataGridViewCellEventArgs e)
        {
        }

        internal void OnContentDoubleClickInternal(DataGridViewCellEventArgs e)
        {
            OnContentDoubleClick(e);
        }

        protected virtual void OnDoubleClick(DataGridViewCellEventArgs e)
        {
        }

        internal void OnDoubleClickInternal(DataGridViewCellEventArgs e)
        {
            OnDoubleClick(e);
        }

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

        protected virtual void OnKeyDown(KeyEventArgs e, int rowIndex)
        {
        }

        internal void OnKeyPressInternal(KeyPressEventArgs e, int rowIndex)
        {
            OnKeyPress(e, rowIndex);
        }

        protected virtual void OnKeyPress(KeyPressEventArgs e, int rowIndex)
        {
        }

        protected virtual void OnKeyUp(KeyEventArgs e, int rowIndex)
        {
        }

        internal void OnKeyUpInternal(KeyEventArgs e, int rowIndex)
        {
            OnKeyUp(e, rowIndex);
        }

        protected virtual void OnLeave(int rowIndex, bool throughMouseClick)
        {
        }

        internal void OnLeaveInternal(int rowIndex, bool throughMouseClick)
        {
            OnLeave(rowIndex, throughMouseClick);
        }

        protected virtual void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
        }

        internal void OnMouseClickInternal(DataGridViewCellMouseEventArgs e)
        {
            OnMouseClick(e);
        }

        protected virtual void OnMouseDoubleClick(DataGridViewCellMouseEventArgs e)
        {
        }

        internal void OnMouseDoubleClickInternal(DataGridViewCellMouseEventArgs e)
        {
            OnMouseDoubleClick(e);
        }

        protected virtual void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
        }

        internal void OnMouseDownInternal(DataGridViewCellMouseEventArgs e)
        {
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

            CurrentMouseLocation = DATAGRIDVIEWCELL_flagAreaNotSet;
            OnMouseLeave(rowIndex);
        }

        protected virtual void OnMouseMove(DataGridViewCellMouseEventArgs e)
        {
        }

        internal void OnMouseMoveInternal(DataGridViewCellMouseEventArgs e)
        {
            byte mouseLocation = CurrentMouseLocation;
            UpdateCurrentMouseLocation(e);
            Debug.Assert(CurrentMouseLocation != DATAGRIDVIEWCELL_flagAreaNotSet);
            switch (mouseLocation)
            {
                case DATAGRIDVIEWCELL_flagAreaNotSet:
                    if (CurrentMouseLocation == DATAGRIDVIEWCELL_flagDataArea)
                    {
                        OnCellDataAreaMouseEnterInternal(e.RowIndex);
                    }
                    else
                    {
                        OnCellErrorAreaMouseEnterInternal(e.RowIndex);
                    }
                    break;
                case DATAGRIDVIEWCELL_flagDataArea:
                    if (CurrentMouseLocation == DATAGRIDVIEWCELL_flagErrorArea)
                    {
                        OnCellDataAreaMouseLeaveInternal();
                        OnCellErrorAreaMouseEnterInternal(e.RowIndex);
                    }
                    break;
                case DATAGRIDVIEWCELL_flagErrorArea:
                    if (CurrentMouseLocation == DATAGRIDVIEWCELL_flagDataArea)
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

            if (DataGridView != null && e.ColumnIndex < DataGridView.Columns.Count && e.RowIndex < DataGridView.Rows.Count)
            {
                OnMouseUp(e);
            }
        }

        protected override void OnDataGridViewChanged()
        {
            if (HasStyle)
            {
                if (DataGridView == null)
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
            if (advancedBorderStyle == null)
            {
                throw new ArgumentNullException(nameof(advancedBorderStyle));
            }

            if (DataGridView == null)
            {
                return;
            }

            // Using system colors for non-single grid colors for now
            int y1, y2;

            Pen penControlDark = null, penControlLightLight = null;
            Pen penBackColor = DataGridView.GetCachedPen(cellStyle.BackColor);
            Pen penGridColor = DataGridView.GridPen;

            GetContrastedPens(cellStyle.BackColor, ref penControlDark, ref penControlLightLight);

            int dividerThickness = OwningColumn?.DividerWidth ?? 0;
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
                        dividerWidthColor = DataGridView.GridPen.Color;
                        break;

                    case DataGridViewAdvancedCellBorderStyle.Inset:
                        dividerWidthColor = SystemColors.ControlLightLight;
                        break;

                    default:   /* ie DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.None */
                        dividerWidthColor = SystemColors.ControlDark;
                        break;
                }
                graphics.FillRectangle(DataGridView.GetCachedBrush(dividerWidthColor),
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
                Color dividerHeightColor;
                switch (advancedBorderStyle.Bottom)
                {
                    case DataGridViewAdvancedCellBorderStyle.Single:
                        dividerHeightColor = DataGridView.GridPen.Color;
                        break;

                    case DataGridViewAdvancedCellBorderStyle.Inset:
                        dividerHeightColor = SystemColors.ControlLightLight;
                        break;

                    default:   /* ie DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.None */
                        dividerHeightColor = SystemColors.ControlDark;
                        break;
                }
                graphics.FillRectangle(DataGridView.GetCachedBrush(dividerHeightColor), bounds.X, bounds.Bottom - dividerThickness, bounds.Width, dividerThickness);
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
                int dividerThickness = this.owningColumn == null ? 0 : this.OwningColumn.DividerWidth;
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

                dividerThickness = OwningRow.DividerHeight ?? 0;
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

        protected virtual void PaintErrorIcon(Graphics graphics, Rectangle clipBounds, Rectangle cellValueBounds, string errorText)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            if (DataGridView == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(errorText) &&
                cellValueBounds.Width >= DATAGRIDVIEWCELL_iconMarginWidth * 2 + iconsWidth &&
                cellValueBounds.Height >= DATAGRIDVIEWCELL_iconMarginHeight * 2 + iconsHeight)
            {
                PaintErrorIcon(graphics, ComputeErrorIconBounds(cellValueBounds));
            }
        }

        private static void PaintErrorIcon(Graphics graphics, Rectangle iconBounds)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

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
            Debug.Assert(DataGridView != null);
            DataGridView dataGridView = DataGridView;
            int columnIndex = ColumnIndex;
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

        public virtual object ParseFormattedValue(object formattedValue,
                                                  DataGridViewCellStyle cellStyle,
                                                  TypeConverter formattedValueTypeConverter,
                                                  TypeConverter valueTypeConverter)
        {
            return ParseFormattedValueInternal(ValueType, formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
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
            if (FormattedValueType == null)
            {
                throw new FormatException(SR.DataGridViewCell_FormattedValueTypeNull);
            }
            if (valueType == null)
            {
                throw new FormatException(SR.DataGridViewCell_ValueTypeNull);
            }
            if (formattedValue == null ||
                !FormattedValueType.IsAssignableFrom(formattedValue.GetType()))
            {
                throw new ArgumentException(SR.DataGridViewCell_FormattedValueHasWrongType, "formattedValue");
            }
            return Formatter.ParseObject(formattedValue,
                                         valueType,
                                         FormattedValueType,
                                         valueTypeConverter ?? ValueTypeConverter,
                                         formattedValueTypeConverter ?? FormattedValueTypeConverter,
                                         cellStyle.FormatProvider,
                                         cellStyle.NullValue,
                                         cellStyle.IsDataSourceNullValueDefault ? Formatter.GetDefaultDataSourceNullValue(valueType) : cellStyle.DataSourceNullValue);
        }

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
                DataGridView.EditingControl.Location = new Point(editingControlBounds.X, editingControlBounds.Y);
            }
            if (setSize)
            {
                DataGridView.EditingControl.Size = new Size(editingControlBounds.Width, editingControlBounds.Height);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        // Positions the editing panel and returns the normal bounds of the editing control, within the editing panel.
        public virtual Rectangle PositionEditingPanel(Rectangle cellBounds,
                                                      Rectangle cellClip,
                                                      DataGridViewCellStyle cellStyle,
                                                      bool singleVerticalBorderAdded,
                                                      bool singleHorizontalBorderAdded,
                                                      bool isFirstDisplayedColumn,
                                                      bool isFirstDisplayedRow)
        {
            if (DataGridView == null)
            {
                throw new InvalidOperationException();
            }

            DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder = new DataGridViewAdvancedBorderStyle(), dgvabsEffective;

            dgvabsEffective = AdjustCellBorderStyle(DataGridView.AdvancedCellBorderStyle,
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
            /*
            if (this.DataGridView.RightToLeftInternal)
            {
                xEditingControl = wEditingPanel - xEditingControl - wEditingControl;
            }
            */
            return new Rectangle(xEditingControl, yEditingControl, wEditingControl, hEditingControl);
        }

        protected virtual bool SetValue(int rowIndex, object value)
        {
            object originalValue = null;
            DataGridView dataGridView = DataGridView;
            if (dataGridView != null && !dataGridView.InSortOperation)
            {
                originalValue = GetValue(rowIndex);
            }

            if (dataGridView != null && OwningColumn != null && OwningColumn.IsDataBound)
            {
                DataGridView.DataGridViewDataConnection dataConnection = dataGridView.DataConnection;
                if (dataConnection == null)
                {
                    return false;
                }
                else if (dataConnection.CurrencyManager.Count <= rowIndex)
                {
                    if (value != null || Properties.ContainsObject(PropCellValue))
                    {
                        Properties.SetObject(PropCellValue, value);
                    }
                }
                else
                {
                    if (dataConnection.PushValue(OwningColumn.BoundColumnIndex, ColumnIndex, rowIndex, value))
                    {
                        if (DataGridView == null || OwningRow == null || OwningRow.DataGridView == null)
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

                        if (OwningRow.Index == DataGridView.CurrentCellAddress.Y)
                        {
                            // The user programatically changed a value in the current row.
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
            else if (dataGridView == null ||
                !dataGridView.VirtualMode ||
                rowIndex == -1 ||
                ColumnIndex == -1)
            {
                if (value != null || Properties.ContainsObject(PropCellValue))
                {
                    Properties.SetObject(PropCellValue, value);
                }
            }
            else
            {
                Debug.Assert(rowIndex >= 0);
                Debug.Assert(ColumnIndex >= 0);
                dataGridView.OnCellValuePushed(ColumnIndex, rowIndex, value);
            }

            if (dataGridView != null &&
                !dataGridView.InSortOperation &&
                ((originalValue == null && value != null) ||
                 (originalValue != null && value == null) ||
                 (originalValue != null && !value.Equals(originalValue))))
            {
                RaiseCellValueChanged(new DataGridViewCellEventArgs(ColumnIndex, rowIndex));
            }
            return true;
        }

        internal bool SetValueInternal(int rowIndex, object value)
        {
            return SetValue(rowIndex, value);
        }

        internal static bool TextFitsInBounds(Graphics graphics, string text, Font font, Size maxBounds, TextFormatFlags flags)
        {
            int requiredHeight = DataGridViewCell.MeasureTextHeight(graphics, text, font, maxBounds.Width, flags, out bool widthTruncated);
            return requiredHeight <= maxBounds.Height && !widthTruncated;
        }

        /// <summary>
        ///  Gets the row Index and column Index of the cell.
        /// </summary>
        public override string ToString()
        {
            return "DataGridViewCell { ColumnIndex=" + ColumnIndex.ToString(CultureInfo.CurrentCulture) + ", RowIndex=" + RowIndex.ToString(CultureInfo.CurrentCulture) + " }";
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
                CurrentMouseLocation = DATAGRIDVIEWCELL_flagErrorArea;
            }
            else
            {
                CurrentMouseLocation = DATAGRIDVIEWCELL_flagDataArea;
            }
        }

        [
            ComVisible(true)
        ]
        protected class DataGridViewCellAccessibleObject : AccessibleObject
        {
            private int[] runtimeId = null; // Used by UIAutomation
            private AccessibleObject _child = null;

            DataGridViewCell owner;

            public DataGridViewCellAccessibleObject()
            {
            }

            public DataGridViewCellAccessibleObject(DataGridViewCell owner)
            {
                this.owner = owner;
            }

            public override Rectangle Bounds
            {
                get
                {
                    return GetAccessibleObjectBounds(GetAccessibleObjectParent());
                }
            }

            public override string DefaultAction
            {
                get
                {
                    if (Owner == null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                    }
                    if (!Owner.ReadOnly)
                    {
                        return SR.DataGridView_AccCellDefaultAction;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            public override string Name
            {
                get
                {
                    if (owner == null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                    }
                    if (owner.OwningColumn != null)
                    {
                        string name = string.Format(SR.DataGridView_AccDataGridViewCellName, owner.OwningColumn.HeaderText, owner.OwningRow.Index);

                        if (owner.OwningColumn.SortMode != DataGridViewColumnSortMode.NotSortable)
                        {
                            DataGridViewCell dataGridViewCell = Owner;
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
                        return string.Empty;
                    }
                }
            }

            public DataGridViewCell Owner
            {
                get
                {
                    return owner;
                }
                set
                {
                    if (owner != null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerAlreadySet);
                    }
                    owner = value;
                }
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return ParentPrivate;
                }
            }

            private AccessibleObject ParentPrivate
            {
                get
                {
                    if (owner == null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                    }

                    return owner.OwningRow?.AccessibilityObject;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.Cell;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    if (owner == null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                    }

                    AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable;
                    if (owner.DataGridView != null && owner == owner.DataGridView.CurrentCell)
                    {
                        state |= AccessibleStates.Focused;
                    }

                    if (owner.Selected)
                    {
                        state |= AccessibleStates.Selected;
                    }

                    if (owner.ReadOnly)
                    {
                        state |= AccessibleStates.ReadOnly;
                    }

                    if (Owner.DataGridView != null)
                    {
                        Rectangle cellBounds;
                        if (owner.OwningColumn != null && owner.OwningRow != null)
                        {
                            cellBounds = owner.DataGridView.GetCellDisplayRectangle(owner.OwningColumn.Index, owner.OwningRow.Index, false /*cutOverflow*/);
                        }
                        else if (owner.OwningRow != null)
                        {
                            cellBounds = owner.DataGridView.GetCellDisplayRectangle(-1, owner.OwningRow.Index, false /*cutOverflow*/);
                        }
                        else if (owner.OwningColumn != null)
                        {
                            cellBounds = owner.DataGridView.GetCellDisplayRectangle(owner.OwningColumn.Index, -1, false /*cutOverflow*/);
                        }
                        else
                        {
                            cellBounds = owner.DataGridView.GetCellDisplayRectangle(-1, -1, false /*cutOverflow*/);
                        }

                        if (!cellBounds.IntersectsWith(owner.DataGridView.ClientRectangle))
                        {
                            state |= AccessibleStates.Offscreen;
                        }
                    }

                    return state;
                }
            }

            public override string Value
            {
                get
                {
                    if (owner == null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                    }

                    object formattedValue = owner.FormattedValue;
                    string formattedValueAsString = formattedValue as string;
                    if (formattedValue == null || (formattedValueAsString != null && string.IsNullOrEmpty(formattedValueAsString)))
                    {
                        return SR.DataGridView_AccNullValue;
                    }
                    else if (formattedValueAsString != null)
                    {
                        return formattedValueAsString;
                    }
                    else if (owner.OwningColumn != null)
                    {
                        TypeConverter converter = owner.FormattedValueTypeConverter;
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
                        return string.Empty;
                    }
                }

                set
                {
                    if (owner is DataGridViewHeaderCell)
                    {
                        return;
                    }

                    if (owner.ReadOnly)
                    {
                        return;
                    }

                    if (owner.OwningRow == null)
                    {
                        return;
                    }

                    if (owner.DataGridView.IsCurrentCellInEditMode)
                    {
                        // EndEdit before setting the accessible object value.
                        // This way the value being edited is validated.
                        owner.DataGridView.EndEdit();
                    }

                    DataGridViewCellStyle dataGridViewCellStyle = owner.InheritedStyle;

                    // Format string "True" to boolean True.
                    object formattedValue = owner.GetFormattedValue(value,
                                                                         owner.OwningRow.Index,
                                                                         ref dataGridViewCellStyle,
                                                                         null /*formattedValueTypeConverter*/ ,
                                                                         null /*valueTypeConverter*/,
                                                                         DataGridViewDataErrorContexts.Formatting);
                    // Parse the formatted value and push it into the back end.
                    owner.Value = owner.ParseFormattedValue(formattedValue,
                                                                 dataGridViewCellStyle,
                                                                 null /*formattedValueTypeConverter*/,
                                                                 null /*valueTypeConverter*/);
                }
            }

            public override void DoDefaultAction()
            {
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                DataGridViewCell dataGridViewCell = (DataGridViewCell)Owner;
                DataGridView dataGridView = dataGridViewCell.DataGridView;

                if (dataGridViewCell is DataGridViewHeaderCell)
                {
                    return;
                }

                if (dataGridView != null && dataGridViewCell.RowIndex == -1)
                {
                    throw new InvalidOperationException(SR.DataGridView_InvalidOperationOnSharedCell);
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
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (owner.OwningColumn == null)
                {
                    return Rectangle.Empty;
                }

                Rectangle rowRect = parentAccObject.Bounds;
                Rectangle cellRect = rowRect;
                Rectangle columnRect = owner.DataGridView.RectangleToScreen(owner.DataGridView.GetColumnDisplayRectangle(owner.ColumnIndex, false /*cutOverflow*/));

                var cellRight = columnRect.Left + columnRect.Width;
                var cellLeft = columnRect.Left;

                int rightToLeftRowHeadersWidth = 0;
                int leftToRightRowHeadersWidth = 0;
                if (owner.DataGridView.RowHeadersVisible)
                {
                    if (owner.DataGridView.RightToLeft == RightToLeft.Yes)
                    {
                        rightToLeftRowHeadersWidth = owner.DataGridView.RowHeadersWidth;
                    }
                    else
                    {
                        leftToRightRowHeadersWidth = owner.DataGridView.RowHeadersWidth;
                    }
                }

                if (cellLeft < rowRect.Left + leftToRightRowHeadersWidth)
                {
                    cellLeft = rowRect.Left + leftToRightRowHeadersWidth;
                }
                cellRect.X = cellLeft;

                if (cellRight > rowRect.Right - rightToLeftRowHeadersWidth)
                {
                    cellRight = rowRect.Right - rightToLeftRowHeadersWidth;
                }

                if ((cellRight - cellLeft) >= 0)
                {
                    cellRect.Width = cellRight - cellLeft;
                }
                else
                {
                    cellRect.Width = 0;
                }

                return cellRect;
            }

            private AccessibleObject GetAccessibleObjectParent()
            {
                // If this is one of our types, use the shortcut provided by ParentPrivate property.
                // Otherwise, use the Parent property.
                if (owner is DataGridViewButtonCell ||
                    owner is DataGridViewCheckBoxCell ||
                    owner is DataGridViewComboBoxCell ||
                    owner is DataGridViewImageCell ||
                    owner is DataGridViewLinkCell ||
                    owner is DataGridViewTextBoxCell)
                {
                    return ParentPrivate;
                }
                else
                {
                    return Parent;
                }
            }

            public override AccessibleObject GetChild(int index)
            {
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (owner.DataGridView != null &&
                    owner.DataGridView.EditingControl != null &&
                    owner.DataGridView.IsCurrentCellInEditMode &&
                    owner.DataGridView.CurrentCell == owner &&
                    index == 0)
                {
                    return owner.DataGridView.EditingControl.AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            public override int GetChildCount()
            {
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (owner.DataGridView != null &&
                    owner.DataGridView.EditingControl != null &&
                    owner.DataGridView.IsCurrentCellInEditMode &&
                    owner.DataGridView.CurrentCell == owner)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            public override AccessibleObject GetFocused()
            {
                return null;
            }

            public override AccessibleObject GetSelected()
            {
                return null;
            }

            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (owner.OwningColumn == null || owner.OwningRow == null)
                {
                    return null;
                }

                switch (navigationDirection)
                {
                    case AccessibleNavigation.Right:
                        if (owner.DataGridView.RightToLeft == RightToLeft.No)
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
                        if (owner.DataGridView.RightToLeft == RightToLeft.No)
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
                        if (owner.OwningRow.Index == owner.DataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible))
                        {
                            if (owner.DataGridView.ColumnHeadersVisible)
                            {
                                // Return the column header accessible object.
                                return owner.OwningColumn.HeaderCell.AccessibilityObject;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            int previousVisibleRow = owner.DataGridView.Rows.GetPreviousRow(owner.OwningRow.Index, DataGridViewElementStates.Visible);
                            return owner.DataGridView.Rows[previousVisibleRow].Cells[owner.OwningColumn.Index].AccessibilityObject;
                        }
                    case AccessibleNavigation.Down:
                        if (owner.OwningRow.Index == owner.DataGridView.Rows.GetLastRow(DataGridViewElementStates.Visible))
                        {
                            return null;
                        }
                        else
                        {
                            int nextVisibleRow = owner.DataGridView.Rows.GetNextRow(owner.OwningRow.Index, DataGridViewElementStates.Visible);
                            return owner.DataGridView.Rows[nextVisibleRow].Cells[owner.OwningColumn.Index].AccessibilityObject;
                        }
                    default:
                        return null;
                }
            }

            private AccessibleObject NavigateBackward(bool wrapAround)
            {
                if (owner.OwningColumn == owner.DataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible))
                {
                    if (wrapAround)
                    {
                        // Return the last accessible object in the previous row
                        AccessibleObject previousRow = Owner.OwningRow.AccessibilityObject.Navigate(AccessibleNavigation.Previous);
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
                        if (owner.DataGridView.RowHeadersVisible)
                        {
                            return owner.OwningRow.AccessibilityObject.GetChild(0);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    int previousVisibleColumnIndex = owner.DataGridView.Columns.GetPreviousColumn(owner.OwningColumn,
                                                                                                       DataGridViewElementStates.Visible,
                                                                                                       DataGridViewElementStates.None).Index;
                    return owner.OwningRow.Cells[previousVisibleColumnIndex].AccessibilityObject;
                }
            }

            private AccessibleObject NavigateForward(bool wrapAround)
            {
                if (owner.OwningColumn == owner.DataGridView.Columns.GetLastColumn(DataGridViewElementStates.Visible,
                                                                                             DataGridViewElementStates.None))
                {

                    if (wrapAround)
                    {
                        // Return the first cell in the next visible row.
                        //
                        AccessibleObject nextRow = Owner.OwningRow.AccessibilityObject.Navigate(AccessibleNavigation.Next);
                        if (nextRow != null && nextRow.GetChildCount() > 0)
                        {
                            if (Owner.DataGridView.RowHeadersVisible)
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
                    int nextVisibleColumnIndex = owner.DataGridView.Columns.GetNextColumn(owner.OwningColumn,
                                                                                               DataGridViewElementStates.Visible,
                                                                                               DataGridViewElementStates.None).Index;
                    return owner.OwningRow.Cells[nextVisibleColumnIndex].AccessibilityObject;
                }
            }

            public override void Select(AccessibleSelection flags)
            {
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }
                if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
                {
                    owner.DataGridView?.Focus();
                }
                if ((flags & AccessibleSelection.TakeSelection) == AccessibleSelection.TakeSelection)
                {
                    owner.Selected = true;
                    if (owner.DataGridView != null)
                    {
                        owner.DataGridView.CurrentCell = owner; // Do not change old selection
                    }
                }
                if ((flags & AccessibleSelection.AddSelection) == AccessibleSelection.AddSelection)
                {
                    // it seems that in any circumstances a cell can become selected
                    owner.Selected = true;
                }
                if ((flags & AccessibleSelection.RemoveSelection) == AccessibleSelection.RemoveSelection &&
                    (flags & (AccessibleSelection.AddSelection | AccessibleSelection.TakeSelection)) == 0)
                {
                    owner.Selected = false;
                }
            }

            /// <summary>
            ///  Sets the detachable child accessible object which may be added or removed to/from hierachy nodes.
            /// </summary>
            /// <param name="child">The child accessible object.</param>
            internal override void SetDetachableChild(AccessibleObject child)
            {
                _child = child;
            }

            internal override void SetFocus()
            {
                base.SetFocus();

                RaiseAutomationEvent(NativeMethods.UIA_AutomationFocusChangedEventId);
            }

            internal override int[] RuntimeId
            {
                get
                {
                    if (runtimeId == null)
                    {
                        runtimeId = new int[2];
                        runtimeId[0] = RuntimeIDFirstItem; // first item is static - 0x2a
                        runtimeId[1] = GetHashCode();
                    }

                    return runtimeId;
                }
            }

            private string AutomationId
            {
                get
                {
                    string automationId = string.Empty;
                    foreach (int runtimeIdPart in RuntimeId)
                    {
                        automationId += runtimeIdPart.ToString();
                    }

                    return automationId;
                }
            }

            internal override bool IsIAccessibleExSupported() => true;

            #region IRawElementProviderFragment Implementation

            internal override Rectangle BoundingRectangle
            {
                get
                {
                    return Bounds;
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
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (owner.OwningColumn == null || owner.OwningRow == null)
                {
                    return null;
                }

                switch (direction)
                {
                    case UnsafeNativeMethods.NavigateDirection.Parent:
                        return owner.OwningRow.AccessibilityObject;
                    case UnsafeNativeMethods.NavigateDirection.NextSibling:
                        return NavigateForward(false);
                    case UnsafeNativeMethods.NavigateDirection.PreviousSibling:
                        return NavigateBackward(false);
                    case UnsafeNativeMethods.NavigateDirection.FirstChild:
                    case UnsafeNativeMethods.NavigateDirection.LastChild:
                        if (owner.DataGridView.CurrentCell == owner &&
                            owner.DataGridView.IsCurrentCellInEditMode &&
                            owner.DataGridView.EditingControl != null)
                        {
                            return _child;
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
                switch (propertyID)
                {
                    case NativeMethods.UIA_NamePropertyId:
                        return Name;
                    case NativeMethods.UIA_HasKeyboardFocusPropertyId:
                        return (State & AccessibleStates.Focused) == AccessibleStates.Focused; // Announce the cell when focusing.
                    case NativeMethods.UIA_IsEnabledPropertyId:
                        return owner.DataGridView.Enabled;
                    case NativeMethods.UIA_AutomationIdPropertyId:
                        return AutomationId;
                    case NativeMethods.UIA_HelpTextPropertyId:
                        return Help ?? string.Empty;
                    case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                        return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                    case NativeMethods.UIA_IsPasswordPropertyId:
                        return false;
                    case NativeMethods.UIA_IsOffscreenPropertyId:
                        return (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
                    case NativeMethods.UIA_AccessKeyPropertyId:
                        return string.Empty;
                    case NativeMethods.UIA_GridItemContainingGridPropertyId:
                        return Owner.DataGridView.AccessibilityObject;
                    case NativeMethods.UIA_IsTableItemPatternAvailablePropertyId:
                        return IsPatternSupported(NativeMethods.UIA_TableItemPatternId);
                    case NativeMethods.UIA_IsGridItemPatternAvailablePropertyId:
                        return IsPatternSupported(NativeMethods.UIA_GridItemPatternId);
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override bool IsPatternSupported(int patternId)
            {
                if (patternId == NativeMethods.UIA_LegacyIAccessiblePatternId ||
                    patternId == NativeMethods.UIA_InvokePatternId ||
                    patternId == NativeMethods.UIA_ValuePatternId)
                {
                    return true;
                }

                if ((patternId == NativeMethods.UIA_TableItemPatternId ||
                    patternId == NativeMethods.UIA_GridItemPatternId) &&
                    // We don't want to implement patterns for header cells
                    owner.ColumnIndex != -1 && owner.RowIndex != -1)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            #endregion

            internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetRowHeaderItems()
            {
                if (owner.DataGridView.RowHeadersVisible && owner.OwningRow.HasHeaderCell)
                {
                    return new UnsafeNativeMethods.IRawElementProviderSimple[1] { owner.OwningRow.HeaderCell.AccessibilityObject };
                }

                return null;
            }

            internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetColumnHeaderItems()
            {
                if (owner.DataGridView.ColumnHeadersVisible && owner.OwningColumn.HasHeaderCell)
                {
                    return new UnsafeNativeMethods.IRawElementProviderSimple[1] { owner.OwningColumn.HeaderCell.AccessibilityObject };
                }

                return null;
            }

            internal override int Row
            {
                get
                {
                    return owner.OwningRow != null ? owner.OwningRow.Index : -1;
                }
            }

            internal override int Column
            {
                get
                {
                    return owner.OwningColumn != null ? owner.OwningColumn.Index : -1;
                }
            }

            internal override UnsafeNativeMethods.IRawElementProviderSimple ContainingGrid
            {
                get
                {
                    return owner.DataGridView.AccessibilityObject;
                }
            }

            internal override bool IsReadOnly => owner.ReadOnly;
        }
    }
}

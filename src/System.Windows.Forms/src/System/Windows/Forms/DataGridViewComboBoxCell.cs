// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms.VisualStyles;
using static Interop;

namespace System.Windows.Forms
{
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

        private const byte IgnoreNextMouseClick = 0x01;
        private const byte CellSorted = 0x02;
        private const byte CellCreateItemsFromDataSource = 0x04;
        private const byte CellAutoComplete = 0x08;
        private const byte DataSourceInitializedHookedUp = 0x10;
        private const byte DropDownHookedUp = 0x20;

        internal const int DefaultMaxDropDownItems = 8;

        private static readonly Type s_defaultFormattedValueType = typeof(string);
        private static readonly Type s_defaultEditType = typeof(DataGridViewComboBoxEditingControl);
        private static readonly Type s_defaultValueType = typeof(object);
        private static readonly Type s_cellType = typeof(DataGridViewComboBoxCell);

        private byte _flags;  // see DATAGRIDVIEWCOMBOBOXCELL_ consts above
        private static bool s_mouseInDropDownButtonBounds;
        private static int s_cachedDropDownWidth = -1;

        // Autosizing changed for VS
        // We need to make ItemFromComboBoxDataSource as fast as possible because ItemFromComboBoxDataSource is getting called a lot
        // during AutoSize. To do that we keep a copy of the key and the value.
        //private object keyUsedDuringAutoSize    = null;
        //private object valueUsedDuringAutoSize  = null;

        private static bool s_isScalingInitialized;
        private const int Offset2Pixels = 2;
        private static int s_offset2X = Offset2Pixels;
        private static int s_offset2Y = Offset2Pixels;
        private static byte s_nonXPTriangleHeight = NonXPTriangleHeight;
        private static byte s_nonXPTriangleWidth = NonXPTriangleWidth;

        public DataGridViewComboBoxCell()
        {
            _flags = CellAutoComplete;
            if (!s_isScalingInitialized)
            {
                if (DpiHelper.IsScalingRequired)
                {
                    s_offset2X = DpiHelper.LogicalToDeviceUnitsX(Offset2Pixels);
                    s_offset2Y = DpiHelper.LogicalToDeviceUnitsY(Offset2Pixels);
                    s_nonXPTriangleWidth = (byte)DpiHelper.LogicalToDeviceUnitsX(NonXPTriangleWidth);
                    s_nonXPTriangleHeight = (byte)DpiHelper.LogicalToDeviceUnitsY(NonXPTriangleHeight);
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
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewComboBoxCellAccessibleObject(this);
        }

        [DefaultValue(true)]
        public virtual bool AutoComplete
        {
            get
            {
                return ((_flags & CellAutoComplete) != 0x00);
            }
            set
            {
                //CheckNoSharedCell();
                if (value != AutoComplete)
                {
                    if (value)
                    {
                        _flags |= (byte)CellAutoComplete;
                    }
                    else
                    {
                        _flags = (byte)(_flags & ~CellAutoComplete);
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
        }

        private CurrencyManager DataManager
        {
            get
            {
                return GetDataManager(DataGridView);
            }
            set
            {
                if (value != null || Properties.ContainsObject(s_propComboBoxCellDataManager))
                {
                    Properties.SetObject(s_propComboBoxCellDataManager, value);
                }
            }
        }

        public virtual object DataSource
        {
            get
            {
                return Properties.GetObject(s_propComboBoxCellDataSource);
            }
            set
            {
                //CheckNoSharedCell();
                // Same check as for ListControl's DataSource
                if (value != null && !(value is IList || value is IListSource))
                {
                    throw new ArgumentException(SR.BadDataSourceForComplexBinding);
                }
                if (DataSource != value)
                {
                    // Invalidate the currency manager
                    DataManager = null;

                    UnwireDataSource();

                    Properties.SetObject(s_propComboBoxCellDataSource, value);

                    WireDataSource(value);

                    // Invalidate existing Items collection
                    CreateItemsFromDataSource = true;
                    s_cachedDropDownWidth = -1;

                    try
                    {
                        InitializeDisplayMemberPropertyDescriptor(DisplayMember);
                    }
                    catch (Exception exception)
                    {
                        if (ClientUtils.IsCriticalException(exception))
                        {
                            throw;
                        }
                        Debug.Assert(DisplayMember != null && DisplayMember.Length > 0);
                        DisplayMemberInternal = null;
                    }

                    try
                    {
                        InitializeValueMemberPropertyDescriptor(ValueMember);
                    }
                    catch (Exception exception)
                    {
                        if (ClientUtils.IsCriticalException(exception))
                        {
                            throw;
                        }
                        Debug.Assert(ValueMember != null && ValueMember.Length > 0);
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
        }

        [DefaultValue("")]
        public virtual string DisplayMember
        {
            get
            {
                object displayMember = Properties.GetObject(s_propComboBoxCellDisplayMember);
                if (displayMember is null)
                {
                    return string.Empty;
                }
                else
                {
                    return (string)displayMember;
                }
            }
            set
            {
                //CheckNoSharedCell();
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

        private string DisplayMemberInternal
        {
            set
            {
                InitializeDisplayMemberPropertyDescriptor(value);
                if ((value != null && value.Length > 0) || Properties.ContainsObject(s_propComboBoxCellDisplayMember))
                {
                    Properties.SetObject(s_propComboBoxCellDisplayMember, value);
                }
            }
        }

        private PropertyDescriptor DisplayMemberProperty
        {
            get
            {
                return (PropertyDescriptor)Properties.GetObject(s_propComboBoxCellDisplayMemberProp);
            }
            set
            {
                if (value != null || Properties.ContainsObject(s_propComboBoxCellDisplayMemberProp))
                {
                    Properties.SetObject(s_propComboBoxCellDisplayMemberProp, value);
                }
            }
        }

        [DefaultValue(DataGridViewComboBoxDisplayStyle.DropDownButton)]
        public DataGridViewComboBoxDisplayStyle DisplayStyle
        {
            get
            {
                int displayStyle = Properties.GetInteger(s_propComboBoxCellDisplayStyle, out bool found);
                if (found)
                {
                    return (DataGridViewComboBoxDisplayStyle)displayStyle;
                }
                return DataGridViewComboBoxDisplayStyle.DropDownButton;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewComboBoxDisplayStyle.ComboBox, (int)DataGridViewComboBoxDisplayStyle.Nothing))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewComboBoxDisplayStyle));
                }
                if (value != DisplayStyle)
                {
                    Properties.SetInteger(s_propComboBoxCellDisplayStyle, (int)value);
                    if (DataGridView != null)
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
        }

        internal DataGridViewComboBoxDisplayStyle DisplayStyleInternal
        {
            set
            {
                Debug.Assert(value >= DataGridViewComboBoxDisplayStyle.ComboBox && value <= DataGridViewComboBoxDisplayStyle.Nothing);
                if (value != DisplayStyle)
                {
                    Properties.SetInteger(s_propComboBoxCellDisplayStyle, (int)value);
                }
            }
        }

        [DefaultValue(false)]
        public bool DisplayStyleForCurrentCellOnly
        {
            get
            {
                int displayStyleForCurrentCellOnly = Properties.GetInteger(s_propComboBoxCellDisplayStyleForCurrentCellOnly, out bool found);
                if (found)
                {
                    return displayStyleForCurrentCellOnly == 0 ? false : true;
                }
                return false;
            }
            set
            {
                if (value != DisplayStyleForCurrentCellOnly)
                {
                    Properties.SetInteger(s_propComboBoxCellDisplayStyleForCurrentCellOnly, value ? 1 : 0);
                    if (DataGridView != null)
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
        }

        internal bool DisplayStyleForCurrentCellOnlyInternal
        {
            set
            {
                if (value != DisplayStyleForCurrentCellOnly)
                {
                    Properties.SetInteger(s_propComboBoxCellDisplayStyleForCurrentCellOnly, value ? 1 : 0);
                }
            }
        }

        private Type DisplayType
        {
            get
            {
                if (DisplayMemberProperty != null)
                {
                    return DisplayMemberProperty.PropertyType;
                }
                else if (ValueMemberProperty != null)
                {
                    return ValueMemberProperty.PropertyType;
                }
                else
                {
                    return s_defaultFormattedValueType;
                }
            }
        }

        private TypeConverter DisplayTypeConverter
        {
            get
            {
                if (DataGridView != null)
                {
                    return DataGridView.GetCachedTypeConverter(DisplayType);
                }
                else
                {
                    return TypeDescriptor.GetConverter(DisplayType);
                }
            }
        }

        [DefaultValue(1)]
        public virtual int DropDownWidth
        {
            get
            {
                int dropDownWidth = Properties.GetInteger(s_propComboBoxCellDropDownWidth, out bool found);
                return found ? dropDownWidth : 1;
            }
            set
            {
                //CheckNoSharedCell();
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(DropDownWidth), value, string.Format(SR.DataGridViewComboBoxCell_DropDownWidthOutOfRange, 1));
                }
                Properties.SetInteger(s_propComboBoxCellDropDownWidth, (int)value);
                if (OwnsEditingComboBox(RowIndex))
                {
                    EditingComboBox.DropDownWidth = value;
                }
            }
        }

        private DataGridViewComboBoxEditingControl EditingComboBox
        {
            get
            {
                return (DataGridViewComboBoxEditingControl)Properties.GetObject(s_propComboBoxCellEditingComboBox);
            }
            set
            {
                if (value != null || Properties.ContainsObject(s_propComboBoxCellEditingComboBox))
                {
                    Properties.SetObject(s_propComboBoxCellEditingComboBox, value);
                }
            }
        }

        public override Type EditType
        {
            get
            {
                return s_defaultEditType;
            }
        }

        [DefaultValue(FlatStyle.Standard)]
        public FlatStyle FlatStyle
        {
            get
            {
                int flatStyle = Properties.GetInteger(s_propComboBoxCellFlatStyle, out bool found);
                if (found)
                {
                    return (FlatStyle)flatStyle;
                }
                return FlatStyle.Standard;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)FlatStyle.Flat, (int)FlatStyle.System))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FlatStyle));
                }
                if (value != FlatStyle)
                {
                    Properties.SetInteger(s_propComboBoxCellFlatStyle, (int)value);
                    OnCommonChange();
                }
            }
        }

        internal FlatStyle FlatStyleInternal
        {
            set
            {
                Debug.Assert(value >= FlatStyle.Flat && value <= FlatStyle.System);
                if (value != FlatStyle)
                {
                    Properties.SetInteger(s_propComboBoxCellFlatStyle, (int)value);
                }
            }
        }

        public override Type FormattedValueType
        {
            get
            {
                return s_defaultFormattedValueType;
            }
        }

        internal bool HasItems
        {
            get
            {
                return Properties.ContainsObject(s_propComboBoxCellItems) && Properties.GetObject(s_propComboBoxCellItems) != null;
            }
        }

        [Browsable(false)]
        public virtual ObjectCollection Items
        {
            get
            {
                return GetItems(DataGridView);
            }
        }

        [DefaultValue(DefaultMaxDropDownItems)]
        public virtual int MaxDropDownItems
        {
            get
            {
                int maxDropDownItems = Properties.GetInteger(s_propComboBoxCellMaxDropDownItems, out bool found);
                if (found)
                {
                    return maxDropDownItems;
                }
                return DefaultMaxDropDownItems;
            }
            set
            {
                //CheckNoSharedCell();
                if (value < 1 || value > 100)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaxDropDownItems), value, string.Format(SR.DataGridViewComboBoxCell_MaxDropDownItemsOutOfRange, 1, 100));
                }
                Properties.SetInteger(s_propComboBoxCellMaxDropDownItems, (int)value);
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
                bool paintFlat = FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup;
                return !paintFlat && DataGridView.ApplyVisualStylesToInnerCells;
            }
        }

        private static bool PostXPThemesExist
        {
            get
            {
                return VisualStyleRenderer.IsElementDefined(VisualStyleElement.ComboBox.ReadOnlyButton.Normal);
            }
        }

        [DefaultValue(false)]
        public virtual bool Sorted
        {
            get
            {
                return ((_flags & CellSorted) != 0x00);
            }
            set
            {
                //CheckNoSharedCell();
                if (value != Sorted)
                {
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
                        _flags |= (byte)CellSorted;
                    }
                    else
                    {
                        _flags = (byte)(_flags & ~CellSorted);
                    }
                    if (OwnsEditingComboBox(RowIndex))
                    {
                        EditingComboBox.Sorted = value;
                    }
                }
            }
        }

        internal DataGridViewComboBoxColumn TemplateComboBoxColumn
        {
            get
            {
                return (DataGridViewComboBoxColumn)Properties.GetObject(s_propComboBoxCellColumnTemplate);
            }
            set
            {
                Properties.SetObject(s_propComboBoxCellColumnTemplate, value);
            }
        }

        [DefaultValue("")]
        public virtual string ValueMember
        {
            get
            {
                object valueMember = Properties.GetObject(s_propComboBoxCellValueMember);
                if (valueMember is null)
                {
                    return string.Empty;
                }
                else
                {
                    return (string)valueMember;
                }
            }
            set
            {
                //CheckNoSharedCell();
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

        private string ValueMemberInternal
        {
            set
            {
                InitializeValueMemberPropertyDescriptor(value);
                if ((value != null && value.Length > 0) || Properties.ContainsObject(s_propComboBoxCellValueMember))
                {
                    Properties.SetObject(s_propComboBoxCellValueMember, value);
                }
            }
        }

        private PropertyDescriptor ValueMemberProperty
        {
            get
            {
                return (PropertyDescriptor)Properties.GetObject(s_propComboBoxCellValueMemberProp);
            }
            set
            {
                if (value != null || Properties.ContainsObject(s_propComboBoxCellValueMemberProp))
                {
                    Properties.SetObject(s_propComboBoxCellValueMemberProp, value);
                }
            }
        }

        public override Type ValueType
        {
            get
            {
                if (ValueMemberProperty != null)
                {
                    return ValueMemberProperty.PropertyType;
                }
                else if (DisplayMemberProperty != null)
                {
                    return DisplayMemberProperty.PropertyType;
                }
                else
                {
                    Type baseValueType = base.ValueType;
                    if (baseValueType != null)
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
            EditingComboBox = DataGridView.EditingControl as DataGridViewComboBoxEditingControl;
        }

        private void CheckDropDownList(int x, int y, int rowIndex)
        {
            Debug.Assert(EditingComboBox != null);
            DataGridViewAdvancedBorderStyle dgvabsPlaceholder = new DataGridViewAdvancedBorderStyle(), dgvabsEffective;
            dgvabsEffective = AdjustCellBorderStyle(DataGridView.AdvancedCellBorderStyle,
                dgvabsPlaceholder,
                false /*singleVerticalBorderAdded*/,
                false /*singleHorizontalBorderAdded*/,
                false /*isFirstDisplayedColumn*/,
                false /*isFirstDisplayedRow*/);
            DataGridViewCellStyle cellStyle = GetInheritedStyle(null, rowIndex, false /*includeColors*/);
            Rectangle borderAndPaddingWidths = BorderWidths(dgvabsEffective);
            borderAndPaddingWidths.X += cellStyle.Padding.Left;
            borderAndPaddingWidths.Y += cellStyle.Padding.Top;
            borderAndPaddingWidths.Width += cellStyle.Padding.Right;
            borderAndPaddingWidths.Height += cellStyle.Padding.Bottom;
            Size size = GetSize(rowIndex);
            Size adjustedSize = new Size(size.Width - borderAndPaddingWidths.X - borderAndPaddingWidths.Width,
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
            if (DataSource != null)
            {
                throw new ArgumentException(SR.DataSourceLocksItems);
            }
        }

        private void ComboBox_DropDown(object sender, EventArgs e)
        {
            Debug.Assert(DataGridView != null);
            Debug.Assert(EditingComboBox != null);

            ComboBox comboBox = EditingComboBox;
            if (OwningColumn is DataGridViewComboBoxColumn owningComboBoxColumn)
            {
                DataGridViewAutoSizeColumnMode autoSizeColumnMode = owningComboBoxColumn.GetInheritedAutoSizeMode(DataGridView);
                if (autoSizeColumnMode != DataGridViewAutoSizeColumnMode.ColumnHeader &&
                    autoSizeColumnMode != DataGridViewAutoSizeColumnMode.Fill &&
                    autoSizeColumnMode != DataGridViewAutoSizeColumnMode.None)
                {
                    if (DropDownWidth == 1)
                    {
                        // Owning combobox column is autosized based on inner cells.
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
                        User32.SendMessageW(comboBox, (User32.WM)User32.CB.SETDROPPEDWIDTH, (IntPtr)s_cachedDropDownWidth);
                    }
                }
                else
                {
                    // The dropdown width may have been previously adjusted to the items because of the owning column autosized.
                    // The dropdown width needs to be realigned to the DropDownWidth property value.
                    int dropDownWidth = unchecked((int)(long)User32.SendMessageW(comboBox, (User32.WM)User32.CB.GETDROPPEDWIDTH));
                    if (dropDownWidth != DropDownWidth)
                    {
                        User32.SendMessageW(comboBox, (User32.WM)User32.CB.SETDROPPEDWIDTH, (IntPtr)DropDownWidth);
                    }
                }
            }
        }

        public override object Clone()
        {
            DataGridViewComboBoxCell dataGridViewCell;
            Type thisType = GetType();

            if (thisType == s_cellType) //performance improvement
            {
                dataGridViewCell = new DataGridViewComboBoxCell();
            }
            else
            {
                //
                dataGridViewCell = (DataGridViewComboBoxCell)System.Activator.CreateInstance(thisType);
            }
            base.CloneInternal(dataGridViewCell);
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
            get
            {
                return ((_flags & CellCreateItemsFromDataSource) != 0x00);
            }
            set
            {
                if (value)
                {
                    _flags |= (byte)CellCreateItemsFromDataSource;
                }
                else
                {
                    _flags = (byte)(_flags & ~CellCreateItemsFromDataSource);
                }
            }
        }

        private void DataSource_Disposed(object sender, EventArgs e)
        {
            Debug.Assert(sender == DataSource, "How can we get dispose notification from anything other than our DataSource?");
            DataSource = null;
        }

        private void DataSource_Initialized(object sender, EventArgs e)
        {
            Debug.Assert(sender == DataSource);
            Debug.Assert(DataSource is ISupportInitializeNotification);
            Debug.Assert((_flags & DataSourceInitializedHookedUp) != 0x00);

            // Unhook the Initialized event.
            if (DataSource is ISupportInitializeNotification dsInit)
            {
                dsInit.Initialized -= new EventHandler(DataSource_Initialized);
            }

            // The wait is over: DataSource is initialized.
            _flags = (byte)(_flags & ~DataSourceInitializedHookedUp);

            // Check the DisplayMember and ValueMember values - will throw if values don't match existing fields.
            InitializeDisplayMemberPropertyDescriptor(DisplayMember);
            InitializeValueMemberPropertyDescriptor(ValueMember);
        }

        public override void DetachEditingControl()
        {
            DataGridView dgv = DataGridView;
            if (dgv is null || dgv.EditingControl is null)
            {
                throw new InvalidOperationException();
            }

            if (EditingComboBox != null &&
                (_flags & DropDownHookedUp) != 0x00)
            {
                EditingComboBox.DropDown -= new EventHandler(ComboBox_DropDown);
                _flags = (byte)(_flags & ~DropDownHookedUp);
            }

            EditingComboBox = null;
            base.DetachEditingControl();
        }

        protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle is null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (DataGridView is null || rowIndex < 0 || OwningColumn is null)
            {
                return Rectangle.Empty;
            }

            object value = GetValue(rowIndex);
            object formattedValue = GetEditedFormattedValue(value, rowIndex, ref cellStyle, DataGridViewDataErrorContexts.Formatting);

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out DataGridViewAdvancedBorderStyle dgvabsEffective, out DataGridViewElementStates cellState, out Rectangle cellBounds);

            Rectangle contentBounds = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                formattedValue,
                null /*errorText*/,             // contentBounds is independent of errorText
                cellStyle,
                dgvabsEffective,
                out Rectangle dropDownButtonRect,         // not used
                DataGridViewPaintParts.ContentForeground,
                true  /*computeContentBounds*/,
                false /*computeErrorIconBounds*/,
                false /*computeDropDownButtonRect*/,
                false /*paint*/);

#if DEBUG
            Rectangle contentBoundsDebug = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                formattedValue,
                GetErrorText(rowIndex),
                cellStyle,
                dgvabsEffective,
                out dropDownButtonRect,         // not used
                DataGridViewPaintParts.ContentForeground,
                true  /*computeContentBounds*/,
                false /*computeErrorIconBounds*/,
                false /*computeDropDownButtonRect*/,
                false /*paint*/);
            Debug.Assert(contentBoundsDebug.Equals(contentBounds));
#endif

            return contentBounds;
        }

        private CurrencyManager GetDataManager(DataGridView dataGridView)
        {
            CurrencyManager cm = (CurrencyManager)Properties.GetObject(s_propComboBoxCellDataManager);
            if (cm is null && DataSource != null && dataGridView != null && dataGridView.BindingContext != null && !(DataSource == Convert.DBNull))
            {
                if (DataSource is ISupportInitializeNotification dsInit && !dsInit.IsInitialized)
                {
                    if ((_flags & DataSourceInitializedHookedUp) == 0x00)
                    {
                        dsInit.Initialized += new EventHandler(DataSource_Initialized);
                        _flags |= (byte)DataSourceInitializedHookedUp;
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

        private protected override string GetDefaultToolTipText()
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
                if (PostXPThemesExist)
                {
                    adjustment = 8;
                }
                else
                {
                    adjustment = 6;
                }
            }
            return DataGridViewCell.MeasureTextHeight(graphics, " ", cellStyle.Font, int.MaxValue, TextFormatFlags.Default) + adjustment;
        }

        protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle is null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (DataGridView is null ||
                rowIndex < 0 ||
                OwningColumn is null ||
                !DataGridView.ShowCellErrors ||
                string.IsNullOrEmpty(GetErrorText(rowIndex)))
            {
                return Rectangle.Empty;
            }

            object value = GetValue(rowIndex);
            object formattedValue = GetEditedFormattedValue(value, rowIndex, ref cellStyle, DataGridViewDataErrorContexts.Formatting);

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out DataGridViewAdvancedBorderStyle dgvabsEffective, out DataGridViewElementStates cellState, out Rectangle cellBounds);

            Rectangle errorIconBounds = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                formattedValue,
                GetErrorText(rowIndex),
                cellStyle,
                dgvabsEffective,
                out Rectangle dropDownButtonRect,         // not used
                DataGridViewPaintParts.ContentForeground,
                false /*computeContentBounds*/,
                true  /*computeErrorBounds*/,
                false /*computeDropDownButtonRect*/,
                false /*paint*/);

#if DEBUG
            Rectangle errorIconBoundsDebug = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                formattedValue,
                GetErrorText(rowIndex),
                cellStyle,
                dgvabsEffective,
                out dropDownButtonRect,         // not used
                DataGridViewPaintParts.ContentForeground,
                false /*computeContentBounds*/,
                true  /*computeErrorBounds*/,
                false /*computeDropDownButtonRect*/,
                false /*paint*/);
            Debug.Assert(errorIconBoundsDebug.Equals(errorIconBounds));
#endif

            return errorIconBounds;
        }

        protected override object GetFormattedValue(object value,
                                                    int rowIndex,
                                                    ref DataGridViewCellStyle cellStyle,
                                                    TypeConverter valueTypeConverter,
                                                    TypeConverter formattedValueTypeConverter,
                                                    DataGridViewDataErrorContexts context)
        {
            if (valueTypeConverter is null)
            {
                if (ValueMemberProperty != null)
                {
                    valueTypeConverter = ValueMemberProperty.Converter;
                }
                else if (DisplayMemberProperty != null)
                {
                    valueTypeConverter = DisplayMemberProperty.Converter;
                }
            }

            if (value is null || ((ValueType != null && !ValueType.IsAssignableFrom(value.GetType())) && value != System.DBNull.Value))
            {
                // Do not raise the DataError event if the value is null and the row is the 'new row'.

                if (value is null)
                {
                    return base.GetFormattedValue(null, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
                }
                if (DataGridView != null)
                {
                    DataGridViewDataErrorEventArgs dgvdee = new DataGridViewDataErrorEventArgs(
                        new FormatException(SR.DataGridViewComboBoxCell_InvalidValue), ColumnIndex,
                        rowIndex, context);
                    RaiseDataError(dgvdee);
                    if (dgvdee.ThrowException)
                    {
                        throw dgvdee.Exception;
                    }
                }
                return base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
            }

            string strValue = value as string;
            if ((DataManager != null && (ValueMemberProperty != null || DisplayMemberProperty != null)) ||
                !string.IsNullOrEmpty(ValueMember) || !string.IsNullOrEmpty(DisplayMember))
            {
                if (!LookupDisplayValue(rowIndex, value, out object displayValue))
                {
                    if (value == System.DBNull.Value)
                    {
                        displayValue = System.DBNull.Value;
                    }
                    else if (strValue != null && string.IsNullOrEmpty(strValue) && DisplayType == typeof(string))
                    {
                        displayValue = string.Empty;
                    }
                    else if (DataGridView != null)
                    {
                        DataGridViewDataErrorEventArgs dgvdee = new DataGridViewDataErrorEventArgs(
                            new ArgumentException(SR.DataGridViewComboBoxCell_InvalidValue), ColumnIndex,
                            rowIndex, context);
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
                return base.GetFormattedValue(displayValue, rowIndex, ref cellStyle, DisplayTypeConverter, formattedValueTypeConverter, context);
            }
            else
            {
                if (!Items.Contains(value) &&
                    value != System.DBNull.Value &&
                    (!(value is string) || !string.IsNullOrEmpty(strValue)))
                {
                    if (DataGridView != null)
                    {
                        DataGridViewDataErrorEventArgs dgvdee = new DataGridViewDataErrorEventArgs(
                            new ArgumentException(SR.DataGridViewComboBoxCell_InvalidValue), ColumnIndex,
                            rowIndex, context);
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
                return base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
            }
        }

        internal string GetItemDisplayText(object item)
        {
            object displayValue = GetItemDisplayValue(item);
            return (displayValue != null) ? Convert.ToString(displayValue, CultureInfo.CurrentCulture) : string.Empty;
        }

        internal object GetItemDisplayValue(object item)
        {
            Debug.Assert(item != null);
            bool displayValueSet = false;
            object displayValue = null;
            if (DisplayMemberProperty != null)
            {
                displayValue = DisplayMemberProperty.GetValue(item);
                displayValueSet = true;
            }
            else if (ValueMemberProperty != null)
            {
                displayValue = ValueMemberProperty.GetValue(item);
                displayValueSet = true;
            }
            else if (!string.IsNullOrEmpty(DisplayMember))
            {
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(item).Find(DisplayMember, true /*caseInsensitive*/);
                if (propDesc != null)
                {
                    displayValue = propDesc.GetValue(item);
                    displayValueSet = true;
                }
            }
            else if (!string.IsNullOrEmpty(ValueMember))
            {
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(item).Find(ValueMember, true /*caseInsensitive*/);
                if (propDesc != null)
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

        internal ObjectCollection GetItems(DataGridView dataGridView)
        {
            ObjectCollection items = (ObjectCollection)Properties.GetObject(s_propComboBoxCellItems);
            if (items is null)
            {
                items = new ObjectCollection(this);
                Properties.SetObject(s_propComboBoxCellItems, items);
            }
            if (CreateItemsFromDataSource)
            {
                items.ClearInternal();
                CurrencyManager dataManager = GetDataManager(dataGridView);
                if (dataManager != null && dataManager.Count != -1)
                {
                    object[] newItems = new object[dataManager.Count];
                    for (int i = 0; i < newItems.Length; i++)
                    {
                        newItems[i] = dataManager[i];
                    }
                    items.AddRangeInternal(newItems);
                }
                // Do not clear the CreateItemsFromDataSource flag when the data source has not been initialized yet
                if (dataManager != null || (_flags & DataSourceInitializedHookedUp) == 0x00)
                {
                    CreateItemsFromDataSource = false;
                }
            }
            return items;
        }

        internal object GetItemValue(object item)
        {
            bool valueSet = false;
            object value = null;
            if (ValueMemberProperty != null)
            {
                value = ValueMemberProperty.GetValue(item);
                valueSet = true;
            }
            else if (DisplayMemberProperty != null)
            {
                value = DisplayMemberProperty.GetValue(item);
                valueSet = true;
            }
            else if (!string.IsNullOrEmpty(ValueMember))
            {
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(item).Find(ValueMember, true /*caseInsensitive*/);
                if (propDesc != null)
                {
                    value = propDesc.GetValue(item);
                    valueSet = true;
                }
            }
            if (!valueSet && !string.IsNullOrEmpty(DisplayMember))
            {
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(item).Find(DisplayMember, true /*caseInsensitive*/);
                if (propDesc != null)
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

        protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
        {
            if (DataGridView is null)
            {
                return new Size(-1, -1);
            }

            if (cellStyle is null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            Size preferredSize = Size.Empty;
            DataGridViewFreeDimension freeDimension = DataGridViewCell.GetFreeDimensionFromConstraint(constraintSize);
            Rectangle borderWidthsRect = StdBorderWidths;
            int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
            int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;
            TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);

            string formattedValue = GetFormattedValue(rowIndex, ref cellStyle, DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.PreferredSize) as string;
            if (!string.IsNullOrEmpty(formattedValue))
            {
                preferredSize = DataGridViewCell.MeasureTextSize(graphics, formattedValue, cellStyle.Font, flags);
            }
            else
            {
                preferredSize = DataGridViewCell.MeasureTextSize(graphics, " ", cellStyle.Font, flags);
            }

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
                preferredSize.Width += SystemInformation.HorizontalScrollBarThumbWidth + 1 + 2 * Margin + borderAndPaddingWidths;
                if (DataGridView.ShowCellErrors)
                {
                    // Making sure that there is enough room for the potential error icon
                    preferredSize.Width = Math.Max(preferredSize.Width, borderAndPaddingWidths + SystemInformation.HorizontalScrollBarThumbWidth + 1 + IconMarginWidth * 2 + s_iconsWidth);
                }
            }
            if (freeDimension != DataGridViewFreeDimension.Width)
            {
                if (FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup)
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
            Debug.Assert(EditingComboBox != null);
            ((IDataGridViewEditingControl)EditingComboBox).EditingControlValueChanged = false;
            int rowIndex = ((IDataGridViewEditingControl)EditingComboBox).EditingControlRowIndex;
            Debug.Assert(rowIndex > -1);
            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false);
            EditingComboBox.Text = (string)GetFormattedValue(GetValue(rowIndex), rowIndex, ref dataGridViewCellStyle, null, null, DataGridViewDataErrorContexts.Formatting);
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            Debug.Assert(DataGridView != null &&
                         DataGridView.EditingPanel != null &&
                         DataGridView.EditingControl != null);
            Debug.Assert(!ReadOnly);
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            if (DataGridView.EditingControl is ComboBox comboBox)
            {
                // Use the selection backcolor for the editing panel when the cell is selected
                if ((GetInheritedState(rowIndex) & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
                {
                    DataGridView.EditingPanel.BackColor = dataGridViewCellStyle.SelectionBackColor;
                }

                // We need the comboBox to be parented by a control which has a handle or else the native ComboBox ends up
                // w/ its parentHwnd pointing to the WinFormsParkingWindow.
                IntPtr h;
                if (comboBox.ParentInternal != null)
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
                if (this.DataManager != null && this.DataManager.Count > 0)
                {
                    this.DataManager.Position = 0;
                }
                */

                comboBox.DataSource = DataSource;
                comboBox.DisplayMember = DisplayMember;
                comboBox.ValueMember = ValueMember;
                if (HasItems && DataSource is null && Items.Count > 0)
                {
                    comboBox.Items.AddRange(Items.InnerArray.ToArray());
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

                if (!(initialFormattedValue is string initialFormattedValueStr))
                {
                    initialFormattedValueStr = string.Empty;
                }
                comboBox.Text = initialFormattedValueStr;

                if ((_flags & DropDownHookedUp) == 0x00)
                {
                    comboBox.DropDown += new EventHandler(ComboBox_DropDown);
                    _flags |= (byte)DropDownHookedUp;
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

        private void InitializeDisplayMemberPropertyDescriptor(string displayMember)
        {
            if (DataManager != null)
            {
                if (string.IsNullOrEmpty(displayMember))
                {
                    DisplayMemberProperty = null;
                }
                else
                {
                    BindingMemberInfo displayBindingMember = new BindingMemberInfo(displayMember);
                    // make the DataManager point to the sublist inside this.DataSource
                    DataManager = DataGridView.BindingContext[DataSource, displayBindingMember.BindingPath] as CurrencyManager;

                    PropertyDescriptorCollection props = DataManager.GetItemProperties();
                    PropertyDescriptor displayMemberProperty = props.Find(displayBindingMember.BindingField, true);
                    if (displayMemberProperty is null)
                    {
                        throw new ArgumentException(string.Format(SR.DataGridViewComboBoxCell_FieldNotFound, displayMember));
                    }
                    else
                    {
                        DisplayMemberProperty = displayMemberProperty;
                    }
                }
            }
        }

        private void InitializeValueMemberPropertyDescriptor(string valueMember)
        {
            if (DataManager != null)
            {
                if (string.IsNullOrEmpty(valueMember))
                {
                    ValueMemberProperty = null;
                }
                else
                {
                    BindingMemberInfo valueBindingMember = new BindingMemberInfo(valueMember);
                    // make the DataManager point to the sublist inside this.DataSource
                    DataManager = DataGridView.BindingContext[DataSource, valueBindingMember.BindingPath] as CurrencyManager;

                    PropertyDescriptorCollection props = DataManager.GetItemProperties();
                    PropertyDescriptor valueMemberProperty = props.Find(valueBindingMember.BindingField, true);
                    if (valueMemberProperty is null)
                    {
                        throw new ArgumentException(string.Format(SR.DataGridViewComboBoxCell_FieldNotFound, valueMember));
                    }
                    else
                    {
                        ValueMemberProperty = valueMemberProperty;
                    }
                }
            }
        }

        /// <summary>
        ///  Find the item in the ComboBox currency manager for the current cell
        ///  This can be horribly inefficient and it uses reflection which makes it expensive
        ///  - ripe for optimization
        /// </summary>
        private object ItemFromComboBoxDataSource(PropertyDescriptor property, object key)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            //if (key == this.keyUsedDuringAutoSize)
            //{
            //    return this.valueUsedDuringAutoSize;
            //}

            Debug.Assert(property != null);
            Debug.Assert(DataManager != null);
            object item = null;

            //If the data source is a bindinglist use that as it's probably more efficient
            if ((DataManager.List is IBindingList) && ((IBindingList)DataManager.List).SupportsSearching)
            {
                int index = ((IBindingList)DataManager.List).Find(property, key);
                if (index != -1)
                {
                    item = DataManager.List[index];
                }
            }
            else
            {
                //Otherwise walk across the items looking for the item we want
                for (int i = 0; i < DataManager.List.Count; i++)
                {
                    object itemTmp = DataManager.List[i];
                    object value = property.GetValue(itemTmp);
                    if (key.Equals(value))
                    {
                        item = itemTmp;
                        break;
                    }
                }
            }
            return item;
        }

        private object ItemFromComboBoxItems(int rowIndex, string field, object key)
        {
            Debug.Assert(!string.IsNullOrEmpty(field));

            object item = null;
            if (OwnsEditingComboBox(rowIndex))
            {
                // It is likely that the item looked for is the selected item.
                item = EditingComboBox.SelectedItem;
                object displayValue = null;
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(item).Find(field, true /*caseInsensitive*/);
                if (propDesc != null)
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
                    object displayValue = null;
                    PropertyDescriptor propDesc = TypeDescriptor.GetProperties(itemCandidate).Find(field, true /*caseInsensitive*/);
                    if (propDesc != null)
                    {
                        displayValue = propDesc.GetValue(itemCandidate);
                    }
                    if (displayValue != null && displayValue.Equals(key))
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
        private bool LookupDisplayValue(int rowIndex, object value, out object displayValue)
        {
            Debug.Assert(value != null);
            Debug.Assert(ValueMemberProperty != null || DisplayMemberProperty != null ||
                         !string.IsNullOrEmpty(ValueMember) || !string.IsNullOrEmpty(DisplayMember));

            object item = null;
            if (DisplayMemberProperty != null || ValueMemberProperty != null)
            {
                //Now look up the item in the Combobox datasource - this can be horribly inefficient
                //and it uses reflection which makes it expensive - ripe for optimization
                item = ItemFromComboBoxDataSource(ValueMemberProperty ?? DisplayMemberProperty, value);
            }
            else
            {
                //Find the item in the Items collection based on the provided ValueMember or DisplayMember
                item = ItemFromComboBoxItems(rowIndex, string.IsNullOrEmpty(ValueMember) ? DisplayMember : ValueMember, value);
            }
            if (item is null)
            {
                displayValue = null;
                return false;
            }

            //Now we've got the item for the value - we can get the display text using the DisplayMember

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
        private bool LookupValue(object formattedValue, out object value)
        {
            if (formattedValue is null)
            {
                value = null;
                return true;
            }

            Debug.Assert(DisplayMemberProperty != null || ValueMemberProperty != null ||
                         !string.IsNullOrEmpty(DisplayMember) || !string.IsNullOrEmpty(ValueMember));

            object item = null;
            if (DisplayMemberProperty != null || ValueMemberProperty != null)
            {
                //Now look up the item in the DataGridViewComboboxCell datasource - this can be horribly inefficient
                //and it uses reflection which makes it expensive - ripe for optimization
                item = ItemFromComboBoxDataSource(DisplayMemberProperty ?? ValueMemberProperty, formattedValue);
            }
            else
            {
                //Find the item in the Items collection based on the provided DisplayMember or ValueMember
                item = ItemFromComboBoxItems(RowIndex, string.IsNullOrEmpty(DisplayMember) ? ValueMember : DisplayMember, formattedValue);
            }
            if (item is null)
            {
                value = null;
                return false;
            }

            //Now we've got the item for the value - we can get the value using the ValueMember
            value = GetItemValue(item);
            return true;
        }

        protected override void OnDataGridViewChanged()
        {
            if (DataGridView != null)
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
                _flags |= (byte)IgnoreNextMouseClick;
            }
        }

        private void OnItemsCollectionChanged()
        {
            if (TemplateComboBoxColumn != null)
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
            _flags = (byte)(_flags & ~IgnoreNextMouseClick);
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
                if ((_flags & IgnoreNextMouseClick) != 0x00)
                {
                    _flags = (byte)(_flags & ~IgnoreNextMouseClick);
                }
                else if ((EditingComboBox is null || !EditingComboBox.DroppedDown) &&
                         DataGridView.EditMode != DataGridViewEditMode.EditProgrammatically &&
                         DataGridView.BeginEdit(true /*selectAll*/))
                {
                    if (EditingComboBox != null && DisplayStyle != DataGridViewComboBoxDisplayStyle.Nothing)
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
                int rowIndex = e.RowIndex;
                DataGridViewCellStyle cellStyle = GetInheritedStyle(null, rowIndex, false /*includeColors*/);

                // get the border style
                bool singleVerticalBorderAdded = !DataGridView.RowHeadersVisible && DataGridView.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single;
                bool singleHorizontalBorderAdded = !DataGridView.ColumnHeadersVisible && DataGridView.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single;
                bool isFirstDisplayedRow = rowIndex == DataGridView.FirstDisplayedScrollingRowIndex;
                bool isFirstDisplayedColumn = OwningColumn.Index == DataGridView.FirstDisplayedColumnIndex;
                bool isFirstDisplayedScrollingColumn = OwningColumn.Index == DataGridView.FirstDisplayedScrollingColumnIndex;
                DataGridViewAdvancedBorderStyle dgvabsEffective, dgvabsPlaceholder;
                dgvabsPlaceholder = new DataGridViewAdvancedBorderStyle();
                dgvabsEffective = AdjustCellBorderStyle(DataGridView.AdvancedCellBorderStyle, dgvabsPlaceholder,
                                                        singleVerticalBorderAdded,
                                                        singleHorizontalBorderAdded,
                                                        isFirstDisplayedRow,
                                                        isFirstDisplayedColumn);

                Rectangle cellBounds = DataGridView.GetCellDisplayRectangle(OwningColumn.Index, rowIndex, false /*cutOverflow*/);
                Rectangle cutoffCellBounds = cellBounds;
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
                    PaintPrivate(screen,
                        cellBounds,
                        cellBounds,
                        rowIndex,
                        cellState,
                        formattedValue: null,            // dropDownButtonRect is independent of formattedValue
                        errorText: null,                 // dropDownButtonRect is independent of errorText
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

        private bool OwnsEditingComboBox(int rowIndex)
        {
            return rowIndex != -1 && EditingComboBox != null && rowIndex == ((IDataGridViewEditingControl)EditingComboBox).EditingControlRowIndex;
        }

        protected override void Paint(Graphics graphics,
            Rectangle clipBounds,
            Rectangle cellBounds,
            int rowIndex,
            DataGridViewElementStates elementState,
            object value,
            object formattedValue,
            string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            if (cellStyle is null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            PaintPrivate(graphics,
                clipBounds,
                cellBounds,
                rowIndex,
                elementState,
                formattedValue,
                errorText,
                cellStyle,
                advancedBorderStyle,
                out Rectangle dropDownButtonRect,     // not used
                paintParts,
                false /*computeContentBounds*/,
                false /*computeErrorIconBounds*/,
                false /*computeDropDownButtonRect*/,
                true  /*paint*/);
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
        private Rectangle PaintPrivate(Graphics g,
            Rectangle clipBounds,
            Rectangle cellBounds,
            int rowIndex,
            DataGridViewElementStates elementState,
            object formattedValue,
            string errorText,
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
            Debug.Assert(cellStyle != null);

            Rectangle resultBounds = Rectangle.Empty;
            dropDownButtonRect = Rectangle.Empty;

            bool paintFlat = FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup;
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

            if (paint && DataGridViewCell.PaintBorder(paintParts))
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
            bool cellEdited = cellCurrent && DataGridView.EditingControl != null;
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
                PaintPadding(g, valBounds, cellStyle, brush, DataGridView.RightToLeftInternal);
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
                        g.FillRectangle(brush, valBounds.Left, valBounds.Top, valBounds.Width, valBounds.Height);
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
                        g.FillRectangle(brush, valBounds.Left + 1, valBounds.Top + 1, valBounds.Width - 2, valBounds.Height - 2);
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
                        g.FillRectangle(brush, valBounds.Left, valBounds.Top, valBounds.Width, valBounds.Height);
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

                            // the bounds around the combobox control
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

                            // the bounds around the combobox control
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
                                Point middle = new Point(dropRect.Left + dropRect.Width / 2, dropRect.Top + dropRect.Height / 2);
                                // if the width is odd - favor pushing it over one pixel right.
                                middle.X += (dropRect.Width % 2);
                                // if the height is odd - favor pushing it over one pixel down.
                                middle.Y += (dropRect.Height % 2);

                                g.FillPolygon(SystemBrushes.ControlText, new Point[]
                                {
                                    new Point(middle.X - s_offset2X, middle.Y - 1),
                                    new Point(middle.X + s_offset2X + 1, middle.Y - 1),
                                    new Point(middle.X, middle.Y + s_offset2Y)
                                });
                            }
                            else if (!paintXPThemes)
                            {
                                // XPThemes already painted the drop down button

                                // the down arrow looks better when it's fatten up by a pixel
                                dropRect.X--;
                                dropRect.Width++;

                                Point middle = new Point(dropRect.Left + (dropRect.Width - 1) / 2,
                                        dropRect.Top + (dropRect.Height + s_nonXPTriangleHeight) / 2);
                                // if the width is event - favor pushing it over one pixel right.
                                middle.X += ((dropRect.Width + 1) % 2);
                                // if the height is odd - favor pushing it over one pixel down.
                                middle.Y += (dropRect.Height % 2);
                                Point pt1 = new Point(middle.X - (s_nonXPTriangleWidth - 1) / 2, middle.Y - s_nonXPTriangleHeight);
                                Point pt2 = new Point(middle.X + (s_nonXPTriangleWidth - 1) / 2, middle.Y - s_nonXPTriangleHeight);
                                g.FillPolygon(SystemBrushes.ControlText, new Point[] { pt1, pt2, middle });
                                // quirk in GDI+ : if we dont draw the line below then the top right most pixel of the DropDown triangle will not paint
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
                        ControlPaint.DrawFocusRectangle(g, focusBounds, Color.Empty, brushColor);
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
                            ControlPaint.DrawFocusRectangle(g, focusBounds, Color.Empty, brushColor);
                        }
                    }
                    else
                    {
                        ControlPaint.DrawFocusRectangle(g, textBounds, Color.Empty, brushColor);
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
                                if (paintPostXPThemes && (drawDropDownButton || drawComboBox))
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

        public override object ParseFormattedValue(
            object formattedValue,
            DataGridViewCellStyle cellStyle,
            TypeConverter formattedValueTypeConverter,
            TypeConverter valueTypeConverter)
        {
            if (valueTypeConverter is null)
            {
                if (ValueMemberProperty != null)
                {
                    valueTypeConverter = ValueMemberProperty.Converter;
                }
                else if (DisplayMemberProperty != null)
                {
                    valueTypeConverter = DisplayMemberProperty.Converter;
                }
            }

            // Find the item given its display value
            if ((DataManager != null &&
                (DisplayMemberProperty != null || ValueMemberProperty != null)) ||
                !string.IsNullOrEmpty(DisplayMember) || !string.IsNullOrEmpty(ValueMember))
            {
                object value = ParseFormattedValueInternal(DisplayType, formattedValue, cellStyle,
                                                           formattedValueTypeConverter, DisplayTypeConverter);
                object originalValue = value;
                if (!LookupValue(originalValue, out value))
                {
                    if (originalValue == System.DBNull.Value)
                    {
                        value = System.DBNull.Value;
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
                return ParseFormattedValueInternal(ValueType, formattedValue, cellStyle,
                                                   formattedValueTypeConverter, valueTypeConverter);
            }
        }

        /// <summary>
        ///  Gets the row Index and column Index of the cell.
        /// </summary>
        public override string ToString()
        {
            return "DataGridViewComboBoxCell { ColumnIndex=" + ColumnIndex.ToString(CultureInfo.CurrentCulture) + ", RowIndex=" + RowIndex.ToString(CultureInfo.CurrentCulture) + " }";
        }

        private void UnwireDataSource()
        {
            if (DataSource is IComponent component)
            {
                component.Disposed -= new EventHandler(DataSource_Disposed);
            }

            if (DataSource is ISupportInitializeNotification dsInit && (_flags & DataSourceInitializedHookedUp) != 0x00)
            {
                // If we previously hooked the data source's ISupportInitializeNotification
                // Initialized event, then unhook it now (we don't always hook this event,
                // only if we needed to because the data source was previously uninitialized)
                dsInit.Initialized -= new EventHandler(DataSource_Initialized);
                _flags = (byte)(_flags & ~DataSourceInitializedHookedUp);
            }
        }

        private void WireDataSource(object dataSource)
        {
            // If the source is a component, then hook the Disposed event,
            // so we know when the component is deleted from the form
            if (dataSource is IComponent component)
            {
                component.Disposed += new EventHandler(DataSource_Disposed);
            }
        }

        /// <summary>
            ///  A collection that stores objects.
        /// </summary>
        [ListBindable(false)]
        public class ObjectCollection : IList
        {
            private readonly DataGridViewComboBoxCell owner;
            private ArrayList items;
            private IComparer comparer;

            public ObjectCollection(DataGridViewComboBoxCell owner)
            {
                Debug.Assert(owner != null);
                this.owner = owner;
            }

            private IComparer Comparer
            {
                get
                {
                    if (comparer is null)
                    {
                        comparer = new ItemComparer(owner);
                    }
                    return comparer;
                }
            }

            /// <summary>
            ///  Retrieves the number of items.
            /// </summary>
            public int Count
            {
                get
                {
                    return InnerArray.Count;
                }
            }

            /// <summary>
            ///  Internal access to the actual data store.
            /// </summary>
            internal ArrayList InnerArray
            {
                get
                {
                    if (items is null)
                    {
                        items = new ArrayList();
                    }
                    return items;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            bool IList.IsFixedSize
            {
                get
                {
                    return false;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            ///  Adds an item to the collection. For an unsorted combo box, the item is
            ///  added to the end of the existing list of items. For a sorted combo box,
            ///  the item is inserted into the list according to its sorted position.
            ///  The item's ToString() method is called to obtain the string that is
            ///  displayed in the combo box.
            /// </summary>
            public int Add(object item)
            {
                //this.owner.CheckNoSharedCell();
                owner.CheckNoDataSource();

                if (item is null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                int index = InnerArray.Add(item);

                bool success = false;
                if (owner.Sorted)
                {
                    try
                    {
                        InnerArray.Sort(Comparer);
                        index = InnerArray.IndexOf(item);
                        success = true;
                    }
                    finally
                    {
                        if (!success)
                        {
                            InnerArray.Remove(item);
                        }
                    }
                }

                owner.OnItemsCollectionChanged();
                return index;
            }

            int IList.Add(object item)
            {
                return Add(item);
            }

            public void AddRange(params object[] items)
            {
                //this.owner.CheckNoSharedCell();
                owner.CheckNoDataSource();
                AddRangeInternal((ICollection)items);
                owner.OnItemsCollectionChanged();
            }

            public void AddRange(ObjectCollection value)
            {
                //this.owner.CheckNoSharedCell();
                owner.CheckNoDataSource();
                AddRangeInternal((ICollection)value);
                owner.OnItemsCollectionChanged();
            }

            /// <summary>
            ///  Add range that bypasses the data source check.
            /// </summary>
            internal void AddRangeInternal(ICollection items)
            {
                if (items is null)
                {
                    throw new ArgumentNullException(nameof(items));
                }

                foreach (object item in items)
                {
                    if (item is null)
                    {
                        throw new InvalidOperationException(SR.InvalidNullItemInCollection);
                    }
                }

                // Add everything to the collection first, then sort
                InnerArray.AddRange(items);
                if (owner.Sorted)
                {
                    InnerArray.Sort(Comparer);
                }
            }

            internal void SortInternal()
            {
                InnerArray.Sort(Comparer);
            }

            /// <summary>
            ///  Retrieves the item with the specified index.
            /// </summary>
            public virtual object this[int index]
            {
                get
                {
                    if (index < 0 || index >= InnerArray.Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }
                    return InnerArray[index];
                }
                set
                {
                    //this.owner.CheckNoSharedCell();
                    owner.CheckNoDataSource();

                    if (index < 0 || index >= InnerArray.Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    InnerArray[index] = value ?? throw new ArgumentNullException(nameof(value));
                    owner.OnItemsCollectionChanged();
                }
            }

            /// <summary>
            ///  Removes all items from the collection.
            /// </summary>
            public void Clear()
            {
                if (InnerArray.Count > 0)
                {
                    //this.owner.CheckNoSharedCell();
                    owner.CheckNoDataSource();
                    InnerArray.Clear();
                    owner.OnItemsCollectionChanged();
                }
            }

            internal void ClearInternal()
            {
                InnerArray.Clear();
            }

            public bool Contains(object value)
            {
                return IndexOf(value) != -1;
            }

            /// <summary>
            ///  Copies the DataGridViewComboBoxCell Items collection to a destination array.
            /// </summary>
            public void CopyTo(object[] destination, int arrayIndex)
            {
                int count = InnerArray.Count;
                for (int i = 0; i < count; i++)
                {
                    destination[i + arrayIndex] = InnerArray[i];
                }
            }

            void ICollection.CopyTo(Array destination, int index)
            {
                int count = InnerArray.Count;
                for (int i = 0; i < count; i++)
                {
                    destination.SetValue(InnerArray[i], i + index);
                }
            }

            /// <summary>
            ///  Returns an enumerator for the DataGridViewComboBoxCell Items collection.
            /// </summary>
            public IEnumerator GetEnumerator()
            {
                return InnerArray.GetEnumerator();
            }

            public int IndexOf(object value)
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                return InnerArray.IndexOf(value);
            }

            /// <summary>
            ///  Adds an item to the collection. For an unsorted combo box, the item is
            ///  added to the end of the existing list of items. For a sorted combo box,
            ///  the item is inserted into the list according to its sorted position.
            ///  The item's toString() method is called to obtain the string that is
            ///  displayed in the combo box.
            /// </summary>
            public void Insert(int index, object item)
            {
                //this.owner.CheckNoSharedCell();
                owner.CheckNoDataSource();

                if (item is null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                if (index < 0 || index > InnerArray.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), nameof(index)));
                }

                // If the combo box is sorted, then just treat this like an add
                // because we are going to twiddle the index anyway.
                if (owner.Sorted)
                {
                    Add(item);
                }
                else
                {
                    InnerArray.Insert(index, item);
                    owner.OnItemsCollectionChanged();
                }
            }

            /// <summary>
            ///  Removes the given item from the collection, provided that it is
            ///  actually in the list.
            /// </summary>
            public void Remove(object value)
            {
                int index = InnerArray.IndexOf(value);

                if (index != -1)
                {
                    RemoveAt(index);
                }
            }

            /// <summary>
            ///  Removes an item from the collection at the given index.
            /// </summary>
            public void RemoveAt(int index)
            {
                //this.owner.CheckNoSharedCell();
                owner.CheckNoDataSource();

                if (index < 0 || index >= InnerArray.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }
                InnerArray.RemoveAt(index);
                owner.OnItemsCollectionChanged();
            }
        } // end ObjectCollection

        private sealed class ItemComparer : IComparer
        {
            private readonly DataGridViewComboBoxCell dataGridViewComboBoxCell;

            public ItemComparer(DataGridViewComboBoxCell dataGridViewComboBoxCell)
            {
                this.dataGridViewComboBoxCell = dataGridViewComboBoxCell;
            }

            public int Compare(object item1, object item2)
            {
                if (item1 is null)
                {
                    if (item2 is null)
                    {
                        return 0; //both null, then they are equal
                    }
                    return -1; //item1 is null, but item2 is valid (greater)
                }
                if (item2 is null)
                {
                    return 1; //item2 is null, so item 1 is greater
                }
                string itemName1 = dataGridViewComboBoxCell.GetItemDisplayText(item1);
                string itemName2 = dataGridViewComboBoxCell.GetItemDisplayText(item2);

                CompareInfo compInfo = Application.CurrentCulture.CompareInfo;
                return compInfo.Compare(itemName1, itemName2, CompareOptions.StringSort);
            }
        }
    }
}

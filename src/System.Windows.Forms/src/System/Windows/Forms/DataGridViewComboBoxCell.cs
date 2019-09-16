// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
    public class DataGridViewComboBoxCell : DataGridViewCell
    {
        private static readonly int PropComboBoxCellDataSource = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellDisplayMember = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellValueMember = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellItems = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellDropDownWidth = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellMaxDropDownItems = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellEditingComboBox = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellValueMemberProp = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellDisplayMemberProp = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellDataManager = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellColumnTemplate = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellFlatStyle = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellDisplayStyle = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellDisplayStyleForCurrentCellOnly = PropertyStore.CreateKey();

        private const byte DATAGRIDVIEWCOMBOBOXCELL_margin = 3;
        private const byte DATAGRIDVIEWCOMBOBOXCELL_nonXPTriangleHeight = 4;
        private const byte DATAGRIDVIEWCOMBOBOXCELL_nonXPTriangleWidth = 7;
        private const byte DATAGRIDVIEWCOMBOBOXCELL_horizontalTextMarginLeft = 0;
        private const byte DATAGRIDVIEWCOMBOBOXCELL_verticalTextMarginTopWithWrapping = 0;
        private const byte DATAGRIDVIEWCOMBOBOXCELL_verticalTextMarginTopWithoutWrapping = 1;

        private const byte DATAGRIDVIEWCOMBOBOXCELL_ignoreNextMouseClick = 0x01;
        private const byte DATAGRIDVIEWCOMBOBOXCELL_sorted = 0x02;
        private const byte DATAGRIDVIEWCOMBOBOXCELL_createItemsFromDataSource = 0x04;
        private const byte DATAGRIDVIEWCOMBOBOXCELL_autoComplete = 0x08;
        private const byte DATAGRIDVIEWCOMBOBOXCELL_dataSourceInitializedHookedUp = 0x10;
        private const byte DATAGRIDVIEWCOMBOBOXCELL_dropDownHookedUp = 0x20;

        internal const int DATAGRIDVIEWCOMBOBOXCELL_defaultMaxDropDownItems = 8;

        private static readonly Type defaultFormattedValueType = typeof(string);
        private static readonly Type defaultEditType = typeof(DataGridViewComboBoxEditingControl);
        private static readonly Type defaultValueType = typeof(object);
        private static readonly Type cellType = typeof(DataGridViewComboBoxCell);

        private byte flags;  // see DATAGRIDVIEWCOMBOBOXCELL_ consts above
        private static bool mouseInDropDownButtonBounds = false;
        private static int cachedDropDownWidth = -1;

        // Autosizing changed for VS
        // We need to make ItemFromComboBoxDataSource as fast as possible because ItemFromComboBoxDataSource is getting called a lot
        // during AutoSize. To do that we keep a copy of the key and the value.
        //private object keyUsedDuringAutoSize    = null;
        //private object valueUsedDuringAutoSize  = null;

        private static bool isScalingInitialized = false;
        private static readonly int OFFSET_2PIXELS = 2;
        private static int offset2X = OFFSET_2PIXELS;
        private static int offset2Y = OFFSET_2PIXELS;
        private static byte nonXPTriangleHeight = DATAGRIDVIEWCOMBOBOXCELL_nonXPTriangleHeight;
        private static byte nonXPTriangleWidth = DATAGRIDVIEWCOMBOBOXCELL_nonXPTriangleWidth;

        public DataGridViewComboBoxCell()
        {
            flags = DATAGRIDVIEWCOMBOBOXCELL_autoComplete;
            if (!isScalingInitialized)
            {
                if (DpiHelper.IsScalingRequired)
                {
                    offset2X = DpiHelper.LogicalToDeviceUnitsX(OFFSET_2PIXELS);
                    offset2Y = DpiHelper.LogicalToDeviceUnitsY(OFFSET_2PIXELS);
                    nonXPTriangleWidth = (byte)DpiHelper.LogicalToDeviceUnitsX(DATAGRIDVIEWCOMBOBOXCELL_nonXPTriangleWidth);
                    nonXPTriangleHeight = (byte)DpiHelper.LogicalToDeviceUnitsY(DATAGRIDVIEWCOMBOBOXCELL_nonXPTriangleHeight);
                }
                isScalingInitialized = true;
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
                return ((flags & DATAGRIDVIEWCOMBOBOXCELL_autoComplete) != 0x00);
            }
            set
            {
                //CheckNoSharedCell();
                if (value != AutoComplete)
                {
                    if (value)
                    {
                        flags |= (byte)DATAGRIDVIEWCOMBOBOXCELL_autoComplete;
                    }
                    else
                    {
                        flags = (byte)(flags & ~DATAGRIDVIEWCOMBOBOXCELL_autoComplete);
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
                if (value != null || Properties.ContainsObject(PropComboBoxCellDataManager))
                {
                    Properties.SetObject(PropComboBoxCellDataManager, value);
                }
            }
        }

        public virtual object DataSource
        {
            get
            {
                return Properties.GetObject(PropComboBoxCellDataSource);
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

                    Properties.SetObject(PropComboBoxCellDataSource, value);

                    WireDataSource(value);

                    // Invalidate existing Items collection
                    CreateItemsFromDataSource = true;
                    cachedDropDownWidth = -1;

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

                    if (value == null)
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
                object displayMember = Properties.GetObject(PropComboBoxCellDisplayMember);
                if (displayMember == null)
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
                if ((value != null && value.Length > 0) || Properties.ContainsObject(PropComboBoxCellDisplayMember))
                {
                    Properties.SetObject(PropComboBoxCellDisplayMember, value);
                }
            }
        }

        private PropertyDescriptor DisplayMemberProperty
        {
            get
            {
                return (PropertyDescriptor)Properties.GetObject(PropComboBoxCellDisplayMemberProp);
            }
            set
            {
                if (value != null || Properties.ContainsObject(PropComboBoxCellDisplayMemberProp))
                {
                    Properties.SetObject(PropComboBoxCellDisplayMemberProp, value);
                }
            }
        }

        [DefaultValue(DataGridViewComboBoxDisplayStyle.DropDownButton)]
        public DataGridViewComboBoxDisplayStyle DisplayStyle
        {
            get
            {
                int displayStyle = Properties.GetInteger(PropComboBoxCellDisplayStyle, out bool found);
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
                    Properties.SetInteger(PropComboBoxCellDisplayStyle, (int)value);
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
                    Properties.SetInteger(PropComboBoxCellDisplayStyle, (int)value);
                }
            }
        }

        [DefaultValue(false)]
        public bool DisplayStyleForCurrentCellOnly
        {
            get
            {
                int displayStyleForCurrentCellOnly = Properties.GetInteger(PropComboBoxCellDisplayStyleForCurrentCellOnly, out bool found);
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
                    Properties.SetInteger(PropComboBoxCellDisplayStyleForCurrentCellOnly, value ? 1 : 0);
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
                    Properties.SetInteger(PropComboBoxCellDisplayStyleForCurrentCellOnly, value ? 1 : 0);
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
                    return defaultFormattedValueType;
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
                int dropDownWidth = Properties.GetInteger(PropComboBoxCellDropDownWidth, out bool found);
                return found ? dropDownWidth : 1;
            }
            set
            {
                //CheckNoSharedCell();
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(DropDownWidth), value, string.Format(SR.DataGridViewComboBoxCell_DropDownWidthOutOfRange, 1));
                }
                Properties.SetInteger(PropComboBoxCellDropDownWidth, (int)value);
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
                return (DataGridViewComboBoxEditingControl)Properties.GetObject(PropComboBoxCellEditingComboBox);
            }
            set
            {
                if (value != null || Properties.ContainsObject(PropComboBoxCellEditingComboBox))
                {
                    Properties.SetObject(PropComboBoxCellEditingComboBox, value);
                }
            }
        }

        public override Type EditType
        {
            get
            {
                return defaultEditType;
            }
        }

        [DefaultValue(FlatStyle.Standard)]
        public FlatStyle FlatStyle
        {
            get
            {
                int flatStyle = Properties.GetInteger(PropComboBoxCellFlatStyle, out bool found);
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
                    Properties.SetInteger(PropComboBoxCellFlatStyle, (int)value);
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
                    Properties.SetInteger(PropComboBoxCellFlatStyle, (int)value);
                }
            }
        }

        public override Type FormattedValueType
        {
            get
            {
                return defaultFormattedValueType;
            }
        }

        internal bool HasItems
        {
            get
            {
                return Properties.ContainsObject(PropComboBoxCellItems) && Properties.GetObject(PropComboBoxCellItems) != null;
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

        [DefaultValue(DATAGRIDVIEWCOMBOBOXCELL_defaultMaxDropDownItems)]
        public virtual int MaxDropDownItems
        {
            get
            {
                int maxDropDownItems = Properties.GetInteger(PropComboBoxCellMaxDropDownItems, out bool found);
                if (found)
                {
                    return maxDropDownItems;
                }
                return DATAGRIDVIEWCOMBOBOXCELL_defaultMaxDropDownItems;
            }
            set
            {
                //CheckNoSharedCell();
                if (value < 1 || value > 100)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaxDropDownItems), value, string.Format(SR.DataGridViewComboBoxCell_MaxDropDownItemsOutOfRange, 1, 100));
                }
                Properties.SetInteger(PropComboBoxCellMaxDropDownItems, (int)value);
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
                return ((flags & DATAGRIDVIEWCOMBOBOXCELL_sorted) != 0x00);
            }
            set
            {
                //CheckNoSharedCell();
                if (value != Sorted)
                {
                    if (value)
                    {
                        if (DataSource == null)
                        {
                            Items.SortInternal();
                        }
                        else
                        {
                            throw new ArgumentException(SR.ComboBoxSortWithDataSource);
                        }
                        flags |= (byte)DATAGRIDVIEWCOMBOBOXCELL_sorted;
                    }
                    else
                    {
                        flags = (byte)(flags & ~DATAGRIDVIEWCOMBOBOXCELL_sorted);
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
                return (DataGridViewComboBoxColumn)Properties.GetObject(PropComboBoxCellColumnTemplate);
            }
            set
            {
                Properties.SetObject(PropComboBoxCellColumnTemplate, value);
            }
        }

        [DefaultValue("")]
        public virtual string ValueMember
        {
            get
            {
                object valueMember = Properties.GetObject(PropComboBoxCellValueMember);
                if (valueMember == null)
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
                if ((value != null && value.Length > 0) || Properties.ContainsObject(PropComboBoxCellValueMember))
                {
                    Properties.SetObject(PropComboBoxCellValueMember, value);
                }
            }
        }

        private PropertyDescriptor ValueMemberProperty
        {
            get
            {
                return (PropertyDescriptor)Properties.GetObject(PropComboBoxCellValueMemberProp);
            }
            set
            {
                if (value != null || Properties.ContainsObject(PropComboBoxCellValueMemberProp))
                {
                    Properties.SetObject(PropComboBoxCellValueMemberProp, value);
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
                    return defaultValueType;
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
            using (Graphics g = WindowsFormsUtils.CreateMeasurementGraphics())
            {
                dropHeight = Math.Min(GetDropDownButtonHeight(g, cellStyle), adjustedSize.Height - 2);
            }

            int dropWidth = Math.Min(SystemInformation.HorizontalScrollBarThumbWidth, adjustedSize.Width - 2 * DATAGRIDVIEWCOMBOBOXCELL_margin - 1);

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
                        if (cachedDropDownWidth == -1)
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
                            cachedDropDownWidth = maxPreferredWidth + 2 + SystemInformation.VerticalScrollBarWidth;
                        }
                        Debug.Assert(cachedDropDownWidth >= 1);
                        UnsafeNativeMethods.SendMessage(new HandleRef(comboBox, comboBox.Handle), NativeMethods.CB_SETDROPPEDWIDTH, cachedDropDownWidth, 0);
                    }
                }
                else
                {
                    // The dropdown width may have been previously adjusted to the items because of the owning column autosized.
                    // The dropdown width needs to be realigned to the DropDownWidth property value.
                    int dropDownWidth = unchecked((int)(long)UnsafeNativeMethods.SendMessage(new HandleRef(comboBox, comboBox.Handle), NativeMethods.CB_GETDROPPEDWIDTH, 0, 0));
                    if (dropDownWidth != DropDownWidth)
                    {
                        UnsafeNativeMethods.SendMessage(new HandleRef(comboBox, comboBox.Handle), NativeMethods.CB_SETDROPPEDWIDTH, DropDownWidth, 0);
                    }
                }
            }
        }

        public override object Clone()
        {
            DataGridViewComboBoxCell dataGridViewCell;
            Type thisType = GetType();

            if (thisType == cellType) //performance improvement
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
            if (HasItems && DataSource == null && Items.Count > 0)
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
                return ((flags & DATAGRIDVIEWCOMBOBOXCELL_createItemsFromDataSource) != 0x00);
            }
            set
            {
                if (value)
                {
                    flags |= (byte)DATAGRIDVIEWCOMBOBOXCELL_createItemsFromDataSource;
                }
                else
                {
                    flags = (byte)(flags & ~DATAGRIDVIEWCOMBOBOXCELL_createItemsFromDataSource);
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
            Debug.Assert((flags & DATAGRIDVIEWCOMBOBOXCELL_dataSourceInitializedHookedUp) != 0x00);

            // Unhook the Initialized event.
            if (DataSource is ISupportInitializeNotification dsInit)
            {
                dsInit.Initialized -= new EventHandler(DataSource_Initialized);
            }

            // The wait is over: DataSource is initialized.
            flags = (byte)(flags & ~DATAGRIDVIEWCOMBOBOXCELL_dataSourceInitializedHookedUp);

            // Check the DisplayMember and ValueMember values - will throw if values don't match existing fields.
            InitializeDisplayMemberPropertyDescriptor(DisplayMember);
            InitializeValueMemberPropertyDescriptor(ValueMember);
        }

        public override void DetachEditingControl()
        {
            DataGridView dgv = DataGridView;
            if (dgv == null || dgv.EditingControl == null)
            {
                throw new InvalidOperationException();
            }

            if (EditingComboBox != null &&
                (flags & DATAGRIDVIEWCOMBOBOXCELL_dropDownHookedUp) != 0x00)
            {
                EditingComboBox.DropDown -= new EventHandler(ComboBox_DropDown);
                flags = (byte)(flags & ~DATAGRIDVIEWCOMBOBOXCELL_dropDownHookedUp);
            }

            EditingComboBox = null;
            base.DetachEditingControl();
        }

        protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (DataGridView == null || rowIndex < 0 || OwningColumn == null)
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
            CurrencyManager cm = (CurrencyManager)Properties.GetObject(PropComboBoxCellDataManager);
            if (cm == null && DataSource != null && dataGridView != null && dataGridView.BindingContext != null && !(DataSource == Convert.DBNull))
            {
                if (DataSource is ISupportInitializeNotification dsInit && !dsInit.IsInitialized)
                {
                    if ((flags & DATAGRIDVIEWCOMBOBOXCELL_dataSourceInitializedHookedUp) == 0x00)
                    {
                        dsInit.Initialized += new EventHandler(DataSource_Initialized);
                        flags |= (byte)DATAGRIDVIEWCOMBOBOXCELL_dataSourceInitializedHookedUp;
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
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (DataGridView == null ||
                rowIndex < 0 ||
                OwningColumn == null ||
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
            if (valueTypeConverter == null)
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

            if (value == null || ((ValueType != null && !ValueType.IsAssignableFrom(value.GetType())) && value != System.DBNull.Value))
            {
                // Do not raise the DataError event if the value is null and the row is the 'new row'.

                if (value == null /* && ((this.DataGridView != null && rowIndex == this.DataGridView.NewRowIndex) || this.Items.Count == 0)*/)
                {
                    // Debug.Assert(rowIndex != -1 || this.Items.Count == 0);
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
            ObjectCollection items = (ObjectCollection)Properties.GetObject(PropComboBoxCellItems);
            if (items == null)
            {
                items = new ObjectCollection(this);
                Properties.SetObject(PropComboBoxCellItems, items);
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
                if (dataManager != null || (flags & DATAGRIDVIEWCOMBOBOXCELL_dataSourceInitializedHookedUp) == 0x00)
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
            if (DataGridView == null)
            {
                return new Size(-1, -1);
            }

            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            Size preferredSize = Size.Empty;
            DataGridViewFreeDimension freeDimension = DataGridViewCell.GetFreeDimensionFromConstraint(constraintSize);
            Rectangle borderWidthsRect = StdBorderWidths;
            int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
            int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;
            TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);

            /* Changing design of DGVComboBoxCell.GetPreferredSize for performance reasons.
             * Old design required looking through each combo item
            string formattedValue;
            if (freeDimension == DataGridViewFreeDimension.Height)
            {
                formattedValue = GetFormattedValue(rowIndex, ref cellStyle, DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.PreferredSize) as string;
                if (formattedValue != null)
                {
                    preferredSize = new Size(0,
                                             DataGridViewCell.MeasureTextSize(graphics, formattedValue, cellStyle.Font, flags).Height);
                }
                else
                {
                    preferredSize = new Size(DataGridViewCell.MeasureTextSize(graphics, " ", cellStyle.Font, flags).Height,
                                             0);
                }
            }
            else
            {
                if ((this.HasItems || this.CreateItemsFromDataSource) && this.Items.Count > 0)
                {
                    int maxPreferredWidth = -1;
                    try
                    {
                        foreach (object item in this.Items)
                        {
                            this.valueUsedDuringAutoSize = item;
                            this.keyUsedDuringAutoSize = GetItemValue(item);
                            formattedValue = GetFormattedValue(this.keyUsedDuringAutoSize, rowIndex, ref cellStyle, null, null, DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.PreferredSize) as string;
                            if (formattedValue != null)
                            {
                                preferredSize = DataGridViewCell.MeasureTextSize(graphics, formattedValue, cellStyle.Font, flags);
                            }
                            else
                            {
                                preferredSize = DataGridViewCell.MeasureTextSize(graphics, " ", cellStyle.Font, flags);
                            }
                            if (preferredSize.Width > maxPreferredWidth)
                            {
                                maxPreferredWidth = preferredSize.Width;
                            }
                        }
                    }
                    finally
                    {
                        this.keyUsedDuringAutoSize = null;
                        this.valueUsedDuringAutoSize = null;
                    }
                    preferredSize.Width = maxPreferredWidth;
                }
                else
                {
                    formattedValue = GetFormattedValue(rowIndex, ref cellStyle, DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.PreferredSize) as string;
                    if (formattedValue != null)
                    {
                        preferredSize = DataGridViewCell.MeasureTextSize(graphics, formattedValue, cellStyle.Font, flags);
                    }
                    else
                    {
                        preferredSize = DataGridViewCell.MeasureTextSize(graphics, " ", cellStyle.Font, flags);
                    }
                }
                if (freeDimension == DataGridViewFreeDimension.Width)
                {
                    preferredSize.Height = 0;
                }
            }
            */

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
                preferredSize.Width += SystemInformation.HorizontalScrollBarThumbWidth + 1 + 2 * DATAGRIDVIEWCOMBOBOXCELL_margin + borderAndPaddingWidths;
                if (DataGridView.ShowCellErrors)
                {
                    // Making sure that there is enough room for the potential error icon
                    preferredSize.Width = Math.Max(preferredSize.Width, borderAndPaddingWidths + SystemInformation.HorizontalScrollBarThumbWidth + 1 + DATAGRIDVIEWCELL_iconMarginWidth * 2 + iconsWidth);
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
                    preferredSize.Height = Math.Max(preferredSize.Height, borderAndPaddingHeights + DATAGRIDVIEWCELL_iconMarginHeight * 2 + iconsHeight);
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
                if (HasItems && DataSource == null && Items.Count > 0)
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

                if ((flags & DATAGRIDVIEWCOMBOBOXCELL_dropDownHookedUp) == 0x00)
                {
                    comboBox.DropDown += new EventHandler(ComboBox_DropDown);
                    flags |= (byte)DATAGRIDVIEWCOMBOBOXCELL_dropDownHookedUp;
                }
                cachedDropDownWidth = -1;

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
                    if (displayMemberProperty == null)
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
                    if (valueMemberProperty == null)
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
            if (key == null)
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
                if (displayValue == null || !displayValue.Equals(key))
                {
                    // No, the selected item is not looked for.
                    item = null; // Need to loop through all the items
                }
            }
            if (item == null)
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
            if (item == null)
            {
                // The provided field could be wrong - try to match the key against an actual item
                if (OwnsEditingComboBox(rowIndex))
                {
                    // It is likely that the item looked for is the selected item.
                    item = EditingComboBox.SelectedItem;
                    if (item == null || !item.Equals(key))
                    {
                        item = null;
                    }
                }
                if (item == null && Items.Contains(key))
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
            if (item == null)
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
            if (formattedValue == null)
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
            if (item == null)
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
            if (DataGridView == null)
            {
                return;
            }
            if (throughMouseClick && DataGridView.EditMode != DataGridViewEditMode.EditOnEnter)
            {
                flags |= (byte)DATAGRIDVIEWCOMBOBOXCELL_ignoreNextMouseClick;
            }
        }

        private void OnItemsCollectionChanged()
        {
            if (TemplateComboBoxColumn != null)
            {
                Debug.Assert(TemplateComboBoxColumn.CellTemplate == this);
                TemplateComboBoxColumn.OnItemsCollectionChanged();
            }
            cachedDropDownWidth = -1;
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
            if (DataGridView == null)
            {
                return;
            }
            flags = (byte)(flags & ~DATAGRIDVIEWCOMBOBOXCELL_ignoreNextMouseClick);
        }

        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            if (DataGridView == null)
            {
                return;
            }
            Debug.Assert(e.ColumnIndex == ColumnIndex);
            Point ptCurrentCell = DataGridView.CurrentCellAddress;
            if (ptCurrentCell.X == e.ColumnIndex && ptCurrentCell.Y == e.RowIndex)
            {
                if ((flags & DATAGRIDVIEWCOMBOBOXCELL_ignoreNextMouseClick) != 0x00)
                {
                    flags = (byte)(flags & ~DATAGRIDVIEWCOMBOBOXCELL_ignoreNextMouseClick);
                }
                else if ((EditingComboBox == null || !EditingComboBox.DroppedDown) &&
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
            if (DataGridView == null)
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
            if (DataGridView == null)
            {
                return;
            }

            if (mouseInDropDownButtonBounds)
            {
                mouseInDropDownButtonBounds = false;
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
            if (DataGridView == null)
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
                using (Graphics g = WindowsFormsUtils.CreateMeasurementGraphics())
                {
                    PaintPrivate(g,
                        cellBounds,
                        cellBounds,
                        rowIndex,
                        cellState,
                        null /*formattedValue*/,            // dropDownButtonRect is independent of formattedValue
                        null /*errorText*/,                 // dropDownButtonRect is independent of errorText
                        cellStyle,
                        dgvabsEffective,
                        out dropDownButtonRect,
                        DataGridViewPaintParts.ContentForeground,
                        false /*computeContentBounds*/,
                        false /*computeErrorIconBounds*/,
                        true  /*computeDropDownButtonRect*/,
                        false /*paint*/);
                }

                bool newMouseInDropDownButtonBounds = dropDownButtonRect.Contains(DataGridView.PointToClient(Control.MousePosition));
                if (newMouseInDropDownButtonBounds != mouseInDropDownButtonBounds)
                {
                    mouseInDropDownButtonBounds = newMouseInDropDownButtonBounds;
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
            if (cellStyle == null)
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
                mouseInDropDownButtonBounds)
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

            SolidBrush br;
            Point ptCurrentCell = DataGridView.CurrentCellAddress;
            bool cellCurrent = ptCurrentCell.X == ColumnIndex && ptCurrentCell.Y == rowIndex;
            bool cellEdited = cellCurrent && DataGridView.EditingControl != null;
            bool cellSelected = (elementState & DataGridViewElementStates.Selected) != 0;
            bool drawComboBox = DisplayStyle == DataGridViewComboBoxDisplayStyle.ComboBox &&
                                ((DisplayStyleForCurrentCellOnly && cellCurrent) || !DisplayStyleForCurrentCellOnly);
            bool drawDropDownButton = DisplayStyle != DataGridViewComboBoxDisplayStyle.Nothing &&
                                ((DisplayStyleForCurrentCellOnly && cellCurrent) || !DisplayStyleForCurrentCellOnly);
            if (DataGridViewCell.PaintSelectionBackground(paintParts) && cellSelected && !cellEdited)
            {
                br = DataGridView.GetCachedBrush(cellStyle.SelectionBackColor);
            }
            else
            {
                br = DataGridView.GetCachedBrush(cellStyle.BackColor);
            }

            if (paint && DataGridViewCell.PaintBackground(paintParts) && br.Color.A == 255 && valBounds.Width > 0 && valBounds.Height > 0)
            {
                DataGridViewCell.PaintPadding(g, valBounds, cellStyle, br, DataGridView.RightToLeftInternal);
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
                    if (paintPostXPThemes && DataGridViewCell.PaintBackground(paintParts) && br.Color.A == 255)
                    {
                        g.FillRectangle(br, valBounds.Left, valBounds.Top, valBounds.Width, valBounds.Height);
                    }
                    if (DataGridViewCell.PaintContentBackground(paintParts))
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
                        DataGridViewCell.PaintBackground(paintParts) && br.Color.A == 255 && valBounds.Width > 2 && valBounds.Height > 2)
                    {
                        g.FillRectangle(br, valBounds.Left + 1, valBounds.Top + 1, valBounds.Width - 2, valBounds.Height - 2);
                    }
                }
                else if (DataGridViewCell.PaintBackground(paintParts) && br.Color.A == 255)
                {
                    if (paintPostXPThemes && drawDropDownButton && !drawComboBox)
                    {
                        g.DrawRectangle(SystemPens.ControlLightLight, new Rectangle(valBounds.X, valBounds.Y, valBounds.Width - 1, valBounds.Height - 1));
                    }
                    else
                    {
                        g.FillRectangle(br, valBounds.Left, valBounds.Top, valBounds.Width, valBounds.Height);
                    }
                }
            }

            int dropWidth = Math.Min(SystemInformation.HorizontalScrollBarThumbWidth, valBounds.Width - 2 * DATAGRIDVIEWCOMBOBOXCELL_margin - 1);

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
                            dropRect = new Rectangle(DataGridView.RightToLeftInternal ? valBounds.Left : valBounds.Right - dropWidth,
                                                    valBounds.Top,
                                                    dropWidth,
                                                    dropHeight);
                        }
                        else
                        {
                            dropRect = new Rectangle(DataGridView.RightToLeftInternal ? valBounds.Left + 1 : valBounds.Right - dropWidth - 1,
                                                    valBounds.Top + 1,
                                                    dropWidth,
                                                    dropHeight);
                        }
                    }
                    else
                    {
                        dropRect = new Rectangle(DataGridView.RightToLeftInternal ? valBounds.Left + 2 : valBounds.Right - dropWidth - 2,
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

                    if (paint && DataGridViewCell.PaintContentBackground(paintParts))
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
                                        // In the case of ComboBox style, background is not filled in,
                                        // in the case of DrawReadOnlyButton uses theming API to render CP_READONLY COMBOBOX part that renders the background,
                                        // this API does not have "selected" state, thus always uses BackColor
                                        br = DataGridView.GetCachedBrush(cellStyle.BackColor);
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
                            // border painting is ripped from button renderer
                            Color color = SystemColors.Control;
                            Color buttonShadow;
                            Color buttonShadowDark;
                            Color buttonFace = color;
                            Color highlight;
                            bool stockColor = color.ToKnownColor() == SystemColors.Control.ToKnownColor();
                            bool highContrast = SystemInformation.HighContrast;
                            if (color == SystemColors.Control)
                            {
                                buttonShadow = SystemColors.ControlDark;
                                buttonShadowDark = SystemColors.ControlDarkDark;
                                highlight = SystemColors.ControlLightLight;
                            }
                            else
                            {
                                buttonShadow = ControlPaint.Dark(color);
                                highlight = ControlPaint.LightLight(color);
                                if (highContrast)
                                {
                                    buttonShadowDark = ControlPaint.LightLight(color);
                                }
                                else
                                {
                                    buttonShadowDark = ControlPaint.DarkDark(color);
                                }
                            }

                            buttonShadow = g.GetNearestColor(buttonShadow);
                            buttonShadowDark = g.GetNearestColor(buttonShadowDark);
                            buttonFace = g.GetNearestColor(buttonFace);
                            highlight = g.GetNearestColor(highlight);
                            // top + left
                            Pen pen;
                            if (stockColor)
                            {
                                if (SystemInformation.HighContrast)
                                {
                                    pen = SystemPens.ControlLight;
                                }
                                else
                                {
                                    pen = SystemPens.Control;
                                }
                            }
                            else
                            {
                                pen = new Pen(highlight);
                            }

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
                            // bottom + right
                            if (stockColor)
                            {
                                pen = SystemPens.ControlDarkDark;
                            }
                            else
                            {
                                pen.Color = buttonShadowDark;
                            }
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
                            if (stockColor)
                            {
                                pen = SystemPens.ControlLightLight;
                            }
                            else
                            {
                                pen.Color = buttonFace;
                            }
                            if (drawDropDownButton)
                            {
                                g.DrawLine(pen, dropRect.X + 1, dropRect.Y + 1,
                                        dropRect.X + dropRect.Width - 2, dropRect.Y + 1);
                                g.DrawLine(pen, dropRect.X + 1, dropRect.Y + 1,
                                        dropRect.X + 1, dropRect.Y + dropRect.Height - 2);
                            }
                            // Bottom + Right inset
                            if (stockColor)
                            {
                                pen = SystemPens.ControlDark;
                            }
                            else
                            {
                                pen.Color = buttonShadow;
                            }
                            if (drawDropDownButton)
                            {
                                g.DrawLine(pen, dropRect.X + 1, dropRect.Y + dropRect.Height - 2,
                                        dropRect.X + dropRect.Width - 2, dropRect.Y + dropRect.Height - 2);
                                g.DrawLine(pen, dropRect.X + dropRect.Width - 2, dropRect.Y + 1,
                                        dropRect.X + dropRect.Width - 2, dropRect.Y + dropRect.Height - 2);
                            }
                            if (!stockColor)
                            {
                                pen.Dispose();
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
                                    new Point(middle.X - offset2X, middle.Y - 1),
                                    new Point(middle.X + offset2X + 1, middle.Y - 1),
                                    new Point(middle.X, middle.Y + offset2Y)
                                });
                            }
                            else if (!paintXPThemes)
                            {
                                // XPThemes already painted the drop down button

                                // the down arrow looks better when it's fatten up by a pixel
                                dropRect.X--;
                                dropRect.Width++;

                                Point middle = new Point(dropRect.Left + (dropRect.Width - 1) / 2,
                                        dropRect.Top + (dropRect.Height + nonXPTriangleHeight) / 2);
                                // if the width is event - favor pushing it over one pixel right.
                                middle.X += ((dropRect.Width + 1) % 2);
                                // if the height is odd - favor pushing it over one pixel down.
                                middle.Y += (dropRect.Height % 2);
                                Point pt1 = new Point(middle.X - (nonXPTriangleWidth - 1) / 2, middle.Y - nonXPTriangleHeight);
                                Point pt2 = new Point(middle.X + (nonXPTriangleWidth - 1) / 2, middle.Y - nonXPTriangleHeight);
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
                    DataGridViewCell.PaintFocus(paintParts) &&
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
                        ControlPaint.DrawFocusRectangle(g, focusBounds, Color.Empty, br.Color);
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
                            ControlPaint.DrawFocusRectangle(g, focusBounds, Color.Empty, br.Color);
                        }
                    }
                    else
                    {
                        ControlPaint.DrawFocusRectangle(g, textBounds, Color.Empty, br.Color);
                    }
                }

                if (paintPopup)
                {
                    valBounds.Width--;
                    valBounds.Height--;
                    if (!cellEdited && paint && DataGridViewCell.PaintContentBackground(paintParts) && drawComboBox)
                    {
                        g.DrawRectangle(SystemPens.ControlDark, valBounds);
                    }
                }

                if (formattedValue is string formattedString)
                {
                    // Font independent margins
                    int verticalTextMarginTop = cellStyle.WrapMode == DataGridViewTriState.True ? DATAGRIDVIEWCOMBOBOXCELL_verticalTextMarginTopWithWrapping : DATAGRIDVIEWCOMBOBOXCELL_verticalTextMarginTopWithoutWrapping;
                    if (DataGridView.RightToLeftInternal)
                    {
                        textBounds.Offset(DATAGRIDVIEWCOMBOBOXCELL_horizontalTextMarginLeft, verticalTextMarginTop);
                        textBounds.Width += 2 - DATAGRIDVIEWCOMBOBOXCELL_horizontalTextMarginLeft;
                    }
                    else
                    {
                        textBounds.Offset(DATAGRIDVIEWCOMBOBOXCELL_horizontalTextMarginLeft - 1, verticalTextMarginTop);
                        textBounds.Width += 1 - DATAGRIDVIEWCOMBOBOXCELL_horizontalTextMarginLeft;
                    }
                    textBounds.Height -= verticalTextMarginTop;

                    if (textBounds.Width > 0 && textBounds.Height > 0)
                    {
                        TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
                        if (!cellEdited && paint)
                        {
                            if (DataGridViewCell.PaintContentForeground(paintParts))
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
                                TextRenderer.DrawText(g,
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

                if (DataGridView.ShowCellErrors && paint && DataGridViewCell.PaintErrorIcon(paintParts))
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

        public override object ParseFormattedValue(object formattedValue,
                                                   DataGridViewCellStyle cellStyle,
                                                   TypeConverter formattedValueTypeConverter,
                                                   TypeConverter valueTypeConverter)
        {
            if (valueTypeConverter == null)
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

            if (DataSource is ISupportInitializeNotification dsInit && (flags & DATAGRIDVIEWCOMBOBOXCELL_dataSourceInitializedHookedUp) != 0x00)
            {
                // If we previously hooked the data source's ISupportInitializeNotification
                // Initialized event, then unhook it now (we don't always hook this event,
                // only if we needed to because the data source was previously uninitialized)
                dsInit.Initialized -= new EventHandler(DataSource_Initialized);
                flags = (byte)(flags & ~DATAGRIDVIEWCOMBOBOXCELL_dataSourceInitializedHookedUp);
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
                    if (comparer == null)
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
                    if (items == null)
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

                if (item == null)
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
                if (items == null)
                {
                    throw new ArgumentNullException(nameof(items));
                }

                foreach (object item in items)
                {
                    if (item == null)
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
                if (value == null)
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

                if (item == null)
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
                if (item1 == null)
                {
                    if (item2 == null)
                    {
                        return 0; //both null, then they are equal
                    }
                    return -1; //item1 is null, but item2 is valid (greater)
                }
                if (item2 == null)
                {
                    return 1; //item2 is null, so item 1 is greater
                }
                string itemName1 = dataGridViewComboBoxCell.GetItemDisplayText(item1);
                string itemName2 = dataGridViewComboBoxCell.GetItemDisplayText(item2);

                CompareInfo compInfo = Application.CurrentCulture.CompareInfo;
                return compInfo.Compare(itemName1, itemName2, CompareOptions.StringSort);
            }
        }

        private class DataGridViewComboBoxCellRenderer
        {
            [ThreadStatic]
            private static VisualStyleRenderer visualStyleRenderer;
            private static readonly VisualStyleElement ComboBoxBorder = VisualStyleElement.ComboBox.Border.Normal;
            private static readonly VisualStyleElement ComboBoxDropDownButtonRight = VisualStyleElement.ComboBox.DropDownButtonRight.Normal;
            private static readonly VisualStyleElement ComboBoxDropDownButtonLeft = VisualStyleElement.ComboBox.DropDownButtonLeft.Normal;
            private static readonly VisualStyleElement ComboBoxReadOnlyButton = VisualStyleElement.ComboBox.ReadOnlyButton.Normal;

            private DataGridViewComboBoxCellRenderer()
            {
            }

            public static VisualStyleRenderer VisualStyleRenderer
            {
                get
                {
                    if (visualStyleRenderer == null)
                    {
                        visualStyleRenderer = new VisualStyleRenderer(ComboBoxReadOnlyButton);
                    }
                    return visualStyleRenderer;
                }
            }

            public static void DrawTextBox(Graphics g, Rectangle bounds, ComboBoxState state)
            {
                ComboBoxRenderer.DrawTextBox(g, bounds, state);
            }

            public static void DrawDropDownButton(Graphics g, Rectangle bounds, ComboBoxState state)
            {
                ComboBoxRenderer.DrawDropDownButton(g, bounds, state);
            }

            // Post theming functions
            public static void DrawBorder(Graphics g, Rectangle bounds)
            {
                if (visualStyleRenderer == null)
                {
                    visualStyleRenderer = new VisualStyleRenderer(ComboBoxBorder);
                }
                else
                {
                    visualStyleRenderer.SetParameters(ComboBoxBorder.ClassName, ComboBoxBorder.Part, ComboBoxBorder.State);
                }
                visualStyleRenderer.DrawBackground(g, bounds);
            }

            public static void DrawDropDownButton(Graphics g, Rectangle bounds, ComboBoxState state, bool rightToLeft)
            {
                if (rightToLeft)
                {
                    if (visualStyleRenderer == null)
                    {
                        visualStyleRenderer = new VisualStyleRenderer(ComboBoxDropDownButtonLeft.ClassName, ComboBoxDropDownButtonLeft.Part, (int)state);
                    }
                    else
                    {
                        visualStyleRenderer.SetParameters(ComboBoxDropDownButtonLeft.ClassName, ComboBoxDropDownButtonLeft.Part, (int)state);
                    }
                }
                else
                {
                    if (visualStyleRenderer == null)
                    {
                        visualStyleRenderer = new VisualStyleRenderer(ComboBoxDropDownButtonRight.ClassName, ComboBoxDropDownButtonRight.Part, (int)state);
                    }
                    else
                    {
                        visualStyleRenderer.SetParameters(ComboBoxDropDownButtonRight.ClassName, ComboBoxDropDownButtonRight.Part, (int)state);
                    }
                }
                visualStyleRenderer.DrawBackground(g, bounds);
            }

            public static void DrawReadOnlyButton(Graphics g, Rectangle bounds, ComboBoxState state)
            {
                if (visualStyleRenderer == null)
                {
                    visualStyleRenderer = new VisualStyleRenderer(ComboBoxReadOnlyButton.ClassName, ComboBoxReadOnlyButton.Part, (int)state);
                }
                else
                {
                    visualStyleRenderer.SetParameters(ComboBoxReadOnlyButton.ClassName, ComboBoxReadOnlyButton.Part, (int)state);
                }
                visualStyleRenderer.DrawBackground(g, bounds);
            }
        }

        protected class DataGridViewComboBoxCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            public DataGridViewComboBoxCellAccessibleObject(DataGridViewCell owner) : base(owner)
            {
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override object GetPropertyValue(int propertyID)
            {
                switch (propertyID)
                {
                    case NativeMethods.UIA_ControlTypePropertyId:
                        return NativeMethods.UIA_ComboBoxControlTypeId;
                    case NativeMethods.UIA_IsExpandCollapsePatternAvailablePropertyId:
                        return IsPatternSupported(NativeMethods.UIA_ExpandCollapsePatternId);
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override bool IsPatternSupported(int patternId)
            {
                if (patternId == NativeMethods.UIA_ExpandCollapsePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override UnsafeNativeMethods.ExpandCollapseState ExpandCollapseState
            {
                get
                {
                    DataGridViewComboBoxEditingControl comboBox = Owner.Properties.GetObject(PropComboBoxCellEditingComboBox) as DataGridViewComboBoxEditingControl;
                    if (comboBox != null)
                    {
                        return comboBox.DroppedDown ? UnsafeNativeMethods.ExpandCollapseState.Expanded : UnsafeNativeMethods.ExpandCollapseState.Collapsed;
                    }

                    return UnsafeNativeMethods.ExpandCollapseState.Collapsed;
                }
            }
        }
    }
}

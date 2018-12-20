// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms.Internal;
    using System.Drawing.Drawing2D;
    using System.ComponentModel;
    using System.Collections;
    using System.Globalization;
    using System.Windows.Forms.VisualStyles;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell"]/*' />
    /// <devdoc>
    ///    <para></para>
    /// </devdoc>
    public class DataGridViewComboBoxCell : DataGridViewCell
    {
        private static readonly int PropComboBoxCellDataSource                     = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellDisplayMember                  = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellValueMember                    = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellItems                          = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellDropDownWidth                  = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellMaxDropDownItems               = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellEditingComboBox                = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellValueMemberProp                = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellDisplayMemberProp              = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellDataManager                    = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellColumnTemplate                 = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellFlatStyle                      = PropertyStore.CreateKey();
        private static readonly int PropComboBoxCellDisplayStyle                   = PropertyStore.CreateKey();
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

        private static Type defaultFormattedValueType = typeof(System.String);
        private static Type defaultEditType = typeof(System.Windows.Forms.DataGridViewComboBoxEditingControl);
        private static Type defaultValueType = typeof(System.Object);
        private static Type cellType = typeof(DataGridViewComboBoxCell);

        private byte flags;  // see DATAGRIDVIEWCOMBOBOXCELL_ consts above
        private static bool mouseInDropDownButtonBounds = false;
        private static int cachedDropDownWidth = -1;

        // Autosizing changed for VS 
        // We need to make ItemFromComboBoxDataSource as fast as possible because ItemFromComboBoxDataSource is getting called a lot
        // during AutoSize. To do that we keep a copy of the key and the value.
        //private object keyUsedDuringAutoSize    = null;
        //private object valueUsedDuringAutoSize  = null;

        private static bool isScalingInitialized = false;
        private static int OFFSET_2PIXELS = 2;
        private static int offset2X = OFFSET_2PIXELS;
        private static int offset2Y = OFFSET_2PIXELS;
        private static byte nonXPTriangleHeight = DATAGRIDVIEWCOMBOBOXCELL_nonXPTriangleHeight;
        private static byte nonXPTriangleWidth = DATAGRIDVIEWCOMBOBOXCELL_nonXPTriangleWidth;

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.DataGridViewComboBoxCell"]/*' />
        public DataGridViewComboBoxCell()
        {
            this.flags = DATAGRIDVIEWCOMBOBOXCELL_autoComplete;
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
        /// Creates a new AccessibleObject for this DataGridViewComboBoxCell instance.
        /// The AccessibleObject instance returned by this method supports ControlType UIA property.
        /// However the new object is only available in applications that are recompiled to target 
        /// .NET Framework 4.7.2 or opt-in into this feature using a compatibility switch. 
        /// </summary>
        /// <returns>
        /// AccessibleObject for this DataGridViewComboBoxCell instance.
        /// </returns>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            if (AccessibilityImprovements.Level2)
            {
                return new DataGridViewComboBoxCellAccessibleObject(this);
            }

            return base.CreateAccessibilityInstance();
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.AutoComplete"]/*' />
        [DefaultValue(true)]
        public virtual bool AutoComplete
        {
            get
            {
                return ((this.flags & DATAGRIDVIEWCOMBOBOXCELL_autoComplete) != 0x00);
            }
            set
            {
                //CheckNoSharedCell();
                if (value != this.AutoComplete)
                {
                    if (value)
                    {
                        this.flags |= (byte)DATAGRIDVIEWCOMBOBOXCELL_autoComplete;
                    }
                    else
                    {
                        this.flags = (byte)(this.flags & ~DATAGRIDVIEWCOMBOBOXCELL_autoComplete);
                    }
                    if (OwnsEditingComboBox(this.RowIndex))
                    {
                        if (value)
                        {
                            this.EditingComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
                            this.EditingComboBox.AutoCompleteMode = AutoCompleteMode.Append;
                        }
                        else
                        {
                            this.EditingComboBox.AutoCompleteMode = AutoCompleteMode.None;
                            this.EditingComboBox.AutoCompleteSource = AutoCompleteSource.None;
                        }
                    }
                }
            }
        }

        private CurrencyManager DataManager
        {
            get
            {
                return GetDataManager(this.DataGridView);
            }
            set
            {
                if (value != null || this.Properties.ContainsObject(PropComboBoxCellDataManager))
                {
                    this.Properties.SetObject(PropComboBoxCellDataManager, value);
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.DataSource"]/*' />
        public virtual object DataSource
        {
            get
            {
                return this.Properties.GetObject(PropComboBoxCellDataSource);
            }
            set
            {
                //CheckNoSharedCell();
                // Same check as for ListControl's DataSource
                if (value != null && !(value is IList || value is IListSource))
                {
                    throw new ArgumentException(SR.BadDataSourceForComplexBinding);
                }
                if (this.DataSource != value)
                {
                    // Invalidate the currency manager
                    this.DataManager = null;

                    UnwireDataSource();

                    this.Properties.SetObject(PropComboBoxCellDataSource, value);

                    WireDataSource(value);

                    // Invalidate existing Items collection
                    this.CreateItemsFromDataSource = true;
                    cachedDropDownWidth = -1;

                    try
                    {
                        InitializeDisplayMemberPropertyDescriptor(this.DisplayMember);
                    }
                    catch (Exception exception)
                    {
                        if (ClientUtils.IsCriticalException(exception))
                        {
                            throw;
                        }
                        Debug.Assert(this.DisplayMember != null && this.DisplayMember.Length > 0);
                        this.DisplayMemberInternal = null;
                    }

                    try
                    {
                        InitializeValueMemberPropertyDescriptor(this.ValueMember);
                    }
                    catch (Exception exception)
                    {
                        if (ClientUtils.IsCriticalException(exception))
                        {
                            throw;
                        }
                        Debug.Assert(this.ValueMember != null && this.ValueMember.Length > 0);
                        this.ValueMemberInternal = null;
                    }

                    if (value == null)
                    {
                        this.DisplayMemberInternal = null;
                        this.ValueMemberInternal = null;
                    }

                    if (OwnsEditingComboBox(this.RowIndex))
                    {
                        this.EditingComboBox.DataSource = value;
                        InitializeComboBoxText();
                    }
                    else
                    {
                        OnCommonChange();
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.DisplayMember"]/*' />
        [DefaultValue("")]
        public virtual string DisplayMember
        {
            get
            {
                object displayMember = this.Properties.GetObject(PropComboBoxCellDisplayMember);
                if (displayMember == null)
                {
                    return String.Empty;
                }
                else
                {
                    return (string)displayMember;
                }
            }
            set
            {
                //CheckNoSharedCell();
                this.DisplayMemberInternal = value;
                if (OwnsEditingComboBox(this.RowIndex))
                {
                    this.EditingComboBox.DisplayMember = value;
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
                if ((value != null && value.Length > 0) || this.Properties.ContainsObject(PropComboBoxCellDisplayMember))
                {
                    this.Properties.SetObject(PropComboBoxCellDisplayMember, value);
                }
            }
        }

        private PropertyDescriptor DisplayMemberProperty
        {
            get
            {
                return (PropertyDescriptor)this.Properties.GetObject(PropComboBoxCellDisplayMemberProp);
            }
            set
            {
                if (value != null || this.Properties.ContainsObject(PropComboBoxCellDisplayMemberProp))
                {
                    this.Properties.SetObject(PropComboBoxCellDisplayMemberProp, value);
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.DisplayStyle"]/*' />
        [DefaultValue(DataGridViewComboBoxDisplayStyle.DropDownButton)]
        public DataGridViewComboBoxDisplayStyle DisplayStyle
        {
            get
            {
                bool found;
                int displayStyle = this.Properties.GetInteger(PropComboBoxCellDisplayStyle, out found);
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
                if (value != this.DisplayStyle)
                {
                    this.Properties.SetInteger(PropComboBoxCellDisplayStyle, (int)value);
                    if (this.DataGridView != null)
                    {
                        if (this.RowIndex != -1)
                        {
                            this.DataGridView.InvalidateCell(this);
                        }
                        else
                        {
                            this.DataGridView.InvalidateColumnInternal(this.ColumnIndex);
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
                if (value != this.DisplayStyle)
                {
                    this.Properties.SetInteger(PropComboBoxCellDisplayStyle, (int)value);
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.DisplayStyleForCurrentCellOnly"]/*' />
        [DefaultValue(false)]
        public bool DisplayStyleForCurrentCellOnly
        {
            get
            {
                bool found;
                int displayStyleForCurrentCellOnly = this.Properties.GetInteger(PropComboBoxCellDisplayStyleForCurrentCellOnly, out found);
                if (found)
                {
                    return displayStyleForCurrentCellOnly == 0 ? false : true;
                }
                return false;
            }
            set
            {
                if (value != this.DisplayStyleForCurrentCellOnly)
                {
                    this.Properties.SetInteger(PropComboBoxCellDisplayStyleForCurrentCellOnly, value ? 1 : 0);
                    if (this.DataGridView != null)
                    {
                        if (this.RowIndex != -1)
                        {
                            this.DataGridView.InvalidateCell(this);
                        }
                        else
                        {
                            this.DataGridView.InvalidateColumnInternal(this.ColumnIndex);
                        }
                    }
                }
            }
        }

        internal bool DisplayStyleForCurrentCellOnlyInternal
        {
            set
            {
                if (value != this.DisplayStyleForCurrentCellOnly)
                {
                    this.Properties.SetInteger(PropComboBoxCellDisplayStyleForCurrentCellOnly, value ? 1 : 0);
                }
            }
        }

        private Type DisplayType
        {
            get
            {
                if (this.DisplayMemberProperty != null)
                {
                    return this.DisplayMemberProperty.PropertyType;
                }
                else if (this.ValueMemberProperty != null)
                {
                    return this.ValueMemberProperty.PropertyType;
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
                if (this.DataGridView != null)
                {
                    return this.DataGridView.GetCachedTypeConverter(this.DisplayType);
                }
                else
                {
                    return TypeDescriptor.GetConverter(this.DisplayType);
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.DropDownWidth"]/*' />
        [DefaultValue(1)]
        public virtual int DropDownWidth
        {
            get
            {
                bool found;
                int dropDownWidth = this.Properties.GetInteger(PropComboBoxCellDropDownWidth, out found);
                return found ? dropDownWidth : 1;
            }
            set
            {
                //CheckNoSharedCell();
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(DropDownWidth), value, string.Format(SR.DataGridViewComboBoxCell_DropDownWidthOutOfRange, (1).ToString(CultureInfo.CurrentCulture)));
                }
                this.Properties.SetInteger(PropComboBoxCellDropDownWidth, (int)value);
                if (OwnsEditingComboBox(this.RowIndex))
                {
                    this.EditingComboBox.DropDownWidth = value;
                }
            }
        }

        private DataGridViewComboBoxEditingControl EditingComboBox
        {
            get
            {
                return (DataGridViewComboBoxEditingControl)this.Properties.GetObject(PropComboBoxCellEditingComboBox);
            }
            set
            {
                if (value != null || this.Properties.ContainsObject(PropComboBoxCellEditingComboBox))
                {
                    this.Properties.SetObject(PropComboBoxCellEditingComboBox, value);
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.EditType"]/*' />
        public override Type EditType
        {
            get
            {
                return defaultEditType;
            }
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.FlatStyle"]/*' />
        [DefaultValue(FlatStyle.Standard)]
        public FlatStyle FlatStyle
        {
            get
            {
                bool found;
                int flatStyle = this.Properties.GetInteger(PropComboBoxCellFlatStyle, out found);
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
                if (value != this.FlatStyle)
                {
                    this.Properties.SetInteger(PropComboBoxCellFlatStyle, (int)value);
                    OnCommonChange();
                }
            }
        }

        internal FlatStyle FlatStyleInternal
        {
            set
            {
                Debug.Assert(value >= FlatStyle.Flat && value <= FlatStyle.System);
                if (value != this.FlatStyle)
                {
                    this.Properties.SetInteger(PropComboBoxCellFlatStyle, (int)value);
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.FormattedValueType"]/*' />
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
                return this.Properties.ContainsObject(PropComboBoxCellItems) && this.Properties.GetObject(PropComboBoxCellItems) != null;
            }
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.Items"]/*' />
        [Browsable(false)]
        public virtual ObjectCollection Items
        {
            get
            {
                return GetItems(this.DataGridView);
            }
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.MaxDropDownItems"]/*' />
        [DefaultValue(DATAGRIDVIEWCOMBOBOXCELL_defaultMaxDropDownItems)]
        public virtual int MaxDropDownItems
        {
            get
            {
                bool found;
                int maxDropDownItems = this.Properties.GetInteger(PropComboBoxCellMaxDropDownItems, out found);
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
                    throw new ArgumentOutOfRangeException(nameof(MaxDropDownItems), value, string.Format(SR.DataGridViewComboBoxCell_MaxDropDownItemsOutOfRange, (1).ToString(CultureInfo.CurrentCulture), (100).ToString(CultureInfo.CurrentCulture)));
                }
                this.Properties.SetInteger(PropComboBoxCellMaxDropDownItems, (int)value);
                if (OwnsEditingComboBox(this.RowIndex))
                {
                    this.EditingComboBox.MaxDropDownItems = value;
                }
            }
        }

        private bool PaintXPThemes
        {
            get
            {
                bool paintFlat = this.FlatStyle == FlatStyle.Flat || this.FlatStyle == FlatStyle.Popup;
                return !paintFlat && this.DataGridView.ApplyVisualStylesToInnerCells;
            }
        }
        
        private static bool PostXPThemesExist
        {
            get
            {
                return VisualStyleRenderer.IsElementDefined(VisualStyleElement.ComboBox.ReadOnlyButton.Normal);
            }
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.Sorted"]/*' />
        [DefaultValue(false)]
        public virtual bool Sorted
        {
            get
            {
                return ((this.flags & DATAGRIDVIEWCOMBOBOXCELL_sorted) != 0x00);
            }
            set
            {
                //CheckNoSharedCell();
                if (value != this.Sorted)
                {
                    if (value)
                    {
                        if (this.DataSource == null)
                        {
                            this.Items.SortInternal();
                        }
                        else
                        {
                            throw new ArgumentException(SR.ComboBoxSortWithDataSource);
                        }
                        this.flags |= (byte)DATAGRIDVIEWCOMBOBOXCELL_sorted;
                    }
                    else
                    {
                        this.flags = (byte)(this.flags & ~DATAGRIDVIEWCOMBOBOXCELL_sorted);
                    }
                    if (OwnsEditingComboBox(this.RowIndex))
                    {
                        this.EditingComboBox.Sorted = value;
                    }
                }
            }
        }

        internal DataGridViewComboBoxColumn TemplateComboBoxColumn
        {
            get
            {
                return (DataGridViewComboBoxColumn) this.Properties.GetObject(PropComboBoxCellColumnTemplate);
            }
            set
            {
                this.Properties.SetObject(PropComboBoxCellColumnTemplate, value);
            }
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ValueMember"]/*' />
        [DefaultValue("")]
        public virtual string ValueMember
        {
            get
            {
                object valueMember = this.Properties.GetObject(PropComboBoxCellValueMember);
                if (valueMember == null)
                {
                    return String.Empty;
                }
                else
                {
                    return (string)valueMember;
                }
            }
            set
            {
                //CheckNoSharedCell();
                this.ValueMemberInternal = value;
                if (OwnsEditingComboBox(this.RowIndex))
                {
                    this.EditingComboBox.ValueMember = value;
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
                if ((value != null && value.Length > 0) || this.Properties.ContainsObject(PropComboBoxCellValueMember))
                {
                    this.Properties.SetObject(PropComboBoxCellValueMember, value);
                }
            }
        }

        private PropertyDescriptor ValueMemberProperty
        {
            get
            {
                return (PropertyDescriptor)this.Properties.GetObject(PropComboBoxCellValueMemberProp);
            }
            set
            {
                if (value != null || this.Properties.ContainsObject(PropComboBoxCellValueMemberProp))
                {
                    this.Properties.SetObject(PropComboBoxCellValueMemberProp, value);
                }
            }
        }
        
     
        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ValueType"]/*' />
        public override Type ValueType
        {
            get
            {
                if (this.ValueMemberProperty != null)
                {
                    return this.ValueMemberProperty.PropertyType;
                }
                else if (this.DisplayMemberProperty != null)
                {
                    return this.DisplayMemberProperty.PropertyType;
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
            this.EditingComboBox = this.DataGridView.EditingControl as DataGridViewComboBoxEditingControl;
        }

        private void CheckDropDownList(int x, int y, int rowIndex)
        {
            Debug.Assert(this.EditingComboBox != null);
            DataGridViewAdvancedBorderStyle dgvabsPlaceholder = new DataGridViewAdvancedBorderStyle(), dgvabsEffective;
            dgvabsEffective = AdjustCellBorderStyle(this.DataGridView.AdvancedCellBorderStyle,
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
                if (this.DataGridView.RightToLeftInternal)
                {
                    if (x >= borderAndPaddingWidths.X + 1 && 
                        x <= borderAndPaddingWidths.X + dropWidth + 1)
                    {
                        this.EditingComboBox.DroppedDown = true;
                    }
                }
                else
                {
                    if (x >= size.Width - borderAndPaddingWidths.Width - dropWidth - 1 && 
                        x <= size.Width - borderAndPaddingWidths.Width - 1)
                    {
                        this.EditingComboBox.DroppedDown = true;
                    }
                }
            }
        }

        private void CheckNoDataSource()
        {
            if (this.DataSource != null)
            {
                throw new ArgumentException(SR.DataSourceLocksItems);
            }
        }

        //private void CheckNoSharedCell()
        //{
        //    if (this.DataGridView != null && this.RowIndex == -1)
        //    {
        //        throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidOperationOnSharedCell));
        //    }
        //}
        
        private void ComboBox_DropDown(object sender, EventArgs e)
        {
            Debug.Assert(this.DataGridView != null);
            Debug.Assert(this.EditingComboBox != null);
            
            ComboBox comboBox = this.EditingComboBox;
            DataGridViewComboBoxColumn owningComboBoxColumn = this.OwningColumn as DataGridViewComboBoxColumn;
            if (owningComboBoxColumn != null)
            {
                DataGridViewAutoSizeColumnMode autoSizeColumnMode = owningComboBoxColumn.GetInheritedAutoSizeMode(this.DataGridView);
                if (autoSizeColumnMode != DataGridViewAutoSizeColumnMode.ColumnHeader &&
                    autoSizeColumnMode != DataGridViewAutoSizeColumnMode.Fill &&
                    autoSizeColumnMode != DataGridViewAutoSizeColumnMode.None)
                {
                    if (this.DropDownWidth == 1)
                    {
                        // Owning combobox column is autosized based on inner cells.
                        // Resize the dropdown list based on the max width of the items.
                        if (cachedDropDownWidth == -1)
                        {
                            int maxPreferredWidth = -1;
                            if ((this.HasItems || this.CreateItemsFromDataSource) && this.Items.Count > 0)
                            {
                                foreach (object item in this.Items)
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
                    int dropDownWidth = unchecked( (int) (long)UnsafeNativeMethods.SendMessage(new HandleRef(comboBox, comboBox.Handle), NativeMethods.CB_GETDROPPEDWIDTH, 0, 0));
                    if (dropDownWidth != this.DropDownWidth)
                    {
                        UnsafeNativeMethods.SendMessage(new HandleRef(comboBox, comboBox.Handle), NativeMethods.CB_SETDROPPEDWIDTH, this.DropDownWidth, 0);
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.Clone"]/*' />
        public override object Clone()
        {
            DataGridViewComboBoxCell dataGridViewCell;
            Type thisType = this.GetType();

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
            dataGridViewCell.DropDownWidth = this.DropDownWidth;
            dataGridViewCell.MaxDropDownItems = this.MaxDropDownItems;
            dataGridViewCell.CreateItemsFromDataSource = false;
            dataGridViewCell.DataSource = this.DataSource;
            dataGridViewCell.DisplayMember = this.DisplayMember;
            dataGridViewCell.ValueMember = this.ValueMember;
            if (this.HasItems && this.DataSource == null && this.Items.Count > 0)
            {
                dataGridViewCell.Items.AddRangeInternal(this.Items.InnerArray.ToArray());
            }
            dataGridViewCell.AutoComplete = this.AutoComplete;
            dataGridViewCell.Sorted = this.Sorted;
            dataGridViewCell.FlatStyleInternal = this.FlatStyle;
            dataGridViewCell.DisplayStyleInternal = this.DisplayStyle;
            dataGridViewCell.DisplayStyleForCurrentCellOnlyInternal = this.DisplayStyleForCurrentCellOnly;
            return dataGridViewCell;
        }

        private bool CreateItemsFromDataSource
        {
            get
            {
                return ((this.flags & DATAGRIDVIEWCOMBOBOXCELL_createItemsFromDataSource) != 0x00);
            }
            set
            {
                if (value)
                {
                    this.flags |= (byte)DATAGRIDVIEWCOMBOBOXCELL_createItemsFromDataSource;
                }
                else
                {
                    this.flags = (byte)(this.flags & ~DATAGRIDVIEWCOMBOBOXCELL_createItemsFromDataSource);
                }
            }
        }

        private void DataSource_Disposed(object sender, EventArgs e)
        {
            Debug.Assert(sender == this.DataSource, "How can we get dispose notification from anything other than our DataSource?");
            this.DataSource = null;
        }

        private void DataSource_Initialized(object sender, EventArgs e)
        {
            Debug.Assert(sender == this.DataSource);
            Debug.Assert(this.DataSource is ISupportInitializeNotification);
            Debug.Assert((this.flags & DATAGRIDVIEWCOMBOBOXCELL_dataSourceInitializedHookedUp) != 0x00);

            ISupportInitializeNotification dsInit = this.DataSource as ISupportInitializeNotification;
            // Unhook the Initialized event.
            if (dsInit != null)
            {
                dsInit.Initialized -= new EventHandler(DataSource_Initialized);
            }

            // The wait is over: DataSource is initialized.
            this.flags = (byte)(this.flags & ~DATAGRIDVIEWCOMBOBOXCELL_dataSourceInitializedHookedUp);

            // Check the DisplayMember and ValueMember values - will throw if values don't match existing fields.
            InitializeDisplayMemberPropertyDescriptor(this.DisplayMember);
            InitializeValueMemberPropertyDescriptor(this.ValueMember);
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.DetachEditingControl"]/*' />
        public override void DetachEditingControl()
        {
            DataGridView dgv = this.DataGridView;
            if (dgv == null || dgv.EditingControl == null)
            {
                throw new InvalidOperationException();
            }

            if (this.EditingComboBox != null &&
                (this.flags & DATAGRIDVIEWCOMBOBOXCELL_dropDownHookedUp) != 0x00)
            {
                this.EditingComboBox.DropDown -= new EventHandler(ComboBox_DropDown);
                this.flags = (byte)(this.flags & ~DATAGRIDVIEWCOMBOBOXCELL_dropDownHookedUp);
            }

            this.EditingComboBox = null;
            base.DetachEditingControl();
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.GetContentBounds"]/*' />
        protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (this.DataGridView == null || rowIndex < 0 || this.OwningColumn == null)
            {
                return Rectangle.Empty;
            }

            object value = GetValue(rowIndex);
            object formattedValue = GetEditedFormattedValue(value, rowIndex, ref cellStyle, DataGridViewDataErrorContexts.Formatting);

            DataGridViewAdvancedBorderStyle dgvabsEffective;
            DataGridViewElementStates cellState;
            Rectangle cellBounds;

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out dgvabsEffective, out cellState, out cellBounds);

            Rectangle dropDownButtonRect;
            Rectangle contentBounds = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                formattedValue,
                null /*errorText*/,             // contentBounds is independent of errorText
                cellStyle,
                dgvabsEffective,
                out dropDownButtonRect,         // not used
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
            CurrencyManager cm = (CurrencyManager)this.Properties.GetObject(PropComboBoxCellDataManager);
            if (cm == null && this.DataSource != null && dataGridView != null && dataGridView.BindingContext != null && !(this.DataSource == Convert.DBNull))
            {
                ISupportInitializeNotification dsInit = this.DataSource as ISupportInitializeNotification;
                if (dsInit != null && !dsInit.IsInitialized)
                {
                    if ((this.flags & DATAGRIDVIEWCOMBOBOXCELL_dataSourceInitializedHookedUp) == 0x00)
                    {
                        dsInit.Initialized += new EventHandler(DataSource_Initialized);
                        this.flags |= (byte)DATAGRIDVIEWCOMBOBOXCELL_dataSourceInitializedHookedUp;
                    }
                }
                else
                {
                    cm = (CurrencyManager)dataGridView.BindingContext[this.DataSource];
                    this.DataManager = cm;
                }
            }
            return cm;
        }

        [
            SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // Hard coded space is OK here.
        ]
        private int GetDropDownButtonHeight(Graphics graphics, DataGridViewCellStyle cellStyle)
        {
            int adjustment = 4;
            if (this.PaintXPThemes)
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
            return DataGridViewCell.MeasureTextHeight(graphics, " ", cellStyle.Font, System.Int32.MaxValue, TextFormatFlags.Default) + adjustment;
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.GetErrorIconBounds"]/*' />
        protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (this.DataGridView == null ||
                rowIndex < 0 ||
                this.OwningColumn == null ||
                !this.DataGridView.ShowCellErrors ||
                String.IsNullOrEmpty(GetErrorText(rowIndex)))
            {
                return Rectangle.Empty;
            }

            object value = GetValue(rowIndex);
            object formattedValue = GetEditedFormattedValue(value, rowIndex, ref cellStyle, DataGridViewDataErrorContexts.Formatting);

            DataGridViewAdvancedBorderStyle dgvabsEffective;
            DataGridViewElementStates cellState;
            Rectangle cellBounds;

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out dgvabsEffective, out cellState, out cellBounds);

            Rectangle dropDownButtonRect;
            Rectangle errorIconBounds = PaintPrivate(graphics,
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

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.GetFormattedValue"]/*' />
        [
            SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")   // OK to cast value into String twice.
        ]
        protected override object GetFormattedValue(object value,
                                                    int rowIndex,
                                                    ref DataGridViewCellStyle cellStyle,
                                                    TypeConverter valueTypeConverter,
                                                    TypeConverter formattedValueTypeConverter,
                                                    DataGridViewDataErrorContexts context)
        {
            if (valueTypeConverter == null)
            {
                if (this.ValueMemberProperty != null)
                {
                    valueTypeConverter = this.ValueMemberProperty.Converter;
                }
                else if (this.DisplayMemberProperty != null)
                {
                    valueTypeConverter = this.DisplayMemberProperty.Converter;
                }
            }

            if (value == null || ((this.ValueType != null && !this.ValueType.IsAssignableFrom(value.GetType())) && value != System.DBNull.Value))
            {
                // Do not raise the DataError event if the value is null and the row is the 'new row'.
                
                if (value == null /* && ((this.DataGridView != null && rowIndex == this.DataGridView.NewRowIndex) || this.Items.Count == 0)*/)
                {
                    // Debug.Assert(rowIndex != -1 || this.Items.Count == 0);
                    return base.GetFormattedValue(null, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
                }
                if (this.DataGridView != null)
                {
                    DataGridViewDataErrorEventArgs dgvdee = new DataGridViewDataErrorEventArgs(
                        new FormatException(string.Format(SR.DataGridViewComboBoxCell_InvalidValue)), this.ColumnIndex,
                        rowIndex, context);
                    RaiseDataError(dgvdee);
                    if (dgvdee.ThrowException)
                    {
                        throw dgvdee.Exception;
                    }
                }
                return base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
            }

            String strValue = value as String;
            if ((this.DataManager != null && (this.ValueMemberProperty != null || this.DisplayMemberProperty != null)) ||
                !string.IsNullOrEmpty(this.ValueMember) || !string.IsNullOrEmpty(this.DisplayMember))
            {
                object displayValue;
                if (!LookupDisplayValue(rowIndex, value, out displayValue))
                {
                    if (value == System.DBNull.Value)
                    {
                        displayValue = System.DBNull.Value;
                    }
                    else if (strValue != null && String.IsNullOrEmpty(strValue) && this.DisplayType == typeof(String))
                    {
                        displayValue = String.Empty;
                    }
                    else if (this.DataGridView != null)
                    {
                        DataGridViewDataErrorEventArgs dgvdee = new DataGridViewDataErrorEventArgs(
                            new ArgumentException(string.Format(SR.DataGridViewComboBoxCell_InvalidValue)), this.ColumnIndex,
                            rowIndex, context);
                        RaiseDataError(dgvdee);
                        if (dgvdee.ThrowException)
                        {
                            throw dgvdee.Exception;
                        }

                        if (OwnsEditingComboBox(rowIndex))
                        {
                            ((IDataGridViewEditingControl)this.EditingComboBox).EditingControlValueChanged = true;
                            this.DataGridView.NotifyCurrentCellDirty(true);
                        }
                    }
                }
                return base.GetFormattedValue(displayValue, rowIndex, ref cellStyle, this.DisplayTypeConverter, formattedValueTypeConverter, context);
            }
            else
            {
                if (!this.Items.Contains(value) && 
                    value != System.DBNull.Value &&
                    (!(value is String) || !String.IsNullOrEmpty(strValue)))
                {
                    if (this.DataGridView != null)
                    {
                        DataGridViewDataErrorEventArgs dgvdee = new DataGridViewDataErrorEventArgs(
                            new ArgumentException(string.Format(SR.DataGridViewComboBoxCell_InvalidValue)), this.ColumnIndex,
                            rowIndex, context);
                        RaiseDataError(dgvdee);
                        if (dgvdee.ThrowException)
                        {
                            throw dgvdee.Exception;
                        }
                    }

                    if (this.Items.Count > 0)
                    {
                        value = this.Items[0];
                    }
                    else
                    {
                        value = String.Empty;
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
            if (this.DisplayMemberProperty != null)
            {
                displayValue = this.DisplayMemberProperty.GetValue(item);
                displayValueSet = true;
            }
            else if (this.ValueMemberProperty != null)
            {
                displayValue = this.ValueMemberProperty.GetValue(item);
                displayValueSet = true;
            }
            else if (!string.IsNullOrEmpty(this.DisplayMember))
            {
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(item).Find(this.DisplayMember, true /*caseInsensitive*/);
                if (propDesc != null)
                {
                    displayValue = propDesc.GetValue(item);
                    displayValueSet = true;
                }
            }
            else if (!string.IsNullOrEmpty(this.ValueMember))
            {
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(item).Find(this.ValueMember, true /*caseInsensitive*/);
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
            ObjectCollection items = (ObjectCollection)this.Properties.GetObject(PropComboBoxCellItems);
            if (items == null)
            {
                items = new ObjectCollection(this);
                this.Properties.SetObject(PropComboBoxCellItems, items);
            }
            if (this.CreateItemsFromDataSource)
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
                if (dataManager != null || (this.flags & DATAGRIDVIEWCOMBOBOXCELL_dataSourceInitializedHookedUp) == 0x00)
                {
                    this.CreateItemsFromDataSource = false;
                }
            }
            return items;
        }

        internal object GetItemValue(object item)
        {
            bool valueSet = false;
            object value = null;
            if (this.ValueMemberProperty != null)
            {
                value = this.ValueMemberProperty.GetValue(item);
                valueSet = true;
            }
            else if (this.DisplayMemberProperty != null)
            {
                value = this.DisplayMemberProperty.GetValue(item);
                valueSet = true;
            }
            else if (!string.IsNullOrEmpty(this.ValueMember))
            {
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(item).Find(this.ValueMember, true /*caseInsensitive*/);
                if (propDesc != null)
                {
                    value = propDesc.GetValue(item);
                    valueSet = true;
                }
            }
            if (!valueSet && !string.IsNullOrEmpty(this.DisplayMember))
            {
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(item).Find(this.DisplayMember, true /*caseInsensitive*/);
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

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.GetPreferredSize"]/*' />
        [
            SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // Hard coded space is OK here.
        ]
        protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
        {
            if (this.DataGridView == null)
            {
                return new Size(-1, -1);
            }

            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            Size preferredSize = Size.Empty;
            DataGridViewFreeDimension freeDimension = DataGridViewCell.GetFreeDimensionFromConstraint(constraintSize);
            Rectangle borderWidthsRect = this.StdBorderWidths;
            int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
            int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;
            TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(this.DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);

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
                if (this.DataGridView.ShowCellErrors)
                {
                    // Making sure that there is enough room for the potential error icon
                    preferredSize.Width = Math.Max(preferredSize.Width, borderAndPaddingWidths + SystemInformation.HorizontalScrollBarThumbWidth + 1 + DATAGRIDVIEWCELL_iconMarginWidth * 2 + iconsWidth);
                }
            }
            if (freeDimension != DataGridViewFreeDimension.Width)
            {
                if (this.FlatStyle == FlatStyle.Flat || this.FlatStyle == FlatStyle.Popup)
                {
                    preferredSize.Height += 6;
                }
                else
                {
                    preferredSize.Height += 8;
                }
                preferredSize.Height += borderAndPaddingHeights;
                if (this.DataGridView.ShowCellErrors)
                {
                    // Making sure that there is enough room for the potential error icon
                    preferredSize.Height = Math.Max(preferredSize.Height, borderAndPaddingHeights + DATAGRIDVIEWCELL_iconMarginHeight * 2 + iconsHeight);
                }
            }
            return preferredSize;
        }

        private void InitializeComboBoxText()
        {
            Debug.Assert(this.EditingComboBox != null);
            ((IDataGridViewEditingControl)this.EditingComboBox).EditingControlValueChanged = false;
            int rowIndex = ((IDataGridViewEditingControl)this.EditingComboBox).EditingControlRowIndex;
            Debug.Assert(rowIndex > -1);
            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false);
            this.EditingComboBox.Text = (string) GetFormattedValue(GetValue(rowIndex), rowIndex, ref dataGridViewCellStyle, null, null, DataGridViewDataErrorContexts.Formatting);
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.InitializeEditingControl"]/*' />
        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            Debug.Assert(this.DataGridView != null && 
                         this.DataGridView.EditingPanel != null && 
                         this.DataGridView.EditingControl != null);
            Debug.Assert(!this.ReadOnly);
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            ComboBox comboBox = this.DataGridView.EditingControl as ComboBox;
            if (comboBox != null)
            {
                // Use the selection backcolor for the editing panel when the cell is selected
                if ((GetInheritedState(rowIndex) & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
                {
                    this.DataGridView.EditingPanel.BackColor = dataGridViewCellStyle.SelectionBackColor;
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
                comboBox.MaxDropDownItems = this.MaxDropDownItems;
                comboBox.DropDownWidth = this.DropDownWidth;
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

                comboBox.DataSource = this.DataSource;
                comboBox.DisplayMember = this.DisplayMember;
                comboBox.ValueMember = this.ValueMember;
                if (this.HasItems && this.DataSource == null && this.Items.Count > 0)
                {
                    comboBox.Items.AddRange(this.Items.InnerArray.ToArray());
                }
                comboBox.Sorted = this.Sorted;
                comboBox.FlatStyle = this.FlatStyle;
                if (this.AutoComplete)
                {
                    comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
                    comboBox.AutoCompleteMode = AutoCompleteMode.Append;
                }
                else
                {
                    comboBox.AutoCompleteMode = AutoCompleteMode.None;
                    comboBox.AutoCompleteSource = AutoCompleteSource.None;
                }

                string initialFormattedValueStr = initialFormattedValue as string;
                if (initialFormattedValueStr == null)
                {
                    initialFormattedValueStr = string.Empty;
                }
                comboBox.Text = initialFormattedValueStr;

                if ((this.flags & DATAGRIDVIEWCOMBOBOXCELL_dropDownHookedUp) == 0x00)
                {
                    comboBox.DropDown += new EventHandler(ComboBox_DropDown);
                    this.flags |= (byte)DATAGRIDVIEWCOMBOBOXCELL_dropDownHookedUp;
                }
                cachedDropDownWidth = -1;

                this.EditingComboBox = this.DataGridView.EditingControl as DataGridViewComboBoxEditingControl;
                if (GetHeight(rowIndex) > 21)
                {
                    Rectangle rectBottomSection = this.DataGridView.GetCellDisplayRectangle(this.ColumnIndex, rowIndex, true);
                    rectBottomSection.Y += 21;
                    rectBottomSection.Height -= 21;
                    this.DataGridView.Invalidate(rectBottomSection);
                }
            }
        }

        private void InitializeDisplayMemberPropertyDescriptor(string displayMember)
        {
            if (this.DataManager != null)
            {
                if (String.IsNullOrEmpty(displayMember))
                {
                    this.DisplayMemberProperty = null;
                } 
                else
                {
                    BindingMemberInfo displayBindingMember = new BindingMemberInfo(displayMember);
                    // make the DataManager point to the sublist inside this.DataSource
                    this.DataManager = this.DataGridView.BindingContext[this.DataSource, displayBindingMember.BindingPath] as CurrencyManager;

                    PropertyDescriptorCollection props = this.DataManager.GetItemProperties();
                    PropertyDescriptor displayMemberProperty = props.Find(displayBindingMember.BindingField, true);
                    if (displayMemberProperty == null)
                    {
                        throw new ArgumentException(string.Format(SR.DataGridViewComboBoxCell_FieldNotFound, displayMember));
                    }
                    else
                    {
                        this.DisplayMemberProperty = displayMemberProperty;
                    }
                } 
            }
        }

        private void InitializeValueMemberPropertyDescriptor(string valueMember)
        {
            if (this.DataManager != null)
            {
                if (String.IsNullOrEmpty(valueMember))
                {
                    this.ValueMemberProperty = null;
                } 
                else
                {
                    BindingMemberInfo valueBindingMember = new BindingMemberInfo(valueMember);
                    // make the DataManager point to the sublist inside this.DataSource
                    this.DataManager = this.DataGridView.BindingContext[this.DataSource, valueBindingMember.BindingPath] as CurrencyManager;

                    PropertyDescriptorCollection props = this.DataManager.GetItemProperties();
                    PropertyDescriptor valueMemberProperty = props.Find(valueBindingMember.BindingField, true);
                    if (valueMemberProperty == null)
                    {
                        throw new ArgumentException(string.Format(SR.DataGridViewComboBoxCell_FieldNotFound, valueMember));
                    }
                    else
                    {
                        this.ValueMemberProperty = valueMemberProperty;
                    }
                }
            }
        }

        /// <summary>
        ///     Find the item in the ComboBox currency manager for the current cell
        ///     This can be horribly inefficient and it uses reflection which makes it expensive 
        ///     - ripe for optimization
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
            Debug.Assert(this.DataManager != null);
            object item = null;

            //If the data source is a bindinglist use that as it's probably more efficient
            if ((this.DataManager.List is IBindingList) && ((IBindingList)this.DataManager.List).SupportsSearching)
            {
                int index = ((IBindingList)this.DataManager.List).Find(property, key);
                if (index != -1)
                {
                    item = this.DataManager.List[index];
                }
            }
            else
            {
                //Otherwise walk across the items looking for the item we want 
                for (int i = 0; i < this.DataManager.List.Count; i++)
                {
                    object itemTmp = this.DataManager.List[i];
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
                item = this.EditingComboBox.SelectedItem;
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
                foreach (object itemCandidate in this.Items)
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
                    item = this.EditingComboBox.SelectedItem;
                    if (item == null || !item.Equals(key))
                    {
                        item = null;
                    }
                }
                if (item == null && this.Items.Contains(key))
                {
                    item = key;
                }
            }
            return item;
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.KeyEntersEditMode"]/*' />
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
        ///     Lookup the display text for the given value.
        ///     
        ///     We use the value and ValueMember to look up the item in the 
        ///     ComboBox datasource. We then use DisplayMember to get the 
        ///     text to display.
        /// </summary>
        private bool LookupDisplayValue(int rowIndex, object value, out object displayValue)
        {
            Debug.Assert(value != null);
            Debug.Assert(this.ValueMemberProperty != null || this.DisplayMemberProperty != null ||
                         !string.IsNullOrEmpty(this.ValueMember) || !string.IsNullOrEmpty(this.DisplayMember));

            object item = null;
            if (this.DisplayMemberProperty != null || this.ValueMemberProperty != null)
            {
                //Now look up the item in the Combobox datasource - this can be horribly inefficient
                //and it uses reflection which makes it expensive - ripe for optimization
                item = this.ItemFromComboBoxDataSource(this.ValueMemberProperty != null ? this.ValueMemberProperty : this.DisplayMemberProperty, value);
            }
            else
            {
                //Find the item in the Items collection based on the provided ValueMember or DisplayMember
                item = ItemFromComboBoxItems(rowIndex, string.IsNullOrEmpty(this.ValueMember) ? this.DisplayMember : this.ValueMember, value);
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
        ///     Lookup the value for the given display value.
        ///     
        ///     We use the display value and DisplayMember to look up the item in the 
        ///     ComboBox datasource. We then use ValueMember to get the value.
        /// </summary>
        private bool LookupValue(object formattedValue, out object value)
        {
            if (formattedValue == null)
            {
                value = null;
                return true;
            }

            Debug.Assert(this.DisplayMemberProperty != null || this.ValueMemberProperty != null ||
                         !string.IsNullOrEmpty(this.DisplayMember) || !string.IsNullOrEmpty(this.ValueMember));

            object item = null;
            if (this.DisplayMemberProperty != null || this.ValueMemberProperty != null)
            {
                //Now look up the item in the DataGridViewComboboxCell datasource - this can be horribly inefficient
                //and it uses reflection which makes it expensive - ripe for optimization
                item = ItemFromComboBoxDataSource(this.DisplayMemberProperty != null ? this.DisplayMemberProperty : this.ValueMemberProperty, formattedValue);
            }
            else
            {
                //Find the item in the Items collection based on the provided DisplayMember or ValueMember
                item = ItemFromComboBoxItems(this.RowIndex, string.IsNullOrEmpty(this.DisplayMember) ? this.ValueMember : this.DisplayMember, formattedValue);
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

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.OnDataGridViewChanged"]/*' />
        protected override void OnDataGridViewChanged()
        {
            if (this.DataGridView != null)
            {
                // Will throw an error if DataGridView is set and a member is invalid
                InitializeDisplayMemberPropertyDescriptor(this.DisplayMember);
                InitializeValueMemberPropertyDescriptor(this.ValueMember);
            }
            base.OnDataGridViewChanged();
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.OnEnter"]/*' />
        protected override void OnEnter(int rowIndex, bool throughMouseClick)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            if (throughMouseClick && this.DataGridView.EditMode != DataGridViewEditMode.EditOnEnter)
            {
                this.flags |= (byte)DATAGRIDVIEWCOMBOBOXCELL_ignoreNextMouseClick;
            }
        }

        private void OnItemsCollectionChanged()
        {
            if (this.TemplateComboBoxColumn != null)
            {
                Debug.Assert(this.TemplateComboBoxColumn.CellTemplate == this);
                this.TemplateComboBoxColumn.OnItemsCollectionChanged();
            }
            cachedDropDownWidth = -1;
            if (OwnsEditingComboBox(this.RowIndex))
            {
                InitializeComboBoxText();
            }
            else
            {
                OnCommonChange();
            }
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.OnLeave"]/*' />
        protected override void OnLeave(int rowIndex, bool throughMouseClick)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            this.flags = (byte)(this.flags & ~DATAGRIDVIEWCOMBOBOXCELL_ignoreNextMouseClick);
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.OnMouseClick"]/*' />
        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            Debug.Assert(e.ColumnIndex == this.ColumnIndex);
            Point ptCurrentCell = this.DataGridView.CurrentCellAddress;
            if (ptCurrentCell.X == e.ColumnIndex && ptCurrentCell.Y == e.RowIndex)
            {
                if ((this.flags & DATAGRIDVIEWCOMBOBOXCELL_ignoreNextMouseClick) != 0x00)
                {
                    this.flags = (byte)(this.flags & ~DATAGRIDVIEWCOMBOBOXCELL_ignoreNextMouseClick);
                }
                else if ((this.EditingComboBox == null || !this.EditingComboBox.DroppedDown) &&
                         this.DataGridView.EditMode != DataGridViewEditMode.EditProgrammatically &&
                         this.DataGridView.BeginEdit(true /*selectAll*/))
                {
                    if (this.EditingComboBox != null && this.DisplayStyle != DataGridViewComboBoxDisplayStyle.Nothing)
                    {
                        CheckDropDownList(e.X, e.Y, e.RowIndex);
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.OnMouseEnter"]/*' />
        protected override void OnMouseEnter(int rowIndex)
        {
            if (this.DataGridView == null)
            {
                return;
            }

            if (this.DisplayStyle == DataGridViewComboBoxDisplayStyle.ComboBox && this.FlatStyle == FlatStyle.Popup)
            {
                this.DataGridView.InvalidateCell(this.ColumnIndex, rowIndex);
            }

            base.OnMouseEnter(rowIndex);
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.OnMouseLeave"]/*' />
        protected override void OnMouseLeave(int rowIndex)
        {
            if (this.DataGridView == null)
            {
                return;
            }

            if (mouseInDropDownButtonBounds)
            {
                mouseInDropDownButtonBounds = false;
                if (this.ColumnIndex >= 0 &&
                    rowIndex >= 0 &&
                    (this.FlatStyle == FlatStyle.Standard || this.FlatStyle == FlatStyle.System) && 
                    this.DataGridView.ApplyVisualStylesToInnerCells)
                {
                    this.DataGridView.InvalidateCell(this.ColumnIndex, rowIndex);
                }
            }
            
            if (this.DisplayStyle == DataGridViewComboBoxDisplayStyle.ComboBox && this.FlatStyle == FlatStyle.Popup)
            {
                this.DataGridView.InvalidateCell(this.ColumnIndex, rowIndex);
            }

            base.OnMouseEnter(rowIndex);
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.OnMouseMove"]/*' />
        protected override void OnMouseMove(DataGridViewCellMouseEventArgs e)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            if ((this.FlatStyle == FlatStyle.Standard || this.FlatStyle == FlatStyle.System) && this.DataGridView.ApplyVisualStylesToInnerCells)
            {
                int rowIndex = e.RowIndex;
                DataGridViewCellStyle cellStyle = GetInheritedStyle(null, rowIndex, false /*includeColors*/);

                // get the border style
                bool singleVerticalBorderAdded = !this.DataGridView.RowHeadersVisible && this.DataGridView.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single;
                bool singleHorizontalBorderAdded = !this.DataGridView.ColumnHeadersVisible && this.DataGridView.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single;
                bool isFirstDisplayedRow = rowIndex == this.DataGridView.FirstDisplayedScrollingRowIndex;
                bool isFirstDisplayedColumn = this.OwningColumn.Index == this.DataGridView.FirstDisplayedColumnIndex;
                bool isFirstDisplayedScrollingColumn = this.OwningColumn.Index == this.DataGridView.FirstDisplayedScrollingColumnIndex;
                DataGridViewAdvancedBorderStyle dgvabsEffective, dgvabsPlaceholder;
                dgvabsPlaceholder = new DataGridViewAdvancedBorderStyle();
                dgvabsEffective = AdjustCellBorderStyle(this.DataGridView.AdvancedCellBorderStyle, dgvabsPlaceholder, 
                                                        singleVerticalBorderAdded,
                                                        singleHorizontalBorderAdded,
                                                        isFirstDisplayedRow,
                                                        isFirstDisplayedColumn);

                Rectangle cellBounds = this.DataGridView.GetCellDisplayRectangle(this.OwningColumn.Index, rowIndex, false /*cutOverflow*/);
                Rectangle cutoffCellBounds = cellBounds;
                if (isFirstDisplayedScrollingColumn)
                {
                    cellBounds.X -= this.DataGridView.FirstDisplayedScrollingColumnHiddenWidth;
                    cellBounds.Width += this.DataGridView.FirstDisplayedScrollingColumnHiddenWidth;
                }

                DataGridViewElementStates rowState = this.DataGridView.Rows.GetRowState(rowIndex);
                DataGridViewElementStates cellState = this.CellStateFromColumnRowStates(rowState);
                cellState |= this.State;

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

                bool newMouseInDropDownButtonBounds = dropDownButtonRect.Contains(this.DataGridView.PointToClient(Control.MousePosition));
                if (newMouseInDropDownButtonBounds != mouseInDropDownButtonBounds)
                {
                    mouseInDropDownButtonBounds = newMouseInDropDownButtonBounds;
                    this.DataGridView.InvalidateCell(e.ColumnIndex, rowIndex);
                }
            }
            base.OnMouseMove(e);
        }

        private bool OwnsEditingComboBox(int rowIndex)
        {
            return rowIndex != -1 && this.EditingComboBox != null && rowIndex == ((IDataGridViewEditingControl)this.EditingComboBox).EditingControlRowIndex;
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.Paint"]/*' />
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

            Rectangle dropDownButtonRect;
            PaintPrivate(graphics, 
                clipBounds,
                cellBounds, 
                rowIndex, 
                elementState,
                formattedValue,
                errorText,
                cellStyle,
                advancedBorderStyle,
                out dropDownButtonRect,     // not used
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

            bool paintFlat = this.FlatStyle == FlatStyle.Flat || this.FlatStyle == FlatStyle.Popup;
            bool paintPopup = this.FlatStyle == FlatStyle.Popup &&
                              this.DataGridView.MouseEnteredCellAddress.Y == rowIndex &&
                              this.DataGridView.MouseEnteredCellAddress.X == this.ColumnIndex;

            bool paintXPThemes = !paintFlat && this.DataGridView.ApplyVisualStylesToInnerCells;
            bool paintPostXPThemes = paintXPThemes && PostXPThemesExist;

            ComboBoxState comboBoxState = ComboBoxState.Normal;
            if (this.DataGridView.MouseEnteredCellAddress.Y == rowIndex &&
                this.DataGridView.MouseEnteredCellAddress.X == this.ColumnIndex &&
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
            Point ptCurrentCell = this.DataGridView.CurrentCellAddress;
            bool cellCurrent = ptCurrentCell.X == this.ColumnIndex && ptCurrentCell.Y == rowIndex;
            bool cellEdited = cellCurrent && this.DataGridView.EditingControl != null;
            bool cellSelected = (elementState & DataGridViewElementStates.Selected) != 0;
            bool drawComboBox = this.DisplayStyle == DataGridViewComboBoxDisplayStyle.ComboBox &&
                                ((this.DisplayStyleForCurrentCellOnly && cellCurrent) || !this.DisplayStyleForCurrentCellOnly);
            bool drawDropDownButton = this.DisplayStyle != DataGridViewComboBoxDisplayStyle.Nothing &&
                                ((this.DisplayStyleForCurrentCellOnly && cellCurrent) || !this.DisplayStyleForCurrentCellOnly);
            if (DataGridViewCell.PaintSelectionBackground(paintParts) && cellSelected && !cellEdited)
            {
                br = this.DataGridView.GetCachedBrush(cellStyle.SelectionBackColor);
            }
            else
            {
                br = this.DataGridView.GetCachedBrush(cellStyle.BackColor);
            }

            if (paint && DataGridViewCell.PaintBackground(paintParts) && br.Color.A == 255 && valBounds.Width > 0 && valBounds.Height > 0)
            {
                DataGridViewCell.PaintPadding(g, valBounds, cellStyle, br, this.DataGridView.RightToLeftInternal);
            }

            if (cellStyle.Padding != Padding.Empty)
            {
                if (this.DataGridView.RightToLeftInternal)
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
                        g.DrawRectangle(SystemPens.ControlLightLight, new Rectangle(valBounds.X, valBounds.Y, valBounds.Width-1, valBounds.Height-1));
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
                            dropRect = new Rectangle(this.DataGridView.RightToLeftInternal ? valBounds.Left : valBounds.Right - dropWidth,
                                                    valBounds.Top,
                                                    dropWidth,
                                                    dropHeight);
                        }
                        else
                        {
                            dropRect = new Rectangle(this.DataGridView.RightToLeftInternal ? valBounds.Left + 1 : valBounds.Right - dropWidth - 1,
                                                    valBounds.Top + 1,
                                                    dropWidth,
                                                    dropHeight);
                        }
                    }
                    else
                    {
                        dropRect = new Rectangle(this.DataGridView.RightToLeftInternal ? valBounds.Left + 2 : valBounds.Right - dropWidth - 2,
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
                                        DataGridViewComboBoxCellRenderer.DrawDropDownButton(g, dropRect, comboBoxState, this.DataGridView.RightToLeftInternal);
                                    }
                                    else
                                    {
                                        DataGridViewComboBoxCellRenderer.DrawReadOnlyButton(g, valBounds, comboBoxState);
                                        DataGridViewComboBoxCellRenderer.DrawDropDownButton(g, dropRect, ComboBoxState.Normal);
                                    }

                                    if (SystemInformation.HighContrast && AccessibilityImprovements.Level1)
                                    {
                                        // In the case of ComboBox style, background is not filled in, 
                                        // in the case of DrawReadOnlyButton uses theming API to render CP_READONLY COMBOBOX part that renders the background,
                                        // this API does not have "selected" state, thus always uses BackColor
                                        br = this.DataGridView.GetCachedBrush(cellStyle.BackColor);
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
                            Color color= SystemColors.Control;
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
                            if (stockColor) {
                                if (SystemInformation.HighContrast) {
                                    pen = SystemPens.ControlLight;
                                }
                                else {
                                    pen = SystemPens.Control;
                                }
                            }
                            else {
                                pen= new Pen(highlight);
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
                            if (stockColor) {
                                pen = SystemPens.ControlDarkDark;
                            }
                            else {
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
                            if (stockColor) {
                                pen = SystemPens.ControlDark;
                            }
                            else {
                                pen.Color = buttonShadow;
                            }
                            if (drawDropDownButton)
                            {
                                g.DrawLine(pen, dropRect.X + 1, dropRect.Y + dropRect.Height - 2,
                                        dropRect.X + dropRect.Width - 2, dropRect.Y + dropRect.Height - 2);
                                g.DrawLine(pen, dropRect.X + dropRect.Width - 2, dropRect.Y + 1,
                                        dropRect.X + dropRect.Width - 2, dropRect.Y + dropRect.Height - 2);
                            }
                            if (!stockColor) {
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
                if (!this.DataGridView.RightToLeftInternal)
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
                    if (this.DataGridView.RightToLeftInternal)
                    {
                        errorBounds.X += dropWidth;
                        textBounds.X += dropWidth;
                    }
                }
                else
                {
                    errorBounds.Width -= dropWidth + 1;
                    textBounds.Width -= dropWidth + 1;
                    if (this.DataGridView.RightToLeftInternal)
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
                    this.DataGridView.ShowFocusCues && 
                    this.DataGridView.Focused && 
                    paint)
                {
                    // Draw focus rectangle
                    if (paintFlat)
                    {
                        Rectangle focusBounds = textBounds;
                        if (!this.DataGridView.RightToLeftInternal)
                        {
                            focusBounds.X--;
                        }
                        focusBounds.Width++;
                        focusBounds.Y--;
                        focusBounds.Height+=2;
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

                string formattedString = formattedValue as string;

                if (formattedString != null)
                {
                    // Font independent margins
                    int verticalTextMarginTop = cellStyle.WrapMode == DataGridViewTriState.True ? DATAGRIDVIEWCOMBOBOXCELL_verticalTextMarginTopWithWrapping : DATAGRIDVIEWCOMBOBOXCELL_verticalTextMarginTopWithoutWrapping;
                    if (this.DataGridView.RightToLeftInternal)
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
                        TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(this.DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
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

                if (this.DataGridView.ShowCellErrors && paint && DataGridViewCell.PaintErrorIcon(paintParts))
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
                if (!String.IsNullOrEmpty(errorText))
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

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ParseFormattedValue"]/*' />
        public override object ParseFormattedValue(object formattedValue,
                                                   DataGridViewCellStyle cellStyle,
                                                   TypeConverter formattedValueTypeConverter,
                                                   TypeConverter valueTypeConverter)
        {
            if (valueTypeConverter == null)
            {
                if (this.ValueMemberProperty != null)
                {
                    valueTypeConverter = this.ValueMemberProperty.Converter;
                }
                else if (this.DisplayMemberProperty != null)
                {
                    valueTypeConverter = this.DisplayMemberProperty.Converter;
                }
            }

            // Find the item given its display value
            if ((this.DataManager != null && 
                (this.DisplayMemberProperty != null || this.ValueMemberProperty != null)) ||
                !string.IsNullOrEmpty(this.DisplayMember) || !string.IsNullOrEmpty(this.ValueMember))
            {
                object value = ParseFormattedValueInternal(this.DisplayType, formattedValue, cellStyle,
                                                           formattedValueTypeConverter, this.DisplayTypeConverter);
                object originalValue = value;
                if (!LookupValue(originalValue, out value))
                {
                    if (originalValue == System.DBNull.Value)
                    {
                        value = System.DBNull.Value;
                    }
                    else
                    {
                        throw new FormatException(String.Format(CultureInfo.CurrentCulture, string.Format(SR.Formatter_CantConvert), value, this.DisplayType));
                    }
                }
                return value;
            }
            else
            {
                return ParseFormattedValueInternal(this.ValueType, formattedValue, cellStyle,
                                                   formattedValueTypeConverter, valueTypeConverter);
            }
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ToString"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the row Index and column Index of the cell.
        ///    </para>
        /// </devdoc>
        public override string ToString() 
        {
            return "DataGridViewComboBoxCell { ColumnIndex=" + this.ColumnIndex.ToString(CultureInfo.CurrentCulture) + ", RowIndex=" + this.RowIndex.ToString(CultureInfo.CurrentCulture) + " }";
        }

        private void UnwireDataSource()
        {
            IComponent component = this.DataSource as IComponent;
            if (component != null) 
            {
                component.Disposed -= new EventHandler(DataSource_Disposed);
            }

            ISupportInitializeNotification dsInit = this.DataSource as ISupportInitializeNotification;
            if (dsInit != null && (this.flags & DATAGRIDVIEWCOMBOBOXCELL_dataSourceInitializedHookedUp) != 0x00)
            {
                // If we previously hooked the data source's ISupportInitializeNotification
                // Initialized event, then unhook it now (we don't always hook this event,
                // only if we needed to because the data source was previously uninitialized)
                dsInit.Initialized -= new EventHandler(DataSource_Initialized);
                this.flags = (byte)(this.flags & ~DATAGRIDVIEWCOMBOBOXCELL_dataSourceInitializedHookedUp);
            }
        }

        private void WireDataSource(object dataSource)
        {
            // If the source is a component, then hook the Disposed event,
            // so we know when the component is deleted from the form
            IComponent component = dataSource as IComponent;
            if (component != null)
            {
                component.Disposed += new EventHandler(DataSource_Disposed);
            }
        }

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ObjectCollection"]/*' />
        /// <devdoc>
        ///     <para>
        ///       A collection that stores objects.
        ///    </para>
        /// </devdoc>
        [ListBindable(false)]
        public class ObjectCollection : IList 
        {
            private DataGridViewComboBoxCell owner;
            private ArrayList items;
            private IComparer comparer;

            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ObjectCollection.ObjectCollection"]/*' />
            public ObjectCollection(DataGridViewComboBoxCell owner) 
            {
                Debug.Assert(owner != null);
                this.owner = owner;
            }

            private IComparer Comparer 
            {
                get 
                {
                    if (this.comparer == null) 
                    {
                        this.comparer = new ItemComparer(this.owner);
                    }
                    return this.comparer;
                }
            }
                        
            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ObjectCollection.Count"]/*' />
            /// <devdoc>
            ///     Retrieves the number of items.
            /// </devdoc>
            public int Count
            {
                get 
                {
                    return this.InnerArray.Count;
                }
            }
            
            /// <devdoc>
            ///     Internal access to the actual data store.
            /// </devdoc>
            internal ArrayList InnerArray 
            {
                get
                {
                    if (this.items == null)
                    {
                        this.items = new ArrayList();
                    }
                    return this.items;
                }
            }

            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="ObjectCollection.ICollection.SyncRoot"]/*' />
            /// <internalonly/>
            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="ObjectCollection.ICollection.IsSynchronized"]/*' />
            /// <internalonly/>
            bool ICollection.IsSynchronized 
            {
                get
                {
                    return false;
                }
            }

            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="ObjectCollection.IList.IsFixedSize"]/*' />
            /// <internalonly/>
            bool IList.IsFixedSize 
            {
                get
                {
                    return false;
                }
            }
            
            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ObjectCollection.IsReadOnly"]/*' />
            public bool IsReadOnly 
            {
                get 
                {
                    return false;
                }
            }
        
            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ObjectCollection.Add"]/*' />
            /// <devdoc>
            ///     Adds an item to the collection. For an unsorted combo box, the item is
            ///     added to the end of the existing list of items. For a sorted combo box,
            ///     the item is inserted into the list according to its sorted position.
            ///     The item's ToString() method is called to obtain the string that is
            ///     displayed in the combo box.
            /// </devdoc>
            public int Add(object item) 
            {
                //this.owner.CheckNoSharedCell();
                this.owner.CheckNoDataSource();

                if (item == null) 
                {
                    throw new ArgumentNullException(nameof(item));
                }
                
                int index = this.InnerArray.Add(item);
                
                bool success = false;
                if (this.owner.Sorted) 
                {
                    try 
                    {
                        this.InnerArray.Sort(this.Comparer);
                        index = this.InnerArray.IndexOf(item);
                        success = true;
                    }
                    finally 
                    {
                        if (!success) 
                        {
                            this.InnerArray.Remove(item);
                        }
                    }
                }

                this.owner.OnItemsCollectionChanged();
                return index;
            }
            
            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="ObjectCollection.IList.Add"]/*' />
            /// <internalonly/>
            int IList.Add(object item) 
            {
                return Add(item);
            }
            
            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ObjectCollection.AddRange"]/*' />
            public void AddRange(params object[] items)
            {
                //this.owner.CheckNoSharedCell();
                this.owner.CheckNoDataSource();
                AddRangeInternal((ICollection)items);
                this.owner.OnItemsCollectionChanged();
            }
            
            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ObjectCollection.AddRange1"]/*' />
            public void AddRange(ObjectCollection value) 
            {
                //this.owner.CheckNoSharedCell();
                this.owner.CheckNoDataSource();
                AddRangeInternal((ICollection) value);
                this.owner.OnItemsCollectionChanged();
            }
            
            /// <devdoc>
            ///     Add range that bypasses the data source check.
            /// </devdoc>
            internal void AddRangeInternal(ICollection items) 
            {
                if (items == null)
                {
                    throw new ArgumentNullException(nameof(items));
                }
                
                foreach(object item in items)
                {
                    if (item == null)
                    {
                        throw new InvalidOperationException(SR.InvalidNullItemInCollection);
                    }
                }

                // Add everything to the collection first, then sort
                this.InnerArray.AddRange(items);
                if (this.owner.Sorted)
                {
                    this.InnerArray.Sort(this.Comparer);
                }
            }

            internal void SortInternal()
            {
                this.InnerArray.Sort(this.Comparer);
            }

            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ObjectCollection.this"]/*' />
            /// <devdoc>
            ///     Retrieves the item with the specified index.
            /// </devdoc>
            public virtual object this[int index] 
            {
                get 
                {
                    if (index < 0 || index >= this.InnerArray.Count) 
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                    }
                    return this.InnerArray[index];
                }
                set 
                {
                    //this.owner.CheckNoSharedCell();
                    this.owner.CheckNoDataSource();

                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(value));
                    }

                    if (index < 0 || index >= this.InnerArray.Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                    }

                    this.InnerArray[index] = value;
                    this.owner.OnItemsCollectionChanged();
                }
            }
            
            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ObjectCollection.Clear"]/*' />
            /// <devdoc>
            ///     Removes all items from the collection.
            /// </devdoc>
            public void Clear()
            {
                if (this.InnerArray.Count > 0)
                {
                    //this.owner.CheckNoSharedCell();
                    this.owner.CheckNoDataSource();
                    this.InnerArray.Clear();
                    this.owner.OnItemsCollectionChanged();
                }
            }

            internal void ClearInternal()
            {
                this.InnerArray.Clear();
            }

            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ObjectCollection.Contains"]/*' />
            public bool Contains(object value) 
            {
                return IndexOf(value) != -1;
            }

            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ObjectCollection.CopyTo"]/*' />
            /// <devdoc>
            ///     Copies the DataGridViewComboBoxCell Items collection to a destination array.
            /// </devdoc>
            public void CopyTo(object[] destination, int arrayIndex) 
            {
                int count = this.InnerArray.Count;
                for(int i = 0; i < count; i++)
                {
                    destination[i + arrayIndex] = this.InnerArray[i];
                }
            }

            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="ObjectCollection.ICollection.CopyTo"]/*' />
            /// <internalonly/>
            void ICollection.CopyTo(Array destination, int index)
            {
                int count = this.InnerArray.Count;
                for(int i = 0; i < count; i++) 
                {
                    destination.SetValue(this.InnerArray[i], i + index);
                }
            }

            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ObjectCollection.GetEnumerator"]/*' />
            /// <devdoc>
            ///     Returns an enumerator for the DataGridViewComboBoxCell Items collection.
            /// </devdoc>
            public IEnumerator GetEnumerator() 
            {
                return this.InnerArray.GetEnumerator();
            }
            
            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ObjectCollection.IndexOf"]/*' />
            public int IndexOf(object value) 
            {
                if (value == null) 
                {
                    throw new ArgumentNullException(nameof(value));
                }
                return this.InnerArray.IndexOf(value);
            }

            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ObjectCollection.Insert"]/*' />
            /// <devdoc>
            ///     Adds an item to the collection. For an unsorted combo box, the item is
            ///     added to the end of the existing list of items. For a sorted combo box,
            ///     the item is inserted into the list according to its sorted position.
            ///     The item's toString() method is called to obtain the string that is
            ///     displayed in the combo box.
            /// </devdoc>
            public void Insert(int index, object item) 
            {
                //this.owner.CheckNoSharedCell();
                this.owner.CheckNoDataSource();
                
                if (item == null) 
                {
                    throw new ArgumentNullException(nameof(item));
                }
                
                if (index < 0 || index > this.InnerArray.Count) 
                {
                    throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                }
                
                // If the combo box is sorted, then just treat this like an add
                // because we are going to twiddle the index anyway.
                if (this.owner.Sorted)
                {
                    Add(item);
                }
                else 
                {
                    this.InnerArray.Insert(index, item);
                    this.owner.OnItemsCollectionChanged();
                }
            }
            
            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ObjectCollection.Remove"]/*' />
            /// <devdoc>
            ///     Removes the given item from the collection, provided that it is
            ///     actually in the list.
            /// </devdoc>
            public void Remove(object value) 
            {
                int index = this.InnerArray.IndexOf(value);
                
                if (index != -1) 
                {
                    RemoveAt(index);
                }
            }
        
            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCell.ObjectCollection.RemoveAt"]/*' />
            /// <devdoc>
            ///     Removes an item from the collection at the given index.
            /// </devdoc>
            public void RemoveAt(int index) 
            {
                //this.owner.CheckNoSharedCell();
                this.owner.CheckNoDataSource();
                
                if (index < 0 || index >= this.InnerArray.Count) 
                {
                    throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                }
                this.InnerArray.RemoveAt(index);
                this.owner.OnItemsCollectionChanged();
            }
        } // end ObjectCollection

        private sealed class ItemComparer : System.Collections.IComparer 
        {
            private DataGridViewComboBoxCell dataGridViewComboBoxCell;

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
                String itemName1 = this.dataGridViewComboBoxCell.GetItemDisplayText(item1);
                String itemName2 = this.dataGridViewComboBoxCell.GetItemDisplayText(item2);

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

            // Post XP theming functions
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

        /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCellAccessibleObject"]/*' />
        protected class DataGridViewComboBoxCellAccessibleObject : DataGridViewCellAccessibleObject
        {

            /// <include file='doc\DataGridViewComboBoxCell.uex' path='docs/doc[@for="DataGridViewComboBoxCellAccessibleObject.DataGridViewComboBoxCellAccessibleObject"]/*' />
            public DataGridViewComboBoxCellAccessibleObject(DataGridViewCell owner) : base(owner)
            {
            }

            internal override bool IsIAccessibleExSupported()
            {
                return true;
            }

            internal override object GetPropertyValue(int propertyID)
            {
                if (propertyID == NativeMethods.UIA_ControlTypePropertyId)
                {
                    return NativeMethods.UIA_ComboBoxControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}

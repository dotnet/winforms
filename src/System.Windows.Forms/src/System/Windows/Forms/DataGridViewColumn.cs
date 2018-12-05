// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Text;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Globalization;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn"]/*' />
    /// <devdoc>
    ///    <para> Base class for the columns in a data grid view.</para>
    /// </devdoc>
    [
        Designer("System.Windows.Forms.Design.DataGridViewColumnDesigner, " + AssemblyRef.SystemDesign),
        TypeConverterAttribute(typeof(DataGridViewColumnConverter)),
        ToolboxItem(false),
        DesignTimeVisible(false)
    ]
    public class DataGridViewColumn : DataGridViewBand, IComponent
    {
        private const float DATAGRIDVIEWCOLUMN_defaultFillWeight = 100F;
        private const int   DATAGRIDVIEWCOLUMN_defaultWidth = 100;
        private const int   DATAGRIDVIEWCOLUMN_defaultMinColumnThickness = 5;

        private const byte DATAGRIDVIEWCOLUMN_automaticSort                     = 0x01;
        private const byte DATAGRIDVIEWCOLUMN_programmaticSort                  = 0x02;
        private const byte DATAGRIDVIEWCOLUMN_isDataBound                       = 0x04;
        private const byte DATAGRIDVIEWCOLUMN_isBrowsableInternal               = 0x08;
        private const byte DATAGRIDVIEWCOLUMN_displayIndexHasChangedInternal    = 0x10;

        private byte flags;  // see DATAGRIDVIEWCOLUMN_ consts above
        private DataGridViewCell cellTemplate;
        private string name;
        private int displayIndex;
        private int desiredFillWidth = 0;
        private int desiredMinimumWidth = 0;
        private float fillWeight, usedFillWeight;
        private DataGridViewAutoSizeColumnMode autoSizeMode;
        private int boundColumnIndex = -1;
        private string dataPropertyName = String.Empty;
        private TypeConverter boundColumnConverter = null;

        // needed for IComponent
        private ISite site = null;
        private EventHandler disposed = null;

        private static readonly int PropDataGridViewColumnValueType = PropertyStore.CreateKey();

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.DataGridViewColumn"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.DataGridViewColumn'/> class.
        ///    </para>
        /// </devdoc>
        public DataGridViewColumn() : this((DataGridViewCell) null)
        {
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.DataGridViewColumn3"]/*' />
        public DataGridViewColumn(DataGridViewCell cellTemplate) : base()
        {
            this.fillWeight = DATAGRIDVIEWCOLUMN_defaultFillWeight;
            this.usedFillWeight = DATAGRIDVIEWCOLUMN_defaultFillWeight;
            this.Thickness = ScaleToCurrentDpi(DATAGRIDVIEWCOLUMN_defaultWidth);
            this.MinimumThickness = ScaleToCurrentDpi(DATAGRIDVIEWCOLUMN_defaultMinColumnThickness);
            this.name = String.Empty;
            this.bandIsRow = false;
            this.displayIndex = -1;
            this.cellTemplate = cellTemplate;
            this.autoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
        }

        /// <summary>
        /// Scale to current device dpi settings
        /// </summary>
        /// <param name="value"> initial value</param>
        /// <returns> scaled metric</returns>
        private int ScaleToCurrentDpi(int value)
        {
            return DpiHelper.IsScalingRequirementMet ? DpiHelper.LogicalToDeviceUnits(value) : value;
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.AutoSizeMode"]/*' />
        [
            SRCategory(nameof(SR.CatLayout)),
            DefaultValue(DataGridViewAutoSizeColumnMode.NotSet),
            SRDescription(nameof(SR.DataGridViewColumn_AutoSizeModeDescr)),
            RefreshProperties(RefreshProperties.Repaint)
        ]
        public DataGridViewAutoSizeColumnMode AutoSizeMode
        {
            get
            {
                return this.autoSizeMode;
            }
            set
            {
               switch (value) 
               { 
                   case DataGridViewAutoSizeColumnMode.NotSet:
                   case DataGridViewAutoSizeColumnMode.None:
                   case DataGridViewAutoSizeColumnMode.ColumnHeader:
                   case DataGridViewAutoSizeColumnMode.AllCellsExceptHeader:
                   case DataGridViewAutoSizeColumnMode.AllCells:
                   case DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader:
                   case DataGridViewAutoSizeColumnMode.DisplayedCells:
                   case DataGridViewAutoSizeColumnMode.Fill:
                       break;
                   default: 
                       throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewAutoSizeColumnMode)); 
                }
                if (this.autoSizeMode != value)
                {
                    if (this.Visible && this.DataGridView != null)
                    {
                        if (!this.DataGridView.ColumnHeadersVisible &&
                            (value == DataGridViewAutoSizeColumnMode.ColumnHeader ||
                             (value == DataGridViewAutoSizeColumnMode.NotSet && this.DataGridView.AutoSizeColumnsMode == DataGridViewAutoSizeColumnsMode.ColumnHeader)))
                        {
                            throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_AutoSizeCriteriaCannotUseInvisibleHeaders));
                        }
                        if (this.Frozen &&
                            (value == DataGridViewAutoSizeColumnMode.Fill ||
                             (value == DataGridViewAutoSizeColumnMode.NotSet && this.DataGridView.AutoSizeColumnsMode == DataGridViewAutoSizeColumnsMode.Fill)))
                        {
                            // Cannot set the inherited auto size mode to Fill when the column is frozen
                            throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_FrozenColumnCannotAutoFill));
                        }
                    }
                    DataGridViewAutoSizeColumnMode previousInheritedMode = this.InheritedAutoSizeMode;
                    bool previousInheritedModeAutoSized = previousInheritedMode != DataGridViewAutoSizeColumnMode.Fill &&
                                                          previousInheritedMode != DataGridViewAutoSizeColumnMode.None &&
                                                          previousInheritedMode != DataGridViewAutoSizeColumnMode.NotSet;
                    this.autoSizeMode = value;
                    if (this.DataGridView == null)
                    {
                        if (this.InheritedAutoSizeMode != DataGridViewAutoSizeColumnMode.Fill && 
                            this.InheritedAutoSizeMode != DataGridViewAutoSizeColumnMode.None &&
                            this.InheritedAutoSizeMode != DataGridViewAutoSizeColumnMode.NotSet)
                        {
                            if (!previousInheritedModeAutoSized)
                            {
                                // Save current column width for later reuse
                                this.CachedThickness = this.Thickness;
                            }
                        }
                        else
                        {
                            if (this.Thickness != this.CachedThickness && previousInheritedModeAutoSized)
                            {
                                // Restoring cached column width
                                this.ThicknessInternal = this.CachedThickness;
                            }
                        }
                    }
                    else
                    {
                        this.DataGridView.OnAutoSizeColumnModeChanged(this, previousInheritedMode);
                    }
                }
            }
        }

        // TypeConverter of the PropertyDescriptor attached to this column
        // in databound cases. Null otherwise.
        internal TypeConverter BoundColumnConverter
        {
            get
            {
                return this.boundColumnConverter;
            }
            set
            {
                this.boundColumnConverter = value;
            }
        }

        internal int BoundColumnIndex
        {
            get
            {
                return this.boundColumnIndex;
            }
            set
            {
                this.boundColumnIndex = value;
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.CellTemplate"]/*' />
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual DataGridViewCell CellTemplate
        {
            get
            {
                return this.cellTemplate;
            }
            set
            {
                this.cellTemplate = value;
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.CellType"]/*' />
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced),
        ]
        public Type CellType
        {
            get
            {
                if (this.cellTemplate != null)
                {
                    return this.cellTemplate.GetType();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.ContextMenuStrip"]/*' />
        [
            DefaultValue(null),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_ColumnContextMenuStripDescr))
        ]
        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return base.ContextMenuStrip;
            }
            set
            {
                base.ContextMenuStrip = value;
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.DataPropertyName"]/*' />
        [
            Browsable(true),
            DefaultValue(""),
            TypeConverterAttribute("System.Windows.Forms.Design.DataMemberFieldConverter, " + AssemblyRef.SystemDesign),
            Editor("System.Windows.Forms.Design.DataGridViewColumnDataPropertyNameEditor, " + AssemblyRef.SystemDesign, typeof(System.Drawing.Design.UITypeEditor)),
            SRDescription(nameof(SR.DataGridView_ColumnDataPropertyNameDescr)),
            SRCategory(nameof(SR.CatData))
        ]
        public string DataPropertyName
        {
            get
            {
                return this.dataPropertyName;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                if (value != this.dataPropertyName)
                {
                    this.dataPropertyName = value;
                    if (this.DataGridView != null)
                    {
                        this.DataGridView.OnColumnDataPropertyNameChanged(this);
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.DefaultCellStyle"]/*' />
        [
            Browsable(true),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ColumnDefaultCellStyleDescr))
        ]
        public override DataGridViewCellStyle DefaultCellStyle
        {
            get
            {
                return base.DefaultCellStyle;
            }
            set
            {
                base.DefaultCellStyle = value;
            }
        }

        private bool ShouldSerializeDefaultCellStyle()
        {

            if (!this.HasDefaultCellStyle)
            {
                return false;
            }

            DataGridViewCellStyle defaultCellStyle = this.DefaultCellStyle;

            return (!defaultCellStyle.BackColor.IsEmpty || 
                    !defaultCellStyle.ForeColor.IsEmpty ||
                    !defaultCellStyle.SelectionBackColor.IsEmpty || 
                    !defaultCellStyle.SelectionForeColor.IsEmpty ||
                    defaultCellStyle.Font != null ||
                    !defaultCellStyle.IsNullValueDefault ||
                    !defaultCellStyle.IsDataSourceNullValueDefault ||
                    !String.IsNullOrEmpty(defaultCellStyle.Format) ||
                    !defaultCellStyle.FormatProvider.Equals(System.Globalization.CultureInfo.CurrentCulture) ||
                    defaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet ||
                    defaultCellStyle.WrapMode != DataGridViewTriState.NotSet ||
                    defaultCellStyle.Tag !=  null ||
                    !defaultCellStyle.Padding.Equals(Padding.Empty));
        }

        internal int DesiredFillWidth
        {
            get
            {
                return this.desiredFillWidth;
            }
            set
            {
                this.desiredFillWidth = value;
            }
        }

        internal int DesiredMinimumWidth
        {
            get
            {
                return this.desiredMinimumWidth;
            }
            set
            {
                this.desiredMinimumWidth = value;
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.DisplayIndex"]/*' />
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int DisplayIndex
        {
            get 
            {
                return this.displayIndex;
            }
            set 
            {
                if (this.displayIndex != value)
                {
                    if (value == Int32.MaxValue)
                    {
                        throw new ArgumentOutOfRangeException(nameof(DisplayIndex), value, string.Format(SR.DataGridViewColumn_DisplayIndexTooLarge, Int32.MaxValue.ToString(CultureInfo.CurrentCulture)));
                    }
                    if (this.DataGridView != null)
                    {
                        if (value < 0)
                        {
                            throw new ArgumentOutOfRangeException(nameof(DisplayIndex), value, string.Format(SR.DataGridViewColumn_DisplayIndexNegative));
                        }
                        if (value >= this.DataGridView.Columns.Count)
                        {
                            throw new ArgumentOutOfRangeException(nameof(DisplayIndex), value, string.Format(SR.DataGridViewColumn_DisplayIndexExceedsColumnCount));
                        }
                        // Will throw an error if a visible frozen column is placed inside a non-frozen area or vice-versa.
                        this.DataGridView.OnColumnDisplayIndexChanging(this, value);
                        this.displayIndex = value;
                        try
                        {
                            this.DataGridView.InDisplayIndexAdjustments = true;
                            this.DataGridView.OnColumnDisplayIndexChanged_PreNotification();
                            this.DataGridView.OnColumnDisplayIndexChanged(this);
                            this.DataGridView.OnColumnDisplayIndexChanged_PostNotification();
                        }
                        finally
                        {
                            this.DataGridView.InDisplayIndexAdjustments = false;
                        }
                    }
                    else
                    {
                        if (value < -1)
                        {
                            throw new ArgumentOutOfRangeException(nameof(DisplayIndex), value, string.Format(SR.DataGridViewColumn_DisplayIndexTooNegative));
                        }
                        this.displayIndex = value;
                    }
                }
            }
        }

        internal bool DisplayIndexHasChanged
        {
            get
            {
                return (this.flags & DATAGRIDVIEWCOLUMN_displayIndexHasChangedInternal) != 0;
            }
            set
            {
                if (value)
                {
                    this.flags |= (byte) DATAGRIDVIEWCOLUMN_displayIndexHasChangedInternal;
                }
                else
                {
                    this.flags = (byte)(this.flags & ~DATAGRIDVIEWCOLUMN_displayIndexHasChangedInternal);
                }
            }
        }

        internal int DisplayIndexInternal
        {
            set
            {
                Debug.Assert(value >= -1);
                Debug.Assert(value < Int32.MaxValue);

                this.displayIndex = value;
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.Disposed"]/*' />
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public event EventHandler Disposed
        {
            add
            {
                this.disposed += value;
            }
            remove
            {
                this.disposed -= value;
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.DividerWidth"]/*' />
        [
            DefaultValue(0),
            SRCategory(nameof(SR.CatLayout)),
            SRDescription(nameof(SR.DataGridView_ColumnDividerWidthDescr))
        ]
        public int DividerWidth
        {
            get 
            {
                return this.DividerThickness;
            }
            set 
            {
                this.DividerThickness = value;
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.FillWeight"]/*' />
        [
            SRCategory(nameof(SR.CatLayout)),
            DefaultValue(DATAGRIDVIEWCOLUMN_defaultFillWeight),
            SRDescription(nameof(SR.DataGridViewColumn_FillWeightDescr)),
        ]
        public float FillWeight
        {
            get
            {
                return this.fillWeight;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(FillWeight), string.Format(SR.InvalidLowBoundArgument, "FillWeight", (value).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                }
                if (value > (float)ushort.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(FillWeight), string.Format(SR.InvalidHighBoundArgumentEx, "FillWeight", (value).ToString(CultureInfo.CurrentCulture), (ushort.MaxValue).ToString(CultureInfo.CurrentCulture)));
                }
                if (this.DataGridView != null)
                {
                    this.DataGridView.OnColumnFillWeightChanging(this, value);
                    this.fillWeight = value;
                    this.DataGridView.OnColumnFillWeightChanged(this);
                }
                else
                {
                    this.fillWeight = value;
                }
            }
        }

        internal float FillWeightInternal
        {
            set
            {
                Debug.Assert(value > 0);
                this.fillWeight = value;
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.Frozen"]/*' />
        [
            DefaultValue(false),
            RefreshProperties(RefreshProperties.All),
            SRCategory(nameof(SR.CatLayout)),
            SRDescription(nameof(SR.DataGridView_ColumnFrozenDescr))
        ]
        public override bool Frozen
        {
            get
            {
                return base.Frozen;
            }
            set
            {
                base.Frozen = value;
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.HeaderCell"]/*' />
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public DataGridViewColumnHeaderCell HeaderCell
        {
            get
            {
                return (DataGridViewColumnHeaderCell) base.HeaderCellCore;
            }
            set
            {
                base.HeaderCellCore = value;
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.HeaderText"]/*' />
        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ColumnHeaderTextDescr)),
            Localizable(true)
        ]
        public string HeaderText
        {
            get
            {
                if (this.HasHeaderCell)
                {
                    string headerValue = this.HeaderCell.Value as string;
                    if (headerValue != null)
                    {
                        return headerValue;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if ((value != null || this.HasHeaderCell) &&
                    this.HeaderCell.ValueType != null &&
                    this.HeaderCell.ValueType.IsAssignableFrom(typeof(System.String)))
                {
                    this.HeaderCell.Value = value;
                }
            }
        }

        private bool ShouldSerializeHeaderText()
        {
            return this.HasHeaderCell && ((DataGridViewColumnHeaderCell) this.HeaderCell).ContainsLocalValue;
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.InheritedAutoSizeMode"]/*' />
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public DataGridViewAutoSizeColumnMode InheritedAutoSizeMode
        {
            get
            {
                return GetInheritedAutoSizeMode(this.DataGridView);
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.InheritedStyle"]/*' />
        [
            Browsable(false)
        ]
        public override DataGridViewCellStyle InheritedStyle
        {
            get
            {
                DataGridViewCellStyle columnStyle = null;
                Debug.Assert(this.Index > -1);
                if (this.HasDefaultCellStyle)
                {
                    columnStyle = this.DefaultCellStyle;
                    Debug.Assert(columnStyle != null);
                }

                if (this.DataGridView == null)
                {
                    return columnStyle;
                }

                DataGridViewCellStyle inheritedCellStyleTmp = new DataGridViewCellStyle();
                DataGridViewCellStyle dataGridViewStyle = this.DataGridView.DefaultCellStyle;
                Debug.Assert(dataGridViewStyle != null);

                if (columnStyle != null && !columnStyle.BackColor.IsEmpty)
                {
                    inheritedCellStyleTmp.BackColor = columnStyle.BackColor;
                }
                else
                {
                    inheritedCellStyleTmp.BackColor = dataGridViewStyle.BackColor;
                }

                if (columnStyle != null && !columnStyle.ForeColor.IsEmpty)
                {
                    inheritedCellStyleTmp.ForeColor = columnStyle.ForeColor;
                }
                else
                {
                    inheritedCellStyleTmp.ForeColor = dataGridViewStyle.ForeColor;
                }

                if (columnStyle != null && !columnStyle.SelectionBackColor.IsEmpty)
                {
                    inheritedCellStyleTmp.SelectionBackColor = columnStyle.SelectionBackColor;
                }
                else
                {
                    inheritedCellStyleTmp.SelectionBackColor = dataGridViewStyle.SelectionBackColor;
                }

                if (columnStyle != null && !columnStyle.SelectionForeColor.IsEmpty)
                {
                    inheritedCellStyleTmp.SelectionForeColor = columnStyle.SelectionForeColor;
                }
                else
                {
                    inheritedCellStyleTmp.SelectionForeColor = dataGridViewStyle.SelectionForeColor;
                }

                if (columnStyle != null && columnStyle.Font != null)
                {
                    inheritedCellStyleTmp.Font = columnStyle.Font;
                }
                else
                {
                    inheritedCellStyleTmp.Font = dataGridViewStyle.Font;
                }

                if (columnStyle != null && !columnStyle.IsNullValueDefault)
                {
                    inheritedCellStyleTmp.NullValue = columnStyle.NullValue;
                }
                else
                {
                    inheritedCellStyleTmp.NullValue = dataGridViewStyle.NullValue;
                }

                if (columnStyle != null && !columnStyle.IsDataSourceNullValueDefault)
                {
                    inheritedCellStyleTmp.DataSourceNullValue = columnStyle.DataSourceNullValue;
                }
                else
                {
                    inheritedCellStyleTmp.DataSourceNullValue = dataGridViewStyle.DataSourceNullValue;
                }

                if (columnStyle != null && columnStyle.Format.Length != 0)
                {
                    inheritedCellStyleTmp.Format = columnStyle.Format;
                }
                else
                {
                    inheritedCellStyleTmp.Format = dataGridViewStyle.Format;
                }

                if (columnStyle != null && !columnStyle.IsFormatProviderDefault)
                {
                    inheritedCellStyleTmp.FormatProvider = columnStyle.FormatProvider;
                }
                else
                {
                    inheritedCellStyleTmp.FormatProvider = dataGridViewStyle.FormatProvider;
                }

                if (columnStyle != null && columnStyle.Alignment != DataGridViewContentAlignment.NotSet)
                {
                    inheritedCellStyleTmp.AlignmentInternal = columnStyle.Alignment;
                }
                else
                {
                    Debug.Assert(dataGridViewStyle.Alignment != DataGridViewContentAlignment.NotSet);
                    inheritedCellStyleTmp.AlignmentInternal = dataGridViewStyle.Alignment;
                }

                if (columnStyle != null && columnStyle.WrapMode != DataGridViewTriState.NotSet)
                {
                    inheritedCellStyleTmp.WrapModeInternal = columnStyle.WrapMode;
                }
                else
                {
                    Debug.Assert(dataGridViewStyle.WrapMode != DataGridViewTriState.NotSet);
                    inheritedCellStyleTmp.WrapModeInternal = dataGridViewStyle.WrapMode;
                }

                if (columnStyle != null && columnStyle.Tag != null)
                {
                    inheritedCellStyleTmp.Tag = columnStyle.Tag;
                }
                else
                {
                    inheritedCellStyleTmp.Tag = dataGridViewStyle.Tag;
                }

                if (columnStyle != null && columnStyle.Padding != Padding.Empty)
                {
                    inheritedCellStyleTmp.PaddingInternal = columnStyle.Padding;
                }
                else
                {
                    inheritedCellStyleTmp.PaddingInternal = dataGridViewStyle.Padding;
                }

                return inheritedCellStyleTmp;
            }
        }

        internal bool IsBrowsableInternal
        {
            get
            {
                return (this.flags & DATAGRIDVIEWCOLUMN_isBrowsableInternal) != 0;
            }
            set
            {
                if (value)
                {
                    this.flags |= (byte) DATAGRIDVIEWCOLUMN_isBrowsableInternal;
                }
                else
                {
                    this.flags = (byte)(this.flags & ~DATAGRIDVIEWCOLUMN_isBrowsableInternal);
                }
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.IsDataBound"]/*' />
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool IsDataBound
        {
            get
            {
                return this.IsDataBoundInternal;
            }
        }

        internal bool IsDataBoundInternal
        {
            get
            {
                return (this.flags & DATAGRIDVIEWCOLUMN_isDataBound) != 0;
            }
            set
            {
                if (value)
                {
                    this.flags |= (byte)DATAGRIDVIEWCOLUMN_isDataBound;
                }
                else
                {
                    this.flags = (byte)(this.flags & ~DATAGRIDVIEWCOLUMN_isDataBound);
                }
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.MinimumWidth"]/*' />
        [
            DefaultValue(DATAGRIDVIEWCOLUMN_defaultMinColumnThickness),
            Localizable(true), 
            SRCategory(nameof(SR.CatLayout)),
            SRDescription(nameof(SR.DataGridView_ColumnMinimumWidthDescr)),
            RefreshProperties(RefreshProperties.Repaint)
        ]
        public int MinimumWidth
        {
            get
            {
                return this.MinimumThickness;
            }
            set
            {
                this.MinimumThickness = value;
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.Name"]/*' />
        [
            Browsable(false)
        ]
        public string Name
        {
            get
            {
                //
                // Change needed to bring the design time and the runtime "Name" property together.
                // The ExtenderProvider adds a "Name" property of its own. It does this for all IComponents.
                // The "Name" property added by the ExtenderProvider interacts only w/ the Site property.
                // The Control class' Name property can be changed only thru the "Name" property provided by the 
                // Extender Service.
                //
                // However, the user can change the DataGridView::Name property in the DataGridViewEditColumnDialog.
                // So while the Control can fall back to Site.Name if the user did not explicitly set Control::Name, 
                // the DataGridViewColumn should always go first to the Site.Name to retrieve the name.
                //
                // NOTE: one side effect of bringing together the design time and the run time "Name" properties is that DataGridViewColumn::Name changes.
                // However, DataGridView does not fire ColumnNameChanged event.
                // We can't fix this because ISite does not provide Name change notification. So in effect
                // DataGridViewColumn does not know when its name changed.
                // I talked w/ MarkRi and he is perfectly fine w/ DataGridViewColumn::Name changing w/o ColumnNameChanged 
                // being fired.
                //
                if (this.Site != null && !String.IsNullOrEmpty(this.Site.Name))
                {
                    this.name = this.Site.Name;
                }

                return name;
            }
            set
            {                
                string oldName = this.name;
                if (String.IsNullOrEmpty(value))
                {
                    this.name = string.Empty;
                }
                else
                {
                    this.name = value;
                }
               
                if (this.DataGridView != null && !string.Equals(this.name, oldName,StringComparison.Ordinal))
                {
                    this.DataGridView.OnColumnNameChanged(this);
                }
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.ReadOnly"]/*' />
        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_ColumnReadOnlyDescr))
        ]
        public override bool ReadOnly
        {
            get
            {
                return base.ReadOnly;
            }
            set
            {
                if (this.IsDataBound &&
                    this.DataGridView != null &&
                    this.DataGridView.DataConnection != null &&
                    this.boundColumnIndex != -1 && 
                    this.DataGridView.DataConnection.DataFieldIsReadOnly(this.boundColumnIndex) &&
                    !value)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_ColumnBoundToAReadOnlyFieldMustRemainReadOnly));
                }
                base.ReadOnly = value;
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.Resizable"]/*' />
        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_ColumnResizableDescr))
        ]
        public override DataGridViewTriState Resizable
        {
            get
            {
                return base.Resizable;
            }
            set
            {
                base.Resizable = value;
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.Site"]/*' />
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public ISite Site
        {
            get
            {
                return this.site;
            }
            set
            {
                this.site = value;
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.SortMode"]/*' />
        [
            DefaultValue(DataGridViewColumnSortMode.NotSortable),
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.DataGridView_ColumnSortModeDescr))
        ]
        public DataGridViewColumnSortMode SortMode
        {
            get
            {
                if ((this.flags & DATAGRIDVIEWCOLUMN_automaticSort) != 0x00)
                {
                    return DataGridViewColumnSortMode.Automatic;
                }
                else if ((this.flags & DATAGRIDVIEWCOLUMN_programmaticSort) != 0x00)
                {
                    return DataGridViewColumnSortMode.Programmatic;
                }
                else
                {
                    return DataGridViewColumnSortMode.NotSortable;
                }
            }
            set
            {
                if (value != this.SortMode)
                {
                    if (value != DataGridViewColumnSortMode.NotSortable)
                    {
                        if (this.DataGridView != null &&
                            !this.DataGridView.InInitialization &&
                            value == DataGridViewColumnSortMode.Automatic &&
                            (this.DataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect ||
                            this.DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect))
                        {
                            throw new InvalidOperationException(string.Format(SR.DataGridViewColumn_SortModeAndSelectionModeClash, (value).ToString(), this.DataGridView.SelectionMode.ToString()));
                        }
                        if (value == DataGridViewColumnSortMode.Automatic)
                        {
                            this.flags = (byte)(this.flags & ~DATAGRIDVIEWCOLUMN_programmaticSort);
                            this.flags |= (byte)DATAGRIDVIEWCOLUMN_automaticSort;
                        }
                        else
                        {
                            this.flags = (byte)(this.flags & ~DATAGRIDVIEWCOLUMN_automaticSort);
                            this.flags |= (byte)DATAGRIDVIEWCOLUMN_programmaticSort;
                        }
                    }
                    else
                    {
                        this.flags = (byte)(this.flags & ~DATAGRIDVIEWCOLUMN_automaticSort);
                        this.flags = (byte)(this.flags & ~DATAGRIDVIEWCOLUMN_programmaticSort);
                    }
                    if (this.DataGridView != null)
                    {
                        this.DataGridView.OnColumnSortModeChanged(this);
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.ToolTipText"]/*' />
        [
            DefaultValue(""),
            Localizable(true), 
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ColumnToolTipTextDescr))
        ]
        public string ToolTipText
        {
            get
            {
                return this.HeaderCell.ToolTipText;
            }
            set
            {
                if (String.Compare(this.ToolTipText, value, false /*ignore case*/, CultureInfo.InvariantCulture) != 0)
                {
                    this.HeaderCell.ToolTipText = value;

                    if (this.DataGridView != null)
                    {
                        this.DataGridView.OnColumnToolTipTextChanged(this);
                    }
                }
            }
        }

        internal float UsedFillWeight
        {
            get
            {
                return this.usedFillWeight;
            }
            set
            {
                Debug.Assert(value > 0);
                this.usedFillWeight = value;
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.ValueType"]/*' />
        [
            Browsable(false),
            DefaultValue(null),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public Type ValueType
        {
            get
            {
                return (Type) this.Properties.GetObject(PropDataGridViewColumnValueType);
            }
            set
            {
                // what should we do when we modify the ValueType in the dataGridView column???
                this.Properties.SetObject(PropDataGridViewColumnValueType, value);
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.Visible"]/*' />
        [
            DefaultValue(true),
            Localizable(true), 
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.DataGridView_ColumnVisibleDescr))
        ]
        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.Width"]/*' />
        [
            SRCategory(nameof(SR.CatLayout)),
            Localizable(true), 
            SRDescription(nameof(SR.DataGridView_ColumnWidthDescr)),
            RefreshProperties(RefreshProperties.Repaint)
        ]
        public int Width
        {
            get
            {
                return this.Thickness;
            }
            set
            {
                this.Thickness = value;
            }
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.Clone"]/*' />
        public override object Clone()
        {
            // 

            DataGridViewColumn dataGridViewColumn = (DataGridViewColumn) System.Activator.CreateInstance(this.GetType());
            if (dataGridViewColumn != null)
            {
                CloneInternal(dataGridViewColumn);
            }
            return dataGridViewColumn;
        }

        internal void CloneInternal(DataGridViewColumn dataGridViewColumn)
        {
            base.CloneInternal(dataGridViewColumn);

            dataGridViewColumn.name = this.Name;
            dataGridViewColumn.displayIndex = -1;
            dataGridViewColumn.HeaderText = this.HeaderText;
            dataGridViewColumn.DataPropertyName = this.DataPropertyName;

            // dataGridViewColumn.boundColumnConverter = columnTemplate.BoundColumnConverter;  setting the DataPropertyName should also set the bound column converter later on.
            if (dataGridViewColumn.CellTemplate != null)
            {
                dataGridViewColumn.cellTemplate = (DataGridViewCell)this.CellTemplate.Clone();
            }
            else
            {
                dataGridViewColumn.cellTemplate = null;
            }

            if (this.HasHeaderCell)
            {
                dataGridViewColumn.HeaderCell = (DataGridViewColumnHeaderCell) this.HeaderCell.Clone();
            }

            dataGridViewColumn.AutoSizeMode = this.AutoSizeMode;
            dataGridViewColumn.SortMode = this.SortMode;
            dataGridViewColumn.FillWeightInternal = this.FillWeight;
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.Dispose"]/*' />
        protected override void Dispose(bool disposing) {
            try 
            {
                if (disposing)
                {
                    // 
                    lock(this)
                    {
                        if (this.site != null && this.site.Container != null)
                        {
                            this.site.Container.Remove(this);
                        }

                        if (this.disposed != null)
                        {
                            this.disposed(this, EventArgs.Empty);
                        }
                    }
                }
            }
            finally 
            {
                base.Dispose(disposing);
            }
        }

        internal DataGridViewAutoSizeColumnMode GetInheritedAutoSizeMode(DataGridView dataGridView)
        {
            if (dataGridView != null && this.autoSizeMode == DataGridViewAutoSizeColumnMode.NotSet)
            {
                switch (dataGridView.AutoSizeColumnsMode)
                {
                    case DataGridViewAutoSizeColumnsMode.AllCells:
                        return DataGridViewAutoSizeColumnMode.AllCells;

                    case DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader:
                        return DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

                    case DataGridViewAutoSizeColumnsMode.DisplayedCells:
                        return DataGridViewAutoSizeColumnMode.DisplayedCells;

                    case DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader:
                        return DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;

                    case DataGridViewAutoSizeColumnsMode.ColumnHeader:
                        return DataGridViewAutoSizeColumnMode.ColumnHeader;

                    case DataGridViewAutoSizeColumnsMode.Fill:
                        return DataGridViewAutoSizeColumnMode.Fill;

                    default: // None
                        return DataGridViewAutoSizeColumnMode.None;
                }
            }
            return this.autoSizeMode;
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.GetPreferredWidth"]/*' />
        public virtual int GetPreferredWidth(DataGridViewAutoSizeColumnMode autoSizeColumnMode, bool fixedHeight)
        {
            if (autoSizeColumnMode == DataGridViewAutoSizeColumnMode.NotSet ||
                autoSizeColumnMode == DataGridViewAutoSizeColumnMode.None ||
                autoSizeColumnMode == DataGridViewAutoSizeColumnMode.Fill)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_NeedColumnAutoSizingCriteria, "autoSizeColumnMode"));
            }
            switch (autoSizeColumnMode) { 
                case DataGridViewAutoSizeColumnMode.NotSet:
                case DataGridViewAutoSizeColumnMode.None:
                case DataGridViewAutoSizeColumnMode.ColumnHeader:
                case DataGridViewAutoSizeColumnMode.AllCellsExceptHeader:
                case DataGridViewAutoSizeColumnMode.AllCells:
                case DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader:
                case DataGridViewAutoSizeColumnMode.DisplayedCells:
                case DataGridViewAutoSizeColumnMode.Fill:
                    break;
                default: 
                    throw new InvalidEnumArgumentException(nameof(autoSizeColumnMode), (int) autoSizeColumnMode, typeof(DataGridViewAutoSizeColumnMode)); 
             }

            DataGridView dataGridView = this.DataGridView;

            Debug.Assert(dataGridView == null || this.Index > -1);

            if (dataGridView == null)
            {
                return -1;
            }

            DataGridViewAutoSizeColumnCriteriaInternal autoSizeColumnCriteriaInternal = (DataGridViewAutoSizeColumnCriteriaInternal) autoSizeColumnMode;
            Debug.Assert(autoSizeColumnCriteriaInternal == DataGridViewAutoSizeColumnCriteriaInternal.Header ||
                autoSizeColumnCriteriaInternal == DataGridViewAutoSizeColumnCriteriaInternal.AllRows ||
                autoSizeColumnCriteriaInternal == DataGridViewAutoSizeColumnCriteriaInternal.DisplayedRows ||
                autoSizeColumnCriteriaInternal == (DataGridViewAutoSizeColumnCriteriaInternal.Header | DataGridViewAutoSizeColumnCriteriaInternal.AllRows) ||
                autoSizeColumnCriteriaInternal == (DataGridViewAutoSizeColumnCriteriaInternal.Header | DataGridViewAutoSizeColumnCriteriaInternal.DisplayedRows));

            int preferredColumnThickness = 0, preferredCellThickness, rowIndex;
            DataGridViewRow dataGridViewRow;
            Debug.Assert(dataGridView.ColumnHeadersVisible || autoSizeColumnCriteriaInternal != DataGridViewAutoSizeColumnCriteriaInternal.Header);

            // take into account the preferred width of the header cell if displayed and cared about
            if (dataGridView.ColumnHeadersVisible &&
                (autoSizeColumnCriteriaInternal & DataGridViewAutoSizeColumnCriteriaInternal.Header) != 0)
            {
                if (fixedHeight)
                {
                    preferredCellThickness = this.HeaderCell.GetPreferredWidth(-1, dataGridView.ColumnHeadersHeight);
                }
                else
                {
                    preferredCellThickness = this.HeaderCell.GetPreferredSize(-1).Width;
                }
                if (preferredColumnThickness < preferredCellThickness)
                {
                    preferredColumnThickness = preferredCellThickness;
                }
            }
            if ((autoSizeColumnCriteriaInternal & DataGridViewAutoSizeColumnCriteriaInternal.AllRows) != 0)
            {
                for (rowIndex = dataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible);
                    rowIndex != -1;
                    rowIndex = dataGridView.Rows.GetNextRow(rowIndex, DataGridViewElementStates.Visible))
                {
                    dataGridViewRow = dataGridView.Rows.SharedRow(rowIndex);
                    if (fixedHeight)
                    {
                        preferredCellThickness = dataGridViewRow.Cells[this.Index].GetPreferredWidth(rowIndex, dataGridViewRow.Thickness);
                    }
                    else
                    {
                        preferredCellThickness = dataGridViewRow.Cells[this.Index].GetPreferredSize(rowIndex).Width;
                    }
                    if (preferredColumnThickness < preferredCellThickness)
                    {
                        preferredColumnThickness = preferredCellThickness;
                    }
                }
            }
            else if ((autoSizeColumnCriteriaInternal & DataGridViewAutoSizeColumnCriteriaInternal.DisplayedRows) != 0)
            {
                int displayHeight = dataGridView.LayoutInfo.Data.Height;
                int cy = 0;

                rowIndex = dataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);
                while (rowIndex != -1 && cy < displayHeight)
                {
                    dataGridViewRow = dataGridView.Rows.SharedRow(rowIndex);
                    if (fixedHeight)
                    {
                        preferredCellThickness = dataGridViewRow.Cells[this.Index].GetPreferredWidth(rowIndex, dataGridViewRow.Thickness);
                    }
                    else
                    {
                        preferredCellThickness = dataGridViewRow.Cells[this.Index].GetPreferredSize(rowIndex).Width;
                    }
                    if (preferredColumnThickness < preferredCellThickness)
                    {
                        preferredColumnThickness = preferredCellThickness;
                    }
                    cy += dataGridViewRow.Thickness;
                    rowIndex = dataGridView.Rows.GetNextRow(rowIndex,
                        DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen);
                }

                if (cy < displayHeight)
                {
                    rowIndex = dataGridView.DisplayedBandsInfo.FirstDisplayedScrollingRow;
                    while (rowIndex != -1 && cy < displayHeight)
                    {
                        dataGridViewRow = dataGridView.Rows.SharedRow(rowIndex);
                        if (fixedHeight)
                        {
                            preferredCellThickness = dataGridViewRow.Cells[this.Index].GetPreferredWidth(rowIndex, dataGridViewRow.Thickness);
                        }
                        else
                        {
                            preferredCellThickness = dataGridViewRow.Cells[this.Index].GetPreferredSize(rowIndex).Width;
                        }
                        if (preferredColumnThickness < preferredCellThickness)
                        {
                            preferredColumnThickness = preferredCellThickness;
                        }
                        cy += dataGridViewRow.Thickness;
                        rowIndex = dataGridView.Rows.GetNextRow(rowIndex, DataGridViewElementStates.Visible);
                    }
                }
            }
            return preferredColumnThickness;
        }

        /// <include file='doc\DataGridViewColumn.uex' path='docs/doc[@for="DataGridViewColumn.ToString"]/*' />
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append("DataGridViewColumn { Name=");
            sb.Append(this.Name);
            sb.Append(", Index=");
            sb.Append(this.Index.ToString(CultureInfo.CurrentCulture));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Text;
    using System.Drawing;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Windows.Forms.Design;
    using System.Drawing.Design;
    using System.Diagnostics.CodeAnalysis;

    /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle"]/*' />
    [
        TypeConverterAttribute(typeof(DataGridViewCellStyleConverter)),
        EditorAttribute("System.Windows.Forms.Design.DataGridViewCellStyleEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))
    ]
    public class DataGridViewCellStyle : ICloneable
    {
        private static readonly int PropAlignment = PropertyStore.CreateKey();
        private static readonly int PropBackColor = PropertyStore.CreateKey();
        private static readonly int PropDataSourceNullValue = PropertyStore.CreateKey();
        private static readonly int PropFont = PropertyStore.CreateKey();
        private static readonly int PropForeColor = PropertyStore.CreateKey();
        private static readonly int PropFormat = PropertyStore.CreateKey();
        private static readonly int PropFormatProvider = PropertyStore.CreateKey();
        private static readonly int PropNullValue = PropertyStore.CreateKey();
        private static readonly int PropPadding = PropertyStore.CreateKey();
        private static readonly int PropSelectionBackColor = PropertyStore.CreateKey();
        private static readonly int PropSelectionForeColor = PropertyStore.CreateKey();
        private static readonly int PropTag = PropertyStore.CreateKey();
        private static readonly int PropWrapMode = PropertyStore.CreateKey();

        private const string DATAGRIDVIEWCELLSTYLE_nullText = "";    // default value of NullValue property

        private DataGridViewCellStyleScopes scope;
        private PropertyStore propertyStore;          // Contains all properties that are not always set.
        private DataGridView dataGridView;

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.DataGridViewCellStyle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.DataGridViewCellStyle'/> class.
        ///    </para>
        /// </devdoc>
        public DataGridViewCellStyle()
        {
            this.propertyStore = new PropertyStore();
            this.scope = DataGridViewCellStyleScopes.None;
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.DataGridViewCellStyle2"]/*' />
        public DataGridViewCellStyle(DataGridViewCellStyle dataGridViewCellStyle)
        {
            if (dataGridViewCellStyle == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewCellStyle));
            }
            this.propertyStore = new PropertyStore();
            this.scope = DataGridViewCellStyleScopes.None;
            this.BackColor = dataGridViewCellStyle.BackColor;
            this.ForeColor = dataGridViewCellStyle.ForeColor;
            this.SelectionBackColor = dataGridViewCellStyle.SelectionBackColor;
            this.SelectionForeColor = dataGridViewCellStyle.SelectionForeColor;
            this.Font = dataGridViewCellStyle.Font;
            this.NullValue = dataGridViewCellStyle.NullValue;
            this.DataSourceNullValue = dataGridViewCellStyle.DataSourceNullValue;
            this.Format = dataGridViewCellStyle.Format;
            if (!dataGridViewCellStyle.IsFormatProviderDefault)
            {
                this.FormatProvider = dataGridViewCellStyle.FormatProvider;
            }
            this.AlignmentInternal = dataGridViewCellStyle.Alignment;
            this.WrapModeInternal = dataGridViewCellStyle.WrapMode;
            this.Tag = dataGridViewCellStyle.Tag;
            this.PaddingInternal = dataGridViewCellStyle.Padding;
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.Alignment"]/*' />
        [
            SRDescription(nameof(SR.DataGridViewCellStyleAlignmentDescr)),
            //Localizable(true),
            DefaultValue(DataGridViewContentAlignment.NotSet),
            SRCategory(nameof(SR.CatLayout))
        ]
        public DataGridViewContentAlignment Alignment
        {
            get
            {
                bool found;
                int alignment = this.Properties.GetInteger(PropAlignment, out found);
                if (found)
                {
                    return (DataGridViewContentAlignment) alignment;
                }
                return DataGridViewContentAlignment.NotSet;
            }
            set
            {
               switch (value) 
               { 
                    case DataGridViewContentAlignment.NotSet:
                    case DataGridViewContentAlignment.TopLeft:
                    case DataGridViewContentAlignment.TopCenter:
                    case DataGridViewContentAlignment.TopRight:
                    case DataGridViewContentAlignment.MiddleLeft:
                    case DataGridViewContentAlignment.MiddleCenter:
                    case DataGridViewContentAlignment.MiddleRight:
                    case DataGridViewContentAlignment.BottomLeft:
                    case DataGridViewContentAlignment.BottomCenter:
                    case DataGridViewContentAlignment.BottomRight:
                        break;
                    default: 
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewContentAlignment)); 
                }
                this.AlignmentInternal = value;
            }
        }

        internal DataGridViewContentAlignment AlignmentInternal
        {
            [
                SuppressMessage("Microsoft.Performance", "CA1803:AvoidCostlyCallsWherePossible") // Enum.IsDefined is OK here. Debug only.
            ]
            set
            {
                Debug.Assert(Enum.IsDefined(typeof(DataGridViewContentAlignment), value));
                if (this.Alignment != value)
                {
                    this.Properties.SetInteger(PropAlignment, (int) value);
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
                }
            }
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.BackColor"]/*' />
        [
            SRCategory(nameof(SR.CatAppearance))
        ]
        public Color BackColor
        {
            get
            {
                return this.Properties.GetColor(PropBackColor);              
            }
            set
            {
                Color c = this.BackColor;
                if (!value.IsEmpty || this.Properties.ContainsObject(PropBackColor))
                {
                    this.Properties.SetColor(PropBackColor, value);
                }
                if (!c.Equals(this.BackColor))
                {
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.Color);
                }
            }
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.DataSourceNullValue"]/*' />
        [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public object DataSourceNullValue
        {
            get
            {
                if (this.Properties.ContainsObject(PropDataSourceNullValue))
                {
                    return this.Properties.GetObject(PropDataSourceNullValue);
                }
                return System.DBNull.Value;
            }
            set
            {
                object oldDataSourceNullValue = this.DataSourceNullValue;

                if ((oldDataSourceNullValue == value) ||
                    (oldDataSourceNullValue != null && oldDataSourceNullValue.Equals(value)))
                {
                    return;
                }

                if (value == System.DBNull.Value && 
                    this.Properties.ContainsObject(PropDataSourceNullValue))
                {
                    this.Properties.RemoveObject(PropDataSourceNullValue);
                }
                else
                {
                    this.Properties.SetObject(PropDataSourceNullValue, value);
                }

                Debug.Assert((oldDataSourceNullValue == null && this.DataSourceNullValue != null) ||
                             (oldDataSourceNullValue != null && this.DataSourceNullValue == null) ||
                             (oldDataSourceNullValue != this.DataSourceNullValue && !oldDataSourceNullValue.Equals(this.DataSourceNullValue)));

                OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
            }
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.Font"]/*' />
        [
            SRCategory(nameof(SR.CatAppearance))
        ]
        public Font Font
        {
            get
            {
                return (Font) this.Properties.GetObject(PropFont);
            }
            set
            {
                Font f = this.Font;
                if (value != null || this.Properties.ContainsObject(PropFont))
                {
                    this.Properties.SetObject(PropFont, value);
                }
                if ((f == null && value != null) ||
                    (f != null && value == null) ||
                    (f != null && value != null && !f.Equals(this.Font)))
                {
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.Font);
                }
            }
        }
        
        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.ForeColor"]/*' />
        [
            SRCategory(nameof(SR.CatAppearance))
        ]
        public Color ForeColor
        {
            get
            {
                return this.Properties.GetColor(PropForeColor);
            }
            set
            {
                Color c = this.ForeColor;
                if (!value.IsEmpty || this.Properties.ContainsObject(PropForeColor))
                {
                    this.Properties.SetColor(PropForeColor, value);
                }
                if (!c.Equals(this.ForeColor)) 
                {
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.ForeColor);
                }
            }
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.Format"]/*' />
        [
            DefaultValue(""),
            EditorAttribute("System.Windows.Forms.Design.FormatStringEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
            SRCategory(nameof(SR.CatBehavior)),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public string Format
        {
            get
            {
                object format = this.Properties.GetObject(PropFormat);
                if (format == null)
                {
                    return string.Empty;
                }
                else
                {
                    return (string) format;
                }
            }
            set
            {
                string format = this.Format;
                if ((value != null && value.Length > 0) || this.Properties.ContainsObject(PropFormat))
                {
                    this.Properties.SetObject(PropFormat, value);
                }
                if (!format.Equals(this.Format))
                {
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
                }
            }
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.FormatProvider"]/*' />
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public IFormatProvider FormatProvider
        {
            get
            {
                object formatProvider = this.Properties.GetObject(PropFormatProvider);
                if (formatProvider == null)
                {
                    return System.Globalization.CultureInfo.CurrentCulture;
                }
                else
                {
                    return (IFormatProvider) formatProvider;
                }
            }
            set
            {
                object originalFormatProvider = this.Properties.GetObject(PropFormatProvider);
                this.Properties.SetObject(PropFormatProvider, value);
                if (value != originalFormatProvider)
                {
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
                }
            }
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.IsDataSourceNullValueDefault"]/*' />
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public bool IsDataSourceNullValueDefault
        {
            get
            {
                if (!this.Properties.ContainsObject(PropDataSourceNullValue))
                {
                    return true;
                }
                return this.Properties.GetObject(PropDataSourceNullValue) == System.DBNull.Value;
            }
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.IsFormatProviderDefault"]/*' />
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public bool IsFormatProviderDefault
        {
            get
            {
                return this.Properties.GetObject(PropFormatProvider) == null;
            }
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.IsNullValueDefault"]/*' />
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public bool IsNullValueDefault
        {
            get
            {
                if (!this.Properties.ContainsObject(PropNullValue))
                {
                    return true;
                }
                object nullValue = this.Properties.GetObject(PropNullValue);
                return (nullValue is string && nullValue.Equals(DATAGRIDVIEWCELLSTYLE_nullText));
            }
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.NullValue"]/*' />
        [
            DefaultValue(DATAGRIDVIEWCELLSTYLE_nullText),
            TypeConverter(typeof(StringConverter)),
            SRCategory(nameof(SR.CatData))
        ]
        public object NullValue
        {
            get
            {
                if (this.Properties.ContainsObject(PropNullValue))
                {
                    return this.Properties.GetObject(PropNullValue);
                }
                return DATAGRIDVIEWCELLSTYLE_nullText;
            }
            set
            {
                object oldNullValue = this.NullValue;

                if ((oldNullValue == value) ||
                    (oldNullValue != null && oldNullValue.Equals(value)))
                {
                    return;
                }

                if (value is string &&
                    value.Equals(DATAGRIDVIEWCELLSTYLE_nullText) &&
                    this.Properties.ContainsObject(PropNullValue))
                {
                    this.Properties.RemoveObject(PropNullValue);
                }
                else
                {
                    this.Properties.SetObject(PropNullValue, value);
                }

                Debug.Assert((oldNullValue == null && this.NullValue != null) ||
                             (oldNullValue != null && this.NullValue == null) ||
                             (oldNullValue != this.NullValue && !oldNullValue.Equals(this.NullValue)));

                OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
            }
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.Padding"]/*' />
        [
            SRCategory(nameof(SR.CatLayout))
        ]
        public Padding Padding
        {
            get
            {
                return this.Properties.GetPadding(PropPadding);
            }
            set
            {
                if (value.Left < 0 || value.Right < 0 || value.Top < 0 || value.Bottom < 0)
                {
                    if (value.All != -1)
                    {
                        Debug.Assert(value.All < -1);
                        value.All = 0;
                    }
                    else
                    {
                        value.Left = Math.Max(0, value.Left);
                        value.Right = Math.Max(0, value.Right);
                        value.Top = Math.Max(0, value.Top);
                        value.Bottom = Math.Max(0, value.Bottom);
                    }
                }
                this.PaddingInternal = value;
            }
        }

        internal Padding PaddingInternal
        {
            set
            {
                Debug.Assert(value.Left >= 0);
                Debug.Assert(value.Right >= 0);
                Debug.Assert(value.Top >= 0);
                Debug.Assert(value.Bottom >= 0);
                if (value != this.Padding)
                {
                    this.Properties.SetPadding(PropPadding, value);
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
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

        internal DataGridViewCellStyleScopes Scope
        {
            get
            {
                return this.scope;
            }
            set
            {
                this.scope = value;
            }
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.SelectionBackColor"]/*' />
        [
            SRCategory(nameof(SR.CatAppearance))
        ]
        public Color SelectionBackColor
        {
            get
            {
                return this.Properties.GetColor(PropSelectionBackColor);              
            }
            set
            {
                Color c = this.SelectionBackColor;
                if (!value.IsEmpty || this.Properties.ContainsObject(PropSelectionBackColor))
                {
                    this.Properties.SetColor(PropSelectionBackColor, value);
                }
                if (!c.Equals(this.SelectionBackColor))
                {
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.Color);
                }
            }
        }
        
        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.SelectionForeColor"]/*' />
        [
            SRCategory(nameof(SR.CatAppearance))
        ]
        public Color SelectionForeColor
        {
            get
            {
                return this.Properties.GetColor(PropSelectionForeColor);        
            }
            set
            {
                Color c = this.SelectionForeColor;
                if (!value.IsEmpty || this.Properties.ContainsObject(PropSelectionForeColor))
                {
                    this.Properties.SetColor(PropSelectionForeColor, value);
                }
                if (!c.Equals(this.SelectionForeColor)) 
                {
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.Color);
                }
            }
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.Tag"]/*' />
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public object Tag
        {
            get
            {
                return Properties.GetObject(PropTag);
            }
            set
            {
                if (value != null || this.Properties.ContainsObject(PropTag))
                {
                    Properties.SetObject(PropTag, value);
                }
            }
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.WrapMode"]/*' />
        [
            DefaultValue(DataGridViewTriState.NotSet),
            SRCategory(nameof(SR.CatLayout))

        ]
        public DataGridViewTriState WrapMode
        {
            get
            {
                bool found;
                int wrap = this.Properties.GetInteger(PropWrapMode, out found);
                if (found)
                {
                    return (DataGridViewTriState) wrap;
                }
                return DataGridViewTriState.NotSet;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewTriState.NotSet, (int)DataGridViewTriState.False)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewTriState)); 
                }
                this.WrapModeInternal = value;
            }
        }

        internal DataGridViewTriState WrapModeInternal
        {
            set
            {
                Debug.Assert(value >= DataGridViewTriState.NotSet && value <= DataGridViewTriState.False);
                if (this.WrapMode != value)
                {
                    this.Properties.SetInteger(PropWrapMode, (int) value);
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
                }
            }
        }

        internal void AddScope(DataGridView dataGridView, DataGridViewCellStyleScopes scope)
        {
            this.scope |= scope;
            this.dataGridView = dataGridView;
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.Dispose"]/*' />
        public virtual void ApplyStyle(DataGridViewCellStyle dataGridViewCellStyle)
        {
            if (dataGridViewCellStyle == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewCellStyle));
            }
            if (!dataGridViewCellStyle.BackColor.IsEmpty)
            {
                this.BackColor = dataGridViewCellStyle.BackColor;
            }
            if (!dataGridViewCellStyle.ForeColor.IsEmpty)
            {
                this.ForeColor = dataGridViewCellStyle.ForeColor;
            }
            if (!dataGridViewCellStyle.SelectionBackColor.IsEmpty)
            {
                this.SelectionBackColor = dataGridViewCellStyle.SelectionBackColor;
            }
            if (!dataGridViewCellStyle.SelectionForeColor.IsEmpty)
            {
                this.SelectionForeColor = dataGridViewCellStyle.SelectionForeColor;
            }
            if (dataGridViewCellStyle.Font != null)
            {
                this.Font = dataGridViewCellStyle.Font;
            }
            if (!dataGridViewCellStyle.IsNullValueDefault)
            {
                this.NullValue = dataGridViewCellStyle.NullValue;
            }
            if (!dataGridViewCellStyle.IsDataSourceNullValueDefault)
            {
                this.DataSourceNullValue = dataGridViewCellStyle.DataSourceNullValue;
            }
            if (dataGridViewCellStyle.Format.Length != 0)
            {
                this.Format = dataGridViewCellStyle.Format;
            }
            if (!dataGridViewCellStyle.IsFormatProviderDefault)
            {
                this.FormatProvider = dataGridViewCellStyle.FormatProvider;
            }
            if (dataGridViewCellStyle.Alignment != DataGridViewContentAlignment.NotSet)
            {
                this.AlignmentInternal = dataGridViewCellStyle.Alignment;
            }
            if (dataGridViewCellStyle.WrapMode != DataGridViewTriState.NotSet)
            {
                this.WrapModeInternal = dataGridViewCellStyle.WrapMode;
            }
            if (dataGridViewCellStyle.Tag != null)
            {
                this.Tag = dataGridViewCellStyle.Tag;
            }
            if (dataGridViewCellStyle.Padding != Padding.Empty)
            {
                this.PaddingInternal = dataGridViewCellStyle.Padding;
            }
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.Clone"]/*' />
        public virtual DataGridViewCellStyle Clone() {
            return new DataGridViewCellStyle(this);
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.Equals"]/*' />
        public override bool Equals(object o) 
        {
            DataGridViewCellStyle dgvcs = o as DataGridViewCellStyle;
            if (dgvcs != null)
            {
                return GetDifferencesFrom(dgvcs) == DataGridViewCellStyleDifferences.None;
            }
            else
            {
                return false;
            }
        }

        internal DataGridViewCellStyleDifferences GetDifferencesFrom(DataGridViewCellStyle dgvcs)
        {
            Debug.Assert(dgvcs != null);

            bool preferredSizeAffectingPropDifferent = (
                    dgvcs.Alignment != this.Alignment ||
                    dgvcs.DataSourceNullValue != this.DataSourceNullValue ||
                    dgvcs.Font != this.Font ||
                    dgvcs.Format != this.Format ||
                    dgvcs.FormatProvider != this.FormatProvider ||
                    dgvcs.NullValue != this.NullValue ||
                    dgvcs.Padding != this.Padding ||
                    dgvcs.Tag != this.Tag ||
                    dgvcs.WrapMode != this.WrapMode );

            bool preferredSizeNonAffectingPropDifferent = (
                    dgvcs.BackColor != this.BackColor ||
                    dgvcs.ForeColor != this.ForeColor ||
                    dgvcs.SelectionBackColor != this.SelectionBackColor ||
                    dgvcs.SelectionForeColor != this.SelectionForeColor );

            if (preferredSizeAffectingPropDifferent)
            {
                return DataGridViewCellStyleDifferences.AffectPreferredSize;
            }
            else if (preferredSizeNonAffectingPropDifferent)
            {
                return DataGridViewCellStyleDifferences.DoNotAffectPreferredSize;
            }
            else
            {
                return DataGridViewCellStyleDifferences.None;
            }
        }

        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.GetHashCode"]/*' />
        public override int GetHashCode()
        {
            return WindowsFormsUtils.GetCombinedHashCodes((int) this.Alignment,
                                                              (int) this.WrapMode,
                                                              this.Padding.GetHashCode(),
                                                              this.Format.GetHashCode(),
                                                              this.BackColor.GetHashCode(),
                                                              this.ForeColor.GetHashCode(),
                                                              this.SelectionBackColor.GetHashCode(),
                                                              this.SelectionForeColor.GetHashCode(),
                                                              (this.Font == null ? 1 : this.Font.GetHashCode()),
                                                              (this.NullValue == null ? 1 : this.NullValue.GetHashCode()),
                                                              (this.DataSourceNullValue == null ? 1 : this.DataSourceNullValue.GetHashCode()),
                                                              (this.Tag == null ? 1 : this.Tag.GetHashCode()));
        }

        private void OnPropertyChanged(DataGridViewCellStylePropertyInternal property)
        {
            if (this.dataGridView != null && this.scope != DataGridViewCellStyleScopes.None)
            {
                this.dataGridView.OnCellStyleContentChanged(this, property);
            }

            /*
            if ((this.scope & DataGridViewCellStyleScopeInternal.Cell) == DataGridViewCellStyleScopeInternal.Cell)
            {
                this.dataGridView.OnDataGridViewCellsStyleChanged(EventArgs.Empty);
            }

            if ((this.scope & DataGridViewCellStyleScopeInternal.ColumnDefault) == DataGridViewCellStyleScopeInternal.ColumnDefault)
            {
                this.dataGridView.OnDataGridViewColumnsDefaultCellStyleChanged(EventArgs.Empty);
            }

            if ((this.scope & DataGridViewCellStyleScopeInternal.RowDefault) == DataGridViewCellStyleScopeInternal.RowDefault)
            {
                this.dataGridView.OnDataGridViewRowsDefaultCellStyleChanged(EventArgs.Empty);
            }

            if ((this.scope & DataGridViewCellStyleScopeInternal.DataGridViewDefault) == DataGridViewCellStyleScopeInternal.DataGridViewDefault)
            {
                this.dataGridView.OnDefaultCellStyleChanged(EventArgs.Empty);
            }

            if ((this.scope & DataGridViewCellStyleScopeInternal.DataGridViewColumnHeadersDefault) == DataGridViewCellStyleScopeInternal.DataGridViewColumnHeadersDefault)
            {
                this.dataGridView.OnColumnHeadersDefaultCellStyleChanged(EventArgs.Empty);
            }

            if ((this.scope & DataGridViewCellStyleScopeInternal.DataGridViewRowHeadersDefault) == DataGridViewCellStyleScopeInternal.DataGridViewRowHeadersDefault)
            {
                this.dataGridView.OnRowHeadersDefaultCellStyleChanged(EventArgs.Empty);
            }*/
        }

        internal void RemoveScope(DataGridViewCellStyleScopes scope)
        {
            this.scope &= ~scope;
            if (this.scope == DataGridViewCellStyleScopes.None)
            {
                this.dataGridView = null;
            }
        }

        private bool ShouldSerializeBackColor() {
            bool found;
            this.Properties.GetColor(PropBackColor, out found);
            return found;
        }

        private bool ShouldSerializeFont() {
            return this.Properties.GetObject(PropFont) != null;
        }

        private bool ShouldSerializeForeColor() {
            bool found;
            this.Properties.GetColor(PropForeColor, out found);
            return found;
        }

        private bool ShouldSerializeFormatProvider() {
            return this.Properties.GetObject(PropFormatProvider) != null;
        }

        private bool ShouldSerializePadding() {
            return this.Padding != Padding.Empty;
        }

        private bool ShouldSerializeSelectionBackColor() {
            bool found;
            this.Properties.GetObject(PropSelectionBackColor, out found);
            return found;
        }

        private bool ShouldSerializeSelectionForeColor() {
            bool found;
            this.Properties.GetColor(PropSelectionForeColor, out found);
            return found;
        }
        
        /// <include file='doc\DataGridViewCellStyle.uex' path='docs/doc[@for="DataGridViewCellStyle.ToString"]/*' />
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(128);
            sb.Append("DataGridViewCellStyle {");
            bool firstPropAdded = true;
            if (this.BackColor != Color.Empty)
            {
                sb.Append(" BackColor=" + this.BackColor.ToString());
                firstPropAdded = false;
            }
            if (this.ForeColor != Color.Empty)
            {
                if (!firstPropAdded)
                {
                    sb.Append(",");
                }
                sb.Append(" ForeColor=" + this.ForeColor.ToString());
                firstPropAdded = false;
            }
            if (this.SelectionBackColor != Color.Empty)
            {
                if (!firstPropAdded)
                {
                    sb.Append(",");
                }
                sb.Append(" SelectionBackColor=" + this.SelectionBackColor.ToString());
                firstPropAdded = false;
            }
            if (this.SelectionForeColor != Color.Empty)
            {
                if (!firstPropAdded)
                {
                    sb.Append(",");
                }
                sb.Append(" SelectionForeColor=" + this.SelectionForeColor.ToString());
                firstPropAdded = false;
            }
            if (this.Font != null)
            {
                if (!firstPropAdded)
                {
                    sb.Append(",");
                }
                sb.Append(" Font=" + this.Font.ToString());
                firstPropAdded = false;
            }
            if (!this.IsNullValueDefault && this.NullValue != null)
            {
                if (!firstPropAdded)
                {
                    sb.Append(",");
                }
                sb.Append(" NullValue=" + this.NullValue.ToString());
                firstPropAdded = false;
            }
            if (!this.IsDataSourceNullValueDefault && this.DataSourceNullValue != null)
            {
                if (!firstPropAdded)
                {
                    sb.Append(",");
                }
                sb.Append(" DataSourceNullValue=" + this.DataSourceNullValue.ToString());
                firstPropAdded = false;
            }
            if (!string.IsNullOrEmpty(this.Format))
            {
                if (!firstPropAdded)
                {
                    sb.Append(",");
                }
                sb.Append(" Format=" + this.Format);
                firstPropAdded = false;
            }
            if (this.WrapMode != DataGridViewTriState.NotSet)
            {
                if (!firstPropAdded)
                {
                    sb.Append(",");
                }
                sb.Append(" WrapMode=" + this.WrapMode.ToString());
                firstPropAdded = false;
            }
            if (this.Alignment != DataGridViewContentAlignment.NotSet)
            {
                if (!firstPropAdded)
                {
                    sb.Append(",");
                }
                sb.Append(" Alignment=" + this.Alignment.ToString());
                firstPropAdded = false;
            }
            if (this.Padding != Padding.Empty)
            {
                if (!firstPropAdded)
                {
                    sb.Append(",");
                }
                sb.Append(" Padding=" + this.Padding.ToString());
                firstPropAdded = false;
            }
            if (this.Tag != null)
            {
                if (!firstPropAdded)
                {
                    sb.Append(",");
                }
                sb.Append(" Tag=" + this.Tag.ToString());
                firstPropAdded = false;
            }
            sb.Append(" }");
            return sb.ToString();
        }

        object ICloneable.Clone() {
            return Clone();
        }

        internal enum DataGridViewCellStylePropertyInternal
        {
            Color,
            Other,
            Font,
            ForeColor
        }
    }
}

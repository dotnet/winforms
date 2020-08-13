// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Text;

namespace System.Windows.Forms
{
    [TypeConverter(typeof(DataGridViewCellStyleConverter))]
    [Editor("System.Windows.Forms.Design.DataGridViewCellStyleEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
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

        private DataGridViewCellStyleScopes scope;
        private readonly PropertyStore propertyStore;          // Contains all properties that are not always set.
        private DataGridView dataGridView;

        /// <summary>
        ///  Initializes a new instance of the <see cref='DataGridViewCellStyle'/> class.
        /// </summary>
        public DataGridViewCellStyle()
        {
            propertyStore = new PropertyStore();
            scope = DataGridViewCellStyleScopes.None;
        }

        public DataGridViewCellStyle(DataGridViewCellStyle dataGridViewCellStyle)
        {
            if (dataGridViewCellStyle is null)
            {
                throw new ArgumentNullException(nameof(dataGridViewCellStyle));
            }
            propertyStore = new PropertyStore();
            scope = DataGridViewCellStyleScopes.None;
            BackColor = dataGridViewCellStyle.BackColor;
            ForeColor = dataGridViewCellStyle.ForeColor;
            SelectionBackColor = dataGridViewCellStyle.SelectionBackColor;
            SelectionForeColor = dataGridViewCellStyle.SelectionForeColor;
            Font = dataGridViewCellStyle.Font;
            NullValue = dataGridViewCellStyle.NullValue;
            DataSourceNullValue = dataGridViewCellStyle.DataSourceNullValue;
            Format = dataGridViewCellStyle.Format;
            if (!dataGridViewCellStyle.IsFormatProviderDefault)
            {
                FormatProvider = dataGridViewCellStyle.FormatProvider;
            }
            AlignmentInternal = dataGridViewCellStyle.Alignment;
            WrapModeInternal = dataGridViewCellStyle.WrapMode;
            Tag = dataGridViewCellStyle.Tag;
            PaddingInternal = dataGridViewCellStyle.Padding;
        }

        [SRDescription(nameof(SR.DataGridViewCellStyleAlignmentDescr))]
        [DefaultValue(DataGridViewContentAlignment.NotSet)]
        [SRCategory(nameof(SR.CatLayout))]
        public DataGridViewContentAlignment Alignment
        {
            get
            {
                int alignment = Properties.GetInteger(PropAlignment, out bool found);
                if (found)
                {
                    return (DataGridViewContentAlignment)alignment;
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
                AlignmentInternal = value;
            }
        }

        internal DataGridViewContentAlignment AlignmentInternal
        {
            set
            {
                Debug.Assert(Enum.IsDefined(typeof(DataGridViewContentAlignment), value));
                if (Alignment != value)
                {
                    Properties.SetInteger(PropAlignment, (int)value);
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
                }
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        public Color BackColor
        {
            get
            {
                return Properties.GetColor(PropBackColor);
            }
            set
            {
                Color c = BackColor;
                if (!value.IsEmpty || Properties.ContainsObject(PropBackColor))
                {
                    Properties.SetColor(PropBackColor, value);
                }
                if (!c.Equals(BackColor))
                {
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.Color);
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object DataSourceNullValue
        {
            get
            {
                if (Properties.ContainsObject(PropDataSourceNullValue))
                {
                    return Properties.GetObject(PropDataSourceNullValue);
                }
                return System.DBNull.Value;
            }
            set
            {
                object oldDataSourceNullValue = DataSourceNullValue;

                if ((oldDataSourceNullValue == value) ||
                    (oldDataSourceNullValue != null && oldDataSourceNullValue.Equals(value)))
                {
                    return;
                }

                if (value == System.DBNull.Value &&
                    Properties.ContainsObject(PropDataSourceNullValue))
                {
                    Properties.RemoveObject(PropDataSourceNullValue);
                }
                else
                {
                    Properties.SetObject(PropDataSourceNullValue, value);
                }

                Debug.Assert((oldDataSourceNullValue is null && DataSourceNullValue != null) ||
                             (oldDataSourceNullValue != null && DataSourceNullValue is null) ||
                             (oldDataSourceNullValue != DataSourceNullValue && !oldDataSourceNullValue.Equals(DataSourceNullValue)));

                OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        public Font Font
        {
            get
            {
                return (Font)Properties.GetObject(PropFont);
            }
            set
            {
                Font f = Font;
                if (value != null || Properties.ContainsObject(PropFont))
                {
                    Properties.SetObject(PropFont, value);
                }
                if ((f is null && value != null) ||
                    (f != null && value is null) ||
                    (f != null && value != null && !f.Equals(Font)))
                {
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.Font);
                }
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        public Color ForeColor
        {
            get
            {
                return Properties.GetColor(PropForeColor);
            }
            set
            {
                Color c = ForeColor;
                if (!value.IsEmpty || Properties.ContainsObject(PropForeColor))
                {
                    Properties.SetColor(PropForeColor, value);
                }
                if (!c.Equals(ForeColor))
                {
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.ForeColor);
                }
            }
        }

        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.FormatStringEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [SRCategory(nameof(SR.CatBehavior))]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string Format
        {
            get
            {
                object format = Properties.GetObject(PropFormat);
                if (format is null)
                {
                    return string.Empty;
                }
                else
                {
                    return (string)format;
                }
            }
            set
            {
                string format = Format;
                if ((value != null && value.Length > 0) || Properties.ContainsObject(PropFormat))
                {
                    Properties.SetObject(PropFormat, value);
                }
                if (!format.Equals(Format))
                {
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IFormatProvider FormatProvider
        {
            get
            {
                object formatProvider = Properties.GetObject(PropFormatProvider);
                if (formatProvider is null)
                {
                    return System.Globalization.CultureInfo.CurrentCulture;
                }
                else
                {
                    return (IFormatProvider)formatProvider;
                }
            }
            set
            {
                object originalFormatProvider = Properties.GetObject(PropFormatProvider);
                Properties.SetObject(PropFormatProvider, value);
                if (value != originalFormatProvider)
                {
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsDataSourceNullValueDefault
        {
            get
            {
                if (!Properties.ContainsObject(PropDataSourceNullValue))
                {
                    return true;
                }
                return Properties.GetObject(PropDataSourceNullValue) == System.DBNull.Value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsFormatProviderDefault
        {
            get
            {
                return Properties.GetObject(PropFormatProvider) is null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsNullValueDefault
        {
            get
            {
                if (!Properties.ContainsObject(PropNullValue))
                {
                    return true;
                }

                object nullValue = Properties.GetObject(PropNullValue);
                return nullValue is string nullValueString && nullValueString.Length == 0;
            }
        }

        [DefaultValue("")]
        [TypeConverter(typeof(StringConverter))]
        [SRCategory(nameof(SR.CatData))]
        public object NullValue
        {
            get
            {
                if (Properties.ContainsObject(PropNullValue))
                {
                    return Properties.GetObject(PropNullValue);
                }
                return string.Empty;
            }
            set
            {
                object oldNullValue = NullValue;

                if ((oldNullValue == value) ||
                    (oldNullValue != null && oldNullValue.Equals(value)))
                {
                    return;
                }

                if (value is string stringValue && stringValue.Length == 0 && Properties.ContainsObject(PropNullValue))
                {
                    Properties.RemoveObject(PropNullValue);
                }
                else
                {
                    Properties.SetObject(PropNullValue, value);
                }

                Debug.Assert((oldNullValue is null && NullValue != null) ||
                             (oldNullValue != null && NullValue is null) ||
                             (oldNullValue != NullValue && !oldNullValue.Equals(NullValue)));

                OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
            }
        }

        [SRCategory(nameof(SR.CatLayout))]
        public Padding Padding
        {
            get => Properties.GetPadding(PropPadding, out _);
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
                PaddingInternal = value;
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
                if (value != Padding)
                {
                    Properties.SetPadding(PropPadding, value);
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
                }
            }
        }

        internal PropertyStore Properties
        {
            get
            {
                return propertyStore;
            }
        }

        internal DataGridViewCellStyleScopes Scope
        {
            get
            {
                return scope;
            }
            set
            {
                scope = value;
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        public Color SelectionBackColor
        {
            get
            {
                return Properties.GetColor(PropSelectionBackColor);
            }
            set
            {
                Color c = SelectionBackColor;
                if (!value.IsEmpty || Properties.ContainsObject(PropSelectionBackColor))
                {
                    Properties.SetColor(PropSelectionBackColor, value);
                }
                if (!c.Equals(SelectionBackColor))
                {
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.Color);
                }
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        public Color SelectionForeColor
        {
            get
            {
                return Properties.GetColor(PropSelectionForeColor);
            }
            set
            {
                Color c = SelectionForeColor;
                if (!value.IsEmpty || Properties.ContainsObject(PropSelectionForeColor))
                {
                    Properties.SetColor(PropSelectionForeColor, value);
                }
                if (!c.Equals(SelectionForeColor))
                {
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.Color);
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Tag
        {
            get
            {
                return Properties.GetObject(PropTag);
            }
            set
            {
                if (value != null || Properties.ContainsObject(PropTag))
                {
                    Properties.SetObject(PropTag, value);
                }
            }
        }

        [DefaultValue(DataGridViewTriState.NotSet)]
        [SRCategory(nameof(SR.CatLayout))]
        public DataGridViewTriState WrapMode
        {
            get
            {
                int wrap = Properties.GetInteger(PropWrapMode, out bool found);
                if (found)
                {
                    return (DataGridViewTriState)wrap;
                }
                return DataGridViewTriState.NotSet;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewTriState.NotSet, (int)DataGridViewTriState.False))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewTriState));
                }
                WrapModeInternal = value;
            }
        }

        internal DataGridViewTriState WrapModeInternal
        {
            set
            {
                Debug.Assert(value >= DataGridViewTriState.NotSet && value <= DataGridViewTriState.False);
                if (WrapMode != value)
                {
                    Properties.SetInteger(PropWrapMode, (int)value);
                    OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
                }
            }
        }

        internal void AddScope(DataGridView dataGridView, DataGridViewCellStyleScopes scope)
        {
            this.scope |= scope;
            this.dataGridView = dataGridView;
        }

        public virtual void ApplyStyle(DataGridViewCellStyle dataGridViewCellStyle)
        {
            if (dataGridViewCellStyle is null)
            {
                throw new ArgumentNullException(nameof(dataGridViewCellStyle));
            }
            if (!dataGridViewCellStyle.BackColor.IsEmpty)
            {
                BackColor = dataGridViewCellStyle.BackColor;
            }
            if (!dataGridViewCellStyle.ForeColor.IsEmpty)
            {
                ForeColor = dataGridViewCellStyle.ForeColor;
            }
            if (!dataGridViewCellStyle.SelectionBackColor.IsEmpty)
            {
                SelectionBackColor = dataGridViewCellStyle.SelectionBackColor;
            }
            if (!dataGridViewCellStyle.SelectionForeColor.IsEmpty)
            {
                SelectionForeColor = dataGridViewCellStyle.SelectionForeColor;
            }
            if (dataGridViewCellStyle.Font != null)
            {
                Font = dataGridViewCellStyle.Font;
            }
            if (!dataGridViewCellStyle.IsNullValueDefault)
            {
                NullValue = dataGridViewCellStyle.NullValue;
            }
            if (!dataGridViewCellStyle.IsDataSourceNullValueDefault)
            {
                DataSourceNullValue = dataGridViewCellStyle.DataSourceNullValue;
            }
            if (dataGridViewCellStyle.Format.Length != 0)
            {
                Format = dataGridViewCellStyle.Format;
            }
            if (!dataGridViewCellStyle.IsFormatProviderDefault)
            {
                FormatProvider = dataGridViewCellStyle.FormatProvider;
            }
            if (dataGridViewCellStyle.Alignment != DataGridViewContentAlignment.NotSet)
            {
                AlignmentInternal = dataGridViewCellStyle.Alignment;
            }
            if (dataGridViewCellStyle.WrapMode != DataGridViewTriState.NotSet)
            {
                WrapModeInternal = dataGridViewCellStyle.WrapMode;
            }
            if (dataGridViewCellStyle.Tag != null)
            {
                Tag = dataGridViewCellStyle.Tag;
            }
            if (dataGridViewCellStyle.Padding != Padding.Empty)
            {
                PaddingInternal = dataGridViewCellStyle.Padding;
            }
        }

        public virtual DataGridViewCellStyle Clone()
        {
            return new DataGridViewCellStyle(this);
        }

        public override bool Equals(object o)
        {
            if (o is DataGridViewCellStyle dgvcs)
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
                    dgvcs.Alignment != Alignment ||
                    dgvcs.DataSourceNullValue != DataSourceNullValue ||
                    dgvcs.Font != Font ||
                    dgvcs.Format != Format ||
                    dgvcs.FormatProvider != FormatProvider ||
                    dgvcs.NullValue != NullValue ||
                    dgvcs.Padding != Padding ||
                    dgvcs.Tag != Tag ||
                    dgvcs.WrapMode != WrapMode);

            bool preferredSizeNonAffectingPropDifferent = (
                    dgvcs.BackColor != BackColor ||
                    dgvcs.ForeColor != ForeColor ||
                    dgvcs.SelectionBackColor != SelectionBackColor ||
                    dgvcs.SelectionForeColor != SelectionForeColor);

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

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Alignment);
            hash.Add(WrapMode);
            hash.Add(Padding);
            hash.Add(Format);
            hash.Add(BackColor);
            hash.Add(ForeColor);
            hash.Add(SelectionBackColor);
            hash.Add(SelectionForeColor);
            hash.Add(Font);
            hash.Add(NullValue);
            hash.Add(DataSourceNullValue);
            hash.Add(Tag);
            return hash.ToHashCode();
        }

        private void OnPropertyChanged(DataGridViewCellStylePropertyInternal property)
        {
            if (dataGridView != null && scope != DataGridViewCellStyleScopes.None)
            {
                dataGridView.OnCellStyleContentChanged(this, property);
            }
        }

        internal void RemoveScope(DataGridViewCellStyleScopes scope)
        {
            this.scope &= ~scope;
            if (this.scope == DataGridViewCellStyleScopes.None)
            {
                dataGridView = null;
            }
        }

        private bool ShouldSerializeBackColor()
        {
            Properties.GetColor(PropBackColor, out bool found);
            return found;
        }

        private bool ShouldSerializeFont()
        {
            return Properties.GetObject(PropFont) != null;
        }

        private bool ShouldSerializeForeColor()
        {
            Properties.GetColor(PropForeColor, out bool found);
            return found;
        }

        private bool ShouldSerializeFormatProvider()
        {
            return Properties.GetObject(PropFormatProvider) != null;
        }

        private bool ShouldSerializePadding()
        {
            return Padding != Padding.Empty;
        }

        private bool ShouldSerializeSelectionBackColor()
        {
            Properties.GetObject(PropSelectionBackColor, out bool found);
            return found;
        }

        private bool ShouldSerializeSelectionForeColor()
        {
            Properties.GetColor(PropSelectionForeColor, out bool found);
            return found;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(128);
            sb.Append("DataGridViewCellStyle {");
            bool firstPropAdded = true;
            if (BackColor != Color.Empty)
            {
                sb.Append(" BackColor=" + BackColor.ToString());
                firstPropAdded = false;
            }
            if (ForeColor != Color.Empty)
            {
                if (!firstPropAdded)
                {
                    sb.Append(',');
                }
                sb.Append(" ForeColor=" + ForeColor.ToString());
                firstPropAdded = false;
            }
            if (SelectionBackColor != Color.Empty)
            {
                if (!firstPropAdded)
                {
                    sb.Append(',');
                }
                sb.Append(" SelectionBackColor=" + SelectionBackColor.ToString());
                firstPropAdded = false;
            }
            if (SelectionForeColor != Color.Empty)
            {
                if (!firstPropAdded)
                {
                    sb.Append(',');
                }
                sb.Append(" SelectionForeColor=" + SelectionForeColor.ToString());
                firstPropAdded = false;
            }
            if (Font != null)
            {
                if (!firstPropAdded)
                {
                    sb.Append(',');
                }
                sb.Append(" Font=" + Font.ToString());
                firstPropAdded = false;
            }
            if (!IsNullValueDefault && NullValue != null)
            {
                if (!firstPropAdded)
                {
                    sb.Append(',');
                }
                sb.Append(" NullValue=" + NullValue.ToString());
                firstPropAdded = false;
            }
            if (!IsDataSourceNullValueDefault && DataSourceNullValue != null)
            {
                if (!firstPropAdded)
                {
                    sb.Append(',');
                }
                sb.Append(" DataSourceNullValue=" + DataSourceNullValue.ToString());
                firstPropAdded = false;
            }
            if (!string.IsNullOrEmpty(Format))
            {
                if (!firstPropAdded)
                {
                    sb.Append(',');
                }
                sb.Append(" Format=" + Format);
                firstPropAdded = false;
            }
            if (WrapMode != DataGridViewTriState.NotSet)
            {
                if (!firstPropAdded)
                {
                    sb.Append(',');
                }
                sb.Append(" WrapMode=" + WrapMode.ToString());
                firstPropAdded = false;
            }
            if (Alignment != DataGridViewContentAlignment.NotSet)
            {
                if (!firstPropAdded)
                {
                    sb.Append(',');
                }
                sb.Append(" Alignment=" + Alignment.ToString());
                firstPropAdded = false;
            }
            if (Padding != Padding.Empty)
            {
                if (!firstPropAdded)
                {
                    sb.Append(',');
                }
                sb.Append(" Padding=" + Padding.ToString());
                firstPropAdded = false;
            }
            if (Tag != null)
            {
                if (!firstPropAdded)
                {
                    sb.Append(',');
                }
                sb.Append(" Tag=" + Tag.ToString());
                firstPropAdded = false;
            }
            sb.Append(" }");
            return sb.ToString();
        }

        object ICloneable.Clone()
        {
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

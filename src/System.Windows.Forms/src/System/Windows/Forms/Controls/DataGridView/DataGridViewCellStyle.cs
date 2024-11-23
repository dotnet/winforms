// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text;

namespace System.Windows.Forms;

[TypeConverter(typeof(DataGridViewCellStyleConverter))]
[Editor($"System.Windows.Forms.Design.DataGridViewCellStyleEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
public class DataGridViewCellStyle : ICloneable
{
    private static readonly int s_propAlignment = PropertyStore.CreateKey();
    private static readonly int s_propBackColor = PropertyStore.CreateKey();
    private static readonly int s_propDataSourceNullValue = PropertyStore.CreateKey();
    private static readonly int s_propFont = PropertyStore.CreateKey();
    private static readonly int s_propForeColor = PropertyStore.CreateKey();
    private static readonly int s_propFormat = PropertyStore.CreateKey();
    private static readonly int s_propFormatProvider = PropertyStore.CreateKey();
    private static readonly int s_propNullValue = PropertyStore.CreateKey();
    private static readonly int s_propPadding = PropertyStore.CreateKey();
    private static readonly int s_propSelectionBackColor = PropertyStore.CreateKey();
    private static readonly int s_propSelectionForeColor = PropertyStore.CreateKey();
    private static readonly int s_propTag = PropertyStore.CreateKey();
    private static readonly int s_propWrapMode = PropertyStore.CreateKey();
    private DataGridView? _dataGridView;

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataGridViewCellStyle"/> class.
    /// </summary>
    public DataGridViewCellStyle()
    {
        Properties = new PropertyStore();
        Scope = DataGridViewCellStyleScopes.None;
    }

    public DataGridViewCellStyle(DataGridViewCellStyle dataGridViewCellStyle)
    {
        ArgumentNullException.ThrowIfNull(dataGridViewCellStyle);

        Properties = new PropertyStore();
        Scope = DataGridViewCellStyleScopes.None;
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
        get => Properties.GetValueOrDefault(s_propAlignment, DataGridViewContentAlignment.NotSet);
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
            Debug.Assert(Enum.IsDefined(value));
            if (Alignment != value)
            {
                Properties.AddOrRemoveValue(s_propAlignment, value, defaultValue: DataGridViewContentAlignment.NotSet);
                OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
            }
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    public Color BackColor
    {
        get => Properties.GetValueOrDefault<Color>(s_propBackColor);
        set
        {
            Color previous = Properties.AddOrRemoveValue(s_propBackColor, value);
            if (!previous.Equals(value))
            {
                OnPropertyChanged(DataGridViewCellStylePropertyInternal.Color);
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? DataSourceNullValue
    {
        get => Properties.TryGetValueOrNull(s_propDataSourceNullValue, out object? value)
            ? value
            : DBNull.Value;
        set
        {
            // Deliberately don't change the value if it is "Equal".
            if (Properties.TryGetValueOrNull(s_propDataSourceNullValue, out object? oldValue)
                && Equals(oldValue, value))
            {
                return;
            }

            Properties.AddOrRemoveValue(s_propDataSourceNullValue, value, DBNull.Value);
            OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    public Font? Font
    {
        get => Properties.GetValueOrDefault<Font>(s_propFont);
        set
        {
            Font? previous = Properties.AddOrRemoveValue(s_propFont, value);

            if (previous != value || (previous is not null && !previous.Equals(value)))
            {
                OnPropertyChanged(DataGridViewCellStylePropertyInternal.Font);
            }
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    public Color ForeColor
    {
        get => Properties.TryGetValue(s_propForeColor, out Color color) ? color : Color.Empty;
        set
        {
            Color previous = Properties.AddOrRemoveValue(s_propForeColor, value);
            if (!previous.Equals(value))
            {
                OnPropertyChanged(DataGridViewCellStylePropertyInternal.ForeColor);
            }
        }
    }

    [DefaultValue("")]
    [Editor($"System.Windows.Forms.Design.FormatStringEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [SRCategory(nameof(SR.CatBehavior))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [AllowNull]
    public string Format
    {
        get => Properties.GetStringOrEmptyString(s_propFormat);
        set
        {
            if (Properties.AddOrRemoveString(s_propFormat, value))
            {
                OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [AllowNull]
    public IFormatProvider FormatProvider
    {
        get => Properties.GetValueOrDefault<IFormatProvider>(s_propFormatProvider) ?? Globalization.CultureInfo.CurrentCulture;
        set
        {
            IFormatProvider? originalValue = Properties.AddOrRemoveValue(s_propFormatProvider, value);
            if (value != originalValue)
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
            if (!Properties.TryGetValueOrNull(s_propDataSourceNullValue, out object? value))
            {
                return true;
            }

            return value == DBNull.Value;
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool IsFormatProviderDefault => !Properties.ContainsKey(s_propFormatProvider);

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool IsNullValueDefault
    {
        get
        {
            if (!Properties.TryGetValueOrNull(s_propNullValue, out object? nullValue))
            {
                return true;
            }

            return nullValue is string nullValueString && nullValueString.Length == 0;
        }
    }

    [DefaultValue("")]
    [TypeConverter(typeof(StringConverter))]
    [SRCategory(nameof(SR.CatData))]
    public object? NullValue
    {
        get => Properties.GetValueOrDefaultAllowNull<object?>(s_propNullValue, string.Empty);
        set
        {
            object? originalValue = NullValue;

            if (Equals(originalValue, value))
            {
                return;
            }

            if (Properties.AddOrRemoveValue(s_propNullValue, value, defaultValue: string.Empty) != value)
            {
                OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
            }
        }
    }

    [SRCategory(nameof(SR.CatLayout))]
    public Padding Padding
    {
        get => Properties.GetValueOrDefault<Padding>(s_propPadding);
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
                Properties.AddValue(s_propPadding, value);
                OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
            }
        }
    }

    internal PropertyStore Properties { get; }

    internal DataGridViewCellStyleScopes Scope { get; set; }

    [SRCategory(nameof(SR.CatAppearance))]
    public Color SelectionBackColor
    {
        get => Properties.GetValueOrDefault<Color>(s_propSelectionBackColor);
        set
        {
            Color previous = Properties.AddOrRemoveValue(s_propSelectionBackColor, value);
            if (!previous.Equals(value))
            {
                OnPropertyChanged(DataGridViewCellStylePropertyInternal.Color);
            }
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    public Color SelectionForeColor
    {
        get => Properties.GetValueOrDefault<Color>(s_propSelectionForeColor);
        set
        {
            Color previous = Properties.AddOrRemoveValue(s_propSelectionForeColor, value);
            if (!previous.Equals(value))
            {
                OnPropertyChanged(DataGridViewCellStylePropertyInternal.Color);
            }
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? Tag
    {
        get => Properties.GetValueOrDefault<object?>(s_propTag);
        set => Properties.AddOrRemoveValue(s_propTag, value);
    }

    [DefaultValue(DataGridViewTriState.NotSet)]
    [SRCategory(nameof(SR.CatLayout))]
    public DataGridViewTriState WrapMode
    {
        get => Properties.GetValueOrDefault(s_propWrapMode, DataGridViewTriState.NotSet);
        set
        {
            // Sequential enum. Valid values are 0x0 to 0x2
            SourceGenerated.EnumValidator.Validate(value);
            WrapModeInternal = value;
        }
    }

    internal DataGridViewTriState WrapModeInternal
    {
        set
        {
            Debug.Assert(value is >= DataGridViewTriState.NotSet and <= DataGridViewTriState.False);
            if (WrapMode != value)
            {
                Properties.AddOrRemoveValue(s_propWrapMode, value, defaultValue: DataGridViewTriState.NotSet);
                OnPropertyChanged(DataGridViewCellStylePropertyInternal.Other);
            }
        }
    }

    internal void AddScope(DataGridView? dataGridView, DataGridViewCellStyleScopes scope)
    {
        Scope |= scope;
        _dataGridView = dataGridView;
    }

    public virtual void ApplyStyle(DataGridViewCellStyle dataGridViewCellStyle)
    {
        ArgumentNullException.ThrowIfNull(dataGridViewCellStyle);

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

        if (dataGridViewCellStyle.Font is not null)
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

        if (dataGridViewCellStyle.Tag is not null)
        {
            Tag = dataGridViewCellStyle.Tag;
        }

        if (dataGridViewCellStyle.Padding != Padding.Empty)
        {
            PaddingInternal = dataGridViewCellStyle.Padding;
        }
    }

    public virtual DataGridViewCellStyle Clone() => new(this);

    public override bool Equals(object? o) =>
        o is DataGridViewCellStyle dgvcs && GetDifferencesFrom(dgvcs) == DataGridViewCellStyleDifferences.None;

    internal DataGridViewCellStyleDifferences GetDifferencesFrom(DataGridViewCellStyle dgvcs)
    {
        Debug.Assert(dgvcs is not null);

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
        HashCode hash = default;
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
        if (_dataGridView is not null && Scope != DataGridViewCellStyleScopes.None)
        {
            _dataGridView.OnCellStyleContentChanged(this, property);
        }
    }

    internal void RemoveScope(DataGridViewCellStyleScopes scope)
    {
        Scope &= ~scope;
        if (Scope == DataGridViewCellStyleScopes.None)
        {
            _dataGridView = null;
        }
    }

    private bool ShouldSerializeBackColor() => Properties.ContainsKey(s_propBackColor);

    private bool ShouldSerializeFont() => Properties.ContainsKey(s_propFont);

    private bool ShouldSerializeForeColor() => Properties.ContainsKey(s_propForeColor);

    private bool ShouldSerializeFormatProvider() => Properties.ContainsKey(s_propFormatProvider);

    private bool ShouldSerializePadding() => Properties.ContainsKey(s_propPadding);

    private bool ShouldSerializeSelectionBackColor() => Properties.ContainsKey(s_propSelectionBackColor);

    private bool ShouldSerializeSelectionForeColor() => Properties.ContainsKey(s_propSelectionForeColor);

    public override string ToString()
    {
        StringBuilder sb = new(128);
        sb.Append("DataGridViewCellStyle {");
        bool firstPropAdded = true;
        if (BackColor != Color.Empty)
        {
            sb.Append($" BackColor={BackColor}");
            firstPropAdded = false;
        }

        if (ForeColor != Color.Empty)
        {
            if (!firstPropAdded)
            {
                sb.Append(',');
            }

            sb.Append($" ForeColor={ForeColor}");
            firstPropAdded = false;
        }

        if (SelectionBackColor != Color.Empty)
        {
            if (!firstPropAdded)
            {
                sb.Append(',');
            }

            sb.Append($" SelectionBackColor={SelectionBackColor}");
            firstPropAdded = false;
        }

        if (SelectionForeColor != Color.Empty)
        {
            if (!firstPropAdded)
            {
                sb.Append(',');
            }

            sb.Append($" SelectionForeColor={SelectionForeColor}");
            firstPropAdded = false;
        }

        if (Font is not null)
        {
            if (!firstPropAdded)
            {
                sb.Append(',');
            }

            sb.Append($" Font={Font}");
            firstPropAdded = false;
        }

        if (!IsNullValueDefault && NullValue is not null)
        {
            if (!firstPropAdded)
            {
                sb.Append(',');
            }

            sb.Append($" NullValue={NullValue}");
            firstPropAdded = false;
        }

        if (!IsDataSourceNullValueDefault && DataSourceNullValue is not null)
        {
            if (!firstPropAdded)
            {
                sb.Append(',');
            }

            sb.Append($" DataSourceNullValue={DataSourceNullValue}");
            firstPropAdded = false;
        }

        if (!string.IsNullOrEmpty(Format))
        {
            if (!firstPropAdded)
            {
                sb.Append(',');
            }

            sb.Append($" Format={Format}");
            firstPropAdded = false;
        }

        if (WrapMode != DataGridViewTriState.NotSet)
        {
            if (!firstPropAdded)
            {
                sb.Append(',');
            }

            sb.Append($" WrapMode={WrapMode}");
            firstPropAdded = false;
        }

        if (Alignment != DataGridViewContentAlignment.NotSet)
        {
            if (!firstPropAdded)
            {
                sb.Append(',');
            }

            sb.Append($" Alignment={Alignment}");
            firstPropAdded = false;
        }

        if (Padding != Padding.Empty)
        {
            if (!firstPropAdded)
            {
                sb.Append(',');
            }

            sb.Append($" Padding={Padding}");
            firstPropAdded = false;
        }

        if (Tag is not null)
        {
            if (!firstPropAdded)
            {
                sb.Append(',');
            }

            sb.Append($" Tag={Tag}");
        }

        sb.Append(" }");
        return sb.ToString();
    }

    object ICloneable.Clone() => Clone();

    internal enum DataGridViewCellStylePropertyInternal
    {
        Color,
        Other,
        Font,
        ForeColor
    }
}

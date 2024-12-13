// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Text;

namespace System.Drawing;

/// <summary>
///  Encapsulates text layout information (such as alignment and linespacing), display manipulations (such as
///  ellipsis insertion and national digit substitution) and OpenType features.
/// </summary>
public sealed unsafe class StringFormat : MarshalByRefObject, ICloneable, IDisposable
{
    internal GpStringFormat* _nativeFormat;

    private StringFormat(GpStringFormat* format) => _nativeFormat = format;

    /// <summary>
    ///  Initializes a new instance of the <see cref='StringFormat'/> class.
    /// </summary>
    public StringFormat() : this(0, 0)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='StringFormat'/> class with the specified <see cref='StringFormatFlags'/>.
    /// </summary>
    public StringFormat(StringFormatFlags options) : this(options, 0)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='StringFormat'/> class with the specified
    ///  <see cref='StringFormatFlags'/> and language.
    /// </summary>
    public StringFormat(StringFormatFlags options, int language)
    {
        GpStringFormat* format;
        PInvokeGdiPlus.GdipCreateStringFormat((int)options, (ushort)language, &format).ThrowIfFailed();
        _nativeFormat = format;
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='StringFormat'/> class from the specified
    ///  existing <see cref='StringFormat'/>.
    /// </summary>
    public StringFormat(StringFormat format)
    {
        ArgumentNullException.ThrowIfNull(format);
        GpStringFormat* newFormat;
        PInvokeGdiPlus.GdipCloneStringFormat(format._nativeFormat, &newFormat).ThrowIfFailed();
        _nativeFormat = newFormat;
        GC.KeepAlive(format);
    }

    /// <summary>
    ///  Cleans up Windows resources for this <see cref='StringFormat'/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_nativeFormat is not null)
        {
            try
            {
#if DEBUG
                Status status = !Gdip.Initialized ? Status.Ok :
#endif
                PInvokeGdiPlus.GdipDeleteStringFormat(_nativeFormat);
#if DEBUG
                Debug.Assert(status == Status.Ok, $"GDI+ returned an error status: {status}");
#endif
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsCriticalException(ex))
                {
                    throw;
                }

                Debug.Fail($"Exception thrown during Dispose: {ex}");
            }
            finally
            {
                _nativeFormat = null;
            }
        }
    }

    /// <summary>
    ///  Creates an exact copy of this <see cref='StringFormat'/>.
    /// </summary>
    public object Clone() => new StringFormat(this);

    /// <summary>
    ///  Gets or sets a <see cref='StringFormatFlags'/> that contains formatting information.
    /// </summary>
    public StringFormatFlags FormatFlags
    {
        get
        {
            StringFormatFlags format;
            PInvokeGdiPlus.GdipGetStringFormatFlags(_nativeFormat, (int*)&format).ThrowIfFailed();
            GC.KeepAlive(this);
            return format;
        }
        set
        {
            PInvokeGdiPlus.GdipSetStringFormatFlags(_nativeFormat, (int)value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Sets the measure of characters to the specified range.
    /// </summary>
    public void SetMeasurableCharacterRanges(CharacterRange[] ranges)
    {
        ArgumentNullException.ThrowIfNull(ranges);

        // Passing no count will clear the ranges, but it still requires a valid pointer. Taking the address of an
        // empty array gives a null pointer, so we need to pass a dummy value.
        GdiPlus.CharacterRange stub;
        fixed (void* r = ranges)
        {
            PInvokeGdiPlus.GdipSetStringFormatMeasurableCharacterRanges(
                _nativeFormat,
                ranges.Length,
                r is null ? &stub : (GdiPlus.CharacterRange*)r).ThrowIfFailed();
        }

        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Specifies text alignment information.
    /// </summary>
    public StringAlignment Alignment
    {
        get
        {
            StringAlignment alignment;
            PInvokeGdiPlus.GdipGetStringFormatAlign(_nativeFormat, (GdiPlus.StringAlignment*)&alignment).ThrowIfFailed();
            GC.KeepAlive(this);
            return alignment;
        }
        set
        {
            if (value is < StringAlignment.Near or > StringAlignment.Far)
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(StringAlignment));
            }

            PInvokeGdiPlus.GdipSetStringFormatAlign(_nativeFormat, (GdiPlus.StringAlignment)value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Gets or sets the line alignment.
    /// </summary>
    public StringAlignment LineAlignment
    {
        get
        {
            StringAlignment alignment;
            PInvokeGdiPlus.GdipGetStringFormatLineAlign(_nativeFormat, (GdiPlus.StringAlignment*)&alignment).ThrowIfFailed();
            GC.KeepAlive(this);
            return alignment;
        }
        set
        {
            if (value is < 0 or > StringAlignment.Far)
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(StringAlignment));
            }

            PInvokeGdiPlus.GdipSetStringFormatLineAlign(_nativeFormat, (GdiPlus.StringAlignment)value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Gets or sets the <see cref='HotkeyPrefix'/> for this <see cref='StringFormat'/> .
    /// </summary>
    public HotkeyPrefix HotkeyPrefix
    {
        get
        {
            HotkeyPrefix hotkeyPrefix;
            PInvokeGdiPlus.GdipGetStringFormatHotkeyPrefix(_nativeFormat, (int*)&hotkeyPrefix).ThrowIfFailed();
            GC.KeepAlive(this);
            return hotkeyPrefix;
        }
        set
        {
            if (value is < HotkeyPrefix.None or > HotkeyPrefix.Hide)
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(HotkeyPrefix));
            }

            PInvokeGdiPlus.GdipSetStringFormatHotkeyPrefix(_nativeFormat, (int)value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Sets tab stops for this <see cref='StringFormat'/>.
    /// </summary>
    public void SetTabStops(float firstTabOffset, float[] tabStops)
    {
        ArgumentNullException.ThrowIfNull(tabStops);

        if (firstTabOffset < 0)
        {
            throw new ArgumentException(SR.Format(SR.InvalidArgumentValue, nameof(firstTabOffset), firstTabOffset));
        }

        // To clear the tab stops you need to pass a count of 0 with a valid pointer. Taking the address of an
        // empty array gives a null pointer, so we need to pass a dummy value.
        float stub;
        fixed (float* ts = tabStops)
        {
            PInvokeGdiPlus.GdipSetStringFormatTabStops(
                _nativeFormat,
                firstTabOffset,
                tabStops.Length,
                ts is null ? &stub : ts).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Gets the tab stops for this <see cref='StringFormat'/>.
    /// </summary>
    public float[] GetTabStops(out float firstTabOffset)
    {
        int count;
        PInvokeGdiPlus.GdipGetStringFormatTabStopCount(_nativeFormat, &count).ThrowIfFailed();

        if (count == 0)
        {
            firstTabOffset = 0;
            return [];
        }

        float[] tabStops = new float[count];

        fixed (float* fto = &firstTabOffset)
        fixed (float* ts = tabStops)
        {
            PInvokeGdiPlus.GdipGetStringFormatTabStops(_nativeFormat, count, fto, ts).ThrowIfFailed();
            GC.KeepAlive(this);
            return tabStops;
        }
    }

    // String trimming. How to handle more text than can be displayed
    // in the limits available.

    /// <summary>
    ///  Gets or sets the <see cref='StringTrimming'/> for this <see cref='StringFormat'/>.
    /// </summary>
    public StringTrimming Trimming
    {
        get
        {
            StringTrimming trimming;
            PInvokeGdiPlus.GdipGetStringFormatTrimming(_nativeFormat, (GdiPlus.StringTrimming*)&trimming).ThrowIfFailed();
            GC.KeepAlive(this);
            return trimming;
        }
        set
        {
            if (value is < StringTrimming.None or > StringTrimming.EllipsisPath)
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(StringTrimming));
            }

            PInvokeGdiPlus.GdipSetStringFormatTrimming(_nativeFormat, (GdiPlus.StringTrimming)value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Gets a generic default <see cref='StringFormat'/>.
    /// </summary>
    /// <devdoc>
    ///  Remarks from MSDN: A generic, default StringFormat object has the following characteristics:
    ///
    ///     - No string format flags are set.
    ///     - Character alignment and line alignment are set to StringAlignmentNear.
    ///     - Language ID is set to neutral language, which means that the current language associated
    ///       with the calling thread is used.
    ///     - String digit substitution is set to StringDigitSubstituteUser.
    ///     - Hot key prefix is set to HotkeyPrefixNone.
    ///     - Number of tab stops is set to zero.
    ///     - String trimming is set to StringTrimmingCharacter.
    /// </devdoc>
    public static StringFormat GenericDefault
    {
        get
        {
            GpStringFormat* format;
            PInvokeGdiPlus.GdipStringFormatGetGenericDefault(&format).ThrowIfFailed();
            return new StringFormat(format);
        }
    }

    /// <summary>
    ///  Gets a generic typographic <see cref='StringFormat'/>.
    /// </summary>
    /// <devdoc>
    ///  Remarks from MSDN: A generic, typographic StringFormat object has the following characteristics:
    ///
    ///     - String format flags StringFormatFlagsLineLimit, StringFormatFlagsNoClip,
    ///       and StringFormatFlagsNoFitBlackBox are set.
    ///     - Character alignment and line alignment are set to StringAlignmentNear.
    ///     - Language ID is set to neutral language, which means that the current language associated
    ///       with the calling thread is used.
    ///     - String digit substitution is set to StringDigitSubstituteUser.
    ///     - Hot key prefix is set to HotkeyPrefixNone.
    ///     - Number of tab stops is set to zero.
    ///     - String trimming is set to StringTrimmingNone.
    /// </devdoc>
    public static StringFormat GenericTypographic
    {
        get
        {
            GpStringFormat* format;
            PInvokeGdiPlus.GdipStringFormatGetGenericTypographic(&format).ThrowIfFailed();
            return new StringFormat(format);
        }
    }

    public void SetDigitSubstitution(int language, StringDigitSubstitute substitute)
    {
        PInvokeGdiPlus.GdipSetStringFormatDigitSubstitution(
            _nativeFormat,
            (ushort)language,
            (GdiPlus.StringDigitSubstitute)substitute).ThrowIfFailed();

        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Gets the <see cref='StringDigitSubstitute'/> for this <see cref='StringFormat'/>.
    /// </summary>
    public StringDigitSubstitute DigitSubstitutionMethod
    {
        get
        {
            StringDigitSubstitute digitSubstitute;
            PInvokeGdiPlus.GdipGetStringFormatDigitSubstitution(
                _nativeFormat,
                null,
                (GdiPlus.StringDigitSubstitute*)&digitSubstitute).ThrowIfFailed();

            GC.KeepAlive(this);
            return digitSubstitute;
        }
    }

    /// <summary>
    ///  Gets the language of <see cref='StringDigitSubstitute'/> for this <see cref='StringFormat'/>.
    /// </summary>
    public int DigitSubstitutionLanguage
    {
        get
        {
            ushort language;
            PInvokeGdiPlus.GdipGetStringFormatDigitSubstitution(_nativeFormat, &language, null).ThrowIfFailed();
            GC.KeepAlive(this);
            return language;
        }
    }

    internal int GetMeasurableCharacterRangeCount()
    {
        int count;
        PInvokeGdiPlus.GdipGetStringFormatMeasurableCharacterRangeCount(_nativeFormat, &count).ThrowIfFailed();
        GC.KeepAlive(this);
        return count;
    }

    /// <summary>
    ///  Cleans up Windows resources for this <see cref='StringFormat'/>.
    /// </summary>
    ~StringFormat() => Dispose(disposing: false);

    /// <summary>
    ///  Converts this <see cref='StringFormat'/> to a human-readable string.
    /// </summary>
    public override string ToString() => $"[StringFormat, FormatFlags={FormatFlags}]";
}

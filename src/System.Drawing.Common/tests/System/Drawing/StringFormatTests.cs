// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Text;

namespace System.Drawing.Tests;

public class StringFormatTests
{
    private const int RandomLanguageCode = 10;
    private const int EnglishLanguageCode = 2057;

    [Fact]
    public void Ctor_Default()
    {
        using StringFormat format = new();
        Assert.Equal(StringAlignment.Near, format.Alignment);
        Assert.Equal(0, format.DigitSubstitutionLanguage);
        Assert.Equal(StringDigitSubstitute.User, format.DigitSubstitutionMethod);
        Assert.Equal((StringFormatFlags)0, format.FormatFlags);
        Assert.Equal(HotkeyPrefix.None, format.HotkeyPrefix);
        Assert.Equal(StringAlignment.Near, format.LineAlignment);
        Assert.Equal(StringTrimming.Character, format.Trimming);
    }

    [Theory]
    [InlineData(StringFormatFlags.DirectionRightToLeft | StringFormatFlags.DirectionVertical)]
    [InlineData((StringFormatFlags)(-1))]
    public void Ctor_Options(StringFormatFlags options)
    {
        using StringFormat format = new(options);
        Assert.Equal(StringAlignment.Near, format.Alignment);
        Assert.Equal(0, format.DigitSubstitutionLanguage);
        Assert.Equal(StringDigitSubstitute.User, format.DigitSubstitutionMethod);
        Assert.Equal(options, format.FormatFlags);
        Assert.Equal(HotkeyPrefix.None, format.HotkeyPrefix);
        Assert.Equal(StringAlignment.Near, format.LineAlignment);
        Assert.Equal(StringTrimming.Character, format.Trimming);
    }

    [Theory]
    [InlineData(StringFormatFlags.DirectionRightToLeft | StringFormatFlags.DirectionVertical, RandomLanguageCode)]
    [InlineData(StringFormatFlags.NoClip, EnglishLanguageCode)]
    [InlineData((StringFormatFlags)(-1), -1)]
    public void Ctor_Options_Language(StringFormatFlags options, int language)
    {
        using StringFormat format = new(options, language);
        Assert.Equal(StringAlignment.Near, format.Alignment);
        Assert.Equal(0, format.DigitSubstitutionLanguage);
        Assert.Equal(StringDigitSubstitute.User, format.DigitSubstitutionMethod);
        Assert.Equal(options, format.FormatFlags);
        Assert.Equal(HotkeyPrefix.None, format.HotkeyPrefix);
        Assert.Equal(StringAlignment.Near, format.LineAlignment);
        Assert.Equal(StringTrimming.Character, format.Trimming);
    }

    [Fact]
    public void Ctor_Format()
    {
        using StringFormat original = new(StringFormatFlags.NoClip, EnglishLanguageCode);
        using StringFormat format = new(original);
        Assert.Equal(StringAlignment.Near, format.Alignment);
        Assert.Equal(0, format.DigitSubstitutionLanguage);
        Assert.Equal(StringDigitSubstitute.User, format.DigitSubstitutionMethod);
        Assert.Equal(StringFormatFlags.NoClip, format.FormatFlags);
        Assert.Equal(HotkeyPrefix.None, format.HotkeyPrefix);
        Assert.Equal(StringAlignment.Near, format.LineAlignment);
        Assert.Equal(StringTrimming.Character, format.Trimming);

        // The new format is a clone.
        original.FormatFlags = StringFormatFlags.NoFontFallback;
        Assert.Equal(StringFormatFlags.NoClip, format.FormatFlags);
    }

    [Fact]
    public void Ctor_NullFormat_ThrowsArgumentNullException()
    {
        AssertExtensions.Throws<ArgumentNullException>("format", () => new StringFormat(null));
    }

    [Fact]
    public void Ctor_DisposedFormat_ThrowsArgumentException()
    {
        StringFormat format = new();
        format.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => new StringFormat(format));
    }

    [Fact]
    public void Dispose_MultipleTimes_Success()
    {
        StringFormat format = new();
        format.Dispose();
        format.Dispose();
    }

    [Fact]
    public void Clone_Valid_Success()
    {
        using StringFormat original = new(StringFormatFlags.NoClip, EnglishLanguageCode);
        using StringFormat format = Assert.IsType<StringFormat>(original.Clone());
        Assert.Equal(StringAlignment.Near, format.Alignment);
        Assert.Equal(0, format.DigitSubstitutionLanguage);
        Assert.Equal(StringDigitSubstitute.User, format.DigitSubstitutionMethod);
        Assert.Equal(StringFormatFlags.NoClip, format.FormatFlags);
        Assert.Equal(HotkeyPrefix.None, format.HotkeyPrefix);
        Assert.Equal(StringAlignment.Near, format.LineAlignment);
        Assert.Equal(StringTrimming.Character, format.Trimming);

        // The new format is a clone.
        original.FormatFlags = StringFormatFlags.NoFontFallback;
        Assert.Equal(StringFormatFlags.NoClip, format.FormatFlags);
    }

    [Fact]
    public void Clone_Disposed_ThrowsArgumentException()
    {
        StringFormat format = new();
        format.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, format.Clone);
    }

    [Theory]
    [InlineData(0, StringDigitSubstitute.None, 0)]
    [InlineData(EnglishLanguageCode, StringDigitSubstitute.Traditional, EnglishLanguageCode)]
    [InlineData(int.MaxValue, StringDigitSubstitute.Traditional + 1, 65535)]
    [InlineData(-1, StringDigitSubstitute.User - 1, 65535)]
    public void SetDigitSubstitution_Invoke_SetsProperties(int language, StringDigitSubstitute substitute, int expectedLanguage)
    {
        using StringFormat format = new();
        format.SetDigitSubstitution(language, substitute);
        Assert.Equal(expectedLanguage, format.DigitSubstitutionLanguage);
        Assert.Equal(substitute, format.DigitSubstitutionMethod);
    }

    [Fact]
    public void SetDigitSubstitution_Disposed_ThrowsArgumentException()
    {
        StringFormat format = new();
        format.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => format.SetDigitSubstitution(0, StringDigitSubstitute.None));
    }

    [Theory]
    [InlineData(0, new float[0])]
    [InlineData(10, new float[] { 1, 2.3f, 4, float.PositiveInfinity, float.NaN })]
    public void SetTabStops_GetTabStops_ReturnsExpected(float firstTabOffset, float[] tabStops)
    {
        using StringFormat format = new();
        format.SetTabStops(firstTabOffset, tabStops);

        Assert.Equal(tabStops, format.GetTabStops(out float actualFirstTabOffset));
        Assert.Equal(firstTabOffset, actualFirstTabOffset);
    }

    [Fact]
    public void SetTabStops_NullTabStops_ThrowsArgumentNullException()
    {
        using StringFormat format = new();
        Assert.Throws<ArgumentNullException>(() => format.SetTabStops(0, null));
    }

    [Fact]
    public void SetTabStops_NegativeFirstTabOffset_ThrowsArgumentException()
    {
        using StringFormat format = new();
        AssertExtensions.Throws<ArgumentException>(null, () => format.SetTabStops(-1, []));
    }

    [Fact]
    public void SetTabStops_NegativeInfinityInTabStops_ThrowsNotImplementedException()
    {
        using StringFormat format = new();
        Assert.Throws<NotImplementedException>(() => format.SetTabStops(0, [float.NegativeInfinity]));
    }

    [Fact]
    public void SetTabStops_Disposed_ThrowsArgumentException()
    {
        StringFormat format = new();
        format.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => format.SetTabStops(0, []));
    }

    [Fact]
    public void GetTabStops_Disposed_ThrowsArgumentException()
    {
        StringFormat format = new();
        format.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => format.GetTabStops(out float firstTabOffset));
    }

    public static IEnumerable<object[]> SetMeasurableCharacterRanges_TestData()
    {
        yield return new object[] { Array.Empty<CharacterRange>() };
        yield return new object[] { new CharacterRange[] { new(1, 2) } };
        yield return new object[] { new CharacterRange[] { new(-1, -1) } };
        yield return new object[] { new CharacterRange[32] };
    }

    [Theory]
    [MemberData(nameof(SetMeasurableCharacterRanges_TestData))]
    public void SetMeasurableCharacterRanges_Valid_Success(CharacterRange[] ranges)
    {
        using StringFormat format = new();
        format.SetMeasurableCharacterRanges(ranges);
    }

    [Fact]
    public void SetMeasurableCharacterRanges_NullRanges_ThrowsArgumentNullException()
    {
        using StringFormat format = new();
        Assert.Throws<ArgumentNullException>(() => format.SetMeasurableCharacterRanges(null));
    }

    [Fact]
    public void SetMeasurableCharacterRanges_RangesTooLarge_ThrowsOverflowException()
    {
        using StringFormat format = new();
        Assert.Throws<OverflowException>(() => format.SetMeasurableCharacterRanges(new CharacterRange[33]));
    }

    [Fact]
    public void SetMeasurableCharacterRanges_Disposed_ThrowsArgumentException()
    {
        StringFormat format = new();
        format.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => format.SetMeasurableCharacterRanges([]));
    }

    [Theory]
    [InlineData(StringAlignment.Center)]
    [InlineData(StringAlignment.Far)]
    [InlineData(StringAlignment.Near)]
    public void Alignment_SetValid_GetReturnsExpected(StringAlignment alignment)
    {
        using StringFormat format = new() { Alignment = alignment };
        Assert.Equal(alignment, format.Alignment);
    }

    [Theory]
    [InlineData(StringAlignment.Near - 1)]
    [InlineData(StringAlignment.Far + 1)]
    public void Alignment_SetInvalid_ThrowsInvalidEnumArgumentException(StringAlignment alignment)
    {
        using StringFormat format = new();
        Assert.ThrowsAny<ArgumentException>(() => format.Alignment = alignment);
    }

    [Fact]
    public void Alignment_GetSetWhenDisposed_ThrowsArgumentException()
    {
        StringFormat format = new();
        format.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => format.Alignment);
        AssertExtensions.Throws<ArgumentException>(null, () => format.Alignment = StringAlignment.Center);
    }

    [Fact]
    public void DigitSubstitutionMethod_GetSetWhenDisposed_ThrowsArgumentException()
    {
        StringFormat format = new();
        format.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => format.DigitSubstitutionMethod);
    }

    [Fact]
    public void DigitSubstitutionLanguage_GetSetWhenDisposed_ThrowsArgumentException()
    {
        StringFormat format = new();
        format.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => format.DigitSubstitutionLanguage);
    }

    [Theory]
    [InlineData(StringFormatFlags.DirectionRightToLeft)]
    [InlineData((StringFormatFlags)int.MinValue)]
    [InlineData((StringFormatFlags)int.MaxValue)]
    public void FormatFlags_Set_GetReturnsExpected(StringFormatFlags formatFlags)
    {
        using StringFormat format = new() { FormatFlags = formatFlags };
        Assert.Equal(formatFlags, format.FormatFlags);
    }

    [Fact]
    public void FormatFlags_GetSetWhenDisposed_ThrowsArgumentException()
    {
        StringFormat format = new();
        format.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => format.FormatFlags);
        AssertExtensions.Throws<ArgumentException>(null, () => format.FormatFlags = StringFormatFlags.NoClip);
    }

    [Theory]
    [InlineData(StringAlignment.Center)]
    [InlineData(StringAlignment.Far)]
    [InlineData(StringAlignment.Near)]
    public void LineAlignment_SetValid_GetReturnsExpected(StringAlignment alignment)
    {
        using StringFormat format = new() { LineAlignment = alignment };
        Assert.Equal(alignment, format.LineAlignment);
    }

    [Theory]
    [InlineData(StringAlignment.Near - 1)]
    [InlineData(StringAlignment.Far + 1)]
    public void LineAlignment_SetInvalid_ThrowsInvalidEnumArgumentException(StringAlignment alignment)
    {
        using StringFormat format = new();
        Assert.ThrowsAny<ArgumentException>(() => format.LineAlignment = alignment);
    }

    [Fact]
    public void LineAlignment_GetSetWhenDisposed_ThrowsArgumentException()
    {
        StringFormat format = new();
        format.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => format.LineAlignment);
        AssertExtensions.Throws<ArgumentException>(null, () => format.LineAlignment = StringAlignment.Center);
    }

    [Theory]
    [InlineData(HotkeyPrefix.Hide)]
    [InlineData(HotkeyPrefix.None)]
    [InlineData(HotkeyPrefix.Show)]
    public void HotKeyPrefix_SetValid_GetReturnsExpected(HotkeyPrefix prefix)
    {
        using StringFormat format = new() { HotkeyPrefix = prefix };
        Assert.Equal(prefix, format.HotkeyPrefix);
    }

    [Theory]
    [InlineData(HotkeyPrefix.None - 1)]
    [InlineData(HotkeyPrefix.Hide + 1)]
    public void HotKeyPrefix_SetInvalid_ThrowsInvalidEnumArgumentException(HotkeyPrefix prefix)
    {
        using StringFormat format = new();
        Assert.ThrowsAny<ArgumentException>(() => format.HotkeyPrefix = prefix);
    }

    [Fact]
    public void HotkeyPrefix_GetSetWhenDisposed_ThrowsArgumentException()
    {
        StringFormat format = new();
        format.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => format.HotkeyPrefix);
        AssertExtensions.Throws<ArgumentException>(null, () => format.HotkeyPrefix = HotkeyPrefix.Hide);
    }

    [Theory]
    [InlineData(StringTrimming.Word)]
    public void Trimming_SetValid_GetReturnsExpected(StringTrimming trimming)
    {
        using StringFormat format = new() { Trimming = trimming };
        Assert.Equal(trimming, format.Trimming);
    }

    [Theory]
    [InlineData(StringTrimming.None - 1)]
    [InlineData(StringTrimming.EllipsisPath + 1)]
    public void Trimming_SetInvalid_ThrowsInvalidEnumArgumentException(StringTrimming trimming)
    {
        using StringFormat format = new();
        Assert.ThrowsAny<ArgumentException>(() => format.Trimming = trimming);
    }

    [Fact]
    public void Trimming_GetSetWhenDisposed_ThrowsArgumentException()
    {
        StringFormat format = new();
        format.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => format.Trimming);
        AssertExtensions.Throws<ArgumentException>(null, () => format.Trimming = StringTrimming.Word);
    }

    [Fact]
    public void GenericDefault_Get_ReturnsExpected()
    {
        StringFormat format = StringFormat.GenericDefault;
        Assert.NotSame(format, StringFormat.GenericDefault);

        Assert.Equal(StringAlignment.Near, format.Alignment);
        Assert.Equal(0, format.DigitSubstitutionLanguage);
        Assert.Equal(StringDigitSubstitute.User, format.DigitSubstitutionMethod);
        Assert.Equal((StringFormatFlags)0, format.FormatFlags);
        Assert.Equal(HotkeyPrefix.None, format.HotkeyPrefix);
        Assert.Equal(StringAlignment.Near, format.LineAlignment);
        Assert.Equal(StringTrimming.Character, format.Trimming);
    }

    [Fact]
    public void GenericTypographic_Get_ReturnsExpected()
    {
        StringFormat format = StringFormat.GenericTypographic;
        Assert.NotSame(format, StringFormat.GenericTypographic);

        Assert.Equal(StringAlignment.Near, format.Alignment);
        Assert.Equal(0, format.DigitSubstitutionLanguage);
        Assert.Equal(StringDigitSubstitute.User, format.DigitSubstitutionMethod);
        Assert.Equal(StringFormatFlags.FitBlackBox | StringFormatFlags.LineLimit | StringFormatFlags.NoClip, format.FormatFlags);
        Assert.Equal(HotkeyPrefix.None, format.HotkeyPrefix);
        Assert.Equal(StringAlignment.Near, format.LineAlignment);
        Assert.Equal(StringTrimming.None, format.Trimming);
    }

    [Fact]
    public void ToString_Flags_ReturnsExpected()
    {
        using StringFormat format = new(StringFormatFlags.DirectionVertical);
        Assert.Equal("[StringFormat, FormatFlags=DirectionVertical]", format.ToString());
    }

    [Fact]
    public void ToString_Disposed_ThrowsArgumentException()
    {
        StringFormat format = new(StringFormatFlags.DirectionVertical);
        format.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, format.ToString);
    }
}

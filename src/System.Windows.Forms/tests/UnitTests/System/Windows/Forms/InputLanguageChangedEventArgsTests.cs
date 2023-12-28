// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class InputLanguageChangedEventArgsTests
{
    public static IEnumerable<object[]> Ctor_CultureInfo_Byte_TestData()
    {
        yield return new object[] { new CultureInfo("en-US"), 0 };
        yield return new object[] { new CultureInfo("en-US"), 1 };
    }

    [Theory]
    [MemberData(nameof(Ctor_CultureInfo_Byte_TestData))]
    public void Ctor_CultureInfo_Byte(CultureInfo culture, byte charSet)
    {
        InputLanguageChangedEventArgs e = new(culture, charSet);
        Assert.Equal(InputLanguage.FromCulture(culture), e.InputLanguage);
        Assert.Equal(culture, e.Culture);
        Assert.Equal(charSet, e.CharSet);
    }

    [Fact]
    public void Ctor_NullCultureInfo_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("culture", () => new InputLanguageChangedEventArgs((CultureInfo)null, 0));
    }

    public static IEnumerable<object[]> Ctor_NoSuchCultureInfo_TestData()
    {
        yield return new object[] { CultureInfo.InvariantCulture };
        yield return new object[] { new CultureInfo("en") };
    }

    [Theory]
    [MemberData(nameof(Ctor_NoSuchCultureInfo_TestData))]
    public void Ctor_NoSuchCultureInfo_ThrowsArgumentException(CultureInfo culture)
    {
        Assert.Throws<ArgumentException>("culture", () => new InputLanguageChangedEventArgs(culture, 0));
    }

    public static IEnumerable<object[]> Ctor_InputLanguage_Byte_TestData()
    {
        yield return new object[] { InputLanguage.FromCulture(CultureInfo.InvariantCulture), 0 };
        yield return new object[] { InputLanguage.FromCulture(new CultureInfo("en")), 1 };
    }

    [Theory]
    [MemberData(nameof(Ctor_InputLanguage_Byte_TestData))]
    public void Ctor_InputLanguage_Byte(InputLanguage inputLanguage, byte charSet)
    {
        if (inputLanguage is null)
        {
            // Couldn't get the language.
            return;
        }

        InputLanguageChangedEventArgs e = new(inputLanguage, charSet);
        Assert.Equal(inputLanguage, e.InputLanguage);
        Assert.Equal(inputLanguage.Culture, e.Culture);
        Assert.Equal(charSet, e.CharSet);
    }

    [Fact]
    public void Ctor_NullInputLanguage_ThrowsNullReferenceException()
    {
        Assert.Throws<ArgumentNullException>("inputLanguage", () => new InputLanguageChangedEventArgs((InputLanguage)null, 0));
    }
}

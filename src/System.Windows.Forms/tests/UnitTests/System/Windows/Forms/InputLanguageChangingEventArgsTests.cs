﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class InputLanguageChangingEventArgsTests
{
    public static IEnumerable<object[]> Ctor_CultureInfo_Bool_TestData()
    {
        yield return new object[] { new CultureInfo("en-US"), true };
        yield return new object[] { new CultureInfo("en-US"), false };
    }

    [Theory]
    [MemberData(nameof(Ctor_CultureInfo_Bool_TestData))]
    public void Ctor_CultureInfo_Bool(CultureInfo culture, bool sysCharSet)
    {
        var e = new InputLanguageChangingEventArgs(culture, sysCharSet);
        Assert.Equal(InputLanguage.FromCulture(culture), e.InputLanguage);
        Assert.Equal(culture, e.Culture);
        Assert.Equal(sysCharSet, e.SysCharSet);
        Assert.False(e.Cancel);
    }

    [Fact]
    public void Ctor_NullCultureInfo_ThrowsNullReferenceException()
    {
        Assert.Throws<ArgumentNullException>("culture", () => new InputLanguageChangingEventArgs((CultureInfo)null, true));
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
        Assert.Throws<ArgumentException>("culture", () => new InputLanguageChangingEventArgs(culture, true));
    }

    public static IEnumerable<object[]> Ctor_InputLanguage_Bool_TestData()
    {
        yield return new object[] { InputLanguage.FromCulture(CultureInfo.InvariantCulture), true };
        yield return new object[] { InputLanguage.FromCulture(new CultureInfo("en")), false };
    }

    [Theory]
    [MemberData(nameof(Ctor_InputLanguage_Bool_TestData))]
    public void Ctor_InputLanguage_Bool(InputLanguage inputLanguage, bool sysCharSet)
    {
        if (inputLanguage is null)
        {
            // Couldn't get the language.
            return;
        }

        var e = new InputLanguageChangingEventArgs(inputLanguage, sysCharSet);
        Assert.Equal(inputLanguage, e.InputLanguage);
        Assert.Equal(inputLanguage.Culture, e.Culture);
        Assert.Equal(sysCharSet, e.SysCharSet);
    }

    [Fact]
    public void Ctor_NullInputLanguage_ThrowsNullReferenceException()
    {
        Assert.Throws<ArgumentNullException>("inputLanguage", () => new InputLanguageChangingEventArgs((InputLanguage)null, true));
    }
}

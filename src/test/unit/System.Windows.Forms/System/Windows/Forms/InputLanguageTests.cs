// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Globalization;
using System.Reflection;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
[UseDefaultXunitCulture(SetUnmanagedUiThreadCulture = true)]
public class InputLanguageTests
{
    [Fact]
    public void InputLanguage_InstalledInputLanguages_Get_ReturnsExpected()
    {
        InputLanguageCollection collection = InputLanguage.InstalledInputLanguages;
        Assert.NotSame(collection, InputLanguage.InstalledInputLanguages);
        Assert.NotEmpty(collection);
        Assert.All(collection.Cast<InputLanguage>(), VerifyInputLanguage);
    }

    [Fact]
    public void InputLanguage_DefaultInputLanguage_Get_ReturnsExpected()
    {
        InputLanguage language = InputLanguage.DefaultInputLanguage;
        Assert.NotSame(language, InputLanguage.DefaultInputLanguage);
        VerifyInputLanguage(language);
    }

    [Fact]
    public void InputLanguage_CurrentInputLanguage_Get_ReturnsExpected()
    {
        InputLanguage language = InputLanguage.CurrentInputLanguage;
        Assert.NotSame(language, InputLanguage.CurrentInputLanguage);
        VerifyInputLanguage(language);
    }

    [Fact]
    public void InputLanguage_CurrentInputLanguage_Set_GetReturnsExpected()
    {
        InputLanguage original = InputLanguage.CurrentInputLanguage;
        try
        {
            // Set null.
            InputLanguage.CurrentInputLanguage = null;
            Assert.Equal(InputLanguage.DefaultInputLanguage, InputLanguage.CurrentInputLanguage);

            foreach (InputLanguage language in InputLanguage.InstalledInputLanguages)
            {
                // Set other.
                InputLanguage.CurrentInputLanguage = language;
                Assert.Equal(language, InputLanguage.CurrentInputLanguage);

                // Set same.
                InputLanguage.CurrentInputLanguage = language;
                Assert.Equal(language, InputLanguage.CurrentInputLanguage);
            }
        }
        finally
        {
            InputLanguage.CurrentInputLanguage = original;
        }
    }

    [Fact]
    public void InputLanguage_CurrentInputLanguage_SetInvalidValue_ThrowsArgumentException()
    {
        InputLanguage language = Assert.IsType<InputLanguage>(Activator.CreateInstance(typeof(InputLanguage), BindingFlags.Instance | BindingFlags.NonPublic, null, [(IntPtr)250], null));
        InputLanguage original = InputLanguage.CurrentInputLanguage;
        try
        {
            Assert.Throws<ArgumentException>("value", () => InputLanguage.CurrentInputLanguage = language);
        }
        finally
        {
            InputLanguage.CurrentInputLanguage = original;
        }
    }

    public static IEnumerable<object[]> Equals_TestData()
    {
        yield return new object[] { InputLanguage.DefaultInputLanguage, InputLanguage.DefaultInputLanguage, true };
        yield return new object[] { InputLanguage.DefaultInputLanguage, new(), false };
        yield return new object[] { InputLanguage.DefaultInputLanguage, null, false };
    }

    [Theory]
    [MemberData(nameof(Equals_TestData))]
    public void InputLanguage_Equals_Invoke_ReturnsExpected(InputLanguage language, object value, bool expected)
    {
        Assert.Equal(expected, language.Equals(value));
    }

    [Fact]
    public void InputLanguage_FromCulture_Roundtrip_Success()
    {
        InputLanguageCollection installed = InputLanguage.InstalledInputLanguages;
        foreach (InputLanguage language in installed)
        {
            InputLanguage result = InputLanguage.FromCulture(language.Culture);
            Assert.NotNull(result);
            Assert.NotSame(language, result);
            Assert.Equal(language.Culture, result.Culture);
            Assert.Equal(installed.Cast<InputLanguage>().First(item => item.Culture.Equals(language.Culture)), result);
            VerifyInputLanguage(result);
        }
    }

    [Fact]
    public void InputLanguage_FromCulture_NoSuchCulture_ReturnsNull()
    {
        var invariantCulture = CultureInfo.InvariantCulture;
        Assert.Null(InputLanguage.FromCulture(invariantCulture));
    }

    [Fact]
    public void InputLanguage_FromCulture_NullCulture_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("culture", () => InputLanguage.FromCulture(null));
    }

    [Fact]
    public void InputLanguage_GetHashCode_Invoke_RemainsSameAcrossCalls()
    {
        InputLanguage language = InputLanguage.CurrentInputLanguage;
        Assert.Equal(language.GetHashCode(), language.GetHashCode());
    }

    public static IEnumerable<object[]> InputLanguageLayoutId_TestData()
    {
        yield return new object[] { 0x0409, 0x0000, "en-US", "00000409", "US" };
        yield return new object[] { 0x0409, 0x0409, "en-US", "00000409", "US" };
        yield return new object[] { 0x0409, 0x040c, "en-US", "0000040C", "French" };
        yield return new object[] { 0x0409, 0xf020, "en-US", "00011009", "Canadian Multilingual Standard" };
        yield return new object[] { 0x0c0c, 0x1009, "fr-CA", "00001009", "Canadian French" };
        yield return new object[] { 0x0c0c, 0xf020, "fr-CA", "00011009", "Canadian Multilingual Standard" };
    }

    [Theory]
    [MemberData(nameof(InputLanguageLayoutId_TestData))]
    public void InputLanguage_InputLanguageLayoutId_Expected(int langId, int device, string languageTag, string layoutId, string layoutName)
    {
        InputLanguage language = new(PARAM.FromLowHigh(langId, device));
        VerifyInputLanguage(language, languageTag, layoutId, layoutName);
    }

    public static IEnumerable<object[]> SupplementalInputLanguages_TestData()
    {
        foreach (string languageTag in new[] { "got-Goth", "jv-Java", "zgh-Tfng", "nqo" })
        {
            yield return new object[] { languageTag, (int)PInvoke.LOCALE_TRANSIENT_KEYBOARD1 };
            yield return new object[] { languageTag, (int)PInvoke.LOCALE_TRANSIENT_KEYBOARD2 };
            yield return new object[] { languageTag, (int)PInvoke.LOCALE_TRANSIENT_KEYBOARD3 };
            yield return new object[] { languageTag, (int)PInvoke.LOCALE_TRANSIENT_KEYBOARD4 };
        }
    }

    [Theory]
    [MemberData(nameof(SupplementalInputLanguages_TestData))]
    public void InputLanguage_GetLanguageTag_SupplementalInputLanguages_Expected(string languageTag, int langId)
    {
        KeyValuePair<string, int>[] languages = [new("en-US", 0x0409), new(languageTag, langId), new("fr-FR", 0x040c)];
        string actual = InputLanguage.GetLanguageTag(langId, () => languages);
        Assert.Equal(languageTag, actual);
    }

    [Theory]
    [InlineData(0x0409, "en-US")]
    [InlineData(0x0415, "pl-PL")]
    public void InputLanguage_GetLanguageTag_StandardLanguage_DoesNotReadUserProfile(int langId, string expected)
    {
        Assert.Equal(expected, InputLanguage.GetLanguageTag(langId, () => throw new InvalidOperationException()));
    }

    [Theory]
    [InlineData((int)PInvoke.LOCALE_TRANSIENT_KEYBOARD1)]
    [InlineData((int)PInvoke.LOCALE_TRANSIENT_KEYBOARD2)]
    [InlineData((int)PInvoke.LOCALE_TRANSIENT_KEYBOARD3)]
    [InlineData((int)PInvoke.LOCALE_TRANSIENT_KEYBOARD4)]
    public void InputLanguage_GetLanguageTag_DuplicateMatches_ReturnsFirst(int langId)
    {
        KeyValuePair<string, int>[] languages = [new("got-Goth", langId), new("jv-Java", langId)];
        Assert.Equal("got-Goth", InputLanguage.GetLanguageTag(langId, () => languages));
    }

    [Theory]
    [InlineData((int)PInvoke.LOCALE_TRANSIENT_KEYBOARD1)]
    [InlineData((int)PInvoke.LOCALE_TRANSIENT_KEYBOARD2)]
    [InlineData((int)PInvoke.LOCALE_TRANSIENT_KEYBOARD3)]
    [InlineData((int)PInvoke.LOCALE_TRANSIENT_KEYBOARD4)]
    public void InputLanguage_GetLanguageTag_NoMatch_UsesCultureInfoFallback(int langId)
    {
        // A transient ID may not be known to CultureInfo on this machine. Preserve that behavior too.
        string expected = null;
        Exception expectedException = Record.Exception(() => expected = CultureInfo.GetCultureInfo(langId).Name);
        KeyValuePair<string, int>[][] languageLists = [[], [new("en-US", 0x0409)]];
        foreach (KeyValuePair<string, int>[] languages in languageLists)
        {
            bool readLanguages = false;
            string actual = null;
            Exception actualException = Record.Exception(() => actual = InputLanguage.GetLanguageTag(langId, () =>
            {
                readLanguages = true;
                return languages;
            }));

            Assert.True(readLanguages);
            Assert.Equal(expectedException?.GetType(), actualException?.GetType());
            Assert.Equal(expected, actual);
        }
    }

    [Theory]
    [InlineData(0x0000, 0x0409)]
    [InlineData(0xffff, 0x0409)]
    public void InputLanguage_Culture_ThrowsArgumentException(int langId, int device)
    {
        InputLanguage language = new(PARAM.FromLowHigh(langId, device));
        Assert.ThrowsAny<ArgumentException>(() => language.Culture);
    }

    [Theory]
    [InlineData(0x0409, 0xf000)]
    [InlineData(0x0409, 0xffff)]
    public void InputLanguage_LayoutName_UnknownExpected(int langId, int device)
    {
        InputLanguage language = new(PARAM.FromLowHigh(langId, device));
        Assert.Equal(SR.UnknownInputLanguageLayout, language.LayoutName);
    }

    private static void VerifyInputLanguage(InputLanguage language, string languageTag, string layoutId, string layoutName)
    {
        language.Should().NotBeNull();
        language.Handle.Should().NotBe(IntPtr.Zero);
        language.Culture.Name.Should().Be(languageTag);
        language.LayoutId.Should().Be(layoutId);

        if (CultureInfo.InstalledUICulture.Name.StartsWith("en-", StringComparison.OrdinalIgnoreCase))
        {
            // Layout display names may include OS-specific suffixes e.g. "French" became
            // "French (Legacy, AZERTY)" in Windows 11.0. Due to this reason we are using StartsWith
            // instead of equality check for layout name.
            language.LayoutName.Should().StartWith(layoutName);
        }
        else
        {
            language.LayoutName.Should().NotBeNullOrEmpty();
            language.LayoutName.Should().NotBeEquivalentTo(SR.UnknownInputLanguageLayout);
        }
    }

    private static void VerifyInputLanguage(InputLanguage language)
    {
        Assert.NotEqual(IntPtr.Zero, language.Handle);
        Assert.NotNull(language.Culture);
        Assert.NotNull(language.LayoutName);
        Assert.NotEmpty(language.LayoutName);
        Assert.NotEqual(SR.UnknownInputLanguageLayout, language.LayoutName);
        Assert.DoesNotContain('\0', language.LayoutName);
    }
}

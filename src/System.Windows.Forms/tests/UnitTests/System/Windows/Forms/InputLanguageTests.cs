// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Reflection;
using static Interop;

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
        InputLanguage language = InputLanguage.CurrentInputLanguage;
        try
        {
            // Set null.
            InputLanguage.CurrentInputLanguage = null;
            Assert.Equal(InputLanguage.DefaultInputLanguage, InputLanguage.CurrentInputLanguage);

            // Set other.
            InputLanguage.CurrentInputLanguage = language;
            Assert.Equal(language, InputLanguage.CurrentInputLanguage);

            // Set same.
            InputLanguage.CurrentInputLanguage = language;
            Assert.Equal(language, InputLanguage.CurrentInputLanguage);
        }
        catch
        {
            InputLanguage.CurrentInputLanguage = language;
        }
    }

    [Fact]
    public void InputLanguage_CurrentInputLanguage_SetInvalidValue_ThrowsArgumentException()
    {
        InputLanguage language = Assert.IsType<InputLanguage>(Activator.CreateInstance(typeof(InputLanguage), BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { (IntPtr)250 }, null));
        Assert.Throws<ArgumentException>("value", () => InputLanguage.CurrentInputLanguage = language);
    }

    public static IEnumerable<object[]> Equals_TestData()
    {
        yield return new object[] { InputLanguage.DefaultInputLanguage, InputLanguage.DefaultInputLanguage, true };
        yield return new object[] { InputLanguage.DefaultInputLanguage, new object(), false };
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
        InputLanguage language = InputLanguage.CurrentInputLanguage;
        InputLanguage result = InputLanguage.FromCulture(language.Culture);
        Assert.NotSame(language, result);
        Assert.Equal(language, result);
        VerifyInputLanguage(result);
    }

    [Fact]
    public void InputLanguage_FromCulture_NoSuchCulture_ReturnsNull()
    {
        var unknownCulture = new UnknownKeyboardCultureInfo();
        Assert.Null(InputLanguage.FromCulture(unknownCulture));
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
        var language = new InputLanguage(PARAM.FromLowHigh(langId, device));
        Assert.Equal(languageTag, language.Culture.Name);
        Assert.Equal(layoutId, language.LayoutId);
        Assert.Equal(layoutName, language.LayoutName);
        VerifyInputLanguage(language);
    }

    [Theory]
    [InlineData(0x0000, 0x0409)]
    [InlineData(0xffff, 0x0409)]
    public void InputLanguage_Culture_ThrowsArgumentException(int langId, int device)
    {
        var language = new InputLanguage(PARAM.FromLowHigh(langId, device));
        Assert.ThrowsAny<ArgumentException>(() => language.Culture);
    }

    [Theory]
    [InlineData(0x0409, 0xf000)]
    [InlineData(0x0409, 0xffff)]
    public void InputLanguage_LayoutName_UnknownExpected(int langId, int device)
    {
        var language = new InputLanguage(PARAM.FromLowHigh(langId, device));
        Assert.Equal(SR.UnknownInputLanguageLayout, language.LayoutName);
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

    private class UnknownKeyboardCultureInfo : CultureInfo
    {
        public UnknownKeyboardCultureInfo() : base("en-US")
        {
        }

        public override int KeyboardLayoutId => int.MaxValue;
    }
}

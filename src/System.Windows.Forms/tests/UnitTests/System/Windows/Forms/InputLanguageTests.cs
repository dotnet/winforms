// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
        InputLanguage language = Assert.IsType<InputLanguage>(Activator.CreateInstance(typeof(InputLanguage), BindingFlags.Instance | BindingFlags.NonPublic, null, [(IntPtr)250], null));
        Assert.Throws<ArgumentException>("value", () => InputLanguage.CurrentInputLanguage = language);
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
        InputLanguage language = InputLanguage.CurrentInputLanguage;
        InputLanguage result = InputLanguage.FromCulture(language.Culture);
        Assert.NotSame(language, result);
        Assert.Equal(language, result);
        VerifyInputLanguage(result);
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
        yield return new object[] { "got-Goth", "000C0C00", "Gothic" };
        yield return new object[] { "jv-Java", "00110C00", "Javanese" };
        yield return new object[] { "nqo", "00090C00", "N’Ko" };
        yield return new object[] { "zgh-Tfng", "0000105F", "Tifinagh (Basic)" };
    }

    [Theory]
    [MemberData(nameof(SupplementalInputLanguages_TestData))]
    public void InputLanguage_FromCulture_SupplementalInputLanguages_Expected(string languageTag, string layoutId, string layoutName)
    {
        // This condition should be removed once https://github.com/dotnet/winforms/issues/10150 is resolved.
        if (languageTag == "nqo" && !OsVersion.IsWindows11_22H2OrGreater())
        {
            return;
        }

        // Also installs default keyboard layout for this language
        // https://learn.microsoft.com/windows-hardware/manufacture/desktop/default-input-locales-for-windows-language-packs
        InstallUserLanguage(languageTag);

        try
        {
            CultureInfo culture = new(languageTag);
            InputLanguage language = InputLanguage.FromCulture(culture);
            VerifyInputLanguage(language, languageTag, layoutId, layoutName);
        }
        finally
        {
            UninstallUserLanguage(languageTag);
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
        Assert.NotNull(language);
        Assert.NotEqual(IntPtr.Zero, language.Handle);
        Assert.Equal(languageTag, language.Culture.Name);
        Assert.Equal(layoutId, language.LayoutId);
        Assert.Equal(layoutName, language.LayoutName);
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

    private static void RunPowerShellScript(string path)
    {
        using Process process = new();

        process.StartInfo.FileName = "powershell.exe";
        process.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy ByPass -File \"{path}\"";

        process.Start();
        process.WaitForExit();
    }

    private static void InstallUserLanguage(string languageTag)
    {
        string file = Path.Combine(Path.GetTempPath(), $"install-language-{languageTag}.ps1");
        string script = $$"""
            $list = Get-WinUserLanguageList
            $list.Add("{{languageTag}}")
            Set-WinUserLanguageList $list -force
            """;

        using TempFile tempFile = new(file, script);
        RunPowerShellScript(tempFile.Path);
    }

    private static void UninstallUserLanguage(string languageTag)
    {
        string file = Path.Combine(Path.GetTempPath(), $"uninstall-language-{languageTag}.ps1");
        string script = $$"""
            $list = Get-WinUserLanguageList
            $item = $list | Where-Object {$_.LanguageTag -like "{{languageTag}}"}
            $list.Remove($item)
            Set-WinUserLanguageList $list -force
            """;

        using TempFile tempFile = new(file, script);
        RunPowerShellScript(tempFile.Path);
    }
}

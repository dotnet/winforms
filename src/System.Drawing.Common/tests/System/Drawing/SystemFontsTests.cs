// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Drawing.Tests;

public class SystemFontsTests
{
    public static IEnumerable<object[]> SystemFonts_TestData()
    {
        yield return new object[] { () => SystemFonts.CaptionFont };
        yield return new object[] { () => SystemFonts.IconTitleFont };
        yield return new object[] { () => SystemFonts.MenuFont };
        yield return new object[] { () => SystemFonts.MessageBoxFont };
        yield return new object[] { () => SystemFonts.SmallCaptionFont };
        yield return new object[] { () => SystemFonts.StatusFont };
    }

    [Theory]
    [MemberData(nameof(SystemFonts_TestData))]
    public void SystemFont_Get_ReturnsExpected(Func<Font> getFont)
    {
        using Font font = getFont();
        using Font otherFont = getFont();
        Assert.NotNull(font);
        Assert.NotNull(otherFont);
        Assert.NotSame(font, otherFont);

        // Assert.Equal on a font will use the native handle to assert equality, which is not always guaranteed.
        Assert.Equal(font.Name, otherFont.Name);
    }

    public static IEnumerable<object[]> SystemFonts_WindowsNames_TestData()
    {
        int userLangId = GetUserDefaultLCID();
        SystemFontList fonts = (userLangId & 0x3ff) switch
        {
            // ja-JP (Japanese)
            0x11 => new SystemFontList("Yu Gothic UI"),
            // chr-Cher-US (Cherokee)
            0x5C => new SystemFontList("Gadugi"),
            // ko-KR (Korean)
            0x12 => new SystemFontList("\ub9d1\uc740\x20\uace0\ub515"),
            // zh-TW (Traditional Chinese, Taiwan) or zh-CN (Simplified Chinese, PRC)
            0x4 => (userLangId & 0xFFFF) switch
            {
                // Although the primary language ID is the same, the fonts are different
                // So we have to determine by the full language ID
                // https://docs.microsoft.com/openspecs/windows_protocols/ms-lcid/70feba9f-294e-491e-b6eb-56532684c37f
                // Assuming this doc is correct AND the font only differs by whether it's traditional or not it should work

                // zh-Hans
                0x0004 or 0x7804 or 0x0804 or 0x1004 => new SystemFontList("Microsoft JhengHei UI"),
                // zh-Hant
                0x7C04 or 0x0C04 or 0x1404 or 0x0404 => new SystemFontList("Microsoft YaHei UI"),
                _ => throw new InvalidOperationException(
                    "The primary language ID is Chinese, however it was not able to" +
                    $" determine the user locale from the LCID with value: {userLangId & 0xFFFF:X4}."),
            },
            // th-TH
            0x1E or 0x54 or 0x53 => new SystemFontList("Leelawadee UI"),
            // te-IN
            0x4A or 0x49 or 0x5B or 0x48 or 0x4E or 0x4C or 0x57 or 0x45 or 0x4D => new SystemFontList("Nirmala UI"),
            // am-ET
            0x5E => new SystemFontList("Ebrima"),
            // For now we assume everything else uses Segoe UI
            _ => new SystemFontList("Segoe UI"), // If there's other failure reported we can add it
        };
        return fonts.ToTestData();
    }

    [Theory]
    [MemberData(nameof(SystemFonts_WindowsNames_TestData))]
    public void SystemFont_Get_ReturnsExpected_WindowsNames(Func<Font> getFont, string systemFontName, string windowsFontName)
    {
        using Font font = getFont();
        using Font otherFont = getFont();
        using Font fontFromName = SystemFonts.GetFontByName(systemFontName);
        Assert.NotSame(font, otherFont);
        Assert.Equal(font, otherFont);
        Assert.Equal(font, fontFromName);

        Assert.Equal(systemFontName, font.SystemFontName);

        // Windows 8 updated some system fonts.
        if (!PlatformDetection.IsWindows7)
        {
            Assert.Equal(windowsFontName, font.Name);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("captionfont")]
    public void GetFontByName_NoSuchName_ReturnsNull(string? systemFontName)
    {
        Assert.Null(SystemFonts.GetFontByName(systemFontName));
    }

    [DllImport("kernel32.dll", SetLastError = false, CharSet = CharSet.Auto)]
    internal static extern int GetUserDefaultLCID();

    // Do not test DefaultFont and DialogFont, as we can't reliably determine from LCID
    // https://github.com/dotnet/runtime/issues/28830#issuecomment-473556522
    private class SystemFontList
    {
        public SystemFontList(string c_it_m_mb_scFonts)
        {
            CaptionFont = c_it_m_mb_scFonts;
            IconTitleFont = c_it_m_mb_scFonts;
            MenuFont = c_it_m_mb_scFonts;
            MessageBoxFont = c_it_m_mb_scFonts;
            SmallCaptionFont = c_it_m_mb_scFonts;
            StatusFont = c_it_m_mb_scFonts;
        }

        public string CaptionFont { get; set; }
        public string IconTitleFont { get; set; }
        public string MenuFont { get; set; }
        public string MessageBoxFont { get; set; }
        public string SmallCaptionFont { get; set; }
        public string StatusFont { get; set; }

        public IEnumerable<object[]> ToTestData()
        {
            return new[]
            {
            new object[] { () => SystemFonts.CaptionFont, nameof(CaptionFont), CaptionFont},
            [(() => SystemFonts.IconTitleFont), nameof(IconTitleFont), IconTitleFont],
            [(() => SystemFonts.MenuFont), nameof(MenuFont), MenuFont],
            [(() => SystemFonts.MessageBoxFont), nameof(MessageBoxFont), MessageBoxFont],
            [(() => SystemFonts.SmallCaptionFont), nameof(SmallCaptionFont), SmallCaptionFont],
            [(() => SystemFonts.StatusFont), nameof(StatusFont), StatusFont]
            };
        }
    }
}

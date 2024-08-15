// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Win32;

namespace System.Windows.Forms;

internal static class DisplayInformation
{
    private static bool s_highContrast;               // whether we are under hight contrast mode
    private static bool s_lowRes;                     // whether we are under low resolution mode
    private static bool s_highContrastSettingValid;   // indicates whether the high contrast setting is correct
    private static bool s_lowResSettingValid;         // indicates whether the low resolution setting is correct
    private static short s_bitsPerPixel;
    private static bool s_dropShadowSettingValid;
    private static bool s_dropShadowEnabled;
    private static bool s_menuAccessKeysUnderlinedValid;
    private static bool s_menuAccessKeysUnderlined;

    static DisplayInformation()
    {
        SystemEvents.UserPreferenceChanging += UserPreferenceChanging;
        SystemEvents.DisplaySettingsChanging += DisplaySettingsChanging;
    }

    public static short BitsPerPixel
    {
        get
        {
            if (s_bitsPerPixel == 0 && Screen.PrimaryScreen is not null)
            {
                // we used to iterate through all screens, but
                // for some reason unused screens can temporarily appear
                // in the AllScreens collection - we would honor the display
                // setting of an unused screen.
                // According to EnumDisplayMonitors, a primary screen check should be sufficient
                s_bitsPerPixel = (short)Screen.PrimaryScreen.BitsPerPixel;
            }

            return s_bitsPerPixel;
        }
    }

    /// <summary>
    ///  Tests to see if the monitor is in low resolution mode (8-bit color depth or less).
    /// </summary>
    public static bool LowResolution
    {
        get
        {
            if (s_lowResSettingValid && !s_lowRes)
            {
                return s_lowRes;
            }

            // don't cache if we're in low resolution.
            s_lowRes = BitsPerPixel <= 8;
            s_lowResSettingValid = true;
            return s_lowRes;
        }
    }

    /// <summary>
    ///  Tests to see if we are under high contrast mode
    /// </summary>
    public static bool HighContrast
    {
        get
        {
            if (s_highContrastSettingValid)
            {
                return s_highContrast;
            }

            s_highContrast = SystemInformation.HighContrast;
            s_highContrastSettingValid = true;
            return s_highContrast;
        }
    }

    public static bool IsDropShadowEnabled
    {
        get
        {
            if (s_dropShadowSettingValid)
            {
                return s_dropShadowEnabled;
            }

            s_dropShadowEnabled = SystemInformation.IsDropShadowEnabled;
            s_dropShadowSettingValid = true;
            return s_dropShadowEnabled;
        }
    }

    // return if mnemonic underlines should always be there regardless of ALT
    public static bool MenuAccessKeysUnderlined
    {
        get
        {
            if (s_menuAccessKeysUnderlinedValid)
            {
                return s_menuAccessKeysUnderlined;
            }

            s_menuAccessKeysUnderlined = SystemInformation.MenuAccessKeysUnderlined;
            s_menuAccessKeysUnderlinedValid = true;
            return s_menuAccessKeysUnderlined;
        }
    }

    /// <summary>
    ///event handler for change in display setting
    /// </summary>
    private static void DisplaySettingsChanging(object? obj, EventArgs ea)
    {
        s_highContrastSettingValid = false;
        s_lowResSettingValid = false;
        s_dropShadowSettingValid = false;
        s_menuAccessKeysUnderlinedValid = false;
    }

    /// <summary>
    ///event handler for change in user preference
    /// </summary>
    private static void UserPreferenceChanging(object obj, UserPreferenceChangingEventArgs e)
    {
        s_highContrastSettingValid = false;
        s_lowResSettingValid = false;
        s_dropShadowSettingValid = false;
        s_bitsPerPixel = 0;

        if (e.Category == UserPreferenceCategory.General)
        {
            s_menuAccessKeysUnderlinedValid = false;
        }
    }
}

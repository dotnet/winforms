// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32;

namespace System.Windows.Forms
{
    internal class DisplayInformation
    {
        private static bool highContrast;               //whether we are under hight contrast mode
        private static bool lowRes;                     //whether we are under low resolution mode
        private static bool isTerminalServerSession;    //whether this application is run on a terminal server (remote desktop)
        private static bool highContrastSettingValid;   //indicates whether the high contrast setting is correct
        private static bool lowResSettingValid;         //indicates whether the low resolution setting is correct
        private static bool terminalSettingValid;       //indicates whether the terminal server setting is correct
        private static short bitsPerPixel;
        private static bool dropShadowSettingValid;
        private static bool dropShadowEnabled;
        private static bool menuAccessKeysUnderlinedValid;
        private static bool menuAccessKeysUnderlined;

        static DisplayInformation()
        {
            SystemEvents.UserPreferenceChanging += new UserPreferenceChangingEventHandler(UserPreferenceChanging);
            SystemEvents.DisplaySettingsChanging += new EventHandler(DisplaySettingsChanging);
        }

        public static short BitsPerPixel
        {
            get
            {
                if (bitsPerPixel == 0)
                {
                    // we used to iterate through all screens, but
                    // for some reason unused screens can temparily appear
                    // in the AllScreens collection - we would honor the display
                    // setting of an unused screen.
                    // According to EnumDisplayMonitors, a primary screen check should be sufficient
                    bitsPerPixel = (short)Screen.PrimaryScreen.BitsPerPixel;

                }
                return bitsPerPixel;
            }
        }

        ///<summary>
        ///tests to see if the monitor is in low resolution mode (8-bit color depth or less).
        ///</summary>
        public static bool LowResolution
        {
            get
            {

                if (lowResSettingValid && !lowRes)
                {
                    return lowRes;
                }
                // dont cache if we're in low resolution.
                lowRes = BitsPerPixel <= 8;
                lowResSettingValid = true;
                return lowRes;
            }
        }

        ///<summary>
        ///tests to see if we are under high contrast mode
        ///</summary>
        public static bool HighContrast
        {
            get
            {
                if (highContrastSettingValid)
                {
                    return highContrast;
                }
                highContrast = SystemInformation.HighContrast;
                highContrastSettingValid = true;
                return highContrast;
            }
        }
        public static bool IsDropShadowEnabled
        {
            get
            {
                if (dropShadowSettingValid)
                {
                    return dropShadowEnabled;
                }
                dropShadowEnabled = SystemInformation.IsDropShadowEnabled;
                dropShadowSettingValid = true;
                return dropShadowEnabled;
            }
        }

        ///<summary>
        ///test to see if we are under terminal server mode
        ///</summary>
        public static bool TerminalServer
        {
            get
            {
                if (terminalSettingValid)
                {
                    return isTerminalServerSession;
                }

                isTerminalServerSession = SystemInformation.TerminalServerSession;
                terminalSettingValid = true;
                return isTerminalServerSession;
            }
        }

        // return if mnemonic underlines should always be there regardless of ALT
        public static bool MenuAccessKeysUnderlined
        {
            get
            {
                if (menuAccessKeysUnderlinedValid)
                {
                    return menuAccessKeysUnderlined;
                }
                menuAccessKeysUnderlined = SystemInformation.MenuAccessKeysUnderlined;
                menuAccessKeysUnderlinedValid = true;
                return menuAccessKeysUnderlined;
            }
        }

        ///<summary>
        ///event handler for change in display setting
        ///</summary>
        private static void DisplaySettingsChanging(object obj, EventArgs ea)
        {
            highContrastSettingValid = false;
            lowResSettingValid = false;
            terminalSettingValid = false;
            dropShadowSettingValid = false;
            menuAccessKeysUnderlinedValid = false;

        }

        ///<summary>
        ///event handler for change in user preference
        ///</summary>
        private static void UserPreferenceChanging(object obj, UserPreferenceChangingEventArgs e)
        {
            highContrastSettingValid = false;
            lowResSettingValid = false;
            terminalSettingValid = false;
            dropShadowSettingValid = false;
            bitsPerPixel = 0;

            if (e.Category == UserPreferenceCategory.General)
            {
                menuAccessKeysUnderlinedValid = false;
            }
        }
    }
}

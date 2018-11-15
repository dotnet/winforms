// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Globalization;
using System.Security.Permissions;

    internal class LinkUtilities {

        // IE fonts and colors
        static Color ielinkColor = Color.Empty;
        static Color ieactiveLinkColor = Color.Empty;
        static Color ievisitedLinkColor = Color.Empty;

        const string IESettingsRegPath = "Software\\Microsoft\\Internet Explorer\\Settings";
        public const string IEMainRegPath = "Software\\Microsoft\\Internet Explorer\\Main";
        const string IEAnchorColor = "Anchor Color";
        const string IEAnchorColorVisited = "Anchor Color Visited";
        const string IEAnchorColorHover = "Anchor Color Hover";

        /// <include file='doc\LinkUtilities.uex' path='docs/doc[@for="LinkUtilities.GetIEColor"]/*' />
        /// <devdoc>
        ///     Retrieves a named IE color from the registry. There are constants at the top
        ///     of this file of the valid names to retrieve.
        /// </devdoc>
        private static Color GetIEColor(string name) {
            // 




            new RegistryPermission(PermissionState.Unrestricted).Assert();
            try {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(IESettingsRegPath);

                if (key != null) {

                    // Since this comes from the registry, be very careful about its contents.
                    //
                    string s = (string)key.GetValue(name);
                    key.Close();

                    if (s != null) {
                        string[] rgbs = s.Split(new char[] {','});
                        int[] rgb = new int[3];

                        int nMax = Math.Min(rgb.Length, rgbs.Length);

                        //NOTE: if we can't parse rgbs[i], rgb[i] will be set to 0.
                        for (int i = 0; i < nMax; i++)
                        {
                            int.TryParse(rgbs[i], out rgb[i]);
                        }

                        return Color.FromArgb(rgb[0], rgb[1], rgb[2]);
                    }
                }

                if (string.Equals(name, IEAnchorColor, StringComparison.OrdinalIgnoreCase)) {
                    return Color.Blue;
                }
                else if (string.Equals(name, IEAnchorColorVisited, StringComparison.OrdinalIgnoreCase)) {
                    return Color.Purple;
                }
                else if (string.Equals(name, IEAnchorColorHover, StringComparison.OrdinalIgnoreCase)) {
                    return Color.Red;
                }
                else {
                    return Color.Red;
                }
            }
            finally {
                System.Security.CodeAccessPermission.RevertAssert();
            }

        }

        public static Color IELinkColor {
            get {
                if (ielinkColor.IsEmpty) {
                    ielinkColor = GetIEColor(IEAnchorColor);
                }
                return ielinkColor;
            }
        }

        public static Color IEActiveLinkColor {
            get {
                if (ieactiveLinkColor.IsEmpty) {
                    ieactiveLinkColor = GetIEColor(IEAnchorColorHover);
                }
                return ieactiveLinkColor;
            }
        }
        public static Color IEVisitedLinkColor {
            get {
                if (ievisitedLinkColor.IsEmpty) {
                    ievisitedLinkColor = GetIEColor(IEAnchorColorVisited);
                }
                return ievisitedLinkColor;
            }
        }

        /// Produces a color for visited links using SystemColors
        public static Color GetVisitedLinkColor()
        {
            int r = (SystemColors.Window.R + SystemColors.WindowText.R + 1) / 2;
            int g = SystemColors.WindowText.G;
            int b = (SystemColors.Window.B + SystemColors.WindowText.B + 1) / 2;

            return Color.FromArgb(r, g, b);
        }

        /// <include file='doc\LinkUtilities.uex' path='docs/doc[@for="LinkUtilities.GetIELinkBehavior"]/*' />
        /// <devdoc>
        ///     Retrieves the IE settings for link behavior from the registry.
        /// </devdoc>
        public static LinkBehavior GetIELinkBehavior() {
            // 




            new RegistryPermission(PermissionState.Unrestricted).Assert();
            try {
                RegistryKey key = null;
                try {
                    key = Registry.CurrentUser.OpenSubKey(IEMainRegPath);
                }
                catch (System.Security.SecurityException) {
                    // User does not have right to access Registry path HKCU\\Software\\Microsoft\\Internet Explorer\\Main.
                    // Catch SecurityException silently and let the return value fallback to AlwaysUnderline.
                }

                if (key != null) {
                    string s = (string)key.GetValue("Anchor Underline");
                    key.Close();

                    if (s != null && string.Compare(s, "no", true, CultureInfo.InvariantCulture) == 0) {
                        return LinkBehavior.NeverUnderline;
                    }
                    if (s != null && string.Compare(s, "hover", true, CultureInfo.InvariantCulture) == 0) {
                        return LinkBehavior.HoverUnderline;
                    }
                    else {
                        return LinkBehavior.AlwaysUnderline;
                    }
                }
            }
            finally {
                System.Security.CodeAccessPermission.RevertAssert();
            }

            return LinkBehavior.AlwaysUnderline;
        }

        public static void EnsureLinkFonts(Font baseFont, LinkBehavior link, ref Font linkFont, ref Font hoverLinkFont) {
            if (linkFont != null && hoverLinkFont != null) {
                return;
            }

            bool underlineLink = true;
            bool underlineHover = true;

            if (link == LinkBehavior.SystemDefault) {
                link = GetIELinkBehavior();
            }

            switch (link) {
                case LinkBehavior.AlwaysUnderline:
                    underlineLink = true;
                    underlineHover = true;
                    break;
                case LinkBehavior.HoverUnderline:
                    underlineLink = false;
                    underlineHover = true;
                    break;
                case LinkBehavior.NeverUnderline:
                    underlineLink = false;
                    underlineHover = false;
                    break;
            }

            Font f = baseFont;

            // We optimize for the "same" value (never & always) to avoid creating an
            // extra font object.
            //
            if (underlineHover == underlineLink) {
                FontStyle style = f.Style;
                if (underlineHover) {
                    style |= FontStyle.Underline;
                }
                else {
                    style &= ~FontStyle.Underline;
                }
                hoverLinkFont = new Font(f, style);
                linkFont = hoverLinkFont;
            }
            else {
                FontStyle hoverStyle = f.Style;
                if (underlineHover) {
                    hoverStyle |= FontStyle.Underline;
                }
                else {
                    hoverStyle &= ~FontStyle.Underline;
                }

                hoverLinkFont = new Font(f, hoverStyle);

                FontStyle linkStyle = f.Style;
                if (underlineLink) {
                    linkStyle |= FontStyle.Underline;
                }
                else {
                    linkStyle &= ~FontStyle.Underline;
                }

                linkFont = new Font(f, linkStyle);
            }
        }
    }
}

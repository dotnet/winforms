// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Globalization;
using Microsoft.Win32;

namespace System.Windows.Forms;

internal static class LinkUtilities
{
    // IE fonts and colors
    private static Color s_ielinkColor = Color.Empty;
    private static Color s_ieactiveLinkColor = Color.Empty;
    private static Color s_ievisitedLinkColor = Color.Empty;

    private const string IESettingsRegPath = "Software\\Microsoft\\Internet Explorer\\Settings";
    public const string IEMainRegPath = "Software\\Microsoft\\Internet Explorer\\Main";
    private const string IEAnchorColor = "Anchor Color";
    private const string IEAnchorColorVisited = "Anchor Color Visited";
    private const string IEAnchorColorHover = "Anchor Color Hover";

    /// <summary>
    ///  Retrieves a named IE color from the registry. There are constants at the top
    ///  of this file of the valid names to retrieve.
    /// </summary>
    private static Color GetIEColor(string name)
    {
        RegistryKey? key = Registry.CurrentUser.OpenSubKey(IESettingsRegPath);

        if (key is not null)
        {
            // Since this comes from the registry, be very careful about its contents.
            string? s = (string?)key.GetValue(name);
            key.Close();

            if (s is not null)
            {
                Span<Range> rgbs = stackalloc Range[4];
                int rgbsCount = s.AsSpan().Split(rgbs, ',');
                Span<int> rgb = stackalloc int[3];
                rgb.Clear();

                int nMax = Math.Min(rgb.Length, rgbsCount);

                // NOTE: if we can't parse rgbs[i], rgb[i] will be set to 0.
                for (int i = 0; i < nMax; i++)
                {
                    int.TryParse(s.AsSpan(rgbs[i]), out rgb[i]);
                }

                return Color.FromArgb(rgb[0], rgb[1], rgb[2]);
            }
        }

        if (string.Equals(name, IEAnchorColor, StringComparison.OrdinalIgnoreCase))
        {
            return Color.Blue;
        }
        else if (string.Equals(name, IEAnchorColorVisited, StringComparison.OrdinalIgnoreCase))
        {
            return Color.Purple;
        }
        else if (string.Equals(name, IEAnchorColorHover, StringComparison.OrdinalIgnoreCase))
        {
            return Color.Red;
        }
        else
        {
            return Color.Red;
        }
    }

    public static Color IELinkColor
    {
        get
        {
            if (s_ielinkColor.IsEmpty)
            {
                s_ielinkColor = GetIEColor(IEAnchorColor);
            }

            return s_ielinkColor;
        }
    }

    public static Color IEActiveLinkColor
    {
        get
        {
            if (s_ieactiveLinkColor.IsEmpty)
            {
                s_ieactiveLinkColor = GetIEColor(IEAnchorColorHover);
            }

            return s_ieactiveLinkColor;
        }
    }

    public static Color IEVisitedLinkColor
    {
        get
        {
            if (s_ievisitedLinkColor.IsEmpty)
            {
                s_ievisitedLinkColor = GetIEColor(IEAnchorColorVisited);
            }

            return s_ievisitedLinkColor;
        }
    }

    ///  Produces a color for visited links using SystemColors
    public static Color GetVisitedLinkColor()
    {
        int r = (SystemColors.Window.R + Application.SystemColors.WindowText.R + 1) / 2;
        int g = Application.SystemColors.WindowText.G;
        int b = (SystemColors.Window.B + Application.SystemColors.WindowText.B + 1) / 2;

        return Color.FromArgb(r, g, b);
    }

    /// <summary>
    ///  Retrieves the IE settings for link behavior from the registry.
    /// </summary>
    public static LinkBehavior GetIELinkBehavior()
    {
        RegistryKey? key = null;
        try
        {
            key = Registry.CurrentUser.OpenSubKey(IEMainRegPath);
        }
        catch (Security.SecurityException)
        {
            // User does not have right to access Registry path HKCU\\Software\\Microsoft\\Internet Explorer\\Main.
            // Catch SecurityException silently and let the return value fallback to AlwaysUnderline.
        }

        if (key is not null)
        {
            string? s = (string?)key.GetValue("Anchor Underline");
            key.Close();

            if (s is not null && string.Compare(s, "no", true, CultureInfo.InvariantCulture) == 0)
            {
                return LinkBehavior.NeverUnderline;
            }

            if (s is not null && string.Compare(s, "hover", true, CultureInfo.InvariantCulture) == 0)
            {
                return LinkBehavior.HoverUnderline;
            }
            else
            {
                return LinkBehavior.AlwaysUnderline;
            }
        }

        return LinkBehavior.AlwaysUnderline;
    }

    public static void EnsureLinkFonts(
        Font baseFont,
        LinkBehavior link,
        [AllowNull] ref Font linkFont,
        [AllowNull] ref Font hoverLinkFont,
        bool isActive = false)
    {
        if (linkFont is not null && hoverLinkFont is not null)
        {
            return;
        }

        bool underlineLink = true;
        bool underlineHover = true;

        if (link == LinkBehavior.SystemDefault)
        {
            link = GetIELinkBehavior();
        }

        switch (link)
        {
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

        // We optimize for the "same" value (never & always) to avoid creating an extra font object.
        if (underlineHover == underlineLink)
        {
            FontStyle style = f.Style;
            if (underlineHover)
            {
                style |= FontStyle.Underline;
            }
            else
            {
                style &= ~FontStyle.Underline;
            }

            if (isActive)
            {
                style |= FontStyle.Bold;
            }
            else
            {
                style &= ~FontStyle.Bold;
            }

            hoverLinkFont = new Font(f, style);
            linkFont = hoverLinkFont;
        }
        else
        {
            FontStyle hoverStyle = f.Style;
            if (underlineHover)
            {
                hoverStyle |= FontStyle.Underline;
            }
            else
            {
                hoverStyle &= ~FontStyle.Underline;
            }

            hoverLinkFont = new Font(f, hoverStyle);

            FontStyle linkStyle = f.Style;
            if (underlineLink)
            {
                linkStyle |= FontStyle.Underline;
            }
            else
            {
                linkStyle &= ~FontStyle.Underline;
            }

            linkFont = new Font(f, linkStyle);
        }
    }
}

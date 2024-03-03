// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing;

public static class SystemColorsRouter
{
    internal static Func<Color> ActiveBorderRouter = () => SystemColors.ActiveBorder;

    public static Color ActiveBorder => ActiveBorderRouter();
    public static Color ActiveCaption => SystemColors.ActiveCaption;
    public static Color ActiveCaptionText => SystemColors.ActiveCaptionText;
    public static Color AppWorkspace => SystemColors.AppWorkspace;
    public static Color ButtonFace => SystemColors.ButtonFace;
    public static Color ButtonHighlight => SystemColors.ButtonHighlight;
    public static Color ButtonShadow => SystemColors.ButtonShadow;
    public static Color Control => SystemColors.Control;
    public static Color ControlDark => SystemColors.ControlDark;
    public static Color ControlDarkDark => SystemColors.ControlDarkDark;
    public static Color ControlLight => SystemColors.ControlLight;
    public static Color ControlLightLight => SystemColors.ControlLightLight;
    public static Color ControlText => SystemColors.ControlText;
    public static Color Desktop => SystemColors.Desktop;
    public static Color GradientActiveCaption => SystemColors.GradientActiveCaption;
    public static Color GradientInactiveCaption => SystemColors.GradientInactiveCaption;
    public static Color GrayText => SystemColors.GrayText;
    public static Color Highlight => SystemColors.Highlight;
    public static Color HighlightText => SystemColors.HighlightText;
    public static Color HotTrack => SystemColors.HotTrack;
    public static Color InactiveBorder => SystemColors.InactiveBorder;
    public static Color InactiveCaption => SystemColors.InactiveCaption;
    public static Color InactiveCaptionText => SystemColors.InactiveCaptionText;
    public static Color Info => SystemColors.Info;
    public static Color InfoText => SystemColors.InfoText;
    public static Color Menu => SystemColors.Menu;
    public static Color MenuBar => SystemColors.MenuBar;
    public static Color MenuHighlight => SystemColors.MenuHighlight;
    public static Color MenuText => SystemColors.MenuText;
    public static Color ScrollBar => SystemColors.ScrollBar;
    public static Color Window => SystemColors.Window;
    public static Color WindowFrame => SystemColors.WindowFrame;
    public static Color WindowText => SystemColors.WindowText;
}

public static class SystemPens
{
    private static readonly object s_systemPensKey = new();

    public static Pen ActiveBorder => FromSystemColor(SystemColors.ActiveBorder);
    public static Pen ActiveCaption => FromSystemColor(SystemColors.ActiveCaption);
    public static Pen ActiveCaptionText => FromSystemColor(SystemColors.ActiveCaptionText);
    public static Pen AppWorkspace => FromSystemColor(SystemColors.AppWorkspace);

    public static Pen ButtonFace => FromSystemColor(SystemColors.ButtonFace);
    public static Pen ButtonHighlight => FromSystemColor(SystemColors.ButtonHighlight);

    public static Pen ButtonShadow => FromSystemColor(SystemColors.ButtonShadow);

    public static Pen Control => FromSystemColor(SystemColors.Control);
    public static Pen ControlText => FromSystemColor(SystemColors.ControlText);
    public static Pen ControlDark => FromSystemColor(SystemColors.ControlDark);
    public static Pen ControlDarkDark => FromSystemColor(SystemColors.ControlDarkDark);
    public static Pen ControlLight => FromSystemColor(SystemColors.ControlLight);
    public static Pen ControlLightLight => FromSystemColor(SystemColors.ControlLightLight);

    public static Pen Desktop => FromSystemColor(SystemColors.Desktop);

    public static Pen GradientActiveCaption => FromSystemColor(SystemColors.GradientActiveCaption);
    public static Pen GradientInactiveCaption => FromSystemColor(SystemColors.GradientInactiveCaption);
    public static Pen GrayText => FromSystemColor(SystemColors.GrayText);

    public static Pen Highlight => FromSystemColor(SystemColors.Highlight);
    public static Pen HighlightText => FromSystemColor(SystemColors.HighlightText);
    public static Pen HotTrack => FromSystemColor(SystemColors.HotTrack);

    public static Pen InactiveBorder => FromSystemColor(SystemColors.InactiveBorder);
    public static Pen InactiveCaption => FromSystemColor(SystemColors.InactiveCaption);
    public static Pen InactiveCaptionText => FromSystemColor(SystemColors.InactiveCaptionText);
    public static Pen Info => FromSystemColor(SystemColors.Info);
    public static Pen InfoText => FromSystemColor(SystemColors.InfoText);

    public static Pen Menu => FromSystemColor(SystemColors.Menu);
    public static Pen MenuBar => FromSystemColor(SystemColors.MenuBar);
    public static Pen MenuHighlight => FromSystemColor(SystemColors.MenuHighlight);
    public static Pen MenuText => FromSystemColor(SystemColors.MenuText);

    public static Pen ScrollBar => FromSystemColor(SystemColors.ScrollBar);

    public static Pen Window => FromSystemColor(SystemColors.Window);
    public static Pen WindowFrame => FromSystemColor(SystemColors.WindowFrame);
    public static Pen WindowText => FromSystemColor(SystemColors.WindowText);

    public static Pen FromSystemColor(Color c)
    {
        if (!c.IsSystemColor)
        {
            throw new ArgumentException(SR.Format(SR.ColorNotSystemColor, c.ToString()));
        }

        if (!Gdip.ThreadData.TryGetValue(s_systemPensKey, out object? tempSystemPens) || tempSystemPens is not Pen[] systemPens)
        {
            systemPens = new Pen[(int)KnownColor.WindowText + (int)KnownColor.MenuHighlight - (int)KnownColor.YellowGreen];
            Gdip.ThreadData[s_systemPensKey] = systemPens;
        }

        int idx = (int)c.ToKnownColor();
        if (idx > (int)KnownColor.YellowGreen)
        {
            idx -= (int)KnownColor.YellowGreen - (int)KnownColor.WindowText;
        }

        idx--;
        Debug.Assert(idx >= 0 && idx < systemPens.Length, "System colors have been added but our system color array has not been expanded.");

        return systemPens[idx] ??= new Pen(c, true);
    }
}

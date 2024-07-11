// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

internal static class SystemCOLORs
{
    private static readonly Dictionary<COLORREF, List<SYS_COLOR_INDEX>> s_systemColors = CreateColorDictionary();

    private static readonly Dictionary<SYS_COLOR_INDEX, string> s_names = new()
    {
        { SYS_COLOR_INDEX.COLOR_SCROLLBAR, "COLOR_SCROLLBAR" },
        { SYS_COLOR_INDEX.COLOR_BACKGROUND, "COLOR_BACKGROUND" },
        { SYS_COLOR_INDEX.COLOR_ACTIVECAPTION, "COLOR_ACTIVECAPTION" },
        { SYS_COLOR_INDEX.COLOR_INACTIVECAPTION, "COLOR_INACTIVECAPTION" },
        { SYS_COLOR_INDEX.COLOR_MENU, "COLOR_MENU" },
        { SYS_COLOR_INDEX.COLOR_WINDOW, "COLOR_WINDOW" },
        { SYS_COLOR_INDEX.COLOR_WINDOWFRAME, "COLOR_WINDOWFRAME" },
        { SYS_COLOR_INDEX.COLOR_MENUTEXT, "COLOR_MENUTEXT" },
        { SYS_COLOR_INDEX.COLOR_WINDOWTEXT, "COLOR_WINDOWTEXT" },
        { SYS_COLOR_INDEX.COLOR_CAPTIONTEXT, "COLOR_CAPTIONTEXT" },
        { SYS_COLOR_INDEX.COLOR_ACTIVEBORDER, "COLOR_ACTIVEBORDER" },
        { SYS_COLOR_INDEX.COLOR_INACTIVEBORDER, "COLOR_INACTIVEBORDER" },
        { SYS_COLOR_INDEX.COLOR_APPWORKSPACE, "COLOR_APPWORKSPACE" },
        { SYS_COLOR_INDEX.COLOR_HIGHLIGHT, "COLOR_HIGHLIGHT" },
        { SYS_COLOR_INDEX.COLOR_HIGHLIGHTTEXT, "COLOR_HIGHLIGHTTEXT" },
        { SYS_COLOR_INDEX.COLOR_BTNFACE, "COLOR_BTNFACE" },
        { SYS_COLOR_INDEX.COLOR_BTNSHADOW, "COLOR_BTNSHADOW" },
        { SYS_COLOR_INDEX.COLOR_GRAYTEXT, "COLOR_GRAYTEXT" },
        { SYS_COLOR_INDEX.COLOR_BTNTEXT, "COLOR_BTNTEXT" },
        { SYS_COLOR_INDEX.COLOR_INACTIVECAPTIONTEXT, "COLOR_INACTIVECAPTIONTEXT" },
        { SYS_COLOR_INDEX.COLOR_BTNHIGHLIGHT, "COLOR_BTNHIGHLIGHT" },
        { SYS_COLOR_INDEX.COLOR_3DDKSHADOW, "COLOR_3DDKSHADOW" },
        { SYS_COLOR_INDEX.COLOR_3DLIGHT, "COLOR_3DLIGHT" },
        { SYS_COLOR_INDEX.COLOR_INFOTEXT, "COLOR_INFOTEXT" },
        { SYS_COLOR_INDEX.COLOR_INFOBK, "COLOR_INFOBK" },
        { SYS_COLOR_INDEX.COLOR_HOTLIGHT, "COLOR_HOTLIGHT" },
        { SYS_COLOR_INDEX.COLOR_GRADIENTACTIVECAPTION, "COLOR_GRADIENTACTIVECAPTION" },
        { SYS_COLOR_INDEX.COLOR_GRADIENTINACTIVECAPTION, "COLOR_GRADIENTINACTIVECAPTION" },
        { SYS_COLOR_INDEX.COLOR_MENUHILIGHT, "COLOR_MENUHILIGHT" },
        { SYS_COLOR_INDEX.COLOR_MENUBAR, "COLOR_MENUBAR" },
    };

    public static bool TryGetSystemColor(COLORREF colorRef, out List<SYS_COLOR_INDEX> colors)
        => s_systemColors.TryGetValue(colorRef, out colors);

    public static string ToSystemColorString(COLORREF colorRef)
    {
        if (TryGetSystemColor(colorRef, out List<SYS_COLOR_INDEX> colors))
        {
            string colorString = string.Join(", ", colors.Select(c => s_names[c]));
            return $"{colorRef} ({colorString})";
        }
        else
        {
            return colorRef.ToString();
        }
    }

    private static Dictionary<COLORREF, List<SYS_COLOR_INDEX>> CreateColorDictionary()
    {
        Dictionary<COLORREF, List<SYS_COLOR_INDEX>> dictionary = [];

        for (int i = 0; i <= (int)SYS_COLOR_INDEX.COLOR_MENUBAR; i++)
        {
            if (i == 25)
            {
                // Only undefined value
                continue;
            }

            COLORREF colorRef = (COLORREF)PInvoke.GetSysColor((SYS_COLOR_INDEX)i);

            if (dictionary.TryGetValue(colorRef, out List<SYS_COLOR_INDEX> colors))
            {
                colors.Add((SYS_COLOR_INDEX)i);
            }
            else
            {
                List<SYS_COLOR_INDEX> colorList =
                [
                    (SYS_COLOR_INDEX)i
                ];

                dictionary.Add(colorRef, colorList);
            }
        }

        return dictionary;
    }
}

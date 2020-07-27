// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using static Interop;

namespace System
{
    internal static class SystemCOLORs
    {
        private readonly static Dictionary<COLORREF, List<User32.COLOR>> s_systemColors = CreateColorDictionary();

        private readonly static Dictionary<User32.COLOR, string> s_names = new Dictionary<User32.COLOR, string>
        {
            { User32.COLOR.SCROLLBAR, "COLOR_SCROLLBAR" },
            { User32.COLOR.BACKGROUND, "COLOR_BACKGROUND" },
            { User32.COLOR.ACTIVECAPTION, "COLOR_ACTIVECAPTION" },
            { User32.COLOR.INACTIVECAPTION, "COLOR_INACTIVECAPTION" },
            { User32.COLOR.MENU, "COLOR_MENU" },
            { User32.COLOR.WINDOW, "COLOR_WINDOW" },
            { User32.COLOR.WINDOWFRAME, "COLOR_WINDOWFRAME" },
            { User32.COLOR.MENUTEXT, "COLOR_MENUTEXT" },
            { User32.COLOR.WINDOWTEXT, "COLOR_WINDOWTEXT" },
            { User32.COLOR.CAPTIONTEXT, "COLOR_CAPTIONTEXT" },
            { User32.COLOR.ACTIVEBORDER, "COLOR_ACTIVEBORDER" },
            { User32.COLOR.INACTIVEBORDER, "COLOR_INACTIVEBORDER" },
            { User32.COLOR.APPWORKSPACE, "COLOR_APPWORKSPACE" },
            { User32.COLOR.HIGHLIGHT, "COLOR_HIGHLIGHT" },
            { User32.COLOR.HIGHLIGHTTEXT, "COLOR_HIGHLIGHTTEXT" },
            { User32.COLOR.BTNFACE, "COLOR_BTNFACE" },
            { User32.COLOR.BTNSHADOW, "COLOR_BTNSHADOW" },
            { User32.COLOR.GRAYTEXT, "COLOR_GRAYTEXT" },
            { User32.COLOR.BTNTEXT, "COLOR_BTNTEXT" },
            { User32.COLOR.INACTIVECAPTIONTEXT, "COLOR_INACTIVECAPTIONTEXT" },
            { User32.COLOR.BTNHIGHLIGHT, "COLOR_BTNHIGHLIGHT" },
            { User32.COLOR.DKSHADOW3D, "COLOR_3DDKSHADOW" },
            { User32.COLOR.LIGHT3D, "COLOR_3DLIGHT" },
            { User32.COLOR.INFOTEXT, "COLOR_INFOTEXT" },
            { User32.COLOR.INFOBK, "COLOR_INFOBK" },
            { User32.COLOR.HOTLIGHT, "COLOR_HOTLIGHT" },
            { User32.COLOR.GRADIENTACTIVECAPTION, "COLOR_GRADIENTACTIVECAPTION" },
            { User32.COLOR.GRADIENTINACTIVECAPTION, "COLOR_GRADIENTINACTIVECAPTION" },
            { User32.COLOR.MENUHILIGHT, "COLOR_MENUHILIGHT" },
            { User32.COLOR.MENUBAR, "COLOR_MENUBAR" },
        };

        public static bool TryGetSystemColor(COLORREF colorRef, out List<User32.COLOR> colors)
            => s_systemColors.TryGetValue(colorRef, out colors);

        public static string ToSystemColorString(COLORREF colorRef)
        {
            if (TryGetSystemColor(colorRef, out List<User32.COLOR> colors))
            {
                string colorString = string.Empty;
                for (int i = 0; i < colors.Count; i++)
                {
                    colorString += s_names[colors[i]];
                    if (i < colors.Count - 1)
                    {
                        colorString += ", ";
                    }
                }

                return $"{colorRef} ({colorString})";
            }
            else
            {
                return colorRef.ToString();
            }
        }

        private static Dictionary<COLORREF, List<User32.COLOR>> CreateColorDictionary()
        {
            var dictionary = new Dictionary<COLORREF, List<User32.COLOR>>();

            for (int i = 0; i <= (int)User32.COLOR.MENUBAR; i++)
            {
                if (i == 25)
                {
                    // Only undefined value
                    continue;
                }

                COLORREF colorRef = User32.GetSysColor((User32.COLOR)i);

                if (dictionary.TryGetValue(colorRef, out List<User32.COLOR> colors))
                {
                    colors.Add((User32.COLOR)i);
                }
                else
                {
                    var colorList = new List<User32.COLOR>
                    {
                        (User32.COLOR)i
                    };
                    dictionary.Add(colorRef, colorList);
                }
            }

            return dictionary;
        }
    }
}

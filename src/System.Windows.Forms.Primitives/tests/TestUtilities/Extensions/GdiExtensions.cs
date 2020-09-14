// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Numerics;
using static Interop;

namespace System
{
    internal static class GdiExtensions
    {
        public static string ToSystemColorString(this COLORREF colorRef)
            => SystemCOLORs.ToSystemColorString(colorRef);

        public static Point TransformPoint(in this Matrix3x2 transform, Point point)
        {
            if (transform.IsIdentity)
            {
                return point;
            }

            float y = point.Y;
            float x = point.X;

            float xadd = y * transform.M21 + transform.M31;
            float yadd = x * transform.M12 + transform.M32;
            x *= transform.M11;
            x += xadd;
            y *= transform.M22;
            y += yadd;

            return new Point((int)x, (int)y);
        }

        public static Color ToSystemColor(User32.COLOR color) => color switch
        {
            User32.COLOR.SCROLLBAR => SystemColors.ScrollBar,
            User32.COLOR.DESKTOP => SystemColors.Desktop,                   // Also BACKGROUND
            User32.COLOR.ACTIVECAPTION => SystemColors.ActiveCaption,
            User32.COLOR.INACTIVECAPTION => SystemColors.InactiveCaption,
            User32.COLOR.MENU => SystemColors.Menu,
            User32.COLOR.WINDOW => SystemColors.Window,
            User32.COLOR.WINDOWFRAME => SystemColors.WindowFrame,
            User32.COLOR.MENUTEXT => SystemColors.MenuText,
            User32.COLOR.WINDOWTEXT => SystemColors.WindowText,
            User32.COLOR.CAPTIONTEXT => SystemColors.ActiveCaptionText,
            User32.COLOR.ACTIVEBORDER => SystemColors.ActiveBorder,
            User32.COLOR.INACTIVEBORDER => SystemColors.InactiveBorder,
            User32.COLOR.APPWORKSPACE => SystemColors.AppWorkspace,
            User32.COLOR.HIGHLIGHT => SystemColors.Highlight,
            User32.COLOR.HIGHLIGHTTEXT => SystemColors.HighlightText,
            User32.COLOR.BTNFACE => SystemColors.ButtonFace,
            User32.COLOR.BTNSHADOW => SystemColors.ButtonShadow,
            User32.COLOR.GRAYTEXT => SystemColors.GrayText,
            User32.COLOR.BTNTEXT => SystemColors.ControlText,
            User32.COLOR.INACTIVECAPTIONTEXT => SystemColors.InactiveCaptionText,
            User32.COLOR.BTNHIGHLIGHT => SystemColors.ButtonHighlight,
            User32.COLOR.DKSHADOW3D => SystemColors.ControlDarkDark,
            User32.COLOR.LIGHT3D => SystemColors.ControlLight,
            User32.COLOR.INFOTEXT => SystemColors.InfoText,
            User32.COLOR.INFOBK => SystemColors.Info,
            User32.COLOR.HOTLIGHT => SystemColors.HotTrack,
            User32.COLOR.GRADIENTACTIVECAPTION => SystemColors.GradientActiveCaption,
            User32.COLOR.GRADIENTINACTIVECAPTION => SystemColors.GradientInactiveCaption,
            User32.COLOR.MENUHILIGHT => SystemColors.MenuHighlight,
            User32.COLOR.MENUBAR => SystemColors.MenuBar,
            _ => throw new ArgumentOutOfRangeException(nameof(color))
        };
    }
}

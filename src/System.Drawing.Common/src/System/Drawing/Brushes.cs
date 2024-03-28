// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
namespace System.Drawing;

public static class Brushes
{
    private static readonly object s_transparentKey = new();
    private static readonly object s_aliceBlueKey = new();
    private static readonly object s_antiqueWhiteKey = new();
    private static readonly object s_aquaKey = new();
    private static readonly object s_aquamarineKey = new();
    private static readonly object s_azureKey = new();
    private static readonly object s_beigeKey = new();
    private static readonly object s_bisqueKey = new();
    private static readonly object s_blackKey = new();
    private static readonly object s_blanchedAlmondKey = new();
    private static readonly object s_blueKey = new();
    private static readonly object s_blueVioletKey = new();
    private static readonly object s_brownKey = new();
    private static readonly object s_burlyWoodKey = new();
    private static readonly object s_cadetBlueKey = new();
    private static readonly object s_chartreuseKey = new();
    private static readonly object s_chocolateKey = new();
    private static readonly object s_coralKey = new();
    private static readonly object s_cornflowerBlueKey = new();
    private static readonly object s_cornsilkKey = new();
    private static readonly object s_crimsonKey = new();
    private static readonly object s_cyanKey = new();
    private static readonly object s_darkBlueKey = new();
    private static readonly object s_darkCyanKey = new();
    private static readonly object s_darkGoldenrodKey = new();
    private static readonly object s_darkGrayKey = new();
    private static readonly object s_darkGreenKey = new();
    private static readonly object s_darkKhakiKey = new();
    private static readonly object s_darkMagentaKey = new();
    private static readonly object s_darkOliveGreenKey = new();
    private static readonly object s_darkOrangeKey = new();
    private static readonly object s_darkOrchidKey = new();
    private static readonly object s_darkRedKey = new();
    private static readonly object s_darkSalmonKey = new();
    private static readonly object s_darkSeaGreenKey = new();
    private static readonly object s_darkSlateBlueKey = new();
    private static readonly object s_darkSlateGrayKey = new();
    private static readonly object s_darkTurquoiseKey = new();
    private static readonly object s_darkVioletKey = new();
    private static readonly object s_deepPinkKey = new();
    private static readonly object s_deepSkyBlueKey = new();
    private static readonly object s_dimGrayKey = new();
    private static readonly object s_dodgerBlueKey = new();
    private static readonly object s_firebrickKey = new();
    private static readonly object s_floralWhiteKey = new();
    private static readonly object s_forestGreenKey = new();
    private static readonly object s_fuchsiaKey = new();
    private static readonly object s_gainsboroKey = new();
    private static readonly object s_ghostWhiteKey = new();
    private static readonly object s_goldKey = new();
    private static readonly object s_goldenrodKey = new();
    private static readonly object s_grayKey = new();
    private static readonly object s_greenKey = new();
    private static readonly object s_greenYellowKey = new();
    private static readonly object s_honeydewKey = new();
    private static readonly object s_hotPinkKey = new();
    private static readonly object s_indianRedKey = new();
    private static readonly object s_indigoKey = new();
    private static readonly object s_ivoryKey = new();
    private static readonly object s_khakiKey = new();
    private static readonly object s_lavenderKey = new();
    private static readonly object s_lavenderBlushKey = new();
    private static readonly object s_lawnGreenKey = new();
    private static readonly object s_lemonChiffonKey = new();
    private static readonly object s_lightBlueKey = new();
    private static readonly object s_lightCoralKey = new();
    private static readonly object s_lightCyanKey = new();
    private static readonly object s_lightGoldenrodYellowKey = new();
    private static readonly object s_lightGreenKey = new();
    private static readonly object s_lightGrayKey = new();
    private static readonly object s_lightPinkKey = new();
    private static readonly object s_lightSalmonKey = new();
    private static readonly object s_lightSeaGreenKey = new();
    private static readonly object s_lightSkyBlueKey = new();
    private static readonly object s_lightSlateGrayKey = new();
    private static readonly object s_lightSteelBlueKey = new();
    private static readonly object s_lightYellowKey = new();
    private static readonly object s_limeKey = new();
    private static readonly object s_limeGreenKey = new();
    private static readonly object s_linenKey = new();
    private static readonly object s_magentaKey = new();
    private static readonly object s_maroonKey = new();
    private static readonly object s_mediumAquamarineKey = new();
    private static readonly object s_mediumBlueKey = new();
    private static readonly object s_mediumOrchidKey = new();
    private static readonly object s_mediumPurpleKey = new();
    private static readonly object s_mediumSeaGreenKey = new();
    private static readonly object s_mediumSlateBlueKey = new();
    private static readonly object s_mediumSpringGreenKey = new();
    private static readonly object s_mediumTurquoiseKey = new();
    private static readonly object s_mediumVioletRedKey = new();
    private static readonly object s_midnightBlueKey = new();
    private static readonly object s_mintCreamKey = new();
    private static readonly object s_mistyRoseKey = new();
    private static readonly object s_moccasinKey = new();
    private static readonly object s_navajoWhiteKey = new();
    private static readonly object s_navyKey = new();
    private static readonly object s_oldLaceKey = new();
    private static readonly object s_oliveKey = new();
    private static readonly object s_oliveDrabKey = new();
    private static readonly object s_orangeKey = new();
    private static readonly object s_orangeRedKey = new();
    private static readonly object s_orchidKey = new();
    private static readonly object s_paleGoldenrodKey = new();
    private static readonly object s_paleGreenKey = new();
    private static readonly object s_paleTurquoiseKey = new();
    private static readonly object s_paleVioletRedKey = new();
    private static readonly object s_papayaWhipKey = new();
    private static readonly object s_peachPuffKey = new();
    private static readonly object s_peruKey = new();
    private static readonly object s_pinkKey = new();
    private static readonly object s_plumKey = new();
    private static readonly object s_powderBlueKey = new();
    private static readonly object s_purpleKey = new();
    private static readonly object s_redKey = new();
    private static readonly object s_rosyBrownKey = new();
    private static readonly object s_royalBlueKey = new();
    private static readonly object s_saddleBrownKey = new();
    private static readonly object s_salmonKey = new();
    private static readonly object s_sandyBrownKey = new();
    private static readonly object s_seaGreenKey = new();
    private static readonly object s_seaShellKey = new();
    private static readonly object s_siennaKey = new();
    private static readonly object s_silverKey = new();
    private static readonly object s_skyBlueKey = new();
    private static readonly object s_slateBlueKey = new();
    private static readonly object s_slateGrayKey = new();
    private static readonly object s_snowKey = new();
    private static readonly object s_springGreenKey = new();
    private static readonly object s_steelBlueKey = new();
    private static readonly object s_tanKey = new();
    private static readonly object s_tealKey = new();
    private static readonly object s_thistleKey = new();
    private static readonly object s_tomatoKey = new();
    private static readonly object s_turquoiseKey = new();
    private static readonly object s_violetKey = new();
    private static readonly object s_wheatKey = new();
    private static readonly object s_whiteKey = new();
    private static readonly object s_whiteSmokeKey = new();
    private static readonly object s_yellowKey = new();
    private static readonly object s_yellowGreenKey = new();

    public static Brush Transparent => GetBrush(s_transparentKey, Color.Transparent);

    public static Brush AliceBlue => GetBrush(s_aliceBlueKey, Color.AliceBlue);
    public static Brush AntiqueWhite => GetBrush(s_antiqueWhiteKey, Color.AntiqueWhite);
    public static Brush Aqua => GetBrush(s_aquaKey, Color.Aqua);
    public static Brush Aquamarine => GetBrush(s_aquamarineKey, Color.Aquamarine);
    public static Brush Azure => GetBrush(s_azureKey, Color.Azure);

    public static Brush Beige => GetBrush(s_beigeKey, Color.Beige);
    public static Brush Bisque => GetBrush(s_bisqueKey, Color.Bisque);
    public static Brush Black => GetBrush(s_blackKey, Color.Black);
    public static Brush BlanchedAlmond => GetBrush(s_blanchedAlmondKey, Color.BlanchedAlmond);
    public static Brush Blue => GetBrush(s_blueKey, Color.Blue);
    public static Brush BlueViolet => GetBrush(s_blueVioletKey, Color.BlueViolet);
    public static Brush Brown => GetBrush(s_brownKey, Color.Brown);
    public static Brush BurlyWood => GetBrush(s_burlyWoodKey, Color.BurlyWood);

    public static Brush CadetBlue => GetBrush(s_cadetBlueKey, Color.CadetBlue);
    public static Brush Chartreuse => GetBrush(s_chartreuseKey, Color.Chartreuse);
    public static Brush Chocolate => GetBrush(s_chocolateKey, Color.Chocolate);
    public static Brush Coral => GetBrush(s_coralKey, Color.Coral);
    public static Brush CornflowerBlue => GetBrush(s_cornflowerBlueKey, Color.CornflowerBlue);
    public static Brush Cornsilk => GetBrush(s_cornsilkKey, Color.Cornsilk);
    public static Brush Crimson => GetBrush(s_crimsonKey, Color.Crimson);
    public static Brush Cyan => GetBrush(s_cyanKey, Color.Cyan);

    public static Brush DarkBlue => GetBrush(s_darkBlueKey, Color.DarkBlue);
    public static Brush DarkCyan => GetBrush(s_darkCyanKey, Color.DarkCyan);
    public static Brush DarkGoldenrod => GetBrush(s_darkGoldenrodKey, Color.DarkGoldenrod);
    public static Brush DarkGray => GetBrush(s_darkGrayKey, Color.DarkGray);
    public static Brush DarkGreen => GetBrush(s_darkGreenKey, Color.DarkGreen);
    public static Brush DarkKhaki => GetBrush(s_darkKhakiKey, Color.DarkKhaki);
    public static Brush DarkMagenta => GetBrush(s_darkMagentaKey, Color.DarkMagenta);
    public static Brush DarkOliveGreen => GetBrush(s_darkOliveGreenKey, Color.DarkOliveGreen);
    public static Brush DarkOrange => GetBrush(s_darkOrangeKey, Color.DarkOrange);
    public static Brush DarkOrchid => GetBrush(s_darkOrchidKey, Color.DarkOrchid);
    public static Brush DarkRed => GetBrush(s_darkRedKey, Color.DarkRed);
    public static Brush DarkSalmon => GetBrush(s_darkSalmonKey, Color.DarkSalmon);
    public static Brush DarkSeaGreen => GetBrush(s_darkSeaGreenKey, Color.DarkSeaGreen);
    public static Brush DarkSlateBlue => GetBrush(s_darkSlateBlueKey, Color.DarkSlateBlue);
    public static Brush DarkSlateGray => GetBrush(s_darkSlateGrayKey, Color.DarkSlateGray);
    public static Brush DarkTurquoise => GetBrush(s_darkTurquoiseKey, Color.DarkTurquoise);
    public static Brush DarkViolet => GetBrush(s_darkVioletKey, Color.DarkViolet);
    public static Brush DeepPink => GetBrush(s_deepPinkKey, Color.DeepPink);
    public static Brush DeepSkyBlue => GetBrush(s_deepSkyBlueKey, Color.DeepSkyBlue);
    public static Brush DimGray => GetBrush(s_dimGrayKey, Color.DimGray);
    public static Brush DodgerBlue => GetBrush(s_dodgerBlueKey, Color.DodgerBlue);

    public static Brush Firebrick => GetBrush(s_firebrickKey, Color.Firebrick);
    public static Brush FloralWhite => GetBrush(s_floralWhiteKey, Color.FloralWhite);
    public static Brush ForestGreen => GetBrush(s_forestGreenKey, Color.ForestGreen);
    public static Brush Fuchsia => GetBrush(s_fuchsiaKey, Color.Fuchsia);

    public static Brush Gainsboro => GetBrush(s_gainsboroKey, Color.Gainsboro);
    public static Brush GhostWhite => GetBrush(s_ghostWhiteKey, Color.GhostWhite);
    public static Brush Gold => GetBrush(s_goldKey, Color.Gold);
    public static Brush Goldenrod => GetBrush(s_goldenrodKey, Color.Goldenrod);
    public static Brush Gray => GetBrush(s_grayKey, Color.Gray);
    public static Brush Green => GetBrush(s_greenKey, Color.Green);
    public static Brush GreenYellow => GetBrush(s_greenYellowKey, Color.GreenYellow);

    public static Brush Honeydew => GetBrush(s_honeydewKey, Color.Honeydew);
    public static Brush HotPink => GetBrush(s_hotPinkKey, Color.HotPink);

    public static Brush IndianRed => GetBrush(s_indianRedKey, Color.IndianRed);
    public static Brush Indigo => GetBrush(s_indigoKey, Color.Indigo);
    public static Brush Ivory => GetBrush(s_ivoryKey, Color.Ivory);

    public static Brush Khaki => GetBrush(s_khakiKey, Color.Khaki);

    public static Brush Lavender => GetBrush(s_lavenderKey, Color.Lavender);
    public static Brush LavenderBlush => GetBrush(s_lavenderBlushKey, Color.LavenderBlush);
    public static Brush LawnGreen => GetBrush(s_lawnGreenKey, Color.LawnGreen);
    public static Brush LemonChiffon => GetBrush(s_lemonChiffonKey, Color.LemonChiffon);
    public static Brush LightBlue => GetBrush(s_lightBlueKey, Color.LightBlue);
    public static Brush LightCoral => GetBrush(s_lightCoralKey, Color.LightCoral);
    public static Brush LightCyan => GetBrush(s_lightCyanKey, Color.LightCyan);
    public static Brush LightGoldenrodYellow => GetBrush(s_lightGoldenrodYellowKey, Color.LightGoldenrodYellow);
    public static Brush LightGreen => GetBrush(s_lightGreenKey, Color.LightGreen);
    public static Brush LightGray => GetBrush(s_lightGrayKey, Color.LightGray);
    public static Brush LightPink => GetBrush(s_lightPinkKey, Color.LightPink);
    public static Brush LightSalmon => GetBrush(s_lightSalmonKey, Color.LightSalmon);
    public static Brush LightSeaGreen => GetBrush(s_lightSeaGreenKey, Color.LightSeaGreen);
    public static Brush LightSkyBlue => GetBrush(s_lightSkyBlueKey, Color.LightSkyBlue);
    public static Brush LightSlateGray => GetBrush(s_lightSlateGrayKey, Color.LightSlateGray);
    public static Brush LightSteelBlue => GetBrush(s_lightSteelBlueKey, Color.LightSteelBlue);
    public static Brush LightYellow => GetBrush(s_lightYellowKey, Color.LightYellow);
    public static Brush Lime => GetBrush(s_limeKey, Color.Lime);
    public static Brush LimeGreen => GetBrush(s_limeGreenKey, Color.LimeGreen);
    public static Brush Linen => GetBrush(s_linenKey, Color.Linen);

    public static Brush Magenta => GetBrush(s_magentaKey, Color.Magenta);
    public static Brush Maroon => GetBrush(s_maroonKey, Color.Maroon);
    public static Brush MediumAquamarine => GetBrush(s_mediumAquamarineKey, Color.MediumAquamarine);
    public static Brush MediumBlue => GetBrush(s_mediumBlueKey, Color.MediumBlue);
    public static Brush MediumOrchid => GetBrush(s_mediumOrchidKey, Color.MediumOrchid);
    public static Brush MediumPurple => GetBrush(s_mediumPurpleKey, Color.MediumPurple);
    public static Brush MediumSeaGreen => GetBrush(s_mediumSeaGreenKey, Color.MediumSeaGreen);
    public static Brush MediumSlateBlue => GetBrush(s_mediumSlateBlueKey, Color.MediumSlateBlue);
    public static Brush MediumSpringGreen => GetBrush(s_mediumSpringGreenKey, Color.MediumSpringGreen);
    public static Brush MediumTurquoise => GetBrush(s_mediumTurquoiseKey, Color.MediumTurquoise);
    public static Brush MediumVioletRed => GetBrush(s_mediumVioletRedKey, Color.MediumVioletRed);
    public static Brush MidnightBlue => GetBrush(s_midnightBlueKey, Color.MidnightBlue);
    public static Brush MintCream => GetBrush(s_mintCreamKey, Color.MintCream);
    public static Brush MistyRose => GetBrush(s_mistyRoseKey, Color.MistyRose);
    public static Brush Moccasin => GetBrush(s_moccasinKey, Color.Moccasin);

    public static Brush NavajoWhite => GetBrush(s_navajoWhiteKey, Color.NavajoWhite);
    public static Brush Navy => GetBrush(s_navyKey, Color.Navy);

    public static Brush OldLace => GetBrush(s_oldLaceKey, Color.OldLace);
    public static Brush Olive => GetBrush(s_oliveKey, Color.Olive);
    public static Brush OliveDrab => GetBrush(s_oliveDrabKey, Color.OliveDrab);
    public static Brush Orange => GetBrush(s_orangeKey, Color.Orange);
    public static Brush OrangeRed => GetBrush(s_orangeRedKey, Color.OrangeRed);
    public static Brush Orchid => GetBrush(s_orchidKey, Color.Orchid);

    public static Brush PaleGoldenrod => GetBrush(s_paleGoldenrodKey, Color.PaleGoldenrod);
    public static Brush PaleGreen => GetBrush(s_paleGreenKey, Color.PaleGreen);
    public static Brush PaleTurquoise => GetBrush(s_paleTurquoiseKey, Color.PaleTurquoise);
    public static Brush PaleVioletRed => GetBrush(s_paleVioletRedKey, Color.PaleVioletRed);
    public static Brush PapayaWhip => GetBrush(s_papayaWhipKey, Color.PapayaWhip);
    public static Brush PeachPuff => GetBrush(s_peachPuffKey, Color.PeachPuff);
    public static Brush Peru => GetBrush(s_peruKey, Color.Peru);
    public static Brush Pink => GetBrush(s_pinkKey, Color.Pink);
    public static Brush Plum => GetBrush(s_plumKey, Color.Plum);
    public static Brush PowderBlue => GetBrush(s_powderBlueKey, Color.PowderBlue);
    public static Brush Purple => GetBrush(s_purpleKey, Color.Purple);

    public static Brush Red => GetBrush(s_redKey, Color.Red);
    public static Brush RosyBrown => GetBrush(s_rosyBrownKey, Color.RosyBrown);
    public static Brush RoyalBlue => GetBrush(s_royalBlueKey, Color.RoyalBlue);

    public static Brush SaddleBrown => GetBrush(s_saddleBrownKey, Color.SaddleBrown);
    public static Brush Salmon => GetBrush(s_salmonKey, Color.Salmon);
    public static Brush SandyBrown => GetBrush(s_sandyBrownKey, Color.SandyBrown);
    public static Brush SeaGreen => GetBrush(s_seaGreenKey, Color.SeaGreen);
    public static Brush SeaShell => GetBrush(s_seaShellKey, Color.SeaShell);
    public static Brush Sienna => GetBrush(s_siennaKey, Color.Sienna);
    public static Brush Silver => GetBrush(s_silverKey, Color.Silver);
    public static Brush SkyBlue => GetBrush(s_skyBlueKey, Color.SkyBlue);
    public static Brush SlateBlue => GetBrush(s_slateBlueKey, Color.SlateBlue);
    public static Brush SlateGray => GetBrush(s_slateGrayKey, Color.SlateGray);
    public static Brush Snow => GetBrush(s_snowKey, Color.Snow);
    public static Brush SpringGreen => GetBrush(s_springGreenKey, Color.SpringGreen);
    public static Brush SteelBlue => GetBrush(s_steelBlueKey, Color.SteelBlue);

    public static Brush Tan => GetBrush(s_tanKey, Color.Tan);
    public static Brush Teal => GetBrush(s_tealKey, Color.Teal);
    public static Brush Thistle => GetBrush(s_thistleKey, Color.Thistle);
    public static Brush Tomato => GetBrush(s_tomatoKey, Color.Tomato);
    public static Brush Turquoise => GetBrush(s_turquoiseKey, Color.Turquoise);

    public static Brush Violet => GetBrush(s_violetKey, Color.Violet);

    public static Brush Wheat => GetBrush(s_wheatKey, Color.Wheat);
    public static Brush White => GetBrush(s_whiteKey, Color.White);
    public static Brush WhiteSmoke => GetBrush(s_whiteSmokeKey, Color.WhiteSmoke);

    public static Brush Yellow => GetBrush(s_yellowKey, Color.Yellow);
    public static Brush YellowGreen => GetBrush(s_yellowGreenKey, Color.YellowGreen);

    private static Brush GetBrush(object key, Color color)
    {
        if (Gdip.ThreadData.TryGetValue(key, out object? objectBrush) && objectBrush is Brush brush)
        {
            return brush;
        }

        Brush newBrush = new SolidBrush(color);
        Gdip.ThreadData[key] = newBrush;
        return newBrush;
    }
}

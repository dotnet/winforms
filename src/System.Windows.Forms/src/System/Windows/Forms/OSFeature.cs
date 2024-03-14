// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Provides operating-system specific feature queries.
/// </summary>
public class OSFeature : FeatureSupport
{
    /// <summary>
    ///  Represents the layered, top-level windows feature. This <see langword="static"/> field
    ///  is read-only.
    /// </summary>
    public static readonly object LayeredWindows = new();

    /// <summary>
    ///  Determines if the OS supports themes
    /// </summary>
    public static readonly object Themes = new();

    private static OSFeature? s_feature;

    /// <summary>
    ///  Initializes a new instance of the <see cref="OSFeature"/> class.
    /// </summary>
    protected OSFeature()
    {
    }

    /// <summary>
    ///  Represents the <see langword="static"/> instance of <see cref="OSFeature"/>
    ///  to use for feature queries. This property is read-only.
    /// </summary>
    public static OSFeature Feature => s_feature ??= new OSFeature();

    /// <summary>
    ///  Retrieves the version of the specified feature currently available on the system.
    /// </summary>
    public override Version? GetVersionPresent(object feature)
    {
        // These are always supported on platforms that .NET Core supports.
        if (feature == LayeredWindows || feature == Themes)
        {
            return new Version(0, 0, 0, 0);
        }

        return null;
    }

    /// <summary>
    ///  Retrieves whether SystemParameterType is supported on the Current OS version.
    /// </summary>
    public static bool IsPresent(SystemParameter enumVal) => enumVal switch
    {
        SystemParameter.DropShadow
            or SystemParameter.FlatMenu
            or SystemParameter.FontSmoothingContrastMetric
            or SystemParameter.FontSmoothingTypeMetric
            or SystemParameter.MenuFadeEnabled
            or SystemParameter.SelectionFade
            or SystemParameter.ToolTipAnimationMetric
            or SystemParameter.UIEffects
            or SystemParameter.CaretWidthMetric
            or SystemParameter.VerticalFocusThicknessMetric
            or SystemParameter.HorizontalFocusThicknessMetric => true,
        _ => false,
    };
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[AttributeUsage(AttributeTargets.All)]
internal sealed class SRCategoryAttribute : CategoryAttribute
{
    private static volatile SRCategoryAttribute? s_defaultAttribute;
    public SRCategoryAttribute(string category) : base(category)
    {
    }

    protected override string? GetLocalizedString(string value)
    {
        return SR.GetResourceString(value);
    }

    public static string GetLocalizedCategory(string category) => category switch
    {
        "Accessibility" => SR.CatAccessibility,
        "Action" => SR.CatAction,
        "Appearance" => SR.CatAppearance,
        "Asynchronous" => SR.CatAsynchronous,
        "Behavior" => SR.PropertyCategoryBehavior,
        //"Config" => SR.PropertyCategoryConfig,
        "Data" => SR.PropertyCategoryData,
        "DDE" => SR.PropertyCategoryDDE,
        "Misc" => SR.PropertyCategoryMisc,
        "Default" => SR.PropertyCategoryMisc,
        //"Design" => SR.PropertyCategoryDesign,
        "DragDrop" => SR.CatDragDrop,
        "Focus" => SR.CatFocus,
        "Font" => SR.PropertyCategoryFont,
        //"Format" => SR.PropertyCategoryFormat,
        "Key" => SR.CatKey,
        "Layout" => SR.CatLayout,
        "List" => SR.PropertyCategoryList,
        "Mouse" => SR.CatMouse,
        "Position" => SR.PropertyCategoryPosition,
        "Scale" => SR.PropertyCategoryScale,
        "Text" => SR.PropertyCategoryText,
        "WindowStyle" => SR.CatWindowStyle,
        _ => category
    };

    public static new SRCategoryAttribute Default
    {
        get => s_defaultAttribute ??= new SRCategoryAttribute("PropertyCategoryMisc");
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

/// <summary>
///  Browsing handler for <see cref="ICategorizeProperties"/>.
/// </summary>
internal sealed class Com2ICategorizePropertiesHandler : Com2ExtendedBrowsingHandler<ICategorizeProperties>
{
    private static unsafe string? GetCategoryFromObject(object? @object, int dispid)
    {
        using var categorizeProperties = TryGetComScope(@object, out HRESULT hr);
        if (hr.Failed)
        {
            return null;
        }

        PROPCAT categoryId;
        if (categorizeProperties.Value->MapPropertyToCategory(dispid, &categoryId).Failed)
        {
            return null;
        }

        string? knownCategoryName = categoryId switch
        {
            PROPCAT.Nil => string.Empty,
            PROPCAT.Misc => SR.PropertyCategoryMisc,
            PROPCAT.Font => SR.PropertyCategoryFont,
            PROPCAT.Position => SR.PropertyCategoryPosition,
            PROPCAT.Appearance => SR.PropertyCategoryAppearance,
            PROPCAT.Behavior => SR.PropertyCategoryBehavior,
            PROPCAT.Data => SR.PropertyCategoryData,
            PROPCAT.List => SR.PropertyCategoryList,
            PROPCAT.Text => SR.PropertyCategoryText,
            PROPCAT.Scale => SR.PropertyCategoryScale,
            PROPCAT.DDE => SR.PropertyCategoryDDE,
            _ => null,
        };

        if (knownCategoryName is not null)
        {
            return knownCategoryName;
        }

        using BSTR categoryName = default;
        return categorizeProperties.Value->GetCategoryName(categoryId, (int)PInvokeCore.GetThreadLocale(), &categoryName).Succeeded
            ? categoryName.ToString()
            : null;
    }

    public override void RegisterEvents(Com2PropertyDescriptor[]? properties)
    {
        if (properties is null)
        {
            return;
        }

        for (int i = 0; i < properties.Length; i++)
        {
            properties[i].QueryGetBaseAttributes += OnGetAttributes;
        }
    }

    private void OnGetAttributes(Com2PropertyDescriptor sender, GetAttributesEvent e)
    {
        if (GetCategoryFromObject(sender.TargetObject, sender.DISPID) is string category && category.Length > 0)
        {
            e.Add(new CategoryAttribute(category));
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using Windows.Win32.System.Com;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

/// <summary>
///  Browsing handler for <see cref="IVsPerPropertyBrowsing"/>.
/// </summary>
[RequiresUnreferencedCode(ComNativeDescriptor.ComTypeDescriptorsMessage + " Uses Com2IDispatchConverter which is not trim-compatible.")]
internal sealed unsafe class Com2IVsPerPropertyBrowsingHandler : Com2ExtendedBrowsingHandler<IVsPerPropertyBrowsing>
{
    public static unsafe bool AllowChildProperties(Com2PropertyDescriptor property)
    {
        using var browsing = TryGetComScope(property.TargetObject, out HRESULT hr);
        if (hr.Succeeded)
        {
            BOOL hide = false;
            hr = browsing.Value->DisplayChildProperties(property.DISPID, &hide);
            if (hr == HRESULT.S_OK)
            {
                return hide;
            }
        }

        return false;
    }

    public override void RegisterEvents(Com2PropertyDescriptor[]? properties)
    {
        if (properties is null)
        {
            return;
        }

        for (int i = 0; i < properties.Length; i++)
        {
            properties[i].QueryGetDynamicAttributes += OnGetDynamicAttributes;
            properties[i].QueryGetBaseAttributes += OnGetBaseAttributes;
            properties[i].QueryGetDisplayName += OnGetDisplayName;
            properties[i].QueryGetIsReadOnly += OnGetIsReadOnly;

            properties[i].QueryShouldSerializeValue += OnShouldSerializeValue;
            properties[i].QueryCanResetValue += OnCanResetPropertyValue;
            properties[i].QueryResetValue += OnResetPropertyValue;

            properties[i].QueryGetTypeConverterAndTypeEditor += OnGetTypeConverterAndTypeEditor;
        }
    }

    private void OnGetBaseAttributes(Com2PropertyDescriptor sender, GetAttributesEvent e)
    {
        using var propertyBrowsing = TryGetComScope(sender.TargetObject, out HRESULT hr);
        if (hr.Failed)
        {
            Debug.Assert(sender.TargetObject is null);
            return;
        }

        using BSTR helpString = default;
        hr = propertyBrowsing.Value->GetLocalizedPropertyInfo(sender.DISPID, PInvokeCore.GetThreadLocale(), null, &helpString);
        if (hr == HRESULT.S_OK && !helpString.IsNull)
        {
            e.Add(new DescriptionAttribute(helpString.ToString()));
        }
    }

    private unsafe void OnGetDynamicAttributes(Com2PropertyDescriptor sender, GetAttributesEvent e)
    {
        using var propertyBrowsing = TryGetComScope(sender.TargetObject, out HRESULT hr);
        if (hr.Failed)
        {
            Debug.Assert(sender.TargetObject is null);
            return;
        }

        hr = HRESULT.S_OK;

        // We want to avoid allowing clients to force a bad property to be browsable so we don't allow things
        // that are marked as non browsable to become browsable, only the other way around.
        if (sender.CanShow)
        {
            // Should we hide this?
            BOOL hide = sender.Attributes[typeof(BrowsableAttribute)] is Attribute browsableAttribute
                && browsableAttribute.Equals(BrowsableAttribute.No);
            hr = propertyBrowsing.Value->HideProperty(sender.DISPID, &hide);
            if (hr == HRESULT.S_OK)
            {
                e.Add(hide ? BrowsableAttribute.No : BrowsableAttribute.Yes);
            }
        }

        // Should we show this?
        if (typeof(IDispatch.Interface).IsAssignableFrom(sender.PropertyType) && sender.CanShow)
        {
            BOOL display = false;
            hr = propertyBrowsing.Value->DisplayChildProperties(sender.DISPID, &display);
            if (hr == HRESULT.S_OK && display)
            {
                e.Add(BrowsableAttribute.Yes);
            }
        }
    }

    private unsafe void OnCanResetPropertyValue(Com2PropertyDescriptor sender, GetBoolValueEvent e)
    {
        using var propertyBrowsing = TryGetComScope(sender.TargetObject, out HRESULT hr);
        if (hr.Failed)
        {
            Debug.Assert(sender.TargetObject is null);
            return;
        }

        BOOL canReset = e.Value;
        hr = propertyBrowsing.Value->CanResetPropertyValue(sender.DISPID, &canReset);
        if (hr.Succeeded)
        {
            e.Value = canReset;
        }
    }

    private void OnGetDisplayName(Com2PropertyDescriptor sender, GetNameItemEvent e)
    {
        using var propertyBrowsing = TryGetComScope(sender.TargetObject, out HRESULT hr);
        if (hr.Failed)
        {
            Debug.Assert(sender.TargetObject is null);
            return;
        }

        // Get the localized name, if applicable.
        using BSTR name = default;
        hr = propertyBrowsing.Value->GetLocalizedPropertyInfo(sender.DISPID, PInvokeCore.GetThreadLocale(), &name, null);
        if (hr == HRESULT.S_OK && !name.IsNull)
        {
            e.Name = name.ToString();
        }
    }

    private unsafe void OnGetIsReadOnly(Com2PropertyDescriptor sender, GetBoolValueEvent e)
    {
        using var propertyBrowsing = TryGetComScope(sender.TargetObject, out HRESULT hr);
        if (hr.Failed)
        {
            Debug.Assert(sender.TargetObject is null);
            return;
        }

        // Should we make this read only?
        BOOL readOnly = false;
        hr = propertyBrowsing.Value->IsPropertyReadOnly(sender.DISPID, &readOnly);
        if (hr == HRESULT.S_OK)
        {
            e.Value = readOnly;
        }
    }

    private unsafe void OnGetTypeConverterAndTypeEditor(Com2PropertyDescriptor sender, GetTypeConverterAndTypeEditorEvent e)
    {
        using var propertyBrowsing = TryGetComScope(sender.TargetObject, out HRESULT hr);
        if (hr.Failed)
        {
            Debug.Assert(sender.TargetObject is null);
            return;
        }

        // We only do this for IDispatch types.
        if (sender.CanShow && typeof(IDispatch.Interface).IsAssignableFrom(sender.PropertyType))
        {
            // Should we make this read only?
            BOOL result;
            hr = propertyBrowsing.Value->DisplayChildProperties(sender.DISPID, &result);
            e.TypeConverter = e.TypeConverter is Com2IDispatchConverter
                ? new Com2IDispatchConverter(sender, hr == HRESULT.S_OK && result)
                : new Com2IDispatchConverter(hr == HRESULT.S_OK && result, e.TypeConverter);
        }
    }

    private unsafe void OnResetPropertyValue(Com2PropertyDescriptor sender, EventArgs e)
    {
        using var propertyBrowsing = TryGetComScope(sender.TargetObject, out HRESULT hr);
        if (hr.Failed)
        {
            Debug.Assert(sender.TargetObject is null);
            return;
        }

        int dispid = sender.DISPID;
        BOOL canReset = false;
        hr = propertyBrowsing.Value->CanResetPropertyValue(dispid, &canReset);
        if (hr.Succeeded)
        {
            propertyBrowsing.Value->ResetPropertyValue(dispid);
        }
    }

    private unsafe void OnShouldSerializeValue(Com2PropertyDescriptor sender, GetBoolValueEvent e)
    {
        using var propertyBrowsing = TryGetComScope(sender.TargetObject, out HRESULT hr);
        if (hr.Failed)
        {
            Debug.Assert(sender.TargetObject is null);
            return;
        }

        // By default we say it's default.
        BOOL isDefault = true;
        hr = propertyBrowsing.Value->HasDefaultValue(sender.DISPID, &isDefault);
        if (hr == HRESULT.S_OK && !isDefault)
        {
            // Specify a default value editor.
            e.Value = true;
        }
    }
}

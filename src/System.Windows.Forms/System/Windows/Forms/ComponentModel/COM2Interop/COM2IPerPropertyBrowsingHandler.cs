// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

/// <summary>
///  Browsing handler for <see cref="IPerPropertyBrowsing"/>.
/// </summary>
[RequiresUnreferencedCode(ComNativeDescriptor.ComTypeDescriptorsMessage + " Uses COM2PropertyDescriptor which is not trim-compatible.")]
internal sealed unsafe partial class Com2IPerPropertyBrowsingHandler : Com2ExtendedBrowsingHandler<IPerPropertyBrowsing>
{
    public override void RegisterEvents(Com2PropertyDescriptor[]? properties)
    {
        if (properties is null)
        {
            return;
        }

        for (int i = 0; i < properties.Length; i++)
        {
            properties[i].QueryGetBaseAttributes += OnGetBaseAttributes;
            properties[i].QueryGetDisplayValue += OnGetDisplayValue;
            properties[i].QueryGetTypeConverterAndTypeEditor += OnGetTypeConverterAndTypeEditor;
        }
    }

    private static Guid GetPropertyPageGuid(IPerPropertyBrowsing* propertyBrowsing, int dispid)
    {
        // Check for a property page
        Guid guid = Guid.Empty;
        HRESULT hr = propertyBrowsing->MapPropertyToPage(dispid, &guid);
        return hr.Succeeded ? guid : Guid.Empty;
    }

    internal static bool TryGetDisplayString(
        IPerPropertyBrowsing* propertyBrowsing,
        int dispid,
        [NotNullWhen(true)] out string? displayString)
    {
        using BSTR value = default;
        if (propertyBrowsing->GetDisplayString(dispid, &value).Failed)
        {
            displayString = null;
            return false;
        }

        displayString = value.ToString();
        return displayString is not null;
    }

    /// <summary>
    ///  Here is where we handle IVsPerPropertyBrowsing.GetLocalizedPropertyInfo and IVsPerPropertyBrowsing. We
    ///  hide properties such as IPerPropertyBrowsing, IProvidePropertyBuilder, etc.
    /// </summary>
    private void OnGetBaseAttributes(Com2PropertyDescriptor sender, GetAttributesEvent e)
    {
        using var propertyBrowsing = TryGetComScope(sender.TargetObject, out HRESULT hr);
        if (hr.Succeeded)
        {
            // We hide IDispatch props by default, we we need to force showing them here.

            bool validPropPage = !Guid.Empty.Equals(GetPropertyPageGuid(propertyBrowsing, sender.DISPID));

            if (sender.CanShow && validPropPage)
            {
                e.Add(BrowsableAttribute.Yes);
            }
        }
    }

    private void OnGetDisplayValue(Com2PropertyDescriptor sender, GetNameItemEvent e)
    {
        using var propertyBrowsing = TryGetComScope(sender.TargetObject, out HRESULT hr);
        if (hr.Failed)
        {
            return;
        }

        // If we are using the dropdown, don't convert the value or the values will change when we select them
        // and call back for the display value.
        if (sender.Converter is Com2IPerPropertyEnumConverter || sender.ConvertingNativeType)
        {
            return;
        }

        if (TryGetDisplayString(propertyBrowsing, sender.DISPID, out string? displayString))
        {
            e.Name = displayString;
        }
    }

    private unsafe void OnGetTypeConverterAndTypeEditor(Com2PropertyDescriptor sender, GetTypeConverterAndTypeEditorEvent e)
    {
        using var propertyBrowsing = TryGetComScope(sender.TargetObject, out HRESULT hr);
        if (hr.Failed)
        {
            return;
        }

        // Check for enums.
        CALPOLESTR caStrings = default;
        CADWORD caCookies = default;

        hr = propertyBrowsing.Value->GetPredefinedStrings(sender.DISPID, &caStrings, &caCookies);

        if (hr.Failed)
        {
            Debug.Fail($"IPerPropertyBrowsing::GetPredefinedStrings(dispid={sender.DISPID}) failed.");
        }

        // Terminate the existing editor if we created the current one so if the items have disappeared,
        // we don't hold onto the old items.
        if (e.TypeConverter is Com2IPerPropertyEnumConverter)
        {
            e.TypeConverter = null;
        }

        if (hr == HRESULT.S_OK)
        {
            string?[] names = caStrings.ConvertAndFree();
            uint[] cookies = caCookies.ConvertAndFree();

            if (names.Length > 0 && cookies.Length > 0)
            {
                e.TypeConverter = new Com2IPerPropertyEnumConverter(new Com2IPerPropertyBrowsingEnum(sender, names, cookies));
            }
        }
        else
        {
            // If we didn't get any strings, try the property page editor.
            //
            // This is a bit of a backwards-compat work around. Many older ActiveX controls will show a
            // property page for all properties since the old grid would only put up the [...] button for
            // "(Custom)". If we have a conversion editor, don't allow this to override it.

            if (sender.ConvertingNativeType)
            {
                return;
            }

            Guid guid = GetPropertyPageGuid(propertyBrowsing, sender.DISPID);

            if (!Guid.Empty.Equals(guid))
            {
                e.TypeEditor = new Com2PropertyPageUITypeEditor(sender, guid, (UITypeEditor?)e.TypeEditor);
            }
        }
    }
}

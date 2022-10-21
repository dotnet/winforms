// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using Windows.Win32.System.Ole;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal partial class Com2IPerPropertyBrowsingHandler : Com2ExtendedBrowsingHandler
    {
        public override Type Interface => typeof(IPerPropertyBrowsing.Interface);

        public override void SetupPropertyHandlers(Com2PropertyDescriptor[]? propDesc)
        {
            if (propDesc is null)
            {
                return;
            }

            for (int i = 0; i < propDesc.Length; i++)
            {
                propDesc[i].QueryGetBaseAttributes += new GetAttributesEventHandler(OnGetBaseAttributes);
                propDesc[i].QueryGetDisplayValue += new GetNameItemEventHandler(OnGetDisplayValue);

                propDesc[i].QueryGetTypeConverterAndTypeEditor += new GetTypeConverterAndTypeEditorEventHandler(OnGetTypeConverterAndTypeEditor);
            }
        }

        private static unsafe Guid GetPropertyPageGuid(IPerPropertyBrowsing.Interface target, Ole32.DispatchID dispid)
        {
            // Check for a property page
            Guid guid = Guid.Empty;
            HRESULT hr = target.MapPropertyToPage((int)dispid, &guid);
            return hr.Succeeded ? guid : Guid.Empty;
        }

        internal static unsafe string? GetDisplayString(IPerPropertyBrowsing.Interface ppb, Ole32.DispatchID dispid, ref bool success)
        {
            using BSTR strVal = default;
            HRESULT hr = ppb.GetDisplayString((int)dispid, &strVal);
            if (hr != HRESULT.S_OK)
            {
                success = false;
                return null;
            }

            success = strVal.Value is not null;
            return strVal.ToString();
        }

        /// <summary>
        ///  Here is where we handle IVsPerPropertyBrowsing.GetLocalizedPropertyInfo and IVsPerPropertyBrowsing. We
        ///  hide properties such as IPerPropertyBrowsing, IProvidePropertyBuilder, etc.
        /// </summary>
        private void OnGetBaseAttributes(Com2PropertyDescriptor sender, GetAttributesEvent attrEvent)
        {
            if (sender.TargetObject is IPerPropertyBrowsing.Interface target)
            {
                // We hide IDispatch props by default, we we need to force showing them here.

                bool validPropPage = !Guid.Empty.Equals(GetPropertyPageGuid(target, sender.DISPID));

                if (sender.CanShow && validPropPage)
                {
                    if (typeof(Oleaut32.IDispatch).IsAssignableFrom(sender.PropertyType))
                    {
                        attrEvent.Add(BrowsableAttribute.Yes);
                    }
                }
            }
        }

        private void OnGetDisplayValue(Com2PropertyDescriptor sender, GetNameItemEvent gnievent)
        {
            try
            {
                if (sender.TargetObject is not IPerPropertyBrowsing.Interface browsing)
                {
                    return;
                }

                // If we are using the dropdown, don't convert the value or the values will change when we select them
                // and call back for the display value.
                if (sender.Converter is Com2IPerPropertyEnumConverter || sender.ConvertingNativeType)
                {
                    return;
                }

                bool success = true;

                string? displayString = GetDisplayString(browsing, sender.DISPID, ref success);

                if (success)
                {
                    gnievent.Name = displayString;
                }
            }
            catch
            {
            }
        }

        private unsafe void OnGetTypeConverterAndTypeEditor(Com2PropertyDescriptor sender, GetTypeConverterAndTypeEditorEvent gveevent)
        {
            if (sender.TargetObject is not IPerPropertyBrowsing.Interface ppb)
            {
                return;
            }

            // Check for enums.
            CALPOLESTR caStrings = default;
            CADWORD caCookies = default;

            HRESULT hr;
            try
            {
                hr = ppb.GetPredefinedStrings((int)sender.DISPID, &caStrings, &caCookies);
            }
            catch (ExternalException ex)
            {
                hr = (HRESULT)ex.ErrorCode;
                Debug.Fail($"An exception occurred inside IPerPropertyBrowsing::GetPredefinedStrings(dispid={sender.DISPID}), object type={ComNativeDescriptor.GetClassName(ppb)}");
            }

            // Terminate the existing editor if we created the current one so if the items have disappeared,
            // we don't hold onto the old items.
            if (gveevent.TypeConverter is Com2IPerPropertyEnumConverter)
            {
                gveevent.TypeConverter = null;
            }

            if (hr == HRESULT.S_OK)
            {
                string?[] names = caStrings.ConvertAndFree();
                uint[] cookies = caCookies.ConvertAndFree();

                if (names.Length > 0 && cookies.Length > 0)
                {
                    gveevent.TypeConverter = new Com2IPerPropertyEnumConverter(new Com2IPerPropertyBrowsingEnum(sender, names, cookies));
                }
            }
            else
            {
                // If we didn't get any strings, try the proppage editor
                //
                // This is a bit of a backwards-compat work around. Many older ActiveX controls will show a
                // property page for all properties since the old grid would only put up the [...] button for
                // "(Custom)".  If we have a conversion editor, don't allow this to override it.

                if (sender.ConvertingNativeType)
                {
                    return;
                }

                Guid guid = GetPropertyPageGuid(ppb, sender.DISPID);

                if (!Guid.Empty.Equals(guid))
                {
                    gveevent.TypeEditor = new Com2PropertyPageUITypeEditor(sender, guid, (UITypeEditor)gveevent.TypeEditor);
                }
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal class Com2IVsPerPropertyBrowsingHandler : Com2ExtendedBrowsingHandler
    {
        public override Type Interface => typeof(VSSDK.IVsPerPropertyBrowsing);

        public static unsafe bool AllowChildProperties(Com2PropertyDescriptor propDesc)
        {
            if (propDesc.TargetObject is VSSDK.IVsPerPropertyBrowsing browsing)
            {
                BOOL pfHide = false;
                HRESULT hr = browsing.DisplayChildProperties(propDesc.DISPID, &pfHide);
                if (hr == HRESULT.S_OK)
                {
                    return pfHide;
                }
            }

            return false;
        }

        public override void SetupPropertyHandlers(Com2PropertyDescriptor[]? propDesc)
        {
            if (propDesc is null)
            {
                return;
            }

            for (int i = 0; i < propDesc.Length; i++)
            {
                propDesc[i].QueryGetDynamicAttributes += OnGetDynamicAttributes;
                propDesc[i].QueryGetBaseAttributes += OnGetBaseAttributes;
                propDesc[i].QueryGetDisplayName += OnGetDisplayName;
                propDesc[i].QueryGetIsReadOnly += OnGetIsReadOnly;

                propDesc[i].QueryShouldSerializeValue += OnShouldSerializeValue;
                propDesc[i].QueryCanResetValue += OnCanResetPropertyValue;
                propDesc[i].QueryResetValue += OnResetPropertyValue;

                propDesc[i].QueryGetTypeConverterAndTypeEditor += OnGetTypeConverterAndTypeEditor;
            }
        }

        private void OnGetBaseAttributes(Com2PropertyDescriptor sender, GetAttributesEvent attrEvent)
        {
            if (sender.TargetObject is not VSSDK.IVsPerPropertyBrowsing vsObj)
            {
                return;
            }

            // Should we localize this?
            string[] pHelpString = new string[1];
            HRESULT hr = vsObj.GetLocalizedPropertyInfo(sender.DISPID, PInvoke.GetThreadLocale(), null, pHelpString);
            if (hr == HRESULT.S_OK && pHelpString[0] is not null)
            {
                attrEvent.Add(new DescriptionAttribute(pHelpString[0]));
            }
        }

        /// <summary>
        ///  Here is where we handle IVsPerPropertyBrowsing.GetLocalizedPropertyInfo and IVsPerPropertyBrowsing. We
        ///  hide properties such as IPerPropertyBrowsing, IProvidePropertyBuilder, etc.
        /// </summary>
        private unsafe void OnGetDynamicAttributes(Com2PropertyDescriptor sender, GetAttributesEvent attrEvent)
        {
            if (sender.TargetObject is VSSDK.IVsPerPropertyBrowsing vsObj)
            {
                HRESULT hr = HRESULT.S_OK;

                // We want to avoid allowing clients to force a bad property to be browsable so we don't allow things
                // that are marked as non browsable to become browsable, only the other way around.
                if (sender.CanShow)
                {
                    // Should we hide this?
                    BOOL hide = sender.Attributes[typeof(BrowsableAttribute)] is Attribute browsableAttribute
                        && browsableAttribute.Equals(BrowsableAttribute.No);
                    hr = vsObj.HideProperty(sender.DISPID, &hide);
                    if (hr == HRESULT.S_OK)
                    {
                        attrEvent.Add(hide ? BrowsableAttribute.No : BrowsableAttribute.Yes);
                    }
                }

                // should we show this
                if (typeof(Oleaut32.IDispatch).IsAssignableFrom(sender.PropertyType) && sender.CanShow)
                {
                    BOOL pfDisplay = false;
                    hr = vsObj.DisplayChildProperties(sender.DISPID, &pfDisplay);
                    if (hr == HRESULT.S_OK && pfDisplay)
                    {
                        attrEvent.Add(BrowsableAttribute.Yes);
                    }
                }
            }

            Debug.Assert(sender.TargetObject is null or VSSDK.IVsPerPropertyBrowsing, $"Object is not {Interface.Name}!");
        }

        private unsafe void OnCanResetPropertyValue(Com2PropertyDescriptor sender, GetBoolValueEvent boolEvent)
        {
            if (sender.TargetObject is VSSDK.IVsPerPropertyBrowsing target)
            {
                BOOL canReset = boolEvent.Value ? true : false;
                HRESULT hr = target.CanResetPropertyValue(sender.DISPID, &canReset);
                if (hr.Succeeded)
                {
                    boolEvent.Value = canReset;
                }
            }

            Debug.Assert(sender.TargetObject is null or VSSDK.IVsPerPropertyBrowsing, $"Object is not {Interface.Name}!");
        }

        /// <summary>
        ///  Here is where we handle IVsPerPropertyBrowsing.GetLocalizedPropertyInfo (part 2).
        /// </summary>
        private void OnGetDisplayName(Com2PropertyDescriptor sender, GetNameItemEvent nameItem)
        {
            if (sender.TargetObject is VSSDK.IVsPerPropertyBrowsing vsObj)
            {
                // Get the localized name, if applicable.
                string[] pNameString = new string[1];
                HRESULT hr = vsObj.GetLocalizedPropertyInfo(sender.DISPID, PInvoke.GetThreadLocale(), pNameString, null);
                if (hr == HRESULT.S_OK && pNameString[0] is not null)
                {
                    nameItem.Name = pNameString[0];
                }
            }

            Debug.Assert(sender.TargetObject is null or VSSDK.IVsPerPropertyBrowsing, $"Object is not {Interface.Name}!");
        }

        /// <summary>
        ///  Here is where we handle IVsPerPropertyBrowsing.IsPropertyReadOnly.
        /// </summary>
        private unsafe void OnGetIsReadOnly(Com2PropertyDescriptor sender, GetBoolValueEvent gbvevent)
        {
            if (sender.TargetObject is VSSDK.IVsPerPropertyBrowsing vsObj)
            {
                // Should we make this read only?
                BOOL pfResult = false;
                HRESULT hr = vsObj.IsPropertyReadOnly(sender.DISPID, &pfResult);
                if (hr == HRESULT.S_OK)
                {
                    gbvevent.Value = pfResult;
                }
            }
        }

        /// <summary>
        ///  Here is where we handle IVsPerPropertyBrowsing.DisplayChildProperties.
        /// </summary>
        private unsafe void OnGetTypeConverterAndTypeEditor(Com2PropertyDescriptor sender, GetTypeConverterAndTypeEditorEvent gveevent)
        {
            if (sender.TargetObject is VSSDK.IVsPerPropertyBrowsing browsing)
            {
                // we only do this for IDispatch types
                if (sender.CanShow && typeof(Oleaut32.IDispatch).IsAssignableFrom(sender.PropertyType))
                {
                    VSSDK.IVsPerPropertyBrowsing vsObj = browsing;

                    // Should we make this read only?
                    BOOL pfResult = false;
                    HRESULT hr = vsObj.DisplayChildProperties(sender.DISPID, &pfResult);
                    if (gveevent.TypeConverter is Com2IDispatchConverter)
                    {
                        gveevent.TypeConverter = new Com2IDispatchConverter(sender, hr == HRESULT.S_OK && pfResult);
                    }
                    else
                    {
                        gveevent.TypeConverter = new Com2IDispatchConverter(hr == HRESULT.S_OK && pfResult, gveevent.TypeConverter);
                    }
                }
            }

            Debug.Assert(sender.TargetObject is null or VSSDK.IVsPerPropertyBrowsing, $"Object is not {Interface.Name}!");
        }

        private unsafe void OnResetPropertyValue(Com2PropertyDescriptor sender, EventArgs e)
        {
            if (sender.TargetObject is VSSDK.IVsPerPropertyBrowsing target)
            {
                Ole32.DispatchID dispid = sender.DISPID;
                BOOL canReset = false;
                HRESULT hr = target.CanResetPropertyValue(dispid, &canReset);
                if (hr.Succeeded)
                {
                    target.ResetPropertyValue(dispid);
                }
            }

            Debug.Assert(sender.TargetObject is null or VSSDK.IVsPerPropertyBrowsing, $"Object is not {Interface.Name}!");
        }

        private unsafe void OnShouldSerializeValue(Com2PropertyDescriptor sender, GetBoolValueEvent gbvevent)
        {
            if (sender.TargetObject is VSSDK.IVsPerPropertyBrowsing vsObj)
            {
                // By default we say it's default.
                BOOL pfResult = true;
                HRESULT hr = vsObj.HasDefaultValue(sender.DISPID, &pfResult);
                if (hr == HRESULT.S_OK && !pfResult)
                {
                    // Specify a default value editor.
                    gbvevent.Value = true;
                }
            }

            Debug.Assert(sender.TargetObject is null or VSSDK.IVsPerPropertyBrowsing, $"Object is not {Interface.Name}!");
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This is the base class for handlers for Com2 extended browsing interface
    ///  such as IPerPropertyBrowsing, etc.
    ///
    ///  These handlers should be stateless.  That is, they should keep no refs to object
    ///  and should only work on a give object and dispid.  That way all objects that
    ///  support a give interface can share a handler.
    ///
    ///  See Com2Properties for the array of handler classes to interface classes
    ///  where handlers should be registered.
    /// </summary>
    internal class Com2IVsPerPropertyBrowsingHandler : Com2ExtendedBrowsingHandler
    {
        /// <summary>
        ///  The interface that this handler managers
        ///  such as IPerPropertyBrowsing, IProvidePropertyBuilder, etc.
        /// </summary>
        public override Type Interface => typeof(VSSDK.IVsPerPropertyBrowsing);

        public unsafe static bool AllowChildProperties(Com2PropertyDescriptor propDesc)
        {
            if (propDesc.TargetObject is VSSDK.IVsPerPropertyBrowsing)
            {
                BOOL pfHide = BOOL.FALSE;
                HRESULT hr = ((VSSDK.IVsPerPropertyBrowsing)propDesc.TargetObject).DisplayChildProperties(propDesc.DISPID, &pfHide);
                if (hr == HRESULT.S_OK)
                {
                    return pfHide.IsTrue();
                }
            }

            return false;
        }

        /// <summary>
        ///  Called to setup the property handlers on a given properties
        ///  In this method, the handler will add listeners to the events that
        ///  the Com2PropertyDescriptor surfaces that it cares about.
        /// </summary>
        public override void SetupPropertyHandlers(Com2PropertyDescriptor[] propDesc)
        {
            if (propDesc is null)
            {
                return;
            }
            for (int i = 0; i < propDesc.Length; i++)
            {
                propDesc[i].QueryGetDynamicAttributes += new GetAttributesEventHandler(OnGetDynamicAttributes);
                propDesc[i].QueryGetBaseAttributes += new GetAttributesEventHandler(OnGetBaseAttributes);
                propDesc[i].QueryGetDisplayName += new GetNameItemEventHandler(OnGetDisplayName);
                propDesc[i].QueryGetIsReadOnly += new GetBoolValueEventHandler(OnGetIsReadOnly);

                propDesc[i].QueryShouldSerializeValue += new GetBoolValueEventHandler(OnShouldSerializeValue);
                propDesc[i].QueryCanResetValue += new GetBoolValueEventHandler(OnCanResetPropertyValue);
                propDesc[i].QueryResetValue += new Com2EventHandler(OnResetPropertyValue);

                propDesc[i].QueryGetTypeConverterAndTypeEditor += new GetTypeConverterAndTypeEditorEventHandler(OnGetTypeConverterAndTypeEditor);
            }
        }

        private void OnGetBaseAttributes(Com2PropertyDescriptor sender, GetAttributesEvent attrEvent)
        {
            if (!(sender.TargetObject is VSSDK.IVsPerPropertyBrowsing vsObj))
            {
                return;
            }

            // should we localize this?
            string[] pHelpString = new string[1];
            HRESULT hr = vsObj.GetLocalizedPropertyInfo(sender.DISPID, Kernel32.GetThreadLocale(), null, pHelpString);
            if (hr == HRESULT.S_OK && pHelpString[0] != null)
            {
                attrEvent.Add(new DescriptionAttribute(pHelpString[0]));
            }
        }

        /// <summary>
        ///  Here is where we handle IVsPerPropertyBrowsing.GetLocalizedPropertyInfo and IVsPerPropertyBrowsing.   HideProperty
        ///  such as IPerPropertyBrowsing, IProvidePropertyBuilder, etc.
        /// </summary>
        private unsafe void OnGetDynamicAttributes(Com2PropertyDescriptor sender, GetAttributesEvent attrEvent)
        {
            if (sender.TargetObject is VSSDK.IVsPerPropertyBrowsing vsObj)
            {
                HRESULT hr = HRESULT.S_OK;

                // we want to avoid allowing clients to force a bad property to be browsable,
                // so we don't allow things that are marked as non browsable to become browsable,
                // only the other way around.
                //
                if (sender.CanShow)
                {
                    // should we hide this?
                    BOOL pfHide = sender.Attributes[typeof(BrowsableAttribute)].Equals(BrowsableAttribute.No) ? BOOL.TRUE : BOOL.FALSE;
                    hr = vsObj.HideProperty(sender.DISPID, &pfHide);
                    if (hr == HRESULT.S_OK)
                    {
                        attrEvent.Add(pfHide.IsTrue() ? BrowsableAttribute.No : BrowsableAttribute.Yes);
                    }
                }

                // should we show this
                if (typeof(Oleaut32.IDispatch).IsAssignableFrom(sender.PropertyType) && sender.CanShow)
                {
                    BOOL pfDisplay = BOOL.FALSE;
                    hr = vsObj.DisplayChildProperties(sender.DISPID, &pfDisplay);
                    if (hr == HRESULT.S_OK && pfDisplay.IsTrue())
                    {
                        attrEvent.Add(BrowsableAttribute.Yes);
                    }
                }
            }
            Debug.Assert(sender.TargetObject is null || sender.TargetObject is VSSDK.IVsPerPropertyBrowsing, "Object is not " + Interface.Name + "!");
        }

        private unsafe void OnCanResetPropertyValue(Com2PropertyDescriptor sender, GetBoolValueEvent boolEvent)
        {
            if (sender.TargetObject is VSSDK.IVsPerPropertyBrowsing target)
            {
                BOOL canReset = boolEvent.Value ? BOOL.TRUE : BOOL.FALSE;
                HRESULT hr = target.CanResetPropertyValue(sender.DISPID, &canReset);
                if (hr.Succeeded())
                {
                    boolEvent.Value = canReset.IsTrue();
                }
            }

            Debug.Assert(sender.TargetObject is null || sender.TargetObject is VSSDK.IVsPerPropertyBrowsing, "Object is not " + Interface.Name + "!");
        }

        /// <summary>
        ///  Here is where we handle IVsPerPropertyBrowsing.GetLocalizedPropertyInfo (part 2)
        /// </summary>
        private void OnGetDisplayName(Com2PropertyDescriptor sender, GetNameItemEvent nameItem)
        {
            if (sender.TargetObject is VSSDK.IVsPerPropertyBrowsing vsObj)
            {
                // get the localized name, if applicable
                string[] pNameString = new string[1];
                HRESULT hr = vsObj.GetLocalizedPropertyInfo(sender.DISPID, Kernel32.GetThreadLocale(), pNameString, null);
                if (hr == HRESULT.S_OK && pNameString[0] != null)
                {
                    nameItem.Name = pNameString[0];
                }
            }

            Debug.Assert(sender.TargetObject is null || sender.TargetObject is VSSDK.IVsPerPropertyBrowsing, "Object is not " + Interface.Name + "!");
        }

        /// <summary>
        ///  Here is where we handle IVsPerPropertyBrowsing.IsPropertyReadOnly
        /// </summary>
        private unsafe void OnGetIsReadOnly(Com2PropertyDescriptor sender, GetBoolValueEvent gbvevent)
        {
            if (sender.TargetObject is VSSDK.IVsPerPropertyBrowsing vsObj)
            {
                // should we make this read only?
                BOOL pfResult = BOOL.FALSE;
                HRESULT hr = vsObj.IsPropertyReadOnly(sender.DISPID, &pfResult);
                if (hr == HRESULT.S_OK)
                {
                    gbvevent.Value = pfResult.IsTrue();
                }
            }
        }

        /// <summary>
        ///  Here is where we handle IVsPerPropertyBrowsing.DisplayChildProperties
        /// </summary>
        private unsafe void OnGetTypeConverterAndTypeEditor(Com2PropertyDescriptor sender, GetTypeConverterAndTypeEditorEvent gveevent)
        {
            if (sender.TargetObject is VSSDK.IVsPerPropertyBrowsing)
            {
                // we only do this for IDispatch types
                if (sender.CanShow && typeof(Oleaut32.IDispatch).IsAssignableFrom(sender.PropertyType))
                {
                    VSSDK.IVsPerPropertyBrowsing vsObj = (VSSDK.IVsPerPropertyBrowsing)sender.TargetObject;

                    // should we make this read only?
                    BOOL pfResult = BOOL.FALSE;
                    HRESULT hr = vsObj.DisplayChildProperties(sender.DISPID, &pfResult);
                    if (gveevent.TypeConverter is Com2IDispatchConverter)
                    {
                        gveevent.TypeConverter = new Com2IDispatchConverter(sender, (hr == HRESULT.S_OK && pfResult.IsTrue()));
                    }
                    else
                    {
                        gveevent.TypeConverter = new Com2IDispatchConverter(sender, (hr == HRESULT.S_OK && pfResult.IsTrue()), gveevent.TypeConverter);
                    }
                }
            }
            Debug.Assert(sender.TargetObject is null || sender.TargetObject is VSSDK.IVsPerPropertyBrowsing, "Object is not " + Interface.Name + "!");
        }

        private unsafe void OnResetPropertyValue(Com2PropertyDescriptor sender, EventArgs e)
        {
            if (sender.TargetObject is VSSDK.IVsPerPropertyBrowsing target)
            {
                Ole32.DispatchID dispid = sender.DISPID;
                BOOL canReset = BOOL.FALSE;
                HRESULT hr = target.CanResetPropertyValue(dispid, &canReset);
                if (hr.Succeeded())
                {
                    target.ResetPropertyValue(dispid);
                }
            }

            Debug.Assert(sender.TargetObject is null || sender.TargetObject is VSSDK.IVsPerPropertyBrowsing, "Object is not " + Interface.Name + "!");
        }

        private unsafe void OnShouldSerializeValue(Com2PropertyDescriptor sender, GetBoolValueEvent gbvevent)
        {
            if (sender.TargetObject is VSSDK.IVsPerPropertyBrowsing vsObj)
            {
                // by default we say it's default
                BOOL pfResult = BOOL.TRUE;
                HRESULT hr = vsObj.HasDefaultValue(sender.DISPID, &pfResult);
                if (hr == HRESULT.S_OK && pfResult.IsFalse())
                {
                    // specify a default value editor
                    gbvevent.Value = true;
                }
            }

            Debug.Assert(sender.TargetObject is null || sender.TargetObject is VSSDK.IVsPerPropertyBrowsing, "Object is not " + Interface.Name + "!");
        }
    }
}

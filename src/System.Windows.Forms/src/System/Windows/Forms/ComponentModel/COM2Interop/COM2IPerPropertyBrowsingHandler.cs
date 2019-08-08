// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal class Com2IPerPropertyBrowsingHandler : Com2ExtendedBrowsingHandler
    {
        public override Type Interface
        {
            get
            {
                return typeof(NativeMethods.IPerPropertyBrowsing);
            }
        }

        public override void SetupPropertyHandlers(Com2PropertyDescriptor[] propDesc)
        {
            if (propDesc == null)
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

        private Guid GetPropertyPageGuid(NativeMethods.IPerPropertyBrowsing target, int dispid)
        {
            // check for a property page
            int hr = target.MapPropertyToPage(dispid, out Guid guid);
            if (hr == NativeMethods.S_OK)
            {
                return guid;
            }
            return Guid.Empty;
        }

        internal static string GetDisplayString(NativeMethods.IPerPropertyBrowsing ppb, int dispid, ref bool success)
        {
            string[] strVal = new string[1];
            int hr = ppb.GetDisplayString(dispid, strVal);
            if (hr == NativeMethods.S_OK)
            {
                success = (strVal[0] != null);
                //Debug.Assert(success, "IPerPropertyBrowsing::GetDisplayString returned NULL and S_OK -- this is not a valid state. This component does not property implement IPerPropertyBrowsing. (component class=" + TypeDescriptor.GetClassName(ppb) + ")");
                return strVal[0];
            }
            else
            {
                success = false;
            }
            return null;
        }

        /// <summary>
        ///  Here is where we handle IVsPerPropertyBrowsing.GetLocalizedPropertyInfo and IVsPerPropertyBrowsing.   HideProperty
        ///  such as IPerPropertyBrowsing, IProvidePropertyBuilder, etc.
        /// </summary>
        private void OnGetBaseAttributes(Com2PropertyDescriptor sender, GetAttributesEvent attrEvent)
        {
            if (sender.TargetObject is NativeMethods.IPerPropertyBrowsing target)
            {
                // we hide IDispatch props by default, we we need to force showing them here

                bool validPropPage = !Guid.Empty.Equals(GetPropertyPageGuid(target, sender.DISPID));

                if (sender.CanShow && validPropPage)
                {
                    if (typeof(UnsafeNativeMethods.IDispatch).IsAssignableFrom(sender.PropertyType))
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
                if (sender.TargetObject is NativeMethods.IPerPropertyBrowsing)
                {

                    // if we are using the dropdown, don't convert the value
                    // or the values will change when we select them and call back
                    // for the display value.
                    if (sender.Converter is Com2IPerPropertyEnumConverter || sender.ConvertingNativeType)
                    {
                        return;
                    }

                    bool success = true;

                    string displayString = GetDisplayString((NativeMethods.IPerPropertyBrowsing)sender.TargetObject, sender.DISPID, ref success);

                    if (success)
                    {
                        gnievent.Name = displayString;
                    }
                }
            }
            catch
            {
            }
        }

        private void OnGetTypeConverterAndTypeEditor(Com2PropertyDescriptor sender, GetTypeConverterAndTypeEditorEvent gveevent)
        {
            if (sender.TargetObject is NativeMethods.IPerPropertyBrowsing ppb)
            {
                bool hasStrings = false;

                // check for enums
                NativeMethods.CA_STRUCT caStrings = new NativeMethods.CA_STRUCT();
                NativeMethods.CA_STRUCT caCookies = new NativeMethods.CA_STRUCT();

                int hr = NativeMethods.S_OK;

                try
                {
                    hr = ppb.GetPredefinedStrings(sender.DISPID, caStrings, caCookies);
                }
                catch (ExternalException ex)
                {
                    hr = ex.ErrorCode;
                    Debug.Fail("An exception occurred inside IPerPropertyBrowsing::GetPredefinedStrings(dispid=" + sender.DISPID + "), object type=" + new ComNativeDescriptor().GetClassName(ppb) + ".  This is caused by an exception (usually an AV) inside the object being browsed, and is not a problem in the properties window.");
                }

                // Terminate the existing editor if we created the current one
                // so if the items have disappeared, we don't hold onto the old
                // items.
                if (gveevent.TypeConverter is Com2IPerPropertyEnumConverter)
                {
                    gveevent.TypeConverter = null;
                }

                if (hr != NativeMethods.S_OK)
                {
                    hasStrings = false;
                }
                else
                {
                    hasStrings = true;
                }

                if (hasStrings)
                {

                    OleStrCAMarshaler stringMarshaler = new OleStrCAMarshaler(caStrings);
                    Int32CAMarshaler intMarshaler = new Int32CAMarshaler(caCookies);

                    if (stringMarshaler.Count > 0 && intMarshaler.Count > 0)
                    {
                        gveevent.TypeConverter = new Com2IPerPropertyEnumConverter(new Com2IPerPropertyBrowsingEnum(sender, this, stringMarshaler, intMarshaler, true));
                    }
                    else
                    {
                        //hasStrings = false;
                    }
                }

                // if we didn't get any strings, try the proppage edtior
                //
                if (!hasStrings)
                {

                    // this is a _bit_ of a backwards-compat work around...
                    // many older ActiveX controls will show a property page
                    // for all properties since the old grid would only put up the
                    // [...] button for "(Custom)".  If we have a conversion editor,
                    // don't allow this to override it...
                    //
                    if (sender.ConvertingNativeType)
                    {
                        return;
                    }

                    Guid g = GetPropertyPageGuid(ppb, sender.DISPID);

                    if (!Guid.Empty.Equals(g))
                    {
                        gveevent.TypeEditor = new Com2PropertyPageUITypeEditor(sender, g, (UITypeEditor)gveevent.TypeEditor);
                    }
                }
            }
        }

        // this is just here so we can identify the enums that we added
        private class Com2IPerPropertyEnumConverter : Com2EnumConverter
        {
            private readonly Com2IPerPropertyBrowsingEnum itemsEnum;
            public Com2IPerPropertyEnumConverter(Com2IPerPropertyBrowsingEnum items) : base(items)
            {
                itemsEnum = items;
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
            {

                if (destType == typeof(string) && !itemsEnum.arraysFetched)
                {
                    object curValue = itemsEnum.target.GetValue(itemsEnum.target.TargetObject);
                    if (curValue == value || (curValue != null && curValue.Equals(value)))
                    {
                        bool success = false;
                        string val = GetDisplayString((NativeMethods.IPerPropertyBrowsing)itemsEnum.target.TargetObject, itemsEnum.target.DISPID, ref success);
                        if (success)
                        {
                            return val;
                        }
                    }
                }
                return base.ConvertTo(context, culture, value, destType);
            }
        }

        // This exists for perf reasons.   We delay doing this until we
        // are actually asked for the array of values.
        //
        private class Com2IPerPropertyBrowsingEnum : Com2Enum
        {
            internal Com2PropertyDescriptor target;
            private readonly Com2IPerPropertyBrowsingHandler handler;
            private readonly OleStrCAMarshaler nameMarshaller;
            private readonly Int32CAMarshaler valueMarshaller;
            internal bool arraysFetched;
            //private bool                 standardValuesQueried;

            public Com2IPerPropertyBrowsingEnum(Com2PropertyDescriptor targetObject, Com2IPerPropertyBrowsingHandler handler, OleStrCAMarshaler names, Int32CAMarshaler values, bool allowUnknowns) : base(Array.Empty<string>(), Array.Empty<object>(), allowUnknowns)
            {
                target = targetObject;
                nameMarshaller = names;
                valueMarshaller = values;
                this.handler = handler;
                arraysFetched = false;
            }

            /// <summary>
            ///  Retrieve a copy of the value array
            /// </summary>
            public override object[] Values
            {
                get
                {
                    EnsureArrays();
                    return base.Values;
                }
            }

            /// <summary>
            ///  Retrieve a copy of the nme array.
            /// </summary>
            public override string[] Names
            {
                get
                {
                    EnsureArrays();
                    return base.Names;
                }
            }

            /*internal bool StandardValuesQueried {
               get {
                   this.standardValuesQueried = value;
               }
            } */

            // ensure that we have processed the caStructs into arrays
            // of values and strings
            //
            private void EnsureArrays()
            {
                if (arraysFetched)
                {
                    return;
                }

                arraysFetched = true;

                try
                {
                    // marshal the items.
                    object[] nameItems = nameMarshaller.Items;
                    object[] cookieItems = valueMarshaller.Items;

                    NativeMethods.IPerPropertyBrowsing ppb = (NativeMethods.IPerPropertyBrowsing)target.TargetObject;
                    int itemCount = 0;

                    Debug.Assert(cookieItems != null && nameItems != null, "An item array is null");

                    if (nameItems.Length > 0)
                    {

                        object[] valueItems = new object[cookieItems.Length];
                        NativeMethods.VARIANT var = new NativeMethods.VARIANT();
                        int cookie;

                        Debug.Assert(cookieItems.Length == nameItems.Length, "Got uneven names and cookies");

                        // for each name item, we ask the object for it's corresponding value.
                        //
                        Type targetType = target.PropertyType;
                        for (int i = nameItems.Length - 1; i >= 0; i--)
                        {
                            cookie = (int)cookieItems[i];
                            if (nameItems[i] == null || !(nameItems[i] is string))
                            {
                                Debug.Fail("Bad IPerPropertyBrowsing item [" + i.ToString(CultureInfo.InvariantCulture) + "], name=" + (nameItems == null ? "(unknown)" : nameItems[i].ToString()));
                                continue;
                            }
                            var.vt = (short)NativeMethods.tagVT.VT_EMPTY;
                            int hr = ppb.GetPredefinedValue(target.DISPID, cookie, var);
                            if (hr == NativeMethods.S_OK && var.vt != (short)NativeMethods.tagVT.VT_EMPTY)
                            {
                                valueItems[i] = var.ToObject();
                                if (valueItems[i].GetType() != targetType)
                                {
                                    if (targetType.IsEnum)
                                    {
                                        valueItems[i] = Enum.ToObject(targetType, valueItems[i]);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            valueItems[i] = Convert.ChangeType(valueItems[i], targetType, CultureInfo.InvariantCulture);
                                        }
                                        catch
                                        {
                                            // oh well...
                                        }
                                    }
                                }
                            }

                            var.Clear();
                            if (hr == NativeMethods.S_OK)
                            {
                                itemCount++;
                                continue;
                            }
                            else if (itemCount > 0)
                            {
                                // shorten the arrays to ignore the failed ones.  this isn't terribly
                                // efficient but shouldn't happen very often.  It's rare for these to fail.
                                //
                                Array.Copy(nameItems, i, nameItems, i + 1, itemCount);
                                Array.Copy(valueItems, i, valueItems, i + 1, itemCount);
                            }

                        }

                        // pass this data down to the base Com2Enum object...
                        string[] strings = new string[itemCount];
                        Array.Copy(nameItems, 0, strings, 0, itemCount);
                        base.PopulateArrays(strings, valueItems);

                    }
                }
                catch (Exception ex)
                {
                    base.PopulateArrays(Array.Empty<string>(), Array.Empty<object>());
                    Debug.Fail("Failed to build IPerPropertyBrowsing editor. " + ex.GetType().Name + ", " + ex.Message);
                }
            }

            protected override void PopulateArrays(string[] names, object[] values)
            {
                // we call base.PopulateArrays directly when we actually want to do this.
            }

            public override object FromString(string s)
            {
                EnsureArrays();
                return base.FromString(s);
            }

            public override string ToString(object v)
            {

                // If the value is the object's current value, then
                // ask GetDisplay string first.  This is a perf improvement
                // because this way we don't populate the arrays when an object is selected, only
                // when the dropdown is actually opened.
                //
                if (target.IsCurrentValue(v))
                {

                    bool success = false;

                    string displayString = Com2IPerPropertyBrowsingHandler.GetDisplayString((NativeMethods.IPerPropertyBrowsing)target.TargetObject, target.DISPID, ref success);

                    if (success)
                    {
                        return displayString;
                    }
                }

                // couldn't get a display string...do the normal thing.
                //
                EnsureArrays();
                return base.ToString(v);
            }
        }

    }
}

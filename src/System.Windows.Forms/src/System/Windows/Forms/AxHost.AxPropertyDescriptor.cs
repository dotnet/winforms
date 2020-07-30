// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.ComponentModel.Com2Interop;
using System.Windows.Forms.Design;
using static Interop;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        internal class AxPropertyDescriptor : PropertyDescriptor
        {
            private readonly PropertyDescriptor baseProp;
            internal AxHost owner;
            private readonly DispIdAttribute dispid;

            private TypeConverter converter;
            private UITypeEditor editor;
            private readonly ArrayList updateAttrs = new ArrayList();
            private int flags;

            private const int FlagUpdatedEditorAndConverter = 0x00000001;
            private const int FlagCheckGetter = 0x00000002;
            private const int FlagGettterThrew = 0x00000004;
            private const int FlagIgnoreCanAccessProperties = 0x00000008;
            private const int FlagSettingValue = 0x00000010;

            internal AxPropertyDescriptor(PropertyDescriptor baseProp, AxHost owner) : base(baseProp)
            {
                this.baseProp = baseProp;
                this.owner = owner;

                // Get the category for this dispid.
                //
                dispid = (DispIdAttribute)baseProp.Attributes[typeof(DispIdAttribute)];
                if (dispid != null)
                {
                    // Look to see if this property has a property page.
                    // If it does, then it needs to be Browsable(true).
                    //
                    if (!IsBrowsable && !IsReadOnly)
                    {
                        Guid g = GetPropertyPage((Ole32.DispatchID)dispid.Value);
                        if (!Guid.Empty.Equals(g))
                        {
                            Debug.WriteLineIf(AxPropTraceSwitch.TraceVerbose, "Making property: " + Name + " browsable because we found an property page.");
                            AddAttribute(new BrowsableAttribute(true));
                        }
                    }

                    // Use the CategoryAttribute provided by the OCX.
                    CategoryAttribute cat = owner.GetCategoryForDispid((Ole32.DispatchID)dispid.Value);
                    if (cat != null)
                    {
                        AddAttribute(cat);
                    }

                    // Check to see if this a DataSource property.
                    // If it is, we can always get and set the value of this property.
                    //
                    if (PropertyType.GUID.Equals(dataSource_Guid))
                    {
                        SetFlag(FlagIgnoreCanAccessProperties, true);
                    }
                }
            }

            public override Type ComponentType
            {
                get
                {
                    return baseProp.ComponentType;
                }
            }

            public override TypeConverter Converter
            {
                get
                {
                    if (dispid != null)
                    {
                        UpdateTypeConverterAndTypeEditorInternal(false, Dispid);
                    }
                    return converter ?? base.Converter;
                }
            }

            internal Ole32.DispatchID Dispid
            {
                get
                {
                    DispIdAttribute dispid = (DispIdAttribute)baseProp.Attributes[typeof(DispIdAttribute)];
                    if (dispid != null)
                    {
                        return (Ole32.DispatchID)dispid.Value;
                    }

                    return Ole32.DispatchID.UNKNOWN;
                }
            }

            public override bool IsReadOnly
            {
                get
                {
                    return baseProp.IsReadOnly;
                }
            }

            public override Type PropertyType
            {
                get
                {
                    return baseProp.PropertyType;
                }
            }

            internal bool SettingValue
            {
                get
                {
                    return GetFlag(FlagSettingValue);
                }
            }

            private void AddAttribute(Attribute attr)
            {
                updateAttrs.Add(attr);
            }

            public override bool CanResetValue(object o)
            {
                return baseProp.CanResetValue(o);
            }

            public override object GetEditor(Type editorBaseType)
            {
                if (editorBaseType is null)
                {
                    throw new ArgumentNullException(nameof(editorBaseType));
                }

                if (dispid != null)
                {
                    UpdateTypeConverterAndTypeEditorInternal(false, (Ole32.DispatchID)dispid.Value);
                }

                if (editorBaseType.Equals(typeof(UITypeEditor)) && editor != null)
                {
                    return editor;
                }

                return base.GetEditor(editorBaseType);
            }

            private bool GetFlag(int flagValue)
            {
                return ((flags & flagValue) == flagValue);
            }

            private unsafe Guid GetPropertyPage(Ole32.DispatchID dispid)
            {
                try
                {
                    Oleaut32.IPerPropertyBrowsing ippb = owner.GetPerPropertyBrowsing();
                    if (ippb is null)
                    {
                        return Guid.Empty;
                    }

                    Guid rval = Guid.Empty;
                    if (ippb.MapPropertyToPage(dispid, &rval).Succeeded())
                    {
                        return rval;
                    }
                }
                catch (COMException)
                {
                }
                catch (Exception t)
                {
                    Debug.Fail(t.ToString());
                }
                return Guid.Empty;
            }

            public override object GetValue(object component)
            {
                if ((!GetFlag(FlagIgnoreCanAccessProperties) && !owner.CanAccessProperties) || GetFlag(FlagGettterThrew))
                {
                    return null;
                }

                try
                {
                    // Some controls fire OnChanged() notifications when getting values of some properties.
                    // To prevent this kind of recursion, we check to see if we are already inside a OnChanged() call.
                    //
                    owner.NoComponentChangeEvents++;
                    return baseProp.GetValue(component);
                }
                catch (Exception e)
                {
                    if (!GetFlag(FlagCheckGetter))
                    {
                        Debug.WriteLineIf(AxPropTraceSwitch.TraceVerbose, "Get failed for : " + Name + " with exception: " + e.Message + " .Making property non-browsable.");
                        SetFlag(FlagCheckGetter, true);
                        AddAttribute(new BrowsableAttribute(false));
                        owner.RefreshAllProperties = true;
                        SetFlag(FlagGettterThrew, true);
                    }
                    throw;
                }
                finally
                {
                    owner.NoComponentChangeEvents--;
                }
            }

            public void OnValueChanged(object component)
            {
                OnValueChanged(component, EventArgs.Empty);
            }

            public override void ResetValue(object o)
            {
                baseProp.ResetValue(o);
            }

            private void SetFlag(int flagValue, bool value)
            {
                if (value)
                {
                    flags |= flagValue;
                }
                else
                {
                    flags &= ~flagValue;
                }
            }

            public override void SetValue(object component, object value)
            {
                if (!GetFlag(FlagIgnoreCanAccessProperties) && !owner.CanAccessProperties)
                {
                    return;
                }

                // State oldOcxState = owner.OcxState;

                try
                {
                    SetFlag(FlagSettingValue, true);
                    if (PropertyType.IsEnum && value != null && value.GetType() != PropertyType)
                    {
                        baseProp.SetValue(component, Enum.ToObject(PropertyType, value));
                    }
                    else
                    {
                        baseProp.SetValue(component, value);
                    }
                }
                finally
                {
                    SetFlag(FlagSettingValue, false);
                }

                OnValueChanged(component);
                if (owner == component)
                {
                    owner.SetAxState(AxHost.valueChanged, true);
                }
            }

            public override bool ShouldSerializeValue(object o)
            {
                return baseProp.ShouldSerializeValue(o);
            }

            internal void UpdateAttributes()
            {
                if (updateAttrs.Count == 0)
                {
                    return;
                }

                ArrayList attributes = new ArrayList(AttributeArray);
                foreach (Attribute attr in updateAttrs)
                {
                    attributes.Add(attr);
                }

                Attribute[] temp = new Attribute[attributes.Count];
                attributes.CopyTo(temp, 0);
                AttributeArray = temp;

                updateAttrs.Clear();
            }

            /// <summary>
            ///  Called externally to update the editor or type converter.
            ///  This simply sets flags so this will happen, it doesn't actually to the update...
            ///  we wait and do that on-demand for perf.
            /// </summary>
            internal void UpdateTypeConverterAndTypeEditor(bool force)
            {
                // if this is an external request, flip the flag to false so we do the update on demand.
                //
                if (GetFlag(FlagUpdatedEditorAndConverter) && force)
                {
                    SetFlag(FlagUpdatedEditorAndConverter, false);
                }
            }

            /// <summary>
            ///  Called externally to update the editor or type converter.
            ///  This simply sets flags so this will happen, it doesn't actually to the update...
            ///  we wait and do that on-demand for perf.
            /// </summary>
            internal unsafe void UpdateTypeConverterAndTypeEditorInternal(bool force, Ole32.DispatchID dispid)
            {
                // check to see if we're being forced here or if the work really
                // needs to be done.
                //
                if (GetFlag(FlagUpdatedEditorAndConverter) && !force)
                {
                    return;
                }

                if (owner.GetOcx() is null)
                {
                    return;
                }

                try
                {
                    Oleaut32.IPerPropertyBrowsing ppb = owner.GetPerPropertyBrowsing();

                    if (ppb != null)
                    {
                        bool hasStrings = false;

                        // check for enums
                        var caStrings = new Ole32.CA();
                        var caCookies = new Ole32.CA();

                        HRESULT hr = HRESULT.S_OK;
                        try
                        {
                            hr = ppb.GetPredefinedStrings(dispid, &caStrings, &caCookies);
                        }
                        catch (ExternalException ex)
                        {
                            hr = (HRESULT)ex.ErrorCode;
                            Debug.Fail("An exception occurred inside IPerPropertyBrowsing::GetPredefinedStrings(dispid=" +
                                       dispid + "), object type=" + new ComNativeDescriptor().GetClassName(ppb));
                        }

                        if (hr != HRESULT.S_OK)
                        {
                            hasStrings = false;
                            // Destroy the existing editor if we created the current one
                            // so if the items have disappeared, we don't hold onto the old
                            // items.
                            if (converter is Com2EnumConverter)
                            {
                                converter = null;
                            }
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
                                if (converter is null)
                                {
                                    converter = new AxEnumConverter(this, new AxPerPropertyBrowsingEnum(this, owner, stringMarshaler, intMarshaler, true));
                                }
                                else if (converter is AxEnumConverter)
                                {
                                    ((AxEnumConverter)converter).RefreshValues();
                                    if (((AxEnumConverter)converter).com2Enum is AxPerPropertyBrowsingEnum axEnum)
                                    {
                                        axEnum.RefreshArrays(stringMarshaler, intMarshaler);
                                    }
                                }
                            }
                            else
                            {
                                //hasStrings = false;
                            }
                        }
                        else
                        {
                            // if we didn't get any strings, try the proppage edtior
                            //
                            // Check to see if this is a property that we have already massaged to be a
                            // .Net type. If it is, don't bother with custom property pages. We already
                            // have a .Net Editor for this type.
                            //
                            ComAliasNameAttribute comAlias = (ComAliasNameAttribute)baseProp.Attributes[typeof(ComAliasNameAttribute)];
                            if (comAlias is null)
                            {
                                Guid g = GetPropertyPage(dispid);

                                if (!Guid.Empty.Equals(g))
                                {
                                    editor = new AxPropertyTypeEditor(this, g);

                                    // Show any non-browsable property that has an editor through a
                                    // property page.
                                    //
                                    if (!IsBrowsable)
                                    {
                                        Debug.WriteLineIf(AxPropTraceSwitch.TraceVerbose, "Making property: " + Name + " browsable because we found an editor.");
                                        AddAttribute(new BrowsableAttribute(true));
                                    }
                                }
                            }
                        }
                    }

                    SetFlag(FlagUpdatedEditorAndConverter, true);
                }
                catch (Exception e)
                {
                    Debug.WriteLineIf(AxPropTraceSwitch.TraceVerbose, "could not get the type editor for property: " + Name + " Exception: " + e);
                }
            }
        }

        private class AxPropertyTypeEditor : UITypeEditor
        {
            private readonly AxPropertyDescriptor propDesc;
            private Guid guid;

            public AxPropertyTypeEditor(AxPropertyDescriptor pd, Guid guid)
            {
                propDesc = pd;
                this.guid = guid;
            }

            /// <summary>
            ///  Takes the value returned from valueAccess.getValue() and modifies or replaces
            ///  the value, passing the result into valueAccess.setValue().  This is where
            ///  an editor can launch a modal dialog or create a drop down editor to allow
            ///  the user to modify the value.  Host assistance in presenting UI to the user
            ///  can be found through the valueAccess.getService function.
            /// </summary>
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                try
                {
                    object instance = context.Instance;
                    propDesc.owner.ShowPropertyPageForDispid(propDesc.Dispid, guid);
                }
                catch (Exception ex1)
                {
                    if (provider != null)
                    {
                        IUIService uiSvc = (IUIService)provider.GetService(typeof(IUIService));
                        if (uiSvc != null)
                        {
                            uiSvc.ShowError(ex1, SR.ErrorTypeConverterFailed);
                        }
                    }
                }
                return value;
            }

            /// <summary>
            ///  Retrieves the editing style of the Edit method.  If the method
            ///  is not supported, this will return None.
            /// </summary>
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }
        }

        /// <summary>
        ///  simple derivation of the com2enumconverter that allows us to intercept
        ///  the call to GetStandardValues so we can on-demand update the enum values.
        /// </summary>
        private class AxEnumConverter : Com2EnumConverter
        {
            private readonly AxPropertyDescriptor target;

            public AxEnumConverter(AxPropertyDescriptor target, Com2Enum com2Enum) : base(com2Enum)
            {
                this.target = target;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                // make sure the converter has been properly refreshed -- calling
                // the Converter property does this.
                //
                TypeConverter tc = this;
                tc = target.Converter;
                return base.GetStandardValues(context);
            }
        }
    }
}

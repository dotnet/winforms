﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.ComponentModel.Com2Interop;
using Windows.Win32.System.Ole;
using static Interop;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        internal class AxPropertyDescriptor : PropertyDescriptor
        {
            private readonly PropertyDescriptor _baseDescriptor;
            internal AxHost _owner;
            private readonly DispIdAttribute _dispid;

            private TypeConverter _converter;
            private UITypeEditor _editor;
            private readonly ArrayList _updateAttributes = new();
            private int _flags;

            private const int FlagUpdatedEditorAndConverter = 0x00000001;
            private const int FlagCheckGetter = 0x00000002;
            private const int FlagGetterThrew = 0x00000004;
            private const int FlagIgnoreCanAccessProperties = 0x00000008;
            private const int FlagSettingValue = 0x00000010;

            internal AxPropertyDescriptor(PropertyDescriptor baseDescriptor, AxHost owner) : base(baseDescriptor)
            {
                _baseDescriptor = baseDescriptor;
                _owner = owner;

                // Get the category for this dispid.
                _dispid = baseDescriptor.GetAttribute<DispIdAttribute>();
                if (_dispid is not null)
                {
                    // Look to see if this property has a property page.
                    // If it does, then it needs to be Browsable(true).
                    if (!IsBrowsable && !IsReadOnly)
                    {
                        Guid g = GetPropertyPage((Ole32.DispatchID)_dispid.Value);
                        if (!Guid.Empty.Equals(g))
                        {
                            Debug.WriteLineIf(s_axPropTraceSwitch.TraceVerbose, $"Making property: {Name} browsable because we found an property page.");
                            AddAttribute(new BrowsableAttribute(true));
                        }
                    }

                    // Use the CategoryAttribute provided by the OCX.
                    CategoryAttribute cat = owner.GetCategoryForDispid((Ole32.DispatchID)_dispid.Value);
                    if (cat is not null)
                    {
                        AddAttribute(cat);
                    }

                    // Check to see if this a DataSource property.
                    // If it is, we can always get and set the value of this property.
                    if (PropertyType.GUID.Equals(s_dataSource_Guid))
                    {
                        SetFlag(FlagIgnoreCanAccessProperties, true);
                    }
                }
            }

            public override Type ComponentType
            {
                get
                {
                    return _baseDescriptor.ComponentType;
                }
            }

            public override TypeConverter Converter
            {
                [RequiresUnreferencedCode(TrimmingConstants.PropertyDescriptorPropertyTypeMessage)]
                get
                {
                    if (_dispid is not null)
                    {
                        UpdateTypeConverterAndTypeEditorInternal(force: false, Dispid);
                    }

                    return _converter ?? base.Converter;
                }
            }

            internal Ole32.DispatchID Dispid
                => _baseDescriptor.TryGetAttribute(out DispIdAttribute dispid)
                    ? (Ole32.DispatchID)dispid.Value
                    : Ole32.DispatchID.UNKNOWN;

            public override bool IsReadOnly => _baseDescriptor.IsReadOnly;

            public override Type PropertyType => _baseDescriptor.PropertyType;

            internal bool SettingValue => GetFlag(FlagSettingValue);

            private void AddAttribute(Attribute attr)
            {
                _updateAttributes.Add(attr);
            }

            public override bool CanResetValue(object o)
            {
                return _baseDescriptor.CanResetValue(o);
            }

            [RequiresUnreferencedCode(TrimmingConstants.EditorRequiresUnreferencedCode)]
            public override object GetEditor(Type editorBaseType)
            {
                ArgumentNullException.ThrowIfNull(editorBaseType);

                if (_dispid is not null)
                {
                    UpdateTypeConverterAndTypeEditorInternal(false, (Ole32.DispatchID)_dispid.Value);
                }

                if (editorBaseType.Equals(typeof(UITypeEditor)) && _editor is not null)
                {
                    return _editor;
                }

                return base.GetEditor(editorBaseType);
            }

            private bool GetFlag(int flagValue)
            {
                return ((_flags & flagValue) == flagValue);
            }

            private unsafe Guid GetPropertyPage(Ole32.DispatchID dispid)
            {
                try
                {
                    IPerPropertyBrowsing.Interface ippb = _owner.GetPerPropertyBrowsing();
                    if (ippb is null)
                    {
                        return Guid.Empty;
                    }

                    Guid rval = Guid.Empty;
                    if (ippb.MapPropertyToPage((int)dispid, &rval).Succeeded)
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
                if ((!GetFlag(FlagIgnoreCanAccessProperties) && !_owner.CanAccessProperties) || GetFlag(FlagGetterThrew))
                {
                    return null;
                }

                try
                {
                    // Some controls fire OnChanged() notifications when getting values of some properties.
                    // To prevent this kind of recursion, we check to see if we are already inside a OnChanged() call.

                    _owner.NoComponentChangeEvents++;
                    return _baseDescriptor.GetValue(component);
                }
                catch (Exception e)
                {
                    if (!GetFlag(FlagCheckGetter))
                    {
                        Debug.WriteLineIf(
                            s_axPropTraceSwitch.TraceVerbose,
                            $"Get failed for : {Name} with exception: {e.Message}. Making property non-browsable.");
                        SetFlag(FlagCheckGetter, true);
                        AddAttribute(new BrowsableAttribute(false));
                        _owner.RefreshAllProperties = true;
                        SetFlag(FlagGetterThrew, true);
                    }

                    throw;
                }
                finally
                {
                    _owner.NoComponentChangeEvents--;
                }
            }

            public void OnValueChanged(object component)
            {
                OnValueChanged(component, EventArgs.Empty);
            }

            public override void ResetValue(object o)
            {
                _baseDescriptor.ResetValue(o);
            }

            private void SetFlag(int flagValue, bool value)
            {
                if (value)
                {
                    _flags |= flagValue;
                }
                else
                {
                    _flags &= ~flagValue;
                }
            }

            public override void SetValue(object component, object value)
            {
                if (!GetFlag(FlagIgnoreCanAccessProperties) && !_owner.CanAccessProperties)
                {
                    return;
                }

                // State oldOcxState = owner.OcxState;

                try
                {
                    SetFlag(FlagSettingValue, true);
                    if (PropertyType.IsEnum && value is not null && value.GetType() != PropertyType)
                    {
                        _baseDescriptor.SetValue(component, Enum.ToObject(PropertyType, value));
                    }
                    else
                    {
                        _baseDescriptor.SetValue(component, value);
                    }
                }
                finally
                {
                    SetFlag(FlagSettingValue, false);
                }

                OnValueChanged(component);
                if (_owner == component)
                {
                    _owner.SetAxState(AxHost.s_valueChanged, true);
                }
            }

            public override bool ShouldSerializeValue(object o)
            {
                return _baseDescriptor.ShouldSerializeValue(o);
            }

            internal void UpdateAttributes()
            {
                if (_updateAttributes.Count == 0)
                {
                    return;
                }

                ArrayList attributes = new ArrayList(AttributeArray);
                foreach (Attribute attr in _updateAttributes)
                {
                    attributes.Add(attr);
                }

                Attribute[] temp = new Attribute[attributes.Count];
                attributes.CopyTo(temp, 0);
                AttributeArray = temp;

                _updateAttributes.Clear();
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
                // Check to see if we're being forced here or if the work really needs to be done.
                if ((GetFlag(FlagUpdatedEditorAndConverter) && !force) || _owner.GetOcx() is null)
                {
                    return;
                }

                try
                {
                    IPerPropertyBrowsing.Interface ppb = _owner.GetPerPropertyBrowsing();

                    if (ppb is not null)
                    {
                        // Check for enums
                        CALPOLESTR caStrings = default;
                        CADWORD caCookies = default;

                        HRESULT hr = HRESULT.S_OK;
                        try
                        {
                            hr = ppb.GetPredefinedStrings((int)dispid, &caStrings, &caCookies);
                        }
                        catch (ExternalException ex)
                        {
                            hr = (HRESULT)ex.ErrorCode;
                            Debug.Fail($"An exception occurred inside IPerPropertyBrowsing::GetPredefinedStrings(dispid={dispid}), object type={ComNativeDescriptor.GetClassName(ppb)}");
                        }

                        if (hr == HRESULT.S_OK)
                        {
                            string[] names = caStrings.ConvertAndFree();
                            uint[] cookies = caCookies.ConvertAndFree();

                            if (names.Length > 0 && cookies.Length > 0)
                            {
                                if (_converter is null)
                                {
                                    _converter = new AxEnumConverter(this, new AxPerPropertyBrowsingEnum(
                                        this,
                                        _owner,
                                        names,
                                        cookies));
                                }
                                else if (_converter is AxEnumConverter enumConverter)
                                {
                                    enumConverter.RefreshValues();
                                    if (enumConverter._com2Enum is AxPerPropertyBrowsingEnum axEnum)
                                    {
                                        axEnum.RefreshArrays(names, cookies);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Destroy the existing editor if we created the current one so if the items have
                            // disappeared, we don't hold onto the old items.
                            if (_converter is Com2EnumConverter)
                            {
                                _converter = null;
                            }

                            // If we didn't get any strings, try the proppage editor
                            //
                            // Check to see if this is a property that we have already massaged to be a
                            // .Net type. If it is, don't bother with custom property pages. We already
                            // have a .Net Editor for this type.

                            if (_baseDescriptor.GetAttribute<ComAliasNameAttribute>() is null)
                            {
                                Guid g = GetPropertyPage(dispid);

                                if (!Guid.Empty.Equals(g))
                                {
                                    _editor = new AxPropertyTypeEditor(this, g);

                                    // Show any non-browsable property that has an editor through a property page.
                                    if (!IsBrowsable)
                                    {
                                        Debug.WriteLineIf(
                                            s_axPropTraceSwitch.TraceVerbose,
                                            $"Making property: {Name} browsable because we found an editor.");

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
                    Debug.WriteLineIf(s_axPropTraceSwitch.TraceVerbose, $"could not get the type editor for property: {Name} Exception: {e}");
                }
            }
        }
    }
}

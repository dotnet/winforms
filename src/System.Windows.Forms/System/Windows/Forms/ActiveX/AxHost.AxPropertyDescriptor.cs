// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.ComponentModel.Com2Interop;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms;

public abstract partial class AxHost
{
    internal class AxPropertyDescriptor : PropertyDescriptor
    {
        private readonly PropertyDescriptor _baseDescriptor;
        internal AxHost _owner;
        private readonly DispIdAttribute? _dispid;

        private TypeConverter? _converter;
        private UITypeEditor? _editor;
        private readonly List<Attribute> _updateAttributes = [];
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
                    Guid g = GetPropertyPage(_dispid.Value);
                    if (!Guid.Empty.Equals(g))
                    {
                        AddAttribute(new BrowsableAttribute(true));
                    }
                }

                // Use the CategoryAttribute provided by the OCX.
                CategoryAttribute? cat = owner.GetCategoryForDispid(_dispid.Value);
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

        internal int Dispid
            => _baseDescriptor.TryGetAttribute(out DispIdAttribute? dispid)
                ? dispid.Value
                : PInvokeCore.DISPID_UNKNOWN;

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
        public override object? GetEditor(Type editorBaseType)
        {
            ArgumentNullException.ThrowIfNull(editorBaseType);

            if (_dispid is not null)
            {
                UpdateTypeConverterAndTypeEditorInternal(false, _dispid.Value);
            }

            return editorBaseType.Equals(typeof(UITypeEditor)) && _editor is not null
                ? _editor
                : base.GetEditor(editorBaseType);
        }

        private bool GetFlag(int flagValue)
        {
            return ((_flags & flagValue) == flagValue);
        }

        private unsafe Guid GetPropertyPage(int dispid)
        {
            try
            {
                using var propertyBrowsing = _owner.TryGetComScope<IPerPropertyBrowsing>(out HRESULT hr);
                if (hr.Failed)
                {
                    return Guid.Empty;
                }

                Guid rval = Guid.Empty;
                if (propertyBrowsing.Value->MapPropertyToPage(dispid, &rval).Succeeded)
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

        public override object? GetValue(object? component)
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
            catch (Exception)
            {
                if (!GetFlag(FlagCheckGetter))
                {
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

        public void OnValueChanged(object? component)
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

        public override void SetValue(object? component, object? value)
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
                _owner.SetAxState(s_valueChanged, true);
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

            List<Attribute> attributes = [..AttributeArray!];
            attributes.AddRange(_updateAttributes);
            AttributeArray = [.. attributes];
            _updateAttributes.Clear();
        }

        /// <summary>
        ///  Called externally to flag that we need to update the editor or type converter.
        /// </summary>
        internal void UpdateTypeConverterAndTypeEditor(bool force)
        {
            // If this is an external request, flip the flag to false so we do the update on demand.
            if (GetFlag(FlagUpdatedEditorAndConverter) && force)
            {
                SetFlag(FlagUpdatedEditorAndConverter, false);
            }
        }

        /// <summary>
        ///  Called externally to flag that we need to update the editor or type converter for a specific DISPID.
        /// </summary>
        internal unsafe void UpdateTypeConverterAndTypeEditorInternal(bool force, int dispid)
        {
            // Check to see if we're being forced here or if the work really needs to be done.
            if ((GetFlag(FlagUpdatedEditorAndConverter) && !force) || _owner.GetOcx() is null)
            {
                return;
            }

            try
            {
                using var propertyBrowsing = _owner.TryGetComScope<IPerPropertyBrowsing>(out HRESULT hr);
                if (hr.Failed)
                {
                    return;
                }

                // Check for enums
                CALPOLESTR caStrings = default;
                CADWORD caCookies = default;

                hr = propertyBrowsing.Value->GetPredefinedStrings(dispid, &caStrings, &caCookies);

                if (hr.Failed)
                {
                    Debug.Fail($"IPerPropertyBrowsing::GetPredefinedStrings(dispid={dispid}) failed: {hr}");
                }

                if (hr == HRESULT.S_OK)
                {
                    string?[] names = caStrings.ConvertAndFree();
                    uint[] cookies = caCookies.ConvertAndFree();

                    if (names.Length > 0 && cookies.Length > 0)
                    {
                        if (_converter is null)
                        {
                            _converter = new AxEnumConverter(
                                this,
                                new AxPerPropertyBrowsingEnum(
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

                    return;
                }

                // Destroy the existing editor if we created the current one so if the items have
                // disappeared, we don't hold onto the old items.
                if (_converter is Com2EnumConverter)
                {
                    _converter = null;
                }

                // If we didn't get any strings, try the property page editor.
                //
                // Check to see if this is a property that we have already massaged to be a .NET type. If it is, don't
                // bother with custom property pages. We already have a .NET Editor for this type.

                if (_baseDescriptor.GetAttribute<ComAliasNameAttribute>() is not null)
                {
                    return;
                }

                Guid pageGuid = GetPropertyPage(dispid);

                if (pageGuid != Guid.Empty)
                {
                    _editor = new AxPropertyTypeEditor(this, pageGuid);

                    // Show any non-browsable property that has an editor through a property page.
                    if (!IsBrowsable)
                    {
                        AddAttribute(new BrowsableAttribute(true));
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                SetFlag(FlagUpdatedEditorAndConverter, true);
            }
        }
    }
}

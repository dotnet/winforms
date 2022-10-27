﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Diagnostics.Debug;
using Windows.Win32.System.Ole;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  <para>
    ///   This class wraps a com native property in a property descriptor. It maintains all information relative to the
    ///   basic (e.g. ITypeInfo) information about the member dispid function, and converts that info to meaningful
    ///   managed code information.
    ///  </para>
    ///  <para>
    ///   It also allows other objects to register listeners to add extended information at runtime such as attributes
    ///   of <see cref="TypeConverter"/>s.
    ///  </para>
    /// </summary>
    internal partial class Com2PropertyDescriptor : PropertyDescriptor, ICloneable
    {
        private EventHandlerList? _events;

        // Is this property read only?
        private readonly bool _baseReadOnly;
        private bool _readOnly;

        // The resolved native type -> clr type
        private readonly Type? _propertyType;
        private TypeConverter? _converter;
        private object? _editor;

        private string? _displayName;

        // This is any extra data needed. For IDispatch types, it's the GUID of the interface, etc.
        private readonly object? _typeData;

        // Keeps track of which data members need to be refreshed.
        private int _refreshState;

        // Our original baseline properties
        private Attribute[]? _baseAttributes;

        // Our cached last value -- this is only for checking if we should ask for a display value.
        private object? _lastValue;

        // For Object and dispatch types, we hide them by default.
        private readonly bool _typeHide;

        // This property is hidden because its get didn't return S_OK.
        private bool _hrHidden;

        // Our event signatures.
        private static readonly object EventGetBaseAttributes = new();
        private static readonly object EventGetDynamicAttributes = new();
        private static readonly object EventShouldRefresh = new();
        private static readonly object EventGetDisplayName = new();
        private static readonly object EventGetDisplayValue = new();
        private static readonly object EventGetIsReadOnly = new();
        private static readonly object EventGetTypeConverterAndTypeEditor = new();
        private static readonly object EventShouldSerializeValue = new();
        private static readonly object EventCanResetValue = new();
        private static readonly object EventResetValue = new();

        private static readonly Guid GUID_COLOR = new("{66504301-BE0F-101A-8BBB-00AA00300CAB}");

        // Our map of native types that we can map to managed types for editors
        private static readonly IDictionary s_oleConverters = new SortedList
        {
            [GUID_COLOR] = typeof(Com2ColorConverter),
            [typeof(Ole32.IFontDisp).GUID] = typeof(Com2FontConverter),
            [typeof(Ole32.IFont).GUID] = typeof(Com2FontConverter),
            [IPictureDisp.Guid] = typeof(Com2PictureConverter),
            [IPicture.Guid] = typeof(Com2PictureConverter)
        };

        private readonly Com2DataTypeToManagedDataTypeConverter? _valueConverter;

        public Com2PropertyDescriptor(
            Ole32.DispatchID dispid,
            string name,
            Attribute[] attributes,
            bool readOnly,
            Type? propertyType,
            object? typeData,
            bool hrHidden)
            : base(name, attributes)
        {
            _baseReadOnly = readOnly;
            _readOnly = readOnly;

            _baseAttributes = attributes;
            SetNeedsRefresh(Com2PropertyDescriptorRefresh.BaseAttributes, true);

            _hrHidden = hrHidden;

            // Readonly to begin with is always read only.
            SetNeedsRefresh(Com2PropertyDescriptorRefresh.ReadOnly, readOnly);

            _propertyType = propertyType;

            DISPID = dispid;

            if (typeData is not null)
            {
                _typeData = typeData;
                if (typeData is Com2Enum comEnum)
                {
                    _converter = new Com2EnumConverter(comEnum);
                }
                else if (typeData is Guid guid)
                {
                    _valueConverter = CreateOleTypeConverter((Type?)s_oleConverters[guid]);
                }
            }

            // Check if this is hidden from metadata.
            CanShow = true;

            if (attributes is not null)
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    if (attributes[i].Equals(BrowsableAttribute.No) && !hrHidden)
                    {
                        CanShow = false;
                        break;
                    }
                }
            }

            if (CanShow && (propertyType == typeof(object) || (_valueConverter is null && propertyType == typeof(Oleaut32.IDispatch))))
            {
                _typeHide = true;
            }
        }

        protected Attribute[] BaseAttributes
        {
            get
            {
                if (!GetNeedsRefresh(Com2PropertyDescriptorRefresh.BaseAttributes))
                {
                    Debug.Assert(_baseAttributes is not null);
                    return _baseAttributes ??= Array.Empty<Attribute>();
                }

                SetNeedsRefresh(Com2PropertyDescriptorRefresh.BaseAttributes, false);

                int baseCount = _baseAttributes is null ? 0 : _baseAttributes.Length;

                List<Attribute> attributes = new();

                if (_baseAttributes is not null)
                {
                    attributes.AddRange(_baseAttributes);
                }

                OnGetBaseAttributes(new GetAttributesEvent(attributes));

                if (attributes.Count != baseCount)
                {
                    _baseAttributes = new Attribute[attributes.Count];
                }

                if (_baseAttributes is not null)
                {
                    attributes.CopyTo(_baseAttributes, 0);
                }
                else
                {
                    _baseAttributes = Array.Empty<Attribute>();
                }

                return _baseAttributes;
            }
        }

        public override AttributeCollection Attributes
        {
            get
            {
                if (AttributesValid || InAttributeQuery)
                {
                    return base.Attributes;
                }

                // Restore our base attributes
                AttributeArray = BaseAttributes;

                List<Attribute>? newAttributes = null;

                // If we are forcing a hide.
                if (_typeHide && CanShow)
                {
                    newAttributes ??= new(AttributeArray);
                    newAttributes.Add(new BrowsableAttribute(false));
                }
                else if (_hrHidden)
                {
                    // Check to see if the get still fails.
                    object? target = TargetObject;
                    if (target is not null)
                    {
                        HRESULT hr = ComNativeDescriptor.GetPropertyValue(target, DISPID, new object[1]);

                        // If not, go ahead and make this a browsable item.
                        if (hr.Succeeded)
                        {
                            // Make it browsable.
                            newAttributes ??= new(AttributeArray);
                            newAttributes.Add(new BrowsableAttribute(true));
                            _hrHidden = false;
                        }
                    }
                }

                InAttributeQuery = true;
                try
                {
                    // Demand get any extended attributes
                    List<Attribute> attributeList = new();

                    OnGetDynamicAttributes(new GetAttributesEvent(attributeList));

                    if (attributeList.Count > 0)
                    {
                        newAttributes ??= new(AttributeArray);

                        // Push any new attributes into the base type.
                        for (int i = 0; i < attributeList.Count; i++)
                        {
                            newAttributes.Add(attributeList[i]);
                        }
                    }
                }
                finally
                {
                    InAttributeQuery = false;
                }

                // These are now valid.
                SetNeedsRefresh(Com2PropertyDescriptorRefresh.Attributes, false);

                // If we reconfigured attributes, then poke the new set back in.
                if (newAttributes is not null)
                {
                    Attribute[] temp = new Attribute[newAttributes.Count];
                    newAttributes.CopyTo(temp, 0);
                    AttributeArray = temp;
                }

                return base.Attributes;
            }
        }

        /// <summary>
        ///  Checks if the attributes are valid. Asks any clients if they  would like attributes required.
        /// </summary>
        protected bool AttributesValid => !GetNeedsRefresh(Com2PropertyDescriptorRefresh.Attributes);

        /// <summary>
        ///  Checks if this item can be shown.
        /// </summary>
        public bool CanShow { get; }

        /// <summary>
        ///  Retrieves the type of the component this PropertyDescriptor is bound to.
        /// </summary>
        public override Type ComponentType => typeof(Oleaut32.IDispatch);

        /// <summary>
        ///  Retrieves the type converter for this property.
        /// </summary>
        public override TypeConverter Converter
        {
            [RequiresUnreferencedCode(TrimmingConstants.PropertyDescriptorPropertyTypeMessage)]
            get
            {
                if (TypeConverterValid)
                {
                    return _converter;
                }

                object? typeEditor = null;

                GetTypeConverterAndTypeEditor(ref _converter, typeof(UITypeEditor), ref typeEditor);

                if (!TypeEditorValid)
                {
                    _editor = typeEditor;
                    SetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeEditor, false);
                }

                SetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeConverter, false);

                return _converter;
            }
        }

        /// <summary>
        ///  Retrieves whether this component is applying a type conversion...
        /// </summary>
        [MemberNotNullWhen(true, nameof(_valueConverter))]
        public bool ConvertingNativeType => _valueConverter is not null;

        /// <summary>
        ///  Retrieves the default value for this property.
        /// </summary>
        protected virtual object? DefaultValue => null;

        /// <summary>
        ///  Retrieves the DISPID for this item
        /// </summary>
        public Ole32.DispatchID DISPID { get; }

        /// <summary>
        ///  Gets the friendly name that should be displayed to the user in a window like the Property Browser.
        /// </summary>
        public override string DisplayName
        {
            get
            {
                if (!DisplayNameValid)
                {
                    GetNameItemEvent getNameEvent = new(base.DisplayName);
                    OnGetDisplayName(getNameEvent);
                    _displayName = getNameEvent.NameString;
                    SetNeedsRefresh(Com2PropertyDescriptorRefresh.DisplayName, false);
                }

                return _displayName;
            }
        }

        /// <summary>
        ///  Checks if the property display name is valid.
        /// </summary>
        [MemberNotNullWhen(true, nameof(_displayName))]
        protected bool DisplayNameValid
            => _displayName is not null && !GetNeedsRefresh(Com2PropertyDescriptorRefresh.DisplayName);

        protected EventHandlerList Events => _events ??= new EventHandlerList();

        protected bool InAttributeQuery { get; private set; }

        public override bool IsReadOnly
        {
            get
            {
                if (!ReadOnlyValid)
                {
                    _readOnly |= Attributes[typeof(ReadOnlyAttribute)]?.Equals(ReadOnlyAttribute.Yes) ?? false;
                    GetBoolValueEvent getBoolEvent = new(_readOnly);
                    OnGetIsReadOnly(getBoolEvent);
                    _readOnly = getBoolEvent.Value;
                    SetNeedsRefresh(Com2PropertyDescriptorRefresh.ReadOnly, false);
                }

                return _readOnly;
            }
        }

        internal Com2Properties? PropertyManager { get; set; }

#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
        // Unfortunately we buck the annotations on PropertyDescriptor here. In cases where we can't resolve a
        // type for a COM object's property, we create descriptors that are marked as non browsable. It isn't clear
        // what the repercussions would be of not creating the descriptor so we'll continue to create them this way
        // and mark as nullable here and try to guard where we can.

        /// <inheritdoc/>
        public override Type? PropertyType
             // Replace the type with the mapped converter type
             => _valueConverter is not null ? _valueConverter.ManagedType : _propertyType;
#pragma warning restore CS8764

        /// <summary>
        ///  Checks if the read only state is valid. Asks clients if they would like read-only required.
        /// </summary>
        protected bool ReadOnlyValid => _baseReadOnly || !GetNeedsRefresh(Com2PropertyDescriptorRefresh.ReadOnly);

        /// <summary>
        ///  Gets the Object that this descriptor was created for.
        ///  May be null if the Object's reference has died.
        /// </summary>
        public virtual object? TargetObject => PropertyManager?.TargetObject;

        [MemberNotNullWhen(true, nameof(_converter))]
        protected bool TypeConverterValid
            => _converter is not null && !GetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeConverter);

        [MemberNotNullWhen(true, nameof(_editor))]
        protected bool TypeEditorValid
            => _editor is not null && !GetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeEditor);

        public event GetBoolValueEventHandler QueryCanResetValue
        {
            add => Events.AddHandler(EventCanResetValue, value);
            remove => Events.RemoveHandler(EventCanResetValue, value);
        }

        public event GetAttributesEventHandler QueryGetBaseAttributes
        {
            add => Events.AddHandler(EventGetBaseAttributes, value);
            remove => Events.RemoveHandler(EventGetBaseAttributes, value);
        }

        public event GetAttributesEventHandler QueryGetDynamicAttributes
        {
            add => Events.AddHandler(EventGetDynamicAttributes, value);
            remove => Events.RemoveHandler(EventGetDynamicAttributes, value);
        }

        public event GetNameItemEventHandler QueryGetDisplayName
        {
            add => Events.AddHandler(EventGetDisplayName, value);
            remove => Events.RemoveHandler(EventGetDisplayName, value);
        }

        public event GetNameItemEventHandler QueryGetDisplayValue
        {
            add => Events.AddHandler(EventGetDisplayValue, value);
            remove => Events.RemoveHandler(EventGetDisplayValue, value);
        }

        public event GetBoolValueEventHandler QueryGetIsReadOnly
        {
            add => Events.AddHandler(EventGetIsReadOnly, value);
            remove => Events.RemoveHandler(EventGetIsReadOnly, value);
        }

        public event GetTypeConverterAndTypeEditorEventHandler QueryGetTypeConverterAndTypeEditor
        {
            add => Events.AddHandler(EventGetTypeConverterAndTypeEditor, value);
            remove => Events.RemoveHandler(EventGetTypeConverterAndTypeEditor, value);
        }

        public event Com2EventHandler QueryResetValue
        {
            add => Events.AddHandler(EventResetValue, value);
            remove => Events.RemoveHandler(EventResetValue, value);
        }

        public event GetBoolValueEventHandler QueryShouldSerializeValue
        {
            add => Events.AddHandler(EventShouldSerializeValue, value);
            remove => Events.RemoveHandler(EventShouldSerializeValue, value);
        }

        /// <summary>
        ///  Indicates whether reset will change the value of the component.  If there
        ///  is a DefaultValueAttribute, then this will return true if getValue returns
        ///  something different than the default value.  If there is a reset method and
        ///  a shouldPersist method, this will return what shouldPersist returns.
        ///  If there is just a reset method, this always returns true.  If none of these
        ///  cases apply, this returns false.
        /// </summary>
        public override bool CanResetValue(object component)
        {
            if (component is ICustomTypeDescriptor descriptor)
            {
                object? owner = descriptor.GetPropertyOwner(this);
                if (owner is null)
                {
                    return false;
                }

                component = owner;
            }

            if (component == TargetObject)
            {
                GetBoolValueEvent boolEvent = new(defaultValue: false);
                OnCanResetValue(boolEvent);
                return boolEvent.Value;
            }

            return false;
        }

        public object Clone()
            => new Com2PropertyDescriptor(
                DISPID,
                Name,
                (Attribute[])(_baseAttributes?.Clone() ?? Array.Empty<Attribute>()),
                _readOnly,
                _propertyType,
                _typeData,
                _hrHidden);

        /// <summary>
        ///  Creates a converter Object, first by looking for a ctor with a Com2PropertyDescriptor
        ///  parameter, then using the default ctor if it is not found.
        /// </summary>
        private Com2DataTypeToManagedDataTypeConverter? CreateOleTypeConverter(Type? type)
        {
            if (type is null)
            {
                return null;
            }

            ConstructorInfo? constructor = type.GetConstructor(new Type[] { typeof(Com2PropertyDescriptor) });
            Com2DataTypeToManagedDataTypeConverter? converter = constructor is not null
                ? (Com2DataTypeToManagedDataTypeConverter)constructor.Invoke(new object[] { this })
                : (Com2DataTypeToManagedDataTypeConverter?)Activator.CreateInstance(type);

            return converter;
        }

        /// <summary>
        ///  Creates an instance of the member attribute collection. This can
        ///  be overriden by subclasses to return a subclass of AttributeCollection.
        /// </summary>
        protected override AttributeCollection CreateAttributeCollection() => new AttributeCollection(AttributeArray);

        private TypeConverter GetBaseTypeConverter()
        {
            if (PropertyType is null)
            {
                return new TypeConverter();
            }

            TypeConverter? localConverter = null;

            if (Attributes[typeof(TypeConverterAttribute)] is TypeConverterAttribute attribute)
            {
                string converterTypeName = attribute.ConverterTypeName;
                if (!string.IsNullOrEmpty(converterTypeName)
                    && Type.GetType(converterTypeName) is { } converterType
                    && typeof(TypeConverter).IsAssignableFrom(converterType))
                {
                    try
                    {
                        localConverter = (TypeConverter?)Activator.CreateInstance(converterType);
                        if (localConverter is not null)
                        {
                            _refreshState |= Com2PropertyDescriptorRefresh.TypeConverterAttr;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Fail($"Failed to create TypeConverter of type '{attribute.ConverterTypeName}' from Attribute", ex.ToString());
                    }
                }
            }

            // If we didn't get one from the attribute, ask the type descriptor. We don't want to create the value
            // editor for the IDispatch properties because that will create the reference editor.
            localConverter ??= typeof(Oleaut32.IDispatch).IsAssignableFrom(PropertyType)
                ? new Com2IDispatchConverter(this, allowExpand: false)
                : base.Converter;

            return localConverter ?? new TypeConverter();
        }

        private object? GetBaseTypeEditor(Type editorBaseType)
        {
            if (PropertyType is null)
            {
                return null;
            }

            if (Attributes[typeof(EditorAttribute)] is EditorAttribute attribute)
            {
                string? editorTypeName = attribute.EditorBaseTypeName;

                if (!string.IsNullOrEmpty(editorTypeName)
                    && Type.GetType(editorTypeName) is { } attributeEditorBaseType
                    && attributeEditorBaseType == editorBaseType
                    && Type.GetType(attribute.EditorTypeName) is { } type)
                {
                    try
                    {
                        if (Activator.CreateInstance(type) is { } localEditor)
                        {
                            _refreshState |= Com2PropertyDescriptorRefresh.TypeEditorAttr;
                            return localEditor;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Fail($"Failed to create editor of type '{attribute.EditorTypeName}' from Attribute", ex.ToString());
                    }
                }
            }

            return base.GetEditor(editorBaseType);
        }

        /// <summary>
        ///  Gets the value that should be displayed to the user, such as in the Property Browser.
        /// </summary>
        public virtual string? GetDisplayValue(string? defaultValue)
        {
            GetNameItemEvent name = new(defaultValue);
            OnGetDisplayValue(name);

            return name.Name?.ToString();
        }

        /// <summary>
        ///  Retrieves an editor of the requested type.
        /// </summary>
        [RequiresUnreferencedCode($"{TrimmingConstants.EditorRequiresUnreferencedCode} {TrimmingConstants.PropertyDescriptorPropertyTypeMessage}")]
        public override object? GetEditor(Type editorBaseType)
        {
            if (TypeEditorValid)
            {
                return _editor;
            }

            if (PropertyType is null)
            {
                return null;
            }

            if (editorBaseType == typeof(UITypeEditor))
            {
                TypeConverter? converter = null;
                GetTypeConverterAndTypeEditor(ref converter, editorBaseType, ref _editor);

                if (!TypeConverterValid)
                {
                    _converter = converter;
                    SetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeConverter, false);
                }

                SetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeEditor, false);
            }
            else
            {
                _editor = base.GetEditor(editorBaseType);
            }

            return _editor;
        }

        /// <summary>
        ///  Retrieves the current native value of the property on component, invoking the getXXX method.
        ///  An exception in the getXXX method will pass through.
        /// </summary>
        public unsafe object? GetNativeValue(object? component)
        {
            if (component is null)
            {
                return null;
            }

            if (component is ICustomTypeDescriptor descriptor)
            {
                component = descriptor.GetPropertyOwner(this);
            }

            if (component is null || !Marshal.IsComObject(component) || component is not Oleaut32.IDispatch dispatch)
            {
                return null;
            }

            object[] pVarResult = new object[1];
            EXCEPINFO pExcepInfo = default;
            DISPPARAMS dispParams = default;
            Guid guid = Guid.Empty;

            HRESULT hr = dispatch.Invoke(
                DISPID,
                &guid,
                PInvoke.GetThreadLocale(),
                DISPATCH_FLAGS.DISPATCH_PROPERTYGET,
                &dispParams,
                pVarResult,
                &pExcepInfo,
                null);

            if (hr == HRESULT.S_OK || hr == HRESULT.S_FALSE)
            {
                _lastValue = pVarResult[0] is null || Convert.IsDBNull(pVarResult[0]) ? null : pVarResult[0];

                return _lastValue;
            }
            else
            {
                return hr == HRESULT.DISP_E_EXCEPTION
                    ? null
                    : throw new ExternalException(string.Format(SR.DispInvokeFailed, "GetValue", hr), (int)hr);
            }
        }

        /// <summary>
        ///  Checks whether the particular item(s) need refreshing.
        /// </summary>
        private bool GetNeedsRefresh(int mask) => (_refreshState & mask) != 0;

        /// <summary>
        ///  Retrieves the current value of the property on component, invoking the getXXX method. An exception in
        ///  the getXXX method will pass through.
        /// </summary>
        public override object? GetValue(object? component)
        {
            _lastValue = GetNativeValue(component);

            // Do we need to convert the type?
            if (ConvertingNativeType && _lastValue is not null)
            {
                _lastValue = _valueConverter.ConvertNativeToManaged(_lastValue, this);
            }
            else if (_lastValue is not null && _propertyType is not null && _propertyType.IsEnum && _lastValue.GetType().IsPrimitive)
            {
                // We've got to convert the value here. We built the enum but the native object returns values as integers.
                try
                {
                    _lastValue = Enum.ToObject(_propertyType, _lastValue);
                }
                catch
                {
                }
            }

            return _lastValue;
        }

        /// <summary>
        ///  Retrieves the value editor for the property.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///   If a value editor is passed in as a <see cref="TypeConverterAttribute"/>, that value editor will be
        ///   instantiated. If no such attribute was found, a system value editor will be looked for. See
        ///   <see cref="TypeConverter"/> for a description of how system value editors are found. If there is no
        ///   system value editor, null is returned. If the value editor found takes an IEditorSite in its constructor,
        ///   the parameter will be passed in.
        ///  </para>
        /// </remarks>
        public void GetTypeConverterAndTypeEditor([NotNull] ref TypeConverter? typeConverter, Type editorBaseType, ref object? typeEditor)
        {
            // Get the base editor and converter, attributes first.
            TypeConverter? localConverter = typeConverter;
            object? localEditor = typeEditor;

            localConverter ??= GetBaseTypeConverter();
            localEditor ??= GetBaseTypeEditor(editorBaseType);

            // If this is a object, get the value and attempt to create the correct value editor based on that value.
            // We don't do this if the state came from an attribute.
            if (0 == (_refreshState & Com2PropertyDescriptorRefresh.TypeConverterAttr) && PropertyType == typeof(Com2Variant))
            {
                Type editorType = PropertyType;
                object? value = GetValue(TargetObject);
                if (value is not null)
                {
                    editorType = value.GetType();
                }

                ComNativeDescriptor.ResolveVariantTypeConverterAndTypeEditor(value, ref localConverter, editorBaseType, ref localEditor);
            }

            // Now see if someone else would like to serve up a value editor.

            // Unwrap the editor if it's one of ours.
            if (localConverter is Com2PropDescMainConverter mainConverter)
            {
                localConverter = mainConverter.InnerConverter;
            }

            GetTypeConverterAndTypeEditorEvent e = new(localConverter, localEditor);
            OnGetTypeConverterAndTypeEditor(e);
            localConverter = e.TypeConverter;
            localEditor = e.TypeEditor;

            // Just in case one of the handlers removed our editor.
            localConverter ??= GetBaseTypeConverter();
            localEditor ??= GetBaseTypeEditor(editorBaseType);

            // Wrap the value editor in our main value editor, but only if it isn't "TypeConverter" or already a Com2PropDescMainTypeConverter
            Type localConverterType = localConverter.GetType();
            if (localConverterType != typeof(TypeConverter) && localConverterType != typeof(Com2PropDescMainConverter))
            {
                localConverter = new Com2PropDescMainConverter(this, localConverter);
            }

            // Save the values back to the variables.
            typeConverter = localConverter;
            typeEditor = localEditor;
        }

        /// <summary>
        ///  Is the given value equal to the last known value for this object?
        /// </summary>
        public bool IsCurrentValue(object? value) => value == _lastValue || (_lastValue is not null && _lastValue.Equals(value));

        protected void OnCanResetValue(GetBoolValueEvent gvbe) => RaiseGetBoolValueEvent(EventCanResetValue, gvbe);

        protected void OnGetBaseAttributes(GetAttributesEvent e)
        {
            using ValidityScope scope = new(PropertyManager);
            ((GetAttributesEventHandler?)Events[EventGetBaseAttributes])?.Invoke(this, e);
        }

        protected void OnGetDisplayName(GetNameItemEvent gnie) => RaiseGetNameItemEvent(EventGetDisplayName, gnie);

        protected void OnGetDisplayValue(GetNameItemEvent gnie) => RaiseGetNameItemEvent(EventGetDisplayValue, gnie);

        protected void OnGetDynamicAttributes(GetAttributesEvent e)
        {
            using ValidityScope scope = new(PropertyManager);
            ((GetAttributesEventHandler?)Events[EventGetDynamicAttributes])?.Invoke(this, e);
        }

        protected void OnGetIsReadOnly(GetBoolValueEvent gvbe) => RaiseGetBoolValueEvent(EventGetIsReadOnly, gvbe);

        protected void OnGetTypeConverterAndTypeEditor(GetTypeConverterAndTypeEditorEvent e)
        {
            using ValidityScope scope = new(PropertyManager);
            ((GetTypeConverterAndTypeEditorEventHandler?)Events[EventGetTypeConverterAndTypeEditor])?.Invoke(this, e);
        }

        protected void OnResetValue(EventArgs e) => RaiseCom2Event(EventResetValue, e);

        protected void OnShouldSerializeValue(GetBoolValueEvent gvbe) => RaiseGetBoolValueEvent(EventShouldSerializeValue, gvbe);

        protected void OnShouldRefresh(GetRefreshStateEvent gvbe) => RaiseGetBoolValueEvent(EventShouldRefresh, gvbe);

        private void RaiseGetBoolValueEvent(object key, GetBoolValueEvent e)
        {
            using ValidityScope scope = new(PropertyManager);
            ((GetBoolValueEventHandler?)Events[key])?.Invoke(this, e);
        }

        private void RaiseCom2Event(object key, EventArgs e)
        {
            using ValidityScope scope = new(PropertyManager);
            ((Com2EventHandler?)Events[key])?.Invoke(this, e);
        }

        private void RaiseGetNameItemEvent(object key, GetNameItemEvent e)
        {
            using ValidityScope scope = new(PropertyManager);
            ((GetNameItemEventHandler?)Events[key])?.Invoke(this, e);
        }

        public override void ResetValue(object? component)
        {
            if (component is ICustomTypeDescriptor descriptor)
            {
                component = descriptor.GetPropertyOwner(this);
            }

            if (component == TargetObject)
            {
                OnResetValue(EventArgs.Empty);
            }
        }

        /// <summary>
        ///  Sets whether the particular item(s) need refreshing.
        /// </summary>
        internal void SetNeedsRefresh(int mask, bool value)
        {
            if (value)
            {
                _refreshState |= mask;
            }
            else
            {
                _refreshState &= ~mask;
            }
        }

        public override unsafe void SetValue(object? component, object? value)
        {
            if (_readOnly)
            {
                throw new NotSupportedException(string.Format(SR.COM2ReadonlyProperty, Name));
            }

            object? owner = component;
            if (owner is ICustomTypeDescriptor descriptor)
            {
                owner = descriptor.GetPropertyOwner(this);
            }

            if (owner is null || !Marshal.IsComObject(owner) || owner is not Oleaut32.IDispatch)
            {
                return;
            }

            // Do we need to convert the type?
            if (_valueConverter is not null)
            {
                bool cancel = false;
                value = _valueConverter.ConvertManagedToNative(value, this, ref cancel);
                if (cancel)
                {
                    return;
                }
            }

            Oleaut32.IDispatch dispatch = (Oleaut32.IDispatch)owner;

            EXCEPINFO excepInfo = default(EXCEPINFO);
            Ole32.DispatchID namedArg = Ole32.DispatchID.PROPERTYPUT;
            DISPPARAMS dispParams = new()
            {
                cArgs = 1,
                cNamedArgs = 1,
                rgdispidNamedArgs = (int*)&namedArg
            };

            using VARIANT variant = default(VARIANT);
            Marshal.GetNativeVariantForObject(value, (IntPtr)(&variant));
            dispParams.rgvarg = &variant;
            Guid guid = Guid.Empty;
            HRESULT hr = dispatch.Invoke(
                DISPID,
                &guid,
                PInvoke.GetThreadLocale(),
                DISPATCH_FLAGS.DISPATCH_PROPERTYPUT,
                &dispParams,
                null,
                &excepInfo,
                null);

            string? errorText = null;
            if (hr == HRESULT.DISP_E_EXCEPTION && excepInfo.scode != 0)
            {
                hr = (HRESULT)excepInfo.scode;
                if (!excepInfo.bstrDescription.IsNull)
                {
                    errorText = excepInfo.bstrDescription.ToString();
                }
            }

            if (hr == HRESULT.E_ABORT || hr == HRESULT.OLE_E_PROMPTSAVECANCELLED)
            {
                return;
            }
            else if (hr == HRESULT.S_OK || hr == HRESULT.S_FALSE)
            {
                OnValueChanged(component, EventArgs.Empty);
                _lastValue = value;
            }

            if (dispatch is Oleaut32.ISupportErrorInfo iSupportErrorInfo)
            {
                guid = typeof(Oleaut32.IDispatch).GUID;
                if (iSupportErrorInfo.InterfaceSupportsErrorInfo(&guid) == HRESULT.S_OK)
                {
                    Oleaut32.GetErrorInfo(out WinFormsComWrappers.ErrorInfoWrapper? errorInfo);

                    if (errorInfo is not null)
                    {
                        if (errorInfo.GetDescription(out string? description))
                        {
                            errorText = description;
                        }

                        errorInfo.Dispose();
                    }
                }
            }
            else if (errorText is null)
            {
                char[] buffer = ArrayPool<char>.Shared.Rent(256 + 1);

                fixed (char* b = buffer)
                {
                    uint result = PInvoke.FormatMessage(
                        FORMAT_MESSAGE_OPTIONS.FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_OPTIONS.FORMAT_MESSAGE_IGNORE_INSERTS,
                        null,
                        (uint)hr,
                        PInvoke.GetThreadLocale(),
                        b,
                        255,
                        null);

                    errorText = result == 0
                        ? string.Format(CultureInfo.CurrentCulture, SR.DispInvokeFailed, "SetValue", hr)
                        : buffer.AsSpan()[..(int)result].TrimEnd(CharacterConstants.NewLine).ToString();
                }

                ArrayPool<char>.Shared.Return(buffer);
            }

            throw new ExternalException(errorText, (int)hr);
        }

        /// <summary>
        ///  Indicates whether the value of this property needs to be persisted. In
        ///  other words, it indicates whether the state of the property is distinct
        ///  from when the component is first instantiated. If there is a default
        ///  value specified in this PropertyDescriptor, it will be compared against the
        ///  property's current value to determine this.  If there isn't, the
        ///  shouldPersistXXX method is looked for and invoked if found.  If both
        ///  these routes fail, true will be returned.
        ///
        ///  If this returns false, a tool should not persist this property's value.
        /// </summary>
        public override bool ShouldSerializeValue(object component)
        {
            GetBoolValueEvent gbv = new GetBoolValueEvent(false);
            OnShouldSerializeValue(gbv);
            return gbv.Value;
        }
    }

    internal class GetAttributesEvent : EventArgs
    {
        private readonly List<Attribute> _attributeList;

        public GetAttributesEvent(List<Attribute> attributeList) => _attributeList = attributeList;

        public void Add(Attribute attribute) => _attributeList.Add(attribute);
    }

    internal delegate void Com2EventHandler(Com2PropertyDescriptor sender, EventArgs e);

    internal delegate void GetAttributesEventHandler(Com2PropertyDescriptor sender, GetAttributesEvent gaevent);

    internal class GetNameItemEvent : EventArgs
    {
        public GetNameItemEvent(object? defaultName) => Name = defaultName;

        public object? Name { get; set; }

        public string NameString => Name?.ToString() ?? string.Empty;
    }

    internal delegate void GetNameItemEventHandler(Com2PropertyDescriptor sender, GetNameItemEvent gnievent);

    internal class GetBoolValueEvent : EventArgs
    {
        public GetBoolValueEvent(bool defaultValue) => Value = defaultValue;
        public bool Value { get; set; }
    }

    internal delegate void GetBoolValueEventHandler(Com2PropertyDescriptor sender, GetBoolValueEvent gbeevent);

    internal class GetRefreshStateEvent : GetBoolValueEvent
    {
        public GetRefreshStateEvent(bool defaultValue) : base(defaultValue) { }
    }

    internal delegate void GetTypeConverterAndTypeEditorEventHandler(Com2PropertyDescriptor sender, GetTypeConverterAndTypeEditorEvent e);

    internal class GetTypeConverterAndTypeEditorEvent : EventArgs
    {
        public GetTypeConverterAndTypeEditorEvent(TypeConverter? typeConverter, object? typeEditor)
        {
            TypeEditor = typeEditor;
            TypeConverter = typeConverter;
        }

        public TypeConverter? TypeConverter { get; set; }

        public object? TypeEditor { get; set; }
    }
}

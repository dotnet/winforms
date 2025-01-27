// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Diagnostics.Debug;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

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
[RequiresUnreferencedCode(ComNativeDescriptor.ComTypeDescriptorsMessage + " Uses ComNativeDescriptor which is not trim-compatible.")]
internal unsafe partial class Com2PropertyDescriptor : PropertyDescriptor, ICloneable
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
    private static readonly object s_eventGetBaseAttributes = new();
    private static readonly object s_eventGetDynamicAttributes = new();
    private static readonly object s_eventShouldRefresh = new();
    private static readonly object s_eventGetDisplayName = new();
    private static readonly object s_eventGetDisplayValue = new();
    private static readonly object s_eventGetIsReadOnly = new();
    private static readonly object s_eventGetTypeConverterAndTypeEditor = new();
    private static readonly object s_eventShouldSerializeValue = new();
    private static readonly object s_eventCanResetValue = new();
    private static readonly object s_eventResetValue = new();

    private static Guid GUID_COLOR { get; } = new("{66504301-BE0F-101A-8BBB-00AA00300CAB}");

    private readonly Com2DataTypeToManagedDataTypeConverter? _valueConverter;

    public Com2PropertyDescriptor(
        int dispid,
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
                if (guid.Equals(GUID_COLOR))
                {
                    _valueConverter = new Com2ColorConverter();
                }
                else if (guid.Equals(IID.GetRef<IFontDisp>()) || guid.Equals(IID.GetRef<IFont>()))
                {
                    _valueConverter = new Com2FontConverter();
                }
                else if (guid.Equals(IID.GetRef<IPictureDisp>()) || guid.Equals(IID.GetRef<IPicture>()))
                {
                    _valueConverter = new Com2PictureConverter(this);
                }
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

        if (CanShow && (propertyType == typeof(object) || (_valueConverter is null && propertyType == typeof(IDispatch.Interface))))
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
                return _baseAttributes ??= [];
            }

            SetNeedsRefresh(Com2PropertyDescriptorRefresh.BaseAttributes, false);

            int baseCount = _baseAttributes is null ? 0 : _baseAttributes.Length;

            List<Attribute> attributes = [];

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
                _baseAttributes = [];
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
                newAttributes = new(AttributeArray)
                {
                    new BrowsableAttribute(false)
                };
            }
            else if (_hrHidden)
            {
                // Check to see if the get still fails.
                using var dispatch = ComHelpers.TryGetComScope<IDispatch>(TargetObject, out HRESULT hr);
                if (hr.Succeeded)
                {
                    hr = ComNativeDescriptor.GetPropertyValue(dispatch, DISPID, out _);

                    // If not, go ahead and make this a browsable item.
                    if (hr.Succeeded)
                    {
                        // Make it browsable.
                        newAttributes = new(AttributeArray)
                        {
                            new BrowsableAttribute(true)
                        };

                        _hrHidden = false;
                    }
                }
            }

            InAttributeQuery = true;
            try
            {
                // Demand get any extended attributes
                List<Attribute> attributeList = [];

                OnGetDynamicAttributes(new GetAttributesEvent(attributeList));

                if (attributeList.Count > 0)
                {
                    newAttributes ??= [..AttributeArray];

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

    // Historically this was always an internal interface
    public sealed override Type ComponentType => typeof(IDispatch.Interface);

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
    public int DISPID { get; }

    public override string DisplayName
    {
        get
        {
            if (_displayName is null || GetNeedsRefresh(Com2PropertyDescriptorRefresh.DisplayName))
            {
                GetNameItemEvent getNameEvent = new(base.DisplayName);
                OnGetDisplayName(getNameEvent);
                _displayName = getNameEvent.NameString;
                SetNeedsRefresh(Com2PropertyDescriptorRefresh.DisplayName, false);
            }

            return _displayName;
        }
    }

    protected EventHandlerList Events => _events ??= new EventHandlerList();

    protected bool InAttributeQuery { get; private set; }

    public override bool IsReadOnly
    {
        get
        {
            if (!_baseReadOnly && GetNeedsRefresh(Com2PropertyDescriptorRefresh.ReadOnly))
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

    public override Type? PropertyType
         // Replace the type with the mapped converter type
         => _valueConverter is not null ? _valueConverter.ManagedType : _propertyType;
#pragma warning restore CS8764

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
        add => Events.AddHandler(s_eventCanResetValue, value);
        remove => Events.RemoveHandler(s_eventCanResetValue, value);
    }

    public event GetAttributesEventHandler QueryGetBaseAttributes
    {
        add => Events.AddHandler(s_eventGetBaseAttributes, value);
        remove => Events.RemoveHandler(s_eventGetBaseAttributes, value);
    }

    public event GetAttributesEventHandler QueryGetDynamicAttributes
    {
        add => Events.AddHandler(s_eventGetDynamicAttributes, value);
        remove => Events.RemoveHandler(s_eventGetDynamicAttributes, value);
    }

    public event GetNameItemEventHandler QueryGetDisplayName
    {
        add => Events.AddHandler(s_eventGetDisplayName, value);
        remove => Events.RemoveHandler(s_eventGetDisplayName, value);
    }

    public event GetNameItemEventHandler QueryGetDisplayValue
    {
        add => Events.AddHandler(s_eventGetDisplayValue, value);
        remove => Events.RemoveHandler(s_eventGetDisplayValue, value);
    }

    public event GetBoolValueEventHandler QueryGetIsReadOnly
    {
        add => Events.AddHandler(s_eventGetIsReadOnly, value);
        remove => Events.RemoveHandler(s_eventGetIsReadOnly, value);
    }

    public event GetTypeConverterAndTypeEditorEventHandler QueryGetTypeConverterAndTypeEditor
    {
        add => Events.AddHandler(s_eventGetTypeConverterAndTypeEditor, value);
        remove => Events.RemoveHandler(s_eventGetTypeConverterAndTypeEditor, value);
    }

    public event Com2EventHandler QueryResetValue
    {
        add => Events.AddHandler(s_eventResetValue, value);
        remove => Events.RemoveHandler(s_eventResetValue, value);
    }

    public event GetBoolValueEventHandler QueryShouldSerializeValue
    {
        add => Events.AddHandler(s_eventShouldSerializeValue, value);
        remove => Events.RemoveHandler(s_eventShouldSerializeValue, value);
    }

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

    protected sealed override AttributeCollection CreateAttributeCollection() => new(AttributeArray);

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
        localConverter ??= typeof(IDispatch).IsAssignableFrom(PropertyType)
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
    public string? GetDisplayValue(string? defaultValue)
    {
        GetNameItemEvent name = new(defaultValue);
        OnGetDisplayValue(name);

        return name.Name?.ToString();
    }

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
    ///  Retrieves the current native value of the property on the given component. You must dispose of the
    ///  returned <see cref="VARIANT"/> after using it.
    /// </summary>
    internal unsafe VARIANT GetNativeValue(object? component)
    {
        if (component is null)
        {
            return VARIANT.Empty;
        }

        if (component is ICustomTypeDescriptor descriptor)
        {
            component = descriptor.GetPropertyOwner(this);
        }

        using var dispatch = ComHelpers.TryGetComScope<IDispatch>(component, out HRESULT hr);
        if (hr.Failed)
        {
            return VARIANT.Empty;
        }

        VARIANT nativeValue = default;
        hr = dispatch.Value->TryGetProperty(DISPID, &nativeValue, PInvokeCore.GetThreadLocale());

        if (hr != HRESULT.S_OK && hr != HRESULT.S_FALSE)
        {
            Debug.Fail($"Failed to get property: {hr}");
            return VARIANT.Empty;
        }

        return nativeValue;
    }

    /// <summary>
    ///  Checks whether the particular item(s) need refreshing.
    /// </summary>
    private bool GetNeedsRefresh(int mask) => (_refreshState & mask) != 0;

    public override object? GetValue(object? component)
    {
        using VARIANT nativeValue = GetNativeValue(component);

        // Do we need to convert the type?
        if (ConvertingNativeType && !nativeValue.IsEmpty)
        {
            return _valueConverter.ConvertNativeToManaged(nativeValue, this);
        }

        try
        {
            _lastValue = nativeValue.ToObject();
        }
        catch (Exception ex)
        {
            Debug.Fail($"Could not convert the native value to a .NET object: {ex.Message}");
        }

        if (_lastValue is not null && _propertyType is not null && _propertyType.IsEnum && _lastValue.GetType().IsPrimitive)
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
        if ((_refreshState & Com2PropertyDescriptorRefresh.TypeConverterAttr) == 0 && PropertyType == typeof(Com2Variant))
        {
            // The results were never used here, they probably were intended to be used.
            // Without specific scenarios, leaving the access as is to avoid breaking changes.
            _ = PropertyType;
            object? value = GetValue(TargetObject);
            if (value is not null)
            {
                _ = value.GetType();
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
    internal bool IsLastKnownValue(object? value) => value == _lastValue;

    protected void OnCanResetValue(GetBoolValueEvent e) => RaiseGetBoolValueEvent(s_eventCanResetValue, e);

    protected void OnGetBaseAttributes(GetAttributesEvent e)
    {
        using ValidityScope scope = new(PropertyManager);
        ((GetAttributesEventHandler?)Events[s_eventGetBaseAttributes])?.Invoke(this, e);
    }

    protected void OnGetDisplayName(GetNameItemEvent e) => RaiseGetNameItemEvent(s_eventGetDisplayName, e);

    protected void OnGetDisplayValue(GetNameItemEvent e) => RaiseGetNameItemEvent(s_eventGetDisplayValue, e);

    protected void OnGetDynamicAttributes(GetAttributesEvent e)
    {
        using ValidityScope scope = new(PropertyManager);
        ((GetAttributesEventHandler?)Events[s_eventGetDynamicAttributes])?.Invoke(this, e);
    }

    protected void OnGetIsReadOnly(GetBoolValueEvent e) => RaiseGetBoolValueEvent(s_eventGetIsReadOnly, e);

    protected void OnGetTypeConverterAndTypeEditor(GetTypeConverterAndTypeEditorEvent e)
    {
        using ValidityScope scope = new(PropertyManager);
        ((GetTypeConverterAndTypeEditorEventHandler?)Events[s_eventGetTypeConverterAndTypeEditor])?.Invoke(this, e);
    }

    protected void OnResetValue(EventArgs e) => RaiseCom2Event(s_eventResetValue, e);

    protected void OnShouldSerializeValue(GetBoolValueEvent e) => RaiseGetBoolValueEvent(s_eventShouldSerializeValue, e);

    protected void OnShouldRefresh(GetRefreshStateEvent e) => RaiseGetBoolValueEvent(s_eventShouldRefresh, e);

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

        using var dispatch = ComHelpers.TryGetComScope<IDispatch>(owner, out HRESULT hr);
        if (hr.Failed)
        {
            return;
        }

        VARIANT nativeValue = default;

        // Do we need to convert the type?
        if (_valueConverter is not null)
        {
            bool cancel = false;
            nativeValue = _valueConverter.ConvertManagedToNative(value, this, ref cancel);
            if (cancel)
            {
                return;
            }
        }
        else
        {
            nativeValue = VARIANT.FromObject(value);
        }

        hr = dispatch.Value->SetPropertyValue(DISPID, nativeValue, out string? errorText);
        nativeValue.Dispose();

        if (hr == HRESULT.S_OK || hr == HRESULT.S_FALSE)
        {
            OnValueChanged(component, EventArgs.Empty);
            _lastValue = value;
            return;
        }

        if (hr == HRESULT.E_ABORT || hr == HRESULT.OLE_E_PROMPTSAVECANCELLED)
        {
            // Cancelled checkout, etc.
            return;
        }

        HRESULT setError = hr;

        using var iSupportErrorInfo = dispatch.TryQuery<ISupportErrorInfo>(out hr);
        if (hr.Succeeded)
        {
            if (iSupportErrorInfo.Value->InterfaceSupportsErrorInfo(IID.Get<IDispatch>()).Succeeded)
            {
                using ComScope<IErrorInfo> errorInfo = new(null);
                hr = PInvoke.GetErrorInfo(0, errorInfo);

                if (hr.Succeeded && hr != HRESULT.S_FALSE && !errorInfo.IsNull)
                {
                    using BSTR description = default;
                    hr = errorInfo.Value->GetDescription(&description);

                    if (hr.Succeeded)
                    {
                        errorText = description.ToString();
                    }
                }
            }
        }
        else if (string.IsNullOrEmpty(errorText))
        {
            using BufferScope<char> buffer = new(256 + 1);

            fixed (char* b = buffer)
            {
                uint result = PInvoke.FormatMessage(
                    FORMAT_MESSAGE_OPTIONS.FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_OPTIONS.FORMAT_MESSAGE_IGNORE_INSERTS,
                    null,
                    (uint)hr,
                    PInvokeCore.GetThreadLocale(),
                    b,
                    (uint)buffer.Length - 2,
                    null);

                errorText = result == 0
                    ? string.Format(CultureInfo.CurrentCulture, SR.DispInvokeFailed, "SetValue", setError)
                    : buffer[..(int)result].TrimEnd(CharacterConstants.NewLine).ToString();
            }
        }

        throw new ExternalException(errorText, (int)setError);
    }

    public override bool ShouldSerializeValue(object component)
    {
        GetBoolValueEvent e = new(defaultValue: false);
        OnShouldSerializeValue(e);
        return e.Value;
    }
}

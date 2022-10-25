// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

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
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This class wraps a com native property in a property descriptor.
    ///  It maintains all information relative to the basic (e.g. ITypeInfo)
    ///  information about the member dispid function, and converts that info
    ///  to meaningful managed code information.
    ///
    ///  It also allows other objects to register listeners to add extended
    ///  information at runtime such as attributes of TypeConverters.
    /// </summary>
    internal partial class Com2PropertyDescriptor : PropertyDescriptor, ICloneable
    {
        private EventHandlerList events;

        /// <summary>
        ///  Is this guy read only?
        /// </summary>
        private readonly bool baseReadOnly;
        private bool readOnly;

        /// <summary>
        ///  The resolved native type -> clr type
        /// </summary>
        private readonly Type propertyType;

        /// <summary>
        ///  The dispid. This is also in a DispIDAttribute, but we
        ///  need it a lot.
        /// </summary>
        private readonly Ole32.DispatchID dispid;

        private TypeConverter converter;
        private object editor;

        /// <summary>
        ///  The current display name to show for this property
        /// </summary>
        private string displayName;

        /// <summary>
        ///  This is any extra data needed.  For IDispatch types, it's the GUID of
        ///  the interface, etc.
        /// </summary>
        private readonly object typeData;

        /// <summary>
        ///  Keeps track of which data members need to be refreshed.
        /// </summary>
        private int refreshState;

        /// <summary>
        ///  Our properties manager
        /// </summary>
        private Com2Properties com2props;

        /// <summary>
        ///  Our original baseline properties
        /// </summary>
        private Attribute[] baseAttrs;

        /// <summary>
        ///  Our cached last value -- this is only
        ///  for checking if we should ask for a display value
        /// </summary>
        private object lastValue;

        /// <summary>
        ///  For Object and dispatch types, we hide them by default.
        /// </summary>
        private readonly bool typeHide;

        /// <summary>
        ///  Set if the metadata causes this property to always be hidden
        /// </summary>
        private readonly bool canShow;

        /// <summary>
        ///  This property is hidden because its get didn't return S_OK
        /// </summary>
        private bool hrHidden;

        /// <summary>
        ///  Set if we are in the process of asking handlers for attributes
        /// </summary>
        private bool inAttrQuery;

        /// <summary>
        ///  Our event signatures.
        /// </summary>
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

        private static readonly Guid GUID_COLOR = new Guid("{66504301-BE0F-101A-8BBB-00AA00300CAB}");

        /// <summary>
        ///  Our map of native types that we can map to managed types for editors
        /// </summary>
        private static readonly IDictionary oleConverters = new SortedList
        {
            [GUID_COLOR] = typeof(Com2ColorConverter),
            [typeof(Ole32.IFontDisp).GUID] = typeof(Com2FontConverter),
            [typeof(Ole32.IFont).GUID] = typeof(Com2FontConverter),
            [typeof(Ole32.IPictureDisp).GUID] = typeof(Com2PictureConverter),
            [typeof(Ole32.IPicture).GUID] = typeof(Com2PictureConverter)
        };

        /// <summary>
        ///  Should we convert our type?
        /// </summary>
        private readonly Com2DataTypeToManagedDataTypeConverter valueConverter;

        /// <summary>
        ///  Ctor.
        /// </summary>
        public Com2PropertyDescriptor(Ole32.DispatchID dispid, string name, Attribute[] attrs, bool readOnly, Type propType, object typeData, bool hrHidden)
            : base(name, attrs)
        {
            baseReadOnly = readOnly;
            this.readOnly = readOnly;

            baseAttrs = attrs;
            SetNeedsRefresh(Com2PropertyDescriptorRefresh.BaseAttributes, true);

            this.hrHidden = hrHidden;

            // readonly to begin with are always read only
            SetNeedsRefresh(Com2PropertyDescriptorRefresh.ReadOnly, readOnly);

            propertyType = propType;

            this.dispid = dispid;

            if (typeData is not null)
            {
                this.typeData = typeData;
                if (typeData is Com2Enum)
                {
                    converter = new Com2EnumConverter((Com2Enum)typeData);
                }
                else if (typeData is Guid)
                {
                    valueConverter = CreateOleTypeConverter((Type)oleConverters[(Guid)typeData]);
                }
            }

            // check if this thing is hidden from metadata
            canShow = true;

            if (attrs is not null)
            {
                for (int i = 0; i < attrs.Length; i++)
                {
                    if (attrs[i].Equals(BrowsableAttribute.No) && !hrHidden)
                    {
                        canShow = false;
                        break;
                    }
                }
            }

            if (canShow && (propType == typeof(object) || (valueConverter is null && propType == typeof(Oleaut32.IDispatch))))
            {
                typeHide = true;
            }
        }

        protected Attribute[] BaseAttributes
        {
            get
            {
                if (GetNeedsRefresh(Com2PropertyDescriptorRefresh.BaseAttributes))
                {
                    SetNeedsRefresh(Com2PropertyDescriptorRefresh.BaseAttributes, false);

                    int baseCount = baseAttrs is null ? 0 : baseAttrs.Length;

                    ArrayList attrList = new ArrayList();

                    if (baseCount != 0)
                    {
                        attrList.AddRange(baseAttrs);
                    }

                    OnGetBaseAttributes(new GetAttributesEvent(attrList));

                    if (attrList.Count != baseCount)
                    {
                        baseAttrs = new Attribute[attrList.Count];
                    }

                    if (baseAttrs is not null)
                    {
                        attrList.CopyTo(baseAttrs, 0);
                    }
                    else
                    {
                        baseAttrs = Array.Empty<Attribute>();
                    }
                }

                return baseAttrs;
            }
            set
            {
                baseAttrs = value;
            }
        }

        /// <summary>
        ///  Attributes
        /// </summary>
        public override AttributeCollection Attributes
        {
            get
            {
                if (AttributesValid || InAttrQuery)
                {
                    return base.Attributes;
                }

                // restore our base attributes
                AttributeArray = BaseAttributes;

                ArrayList newAttributes = null;

                // if we are forcing a hide
                if (typeHide && canShow)
                {
                    newAttributes ??= new ArrayList(AttributeArray);

                    newAttributes.Add(new BrowsableAttribute(false));
                }
                else if (hrHidden)
                {
                    // check to see if the get still fails
                    object target = TargetObject;
                    if (target is not null)
                    {
                        HRESULT hr = ComNativeDescriptor.GetPropertyValue(target, dispid, new object[1]);

                        // if not, go ahead and make this a browsable item
                        if (hr.Succeeded)
                        {
                            // make it browsable
                            newAttributes ??= new ArrayList(AttributeArray);

                            newAttributes.Add(new BrowsableAttribute(true));
                            hrHidden = false;
                        }
                    }
                }

                inAttrQuery = true;
                try
                {
                    // demand get any extended attributes
                    ArrayList attrList = new ArrayList();

                    OnGetDynamicAttributes(new GetAttributesEvent(attrList));

                    Attribute ma;

                    if (attrList.Count != 0 && newAttributes is null)
                    {
                        newAttributes = new ArrayList(AttributeArray);
                    }

                    // push any new attributes into the base type
                    for (int i = 0; i < attrList.Count; i++)
                    {
                        ma = (Attribute)attrList[i];
                        newAttributes.Add(ma);
                    }
                }
                finally
                {
                    inAttrQuery = false;
                }

                // these are now valid.
                SetNeedsRefresh(Com2PropertyDescriptorRefresh.Attributes, false);

                // If we reconfigured attributes, then poke the new set back in.
                //
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
        ///  Checks if the attributes are valid.  Asks any clients if they
        ///  would like attributes required.
        /// </summary>
        protected bool AttributesValid
        {
            get
            {
                bool currentRefresh = !GetNeedsRefresh(Com2PropertyDescriptorRefresh.Attributes);

                return currentRefresh;
            }
        }

        /// <summary>
        ///  Checks if this item can be shown.
        /// </summary>
        public bool CanShow
        {
            get
            {
                return canShow;
            }
        }

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
                    return converter;
                }

                object typeEd = null;

                GetTypeConverterAndTypeEditor(ref converter, typeof(UITypeEditor), ref typeEd);

                if (!TypeEditorValid)
                {
                    editor = typeEd;
                    SetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeEditor, false);
                }

                SetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeConverter, false);

                return converter;
            }
        }

        /// <summary>
        ///  Retrieves whether this component is applying a type conversion...
        /// </summary>
        public bool ConvertingNativeType
        {
            get
            {
                return (valueConverter is not null);
            }
        }

        /// <summary>
        ///  Retrieves the default value for this property.
        /// </summary>
        protected virtual object DefaultValue
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        ///  Retrieves the DISPID for this item
        /// </summary>
        public Ole32.DispatchID DISPID
        {
            get
            {
                return dispid;
            }
        }

        /// <summary>
        ///  Gets the friendly name that should be displayed to the user in a window like
        ///  the Property Browser.
        /// </summary>
        public override string DisplayName
        {
            get
            {
                if (!DisplayNameValid)
                {
                    GetNameItemEvent gni = new GetNameItemEvent(base.DisplayName);
                    OnGetDisplayName(gni);
                    displayName = gni.NameString;
                    SetNeedsRefresh(Com2PropertyDescriptorRefresh.DisplayName, false);
                }

                return displayName;
            }
        }

        /// <summary>
        ///  Checks if the property display name is valid
        ///  asks clients if they would like display name required.
        /// </summary>
        protected bool DisplayNameValid
        {
            get
            {
                bool currentRefresh = !(displayName is null || GetNeedsRefresh(Com2PropertyDescriptorRefresh.DisplayName));

                return currentRefresh;
            }
        }

        protected EventHandlerList Events
        {
            get
            {
                events ??= new EventHandlerList();

                return events;
            }
        }

        protected bool InAttrQuery
        {
            get
            {
                return inAttrQuery;
            }
        }

        /// <summary>
        ///  Indicates whether this property is read only.
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                if (!ReadOnlyValid)
                {
                    readOnly |= (Attributes[typeof(ReadOnlyAttribute)].Equals(ReadOnlyAttribute.Yes));
                    GetBoolValueEvent gbv = new GetBoolValueEvent(readOnly);
                    OnGetIsReadOnly(gbv);
                    readOnly = gbv.Value;
                    SetNeedsRefresh(Com2PropertyDescriptorRefresh.ReadOnly, false);
                }

                return readOnly;
            }
        }

        internal Com2Properties PropertyManager
        {
            get
            {
                return com2props;
            }
            set
            {
                com2props = value;
            }
        }

        /// <summary>
        ///  Retrieves the type of the property.
        /// </summary>
        public override Type PropertyType
        {
            get
            {
                // replace the type with the mapped converter type
                if (valueConverter is not null)
                {
                    return valueConverter.ManagedType;
                }
                else
                {
                    return propertyType;
                }
            }
        }

        /// <summary>
        ///  Checks if the read only state is valid.
        ///  Asks clients if they would like read-only required.
        /// </summary>
        protected bool ReadOnlyValid
        {
            get
            {
                if (baseReadOnly)
                {
                    return true;
                }

                bool currentRefresh = !GetNeedsRefresh(Com2PropertyDescriptorRefresh.ReadOnly);

                return currentRefresh;
            }
        }

        /// <summary>
        ///  Gets the Object that this descriptor was created for.
        ///  May be null if the Object's ref has died.
        /// </summary>
        public virtual object TargetObject
        {
            get
            {
                if (com2props is not null)
                {
                    return com2props.TargetObject;
                }

                return null;
            }
        }

        protected bool TypeConverterValid
        {
            get
            {
                bool currentRefresh = !(converter is null || GetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeConverter));

                return currentRefresh;
            }
        }

        protected bool TypeEditorValid
        {
            get
            {
                bool currentRefresh = !(editor is null || GetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeEditor));

                return currentRefresh;
            }
        }

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
            if (component is ICustomTypeDescriptor)
            {
                component = ((ICustomTypeDescriptor)component).GetPropertyOwner(this);
            }

            if (component == TargetObject)
            {
                GetBoolValueEvent gbv = new GetBoolValueEvent(false);
                OnCanResetValue(gbv);
                return gbv.Value;
            }

            return false;
        }

        public object Clone()
        {
            return new Com2PropertyDescriptor(dispid, Name, (Attribute[])baseAttrs.Clone(), readOnly, propertyType, typeData, hrHidden);
        }

        /// <summary>
        ///  Creates a converter Object, first by looking for a ctor with a Com2PropertyDescriptor
        ///  parameter, then using the default ctor if it is not found.
        /// </summary>
        private Com2DataTypeToManagedDataTypeConverter CreateOleTypeConverter(Type t)
        {
            if (t is null)
            {
                return null;
            }

            ConstructorInfo ctor = t.GetConstructor(new Type[] { typeof(Com2PropertyDescriptor) });
            Com2DataTypeToManagedDataTypeConverter converter;
            if (ctor is not null)
            {
                converter = (Com2DataTypeToManagedDataTypeConverter)ctor.Invoke(new object[] { this });
            }
            else
            {
                converter = (Com2DataTypeToManagedDataTypeConverter)Activator.CreateInstance(t);
            }

            return converter;
        }

        /// <summary>
        ///  Creates an instance of the member attribute collection. This can
        ///  be overriden by subclasses to return a subclass of AttributeCollection.
        /// </summary>
        protected override AttributeCollection CreateAttributeCollection()
        {
            return new AttributeCollection(AttributeArray);
        }

        private TypeConverter GetBaseTypeConverter()
        {
            if (PropertyType is null)
            {
                return new TypeConverter();
            }

            TypeConverter localConverter = null;

            TypeConverterAttribute attr = (TypeConverterAttribute)Attributes[typeof(TypeConverterAttribute)];
            if (attr is not null)
            {
                string converterTypeName = attr.ConverterTypeName;
                if (converterTypeName is not null && converterTypeName.Length > 0)
                {
                    Type converterType = Type.GetType(converterTypeName);
                    if (converterType is not null && typeof(TypeConverter).IsAssignableFrom(converterType))
                    {
                        try
                        {
                            localConverter = (TypeConverter)Activator.CreateInstance(converterType);
                            if (localConverter is not null)
                            {
                                refreshState |= Com2PropertyDescriptorRefresh.TypeConverterAttr;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.Fail("Failed to create TypeConverter of type '" + attr.ConverterTypeName + "' from Attribute", ex.ToString());
                        }
                    }
                }
            }

            // if we didn't get one from the attribute, ask the type descriptor
            if (localConverter is null)
            {
                // we don't want to create the value editor for the IDispatch props because
                // that will create the reference editor.  We don't want that guy!
                //
                if (!typeof(Oleaut32.IDispatch).IsAssignableFrom(PropertyType))
                {
                    localConverter = base.Converter;
                }
                else
                {
                    localConverter = new Com2IDispatchConverter(this, false);
                }
            }

            localConverter ??= new TypeConverter();

            return localConverter;
        }

        private object GetBaseTypeEditor(Type editorBaseType)
        {
            if (PropertyType is null)
            {
                return null;
            }

            object localEditor = null;
            EditorAttribute attr = (EditorAttribute)Attributes[typeof(EditorAttribute)];
            if (attr is not null)
            {
                string editorTypeName = attr.EditorBaseTypeName;

                if (editorTypeName is not null && editorTypeName.Length > 0)
                {
                    Type attrEditorBaseType = Type.GetType(editorTypeName);
                    if (attrEditorBaseType is not null && attrEditorBaseType == editorBaseType)
                    {
                        Type type = Type.GetType(attr.EditorTypeName);
                        if (type is not null)
                        {
                            try
                            {
                                localEditor = Activator.CreateInstance(type);
                                if (localEditor is not null)
                                {
                                    refreshState |= Com2PropertyDescriptorRefresh.TypeEditorAttr;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.Fail("Failed to create editor of type '" + attr.EditorTypeName + "' from Attribute", ex.ToString());
                            }
                        }
                    }
                }
            }

            localEditor ??= base.GetEditor(editorBaseType);

            return localEditor;
        }

        /// <summary>
        ///  Gets the value that should be displayed to the user, such as in
        ///  the Property Browser.
        /// </summary>
        public virtual string GetDisplayValue(string defaultValue)
        {
            GetNameItemEvent nie = new GetNameItemEvent(defaultValue);
            OnGetDisplayValue(nie);

            string str = (nie.Name?.ToString());
            return str;
        }

        /// <summary>
        ///  Retrieves an editor of the requested type.
        /// </summary>
        [RequiresUnreferencedCode(TrimmingConstants.EditorRequiresUnreferencedCode + " " + TrimmingConstants.PropertyDescriptorPropertyTypeMessage)]
        public override object GetEditor(Type editorBaseType)
        {
            if (TypeEditorValid)
            {
                return editor;
            }

            if (PropertyType is null)
            {
                return null;
            }

            if (editorBaseType == typeof(UITypeEditor))
            {
                TypeConverter c = null;
                GetTypeConverterAndTypeEditor(ref c, editorBaseType, ref editor);

                if (!TypeConverterValid)
                {
                    converter = c;
                    SetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeConverter, false);
                }

                SetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeEditor, false);
            }
            else
            {
                editor = base.GetEditor(editorBaseType);
            }

            return editor;
        }

        /// <summary>
        ///  Retrieves the current native value of the property on component,
        ///  invoking the getXXX method.  An exception in the getXXX
        ///  method will pass through.
        /// </summary>
        public unsafe object GetNativeValue(object component)
        {
            if (component is null)
            {
                return null;
            }

            if (component is ICustomTypeDescriptor)
            {
                component = ((ICustomTypeDescriptor)component).GetPropertyOwner(this);
            }

            if (component is null || !Marshal.IsComObject(component) || !(component is Oleaut32.IDispatch))
            {
                return null;
            }

            Oleaut32.IDispatch pDisp = (Oleaut32.IDispatch)component;
            object[] pVarResult = new object[1];
            EXCEPINFO pExcepInfo = default(EXCEPINFO);
            DISPPARAMS dispParams = default(DISPPARAMS);
            Guid g = Guid.Empty;

            HRESULT hr = pDisp.Invoke(
                dispid,
                &g,
                PInvoke.GetThreadLocale(),
                DISPATCH_FLAGS.DISPATCH_PROPERTYGET,
                &dispParams,
                pVarResult,
                &pExcepInfo,
                null);

            if (hr == HRESULT.S_OK || hr == HRESULT.S_FALSE)
            {
                if (pVarResult[0] is null || Convert.IsDBNull(pVarResult[0]))
                {
                    lastValue = null;
                }
                else
                {
                    lastValue = pVarResult[0];
                }

                return lastValue;
            }
            else if (hr == HRESULT.DISP_E_EXCEPTION)
            {
                return null;
            }
            else
            {
                throw new ExternalException(string.Format(SR.DispInvokeFailed, "GetValue", hr), (int)hr);
            }
        }

        /// <summary>
        ///  Checks whether the particular item(s) need refreshing.
        /// </summary>
        private bool GetNeedsRefresh(int mask)
        {
            return (refreshState & mask) != 0;
        }

        /// <summary>
        ///  Retrieves the current value of the property on component,
        ///  invoking the getXXX method.  An exception in the getXXX
        ///  method will pass through.
        /// </summary>
        public override object GetValue(object component)
        {
            lastValue = GetNativeValue(component);
            // do we need to convert the type?
            if (ConvertingNativeType && lastValue is not null)
            {
                lastValue = valueConverter.ConvertNativeToManaged(lastValue, this);
            }
            else if (lastValue is not null && propertyType is not null && propertyType.IsEnum && lastValue.GetType().IsPrimitive)
            {
                // we've got to convert the value here -- we built the enum but the native object returns
                // us values as integers
                //
                try
                {
                    lastValue = Enum.ToObject(propertyType, lastValue);
                }
                catch
                {
                }
            }

            return lastValue;
        }

        /// <summary>
        ///  Retrieves the value editor for the property.  If a value editor is passed
        ///  in as a TypeConverterAttribute, that value editor will be instantiated.
        ///  If no such attribute was found, a system value editor will be looked for.
        ///  See TypeConverter for a description of how system value editors are found.
        ///  If there is no system value editor, null is returned.  If the value editor found
        ///  takes an IEditorSite in its constructor, the parameter will be passed in.
        /// </summary>
        public void GetTypeConverterAndTypeEditor(ref TypeConverter typeConverter, Type editorBaseType, ref object typeEditor)
        {
            // get the base editor and converter, attributes first
            TypeConverter localConverter = typeConverter;
            object localEditor = typeEditor;

            localConverter ??= GetBaseTypeConverter();

            localEditor ??= GetBaseTypeEditor(editorBaseType);

            // if this is a object, get the value and attempt to create the correct value editor based on that value.
            // we don't do this if the state came from an attribute
            //
            if (0 == (refreshState & Com2PropertyDescriptorRefresh.TypeConverterAttr) && PropertyType == typeof(Com2Variant))
            {
                Type editorType = PropertyType;
                object value = GetValue(TargetObject);
                if (value is not null)
                {
                    editorType = value.GetType();
                }

                ComNativeDescriptor.ResolveVariantTypeConverterAndTypeEditor(value, ref localConverter, editorBaseType, ref localEditor);
            }

            // now see if someone else would like to serve up a value editor
            //

            // unwrap the editor if it's one of ours.
            if (localConverter is Com2PropDescMainConverter)
            {
                localConverter = ((Com2PropDescMainConverter)localConverter).InnerConverter;
            }

            GetTypeConverterAndTypeEditorEvent e = new GetTypeConverterAndTypeEditorEvent(localConverter, localEditor);
            OnGetTypeConverterAndTypeEditor(e);
            localConverter = e.TypeConverter;
            localEditor = e.TypeEditor;

            // just in case one of the handlers removed our editor...
            //
            localConverter ??= GetBaseTypeConverter();

            localEditor ??= GetBaseTypeEditor(editorBaseType);

            // wrap the value editor in our main value editor, but only if it isn't "TypeConverter" or already a Com2PropDescMainTypeConverter
            //
            Type localConverterType = localConverter.GetType();
            if (localConverterType != typeof(TypeConverter) && localConverterType != (typeof(Com2PropDescMainConverter)))
            {
                localConverter = new Com2PropDescMainConverter(this, localConverter);
            }

            // save the values back to the variables.
            //
            typeConverter = localConverter;
            typeEditor = localEditor;
        }

        /// <summary>
        ///  Is the given value equal to the last known value for this object?
        /// </summary>
        public bool IsCurrentValue(object value)
        {
            return (value == lastValue || (lastValue is not null && lastValue.Equals(value)));
        }

        /// <summary>
        ///  Raises the appropriate event
        /// </summary>
        protected void OnCanResetValue(GetBoolValueEvent gvbe)
        {
            RaiseGetBoolValueEvent(EventCanResetValue, gvbe);
        }

        protected void OnGetBaseAttributes(GetAttributesEvent e)
        {
            try
            {
                com2props.AlwaysValid = com2props.CheckValidity();

                ((GetAttributesEventHandler)Events[EventGetBaseAttributes])?.Invoke(this, e);
            }
            finally
            {
                com2props.AlwaysValid = false;
            }
        }

        /// <summary>
        ///  Raises the appropriate event
        /// </summary>
        protected void OnGetDisplayName(GetNameItemEvent gnie)
        {
            RaiseGetNameItemEvent(EventGetDisplayName, gnie);
        }

        /// <summary>
        ///  Raises the appropriate event
        /// </summary>
        protected void OnGetDisplayValue(GetNameItemEvent gnie)
        {
            RaiseGetNameItemEvent(EventGetDisplayValue, gnie);
        }

        /// <summary>
        ///  Raises the appropriate event
        /// </summary>
        protected void OnGetDynamicAttributes(GetAttributesEvent e)
        {
            try
            {
                com2props.AlwaysValid = com2props.CheckValidity();
                ((GetAttributesEventHandler)Events[EventGetDynamicAttributes])?.Invoke(this, e);
            }
            finally
            {
                com2props.AlwaysValid = false;
            }
        }

        /// <summary>
        ///  Raises the appropriate event
        /// </summary>
        protected void OnGetIsReadOnly(GetBoolValueEvent gvbe)
        {
            RaiseGetBoolValueEvent(EventGetIsReadOnly, gvbe);
        }

        protected void OnGetTypeConverterAndTypeEditor(GetTypeConverterAndTypeEditorEvent e)
        {
            try
            {
                com2props.AlwaysValid = com2props.CheckValidity();
                ((GetTypeConverterAndTypeEditorEventHandler)Events[EventGetTypeConverterAndTypeEditor])?.Invoke(this, e);
            }
            finally
            {
                com2props.AlwaysValid = false;
            }
        }

        /// <summary>
        ///  Raises the appropriate event
        /// </summary>
        protected void OnResetValue(EventArgs e)
        {
            RaiseCom2Event(EventResetValue, e);
        }

        /// <summary>
        ///  Raises the appropriate event
        /// </summary>
        protected void OnShouldSerializeValue(GetBoolValueEvent gvbe)
        {
            RaiseGetBoolValueEvent(EventShouldSerializeValue, gvbe);
        }

        /// <summary>
        ///  Raises the appropriate event
        /// </summary>
        protected void OnShouldRefresh(GetRefreshStateEvent gvbe)
        {
            RaiseGetBoolValueEvent(EventShouldRefresh, gvbe);
        }

        /// <summary>
        ///  Raises the appropriate event
        /// </summary>
        private void RaiseGetBoolValueEvent(object key, GetBoolValueEvent e)
        {
            try
            {
                com2props.AlwaysValid = com2props.CheckValidity();
                ((GetBoolValueEventHandler)Events[key])?.Invoke(this, e);
            }
            finally
            {
                com2props.AlwaysValid = false;
            }
        }

        /// <summary>
        ///  Raises the appropriate event
        /// </summary>
        private void RaiseCom2Event(object key, EventArgs e)
        {
            try
            {
                com2props.AlwaysValid = com2props.CheckValidity();
                ((Com2EventHandler)Events[key])?.Invoke(this, e);
            }
            finally
            {
                com2props.AlwaysValid = false;
            }
        }

        /// <summary>
        ///  Raises the appropriate event
        /// </summary>
        private void RaiseGetNameItemEvent(object key, GetNameItemEvent e)
        {
            try
            {
                com2props.AlwaysValid = com2props.CheckValidity();
                ((GetNameItemEventHandler)Events[key])?.Invoke(this, e);
            }
            finally
            {
                com2props.AlwaysValid = false;
            }
        }

        /// <summary>
        ///  Will reset the default value for this property on the component.  If
        ///  there was a default value passed in as a DefaultValueAttribute, that
        ///  value will be set as the value of the property on the component.  If
        ///  there was no default value passed in, a ResetXXX method will be looked
        ///  for.  If one is found, it will be invoked.  If one is not found, this
        ///  is a nop.
        /// </summary>
        public override void ResetValue(object component)
        {
            if (component is ICustomTypeDescriptor)
            {
                component = ((ICustomTypeDescriptor)component).GetPropertyOwner(this);
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
                refreshState |= mask;
            }
            else
            {
                refreshState &= ~mask;
            }
        }

        /// <summary>
        ///  This will set value to be the new value of this property on the
        ///  component by invoking the setXXX method on the component.  If the
        ///  value specified is invalid, the component should throw an exception
        ///  which will be passed up.  The component designer should design the
        ///  property so that getXXX following a setXXX should return the value
        ///  passed in if no exception was thrown in the setXXX call.
        /// </summary>
        public override unsafe void SetValue(object component, object value)
        {
            if (readOnly)
            {
                throw new NotSupportedException(string.Format(SR.COM2ReadonlyProperty, Name));
            }

            object owner = component;
            if (owner is ICustomTypeDescriptor)
            {
                owner = ((ICustomTypeDescriptor)owner).GetPropertyOwner(this);
            }

            if (owner is null || !Marshal.IsComObject(owner) || owner is not Oleaut32.IDispatch)
            {
                return;
            }

            // do we need to convert the type?
            if (valueConverter is not null)
            {
                bool cancel = false;
                value = valueConverter.ConvertManagedToNative(value, this, ref cancel);
                if (cancel)
                {
                    return;
                }
            }

            Oleaut32.IDispatch pDisp = (Oleaut32.IDispatch)owner;

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
            Guid g = Guid.Empty;
            HRESULT hr = pDisp.Invoke(
                dispid,
                &g,
                PInvoke.GetThreadLocale(),
                DISPATCH_FLAGS.DISPATCH_PROPERTYPUT,
                &dispParams,
                null,
                &excepInfo,
                null);

            string errorInfo = null;
            if (hr == HRESULT.DISP_E_EXCEPTION && excepInfo.scode != 0)
            {
                hr = (HRESULT)excepInfo.scode;
                if (!excepInfo.bstrDescription.IsNull)
                {
                    errorInfo = excepInfo.bstrDescription.ToString();
                }
            }

            if (hr == HRESULT.E_ABORT || hr == HRESULT.OLE_E_PROMPTSAVECANCELLED)
            {
                return;
            }
            else if (hr == HRESULT.S_OK || hr == HRESULT.S_FALSE)
            {
                OnValueChanged(component, EventArgs.Empty);
                lastValue = value;
            }
            else
            {
                if (pDisp is Oleaut32.ISupportErrorInfo iSupportErrorInfo)
                {
                    g = typeof(Oleaut32.IDispatch).GUID;
                    if (iSupportErrorInfo.InterfaceSupportsErrorInfo(&g) == HRESULT.S_OK)
                    {
                        WinFormsComWrappers.ErrorInfoWrapper pErrorInfo;
                        Oleaut32.GetErrorInfo(out pErrorInfo);

                        if (pErrorInfo is not null)
                        {
                            string info;
                            if (pErrorInfo.GetDescription(out info))
                            {
                                errorInfo = info;
                            }

                            pErrorInfo.Dispose();
                        }
                    }
                }
                else if (errorInfo is null)
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

                        if (result == 0)
                        {
                            errorInfo = string.Format(CultureInfo.CurrentCulture, SR.DispInvokeFailed, "SetValue", hr);
                        }
                        else
                        {
                            ReadOnlySpan<char> ipBuffer = new(buffer);
                            ipBuffer.TrimEnd('\n');
                            ipBuffer.TrimEnd('\r');
                            errorInfo = ipBuffer.ToString();
                        }
                    }

                    ArrayPool<char>.Shared.Return(buffer);
                }

                throw new ExternalException(errorInfo, (int)hr);
            }
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
        private readonly ArrayList attrList;

        public GetAttributesEvent(ArrayList attrList)
        {
            this.attrList = attrList;
        }

        public void Add(Attribute attribute)
        {
            attrList.Add(attribute);
        }
    }

    internal delegate void Com2EventHandler(Com2PropertyDescriptor sender, EventArgs e);

    internal delegate void GetAttributesEventHandler(Com2PropertyDescriptor sender, GetAttributesEvent gaevent);

    internal class GetNameItemEvent : EventArgs
    {
        private object nameItem;

        public GetNameItemEvent(object defName)
        {
            nameItem = defName;
        }

        public object Name
        {
            get
            {
                return nameItem;
            }
            set
            {
                nameItem = value;
            }
        }

        public string NameString
        {
            get
            {
                if (nameItem is not null)
                {
                    return nameItem.ToString();
                }

                return "";
            }
        }
    }

    internal delegate void GetNameItemEventHandler(Com2PropertyDescriptor sender, GetNameItemEvent gnievent);

    internal class GetBoolValueEvent : EventArgs
    {
        private bool value;

        public GetBoolValueEvent(bool defValue)
        {
            value = defValue;
        }

        public bool Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }
    }

    internal delegate void GetBoolValueEventHandler(Com2PropertyDescriptor sender, GetBoolValueEvent gbeevent);

    internal class GetRefreshStateEvent : GetBoolValueEvent
    {
        readonly Com2ShouldRefreshTypes item;

        public GetRefreshStateEvent(Com2ShouldRefreshTypes item, bool defValue) : base(defValue)
        {
            this.item = item;
        }
    }

    internal delegate void GetTypeConverterAndTypeEditorEventHandler(Com2PropertyDescriptor sender, GetTypeConverterAndTypeEditorEvent e);

    internal class GetTypeConverterAndTypeEditorEvent : EventArgs
    {
        private TypeConverter typeConverter;
        private object typeEditor;

        public GetTypeConverterAndTypeEditorEvent(TypeConverter typeConverter, object typeEditor)
        {
            this.typeEditor = typeEditor;
            this.typeConverter = typeConverter;
        }

        public TypeConverter TypeConverter
        {
            get
            {
                return typeConverter;
            }
            set
            {
                typeConverter = value;
            }
        }

        public object TypeEditor
        {
            get
            {
                return typeEditor;
            }
            set
            {
                typeEditor = value;
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Reflection;
    using System.ComponentModel.Design;
    using Microsoft.Win32;
    using System.Collections;
    using System.Text;
    using System.Drawing.Design;
    using System.Globalization;
    using System.Runtime.Versioning;

    /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor"]/*' />
    /// <devdoc>
    /// This class wraps a com native property in a property descriptor.
    /// It maintains all information relative to the basic (e.g. ITypeInfo)
    /// information about the member dispid function, and converts that info
    /// to meaningful managed code information.
    ///
    /// It also allows other objects to register listeners to add extended
    /// information at runtime such as attributes of TypeConverters.
    /// </devdoc>
    internal class Com2PropertyDescriptor : PropertyDescriptor, ICloneable{
        private EventHandlerList events;

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.baseReadOnly"]/*' />
        /// <devdoc>
        /// Is this guy read only?
        /// </devdoc>
        private bool baseReadOnly;
        private bool readOnly;

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.propertyType"]/*' />
        /// <devdoc>
        /// The resoved native type -> clr type
        /// </devdoc>
        private Type propertyType;

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.dispid"]/*' />
        /// <devdoc>
        /// The dispid. This is also in a DispIDAttiribute, but we
        /// need it a lot.
        /// </devdoc>
        private int  dispid;
        
        private TypeConverter   converter;
        private object          editor;

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.displayName"]/*' />
        /// <devdoc>
        /// The current display name to show for this property
        /// </devdoc>
        private string displayName;

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.typeData"]/*' />
        /// <devdoc>
        /// This is any extra data needed.  For IDispatch types, it's the GUID of
        /// the interface, etc.
        /// </devdoc>
        private object typeData;


        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.refreshState"]/*' />
        /// <devdoc>
        /// Keeps track of which data members need to be refreshed.
        /// </devdoc>
        private int  refreshState;
        
        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.queryRefresh"]/*' />
        /// <devdoc>
        /// Should we bother asking if refresh is needed?
        /// </devdoc>
        private bool queryRefresh = false;

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.com2props"]/*' />
        /// <devdoc>
        /// Our properties manager
        /// </devdoc>
        private Com2Properties com2props;


        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.baseAttrs"]/*' />
        /// <devdoc>
        /// Our original baseline properties
        /// </devdoc>
        private Attribute[] baseAttrs;

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.lastValue"]/*' />
        /// <devdoc>
        /// Our cached last value -- this is only
        /// for checking if we should ask for a display value
        /// </devdoc>
        private Object lastValue;

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.typeHide"]/*' />
        /// <devdoc>
        /// For Object and dispatch types, we hide them by default.
        /// </devdoc>
        private bool   typeHide;

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.canShow"]/*' />
        /// <devdoc>
        /// Set if the metadata causes this property to always be hidden
        /// </devdoc>
        private bool   canShow;

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.hrHidden"]/*' />
        /// <devdoc>
        /// This property is hidden because its get didn't return S_OK
        /// </devdoc>
        private bool   hrHidden;

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.inAttrQuery"]/*' />
        /// <devdoc>
        /// Set if we are in the process of asking handlers for attributes
        /// </devdoc>
        private bool   inAttrQuery;

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.EventGetDynamicAttributes"]/*' />
        /// <devdoc>
        /// Our event signitures.
        /// </devdoc>
        private static readonly Object EventGetBaseAttributes      = new Object();
        private static readonly Object EventGetDynamicAttributes   = new Object();
        private static readonly Object EventShouldRefresh          = new Object();
        private static readonly Object EventGetDisplayName         = new Object();
        private static readonly Object EventGetDisplayValue        = new Object();
        private static readonly Object EventGetIsReadOnly          = new Object();
        
        
        private static readonly Object EventGetTypeConverterAndTypeEditor   = new Object();
        
        private static readonly Object EventShouldSerializeValue = new Object();
        private static readonly Object EventCanResetValue      = new Object();
        private static readonly Object EventResetValue         = new Object();

        private static readonly Guid GUID_COLOR = new Guid("{66504301-BE0F-101A-8BBB-00AA00300CAB}");
                        

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.oleTypeGuids"]/*' />
        /// <devdoc>
        /// Our map of native types that we can map to managed types for editors
        /// </devdoc>
        private static IDictionary oleConverters;

        static Com2PropertyDescriptor() {
            oleConverters = new SortedList();
            oleConverters[GUID_COLOR] = typeof(Com2ColorConverter);
            oleConverters[typeof(SafeNativeMethods.IFontDisp).GUID] = typeof(Com2FontConverter);
            oleConverters[typeof(UnsafeNativeMethods.IFont).GUID] = typeof(Com2FontConverter);
            oleConverters[typeof(UnsafeNativeMethods.IPictureDisp).GUID] = typeof(Com2PictureConverter);
            oleConverters[typeof(UnsafeNativeMethods.IPicture).GUID] = typeof(Com2PictureConverter);
        }
       
        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.valueConverter"]/*' />
        /// <devdoc>
        /// Should we convert our type?
        /// </devdoc>
        private Com2DataTypeToManagedDataTypeConverter valueConverter;


        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.Com2PropertyDescriptor"]/*' />
        /// <devdoc>
        /// Ctor.
        /// </devdoc>
        public Com2PropertyDescriptor(int dispid, string name, Attribute[] attrs, bool readOnly, Type propType, Object typeData, bool hrHidden)
        : base(name, attrs) {
            this.baseReadOnly = readOnly;
            this.readOnly = readOnly;

            this.baseAttrs = attrs;
            SetNeedsRefresh(Com2PropertyDescriptorRefresh.BaseAttributes, true);

            this.hrHidden = hrHidden;

            // readonly to begin with are always read only
            SetNeedsRefresh(Com2PropertyDescriptorRefresh.ReadOnly, readOnly);

            this.propertyType = propType;
            
            this.dispid = dispid;

            if (typeData != null) {
                this.typeData = typeData;
                if (typeData is Com2Enum) {
                     converter = new Com2EnumConverter((Com2Enum)typeData);
                }
                else if (typeData is Guid) {
                    valueConverter =  CreateOleTypeConverter((Type)oleConverters[(Guid)typeData]);
                }
            }

            // check if this thing is hidden from metadata
            this.canShow = true;

            if (attrs != null) {
                for (int i = 0; i < attrs.Length; i++) {
                    if (attrs[i].Equals(BrowsableAttribute.No) && !hrHidden) {
                        this.canShow = false;
                        break;
                    }
                }
            }
            
            if (this.canShow && (propType == typeof(Object) || (valueConverter == null && propType == typeof(UnsafeNativeMethods.IDispatch)))) {
                this.typeHide = true;
            }
        }

        protected Attribute[] BaseAttributes {
            get {

                if (GetNeedsRefresh(Com2PropertyDescriptorRefresh.BaseAttributes)) {
                    SetNeedsRefresh(Com2PropertyDescriptorRefresh.BaseAttributes, false);

                    int baseCount = baseAttrs == null ? 0 : baseAttrs.Length;

                    ArrayList attrList = new ArrayList();

                    if (baseCount != 0) {
                        attrList.AddRange(baseAttrs);
                    }

                    OnGetBaseAttributes(new GetAttributesEvent(attrList));

                    if (attrList.Count != baseCount) {
                        this.baseAttrs = new Attribute[attrList.Count];
                    }
                    
                    if (baseAttrs != null) {
                        attrList.CopyTo(this.baseAttrs, 0);
                    }
                    else {
                        baseAttrs = new Attribute[0];
                    }
                }

                return baseAttrs;
            }
            set {
                baseAttrs = value;
            }
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.Attributes"]/*' />
        /// <devdoc>
        ///     Attributes
        /// </devdoc>
        public override AttributeCollection Attributes {
            get {
                if (this.AttributesValid || this.InAttrQuery) {
                    return base.Attributes;
                }

                // restore our base attributes
                this.AttributeArray = this.BaseAttributes;

                ArrayList newAttributes = null;

                // if we are forcing a hide
                if (typeHide && canShow) {
                    if (newAttributes == null) {
                        newAttributes = new ArrayList(AttributeArray);
                    }
                    newAttributes.Add(new BrowsableAttribute(false));
                }
                else if (hrHidden) {
                    // check to see if the get still fails
                    Object target = this.TargetObject;
                    if (target != null) {
                        int hr = new ComNativeDescriptor().GetPropertyValue(target, this.dispid, new object[1]);

                        // if not, go ahead and make this a browsable item
                        if (NativeMethods.Succeeded(hr)) {
                            // make it browsable
                            if (newAttributes == null) {
                                newAttributes = new ArrayList(AttributeArray);
                            }
                            newAttributes.Add(new BrowsableAttribute(true));
                            hrHidden = false;
                        }
                    }
                }
                
                this.inAttrQuery = true;
                try {

                    // demand get any extended attributes
                    ArrayList attrList = new ArrayList();

                    OnGetDynamicAttributes(new GetAttributesEvent(attrList));

                    Attribute ma;
                    
                    if (attrList.Count != 0 && newAttributes == null) {
                        newAttributes = new ArrayList(AttributeArray);
                    }

                    // push any new attributes into the base type
                    for (int i=0; i < attrList.Count; i++) {
                        ma = (Attribute)attrList[i];
                        newAttributes.Add(ma);
                    }
                }
                finally {
                    this.inAttrQuery = false;
                }

                // these are now valid.
                SetNeedsRefresh(Com2PropertyDescriptorRefresh.Attributes, false);
                
                // If we reconfigured attributes, then poke the new set back in.
                //
                if (newAttributes != null) {
                    Attribute[] temp = new Attribute[newAttributes.Count];
                    newAttributes.CopyTo(temp, 0);
                    AttributeArray = temp;
                }
                
                return base.Attributes;
            }

        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.AttributesValid"]/*' />
        /// <devdoc>
        ///     Checks if the attributes are valid.  Asks any clients if they
        ///     would like attributes requeried.
        /// </devdoc>
        protected bool AttributesValid{
            get{
                bool currentRefresh = !GetNeedsRefresh(Com2PropertyDescriptorRefresh.Attributes);
                if (queryRefresh) {
                    GetRefreshStateEvent rse = new GetRefreshStateEvent(Com2ShouldRefreshTypes.Attributes, !currentRefresh);
                    OnShouldRefresh(rse);
                    currentRefresh = !rse.Value;
                    SetNeedsRefresh(Com2PropertyDescriptorRefresh.Attributes, rse.Value);
                }
                return currentRefresh;
            }
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.CanShow"]/*' />
        /// <devdoc>
        ///     Checks if this item can be shown.
        /// </devdoc>
        public bool CanShow{
            get{
                return this.canShow;
            }
        }


        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.ComponentType"]/*' />
        /// <devdoc>
        ///     Retrieves the type of the component this PropertyDescriptor is bound to.
        /// </devdoc>
        public override Type ComponentType {
            get {
                return typeof(UnsafeNativeMethods.IDispatch);
            }
        }
        
        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.Converter"]/*' />
        /// <devdoc>
        ///      Retrieves the type converter for this property.
        /// </devdoc>
        public override TypeConverter Converter {
            get {
               if (TypeConverterValid) {
                  return converter;
               }
               
               Object typeEd = null;
               
               GetTypeConverterAndTypeEditor(ref converter, typeof(UITypeEditor), ref typeEd);
               
               if (!TypeEditorValid) {
                  this.editor = typeEd;
                  SetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeEditor, false);
               }
               SetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeConverter, false);
               
               return converter;
            }
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.ConvertingNativeType"]/*' />
        /// <devdoc>
        ///     Retrieves whether this component is applying a type conversion...
        /// </devdoc>
        public bool ConvertingNativeType {
            get {
                return(valueConverter != null);
            }
        }        

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.DefaultValue"]/*' />
        /// <devdoc>
        ///      Retrieves the default value for this property.
        /// </devdoc>
        protected virtual Object DefaultValue {
            get {
                return null;
            }
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.DISPID"]/*' />
        /// <devdoc>
        ///     Retrieves the DISPID for this item
        /// </devdoc>
        public int DISPID{
            get{
                return this.dispid;
            }
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.DisplayName"]/*' />
        /// <devdoc>
        ///     Gets the friendly name that should be displayed to the user in a window like
        ///     the Property Browser.
        /// </devdoc>
        public override string DisplayName {
            get {
                if (!this.DisplayNameValid) {
                    GetNameItemEvent gni = new GetNameItemEvent(base.DisplayName);
                    OnGetDisplayName(gni);
                    this.displayName = gni.NameString;
                    SetNeedsRefresh(Com2PropertyDescriptorRefresh.DisplayName, false);
                }
                return this.displayName;
            }
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.DisplayNameValid"]/*' />
        /// <devdoc>
        ///     Checks if the property display name is valid
        ///     asks clients if they would like display name requeried.
        /// </devdoc>
        protected bool DisplayNameValid{
            get{
                bool currentRefresh = !(displayName == null || GetNeedsRefresh(Com2PropertyDescriptorRefresh.DisplayName));
                if (queryRefresh) {
                    GetRefreshStateEvent rse = new GetRefreshStateEvent(Com2ShouldRefreshTypes.DisplayName, !currentRefresh);
                    OnShouldRefresh(rse);
                    SetNeedsRefresh(Com2PropertyDescriptorRefresh.DisplayName, rse.Value);
                    currentRefresh = !rse.Value;
                }
                return currentRefresh;
            }
        }

        protected EventHandlerList Events {
            get {
                if (events == null) {
                    events = new EventHandlerList();
                }
                return events;
            }
        }

        protected bool InAttrQuery{
            get{
                return this.inAttrQuery;
            }
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.IsReadOnly"]/*' />
        /// <devdoc>
        ///     Indicates whether this property is read only.
        /// </devdoc>
        public override bool IsReadOnly {
            get {
                if (!this.ReadOnlyValid) {
                    this.readOnly |= (this.Attributes[typeof(ReadOnlyAttribute)].Equals(ReadOnlyAttribute.Yes));
                    GetBoolValueEvent gbv = new GetBoolValueEvent(this.readOnly);
                    OnGetIsReadOnly(gbv);
                    this.readOnly = gbv.Value;
                    SetNeedsRefresh(Com2PropertyDescriptorRefresh.ReadOnly, false);
                }
                return this.readOnly;
            }
        }

        internal Com2Properties PropertyManager{
            set{
                this.com2props = value;
            }
            get{
                return this.com2props;
            }
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.PropertyType"]/*' />
        /// <devdoc>
        ///     Retrieves the type of the property.
        /// </devdoc>
        public override Type PropertyType {
            get {
                // replace the type with the mapped converter type
                if (valueConverter != null) {
                    return valueConverter.ManagedType;
                }
                else {
                    return propertyType;
                }
            }
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.ReadOnlyValid"]/*' />
        /// <devdoc>
        ///     Checks if the read only state is valid.
        ///     Asks clients if they would like read-only requeried.
        /// </devdoc>
        protected bool ReadOnlyValid{
            get{
                if (baseReadOnly) {
                    return true;
                }
                
                bool currentRefresh = !GetNeedsRefresh(Com2PropertyDescriptorRefresh.ReadOnly);
                
                if (queryRefresh) {
                    GetRefreshStateEvent rse = new GetRefreshStateEvent(Com2ShouldRefreshTypes.ReadOnly, !currentRefresh);
                    OnShouldRefresh(rse);
                    SetNeedsRefresh(Com2PropertyDescriptorRefresh.ReadOnly, rse.Value);
                    currentRefresh = !rse.Value;
                }
                return currentRefresh;
            }
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.TargetObject"]/*' />
        /// <devdoc>
        ///     Gets the Object that this descriptor was created for.
        ///     May be null if the Object's ref has died.
        /// </devdoc>
        public virtual Object TargetObject{
            get{
                if (com2props != null) {
                    return com2props.TargetObject;
                }
                return null;
            }
        }
        
        protected bool TypeConverterValid {
            get {
                bool currentRefresh =!(converter == null || GetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeConverter));
                if (queryRefresh) {
                    GetRefreshStateEvent rse = new GetRefreshStateEvent(Com2ShouldRefreshTypes.TypeConverter, !currentRefresh);
                    OnShouldRefresh(rse);
                    SetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeConverter, rse.Value);
                    currentRefresh = !rse.Value;
                }
                return currentRefresh;
            }
        }
        
        protected bool TypeEditorValid {
            get {
                bool currentRefresh = !(editor == null || GetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeEditor));
                
                if (queryRefresh) {
                    GetRefreshStateEvent rse = new GetRefreshStateEvent(Com2ShouldRefreshTypes.TypeEditor, !currentRefresh);
                    OnShouldRefresh(rse);
                    SetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeEditor, rse.Value);
                    currentRefresh = !rse.Value;
                }
                return currentRefresh;
            }
        }


        public event GetBoolValueEventHandler QueryCanResetValue {
            add {
                Events.AddHandler(EventCanResetValue, value);
            }
            remove {
                Events.RemoveHandler(EventCanResetValue, value);
            }
        }

        public event GetAttributesEventHandler QueryGetBaseAttributes {
            add {
                Events.AddHandler(EventGetBaseAttributes, value);
            }
            remove {
                Events.RemoveHandler(EventGetBaseAttributes, value);
            }
        }

        public event GetAttributesEventHandler QueryGetDynamicAttributes {
            add {
                Events.AddHandler(EventGetDynamicAttributes, value);
            }
            remove {
                Events.RemoveHandler(EventGetDynamicAttributes, value);
            }
        }


        public event GetNameItemEventHandler QueryGetDisplayName {
            add {
                Events.AddHandler(EventGetDisplayName, value);
            }
            remove {
                Events.RemoveHandler(EventGetDisplayName, value);
            }
        }


        public event GetNameItemEventHandler QueryGetDisplayValue {
            add {
                Events.AddHandler(EventGetDisplayValue, value); 
            }
            remove {
                Events.RemoveHandler(EventGetDisplayValue, value);
            }
        }


        public event GetBoolValueEventHandler QueryGetIsReadOnly {
            add {
                Events.AddHandler(EventGetIsReadOnly, value);
            }
            remove {
                Events.RemoveHandler(EventGetIsReadOnly, value);
            }
        }


        public event GetTypeConverterAndTypeEditorEventHandler QueryGetTypeConverterAndTypeEditor {
            add {
                Events.AddHandler(EventGetTypeConverterAndTypeEditor, value);
            }
            remove {
                Events.RemoveHandler(EventGetTypeConverterAndTypeEditor, value);
            }
        }
        

        public event Com2EventHandler QueryResetValue {
            add {
                Events.AddHandler(EventResetValue, value);
            }
            remove {
                Events.RemoveHandler(EventResetValue, value);
            }
        }


        public event GetBoolValueEventHandler QueryShouldSerializeValue {
            add {
                Events.AddHandler(EventShouldSerializeValue, value);
            }
            remove {
                Events.RemoveHandler(EventShouldSerializeValue, value);
            }
        }



        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.CanResetValue"]/*' />
        /// <devdoc>
        ///     Indicates whether reset will change the value of the component.  If there
        ///     is a DefaultValueAttribute, then this will return true if getValue returns
        ///     something different than the default value.  If there is a reset method and
        ///     a shouldPersist method, this will return what shouldPersist returns.
        ///     If there is just a reset method, this always returns true.  If none of these
        ///     cases apply, this returns false.
        /// </devdoc>
        public override bool CanResetValue(Object component) {

            if (component is ICustomTypeDescriptor) {
                component = ((ICustomTypeDescriptor)component).GetPropertyOwner(this);
            }
        
            if (component == this.TargetObject) {
                GetBoolValueEvent gbv = new GetBoolValueEvent(false);
                OnCanResetValue(gbv);
                return gbv.Value;
            }
            return false;
        }

        public object Clone() {
            return new Com2PropertyDescriptor(this.dispid, this.Name, (Attribute[])this.baseAttrs.Clone(), this.readOnly, this.propertyType, this.typeData, this.hrHidden);
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.CreateOleTypeConverter"]/*' />
        /// <devdoc>
        ///     Creates a converter Object, first by looking for a ctor with a Com2ProeprtyDescriptor
        ///     parameter, then using the default ctor if it is not found.
        /// </devdoc>
        private Com2DataTypeToManagedDataTypeConverter CreateOleTypeConverter(Type t) {

            if (t == null) {
                return null;
            }

            ConstructorInfo ctor = t.GetConstructor(new Type[]{typeof(Com2PropertyDescriptor)});
            Com2DataTypeToManagedDataTypeConverter converter;
            if (ctor != null) {
                converter = (Com2DataTypeToManagedDataTypeConverter)ctor.Invoke(new Object[]{this});
            }
            else {
                converter = (Com2DataTypeToManagedDataTypeConverter)Activator.CreateInstance(t);
            }
            return converter;
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.CreateAttributeCollection"]/*' />
        /// <devdoc>
        ///     Creates an instance of the member attribute collection. This can
        ///     be overriden by subclasses to return a subclass of AttributeCollection.
        /// </devdoc>
        protected override AttributeCollection CreateAttributeCollection() {
            return new AttributeCollection(AttributeArray);
        }
        
        private TypeConverter GetBaseTypeConverter() {
        
            if (PropertyType == null) {
                return new TypeConverter();
            }
            
            TypeConverter localConverter = null;
            
            TypeConverterAttribute attr = (TypeConverterAttribute)Attributes[typeof(TypeConverterAttribute)];
            if (attr != null) {
               string converterTypeName = attr.ConverterTypeName;
               if (converterTypeName != null && converterTypeName.Length > 0) {
                   Type converterType = Type.GetType(converterTypeName);
                   if (converterType != null && typeof(TypeConverter).IsAssignableFrom(converterType)) {
                       try {
                          localConverter = (TypeConverter)Activator.CreateInstance(converterType);
                          if (localConverter != null) {
                               refreshState |= Com2PropertyDescriptorRefresh.TypeConverterAttr;
                          }
                       }
                       catch (Exception ex) {
                          Debug.Fail("Failed to create TypeConverter of type '" + attr.ConverterTypeName + "' from Attribute", ex.ToString());
                       }
                   }
               }
            }
            
            // if we didn't get one from the attribute, ask the type descriptor
            if (localConverter == null) {
               // we don't want to create the value editor for the IDispatch props because
                // that will create the reference editor.  We don't want that guy!
                //
                if (!typeof(UnsafeNativeMethods.IDispatch).IsAssignableFrom(this.PropertyType)) {
                     localConverter = base.Converter;
                }
                else {
                     localConverter = new Com2IDispatchConverter(this, false);
                }
            }
            
            if (localConverter == null) {
                localConverter = new TypeConverter();
            }
            return localConverter;
        }
        
        private Object GetBaseTypeEditor(Type editorBaseType) {
            
            if (PropertyType == null) {
                return null;
            }
            
            Object localEditor = null;
            EditorAttribute attr = (EditorAttribute)Attributes[typeof(EditorAttribute)];
            if (attr != null) {
               string editorTypeName = attr.EditorBaseTypeName;
               
               if (editorTypeName != null && editorTypeName.Length > 0) {
                   Type attrEditorBaseType = Type.GetType(editorTypeName);
                   if (attrEditorBaseType != null && attrEditorBaseType == editorBaseType) {
                        Type type = Type.GetType(attr.EditorTypeName);
                        if (type != null) {
                            try {
                               localEditor = Activator.CreateInstance(type);
                               if (localEditor != null) {
                                  refreshState |= Com2PropertyDescriptorRefresh.TypeEditorAttr;
                               }
                            }
                            catch (Exception ex) {
                               Debug.Fail("Failed to create edtior of type '" + attr.EditorTypeName + "' from Attribute", ex.ToString()); 
                            }
                        }
                   }
               }
            }
            if (localEditor == null) {
                 localEditor = base.GetEditor(editorBaseType);
            }
            return localEditor;
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.GetDisplayValue"]/*' />
        /// <devdoc>
        ///     Gets the value that should be displayed to the user, such as in
        ///     the Property Browser.
        /// </devdoc>
        public virtual string GetDisplayValue(string defaultValue) {

            GetNameItemEvent nie = new GetNameItemEvent(defaultValue);
            OnGetDisplayValue(nie);

            string str = (nie.Name == null ? null : nie.Name.ToString());
            return str;
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.GetEditor"]/*' />
        /// <devdoc>
        ///      Retrieves an editor of the requested type.
        /// </devdoc>
        public override Object GetEditor(Type editorBaseType) {
               if (TypeEditorValid) {
                  return editor;
               }
               
               if (PropertyType == null) {
                   return null;
               }
               
               if (editorBaseType == typeof(UITypeEditor)) {
                  TypeConverter c = null;
                  GetTypeConverterAndTypeEditor(ref c, editorBaseType, ref editor);
                  
                  if (!TypeConverterValid) {
                     this.converter = c;
                     SetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeConverter, false);
                  }
                  SetNeedsRefresh(Com2PropertyDescriptorRefresh.TypeEditor, false);
               }
               else {
                  editor = base.GetEditor(editorBaseType);
               }
               return editor;
          
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.GetNativeValue"]/*' />
        /// <devdoc>
        ///     Retrieves the current native value of the property on component,
        ///     invoking the getXXX method.  An exception in the getXXX
        ///     method will pass through.
        /// </devdoc>
        public Object GetNativeValue(Object component){
            if (component == null)
                return null;

            if (component is ICustomTypeDescriptor) {
                component = ((ICustomTypeDescriptor)component).GetPropertyOwner(this);
            }

            if (component == null || !Marshal.IsComObject(component) || !(component is UnsafeNativeMethods.IDispatch))
                return null;

            UnsafeNativeMethods.IDispatch pDisp = (UnsafeNativeMethods.IDispatch)component;
            Object[] pVarResult = new Object[1];
            NativeMethods.tagEXCEPINFO pExcepInfo = new NativeMethods.tagEXCEPINFO();
            Guid g = Guid.Empty;

            int hr = pDisp.Invoke(this.dispid,
                                  ref g,
                                  SafeNativeMethods.GetThreadLCID(),
                                  NativeMethods.DISPATCH_PROPERTYGET,
                                  new NativeMethods.tagDISPPARAMS(),
                                  pVarResult,
                                  pExcepInfo, null);

            switch (hr) {
            case NativeMethods.S_OK:
            case NativeMethods.S_FALSE:

                if (pVarResult[0] == null || Convert.IsDBNull(pVarResult[0])) {
                    lastValue = null;
                }
                else {
                    lastValue = pVarResult[0];
                }
                return lastValue;
            case NativeMethods.DISP_E_EXCEPTION:
                //PrintExceptionInfo(pExcepInfo);
                return null;
            default:
                throw new ExternalException(string.Format(SR.DispInvokeFailed, "GetValue" , hr), hr);
            }
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.GetNeedsRefresh"]/*' />
        /// <devdoc>
        ///     Checks whether the particular item(s) need refreshing.
        /// </devdoc>
        private bool GetNeedsRefresh(int mask){
            return(refreshState & mask) != 0;
        }


        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.GetValue"]/*' />
        /// <devdoc>
        ///     Retrieves the current value of the property on component,
        ///     invoking the getXXX method.  An exception in the getXXX
        ///     method will pass through.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public override Object GetValue(Object component) {
            lastValue = GetNativeValue(component);
            // do we need to convert the type?
            if (this.ConvertingNativeType && lastValue != null) {
                lastValue = valueConverter.ConvertNativeToManaged(lastValue, this);
            }
            else if (lastValue != null && propertyType != null && propertyType.IsEnum && lastValue.GetType().IsPrimitive) {
                // we've got to convert the value here -- we built the enum but the native object returns
                // us values as integers
                //
                try {
                    lastValue = Enum.ToObject(propertyType, lastValue);
                }
                catch {
                }
            }
            return lastValue;
        }
        
        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.GetTypeConverterAndTypeEditor"]/*' />
        /// <devdoc>
        ///     Retrieves the value editor for the property.  If a value editor is passed
        ///     in as a TypeConverterAttribute, that value editor will be instantiated.
        ///     If no such attribute was found, a system value editor will be looked for.
        ///     See TypeConverter for a description of how system value editors are found.
        ///     If there is no system value editor, null is returned.  If the value editor found
        ///     takes an IEditorSite in its constructor, the parameter will be passed in.
        /// </devdoc>
        public void GetTypeConverterAndTypeEditor(ref TypeConverter typeConverter, Type editorBaseType, ref Object typeEditor) {
        
                // get the base editor and converter, attributes first
                TypeConverter localConverter = typeConverter;
                Object        localEditor    = typeEditor;
                
                if (localConverter == null) {
                     localConverter = GetBaseTypeConverter();
                }
                
                if (localEditor == null) {
                     localEditor = GetBaseTypeEditor(editorBaseType);
                }
                
                // if this is a object, get the value and attempt to create the correct value editor based on that value.
                // we don't do this if the state came from an attribute
                //
                if (0 == (refreshState & Com2PropertyDescriptorRefresh.TypeConverterAttr) && this.PropertyType == typeof(Com2Variant)) {
                    Type editorType = PropertyType;
                    Object value = GetValue(TargetObject);
                    if (value != null) {
                        editorType = value.GetType();
                    }
                    ComNativeDescriptor.ResolveVariantTypeConverterAndTypeEditor(value, ref localConverter, editorBaseType, ref localEditor);
                }
                
                // now see if someone else would like to serve up a value editor
                //
                
                // unwrap the editor if it's one of ours.
                if (localConverter is Com2PropDescMainConverter) {
                    localConverter = ((Com2PropDescMainConverter)localConverter).InnerConverter;
                }
                 
                GetTypeConverterAndTypeEditorEvent e = new GetTypeConverterAndTypeEditorEvent(localConverter, localEditor);
                OnGetTypeConverterAndTypeEditor(e);
                localConverter = e.TypeConverter;
                localEditor    = e.TypeEditor;
                
                // just in case one of the handlers removed our editor...
                //
                if (localConverter == null) {
                     localConverter = GetBaseTypeConverter();
                }
                
                if (localEditor == null) {
                     localEditor = GetBaseTypeEditor(editorBaseType);
                } 
                               
                // wrap the value editor in our main value editor, but only if it isn't "TypeConverter" or already a Com2PropDescMainTypeConverter
                //
                Type localConverterType = localConverter.GetType();
                if (localConverterType != typeof(TypeConverter) && localConverterType != (typeof(Com2PropDescMainConverter))) {
                    localConverter = new Com2PropDescMainConverter(this, localConverter);
                }
                
                // save the values back to the variables.
                //
                typeConverter = localConverter;
                typeEditor    = localEditor;
        }
        
        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.IsCurrentValue"]/*' />
        /// <devdoc>
        ///     Is the given value equal to the last known value for this object?
        /// </devdoc>
        public bool IsCurrentValue(object value) {
            return (value == lastValue || (lastValue != null && lastValue.Equals(value)));
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.OnCanResetValue"]/*' />
        /// <devdoc>
        ///     Raises the appropriate event
        /// </devdoc>
        protected void OnCanResetValue(GetBoolValueEvent gvbe) {
            RaiseGetBoolValueEvent(EventCanResetValue, gvbe);
        }

        protected void OnGetBaseAttributes(GetAttributesEvent e) {
            
            try {
                com2props.AlwaysValid = com2props.CheckValid();
    
                GetAttributesEventHandler handler = (GetAttributesEventHandler)Events[EventGetBaseAttributes];
                if (handler != null) handler(this, e);
            }
            finally {
                com2props.AlwaysValid = false;
            }
        }
        
        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.OnGetDisplayName"]/*' />
        /// <devdoc>
        ///     Raises the appropriate event
        /// </devdoc>
        protected void OnGetDisplayName(GetNameItemEvent gnie) {
            RaiseGetNameItemEvent(EventGetDisplayName, gnie);
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.OnGetDisplayValue"]/*' />
        /// <devdoc>
        ///     Raises the appropriate event
        /// </devdoc>
        protected void OnGetDisplayValue(GetNameItemEvent gnie) {
            RaiseGetNameItemEvent(EventGetDisplayValue, gnie);
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.OnGetDynamicAttributes"]/*' />
        /// <devdoc>
        ///     Raises the appropriate event
        /// </devdoc>
        protected void OnGetDynamicAttributes(GetAttributesEvent e) {

            try {
                com2props.AlwaysValid = com2props.CheckValid();
                GetAttributesEventHandler handler = (GetAttributesEventHandler)Events[EventGetDynamicAttributes];
                if (handler != null) handler(this, e);
            }
            finally {
                com2props.AlwaysValid = false;
            }
        }



        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.OnGetIsReadOnly"]/*' />
        /// <devdoc>
        ///     Raises the appropriate event
        /// </devdoc>
        protected void OnGetIsReadOnly(GetBoolValueEvent gvbe) {
            RaiseGetBoolValueEvent(EventGetIsReadOnly, gvbe);
        }

        protected void OnGetTypeConverterAndTypeEditor(GetTypeConverterAndTypeEditorEvent e) {
            try {
                com2props.AlwaysValid = com2props.CheckValid();
                GetTypeConverterAndTypeEditorEventHandler handler = (GetTypeConverterAndTypeEditorEventHandler)Events[EventGetTypeConverterAndTypeEditor];
                if (handler != null) handler(this, e);  
            }
            finally {
                com2props.AlwaysValid = false;
            }
        }
        
        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.OnResetValue"]/*' />
        /// <devdoc>
        ///     Raises the appropriate event
        /// </devdoc>
        protected void OnResetValue(EventArgs e) {
            RaiseCom2Event(EventResetValue, e);
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.OnShouldSerializeValue"]/*' />
        /// <devdoc>
        ///     Raises the appropriate event
        /// </devdoc>
        protected void OnShouldSerializeValue(GetBoolValueEvent gvbe) {
            RaiseGetBoolValueEvent(EventShouldSerializeValue, gvbe);
        }


        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.OnShouldRefresh"]/*' />
        /// <devdoc>
        ///     Raises the appropriate event
        /// </devdoc>
        protected void OnShouldRefresh(GetRefreshStateEvent gvbe) {
            RaiseGetBoolValueEvent(EventShouldRefresh, gvbe);
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.RaiseGetBoolValueEvent"]/*' />
        /// <devdoc>
        ///     Raises the appropriate event
        /// </devdoc>
        private void RaiseGetBoolValueEvent(Object key, GetBoolValueEvent e) {
            try {
                com2props.AlwaysValid = com2props.CheckValid();
                GetBoolValueEventHandler handler = (GetBoolValueEventHandler)Events[key];
                if (handler != null) handler(this, e);
            }
            finally {
                com2props.AlwaysValid = false;
            }
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.RaiseCom2Event"]/*' />
        /// <devdoc>
        ///     Raises the appropriate event
        /// </devdoc>
        private void RaiseCom2Event(Object key, EventArgs e) {
            try {
                com2props.AlwaysValid = com2props.CheckValid();
                Com2EventHandler handler = (Com2EventHandler)Events[key];
                if (handler != null) handler(this, e);
            }
            finally {
                com2props.AlwaysValid = false;
            }
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.RaiseGetNameItemEvent"]/*' />
        /// <devdoc>
        ///     Raises the appropriate event
        /// </devdoc>
        private void RaiseGetNameItemEvent(Object key, GetNameItemEvent e) {
            try {
               com2props.AlwaysValid = com2props.CheckValid();
                GetNameItemEventHandler handler = (GetNameItemEventHandler)Events[key];
                if (handler != null) handler(this, e);
            }
            finally {
                com2props.AlwaysValid = false;
            }
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.ResetValue"]/*' />
        /// <devdoc>
        ///     Will reset the default value for this property on the component.  If
        ///     there was a default value passed in as a DefaultValueAttribute, that
        ///     value will be set as the value of the property on the component.  If
        ///     there was no default value passed in, a ResetXXX method will be looked
        ///     for.  If one is found, it will be invoked.  If one is not found, this
        ///     is a nop.
        /// </devdoc>
        public override void ResetValue(Object component) {
            if (component is ICustomTypeDescriptor) {
                component = ((ICustomTypeDescriptor)component).GetPropertyOwner(this);
            }
        
            if (component == this.TargetObject) {
                OnResetValue(EventArgs.Empty);
            }
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.SetNeedsRefresh"]/*' />
        /// <devdoc>
        ///     Sets whether the particular item(s) need refreshing.
        /// </devdoc>
        internal void SetNeedsRefresh(int mask, bool value){
            if (value) {
                refreshState |= mask;
            }
            else {
                refreshState &= ~mask;
            }
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.SetValue"]/*' />
        /// <devdoc>
        ///     This will set value to be the new value of this property on the
        ///     component by invoking the setXXX method on the component.  If the
        ///     value specified is invalid, the component should throw an exception
        ///     which will be passed up.  The component designer should design the
        ///     property so that getXXX following a setXXX should return the value
        ///     passed in if no exception was thrown in the setXXX call.
        /// </devdoc>
        [ResourceExposure(ResourceScope.None)]
        [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)]
        public override void SetValue(Object component, Object value) {

            if (this.readOnly) {
                throw new NotSupportedException(string.Format(SR.COM2ReadonlyProperty, this.Name ));
            }

            object owner = component;
            if (owner is ICustomTypeDescriptor) {
                owner = ((ICustomTypeDescriptor)owner).GetPropertyOwner(this);
            }

            if (owner == null || !Marshal.IsComObject(owner) || !(owner is UnsafeNativeMethods.IDispatch)) {
                return;
            }

            // do we need to convert the type?
            if (valueConverter != null) {
                bool cancel = false;
                value = valueConverter.ConvertManagedToNative(value, this, ref cancel);
                if (cancel) {
                    return;
                }
            }

            UnsafeNativeMethods.IDispatch pDisp = (UnsafeNativeMethods.IDispatch)owner;

            NativeMethods.tagDISPPARAMS dp = new NativeMethods.tagDISPPARAMS();
            NativeMethods.tagEXCEPINFO excepInfo = new NativeMethods.tagEXCEPINFO();
            dp.cArgs = 1;
            dp.cNamedArgs = 1;
            int[] namedArgs = new int[]{NativeMethods.DISPID_PROPERTYPUT};
            GCHandle gcHandle = GCHandle.Alloc(namedArgs, GCHandleType.Pinned);

            try {
                dp.rgdispidNamedArgs = Marshal.UnsafeAddrOfPinnedArrayElement(namedArgs, 0);
                IntPtr mem = Marshal.AllocCoTaskMem( 16 /*Marshal.SizeOf(typeof(VARIANT)) */);
                SafeNativeMethods.VariantInit(new HandleRef(null, mem));
                Marshal.GetNativeVariantForObject(value, mem);
                dp.rgvarg = mem;
                try {

                    Guid g = Guid.Empty;
                    int hr = pDisp.Invoke(this.dispid,
                                          ref g,
                                          SafeNativeMethods.GetThreadLCID(),
                                          NativeMethods.DISPATCH_PROPERTYPUT,
                                          dp,
                                          null,
                                          excepInfo, new IntPtr[1]);


                    string errorInfo = null;
                    if (hr == NativeMethods.DISP_E_EXCEPTION && excepInfo.scode != 0) {
                        hr = excepInfo.scode;
                        errorInfo = excepInfo.bstrDescription;
                    }

                    switch (hr) {
                    case NativeMethods.E_ABORT:
                    case NativeMethods.OLE_E_PROMPTSAVECANCELLED:
                        // cancelled checkout, etc.
                        return;
                    case NativeMethods.S_OK:
                    case NativeMethods.S_FALSE:
                        OnValueChanged(component, EventArgs.Empty);
                        lastValue = value;
                        return;
                    default:
                        
                        //Debug.Fail(String.Format("IDispatch::Invoke(INVOKE_PROPPUT) returned hr=0x{0:X}", hr));
                        
                        if (pDisp is UnsafeNativeMethods.ISupportErrorInfo) {
                            g = typeof(UnsafeNativeMethods.IDispatch).GUID;
                            if (NativeMethods.Succeeded(((UnsafeNativeMethods.ISupportErrorInfo)pDisp).InterfaceSupportsErrorInfo(ref g))) {
                                UnsafeNativeMethods.IErrorInfo pErrorInfo = null;
                                UnsafeNativeMethods.GetErrorInfo(0, ref pErrorInfo);
                                string info= null;
                                if (pErrorInfo != null) {
                                    if (NativeMethods.Succeeded(pErrorInfo.GetDescription(ref info))) {
                                        errorInfo = info;
                                    }
                                }
                                
                            }
                        }
                        else if (errorInfo == null) {
                            StringBuilder strMessage = new StringBuilder(256);
                        
                            int result = SafeNativeMethods.FormatMessage(NativeMethods.FORMAT_MESSAGE_FROM_SYSTEM | 
                                                                    NativeMethods.FORMAT_MESSAGE_IGNORE_INSERTS,
                                                                    NativeMethods.NullHandleRef, 
                                                                    hr,
                                                                    CultureInfo.CurrentCulture.LCID,
                                                                    strMessage,
                                                                    255,
                                                                    NativeMethods.NullHandleRef);
                            
                            
                            if (result == 0) {   
                                errorInfo = String.Format(CultureInfo.CurrentCulture, string.Format(SR.DispInvokeFailed, "SetValue", hr));
                            }
                            else {       
                                errorInfo = strMessage.ToString();
                                // strip of any trailing cr/lf
                                while (errorInfo.Length > 0 && 
                                        errorInfo[errorInfo.Length -1] == '\n' ||
                                        errorInfo[errorInfo.Length -1] == '\r') {
                                    errorInfo = errorInfo.Substring(0, errorInfo.Length-1);
                                }    
                            }
                        }
                        throw new ExternalException(errorInfo, hr);
                    }
                }
                finally {
                    SafeNativeMethods.VariantClear(new HandleRef(null, mem));
                    Marshal.FreeCoTaskMem(mem);
                }
            }
            finally {
                gcHandle.Free();
            }
        }

        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.ShouldSerializeValue"]/*' />
        /// <devdoc>
        ///     Indicates whether the value of this property needs to be persisted. In
        ///     other words, it indicates whether the state of the property is distinct
        ///     from when the component is first instantiated. If there is a default
        ///     value specified in this PropertyDescriptor, it will be compared against the
        ///     property's current value to determine this.  If there is't, the
        ///     shouldPersistXXX method is looked for and invoked if found.  If both
        ///     these routes fail, true will be returned.
        ///
        ///     If this returns false, a tool should not persist this property's value.
        /// </devdoc>
        public override bool ShouldSerializeValue(Object component) {
            GetBoolValueEvent gbv = new GetBoolValueEvent(false);
            OnShouldSerializeValue(gbv);
            return gbv.Value;
        }
        /// <include file='doc\COM2PropertyDescriptor.uex' path='docs/doc[@for="Com2PropertyDescriptor.Com2PropDescMainConverter"]/*' />
        /// <devdoc>
        /// we wrap all value editors in this one so we can intercept
        /// the GetTextFromValue calls for objects that would like
        /// to modify the display name
        /// </devdoc>
        private class Com2PropDescMainConverter : Com2ExtendedTypeConverter {
            Com2PropertyDescriptor pd;
            
            private const int CheckSubprops = 0;
            private const int AllowSubprops = 1;
            private const int SupressSubprops = 2;
            
            
            private int subprops = CheckSubprops;
            
            public Com2PropDescMainConverter(Com2PropertyDescriptor pd, TypeConverter baseConverter) : base(baseConverter) {
                  this.pd = pd;
            }
            
            public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType) {
                  Object baseConversion = base.ConvertTo(context, culture, value, destinationType);
                  if (destinationType == typeof(string)) {
                      // if this is our current value, ask if it should be changed for display,
                      // otherwise we'll ask for our enum drop downs, which we don't wanna do!
                      //
                      if (pd.IsCurrentValue(value)) {
                          // don't ever do this for enum types
                          if (!pd.PropertyType.IsEnum) {
                              Com2EnumConverter baseConverter = (Com2EnumConverter)GetWrappedConverter(typeof(Com2EnumConverter));
                              if (baseConverter == null) {
                                return pd.GetDisplayValue((string)baseConversion);
                              }
                              else {
                                  return baseConverter.ConvertTo(value, destinationType);
                              }
                          }
                      }
                  }
                  return baseConversion;
            }
            
            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(value, attributes);

                if (props != null && props.Count > 0) {
                    // Return sorted read-only collection (can't sort original because its read-only)
                    props = props.Sort();
                    PropertyDescriptor[] descs = new PropertyDescriptor[props.Count];
                    props.CopyTo(descs, 0);
                    props = new PropertyDescriptorCollection(descs, true);
                }

                return props;
            }  
            
            public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
                  if (subprops == CheckSubprops) {
                     if (!base.GetPropertiesSupported(context)){
                        subprops = SupressSubprops;
                     }
                     else {
                        // special case the font converter here.
                        //
                        if ((pd.valueConverter != null && pd.valueConverter.AllowExpand) || Com2IVsPerPropertyBrowsingHandler.AllowChildProperties(this.pd)) {
                           subprops = AllowSubprops;
                        }
                     }
                  }
                  return (subprops == AllowSubprops);
            }
        }
    }

    internal class GetAttributesEvent : EventArgs {
        private ArrayList attrList;

        public GetAttributesEvent(ArrayList attrList) {
            this.attrList = attrList;

        }

        public void Add(Attribute attribute) {
            attrList.Add(attribute);
        }
    }

    internal delegate void Com2EventHandler(Com2PropertyDescriptor sender, EventArgs e);

    internal delegate void GetAttributesEventHandler(Com2PropertyDescriptor sender, GetAttributesEvent gaevent);

    internal class GetNameItemEvent : EventArgs {
        private Object nameItem;

        public GetNameItemEvent(Object defName) {
            this.nameItem = defName;

        }

        public Object Name{
            get{
                return nameItem;
            }
            set{
                nameItem = value;
            }
        }

        public string NameString{
            get{
                if (nameItem != null) {
                    return nameItem.ToString();
                }
                return "";
            }
        }
    }


    internal delegate void GetNameItemEventHandler(Com2PropertyDescriptor sender, GetNameItemEvent gnievent);


    internal class GetBoolValueEvent : EventArgs {
        private bool value;

        public GetBoolValueEvent(bool defValue) {
            this.value= defValue;

        }

        public bool Value{
            get{
                return value;
            }
            set{
                this.value = value;
            }
        }
    }

    internal delegate void GetBoolValueEventHandler(Com2PropertyDescriptor sender, GetBoolValueEvent gbeevent);

    internal class GetRefreshStateEvent : GetBoolValueEvent {

        Com2ShouldRefreshTypes item;

        public GetRefreshStateEvent(Com2ShouldRefreshTypes item, bool defValue) : base(defValue) {
            this.item = item;
        }
    }

    internal delegate void GetTypeConverterAndTypeEditorEventHandler(Com2PropertyDescriptor sender, GetTypeConverterAndTypeEditorEvent e);
    
    internal class GetTypeConverterAndTypeEditorEvent : EventArgs {
        private TypeConverter typeConverter;
        private Object        typeEditor;

        public GetTypeConverterAndTypeEditorEvent(TypeConverter typeConverter, Object typeEditor) {
            this.typeEditor = typeEditor;
            this.typeConverter = typeConverter;
        }
        
        public TypeConverter TypeConverter{
            get{
                return typeConverter;
            }
            set{
                typeConverter = value;
            }
        }
        
        public Object TypeEditor{
            get{
                return typeEditor;
            }
            set{
                typeEditor = value;
            }
        }
    }

}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.Collections;    
    using System.Collections.Generic;    
    using System.ComponentModel.Design;
    using Microsoft.Win32;

    /// <include file='doc\ComNativeDescriptor.uex' path='docs/doc[@for="ComNativeDescriptor"]/*' />
    /// <devdoc>
    ///     Top level mapping layer between COM Object and TypeDescriptor.
    ///
    /// </devdoc>
    internal class ComNativeDescriptor : TypeDescriptionProvider {
      
        private static ComNativeDescriptor handler = null;

        private AttributeCollection staticAttrs = new AttributeCollection(new Attribute[]{BrowsableAttribute.Yes, DesignTimeVisibleAttribute.No});

        /// <include file='doc\ComNativeDescriptor.uex' path='docs/doc[@for="ComNativeDescriptor.nativeProps"]/*' />
        /// <devdoc>
        /// Our collection of Object managers (Com2Properties) for native properties
        /// </devdoc>
        private WeakHashtable  nativeProps = new WeakHashtable();
        
        /// <include file='doc\ComNativeDescriptor.uex' path='docs/doc[@for="ComNativeDescriptor.extendedBrowsingHandlers"]/*' />
        /// <devdoc>
        /// Our collection of browsing handlers, which are stateless and shared across objects.
        /// </devdoc>
        private Hashtable         extendedBrowsingHandlers = new Hashtable();
        
        /// <include file='doc\ComNativeDescriptor.uex' path='docs/doc[@for="ComNativeDescriptor.clearCount"]/*' />
        /// <devdoc>
        /// We increment this every time we look at an Object, at specified
        /// intervals, we run through the properies list to see if we should
        /// delete any.
        /// </devdoc>
        private int               clearCount  = 0;
        private const  int        CLEAR_INTERVAL = 25;

        internal static ComNativeDescriptor Instance {
            get {
                if (handler == null) {
                    handler = new ComNativeDescriptor();
                }
                return handler;
            }
        }   


        [
            System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")
        ]
        // called via reflection for AutomationExtender stuff. Don't delete!
        //
        public static object GetNativePropertyValue(object component, string propertyName, ref bool succeeded) 
        {
            return Instance.GetPropertyValue(component, propertyName, ref succeeded);
        }


        /// <devdoc>
        ///     This method returns a custom type descriptor for the given type / object.  
        ///     The objectType parameter is always valid, but the instance parameter may 
        ///     be null if no instance was passed to TypeDescriptor.  The method should 
        ///     return a custom type descriptor for the object.  If the method is not 
        ///     interested in providing type information for the object it should 
        ///     return null.
        ///
        ///     This method is prototyped as virtual, and by default returns null 
        ///     if no parent provider was passed.  If a parent provider was passed, 
        ///     this method will invoke the parent provider's GetTypeDescriptor 
        ///     method.
        /// </devdoc>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new ComTypeDescriptor(this, instance);
        }

        internal string GetClassName(Object component) {

            string name = null;

            // does IVsPerPropretyBrowsing supply us a name?
            if (component is NativeMethods.IVsPerPropertyBrowsing) {
               int hr = ((NativeMethods.IVsPerPropertyBrowsing)component).GetClassName(ref name);
               if (NativeMethods.Succeeded(hr) && name != null) {
                  return name;
               }
               // otherwise fall through...
            }

            UnsafeNativeMethods.ITypeInfo  pTypeInfo = Com2TypeInfoProcessor.FindTypeInfo(component, true);

            if (pTypeInfo == null) {
                //Debug.Fail("The current component failed to return an ITypeInfo");
                return "";
            }

            if (pTypeInfo != null) {
                string desc = null;
                try {
                    pTypeInfo.GetDocumentation(NativeMethods.MEMBERID_NIL, ref name, ref desc, null, null);
                    
                    // strip the leading underscores
                    while (name != null && name.Length > 0 && name[0] == '_') {
                        name = name.Substring(1);
                    }
                    return name;
                }
                catch {
                }
            }
            return "";
        }
        
        internal TypeConverter GetConverter(Object component) {
            return TypeDescriptor.GetConverter(typeof(IComponent));
        }
        
        internal Object GetEditor(Object component, Type baseEditorType) {
            return TypeDescriptor.GetEditor(component.GetType(), baseEditorType);
        }

        internal string GetName(Object component) {

            if (!(component is UnsafeNativeMethods.IDispatch)) {
                return "";
            }
            
            int dispid = Com2TypeInfoProcessor.GetNameDispId((UnsafeNativeMethods.IDispatch)component);
            if (dispid != NativeMethods.MEMBERID_NIL) {
                bool success = false;
                object value = GetPropertyValue(component, dispid, ref success);
                
                if (success && value != null) {
                    return value.ToString();
                }
            }
            return "";
        }

        internal Object GetPropertyValue(Object component, string propertyName, ref bool succeeded) {

            if (!(component is UnsafeNativeMethods.IDispatch)) {
                return null;
            }

            UnsafeNativeMethods.IDispatch iDispatch = (UnsafeNativeMethods.IDispatch)component;
            string[] names = new string[]{propertyName};
            int[] dispid = new int[1];
            dispid[0] = NativeMethods.DISPID_UNKNOWN;
            Guid g = Guid.Empty;
            try {
               int hr = iDispatch.GetIDsOfNames(ref g, names, 1, SafeNativeMethods.GetThreadLCID(), dispid);
   
               if (dispid[0] == NativeMethods.DISPID_UNKNOWN || NativeMethods.Failed(hr)) {
                   return null;
               }
            }
            catch {
                return null;   
            }
            return GetPropertyValue(component, dispid[0], ref succeeded);
        }

        internal Object GetPropertyValue(Object component, int dispid, ref bool succeeded) {
            if (!(component is UnsafeNativeMethods.IDispatch)) {
                return null;
            }
            Object[] pVarResult = new Object[1];
            if (GetPropertyValue(component, dispid, pVarResult) == NativeMethods.S_OK) {
                succeeded = true;
                return pVarResult[0];
            }
            else {
                succeeded = false;
                return null;
            }
        }

        internal int GetPropertyValue(Object component, int dispid, Object[] retval) {
            if (!(component is UnsafeNativeMethods.IDispatch)) {
                return NativeMethods.E_NOINTERFACE;
            }
            UnsafeNativeMethods.IDispatch iDispatch = (UnsafeNativeMethods.IDispatch)component;
            try {
                Guid g = Guid.Empty;
                NativeMethods.tagEXCEPINFO pExcepInfo = new NativeMethods.tagEXCEPINFO();
                int hr;

                try{

                   hr = iDispatch.Invoke(dispid,
                                             ref g,
                                             SafeNativeMethods.GetThreadLCID(),
                                             NativeMethods.DISPATCH_PROPERTYGET,
                                             new NativeMethods.tagDISPPARAMS(),
                                             retval,
                                             pExcepInfo, null);

                   /*if (hr != NativeMethods.S_OK){
                     Com2PropertyDescriptor.PrintExceptionInfo(pExcepInfo);

                   } */
                   if (hr == NativeMethods.DISP_E_EXCEPTION) {
                       hr = pExcepInfo.scode;
                   }

                }
                catch (ExternalException ex){
                    hr = ex.ErrorCode;
                }
                return hr;
            }
            catch {
                //Debug.Fail(e.ToString() + " " + component.GetType().GUID.ToString() + " " + component.ToString());
            }
            return NativeMethods.E_FAIL;
        }

        /// <include file='doc\ComNativeDescriptor.uex' path='docs/doc[@for="ComNativeDescriptor.IsNameDispId"]/*' />
        /// <devdoc>
        /// Checks if the given dispid matches the dispid that the Object would like to specify
        /// as its identification proeprty (Name, ID, etc).
        /// </devdoc>
        internal bool IsNameDispId(Object obj, int dispid) {
            if (obj == null || !obj.GetType().IsCOMObject) {
                return false;
            }
            return dispid == Com2TypeInfoProcessor.GetNameDispId((UnsafeNativeMethods.IDispatch)obj);
        }

        /// <include file='doc\ComNativeDescriptor.uex' path='docs/doc[@for="ComNativeDescriptor.CheckClear"]/*' />
        /// <devdoc>
        /// Checks all our property manages to see if any have become invalid.
        /// </devdoc>
        private void CheckClear(Object component) {
            
            // walk the list every so many calls
            if ((++clearCount % CLEAR_INTERVAL) == 0) {
            
               lock(nativeProps) {
                   clearCount = 0;   

                   List<object> disposeList = null;
                   Com2Properties entry;

                   // first walk the list looking for items that need to be
                   // cleaned out.
                   //
                   foreach(DictionaryEntry de in nativeProps) {
                        entry = de.Value as Com2Properties;

                        if (entry != null && entry.TooOld) {
                            if (disposeList == null) {
                                disposeList = new List<object>(3);
                            }
                            disposeList.Add(de.Key);
                        }
                   }

                   // now run through the ones that are dead and dispose them.
                   // there's going to be a very small number of these.
                   //
                   if (disposeList != null) {
                       object oldKey; 
                       for (int i = disposeList.Count - 1; i >= 0; i--) {
                          oldKey = disposeList[i]; 
                          entry = nativeProps[oldKey] as Com2Properties;
                        
                          if (entry != null) {
                               entry.Disposed -= new EventHandler(OnPropsInfoDisposed);
                               entry.Dispose();
                               nativeProps.Remove(oldKey);
                          }
                       }
                   }
                }
            }
        }

        /// <include file='doc\ComNativeDescriptor.uex' path='docs/doc[@for="ComNativeDescriptor.GetPropsInfo"]/*' />
        /// <devdoc>
        /// Gets the properties manager for an Object.
        /// </devdoc>
        private Com2Properties GetPropsInfo(Object component) {
            // check caches if necessary
            //
            CheckClear(component);

            // Get the property info Object
            //
            Com2Properties propsInfo = (Com2Properties)nativeProps[component];
            
            // if we dont' have one, create one and set it up
            //
            if (propsInfo == null || !propsInfo.CheckValid()) {
                propsInfo = Com2TypeInfoProcessor.GetProperties(component);
                if (propsInfo != null) {                    
                    propsInfo.Disposed += new EventHandler(OnPropsInfoDisposed);                    
                    nativeProps.SetWeak(component, propsInfo);
                    propsInfo.AddExtendedBrowsingHandlers(extendedBrowsingHandlers);
                }
            }
            return propsInfo;
        }

        /// <include file='doc\ComNativeDescriptor.uex' path='docs/doc[@for="ComNativeDescriptor.GetAttributes"]/*' />
        /// <devdoc>
        /// Got attributes?
        /// </devdoc>
        internal AttributeCollection GetAttributes(Object component) {

            ArrayList attrs = new ArrayList();

            if (component is NativeMethods.IManagedPerPropertyBrowsing) {
                Object[] temp = Com2IManagedPerPropertyBrowsingHandler.GetComponentAttributes((NativeMethods.IManagedPerPropertyBrowsing)component, NativeMethods.MEMBERID_NIL);
                for (int i = 0; i < temp.Length; ++i) {
                    attrs.Add(temp[i]);
                }
            }
            
            if (Com2ComponentEditor.NeedsComponentEditor(component)) {
                EditorAttribute a = new EditorAttribute(typeof(Com2ComponentEditor), typeof(ComponentEditor));
                attrs.Add(a);
            }

            if (attrs == null || attrs.Count == 0) {
                return staticAttrs;
            }
            else {
                Attribute[] temp = new Attribute[attrs.Count];
                attrs.CopyTo(temp, 0);
                return new AttributeCollection(temp);
            }
        }

        /// <include file='doc\ComNativeDescriptor.uex' path='docs/doc[@for="ComNativeDescriptor.GetDefaultProperty"]/*' />
        /// <devdoc>
        /// Default Property, please
        /// </devdoc>
        internal PropertyDescriptor GetDefaultProperty(Object component) {
            CheckClear(component);

            Com2Properties propsInfo = GetPropsInfo(component);
            if (propsInfo != null) {
                return propsInfo.DefaultProperty;
            }
            return null;
        }

        internal EventDescriptorCollection GetEvents(Object component) {
            return new EventDescriptorCollection(null);
        }

        internal EventDescriptorCollection GetEvents(Object component, Attribute[] attributes) {
            return new EventDescriptorCollection(null);
        }

        internal EventDescriptor GetDefaultEvent(Object component) {
            return null;
        }

        /// <devdoc>
        /// Props!
        /// </devdoc>
        [SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")]
        internal PropertyDescriptorCollection GetProperties(Object component, Attribute[] attributes) {
            
            Com2Properties propsInfo = GetPropsInfo(component);

            if (propsInfo == null) {
                return PropertyDescriptorCollection.Empty;
            }

            try {
                propsInfo.AlwaysValid = true;
                PropertyDescriptor[] props = propsInfo.Properties;
                
                //Debug.Assert(propDescList.Count > 0, "Didn't add any properties! (propInfos=0)");
                return new PropertyDescriptorCollection(props);
            }
            finally {
                propsInfo.AlwaysValid = false;
            }
        }

        /// <devdoc>
        /// Fired when the property info gets disposed.
        /// </devdoc>        
        private void OnPropsInfoDisposed(object sender, EventArgs e) {
            Com2Properties propsInfo = sender as Com2Properties;

            if (propsInfo != null) {
                propsInfo.Disposed -= new EventHandler(OnPropsInfoDisposed);

                lock(nativeProps) {

                    // find the key
                    object key = propsInfo.TargetObject;

                    if (key == null && nativeProps.ContainsValue(propsInfo)) {
                        // need to find it - the target object has probably been cleaned out
                        // of the Com2Properties object already, so we run through the
                        // hashtable looking for the value, so we know what key to remove.
                        //
                        foreach (DictionaryEntry de in nativeProps) {
                            if (de.Value == propsInfo) {
                                key = de.Key;
                                break;                                    
                            }
                        }

                        if (key == null)  {
                            Debug.Fail("Failed to find Com2 properties key on dispose.");
                            return;
                        }
                    }
                    
                    nativeProps.Remove(key);
               }
            }
        }

        /// <include file='doc\ComNativeDescriptor.uex' path='docs/doc[@for="ComNativeDescriptor.ResolveVariantTypeConverterAndTypeEditor"]/*' />
        /// <devdoc>
        /// Looks at at value's type and creates an editor based on that.  We use this to decide which editor to use
        /// for a generic variant.
        /// </devdoc>
        internal static void ResolveVariantTypeConverterAndTypeEditor(Object propertyValue, ref TypeConverter currentConverter, Type editorType, ref Object currentEditor) {

            Object curValue = propertyValue;
            if (curValue != null && curValue != null && !Convert.IsDBNull(curValue)){
                  Type t = curValue.GetType();
                  TypeConverter subConverter = TypeDescriptor.GetConverter(t);
                  if (subConverter != null && subConverter.GetType() != typeof(TypeConverter)){
                     currentConverter = subConverter;
                  }
                  Object subEditor = TypeDescriptor.GetEditor(t, editorType);
                  if (subEditor != null) {
                     currentEditor = subEditor;
                  }
            }
        }

        /// <devdoc>
        ///     This type descriptor sits on top of a ComNativeDescriptor
        /// </devdoc>
        private sealed class ComTypeDescriptor : ICustomTypeDescriptor
        {
            private ComNativeDescriptor         _handler;
            private object                      _instance;

            /// <devdoc>
            ///     Creates a new WalkingTypeDescriptor.
            /// </devdoc>
            internal ComTypeDescriptor(ComNativeDescriptor handler, object instance)
            {
                _handler = handler;
                _instance = instance;
            }

            /// <devdoc>
            ///     ICustomTypeDescriptor implementation.
            /// </devdoc>
            AttributeCollection ICustomTypeDescriptor.GetAttributes()
            {
                return _handler.GetAttributes(_instance);
            }

            /// <devdoc>
            ///     ICustomTypeDescriptor implementation.
            /// </devdoc>
            string ICustomTypeDescriptor.GetClassName()
            {
                return _handler.GetClassName(_instance);
            }

            /// <devdoc>
            ///     ICustomTypeDescriptor implementation.
            /// </devdoc>
            string ICustomTypeDescriptor.GetComponentName()
            {
                return _handler.GetName(_instance);
            }

            /// <devdoc>
            ///     ICustomTypeDescriptor implementation.
            /// </devdoc>
            TypeConverter ICustomTypeDescriptor.GetConverter()
            {
                return _handler.GetConverter(_instance);
            }

            /// <devdoc>
            ///     ICustomTypeDescriptor implementation.
            /// </devdoc>
            EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
            {
                return _handler.GetDefaultEvent(_instance);
            }

            /// <devdoc>
            ///     ICustomTypeDescriptor implementation.
            /// </devdoc>
            PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
            {
                return _handler.GetDefaultProperty(_instance);
            }

            /// <devdoc>
            ///     ICustomTypeDescriptor implementation.
            /// </devdoc>
            object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
            {
                return _handler.GetEditor(_instance, editorBaseType);
            }

            /// <devdoc>
            ///     ICustomTypeDescriptor implementation.
            /// </devdoc>
            EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
            {
                return _handler.GetEvents(_instance);
            }

            /// <devdoc>
            ///     ICustomTypeDescriptor implementation.
            /// </devdoc>
            EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
            {
                return _handler.GetEvents(_instance, attributes);
            }

            /// <devdoc>
            ///     ICustomTypeDescriptor implementation.
            /// </devdoc>
            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
            {
                return _handler.GetProperties(_instance, null);
            }

            /// <devdoc>
            ///     ICustomTypeDescriptor implementation.
            /// </devdoc>
            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
            {
                return _handler.GetProperties(_instance, attributes);
            }

            /// <devdoc>
            ///     ICustomTypeDescriptor implementation.
            /// </devdoc>
            object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
            {
                return _instance;
            }
        }
    }
}

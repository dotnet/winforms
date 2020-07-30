// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Interop;
using static Interop.Ole32;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  Top level mapping layer between COM Object and TypeDescriptor.
    /// </summary>
    internal class ComNativeDescriptor : TypeDescriptionProvider
    {
        private static ComNativeDescriptor handler;

        private readonly AttributeCollection staticAttrs = new AttributeCollection(new Attribute[] { BrowsableAttribute.Yes, DesignTimeVisibleAttribute.No });

        /// <summary>
        ///  Our collection of Object managers (Com2Properties) for native properties
        /// </summary>
        private readonly WeakHashtable nativeProps = new WeakHashtable();

        /// <summary>
        ///  Our collection of browsing handlers, which are stateless and shared across objects.
        /// </summary>
        private readonly Hashtable extendedBrowsingHandlers = new Hashtable();

        /// <summary>
        ///  We increment this every time we look at an Object, at specified
        ///  intervals, we run through the properies list to see if we should
        ///  delete any.
        /// </summary>
        private int clearCount;
        private const int CLEAR_INTERVAL = 25;

        internal static ComNativeDescriptor Instance
        {
            get
            {
                if (handler is null)
                {
                    handler = new ComNativeDescriptor();
                }
                return handler;
            }
        }

        // Called via reflection for AutomationExtender stuff. Don't delete!
        public static object GetNativePropertyValue(object component, string propertyName, ref bool succeeded)
        {
            return Instance.GetPropertyValue(component, propertyName, ref succeeded);
        }

        /// <summary>
        ///  This method returns a custom type descriptor for the given type / object.
        ///  The objectType parameter is always valid, but the instance parameter may
        ///  be null if no instance was passed to TypeDescriptor.  The method should
        ///  return a custom type descriptor for the object.  If the method is not
        ///  interested in providing type information for the object it should
        ///  return null.
        ///
        ///  This method is prototyped as virtual, and by default returns null
        ///  if no parent provider was passed.  If a parent provider was passed,
        ///  this method will invoke the parent provider's GetTypeDescriptor
        ///  method.
        /// </summary>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new ComTypeDescriptor(this, instance);
        }

        internal unsafe string GetClassName(object component)
        {
            string name = null;

            // does IVsPerPropretyBrowsing supply us a name?
            if (component is VSSDK.IVsPerPropertyBrowsing)
            {
                HRESULT hr = ((VSSDK.IVsPerPropertyBrowsing)component).GetClassName(ref name);
                if (hr.Succeeded() && name != null)
                {
                    return name;
                }
                // otherwise fall through...
            }

            Oleaut32.ITypeInfo pTypeInfo = Com2TypeInfoProcessor.FindTypeInfo(component, true);

            if (pTypeInfo is null)
            {
                return string.Empty;
            }

            using var nameBstr = new BSTR();
            pTypeInfo.GetDocumentation(DispatchID.MEMBERID_NIL, &nameBstr, null, null, null);
            return nameBstr.String.TrimStart('_').ToString();
        }

        internal TypeConverter GetConverter(object component)
        {
            return TypeDescriptor.GetConverter(typeof(IComponent));
        }

        internal object GetEditor(object component, Type baseEditorType)
        {
            return TypeDescriptor.GetEditor(component.GetType(), baseEditorType);
        }

        internal string GetName(object component)
        {
            if (!(component is Oleaut32.IDispatch))
            {
                return "";
            }

            DispatchID dispid = Com2TypeInfoProcessor.GetNameDispId((Oleaut32.IDispatch)component);
            if (dispid != DispatchID.UNKNOWN)
            {
                bool success = false;
                object value = GetPropertyValue(component, dispid, ref success);

                if (success && value != null)
                {
                    return value.ToString();
                }
            }

            return string.Empty;
        }

        internal unsafe object GetPropertyValue(object component, string propertyName, ref bool succeeded)
        {
            if (!(component is Oleaut32.IDispatch iDispatch))
            {
                return null;
            }

            string[] names = new string[] { propertyName };
            DispatchID dispid = DispatchID.UNKNOWN;
            Guid g = Guid.Empty;
            try
            {
                HRESULT hr = iDispatch.GetIDsOfNames(&g, names, 1, Kernel32.GetThreadLocale(), &dispid);
                if (dispid == DispatchID.UNKNOWN || !hr.Succeeded())
                {
                    return null;
                }

                return GetPropertyValue(component, dispid, ref succeeded);
            }
            catch
            {
                return null;
            }
        }

        internal object GetPropertyValue(object component, DispatchID dispid, ref bool succeeded)
        {
            if (!(component is Oleaut32.IDispatch))
            {
                return null;
            }
            object[] pVarResult = new object[1];
            if (GetPropertyValue(component, dispid, pVarResult) == HRESULT.S_OK)
            {
                succeeded = true;
                return pVarResult[0];
            }
            else
            {
                succeeded = false;
                return null;
            }
        }

        internal unsafe HRESULT GetPropertyValue(object component, DispatchID dispid, object[] retval)
        {
            if (!(component is Oleaut32.IDispatch iDispatch))
            {
                return HRESULT.E_NOINTERFACE;
            }

            try
            {
                Guid g = Guid.Empty;
                var pExcepInfo = new Oleaut32.EXCEPINFO();
                var dispParams = new Oleaut32.DISPPARAMS();
                try
                {
                    HRESULT hr = iDispatch.Invoke(
                        dispid,
                        &g,
                        Kernel32.GetThreadLocale(),
                        Oleaut32.DISPATCH.PROPERTYGET,
                        &dispParams,
                        retval,
                        &pExcepInfo,
                        null);
                    if (hr == HRESULT.DISP_E_EXCEPTION)
                    {
                        return pExcepInfo.scode;
                    }

                    return hr;
                }
                catch (ExternalException ex)
                {
                    return (HRESULT)ex.ErrorCode;
                }
            }
            catch
            {
            }

            return HRESULT.E_FAIL;
        }

        /// <summary>
        ///  Checks if the given dispid matches the dispid that the Object would like to specify
        ///  as its identification proeprty (Name, ID, etc).
        /// </summary>
        internal bool IsNameDispId(object obj, DispatchID dispid)
        {
            if (obj is null || !obj.GetType().IsCOMObject)
            {
                return false;
            }

            return dispid == Com2TypeInfoProcessor.GetNameDispId((Oleaut32.IDispatch)obj);
        }

        /// <summary>
        ///  Checks all our property manages to see if any have become invalid.
        /// </summary>
        private void CheckClear(object component)
        {
            // walk the list every so many calls
            if ((++clearCount % CLEAR_INTERVAL) == 0)
            {
                lock (nativeProps)
                {
                    clearCount = 0;

                    List<object> disposeList = null;
                    Com2Properties entry;

                    // first walk the list looking for items that need to be
                    // cleaned out.
                    //
                    foreach (DictionaryEntry de in nativeProps)
                    {
                        entry = de.Value as Com2Properties;

                        if (entry != null && entry.TooOld)
                        {
                            if (disposeList is null)
                            {
                                disposeList = new List<object>(3);
                            }
                            disposeList.Add(de.Key);
                        }
                    }

                    // now run through the ones that are dead and dispose them.
                    // there's going to be a very small number of these.
                    //
                    if (disposeList != null)
                    {
                        object oldKey;
                        for (int i = disposeList.Count - 1; i >= 0; i--)
                        {
                            oldKey = disposeList[i];
                            entry = nativeProps[oldKey] as Com2Properties;

                            if (entry != null)
                            {
                                entry.Disposed -= new EventHandler(OnPropsInfoDisposed);
                                entry.Dispose();
                                nativeProps.Remove(oldKey);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Gets the properties manager for an Object.
        /// </summary>
        private Com2Properties GetPropsInfo(object component)
        {
            // check caches if necessary
            //
            CheckClear(component);

            // Get the property info Object
            //
            Com2Properties propsInfo = (Com2Properties)nativeProps[component];

            // if we dont' have one, create one and set it up
            //
            if (propsInfo is null || !propsInfo.CheckValid())
            {
                propsInfo = Com2TypeInfoProcessor.GetProperties(component);
                if (propsInfo != null)
                {
                    propsInfo.Disposed += new EventHandler(OnPropsInfoDisposed);
                    nativeProps.SetWeak(component, propsInfo);
                    propsInfo.AddExtendedBrowsingHandlers(extendedBrowsingHandlers);
                }
            }
            return propsInfo;
        }

        /// <summary>
        ///  Got attributes?
        /// </summary>
        internal AttributeCollection GetAttributes(object component)
        {
            ArrayList attrs = new ArrayList();

            if (component is VSSDK.IVSMDPerPropertyBrowsing)
            {
                object[] temp = Com2IManagedPerPropertyBrowsingHandler.GetComponentAttributes((VSSDK.IVSMDPerPropertyBrowsing)component, DispatchID.MEMBERID_NIL);
                for (int i = 0; i < temp.Length; ++i)
                {
                    attrs.Add(temp[i]);
                }
            }

            if (Com2ComponentEditor.NeedsComponentEditor(component))
            {
                EditorAttribute a = new EditorAttribute(typeof(Com2ComponentEditor), typeof(ComponentEditor));
                attrs.Add(a);
            }

            if (attrs is null || attrs.Count == 0)
            {
                return staticAttrs;
            }
            else
            {
                Attribute[] temp = new Attribute[attrs.Count];
                attrs.CopyTo(temp, 0);
                return new AttributeCollection(temp);
            }
        }

        /// <summary>
        ///  Default Property, please
        /// </summary>
        internal PropertyDescriptor GetDefaultProperty(object component)
        {
            CheckClear(component);

            Com2Properties propsInfo = GetPropsInfo(component);
            if (propsInfo != null)
            {
                return propsInfo.DefaultProperty;
            }
            return null;
        }

        internal EventDescriptorCollection GetEvents(object component)
        {
            return new EventDescriptorCollection(null);
        }

        internal EventDescriptorCollection GetEvents(object component, Attribute[] attributes)
        {
            return new EventDescriptorCollection(null);
        }

        internal EventDescriptor GetDefaultEvent(object component)
        {
            return null;
        }

        /// <summary>
        ///  Props!
        /// </summary>
        internal PropertyDescriptorCollection GetProperties(object component)
        {
            Com2Properties propsInfo = GetPropsInfo(component);

            if (propsInfo is null)
            {
                return PropertyDescriptorCollection.Empty;
            }

            try
            {
                propsInfo.AlwaysValid = true;
                return new PropertyDescriptorCollection(propsInfo.Properties);
            }
            finally
            {
                propsInfo.AlwaysValid = false;
            }
        }

        /// <summary>
        ///  Fired when the property info gets disposed.
        /// </summary>
        private void OnPropsInfoDisposed(object sender, EventArgs e)
        {
            if (sender is Com2Properties propsInfo)
            {
                propsInfo.Disposed -= new EventHandler(OnPropsInfoDisposed);

                lock (nativeProps)
                {
                    // find the key
                    object key = propsInfo.TargetObject;

                    if (key is null && nativeProps.ContainsValue(propsInfo))
                    {
                        // need to find it - the target object has probably been cleaned out
                        // of the Com2Properties object already, so we run through the
                        // hashtable looking for the value, so we know what key to remove.
                        //
                        foreach (DictionaryEntry de in nativeProps)
                        {
                            if (de.Value == propsInfo)
                            {
                                key = de.Key;
                                break;
                            }
                        }

                        if (key is null)
                        {
                            Debug.Fail("Failed to find Com2 properties key on dispose.");
                            return;
                        }
                    }

                    nativeProps.Remove(key);
                }
            }
        }

        /// <summary>
        ///  Looks at at value's type and creates an editor based on that.  We use this to decide which editor to use
        ///  for a generic variant.
        /// </summary>
        internal static void ResolveVariantTypeConverterAndTypeEditor(object propertyValue, ref TypeConverter currentConverter, Type editorType, ref object currentEditor)
        {
            object curValue = propertyValue;
            if (curValue != null && curValue != null && !Convert.IsDBNull(curValue))
            {
                Type t = curValue.GetType();
                TypeConverter subConverter = TypeDescriptor.GetConverter(t);
                if (subConverter != null && subConverter.GetType() != typeof(TypeConverter))
                {
                    currentConverter = subConverter;
                }
                object subEditor = TypeDescriptor.GetEditor(t, editorType);
                if (subEditor != null)
                {
                    currentEditor = subEditor;
                }
            }
        }

        /// <summary>
        ///  This type descriptor sits on top of a ComNativeDescriptor
        /// </summary>
        private sealed class ComTypeDescriptor : ICustomTypeDescriptor
        {
            private readonly ComNativeDescriptor _handler;
            private readonly object _instance;

            /// <summary>
            ///  Creates a new WalkingTypeDescriptor.
            /// </summary>
            internal ComTypeDescriptor(ComNativeDescriptor handler, object instance)
            {
                _handler = handler;
                _instance = instance;
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            AttributeCollection ICustomTypeDescriptor.GetAttributes()
            {
                return _handler.GetAttributes(_instance);
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            string ICustomTypeDescriptor.GetClassName()
            {
                return _handler.GetClassName(_instance);
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            string ICustomTypeDescriptor.GetComponentName()
            {
                return _handler.GetName(_instance);
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            TypeConverter ICustomTypeDescriptor.GetConverter()
            {
                return _handler.GetConverter(_instance);
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
            {
                return _handler.GetDefaultEvent(_instance);
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
            {
                return _handler.GetDefaultProperty(_instance);
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
            {
                return _handler.GetEditor(_instance, editorBaseType);
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
            {
                return _handler.GetEvents(_instance);
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
            {
                return _handler.GetEvents(_instance, attributes);
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
            {
                return _handler.GetProperties(_instance);
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
            {
                return _handler.GetProperties(_instance);
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
            {
                return _instance;
            }
        }
    }
}

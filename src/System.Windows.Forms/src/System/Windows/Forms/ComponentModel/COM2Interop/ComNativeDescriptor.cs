﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using static Interop;
using static Interop.Ole32;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  Top level mapping layer between COM Object and TypeDescriptor.
    /// </summary>
    internal partial class ComNativeDescriptor : TypeDescriptionProvider
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
        ///  intervals, we run through the properties list to see if we should
        ///  delete any.
        /// </summary>
        private int clearCount;
        private const int CLEAR_INTERVAL = 25;

        internal static ComNativeDescriptor Instance
        {
            get
            {
                handler ??= new ComNativeDescriptor();

                return handler;
            }
        }

        // Called via reflection for AutomationExtender stuff. Don't delete!
        public static object GetNativePropertyValue(object component, string propertyName, ref bool succeeded)
        {
            return GetPropertyValue(component, propertyName, ref succeeded);
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
        public override ICustomTypeDescriptor GetTypeDescriptor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type objectType, object instance)
        {
            return new ComTypeDescriptor(this, instance);
        }

        internal static unsafe string GetClassName(object component)
        {
            string name = null;

            // does IVsPerPropertyBrowsing supply us a name?
            if (component is VSSDK.IVsPerPropertyBrowsing browsing)
            {
                HRESULT hr = browsing.GetClassName(ref name);
                if (hr.Succeeded && name is not null)
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

            using BSTR nameBstr = default(BSTR);
            pTypeInfo.GetDocumentation(DispatchID.MEMBERID_NIL, &nameBstr, null, null, null);
            return nameBstr.AsSpan().TrimStart('_').ToString();
        }

        internal static TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(typeof(IComponent));
        }

        internal static object GetEditor(object component, Type baseEditorType)
        {
            return TypeDescriptor.GetEditor(component.GetType(), baseEditorType);
        }

        internal static string GetName(object component)
        {
            if (component is not Oleaut32.IDispatch)
            {
                return string.Empty;
            }

            DispatchID dispid = Com2TypeInfoProcessor.GetNameDispId((Oleaut32.IDispatch)component);
            if (dispid != DispatchID.UNKNOWN)
            {
                bool success = false;
                object value = GetPropertyValue(component, dispid, ref success);

                if (success && value is not null)
                {
                    return value.ToString();
                }
            }

            return string.Empty;
        }

        internal static unsafe object GetPropertyValue(object component, string propertyName, ref bool succeeded)
        {
            if (component is not Oleaut32.IDispatch dispatch)
            {
                return null;
            }

            string[] names = new string[] { propertyName };
            DispatchID dispid = DispatchID.UNKNOWN;
            Guid g = Guid.Empty;
            try
            {
                HRESULT hr = dispatch.GetIDsOfNames(&g, names, 1, PInvoke.GetThreadLocale(), &dispid);
                if (dispid == DispatchID.UNKNOWN || !hr.Succeeded)
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

        internal static object GetPropertyValue(object component, DispatchID dispid, ref bool succeeded)
        {
            if (component is not Oleaut32.IDispatch)
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

        internal static unsafe HRESULT GetPropertyValue(object component, DispatchID dispid, object[] retval)
        {
            if (component is not Oleaut32.IDispatch dispatch)
            {
                return HRESULT.E_NOINTERFACE;
            }

            try
            {
                Guid g = Guid.Empty;
                EXCEPINFO pExcepInfo = default(EXCEPINFO);
                DISPPARAMS dispParams = default(DISPPARAMS);
                try
                {
                    HRESULT hr = dispatch.Invoke(
                        dispid,
                        &g,
                        PInvoke.GetThreadLocale(),
                        DISPATCH_FLAGS.DISPATCH_PROPERTYGET,
                        &dispParams,
                        retval,
                        &pExcepInfo,
                        null);

                    if (hr == HRESULT.DISP_E_EXCEPTION)
                    {
                        return (HRESULT)pExcepInfo.scode;
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
        ///  as its identification property (Name, ID, etc).
        /// </summary>
        internal static bool IsNameDispId(object obj, DispatchID dispid)
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
        private void CheckClear()
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

                        if (entry is not null && entry.NeedsRefreshed)
                        {
                            disposeList ??= new List<object>(3);

                            disposeList.Add(de.Key);
                        }
                    }

                    // now run through the ones that are dead and dispose them.
                    // there's going to be a very small number of these.
                    //
                    if (disposeList is not null)
                    {
                        object oldKey;
                        for (int i = disposeList.Count - 1; i >= 0; i--)
                        {
                            oldKey = disposeList[i];
                            entry = nativeProps[oldKey] as Com2Properties;

                            if (entry is not null)
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
            CheckClear();

            // Get the property info Object
            //
            Com2Properties propsInfo = (Com2Properties)nativeProps[component];

            // if we don't have one, create one and set it up
            //
            if (propsInfo is null || !propsInfo.CheckValidity())
            {
                propsInfo = Com2TypeInfoProcessor.GetProperties(component);
                if (propsInfo is not null)
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

            if (component is VSSDK.IVSMDPerPropertyBrowsing browsing)
            {
                object[] temp = Com2IManagedPerPropertyBrowsingHandler.GetComponentAttributes(browsing, DispatchID.MEMBERID_NIL);
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
            CheckClear();

            Com2Properties propsInfo = GetPropsInfo(component);
            if (propsInfo is not null)
            {
                return propsInfo.DefaultProperty;
            }

            return null;
        }

        internal static EventDescriptorCollection GetEvents()
        {
            return new EventDescriptorCollection(null);
        }

        internal static EventDescriptor GetDefaultEvent()
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
            if (curValue is not null && curValue is not null && !Convert.IsDBNull(curValue))
            {
                Type t = curValue.GetType();
                TypeConverter subConverter = TypeDescriptor.GetConverter(t);
                if (subConverter is not null && subConverter.GetType() != typeof(TypeConverter))
                {
                    currentConverter = subConverter;
                }

                object subEditor = TypeDescriptor.GetEditor(t, editorType);
                if (subEditor is not null)
                {
                    currentEditor = subEditor;
                }
            }
        }
    }
}

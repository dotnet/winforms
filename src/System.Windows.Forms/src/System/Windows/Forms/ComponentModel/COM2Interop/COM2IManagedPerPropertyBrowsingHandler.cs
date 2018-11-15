// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop {
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Reflection;
    using Microsoft.Win32;
    using System.Collections;
    using System.Globalization;

    [System.Security.SuppressUnmanagedCodeSecurityAttribute()]
    internal class Com2IManagedPerPropertyBrowsingHandler : Com2ExtendedBrowsingHandler {

        public override Type Interface {
            get {
                return typeof(NativeMethods.IManagedPerPropertyBrowsing);
            }
        }

        public override void SetupPropertyHandlers(Com2PropertyDescriptor[] propDesc) {
            if (propDesc == null) {
                return;
            }
            for (int i = 0; i < propDesc.Length; i++) {
                propDesc[i].QueryGetDynamicAttributes += new GetAttributesEventHandler(this.OnGetAttributes);
            }
        }

        /// <include file='doc\COM2IManagedPerPropertyBrowsingHandler.uex' path='docs/doc[@for="Com2IManagedPerPropertyBrowsingHandler.OnGetAttributes"]/*' />
        /// <devdoc>
        /// Here is where we handle IVsPerPropertyBrowsing.GetLocalizedPropertyInfo and IVsPerPropertyBrowsing.   HideProperty
        /// such as IPerPropertyBrowsing, IProvidePropertyBuilder, etc.
        /// </devdoc>
        private void OnGetAttributes(Com2PropertyDescriptor sender, GetAttributesEvent attrEvent) {
            Object target = sender.TargetObject;

            if (target is NativeMethods.IManagedPerPropertyBrowsing) {
                Attribute[] attrs = GetComponentAttributes((NativeMethods.IManagedPerPropertyBrowsing)target,sender.DISPID);
                if (attrs != null) {
                    for (int i = 0; i < attrs.Length; i++) {
                        attrEvent.Add(attrs[i]);
                    }
                }
            }
        }

        internal static Attribute[] GetComponentAttributes(NativeMethods.IManagedPerPropertyBrowsing target, int dispid) {
            int cItems = 0;
            IntPtr pbstrs = IntPtr.Zero;
            IntPtr pvars = IntPtr.Zero;

            int hr = target.GetPropertyAttributes(dispid, ref cItems, ref pbstrs, ref pvars);

            if (hr != NativeMethods.S_OK || cItems == 0) {
                return new Attribute[0];
            }

            ArrayList attrs = new ArrayList();

            string[] attrTypeNames = GetStringsFromPtr(pbstrs, cItems);
            Object[] varParams = GetVariantsFromPtr(pvars, cItems);

            Debug.Assert(attrTypeNames.Length == varParams.Length, "Mismatched parameter and attribute name length");
            if (attrTypeNames.Length != varParams.Length) {
                return new Attribute[0];
            }

            // get the types
            Type[] types = new Type[attrTypeNames.Length];
            for (int i = 0; i < attrTypeNames.Length; i++) {
                
                string attrName = attrTypeNames[i];
                
                // try the name first
                Type t = Type.GetType(attrName);
                Assembly a = null;
                
                if (t != null) {
                    a = t.Assembly;    
                }

                if (t == null) {


                    // check for an assembly name.
                    //
                    string assemblyName = "";

                    int comma = attrName.LastIndexOf(',');

                    if (comma != -1) {
                        assemblyName = attrName.Substring(comma);
                        attrName = attrName.Substring(0, comma);
                    }

                    string fieldName;
                    int lastDot = attrName.LastIndexOf('.');
                    if (lastDot != -1) {
                        fieldName = attrName.Substring(lastDot + 1);
                    }
                    else {
                        // somethings odd
                        Debug.Fail("No dot in class name?");
                        continue;
                    }

                    // try to get the field value
                    if (a == null) {
                        t = Type.GetType(attrName.Substring(0,lastDot) + assemblyName);
                    }
                    else {
                        t = a.GetType(attrName.Substring(0,lastDot) + assemblyName);
                    }

                    if (t == null){
                        Debug.Fail("Failed load attribute '" + attrName + assemblyName + "'.  It's Type could not be found.");
                        continue;
                    }

                    Debug.Assert(typeof(Attribute).IsAssignableFrom(t), "Attribute type " + t.FullName + " does not derive from Attribute");
                    if (!typeof(Attribute).IsAssignableFrom(t)) {
                        continue;
                    }

                    if (t != null) {
                        FieldInfo fi = t.GetField(fieldName);

                        // only if it's static
                        if (fi != null && fi.IsStatic) {
                            Object fieldValue = fi.GetValue(null);
                            if (fieldValue is Attribute) {
                                // add it to the list
                                attrs.Add(fieldValue);
                                continue;
                            }
                        }
                        else {
                            Debug.Fail("Couldn't load field '" + fieldName + "' from type '" + attrName.Substring(0,lastDot) + "'.  It does not exist or is not static");
                        }
                    }
                }


                Debug.Assert(typeof(Attribute).IsAssignableFrom(t), "Attribute type " + t.FullName + " does not derive from Attribute");
                if (!typeof(Attribute).IsAssignableFrom(t)) {
                    continue;
                }

                Attribute attr = null;

                // okay, if we got here, we need to build the attribute...
                // get the initalizer value if we've got a one item ctor

                if (!Convert.IsDBNull(varParams[i]) && varParams[i] != null) {
                    ConstructorInfo[] ctors = t.GetConstructors();
                    for (int c=0; c < ctors.Length; c++) {
                        ParameterInfo[] pis = ctors[c].GetParameters();
                        if (pis.Length == 1 && pis[0].ParameterType.IsAssignableFrom(varParams[i].GetType())) {
                            // found a one-parameter ctor, use it
                            // try to construct a default one
                            try {
                                attr = (Attribute)Activator.CreateInstance(t, new Object[]{varParams[i]});
                                attrs.Add(attr);
                            }
                            catch {
                                // nevermind
                                Debug.Fail("Attribute " + t.FullName + " did not have a initalizer specified and has no default constructor");
                                continue;
                            }
                        }
                    }
                }
                else {
                    // try to construct a default one
                    try {
                        attr = (Attribute)Activator.CreateInstance(t);
                        attrs.Add(attr);
                    }
                    catch {
                        // nevermind
                        Debug.Fail("Attribute " + t.FullName + " did not have a initalizer specified and has no default constructor");
                        continue;
                    }
                }
            }

            Attribute[] temp = new Attribute[attrs.Count];
            attrs.CopyTo(temp, 0);
            return temp;
        }

        private static string[] GetStringsFromPtr(IntPtr ptr, int cStrings) {
            if (ptr != IntPtr.Zero) {
                string[] strs = new string[cStrings];
                IntPtr bstr;
                for (int i = 0; i < cStrings; i++) {
                    try{
                        bstr = Marshal.ReadIntPtr(ptr, i*4);
                        if (bstr != IntPtr.Zero) {
                            strs[i] = Marshal.PtrToStringUni(bstr);
                            SafeNativeMethods.SysFreeString(new HandleRef(null, bstr));
                        }
                        else {
                            strs[i] = "";
                        }
                    }
                    catch (Exception ex) {
                        Debug.Fail("Failed to marshal component attribute BSTR " + i.ToString(CultureInfo.InvariantCulture), ex.ToString());
                    }
                }
                try{
                    Marshal.FreeCoTaskMem(ptr);
                }
                catch (Exception ex) {
                    Debug.Fail("Failed to free BSTR array memory", ex.ToString());
                }
                return strs;
            }
            else {
                return new string[0];
            }
        }

        private static Object[] GetVariantsFromPtr(IntPtr ptr, int cVariants) {
            if (ptr != IntPtr.Zero) {
                Object[] objects = new Object[cVariants];
                IntPtr curVariant;
                
                for (int i = 0; i < cVariants; i++) {
                    try{
                        curVariant = (IntPtr)((long)ptr + (i* 16 /*sizeof(VARIANT)*/));
                        if (curVariant != IntPtr.Zero) {
                            objects[i] = Marshal.GetObjectForNativeVariant(curVariant);
                            SafeNativeMethods.VariantClear(new HandleRef(null, curVariant));
                        }
                        else {
                            objects[i] = Convert.DBNull;
                        }
                    }
                    catch (Exception ex) {
                        Debug.Fail("Failed to marshal component attribute VARIANT " + i.ToString(CultureInfo.InvariantCulture), ex.ToString());
                    }
                }
                try{
                    Marshal.FreeCoTaskMem(ptr);
                }
                catch (Exception ex) {
                    Debug.Fail("Failed to free VARIANT array memory", ex.ToString());
                }
                return objects;
            }
            else {
                return new Object[cVariants];
            }
        }
    }
}

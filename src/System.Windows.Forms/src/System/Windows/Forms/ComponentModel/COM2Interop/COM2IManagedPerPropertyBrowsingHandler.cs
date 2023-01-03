// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal class Com2IManagedPerPropertyBrowsingHandler : Com2ExtendedBrowsingHandler
    {
        public override Type Interface => typeof(VSSDK.IVSMDPerPropertyBrowsing);

        public override void SetupPropertyHandlers(Com2PropertyDescriptor[]? propDesc)
        {
            if (propDesc is null)
            {
                return;
            }

            for (int i = 0; i < propDesc.Length; i++)
            {
                propDesc[i].QueryGetDynamicAttributes += new GetAttributesEventHandler(OnGetAttributes);
            }
        }

        /// <summary>
        ///  Here is where we handle IVsPerPropertyBrowsing.GetLocalizedPropertyInfo and IVsPerPropertyBrowsing.
        ///  Hide properties such as IPerPropertyBrowsing, IProvidePropertyBuilder, etc.
        /// </summary>
        private void OnGetAttributes(Com2PropertyDescriptor sender, GetAttributesEvent attrEvent)
        {
            object? target = sender.TargetObject;

            if (target is VSSDK.IVSMDPerPropertyBrowsing browsing)
            {
                Attribute[] attrs = GetComponentAttributes(browsing, sender.DISPID);
                for (int i = 0; i < attrs.Length; i++)
                {
                    attrEvent.Add(attrs[i]);
                }
            }
        }

        internal static unsafe Attribute[] GetComponentAttributes(VSSDK.IVSMDPerPropertyBrowsing target, Ole32.DispatchID dispid)
        {
            uint cItems = 0;
            IntPtr pbstrs = IntPtr.Zero;
            VARIANT* pvars = null;

            HRESULT hr = target.GetPropertyAttributes(dispid, &cItems, &pbstrs, &pvars);
            if (hr != HRESULT.S_OK || cItems == 0 || pvars is null)
            {
                return Array.Empty<Attribute>();
            }

            List<Attribute> attrs = new();

            string[] attrTypeNames = GetStringsFromPtr(pbstrs, cItems);
            object?[] varParams = GetVariantsFromPtr(pvars, cItems);

            Debug.Assert(attrTypeNames.Length == varParams.Length, "Mismatched parameter and attribute name length");
            if (attrTypeNames.Length != varParams.Length)
            {
                return Array.Empty<Attribute>();
            }

            // Get the types.
            for (int i = 0; i < attrTypeNames.Length; i++)
            {
                string attrName = attrTypeNames[i];

                // Try the name first.
                Type? t = null;
                if (attrName.Length > 0)
                {
                    t = Type.GetType(attrName);
                }

                Assembly? a = t?.Assembly;

                if (t is null)
                {
                    // Check for an assembly name.
                    string assemblyName = string.Empty;

                    int comma = attrName.LastIndexOf(',');

                    if (comma != -1)
                    {
                        assemblyName = attrName[comma..];
                        attrName = attrName[..comma];
                    }

                    string fieldName;
                    int lastDot = attrName.LastIndexOf('.');
                    if (lastDot != -1)
                    {
                        fieldName = attrName[(lastDot + 1)..];
                    }
                    else
                    {
                        Debug.Fail("No dot in class name?");
                        continue;
                    }

                    // Try to get the field value
                    t = a is null
                        ? Type.GetType(string.Concat(attrName.AsSpan(0, lastDot), assemblyName))
                        : a.GetType(string.Concat(attrName.AsSpan(0, lastDot), assemblyName));

                    if (t is null)
                    {
                        Debug.Fail($"Failed load attribute '{attrName}{assemblyName}'.  It's Type could not be found.");
                        continue;
                    }

                    Debug.Assert(typeof(Attribute).IsAssignableFrom(t), $"Attribute type {t.FullName} does not derive from Attribute");
                    if (!typeof(Attribute).IsAssignableFrom(t))
                    {
                        continue;
                    }

                    if (t is not null)
                    {
                        FieldInfo? fi = t.GetField(fieldName);

                        // only if it's static
                        if (fi is not null && fi.IsStatic)
                        {
                            if (fi.GetValue(null) is Attribute attribute)
                            {
                                // add it to the list
                                attrs.Add(attribute);
                                continue;
                            }
                        }
                        else
                        {
                            Debug.Fail($"Couldn't load field '{fieldName}' from type '{attrName[..lastDot]}'.  It does not exist or is not static");
                        }
                    }
                }

                Debug.Assert(typeof(Attribute).IsAssignableFrom(t), $"Attribute type {t.FullName} does not derive from Attribute");
                if (!typeof(Attribute).IsAssignableFrom(t))
                {
                    continue;
                }

                Attribute? attr;

                // Okay, if we got here, we need to build the attribute.
                // Get the initializer value if we've got a one item constructor.

                var varParam = varParams[i];
                if (!Convert.IsDBNull(varParam) && varParam is not null)
                {
                    ConstructorInfo[] ctors = t.GetConstructors();
                    for (int c = 0; c < ctors.Length; c++)
                    {
                        ParameterInfo[] pis = ctors[c].GetParameters();
                        if (pis.Length == 1 && pis[0].ParameterType.IsAssignableFrom(varParam.GetType()))
                        {
                            // Found a one-parameter ctor, use it to try to construct a default one.
                            try
                            {
                                attr = (Attribute?)Activator.CreateInstance(t, new object[] { varParam });
                                if (attr is not null) attrs.Add(attr);
                            }
                            catch
                            {
                                Debug.Fail($"Attribute {t.FullName} did not have a initializer specified and has no default constructor");
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    // Try to construct a default one.
                    try
                    {
                        attr = (Attribute?)Activator.CreateInstance(t);
                        if (attr is not null) attrs.Add(attr);
                    }
                    catch
                    {
                        Debug.Fail($"Attribute {t.FullName} did not have a initializer specified and has no default constructor");
                        continue;
                    }
                }
            }

            return attrs.ToArray();
        }

        private static string[] GetStringsFromPtr(IntPtr ptr, uint cStrings)
        {
            if (ptr == IntPtr.Zero)
            {
                return Array.Empty<string>();
            }

            string[] strs = new string[cStrings];
            IntPtr bstr;
            for (int i = 0; i < cStrings; i++)
            {
                try
                {
                    bstr = Marshal.ReadIntPtr(ptr, i * 4);
                    if (bstr != IntPtr.Zero)
                    {
                        strs[i] = Marshal.PtrToStringUni(bstr)!;
                        Oleaut32.SysFreeString(bstr);
                    }
                    else
                    {
                        strs[i] = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Fail($"Failed to marshal component attribute BSTR {i}", ex.ToString());
                }
            }

            try
            {
                Marshal.FreeCoTaskMem(ptr);
            }
            catch (Exception ex)
            {
                Debug.Fail("Failed to free BSTR array memory", ex.ToString());
            }

            return strs;
        }

        private static unsafe object?[] GetVariantsFromPtr(VARIANT* ptr, uint cVariants)
        {
            var objects = new object?[cVariants];
            for (int i = 0; i < cVariants; i++)
            {
                try
                {
                    using VARIANT variant = ptr[i];
                    objects[i] = variant.ToObject();
                }
                catch (Exception ex)
                {
                    Debug.Fail($"Failed to marshal component attribute VARIANT {i}", ex.ToString());
                }
            }

            try
            {
                Marshal.FreeCoTaskMem((IntPtr)ptr);
            }
            catch (Exception ex)
            {
                Debug.Fail("Failed to free VARIANT array memory", ex.ToString());
            }

            return objects;
        }
    }
}

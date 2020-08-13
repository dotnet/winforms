// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using static Interop;
using static Interop.Ole32;
using static Interop.Oleaut32;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This is the main worker class of Com2 property interop. It takes an IDispatch Object
    ///  and translates it's ITypeInfo into Com2PropertyDescriptor objects that are understandable
    ///  by managed code.
    ///
    ///  This class only knows how to process things that are natively in the typeinfo.  Other property
    ///  information such as IPerPropertyBrowsing is handled elsewhere.
    /// </summary>
    internal class Com2TypeInfoProcessor
    {
        private static readonly TraceSwitch DbgTypeInfoProcessorSwitch = new TraceSwitch("DbgTypeInfoProcessor", "Com2TypeInfoProcessor: debug Com2 type info processing");

        private Com2TypeInfoProcessor()
        {
        }

        private static ModuleBuilder moduleBuilder;

        private static ModuleBuilder ModuleBuilder
        {
            get
            {
                if (moduleBuilder is null)
                {
                    AssemblyName assemblyName = new AssemblyName
                    {
                        Name = "COM2InteropEmit"
                    };
                    AssemblyBuilder aBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                    moduleBuilder = aBuilder.DefineDynamicModule("COM2Interop.Emit");
                }
                return moduleBuilder;
            }
        }

        private static Hashtable builtEnums;
        private static Hashtable processedLibraries;

        /// <summary>
        ///  Given an Object, this attempts to locate its type ifo
        /// </summary>
        public static ITypeInfo FindTypeInfo(object obj, bool wantCoClass)
        {
            ITypeInfo pTypeInfo = null;

            // This is kind of odd.  What's going on here is that if we want the CoClass (e.g. for
            // the interface name), we need to look for IProvideClassInfo first, then look for the
            // typeinfo from the IDispatch. In the case of many Oleaut32 operations, the CoClass
            // doesn't have the interface members on it, although in the shell it usually does, so
            // we need to re-order the lookup if we *actually* want the CoClass if it's available.
            for (int i = 0; pTypeInfo is null && i < 2; i++)
            {
                if (wantCoClass == (i == 0))
                {
                    if (obj is IProvideClassInfo pProvideClassInfo)
                    {
                        pProvideClassInfo.GetClassInfo(out pTypeInfo);
                    }
                }
                else
                {
                    if (obj is IDispatch iDispatch)
                    {
                        iDispatch.GetTypeInfo(0, Kernel32.GetThreadLocale(), out pTypeInfo);
                    }
                }
            }

            return pTypeInfo;
        }

        /// <summary>
        ///  Given an Object, this attempts to locate its type info. If it implementes IProvideMultipleClassInfo
        ///  all available type infos will be returned, otherwise the primary one will be alled.
        /// </summary>
        public unsafe static ITypeInfo[] FindTypeInfos(object obj, bool wantCoClass)
        {
            if (obj is IProvideMultipleClassInfo pCI)
            {
                uint n = 0;
                if (pCI.GetMultiTypeInfoCount(&n).Succeeded() && n > 0)
                {
                    var typeInfos = new ITypeInfo[n];
                    for (uint i = 0; i < n; i++)
                    {
                        if (pCI.GetInfoOfIndex(i, MULTICLASSINFO.GETTYPEINFO, out ITypeInfo result, null, null, null, null).Failed())
                        {
                            continue;
                        }

                        Debug.Assert(result != null, "IProvideMultipleClassInfo::GetInfoOfIndex returned S_OK for ITypeInfo index " + i + ", this is a issue in the object that's being browsed, NOT the property browser.");
                        typeInfos[i] = result;
                    }

                    return typeInfos;
                }
            }

            ITypeInfo temp = FindTypeInfo(obj, wantCoClass);
            if (temp != null)
            {
                return new ITypeInfo[] { temp };
            }

            return null;
        }

        /// <summary>
        ///  Retrieve the dispid of the property that we are to use as the name
        ///  member.  In this case, the grid will put parens around the name.
        /// </summary>
        public unsafe static DispatchID GetNameDispId(IDispatch obj)
        {
            DispatchID dispid = DispatchID.UNKNOWN;
            string[] names = null;

            ComNativeDescriptor cnd = ComNativeDescriptor.Instance;
            bool succeeded = false;

            // first try to find one with a valid value
            cnd.GetPropertyValue(obj, "__id", ref succeeded);

            if (succeeded)
            {
                names = new string[] { "__id" };
            }
            else
            {
                cnd.GetPropertyValue(obj, DispatchID.Name, ref succeeded);
                if (succeeded)
                {
                    dispid = DispatchID.Name;
                }
                else
                {
                    cnd.GetPropertyValue(obj, "Name", ref succeeded);
                    if (succeeded)
                    {
                        names = new string[] { "Name" };
                    }
                }
            }

            // now get the dispid of the one that worked...
            if (names != null)
            {
                DispatchID pDispid = DispatchID.UNKNOWN;
                Guid g = Guid.Empty;
                HRESULT hr = obj.GetIDsOfNames(&g, names, 1, Kernel32.GetThreadLocale(), &pDispid);
                if (hr.Succeeded())
                {
                    dispid = pDispid;
                }
            }

            return dispid;
        }

        /// <summary>
        ///  Gets the properties for a given Com2 Object.  The returned Com2Properties
        ///  Object contains the properties and relevant data about them.
        /// </summary>
        public static Com2Properties GetProperties(object obj)
        {
            Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "Com2TypeInfoProcessor.GetProperties");

            if (obj is null || !Marshal.IsComObject(obj))
            {
                Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "Com2TypeInfoProcessor.GetProperties returning null: Object is not a com Object");
                return null;
            }

            ITypeInfo[] typeInfos = FindTypeInfos(obj, false);

            // oops, looks like this guy doesn't surface any type info
            // this is okay, so we just say it has no props
            if (typeInfos is null || typeInfos.Length == 0)
            {
                Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "Com2TypeInfoProcessor.GetProperties :: Didn't get typeinfo");
                return null;
            }

            int defaultProp = -1;
            int temp = -1;
            ArrayList propList = new ArrayList();
            Guid[] typeGuids = new Guid[typeInfos.Length];

            for (int i = 0; i < typeInfos.Length; i++)
            {
                ITypeInfo ti = typeInfos[i];

                if (ti is null)
                {
                    continue;
                }

                uint[] versions = new uint[2];
                Guid typeGuid = GetGuidForTypeInfo(ti, versions);
                PropertyDescriptor[] props = null;
                bool dontProcess = typeGuid != Guid.Empty && processedLibraries != null && processedLibraries.Contains(typeGuid);

                if (dontProcess)
                {
                    CachedProperties cp = (CachedProperties)processedLibraries[typeGuid];

                    if (versions[0] == cp.MajorVersion && versions[1] == cp.MinorVersion)
                    {
                        props = cp.Properties;
                        if (i == 0 && cp.DefaultIndex != -1)
                        {
                            defaultProp = cp.DefaultIndex;
                        }
                    }
                    else
                    {
                        dontProcess = false;
                    }
                }

                if (!dontProcess)
                {
                    props = InternalGetProperties(obj, ti, DispatchID.MEMBERID_NIL, ref temp);

                    // only save the default property from the first type Info
                    if (i == 0 && temp != -1)
                    {
                        defaultProp = temp;
                    }

                    if (processedLibraries is null)
                    {
                        processedLibraries = new Hashtable();
                    }

                    if (typeGuid != Guid.Empty)
                    {
                        processedLibraries[typeGuid] = new CachedProperties(props, i == 0 ? defaultProp : -1, versions[0], versions[1]);
                    }
                }

                if (props != null)
                {
                    propList.AddRange(props);
                }
            }

            Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "Com2TypeInfoProcessor.GetProperties : returning " + propList.Count.ToString(CultureInfo.InvariantCulture) + " properties");

            // done!
            Com2PropertyDescriptor[] temp2 = new Com2PropertyDescriptor[propList.Count];
            propList.CopyTo(temp2, 0);

            return new Com2Properties(obj, temp2, defaultProp);
        }

        private unsafe static Guid GetGuidForTypeInfo(ITypeInfo typeInfo, uint[] versions)
        {
            TYPEATTR* pTypeAttr = null;
            HRESULT hr = typeInfo.GetTypeAttr(&pTypeAttr);
            if (!hr.Succeeded())
            {
                throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
            }

            try
            {
                if (versions != null)
                {
                    versions[0] = pTypeAttr->wMajorVerNum;
                    versions[1] = pTypeAttr->wMinorVerNum;
                }

                return pTypeAttr->guid;
            }
            finally
            {
                typeInfo.ReleaseTypeAttr(pTypeAttr);
            }
        }

        /// <summary>
        ///  Resolves a value type for a property from a TYPEDESC.  Value types can be
        ///  user defined, which and may be aliased into other type infos.  This function
        ///  will recusively walk the ITypeInfos to resolve the type to a clr Type.
        /// </summary>
        private unsafe static Type GetValueTypeFromTypeDesc(in TYPEDESC typeDesc, ITypeInfo typeInfo, object[] typeData)
        {
            uint hreftype;
            HRESULT hr = HRESULT.S_OK;

            switch (typeDesc.vt)
            {
                default:
                    return VTToType(typeDesc.vt);

                case VARENUM.UNKNOWN:
                case VARENUM.DISPATCH:
                    // get the guid
                    typeData[0] = GetGuidForTypeInfo(typeInfo, null);

                    // return the type
                    return VTToType(typeDesc.vt);

                case VARENUM.USERDEFINED:
                    // we'll need to recurse into a user defined reference typeinfo
                    Debug.Assert(typeDesc.union.hreftype != 0u, "typeDesc doesn't contain an hreftype!");
                    hreftype = typeDesc.union.hreftype;
                    break;

                case VARENUM.PTR:
                    // we'll need to recurse into a user defined reference typeinfo
                    Debug.Assert(typeDesc.union.lptdesc != null, "typeDesc doesn't contain an refTypeDesc!");
                    if (typeDesc.union.lptdesc->vt == VARENUM.VARIANT)
                    {
                        return VTToType(typeDesc.union.lptdesc->vt);
                    }

                    hreftype = typeDesc.union.lptdesc->union.hreftype;
                    break;
            }

            // get the reference type info
            hr = typeInfo.GetRefTypeInfo(hreftype, out ITypeInfo refTypeInfo);
            if (!hr.Succeeded())
            {
                throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetRefTypeInfoFailed, hr), (int)hr);
            }

            try
            {
                // here is where we look at the next level type info.
                // if we get an enum, process it, otherwise we will recurse
                // or get a dispatch.
                if (refTypeInfo != null)
                {
                    TYPEATTR* pTypeAttr = null;
                    hr = refTypeInfo.GetTypeAttr(&pTypeAttr);
                    if (!hr.Succeeded())
                    {
                        throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
                    }

                    try
                    {
                        Guid g = pTypeAttr->guid;

                        // save the guid if we've got one here
                        if (!Guid.Empty.Equals(g))
                        {
                            typeData[0] = g;
                        }

                        switch (pTypeAttr->typekind)
                        {
                            case TYPEKIND.ENUM:
                                return ProcessTypeInfoEnum(refTypeInfo);
                            case TYPEKIND.ALIAS:
                                // recurse here
                                return GetValueTypeFromTypeDesc(pTypeAttr->tdescAlias, refTypeInfo, typeData);
                            case TYPEKIND.DISPATCH:
                                return VTToType(VARENUM.DISPATCH);
                            case TYPEKIND.INTERFACE:
                            case TYPEKIND.COCLASS:
                                return VTToType(VARENUM.UNKNOWN);
                            default:
                                return null;
                        }
                    }
                    finally
                    {
                        refTypeInfo.ReleaseTypeAttr(pTypeAttr);
                    }
                }
            }
            finally
            {
                refTypeInfo = null;
            }
            return null;
        }

        private static PropertyDescriptor[] InternalGetProperties(object obj, ITypeInfo typeInfo, DispatchID dispidToGet, ref int defaultIndex)
        {
            if (typeInfo is null)
            {
                return null;
            }

            Hashtable propInfos = new Hashtable();

            DispatchID nameDispID = GetNameDispId((IDispatch)obj);
            bool addAboutBox = false;

            // properties can live as functions with get_ and put_ or
            // as variables, so we do two steps here.
            try
            {
                // DO FUNCDESC things
                ProcessFunctions(typeInfo, propInfos, dispidToGet, nameDispID, ref addAboutBox);
            }
            catch (ExternalException ex)
            {
                Debug.Fail("ProcessFunctions failed with hr=" + ex.ErrorCode.ToString(CultureInfo.InvariantCulture) + ", message=" + ex.ToString());
            }

            try
            {
                // DO VARDESC things.
                ProcessVariables(typeInfo, propInfos, dispidToGet, nameDispID);
            }
            catch (ExternalException ex)
            {
                Debug.Fail("ProcessVariables failed with hr=" + ex.ErrorCode.ToString(CultureInfo.InvariantCulture) + ", message=" + ex.ToString());
            }

            typeInfo = null;

            // now we take the propertyInfo structures we built up
            // and use them to create the actual descriptors.
            int cProps = propInfos.Count;

            if (addAboutBox)
            {
                cProps++;
            }

            PropertyDescriptor[] props = new PropertyDescriptor[cProps];
            int defaultProp = -1;

            HRESULT hr = HRESULT.S_OK;
            object[] pvar = new object[1];
            ComNativeDescriptor cnd = ComNativeDescriptor.Instance;

            // for each item in uur list, create the descriptor an check
            // if it's the default one.
            foreach (PropInfo pi in propInfos.Values)
            {
                if (!pi.NonBrowsable)
                {
                    // finally, for each property, make sure we can get the value
                    // if we can't then we should mark it non-browsable

                    try
                    {
                        hr = cnd.GetPropertyValue(obj, pi.DispId, pvar);
                    }
                    catch (ExternalException ex)
                    {
                        hr = (HRESULT)ex.ErrorCode;
                        Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "IDispatch::Invoke(PROPGET, " + pi.Name + ") threw an exception :" + ex.ToString());
                    }
                    if (!hr.Succeeded())
                    {
                        Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "Adding Browsable(false) to property '" + pi.Name + "' because Invoke(dispid=0x{0:X} ,DISPATCH_PROPERTYGET) returned hr=0x{1:X}.  Properties that do not return S_OK are hidden by default.", pi.DispId, hr));
                        pi.Attributes.Add(new BrowsableAttribute(false));
                        pi.NonBrowsable = true;
                    }
                }
                else
                {
                    hr = HRESULT.S_OK;
                }

                Attribute[] temp = new Attribute[pi.Attributes.Count];
                pi.Attributes.CopyTo(temp, 0);
                props[pi.Index] = new Com2PropertyDescriptor(pi.DispId, pi.Name, temp, pi.ReadOnly != PropInfo.ReadOnlyFalse, pi.ValueType, pi.TypeData, !hr.Succeeded());
                if (pi.IsDefault)
                {
                    defaultProp = pi.Index;
                }
            }

            if (addAboutBox)
            {
                props[props.Length - 1] = new Com2AboutBoxPropertyDescriptor();
            }
            return props;
        }

        private unsafe static PropInfo ProcessDataCore(ITypeInfo typeInfo, IDictionary propInfoList, DispatchID dispid, DispatchID nameDispID, in TYPEDESC typeDesc, VARFLAGS flags)
        {
            // get the name and the helpstring
            using var nameBstr = new BSTR();
            using var helpStringBstr = new BSTR();
            HRESULT hr = typeInfo.GetDocumentation(dispid, &nameBstr, &helpStringBstr, null, null);
            ComNativeDescriptor cnd = ComNativeDescriptor.Instance;
            if (!hr.Succeeded())
            {
                throw new COMException(string.Format(SR.TYPEINFOPROCESSORGetDocumentationFailed, dispid, hr, cnd.GetClassName(typeInfo)), (int)hr);
            }

            string name = nameBstr.String.ToString();
            if (string.IsNullOrEmpty(name))
            {
                Debug.Fail(string.Format(CultureInfo.CurrentCulture, "ITypeInfo::GetDocumentation didn't return a name for DISPID 0x{0:X} but returned SUCEEDED(hr),  Component=" + cnd.GetClassName(typeInfo), dispid));
                return null;
            }

            // now we can create our struct... make sure we don't already have one
            PropInfo pi = (PropInfo)propInfoList[name];

            if (pi is null)
            {
                pi = new PropInfo
                {
                    Index = propInfoList.Count
                };
                propInfoList[name] = pi;
                pi.Name = name;
                pi.DispId = dispid;
                pi.Attributes.Add(new DispIdAttribute((int)pi.DispId));
            }

            string helpString = helpStringBstr.String.ToString();
            if (!string.IsNullOrEmpty(helpString))
            {
                pi.Attributes.Add(new DescriptionAttribute(helpString));
            }

            // figure out the value type
            if (pi.ValueType is null)
            {
                object[] pTypeData = new object[1];
                try
                {
                    pi.ValueType = GetValueTypeFromTypeDesc(in typeDesc, typeInfo, pTypeData);
                }
                catch (Exception ex)
                {
                    Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "Hiding property " + pi.Name + " because value Type could not be resolved: " + ex.ToString());
                }

                // if we can't resolve the type, mark the property as nonbrowsable
                // from the browser
                //
                if (pi.ValueType is null)
                {
                    pi.NonBrowsable = true;
                }

                if (pi.NonBrowsable)
                {
                    flags |= VARFLAGS.FNONBROWSABLE;
                }

                if (pTypeData[0] != null)
                {
                    pi.TypeData = pTypeData[0];
                }
            }

            // check the flags
            if ((flags & VARFLAGS.FREADONLY) != 0)
            {
                pi.ReadOnly = PropInfo.ReadOnlyTrue;
            }

            if ((flags & VARFLAGS.FHIDDEN) != 0 ||
                (flags & VARFLAGS.FNONBROWSABLE) != 0 ||
                pi.Name[0] == '_' ||
                dispid == DispatchID.HWND)
            {
                pi.Attributes.Add(new BrowsableAttribute(false));
                pi.NonBrowsable = true;
            }

            if ((flags & VARFLAGS.FUIDEFAULT) != 0)
            {
                pi.IsDefault = true;
            }

            if ((flags & VARFLAGS.FBINDABLE) != 0 &&
                (flags & VARFLAGS.FDISPLAYBIND) != 0)
            {
                pi.Attributes.Add(new BindableAttribute(true));
            }

            // lastly, if it's DISPID_Name, add the ParenthesizeNameAttribute
            if (dispid == nameDispID)
            {
                pi.Attributes.Add(new ParenthesizePropertyNameAttribute(true));

                // don't allow merges on the name
                pi.Attributes.Add(new MergablePropertyAttribute(false));
            }

            return pi;
        }

        private unsafe static void ProcessFunctions(ITypeInfo typeInfo, IDictionary propInfoList, DispatchID dispidToGet, DispatchID nameDispID, ref bool addAboutBox)
        {
            TYPEATTR* pTypeAttr = null;
            HRESULT hr = typeInfo.GetTypeAttr(&pTypeAttr);
            if (!hr.Succeeded() || pTypeAttr is null)
            {
                throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
            }

            try
            {
                bool isPropGet;
                PropInfo pi;

                for (uint i = 0; i < pTypeAttr->cFuncs; i++)
                {
                    FUNCDESC* pFuncDesc = null;
                    hr = typeInfo.GetFuncDesc(i, &pFuncDesc);
                    if (!hr.Succeeded() || pFuncDesc is null)
                    {
                        Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "ProcessTypeInfoEnum: ignoring function item 0x{0:X} because ITypeInfo::GetFuncDesc returned hr=0x{1:X} or NULL", i, hr));
                        continue;
                    }

                    try
                    {
                        if (pFuncDesc->invkind == INVOKEKIND.FUNC ||
                            (dispidToGet != DispatchID.MEMBERID_NIL && pFuncDesc->memid != dispidToGet))
                        {
                            if (pFuncDesc->memid == DispatchID.ABOUTBOX)
                            {
                                addAboutBox = true;
                            }
                            continue;
                        }

                        TYPEDESC typeDesc;

                        // is this a get or a put?
                        isPropGet = (pFuncDesc->invkind == INVOKEKIND.PROPERTYGET);

                        if (isPropGet)
                        {
                            if (pFuncDesc->cParams != 0)
                            {
                                continue;
                            }

                            unsafe
                            {
                                typeDesc = pFuncDesc->elemdescFunc.tdesc;
                            }
                        }
                        else
                        {
                            Debug.Assert(pFuncDesc->lprgelemdescParam != null, "ELEMDESC param is null!");
                            if (pFuncDesc->lprgelemdescParam is null || pFuncDesc->cParams != 1)
                            {
                                continue;
                            }

                            unsafe
                            {
                                typeDesc = pFuncDesc->lprgelemdescParam->tdesc;
                            }
                        }
                        pi = ProcessDataCore(typeInfo, propInfoList, pFuncDesc->memid, nameDispID, in typeDesc, (VARFLAGS)pFuncDesc->wFuncFlags);

                        // if we got a setmethod, it's not readonly
                        if (pi != null && !isPropGet)
                        {
                            pi.ReadOnly = PropInfo.ReadOnlyFalse;
                        }
                    }
                    finally
                    {
                        typeInfo.ReleaseFuncDesc(pFuncDesc);
                    }
                }
            }
            finally
            {
                typeInfo.ReleaseTypeAttr(pTypeAttr);
            }
        }

        /// <summary>
        ///  This converts a type info that describes a IDL defined enum
        ///  into one we can use
        /// </summary>
        private unsafe static Type ProcessTypeInfoEnum(ITypeInfo enumTypeInfo)
        {
            Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "ProcessTypeInfoEnum entered");

            if (enumTypeInfo is null)
            {
                Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "ProcessTypeInfoEnum got a NULL enumTypeInfo");
                return null;
            }

            try
            {
                TYPEATTR* pTypeAttr = null;
                HRESULT hr = enumTypeInfo.GetTypeAttr(&pTypeAttr);
                if (!hr.Succeeded() || pTypeAttr is null)
                {
                    throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
                }

                try
                {
                    uint nItems = pTypeAttr->cVars;

                    Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "ProcessTypeInfoEnum: processing " + nItems.ToString(CultureInfo.InvariantCulture) + " variables");

                    ArrayList strs = new ArrayList();
                    ArrayList vars = new ArrayList();

                    object varValue = null;

                    using var enumNameBstr = new BSTR();
                    using var enumHelpStringBstr = new BSTR();
                    enumTypeInfo.GetDocumentation(DispatchID.MEMBERID_NIL, &enumNameBstr, &enumHelpStringBstr, null, null);

                    // For each item in the enum type info, we just need it's name and value and
                    // helpstring if it's there.
                    for (uint i = 0; i < nItems; i++)
                    {
                        VARDESC* pVarDesc = null;
                        hr = enumTypeInfo.GetVarDesc(i, &pVarDesc);
                        if (!hr.Succeeded() || pVarDesc is null)
                        {
                            Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "ProcessTypeInfoEnum: ignoring item 0x{0:X} because ITypeInfo::GetVarDesc returned hr=0x{1:X} or NULL", i, hr));
                            continue;
                        }

                        try
                        {
                            if (pVarDesc->varkind != VARKIND.CONST || pVarDesc->unionMember == IntPtr.Zero)
                            {
                                continue;
                            }

                            varValue = null;

                            // get the name and the helpstring
                            using var nameBstr = new BSTR();
                            using var helpBstr = new BSTR();
                            hr = enumTypeInfo.GetDocumentation(pVarDesc->memid, &nameBstr, &helpBstr, null, null);
                            if (!hr.Succeeded())
                            {
                                Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "ProcessTypeInfoEnum: ignoring item 0x{0:X} because ITypeInfo::GetDocumentation returned hr=0x{1:X} or NULL", i, hr));
                                continue;
                            }

                            string name = nameBstr.String.ToString();
                            string helpString = helpBstr.String.ToString();
                            Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "ProcessTypeInfoEnum got name=" + (name ?? "(null)") + ", helpstring=" + (helpString ?? "(null)"));

                            // get the value
                            try
                            {
                                varValue = Marshal.GetObjectForNativeVariant(pVarDesc->unionMember);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "ProcessTypeInfoEnum: PtrtoStructFailed " + ex.GetType().Name + "," + ex.Message);
                            }

                            Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "ProcessTypeInfoEnum: adding variable value=" + Convert.ToString(varValue, CultureInfo.InvariantCulture));
                            vars.Add(varValue);

                            // if we have a helpstring, use it, otherwise use name
                            string nameString;
                            if (!string.IsNullOrEmpty(helpString))
                            {
                                nameString = helpString;
                            }
                            else
                            {
                                Debug.Assert(name != null, "No name for VARDESC member, but GetDocumentation returned S_OK!");
                                nameString = name;
                            }
                            Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "ProcessTypeInfoEnum: adding name value=" + nameString);
                            strs.Add(nameString);
                        }
                        finally
                        {
                            enumTypeInfo.ReleaseVarDesc(pVarDesc);
                        }
                    }

                    Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "ProcessTypeInfoEnum: returning enum with " + strs.Count.ToString(CultureInfo.InvariantCulture) + " items");

                    // just build our enumerator
                    if (strs.Count > 0)
                    {
                        // get the IUnknown value of the ITypeInfo
                        IntPtr pTypeInfoUnk = Marshal.GetIUnknownForObject(enumTypeInfo);

                        try
                        {
                            string enumName = pTypeInfoUnk.ToString() + "_" + enumNameBstr.String.ToString();

                            if (builtEnums is null)
                            {
                                builtEnums = new Hashtable();
                            }
                            else if (builtEnums.ContainsKey(enumName))
                            {
                                return (Type)builtEnums[enumName];
                            }

                            Type enumType = typeof(int);

                            if (vars.Count > 0 && vars[0] != null)
                            {
                                enumType = vars[0].GetType();
                            }

                            EnumBuilder enumBuilder = ModuleBuilder.DefineEnum(enumName, TypeAttributes.Public, enumType);
                            for (int i = 0; i < strs.Count; i++)
                            {
                                enumBuilder.DefineLiteral((string)strs[i], vars[i]);
                            }
                            Type t = enumBuilder.CreateTypeInfo().AsType();
                            builtEnums[enumName] = t;
                            return t;
                        }
                        finally
                        {
                            if (pTypeInfoUnk != IntPtr.Zero)
                            {
                                Marshal.Release(pTypeInfoUnk);
                            }
                        }
                    }
                }
                finally
                {
                    enumTypeInfo.ReleaseTypeAttr(pTypeAttr);
                }
            }
            catch
            {
            }
            return null;
        }

        private unsafe static void ProcessVariables(ITypeInfo typeInfo, IDictionary propInfoList, DispatchID dispidToGet, DispatchID nameDispID)
        {
            TYPEATTR* pTypeAttr = null;
            HRESULT hr = typeInfo.GetTypeAttr(&pTypeAttr);
            if (!hr.Succeeded() || pTypeAttr is null)
            {
                throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
            }

            try
            {
                for (uint i = 0; i < pTypeAttr->cVars; i++)
                {
                    VARDESC* pVarDesc = null;
                    hr = typeInfo.GetVarDesc(i, &pVarDesc);
                    if (!hr.Succeeded() || pVarDesc is null)
                    {
                        Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "ProcessTypeInfoEnum: ignoring variable item 0x{0:X} because ITypeInfo::GetFuncDesc returned hr=0x{1:X} or NULL", i, hr));
                        continue;
                    }

                    try
                    {
                        if (pVarDesc->varkind == VARKIND.CONST ||
                            (dispidToGet != DispatchID.MEMBERID_NIL && pVarDesc->memid != dispidToGet))
                        {
                            continue;
                        }

                        unsafe
                        {
                            PropInfo pi = ProcessDataCore(typeInfo, propInfoList, pVarDesc->memid, nameDispID, in pVarDesc->elemdescVar.tdesc, pVarDesc->wVarFlags);
                            if (pi.ReadOnly != PropInfo.ReadOnlyTrue)
                            {
                                pi.ReadOnly = PropInfo.ReadOnlyFalse;
                            }
                        }
                    }
                    finally
                    {
                        typeInfo.ReleaseVarDesc(pVarDesc);
                    }
                }
            }
            finally
            {
                typeInfo.ReleaseTypeAttr(pTypeAttr);
            }
        }

        private static Type VTToType(VARENUM vt)
        {
            switch (vt)
            {
                case VARENUM.EMPTY:
                case VARENUM.NULL:
                    return null;
                case VARENUM.I1:
                    return typeof(sbyte);
                case VARENUM.UI1:
                    return typeof(byte);
                case VARENUM.I2:
                    return typeof(short);
                case VARENUM.UI2:
                    return typeof(ushort);
                case VARENUM.I4:
                case VARENUM.INT:
                    return typeof(int);
                case VARENUM.UI4:
                case VARENUM.UINT:
                    return typeof(uint);
                case VARENUM.I8:
                    return typeof(long);
                case VARENUM.UI8:
                    return typeof(ulong);
                case VARENUM.R4:
                    return typeof(float);
                case VARENUM.R8:
                    return typeof(double);
                case VARENUM.CY:
                    return typeof(decimal);
                case VARENUM.DATE:
                    return typeof(DateTime);
                case VARENUM.BSTR:
                case VARENUM.LPSTR:
                case VARENUM.LPWSTR:
                    return typeof(string);
                case VARENUM.DISPATCH:
                    return typeof(IDispatch);
                case VARENUM.UNKNOWN:
                    return typeof(object);
                case VARENUM.ERROR:
                case VARENUM.HRESULT:
                    return typeof(int);
                case VARENUM.BOOL:
                    return typeof(bool);
                case VARENUM.VARIANT:
                    return typeof(Com2Variant);
                case VARENUM.CLSID:
                    return typeof(Guid);
                case VARENUM.FILETIME:
                    return typeof(Runtime.InteropServices.ComTypes.FILETIME);
                case VARENUM.USERDEFINED:
                    throw new ArgumentException(string.Format(SR.COM2UnhandledVT, "VT_USERDEFINED"));
                case VARENUM.VOID:
                case VARENUM.PTR:
                case VARENUM.SAFEARRAY:
                case VARENUM.CARRAY:
                case VARENUM.RECORD:
                case VARENUM.BLOB:
                case VARENUM.STREAM:
                case VARENUM.STORAGE:
                case VARENUM.STREAMED_OBJECT:
                case VARENUM.STORED_OBJECT:
                case VARENUM.BLOB_OBJECT:
                case VARENUM.CF:
                case VARENUM.BSTR_BLOB:
                case VARENUM.VECTOR:
                case VARENUM.ARRAY:
                case VARENUM.BYREF:
                case VARENUM.RESERVED:
                default:
                    throw new ArgumentException(string.Format(SR.COM2UnhandledVT, ((int)vt).ToString(CultureInfo.InvariantCulture)));
            }
        }

        internal class CachedProperties
        {
            private readonly PropertyDescriptor[] props;

            public readonly uint MajorVersion;
            public readonly uint MinorVersion;
            private readonly int defaultIndex;

            internal CachedProperties(PropertyDescriptor[] props, int defIndex, uint majVersion, uint minVersion)
            {
                this.props = ClonePropertyDescriptors(props);
                MajorVersion = majVersion;
                MinorVersion = minVersion;
                defaultIndex = defIndex;
            }

            public PropertyDescriptor[] Properties
            {
                get
                {
                    return ClonePropertyDescriptors(props);
                }
            }

            public int DefaultIndex
            {
                get
                {
                    return defaultIndex;
                }
            }

            private PropertyDescriptor[] ClonePropertyDescriptors(PropertyDescriptor[] props)
            {
                PropertyDescriptor[] retProps = new PropertyDescriptor[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    if (props[i] is ICloneable)
                    {
                        retProps[i] = (PropertyDescriptor)((ICloneable)props[i]).Clone();
                        ;
                    }
                    else
                    {
                        retProps[i] = props[i];
                    }
                }
                return retProps;
            }
        }

        private class PropInfo
        {
            public const int ReadOnlyUnknown = 0;
            public const int ReadOnlyTrue = 1;
            public const int ReadOnlyFalse = 2;

            public string Name { get; set; }

            public DispatchID DispId { get; set; } = DispatchID.UNKNOWN;

            public Type ValueType { get; set; }

            public ArrayList Attributes { get; } = new ArrayList();

            public int ReadOnly { get; set; } = ReadOnlyUnknown;

            public bool IsDefault { get; set; }

            public object TypeData { get; set; }

            public bool NonBrowsable { get; set; }

            public int Index { get; set; }

            public override int GetHashCode() => Name?.GetHashCode() ?? base.GetHashCode();
        }
    }

    /// <summary>
    ///  A class included so we can recognize a variant properly.
    /// </summary>
    public class Com2Variant
    {
    }
}

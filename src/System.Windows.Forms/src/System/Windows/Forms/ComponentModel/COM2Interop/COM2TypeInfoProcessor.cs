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
using Windows.Win32.System.Com;
using static Windows.Win32.System.Com.TYPEKIND;
using static Windows.Win32.System.Com.VARENUM;
using static Windows.Win32.System.Com.VARFLAGS;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This is the main worker class of Com2 property interop. It takes an IDispatch Object
    ///  and translates it's Oleaut32.ITypeInfo into Com2PropertyDescriptor objects that are understandable
    ///  by managed code.
    ///
    ///  This class only knows how to process things that are natively in the typeinfo.  Other property
    ///  information such as IPerPropertyBrowsing is handled elsewhere.
    /// </summary>
    internal static partial class Com2TypeInfoProcessor
    {
        private static readonly TraceSwitch DbgTypeInfoProcessorSwitch = new(
            "DbgTypeInfoProcessor",
             "Com2TypeInfoProcessor: debug Com2 type info processing");

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
        ///  Given an Object, this attempts to locate its type info.
        /// </summary>
        public static Oleaut32.ITypeInfo FindTypeInfo(object obj, bool wantCoClass)
        {
            Oleaut32.ITypeInfo pTypeInfo = null;

            // This is kind of odd.  What's going on here is that if we want the CoClass (e.g. for
            // the interface name), we need to look for IProvideClassInfo first, then look for the
            // typeinfo from the IDispatch. In the case of many Oleaut32 operations, the CoClass
            // doesn't have the interface members on it, although in the shell it usually does, so
            // we need to re-order the lookup if we *actually* want the CoClass if it's available.
            for (int i = 0; pTypeInfo is null && i < 2; i++)
            {
                if (wantCoClass == (i == 0))
                {
                    if (obj is Oleaut32.IProvideClassInfo pProvideClassInfo)
                    {
                        pProvideClassInfo.GetClassInfo(out pTypeInfo);
                    }
                }
                else
                {
                    if (obj is Oleaut32.IDispatch iDispatch)
                    {
                        iDispatch.GetTypeInfo(0, PInvoke.GetThreadLocale(), out pTypeInfo);
                    }
                }
            }

            return pTypeInfo;
        }

        /// <summary>
        ///  Given an Object, this attempts to locate its type info. If it implements IProvideMultipleClassInfo
        ///  all available type infos will be returned, otherwise the primary one will be called.
        /// </summary>
        public static unsafe Oleaut32.ITypeInfo[] FindTypeInfos(object obj, bool wantCoClass)
        {
            if (obj is Oleaut32.IProvideMultipleClassInfo pCI)
            {
                uint n = 0;
                if (pCI.GetMultiTypeInfoCount(&n).Succeeded && n > 0)
                {
                    var typeInfos = new Oleaut32.ITypeInfo[n];
                    for (uint i = 0; i < n; i++)
                    {
                        if (pCI.GetInfoOfIndex(i, Oleaut32.MULTICLASSINFO.GETTYPEINFO, out Oleaut32.ITypeInfo result, null, null, null, null).Failed)
                        {
                            continue;
                        }

                        Debug.Assert(
                            result is not null,
                            $"IProvideMultipleClassInfo::GetInfoOfIndex returned S_OK for Oleaut32.ITypeInfo index {i}, this is a issue in the object that's being browsed, NOT the property browser.");
                        typeInfos[i] = result;
                    }

                    return typeInfos;
                }
            }

            Oleaut32.ITypeInfo temp = FindTypeInfo(obj, wantCoClass);
            return temp is not null ? (new Oleaut32.ITypeInfo[] { temp }) : null;
        }

        /// <summary>
        ///  Retrieve the dispid of the property that we are to use as the name
        ///  member.  In this case, the grid will put parens around the name.
        /// </summary>
        public static unsafe Ole32.DispatchID GetNameDispId(Oleaut32.IDispatch obj)
        {
            Ole32.DispatchID dispid = Ole32.DispatchID.UNKNOWN;
            string[] names = null;

            ComNativeDescriptor cnd = ComNativeDescriptor.Instance;
            bool succeeded = false;

            // first try to find one with a valid value
            ComNativeDescriptor.GetPropertyValue(obj, "__id", ref succeeded);

            if (succeeded)
            {
                names = new string[] { "__id" };
            }
            else
            {
                ComNativeDescriptor.GetPropertyValue(obj, Ole32.DispatchID.Name, ref succeeded);
                if (succeeded)
                {
                    dispid = Ole32.DispatchID.Name;
                }
                else
                {
                    ComNativeDescriptor.GetPropertyValue(obj, "Name", ref succeeded);
                    if (succeeded)
                    {
                        names = new string[] { "Name" };
                    }
                }
            }

            // now get the dispid of the one that worked...
            if (names is not null)
            {
                Ole32.DispatchID pDispid = Ole32.DispatchID.UNKNOWN;
                Guid g = Guid.Empty;
                HRESULT hr = obj.GetIDsOfNames(&g, names, 1, PInvoke.GetThreadLocale(), &pDispid);
                if (hr.Succeeded)
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

            Oleaut32.ITypeInfo[] typeInfos = FindTypeInfos(obj, false);

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
                Oleaut32.ITypeInfo ti = typeInfos[i];

                if (ti is null)
                {
                    continue;
                }

                uint[] versions = new uint[2];
                Guid typeGuid = GetGuidForTypeInfo(ti, versions);
                PropertyDescriptor[] props = null;
                bool dontProcess = typeGuid != Guid.Empty && processedLibraries is not null && processedLibraries.Contains(typeGuid);

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
                    props = InternalGetProperties(obj, ti, Ole32.DispatchID.MEMBERID_NIL, ref temp);

                    // only save the default property from the first type Info
                    if (i == 0 && temp != -1)
                    {
                        defaultProp = temp;
                    }

                    processedLibraries ??= new Hashtable();

                    if (typeGuid != Guid.Empty)
                    {
                        processedLibraries[typeGuid] = new CachedProperties(props, i == 0 ? defaultProp : -1, versions[0], versions[1]);
                    }
                }

                if (props is not null)
                {
                    propList.AddRange(props);
                }
            }

            Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, $"Com2TypeInfoProcessor.GetProperties : returning {propList.Count} properties");

            // Done!
            Com2PropertyDescriptor[] temp2 = new Com2PropertyDescriptor[propList.Count];
            propList.CopyTo(temp2, 0);

            return new Com2Properties(obj, temp2, defaultProp);
        }

        private static unsafe Guid GetGuidForTypeInfo(Oleaut32.ITypeInfo typeInfo, uint[] versions)
        {
            TYPEATTR* pTypeAttr = null;
            HRESULT hr = typeInfo.GetTypeAttr(&pTypeAttr);
            if (!hr.Succeeded)
            {
                throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
            }

            try
            {
                if (versions is not null)
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
        ///  will recursively walk the ITypeInfos to resolve the type to a clr Type.
        /// </summary>
        private static unsafe Type GetValueTypeFromTypeDesc(in TYPEDESC typeDesc, Oleaut32.ITypeInfo typeInfo, object[] typeData)
        {
            uint hreftype;
            HRESULT hr = HRESULT.S_OK;

            switch (typeDesc.vt)
            {
                default:
                    return VTToType(typeDesc.vt);

                case VT_UNKNOWN:
                case VT_DISPATCH:
                    // Get the guid.
                    typeData[0] = GetGuidForTypeInfo(typeInfo, null);

                    // Return the type.
                    return VTToType(typeDesc.vt);

                case VT_USERDEFINED:
                    // We'll need to recurse into a user defined reference typeinfo.
                    Debug.Assert(typeDesc.Anonymous.hreftype != 0u, "typeDesc doesn't contain an hreftype!");
                    hreftype = typeDesc.Anonymous.hreftype;
                    break;

                case VT_PTR:
                    // We'll need to recurse into a user defined reference typeinfo.
                    Debug.Assert(typeDesc.Anonymous.lptdesc is not null, "typeDesc doesn't contain an refTypeDesc!");
                    if (typeDesc.Anonymous.lptdesc->vt == VT_VARIANT)
                    {
                        return VTToType(typeDesc.Anonymous.lptdesc->vt);
                    }

                    hreftype = typeDesc.Anonymous.lptdesc->Anonymous.hreftype;
                    break;
            }

            // Get the reference type info.
            hr = typeInfo.GetRefTypeInfo(hreftype, out Oleaut32.ITypeInfo refTypeInfo);
            if (!hr.Succeeded)
            {
                throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetRefTypeInfoFailed, hr), (int)hr);
            }

            try
            {
                // Here is where we look at the next level type info. If we get an enum, process it, otherwise we will
                // recurse or get a dispatch.
                if (refTypeInfo is null)
                {
                    return null;
                }

                TYPEATTR* pTypeAttr = null;
                hr = refTypeInfo.GetTypeAttr(&pTypeAttr);
                if (!hr.Succeeded)
                {
                    throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
                }

                try
                {
                    Guid g = pTypeAttr->guid;

                    // Save the guid if we've got one here.
                    if (!Guid.Empty.Equals(g))
                    {
                        typeData[0] = g;
                    }

                    switch (pTypeAttr->typekind)
                    {
                        case TKIND_ENUM:
                            return ProcessTypeInfoEnum(refTypeInfo);
                        case TKIND_ALIAS:
                            // Recurse here.
                            return GetValueTypeFromTypeDesc(pTypeAttr->tdescAlias, refTypeInfo, typeData);
                        case TKIND_DISPATCH:
                            return VTToType(VT_DISPATCH);
                        case TKIND_INTERFACE:
                        case TKIND_COCLASS:
                            return VTToType(VT_UNKNOWN);
                        default:
                            return null;
                    }
                }
                finally
                {
                    refTypeInfo.ReleaseTypeAttr(pTypeAttr);
                }
            }
            finally
            {
                refTypeInfo = null;
            }
        }

        private static PropertyDescriptor[] InternalGetProperties(
            object obj,
            Oleaut32.ITypeInfo typeInfo,
            Ole32.DispatchID dispidToGet,
            ref int defaultIndex)
        {
            if (typeInfo is null)
            {
                return null;
            }

            Hashtable propInfos = new Hashtable();

            Ole32.DispatchID nameDispID = GetNameDispId((Oleaut32.IDispatch)obj);
            bool addAboutBox = false;

            // Properties can live as functions with get_ and put_ or as variables, so we do two steps here.
            try
            {
                // Do FUNCDESC things.
                ProcessFunctions(typeInfo, propInfos, dispidToGet, nameDispID, ref addAboutBox);
            }
            catch (ExternalException ex)
            {
                Debug.Fail($"ProcessFunctions failed with hr={ex.ErrorCode}, message={ex}");
            }

            try
            {
                // Do VARDESC things.
                ProcessVariables(typeInfo, propInfos, dispidToGet, nameDispID);
            }
            catch (ExternalException ex)
            {
                Debug.Fail($"ProcessVariables failed with hr={ex.ErrorCode}, message={ex}");
            }

            typeInfo = null;

            // Now we take the propertyInfo structures we built up and use them to create the actual descriptors.
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

            // For each item in our list, create the descriptor an check if it's the default one.
            foreach (PropInfo pi in propInfos.Values)
            {
                if (!pi.NonBrowsable)
                {
                    // Finally, for each property, make sure we can get the value
                    // if we can't then we should mark it non-browsable.

                    try
                    {
                        hr = ComNativeDescriptor.GetPropertyValue(obj, pi.DispId, pvar);
                    }
                    catch (ExternalException ex)
                    {
                        hr = (HRESULT)ex.ErrorCode;
                        Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, $"IDispatch::Invoke(PROPGET, {pi.Name}) threw an exception :{ex}");
                    }

                    if (!hr.Succeeded)
                    {
                        Debug.WriteLineIf(
                            DbgTypeInfoProcessorSwitch.TraceVerbose,
                            $"Adding Browsable(false) to property '{pi.Name}' because Invoke(dispid=0x{pi.DispId:X} ,DISPATCH_PROPERTYGET) returned hr=0x{hr:X}. Properties that do not return S_OK are hidden by default.");
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
                props[pi.Index] = new Com2PropertyDescriptor(pi.DispId, pi.Name, temp, pi.ReadOnly != PropInfo.ReadOnlyFalse, pi.ValueType, pi.TypeData, !hr.Succeeded);
                if (pi.IsDefault)
                {
                    defaultProp = pi.Index;
                }
            }

            if (addAboutBox)
            {
                props[^1] = new Com2AboutBoxPropertyDescriptor();
            }

            return props;
        }

        private static unsafe PropInfo ProcessDataCore(
            Oleaut32.ITypeInfo typeInfo,
            IDictionary propInfoList,
            Ole32.DispatchID dispid,
            Ole32.DispatchID nameDispID,
            in TYPEDESC typeDesc,
            VARFLAGS flags)
        {
            // Get the name and the helpstring.
            using var nameBstr = new BSTR();
            using var helpStringBstr = new BSTR();
            HRESULT hr = typeInfo.GetDocumentation(dispid, &nameBstr, &helpStringBstr, null, null);
            ComNativeDescriptor cnd = ComNativeDescriptor.Instance;
            if (!hr.Succeeded)
            {
                throw new COMException(string.Format(SR.TYPEINFOPROCESSORGetDocumentationFailed, dispid, hr, ComNativeDescriptor.GetClassName(typeInfo)), (int)hr);
            }

            string name = nameBstr.AsSpan().ToString();
            if (string.IsNullOrEmpty(name))
            {
                Debug.Fail($"ITypeInfo::GetDocumentation didn't return a name for DISPID 0x{dispid:X} but returned SUCCEEDED(hr),  Component={ComNativeDescriptor.GetClassName(typeInfo)}");
                return null;
            }

            // Now we can create our struct. Make sure we don't already have one.
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

            var helpString = helpStringBstr.AsSpan();
            if (!helpString.IsEmpty)
            {
                pi.Attributes.Add(new DescriptionAttribute(helpString.ToString()));
            }

            // Figure out the value type.
            if (pi.ValueType is null)
            {
                object[] pTypeData = new object[1];
                try
                {
                    pi.ValueType = GetValueTypeFromTypeDesc(in typeDesc, typeInfo, pTypeData);
                }
                catch (Exception ex)
                {
                    Debug.WriteLineIf(
                        DbgTypeInfoProcessorSwitch.TraceVerbose,
                         $"Hiding property {pi.Name} because value Type could not be resolved: {ex}");
                }

                // If we can't resolve the type, mark the property as nonbrowsable from the browser.
                if (pi.ValueType is null)
                {
                    pi.NonBrowsable = true;
                }

                if (pi.NonBrowsable)
                {
                    flags |= VARFLAG_FNONBROWSABLE;
                }

                if (pTypeData[0] is not null)
                {
                    pi.TypeData = pTypeData[0];
                }
            }

            // Check the flags.
            if ((flags & VARFLAG_FREADONLY) != 0)
            {
                pi.ReadOnly = PropInfo.ReadOnlyTrue;
            }

            if ((flags & VARFLAG_FHIDDEN) != 0 ||
                (flags & VARFLAG_FNONBROWSABLE) != 0 ||
                pi.Name[0] == '_' ||
                dispid == Ole32.DispatchID.HWND)
            {
                pi.Attributes.Add(new BrowsableAttribute(false));
                pi.NonBrowsable = true;
            }

            if ((flags & VARFLAG_FUIDEFAULT) != 0)
            {
                pi.IsDefault = true;
            }

            if ((flags & VARFLAG_FBINDABLE) != 0 &&
                (flags & VARFLAG_FDISPLAYBIND) != 0)
            {
                pi.Attributes.Add(new BindableAttribute(true));
            }

            // Lastly, if it's DISPID_Name, add the ParenthesizeNameAttribute.
            if (dispid == nameDispID)
            {
                pi.Attributes.Add(new ParenthesizePropertyNameAttribute(true));

                // Don't allow merges on the name.
                pi.Attributes.Add(new MergablePropertyAttribute(false));
            }

            return pi;
        }

        private static unsafe void ProcessFunctions(Oleaut32.ITypeInfo typeInfo, IDictionary propInfoList, Ole32.DispatchID dispidToGet, Ole32.DispatchID nameDispID, ref bool addAboutBox)
        {
            TYPEATTR* pTypeAttr = null;
            HRESULT hr = typeInfo.GetTypeAttr(&pTypeAttr);
            if (!hr.Succeeded || pTypeAttr is null)
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
                    if (!hr.Succeeded || pFuncDesc is null)
                    {
                        Debug.WriteLineIf(
                            DbgTypeInfoProcessorSwitch.TraceVerbose,
                            $"ProcessTypeInfoEnum: ignoring function item 0x{i:X} because Oleaut32.ITypeInfo::GetFuncDesc returned hr=0x{hr:X} or NULL");
                        continue;
                    }

                    try
                    {
                        if (pFuncDesc->invkind == INVOKEKIND.INVOKE_FUNC ||
                            (dispidToGet != Ole32.DispatchID.MEMBERID_NIL && pFuncDesc->memid != (int)dispidToGet))
                        {
                            if (pFuncDesc->memid == (int)Ole32.DispatchID.ABOUTBOX)
                            {
                                addAboutBox = true;
                            }

                            continue;
                        }

                        TYPEDESC typeDesc;

                        // is this a get or a put?
                        isPropGet = (pFuncDesc->invkind == INVOKEKIND.INVOKE_PROPERTYGET);

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
                            Debug.Assert(pFuncDesc->lprgelemdescParam is not null, "ELEMDESC param is null!");
                            if (pFuncDesc->lprgelemdescParam is null || pFuncDesc->cParams != 1)
                            {
                                continue;
                            }

                            unsafe
                            {
                                typeDesc = pFuncDesc->lprgelemdescParam->tdesc;
                            }
                        }

                        pi = ProcessDataCore(typeInfo, propInfoList, (Ole32.DispatchID)pFuncDesc->memid, nameDispID, in typeDesc, (VARFLAGS)pFuncDesc->wFuncFlags);

                        // if we got a setmethod, it's not readonly
                        if (pi is not null && !isPropGet)
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
        ///  This converts a type info that describes a IDL defined enum into one we can use
        /// </summary>
        private static unsafe Type ProcessTypeInfoEnum(Oleaut32.ITypeInfo enumTypeInfo)
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
                if (!hr.Succeeded || pTypeAttr is null)
                {
                    throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
                }

                try
                {
                    uint nItems = pTypeAttr->cVars;

                    Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, $"ProcessTypeInfoEnum: processing {nItems} variables");

                    ArrayList strs = new ArrayList();
                    ArrayList vars = new ArrayList();

                    object varValue = null;

                    using var enumNameBstr = new BSTR();
                    using var enumHelpStringBstr = new BSTR();
                    enumTypeInfo.GetDocumentation(Ole32.DispatchID.MEMBERID_NIL, &enumNameBstr, &enumHelpStringBstr, null, null);

                    // For each item in the enum type info, we just need it's name and value and
                    // helpstring if it's there.
                    for (uint i = 0; i < nItems; i++)
                    {
                        VARDESC* pVarDesc = null;
                        hr = enumTypeInfo.GetVarDesc(i, &pVarDesc);
                        if (!hr.Succeeded || pVarDesc is null)
                        {
                            Debug.WriteLineIf(
                                DbgTypeInfoProcessorSwitch.TraceVerbose,
                                $"ProcessTypeInfoEnum: ignoring item 0x{i:X} because Oleaut32.ITypeInfo::GetVarDesc returned hr=0x{hr:X} or NULL");
                            continue;
                        }

                        try
                        {
                            if (pVarDesc->varkind != VARKIND.VAR_CONST || pVarDesc->Anonymous.lpvarValue == null)
                            {
                                continue;
                            }

                            varValue = null;

                            // get the name and the helpstring
                            using BSTR nameBstr = new();
                            using BSTR helpBstr = new();
                            hr = enumTypeInfo.GetDocumentation((Ole32.DispatchID)pVarDesc->memid, &nameBstr, &helpBstr, null, null);
                            if (!hr.Succeeded)
                            {
                                Debug.WriteLineIf(
                                    DbgTypeInfoProcessorSwitch.TraceVerbose,
                                    $"ProcessTypeInfoEnum: ignoring item 0x{i:X} because Oleaut32.ITypeInfo::GetDocumentation returned hr=0x{(int)hr:X} or NULL");
                                continue;
                            }

                            var name = nameBstr.AsSpan();
                            var helpString = helpBstr.AsSpan();
                            Debug.WriteLineIf(
                                DbgTypeInfoProcessorSwitch.TraceVerbose,
                                $"ProcessTypeInfoEnum got name={name}, helpstring={helpString}");

                            // Get the value.
                            try
                            {
                                varValue = Marshal.GetObjectForNativeVariant((nint)(void*)pVarDesc->Anonymous.lpvarValue);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLineIf(
                                    DbgTypeInfoProcessorSwitch.TraceVerbose,
                                    $"ProcessTypeInfoEnum: PtrtoStructFailed {ex.GetType().Name},{ex.Message}");
                            }

                            Debug.WriteLineIf(
                                DbgTypeInfoProcessorSwitch.TraceVerbose,
                                $"ProcessTypeInfoEnum: adding variable value={Convert.ToString(varValue, CultureInfo.InvariantCulture)}");
                            vars.Add(varValue);

                            // If we have a helpstring, use it, otherwise use name.
                            string nameString;
                            if (!helpString.IsEmpty)
                            {
                                nameString = helpString.ToString();
                            }
                            else
                            {
                                Debug.Assert(!name.IsEmpty, "No name for VARDESC member, but GetDocumentation returned S_OK!");
                                nameString = name.ToString();
                            }

                            Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, $"ProcessTypeInfoEnum: adding name value={nameString}");
                            strs.Add(nameString);
                        }
                        finally
                        {
                            enumTypeInfo.ReleaseVarDesc(pVarDesc);
                        }
                    }

                    Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, $"ProcessTypeInfoEnum: returning enum with {strs.Count} items");

                    // Just build our enumerator.
                    if (strs.Count > 0)
                    {
                        // Get the IUnknown value of the Oleaut32.ITypeInfo.
                        IntPtr pTypeInfoUnk = Marshal.GetIUnknownForObject(enumTypeInfo);

                        try
                        {
                            string enumName = $"{pTypeInfoUnk}_{enumNameBstr.AsSpan()}";

                            if (builtEnums is null)
                            {
                                builtEnums = new Hashtable();
                            }
                            else if (builtEnums.ContainsKey(enumName))
                            {
                                return (Type)builtEnums[enumName];
                            }

                            Type enumType = typeof(int);

                            if (vars.Count > 0 && vars[0] is not null)
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

        private static unsafe void ProcessVariables(Oleaut32.ITypeInfo typeInfo, IDictionary propInfoList, Ole32.DispatchID dispidToGet, Ole32.DispatchID nameDispID)
        {
            TYPEATTR* pTypeAttr = null;
            HRESULT hr = typeInfo.GetTypeAttr(&pTypeAttr);
            if (!hr.Succeeded || pTypeAttr is null)
            {
                throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
            }

            try
            {
                for (uint i = 0; i < pTypeAttr->cVars; i++)
                {
                    VARDESC* pVarDesc = null;
                    hr = typeInfo.GetVarDesc(i, &pVarDesc);
                    if (!hr.Succeeded || pVarDesc is null)
                    {
                        Debug.WriteLineIf(
                            DbgTypeInfoProcessorSwitch.TraceVerbose,
                            $"ProcessTypeInfoEnum: ignoring variable item 0x{i:X} because Oleaut32.ITypeInfo::GetFuncDesc returned hr=0x{hr:X} or NULL");
                        continue;
                    }

                    try
                    {
                        if (pVarDesc->varkind == VARKIND.VAR_CONST
                            || (dispidToGet != Ole32.DispatchID.MEMBERID_NIL && pVarDesc->memid != (int)dispidToGet))
                        {
                            continue;
                        }

                        unsafe
                        {
                            PropInfo pi = ProcessDataCore(typeInfo, propInfoList, (Ole32.DispatchID)pVarDesc->memid, nameDispID, in pVarDesc->elemdescVar.tdesc, pVarDesc->wVarFlags);
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
                case VT_EMPTY:
                case VT_NULL:
                    return null;
                case VT_I1:
                    return typeof(sbyte);
                case VT_UI1:
                    return typeof(byte);
                case VT_I2:
                    return typeof(short);
                case VT_UI2:
                    return typeof(ushort);
                case VT_I4:
                case VT_INT:
                    return typeof(int);
                case VT_UI4:
                case VT_UINT:
                    return typeof(uint);
                case VT_I8:
                    return typeof(long);
                case VT_UI8:
                    return typeof(ulong);
                case VT_R4:
                    return typeof(float);
                case VT_R8:
                    return typeof(double);
                case VT_CY:
                    return typeof(decimal);
                case VT_DATE:
                    return typeof(DateTime);
                case VT_BSTR:
                case VT_LPSTR:
                case VT_LPWSTR:
                    return typeof(string);
                case VT_DISPATCH:
                    return typeof(IDispatch);
                case VT_UNKNOWN:
                    return typeof(object);
                case VT_ERROR:
                case VT_HRESULT:
                    return typeof(int);
                case VT_BOOL:
                    return typeof(bool);
                case VT_VARIANT:
                    return typeof(Com2Variant);
                case VT_CLSID:
                    return typeof(Guid);
                case VT_FILETIME:
                    return typeof(Runtime.InteropServices.ComTypes.FILETIME);
                case VT_USERDEFINED:
                    throw new ArgumentException(string.Format(SR.COM2UnhandledVT, "VT_USERDEFINED"));
                case VT_VOID:
                case VT_PTR:
                case VT_SAFEARRAY:
                case VT_CARRAY:
                case VT_RECORD:
                case VT_BLOB:
                case VT_STREAM:
                case VT_STORAGE:
                case VT_STREAMED_OBJECT:
                case VT_STORED_OBJECT:
                case VT_BLOB_OBJECT:
                case VT_CF:
                case VT_BSTR_BLOB:
                case VT_VECTOR:
                case VT_ARRAY:
                case VT_BYREF:
                case VT_RESERVED:
                default:
                    throw new ArgumentException(string.Format(SR.COM2UnhandledVT, ((int)vt).ToString(CultureInfo.InvariantCulture)));
            }
        }
    }
}

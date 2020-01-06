// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using static Interop;

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

        private static ModuleBuilder moduleBuilder = null;

        private static ModuleBuilder ModuleBuilder
        {
            get
            {
                if (moduleBuilder == null)
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
        public static UnsafeNativeMethods.ITypeInfo FindTypeInfo(object obj, bool wantCoClass)
        {
            UnsafeNativeMethods.ITypeInfo pTypeInfo = null;

            // This is kind of odd.  What's going on here is that if we want the CoClass (e.g. for
            // the interface name), we need to look for IProvideClassInfo first, then look for the
            // typeinfo from the IDispatch. In the case of many OleAut32 operations, the CoClass
            // doesn't have the interface members on it, although in the shell it usually does, so
            // we need to re-order the lookup if we *actually* want the CoClass if it's available.

            for (int i = 0; pTypeInfo == null && i < 2; i++)
            {
                if (wantCoClass == (i == 0))
                {
                    if (obj is NativeMethods.IProvideClassInfo pProvideClassInfo)
                    {
                        pProvideClassInfo.GetClassInfo(out pTypeInfo);
                    }
                }
                else
                {
                    if (obj is UnsafeNativeMethods.IDispatch iDispatch)
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
        public static UnsafeNativeMethods.ITypeInfo[] FindTypeInfos(object obj, bool wantCoClass)
        {
            UnsafeNativeMethods.ITypeInfo[] typeInfos = null;
            int n = 0;
            UnsafeNativeMethods.ITypeInfo temp = null;

            if (obj is NativeMethods.IProvideMultipleClassInfo pCI)
            {
                if (!NativeMethods.Succeeded(pCI.GetMultiTypeInfoCount(ref n)) || n == 0)
                {
                    n = 0;
                }

                if (n > 0)
                {
                    typeInfos = new UnsafeNativeMethods.ITypeInfo[n];

                    for (int i = 0; i < n; i++)
                    {
                        if (NativeMethods.Failed(pCI.GetInfoOfIndex(i, 1 /*MULTICLASSINFO_GETTYPEINFO*/, ref temp, 0, 0, IntPtr.Zero, IntPtr.Zero)))
                        {
                            continue;
                        }
                        Debug.Assert(temp != null, "IProvideMultipleClassInfo::GetInfoOfIndex returned S_OK for ITypeInfo index " + i + ", this is a issue in the object that's being browsed, NOT the property browser.");
                        typeInfos[i] = temp;
                    }
                }
            }

            if (typeInfos == null || typeInfos.Length == 0)
            {
                temp = FindTypeInfo(obj, wantCoClass);
                if (temp != null)
                {
                    typeInfos = new UnsafeNativeMethods.ITypeInfo[] { temp };
                }
            }

            return typeInfos;
        }

        /// <summary>
        ///  Retrieve the dispid of the property that we are to use as the name
        ///  member.  In this case, the grid will put parens around the name.
        /// </summary>
        public unsafe static Ole32.DispatchID GetNameDispId(UnsafeNativeMethods.IDispatch obj)
        {
            Ole32.DispatchID dispid = Ole32.DispatchID.UNKNOWN;
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
                cnd.GetPropertyValue(obj, Ole32.DispatchID.Name, ref succeeded);
                if (succeeded)
                {
                    dispid = Ole32.DispatchID.Name;
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
                Ole32.DispatchID pDispid = Ole32.DispatchID.UNKNOWN;
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

            if (obj == null || !Marshal.IsComObject(obj))
            {
                Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "Com2TypeInfoProcessor.GetProperties returning null: Object is not a com Object");
                return null;
            }

            UnsafeNativeMethods.ITypeInfo[] typeInfos = FindTypeInfos(obj, false);

            // oops, looks like this guy doesn't surface any type info
            // this is okay, so we just say it has no props
            if (typeInfos == null || typeInfos.Length == 0)
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
                UnsafeNativeMethods.ITypeInfo ti = typeInfos[i];

                if (ti == null)
                {
                    continue;
                }

                int[] versions = new int[2];
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
                    props = InternalGetProperties(obj, ti, Ole32.DispatchID.MEMBERID_NIL, ref temp);

                    // only save the default property from the first type Info
                    if (i == 0 && temp != -1)
                    {
                        defaultProp = temp;
                    }

                    if (processedLibraries == null)
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

        private static Guid GetGuidForTypeInfo(UnsafeNativeMethods.ITypeInfo typeInfo, int[] versions)
        {
            IntPtr pTypeAttr = IntPtr.Zero;
            HRESULT hr = typeInfo.GetTypeAttr(ref pTypeAttr);
            if (!hr.Succeeded())
            {
                throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
            }

            try
            {
                ref readonly Ole32.TYPEATTR typeAttr = ref UnsafeNativeMethods.PtrToRef<Ole32.TYPEATTR>(pTypeAttr);
                if (versions != null)
                {
                    versions[0] = typeAttr.wMajorVerNum;
                    versions[1] = typeAttr.wMinorVerNum;
                }

                return typeAttr.guid;
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
        private static Type GetValueTypeFromTypeDesc(in Ole32.TYPEDESC typeDesc, UnsafeNativeMethods.ITypeInfo typeInfo, object[] typeData)
        {
            IntPtr hreftype;
            HRESULT hr = HRESULT.S_OK;

            switch (typeDesc.vt)
            {
                default:
                    return VTToType(typeDesc.vt);

                case Ole32.VARENUM.UNKNOWN:
                case Ole32.VARENUM.DISPATCH:
                    // get the guid
                    typeData[0] = GetGuidForTypeInfo(typeInfo, null);

                    // return the type
                    return VTToType(typeDesc.vt);

                case Ole32.VARENUM.USERDEFINED:
                    // we'll need to recurse into a user defined reference typeinfo
                    Debug.Assert(typeDesc.unionMember != IntPtr.Zero, "typeDesc doesn't contain an hreftype!");
                    hreftype = typeDesc.unionMember;
                    break;

                case Ole32.VARENUM.PTR:
                    // we'll need to recurse into a user defined reference typeinfo
                    Debug.Assert(typeDesc.unionMember != IntPtr.Zero, "typeDesc doesn't contain an refTypeDesc!");
                    ref readonly Ole32.TYPEDESC refTypeDesc = ref UnsafeNativeMethods.PtrToRef<Ole32.TYPEDESC>(typeDesc.unionMember);

                    if (refTypeDesc.vt == Ole32.VARENUM.VARIANT)
                    {
                        return VTToType(refTypeDesc.vt);
                    }

                    hreftype = refTypeDesc.unionMember;
                    break;
            }

            // get the reference type info
            UnsafeNativeMethods.ITypeInfo refTypeInfo = null;

            hr = typeInfo.GetRefTypeInfo(hreftype, ref refTypeInfo);
            if (!hr.Succeeded())
            {
                throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetRefTypeInfoFailed, hr), (int)hr);
            }

            try
            {
                // here is where we look at the next level type info.
                // if we get an enum, process it, otherwise we will recurse
                // or get a dispatch.
                //
                if (refTypeInfo != null)
                {
                    IntPtr pRefTypeAttr = IntPtr.Zero;
                    hr = refTypeInfo.GetTypeAttr(ref pRefTypeAttr);
                    if (!hr.Succeeded())
                    {
                        throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
                    }

                    try
                    {
                        ref readonly Ole32.TYPEATTR refTypeAttr = ref UnsafeNativeMethods.PtrToRef<Ole32.TYPEATTR>(pRefTypeAttr);
                        Guid g = refTypeAttr.guid;

                        // save the guid if we've got one here
                        if (!Guid.Empty.Equals(g))
                        {
                            typeData[0] = g;
                        }

                        switch (refTypeAttr.typekind)
                        {
                            case Ole32.TYPEKIND.ENUM:
                                return ProcessTypeInfoEnum(refTypeInfo);
                            case Ole32.TYPEKIND.ALIAS:
                                // recurse here
                                return GetValueTypeFromTypeDesc(refTypeAttr.tdescAlias, refTypeInfo, typeData);
                            case Ole32.TYPEKIND.DISPATCH:
                                return VTToType(Ole32.VARENUM.DISPATCH);
                            case Ole32.TYPEKIND.INTERFACE:
                            case Ole32.TYPEKIND.COCLASS:
                                return VTToType(Ole32.VARENUM.UNKNOWN);
                            default:
                                return null;
                        }
                    }
                    finally
                    {
                        refTypeInfo.ReleaseTypeAttr(pRefTypeAttr);
                    }
                }
            }
            finally
            {
                refTypeInfo = null;
            }
            return null;
        }

        private static PropertyDescriptor[] InternalGetProperties(object obj, UnsafeNativeMethods.ITypeInfo typeInfo, Ole32.DispatchID dispidToGet, ref int defaultIndex)
        {
            if (typeInfo == null)
            {
                return null;
            }

            Hashtable propInfos = new Hashtable();

            Ole32.DispatchID nameDispID = GetNameDispId((UnsafeNativeMethods.IDispatch)obj);
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
                //Debug.Assert(pi.nonbrowsable || pi.valueType != null, "Browsable property '" + pi.name + "' has a null type");
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

        private static PropInfo ProcessDataCore(UnsafeNativeMethods.ITypeInfo typeInfo, IDictionary propInfoList, Ole32.DispatchID dispid, Ole32.DispatchID nameDispID, in Ole32.TYPEDESC typeDesc, Ole32.VARFLAGS flags)
        {
            string pPropName = null;
            string pPropDesc = null;

            // get the name and the helpstring
            HRESULT hr = typeInfo.GetDocumentation(dispid, ref pPropName, ref pPropDesc, null, null);
            ComNativeDescriptor cnd = ComNativeDescriptor.Instance;
            if (!hr.Succeeded())
            {
                throw new COMException(string.Format(SR.TYPEINFOPROCESSORGetDocumentationFailed, dispid, hr, cnd.GetClassName(typeInfo)), (int)hr);
            }

            if (pPropName == null)
            {
                Debug.Fail(string.Format(CultureInfo.CurrentCulture, "ITypeInfo::GetDocumentation didn't return a name for DISPID 0x{0:X} but returned SUCEEDED(hr),  Component=" + cnd.GetClassName(typeInfo), dispid));
                return null;
            }

            // now we can create our struct... make sure we don't already have one
            PropInfo pi = (PropInfo)propInfoList[pPropName];

            if (pi == null)
            {
                pi = new PropInfo
                {
                    Index = propInfoList.Count
                };
                propInfoList[pPropName] = pi;
                pi.Name = pPropName;
                pi.DispId = dispid;
                pi.Attributes.Add(new DispIdAttribute((int)pi.DispId));
            }

            if (pPropDesc != null)
            {
                pi.Attributes.Add(new DescriptionAttribute(pPropDesc));
            }

            // figure out the value type
            if (pi.ValueType == null)
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
                if (pi.ValueType == null)
                {
                    pi.NonBrowsable = true;
                }

                if (pi.NonBrowsable)
                {
                    flags |= Ole32.VARFLAGS.FNONBROWSABLE;
                }

                if (pTypeData[0] != null)
                {
                    pi.TypeData = pTypeData[0];
                }
            }

            // check the flags
            if ((flags & Ole32.VARFLAGS.FREADONLY) != 0)
            {
                pi.ReadOnly = PropInfo.ReadOnlyTrue;
            }

            if ((flags & Ole32.VARFLAGS.FHIDDEN) != 0 ||
                (flags & Ole32.VARFLAGS.FNONBROWSABLE) != 0 ||
                pi.Name[0] == '_' ||
                dispid == Ole32.DispatchID.HWND)
            {
                pi.Attributes.Add(new BrowsableAttribute(false));
                pi.NonBrowsable = true;
            }

            if ((flags & Ole32.VARFLAGS.FUIDEFAULT) != 0)
            {
                pi.IsDefault = true;
            }

            if ((flags & Ole32.VARFLAGS.FBINDABLE) != 0 &&
                (flags & Ole32.VARFLAGS.FDISPLAYBIND) != 0)
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

        private unsafe static void ProcessFunctions(UnsafeNativeMethods.ITypeInfo typeInfo, IDictionary propInfoList, Ole32.DispatchID dispidToGet, Ole32.DispatchID nameDispID, ref bool addAboutBox)
        {
            IntPtr pTypeAttr = IntPtr.Zero;
            HRESULT hr = typeInfo.GetTypeAttr(ref pTypeAttr);
            if (!hr.Succeeded() || pTypeAttr == IntPtr.Zero)
            {
                throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
            }

            try
            {
                ref readonly Ole32.TYPEATTR typeAttr = ref UnsafeNativeMethods.PtrToRef<Ole32.TYPEATTR>(pTypeAttr);

                bool isPropGet;
                PropInfo pi;

                for (int i = 0; i < typeAttr.cFuncs; i++)
                {
                    IntPtr pFuncDesc = IntPtr.Zero;
                    hr = typeInfo.GetFuncDesc(i, ref pFuncDesc);
                    if (!hr.Succeeded() || pFuncDesc == IntPtr.Zero)
                    {
                        Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "ProcessTypeInfoEnum: ignoring function item 0x{0:X} because ITypeInfo::GetFuncDesc returned hr=0x{1:X} or NULL", i, hr));
                        continue;
                    }

                    try
                    {
                        ref readonly Ole32.FUNCDESC funcDesc = ref UnsafeNativeMethods.PtrToRef<Ole32.FUNCDESC>(pFuncDesc);
                        if (funcDesc.invkind == Ole32.INVOKEKIND.FUNC ||
                            (dispidToGet != Ole32.DispatchID.MEMBERID_NIL && funcDesc.memid != dispidToGet))
                        {

                            if (funcDesc.memid == Ole32.DispatchID.ABOUTBOX)
                            {
                                addAboutBox = true;
                            }
                            continue;
                        }

                        Ole32.TYPEDESC typeDesc;

                        // is this a get or a put?
                        isPropGet = (funcDesc.invkind == Ole32.INVOKEKIND.PROPERTYGET);

                        if (isPropGet)
                        {

                            if (funcDesc.cParams != 0)
                            {

                                continue;
                            }

                            unsafe
                            {
                                typeDesc = funcDesc.elemdescFunc.tdesc;
                            }
                        }
                        else
                        {
                            Debug.Assert(funcDesc.lprgelemdescParam != null, "ELEMDESC param is null!");
                            if (funcDesc.lprgelemdescParam == null || funcDesc.cParams != 1)
                            {

                                continue;
                            }

                            unsafe
                            {
                                typeDesc = funcDesc.lprgelemdescParam->tdesc;
                            }
                        }
                        pi = ProcessDataCore(typeInfo, propInfoList, funcDesc.memid, nameDispID, in typeDesc, (Ole32.VARFLAGS)funcDesc.wFuncFlags);

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
        private static Type ProcessTypeInfoEnum(UnsafeNativeMethods.ITypeInfo enumTypeInfo)
        {
            Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "ProcessTypeInfoEnum entered");

            if (enumTypeInfo == null)
            {
                Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "ProcessTypeInfoEnum got a NULL enumTypeInfo");
                return null;
            }

            try
            {
                IntPtr pTypeAttr = IntPtr.Zero;
                HRESULT hr = enumTypeInfo.GetTypeAttr(ref pTypeAttr);
                if (!hr.Succeeded() || pTypeAttr == IntPtr.Zero)
                {
                    throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
                }

                try
                {
                    ref readonly Ole32.TYPEATTR typeAttr = ref UnsafeNativeMethods.PtrToRef<Ole32.TYPEATTR>(pTypeAttr);

                    int nItems = typeAttr.cVars;

                    Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "ProcessTypeInfoEnum: processing " + nItems.ToString(CultureInfo.InvariantCulture) + " variables");

                    ArrayList strs = new ArrayList();
                    ArrayList vars = new ArrayList();

                    object varValue = null;
                    string enumName = null;
                    string name = null;
                    string helpstr = null;

                    enumTypeInfo.GetDocumentation(Ole32.DispatchID.MEMBERID_NIL, ref enumName, ref helpstr, null, null);

                    // For each item in the enum type info,
                    // we just need it's name and value, and helpstring if it's there.
                    //
                    for (int i = 0; i < nItems; i++)
                    {
                        IntPtr pVarDesc = IntPtr.Zero;
                        hr = enumTypeInfo.GetVarDesc(i, ref pVarDesc);
                        if (!hr.Succeeded() || pVarDesc == IntPtr.Zero)
                        {
                            Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "ProcessTypeInfoEnum: ignoring item 0x{0:X} because ITypeInfo::GetVarDesc returned hr=0x{1:X} or NULL", i, hr));
                            continue;
                        }

                        try
                        {
                            ref readonly Ole32.VARDESC varDesc = ref UnsafeNativeMethods.PtrToRef<Ole32.VARDESC>(pVarDesc);
                            if (varDesc.varkind != Ole32.VARKIND.CONST || varDesc.unionMember == IntPtr.Zero)
                            {
                                continue;
                            }

                            name = helpstr = null;
                            varValue = null;

                            // get the name and the helpstring
                            hr = enumTypeInfo.GetDocumentation(varDesc.memid, ref name, ref helpstr, null, null);
                            if (!hr.Succeeded())
                            {
                                Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "ProcessTypeInfoEnum: ignoring item 0x{0:X} because ITypeInfo::GetDocumentation returned hr=0x{1:X} or NULL", i, hr));
                                continue;
                            }

                            Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "ProcessTypeInfoEnum got name=" + (name ?? "(null)") + ", helpstring=" + (helpstr ?? "(null)"));

                            // get the value
                            try
                            {
                                varValue = Marshal.GetObjectForNativeVariant(varDesc.unionMember);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "ProcessTypeInfoEnum: PtrtoStructFailed " + ex.GetType().Name + "," + ex.Message);
                            }

                            Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "ProcessTypeInfoEnum: adding variable value=" + Convert.ToString(varValue, CultureInfo.InvariantCulture));
                            vars.Add(varValue);

                            // if we have a helpstring, use it, otherwise use name
                            string nameString;
                            if (helpstr != null)
                            {
                                nameString = helpstr;
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
                            if (pVarDesc != IntPtr.Zero)
                            {
                                enumTypeInfo.ReleaseVarDesc(pVarDesc);
                            }
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
                            enumName = pTypeInfoUnk.ToString() + "_" + enumName;

                            if (builtEnums == null)
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

        private static void ProcessVariables(UnsafeNativeMethods.ITypeInfo typeInfo, IDictionary propInfoList, Ole32.DispatchID dispidToGet, Ole32.DispatchID nameDispID)
        {
            IntPtr pTypeAttr = IntPtr.Zero;
            HRESULT hr = typeInfo.GetTypeAttr(ref pTypeAttr);
            if (!hr.Succeeded() || pTypeAttr == IntPtr.Zero)
            {
                throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
            }

            try
            {
                ref readonly Ole32.TYPEATTR typeAttr = ref UnsafeNativeMethods.PtrToRef<Ole32.TYPEATTR>(pTypeAttr);
                for (int i = 0; i < typeAttr.cVars; i++)
                {
                    IntPtr pVarDesc = IntPtr.Zero;
                    hr = typeInfo.GetVarDesc(i, ref pVarDesc);
                    if (!hr.Succeeded() || pVarDesc == IntPtr.Zero)
                    {
                        Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "ProcessTypeInfoEnum: ignoring variable item 0x{0:X} because ITypeInfo::GetFuncDesc returned hr=0x{1:X} or NULL", i, hr));
                        continue;
                    }

                    try
                    {
                        ref readonly Ole32.VARDESC varDesc = ref UnsafeNativeMethods.PtrToRef<Ole32.VARDESC>(pVarDesc);

                        if (varDesc.varkind == Ole32.VARKIND.CONST ||
                            (dispidToGet != Ole32.DispatchID.MEMBERID_NIL && varDesc.memid != dispidToGet))
                        {
                            continue;
                        }

                        unsafe
                        {
                            PropInfo pi = ProcessDataCore(typeInfo, propInfoList, varDesc.memid, nameDispID, in varDesc.elemdescVar.tdesc, varDesc.wVarFlags);
                            if (pi.ReadOnly != PropInfo.ReadOnlyTrue)
                            {
                                pi.ReadOnly = PropInfo.ReadOnlyFalse;
                            }
                        }
                    }
                    finally
                    {
                        if (pVarDesc != IntPtr.Zero)
                        {
                            typeInfo.ReleaseVarDesc(pVarDesc);
                        }
                    }
                }
            }
            finally
            {
                typeInfo.ReleaseTypeAttr(pTypeAttr);
            }
        }

        private static Type VTToType(Ole32.VARENUM vt)
        {
            switch (vt)
            {
                case Ole32.VARENUM.EMPTY:
                case Ole32.VARENUM.NULL:
                    return null;
                case Ole32.VARENUM.I1:
                    return typeof(sbyte);
                case Ole32.VARENUM.UI1:
                    return typeof(byte);
                case Ole32.VARENUM.I2:
                    return typeof(short);
                case Ole32.VARENUM.UI2:
                    return typeof(ushort);
                case Ole32.VARENUM.I4:
                case Ole32.VARENUM.INT:
                    return typeof(int);
                case Ole32.VARENUM.UI4:
                case Ole32.VARENUM.UINT:
                    return typeof(uint);
                case Ole32.VARENUM.I8:
                    return typeof(long);
                case Ole32.VARENUM.UI8:
                    return typeof(ulong);
                case Ole32.VARENUM.R4:
                    return typeof(float);
                case Ole32.VARENUM.R8:
                    return typeof(double);
                case Ole32.VARENUM.CY:
                    return typeof(decimal);
                case Ole32.VARENUM.DATE:
                    return typeof(DateTime);
                case Ole32.VARENUM.BSTR:
                case Ole32.VARENUM.LPSTR:
                case Ole32.VARENUM.LPWSTR:
                    return typeof(string);
                case Ole32.VARENUM.DISPATCH:
                    return typeof(UnsafeNativeMethods.IDispatch);
                case Ole32.VARENUM.UNKNOWN:
                    return typeof(object);
                case Ole32.VARENUM.ERROR:
                case Ole32.VARENUM.HRESULT:
                    return typeof(int);
                case Ole32.VARENUM.BOOL:
                    return typeof(bool);
                case Ole32.VARENUM.VARIANT:
                    return typeof(Com2Variant);
                case Ole32.VARENUM.CLSID:
                    return typeof(Guid);
                case Ole32.VARENUM.FILETIME:
                    return typeof(FILETIME);
                case Ole32.VARENUM.USERDEFINED:
                    throw new ArgumentException(string.Format(SR.COM2UnhandledVT, "VT_USERDEFINED"));
                case Ole32.VARENUM.VOID:
                case Ole32.VARENUM.PTR:
                case Ole32.VARENUM.SAFEARRAY:
                case Ole32.VARENUM.CARRAY:
                case Ole32.VARENUM.RECORD:
                case Ole32.VARENUM.BLOB:
                case Ole32.VARENUM.STREAM:
                case Ole32.VARENUM.STORAGE:
                case Ole32.VARENUM.STREAMED_OBJECT:
                case Ole32.VARENUM.STORED_OBJECT:
                case Ole32.VARENUM.BLOB_OBJECT:
                case Ole32.VARENUM.CF:
                case Ole32.VARENUM.BSTR_BLOB:
                case Ole32.VARENUM.VECTOR:
                case Ole32.VARENUM.ARRAY:
                case Ole32.VARENUM.BYREF:
                case Ole32.VARENUM.RESERVED:
                default:
                    throw new ArgumentException(string.Format(SR.COM2UnhandledVT, ((int)vt).ToString(CultureInfo.InvariantCulture)));
            }
        }

        internal class CachedProperties
        {
            private readonly PropertyDescriptor[] props;

            public readonly int MajorVersion;
            public readonly int MinorVersion;
            private readonly int defaultIndex;

            internal CachedProperties(PropertyDescriptor[] props, int defIndex, int majVersion, int minVersion)
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

            public Ole32.DispatchID DispId { get; set; } = Ole32.DispatchID.UNKNOWN;

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

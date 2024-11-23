// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;
using static Windows.Win32.System.Com.TYPEKIND;
using static Windows.Win32.System.Com.VARFLAGS;
using static Windows.Win32.System.Variant.VARENUM;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

/// <summary>
///  <para>
///   This is the main worker class of Com2 property interop. It takes an IDispatch Object
///   and translates it's ITypeInfo into Com2PropertyDescriptor objects that are understandable
///   by managed code.
///  </para>
///  <para>
///   This class only knows how to process things that are natively in the typeinfo. Other property
///   information such as IPerPropertyBrowsing is handled elsewhere.
///  </para>
/// </summary>
[RequiresUnreferencedCode(ComNativeDescriptor.ComTypeDescriptorsMessage + " Uses ComNativeDescriptor which is not trim-compatible.")]
internal static unsafe partial class Com2TypeInfoProcessor
{
    private static ModuleBuilder? s_moduleBuilder;

    private static ModuleBuilder ModuleBuilder
    {
        get
        {
            if (s_moduleBuilder is null)
            {
                AssemblyName assemblyName = new()
                {
                    Name = "COM2InteropEmit"
                };

                AssemblyBuilder aBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                s_moduleBuilder = aBuilder.DefineDynamicModule("COM2Interop.Emit");
            }

            return s_moduleBuilder;
        }
    }

    private static Dictionary<string, Type>? s_builtEnums;
    private static Dictionary<Guid, CachedProperties>? s_processedLibraries;

    /// <summary>
    ///  Given a COM object attempt to locate its type info.
    /// </summary>
    public static ITypeInfo* FindTypeInfo(object comObject, bool preferIProvideClassInfo)
    {
        ITypeInfo* typeInfo = null;

        // What's going on here is that if we want the CoClass (e.g. for the interface name), we need to look for
        // IProvideClassInfo first, then look for the typeinfo from the IDispatch. In the case of many Oleaut32
        // operations, the CoClass doesn't have the interface members on it, although in the shell it usually does, so
        // we need to re-order the lookup if we *actually* want the CoClass if it's available.
        for (int i = 0; typeInfo is null && i < 2; i++)
        {
            if (preferIProvideClassInfo == (i == 0))
            {
                using var provideClassInfo = ComHelpers.TryGetComScope<IProvideClassInfo>(comObject, out HRESULT hr);
                if (hr.Succeeded)
                {
                    // If this fails typeInfo will be null and we'll loop again if we haven't already.
                    provideClassInfo.Value->GetClassInfo(&typeInfo);
                }
            }
            else
            {
                using var dispatch = ComHelpers.TryGetComScope<IDispatch>(comObject, out HRESULT hr);
                if (hr.Succeeded)
                {
                    // If this fails typeInfo will be null and we'll loop again if we haven't already.
                    dispatch.Value->GetTypeInfo(0, PInvokeCore.GetThreadLocale(), &typeInfo);
                }
            }
        }

        return typeInfo;
    }

    /// <summary>
    ///  Given an object, this attempts to locate its type info. If it implements IProvideMultipleClassInfo
    ///  all available type infos will be returned, otherwise the primary one will be called.
    /// </summary>
    public static ITypeInfo*[] FindTypeInfos(object comObject)
    {
        using var classInfo = ComHelpers.TryGetComScope<IProvideMultipleClassInfo>(comObject, out HRESULT hr);
        if (hr.Succeeded)
        {
            uint count = 0;
            if (classInfo.Value->GetMultiTypeInfoCount(&count).Succeeded && count > 0)
            {
                List<nint> handles = new((int)count);
                for (uint i = 0; i < count; i++)
                {
                    ITypeInfo* typeInfo;
                    if (classInfo.Value->GetInfoOfIndex(
                        i,
                        MULTICLASSINFO_FLAGS.MULTICLASSINFO_GETTYPEINFO,
                        &typeInfo,
                        pdwTIFlags: null,
                        pcdispidReserved: null,
                        piidPrimary: null,
                        piidSource: null).Succeeded
                        && typeInfo is not null)
                    {
                        handles.Add((nint)typeInfo);
                    }
                }

                if (handles.Count > 0)
                {
                    ITypeInfo*[] typeInfos = new ITypeInfo*[handles.Count];
                    for (int i = 0; i < handles.Count; i++)
                    {
                        typeInfos[i] = (ITypeInfo*)handles[i];
                    }

                    return typeInfos;
                }
            }
        }

        ITypeInfo* temp = FindTypeInfo(comObject, preferIProvideClassInfo: false);
        return temp is not null ? ([temp]) : [];
    }

    /// <summary>
    ///  Retrieve the dispatch id of the property that we are to use as the name member.
    /// </summary>
    public static int GetNameDispId(IDispatch* dispatch)
    {
        int dispid = PInvokeCore.DISPID_UNKNOWN;
        string? name = null;

        // First try to find one with a valid value.
        HRESULT hr = ComNativeDescriptor.GetPropertyValue(dispatch, "__id", out _);

        if (hr.Succeeded)
        {
            name = "__id";
        }
        else
        {
            hr = ComNativeDescriptor.GetPropertyValue(dispatch, PInvokeCore.DISPID_Name, out _);
            if (hr.Succeeded)
            {
                dispid = PInvokeCore.DISPID_Name;
            }
            else
            {
                hr = ComNativeDescriptor.GetPropertyValue(dispatch, "Name", out _);
                if (hr.Succeeded)
                {
                    name = "Name";
                }
            }
        }

        // Now get the dispid of the one that worked.
        if (name is not null)
        {
            int pDispid = PInvokeCore.DISPID_UNKNOWN;
            Guid guid = Guid.Empty;

            fixed (char* n = name)
            {
                hr = dispatch->GetIDsOfNames(&guid, (PWSTR*)&n, 1, PInvokeCore.GetThreadLocale(), &pDispid);
                if (hr.Succeeded)
                {
                    dispid = pDispid;
                }
            }
        }

        return dispid;
    }

    public static Com2Properties? GetProperties(object comObject)
    {
        if (!ComHelpers.SupportsInterface<IDispatch>(comObject))
        {
            return null;
        }

        ITypeInfo*[] typeInfos = FindTypeInfos(comObject);

        if (typeInfos.Length == 0)
        {
            return null;
        }

        try
        {
            return ProcessTypeInfos(comObject, typeInfos);
        }
        finally
        {
            for (int i = 0; i < typeInfos.Length; i++)
            {
                typeInfos[i]->Release();
            }
        }
    }

    private static Com2Properties ProcessTypeInfos(object comObject, ITypeInfo*[] typeInfos)
    {
        int defaultProperty = -1;
        List<Com2PropertyDescriptor> propList = [];

        for (int i = 0; i < typeInfos.Length; i++)
        {
            ITypeInfo* typeInfo = typeInfos[i];

            uint[] versions = new uint[2];
            Guid typeGuid = GetGuidForTypeInfo(typeInfo, versions);
            Com2PropertyDescriptor[]? properties = null;

            s_processedLibraries ??= [];

            bool wasProcessed = typeGuid != Guid.Empty && s_processedLibraries.ContainsKey(typeGuid);

            if (wasProcessed)
            {
                CachedProperties cachedProperties = s_processedLibraries[typeGuid];

                if (versions[0] == cachedProperties.MajorVersion && versions[1] == cachedProperties.MinorVersion)
                {
                    properties = cachedProperties.Properties;
                    if (i == 0 && cachedProperties.DefaultIndex != -1)
                    {
                        defaultProperty = cachedProperties.DefaultIndex;
                    }
                }
                else
                {
                    // Updated version, mark for reprocessing.
                    wasProcessed = false;
                }
            }

            if (!wasProcessed)
            {
                using var dispatch = ComHelpers.GetComScope<IDispatch>(comObject);
                properties = InternalGetProperties(dispatch, typeInfo, PInvoke.MEMBERID_NIL);

                // Only save the default property from the first type Info.
                if (i == 0)
                {
                    defaultProperty = -1;
                }

                if (typeGuid != Guid.Empty)
                {
                    s_processedLibraries[typeGuid] = new CachedProperties(properties, i == 0 ? defaultProperty : -1, versions[0], versions[1]);
                }
            }

            if (properties is not null)
            {
                propList.AddRange(properties);
            }
        }

        // Done!
        Com2PropertyDescriptor[] temp2 = new Com2PropertyDescriptor[propList.Count];
        propList.CopyTo(temp2, 0);

        return new Com2Properties(comObject, [.. propList], defaultProperty);
    }

    private static unsafe Guid GetGuidForTypeInfo(ITypeInfo* typeInfo, uint[]? versions)
    {
        TYPEATTR* pTypeAttr = null;
        HRESULT hr = typeInfo->GetTypeAttr(&pTypeAttr);
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
            typeInfo->ReleaseTypeAttr(pTypeAttr);
        }
    }

    /// <summary>
    ///  Resolves a value type for a property from a TYPEDESC. Value types can be user defined, which and may be
    ///  aliased into other type infos. This function will recursively walk the ITypeInfos to resolve the type to
    ///  a CLR Type.
    /// </summary>
    private static unsafe Type? GetValueTypeFromTypeDesc(ref TYPEDESC typeDesc, ITypeInfo* typeInfo, ref Guid typeGuid)
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
                typeGuid = GetGuidForTypeInfo(typeInfo, null);

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
        using ComScope<ITypeInfo> refTypeInfo = new(null);
        hr = typeInfo->GetRefTypeInfo(hreftype, refTypeInfo);
        if (!hr.Succeeded)
        {
            throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetRefTypeInfoFailed, hr), (int)hr);
        }

        // Here is where we look at the next level type info. If we get an enum, process it, otherwise we will
        // recurse or get a dispatch.
        if (refTypeInfo.IsNull)
        {
            return null;
        }

        TYPEATTR* pTypeAttr = null;
        hr = refTypeInfo.Value->GetTypeAttr(&pTypeAttr);
        if (!hr.Succeeded)
        {
            throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
        }

        try
        {
            Guid guid = pTypeAttr->guid;

            // Save the guid if we've got one here.
            if (!Guid.Empty.Equals(guid))
            {
                typeGuid = guid;
            }

            return pTypeAttr->typekind switch
            {
                TKIND_ENUM => ProcessTypeInfoEnum(refTypeInfo),

                // Recurse here.
                TKIND_ALIAS => GetValueTypeFromTypeDesc(ref pTypeAttr->tdescAlias, refTypeInfo, ref typeGuid),
                TKIND_DISPATCH => VTToType(VT_DISPATCH),
                TKIND_INTERFACE or TKIND_COCLASS => VTToType(VT_UNKNOWN),
                _ => null,
            };
        }
        finally
        {
            refTypeInfo.Value->ReleaseTypeAttr(pTypeAttr);
        }
    }

    private static Com2PropertyDescriptor[] InternalGetProperties(
        IDispatch* dispatch,
        ITypeInfo* typeInfo,
        int dispidToGet)
    {
        Dictionary<string, PropertyInfo> propertyInfo = [];

        int nameDispID = GetNameDispId(dispatch);
        bool addAboutBox = false;

        // Properties can live as functions with get_ and put_ or as variables, so we do two steps here.
        try
        {
            // Do FUNCDESC things.
            ProcessFunctions(typeInfo, propertyInfo, dispidToGet, nameDispID, ref addAboutBox);
        }
        catch (ExternalException ex)
        {
            Debug.Fail($"ProcessFunctions failed with hr={ex.ErrorCode}, message={ex}");
        }

        try
        {
            // Do VARDESC things.
            ProcessVariables(typeInfo, propertyInfo, dispidToGet, nameDispID);
        }
        catch (ExternalException ex)
        {
            Debug.Fail($"ProcessVariables failed with hr={ex.ErrorCode}, message={ex}");
        }

        // Now we take the propertyInfo structures we built up and use them to create the actual descriptors.
        int propertyCount = propertyInfo.Count;

        if (addAboutBox)
        {
            propertyCount++;
        }

        Com2PropertyDescriptor[] properties = new Com2PropertyDescriptor[propertyCount];

        HRESULT hr;

        // For each item in our list, create the descriptor an check if it's the default one.
        foreach (PropertyInfo info in propertyInfo.Values)
        {
            if (!info.NonBrowsable)
            {
                // Finally, for each property, make sure we can get the value
                // if we can't then we should mark it non-browsable.

                hr = ComNativeDescriptor.GetPropertyValue(dispatch, info.DispId, out object? pvar);

                if (!hr.Succeeded)
                {
                    // Hide the property.
                    info.Attributes.Add(new BrowsableAttribute(false));
                    info.NonBrowsable = true;
                }
            }
            else
            {
                hr = HRESULT.S_OK;
            }

            properties[info.Index] = new Com2PropertyDescriptor(
                info.DispId,
                info.Name,
                [.. info.Attributes],
                info.ReadOnly != PropertyInfo.ReadOnlyFalse,
                info.ValueType,
                info.TypeData,
                !hr.Succeeded);
        }

        if (addAboutBox)
        {
            properties[^1] = new Com2AboutBoxPropertyDescriptor();
        }

        return properties;
    }

    private static unsafe PropertyInfo ProcessDataCore(
        ITypeInfo* typeInfo,
        IDictionary<string, PropertyInfo> properties,
        int dispid,
        int nameDispid,
        TYPEDESC typeDescription,
        VARFLAGS flags)
    {
        // Get the name and the helpstring.
        using BSTR nameBstr = default;
        using BSTR helpStringBstr = default;
        HRESULT hr = typeInfo->GetDocumentation(dispid, &nameBstr, &helpStringBstr, null, null);
        if (!hr.Succeeded)
        {
            throw new COMException(string.Format(SR.TYPEINFOPROCESSORGetDocumentationFailed, dispid, hr, "ITypeInfo", (int)hr));
        }

        if (nameBstr.Length == 0)
        {
            Debug.Fail($"ITypeInfo::GetDocumentation didn't return a name for DISPID 0x{dispid:X} but returned SUCCEEDED(hr)");
            return null;
        }

        string name = nameBstr.ToString();

        // Now we can create our struct. Make sure we don't already have one.
        if (!properties.TryGetValue(name, out PropertyInfo? info))
        {
            info = new()
            {
                Index = properties.Count,
                Name = name,
                DispId = dispid
            };

            properties.Add(name, info);
            info.Attributes.Add(new DispIdAttribute(info.DispId));
        }

        if (helpStringBstr.Length > 0)
        {
            info.Attributes.Add(new DescriptionAttribute(helpStringBstr.ToString()));
        }

        // Figure out the value type.
        if (info.ValueType is null)
        {
            Guid typeGuid = Guid.Empty;
            try
            {
                info.ValueType = GetValueTypeFromTypeDesc(ref typeDescription, typeInfo, ref typeGuid);
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
            }

            // If we can't resolve the type, mark the property as nonbrowsable.
            if (info.ValueType is null)
            {
                info.NonBrowsable = true;
            }

            if (info.NonBrowsable)
            {
                flags |= VARFLAG_FNONBROWSABLE;
            }

            if (typeGuid != Guid.Empty)
            {
                info.TypeData = typeGuid;
            }
        }

        // Check the flags.
        if (flags.HasFlag(VARFLAG_FREADONLY))
        {
            info.ReadOnly = PropertyInfo.ReadOnlyTrue;
        }

        if (flags.HasFlag(VARFLAG_FHIDDEN)
            || flags.HasFlag(VARFLAG_FNONBROWSABLE)
            || info.Name[0] == '_'
            || dispid == PInvokeCore.DISPID_HWND)
        {
            info.Attributes.Add(new BrowsableAttribute(false));
            info.NonBrowsable = true;
        }

        if (flags.HasFlag(VARFLAG_FUIDEFAULT))
        {
            info.IsDefault = true;
        }

        if (flags.HasFlag(VARFLAG_FBINDABLE) && flags.HasFlag(VARFLAG_FDISPLAYBIND))
        {
            info.Attributes.Add(new BindableAttribute(true));
        }

        // Lastly, if it's DISPID_Name, add the ParenthesizeNameAttribute.
        if (dispid == nameDispid)
        {
            info.Attributes.Add(new ParenthesizePropertyNameAttribute(true));

            // Don't allow merges on the name.
            info.Attributes.Add(new MergablePropertyAttribute(false));
        }

        return info;
    }

    private static unsafe void ProcessFunctions(
        ITypeInfo* typeInfo,
        IDictionary<string, PropertyInfo> properties,
        int dispidToGet,
        int nameDispID,
        ref bool addAboutBox)
    {
        TYPEATTR* typeAttributes;
        HRESULT hr = typeInfo->GetTypeAttr(&typeAttributes);
        if (!hr.Succeeded || typeAttributes is null)
        {
            throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
        }

        try
        {
            bool isPropertyGetter;
            PropertyInfo? propertyInfo;

            for (uint i = 0; i < typeAttributes->cFuncs; i++)
            {
                FUNCDESC* functionDescription;
                hr = typeInfo->GetFuncDesc(i, &functionDescription);
                if (!hr.Succeeded || functionDescription is null)
                {
                    continue;
                }

                try
                {
                    if (functionDescription->invkind == INVOKEKIND.INVOKE_FUNC
                        || (dispidToGet != PInvoke.MEMBERID_NIL && functionDescription->memid != dispidToGet))
                    {
                        if (functionDescription->memid == PInvokeCore.DISPID_ABOUTBOX)
                        {
                            addAboutBox = true;
                        }

                        continue;
                    }

                    TYPEDESC typeDescription;

                    // Is this a get or a put?
                    isPropertyGetter = functionDescription->invkind == INVOKEKIND.INVOKE_PROPERTYGET;

                    if (isPropertyGetter)
                    {
                        if (functionDescription->cParams != 0)
                        {
                            continue;
                        }

                        typeDescription = functionDescription->elemdescFunc.tdesc;
                    }
                    else
                    {
                        Debug.Assert(functionDescription->lprgelemdescParam is not null, "ELEMDESC param is null!");
                        if (functionDescription->lprgelemdescParam is null || functionDescription->cParams != 1)
                        {
                            continue;
                        }

                        typeDescription = functionDescription->lprgelemdescParam->tdesc;
                    }

                    propertyInfo = ProcessDataCore(
                        typeInfo,
                        properties,
                        functionDescription->memid,
                        nameDispID,
                        typeDescription,
                        (VARFLAGS)functionDescription->wFuncFlags);

                    // If we got a set method, it's not readonly.
                    if (propertyInfo is not null && !isPropertyGetter)
                    {
                        propertyInfo.ReadOnly = PropertyInfo.ReadOnlyFalse;
                    }
                }
                finally
                {
                    typeInfo->ReleaseFuncDesc(functionDescription);
                }
            }
        }
        finally
        {
            typeInfo->ReleaseTypeAttr(typeAttributes);
        }
    }

    /// <summary>
    ///  This converts a type info that describes a IDL defined enum into one we can use
    /// </summary>
    private static unsafe Type? ProcessTypeInfoEnum(ITypeInfo* enumTypeInfo)
    {
        if (enumTypeInfo is null)
        {
            return null;
        }

        try
        {
            TYPEATTR* pTypeAttr;
            HRESULT hr = enumTypeInfo->GetTypeAttr(&pTypeAttr);
            if (!hr.Succeeded || pTypeAttr is null)
            {
                throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
            }

            try
            {
                uint nItems = pTypeAttr->cVars;

                List<string> strings = [];
                List<object?> vars = [];

                object? varValue = null;

                using BSTR enumNameBstr = default;
                using BSTR enumHelpStringBstr = default;
                enumTypeInfo->GetDocumentation(PInvoke.MEMBERID_NIL, &enumNameBstr, &enumHelpStringBstr, null, null);

                // For each item in the enum type info, we just need it's name and value and
                // helpstring if it's there.
                for (uint i = 0; i < nItems; i++)
                {
                    VARDESC* pVarDesc;
                    hr = enumTypeInfo->GetVarDesc(i, &pVarDesc);
                    if (!hr.Succeeded || pVarDesc is null)
                    {
                        continue;
                    }

                    try
                    {
                        if (pVarDesc->varkind != VARKIND.VAR_CONST || pVarDesc->Anonymous.lpvarValue == null)
                        {
                            continue;
                        }

                        varValue = null;

                        // Get the name and the helpstring
                        using BSTR nameBstr = default;
                        using BSTR helpBstr = default;
                        hr = enumTypeInfo->GetDocumentation(pVarDesc->memid, &nameBstr, &helpBstr, null, null);
                        if (!hr.Succeeded)
                        {
                            continue;
                        }

                        var name = nameBstr.AsSpan();
                        var helpString = helpBstr.AsSpan();

                        // Get the value.
                        try
                        {
                            varValue = (*pVarDesc->Anonymous.lpvarValue).ToObject();
                        }
                        catch (Exception ex) when (!ex.IsCriticalException())
                        {
                        }

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

                        strings.Add(nameString);
                    }
                    finally
                    {
                        enumTypeInfo->ReleaseVarDesc(pVarDesc);
                    }
                }

                // Just build our enumerator.
                if (strings.Count > 0)
                {
                    string enumName = $"ITypeInfo_{enumNameBstr.AsSpan()}";
                    s_builtEnums ??= [];
                    if (s_builtEnums.TryGetValue(enumName, out Type? typeValue))
                    {
                        return typeValue;
                    }

                    Type enumType = typeof(int);
                    if (vars.Count > 0 && vars[0] is { } var)
                    {
                        enumType = var.GetType();
                    }

                    EnumBuilder enumBuilder = ModuleBuilder.DefineEnum(enumName, TypeAttributes.Public, enumType);
                    for (int i = 0; i < strings.Count; i++)
                    {
                        enumBuilder.DefineLiteral(strings[i], vars[i]);
                    }

                    Type t = enumBuilder.CreateTypeInfo().AsType();
                    s_builtEnums[enumName] = t;
                    return t;
                }
            }
            finally
            {
                enumTypeInfo->ReleaseTypeAttr(pTypeAttr);
            }
        }
        catch (Exception ex)
        {
            Debug.Fail($"Failed to process type info enum. {ex.Message}");
        }

        return null;
    }

    private static unsafe void ProcessVariables(
        ITypeInfo* typeInfo,
        IDictionary<string, PropertyInfo> propertyInfo,
        int dispidToGet,
        int nameDispID)
    {
        TYPEATTR* pTypeAttr;
        HRESULT hr = typeInfo->GetTypeAttr(&pTypeAttr);
        if (!hr.Succeeded || pTypeAttr is null)
        {
            throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
        }

        try
        {
            for (uint i = 0; i < pTypeAttr->cVars; i++)
            {
                VARDESC* pVarDesc;
                hr = typeInfo->GetVarDesc(i, &pVarDesc);
                if (!hr.Succeeded || pVarDesc is null)
                {
                    continue;
                }

                try
                {
                    if (pVarDesc->varkind == VARKIND.VAR_CONST
                        || (dispidToGet != PInvoke.MEMBERID_NIL && pVarDesc->memid != dispidToGet))
                    {
                        continue;
                    }

                    PropertyInfo pi = ProcessDataCore(
                        typeInfo,
                        propertyInfo,
                        pVarDesc->memid,
                        nameDispID,
                        pVarDesc->elemdescVar.tdesc,
                        pVarDesc->wVarFlags);

                    if (pi.ReadOnly != PropertyInfo.ReadOnlyTrue)
                    {
                        pi.ReadOnly = PropertyInfo.ReadOnlyFalse;
                    }
                }
                finally
                {
                    typeInfo->ReleaseVarDesc(pVarDesc);
                }
            }
        }
        finally
        {
            typeInfo->ReleaseTypeAttr(pTypeAttr);
        }
    }

    private static Type? VTToType(VARENUM vt) => vt switch
    {
        VT_EMPTY or VT_NULL => null,
        VT_I1 => typeof(sbyte),
        VT_UI1 => typeof(byte),
        VT_I2 => typeof(short),
        VT_UI2 => typeof(ushort),
        VT_I4 or VT_INT => typeof(int),
        VT_UI4 or VT_UINT => typeof(uint),
        VT_I8 => typeof(long),
        VT_UI8 => typeof(ulong),
        VT_R4 => typeof(float),
        VT_R8 => typeof(double),
        VT_CY => typeof(decimal),
        VT_DATE => typeof(DateTime),
        VT_BSTR or VT_LPSTR or VT_LPWSTR => typeof(string),
        VT_DISPATCH => typeof(IDispatch),
        VT_UNKNOWN => typeof(object),
        VT_ERROR or VT_HRESULT => typeof(int),
        VT_BOOL => typeof(bool),
        VT_VARIANT => typeof(Com2Variant),
        VT_CLSID => typeof(Guid),
        VT_FILETIME => typeof(Runtime.InteropServices.ComTypes.FILETIME),
        VT_USERDEFINED => throw new ArgumentException(string.Format(SR.COM2UnhandledVT, "VT_USERDEFINED")),
        _ => throw new ArgumentException(string.Format(SR.COM2UnhandledVT, ((int)vt).ToString(CultureInfo.InvariantCulture))),
    };
}

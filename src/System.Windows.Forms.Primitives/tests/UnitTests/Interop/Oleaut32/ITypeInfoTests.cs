// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;
using Windows.Win32.Graphics.GdiPlus;

namespace System.Windows.Forms.Primitives.Tests.Interop.Oleaut32;

[Collection("Sequential")]
public class ITypeInfoTests
{
    [StaFact]
    public unsafe void ITypeInfo_AddressOfMember_Invoke_Success()
    {
        using Bitmap image = new(16, 32);
        using var iPictureDisp = image.CreateIPictureDisp();
        Assert.False(iPictureDisp.IsNull);
        using ComScope<ITypeInfo> typeInfo = new(null);
        ((IDispatch*)iPictureDisp.Value)->GetTypeInfo(0, PInvokeCore.GetThreadLocale(), typeInfo);

        void* pvObj;
        HRESULT hr = typeInfo.Value->AddressOfMember(6, INVOKEKIND.INVOKE_FUNC, &pvObj);
        Assert.Equal(HRESULT.TYPE_E_BADMODULEKIND, hr);
    }

    [StaFact]
    public unsafe void ITypeInfo_CreateInstance_Invoke_Success()
    {
        using Bitmap image = new(16, 32);
        using var iPictureDisp = image.CreateIPictureDisp();
        Assert.False(iPictureDisp.IsNull);
        using ComScope<ITypeInfo> typeInfo = new(null);
        ((IDispatch*)iPictureDisp.Value)->GetTypeInfo(0, PInvokeCore.GetThreadLocale(), typeInfo);

        void* pvObj;
        HRESULT hr = typeInfo.Value->CreateInstance(null, IID.Get<IPictureDisp>(), &pvObj);
        Assert.Equal(HRESULT.TYPE_E_BADMODULEKIND, hr);
    }

    [StaFact]
    public unsafe void ITypeInfo_GetContainingTypeLib_Invoke_Success()
    {
        using Bitmap image = new(16, 32);
        using var iPictureDisp = image.CreateIPictureDisp();
        Assert.False(iPictureDisp.IsNull);
        using ComScope<ITypeInfo> typeInfo = new(null);
        ((IDispatch*)iPictureDisp.Value)->GetTypeInfo(0, PInvokeCore.GetThreadLocale(), typeInfo);

        using ComScope<ITypeLib> typeLib = new(null);
        uint index = uint.MaxValue;
        typeInfo.Value->GetContainingTypeLib(typeLib, &index);

        Assert.NotEqual(0u, index);
    }

    [StaFact]
    public unsafe void ITypeInfo_GetDllEntry_Invoke_Success()
    {
        using Bitmap image = new(16, 32);
        using var iPictureDisp = image.CreateIPictureDisp();
        Assert.False(iPictureDisp.IsNull);
        using ComScope<ITypeInfo> typeInfo = new(null);
        ((IDispatch*)iPictureDisp.Value)->GetTypeInfo(0, PInvokeCore.GetThreadLocale(), typeInfo);

        using BSTR dllName = new("DllName");
        using BSTR name = new("Name");
        ushort wOrdinal = ushort.MaxValue;

        HRESULT hr = typeInfo.Value->GetDllEntry(6, INVOKEKIND.INVOKE_FUNC, &dllName, &name, &wOrdinal);
        Assert.Equal(HRESULT.TYPE_E_BADMODULEKIND, hr);
        Assert.Equal(0, dllName.Length);
        Assert.Equal(0, name.Length);
        Assert.Equal(0u, wOrdinal);
    }

    [StaFact]
    public unsafe void ITypeInfo_GetDocumentation_Invoke_Success()
    {
        using Bitmap image = new(16, 32);
        using var iPictureDisp = image.CreateIPictureDisp();
        Assert.False(iPictureDisp.IsNull);
        using ComScope<ITypeInfo> typeInfo = new(null);
        ((IDispatch*)iPictureDisp.Value)->GetTypeInfo(0, PInvokeCore.GetThreadLocale(), typeInfo);

        using BSTR name = new("Name");
        using BSTR docString = new("DocString");
        uint dwHelpContext = uint.MaxValue;
        using BSTR helpFile = new("HelpFile");
        typeInfo.Value->GetDocumentation(4, &name, &docString, &dwHelpContext, &helpFile);
        Assert.Equal("Width", name.ToString());
        Assert.Equal(0, docString.Length);
        Assert.Equal(0u, dwHelpContext);
        Assert.Equal(0, helpFile.Length);
    }

    [StaFact]
    public unsafe void ITypeInfo_GetFuncDesc_Invoke_Success()
    {
        using Bitmap image = new(16, 32);
        using var iPictureDisp = image.CreateIPictureDisp();
        Assert.False(iPictureDisp.IsNull);
        using ComScope<ITypeInfo> typeInfo = new(null);
        ((IDispatch*)iPictureDisp.Value)->GetTypeInfo(0, PInvokeCore.GetThreadLocale(), typeInfo);

        FUNCDESC* pFuncDesc = null;
        try
        {
            typeInfo.Value->GetFuncDesc(0, &pFuncDesc);
            Assert.Equal(6, pFuncDesc->memid);
            Assert.True(pFuncDesc->lprgscode is null);
            Assert.NotEqual(IntPtr.Zero, (IntPtr)pFuncDesc->lprgelemdescParam);
            Assert.Equal(FUNCKIND.FUNC_DISPATCH, pFuncDesc->funckind);
            Assert.Equal(INVOKEKIND.INVOKE_FUNC, pFuncDesc->invkind);
            Assert.Equal(CALLCONV.CC_STDCALL, pFuncDesc->callconv);
            Assert.Equal(10, pFuncDesc->cParams);
            Assert.Equal(0, pFuncDesc->cParamsOpt);
            Assert.Equal(0, pFuncDesc->oVft);
            Assert.Equal(0, pFuncDesc->cScodes);
            Assert.Equal(VARENUM.VT_VOID, pFuncDesc->elemdescFunc.tdesc.vt);
            Assert.True(pFuncDesc->elemdescFunc.tdesc.Anonymous.lpadesc is null);
            Assert.True(pFuncDesc->elemdescFunc.Anonymous.paramdesc.pparamdescex is null);
            Assert.True(pFuncDesc->elemdescFunc.Anonymous.paramdesc.pparamdescex is null);
        }
        finally
        {
            typeInfo.Value->ReleaseFuncDesc(pFuncDesc);
        }
    }

    [StaFact]
    public unsafe void ITypeInfo_GetIDsOfNames_Invoke_Success()
    {
        using Bitmap image = new(16, 32);
        using var iPictureDisp = image.CreateIPictureDisp();
        Assert.False(iPictureDisp.IsNull);
        using ComScope<ITypeInfo> typeInfo = new(null);
        ((IDispatch*)iPictureDisp.Value)->GetTypeInfo(0, PInvokeCore.GetThreadLocale(), typeInfo);

        fixed (char* width = "Width")
        fixed (char* other = "Other")
        {
            var rgszNames = new PWSTR[] { width, other };
            int[] rgDispId = new int[rgszNames.Length];
            fixed (PWSTR* pRgszNames = rgszNames)
            fixed (int* pRgDispId = rgDispId)
            {
                typeInfo.Value->GetIDsOfNames(pRgszNames, (uint)rgszNames.Length, pRgDispId);
                Assert.Equal([width, other], rgszNames);
                Assert.Equal([(int)PInvokeCore.DISPID_PICT_WIDTH, PInvokeCore.DISPID_UNKNOWN], rgDispId);
            }
        }
    }

    [StaFact]
    public unsafe void ITypeInfo_GetImplTypeFlags_Invoke_Success()
    {
        using Bitmap image = new(16, 32);
        using var iPictureDisp = image.CreateIPictureDisp();
        Assert.False(iPictureDisp.IsNull);
        using ComScope<ITypeInfo> typeInfo = new(null);
        ((IDispatch*)iPictureDisp.Value)->GetTypeInfo(0, PInvokeCore.GetThreadLocale(), typeInfo);

        IMPLTYPEFLAGS implTypeFlags = (IMPLTYPEFLAGS)(-1);
        typeInfo.Value->GetImplTypeFlags(0, &implTypeFlags);
        Assert.NotEqual(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT, implTypeFlags);
    }

    [StaFact]
    public unsafe void ITypeInfo_GetMops_Invoke_Success()
    {
        using Bitmap image = new(16, 32);
        using var iPictureDisp = image.CreateIPictureDisp();
        Assert.False(iPictureDisp.IsNull);
        using ComScope<ITypeInfo> typeInfo = new(null);
        ((IDispatch*)iPictureDisp.Value)->GetTypeInfo(0, PInvokeCore.GetThreadLocale(), typeInfo);

        using BSTR mops = new("Mops");
        typeInfo.Value->GetMops(4, &mops);
        Assert.Equal(0, mops.Length);
    }

    [StaFact]
    public unsafe void ITypeInfo_GetNames_Invoke_Success()
    {
        using Bitmap image = new(16, 32);
        using var iPictureDisp = image.CreateIPictureDisp();
        Assert.False(iPictureDisp.IsNull);
        using ComScope<ITypeInfo> typeInfo = new(null);
        ((IDispatch*)iPictureDisp.Value)->GetTypeInfo(0, PInvokeCore.GetThreadLocale(), typeInfo);

        BSTR* rgszNames = stackalloc BSTR[2];
        rgszNames[0] = new BSTR("Name1");
        rgszNames[1] = new BSTR("Name2");
        uint cNames = 0;
        typeInfo.Value->GetNames(4, rgszNames, 2u, &cNames);
        Assert.Equal("Width", rgszNames[0].ToString());
        Assert.Equal("Name2", rgszNames[1].ToString());
        Assert.Equal(1u, cNames);

        rgszNames[0].Dispose();
        rgszNames[1].Dispose();
    }

    [StaFact]
    public unsafe void ITypeInfo_GetRefTypeInfo_Invoke_Success()
    {
        using Bitmap image = new(16, 32);
        using var iPictureDisp = image.CreateIPictureDisp();
        Assert.False(iPictureDisp.IsNull);
        using ComScope<ITypeInfo> typeInfo = new(null);
        ((IDispatch*)iPictureDisp.Value)->GetTypeInfo(0, PInvokeCore.GetThreadLocale(), typeInfo);

        uint refType = uint.MaxValue;
        typeInfo.Value->GetRefTypeOfImplType(0, &refType);
        Assert.NotEqual(0u, refType);

        using ComScope<ITypeInfo> refTypeInfo = new(null);
        typeInfo.Value->GetRefTypeInfo(refType, refTypeInfo);
    }

    [StaFact]
    public unsafe void ITypeInfo_GetRefTypeOfImplType_Invoke_Success()
    {
        using Bitmap image = new(16, 32);
        using var iPictureDisp = image.CreateIPictureDisp();
        Assert.False(iPictureDisp.IsNull);
        using ComScope<ITypeInfo> typeInfo = new(null);
        ((IDispatch*)iPictureDisp.Value)->GetTypeInfo(0, PInvokeCore.GetThreadLocale(), typeInfo);

        uint refType = uint.MaxValue;
        typeInfo.Value->GetRefTypeOfImplType(0, &refType);
        Assert.NotEqual(0u, refType);
    }

    [StaFact]
    public unsafe void ITypeInfo_GetTypeAttr_Invoke_Success()
    {
        using Bitmap image = new(16, 32);
        using var iPictureDisp = image.CreateIPictureDisp();
        Assert.False(iPictureDisp.IsNull);
        using ComScope<ITypeInfo> typeInfo = new(null);
        ((IDispatch*)iPictureDisp.Value)->GetTypeInfo(0, PInvokeCore.GetThreadLocale(), typeInfo);

        TYPEATTR* pTypeAttr = null;
        try
        {
            typeInfo.Value->GetTypeAttr(&pTypeAttr);
            Assert.Equal(typeof(IPictureDisp).GUID, pTypeAttr->guid);
            Assert.Equal(0u, pTypeAttr->lcid);
            Assert.Equal(0u, pTypeAttr->dwReserved);
            Assert.Equal(PInvokeCore.DISPID_UNKNOWN, pTypeAttr->memidConstructor);
            Assert.Equal(PInvokeCore.DISPID_UNKNOWN, pTypeAttr->memidDestructor);
            Assert.True(pTypeAttr->lpstrSchema.IsNull);
            Assert.Equal((uint)IntPtr.Size, pTypeAttr->cbSizeInstance);
            Assert.Equal(TYPEKIND.TKIND_DISPATCH, pTypeAttr->typekind);
            Assert.Equal(1, pTypeAttr->cFuncs);
            Assert.Equal(5, pTypeAttr->cVars);
            Assert.Equal(1, pTypeAttr->cImplTypes);
            Assert.Equal(7 * IntPtr.Size, pTypeAttr->cbSizeVft);
            Assert.Equal((ushort)IntPtr.Size, pTypeAttr->cbAlignment);
            Assert.Equal(0x1000, pTypeAttr->wTypeFlags);
            Assert.Equal(0, pTypeAttr->wMajorVerNum);
            Assert.Equal(0, pTypeAttr->wMinorVerNum);
            Assert.Equal(VARENUM.VT_EMPTY, pTypeAttr->tdescAlias.vt);
            Assert.Equal((nuint)0, pTypeAttr->idldescType.dwReserved);
            Assert.Equal(IDLFLAGS.IDLFLAG_NONE, pTypeAttr->idldescType.wIDLFlags);
        }
        finally
        {
            typeInfo.Value->ReleaseTypeAttr(pTypeAttr);
        }
    }

    [StaFact]
    public unsafe void ITypeInfo_GetTypeComp_Invoke_Success()
    {
        using Bitmap image = new(16, 32);
        using var iPictureDisp = image.CreateIPictureDisp();
        Assert.False(iPictureDisp.IsNull);
        using ComScope<ITypeInfo> typeInfo = new(null);
        ((IDispatch*)iPictureDisp.Value)->GetTypeInfo(0, PInvokeCore.GetThreadLocale(), typeInfo);

        using ComScope<ITypeComp> typeComp = new(null);
        typeInfo.Value->GetTypeComp(typeComp);
    }

    [StaFact]
    public unsafe void ITypeInfo_GetVarDesc_Invoke_Success()
    {
        using Bitmap image = new(16, 32);
        using var iPictureDisp = image.CreateIPictureDisp();
        Assert.False(iPictureDisp.IsNull);
        using ComScope<ITypeInfo> typeInfo = new(null);
        ((IDispatch*)iPictureDisp.Value)->GetTypeInfo(0, PInvokeCore.GetThreadLocale(), typeInfo);

        VARDESC* pVarDesc = null;
        try
        {
            typeInfo.Value->GetVarDesc(3, &pVarDesc);
            Assert.Equal(4, pVarDesc->memid);
            Assert.True(pVarDesc->lpstrSchema.IsNull);
            Assert.True(pVarDesc->Anonymous.lpvarValue is null);
            Assert.Equal(VARENUM.VT_USERDEFINED, pVarDesc->elemdescVar.tdesc.vt);
            Assert.False(pVarDesc->elemdescVar.tdesc.Anonymous.lpadesc is null);
            Assert.True(pVarDesc->elemdescVar.Anonymous.paramdesc.pparamdescex is null);
            Assert.Equal(PARAMFLAGS.PARAMFLAG_NONE, pVarDesc->elemdescVar.Anonymous.paramdesc.wParamFlags);
            Assert.Equal(VARFLAGS.VARFLAG_FREADONLY, pVarDesc->wVarFlags);
            Assert.Equal(VARKIND.VAR_DISPATCH, pVarDesc->varkind);
        }
        finally
        {
            typeInfo.Value->ReleaseVarDesc(pVarDesc);
        }
    }

    [StaFact]
    public unsafe void ITypeInfo_Invoke_Invoke_Success()
    {
        using Bitmap image = new(16, 32);
        using var iPictureDisp = image.CreateIPictureDisp();
        Assert.False(iPictureDisp.IsNull);
        using ComScope<ITypeInfo> typeInfo = new(null);
        ((IDispatch*)iPictureDisp.Value)->GetTypeInfo(0, PInvokeCore.GetThreadLocale(), typeInfo);

        DISPPARAMS dispParams = default;
        VARIANT varResult = default;
        EXCEPINFO excepInfo = default;
        uint argErr = 0;

        HRESULT hr = typeInfo.Value->Invoke(
            iPictureDisp,
            (int)PInvokeCore.DISPID_PICT_WIDTH,
            DISPATCH_FLAGS.DISPATCH_PROPERTYGET,
            &dispParams,
            &varResult,
            &excepInfo,
            &argErr);

        hr.Should().Be(HRESULT.DISP_E_MEMBERNOTFOUND);
        varResult.IsEmpty.Should().BeTrue();
        argErr.Should().Be(0);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Primitives.Tests.Interop.Mocks;
using Xunit;
using Windows.Win32.System.Com;
using IDispatch = Interop.Oleaut32.IDispatch;
using ITypeInfo = Interop.Oleaut32.ITypeInfo;
using IPictureDisp = Interop.Ole32.IPictureDisp;
using DispatchID = Interop.Ole32.DispatchID;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms.Primitives.Tests.Interop.Oleaut32
{
    [Collection("Sequential")]
    public class ITypeInfoTests
    {
        [StaFact]
        public unsafe void ITypeInfo_AddressOfMember_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            IPictureDisp picture = MockAxHost.GetIPictureDispFromPicture(image);
            IDispatch dispatch = (IDispatch)picture;
            ITypeInfo typeInfo;
            HRESULT hr = dispatch.GetTypeInfo(0, PInvoke.GetThreadLocale(), out typeInfo);
            using var typeInfoReleaser = new ComRefReleaser(typeInfo);
            Assert.Equal(HRESULT.S_OK, hr);

            nint pv = int.MaxValue;
            hr = typeInfo.AddressOfMember((DispatchID)6, INVOKEKIND.INVOKE_FUNC, &pv);
            Assert.Equal(HRESULT.TYPE_E_BADMODULEKIND, hr);
            Assert.Equal(0, pv);
        }

        [StaFact]
        public unsafe void ITypeInfo_CreateInstance_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            IPictureDisp picture = MockAxHost.GetIPictureDispFromPicture(image);
            IDispatch dispatch = (IDispatch)picture;
            ITypeInfo typeInfo;
            HRESULT hr = dispatch.GetTypeInfo(0, PInvoke.GetThreadLocale(), out typeInfo);
            using var typeInfoReleaser = new ComRefReleaser(typeInfo);
            Assert.Equal(HRESULT.S_OK, hr);

            Guid riid = typeof(IPictureDisp).GUID;
            IntPtr pvObj = (IntPtr)int.MaxValue;
            hr = typeInfo.CreateInstance(IntPtr.Zero, &riid, &pvObj);
            Assert.Equal(HRESULT.TYPE_E_BADMODULEKIND, hr);
            Assert.Equal(IntPtr.Zero, pvObj);
        }

        [StaFact]
        public unsafe void ITypeInfo_GetContainingTypeLib_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            IPictureDisp picture = MockAxHost.GetIPictureDispFromPicture(image);
            IDispatch dispatch = (IDispatch)picture;
            ITypeInfo typeInfo;
            HRESULT hr = dispatch.GetTypeInfo(0, PInvoke.GetThreadLocale(), out typeInfo);
            using var typeInfoReleaser = new ComRefReleaser(typeInfo);
            Assert.Equal(HRESULT.S_OK, hr);

            nint typeLib = (nint)int.MaxValue;
            uint index = uint.MaxValue;
            hr = typeInfo.GetContainingTypeLib(&typeLib, &index);
            try
            {
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.NotEqual(0, typeLib);
                Assert.NotEqual(0u, index);
            }
            finally
            {
                Marshal.Release(typeLib);
            }
        }

        [StaFact]
        public unsafe void ITypeInfo_GetDllEntry_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            IPictureDisp picture = MockAxHost.GetIPictureDispFromPicture(image);
            IDispatch dispatch = (IDispatch)picture;
            ITypeInfo typeInfo;
            HRESULT hr = dispatch.GetTypeInfo(0, PInvoke.GetThreadLocale(), out typeInfo);
            using var typeInfoReleaser = new ComRefReleaser(typeInfo);
            Assert.Equal(HRESULT.S_OK, hr);

            var dllName = new BSTR("DllName");
            var name = new BSTR("Name");
            ushort wOrdinal = ushort.MaxValue;
            hr = typeInfo.GetDllEntry((DispatchID)6, INVOKEKIND.INVOKE_FUNC, &dllName, &name, &wOrdinal);
            Assert.Equal(HRESULT.TYPE_E_BADMODULEKIND, hr);
            Assert.True(dllName.Length == 0);
            Assert.True(name.Length == 0);
            Assert.Equal(0u, wOrdinal);
        }

        [StaFact]
        public unsafe void ITypeInfo_GetDocumentation_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            IPictureDisp picture = MockAxHost.GetIPictureDispFromPicture(image);
            IDispatch dispatch = (IDispatch)picture;
            ITypeInfo typeInfo;
            HRESULT hr = dispatch.GetTypeInfo(0, PInvoke.GetThreadLocale(), out typeInfo);
            using var typeInfoReleaser = new ComRefReleaser(typeInfo);
            Assert.Equal(HRESULT.S_OK, hr);

            using BSTR name = new("Name");
            using BSTR docString = new("DocString");
            uint dwHelpContext = uint.MaxValue;
            using BSTR helpFile = new("HelpFile");
            hr = typeInfo.GetDocumentation((DispatchID)4, &name, &docString, &dwHelpContext, &helpFile);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal("Width", name.ToString());
            Assert.True(docString.Length == 0);
            Assert.Equal(0u, dwHelpContext);
            Assert.True(helpFile.Length == 0);
        }

        [StaFact]
        public unsafe void ITypeInfo_GetFuncDesc_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            IPictureDisp picture = MockAxHost.GetIPictureDispFromPicture(image);
            IDispatch dispatch = (IDispatch)picture;
            ITypeInfo typeInfo;
            HRESULT hr = dispatch.GetTypeInfo(0, PInvoke.GetThreadLocale(), out typeInfo);
            using var typeInfoReleaser = new ComRefReleaser(typeInfo);
            Assert.Equal(HRESULT.S_OK, hr);

            global::Windows.Win32.System.Com.FUNCDESC* pFuncDesc = null;
            try
            {
                hr = typeInfo.GetFuncDesc(0, &pFuncDesc);
                Assert.Equal(HRESULT.S_OK, hr);
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
                typeInfo.ReleaseFuncDesc(pFuncDesc);
            }
        }

        [StaFact]
        public unsafe void ITypeInfo_GetIDsOfNames_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            IPictureDisp picture = MockAxHost.GetIPictureDispFromPicture(image);
            IDispatch dispatch = (IDispatch)picture;
            ITypeInfo typeInfo;
            HRESULT hr = dispatch.GetTypeInfo(0, PInvoke.GetThreadLocale(), out typeInfo);
            using var typeInfoReleaser = new ComRefReleaser(typeInfo);
            Assert.Equal(HRESULT.S_OK, hr);

            var rgszNames = new string[] { "Width", "Other" };
            var rgDispId = new DispatchID[rgszNames.Length];
            fixed (DispatchID* pRgDispId = rgDispId)
            {
                hr = typeInfo.GetIDsOfNames(rgszNames, (uint)rgszNames.Length, pRgDispId);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(new string[] { "Width", "Other" }, rgszNames);
                Assert.Equal(new DispatchID[] { (DispatchID)4, DispatchID.UNKNOWN }, rgDispId);
            }
        }

        [StaFact]
        public unsafe void ITypeInfo_GetImplTypeFlags_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            IPictureDisp picture = MockAxHost.GetIPictureDispFromPicture(image);
            IDispatch dispatch = (IDispatch)picture;
            ITypeInfo typeInfo;
            HRESULT hr = dispatch.GetTypeInfo(0, PInvoke.GetThreadLocale(), out typeInfo);
            using var typeInfoReleaser = new ComRefReleaser(typeInfo);
            Assert.Equal(HRESULT.S_OK, hr);

            IMPLTYPEFLAGS implTypeFlags = (IMPLTYPEFLAGS)(-1);
            hr = typeInfo.GetImplTypeFlags(0, &implTypeFlags);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.NotEqual(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT, implTypeFlags);
        }

        [StaFact]
        public unsafe void ITypeInfo_GetMops_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            IPictureDisp picture = MockAxHost.GetIPictureDispFromPicture(image);
            IDispatch dispatch = (IDispatch)picture;
            ITypeInfo typeInfo;
            HRESULT hr = dispatch.GetTypeInfo(0, PInvoke.GetThreadLocale(), out typeInfo);
            using var typeInfoReleaser = new ComRefReleaser(typeInfo);
            Assert.Equal(HRESULT.S_OK, hr);

            var mops = new BSTR("Mops");
            hr = typeInfo.GetMops((DispatchID)4, &mops);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.True(mops.Length == 0);
        }

        [StaFact]
        public unsafe void ITypeInfo_GetNames_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            IPictureDisp picture = MockAxHost.GetIPictureDispFromPicture(image);
            IDispatch dispatch = (IDispatch)picture;
            ITypeInfo typeInfo;
            HRESULT hr = dispatch.GetTypeInfo(0, PInvoke.GetThreadLocale(), out typeInfo);
            using var typeInfoReleaser = new ComRefReleaser(typeInfo);
            Assert.Equal(HRESULT.S_OK, hr);

            BSTR* rgszNames = stackalloc BSTR[2];
            rgszNames[0] = new BSTR("Name1");
            rgszNames[1] = new BSTR("Name2");
            uint cNames = 0;
            hr = typeInfo.GetNames((DispatchID)4, rgszNames, 2u, &cNames);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal("Width", rgszNames[0].ToString());
            Assert.Equal("Name2", rgszNames[1].ToString());
            Assert.Equal(1u, cNames);

            rgszNames[0].Dispose();
            rgszNames[1].Dispose();
        }

        [StaFact]
        public unsafe void ITypeInfo_GetRefTypeInfo_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            IPictureDisp picture = MockAxHost.GetIPictureDispFromPicture(image);
            IDispatch dispatch = (IDispatch)picture;
            ITypeInfo typeInfo;
            HRESULT hr = dispatch.GetTypeInfo(0, PInvoke.GetThreadLocale(), out typeInfo);
            using var typeInfoReleaser = new ComRefReleaser(typeInfo);
            Assert.Equal(HRESULT.S_OK, hr);

            uint refType = uint.MaxValue;
            hr = typeInfo.GetRefTypeOfImplType(0, &refType);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.NotEqual(0u, refType);

            ITypeInfo refTypeInfo;
            hr = typeInfo.GetRefTypeInfo(refType, out refTypeInfo);
            using var refTypeInfoReleaser = new ComRefReleaser(refTypeInfo);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.NotNull(refTypeInfo);
        }

        [StaFact]
        public unsafe void ITypeInfo_GetRefTypeOfImplType_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            IPictureDisp picture = MockAxHost.GetIPictureDispFromPicture(image);
            IDispatch dispatch = (IDispatch)picture;
            ITypeInfo typeInfo;
            HRESULT hr = dispatch.GetTypeInfo(0, PInvoke.GetThreadLocale(), out typeInfo);
            using var typeInfoReleaser = new ComRefReleaser(typeInfo);
            Assert.Equal(HRESULT.S_OK, hr);

            uint refType = uint.MaxValue;
            hr = typeInfo.GetRefTypeOfImplType(0, &refType);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.NotEqual(0u, refType);
        }

        [StaFact]
        public unsafe void ITypeInfo_GetTypeAttr_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            IPictureDisp picture = MockAxHost.GetIPictureDispFromPicture(image);
            IDispatch dispatch = (IDispatch)picture;
            ITypeInfo typeInfo;
            HRESULT hr = dispatch.GetTypeInfo(0, PInvoke.GetThreadLocale(), out typeInfo);
            using var typeInfoReleaser = new ComRefReleaser(typeInfo);
            Assert.Equal(HRESULT.S_OK, hr);

            TYPEATTR* pTypeAttr = null;
            try
            {
                hr = typeInfo.GetTypeAttr(&pTypeAttr);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(typeof(IPictureDisp).GUID, pTypeAttr->guid);
                Assert.Equal(0u, pTypeAttr->lcid);
                Assert.Equal(0u, pTypeAttr->dwReserved);
                Assert.Equal((int)DispatchID.UNKNOWN, pTypeAttr->memidConstructor);
                Assert.Equal((int)DispatchID.UNKNOWN, pTypeAttr->memidDestructor);
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
                typeInfo.ReleaseTypeAttr(pTypeAttr);
            }
        }

        [StaFact]
        public unsafe void ITypeInfo_GetTypeComp_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            IPictureDisp picture = MockAxHost.GetIPictureDispFromPicture(image);
            IDispatch dispatch = (IDispatch)picture;
            ITypeInfo typeInfo;
            HRESULT hr = dispatch.GetTypeInfo(0, PInvoke.GetThreadLocale(), out typeInfo);
            using var typeInfoReleaser = new ComRefReleaser(typeInfo);
            Assert.Equal(HRESULT.S_OK, hr);

            IntPtr typeComp = IntPtr.Zero;
            hr = typeInfo.GetTypeComp(&typeComp);
            try
            {
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.NotEqual(IntPtr.Zero, typeComp);
            }
            finally
            {
                Runtime.InteropServices.Marshal.Release(typeComp);
            }
        }

        [StaFact]
        public unsafe void ITypeInfo_GetVarDesc_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            IPictureDisp picture = MockAxHost.GetIPictureDispFromPicture(image);
            IDispatch dispatch = (IDispatch)picture;
            ITypeInfo typeInfo;
            HRESULT hr = dispatch.GetTypeInfo(0, PInvoke.GetThreadLocale(), out typeInfo);
            using var typeInfoReleaser = new ComRefReleaser(typeInfo);
            Assert.Equal(HRESULT.S_OK, hr);

            VARDESC* pVarDesc = null;
            try
            {
                hr = typeInfo.GetVarDesc(3, &pVarDesc);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(4, pVarDesc->memid);
                Assert.True(pVarDesc->lpstrSchema.IsNull);
                Assert.True(pVarDesc->Anonymous.lpvarValue  is null);
                Assert.Equal(VARENUM.VT_USERDEFINED, pVarDesc->elemdescVar.tdesc.vt);
                Assert.False(pVarDesc->elemdescVar.tdesc.Anonymous.lpadesc is null);
                Assert.True(pVarDesc->elemdescVar.Anonymous.paramdesc.pparamdescex is null);
                Assert.Equal(PARAMFLAGS.PARAMFLAG_NONE, pVarDesc->elemdescVar.Anonymous.paramdesc.wParamFlags);
                Assert.Equal(VARFLAGS.VARFLAG_FREADONLY, pVarDesc->wVarFlags);
                Assert.Equal(VARKIND.VAR_DISPATCH, pVarDesc->varkind);
            }
            finally
            {
                typeInfo.ReleaseVarDesc(pVarDesc);
            }
        }

        [StaFact]
        public unsafe void ITypeInfo_Invoke_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            IPictureDisp picture = MockAxHost.GetIPictureDispFromPicture(image);
            IDispatch dispatch = (IDispatch)picture;
            ITypeInfo typeInfo;
            HRESULT hr = dispatch.GetTypeInfo(0, PInvoke.GetThreadLocale(), out typeInfo);
            using var typeInfoReleaser = new ComRefReleaser(typeInfo);
            Assert.Equal(HRESULT.S_OK, hr);

            var dispParams = new DISPPARAMS();
            var varResult = new object[1];
            var excepInfo = new EXCEPINFO();
            uint argErr = 0;
            hr = typeInfo.Invoke(
                picture,
                (DispatchID)4,
                DISPATCH_FLAGS.DISPATCH_PROPERTYGET,
                &dispParams,
                varResult,
                &excepInfo,
                &argErr);
            Assert.Equal(HRESULT.DISP_E_MEMBERNOTFOUND, hr);
            Assert.Null(varResult[0]);
            Assert.Equal(0u, argErr);
        }

        // ITypeInfo often requires manual RCW reference management. The native object may be free threaded
        // but when created on an STA thread it will be associated with that thread. If the native code keeps
        // reusing the same instance you can run into a condition where the GC cleans up the RCW from one STA
        // thread while you want to start using the same underlying object on a new STA thread. This will lead
        // to an error so manual release is required to avoid running into this condition.
        private struct ComRefReleaser : IDisposable
        {
            private object? _reference;

            public ComRefReleaser(object? reference)
            {
                _reference = reference;
            }

            public void Dispose()
            {
                if (_reference != null)
                {
                    Marshal.ReleaseComObject(_reference);
                    _reference = null;
                }
            }
        }
    }
}

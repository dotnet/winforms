// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.Primitives.Tests.Interop.Mocks;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Xunit;

namespace System.Windows.Forms.Primitives.Tests.Interop.Oleaut32
{
    [Collection("Sequential")]
    public partial class IDispatchTests
    {
        [StaFact]
        public unsafe void IDispatch_GetIDsOfNames_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            ComHelpers.QueryInterface(MockAxHost.GetIPictureDispFromPicture(image), out IPictureDisp* picture).ThrowOnFailure();

            Guid riid = Guid.Empty;
            fixed (char* width = "Width")
            fixed (char* other = "Other")
            {
                var rgszNames = new PWSTR[] { width, other };
                var rgDispId = new int[rgszNames.Length];
                fixed (int* pRgDispId = rgDispId)
                fixed (PWSTR* pRgszNames = rgszNames)
                {
                    HRESULT hr = picture->GetIDsOfNames(&riid, pRgszNames, (uint)rgszNames.Length, PInvoke.GetThreadLocale(), pRgDispId);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(new PWSTR[] { width, other }, rgszNames);
                    
                    Assert.Equal(new int[] { (int)PInvoke.DISPID_PICT_WIDTH, PInvoke.DISPID_UNKNOWN }, rgDispId);
                }
            }
        }

        [StaFact]
        public unsafe void IDispatch_GetTypeInfo_Invoke_Success()
        {
            using var image = new Bitmap(16, 16);
            ComHelpers.QueryInterface(MockAxHost.GetIPictureDispFromPicture(image), out IPictureDisp* picture).ThrowOnFailure();

            using ComScope<ITypeInfo> typeInfo = new(null);
            HRESULT hr = picture->GetTypeInfo(0, PInvoke.GetThreadLocale(), typeInfo);
            Assert.Equal(HRESULT.S_OK, hr);
        }

        [StaFact]
        public unsafe void IDispatch_GetTypeInfoCount_Invoke_Success()
        {
            using var image = new Bitmap(16, 16);
            ComHelpers.QueryInterface(MockAxHost.GetIPictureDispFromPicture(image), out IPictureDisp* picture).ThrowOnFailure();

            uint ctInfo = uint.MaxValue;
            HRESULT hr = picture->GetTypeInfoCount(&ctInfo);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(1u, ctInfo);
        }

        [StaFact]
        public unsafe void IDispatch_Invoke_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            ComHelpers.QueryInterface(MockAxHost.GetIPictureDispFromPicture(image), out IPictureDisp* picture).ThrowOnFailure();

            var varResult = new VARIANT();
            var excepInfo = new EXCEPINFO();
            uint argErr = 0;
            HRESULT hr = ComHelpers.InvokePictureDisp(
                picture,
                PInvoke.DISPID_PICT_WIDTH,
                &varResult,
                PInvoke.GetThreadLocale(),
                &excepInfo,
                &argErr);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(VARENUM.VT_I4, varResult.vt);
            Assert.Equal(16, GdiHelper.HimetricToPixelY(varResult.data.intVal));
            Assert.Equal(0u, argErr);
        }
    }
}

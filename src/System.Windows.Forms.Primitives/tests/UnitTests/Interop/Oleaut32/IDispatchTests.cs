// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms.Primitives.Tests.Interop.Oleaut32
{
    [Collection("Sequential")]
    public partial class IDispatchTests
    {
        [StaFact]
        public unsafe void IDispatch_GetIDsOfNames_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            using var picture = IPictureDisp.CreateFromImage(image);
            Assert.False(picture.IsNull);

            Guid riid = Guid.Empty;
            fixed (char* width = "Width")
            fixed (char* other = "Other")
            {
                var rgszNames = new PWSTR[] { width, other };
                var rgDispId = new int[rgszNames.Length];
                fixed (int* pRgDispId = rgDispId)
                fixed (PWSTR* pRgszNames = rgszNames)
                {
                    HRESULT hr = picture.Value->GetIDsOfNames(&riid, pRgszNames, (uint)rgszNames.Length, PInvoke.GetThreadLocale(), pRgDispId);
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
            using var picture = IPictureDisp.CreateFromImage(image);
            Assert.False(picture.IsNull);

            using ComScope<ITypeInfo> typeInfo = new(null);
            HRESULT hr = picture.Value->GetTypeInfo(0, PInvoke.GetThreadLocale(), typeInfo);
            Assert.Equal(HRESULT.S_OK, hr);
        }

        [StaFact]
        public unsafe void IDispatch_GetTypeInfoCount_Invoke_Success()
        {
            using var image = new Bitmap(16, 16);
            using var picture = IPictureDisp.CreateFromImage(image);
            Assert.False(picture.IsNull);

            uint ctInfo = uint.MaxValue;
            HRESULT hr = picture.Value->GetTypeInfoCount(&ctInfo);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(1u, ctInfo);
        }

        [StaFact]
        public unsafe void IDispatch_Invoke_Invoke_Success()
        {
            using var image = new Bitmap(16, 32);
            using var picture = IPictureDisp.CreateFromImage(image);
            Assert.False(picture.IsNull);

            using VARIANT varResult = default;
            HRESULT hr = ((IDispatch*)picture.Value)->GetProperty(
                PInvoke.DISPID_PICT_WIDTH,
                &varResult,
                PInvoke.GetThreadLocale());
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(VARENUM.VT_I4, varResult.vt);
            Assert.Equal(16, GdiHelper.HimetricToPixelY(varResult.data.intVal));
        }
    }
}

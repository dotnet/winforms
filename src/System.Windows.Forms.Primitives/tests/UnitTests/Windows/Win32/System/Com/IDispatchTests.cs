// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.Graphics.GdiPlus;
using Windows.Win32.System.Variant;

namespace Windows.Win32.System.Com.Tests;

[Collection("Sequential")]
public partial class IDispatchTests
{
    [StaFact]
    public unsafe void IDispatch_GetIDsOfNames_Invoke_Success()
    {
        using Bitmap image = new(16, 32);
        using var picture = image.CreateIPictureDisp();
        Assert.False(picture.IsNull);

        Guid riid = Guid.Empty;
        fixed (char* width = "Width")
        fixed (char* other = "Other")
        {
            var rgszNames = new PWSTR[] { width, other };
            int[] rgDispId = new int[rgszNames.Length];
            fixed (int* pRgDispId = rgDispId)
            fixed (PWSTR* pRgszNames = rgszNames)
            {
                picture.Value->GetIDsOfNames(&riid, pRgszNames, (uint)rgszNames.Length, PInvokeCore.GetThreadLocale(), pRgDispId);
                Assert.Equal([width, other], rgszNames);

                Assert.Equal([(int)PInvokeCore.DISPID_PICT_WIDTH, PInvokeCore.DISPID_UNKNOWN], rgDispId);
            }
        }
    }

    [StaFact]
    public unsafe void IDispatch_GetTypeInfo_Invoke_Success()
    {
        using Bitmap image = new(16, 16);
        using var picture = image.CreateIPictureDisp();
        Assert.False(picture.IsNull);

        using ComScope<ITypeInfo> typeInfo = new(null);
        picture.Value->GetTypeInfo(0, PInvokeCore.GetThreadLocale(), typeInfo);
    }

    [StaFact]
    public unsafe void IDispatch_GetTypeInfoCount_Invoke_Success()
    {
        using Bitmap image = new(16, 16);
        using var picture = image.CreateIPictureDisp();
        Assert.False(picture.IsNull);

        uint ctInfo = uint.MaxValue;
        picture.Value->GetTypeInfoCount(&ctInfo);
        Assert.Equal(1u, ctInfo);
    }

    [StaFact]
    public unsafe void IDispatch_Invoke_Invoke_Success()
    {
        using Bitmap image = new(16, 32);
        using var picture = image.CreateIPictureDisp();
        Assert.False(picture.IsNull);

        using VARIANT varResult = default;
        HRESULT hr = ((IDispatch*)picture.Value)->TryGetProperty(
            PInvokeCore.DISPID_PICT_WIDTH,
            &varResult,
            PInvokeCore.GetThreadLocale());
        Assert.Equal(HRESULT.S_OK, hr);
        Assert.Equal(VARENUM.VT_I4, varResult.vt);
        Assert.Equal(16, GdiHelper.HimetricToPixelY(varResult.data.intVal));
    }
}

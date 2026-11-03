// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

using DataFormats = System.Private.Windows.Ole.DataFormatsCore<System.Private.Windows.Ole.TestFormat>;
using DataObject = System.Private.Windows.Ole.TestDataObject<System.Private.Windows.Ole.MockOleServices<System.Private.Windows.Ole.ManagedToNativeAdapterTests>>;
using OleServices = System.Private.Windows.Ole.MockOleServices<System.Private.Windows.Ole.ManagedToNativeAdapterTests>;

namespace System.Private.Windows.Ole;

public unsafe class ManagedToNativeAdapterTests
{
    private const TYMED DefaultTymeds = TYMED.TYMED_HGLOBAL | TYMED.TYMED_ISTREAM | TYMED.TYMED_GDI;

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ManagedToNativeAdapter_QueryGetData_EnhancedMetafile_UsesPlatformAllowedTymeds(bool platformAllowsEmf)
    {
        OleServices.SupportedTymeds = platformAllowsEmf ? DefaultTymeds | TYMED.TYMED_ENHMF : DefaultTymeds;

        try
        {
            DataObject dataObject = new(DataFormatNames.Emf, new MemoryStream([1, 2, 3]));

            FORMATETC formatEtc = new()
            {
                cfFormat = (ushort)DataFormats.GetOrAddFormat(DataFormatNames.Emf).Id,
                dwAspect = (uint)DVASPECT.DVASPECT_CONTENT,
                lindex = -1,
                tymed = (uint)TYMED.TYMED_ENHMF
            };

            // CF_ENHMETAFILE is always requested as TYMED_ENHMF, so it is only available when the platform allows it.
            dataObject.QueryGetData(&formatEtc).Should().Be(platformAllowsEmf ? HRESULT.S_OK : HRESULT.DV_E_TYMED);
        }
        finally
        {
            OleServices.SupportedTymeds = DefaultTymeds;
        }
    }
}

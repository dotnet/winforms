// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class PropertyGrid_IErrorInfoSupportTests : IClassFixture<ThreadExceptionFixture>
    {
        public const int DISP_E_MEMBERNOTFOUND = unchecked((int)0x80020003);

        [WinFormsFact]
        public void ISupportErrorInfo_Supported_ButNoIErrorInfoGiven()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            propertyGrid.SelectedObject = CreateComObjectWithIErrorInfo();
            var entries = propertyGrid.GetCurrentEntries();
            var encodingEntry = entries[0].Children.First(_ => _.PropertyName == "encoding");
            try
            {
                encodingEntry.SetPropertyTextValue("nonexisting");
                Assert.False(true);
            }
            catch (ExternalException ex)
            {
                Assert.Equal(DISP_E_MEMBERNOTFOUND, ex.HResult);
            }
        }

        private object CreateComObjectWithIErrorInfo()
        {
            var CLSID_MXXMLWriter60 = new Guid("88d96a0f-f192-11d4-a65f-0040963251e5");
            var IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");
            CoCreateInstance(ref CLSID_MXXMLWriter60,
                IntPtr.Zero,
                1,
                ref IID_IUnknown,
                out object result);
            return result;
        }

        [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
        private static extern void CoCreateInstance(
            ref Guid rclsid,
            IntPtr punkOuter,
            int dwClsContext,
            ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppv);
    }
}

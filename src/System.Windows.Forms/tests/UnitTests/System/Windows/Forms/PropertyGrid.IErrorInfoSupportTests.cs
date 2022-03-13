// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;

namespace System.Windows.Forms.Tests
{
    [Collection("Sequential")]
    public class PropertyGrid_IErrorInfoSupportTests
    {
        public const int DISP_E_MEMBERNOTFOUND = unchecked((int)0x80020003);
        public const string System_Windows_Forms_NativeTests = "System_Windows_Forms_NativeTests";

        [WinFormsFact]
        public void ISupportErrorInfo_Supported_ButNoIErrorInfoGiven()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            Create_Standard_IErrorInfo_UsageObject(out var target);
            propertyGrid.SelectedObject = target;
            var entries = propertyGrid.GetCurrentEntries();
            var encodingEntry = entries[0].Children.First(_ => _.PropertyName == "Int_Property");
            try
            {
                encodingEntry.SetPropertyTextValue("333");
                Assert.False(true);
            }
            catch (ExternalException ex)
            {
                Assert.Equal(DISP_E_MEMBERNOTFOUND, ex.HResult);
            }
            finally
            {
                propertyGrid.SelectedObject = null;
                Marshal.ReleaseComObject(target);
            }
        }

        [WinFormsFact]
        public void ISupportErrorInfo_Supported_WithIErrorInfoGiven()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            Create_Raw_IErrorInfo_UsageObject(out var target);
            propertyGrid.SelectedObject = target;
            var entries = propertyGrid.GetCurrentEntries();
            var encodingEntry = entries[0].Children.First(_ => _.PropertyName == "Int_Property");
            try
            {
                encodingEntry.SetPropertyTextValue("123");
                Assert.False(true);
            }
            catch (ExternalException ex)
            {
                Assert.Equal("Error From IErrorInfo", ex.Message);
            }
            finally
            {
                propertyGrid.SelectedObject = null;
                Marshal.ReleaseComObject(target);
            }
        }

        [DllImport(System_Windows_Forms_NativeTests, CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern void Create_Raw_IErrorInfo_UsageObject([MarshalAs(UnmanagedType.Interface)] out object pDisp);

        [DllImport(System_Windows_Forms_NativeTests, CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern void Create_Standard_IErrorInfo_UsageObject([MarshalAs(UnmanagedType.IUnknown)] out object pDisp);
    }
}

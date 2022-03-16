// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Windows.Forms.Tests
{
    [Collection("Sequential")]
    public class PropertyGrid_IErrorInfoSupportTests
    {
        public const int DISP_E_MEMBERNOTFOUND = unchecked((int)0x80020003);
        public const string System_Windows_Forms_NativeTests = "System_Windows_Forms_NativeTests";

        [Fact]
        public void ISupportErrorInfo_Supported_ButNoIErrorInfoGiven()
        {
            ExecuteWithActivationContext(
                "App.manifest",
                () =>
                {
                    using PropertyGrid propertyGrid = new PropertyGrid();
                    var target = CreateComObjectWithStandardIErrorInfoUsage();
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
                });
        }

        [Fact]
        public void ISupportErrorInfo_Supported_WithIErrorInfoGiven()
        {
            ExecuteWithActivationContext(
                "App.manifest",
                () =>
                {
                    using PropertyGrid propertyGrid = new PropertyGrid();
                    var target = CreateComObjectWithRawIErrorInfoUsage();
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
                });
        }

        private void ExecuteWithActivationContext(string applicationManifest, Action action)
        {
            ACTCTXW context = new ACTCTXW();
            context.cbSize = Marshal.SizeOf<ACTCTXW>();
            context.lpSource = applicationManifest;

            var handle = CreateActCtxW(in context);
            if (handle == IntPtr.Zero)
                throw new Win32Exception();
            try
            {
                if (!ActivateActCtx(handle, out var cookie))
                    throw new Win32Exception();
                try
                {
                    action();
                }
                finally
                {
                    if (!DeactivateActCtx(0, cookie))
                        throw new Win32Exception();
                }
            }
            finally
            {
                ReleaseActCtx(handle);
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct ACTCTXW
        {
            public int cbSize; // ULONG
            public int dwFlags; // DWORD
            public string lpSource; // LPCWSTR
            public short wProcessorArchitecture; // USHORT
            public short wLangId; // LANGID
            public string lpAssemblyDirectory; // LPCWSTR
            public string lpResourceName; // LPCWSTR
            public string lpApplicationName; // LPCWSTR
            public IntPtr hModule; // HMODULE
        }

        private object CreateComObjectWithRawIErrorInfoUsage()
        {
            var clsid = new Guid("0ED8EE0D-22E3-49EA-850C-E69B20D1F296");
            var IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");
            CoCreateInstance(ref clsid,
                IntPtr.Zero,
                1,
                ref IID_IUnknown,
                out object result);
            return result;
        }

        private object CreateComObjectWithStandardIErrorInfoUsage()
        {
            var clsid = new Guid("EA1FCB3A-277C-4C79-AB85-E2ED3E858201");
            var IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");
            CoCreateInstance(ref clsid,
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

        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr CreateActCtxW(in ACTCTXW pActCtx);

        [DllImport("kernel32", SetLastError = true)]
        private static extern void ReleaseActCtx(IntPtr hActCtx);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool ActivateActCtx(IntPtr hActCtx, out IntPtr lpCookie);

        [DllImport("kernel32")]
        private static extern bool DeactivateActCtx(int dwFlags, IntPtr ulCookie);
    }
}

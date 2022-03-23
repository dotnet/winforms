// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.InteropServices;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    [Collection("Sequential")]
    public class PropertyGrid_IErrorInfoSupportTests
    {
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
                        Assert.False(true, "Invalid set values should produce ExternalException which will be presenttted to the user.");
                    }
                    catch (ExternalException ex)
                    {
                        // Most default C++ implementation when Invoke return error code
                        // consult IErrorInfo object and populate EXCEPINFO structure
                        // So grid entry knows only about error code.
                        Assert.Equal((int)HRESULT.DISP_E_MEMBERNOTFOUND, ex.HResult);
                        Assert.Equal("Error From StandardErrorInfoUsageTest", ex.Message);
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
                        Assert.False(true, "Invalid set values should produce ExternalException which will be presenttted to the user.");
                    }
                    catch (ExternalException ex)
                    {
                        // If C++ implementation of Invoke did not populate EXCEPINFO structure
                        // from IErrorInfo then we read that information about error call.
                        // and display that error message to the user.
                        Assert.Equal("Error From RawErrorInfoUsageTest", ex.Message);
                    }
                    finally
                    {
                        propertyGrid.SelectedObject = null;
                        Marshal.ReleaseComObject(target);
                    }
                });
        }

        private unsafe void ExecuteWithActivationContext(string applicationManifest, Action action)
        {
            var context = new Kernel32.ACTCTXW();
            IntPtr handle;
            fixed (char* p = applicationManifest)
            {
                context.cbSize = (uint)sizeof(Kernel32.ACTCTXW);
                context.lpSource = p;

                handle = Kernel32.CreateActCtxW(ref context);
            }

            if (handle == IntPtr.Zero)
                throw new Win32Exception();
            try
            {
                if (Kernel32.ActivateActCtx(handle, out var cookie).IsFalse())
                    throw new Win32Exception();
                try
                {
                    action();
                }
                finally
                {
                    if (Kernel32.DeactivateActCtx(0, cookie).IsFalse())
                        throw new Win32Exception();
                }
            }
            finally
            {
                ReleaseActCtx(handle);
            }
        }

        private object CreateComObjectWithRawIErrorInfoUsage()
        {
            Guid clsidRawErrorInfoUsageTest = new("0ED8EE0D-22E3-49EA-850C-E69B20D1F296");
            var IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");
            CoCreateInstance(ref clsidRawErrorInfoUsageTest,
                IntPtr.Zero,
                1,
                ref IID_IUnknown,
                out object result);
            return result;
        }

        private object CreateComObjectWithStandardIErrorInfoUsage()
        {
            Guid clsidStandardErrorInfoUsageTest = new("EA1FCB3A-277C-4C79-AB85-E2ED3E858201");
            var IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");
            CoCreateInstance(ref clsidStandardErrorInfoUsageTest,
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
        private static extern void ReleaseActCtx(IntPtr hActCtx);
    }
}

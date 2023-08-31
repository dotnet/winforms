// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32.System.ApplicationInstallationAndServicing;
using Windows.Win32.System.Com;
using static Interop;

namespace System.Windows.Forms.Interop.Tests;

[Collection("Sequential")]
public class PropertyGridTests
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
                    Assert.Fail("Invalid set values should produce ExternalException which will be presented to the user.");
                }
                catch (ExternalException ex)
                {
                    // Most default C++ implementation when Invoke return error code
                    // implementation consults IErrorInfo object and populates EXCEPINFO structure.
                    // From EXCEPINFO grid entry reads error code and message.
                    // IErrorInfo consulted too, but it does not hold error message anymore.
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
                    Assert.Fail("Invalid set values should produce ExternalException which will be presenttted to the user.");
                }
                catch (ExternalException ex)
                {
                    // If C++ implementation of Invoke did not populate EXCEPINFO structure
                    // from IErrorInfo, then we read that information about error call and display that error message to the user.
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
        var context = new ACTCTXW();
        HANDLE handle;
        fixed (char* p = applicationManifest)
        {
            context.cbSize = (uint)sizeof(ACTCTXW);
            context.lpSource = p;

            handle = PInvoke.CreateActCtx(&context);
        }

        if (handle == IntPtr.Zero)
        {
            throw new Win32Exception();
        }

        try
        {
            nuint cookie;
            if (!PInvoke.ActivateActCtx(handle, &cookie))
            {
                throw new Win32Exception();
            }

            try
            {
                action();
            }
            finally
            {
                if (!PInvoke.DeactivateActCtx(0, cookie))
                {
                    throw new Win32Exception();
                }
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
        Ole32.CoCreateInstance(in clsidRawErrorInfoUsageTest,
            IntPtr.Zero,
            CLSCTX.CLSCTX_INPROC_SERVER,
            in IID.GetRef<global::Windows.Win32.System.Com.IUnknown>(),
            out object result);
        return result;
    }

    private object CreateComObjectWithStandardIErrorInfoUsage()
    {
        Guid clsidStandardErrorInfoUsageTest = new("EA1FCB3A-277C-4C79-AB85-E2ED3E858201");
        Ole32.CoCreateInstance(in clsidStandardErrorInfoUsageTest,
            IntPtr.Zero,
            CLSCTX.CLSCTX_INPROC_SERVER,
            in IID.GetRef<global::Windows.Win32.System.Com.IUnknown>(),
            out object result);
        return result;
    }

    [DllImport("kernel32", SetLastError = true)]
    private static extern void ReleaseActCtx(IntPtr hActCtx);
}

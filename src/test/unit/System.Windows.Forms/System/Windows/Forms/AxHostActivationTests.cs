// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Tests;

[Collection("Sequential")] // workaround for WebBrowser control corrupting memory when run on multiple UI threads (instantiated via GUID)
public class AxHostActivationTests
{
    private const string WebBrowserClsidString = "8856f961-340a-11d0-a96b-00c04fd705a2";
    private const string UnregisteredClsidString = "5d0a1e3b-7c24-4f6e-9a8b-3e1f0c9d2b47";

    private const int REGDB_E_CLASSNOTREG = unchecked((int)0x80040154);
    private const int CLASS_E_NOTLICENSED = unchecked((int)0x80040112);
    private const int ERROR_BAD_EXE_FORMAT_HR = unchecked((int)0x800700C1);
    private const int E_FAIL = unchecked((int)0x80004005);

    // Scenario: a registered COM class (WebBrowser) is activated through AxHost on the STA UI thread.
    // The host passes the configured CLSID to CreateInstanceCore and exposes the native COM object through GetOcx.
    [WinFormsFact]
    public void AxHost_CreateControl_RegisteredClsid_ActivatesNativeOcxOnStaThread()
    {
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());

        using ActivationAxHost control = new(WebBrowserClsidString);
        Assert.Null(control.GetOcx());
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, control.CreateInstanceCoreCallCount);

        control.CreateControl();

        Assert.True(control.Created);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, control.CreateInstanceCoreCallCount);
        Assert.Equal(new Guid(WebBrowserClsidString), control.RequestedClsid);
        Assert.Equal(ApartmentState.STA, control.CreationApartment);

        object? ocx = control.GetOcx();
        Assert.NotNull(ocx);
        Assert.True(Marshal.IsComObject(ocx));
        Assert.Same(ocx, control.CreatedInstance);
    }

    // Scenario: calling CreateControl or reading Handle again on an already-activated host
    // must not create a second native object; the same OCX is reused.
    [WinFormsFact]
    public void AxHost_CreateControl_CalledRepeatedly_ReusesSameOcx()
    {
        using ActivationAxHost control = new(WebBrowserClsidString);
        control.CreateControl();
        object? ocx = control.GetOcx();
        Assert.NotNull(ocx);

        control.CreateControl();
        _ = control.Handle;

        Assert.Same(ocx, control.GetOcx());
        Assert.Equal(1, control.CreateInstanceCoreCallCount);
    }

    // Scenario: braced, undashed and whitespace-padded spellings of the same CLSID
    // all resolve to the same class when the host activates it.
    [WinFormsTheory]
    [InlineData("{8856F961-340A-11D0-A96B-00C04FD705A2}")]
    [InlineData("8856F961340A11D0A96B00C04FD705A2")]
    [InlineData("  8856f961-340a-11d0-a96b-00c04fd705a2  ")]
    public void AxHost_CreateControl_AlternateClsidFormats_ActivatesSameClass(string clsid)
    {
        using ActivationAxHost control = new(clsid);
        control.CreateControl();

        Assert.True(control.Created);
        Assert.Equal(new Guid(WebBrowserClsidString), control.RequestedClsid);
        Assert.NotNull(control.GetOcx());
    }

    // Scenario: disposing an activated host releases the native OCX reference and destroys the hosted window.
    // The `using` keeps cleanup in place if an assertion fails before the explicit Dispose.
    [WinFormsFact]
    public void AxHost_Dispose_AfterActivation_ReleasesOcxAndDestroysWindow()
    {
        using ActivationAxHost control = new(WebBrowserClsidString);
        control.CreateControl();
        HWND hwnd = (HWND)control.Handle;
        Assert.NotNull(control.GetOcx());
        Assert.True(PInvoke.IsWindow(hwnd));

        control.Dispose();

        Assert.Null(control.GetOcx());
        Assert.False(control.IsHandleCreated);
        Assert.False(PInvoke.IsWindow(hwnd));
    }

    // Scenario: a CLSID with no registered COM server fails explicitly with REGDB_E_CLASSNOTREG
    // and leaves the host unactivated; retrying fails the same way (the failure is not cached).
    [WinFormsFact]
    public void AxHost_CreateControl_UnregisteredClsid_ThrowsClassNotRegistered()
    {
        using ActivationAxHost control = new(UnregisteredClsidString);

        COMException exception = Assert.Throws<COMException>(control.CreateControl);

        Assert.Equal(REGDB_E_CLASSNOTREG, exception.HResult);
        AssertNotActivated(control);

        // The failure is not cached or swallowed: retrying fails the same way.
        exception = Assert.Throws<COMException>(control.CreateControl);
        Assert.Equal(REGDB_E_CLASSNOTREG, exception.HResult);
        AssertNotActivated(control);
        Assert.Equal(2, control.CreateInstanceCoreCallCount);
    }

    // Scenario: forcing the window handle of a host whose COM server is not registered
    // surfaces the same class-not-registered failure and leaves the host unactivated.
    [WinFormsFact]
    public void AxHost_Handle_UnregisteredClsid_ThrowsClassNotRegistered()
    {
        using ActivationAxHost control = new(UnregisteredClsidString);

        COMException exception = Assert.Throws<COMException>(() => control.Handle);

        Assert.Equal(REGDB_E_CLASSNOTREG, exception.HResult);
        AssertNotActivated(control);
    }

    // Scenario: native activation failures other than a missing license (class not registered, the
    // bad-executable-format error a wrong-bitness server DLL produces, a generic failure) reach the
    // caller with their original HRESULT. The failure is injected, so no real mismatched-bitness server is needed.
    [WinFormsTheory]
    [InlineData(REGDB_E_CLASSNOTREG)]
    [InlineData(ERROR_BAD_EXE_FORMAT_HR)]
    [InlineData(E_FAIL)]
    public void AxHost_CreateControl_ActivationFailure_PropagatesOriginalHResult(int hresult)
    {
        using ActivationAxHost control = new(WebBrowserClsidString)
        {
            CreateInstanceHandler = _ => throw new NativeActivationException("activation failed", hresult)
        };

        NativeActivationException exception = Assert.Throws<NativeActivationException>(control.CreateControl);

        Assert.Equal(hresult, exception.HResult);
        AssertNotActivated(control);
    }

    // Scenario: CLASS_E_NOTLICENSED from the COM server is reported as a LicenseException
    // that names the host type, and the host stays unactivated.
    [WinFormsFact]
    public void AxHost_CreateControl_NotLicensed_ThrowsLicenseException()
    {
        using ActivationAxHost control = new(WebBrowserClsidString)
        {
            CreateInstanceHandler = _ => throw new NativeActivationException("not licensed", CLASS_E_NOTLICENSED)
        };

        LicenseException exception = Assert.Throws<LicenseException>(control.CreateControl);

        Assert.Equal(typeof(ActivationAxHost), exception.LicensedType);
        AssertNotActivated(control);
    }

    // Scenario: an activation exception that is not a COM error is not translated or swallowed;
    // the same exception instance reaches the caller.
    [WinFormsFact]
    public void AxHost_CreateControl_NonComActivationException_Propagates()
    {
        InvalidOperationException expected = new();
        using ActivationAxHost control = new(WebBrowserClsidString)
        {
            CreateInstanceHandler = _ => throw expected
        };

        Assert.Same(expected, Assert.Throws<InvalidOperationException>(control.CreateControl));
        AssertNotActivated(control);
    }

    // Scenario: a transient activation failure does not leave the host in a state that blocks a later,
    // successful activation of the same host.
    [WinFormsFact]
    public void AxHost_CreateControl_ActivationFailureThenSuccess_ActivatesOnRetry()
    {
        using ActivationAxHost control = new(WebBrowserClsidString);
        control.CreateInstanceHandler = _ => throw new NativeActivationException("transient", E_FAIL);
        Assert.Throws<NativeActivationException>(control.CreateControl);
        AssertNotActivated(control);

        control.CreateInstanceHandler = null;
        control.CreateControl();

        Assert.True(control.Created);
        Assert.NotNull(control.GetOcx());
        Assert.Equal(2, control.CreateInstanceCoreCallCount);
    }

    private static void AssertNotActivated(ActivationAxHost control)
    {
        Assert.False(control.Created);
        Assert.False(control.IsHandleCreated);
        Assert.Null(control.GetOcx());
    }

    private sealed class NativeActivationException(string message, int errorCode) : ExternalException(message, errorCode);

    private class ActivationAxHost : AxHost
    {
        public ActivationAxHost(string clsid) : base(clsid)
        {
        }

        public int CreateInstanceCoreCallCount { get; private set; }

        public Guid RequestedClsid { get; private set; }

        public ApartmentState CreationApartment { get; private set; }

        public object? CreatedInstance { get; private set; }

        public Func<Guid, object?>? CreateInstanceHandler { get; set; }

        protected override object? CreateInstanceCore(Guid clsid)
        {
            CreateInstanceCoreCallCount++;
            RequestedClsid = clsid;
            CreationApartment = Thread.CurrentThread.GetApartmentState();

            CreatedInstance = CreateInstanceHandler is { } handler ? handler(clsid) : base.CreateInstanceCore(clsid);
            return CreatedInstance;
        }
    }
}

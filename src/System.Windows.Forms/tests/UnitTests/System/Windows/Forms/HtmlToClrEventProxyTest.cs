// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms.Tests.System.Windows.Forms;

public unsafe class HtmlToClrEventProxyTest
{
    [WinFormsFact]
    public void HtmlToClrEventProxy_EnumerateDispId_NamesExpected()
    {
        HtmlToClrEventProxy proxy = new("testEvent", (object sender, EventArgs e) => { });
        using var dispatchEx = ComHelpers.GetComScope<IDispatchEx>(proxy);

        // Requests that the object enumerates all of the elements on IDispatchEx
        uint fdexEnumAll = 2;
        dispatchEx.Value->GetNextDispID(fdexEnumAll, PInvokeCore.DISPID_UNKNOWN, out int id);
        Assert.Equal(0, id);
        using BSTR onHtmlEvent = default;
        dispatchEx.Value->GetMemberName(id, &onHtmlEvent);
        Assert.Equal("OnHtmlEvent", onHtmlEvent.ToString());

        dispatchEx.Value->GetNextDispID(fdexEnumAll, id, out id);
        Assert.Equal(65536, id);
        using BSTR eventName = default;
        dispatchEx.Value->GetMemberName(id, &eventName);
        Assert.Equal("EventName", eventName.ToString());

        dispatchEx.Value->GetNextDispID(fdexEnumAll, id, out id);
        Assert.Equal(65537, id);
        using BSTR getEventName = default;
        dispatchEx.Value->GetMemberName(id, &getEventName);
        Assert.Equal("get_EventName", getEventName.ToString());
    }

    [WinFormsFact]
    public void HtmlToClrEventProxy_PropFlags_Expected()
    {
        HtmlToClrEventProxy proxy = new("testEvent", (object sender, EventArgs e) => { });
        using var dispatchEx = ComHelpers.GetComScope<IDispatchEx>(proxy);

        FDEX_PROP_FLAGS methodFlags = FDEX_PROP_FLAGS.fdexPropCannotGet
            | FDEX_PROP_FLAGS.fdexPropCannotPut
            | FDEX_PROP_FLAGS.fdexPropCannotPutRef
            | FDEX_PROP_FLAGS.fdexPropCanCall
            | FDEX_PROP_FLAGS.fdexPropCannotConstruct
            | FDEX_PROP_FLAGS.fdexPropCannotSourceEvents;
        FDEX_PROP_FLAGS readPropertyFlags = FDEX_PROP_FLAGS.fdexPropCanGet
            | FDEX_PROP_FLAGS.fdexPropCannotPut
            | FDEX_PROP_FLAGS.fdexPropCannotPutRef
            | FDEX_PROP_FLAGS.fdexPropCannotCall
            | FDEX_PROP_FLAGS.fdexPropCannotConstruct
            | FDEX_PROP_FLAGS.fdexPropCannotSourceEvents;

        dispatchEx.Value->GetMemberProperties(0, uint.MaxValue, out FDEX_PROP_FLAGS flags);
        Assert.Equal(methodFlags, flags);
        dispatchEx.Value->GetMemberProperties(65536, uint.MaxValue, out flags);
        Assert.Equal(readPropertyFlags, flags);
        dispatchEx.Value->GetMemberProperties(65537, uint.MaxValue, out flags);
        Assert.Equal(methodFlags, flags);
    }

    [WinFormsFact]
    public void HtmlToClrEventProxy_InvokeAll()
    {
        string eventName = "testEvent";
        int count = 0;
        HtmlToClrEventProxy proxy = new(eventName, (object sender, EventArgs e) => count++);
        using var dispatch = ComHelpers.GetComScope<IDispatch>(proxy);

        VARIANT result = default;
        DISPPARAMS dispParams = default;
        uint locale = PInvokeCore.GetThreadLocale();
        HRESULT hr = dispatch.Value->Invoke(0, IID.NULL(), locale, DISPATCH_FLAGS.DISPATCH_METHOD, &dispParams, &result, default, default);
        Assert.True(hr.Succeeded);
        Assert.Equal(1, count);

        hr = dispatch.Value->Invoke(65536, IID.NULL(), locale, DISPATCH_FLAGS.DISPATCH_PROPERTYGET, &dispParams, &result, default, default);
        Assert.True(hr.Succeeded);
        Assert.Equal(eventName, (string)result.ToObject());

        hr = dispatch.Value->Invoke(65536, IID.NULL(), locale, DISPATCH_FLAGS.DISPATCH_PROPERTYPUT, &dispParams, &result, default, default);
        Assert.True(hr.Failed);

        hr = dispatch.Value->Invoke(65537, IID.NULL(), locale, DISPATCH_FLAGS.DISPATCH_METHOD, &dispParams, &result, default, default);
        Assert.True(hr.Succeeded);
        Assert.Equal(eventName, (string)result.ToObject());
    }
}

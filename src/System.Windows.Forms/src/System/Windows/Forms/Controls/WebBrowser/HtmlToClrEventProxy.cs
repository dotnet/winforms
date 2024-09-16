// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms;

/// <summary>
///  This class is here for IHTML*3.AttachHandler style eventing.
///  We need a way of routing requests for DISPID(0) to a particular CLR event without creating
///  a public class. In order to accomplish this we implement IReflect and handle InvokeMethod
///  to call back on a CLR event handler.
/// </summary>
internal class HtmlToClrEventProxy : UnknownDispatch, IManagedWrapper<IDispatch, IDispatchEx>
{
    private const int EventNamePropertyDispId = 65536;
    private const string EventNameProperty = "EventName";
    private const int GetEventNameDispId = 65537;
    private const string GetEventName = "get_EventName";
    private const int OnHtmlEventDispId = 0;
    private const string OnHtmlEventName = "OnHtmlEvent";

    private readonly EventHandler _eventHandler;
    private readonly string _eventName;

    public HtmlToClrEventProxy(string eventName, EventHandler eventHandler)
    {
        _eventHandler = eventHandler;
        _eventName = eventName;
    }

    public string EventName => _eventName;

    [DispId(OnHtmlEventDispId)]
    public void OnHtmlEvent()
    {
        InvokeClrEvent();
    }

    private void InvokeClrEvent()
    {
        _eventHandler?.Invoke(null, EventArgs.Empty);
    }

    protected override unsafe HRESULT GetDispID(BSTR bstrName, uint grfdex, int* pid)
    {
        if (pid is null)
        {
            return HRESULT.E_POINTER;
        }

        switch (bstrName.ToString())
        {
            case EventNameProperty:
                *pid = EventNamePropertyDispId;
                return HRESULT.S_OK;
            case GetEventName:
                *pid = GetEventNameDispId;
                return HRESULT.S_OK;
            case OnHtmlEventName:
                *pid = OnHtmlEventDispId;
                return HRESULT.S_OK;
            default:
                *pid = PInvokeCore.DISPID_UNKNOWN;
                return HRESULT.DISP_E_UNKNOWNNAME;
        }
    }

    protected override unsafe HRESULT GetNextDispID(uint grfdex, int id, int* pid)
    {
        if (pid is null)
        {
            return HRESULT.E_POINTER;
        }

        switch (id)
        {
            case PInvokeCore.DISPID_UNKNOWN:
                *pid = OnHtmlEventDispId;
                return HRESULT.S_OK;
            case OnHtmlEventDispId:
                *pid = EventNamePropertyDispId;
                return HRESULT.S_OK;
            case EventNamePropertyDispId:
                *pid = GetEventNameDispId;
                return HRESULT.S_OK;
            default:
                *pid = PInvokeCore.DISPID_UNKNOWN;
                return HRESULT.S_FALSE;
        }
    }

    protected override unsafe HRESULT GetMemberName(int id, BSTR* pbstrName)
    {
        if (pbstrName is null)
        {
            return HRESULT.E_POINTER;
        }

        switch (id)
        {
            case OnHtmlEventDispId:
                *pbstrName = new(OnHtmlEventName);
                return HRESULT.S_OK;
            case EventNamePropertyDispId:
                *pbstrName = new(EventNameProperty);
                return HRESULT.S_OK;
            case GetEventNameDispId:
                *pbstrName = new(GetEventName);
                return HRESULT.S_OK;
            default:
                *pbstrName = default;
                return HRESULT.DISP_E_UNKNOWNNAME;
        }
    }

    protected override HRESULT GetMemberProperties(int dispId, out FDEX_PROP_FLAGS properties)
    {
        switch (dispId)
        {
            case OnHtmlEventDispId:
                properties = IDispatch.GetMethodFlags();
                return HRESULT.S_OK;
            case EventNamePropertyDispId:
                properties = IDispatch.GetPropertyFlags(canRead: true, canWrite: false);
                return HRESULT.S_OK;
            case GetEventNameDispId:
                properties = IDispatch.GetMethodFlags();
                return HRESULT.S_OK;
            default:
                properties = default;
                return HRESULT.DISP_E_UNKNOWNNAME;
        }
    }

    protected override unsafe HRESULT Invoke(int dispId, uint lcid, DISPATCH_FLAGS flags, DISPPARAMS* parameters, VARIANT* result, EXCEPINFO* exceptionInfo, uint* argumentError)
    {
        switch (dispId)
        {
            case OnHtmlEventDispId:
                OnHtmlEvent();
                if (result is not null)
                {
                    *result = VARIANT.Empty;
                }

                return HRESULT.S_OK;
            case EventNamePropertyDispId:
                if (flags.HasFlag(DISPATCH_FLAGS.DISPATCH_PROPERTYGET))
                {
                    if (result is null)
                    {
                        return HRESULT.DISP_E_PARAMNOTOPTIONAL;
                    }

                    *result = (VARIANT)EventName;
                    return HRESULT.S_OK;
                }

                return HRESULT.DISP_E_MEMBERNOTFOUND;
            case GetEventNameDispId:
                if (result is null)
                {
                    return HRESULT.DISP_E_PARAMNOTOPTIONAL;
                }

                *result = (VARIANT)EventName;
                return HRESULT.S_OK;
            default:
                return HRESULT.DISP_E_MEMBERNOTFOUND;
        }
    }
}

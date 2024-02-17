#pragma once

#ifndef STRICT
#define STRICT
#endif

#define _USRDLL
#define _ATL_APARTMENT_THREADED
#define _ATL_NO_AUTOMATIC_NAMESPACE
#define _ATL_CSTRING_EXPLICIT_CONSTRUCTORS
#define ATL_NO_ASSERT_ON_DESTROY_NONEXISTENT_WINDOW

#include <atlstr.h>
#include <atlbase.h>
#include <atlctl.h>

#include <Contract.h>

using namespace ATL;

class ATL_NO_VTABLE CAxTestControl :
    public CComObjectRootEx<CComSingleThreadModel>,
    public CStockPropImpl<CAxTestControl, IAxTestControl>,
    public IPersistStreamInitImpl<CAxTestControl>,
    public IOleControlImpl<CAxTestControl>,
    public IOleObjectImpl<CAxTestControl>,
    public IOleInPlaceActiveObjectImpl<CAxTestControl>,
    public IViewObjectExImpl<CAxTestControl>,
    public IOleInPlaceObjectWindowlessImpl<CAxTestControl>,
    public IConnectionPointContainerImpl<CAxTestControl>,
    public IConnectionPointImpl<CAxTestControl, &__uuidof(_IAxTestControlEvents)>,
    public CComCoClass<CAxTestControl, &CLSID_AxTestControl>,
    public CComControl<CAxTestControl>
{
public:
    CContainedWindow m_ctlButton;
    CComBSTR m_bstrText;
    DWORD m_count;

    CAxTestControl();
    HRESULT FinalConstruct();
    void FinalRelease();

    DECLARE_PROTECT_FINAL_CONSTRUCT()
    DECLARE_NO_REGISTRY()

    DECLARE_OLEMISC_STATUS(0
        | OLEMISC_RECOMPOSEONRESIZE
        | OLEMISC_ACTSLIKEBUTTON
        | OLEMISC_CANTLINKINSIDE
        | OLEMISC_INSIDEOUT
        | OLEMISC_ACTIVATEWHENVISIBLE
        | OLEMISC_SETCLIENTSITEFIRST
    )

    BEGIN_COM_MAP(CAxTestControl)
        COM_INTERFACE_ENTRY(IAxTestControl)
        COM_INTERFACE_ENTRY(IDispatch)
        COM_INTERFACE_ENTRY(IViewObjectEx)
        COM_INTERFACE_ENTRY(IViewObject2)
        COM_INTERFACE_ENTRY(IViewObject)
        COM_INTERFACE_ENTRY(IOleInPlaceObjectWindowless)
        COM_INTERFACE_ENTRY(IOleInPlaceObject)
        COM_INTERFACE_ENTRY2(IOleWindow, IOleInPlaceObjectWindowless)
        COM_INTERFACE_ENTRY(IOleInPlaceActiveObject)
        COM_INTERFACE_ENTRY(IOleControl)
        COM_INTERFACE_ENTRY(IOleObject)
        COM_INTERFACE_ENTRY(IPersistStreamInit)
        COM_INTERFACE_ENTRY2(IPersist, IPersistStreamInit)
        COM_INTERFACE_ENTRY(IConnectionPointContainer)
    END_COM_MAP()

    BEGIN_PROP_MAP(CAxTestControl)
        PROP_DATA_ENTRY("_cx", m_sizeExtent.cx, VT_UI4)
        PROP_DATA_ENTRY("_cy", m_sizeExtent.cy, VT_UI4)
        PROP_ENTRY_TYPE("Text", DISPID_TEXT, CLSID_NULL, VT_BSTR)
    END_PROP_MAP()

    BEGIN_CONNECTION_POINT_MAP(CAxTestControl)
        CONNECTION_POINT_ENTRY(__uuidof(_IAxTestControlEvents))
    END_CONNECTION_POINT_MAP()

    BEGIN_MSG_MAP(CAxTestControl)
        MESSAGE_HANDLER(WM_CREATE, OnCreate)
        MESSAGE_HANDLER(WM_SETFOCUS, OnSetFocus)
        COMMAND_CODE_HANDLER(BN_CLICKED, OnButtonClicked)
        CHAIN_MSG_MAP(CComControl<CAxTestControl>)
        ALT_MSG_MAP(1)
    END_MSG_MAP()

    LRESULT OnSetFocus(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled);
    LRESULT OnCreate(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled);
    STDMETHOD(SetObjectRects)(LPCRECT prcPos, LPCRECT prcClip);

    // IViewObjectEx
    DECLARE_VIEW_STATUS(VIEWSTATUS_SOLIDBKGND | VIEWSTATUS_OPAQUE)

    // IAxTestControl
    void OnTextChanged();
    LRESULT OnButtonClicked(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled);
    HRESULT Fire_OnTextChanged(BSTR text);
    HRESULT Fire_OnButtonClick(LONG count);
    HRESULT Fire_OnClick();
};
